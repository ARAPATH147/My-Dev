\*****************************************************************************
\*****************************************************************************
\***
\***           TILLBAG FILE FUNCTIONS
\***
\***           REFERENCE  : BTCSFUN
\***
\***           VERSION A  : JULIA STONES 23/10/03
\***
\***
\*****************************************************************************
\*****************************************************************************

    INTEGER*2 GLOBAL      CURRENT.REPORT.NUM%


    STRING GLOBAL         CURRENT.CODE$,                 \
                          FILE.OPERATION$


    %INCLUDE              BTCSDEC.J86

\*****************************************************************************

    FUNCTION BTCS.SET PUBLIC

! Full file name will be added by PSS37 (Either be BTCSK.??? for a count,
!                                        BTCSF.??? for a book in,
! for both files ext ??? is the next number 001 - 999 taken from the SSPSCTRL
! control file

        BTCS.FILE.NAME$  = "ADXLXACN::D:\ADX_UDT1\BTCS"
        BTCS.REPORT.NUM% = 665
        BTCS.RECL%       = 42

    END FUNCTION

\*****************************************************************************

    SUB BTCS.SPLIT

        BTCS.RECORD.TYPE$ = MID$(BTCS.RECORD$,36, 1)

        IF BTCS.RECORD.TYPE$ = "H" THEN BEGIN

            BTCS.STORE.NUMBER$      = LEFT$(BTCS.RECORD$,   4)
            BTCS.STKTAKE.NUM$   = MID$(BTCS.RECORD$,  5, 4)
            BTCS.DATE$          = MID$(BTCS.RECORD$,  9, 6)
            BTCS.TIME$          = MID$(BTCS.RECORD$, 15, 6)
            BTCS.DISP.AREA$     = MID$(BTCS.RECORD$, 21, 1)
            BTCS.FILLER$        = MID$(BTCS.RECORD$, 22,14)
            BTCS.NUM.RECORD$    = MID$(BTCS.RECORD$, 36, 4)
            BTCS.ENDREC$        = RIGHT$(BTCS.RECORD$,   2)

        ENDIF ELSE IF BTCS.RECORD.TYPE$ = "D" THEN BEGIN

            BTCS.ITEM.CODE$     = LEFT$(BTCS.RECORD$,  13)
            BTCS.CODE.TYPE$     = MID$(BTCS.RECORD$, 14,  1)
            BTCS.PACK.QTY$      = MID$(BTCS.RECORD$, 15,  6)
            BTCS.DIS.UNIT.QTY$  = MID$(BTCS.RECORD$, 21,  4)
            BTCS.FILLER$        = MID$(BTCS.RECORD$, 25, 11)
            BTCS.NUM.RECORD$    = MID$(BTCS.RECORD$, 36,  4)
            BTCS.ENDREC$        = RIGHT$(BTCS.RECORD$,    2)

        ENDIF ELSE IF BTCS.RECORD.TYPE$ = "T" THEN BEGIN

            BTCS.RECORD.COUNT$  = LEFT$(BTCS.RECORD$,   4)
            BTCS.FILLER$        = MID$(BTCS.RECORD$,  5, 31)
            BTCS.NUM.RECORD$    = MID$(BTCS.RECORD$, 36,  4)
            BTCS.ENDREC$        = RIGHT$(BTCS.RECORD$,    2)


        ENDIF

    END SUB

\*****************************************************************************

    FUNCTION READ.BTCS PUBLIC

    INTEGER*1 READ.BTCS

        READ.BTCS = 1

        IF END # BTCS.SESS.NUM% THEN READ.BTCS.ERROR

        READ FORM "C42";                                 \
             # BTCS.SESS.NUM%;                           \
               BTCS.RECORD$

        CALL BTCS.SPLIT

        READ.BTCS = 0

    EXIT FUNCTION

READ.BTCS.ERROR:

    CURRENT.REPORT.NUM% = BTCS.REPORT.NUM%
    FILE.OPERATION$     = "R"
    CURRENT.CODE$       = PACK$("00000000000000")

    END FUNCTION

\*****************************************************************************

    FUNCTION WRITE.BTCS PUBLIC

    INTEGER*1 WRITE.BTCS

        WRITE.BTCS = 1

        IF END # BTCS.SESS.NUM% THEN WRITE.BTCS.ERROR

        IF BTCS.RECORD.TYPE$ = "H" THEN BEGIN

           WRITE FORM "C4,C4,C6,C6,C1,C14,C1,C4,C2";        \
                 # BTCS.SESS.NUM%, BTCS.REC.NUM%;           \
                   BTCS.STORE.NUMBER$,                      \
                   BTCS.STKTAKE.NUM$,                       \
                   BTCS.DATE$,                              \
                   BTCS.TIME$,                              \
                   BTCS.DISP.AREA$,                         \
                   BTCS.FILLER$,                            \
                   BTCS.RECORD.TYPE$,                       \
                   BTCS.NUM.RECORD$,                        \
                   BTCS.ENDREC$

        ENDIF ELSE IF BTCS.RECORD.TYPE$ = "D" THEN BEGIN

           WRITE FORM "C13,C1,C6,C4,C11,C1,C4,C2";          \
                 # BTCS.SESS.NUM%, BTCS.REC.NUM%;           \
                   BTCS.ITEM.CODE$,                         \
                   BTCS.CODE.TYPE$,                         \
                   BTCS.PACK.QTY$,                          \
                   BTCS.DIS.UNIT.QTY$,                      \
                   BTCS.FILLER$,                            \
                   BTCS.RECORD.TYPE$,                       \
                   BTCS.NUM.RECORD$,                        \
                   BTCS.ENDREC$

        ENDIF ELSE IF BTCS.RECORD.TYPE$ = "T" THEN BEGIN

           WRITE FORM "C4,C31,C1,C4,C2";                    \
                 # BTCS.SESS.NUM%, BTCS.REC.NUM%;           \
                   BTCS.RECORD.COUNT$,                      \
                   BTCS.FILLER$,                            \
                   BTCS.RECORD.TYPE$,                       \
                   BTCS.NUM.RECORD$,                        \
                   BTCS.ENDREC$

        ENDIF ELSE BEGIN

        GOTO WRITE.BTCS.ERROR:

        ENDIF

        WRITE.BTCS = 0

    EXIT FUNCTION

WRITE.BTCS.ERROR:

    CURRENT.REPORT.NUM% = BTCS.REPORT.NUM%
    FILE.OPERATION$     = "W"
    CURRENT.CODE$       = PACK$("00000000000000")

    END FUNCTION

\*****************************************************************************

