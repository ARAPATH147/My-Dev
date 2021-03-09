\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE DAILY DEC API LOG FILE
\***
\***                     REFERENCE    : DECAPFUN.BAS
\***
\***       Version A      Mark Goode      20th October 2010
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE DECAPDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
\------------------------------------------------------------------------------   

   FUNCTION DECAP.SET PUBLIC   
 \***************************
  
      DECAP.REPORT.NUM% = 814
      DECAP.FILE.NAME$  = "DECAPIL:"
  
   END FUNCTION

\------------------------------------------------------------------------------
  
   FUNCTION READ.DECAP PUBLIC
 \****************************   

      INTEGER*2 I%, READ.DECAP      
    
      READ.DECAP = 1

      IF END #DECAP.SESS.NUM% THEN END.OF.DECAP    
      READ   #DECAP.SESS.NUM%; DECAP.RECORD$
    
      READ.DECAP = 0     
      EXIT FUNCTION      
      
      
      END.OF.DECAP:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000"), 8)
         CURRENT.REPORT.NUM% = DECAP.REPORT.NUM%

      EXIT FUNCTION
          
   END FUNCTION                             

\---------------------------------------------------------------------------- 

   FUNCTION WRITE.DECAP PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.DECAP                                                    
    
      WRITE.DECAP = 1      
  
      IF END #DECAP.SESS.NUM% THEN DECAP.WRITE.PROBLEM  
      PRINT USING "&"; #DECAP.SESS.NUM%; DECAP.RECORD$
 
      WRITE.DECAP = 0     
      EXIT FUNCTION      
      
      
      DECAP.WRITE.PROBLEM:
      
          
        FILE.OPERATION$     = "W"
        CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000"), 8)
        CURRENT.REPORT.NUM% = DECAP.REPORT.NUM%

      EXIT FUNCTION
          

   END FUNCTION

\------------------------------------------------------------------------------

