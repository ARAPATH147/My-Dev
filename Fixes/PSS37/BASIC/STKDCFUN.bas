\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKDCFUN.BAS  $
\***
\***   $Revision:   1.2  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE DUMP CODE FILE FUNCTIONS
\***
\***               REFERENCE    : STKDCFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***         VERSION B            Johnnie Chan    11th December 1997
\***         Added extra digit to location code.
\***
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKDCDEC.J86                                             

  FUNCTION STKDC.SET PUBLIC
\***************************

    STKDC.REPORT.NUM% = 528
    STKDC.RECL%       =20                      !*Ver B*
    STKDC.FILE.NAME$  = "STKDC"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKDC PUBLIC
\****************************

    INTEGER*2 READ.STKDC
    
    READ.STKDC = 1
    
    IF END #STKDC.SESS.NUM% THEN READ.STKDC.ERROR
    READ FORM "C7,2C3,C5,C2"; #STKDC.SESS.NUM%,STKDC.RECORD.NUM%;  \*Ver B*
         STKDC.DUMP.CODE$,                             \
         STKDC.QUANTITY$,                              \
         STKDC.PRICE$,                                 \
         STKDC.LOCATION$,                              \
         STKDC.STOCKTAKER.NUM$

    READ.STKDC = 0
    EXIT FUNCTION
    
READ.STKDC.ERROR:
    
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKDC.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

  
FUNCTION WRITE.STKDC PUBLIC
\*****************************

       INTEGER*2 WRITE.STKDC
    
       WRITE.STKDC = 1
    
       IF END #STKDC.SESS.NUM% THEN WRITE.STKDC.ERROR
       WRITE FORM "C7,2C3,C5,C2"; #STKDC.SESS.NUM%,STKDC.RECORD.NUM%; \*Ver B*
               STKDC.DUMP.CODE$,                               \
               STKDC.QUANTITY$,                                \
               STKDC.PRICE$,                                   \
               STKDC.LOCATION$,                                \
               STKDC.STOCKTAKER.NUM$

       WRITE.STKDC = 0
       EXIT FUNCTION
   
WRITE.STKDC.ERROR:
   

      FILE.OPERATION$ = "W"                                            
      CURRENT.REPORT.NUM% = STKDC.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION


