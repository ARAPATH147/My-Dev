
\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR PRODUCT GROUP FILES PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   PGFDEC (J86)
\***
\***        FILE TYPE   :   Direct (PGF and PGFD)
\***                        Keyed (PGFO)
\***
\***    VERSION B.              ROBERT COWEY.                       30 SEP 1993.
\***    Created by combining PGFFUNA, PGFDFNS and PGFOFNS.
\***
\***    REVISION 1.2.           ROBERT COWEY.                       27 OCT 2003.
\***    Defined WRITE.PGF function.
\***
\***    Version C               Mark Goode                          21 DEC 2004.
\***    Additional flag for product group marked OSSR.
\***
\***    Version D               Tittoo Thomas                        2 SEP 2011
\***    Changes to accomodate new csv format for PGDIR as part of
\***    the Core Stores Release 2 project
\***
\***    Version E               Mark Goode                           16 Nov 2011
\***    Defect 2512 - Changed the read # to a read form #, when
\***                  checking the format of the PGFDIR file.
\***
\***    Version F               Mark Goode                           05 Dec 2011
\***    Defect 2835 - Changes to the READ.PGFDIR. This now returns the record in
\***    the new format regardless, if the PGF direct file is in the old or new
\***    format.
\***
\***    Version G               Charles Skadorwa                     28 Dec 2011
\***    Defect 3329 - Change to READ.PGFDIR. In the new format PGFXX, the Product
\***    Group requires an extra zero adding to the Concept Group ie. PP0CCC. This
\***    is required as the Product Group is packed later and needs to be an even
\***    number of digits. This is not required for the old format.
\***
\***    Also changed WRITE.PGFDIR. The code was transposed for the new and old
\***    formats - not picked up as not used by any code.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE PGFDEC.J86  ! PGF variable declarations
    %INCLUDE PSBF20G.J86 ! Globals for File session allocation utility   ! DTT

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    STRING \                                                             ! DTT
        PGFDIR.MATRIX$(1), \                                             ! DTT
        PGFDIR.RECORD$                                                   ! DTT

    INTEGER*1 GLOBAL \                                                   ! DTT
        PGFDIR.NEW.FORMAT%                                               ! DTT

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%

    %INCLUDE PSBF20E.J86 ! External funcs for File session alloc util    ! DTT

FUNCTION PGF.SET PUBLIC

    INTEGER*2 PGF.SET
    PGF.SET EQ 1

    PGF.FILE.NAME$     EQ "PGF"
    PGFDIR.FILE.NAME$  EQ "PGFXX"
    PGFO.FILE.NAME$    EQ "PGFO"

    PGF.REPORT.NUM%    EQ 10
    PGFDIR.REPORT.NUM% EQ 52
    PGFO.REPORT.NUM%   EQ 51

    PGF.RECL%          EQ 30
    PGFDIR.RECL%       EQ 30
    PGFO.RECL%         EQ 30

    PGFDIR.NEW.FORMAT% EQ 0                                             ! DTT


    !---------------------------------------------------------
    ! Open the PGFDIR, read the first record, then close the file
    !---------------------------------------------------------
    CALL SESS.NUM.UTILITY("O", PGFDIR.REPORT.NUM%, PGFDIR.FILE.NAME$)   ! DTT
    PGFDIR.SESS.NUM% = F20.INTEGER.FILE.NO%                             ! DTT

    IF END # PGFDIR.SESS.NUM% THEN EXIT.PGF.SET                         ! DTT
    OPEN PGFDIR.FILE.NAME$ AS PGFDIR.SESS.NUM%      \                   ! DTT
         BUFFSIZE 32256 LOCKED NOWRITE NODEL                            ! DTT

    FILE.OPERATION$ = "R"                                               ! DTT

    IF END # PGFDIR.SESS.NUM% THEN EXIT.PGF.SET                         ! DTT
    READ FORM "C2";#PGFDIR.SESS.NUM%; PGFDIR.RECORD$                    ! EMG ! DTT
    CLOSE PGFDIR.SESS.NUM%                                              ! DTT

    !------------------------------------------------
    ! If the record is a header in old or new format
    !------------------------------------------------
    IF PGFDIR.RECORD$ = "P," THEN BEGIN                                 ! EMG ! DTT
       PGFDIR.NEW.FORMAT% = -1                                          ! DTT
    ENDIF ELSE BEGIN                                                    ! DTT
       PGFDIR.NEW.FORMAT% = 0                                           ! DTT
    ENDIF                                                               ! DTT

    PGF.SET EQ 0

EXIT.PGF.SET:                                                           ! DTT

END FUNCTION


FUNCTION READ.PGF PUBLIC

    INTEGER*2 READ.PGF
    READ.PGF EQ 1

    IF END # PGF.SESS.NUM% THEN READ.PGF.IF.END
    READ FORM "T4,C18,2C1,C7"; \   CMG
      # PGF.SESS.NUM% \
        KEY PGF.PROD.GRP.NO$;   \
            PGF.PROD.GRP.NAME$, \
            PGF.SEL.FLAG$,      \
            PGF.OSSR.FLAG$,     \  CMG
            PGF.SPACE$

    READ.PGF EQ 0
    EXIT FUNCTION

READ.PGF.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ PGF.REPORT.NUM%
    CURRENT.CODE$       EQ PACK$("00000000") + PGF.PROD.GRP.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.PGF PUBLIC ! Entire function new for revision 1.2        ! 1.2 RC

    INTEGER*2 WRITE.PGF
    WRITE.PGF EQ 1

    IF END # PGF.SESS.NUM% THEN WRITE.PGF.IF.END
    WRITE FORM "C3,C18,2C1,C7"; \  CMG
      # PGF.SESS.NUM% ;\
            PGF.PROD.GRP.NO$,   \
            PGF.PROD.GRP.NAME$, \
            PGF.SEL.FLAG$,      \
            PGF.OSSR.FLAG$,     \  CMG
            PGF.SPACE$

    WRITE.PGF EQ 0
    EXIT FUNCTION

WRITE.PGF.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ PGF.REPORT.NUM%
    CURRENT.CODE$       EQ PACK$("00000000") + PGF.PROD.GRP.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.PGFDIR PUBLIC

    INTEGER*2 READ.PGFDIR
    READ.PGFDIR EQ 1

    IF PGFDIR.NEW.FORMAT% THEN BEGIN                                    ! DTT

        DIM PGFDIR.MATRIX$(5)                                           ! DTT
        IF END # PGFDIR.SESS.NUM% THEN READ.PGFDIR.IF.END               ! DTT
        READ MATRIX #PGFDIR.SESS.NUM%; PGFDIR.MATRIX$(1), 5             ! DTT
        PGFDIR.REC.TYPE$   = PGFDIR.MATRIX$(1)                          ! DTT
        PGF.PROD.GRP.NO$   = PGFDIR.MATRIX$(2)                          ! DTT

        ! Add extra zero to Concept Group so that field can be packed   ! FCS
        PGF.PROD.GRP.NO$ = LEFT$(PGF.PROD.GRP.NO$,2)  + \               ! FCS
                           "0"                        + \               ! FCS
                           RIGHT$(PGF.PROD.GRP.NO$,3)                   ! FCS


        PGF.PROD.GRP.NAME$ = PGFDIR.MATRIX$(3)                          ! DTT
        PGF.SEL.FLAG$      = PGFDIR.MATRIX$(4)                          ! DTT
        PGF.OSSR.FLAG$     = PGFDIR.MATRIX$(5)                          ! DTT

    ENDIF ELSE BEGIN                                                    ! DTT

        IF END # PGFDIR.SESS.NUM% THEN READ.PGFDIR.IF.END
        READ FORM "C3,C18,2C1,C7"; \  CMG
           # PGFDIR.SESS.NUM%; \
              PGF.PROD.GRP.NO$, \
              PGF.PROD.GRP.NAME$, \
              PGF.SEL.FLAG$, \
              PGF.OSSR.FLAG$, \ CMG
              PGF.SPACE$

        PGFDIR.REC.TYPE$ = ""                                           ! FMG
        PGF.PROD.GRP.NO$ = UNPACK$(PGF.PROD.GRP.NO$)                    ! FMG

    ENDIF                                                               ! DTT

    READ.PGFDIR EQ 0
    EXIT FUNCTION

READ.PGFDIR.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ  PGFDIR.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(PGFDIR.REC.NO%),14))

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.PGFDIR PUBLIC

    INTEGER*2 WRITE.PGFDIR
    WRITE.PGFDIR EQ 1

    IF PGFDIR.NEW.FORMAT% THEN BEGIN                                    ! DTT
        PGFDIR.MATRIX$(1) = PGFDIR.REC.TYPE$                            ! DTT
        PGFDIR.MATRIX$(2) = PGF.PROD.GRP.NO$                            ! DTT
        PGFDIR.MATRIX$(3) = PGF.PROD.GRP.NAME$                          ! DTT
        PGFDIR.MATRIX$(4) = PGF.SEL.FLAG$                               ! DTT
        PGFDIR.MATRIX$(5) = PGF.OSSR.FLAG$                              ! DTT
        IF END # PGFDIR.SESS.NUM% THEN WRITE.PGFDIR.IF.END              ! DTT
        WRITE MATRIX #PGFDIR.SESS.NUM%; PGFDIR.MATRIX$(1), 5            ! DTT

    ENDIF ELSE BEGIN                                                    ! DTT
        IF END # PGFDIR.SESS.NUM% THEN WRITE.PGFDIR.IF.END
        WRITE FORM "C3,C18,2C1,C7"; \    CMG
          # PGFDIR.SESS.NUM%, \
            PGFDIR.REC.NO%; \
              PGF.PROD.GRP.NO$, \
              PGF.PROD.GRP.NAME$, \
              PGF.SEL.FLAG$, \
              PGF.OSSR.FLAG$, \ CMG
              PGF.SPACE$
    ENDIF                                                               ! DTT

    WRITE.PGFDIR EQ 0
    EXIT FUNCTION

WRITE.PGFDIR.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  PGFDIR.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(PGFDIR.REC.NO%),14))

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.PGFO PUBLIC

    INTEGER*2 READ.PGFO
    READ.PGFO EQ 1

    IF END # PGFO.SESS.NUM% THEN READ.PGFO.IF.END
    READ FORM "T4,C18,2C1,C7"; \    CMG
      # PGFO.SESS.NUM% \
        KEY PGF.PROD.GRP.NO$; \
            PGF.PROD.GRP.NAME$, \
            PGF.SEL.FLAG$, \
            PGF.OSSR.FLAG$, \ CMG
            PGF.SPACE$

    READ.PGFO EQ 0
    EXIT FUNCTION

READ.PGFO.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ PGFO.REPORT.NUM%
    CURRENT.CODE$       EQ PACK$("00000000") + PGF.PROD.GRP.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.PGFO PUBLIC

    INTEGER*2 WRITE.PGFO
    WRITE.PGFO EQ 1

    IF END # PGFO.SESS.NUM% THEN WRITE.PGFO.IF.END
    WRITE FORM "C3,C18,2C1,C7"; \   CMG
      # PGFO.SESS.NUM%; \
          PGF.PROD.GRP.NO$, \
          PGF.PROD.GRP.NAME$, \
          PGF.SEL.FLAG$, \
          PGF.OSSR.FLAG$, \ CMG
          PGF.SPACE$

    WRITE.PGFO EQ 0
    EXIT FUNCTION

WRITE.PGFO.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ PGFO.REPORT.NUM%
    CURRENT.CODE$       EQ PACK$("00000000") + PGF.PROD.GRP.NO$

    EXIT FUNCTION

END FUNCTION

