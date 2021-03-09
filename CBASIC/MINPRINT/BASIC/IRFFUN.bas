
\*****************************************************************************
\***
\***      IRF - ITEM REFERENCE FILE - External functions
\***
\***      Reference : IRFFUN.BAS
\***
\***      Version A           Andrew Wedgeworth     9th July 1992
\***
\***      Version B           Steve Windsor        14th December 1992
\***      Version letter incremented in line with other IRF code.
\***
\***      Version C           Steve Windsor         12.02.93
\***      Added function to read the IRF ALT.
\***
\***      Version D           Steve Windsor         12.05.93
\***      All IRF functions included.
\***
\***      Version E           Steve Perkins       20th September 1993
\***      Deals project : Handling of Converted/Unconverted records
\***      ++   Anything with 'Delete' after initials should be   ++
\***      ++   deleted once the IRF has been converted in all    ++
\***      ++   stores.                                           ++
\***
\***      Version F           Mark Walker            5th January 1994
\***      If a read/write fails, set the CURRENT.CODE$ variable
\***      to the current bar code.
\***
\***      Version 96A         Mark Walker               22nd May 1995
\***      A new field IRF.POINTS% has replaced the existing
\***      field IRF.FILLER$. Redundant deals code has been removed.
\***
\***      96A supplemental    David Smallwood           20th September 1995
\***      IRF.RECORD$ initialised to "CONVERTED RECORD" for READ functions.
\***      This is to enable programs that still refer to IRF.RECORD$
\***      (e.g.PSS35) to function correctly.
\***
\***      Version G     Stuart William McConnachie      11th February 2000
\***      Converted IRF.INDICAT2$ to integer flag byte.
\***
\***      REVISION 1.6.               ROBERT COWEY.               9 JUL 2002.
\***      Major changes for 2002 Deals Rewrite project.
\***      See IRFDEC.J86 for updated record layout.
\***      Removed variables that are redundant or related to old deals system
\***      from record (input and output) definitions ...
\***      INDICAT2%, DEAL.NUM$, INDICAT4%, SALEQUAN$, DEAL.SAVING$, POINTS%
\***      Defined new deal data (table) variables within record definitions ...
\***      DEAL.DATA%(0-2)
\***
\***      Defined new function IRF.SPLIT.RECORD
\***      Set variables that may be "retained" within programs to nulls X'00' ...
\***      INDICAT2%, INDICAT4%, SALEQUAN$, POINTS%
\***      Interpreted DEAL.DATA%(n) variables into sub-variables ...
\***      DEAL.NUM$(0-2), LIST.ID%(0-2)
\***
\***      Defined new function IRF.CONCAT.RECORD
\***      Recreated DEAL.DATA%(n) variables from (interpreted) sub-variables ...
\***      DEAL.NUM$(0-2), LIST.ID%(0-2)
\***
\***      Avoided use of WHILE loop and subscript to keep code more readable
\***      when handling DEAL.DATA%(n) related variables
\***
\***      REVISION 1.7.               ROBERT COWEY.               5 AUG 2002.
\***      Further changes for 2002 Deals Rewrite project (PSBF19 related).
\***      Included definition of external GETNn and PUTNn assembler functions.
\***      Defined function CONCAT.NEW.IRF.DATA$ to combine individual IRF variables
\***      into a single (record) string NEW.IRF.DATA$.
\***      Defined function SPLIT.NEW.IRF.DATA$ to extract individual IRF variables
\***      from (IRF record) string NEW.IRF.DATA$.
\***
\***      REVISION 1.8                ROBERT COWEY.              15 JUL 2003.
\***      Usage of INDICAT0% bit-3 X'08' changed to Item Contains Alcohol.
\***      No changes to this file other than description.
\***      No changes to IRF file functions.
\***
\***      REVISION 1.9.     STUART WILLIAM MCCONNACHIE           22 OCT 2003.
\***      Changes to remove limit of 3 deals per item.  Added code to access
\***      aditional deals on the IRF Deal Extension file (IRFDEX).
\***      SUPPLEMENTAL      ROBERT COWEY                         02 DEC 2003
\***      Modified CONCAT.NEW.IRF.DATA$ and SPLIT.NEW.IRF.DATA$ to incorporate
\***      new IRFDEX deal variables within NEW.IRF.DATA$ string used by PSBF19.
\***      Modified READ.IRF.ALT to call READ.IRFDEX (to match other functions).
\***      Changed IRFDEX report number from 663 to 673.
\***
\***      REVISION 2.0.             ALAN CARR                      9 FEB 2006.
\***      Add new IRF.INDICAT8% 1 byte, amend IRF.UNUSED$ from 3 to 2 bytes.
\***
\***      REVISION 2.1              BRIAN GREENFIELD               14 May 2009
\***      Chanegs to increase the number of deals from 10 to 40.
\***      Converted two sections to loops to simplify the code.
\***
\***      REVISION 2.2              TITTOO THOMAS                  01 July 2011
\***      The IRF.UNUSED field is disintegrated to 2 new indicator fields
\***                      IRF.INDICAT9%   1 INT
\***                      IRF.INDICAT10%  1 INT
\***...............................................................................


    STRING GLOBAL                     \
        CURRENT.CODE$,                 \
        FILLER$,                      \ 2.1 BG
        FILE.OPERATION$

    INTEGER*2 GLOBAL                  \
        CURRENT.REPORT.NUM%

    INTEGER*2 I%          ! Loop counter                                   ! 1.9 SM
    INTEGER*2 OFFSET%     ! Offset value                                   ! 2.1 BG
    INTEGER*2             \ IRF.DEAL.DATA%(n) variable defined locally     ! 1.6 RC
        IRF.DEAL.DATA%(1) ! to keep underlying data invisible to programs  ! 1.6 RC


    %INCLUDE IRFDEC.J86   ! IRF variable declarations

    %INCLUDE EALHSASC.J86 ! External assembler function definitions        ! 1.7 RC
                          ! Includes all functions defined by EALGAADF.J86 ! 1.7.RC

FUNCTION IRF.SET PUBLIC

    IRF.REPORT.NUM%     =  7
    IRF.ALT.REPORT.NUM% = 58
    IRF.RECL%           = 50
    IRF.FILE.NAME$      = "IRF"
    IRF.ALT.FILE.NAME$  = "IRFALT"

    IRFDEX.REPORT.NUM%  = 673                                             ! 1.9 SM+RC
    IRFDEX.RECL%        = 84                                              ! 1.9 SM 2.1 BG
    IRFDEX.FILE.NAME$   = "IRFDEX"                                        ! 1.9 SM

    IRF.MAX.DEALS%      = 40                                              ! 1.9 SM 2.1 BG
    DIM IRF.DEAL.DATA%(IRF.MAX.DEALS%-1) ! Entries used are 0 to 2        ! 1.6 RC 1.9 SM
    DIM IRF.DEAL.NUM$(IRF.MAX.DEALS%-1)  ! Entries used are 0 to 2        ! 1.6 RC 1.9 SM
    DIM IRF.LIST.ID%(IRF.MAX.DEALS%-1)   ! Entries used are 0 to 2        ! 1.6 RC 1.9 SM

END FUNCTION


\*******************************************************************************
\***
\***    IRF.SPLIT.RECORD
\***    Interprets DEAL.DATA%(n) variables into sub-variables ...
\***    DEAL.NUM$(0-2), LIST.ID%(0-2)
\***    Sets variables that may be "retained" within programs to nulls X'00' ...
\***    INDICAT2%, INDICAT4%, SALEQUAN$, POINTS%
\***    The function is called immediately following a READ of the IRF.
\***
\***............................................................................


FUNCTION IRF.SPLIT.RECORD ! Local to IRFFUN       ! Entire function new for 1.6 RC

    IRF.DD.SUB% = 0

    FOR I% = 0 TO IRF.MAX.DEALS% - 1                                    ! 1.9 SM

        IRF.DEAL.NUM$(I%) = PACK$(RIGHT$("0000"+                        \ 1.9 SM
                              STR$(IRF.DEAL.DATA%(I%) AND 03FFFh),4))   ! 1.9 SM
        IRF.LIST.ID%(I%) = SHIFT(IRF.DEAL.DATA%(I%),14) AND 03h         ! 1.9 SM

    NEXT I%                                                             ! 1.9 SM

    IRF.INDICAT2% = 0           ! Redundant variable
    IRF.INDICAT4% = 0           ! Redundant variable
    IRF.POINTS%   = 0           ! Redundant variable
    IRF.SALEQUAN$ = PACK$("00") ! Redundant variable

END FUNCTION


\*******************************************************************************
\***
\***    IRF.CONCAT.RECORD
\***    Recreates DEAL.DATA%(n) variables from (interpreted) sub-variables ...
\***    DEAL.NUM$(0-2), LIST.ID%(0-2).
\***    The function is called immediately prior to a WRITE to the IRF.
\***
\***............................................................................


FUNCTION IRF.CONCAT.RECORD ! Local to IRFFUN      ! Entire function new for 1.6 RC

    INTEGER*2 DEAL.NUM%                                                 ! 1.9 SM

    FOR I% = 0 TO IRF.MAX.DEALS% - 1                                    ! 1.9 SM

        DEAL.NUM% = VAL(UNPACK$(IRF.DEAL.NUM$(I%))) AND 03FFFh          ! 1.9 SM
        IF IRF.LIST.ID%(I%) AND 1 THEN BEGIN                            ! 1.9 SM
            DEAL.NUM% = DEAL.NUM% OR 04000h                             ! 1.9 SM
        ENDIF                                                           ! 1.9 SM
        IF IRF.LIST.ID%(I%) AND 2 THEN BEGIN                            ! 1.9 SM
            DEAL.NUM% = DEAL.NUM% OR 08000h                             ! 1.9 SM
        ENDIF                                                           ! 1.9 SM
        IRF.DEAL.DATA%(I%) = DEAL.NUM%                                  ! 1.9 SM

    NEXT I%                                                             ! 1.9 SM

END FUNCTION


\********************************************************************************
\***
\***    CONCAT.NEW.IRF.DATA$
\***    Sets NEW.IRF.DATA$ from individual IRF variables.
\***    The function is typically called immediately prior to calling the
\***    UPDT.IRF.UPDT function defined within PSBF19.
\***
\***.............................................................................


FUNCTION CONCAT.NEW.IRF.DATA$ PUBLIC             ! Entire function new for 1.7 RC

    NEW.IRF.DATA$ =          \
           IRF.BAR.CODE$   + \
      CHR$(IRF.INDICAT0%)  + \
      CHR$(IRF.INDICAT1%)  + \
          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(0)
      CHR$(IRF.INDICAT8%)  + \ ! New WEEE Item Flag                        ! 2.0 AC
\!           IRF.UNUSED$     + \                                           ! 2.2 TT
      CHR$(IRF.INDICAT9%)  + \                                             ! 2.2 TT
      CHR$(IRF.INDICAT10%) + \                                             ! 2.2 TT
           IRF.SALEPRIC$   + \
      CHR$(IRF.INDICAT5%)  + \
           IRF.ITEMNAME$   + \
           IRF.BOOTS.CODE$ + \
          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(1)
          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(2)
      CHR$(IRF.INDICAT3%)  + \                                             ! 1.9 RC
      STRING$((IRF.MAX.DEALS% * 2) - 6, " ")                                ! 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(3)  ! 1.9 RC 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(4)  ! 1.9 RC 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(5)  ! 1.9 RC 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(6)  ! 1.9 RC 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(7)  ! 1.9 RC 2.1 BG
!          "  "             + \ ! Reserves two bytes for IRF.DEAL.DATA%(8)  ! 1.9 RC 2.1 BG
!          "  "                 ! Reserves two bytes for IRF.DEAL.DATA%(9)  ! 1.9 RC 2.1 BG

    CALL IRF.CONCAT.RECORD ! Sets local IRF.DEAL.DATA%(n) variables from global
                           ! IRF.LIST.ID%(n) and IRF.DEAL.NUM$(n) variables

    CALL PUTN2 (NEW.IRF.DATA$, 13, IRF.DEAL.DATA%(0)) ! Inserts IRF.DEAL.DATA%(n)
    CALL PUTN2 (NEW.IRF.DATA$, 45, IRF.DEAL.DATA%(1)) ! variables into string
    CALL PUTN2 (NEW.IRF.DATA$, 47, IRF.DEAL.DATA%(2)) ! NEW.IRF.DATA$
!    CALL PUTN2 (NEW.IRF.DATA$, 50, IRF.DEAL.DATA%(3))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 52, IRF.DEAL.DATA%(4))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 54, IRF.DEAL.DATA%(5))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 56, IRF.DEAL.DATA%(6))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 58, IRF.DEAL.DATA%(7))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 60, IRF.DEAL.DATA%(8))                      ! 1.9 RC 2.1 BG
!    CALL PUTN2 (NEW.IRF.DATA$, 62, IRF.DEAL.DATA%(9))                      ! 1.9 RC 2.1 BG

    OFFSET% = 50                                                            ! 2.1 BG
    FOR I% = 3 TO IRF.MAX.DEALS% - 1                                        ! 2.1 BG
        CALL PUTN2 (NEW.IRF.DATA$, OFFSET%, IRF.DEAL.DATA%(I%))             ! 2.1 BG
        OFFSET% = OFFSET% + 2                                               ! 2.1 BG
    NEXT I%                                                                 ! 2.1 BG


END FUNCTION


\********************************************************************************
\***
\***    SPLIT.NEW.IRF.DATA$
\***    Sets individual IRF variables from NEW.IRF.DATA$
\***    The function is typically called from within the UPDT.IRF.UPDT function
\***    defined within PSBF19.
\***
\***.............................................................................


FUNCTION SPLIT.NEW.IRF.DATA$ PUBLIC              ! Entire function new for 1.7 RC

    IRF.BAR.CODE$      =      MID$(NEW.IRF.DATA$, 1,11)
    IRF.INDICAT0%      =  ASC(MID$(NEW.IRF.DATA$,12, 1))
    IRF.INDICAT1%      =  ASC(MID$(NEW.IRF.DATA$,13, 1))
!   IRF.DEAL.DATA%(0) from    MID$(NEW.IRF.DATA$,14, 2)
    IRF.INDICAT8%      =  ASC(MID$(NEW.IRF.DATA$,16, 1))                 ! 2.0 AC
!   IRF.UNUSED$        =      MID$(NEW.IRF.DATA$,17, 2)                  ! 2.0 AC 2.2 TT
    IRF.INDICAT9%      =  ASC(MID$(NEW.IRF.DATA$,17, 1))                 ! 2.2 TT
    IRF.INDICAT10%     =  ASC(MID$(NEW.IRF.DATA$,18, 1))                 ! 2.2 TT
    IRF.SALEPRIC$      =      MID$(NEW.IRF.DATA$,19, 5)
    IRF.INDICAT5%      =  ASC(MID$(NEW.IRF.DATA$,24, 1))
    IRF.ITEMNAME$      =      MID$(NEW.IRF.DATA$,25,18)
    IRF.BOOTS.CODE$    =      MID$(NEW.IRF.DATA$,43, 3)
!   IRF.DEAL.DATA%(1) from    MID$(NEW.IRF.DATA$,46, 2)
!   IRF.DEAL.DATA%(2) from    MID$(NEW.IRF.DATA$,48, 2)
    IRF.INDICAT3%      =  ASC(MID$(NEW.IRF.DATA$,50, 1))

    IRF.DEAL.DATA%(0) = GETN2 (NEW.IRF.DATA$, 13) ! Extracts IRF.DEAL.DATA%(n)
    IRF.DEAL.DATA%(1) = GETN2 (NEW.IRF.DATA$, 45) ! variables from string
    IRF.DEAL.DATA%(2) = GETN2 (NEW.IRF.DATA$, 47) ! NEW.IRF.DATA$
!    IRF.DEAL.DATA%(3) = GETN2 (NEW.IRF.DATA$, 50)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(4) = GETN2 (NEW.IRF.DATA$, 52)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(5) = GETN2 (NEW.IRF.DATA$, 54)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(6) = GETN2 (NEW.IRF.DATA$, 56)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(7) = GETN2 (NEW.IRF.DATA$, 58)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(8) = GETN2 (NEW.IRF.DATA$, 60)                       ! 1.9 RC 2.1 BG
!    IRF.DEAL.DATA%(9) = GETN2 (NEW.IRF.DATA$, 62)                       ! 1.9 RC 2.1 BG

    OFFSET% = 50                                                        ! 2.1 BG
    FOR I% = 3 TO IRF.MAX.DEALS% - 1                                    ! 2.1 BG
        IRF.DEAL.DATA%(I%) = GETN2 (NEW.IRF.DATA$, OFFSET%)             ! 2.1 BG
        OFFSET% = OFFSET% + 2                                           ! 2.1 BG
    NEXT I%                                                             ! 2.1 BG

    CALL IRF.SPLIT.RECORD ! Sets global IRF.LIST.ID%(n) and IRF.DEAL.NUM$(n)
                          ! variables from local IRF.DEAL.DATA%(n) variables

END FUNCTION


\----------------------------------------------------------------------------

    FUNCTION READ.IRFDEX                                                  ! 1.9 SM

    INTEGER*2 READ.IRFDEX                                                 ! 1.9 SM

    READ.IRFDEX = 1                                                       ! 1.9 SM

    IF IRFDEX.SESS.NUM% <> 0 AND                                          \ 1.9 SM
       IRF.DEAL.DATA%(2) <> 0 AND                                         \ 1.9 SM
       IRF.BOOTS.CODE$ <> PACK$("000000") THEN BEGIN                      ! 1.9 SM

        IF END #IRFDEX.SESS.NUM% THEN READ.IRFDEX.ERROR                   ! 1.9 SM
        READ FORM "T4,37I2,C7"; #IRFDEX.SESS.NUM%                         \ 1.9 SM 2.1 BG
        KEY IRF.BOOTS.CODE$;                                              \ 1.9 SM
            IRF.DEAL.DATA%(3),                                            \ 1.9 SM
            IRF.DEAL.DATA%(4),                                            \ 1.9 SM
            IRF.DEAL.DATA%(5),                                            \ 1.9 SM
            IRF.DEAL.DATA%(6),                                            \ 1.9 SM
            IRF.DEAL.DATA%(7),                                            \ 1.9 SM
            IRF.DEAL.DATA%(8),                                            \ 1.9 SM
            IRF.DEAL.DATA%(9),                                            \ 1.9 SM 2.1 BG
            IRF.DEAL.DATA%(10),                                           \ 2.1 BG
            IRF.DEAL.DATA%(11),                                           \ 2.1 BG
            IRF.DEAL.DATA%(12),                                           \ 2.1 BG
            IRF.DEAL.DATA%(13),                                           \ 2.1 BG
            IRF.DEAL.DATA%(14),                                           \ 2.1 BG
            IRF.DEAL.DATA%(15),                                           \ 2.1 BG
            IRF.DEAL.DATA%(16),                                           \ 2.1 BG
            IRF.DEAL.DATA%(17),                                           \ 2.1 BG
            IRF.DEAL.DATA%(18),                                           \ 2.1 BG
            IRF.DEAL.DATA%(19),                                           \ 2.1 BG
            IRF.DEAL.DATA%(20),                                           \ 2.1 BG
            IRF.DEAL.DATA%(21),                                           \ 2.1 BG
            IRF.DEAL.DATA%(22),                                           \ 2.1 BG
            IRF.DEAL.DATA%(23),                                           \ 2.1 BG
            IRF.DEAL.DATA%(24),                                           \ 2.1 BG
            IRF.DEAL.DATA%(25),                                           \ 2.1 BG
            IRF.DEAL.DATA%(26),                                           \ 2.1 BG
            IRF.DEAL.DATA%(27),                                           \ 2.1 BG
            IRF.DEAL.DATA%(28),                                           \ 2.1 BG
            IRF.DEAL.DATA%(29),                                           \ 2.1 BG
            IRF.DEAL.DATA%(30),                                           \ 2.1 BG
            IRF.DEAL.DATA%(31),                                           \ 2.1 BG
            IRF.DEAL.DATA%(32),                                           \ 2.1 BG
            IRF.DEAL.DATA%(33),                                           \ 2.1 BG
            IRF.DEAL.DATA%(34),                                           \ 2.1 BG
            IRF.DEAL.DATA%(35),                                           \ 2.1 BG
            IRF.DEAL.DATA%(36),                                           \ 2.1 BG
            IRF.DEAL.DATA%(37),                                           \ 2.1 BG
            IRF.DEAL.DATA%(38),                                           \ 2.1 BG
            IRF.DEAL.DATA%(39),                                           \ 2.1 BG
            FILLER$                                                       ! 2.1 BG

    ENDIF ELSE BEGIN                                                      ! 1.9 SM

READ.IRFDEX.ERROR:                                                        ! 1.9 SM
        FOR I% = 3 TO IRF.MAX.DEALS% - 1                                  ! 1.9 SM
            IRF.DEAL.DATA%(I%) = 0                                        ! 1.9 SM
        NEXT I%                                                           ! 1.9 SM

    ENDIF                                                                 ! 1.9 SM

    READ.IRFDEX = 0                                                       ! 1.9 SM
    EXIT FUNCTION                                                         ! 1.9 SM

    END FUNCTION                                                          ! 1.9 SM

\----------------------------------------------------------------------------

    FUNCTION WRITE.IRFDEX                                                 ! 1.9 SM

    INTEGER*2 WRITE.IRFDEX                                                ! 1.9 SM

    WRITE.IRFDEX = 1                                                      ! 1.9 SM

    IF IRFDEX.SESS.NUM% THEN BEGIN                                        ! 1.9 SM

        IF IRF.BAR.CODE$ = PACK$("0000000000000000")+IRF.BOOTS.CODE$ THEN BEGIN ! 1.9 SM

            IF IRF.DEAL.DATA%(3) <> 0 THEN BEGIN                          ! 1.9 SM

                !Ensure filler is packed zero's.                          ! 2.1 BG
                FILLER$ = PACK$("00000000000000")                         ! 2.1 BG

                IF END #IRFDEX.SESS.NUM% THEN WRITE.IRFDEX.ERROR          ! 1.9 SM
                WRITE FORM "C3,37I2,C7"; #IRFDEX.SESS.NUM%;               \ 1.9 SM 2.1 BG
                    IRF.BOOTS.CODE$,                                      \ 1.9 SM
                    IRF.DEAL.DATA%(3),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(4),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(5),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(6),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(7),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(8),                                    \ 1.9 SM
                    IRF.DEAL.DATA%(9),                                    \ 1.9 SM 2.1 BG
                    IRF.DEAL.DATA%(10),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(11),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(12),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(13),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(14),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(15),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(16),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(17),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(18),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(19),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(20),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(21),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(22),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(23),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(24),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(25),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(26),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(27),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(28),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(29),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(30),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(31),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(32),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(33),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(34),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(35),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(36),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(37),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(38),                                   \ 2.1 BG
                    IRF.DEAL.DATA%(39),                                   \ 2.1 BG
                    FILLER$                                               ! 2.1 BG

            ENDIF ELSE BEGIN                                              ! 1.9 SM

                IF END #IRFDEX.SESS.NUM% THEN NO.IRFDEX.RECORD            ! 1.9 SM
                DELREC IRFDEX.SESS.NUM%; IRF.BOOTS.CODE$                  ! 1.9 SM
NO.IRFDEX.RECORD:                                                         ! 1.9 SM
            ENDIF                                                         ! 1.9 SM

        ENDIF                                                             ! 1.9 SM

    ENDIF                                                                 ! 1.9 SM

    WRITE.IRFDEX = 0                                                      ! 1.9 SM
    EXIT FUNCTION                                                         ! 1.9 SM

WRITE.IRFDEX.ERROR:                                                       ! 1.9 SM
    CURRENT.REPORT.NUM% = IRFDEX.REPORT.NUM%                              ! 1.9 SM
    FILE.OPERATION$ = "W"                                                 ! 1.9 SM
    CURRENT.CODE$ = PACK$("00000000") + IRF.BOOTS.CODE$                   ! 1.9 SM

    EXIT FUNCTION                                                         ! 1.9 SM

    END FUNCTION                                                          ! 1.9 SM

\---------------------------------------------------------------------------------

  FUNCTION READ.IRF PUBLIC

   INTEGER*2 READ.IRF

   READ.IRF = 1

   IF END#IRF.SESS.NUM% THEN READ.IRF.ERROR

    READ FORM  "T12,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; \               ! 1.6.RC ! 2.0 AJC ! 2.2 TT
         #IRF.SESS.NUM%                                             \ MW96A
         KEY IRF.BAR.CODE$;                                         \
             IRF.INDICAT0%,                                         \
             IRF.INDICAT1%,                                         \
             IRF.DEAL.DATA%(0),                                     \     ! 1.6 RC
             IRF.INDICAT8%,                                         \     ! 2.0 AJC
\!            IRF.UNUSED$,                                          \     ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                         \     ! 2.2 TT
             IRF.INDICAT10%,                                        \     ! 2.2 TT
             IRF.SALEPRIC$,                                         \
             IRF.INDICAT5%,                                         \ SBH 31/1/96
             IRF.ITEMNAME$,                                         \
             IRF.BOOTS.CODE$,                                       \
             IRF.DEAL.DATA%(1),                                     \     ! 1.6 RC
             IRF.DEAL.DATA%(2),                                     \     ! 1.6 RC
             IRF.INDICAT3%

    IF READ.IRFDEX THEN EXIT FUNCTION                                     ! 1.9 SM

    CALL IRF.SPLIT.RECORD                                                 ! 1.6 RC

    READ.IRF = 0

    IRF.RECORD$ = "CONVERTED RECORD"                                ! DS96A

   EXIT FUNCTION

   READ.IRF.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                          ! FMW

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.IRF.ALT PUBLIC

   INTEGER*2 I%                                                           ! 1.9 SM
   INTEGER*2 READ.IRF.ALT

   READ.IRF.ALT = 1

   IF END#IRF.ALT.SESS.NUM% THEN READ.IRF.ALT.ERROR

    READ FORM  "T12,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; \               ! 1.6.RC ! 2.0 AJC ! 2.2 TT
         #IRF.ALT.SESS.NUM%                                         \
         KEY IRF.BAR.CODE$;                                         \
             IRF.INDICAT0%,                                         \
             IRF.INDICAT1%,                                         \
             IRF.DEAL.DATA%(0),                                     \     ! 1.6 RC
             IRF.INDICAT8%,                                         \     ! 2.0 AJC
\!            IRF.UNUSED$,                                          \     ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                         \     ! 2.2 TT
             IRF.INDICAT10%,                                        \     ! 2.2 TT
             IRF.SALEPRIC$,                                         \
             IRF.INDICAT5%,                                         \ SBH 31/1/96
             IRF.ITEMNAME$,                                         \
             IRF.BOOTS.CODE$,                                       \
             IRF.DEAL.DATA%(1),                                     \     ! 1.6 RC
             IRF.DEAL.DATA%(2),                                     \     ! 1.6 RC
             IRF.INDICAT3%

    IF READ.IRFDEX THEN EXIT FUNCTION                                     ! 1.9 RC

!   FOR I% = 3 TO IRF.MAX.DEALS% - 1                                      ! 1.9 SM-RC
!       IRF.DEAL.DATA%(I%) = 0                                            ! 1.9 SM-RC
!   NEXT I%                                                               ! 1.9 SM-RC

    CALL IRF.SPLIT.RECORD                                                 ! 1.6 RC

    READ.IRF.ALT = 0

    IRF.RECORD$ = "CONVERTED RECORD"                                ! DS96A

   EXIT FUNCTION

   READ.IRF.ALT.ERROR:

   CURRENT.REPORT.NUM% = IRF.ALT.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                          ! FMW

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.IRF.LOCK PUBLIC

   INTEGER*2 READ.IRF.LOCK

   READ.IRF.LOCK = 1

   IF END#IRF.SESS.NUM% THEN READ.IRF.LOCK.ERROR

    READ FORM  "T12,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; \               ! 1.6.RC ! 2.0 AJC ! 2.2 TT
         #IRF.SESS.NUM%                                             \ MW96A
         AUTOLOCK                                                   \
         KEY IRF.BAR.CODE$;                                         \
             IRF.INDICAT0%,                                         \
             IRF.INDICAT1%,                                         \
             IRF.DEAL.DATA%(0),                                     \     ! 1.6 RC
             IRF.INDICAT8%,                                         \     ! 2.0 AJC
\!            IRF.UNUSED$,                                          \     ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                         \     ! 2.2 TT
             IRF.INDICAT10%,                                        \     ! 2.2 TT
             IRF.SALEPRIC$,                                         \
             IRF.INDICAT5%,                                         \ SBH 31/1/96
             IRF.ITEMNAME$,                                         \
             IRF.BOOTS.CODE$,                                       \
             IRF.DEAL.DATA%(1),                                     \     ! 1.6 RC
             IRF.DEAL.DATA%(2),                                     \     ! 1.6 RC
             IRF.INDICAT3%

    IF READ.IRFDEX THEN EXIT FUNCTION                                     ! 1.9 SM

    CALL IRF.SPLIT.RECORD                                                 ! 1.6 RC

    READ.IRF.LOCK = 0

    IRF.RECORD$ = "CONVERTED RECORD"                                ! DS96A

   EXIT FUNCTION

   READ.IRF.LOCK.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                          ! FMW

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.IRF PUBLIC

   INTEGER*2 WRITE.IRF

    WRITE.IRF = 1

    CALL IRF.CONCAT.RECORD                                                ! 1.6 RC

    IF END#IRF.SESS.NUM% THEN WRITE.IRF.ERROR

    WRITE FORM "C11,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; \               ! 1.6 RC ! 2.0 AJC ! 2.2 TT
          #IRF.SESS.NUM%;                                         \ 96AM
             IRF.BAR.CODE$,                                       \ 96AMW
             IRF.INDICAT0%,                                       \
             IRF.INDICAT1%,                                       \
             IRF.DEAL.DATA%(0),                                   \       ! 1.6 RC
             IRF.INDICAT8%,                                       \       ! 2.0 AJC
\!            IRF.UNUSED$,                                        \       ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                       \       ! 2.2 TT
             IRF.INDICAT10%,                                      \       ! 2.2 TT
             IRF.SALEPRIC$,                                       \
             IRF.INDICAT5%,                                       \ SBH 31/1/96
             IRF.ITEMNAME$,                                       \
             IRF.BOOTS.CODE$,                                     \
             IRF.DEAL.DATA%(1),                                   \       ! 1.6 RC
             IRF.DEAL.DATA%(2),                                   \       ! 1.6 RC
             IRF.INDICAT3%

    IF WRITE.IRFDEX THEN EXIT FUNCTION                                    ! 1.9 SM

    WRITE.IRF = 0

   EXIT FUNCTION

   WRITE.IRF.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                        ! FMW
   EXIT FUNCTION

  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.IRF.UNLOCK PUBLIC

   INTEGER*2 WRITE.IRF.UNLOCK

    WRITE.IRF.UNLOCK = 1

    CALL IRF.CONCAT.RECORD                                                ! 1.6 RC

    IF END#IRF.SESS.NUM% THEN WRITE.IRF.UNLOCK.ERROR

    WRITE FORM "C11,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; \               ! 1.6 RC ! 2.0 AJC ! 2.2 TT
         #IRF.SESS.NUM% AUTOUNLOCK;                               \ MW96A
             IRF.BAR.CODE$,                                       \
             IRF.INDICAT0%,                                       \
             IRF.INDICAT1%,                                       \
             IRF.DEAL.DATA%(0),                                   \       ! 1.6 RC
             IRF.INDICAT8%,                                       \       ! 2.0 AJC
\!            IRF.UNUSED$,                                        \       ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                       \       ! 2.2 TT
             IRF.INDICAT10%,                                      \       ! 2.2 TT
             IRF.SALEPRIC$,                                       \
             IRF.INDICAT5%,                                       \ SBH 31/1/96
             IRF.ITEMNAME$,                                       \
             IRF.BOOTS.CODE$,                                     \
             IRF.DEAL.DATA%(1),                                   \       ! 1.6 RC
             IRF.DEAL.DATA%(2),                                   \       ! 1.6 RC
             IRF.INDICAT3%

    IF WRITE.IRFDEX THEN EXIT FUNCTION                                    ! 1.9 SM

    WRITE.IRF.UNLOCK = 0

   EXIT FUNCTION

   WRITE.IRF.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                        ! FMW

   EXIT FUNCTION

  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.IRF.HOLD PUBLIC

   INTEGER*2 WRITE.IRF.HOLD

    WRITE.IRF.HOLD = 1

    CALL IRF.CONCAT.RECORD                                                ! 1.6 RC

     IF END#IRF.SESS.NUM% THEN WRITE.IRF.HOLD.ERROR

     WRITE FORM "C11,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; HOLD \         ! 1.6 RC ! 2.0 AJC ! 2.2 TT
             #IRF.SESS.NUM%;                                      \
             IRF.BAR.CODE$,                                       \
             IRF.INDICAT0%,                                       \
             IRF.INDICAT1%,                                       \
             IRF.DEAL.DATA%(0),                                   \       ! 1.6 RC
             IRF.INDICAT8%,                                       \       ! 2.0 AJC
\!            IRF.UNUSED$,                                        \       ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                       \       ! 2.2 TT
             IRF.INDICAT10%,                                      \       ! 2.2 TT
             IRF.SALEPRIC$,                                       \
             IRF.INDICAT5%,                                       \ SBH 31/1/96
             IRF.ITEMNAME$,                                       \
             IRF.BOOTS.CODE$,                                     \
             IRF.DEAL.DATA%(1),                                   \       ! 1.6 RC
             IRF.DEAL.DATA%(2),                                   \       ! 1.6 RC
             IRF.INDICAT3%

    IF WRITE.IRFDEX THEN EXIT FUNCTION                                    ! 1.9 SM

    WRITE.IRF.HOLD = 0

   EXIT FUNCTION

   WRITE.IRF.HOLD.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                        ! FMW

   EXIT FUNCTION

  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.IRF.HOLD.UNLOCK PUBLIC

   INTEGER*2 WRITE.IRF.HOLD.UNLOCK

   WRITE.IRF.HOLD.UNLOCK = 1

    CALL IRF.CONCAT.RECORD                                                ! 1.6 RC

    IF END#IRF.SESS.NUM% THEN WRITE.IRF.HOLD.UNLOCK.ERROR

    WRITE FORM "C11,2I1,I2,I1,2I1,C5,I1,C18,C3,I2,I2,I1"; HOLD \          ! 1.6 RC ! 2.0 AJC ! 2.2 TT
             #IRF.SESS.NUM% AUTOUNLOCK;                           \
             IRF.BAR.CODE$,                                       \
             IRF.INDICAT0%,                                       \
             IRF.INDICAT1%,                                       \
             IRF.DEAL.DATA%(0),                                   \       ! 1.6 RC
             IRF.INDICAT8%,                                       \       ! 2.0 AJC
\!            IRF.UNUSED$,                                        \       ! 1.6 RC 2.2 TT
             IRF.INDICAT9%,                                       \       ! 2.2 TT
             IRF.INDICAT10%,                                      \       ! 2.2 TT
             IRF.SALEPRIC$,                                       \
             IRF.INDICAT5%,                                       \ SBH 31/1/96
             IRF.ITEMNAME$,                                       \
             IRF.BOOTS.CODE$,                                     \
             IRF.DEAL.DATA%(1),                                   \       ! 1.6 RC
             IRF.DEAL.DATA%(2),                                   \       ! 1.6 RC
             IRF.INDICAT3%

    IF WRITE.IRFDEX THEN EXIT FUNCTION                                    ! 1.9 SM

    WRITE.IRF.HOLD.UNLOCK = 0

   EXIT FUNCTION

   WRITE.IRF.HOLD.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = RIGHT$(IRF.BAR.CODE$,7)                        ! FMW

   EXIT FUNCTION

  END FUNCTION

