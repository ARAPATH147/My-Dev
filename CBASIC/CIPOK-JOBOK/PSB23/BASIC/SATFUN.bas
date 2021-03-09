\*****************************************************************************
\*****************************************************************************
\***
\***            SHELF-EDGE LABEL (SATURDAY) FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor               5th Jan 1993
\***
\***      Version B           Jamie Thorpe                7th Jun 1999
\***      Removed version lettering.
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE SATDEC.J86               ! BJT


   FUNCTION SAT.SET PUBLIC
REM \

    SAT.REPORT.NUM%  = 27  
    SAT.FILE.NAME$   = "SSAT"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.SAT PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.SAT

   READ.SAT = 1

   IF END#SAT.SESS.NUM% THEN READ.SAT.ERROR

    READ #SAT.SESS.NUM%; RPRT.LINE$

   READ.SAT = 0
   EXIT FUNCTION

   READ.SAT.ERROR:

   CURRENT.REPORT.NUM% = SAT.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.SAT PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.SAT

   WRITE.SAT = 1

   IF END#SAT.SESS.NUM% THEN WRITE.SAT.ERROR

    WRITE #SAT.SESS.NUM%; RPRT.LINE$

   WRITE.SAT = 0
   EXIT FUNCTION

   WRITE.SAT.ERROR:

   CURRENT.REPORT.NUM% = SAT.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

