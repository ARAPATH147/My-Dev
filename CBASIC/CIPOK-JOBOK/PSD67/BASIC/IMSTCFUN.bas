\******************************************************************************
\******************************************************************************
\***
\***          CURRENT ITEM MOVEMENT & STOCK FILE FUNCTIONS
\***
\***               REFERENCE    : IMSTCFUN.BAS
\***
\***          VERSION A       Les Cook     21st August 1992
\***
\*****************************************************************************
\***            VERSION 1.1 JULIA STONES     13/6/05
\***            Added new external functions for
\***            MIMSTC - Merged IMSTC
\***            CIMSTC - Copy of IMSTC
\***            BIMSTC - Backup of IMSTC
\***
\******************************************************************************
\******************************************************************************

   INTEGER*2 GLOBAL               \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                  \
      CURRENT.CODE$,              \
      FILE.OPERATION$

   %INCLUDE IMSTCDEC.J86

  FUNCTION IMSTC.SET PUBLIC
\***************************

     IMSTC.REPORT.NUM% = 31
     IMSTC.RECL%       = 40
     IMSTC.FILE.NAME$  = "IMSTC"

  END FUNCTION

  FUNCTION MIMSTC.SET PUBLIC                                  ! 1.1JAS
\***************************
                                                              ! 1.1JAS
     MIMSTC.REPORT.NUM% = 703                                 ! 1.1JAS
     MIMSTC.RECL%       = 40                                  ! 1.1JAS
     MIMSTC.FILE.NAME$  = "ADXLXACN::C:\ADX_IDT1\MIMSTC.BIN"  ! 1.1JAS

  END FUNCTION                                                ! 1.1JAS

  FUNCTION CIMSTC.SET PUBLIC                                  ! 1.1JAS
\***************************
                                                              ! 1.1JAS
     CIMSTC.REPORT.NUM% = 704                                 ! 1.1JAS
     CIMSTC.RECL%       = 40                                  ! 1.1JAS
     CIMSTC.FILE.NAME$  = "ADXLXACN::C:\ADX_IDT1\CIMSTC.BIN"  ! 1.1JAS

  END FUNCTION                                                ! 1.1JAS

  FUNCTION BIMSTC.SET PUBLIC                                  ! 1.1JAS
\***************************
                                                              ! 1.1JAS
     BIMSTC.REPORT.NUM% = 705                                 ! 1.1JAS
     BIMSTC.RECL%       = 40                                  ! 1.1JAS
     BIMSTC.FILE.NAME$  = "ADXLXACN::C:\ADX_IDT1\BIMSTC.BIN"  ! 1.1JAS

  END FUNCTION                                                ! 1.1JAS

\-----------------------------------------------------------------------------

  FUNCTION READ.IMSTC PUBLIC
\****************************

    INTEGER*1 READ.IMSTC

    READ.IMSTC = 1

    IF END #IMSTC.SESS.NUM% THEN READ.IMSTC.ERROR
    READ FORM "T12,5I4,C1,I2,C1,C5"; #IMSTC.SESS.NUM%                 \
         KEY IMSTC.BAR.CODE$;                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    READ.IMSTC = 0
    EXIT FUNCTION

    READ.IMSTC.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "R"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION READ.MIMSTC PUBLIC                                             ! 1.1JAS
\****************************                                             ! 1.1JAS

    INTEGER*1 READ.MIMSTC                                                 ! 1.1JAS

    READ.MIMSTC = 1                                                       ! 1.1JAS

    IF END #MIMSTC.SESS.NUM% THEN READ.MIMSTC.ERROR                       ! 1.1JAS
    READ FORM "T12,5I4,C1,I2,C1,C5"; #MIMSTC.SESS.NUM%                \   ! 1.1JAS
         KEY MIMSTC.BAR.CODE$;                                         \  ! 1.1JAS
             MIMSTC.RESTART%,                                          \  ! 1.1JAS
             MIMSTC.NUMITEMS%,                                         \  ! 1.1JAS
             MIMSTC.AMTSALE%,                                          \  ! 1.1JAS
             MIMSTC.RESERVED%,                                         \  ! 1.1JAS
             MIMSTC.STKMQ.RESTART%,                                    \  ! 1.1JAS
             MIMSTC.STATUS.FLAG$,                                      \  ! 1.1JAS
             MIMSTC.STOCK.FIGURE%,                                     \  ! 1.1JAS
             MIMSTC.REASON.ITEM.REMOVED$,                              \  ! 1.1JAS
             MIMSTC.FILLER$                                               ! 1.1JAS
    READ.MIMSTC = 0                                                       ! 1.1JAS
    EXIT FUNCTION                                                         ! 1.1JAS

    READ.MIMSTC.ERROR:                                                    ! 1.1JAS

       CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%                           ! 1.1JAS
       FILE.OPERATION$ = "R"                                              ! 1.1JAS
       CURRENT.CODE$ = MIMSTC.BAR.CODE$                                   ! 1.1JAS

       EXIT FUNCTION                                                      ! 1.1JAS

  END FUNCTION                                                            ! 1.1JAS

\-----------------------------------------------------------------------------

  FUNCTION READ.CIMSTC PUBLIC                                             ! 1.1JAS
\****************************                                             ! 1.1JAS

    INTEGER*1 READ.CIMSTC                                                 ! 1.1JAS

    READ.CIMSTC = 1                                                       ! 1.1JAS

    IF END #CIMSTC.SESS.NUM% THEN READ.CIMSTC.ERROR                       ! 1.1JAS
    READ FORM "T12,5I4,C1,I2,C1,C5"; #CIMSTC.SESS.NUM%                \   ! 1.1JAS
         KEY CIMSTC.BAR.CODE$;                                         \  ! 1.1JAS
             CIMSTC.RESTART%,                                          \  ! 1.1JAS
             CIMSTC.NUMITEMS%,                                         \  ! 1.1JAS
             CIMSTC.AMTSALE%,                                          \  ! 1.1JAS
             CIMSTC.RESERVED%,                                         \  ! 1.1JAS
             CIMSTC.STKMQ.RESTART%,                                    \  ! 1.1JAS
             CIMSTC.STATUS.FLAG$,                                      \  ! 1.1JAS
             CIMSTC.STOCK.FIGURE%,                                     \  ! 1.1JAS
             CIMSTC.REASON.ITEM.REMOVED$,                              \  ! 1.1JAS
             CIMSTC.FILLER$                                               ! 1.1JAS
    READ.CIMSTC = 0                                                       ! 1.1JAS
    EXIT FUNCTION                                                         ! 1.1JAS

    READ.CIMSTC.ERROR:                                                    ! 1.1JAS

       CURRENT.REPORT.NUM% = CIMSTC.REPORT.NUM%                           ! 1.1JAS
       FILE.OPERATION$ = "R"                                              ! 1.1JAS
       CURRENT.CODE$ = CIMSTC.BAR.CODE$                                   ! 1.1JAS

       EXIT FUNCTION                                                      ! 1.1JAS

  END FUNCTION                                                            ! 1.1JAS

\-----------------------------------------------------------------------------

  FUNCTION READ.IMSTC.LOCK PUBLIC
\*********************************

    INTEGER*1 READ.IMSTC.LOCK

    READ.IMSTC.LOCK = 1

    IF END #IMSTC.SESS.NUM% THEN READ.IMSTC.LOCK.ERROR
    READ FORM "T12,5I4,C1,I2,C1,C5"; #IMSTC.SESS.NUM% AUTOLOCK           \
         KEY IMSTC.BAR.CODE$;                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    READ.IMSTC.LOCK = 0
    EXIT FUNCTION

    READ.IMSTC.LOCK.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "R"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.IMSTC.UNLOCK.HOLD PUBLIC
\*****************************************

    INTEGER*1 WRITE.IMSTC.UNLOCK.HOLD

    WRITE.IMSTC.UNLOCK.HOLD = 1

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.UNLOCK.HOLD.ERROR
    WRITE FORM "C11,5I4,C1,I2,C1,C5"; HOLD #IMSTC.SESS.NUM% AUTOUNLOCK;  \
             IMSTC.BAR.CODE$,                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    WRITE.IMSTC.UNLOCK.HOLD = 0
    EXIT FUNCTION

    WRITE.IMSTC.UNLOCK.HOLD.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "O"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.IMSTC.UNLOCK PUBLIC
\************************************

    INTEGER*1 WRITE.IMSTC.UNLOCK

    WRITE.IMSTC.UNLOCK = 1

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.UNLOCK.ERROR
    WRITE FORM "C11,5I4,C1,I2,C1,C5";  #IMSTC.SESS.NUM% AUTOUNLOCK;   \
             IMSTC.BAR.CODE$,                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    WRITE.IMSTC.UNLOCK = 0
    EXIT FUNCTION

    WRITE.IMSTC.UNLOCK.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "O"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.IMSTC.HOLD PUBLIC
\**********************************

    INTEGER*1 WRITE.IMSTC.HOLD

    WRITE.IMSTC.HOLD = 1

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.HOLD.ERROR
    WRITE FORM "C11,5I4,C1,I2,C1,C5"; HOLD #IMSTC.SESS.NUM%;          \
             IMSTC.BAR.CODE$,                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    WRITE.IMSTC.HOLD = 0
    EXIT FUNCTION

    WRITE.IMSTC.HOLD.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "O"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.IMSTC PUBLIC
\*****************************

    INTEGER*1 WRITE.IMSTC

    WRITE.IMSTC = 1

    IF END #IMSTC.SESS.NUM% THEN WRITE.IMSTC.ERROR
    WRITE FORM "C11,5I4,C1,I2,C1,C5"; #IMSTC.SESS.NUM%;               \
             IMSTC.BAR.CODE$,                                         \
             IMSTC.RESTART%,                                          \
             IMSTC.NUMITEMS%,                                         \
             IMSTC.AMTSALE%,                                          \
             IMSTC.RESERVED%,                                         \
             IMSTC.STKMQ.RESTART%,                                    \
             IMSTC.STATUS.FLAG$,                                      \
             IMSTC.STOCK.FIGURE%,                                     \
             IMSTC.REASON.ITEM.REMOVED$,                              \
             IMSTC.FILLER$
    WRITE.IMSTC = 0
    EXIT FUNCTION

    WRITE.IMSTC.ERROR:

       CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
       FILE.OPERATION$ = "O"
       CURRENT.CODE$ = IMSTC.BAR.CODE$

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.MIMSTC.HOLD PUBLIC                                       ! 1.1JAS
\**********************************                                       ! 1.1JAS

    INTEGER*1 WRITE.MIMSTC.HOLD                                           ! 1.1JAS

    WRITE.MIMSTC.HOLD = 1                                                 ! 1.1JAS

    IF END #MIMSTC.SESS.NUM% THEN WRITE.MIMSTC.HOLD.ERROR                 ! 1.1JAS
    WRITE FORM "C11,5I4,C1,I2,C1,C5"; HOLD #MIMSTC.SESS.NUM%;         \   ! 1.1JAS
             MIMSTC.BAR.CODE$,                                        \   ! 1.1JAS
             MIMSTC.RESTART%,                                         \   ! 1.1JAS
             MIMSTC.NUMITEMS%,                                        \   ! 1.1JAS
             MIMSTC.AMTSALE%,                                         \   ! 1.1JAS
             MIMSTC.RESERVED%,                                        \   ! 1.1JAS
             MIMSTC.STKMQ.RESTART%,                                   \   ! 1.1JAS
             MIMSTC.STATUS.FLAG$,                                     \   ! 1.1JAS
             MIMSTC.STOCK.FIGURE%,                                    \   ! 1.1JAS
             MIMSTC.REASON.ITEM.REMOVED$,                             \   ! 1.1JAS
             MIMSTC.FILLER$                                               ! 1.1JAS
    WRITE.MIMSTC.HOLD = 0                                                 ! 1.1JAS
    EXIT FUNCTION                                                         ! 1.1JAS

    WRITE.MIMSTC.HOLD.ERROR:                                              ! 1.1JAS

       CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%                           ! 1.1JAS
       FILE.OPERATION$ = "O"                                              ! 1.1JAS
       CURRENT.CODE$ = MIMSTC.BAR.CODE$                                   ! 1.1JAS

       EXIT FUNCTION                                                      ! 1.1JAS

  END FUNCTION                                                            ! 1.1JAS

\-----------------------------------------------------------------------------

  FUNCTION WRITE.MIMSTC PUBLIC                                            ! 1.1JAS
\*****************************                                            ! 1.1JAS

    INTEGER*1 WRITE.MIMSTC                                                ! 1.1JAS

    WRITE.MIMSTC = 1                                                      ! 1.1JAS

    IF END #MIMSTC.SESS.NUM% THEN WRITE.MIMSTC.ERROR                      ! 1.1JAS
    WRITE FORM "C11,5I4,C1,I2,C1,C5"; #MIMSTC.SESS.NUM%;               \  ! 1.1JAS
             MIMSTC.BAR.CODE$,                                         \  ! 1.1JAS
             MIMSTC.RESTART%,                                          \  ! 1.1JAS
             MIMSTC.NUMITEMS%,                                         \  ! 1.1JAS
             MIMSTC.AMTSALE%,                                          \  ! 1.1JAS
             MIMSTC.RESERVED%,                                         \  ! 1.1JAS
             MIMSTC.STKMQ.RESTART%,                                    \  ! 1.1JAS
             MIMSTC.STATUS.FLAG$,                                      \  ! 1.1JAS
             MIMSTC.STOCK.FIGURE%,                                     \  ! 1.1JAS
             MIMSTC.REASON.ITEM.REMOVED$,                              \  ! 1.1JAS
             MIMSTC.FILLER$                                               ! 1.1JAS
    WRITE.MIMSTC = 0                                                      ! 1.1JAS
    EXIT FUNCTION                                                         ! 1.1JAS

    WRITE.MIMSTC.ERROR:                                                   ! 1.1JAS

       CURRENT.REPORT.NUM% = MIMSTC.REPORT.NUM%                           ! 1.1JAS
       FILE.OPERATION$ = "O"                                              ! 1.1JAS
       CURRENT.CODE$ = MIMSTC.BAR.CODE$                                   ! 1.1JAS

       EXIT FUNCTION                                                      ! 1.1JAS

  END FUNCTION                                                            ! 1.1JAS
