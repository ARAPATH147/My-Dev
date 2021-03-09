
\*****************************************************************************
\***
\***    %INCLUDE FOR TERMINAL ITEM FILE - FUNCTION DEFINITIONS
\***
\***            REFERENCE : TIFFUN
\***
\***    Version A           Steve Windsor         06.05.93
\***
\***    Version B           Steve Perkins         21.09.93
\***    Deals Project: Change TIF fields to reflect new IRF
\***    record layout.
\***
\***    Version C            Dave West            08.09.98
\***    Added TIF.RECL7 to function TIF.SET and new format
\***    TIF read and write functions.
\***
\***    Version D   Stuart William McConnachie    11.02.2000
\***    Changed TIF.INDICAT2$ to TIF.INDICAT2%
\***    Changed TIF.DEAL.NUM$ to TIF.DEAL.NUM%
\***
\***    REVISION 1.5.                ROBERT COWEY.                05 AUG 2002.
\***    Major changes for 2002 Deals Rewrite project.
\***    Deleted redundant functions specific to unused TIF formats ...
\***      READ.TIF.FULL.PLUS.USER.DATA
\***      READ.TIF.FULL.NO.USER.DATA
\***      READ.TIF.SHORT.PLUS.USER.DATA
\***      READ.TIF.SHORT.NO.USER.DATA
\***      READ.TIF.JUST.USER.DATA
\***      READ.TIF.NO.DESC.OR.DATA
\***      WRITE.TIF.FULL.NO.USER.DATA
\***      WRITE.TIF.SHORT.PLUS.USER.DATA
\***      WRITE.TIF.SHORT.NO.USER.DATA
\***      WRITE.TIF.JUST.USER.DATA
\***      WRITE.TIF.NO.DESC.OR.DATA
\***    Created several new functions ...
\***      TIF.CONCAT.RECORD   - Concatonate TIF variables into file format data
\***      CREATE.TIF.RECORD$  - Create TIF record string from TIF variables
\***      TIF.SPLIT.RECORD    - Expand file format data into TIF variables
\***      SPLIT.TIF.IRF.DATA$ - Extract TIF variables from IRF record string
\***    Modified READ and WRITE functions for new record layout.
\***
\***    REVISION 1.6.                ROBERT COWEY.                23 JAN 2003.
\***    Corrected function SPLIT.TIF.IRF.DATA$ to set TIF.SALEPRIC$ to zero when
\***    IRF variable INDICAT0% Enforced Price Entry flag X'20' is ON (because
\***    TIF variable INDICAT0% is no longer present on the physical TIF record).
\***
\***    REVISION 1.7                 TITTOO THOMAS                19 AUG 2011.
\***    Changed function SPLIT.TIF.IRF.DATA$ to set the Age check flag if the
\***    IRF record has a non-zero Age Restriction set. Otherwise, set it OFF.
\***
\***    REVISION 1.8.                ROBERT COWEY.                01 MAY 2012.
\***    Changes creating PSB21.286 Core Release 2 version 1.15.
\***
\***    Defect 200 - Commented 1.8 RC (200)
\***    Modified function READ.TIF.BOOTS.DATA to bypass TIF file read when 
\***    key TIF.BAR.CODE$ contains nulls (to prevent program abending with 
\***    KF error 80F306CD null key specified).
\***
\***...........................................................................


    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$


    %INCLUDE TIFDEC.J86   ! TIF variable declarations

    %INCLUDE EALHSASC.J86 ! External assembler function definitions        ! 1.5 RC
                          ! Includes all functions defined by EALGAADF.J86 ! 1.5.RC

FUNCTION TIF.SET PUBLIC

    TIF.RECL1%     = 50 ! For TIF.FORMAT% = 1 - not used by Boots
    TIF.RECL2%     = 42 ! For TIF.FORMAT% = 2 - not used by Boots
    TIF.RECL3%     = 44 ! For TIF.FORMAT% = 3 - not used by Boots
    TIF.RECL4%     = 36 ! For TIF.FORMAT% = 4 - not used by Boots
    TIF.RECL5%     = 32 ! For TIF.FORMAT% = 5 - not used by Boots
    TIF.RECL6%     = 24 ! For TIF.FORMAT% = 6 - not used by Boots
    TIF.RECL7%     = 18 ! TIF.FORMAT% = 7 - As used by Boots                 ! DW

    TIF.RECL% EQ TIF.RECL7%                                                ! 1.5 RC

    TIF.REPORT.NUM% = 65
    TIF.FILE.NAME$ = "EALIMAGE"

    DIM TIF.DEAL.DATA%(2) ! Entries used are 0 to 2

END FUNCTION


\********************************************************************************
\***
\***    TIF.CONCAT.RECORD
\***    Sets Giveaway and Item Movement data from INDICAT0% as required.
\***    Truncates some variables to match compressed format of record layout.
\***
\***.............................................................................


FUNCTION TIF.CONCAT.RECORD ! Local to TIFFUN      ! Entire function new for 1.5 RC

!   If INDICAT0% Giveaway bit flag ON, set SALEPRIC to Not Priced (X'FFFFFF')
!   Leave INDICAT0% bit flag set

    IF (TIF.INDICAT0% AND 00000010b) EQ 00000010b THEN \ ! X'02' is ON
        BEGIN
        TIF.SALEPRIC$ EQ STRING$(3,CHR$(255)) ! X'FFFFFF'
        ENDIF

!   If INDICAT0% Item Movement flag set ON, set corresponding INDICAT5% flag
!   Leave INDICAT0% bit flag set

    IF (TIF.INDICAT0% AND 10000000b) EQ 10000000b THEN \ ! X'80' is ON
        BEGIN
        TIF.INDICAT5% EQ (TIF.INDICAT5% OR 01000000b) ! X'40' set ON
        ENDIF

!   Truncates variables to match compressed format of TIF record layout

    TIF.BAR.CODE$ = RIGHT$(TIF.BAR.CODE$,6)
    TIF.SALEPRIC$ = RIGHT$(TIF.SALEPRIC$,3)

END FUNCTION


\********************************************************************************
\***
\***    CREATE.TIF.RECORD$
\***    Creates TIF record string (TIF.RECORD$) from individual TIF variables.
\***
\***.............................................................................


FUNCTION CREATE.TIF.RECORD$ PUBLIC                ! Entire function new for 1.5 RC

    CALL TIF.CONCAT.RECORD

    TIF.RECORD$ = \
           TIF.BAR.CODE$  + \
      CHR$(TIF.INDICAT6%) + \
          "  "            + \ ! Reserves two bytes for TIF.DEAL.DATA%(0)
          "  "            + \ ! Reserves two bytes for TIF.DEAL.DATA%(1)
           TIF.SALEPRIC$  + \
      CHR$(TIF.INDICAT5%) + \
          "  "            + \ ! Reserves two bytes for TIF.DEAL.DATA%(2)
      CHR$(TIF.INDICAT3%)

    CALL PUTN2 (TIF.RECORD$,  7, TIF.DEAL.DATA%(0)) ! Inserts TIF.DEAL.DATA%(n)
    CALL PUTN2 (TIF.RECORD$,  9, TIF.DEAL.DATA%(1)) ! variables into string
    CALL PUTN2 (TIF.RECORD$, 15, TIF.DEAL.DATA%(2)) ! TIF.RECORD$

END FUNCTION


\********************************************************************************
\***
\***    TIF.SPLIT.RECORD
\***    Sets Giveaway and Item Movement data as required.
\***    Expands some variables to match the format of their IRF counterparts.
\***    The function is called from TIF.SPLIT.RECORD (following a READ of the
\***    TIF) and from SPLIT.TIF.IRF.DATA (following a "read" of TIF data from
\***    IRF record string TIF.IRF.DATA$).
\***
\***    Note that when TIF.SALEPRIC$ is zero it is not possible to        ! 1.6 RC
\***    distinguish whether this is because it is genuinely zero or       ! 1.6 RC
\***    because its corresponding IRF record has the Enforced Price       ! 1.6 RC
\***    Entry flag set (and consequently this flag cannot be set on       ! 1.6 RC
\***    TIF.INDICAT0%).                                                   ! 1.6 RC
\***
\***.............................................................................


FUNCTION TIF.SPLIT.RECORD ! Local to TIFFUN       ! Entire function new for 1.5 RC

!   Zeroise INDICAT0% prior to setting Giveaway and Item Movement bit flags

    TIF.INDICAT0% = 0

!   If item Not Priced, set INDICAT0% Giveaway bit flag to ON and Price to 1p

    IF TIF.SALEPRIC$ EQ STRING$(3,CHR$(255)) THEN \ ! X'FFFFFF' (Not Priced)
        BEGIN
        TIF.INDICAT0% EQ (TIF.INDICAT0% OR 00000010b) ! X'02' set ON
        TIF.SALEPRIC$ EQ PACK$("000001") ! Giveaway price 1 penny
        ENDIF

!   If Item Movement kept, set INDICAT0% Item Movement bit flag to ON

    IF (TIF.INDICAT5% AND 01000000b) EQ 01000000b THEN \ ! X'40' is ON
        BEGIN
        TIF.INDICAT0% EQ (TIF.INDICAT0% OR 10000000b) ! X'80' set ON
        ENDIF

!   Expand TIF variables to match length of IRF counterparts

    TIF.BAR.CODE$ = PACK$("0000000000") + TIF.BAR.CODE$                    ! DW
    TIF.SALEPRIC$ = PACK$("0000") + TIF.SALEPRIC$                          ! DW

!   Set variables no longer used to null

    TIF.INDICAT1%    = 0                                                   ! DW
    TIF.INDICAT2%    = 0
    TIF.INDICAT4%    = 0
    TIF.DEAL.NUM%    = 0
    TIF.DEAL.SAVING$ = PACK$("0000")
    TIF.SALEQUAN$    = PACK$("00")                                         ! DW
    TIF.BOOTS.CODE$  = PACK$("000000")                                     ! DW

END FUNCTION


\********************************************************************************
\***
\***    SPLIT.TIF.IRF.DATA$
\***    Sets individual TIF variables from TIF.IRF.DATA$.
\***    Sets Giveaway and Item Movement data as though read from the TIF (calling
\***    TIF.SPLIT.RECORD to complete this process).
\***    The function is only ever called from within the UPDT.IRF.UPDT    ! 1.6 RC
\***    function defined within PSBF19.                                   ! 1.6 RC
\***
\***.............................................................................


FUNCTION SPLIT.TIF.IRF.DATA$ PUBLIC               ! Entire function new for 1.5 RC

!   Extract TIF variables from TIF.IRF.DATA$ (copy of IRF record string)

    TIF.BAR.CODE$      =      MID$(TIF.IRF.DATA$,1,11)   ! IRF.BARCODE$
    TIF.INDICAT6%      =  ASC(MID$(TIF.IRF.DATA$,13,1))  ! IRF.INDICAT1%
!   TIF.DEAL.DATA%(0) from    MID$(TIF.IRF.DATA$,14, 2)  ! IRF.DEAL.DATA%(0)
!   TIF.DEAL.DATA%(1) from    MID$(TIF.IRF.DATA$,46, 2)  ! IRF.DEAL.DATA%(1)
    TIF.SALEPRIC$      =      MID$(TIF.IRF.DATA$,19,5)   ! IRF.SALEPRIC$
    TIF.INDICAT5%      =  ASC(MID$(TIF.IRF.DATA$,24,1))  ! IRF.INDICAT5% modified
!   TIF.DEAL.DATA%(2) from    MID$(TIF.IRF.DATA$,48, 2)  ! IRF.DEAL.DATA%(1)
    TIF.INDICAT3%      =  ASC(MID$(TIF.IRF.DATA$,50, 1)) ! IRF.INDICAT3%

    TIF.DEAL.DATA%(0) = GETN2 (TIF.IRF.DATA$, 13) ! Extracts TIF.DEAL.DATA%(n)
    TIF.DEAL.DATA%(1) = GETN2 (TIF.IRF.DATA$, 45) ! variables from string
    TIF.DEAL.DATA%(2) = GETN2 (TIF.IRF.DATA$, 47) ! TIF.IRF.DATA$

!   If in IRF (TIF.IRF.DATA$) a non-zero age restriction is set ...         ! 1.7 TT
!   Set in TIF.INDICAT6% the Check Age flag ON, else set it OFF                ! 1.7 TT

    IF ASC(MID$(TIF.IRF.DATA$,18,1)) AND 00000111b THEN BEGIN ! X'07' is ON ! 1.7 TT
        TIF.INDICAT6% EQ (TIF.INDICAT6% OR 00100000b)  ! set X'20' ON       ! 1.7 TT
    ENDIF ELSE BEGIN                                                        ! 1.7 TT
        TIF.INDICAT6% EQ (TIF.INDICAT6% AND 11011111b) ! set X'20' OFF      ! 1.7 TT
    ENDIF                                                                   ! 1.7 TT

!   If IRF (TIF.IRF.DATA$) Giveaway bit flag is ON ...
!   Set TIF.SALEPRIC$ as for a Giveaway item read directly from the TIF
!   Further Giveaway processing is performed by the TIF.SPLIT.RECORD function

    IF ASC(MID$(TIF.IRF.DATA$,12,1)) AND 00000010b THEN \ ! X'02' is ON
        BEGIN
        TIF.SALEPRIC$ EQ STRING$(3,CHR$(255)) ! X'FFFFFF' (Not Priced)
        ENDIF

!   If IRF (TIF.IRF.DATA$) Item Movement flag is ON ...
!   Set TIF.INDICAT5% as for an Item Movement item read directly from the TIF
!   The corresponding TIF.INDICAT0% flag is set by the TIF.SPLIT.RECORD function

    IF ASC(MID$(TIF.IRF.DATA$,12,1)) AND 10000000b THEN \ ! X'80' is ON
        BEGIN
        TIF.INDICAT5% EQ (TIF.INDICAT5% OR 01000000b) ! X'40' set ON
        ENDIF

!   At this point the TIF variables extracted from the IRF record string
!   TIF.IRF.DATA$ have the same format and meaning as though read directly
!   from a record on the TIF itself.
!   (Eg; by the READ FORM statement within READ.TIF.BOOTS.DATA)           ! 1.6 RC

    CALL TIF.SPLIT.RECORD

!   At this point the TIF variables extracted from the IRF record string  ! 1.6 RC
!   TIF.IRF.DATA$ have the same format and meaning as though read         ! 1.6 RC
!   from a record on the TIF using function READ.TIF.BOOTS.DATA).         ! 1.6 RC


!   If IRF (TIF.IRF.DATA$) Enforced Price Entry flag is ON ...            ! 1.6 RC
!   Set TIF.SALEPRIC$ to zero to cause till to enforce price entry        ! 1.6 RC
!   when selling offline.                                                 ! 1.6 RC

    IF ASC(MID$(TIF.IRF.DATA$,12,1)) AND 00100000b THEN \ ! X'20' is ON   ! 1.6 RC
        BEGIN                                                             ! 1.6 RC
        TIF.SALEPRIC$ EQ PACK$("000000") ! Causes till to enforce price   ! 1.6 RC
        ENDIF                            ! entry when selling offline     ! 1.6 RC

END FUNCTION


\********************************************************************************
\***
\***    READ.TIF.BOOTS.DATA
\***    Corresponds to TIF.FORMAT% = 7 (Boots format) using TIF RECL 18.
\***    Reads the TIF record (and initialises TIF.INDICAT0% to zero).
\***    Calls TIF.SPLIT.RECORD to set Giveaway and Item Movement data as
\***    required and expand some variables to match the format of their
\***    IRF counterparts.
\***
\***.............................................................................


FUNCTION READ.TIF.BOOTS.DATA PUBLIC                              ! DW

    INTEGER*2 READ.TIF.BOOTS.DATA                                   ! DW

    READ.TIF.BOOTS.DATA = 1                                         ! DW

    TIF.BAR.CODE$ = RIGHT$(TIF.BAR.CODE$,6)                         ! DW

!   Prevent null key from causing KF error 80F306CD program abend   ! 1.8 RC (200)
    IF TIF.BAR.CODE$ = PACK$("000000000000") THEN BEGIN             ! 1.8 RC (200)
        GOTO READ.TIF.BOOTS.DATA.ERROR ! Exits function as for      ! 1.8 RC (200)
    ENDIF                              ! record not found on file   ! 1.8 RC (200)

    IF END # TIF.SESS.NUM% THEN READ.TIF.BOOTS.DATA.ERROR             ! DW

    READ FORM "T7,I1,2I2,C3,I1,I2,I1"; \                                  ! 1.5 RC
      # TIF.SESS.NUM%          \                                          ! DSWM
        KEY TIF.BAR.CODE$;     \                                          ! DW
            TIF.INDICAT6%,     \                                          ! 1.5 RC
            TIF.DEAL.DATA%(0), \                                          ! 1.5 RC
            TIF.DEAL.DATA%(1), \                                          ! 1.5 RC
            TIF.SALEPRIC$,     \                                          ! DW
            TIF.INDICAT5%,     \                                          ! DW
            TIF.DEAL.DATA%(2), \                                          ! 1.5 RC
            TIF.INDICAT3%                                                 ! DW

    CALL TIF.SPLIT.RECORD                                                 ! 1.5 RC

!   Lines incorporated into new function TIF.SPLIT.RECORD                 ! 1.5 RC

    READ.TIF.BOOTS.DATA = 0                                         ! DW
    EXIT FUNCTION                                                   ! DW

READ.TIF.BOOTS.DATA.ERROR:                                          ! DW

    CURRENT.REPORT.NUM% = TIF.REPORT.NUM%                           ! DW
    FILE.OPERATION$ = "R"                                           ! DW
    CURRENT.CODE$ = ""                                              ! DW

END FUNCTION                                                        ! DW


\********************************************************************************
\***
\***    WRITE.TIF.BOOTS.DATA
\***    Corresponds to TIF.FORMAT% = 7 (Boots format) using TIF RECL 18.
\***    Calls TIF.CONCAT.RECORD to set Giveaway and Item Movement data from
\***    INDICAT0% and to truncate some variables as required.
\***    Writes the TIF record.
\***
\***.............................................................................


FUNCTION WRITE.TIF.BOOTS.DATA PUBLIC                                ! DW

    INTEGER*2 WRITE.TIF.BOOTS.DATA                                  ! DW

    WRITE.TIF.BOOTS.DATA = 1                                        ! DW

!   Lines deleted                                                   ! 1.5 RC

    CALL TIF.CONCAT.RECORD                                          ! 1.5 RC

    IF END # TIF.SESS.NUM% THEN WRITE.TIF.BOOTS.DATA.ERROR          ! DW

    WRITE FORM "C6,I1,2I2,C3,I1,I2,I1"; \                           ! 1.5 RC
      # TIF.SESS.NUM%;         \                                    ! 1.5 RC
            TIF.BAR.CODE$,     \                                    ! DW
            TIF.INDICAT6%,     \                                    ! DW
            TIF.DEAL.DATA%(0), \                                    ! 1.5 RC
            TIF.DEAL.DATA%(1), \                                    ! 1.5 RC
            TIF.SALEPRIC$,     \                                    ! DW
            TIF.INDICAT5%,     \                                    ! DW
            TIF.DEAL.DATA%(2), \                                    ! 1.5 RC
            TIF.INDICAT3%                                           ! DW

    WRITE.TIF.BOOTS.DATA = 0                                        ! DW
    EXIT FUNCTION                                                   ! DW

    WRITE.TIF.BOOTS.DATA.ERROR:                                     ! DW

    CURRENT.REPORT.NUM% = TIF.REPORT.NUM%                           ! DW
    FILE.OPERATION$ = "W"                                           ! DW
    CURRENT.CODE$ = ""                                              ! DW

END FUNCTION                                                     ! DW

