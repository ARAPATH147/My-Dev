\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKRCFUN.BAS  $
\***
\***   $Revision:   1.5  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE RECOUNT FILE FUNCTIONS
\***
\***               REFERENCE    : STKRCFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***
\***         VERSION B            Nik Sen         20th August 1997
\***         Changed length of item code field.
\***
\***         VERSION C            Johnnie Chan    11th December 1997
\***         Added extra digit to location code.
\***
\***         VERSION D            Johnnie Chan    3rd February 1998
\***         Added a new one byte field - location count number
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKRCDEC.J86                                             

  FUNCTION STKRC.SET PUBLIC
\***************************

    STKRC.REPORT.NUM% = 529
    STKRC.RECL%       = 47                                     ! *Ver D*
    STKRC.FILE.NAME$  = "STKRC"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKRC PUBLIC
\****************************

    INTEGER*2 READ.STKRC
    
    READ.STKRC = 1
    
    IF END #STKRC.SESS.NUM% THEN READ.STKRC.ERROR
    READ FORM "C7,C24,C5,C3,C2,C3,C2,I1"; #STKRC.SESS.NUM%,STKRC.RECORD.NUM%; \ *Ver D*
       STKRC.ITEM.CODE$,                                       \
       STKRC.ITEM.DESCRIPTION$,                                \
       STKRC.LOCATION$,                                        \
       STKRC.INITIAL.QUANTITY$,                                \
       STKRC.INITIAL.STOCKTAKER$,                              \
       STKRC.FINAL.QUANTITY$,                                  \
       STKRC.FINAL.STOCKTAKER$,                                \ *Ver D*
       STKRC.LOC.CNT%                                          ! *Ver D*

    READ.STKRC = 0
    EXIT FUNCTION
    
READ.STKRC.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKRC.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.STKRC PUBLIC
\*****************************

       INTEGER*2 WRITE.STKRC
    
       WRITE.STKRC = 1
    
       IF END #STKRC.SESS.NUM% THEN WRITE.STKRC.ERROR
       WRITE FORM "C7,C24,C5,C3,C2,C3,C2,I1"; #STKRC.SESS.NUM%,STKRC.RECORD.NUM%; \ *Ver D*
         STKRC.ITEM.CODE$,                                       \
         STKRC.ITEM.DESCRIPTION$,                                \
         STKRC.LOCATION$,                                        \
         STKRC.INITIAL.QUANTITY$,                                \
         STKRC.INITIAL.STOCKTAKER$,                              \
         STKRC.FINAL.QUANTITY$,                                  \
         STKRC.FINAL.STOCKTAKER$,                                \ *Ver D*
         STKRC.LOC.CNT%                                          ! *Ver D*
       
       WRITE.STKRC = 0
       EXIT FUNCTION
   
WRITE.STKRC.ERROR:
   
      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = STKRC.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


