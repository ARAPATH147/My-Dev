\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKTFFUN.BAS  $
\***
\***   $Revision:   1.2  $
\***
\******************************************************************************
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE TOTALS FILE FUNCTIONS
\***
\***               REFERENCE    : STKTFFUN.BAS
\***
\***         VERSION A            Nik Sen         15th July 1997
\***
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE STKTFDEC.J86                                              

  FUNCTION STKTF.SET PUBLIC
\***************************

    STKTF.REPORT.NUM% = 534
    STKTF.RECL%       = 11
    STKTF.FILE.NAME$  = "STKTF"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.STKTF PUBLIC
\****************************

    INTEGER*2 READ.STKTF
    
    READ.STKTF = 1
                 

    IF END #STKTF.SESS.NUM% THEN READ.STKTF.ERROR
    READ FORM "T5,C3,C1,C3"; #STKTF.SESS.NUM% KEY STKTF.BOOTS.CODE$;     \
       STKTF.QUANTITY$,                                                \
       STKTF.BUSINESS.CENTRE$,                                         \
       STKTF.GROUP.SEQUENCE$                                          
         

    READ.STKTF = 0
    EXIT FUNCTION
    
READ.STKTF.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKTF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  
FUNCTION WRITE.STKTF PUBLIC
\****************************

    INTEGER*2 WRITE.STKTF
    
    WRITE.STKTF = 1
                 

    IF END #STKTF.SESS.NUM% THEN WRITE.STKTF.ERROR
    WRITE FORM "C4,C3,C1,C3"; #STKTF.SESS.NUM%;                   \
       STKTF.BOOTS.CODE$,                                                     \
       STKTF.QUANTITY$,                                                \
       STKTF.BUSINESS.CENTRE$,                                         \
       STKTF.GROUP.SEQUENCE$                                          


    WRITE.STKTF = 0
    EXIT FUNCTION
    
WRITE.STKTF.ERROR:
    

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STKTF.REPORT.NUM%
       
       EXIT FUNCTION

END FUNCTION



