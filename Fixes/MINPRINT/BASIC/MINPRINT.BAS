\*****************************************************************************
\*****************************************************************************
\***
\***    PROGRAM  .....  MINPRINT
\***    MODULE  ......  MINPRINT.BAS
\***
\***    REVISION 1.0.           ROBERT COWEY.                  01 NOV 1995.
\***    Original version.
\***
\***    VERSION B (1.1)        Nik Sen                         06 FEB 1998
\***    Re-written to use included code for file functions etc.
\***
\***    VERSION C              Nik Sen                         12 OCTOBER 1998
\***    RF version. Changed to send count lists to RF files instead of
\***    printing report if RF is active.
\***
\***    VERSION D              Charles Skadorwa                 7 DECEMBER 1998
\***    Processing Statements added for debugging purposes.
\***    RFSCF opened in NOWRITE NODEL mode instead of READONLY.
\***    Check for List Driven Counting (RFSCF.ACTIVITY%) changed to check for
\***    ASCII value "2" rather than bit value (& 64).
\***
\***    VERSION E              Charles Skadorwa                 15 January 1999
\***    Program will now operate whether List Driven Counting is active or not.
\***    Check removed from RF.PROCESS subroutine.
\***
\***    VERSION F              Charles Skadorwa                 18 January 1999
\***    Items description now taken from the RPRPT file if one does not exist on
\***    the ISF (this resulted in the item being ignored). In VALID.ITEM:,
\***    IRF.DEAL.NUM$ should be initialised before the IRF is read,  otherwise
\***    the next item will become a deal item if the previous one was!
\***    CLILF size increased from 5,000 to 10,000 records to decrease chaining.
\***    RFHOCNT.286 chained to if it is detected that MINPRINT is running outside
\***    of normal hours (before 5pm or after 6am)- otherwise the Head Office
\***    counts would be overwritten.
\***    Barcode constructed from Boots Code if IDF barcode is null.
\***    Initialise new CLILF field (CLILF.HO.SEQNO$)and CLOLF filed (CLOLF.TOTAL.ITEMS$).
\***    Use pipe to instruct RF Server program (TRANSACT) to close down while
\***    MINPRINT is executing (It will restart automatically at next signon).
\***    If description not on ISF then description set to "X " in order that
\***    TRANSACT will read the IDF and perform description formatting.
\***
\***    VERSION G              Julia Stones                  9th November 1999
\***    Code removed that produced the RF report if RF was inactive on SOFTS
\***    Please note that if RF is ACTIVE and DO MAIN gets set to 1 - before
\***    changes were made this would have meant that the code would have
\***    produced the RF report.  DO MAIN is not checked now and so the program
\***    will just go into the termination part of the code.
\***
\***    VERSION H              Brian Greenfield              20th August 2003
\***    Changes made for RF trial to accomodate IRF changes made in DEALS
\***    rewrite.
\***
\***    REVISION 1.12.         ROBERT COWEY.                  09 SEP 2003.
\***    Changes for RF trial.
\***    Removed redundant PVCS revision control block from top of code.
\***    Removed section of code at end of program that started RFHOCNT.
\***    Corrected setting of HO.SEQNO within VALID.ITEMS routine.
\***
\***
\***    VERSION I              Chris Combes (CC)            29th March 2004
\***    Changes made to VALID.ITEM and CREATE.NEW.LIST procedures so lists are
\***    divided into product groups rather than business units
\***
\***
\***    Version J              Mark Goode                   17th September 2004
\***    Changes made for OSSR Basic location project, additional information on
\***    the CLILF/CLOLF files
\***
\***    Version K               Mark Goode                   14th January 2005
\***    Changes made for OSSR WAN project, additioal information on the the CLOLF file.
\***
\***    Version L               Jamie Thorpe                 13th March 2006
\***    Changes for Removal of RF Recounts project.This introduces the ability to
\***    switch off the ability to perform recounts.
\***
\***    Version M               Jamie Thorpe                 4th April 2006
\***    Updated the way that the CLOLF is initially created. This is to rectify
\***    a hang reported in TRANSACT when it encounters 0 bytes in the file.
\***
\***    Version N              Charles Skadorwa              18th June 2009
\***    CR006 - Change to ensure that if it is an MC70/POD store, then
\***            processing will be identical to RF PPC processing. This is
\***            achieved by checking SOFTS record 20 for " ACTIVE"
\***
\***    Version O              Arun Sudhakarannair           14th June 2012
\***    The program is updated to support the following changes as part of
\***    SFA Project
\***        - Rerun and Rerun warning message
\***              With the re-introduction of counts, it is important that
\***              MINPRINT is not rerun without understanding what it will
\***              do to the CLOLF.CLILF and the backups. Hence a new parameter
\***              (RERUN) must be passed if run from command mode, with
\***              help details. Format is "MINPRINT RERUN"
\***        - Prepare a new RF Count report (RFCNTLST.DAY)
\***              Report gives a summary for each count list type (Negative,
\***              User Generated and Support Office) and also gives details
\***              of which users performed the counts, and whether the list
\***              was fully counted, part counted or not counted at all.
\***        - For non RF/POD stores, create the CLOLF/CLILF
\***              Currently, only RF/POD stores have the CLOLF/CLILF files
\***              present. However as part of SFA, these files need creating
\***              and populating like RF/POD stores
\***        - Create daily backups of the CLOLF and CLILF files
\***              Back up the CLOLF and CLILF files on a daily basis (rolling
\***              7 days). Backup the files before RF.PROCESS subroutine is
\***              called. The backup will be used to generate the weekly
\***              store manager count report.
\***        - Support the new CLOLF and CLILF formats
\***        - Introduce a new OK file – MINOK.BIN
\***              Introduce a new OK file to show success status of each step
\***              of MINPRINT processing.
\***
\***    Version P              Bibin Thomas                  10th Aug 2012
\***    Changes made to accommodate SFA CR7 requirements.
\***    In case of a non trading day, MINPRINT is run any time b/w 21:05Hrs
\***    and 00:30Hrs. So if MINPRINT is run after midnight, the program will
\***    backup CLILF/CLOLF with Today's extension, where as it is actually
\***    supposed to backup with yesterday's <DAY>.
\***    Changes are made so that if MINPRINT is run after 00:00Hrs and before
\***    04:00Hrs, CLILF/CLOLF is backed-up with file extension set to
\***    yesterday, else with today.
\***
\***    Version Q              Charles Skadorwa (CCSk)       10th Sept 2012
\***    SFA Defect 661 -  Summary counts are not updated.
\***    Also corrected screen prompt from "u" to "you".
\***
\***    Version R              Tittoo Thomas (RTT)           21th Sept 2012
\***    SFA Defect 688, 695 - Restricted multiple runs of MINPRINT in a day.
\***    If MINPPRINT is found to have already run after 1200 noon the previous
\***    day or before 12 noon today, the application stops without any further
\***    processing.
\***
\*****************************************************************************
\*****************************************************************************




\*****************************************************************************
\***
\***    DEC included code defining file related fields
\***
\***..........................................................................

    %INCLUDE   RPRPTDEC.J86    ! Report file
    %INCLUDE   PRINTDEC.J86    ! Printer
    %INCLUDE   SOFTSDEC.J86    ! SOFTSTAT                      CNS
    %INCLUDE   RFSCFDEC.J86    ! RF Control File               CNS
    %INCLUDE   CLOLFDEC.J86    ! RF Count List Of Lists File   CNS
    %INCLUDE   CLILFDEC.J86    ! RF Count Lists File           CNS
    !%INCLUDE   ISFDEC.J86      ! Item Shelf Edge Label Descriptor File  CNS OAS
    !%INCLUDE   IRFDEC.J86      ! Item Record File             CNS OAS
    %INCLUDE   IDFDEC.J86      ! Item Data File                CNS
    %INCLUDE   PGFDEC.J86      ! Product Group File            JMG
    %INCLUDE   MINLSDEC.J86    ! Minsits Recount Information   LJT
    %INCLUDE   SRITLDEC.J86    ! Active Planner Details        OAS

\*****************************************************************************
\***
\***    Included code defining function related global variables
\***
\***..........................................................................

    %INCLUDE PSBF01G.J86   !   APPLICATION.LOG
    %INCLUDE PSBF02G.J86   !   Update Date
    %INCLUDE PSBF06G.J86   !   Barcode check digit calculation  FCS
    %INCLUDE PSBF20G.J86   !   ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF13G.J86   !   To find the day of the week      OAS

\*****************************************************************************
\***
\***    Global variable definitions
\***
\***..........................................................................

    STRING GLOBAL                           \
        BATCH.SCREEN.FLAG$,                 \
        COMM.MODE.FLAG$,                    \ OAS Set to B-Background  C-Foreground mode
        CURRENT.CODE$,                      \
        FILE.OPERATION$,                    \
        MODULE.NUMBER$,                     \
        OPERATOR.NUMBER$,                   \ OAS
        REPORTING.STATUS$                   ! OAS

    INTEGER*2 GLOBAL                        \
        CURRENT.REPORT.NUM%,                \
        CURRENT.SESS.NUM%,                  \ OAS
        FILE.RETURN.CODE%,                  \
        FUNCTION.RETURN.CODE%

    INTEGER*1 GLOBAL                        \ OAS
        TRUE,                               \ OAS
        FALSE                               ! OAS

\*****************************************************************************
\***
\***    Variable definitions
\***
\***..........................................................................

    STRING                                  \ OAS
        ADXSERVE.DATA$,                     \ DCS  \ OAS \ Holds display message
       \ADXSTART.NAME$,                     \ OAS        \ Variable not used
       \ADXSTART.PARM$,                     \ OAS        \ Variable not used
       \ADXSTART.MESS$,                     \ OAS        \ Variable not used
        BSNS.CNTR$,                         \ OAS
       \COMMAND.STRING$,                    \ OAS        \ Variable not used
        CLILF.DDD$,                         \ OAS
        CLILF.BKUP.STATUS$,                 \ OAS
        CLOLF.DDD$,                         \ OAS
        CLOLF.BKUP.STATUS$,                 \ OAS
        CLOLF.PGNAME$,                      \ ICC  \ OAS
       \COMM.MODE.FLAG$,                    \ DCS  \ OAS \ MOVED TO TOP
       \CRLF$,                              \ OAS        \ Variable not used
        CURRENT.CODE.LOGGED$,               \ OAS
        CURR.RUN.DAY$,                      \ RTT
        FUNCTION.FLAG$,                     \ OAS
        GET.DAY$,                           \ OAS
       \GET.DATE$,                          \ OAS \ PBT
        MINLS.HK.STATUS$,                   \ OAS
        MINOK.FILE.NAME$,                   \ OAS
        MINOK.FILLER$,                      \ RTT
        MINOK.RECORD$,                      \ OAS
        MINOK.RUN.DATE$,                    \ RTT
        MINOK.RUN.TIME$,                    \ RTT
        MINOK.TIME.STAMP$,                  \ RTT
        MODULE$,                            \ OAS
        OLD.BULETT$,                        \ CNS  \ OAS
        OLD.PGNAME$,                        \ ICC  \ OAS
        PASSED.STRING$,                     \ OAS
        PIPE.OPEN$,                         \ FCS  \ OAS
        PROGRAM$,                           \ OAS
        QUOTES$,                            \ RTT
        RESPONSE$,                          \ OAS
        RPRPT.RECORD$,                      \ CNS  \ OAS
       \RPRPT.REP.DATA$,                    \ OAS        \ Variable not used
        TIME.NOW$,                          \ PBT
        UPDATE.DATE$,                       \ CNS  \ OAS
        VAR.STRING.1$,                      \ OAS
        VAR.STRING.2$,                      \ OAS
        YESTERDAY$                          ! RTT

    INTEGER*1                               \ OAS
        COUNTER%,                           \ OAS
        DO.MAIN,                            \ CNS  \ OAS
        EOF,                                \ CNS  \ OAS
        ERROR.COUNT%,                       \ OAS
        EVENT.NUMBER%,                      \ OAS
      \ LIST.ITEMS,                         \ CNS  \ OAS ! MOVED TO BOTTOM
        MINPRINT.ALREADY.RUN%,              \ RTT
        MINOK.ERROR.CHK%                    ! OAS

    INTEGER*2                               \ OAS
        ADX.FUNCTION%,                      \ DCS  \ OAS
        ADX.INTEGER%,                       \ DCS  \ OAS
      \ CURRENT.SESS.NUM%,                  \ CNS  \ OAS ! MOVED TO TOP
        LOOPCNT%,                           \ OAS
        LIST.ITEMS,                         \ OAS
        MESSAGE.NUMBER%,                    \ OAS
        MINOK.REPORT.NUM%,                  \ OAS
        MINOK.SESS.NUM%,                    \ OAS
        PASSED.INTEGER%,                    \ OAS
        RC%                                 ! OAS
        !EVENT.NO%,                         ! FCS  \ OAS  \ Variable not used
        !MESSAGE.NO%                        ! FCS  \ OAS  \ Variable not used

    INTEGER*4                               \ OAS
        ADX.RETURN.CODE%,                   \ DCS  \ OAS
        ADXCOPY.CHK%,                       \ OAS
        FILE.SIZE%,                         \ OAS
        RECORD.COUNT%                       ! OAS
       !RPRPT.REC.MAX%,                     ! OAS         \ Variable not used
       !RPRPT.REC.NUM%,                     ! OAS         \ Variable not used
       !THE.TIME%                           ! FCS  \ OAS  \ Variable not used

\*****************************************************************************
\***
\***    EXT included code defining file related external functions
\***
\***..........................................................................

       %INCLUDE RPRPTEXT.J86
       %INCLUDE PRINTEXT.J86
       %INCLUDE SOFTSEXT.J86                                   ! CNS
       %INCLUDE RFSCFEXT.J86                                   ! CNS
       %INCLUDE CLILFEXT.J86                                   ! CNS
       %INCLUDE CLOLFEXT.J86                                   ! CNS
       !%INCLUDE ISFEXT.J86                                    ! CNS !OAS
       !%INCLUDE IRFEXT.J86                                    ! CNS !OAS
       %INCLUDE IDFEXT.J86                                     ! CNS
       %INCLUDE PGFEXT.J86                                     ! KMG
       %INCLUDE MINLSEXT.J86   ! Minsits Recount Information   ! LJT
       %INCLUDE PSBF13E.J86    ! To initialise day variable    ! OAS
       %INCLUDE ADXCOPY.J86    ! To copy files                 ! OAS
       %INCLUDE SRITLEXT.J86   ! Active Planner Details        ! OAS

        SUB MINRFCNT EXTERNAL                                   ! OAS
        END SUB

\*****************************************************************************
\***
\***    Included code defining external Boots functions
\***
\***..........................................................................

    %INCLUDE PSBF01E.J86   !   APPLICATION.LOG
    %INCLUDE PSBF02E.J86   !   Update Date
    %INCLUDE PSBF06E.J86   !   Barcode Check Digit Calculation   ! FCS ! OAS
    %INCLUDE PSBF08E.J86   !   Print Report Function
    %INCLUDE PSBF20E.J86   !   ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF24E.J86   !   STANDARD.ERROR.DETECTED
    %INCLUDE PSBF30E.J86   !   Process Keyed Record              ! LJT
    %INCLUDE ADXSERVE.J86  !   Message Logging                   ! DCS ! OAS
    %INCLUDE ADXSTART.J86  !   Chain to new program              ! FCS ! OAS
    %INCLUDE CMPDATE.J86   !   Date compare for Y2K compliance   ! IMG ! OAS
    %INCLUDE EALHSASC.J86  !                                           ! OAS

!**********************************************************************! LJT
!***                                                                   ! LJT
!***      PROCESS.KEYED.RECORD$                                        ! LJT
!***                                                                   ! LJT
!***      'User exit' for PROCESS.KEYED.FILE (PSBF30)                  ! LJT
!***                                                                   ! LJT
!**********************************************************************! LJT

FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC                         ! LJT

    STRING PROCESS.KEYED.RECORD$,RECORD$                               ! LJT

       MINLS.ITEM.CODE$ = LEFT$(RECORD$,4)                             ! LJT
       RC% = READ.MINLS                                                ! LJT

       F02.DATE$ = UNPACK$(MINLS.RECOUNT.DATE$)                        ! LJT
       CALL UPDATE.DATE(RFSCF.RECOUNT.DAYS.RETAIN%)                    ! LJT
       IF DATE.LE(F02.DATE$, DATE$) THEN BEGIN                         ! LJT
          DELREC MINLS.SESS.NUM%; MINLS.ITEM.CODE$                     ! LJT
          !Turn off PENDING COUNT flag on the IDF                      ! LJT
          IDF.BOOTS.CODE$ = MINLS.ITEM.CODE$                           ! LJT
          RC% = READ.IDF                                               ! LJT
          IF RC% = 0 THEN BEGIN                                        ! LJT
          !Make sure the flag is set to on first                       ! LJT
             IF (IDF.BIT.FLAGS.2% AND 00000100b) <> 0 THEN BEGIN       ! LJT
             !It's on, so turn it off                                  ! LJT
                 IDF.BIT.FLAGS.2% = (IDF.BIT.FLAGS.2% XOR 4)           ! LJT
                 RC% = WRITE.IDF                                       ! LJT
             ENDIF                                                     ! LJT
          ENDIF                                                        ! LJT
       ENDIF                                                           ! LJT

    PROCESS.KEYED.RECORD$ = RECORD$                                    ! LJT

END FUNCTION                                                           ! LJT

\*****************************************************************************
\***
\***    PROGRAM.CONTROL
\***
\***..........................................................................


MINPRINT.START:


    ON ERROR GOTO ERROR.DETECTED

    IF LEFT$(COMMAND$,8) = "BACKGRND" THEN BEGIN                       ! DCS ! OAS
        COMM.MODE.FLAG$ = "B"                                          ! DCS ! OAS
    ENDIF ELSE BEGIN                                                   ! OAS
        COMM.MODE.FLAG$ = "C"                                          ! DCS ! OAS
        IF LEFT$(COMMAND$,8) = "RERUN" THEN BEGIN                      ! OAS

            !MINPRINT will run with the help of SLEEPER program in     ! OAS
            !background. If MINPRINT has to be run manually, then      ! OAS
            !user has to enter the following parameters in command     ! OAS
            !mode, "MINPRINT RERUN".                                   ! OAS

            GOSUB MINPRINT.HELP                                        ! OAS
            INPUT "Do you really want to run MINPRINT? (Y/N) ";RESPONSE$ ! OAS ! CCSk

            !If the user enters wrong option other than "Y", "N", "y"  ! OAS
            !or "n", program will ask the user to enter the correct    ! OAS
            !choice                                                    ! OAS

            WHILE ((RESPONSE$ <> "Y" ) AND    \                        ! OAS
                   (RESPONSE$ <> "y" ) AND    \                        ! OAS
                   (RESPONSE$ <> "N" ) AND    \                        ! OAS
                   (RESPONSE$ <> "n" ) )                               ! OAS

                INPUT "Enter the correct option (Y/N) ";RESPONSE$      ! OAS
            WEND                                                       ! OAS

            !If the user response is "Y" or "y" then re-run MINPRINT   ! OAS
            !If the user response is "N" or "n" then STOP the program  ! OAS

            IF ((RESPONSE$ = "Y") OR       \                           ! OAS
                (RESPONSE$ = "y")) THEN BEGIN                          ! OAS

                ADXSERVE.DATA$ = "Re - running MINPRINT"               ! OAS
                GOSUB DISPLAY.MESSAGE                                  ! OAS

            ENDIF ELSE IF ((RESPONSE$ = "N") OR \                      ! OAS
                           (RESPONSE$ = "n")) THEN BEGIN               ! OAS
                STOP                                                   ! OAS
            ENDIF                                                      ! OAS

        ENDIF ELSE BEGIN                                               ! OAS

            !If the user runs MINPRINT without any parameter,          ! OAS
            !then the program will display the help details            ! OAS

            GOSUB MINPRINT.HELP                                        ! OAS
            STOP                                                       ! OAS
        ENDIF                                                          ! OAS

    ENDIF

    ADXSERVE.DATA$ = "MINPRINT has started"                            ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    GOSUB CREATE.MINOK.FILE                                            ! OAS

    IF MINPRINT.ALREADY.RUN% THEN BEGIN                                ! RTT
       ADXSERVE.DATA$ = "MINPRINT already run. " +                     \ RTT
                        "Rerun will overwrite CLILF/CLOLF backups"     ! RTT
       GOSUB DISPLAY.MESSAGE                                           ! RTT
       GOSUB STOP.PROGRAM                                              ! RTT
    ENDIF                                                              ! RTT

    !Processing of RFCNTLST.DAY report which is defined in MINRFCNT.BAS! OAS
    GOSUB CALL.MINRFCNT                                                ! OAS

    GOSUB INITIALISATION
    GOSUB GET.SOFTS

    DO.MAIN = 1                                                        ! CNS

    !If RF is ON, then will instruct RF server to close all files and  ! OAS
    !backup the CLILF and CLOLF files else it will continue with the   ! OAS
    !backup of CLILF/CLOLF files                                       ! OAS

    IF MATCH(" ACTIVE",SOFTS.RECORD$,1) THEN BEGIN                     ! NCS
       ADXSERVE.DATA$ = "Detected RF / Network PDT  Store"             ! DCS ! NCS
       GOSUB DISPLAY.MESSAGE                                           ! DCS
       DO.MAIN = 0                                                     ! CNS

       \***********************************************************    ! FCS ! OAS
       \***                                                            ! OAS
       \***  Instruct the RF Server to close all files                 ! FCS ! OAS
       \***                                                            ! OAS
       \***********************************************************    ! FCS ! OAS

       IF END #64 THEN RFS.BYPASS                                      ! HBG
       OPEN "PI:rfscomms" AS 64                                        ! FCS
       PIPE.OPEN$ = "Y"                                                ! FCS
       PRINT USING "&"; #64 ; "CLS*"                                   ! FCS
       CLOSE 64                                                        ! FCS
       PIPE.OPEN$ = "N"                                                ! FCS
       ADXSERVE.DATA$ = "Waiting for RFS Close."                       ! FCS
       GOSUB DISPLAY.MESSAGE                                           ! FCS
       WAIT ;15000                                                     ! FCS

    ENDIF                                                              ! OAS

RFS.BYPASS:                                                            ! HBG
    GOSUB BACKUP.CLILF.CLOLF.FILES                                     ! OAS
    GOSUB RF.PROCESS                                                   ! CNS
    !Earlier, only RF/POD stores have the CLOLF/CLILF files present
    !(around 900 stores). All other stores are PDT stores and they do
    !not use the CLOLF/CLILF. However as part of SFA, these files need
    !creating and populating like RF/POD stores. Hence, removing the
    !seprate processing section for the ‘non RF’ stores

    !ENDIF ELSE BEGIN                                                  ! DCS ! OAS
    !  ADXSERVE.DATA$ = "Detected NON-RF Store"                        ! DCS ! NCS
    !  GOSUB DISPLAY.MESSAGE                                           ! DCS ! OAS
    !                                                                        ! OAS
    !  GOSUB PROCESS.RFSCF                                             ! LJT ! OAS
    !                                                                        ! OAS
    !  ADXSERVE.DATA$ = "Open MINLS file"                              ! LJT ! OAS
    !  GOSUB DISPLAY.MESSAGE                                           ! LJT ! OAS
    !  CURRENT.SESS.NUM% = MINLS.SESS.NUM%                             ! LJT ! OAS
    !  IF END #MINLS.SESS.NUM% THEN FILE.ERROR.EXIT                    ! LJT ! OAS
    !  OPEN MINLS.FILE.NAME$ KEYED RECL MINLS.RECL% AS MINLS.SESS.NUM% \ LJT ! OAS
    !                                                            NODEL ! LJT ! OAS
    !                                                                        ! OAS
    !  ADXSERVE.DATA$ = "Open IDF file"                                ! LJT ! OAS
    !  GOSUB DISPLAY.MESSAGE                                           ! LJT ! OAS
    !  CURRENT.SESS.NUM% = IDF.SESS.NUM%                               ! LJT ! OAS
    !  IF END #IDF.SESS.NUM% THEN FILE.ERROR.EXIT                      ! LJT ! OAS
    !  OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%       \ LJT ! OAS
    !                                                           NODEL  ! LJT ! OAS
    !                                                                        ! OAS
    !  RC% = PROCESS.KEYED.FILE(MINLS.FILE.NAME$,MINLS.REPORT.NUM%,"N")! LJT ! OAS
    !                                                                        ! OAS
    !  CLOSE MINLS.SESS.NUM%                                           ! LJT ! OAS
    !ENDIF                                                             ! DCS ! OAS

    GOSUB WRITE.MINOK                                                  ! OAS

    GOSUB TERMINATION


STOP.PROGRAM:

    ADXSERVE.DATA$ = "--- MINPRINT has finished ---"                   ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS


    STOP


\*****************************************************************************
\***
\***    MINPRINT.HELP
\***
\***..........................................................................

MINPRINT.HELP:

    PRINT "                         MINPRINT HELP                                       "! OAS
    PRINT "                                                                             "! OAS
    PRINT "This program requires the RERUN parameter to be entered for running          "! OAS
    PRINT "from command mode. For a rerun, the complete program will run, and it        "! OAS
    PRINT "must be noted that the following will take place:                            "! OAS
    PRINT "                                                                             "! OAS
    PRINT "-    backs up the CLOLF.BIN & CLILF.BIN count list files to CLOLF.nnn and    "! OAS
    PRINT "     CLILF.nnn where nnn is MON or TUE etc depending on todays date. This    "! OAS
    PRINT "     will overwrite any existing backup files which feed into the Weekly     "! OAS
    PRINT "     Count reporting                                                         "! OAS
    PRINT "-    creates empty CLOLF.BIN and CLILF.BIN files deleting any counts         "! OAS
    PRINT "     currently on the files                                                  "! OAS
    PRINT "-    it is recommended to manually backup all CLILF/CLOLF backups and BIN    "! OAS
    PRINT "     before rerunning MINPRINT                                               "! OAS
    PRINT "                                                                             "! OAS

RETURN

\*****************************************************************************
\***
\***    WRITE.MINOK
\***
\***..........................................................................

WRITE.MINOK:                                                            ! OAS

    ADXSERVE.DATA$ = "Updating MINOK file"                              ! OAS
    GOSUB DISPLAY.MESSAGE                                               ! OAS

    FUNCTION.FLAG$ EQ "C"                                               ! OAS
    CURRENT.SESS.NUM% = MINOK.SESS.NUM%                                 ! OAS
    IF END #MINOK.SESS.NUM% THEN FILE.ERROR.EXIT                        ! OAS
    CREATE MINOK.FILE.NAME$ AS MINOK.SESS.NUM%                          ! OAS

    FILE.OPERATION$ = "W"                                               ! OAS
    CURRENT.REPORT.NUM% = MINOK.REPORT.NUM%                             ! OAS

    MINOK.ERROR.CHK% = FALSE                                            ! OAS

!                             MINOK FILE FORMAT                                    ! OAS
!                                                                                  ! OAS
!   |--------------------------|-------------|---------------------------------|   ! OAS
!   |Field Name                |  Field Type | Description                     |   ! OAS
!   |--------------------------|-------------|---------------------------------|   ! OAS
!   |Run date                  |  8 ASC      | CCYYMMDD                        |   ! OAS
!   |Run time                  |  6 ASC      | HHMMSS                          |   ! OAS
!   |Reporting status          |  1 ASC      | E = Report processing successful|   ! OAS
!   |                          |             | X = Report processing failed    |   ! OAS
!   |CLILF backup status       |  1 ASC      | E = Backup successful           |   ! OAS
!   |                          |             | X = Backup failed               |   ! OAS
!   |CLOLF backup status       |  1 ASC      | E = Backup successful           |   ! OAS
!   |                          |             | X = Backup failed               |   ! OAS
!   |MINLS housekeeping status |  1 ASC      | E = Processing successful       |   ! OAS
!   |                          |             | X = Processing failed           |   ! OAS
!   |Filler                    |  62 ASC     | Spaces                          |   ! OAS
!   |--------------------------|-------------|---------------------------------|   ! OAS

!    WRITE #MINOK.SESS.NUM%;                             \               ! OAS  ! RTT
!        LEFT$(MINOK.RECORD$, 14) +                      \               ! OAS  ! RTT
!        REPORTING.STATUS$ +                             \               ! OAS  ! RTT
!        CLILF.BKUP.STATUS$ +                            \               ! OAS  ! RTT
!        CLOLF.BKUP.STATUS$ +                            \               ! OAS  ! RTT
!        MINLS.HK.STATUS$ +                              \               ! OAS  ! RTT
!        STRING$(62, " ")                                                ! OAS  ! RTT

    WRITE #MINOK.SESS.NUM%;                             \               ! RTT
        MINOK.RUN.DATE$ +                               \               ! RTT
        MINOK.RUN.TIME$ +                               \               ! RTT
        REPORTING.STATUS$ +                             \               ! RTT
        CLILF.BKUP.STATUS$ +                            \               ! RTT
        CLOLF.BKUP.STATUS$ +                            \               ! RTT
        MINLS.HK.STATUS$ +                              \               ! RTT
        STRING$(62, " ")                                                ! RTT

       MINOK.ERROR.CHK% = TRUE                                          ! OAS

MINOK.ERROR:                                                            ! OAS

    IF MINOK.ERROR.CHK% = FALSE THEN BEGIN                              ! OAS

        ADXSERVE.DATA$ = "MINOK file not processed successfully - ERROR"! OAS
        GOSUB DISPLAY.MESSAGE                                           ! OAS

    ENDIF ELSE IF MINOK.ERROR.CHK% = TRUE THEN BEGIN                    ! OAS

        ADXSERVE.DATA$ = "MINOK file processed successfully"            ! OAS
        GOSUB DISPLAY.MESSAGE                                           ! OAS

    ENDIF                                                               ! OAS

    CLOSE MINOK.SESS.NUM%                                               ! OAS

RETURN                                                                  ! OAS

\*****************************************************************************
\***
\***    INITIALISATION
\***
\***..........................................................................


INITIALISATION:


    ADXSERVE.DATA$ = "INITIALISATION"                                ! DCS
    GOSUB DISPLAY.MESSAGE                                            ! DCS
    GOSUB INITIALISE.VARIABLES

    GOSUB ALLOCATE.SESSION.NUMBERS

RETURN


\*****************************************************************************
\***
\***    INITIALISATION SPECIFIC ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    INITIALISE.VARIABLES
\***    Sets program variables.
\***
\***..........................................................................


INITIALISE.VARIABLES:

    ADXSERVE.DATA$ = "INITIALISE.VARIABLES"                            ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    PROGRAM$           EQ "MINPRINT"
    MODULE$            EQ "00"
    MODULE.NUMBER$     EQ  PROGRAM$ + MODULE$
    BATCH.SCREEN.FLAG$ EQ "B" ! Batch

    FALSE = 0                                                          ! OAS
    TRUE  = -1                                                         ! OAS

    YESTERDAY$ = "      "                                              ! RTT

    CLILF.BKUP.STATUS$ = "X"                                           ! OAS
    CLOLF.BKUP.STATUS$ = "X"                                           ! OAS
    MINLS.HK.STATUS$   = "E"                                           ! OAS

RETURN

!!!**************************************************************************
!!!**************************************************************************
!!!
!!!   CALL.MINRFCNT
!!!
!!!   Call MINRFCNT module to write information to report file.
!!!
!!!**************************************************************************

CALL.MINRFCNT:

   CALL MINRFCNT

RETURN

\*****************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS
\***    Perform CALL.F20.SESS.NUM.UTILITY to allocate file session numbers
\***    for all files referenced by the program.
\***
\***..........................................................................


ALLOCATE.SESSION.NUMBERS:

    ADXSERVE.DATA$ = "ALLOCATE.SESSION.NUMBERS"                        ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    CALL RPRPT.SET
    CALL PRINT.SET
    CALL SOFTS.SET                                                     ! CNS
    CALL RFSCF.SET                                                     ! CNS
    CALL CLOLF.SET                                                     ! CNS
    CALL CLILF.SET                                                     ! CNS
    !CALL ISF.SET                                                      ! CNS ! OAS
    !CALL IRF.SET                                                      ! CNS ! OAS
    CALL IDF.SET                                                       ! CNS
    CALL PGF.SET                                                       ! KMG
    CALL MINLS.SET                                                     ! LJT
    CALL SRITL.SET                                                     ! OAS

    FUNCTION.FLAG$ EQ "O"

    PASSED.INTEGER% EQ RPRPT.REPORT.NUM%
    PASSED.STRING$ EQ RPRPT.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    RPRPT.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    PASSED.INTEGER% = PRINT.REPORT.NUM%
    PASSED.STRING$ = PRINT.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    PRINT.SESS.NUM% = F20.INTEGER.FILE.NO%

    PASSED.INTEGER% EQ SOFTS.REPORT.NUM%                               ! CNS
    PASSED.STRING$ EQ SOFTS.FILE.NAME$                                 ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS
    SOFTS.SESS.NUM% EQ F20.INTEGER.FILE.NO%                            ! CNS

    PASSED.INTEGER% EQ RFSCF.REPORT.NUM%                               ! CNS
    PASSED.STRING$ EQ RFSCF.FILE.NAME$                                 ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS
    RFSCF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                            ! CNS

    PASSED.INTEGER% EQ CLOLF.REPORT.NUM%                               ! CNS
    PASSED.STRING$ EQ CLOLF.FILE.NAME$                                 ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS
    CLOLF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                            ! CNS

    PASSED.INTEGER% EQ CLILF.REPORT.NUM%                               ! CNS
    PASSED.STRING$ EQ CLILF.FILE.NAME$                                 ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS
    CLILF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                            ! CNS

    !Commenting the below as it is not required any more                     ! OAS
    !PASSED.INTEGER% EQ ISF.REPORT.NUM%                                ! CNS ! OAS
    !PASSED.STRING$ EQ ISF.FILE.NAME$                                  ! CNS ! OAS
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                   ! CNS ! OAS
    !ISF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                             ! CNS ! OAS

    !PASSED.INTEGER% EQ IRF.REPORT.NUM%                                ! CNS ! OAS
    !PASSED.STRING$ EQ IRF.FILE.NAME$                                  ! CNS ! OAS
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                   ! CNS ! OAS
    !IRF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                             ! CNS ! OAS

    PASSED.INTEGER% EQ IDF.REPORT.NUM%                                 ! CNS
    PASSED.STRING$ EQ IDF.FILE.NAME$                                   ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS
    IDF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                              ! CNS

    PASSED.INTEGER% EQ PGF.REPORT.NUM%                                 ! KMG
    PASSED.STRING$ EQ PGF.FILE.NAME$                                   ! KMG
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! KMG
    PGF.SESS.NUM% EQ F20.INTEGER.FILE.NO%                              ! KMG

    PASSED.INTEGER% EQ MINLS.REPORT.NUM%                               ! LJT
    PASSED.STRING$ EQ MINLS.FILE.NAME$                                 ! LJT
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! LJT
    MINLS.SESS.NUM% EQ F20.INTEGER.FILE.NO%                            ! LJT

    PASSED.INTEGER% = SRITL.REPORT.NUM%                                ! OAS
    PASSED.STRING$ = SRITL.FILE.NAME$                                  ! OAS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! OAS
    SRITL.SESS.NUM% = F20.INTEGER.FILE.NO%                             ! OAS

    ADXSERVE.DATA$ = "Session numbers allocated "                      ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

RETURN

\*****************************************************************************
\***
\***    CREATE.MINOK.FILE
\***    Allocating Session number for MINOK file
\***    Creating MINOK File
\***    Writing initial values to the file
\***
\***..........................................................................

CREATE.MINOK.FILE:                                                     ! OAS

    ADXSERVE.DATA$ = "Creating MINOK File"                             ! OAS
    GOSUB DISPLAY.MESSAGE                                              ! OAS

    !Initialised a temporary report number for MINOK file              ! OAS
    MINOK.REPORT.NUM% = 450                                            ! OAS
    MINOK.FILE.NAME$ = "D:/ADX_UDT1/MINOK.BIN"                         ! OAS

    FUNCTION.FLAG$ EQ "O"                                              ! OAS

    PASSED.INTEGER% = MINOK.REPORT.NUM%                                ! OAS
    PASSED.STRING$ = MINOK.FILE.NAME$                                  ! OAS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! OAS
    MINOK.SESS.NUM% = F20.INTEGER.FILE.NO%                             ! OAS

    FUNCTION.FLAG$ EQ "C"                                              ! OAS

    CURRENT.SESS.NUM% = MINOK.SESS.NUM%                                ! OAS
!    IF END #MINOK.SESS.NUM% THEN FILE.ERROR.EXIT                       ! OAS

    IF END #MINOK.SESS.NUM% THEN NO.PREV.RUN.DETAILS                   ! RTT
    OPEN MINOK.FILE.NAME$ AS MINOK.SESS.NUM%                           ! RTT

    IF END #MINOK.SESS.NUM% THEN NO.PREV.RUN.DETAILS                   ! RTT
    READ FORM "C1,C8,C6,4C1,C62"; #MINOK.SESS.NUM%;                    \ RTT
        QUOTES$,                                                       \ RTT
        MINOK.RUN.DATE$,                                               \ RTT
        MINOK.RUN.TIME$,                                               \ RTT
        REPORTING.STATUS$,                                             \ RTT
        CLILF.BKUP.STATUS$,                                            \ RTT
        CLOLF.BKUP.STATUS$,                                            \ RTT
        MINLS.HK.STATUS$,                                              \ RTT
        MINOK.FILLER$                                                  ! RTT

    GOTO PROCESS.MINOK.RECORD                                          ! RTT

NO.PREV.RUN.DETAILS:                                                   ! RTT

PROCESS.MINOK.RECORD:                                                  ! RTT

    F02.DATE$ = DATE$                                                  ! RTT
    RC% = UPDATE.DATE(-1)                                              ! RTT
    YESTERDAY$ = F02.DATE$                                             ! RTT

    IF TIME$ > "120000" THEN BEGIN                                     ! RTT
       CURR.RUN.DAY$ = DATE$ + "120000"                                ! RTT
    ENDIF ELSE BEGIN                                                   ! RTT
       CURR.RUN.DAY$ = YESTERDAY$ + "120000"                           ! RTT
    ENDIF                                                              ! RTT

    MINOK.TIME.STAMP$ = RIGHT$(MINOK.RUN.DATE$,6) + MINOK.RUN.TIME$    ! RTT

    IF MINOK.TIME.STAMP$ > CURR.RUN.DAY$ THEN BEGIN                    ! RTT
       MINPRINT.ALREADY.RUN% = -1                                      ! RTT
    ENDIF                                                              ! RTT

    IF NOT MINPRINT.ALREADY.RUN% THEN BEGIN                            ! RTT
        MINOK.RUN.DATE$ = "20" + DATE$                                 ! RTT
        MINOK.RUN.TIME$ = TIME$                                        ! RTT
        CLOSE MINOK.SESS.NUM%                                          ! RTT
        CREATE MINOK.FILE.NAME$ AS MINOK.SESS.NUM%                     ! OAS

!        MINOK.RECORD$ = ("20" + DATE$) +               \               ! OAS ! RTT
!                        TIME$ +                        \               ! OAS ! RTT
!                        "XXXX" +                       \               ! OAS ! RTT
!                        STRING$(62, " ")                               ! OAS ! RTT

        MINOK.RECORD$ = MINOK.RUN.DATE$ +               \              ! RTT
                        MINOK.RUN.TIME$ +               \              ! RTT
                        "XXXX" +                        \              ! RTT
                        STRING$(62, " ")                               ! RTT

        WRITE #MINOK.SESS.NUM%; MINOK.RECORD$                          ! OAS
    ENDIF                                                              ! RTT

    CLOSE MINOK.SESS.NUM%                                              ! OAS

RETURN                                                                 ! OAS

\*****************************************************************************
\***
\***    GET.SOFTS
\***
\***..........................................................................

GET.SOFTS:                                                             ! CNS

    ADXSERVE.DATA$ = "GET.SOFTS"                                       ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    FILE.OPERATION$ = "O"                                              ! CNS
    CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%                            ! CNS

    CURRENT.SESS.NUM% = SOFTS.SESS.NUM%                                ! CNS
    IF END # SOFTS.SESS.NUM% THEN FILE.ERROR.EXIT                      ! CNS
    OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL% \                      CNS
      AS SOFTS.SESS.NUM% NOWRITE NODEL                                 ! CNS

    FILE.SIZE% = SIZE(SOFTS.FILE.NAME$)                                ! CNS

    IF FILE.SIZE% = 0 \ ! File is empty                                  CNS
       OR MOD(FILE.SIZE%,SOFTS.RECL%) <> 0 THEN \ ! File is corrupt      CNS
       BEGIN                                                           ! CNS
          ADXSERVE.DATA$ = "SOFTS file is corrupted"                   ! DCS
          GOSUB DISPLAY.MESSAGE                                        ! DCS
          FILE.OPERATION$ = "S"                                        ! CNS
          GOTO FILE.ERROR.EXIT                                         ! CNS
       ENDIF                                                           ! CNS

    SOFTS.REC.NUM% = FILE.SIZE% / SOFTS.RECL%                          ! CNS

    SOFTS.RECORD$ = ""                                                 ! CNS

    IF SOFTS.REC.NUM% >= 20 THEN BEGIN                                 ! CNS
       SOFTS.REC.NUM% = 20                                             ! CNS
       RC% = READ.SOFTS                                                ! CNS
    ENDIF                                                              ! CNS

    CLOSE SOFTS.SESS.NUM%                                              ! CNS

RETURN                                                                 ! CNS

\*****************************************************************************
\***
\***    BACKUP OF CLILF AND CLOLF FILES AT EOD
\***
\***..........................................................................

BACKUP.CLILF.CLOLF.FILES:                                              ! OAS

!   GET.DATE$ = DATE$                                                  ! OAS ! PBT

    F02.DATE$ = DATE$                                                  ! PBT
    TIME.NOW$ = TIME$                                                  ! PBT
    IF TIME.NOW$ < "040000" THEN BEGIN                                 ! PBT
        !If it comes here, we need to set yesterday as the backup file ! PBT
        !extnesion.                                                    ! PBT
        CALL UPDATE.DATE(-1) !FO2.DATE$ = FO2.DATE$ - 1Day             ! PBT
    ENDIF                                                              ! PBT

    !Getting day using PSDATE FN                                       ! PBT
!   CALL PSDATE(GET.DATE$)                                             ! OAS ! OBT
    CALL PSDATE(F02.DATE$)                                             ! OBT
    GET.DAY$ = F13.DAY$                                                ! OAS

    !First MINPRINT will backup CLILF.BIN. If CLILF backup is          ! OAS
    !successful, then it will backup CLOLF.BIN                         ! OAS

    !Taking backup of CLILF.BIN                                        ! OAS
    ADXSERVE.DATA$ = "Taking backup of CLILF.BIN"                      ! OAS
    GOSUB DISPLAY.MESSAGE                                              ! OAS

    CLILF.DDD$ = "D:\ADX_UDT1\CLILF." + GET.DAY$                       ! OAS
    CALL ADXCOPYF(ADXCOPY.CHK%,"D:\ADX_UDT1\CLILF.BIN",      \         ! OAS
                                      CLILF.DDD$,0,1,0)                ! OAS

   !Checking whether backup is created or not while using ADXCOPY FN   ! OAS
    IF ADXCOPY.CHK% = 0 THEN BEGIN                                     ! OAS

        ADXSERVE.DATA$ = "Backup of CLILF.BIN was completed successfully"    ! OAS
        GOSUB DISPLAY.MESSAGE                                          ! OAS

        CLILF.BKUP.STATUS$ = "E"                                       ! OAS

        !Taking backup of CL0LF.BIN                                    ! OAS
        ADXSERVE.DATA$ = "Taking backup of CLOLF.BIN"                  ! OAS
        GOSUB DISPLAY.MESSAGE                                          ! OAS

        CLOLF.DDD$ = "D:\ADX_UDT1\CLOLF." + GET.DAY$                   ! OAS
        CALL ADXCOPYF(ADXCOPY.CHK%,"D:\ADX_UDT1\CLOLF.BIN",  \         ! OAS
                                          CLOLF.DDD$,0,1,0)            ! OAS

        !Checking whether backup is created using ADXCOPY FN           ! OAS
        IF ADXCOPY.CHK% = 0 THEN BEGIN                                 ! OAS

           ADXSERVE.DATA$ = "Backup of CLOLF.BIN was completed successfully" ! OAS
           GOSUB DISPLAY.MESSAGE                                       ! OAS
           CLOLF.BKUP.STATUS$ = "E"                                    ! OAS

        ENDIF ELSE BEGIN                                               ! OAS

           ADXSERVE.DATA$ = "*****ERROR: Could not backup CLOLF.BIN*****"    ! OAS
           GOSUB DISPLAY.MESSAGE                                       ! OAS

        ENDIF                                                          ! OAS

    ENDIF ELSE BEGIN                                                   ! OAS

       ADXSERVE.DATA$ = "*****ERROR: Could not backup CLILF.BIN  *****"! OAS
       GOSUB DISPLAY.MESSAGE                                           ! OAS

       ADXSERVE.DATA$ = "*****ERROR: So CLOLF.BIN backup not done*****"! OAS
       GOSUB DISPLAY.MESSAGE                                           ! OAS

    ENDIF                                                              ! OAS

RETURN                                                                 ! OAS

\*****************************************************************************
\***
\***    RF PROCESS
\***
\***    Open RFSCF
\***    Read RFSCF
\***    Create CLILF & CLOLF
\***    Read through RPRPT and extract required information
\***    Write to CLILF & CLOLF files
\***..........................................................................

RF.PROCESS:                                                            ! CNS

      ADXSERVE.DATA$ = "RF.PROCESS"                                    ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

      GOSUB PROCESS.RFSCF                                              ! LJT

      FILE.OPERATION$ = "O"                                            ! CNS

      IF RC% = 1 THEN BEGIN                                            ! CNS
         DO.MAIN = 1                                                   ! CNS
         GOTO END.RF.PROCESS                                           ! CNS
      ENDIF                                                            ! CNS

      ADXSERVE.DATA$ = "Create CLILF file"                             ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS
      CURRENT.SESS.NUM% = CLILF.SESS.NUM%                              ! CNS
      IF END# CLILF.SESS.NUM% THEN CREATE.ERROR                        ! CNS
      CREATE POSFILE CLILF.FILE.NAME$ KEYED 6,,,10000 RECL CLILF.RECL% \ FCS
             AS CLILF.SESS.NUM% MIRRORED PERUPDATE                     ! CNS

      ADXSERVE.DATA$ = "Create CLOLF file"                             ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS
      CURRENT.SESS.NUM% = CLOLF.SESS.NUM%                              ! CNS
      IF END# CLOLF.SESS.NUM% THEN CREATE.ERROR                        ! CNS
      CREATE POSFILE CLOLF.FILE.NAME$ DIRECT 0 RECL CLOLF.RECL%        \ MJT
             AS CLOLF.SESS.NUM% MIRRORED ATCLOSE                       ! CNS

      !Commenting the below section as it not required anymore
      !ADXSERVE.DATA$ = "Open ISF file"                                ! DCS !OAS
      !GOSUB DISPLAY.MESSAGE                                           ! DCS !OAS
      !CURRENT.SESS.NUM% = ISF.SESS.NUM%                               ! CNS !OAS
      !IF END #ISF.SESS.NUM% THEN FILE.ERROR.EXIT                      ! CNS !OAS
      !OPEN ISF.FILE.NAME$ KEYED RECL ISF.RECL% AS ISF.SESS.NUM%       \ CNS !OAS
      !   NOWRITE NODEL                                                ! CNS !OAS

      !ADXSERVE.DATA$ = "Open IRF file"                                ! DCS !OAS
      !GOSUB DISPLAY.MESSAGE                                           ! DCS !OAS
      !CURRENT.SESS.NUM% = IRF.SESS.NUM%                               ! CNS !OAS
      !IF END #IRF.SESS.NUM% THEN FILE.ERROR.EXIT                      ! CNS !OAS
      !OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%       \ CNS !OAS
      !   NOWRITE NODEL                                                ! CNS !OAS

      ADXSERVE.DATA$ = "Open IDF file"                                 ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS
      CURRENT.SESS.NUM% = IDF.SESS.NUM%                                ! CNS
      IF END #IDF.SESS.NUM% THEN FILE.ERROR.EXIT                       ! CNS
      OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%        \ CNS
         NODEL                                                         ! CNS

      ADXSERVE.DATA$ = "Open RPRPT file"                               ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS
      CURRENT.SESS.NUM% = RPRPT.SESS.NUM%                              ! CNS
      IF END # RPRPT.SESS.NUM% THEN FILE.ERROR.EXIT                    ! CNS

      OPEN RPRPT.FILE.NAME$ AS RPRPT.SESS.NUM%                         ! CNS

      IF END #RPRPT.SESS.NUM% THEN RPRPT.EOF                           ! CNS

      ADXSERVE.DATA$ = "Open PGF file"                                 ! KMG
      GOSUB DISPLAY.MESSAGE                                            ! KMG
      CURRENT.SESS.NUM% = PGF.SESS.NUM%                                ! KMG
      IF END #PGF.SESS.NUM% THEN FILE.ERROR.EXIT                       ! KMG
      OPEN PGF.FILE.NAME$ KEYED RECL PGF.RECL% AS PGF.SESS.NUM%        \ KMG
         NOWRITE NODEL                                                 ! KMG

      ADXSERVE.DATA$ = "Open MINLS file"                               ! LJT
      GOSUB DISPLAY.MESSAGE                                            ! LJT
      CURRENT.SESS.NUM% = MINLS.SESS.NUM%                              ! LJT
      IF END #MINLS.SESS.NUM% THEN FILE.ERROR.EXIT                     ! LJT
      OPEN MINLS.FILE.NAME$ KEYED RECL MINLS.RECL% AS MINLS.SESS.NUM%  ! LJT

      ADXSERVE.DATA$ = "Open SRITML file"                              ! OAS
      GOSUB DISPLAY.MESSAGE                                            ! OAS
      CURRENT.SESS.NUM% = SRITL.SESS.NUM%                              ! OAS
      IF END #SRITL.SESS.NUM% THEN FILE.ERROR.EXIT                     ! OAS
      OPEN SRITL.FILE.NAME$ KEYED RECL SRITL.RECL% AS SRITL.SESS.NUM%  \ OAS
         NOWRITE NODEL

      ADXSERVE.DATA$ = "Set CLOLF/CLILF default values"                ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS
      EOF = 0                                                          ! CNS

     !OLD variables which are commented are put together,              ! OAS
     !so that it can be taken out easily during the next release
     !OLD.BULETT$ = " "                                                ! CNS ! OAS
     !OLD.PGNAME$ = " "                                                ! ICC ! OAS
     !CLOLF.CNTDATE$ = "19850101"                                      ! CNS ! OAS
     !CLOLF.LISTID$ = "001"                                            ! CNS ! OAS
     !CLOLF.HOLISTID$ = "0000"                                         ! CNS ! OAS
     !CLOLF.OSSRITEMS$ = "000"                                         ! KMG ! OAS
     !CLILF.BSCNT$ = "-001"                                            ! CNS ! OAS
     !CLILF.SFCNT$ = "-001"                                            ! CNS ! OAS
     !CLILF.SALESSFCNT$ = "0000"                                       ! CNS ! OAS
     !CLILF.HO.SEQNO$   = "00"                                         ! FCS ! OAS
     !CLILF.FILLER$ = "   "                                            ! CNS ! JMG
     !CLILF.OSSR.ITMSTKCNT$ = "-001"                                   ! JMG ! OAS
     !CLOLF.ACTIVE.STATUS$ = " "                                       ! CNS ! OAS

      LIST.ITEMS = 1                                                   ! CNS
      CLOLF.LISTID$ = "001"                    !Set to one             !     ! OAS
      CLOLF.USERID$ = "000"                    !Set to zero            ! KMG ! OAS
      CLOLF.LSTTYP$ = "R"                      !Recount List           ! CNS ! OAS
      CLOLF.BULETT$ = " "                      !Set to space           !     ! OAS
      CLOLF.LIST.NAME$ = "Recount List                  "              !     ! OAS
      CLOLF.PICKER.USER.ID$ = "000"            !Initialise to zero     !     ! OAS
      CLOLF.ACTIVE.STATUS$ = "I"               !Initial                !     ! OAS
      CLOLF.PILST.ID$ = "0000"                 !Set to zero            !     ! OAS
      CLOLF.EXPIRY.DATE$ = PACK$("000000")     !Set to zero            !     ! OAS
      CLOLF.PICK.START.TIME$ = PACK$("0000")   !Set to zero            !     ! OAS
      CLOLF.PICK.END.TIME$ = PACK$("0000")     !Set to zero            !     ! OAS
      CLOLF.CURRENT.LOCATION$ = " "            !Set to space           !     ! OAS

      CLOLF.RECORD.NUM% = 1         !REC position to write to CLOLF    ! CNS

      CLILF.HO.SEQNO$   = "00"                 !Set to zero            !     ! OAS
      CLILF.COUNTED.STATUS$ = "U"              !Not Counted            !     ! OAS
      CLILF.DATE.LASTCNT$ = PACK$("000000")    !Set to zero            !     ! OAS
      CLILF.SALESCNT% = 0                      !Set to zero            !     ! OAS
      CLILF.BSCNT% = -1                        !Set to -1              !     ! OAS
      CLILF.OSSR.ITMSTKCNT% = -1               !Set to -1              !     ! OAS
      CLILF.BS.PEND.SA.CNT% = -1               !Set to -1              !     ! OAS
      CLILF.OSSR.PEND.SA.CNT% = -1             !Set to -1              !     ! OAS
      CLILF.SFCNT% = -1                        !Set to -1              !     ! OAS
      CLILF.SPACE$ = STRING$(16," ")                                   !     ! OAS

      ADXSERVE.DATA$ = "Start of Report File processing"               ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

      WHILE NOT EOF                                                    ! CNS

         READ #RPRPT.SESS.NUM%; RPRPT.RECORD$                          ! CNS

        !CLOLF.BULETT$ = LEFT$(RPRPT.RECORD$,1)                        ! CNS ! OAS
        !Getting the Business Center letter                            ! OAS
        BSNS.CNTR$ = LEFT$(RPRPT.RECORD$,1)                            ! OAS

        !IF CLOLF.BULETT$ >= "A" AND CLOLF.BULETT$ <= "Z" THEN BEGIN   ! CNS ! OAS

        IF BSNS.CNTR$ >= "A" AND BSNS.CNTR$ <= "Z" THEN BEGIN
           IF MID$(RPRPT.RECORD$,2,1) = " " THEN BEGIN                 ! CNS
              GOSUB EXTRACT.FIELDS                                     ! CNS
              !ISF.BOOTS.CODE$ = PACK$("0" + CLILF.BOOTSCODE$)         ! CNS ! OAS
              !ISF.S.E.DESC$ = "X "                                    ! HBG ! OAS
              !RC% = READ.ISF                                          ! CNS ! OAS

              !Removed the addition of "0" in CLILF.BOOTSCODE$ as      ! OAS
              !the new boots code contains 8 digits

              !MINLS.ITEM.CODE$ = PACK$("0" + CLILF.BOOTSCODE$)        ! LJT ! OAS
              MINLS.ITEM.CODE$ = PACK$(CLILF.BOOTSCODE$)               ! LJT ! OAS

              RC% = READ.MINLS                                         ! LJT

              IF RC% = 0 THEN BEGIN                                    ! LJT
                 IF DATE.LE(UNPACK$(MINLS.RECOUNT.DATE$),DATE$)        \ LJT
                    THEN BEGIN                                         ! LJT
                    F02.DATE$ = UNPACK$(MINLS.RECOUNT.DATE$)           ! LJT
                    CALL UPDATE.DATE(RFSCF.RECOUNT.DAYS.RETAIN%)       ! LJT

                    IF DATE.LE(F02.DATE$, DATE$) THEN BEGIN            ! LJT

                       !Setting FILE.OPERATION$ flag to "D" for        ! OAS
                       !catching any error while deleting the record   ! OAS

                       FILE.OPERATION$ = "D"                           ! OAS
                       DELREC MINLS.SESS.NUM%; MINLS.ITEM.CODE$        ! LJT
                       FILE.OPERATION$ = " "                           ! OAS

                       !Turn off PENDING COUNT flag on the IDF         ! LJT
                       IDF.BOOTS.CODE$ = MINLS.ITEM.CODE$              ! LJT
                       RC% = READ.IDF                                  ! LJT
                       IF RC% = 0 THEN BEGIN                           ! LJT
                         !Make sure the flag is set to on first        ! LJT
                         IF (IDF.BIT.FLAGS.2% AND 00000100b) <> 0      \ LJT
                             THEN BEGIN                                ! LJT
                             !It's on, so turn it off                  ! LJT
                             IDF.BIT.FLAGS.2% = (IDF.BIT.FLAGS.2% XOR 4)!LJT
                             RC% = WRITE.IDF                           ! LJT
                         ENDIF                                         ! LJT
                      ENDIF                                            ! LJT
                    ENDIF ELSE BEGIN                                   ! LJT
                       GOSUB VALID.ITEM                                ! CNS
                    ENDIF                                              ! LJT
                 ENDIF                                                 ! LJT
              ENDIF                                                    ! LJT
           ENDIF                                                       ! CNS
        ENDIF                                                          ! OAS
        !ELSE BEGIN                                                    ! CNS ! OAS
        !   IF CLOLF.BULETT$ = "*" THEN BEGIN                          ! CNS ! OAS
        !      UPDATE.DATE$ = MID$(RPRPT.RECORD$,41,2)                 ! CNS ! OAS
        !      IF VAL(UPDATE.DATE$) > 85 THEN BEGIN                    ! CNS ! OAS
        !         UPDATE.DATE$ = "19" + UPDATE.DATE$                   ! CNS ! OAS
        !      ENDIF ELSE BEGIN                                        ! CNS ! OAS
        !         UPDATE.DATE$ = "20" + UPDATE.DATE$                   ! CNS ! OAS
        !      ENDIF                                                   ! CNS ! OAS
        !      CLOLF.CNTDATE$ = UPDATE.DATE$ +                         \ CNS ! OAS
        !                       MID$(RPRPT.RECORD$,38,2) +             \ CNS ! OAS
        !                       MID$(RPRPT.RECORD$,35,2)               ! CNS ! OAS
        !   ENDIF                                                      ! CNS ! OAS
        !ENDIF                                                         ! CNS ! OAS


RPRPT.EOF.RETURN:                                                      ! CNS

      WEND                                                             ! CNS

      ADXSERVE.DATA$ = "End of Report File processing"                 ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

      !GOSUB CREATE.NEW.LIST                                           ! CNS ! OAS
      GOSUB CREATE.RECOUNT.LIST                                        ! OAS

      ADXSERVE.DATA$ = "Closing Files"                                 ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

      CLOSE RPRPT.SESS.NUM%                                            ! CNS
      CLOSE CLILF.SESS.NUM%                                            ! CNS
      CLOSE CLOLF.SESS.NUM%                                            ! CNS
      !CLOSE ISF.SESS.NUM%                                             ! CNS ! OAS
      !CLOSE IRF.SESS.NUM%                                             ! CNS ! OAS
      CLOSE IDF.SESS.NUM%                                              ! CNS
      CLOSE MINLS.SESS.NUM%                                            ! LJT
      CLOSE SRITL.SESS.NUM%                                            ! OAS
      ADXSERVE.DATA$ = "Files Closed"                                  ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

END.RF.PROCESS:                                                        ! CNS


RETURN                                                                 ! CNS

RPRPT.EOF:                                                             ! CNS

       EOF = 1                                                         ! CNS
       GOTO RPRPT.EOF.RETURN                                           ! CNS


RFSCF.OPEN.ERROR:                                                      ! CNS

       DO.MAIN = 1                                                     ! CNS
       GOTO END.RF.PROCESS                                             ! CNS

\*****************************************************************************
\***
\***   PROCESS.RFSCF
\***
\***   This code was originally in RF.PROCESS, I've moved it into a subroutine
\***   so that it can also be used if RF is inactive.
\***
\*****************************************************************************

      PROCESS.RFSCF:

      FILE.OPERATION$ = "O"                                            ! CNS

      ADXSERVE.DATA$ = "Open/Read/Close RFSCF file"                    ! DCS
      GOSUB DISPLAY.MESSAGE                                            ! DCS

      IF END #RFSCF.SESS.NUM% THEN RFSCF.OPEN.ERROR                    ! CNS
      CURRENT.SESS.NUM% = RFSCF.SESS.NUM%
      OPEN RFSCF.FILE.NAME$ DIRECT RECL RFSCF.RECL% AS RFSCF.SESS.NUM% \ DCS
            NOWRITE NODEL                                              ! DCS
      RC% = READ.RFSCF1                                                ! CNS

      CLOSE RFSCF.SESS.NUM%                                            ! CNS

      RETURN

\*****************************************************************************
\***
\***   EXTRACT FIELDS
\***
\*****************************************************************************
EXTRACT.FIELDS:                                                        ! CNS

      !Below statements are commented out, since these are             ! OAS
      !not present in the updated CLOLF file format                    ! OAS
      !CLOLF.BUNAME$ = MID$(RPRPT.RECORD$,3,15)                        ! CNS ! OAS
      !CLILF.PRODGRP$ = MID$(RPRPT.RECORD$,19,2) + "0" +               \ CNS ! OAS
      !                 MID$(RPRPT.RECORD$,22,3)                       ! CNS ! OAS
      !CLILF.PRODGRPDESC$ = MID$(RPRPT.RECORD$,28,12)                  ! CNS ! OAS

      !Adding zero to CLILF.BOOTSCODE$ to make it to 8 digit item code ! OAS

      CLILF.BOOTSCODE$ = "0" + MID$(RPRPT.RECORD$,48,2) +              \ CNS ! OAS
                         MID$(RPRPT.RECORD$,51,2) +                    \ CNS ! OAS
                         MID$(RPRPT.RECORD$,54,3)                      ! CNS ! OAS

RETURN                                                                 ! CNS

\*****************************************************************************
\***
\***   VALID ITEM
\***
\***   Extract information from IDF and IRF and write to CLILF
\***
\*****************************************************************************
VALID.ITEM:

       !The subroutine has undergone many changes as part of SFA project     ! OAS

       !CLOLF.PGNAME$ = CLILF.PRODGRPDESC$+"   "                       ! ICC ! OAS
       !IF CLOLF.PGNAME$<> OLD.PGNAME$ OR LIST.ITEMS > 30 THEN BEGIN   ! ICC ! OAS

       !The below IF condition is changed to restrict the                    ! OAS
       !list size to maximum of 999 records.                                 ! OAS
       !IF LIST.ITEMS > 30 THEN BEGIN                                        ! OAS
        IF LIST.ITEMS > 999 THEN BEGIN                                       ! OAS
           GOSUB CREATE.RECOUNT.LIST                                   ! CNS ! OAS
        ENDIF                                                          ! CNS ! OAS

       !CLILF.HO.SEQNO$ = RIGHT$(CLILF.ITEMSEQ$,2)                     ! 1.12 RC ! OAS
       !CLILF.DEALMKR$ = "N"                                           ! CNS ! OAS
       !CLILF.BARCODE$= RIGHT$("000000000000" + \
       !                LEFT$(UNPACK$(IDF.BOOTS.CODE$),7),13)          ! FCS ! OAS
       !CALL CALC.BAR.CODE.CHECK.DIGIT(RIGHT$(CLILF.BARCODE$,12))      ! FCS ! OAS
       !CLILF.BARCODE$ = RIGHT$(CLILF.BARCODE$,12) + F06.CHECK.DIGIT$  ! FCS ! OAS

       CLILF.LISTID$ = CLOLF.LISTID$                                   ! CNS
       CLILF.ITEMSEQ$  = RIGHT$("000" + STR$(LIST.ITEMS),3)            ! CNS

       !IDF.BOOTS.CODE$ = ISF.BOOTS.CODE$                              ! CNS ! OAS
       !IDF.STNDRD.DESC$ = "UNKNOWN ITEM"                              ! FCS ! OAS

       !CLILF.DEALMKR is not present in the updated CLILF format.            ! OAS
       !As CLILF.DEALMKR is not needed, the below code section is            ! OAS
       !not required and hence commented                                     ! OAS

       !IRF.BAR.CODE$ = PACK$("000000000000000" + \
       !                LEFT$(UNPACK$(IDF.BOOTS.CODE$),7))             ! FCS ! OAS
       !RC% = READ.IDF                                                 ! CNS ! OAS

       !IF RC% = 0 THEN BEGIN
         !CHECK TO SEE IF IDF HAS VALID EAN BARCODE TO REPLACE BTC BARCODE        ! FCS ! OAS
         !IF IDF.FIRST.BAR.CODE$ <> PACK$("0000000000000000000000") THEN BEGIN    ! FCS ! OAS
            !IF VAL(UNPACK$(IDF.NO.OF.BAR.CODES$)) > 1   \                        ! FCS ! OAS
               !AND  (IDF.SECOND.BAR.CODE$ <> IRF.BAR.CODE$)  THEN BEGIN          ! FCS ! OAS

               !CALL CALC.BAR.CODE.CHECK.DIGIT(UNPACK$(IDF.SECOND.BAR.CODE$))     ! FCS ! OAS

               !CLILF.BARCODE$ = RIGHT$(UNPACK$(IDF.SECOND.BAR.CODE$) +  \        ! FCS ! OAS
               !     F06.CHECK.DIGIT$,13)                                         ! FCS ! OAS
               !IRF.BAR.CODE$ = PACK$("0000000000"+ UNPACK$(IDF.SECOND.BAR.CODE$))! FCS ! OAS
            !ENDIF ELSE BEGIN                                                     ! FCS ! OAS
               !CALL CALC.BAR.CODE.CHECK.DIGIT(UNPACK$(IDF.FIRST.BAR.CODE$))      ! FCS ! OAS

               !CLILF.BARCODE$ = RIGHT$(UNPACK$(IDF.FIRST.BAR.CODE$) +   \        ! FCS ! OAS
               !     F06.CHECK.DIGIT$,13)                                         ! FCS ! OAS
               !IRF.BAR.CODE$ = PACK$("0000000000"+ UNPACK$(IDF.FIRST.BAR.CODE$)) ! FCS ! OAS
            !ENDIF                                                                ! FCS ! OAS
         !ENDIF                                                                   ! FCS ! OAS


         !RC% = READ.IRF                                               ! CNS ! OAS
         !IF RC% = 0 THEN BEGIN                                        ! CNS ! OAS
            !IF IRF.DEAL.NUM$(0) = PACK$("0000") AND \                 ! HBG ! OAS
            !   IRF.DEAL.NUM$(1) = PACK$("0000") AND \                 ! HBG ! OAS
            !   IRF.DEAL.NUM$(2) = PACK$("0000") THEN BEGIN            ! HBG ! OAS
            !   CLILF.DEALMKR$ = "N"                                   ! HBG ! OAS
            !ENDIF ELSE BEGIN                                          ! HBG ! OAS
            !   CLILF.DEALMKR$ = "Y"                                   ! HBG ! OAS
            !ENDIF                                                     ! HBG ! OAS
         !ENDIF                                                        ! CNS ! OAS
       !ENDIF                                                          ! CNS ! OAS

      ! If SEL description not available, use short one on IDF         ! HBG ! OAS

      !CLILF.SELDESC$ is not present in the updated CLILF format.            ! OAS
      !The conditional statement given below is used to set the              ! OAS
      !variable CLILF.SELDESC$.As CLILF.SELDESC$ is not needed,              ! OAS
      !the below conditional statement is also commented.                    ! OAS
      !IF UCASE$(LEFT$(ISF.S.E.DESC$,2)) = "X " OR \                   ! HBG ! OAS
      !   ISF.S.E.DESC$ = STRING$(45," ") THEN BEGIN                   ! HBG ! OAS
      !   CLILF.SELDESC$ = IDF.STNDRD.DESC$ + STRING$(21," ")!length 45! HBG ! OAS
      !ENDIF ELSE BEGIN                                                ! HBG ! OAS
      !   CLILF.SELDESC$ = ISF.S.E.DESC$                               ! CNS ! OAS
      !ENDIF                                                           ! HBG ! OAS

       GOSUB PROCESS.SRITL   !To get the active planner details of an item   ! OAS

       CLILF.BOOTSCODE$ = PACK$(CLILF.BOOTSCODE$)                      ! OAS
       CURRENT.REPORT.NUM% = CLILF.REPORT.NUM%                         ! CNS
       FILE.OPERATION$ = "W"                                           ! CNS
       RC% = WRITE.CLILF                                               ! CNS
       IF RC% <> 0 THEN GOTO FILE.ERROR.EXIT                           ! CNS

       LIST.ITEMS = LIST.ITEMS + 1                                     ! CNS

RETURN

\*****************************************************************************
\***
\***   New functionality added -                                        ! OAS
\***   PROCESS.SRITL
\***   Extract information from SRITML (Active Planner Details)
\***   for writing to CLILF
\***
\*****************************************************************************
PROCESS.SRITL:

    !Initialising the details for each possible sales floor site        ! OAS
    FOR COUNTER% = 0 TO 31                                              ! OAS
        CLILF.MODULE.ID%(COUNTER%) = 0                                  ! OAS
        CLILF.MODULE.SEQ%(COUNTER%) = 0                                 ! OAS
        CLILF.REPEAT.CNT%(COUNTER%) = 0                                 ! OAS
        CLILF.COUNT%(COUNTER%) = -1                                     ! OAS
        CLILF.FILL.QUANTITY%(COUNTER%) = 0                              ! OAS
        CLILF.FILLER$(COUNTER%) = STRING$(4, " ")                       ! OAS
    NEXT COUNTER%                                                       ! OAS

    !Populate the Planner ID,Module and REPEAT COUNT from SRITML        ! OAS

    !Item code without check digit                                      ! OAS
    SRITL.ITEM.CODE$ = PACK$(MID$(CLILF.BOOTSCODE$,2, 6))               ! OAS

    SRITL.RECORD.CHAIN% = 0                                             ! OAS
    RC% = READ.SRITL                                                    ! OAS
    RECORD.COUNT% = 0                                                   ! OAS

    WHILE RC% = 0                                                       ! OAS

        !For each module key                                                   ! OAS
        FOR LOOPCNT% = 0 TO SRITL.MAX.MOD.KEYS% - 1                            ! OAS
            IF SRITL.POGDB%(LOOPCNT%) AND     \                                ! OAS
               RECORD.COUNT% <= 32 THEN BEGIN                                  ! OAS
               !32 is the maximum limit in CLILF                               ! OAS
                CLILF.MODULE.ID%(RECORD.COUNT%) = SRITL.POGDB%(LOOPCNT%)       ! OAS
                CLILF.MODULE.SEQ%(RECORD.COUNT%) = SRITL.MODULE.SEQ%(LOOPCNT%) ! OAS
                CLILF.REPEAT.CNT%(RECORD.COUNT%) = SRITL.REPEAT.CNT%(LOOPCNT%) ! OAS
                RECORD.COUNT% = RECORD.COUNT% + 1                              ! OAS
            ENDIF ELSE BEGIN                                                   ! OAS
                LOOPCNT% = SRITL.MAX.MOD.KEYS%   !To quit the FOR Loop         ! OAS
                RC% = 1                          !To quit the While Loop       ! OAS
            ENDIF                                                              ! OAS
        NEXT LOOPCNT%                                                          ! OAS

        !Read the next record chain from SRITEM if the FOR loop         ! OAS
        !is exited normally                                             ! OAS

        IF (RECORD.COUNT% <= 32) AND (RC% = 0) THEN BEGIN               ! OAS
            !32 is the maximum limit in CLILF                           ! OAS
            SRITL.RECORD.CHAIN% = SRITL.RECORD.CHAIN% + 1               ! OAS
            RC% = READ.SRITL                                            ! OAS
        ENDIF ELSE IF RECORD.COUNT% > 32 THEN BEGIN                     ! OAS
            !CLILF cannot accomodate > 32                               ! OAS
            ADXSERVE.DATA$ = "Error: Item " + CLILF.BOOTSCODE$          ! OAS
            GOSUB DISPLAY.MESSAGE                                       ! OAS
            ADXSERVE.DATA$ = "       CLILF can only accomodate 32 sites"! OAS
            GOSUB DISPLAY.MESSAGE                                       ! OAS
        ENDIF                                                           ! OAS

    WEND                                                                ! OAS

RETURN                                                                  ! OAS

\*****************************************************************************
\***
\***   CREATE RECOUNT LIST                                             ! OAS - Changed the subroutine name
\***
\*****************************************************************************
!Renamed the sub routine CREATE.NEW.LIST to CREATE.RECOUNT.LIST        ! OAS
CREATE.RECOUNT.LIST:
       !Grouping all the variables commented since they were not in    ! OAS
       !the new CLOLF RECORD. Need to remove these in the next release ! OAS
       !IF OLD.PGNAME$ <> " " THEN BEGIN                               ! ICC
         !CLOLF.SRITEMS$ = RIGHT$("000" + STR$(LIST.ITEMS - 1),3)      ! CNS ! OAS
         !ADXSERVE.DATA$ = "CREATE.NEW.LIST: " + CLOLF.LISTID$         ! DCS ! OAS
         !CLOLF.BSITEMS$ = CLOLF.SRITEMS$                              ! CNS ! OAS
         !CLOLF.TOTAL.ITEMS$ = CLOLF.SRITEMS$                          ! FCS ! OAS
         !CLOLF.OSSRITEMS$ = CLOLF.SRITEMS$                            ! KMG ! OAS
         !CLOLF.BULETT$ = OLD.BULETT$                                  ! CNS ! OAS
         !CLOLF.PGNAME$ = OLD.PGNAME$                                  ! ICC
         !CLOLF.BUNAME$ = CLOLF.PGNAME$                                ! ICC  - Assignment needed to BUNAME as 1
                                                                       ! this is the var the WRITE.CLOLF
                                                                       ! uses to write to the relevant part of
                                                                       ! the CLOLF record
        IF (LIST.ITEMS <= 1) THEN BEGIN \! If no list to populate      ! OAS
            ADXSERVE.DATA$ = "No list to create"                       ! OAS
            GOSUB DISPLAY.MESSAGE
        ENDIF \                                                        ! OAS
        ELSE BEGIN
            ADXSERVE.DATA$ = "CLOLF.LISTID: " + CLOLF.LISTID$          ! OAS
            GOSUB DISPLAY.MESSAGE                                      ! DCS
            CLOLF.TOTAL.ITEMS% = LIST.ITEMS - 1                        ! OAS
            CLOLF.SRITEMS% = CLOLF.TOTAL.ITEMS%                        ! OAS
            CLOLF.BSITEMS% = CLOLF.TOTAL.ITEMS%                        ! OAS
            CLOLF.OSSRITEMS% = CLOLF.TOTAL.ITEMS%                      ! OAS
            CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM%                    ! CNS
            FILE.OPERATION$ = "W"                                      ! CNS
            CLOLF.CREATION.DATE$ = PACK$(DATE$)                        ! OAS
            CLOLF.CREATION.TIME$ = PACK$(LEFT$(TIME$,4))               ! OAS

            RC% = WRITE.CLOLF                                          ! CNS
            IF RC% <> 0 THEN GOTO FILE.ERROR.EXIT                      ! CNS

            !Grouping all the variables commented since they were not in the
            !new CLOLF RECORD. Need to remove these in the next release
            !CLOLF.OSSRITEMS$ = "000"                                  ! KMG ! OAS
            !CLOLF.BULETT$ = LEFT$(RPRPT.RECORD$,1)                    ! CNS ! OAS
            !CLOLF.PGNAME$ = MID$(RPRPT.RECORD$,28,12)+"   "           ! ICC ! OAS

            CLOLF.RECORD.NUM% = CLOLF.RECORD.NUM% + 1                  ! CNS
            CLOLF.LISTID$ = RIGHT$("000" + STR$(VAL(CLOLF.LISTID$) + 1),3)!CNS
            LIST.ITEMS = 1                                             ! CNS

        ENDIF
       !ENDIF                                                          ! CNS ! OAS

         !Commented since they were not in the new CLOLF RECORD.       ! OAS
         !Need to remove these in the next release                     ! OAS
         !OLD.BULETT$ = CLOLF.BULETT$                                  ! CNS ! OAS
         !OLD.PGNAME$ = CLOLF.PGNAME$                                  ! ICC ! OAS

RETURN



\*******************************************************************   ! OAS
\***    Display background message                                     ! OAS
\***................................................................   ! OAS

DISPLAY.MESSAGE:                                                       ! DCS
                                                                       ! DCS
     IF COMM.MODE.FLAG$ = "B" THEN BEGIN                               ! DCS
         ADX.INTEGER%  = 0                                             ! DCS
         ADX.FUNCTION% = 26                                            ! DCS
         CALL ADXSERVE (ADX.RETURN.CODE%,                              \ DCS
                        ADX.FUNCTION%,                                 \ DCS
                        ADX.INTEGER%,                                  \ DCS
                        ADXSERVE.DATA$)                                ! DCS
     ENDIF ELSE BEGIN                                                  ! DCS
        PRINT ADXSERVE.DATA$                                           ! DCS
     ENDIF                                                             ! DCS
RETURN                                                                 ! DCS

\*****************************************************************************
\***
\***    TERMINATION
\***
\***..........................................................................


TERMINATION:

    ADXSERVE.DATA$ = "TERMINATION"                                     ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    GOSUB DEALLOCATE.SESSION.NUMBERS

RETURN



\*****************************************************************************
\***
\***    TERMINATION SPECIFIC ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    DEALLOCATE.SESSION.NUMBERS
\***    Perform CALL.F20.SESS.NUM.UTILITY to de-allocate file session numbers
\***    from all files referenced by the program.
\***
\***..........................................................................


DEALLOCATE.SESSION.NUMBERS:


    ADXSERVE.DATA$ = "DEALLOCATE.SESSION.NUMBERS"                      ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    FUNCTION.FLAG$ EQ "C"

    PASSED.INTEGER% EQ RPRPT.SESS.NUM%
    PASSED.STRING$ EQ ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    PASSED.INTEGER% = PRINT.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    PASSED.INTEGER% = SOFTS.SESS.NUM%                                  ! CNS
    PASSED.STRING$ = ""                                                ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS

    PASSED.INTEGER% = RFSCF.SESS.NUM%                                  ! CNS
    PASSED.STRING$ = ""                                                ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS

    PASSED.INTEGER% = CLOLF.SESS.NUM%                                  ! CNS
    PASSED.STRING$ = ""                                                ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS

    PASSED.INTEGER% = CLILF.SESS.NUM%                                  ! CNS
    PASSED.STRING$ = ""                                                ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS

    !PASSED.INTEGER% = ISF.SESS.NUM%                                   ! CNS ! OAS
    !PASSED.STRING$ = ""                                               ! CNS ! OAS
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                   ! CNS ! OAS

    !PASSED.INTEGER% = IRF.SESS.NUM%                                   ! CNS ! OAS
    !PASSED.STRING$ = ""                                               ! CNS ! OAS
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                   ! CNS ! OAS

    PASSED.INTEGER% = IDF.SESS.NUM%                                    ! CNS
    PASSED.STRING$ = ""                                                ! CNS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! CNS

    PASSED.INTEGER% = MINLS.SESS.NUM%                                  ! LJT
    PASSED.STRING$ = ""                                                ! LJT
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! LJT

    PASSED.INTEGER% = SRITL.SESS.NUM%                                  ! OAS
    PASSED.STRING$ = ""                                                ! OAS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! OAS

    PASSED.INTEGER% = MINOK.SESS.NUM%                                  ! OAS
    PASSED.STRING$ = ""                                                ! OAS
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! OAS

    ADXSERVE.DATA$ = "Session numbers deallocated"                     ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

RETURN


\*****************************************************************************
\***
\***    CHECK.FUNCTION.RETURN.CODE:
\***    If FUNCTION.RETURN.CODE% is zero ...
\***        By-passes rest of procedure.
\***    Displays error message on background screen.
\***    Stops program.
\***
\***..........................................................................


CHECK.FUNCTION.RETURN.CODE:

    IF FUNCTION.RETURN.CODE% EQ 0 THEN \
        BEGIN
        RETURN
        ENDIF

    GOSUB STOP.PROGRAM

RETURN


\*****************************************************************************
\***
\***    FORMAT.CURRENT.CODE:
\***    Sets CURRENT.CODE.LOGGED$ for use with application event log.
\***
\***..........................................................................


FORMAT.CURRENT.CODE:

    IF FILE.OPERATION$ EQ "C"                            \ ! Create
      OR FILE.OPERATION$ EQ "O" THEN                     \ ! Open
    BEGIN
        CURRENT.CODE.LOGGED$ EQ PACK$("00000000000000")
    ENDIF

    IF FILE.OPERATION$ EQ "R"                            \ ! Read
      OR FILE.OPERATION$ EQ "W" THEN                     \ ! Write
    BEGIN
        CURRENT.CODE.LOGGED$ EQ RIGHT$(CURRENT.CODE$,7)
    ENDIF

RETURN



\*****************************************************************************
\***
\***    PROGRAM INDEPENDENT ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    CALL.F01.APPLICATION.LOG:
\***    References APPLICATION.LOG (F01) to write details of event defined
\***    by EVENT.NUMBER% and VAR.STRING.1$ to Application Event Log, and to
\***    display any message defined by MESSAGE.NUMBER% and VAR.STRING.2$.
\***
\***..........................................................................


CALL.F01.APPLICATION.LOG:


    FUNCTION.RETURN.CODE% EQ \
      APPLICATION.LOG \
       (MESSAGE.NUMBER%, \
        VAR.STRING.1$, \
        VAR.STRING.2$, \
        EVENT.NUMBER%)

    GOSUB CHECK.FUNCTION.RETURN.CODE

RETURN


\*****************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY:
\***    References SESS.NUM.UTILITY (F20) to create, read, or delete entry on
\***    Session Number Table as determined by FUNCTION.FLAG$ ("O" "R" "C").
\***
\***..........................................................................


CALL.F20.SESS.NUM.UTILITY:


    FUNCTION.RETURN.CODE% EQ \
      SESS.NUM.UTILITY \
       (FUNCTION.FLAG$, \
        PASSED.INTEGER%, \
        PASSED.STRING$)

    GOSUB CHECK.FUNCTION.RETURN.CODE

    IF FUNCTION.FLAG$ = "R" THEN CURRENT.REPORT.NUM% = F20.INTEGER.FILE.NO% !HBG

RETURN


\*****************************************************************************
\***
\***    LOG.AN.EVENT.106:
\***    Writes details of Event 106 to application event log and displays
\***    message B501 (for file open errors) or B514 (for other errors).
\***
\***..........................................................................


LOG.AN.EVENT.106:


    ADXSERVE.DATA$ = "LOG.AN.EVENT.106 File Op= " + FILE.OPERATION$    ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    EVENT.NUMBER% EQ 106

    GOSUB FORMAT.CURRENT.CODE

    !OBTAIN REPORT NUMBER                                              ! HBG
    FUNCTION.FLAG$ = "R"                                               ! HBG
    PASSED.INTEGER% = CURRENT.SESS.NUM%                                ! HBG
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! HBG

    VAR.STRING.1$ EQ                         \ ! Application event log data
        FILE.OPERATION$                    + \
        CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) + \ ! Two byte integer byte order
        CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) + \ ! reversed to give hex number
        CURRENT.CODE.LOGGED$

    GOSUB CALL.F01.APPLICATION.LOG

RETURN



\*****************************************************************************
\***
\***    IF END # AND ERROR.DETECTED ROUTINES
\***
\***..........................................................................


\******************************************************************************
\***
\***   CREATE.ERROR:
\***
\***   LOG an event 106
\***
\***   GOTO PROGRAM.EXIT
\***
\******************************************************************************

CREATE.ERROR:

       GOSUB LOG.AN.EVENT.106

       GOTO STOP.PROGRAM

\*****************************************************************************
\***
\***    FILE.ERROR.EXIT:
\***    Logs events for specific file errors.
\***    Formats error message and displays on background screen.
\***    Logs an event 106.
\***    Stops program.
\***
\***..........................................................................


FILE.ERROR.EXIT:


    GOSUB FORMAT.CURRENT.CODE
    GOSUB LOG.AN.EVENT.106
    GOTO  STOP.PROGRAM

\*****************************************************************************
\***
\***    ERROR.DETECTED:
\***    Increments ERROR.COUNT% by one and tests it against values greater
\***    than one before any other commands executed.
\***    Further errors within ERROR.DETECTED causing control to be passed here
\***    again result in this test being failed and the immediate diversion of
\***    program control to STOP.PROGRAM.
\***    If no chaining parameters passed ...
\***        Diverts program control to OBTAIN.CHAIN.PARAMETERS.FROM.COMMAND
\***    References STANDARD.ERROR.DETECTED to log Event 101 and display
\***    message B550.
\***
\***..........................................................................


ERROR.DETECTED:

    ADXSERVE.DATA$ = "ERROR.DETECTED"                                  ! DCS
    GOSUB DISPLAY.MESSAGE                                              ! DCS

    ERROR.COUNT% EQ ERROR.COUNT% + 1

    IF ERROR.COUNT% GT 1 THEN \
        BEGIN
        RESUME STOP.PROGRAM
        ENDIF

    IF FILE.OPERATION$ = "W" AND \                                     ! OAS
       CURRENT.SESS.NUM% = MINOK.SESS.NUM% THEN BEGIN                  ! OAS

        ADXSERVE.DATA$ = "Error while writing to MINOK File"           ! OAS
        GOSUB DISPLAY.MESSAGE                                          ! OAS
        ERROR.COUNT% = 0                                               ! OAS
        RESUME MINOK.ERROR                                             ! OAS

    ENDIF                                                              ! OAS

    !Setting MINLS HOUSEKEEPING FLAG to FALSE when there is an error   ! OAS
    !while deleting the record from the file                           ! OAS

    IF FILE.OPERATION$ = "D" THEN BEGIN                                ! OAS

        ADXSERVE.DATA$ = "Error while deleting MINLS record"           ! OAS
        GOSUB DISPLAY.MESSAGE                                          ! OAS
        MINLS.HK.STATUS$ = "X"                                         ! OAS
        ERROR.COUNT% = 0                                               ! OAS
        RESUME                                                         ! OAS

    ENDIF                                                              ! OAS


    IF ERR = "OE" THEN BEGIN                                           ! CNS
       IF CURRENT.SESS.NUM% = RFSCF.SESS.NUM% THEN BEGIN               ! CNS
          ERROR.COUNT% = 0                                             ! CNS
          DO.MAIN = 1                                                  ! CNS
          RESUME END.RF.PROCESS                                        ! CNS
       ENDIF                                                           ! CNS
    ENDIF                                                              ! CNS

    IF ERR = "KF" AND CURRENT.SESS.NUM% = SRITL.SESS.NUM%             \! OAS
      THEN BEGIN                                                       ! OAS
      ERROR.COUNT% = 0                                                 ! OAS
      RESUME                                                           ! OAS
    ENDIF                                                              ! OAS

    IF ERR = "CU" AND CURRENT.SESS.NUM% = MINOK.SESS.NUM%             \! RTT
      THEN BEGIN                                                       ! RTT
      ERROR.COUNT% = 0                                                 ! RTT
      RESUME                                                           ! RTT
    ENDIF                                                              ! RTT

    FUNCTION.RETURN.CODE% EQ  \ OAS
      STANDARD.ERROR.DETECTED \
       (ERRN,                 \ OAS
        ERRF%,                \ OAS
        ERRL,                 \ OAS
        ERR)

RESUME STOP.PROGRAM


\*****************************************************************************
\*****************************************************************************
\***
\***    End of program MINPRINT
\***
\*****************************************************************************
\*****************************************************************************




