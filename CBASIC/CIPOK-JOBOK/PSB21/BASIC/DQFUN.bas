\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE DQ FILE
\***
\***                     REFERENCE    : DQFUN.BAS
\***
\***       Version A      Mark Goode      20th October 2010
\***
\***       Version B      Mark Goode      26th September 2011
\***       Removed redundant code and comments
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE DQDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
\------------------------------------------------------------------------------   

   FUNCTION DQ.SET PUBLIC   
 \***************************
  
      DQ.REPORT.NUM% = 812
      DQ.FILE.NAME$  = "DQ:"
  
   END FUNCTION

\------------------------------------------------------------------------------
  
   FUNCTION READ.DQ PUBLIC
 \****************************   

      INTEGER*2 I%, READ.DQ      
    
      READ.DQ = 1
      
      IF END #DQ.SESS.NUM% THEN END.OF.DQ    
      READ   #DQ.SESS.NUM%,DQ.REC.NUM%; DQ.RECORD$
    
      READ.DQ = 0     
      EXIT FUNCTION      
      
      
      END.OF.DQ:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                      STR$(DQ.REC.NUM%)), 8)
         CURRENT.REPORT.NUM% = DQ.REPORT.NUM%

      EXIT FUNCTION
          
   END FUNCTION                             
                
\---------------------------------------------------------------------------- 

   FUNCTION WRITE.DQ PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.DQ                                                    
    
      WRITE.DQ = 1      
  
      IF END #DQ.SESS.NUM% THEN DQ.WRITE.PROBLEM  
      WRITE  #DQ.SESS.NUM%; DQ.PAYLOAD$ 
      WRITE.DQ = 0     
      EXIT FUNCTION      
      
      
      DQ.WRITE.PROBLEM:
          
           FILE.OPERATION$     = "W"
           CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                        STR$(DQ.REC.NUM%)), 8)
           CURRENT.REPORT.NUM% = DQ.REPORT.NUM%

   END FUNCTION
   
   \---------------------------------------------------------------------------- 

   FUNCTION WRITE.MATRIX.DQ PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.MATRIX.DQ                                                    
    
      WRITE.MATRIX.DQ = 1      
  
      IF END #DQ.SESS.NUM% THEN DQ.WRITE.MATRIX.PROBLEM  
      WRITE MATRIX #DQ.SESS.NUM%; DQ.ARRAY.RECORD$(0), DQ.ELEMENT%+1 
      WRITE.MATRIX.DQ = 0     
      EXIT FUNCTION      
      
      
      DQ.WRITE.MATRIX.PROBLEM:
      
           FILE.OPERATION$     = "W"
           CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                        STR$(DQ.REC.NUM%)), 8)
           CURRENT.REPORT.NUM% = DQ.REPORT.NUM%

      EXIT FUNCTION
          

   END FUNCTION

\------------------------------------------------------------------------------

