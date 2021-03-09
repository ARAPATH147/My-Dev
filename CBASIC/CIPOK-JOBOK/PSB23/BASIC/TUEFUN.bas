\*****************************************************************************
\***              SHELF-EDGE LABEL (TUESDAY) FILE FUNCTIONS 
\***      Version A           Steve Windsor             5th Jan 1993
\***
\***      Version B           Jamie Thorpe                7th Jun 1999
\***      Removed version lettering.
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE TUEDEC.J86               ! BJT

FUNCTION TUE.SET PUBLIC

    TUE.REPORT.NUM%  = 23   
    TUE.FILE.NAME$   = "STUE"
END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.TUE PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.TUE

   READ.TUE = 1

   IF END#TUE.SESS.NUM% THEN READ.TUE.ERROR

    READ #TUE.SESS.NUM%; RPRT.LINE$

   READ.TUE = 0
   EXIT FUNCTION

   READ.TUE.ERROR:

   CURRENT.REPORT.NUM% = TUE.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.TUE PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.TUE

   WRITE.TUE = 1

   IF END#TUE.SESS.NUM% THEN WRITE.TUE.ERROR

    WRITE #TUE.SESS.NUM%; RPRT.LINE$

   WRITE.TUE = 0
   EXIT FUNCTION

   WRITE.TUE.ERROR:

   CURRENT.REPORT.NUM% = TUE.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

