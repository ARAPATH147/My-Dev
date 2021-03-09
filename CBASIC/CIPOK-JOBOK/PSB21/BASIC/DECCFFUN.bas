\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE DEC CONFIGURATION FILE
\***
\***                     REFERENCE    : DECCFFUN.BAS
\***
\***       Version A      Mark Goode      20th October 2010
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE DECCFDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
\------------------------------------------------------------------------------   

   FUNCTION DECCF.SET PUBLIC   
 \***************************
  
      DECCF.REPORT.NUM% = 815
      DECCF.FILE.NAME$  = "DECCONF"
      DECCF.RECL% = 56
  
   END FUNCTION

\------------------------------------------------------------------------------
  
   FUNCTION READ.DECCF PUBLIC
 \****************************   

      INTEGER*2 I%, READ.DECCF      
    
      READ.DECCF = 1

      IF END #DECCF.SESS.NUM% THEN END.OF.DECCF    
      READ FORM "T1,C56"; #DECCF.SESS.NUM%,DECCF.REC.NUM%; DECCF.RECORD$
    
      READ.DECCF = 0     
      EXIT FUNCTION      
      
      
      END.OF.DECCF:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                      STR$(DECCF.REC.NUM%)), 8)
         CURRENT.REPORT.NUM% = DECCF.REPORT.NUM%

      EXIT FUNCTION
          
   END FUNCTION                             

\---------------------------------------------------------------------------- 

   FUNCTION WRITE.DECCF PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.DECCF                                                   
    
      WRITE.DECCF = 1      
  
      DECCF.RECORD$ = DECCF.MSGID$ + DECCF.MSGNAME$ + DECCF.DIRECTION$ + DECCF.TYPE$ + DECCF.QOS$ + DECCF.DELIVERY$
  
      IF END #DECCF.SESS.NUM% THEN END.OF.WRITE.DECCF  
      WRITE FORM "C54,C2"; #DECCF.SESS.NUM%, DECCF.REC.NUM%;           \
                            DECCF.RECORD$, CHR$(0DH) + CHR$(0AH)
 
      WRITE.DECCF = 0     
      EXIT FUNCTION      
      
      
      END.OF.WRITE.DECCF:
      
      FILE.OPERATION$     = "W"
      CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                   STR$(DECCF.REC.NUM%)), 8)
      CURRENT.REPORT.NUM% = DECCF.REPORT.NUM%

      EXIT FUNCTION
          

   END FUNCTION

\------------------------------------------------------------------------------

