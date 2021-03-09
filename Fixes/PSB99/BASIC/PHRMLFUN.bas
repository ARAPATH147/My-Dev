
\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR PHARMACY LOCAL FILES PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   PHRMLFU (BAS)
\***
\***        FILE TYPE   :   Accessed as direct
\***
\***    VERSION A.              ROBERT COWEY.                       24 MAR 1994.
\***    Original version dealing with Pharmacy password field only.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE PHRMLDEC.J86 ! PHRML variable declarations

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION PHRML.SET PUBLIC

    INTEGER*2 PHRML.SET
    PHRML.SET EQ 1

    PHRML.FILE.NAME$  EQ "PHRML"
    PHRML.REPORT.NUM% EQ  150
    PHRML.RECL%       EQ  9

    PHRML.SET EQ 0

END FUNCTION


FUNCTION WRITE.PHRML.PASSWORD PUBLIC

    INTEGER*2 WRITE.PHRML.PASSWORD
    WRITE.PHRML.PASSWORD EQ 1

    PHRML.REC.NUM% EQ 3612

    IF END # PHRML.SESS.NUM% THEN WRITE.PHRML.IF.END
    WRITE FORM "C9"; \
      # PHRML.SESS.NUM%, \
        PHRML.REC.NUM%; \
          PHRML.PASSWORD$

    WRITE.PHRML.PASSWORD EQ 0
    EXIT FUNCTION

WRITE.PHRML.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  PHRML.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(PHRML.REC.NUM%),14))

    EXIT FUNCTION

END FUNCTION

