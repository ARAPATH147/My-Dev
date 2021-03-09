\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Item Movement & Stock File (Copy)
\***                 File Functions
\***
\***    FILE TYPE : Keyed
\***
\***********************************************************************
\***
\***    Version A.          Mark Walker                  23rd Jan 2014
\***    Initial version (moved from IMSTCFUN.BAS)
\***
\***********************************************************************
\***********************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

%INCLUDE CIMSTDEC.J86

FUNCTION CIMSTC.SET PUBLIC

    CIMSTC.REPORT.NUM% = 704
    CIMSTC.RECL%       = 40
    CIMSTC.FILE.NAME$  = "ADXLXACN::C:\ADX_IDT1\CIMSTC.BIN"

END FUNCTION

FUNCTION READ.CIMSTC PUBLIC

    INTEGER*1   READ.CIMSTC
    STRING      FORMAT.STRING$

    READ.CIMSTC = 1
    
    FORMAT.STRING$ = "T12,5I4,C1,I2,C1,I4,C1"

    IF END #CIMSTC.SESS.NUM% THEN READ.CIMSTC.ERROR
    READ FORM FORMAT.STRING$; #CIMSTC.SESS.NUM%                         \
        KEY CIMSTC.BAR.CODE$;               \ Barcode                   \
            CIMSTC.RESTART%,                \ Restart pointer           \
            CIMSTC.NUMITEMS%,               \ Number of items sold      \
            CIMSTC.AMTSALE%,                \ Amount of items sold      \
            CIMSTC.RESERVED%,               \ RESERVED by IBM           \
            CIMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer     \
            CIMSTC.STATUS.FLAG$,            \ Status flags              \
            CIMSTC.STOCK.FIGURE%,           \ Stock Figure              \
            CIMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason           \
            CIMSTC.SID%,                    \ Sequence ID               \
            CIMSTC.FILLER$                  ! Filler
    
    READ.CIMSTC = 0
    
    EXIT FUNCTION

READ.CIMSTC.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = CIMSTC.REPORT.NUM%
    CURRENT.CODE$       = CIMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.CIMSTC PUBLIC

    INTEGER*1   WRITE.CIMSTC
    STRING      FORMAT.STRING$

    WRITE.CIMSTC = 1

    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"
    
    IF END #CIMSTC.SESS.NUM% THEN WRITE.CIMSTC.ERROR
    WRITE FORM FORMAT.STRING$; #CIMSTC.SESS.NUM%;                       \
        CIMSTC.BAR.CODE$,               \ Barcode                       \
        CIMSTC.RESTART%,                \ Restart pointer               \
        CIMSTC.NUMITEMS%,               \ Number of items sold          \
        CIMSTC.AMTSALE%,                \ Amount of items sold          \
        CIMSTC.RESERVED%,               \ RESERVED by IBM               \
        CIMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer         \
        CIMSTC.STATUS.FLAG$,            \ Status flags                  \
        CIMSTC.STOCK.FIGURE%,           \ Stock Figure                  \
        CIMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason               \
        CIMSTC.SID%,                    \ Sequence ID                   \
        CIMSTC.FILLER$                  ! Filler
    
    WRITE.CIMSTC = 0
    
    EXIT FUNCTION

WRITE.CIMSTC.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = CIMSTC.REPORT.NUM%
    CURRENT.CODE$       = CIMSTC.BAR.CODE$

END FUNCTION

