\*****************************************************************************
\***               SHELF-EDGE LABEL (WEDNESDAY) FILE FUNCTIONS 
\***      Version A           Steve Windsor              5th Jan 1993
\***
\***      Version B           Jamie Thorpe                7th Jun 1999
\***      Removed version lettering.
\***
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE WEDDEC.J86               ! BJT

   FUNCTION WED.SET PUBLIC

    WED.REPORT.NUM%  = 24
    WED.FILE.NAME$   = "SWED"
END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.WED PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.WED

   READ.WED = 1

   IF END#WED.SESS.NUM% THEN READ.WED.ERROR

    READ #WED.SESS.NUM%; RPRT.LINE$

   READ.WED = 0
   EXIT FUNCTION

   READ.WED.ERROR:

   CURRENT.REPORT.NUM% = WED.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.WED PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.WED

   WRITE.WED = 1

   IF END#WED.SESS.NUM% THEN WRITE.WED.ERROR

    WRITE #WED.SESS.NUM%; RPRT.LINE$

   WRITE.WED = 0
   EXIT FUNCTION

   WRITE.WED.ERROR:

   CURRENT.REPORT.NUM% = WED.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

