
\******************************************************************************
\******************************************************************************
\***
\***    Pending Prices Keyed File (PPFK) public functions
\***
\******************************************************************************
\***
\***    Version 1.0             Mark Walker                 13th October 2011
\***    Initial version.
\***
\***    VERSION 1.1.                ROBERT COWEY.                02 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.5.
\***    Defect 2678
\***    Defined variable PPFK.PCR.COUNT.READ% to save total number of price 
\***    changes for an item as found by READ.PPFK.
\***    Modified WRITE.PPFK to clear any unwanted price change data from the
\***    PPFK tables populated by most recent READ.PPFK and to delete any 
\***    remaining unwanted PPFK price change records.
\***    This prevents unwanted retention of superceded price changes.
\***
\***    VERSION 1.2.                ROBERT COWEY.                01 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.7.
\***    Defect 53 - Commented 1.2 RC (53)
\***    Modified function DELETE.PPFK to use PPFK.PCR.COUNT.READ% to 
\***    determine number of PPFK record entries to be deleted.
\***    Removed redundant (never executed) code preceding FOR loop.
\***
\******************************************************************************
\******************************************************************************

    INTEGER*1 GLOBAL            \
        TRUE,                   \
        FALSE

    STRING                      \
        PPFK.KEY$,              \
        PPFK.END.SEQ$,          \
        WORK.BOOTS.CODE$,       \
        WORK.DATE.DUE$(1),      \
        WORK.RPD.NO$(1),        \
        WORK.STATUS.FLAG$(1),   \
        WORK.INC.DEC.FLAG$(1),  \
        WORK.PRICE$(1),         \
        WORK.MARKDOWN$(1)

%INCLUDE PPFKDEC.J86

\******************************************************************************
\***
\***    PPFK.SET
\***
\******************************************************************************
\***
\***    Define PPFK constants
\***
\******************************************************************************
FUNCTION PPFK.SET PUBLIC
                                     ! Use W-RAM Drive for speed. No PPFK
    PPFK.FILE.NAME$  = "W:\PPFK.BIN" ! logical as only this program uses it.
    PPFK.REPORT.NUM% = 835 

    PPFK.RECL% = 84     ! Record length
    PPFK.KEYL% = 5      ! Key length
    PPFK.MAXR% = 307000 ! Maximum number of records

    ! Maximum number of pending price changes allowed for a single item
    PPFK.PCR.MAX% = 120

    ! Dimension arrays for maximum number of expected price changes
    DIM PPFK.DATE.DUE$(PPFK.PCR.MAX%)
    DIM PPFK.RPD.NO$(PPFK.PCR.MAX%)
    DIM PPFK.STATUS.FLAG$(PPFK.PCR.MAX%)
    DIM PPFK.INC.DEC.FLAG$(PPFK.PCR.MAX%)
    DIM PPFK.PRICE$(PPFK.PCR.MAX%)
    DIM PPFK.MARKDOWN$(PPFK.PCR.MAX%)

    ! Number of pending price changes per PPFK sequence record
    PPFK.PCR.PER.RECORD% = 6

    ! Record length of pending price change sub-records
    PPFK.PCR.RECL% = 13

    !Dimension arrays for number of pending price changes per PPFK sequence record
    DIM WORK.DATE.DUE$(PPFK.PCR.PER.RECORD%)
    DIM WORK.RPD.NO$(PPFK.PCR.PER.RECORD%)
    DIM WORK.STATUS.FLAG$(PPFK.PCR.PER.RECORD%)
    DIM WORK.INC.DEC.FLAG$(PPFK.PCR.PER.RECORD%)
    DIM WORK.PRICE$(PPFK.PCR.PER.RECORD%)
    DIM WORK.MARKDOWN$(PPFK.PCR.PER.RECORD%)

    ! Define pre-packed null values (for performance reasons)
    PPFK.PACK01$ = PACK$("00")
    PPFK.PACK03$ = PACK$("000000")
    PPFK.PACK04$ = PACK$("00000000")

    ! Build format string for reading PPFK sequence records
    PPFK.READ.FORM$ = "T6,C1" + \
                      STRING$(PPFK.PCR.PER.RECORD%,",C3,C3,C1,C1,C4,C1")

    ! Build format string for writing PPFK sequence records
    PPFK.WRITE.FORM$ = "C5,C1" + \
                       STRING$(PPFK.PCR.PER.RECORD%,",C3,C3,C1,C1,C4,C1")

END FUNCTION

\******************************************************************************
\***
\***    READ.PPFK.SEQ
\***
\******************************************************************************
\***
\***    Reads a single sequence record from the PPFK for a specified item
\***
\***    INPUT:
\***        PPFK.KEY$ - Boots item code and sequence          4 UPD + 1 UPD
\***
\***    OUTPUT:
\***        PPFK.END.SEQ$ - Count of PPFK sequence records            1 UPD
\***
\***        The following fields are populated 'n' times
\***        WORK.DATE.DUE$(n)     - Date price change is due          3 UPD
\***        WORK.RPD.NO$(n)       - RPD Number                        3 UPD
\***        WORK.STATUS.FLAG$(n)  - SEL Printed flag                  1 ASC
\***        WORK.INC.DEC.FLAG$(n) - Price Increase/Decrease flag      1 ASC
\***        WORK.PRICE$(n)        - New price                         4 UPD
\***        WORK.MARKDOWN$(n)     - Markdown flag                     1 ASC
\***
\***        where n = number of price change records per sequence record
\***
\******************************************************************************
FUNCTION READ.PPFK.SEQ !PRIVATE

    INTEGER*1 READ.PPFK.SEQ

    READ.PPFK.SEQ = 1

    IF END #PPFK.SESS.NUM% THEN PPFK.READ.ERROR
    READ FORM PPFK.READ.FORM$; #PPFK.SESS.NUM%      \
             KEY PPFK.KEY$;                         \
             PPFK.END.SEQ$,                         \
             WORK.DATE.DUE$(1),                     \
             WORK.RPD.NO$(1),                       \
             WORK.STATUS.FLAG$(1),                  \
             WORK.INC.DEC.FLAG$(1),                 \
             WORK.PRICE$(1),                        \
             WORK.MARKDOWN$(1),                     \
             WORK.DATE.DUE$(2),                     \
             WORK.RPD.NO$(2),                       \
             WORK.STATUS.FLAG$(2),                  \
             WORK.INC.DEC.FLAG$(2),                 \
             WORK.PRICE$(2),                        \
             WORK.MARKDOWN$(2),                     \
             WORK.DATE.DUE$(3),                     \
             WORK.RPD.NO$(3),                       \
             WORK.STATUS.FLAG$(3),                  \
             WORK.INC.DEC.FLAG$(3),                 \
             WORK.PRICE$(3),                        \
             WORK.MARKDOWN$(3),                     \
             WORK.DATE.DUE$(4),                     \
             WORK.RPD.NO$(4),                       \
             WORK.STATUS.FLAG$(4),                  \
             WORK.INC.DEC.FLAG$(4),                 \
             WORK.PRICE$(4),                        \
             WORK.MARKDOWN$(4),                     \
             WORK.DATE.DUE$(5),                     \
             WORK.RPD.NO$(5),                       \
             WORK.STATUS.FLAG$(5),                  \
             WORK.INC.DEC.FLAG$(5),                 \
             WORK.PRICE$(5),                        \
             WORK.MARKDOWN$(5),                     \
             WORK.DATE.DUE$(6),                     \
             WORK.RPD.NO$(6),                       \
             WORK.STATUS.FLAG$(6),                  \
             WORK.INC.DEC.FLAG$(6),                 \
             WORK.PRICE$(6),                        \
             WORK.MARKDOWN$(6)

    READ.PPFK.SEQ = 0

    PPFK.READ.ERROR:

END FUNCTION

\******************************************************************************
\***
\***    WRITE.PPFK.SEQ
\***
\******************************************************************************
\***
\***    Writes a single sequence record to the PPFK for a specified item
\***
\***    INPUT:
\***        PPFK.KEY$     - Boots item code and sequence      4 UPD + 1 UPD
\***        PPFK.END.SEQ$ - Count of PPFK sequence records            1 UPD
\***
\***        The following fields are required 'n' times
\***        WORK.DATE.DUE$(n)     - Date price change is due          3 UPD
\***        WORK.RPD.NO$(n)       - RPD Number                        3 UPD
\***        WORK.STATUS.FLAG$(n)  - SEL Printed flag                  1 ASC
\***        WORK.INC.DEC.FLAG$(n) - Price Increase/Decrease flag      1 ASC
\***        WORK.PRICE$(n)        - New price                         4 UPD
\***        WORK.MARKDOWN$(n)     - Markdown flag                     1 ASC
\***
\***        where n = number of price change records per sequence record
\***
\******************************************************************************
FUNCTION WRITE.PPFK.SEQ !PRIVATE

    INTEGER*1 WRITE.PPFK.SEQ

    WRITE.PPFK.SEQ = 1

    IF END #PPFK.SESS.NUM% THEN PPFK.WRITE.ERROR
    WRITE FORM PPFK.WRITE.FORM$; #PPFK.SESS.NUM%;   \
             PPFK.KEY$,                             \
             PPFK.END.SEQ$,                         \
             WORK.DATE.DUE$(1),                     \
             WORK.RPD.NO$(1),                       \
             WORK.STATUS.FLAG$(1),                  \
             WORK.INC.DEC.FLAG$(1),                 \
             WORK.PRICE$(1),                        \
             WORK.MARKDOWN$(1),                     \
             WORK.DATE.DUE$(2),                     \
             WORK.RPD.NO$(2),                       \
             WORK.STATUS.FLAG$(2),                  \
             WORK.INC.DEC.FLAG$(2),                 \
             WORK.PRICE$(2),                        \
             WORK.MARKDOWN$(2),                     \
             WORK.DATE.DUE$(3),                     \
             WORK.RPD.NO$(3),                       \
             WORK.STATUS.FLAG$(3),                  \
             WORK.INC.DEC.FLAG$(3),                 \
             WORK.PRICE$(3),                        \
             WORK.MARKDOWN$(3),                     \
             WORK.DATE.DUE$(4),                     \
             WORK.RPD.NO$(4),                       \
             WORK.STATUS.FLAG$(4),                  \
             WORK.INC.DEC.FLAG$(4),                 \
             WORK.PRICE$(4),                        \
             WORK.MARKDOWN$(4),                     \
             WORK.DATE.DUE$(5),                     \
             WORK.RPD.NO$(5),                       \
             WORK.STATUS.FLAG$(5),                  \
             WORK.INC.DEC.FLAG$(5),                 \
             WORK.PRICE$(5),                        \
             WORK.MARKDOWN$(5),                     \
             WORK.DATE.DUE$(6),                     \
             WORK.RPD.NO$(6),                       \
             WORK.STATUS.FLAG$(6),                  \
             WORK.INC.DEC.FLAG$(6),                 \
             WORK.PRICE$(6),                        \
             WORK.MARKDOWN$(6)

    WRITE.PPFK.SEQ = 0

    PPFK.WRITE.ERROR:

END FUNCTION

\******************************************************************************
\***
\***    DELETE.PPFK.SEQ
\***
\******************************************************************************
\***
\***    Deletes a single sequence record from the PPFK for a specified item
\***
\***    INPUT:
\***        PPFK.KEY$ - Boots item code and sequence          4 UPD + 1 UPD
\***
\******************************************************************************
FUNCTION DELETE.PPFK.SEQ !PRIVATE

    INTEGER*1 DELETE.PPFK.SEQ

    DELETE.PPFK.SEQ = 1

    IF END # PPFK.SESS.NUM% THEN PPFK.DELETE.ERROR
    DELREC PPFK.SESS.NUM%; PPFK.KEY$

    DELETE.PPFK.SEQ = 0

    PPFK.DELETE.ERROR:

END FUNCTION

\******************************************************************************
\***
\***    READ.PPFK
\***
\******************************************************************************
\***
\***    Reads all pending prices from the PPFK file for a specified item
\***
\***    INPUT:
\***        PPFK.BOOTS.CODE$      - 7-digit boots item code           7 ASC
\***
\***    OUTPUT:
\***        PPFK.PCR.COUNT%       - Number of price change records    2 INT
\***
\***        The following arrays will be fully populated:
\***        PPFK.DATE.DUE$(n)     - Date price change is due          3 UPD
\***        PPFK.RPD.NO$(n)       - RPD Number                        3 UPD
\***        PPFK.STATUS.FLAG$(n)  - SEL Printed flag                  1 ASC
\***        PPFK.INC.DEC.FLAG$(n) - Price Increase/Decrease flag      1 ASC
\***        PPFK.PRICE$(n)        - New price                         4 UPD
\***        PPFK.MARKDOWN$(n)     - Markdown flag                     1 ASC
\***
\***        where n = count of number of price change records
\***
\******************************************************************************
FUNCTION READ.PPFK PUBLIC

    INTEGER*1   READ.PPFK
    INTEGER*1   RC%
    INTEGER*1   END.OF.PPFK.PRICES
    INTEGER*1   END.OF.PPFK.RECORDS
    INTEGER*2   PPFK.RECORD.COUNT%
    INTEGER*2   PPFK.SEQ.COUNT%
    INTEGER*2   I%,J%,K%

    READ.PPFK = 1

    ! Initialise total number of price changes for this item
    PPFK.PCR.COUNT% = 0
    PPFK.PCR.COUNT.READ% = 0                                          ! 1.1 RC

    ! Pack item code
    WORK.BOOTS.CODE$ = PACK$("0" + PPFK.BOOTS.CODE$)

    ! Build key field for initial record
    PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$("01")

    ! Read the first PPFK sequence record
    RC% = READ.PPFK.SEQ

    PPFK.RECORD.COUNT% = VAL(UNPACK$(PPFK.END.SEQ$))

    ! IF PPFK sequence record read was unsuccessful
    IF RC% <> 0 THEN BEGIN
        ! Ensure all output values are reset when a file read error occurs
        PPFK.PCR.COUNT% = 0
        DIM PPFK.DATE.DUE$(PPFK.PCR.MAX%)
        DIM PPFK.RPD.NO$(PPFK.PCR.MAX%)
        DIM PPFK.STATUS.FLAG$(PPFK.PCR.MAX%)
        DIM PPFK.INC.DEC.FLAG$(PPFK.PCR.MAX%)
        DIM PPFK.PRICE$(PPFK.PCR.MAX%)
        DIM PPFK.MARKDOWN$(PPFK.PCR.MAX%)

        EXIT FUNCTION

    ENDIF

    I% = 1  ! Index for PPFK sequence records

    END.OF.PPFK.RECORDS = FALSE

    ! WHILE there are still PPFK sequence records to process
    WHILE NOT END.OF.PPFK.RECORDS

        J% = 1  ! Index for price changes

        END.OF.PPFK.PRICES = FALSE

        ! WHILE there are still price changes to process
        WHILE NOT END.OF.PPFK.PRICES

            ! IF price change slot is empty
            IF WORK.DATE.DUE$(J%) = PPFK.PACK03$ THEN BEGIN

                END.OF.PPFK.PRICES = TRUE

            ENDIF ELSE BEGIN

                ! Increment total number of price changes for this item
                PPFK.PCR.COUNT% = PPFK.PCR.COUNT% + 1

                PPFK.DATE.DUE$(PPFK.PCR.COUNT%)     = WORK.DATE.DUE$(J%)
                PPFK.RPD.NO$(PPFK.PCR.COUNT%)       = WORK.RPD.NO$(J%)
                PPFK.STATUS.FLAG$(PPFK.PCR.COUNT%)  = WORK.STATUS.FLAG$(J%)
                PPFK.INC.DEC.FLAG$(PPFK.PCR.COUNT%) = WORK.INC.DEC.FLAG$(J%)
                PPFK.PRICE$(PPFK.PCR.COUNT%)        = WORK.PRICE$(J%)
                PPFK.MARKDOWN$(PPFK.PCR.COUNT%)     = WORK.MARKDOWN$(J%)

                ! IF we've just processed the record in the last price change slot
                IF J% = PPFK.PCR.PER.RECORD% THEN BEGIN
                    END.OF.PPFK.PRICES = TRUE
                ENDIF ELSE BEGIN
                    ! Move on to next price change slot
                    J% = J% + 1
                ENDIF

            ENDIF

        WEND

        ! IF there are more PPFK sequence records to read
        IF I% < PPFK.RECORD.COUNT% THEN BEGIN

            ! Move on to the next PPFK sequence record
            I% = I% + 1

            ! Build key field for the next record
            PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$(RIGHT$("00" + STR$(I%),2))

            ! Read the next PPFK sequence record
            RC% = READ.PPFK.SEQ

            ! IF PPFK sequence record read was unsuccessful
            IF RC% <> 0 THEN BEGIN

                ! Ensure all output values are reset when a file read error occurs
                PPFK.PCR.COUNT% = 0
                DIM PPFK.DATE.DUE$(PPFK.PCR.MAX%)
                DIM PPFK.RPD.NO$(PPFK.PCR.MAX%)
                DIM PPFK.STATUS.FLAG$(PPFK.PCR.MAX%)
                DIM PPFK.INC.DEC.FLAG$(PPFK.PCR.MAX%)
                DIM PPFK.PRICE$(PPFK.PCR.MAX%)
                DIM PPFK.MARKDOWN$(PPFK.PCR.MAX%)

                EXIT FUNCTION
            ENDIF

        ! ELSE we must have just processed the last PPFK sequence record
        ENDIF ELSE BEGIN
            END.OF.PPFK.RECORDS = TRUE
        ENDIF

    WEND

    PPFK.PCR.COUNT.READ% = PPFK.PCR.COUNT%                            ! 1.1 RC
    READ.PPFK = 0

END FUNCTION


\******************************************************************************
\***
\***    WRITE.PPFK
\***
\******************************************************************************
\***
\***    Writes all pending prices to the PPFK file for a specified item
\***
\***    INPUT:
\***        PPFK.BOOTS.CODE$      - 7-digit boots item code           7 ASC
\***        PPFK.PCR.COUNT%       - Number of price change records    2 INT
\***        PPFK.DATE.DUE$(n)     - Date price change is due          3 UPD
\***        PPFK.RPD.NO$(n)       - RPD Number                        3 UPD
\***        PPFK.STATUS.FLAG$(n)  - SEL Printed flag                  1 ASC
\***        PPFK.INC.DEC.FLAG$(n) - Price Increase/Decrease flag      1 ASC
\***        PPFK.PRICE$(n)        - New price                         4 UPD
\***        PPFK.MARKDOWN$(n)     - Markdown flag                     1 ASC
\***
\***        where n = count of number of price change records
\***
\******************************************************************************
FUNCTION WRITE.PPFK PUBLIC

    INTEGER*1   WRITE.PPFK
    INTEGER*1   RC%
    INTEGER*1   PPFK.PCRS.USED%
    INTEGER*2   I%,J%
    INTEGER*2   PPFK.RECORD.COUNT%
    INTEGER*2   PPFK.PCR.INDEX%

    WRITE.PPFK = 1

    ! Pack item code
    WORK.BOOTS.CODE$ = PACK$("0" + PPFK.BOOTS.CODE$)

    ! IF there are no price changes
    IF PPFK.PCR.COUNT% = 0 THEN BEGIN

        ! Build key field for initial PPFK sequence record
        PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$("01")

        ! Delete PPFK sequence record
        RC% = DELETE.PPFK.SEQ
        
        ! IF PPFK sequence record delete was unsuccessful
        IF RC% <> 0 THEN BEGIN
            EXIT FUNCTION
        ENDIF

        WRITE.PPFK = 0
        EXIT FUNCTION

    ENDIF

    ! Calculate the number of fully populated sequence records required
    PPFK.RECORD.COUNT% = INT(PPFK.PCR.COUNT% / PPFK.PCR.PER.RECORD%)

    ! Calculate how many used slots we have in the partially populated sequence record
    PPFK.PCRS.USED% = MOD(PPFK.PCR.COUNT%,PPFK.PCR.PER.RECORD%)

    ! Calculate whether we need an extra partially full sequence record
    IF PPFK.PCRS.USED% > 0 THEN BEGIN

        ! Include the partially populated sequence record in the total
        PPFK.RECORD.COUNT% = PPFK.RECORD.COUNT% + 1

        ! FOR all remaining price change slots in the partially full sequence record
        FOR I% = (PPFK.PCRS.USED% + 1) TO PPFK.PCR.PER.RECORD%

            ! Calculate position of empty price change slots
            PPFK.PCR.INDEX% = ((PPFK.RECORD.COUNT% - 1) * PPFK.PCR.PER.RECORD%) + I%

            ! Clear all values in the empty price change slots
            PPFK.DATE.DUE$(PPFK.PCR.INDEX%)     = PPFK.PACK03$
            PPFK.RPD.NO$(PPFK.PCR.INDEX%)       = PPFK.PACK03$
            PPFK.STATUS.FLAG$(PPFK.PCR.INDEX%)  = PPFK.PACK01$
            PPFK.INC.DEC.FLAG$(PPFK.PCR.INDEX%) = PPFK.PACK01$
            PPFK.PRICE$(PPFK.PCR.INDEX%)        = PPFK.PACK04$
            PPFK.MARKDOWN$(PPFK.PCR.INDEX%)     = PPFK.PACK01$

        NEXT I%

!       Clear remaining unwanted price change data (if any) populated by READ.PPFK   ! 1.1 RC
        WHILE I% <= PPFK.PCR.COUNT.READ%                                             ! 1.1 RC
!       Where I% is the total number of price changes to be written                  ! 1.1 RC
                                                                                     
            ! Calculate position of empty price change slots                         ! 1.1 RC
            PPFK.PCR.INDEX% = ((PPFK.RECORD.COUNT% - 1) * PPFK.PCR.PER.RECORD%) + I% ! 1.1 RC

            ! Clear all values in the empty price change slots                       ! 1.1 RC
            PPFK.DATE.DUE$(PPFK.PCR.INDEX%)     = PPFK.PACK03$                       ! 1.1 RC
            PPFK.RPD.NO$(PPFK.PCR.INDEX%)       = PPFK.PACK03$                       ! 1.1 RC
            PPFK.STATUS.FLAG$(PPFK.PCR.INDEX%)  = PPFK.PACK01$                       ! 1.1 RC
            PPFK.INC.DEC.FLAG$(PPFK.PCR.INDEX%) = PPFK.PACK01$                       ! 1.1 RC
            PPFK.PRICE$(PPFK.PCR.INDEX%)        = PPFK.PACK04$                       ! 1.1 RC
            PPFK.MARKDOWN$(PPFK.PCR.INDEX%)     = PPFK.PACK01$                       ! 1.1 RC

            I% = I% + 1                                                              ! 1.1 RC
        WEND                                                                         ! 1.1 RC

    ENDIF

    PPFK.PCR.INDEX% = 0 ! Index for price change records

    ! FOR each required PPFK sequence record
    FOR I% = 1 TO PPFK.RECORD.COUNT%

        ! FOR each PPFK price change slot
        FOR J% = 1 TO PPFK.PCR.PER.RECORD%

            ! Increment price change record index
            PPFK.PCR.INDEX% = PPFK.PCR.INDEX% + 1

            WORK.DATE.DUE$(J%)     = PPFK.DATE.DUE$(PPFK.PCR.INDEX%)
            WORK.RPD.NO$(J%)       = PPFK.RPD.NO$(PPFK.PCR.INDEX%)
            WORK.STATUS.FLAG$(J%)  = PPFK.STATUS.FLAG$(PPFK.PCR.INDEX%)
            WORK.INC.DEC.FLAG$(J%) = PPFK.INC.DEC.FLAG$(PPFK.PCR.INDEX%)
            WORK.PRICE$(J%)        = PPFK.PRICE$(PPFK.PCR.INDEX%)
            WORK.MARKDOWN$(J%)     = PPFK.MARKDOWN$(PPFK.PCR.INDEX%)

        NEXT J%

        ! Build key field for PPFK record
        PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$(RIGHT$("00" + STR$(I%),2))

        ! Set total number of PPFK sequence records
        PPFK.END.SEQ$ = PACK$(RIGHT$("00" + STR$(PPFK.RECORD.COUNT%),2))

        ! Write PPFK sequence record
        RC% = WRITE.PPFK.SEQ

        ! IF PPFK sequence record write was unsuccessful
        IF RC% <> 0 THEN BEGIN
            EXIT FUNCTION
        ENDIF

    NEXT I%

!   Delete remaining unwanted PPFK price change records (if any)        ! 1.1 RC
!   Redundant PPFK records arise when there are less PPFK records       ! 1.1 RC
!   written by WRITE.PPFK than read in by a previous READ.PPFK.         ! 1.1 RC

    WHILE PPFK.PCR.COUNT.READ% \                                        ! 1.1 RC
            > (I% -1) * PPFK.PCR.PER.RECORD%                            ! 1.1 RC
!   Where I% is the sequence number of the next PPFK price change       ! 1.1 RC
!   record for potential deletion                                       ! 1.1 RC
        
!       Build key field for PPFK record                                 ! 1.1 RC
        PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$(RIGHT$("00" + STR$(I%),2)) ! 1.1 RC

!       Delete PPFK sequence record                                     ! 1.1 RC
        RC% = DELETE.PPFK.SEQ                                           ! 1.1 RC

        IF RC% <> 0 THEN BEGIN                                          ! 1.1 RC
            EXIT FUNCTION                                               ! 1.1 RC
        ENDIF                                                           ! 1.1 RC
    
        I% = I% + 1                                                     ! 1.1 RC
    WEND                                                                ! 1.1 RC

    WRITE.PPFK = 0

END FUNCTION


\******************************************************************************
\***
\***    DELETE.PPFK
\***
\******************************************************************************
\***
\***    Deletes all pending prices from the PPFK file for a specified item
\***
\***    INPUT:
\***        PPFK.BOOTS.CODE$      - 7-digit boots item code           7 ASC
\***        PPFK.PCR.COUNT%       - Number of price change records    2 INT
\***
\******************************************************************************
FUNCTION DELETE.PPFK PUBLIC

    INTEGER*1   DELETE.PPFK
    INTEGER*1   RC%
    INTEGER*1   PPFK.PCRS.USED%
    INTEGER*2   I%
    INTEGER*2   PPFK.RECORD.COUNT%
    INTEGER*2   PPFK.PCR.INDEX%

    DELETE.PPFK = 1

    ! Strip check digit and pack item code
    WORK.BOOTS.CODE$ = PACK$("0" + PPFK.BOOTS.CODE$)

    ! Calculate the number of fully populated sequence records present
    PPFK.RECORD.COUNT% = INT(PPFK.PCR.COUNT.READ% / PPFK.PCR.PER.RECORD%) ! 1.2 RC (53)

    ! Calculate how many used slots we have in the partially populated sequence record
    PPFK.PCRS.USED% = MOD(PPFK.PCR.COUNT.READ%, PPFK.PCR.PER.RECORD%)     ! 1.2 RC (53)

    ! Calculate whether we have an extra partially full sequence record
    IF PPFK.PCRS.USED% > 0 THEN BEGIN

        ! Include the partially populated sequence record in the total
        PPFK.RECORD.COUNT% = PPFK.RECORD.COUNT% + 1

    ENDIF
    
!   Redundant (never executed) lines deleted                              ! 1.2 RC (53)
    
    ! FOR each required PPFK sequence record
        FOR I% = PPFK.RECORD.COUNT% TO 1 STEP -1 
    
            ! Build key field for PPFK record
            PPFK.KEY$ = WORK.BOOTS.CODE$ + PACK$(RIGHT$("00" + STR$(I%),2))
    
            ! Delete PPFK sequence record
            RC% = DELETE.PPFK.SEQ
    
            ! IF PPFK sequence record delete was unsuccessful
            IF RC% <> 0 THEN BEGIN
                EXIT FUNCTION
            ENDIF
    
        NEXT I%

    DELETE.PPFK = 0

END FUNCTION

