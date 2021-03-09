\*****************************************************************************
\*****************************************************************************
\***
\***                SHELF-EDGE LABEL (THURSDAY) FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor              5th Jan 1993
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

   %INCLUDE THUDEC.J86               !BJT


   FUNCTION THU.SET PUBLIC
REM \

    THU.REPORT.NUM%  = 25 
    THU.FILE.NAME$   = "STHU"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.THU PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 READ.THU

   READ.THU = 1

   IF END#THU.SESS.NUM% THEN READ.THU.ERROR

    READ #THU.SESS.NUM%; RPRT.LINE$

   READ.THU = 0
   EXIT FUNCTION

   READ.THU.ERROR:

   CURRENT.REPORT.NUM% = THU.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.THU PUBLIC

   STRING RPRT.LINE$ 
   INTEGER*2 WRITE.THU

   WRITE.THU = 1

   IF END#THU.SESS.NUM% THEN WRITE.THU.ERROR

    WRITE #THU.SESS.NUM%; RPRT.LINE$

   WRITE.THU = 0
   EXIT FUNCTION

   WRITE.THU.ERROR:

   CURRENT.REPORT.NUM% = THU.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

