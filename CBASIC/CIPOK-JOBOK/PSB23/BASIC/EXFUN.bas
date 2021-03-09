
\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR EXCHANGE RATE FILE PUBLIC FUNCTIONS
\***
\***        REFERENCE   :  EXFUN (BAS)
\***
\***        FILE TYPE   :  DIRECT
\***
\***    VERSION A.              Steve Hughes.                       23 JAN 1996.
\***    Original version.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE EXDEC.J86 ! DETS variable declarations

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION EX.SET PUBLIC

    INTEGER*2 EX.SET
    EX.SET EQ 1

    EX.FILENAME.NAME$  EQ "EXRATE" ! Suffix needed to define individual files
    EX.REPORT.NUM%   EQ  465
    EX.RECL%         EQ  64

    EX.SET EQ 0

END FUNCTION


FUNCTION READ.EX PUBLIC

    INTEGER*2 READ.EX
    READ.EX EQ 1

    IF END # EX.SESS.NUM% THEN READ.EX.IF.END
    READ FORM "I1,C14,I1,R,C30,C3,I2,C3"; \
      # EX.SESS.NUM%,         \
        EX.REC.NUM%;          \
          EX.FLAGS, \
          EX.OP.NAME$, \
          EX.DEC.PLACES, \
          EX.EXCH.RATE, \
          EX.NAME$, \
          EX.DATE$, \
          EX.ACCEPT%,\
          EX.UNUSED$

    READ.EX EQ 0
    EXIT FUNCTION

READ.EX.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ  EX.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("000000" + STR$(EX.REC.NUM%),6))

END FUNCTION


FUNCTION WRITE.EX PUBLIC

    INTEGER*2 WRITE.EX
    WRITE.EX EQ 1

    IF END # EX.SESS.NUM% THEN WRITE.EX.ERROR
    WRITE FORM "I1,C14,I1,R,C30,C3,I2,C3"; \
      # EX.SESS.NUM%,         \
        EX.REC.NUM%;          \
          EX.FLAGS, \
          EX.OP.NAME$, \
          EX.DEC.PLACES, \
          EX.EXCH.RATE, \
          EX.NAME$, \
          EX.DATE$, \
          EX.ACCEPT%, \
          EX.UNUSED$

    WRITE.EX EQ 0
    EXIT FUNCTION

WRITE.EX.ERROR:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  EX.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("000000" + STR$(EX.REC.NUM%),6))

    EXIT FUNCTION

END FUNCTION

