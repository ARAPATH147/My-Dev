\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Stock File
\***                 Public File Function Definitions
\***
\***    FILE TYPE : Keyed
\***
\***********************************************************************
\***
\***    Version A.          Les Cook                     21st Aug 1992
\***    Initial version.
\***
\***    Version B.          Robert Cowey                 21st Oct 1993
\***    Corrected setting of FILE.OPERATION$ within WRITE functions.
\***
\***    Version C.          Nik Sen                      14th Dec 1994
\***    WRITE.STOCK.HOLD added.
\***
\***    Version D.          Mark Walker                  24th Jan 2014
\***    F337 Centralised View of Stock
\***    - Added sequence ID and item status to all reads and writes.
\***    - Removed hardcoded filler value.
\***    - Minor formatting changes (uncommented).
\***
\***    Version E.          Mark Walker                  15th Mar 2014
\***    F337 Centralised View of Stock
\***    Added next sequence ID field.
\***
\***    Version F.          Mark Walker                   7th May 2014
\***    F337 Centralised View of Stock
\***    Added new 'stock flags' field for future use.
\***
\***********************************************************************
\***********************************************************************

    INTEGER*2 GLOBAL                                                    \
       CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
       CURRENT.CODE$,                                                   \
       FILE.OPERATION$

%INCLUDE STOCKDEC.J86                                                       !BRC

FUNCTION STOCK.SET PUBLIC

    STOCK.REPORT.NUM% = 108
    STOCK.RECL%       = 30
    STOCK.FILE.NAME$  = "STOCK"

END FUNCTION

FUNCTION READ.STOCK PUBLIC

    INTEGER*2   READ.STOCK
    STRING      FORMAT.STRING$                                              !DMW

    READ.STOCK = 1

    STOCK.NEXT.SID% = 0                                                     !EMW

    FORMAT.STRING$ = "T5,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !FMW

    IF END #STOCK.SESS.NUM% THEN READ.STOCK.ERROR
    READ FORM FORMAT.STRING$; #STOCK.SESS.NUM%                          \   !DMW
        KEY STOCK.BOOTS.CODE$;      \ Item Code                         \
            STOCK.STOCK.FIG%,       \ Stock Figure                      \
            STOCK.LAST.COUNT%,      \ Last Count Quantity               \
            STOCK.DATE.LAST.COUNT$, \ Date of Last Count                \
            STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement             \
            STOCK.LAST.REC%,        \ Last Receipt Quantity             \
            STOCK.DATE.LAST.REC$,   \ Date of Last Receipt              \
            STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                  \
            STOCK.SID%,             \ Sequence ID                       \   !DMW
            STOCK.STATUS.1$,        \ Item Status                       \   !DMW
            STOCK.FLAGS%,           \ Stock flags                       \   !FMW
            STOCK.FILLER$           ! Filler
    
    READ.STOCK = 0
    
    ! Get the next sequence ID                                              !EMW
    STOCK.NEXT.SID% = STOCK.SID% + 1                                        !EMW
                                                                            !EMW
    ! IF the sequence ID has wrapped                                        !EMW
    IF STOCK.NEXT.SID% < 0 THEN BEGIN                                       !EMW
        ! Re-initialise sequence ID                                         !EMW
        STOCK.NEXT.SID% = 0                                                 !EMW
    ENDIF                                                                   !EMW
    
    EXIT FUNCTION

READ.STOCK.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

FUNCTION WRITE.STOCK PUBLIC

    INTEGER*2   WRITE.STOCK
    STRING      FORMAT.STRING$                                              !DMW

    WRITE.STOCK = 1

    FORMAT.STRING$ = "C4,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !DMW
    
    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.ERROR
    WRITE FORM FORMAT.STRING$; #STOCK.SESS.NUM%;                        \   !DMW
        STOCK.BOOTS.CODE$,      \ Item Code                             \
        STOCK.STOCK.FIG%,       \ Stock Figure                          \
        STOCK.LAST.COUNT%,      \ Last Count Quantity                   \
        STOCK.DATE.LAST.COUNT$, \ Date of Last Count                    \
        STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement                 \
        STOCK.LAST.REC%,        \ Last Receipt Quantity                 \
        STOCK.DATE.LAST.REC$,   \ Date of Last Receipt                  \
        STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                      \
        STOCK.SID%,             \ Sequence ID                           \   !DMW
        STOCK.STATUS.1$,        \ Item Status                           \   !DMW
        STOCK.FLAGS%,           \ Stock flags                           \   !FMW
        STOCK.FILLER$           ! Filler                                    !DMW
   
    WRITE.STOCK = 0
   
    EXIT FUNCTION

WRITE.STOCK.ERROR:

    FILE.OPERATION$     = "W"                                               !BRC
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

FUNCTION WRITE.STOCK.HOLD PUBLIC

    INTEGER*2 WRITE.STOCK.HOLD
    STRING      FORMAT.STRING$                                              !DMW

    WRITE.STOCK.HOLD = 1
    
    FORMAT.STRING$ = "C4,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !FMW

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #STOCK.SESS.NUM%;                   \   !DMW
        STOCK.BOOTS.CODE$,      \ Item Code                             \
        STOCK.STOCK.FIG%,       \ Stock Figure                          \
        STOCK.LAST.COUNT%,      \ Last Count Quantity                   \
        STOCK.DATE.LAST.COUNT$, \ Date of Last Count                    \
        STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement                 \
        STOCK.LAST.REC%,        \ Last Receipt Quantity                 \
        STOCK.DATE.LAST.REC$,   \ Date of Last Receipt                  \
        STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                      \
        STOCK.SID%,             \ Sequence ID                           \   !DMW
        STOCK.STATUS.1$,        \ Item Status                           \   !DMW
        STOCK.FLAGS%,           \ Stock flags                           \   !FMW
        STOCK.FILLER$           ! Filler                                    !DMW
   
    WRITE.STOCK.HOLD = 0
    
    EXIT FUNCTION

WRITE.STOCK.HOLD.ERROR:

    FILE.OPERATION$     = "W"                                               !BRC
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

FUNCTION READ.STOCK.LOCK PUBLIC

    INTEGER*2   READ.STOCK.LOCK
    STRING      FORMAT.STRING$                                              !DMW

    READ.STOCK.LOCK = 1

    STOCK.NEXT.SID% = 0                                                     !EMW
    
    FORMAT.STRING$ = "T5,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !FMW

    IF END #STOCK.SESS.NUM% THEN READ.STOCK.LOCK.ERROR
    READ FORM FORMAT.STRING$; #STOCK.SESS.NUM% AUTOLOCK                 \   !DMW
        KEY STOCK.BOOTS.CODE$;      \ Item Code                         \
            STOCK.STOCK.FIG%,       \ Stock Figure                      \
            STOCK.LAST.COUNT%,      \ Last Count Quantity               \
            STOCK.DATE.LAST.COUNT$, \ Date of Last Count                \
            STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement             \
            STOCK.LAST.REC%,        \ Last Receipt Quantity             \
            STOCK.DATE.LAST.REC$,   \ Date of Last Receipt              \
            STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                  \
            STOCK.SID%,             \ Sequence ID                       \   !DMW
            STOCK.STATUS.1$,        \ Item Status                       \   !DMW
            STOCK.FLAGS%,           \ Stock flags                       \   !FMW
            STOCK.FILLER$           ! Filler
            
    READ.STOCK.LOCK = 0
    
    ! Get the next sequence ID                                              !EMW
    STOCK.NEXT.SID% = STOCK.SID% + 1                                        !EMW
                                                                            !EMW
    ! IF the sequence ID has wrapped                                        !EMW
    IF STOCK.NEXT.SID% < 0 THEN BEGIN                                       !EMW
        ! Re-initialise sequence ID                                         !EMW
        STOCK.NEXT.SID% = 0                                                 !EMW
    ENDIF                                                                   !EMW
    
    EXIT FUNCTION

READ.STOCK.LOCK.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

FUNCTION WRITE.STOCK.UNLOCK PUBLIC

    INTEGER*2   WRITE.STOCK.UNLOCK
    STRING      FORMAT.STRING$                                              !DMW
    
    WRITE.STOCK.UNLOCK = 1
    
    FORMAT.STRING$ = "C4,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !FMW

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.UNLOCK.ERROR
    WRITE FORM FORMAT.STRING$; #STOCK.SESS.NUM% AUTOUNLOCK ;            \   !DMW
        STOCK.BOOTS.CODE$,      \ Item Code                             \
        STOCK.STOCK.FIG%,       \ Stock Figure                          \
        STOCK.LAST.COUNT%,      \ Last Count Quantity                   \
        STOCK.DATE.LAST.COUNT$, \ Date of Last Count                    \
        STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement                 \
        STOCK.LAST.REC%,        \ Last Receipt Quantity                 \
        STOCK.DATE.LAST.REC$,   \ Date of Last Receipt                  \
        STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                      \
        STOCK.SID%,             \ Sequence ID                           \   !DMW
        STOCK.STATUS.1$,        \ Item Status                           \   !DMW
        STOCK.FLAGS%,           \ Stock flags                           \   !FMW
        STOCK.FILLER$           ! Filler                                    !DMW
                
    WRITE.STOCK.UNLOCK = 0
    
    EXIT FUNCTION

WRITE.STOCK.UNLOCK.ERROR:

    FILE.OPERATION$     = "W"                                               !BRC
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

FUNCTION WRITE.STOCK.UNLOCK.HOLD PUBLIC

    INTEGER*2   WRITE.STOCK.UNLOCK.HOLD
    STRING      FORMAT.STRING$                                              !DMW

    WRITE.STOCK.UNLOCK.HOLD = 1
    
    FORMAT.STRING$ = "C4,2I2,2C3,I2,2C3,I4,C1,I1,C2"                        !FMW

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.UNLOCK.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #STOCK.SESS.NUM% AUTOUNLOCK ;       \   !DMW
        STOCK.BOOTS.CODE$,      \ Item Code                             \
        STOCK.STOCK.FIG%,       \ Stock Figure                          \
        STOCK.LAST.COUNT%,      \ Last Count Quantity                   \
        STOCK.DATE.LAST.COUNT$, \ Date of Last Count                    \
        STOCK.DATE.LAST.MOVE$,  \ Date of Last Movement                 \
        STOCK.LAST.REC%,        \ Last Receipt Quantity                 \
        STOCK.DATE.LAST.REC$,   \ Date of Last Receipt                  \
        STOCK.DATE.LAST.GAP$,   \ Date of Last Gap                      \
        STOCK.SID%,             \ Sequence ID                           \   !DMW
        STOCK.STATUS.1$,        \ Item Status                           \   !DMW
        STOCK.FLAGS%,           \ Stock flags                           \   !FMW
        STOCK.FILLER$           ! Filler                                    !DMW
                
    WRITE.STOCK.UNLOCK.HOLD = 0
    
    EXIT FUNCTION

WRITE.STOCK.UNLOCK.HOLD.ERROR:

    FILE.OPERATION$     = "W"                                               !BRC
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%
    CURRENT.CODE$       = STOCK.BOOTS.CODE$

END FUNCTION

