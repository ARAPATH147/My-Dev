
\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR PHARMACY PASSWORD DETAILS FILES PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   PPDFFUN (BAS)
\***
\***        FILE TYPE   :   Direct
\***
\***    VERSION A.              ROBERT COWEY.                       21 FEB 1994.
\***    Original version.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE PPDFDEC.J86 ! PPDF variable declarations

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION PPDF.SET PUBLIC

    INTEGER*2 PPDF.SET
    PPDF.SET EQ 1

    PPDF.FILE.NAME$  EQ "PPDF"
    PPDF.REPORT.NUM% EQ  385
    PPDF.RECL%       EQ  16

    PPDF.SET EQ 0

END FUNCTION


FUNCTION READ.PPDF PUBLIC

    INTEGER*2 READ.PPDF
    READ.PPDF EQ 1

    IF END # PPDF.SESS.NUM% THEN READ.PPDF.IF.END
    READ FORM "C3,C1,C1,C11"; \
      # PPDF.SESS.NUM%, \
        PPDF.REC.NUM%; \
          PPDF.DATE.LAST.PSWD$, \
          PPDF.PSWD.DURATION$, \
          PPDF.INTERMEDIATE.FLAG$, \
          PPDF.FILLER$

    READ.PPDF EQ 0
    EXIT FUNCTION

READ.PPDF.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ  PPDF.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(PPDF.REC.NUM%),14))

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.PPDF PUBLIC

    INTEGER*2 WRITE.PPDF
    WRITE.PPDF EQ 1

    IF END # PPDF.SESS.NUM% THEN WRITE.PPDF.IF.END
    WRITE FORM "C3,C1,C1,C11"; \
      # PPDF.SESS.NUM%, \
        PPDF.REC.NUM%; \
          PPDF.DATE.LAST.PSWD$, \
          PPDF.PSWD.DURATION$, \
          PPDF.INTERMEDIATE.FLAG$, \
          PPDF.FILLER$

    WRITE.PPDF EQ 0
    EXIT FUNCTION

WRITE.PPDF.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  PPDF.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(PPDF.REC.NUM%),14))

    EXIT FUNCTION

END FUNCTION

