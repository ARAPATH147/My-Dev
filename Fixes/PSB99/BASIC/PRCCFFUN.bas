\******************************************************************************
\******************************************************************************
\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE PRINTER CONTROL CODES FILE
\***
\***                     REFERENCE    : PRCCFFUN.BAS
\***
\***       Version A          Stuart William McConnachie       31th July 2000
\***
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE PRCCFDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
   
   FUNCTION PRCCF.SET PUBLIC   
 \***************************
  
      PRCCF.REPORT.NUM% = 1024
      PRCCF.FILE.NAME$  = "PRCCF"
  

   END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L    
 
 
   FUNCTION READ.PRCCF PUBLIC
 \****************************   

      INTEGER*2 READ.PRCCF
    
      READ.PRCCF = 1

      IF END #PRCCF.SESS.NUM% THEN END.OF.PRCCF
      READ # PRCCF.SESS.NUM%; PRCCF.PRINTER.TYPE$,                     \
                              PRCCF.RESET$,                            \
                              PRCCF.START.PAGE.NORM$,                  \
                              PRCCF.SBCS.FONT.NORM$,                   \
                              PRCCF.DBCS.FONT.NORM$,                   \
                              PRCCF.START.PAGE.ELITE$,                 \
                              PRCCF.SBCS.FONT.ELITE$,                  \
                              PRCCF.DBCS.FONT.ELITE$,                  \
                              PRCCF.NEW.PAGE$,                         \
                              PRCCF.DEFAULT$

      READ.PRCCF = 0     
      EXIT FUNCTION      
      
      
      END.OF.PRCCF:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = PRCCF.REPORT.NUM%          

      EXIT FUNCTION                     
          

   END FUNCTION                             


\------------------------------------------------------------------------------
