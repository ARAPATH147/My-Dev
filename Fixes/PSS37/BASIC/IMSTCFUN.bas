\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Item Movement & Stock File
\***                 Public File Function Definitions
\***
\***    FILE TYPE : Keyed
\***
\***********************************************************************
\***
\***    Version A.          Les Cook                     21st Aug 1992
\***    Initial version.
\***
\***    Version B.          Julia Stones                 13th Jun 2005
\***    Added new external functions for:
\***        MIMSTC - Merged IMSTC
\***        CIMSTC - Copy of IMSTC
\***        BIMSTC - Backup of IMSTC
\***
\***    Version C.          Mark Walker                  23rd Jan 2014
\***    F337 Centralised View of Stock
\***    - Added sequence ID to all reads and writes.
\***    - When reading, calculate the value of the next sequence ID.
\***    - Fixed error handling file operation flag for writes.
\***    - Moved BIMSTC, CIMSTC and MIMSTC functions to separate modules.
\***    - Minor formatting changes (uncommented).
\***
\***    Version D.          Mark Walker                  15th Mar 2014
\***    F337 Centralised View of Stock
\***    Added initialisation of next sequence ID field.
\***
\***    Version E.          Mark Walker                  30th Apr 2014
\***    F337 Centralised View of Stock
\***    QC599: Introduced item level TSL restart pointer to allow
\***           Sales Support restart/recovery to continue processing
\***           correctly from the next unprocessed item.
\***
\***********************************************************************
\***********************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

    %INCLUDE IMSTCDEC.J86

FUNCTION IMSTC.SET PUBLIC

    IMSTC.REPORT.NUM% = 31
    IMSTC.RECL%       = 40
    IMSTC.FILE.NAME$  = "IMSTC"

END FUNCTION

FUNCTION READ.IMSTC PUBLIC

    INTEGER*1   READ.IMSTC
    STRING      FORMAT.STRING$                                              !CMW

    READ.IMSTC = 1
    
    IMSTC.NEXT.SID% = 0                                                     !DMW
    IMSTC.RESERVED% = 0                                                     !EMW

    FORMAT.STRING$ = "T12,5I4,C1,I2,C1,I4,C1"                               !CMW
    
    IF END #IMSTC.SESS.NUM% THEN READ.IMSTC.ERROR
    READ FORM FORMAT.STRING$; #IMSTC.SESS.NUM%                          \   !CMW
         KEY IMSTC.BAR.CODE$;               \ Barcode                   \
             IMSTC.RESTART%,                \ Restart pointer           \
             IMSTC.NUMITEMS%,               \ Number of items sold      \
             IMSTC.AMTSALE%,                \ Amount of items sold      \
             IMSTC.TSL.RESTART%,            \ TSL Restart pointer       \   !EMW
             IMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer     \
             IMSTC.STATUS.FLAG$,            \ Status flags              \
             IMSTC.STOCK.FIGURE%,           \ Stock Figure              \
             IMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason           \
             IMSTC.SID%,                    \ Sequence ID               \   !CMW
             IMSTC.FILLER$                  ! Filler
                                                                          
    READ.IMSTC = 0

    ! Get the next sequence ID                                              !CMW
    IMSTC.NEXT.SID% = IMSTC.SID% + 1                                        !CMW
                                                                            !CMW
    ! IF the sequence ID has wrapped                                        !CMW
    IF IMSTC.NEXT.SID% < 0 THEN BEGIN                                       !CMW
        ! Re-initialise sequence ID                                         !CMW
        IMSTC.NEXT.SID% = 0                                                 !CMW
    ENDIF                                                                   !CMW

    EXIT FUNCTION

READ.IMSTC.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
    CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

FUNCTION READ.IMSTC.LOCK PUBLIC

    INTEGER*1   READ.IMSTC.LOCK
    STRING      FORMAT.STRING$                                              !CMW

    READ.IMSTC.LOCK = 1
    
    IMSTC.NEXT.SID% = 0                                                     !DMW
    IMSTC.RESERVED% = 0                                                     !EMW
    
    FORMAT.STRING$ = "T12,5I4,C1,I2,C1,I4,C1"                               !CMW

    IF END #IMSTC.SESS.NUM% THEN READ.IMSTC.LOCK.ERROR
    READ FORM FORMAT.STRING$; #IMSTC.SESS.NUM% AUTOLOCK                 \   !CMW
         KEY IMSTC.BAR.CODE$;               \ Barcode                   \
             IMSTC.RESTART%,                \ Restart pointer           \
             IMSTC.NUMITEMS%,               \ Number of items sold      \
             IMSTC.AMTSALE%,                \ Amount of items sold      \
             IMSTC.TSL.RESTART%,            \ TSL Restart pointer       \   !EMW
             IMSTC.STKMQ.RESTART%,          \ STKMQ Restart pointer     \
             IMSTC.STATUS.FLAG$,            \ Status flags              \
             IMSTC.STOCK.FIGURE%,           \ Stock Figure              \
             IMSTC.REASON.ITEM.REMOVED$,    \ Deletion Reason           \
             IMSTC.SID%,                    \ Sequence ID               \   !CMW
             IMSTC.FILLER$                  ! Filler
             
    READ.IMSTC.LOCK = 0
    
    ! Get the next sequence ID                                              !CMW
    IMSTC.NEXT.SID% = IMSTC.SID% + 1                                        !CMW
                                                                            !CMW
    ! IF the sequence ID has wrapped                                        !CMW
    IF IMSTC.NEXT.SID% < 0 THEN BEGIN                                       !CMW
        ! Re-initialise sequence ID                                         !CMW
        IMSTC.NEXT.SID% = 0                                                 !CMW
    ENDIF                                                                   !CMW
    
    EXIT FUNCTION

READ.IMSTC.LOCK.ERROR:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.IMSTC PUBLIC

    INTEGER*1   WRITE.IMSTC
    STRING      FORMAT.STRING$                                              !CMW

    WRITE.IMSTC = 1
    
    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"                               !CMW

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.ERROR
    WRITE FORM FORMAT.STRING$; #IMSTC.SESS.NUM%;                        \   !CMW
        IMSTC.BAR.CODE$,                \ Barcode                       \
        IMSTC.RESTART%,                 \ Restart pointer               \
        IMSTC.NUMITEMS%,                \ Number of items sold          \
        IMSTC.AMTSALE%,                 \ Amount of items sold          \
        IMSTC.TSL.RESTART%,             \ TSL Restart pointer           \   !EMW
        IMSTC.STKMQ.RESTART%,           \ STKMQ Restart pointer         \
        IMSTC.STATUS.FLAG$,             \ Status flags                  \
        IMSTC.STOCK.FIGURE%,            \ Stock Figure                  \
        IMSTC.REASON.ITEM.REMOVED$,     \ Deletion Reason               \
        IMSTC.SID%,                     \ Sequence ID                   \   !CMW
        IMSTC.FILLER$                   ! Filler
    
    WRITE.IMSTC = 0
    
    EXIT FUNCTION

WRITE.IMSTC.ERROR:

       FILE.OPERATION$     = "W"                                            !CMW
       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.IMSTC.UNLOCK.HOLD PUBLIC

    INTEGER*1   WRITE.IMSTC.UNLOCK.HOLD
    STRING      FORMAT.STRING$                                              !CMW

    WRITE.IMSTC.UNLOCK.HOLD = 1
    
    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"                               !CMW

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.UNLOCK.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #IMSTC.SESS.NUM% AUTOUNLOCK;        \   !CMW
        IMSTC.BAR.CODE$,                \ Barcode                       \
        IMSTC.RESTART%,                 \ Restart pointer               \
        IMSTC.NUMITEMS%,                \ Number of items sold          \
        IMSTC.AMTSALE%,                 \ Amount of items sold          \
        IMSTC.TSL.RESTART%,             \ TSL Restart pointer           \   !EMW
        IMSTC.STKMQ.RESTART%,           \ STKMQ Restart pointer         \
        IMSTC.STATUS.FLAG$,             \ Status flags                  \
        IMSTC.STOCK.FIGURE%,            \ Stock Figure                  \
        IMSTC.REASON.ITEM.REMOVED$,     \ Deletion Reason               \
        IMSTC.SID%,                     \ Sequence ID                   \   !CMW
        IMSTC.FILLER$                   ! Filler
        
    WRITE.IMSTC.UNLOCK.HOLD = 0
    
    EXIT FUNCTION

WRITE.IMSTC.UNLOCK.HOLD.ERROR:

       FILE.OPERATION$     = "W"                                            !CMW
       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.IMSTC.UNLOCK PUBLIC

    INTEGER*1   WRITE.IMSTC.UNLOCK
    STRING      FORMAT.STRING$                                              !CMW

    WRITE.IMSTC.UNLOCK = 1
    
    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"                               !CMW

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.UNLOCK.ERROR
    WRITE FORM FORMAT.STRING$;  #IMSTC.SESS.NUM% AUTOUNLOCK;            \   !CMW
        IMSTC.BAR.CODE$,                \ Barcode                       \
        IMSTC.RESTART%,                 \ Restart pointer               \
        IMSTC.NUMITEMS%,                \ Number of items sold          \
        IMSTC.AMTSALE%,                 \ Amount of items sold          \
        IMSTC.TSL.RESTART%,             \ TSL Restart pointer           \   !EMW
        IMSTC.STKMQ.RESTART%,           \ STKMQ Restart pointer         \
        IMSTC.STATUS.FLAG$,             \ Status flags                  \
        IMSTC.STOCK.FIGURE%,            \ Stock Figure                  \
        IMSTC.REASON.ITEM.REMOVED$,     \ Deletion Reason               \
        IMSTC.SID%,                     \ Sequence ID                   \   !CMW
        IMSTC.FILLER$                   ! Filler
        
    WRITE.IMSTC.UNLOCK = 0
    
    EXIT FUNCTION

WRITE.IMSTC.UNLOCK.ERROR:

       FILE.OPERATION$     = "W"                                            !CMW
       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

FUNCTION WRITE.IMSTC.HOLD PUBLIC

    INTEGER*1   WRITE.IMSTC.HOLD
    STRING      FORMAT.STRING$                                              !CMW

    WRITE.IMSTC.HOLD = 1

    FORMAT.STRING$ = "C11,5I4,C1,I2,C1,I4,C1"                               !CMW
    
    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #IMSTC.SESS.NUM%;                   \   !CMW
        IMSTC.BAR.CODE$,                \ Barcode                       \
        IMSTC.RESTART%,                 \ Restart pointer               \
        IMSTC.NUMITEMS%,                \ Number of items sold          \
        IMSTC.AMTSALE%,                 \ Amount of items sold          \
        IMSTC.TSL.RESTART%,             \ TSL Restart pointer           \   !EMW
        IMSTC.STKMQ.RESTART%,           \ STKMQ Restart pointer         \
        IMSTC.STATUS.FLAG$,             \ Status flags                  \
        IMSTC.STOCK.FIGURE%,            \ Stock Figure                  \
        IMSTC.REASON.ITEM.REMOVED$,     \ Deletion Reason               \
        IMSTC.SID%,                     \ Sequence ID                   \   !CMW
        IMSTC.FILLER$                   ! Filler
             
    WRITE.IMSTC.HOLD = 0
    
    EXIT FUNCTION

WRITE.IMSTC.HOLD.ERROR:

       FILE.OPERATION$     = "W"                                            !CMW
       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       CURRENT.CODE$       = IMSTC.BAR.CODE$

END FUNCTION

