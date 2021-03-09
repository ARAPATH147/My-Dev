REM \
\******************************************************************************
\******************************************************************************
\***
\***    %INCLUDE FOR RETURNS CLAIMING DETAILED REPORT FILE EXTERNAL FUNCTIONS
\***
\***                    REFERENCE    : CCDETFUN.BAS
\***
\***       VERSION A       Michael Kelsall      3rd November 1993
\***
\******************************************************************************
\******************************************************************************

  %INCLUDE CCDETDEC.J86
  
  STRING GLOBAL                     \
      CURRENT.CODE$,                \
      FILE.OPERATION$ 
      
  INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%    

  FUNCTION CCDET.SET PUBLIC

  INTEGER*2 CCDET.SET
  
    CCDET.SET = 1

    CCDET.REPORT.NUM% = 407
    CCDET.FILE.NAME$  = "CCDET"

    CCDET.SET = 0
    
  END FUNCTION
  

  FUNCTION WRITE.CCDET PUBLIC
  
  INTEGER*2 WRITE.CCDET
  
    WRITE.CCDET = 1
  
    IF END #CCDET.SESS.NUM% THEN WRITE.CCDET.ERROR
    WRITE #CCDET.SESS.NUM%; CCDET.REPORT.LINE$

    WRITE.CCDET = 0
    
    EXIT FUNCTION
    
  WRITE.CCDET.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = CCDET.REPORT.NUM%
    CURRENT.CODE$ = ""
    
    EXIT FUNCTION

  END FUNCTION


