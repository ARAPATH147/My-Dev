\*******************************************************************************
\*******************************************************************************
\***
\***           UOD TEMPORARY FILE FUNCTIONS
\***
\***           REFERENCE    : UODTFFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                          
  INTEGER*2 GLOBAL              \
     CURRENT.REPORT.NUM%
     
  STRING GLOBAL                 \
     CURRENT.CODE$,             \
     FILE.OPERATION$
  
  %INCLUDE UODTFDEC.J86
  
  FUNCTION UODTF.SET PUBLIC
\***************************

    UODTF.REPORT.NUM%  = 276
    UODTF.FILE.NAME$ = "UODTF"
    
  END FUNCTION
\-----------------------------------------------------------------------------
    
                                                                     
  FUNCTION READ.UODTF PUBLIC
\****************************

    INTEGER*2 READ.UODTF
    
    READ.UODTF = 1
    
    IF END #UODTF.SESS.NUM% THEN READ.ERROR
    READ #UODTF.SESS.NUM%; LINE UODTF.RECORD$
    READ.UODTF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = UODTF.RECORD$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = UODTF.REPORT.NUM%
       
       EXIT FUNCTION
                               
  END FUNCTION
\-----------------------------------------------------------------------------  

  FUNCTION WRITE.UODTF PUBLIC
\*****************************

  INTEGER*2 WRITE.UODTF  

  STRING  FORMAT$,                                                   \
          STRING.LENGTH$

  WRITE.UODTF = 1         

    STRING.LENGTH$ = STR$(LEN(UODTF.RECORD$))
    FORMAT$ = "C" + STRING.LENGTH$                                      
    IF END #UODTF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM FORMAT$; #UODTF.SESS.NUM%; UODTF.RECORD$                  
    WRITE.UODTF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
       CURRENT.CODE$ = UODTF.RECORD$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODTF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
