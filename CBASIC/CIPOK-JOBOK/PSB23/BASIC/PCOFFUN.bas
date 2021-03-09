\*****************************************************************************
\*****************************************************************************
\***
\***               OVERDUE PRICE CHANGES REPORT FUNCTIONS 
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

   %INCLUDE PCOFDEC.J86


   FUNCTION PCOF.SET PUBLIC
REM \

    PCOF.REPORT.NUM%  = 21 
    PCOF.FILE.NAME$   = "PCOF"
END FUNCTION

\----------------------------------------------------------------------------

REM \

  FUNCTION READ.PCOF PUBLIC

   STRING RPRT.LINE$
   INTEGER*2 READ.PCOF

   READ.PCOF = 1

   IF END#PCOF.SESS.NUM% THEN READ.PCOF.ERROR

    READ #PCOF.SESS.NUM%; RPRT.LINE$

   READ.PCOF = 0
   EXIT FUNCTION

   READ.PCOF.ERROR:

   CURRENT.REPORT.NUM% = PCOF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.PCOF PUBLIC

   STRING RPRT.LINE$
   INTEGER*2 WRITE.PCOF

   WRITE.PCOF = 1

   IF END#PCOF.SESS.NUM% THEN WRITE.PCOF.ERROR

    WRITE #PCOF.SESS.NUM%; RPRT.LINE$

   WRITE.PCOF = 0
   EXIT FUNCTION

   WRITE.PCOF.ERROR:

   CURRENT.REPORT.NUM% = PCOF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

