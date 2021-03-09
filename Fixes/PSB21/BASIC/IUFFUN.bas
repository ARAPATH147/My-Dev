\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR ITEM UPDATE FILES PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   IUFFUN (BAS)
\***
\***        FILE TYPE   :   Sequential
\***
\***
\***    Version 1.6           Charles Skadorwa / Mark Goode         28th Novemeber 2011
\***    CORE Heritage Stores Release 2 (Outbound) Project.
\***    Functions rewritten.
\***
\***    Version 1.7           Mark Goode                            22nd December 2011
\***    Calling application to validate RPD date length
\***
\***    Version 1.8           Rob Cowey                             18th January 2012
\***    Fix to GETN1 function to prevent privilege exception when the passed string is
\***    NULL.
\***
\***    Version 1.9          Tittoo Thomas                          19th January 2012
\***    Fixed for the local opening and processing of the BCF file without
\***    affecting the mainline program.
\***
\***    Old format IUF.RPD.DATE$ extended to 8-bytes by prefixing with century ie. "20".
\***
\***    Defect 2551: CIP Markdown processing fails for new IUF format.
\***    New format IUF.MARKDOWN$ flag overridden to "N" (same as old IUF setting) as
\***    it is not currently used - the code references IDF INDICAT1% Bit6 CIP Markdown
\***    flag.
\***
\***    VERSION 1.10.                ROBERT COWEY.                07 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.5.
\***    Various corrections relating to the transformation of legacy IUF
\***    data into Core R2 format
\***
\***    Defect 19 - Commented 1.10 RC (19).
\***    Corrected transformation of IUF.LOYALTY.FLAG$ to IUF.EARN.POINTS$
\***    and IUF.REDEEMABLE$.
\***
\***    Defect 21 - Commented 1.10 RC (21).
\***    Original fix (and associated comments) were incorrect as they were    ! 1.11 RC (21)
\***    based on a mis-understanding of variable IUF.DISCOUNTABLE$.           ! 1.11 RC (21)
\***    The fix has now been re-coded (commented 1.11 RC (21) ) and any       ! 1.11 RC (21)
\***    potentially confusing comments also corrected or removed.             ! 1.11 RC (21)
\***
\***    Defect 39 - Commented 1.10 RC (39).
\***    Set IUF.GUARANTEE.LENGTH$ to "25" for IUF.GUARANTEE.CAT$ = "Z".
\***    Set IUF.GUARANTEE.LENGTH$ to "63" for IUF.GUARANTEE.CAT$ = "G".
\***
\***    Defect 40 - Commented 1.10 RC (40).
\***    Converted IUF.GIVEAWAY$ of "C" to "Y" otherwise set to "N".
\***
\***    Defect 46 - Commented 1.10 RC (46).
\***    Converted IUF.GRP.CODE.FLAG$ of "G" to "Y" otherwise set to "N".
\***
\***    Defect 47 - Commented 1.10 RC (47).
\***    Converted IUF.ENF.PRICE.ENTRY$ to "N" when not "Y".
\***
\***    VERSION 1.11.                ROBERT COWEY.                20 FEB 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.6.
\***    Further correction relating to the transformation of legacy IUF
\***    data into Core R2 format
\***
\***    Defect 44 - Commented 1.11 RC (44).
\***    Modified LEGACY.IUF.FORMAT to set INSURE.CONCEPT.GROUPS$ from
\***    BCF record 2 containing list of insurance concept groups.
\***    Modified READ.IUF to search this using IUF items concept group
\***    to identify insurance related item.
\***
\***    Defect 21 - Commented 1.11 RC (21).
\***    For legacy IUF ...
\***    Set IUF.DISCOUNTABLE$ to "N" when IUF.DISCOUNT.CAT$ is "X"
\***    (or default it to "Y" otherwise).
\***    For Core R2 format IUF ...
\***    Check IUF.DISCOUNTABLE$ is "Y" (or default it to "N" otherwise).
\***
\***    Defect 71 - Commented 1.11 RC (71)                        24 FEB 2012.
\***    For Core R2 IUF ...
\***    Set IUF.UNRESTRICTED.GROUP.CODE$ to a default value of "N" when it
\***    contains a value other than "N" or "Y" (eg, null, blank, invalid).
\***
\***    Defect 71 - Reversed out                                  27 FEB 2012.
\***    The previous fix correctly follows Detailed Design 1.6 however the
\***    DD refers to setting of IRF flags from valid IUF data ("N" or "Y").
\***    The project level documents however require that invalid IUF data
\***    for this variable causes the item to be rejected (by PSB2100.BAS).
\***
\***    VERSION 1.12.                ROBERT COWEY.                13 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.8.
\***
\***    Defect 103 - Commented 1.12 RC (103).
\***    When processing REFPGF data from REFPGF.RECORDS$ table ...
\***    Cater for non-Dispose return route of blank (ie, special instructions).
\***
\***    VERSION 1.13.                ROBERT COWEY.                20 MAR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.9.
\***
\***    Defect 129 - Commented 1.13 RC (129)
\***    Removed "YN" translation of Core R2 format IUF.EARN.POINTS$ flag.
\***    Code now causes Core R2 IUF.EARN.POINTS$ flag "Y" to set IRF.INDICAT3%
\***    Exclude-from-Loyalty flag X'08' to OFF.
\***    This correction should have been made as part of defect 19 work.
\***
\***    VERSION 1.14.                ROBERT COWEY.                26 APR 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.14.
\***
\***    Defect 208 - Commented 1.14 RC (208)
\***    Corrected IF test setting IUF.RETURN.ROUTE$ to uniquely identify
\***    each REFPGF Return Label Type (incl "Semi-Centralised").
\***
\***    VERSION 1.15.               ROBERT COWEY.                16 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.17.
\***
\***    Defect 230 - Commented 1.15 RC (230)
\***    Modified READ.IUF to bypass redundant type 002 Flashpack records
\***    and adjusted IUF.RECORD.COUNT% normally incremented within PSB21.
\***
\***    VERSION B               Rejiya Nair             13th May 2016
\***    PRJ1452 Restricting Item Sales
\***    - Check for the pattern; (*R##) or (*R#) in IUF.STNDRD.DESC$;
\***      where # refers to group number. This means that; the group
\***      number can be from 01 - 99 or 1 to 99. If the pattern is found
\***      then;
\***       . Set the new variable, IUF.RESTRICT.SALES.FLAG$ to "Y".
\***       . Assign the group number of this item in to IUF.GRP.NO$.
\***    - Removed the commented out code
\***
\***--------------------------------------------------------------------------------
\***  IMPORTANT: When you change PSB21, please search for the line containing
\***------------        CALL DO.MESSAGE("PSB21 PROGRAM START
\***             in Module PSB2100 and wind the date and version on.
\***
\*******************************************************************************
\*******************************************************************************

\*******************************************************************************
\***    Function globals
\*******************************************************************************
%INCLUDE PSBF20G.J86    ! Sess Num Utility

%INCLUDE IUFDEC.J86 ! IUF variable declarations
%INCLUDE BCFDEC.J86 ! BCF File Functions

STRING GLOBAL                                \
    CURRENT.CODE$,                           \
    FILE.OPERATION$,                         \
    REFPGF.FILE.NAME$                        !

INTEGER*1 GLOBAL                             \
    TRUE,                                    \
    FALSE,                                   \
    DO.PGF.FILE,                             \
    FINISHED                                 !

INTEGER*2 GLOBAL                             \
    CURRENT.REPORT.NUM%

INTEGER*4 GLOBAL \                           ! 1.15 RC (230)
    COUNT.RECORDS.IUF%                       ! 1.15 RC (230)

INTEGER*2                                    \
    CGRP.NUM%,                               \
    PGRP.NUM%,                               \
    NO.OF.BCF.19.FIELDS%,                    \
    VALUE1%,                                 \
    VALUE2%                                  !

STRING                                       \
    BCF.RECORD19$,                           \
    BCF.RECORD19.INDICATOR$(1),              \
    BCF.RECORD19.FIELD$,                     \
    RESALEABLE$,                             \
    RETURN.ROUTE$,                           \
    SPECIAL.INSTRUCTION$,                    \
    RETURN.LABEL.TYPE$,                      \
    TEMP$                                    !



\*******************************************************************
\***    External functions
\********************************************************************
%INCLUDE PSBF20E.J86        ! SESSION NUMBER UTILITY
%INCLUDE PSBF30E.J86        ! KEYED FILE READER

%INCLUDE BCFEXT.J86         ! BCF File Functions


\*****************************************************************************
\***
\***    Included code defining external IBM functions / subroutines
\***
\***..........................................................................

    %INCLUDE EALHSASC.J86 ! External assembler function definitions
                          ! Includes all functions defined by EALGAADF.J86


\*************************************************************************
\***
\***    GETN1 - Extract 1 byte integer from string
\***
\*************************************************************************

FUNCTION GETN1(DATA$, POS%) PUBLIC

    STRING DATA$
    INTEGER*2 GETN1
    INTEGER*4 POS% ! Offset of required byte within DATA$              ! 1.8 RC

!   Set GETN1 to zero when DATA$ is not long enough compared to POS%   ! 1.8 RC
!   and so prevent privilege exception error                           ! 1.8 RC
    IF LEN(DATA$) < POS% + 1 THEN BEGIN                                ! 1.8 RC
        GETN1 = 0                                                      ! 1.8 RC
        EXIT FUNCTION                                                  ! 1.8 RC
    ENDIF                                                              ! 1.8 RC

    GETN1 = PEEK(SADD(DATA$) + POS% + 2)

END FUNCTION


\*****************************************************************************
\***
\***           Subprogram : LOAD.RECORDS.INTO.MEDICINES.TABLE
\***
\*****************************************************************************

SUB LOAD.RECORDS.INTO.MEDICINES.TABLE

    ALL.MEDICINES$ = ASPIRIN.RECORD$ + "," + PARACETAMOL.RECORD$ +        \
                                       "," + IBRUPROFEN.RECORD$  + " "
    NO.OF.MEDICINES%    = 0
    MATCH.COMMA%        = 1
    MEDICINES.IN.TABLE% = 1

    WHILE MATCH.COMMA% <> 0
          MATCH.COMMA% = MATCH(",", ALL.MEDICINES$, MATCH.COMMA% +1)
          NO.OF.MEDICINES% = NO.OF.MEDICINES% + 1
    WEND

    DIM MEDICINES.TABLE$(NO.OF.MEDICINES%)

    MATCH.POS1% = 1
    FOR A% = 1 TO NO.OF.MEDICINES%

        IF MATCH.POS1% = 1 THEN BEGIN
            MATCH.POS2% = MATCH(",",ALL.MEDICINES$,MATCH.POS1%)
            IF MATCH.POS2% THEN BEGIN
               DOSAGE$ = LEFT$(ALL.MEDICINES$,MATCH.POS2% - 1)
               MATCH.POS1% = MATCH.POS2% + 1
            ENDIF ELSE BEGIN
               MATCH.POS1% = MATCH(" ", ALL.MEDICINES$, 1)
               DOSAGE$ = LEFT$(ALL.MEDICINES$, MATCH.POS1% - 1)
            ENDIF
        ENDIF ELSE BEGIN
            MATCH.POS2% = MATCH(",",ALL.MEDICINES$,MATCH.POS1%)
            IF MATCH.POS2% <> 0 THEN BEGIN
               DOSAGE$ = MID$(ALL.MEDICINES$, MATCH.POS1%,  \
                      (MATCH.POS2% - MATCH.POS1%))
               MATCH.POS1% = MATCH.POS2% + 1
            ENDIF ELSE BEGIN
               DOSAGE$ = RIGHT$(ALL.MEDICINES$,(LEN(ALL.MEDICINES$) \
                      - MATCH.POS1%) + 1)
               MATCH.POS1% = MATCH(" ", DOSAGE$, 1)
               DOSAGE$ = LEFT$(DOSAGE$, MATCH.POS1% - 1)
            ENDIF
        ENDIF

        ALREADY.IN.TABLE$ = "N"
        FOR ADD.TO.TABLE% = 1 TO NO.OF.MEDICINES%
            IF MATCH(DOSAGE$,MEDICINES.TABLE$(ADD.TO.TABLE%),1) THEN BEGIN
               ALREADY.IN.TABLE$ = "Y"
            ENDIF
        NEXT ADD.TO.TABLE%

        IF ALREADY.IN.TABLE$ = "N" THEN BEGIN
            MEDICINES.TABLE$(MEDICINES.IN.TABLE%) = DOSAGE$
            MEDICINES.IN.TABLE% = MEDICINES.IN.TABLE% + 1
        ENDIF

    NEXT A%

    MEDICINES.IN.TABLE% = MEDICINES.IN.TABLE% -1

END SUB

\*****************************************************************************
\***
\***           Subprogram : LEGACY.IUF.FORMAT
\***
\*****************************************************************************


SUB LEGACY.IUF.FORMAT

    INTEGER*1 FALSE, TRUE
    INTEGER*2 BCF.SAVED.SESS.NUM%                                       ! 1.9 TT
    INTEGER*4 RC%

    ! If global 'IUF.LEGACY.SUPPORT' is ON then extract details from BCF
    CALL BCF.SET
    !----------------------------------------------------------------
    ! Save off global session number since BCF needs to be opened and
    ! read locally.
    !----------------------------------------------------------------
    BCF.SAVED.SESS.NUM% = BCF.SESS.NUM%                                 ! 1.9 TT
    CALL SESS.NUM.UTILITY("O", BCF.REPORT.NUM%, BCF.FILE.NAME$)
    BCF.SESS.NUM% = F20.INTEGER.FILE.NO%


    !Open, read and close the BRCF to obtain:
    FILE.OPERATION$ = "O"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    IF END # BCF.SESS.NUM% THEN EXIT.SUB
    OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM% NODEL

    FILE.OPERATION$ = "R"

!---------------------------------------------------------  ! 1.11 (44)
!   Read BCF record 2 for list of Insurance Concept Groups  ! 1.11 (44)
!---------------------------------------------------------  ! 1.11 (44)
    BCF.REC.NO% = 2                                         ! 1.11 (44)
    RC% = READ.BCF                                          ! 1.11 (44)

    IF RC% <> 0 THEN BEGIN                                  ! 1.11 (44)
        EXIT SUB                                            ! 1.11 (44)
    ENDIF                                                   ! 1.11 (44)

    INSURE.CONCEPT.GROUPS$ = BCF.RECORD$                    ! 1.11 (44)

    !-----------------------------------------------------
    ! The list of Age Restriction patterns from record 3
    ! Currently set to (12),(15),(16),(18),(21)
    !-----------------------------------------------------
    BCF.REC.NO% = 3
    RC% = READ.BCF

    IF RC% <> 0 THEN BEGIN
        EXIT SUB
    ENDIF

    AGE.POS% = MATCH(":",BCF.RECORD$,1)
    AGE.RESTRICT$ = MID$(BCF.RECORD$,AGE.POS% + 1, LEN(BCF.RECORD$) - AGE.POS%)

    NO.OF.AGES% = 0
    IF LEFT$(AGE.RESTRICT$,1) <> " " THEN BEGIN

        MATCH.COMMA% = 1
        WHILE MATCH.COMMA% <> 0
            MATCH.COMMA% = MATCH(",", AGE.RESTRICT$, MATCH.COMMA% + 1)
            NO.OF.AGES% = NO.OF.AGES% + 1
        WEND

        DIM AGES$(NO.OF.AGES%)
    ENDIF ELSE BEGIN
         AGE.RESTRICT$ = "EMPTY"
    ENDIF

    IF NO.OF.AGES% > 0 THEN BEGIN

        MATCH.POS1% = 1

        FOR J% = 1 TO NO.OF.AGES%

            IF MATCH.POS1% = 1 THEN BEGIN
               MATCH.POS2% = MATCH(",",AGE.RESTRICT$,MATCH.POS1%)
               IF MATCH.POS2% THEN BEGIN
                  AGE$ = LEFT$(AGE.RESTRICT$,MATCH.POS2% - 1)
                  MATCH.POS1% = MATCH.POS2% + 1
               ENDIF ELSE BEGIN
                  MATCH.POS1% = MATCH(" ", AGE.RESTRICT$, 1)
                  AGE$ = LEFT$(AGE.RESTRICT$, MATCH.POS1% - 1)
               ENDIF
            ENDIF ELSE BEGIN
                MATCH.POS2% = MATCH(",",AGE.RESTRICT$,MATCH.POS1%)
                IF MATCH.POS2% <> 0 THEN BEGIN
                    AGE$ = MID$(AGE.RESTRICT$, MATCH.POS1%, (MATCH.POS2% - MATCH.POS1%))
                    MATCH.POS1% = MATCH.POS2% + 1
                ENDIF ELSE BEGIN
                    AGE$ = RIGHT$(AGE.RESTRICT$,(LEN(AGE.RESTRICT$) - MATCH.POS1%) + 1)
                    MATCH.POS1% = MATCH(" ", AGE$, 1)
                    AGE$ = LEFT$(AGE$, MATCH.POS1% - 1)
                ENDIF
            ENDIF
            AGES$(J%) = AGE$
        NEXT J%
    ENDIF

    !-----------------------------------------------------
    ! Extract data from BCF records 6, 7 & 8
    !-----------------------------------------------------
    FOR BCF.REC.NO% = 6 TO 8
        RC% = READ.BCF

        IF RC% <> 0 THEN BEGIN
            EXIT SUB
        ENDIF

        SPACE% = MATCH(" ",BCF.RECORD$,1)

        IF BCF.REC.NO% = 6 THEN BEGIN
            !-----------------------------------------------------
            ! The Aspirin indicators from record 6 -
            ! Currently set to (2),(4),(6),(7),(2E),(4E),(6E),(7E)
            !-----------------------------------------------------
            ASPIRIN.RECORD$ = LEFT$(BCF.RECORD$,SPACE% -1)

        ENDIF ELSE IF BCF.REC.NO% = 7 THEN BEGIN
            !-----------------------------------------------------
            ! The Paracetamol indicators from record 7. Currently
            ! set to (1),(4),(5),(7),(1E),(4E),(5E),(7E)
            !-----------------------------------------------------
            PARACETAMOL.RECORD$ = LEFT$(BCF.RECORD$,SPACE% -1)

        ENDIF ELSE IF BCF.REC.NO% = 8 THEN BEGIN
            !-----------------------------------------------------
            ! The Ibuprofen indicators from record 8
            ! Currently set to (3),(5),(6),(7),(3E),(5E) (6E),(7E)
            !-----------------------------------------------------
            IBRUPROFEN.RECORD$ = LEFT$(BCF.RECORD$,SPACE% -1)
        ENDIF

    NEXT BCF.REC.NO%

    CALL LOAD.RECORDS.INTO.MEDICINES.TABLE

    !-----------------------------------------------------
    ! The list of Alcohol product groups from record 15
    !-----------------------------------------------------
    BCF.REC.NO% = 15
    RC% = READ.BCF

    IF RC% <> 0 THEN BEGIN
        EXIT SUB
    ENDIF

    PROD.GRP.NUM% = 1

    WHILE PROD.GRP.NUM% LE 16 ! Maximum 16 product groups on BCF record

        PROD.GRP$ = MID$(BCF.RECORD$, 5*PROD.GRP.NUM% -4 , 5)

        IF PROD.GRP$ <> "     " THEN BEGIN ! Product group position in
                                           ! use so append data to list
            ALCOHOL.PROD.GRP.LIST$ = ALCOHOL.PROD.GRP.LIST$ + \
                                     PROD.GRP$ + ","
            PROD.GRP$ = STR$(VAL(PROD.GRP$)) ! Error if not numeric
        ENDIF

        PROD.GRP.NUM% = PROD.GRP.NUM% + 1

    WEND

    !---------------------------------------------------------
    ! Ephedrine and Pseudoephedrine indicators from record 19
    !---------------------------------------------------------
    BCF.REC.NO% = 19
    RC% = READ.BCF
    IF RC% <> 0 THEN BEGIN
        EXIT SUB
    ENDIF ELSE BEGIN
        BCF.RECORD19$ = BCF.RECORD$
    ENDIF

    MATCH.COMMA% = 1
    WHILE MATCH.COMMA% <> 0
        MATCH.COMMA% = MATCH(",", BCF.RECORD19$, MATCH.COMMA% + 1)
        NO.OF.BCF.19.FIELDS% = NO.OF.BCF.19.FIELDS% + 1
    WEND

    DIM BCF.RECORD19.INDICATOR$(NO.OF.BCF.19.FIELDS%)

    MATCH.POS1% = 1

    FOR J% = 1 TO NO.OF.BCF.19.FIELDS%

         IF MATCH.POS1% = 1 THEN BEGIN
               MATCH.POS2% = MATCH(",",BCF.RECORD19$,MATCH.POS1%)
               IF MATCH.POS2% THEN BEGIN
                  BCF.RECORD19.FIELD$ = LEFT$(BCF.RECORD19$,MATCH.POS2% - 1)
                  MATCH.POS1% = MATCH.POS2% + 1
               ENDIF ELSE BEGIN
                  MATCH.POS1% = MATCH(" ", BCF.RECORD19$, 1)
                  BCF.RECORD19.FIELD$ = LEFT$(BCF.RECORD19$, MATCH.POS1% - 1)
               ENDIF
            ENDIF ELSE BEGIN
                MATCH.POS2% = MATCH(",",BCF.RECORD19$,MATCH.POS1%)
                IF MATCH.POS2% <> 0 THEN BEGIN
                    BCF.RECORD19.FIELD$ = MID$(BCF.RECORD19$, MATCH.POS1%, (MATCH.POS2% - MATCH.POS1%))
                    MATCH.POS1% = MATCH.POS2% + 1
                ENDIF ELSE BEGIN
                    BCF.RECORD19.FIELD$ = RIGHT$(BCF.RECORD19$,(LEN(BCF.RECORD19$) - MATCH.POS1%) + 1)
                    MATCH.POS1% = MATCH(" ", BCF.RECORD19.FIELD$, 1)
                    BCF.RECORD19.FIELD$ = LEFT$(BCF.RECORD19.FIELD$, MATCH.POS1% - 1)
                ENDIF
            ENDIF
            BCF.RECORD19.INDICATOR$(J%) = BCF.RECORD19.FIELD$

    NEXT

    CLOSE BCF.SESS.NUM%
    BCF.SESS.NUM% = BCF.SAVED.SESS.NUM%                                 ! 1.9 TT

    !-------------------------------
    ! Load records into REFPGF table
    !-------------------------------
    REFPGF.FILE.NAME$  = "REFPGF"    ! No .set as previously only
    REFPGF.REPORT.NUM% = 789         ! used by the Till.

    DIM REFPGF.RECORDS$(100) ! Each record holds the Prod Group details
                             ! associated to a concept group(0-99)
    REFPGF.COUNT% = 0
    DO.PGF.FILE = FALSE

    RC% = PROCESS.KEYED.FILE(REFPGF.FILE.NAME$,REFPGF.REPORT.NUM%,"N")

    IF RC% <> 0 THEN BEGIN
       EXIT SUB
    ENDIF

 EXIT.SUB:

END SUB

\*******************************************************************
\***    IUF File functions
\*******************************************************************


!==================================================================================
!
!  IUF.SET
!
!==================================================================================
FUNCTION IUF.SET PUBLIC

    IUF.FILE.NAME$  = "IUF"
    IUF.REPORT.NUM% =  15
    IUF.NEW.FORMAT  = -1

END FUNCTION

!==================================================================================
!
!  VALIDATE.HEADER
!
!==================================================================================


FUNCTION VALIDATE.IUF.HEADER.RECORD PUBLIC

    STRING TYPE$
    STRING NUMBER$
    STRING FLAG$
    STRING TIME.STAMP$
    STRING MATRIX$(1)
    STRING VALIDATE.IUF.HEADER.RECORD

    VALIDATE.IUF.HEADER.RECORD = "ACK"

    !---------------------------------------------------------
    ! Open the IUF, read the first record, then close the file
    !---------------------------------------------------------
    CALL SESS.NUM.UTILITY("O", IUF.REPORT.NUM%, IUF.FILE.NAME$)
    IUF.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF END # IUF.SESS.NUM% THEN EXIT.FUNCTION
    OPEN IUF.FILE.NAME$ AS IUF.SESS.NUM%      \
         BUFFSIZE 32256 LOCKED NOWRITE NODEL

    FILE.OPERATION$ = "R"

    IF END # IUF.SESS.NUM% THEN EXIT.FUNCTION
    READ # IUF.SESS.NUM%; IUF.RECORD$

    CLOSE IUF.SESS.NUM%

    !-----------------------------------------------------
    ! Check if the record is a header in old or new format
    ! also validate each field.
    !-----------------------------------------------------
    ! IF we have a new format IUF
    IF LEFT$(UCASE$(IUF.RECORD$),1) = "H" THEN BEGIN

        DIM MATRIX$(4)

        ! Open file as and read the header record
        IF END # IUF.SESS.NUM% THEN EXIT.FUNCTION
        OPEN IUF.FILE.NAME$ AS IUF.SESS.NUM% NOWRITE NODEL

        IF END # IUF.SESS.NUM% THEN EXIT.FUNCTION
        READ MATRIX #IUF.SESS.NUM%; MATRIX$(1), 4

        CLOSE IUF.SESS.NUM%

        IF MATCH("!",MATRIX$(2),1) OR LEN(MATRIX$(2)) <> 4 THEN BEGIN
            VALIDATE.IUF.HEADER.RECORD = "INVALID STORE NUMBER"
            EXIT FUNCTION
        ENDIF ELSE IF NOT MATCH(MATRIX$(3),"NY",1) THEN BEGIN
            VALIDATE.IUF.HEADER.RECORD = "INVALID INITIAL LOAD FLAG"
            EXIT FUNCTION
        ENDIF ELSE IF MATCH("!",MATRIX$(4),1) OR LEN(MATRIX$(4)) <> 17 THEN BEGIN
            VALIDATE.IUF.HEADER.RECORD = "INVALID DATE TIME STAMP"
            EXIT FUNCTION
        ENDIF

        IUF.NEW.FORMAT = TRUE

       ! Check store number is valid. i.e. numeric
    ! ELSE IF we have an old format IUF
    ENDIF ELSE IF MID$(IUF.RECORD$,8,3) = "000" THEN BEGIN

        ! Check store number is valid. i.e. numeric
        IF MATCH("!",MID$(IUF.RECORD$,11,4),1) THEN BEGIN
            VALIDATE.IUF.HEADER.RECORD = "INVALID STORE NUMBER"
            EXIT FUNCTION

        ! Check serial number is valid. i.e. numeric
        ENDIF ELSE IF MATCH("!",MID$(IUF.RECORD$,15,5),1) THEN BEGIN

             VALIDATE.IUF.HEADER.RECORD = "INVALID SERIAL NUMBER"
             EXIT FUNCTION

        ENDIF ELSE BEGIN

            IUF.NEW.FORMAT = FALSE
            !Set up for legacy IUF format
            CALL LEGACY.IUF.FORMAT

        ENDIF

    ENDIF ELSE BEGIN
         ! ELSE IF the IUF format is unknown
         ! Return unknow iuf format
         VALIDATE.IUF.HEADER.RECORD = "UNKNOWN IUF FORMAT"
         EXIT FUNCTION
    ENDIF

         EXIT FUNCTION

EXIT.FUNCTION:

    VALIDATE.IUF.HEADER.RECORD = "UNABLE TO OPEN / READ IUF"

END FUNCTION


!==================================================================================
!
!  READ.IUF
!
!==================================================================================
FUNCTION READ.IUF PUBLIC

    INTEGER*2 READ.IUF
    READ.IUF = 1

    IUF.REC.TYPE$  = ""

    IF NOT IUF.NEW.FORMAT THEN BEGIN
        !---------------------------------------
        ! Old IUF Format - Convert to new format
        !---------------------------------------
        CURRENT.CODE$ = MID$(IUF.RECORD$,1,10)

        IF END # IUF.SESS.NUM% THEN READ.IUF.IF.END
        READ # IUF.SESS.NUM%; IUF.RECORD$

        IUF.BOOTS.CODE$ = MID$(IUF.RECORD$,1,7)
        IUF.TRANS.TYPE$ = MID$(IUF.RECORD$,8,3)

        WHILE IUF.TRANS.TYPE$ = "002" ! Bypass redundant       ! 1.15 RC (230)
                                      ! flashpack record       ! 1.15 RC (230)
            READ # IUF.SESS.NUM%; IUF.RECORD$                  ! 1.15 RC (230)
            IUF.BOOTS.CODE$ = MID$(IUF.RECORD$,1,7)            ! 1.15 RC (230)
            IUF.TRANS.TYPE$ = MID$(IUF.RECORD$,8,3)            ! 1.15 RC (230)
!           Adjust PSB21 trailer count                         ! 1.15 RC (230)
            COUNT.RECORDS.IUF% = COUNT.RECORDS.IUF% + 1        ! 1.15 RC (230)
        WEND                                                   ! 1.15 RC (230)

        IF IUF.TRANS.TYPE$ = "000" THEN BEGIN  ! Header record

            IUF.REC.TYPE$     = "H"
            IUF.BRANCH.NO$    = MID$(IUF.RECORD$,11,4)
            IUF.STORE.NUM$    = IUF.BRANCH.NO$
            IUF.SERIAL.NO$    = MID$(IUF.RECORD$,15,5)
            IUF.INITIAL.LOAD$ = "N"
            IUF.TIME.STAMP$   = "20" + DATE$ + TIME$ + "000"

        ENDIF ELSE IF IUF.TRANS.TYPE$ = "001" THEN BEGIN ! Item reference record

            IUF.REC.TYPE$      = "I"
            IUF.GRP.CODE.FLAG$ = MID$(IUF.RECORD$,11,1)
            IF IUF.GRP.CODE.FLAG$ = "G" THEN BEGIN                            ! 1.10 RC (46)
                IUF.GRP.CODE.FLAG$ = "Y"                                      ! 1.10 RC (46)
            ENDIF ELSE BEGIN                                                  ! 1.10 RC (46)
                IUF.GRP.CODE.FLAG$ = "N"                                      ! 1.10 RC (46)
            ENDIF
            IUF.STNDRD.DESC$   = MID$(IUF.RECORD$,12,24)
            IUF.TILL.DESC$     = MID$(IUF.RECORD$,36,18)
            IUF.S.E.DESC$      = MID$(IUF.RECORD$,54,45)
            IUF.SUPPLY.ROUTE$  = MID$(IUF.RECORD$,99,1)
            IUF.GIVEAWAY$      = MID$(IUF.RECORD$,100,1)

            IF IUF.GIVEAWAY$ = "C" THEN BEGIN                                 ! 1.10 RC (40)
                IUF.GIVEAWAY$ = "Y"                                           ! 1.10 RC (40)
            ENDIF ELSE BEGIN                                                  ! 1.10 RC (40)
                IUF.GIVEAWAY$ = "N"
            ENDIF
            IUF.PROD.GRP$      = MID$(IUF.RECORD$,101,5)
            IUF.GUARANTEE.CAT$ = MID$(IUF.RECORD$,106,1)

            IF   IUF.GUARANTEE.CAT$ = "L" \                                   ! 1.10 RC (39)
              OR IUF.GUARANTEE.CAT$ = "Z" THEN BEGIN                          ! 1.10 RC (39)
                IUF.GUARANTEE.LENGTH$  =  "25"
            ENDIF ELSE BEGIN                                                  ! 1.10 RC (39)
                IF IUF.GUARANTEE.CAT$ = "G" THEN BEGIN ! Gift Experience      ! 1.10 RC (39)
                    IUF.GUARANTEE.LENGTH$  =  "63"                            ! 1.10 RC (39)
                ENDIF ELSE BEGIN
                    IUF.GUARANTEE.LENGTH$  =  STR$(MATCH(IUF.GUARANTEE.CAT$,"ABCDFHIJKMOPQRSTUVWX",1)) ! 1.10 RC (39)
                ENDIF
            ENDIF                                                             ! 1.10 RC (39)

            IUF.ENF.PRICE.ENTRY$ = MID$(IUF.RECORD$,107,1)

            IF IUF.ENF.PRICE.ENTRY$ <> "Y" THEN BEGIN                         ! 1.10 RC (47)
                IUF.ENF.PRICE.ENTRY$ = "N"                                    ! 1.10 RC (47)
            ENDIF                                                             ! 1.10 RC (47)

            IUF.LOYALTY.FLAG$    = MID$(IUF.RECORD$,108,1)
            IUF.EARN.POINTS$     = "Y"                                        ! 1.10 RC (19)
            IUF.REDEEMABLE$      = "N"

            IF   IUF.LOYALTY.FLAG$ = "R" \                                    ! 1.10 RC (19)
              OR IUF.LOYALTY.FLAG$ = "S" THEN BEGIN                           ! 1.10 RC (19)
                IUF.REDEEMABLE$ = "Y"                                         ! 1.10 RC (19)
            ENDIF                                                             ! 1.10 RC (19)

            IF IUF.LOYALTY.FLAG$ = "E" THEN BEGIN                             ! 1.10 RC (19)
                IUF.EARN.POINTS$ = "N"                                        ! 1.10 RC (19)
            ENDIF                                                             ! 1.10 RC (19)

            IF IUF.LOYALTY.FLAG$ = "X" THEN BEGIN                             ! 1.10 RC (19)
                IUF.REDEEMABLE$ = "Y"                                         ! 1.10 RC (19)
                IUF.EARN.POINTS$ = "N"                                        ! 1.10 RC (19)
            ENDIF                                                             ! 1.10 RC (19)

            IUF.DISCOUNT.CAT$      =  MID$(IUF.RECORD$,109,1)

            IUF.DISCOUNTABLE$ = "Y" ! Default - equates to IRF.INDICAT3% X'01' OFF              ! 1.11 RC (21)

            IF IUF.DISCOUNT.CAT$ = "X" THEN BEGIN ! Exempt from discount                        ! 1.11 RC (21)
                IUF.DISCOUNTABLE$ = "N" ! Equates to IRF.INDICAT3% X'01' ON                     ! 1.11 RC (21)
            ENDIF                                                                               ! 1.10 RC (21)

            IUF.OWN.BRAND$         = MID$(IUF.RECORD$,110,1)
            IUF.DIRECT.PROC$       = MID$(IUF.RECORD$,111,1)
            IUF.STATUS.1$          = MID$(IUF.RECORD$,112,1)
            IUF.EANS.CHANGED$      = MID$(IUF.RECORD$,113,1)
            IUF.NO.OF.EANS$        = MID$(IUF.RECORD$,114,3)
            IUF.CURRENT.PRICE$     = MID$(IUF.RECORD$,117,8)
            IUF.NO.OF.RPD.PRICES$  = MID$(IUF.RECORD$,125,1)
            IUF.STOCK.SYSTEM.FLAG$ = MID$(IUF.RECORD$,136,1)
            IUF.NEW.DEAL.COUNT$    = MID$(IUF.RECORD$,137,2)
            IUF.ETHICAL.CLASS$     = ""

            IF MATCH("(P)",IUF.STNDRD.DESC$,1) THEN BEGIN
                IUF.ETHICAL.CLASS$  =  "P"
            ENDIF

            IUF.ETHICAL.DESCRIPTION$   =  ""
            IUF.ETHICAL.ACTIVE$        =  "N"
            IUF.ETHICAL.PACK.SIZE$     =  "0000000"
            IUF.PRIMARY.SUPPLIER$      =  "00000000"
            IUF.CONTAINS.NONSOLID.PAINKILLER$  =  "N"

            IF MATCH("(#E)",IUF.STNDRD.DESC$,1) THEN BEGIN
                IUF.CONTAINS.NONSOLID.PAINKILLER$  =  "Y"
            ENDIF

            IUF.BC.LETTER$        = MID$(IUF.RECORD$,139,1)
            IUF.ITEM.QTY$         = MID$(IUF.RECORD$,140,7)
            IUF.UNIT.MEASUREMENT$ = MID$(IUF.RECORD$,147,4)
            IUF.UNIT.NAME$        = MID$(IUF.RECORD$,151,10)

            IUF.RESTRICT.SALES.FLAG$  =  "N"                            !BRN

            MATCH.POS1% = MATCH("(*R##)",IUF.STNDRD.DESC$,1)            !BRN
            MATCH.POS2% = MATCH("(*R#)",IUF.STNDRD.DESC$,1)             !BRN

            !Check for the match of (*R##) where # refers to a number   !BRN
            IF (MATCH.POS1% <> 0) OR (MATCH.POS2% <> 0) THEN BEGIN      !BRN

                IUF.RESTRICT.SALES.FLAG$  =  "Y"                        !BRN

                IF MATCH.POS1% <> 0 THEN BEGIN                          !BRN
                    !Get the group number from the patten; (*R##)       !BRN
                    IUF.GRP.NO$ = MID$(IUF.STNDRD.DESC$,               \!BRN
                                                (MATCH.POS1% + 3),2)    !BRN

                ENDIF ELSE IF MATCH.POS2% <> 0 THEN BEGIN               !BRN
                    !Get the group number from the patten; (*R#)        !BRN
                    IUF.GRP.NO$ = MID$(IUF.STNDRD.DESC$,               \!BRN
                                                (MATCH.POS2% + 3),1)    !BRN
                ENDIF                                                   !BRN

            ENDIF                                                       !BRN

            ! The following code fixes a mainframe feature that results in the pound
            ! and dollar characters being transposed.
            IUF.STNDRD.DESC$ = TRANSLATE$ (IUF.STNDRD.DESC$, "$", "œ")
            IUF.TILL.DESC$   = TRANSLATE$ (IUF.TILL.DESC$,   "$", "œ")
            IUF.S.E.DESC$    = TRANSLATE$ (IUF.S.E.DESC$,    "$", "œ")

            !-----------------------------------------------------------
            !Lookup Product Group in new REFPGF table and set values
            !-----------------------------------------------------------
            CGRP.NUM% = VAL(LEFT$(IUF.PROD.GRP$,2)) !2 bytes concept grp
            PGRP.NUM% = VAL(RIGHT$(IUF.PROD.GRP$,3)) !3 bytes prodct grp

            IUF.RESALEABLE$   = "N"
            IUF.RETURN.ROUTE$ = ""
            IUF.RETURNABLE$   = "Y"                                            ! 1.8 RC
            IUF.SPECIAL.INSTRUCTION$ = "0"

            TEMP$ = REFPGF.RECORDS$(CGRP.NUM%)

            VALUE1% = GETN1( TEMP$, PGRP.NUM% * 2)
            VALUE2% = GETN1( TEMP$, PGRP.NUM% * 2 + 1)

            IF VALUE2% AND 40H THEN BEGIN
                IUF.RESALEABLE$   = "Y"
            ENDIF

                       IF VALUE1% = 0040h THEN BEGIN   ! 1.14 RC (208)
                IUF.RETURN.ROUTE$   = "R"
            ENDIF ELSE IF VALUE1% = 0080h THEN BEGIN   ! 1.14 RC (208)
                IUF.RETURN.ROUTE$   = "D"
            ENDIF ELSE IF VALUE1% = 00C0h THEN BEGIN   ! 1.14 RC (208)
                IUF.RETURN.ROUTE$   = "S"
            ENDIF ELSE IF VALUE1% = 0020h THEN BEGIN   ! 1.14 RC (208)
                IUF.RETURN.ROUTE$   = " "              ! 1.12 RC (103)
            ENDIF ELSE BEGIN
                IUF.RETURNABLE$ = "N"
            ENDIF

            IUF.SPECIAL.INSTRUCTION$ = STR$(VALUE2% AND 3FH)

            !----------------------------------------------------------------
            ! Check if product group is in the list of alcohol product groups
            !----------------------------------------------------------------
            IF MATCH(IUF.PROD.GRP$, ALCOHOL.PROD.GRP.LIST$, 1) <> 0 THEN BEGIN
                IUF.CONTAINS.ALCOHOL$ = "Y"
            ENDIF ELSE BEGIN
                IUF.CONTAINS.ALCOHOL$ = "N"
            ENDIF
            !-------------------------------------------------------------------------
            ! Check if one of the Medicine patterns appears in the new IDF description
            !-------------------------------------------------------------------------
            IUF.CONTAINS.ASPIRIN$     = "N"
            IUF.CONTAINS.PARACETAMOL$ = "N"
            IUF.CONTAINS.IBUPROFEN$   = "N"

            FOR A% = 1 TO MEDICINES.IN.TABLE%
                IF MATCH(MEDICINES.TABLE$(A%),IUF.STNDRD.DESC$,1) THEN BEGIN  ! Check if medicine
                   IF MATCH(MEDICINES.TABLE$(A%),ASPIRIN.RECORD$,1) THEN BEGIN
                       IUF.CONTAINS.ASPIRIN$ = "Y"                            ! Set Aspirin Flag
                   ENDIF
                   IF MATCH(MEDICINES.TABLE$(A%),PARACETAMOL.RECORD$,1) THEN BEGIN
                       IUF.CONTAINS.PARACETAMOL$ = "Y"                        ! Set Paracetamol Flag
                   ENDIF
                   IF MATCH(MEDICINES.TABLE$(A%),IBRUPROFEN.RECORD$,1) THEN BEGIN
                       IUF.CONTAINS.IBUPROFEN$ = "Y"                          ! Set Ibruprofen Flag
                   ENDIF
                ENDIF
            NEXT A%

            !---------------------------------------------------------------------------------------------
            ! Check if one of the Ephedrine or Pseudoephedrine patterns appears in the new IDF description
            !---------------------------------------------------------------------------------------------

                ! Ephedrine indicator
                IF MATCH(BCF.RECORD19.INDICATOR$(1),IUF.STNDRD.DESC$,1) THEN BEGIN
                    IUF.CONTAINS.EPHEDRINE$ = "Y"
                ENDIF ELSE BEGIN
                    IUF.CONTAINS.EPHEDRINE$ = "N"
                ENDIF

                ! Pseudoephedrine indicator
                IF MATCH(BCF.RECORD19.INDICATOR$(2),IUF.STNDRD.DESC$,1) THEN BEGIN
                    IUF.CONTAINS.PSEUDOEPHEDRINE$ = "Y"
                ENDIF ELSE BEGIN
                    IUF.CONTAINS.PSEUDOEPHEDRINE$ = "N"
                ENDIF

            !----------------------------------------------------------------------------------
            ! Check if one of the age patterns appears in the new IRF description & set the age
            !----------------------------------------------------------------------------------
            IUF.AGE.RESTRICTION$ = ""

            FINISHED = FALSE
            J% = 1

            WHILE NOT FINISHED

              IF MATCH(AGES$(J%),IUF.TILL.DESC$,1) THEN BEGIN ! Match against Till description

                    IUF.AGE.RESTRICTION$ = MID$(AGES$(J%),2,2)
                    FINISHED = TRUE

              ENDIF
              J% = J% + 1
              IF J% > NO.OF.AGES% THEN FINISHED = TRUE

            WEND

            IUF.GIFTCARD$                = "N"
            IUF.UNRESTRICTED.GROUP.CODE$ = "N"
            IUF.BOOTS.COM.EXTENDED$      = "N"
            IUF.DATE.SENSITIVE$          = "N"
            IUF.BLOCKED.FROM.SALE$       = ""

            !--------------------------------------------------------
            ! Check if item has an insurance related concept groups                        ! 1.11 RC (44)
            !--------------------------------------------------------
            IUF.INSURANCE$ = "N"

            IF MATCH(LEFT$(IUF.PROD.GRP$,2) + "000", INSURE.CONCEPT.GROUPS$, 1) THEN BEGIN ! 1.11 RC (44)
                IUF.INSURANCE$ = "Y"                                                       ! 1.11 RC (44)
            ENDIF                                                                          ! 1.11 RC (44)

            !---------------------------------------------------------
            ! Set OWN BRAND flag. Ignore Exclusive line flag.
            !---------------------------------------------------------
            IF IUF.OWN.BRAND$ = "B" THEN BEGIN
                IUF.OWN.BRAND$ = "Y"
            ENDIF ELSE BEGIN
                IUF.OWN.BRAND$ = "N"
            ENDIF

            !---------------------------------------------------------
            ! Force Direct Procedures B and C to be Direct Procedure A
            !---------------------------------------------------------
            ! IF direct supplied item
            IF NOT (IUF.DIRECT.PROC$ = " ") THEN BEGIN
               IUF.DIRECT.PROC$ = "A"
            ENDIF

            !------------------------------------------------------------
            ! Convert any Supply Status 'U' items to 'D', as 'U' will not
            ! exist as a supply status in future.
            !------------------------------------------------------------
            IF IUF.STATUS.1$ = "U" THEN BEGIN
                IUF.STATUS.1$ = "D"
            ENDIF

            !-----------------------------------
            ! Convert any CSR items to Warehouse
            !-----------------------------------OLD.BATCH.FOUND
            IF IUF.SUPPLY.ROUTE$ = "C" THEN BEGIN
                IUF.SUPPLY.ROUTE$ = "W"
            ENDIF

        ENDIF ELSE IF IUF.TRANS.TYPE$ = "004" THEN BEGIN ! Pending price record

            IUF.REC.TYPE$  = "P"
            IF LEN(IUF.RECORD$) = 29 THEN IUF.RECORD$ = IUF.RECORD$ + "N"
            IUF.RPD.DATE$  = "20" + MID$(IUF.RECORD$,11,6)                ! 1.9 TT
            IUF.NEW.PRICE$ = MID$(IUF.RECORD$,17,8)
            IUF.RPD.NO$    = MID$(IUF.RECORD$,25,5)
            IUF.MARKDOWN$  = "N"
        ENDIF ELSE IF IUF.TRANS.TYPE$ = "005" THEN BEGIN ! Barcode record

            IUF.REC.TYPE$   = "B"
            IUF.BAR.CODE$   = MID$(IUF.RECORD$,11,12) + "0"   ! Zero added.

        ENDIF ELSE IF IUF.TRANS.TYPE$ = "999" THEN BEGIN ! Trailer record

            IUF.REC.TYPE$   = "T"
            IUF.ITEM.COUNT$ = MID$(IUF.RECORD$,11,7)
            IUF.REC.COUNT$  = IUF.ITEM.COUNT$
        ENDIF ELSE BEGIN
            ! Unknown Record Type - default to '*'
            ! and let calling program handle it
            IUF.REC.TYPE$   = "*"
        ENDIF

    ENDIF ELSE BEGIN
        !------------------------------------------------------------
        !                      NEW IUF Format
        !------------------------------------------------------------
        DIM IUF.MATRIX$(200)

        IF END # IUF.SESS.NUM% THEN READ.IUF.IF.END
        READ MATRIX #IUF.SESS.NUM%; IUF.MATRIX$(1), 200

        IUF.REC.TYPE$ = IUF.MATRIX$(1)

        IF IUF.REC.TYPE$ = "I" THEN BEGIN
            !-----------------------------------------------------------------------------
            ! Process Item Record
            !
            !-----------------------------------------------------------------------------
            !1  IUF.REC.TYPE$          ASCII   1   "I"
            !2  IUF.BOOTS.CODE$        ASCII   7   Boots code including check digit
            !3  IUF.GRP.CODE.FLAG$     ASCII   1   Y/N
            !4  IUF.STNDRD.DESC$       ASCII   24  Report description
            !5  IUF.TILL.DESC$         ASCII   18
            !6  IUF.S.E.DESC$          ASCII   45  3 lines of 15 characters
            !7  IUF.SUPPLY.ROUTE$      ASCII   1   W - Warehouse  D - Direct   B - Direct via warehouse
            !8  IUF.GIVEAWAY$          ASCII   1   Y/N  Blank = N
            !9  IUF.PROD.GRP$          ASCII   5   ccsss
            !                                      cc  = Concept Group
            !                                      sss = Sequence number
            !10 IUF.GUARANTEE.LENGTH$  ASCII   3   range 0-63
            !                                       0  - No guarantee
            !                                       63 - Gift Experience
            !                                       Assumed unit is years, but can be overridden to months.
            !                                        E.g., 10M = 10 months
            !11 IUF.ENF.PRICE.ENTRY$   ASCII   1   Y/N
            !12 IUF.EARN.POINTS$       ASCII   1   Y/N  Y - Earns points etc
            !13 IUF.REDEEMABLE$        ASCII   1   Y/N  Y - Can be redeemed
            !14 IUF.DISCOUNTABLE$      ASCII   1   If on, item does attract staff discount
            !15 IUF.OWN.BRAND$         ASCII   1   Y - Boots  N - Brand
            !16 IUF.STATUS.1$          ASCII   1   Space - Active (default)
            !                                          B - Discontinued (on planner)
            !                                          C - Outstanding order cancelled (demised?)
            !                                          D - Discontinued (off planner)
            !                                          P - Suspended (demised?)
            !                                          U - Unsuppliable (demised)
            !                                          X - Flagged for deletion
            !                                          Z - Deleted
            !17 IUF.CURRENT.PRICE$     ASCII   8   In pence or cents
            !18 IUF.STOCK.SYSTEM.FLAG$ ASCII   1   Y/N  Y - EPOS maintains stock figure
            !19 IUF.BC.LETTER$         ASCII   1   e.g. H - Food & Baby
            !20 IUF.ITEM.QTY$          ASCII   7   Number of singles in the pack
            !21 IUF.UNIT.MEASUREMENT$  ASCII   5   eg. 100, if "per 100ml"
            !22 IUF.UNIT.NAME$         ASCII   10
            !23 IUF.DATE.SENSITIVE$    ASCII   1   Y/N
            !24 IUF.RETURNABLE$        ASCII   1   Y - Can be returned  N - Must be destroyed
            !25 IUF.RESALEABLE$        ASCII   1   Y/N
            !26 IUF.SPECIAL.INSTRUCTION$   ASCII   2   0-63
            !27 IUF.RETURN.ROUTE$      ASCII   1   NULL - No returns route
            !                                         R - Returns & Recovery
            !                                         S - Semi-centralised
            !                                         D - Direct
            !28 IUF.ETHICAL.CLASS$     ASCII   1   NULL - Retail
            !                                         G - General Sales Licence
            !                                         M - Pharmacy only med
            !                                         P - Pharmacy medicine
            !29 IUF.ETHICAL.DESCRIPTION$   Alphan  40  Ethical item description. Must be null if legal class is blank
            !30 IUF.ETHICAL.ACTIVE$        ASCII   1   N - NOT active in store   Y - Active in store (load into POD)
            !31 IUF.ETHICAL.PACK.SIZE$     ASCII   7   TODO-Will this always be the same as Unit pricing field?
            !32 IUF.PRIMARY.SUPPLIER$      ASCII   8
            !33 IUF.CONTAINS.ALCOHOL$      ASCII   1   Y/N
            !34 IUF.CONTAINS.PARACETAMOL$  ASCII   1   Y/N
            !35 IUF.CONTAINS.ASPIRIN$      ASCII   1   Y/N
            !36 IUF.CONTAINS.IBUPROFEN$    ASCII   1   Y/N
            !37 IUF.CONTAINS.EPHEDRINE$    ASCII   1   Y/N
            !38 IUF.CONTAINS.PSUEDOEPHEDRINE$     ASCII    1   Y/N
            !39 IUF.CONTAINS.NONSOLID.PAINKILLER$ ASCII    1   Y/N
            !40 IUF.UNRESTRICTED.GROUP.CODE$      ASCII    1   Y/N
            !41 IUF.GIFTCARD$                     ASCII    1   Y/N
            !42 IUF.AGE.RESTRICTION$              ASCII    2   BLANK, 12, 15, 16, 18, 21
            !43 IUF.BOOTS.COM.EXTENDED$           ASCII    1   Y/N
            !44 IUF.INSURANCE$                    ASCII    1   Y/N
            !45 IUF.BLOCKED.FROM.SALE$            ASCII    1   Blank - Not blocked
            !                                                      R - Blocked (recall)
            !                                                      W - Blocked (withdrawn)
            !-----------------------------------------------------------------------------
            IUF.BOOTS.CODE$ = IUF.MATRIX$(2)  !SAP sends leading zeroes

            IUF.GRP.CODE.FLAG$ = IUF.MATRIX$(3)

            IF IUF.GRP.CODE.FLAG$ <> "Y" THEN BEGIN
               IUF.GRP.CODE.FLAG$ = "N"
            ENDIF

            IUF.STNDRD.DESC$ = IUF.MATRIX$(4)
            IUF.STNDRD.DESC$ = LEFT$(IUF.STNDRD.DESC$ + STRING$(24, " "), 24)

            IUF.TILL.DESC$   = IUF.MATRIX$(5)
            IUF.TILL.DESC$   = LEFT$(IUF.TILL.DESC$   + STRING$(18, " "), 18)

            IUF.S.E.DESC$    = IUF.MATRIX$(6)
            IUF.S.E.DESC$    = LEFT$(IUF.S.E.DESC$    + STRING$(45, " "), 45)

            IUF.SUPPLY.ROUTE$                 = IUF.MATRIX$(7)
            IUF.GIVEAWAY$                     = IUF.MATRIX$(8)
            IUF.PROD.GRP$                     = IUF.MATRIX$(9)   !SAP sends leading zeroes
            IUF.GUARANTEE.LENGTH$             = IUF.MATRIX$(10)  !SAP sends leading zeroes
            IUF.ENF.PRICE.ENTRY$              = IUF.MATRIX$(11)
            IUF.EARN.POINTS$                  = IUF.MATRIX$(12)

            IUF.REDEEMABLE$                   = IUF.MATRIX$(13)
            IUF.DISCOUNTABLE$                 = IUF.MATRIX$(14)

!           IUF.DISCOUNTABLE$ should be either "Y" or "N"                     ! 1.11 RC (21)
!           If it is not then force an invalid value to "N"                   ! 1.11 RC (21)
            IF IUF.DISCOUNTABLE$ <> "Y" THEN BEGIN                            ! 1.11 RC (21)
                IUF.DISCOUNTABLE$ = "N" ! No discount given                   ! 1.11 RC (21)
            ENDIF                                                             ! 1.11 RC (21)

            IUF.OWN.BRAND$                    = IUF.MATRIX$(15)
            IUF.STATUS.1$                     = IUF.MATRIX$(16)

            IUF.CURRENT.PRICE$ = IUF.MATRIX$(17)
            IUF.CURRENT.PRICE$ = RIGHT$("00000000" + IUF.CURRENT.PRICE$, 8)

            IUF.STOCK.SYSTEM.FLAG$            = IUF.MATRIX$(18)
            IUF.BC.LETTER$                    = IUF.MATRIX$(19)
            IUF.ITEM.QTY$                     = IUF.MATRIX$(20)
            IUF.UNIT.MEASUREMENT$             = IUF.MATRIX$(21)
            IUF.UNIT.NAME$                    = IUF.MATRIX$(22)
            IUF.DATE.SENSITIVE$               = IUF.MATRIX$(23)
            IUF.RETURNABLE$                   = IUF.MATRIX$(24)
            IUF.RESALEABLE$                   = IUF.MATRIX$(25)
            IUF.SPECIAL.INSTRUCTION$          = IUF.MATRIX$(26)
            IUF.RETURN.ROUTE$                 = IUF.MATRIX$(27)
            IUF.ETHICAL.CLASS$                = IUF.MATRIX$(28)

            IUF.ETHICAL.DESCRIPTION$          = IUF.MATRIX$(29)
            IUF.ETHICAL.DESCRIPTION$  = LEFT$(IUF.ETHICAL.DESCRIPTION$    + STRING$(40, " "), 40)

            IUF.ETHICAL.ACTIVE$               = IUF.MATRIX$(30)
            IUF.ETHICAL.PACK.SIZE$            = IUF.MATRIX$(31)
            IUF.PRIMARY.SUPPLIER$             = IUF.MATRIX$(32)
            IUF.CONTAINS.ALCOHOL$             = IUF.MATRIX$(33)
            IUF.CONTAINS.PARACETAMOL$         = IUF.MATRIX$(34)
            IUF.CONTAINS.ASPIRIN$             = IUF.MATRIX$(35)
            IUF.CONTAINS.IBUPROFEN$           = IUF.MATRIX$(36)
            IUF.CONTAINS.EPHEDRINE$           = IUF.MATRIX$(37)
            IUF.CONTAINS.PSEUDOEPHEDRINE$     = IUF.MATRIX$(38)
            IUF.CONTAINS.NONSOLID.PAINKILLER$ = IUF.MATRIX$(39)
            IUF.UNRESTRICTED.GROUP.CODE$      = IUF.MATRIX$(40)
            IUF.GIFTCARD$                     = IUF.MATRIX$(41)
            IUF.AGE.RESTRICTION$              = IUF.MATRIX$(42)
            IUF.BOOTS.COM.EXTENDED$           = IUF.MATRIX$(43)
            IUF.INSURANCE$                    = IUF.MATRIX$(44)
            IUF.BLOCKED.FROM.SALE$            = IUF.MATRIX$(45)

        ENDIF ELSE IF IUF.REC.TYPE$ = "P" THEN BEGIN
            !-----------------------------------------------------------------------------
            ! Process Price Change Record
            !
            !-----------------------------------------------------------------------------
            !1  IUF.REC.TYPE$      ASCII   1   "P"
            !2  IUF.BOOTS.CODE$    ASCII   7   Includes check digit
            !3  IUF.RPD.DATE$      ASCII   8   Date the price change is required
            !4  IUF.RPD.NO$        ASCII   5   5-digit RPD number special values:
            !                                  99999 Emergency RPD
            !                                  99998 May have special processing associated with it
            !                                  99995 May be reserved for CIP markdowns
            !                                  99997 May have special processing associated with it
            !5  IUF.NEW.PRICE$     ASCII   8   Price to be activated at the specified RPD date
            !6  IUF.MARKDOWN$      ASCII   1   PLACEHOLDER Possible future use:
            !                                  blank - Normal price change
            !                                      L - SSM Normal Leaver
            !                                      S - SSM Sales Plan Leaver
            !-----------------------------------------------------------------------------
            IUF.BOOTS.CODE$ = IUF.MATRIX$(2)  !SAP sends leading zeroes
            IUF.RPD.DATE$   = IUF.MATRIX$(3)                                                        ! 1.7MG
            IUF.RPD.NO$     = IUF.MATRIX$(4)  !xxxx can be NULL? Default to 0's?

            IUF.NEW.PRICE$  = IUF.MATRIX$(5)
            IUF.NEW.PRICE$  = RIGHT$("00000000" + IUF.NEW.PRICE$, 8)

            IUF.MARKDOWN$   = "N"                                                         ! 1.9 TT

        ENDIF ELSE IF IUF.REC.TYPE$ = "B" THEN BEGIN
            !-----------------------------------------------------------------------------
            ! Process Barcode Record
            !
            !-----------------------------------------------------------------------------
            !1  IUF.REC.TYPE$       ASCII  1   "B"
            !2  IUF.BOOTS.CODE$     ASCII  8
            !3  IUF.BAR.CODE$       ASCII  13
            !-----------------------------------------------------------------------------
            IUF.BOOTS.CODE$ = IUF.MATRIX$(2)  !SAP sends leading zeroes
            IUF.BAR.CODE$   = IUF.MATRIX$(3)  ! SAP will not be sending leading zeroes
            IUF.BAR.CODE$   = RIGHT$(STRING$(13,"0") +  IUF.BAR.CODE$,13)

        ENDIF ELSE IF IUF.REC.TYPE$ = "T" THEN BEGIN
            !-----------------------------------------------------------------------------
            ! Process Trailer Record
            !
            !-----------------------------------------------------------------------------
            !1  IUF.REC.TYPE$      ASCII   1   "T"
            !2  IUF.REC.COUNT$     Integer 8
            !-----------------------------------------------------------------------------
            IUF.REC.COUNT$ = IUF.MATRIX$(2)

        ENDIF ELSE IF IUF.REC.TYPE$ = "H" THEN BEGIN
            !-----------------------------------------------------------------------------
            ! Process Header Record (moved to end of ELSE test in order to speed up processing
            !                         - rarer record types should always be tested for last).
            !
            !1  IUF.REC.TYPE$     ASCII  1  "H"
            !2  IUF.STORE.NUM$    ASCII  4
            !3  IUF.INITIAL.LOAD$ Char   1  Y/N - initial load flag
            !4  IUF.TIME.STAMP$   Char  17  yyyymmddhhmmsssss
            !-----------------------------------------------------------------------------
            IUF.STORE.NUM$    = IUF.MATRIX$(2)  !SAP sends leading zeroes
            IUF.INITIAL.LOAD$ = IUF.MATRIX$(3)
            IUF.TIME.STAMP$   = IUF.MATRIX$(4)

        ENDIF
    ENDIF

    READ.IUF = 0
    EXIT FUNCTION

READ.IUF.IF.END:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% =  IUF.REPORT.NUM%
    CURRENT.CODE$       =  CURRENT.CODE$ ! Previous successful read (if any)

    EXIT FUNCTION

END FUNCTION
