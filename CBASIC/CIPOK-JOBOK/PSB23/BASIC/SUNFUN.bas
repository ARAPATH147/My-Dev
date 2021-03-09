\*****************************************************************************
\*****************************************************************************
\***
\***                SHELF-EDGE LABEL (SUNDAY) FILE FUNCTIONS 
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

   %INCLUDE SUNDEC.J86               ! BJT


   FUNCTION SUN.SET PUBLIC
REM \

    SUN.REPORT.NUM%  = 49  
    SUN.FILE.NAME$   = "SSUN"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.SUN PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.SUN

   READ.SUN = 1

   IF END#SUN.SESS.NUM% THEN READ.SUN.ERROR

    READ #SUN.SESS.NUM%; RPRT.LINE$

   READ.SUN = 0
   EXIT FUNCTION

   READ.SUN.ERROR:

   CURRENT.REPORT.NUM% = SUN.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.SUN PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.SUN

   WRITE.SUN = 1

   IF END#SUN.SESS.NUM% THEN WRITE.SUN.ERROR

    WRITE #SUN.SESS.NUM%; RPRT.LINE$

   WRITE.SUN = 0
   EXIT FUNCTION

   WRITE.SUN.ERROR:

   CURRENT.REPORT.NUM% = SUN.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

