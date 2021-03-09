REM \
\******************************************************************************
\******************************************************************************
\***
\***     %INCLUDE FOR CHILLED FOODS WASTAGE REPORT FILE EXTERNAL FUNCTIONS
\***
\***                  REFERENCE    : CCCFWFUN.BAS
\***
\***       VERSION A       Michael Kelsall      3rd November 1993
\***
\******************************************************************************
\******************************************************************************

  %INCLUDE CCCFWDEC.J86
  
  STRING GLOBAL                     \
      CURRENT.CODE$,                \
      FILE.OPERATION$ 
      
  INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%    

  FUNCTION CCCFW.SET PUBLIC

  INTEGER*2 CCCFW.SET
  
    CCCFW.SET = 1

    CCCFW.REPORT.NUM% = 409
    CCCFW.FILE.NAME$  = "CCCFW"

    CCCFW.SET = 0
    
  END FUNCTION
  

  FUNCTION WRITE.CCCFW PUBLIC
  
  INTEGER*2 WRITE.CCCFW
  
    WRITE.CCCFW = 1
  
    IF END #CCCFW.SESS.NUM% THEN WRITE.CCCFW.ERROR
    WRITE #CCCFW.SESS.NUM%; CCCFW.REPORT.LINE$

    WRITE.CCCFW = 0
    
    EXIT FUNCTION
    
  WRITE.CCCFW.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = CCCFW.REPORT.NUM%
    CURRENT.CODE$ = ""
    
    EXIT FUNCTION

  END FUNCTION


