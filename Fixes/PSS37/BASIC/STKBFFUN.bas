\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKBFFUN.BAS  $
\***
\***   $Revision:   1.3  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE BUFFER FILE FUNCTIONS
\***
\***               REFERENCE    : STKBFFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***         VERSION B            Nik Sen         18th August 1997
\***         Changed file length.
\***
\***         VERSION C            Nik Sen         10th December 1997
\***         Changed file length again.
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKBFDEC.J86                                              

  FUNCTION STKBF.SET PUBLIC
\***************************

    STKBF.REPORT.NUM% = 525
    STKBF.RECL%       = 30                                             ! CNS
    STKBF.FILE.NAME$  = "STKBF"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKBF PUBLIC
\****************************

    INTEGER*2 READ.STKBF
    
    READ.STKBF = 1
    
    IF END #STKBF.SESS.NUM% THEN READ.STKBF.ERROR
    READ #STKBF.SESS.NUM%;                             \
       STKBF.DATA$                                    

    READ.STKBF = 0
    EXIT FUNCTION
    
READ.STKBF.ERROR:
    
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKBF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION
               

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.STKBF PUBLIC
\*****************************

       INTEGER*2 WRITE.STKBF
    
       WRITE.STKBF = 1
    
       IF END #STKBF.SESS.NUM% THEN WRITE.STKBF.ERROR
       WRITE #STKBF.SESS.NUM%;                        \
          STKBF.DATA$                                    




       WRITE.STKBF = 0

       EXIT FUNCTION
   
WRITE.STKBF.ERROR:
   
      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = STKBF.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


