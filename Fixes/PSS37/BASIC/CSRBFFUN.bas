REM \
\*******************************************************************************
\*******************************************************************************
\***
\***              CSR BUFFER WORKFILE FUNCTIONS
\***
\***                REFERENCE    : CSRBFFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************

  STRING GLOBAL                         \
      CURRENT.CODE$,                    \
      FILE.OPERATION$                   \
      
  INTEGER*2                             \
      CURRENT.REPORT.NUM%
      
  %INCLUDE CSRBFDEC.J86
  
  
  FUNCTION CSRBF.SET PUBLIC
\***************************  

    CSRBF.REPORT.NUM% = 200
    CSRBF.FILE.NAME$ = "CSRBF"
    
  END FUNCTION

  
\-----------------------------------------------------------------------------
                                                                     
  FUNCTION READ.CSRBF PUBLIC
\****************************

    INTEGER*2 READ.CSRBF
    
    READ.CSRBF = 1  

    IF END #CSRBF.SESS.NUM% THEN READ.ERROR
    READ #CSRBF.SESS.NUM%; CSRBF.DATA$

    READ.CSRBF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
    CURRENT.CODE$ = CSRBF.DATA$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = CSRBF.REPORT.NUM%
    
    EXIT FUNCTION

   END FUNCTION
\-----------------------------------------------------------------------------

   
   FUNCTION WRITE.CSRBF PUBLIC
\*******************************

    INTEGER*2 WRITE.CSRBF
    
    WRITE.CSRBF = 1
       
    IF END #CSRBF.SESS.NUM% THEN WRITE.ERROR  
    WRITE #CSRBF.SESS.NUM%; CSRBF.DATA$  
    
    WRITE.CSRBF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
    CURRENT.CODE$ = CSRBF.DATA$
    FILE.OPERATION$ = "O"
    CURRENT.REPORT.NUM% = CSRBF.REPORT.NUM%
    
    EXIT FUNCTION
                                                                                                  
   END FUNCTION 
                                     
