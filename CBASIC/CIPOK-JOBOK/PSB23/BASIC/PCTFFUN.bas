\*****************************************************************************
\*****************************************************************************
\***
\***            TODAYS PRICE CHANGE REPORT FILE FUNCTIONS 
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

   %INCLUDE PCTFDEC.J86


   FUNCTION PCTF.SET PUBLIC
REM \

    PCTF.REPORT.NUM%  = 20  
    PCTF.FILE.NAME$   = "PCTF"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.PCTF PUBLIC

   STRING RPRT.LINE$  
   INTEGER*2 READ.PCTF

   READ.PCTF = 1

   IF END#PCTF.SESS.NUM% THEN READ.PCTF.ERROR

    READ #PCTF.SESS.NUM%; RPRT.LINE$

   READ.PCTF = 0
   EXIT FUNCTION

   READ.PCTF.ERROR:

   CURRENT.REPORT.NUM% = PCTF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.PCTF PUBLIC

   STRING RPRT.LINE$  
   INTEGER*2 WRITE.PCTF

   WRITE.PCTF = 1

   IF END#PCTF.SESS.NUM% THEN WRITE.PCTF.ERROR

    WRITE #PCTF.SESS.NUM%; RPRT.LINE$

   WRITE.PCTF = 0
   EXIT FUNCTION

   WRITE.PCTF.ERROR:

   CURRENT.REPORT.NUM% = PCTF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

