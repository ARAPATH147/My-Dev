REM \
\******************************************************************************
\******************************************************************************
\***
\***                NIADF FILE FUNCTIONS
\***
\***                REFERENCE   : NIADFFUN.BAS
\***
\***    VERSION A.       Charles Skadorwa                         20 July 2011
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM%

  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$

  %INCLUDE NIADFDEC.J86

  FUNCTION NIADF.SET PUBLIC
\*************************

     NIADF.REPORT.NUM% = 830
     NIADF.RECL%       = 6
     NIADF.FILE.NAME$  = "NIADF"

  END FUNCTION
\------------------------------------------------------------------------------


  FUNCTION READ.NIADF PUBLIC
\**************************
  INTEGER*2 READ.NIADF

    READ.NIADF = 1
    IF END #NIADF.SESS.NUM% THEN READ.ERROR
    READ FORM "T4,C3";                         \
         #NIADF.SESS.NUM%                       \
         KEY NIADF.ITEM.CODE$;                  \
         NIADF.DATE.ADDED$                      \

     READ.NIADF = 0

     EXIT FUNCTION

     READ.ERROR:
        FILE.OPERATION$     EQ "R"
        CURRENT.REPORT.NUM% EQ NIADF.REPORT.NUM%
        CURRENT.CODE$       EQ UNPACK$(NIADF.ITEM.CODE$)

        EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION READ.NIADF.LOCK PUBLIC
\*******************************

    INTEGER*2 READ.NIADF.LOCK

    READ.NIADF.LOCK = 1

    IF END #NIADF.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T4,C3";                        \
         #NIADF.SESS.NUM% AUTOLOCK             \
         KEY NIADF.ITEM.CODE$;                 \
         NIADF.DATE.ADDED$

    READ.NIADF.LOCK = 0
    EXIT FUNCTION

    READ.LOCK.ERROR:

       CURRENT.CODE$ = UNPACK$(NIADF.ITEM.CODE$)
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = NIADF.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.NIADF PUBLIC
\**************************

    INTEGER*2 WRITE.NIADF

    WRITE.NIADF = 1
    IF END #NIADF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C3,C3";             \
             #NIADF.SESS.NUM%;       \
             NIADF.ITEM.CODE$,       \
             NIADF.DATE.ADDED$

    WRITE.NIADF = 0
    EXIT FUNCTION

    WRITE.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = NIADF.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(NIADF.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.NIADF.HOLD PUBLIC
    INTEGER*2 WRITE.NIADF.HOLD

    WRITE.NIADF.HOLD = 1

    IF END #NIADF.SESS.NUM% THEN WRITE.HOLD.ERROR
    WRITE FORM "C3,C3"; HOLD        \
             #NIADF.SESS.NUM%;       \
             NIADF.ITEM.CODE$,       \
             NIADF.DATE.ADDED$

    WRITE.NIADF.HOLD = 0
    EXIT FUNCTION

    WRITE.HOLD.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = NIADF.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(NIADF.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.NIADF.UNLOCK PUBLIC
\**********************************

    INTEGER*2 WRITE.NIADF.UNLOCK

    WRITE.NIADF.UNLOCK = 1

    IF END #NIADF.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C3,C3";                    \
             #NIADF.SESS.NUM% AUTOUNLOCK;   \
             NIADF.ITEM.CODE$,              \
             NIADF.DATE.ADDED$

    WRITE.NIADF.UNLOCK = 0
    EXIT FUNCTION

    WRITE.UNLOCK.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = NIADF.REPORT.NUM%
    CURRENT.CODE$ = UNPACK$(NIADF.ITEM.CODE$)

    EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

