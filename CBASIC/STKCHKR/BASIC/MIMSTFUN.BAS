\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Item Movement & Stock File (Merged)
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

    %INCLUDE MIMSTDEC.J86

FUNCTION MIMSTC.SET PUBLIC

    MIMSTC.REPORT.NUM% = 703
    MIMSTC.RECL%       = 40
    MIMSTC.FILE.NAME$  = "ADXLXACN::C:\ADX_IDT1\MIMSTC.BIN"

END FUNCTION

FUNCTION READ.MIMSTC PUBLIC

    INTEGER*1   READ.MIMSTC
    STRING      FORMAT.STRING$

    READ.MIMSTC = 1
    
    FORMAT.STRING$ = "T12,5I4,C1,I2,C1,I4,C1"

    IF END #MIMSTC.SESS.NUM% THEN READ.MIMSTC.ERROR
    READ FORM FORMAT.STRING$; #MIMSTC.SESS.NUM%                         \
        KEY MIMSTC.BAR.CODE$;               \ Barcode                   \
            MIMSTC.RESTART%,                \ Restart pointer           \
            MIMSTC.NUMITEMS%,               \ Number of items sold      \
            MIMSTC.AMTSALE%,                \ Amount of items sold      \
            MIMSTC.RESERVED%,               \ RESERVED by IBM           \
            MIMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer     \
            MIMSTC.STATUS.FLAG$,            \ Status flags              \
            MIMSTC.STOCK.FIGURE%,           \ Stock Figure              \
            MIMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason           \
            MIMSTC.SID%,                    \ Sequence ID               \
            MIMSTC.FILLER$                  ! Filler
    
    READ.MIMSTC = 0
    
    EXIT FUNCTION

READ.MIMSTC.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%
    CURRENT.CODE$       = MIMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.MIMSTC PUBLIC

    INTEGER*1   WRITE.MIMSTC
    STRING      FORMAT.STRING$

    WRITE.MIMSTC = 1

    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"

    IF END #MIMSTC.SESS.NUM% THEN WRITE.MIMSTC.ERROR
    WRITE FORM FORMAT.STRING$; #MIMSTC.SESS.NUM%;                       \
        MIMSTC.BAR.CODE$,               \ Barcode                       \
        MIMSTC.RESTART%,                \ Restart pointer               \
        MIMSTC.NUMITEMS%,               \ Number of items sold          \
        MIMSTC.AMTSALE%,                \ Amount of items sold          \
        MIMSTC.RESERVED%,               \ RESERVED by IBM               \
        MIMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer         \
        MIMSTC.STATUS.FLAG$,            \ Status flags                  \
        MIMSTC.STOCK.FIGURE%,           \ Stock Figure                  \
        MIMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason               \
        MIMSTC.SID%,                    \ Sequence ID                   \
        MIMSTC.FILLER$                  ! Filler
    
    WRITE.MIMSTC = 0
    
    EXIT FUNCTION

WRITE.MIMSTC.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%
    CURRENT.CODE$       = MIMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.MIMSTC.HOLD PUBLIC

    INTEGER*1   WRITE.MIMSTC.HOLD
    STRING      FORMAT.STRING$

    WRITE.MIMSTC.HOLD = 1
    
    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"

    IF END #MIMSTC.SESS.NUM% THEN WRITE.MIMSTC.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #MIMSTC.SESS.NUM%;                  \
        MIMSTC.BAR.CODE$,               \ Barcode                       \
        MIMSTC.RESTART%,                \ Restart pointer               \
        MIMSTC.NUMITEMS%,               \ Number of items sold          \
        MIMSTC.AMTSALE%,                \ Amount of items sold          \
        MIMSTC.RESERVED%,               \ RESERVED by IBM               \
        MIMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer         \
        MIMSTC.STATUS.FLAG$,            \ Status flags                  \
        MIMSTC.STOCK.FIGURE%,           \ Stock Figure                  \
        MIMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason               \
        MIMSTC.SID%,                    \ Sequence ID                   \
        MIMSTC.FILLER$                  ! Filler
    
    WRITE.MIMSTC.HOLD = 0
    
    EXIT FUNCTION

WRITE.MIMSTC.HOLD.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%
    CURRENT.CODE$       = MIMSTC.BAR.CODE$

END FUNCTION

