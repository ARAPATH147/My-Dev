REM \
\******************************************************************************
\******************************************************************************
\***
\***                ITEM DATA FILE FUNCTIONS
\***
\***                REFERENCE   : IEXFUN.BAS
\***
\***    VERSION A.       Nalini Mathusoothanan                     12 JULY 2011.
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM%

  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$

  %INCLUDE IEXDEC.J86

  FUNCTION IEX.SET PUBLIC
\*************************
     IEX.REPORT.NUM% = 828
     IEX.RECL%       = 28
     IEX.FILE.NAME$  = "IEX"
  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION READ.IEX PUBLIC
\**************************
  INTEGER*2 READ.IEX

    READ.IEX = 1

    IF END #IEX.SESS.NUM% THEN READ.ERROR
    READ FORM "T4,C4,C4,C17";                                          \
         #IEX.SESS.NUM%                                                \
         KEY IEX.ITEM.CODE$;                                           \
         IEX.ACTUAL.SUPPLIER.NUM$,                                     \
         IEX.PRIMARY.SUPPLIER$,                                        \
         IEX.FILLER$

     READ.IEX = 0
     EXIT FUNCTION

     READ.ERROR:

        FILE.OPERATION$     EQ "R"
        CURRENT.REPORT.NUM% EQ IEX.REPORT.NUM%
        CURRENT.CODE$       EQ UNPACK$(IEX.ITEM.CODE$)

        EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L

  FUNCTION READ.IEX.LOCK PUBLIC
\*******************************

    INTEGER*2 READ.IEX.LOCK

    READ.IEX.LOCK = 1

    IF END #IEX.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T4,C4,C4,C17";                                          \
         #IEX.SESS.NUM% AUTOLOCK                                       \
         KEY IEX.ITEM.CODE$;                                           \
         IEX.ACTUAL.SUPPLIER.NUM$,                                     \
         IEX.PRIMARY.SUPPLIER$,                                        \
         IEX.FILLER$

    READ.IEX.LOCK = 0
    EXIT FUNCTION


    READ.LOCK.ERROR:

       CURRENT.CODE$ = UNPACK$(IEX.ITEM.CODE$)
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = IEX.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.IEX PUBLIC
\**************************

    INTEGER*2 WRITE.IEX

    WRITE.IEX = 1

    IF END #IEX.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C3,C4,C4,C17";                                         \
             #IEX.SESS.NUM%;                                           \
             IEX.ITEM.CODE$,                                           \
             IEX.ACTUAL.SUPPLIER.NUM$,                                 \
             IEX.PRIMARY.SUPPLIER$,                                    \
             IEX.FILLER$

    WRITE.IEX = 0
    EXIT FUNCTION


    WRITE.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = IEX.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IEX.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.IEX.HOLD PUBLIC
    INTEGER*2 WRITE.IEX.HOLD

    WRITE.IEX.HOLD = 1

    IF END #IEX.SESS.NUM% THEN WRITE.HOLD.ERROR
    WRITE FORM "C3,C4,C4,C17"; HOLD                                    \
             #IEX.SESS.NUM%;                                           \
             IEX.ITEM.CODE$,                                           \
             IEX.ACTUAL.SUPPLIER.NUM$,                                 \
             IEX.PRIMARY.SUPPLIER$,                                    \
             IEX.FILLER$

    WRITE.IEX.HOLD = 0
    EXIT FUNCTION


    WRITE.HOLD.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = IEX.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IEX.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.IEX.UNLOCK PUBLIC
\**********************************

    INTEGER*2 WRITE.IEX.UNLOCK

    WRITE.IEX.UNLOCK = 1

    IF END #IEX.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C3,C4,C4,C17";                                         \
             #IEX.SESS.NUM% AUTOUNLOCK;                                \
             IEX.ITEM.CODE$,                                           \
             IEX.ACTUAL.SUPPLIER.NUM$,                                 \
             IEX.PRIMARY.SUPPLIER$,                                    \
             IEX.FILLER$

    WRITE.IEX.UNLOCK = 0
    EXIT FUNCTION


    WRITE.UNLOCK.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = IEX.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IEX.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------
