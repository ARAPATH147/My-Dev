\*******************************************************************************
\*******************************************************************************
\***
\***
\***            PROGRAM         :       PSB21
\***            MODULE          :       PSB2105
\***            AUTHOR          :       Charles Skadorwa / Mark Goode
\***            DATE WRITTEN    :       Sept 2011
\***
\*******************************************************************************
\***
\***    1.0           Charles Skadorwa  / Mark Goode                 30 SEP 2011
\***    Initial version.
\***
\***    1.1   Charles Skadorwa                                       20 JAN 2012
\***          Defect 7: No. of skipped records now reported when an event 103
\***                    is logged.
\***
\***    1.2   Charles Skadorwa                                       20 FEB 2012
\***          Defect following code WPI - JOBOK last batch processed date needs
\***          to be set to the Datetimestamp from the BCF even if no IUF in order
\***          to help Service resolve issues.
\***
\***    VERSION 1.3.               ROBERT COWEY.                 24 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.13.
\***
\***    Defect 190 - Commented 1.3 RC (190)
\***    Moved DELETE.PPFK.KEYED.FILE into PROGRAM.EXIT to ensure PPFK is
\***    always deleted even when program does not complete successfully.
\***
\***    VERSION 1.4.                ROBERT COWEY.                30 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.4.
\***
\***    Defect 190 corrrection - Commented 1.4 RC (190)
\***    Corrected PROGRAM.EXIT to delete PPFK (via DESTROY.PPFK) when
\***    JOBSOK.FLAG$ is "X" (since this is set prior to all unexpected
\***    calls to PROGRAM.EXIT).
\***
\***    VERSION 1.5.                ROBERT COWEY.                04 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.15.
\***
\***    Defect 223 - Commented 1.5 RC (223)
\***    Modified PROGRAM.EXIT to prevent J103 alert for legacy format IUF.
\***
\***    VERSION 1.5 supplemental    CHARLES SKADORWA.
\***    Ensured BCF is only opened when needed (then closed afterwards).
\***
\***    VERSION 1.6.                ROBERT COWEY.                09 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.16.
\***
\***    Defect 226 - Commented 1.6 RC (226)
\***    Ensured CIPPMR only closed when present.
\***
\***    VERSION 1.7.                ROBERT COWEY.                24 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.19.
\***
\***    Pilot defect - Commented 1.7 RC (238)
\***    Modified PROGRAM.EXIT to delete any RAM files used by the program
\***    that are still present when the program fails.
\***
\***    VERSION 1.8.                ROBERT COWEY.                28 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.20.
\***
\***    Pilot defect - Commented 1.8 RC (240)
\***    Issued RESTRICT prior to deleting files from RAM.
\***    Replaced call to DESTROY.PPFK with OSSHELL delete to make more robust.
\***
\***    VERSION 1.9.                ROBERT COWEY.                11 JUN 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.21.
\***    Incremented lab release version text to v1.21.
\***
\***    Lab test defect - Commented 1.9 RC (242)
\***    Corrected NIADF processing to ensure IRFDEX updated when new item is
\***    on more than three deals.
\***    Allocated IRFDEX session number and opened and closed the file.
\***
\***    VERSION A                   Mark Walker                 8th Jul 2014
\***    F353 Deal Limits Increase
\***    Removed DINF and NIADF file processing.
\***
\***    VERSION B                   Rejiya Nair            16th May 2016
\***    PRJ1452 Restricting Item Sales
\***    - Close IRFITGRP file.
\***    - Removed the previously tagged commented out code
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
\***    ---------------
\***
\***    The purpose of this module is to perform Program Exit processing.
\***
\*******************************************************************************
\*******************************************************************************

%INCLUDE BCFDEC.J86     ! Boots Control File
%INCLUDE CIPPMDEC.J86   ! Markdown File
%INCLUDE DRUGDEC.J86    ! Drug File
%INCLUDE IDFDEC.J86     ! Item Data File
%INCLUDE IEFDEC.J86     ! Item EAN File
%INCLUDE IEXDEC.J86     ! Item Extension File
%INCLUDE IRFDEC.J86     ! Item Reference File
%INCLUDE ITGRPDEC.J86   ! IRF Attribute extension File
%INCLUDE ISFDEC.J86     ! Item Shelf Edge Label Description File
%INCLUDE IUFDEC.J86     ! Item Update file
%INCLUDE JOBOKDEC.J86   ! Jobs OK File
%INCLUDE LOCALDEC.J86   ! Local Price File
%INCLUDE NLINEDEC.J86   ! New Lines File
%INCLUDE RICFDEC.J86    ! Redeem Items Change File
%INCLUDE STOCKDEC.J86   ! Stock File

%INCLUDE PSBF01G.J86    ! APPLICATION.LOG
%INCLUDE PSBF19G.J86    ! UPDATE IRF GLOBALS

INTEGER*1 GLOBAL                        \
    CIPPM.PRESENT,                      \                       ! 1.6 RC (226)
    FALSE,                              \
    IUF.EXISTS,                         \
    TRUE                                !

INTEGER*2 GLOBAL                        \
    CURRENT.REPORT.NUM%,                \
    MESSAGE.NO%                         !

INTEGER*4 GLOBAL                        \
    IUF.ITEM.SKIP%,                     \
    IUF.J103.COUNT%,                    \
    RC%                                 !

STRING GLOBAL                           \
    ACD.FLAG$,                          \
    DEC.MESSAGE$,                       \
    ERROR$,                             \
    EVENT.222.DATA$,                    \
    FILE.OPERATION$,                    \
    JOBSOK.FLAG$,                       \
    MODULE.NUMBER$,                     \
    PROCESSING.DATE$,                   \
    PSB20.PATH.NAME$,                   \
    STORE.NUMBER$,                      \
    SUCCESS$,                           \
    VAR.STRING.1$,                      \
    VAR.STRING.2$                       !


%INCLUDE ADXERROR.J86
%INCLUDE BCFEXT.J86     ! Boots Control File
%INCLUDE IUFEXT.J86
%INCLUDE JOBOKEXT.J86   ! Jobs OK File

%INCLUDE BASROUT.J86    ! OSSHELL function (and others)         ! 1.7 RC (238)

%INCLUDE PSBF01E.J86    ! APPLICATION.LOG
%INCLUDE PSBF19E.J86    ! UPDATE IRF GLOBALS
%INCLUDE PSBF48E.J86    ! DEC API Logging


SUB BACKUP.IUF.FILE EXTERNAL
END SUB

SUB DELETE.PPFK.KEYED.FILE EXTERNAL                             ! 1.3 RC (190)
END SUB                                                         ! 1.3 RC (190)

SUB DO.MESSAGE(MSG$, LOG%) EXTERNAL
END SUB

FUNCTION DESTROY.PPFK EXTERNAL                                  ! 1.4 RC (190)
INTEGER*1 DESTROY.PPFK                                          ! 1.4 RC (190)
END FUNCTION                                                    ! 1.4 RC (190)

\*******************************************************************************
\***
\*** MODULE.5.LOG.EVENT
\***
\*** Log Application Event. This has to be Local otherwise calling the Public
\*** Subprogram will cause a loop since the Public one calls PROGRAM.EXIT.
\***
\*******************************************************************************
SUB MODULE.5.LOG.EVENT(EVENT.NO%)

    INTEGER*1 EVENT.NO%
    INTEGER*2 REPORT.EVENT.NO%

    MODULE.NUMBER$ = "5"                                                        !1.2CSk
    VAR.STRING.2$ = " "

    IF EVENT.NO% = 222 THEN BEGIN
        ! Log DEC API Message issue
        VAR.STRING.1$ = EVENT.222.DATA$

    ENDIF ELSE IF EVENT.NO% = 106 THEN BEGIN
        ! IF END errors: Open/Read/Write/Delete/Create/Size
        VAR.STRING.1$ = FILE.OPERATION$                    +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,0))

    ENDIF ELSE IF EVENT.NO% = 103 THEN BEGIN
        ! Error processing one or more items on the IUF
        !Processing date + number of items skipped
        VAR.STRING.1$ = PACK$("20" + PROCESSING.DATE$ +                                  \
                        RIGHT$("000000" + STR$(IUF.J103.COUNT%),6))
        MESSAGE.NO%   = 103
        CALL ADXERROR (0, ASC("J"), 103, 3, 1, VAR.STRING.1$)
    ENDIF

    IF EVENT.NO% <> 103 THEN BEGIN
        RC% = APPLICATION.LOG (MESSAGE.NO%,     \
                               VAR.STRING.1$,   \
                               VAR.STRING.2$,   \
                               EVENT.NO%)
    ENDIF

    REPORT.EVENT.NO% = EVENT.NO%
    IF REPORT.EVENT.NO% < 0 THEN REPORT.EVENT.NO% = REPORT.EVENT.NO% + 256

    IF EVENT.NO% = 103 THEN BEGIN                                                         !1.1CSk
        CALL DO.MESSAGE("PSB21 *** LOG EVENT: " + STR$(REPORT.EVENT.NO%) + " - "  +  \    !1.1CSk
                          STR$(IUF.J103.COUNT%) + " Record(s) Skipped", FALSE)            !1.1CSk
    ENDIF ELSE BEGIN                                                                      !1.1CSk
        CALL DO.MESSAGE("PSB21 *** LOG EVENT: " + STR$(REPORT.EVENT.NO%) , FALSE)
    ENDIF                                                                                 !1.1CSk

END SUB



\*******************************************************************************
\***
\*** UPDATE.BCF
\***
\***    Updates the BCF record  1 (Old IUF Format - Serial No.) or
\***                           20 (New IUF Format - Date Timestamp).
\***
\*******************************************************************************

SUB UPDATE.BCF PUBLIC

    MODULE.NUMBER$ = "5"                                                        !1.2CSk

    IF JOBSOK.FLAG$ <> "X" THEN BEGIN

        IF IUF.EXISTS THEN BEGIN

            FILE.OPERATION$ = "O"                                    ! 1.5 CSk
            CURRENT.REPORT.NUM% = BCF.REPORT.NUM%                    ! 1.5 CSk
            IF END # BCF.SESS.NUM% THEN UPDATE.BCF.ERROR             ! 1.5 CSk
            OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM%      ! 1.5 CSk

            IF IUF.NEW.FORMAT THEN BEGIN

                BCF.REC.NO% = 20

                RC% = READ.BCF.LOCK
                IF RC% <> 0 THEN BEGIN
                    CALL MODULE.5.LOG.EVENT(106)
                ENDIF

                BCF.IUF.DATETIMESTAMP$ = IUF.TIME.STAMP$
                BCF.FILLER$            = STRING$(63, " ")

                RC% = WRITE.BCF.UNLOCK
                IF RC% <> 0 THEN BEGIN
                    CALL MODULE.5.LOG.EVENT(106)
                ENDIF

            ENDIF ELSE BEGIN

                BCF.REC.NO% = 1

                RC% = READ.BCF.LOCK
                IF RC% <> 0 THEN BEGIN
                    CALL MODULE.5.LOG.EVENT(106)
                ENDIF
                BCF.IUF.SERIAL.NO$ = IUF.SERIAL.NO$

                RC% = WRITE.BCF.UNLOCK
                IF RC% <> 0 THEN BEGIN
                    CALL MODULE.5.LOG.EVENT(106)
                ENDIF

            ENDIF

            CLOSE BCF.SESS.NUM%                                      ! 1.5 CSk
        ENDIF

    ENDIF

    CALL DO.MESSAGE("PSB21 - BCF Rec  1 Serial No         : [" + BCF.IUF.SERIAL.NO$ + "]", FALSE)
    CALL DO.MESSAGE("PSB21 - BCF Rec 14 ECC Date Timestamp: [" + BCF.FILLER.DATETIMESTAMP$ + "]", FALSE)!AMW
    CALL DO.MESSAGE("PSB21 - BCF Rec 20 Batch Timestamp   : [" + BCF.IUF.DATETIMESTAMP$ + "]", FALSE)

    EXIT SUB                                                         ! 1.5 CSk

UPDATE.BCF.ERROR:                                                    ! 1.5 CSk

    CALL MODULE.5.LOG.EVENT(106)                                     ! 1.5 CSk

END SUB


\*******************************************************************************
\***
\*** UPDATE.JOBSOK
\***
\***    Updates PSB21 status flag in JOBOK file.
\***
\*******************************************************************************

SUB UPDATE.JOBSOK

    IF JOBSOK.FLAG$ = " " THEN BEGIN
       JOBSOK.FLAG$ = "E"
    ENDIF

    RC% = READ.JOBSOK
    IF RC% <> 0 THEN BEGIN
        CALL MODULE.5.LOG.EVENT(106)
    ENDIF

    JOBSOK.PSB21$ = JOBSOK.FLAG$

    IF JOBSOK.PSB21$ <> "X" THEN BEGIN

        IF IUF.EXISTS THEN BEGIN

            IF IUF.NEW.FORMAT THEN BEGIN
                JOBSOK.IUF.SOURCE$ = "E" ! SAP
                JOBSOK.LAST.PROCESSED.BATCH$ = IUF.TIME.STAMP$
            ENDIF ELSE BEGIN
                JOBSOK.IUF.SOURCE$ = "M" ! Mainframe
                JOBSOK.LAST.PROCESSED.BATCH$ = STRING$(17, " ")
            ENDIF
        ENDIF ELSE BEGIN
            JOBSOK.IUF.SOURCE$ = " " ! No IUF present
            ! Set the last successfully processed batch on the                 !1.2CSk
            ! JOBOK to the last batch processed from the BCF.                  !1.2CSk
            JOBSOK.LAST.PROCESSED.BATCH$ = BCF.IUF.DATETIMESTAMP$              !1.2CSk
        ENDIF

    ENDIF

    RC% = WRITE.JOBSOK
    IF RC% <> 0 THEN BEGIN
        CALL MODULE.5.LOG.EVENT(106)
    ENDIF

END SUB


\*******************************************************************************
\***
\***    DECAPI.MSG function
\***    Initialises the DEC API, writes a message to it, then closes it.
\***    Writes appropriate error messages to the application event log if needed.
\***
\*******************************************************************************

FUNCTION DECAPI.MSG(PAYLOAD.MSG$) PUBLIC

    INTEGER*2 DECAPI.MSG
    INTEGER*2 DECAPI.MSG.INTERNAL.RC%
    STRING PAYLOAD.MSG$

    DECAPI.MSG = 0 ! Set to non-zero by failed calls to DEC API functions

    ! Initialise the DEC API
    DECAPI.MSG.INTERNAL.RC% = DECAPI.INIT

    IF DECAPI.MSG.INTERNAL.RC% > 2 THEN BEGIN
        DECAPI.MSG = DECAPI.MSG.INTERNAL.RC%
        EVENT.222.DATA$ = "IN"                        + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,8)) + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,0)) + \
               PACK$("0000000000000000")
        CALL MODULE.5.LOG.EVENT(222)
        EXIT FUNCTION
    ENDIF

    ! DEC.MESSAGE.ID$ = "Item Update"
    ! DEC.PAYLOAD$    =  Content of PAYLOAD.MSG$
    ! DEC.COMMIT$     = "C" ! PSB21 payload will not need to be split
    ! DECAPI.MAX      =  0  ! Zero by default
    !                       ! PSB21 payloads will not be very big

    ! Send the payload message to the DEC API
    DECAPI.MSG.INTERNAL.RC% = DECAPI.SEND("Item Update", PAYLOAD.MSG$, "C")

    IF DECAPI.MSG.INTERNAL.RC% <> 0 THEN BEGIN
        DECAPI.MSG = DECAPI.MSG.INTERNAL.RC%
        EVENT.222.DATA$ = "SN"                        + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,8)) + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,0)) + \
               "CID" + "C" + PACK$("0000")
        CALL MODULE.5.LOG.EVENT(222)
    ENDIF

    ! Close the DEC API
    DECAPI.MSG.INTERNAL.RC% = DECAPI.CLOSE

    IF DECAPI.MSG.INTERNAL.RC% <> 0 THEN BEGIN
        DECAPI.MSG = DECAPI.MSG.INTERNAL.RC%
        EVENT.222.DATA$ = "CL"                        + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,8)) + \
               CHR$(SHIFT(DECAPI.MSG.INTERNAL.RC%,0)) + \
               PACK$("000000000000")
        CALL MODULE.5.LOG.EVENT(222)
    ENDIF

END FUNCTION



\*******************************************************************************
\***
\*** SEND.DEC.MESSAGE
\***
\***    Sends CSV formatted message to DEC if new format IUF was processed.
\***
\*******************************************************************************

SUB SEND.DEC.MESSAGE PUBLIC

    MODULE.NUMBER$ = "5"                                                        !1.2CSk

    IF IUF.NEW.FORMAT THEN BEGIN

        IF JOBSOK.FLAG$ <> "X" THEN BEGIN
            SUCCESS$ = "Y"
        ENDIF ELSE BEGIN
            SUCCESS$ = "N"
            IF IUF.INITIAL.LOAD$ = "Y" THEN BEGIN
                ERROR$ = "4"            ! Initial load issue
            ENDIF
        ENDIF

        DEC.MESSAGE$ =                      + \
                "IF_00022,IUF,"             + \  ! Interface Identifier
                STORE.NUMBER$         + "," + \
                IUF.TIME.STAMP$       + "," + \  ! YYYYMMDDHHMMSSsss
                "20" + DATE$ + TIME$  + "," + \  ! YYYYMMDDHHMMSS
                SUCCESS$              + "," + \  ! "Y" - File processed at store (so may be deleted from SAP) ! Batch processed at...
                                            + \  ! "N" - File not processed (so retain in SAP for re-transmission) !Batch not process ...
                ERROR$                           ! "0"  No error
                                                 ! "1"  Item Update File missing
                                                 ! "2"  Header problem
                                                 ! "3"  Trailer problem
                                                 ! "4"  Initial load issue
                                                 ! "5"  Batch already processed
                                                 ! "6"  -- Not defined --
                                                 ! "7"  -- Not defined --
                                                 ! "8"  -- Not defined --
                                                 ! "9"  Unexpected error

        CALL DO.MESSAGE("PSB21 DEC MSG: " + DEC.MESSAGE$, FALSE)
        CALL DO.MESSAGE("PSB21 Sending Message to DEC API", FALSE)

        CALL DECAPI.MSG(DEC.MESSAGE$)

        ERROR$ = "0"                             ! Reset error status

    ENDIF

END SUB



\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLOSE.FILES                                   *
\***                                                                          *
\******************************************************************************

SUB CLOSE.FILES

    CALL DO.MESSAGE("PSB21 Closing Files", TRUE)
    CLOSE BCF.SESS.NUM%

    IF CIPPM.PRESENT THEN BEGIN                                 ! 1.6 RC (226)
        CLOSE CIPPM.SESS.NUM%
    ENDIF                                                       ! 1.6 RC (226)

    CLOSE DRUG.SESS.NUM%
    CLOSE IDF.SESS.NUM%
    CLOSE IEF.SESS.NUM%
    CLOSE IEX.SESS.NUM%
    CLOSE IRF.SESS.NUM%
    CLOSE IRFDEX.SESS.NUM%                                      ! 1.9 RC (242)
    CLOSE ISF.SESS.NUM%
    CLOSE JOBSOK.SESS.NUM%
    CLOSE LOCAL.SESS.NUM%
    CLOSE NEWLINES.SESS.NUM%
    CLOSE RICF.SESS.NUM%
    CLOSE STOCK.SESS.NUM%

    !Close IRFITGRP File                                                !BRN
    IF IRFITGRP.OPEN THEN BEGIN                                         !BRN
        CLOSE IRFITGRP.SESS.NUM%                                        !BRN
        IRFITGRP.OPEN = FALSE                                           !BRN
    ENDIF                                                               !BRN

    CALL CLOSE.IRF.UPDT (NEW.IRF.DATA$,ACD.FLAG$)

END SUB


\*******************************************************************************
\***
\*** PROGRAM.EXIT
\***
\***
\***   Update JOBSOK, send message to DEC API and chain to PSB20 passing
\***   processing date.
\***
\*******************************************************************************
SUB PROGRAM.EXIT PUBLIC

    MODULE.NUMBER$ = "5"                                                        !1.2CSk

!   If PROGRAM.EXIT has been called unexpectedley ensure the PPFK is deleted    ! 1.4 RC (190)
    IF JOBSOK.FLAG$ <> " " THEN BEGIN                                           ! 1.7 RC (238)

        CALL DO.MESSAGE("PSB21 PROGRAM.EXIT (JOBSOK """ + JOBSOK.FLAG$ \        ! 1.7 RC (238)
                                                        + """)", FALSE)         ! 1.7 RC (238)

        CALL DO.MESSAGE("PSB21 Deleting files from RAM disk W:", FALSE)         ! 1.8 RC (240)

!       Delete any RAM files used by the program that are still present         ! 1.7 RC (238)
        CALL OSSHELL("IF EXIST W:\%*.*        RESTRICT W:\%*.*")                ! 1.8 RC (240)
        CALL OSSHELL("IF EXIST W:\%*.*        DEL W:\%*.*")                     ! 1.7 RC (238)

        CALL OSSHELL("IF EXIST W:\PPFK.BIN RESTRICT W:\PPFK.BIN"   )            ! 1.8 RC (240)
        CALL OSSHELL("IF EXIST W:\PPFK.BIN      DEL W:\PPFK.BIN"   )            ! 1.8 RC (240)

    ENDIF                                                                       ! 1.4 RC (190)

    ! Moved from PSB2100.BAS
    CALL BACKUP.IUF.FILE ! Writes "PHASE 6"                     ! 1.4 RC

    CALL UPDATE.JOBSOK

    CALL DO.MESSAGE("PSB21 JOBSOK.FLAG$ = " + JOBSOK.FLAG$, FALSE)

    CALL CLOSE.FILES

    IF IUF.NEW.FORMAT \ ! Not legacy IUF                        ! 1.5 RC (223)
      AND IUF.J103.COUNT% > 0 THEN BEGIN                        ! 1.5 RC (223)
        CALL MODULE.5.LOG.EVENT(103)
    ENDIF

    CALL DO.MESSAGE("PSB21 Chaining to PSB20", TRUE)
    CALL DO.MESSAGE("PSB21 EXITED PROGRAM", TRUE)
    CALL DO.MESSAGE("==================================================================", FALSE)

    CHAIN PSB20.PATH.NAME$,PROCESSING.DATE$

END SUB

