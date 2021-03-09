\*****************************************************************************
\*****************************************************************************
\***
\***    PROGRAM  .....  MINPRINT
\***    MODULE  ......  MINRFCNT.BAS
\***
\***    REVISION 1.0.           Arun Sudhakarannair             14th June 2012
\***    Original version.
\***
\***    VERSION B (1.1)         Tittoo Thomas                   04th Sept 2012
\***    Fixed to specify the date format in RFCNTLST header to DD/MM/YY.
\***    (SFA defect ID 600)
\***
\***    VERSION C (1.2)         Charles Skadorwa (CCSk)         10th Sept 2012
\***    SFA Defect 661 - Summary counts are not updated.
\***    Corrected Summary headings: "User Generated Lists" and
\***         "Support Office Lists" swapped around (in 2 places).
\***    Corrected time separator from "." to ":".
\***    Commented out redundant code (as set by file function) and
\***         moved a static variable outside of a loop.
\***
\***    VERSION D (1.3)            Tittoo Thomas (DTT)          24th Sept 2012
\***    SFA Defect 693, 694 - Operator name and list creation time not correct
\***    Corrected Summary & Detail headings: "User Generated Lists" and
\***         "Support Office Lists" swapped around (in 2 places).
\***
\***    VERSION E (1.4)            Tittoo Thomas (ETT)          02nd Nov 2012
\***    SFA Defect 779 - Excluded lists with Zero items or currently "In Creation"
\***    status.
\***
\***    Version F              Ranjith Gopalankutty(FRG)     10th Feb  2017
\***    After 16A rollout MINPRINT is triggered after midnight as part of  
\***    end of the day reset, there is a date check happens in MINRFCNT    
\***    module before adding the records to RFCNTLST.DAY. Since the date   
\***    match doesn't happen, records are being ignored and count list     
\***    are not appearing in controller screen. Fix is to ensure the date  
\***    parameter check is correct and record is added if the run is after 
\***    mid night.                                                         
\*****************************************************************************
\***                                  OVERVIEW
\***                                  ========
\***    MINRFCNT takes data from CLILF and CLOLF and prepares RF Count report
\***    (RFCNTLST.DAY). This report can be viewed both on the controller and
\***    RF PPC/MC55 and POD MC55/MC70.
\***
\***    The report is split into 2 sections. The first part is a summary for
\***    each count list type (Negative, User Generated and Support Office).
\***    The second part gives details of which users performed the counts,
\***    and whether the list was fully counted, part counted or not counted
\***    at all.
\***
\*****************************************************************************
\*****************************************************************************

\*****************************************************************************
\***
\***    DEC included code defining file related fields
\***
\***..........................................................................

    %INCLUDE   AFDEC.J86        ! GSA Authorisation File field declaration
    %INCLUDE   CLILFDEC.J86     ! RF Count Lists File
    %INCLUDE   CLOLFDEC.J86     ! RF Count List Of Lists File

\*****************************************************************************
\***
\***    Included code defining function related global variables
\***
\***..........................................................................

    %INCLUDE PSBF01G.J86        ! APPLICATION.LOG
    %INCLUDE PSBF02G.J86        ! UPDATE date variable                  ! FRG
    %INCLUDE PSBF14G.J86        ! SORT.TABLE globals
    %INCLUDE PSBF20G.J86        ! Allocating Session Numbers            ! FRG


\*****************************************************************************
\***
\***    Global variable definitions
\***
\***..........................................................................

    STRING GLOBAL               \
        COMM.MODE.FLAG$,        \
        CURRENT.CODE$,          \
        FILE.OPERATION$,        \
        REPORTING.STATUS$

    INTEGER*2 GLOBAL            \
        CURRENT.REPORT.NUM%,    \
        RFCNTLST.SESS.NUM%

    INTEGER*1 GLOBAL            \
        FALSE,                  \
        TRUE

\*****************************************************************************
\***
\***    Variable definitions
\***
\***..........................................................................

    STRING                      \
        ADXSERVE.DATA$,         \
        CURR.LIST.TYPE$,        \
        CURRENT.TYPE$,          \
        CURR.CNT.STATUS$,       \
        CURR.RECORD$,           \
        CURRENT.CODE.LOGGED$,   \
        FUNCTION.FLAG$,         \
        PASSED.STRING$,         \
        PREV.TYPE$,             \
        PREV.CNT.STATUS$,       \
        RFCNTLST.FILE.NAME$,    \
        RFCNTLST.RECORD$,       \
        VAR.STRING.1$,          \
        VAR.STRING.2$

    INTEGER*1                   \
        COUNTER%,               \
        ERROR.COUNT%

    INTEGER*2                   \
        ADX.FUNCTION%,          \
        ADX.INTEGER%,           \
        CLOLF.NUMRECS%,         \
        CLILF.COUNTER%,         \
        CURR.RECORD.INDEX%,     \
        EVENT.NUMBER%,          \
        F14.LIMIT%,             \
        F14TABLE.MAX.INDEX%,    \
        FUNCTION.RETURN.CODE%,  \
        ITEMS.NOT.COUNTED%,     \
        LISTS.COUNTED.ARRAY%(1),\
        MESSAGE.NUMBER%,        \
        NUM.LISTS.ARRAY%(1),    \
        PART.COUNT.ARRAY%(1),   \
        PASSED.INTEGER%,        \
        RC%,                    \
        RFCNTLST.REPORT.NUM%,   \
        UNCOUNTED.ARRAY%(1)

    INTEGER*4                   \
        ADX.RETURN.CODE%

\*****************************************************************************
\***
\***    EXT included code defining file related external functions
\***
\***..........................................................................

    %INCLUDE AFEXT.J86          ! GSA Authorisation File function definition
    %INCLUDE CLOLFEXT.J86       ! RF Count List Of Lists File
    %INCLUDE CLILFEXT.J86       ! RF Count Lists File

\*****************************************************************************
\***
\***    Included code defining external Boots functions
\***
\***..........................................................................

    %INCLUDE ADXSERVE.J86       ! Message Logging
    %INCLUDE PSBF01E.J86        ! APPLICATION.LOG
    %INCLUDE PSBF02E.J86        ! Update Date External Function         ! FRG
    %INCLUDE PSBF14E.J86        ! SORT.TABLE function definition
    %INCLUDE PSBF20E.J86        ! Allocating Session Numbers
    %INCLUDE PSBF24E.J86        ! STANDARD.ERROR.DETECTED

\*****************************************************************************
\***
\***    WRITE RFCNTLST
\***    Detail      : This function writes data to the report file(RFCNTLST.DAY)
\***                  The data which has to be written to report, is
\***                  stored in the variable RFCNTLST.RECORD$ which inturn is
\***                  written to the report file.
\***                  This section also adds prefixes like "0M", "1M", "2M",
\***                  "3M" or "4M" based on the heading types, which helps in
\***                  formatting the report in a user friendly way in POD/PDT
\***
\***..........................................................................

FUNCTION WRITE.RFCNTLST (TYPE$)

    STRING TYPE$

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = RFCNTLST.REPORT.NUM%
    CURRENT.TYPE$ = TYPE$

    !If CURRENT.TYPE$ is "0M", "1M", "2M", "3M" or "4M" then
    !    line starts with that corresponding letter
    !If CURRENT.TYPE$ is "" then line starts with
    !    previous type + D
    !Eg: If PREV.TYPE$ = "1M" and CURRENT.TYPE$ = "" then
    !    line starts with (1+1)D ie 2D

    IF CURRENT.TYPE$ = "0M" THEN BEGIN
        RFCNTLST.RECORD$ = "0M" + RFCNTLST.RECORD$
        PREV.TYPE$ = CURRENT.TYPE$
    ENDIF ELSE IF CURRENT.TYPE$ = "1M" THEN BEGIN
        RFCNTLST.RECORD$ = "1M" + RFCNTLST.RECORD$
        PREV.TYPE$ = CURRENT.TYPE$
    ENDIF ELSE IF CURRENT.TYPE$ = "2M" THEN BEGIN
        RFCNTLST.RECORD$ = "2M" + RFCNTLST.RECORD$
        PREV.TYPE$ = CURRENT.TYPE$
    ENDIF ELSE IF CURRENT.TYPE$ = "3M" THEN BEGIN
        RFCNTLST.RECORD$ = "3M" + RFCNTLST.RECORD$
        PREV.TYPE$ = CURRENT.TYPE$
    ENDIF ELSE IF CURRENT.TYPE$ = "4M" THEN BEGIN
        RFCNTLST.RECORD$ = "4M" + RFCNTLST.RECORD$
        PREV.TYPE$ = CURRENT.TYPE$
    ENDIF ELSE IF CURRENT.TYPE$ = "" THEN BEGIN
        RFCNTLST.RECORD$ =  STR$(VAL(LEFT$(PREV.TYPE$,1)) + 1 ) \
                            + "D" + RFCNTLST.RECORD$
    ENDIF

    WRITE # RFCNTLST.SESS.NUM% ;RFCNTLST.RECORD$

END FUNCTION

\******************************************************************************
\***
\***   Sub-Program : GET.OP.NAME
\***   Detail      : This function key read the EALAUTH and returns the
\***                 Operator Name corresponding to the Operator ID. IF an
\***                 Op Name is not obtained, the "**No Op name**      "
\***                 is returned
\******************************************************************************

FUNCTION GET.OP.NAME$

    STRING GET.OP.NAME$
    GET.OP.NAME$ = "**No Op name**      "

    IF UNPACK$(AF.OPERATOR.NO$) <> "00000000" THEN BEGIN
        RC% = READ.AF
        IF RC% = 0 AND AF.OPERATOR.NAME$ <> "" THEN BEGIN
            !Op Name padded with space upto 20 Chars
            GET.OP.NAME$ = LEFT$(AF.OPERATOR.NAME$ +(STRING$(20," ")), 20)
        ENDIF
    ENDIF

END FUNCTION

\******************************************************************************
\******************************************************************************
\***
\***        S T A R T    O F    SUB PROGRAM
\***
\******************************************************************************
\******************************************************************************

SUB MINRFCNT PUBLIC

    ON ERROR GOTO ERROR.DETECTED:

    ADXSERVE.DATA$ = "*** Creating RFCNTLST.DAY report ***"
    GOSUB DISPLAY.MESSAGE

    GOSUB INITIALISATION
    GOSUB CREATE.RF.COUNT.DETAIL
    GOSUB CREATE.RF.COUNT.SUMMARY
    GOSUB CREATE.RF.COUNT.REPORT

STOP.MINRFCNT:

    CLOSE CLOLF.SESS.NUM%
    CLOSE CLILF.SESS.NUM%
    CLOSE AF.SESS.NUM%
    CLOSE RFCNTLST.SESS.NUM%

    GOSUB DEALLOCATE.SESSION.NUMBERS

    IF ERROR.COUNT% > 0 THEN BEGIN
        ADXSERVE.DATA$ = "*** RFCNTLST.DAY report processing error ***"
        GOSUB DISPLAY.MESSAGE
    ENDIF ELSE BEGIN
        ADXSERVE.DATA$ = "*** RFCNTLST.DAY report creation success ***"
        GOSUB DISPLAY.MESSAGE
        REPORTING.STATUS$ = "E"
    ENDIF

    EXIT SUB

\*****************************************************************************
\***
\***    INITIALISATION
\***    Detail        : Below mentioned are the main initialization done here.
\***                  1. Setting RFCNTLST.FILE.NAME$ and RFCNTLST.REPORT.NUM%
\***                  2. Initialise the F14.TABLE$ array
\***                  3. Initialise NUM.LISTS.ARRAY%, LISTS.COUNTED.ARRAY%,
\***                     PART.COUNT.ARRAY%, UNCOUNTED.ARRAY% array
\***                  4. Allocating Session Numbers
\***                  6. Opening Files
\***                  7. Creates the RFCNTLST file
\***
\***..........................................................................

INITIALISATION:

    ADXSERVE.DATA$ = "Initialisation - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

    RFCNTLST.FILE.NAME$  = "ADXLXACN::D:/ADX_UDT1/RFCNTLST.DAY"
    RFCNTLST.REPORT.NUM% = 426
    FALSE = 0
    TRUE  = -1
    REPORTING.STATUS$ = "X"

    !Initializing the F14.TABLE$, NUM.LISTS.ARRAY%, LISTS.COUNTED.ARRAY%,
    !PART.COUNT.ARRAY%, UNCOUNTED.ARRAY% arrays
    F14.LIMIT% = 1000
    DIM F14.TABLE$(F14.LIMIT%)
    DIM NUM.LISTS.ARRAY%(1000)
    DIM LISTS.COUNTED.ARRAY%(1000)
    DIM PART.COUNT.ARRAY%(1000)
    DIM UNCOUNTED.ARRAY%(1000)

    GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB OPEN.FILES

RETURN

\*****************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS
\***    Detail        : Perform CALL.F20.SESS.NUM.UTILITY to allocate
\***                    file session numbers for all files referenced
\***                    by the program.
\***
\***..........................................................................

ALLOCATE.SESSION.NUMBERS:

    ADXSERVE.DATA$ = "Allocating Session numbers - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

    CALL CLOLF.SET
    CALL CLILF.SET
    CALL AF.SET

    FUNCTION.FLAG$ EQ "O"

    PASSED.INTEGER% EQ CLOLF.REPORT.NUM%
    PASSED.STRING$ EQ CLOLF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    CLOLF.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    PASSED.INTEGER% EQ CLILF.REPORT.NUM%
    PASSED.STRING$ EQ CLILF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    CLILF.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    PASSED.INTEGER% EQ AF.REPORT.NUM%
    PASSED.STRING$ EQ AF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    AF.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    PASSED.INTEGER% EQ RFCNTLST.REPORT.NUM%
    PASSED.STRING$ EQ RFCNTLST.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    RFCNTLST.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    ADXSERVE.DATA$ = "Session numbers allocating success - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

RETURN

\*****************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY:
\***    Detail        : References SESS.NUM.UTILITY (F20) to create,
\***                    read, or delete entry on Session Number Table as
\***                    determined by FUNCTION.FLAG$ ("O" "R" "C").
\***
\***..........................................................................

CALL.F20.SESS.NUM.UTILITY:

    FUNCTION.RETURN.CODE% EQ    \
        SESS.NUM.UTILITY        \
        (FUNCTION.FLAG$,        \
        PASSED.INTEGER%,        \
        PASSED.STRING$)

RETURN

\*****************************************************************************
\***
\***    OPEN.FILES:
\***    Detail          :Opens CLOLF, CLILF and AF Files.
\***
\***..........................................................................

OPEN.FILES:

    FILE.OPERATION$     = "O"

    CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM%
    OPEN CLOLF.FILE.NAME$ DIRECT RECL CLOLF.RECL% AS CLOLF.SESS.NUM%

    CURRENT.REPORT.NUM% = CLILF.REPORT.NUM%
    OPEN CLILF.FILE.NAME$ KEYED RECL CLILF.RECL% AS CLILF.SESS.NUM%

    CURRENT.REPORT.NUM% = AF.REPORT.NUM%
    OPEN AF.FILE.NAME$ KEYED RECL AF.RECL%                          \
         AS AF.SESS.NUM% NOWRITE NODEL

    CURRENT.REPORT.NUM% = RFCNTLST.REPORT.NUM%
    OPEN RFCNTLST.FILE.NAME$ AS RFCNTLST.SESS.NUM% APPEND

    ADXSERVE.DATA$ = "Opened CLILF/CLOLF Backup Files - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

RETURN

\*****************************************************************************
\***
\***    CREATE.RF.COUNT.DETAIL:
\***    Detail        : Below mentioned are the main process done here.
\***                    1. Calculates number of CLOLF records
\***                    2. Reads each CLOLF record and write the necessary
\***                       data to F14.TABLE
\***                    3. Reads CLILF records corresponding to CLOLF record
\***                       and gets the details of items not counted in list
\***                    4. Sort F14.TABLE
\***
\***..........................................................................

CREATE.RF.COUNT.DETAIL:

    ADXSERVE.DATA$ = "Processing CLOLF.BIN File - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

    !Reading CLOLF and getting the number of records
    CLOLF.NUMRECS% = (SIZE(CLOLF.FILE.NAME$) / CLOLF.RECL%)
    CURR.RECORD.INDEX% = 1

    FOR CLOLF.RECORD.NUM% = 1 TO CLOLF.NUMRECS%

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM%
        RC% = READ.CLOLF

        IF CLOLF.LSTTYP$ = "N" OR CLOLF.LSTTYP$ = "H" OR \
           CLOLF.LSTTYP$ = "U" THEN BEGIN

            !Setting CURR.LIST.TYPE$ for sorting purpose
!            IF CLOLF.LSTTYP$ = "N" THEN BEGIN                        ! CCsk
!                CURR.LIST.TYPE$ = "1"                                ! CCsk
!            ENDIF ELSE IF CLOLF.LSTTYP$ = "H" THEN BEGIN             ! CCsk
!                CURR.LIST.TYPE$ = "2"                                ! CCsk
!            ENDIF ELSE IF CLOLF.LSTTYP$ = "U" THEN BEGIN             ! CCsk
!                CURR.LIST.TYPE$ = "3"                                ! CCsk
!            ENDIF                                                    ! CCsk
            IF CLOLF.LSTTYP$ = "N" THEN BEGIN
                CURR.LIST.TYPE$ = "1"
            ENDIF ELSE IF CLOLF.LSTTYP$ = "U" THEN BEGIN              ! CCsk
                CURR.LIST.TYPE$ = "2"
            ENDIF ELSE IF CLOLF.LSTTYP$ = "H" THEN BEGIN              ! CCsk
                CURR.LIST.TYPE$ = "3"
            ENDIF

            !Setting CURR.CNT.STATUS$ for sorting & filtering purpose
            IF CLOLF.ACTIVE.STATUS$ = "C" THEN BEGIN
                CURR.CNT.STATUS$ = "1"
            ENDIF ELSE IF CLOLF.ACTIVE.STATUS$ = "P" THEN BEGIN
                CURR.CNT.STATUS$ = "2"
            ENDIF ELSE IF CLOLF.ACTIVE.STATUS$ = "I" THEN BEGIN
                CURR.CNT.STATUS$ = "3"
            ENDIF ELSE IF CLOLF.ACTIVE.STATUS$ = " " THEN BEGIN       ! ETT
                CURR.CNT.STATUS$ = "4"                                ! ETT
            ENDIF

            ITEMS.NOT.COUNTED% = 0
            ADXSERVE.DATA$ = "Processing CLILF for CLOLF record " \
                             + STR$(CLOLF.RECORD.NUM%) + " - MINRFCNT"
            GOSUB DISPLAY.MESSAGE

            !Reading CLILF and getting the count of items with
            !CLILF.COUNTED.STATUS$ = "P" OR CLILF.COUNTED.STATUS$ = "U"
            !(Items not counted)

            CLILF.LISTID$  = CLOLF.LISTID$                                 ! CCSk

            FOR CLILF.COUNTER% = 1 TO CLOLF.TOTAL.ITEMS%

                !CLILF.LISTID$  = CLOLF.LISTID$                            ! CCsk
                CLILF.ITEMSEQ$ = RIGHT$("000" + STR$(CLILF.COUNTER%),3)
                !FILE.OPERATION$     = "R"                                 ! CCSk
                !CURRENT.REPORT.NUM% = CLILF.REPORT.NUM%                   ! CCSk
                RC% = READ.CLILF

                IF CLILF.COUNTED.STATUS$ = "P" OR \
                    CLILF.COUNTED.STATUS$ = "U" THEN BEGIN
                    ITEMS.NOT.COUNTED% = ITEMS.NOT.COUNTED% + 1
                ENDIF

            NEXT CLILF.COUNTER%

            IF CLOLF.TOTAL.ITEMS% > 0 AND CURR.CNT.STATUS$ <> "4" THEN BEGIN   ! ETT
                CURR.RECORD$ =  CURR.LIST.TYPE$                             + \
                                CURR.CNT.STATUS$                            + \
                                UNPACK$(CLOLF.CREATION.DATE$)               + \
                                UNPACK$(CLOLF.CREATION.TIME$)               + \
                                CLOLF.PICKER.USER.ID$                       + \
                                CLOLF.LISTID$                               + \
                                RIGHT$("000" + STR$(CLOLF.TOTAL.ITEMS%), 3) + \
                                UNPACK$(CLOLF.PICK.START.TIME$)             + \
                                UNPACK$(CLOLF.PICK.END.TIME$)               + \
                                CLOLF.USERID$ + \
                                RIGHT$("000" + STR$(ITEMS.NOT.COUNTED%), 3)

                F14.TABLE$(CURR.RECORD.INDEX%) = CURR.RECORD$
                CURR.RECORD.INDEX% = CURR.RECORD.INDEX% + 1
            ENDIF                                                              ! ETT

            IF CURR.RECORD.INDEX% = F14.LIMIT% THEN BEGIN

                !The F14.TABLE$ array has reached its max limit, hence stop
                !processing further CLOLF/CLILF and continue the report
                !generation

                ADXSERVE.DATA$ = "F14.TABLE$ array limit exceeded, the program "
                GOSUB DISPLAY.MESSAGE
                ADXSERVE.DATA$ = "will not generate the complete report "
                GOSUB DISPLAY.MESSAGE

                !Set the variables to exit both the FOR loop
                CLOLF.RECORD.NUM% = CLOLF.NUMRECS%

            ENDIF
        ENDIF
    NEXT CLOLF.RECORD.NUM%

    !Total no: of records in F14.TABLE$
    F14TABLE.MAX.INDEX% = CURR.RECORD.INDEX% - 1

!       F14.TABLE LAYOUT
!
!    |---------|-------|-----------|------------------------------------|
!    |   From  |  To   |   Length  |    Name                            |
!    |---------|-------|-----------|------------------------------------|
!    |   1     |  1    |   1       |    CURR.LIST.TYPE$                 |
!    |   2     |  2    |   1       |    CURR.CNT.STATUS$                |
!    |   3     |  8    |   6       |    UNPACK$(CLOLF.CREATION.DATE$)   |
!    |   9     |  12   |   4       |    UNPACK$(CLOLF.CREATION.TIME$)   |
!    |   13    |  15   |   3       |    CLOLF.PICKER.USER.ID$           |
!    |   16    |  18   |   3       |    CLOLF.LISTID$                   |
!    |   19    |  21   |   3       |    STR$(CLOLF.TOTAL.ITEMS%)        |
!    |   22    |  25   |   4       |    UNPACK$(CLOLF.PICK.START.TIME$) |
!    |   26    |  29   |   4       |    UNPACK$(CLOLF.PICK.END.TIME$)   |
!    |   30    |  32   |   3       |    CLOLF.USERID$                   |
!    |   33    |  35   |   3       |    STR$(ITEMS.NOT.COUNTED%)        |
!    |---------|-------|-----------|------------------------------------|

    !Sorting F14.TABLE$
    CALL SORT.TABLE (F14TABLE.MAX.INDEX%)

RETURN

\*****************************************************************************
\***
\***    CREATE.RF.COUNT.SUMMARY:
\***    Detail        : Below mentioned are the main process done here.
\***                    1. Initialise the arrays NUM.LISTS.ARRAY%,
\***                       LISTS.COUNTED.ARRAY%, PART.COUNT.ARRAY%,
\***                       UNCOUNTED.ARRAY%
\***                    2. Writes the date and time in the report
\***                    3. Processes F14.TABLE and checks the value of
\***                       CURR.CNT.STATUS$ and setting the arrays
\***                       NUM.LISTS.ARRAY%, LISTS.COUNTED.ARRAY%,
\***                       PART.COUNT.ARRAY%, UNCOUNTED.ARRAY%
\***                    4. Writes the details to the report
\***
\***..........................................................................

CREATE.RF.COUNT.SUMMARY:

    ADXSERVE.DATA$ = "Processing RF.COUNT.SUMMARY - MINRFCNT "
    GOSUB DISPLAY.MESSAGE

    !Initialise the arrays NUM.LISTS.ARRAY%, LISTS.COUNTED.ARRAY%
    !PART.COUNT.ARRAY%,UNCOUNTED.ARRAY%

    FOR COUNTER% = 1 TO 3
        NUM.LISTS.ARRAY%(COUNTER%) = 0
        LISTS.COUNTED.ARRAY%(COUNTER%) = 0
        PART.COUNT.ARRAY%(COUNTER%) = 0
        UNCOUNTED.ARRAY%(COUNTER%) = 0
    NEXT COUNTER%

    !Writes the date and time in the report
!    RFCNTLST.RECORD$  = RIGHT$(DATE$, 2)    + "/"  + \               !BTT
!                        MID$(DATE$,3,2)     + "/"  + \               !BTT
!                        "20" + LEFT$(DATE$, 2)     + \               !BTT
!                        " at " + MID$(TIME$,1,2)   + \               !BTT
!                        ":" + MID$(TIME$,3,2) + " "                  !BTT
    RFCNTLST.RECORD$  = " " + RIGHT$(DATE$, 2) + "/"    + \           !BTT
                              MID$(DATE$,3,2)  + "/"    + \           !BTT
                              LEFT$(DATE$, 2)  + " at " + \           !BTT
                              MID$(TIME$,1,2)  + ":"    + \           !BTT
                              MID$(TIME$,3,2)  + "  "                 !BTT
    CALL WRITE.RFCNTLST("0M")

    RFCNTLST.RECORD$ = "Count List Summary  "
    CALL WRITE.RFCNTLST("1M")

    !Counter 1 to 3 is used for the three lists
    !Negative Lists,User Generated Lists,Support Office Lists

    FOR COUNTER% = 1 TO 3

        !Reading F14.TABLE one by one
        FOR CURR.RECORD.INDEX% = 1 to F14TABLE.MAX.INDEX%

            !Checking each record corresponds to the list specified
            !in COUNTER%
            !Checking whether the list is created today or not
            
            ! Commented the line of code which checks the CLOLF creation    ! FRG
            ! date against DATE$. After 16A MINPRINT gets triggered after   ! FRG
            ! 12.00 clock, by which condition check always fails .          ! FRG
            ! Amended the code so that, date$ is subtracted by one and      ! FRG
            ! Checked against CLOLF creation                                ! FRG

            F02.DATE$ = DATE$                                               ! FRG
            CALL UPDATE.DATE(-1)                                            ! FRG

            IF (LEFT$(F14.TABLE$(CURR.RECORD.INDEX%), 1) = STR$(COUNTER%))  \ FRG
                AND (UNPACK$(CLOLF.CREATION.DATE$) = F02.DATE$) THEN BEGIN  ! FRG

            !   AND (UNPACK$(CLOLF.CREATION.DATE$) = DATE$) THEN BEGIN      ! FRG  

                NUM.LISTS.ARRAY%(COUNTER%) = NUM.LISTS.ARRAY%(COUNTER%) + 1

                !Checking CURR.CNT.STATUS$ from F14.TABLE and getting the
                !values in the arrays LISTS.COUNTED.ARRAY%, PART.COUNT.ARRAY%
                !and UNCOUNTED.ARRAY%

                IF MID$(F14.TABLE$(CURR.RECORD.INDEX%),2,1) = "1" THEN BEGIN
                    LISTS.COUNTED.ARRAY%(COUNTER%) = LISTS.COUNTED.ARRAY%       \
                                                     (COUNTER%) + 1
                ENDIF ELSE IF MID$(F14.TABLE$(CURR.RECORD.INDEX%),2,1) = "2"    \
                              THEN BEGIN
                    PART.COUNT.ARRAY%(COUNTER%) = PART.COUNT.ARRAY%(COUNTER%)   \
                                                  + 1
                ENDIF ELSE IF MID$(F14.TABLE$(CURR.RECORD.INDEX%),2,1) = "3"    \
                              THEN BEGIN
                    UNCOUNTED.ARRAY%(COUNTER%) = UNCOUNTED.ARRAY%(COUNTER%) + 1
                ENDIF

            ENDIF
        NEXT CURR.RECORD.INDEX%

        RFCNTLST.RECORD$ = "                    "
        CALL WRITE.RFCNTLST ("")

        !Writing the data to the report
        IF COUNTER% = 1 THEN BEGIN
            RFCNTLST.RECORD$ = "Negative Lists      "
        ENDIF ELSE IF COUNTER% = 2 THEN BEGIN
            !RFCNTLST.RECORD$ = "User Generated Lists"                   ! CCSk
            !RFCNTLST.RECORD$ = "Support Office Lists"                   ! CCSk ! DTT
            RFCNTLST.RECORD$ = "User Generated Lists"                    ! DTT
        ENDIF ELSE IF COUNTER% = 3 THEN BEGIN
            !RFCNTLST.RECORD$ = "Support Office Lists"                   ! CCSk
            !RFCNTLST.RECORD$ = "User Generated Lists"                   ! CCSk ! DTT
            RFCNTLST.RECORD$ = "Support Office Lists"                    ! DTT
        ENDIF
        CALL WRITE.RFCNTLST ("")

        RFCNTLST.RECORD$ = "Num Lists Today "                       + \
                            RIGHT$( "   "                           + \
                            STR$(NUM.LISTS.ARRAY%(COUNTER%)),3)     + \
                            " "
        CALL WRITE.RFCNTLST ("")

        RFCNTLST.RECORD$ = "Lists counted   "                       + \
                            RIGHT$( "   "                           + \
                            STR$(LISTS.COUNTED.ARRAY%(COUNTER%)),3) + \
                            " "
        CALL WRITE.RFCNTLST ("")

        RFCNTLST.RECORD$ = "Part counted    "                       + \
                            RIGHT$( "   "                           + \
                            STR$(PART.COUNT.ARRAY%(COUNTER%)),3)    + \
                            " "
        CALL WRITE.RFCNTLST ("")

        RFCNTLST.RECORD$ = "Lists uncounted "                       + \
                            RIGHT$( "   "                           + \
                            STR$(UNCOUNTED.ARRAY%(COUNTER%)),3)     + \
                            " "
        CALL WRITE.RFCNTLST ("")

    NEXT COUNTER%

    RFCNTLST.RECORD$ = "--------------------"
    CALL WRITE.RFCNTLST ("")

RETURN

\*****************************************************************************
\***
\***    CREATE.RF.COUNT.REPORT:
\***    Detail        : Below mentioned are the main process done here.
\***                    1. If F14 TABLE has no records, write the blank records
\***                       Counted Lists, Part Counted Lists and Uncounted Lists
\***                    2. Process F14.TABLE and write the data to the report
\***
\***..........................................................................

CREATE.RF.COUNT.REPORT:

    ADXSERVE.DATA$ = "Processing RF.COUNT.REPORT - MINRFCNT "
    GOSUB DISPLAY.MESSAGE

    FOR COUNTER% = 1 TO 3

        IF COUNTER% = 1 THEN BEGIN
            RFCNTLST.RECORD$ = "Negative Lists      "
        ENDIF ELSE IF COUNTER% = 2 THEN BEGIN
            !RFCNTLST.RECORD$ = "User Generated Lists"                   ! CCSk
            !RFCNTLST.RECORD$ = "Support Office Lists"                   ! CCSk ! DTT
            RFCNTLST.RECORD$ = "User Generated Lists"                    ! DTT
        ENDIF ELSE IF COUNTER% = 3 THEN BEGIN
            !RFCNTLST.RECORD$ = "Support Office Lists"                   ! CCSk
            !RFCNTLST.RECORD$ = "User Generated Lists"                   ! CCSk ! DTT
            RFCNTLST.RECORD$ = "Support Office Lists"                    ! DTT
        ENDIF
        CALL WRITE.RFCNTLST ("1M")

        !If F14 TABLE has no records, write the blank records
        !Counted Lists, Part Counted Lists and Uncounted Lists

        IF F14TABLE.MAX.INDEX% = 1 THEN BEGIN
            RFCNTLST.RECORD$ = "Counted Lists       "
            CALL WRITE.RFCNTLST ("2M")
            RFCNTLST.RECORD$ = "Part Counted Lists  "
            CALL WRITE.RFCNTLST ("2M")
            RFCNTLST.RECORD$ = "Uncounted Lists     "
            CALL WRITE.RFCNTLST ("2M")
        ENDIF

        PREV.CNT.STATUS$ = " "

        !Reading F14.TABLE one by one
        FOR CURR.RECORD.INDEX% = 1 TO F14TABLE.MAX.INDEX%

            !Checking each record corresponds to the list specified in
            !COUNTER% and CLOLF creation date matches with current date.

           ! Commented the line of code which checks the CLOLF creation    ! FRG
           ! date against DATE$. After 16A, MINPRINT gets triggered after  ! FRG
           ! 12.00 clock, due to that below date check condition fails     ! FRG
           ! and doesn't write to the RFCNTLST.DAY file                    ! FRG


            F02.DATE$ = DATE$                                              ! FRG
            CALL UPDATE.DATE(-1)                                           ! FRG

            IF (LEFT$(F14.TABLE$(CURR.RECORD.INDEX%), 1) = STR$(COUNTER%)) \ FRG
                AND (MID$(F14.TABLE$(CURR.RECORD.INDEX%),3,6) = F02.DATE$) \ FRG
                THEN BEGIN                                                 ! FRG

               !AND (MID$(F14.TABLE$(CURR.RECORD.INDEX%),3,6) = DATE$)     \ FRG
                

                CURR.CNT.STATUS$ = MID$(F14.TABLE$(CURR.RECORD.INDEX%),2,1)

                !Getting CURR.CNT.STATUS$ from F14.TABLE
                IF CURR.CNT.STATUS$ = "1" THEN BEGIN

                    !Checks the Current Status to verify that
                    !    PREV.CNT.STATUS$ <> CURR.CNT.STATUS$
                    !If PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ then
                    !    print the header

                    IF PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ THEN BEGIN
                        RFCNTLST.RECORD$ = "Counted Lists       "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF

                    !If the list is User generated list, write the time
                    !and the user created the list in the report

                    IF COUNTER% = 2 THEN BEGIN
                        AF.OPERATOR.NO$ = PACK$(RIGHT$(("00000000" +            \
                                          MID$(F14.TABLE$(CURR.RECORD.INDEX%),  \
                                          30,3)),8))
                        CALL GET.OP.NAME$
                        RFCNTLST.RECORD$ = MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                           9,2) + ":" +                         \    ! CCSK
                                           MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                           11,2) + " " +                        \
                                           LEFT$(GET.OP.NAME$,14) ! OP NAME SET TO 14 CHARACTERS
                        CALL WRITE.RFCNTLST ("3M")
                    ENDIF

                    RFCNTLST.RECORD$ =  "List " +                               \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        16,3) + " " +                           \
                                        RIGHT$("   " + STR$(VAL(MID$(F14.TABLE$ \
                                        (CURR.RECORD.INDEX%),19,3))), 3) +      \
                                        " items  "
                    CALL WRITE.RFCNTLST ("")

                    RFCNTLST.RECORD$ =  "from " +                               \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        22,2) + ":" +                           \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        24,2) + " to " +                        \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        26,2) + ":" +                           \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        28,2) + " "
                    CALL WRITE.RFCNTLST ("")

                    !Set the key for reading the EALAUTH
                    AF.OPERATOR.NO$ = PACK$(RIGHT$(("00000000" + \
                                      MID$(F14.TABLE$(CURR.RECORD.INDEX%),      \
                                      13,3)),8))
                    CALL GET.OP.NAME$

                    RFCNTLST.RECORD$ =  "By " + LEFT$(GET.OP.NAME$,17)
                    CALL WRITE.RFCNTLST ("")
                    PREV.CNT.STATUS$ = CURR.CNT.STATUS$

                ENDIF ELSE IF CURR.CNT.STATUS$ = "2" THEN BEGIN

                    IF PREV.CNT.STATUS$ = " " THEN BEGIN
                        RFCNTLST.RECORD$ = "Counted Lists       "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF

                    !Checks the Current Status to verify that
                    !    PREV.CNT.STATUS$ <> CURR.CNT.STATUS$
                    !If PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ then
                    !    print the header

                    IF PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ THEN BEGIN
                        RFCNTLST.RECORD$ = "Part Counted Lists  "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF

                    !If the list is User generated list, write the time
                    !and the user created the list in the report

                    IF COUNTER% = 2 THEN BEGIN
                        AF.OPERATOR.NO$ = PACK$(RIGHT$(("00000000" +            \
                                          MID$(F14.TABLE$(CURR.RECORD.INDEX%),  \
                                          30,3)),8))
                        CALL GET.OP.NAME$
                        RFCNTLST.RECORD$ = MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                           9,2) + ":" +                         \      ! CCSK
                                           MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                           11,2) + " " +                        \
                                           LEFT$(GET.OP.NAME$,14) ! OP NAME SET TO 14 CHARACTERS
                        CALL WRITE.RFCNTLST ("3M")
                    ENDIF

                    RFCNTLST.RECORD$ =  "List " +                               \
                                        MID$(F14.TABLE$(CURR.RECORD.INDEX%),    \
                                        16,3) + " " +                           \
                                        RIGHT$("   "+ STR$(VAL(MID$(F14.TABLE$  \
                                        (CURR.RECORD.INDEX%),19,3))), 3) +      \
                                        " items  "
                    CALL WRITE.RFCNTLST ("")

                    RFCNTLST.RECORD$ =  "Uncounted " + RIGHT$("   " +           \
                                        STR$(VAL(MID$(F14.TABLE$                \
                                        (CURR.RECORD.INDEX%),33,3))), 3) +      \
                                        " items "
                    CALL WRITE.RFCNTLST ("")

                    RFCNTLST.RECORD$ =  "from " + MID$(F14.TABLE$               \
                                        (CURR.RECORD.INDEX%),22,2) +            \
                                        ":" + MID$(F14.TABLE$                   \
                                        (CURR.RECORD.INDEX%),24,2) +            \
                                        + " to " + MID$(F14.TABLE$              \
                                        (CURR.RECORD.INDEX%),26,2) +            \
                                        ":" + MID$(F14.TABLE$                   \
                                        (CURR.RECORD.INDEX%),28,2) + " "
                    CALL WRITE.RFCNTLST ("")

                    !Set the key for reading the EALAUTH
                    AF.OPERATOR.NO$ = PACK$(RIGHT$(("00000000" +                \
                                      MID$(F14.TABLE$(CURR.RECORD.INDEX%),      \
                                      13,3)),8))
                    CALL GET.OP.NAME$

                    RFCNTLST.RECORD$ =  "By " + LEFT$(GET.OP.NAME$,17)
                    CALL WRITE.RFCNTLST ("")
                    PREV.CNT.STATUS$ = CURR.CNT.STATUS$

                ENDIF ELSE IF CURR.CNT.STATUS$ = "3" THEN BEGIN

                    IF PREV.CNT.STATUS$ = " " THEN BEGIN
                        RFCNTLST.RECORD$ = "Counted Lists       "
                        CALL WRITE.RFCNTLST ("2M")
                        RFCNTLST.RECORD$ = "Part Counted Lists  "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF ELSE IF PREV.CNT.STATUS$ = "1" THEN BEGIN
                        RFCNTLST.RECORD$ = "Part Counted Lists  "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF

                    !Checks the Current Status to verify that
                    !    PREV.CNT.STATUS$ <> CURR.CNT.STATUS$
                    !If PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ then
                    !    print the header

                    IF PREV.CNT.STATUS$ <> CURR.CNT.STATUS$ THEN BEGIN
                        RFCNTLST.RECORD$ = "Uncounted Lists     "
                        CALL WRITE.RFCNTLST ("2M")
                    ENDIF

                    !If the list is User generated list, write the time
                    !and the user created the list in the report

                    IF COUNTER% = 2 THEN BEGIN
                        AF.OPERATOR.NO$ = PACK$(RIGHT$(("00000000" +            \
                                          MID$(F14.TABLE$(CURR.RECORD.INDEX%),  \
                                          30,3)),8))
                        CALL GET.OP.NAME$
                        RFCNTLST.RECORD$ = MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                            9,2) + ":" +                        \     ! CCSK
                                           MID$(F14.TABLE$(CURR.RECORD.INDEX%), \
                                           11,2) + " " +                        \
                                           LEFT$(GET.OP.NAME$,14) ! OP NAME SET TO 14 CHARACTERS
                        CALL WRITE.RFCNTLST ("3M")
                    ENDIF

                    RFCNTLST.RECORD$ =  "List " + MID$(F14.TABLE$               \
                                        (CURR.RECORD.INDEX%),16,3) +            \
                                        " " + RIGHT$("   "  + STR$(VAL(MID$     \
                                        (F14.TABLE$(CURR.RECORD.INDEX%),        \
                                        19,3))), 3) + " items  "
                    CALL WRITE.RFCNTLST ("")
                    PREV.CNT.STATUS$ = CURR.CNT.STATUS$

                ENDIF
            ENDIF
        NEXT CURR.RECORD.INDEX%

        !If PREV.CNT.STATUS$ = " " then write the three headers
        !    Counted Lists, Part Counted Lists, Uncounted Lists
        !If PREV.CNT.STATUS$ = "1" then write the two headers
        !    Part Counted Lists, Uncounted Lists
        !If PREV.CNT.STATUS$ = "2" then write the two headers
        !    Uncounted Lists

        IF PREV.CNT.STATUS$ = " " THEN BEGIN
            RFCNTLST.RECORD$ = "Counted Lists       "
            CALL WRITE.RFCNTLST ("2M")
            RFCNTLST.RECORD$ = "Part Counted Lists  "
            CALL WRITE.RFCNTLST ("2M")
            RFCNTLST.RECORD$ = "Uncounted Lists     "
            CALL WRITE.RFCNTLST ("2M")
        ENDIF ELSE IF PREV.CNT.STATUS$ = "1" THEN BEGIN
            RFCNTLST.RECORD$ = "Part Counted Lists  "
            CALL WRITE.RFCNTLST ("2M")
            RFCNTLST.RECORD$ = "Uncounted Lists     "
            CALL WRITE.RFCNTLST ("2M")
        ENDIF ELSE IF PREV.CNT.STATUS$ = "2" THEN BEGIN
            RFCNTLST.RECORD$ = "Uncounted Lists     "
            CALL WRITE.RFCNTLST ("2M")
        ENDIF

        RFCNTLST.RECORD$ = "--------------------"
        CALL WRITE.RFCNTLST ("")

    NEXT COUNTER%

    RFCNTLST.RECORD$ = "   END OF REPORT    "
    CALL WRITE.RFCNTLST ("")

RETURN

\*****************************************************************************
\***
\***    DEALLOCATE.SESSION.NUMBERS
\***    Detail        : Perform CALL.F20.SESS.NUM.UTILITY to de-allocate file
\***                    session numbers from all files referenced by the
\***                    program.
\***
\***..........................................................................

DEALLOCATE.SESSION.NUMBERS:

    ADXSERVE.DATA$ = "Deallocating Session numbers - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

    FUNCTION.FLAG$ EQ "C"

    PASSED.INTEGER% = CLOLF.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    PASSED.INTEGER% = CLILF.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    PASSED.INTEGER% = AF.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    PASSED.INTEGER% = RFCNTLST.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    ADXSERVE.DATA$ = "Session numbers deallocating success - MINRFCNT"
    GOSUB DISPLAY.MESSAGE

RETURN

\*****************************************************************************
\***
\***    Display background message
\***
\***..........................................................................

DISPLAY.MESSAGE:

    IF COMM.MODE.FLAG$ = "B" THEN BEGIN
        ADX.INTEGER%  = 0
        ADX.FUNCTION% = 26
        CALL ADXSERVE (ADX.RETURN.CODE%,     \
                       ADX.FUNCTION%,        \
                       ADX.INTEGER%,         \
                       ADXSERVE.DATA$)
    ENDIF ELSE BEGIN
        PRINT ADXSERVE.DATA$
    ENDIF

RETURN

\*****************************************************************************
\***
\***    FORMAT.CURRENT.CODE:
\***    Detail        : Sets CURRENT.CODE.LOGGED$ for use with application
\***                    event log.
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
\***    CALL.F01.APPLICATION.LOG:
\***    Detail        : References APPLICATION.LOG (F01) to write details
\***                    of event defined by EVENT.NUMBER% and VAR.STRING.1$
\***                    to Application Event Log, and to display any message
\***                    defined by MESSAGE.NUMBER% and VAR.STRING.2$.
\***
\***..........................................................................

CALL.F01.APPLICATION.LOG:
    FUNCTION.RETURN.CODE% EQ    \
        APPLICATION.LOG         \
        (MESSAGE.NUMBER%,       \
        VAR.STRING.1$,          \
        VAR.STRING.2$,          \
        EVENT.NUMBER%)
RETURN

\*****************************************************************************
\***
\***    LOG.AN.EVENT.106:
\***    Detail        : Writes details of Event 106 to application event
\***                    log and displays message B501 (for file open errors)
\***                    or B514 (for other errors).
\***
\***..........................................................................

LOG.AN.EVENT.106:

    ADXSERVE.DATA$ = "LOG.AN.EVENT.106 File Op= " + FILE.OPERATION$
    GOSUB DISPLAY.MESSAGE

    EVENT.NUMBER% EQ 106
    GOSUB FORMAT.CURRENT.CODE
    VAR.STRING.1$ EQ                         \ ! Application event log data
        FILE.OPERATION$                    + \
        CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) + \ ! Two byte integer byte order
        CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) + \ ! reversed to give hex number
        CURRENT.CODE.LOGGED$

    GOSUB CALL.F01.APPLICATION.LOG

RETURN

\*****************************************************************************
\***
\***    ERROR.DETECTED:
\***    Detail        : Below mentioned are the main process done here.
\***                    1. Increments ERROR.COUNT% by one for errors that
\***                       are not handled
\***                    2. Further errors within ERROR.DETECTED causing
\***                       control to be passed here again result in this
\***                       test being failed and the immediate diversion of
\***                       program control to STOP.MINRFCNT
\***
\***..........................................................................

ERROR.DETECTED:

    IF ERR = "CU" THEN BEGIN
        ERROR.COUNT% = 0
        RESUME
    ENDIF

    IF CURRENT.REPORT.NUM% = RFCNTLST.REPORT.NUM% THEN BEGIN
        IF FILE.OPERATION$ = "W" THEN BEGIN
            ADXSERVE.DATA$ = "Error in writing to RFCNTLST.DAY file"
            GOSUB DISPLAY.MESSAGE
            GOSUB LOG.AN.EVENT.106
            RESUME STOP.MINRFCNT
        ENDIF ELSE IF FILE.OPERATION$ = "O" THEN BEGIN
            CREATE RFCNTLST.FILE.NAME$ AS RFCNTLST.SESS.NUM%
            ADXSERVE.DATA$ = "Created RFCNTLST.DAY File" + STRING$(17, " ") + \
                             "- MINRFCNT "
            GOSUB DISPLAY.MESSAGE
            RESUME
        ENDIF
    ENDIF

    IF FILE.OPERATION$ = "O" THEN BEGIN
        ERROR.COUNT% = 0
        IF CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM% THEN BEGIN
            ADXSERVE.DATA$ = "Error in opening CLOLF file"
            GOSUB DISPLAY.MESSAGE
            GOSUB LOG.AN.EVENT.106
            RESUME STOP.MINRFCNT
        ENDIF ELSE IF CURRENT.REPORT.NUM% = CLILF.REPORT.NUM% THEN BEGIN
            ADXSERVE.DATA$ = "Error in opening CLILF file"
            GOSUB DISPLAY.MESSAGE
            GOSUB LOG.AN.EVENT.106
            RESUME STOP.MINRFCNT
        ENDIF ELSE IF CURRENT.REPORT.NUM% = AF.REPORT.NUM% THEN BEGIN
            ADXSERVE.DATA$ = "Error in opening EALAUTH file"
            GOSUB DISPLAY.MESSAGE
            RESUME
        ENDIF
    ENDIF

    FUNCTION.RETURN.CODE% EQ  \
    STANDARD.ERROR.DETECTED   \
    (ERRN,                    \
    ERRF%,                    \
    ERRL,                     \
    ERR)

    ERROR.COUNT% = ERROR.COUNT% + 1
    IF ERROR.COUNT% > 1 THEN BEGIN
        RESUME STOP.MINRFCNT
    ENDIF

END SUB


