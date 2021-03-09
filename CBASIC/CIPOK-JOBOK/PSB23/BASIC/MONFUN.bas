\*****************************************************************************
\***            Shelf-edge label (Monday) file functions 
\***      Version A           Steve Windsor               5th Jan 1993
\***
\***      Version B           Jamie Thorpe                7th Jun 1999
\***      Removed version lettering.
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE MONDEC.J86               ! BJT


   FUNCTION MON.SET PUBLIC

    MON.REPORT.NUM%  = 22  
    MON.FILE.NAME$   = "SMON"
END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.MON PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.MON

   READ.MON = 1

   IF END#MON.SESS.NUM% THEN READ.MON.ERROR

    READ #MON.SESS.NUM%; RPRT.LINE$

   READ.MON = 0
   EXIT FUNCTION

   READ.MON.ERROR:

   CURRENT.REPORT.NUM% = MON.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.MON PUBLIC

   STRING RPRT.LINE$  
   INTEGER*2 WRITE.MON

   WRITE.MON = 1

   IF END#MON.SESS.NUM% THEN WRITE.MON.ERROR

    WRITE #MON.SESS.NUM%; RPRT.LINE$

   WRITE.MON = 0
   EXIT FUNCTION

   WRITE.MON.ERROR:

   CURRENT.REPORT.NUM% = MON.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

