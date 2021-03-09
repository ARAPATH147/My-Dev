\*******************************************************************************
\*******************************************************************************
\***
\***
\***            PROGRAM         :       PSB21
\***            MODULE          :       PSB2101
\***            AUTHOR          :       Charles Skadorwa / Mark Walker / Mark Goode
\***            DATE WRITTEN    :       Sept 2011
\***
\*******************************************************************************
\***
\***    VERSION 1.0       Charles Skadorwa/Mark Walker/Mark Goode 30 SEP 2011
\***    Initial version.
\***
\***    VERSION 1.1           Mark Goode
\***    IEX update function is incorrect, never updates the last actual supplier
\***    for current items.
\***
\***    VERSION 1.2           Tittoo Thomas
\***    Defect 3361 - Fixed the IEX primary supplier number to be 4 UPD.
\***
\***    VERSION 1.3           Charles Skadorwa                  16 January 2012
\***    Defect 3491: Manual processing of Legacy IUF is failed.
\***                 BARCODE.PROCESSING modified to ensure that it continues if
\***                 any problems are found with deleting the IEF chain.
\***
\***    CR EPOS5   : Change to IRF.INDICAT10% Return Route and Returnable bit
\***                 flag settings.
\***
\***    VERSION 1.4.          Charles Skadorwa                  25 January 2012  ! 1.4 RC
\***    Changes creating PSB21.286 Core Release 2 version 1.4.                   ! 1.4 RC
\***    Changed module version comments from 1.3 to 1.4 to distinguish           ! 1.4 RC
\***    Charlies latest fixes (below) from previous version of this module       ! 1.4 RC
\***
\***    Defect   19: IUF Earn Points flag has incorrect data transformation
\***                 listed in DD.
\***
\***    Defect   21: IUF Discountable flag has incorrect data transformation
\***                 listed in DD.
\***
\***    VERSION 1.5.                ROBERT COWEY.                07 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.5.
\***
\***    Defect 21 - Commented 1.5 RC (21).                       07 FEB 2012.
\***    Original fix (and associated comments) were incorrect as they were    ! 1.6 RC (21)
\***    based on a mis-understanding of variable IUF.DISCOUNTABLE$.           ! 1.6 RC (21)
\***    The fix has now been re-coded (commented 1.6 RC (21) ) and any        ! 1.6 RC (21)
\***    potentially confusing comments also corrected or removed.             ! 1.6 RC (21)
\***
\***    Defect 51 - Commented 1.5 RC (51).
\***    Retained setting of ISF.SEL.PRINTED.FLAG$ to "N" for new items but
\***    now leave it unchanged for existing items (instead of set to "Y").
\***
\***    Various Defects/changes resulting from Code WPI's - Commented !1.5CSk(a)
\***
\***    VERSION 1.6.                ROBERT COWEY.                21 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.6.
\***
\***    Defect 69 - Commented 1.6 RC (69).
\***    Expanded the IF test that sets Directs A IDF.BIT.FLAGS.1% X'04' to
\***    uss an ELSE statement to switch the bit OFF (thereby preventing it
\***    becoming a once-ON-always-ON flag).
\***
\***    Defect 21 - Commented 1.6 RC (21).
\***    Corrected setting of IRF Discount Exempt flag within UPDATE.IRF.
\***
\***    Defect 71 - Commented 1.6 RC (71).
\***    Corrected setting of IRF Unrestricted Group Code flag for legacy IUF
\***    by retaining any current IRF value for an existing item.
\***
\***    VERSION 1.7.                ROBERT COWEY.                28 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.7.
\***
\***    Defect 53 - Commented 1.7 RC (53).
\***    Set IRF.PRICE.MISMATCH$ flag to indicate increase or decrease for
\***    use when generating emergency RPD's (within PSB2103).
\***
\***    VERSION 1.8.                ROBERT COWEY.                08 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.8.
\***    Removed IRF.PRICE.MISMATCH$ indicating price increase or decrease as
\***    setting of PPFK.INC.DEC.FLAG$ is done by PSB23.
\***
\***    Defect 95 - Commented 1.8 RC (95)                        12 MAR 2012.
\***    Re-defined NEW.ITEM as being the absence of IDF record (and removed
\***    NEW.ITEM.OVERRIDE.FLAG made redundant).
\***    Called UPDATE.IDF prior to UPDATE.IRF (instead of after it) but
\***    retained BARCODE.PROCESSING at its original position (after both
\***    IDF and IRF records populated from IUF).
\***
\***    VERSION 1.9.                ROBERT COWEY.                13 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.9.
\***    Commented setting of IRF.SALEPRIC$ to zeros when CURR.CURRENT.PRICE$
\***    (taken from IUF) is missing as being to cause price entry.
\***
\***    Defect 106 - Commented 1.9 RC (106)                      13 MAR 2012.
\***    Set IDF.FILLER$ to null for new item.
\***
\***    Defect 112 - Commented 1.9 RC (112)                      15 MAR 2012.
\***    Set DRUG.FILLER$ to 8 bytes null.
\***
\***    Defect 102 - Commented 1.9 RC (95)+(102)                 15 MAR 2012.
\***    Correction to previous fix 1.8 RC (95) to set NEW.ITEM = FALSE when
\***    IDF record is present.
\***
\***    Defect 109 - Commented 1.9 RC (109)                      19 MAR 2012.
\***    Made checking of IUF item price against head office price on LOCAL
\***    and CIPPMR files dependant on appropriate IRF and IDF flags.
\***    Corrected these flags if incorrect.
\***
\***    VERSION 1.10.               ROBERT COWEY.                27 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.10.
\***
\***    Defect 113 - Commented 1.10 RC (113)
\***    Modified UPDATE.CIPPM to update Reversals Price rather than Original
\***    IRF price and also to call WRITE.IDF to correct IDF Markdown flag.
\***
\***    UPDATE.LOCAL procedure deleted.
\***    Price changes for local priced items are treated as for other items.
\***    PSB21 does not update LOCAL (which is done instead by PSB23).
\***
\***    Defect 142 - Commented 1.10 RC (142)
\***    Modified procedure UPDATE.IEX to READ.IEX as part of the update
\***    (defaulting IEX variables to null when the item is not found)
\***    Also set IEX.FILLER$ is null instead of spaces to address the
\***    specific defect raised.
\***
\***    VERSION 1.11.               ROBERT COWEY.                05 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.11.
\***
\***    Defect 172 - Commented 1.11 RC (172)
\***    Corrected UPDATE.CIPPM to use passed item code as well as price.
\***
\***    VERSION 1.12.               ROBERT COWEY.                18 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.12.
\***
\***    Renamed IRF.PRICE.MISMATCH to PRICE.MISMATCH to more properly
\***    reflect its wider usage.
\***    Modified code to use this flag for all discrepancies between
\***    definitive price on IUF item record and Head Office price
\***    whether present on IRF, LOCAL, or on CIPPMR (as Reversals Price).
\***
\***    VERSION 1.13.               ROBERT COWEY.                23 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.13.
\***
\***    Defect 179 - Commented 1.13 RC (179)
\***    Modified UPDATE.IRF and UPDATE.ISF to set SEL.NON.PRICE.CHANGE flag
\***    when non-price-change SEL information changes requiring SEL printing.
\***    Modified UPDATE.RICF to use this flag (and also PRICE.CHANGE.TODAY
\***    flag to save reprocessing PPFK data) to control RICF updates.
\***
\***    VERSION 1.14.               ROBERT COWEY.                01 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.15.
\***    Comment corrected.
\***
\***    Defect 200 - Commented 1.14 RC (200)
\***    Checked return code from PSBF19 function UPDATE.ALL.IRF.BAR.CODES
\***    reporting any items for which barcodes not successfully updated
\***    and setting JOBOK.FLAG$ to "Y"
\***    Modified UPDATE.IRF to create the initial IRF record (for use by
\***    PSBF41 during UPDATE.ALL.IRF.BAR.CODES) using the item code and
\***    non-group-code prefix (as required for the initial PSBF41 IRF read).
\***    Modified UPDATE.IRF to ensure IRF variables properly initialised
\***    when READ.IRF fails because main item-barcode IRF record not found
\***    (thereby preventing previous IUF items price being passed to currrent
\***    IUF items when main item-barcode IRF record is missing).
\***
\***    VERSION 1.15.               ROBERT COWEY.                09 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.16.
\***
\***    Defect 226 - Commented 1.15 RC (226)
\***    Modified CHECK.FOR.PRICE.MISMATCH and UPDATE.CIPPM routines to
\***    cater for stores not having a CIPPMR (eg, not on CIP markdown).
\***
\***    VERSION A                   CHARLES SKADORWA.             5 SEP 2013.
\***    F261 Gift Card Mall IIN Range Extension Project - Commented ! ACSk
\***    In SUB UPDATE.IRF, set BIT3 of IRF.INDICAT0% if the
\***    IUF.RETURNABLE$ flag is set to "N".
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
\***    Update the STOCK file when the item status of an existing item
\***    is updated. This is required to allow an item to be re-considered
\***    for initial stock snapshot processing.
\***
\***    VERSION E                   Mark Walker                 6th May 2014
\***    F337 Centralised View of Stock
\***    QC607: Fix to item status update processing.
\***
\***    VERSION F                   Mark Walker                 8th Jul 2014
\***    F353 Deal Limits Increase
\***    Removed DINF and NIADF file processing.
\***
\***    VERSION G                   Rejiya Nair            13th May 2016
\***    PRJ1452 Restricting Item Sales
\***    - Set the "enforced quantity" bit (bit 7) in the IRF file;
\***      if the item is a group restricted item (The variable;
\***      CURR.RESTRICT.SALES.FLAG$ will be set to "Y").
\***    - Reset bit 7 (if it is already set) if the item is not a group
\***      restricted item (CURR.RESTRICT.SALES.FLAG$ will be set to "N")
\***    - Removed the previously tagged commented out code.
\***
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
\***    Module Overview
\***    ----------------
\***
\***    This module updates all the associated data files for an item.
\***
\***
\*******************************************************************************
\*******************************************************************************

%INCLUDE BCFDEC.J86     ! BRCF File
%INCLUDE CIPPMDEC.J86   ! Markdown File
%INCLUDE DRUGDEC.J86    ! Drug File
%INCLUDE IDFDEC.J86     ! Item Data File
%INCLUDE IEFDEC.J86     ! Item EAN File
%INCLUDE IEXDEC.J86     ! Item Extension File
%INCLUDE IRFDEC.J86     ! Item Reference File
%INCLUDE ISFDEC.J86     ! Item Shelf Edge Label Description File
%INCLUDE IUFDEC.J86     ! Item Update File
%INCLUDE LOCALDEC.J86   ! Local File
%INCLUDE NLINEDEC.J86   ! New Lines File
%INCLUDE PPFKDEC.J86    ! PPFK File
%INCLUDE RICFDEC.J86    ! Redeem Items Change File
%INCLUDE SOFTSDEC.J86   ! Software Status File
%INCLUDE STKMQDEC.J86   ! Stock Movement Queue File
%INCLUDE STOCKDEC.J86   ! Stock File

%INCLUDE PSBF19G.J86    ! UPDATE IRF GLOBALS
%INCLUDE PSBF41G.J86    ! IRF UPDATE FUNCTION


    INTEGER*1 GLOBAL                        \
        CIPPM.PRESENT,                      \                        ! 1.16 RC (226)
        FALSE,                              \
        PRICE.MISMATCH,                     \                        ! 1.12 RC
        ITEM.ON.WEEE.TABLE,                 \
        ITEM.STATUS.CHANGED,                \                               !DMW
        NEW.ITEM,                           \
        PRICE.CHANGE.TODAY,                 \                        ! 1.13 RC (179)
        SAV.IRF.INDICAT3%,                  \                        ! 1.13 RC (179)
        SEL.NON.PRICE.CHANGE,               \                        ! 1.13 RC (179)
        TRUE                                !


    INTEGER*2 GLOBAL                        \
        BARCODE.COUNT%,                     \
        CURR.PCR.COUNT%,                    \
        CURR.SESS.NUM%,                     \
        GOOD.CHAIN.COUNT%,                  \
        I%,                                 \
        K%,                                 \
        MAX.BARCODES%,                      \
        MAX.PRICE.CHANGES%,                 \
        NO.OF.BC%,                          \
        NO.OF.STATINS%,                     \                                   !1.5CSk(a)
        WEEE.ITEM.COUNT%,                   \
        WIX%                                !

    INTEGER*4 GLOBAL                        \
        COUNT.RECORDS.IUF%,                 \
        RC%                                 !

    STRING GLOBAL                           \
        BARCODE.TABLE$(1),                  \
        CHLAMYDIA.ID$(1),                   \                                   !1.5CSk(a)
        DELETION.TABLE(1),                  \
        GCM.PG.LIST$,                       \  List of Gift Card mall     ! BCSk
                                            \  Product Groups read in     ! BCSk
                                            \  from BCF Rec: 22           ! BCSk
        JOBSOK.FLAG$,                       \                                   ! 1.14 RC (200)
        STATINS$(1),                        \                                   !1.5CSk(a)
        WEEEUF.TABLE$(1)                    !

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
        CURR.UNRESTRICTED.GROUP.CODE$,      \
        MODULE.NUMBER$,                     \                                   !1.5CSk(a)
        PACKED.BC$,                         \
\       PRICE.CHANGE.DUE$,                  \                                   ! 1.13 RC (179)
        PROCESSING.DATE$,                   \
        READ.ISF.ITEM.QTY$,                 \                                   ! 1.13 RC (179)
        READ.ISF.S.E.DESC$,                 \                                   ! 1.13 RC (179)
        READ.ISF.UNIT.MEASUREMENT$,         \                                   ! 1.13 RC (179)
        READ.ISF.UNIT.NAME$,                \                                   ! 1.13 RC (179)
\       REDEEM.STATUS.CHANGED$,             \                                   ! 1.13 RC (179)
        SOFTS.REC.62$                       !


    STRING GLOBAL                           \
        CURR.DATE.DUE$(1),                  \
        CURR.INC.DEC.FLAG$(1),              \
        CURR.MARKDOWN$(1),                  \
        CURR.PRICE$(1),                     \
        CURR.RPD.NO$(1),                    \
        CURR.STATUS.FLAG$(1)                !

    STRING GLOBAL                           \
        ACD.FLAG$,                          \
        IRF.LOCKED.FLAG$                    !

    STRING GLOBAL                           \                           !GRN
        CURR.GRP.NO$,                       \                           !GRN
        CURR.RESTRICT.SALES.FLAG$           !                           !GRN

%INCLUDE BCFEXT.J86     ! BRCF File
%INCLUDE CIPPMEXT.J86   ! Markdown File
%INCLUDE DRUGEXT.J86
%INCLUDE IDFEXT.J86
%INCLUDE IEFEXT.J86
%INCLUDE IEXEXT.J86
%INCLUDE IRFEXT.J86
%INCLUDE ISFEXT.J86
%INCLUDE LOCALEXT.J86
%INCLUDE NLINEEXT.J86
%INCLUDE RICFEXT.J86
%INCLUDE SOFTSEXT.J86
%INCLUDE STKMQEXT.J86
%INCLUDE STOCKEXT.J86

%INCLUDE PSBF19E.J86    ! UPDATE IRF EXTERNALS
%INCLUDE PSBF41E.J86    ! IRF UPDATE FUNCTION

%INCLUDE CMPDATE.J86


SUB DO.MESSAGE(MSG$, LOG%) EXTERNAL
END SUB

SUB LOG.EVENT(EVENT.NO%) EXTERNAL
END SUB

SUB PROCESS.PPFK.ITEM EXTERNAL
END SUB

\******************************************************************************
\***
\*** WRITE.STKMQ.6.RECORD
\***
\***
\******************************************************************************

SUB WRITE.STKMQ.6.RECORD

    CURR.SESS.NUM% = STKMQ.SESS.NUM%
    IF END# STKMQ.SESS.NUM% THEN STKMQ.FILE.ERROR
    OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 155 READONLY NODEL APPEND

    STKMQ.RECORD.DELIMITER$ = CHR$(34)
    STKMQ.FIELD.DELIMITER$  = CHR$(59)
    STKMQ.ENDREC.MARKER$    = CHR$(0Dh) + CHR$(0Ah)

    STKMQ.DATE$ = PACK$(DATE$)
    STKMQ.TIME$ = PACK$(TIME$)

    STKMQ.TRANS.TYPE$ = PACK$("06")
    STKMQ.BOOTS.CODE$ = STOCK.BOOTS.CODE$
    STKMQ.REASON$ = "I"

    STKMQ.RECORD$ = STKMQ.RECORD.DELIMITER$  \
                  + STKMQ.TRANS.TYPE$        \
                  + STKMQ.FIELD.DELIMITER$   \
                  + STKMQ.DATE$              \
                  + STKMQ.TIME$              \
                  + STKMQ.BOOTS.CODE$        \
                  + STKMQ.REASON$            \
                  + STKMQ.RECORD.DELIMITER$  \
                  + STKMQ.ENDREC.MARKER$

    RC% = WRITE.STKMQ
    CLOSE STKMQ.SESS.NUM%

    IF RC% = 0 THEN BEGIN
        EXIT SUB
    ENDIF

 STKMQ.FILE.ERROR:
    CALL DO.MESSAGE("PSB21 *** ERROR WRITE.STKMQ.6.RECORD: [" + STKMQ.RECORD$ +"]", FALSE)
    CALL LOG.EVENT(106)


END SUB


\******************************************************************************
\***
\***  BARCODE.PROCESSING
\***
\******************************************************************************
\***  Setup IDF/IRF barcodes and add associated barcodes to IEF.
\***
\***  Firstly, we need to delete all chains from IEF for existing items. The
\***  chain is read into an array and the deletion is performed in reverse so
\***  that if a delete fails then the first part of the chain remains intact.
\***  This routine also caters for situations where the barcode count is
\***  incorrect due to broken chains.
\***
\***  In all the scenarios below, the 1st IDF Barcode ALWAYS defaults to the
\***  Boots Item Code (less the check digit).
\***
\***  Scenario 1: No IUF barcode records are present for an item.
\***              Ans: 2nd barcode defaults to 0's. Set No. of barcodes to 1.
\***
\***  Scenario 2: one IUF barcode record present for an item.
\***              Ans: If the IUF barcode matches the Boots Item Code then set up
\***                   the barcodes as for Scenario 1 else set the 2nd IDF barcode
\***                   to the IUF barcode. Set No. of barcodes to 2.
\***
\***  Scenario 3: two or more IUF barcode records present for an item.
\***              Ans: scan through barcodes and ignore the one that matches the 1st
\***                   barcode. Set the 2nd barcode to one of the IUF barcodes.
\***                   Set No. of barcodes to correct value.
\***
\***  If there are more than 2 barcodes (exluding Boots Item Code one), then write
\***  these out to the IEF.
\***
\******************************************************************************
SUB BARCODE.PROCESSING

    INTEGER*1 DELETING.IEF.CHAIN     ! Used to determine where to continue when error

    STRING BCREF$

    ON ERROR GOTO BARCODE.ERROR

    DELETING.IEF.CHAIN = FALSE

    GOOD.CHAIN.COUNT% = 0

    IF NOT NEW.ITEM THEN BEGIN ! Only delete existing item barcode chains

        DELETING.IEF.CHAIN = TRUE

        NO.OF.BC% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))

        IF NO.OF.BC% > 2 AND \
            IDF.SECOND.BAR.CODE$ <> PACK$("000000000000") THEN BEGIN

            DIM DELETION.TABLE(1000)

            PACKED.BC$ = PACK$(LEFT$(CURR.BOOTS.CODE$,6))

            ! Build current barcode chain
            FOR I% = 1 TO (NO.OF.BC% - 1)
                IF I% = 1 THEN BEGIN
                    IEF.BOOTS.CODE.BAR.CODE$ = PACKED.BC$ + IDF.SECOND.BAR.CODE$
                    RC% = READ.IEF
                    IF RC% = 0 THEN BEGIN
                        DELETION.TABLE(I%) = IEF.BOOTS.CODE.BAR.CODE$
                        GOOD.CHAIN.COUNT% = GOOD.CHAIN.COUNT% + 1
                    ENDIF ELSE BEGIN
                        CALL DO.MESSAGE("PSB21 *** WARNING missing barcode chain on IEF", FALSE)
                        CALL DO.MESSAGE("PSB21 *** WARNING BARCODE.PROCESSING at IUF Record: " + \
                            STR$(COUNT.RECORDS.IUF%), FALSE)
                        CALL DO.MESSAGE("PSB21 *** IEF.BOOTS.CODE.BAR.CODE$ [" + \
                            UNPACK$(IEF.BOOTS.CODE.BAR.CODE$) + "]", FALSE)
                    ENDIF
                ENDIF ELSE BEGIN
                    IEF.BOOTS.CODE.BAR.CODE$ = PACKED.BC$ + IEF.NEXT.BAR.CODE$
                    RC% = READ.IEF
                    IF RC% = 0 THEN BEGIN
                        DELETION.TABLE(I%) = IEF.BOOTS.CODE.BAR.CODE$
                        GOOD.CHAIN.COUNT% = GOOD.CHAIN.COUNT% + 1
                    ENDIF ELSE BEGIN
                        CALL DO.MESSAGE("PSB21 *** WARNING missing barcode chain on IEF", FALSE)
                        CALL DO.MESSAGE("PSB21 *** WARNING BARCODE.PROCESSING at IUF Record: " + \
                            STR$(COUNT.RECORDS.IUF%), FALSE)
                        CALL DO.MESSAGE("PSB21 *** IEF.BOOTS.CODE.BAR.CODE$ [" + \
                            UNPACK$(IEF.BOOTS.CODE.BAR.CODE$) + "]", FALSE)
                    ENDIF
                ENDIF
            NEXT I%

            FOR I% = GOOD.CHAIN.COUNT% TO 1 STEP -1

                IEF.BOOTS.CODE.BAR.CODE$ = DELETION.TABLE(I%)

                DELREC IEF.SESS.NUM%; IEF.BOOTS.CODE.BAR.CODE$
            NEXT I%
        ENDIF
    ENDIF

 ADD.BARCODES:

    ! 1st barcode always defaults to Boots Code
    IDF.FIRST.BAR.CODE$  = PACK$("000000" + LEFT$(CURR.BOOTS.CODE$,6))    ! 6 UPD
    IDF.SECOND.BAR.CODE$ = PACK$("000000000000")                          ! 6 UPD

    IF BARCODE.COUNT% > 0 THEN BEGIN
        IDF.SECOND.BAR.CODE$ = PACK$(BARCODE.TABLE$(1))
    ENDIF

    ! Setup remaining barcodes if any
    IF BARCODE.COUNT% > 1 THEN BEGIN

        !---------------------------------------------------
        ! Write remaining records to the Item EAN File (IEF)
        !---------------------------------------------------
        FOR I% = 2 TO BARCODE.COUNT%

            IF I% = 2 THEN BEGIN
                BCREF$ = IDF.SECOND.BAR.CODE$
            ENDIF ELSE BEGIN
                BCREF$ = IEF.NEXT.BAR.CODE$
            ENDIF


            IEF.BOOTS.CODE.BAR.CODE$ = PACK$(LEFT$(CURR.BOOTS.CODE$, 6)) + \
                                       BCREF$

            IEF.NEXT.BAR.CODE$ = PACK$(BARCODE.TABLE$(I%))

            RC% = WRITE.IEF

            IF RC% <> 0 THEN BEGIN
                CALL LOG.EVENT(106)
            ENDIF

        NEXT I%

        IEF.BOOTS.CODE.BAR.CODE$ = PACK$(LEFT$(CURR.BOOTS.CODE$, 6)) + \
                                   PACK$(BARCODE.TABLE$(I%-1))
        IEF.NEXT.BAR.CODE$ = PACK$("000000000000") ! Mark last barcode

        RC% = WRITE.IEF

        IF RC% <> 0 THEN BEGIN
            CALL LOG.EVENT(106)
        ENDIF
    ENDIF

    ! The barcode table excludes barcodes that match the item
    ! code, therefore we need to add 1 to the barcode table count.
    IDF.NO.OF.BAR.CODES$ = PACK$(RIGHT$("0000" + STR$(BARCODE.COUNT%+1),4))

    RC% = WRITE.IDF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF

    ! IF the item status has changed                                        !DMW
    IF ITEM.STATUS.CHANGED THEN BEGIN                                       !DMW
                                                                            !DMW
        STOCK.BOOTS.CODE$ = IDF.BOOTS.CODE$                                 !DMW
                                                                            !DMW
        RC% = READ.STOCK.LOCK                                               !DMW
                                                                            !DMW
        ! IF the STOCK read was successful                                  !DMW
        IF RC% = 0 THEN BEGIN                                               !DMW
                                                                            !DMW
            ! Update the item status on the STOCK file                      !DMW
            STOCK.STATUS.1$ = CURR.STATUS.1$                                !DMW
                                                                            !DMW
            RC% = WRITE.STOCK.UNLOCK                                        !DMW
                                                                            !DMW
            ! IF the STOCK update failed                                    !DMW
            IF RC% <> 0 THEN BEGIN                                          !DMW
                CALL DO.MESSAGE("PSB21 *** WARNING unable to update " + \   !DMW
                                "STOCK item status for item: " +        \   !DMW
                                CURR.BOOTS.CODE$,FALSE)                     !DMW
                CALL LOG.EVENT(106)                                         !DMW
            ENDIF                                                           !DMW
                                                                            !DMW
        ENDIF                                                               !DMW
                                                                            !DMW
    ENDIF                                                                   !DMW

    RC% = UPDATE.ALL.IRF.BAR.CODES(CURR.BOOTS.CODE$)

    IF RC% <> 0 THEN BEGIN                                     ! 1.14 RC (200)
        CALL DO.MESSAGE("PSB21 *** ERROR in PSBF41 " + \       ! 1.14 RC (200)
                        "updating IRF barcodes for item: " + \ ! 1.14 RC (200)
                         CURR.BOOTS.CODE$, FALSE)              ! 1.14 RC (200)
        JOBSOK.FLAG$ = "Y"                                     ! 1.14 RC (200)
        EXIT SUB                                               ! 1.14 RC (200)
    ENDIF                                                      ! 1.14 RC (200)

    EXIT SUB

 BARCODE.ERROR:

    IF DELETING.IEF.CHAIN THEN BEGIN
        CALL DO.MESSAGE("PSB21 *** WARNING skipping IEF barcode chain deletion", FALSE)
        CALL DO.MESSAGE("PSB21 *** WARNING adding new barcodes for item", FALSE)
        RESUME ADD.BARCODES
    ENDIF ELSE BEGIN
        CALL DO.MESSAGE("PSB21 *** ERROR unable to update IDF/IEF barcode chain for item: " + \
                         CURR.BOOTS.CODE$, FALSE)
        EXIT SUB
    ENDIF

END SUB


\******************************************************************************
\***
\***    CHECK.FOR.PRICE.MISMATCH
\***
\******************************************************************************

FUNCTION CHECK.FOR.PRICE.MISMATCH PUBLIC ! Entire function new for Rv 1.12 RC

    INTEGER*1 CHECK.FOR.PRICE.MISMATCH

    INTEGER*1 CIPPM.ITEM.FOUND


    CHECK.FOR.PRICE.MISMATCH = FALSE


!   If IRF Local Price flag ON check LOCAL for price mis-match with IUF
    IF (IRF.INDICAT3% AND 20h) = 20h THEN BEGIN ! Local Price ON

        LOCAL.ITEM.CODE$ = PACK$(RIGHT$("00000000" + CURR.BOOTS.CODE$, 8))

        IF READ.LOCAL = 0 THEN BEGIN ! Item found

            IF VAL(UNPACK$(LOCAL.H.O.PRICE$)) <> VAL(CURR.CURRENT.PRICE$) THEN BEGIN
                CHECK.FOR.PRICE.MISMATCH = TRUE
            ENDIF

            EXIT FUNCTION

        ENDIF ELSE BEGIN ! Item not found

!           If item not on LOCAL file then correct IRF flag
!           (ahead of WRITE.IRF within BARCODE.PROCESSING)
!           and allow remaining price checks to be done
            IRF.INDICAT3% = IRF.INDICAT3% AND 11011111B ! Set bit X'20' OFF

        ENDIF

    ENDIF


!   If IDF Markdown flag ON then check CIPPM for price mis-match with IUF
    IF (IDF.BIT.FLAGS.1% AND 20h) = 20h THEN BEGIN ! Markdown ON

        CIPPM.ITEM.FOUND = FALSE

        IF CIPPM.PRESENT THEN BEGIN                            ! 1.15 RC (226)

            WHILE NOT CIPPM.ITEM.FOUND

                IF END # CIPPM.SESS.NUM% THEN IF.END.CIPPM
                READ # CIPPM.SESS.NUM%; LINE CIPPM.RCD$

                IF LEFT$(CIPPM.RCD$,8) = "R" + RIGHT$(CURR.BOOTS.CODE$, 7) THEN BEGIN
                    CIPPM.ITEM.FOUND = TRUE
                ENDIF

            WEND

IF.END.CIPPM:

!           Reset file pointer to start of file for future usage
            POINT CIPPM.SESS.NUM%; 0

        ENDIF                                                  ! 1.15 RC (226)

        IF CIPPM.ITEM.FOUND THEN BEGIN

            IF VAL(MID$(CIPPM.RCD$, 17, 8)) <> VAL(CURR.CURRENT.PRICE$) THEN BEGIN
                CHECK.FOR.PRICE.MISMATCH = TRUE
            ENDIF

            EXIT FUNCTION

        ENDIF ELSE BEGIN

!           If item not on CIPPM file then correct the IDF flag
!           (ahead of WRITE.IDF within BARCODE.PROCESSING)
!           and allow remaining price check to be done
!           IDF flag is also corrected when CIPPM file is      ! 1.15 RC (226)
!           not present (eg, store not on CIP markdown)        ! 1.15 RC (226)
            IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1% AND 11011111b ! Markdown X'20' OFF

        ENDIF

    ENDIF


!   If item not on LOCAL or CIP Markdown check IRF for price mis-match with IUF

    IF VAL(UNPACK$(IRF.SALEPRIC$)) <> VAL(CURR.CURRENT.PRICE$) THEN BEGIN
        CHECK.FOR.PRICE.MISMATCH = TRUE
    ENDIF


END FUNCTION


\******************************************************************************
\***
\***    UPDATE.ISF
\***
\******************************************************************************
\***
\***    Updates ISF - Item Shelf Edge Label File for the specified item
\***
\******************************************************************************
SUB UPDATE.ISF

    ISF.BOOTS.CODE$ = PACK$("0" + CURR.BOOTS.CODE$) !4-bytes UPD

    RC% = READ.ISF ! Sets ISF.SEL.PRINTED.FLAG$ from current value            ! 1.5 RC (51)

    IF RC% <> 0 THEN BEGIN ! Existing item with no ISF record                 ! 1.5 RC (51)
                           ! or completely new item                           ! 1.5 RC (51)
        ISF.SEL.PRINTED.FLAG$ = "N"                                           ! 1.5 RC (51)
!       No event 106 logged since new ISF record is being created             ! 1.5 RC (51)
    ENDIF                                                                     ! 1.5 RC (51)

!   Save data from READ.ISF (even if null or blank)            ! 1.13 RC (179)
    READ.ISF.S.E.DESC$         = ISF.S.E.DESC$                 ! 1.13 RC (179)
    READ.ISF.ITEM.QTY$         = ISF.ITEM.QTY$                 ! 1.13 RC (179)
    READ.ISF.UNIT.MEASUREMENT$ = ISF.UNIT.MEASUREMENT$         ! 1.13 RC (179)
    READ.ISF.UNIT.NAME$        = ISF.UNIT.NAME$                ! 1.13 RC (179)

    ISF.S.E.DESC$   = LEFT$(CURR.S.E.DESC$ + STRING$(45, " "),45)

    ITEM.ON.WEEE.TABLE = FALSE

    IF WEEE.ITEM.COUNT% > 0 THEN BEGIN  ! Check if item is present on the WEEE table

        FOR I% = 1 TO WEEE.ITEM.COUNT%

            IF CURR.BOOTS.CODE$ = LEFT$(WEEEUF.TABLE$(I%),7) THEN BEGIN
                ITEM.ON.WEEE.TABLE = TRUE
                WIX% = I%
            ENDIF

            ! Jump out if found Nb. WEEE Table in ascending item code order
            IF ITEM.ON.WEEE.TABLE = TRUE OR \
              CURR.BOOTS.CODE$ < LEFT$(WEEEUF.TABLE$(I%),7) THEN BEGIN
                I% = WEEE.ITEM.COUNT% + 1
            ENDIF

        NEXT I%

    ENDIF

    IF ITEM.ON.WEEE.TABLE THEN BEGIN

        IRF.INDICAT8% = IRF.INDICAT8%  OR 10000000B  !Set   bit 8 WEEE Item
        !-----------------------
        !    ISF WEEE Setup
        !-----------------------
        ISF.UNIT.NAME$        = "WEEE      "                                                 ! 10-bytes
        ISF.ITEM.QTY$         = PACK$(RIGHT$("00000000" + RIGHT$(WEEEUF.TABLE$(WIX%),4), 8)) ! 4-bytes UPD
        ISF.UNIT.MEASUREMENT$ = PACK$("0000")                                                ! 2-bytes UPD

    ENDIF ELSE BEGIN
        IRF.INDICAT8% = IRF.INDICAT8% AND 01111111B  !Clear bit 8  Non-WEEE Item

        ISF.ITEM.QTY$         = PACK$(RIGHT$("00000000" + CURR.ITEM.QTY$, 8))    ! 4-bytes UPD
        ISF.UNIT.MEASUREMENT$ = PACK$(RIGHT$("0000" + CURR.UNIT.MEASUREMENT$,4)) ! 2-bytes UPD
        ISF.UNIT.NAME$        = LEFT$(CURR.UNIT.NAME$ + STRING$(10," "),10)      ! 10-bytes

    ENDIF

!   Set SEL.NON.PRICE.CHANGE flag if non-price-change requires SEL printing      ! 1.13 RC (179)
!   (note that this flag is not checked/used when item is new)                   ! 1.13 RC (179)
    IF   ISF.S.E.DESC$         <> READ.ISF.S.E.DESC$ \                           ! 1.13 RC (179)
      OR ISF.ITEM.QTY$         <> READ.ISF.ITEM.QTY$ \                           ! 1.13 RC (179)
      OR ISF.UNIT.MEASUREMENT$ <> READ.ISF.UNIT.MEASUREMENT$ \                   ! 1.13 RC (179)
      OR ISF.UNIT.NAME$        <> READ.ISF.UNIT.NAME$ THEN BEGIN                 ! 1.13 RC (179)
        SEL.NON.PRICE.CHANGE = TRUE                                              ! 1.13 RC (179)
    ENDIF                                                                        ! 1.13 RC (179)

!   ISF.SEL.PRINTED.FLAG$ now set earlier in procedure                           ! 1.5 RC (51)

    RC% = WRITE.ISF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF

END SUB


!   SUB UPDATE.LOCAL deleted                                    ! 1.7 RC (113)


\******************************************************************************
\***
\***    UPDATE.CIPPM
\***
\******************************************************************************

FUNCTION UPDATE.CIPPM(CIP.ITEM$, REV.PRICE$) PUBLIC             ! 1.11 RC (172)

    INTEGER*1 COMPLETE
    INTEGER*1 UPDATE.CIPPM
    INTEGER*2 L%
    INTEGER*4 POINTER%
    INTEGER*4 POS%
    STRING    CIPPM.DATA$
    STRING    CIP.ITEM$                                         ! 1.11 RC (172)
    STRING    REV.PRICE$                                        ! 1.10 RC (113)

    ON ERROR GOTO FUNCTION.ERROR

    UPDATE.CIPPM = 1                       ! Line re-positioned ! 1.15 RC (226)

!   Cater for stores with no CIPPMR (eg, not on CIP markdown)   ! 1.15 RC (226)
    IF NOT CIPPM.PRESENT THEN BEGIN                             ! 1.15 RC (226)
!       If CIPPMR not present then correct the IDF flag         ! 1.15 RC (226)
!       Set IDF Markdown flag OFF and update IDF                ! 1.15 RC (226)
        IDF.BIT.FLAGS.1% = \                                    ! 1.15 RC (226)
          IDF.BIT.FLAGS.1% AND 11011111b ! Markdown X'20' OFF   ! 1.15 RC (226)
!       IDF will have been read from PROCESS.BTREE.RECORD$      ! 1.15 RC (226)
!       prior to call UPDATE.CIPPM so write unlikely to fail    ! 1.15 RC (226)
!       IF write does fail do not abend program as next run     ! 1.15 RC (226)
!       will re-attempt correction                              ! 1.15 RC (226)
        CALL WRITE.IDF                                          ! 1.15 RC (226)
        EXIT FUNCTION                                           ! 1.15 RC (226)
    ENDIF                                                       ! 1.15 RC (226)

      COMPLETE = FALSE                      ! Line re-positoned ! 1.15 RC (226)

      WHILE NOT COMPLETE
           IF END # CIPPM.SESS.NUM% THEN NO.CIPPM.RECORD
           READ #CIPPM.SESS.NUM%; LINE CIPPM.RCD$
           POINTER% = PTRRTN !Start of record pointer saved to use in direct write
           IF LEFT$(CIPPM.RCD$,8) = "R" + CIP.ITEM$ THEN BEGIN  ! 1.11 RC (172)
                 CLOSE CIPPM.SESS.NUM%
                 OPEN CIPPM.FILE.NAME$ DIRECT RECL 1 AS CIPPM.SESS.NUM% LOCKED
!                Update CIPPMR Reversals Price                                                 ! 1.10 RC (113)
                 FOR L% = 1 TO 8
                      WRITE FORM "C1"; #CIPPM.SESS.NUM%, POINTER%+16+L%; MID$(REV.PRICE$,L%,1) ! 1.10 RC (113)
                 NEXT L%
                 COMPLETE        = TRUE
                 UPDATE.CIPPM    = 0
           ENDIF
      WEND

LEAVE.HERE:
NO.CIPPM.RECORD:

            IF NOT COMPLETE THEN BEGIN
                POINT CIPPM.SESS.NUM%;0
!               Item not on CIPPMR so Markdown flag wrong              ! 1.9 RC (109)
!               Set IDF Markdown flag OFF and update IDF               ! 1.9 RC (109) ! 1.15 RC
                IDF.BIT.FLAGS.1% = \                                   ! 1.9 RC (109)
                  IDF.BIT.FLAGS.1% AND 11011111b ! Markdown X'20' OFF  ! 1.9 RC (109)
!               IDF will have been read from PROCESS.BTREE.RECORD$     ! 1.1O RC (113)
!               prior to call UPDATE.CIPPM so write unlikely to fail   ! 1.1O RC (113)
!               IF write does fail do not abend program as next run    ! 1.1O RC (113)
!               will re-attempt correction                             ! 1.10 RC (113)
                CALL WRITE.IDF                                         ! 1.10 RC (113)
            ENDIF ELSE BEGIN
                CLOSE CIPPM.SESS.NUM%
                OPEN CIPPM.FILE.NAME$ AS CIPPM.SESS.NUM% LOCKED
            ENDIF
            EXIT FUNCTION

FUNCTION.ERROR:

    ! LOG EVENT THEN LEAVE.
    CALL DO.MESSAGE("PSB21 *** ERROR processing CIPPM file", FALSE)
    CALL LOG.EVENT(106)
    RESUME LEAVE.HERE

END FUNCTION


\******************************************************************************
\***
\***    UPDATE.IRF
\***
\******************************************************************************
\***
\***    Updates IRF - Item Reference File for the specified item
\***
\******************************************************************************
SUB UPDATE.IRF

    INTEGER*2 COMMA.OFFSET%
    INTEGER*2 RB.OFF%
    STRING    BCF.MARKER$

!   UPDATE.IRF populates IRF variables and updates IRF with an initial record      ! 1.14 RC (200)
!   This IRF record is read later by PSBF41 during UPDATE.ALL.IRF.BAR.CODES.       ! 1.14 RC (200)
!   PSBF41 sets IRF.BAR.CODE$ from the item code and non-group-code prefix ...     ! 1.14 RC (200)
!     IRF.BAR.CODE$ = PACK$(STRING$(16,"0")) + PACK$(LEFT$(UPDATE.ITEM.CODE$,6))   ! 1.14 RC (200)
!   Therefore UPDATE.IRF must use this barcode on the IRF record it creates.       ! 1.14 RC (200)
!   Note that UPDATE.IRF calls WRITE.IRF as opposed to PSBF19 UPDT.IRF because     ! 1.14 RC (200)
!   the latter is called by PSBF41 and re-updates the initial IRF record.          ! 1.14 RC (200)

!   IF CURR.GRP.CODE.FLAG$ = "N" THEN BEGIN                                        ! 1.14 RC (200)
        IRF.BAR.CODE$ = PACK$(STRING$(16, "0") + LEFT$(CURR.BOOTS.CODE$, 6))
!   ENDIF ELSE BEGIN                                                               ! 1.14 RC (200)
!       IRF.BAR.CODE$ = PACK$("2" + STRING$(15, "0") + LEFT$(CURR.BOOTS.CODE$, 6)) ! 1.14 RC (200)
!   ENDIF                                                                          ! 1.14 RC (200)

    RC% = READ.IRF
    IF RC% <> 0 THEN BEGIN
!       Main item-barcode IRF record does not exist                           ! 1.14 RC (200)
!       Debugger investigation indicates a previous items data may sometimes  ! 1.14 RC (200)
!       be retained within the IRF variables when the IRF READ fails          ! 1.14 RC (200)
!       In this case initialise all IRF variables unless specifically set     ! 1.14 RC (200)
!       later within UPDATE.IRF                                               ! 1.14 RC (200)

!       IRF.BAR.CODE$         ! Set prior to READ.IRF                         ! 1.14 RC (200)
        IRF.INDICAT0%     = 0                                                 ! 1.14 RC (200)
        IRF.INDICAT1%     = 0                                                 ! 1.14 RC (200)
!       IRF.DEAL.DATA(0)  = 0 ! Set via IRF.DEAL.NUM% and IRF.LIST.ID%        ! 1.14 RC (200)
        IRF.INDICAT8%     = 0                                                 ! 1.14 RC (200)
        IRF.INDICAT9%     = 0                                                 ! 1.14 RC (200)
        IRF.INDICAT10%    = 0                                                 ! 1.14 RC (200)
        IRF.SALEPRIC$ = PACK$(RIGHT$("0000000000" + CURR.CURRENT.PRICE$, 10)) ! 1.14 RC (200)
        IRF.INDICAT5%     = 0                                                 ! 1.14 RC (200)
!       IRF.ITEMNAME$         ! Set later within UPDATE.IRF                   ! 1.14 RC (200)
!       IRF.BOOTS.CODE$       ! Set later within UPDATE.IRF                   ! 1.14 RC (200)
!       IRF.DEAL.DATA%(1) = 0 ! Set via IRF.DEAL.NUM% and IRF.LIST.ID%        ! 1.14 RC (200)
!       IRF.DEAL.DATA%(2) = 0 ! Set via IRF.DEAL.NUM% and IRF.LIST.ID%        ! 1.14 RC (200)
        IRF.INDICAT3%     = 0                                                 ! 1.14 RC (200)

        FOR K% = 0 TO IRF.MAX.DEALS% - 1                                      ! 1.14 RC (200)
            IRF.DEAL.NUM$(K%) = PACK$("0000")                                 ! 1.14 RC (200)
            IRF.LIST.ID%(K%)  = 0                                             ! 1.14 RC (200)
        NEXT K%                                                               ! 1.14 RC (200)

!       Set Item Movement flag ON as is done for completely new item          ! 1.14 RC (200)
        IRF.INDICAT0% = IRF.INDICAT0%  OR 10000000B ! Item movement ON        ! 1.14 RC (200)

    ENDIF ELSE BEGIN
!       Redundant line removed                                                ! 1.14 RC (200)
        IF NOT IUF.NEW.FORMAT THEN BEGIN ! Legacy IUF                         ! 1.6 RC (71)
!           For legacy IUF ...                                                ! 1.6 RC (71)
!           READ.IUF sets IUF.UNRESTRICTED.GROUP.CODE$ to default value "N"   ! 1.6 RC (71)
!           This is fine for a new item but for an existing item set the      ! 1.6 RC (71)
!           value to "Y" if so set on the IRF record (ie, retain IRF value)   ! 1.6 RC (71)
            IF (IRF.INDICAT8% AND 00000100B) = 00000100B THEN BEGIN           ! 1.6 RC (71)
                CURR.UNRESTRICTED.GROUP.CODE$ = "Y"                           ! 1.6 RC (71)
            ENDIF                                                             ! 1.6 RC (71)
        ENDIF                                                                 ! 1.6 RC (71)
    ENDIF

    !--------------------------------------------------
    !                     INDICAT 0
    !--------------------------------------------------
    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT0% = IRF.INDICAT0%  OR 10000000B  !Set   bit 8 Item movement kept
        IRF.INDICAT0% = IRF.INDICAT0% AND 10111111B  !Clear bit 7 Enforced Price entry
    ENDIF

    !Set bit 7 (Enforced quantity entry flag) for group restricted items!GRN

    IF CURR.RESTRICT.SALES.FLAG$ = "Y" THEN BEGIN                       !GRN
        IRF.INDICAT0% = IRF.INDICAT0% OR 01000000B                      !GRN

    ENDIF ELSE BEGIN                                                    !GRN

        !If the bit 7 is already set then reset this flag; as the item  !GRN
        !is now configured as an unrestricted item.                     !GRN

        IF IRF.INDICAT0% AND 01000000B THEN BEGIN                       !GRN
            IRF.INDICAT0% = IRF.INDICAT0% AND 10111111B                 !GRN
        ENDIF                                                           !GRN
    ENDIF                                                               !GRN

    IF CURR.ENF.PRICE.ENTRY$ = "Y" THEN BEGIN
        IRF.INDICAT0% = IRF.INDICAT0%  OR 00100000B  !Set   bit 6 Enforced Item entry
    ENDIF ELSE BEGIN
        IRF.INDICAT0% = IRF.INDICAT0% AND 11011111B  !Clear bit 6
    ENDIF

    IF MATCH("RECALL IS ACTIVE",SOFTS.REC.62$,1) THEN BEGIN !NB. Rec 62 contains >1 ACTIVE/line
        IF CURR.BLOCKED.FROM.SALE$ = "R" AND IUF.NEW.FORMAT THEN BEGIN      !On RECALL
            IRF.INDICAT0% = IRF.INDICAT0%  OR 00010000B  !Set   bit 5 Blocked from sale due to recall
        ENDIF ELSE BEGIN
            IRF.INDICAT0% = IRF.INDICAT0% AND 11101111B  !Clear bit 5
        ENDIF
    ENDIF ELSE BEGIN
        IF NEW.ITEM THEN BEGIN
            IRF.INDICAT0% = IRF.INDICAT0% AND 11101111B  !Clear bit 5
        ENDIF
    ENDIF

    IF CURR.CONTAINS.ALCOHOL$ = "Y" THEN BEGIN
        IRF.INDICAT0% = IRF.INDICAT0%  OR 00001000B  !Set   bit 4 Alcohol flag
    ENDIF ELSE BEGIN
        IRF.INDICAT0% = IRF.INDICAT0% AND 11110111B  !Clear bit 4
    ENDIF

    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT0% = IRF.INDICAT0% AND 11111011B  !Clear bit 3 Returnable
    ENDIF

    !IF CURR.RETURNABLE$ = "N" THEN BEGIN                                 ! BCSk
    !    IRF.INDICAT0% = IRF.INDICAT0%  OR 00000100B  !Set   bit 3 Not    ! BCSk
    !                                                 ! returnable        ! BCSk
    !ENDIF                                                                ! BCSk

    !-----------------------------------------------------                ! BCSk
    ! If the item's product group is in the list of Gift                  ! BCSk
    ! Card Mall product groups, then override the settings                ! BCSk
    !-----------------------------------------------------                ! BCSk
    IF MATCH(CURR.PROD.GRP$, GCM.PG.LIST$, 1) <> 0 THEN BEGIN             ! CCSk
        CURR.GIFTCARD$   = "Y"                                            ! BCSk
        IRF.INDICAT0% = IRF.INDICAT0%  OR 00000100B  !Set   bit 3 Not     ! BCSk
                                                     ! returnable         ! BCSk
    ENDIF                                                                 ! BCSk


    IF CURR.GIVEAWAY$ = "Y" THEN BEGIN
        IRF.INDICAT0% = IRF.INDICAT0%  OR 00000010B  !Set   bit 2 Giveaway flag
    ENDIF ELSE BEGIN
        IRF.INDICAT0% = IRF.INDICAT0% AND 11111101B  !Clear bit 2
    ENDIF


    IRF.INDICAT0% = IRF.INDICAT0% AND 11111110B  !Clear bit 1 Statins flag       !1.5CSk(a)
    IF NO.OF.STATINS% > 0 THEN BEGIN                                             !1.5CSk(a)
        FOR J% = 1 TO NO.OF.STATINS%                                             !1.5CSk(a)
            IF MATCH(STATINS$(J%),CURR.STNDRD.DESC$,1) THEN BEGIN                !1.5CSk(a)
                !Set bit 1 Statins flag                                          !1.5CSk(a)
                IRF.INDICAT0% = IRF.INDICAT0%  OR 00000001B                      !1.5CSk(a)
            ENDIF                                                                !1.5CSk(a)
        NEXT J%                                                                  !1.5CSk(a)
    ENDIF                                                                        !1.5CSk(a)

    !--------------------------------------------------
    !                     INDICAT 1
    !--------------------------------------------------
    IF CURR.CONTAINS.IBUPROFEN$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 10000000B  !Set   bit 8 Ibuprofen flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 01111111B  !Clear bit 8
    ENDIF

    IF CURR.INSURANCE$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 01000000B  !Set   bit 7 Insurance flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 10111111B  !Clear bit 7
    ENDIF

    IF CURR.CONTAINS.NONSOLID.PAINKILLER$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 00100000B  !Set   bit 6 Non-solid painkiller flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 11011111B  !Clear bit 6
    ENDIF

    !Use data from previous SOFTS read
    IF MATCH("WITHDRAWN IS ACTIVE",SOFTS.REC.62$,1) THEN BEGIN !NB. Rec 62 contains >1 ACTIVE/line
        IF CURR.BLOCKED.FROM.SALE$ = "W" AND IUF.NEW.FORMAT THEN BEGIN      ! Withdrawn
            IRF.INDICAT1% = IRF.INDICAT1%  OR 00010000B  !Set   bit 5 Item withdrawn from sale
        ENDIF ELSE BEGIN
            IRF.INDICAT1% = IRF.INDICAT1% AND 11101111B  !Clear bit 5
        ENDIF
    ENDIF ELSE BEGIN
        IF NEW.ITEM THEN BEGIN
            IRF.INDICAT1% = IRF.INDICAT1% AND 11101111B  !Clear bit 5
        ENDIF
    ENDIF

    IF CURR.GIFTCARD$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 00001000B  !Set   bit 4 Giftcard flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 11110111B  !Clear bit 4
    ENDIF

    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  AND 11111011B  !Clear bit 3 TPLU inclusion
    ENDIF

    IF CURR.CONTAINS.PARACETAMOL$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 00000010B  !Set   bit 2 Paracetamol flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 11111101B  !Clear bit 2
    ENDIF

    IF CURR.CONTAINS.ASPIRIN$ = "Y" THEN BEGIN
        IRF.INDICAT1% = IRF.INDICAT1%  OR 00000001B  !Set   bit 1 Aspirin flag
    ENDIF ELSE BEGIN
        IRF.INDICAT1% = IRF.INDICAT1% AND 11111110B  !Clear bit 1
    ENDIF

    IF NEW.ITEM THEN BEGIN  !Initialise Deals Data
         FOR K% = 0 TO IRF.MAX.DEALS% - 1
             IRF.DEAL.NUM$(K%) = PACK$("0000")
             IRF.LIST.ID%(K%)  = 0
         NEXT K%
    ENDIF

    !--------------------------------------------------
    !                 IRF INDICAT 8
    !--------------------------------------------------

    CALL UPDATE.ISF ! Set bit 8 WEEE Item
                    ! Also sets SEL.NON.PRICE.CHANGE to TRUE if needed  ! 1.13 RC (179)

    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT8% = IRF.INDICAT8% AND 10011111B  !Clear bits 6 & 7
    ENDIF

    IF MATCH("RECALL IS ACTIVE",SOFTS.REC.62$,1) THEN BEGIN !NB. Rec 62 contains >1 ACTIVE/line

        IF CURR.BLOCKED.FROM.SALE$ = "R" THEN BEGIN         ! RECALL

            IRF.INDICAT8% = IRF.INDICAT8% AND 10011111B    !Clear bits 6 & 7 Item blocked from sale

        ENDIF
    ENDIF

    IF CURR.CONTAINS.EPHEDRINE$ = "Y" THEN BEGIN
        IRF.INDICAT8% = IRF.INDICAT8%  OR 00010000B  !Set   bit 5 contains Ephedrine flag
    ENDIF ELSE BEGIN
        IRF.INDICAT8% = IRF.INDICAT8% AND 11101111B  !Clear bit 5
    ENDIF

    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT8% = IRF.INDICAT8% AND 11110111B  !Clear bit 4 Till prompt required
    ENDIF

    IF CURR.UNRESTRICTED.GROUP.CODE$ = "Y" THEN BEGIN
        IRF.INDICAT8% = IRF.INDICAT8%  OR 00000100B  !Set   bit 3 unrestricted group flag
    ENDIF ELSE BEGIN
        IRF.INDICAT8% = IRF.INDICAT8% AND 11111011B  !Clear bit 3
    ENDIF

    ! Chlamydia Test Kits
    IRF.INDICAT8% = IRF.INDICAT8% AND 11111100B              !Clear bits 1 & 2                !1.5CSk(a)
    !Check against field one                                                                  !1.5CSk(a)
    IF MATCH(CHLAMYDIA.ID$(1),CURR.STNDRD.DESC$,1) THEN BEGIN!IDF Description                 !1.5CSk(a)
        IRF.INDICAT8% = IRF.INDICAT8%  OR 00000001B          !Set bit 1 (CN) NHS Test Kit     !1.5CSk(a)
    ENDIF                                                                                     !1.5CSk(a)
    !Check against field two                                                                  !1.5CSk(a)
    IF MATCH(CHLAMYDIA.ID$(2),CURR.STNDRD.DESC$,1) THEN BEGIN!IDF Description                 !1.5CSk(a)
        IRF.INDICAT8% = IRF.INDICAT8%  OR 00000010B          !Set bit 2 (CO) Private Test Kit !1.5CSk(a)
    ENDIF                                                                                     !1.5CSk(a)

    !--------------------------------------------------
    !                 IRF INDICAT 9
    !--------------------------------------------------
    IF CURR.RESALEABLE$ = "Y" THEN BEGIN
        IRF.INDICAT9% = IRF.INDICAT9%  OR 01000000B  !Set   bit 7 item resaleable flag
    ENDIF ELSE BEGIN
        IRF.INDICAT9% = IRF.INDICAT9% AND 10111111B  !Clear bit 7
    ENDIF

    IF CURR.BOOTS.COM.EXTENDED$ = "Y" THEN BEGIN
        IRF.INDICAT9% = IRF.INDICAT9%  OR 10000000B  !Set   bit 8  .COM Extended flag
    ENDIF ELSE BEGIN
        IRF.INDICAT9% = IRF.INDICAT9% AND 01111111B  !Clear bit 8
    ENDIF

    !Special Instructions - set bits 1-6 (value range 0-63)
    IRF.INDICAT9% = (IRF.INDICAT9% AND 11000000B) + VAL(CURR.SPECIAL.INSTRUCTION$)

    !--------------------------------------------------
    !                 IRF INDICAT 10
    !--------------------------------------------------
    IRF.INDICAT10% = IRF.INDICAT10% AND 00011111B !Clear bits 6, 7 & 8 (Ret Route blank)

    IF CURR.RETURNABLE$ = "Y" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10% OR 00100000B !Set bit 6 Is Returnable

        IF CURR.RETURN.ROUTE$ = "R" THEN BEGIN
            IRF.INDICAT10% = IRF.INDICAT10% OR  01000000B    !set   bit 7
        ENDIF ELSE IF CURR.RETURN.ROUTE$ = "D" THEN BEGIN
            IRF.INDICAT10% = IRF.INDICAT10% OR  10000000B    !set   bit 8
        ENDIF ELSE IF CURR.RETURN.ROUTE$ = "S" THEN BEGIN
            IRF.INDICAT10% = IRF.INDICAT10% OR 11000000B     !set bits 7 & 8
        ENDIF
    ENDIF

    IRF.INDICAT10% = IRF.INDICAT10% AND 11100111B !clear bits 4 & 5
    IF CURR.ETHICAL.CLASS$ = "P" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00001000B       !set bit 4
    ENDIF ELSE IF CURR.ETHICAL.CLASS$ = "G" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00010000B       !set bit 5
    ENDIF ELSE IF CURR.ETHICAL.CLASS$ = "M" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00011000B       !set bit 4 & 5
    ENDIF


    IRF.INDICAT10% = IRF.INDICAT10% AND 11111000B !clear bits 1, 2 & 3

    IF CURR.AGE.RESTRICTION$ = "12" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00000001B !set   bit 1
    ENDIF ELSE IF CURR.AGE.RESTRICTION$ = "15" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00000010B !set   bit 2
    ENDIF ELSE IF CURR.AGE.RESTRICTION$ = "16" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00000011B !set   bits 1 & 2
    ENDIF ELSE IF CURR.AGE.RESTRICTION$ = "18" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00000100B !set   bit 3
    ENDIF ELSE IF CURR.AGE.RESTRICTION$ = "21" THEN BEGIN
        IRF.INDICAT10% = IRF.INDICAT10%  OR 00000101B !set   bits 1 & 3
    ENDIF

!   Line deleted                                                                       ! 1.12 RC

    IF CURR.CURRENT.PRICE$ = "" THEN BEGIN
        IRF.SALEPRIC$ = PACK$("0000000000") ! Zero rather than 1p to cause price entry ! 1.9 RC
    ENDIF ELSE BEGIN ! Set whether new item or not
        IF NEW.ITEM THEN BEGIN
            IRF.SALEPRIC$ = PACK$(RIGHT$("0000000000" + CURR.CURRENT.PRICE$, 10))
        ENDIF ELSE BEGIN
            PRICE.MISMATCH = CHECK.FOR.PRICE.MISMATCH                                  ! 1.12 RC
        ENDIF
    ENDIF

    !--------------------------------------------------
    !                 IRF INDICAT 5
    !--------------------------------------------------
    MATCH.POS1% = MATCH("M", CURR.GUARANTEE.LENGTH$,1) !may contain letter M to signify Months
    IF MATCH.POS1% > 0 THEN BEGIN
        IRF.INDICAT5% = IRF.INDICAT5%  OR 10000000B !set   bit 8
        IRF.INDICAT5% = (IRF.INDICAT5% AND 11000000B) + \  bits 1 -6
            VAL(LEFT$(CURR.GUARANTEE.LENGTH$, MATCH.POS1%-1))
    ENDIF ELSE BEGIN
        IRF.INDICAT5% = IRF.INDICAT5% AND 01111111B !clear bit 8
        IRF.INDICAT5% = (IRF.INDICAT5% AND 11000000B) + VAL(CURR.GUARANTEE.LENGTH$)
    ENDIF

    IF CURR.CONTAINS.PSEUDOEPHEDRINE$ = "Y" THEN BEGIN
         IRF.INDICAT5% = IRF.INDICAT5%  OR 01000000B !set   bit 7
    ENDIF ELSE BEGIN
         IRF.INDICAT5% = IRF.INDICAT5% AND 10111111B !clear bit 7
    ENDIF

    IRF.ITEMNAME$ = RIGHT$(STRING$(18," ") + CURR.TILL.DESC$, 18)

    IRF.BOOTS.CODE$ = PACK$(LEFT$(CURR.BOOTS.CODE$, 6))


    !--------------------------------------------------
    !                 IRF INDICAT 3
    !--------------------------------------------------
    IF NEW.ITEM THEN BEGIN
        IRF.INDICAT3% = IRF.INDICAT3% AND 01001111B  !clear bits 5 Well-being services item (redundant)
                                                     !           6  Locally priced
                                                     !           8  CSR item (redundant)
    ENDIF

    IF CURR.STOCK.SYSTEM.FLAG$ = "Y" THEN BEGIN
         IRF.INDICAT3% = IRF.INDICAT3%  OR 01000000B !set   bit 7
    ENDIF ELSE BEGIN
         IRF.INDICAT3% = IRF.INDICAT3% AND 10111111B !clear bit 7
    ENDIF

    IF CURR.EARN.POINTS$ = "Y" THEN BEGIN
         IRF.INDICAT3% = IRF.INDICAT3% AND 11110111B !clear bit 4         !1.4CSk
    ENDIF ELSE BEGIN
         IRF.INDICAT3% = IRF.INDICAT3%  OR 00001000B !set   bit 4         !1.4CSk
    ENDIF

!   Save IRF.INDICAT3%                                                       ! 1.13 RC (179)
    SAV.IRF.INDICAT3% = IRF.INDICAT3%                                        ! 1.13 RC (179)

    IF CURR.REDEEMABLE$ = "Y" THEN BEGIN
         IRF.INDICAT3% = IRF.INDICAT3%  OR 00000100B !set   bit 3
    ENDIF ELSE BEGIN
         IRF.INDICAT3% = IRF.INDICAT3% AND 11111011B !clear bit 3
    ENDIF

!   Set SEL.NON.PRICE.CHANGE flag if non-price-change requires SEL printing  ! 1.13 RC (179)
!   (note that this flag is not checked/used when item is new)               ! 1.13 RC (179)
    IF   (    IRF.INDICAT3% AND 00000100B) \                                 ! 1.13 RC (179)
      <> (SAV.IRF.INDICAT3% AND 00000100B) THEN BEGIN                        ! 1.13 RC (179)
        SEL.NON.PRICE.CHANGE = TRUE                                          ! 1.13 RC (179)
    ENDIF                                                                    ! 1.13 RC (179)

    IF CURR.OWN.BRAND$ = "Y" THEN BEGIN
         IRF.INDICAT3% = IRF.INDICAT3%  OR 00000010B !set   bit 2
    ENDIF ELSE BEGIN
         IRF.INDICAT3% = IRF.INDICAT3% AND 11111101B !clear bit 2
    ENDIF

!   READ.IUF code ensures IUF.DISCOUNTABLE$ (and hence CURR.DISCOUNTABLE$)
!   only contains "Y" or "N"

    IF CURR.DISCOUNTABLE$ = "N" THEN BEGIN ! No discount given                ! 1.6 RC (21)
!        Set IRF Discount exempt flag ON                                      ! 1.6 RC (21)
         IRF.INDICAT3% = IRF.INDICAT3%  OR 00000001B !set   bit 1    !1.4CSk  ! 1.5 RC (21)
    ENDIF ELSE BEGIN ! else "Y" - Discount given                              ! 1.6 RC (21)
!        Set IRF Discount Exempt flag OFF                                     ! 1.6 RC (21)
         IRF.INDICAT3% = IRF.INDICAT3% AND 11111110B !clear bit 1    !1.4CSk  ! 1.5 RC (21)
    ENDIF

!   Lines deleted - UPDATE.CIPPM moved to PROCESS.BTREE.RECORD to ensure ALL  ! 1.10 RC (113)
!                   PPFK items checked for update and not just those on IUF   ! 1.10 RC (113)

    RC% = WRITE.IRF
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF

END SUB


\******************************************************************************
\***
\***    UPDATE.IDF
\***
\******************************************************************************
\***
\***    Updates IDF - Item Data File for the specified item
\***
\******************************************************************************
SUB UPDATE.IDF

!   Removed OVERRIDE.NEW.ITEM.FLAG now redundant                 ! 1.8 RC (95)

    IDF.BOOTS.CODE$ = PACK$("0" + CURR.BOOTS.CODE$)

!   UPDATE.IDF is now called prior to UPDATE.IRF                 ! 1.8 RC (95)
!   NEW.ITEM flag now set when IDF record not present (not IRF)  ! 1.8 RC (95)
    RC% = READ.IDF
    IF RC% <> 0 THEN BEGIN                                       ! 1.8 RC (95)
        NEW.ITEM = TRUE                                          ! 1.8 RC (95)
    ENDIF ELSE BEGIN                                             ! 1.9 RC (95)+(102)
        NEW.ITEM = FALSE                                         ! 1.9 RC (95)+(102)
    ENDIF                                                        ! 1.9 RC (95)+(102)

    IDF.PRODUCT.GRP$ = PACK$(LEFT$(CURR.PROD.GRP$,2) + "0" + RIGHT$(CURR.PROD.GRP$,3))
    IDF.STNDRD.DESC$ = CURR.STNDRD.DESC$

    IF NEW.ITEM THEN BEGIN                                       ! 1.8 RC (95)
        IDF.INTRO.DATE$ = PACK$(PROCESSING.DATE$)  !passed from PSB20
        IDF.FILLER$     = PACK$("00") ! Null                     ! 1.9 RC (106)
    ENDIF

    IF CURR.BC.LETTER$ = "" THEN BEGIN
        IDF.BSNS.CNTR$ = " "
    ENDIF ELSE BEGIN
        IDF.BSNS.CNTR$ = CURR.BC.LETTER$
    ENDIF

    !--------------------------------------------------
    !                IDF BIT FLAGS 1
    !--------------------------------------------------
    IF CURR.GRP.CODE.FLAG$ = "Y" THEN BEGIN
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1%  OR 10000000B !set   bit 8  Group Code Flag
    ENDIF ELSE BEGIN
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1% AND 01111111B !clear bit 8
    ENDIF

    IF NEW.ITEM THEN BEGIN                                            ! 1.8 RC (95)
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1%  AND 10010111B !clear bits 4 CSR item (redundant)
                                                            !           6 Markdown
                                                            !           7 Keyline  (redundant)
    ENDIF

    IF CURR.SUPPLY.ROUTE$ = "B" OR CURR.SUPPLY.ROUTE$ = "W" THEN BEGIN
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1%  OR 00010000B !set   bit 5  Supply Route B/D/W
    ENDIF ELSE BEGIN
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1% AND 11101111B !clear bit 5
    ENDIF

    IF CURR.SUPPLY.ROUTE$ = "B" OR CURR.SUPPLY.ROUTE$ = "D" THEN BEGIN
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1%  OR 00000100B !set   bit 3     Directs
    !    IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1% AND 11111100B !clear bits 1 & 2 (redundant)
    ENDIF ELSE BEGIN                                                                      ! 1.6 RC (69)
         IDF.BIT.FLAGS.1% = IDF.BIT.FLAGS.1% AND 11111011B ! Clear bit 3   Directs A off  ! 1.6 RC (69)
    ENDIF

    !--------------------------------------------------
    !                IDF BIT FLAGS 2
    !--------------------------------------------------

    IF NEW.ITEM THEN BEGIN                                       ! 1.8 RC (95)
         IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2% AND 10101000B !clear bits 1 Reserved
         !                                                             2 Reserved
         !                                                             3 Pending Count
         !                                                             5 Unused
         !                                                             7 Exclusive Line Flag
         IDF.PARENT.CODE$       = PACK$("00000000")  !redundant
         IDF.DATE.OF.LAST.SALE$ = PACK$("000000")
    ENDIF

    IF CURR.DATE.SENSITIVE$ = "Y" THEN BEGIN
         IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2%  OR 00100000B !set   bit 6  Date sensitive Flag (previously unused)
    ENDIF ELSE BEGIN
         IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2% AND 11011111B !clear bit 6
    ENDIF

    ITEM.STATUS.CHANGED = FALSE                                             !DMW

    IF CURR.STOCK.SYSTEM.FLAG$ = "Y" THEN BEGIN
        IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2%  OR 00001000B !set   bit 4  Stock System Flag

        ! IF this is an existing item                                       !DMW
        IF NOT NEW.ITEM THEN BEGIN                                          !DMW
                                                                            !DMW
            ! IF the item status has changed                                !DMW
            IF IDF.STATUS.1$ <> CURR.STATUS.1$ THEN BEGIN                   !DMW
                ITEM.STATUS.CHANGED = TRUE                                  !DMW
            ENDIF                                                           !DMW
                                                                            !DMW
        ENDIF                                                               !DMW

    ENDIF ELSE BEGIN
        IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2% AND 11110111B !clear bit 4
        !---------------------------------------------------------------------------------------
        ! If the item was previously a stock item, but is changing to non-stock, write a type 6
        ! record to the STKMQ with the reason set to "I" in order to reset the stock figure.
        !---------------------------------------------------------------------------------------
        STOCK.BOOTS.CODE$ = PACK$("0" + CURR.BOOTS.CODE$)

        IF READ.STOCK = 0 THEN BEGIN
            CALL WRITE.STKMQ.6.RECORD
        ENDIF
    ENDIF

    IF CURR.STATUS.1$ = "" THEN BEGIN                                       !EMW
        IDF.STATUS.1$ = " "                                                 !EMW
    ENDIF ELSE BEGIN                                                        !EMW
        IDF.STATUS.1$ = CURR.STATUS.1$                                      !EMW
    ENDIF                                                                   !EMW

!   CALL BARCODE.PROCESSING                                      ! 1.8 RC (95)

END SUB



\******************************************************************************
\***
\***    UPDATE.IEX
\***
\******************************************************************************
\***
\***    Updates IEX - Item Extension File for the specified item
\***
\***    IEX records are only written for Supply Route 'D' items in order to
\***    minimise the time taken to update items.
\***
\******************************************************************************

SUB UPDATE.IEX

    IF CURR.SUPPLY.ROUTE$ = "D" THEN BEGIN

!       Read IEX - If item not found set IEX variables to null          ! 1.10 RC (142)
        IEX.ITEM.CODE$ = PACK$(LEFT$(CURR.BOOTS.CODE$, 6)) !            ! 1.10 RC (142)

        RC% = READ.IEX                                                  ! 1.10 RC (142)

        IF RC% <> 0 THEN BEGIN                                          ! 1.10 RC (142)
            IEX.ACTUAL.SUPPLIER.NUM$ = STRING$( 4, PACK$("00"))         ! 1.10 RC (142)
            IEX.PRIMARY.SUPPLIER$    = STRING$( 4, PACK$("00"))         ! 1.10 RC (142)
            IEX.FILLER$              = STRING$(17, PACK$("00"))         ! 1.10 RC (142)
        ENDIF                                                           ! 1.10 RC (142)

        IF NEW.ITEM THEN BEGIN
!           IEX.ACTUAL.SUPPLIER$ = PACK$("00000000")                    ! 1.2 TT
            IEX.ACTUAL.SUPPLIER.NUM$ = PACK$("00000000")                ! 1.2 TT
            IEX.FILLER$              = STRING$(17, PACK$("00"))         ! 1.10 RC (142)
        ENDIF ELSE BEGIN
!           IEX.ACTUAL.SUPPLIER$ = IEX.PRIMARY.SUPPLIER$                ! 1.2 TT
            IEX.ACTUAL.SUPPLIER.NUM$ = IEX.PRIMARY.SUPPLIER$            ! 1.2 TT
        ENDIF

!       IEX.ITEM.CODE$ = PACK$(LEFT$(CURR.BOOTS.CODE$, 6)) !            ! 1.10 RC (142)
!       IEX.PRIMARY.SUPPLIER$ = PACK$("0000" + CURR.PRIMARY.SUPPLIER$)  ! 1.2 TT
        IEX.PRIMARY.SUPPLIER$ = PACK$( RIGHT$("00000000" + \            ! 1.2 TT
                                       CURR.PRIMARY.SUPPLIER$, 8))      ! 1.2 TT
        RC% = WRITE.IEX
        IF RC% <> 0 THEN BEGIN
            CALL LOG.EVENT(106)
        ENDIF
    ENDIF

END SUB


\******************************************************************************
\***
\***    UPDATE.DRUG
\***
\******************************************************************************
\***
\***    Updates DRUG -  DRUG File for the specified item
\***
\******************************************************************************
SUB UPDATE.DRUG

    IF CURR.ETHICAL.CLASS$ <> "" AND \
       CURR.ETHICAL.CLASS$ <> " " THEN BEGIN

        DRUG.ITEM.CODE$   = PACK$(LEFT$(CURR.BOOTS.CODE$, 6))
        DRUG.DESCRIPTION$ = CURR.ETHICAL.DESCRIPTION$

        IF CURR.ETHICAL.ACTIVE$ = "Y" THEN BEGIN
             DRUG.BIT.FLAGS.1% = DRUG.BIT.FLAGS.1%  OR 00000001B !set   bit 1
        ENDIF ELSE BEGIN
             DRUG.BIT.FLAGS.1% = DRUG.BIT.FLAGS.1% AND 11111110B !clear bit 1
        ENDIF

        DRUG.PACK.SIZE% = VAL(CURR.ETHICAL.PACK.SIZE$)
        DRUG.FILLER$    = PACK$("0000000000000000")             ! 1.9 RC (112)

        RC% = WRITE.DRUG
        IF RC% <> 0 THEN BEGIN
            CALL LOG.EVENT(106)
        ENDIF
    ENDIF

END SUB


\******************************************************************************
\***
\***    UPDATE.NEWLINES
\***
\******************************************************************************
\***
\***    Updates NEWLINES -  NEWLINES File for the specified item
\***
\******************************************************************************
SUB UPDATE.NEWLINES

    NEWLINES.BOOTS.CODE$ = PACK$("0" + CURR.BOOTS.CODE$)
    NEWLINES.DATE.ADDED$ = PACK$("20"+ PROCESSING.DATE$)
    NEWLINES.COUNT%      = 0
    NEWLINES.FILLER$     = "      "

    RC% = WRITE.NEWLINES
    IF RC% <> 0 THEN BEGIN
        CALL LOG.EVENT(106)
    ENDIF

END SUB

\******************************************************************************
\***
\***    UPDATE.RICF
\***
\******************************************************************************
\***
\***    Updates RICF -  Redeem Item Change File for the specified item
\***
\***    If non-price modifications to an item cause changes to SEL details,
\***    and there are no prices changes due (today or sooner) for the item,
\***    and the item has previously been sold, then add records to the RICF
\***    in order to force a label to be printed.
\***
\******************************************************************************
SUB UPDATE.RICF

!   Lines deleted - Made use of PRICE.CHANGE.TODAY flag        ! 1.13 RC (179)
!   instead of reprocessing PPFK data                          ! 1.13 RC (179)

!   Exit this subroutine when ...                              ! 1.13 RC (179)
!     Item never sold (as it is unlikely to be stocked)        ! 1.13 RC (179)
!     Price change today (as this will produce a SEL)          ! 1.13 RC (179)
!     No non-price change to SEL (so no SEL needed via RICF)   ! 1.13 RC (179)
    IF IDF.DATE.OF.LAST.SALE$ = PACK$("000000") \              ! 1.13 RC (179)
      OR PRICE.CHANGE.TODAY \                                  ! 1.13 RC (179)
      OR SEL.NON.PRICE.CHANGE = FALSE THEN BEGIN               ! 1.13 RC (179)
        EXIT SUB                                               ! 1.13 RC (179)
    ENDIF                                                      ! 1.13 RC (179)

!   Prepare RICF record and write to file                      ! 1.13 RC (179)

    RICF.ITEM.CODE$ = IDF.BOOTS.CODE$

    IF CURR.SUPPLY.ROUTE$ = "W" OR   \
       CURR.SUPPLY.ROUTE$ = "D" OR   \
       CURR.SUPPLY.ROUTE$ = "B" THEN BEGIN
        RICF.DELIVERY.FLAG$ = "E"
    ENDIF ELSE BEGIN
        RICF.DELIVERY.FLAG$ = " "
    ENDIF

    IF CURR.REDEEMABLE$ = "Y" THEN BEGIN
        RICF.REDEEM.ITEM$ = "Y"
    ENDIF ELSE BEGIN
       RICF.REDEEM.ITEM$ = "N"
    ENDIF

    RICF.ITEM.DESCRIPTION$ = CURR.S.E.DESC$
    RICF.PRICE$            = RIGHT$(IRF.SALEPRIC$,4)
    RICF.PRODUCT.GROUP$    = PACK$(LEFT$(CURR.PROD.GRP$,2) + \
                             "0" + RIGHT$(CURR.PROD.GRP$,3))
    RICF.ITEM.QTY$         = PACK$(RIGHT$("00000000" + CURR.ITEM.QTY$, 8))     ! 1.13 RC (179)
    RICF.UNIT.MEASUREMENT$ = PACK$(RIGHT$("0000" + CURR.UNIT.MEASUREMENT$, 4)) ! 1.13 RC (179)
    RICF.UNIT.NAME$        = CURR.UNIT.NAME$
    RICF.FILLER$           = PACK$("000000000000")

!   Increment record number prior to write                     ! 1.13 RC (179)
!   File was opened with CREATE so always empty at first       ! 1.13 RC (179)
    RICF.RECORD.NO% = RICF.RECORD.NO% + 1                      ! 1.13 RC (179)

    RC% = WRITE.RICF

    IF RC% <> 0 THEN BEGIN                                     ! 1.13 RC (179)
        CALL LOG.EVENT(106)
    ENDIF

END SUB


\******************************************************************************
\***
\***    PROCESS.IUF.ITEM
\***
\******************************************************************************
\***
\***    Updates all item reference files for the specified item
\***
\******************************************************************************
SUB PROCESS.IUF.ITEM PUBLIC

    MODULE.NUMBER$ = "1"                                                        !1.5CSk(a)

    CALL UPDATE.IDF ! Prepares IDF data but does not update IDF  ! 1.9 RC (109)

    CALL UPDATE.IRF ! Prepares IRF data                          ! 1.9 RC (109)  1.14 RC
                    ! Calls UPDATE.ISF                           !               1.14 RC
                    ! Calls WRITE.IRF for initial barcode        ! 1.9 RC (109)

    CALL BARCODE.PROCESSING ! Called after IDF and IRF prepared  ! 1.8 RC (95)
                            ! Calls WRITE.IDF                    ! 1.9 RC (109)
                            ! Calls WRITE.IRF for other barcodes ! 1.9 RC (109)
    CALL UPDATE.IEX

    CALL UPDATE.DRUG

!   Calls to UPDATE.LOCAL and UPDATE.CIPPM moved into UPDATE.IRF ! 1.9 RC (109)

    IF IUF.INITIAL.LOAD$ = "N" AND NEW.ITEM THEN BEGIN

        CALL UPDATE.NEWLINES

    ENDIF

!   Merge new IUF price changes on CURR tables                  ! 1.13 RC
!   into existing PPFI price changes on PPFK table              ! 1.13 RC
    CALL PROCESS.PPFK.ITEM

    IF NOT NEW.ITEM THEN BEGIN

        CALL UPDATE.RICF

    ENDIF

!   Reset flags as no longer needed for this item              ! 1.13 RC (179)
    PRICE.CHANGE.TODAY   = FALSE                               ! 1.13 RC (179)
    SEL.NON.PRICE.CHANGE = FALSE                               ! 1.13 RC (179)

END SUB


