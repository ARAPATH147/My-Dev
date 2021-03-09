\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKMFFUN.BAS  $
\***
\***   $Revision:   1.1  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE MAINFRAME TRANSMISSION  FILE FUNCTIONS
\***
\***               REFERENCE    : STKMFFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKMFDEC.J86                                       

  FUNCTION STKMF.SET PUBLIC
\***************************

    STKMF.REPORT.NUM% = 530
    STKMF.RECL%       = 27
    STKMF.FILE.NAME$  = "STKMF"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKMF PUBLIC
\****************************

    INTEGER*2 READ.STKMF
    
    READ.STKMF = 1
    
    IF END #STKMF.SESS.NUM% THEN READ.STKMF.ERROR
    READ FORM "C27"; #STKMF.SESS.NUM%, STKMF.RECORD.NUM%;  STKMF.DATA$

    READ.STKMF = 0
    EXIT FUNCTION
    
READ.STKMF.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKMF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.STKMF PUBLIC
\*****************************

       INTEGER*2 WRITE.STKMF
    
       WRITE.STKMF = 1
    
       IF END #STKMF.SESS.NUM% THEN WRITE.STKMF.ERROR
       WRITE FORM "C27"; #STKMF.SESS.NUM%,STKMF.RECORD.NUM%; STKMF.DATA$  

       WRITE.STKMF = 0
       EXIT FUNCTION
   
WRITE.STKMF.ERROR:
   

      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = STKMF.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


