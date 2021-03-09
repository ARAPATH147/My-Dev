\*****************************************************************************
\*****************************************************************************
\***
\***            PRICE CHANGE SUMMARY REPORT FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor               5th Jan 1993
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE PCSFDEC.J86


   FUNCTION PCSF.SET PUBLIC
REM \

    PCSF.REPORT.NUM%  = 19   
    PCSF.FILE.NAME$   = "PCSF"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.PCSF PUBLIC

   STRING RPRT.LINE$  
   INTEGER*2 READ.PCSF

   READ.PCSF = 1

   IF END#PCSF.SESS.NUM% THEN READ.PCSF.ERROR

    READ #PCSF.SESS.NUM%; RPRT.LINE$

   READ.PCSF = 0
   EXIT FUNCTION

   READ.PCSF.ERROR:

   CURRENT.REPORT.NUM% = PCSF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.PCSF PUBLIC

   STRING RPRT.LINE$  
   INTEGER*2 WRITE.PCSF

   WRITE.PCSF = 1

   IF END#PCSF.SESS.NUM% THEN WRITE.PCSF.ERROR

    WRITE #PCSF.SESS.NUM%; RPRT.LINE$

   WRITE.PCSF = 0
   EXIT FUNCTION

   WRITE.PCSF.ERROR:

   CURRENT.REPORT.NUM% = PCSF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

