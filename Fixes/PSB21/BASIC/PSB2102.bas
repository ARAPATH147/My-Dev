\*******************************************************************************
\*******************************************************************************
\***
\***
\***            PROGRAM         :       PSB21
\***            MODULE          :       PSB2102
\***            AUTHOR          :       Charles Skadorwa / Mark Walker / Mark Goode
\***            DATE WRITTEN    :       Sept 2011
\***
\*******************************************************************************
\***
\***    VERSION 1.0      Charles Skadorwa/Mark Walker/Mark Goode 30 SEP 2011
\***    Initial version.
\***
\***    VERSION 1.1.                ROBERT COWEY.                25 JAN 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.4.
\***    Defect 3569 (c).
\***    Modified PROCESS.PGF error handling
\***    If error occurs processing PGF only ...
\***      Program continues and relies on PGFDIR data
\***    If error occurs processing PGFDIR only ...
\***      Program continues and relies on successfully processed PGF data
\***    If error occurs processing BOTH the PGF and PGFDIR ...
\***      Program ends setting JOBOK flag to "X"
\***
\***    VERSION 1.2                Charles Skadorwa.             19 FEB 2012.
\***    Minor changes following code WPI's.
\***
\***    VERSION 1.3.                ROBERT COWEY.                22 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.6.
\***
\***    Defect 50 - Commented 1.3 RC (50).
\***    Shortened message text to fit 46 character background display.
\***
\***    Defect 103 - Commented 1.3 RC (103).
\***    When processing REFPGF data within PROCESS.KEYED.FILES ...
\***    Cater for non-Dispose return route of blank (ie, special instructions).
\***
\***    Review by Brian - Commanted 1.3 RC
\***    Changed SIZE.PLUS.FIFTY.PERCENT% from 1000 to 1500 records to cater
\***    for PGF containing close to 1000 records.
\***
\***    VERSION 1.4.                ROBERT COWEY.                22 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.18.
\***
\***    Pilot defect - Commented 1.4 RC (236)
\***    Modified PROCESS.PGF to set FILE.OPERATION$ "O" prior to PGFDIR opens.
\***
\***    VERSION 1.5.                ROBERT COWEY.                28 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.20.
\***
\***    Pilot defect - Commented 1.5 RC (240)
\***    Modified PROCESS.PGF to separate its ON ERROR routines call to
\***    PROGRAM.EXIT into a separate procedure CHECK.PROCESS.PGF.
\***    This ensures later errors within PROGRAM.EXIT are trapped by the main
\***    ERROR.DETECTED routine instead of the internal PROCESS.PGF ON ERROR 
\***    routine (which can cause looping).
\***
\***    VERSION A                   Mark Walker                 8th Jul 2014
\***    F353 Deal Limits Increase
\***    Removed DINF and NIADF file processing.
\***
\***--------------------------------------------------------------------------------
\***  IMPORTANT: When you change PSB21, please search for the line containing
\***------------        CALL DO.MESSAGE("PSB21 PROGRAM START
\***             in Module PSB2100 and wind the date and version on.
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\*******************************************************************************
\***
\***    Module Overview
\***    ----------------
 \***
\***    The purpose of this module is to remove old product groups from the PGF.
\***    It achieves this by reading the existing PGF into a table and reading
\***    through the PGFDIR. All PGFDIR records are written to a work file,
\***    however, its flags are updated if the product group exists on the PGF.
\***    At the end of processing, the PGF is replaced by the work file.
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\***
\***    Included code defining file related global variables
\***
\***............................................................................
!%INCLUDE DINFDEC.J86   ! Deal Item Inforformation File                     !AMW
%INCLUDE IUFDEC.J86     ! Item Update File
!%INCLUDE NIADFDEC.J86  ! New Item Deal File                                !AMW
%INCLUDE PGFDEC.J86     ! Product Group Description File


\*******************************************************************************
\***
\***    Included code defining function related global variables
\***
\***............................................................................
%INCLUDE PSBF01G.J86    ! APPLICATION.LOG
%INCLUDE PSBF13G.J86    ! PSDATE
%INCLUDE PSBF20G.J86    ! Sess Num Utility
%INCLUDE PSBF42G.J86    ! Update Deal Files Function


\*******************************************************************************
\***    PSB2102 variables
\***............................................................................
    INTEGER*1 GLOBAL                        \
        DINF.AVAILABLE,                     \
        DO.PGF.FILE,                        \
        FALSE,                              \
\       PROCESS.NIADF,                      \                               !AMW
        TRUE,                               \
        WAITED.30.SECS                      !

    INTEGER*2 GLOBAL                        \
        CURR.SESS.NUM%,                     \
        CURRENT.REPORT.NUM%,                \
        I%                                  !                               !AMW
!       NIADF.BACKUP.SESS.NUM%              !                               !AMW

    INTEGER*4 GLOBAL                        \
        PGFWRK.SESS.NUM%,                   \
        PSBF30.RC%,                         \ ! 1.1 RC
        RC%,                                \
        SIZE.PLUS.FIFTY.PERCENT%,           \
        TOTAL.PGF.RECS%                     !


    STRING GLOBAL                           \
        PGF.TABLE$(1)                       !

    STRING GLOBAL                           \
        BACKUP.SOURCE.FILE$,                \
        FILE.OPERATION$,                    \
        JOBSOK.FLAG$,                       \
        MODULE.NUMBER$,                     \ !1.2CSk
        PGFDIR.BACKUP.NAME$,                \
        PGFWRK.FILE.NAME$,                  \
        VAR.STRING.1$,                      \
        VAR.STRING.2$                       !


\*******************************************************************************
\***
\***    External functions
\***
\***............................................................................
%INCLUDE ADXCOPY.J86    ! I.B.M. system subroutine for copying files
%INCLUDE CSORTDEC.J86   ! CSORT
!%INCLUDE DINFEXT.J86                                                       !AMW
%INCLUDE PGFEXT.J86

%INCLUDE PSBF01E.J86    ! APPLICATION.LOG
%INCLUDE PSBF13E.J86    ! PSDATE
%INCLUDE PSBF20E.J86    ! SESSION NUMBER UTILITY
%INCLUDE PSBF30E.J86    ! Process Keyed File Function
%INCLUDE PSBF24E.J86    ! STANDARD ERROR DETECTED                ! 1.1 RC
%INCLUDE PSBF42E.J86    ! Update Deal Files Function


FUNCTION DECAPI.MSG(PAYLOAD.MSG$) EXTERNAL
END FUNCTION

SUB DO.MESSAGE(MSG$, LOG%) EXTERNAL
END SUB

SUB LOG.EVENT(EVENT.NO%) EXTERNAL
END SUB

SUB PROGRAM.EXIT EXTERNAL                                        ! 1.1 RC
END SUB                                                          ! 1.1 RC

\************************************************************************
\***
\***    PUTN1
\***
\***    These routine insert a one byte integer into a string.
\***    POS% is the offset within the string and VALUE% is the source
\***    integer represented as a two byte integer
\***
\***    NOTE: THE OFFSET PASSED TO THIS ROUTINE IS ZERO INDEXED
\***
\************************************************************************

SUB PUTN1(DATA$, POS%, VALUE%)
    STRING    DATA$
    INTEGER*4 POS%
    INTEGER*2 VALUE%
    INTEGER*4 LOC%

    LOC% = SADD(DATA$) + POS% + 2  ! SADD returns the address of the
                                   ! length field of the string; add
                                   ! 2 to get the address of the data.

    POKE LOC%, VALUE%

END SUB


\********************************************************************************
\***                                                                            *
\***      PROCESS.KEYED.RECORD$                                                 *
\***                                                                            *
\***      'User exit' for PROCESS.KEYED.FILE (PSBF30)                           *
\***                                                                            *
\********************************************************************************
FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC

    STRING PROCESS.KEYED.RECORD$,  \
           RECORD$,                \
           RESALEABLE$,            \
           RETURN.LABEL.TYPE$,     \
           RETURN.ROUTE$,          \
           SPECIAL.INSTRUCTION$,   \
           WORK$                   !

    INTEGER*2 CGRP.NUM%,           \
              INDICAT%,            \
              PGRP.NUM%            !

    IF DO.PGF.FILE THEN BEGIN

        TOTAL.PGF.RECS% = TOTAL.PGF.RECS% + 1
        PGF.TABLE$(TOTAL.PGF.RECS%) = LEFT$(RECORD$, 23) ! Ignore Filler

!   ENDIF ELSE IF PROCESS.NIADF THEN BEGIN                                  !AMW
!                                                                           !AMW
!       WORK$           = LEFT$(RECORD$,3)                                  !AMW
!       FILE.OPERATION$ = "D"                                               !AMW
!                                                                           !AMW
!       !Check if item is on an active deal.                                !AMW
!       RC% = 0                                                             !AMW
!       IF DINF.AVAILABLE THEN BEGIN                                        !AMW
!           RC% = DINF.GET.FIRST.DEAL.ITEM (WORK$, 0)                       !AMW
!       ENDIF                                                               !AMW
!                                                                           !AMW
!       IF RC% = 0 THEN BEGIN                                               !AMW
!           RC% = UPDATE.ITEM.DEAL.INFO(WORK$, UNPACK$(RIGHT$(RECORD$,3)))  !AMW
!           IF RC% = 0 THEN BEGIN                                           !AMW
!                ! Successfully set appropriate deal fields on both the     !AMW
!                ! IRF and IRFDEX.                                          !AMW
!                ! Remove item from NIADF file                              !AMW
!                IF END # NIADF.BACKUP.SESS.NUM% THEN NIADF.DEL.ERROR       !AMW
!                DELREC NIADF.BACKUP.SESS.NUM%; WORK$                       !AMW
!           ENDIF                                                           !AMW
!       ENDIF ELSE BEGIN                                                    !AMW
!           !Not on NIADF remove from NIADF file                            !AMW
!           IF END # NIADF.BACKUP.SESS.NUM% THEN NIADF.DEL.ERROR            !AMW
!           DELREC NIADF.BACKUP.SESS.NUM%; WORK$                            !AMW
!       ENDIF                                                               !AMW
!                                                                           !AMW
    ENDIF ELSE BEGIN

        !--------------------------------------------------------------------------
        ! Store the REFPGF fields necessary for later processing of IRF in the
        ! array REFPGF.RECORD$. The offset of the array will be the concept
        ! group. Each element in the array is an array of 1000 elements 0f 2-bytes.
        ! denoting product group and flags.
        !--------------------------------------------------------------------------
        IF LEFT$(UNPACK$(RECORD$),2) = "00" THEN BEGIN
            ! Only extract Product Group info (ignore messages)
            REFPGF.COUNT% = REFPGF.COUNT% + 1

            CGRP.NUM%            = VAL(UNPACK$(MID$(RECORD$,2,1)))
            PGRP.NUM%            = VAL(UNPACK$(MID$(RECORD$,3,2)))
            RESALEABLE$          = MID$(RECORD$,5,1) ! Items allowed to be resold (Y or N)
            RETURN.ROUTE$        = MID$(RECORD$,6,1) ! (D or R) Dispose or Return
            SPECIAL.INSTRUCTION$ = UNPACK$(MID$(RECORD$,7,1)) ! EXCEPTION.NO 0 - 63
            RETURN.LABEL.TYPE$   = MID$(RECORD$,8,1) ! Use this to reference the "99" records (00 = no exception)
                                                     ! 9900 01 00 - need to stitch together the special ins.?

            IF REFPGF.RECORDS$(CGRP.NUM%) = "" THEN BEGIN
                WORK$ = STRING$(2000,CHR$(0))
            ENDIF ELSE BEGIN
                WORK$ = REFPGF.RECORDS$(CGRP.NUM%)
            ENDIF


            IF RETURN.ROUTE$ = "D" THEN BEGIN                 ! Dispose
                INDICAT% = 00H
            ENDIF ELSE IF RETURN.LABEL.TYPE$ = "R" THEN BEGIN ! Return
                INDICAT% = 40H
            ENDIF ELSE IF RETURN.LABEL.TYPE$ = "D" THEN BEGIN ! Direct
                INDICAT% = 80H
            ENDIF ELSE IF RETURN.LABEL.TYPE$ = "S" THEN BEGIN ! Semi-Centralised
                INDICAT% = 0C0H
            ENDIF ELSE IF RETURN.LABEL.TYPE$ = " " THEN BEGIN ! Special Instructions  ! 1.3 RC (103)
                INDICAT% = 20H                                                        ! 1.3 RC (103)
            ENDIF ELSE BEGIN
                !No return label
                INDICAT% = 00H
            ENDIF

            CALL PUTN1(WORK$, PGRP.NUM% * 2, INDICAT%)

            INDICAT% = VAL(SPECIAL.INSTRUCTION$)
            IF RESALEABLE$ = "Y" THEN BEGIN
                INDICAT% = INDICAT% OR 40H
            ENDIF
            CALL PUTN1(WORK$, PGRP.NUM% * 2 + 1, INDICAT%)

            REFPGF.RECORDS$(CGRP.NUM%) = WORK$

        ENDIF

    ENDIF

    PROCESS.KEYED.RECORD$ = RECORD$

    EXIT FUNCTION

!NIADF.DEL.ERROR:                                                           !AMW

    CALL LOG.EVENT(106)

END FUNCTION



\******************************************************************************
\***
\***    PROCESS.PGF
\***
\******************************************************************************
\***
\***    Creates a new PGF file using PGFDIR
\***
\******************************************************************************
SUB PROCESS.PGF PUBLIC

    MODULE.NUMBER$ = "2"                                                        !1.2CSk

    CALL DO.MESSAGE("PSB21 PGF PROCESSING", TRUE)
    CALL DO.MESSAGE("PSB21 PGF PROCESSING - PROCESS.PGF", TRUE)

    DIM PGF.TABLE$(1000)


    ON ERROR GOTO ERROR.PROCESS.PGF

    TOTAL.PGF.RECS% = 0
    DO.PGF.FILE     = TRUE

    CALL DO.MESSAGE("PSB21 PGF PROCESSING - Read PGF into table", TRUE)

!   Separate return code used to save any error associated with PGF   ! 1.1 RC
!   Missing PGF will cause PSBF30 to log an 00000052 SIZE error       ! 1.1 RC
    PSBF30.RC% = PROCESS.KEYED.FILE(PGF.FILE.NAME$, PGF.REPORT.NUM%,"Y") !  Y = Read PGF only ! 1.1 RC

    IF PSBF30.RC% <> 0 THEN BEGIN                                     ! 1.1 RC
         CALL DO.MESSAGE("PSB21 *** ERROR processing PGF", FALSE)     ! 1.1 RC
!        Allow program to continue and update PGF from PGFXX          ! 1.1 RC
    ENDIF                                                             ! 1.1 RC

    CALL DO.MESSAGE("PSB21 PGF PROCESSING - Creating PGFWRK", TRUE)
    !---------------------------------------------------------------------------
    ! Open the PGFDIR as a sequential file. Create a temporary work file PGFWRK.
    ! For each record on the PGFDIR add a keyed record to the PGFWRK file. If
    ! the Product Group from the PGFDIR record exists in the PGF array, set the
    ! fields PGF.SEL.FLAG$ and PGF.OSSR.FLAG$ to their corresponding values from
    ! the PGF array otherwise initialise the fields to "N".
    !---------------------------------------------------------------------------
    PGFWRK.FILE.NAME$ = "ADXLXACN::D:/ADX_UDT1/PGFWRK.TMP"
    CALL SESS.NUM.UTILITY("O", 426, PGFWRK.FILE.NAME$)   ! 426 = General Temp file Report Num
    PGFWRK.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF TOTAL.PGF.RECS% < 1000 THEN BEGIN
        SIZE.PLUS.FIFTY.PERCENT% = 1500                      ! Default size  ! 1.3 RC
    ENDIF ELSE BEGIN
        SIZE.PLUS.FIFTY.PERCENT% = TOTAL.PGF.RECS% * 1.5     ! Add 50% to size of keyed file
    ENDIF

    FILE.OPERATION$ = "C"
    CREATE POSFILE PGFWRK.FILE.NAME$ KEYED 3, , ,                       \
           SIZE.PLUS.FIFTY.PERCENT% RECL PGF.RECL% AS PGFWRK.SESS.NUM%  \
           LOCKED MIRRORED ATCLOSE

    CALL DO.MESSAGE("PSB21 PGF PROCESSING - Opening PGFDIR", TRUE) ! 1.1 RC

    CALL SESS.NUM.UTILITY("O", PGFDIR.REPORT.NUM%, PGFDIR.FILE.NAME$)
    PGFDIR.SESS.NUM% = F20.INTEGER.FILE.NO%

    CURRENT.REPORT.NUM% = PGFDIR.REPORT.NUM%
    !If PGFDIR file exists open and process
!   If PGFDIR does not exist the IF END will execute so SIZE statement redundant  ! 1.1 RC
!   IF SIZE(PGFDIR.FILE.NAME$) > 0 THEN BEGIN                                     ! 1.1 RC

!       If PGFDIR missing then bypass rest of procedure as nothing to process     ! 1.1 RC
        FILE.OPERATION$ = "O"                                                     ! 1.4 RC (236)
        IF END # PGFDIR.SESS.NUM% THEN PGFDIR.FILE.ERROR
        OPEN PGFDIR.FILE.NAME$ AS PGFDIR.SESS.NUM% LOCKED NOWRITE NODEL

        RC% = READ.PGFDIR

        WHILE RC% = 0

            FOR I% = 1 TO TOTAL.PGF.RECS%
                IF PGF.PROD.GRP.NO$ = UNPACK$(LEFT$(PGF.TABLE$(I%),3)) THEN BEGIN
                    PGF.SEL.FLAG$   = MID$(PGF.TABLE$(I%),22,1)
                    PGF.OSSR.FLAG$  = MID$(PGF.TABLE$(I%),23,1)
                    I% = TOTAL.PGF.RECS% + 1  ! Found - Jump out
                ENDIF
            NEXT I%

            PGF.PROD.GRP.NO$ = PACK$(RIGHT$(STRING$(6,"0") + PGF.PROD.GRP.NO$,6))

            WRITE FORM "C3,C18,2C1,C7"; \
              # PGFWRK.SESS.NUM% ;      \
                    PGF.PROD.GRP.NO$,   \
                    PGF.PROD.GRP.NAME$, \
                    PGF.SEL.FLAG$,      \
                    PGF.OSSR.FLAG$,     \
                    PGF.SPACE$

            RC% = READ.PGFDIR
        WEND

        CLOSE PGFDIR.SESS.NUM%

        CALL DO.MESSAGE("PSB21 PGF PROCESSING - Del PGF; Rename PGFWRK", TRUE) ! 1.3 RC (50)

        FILE.OPERATION$ = "O"
        IF END #PGF.SESS.NUM% THEN NO.PGF.FILE
        OPEN PGF.FILE.NAME$ AS PGF.SESS.NUM%

        FILE.OPERATION$ = "D"
        DELETE PGF.SESS.NUM%

 NO.PGF.FILE:

        WAITED.30.SECS = FALSE

 TRY.RENAME.PGFWRK.AGAIN:

        RC% = RENAME(PGF.FILE.NAME$, PGFWRK.FILE.NAME$)

        IF RC% <> -1 THEN BEGIN

            IF NOT WAITED.30.SECS THEN BEGIN
                WAIT; 30000
                WAITED.30.SECS = TRUE
                GOTO TRY.RENAME.PGFWRK.AGAIN
            ENDIF ELSE BEGIN
                CALL DO.MESSAGE("PSB21 *** ERROR renaming " + \
                     PGFWRK.FILE.NAME$ + " to "             + \
                     PGF.FILE.NAME$ + " after waiting 30 seconds", FALSE)
                CALL LOG.EVENT(14)

                EXIT SUB
            ENDIF
        ENDIF

        IF RC% = 0 THEN BEGIN
            CALL DO.MESSAGE("PSB21 *** ERROR renaming " + \
                            PGFWRK.FILE.NAME$ + " to "  + \
                            PGF.FILE.NAME$, FALSE)
            EXIT SUB
        ENDIF

        !----------------------------------------------------------------------
        ! Maintain seven days backups of PGFDIR files: PGFDIR.MON to PGFDIR.SUN
        !----------------------------------------------------------------------
        CALL DO.MESSAGE("PSB21 PGF PROCESSING - Backup to PGFDIR." + F13.DAY$, TRUE) ! 1.3 RC (50)

        PGFDIR.BACKUP.NAME$ = "ADXLXACN::D:\ADX_UDT1\PGFDIR." + F13.DAY$

        CALL ADXCOPYF(RC%, PGFDIR.FILE.NAME$, PGFDIR.BACKUP.NAME$,0,0,0)

        IF RC% <> 0 THEN BEGIN
            CALL DO.MESSAGE("PSB21 *** ERROR copying " + \
                            PGFDIR.FILE.NAME$ + " to " + \
                            PGFDIR.BACKUP.NAME$, FALSE)
            BACKUP.SOURCE.FILE$ = "PGFDIR  "
            CALL LOG.EVENT(57)
            EXIT SUB
        ENDIF

        ! Open and delete the PGFDIR file
        FILE.OPERATION$ = "O"                                                     ! 1.4 RC (236)
        IF END # PGFDIR.SESS.NUM% THEN PGFDIR.FILE.ERROR
        OPEN PGFDIR.FILE.NAME$ AS PGFDIR.SESS.NUM% LOCKED NOWRITE NODEL

        IF END # PGFDIR.SESS.NUM% THEN PGFDIR.FILE.ERROR
        DELETE PGFDIR.SESS.NUM%
        CALL DO.MESSAGE("PSB21 PGF PROCESSING - PGFDIR file deleted", TRUE)

        CALL DO.MESSAGE("PSB21 PGF PROCESSING - Completed", TRUE)      ! 1.3 RC (50)

        EXIT SUB
!   ENDIF                                                              ! 1.1 RC


 PGFDIR.FILE.ERROR: ! Moved prior to PROCESS.PGF's ON ERROR routine    ! 1.1 RC

   CALL DO.MESSAGE("PSB21 *** ERROR opening PGFXX file", FALSE)        ! 1.1 RC
   CALL LOG.EVENT(106)                                                 ! 1.1 RC
   CLOSE PGFWRK.SESS.NUM% ! Will not now be used                       ! 1.1 RC

   IF PSBF30.RC% <> 0 THEN BEGIN                                       ! 1.1 RC
!      Lines moved to CHECK.PROCESS.PGF                                ! 1.5 RC (240)
!      Set JOBSOK.FLAG$ to indicate failure to PSB2100                 ! 1.5 RC (240)
       JOBSOK.FLAG$ = "X"                                              ! 1.1 RC
   ENDIF                                                               ! 1.1 RC

   EXIT SUB ! PGFDIR cannot be processed so exit routine               ! 1.1 RC


 ERROR.PROCESS.PGF: ! PROCESS.PGF's general ON ERROR rouitine          ! 1.1 RC

   CALL DO.MESSAGE("PSB21 *** ERROR detected processing PGFXX etc", FALSE) ! 1.1 RC

   CALL STANDARD.ERROR.DETECTED(ERRN,  \                               ! 1.1 RC
                                 ERRF%, \                              ! 1.1 RC
                                 ERRL,  \                              ! 1.1 RC
                                 ERR)                                  ! 1.1 RC

   IF PSBF30.RC% <> 0 THEN BEGIN                                       ! 1.1 RC
!      Lines moved to CHECK.PROCESS.PGF                                ! 1.5 RC (240)
!      Set JOBSOK.FLAG$ to indicate failure to PSB2100                 ! 1.5 RC (240)
       JOBSOK.FLAG$ = "X"                                              ! 1.1 RC
       EXIT SUB                                                        ! 1.5 RC (240)
   ENDIF                                                               ! 1.1 RC

END SUB


\******************************************************************************
\***
\***    CHECK.PROCESS.PGF ! New procedure for Rv 1.5 RC          ! 1.5 RC (240)
\***
\******************************************************************************
\***
\***    Check for error within PROCESS.PGF.
\***    Report any such error and call PROGRAM.EXIT.
\***    PROGRAM.EXIT is called from outside PROCESS.PGF to ensure that any
\***    general error within it is trapped by ERROR.DETECTED (rather than
\***    by the PROCESS.PGF ON ERROR routine which can cause looping).
\***
\******************************************************************************

SUB CHECK.PROCESS.PGF PUBLIC ! New procedure for Rv 1.5 RC       ! 1.5 RC (240)

    IF JOBSOK.FLAG$ = "X" THEN BEGIN
!       Major problem occured processing PGF and/or PGFXX
        CALL DO.MESSAGE("PSB21 *** ERROR on PGF and/or PGFXX", FALSE)
        CALL DO.MESSAGE("PSB21 *** FATAL ERROR - ABORTING ***", FALSE)
        CALL PROGRAM.EXIT
    ENDIF

END SUB

