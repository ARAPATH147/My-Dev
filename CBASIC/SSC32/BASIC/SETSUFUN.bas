\*****************************************************************************
\***
\***               FUNCTIONS FOR THE DIRECTS SUPPLIER FILE
\***
\***               REFERENCE    : SETSUFUN.BAS
\***               Directs supplier file
\***
\***    Version A               Mark Goode                  01.12.08
\***
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$              

   %INCLUDE SETSUDEC.J86

   FUNCTION SETSU.SET PUBLIC

    SETSUP.REPORT.NUM% = 783         
    SETSUP.FILE.NAME$  = "SETSUPPL"  
    SETSUP.RECL%       = 11
    
   END FUNCTION

\----------------------------------------------------------------------------

   FUNCTION READ.SETSUP PUBLIC

   INTEGER*2 READ.SETSUP

   READ.SETSUP = 1

   IF END#SETSUP.SESS.NUM% THEN READ.SETSUP.ERROR
     
     READ FORM "C11"; #SETSUP.SESS.NUM%; SETSUP.RECORD$                                  
   
   READ.SETSUP = 0
   EXIT FUNCTION

   READ.SETSUP.ERROR:

   CURRENT.REPORT.NUM% = SETSUP.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


   FUNCTION WRITE.SETSUP PUBLIC

   INTEGER*2 WRITE.SETSUP

   WRITE.SETSUP = 1

   IF END#SETSUP.SESS.NUM% THEN WRITE.SETSUP.ERROR

     WRITE FORM "C11"; #SETSUP.SESS.NUM%; SETSUP.RECORD$                          

   WRITE.SETSUP = 0
   EXIT FUNCTION

   WRITE.SETSUP.ERROR:

   CURRENT.REPORT.NUM% = SETSUP.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

     
