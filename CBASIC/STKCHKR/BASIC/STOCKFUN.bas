\******************************************************************************
\******************************************************************************
\***
\***              INSTORE STOCK FILE FUNCTIONS
\***
\***               REFERENCE    : STOCKFUN.BAS
\***
\***         VERSION A            LES COOK         21/8/92
\***
\***    VERSION B.              ROBERT COWEY.                       21 OCT 1993.
\***    Corrected setting of FILE.OPERATION$ within WRITE functions.
\***
\***   VERSION C               Nik Sen                 14th December 1994
\***   WRITE.STOCK.HOLD added
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%

    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$

    %INCLUDE STOCKDEC.J86                                              ! BRC

  FUNCTION STOCK.SET PUBLIC
\***************************

    STOCK.REPORT.NUM% = 108
    STOCK.RECL%       = 30
    STOCK.FILE.NAME$  = "STOCK"

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION READ.STOCK PUBLIC
\****************************

    INTEGER*2 READ.STOCK

    READ.STOCK = 1

    IF END #STOCK.SESS.NUM% THEN READ.STOCK.ERROR
    READ FORM "T5,2I2,2C3,I2,2C3,C8"; #STOCK.SESS.NUM%          \
         KEY    STOCK.BOOTS.CODE$;                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                STOCK.FILLER$
    READ.STOCK = 0
    EXIT FUNCTION

    READ.STOCK.ERROR:

       CURRENT.CODE$ = STOCK.BOOTS.CODE$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------



  FUNCTION WRITE.STOCK PUBLIC
\*****************************

    INTEGER*2 WRITE.STOCK

    WRITE.STOCK = 1

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.ERROR
    WRITE FORM "C4,2I2,2C3,I2,2C3,C8"; #STOCK.SESS.NUM%;        \
                STOCK.BOOTS.CODE$,                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                PACK$(STRING$(8 * 2,"0"))
   WRITE.STOCK = 0
   EXIT FUNCTION

   WRITE.STOCK.ERROR:

      CURRENT.CODE$ = STOCK.BOOTS.CODE$
      FILE.OPERATION$ = "W"                                            ! BRC
      CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

      EXIT FUNCTION

  END FUNCTION


\-----------------------------------------------------------------------------


  FUNCTION WRITE.STOCK.HOLD PUBLIC
\*****************************

    INTEGER*2 WRITE.STOCK.HOLD

    WRITE.STOCK.HOLD = 1

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.HOLD.ERROR
    WRITE FORM "C4,2I2,2C3,I2,2C3,C8"; HOLD #STOCK.SESS.NUM%;   \
                STOCK.BOOTS.CODE$,                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                PACK$(STRING$(8 * 2,"0"))
   WRITE.STOCK.HOLD = 0
   EXIT FUNCTION

   WRITE.STOCK.HOLD.ERROR:

      CURRENT.CODE$ = STOCK.BOOTS.CODE$
      FILE.OPERATION$ = "W"                                            ! BRC
      CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

      EXIT FUNCTION

  END FUNCTION


\-----------------------------------------------------------------------------


  FUNCTION READ.STOCK.LOCK PUBLIC
\*********************************

    INTEGER*2 READ.STOCK.LOCK

    READ.STOCK.LOCK = 1

    IF END #STOCK.SESS.NUM% THEN READ.STOCK.LOCK.ERROR
    READ FORM "T5,2I2,2C3,I2,2C3,C8"; #STOCK.SESS.NUM%          \
        AUTOLOCK                                                \
        KEY     STOCK.BOOTS.CODE$;                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                STOCK.FILLER$
    READ.STOCK.LOCK = 0
    EXIT FUNCTION

    READ.STOCK.LOCK.ERROR:

       CURRENT.CODE$ = STOCK.BOOTS.CODE$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------



  FUNCTION WRITE.STOCK.UNLOCK PUBLIC
\************************************

    INTEGER*2 WRITE.STOCK.UNLOCK

    WRITE.STOCK.UNLOCK = 1

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.UNLOCK.ERROR
    WRITE FORM "C4,2I2,2C3,I2,2C3,C8"; #STOCK.SESS.NUM%         \
          AUTOUNLOCK ;                                          \
                STOCK.BOOTS.CODE$,                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                PACK$(STRING$(8 * 2,"0"))
    WRITE.STOCK.UNLOCK = 0
    EXIT FUNCTION

    WRITE.STOCK.UNLOCK.ERROR:

    CURRENT.CODE$ = STOCK.BOOTS.CODE$
    FILE.OPERATION$ = "W"                                              ! BRC
    CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION WRITE.STOCK.UNLOCK.HOLD PUBLIC
\*****************************************

    INTEGER*2 WRITE.STOCK.UNLOCK.HOLD

    WRITE.STOCK.UNLOCK.HOLD = 1

    IF END #STOCK.SESS.NUM% THEN WRITE.STOCK.UNLOCK.HOLD.ERROR
    WRITE FORM "C4,2I2,2C3,I2,2C3,C8"; HOLD #STOCK.SESS.NUM%    \
          AUTOUNLOCK ;                                          \
                STOCK.BOOTS.CODE$,                              \
                STOCK.STOCK.FIG%,                               \
                STOCK.LAST.COUNT%,                              \
                STOCK.DATE.LAST.COUNT$,                         \
                STOCK.DATE.LAST.MOVE$,                          \
                STOCK.LAST.REC%,                                \
                STOCK.DATE.LAST.REC$,                           \
                STOCK.DATE.LAST.GAP$,                           \
                PACK$(STRING$(8 * 2,"0"))
    WRITE.STOCK.UNLOCK.HOLD = 0
    EXIT FUNCTION

    WRITE.STOCK.UNLOCK.HOLD.ERROR:

       CURRENT.CODE$ = STOCK.BOOTS.CODE$
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION
