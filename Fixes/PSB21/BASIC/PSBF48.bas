\*******************************************************************************
\*******************************************************************************
\***
\***
\***            MODULE          :       PSBF48
\***            AUTHOR          :       Mark Goode
\***            DATE WRITTEN    :       October 2010
\***
\*******************************************************************************
\***
\***    VERSION A.           Mark Goode             15th October 2010
\***    Initial version (Release 1 / 2).
\***
\***    VERSION B.           Mark Goode             31st January 2011
\***    This version includes the service events logging requirements (Release 3).
\***    This relates to change request - CR002
\***    Service Event log record format:-
\***       DECAPI.EVENT(MESSAGE ID, DATA, REASON CODE)
\***
\***    VERSION C.           Mark Goode             9th May 2011
\***    This version contains changes relating to DEC release 3 after system testing.
\***    Tidying up of the DEC API log file layout.
\***
\***    VERSION D.           Mark Goode             16th May 2011
\***    The current solution for housekeeping of the  DQ file causes issues for
\***    the DEC i.e. Process of exclusive locking in JAVA. Therefore, instead of waiting
\***    the API will check for a trigger file. If available do not write to DQ file until
\***    removed. To reduce the system impact on checking on trigger file, install a time period.
\***    House keeping defaults off (99999999). 
\***
\***    VERSION E.           Mark Goode             18th May 2011
\***    Include batching of real time messages. Now only open service event log
\***    when required
\***
\***    VERSION F.           Mark Goode              16th June 2011
\***    The first transaction of the day wakes up the DEC, which can take longer than
\***    the 1000ms timeout for check connection, therefore socket may be open but message
\***    not returned in time. MESSAGE.ARRAY changed to remove check connection message.
\***    Refresh time stamp on retry.
\***
\***    VERSION G.           Mark Goode              29th June 2011
\***    This version removes housekeeping code due to requirement change also
\***    resolves isssues found during testing phase.
\***
\***    VERSION H.           Mark Goode              6th July 2011
\***    Remove Payload time stamp and Housekeeping
\***
\***    VERSION I.           Mark Goode              18th October 2011
\***    Defect 1809 - Improve the error trapping to catch file lock from a
\***    java application. This will also resolve the API not attemping retry.
\***
\***    VERSION J.           Mark Goode              28th November 2011
\***    Resolved issue with commit not being sent in DECAPI.SEND
\***
\***    VERSION K.           Mark Walker              24th January 2012
\***    - Ignore close errors on the DQ file
\***    - Make lock error handling specific to the DQ file
\***    - Return OS error codes to calling program
\***    - Steer clear of doing anything with the DQ pointer file. It also
\***      uses a hardcoded session number already used by Sales Support.
\***
\***    VERSION L.           Mark Walker             15th February 2012
\***    Changed the DAY$ function to calculate the day of the week
\***    rather than using CONTTIME, as this function occasionally
\***    fails causing the DEC API to crash.
\***
\***    VERSION M.           Brian Greenfield        19th February 2013
\***    The queue filename is now DQ2nn.BIN. All references changed.
\***    In OPEN.DQ(), remove the code that specifies the logical node on  
\***    which to open/create the DEC queue file. For example, currently the   
\***    file opened would be ADXLXAAN::D:\ADX_UDT1\DQCE.BIN, but the logical 
\***    node can be removed i.e. D:\ADX_UDT1\DQ2CE.BIN. In addition, set the 
\***    the distribution to 1 (local)
\***    These changes are part of the FOD-226 DEC 2.0 project.
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\*******************************************************************************
\***
\***    Module Overview
\***    ---------------
\***
\***    This function is the DEC API for 4690 controller application. The API
\***    will allow access to the DEC for initialistaion, request and response, as
\***    well as, closing session.
\***
\***        Command:      
\***        Accepted calls to the DEC API
\***        DECAPI.INIT                  - Initialise calling application socket to DEC 
\***        DECAPI.CLOSE                 - Close socket connection to the DEC
\***        DECAPI.SEND                  - Send payload using message type to DEC
\***        DECAPI.RECV                  - Calling application requesting response from DEC
\***        DECAPI.RECV.RESPONSE.TIME    - calling application request response time from last response
\***        DECAPI.EVENT                 - Enables calling application to log Service Events
\***        DECAPI.COMMAND               - General commands to the DEC via socket
\***
\***        
\***         DEC API Return Codes
\***        ----------------------
\***         ACK 
\***           00
\***         NAK (INT)
\***           nn - Where nn is the error code
\***           01 - Time out
\***           02 - Could not connect (DEC - Socket Adapter)
\***           03 - No such message
\***           04 - Request Socket closed
\***           05 - Payload exceeds maximum length
\***           06 - Could not connect (Issue with Node Queue file)
\***           07 - API failed initialisation
\***           08 - Unknown event reason code
\***           09 - Housekeeping in progress
\***           0A - Message not sent
\***         NAK on DECAPI formatted as follows:
\***         NAKnn (ASCII)
\***           Where nn is the error code
\***                 00 - Response not yet available
\***                 01 - Time out
\***                 02 - Invalid response
\***                 03 - Incomplete message                         
\***                 04 - Response Socket closed          
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\***
\***    Function globals
\***
\*******************************************************************************

%INCLUDE PSBF01G.J86         ! Application Logging
%INCLUDE SOPTSDEC.J86        ! Store Options File
%INCLUDE DECCFDEC.J86        ! DEC Configuration file
%INCLUDE DECAPDEC.J86        ! Daily DEC API log file 
%INCLUDE DQDEC.J86           ! DQ queue files
%INCLUDE SERVLDEC.J86        ! Service event logs        ! BMG

%INCLUDE PSBF13G.J86         ! PSDATE
%INCLUDE PSBUSEG.J86         ! Chain parameters


\*******************************************************************************
\***
\***    Globals
\***
\*******************************************************************************

STRING GLOBAL    CURRENT.CODE$
STRING GLOBAL    FILE.OPERATION$
STRING GLOBAL    DECAPI.CLIENT$
STRING GLOBAL    ERR.CODE$                                                      ! KMW
STRING GLOBAL    ERR.MSG$                                                       ! KMW
INTEGER*1 GLOBAL DECAPI.MAX
INTEGER*2 GLOBAL CURRENT.REPORT.NUM%
INTEGER*4 GLOBAL TCPERRNO%

\*******************************************************************************
\***
\***    PSBF48 variables
\***
\*******************************************************************************

STRING CT.TIMEDATE.TABLE$
STRING CT.PARAMBLK$

STRING CRTLF$
STRING COMMA$
STRING PIPE$
STRING QUOTE$
STRING AMPERSAND$
STRING RECORD$
STRING STATUS$
STRING CLIENT.IP$
STRING NODE.ID$
STRING PRIMARY.DQ.EXTENSION$
STRING ALTERNATE.IP$
STRING ALTERNATE.DQ$
STRING PRIMARY.IP$
STRING RESPONSE.TIME
STRING SECONDARY.IP$
STRING DECCONF.RECORD$(1)
STRING SERVL.LOG.RECORD$(1)            ! BMG
STRING SELECTHANDLE$
STRING SECONDARY.DQ.EXTENSION$
STRING DEC.RESPONSE$
STRING SOCKET.MESSAGE.ARRAY$(1)        ! EMG
STRING TEST.RESPONSE$                  ! FMG

INTEGER*1 FALSE
INTEGER*1 NAK
INTEGER*1 FOUND
INTEGER*1 MESSAGE.MATCH
INTEGER*1 PHASE1.COMPLETED
INTEGER*1 DEC.LOGSTATUS
INTEGER*1 TRUE
INTEGER*1 ACK
INTEGER*1 PROBLEM.DQ
INTEGER*1 LOG.FILE.OPEN
INTEGER*1 SERVICE.FILE.OPEN            ! BMG
!INTEGER*1 DQ.OPENED                   ! KMW
INTEGER*1 WRITING.DETAIL
INTEGER*1 QUEUE.FILE                   ! BMG
INTEGER*1 HOUSE.KEEP.ACTIVE            ! DMG

INTEGER*2 DEC.PORT%                
INTEGER*2 DEC.SOCKET.TIMEOUT%
INTEGER*2 DEC.TRANSTATUS
INTEGER*2 DEC.SERVICE.LOG              ! BMG
INTEGER*2 I%, J%
INTEGER*2 LOOP%
INTEGER*2 SOCKET.RC%
INTEGER*2 RECORD.LENGTH%
INTEGER*2 REC.NUM%
INTEGER*2 SOCKET.HANDLE%
INTEGER*2 SOCKET.MESSAGE.COUNT%        ! EMG
INTEGER*4 TIME.IN.MS%                  ! BMG

INTEGER*4 DEC.HOUSEKEEP.TIME%          ! DMG


\*******************************************************************
\***
\***    External functions
\***
\********************************************************************

%INCLUDE BTCMEM.J86
%INCLUDE SOCKLIB.J86        ! CBASIC SOCKET LIBRIARY
%INCLUDE SOPTSEXT.J86       ! SOPTS
%INCLUDE DECCFEXT.J86       ! DEC Configuration file
%INCLUDE DECAPEXT.J86       ! Daily DEC API log file 
%INCLUDE DQEXT.J86          ! DQ queue files
%INCLUDE SERVLEXT.J86       ! Service events log files      ! BMG

%INCLUDE ADXSERVE.J86       ! ADXSERVE FUNCTION
%INCLUDE ADXFILE.J86        ! ADXFILES FUNCTION             ! DMG
!%INCLUDE ERRNH.J86                                         ! KMW
                                                            ! KMW
!*********************************************************  ! KMW
! This function coverts the contents of ERRN to a four   *  ! KMW
! byte hex string.                                       *  ! KMW
! This is accomplished by adding x'30' to each nibble,   *  ! KMW
! building a string, and packing the results.            *  ! KMW
! Example ERRN = 80abcdefh, errnhex$ = 80abcdef          *  ! KMW
!*********************************************************  ! KMW
! NOTE: Local copy of existing public function to avoid  *  ! KMW
! an additional depandancy for the DEC API               *  ! KMW
!*********************************************************  ! KMW
FUNCTION ERRNHEX$                                           ! KMW
STRING ERRNHEX$,ERRFX$                                      ! KMW
INTEGER*4 HX                                                ! KMW
INTEGER*2 S,THE.SUM                                         ! KMW
ERRFX$ = ""                                                 ! KMW
 HX = ERRN                                                  ! KMW
FOR S = 28 TO 0 STEP -4                                     ! KMW
!                                                           ! KMW
   ERRFX$ = ERRFX$ + CHR$((SHIFT(HX,S) AND 000Fh) + 48)     ! KMW
NEXT S                                                      ! KMW
   ERRNHEX$ = PACK$ (ERRFX$)                                ! KMW
END FUNCTION                                                ! KMW

\*****************************************************************************
\***
\***    FUCTION CONTTIME (START.TIME, PARMBLK$, TIMEDATE.TABLE$) EXTERNAL
\***
\***    IBM assembler routine to obtain system time from Flex OS.
\***
\***    Initialise the following variables, once only, at start of program:
\***    PARAMBLK$ = STRING$(20, CHR$(00H))
\***    TIMEDATE.TABLE$ = STRING$(12, CHR$(00H))
\***
\***    Then each call to CONTTIME returns a time in milli-seconds.  The time
\***    returned is the time since midnight, *LESS* START.TIME.
\***    By passing in 0 as an initial value the current time can be obtained
\***    as a starting point.  Then this value can be passed in to subsequent
\***    calls to obtain an elapsed time.
\***
\***    The actual values returned in TIME.DATE.TABLE$ may be of interest:
\***
\***    00 - Year LSB       CCH 
\***    01 - Year MSB       07H = 1996    (i.e. hex, NOT packed)
\***    02 - Month          0AH = October (i.e. hex, NOT packed)
\***    03 - Day            12H = 18th    (i.e. hex, NOT packed)
\***    04 - Time LSB
\***    05 - Time           Milliseconds since midnight
\***    06 - Time
\***    07 - Time MSB
\***    08 - Time Zone LSB  Minutes from Universal Coordinated Time!
\***    09 - Time Zone MSB
\***    0A - Day of Week    00H = Sunday ... 06H = Saturday
\***    0B - 00H
\***
\***


FUNCTION CONTTIME (START.TIME, PARMBLK$, TIMEDATE.TABLE$) EXTERNAL
    STRING PARMBLK$, TIMEDATE.TABLE$
    INTEGER*4 START.TIME, CONTTIME
END FUNCTION

\*****************************************************************************
\***
\***  routines
\***
\*****************************************************************************
\*****************************************************************************
\***
\***    MILLI.TIME% function
\***    Sets MILLI.TIME% to conroller time since midnight in milli-seconds.
\***    Integer format.
\***
\***..........................................................................

FUNCTION MILLI.TIME%

    INTEGER*4 MILLI.TIME%
    STRING CT.PARAMBLK$, CT.TIMEDATE.TABLE$

    CT.PARAMBLK$       = STRING$(20, CHR$(00H)) 
    CT.TIMEDATE.TABLE$ = STRING$(12, CHR$(00H))

    MILLI.TIME% = CONTTIME(0, CT.PARAMBLK$, CT.TIMEDATE.TABLE$)

END FUNCTION

\*****************************************************************************
\***
\***    MILLI.TIME% function
\***    Sets MILLI.TIME% to conroller time since midnight in milli-seconds.
\***    Integer format.
\***
\***..........................................................................

FUNCTION TRIM.STRING(TEXT$)

    STRING TRIM.STRING
    STRING TEXT$
    INTEGER*2 LEN%

    LEN% = LEN(TEXT$)

    WHILE 1=1

     IF MID$(TEXT$,LEN%,1) = " " THEN BEGIN
        TEXT$ = LEFT$(TEXT$,LEN%-1)
        LEN% = LEN% - 1
     ENDIF ELSE BEGIN
        GOTO DONE
     ENDIF

    WEND

DONE:
     TRIM.STRING = TEXT$

END FUNCTION

\***************************************************************************! LMW
\***    DAY.OF.WEEK                                                         ! LMW
\***************************************************************************! LMW
\***    Uses Zellers formula to calculate the day of the week               ! LMW
\***    for the date specified in the function parameter (in YYMMDD format) ! LMW
\***                                                                        ! LMW
\***    The basic mathematical formula is as follows:                       ! LMW
\***                                                                        ! LMW
\***        (     [ (m+1)26 ]       [ K ]   [ J ]      )                    ! LMW
\***    h = ( q + [---------] + K + [---] + [---] + 5J ) MOD 7              ! LMW
\***        (     [   10    ]       [ 4 ]   [ 4 ]      )                    ! LMW
\***                                                                        ! LMW
\***    where h = day of the week (0 = Saturday,...,6 = Friday)             ! LMW
\***          q = day of month                                              ! LMW
\***          m = month (3 = Mar, 4 = Apr,...,13 = Jan, 14 = Feb)           ! LMW
\***          K = the year of the century (e.g. if year 1985, K = 85)       ! LMW
\***          J = the century (e.g. if year is 1985, J = 19)                ! LMW
\***                                                                        ! LMW
\***    Note: There are a number of 'fudges' required for year 2000         ! LMW
\***          and also the months of January and February!                  ! LMW
\***                                                                        ! LMW
\***    The function returns an integer value where:                        ! LMW
\***        0 = Sun, 1 = Mon, 2 = Tue, 3 = Wed, 4 = Thu, 5 = Fri, 6 = Sat   ! LMW
\***                                                                        ! LMW
\************************************************************************** ! LMW
FUNCTION DAY.OF.WEEK(D$)                                                    ! LMW
                                                                            ! LMW
    STRING    D$                                                            ! LMW
    STRING    YEAR$                                                         ! LMW
    STRING    MONTH$                                                        ! LMW
    STRING    DAY$                                                          ! LMW
                                                                            ! LMW
    INTEGER*1 DAY.OF.WEEK                                                   ! LMW
    INTEGER*4 Q,D,H,J,K,Y,M,X                                               ! LMW
    INTEGER*4 N1,N2,N3,N4,N5,N6                                             ! LMW
                                                                            ! LMW
    YEAR$  = LEFT$(D$,2)                                                    ! LMW
    MONTH$ = MID$(D$,3,2)                                                   ! LMW
    DAY$   = RIGHT$(D$,2)                                                   ! LMW
                                                                            ! LMW
    Y = VAL(YEAR$)                                                          ! LMW
                                                                            ! LMW
    ! IF month is January or Feruary                                        ! LMW
    IF MONTH$ = "01" OR MONTH$ = "02" THEN BEGIN                            ! LMW
       Y = Y - 1                                                            ! LMW
    ENDIF                                                                   ! LMW
                                                                            ! LMW
    ! IF we are in the year 2000                                            ! LMW
    IF Y < 0 THEN BEGIN                                                     ! LMW
        Y = 99                                                              ! LMW
        J = 19                                                              ! LMW
    ENDIF ELSE BEGIN                                                        ! LMW
        ! If date is in current century                                     ! LMW
        IF Y < 85 THEN BEGIN                                                ! LMW
            J = 20                                                          ! LMW
        ENDIF ELSE BEGIN                                                    ! LMW
            J = 19                                                          ! LMW
        ENDIF                                                               ! LMW
    ENDIF                                                                   ! LMW
                                                                            ! LMW
    Q = VAL(DAY$)                                                           ! LMW
    M = MOD((VAL(MONTH$)+9),12)+3                                           ! LMW
    K = MOD(Y,100)                                                          ! LMW
                                                                            ! LMW
    X = 26*(M+1)                                                            ! LMW
                                                                            ! LMW
    N1 = Q                                                                  ! LMW
    N2 = INT(X/10)                                                          ! LMW
    N3 = K                                                                  ! LMW
    N4 = INT(K/4)                                                           ! LMW
    N5 = INT(J/4)                                                           ! LMW
    N6 = (5*J)                                                              ! LMW
                                                                            ! LMW
    X = N1+N2+N3+N4+N5+N6                                                   ! LMW
                                                                            ! LMW
    ! 0=Sunday..6=Saturday                                                  ! LMW
    H = MOD(X+6,7)                                                          ! LMW
                                                                            ! LMW
    DAY.OF.WEEK = H                                                         ! LMW
                                                                            ! LMW
END FUNCTION                                                                ! LMW

\*****************************************************************************
\***
\***    DAY function
\***    Returns day of week.
\***    00H = Sunday ... 06H = Saturday
\***
\***..........................................................................

FUNCTION DAY$

!!!!INTEGER*4 RC%                                                           ! LMW
    STRING  DAY$
!!!!STRING CT.PARAMBLK$, CT.TIMEDATE.TABLE$                                 ! LMW
    STRING  TODAYS.DATE$                                                    ! LMW

!!!!CT.PARAMBLK$       = STRING$(20, CHR$(00H))                             ! LMW
!!!!CT.TIMEDATE.TABLE$ = STRING$(12, CHR$(00H))                             ! LMW
                                                                            ! LMW
!!!!RC% = CONTTIME(0, CT.PARAMBLK$, CT.TIMEDATE.TABLE$)                     ! LMW
                                                                            ! LMW
!!!!DAY$ = MID$(CT.TIMEDATE.TABLE$,11,1)                                    ! LMW
                                                                            ! LMW
    TODAYS.DATE$ = DATE$                                                    ! LMW
                                                                            ! LMW
    DAY$ = CHR$(DAY.OF.WEEK(TODAYS.DATE$))                                  ! LMW

END FUNCTION

\*****************************************************************************
\***
\***    HHMMSS.MMM$ function
\***    Converts time (parmed as milli-seconds since midnight) into 
\***    an understandable HH:MM:SS.MMM format
\***
\***..........................................................................

FUNCTION HHMMSS.MMM$(VAL%)

    STRING    HHMMSS.MMM$
    STRING    WORK$
    INTEGER*4 VAL%

    WORK$ = RIGHT$("000" + STR$(MOD(VAL%, 1000)) ,3)
    VAL% = VAL% / 1000

    WORK$ = RIGHT$("00" + STR$(MOD(VAL%, 60)) ,2) + "." + WORK$
    VAL% = VAL% / 60

    WORK$ = RIGHT$("00" + STR$(MOD(VAL%, 60)) ,2) + ":" + WORK$
    VAL% = VAL% / 60

    WORK$ = RIGHT$("00" + STR$(VAL%), 2) + ":" + WORK$

    HHMMSS.MMM$ = WORK$

END FUNCTION

\*****************************************************************************
\***
\***    SOREC156
\***   
\***    Parser for SOPTS record 156
\***
\***


SUB SOREC156(IN$)

    INTEGER*2 NOS%
    INTEGER*2 NOS1%
    STRING    IN$

    ON ERROR GOTO ABORT.SUB

    NOS% = 1
    DEC.PORT% = VAL(LEFT$(IN$, MATCH(",",IN$,NOS%)-1))
    NOS1% = MATCH(",",IN$,MATCH(",",IN$,NOS%)+1)
    PRIMARY.IP$ = MID$(IN$, MATCH(",",IN$,1) + 1, (NOS1%-1) - MATCH(",",IN$,MATCH(",",IN$,1)))
    NOS%  = MATCH(",",IN$,MATCH(",",IN$,NOS1%)+1)
    SECONDARY.IP$ = MID$(IN$,MATCH(",",IN$,NOS1%) + 1,(NOS%-1) - MATCH(",",IN$,MATCH(",",IN$,NOS1%)))
    NOS1% = MATCH(",",IN$,MATCH(",",IN$,NOS%))
    DEC.TRANSTATUS  = VAL(MID$(IN$,MATCH(",",IN$,NOS%) + 1,(NOS1%-1) - MATCH(",",IN$,MATCH(",",IN$,1))))
    NOS%  = MATCH(",",IN$,MATCH(",",IN$,NOS1%)+1)
    DEC.LOGSTATUS  = VAL(MID$(IN$,MATCH(",",IN$,NOS%) + 1,(NOS1%-1) - MATCH(",",IN$,MATCH(",",IN$,1))))
    NOS1%  = MATCH(",",IN$,MATCH(",",IN$,NOS%)+1)
    DEC.SOCKET.TIMEOUT% = VAL(MID$(IN$,MATCH(",",IN$,NOS1%) + 1,(NOS%-1) - MATCH(",",IN$,MATCH(",",IN$,1))))
    NOS%  = MATCH(",",IN$,MATCH(",",IN$,NOS1%)+1)                                                               ! BMG
    DEC.SERVICE.LOG = VAL(MID$(IN$,MATCH(",",IN$,NOS%) + 1,(NOS1%-1) - MATCH(",",IN$,MATCH(",",IN$,1))))        ! BMG
    NOS1%  = MATCH(",",IN$,MATCH(",",IN$,NOS%)+1)
    DEC.HOUSEKEEP.TIME% = VAL(MID$(IN$,MATCH(",",IN$,NOS1%) + 1,(NOS%-1) - MATCH(",",IN$,MATCH(",",IN$,1))))    ! EMG
    ALTERNATE.IP$       =  SECONDARY.IP$


    EXIT SUB

ABORT.SUB:

END SUB

\*****************************************************************************                       ! BMG
\***                                                                                                 ! BMG
\***    MESSAGEID.MATCH                                                                              ! BMG
\***                                                                                                 ! BMG
\***    LOGGING                                                                                      ! BMG
\***      TYPE 0 - DEBUG LOGGING                                                                     ! BMG
\***      TYPE 1 - SERVICE EVENT LOGGING                                                             ! BMG
\***..........................................................................                       ! BMG
                                                                                                     ! BMG
 SUB MESSAGEID.MATCH(ARG.MESSAGEID$,SERVICE.EVENT)                                                   ! BMG
                                                                                                     ! BMG
    STRING TEST.MESSAGE.NAME$                                                                        ! BMG
    STRING ARG.MESSAGEID$                                                                            ! BMG
    INTEGER*1 SERVICE.EVENT                                                                          ! BMG
                                                                                                     ! BMG
    TEST.MESSAGE.NAME$ = ""                                                                          ! BMG
    DECCF.MSGNAME$     = ""                                                                          ! BMG
    DECCF.DELIVERY$    = ""                                                                          ! BMG
                                                                                                     ! BMG
    LOOP% = 0                                                                                        ! BMG
    WHILE NOT FOUND !AND DECCONF.RECORD$(LOOP%) <> ""                                                ! BMG
                                                                                                     ! BMG
        IF MATCH(UCASE$(ARG.MESSAGEID$),UCASE$(DECCONF.RECORD$(LOOP%)),1) > 0 THEN BEGIN             ! BMG
           TEST.MESSAGE.NAME$ = MID$(DECCONF.RECORD$(LOOP%),21,30)                                   ! BMG
           IF  DQ.ELEMENT% = 0 OR SERVICE.EVENT THEN BEGIN                                           ! BMG
               DECCF.MSGNAME$ = TRIM.STRING(TEST.MESSAGE.NAME$) + PIPE$                              ! BMG
               MESSAGE.MATCH = NAK                                                                   ! BMG
           ENDIF                                                                                     ! BMG
           IF MATCH(DECCF.MSGNAME$,DQ.ARRAY.RECORD$(0),1) = 0 AND NOT SERVICE.EVENT THEN BEGIN       ! BMG
              MESSAGE.MATCH = ACK                                                                    ! BMG
              DECCF.DELIVERY$ = MID$(DECCONF.RECORD$(LOOP%),54,1)                                    ! BMG
              DECCF.MSGNAME$ = TRIM.STRING(TEST.MESSAGE.NAME$) + PIPE$                               ! BMG
              FOUND = ACK                                                                            ! BMG
           ENDIF ELSE BEGIN                                                                          ! BMG
              MESSAGE.MATCH = ACK                                                                    ! BMG
              FOUND = ACK                                                                            ! BMG
           ENDIF                                                                                     ! BMG
        ENDIF ELSE BEGIN                                                                             ! BMG
           LOOP% = LOOP% + 1                                                                         ! BMG
        ENDIF                                                                                        ! BMG
                                                                                                     ! BMG
    WEND                                                                                             ! BMG
                                                                                                     ! BMG
END SUB                                                                                              ! BMG

\*****************************************************************************
\***
\***    DECAPI.LOG                                                                                   ! BMG
\***
\***    LOGGING                                                                                      ! BMG
\***      TYPE 0 - DEBUG LOGGING                                                                     ! BMG
\***      TYPE 1 - SERVICE EVENT LOGGING                                                             ! BMG
\***
\***..........................................................................                       ! KMW

SUB DECAPI.LOG(LOG$,TYPE%)                                                                           ! BMG

    STRING    LOG$
    STRING    D$                    
    INTEGER*1 DECAPI.LOG
    INTEGER*1 RC%    
    INTEGER*2 TYPE%                                                                                  ! BMG
    INTEGER*4 TIME.CHECK%                                                                            ! BMG

    D$ = RIGHT$(DATE$,2)+"-"+MID$(DATE$,3,2)+"-"+LEFT$(DATE$,2)+" "+HHMMSS.MMM$(MILLI.TIME%)         ! BMG
          
    IF DEC.LOGSTATUS AND LOG.FILE.OPEN AND TYPE% = 0 THEN BEGIN                                      ! GMG ! BMG
       ! Logging debug data                                                                          ! BMG
       D$ = D$ + " - " + LOG$                                                                        ! BMG
       DECAP.RECORD$ = D$                                                                            ! BMG
       RC% = WRITE.DECAP                                                                             ! BMG
    ENDIF ELSE IF DEC.SERVICE.LOG AND SERVICE.FILE.OPEN AND TYPE% = 1 THEN BEGIN                     ! GMG ! BMG
       ! Logging service event data                                                                  ! GMG
       SERVL.ARRAY.RECORD$(SERVL.ELEMENT%) = D$ + PIPE$ + LOG$                                       ! BMG
       RC% = WRITE.MATRIX.SERVL                                                                      ! BMG
       SERVL.ELEMENT% = 0                                                                            ! BMG
       DIM SERVL.ARRAY.RECORD$(1000)                                                                 ! BMG
    ENDIF

END SUB

\*****************************************************************************
\***
\***    OPEN.DQ
\***   
\***    Open correct DQ file depending on node
\***
\***..........................................................................

SUB OPEN.DQ(NAME$)

    STRING NAME$
    STRING NODE$
    STRING DIST$
    STRING TEMP$

   ON ERROR GOTO ABORT.FUNCTION

   PROBLEM.DQ = -1

!!!!IF DQ.OPENED THEN BEGIN                                                     ! KMW
   CLOSE DQ.SESS.NUM%
!!!!!!!DQ.OPENED = NAK                                                          ! KMW
!!!!ENDIF                                                                       ! KMW

    DQ.FILE.NAME$ = NAME$

!!! Remove code which uses logical node and replace with OPEN code              ! MBG
!!! below opens the local copy of the file                                      ! MBG
!!! IF MATCH("CE",NAME$,1) = 0 THEN BEGIN                                       ! MBG
!!!    NODE$ = "ADXLXACN::"                                                     ! MBG
!!!    IF END # DQ.SESS.NUM% THEN CREATE.DQ                                     ! MBG
!!!    OPEN NODE$ + NAME$ AS DQ.SESS.NUM% NOREAD NODEL APPEND                   ! MBG
!!! ENDIF ELSE BEGIN                                                            ! MBG
!!!    NODE$ = "ADXLXAAN::"                                                     ! MBG
!!!    IF END # DQ.SESS.NUM% THEN CREATE.DQ                                     ! MBG
!!!    OPEN NODE$ + NAME$ AS DQ.SESS.NUM% NOREAD NODEL APPEND                   ! MBG
!!! ENDIF                                                                       ! MBG
    IF END # DQ.SESS.NUM% THEN CREATE.DQ                                        ! MBG
    OPEN NAME$ AS DQ.SESS.NUM% NOREAD NODEL APPEND                              ! MBG
    GOTO SKIP.DQ                                                                ! MBG
     
    CREATE.DQ:
    CREATE POSFILE NAME$ AS DQ.SESS.NUM% LOCAL                                  ! MBG
    
!!!!IF MATCH("CF",NAME$,1) > 0 THEN BEGIN                                                       ! MBG
!!!!!!!CREATE POSFILE NODE$ + NAME$ AS DQ.SESS.NUM% MIRRORED PERUPDATE                          ! MBG
!!!!!!!CREATE POSFILE NODE$ + LEFT$(NAME$,2) + "CF" + RIGHT$(NAME$,6) AS 80 MIRRORED PERUPDATE  ! KMW
!!!!!!!CLOSE 80                                                                                 ! KMW
!!!!ENDIF ELSE BEGIN
!!!!!!!CREATE POSFILE NODE$ + NAME$ AS DQ.SESS.NUM% COMPOUND PERUPDATE                          ! MBG
!!!!!!!CREATE POSFILE NODE$ + LEFT$(NAME$,2) + "CF" + RIGHT$(NAME$,6) AS 80 COMPOUND PERUPDATE  ! KMW
!!!!!!!CLOSE 80                                                                                 ! KMW
!!!!ENDIF                                                                                       ! MBG

SKIP.DQ:

    PROBLEM.DQ = 0
!!!!DQ.OPENED = ACK                                                             ! KMW

LEAVE.SUB:

    EXIT SUB
    
 ABORT.FUNCTION:

    ! We don't care about close errors on the DQ file                           ! KMW
    IF ERRF% = 1003 AND ERR = "CU" THEN RESUME                                  ! KMW

    IF ERR = "*I" THEN BEGIN
       PROBLEM.DQ = -1                                                          ! BMG
    ENDIF ELSE BEGIN
        PROBLEM.DQ = 0
!!!!!!!!DQ.OPENED = ACK                                                         ! KMW
    ENDIF

    RESUME LEAVE.SUB
    
END SUB

\*****************************************************************************                          ! EMG
\***                                                                                                    ! EMG
\***    MESSAGE.ARRAY                                                                                   ! EMG
\***                                                                                                    ! EMG
\***    Process latest response and add to message array                                                ! EMG
\***                                                                                                    ! EMG
\***..........................................................................                          ! EMG
                                                                                                        ! EMG
FUNCTION MESSAGE.ARRAY(DEC.RESPONSE$)                                                                   ! EMG
                                                                                                        ! EMG
    STRING    DEC.RESPONSE$                                                                             ! EMG
    STRING    MESSAGE.ARRAY                                                                             ! EMG
    STRING    TEMP.MESSAGE.ARRAY$                                                                       ! EMG
    INTEGER*2 OFFSET%                                                                                   ! EMG
    INTEGER*2 COUNT%                                                                                    ! EMG
    INTEGER*2 REMAIN%                                                                                   ! EMG
    INTEGER*2 LOC%                                                                                      ! EMG
                                                                                                        ! EMG
    ON ERROR GOTO ABORT.FUNCTION                                                                        ! EMG
                                                                                                        ! EMG
    IF DEC.RESPONSE$ <> "NAK" THEN BEGIN                                                                ! EMG
                                                                                                        ! EMG
       ! If found remove late check connection response from DEC.RESPONSE$                              ! FMG
       IF MATCH(TEST.RESPONSE$,DEC.RESPONSE$,1) THEN BEGIN                                              ! FMG
          DEC.RESPONSE$ = RIGHT$(DEC.RESPONSE$,LEN(DEC.RESPONSE$) - LEN(TEST.RESPONSE$))                ! FMG
       ENDIF                                                                                            ! FMG
       !Add responseS to array                                                                          ! EMG
       OFFSET% = 1                                                                                      ! EMG
                                                                                                        ! EMG
       LOC% = MATCH(CRTLF$,DEC.RESPONSE$,OFFSET%)                                                       ! EMG
                                                                                                        ! EMG
       WHILE MATCH(CRTLF$,DEC.RESPONSE$,OFFSET%)                                                        ! EMG
          SOCKET.MESSAGE.ARRAY$(SOCKET.MESSAGE.COUNT%) = MID$(DEC.RESPONSE$,OFFSET%,LOC% - OFFSET%)     ! EMG
          SOCKET.MESSAGE.COUNT% = SOCKET.MESSAGE.COUNT% + 1                                             ! EMG
          OFFSET% = LOC% + 2                                                                            ! EMG
          LOC% = MATCH(CRTLF$,DEC.RESPONSE$,OFFSET%)                                                    ! EMG
       WEND                                                                                             ! EMG
                                                                                                        ! EMG
    ENDIF                                                                                               ! EMG
                                                                                                        ! EMG
    MESSAGE.ARRAY = RIGHT$(SOCKET.MESSAGE.ARRAY$(0),LEN(SOCKET.MESSAGE.ARRAY$(0)) - 8)                  ! EMG
    RESPONSE.TIME = LEFT$(SOCKET.MESSAGE.ARRAY$(0),8)                                                   ! EMG
                                                                                                        ! EMG
    REMAIN% = SOCKET.MESSAGE.COUNT%                                                                     ! EMG
                                                                                                        ! EMG
    IF SOCKET.MESSAGE.ARRAY$(1) <> "" THEN BEGIN                                                        ! EMG
                                                                                                        ! EMG
       FOR COUNT% = 0 TO SOCKET.MESSAGE.COUNT% - 2                                                      ! EMG
           SOCKET.MESSAGE.ARRAY$(COUNT%) = SOCKET.MESSAGE.ARRAY$(COUNT% + 1)                            ! EMG
           SOCKET.MESSAGE.ARRAY$(COUNT% + 1) = ""                                                       ! EMG
       NEXT                                                                                             ! EMG
       SOCKET.MESSAGE.COUNT% = SOCKET.MESSAGE.COUNT% - 1                                                ! EMG
    ENDIF ELSE BEGIN                                                                                    ! EMG
       SOCKET.MESSAGE.COUNT%    = 0                                                                     ! EMG
       SOCKET.MESSAGE.ARRAY$(0) = ""                                                                    ! EMG
    ENDIF                                                                                               ! EMG
                                                                                                        ! EMG
    CALL DECAPI.LOG(STR$(REMAIN%) + " MESSAGES AVAILABLE IN API QUEUE",0)                               ! EMG
                                                                                                        ! EMG
LEAVE.FUNCTION:                                                                                         ! EMG
                                                                                                        ! EMG
    EXIT FUNCTION                                                                                       ! EMG
                                                                                                        ! EMG
ABORT.FUNCTION:                                                                                         ! EMG
                                                                                                        ! EMG
    !Corrupt data - clean up                                                                            ! EMG
    SOCKET.MESSAGE.COUNT%    = 0                                                                        ! EMG
    DIM SOCKET.MESSAGE.ARRAY$(1000)                                                                     ! EMG
                                                                                                        ! EMG
    RESUME LEAVE.FUNCTION                                                                               ! EMG
                                                                                                        ! EMG
END FUNCTION                                                                                            ! EMG


\*****************************************************************************
\***
\***    CHECK.PAYLOAD.AND.TRANSLATE
\***
\***    Check and translate payload
\***
\***..........................................................................


FUNCTION CHECK.PAYLOAD.AND.TRANSLATE(A$,B$,C$)

    STRING CHECK.PAYLOAD.AND.TRANSLATE
    STRING A$
    STRING B$
    STRING C$
    INTEGER*2 NUM%
    INTEGER*2 LOC%

    NUM% = 1
    LOC% = MATCH(B$,A$,NUM%) 
    !Translate B$ to C$;
    WHILE LOC% <> 0 

       A$ = LEFT$(A$,LOC%-1) + C$ + RIGHT$(A$,len(A$)-((LOC%-1)+LEN(B$)))
       NUM% = LOC% + 1
       LOC% = MATCH(B$,A$,NUM%)
       
    WEND

    CHECK.PAYLOAD.AND.TRANSLATE = A$

END FUNCTION


\********************************************************************
\***
\***   DEC.CONNECT
\***   
\***   Connect client to DEC via socket
\***
\********************************************************************

FUNCTION DEC.CONNECT(IP$,P%, TO%)

    STRING    IP$
    STRING    TEMP$
    STRING    TEST$
    INTEGER*2 SH%
    INTEGER*2 RC%
    INTEGER*2 P%
    INTEGER*2 TO%
    INTEGER*2 DEC.CONNECT

    DEC.CONNECT = 1
    RC% = SOCK.INIT(1, 32767)
    SH% = SOCK.SOCKET(2,1,0)

  IF SH% <> -1 THEN BEGIN
     !Setup Non-blocking mode i.e Do not wait for response from socket
     CALL SOCK.DONT.BLOCK(SH%)

     !Connect to remote host (Time out read from EALSOPTS record 156)
     SOCKET.RC% = SOCK.CONNECT(SH%,IP$,P%)

     !Socket operation in progress
     IF SOCKET.RC% = -1 AND TCPERRNO% = 024H THEN BEGIN
        !
        TEMP$ = "00"
        CALL PUTN2(TEMP$, 0, SH%)
        ! Check if the socket is ready for use
        CALL SOCK.SELECT(TEMP$, 0, 1, 0, TO%, RC%)
        CALL SOCK.RECV(SH%,TEST$,0,RC%)
        IF RC% = 0 OR (RC% = -1 AND (TCPERRNO% = 3DH OR TCPERRNO% = 39H ))  THEN BEGIN
           ! Socket connection timed out on handle or connection failure. Abort socket and try secondary IP 
           DEC.CONNECT = RC%
           RC% = SOCK.ABORT(SOCKET.HANDLE%) 
           EXIT FUNCTION
        ENDIF

     ENDIF

  ENDIF

    DEC.CONNECT = SH%

END FUNCTION

\*****************************************************************************
\***
\***    FLEXOSSVC
\***   
\***    FLEXOS FUNCTION 
\***
\***..........................................................................


 FUNCTION FLEXOSSVC(COMMAND%, PARAM$) EXTERNAL
      STRING PARAM$
      INTEGER*2 COMMAND%, FLEXOSSVC
END FUNCTION

\*****************************************************************************
\***
\***    PROCESS.NAME$
\***   
\***    Get the calling applications name 
\***
\***..........................................................................

FUNCTION PROCESS.NAME$

    STRING    SVC.PARAM$
    STRING    PROCESS.TABLE$
    STRING    PROCESS.NAME$
    STRING    TEMP$
    STRING    NAME$
    INTEGER*2 LOOP%
    INTEGER*4 TABLE.SIZE%
    
    ON ERROR GOTO ABORT.FUNCTION                                                                                                         ! GMG
    
    SVC.PARAM$ = STRING$(28,"0")
    PROCESS.TABLE$ = ""
    TEMP$ = STRING$(60, CHR$(00H))
    FOR LOOP% = 1 TO 200
       PROCESS.TABLE$ = PROCESS.TABLE$ + TEMP$
    NEXT
    TABLE.SIZE% = LEN(PROCESS.TABLE$)

    CALL PUTN4(SVC.PARAM$,  0, 00000000H)
    CALL PUTN4(SVC.PARAM$,  4, 00000000H)
    CALL PUTN4(SVC.PARAM$,  8, 00000000H)
    CALL PUTN4(SVC.PARAM$, 12, SADD(PROCESS.TABLE$) + 2)
    CALL PUTN4(SVC.PARAM$, 16, TABLE.SIZE%)

    \  REM Execute GET SVC
    CALL FLEXOSSVC(0, SVC.PARAM$)

    NAME$ =  MID$(PROCESS.TABLE$,9,8) 

    IF MATCH(PACK$("00"),NAME$,1) THEN BEGIN                                                                                             ! GMG
       NAME$ =  LEFT$(NAME$,MATCH(PACK$("00"),NAME$,1)-1)                                                                                ! GMG
    ENDIF                                                                                                                                ! GMG

    PROCESS.NAME$ = NAME$                                                                                                                ! GMG

LEAVE.FUNCTION:                                                                                                                          ! GMG

    EXIT FUNCTION                                                                                                                        ! GMG

ABORT.FUNCTION:                                                                                                                          ! GMG

    PROCESS.NAME$ = ""                                                                                                                   ! GMG
    RESUME LEAVE.FUNCTION                                                                                                                ! GMG

END FUNCTION

\*******************************************************************************
\***
\***    SOCKET.INITIALISATION
\***
\*******************************************************************************

SUB SOCKET.INITIALISATION(LOST)

    STRING    A.IP$
    STRING    A.DQ$
    STRING    FIELD$
    STRING    ADX.DATA$
    STRING    TEST$
    INTEGER*2 NO%
    INTEGER*2 ADX.RC%
    INTEGER*2 LOST

    ! initialise variable and open files for socket initialisation

    NAK   = 0
    ACK   =  -1
    FOUND = NAK                                                                 
!!!!DQ.OPENED = NAK                                                             ! KMW

    CRTLF$     = CHR$(0DH) + CHR$(0AH)
    COMMA$     = CHR$(2CH)
    QUOTE$     = CHR$(22H)
    AMPERSAND$ = CHR$(26H)
    PIPE$      = CHR$(07CH)

    TEST.RESPONSE$  = "NAKSA003,Check Connection,Check Connection" + CRTLF$                                                              ! FMG

    !------------------------------
    ! Initialise CONTTIME variables
    !------------------------------

    CT.PARAMBLK$       = STRING$(20, CHR$(00H))
    CT.TIMEDATE.TABLE$ = STRING$(12, CHR$(00H))



    IF PHASE1.COMPLETED = 0 THEN BEGIN

        IF NOT LOST THEN BEGIN 
           DIM DECCONF.RECORD$(100)
           DIM SERVL.ARRAY.RECORD$(1000)                                                                                                 ! BMG
           DIM DQ.ARRAY.RECORD$(1000)
           DIM SOCKET.MESSAGE.ARRAY$(1000)                                                                                               ! EMG

           SOCKET.MESSAGE.COUNT% = 0                                                                                                     ! EMG
           DQ.ELEMENT%           = 0                                                                                                    
           SERVL.ELEMENT%        = 0                                                                                                     ! BMG   
        ENDIF

        CALL ADXSERVE(ADX.RC%,                    \
                     4,                           \
                     0,                           \
                     ADX.DATA$)

        NODE.ID$ = MID$(ADX.DATA$,14,2)
        
       IF DECAPI.CLIENT$ = "" THEN BEGIN                                                                                                 ! BMG
          DECAPI.CLIENT$ = PROCESS.NAME$                                                                                                 ! BMG
       ENDIF                                                                                                                             ! BMG

        IF DECCF.REPORT.NUM%   = 0 THEN CALL DECCF.SET
        IF DECAP.REPORT.NUM%   = 0 THEN CALL DECAP.SET
        IF DQ.REPORT.NUM%      = 0 THEN CALL DQ.SET
        IF SOPTS.REPORT.NUM%   = 0 THEN CALL SOPTS.SET
        IF SERVL.SESS.NUM%     = 0 THEN CALL SERVL.SET                                                                                   ! BMG

        DECCF.SESS.NUM% = 1000
        SOPTS.SESS.NUM% = 1001
        DECAP.SESS.NUM% = 1002
        DQ.SESS.NUM%    = 1003
        SERVL.SESS.NUM% = 1004                                                                                                           ! BMG

        SOCKET.HANDLE% = -1                                                                                                              ! BMG
        QUEUE.FILE     = -1                                                                                                              ! BMG   

        IF END # DECCF.SESS.NUM% THEN ABORT.SUB                                                                                          ! BMG
        OPEN DECCF.FILE.NAME$ RECL DECCF.RECL% AS DECCF.SESS.NUM% NOWRITE NODEL

        CALL OPEN.DQ("D:\ADX_UDT1\DQ2" + NODE.ID$ + ".BIN")                                                                              ! MBG

        IF END # SOPTS.SESS.NUM% THEN SKIP.SOPTS.READ                                                                                     ! BMG 
        OPEN SOPTS.FILE.NAME$ RECL SOPTS.RECL% AS SOPTS.SESS.NUM% NOWRITE NODEL                                                           ! BMG

        ! Read EALSOPTS record 156
  READ.SOPT:

        SOPTS.REC.NUM% = 156

        SOCKET.RC% = READ.SOPTS

        IF SOCKET.RC% = 0 THEN BEGIN                                                                                                      ! BMG
           CALL SOREC156(SOPTS.RECORD$)                                                                                                   ! BMG
        ENDIF                                                                                                                             ! BMG

        CLOSE SOPTS.SESS.NUM%

SKIP.SOPTS.READ:


    TEST$ = "ADXLXACN::D:\ADX_UDT1\DECAPIL" + STR$(VAL(UNPACK$(DAY$)))

    !OPEN DECAPI LOCAL FILE IF REQUIRED
    IF DEC.LOGSTATUS AND NOT LOG.FILE.OPEN AND TEST$ <> DECAP.FILE.NAME$  THEN BEGIN
       DECAP.FILE.NAME$ = "ADXLXACN::D:\ADX_UDT1\DECAPIL" + STR$(VAL(UNPACK$(DAY$)))
       IF END # DECAP.SESS.NUM% THEN CREATE.DECAP
       OPEN DECAP.FILE.NAME$ AS DECAP.SESS.NUM% NOREAD NODEL APPEND 
       GOTO SKIP.DECAP
       CREATE.DECAP:
       CREATE POSFILE DECAP.FILE.NAME$ AS DECAP.SESS.NUM% MIRRORED PERUPDATE
       CALL DECAPI.LOG(DECAP.FILE.NAME$ +" daily DECAPI file created",0)
       SKIP.DECAP:
       LOG.FILE.OPEN = ACK
    ENDIF

    !OPEN SERVICE EVENT LOG FILE IF REQUIRED                                                                                              ! BMG
    IF DEC.SERVICE.LOG AND NOT SERVICE.FILE.OPEN THEN BEGIN                                                                               ! BMG
       SERVL.FILE.NAME$ = "ADXLXACN::D:\ADX_UDT1\SERVL" + NODE.ID$                                                                        ! BMG
       IF END # SERVL.SESS.NUM% THEN CREATE.SERVL                                                                                         ! BMG
       OPEN SERVL.FILE.NAME$ AS SERVL.SESS.NUM% NOREAD NODEL APPEND                                                                       ! BMG
       GOTO SKIP.SERVL                                                                                                                    ! BMG
       CREATE.SERVL:                                                                                                                      ! BMG
       CREATE POSFILE SERVL.FILE.NAME$ AS SERVL.SESS.NUM% MIRRORED PERUPDATE                                                              ! BMG
       CALL DECAPI.LOG(SERVL.FILE.NAME$ + " service event log  file created",0)                                                           ! BMG
       SKIP.SERVL:                                                                                                                        ! BMG
       SERVICE.FILE.OPEN = ACK                                                                                                            ! EMG ! BMG
    ENDIF

        ! Read all DECCONF records and store in the DECCONF array
        CALL DECAPI.LOG("Read DEC configuration messages into memory",0)                                                                  ! BMG
        LOOP% = 0
        DECCF.REC.NUM% = 1
        WHILE 1=1

        IF READ.DECCF = 0 THEN BEGIN
            DECCONF.RECORD$(LOOP%) = DECCF.RECORD$
            LOOP% = LOOP% + 1
            DECCF.REC.NUM% = DECCF.REC.NUM% + 1
         ENDIF ELSE BEGIN
          GOTO ALL.READ
         ENDIF

        WEND

ALL.READ:

        PHASE1.COMPLETED = ACK

        CLOSE DECCF.SESS.NUM%

        CLIENT.IP$           = PRIMARY.IP$
        ALTERNATE.IP$        = SECONDARY.IP$

        TIME.IN.MS% = MILLI.TIME%                                                                                                         ! BMG

    ENDIF

    ! Create application handle (SOCK_CONNECT)
    CALL DECAPI.LOG("Create socket for client",0)                                                                                         ! BMG
    SOCKET.RC% = SOCK.INIT(1, 32767)

    CALL DECAPI.LOG("Attempt connection to DEC",0)                                                                                        ! BMG
    CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$ + " Client_IP:" + CLIENT.IP$ + " Port_number:" + STR$(DEC.PORT%),0)                        ! BMG
    SOCKET.RC% = DEC.CONNECT(CLIENT.IP$, DEC.PORT%, DEC.SOCKET.TIMEOUT%)

    IF PROBLEM.DQ THEN BEGIN
       PROBLEM.DQ = 0                                                                                                                     ! BMG
       CALL DECAPI.LOG("API" + "EV00007" + PIPE$ + "Could not open / Create queue file for controller " + NODE.ID$,1)                     ! BMG
    ENDIF ELSE BEGIN                                                                                                                      ! BMG
       QUEUE.FILE = 0                                                                                                                     ! BMG
    ENDIF

    IF SOCKET.RC% = -1 THEN BEGIN

       !Problem with connecting to Primary server, try secondary.
       CALL DECAPI.LOG("Problem with connecting to server - "+CLIENT.IP$,0)                                                               ! BMG
       CALL DECAPI.LOG("Try server - "+ALTERNATE.IP$,0)                                                                                   ! BMG
       A.IP$ = CLIENT.IP$
       CLIENT.IP$ = ALTERNATE.IP$ 
       ALTERNATE.IP$ = A.IP$
       ALTERNATE.DQ$ = A.DQ$
       SOCKET.RC% = DEC.CONNECT(CLIENT.IP$, DEC.PORT%, DEC.SOCKET.TIMEOUT%)
       IF SOCKET.RC% = -1 THEN BEGIN 
          CALL DECAPI.LOG("Problem with connecting to Secondary server",0)
          CALL DECAPI.LOG("Failure to connect. Return to Client with relevant",0)                                                         ! BMG
          CALL DECAPI.LOG("Return code",0)                                                                                                ! BMG
          CALL DECAPI.LOG("Socket handle for client ("+DECAPI.CLIENT$+") " + STR$(SOCKET.HANDLE%),0)                                      ! BMG
          CALL DECAPI.LOG("API" + "EV00006" + PIPE$ + "Could not connect to socket adapter",1)                                            ! BMG
       ENDIF
       
    ENDIF ELSE BEGIN
          CALL DECAPI.LOG(".............SOCKET CONNECTED - AWAITING MESSAGES...............",0)                                           ! BMG
    ENDIF

ABORT.SUB:

    SOCKET.HANDLE% = SOCKET.RC%

END SUB

\*******************************************************************************
\***
\***    PROCESS.TEMP.FILE
\***
\*******************************************************************************
!
!
!FUNCTION PROCESS.TEMP.FILE                                                                                                               ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!   STRING    FORM$                                                                                                                       ! GMG ! DMG
!   STRING    DATA$                                                                                                                       ! GMG ! DMG
!   STRING    NODE$                                                                                                                       ! GMG ! DMG
!    INTEGER*1 RC%                                                                                                                        ! GMG ! DMG
!   INTEGER*2 ARG.RETURN.CODE                                                                                                             ! GMG ! DMG
!   INTEGER*2 PROCESS.TEMP.FILE                                                                                                           ! GMG ! DMG
!   INTEGER*2 TEMP.SESS.NUM%                                                                                                              ! GMG ! DMG
!   INTEGER*4 SIZE%                                                                                                                       ! GMG ! DMG
!                                                                                                                                         ! DMG
!   ARG.RETURN.CODE = 09H                                                                                                                 ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!   TEMP.SESS.NUM% = 1005                                                                                                                 ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!    ON ERROR GOTO ABORT.FUNCTION                                                                                                         ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!    !Open temp file locked and move data in to live queue file.                                                                          ! GMG ! DMG
!    NODE$ = "ADXLXAAN::"                                                                                                                 ! GMG ! DMG
!    IF NODE.ID$ <> "CE" THEN NODE$ = "ADXLXACN::"                                                                                        ! GMG ! DMG
!    SIZE% = SIZE(NODE$ + "D:\ADX_UDT1\DQ" + NODE.ID$ + ".TMP")                                                                           ! GMG ! DMG
!    IF SIZE% <> 0 THEN BEGIN                                                                                                             ! GMG ! DMG
!       OPEN NODE$ + "D:\ADX_UDT1\DQ" + NODE.ID$ + ".TMP" AS TEMP.SESS.NUM% BUFFSIZE 32767 LOCKED NOWRITE                                 ! GMG ! DMG
!       FORM$ = "C" + STR$(SIZE%)                                                                                                         ! GMG ! DMG
!      READ FORM FORM$;# TEMP.SESS.NUM%;DATA$                                                                                             ! GMG ! DMG
!       DELETE TEMP.SESS.NUM%                                                                                                             ! GMG ! DMG
!       DQ.ARRAY.RECORD$(0) = DATA$                                                                                                       ! GMG ! DMG
!       RC% = WRITE.MATRIX.DQ                                                                                                             ! GMG ! DMG
!      DIM DQ.ARRAY.RECORD$(1000)                                                                                                         ! GMG ! DMG
!       ARG.RETURN.CODE = 00H                                                                                                             ! GMG ! DMG
!    ENDIF                                                                                                                                ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!    ABORT.FUNCTION:                                                                                                                      ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!    PROCESS.TEMP.FILE = ARG.RETURN.CODE                                                                                                  ! GMG ! DMG
!                                                                                                                                         ! GMG ! DMG
!END FUNCTION                                                                                                                             ! GMG ! DMG
                                                                                                                                          ! GMG ! DMG
\********************************************************************
\********************************************************************
\***
\***    S T A R T   O F   F U N C T I O N S
\***
\********************************************************************
\********************************************************************

\********************************************************************
\***
\***    DECAP.INIT()
\***
\********************************************************************

FUNCTION DECAPI.INIT PUBLIC

    INTEGER*1 ARG.RETURN.CODE 
    INTEGER*2 DECAPI.INIT

    ARG.RETURN.CODE = 08h
    
    ON ERROR GOTO ABORT.FUNCTION

    CALL SOCKET.INITIALISATION(0)

    IF SOCKET.HANDLE% > 1 AND NOT QUEUE.FILE THEN BEGIN
       ARG.RETURN.CODE = 0
    ENDIF ELSE IF SOCKET.HANDLE% < 1 AND NOT QUEUE.FILE THEN BEGIN  
       ARG.RETURN.CODE = 02h
    ENDIF ELSE IF SOCKET.HANDLE% > 1 AND QUEUE.FILE THEN BEGIN
       ARG.RETURN.CODE = 06h
    ENDIF ELSE BEGIN
       ARG.RETURN.CODE = 07h
    ENDIF 

    CALL DECAPI.LOG("API Initilisation - Return Code = " + STR$(ARG.RETURN.CODE),0)

  EXIT.FUNCTION:

    DECAPI.INIT = ARG.RETURN.CODE

    EXIT FUNCTION

ABORT.FUNCTION:

    RESUME EXIT.FUNCTION

END FUNCTION

\********************************************************************
\***
\***    DECAPI.CLOSE()
\***
\********************************************************************

FUNCTION DECAPI.CLOSE PUBLIC

    INTEGER*1 ARG.RETURN.CODE 
    INTEGER*1 DECAPI.CLOSE
    INTEGER*1 RC%

    ARG.RETURN.CODE = -1

    ON ERROR GOTO ABORT.FUNCTION

    SOCKET.RC% = SOCK.ABORT(SOCKET.HANDLE%)

    IF SOCKET.RC% = 0 THEN ARG.RETURN.CODE = 00H

    CALL DECAPI.LOG("DECAPI.CLOSE  - Attempt to close down socket handle",0)    ! BMG
    CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$ + "Socket_handle:" + \           ! KMW
                    STR$(SOCKET.HANDLE%)+" Return_code:" + STR$(SOCKET.RC%),0)  ! KMW
    CALL DECAPI.LOG("Close down files",0)                                       ! BMG

!!!!IF DQ.OPENED THEN BEGIN                                                     ! KMW
       CLOSE DQ.SESS.NUM%                                                       ! GMG
!!!!!!!DQ.OPENED = NAK                                                          ! KMW
!!!!ENDIF                                                                       ! KMW
                                                                                ! GMG
    IF LOG.FILE.OPEN THEN BEGIN                                                 ! GMG
       CLOSE DECAP.SESS.NUM%                                                    ! GMG
       LOG.FILE.OPEN = NAK                                                      ! GMG
    ENDIF                                                                       ! GMG
                                                                                ! GMG
    IF SERVICE.FILE.OPEN THEN BEGIN                                             ! GMG
       CLOSE SERVL.SESS.NUM%                                                    ! GMG
       SERVICE.FILE.OPEN = NAK                                                  ! GMG
    ENDIF                                                                       ! GMG
                                                                                ! GMG
    PHASE1.COMPLETED = NAK                                                      ! GMG
                                                                                ! GMG
    DECAP.FILE.NAME$ = ""                                                       ! GMG
                                                                                ! GMG
    ARG.RETURN.CODE  = 0                                                        ! GMG

EXIT.FUNCTION:

    DECAPI.CLOSE = ARG.RETURN.CODE

    EXIT FUNCTION    

ABORT.FUNCTION:

    ! We don't care about close errors on the DQ file                           ! KMW
    IF ERRF% = 1003 AND ERR = "CU" THEN RESUME                                  ! KMW

    RESUME EXIT.FUNCTION

END FUNCTION

\********************************************************************
\***
\***    DECAPI.SEND (MESSAGE_ID, PAYLOAD)
\***
\********************************************************************

FUNCTION DECAPI.SEND(ARG.MESSAGEID$,ARG.PAYLOAD$,COMMIT) PUBLIC

    STRING    ARG.MESSAGEID$
    STRING    ARG.PAYLOAD$
    STRING    COMMIT
    STRING    PAYLOAD$
    STRING    FORMAT.REQUEST$
    STRING    TEST.MESSAGE.NAME$
    STRING    QUEUE.FILENAME$
    STRING    SEND$
    STRING    TIME.SENT$
    STRING    HEADER$
    INTEGER*1 ARG.RETURN.CODE
    INTEGER*1 GIVE.UP    
    INTEGER*2 DECAPI.SEND
    INTEGER*2 RC1%
    INTEGER*2 CNT%
    INTEGER*2 RETRY%
    INTEGER*4 PAYLOAD.LENGTH%
    INTEGER*4 MAX.LENGTH%
    INTEGER*4 TIME.SENT%
    INTEGER*4 RC%, PARAM1%    


    ARG.RETURN.CODE = 08h
    RETRY%          = 0                            ! IMG

    ON ERROR GOTO ABORT.FUNCTION

    ERR.MSG$ = ""                                                               ! KMW
    ERR.CODE$ = ""                                                              ! KMW

    IF DECAPI.MAX THEN BEGIN
       MAX.LENGTH%     = 65535
    ENDIF ELSE BEGIN
       MAX.LENGTH%     = 32767
    ENDIF
    
    PAYLOAD.LENGTH% = LEN(ARG.PAYLOAD$)

    IF PAYLOAD.LENGTH% < 0 THEN BEGIN
       PAYLOAD.LENGTH% = 65535 - PAYLOAD.LENGTH%
    ENDIF

 !   TIME.IN.MS% = MILLI.TIME%                                                                                                              ! GMG ! DMG
 !                                                                                                                                          ! GMG ! DMG
 !   IF TIME.IN.MS% > DEC.HOUSEKEEP.TIME%             AND \                                                                                 ! GMG ! DMG
 !      TIME.IN.MS% < (DEC.HOUSEKEEP.TIME% + 1800000) AND \                                                                                 ! GMG ! DMG
 !      NOT HOUSE.KEEP.ACTIVE THEN BEGIN                                                                                                    ! GMG ! DMG
 !      HOUSE.KEEP.ACTIVE = ACK                                                                                                             ! GMG ! DMG
 !      ! Restrict then close down queue file                                                                                               ! GMG ! DMG
 !      CALL ADXFILES (RC%,1,PARAM1%,"D:\ADX_UDT1\DQ" + NODE.ID$ + ".BIN")                                                                  ! GMG ! EMG
 !      CLOSE DQ.SESS.NUM%                                                                                                                  ! GMG ! DMG
 !      DQ.OPENED = NAK                                                                                                                     ! GMG ! DMG
 !      !Open temp file                                                                                                                     ! GMG ! DMG
 !      CALL OPEN.DQ("D:\ADX_UDT1\DQ" + NODE.ID$ + ".TMP")                                                                                  ! GMG ! DMG
 !   ENDIF ELSE IF HOUSE.KEEP.ACTIVE THEN BEGIN                                                                                             ! GMG ! DMG
 !      HOUSE.KEEP.ACTIVE = NAK                                                                                                             ! GMG ! DMG
 !      ! Restrict then close down temp queue file                                                                                          ! GMG ! DMG
 !     CALL ADXFILES (RC%,1,PARAM1%,"D:\ADX_UDT1\DQ" + NODE.ID$ + ".TMP")                                                                   ! GMG ! EMG
 !      CLOSE DQ.SESS.NUM%                                                                                                                  ! GMG ! DMG
 !      DQ.OPENED = NAK                                                                                                                     ! GMG ! DMG
 !      !Open temp file                                                                                                                     ! GMG ! DMG
 !      CALL OPEN.DQ("D:\ADX_UDT1\DQ" + NODE.ID$ + ".BIN")                                                                                  ! GMG ! DMG
 !      ARG.RETURN.CODE = PROCESS.TEMP.FILE                                                                                                 ! GMG ! DMG
 !      IF ARG.RETURN.CODE <> 0 THEN BEGIN                                                                                                  ! GMG ! DMG
 !         DECAPI.SEND = ARG.RETURN.CODE                                                                                                    ! GMG ! DMG
 !         EXIT FUNCTION                                                                                                                    ! GMG ! DMG
 !      ENDIF                                                                                                                               ! GMG ! DMG
 !  ENDIF                                                                                                                                   ! GMG ! DMG


    !Check message does not exceed maximum message length
    IF PAYLOAD.LENGTH% > MAX.LENGTH% OR PAYLOAD.LENGTH% < 0 THEN BEGIN 
           ARG.RETURN.CODE = 05H 
           CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$+" Socket_handle:" + STR$(SOCKET.HANDLE%)+" Message exceeds maximum message length",0) ! BMG
    ENDIF ELSE BEGIN 

            !Check message_id is valid
            CALL MESSAGEID.MATCH(ARG.MESSAGEID$,NAK)                                                                                        ! BMG

            IF FOUND AND MESSAGE.MATCH THEN BEGIN


                !DECCF.MSGNAME$ = ARG.MESSAGEID$ + PIPE$
                FOUND         = NAK
                MESSAGE.MATCH = NAK

                IF DQ.ELEMENT% <> 0 AND COMMIT <> "C" THEN BEGIN  ! 
                   DQ.ARRAY.RECORD$(DQ.ELEMENT%) =  ARG.PAYLOAD$
                   DQ.ELEMENT% = DQ.ELEMENT% + 1 
                   ARG.RETURN.CODE  = 00H
                ENDIF ELSE BEGIN     
                   
                   DQ.ARRAY.RECORD$(DQ.ELEMENT%) = DQ.ARRAY.RECORD$(DQ.ELEMENT%) + ARG.PAYLOAD$
                   IF LEN(DQ.ARRAY.RECORD$(DQ.ELEMENT%)) > MAX.LENGTH% OR LEN(DQ.ARRAY.RECORD$(DQ.ELEMENT%)) < 0 THEN BEGIN
                      DECAPI.SEND = ARG.RETURN.CODE
                      EXIT FUNCTION 
                   ENDIF

                   IF  COMMIT <> "C" THEN BEGIN 
                       DQ.ELEMENT% = DQ.ELEMENT% + 1 
                       ARG.RETURN.CODE  = 00H
                   ENDIF ELSE BEGIN


                       !Check if payload has any control characters which need translating before writing to queue file
                       FOR LOOP% = 0 TO DQ.ELEMENT%
                           IF DEC.TRANSTATUS AND DECCF.DELIVERY$ <> "S"  THEN BEGIN
                              !CHECK FOR the following in payload and translate.
                               DQ.ARRAY.RECORD$(LOOP%) = CHECK.PAYLOAD.AND.TRANSLATE(DQ.ARRAY.RECORD$(LOOP%),AMPERSAND$,"&AMP;")
                               DQ.ARRAY.RECORD$(LOOP%) = CHECK.PAYLOAD.AND.TRANSLATE(DQ.ARRAY.RECORD$(LOOP%),CRTLF$,"&CRTLF;")
                               DQ.ARRAY.RECORD$(LOOP%) = CHECK.PAYLOAD.AND.TRANSLATE(DQ.ARRAY.RECORD$(LOOP%),PIPE$,"&PIPE;")
                               DQ.ARRAY.RECORD$(LOOP%) = CHECK.PAYLOAD.AND.TRANSLATE(DQ.ARRAY.RECORD$(LOOP%),QUOTE$,"&QUOT;")
                           ENDIF
                       NEXT

                   !IF  DECCF.DELIVERY$ = "S" THEN BEGIN
                   !    TIME.SENT% = MILLI.TIME%                                                                                             ! HMG ! BMG
                   !    TIME.SENT$ = RIGHT$(STRING$(8,"0") + STR$(TIME.SENT%),8)                                                             ! HMG ! EMG
                   !    DQ.ARRAY.RECORD$(0) = DECCF.MSGNAME$ + TIME.SENT$ + DQ.ARRAY.RECORD$(0)                                              ! HMG ! BMG
                   !ENDIF ELSE BEGIN                                                                                                         ! HMG ! BMG
                   DQ.ARRAY.RECORD$(0) = DECCF.MSGNAME$ + DQ.ARRAY.RECORD$(0)                                                           ! BMG
                   !ENDIF 

                       PAYLOAD.LENGTH% = LEN(DQ.ARRAY.RECORD$(0))+2

                       !Format message request

                       !Send message via the SOCKET
                       IF  DECCF.DELIVERY$ = "S" THEN BEGIN 
                           DQ.ARRAY.RECORD$(DQ.ELEMENT%) = DQ.ARRAY.RECORD$(DQ.ELEMENT%) + CRTLF$
                           SEND$ = ""
                           FOR LOOP% = 0 TO DQ.ELEMENT%
                              !SOCK.SEND(HANDLE%, MSG$, FLAGS%)
                              CALL DECAPI.LOG("Attempt to send payload for "+DECCF.MSGNAME$,0 )                                             ! BMG
                              CALL DECAPI.LOG("Request = " + DQ.ARRAY.RECORD$(LOOP%),0)                                                     ! BMG
                              CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$ + " Socket handle:" + STR$(SOCKET.HANDLE%),0)                      ! BMG 
                              TIME.SENT% = MILLI.TIME%                                                                                      ! BMG
                              RC1% = SOCK.SEND(SOCKET.HANDLE%,DQ.ARRAY.RECORD$(LOOP%), 0)                                               
                              ! Return code matches data sent                                                                           
                              SEND$ = STR$(LEN(DQ.ARRAY.RECORD$(LOOP%)))                                                                    ! BMG                                                        
                              IF RC1% <> VAL(SEND$) THEN BEGIN                                                                          
                                 RC% = -1                                                                                               
                              ENDIF  ELSE BEGIN                                                                                         
                               RC% = 0                                                                                                  
                              ENDIF                                                                                                     
                           NEXT                                                                                                         

                           IF RC% = 0 THEN BEGIN
                              !Message request sent successfully add to log file (if active) and return ACK to calling application
                              CALL DECAPI.LOG("Message request sent successfully ("+SEND$+" Bytes)",0)                                      ! BMG
                              ARG.RETURN.CODE  = 00H
                           ENDIF ELSE BEGIN
                              !Socket not connected. Attempt to reconnect to DEC and re-send message (Try 2 times then give up)
                              IF RC% = -1 AND (TCPERRNO% = 039H OR TCPERRNO% = 035H OR TCPERRNO% = -1) THEN BEGIN

                                 CALL DECAPI.LOG("Socket not connected. Attempt to reconnect to DEC and re-send message",0)                 ! BMG
                                 CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$+ " Socket_handle:" + STR$(SOCKET.HANDLE%)+" Return_code:" +     \ BMG
                                                  STR$(RC%)+" TCP_ERROR_NO:" + STR$(TCPERRNO%),0)                                           ! BMG                                 
                                 LOOP% = 0
                                 CNT%  = 0
                                 GIVE.UP = NAK
                                 WHILE NOT GIVE.UP
                                      CALL SOCKET.INITIALISATION(-1)
                                      !Failed to connect
                                      IF SOCKET.HANDLE% = -1 THEN BEGIN
                                         ARG.RETURN.CODE  = 02H
                                         IF LOOP% = 1 THEN BEGIN                                                                            ! BMG
                                            GIVE.UP = ACK
                                            !Service event - Failed to connect                                                              ! BMG                                                                                   ! BMG
                                         ENDIF                                                                                              ! BMG
                                         LOOP% = LOOP% + 1
                                      ENDIF ELSE BEGIN
                                          !TIME.SENT% = MILLI.TIME%                                                                         ! HMG ! BMG
                                          !TIME.SENT$ = RIGHT$(STRING$(8,"0") + STR$(TIME.SENT%),8)                                         ! HMG ! EMG 
                                          ! IF DQ.ELEMENT% = 0 THEN BEGIN                                                                   ! HMG 
                                          !HEADER$ =  DECCF.MSGNAME$ + TIME.SENT$                                                           ! HMG !
                                          !DQ.ARRAY.RECORD$(0) = HEADER$ + RIGHT$(DQ.ARRAY.RECORD$(0),LEN(DQ.ARRAY.RECORD$(0)) -            ! HMG  \
                                          !                      LEN(HEADER$))                                                              ! HMG !                                                              !
                                          ! ENDIF                                                                                           ! HMG !
                                          !DQ.ARRAY.RECORD$(0) = DECCF.MSGNAME$ + STR$(TIME.SENT%) + DQ.ARRAY.RECORD$(0)                    ! HMG ! BMG                                                                                          !
                                          FOR LOOP% = 0 TO DQ.ELEMENT%                                                                      ! HMG ! BMG
                                              CALL DECAPI.LOG("Request = " + DQ.ARRAY.RECORD$(LOOP%),0)                                     ! HMG ! 
                                              RC% = SOCK.SEND(SOCKET.HANDLE%,DQ.ARRAY.RECORD$(LOOP%), 0)
                                          NEXT
                                          GIVE.UP = ACK
                                          IF TCPERRNO% >= 0 THEN BEGIN
                                             !Message request sent successfully add to log file (if active) and return ACK to calling application
                                             GIVE.UP = ACK
                                             ARG.RETURN.CODE  = 00H
                                          ENDIF ELSE BEGIN
                                              ARG.RETURN.CODE  = 02H
                                              IF CNT% = 1 THEN GIVE.UP = ACK
                                              CNT% = CNT% + 1
                                              CALL DECAPI.LOG("API" + "EV00006" + PIPE$ + "Could not connect to socket adapter",1)          ! BMG
                                          ENDIF
                                      ENDIF
                                 WEND
                              ENDIF
                           ENDIF
                           FOUND = NAK
                           SEND$ = ""
                           CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$+" Socket_handle:" + STR$(SOCKET.HANDLE%) +                            \ BMG
                                           " Return_code:" + STR$(RC%)+ " TCP_ERROR_NO:" + STR$(TCPERRNO%),0)                               ! BMG
                           CALL DECAPI.LOG("Message could not be sent",0)                
                           !ARG.RETURN.CODE  = 0AH                
                       ENDIF ELSE BEGIN                                                                  
                       DQ.WRITE:
                           !Send message to the queue file
                           CALL DECAPI.LOG("Send async message to "  + DQ.FILE.NAME$,0)                                                     ! BMG
                           RC% = WRITE.MATRIX.DQ
                           IF RC% = 0 THEN BEGIN
                              ARG.RETURN.CODE  = 00H
                           ENDIF
                           CALL DECAPI.LOG("Return code   - " + STR$(RC%),0)                                                                ! BMG
                           CALL DECAPI.LOG("Write type    - " + DECCF.DELIVERY$,0)                                                          ! BMG
                       ENDIF                                                                                                           
                   ENDIF
                ENDIF
            ENDIF ELSE BEGIN

               ARG.RETURN.CODE  = 03H

            ENDIF
    ENDIF

    DECAPI.SEND = ARG.RETURN.CODE

ERROR.LEAVE.FUNCTION:

     IF COMMIT = "C" THEN BEGIN                                                                     ! JMG
         DQ.ELEMENT% = 0
         DIM DQ.ARRAY.RECORD$(1000)
     ENDIF                                                                                          ! JMG
     
    EXIT FUNCTION
    
ABORT.FUNCTION:

  ! IF error on DQ file AND                                                                         ! KMW
  !    cannot write AND                                                                             ! KMW
  !    we've not had enough of retrying yet                                                         ! KMW
  IF ERRF% = 1003 AND                       \                                                       ! KMW
     ((ERR = "*I" OR ERRN = 80F306F0h) OR   \ File locked                                           ! KMW
      (ERR = "WT" OR ERRN = 8020430Dh) OR   \ Cannot write                                          ! KMW
      (ERR = "FU" OR ERRN = 00000022h)) AND \ File unavailable                                      ! KMW
     RETRY% <> 5 THEN BEGIN                                                                         ! KMW
     CALL DECAPI.LOG("File locked. Wait 5 seconds and try again. Attempt " + STR$(RETRY%+1),0)      ! IMG
     WAIT; 5000
     ! Attempt to re-acquire the DQ file                                                            ! KMW
     CALL OPEN.DQ("D:\ADX_UDT1\DQ2" + NODE.ID$ + ".BIN")                                            ! MBG
     RETRY% = RETRY% + 1
     RESUME DQ.WRITE                                                                                ! IMG
  ENDIF ELSE BEGIN
     ERR.MSG$ = ERR                                                                                 ! KMW
     ERR.CODE$ = ERRNHEX$                                                                           ! KMW
     CALL DECAPI.LOG("Problem with DECAPI.SEND.....",0)
     DECAPI.SEND = -1
     RESUME ERROR.LEAVE.FUNCTION
 ENDIF

END FUNCTION  
 
\********************************************************************
\***
\***    DECAPI.RECV()
\***
\********************************************************************

FUNCTION DECAPI.RECV PUBLIC

    STRING    MSG$
    STRING    RES$
    STRING    RESPONSE$
    STRING    TEMP$
    
    STRING    ARG.RETURN.DATA$
    STRING    DECAPI.RECV
    INTEGER*1 ARG.RETURN.CODE
    INTEGER*1 PAYLOAD.COMPLETE 
    INTEGER*2 RETURN.CODE%
    INTEGER*2 RC%

    ARG.RETURN.CODE = 08H


    ON ERROR GOTO ABORT.FUNCTION


        TEMP$ = "00" 
        MSG$  = ""
        RESPONSE.TIME = ""

        IF SOCKET.MESSAGE.ARRAY$(0) = "" THEN BEGIN                                                                                     ! EMG      

            CALL PUTN2(TEMP$, 0, SOCKET.HANDLE%)

            CALL SOCK.SELECT(TEMP$, 1, 0, 0, DEC.SOCKET.TIMEOUT%, RC%)

            IF RC% > 0 THEN BEGIN

                IF GETN2(TEMP$,0) <> -1 THEN BEGIN 

                        PAYLOAD.COMPLETE = NAK
                        RESPONSE$ = ""
                        WHILE NOT PAYLOAD.COMPLETE

                            !Request response from Socket handle
                            CALL SOCK.RECV(SOCKET.HANDLE%,RES$,0,RC%)
                            CALL DECAPI.LOG("Request response from Socket handle",0)                                                    ! BMG
                            CALL DECAPI.LOG("Client:" + DECAPI.CLIENT$+" Socket_handle:" +                                              \ BMG 
                            STR$(SOCKET.HANDLE%)+" TCP_ERROR_NO:" + STR$(TCPERRNO%),0)                                                  ! BMG
                            IF RC% = 0 THEN BEGIN

                               IF LEN(RES$) = 0 THEN BEGIN
                                  !Session closed sockets. Exit to calling application
                                  ARG.RETURN.CODE = 04H
                                  CALL DECAPI.LOG("Session closed sockets. Exit to calling application",0)                              ! BMG     
                                  SOCKET.RC% = SOCK.ABORT(SOCKET.HANDLE%)
                                  ! Service event - session closed sockets                                                              ! BMG     
                                  RESPONSE$ = "NAK0"+STR$(ARG.RETURN.CODE)      
                               ENDIF ELSE BEGIN
                                 !Check all message received i.e. ends 0D0A
                                 IF LEFT$(RES$,3) = "NAK" THEN BEGIN
                                    !Problem with received response
                                    ARG.RETURN.CODE = 02H
                                    CALL DECAPI.LOG("DEC Response = " + RESPONSE$,0)                                                    ! BMG  
                                    CALL DECAPI.LOG("Problem with response   - NAKerrorcode,message,payload",0)                         ! BMG
                                    CALL DECAPI.LOG(RES$,0)                                                                             ! BMG
                                    RESPONSE$ = "NAK0"+STR$(ARG.RETURN.CODE)                                                            ! BMG                 
                                 ENDIF ELSE BEGIN
                                     WHILE RIGHT$(RES$,2) <> CRTLF$ AND LEFT$(RES$,3) <> "NAK" AND RC% = 0
                                        RESPONSE$ = RESPONSE$ + RES$
                                        CALL DECAPI.LOG("RESPONSE      - ACK(" + STR$(LEN(RES$)) + " Bytes)",0)                         ! BMG
                                        RES$ = ""
                                        CALL SOCK.RECV(SOCKET.HANDLE%,RES$,0,RC%)
                                     WEND
                                     IF RC% <> 0 THEN BEGIN
                                        RESPONSE$ = "NAK0"+STR$(03H)
                                     ENDIF ELSE BEGIN   
                                         IF LEFT$(RES$,3) <> "NAK" THEN BEGIN
                                            RESPONSE$ = RESPONSE$ + RES$
                                        ! Request / response elapsed time                                                               ! BMG
                                            IF DEC.TRANSTATUS THEN BEGIN
                                               RESPONSE$ = CHECK.PAYLOAD.AND.TRANSLATE(RESPONSE$,"&AMP;",AMPERSAND$)
                                               RESPONSE$ = CHECK.PAYLOAD.AND.TRANSLATE(RESPONSE$,"&CRTLF;",CRTLF$)
                                               RESPONSE$ = CHECK.PAYLOAD.AND.TRANSLATE(RESPONSE$,"&PIPE;",PIPE$)
                                               RESPONSE$ = CHECK.PAYLOAD.AND.TRANSLATE(RESPONSE$,"&QUOT;",QUOTE$)
                                            ENDIF
                                             RESPONSE$ = MESSAGE.ARRAY(RESPONSE$)                                                       ! EMG
                                        ! Request / response elapsed time                                                               ! BMG
                                            CALL DECAPI.LOG("DEC Response ("+ DECAPI.CLIENT$ + ") = " + RESPONSE$,0)                    ! BMG
                                            CALL DECAPI.LOG("Socket response time  - " + RESPONSE.TIME + "ms",0)                        ! BMG
                                         ENDIF ELSE BEGIN
                                            !Problem with received response
                                            CALL DECAPI.LOG("Problem with response - Client:" + DECAPI.CLIENT$+" Socket_handle:" +      \ BMG 
                                                             STR$(SOCKET.HANDLE%)+ "Return code: " + STR$(RC%) + " TCP_ERROR_NO:" +     \ BMG
                                                             STR$(TCPERRNO%),0)                                                         ! BMG
                                            CALL DECAPI.LOG(RES$,0)                                                                     ! BMG        
                                            ARG.RETURN.CODE = 02H                                                                       ! BMG
                                            RESPONSE$ = "NAK0"+STR$(ARG.RETURN.CODE)                                                    ! BMG
                                         ENDIF 
                                     ENDIF
                                 ENDIF    
                               ENDIF     
                            PAYLOAD.COMPLETE = ACK
                            ENDIF ELSE BEGIN
                               RESPONSE$ = "NAK00"
                            ENDIF
                        WEND
                ENDIF
            ENDIF ELSE BEGIN
               ARG.RETURN.CODE = 01H
               RESPONSE$ = "NAK0"+STR$(ARG.RETURN.CODE)
               CALL DECAPI.LOG("No response Timed out  - Return Code " + STR$(RC%)  ,0)                                                 ! BMG
            ENDIF



RESUME.HERE:

       ENDIF ELSE BEGIN                                                                                                                 ! EMG
                                                                                                                                        ! EMG
           ! Pop next message from queue                                                                                                ! EMG
           RESPONSE$ = "NAK"                                                                                                            ! EMG
                                                                                                                                        ! EMG
       ENDIF                                                                                                                            ! EMG

        IF LEFT$(RESPONSE$,3) = "NAK" AND SOCKET.MESSAGE.ARRAY$(0) <> "" THEN BEGIN                                                     ! EMG
           RESPONSE$ = MESSAGE.ARRAY("NAK")                                                                                             ! EMG           
           CALL DECAPI.LOG("PASSING MESSAGE FROM INTERNAL API QUEUE",0)                                                                 ! EMG          
           CALL DECAPI.LOG("API Queue Response = " + RESPONSE$,0)                                                                       ! EMG
           CALL DECAPI.LOG("DEC Response ("+ DECAPI.CLIENT$ + ") = " + RESPONSE$,0)                                                     ! EMG
           CALL DECAPI.LOG("Socket response time  - " + RESPONSE.TIME + "ms",0)                                                         ! EMG
        ENDIF                                                                                                                           ! EMG

        DECAPI.RECV = RESPONSE$

        EXIT FUNCTION

ABORT.FUNCTION:

        RESUME RESUME.HERE

END FUNCTION

\**************************************************************************************
\***                
\***    DECAPI.EVENT (ARG.MESSAGEID$, ARG.DATA$, ARG.REASONCODE%)                                                                       ! BMG
\***                                                                                                                                    ! BMG
\***    Status and accepted reason codes                                                                                                ! BMG
\***       REASONCODE%                  MEANING                                                                                         ! BMG
\***        EV00000                      Response not yet available                                                                     ! BMG
\***                                        No response from DEC                                                                        ! BMG
\***        EV00001                      Timed out waiting for response                                                                 ! BMG
\***                                        DATA$ relates to PAYLOAD                                                                    ! BMG
\***        EV00002                      Invalid payload                                                                                ! BMG
\***                                        Client received corrupt payload                                                             ! BMG
\***        EV00003                      Incomplete message                                                                             ! BMG
\***                                        Bad response from DEC                                                                       ! BMG
\***        EV00004                      DEC Socket closed                                                                              ! BMG
\***                                        DEC shut down socket                                                                        ! BMG
\***        EV00005                      Good(e) response                                                                               ! BMG
\***                                        DATA$ relates to request / response time                                                    ! BMG
\***        EV00006                      Socket down                                                                                    ! BMG
\***                                        Could not create socket connection                                                          ! BMG
\***        EV00007                      EPOS Queue file down                                                                           ! BMG
\***                                        Could not open the queue file                                                               ! BMG
\***
\***
\**************************************************************************************

FUNCTION DECAPI.EVENT (ARG.MESSAGEID$, ARG.DATA$, ARG.REASONCODE%) PUBLIC                                                               ! BMG
                                                                                                                                        ! BMG
    STRING    ARG.MESSAGEID$                                                                                                            ! BMG
    STRING    ARG.DATA$                                                                                                                 ! BMG
    STRING    EVENT.MESSAGE                                                                                                             ! BMG
    INTEGER*1 DECAPI.EVENT                                                                                                              ! BMG
    INTEGER*1 ARG.RETURN.CODE                                                                                                           ! BMG
    INTEGER*1 RC%                                                                                                                       ! BMG
    INTEGER*2 ARG.REASONCODE%                                                                                                           ! BMG
                                                                                                                                        ! BMG
    ARG.RETURN.CODE = -1                                                                                                                ! BMG
                                                                                                                                        ! BMG
      CALL MESSAGEID.MATCH(ARG.MESSAGEID$,ACK)                                                                                          ! BMG
      IF FOUND AND MESSAGE.MATCH THEN BEGIN                                                                                             ! BMG
         ARG.DATA$ = CHECK.PAYLOAD.AND.TRANSLATE(ARG.DATA$,"&PIPE;",PIPE$)                                                              ! BMG
         CALL DECAPI.LOG("CLI" + STR$(ARG.REASONCODE%)+ PIPE$ + ARG.MESSAGEID$ + " "                                                    \ BMG
                        + ARG.DATA$,1)                                                                                                  ! BMG
         FOUND = NAK                                                                                                                    ! BMG
      ENDIF ELSE BEGIN                                                                                                                  ! BMG
         ARG.RETURN.CODE  = 03H                                                                                                         ! BMG
      ENDIF                                                                                                                             ! BMG
                                                                                                                                        ! BMG
    DECAPI.EVENT = ARG.RETURN.CODE                                                                                                      ! BMG
                                                                                                                                        ! BMG
END FUNCTION                                                                                                                            ! BMG


\********************************************************************
\***
\***    DECAPI.COMMAND (COMMAND$, DATA$)
\***
\********************************************************************

FUNCTION DECAPI.COMMAND(ARG.COMMAND$,ARG.DATA$) PUBLIC

    STRING    ARG.COMMAND$
    STRING    ARG.DATA$
    INTEGER*1 DECAPI.COMMAND
    INTEGER*1 ARG.RETURN.CODE 
    INTEGER*1 RC% 
    
    ARG.RETURN.CODE = -1
    
    CALL MESSAGEID.MATCH(ARG.COMMAND$,NAK)                                                                                              ! BMG

    ! Check that not being used as DECAPI.SEND                                                                                          ! BMG
    IF NOT FOUND THEN BEGIN                                                                                                             ! BMG
       RC% = SOCK.SEND(SOCKET.HANDLE%,ARG.COMMAND$ + "|" + ARG.DATA$ + CRTLF$,0)                                                        ! BMG
       IF LEN(ARG.COMMAND$ + "|" + ARG.DATA$ + CRTLF$) = RC% THEN BEGIN                                                                 ! BMG
          ARG.RETURN.CODE = 0                                                                                                           ! BMG
       ENDIF                                                                                                                            ! BMG
    ENDIF

    DECAPI.COMMAND = ARG.RETURN.CODE

END FUNCTION


\********************************************************************
\***
\***    DECAPI.RECV.RESPONSE.TIME
\***
\********************************************************************

FUNCTION DECAPI.RECV.RESPONSE.TIME PUBLIC

    STRING DECAPI.RECV.RESPONSE.TIME

    DECAPI.RECV.RESPONSE.TIME = RESPONSE.TIME

END FUNCTION 

