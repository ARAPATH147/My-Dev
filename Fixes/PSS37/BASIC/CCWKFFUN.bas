\*******************************************************************************
\*******************************************************************************
\***
\***              RETURNS / AUTOMATIC CREDIT CLAIMING SYSTEM
\***
\***                 EXTERNAL FILE FUNCTION DEFINITIONS
\***
\***                    REFERENCE    : CCWKFFUN.BAS
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
         
  %INCLUDE CCWKFDEC.J86                                                
                                                                     
  
  FUNCTION CCWKF.SET PUBLIC
    INTEGER*2 CCWKF.SET
    
    CCWKF.SET = 1

      CCWKF.REPORT.NUM%  = 327
      CCWKF.FILE.NAME$ = "CCWKF"
    
    CCWKF.SET = 0

  END FUNCTION



  FUNCTION READ.CCWKF PUBLIC
    INTEGER*2 READ.CCWKF 
    
    READ.CCWKF = 1
    
       IF END #CCWKF.SESS.NUM% THEN READ.ERROR
       READ #CCWKF.SESS.NUM%; LINE CCWKF.RECORD$
    
    READ.CCWKF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = ""
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCWKF.REPORT.NUM%
       
       EXIT FUNCTION
                            
  END FUNCTION
  


  FUNCTION WRITE.CCWKF PUBLIC

    INTEGER*2 WRITE.CCWKF
    WRITE.CCWKF = 1  

       IF END #CCWKF.SESS.NUM% THEN WRITE.ERROR                 
       PRINT #CCWKF.SESS.NUM%; CCWKF.RECORD$                
       
    WRITE.CCWKF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
       CURRENT.CODE$ = CCWKF.RECORD$
       FILE.OPERATION$= "W"
       CURRENT.REPORT.NUM% = CCWKF.REPORT.NUM%
       
       EXIT FUNCTION    

  END FUNCTION

  

  FUNCTION WRITE.HOLD.CCWKF PUBLIC                                      

    INTEGER*2 WRITE.HOLD.CCWKF                                          
    WRITE.HOLD.CCWKF = 1                                                

       IF END #CCWKF.SESS.NUM% THEN WRITE.HOLD.ERROR                    
       PRINT #CCWKF.SESS.NUM%; CCWKF.RECORD$         
       
    WRITE.HOLD.CCWKF = 0                                                
    EXIT FUNCTION                                                       
    
    WRITE.HOLD.ERROR:                                                   
    
       CURRENT.CODE$ = CCWKF.RECORD$                                    
       FILE.OPERATION$= "W"                                             
       CURRENT.REPORT.NUM% = CCWKF.REPORT.NUM%                          
       
       EXIT FUNCTION                                                    

  END FUNCTION                                                          
