\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE SERVL FILE
\***
\***                     REFERENCE    : SERVLFUN.BAS
\***
\***       Version A      Mark Goode      31st January 2011
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE SERVLDEC.J86

   INTEGER RETRY%
   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
\------------------------------------------------------------------------------   

   FUNCTION SERVL.SET PUBLIC   
 \***************************
  
      SERVL.REPORT.NUM% = 826
      SERVL.FILE.NAME$  = "SERVL:"
  
   END FUNCTION

\------------------------------------------------------------------------------
  
   FUNCTION READ.SERVL PUBLIC
 \****************************   

      INTEGER*2 I%, READ.SERVL      
    
      READ.SERVL = 1
      
      RETRY% = 0

      IF END #SERVL.SESS.NUM% THEN END.OF.SERVL    
      READ   #SERVL.SESS.NUM%,SERVL.REC.NUM%; SERVL.RECORD$
    
      READ.SERVL = 0     
      EXIT FUNCTION      
      
      
      END.OF.SERVL:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                      STR$(SERVL.REC.NUM%)), 8)
         CURRENT.REPORT.NUM% = SERVL.REPORT.NUM%

      EXIT FUNCTION
          
   END FUNCTION                             
                
\---------------------------------------------------------------------------- 

   FUNCTION WRITE.SERVL PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.SERVL                                                    
    
      WRITE.SERVL = 1      
  
      !SERVL.RECORD$ = {TO BE DEFINED}
  
      IF END #SERVL.SESS.NUM% THEN SERVL.WRITE.PROBLEM  
      WRITE  #SERVL.SESS.NUM%; SERVL.RECORD$ 
      WRITE.SERVL = 0     
      EXIT FUNCTION      
      
      
      SERVL.WRITE.PROBLEM:
      
         ! Check if the error code relates to file locked. If locked wait 5 seconds and retry write.
          
           FILE.OPERATION$     = "W"
           CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                        STR$(SERVL.REC.NUM%)), 8)
           CURRENT.REPORT.NUM% = SERVL.REPORT.NUM%

   END FUNCTION
   
   \---------------------------------------------------------------------------- 

   FUNCTION WRITE.MATRIX.SERVL PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.MATRIX.SERVL                                                    
    
      WRITE.MATRIX.SERVL = 1      
  
      !SERVL.RECORD$ = {TO BE DEFINED}               
      
      IF END #  SERVL.SESS.NUM% THEN SERVL.WRITE.MATRIX.PROBLEM  
      WRITE MATRIX #SERVL.SESS.NUM%; SERVL.ARRAY.RECORD$(0), SERVL.ELEMENT%+1 
      WRITE.MATRIX.SERVL = 0     
      EXIT FUNCTION      
      
      
      SERVL.WRITE.MATRIX.PROBLEM:
      
         ! Check if the error code relates to file locked. If locked wait 5 seconds and retry write.
          
           FILE.OPERATION$     = "W"
           CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                        STR$(SERVL.REC.NUM%)), 8)
           CURRENT.REPORT.NUM% = SERVL.REPORT.NUM%

      EXIT FUNCTION
          

   END FUNCTION

\------------------------------------------------------------------------------

