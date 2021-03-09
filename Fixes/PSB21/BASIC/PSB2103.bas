
\*******************************************************************************
\*******************************************************************************
\***
\***
\***            PROGRAM         :       PSB21
\***            MODULE          :       PSB2103
\***            AUTHOR          :       Mark Walker / Mark Goode
\***            DATE WRITTEN    :       Sept 2011
\***
\*******************************************************************************
\***
\***    VERSION 1.0           Mark Walker / Mark Goode        30 Jun 2011
\***    Initial version.
\***
\***    VERSION 1.1.                ROBERT COWEY.                06 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.5.
\***
\***    Defect 2678
\***    Ensured a new-format-IUF vs IRF price mis-match creates an emergency RPD
\***    on the PPFI and this is not prevented by any existing PPFI price change.
\***
\***    VERSION 1.2.                ROBERT COWEY.                22 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.6.
\***
\***    Defect 50 - Commented 1.2 RC (50).
\***    Shortened message text to fit 46 character background display.
\***
\***    VERSION 1.3.                ROBERT COWEY.                28 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.7.
\***
\***    Defect 53 - Commented 1.3 RC (53).                       28 FEB 2012.
\***    Major changes to simplify and correct sub-routine PROCESS.PPFK.ITEM
\***    which merges IUF price changes into PPFI data (held on the PPFK).
\***
\***    VERSION 1.4.                ROBERT COWEY.                08 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.8.
\***    Removed setting of PPFK.INC.DEC.FLAG$ as this is done by PSB23.
\***    Modified PROCESS.PPFK.ITEM to retain expired RPD price changes
\***    for both legacy and Core R2 format IUF's (with all future and
\***    any todays price changes coming from the IUF).
\***
\***    VERSION 1.5.                ROBERT COWEY.                14 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.9.
\***
\***    Defect 105 - Commented 1.5 RC (105)
\***    No corrective code changes however various clarifications made ...
\***    Wrote comment within CHECK.ITEM.FOR.MARKDOWN.PROCESSING function.
\***    Corrected comments within ITEM.ON.MARKDOWN function.
\***    Commented out superceded MARKDOWN.ITEM.CHECK function and removed
\***    associated redundant variables.
\***
\***    VERSION 1.6.                ROBERT COWEY.                19 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.9.
\***
\***    Defect 109 - Commented 1.6 RC (109)
\***    Modified CHECK.ITEM.FOR.EMERGENCY.RPD to ensure emergency 99999 RPD
\***    is not created when item on Local Price or CIP-Markdown (since in
\***    these cases UPDATE.LOCAL or UPDATE.CIPPM will already have updated
\***    the head office price).
\***
\***    VERSION 1.7.                ROBERT COWEY.                21 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.10.
\***
\***    Defect 113 - Commented 1.7 RC (113)
\***
\***    Modified STORE.PPFI.PRICE.CHANGE(N%) to set PPFK.SORT.NEEDED flag
\***    to TRUE when underlying data (from PPFI) is not in sequence.
\***    Modified PROCESS.OLD.PPFI to call new procedure SORT.PPFK.TABLES
\***    to sort PPFK data tables (and reset PPFK.SORT.NEEDED to FALSE).
\***
\***    Modified PROCESS.PPFK.ITEM to place emergency 99999 RPD onto
\***    CURR data tables when required (see program comment for detail)
\***    Called SORT.CURR.TABLES to sort CURR data tables if needed
\***    (and reset CURR.SORT.NEEDED to FALSE).
\***    Reset PRICE.CHANGE.TODAY flag used to prevent unwanted emergency
\***    99999 RPD's.
\***
\***    Modified user exit function PROCESS.BTREE.RECORD$ to detect CIP
\***    Markdown items and apply any price change for today (or next most
\***    recent price change found) and prevent applied or superceded data
\***    being passed to the PPFO.
\***
\***    Deleted superceded procedures now redundant ...
\***    SUB      PREPARE.EMERGENCY.RPD.FOR.PPFK
\***    FUNCTION CHECK.ITEM.FOR.EMERGENCY.RPD(DUE$,RDATE$)
\***    FUNCTION CHECK.ITEM.FOR.MARKDOWN.PROCESSING
\***    FUNCTION ITEM.ON.MARKDOWN
\***
\***    VERSION 1.8.                ROBERT COWEY.                05 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.11.
\***
\***    Defect 172 - Commented 1.8 RC (172)
\***    Corrected call to UPDATE.CIPPM to pass item code as well as price.
\***
\***    VERSION 1.9.               ROBERT COWEY.                17 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.12.
\***
\***    Renamed IRF.PRICE.MISMATCH to PRICE.MISMATCH to more properly
\***    reflect its wider usage.
\***    Modified code to use this flag for all discrepancies between
\***    definitive price on IUF item record and Head Office price
\***    whether present on IRF, LOCAL, or on CIPPMR (as Reversals Price).
\***
\***    Defect 177 - Commented 1.9 RC (177)
\***    Correction to place RPD data on the output PPFI in ascending date
\***    order (latest last) as needed by PSB72 Effect Price Change program.
\***    Associated changes to programs internal sort of PPFI and IUF data.
\***
\***    VERSION 1.10.               ROBERT COWEY.                24 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.13.
\***
\***    Defect 190 - Commented 1.10 RC (190)
\***    Delete any temporary OS files from RAM (eg, previous failed copies).
\***
\***    VERSION 1.11.               ROBERT COWEY.                30 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.14.
\***
\***    Defect 211 - Commented 1.11 RC (211)
\***    Modified SET.PPFK.FROM.CURR to check PPFK.STATUS.FLAG$ is non-blank
\***    to identify SEL-actioned emergency 99999 RPD and prevent duplication
\***    (instead of checking for "S" - and missing "L" Local price so no SEL).
\***
\***    VERSION 1.12                CHARLES SKADORWA.            04 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.15.
\***    Set PHASE$ correctly.
\***
\***    VERSION 1.13                CHARLES SKADORWA.            20 JUN 2012.
\***    Defect 243 - Commented 1.13 CS (243)
\***                 CIP Markdown items now written to PPFO instead of being
\***                 rejected in order that SEL's can be produced by PSB23.
\***    Changes creating PSB21.286 Core Release 2 version 1.22.
\***
\***    VERSION A                   Mark Walker                23rd May 2014
\***    F337 Centralised View of Stock
\***    Includes the following enhancements:
\***    Modified READKF interface to include the new 'mode' parameter.
\***
\***    VERSION B                   Ranjith Gopalankutty       10th May 2016
\***    INV10004090 - Defect Fix
\***    Module has a defect - While processing the CIPPM records from PPFK
\***    it adds extra entry in PPFO and the line gets duplicated. Over the 
\***    the period of time duplicate limit exceeds allowed limit of 120 
\***    and JOBOK suite fails. 
\***    This issue fails CIPOK mark down removal, due to duplicated entries
\***    CIP removal some time doesn't works. 
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
\***    The purpose of this module is to perform price change processing.
\***
\*******************************************************************************
\*******************************************************************************
    INTEGER*1 GLOBAL                        \
        CIPPMR.PRESENT,                     \                     ! 1.16 RC (226)
        TRUE,                               \
        FALSE,                              \
\       STOP.CHECK,                         \                     ! 1.6 RC (106)
        CURR.SORT.NEEDED,                   \                     ! 1.7 RC (113)
        PRICE.MISMATCH,                     \                     ! 1.9 RC
        PPFK.SORT.NEEDED,                   \                     ! 1.7 RC (113)
        PRICE.CHANGE.TODAY,                 \                     ! 1.7 RC (113)
        EMERGENCY.RPD                       !

    INTEGER*2 GLOBAL                        \
        CURRENT.REPORT.NUM%,                \
        CURR.PCR.COUNT%,                    \
        CURR%,                              \ 1.3 RC (53)
        PREV%,                              \ 1.3 RC (53)
        PPFK%,                              \ 1.3 RC (53)
        PREV.PCR.COUNT%                     !

    STRING GLOBAL                           \
\       CIP.ITEM.UPDATED$,                  \                     ! 1.9 RC (177)
        CURR.BOOTS.CODE$,                   \
        FILE.OPERATION$                     !

    STRING GLOBAL                           \
        CURR.RPD.NO$(1),                    \
        CURR.DATE.DUE$(1),                  \
        CURR.PRICE$(1),                     \
        CURR.INC.DEC.FLAG$(1),              \
        CURR.STATUS.FLAG$(1),               \
        CURR.MARKDOWN$(1)                   !

    STRING GLOBAL                           \
        PREV.RPD.NO$(1),                    \
        PREV.DATE.DUE$(1),                  \
        PREV.PRICE$(1),                     \
        PREV.INC.DEC.FLAG$(1),              \
        PREV.STATUS.FLAG$(1),               \
        PREV.MARKDOWN$(1)                   !

    STRING GLOBAL                           \
        CURR.CURRENT.PRICE$,                \
        PHASE$,                             \                   ! 1.12 CSk
        PROCESSING.DATE$                    !

    STRING GLOBAL \                                             ! 1.6 RC (113)
        SORT.TABLE$(1)                                          ! 1.6 RC (113)

!   Remo ved redundant variables                                 ! 1.5 RC (105)

    INTEGER*4                               \
        PPFO.REC.COUNT%                     !

%INCLUDE IRFDEC.J86                                              ! 1.6 RC (109)
%INCLUDE PPFDEC.J86
%INCLUDE PPFKDEC.J86

%INCLUDE IDFDEC.J86
%INCLUDE IUFDEC.J86                                              ! 1.3 RC (53)

%INCLUDE IDFEXT.J86

%INCLUDE PPFEXT.J86
%INCLUDE PPFKEXT.J86


%INCLUDE KFASMEXT.J86 ! Keyed file assembler external functions
%INCLUDE BTREEEXT.J86 ! Binary tree external functions

%INCLUDE BASROUT.J86  ! OSSHELL function (and others)           ! 1.10 RC (190)
%INCLUDE CSORTDEC.J86 ! Assembler sort routine                  ! 1.7 RC (113)


SUB LOG.EVENT(EVENT.NO%) EXTERNAL
END SUB

SUB DO.MESSAGE(MSG$, LOG%) EXTERNAL
END SUB

SUB SORT.CURR.TABLES EXTERNAL ! PSB2100                         ! 1.7 RC (113)
END SUB                                                         ! 1.7 RC (113)

SUB STORE.EMERG.PRICE.CHANGE(N%) EXTERNAL ! PSB2100             ! 1.7 RC (113)
END SUB                                                         ! 1.7 RC (113)

FUNCTION UPDATE.CIPPM(CIPPM.ITEM$, REV.PRICE$) EXTERNAL         ! 1.8 RC (172)
     INTEGER*1 UPDATE.CIPPM
END FUNCTION

! IBM copy files external subroutine definition
SUB ADXCOPYF(RETC,INFILE,OUTFILE,OPT0,OPT1,OPT2) EXTERNAL
INTEGER*4 RETC
STRING    INFILE, OUTFILE
INTEGER*2 OPT0, OPT1, OPT2
END SUB


!   Function CHECK.ITEM.FOR.MARKDOWN.PROCESSING deleted         ! 1.7 RC (113)

!   Function ITEM.ON.MARKDOWN deleted                           ! 1.7 RC (113)

!   Function CHECK.ITEM.FOR.EMERGENCY.RPD(DUE$,RDATE$) deleted  ! 1.7 RC (113)


\******************************************************************************
\***
\***    PROCESS.BTREE.RECORDS
\***
\******************************************************************************
\***
\***    User exit function to perform processing on binary tree records
\***    with key value V and data D
\***
\******************************************************************************
FUNCTION PROCESS.BTREE.RECORD$(V%,D$) PUBLIC

    STRING PROCESS.BTREE.RECORD$,D$
    INTEGER*1 RC%
    INTEGER*2 I%,PPF.PCR.COUNT%,PPF.PCR.INDEX%
    STRING    PPF.DATE.DUE.NEXT$                                ! 1.9 RC (177)
    INTEGER*2 PPF.PCR.INDEX.NEXT%                               ! 1.9 RC (177)
    INTEGER*4 V%

        ! Extract item code from key value
        PPF.BOOTS.CODE$ = LEFT$(RIGHT$("000000" + STR$(V%),9),7)

!       Lines deleted                                           ! 1.9 RC (177)

!       Read IDF to obtain Markdown flag for this item          ! 1.7 RC (113)
!       If item not found (albeit very unlikely) then set       ! 1.7 RC (113)
!       IDF.BIT.FLAGS.1% to zero to set Markdown flag OFF       ! 1.7 RC (113)
        IDF.BOOTS.CODE$ = PACK$("0" + PPF.BOOTS.CODE$)          ! 1.7 RC (113)
        IF READ.IDF <> 0 THEN BEGIN                             ! 1.7 RC (113)
            IDF.BIT.FLAGS.1% = 0                                ! 1.7 RC (113)
        ENDIF                                                   ! 1.7 RC (113)

        ! Calculate number of price changes (on this PPFK record)     ! 1.9 RC (177)
        PPF.PCR.COUNT% = LEN(D$) / PPFK.PCR.RECL%

        ! FOR each price change record
        FOR I% = 1 TO PPF.PCR.COUNT%

            PPF.PCR.INDEX% = ((I% - 1) * PPFK.PCR.RECL%) + 1

!---------------------------------------------------------------------------- ! 1.9 RC (177)
!   Perform CIP Markdown processing                                           ! 1.9 RC (177)
!---------------------------------------------------------------------------- ! 1.9 RC (177)
!   This section new for Rv 1.9 RC (177)                                      ! 1.9 RC (177)

!           PPFK price changes are in ascending date order

            PPF.DATE.DUE$  = UNPACK$(MID$(D$, PPF.PCR.INDEX%, 3))

!           If item on CIP Markdown
!           and current PPFK price change is expired or is for today
!               Apply any price change for today (or apply next most
!               recent price change found on PPFK record)
!               Prevent applied price change (and any earlier price
!               changes found on PPFK record) being passed to PPFO

            IF (IDF.BIT.FLAGS.1% AND 20h) = 20h \ ! Markdown ON
              AND PPF.DATE.DUE$ <= PROCESSING.DATE$ THEN BEGIN

!               Check whether next price change on PPFK record is for
!               today or earlier and if so bypass current price change
!               Continue checking each price change on PPFK record until
!               most recent price change found (or end of record reached)
!               (Price change found may include an emergency 99999 RPD
!               created within procedure PROCESS.PPFK.ITEM)

                IF I% < PPF.PCR.COUNT% THEN BEGIN
                    PPF.PCR.INDEX.NEXT% = ((I%) * PPFK.PCR.RECL%) + 1
                    PPF.DATE.DUE.NEXT$  = UNPACK$(MID$(D$, PPF.PCR.INDEX.NEXT%, 3))
                ENDIF

                WHILE I% < PPF.PCR.COUNT% \
                  AND PPF.DATE.DUE.NEXT$ <= PROCESSING.DATE$ \

!                   Current record is record number: I%                                                      ! 1.13 CS (243)
!                   This is about to be bypassed because the next record is expired or dated for today       ! 1.13 CS (243)
!                   It its Markdown Flag is "Y", then write it to the PPFO                                   ! 1.13 CS (243)
!                                                                                                            ! 1.13 CS (243)
!                   Set Index and Markdown Flag from I%                                                      ! 1.13 CS (243)
                    PPF.PCR.INDEX% = ((I% - 1) * PPFK.PCR.RECL%) + 1                                         ! 1.13 CS (243)
                    PPF.MARKDOWN$      = MID$(D$,PPF.PCR.INDEX% + 12,1)                                      ! 1.13 CS (243)
                    IF PPF.MARKDOWN$ = "Y" THEN BEGIN                                                        ! 1.13 CS (243)
                        GOSUB WRITE.TO.PPFO   ! Past day                                                     ! 1.13 CS (243)
                    ENDIF                                                                                    ! 1.13 CS (243)

!                   Bypass current PPFK price change
                    I% = I% +1

                    IF I% < PPF.PCR.COUNT% THEN BEGIN
                        PPF.PCR.INDEX.NEXT% = ((I%) * PPFK.PCR.RECL%) + 1
                        PPF.DATE.DUE.NEXT$  = UNPACK$(MID$(D$, PPF.PCR.INDEX.NEXT%, 3))
                    ENDIF

                WEND

!               At this point ...
!                  There are no further price changes for this item
!                   so apply current price change as it is the most recent
!               OR Next price change is the first future one encountered
!                   so apply current price change as it is the most recent
!               OR Next price change is on next PPFK record so may be future
!                   so apply current price change in case it is the most recent
                PPF.PCR.INDEX% = ((I% - 1) * PPFK.PCR.RECL%) + 1
                PPF.PRICE$ = UNPACK$(MID$(D$, PPF.PCR.INDEX% + 8, 4))

!               Call UPDATE.CIPPM to update CIPPMR Reversals Price
!               On success increment I% to prevent applied price change
!               being written to PPFO
!               On fail the PPFK price change gets written to the PPFO
!               to be re-attempted next run


                ! Check whether the current price change has the Markdown Flag set.         ! 1.13 CS (243)
                ! If so, write to the PPFO instead of updating the CIPPM reversals price.   ! 1.13 CS (243)
                PPF.MARKDOWN$      = MID$(D$,PPF.PCR.INDEX% + 12,1)                         ! 1.13 CS (243)
                IF PPF.MARKDOWN$ = "Y" THEN BEGIN                                           ! 1.13 CS (243)
                    GOSUB WRITE.TO.PPFO    ! Current day                                    ! 1.13 CS (243)
                ENDIF ELSE BEGIN                                                            ! 1.13 CS (243)

                    IF UPDATE.CIPPM(PPF.BOOTS.CODE$, PPF.PRICE$) = 0 THEN BEGIN
                        I% = I% + 1
                    ENDIF

                ENDIF                                                                       ! 1.13 CS (243)

!               Exit function if no further price changes on this PPFK record

!               Below condition has to be greater than or equal to      ! BRG
!               rather  than just greater than. Because greater than    ! BRG
!               condition never gets satisfied for PPF.MARKDOWN$ with   ! BRG
!               'Y' . As there is no increment happens for the last     ! BRG
!               PPFK record for a price change, instead it comes down   ! BRG
!               and print one more entry which are designated only for  ! BRG
!               Items with no mark down flag set. below fix will remove ! BRG
!               Duplicate entry issues in PPFO/PPFI                     ! BRG

!               IF I%>   PPF.PCR.COUNT% THEN BEGIN                      ! BRG

                IF I% >= PPF.PCR.COUNT% THEN BEGIN                      ! BRG
                    EXIT FUNCTION
                ENDIF

            ENDIF

!           Set PPF.PCR.INDEX% again to cater for increases in index value I%
!           when bypassing redundant PPFK price changes
            PPF.PCR.INDEX% = ((I% - 1) * PPFK.PCR.RECL%) + 1

!---------------------------------------------------------------------------- ! 1.9 RC (177)
!   End of CIP Markdown processing                                            ! 1.9 RC (177)
!---------------------------------------------------------------------------- ! 1.9 RC (177)

    GOSUB WRITE.TO.PPFO                                                       ! 1.13 CS (243)

        NEXT I%

    EXIT FUNCTION                                                             ! 1.13 CS (243)


WRITE.TO.PPFO: ! NEW SUBROUTINE CONTAINING WRITE TO PPFO CODE

            PPF.DATE.DUE$      = UNPACK$(MID$(D$,PPF.PCR.INDEX%,3))
            PPF.RPD.NO$        = RIGHT$(UNPACK$(MID$(D$,PPF.PCR.INDEX% + 3,3)),5)
            PPF.STATUS.FLAG$   = MID$(D$,PPF.PCR.INDEX% + 6,1)
            PPF.INC.DEC.FLAG$  = MID$(D$,PPF.PCR.INDEX% + 7,1)
            PPF.PRICE$         = UNPACK$(MID$(D$,PPF.PCR.INDEX% + 8,4))
            PPF.MARKDOWN$      = MID$(D$,PPF.PCR.INDEX% + 12,1)

            ! Write price change record to PPFO
            RC% = WRITE.PPFO

            ! IF PPFO write was successful
            IF RC% = 0 THEN BEGIN

                ! Increment number of PPFO records
                PPFO.REC.COUNT% = PPFO.REC.COUNT% + 1

            ENDIF ELSE BEGIN
                ! Handle error - Unable to write to PPFO
                CALL DO.MESSAGE("PSB21 *** ERROR writing to PPFO", FALSE)
                CALL LOG.EVENT(106)
            ENDIF

RETURN ! End of SUB - Nb. LOG.EVENT(106) will ABEND program for a failed write, so safe to do this  ! 1.13 CS (243)



END FUNCTION


FUNCTION HEX4$(H%) PUBLIC

    STRING HEX4$,E$
    INTEGER*2 I%
    INTEGER*4 H%

    E$ = ""
    FOR I% = 28 TO 0 STEP -4
        E$ = E$ + CHR$((SHIFT(H%,I%) AND 000FH) + 48)
    NEXT I%
    HEX4$ = TRANSLATE$(E$,":;<=>?","ABCDEF")

END FUNCTION

\******************************************************************************
\***
\***    STORE.PPFI.PRICE.CHANGE
\***
\******************************************************************************
\***
\***    Add existing price change to the price change arrays
\***
\******************************************************************************
SUB STORE.PPFI.PRICE.CHANGE(N%) !PRIVATE

    INTEGER*2 N%

    PPFK.RPD.NO$(N%)       = PACK$("0" + PPF.RPD.NO$)
    PPFK.DATE.DUE$(N%)     = PACK$(PPF.DATE.DUE$)
    PPFK.PRICE$(N%)        = PACK$(PPF.PRICE$)
    PPFK.INC.DEC.FLAG$(N%) = PPF.INC.DEC.FLAG$
    PPFK.STATUS.FLAG$(N%)  = PPF.STATUS.FLAG$
    PPFK.MARKDOWN$(N%)     = PPF.MARKDOWN$

!   No need to sort PPFK tables for single price change         ! 1.7 RC (113)
    IF N% <= 1 THEN BEGIN                                       ! 1.7 RC (113)
        EXIT SUB                                                ! 1.7 RC (113)
    ENDIF                                                       ! 1.7 RC (113)

!   An items PPFI records need to be in ascending date-RPD order ! 1.9 RC (177)
!   (latest last) and once in this sequence should remain in it. ! 1.9 RC (177)

!   When an items current PPFI record date-RPD is earlier than  ! 1.7 RC (113)
!   its previous PPFI record date-RPD then the items PPFI       ! 1.7 RC (113)
!   record data needs to be sorted.                             ! 1.7 RC (113)
    IF  PPFK.DATE.DUE$(N%)    + PPFK.RPD.NO$(N%) \              ! 1.7 RC (113)
      < PPFK.DATE.DUE$(N% -1) + PPFK.RPD.NO$(N% -1) THEN BEGIN  ! 1.9 RC (177)
        PPFK.SORT.NEEDED = TRUE                                 ! 1.7 RC (113)
    ENDIF                                                       ! 1.7 RC (113)

END SUB


\******************************************************************************
\***
\***    OPEN.PPFI
\***
\******************************************************************************
\***
\***    Opens the PPFI file
\***
\******************************************************************************
FUNCTION OPEN.PPFI !PRIVATE

    INTEGER*1 OPEN.PPFI

    OPEN.PPFI = 1

    CURRENT.REPORT.NUM% = PPFI.REPORT.NUM%
    FILE.OPERATION$ = "O"
    IF END #PPFI.SESS.NUM% THEN OPEN.PPFI.ERROR
    OPEN PPFI.FILE.NAME$ AS PPFI.SESS.NUM% BUFFSIZE 32256 LOCKED NOWRITE NODEL

    OPEN.PPFI = 0

    OPEN.PPFI.ERROR:

END FUNCTION

\******************************************************************************
\***
\***    CREATE.PPFO
\***
\******************************************************************************
\***
\***    Creates the PPFO file
\***
\******************************************************************************
FUNCTION CREATE.PPFO !PRIVATE

    INTEGER*1 CREATE.PPFO

    CREATE.PPFO = 1

    CURRENT.REPORT.NUM% = PPFO.REPORT.NUM%
    FILE.OPERATION$ = "C"
    IF END #PPFO.SESS.NUM% THEN CREATE.PPFO.ERROR
    CREATE POSFILE PPFO.FILE.NAME$ AS PPFO.SESS.NUM% LOCKED MIRRORED ATCLOSE

    CREATE.PPFO = 0

    CREATE.PPFO.ERROR:

END FUNCTION

\******************************************************************************
\***
\***    CREATE.PPFK
\***
\******************************************************************************
\***
\***    Creates the PPFK file
\***
\******************************************************************************
FUNCTION CREATE.PPFK PUBLIC

    INTEGER*1 CREATE.PPFK
    INTEGER*2 RETRY.COUNT%

    ON ERROR GOTO CREATE.PPFK.ERROR

!   Delete any temporary OS files from RAM (eg, previous failed copies)  ! 1.10 RC (190)
    CALL OSSHELL("IF EXIST W:\%*.* DEL W:\%*.*")                         ! 1.10 RC (190)

    RETRY.COUNT% = 0

  OVERRIDE.FILE.NAME:

!   Delete PPFK if it already exists from a previous run       ! 1.10 RC (190)
    IF END # PPFK.SESS.NUM% THEN PPFK.NOT.PRESENT              ! 1.10 RC (190)
    OPEN PPFK.FILE.NAME$ AS PPFK.SESS.NUM% LOCKED              ! 1.10 RC (190)
    DELETE PPFK.SESS.NUM%                                      ! 1.10 RC (190)

PPFK.NOT.PRESENT:                                              ! 1.10 RC (190)

    CREATE.PPFK = 1

    CURRENT.REPORT.NUM% = PPFK.REPORT.NUM%
    FILE.OPERATION$ = "C"
   ! IF END #PPFK.SESS.NUM% THEN CREATE.PPFK.ERROR
    CREATE POSFILE PPFK.FILE.NAME$          \
        KEYED PPFK.KEYL%,,,PPFK.MAXR%       \
        RECL PPFK.RECL%                     \
        AS PPFK.SESS.NUM%

    CREATE.PPFK = 0

    EXIT FUNCTION

  CREATE.PPFK.ERROR:

    RETRY.COUNT% = RETRY.COUNT% + 1

    IF RETRY.COUNT% > 1 THEN EXIT FUNCTION

    ! For ANY error, try creating temp file on physical disk
    PPFK.FILE.NAME$  = "C:\PPFK.BIN"
    RESUME OVERRIDE.FILE.NAME

END FUNCTION

\******************************************************************************
\***
\***    DESTROY.PPFK
\***
\******************************************************************************
\***
\***    Deletes the PPFK file
\***
\******************************************************************************
FUNCTION DESTROY.PPFK PUBLIC

    INTEGER*1 DESTROY.PPFK

    DESTROY.PPFK = 1

    CURRENT.REPORT.NUM% = PPFK.REPORT.NUM%
    FILE.OPERATION$ = "D"
    IF END #PPFK.SESS.NUM% THEN DESTROY.PPFK.ERROR
    DELETE PPFK.SESS.NUM%

    DESTROY.PPFK = 0

    DESTROY.PPFK.ERROR:

END FUNCTION


! Sub MARKDOWN.ITEM.CHECK deleted                               ! 1.7 RC (113)


\******************************************************************************
\***
\***    SORT.PPFK.TABLES
\***    Sorts PPFK data tables into descending date-RPD order
\***
\******************************************************************************

SUB SORT.PPFK.TABLES PUBLIC ! Entire procedure new for Rv 1.7 (113)

    INTEGER*2 SUB%
    INTEGER*2 SORT.LIMIT.SUB%

!   An items PPFI records need to have ascending date-RPD order ! 1.9 RC (177)
!   Once the PPFI is in sequence it should normally remain in it
!   therefore this sort will only be called occassionally

!   Combine all PPFK data tables into SORT.TABLE$ ready for sorting

    SUB% = 1

    WHILE SUB% <= PPFK.PCR.COUNT%
        SORT.TABLE$(SUB% -1) = \
          PPFK.DATE.DUE$(SUB%)     + \ ! Major sort field - YYMMDD
          PPFK.RPD.NO$(SUB%)       + \ ! Minor sort field - 6 digit RPD number
          PPFK.PRICE$(SUB%)        + \
          PPFK.INC.DEC.FLAG$(SUB%) + \
          PPFK.STATUS.FLAG$(SUB%)  + \
          PPFK.MARKDOWN$(SUB%)
        SUB% = SUB% + 1
    WEND

    SORT.LIMIT.SUB% = PPFK.PCR.COUNT% - 1 ! Minus one because table
                                          ! entry zero used by CSORT

!   Sort SORT.TABLE by (ascending) date-RPD
    CALL CSORT (VARPTR(SORT.TABLE$(0)), SORT.LIMIT.SUB%)

!   Re-populate PPFK tables from SORT.TABLE$ retaining new sort order       ! 1.9 RC (177)

    SUB% = 1

    WHILE SUB% <= PPFK.PCR.COUNT%
        PPFK.DATE.DUE$(SUB%)     = MID$(SORT.TABLE$(SUB% - 1),  1, 3) ! 3 UPD   ! 1.9 RC (177)
        PPFK.RPD.NO$(SUB%)       = MID$(SORT.TABLE$(SUB% - 1),  4, 3) ! 3 UPD   ! 1.9 RC (177)
        PPFK.PRICE$(SUB%)        = MID$(SORT.TABLE$(SUB% - 1),  7, 4) ! 4 UPD   ! 1.9 RC (177)
        PPFK.INC.DEC.FLAG$(SUB%) = MID$(SORT.TABLE$(SUB% - 1), 11, 1) ! 1 ASC   ! 1.9 RC (177)
        PPFK.STATUS.FLAG$(SUB%)  = MID$(SORT.TABLE$(SUB% - 1), 12, 1) ! 1 ASC   ! 1.9 RC (177)
        PPFK.MARKDOWN$(SUB%)     = MID$(SORT.TABLE$(SUB% - 1), 13, 1) ! 1 ASC   ! 1.9 RC (177)
        SUB% = SUB% + 1
    WEND

!   Set flag to indicate sort now done
    PPFK.SORT.NEEDED = FALSE

END SUB


\******************************************************************************
\***
\***    PROCESS.OLD.PPFI
\***
\******************************************************************************
\***
\***    Load all existing PPFI records into new temporary keyed file PPFK
\***
\******************************************************************************
SUB PROCESS.OLD.PPFI PUBLIC

    INTEGER*1   END.OF.PPFI.FILE
    INTEGER*1   END.OF.PPFI.ITEM
    INTEGER*1   RC%

    PHASE$ = "2.0"                                                  ! 1.12 CSk
    CALL DO.MESSAGE("PSB21 PHASE 2", TRUE)
    CALL DO.MESSAGE("PSB21 2.0 - PROCESS.OLD.PPFI", TRUE)

    ! Open the PPFI
    RC% = OPEN.PPFI

    ! IF PPFI open was unsuccessful
    IF RC% <> 0 THEN BEGIN
        ! Handle error - unable to access PPFI
        CALL DO.MESSAGE("PSB21 *** UNABLE TO ACCESS PPFI", FALSE)
        CALL LOG.EVENT(106)
        EXIT SUB
    ENDIF

    END.OF.PPFI.FILE = FALSE

    ! Read the first PPFI record
    RC% = READ.PPFI

    ! IF PPFI read was successful
    IF RC% = 0 THEN BEGIN

        ! IF we have an IUF trailer record
        IF PPF.REC.TYPE.FLAG$ = "T" THEN BEGIN

            ! End of file has been reached
            END.OF.PPFI.FILE = TRUE

        ! ELSE IF we have NOT got a price change record type (i.e. unknown)
        ENDIF ELSE IF PPF.REC.TYPE.FLAG$ <> "R" THEN BEGIN
            ! Handle error - Unknown record type
            CALL DO.MESSAGE("PSB21 *** ERROR: Unknown record type on PPFI [" + \
                  PPF.REC.TYPE.FLAG$ + "]",FALSE)
            CALL LOG.EVENT(106)
            EXIT SUB
        ENDIF

    ENDIF ELSE BEGIN
        ! Handle error - empty file
        CALL DO.MESSAGE("PSB21 *** ERROR: Unable to Read PPFI",FALSE)
        CALL LOG.EVENT(106)
        EXIT SUB
    ENDIF

    ! WHILE end of the file has NOT been reached
    WHILE NOT END.OF.PPFI.FILE

        CURR.BOOTS.CODE$ = PPF.BOOTS.CODE$

        PPFK.PCR.COUNT% = 0

        END.OF.PPFI.ITEM = FALSE

        ! WHILE end of the current item's price changes have NOT been reached
        WHILE NOT END.OF.PPFI.ITEM

            PPFK.PCR.COUNT% = PPFK.PCR.COUNT% + 1

            CALL STORE.PPFI.PRICE.CHANGE(PPFK.PCR.COUNT%)

            ! Read next PPFI record
            RC% = READ.PPFI

            ! IF PPFI read was successful
            IF RC% = 0 THEN BEGIN

                ! IF we have a trailer record
                IF NOT (PPF.REC.TYPE.FLAG$ = "T" OR \
                        PPF.REC.TYPE.FLAG$ = "R") THEN BEGIN
                    ! Handle error - Unexpected record type
                    CALL DO.MESSAGE("PSB21 *** ERROR: Unknown record type on PPFI [" + \
                          PPF.REC.TYPE.FLAG$ + "]",FALSE)
                    CALL LOG.EVENT(106)
                ENDIF ELSE BEGIN

                    ! IF we have a change of item code
                    IF CURR.BOOTS.CODE$ <> PPF.BOOTS.CODE$ THEN BEGIN

                        ! End of current item has been reached
                        END.OF.PPFI.ITEM = TRUE

                        ! We now have all price change records for the item
                        PPFK.BOOTS.CODE$ = CURR.BOOTS.CODE$

!                       Sort price change data by ascending     ! 1.9 RC (177)
!                       date-RPD if not already in this order   ! 1.7 RC (113)
                        IF PPFK.SORT.NEEDED THEN BEGIN          ! 1.7 RC (113)
                            CALL SORT.PPFK.TABLES               ! 1.7 RC (113)
                        ENDIF

                        RC% = WRITE.PPFK

                        IF RC% <> 0 THEN BEGIN
                            ! Handle error - Unable to write to PPFK
                            CALL DO.MESSAGE("PSB21 *** UNABLE TO WRITE TO PPFK", FALSE)
                            CALL LOG.EVENT(106)
                        ENDIF

                    ENDIF

                    ! IF we have a PPFI trailer record
                    IF PPF.REC.TYPE.FLAG$ = "T" THEN BEGIN
                        ! No more items left to process
                        END.OF.PPFI.FILE = TRUE
                        CALL DO.MESSAGE("PSB21      - End of PPFI file found", FALSE)
                    ENDIF

                ENDIF

            ENDIF ELSE BEGIN
                ! Handle error - Unexpected end of file
                CALL DO.MESSAGE("PSB21 *** ERROR: Unexpected end of PPFI file",FALSE)
                CALL LOG.EVENT(106)
            ENDIF

        WEND

    WEND

    ! Close the PPFI
    CLOSE PPFI.SESS.NUM%

END SUB


\******************************************************************************
\***    SET.PREV.FROM.PPFK                                        ! 1.3 RC (53)
\***
\******************************************************************************

SUB SET.PREV.FROM.PPFK PUBLIC ! Entire function new for 1.3 RC (53)

    PREV.DATE.DUE$(PREV%)     = PPFK.DATE.DUE$(PPFK%)
    PREV.RPD.NO$(PREV%)       = PPFK.RPD.NO$(PPFK%)
    PREV.STATUS.FLAG$(PREV%)  = PPFK.STATUS.FLAG$(PPFK%)
    PREV.INC.DEC.FLAG$(PREV%) = PPFK.INC.DEC.FLAG$(PPFK%)
    PREV.PRICE$(PREV%)        = PPFK.PRICE$(PPFK%)
    PREV.MARKDOWN$(PREV%)     = PPFK.MARKDOWN$(PPFK%)

!   For both legacy and Core R2 system ...                           ! 1.7 RC (113)
!   Future and current day price changes are transmitted on the IUF  ! 1.7 RC (113)
!   and supercede any such price changes present on the PPFI.        ! 1.7 RC (113)
!   Therefore PPFK (PPFI) content does not set PRICE.CHANGE.TODAY    ! 1.7 RC (113)
!   as is done within STORE.PRICE.CHANGE(N%)                         ! 1.7 RC (113)
!   IF PREV.DATE.DUE$(PREV%) = PROCESSING.DATE$ THEN BEGIN           ! 1.7 RC (113)
!       PRICE.CHANGE.TODAY = Unchanged (See preceding comment)       ! 1.7 RC (113)
!   ENDIF                                                            ! 1.7 RC (113)

END SUB


!   SUB PREPARE.EMERGENCY.RPD.FOR.PPFK PUBLIC deleted            ! 1.7 RC (113)


\******************************************************************************
\***    SET.PPFK.FROM.PREV                                        ! 1.3 RC (53)
\***
\******************************************************************************

SUB SET.PPFK.FROM.PREV PUBLIC ! Entire function new for 1.3 RC (53)

!   For both legacy and Core R2 system ...                            ! 1.7 RC (113)
!   All future and current day price changes are re-transmitted on    ! 1.7 RC (113)
!   the IUF                                                           ! 1.7 RC (113)
!   Only expired PPFI price change data (on the PPFK) is retained     ! 1.7 RC (113)
!   (unless matched to an IUF price change as dealt with in separate  ! 1.7 RC (113)
!    procedure SET.PPFK.FROM.CURR.PLUS.PREV)                          ! 1.7 RC (113)
    IF UNPACK$(PREV.DATE.DUE$(PREV%)) >= PROCESSING.DATE$ THEN BEGIN  ! 1.7 RC (113)
        EXIT SUB ! Do not write to PPFK tables                        ! 1.7 RC (113)
    ENDIF                                                             ! 1.7 RC (113)

!   If an emergency 99999 RPD on the PPFI has not been actioned       ! 1.7 RC (113)
!   (ie, SEL not yet printed so Status flag not yet set to "S")       ! 1.7 RC (113)
!   do not pass it to the output PPFI.                                ! 1.7 RC (113)
!   When an emergency 99999 RPD is still required it will have been   ! 1.7 RC (113)
!   created via the (IUF) CURR data table within PROCESS.PPFK.ITEM    ! 1.7 RC (113)
    IF PREV.RPD.NO$(PREV%) = PACK$("099999") \      ! Emergency RPD   ! 1.7 RC (113)
      AND PREV.STATUS.FLAG$(PREV%) = " " THEN BEGIN ! SEL not printed ! 1.7 RC (113)
        EXIT SUB ! Do not write to PPFK tables                        ! 1.7 RC (113)
    ENDIF                                                             ! 1.7 RC (113)

!   Lines deleted - CIP Markdown process now in PROCESS.BTREE.RECORD  ! 1.7 RC (113)
!   (because this needs to be done for the entire PPFI - not just for ! 1.7 RC (113)
!   those items for which an IUF item is being processed)             ! 1.7 RC (113)

!   Lines removed                                                      ! 1.4 RC
!   Lines removed                                                      ! 1.7 rc (113)

    PPFK% = PPFK% + 1

    PPFK.DATE.DUE$(PPFK%)     = PREV.DATE.DUE$(PREV%)
    PPFK.RPD.NO$(PPFK%)       = PREV.RPD.NO$(PREV%)
    PPFK.STATUS.FLAG$(PPFK%)  = PREV.STATUS.FLAG$(PREV%)
    PPFK.INC.DEC.FLAG$(PPFK%) = PREV.INC.DEC.FLAG$(PREV%)
    PPFK.PRICE$(PPFK%)        = PREV.PRICE$(PREV%)
    PPFK.MARKDOWN$(PPFK%)     = PREV.MARKDOWN$(PREV%)

END SUB


\******************************************************************************
\***    SET.PPFK.FROM.CURR                                        ! 1.3 RC (53)
\***
\******************************************************************************

SUB SET.PPFK.FROM.CURR PUBLIC ! Entire function new for 1.3 RC (53)

    INTEGER*2 I%                                                ! 1.7 RC (113)

!   PREVENT DUPLICATION OF (SEL-PRINTED) EMERGENCY 99999 RPD    ! 1.7 RC (113)
!   If CURR price change is an emergency 99999 RPD and there    ! 1.7 RC (113)
!   is at least one price change already on the PPFK table ...  ! 1.9 RC (177)
    IF CURR.RPD.NO$(CURR%) = PACK$("099999") \                  ! 1.7 RC (113)
      AND PPFK% >= 1 THEN BEGIN                                 ! 1.9 RC (177)

!       Check whether PPFK table contains an identical          ! 1.9 RC (177)
!       emergency 99999 RPD which has had its SEL printed and   ! 1.9 RC (177)
!       if so do not pass CURR emergency 99999 RPD to PPFK      ! 1.9 RC (177)
!       (because this would duplicate existing PPFK emergency   ! 1.9 RC (177)
!        99999 RPD already SEL-actioned by store staff)         ! 1.9 RC (177)
        FOR I% = 1 TO PPFK%                                     ! 1.9 RC (177)
            IF   PPFK.RPD.NO$(I%) = PACK$("099999") \           ! 1.9 RC (177)
              AND PPFK.PRICE$(I%) = CURR.PRICE$(CURR%) \        ! 1.9 RC (177)
              AND PPFK.STATUS.FLAG$(I%) <> " " THEN BEGIN       ! 1.11 RC (211)
                EXIT SUB ! Do not write to PPFK tables          ! 1.7 RC (113)
            ENDIF                                               ! 1.7 RC (113)
        NEXT I%                                                 ! 1.7 RC (113)

    ENDIF                                                       ! 1.7 RC (113)

!   Lines deleted - CIP Markdown process now in PROCESS.BTREE.RECORD  ! 1.7 RC (113)
!   (because this needs to be done for the entire PPFI - not just for ! 1.7 RC (113)
!   those items for which an IUF item is being processed)             ! 1.7 RC (113)

!   Lines removed                                                      ! 1.4 RC
!   Lines removed                                                      ! 1.7 RC (113)

    PPFK% = PPFK% + 1

    PPFK.DATE.DUE$(PPFK%)     = CURR.DATE.DUE$(CURR%)
    PPFK.RPD.NO$(PPFK%)       = CURR.RPD.NO$(CURR%)
    PPFK.STATUS.FLAG$(PPFK%)  = CURR.STATUS.FLAG$(CURR%)
    PPFK.INC.DEC.FLAG$(PPFK%) = CURR.INC.DEC.FLAG$(CURR%)
    PPFK.PRICE$(PPFK%)        = CURR.PRICE$(CURR%)
    PPFK.MARKDOWN$(PPFK%)     = CURR.MARKDOWN$(CURR%)

END SUB


\******************************************************************************
\***    SET.PPFK.FROM.CURR.PLUS.PREV                              ! 1.3 RC (53)
\***
\******************************************************************************

SUB SET.PPFK.FROM.CURR.PLUS.PREV PUBLIC ! Entire function new for 1.3 RC (53)

    CALL SET.PPFK.FROM.CURR ! Adjusts K% as needed

!   When new price change is identical to old price change
    IF   PREV.RPD.NO$(PREV%) = CURR.RPD.NO$(CURR%) \
      AND PREV.PRICE$(PREV%) = CURR.PRICE$(CURR%) THEN BEGIN
!       Keep old price change flags
        PPFK.STATUS.FLAG$(PPFK%)  = PREV.STATUS.FLAG$(PREV%)
        PPFK.INC.DEC.FLAG$(PPFK%) = PREV.INC.DEC.FLAG$(PREV%)
        PPFK.MARKDOWN$(PPFK%)     = PREV.MARKDOWN$(PREV%)
        ENDIF

END SUB


\******************************************************************************
\***
\***    PROCESS.PPFK.ITEM
\***
\******************************************************************************
\***
\***    Merges old and new price changes on the PPFK
\***
\******************************************************************************

SUB PROCESS.PPFK.ITEM PUBLIC

    INTEGER*1 RC%
    INTEGER*1 END.OF.PRICE.CHANGES

!   STOP.CHECK          = FALSE                                               ! 1.6 RC (109)
!   REMOVE.COUNT.CIPPMR = 0                                                   ! 1.5 RC (105)

    PPFK.BOOTS.CODE$ = CURR.BOOTS.CODE$

!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   Read PPFK to obtain any PPFI price changes                                ! 1.3 RC (53)
!   Pass these from the PPFK tables to the PREV tables                        ! 1.3 RC (53)
!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   This section re-written for Rv 1.3 RC (53)                                ! 1.3 RC (53)

    PREV.PCR.COUNT% = 0

    ! Read PPFK record to check if price changes already exist for this item
    RC% = READ.PPFK ! Sets PPFK.PCR.COUNT%

    !IF price changes already exist for this item
    IF RC% = 0 THEN BEGIN

        PPFK% = 1 ! Subscript for PPFK tables holding PPFI data
        PREV% = 1 ! Subscript for PREV tables to be populated from PPFK

!       For each existing price change
        WHILE PPFK% <= PPFK.PCR.COUNT%

!           Move all price changes from PPFK to PREV tables     ! 1.7 RC (113)
            PREV.PCR.COUNT% = PREV.PCR.COUNT% + 1
            CALL SET.PREV.FROM.PPFK
            PREV% = PREV% + 1

            PPFK% = PPFK% + 1
        WEND

    ENDIF

!   SET UP EMERGENCY 99999 RPD                                                  ! 1.7 RC (113)
!   When the price on the IUF item record does not match the IRF sale price     ! 1.9 RC (177)
!   (of LOCAL Head Office price, or CIPPMR Reversals Price)                     ! 1.9 RC (177)
!     even if the item is locally priced                                        ! 1.7 RC (113)
!     even if the item is on CIP markdown                                       ! 1.7 RC (113)
!     and there is no price change for today on (IUF) CURR data table           ! 1.7 RC (113)
!       Place an emergency 99999 RPD onto the CURR data table                   ! 1.7 RC (113)
    IF PRICE.MISMATCH \                            ! Price discrepancy          ! 1.9 RC
\     AND (IRF.INDICAT3%    AND 20h) = 0h \        ! Local price flag OFF       ! 1.7 RC (113)
\     AND (IDF.BIT.FLAGS.1% AND 20h) = 0h \        ! Markdown flag OFF          ! 1.7 RC (113)
      AND NOT PRICE.CHANGE.TODAY  THEN BEGIN       ! No price changes today     ! 1.7 RC (113)
        CURR.PCR.COUNT% = CURR.PCR.COUNT% + 1                                   ! 1.7 RC (113)
        CALL STORE.EMERG.PRICE.CHANGE(CURR.PCR.COUNT%)                          ! 1.7 RC (113)
    ENDIF                                                                       ! 1.7 RC (113)

!   Line deleted                                                                ! 1.7 RC (113)
!   PRICE.CHANGE.TODAY now reset at end of PROCESS.IUF.ITEM                     ! 1.7 RC (113)

!   Sort CURR price change data by ascending date-RPD      ! 1.9 RC (177)
!   if not already in this order                           ! 1.7 RC (113)
    IF CURR.SORT.NEEDED THEN BEGIN                         ! 1.7 RC (113)
        CALL SORT.CURR.TABLES                              ! 1.7 RC (113)
    ENDIF

!   At this point ...
!   PREV table holds all PPFI price changes for the item   ! 1.7 RC (113)
!   CURR table holds all IUF price changes for the item
!   PPFK table will be used to hold the merge of these

    PREV% = 1 ! Subscript for old price changes from PPFI
    CURR% = 1 ! Subscript for new price changes from IUF

    PPFK% = 0 ! Subscript for merged price changes for PPFK
              ! This is incremented within sub-routines as needed to cater
              ! for exclusion of markdown items and for emergency RPD's

!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   Populate PPFK tables by merge of PREV and CURR tables data (if any)       ! 1.3 RC (53)
!   until either the PREV or CURR tables have been fully processed then       ! 1.3 RC (53)
!   continue populating the PPFK tables from the remaining data               ! 1.3 RC (53)
!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   This section new for Rv 1.3 RC (53)                                       ! 1.3 RC (53)

!   Both PPFK and CURR data tables are in ascending date-RPD order (latest last) ! 1.9 RC (177)
!   This merge routine selects the oldest price change(s) first, then the next   ! 1.9 RC (177)
!   oldest price change(s), leaving most future price change(s) until last       ! 1.9 RC (177)

    WHILE PREV% <= PREV.PCR.COUNT% \
      AND CURR% <= CURR.PCR.COUNT%

        IF   PREV.DATE.DUE$(PREV%) < CURR.DATE.DUE$(CURR%) THEN BEGIN   ! 1.9 RC (177)

                 CALL SET.PPFK.FROM.PREV ! Adjusts PPFK% as needed
                 PREV% = PREV% + 1

        ENDIF ELSE BEGIN

         IF  PREV.DATE.DUE$(PREV%) > CURR.DATE.DUE$(CURR%) THEN BEGIN   ! 1.9 RC (177)

                 CALL SET.PPFK.FROM.CURR ! Adjusts PPFK% as needed
                 CURR% = CURR% + 1

         ENDIF ELSE BEGIN

          IF PREV.DATE.DUE$(PREV%) = CURR.DATE.DUE$(CURR%) THEN BEGIN

                 CALL SET.PPFK.FROM.CURR.PLUS.PREV ! Adjusts PPFK% as needed
                 PREV% = PREV% + 1
                 CURR% = CURR% + 1

          ENDIF
         ENDIF
        ENDIF

    WEND

!   Populate PPFK tables from any remaining PREV tables data
    WHILE PREV% <= PREV.PCR.COUNT%
        CALL SET.PPFK.FROM.PREV ! Adjusts PPFK% as needed
        PREV% = PREV% + 1
    WEND

!   Populate PPFK tables from any remaining CURR table data
    WHILE CURR% <= CURR.PCR.COUNT%
        CALL SET.PPFK.FROM.CURR ! Adjusts PPFK% as needed
        CURR% = CURR% + 1
    WEND

!   Section removed                                             ! 1.7 RC (113)

!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   Update PPFK file from PPFK tables merged from PREV (PPFI) and CURR (IUF)  ! 1.3 RC (53)
!---------------------------------------------------------------------------- ! 1.3 RC (53)
!   This section re-structured for Rv 1.3 RC (53)                             ! 1.3 RC (53)

!   Set PPFK file variable PPFK.PCR.COUNT$ to number of PPFK records (if any)
!   to be written back to the PPFK (as this controls function WRITE.PPFK)
    PPFK.PCR.COUNT% = PPFK%

!   If new price changes on PPFK table write them to the PPFK file            ! 1.1 RC
    IF PPFK.PCR.COUNT% <> 0 THEN BEGIN

        RC% = WRITE.PPFK

        IF RC% <> 0 THEN BEGIN
            ! Handle error - Write error on PPFK
            CALL DO.MESSAGE("PSB21 *** WRITE ERROR ON PPFK", FALSE)
            CALL DO.MESSAGE("          PPFK.PCR.COUNT%: " + STR$(PPFK.PCR.COUNT%), FALSE)
            CALL LOG.EVENT(106)
        ENDIF

    ENDIF ELSE BEGIN                                                          ! 1.3 RC (53)

!       No price changes on PPFK table merged from PREV (PPFI) and CURR (IUF) ! 1.3 RC (53)
!       but check if some (from input PPFI) which need to be deleted          ! 1.3 RC (53)
        IF PPFK.PCR.COUNT.READ% <> 0 THEN BEGIN                               ! 1.3 RC (53)

            ! Delete old price changes from PPFK
            RC% = DELETE.PPFK

            IF RC% <> 0 THEN BEGIN
                ! Handle error - Delete error on PPFK
                CALL DO.MESSAGE("PSB21 *** DELETE ERROR ON PPFK", FALSE)
                CALL LOG.EVENT(106)
            ENDIF

        ENDIF                                                                 ! 1.3 RC (53)
    ENDIF                                                                     ! 1.3 RC (53)

END SUB


\******************************************************************************
\***
\***    RENAME.PPFO.TO.PPFI
\***
\******************************************************************************
\***
\***    Replaces the old PPFI with the new PPFO
\***
\******************************************************************************
FUNCTION RENAME.PPFO.TO.PPFI !PRIVATE

    INTEGER*1 RENAME.PPFO.TO.PPFI,RETRY.COUNT%
    INTEGER*4 RETC%

    RENAME.PPFO.TO.PPFI = 0

    RETC% = -1
    RETRY.COUNT% = 0

    ! While we haven't successfully copied the PPFO file AND
    ! we haven't had enough of retrying yet
    WHILE (RETC% < 0  AND RETRY.COUNT% < 4)

        ! Replace the PPFI file with the PPFO file
        CALL ADXCOPYF(RETC%,PPFO.FILE.NAME$,PPFI.FILE.NAME$,0,1,0)

        RETRY.COUNT% = RETRY.COUNT% + 1

        ! IF the copy failed for some strange reason
        IF (RETC% < 0)  THEN BEGIN
            ! Wait for a bit....
            WAIT; 15000
        ENDIF

    WEND

    ! IF PPFO copy was successful
    IF RETC% = 0 THEN BEGIN

        CURRENT.REPORT.NUM% = PPFO.REPORT.NUM%
        ! Open the PPFO file, then delete it
        FILE.OPERATION$ = "O"
        OPEN PPFO.FILE.NAME$ AS PPFO.SESS.NUM%
        FILE.OPERATION$ = "D"
        DELETE PPFO.SESS.NUM%

    ! ELSE IF we still couldn't copy the PPFO
    ENDIF ELSE IF (RETC% < 0)  THEN BEGIN
        RENAME.PPFO.TO.PPFI = 1
    ENDIF

END FUNCTION

\******************************************************************************
\***
\***    CREATE.NEW.PPFI
\***
\******************************************************************************
\***
\***    Create a new PPFI from
\***
\******************************************************************************
SUB CREATE.NEW.PPFI PUBLIC

    INTEGER*4   RC%
    INTEGER*4   PPFK.FID%
    INTEGER*4   PPFK.RETLEN%
    INTEGER*4   PPFK.OFFSET%
    INTEGER*4   PPFK.COUNT%
    INTEGER*4   PPFK.KEY%
    INTEGER*2   I%
    INTEGER*2   PPFK.MODE%                                                  !AMW
    INTEGER*2   PPFK.OPTIONS%
    INTEGER*2   PPFK.PCR.INDEX%
    INTEGER*1   PPFK.START.SEQ%
    INTEGER*1   PPFK.END.SEQ%
    INTEGER*1   END.OF.PPFK.RECORDS
    INTEGER*1   END.OF.PRICE.CHANGES
    STRING      PPFK.RECORD$
    STRING      PPFK.DATA$
    STRING      PPFK.PCR$
    STRING      PPFK.PATTERN$

    PHASE$ = "4.0"                                                  ! 1.12 CSk
    CALL DO.MESSAGE("PSB21 PHASE 4", TRUE)
    CALL DO.MESSAGE("PSB21 4.0 - CREATE.NEW.PPFI", TRUE)

    PPFK.RETLEN%  = PPFK.RECL%                    ! Get entire PPFK record
    PPFK.OPTIONS% = 201AH                         ! Opens file with read access only
    PPFK.PATTERN$ = ""                            ! No search pattern required
    PPFK.OFFSET%  = 0                             ! Start from beginning of PPFK record
    PPFK.RECORD$  = STRING$(PPFK.RETLEN%,CHR$(0)) ! Initialise returned string
    PPFK.MODE%    = 0                             ! Pattern value mode      !AMW

    ! Set record type to price change (this will be set to 'R'
    ! for every PPFO record other than the trailer)
    PPF.REC.TYPE.FLAG$ = "R"

\------------------------------------------------------------------------------
\---
\---    STEP 1: Create a new empty PPFO file
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.1 - Create Empty PPFI", TRUE)

    ! Create new empty PPFO file
    RC% = CREATE.PPFO

    ! IF new PPFO was successfully created
    IF RC% = 0 THEN BEGIN

        ! Initialise the PPFO record count
        PPFO.REC.COUNT% = 0

    ENDIF ELSE BEGIN
        ! Handle error - Failed creating new PPFO
        CALL DO.MESSAGE("PSB21 *** ERROR cannot create PPFO", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 2: Pre-count the number of PPFK records so we know how big
\---            to make our binary tree
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.2 - Pre-count number of PPFK records", TRUE) ! 1.2 RC (50)

    ! Open PPFK as a direct file read access only
    PPFK.FID% = INITKF(PPFK.FILE.NAME$,PPFK.OPTIONS%)

    ! Check for errors opening PPFK
    IF PPFK.FID% <= 0 THEN BEGIN
        ! Handle error - open error
        CALL DO.MESSAGE("PSB21 *** INITKF ERROR = " + HEX4$(PPFK.FID%), FALSE)
        STOP
    ENDIF


   END.OF.PPFK.RECORDS = FALSE

    PPFK.COUNT% = 0

    ! WHILE there are more PPFK records to process
    WHILE NOT END.OF.PPFK.RECORDS

        ! Read next PPFK record
        RC% = READKF(PPFK.RECORD$,PPFK.PATTERN$,PPFK.OFFSET%,PPFK.MODE%)    !AMW

        ! Check whether end of file reached
        IF RC% = 0 THEN BEGIN

            PPFK.COUNT% = PPFK.COUNT% + 1

        ENDIF ELSE BEGIN

            ! Check whether an error has been returned
            IF RC% < -1 THEN BEGIN
                CALL DO.MESSAGE("PSB21 *** READKF ERROR = " + HEX4$(RC%), FALSE)
                CALL DO.MESSAGE("          PPFK.RECORD$ : " + PPFK.RECORD$, FALSE)
                CALL DO.MESSAGE("          PPFK.PATTERN$: " + PPFK.PATTERN$, FALSE)
                CALL DO.MESSAGE("          PPFK.OFFSET% : " + STR$(PPFK.OFFSET%), FALSE)
            ENDIF

            END.OF.PPFK.RECORDS = TRUE

        ENDIF
    WEND

    ! Close PPFK file and deallocate memory
    RC% = TERMKF(PPFK.FID%)

    ! IF close of PPFK failed
    IF RC% < 0 THEN BEGIN
        CALL DO.MESSAGE("PSB21 *** TERMKF ERROR = " + HEX4$(RC%), FALSE)
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 3: Create empty binary tree
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.3 - Make binary tree for new PPFI recs", TRUE) ! 1.2 RC (50)

    ! Create binary tree to store new PPFI records
    RC% = BTREE.CREATE.TREE(PPFK.COUNT%)

    IF RC% <> 0 THEN BEGIN
        ! Handle error - maximum possible tree size exceeded
        CALL DO.MESSAGE("PSB21 *** MAXIMUM POSSIBLE TREE SIZE EXCEEDED", FALSE)
        CALL DO.MESSAGE("          PPFK.COUNT%: " + STR$(PPFK.COUNT%), FALSE)
        STOP
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 4: Process PPFK records and insert them into the binary tree
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.4 - Insert PPFK recs into binary tree", TRUE) ! 1.2 RC (50)

    ! Open PPFK as a direct file read access only
    PPFK.FID% = INITKF(PPFK.FILE.NAME$,PPFK.OPTIONS%)

    ! Check for errors opening PPFK
    IF PPFK.FID% <= 0 THEN BEGIN
        ! Handle error - open error
        CALL DO.MESSAGE("PSB21 *** INITKF ERROR = " + HEX4$(PPFK.FID%), FALSE)
        CALL DO.MESSAGE("          PPFK.FILE.NAME$: " + PPFK.FILE.NAME$, FALSE)
        CALL DO.MESSAGE("          PPFK.OPTIONS%  : " + STR$(PPFK.OPTIONS%), FALSE)
        STOP
    ENDIF

    END.OF.PPFK.RECORDS = FALSE

    ! WHILE there are more PPFK records to process
    WHILE NOT END.OF.PPFK.RECORDS

        ! Read next PPFK record
        RC% = READKF(PPFK.RECORD$,PPFK.PATTERN$,PPFK.OFFSET%,PPFK.MODE%)    !AMW

        ! Check whether end of file reached
        IF RC% = 0 THEN BEGIN

            ! Get key value and data from PPFK record
            PPFK.KEY%       = VAL(UNPACK$(LEFT$(PPFK.RECORD$,5)))
            PPFK.DATA$      = RIGHT$(PPFK.RECORD$,78)

            PPFK.START.SEQ% = VAL(UNPACK$(MID$(PPFK.RECORD$,5,1)))
            PPFK.END.SEQ%   = VAL(UNPACK$(MID$(PPFK.RECORD$,6,1)))
            PPFK.PCR$       = ""

            ! IF this is the last PPFK sequence record (and hence may be only partially full)
            IF PPFK.START.SEQ% = PPFK.END.SEQ% THEN BEGIN

                END.OF.PRICE.CHANGES = FALSE

                I% = 1

                ! WHILE there are still price changes to process
                WHILE NOT END.OF.PRICE.CHANGES

                    ! Calculate the start index position of the next price change record
                    PPFK.PCR.INDEX% = ((I% - 1) * PPFK.PCR.RECL%) + 1

                    ! IF we have an empty price change record
                    IF MID$(PPFK.DATA$,PPFK.PCR.INDEX%,3) = PPFK.PACK03$ THEN BEGIN
                        END.OF.PRICE.CHANGES = TRUE
                    ENDIF ELSE BEGIN
                        ! Add price change record
                        PPFK.PCR$ = PPFK.PCR$ + MID$(PPFK.DATA$,PPFK.PCR.INDEX%,PPFK.PCR.RECL%)
                    ENDIF

                    ! Move to the next price change record
                    I% = I% + 1

                    ! IF we've just processed the last price change in the sequence record
                    IF I% > PPFK.PCR.PER.RECORD% THEN BEGIN
                        END.OF.PRICE.CHANGES = TRUE
                    ENDIF

                WEND

            ! ELSE the sequence record must be fully populated with price change records
            ENDIF ELSE BEGIN

                PPFK.PCR$ = PPFK.DATA$

            ENDIF

            ! Insert PPFK record into binary tree
            RC% = BTREE.INSERT.NODE(PPFK.KEY%,PPFK.PCR$)

            IF RC% <> 0 THEN BEGIN
                ! Handle error - Couldn't insert new node
                CALL DO.MESSAGE("PSB21 *** BTREE INSERT ERROR", FALSE)
                CALL DO.MESSAGE("          PPFK.KEY%: " + STR$(PPFK.KEY%), FALSE)
                CALL DO.MESSAGE("          PPFK.PCR$: " + PPFK.PCR$, FALSE)
            ENDIF

        ENDIF ELSE BEGIN

            ! Check whether an error has been returned
            IF RC% < -1 THEN BEGIN
                ! Handle error - open error
                CALL DO.MESSAGE("PSB21 *** READKF ERROR " + HEX4$(RC%), FALSE)
                CALL DO.MESSAGE("          PPFK.RECORD$ : " + PPFK.RECORD$, FALSE)
                CALL DO.MESSAGE("          PPFK.PATTERN$: " + PPFK.PATTERN$, FALSE)
                CALL DO.MESSAGE("          PPFK.OFFSET% : " + STR$(PPFK.OFFSET%), FALSE)
                !Create new event
                STOP
            ENDIF

            END.OF.PPFK.RECORDS = TRUE

        ENDIF
    WEND

    ! Close PPFK file and deallocate memory
    RC% = TERMKF(PPFK.FID%)

    ! IF close of PPFK failed
    IF RC% < 0 THEN BEGIN
        CALL DO.MESSAGE("PSB21 *** TERMKF ERROR = " + HEX4$(RC%), FALSE)
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 5: Traverse binary tree 'inorder' and write records to PPFO
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.5 - Process binary tree; Write to PPFO", TRUE) ! 1.2 RC (50)

    IF PPFK.COUNT% > 0 THEN BEGIN
        ! Traverse binary tree starting at the root node
        ! CALL BTREE.TRAVERSE.INORDER(BTREE.ROOT%)
        CALL BTREE.TRAVERSE.TREE(BTREE.ROOT%)
    ENDIF ELSE BEGIN
        CALL DO.MESSAGE("PSB21 4.5a- No records found on PPFI", TRUE)
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 6: Add trailer to PPFO
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.6 - Add trailer to PPFO", TRUE)

    PPF.BOOTS.CODE$    = "9999999"
    PPF.REC.TYPE.FLAG$ = "T"

    ! Calculate PPFO record count (including trailer)
    PPF.REC.COUNT$ = RIGHT$("00000" + STR$(PPFO.REC.COUNT% + 1),5)

    ! Write trailer record to PPFO
    RC% = WRITE.PPFO

    ! IF write to PPFO failed
    IF RC% <> 0 THEN BEGIN
        ! Handle error - Failed to write PPFO trailer
        CALL DO.MESSAGE("PSB21 *** ERROR writing PPFO trailer", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

    ! Close the PPFO file
    CLOSE PPFO.SESS.NUM%

\------------------------------------------------------------------------------
\---
\---    STEP 7: Replace the old PPFI with the new PPFO
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.7 - Replace old PPFI with new PPFO", TRUE) ! 1.2 RC (50)

    ! Copy PPFO file to PPFI file, then delete PPFO
    RC% = RENAME.PPFO.TO.PPFI

    IF RC% <> 0 THEN BEGIN
        ! Handle error - Failed renaming PPFO to PPFI
        CALL DO.MESSAGE("PSB21 *** ERROR copying PPFO", FALSE)
        CALL LOG.EVENT(106)
    ENDIF

\------------------------------------------------------------------------------
\---
\---    STEP 8: Destroy the binary tree
\---
\------------------------------------------------------------------------------

    CALL DO.MESSAGE("PSB21 4.8 - Delete binary tree; Free up memory", TRUE) ! 1.2 RC (50)

    ! Delete binary tree and deallocate memory
    CALL BTREE.DESTROY.TREE

END SUB

