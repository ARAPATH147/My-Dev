\*******************************************************************************
\*******************************************************************************
\***
\***
\***            PROGRAM         :       PSB21
\***            MODULE          :       PSB2100
\***            AUTHOR          :       Charles Skadorwa / Mark Walker / Mark Goode
\***            DATE WRITTEN    :       Sept 2011
\***
\*******************************************************************************
\***
\***   VERSION 1.0        Charles Skadorwa/Mark Walker/Mark Goode  30 SEP 2011
\***   Initial version.
\***
\***   VERSION 1.1            Mark Goode                        22 December 2011
\***   Re-introduce date validation process for RPD date field.
\***
\***   VERSION 1.2            Charles Skadorwa                  16 January 2012
\***   Defect 3333: PSB21 log file shows misleading message.
\***                Text change "PSB21 PGF PROCESSING" changed to
\***                            "PSB21 IUF PROCESSING".
\***
\***   VERSION 1.3            Charles Skadorwa                  20 January 2012
\***   Defect 2781: PSB21 sends 2 DEC messages (success & failure) when invalid
\***                trailer count found. Trailer processing modified.
\***
\***   Defect    8: Confusing duplicate error messages. IUF Pre-processing ERROR4
\***                & ERROR6 changed to ERROR10 & ERROR11 respectively.
\***
\***   Defect   10: Fix to Price record date checking.
\***
\***    VERSION 1.4.                ROBERT COWEY.                25 JAN 2012.
\***    Incremented version text to 1.4.
\***    Reduced text to fit 46 characters of background display but excluded
\***    version date to avoid any confusion with run date.
\***    Opened/Created log file UNLOCKED to force physical write for each
\***    logical write in order to capture log data when program abends.
\***    (The extra overhead will be very small for normal program runs).
\***
\***    Defect 3569 (b).
\***    Changed NIADF report number to 834 (as 830 in use by DEALX2).
\***
\***    Also ...
\***    Set JOBSOK.FLAG$ to "X" prior to calling PROGRAM.EXIT due to invalid
\***    IUF header.
\***    Improved log messages when no IUF backup made.
\***
\***    VERSION 1.5.                ROBERT COWEY.                02 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.5.
\***    Incremented version text to 1.5.
\***
\***    Defect 2678
\***    Modified PPFK functions to prevent unwanted retention of superceded
\***    price changes.
\***
\***    Defect 21 - Commented 1.5 RC (21).                       07 FEB 2012.
\***    Original fix (and associated comments) were incorrect as they were    ! 1.6 RC (21)
\***    based on a mis-understanding of variable IUF.DISCOUNTABLE$.           ! 1.6 RC (21)
\***    The fix has now been re-coded (commented 1.6 RC (21) ) and any        ! 1.6 RC (21)
\***    potentially confusing comments also corrected or removed.             ! 1.6 RC (21)
\***
\***    VERSION 1.6            Charles Skadorwa                  14 FEB 2012
\***    Various defects following WPI.
\***
\***    VERSION 1.6 RC.        ROBERT COWEY.                     21 FEB 2012.
\***    Continued Charlies version 1.6 to retain this as next lab release.
\***
\***    Defect 21 - Commented 1.6 RC (21).
\***    Reinstated original variable name CURR.DISCOUNTABLE$.
\***
\***    Defect 50 - Commented 1.6 RC (50).                       22 FEB 2012.
\***    Shortened message text to fit 46 character background display.
\***    Also set initial DO.MESSAGE("PSB21 PROGRAM START ..." to TRUE to
\***    re-instate its display on background screen.
\***
\***    Also ... Commented 1.6 RC  Matrix nn                     24 FEB 2012.
\***    Reported IUF item code in addition to IUF record number when
\***    VALIDATE.IUF.FIELDS routine rejects a particular IUF matrix entry
\***    and skips the record.
\***
\***    Defect 87 - Commented 1.6 RC (87)                        24 FEB 2012.
\***    Defined IUF.OPEN flag to keep track of IUF file open status.
\***    Used this to prevent attempted backup if IUF still open as occurs
\***    when ERROR.DETECTED calls PROGRAM.EXIT calls BACKUP.IUF.FILE.
\***
\***    Defect 71 - Commented 1.6 RC (71)                        24 FEB 2012.
\***    For Core R2 IUF ...
\***    Removed code rejecting items with invalid IUF.UNRESTRICTED.GROUP.CODE$
\***    as these are now intercepted (and defaulted to "N") within READ.IUF.
\***
\***    Defect 71 - Reversed out - Commented 1.6 RC (71b)        27 FEB 2012.
\***    The previous fix correctly follows Detailed Design 1.6 however the
\***    DD refers to setting of IRF flags from valid IUF data ("N" or "Y").
\***    The project level documents however require that invalid IUF data
\***    for this variable causes the item to be rejected.
\***
\***    Modified VALIDATE.IUF.FIELDS to remove the MATCH statement from this
\***    variables original validation code (as it did not trap null values).
\***
\***    Defect 90 - Commented 1.6 RC (90)                        27 FEB 2012.
\***    Modified VALIDATE.IUF.FIELDS to retain IUF.RPD.DATE$ as 8 digits
\***    until loaded into CURR.DATE.DUE$ table.
\***    Modified STORE.PRICE.CHANGE to convert 8-digit ASCII IUF.RPD.DATE$
\***    to 6-digits (3 bytes packed) when passed to CURR.DATE.DUE$.
\***    This then passes to PPFK.DATE.DUE$ and is then written to the PPFK
\***    temporary file via a WRITE FORM (defining it as 3 bytes packed).
\***    (The PPFK is intimately associated with 6-digit PPFI RPD dates).
\***
\***    VERSION 1.7.                ROBERT COWEY.                28 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.7.
\***    Incremented version text to 1.7.
\***    Set JOBOK to "X" when legacy IUF is processed after a Core R2 IUF.
\***
\***    VERSION 1.8.                ROBERT COWEY.                08 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.8.
\***    Incremented lab release version text to V1.8.
\***
\***    Defect 8 - Commented 1.8 RC (8)                          12 MAR 2012.
\***    Defined flag IUF.OPEN.ERROR.REPORTED and used it to ensure no more
\***    than one IUF open error is ever reported to the PSB21day.log file.
\***
\***    VERSION 1.9.                ROBERT COWEY.                13 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.9.
\***    Incremented lab release version text to v1.9.
\***
\***    Defect 109 - Commented 1.9 RC (109)                      19 MAR 2012.
\***    Rejected IUF emergency 99999 RPD's as these are now generated
\***    internally within the program.
\***
\***    VERSION 1.10.               ROBERT COWEY.                21 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.10.
\***    Incremented lab release version text to v1.10.
\***
\***    Defect 113 - Commented 1.10 RC (113)
\***    Defined SORT.TABLE$ for use in sorting CURR data tables (from IUF)
\***    and PPFK data tables (from PPFI) into descending date-RPD order.
\***
\***    Wrote STORE.EMERG.PRICE.CHANGE(N%) based on STORE.PRICE.CHANGE(N%)
\***    to place emergency 99999 RPD onto (IUF) CURR data table
\***    Wrote CHECK.IF.CURR.SORT.NEEDED(M%) to be called by both of these
\***    to set CURR.SORT.NEEDED flag TRUE when underlying data (from IUF)
\***    is not in sequence.
\***    Wrote SORT.CURR.TABLES to sort CURR data tables when needed
\***    (and reset CURR.SORT.NEEDED to FALSE).
\***    Modified STORE.PRICE.CHANGE precedure to set PRICE.CHANGE.TODAY flag
\***    TRUE (for use in preventing an un-necessary emergency 99999 RPD)
\***    when one of the price changes is for the current day.
\***
\***    VERSION 1.11.               ROBERT COWEY.                05 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.11.
\***    Incremented lab release version text to v1.11.
\***
\***    Defect 169 - Commented 1.11 RC (169)
\***    Defined subprogram IUF.END.BATCH.PROCESSES to group together the
\***    tasks performed when an IUF trailer record is encountered after
\***    processing a batch of IUF data.
\***    Also called this when an IUF trailer record is encountered after
\***    processing an empty IUF batch (header and trailer only) provided
\***    it has not already been executed from the main loop.
\***
\***    VERSION 1.12.               ROBERT COWEY.                17 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.12.
\***    Incremented lab release version text to v1.12.
\***
\***    Defect 177 - Commented 1.12 RC (177)
\***    Correction to place RPD data on the output PPFI in ascending date
\***    order (latest last) as needed by PSB72 Effect Price Change program.
\***    Associated changes to programs internal sort of PPFI and IUF data.
\***
\***    VERSION 1.13.               ROBERT COWEY.                23 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.13.
\***    Incremented lab release version text to v1.13.
\***
\***    Defect 179 - Commented 1.13 RC (179)
\***    Moved setting of RICF.RECORD.NO% from CREATE.RICF to UPDATE.RICF.
\***
\***    Defect 190 - Commented 1.13 RC (190)                     24 APR 2012.
\***    Moved DELETE.PPFK.KEYED.FILE into PROGRAM.EXIT to ensure PPFK is
\***    always deleted even when program does not complete successfully.
\***
\***    VERSION 1.14.               ROBERT COWEY.                26 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.14.
\***    Incremented lab release version text to v1.14.
\***
\***    Defect 190 corrrection - Commented 1.14 RC (190)         30 APR 2012.
\***    Re-instated DELETE.PPFK.KEYED.FILE prior to PROCESS.NIADF.FILE to
\***    ensure PPFK deleted prior to requiring RAM disk for NIADF.
\***    PPFK is now only deleted from PROGRAM.EXIT itself (by DESTROY.PPFK)
\***    when JOBSOK.FLAG$ is "X" (since this is set prior to all unexpected
\***    calls to PROGRAM.EXIT).
\***
\***    VERSION 1.15.               ROBERT COWEY.                01 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.15.
\***    Incremented lab release version text to v1.15.
\***
\***    Defect 200 - See TIFFUN.BAS
\***    Modified READ.TIF function to bypass file read for null TIF.BARCODE$.
\***
\***    Defect 223 - Commented 1.15 RC (223)                     04 MAY 2012.
\***    Modified UNEXPECTED.RECORD.TYPE routine to exit the program with
\***    JOBSOK.FLAG$ "X" for unexpected legacy IUF record type.
\***
\***    VERSION 1.15 supplemental   CHARLES SKADORWA.            04 MAY 2012.
\***    Modified ERROR.DETECTED to report SU subscript (table overflow) error
\***    to PSB21day.LOG file.
\***    Ensured BCF is only opened when needed (then closed afterwards).
\***
\***    VERSION 1.16.               ROBERT COWEY.                09 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.16.
\***    Incremented lab release version text to v1.16.
\***
\***    Defect 226 - Commented 1.16 RC (226)
\***    Modified OPEN.FILES to cater for stores not having a CIPPMR and set
\***    CIPPM.PRESENT flag appropriately to indicate this.
\***
\***    VERSION 1.17.               ROBERT COWEY.                16 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.17.
\***    Incremented lab release version text to v1.17.
\***
\***    Defect 230 - Commented 1.17 RC (230)
\***    Modified BACKUP.IUF to ensure IUF is renamed to IUFBK.day
\***    (via ADXCOPYF then delete of IUF) only when JOBSOK.FLAG$ indicates
\***    PSB21 has been successful or IUF does not need to be reprocessed.
\***    Modified IUF.END.BATCH.PROCESSES to update BCF (and send associated
\***    DEC message) only after IUF trailer count validated.
\***
\***    VERSION 1.18.               ROBERT COWEY.                22 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.18.
\***    Incremented lab release version text to v1.18.
\***
\***    Pilot defect - Commented 1.18 RC (237)
\***    Modified DETERMINE.LOG.FILE routine to set log file name correctly.
\***
\***    Pilot defect - Commented 1.18 RC (239)
\***    Set PROCESS.NIADF flag FALSE to temporarily prevent processing of
\***    NIADF until defect 239 is investigated and resolved.
\***
\***    VERSION 1.19.               ROBERT COWEY.                24 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.19.
\***    Incremented lab release version text to v1.19.
\***    Nullified previous change to reinstate NIADF processing.
\***
\***    Pilot defect - Commented 1.19 RC (238)
\***    Defined F18.SOPTS.READ flag as global in modules PSB2100 and PSB2104.
\***    Called PSBF18 CALC.BOOTS.CODE.CHECK.DIGIT from INITIALISATION to
\***    perform function-internal read of SOPTS record 96 at program start.
\***    This avoids session number contention problems causing ERRN 00000014.
\***
\***    VERSION 1.20.               ROBERT COWEY.                28 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.20.
\***    Incremented lab release version text to v1.20.
\***
\***    Pilot defect - Commented 1.20 RC (240)
\***    Defined procedure CHECK.PROCESS.PGF to check for and handle errors
\***    from PROCESS.PGF.
\***
\***    VERSION 1.21.               ROBERT COWEY.                11 JUN 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.21.
\***    Incremented lab release version text to v1.21.
\***
\***    Lab test defect - Commented 1.21 RC (242)
\***    Corrected NIADF processing to ensure IRFDEX updated when new item is
\***    on more than three deals.
\***    Allocated IRFDEX session number and opened and closed the file.
\***
\***    VERSION 1.22                CHARLES SKADORWA.            20 JUN 2012.
\***    Defect 243 - Commented 1.22 CS (243)
\***    Changes creating PSB21.286 Core Release 2 version 1.22.
\***    Incremented lab release version text to v1.22.
\***
\***    VERSION A                   CHARLES SKADORWA.             5 SEP 2013.
\***    F261 Gift Card Mall IIN Range Extension Project - Commented ! ACSk
\***    Incremented lab release version text to vA.
\***
\***    VERSION B                   CHARLES SKADORWA.            18 FEB 2014.
\***    F261 Gift Card Mall IIN Range Extension Project - Commented ! BCSk
\***    Defect 495 - GCM: PSB21: 'Not Returnable' flag value is getting
\***                              set to 1 for a non GCM item.
\***    The IUF.RETURNABLE$ flag (initialised to "Y") gets set in the READ.IUF
\***    function (IUFFUN.BAS) according to the value set against the Product
\***    Group for the item (that's read into an internal table [REFPGF.RECORDS$]
\***    from the REFPGF file). This value is used to determine the Return
\***    Route. If the value is not set ie. zero, then the IUF.RETURNABLE$ flag
\***    is set to "N". The current logic in UPDATE.IRF only UNsets the
\***    Returnable bit flag (IRF.INDICAT0% Bit 3) if it is a new item (not on
\***    IDF),otherwise the item retains its original setting.
\***
\***    My mistake was thinking that the IUF.RETURNABLE$ flag related to this
\***    bit flag, however, it is used to set IRF.INDICAT10% Bit 6.
\***
\***    The fix is to set IRF.INDICAT0% Bit 3 directly in UPDATE.IRF if the
\***    item is in one of the specified GCM product groups read from the BCF.
\***    Release version left at vA as not released but date changed to
\***    18-02-2014.
\***
\***    VERSION C                   CHARLES SKADORWA.            25 FEB 2014.
\***    F261 Gift Card Mall IIN Range Extension Project - Commented ! CCSk
\***    Defect 517 - GCM: PSB21: 'Not Returnable' flag value is NOT
\***                              getting set to 1 for a new GCM item.
\***    Change to UPDATE.IRF in PSB2101.BAS to reference CURR.PROD.GRP$
\***    instead of IUF.PROD.GRP$.
\***    Release version left at vA as not released but date changed to
\***    25-02-2014.
\***
\***    VERSION D                   Mark Walker                 4th Mar 2014
\***    F337 Centralised View of Stock
\***    Incremented lab release version text to vD.
\***
\***    VERSION E                   Mark Walker                 6th May 2014
\***    F337 Centralised View of Stock
\***    Incremented lab release version text to vE.
\***
\***    VERSION F                   Mark Walker                23rd May 2014
\***    F337 Centralised View of Stock
\***    Incremented lab release version text to vF.
\***
\***    VERSION G                   Mark Walker                 8th Jul 2014
\***    F353 Deal Limits Increase
\***    Removed DINF and NIADF file processing.
\***    Removed module 4 from program overview (uncommented).
\***    Incremented lab release version text to vG.
\***
\***    VERSION H                   Rejiya Nair            13th May 2016
\***    PRJ1452 Restricting Item Sales
\***    - Add/Update the new keyed file; IRFITGRP if the IRF file is set
\***      for the bit "enforced quantity entry" (bit 7) in IRF.INDICAT0.
\***    - Delete the item record from IRFITGRP if bit 7 in IRF.INDICAT0
\***      is reset.
\***    - Removed the previously tagged commented out code.
\***
\***    VERSION I                   Ranjith Gopalankutty        4th Nov 2016
\***    INV10004090 - Fix for duplicate entry issue in PPFI
\***    Fixed the 'For' loop breaking condition in Function 
\***    PROCESS.BTREE.RECORD$ within submodule PSB2103.BAS,
\***    Fix will ensure duplicate entries issue in PPFI is
\***    not repeated and hence CIPOK also works without 
\***    problems.
\***    Incremented the version of PSB21 to vI
\***----------------------------------------------------------------------------
\***  IMPORTANT: When you change PSB21, please search for the line containing
\***------------        CALL DO.MESSAGE("PSB21 PROGRAM START
\***             in Module PSB2100 and wind the date and version on.
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\*******************************************************************************
\***
\***    Program Overview
\***    ----------------
\***
\***    This is a multi-modular program which Processes the Item Update File that
\***    is sent from the Boots Central Systems and prepares the data ready for the
\***    next program in the item Update Suite. This program has been rewritten as
\***    part of the CORE Heritage Stores Release 2 Outbound project.
\***
\***
\***    Started By: PSB20 Item Update Suite Control Program
\***
\***    Calls     : none
\***
\***    Chains to : PSB20
\***
\***
\***
\***    MODULE 0:  Main Processing Control
\***               This is the main control module. It performs the initialisation
\***               and contains the main processing loop that is used to process the
\***               IUF. Nb. this program handles both IUF's in the Old format and the
\***               new CSV format. The format is hidden by the IUF function calls ie.
\***               the old fomat records are converted/transalated into the new format.
\***               Old format IUF's contain only one batch whereas new IUF's can contain
\***               multiple batches - a pre-processing stage ensures that duplicate items
\***               that appear in earlier batches are skipped.
\***
\***               An item is not processed until all its associated records have been read
\***               in and validated. Invalid items are skipped.
\***
\***               A comprehensive daily log is maintained of all processing activity in
\***               D:\ADX_UDT1\PSB21MON.LOG through to D:\ADX_UDT1\PSB21SUN.LOG.
\***
\***
\***    MODULE 1:  IUF Processing
\***               This module updates all the associated data files for an item.
\***
\***
\***    MODULE 2:  PGFDIR Processing
\***               The purpose of this module is to remove old product groups from the PGF.
\***               It achieves this by reading the existing PGF into a table and reading
\***               through the PGFDIR. All PGFDIR records are written to a work file,
\***               however, its flags are updated if the product group exists on the PGF.
\***               At the end of processing, the PGF is replaced by the work file.
\***
\***
\***    MODULE 3:  Price Change (PPFI) Processing
\***               The purpose of this module is to perform price change processing.
\***               It updates the PPFI with the current price changes in the IUF.
\***
\***
\***    MODULE 5:  Program Exit Processing
\***               Update JOBSOK, send message to DEC API and chain to PSB20 passing
\***               processing date.
\***
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\***
\***    Included code defining file related global variables
\***
\***............................................................................
%INCLUDE BCFDEC.J86     ! Boots Control File
%INCLUDE CIPPMDEC.J86   ! Markdown File
%INCLUDE DRUGDEC.J86    ! Drug File
%INCLUDE IDFDEC.J86     ! Item Data File
%INCLUDE IEFDEC.J86     ! Item EAN File
%INCLUDE IEXDEC.J86     ! Item Extension File
%INCLUDE IRFDEC.J86     ! Item Reference File
%INCLUDE ITGRPDEC.J86   ! IRF Attribute Extension File                  !HRN
%INCLUDE ISFDEC.J86     ! Item Shelf Edge Label Description File
%INCLUDE IUFDEC.J86     ! Item Update File
%INCLUDE JOBOKDEC.J86   ! Jobs OK File
%INCLUDE LOCALDEC.J86   ! Local Price File
%INCLUDE NLINEDEC.J86   ! New Lines File
%INCLUDE PGFDEC.J86     ! Product Group Description File
%INCLUDE PPFDEC.J86     ! Pending Price File
%INCLUDE PPFKDEC.J86    ! Pending Price Keyed Work File
%INCLUDE RICFDEC.J86    ! Redeem Items Change File
%INCLUDE SOFTSDEC.J86   ! Software Status File
%INCLUDE STKMQDEC.J86   ! Stock Movement Queue File
%INCLUDE STOCKDEC.J86   ! Stock File
%INCLUDE SUPPDEC.J86    ! Supplier File
%INCLUDE WEEEDEC.J86    ! Waste Electrical & Electronic Equipment Update File


\*******************************************************************************
\***
\***    Included code defining function related global variables
\***
\***............................................................................
%INCLUDE PSBF01G.J86    ! APPLICATION.LOG
%INCLUDE PSBF02G.J86    ! UPDATE.DATE                                ! 1.18 RC
%INCLUDE PSBF13G.J86    ! PSDATE
%INCLUDE PSBF18G.J86    ! CALCULATE BOOTS CHECK DIGIT          ! 1.19 RC (238)
%INCLUDE PSBF19G.J86    ! UPDATE IRF GLOBALS
%INCLUDE PSBF20G.J86    ! Sess Num Utility
%INCLUDE PSBUSEG.J86    ! Chain parameters


\*******************************************************************************
\***    PSB2100 variables
\***............................................................................
    INTEGER*1 GLOBAL                        \
        CIPPM.PRESENT,                      \                         ! 1.16 RC (226)
        CURR.SORT.NEEDED,                   \                         ! 1.10 RC (113)
        END.OF.IUF.FILE,                    \
        FALSE,                              \
        FILE.TIME.AFTER.6PM,                \                         ! 1.18 RC
        FOUND,                              \
        ITEM.PRE.PROCESSING.CHECK,          \
        IUF.EXISTS,                         \
        IUF.OPEN,                           \                         ! 1.6 RC (87)
        IUF.OPEN.ERROR.REPORTED,            \                         ! 1.8 RC (8)
        PRICE.CHANGE.TODAY,                 \                         ! 1.10 RC (113)
        PROCESS.ITEM,                       \
        SKIP.IUF.PROCESSING,                \                                   !1.6CSk
        TRUE                                !

    INTEGER*2 GLOBAL                        \
        BARCODE.COUNT%,                     \
        CURR.PCR.COUNT%,                    \
        CURR.SESS.NUM%,                     \
        CURRENT.REPORT.NUM%,                \
        ERROR.COUNT%,                       \
        F20.INTEGER%,                       \
        IUF.BACKUP.SESS.NUM%,               \
        IUF.BATCH.COUNT%,                   \
        MAX.BARCODES%,                      \
        MAX.PRICE.CHANGES%,                 \
        MESSAGE.NO%,                        \
        NO.OF.CHLAMYDIA%,                   \                                   !1.6CSk
        NO.OF.STATINS%,                     \                                   !1.6CSk
        PREV.PCR.COUNT%,                    \
        STATUS%,                            \                                   !1.6CSk
        WEEE.ITEM.LIMIT%                    !

    INTEGER*4 GLOBAL                        \
        ADXSERVE.RET.CODE%,                 \
        ADXSERVE.RETURN.CODE%,              \
        COUNT.ACTUAL%,                      \
        COUNT.EXPECTED%,                    \
        COUNT.RECORDS.IUF%,                 \
        DIR.SESS.NUM%,                      \
        IUF.BATCH.ITEM.TOTAL%,              \
        IUF.BUFF.SIZE%,                     \
        IUF.ITEM.BATCH.LIMIT%,              \
        IUF.ITEM.SKIP%,                     \
        IUF.J103.COUNT%,                    \
        IUF.RECORD.COUNT%,                  \
        LOG.BUFF.SIZE%,                     \
        LOG.SESS.NUM%,                      \
        NO.OF.BARCODE.RECS%,                \
        NO.OF.NEW.PPF.ITEMS%,               \
        NO.OF.OLD.PPF.ITEMS%,               \
        NO.OF.OUT.PPF.ITEMS%,               \
        NO.OF.PRICE.CHANGE.RECS%,           \
        PERCENT.VAL%,                       \
        PPFI.BUFF.SIZE%,                    \
        PPFO.BUFF.SIZE%,                    \
        PROGRESS.COUNT%,                    \
        RC%,                                \
        RECS.PER.5%,                        \
        TOTAL.PPF.RECS%,                    \
        WEEE.ITEM.COUNT%,                   \
        WEEEUF.BUFF.SIZE%                   !

    STRING GLOBAL                           \
        BARCODE.TABLE$(1),                  \
        CHLAMYDIA.ID$(1),                   \                                   !1.6CSk
        STATINS$(1),                        \                                   !1.6CSk
        WEEEUF.TABLE$(1)                    !

    STRING GLOBAL                           \
        ACD.FLAG$,                          \
        APPLICATION.STATUS.INFO$,           \
        BACKUP.SOURCE.FILE$,                \
        CHLAM.TEMP$,                        \                                   !1.6CSk
        CHLAMYDIA.LIST$,                    \                                   !1.6CSk
        COMM.TAIL$,                         \
        COMPARE$,                           \
        CRLF$,                              \
        CT.PARAMBLK$,                       \
        CT.TIMEDATE.TABLE$,                 \
        DEC.MESSAGE$,                       \
        DIR.FILENAME$,                      \
        DIR.RECORD$,                        \
        ERROR$,                             \
        EVENT.222.DATA$,                    \
        EXP.REC.TYPE$,                      \
        F13.INPUT.DATE$,                    \
        F18.SOPTS.READ$,                    \                           ! 1.19 RC (238)
        F20.FUNCTION$,                      \
        F20.STRING$,                        \
        FILE.DATE$,                         \
        FILE.OPERATION$,                    \
        FORM$,                              \
        GCM.PG.LIST$,                       \  List of Gift Card mall   ! ACSk
                                            \  Product Groups read in   ! ACSk
                                            \  from BCF Rec: 22         ! ACSk
        IUF.BACKUP.FILENAME$,               \
        JOBSOK.FLAG$,                       \
        LOG.FILENAME$,                      \
        MODULE.NUMBER$,                     \
        MSG$,                               \
        PHASE$,                             \
        PINFO.TABLE$,                       \
        PREV.IUF.BOOTS.CODE$,               \
        PREV.REC.TYPE$,                     \
        PROCESSING.DATE$,                   \
        PROG.NAME$,                         \
        PSB20.PATH.NAME$,                   \
        RETURN.FLAG$,                       \
        RUN.DATE$,                          \
        RUN.TIME$,                          \
        SOFTS.REC.62$,                      \
        STATIN.INDICATOR$,                  \                                   !1.6CSk
        STKMQ.OPEN.FLAG$,                   \
        STORE.NUMBER$,                      \
        SUCCESS$,                           \
        SVC.PARAM$,                         \
        TODAY$,                             \
        YESTERDAY.DDMMYY$,                  \                                   ! 1.18 RC
        VAR.STRING.1$,                      \
        VAR.STRING.2$                       !

    STRING GLOBAL                           \
        CURR.AGE.RESTRICTION$,              \
        CURR.BC.LETTER$,                    \
        CURR.BLOCKED.FROM.SALE$,            \
        CURR.BOOTS.CODE$,                   \
        CURR.BOOTS.COM.EXTENDED$,           \
        CURR.CONTAINS.ALCOHOL$,             \
        CURR.CONTAINS.ASPIRIN$,             \
        CURR.CONTAINS.EPHEDRINE$,           \
        CURR.CONTAINS.IBUPROFEN$,           \
        CURR.CONTAINS.NONSOLID.PAINKILLER$, \
        CURR.CONTAINS.PARACETAMOL$,         \
        CURR.CONTAINS.PSEUDOEPHEDRINE$,     \
        CURR.CURRENT.PRICE$,                \
        CURR.DATE.SENSITIVE$,               \
        CURR.DISCOUNTABLE$,                 \ ! Reinstated original variable name ! 1.6 RC (21)
        CURR.EARN.POINTS$,                  \
        CURR.ENF.PRICE.ENTRY$,              \
        CURR.ETHICAL.ACTIVE$,               \
        CURR.ETHICAL.CLASS$,                \
        CURR.ETHICAL.DESCRIPTION$,          \
        CURR.ETHICAL.PACK.SIZE$,            \
        CURR.GIFTCARD$,                     \
        CURR.GIVEAWAY$,                     \
        CURR.GRP.CODE.FLAG$,                \
        CURR.GUARANTEE.LENGTH$,             \
        CURR.INSURANCE$,                    \
        CURR.ITEM.QTY$,                     \
        CURR.OWN.BRAND$,                    \
        CURR.PRIMARY.SUPPLIER$,             \
        CURR.PROD.GRP$,                     \
        CURR.REC.TYPE$,                     \
        CURR.REDEEMABLE$,                   \
        CURR.RESALEABLE$,                   \
        CURR.RETURN.ROUTE$,                 \
        CURR.RETURNABLE$,                   \
        CURR.S.E.DESC$,                     \
        CURR.SPECIAL.INSTRUCTION$,          \
        CURR.STATUS.1$,                     \
        CURR.STNDRD.DESC$,                  \
        CURR.STOCK.SYSTEM.FLAG$,            \
        CURR.SUPPLY.ROUTE$,                 \
        CURR.TILL.DESC$,                    \
        CURR.UNIT.MEASUREMENT$,             \
        CURR.UNIT.NAME$,                    \
        CURR.UNRESTRICTED.GROUP.CODE$       !

    STRING GLOBAL                           \
        CURR.DATE.DUE$(1),                  \
        CURR.INC.DEC.FLAG$(1),              \
        CURR.MARKDOWN$(1),                  \
        CURR.PRICE$(1),                     \
        CURR.RPD.NO$(1),                    \
        CURR.STATUS.FLAG$(1)                !

    STRING GLOBAL                           \
        PREV.DATE.DUE$(1),                  \
        PREV.INC.DEC.FLAG$(1),              \
        PREV.MARKDOWN$(1),                  \
        PREV.PRICE$(1),                     \
        PREV.RPD.NO$(1),                    \
        PREV.STATUS.FLAG$(1)                !

    STRING GLOBAL \                                            ! 1.10 RC (113)
        SORT.TABLE$(1)                                         ! 1.10 RC (113)

    STRING GLOBAL                           \                           !HRN
        CURR.GRP.NO$,                       \                           !HRN
        CURR.RESTRICT.SALES.FLAG$           !                           !HRN

\*******************************************************************************
\***
\***    External functions
\***
\***............................................................................
%INCLUDE ADXCOPY.J86    ! I.B.M. system subroutine for copying files
%INCLUDE ADXSERVE.J86   ! I.B.M. system function to obtain Controller Info
%INCLUDE BCFEXT.J86     ! Boots Control File
%INCLUDE BTCMEM.J86     ! Controller CBASIC Memory Functions
%INCLUDE CIPPMEXT.J86
%INCLUDE DRUGEXT.J86
%INCLUDE IDFEXT.J86
%INCLUDE IEFEXT.J86
%INCLUDE IEXEXT.J86
%INCLUDE IRFEXT.J86
%INCLUDE ITGRPEXT.J86   ! IRF Attribute Extension File                  !HRN
%INCLUDE ISFEXT.J86
%INCLUDE IUFEXT.J86
%INCLUDE JOBOKEXT.J86
%INCLUDE LISTLIB.J86    ! UNLIMITED ARRAY ROUTINES
%INCLUDE LOCALEXT.J86
%INCLUDE NLINEEXT.J86
%INCLUDE PGFEXT.J86
%INCLUDE PPFEXT.J86
%INCLUDE PPFKEXT.J86
%INCLUDE RICFEXT.J86
%INCLUDE SOFTSEXT.J86
%INCLUDE STKMQEXT.J86
%INCLUDE STOCKEXT.J86
%INCLUDE SUPPEXT.J86
%INCLUDE WEEEEXT.J86


%INCLUDE PSBF01E.J86    ! APPLICATION.LOG
%INCLUDE PSBF02E.J86    ! UPDATE.DATE                          ! 1.18 RC
%INCLUDE PSBF13E.J86    ! PSDATE
%INCLUDE PSBF18E.J86    ! CALCULATE BOOTS CHECK DIGIT          ! 1.19 RC (238)
%INCLUDE PSBF19E.J86    ! UPDATE IRF EXTERNALS
%INCLUDE PSBF20E.J86    ! SESSION NUMBER UTILITY
%INCLUDE PSBF24E.J86    ! STANDARD ERROR DETECTED
%INCLUDE PSBF48E.J86    ! DEC API Logging


%INCLUDE CSORTDEC.J86   ! Assembler sort routine               ! 1.10 RC (113)


SUB UPDATE.BCF EXTERNAL
END SUB

FUNCTION CREATE.PPFK EXTERNAL
INTEGER*1 CREATE.PPFK
END FUNCTION

FUNCTION DESTROY.PPFK EXTERNAL
INTEGER*1 DESTROY.PPFK
END FUNCTION

SUB SEND.DEC.MESSAGE EXTERNAL
END SUB

%INCLUDE ERRNH.J86

\*******************************************************************************
\***
\***    PSB21 Functions
\***
\***............................................................................

\************************************************************************
\***
\*** Execute FlexOS supervisor commands
\***
\*************************************************************************
FUNCTION FLEXOSSVC(COMMAND%, PARAM$) EXTERNAL
      STRING PARAM$
      INTEGER*2 COMMAND%, FLEXOSSVC
END FUNCTION


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
\*****************************************************************************

FUNCTION CONTTIME (START.TIME, PARMBLK$, TIMEDATE.TABLE$) EXTERNAL
    STRING PARMBLK$, TIMEDATE.TABLE$
    INTEGER*4 START.TIME, CONTTIME
END FUNCTION



\*****************************************************************************
\***
\***    MILLI.TIME% function
\***    Sets MILLI.TIME% to controller time since midnight in milli-seconds.
\***    Integer format.
\***
\*****************************************************************************

FUNCTION MILLI.TIME%

    INTEGER*4 MILLI.TIME%

    MILLI.TIME% = CONTTIME(0, CT.PARAMBLK$, CT.TIMEDATE.TABLE$)

END FUNCTION

\*****************************************************************************
\***
\***    MILLI.TIME$ function
\***    Sets MILLI.TIME$ to controller time since midnight in milli-seconds.
\***    String format.
\***
\***..........................................................................

FUNCTION MILLI.TIME$

    STRING MILLI.TIME$

    MILLI.TIME$ = STR$(MILLI.TIME%)

END FUNCTION


\*****************************************************************************
\***
\***    HHMMSS.MMM$ function
\***    Converts time (parmed as milli-seconds since midnight) into
\***    an understandable HH:MM:SS.MMM format
\***
\*****************************************************************************

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



\******************************************************************************
\***
\***    Format date    YYMMDD --> DD/MM/YYYY
\***
\******************************************************************************

FUNCTION FORMAT.DATE$ (FIELD$)

    STRING      FIELD$
    STRING      FORMAT.DATE$

    FORMAT.DATE$ = MID$(FIELD$, 5, 2) + "/"   + \
                   MID$(FIELD$, 3, 2) + "/20" + \
                   MID$(FIELD$, 1, 2)

END FUNCTION


\******************************************************************************
\***
\***    Format Time      HHMMSS --> HH:MM
\***
\******************************************************************************

FUNCTION FORMAT.TIME$ (FIELD$)

    STRING      FIELD$
    STRING      FORMAT.TIME$


    FORMAT.TIME$ = MID$(FIELD$, 1, 2) +   \
                   ":"                +   \
                   MID$(FIELD$, 3, 2)

END FUNCTION





\*******************************************************************************
\***
\***    PSB21 Subroutines
\***
\***............................................................................

SUB PROCESS.IUF.ITEM EXTERNAL
END SUB

SUB PROCESS.PGF EXTERNAL
END SUB

SUB CHECK.PROCESS.PGF EXTERNAL                                   ! 1.20 RC (240)
END SUB                                                          ! 1.20 RC (240)

SUB PROCESS.OLD.PPFI EXTERNAL
END SUB

SUB CREATE.NEW.PPFI EXTERNAL
END SUB

SUB PROGRAM.EXIT EXTERNAL
END SUB


\******************************************************************************
\***
\***   EXECUTE.SVC
\***
\******************************************************************************
SUB EXECUTE.SVC

    SVC.PARAM$   = "                        "
    PINFO.TABLE$  = "PSB21     " + CHR$(200) + STRING$(9,CHR$(00))
    PROG.NAME$   = PROG.NAME$ + CHR$(00)
    COMM.TAIL$   = COMM.TAIL$ + CHR$(00)
    REM Build a COMMAND SVC on the FlexOS PROCESS table
    CALL PUTN4(SVC.PARAM$,  0, 00000000H)
    CALL PUTN4(SVC.PARAM$,  4, 00000000H)
    CALL PUTN4(SVC.PARAM$,  8, SADD(PROG.NAME$) + 2)
    CALL PUTN4(SVC.PARAM$, 12, SADD(COMM.TAIL$) + 2)
    CALL PUTN4(SVC.PARAM$, 16, LEN(COMM.TAIL$) - 1)
    CALL PUTN4(SVC.PARAM$, 20, SADD(PINFO.TABLE$) + 2)
    CALL PUTN4(SVC.PARAM$, 24, 00000000H)
    CALL FLEXOSSVC(38, SVC.PARAM$)

END SUB



\*******************************************************************************
\***
\***    DO.MESSAGE
\***
\*******************************************************************************
SUB DO.MESSAGE(MSG$, LOG%) PUBLIC

    INTEGER*1 LOG%  ! Output to Background screen if TRUE
    STRING    MSG$

    IF STATUS% = 1 THEN BEGIN
        PRINT HHMMSS.MMM$(MILLI.TIME%) + " " + MSG$
    ENDIF ELSE IF STATUS% = 2 THEN BEGIN
        IF LOG% THEN BEGIN
           CALL ADXSERVE (ADXSERVE.RET.CODE%,26,0,MSG$)
        ENDIF
    ENDIF

    MSG$ = HHMMSS.MMM$(MILLI.TIME%) + " " + MSG$ + CRLF$
    FORM$ = "C" + STR$(LEN(MSG$))
    WRITE FORM FORM$; # LOG.SESS.NUM%; MSG$

END SUB


\******************************************************************************
\***
\***    OPEN.IUF
\***
\******************************************************************************
\***
\***    Opens the IUF
\***
\******************************************************************************

FUNCTION OPEN.IUF

    INTEGER*1 OPEN.IUF

    OPEN.IUF = 1

    IF SIZE(IUF.FILE.NAME$) > 0 THEN BEGIN

        FILE.OPERATION$ = "O"
        CURRENT.REPORT.NUM% = IUF.REPORT.NUM%

        ! Open the IUF
        IF END # IUF.SESS.NUM% THEN IUF.OPEN.ERROR
        OPEN IUF.FILE.NAME$ AS IUF.SESS.NUM% BUFFSIZE 32256 LOCKED NOWRITE NODEL

        IUF.EXISTS = TRUE
        IUF.OPEN = TRUE                                          ! 1.6 RC (87)
        OPEN.IUF   = 0

    ENDIF

    IUF.OPEN.ERROR:

END FUNCTION


\******************************************************************************
\***
\***    CLOSE.IUF
\***
\******************************************************************************
\***
\***    Closes the IUF
\***
\******************************************************************************

FUNCTION CLOSE.IUF ! Entire procedure new for Rv 1.6 RC          ! 1.6 RC (87)

    IF IUF.OPEN THEN BEGIN
        CLOSE IUF.SESS.NUM%
        IUF.OPEN = FALSE
    ENDIF

END FUNCTION


\*******************************************************************************
\***
\***    LOG.EVENT
\***    Writes details of Event to application event log.
\***
\***............................................................................
SUB LOG.EVENT(EVENT.NO%) PUBLIC

    INTEGER*1 EVENT.NO%

    VAR.STRING.2$ = " "

    IF EVENT.NO% = 8 THEN BEGIN
        ! Price mismatch on IUF/IRF. Item Refresh requested.
        VAR.STRING.1$ = PACK$("0" + CURR.BOOTS.CODE$) + PACK$("00000000")

    ENDIF ELSE IF EVENT.NO% = 13 THEN BEGIN
        ! Unexpected record sequence (IUF)
        VAR.STRING.1$ = CURR.BOOTS.CODE$ + EXP.REC.TYPE$ + CURR.REC.TYPE$

    ENDIF ELSE IF EVENT.NO% = 14 THEN BEGIN
            VAR.STRING.1$ = "PGFWRK  "

    ENDIF ELSE IF EVENT.NO% = 15 THEN BEGIN
        ! Unexpected store number
        VAR.STRING.1$ = STORE.NUMBER$ + IUF.STORE.NUM$
        ERROR$ = "9"  ! Unexpected Error
        !ERROR$ = "2"   ! Header problem

    ENDIF ELSE IF EVENT.NO% = 17 THEN BEGIN
        ! Unexpected record type
        VAR.STRING.1$ = STR$(PPFI.REPORT.NUM%) + PACK$("09999999")

    ENDIF ELSE IF EVENT.NO% = 18 THEN BEGIN
        ! CM/CT Program Chaining Error
         VAR.STRING.1$ = "B2100B20  "

    ENDIF ELSE IF EVENT.NO% = 21 THEN BEGIN
        ! Incorrect BCF serial No / Date time stamp
        IF IUF.NEW.FORMAT THEN BEGIN
             VAR.STRING.1$ = PACK$(LEFT$((IUF.TIME.STAMP$ + STRING$(18,"0")),18))
        ENDIF ELSE BEGIN
            VAR.STRING.1$ = PACK$(BCF.IUF.SERIAL.NO$ + IUF.SERIAL.NO$)
        ENDIF
        !  Batch Already Processed, but still process Pending price file
       ERROR$ = "5"  ! Batch Already Processed
       JOBSOK.FLAG$ = "Y"
       CALL SEND.DEC.MESSAGE
       ERROR$ = "0"  ! Reset status for next batch

    ENDIF ELSE IF EVENT.NO% = 23 THEN BEGIN
        ! General ADXSERVE call error
        VAR.STRING.1$ = \
          RIGHT$("00000" + STR$(ADXSERVE.RETURN.CODE%),5) + \
          PACK$("00000000") + "4"

    ENDIF ELSE IF EVENT.NO% = 57 THEN BEGIN
        ! ADXCOPYF Error
        VAR.STRING.1$ = RIGHT$(STR$(RC%),2) + BACKUP.SOURCE.FILE$

    ENDIF ELSE IF EVENT.NO% = 84 THEN BEGIN
        ! Error occured in module 'n'
        VAR.STRING.1$ = MODULE.NUMBER$ + RIGHT$("   " + PHASE$,3)

    ENDIF ELSE IF EVENT.NO% = 92 THEN BEGIN
        ! Count Discrepancy
        VAR.STRING.1$ = CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) + \ ! Two byte integer byte
                        CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) + \ ! order reversed
                        PACK$(RIGHT$("00000000" + STR$(COUNT.EXPECTED%),8)) + \
                        PACK$(RIGHT$("00000000" + STR$(COUNT.ACTUAL%),8))

        ERROR$ = "3" ! Trailer problem

    ENDIF ELSE IF EVENT.NO% = 106 THEN BEGIN
        ! IF END errors: Open/Read/Write/Delete/Create/Size
        VAR.STRING.1$ = FILE.OPERATION$                +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,0))

        IF FILE.OPERATION$ = "O" AND CURRENT.REPORT.NUM% = IUF.REPORT.NUM% THEN BEGIN
            CALL DO.MESSAGE("PSB21 *** Item Update File missing", FALSE)
            ! IUF file missing but still process Pending price file
            JOBSOK.FLAG$ = "E"                                                         !1.6CSk
            EXIT SUB
        ENDIF


    ENDIF ELSE IF EVENT.NO% = 222 THEN BEGIN
        ! Log DEC API Message issue
        VAR.STRING.1$ = EVENT.222.DATA$

    ENDIF ELSE BEGIN
         VAR.STRING.1$ = "PSB21UNK"
         VAR.STRING.2$ = "NOWN ERR"
         ERROR$        = "9"         ! Unexpected error
    ENDIF

    RC% = APPLICATION.LOG (MESSAGE.NO%,     \
                           VAR.STRING.1$,   \
                           VAR.STRING.2$,   \
                           EVENT.NO%)

   IF CURRENT.REPORT.NUM% = IUF.REPORT.NUM% THEN BEGIN
        CALL DO.MESSAGE("PSB21 *** LOG EVENT: " + STR$(EVENT.NO%) + \
                        " at IUF Rec: " + STR$(COUNT.RECORDS.IUF%), FALSE)
   ENDIF

   IF EVENT.NO% <> 84 THEN BEGIN ! Error occured in module 'n'                  !1.6CSk
        ! Log an additonal Event 84 that will identify the Module and Phase     !1.6CSk
        ! that the error occurred in. Useful for any errors that are raised     !1.6CSk
        ! before the log file is created and written to.                        !1.6CSk
        VAR.STRING.1$ = MODULE.NUMBER$ + RIGHT$("   " + PHASE$,3)               !1.6CSk
        VAR.STRING.2$ = ""                                                      !1.6CSk
        RC% = APPLICATION.LOG (MESSAGE.NO%, VAR.STRING.1$, VAR.STRING.2$, 84)   !1.6CSk
   ENDIF

   !-----------------------------------
   ! Check to see if EVENT.NO% is FATAL
   !-----------------------------------
   IF ERROR$ = "9" THEN BEGIN
         EVENT.NO% =  0      ! Override Event No.
   ENDIF

   IF EVENT.NO% =   0 OR \   ! Unknown Event No.
      EVENT.NO% =  15 OR \   ! Unexpected store number
      EVENT.NO% =  23 OR \   ! General ADXSERVE call error
      EVENT.NO% =  84 OR \   ! Fatal Error in Module + Phase
      EVENT.NO% =  92 OR \   ! Incorrect Batch trailer Count
     (EVENT.NO% = 106 AND (CURRENT.REPORT.NUM% = IUF.REPORT.NUM%     OR \
                           CURRENT.REPORT.NUM% = JOBSOK.REPORT.NUM%  OR \
                           CURRENT.REPORT.NUM% = PPFI.REPORT.NUM%    OR \
                           CURRENT.REPORT.NUM% = PPFK.REPORT.NUM%    OR \
                           CURRENT.REPORT.NUM% = PPFO.REPORT.NUM%    OR \
                           CURRENT.REPORT.NUM% = BCF.REPORT.NUM%     OR \
                           CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%)) THEN BEGIN

       CALL DO.MESSAGE("PSB21 *** FATAL ERROR - ABORTING ***", FALSE)
       JOBSOK.FLAG$ = "X"
       CALL SEND.DEC.MESSAGE
       CALL CLOSE.IUF                                            ! 1.6 RC (87)
       CALL PROGRAM.EXIT
   ENDIF

END SUB


\******************************************************************************
\***
\***    OLD.BATCH.FOUND
\***
\******************************************************************************
\***
\***    Checks previously processed batch references to determine whether
\***    the current IUF batch has already been processed.
\***
\***    NOTE: Different processing is required for old and new IUF formats.
\***
\******************************************************************************

FUNCTION OLD.BATCH.FOUND

    INTEGER*1 OLD.BATCH.FOUND

    PHASE$ = "3.1"                                                              !1.6CSk
    OLD.BATCH.FOUND = FALSE


    IF IUF.NEW.FORMAT THEN BEGIN
        CALL DO.MESSAGE("PSB21 3.1 - New IUF Format Detected", TRUE)
    ENDIF ELSE BEGIN
        CALL DO.MESSAGE("PSB21 3.1 - Old IUF Format Detected", TRUE)
        PHASE$ = "3.2"                                                          !1.6CSk
        CALL DO.MESSAGE("PSB21 PHASE 3.2 - PROCESSING IUF", TRUE)
    ENDIF

    !-----------------------------------------------------------------------------
    ! If the BCF IUF timestamp is populated, then check the BCF and IUF serial
    ! numbers otherwise ignore further checks ie. blank timestamp indicates that
    ! it is the very first time its ran.
    !-----------------------------------------------------------------------------

    IF NOT IUF.NEW.FORMAT THEN BEGIN

        IF BCF.IUF.SERIAL.NO$ >= IUF.SERIAL.NO$ THEN BEGIN
            !-------------------------------- --------------------------------------
            ! Check to see if IUF Serial No. has wrapped back round (within reason)
            ! and raise error if IUF Serial No. or BCF Serial No. is outside limits.
            !----------------------------------------------------------------------

            IF BCF.IUF.SERIAL.NO$ < "9990" OR    \
                   IUF.SERIAL.NO$ > "0010" THEN BEGIN
                CALL DO.MESSAGE("*** WARNING: Old IUF Batch Serial No. " + \
                                 IUF.SERIAL.NO$                                   + \
                                 " BCF Serial No. "                               + \
                                 BCF.IUF.SERIAL.NO$, FALSE)

                OLD.BATCH.FOUND = TRUE
                CALL DO.MESSAGE("PSB21 *** ERROR: OLD Batch Already Processed", FALSE)
                CALL LOG.EVENT(21)
            ENDIF
        ENDIF
    ENDIF ELSE BEGIN ! New Format

        IF LEFT$(BCF.IUF.DATETIMESTAMP$,1) <> " " THEN BEGIN

            IF BCF.IUF.DATETIMESTAMP$ >= IUF.TIME.STAMP$ THEN BEGIN
                OLD.BATCH.FOUND = TRUE
                CALL DO.MESSAGE("PSB21 *** ERROR: NEW Batch Already Processed", FALSE)
                CALL LOG.EVENT(21)
            ENDIF
        ENDIF
    ENDIF

END FUNCTION


\******************************************************************************
\***
\***  DETERMINE.LOG.FILE
\***
\***  This routine determines which log file we need to log to. PSB21 will keep
\***  a daily audit log of update activity in rolling files PSB21MON.LOG to
\***  PSB21SUN.LOG. If todays log file does not exist or is last weeks, then it
\***  is created. If todays log file exists with todays date, then it is appended
\***  to.
\******************************************************************************

SUB DETERMINE.LOG.FILE

    PRINT "Determining Log File for day: " + F13.DAY$

    LOG.FILENAME$ = "ADXLXACN::D:\ADX_UDT1\PSB21" + F13.DAY$ + ".LOG"
    DIR.FILENAME$ = "ADXLXACN::D:\ADX_UDT1\PSB21DIR"
    PROG.NAME$    = "C:\ADX_SPGM\COMMAND.286"
    COMM.TAIL$    = "-CDIR " + LOG.FILENAME$ + " > " + DIR.FILENAME$
    CALL EXECUTE.SVC

    CALL SESS.NUM.UTILITY("O", 450, DIR.FILENAME$)   ! 450 = Use General Temp file Report Num
    DIR.SESS.NUM% = F20.INTEGER.FILE.NO%

    CALL SESS.NUM.UTILITY("O", 695, LOG.FILENAME$)   ! 695 = Use General Report Num
    LOG.SESS.NUM% = F20.INTEGER.FILE.NO%

    FOUND = FALSE
    FILE.OPERATION$ = "O"
    CURRENT.REPORT.NUM% = 450
    IF END # DIR.SESS.NUM% THEN JUMP.OUT
    OPEN DIR.FILENAME$ AS DIR.SESS.NUM% BUFFSIZE 32256 LOCKED NOWRITE NODEL

    WHILE NOT FOUND ! Jumps out automatically if end-of-file reached

        IF END #DIR.SESS.NUM% THEN JUMP.OUT
        READ #DIR.SESS.NUM%; DIR.RECORD$

        IF (LEFT$(DIR.RECORD$,2) <> "  ") THEN BEGIN
            DIR.FILENAME$ = LEFT$(DIR.RECORD$,8   )   ! eg. PSB21FRI
            FILE.DATE$    =  MID$(DIR.RECORD$,24,10)  ! MM-DD-CCYY  nb. 1st digit of MM can be a space
            COMPARE$ = "PSB21" + F13.DAY$

            IF COMPARE$ = DIR.FILENAME$ THEN BEGIN
                FOUND = TRUE ! Log File already exists
                IF LEFT$(FILE.DATE$,1) = " " THEN BEGIN
                     FILE.DATE$ = "0" + RIGHT$(FILE.DATE$,9)
                ENDIF
                FILE.TIME.AFTER.6PM = FALSE                          ! 1.18 RC (237)
                IF MID$(DIR.RECORD$,41, 1) = "p" \  ! pm             ! 1.18 RC (237)
                  AND VAL(MID$(DIR.RECORD$,36, 2)) >= 6 THEN BEGIN   ! 1.18 RC (237)
                    FILE.TIME.AFTER.6PM = TRUE                       ! 1.18 RC (237)
                ENDIF                                                ! 1.18 RC (237)
            ENDIF
        ENDIF
    WEND

  JUMP.OUT:
  !--------

    CLOSE DIR.SESS.NUM%                                                                               !1.6CSk


!---------------------------------------------------------------------------- ! 1.18 RC (237)
!   Create, recreate or open log file                                         ! 1.18 RC (237)
!   Pass these from the PPFK tables to the PREV tables                        ! 1.18 RC (237)
!---------------------------------------------------------------------------- ! 1.18 RC (237)
!   Remainder of this section re-structured/re-written for Rv 1.18 RC (237)   ! 1.18 RC (237)

!   PROCESSING.DATE$ date is passed to PSB21 from PSB20.
!   It is set to tomorrows date if the system date is after 6pm
!   (specifically if TIME$ > "1800" which incudes "180000").
!   It is used to set the day component of LOG.FILENAME$ (via F13.DAY$).
!   The log file for day "xxx" is PSB21xxx.LOG and is updated from 6pm
!   prior to day "xxx" and up to 6pm on day "xxx"

    IF NOT FOUND THEN BEGIN                                                                     !
!       Create log file as it does not exist at all
        FILE.OPERATION$ = "C"
        PRINT "Creating new log file: " + LOG.FILENAME$
        CREATE POSFILE LOG.FILENAME$ AS LOG.SESS.NUM% BUFFSIZE LOG.BUFF.SIZE% UNLOCKED MIRRORED PERUPDATE   ! 1.4 RC
        EXIT SUB                                                            ! UNLOCKED to force physical    ! 1.4 RC
    ENDIF                                                                   ! write for each logical write  ! 1.4 RC

    TODAY$ = RUN.DATE$  ! YYMMDD

    F02.DATE$ EQ TODAY$ ! YYMMDD                                                             ! 1.18 RC (237)
    CALL UPDATE.DATE(-1)                                                                     ! 1.18 RC (237)
    YESTERDAY.DDMMYY$ = RIGHT$(F02.DATE$, 2) + MID$(F02.DATE$, 3, 2) + LEFT$(F02.DATE$, 2)   ! 1.18 RC (237)

    TODAY$ =   RIGHT$(TODAY$, 2) + MID$(TODAY$, 3, 2) + LEFT$(TODAY$, 2) ! DDMMYY
    FILE.DATE$ = MID$(FILE.DATE$, 4, 2) + RIGHT$("00" + LEFT$(FILE.DATE$, 2), 2) + RIGHT$(FILE.DATE$, 2) ! DDMMYY

!   At this point it is known a log file already exists for required day of week
!   Check whether this log file relates to price change day currently being processed

    IF (     RUN.TIME$ >= "180000" \   ! Program running in evening before price change day
         AND FILE.DATE$ = TODAY$ \     ! Log file was last created/recreated/updated this evening
         AND FILE.TIME.AFTER.6PM ) \
      OR \
       (     RUN.TIME$ < "180000" \           ! Program running on day of price changes
         AND FILE.DATE$ = YESTERDAY.DDMMYY$ \ ! Log file was last created/recreated/updated yesterday evening
         AND FILE.TIME.AFTER.6PM ) \
      OR \
       (     RUN.TIME$ < "180000" \           ! Program running on day of price changes
        AND FILE.DATE$ = TODAY$) THEN BEGIN   ! Log file was last created/recreated/updated today

!         Append to existing log file recently updated for current PROCESSING.DATE$ day
          FILE.OPERATION$ = "O"
          PRINT "Appending to log file: " + COMPARE$
          OPEN LOG.FILENAME$ AS LOG.SESS.NUM% BUFFSIZE LOG.BUFF.SIZE% UNLOCKED APPEND               ! 1.4 RC
          EXIT SUB                                                  ! UNLOCKED to force physical    ! 1.4 RC
    ENDIF                                                           ! write for each logical write  ! 1.4 RC

!   At this point it is known a log file already exists for required day of week
!   but does not contain any data for the price change day currently being processed

!   Re-create log file
    FILE.OPERATION$ = "C"
    PRINT "Re-creating log file: " + COMPARE$
    CREATE POSFILE LOG.FILENAME$ AS LOG.SESS.NUM% BUFFSIZE LOG.BUFF.SIZE% UNLOCKED MIRRORED PERUPDATE   ! 1.4 RC
                                                                        ! UNLOCKED to force physical    ! 1.4 RC
                                                                        ! write for each logical write  ! 1.4 RC

!   Initial log file message moved to INITIALISATION                      ! 1.18 RC (237)

END SUB


\******************************************************************************
\***
\***  BACKUP.IUF.FILE
\***
\***  This routine maintains 7 days backups of IUF files: IUF.MON to IUF.SUN.
\***  Any existing backups will be overwritten.
\******************************************************************************

SUB BACKUP.IUF.FILE PUBLIC

    PHASE$ = "6.0"                                                                  !1.6CSk
    CALL DO.MESSAGE("PSB21 PHASE 6", TRUE)

!   Ensure IUF is only backed up when processing was successful   ! 1.17 RC (230)
!   (which at this point means JOBSOK.FLAG$ is still " " blank    ! 1.17 RC (230)
!    prior to being changed to "E" within UPDATE.JOBSOK)          ! 1.17 RC (230)
!   or IUF does not otherwise need to be re-processed             ! 1.17 RC (230)
!   (which at this point means JOBSOK.FLAG$ is "Y")               ! 1.17 RC (230)
    IF    JOBSOK.FLAG$ <> " " \                                   ! 1.17 RC (230)
      AND JOBSOK.FLAG$ <> "Y" THEN BEGIN                          ! 1.17 RC (230)
        EXIT SUB                                                  ! 1.17 RC (230)
    ENDIF                                                         ! 1.17 RC (230)

    IF IUF.EXISTS \                                               ! 1.6 RC (87)
      AND NOT IUF.OPEN THEN BEGIN                                 ! 1.6 RC (87)

        PHASE$ = "6.1"                                                              !1.6CSk
        CALL DO.MESSAGE("PSB21 6.1  - Backing up IUF File to: IUFBK." + F13.DAY$, TRUE)

        RC% = OPEN.IUF

        IF RC% <> 0 THEN BEGIN
            PHASE$ = "6.2"                                                          !1.6CSk
            IF NOT IUF.OPEN.ERROR.REPORTED THEN BEGIN             ! 1.8 RC (8)
                CALL DO.MESSAGE("PSB21 6.2 *** ERROR: opening IUF for backup", FALSE)
                !CALL LOG.EVENT(106)
                IUF.OPEN.ERROR.REPORTED = TRUE                    ! 1.8 RC (8)
            ENDIF                                                 ! 1.8 RC (8)
            EXIT SUB
        ENDIF

        IUF.BACKUP.FILENAME$ = "ADXLXACN::D:\ADX_UDT1\IUFBK." + F13.DAY$

        CALL SESS.NUM.UTILITY("O", 695, IUF.BACKUP.FILENAME$)   ! 695 = Use General Report Num
        IUF.BACKUP.SESS.NUM% = F20.INTEGER.FILE.NO%


        CALL ADXCOPYF(RC%, IUF.FILE.NAME$, IUF.BACKUP.FILENAME$,0,0,0)

        IF RC% <> 0 THEN BEGIN
            PHASE$ = "6.3"                                                          !1.6CSk
            CALL DO.MESSAGE("PSB21 6.3 *** ERROR copying IUF to " + \
                            IUF.BACKUP.FILENAME$, TRUE)
            BACKUP.SOURCE.FILE$ = "IUF     "
            CALL LOG.EVENT(57)
            EXIT SUB
        ENDIF ELSE BEGIN
            DELETE IUF.SESS.NUM%
            CALL DO.MESSAGE("PSB21 IUF BACKUP - deleted IUF file", TRUE) ! 1.4 RC
        ENDIF

        CALL DO.MESSAGE("PSB21 IUF BACKUP - Completed IUF Backup", TRUE) ! 1.4 RC

    ENDIF ELSE BEGIN

        CALL DO.MESSAGE("PSB21 IUF BACKUP - IUF missing / not processed", TRUE) ! 1.6 RC (50)
        CALL DO.MESSAGE("PSB21 IUF BACKUP - No IUF backup made", TRUE)          ! 1.4 RC

    ENDIF

END SUB




\******************************************************************************
\***
\***    SET.AND.ALLOCATE.FILE.SESSION.NUMBERS
\***
\******************************************************************************
SUB SET.AND.ALLOCATE.FILE.SESSION.NUMBERS

    CALL BCF.SET
    CALL CIPPM.SET
    CALL DRUG.SET
    CALL IDF.SET
    CALL IEF.SET
    CALL IEX.SET
    CALL IRF.SET
    CALL IRFITGRP.SET                                                   !HRN
    CALL ISF.SET
    CALL IUF.SET
    CALL LOCAL.SET
    CALL NEWLINES.SET
    CALL PGF.SET
    CALL PPFI.SET
    CALL PPFK.SET
    CALL PPFO.SET
    CALL RICF.SET
    CALL SOFTS.SET
    CALL STKMQ.SET
    CALL STOCK.SET
    CALL SUPPARAM.SET
    CALL WEEEUF.SET

    CALL SESS.NUM.UTILITY("O", BCF.REPORT.NUM%, BCF.FILE.NAME$)
    BCF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", CIPPM.REPORT.NUM%, CIPPM.FILE.NAME$)
    CIPPM.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", DRUG.REPORT.NUM%, DRUG.FILE.NAME$)
    DRUG.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IDF.REPORT.NUM%, IDF.FILE.NAME$)
    IDF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IEF.REPORT.NUM%, IEF.FILE.NAME$)
    IEF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IEX.REPORT.NUM%, IEX.FILE.NAME$)
    IEX.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IRF.REPORT.NUM%, IRF.FILE.NAME$)
    IRF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IRFDEX.REPORT.NUM%, IRFDEX.FILE.NAME$)  ! 1.21 RC (242)
    IRFDEX.SESS.NUM% = F20.INTEGER.FILE.NO%                            ! 1.21 RC (242)
    CALL SESS.NUM.UTILITY("O", ISF.REPORT.NUM%, ISF.FILE.NAME$)
    ISF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", IUF.REPORT.NUM%, IUF.FILE.NAME$)
    IUF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", LOCAL.REPORT.NUM%, LOCAL.FILE.NAME$)
    LOCAL.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", NEWLINES.REPORT.NUM%, NEWLINES.FILE.NAME$)
    NEWLINES.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", PPFI.REPORT.NUM%, PPFI.FILE.NAME$)
    PPFI.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", PGF.REPORT.NUM%, PGF.FILE.NAME$)
    PGF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O",PPFK.REPORT.NUM%,PPFK.FILE.NAME$)
    PPFK.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", PPFO.REPORT.NUM%, PPFO.FILE.NAME$)
    PPFO.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", RICF.REPORT.NUM%, RICF.FILE.NAME$)
    RICF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", SOFTS.REPORT.NUM%, SOFTS.FILE.NAME$)
    SOFTS.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", STKMQ.REPORT.NUM%, STKMQ.FILE.NAME$)
    STKMQ.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", STOCK.REPORT.NUM%, STOCK.FILE.NAME$)
    STOCK.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", SUPPARAM.REPORT.NUM%, SUPPARAM.FILE.NAME$)
    SUPPARAM.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O", WEEEUF.REPORT.NUM%, WEEEUF.FILE.NAME$)
    WEEEUF.SESS.NUM% = F20.INTEGER.FILE.NO%
    CALL SESS.NUM.UTILITY("O",IRFITGRP.REPORT.NUM%,IRFITGRP.FILE.NAME$) !HRN
    IRFITGRP.SESS.NUM% = F20.INTEGER.FILE.NO%                           !HRN

END SUB


\******************************************************************************
\***
\***   OPEN.FILES
\***
\******************************************************************************
SUB OPEN.FILES

    FILE.OPERATION$ = "O"

!   CURRENT.REPORT.NUM% = BCF.REPORT.NUM%                           ! 1.15 CSk
!   IF END # BCF.SESS.NUM% THEN OPEN.FILE.ERROR                     ! 1.15 CSk
!   OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM%             ! 1.15 CSk

    CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%
    IF END # SOFTS.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN SOFTS.FILE.NAME$ RECL SOFTS.RECL% AS SOFTS.SESS.NUM% NOWRITE NODEL

    CURRENT.REPORT.NUM% = CIPPM.REPORT.NUM%
    IF END # CIPPM.SESS.NUM% THEN CIPPMR.NOT.PRESENT                ! 1.16 RC (226)
    OPEN CIPPM.FILE.NAME$ AS CIPPM.SESS.NUM% LOCKED
    CIPPM.PRESENT = TRUE                                            ! 1.16 RC (226)

CIPPMR.NOT.PRESENT: ! Store not on CIP                              ! 1.16 RC (226)

    CURRENT.REPORT.NUM% = DRUG.REPORT.NUM%
    IF END # DRUG.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN DRUG.FILE.NAME$ KEYED RECL DRUG.RECL% AS DRUG.SESS.NUM%

    CURRENT.REPORT.NUM% = IDF.REPORT.NUM%
    IF END # IDF.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%

    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%
    IF END # IEF.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%

    CURRENT.REPORT.NUM% = IEX.REPORT.NUM%
    IF END # IEX.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN IEX.FILE.NAME$ KEYED RECL IEX.RECL% AS IEX.SESS.NUM%

    CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
    IF END # IRF.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%

    IF OPEN.IRF.UPDT (NEW.IRF.DATA$,ACD.FLAG$) THEN BEGIN
        GOTO OPEN.FILE.ERROR
    ENDIF

    CURRENT.REPORT.NUM% = IRFDEX.REPORT.NUM%                           ! 1.21 RC (242)
    IF END # IRFDEX.SESS.NUM% THEN OPEN.FILE.ERROR                     ! 1.21 RC (242)
    OPEN IRFDEX.FILE.NAME$ KEYED RECL IRFDEX.RECL% AS IRFDEX.SESS.NUM% ! 1.21 RC (242)

    CURRENT.REPORT.NUM% = ISF.REPORT.NUM%
    IF END # ISF.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN ISF.FILE.NAME$ KEYED RECL ISF.RECL% AS ISF.SESS.NUM%

    CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
    IF END # LOCAL.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN LOCAL.FILE.NAME$ KEYED RECL LOCAL.RECL% AS LOCAL.SESS.NUM%

    CURRENT.REPORT.NUM% = NEWLINES.REPORT.NUM%
    IF END # NEWLINES.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN NEWLINES.FILE.NAME$ KEYED RECL NEWLINES.RECL% AS NEWLINES.SESS.NUM%

    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    IF END # STOCK.SESS.NUM% THEN OPEN.FILE.ERROR
    OPEN STOCK.FILE.NAME$ KEYED RECL STOCK.RECL% AS STOCK.SESS.NUM%

	
	
    IRFITGRP.OPEN = FALSE                                               !HRN
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%                          !HRN
 
    IF END #IRFITGRP.SESS.NUM% THEN OPEN.FILE.ERROR                     !HRN
	
	PRINT "FILE NAME" ;IRFITGRP.FILE.NAME$
	PRINT "RECORD LENGTH" ; IRFITGRP.RECL%
	PRINT " SESS NUM" ; IRFITGRP.SESS.NUM%
	PRINT "REPORT NUM" ; IRFITGRP.REPORT.NUM% 
    OPEN IRFITGRP.FILE.NAME$ KEYED RECL IRFITGRP.RECL% AS IRFITGRP.SESS.NUM%!HRN
													  
	 
	
    IRFITGRP.OPEN = TRUE                                                !HRN

    EXIT SUB

 OPEN.FILE.ERROR:
 
    PRINT " I AM ERRORING"

    CALL LOG.EVENT(106)

END SUB


\******************************************************************************
\***
\***  CREATE.FILES
\***
\******************************************************************************
SUB CREATE.FILES

    CURRENT.REPORT.NUM% = RICF.REPORT.NUM%
    CREATE POSFILE RICF.FILE.NAME$ DIRECT 0 RECL RICF.RECL% \
                    AS RICF.SESS.NUM% MIRRORED ATCLOSE

!   Line deleted                                               ! 1.13 RC (179)

END SUB




\******************************************************************************
\***
\***    GET.BCF.DETAILS
\***
\******************************************************************************
SUB GET.BCF.DETAILS

    INTEGER*2  BCF.REC22.OFFSET.COUNT% !Initial offset of           ! ACSk
                                       !BRCF record 22              ! ACSk
    STRING     DEFAULT.GCM.PG.NO$, \   !Default & Read Gift Card    ! ACSk
               READ.GCM.PG.NO$     !   !Mall Product Group Number   ! ACSk

    CALL DO.MESSAGE("PSB21 - GET.BCF.DETAILS", FALSE)

    FILE.OPERATION$ = "O"                                           ! 1.15 CSk

    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%                           ! 1.15 CSk
    IF END # BCF.SESS.NUM% THEN OPEN.BCF.ERROR                      ! 1.15 CSk
    OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM%             ! 1.15 CSk

    !--------------------------------------------
    ! Extact BCF.IUF.SERIAL.NO$ from BCF record 1
    !--------------------------------------------
    BCF.REC.NO% = 1
    RC% = READ.BCF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF
    CALL DO.MESSAGE("PSB21 - BCF Rec  1 Serial No         : [" + BCF.IUF.SERIAL.NO$ + "]", FALSE)

    !-------------------------------------------------
    ! Extact BCF.ECC.DATETIMESTAMP$ from BCF record 14
    !-------------------------------------------------
    BCF.REC.NO% = 14  !
    RC% = READ.BCF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF
    CALL DO.MESSAGE("PSB21 - BCF Rec 14 ECC Date Timestamp: [" + BCF.FILLER.DATETIMESTAMP$ + "]", FALSE) !GMW

    !------------------------------------------                                          !1.6CSk
    ! Extact Statins markers from BCF record 16                                          !1.6CSk
    !------------------------------------------                                          !1.6CSk
    BCF.REC.NO% = 16                                                                     !1.6CSk
    RC% = READ.BCF                                                                       !1.6CSk
    IF RC% <> 0 THEN BEGIN                                                               !1.6CSk
         CALL LOG.EVENT(106)                                                             !1.6CSk
    ENDIF                                                                                !1.6CSk
    CALL DO.MESSAGE("PSB21 - BCF Rec 16 Statin markers: [" + BCF.RECORD$ + "]", FALSE)   !1.6CSk
                                                                                         !1.6CSk
    NO.OF.STATINS% = 0                                                                   !1.6CSk
    MATCH.COMMA%   = 1                                                                   !1.6CSk
                                                                                         !1.6CSk
    WHILE MATCH.COMMA% <> 0                                                              !1.6CSk
        MATCH.COMMA% = MATCH(",", BCF.RECORD$,MATCH.COMMA% + 1)                          !1.6CSk
        IF MATCH.COMMA% <> 0 THEN BEGIN                                                  !1.6CSk
           NO.OF.STATINS% = NO.OF.STATINS% + 1                                           !1.6CSk
        ENDIF                                                                            !1.6CSk
    WEND                                                                                 !1.6CSk
                                                                                         !1.6CSk
    DIM STATINS$(NO.OF.STATINS%)                                                         !1.6CSk
                                                                                         !1.6CSk
    IF NO.OF.STATINS% > 0 THEN BEGIN                                                     !1.6CSk
                                                                                         !1.6CSk
        MATCH.POS1% = 1                                                                  !1.6CSk
                                                                                         !1.6CSk
        FOR J% = 1 TO NO.OF.STATINS%                                                     !1.6CSk
                                                                                         !1.6CSk
            IF MATCH.POS1% = 1 THEN BEGIN                                                !1.6CSk
                MATCH.POS2% = MATCH(",",BCF.RECORD$,MATCH.POS1%)                         !1.6CSk
                IF MATCH.POS2% THEN BEGIN                                                !1.6CSk
                     STATIN.INDICATOR$ = LEFT$(BCF.RECORD$,MATCH.POS2% - 1)              !1.6CSk
                     MATCH.POS1% = MATCH.POS2% + 1                                       !1.6CSk
                 ENDIF ELSE BEGIN                                                        !1.6CSk
                     MATCH.POS1% = MATCH(" ", BCF.RECORD$, 1)                            !1.6CSk
                     STATIN.INDICATOR$ = LEFT$(BCF.RECORD$, MATCH.POS1%- 1)              !1.6CSk
                 ENDIF                                                                   !1.6CSk
            ENDIF ELSE BEGIN                                                             !1.6CSk
                MATCH.POS2% = MATCH(",",BCF.RECORD$,MATCH.POS1%)                         !1.6CSk
                IF MATCH.POS2% <> 0 THEN BEGIN                                           !1.6CSk
                    STATIN.INDICATOR$ = MID$(BCF.RECORD$, MATCH.POS1%,     \             !1.6CSk
                         (MATCH.POS2% - MATCH.POS1%))                                    !1.6CSk
                    MATCH.POS1% = MATCH.POS2% + 1                                        !1.6CSk
               ENDIF ELSE BEGIN                                                          !1.6CSk
                   STATIN.INDICATOR$ = RIGHT$(BCF.RECORD$,(LEN(BCF.RECORD$)\             !1.6CSk
                         - MATCH.POS1%) + 1)                                             !1.6CSk
                   MATCH.POS1% = MATCH(" ", STATIN.INDICATOR$, 1)                        !1.6CSk
                   STATIN.INDICATOR$ = LEFT$(STATIN.INDICATOR$,MATCH.POS1%-1)            !1.6CSk
               ENDIF                                                                     !1.6CSk
            ENDIF                                                                        !1.6CSk
                                                                                         !1.6CSk
            STATINS$(J%) = STATIN.INDICATOR$                                             !1.6CSk
                                                                                         !1.6CSk
        NEXT J%                                                                          !1.6CSk
                                                                                         !1.6CSk
    ENDIF                                                                                !1.6CSk
    !-----------------------------------------------------                               !1.6CSk
    ! Extact Chlamydia Test Kit markers from BCF record 18                               !1.6CSk
    !-----------------------------------------------------                               !1.6CSk
    BCF.REC.NO% = 18                                                                     !1.6CSk
    RC% = READ.BCF                                                                       !1.6CSk
    IF RC% <> 0 THEN BEGIN                                                               !1.6CSk
         CALL LOG.EVENT(106)                                                             !1.6CSk
    ENDIF                                                                                !1.6CSk
    CALL DO.MESSAGE("PSB21 - BCF Rec 18 Chlamydia Test Kit markers: [" + \               !1.6CSk
                     BCF.RECORD$ + "]", FALSE)                                           !1.6CSk
                                                                                         !1.6CSk
    CHLAMYDIA.LIST$ = BCF.RECORD$                                                        !1.6CSk
                                                                                         !1.6CSk
    NO.OF.CHLAMYDIA% = 0                                                                 !1.6CSk
    IF LEFT$(CHLAMYDIA.LIST$,1) <> " " THEN BEGIN                                        !1.6CSk
       MATCH.COMMA% = 1                                                                  !1.6CSk
       WHILE MATCH.COMMA% <> 0                                                           !1.6CSk
          MATCH.COMMA% = MATCH(",", CHLAMYDIA.LIST$, MATCH.COMMA% + 1)                   !1.6CSk
          NO.OF.CHLAMYDIA% = NO.OF.CHLAMYDIA% + 1                                        !1.6CSk
       WEND                                                                              !1.6CSk
                                                                                         !1.6CSk
       DIM CHLAMYDIA.ID$(NO.OF.CHLAMYDIA%)                                               !1.6CSk
                                                                                         !1.6CSk
    ENDIF                                                                                !1.6CSk
                                                                                         !1.6CSk
    IF NO.OF.CHLAMYDIA% > 0 THEN BEGIN                                                   !1.6CSk
       MATCH.POS1% = 1                                                                   !1.6CSk
                                                                                         !1.6CSk
       FOR J% = 1 TO NO.OF.CHLAMYDIA%                                                    !1.6CSk
                                                                                         !1.6CSk
         IF MATCH.POS1% = 1 THEN BEGIN                                                   !1.6CSk
            MATCH.POS2% = MATCH(",",CHLAMYDIA.LIST$,MATCH.POS1%)                         !1.6CSk
            IF MATCH.POS2% THEN BEGIN                                                    !1.6CSk
               CHLAM.TEMP$ = LEFT$(CHLAMYDIA.LIST$,MATCH.POS2% - 1)                      !1.6CSk
               MATCH.POS1% = MATCH.POS2% + 1                                             !1.6CSk
            ENDIF ELSE BEGIN                                                             !1.6CSk
               MATCH.POS1% = MATCH(" ", CHLAMYDIA.LIST$, 1)                              !1.6CSk
               CHLAM.TEMP$ = LEFT$(CHLAMYDIA.LIST$, MATCH.POS1% - 1)                     !1.6CSk
            ENDIF                                                                        !1.6CSk
         ENDIF ELSE BEGIN                                                                !1.6CSk
            MATCH.POS2% = MATCH(",",CHLAMYDIA.LIST$,MATCH.POS1%)                         !1.6CSk
            IF MATCH.POS2% <> 0 THEN BEGIN                                               !1.6CSk
               CHLAM.TEMP$ = MID$(CHLAMYDIA.LIST$, MATCH.POS1%,  \                       !1.6CSk
                      (MATCH.POS2% - MATCH.POS1%))                                       !1.6CSk
               MATCH.POS1% = MATCH.POS2% + 1                                             !1.6CSk
            ENDIF ELSE BEGIN                                                             !1.6CSk
               CHLAM.TEMP$ = RIGHT$(CHLAMYDIA.LIST$,(LEN(CHLAMYDIA.LIST$) \              !1.6CSk
                           - MATCH.POS1%) + 1)                                           !1.6CSk
               MATCH.POS1% = MATCH(" ", CHLAM.TEMP$, 1)                                  !1.6CSk
               CHLAM.TEMP$ = LEFT$(CHLAM.TEMP$, MATCH.POS1% - 1)                         !1.6CSk
            ENDIF                                                                        !1.6CSk
         ENDIF                                                                           !1.6CSk
                                                                                         !1.6CSk
         CHLAMYDIA.ID$(J%) = CHLAM.TEMP$                                                 !1.6CSk
                                                                                         !1.6CSk
       NEXT J%                                                                           !1.6CSk
                                                                                         !1.6CSk
    ENDIF                                                                                !1.6CSk
    !-----------------------------------------------------
    ! Extact BCF.IUF.DATETIMESTAMP$ from BCF record 20
    !-----------------------------------------------------
    BCF.REC.NO% = 20
    RC% = READ.BCF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF
    CALL DO.MESSAGE("PSB21 - BCF Rec 20 Batch Timestamp   : [" + BCF.IUF.DATETIMESTAMP$ + "]", FALSE)
    !--------------------------------------------------------                  ! ACSk
    ! Extact Gift Card Mall Product Groups from BCF record 22                  ! ACSk
    !--------------------------------------------------------                  ! ACSk
    BCF.REC.NO% = 22                                                           ! ACSk
    RC% = READ.BCF                                                             ! ACSk
    IF RC% <> 0 THEN BEGIN                                                     ! ACSk
        CALL LOG.EVENT(106)                                                    ! ACSk
    ENDIF                                                                      ! ACSk

    BCF.REC22.OFFSET.COUNT% = 1                                                ! ACSk
    GCM.PG.LIST$            = ""                                               ! ACSk
    READ.GCM.PG.NO$         = ""                                               ! ACSk
    DEFAULT.GCM.PG.NO$      = "     "                                          ! ACSk

    !Loop continues until finds a space in the BCF.RECORD$                     ! ACSk
    WHILE BCF.REC22.OFFSET.COUNT% < BCF.REC22.REC.LEN%                         ! ACSk

        READ.GCM.PG.NO$ = MID$(BCF.RECORD$,              \                     ! ACSk
                            BCF.REC22.OFFSET.COUNT%,     \                     ! ACSk
                            BCF.REC22.PROD.GROUP.LEN%)   !                     ! ACSk

        IF READ.GCM.PG.NO$ = DEFAULT.GCM.PG.NO$ THEN BEGIN                     ! ACSk
            !If a space is found, then set the loop exit condition             ! ACSk
            BCF.REC22.REC.LEN% = BCF.REC22.OFFSET.COUNT%                       ! ACSk

        ENDIF ELSE BEGIN  !Space not found - add to list                       ! ACSk

            GCM.PG.LIST$ = GCM.PG.LIST$     + \                                ! ACSk
                           READ.GCM.PG.NO$  + \                                ! ACSk
                            ","               !                                ! ACSk

           BCF.REC22.OFFSET.COUNT% = BCF.REC22.OFFSET.COUNT%   + \             ! ACSk
                                     BCF.REC22.PROD.GROUP.LEN%                 ! ACSk
        ENDIF                                                                  ! ACSk
    WEND                                                                       ! ACSk

    CALL DO.MESSAGE("PSB21 - BCF Rec 22 Gift Card Mall Product Groups: " + \   ! ACSk
                     "[" + GCM.PG.LIST$ + "]", FALSE)                          ! ACSk

    CLOSE BCF.SESS.NUM%                                             ! 1.15 CSk
    EXIT SUB                                                        ! 1.15 CSk

OPEN.BCF.ERROR:                                                     ! 1.15 CSk

    CALL LOG.EVENT(106)                                             ! 1.15 CSk

END SUB



\******************************************************************************
\***
\***    GET.SOFTS.DETAILS
\***
\******************************************************************************
SUB GET.SOFTS.DETAILS

    SOFTS.REC.62$ = ""   ! Default if record not defined
    SOFTS.REC.NUM% = 62  !MDM BLOCKED FROM SALE: RECALL IS INACTIVE;WITHDRAWN IS INACTIVE
    RC% = READ.SOFTS

    IF RC% = 0 THEN BEGIN
        SOFTS.REC.62$ = SOFTS.RECORD$ ! For use later
        CALL DO.MESSAGE("PSB21 - SOFTS Record 62:", FALSE)
        CALL DO.MESSAGE("PSB21 - [" + SOFTS.REC.62$ + "]", FALSE)
    ENDIF ELSE BEGIN
        CALL DO.MESSAGE("PSB21 *** ERROR: Unable to read SOFTS Record 62", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

    CLOSE SOFTS.SESS.NUM%

END SUB



\******************************************************************************
\***
\***    CREATE.EMPTY.PPFK.KEYED.FILE
\***
\******************************************************************************
SUB CREATE.EMPTY.PPFK.KEYED.FILE

    RC% = CREATE.PPFK

    IF RC% <> 0 THEN BEGIN
        ! Handle error - Could not create PPFK file
        CALL DO.MESSAGE("PSB21 *** ERROR cannot create PPFK", FALSE)
        FILE.OPERATION$ = "C"
        CALL LOG.EVENT(106)
    ENDIF

END SUB



\******************************************************************************
\***
\***    INITIALISATION
\***
\******************************************************************************
SUB INITIALISATION

    ON ERROR GOTO INIT.SUB.ERR

    MODULE.NUMBER$ = "0"                                                        !1.6CSk
    PHASE$         = "000"                                                      !1.6CSk

    ERROR.COUNT% = 0

    ! Initialise CONTTIME variables
    CT.PARAMBLK$       = STRING$(20, CHR$(00H))
    CT.TIMEDATE.TABLE$ = STRING$(12, CHR$(00H))

    TRUE = -1
    FALSE = 0

    CRLF$ = CHR$(13) + CHR$(10)

    RUN.TIME$ = TIME$
    RUN.DATE$ = DATE$

    IUF.BUFF.SIZE%    = 32256
    LOG.BUFF.SIZE%    = 32256
    PPFI.BUFF.SIZE%   = 32256
    PPFO.BUFF.SIZE%   = 32256
    WEEEUF.BUFF.SIZE% = 32256

    WEEE.ITEM.LIMIT%  = 16000

    MAX.PRICE.CHANGES% = 100
    MAX.BARCODES%      = 200

    IUF.ITEM.BATCH.LIMIT% = 250000 !         Maximum items which can be processed in an IUF batch.
                                   \         If value increased PPFK will need to be appropriately increased in size.
                                   \         This will also impact the W:\ RAM drive which would also need to increase in size.

    COUNT.RECORDS.IUF% = 0
    IUF.EXISTS         = FALSE

    PSB20.PATH.NAME$ = "C:\ADX_UPGM\PSB20.286"

    ERROR$ = "0"

    SKIP.IUF.PROCESSING = FALSE                                                 !1.6CSk

    ! Initialise STATUS% variable that is used by DO.MESSAGE                    !1.6CSk
    CALL ADXSERVE (ADXSERVE.RET.CODE%,26,0,MSG$)                                !1.6CSk
    IF ADXSERVE.RET.CODE% <> 0 THEN BEGIN                                       !1.6CSk
        STATUS% = 1 !FOREGROUND                                                 !1.6CSk
    ENDIF ELSE BEGIN                                                            !1.6CSk
        STATUS% = 2 !BACKGROUND                                                 !1.6CSk
    ENDIF                                                                       !1.6CSk

    USE PROCESSING.DATE$

 OVERRIDE.DATE:
    ! Calculate Run date (and day of week)
    F13.INPUT.DATE$ = PROCESSING.DATE$
    RC% = PSDATE(F13.INPUT.DATE$)

    IF RC% <> 0 THEN BEGIN ! Event 18 already logged by PSBF13                  !1.6CSk
        CALL DO.MESSAGE("PSB21 *** FATAL ERROR - PSDATE call returned " + \     !1.6CSk
                        "non-zero code: " + STR$(RC%), FALSE)                   !1.6CSk
        CALL LOG.EVENT(84)                                                      !1.6CSk
    ENDIF                                                                       !1.6CSk

    CALL DETERMINE.LOG.FILE

!                   >1234567890123456789012345678901234567890123456<  46 character display ! 1.18 RC (237)
    CALL DO.MESSAGE("PSB21 PROGRAM START    Core Release 2    vI       04-11-2016", TRUE)  ! IRG
    WAIT ; 750 ! Allow message to be read                                                  ! 1.18 RC (237)
    CALL DO.MESSAGE("============================================================", FALSE) ! 1.18 RC (237)

    !----------------------------
    ! Set up JOBSOK File at start
    !----------------------------
    CALL JOBSOK.SET
    FILE.OPERATION$ = "O"
    CALL SESS.NUM.UTILITY("O", JOBSOK.REPORT.NUM%, JOBSOK.FILE.NAME$)
    JOBSOK.SESS.NUM% = F20.INTEGER.FILE.NO%

    CALL DO.MESSAGE("PSB21 Opening JOBSOK", FALSE)
    CURRENT.REPORT.NUM% = JOBSOK.REPORT.NUM%
    IF END # JOBSOK.SESS.NUM% THEN OPEN.JOBSOK.ERROR
    OPEN JOBSOK.FILE.NAME$ RECL JOBSOK.RECL% AS JOBSOK.SESS.NUM% LOCKED NODEL
    GOTO SKIP.JOSBOK

 OPEN.JOBSOK.ERROR:
    CALL DO.MESSAGE("PSB21 *** ERROR - JOBSOK does not exist", FALSE)
    CALL LOG.EVENT(106)

 SKIP.JOSBOK:

    JOBSOK.FLAG$ = " "

    ! Get store no.
    CALL ADXSERVE(ADXSERVE.RETURN.CODE%, 4, 0, APPLICATION.STATUS.INFO$)

    IF ADXSERVE.RETURN.CODE% <> 0 THEN BEGIN
        CALL LOG.EVENT(23)
    ENDIF

    STORE.NUMBER$ = LEFT$(APPLICATION.STATUS.INFO$,4)

    CALL DO.MESSAGE("PSB21 IUF Processing for Store: " + STORE.NUMBER$, FALSE)
    CALL DO.MESSAGE("PSB21 Initialisation", FALSE)
    CALL DO.MESSAGE("PSB21 Passed PROCESSING.DATE$: " + FORMAT.DATE$(PROCESSING.DATE$), FALSE)
    CALL DO.MESSAGE("PSB21                     Day: " + F13.DAY$, FALSE)
    CALL DO.MESSAGE("PSB21 Actual Run Date        : " + FORMAT.DATE$(DATE$), FALSE)
    CALL DO.MESSAGE("PSB21            Time        : " + FORMAT.TIME$(RUN.TIME$), FALSE)
    CALL DO.MESSAGE("PSB21 PHASE 0", FALSE)
    PHASE$ = "0.1"                                                          !1.6CSk
    CALL DO.MESSAGE("PSB21 0.1 - Set, open and create files", FALSE)

    CALL SET.AND.ALLOCATE.FILE.SESSION.NUMBERS
    CALL OPEN.FILES
    CALL CREATE.FILES
    CALL GET.BCF.DETAILS
    CALL GET.SOFTS.DETAILS

    DIM CURR.RPD.NO$(PPFK.PCR.MAX%)
    DIM CURR.DATE.DUE$(PPFK.PCR.MAX%)
    DIM CURR.PRICE$(PPFK.PCR.MAX%)
    DIM CURR.INC.DEC.FLAG$(PPFK.PCR.MAX%)
    DIM CURR.STATUS.FLAG$(PPFK.PCR.MAX%)
    DIM CURR.MARKDOWN$(PPFK.PCR.MAX%)

    DIM PREV.RPD.NO$(PPFK.PCR.MAX%)
    DIM PREV.DATE.DUE$(PPFK.PCR.MAX%)
    DIM PREV.PRICE$(PPFK.PCR.MAX%)
    DIM PREV.INC.DEC.FLAG$(PPFK.PCR.MAX%)
    DIM PREV.STATUS.FLAG$(PPFK.PCR.MAX%)
    DIM PREV.MARKDOWN$(PPFK.PCR.MAX%)

    DIM SORT.TABLE$(PPFK.PCR.MAX%)                             ! 1.10 RC (113)

    !----------------------------------
    ! Check IUF header record is valid
    !----------------------------------

    IF VALIDATE.IUF.HEADER.RECORD <> "ACK" THEN BEGIN
    ! Log message against which field is invalid and stop application
       CALL DO.MESSAGE("PSB21 IUF header - program stopped: " +                    \
                       VALIDATE.IUF.HEADER.RECORD,FALSE)
      CURRENT.REPORT.NUM% = IUF.REPORT.NUM%
      IF NOT MATCH("UNABLE TO OPEN",VALIDATE.IUF.HEADER.RECORD,1) THEN BEGIN
          ERROR$ = "2" ! Header problem
          JOBSOK.FLAG$ = "X"
          IUF.EXISTS = TRUE
          CALL SEND.DEC.MESSAGE
          CALL PROGRAM.EXIT
      ENDIF ELSE BEGIN
          ERROR$ = "1" ! Missing IUF file
      ENDIF
    ENDIF

    ! Create empty PPFK keyed file
    CALL CREATE.EMPTY.PPFK.KEYED.FILE

!   Call PSBF18 to read SOPTS and set F18.SOPTS.READ$ to "Y"  ! 1.19 RC
    RC% = CALC.BOOTS.CODE.CHECK.DIGIT("123456")               ! 1.19 RC

EXIT SUB

INIT.SUB.ERR:

    IF ERR = "NP" THEN BEGIN
         PROCESSING.DATE$ = RUN.DATE$
         RESUME OVERRIDE.DATE
        !PRINT "Do NOT run from command mode"
        !STOP
    ENDIF

END SUB

\******************************************************************************
\***
\***  LOAD.WEEEUF.FILE.INTO.TABLE
\***
\***  This routine loads the entire WEEEUF file into a table - no need to sort
\***  the table since the file is in ascending item code order.
\***
\******************************************************************************

SUB LOAD.WEEEUF.FILE.INTO.TABLE

    PHASE$ = "1.0"                                                              !1.6CSk
    CALL DO.MESSAGE("PSB21 PHASE 1", TRUE)
    CALL DO.MESSAGE("PSB21 1.0 - LOAD.WEEEUF.FILE.INTO.TABLE", TRUE)

    DIM WEEEUF.TABLE$(WEEE.ITEM.LIMIT%)

    WEEE.ITEM.COUNT% = 0
    FILE.OPERATION$  = "O"

    IF END# WEEEUF.SESS.NUM% THEN NO.WEEEUF.FILE
    OPEN WEEEUF.FILE.NAME$ AS WEEEUF.SESS.NUM% BUFFSIZE WEEEUF.BUFF.SIZE% NOWRITE NODEL

    RC% = READ.WEEEUF

    WHILE RC% = 0
        WEEE.ITEM.COUNT% = WEEE.ITEM.COUNT% + 1

        IF WEEE.ITEM.COUNT% <= WEEE.ITEM.LIMIT% THEN BEGIN
            ! 7-digit Boots Item Code followed by 4-digit recycling fee
            WEEEUF.TABLE$(WEEE.ITEM.COUNT%) = WEEE.ITEM.CODE$ + RIGHT$("0000" + STR$(WEEE.ITEM.PRF.AMT%), 4)
        ENDIF

        RC% = READ.WEEEUF
    WEND

    IF WEEE.ITEM.COUNT% > WEEE.ITEM.LIMIT% THEN BEGIN
        ! Should never reach this limit but added this check just in case.
        CALL DO.MESSAGE("PSB21 *** ERROR: WEEE Item Limit exceeded. Item count: " + STR$(WEEE.ITEM.COUNT%), FALSE)
        CALL DO.MESSAGE("           setting WEEE Item Limit to          : " + STR$(WEEE.ITEM.LIMIT%), FALSE)
        WEEE.ITEM.COUNT% = WEEE.ITEM.LIMIT%
    ENDIF

    CLOSE WEEEUF.SESS.NUM%

 NO.WEEEUF.FILE:

    CALL DO.MESSAGE("PSB21 1.1 - "+ STR$(WEEE.ITEM.COUNT%) + " WEEE Items Loaded", TRUE)

END SUB


\******************************************************************************
\***
\***    STORE.ITEM.DETAIL
\***
\******************************************************************************
SUB STORE.ITEM.DETAIL

    !-----------------------------------------------------                    ! ACSk
    ! If the item's product group is in the list of Gift                      ! ACSk
    ! Card Mall product groups, then override the settings                    ! ACSk
    !-----------------------------------------------------                    ! ACSk
    IF MATCH(IUF.PROD.GRP$, GCM.PG.LIST$, 1) <> 0 THEN BEGIN                  ! ACSk
        IUF.GIFTCARD$   = "Y"                                                 ! ACSk
       !IUF.RETURNABLE$ = "N"  ! Now set directly in UPDATE.IRF               ! BCSk
    ENDIF                                                                     ! ACSk

    CURR.AGE.RESTRICTION$              = IUF.AGE.RESTRICTION$
    CURR.BC.LETTER$                    = IUF.BC.LETTER$
    CURR.BLOCKED.FROM.SALE$            = IUF.BLOCKED.FROM.SALE$
    CURR.BOOTS.CODE$                   = IUF.BOOTS.CODE$
    CURR.BOOTS.COM.EXTENDED$           = IUF.BOOTS.COM.EXTENDED$
    CURR.CONTAINS.ALCOHOL$             = IUF.CONTAINS.ALCOHOL$
    CURR.CONTAINS.ASPIRIN$             = IUF.CONTAINS.ASPIRIN$
    CURR.CONTAINS.EPHEDRINE$           = IUF.CONTAINS.EPHEDRINE$
    CURR.CONTAINS.IBUPROFEN$           = IUF.CONTAINS.IBUPROFEN$
    CURR.CONTAINS.NONSOLID.PAINKILLER$ = IUF.CONTAINS.NONSOLID.PAINKILLER$
    CURR.CONTAINS.PARACETAMOL$         = IUF.CONTAINS.PARACETAMOL$
    CURR.CONTAINS.PSEUDOEPHEDRINE$     = IUF.CONTAINS.PSEUDOEPHEDRINE$
    CURR.CURRENT.PRICE$                = IUF.CURRENT.PRICE$
    CURR.DATE.SENSITIVE$               = IUF.DATE.SENSITIVE$
    CURR.DISCOUNTABLE$                 = IUF.DISCOUNTABLE$ ! Reinstated original variable name ! 1.6 RC (21)
    CURR.EARN.POINTS$                  = IUF.EARN.POINTS$
    CURR.ENF.PRICE.ENTRY$              = IUF.ENF.PRICE.ENTRY$
    CURR.ETHICAL.ACTIVE$               = IUF.ETHICAL.ACTIVE$
    CURR.ETHICAL.CLASS$                = IUF.ETHICAL.CLASS$
    CURR.ETHICAL.DESCRIPTION$          = IUF.ETHICAL.DESCRIPTION$
    CURR.ETHICAL.PACK.SIZE$            = IUF.ETHICAL.PACK.SIZE$
    CURR.GIFTCARD$                     = IUF.GIFTCARD$
    CURR.GIVEAWAY$                     = IUF.GIVEAWAY$
    CURR.GRP.CODE.FLAG$                = IUF.GRP.CODE.FLAG$
    CURR.GRP.NO$                       = IUF.GRP.NO$                    !HRN
    CURR.GUARANTEE.LENGTH$             = IUF.GUARANTEE.LENGTH$
    CURR.INSURANCE$                    = IUF.INSURANCE$
    CURR.ITEM.QTY$                     = IUF.ITEM.QTY$
    CURR.OWN.BRAND$                    = IUF.OWN.BRAND$
    CURR.PRIMARY.SUPPLIER$             = IUF.PRIMARY.SUPPLIER$
    CURR.PROD.GRP$                     = IUF.PROD.GRP$
    CURR.REDEEMABLE$                   = IUF.REDEEMABLE$
    CURR.RESALEABLE$                   = IUF.RESALEABLE$
    CURR.RESTRICT.SALES.FLAG$          = IUF.RESTRICT.SALES.FLAG$       !HRN
    CURR.RETURN.ROUTE$                 = IUF.RETURN.ROUTE$
    CURR.RETURNABLE$                   = IUF.RETURNABLE$
    CURR.S.E.DESC$                     = IUF.S.E.DESC$
    CURR.SPECIAL.INSTRUCTION$          = IUF.SPECIAL.INSTRUCTION$
    CURR.STATUS.1$                     = IUF.STATUS.1$
    CURR.STNDRD.DESC$                  = IUF.STNDRD.DESC$
    CURR.STOCK.SYSTEM.FLAG$            = IUF.STOCK.SYSTEM.FLAG$
    CURR.SUPPLY.ROUTE$                 = IUF.SUPPLY.ROUTE$
    CURR.TILL.DESC$                    = IUF.TILL.DESC$
    CURR.UNIT.MEASUREMENT$             = IUF.UNIT.MEASUREMENT$
    CURR.UNIT.NAME$                    = IUF.UNIT.NAME$
    CURR.UNRESTRICTED.GROUP.CODE$      = IUF.UNRESTRICTED.GROUP.CODE$

END SUB

\******************************************************************************
\***
\***    CHECK.IF.CURR.SORT.NEEDED(M%)
\***
\******************************************************************************

SUB CHECK.IF.CURR.SORT.NEEDED(M%) ! Entire procedure new for Rv 1.10 RC (113)

    INTEGER*2 M%

!   No need to sort CURR tables for single price change
    IF M% <= 1 THEN BEGIN
        EXIT SUB
    ENDIF

!   An items IUF price change records need to be in ascending  ! 1.12 RC (177)
!   date-RPD order (latest last)                               ! 1.12 RC (177)

!   When an items current IUF price change record date-RPD is
!   earlier than its previous IUF price change record date-RPD ! 1.12 RC (177)
!   then the items IUF price change record data needs sorting
    IF  CURR.DATE.DUE$(M%)    + CURR.RPD.NO$(M%) \
      < CURR.DATE.DUE$(M% -1) + CURR.RPD.NO$(M% -1) THEN BEGIN ! 1.12 RC (177)
        CURR.SORT.NEEDED = TRUE
    ENDIF

END SUB


\******************************************************************************
\***
\***    STORE.PRICE.CHANGE(N%)
\***
\******************************************************************************
SUB STORE.PRICE.CHANGE(N%)

    INTEGER*2 N%

    ! Add price change to the price change tables
    CURR.RPD.NO$(N%)       = PACK$("0" + IUF.RPD.NO$)
!   Convert 8-digit IUF.RPD.DATE$ (ASCII) to 6-digit CURR.DATE.DUE$ (packed)  ! 1.6 RC (90)
    CURR.DATE.DUE$(N%)     = RIGHT$(PACK$(IUF.RPD.DATE$),3)                   ! 1.6 RC (90)
    CURR.PRICE$(N%)        = PACK$(IUF.NEW.PRICE$)
    CURR.INC.DEC.FLAG$(N%) = " "
    CURR.STATUS.FLAG$(N%)  = " "
    CURR.MARKDOWN$(N%)     = IUF.MARKDOWN$

    IF UNPACK$(CURR.DATE.DUE$(N%)) = PROCESSING.DATE$ THEN BEGIN ! 1.10 RC (113)
        PRICE.CHANGE.TODAY = TRUE                                ! 1.10 RC (113)
    ENDIF                                                        ! 1.10 RC (113)

    CALL CHECK.IF.CURR.SORT.NEEDED(N%)                           ! 1.10 RC (113)

END SUB


\******************************************************************************
\***
\***    STORE.EMERG.PRICE.CHANGE(N%)
\***
\******************************************************************************

SUB STORE.EMERG.PRICE.CHANGE(N%) PUBLIC ! Entire procedure new for Rv 1.10 RC (113)
                                        ! Based on STORE.PRICE.CHANGE(N%)
    INTEGER*2 N%

!   Add emergency price change to CURR (IUF) data tables
    CURR.RPD.NO$(N%)       = PACK$("099999")
    CURR.DATE.DUE$(N%)     = PACK$(PROCESSING.DATE$) ! From PSB20             !
    CURR.PRICE$(N%)        = PACK$(RIGHT$("00000000" + \
                               CURR.CURRENT.PRICE$, 8)) ! From IUF
    CURR.INC.DEC.FLAG$(N%) = " " ! PSB23 sets to Increase / Decrease
    CURR.STATUS.FLAG$(N%)  = " " ! SEL not yet printed
    CURR.MARKDOWN$(N%)     = "N" ! Markdown flag not used

    CALL CHECK.IF.CURR.SORT.NEEDED(N%)

END SUB


\******************************************************************************
\***
\***    STORE.BARCODE
\***
\******************************************************************************
\*** Add barcode to the barcode table but only if it doesn't match the Item Code
\*** in the "I" Item record (excluding check digit). This is because the first
\*** IDF barcode is ALWAYS the Boots Item Code (less check digit), so we ignore
\*** it now instead of later.
\***
\******************************************************************************
SUB STORE.BARCODE

    IF LEFT$(IUF.BAR.CODE$, 12) <> ("000000" + LEFT$(IUF.BOOTS.CODE$, 6)) THEN BEGIN

        BARCODE.COUNT% = BARCODE.COUNT% + 1

        BARCODE.TABLE$(BARCODE.COUNT%) = LEFT$(IUF.BAR.CODE$, 12) ! Remove Check Digit

    ENDIF

END SUB


\******************************************************************************
\***
\***    DO.IUF.PROGRESS
\***
\******************************************************************************
SUB DO.IUF.PROGRESS

    INTEGER*2 STEP%

    IF IUF.RECORD.COUNT% < 21 AND IUF.NEW.FORMAT THEN BEGIN
        STEP% = 100 / IUF.RECORD.COUNT%
    ENDIF ELSE BEGIN
        STEP% = 5
    ENDIF

    IF IUF.NEW.FORMAT THEN BEGIN
        PROGRESS.COUNT% = PROGRESS.COUNT% + 1

        IF PROGRESS.COUNT% >= RECS.PER.5% THEN BEGIN

            PHASE$ = "3.2"                                                      !1.6CSk
            PERCENT.VAL% = PERCENT.VAL% + STEP%
            IF IUF.NEW.FORMAT AND PERCENT.VAL% < 101 THEN BEGIN
                CALL DO.MESSAGE("PSB21 PHASE 3.2 - PROCESS.IUF: " + STR$(PERCENT.VAL%) + "%", TRUE)
            ENDIF ELSE BEGIN
                CALL DO.MESSAGE("PSB21 PHASE 3.2 - PROCESS.IUF: PROCESSING...", FALSE)
            ENDIF

            PROGRESS.COUNT% = 0
        ENDIF
    ENDIF
END SUB


\******************************************************************************
\***
\***    UNEXPECTED.RECORD.TYPE
\***
\******************************************************************************
\***
\***    This subroutine logs an event and writes an entry to the Daily Log
\***    file if an unrecognised or unexpected record type is found. It then
\***    skips to the next Item, Trailer, Batch or End-Of-File.
\***
\******************************************************************************

SUB UNEXPECTED.RECORD.TYPE

    INTEGER*1   JUMP.OUT,    \
                SKIPPED

    CALL DO.MESSAGE("PSB21 *** ERROR1: Unexpected IUF Rec Type: [" + \
                    IUF.REC.TYPE$ + "] at Rec: " + STR$(COUNT.RECORDS.IUF%), FALSE)

!   When unexpeted record type encountered on legacy IUF        ! 1.15 RC (223)
!   end program with JOBSOK.FLAG$ "X"                           ! 1.15 RC (223)
    IF NOT IUF.NEW.FORMAT THEN BEGIN ! Legacy IUF               ! 1.15 RC (223)
       JOBSOK.FLAG$ = "X"                                       ! 1.15 RC (223)
!      CALL SEND.DEC.MESSAGE ! No DEC message for legacy IUF    ! 1.15 RC (223)
       CALL CLOSE.IUF                                           ! 1.15 RC (223)
       CALL PROGRAM.EXIT                                        ! 1.15 RC (223)
    ENDIF                                                       ! 1.15 RC (223)

    CALL DO.MESSAGE("PSB21 *** Skipping to next Item(I)/Trailer(T)/Batch(H)/End-Of-File", FALSE)

    IUF.J103.COUNT% = IUF.J103.COUNT% + 1

    SKIPPED  = FALSE
    JUMP.OUT = FALSE

    ! Get next IUF record
    RC% = READ.IUF

    ! IF we couldn't read the next IUF record
    IF RC% <> 0 THEN BEGIN
        ! Handle error - Unexpected read error
        CALL DO.MESSAGE("PSB21 *** ERROR2: End of IUF File Found", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

    COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
    CALL DO.IUF.PROGRESS

    ! Continue reading until next Item(I), Trailer(T), Batch(H) or End-Of-File.
    WHILE NOT JUMP.OUT

        IF IUF.REC.TYPE$ = "I"  OR \
           IUF.REC.TYPE$ = "H"  OR \
           IUF.REC.TYPE$ = "T" THEN BEGIN

            ! Jump out - found next Item/Header/EOF
            SKIPPED = TRUE
            JUMP.OUT = TRUE
        ENDIF ELSE BEGIN

            RC% = READ.IUF

            ! IF we couldn't read the next IUF record
            IF RC% <> 0 THEN BEGIN
                ! Handle error - Unexpected read error
                CALL DO.MESSAGE("PSB21 *** ERROR3: reading next IUF Record at: " + \
                         STR$(COUNT.RECORDS.IUF%), FALSE)
                CALL LOG.EVENT(106)
                EXIT SUB
            ENDIF

            COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
            CALL DO.IUF.PROGRESS
        ENDIF
    WEND

    PROCESS.ITEM = FALSE

END SUB


\*******************************************************************************
\***
\*** VALIDATE.IUF.FIELDS
\***
\***
\***   processing date.
\***
\*******************************************************************************
SUB VALIDATE.IUF.FIELDS

     !Validate item record
     IF IUF.REC.TYPE$ = "I" THEN BEGIN

         IF MATCH("!",IUF.BOOTS.CODE$,1) OR LEN(IUF.BOOTS.CODE$) <> 7 THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " 'I':" + "[" + IUF.BOOTS.CODE$ + "]"                         +  \
                             ":Item Boots code not valid format",FALSE)

         ENDIF ELSE IF MATCH("!",IUF.PROD.GRP$,1) OR LEN(IUF.PROD.GRP$) <> 5 THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 09
                             " 'I':" + "[" + IUF.PROD.GRP$ + "]"                           +  \
                             ":Item product group not valid format",FALSE)

         ENDIF ELSE IF MATCH("!",IUF.CURRENT.PRICE$,1) OR IUF.CURRENT.PRICE$ = ""   THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 17
                             " 'I':" + "[" + IUF.CURRENT.PRICE$ + "]"                      +  \
                             ":Item price not valid format",FALSE)

         ENDIF ELSE IF MATCH("#",IUF.BC.LETTER$,1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 19
                             " 'I'" + "[" + IUF.BC.LETTER$ + "]"                           +  \
                             ":Item business centre letter not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.SUPPLY.ROUTE$,"WDB",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 07
                            " 'I' " + "[" + IUF.SUPPLY.ROUTE$ + "]"                        +  \
                             ":Item supply route not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.OWN.BRAND$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 15
                             " 'I' " + "[" + IUF.OWN.BRAND$ + "]"                          +  \
                             ":Item own brand flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.ENF.PRICE.ENTRY$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 11
                             " 'I' " + "[" + IUF.ENF.PRICE.ENTRY$ + "]"                    +  \
                             ":Item enforced entry flag not valid format",FALSE)

!        ENDIF ELSE IF NOT MATCH(IUF.GRP.CODE.FLAG$,"NY",1) THEN BEGIN                          ! 1.6 RC

!            Should never arise ...                                                             ! 1.6 RC
!            READ.IUF sets IUF.GRP.CODE.FLAG" to "N" if it is not "Y"      ! 1.6 RC  Matrix 03  ! 1.6 RC

!            CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \ ! 1.6 RC  Matrix 03
!                            " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC
!                            " 'I' " + "[" + IUF.GRP.CODE.FLAG$ + "]"                      +  \ ! 1.6 RC
!                            ":Item group code flag not valid format",FALSE)                    ! 1.6 RC

         ENDIF ELSE IF NOT MATCH(IUF.DATE.SENSITIVE$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 23
                             " 'I' " + "[" + IUF.DATE.SENSITIVE$ + "]"                     +  \
                             ":Item date sensitive flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.ALCOHOL$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 33
                             " 'I' " + "[" + IUF.CONTAINS.ALCOHOL$ + "]"                   +  \
                             ":Item alcohol flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.PARACETAMOL$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 34
                             " 'I' " + "[" + IUF.CONTAINS.PARACETAMOL$ + "]"               +  \
                             ":Item paracetamol flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.ASPIRIN$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 35
                             " 'I' " + "[" + IUF.CONTAINS.ASPIRIN$ + "]"                   +  \
                             ":Item aspirins flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.IBUPROFEN$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 36
                             " 'I' " + "[" + IUF.CONTAINS.IBUPROFEN$ + "]"                 +  \
                             ":Item ibuprofen flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.EPHEDRINE$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 37
                             " 'I' " + "[" + IUF.CONTAINS.EPHEDRINE$ + "]"                 +  \
                             ":Item ephedrine flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.CONTAINS.NONSOLID.PAINKILLER$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 39
                             " 'I' " + "[" + IUF.CONTAINS.NONSOLID.PAINKILLER$ + "]"       +  \
                             ":item non solid painkiller flag not valid format",FALSE)

!        Rejection of invalid IUF.UNRESTRICTED.GROUP.CODE$ (ie, not "N" or "Y") was             ! 1.6 RC (71b)
!        previously removed and then re-instated (and corrected to remove the MATCH)            ! 1.6 RC (71b)

!        ENDIF ELSE IF NOT MATCH(IUF.UNRESTRICTED.GROUP.CODE$,"NY",1) THEN BEGIN                ! 1.6 RC (71)

!            Should never arise ...                                                             ! 1.6 RC (71)
!            READ.IUF sets IUF.UNRESTRICTED.GROUP.CODE$ to "N" if it is not "Y"                 ! 1.6 RC (71)

!            CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \ ! 1.6 RC (71)
!                            " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 40
!                            " 'I' " + "[" + IUF.UNRESTRICTED.GROUP.CODE$ + "]"            +  \ ! 1.6 RC (71)
!                            ":Item unrestricted group flag not valid format",FALSE)

         ENDIF ELSE IF (    IUF.UNRESTRICTED.GROUP.CODE$ <> "N" \                               ! 1.6 RC (71b)
                        AND IUF.UNRESTRICTED.GROUP.CODE$ <> "Y") THEN BEGIN                     ! 1.6 RC (71b)

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \ ! 1.6 RC (71b)
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC (71b) Matrix 40
                             " 'I' " + "[" + IUF.UNRESTRICTED.GROUP.CODE$ + "]"            +  \ ! 1.6 RC (71b)
                             ":Item unrestricted group flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.BOOTS.COM.EXTENDED$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 43
                             " 'I' " + "[" + IUF.BOOTS.COM.EXTENDED$ + "]"                 +  \
                             ":Item Boots.com extended flag not valid format",FALSE)

         ENDIF ELSE IF NOT MATCH(IUF.INSURANCE$,"NY",1) THEN BEGIN

             CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                             " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 44
                             " 'I' " + "[" + IUF.INSURANCE$ + "]"                          +  \
                             ":Item insurance flag not valid format",FALSE)

         ENDIF ELSE BEGIN
             EXIT SUB
         ENDIF

         IUF.J103.COUNT% = IUF.J103.COUNT% + 1
         !Skip to next item
         PROCESS.ITEM = FALSE

     !Validate Price record
     ENDIF ELSE IF IUF.REC.TYPE$ = "P" THEN BEGIN

         IF PROCESS.ITEM THEN BEGIN

             IF MATCH("!",IUF.BOOTS.CODE$,1) OR LEN(IUF.BOOTS.CODE$) <> 7 THEN BEGIN

                 CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                 " 'P' " + "[" + IUF.BOOTS.CODE$ + "]"                         +  \
                                 ":Item Boots code not valid format",FALSE)

             ENDIF ELSE IF IUF.BOOTS.CODE$ <> CURR.BOOTS.CODE$ THEN BEGIN

                 CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                 " 'P' " + "[" + IUF.BOOTS.CODE$ + "]"                         +  \
                                 ":Item Boots code out of sequence",FALSE)

             ENDIF ELSE IF MATCH("!",IUF.RPD.DATE$,1) OR LEN(IUF.RPD.DATE$) <> 8  THEN BEGIN      !  1.1MG 1.3CSk
                                                                                                  !
                 CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                 " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 03
                                 " 'P' " + "[" + IUF.RPD.DATE$ + "]"                           +  \
                                 ":Item RPD date not valid format",FALSE)

             ENDIF ELSE IF LEN(IUF.RPD.NO$) <> 5 OR (VAL(IUF.RPD.NO$) < 10001 OR \
                           VAL(IUF.RPD.NO$) > 99996)  THEN BEGIN

                 CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                 " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 04
                                 " 'P' " + "[" + IUF.RPD.NO$ + "]"                             +  \
                                 ":Item RPD number not valid format",FALSE)

             ENDIF ELSE IF MATCH("!",IUF.NEW.PRICE$,1) THEN BEGIN

                 CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                 " (item " + IUF.BOOTS.CODE$ + ")"                             +  \ ! 1.6 RC  Matrix 05
                                 " 'P' " + "[" + IUF.NEW.PRICE$ + "]"                          +  \
                                 ":Item new price not valid format",FALSE)

             ENDIF ELSE BEGIN

!               Keep IUF.RPD.DATE$ as 8-digits until loaded into CURR.DATE.DUE$ table        ! 1.6 RC (90)
! 1.6 RC (90)   ! Reduced to 6 digits for processing                                         ! 1.6 RC (90)
! 1.6 RC (90)   IUF.RPD.DATE$ = RIGHT$(IUF.RPD.DATE$,6)                                      ! 1.6 RC (90)
                EXIT SUB
             ENDIF

             IUF.J103.COUNT% = IUF.J103.COUNT% + 1
             !Skip to next item
             PROCESS.ITEM = FALSE                                                                 !  1.1MG

         ENDIF

     !Validate Barcode record
     ENDIF ELSE IF IUF.REC.TYPE$ = "B" THEN BEGIN

         IF PROCESS.ITEM THEN BEGIN

             IF MATCH("!",IUF.BOOTS.CODE$,1) OR LEN(IUF.BOOTS.CODE$) <> 7 THEN BEGIN

                CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                " 'B' " + "[" + IUF.BOOTS.CODE$ + "]"                         +  \
                                ":Item Boots code not valid format",FALSE)

             ENDIF ELSE IF IUF.BOOTS.CODE$ <> CURR.BOOTS.CODE$ THEN BEGIN
                CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                " 'B' " + "[" + IUF.BOOTS.CODE$ + "]"                         +  \
                                ":Item Boots code out of sequence",FALSE)

             ENDIF ELSE IF MATCH("!",IUF.BAR.CODE$,1)  THEN BEGIN                             ! removed length validation

                CALL DO.MESSAGE("PSB21 *** ERROR Record " + STR$(COUNT.RECORDS.IUF%)          +  \
                                " 'B' " + "[" + IUF.BAR.CODE$ + "]"                           +  \
                                ":Item bar code not valid format",FALSE)

             ENDIF ELSE BEGIN
                 EXIT SUB
             ENDIF

             IUF.J103.COUNT% = IUF.J103.COUNT% + 1
             !Skip to next item
             PROCESS.ITEM = FALSE

         ENDIF

     !Validate header record fields
     ENDIF ELSE IF IUF.REC.TYPE$ = "H" THEN BEGIN

         !Validate store number format
         IF MATCH("!",IUF.STORE.NUM$,1) OR LEN(IUF.STORE.NUM$) <> 4 THEN BEGIN

              CALL DO.MESSAGE("PSB21 *** ERROR: Store number not valid format - Exit program",FALSE)

              !Validate Initial load flag
         ENDIF ELSE IF NOT MATCH(IUF.INITIAL.LOAD$,"NY",1) THEN BEGIN

              CALL DO.MESSAGE("PSB21 *** ERROR: Initial load flag not valid format - Exit program",FALSE)

              !Validate Date TIme Stamp format
         ENDIF ELSE IF MATCH("!",IUF.TIME.STAMP$,1) OR LEN(IUF.TIME.STAMP$) <> 17 THEN BEGIN

              CALL DO.MESSAGE("PSB21 *** ERROR: Date Time Stamp not valid format - Exit program",FALSE)

         ENDIF ELSE BEGIN
             EXIT SUB
         ENDIF

         ERROR$ = "4"                             ! Header problem

         JOBSOK.FLAG$ = "X"                                           ! 1.4 RC
         CALL PROGRAM.EXIT

     ENDIF

END SUB


SUB REPORT.SKIPPED.ITEM.CODE

    CALL DO.MESSAGE("PSB21 *** ERROR: Skipped item was " + \
                                        MID$(IUF.BOOTS.CODE$,1,2) + "-" + \
                                        MID$(IUF.BOOTS.CODE$,3,2) + "-" + \
                                        MID$(IUF.BOOTS.CODE$,5,3) \
                                                                  ,FALSE)

END SUB

\*******************************************************************************
\***
\*** PROCESS.ITEM.CHECK
\***
\*******************************************************************************

SUB PROCESS.ITEM.CHECK

    INTEGER*4 GET.BATCH.NUMBER%
    INTEGER*4 CHECK.ELEMENT%

    CHECK.ELEMENT% = VAL(LEFT$(IUF.BOOTS.CODE$,6))

    CALL I4.LIST.GET(CHECK.ELEMENT%, GET.BATCH.NUMBER%)

    IF IUF.BATCH.COUNT% < GET.BATCH.NUMBER% THEN BEGIN

       PROCESS.ITEM = FALSE

    ENDIF

END SUB

\*******************************************************************************
\***
\*** MAXIMUM.ITEM.LOG.MESSAGE
\***
\***
\*******************************************************************************

SUB MAXIMUM.ITEM.LOG.MESSAGE

     CALL DO.MESSAGE("PSB21 Max items in batch reached"           +           \
                     " (> " + STR$(IUF.ITEM.BATCH.LIMIT%)         +           \
                     " items) from record " + STR$(COUNT.RECORDS.IUF%),FALSE)

END SUB


\******************************************************************************
\***
\***    UPDATE.SUPPARAM
\***
\******************************************************************************
\***
\***    This subroutine updates the SUPPARAM file.
\***    If batch was an initial load then open SUPPARAM and update the initial
\***    load date to today in order to configure label suppression.
\***
\******************************************************************************
SUB UPDATE.SUPPARAM

    FILE.OPERATION$ = "O"
    IF END #SUPPARAM.SESS.NUM% THEN SUPPARAM.FILE.ERROR
    OPEN SUPPARAM.FILE.NAME$ DIRECT RECL SUPPARAM.RECL% AS SUPPARAM.SESS.NUM%

    RC% = READ.SUPPARAM

    IF RC% <> 0 THEN BEGIN
        CALL DO.MESSAGE("PSB21 *** ERROR Unable to read SUPPARAM file", FALSE)
        CALL LOG.EVENT(106)
    ENDIF ELSE BEGIN
        SUPPARAM.INITIAL.LOAD.DATE$ = PROCESSING.DATE$
        RC% = WRITE.SUPPARAM
        IF RC% <> 0 THEN BEGIN
            CALL DO.MESSAGE("PSB21 *** ERROR Unable to write to SUPPARAM file", FALSE)
            CALL LOG.EVENT(106)
        ENDIF
    ENDIF

    CLOSE SUPPARAM.SESS.NUM%
    EXIT SUB

 SUPPARAM.FILE.ERROR:
    CALL DO.MESSAGE("PSB21 *** ERROR Unable to open SUPPARAM file", FALSE)
    CALL LOG.EVENT(106)

END SUB


\******************************************************************************
\***
\***    SORT.CURR.TABLES
\***    Sorts CURR data tables into descending date-RPD order
\***
\******************************************************************************

SUB SORT.CURR.TABLES PUBLIC ! Entire procedure new for Rv 1.10 (113)

    INTEGER*2 SUB%
    INTEGER*2 SORT.LIMIT.SUB%

!   An items IUF price change records need to have ascending   ! 1.12 RC (177)
!   date-RPD order to facilitate merging into PPFI data.
!   Legacy M/F systems may currently transmit data in the      ! 1.12 RC (177)
!   opposite order however this is not fully certain.          ! 1.12 RC (177)
!   Core R2 (ECC format) IUF price changes will have no
!   sequencing.

!   Combine all CURR data tables into SORT.TABLE$ ready for sorting

    SUB% = 1

    WHILE SUB% <= CURR.PCR.COUNT%
        SORT.TABLE$(SUB% -1) = \
          CURR.DATE.DUE$(SUB%)     + \ ! Major sort field - YYMMDD
          CURR.RPD.NO$(SUB%)       + \ ! Minor sort field - 6 digit RPD number
          CURR.PRICE$(SUB%)        + \
          CURR.INC.DEC.FLAG$(SUB%) + \
          CURR.STATUS.FLAG$(SUB%)  + \
          CURR.MARKDOWN$(SUB%)
        SUB% = SUB% + 1
    WEND

    SORT.LIMIT.SUB% = CURR.PCR.COUNT% - 1 ! Minus one because table
                                          ! entry zero used by CSORT

!   Sort SORT.TABLE by (ascending) date-RPD
    CALL CSORT (VARPTR(SORT.TABLE$(0)), SORT.LIMIT.SUB%)

!   Re-populate CURR tables from SORT.TABLE$ retaining new sort order       ! 1.12 RC (177)

    SUB% = 1

    WHILE SUB% <= CURR.PCR.COUNT%
        CURR.DATE.DUE$(SUB%)     = MID$(SORT.TABLE$(SUB% - 1),  1, 3) ! 3 UPD   ! 1.12 RC (177)
        CURR.RPD.NO$(SUB%)       = MID$(SORT.TABLE$(SUB% - 1),  4, 3) ! 3 UPD   ! 1.12 RC (177)
        CURR.PRICE$(SUB%)        = MID$(SORT.TABLE$(SUB% - 1),  7, 4) ! 4 UPD   ! 1.12 RC (177)
        CURR.INC.DEC.FLAG$(SUB%) = MID$(SORT.TABLE$(SUB% - 1), 11, 1) ! 1 ASC   ! 1.12 RC (177)
        CURR.STATUS.FLAG$(SUB%)  = MID$(SORT.TABLE$(SUB% - 1), 12, 1) ! 1 ASC   ! 1.12 RC (177)
        CURR.MARKDOWN$(SUB%)     = MID$(SORT.TABLE$(SUB% - 1), 13, 1) ! 1 ASC   ! 1.12 RC (177)
        SUB% = SUB% + 1
    WEND

!   Reset flag ready for use with next IUF item
    CURR.SORT.NEEDED = FALSE

END SUB


\******************************************************************************
\***
\***    IUF.END.BATCH.PROCESSES                                    ! 1.11 RC (169)
\***
\******************************************************************************

SUB IUF.END.BATCH.PROCESSES PUBLIC                                 ! 1.11 RC (169)

    ! Builds a new PPFI file containing existing and new records   ! 1.11 RC (169)
    CALL CREATE.NEW.PPFI                                           ! 1.11 RC (169)

!   Update of BCF positioned after trailer count confirmed okay    ! 1.17 RC (230)

    ! Check Trailer count                                          ! 1.3CSk
    IF VAL(IUF.REC.COUNT$) <> COUNT.RECORDS.IUF% THEN BEGIN        ! 1.3CSk
        CURRENT.REPORT.NUM% = IUF.REPORT.NUM%                      ! 1.3CSk
        COUNT.EXPECTED% = VAL(IUF.REC.COUNT$)                      ! 1.3CSk
        COUNT.ACTUAL% = COUNT.RECORDS.IUF%                         ! 1.3CSk
        CALL LOG.EVENT(92) ! Logs message to DEC                   ! 1.3CSk
    ENDIF                                                          ! 1.17 RC (230)

!   Update BCF for batch processed                                 ! 1.17 RC (230)
    CALL UPDATE.BCF                                                ! 1.17 RC (230)
!   Send DEC message for batch processed.                          ! 1.17 RC (230)
    CALL SEND.DEC.MESSAGE                                          ! 1.17 RC (230)

END SUB                                                            ! 1.11 RC (169)


\******************************************************************************
\***
\***    PROCESS.IUF
\***
\******************************************************************************
SUB PROCESS.IUF

    INTEGER*1   END.OF.IUF.BATCH
    INTEGER*1   END.OF.IUF.ITEM
    INTEGER*1   WRITE.LOG.MESSAGE
    INTEGER*1   RC%
    INTEGER*4   BATCH.IUF.ITEM.SKIP%

    PHASE$ = "3.0"                                                              !1.6CSk
    CALL DO.MESSAGE("PSB21 PHASE 3", TRUE)


    IF SKIP.IUF.PROCESSING THEN BEGIN                                           !1.6CSk
        ! Once the system has switched over to NEW format IUF processing        !1.6CSk
        ! (signified by BCF Record 20 having a value), then OLD format          !1.6CSk
        ! IUF files will be skipped unless someone manually modifies the        !1.6CSk
        ! BCF Record 20 to spaces as part of the backout process.               !1.6CSk
                                                                                !1.6CSk
        CALL DO.MESSAGE("PSB21 *** ERROR: old format IUF will NOT be " + \      !1.6CSk
                        "processed", FALSE)                                     !1.6CSk
                                                                                !1.6CSk
        EXIT SUB                                                                !1.6CSk
                                                                                !1.6CSk
    ENDIF                                                                       !1.6CSk


    ! Display progress in 10% increments
    RECS.PER.5% = 0
    !RECS.PER.5% = SIZE(IUF.FILE.NAME$) / 5 ! Rough estimate for no. of records
    RECS.PER.5% = IUF.RECORD.COUNT% * 0.05
    PROGRESS.COUNT% = 0
    PERCENT.VAL%    = 0
    IUF.BATCH.ITEM.TOTAL% = 0
    IUF.ITEM.SKIP% = 0
    IUF.BATCH.COUNT% = 0

    ! Open the IUF
    RC% = OPEN.IUF

    ! IF we couldn't open the IUF then there's no point continuing with processing, but still process pending price file.
    IF RC% <> 0 THEN BEGIN
        ! Handle error - Unexpected open error
        IF NOT IUF.OPEN.ERROR.REPORTED THEN BEGIN                 ! 1.8 RC (8)
            CALL DO.MESSAGE("PSB21 *** ERROR4: opening IUF", FALSE)
            IUF.OPEN.ERROR.REPORTED = TRUE                        ! 1.8 RC (8)
        ENDIF                                                     ! 1.8 RC (8)
        CURRENT.REPORT.NUM% = IUF.REPORT.NUM%
        CALL LOG.EVENT(106)
        CALL CREATE.NEW.PPFI
       EXIT SUB
    ENDIF

    END.OF.IUF.FILE = FALSE

    ! Get the very first IUF record
    RC% = READ.IUF

    ! IF we couldn't read the first IUF record
    IF RC% <> 0 THEN BEGIN
        ! Handle error - Unexpected read error
        CALL DO.MESSAGE("PSB21 *** ERROR5: reading next IUF Record at: " + \
                 STR$(COUNT.RECORDS.IUF%), FALSE)
        CALL CLOSE.IUF                                           ! 1.6 RC (87)
        CALL LOG.EVENT(106)
        EXIT SUB
    ENDIF

    ! CALL DO.IUF.PROGRESS

    ! WHILE end of file has not been reached
    WHILE NOT END.OF.IUF.FILE

        CALL DO.MESSAGE("PSB21 3.0 - Start of IUF Batch found", TRUE)

        IUF.BATCH.COUNT% = IUF.BATCH.COUNT% + 1

        IF IUF.NEW.FORMAT THEN BEGIN
            CALL DO.MESSAGE("PSB21 BATCH = " + IUF.TIME.STAMP$, FALSE)
        ENDIF ELSE BEGIN
            CALL DO.MESSAGE("PSB21 BATCH = " + IUF.SERIAL.NO$, FALSE)
        ENDIF

        ! IF we have an IUF header record
        IF IUF.REC.TYPE$ = "H" THEN BEGIN

            !Validate fields on the item record.
            IF IUF.NEW.FORMAT THEN BEGIN
                CALL VALIDATE.IUF.FIELDS
            ENDIF

            IF IUF.STORE.NUM$ <> STORE.NUMBER$ THEN BEGIN
                CALL DO.MESSAGE("PSB21 *** ERROR: Wrong Store No. " + IUF.STORE.NUM$, FALSE)
                CALL LOG.EVENT(15)  ! abort whole batch of suite
            ENDIF

            COUNT.RECORDS.IUF% = 1  ! Reset Batch Count
            ! IF we've already processed this batch
            IF OLD.BATCH.FOUND THEN BEGIN

                ! WHILE end of batch has not been reached
                WHILE NOT (IUF.REC.TYPE$ = "T")

                    ! Read next IUF record
                    RC% = READ.IUF

                    ! IF we've successfully read the next IUF record
                    IF RC% = 0 THEN BEGIN
                        ! Skip this record
                        COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
                        CALL DO.IUF.PROGRESS
                    ENDIF ELSE BEGIN
                        ! Handle error - Unexpected read error
                        CALL DO.MESSAGE("PSB21 *** ERROR6: reading next IUF Record at: " + \
                                 STR$(COUNT.RECORDS.IUF%), FALSE)
                        CALL LOG.EVENT(106)
                    ENDIF

                WEND

                END.OF.IUF.BATCH = TRUE

            ENDIF ELSE BEGIN

                END.OF.IUF.BATCH = FALSE

                ! Read the next IUF record
                RC% = READ.IUF

                ! IF we've successfully read the next IUF record
                IF RC% = 0 THEN BEGIN

                    COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
                    CALL DO.IUF.PROGRESS
                    ! IF we have an IUF trailer record
                    IF IUF.REC.TYPE$ = "T" THEN BEGIN

                        ! We must have an empty batch
                        END.OF.IUF.BATCH = TRUE

                    ENDIF ELSE IF IUF.REC.TYPE$ = "H" THEN BEGIN
                        ! Handle error - Double header?????????
                        CALL DO.MESSAGE("PSB21 *** ERROR7: reading IUF Record at: " + \
                                 STR$(COUNT.RECORDS.IUF%), FALSE)
                        CALL LOG.EVENT(106)
                    ENDIF

                ENDIF ELSE BEGIN
                    ! Handle error - Unexpected read error
                    !CALL DO.MESSAGE("PSB21 *** ERROR8: reading IUF Record at: " + \
                    !         STR$(COUNT.RECORDS.IUF%), FALSE)
                    !CALL LOG.EVENT(106)
                    CALL DO.MESSAGE("PSB21 *** ERROR: Missing Trailer", FALSE)
                    CALL LOG.EVENT(92)
                ENDIF
            ENDIF
        ENDIF

        ! WHILE end of batch has not been reached
        WHILE NOT END.OF.IUF.BATCH

            END.OF.IUF.ITEM = FALSE

            ! Store the item code we're currently processing
            CURR.BOOTS.CODE$ = IUF.BOOTS.CODE$

            ! WHILE we're processing records for the same item
            WHILE NOT END.OF.IUF.ITEM


                ! IF we have an IUF item detail record
                IF IUF.REC.TYPE$ = "I" THEN BEGIN

                    PROCESS.ITEM = TRUE
                    !Validate fields on the item record.
                    IF IUF.NEW.FORMAT THEN BEGIN
                        CALL VALIDATE.IUF.FIELDS
                    ENDIF
                    !If pre-processing done, check if item in this batch should be processed
                    IF ITEM.PRE.PROCESSING.CHECK AND PROCESS.ITEM THEN BEGIN
                       CALL PROCESS.ITEM.CHECK
                    ENDIF

                    ! Store the item details for later use
                    IF PROCESS.ITEM THEN CALL STORE.ITEM.DETAIL

                        IUF.BATCH.ITEM.TOTAL% =  IUF.BATCH.ITEM.TOTAL% + 1

                        IF IUF.BATCH.ITEM.TOTAL% > IUF.ITEM.BATCH.LIMIT% THEN BEGIN

                            !Only write out message once
                            IF NOT WRITE.LOG.MESSAGE THEN BEGIN
                                CALL MAXIMUM.ITEM.LOG.MESSAGE
                                WRITE.LOG.MESSAGE = TRUE
                            ENDIF
                            PROCESS.ITEM = FALSE

                            IUF.ITEM.SKIP%       = IUF.ITEM.SKIP% + 1
                            BATCH.IUF.ITEM.SKIP% = BATCH.IUF.ITEM.SKIP% + 1

                        ENDIF

                    ! Read the next IUF record
                    RC% = READ.IUF

                    ! IF we've successfully read the next IUF record
                    IF RC% = 0 THEN BEGIN

                        COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
                        CALL DO.IUF.PROGRESS
                        CURR.PCR.COUNT% = 0

                        ! WHILE we still have price change records to process
                        WHILE IUF.REC.TYPE$ = "P"

                            !Validate fields on the item record.
                            IF IUF.NEW.FORMAT THEN BEGIN
                                CALL VALIDATE.IUF.FIELDS
                            ENDIF

!                           Put RPD price change into CURR data tables  ! 1.10 RC (113)
!                           Do not save an emergency 99999 RPD in this  ! 1.10 RC (113)
!                           way as these are generated by PSB21 itself  ! 1.10 RC (113)
                            IF IUF.RPD.NO$ <> "99999" THEN BEGIN  ! 1.9 RC (109)

                                CURR.PCR.COUNT% = CURR.PCR.COUNT% + 1

                                ! Store the price change for later use
                                CALL STORE.PRICE.CHANGE(CURR.PCR.COUNT%)

                            ENDIF                                 ! 1.9 RC (109)

                            ! Read the next IUF record
                            RC% = READ.IUF

                            ! IF we've successfully read the next IUF record
                            IF RC% = 0 THEN BEGIN

                                COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1
                                CALL DO.IUF.PROGRESS
                                ! IF next record is NOT price change, barcode, item detail or trailer
                                IF NOT (IUF.REC.TYPE$ = "P" OR  \
                                        IUF.REC.TYPE$ = "B" OR  \
                                        IUF.REC.TYPE$ = "I" OR  \
                                        IUF.REC.TYPE$ = "T") THEN BEGIN
                                    ! Handle error - Unexpected record type
                                    CALL UNEXPECTED.RECORD.TYPE
                                ENDIF

                            ENDIF ELSE BEGIN
                                ! Handle error - Unexpected read error
                                CALL DO.MESSAGE("PSB21 *** ERROR9: reading IUF Record at: " + \
                                         STR$(COUNT.RECORDS.IUF%), FALSE)
                                CALL LOG.EVENT(106)
                            ENDIF

                        WEND

                        ! Initialise barcode index
                        BARCODE.COUNT% = 0
                        DIM BARCODE.TABLE$(MAX.BARCODES%)

                        ! WHILE we still have barcode records to process
                        WHILE IUF.REC.TYPE$ = "B"

                            !Validate fields on the item record.
                            IF IUF.NEW.FORMAT THEN BEGIN
                                CALL VALIDATE.IUF.FIELDS
                            ENDIF

                            ! Store the barcode for later use
                            CALL STORE.BARCODE

                            ! Read the next IUF record
                            RC% = READ.IUF

                            ! IF we've successfully read the next IUF record
                            IF RC% = 0 THEN BEGIN

                                COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1

                                CALL DO.IUF.PROGRESS
                                ! IF next record is NOT barcode, item detail or trailer
                                IF NOT (IUF.REC.TYPE$ = "B" OR  \
                                        IUF.REC.TYPE$ = "I" OR  \
                                        IUF.REC.TYPE$ = "T") THEN BEGIN
                                    ! Handle error - Unexpected record type
                                    CALL UNEXPECTED.RECORD.TYPE
                                ENDIF

                            ENDIF ELSE BEGIN
                                CALL DO.MESSAGE("PSB21 *** ERROR: Missing Trailer", FALSE)
                                CALL LOG.EVENT(92)
                            ENDIF

                        WEND

                    ENDIF ELSE BEGIN
                        CALL DO.MESSAGE("PSB21 *** ERROR: Missing Trailer", FALSE)
                        CALL LOG.EVENT(92)
                    ENDIF

                ENDIF ELSE BEGIN
                    ! Handle error - Unexpected record type
                    CALL UNEXPECTED.RECORD.TYPE
                ENDIF

                ! IF IUF record is for a different item (or a header or trailer)
                IF CURR.BOOTS.CODE$ <> IUF.BOOTS.CODE$ OR   \
                   IUF.REC.TYPE$ = "H" OR                   \
                   IUF.REC.TYPE$ = "T" THEN BEGIN

                    ! At this point, we know we have every record for an item,
                    ! so we can now go and do some stuff with it but only if we haven't
                    ! detected a problem with any of the item's associated records.
                    IF PROCESS.ITEM THEN BEGIN
!                       At this point all the items IUF price  ! 1.10 RC (113)
!                       changes are loaded into CURR table     ! 1.10 RC (113)
                        CALL PROCESS.IUF.ITEM

                        !Item is configured as "Quantity Restricted"    !HRN
                        IF CURR.RESTRICT.SALES.FLAG$ = "Y" THEN BEGIN   !HRN

                            IRFITGRP.ITEM.CODE$ = \                     !HRN
                                        PACK$(LEFT$(CURR.BOOTS.CODE$,6))!HRN

                            IRFITGRP.GROUP.NO%  = VAL(CURR.GRP.NO$)     !HRN

                            !Write not successful                       !HRN
                            IF WRITE.IRFITGRP THEN BEGIN                !HRN
                                CALL LOG.EVENT(106)                     !HRN
                            ENDIF                                       !HRN

                        ENDIF ELSE BEGIN !Not a Quantity Restricted Item!HRN

                            IRFITGRP.ITEM.CODE$ = \                     !HRN
                                        PACK$(LEFT$(CURR.BOOTS.CODE$,6))!HRN

                            !Delete the Item record if found            !HRN
							
							print "value is " ;IRFITGRP.OPEN
                            IF NOT READ.IRFITGRP THEN BEGIN             !HRN
                                DELREC IRFITGRP.SESS.NUM%;             \!HRN
                                       IRFITGRP.ITEM.CODE$              !HRN
                            ENDIF ELSE BEGIN                            !HRN
                                !Nothing to do                          !HRN
                            ENDIF
                        ENDIF                                           !HRN

                    ENDIF

                    END.OF.IUF.ITEM = TRUE
                ENDIF

            WEND

            ! IF we have an IUF trailer record
            IF IUF.REC.TYPE$ = "T" THEN BEGIN
                ! No more items left to process in this batch
                END.OF.IUF.BATCH = TRUE
                IUF.BATCH.ITEM.TOTAL% = 0
                WRITE.LOG.MESSAGE = FALSE
                PHASE$ = "3.3"                                                 !1.6CSk
                CALL DO.MESSAGE("PSB21 3.3 - End of IUF batch found", TRUE)
                IF BATCH.IUF.ITEM.SKIP% > 0 THEN BEGIN
                    CALL DO.MESSAGE("PSB21 " + STR$(BATCH.IUF.ITEM.SKIP%) +    \
                                    " items skipped from batch", FALSE)
                    BATCH.IUF.ITEM.SKIP% = 0
                ENDIF

                CALL IUF.END.BATCH.PROCESSES                            ! 1.11 RC (169)
!                    Calls CREATE.NEW.PPFI to build new PPFI from PPFK  ! 1.11 RC (169)
!                    Calls UPDATE.BCF for batch processed               ! 1.11 RC (169)
!                    Checks IUF trailer count                           ! 1.11 RC (169)
!                    Send DEC message for batch processed.              ! 1.11 RC (169)

            ENDIF

        WEND

        ! IF we have an IUF trailer record
        IF IUF.REC.TYPE$ = "T" THEN BEGIN

            IF IUF.NEW.FORMAT THEN BEGIN                                ! 1.11 RC (169)
                IF BCF.IUF.DATETIMESTAMP$ <> IUF.TIME.STAMP$ THEN BEGIN ! 1.11 RC (169)
                    CALL IUF.END.BATCH.PROCESSES                        ! 1.11 RC (169)
                ENDIF                                                   ! 1.11 RC (169)
            ENDIF ELSE BEGIN                                            ! 1.11 RC (169)
                IF BCF.IUF.SERIAL.NO$ <> IUF.SERIAL.NO$ THEN BEGIN      ! 1.11 RC (169)
                    CALL IUF.END.BATCH.PROCESSES                        ! 1.11 RC (169)
                ENDIF                                                   ! 1.11 RC (169)
            ENDIF                                                       ! 1.11 RC (169)

            IF IUF.INITIAL.LOAD$ = "Y" THEN BEGIN
                ! Label Suppression
                CALL UPDATE.SUPPARAM
            ENDIF

            ! Read next IUF record
            RC% = READ.IUF

            IF RC% = 0 THEN BEGIN

                CALL DO.IUF.PROGRESS
                ! Should be a header for the next batch - we'll check later
            ENDIF ELSE BEGIN
                END.OF.IUF.FILE = TRUE
                PHASE$ = "3.4"                                                 !1.6CSk
                CALL DO.MESSAGE("PSB21 3.4 - End of IUF file found", TRUE)
            ENDIF

        ENDIF ELSE BEGIN
            ! Handle error - Unexpected record type
            CALL UNEXPECTED.RECORD.TYPE
        ENDIF

    WEND

    ! Close IUF
    CALL CLOSE.IUF                                               ! 1.6 RC (87)

    !Close IRFITGRP File                                                !HRN
    IF IRFITGRP.OPEN THEN BEGIN                                         !HRN
	
	
        CLOSE IRFITGRP.SESS.NUM%                                        !HRN
        IRFITGRP.OPEN = FALSE                                           !HRN
    ENDIF                                                               !HRN

END SUB


\*******************************************************************************
\***
\*** DELETE.PPFK.KEYED.FILE
\***
\***
\*******************************************************************************

SUB DELETE.PPFK.KEYED.FILE PUBLIC                              ! 1.13 RC (190)

    PHASE$ = "005"                                                              !1.6CSk
    CALL DO.MESSAGE("PSB21 PHASE 5", TRUE)
    CALL DO.MESSAGE("PSB21 5.0 - Deleting PPFK File", TRUE)

    RC% = DESTROY.PPFK

    IF RC% <> 0 THEN BEGIN
        ! Handle error - Could not delete PPFK file
        CALL DO.MESSAGE("PSB21 *** ERROR cannot delete PPFK", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

END SUB

\*******************************************************************************
\***
\*** IUF.PRE.PROCESSING
\***
\***
\*******************************************************************************

SUB IUF.PRE.PROCESSING

    INTEGER*1 END.PRE.PROCESSING
    INTEGER*2 NUMBER.OF.BATCHES%
    INTEGER*4 UNIQUE.BATCH.NUMBER%
    INTEGER*4 PREVIOUS.BATCH.NUMBER%
    INTEGER*4 IUF.BOOTS.CODE%

    PHASE$ = "0.2"                                                              !1.6CSk
    CALL DO.MESSAGE("PSB21 0.2 - Check if Pre-Processing of the IUF required ", FALSE)

    IF IUF.NEW.FORMAT THEN BEGIN

        CALL DO.MESSAGE("PSB21 - New IUF format. Start IUF Pre-Processing ", FALSE)

        !Read IUF records until end. If only one batch found end pre-processing

        ! Open the IUF
        RC% = OPEN.IUF

        ! IF we couldn't open the IUF then there's no point continuing
        IF RC% <> 0 THEN BEGIN
            ! Handle error - Unexpected open error
            IF NOT IUF.OPEN.ERROR.REPORTED THEN BEGIN             ! 1.8 RC (8)
                CALL DO.MESSAGE("PSB21 *** ERROR10: IUF PRE-PROCESSING. Unable to open IUF", FALSE) !1.3CSk
                IUF.OPEN.ERROR.REPORTED = TRUE                    ! 1.8 RC (8)
            ENDIF                                                 ! 1.8 RC (8)
            EXIT SUB
        ENDIF

        ! Get the very first IUF record
        RC% = READ.IUF

        ! IF we couldn't read the first IUF record
        IF RC% <> 0 THEN BEGIN
            ! Handle error - Unexpected read error
            CALL DO.MESSAGE("PSB21 *** ERROR11: IUF PRE-PROCESSING. Reading next IUF Record at: " + \     !1.3CSk
                     STR$(COUNT.RECORDS.IUF%), FALSE)
            CALL CLOSE.IUF                                       ! 1.6 RC (87)
            CALL LOG.EVENT(106)
            EXIT SUB
        ENDIF

        !Create a million element Integer*4 array for IUF pre-processing                                  !1.6CSk
        CALL I4.LIST.DIM(1000000)
        END.PRE.PROCESSING        = FALSE
        ITEM.PRE.PROCESSING.CHECK = FALSE
        UNIQUE.BATCH.NUMBER%      = 0
        PREVIOUS.BATCH.NUMBER%    = 0
        NUMBER.OF.BATCHES%        = 0
        IUF.RECORD.COUNT%         = 0

        WHILE NOT END.PRE.PROCESSING

             IF IUF.REC.TYPE$ = "H" THEN BEGIN

                  NUMBER.OF.BATCHES%   = NUMBER.OF.BATCHES% + 1
                  UNIQUE.BATCH.NUMBER% = NUMBER.OF.BATCHES%
                  IUF.RECORD.COUNT%    = IUF.RECORD.COUNT% + 1

             ENDIF ELSE IF IUF.REC.TYPE$ = "I" THEN BEGIN

               !Read and store items for each IUF batch in the pre-processing array.
               WHILE NOT (IUF.REC.TYPE$ = "T") AND NOT END.PRE.PROCESSING

                   IF RC% <> 0 THEN BEGIN
                      ! end of file found
                      END.PRE.PROCESSING = TRUE
                   ENDIF ELSE BEGIN
                       IF IUF.REC.TYPE$ = "I" THEN BEGIN
                           !Validate field does not contain non numeric values and length 7
                           IF NOT MATCH("!",IUF.BOOTS.CODE$,1) THEN BEGIN
                               IF LEN(IUF.BOOTS.CODE$) = 7 THEN BEGIN
                                   IUF.BOOTS.CODE% = VAL(LEFT$(IUF.BOOTS.CODE$,6))
                                   CALL I4.LIST.SET(IUF.BOOTS.CODE%, UNIQUE.BATCH.NUMBER%)
                               ENDIF
                           ENDIF
                       ENDIF

                       RC% = READ.IUF

                       IUF.RECORD.COUNT%    = IUF.RECORD.COUNT% + 1

                   ENDIF

               WEND

               IUF.RECORD.COUNT% = IUF.RECORD.COUNT% + 1

             ENDIF

             RC% = READ.IUF

             IF RC% <> 0 THEN BEGIN
                ! end of file found
                END.PRE.PROCESSING = TRUE
             ENDIF

        WEND

        CALL CLOSE.IUF                                           ! 1.6 RC (87)

        IF NUMBER.OF.BATCHES% > 1 THEN BEGIN

           ITEM.PRE.PROCESSING.CHECK = TRUE

        ENDIF

        CALL DO.MESSAGE("PSB21 - New IUF format. Completed IUF Pre-Processing ", FALSE)

    ! Old format no pre-processing required
    ENDIF ELSE BEGIN

        CALL DO.MESSAGE("PSB21 Old IUF format. No Pre-Processing required ", FALSE)

        IF LEFT$(BCF.IUF.DATETIMESTAMP$,1) <> " " THEN BEGIN                                          !1.6CSk
            CALL DO.MESSAGE("PSB21 *** ERROR: OLD format IUF detected. New format expected", FALSE)   !1.6CSk
            CALL DO.MESSAGE("PSB21            since BCF Rec 20 Batch Timestamp is set. To" , FALSE)   !1.6CSk
            CALL DO.MESSAGE("PSB21            override, manually Set BCF Rec 20 to SPACES.", FALSE)   !1.6CSk
            SKIP.IUF.PROCESSING = TRUE                                                                !1.6CSk
            JOBSOK.FLAG$ = "X"                                                                        ! 1.7 RC
        ENDIF                                                                                         !1.6CSk

    ENDIF

END SUB


\*******************************************************************************
\*******************************************************************************
\*******************************************************************************
\*******************************************************************************
\***                                                                         ***
\***              S T A R T   O F   M A I N L I N E   C O D E                ***
\***                                                                         ***
\*******************************************************************************
\*******************************************************************************
\*******************************************************************************
\*******************************************************************************

    ON ERROR GOTO ERROR.DETECTED

    ! Initialise constants and static tables
    CALL INITIALISATION

    ! Pre-processing of the IUF file
    CALL IUF.PRE.PROCESSING

    !Add all existing WEEEUF records into a table
    CALL LOAD.WEEEUF.FILE.INTO.TABLE

    ! Creates a new PGF using the file PGFDIR
    CALL PROCESS.PGF
    CALL CHECK.PROCESS.PGF ! PSB2102                           ! 1.20 RC (240)

    ! Add all existing PPFI records to a temporary keyed file
    CALL PROCESS.OLD.PPFI

    ! Update item reference files for all items on the IUF
    CALL PROCESS.IUF

!   Delete the PPFK file                                                    !GMW
    CALL DELETE.PPFK.KEYED.FILE ! Writes "PHASE 5"              ! 1.4 RC (190)

    ! Update JOBSOK, send message to DEC API and chain to PSB20
    CALL PROGRAM.EXIT

    STOP

\******************************************************************************
\***
\***    Main error handling routine
\***
\******************************************************************************
ERROR.DETECTED:

    IF ERR = "CU" THEN RESUME

    IF ERR = "OE" AND ERRF% = 0 THEN RESUME

    PRINT ERR;ERRNH;" at line ";ERRL
    PRINT "Got to IUF Record No: " + STR$(COUNT.RECORDS.IUF%)

    ERROR.COUNT% = ERROR.COUNT% + 1

    IF ERROR.COUNT% > 1 THEN BEGIN
        PRINT "ERROR.DETECTED hit for the 2nd time - STOPPING PROGRAM!"
        STOP
    ENDIF

    IF ERR = "CM" OR ERR = "CT" THEN BEGIN ! Chaining Error
        CALL LOG.EVENT(18)
        STOP
    ENDIF

    IF ERR = "SU" THEN BEGIN ! Array Subscript Error                                  ! 1.15 CSk
        PRINT "*** ERROR Subscript out of bounds error" + \                           ! 1.15 CSk
              " - STOPPING PROGRAM!"                                                  ! 1.15 CSk
        PRINT "*** ERROR Exceeded max no. of " + STR$(PPFK.PCR.MAX%) +  \             ! 1.15 CSk
              " price changes per item."                                              ! 1.15 CSk
        CALL DO.MESSAGE("*** ERROR Subscript out of bounds error" + \                 ! 1.15 CSk
                        " - STOPPING PROGRAM!", TRUE)                                 ! 1.15 CSk
        CALL DO.MESSAGE("*** ERROR Exceeded max no. of " + STR$(PPFK.PCR.MAX%) + \    ! 1.15 CSk
                        " price changes per item.", TRUE)                             ! 1.15 CSk
    ENDIF                                                                             ! 1.15 CSk

    RC% = STANDARD.ERROR.DETECTED (ERRN,  \
                                   ERRF%, \
                                   ERRL,  \
                                   ERR)

    JOBSOK.FLAG$ = "X"
    CALL PROGRAM.EXIT    ! If called by PSB20 then chain back to PSB20

    END
