\*****************************************************************************
\*****************************************************************************
\***
\***                  Shelf_edge label (Friday)  FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor               5th January 1993
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

   %INCLUDE FRIDEC.J86               ! BJT


   FUNCTION FRI.SET PUBLIC

    FRI.REPORT.NUM%  = 26
    FRI.FILE.NAME$   = "SFRI"
END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.FRI PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.FRI

   READ.FRI = 1

   IF END#FRI.SESS.NUM% THEN READ.FRI.ERROR

    READ #FRI.SESS.NUM%; RPRT.LINE$

   READ.FRI = 0
   EXIT FUNCTION

   READ.FRI.ERROR:

   CURRENT.REPORT.NUM% = FRI.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = "" 

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.FRI PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.FRI

   WRITE.FRI = 1

   IF END#FRI.SESS.NUM% THEN WRITE.FRI.ERROR

    WRITE #FRI.SESS.NUM%; RPRT.LINE$

   WRITE.FRI = 0
   EXIT FUNCTION

   WRITE.FRI.ERROR:

   CURRENT.REPORT.NUM% = FRI.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = "" 

   EXIT FUNCTION
  END FUNCTION

