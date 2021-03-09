\*******************************************************************************
\*******************************************************************************
\***
\***           DIRECTS TEMPORARY STOCK MOVEMENT QUEUE FILE FUNCTIONS
\***
\***           REFERENCE    : DRSMQFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                          
  INTEGER*2 GLOBAL              \
     CURRENT.REPORT.NUM%
     
  STRING GLOBAL                 \
     CURRENT.CODE$,             \
     FILE.OPERATION$
  
  %INCLUDE DRSMQDEC.J86
  
  FUNCTION DRSMQ.SET PUBLIC
\***************************

    DRSMQ.REPORT.NUM%  = 245
    DRSMQ.FILE.NAME$ = "DRSMQ"
    
  END FUNCTION
\-----------------------------------------------------------------------------
    
                                                                     
  FUNCTION READ.DRSMQ PUBLIC
\****************************

    INTEGER*2 READ.DRSMQ
    
    READ.DRSMQ = 1
    
    IF END #DRSMQ.SESS.NUM% THEN READ.ERROR
    READ #DRSMQ.SESS.NUM%; LINE DRSMQ.RECORD$
    READ.DRSMQ = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = DRSMQ.RECORD$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = DRSMQ.REPORT.NUM%
       
       EXIT FUNCTION
                               
  END FUNCTION
\-----------------------------------------------------------------------------  

  FUNCTION WRITE.DRSMQ PUBLIC
\*****************************

  INTEGER*2 WRITE.DRSMQ  

  STRING  FORMAT$,                                                   \
          STRING.LENGTH$

  WRITE.DRSMQ = 1         

    STRING.LENGTH$ = STR$(LEN(DRSMQ.RECORD$))
    FORMAT$ = "C" + STRING.LENGTH$                                      
    IF END #DRSMQ.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM FORMAT$; #DRSMQ.SESS.NUM%; DRSMQ.RECORD$                  
    WRITE.DRSMQ = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
       CURRENT.CODE$ = DRSMQ.RECORD$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = DRSMQ.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
