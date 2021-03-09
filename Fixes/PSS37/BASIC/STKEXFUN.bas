\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKEXFUN.BAS  $
\***
\***   $Revision:   1.3  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE EXCEPTIONS FILE FUNCTIONS
\***
\***               REFERENCE    : STKEXFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***         VERSION B         Nik Sen            5th August 1997
\***         Added Reason Code.
\***
\***         VERSION C            Johnnie Chan    11th December 1997
\***         Added extra digit to location code.
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKEXDEC.J86                                              

  FUNCTION STKEX.SET PUBLIC
\***************************

    STKEX.REPORT.NUM% = 527
    STKEX.RECL%       = 18                     !*Ver C*
    STKEX.FILE.NAME$  = "STKEX"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKEX PUBLIC
\****************************

    INTEGER*2 READ.STKEX
    
    READ.STKEX = 1
    
    IF END #STKEX.SESS.NUM% THEN READ.STKEX.ERROR
    READ FORM "C7,C3,C5,C2,C1"; #STKEX.SESS.NUM%, STKEX.RECORD.NUM%;  \*Ver C*
       STKEX.ITEM.CODE$,                       \
       STKEX.QUANTITY$,                        \
       STKEX.LOCATION$,                        \
       STKEX.STOCKTAKER.NUM$,                  \
       STKEX.REASON.CODE$

    READ.STKEX = 0
    EXIT FUNCTION
    
READ.STKEX.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKEX.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

FUNCTION WRITE.STKEX PUBLIC
\****************************

    INTEGER*2 WRITE.STKEX
    
    WRITE.STKEX = 1
    
    IF END #STKEX.SESS.NUM% THEN WRITE.STKEX.ERROR
    WRITE FORM "C7,C3,C5,C2,C1"; #STKEX.SESS.NUM%, STKEX.RECORD.NUM%; \*Ver C*
       STKEX.ITEM.CODE$,                       \
       STKEX.QUANTITY$,                        \
       STKEX.LOCATION$,                        \
       STKEX.STOCKTAKER.NUM$,                  \
       STKEX.REASON.CODE$

    WRITE.STKEX = 0
    EXIT FUNCTION
    
WRITE.STKEX.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKEX.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

