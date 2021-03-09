\*******************************************************************************
\*******************************************************************************
\***
\***              RETURNS / AUTOMATIC CREDIT CLAIMING SYSTEM
\***
\***                 EXTERNAL FILE FUNCTION DEFINITIONS
\***
\***                    REFERENCE    : CCBUFFUN.BAS
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
         
  %INCLUDE CCBUFDEC.J86                                                
                                                                     
  
  FUNCTION CCBUF.SET PUBLIC
    INTEGER*2 CCBUF.SET
    
    CCBUF.SET = 1

      CCBUF.REPORT.NUM%  = 326
      CCBUF.FILE.NAME$ = "CCBUF"
    
    CCBUF.SET = 0

  END FUNCTION



  FUNCTION READ.CCBUF PUBLIC
    INTEGER*2 READ.CCBUF 
    
    READ.CCBUF = 1
    
       IF END #CCBUF.SESS.NUM% THEN READ.ERROR
       READ #CCBUF.SESS.NUM%; LINE CCBUF.RECORD$
       CCBUF.TRANS.TYPE$ = UNPACK$(MID$(CCBUF.RECORD$,2,1))
    
    READ.CCBUF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = ""
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCBUF.REPORT.NUM%
       
       EXIT FUNCTION
                            
  END FUNCTION
  


  FUNCTION WRITE.CCBUF PUBLIC

    INTEGER*2 WRITE.CCBUF
    
    STRING FORMAT$,                                                   \
           STRING.LENGTH$
            
    WRITE.CCBUF = 1  

       STRING.LENGTH$ = STR$(LEN(CCBUF.RECORD$))
       FORMAT$ = "C" + STRING.LENGTH$                                      
       IF END #CCBUF.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; #CCBUF.SESS.NUM%; CCBUF.RECORD$                  
       
    WRITE.CCBUF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
       CURRENT.CODE$ = CCBUF.RECORD$
       FILE.OPERATION$= "O"
       CURRENT.REPORT.NUM% = CCBUF.REPORT.NUM%
       
       EXIT FUNCTION    

  END FUNCTION

  

  FUNCTION WRITE.HOLD.CCBUF PUBLIC                                      

    INTEGER*2 WRITE.HOLD.CCBUF                                          
    
    STRING FORMAT$,                                                     \ 
           STRING.LENGTH$                                               
            
    WRITE.HOLD.CCBUF = 1                                                

       STRING.LENGTH$ = STR$(LEN(CCBUF.RECORD$))                        
       FORMAT$ = "C" + STRING.LENGTH$                                   
       IF END #CCBUF.SESS.NUM% THEN WRITE.HOLD.ERROR                    
       WRITE FORM FORMAT$; HOLD #CCBUF.SESS.NUM%; CCBUF.RECORD$         
       
    WRITE.HOLD.CCBUF = 0                                                
    EXIT FUNCTION                                                       
    
    WRITE.HOLD.ERROR:                                                   
    
       CURRENT.CODE$ = CCBUF.RECORD$                                    
       FILE.OPERATION$= "O"                                             
       CURRENT.REPORT.NUM% = CCBUF.REPORT.NUM%                          
       
       EXIT FUNCTION                                                    

  END FUNCTION                                                          
