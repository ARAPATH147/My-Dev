\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE STORE OPTIONS FILE
\***
\***                     REFERENCE    : SOPTSFUN.BAS
\***
\***       Version A      Stuart William McConnachie      19th June 1995
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE SOPTSDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
\------------------------------------------------------------------------------   

   FUNCTION SOPTS.SET PUBLIC   
 \***************************
  
      SOPTS.REPORT.NUM% = 34
      SOPTS.RECL%       = 102
      SOPTS.FILE.NAME$  = "ADXLXAAN::EALSOPTS"
  
   END FUNCTION

\------------------------------------------------------------------------------
  
   FUNCTION READ.SOPTS PUBLIC
 \****************************   

      INTEGER*2 I%, READ.SOPTS      
    
      READ.SOPTS = 1

      IF END #SOPTS.SESS.NUM% THEN END.OF.SOPTS    
      READ FORM "T1,C102"; #SOPTS.SESS.NUM%,SOPTS.REC.NUM%; SOPTS.RECORD$
    
      I% = 100
      WHILE MID$(SOPTS.RECORD$,I%,1) = " "      \
       AND  I% > 1                           
         I% = I% - 1
      WEND
      SOPTS.RECORD$ = LEFT$(SOPTS.RECORD$,I%)  

      READ.SOPTS = 0     
      EXIT FUNCTION      
      
      
      END.OF.SOPTS:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                      STR$(SOPTS.REC.NUM%)), 8)
         CURRENT.REPORT.NUM% = SOPTS.REPORT.NUM%

      EXIT FUNCTION
          
   END FUNCTION                             

\---------------------------------------------------------------------------- 

   FUNCTION WRITE.SOPTS PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.SOPTS                                                    
    
      WRITE.SOPTS = 1      
  
      SOPTS.RECORD$ = LEFT$(SOPTS.RECORD$ +                             \
                            STRING$(SOPTS.RECL%, " "),                  \
                            SOPTS.RECL% - 2)
  
      IF END #SOPTS.SESS.NUM% THEN END.OF.WRITE.SOPTS  
      WRITE FORM "C100,C2"; #SOPTS.SESS.NUM%, SOPTS.REC.NUM%;           \
                            SOPTS.RECORD$, CHR$(0DH) + CHR$(0AH)
 
      WRITE.SOPTS = 0     
      EXIT FUNCTION      
      
      
      END.OF.WRITE.SOPTS:
      
         FILE.OPERATION$     = "W"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                      STR$(SOPTS.REC.NUM%)), 8)
         CURRENT.REPORT.NUM% = SOPTS.REPORT.NUM%

      EXIT FUNCTION
          

   END FUNCTION

\------------------------------------------------------------------------------

