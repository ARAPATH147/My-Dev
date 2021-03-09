REM \
\******************************************************************************
\******************************************************************************
\***
\***    %INCLUDE FOR RETURNS CLAIMING SUMMARY REPORT FILE EXTERNAL FUNCTIONS
\***
\***               REFERENCE    : CCSMYFUN.BAS
\***
\***       VERSION A       Michael Kelsall      3rd November 1993
\***
\******************************************************************************
\******************************************************************************

  %INCLUDE CCSMYDEC.J86
  
  STRING GLOBAL                     \
      CURRENT.CODE$,                \
      FILE.OPERATION$ 
      
  INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%    

  FUNCTION CCSMY.SET PUBLIC

  INTEGER*2 CCSMY.SET
  
    CCSMY.SET = 1

    CCSMY.REPORT.NUM% = 406
    CCSMY.FILE.NAME$  = "CCSMY"

    CCSMY.SET = 0
    
  END FUNCTION
  

  FUNCTION WRITE.CCSMY PUBLIC
  
  INTEGER*2 WRITE.CCSMY
  
    WRITE.CCSMY = 1
  
    IF END #CCSMY.SESS.NUM% THEN WRITE.CCSMY.ERROR
    WRITE #CCSMY.SESS.NUM%; CCSMY.REPORT.LINE$

    WRITE.CCSMY = 0
    
    EXIT FUNCTION
    
  WRITE.CCSMY.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = CCSMY.REPORT.NUM%
    CURRENT.CODE$ = ""
    
    EXIT FUNCTION

  END FUNCTION


