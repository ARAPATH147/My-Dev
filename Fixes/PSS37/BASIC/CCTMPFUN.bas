\*******************************************************************************
\*******************************************************************************
\***
\***              RETURNS / AUTOMATIC CREDIT CLAIMING SYSTEM
\***
\***                 EXTERNAL FILE FUNCTION DEFINITIONS
\***
\***                    REFERENCE    : CCTMPFUN.BAS
\***
\***    Version A.         Michael Kelsall              30th September 1993
\***
\*******************************************************************************
\*******************************************************************************
 
  INTEGER*2 GLOBAL              \
         CURRENT.REPORT.NUM%
         
  STRING GLOBAL                 \
         CURRENT.CODE$,         \
         FILE.OPERATION$
         
  %INCLUDE CCTMPDEC.J86                                                
                                                                     
  
  FUNCTION CCTMP.SET PUBLIC
    INTEGER*2 CCTMP.SET
    
    CCTMP.SET = 1

      CCTMP.REPORT.NUM%  = 325
      CCTMP.FILE.NAME$ = "CCTMP"
    
    CCTMP.SET = 0

  END FUNCTION



  FUNCTION READ.CCTMP PUBLIC
    INTEGER*2 READ.CCTMP 
    
    READ.CCTMP = 1
    
       IF END #CCTMP.SESS.NUM% THEN READ.ERROR
       READ #CCTMP.SESS.NUM%; LINE CCTMP.RECORD$
       CCTMP.TRANS.TYPE$ = UNPACK$(MID$(CCTMP.RECORD$,2,1))
    
    READ.CCTMP = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = ""
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCTMP.REPORT.NUM%
       
       EXIT FUNCTION
                            
  END FUNCTION
  


  FUNCTION WRITE.CCTMP PUBLIC

    INTEGER*2 WRITE.CCTMP
    
    STRING FORMAT$,                                                   \
           STRING.LENGTH$
            
    WRITE.CCTMP = 1  

       STRING.LENGTH$ = STR$(LEN(CCTMP.RECORD$))
       FORMAT$ = "C" + STRING.LENGTH$                                      
       IF END #CCTMP.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; #CCTMP.SESS.NUM%; CCTMP.RECORD$                  
       
    WRITE.CCTMP = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
       CURRENT.CODE$ = CCTMP.RECORD$
       FILE.OPERATION$= "O"
       CURRENT.REPORT.NUM% = CCTMP.REPORT.NUM%
       
       EXIT FUNCTION    

  END FUNCTION

  

  FUNCTION WRITE.HOLD.CCTMP PUBLIC                                      

    INTEGER*2 WRITE.HOLD.CCTMP                                          
    
    STRING FORMAT$,                                                     \ 
           STRING.LENGTH$                                               
            
    WRITE.HOLD.CCTMP = 1                                                

       STRING.LENGTH$ = STR$(LEN(CCTMP.RECORD$))                        
       FORMAT$ = "C" + STRING.LENGTH$                                   
       IF END #CCTMP.SESS.NUM% THEN WRITE.HOLD.ERROR                    
       WRITE FORM FORMAT$; HOLD #CCTMP.SESS.NUM%; CCTMP.RECORD$         
       
    WRITE.HOLD.CCTMP = 0                                                
    EXIT FUNCTION                                                       
    
    WRITE.HOLD.ERROR:                                                   
    
       CURRENT.CODE$ = CCTMP.RECORD$                                    
       FILE.OPERATION$= "O"                                             
       CURRENT.REPORT.NUM% = CCTMP.REPORT.NUM%                          
       
       EXIT FUNCTION                                                    

  END FUNCTION                                                          
