\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE ITEM GROUP FILE FUNCTIONS
\***
\***               REFERENCE    : STKIGFUN.BAS
\***
\***         VERSION A            Nik Sen         17th June 1997
\***
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKIGDEC.J86                                              ! BRC

  FUNCTION STKIG.SET PUBLIC
\***************************

    STKIG.REPORT.NUM% = 524
    STKIG.RECL%       = 6
    STKIG.FILE.NAME$  = "STKIG"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKIG PUBLIC
\****************************

    INTEGER*2 READ.STKIG
    
    READ.STKIG = 1
    
    IF END #STKIG.SESS.NUM% THEN READ.STKIG.ERROR
    READ FORM "C6"; #STKIG.SESS.NUM%,STKIG.RECORD.NUM%;  STKIG.DATA$

    READ.STKIG = 0
    EXIT FUNCTION
    
READ.STKIG.ERROR:
    
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKIG.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.STKIG PUBLIC
\*****************************

       INTEGER*2 WRITE.STKIG
    
       WRITE.STKIG = 1
    
       IF END #STKIG.SESS.NUM% THEN WRITE.STKIG.ERROR
       WRITE FORM "C6"; #STKIG.SESS.NUM%,STKIG.RECORD.NUM%; STKIG.DATA$  

       WRITE.STKIG = 0
       EXIT FUNCTION
   
WRITE.STKIG.ERROR:
   
      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = STKIG.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


