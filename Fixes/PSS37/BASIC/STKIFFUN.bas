\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKIFFUN.BAS  $
\***
\***   $Revision:   1.4  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE ITEM FILE FUNCTIONS
\***
\***               REFERENCE    : STKIFFUN.BAS
\***
\***         VERSION A            Nik Sen         19th June 1997
\***
\***
\***         VERSION B            Nik Sen         19th August 1997
\***         Added field for STKMF record number.
\***
\***         VERSION C            Johnnie Cha     11th December 1997
\***         Record size increased by one byte for extra digit in location code.
\***         Note: Key Length is one larger as a consequence.
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKIFDEC.J86                                              

  FUNCTION STKIF.SET PUBLIC
\***************************

    STKIF.REPORT.NUM% = 526
    STKIF.RECL%       = 49                                             !*Ver C*
    STKIF.FILE.NAME$  = "STKIF"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKIF PUBLIC
\****************************

    INTEGER*2 READ.STKIF
    
    READ.STKIF = 1
                 
    STKIF.KEY$ = STKIF.ITEM.CODE$ + STKIF.LOCATION$
    IF END #STKIF.SESS.NUM% THEN READ.STKIF.ERROR
    READ FORM "T13,C1,2C3,C24,C2,I4"; #STKIF.SESS.NUM% KEY STKIF.KEY$; \*Ver C*
       STKIF.BUSINESS.CENTRE$,                                         \
       STKIF.GROUP.SEQUENCE$,                                          \
       STKIF.QUANTITY$,                                                \
       STKIF.DESCRIPTION$,                                             \
       STKIF.STOCKTAKER.NUM$,                                          \
       STKIF.STKMF.RECORD.NUM%
         

    READ.STKIF = 0
    EXIT FUNCTION
    
READ.STKIF.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKIF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  
FUNCTION WRITE.STKIF PUBLIC
\****************************

    INTEGER*2 WRITE.STKIF
    
    WRITE.STKIF = 1
                 
    STKIF.KEY$ = STKIF.ITEM.CODE$ + STKIF.LOCATION$
    IF END #STKIF.SESS.NUM% THEN WRITE.STKIF.ERROR
    WRITE FORM "C12,C1,2C3,C24,C2,I4"; #STKIF.SESS.NUM%;               \*Ver C*
       STKIF.KEY$,                                                     \
       STKIF.BUSINESS.CENTRE$,                                         \
       STKIF.GROUP.SEQUENCE$,                                          \
       STKIF.QUANTITY$,                                                \
       STKIF.DESCRIPTION$,                                             \
       STKIF.STOCKTAKER.NUM$,                                          \
       STKIF.STKMF.RECORD.NUM%
         

    WRITE.STKIF = 0
    EXIT FUNCTION
    
WRITE.STKIF.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKIF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION



