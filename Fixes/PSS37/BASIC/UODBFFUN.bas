\*******************************************************************************
\*******************************************************************************
\***
\***           UOD BUFFER FILE EXTERNAL FILE FUNCTION DEFINITIONS
\***
\***                    REFERENCE    : UODBFFUN.BAS
\***
\***    Version A.         Michael Kelsall              08th February 1993
\***
\*******************************************************************************
\*******************************************************************************
 
  INTEGER*2 GLOBAL              \
         CURRENT.REPORT.NUM%
         
  STRING GLOBAL                 \
         CURRENT.CODE$,         \
         FILE.OPERATION$
         
  %INCLUDE UODBFDEC.J86                                                
                                                                     
  
  FUNCTION UODBF.SET PUBLIC
    INTEGER*2 UODBF.SET
    
    UODBF.SET = 1

      UODBF.REPORT.NUM%  = 272
      UODBF.FILE.NAME$ = "UODBF"
    
    UODBF.SET = 0

  END FUNCTION



  FUNCTION READ.UODBF PUBLIC
    INTEGER*2 READ.UODBF 
    
    READ.UODBF = 1
    
       IF END #UODBF.SESS.NUM% THEN READ.ERROR
       READ #UODBF.SESS.NUM%; LINE UODBF.RECORD$
       UODBF.TRANS.TYPE$ = UNPACK$(MID$(UODBF.RECORD$,2,1))
       UODBF.TIME$       = UNPACK$(MID$(UODBF.RECORD$,7,3))
    
    READ.UODBF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = UNPACK$(MID$(UODBF.RECORD$,2,1))
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = UODBF.REPORT.NUM%
       
       EXIT FUNCTION
                            
  END FUNCTION
  


  FUNCTION WRITE.UODBF PUBLIC

    INTEGER*2 WRITE.UODBF
    
    STRING FORMAT$,                                                   \
           STRING.LENGTH$
            
    WRITE.UODBF = 1  

       STRING.LENGTH$ = STR$(LEN(UODBF.RECORD$))
       FORMAT$ = "C" + STRING.LENGTH$                                      
       IF END #UODBF.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; #UODBF.SESS.NUM%; UODBF.RECORD$                  
       
    WRITE.UODBF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
       CURRENT.CODE$ = UODBF.RECORD$
       FILE.OPERATION$= "O"
       CURRENT.REPORT.NUM% = UODBF.REPORT.NUM%
       
       EXIT FUNCTION    

  END FUNCTION

  

  FUNCTION WRITE.HOLD.UODBF PUBLIC                                      

    INTEGER*2 WRITE.HOLD.UODBF                                          
    
    STRING FORMAT$,                                                     \ 
           STRING.LENGTH$                                               
            
    WRITE.HOLD.UODBF = 1                                                

       STRING.LENGTH$ = STR$(LEN(UODBF.RECORD$))                        
       FORMAT$ = "C" + STRING.LENGTH$                                   
       IF END #UODBF.SESS.NUM% THEN WRITE.HOLD.ERROR                    
       WRITE FORM FORMAT$; HOLD #UODBF.SESS.NUM%; UODBF.RECORD$         
       
    WRITE.HOLD.UODBF = 0                                                
    EXIT FUNCTION                                                       
    
    WRITE.HOLD.ERROR:                                                   
    
       CURRENT.CODE$ = UODBF.RECORD$                                    
       FILE.OPERATION$= "O"                                             
       CURRENT.REPORT.NUM% = UODBF.REPORT.NUM%                          
       
       EXIT FUNCTION                                                    

  END FUNCTION                                                          
