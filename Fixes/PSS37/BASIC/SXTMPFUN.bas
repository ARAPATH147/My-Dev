\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   SXTMPFUN.BAS  $
\***
\***   $Revision:   1.4  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE TEMPORARY BUFFER FILE FUNCTIONS
\***
\***               REFERENCE    : SXTMPFUN.BAS
\***
\***         VERSION A            Nik Sen         30th June 1997
\***
\***         VERSION B            Nik Sen         18th August 1997
\***         Changed file length to accomodate changed data structure.
\***
\***         VERSION C            Nik Sen         23rd September 1997
\***         Changed file type from direct to sequential to speed up file
\***         copy after download.
\***
\***         VERSION D            Nik Sen          10th December 1997
\***         Changed record length to 30 bytes. (This is not actually
\***         required since file is now sequential) 
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE SXTMPDEC.J86                                              

  FUNCTION SXTMP.SET PUBLIC
\***************************

    SXTMP.REPORT.NUM% = 531
    SXTMP.RECL%       = 30                                             ! DNS
    SXTMP.FILE.NAME$  = "SXTMP"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.SXTMP PUBLIC
\****************************

    INTEGER*2 READ.SXTMP
    
    READ.SXTMP = 1
    
    IF END #SXTMP.SESS.NUM% THEN READ.SXTMP.ERROR
    READ #SXTMP.SESS.NUM%;SXTMP.DATA$                                  ! CNS


    READ.SXTMP = 0
    EXIT FUNCTION
    
READ.SXTMP.ERROR:
    
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = SXTMP.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION
               

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.SXTMP PUBLIC
\*****************************

       INTEGER*2 WRITE.SXTMP
    
       WRITE.SXTMP = 1
    
       IF END #SXTMP.SESS.NUM% THEN WRITE.SXTMP.ERROR
       WRITE #SXTMP.SESS.NUM%;SXTMP.DATA$                              ! CNS



       WRITE.SXTMP = 0

       EXIT FUNCTION
   
WRITE.SXTMP.ERROR:
   
      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = SXTMP.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


