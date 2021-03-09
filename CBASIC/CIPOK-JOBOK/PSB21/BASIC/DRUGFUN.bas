REM \
\******************************************************************************
\******************************************************************************
\***
\***                DRUG FILE FUNCTIONS
\***
\***                REFERENCE   : DRUGFUN.BAS
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

  %INCLUDE DRUGDEC.J86

  FUNCTION DRUG.SET PUBLIC
\*************************

     DRUG.REPORT.NUM% = 829
     DRUG.RECL%       = 56
     DRUG.FILE.NAME$  = "DRUG"

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L

  FUNCTION READ.DRUG PUBLIC
\**************************
  INTEGER*2 READ.DRUG

    READ.DRUG = 1
    IF END #DRUG.SESS.NUM% THEN READ.ERROR
    READ FORM "T4,C40,I1,I4,C8";                                    \
         #DRUG.SESS.NUM%                                            \
         KEY DRUG.ITEM.CODE$;                                       \
         DRUG.DESCRIPTION$,                                         \
         DRUG.BIT.FLAGS.1%,                                         \
         DRUG.PACK.SIZE%,                                           \
         DRUG.FILLER$

     READ.DRUG = 0

     EXIT FUNCTION

     READ.ERROR:
        FILE.OPERATION$     EQ "R"
        CURRENT.REPORT.NUM% EQ DRUG.REPORT.NUM%
        CURRENT.CODE$       EQ UNPACK$(DRUG.ITEM.CODE$)

        EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION READ.DRUG.LOCK PUBLIC
\*******************************

    INTEGER*2 READ.DRUG.LOCK

    READ.DRUG.LOCK = 1

    IF END #DRUG.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T4,C40,I1,I4,C8";                                    \
         #DRUG.SESS.NUM% AUTOLOCK                                   \
         KEY DRUG.ITEM.CODE$;                                       \
         DRUG.DESCRIPTION$,                                         \
         DRUG.BIT.FLAGS.1%,                                         \
         DRUG.PACK.SIZE%,                                           \
         DRUG.FILLER$

    READ.DRUG.LOCK = 0
    EXIT FUNCTION

    READ.LOCK.ERROR:

       CURRENT.CODE$ = UNPACK$(DRUG.ITEM.CODE$)
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = DRUG.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.DRUG PUBLIC
\**************************

    INTEGER*2 WRITE.DRUG

    WRITE.DRUG = 1
    IF END #DRUG.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C3,C40,I1,I4,C8";                                   \
             #DRUG.SESS.NUM%;                                       \
             DRUG.ITEM.CODE$,                                       \
             DRUG.DESCRIPTION$,                                     \
             DRUG.BIT.FLAGS.1%,                                     \
             DRUG.PACK.SIZE%,                                       \
             DRUG.FILLER$

    WRITE.DRUG = 0
    EXIT FUNCTION

    WRITE.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = DRUG.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(DRUG.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.DRUG.HOLD PUBLIC
    INTEGER*2 WRITE.DRUG.HOLD

    WRITE.DRUG.HOLD = 1

    IF END #DRUG.SESS.NUM% THEN WRITE.HOLD.ERROR
    WRITE FORM "C3,C40,I1,I4,C8"; HOLD                              \
             #DRUG.SESS.NUM%;                                       \
             DRUG.ITEM.CODE$,                                       \
             DRUG.DESCRIPTION$,                                     \
             DRUG.BIT.FLAGS.1%,                                     \
             DRUG.PACK.SIZE%,                                       \
             DRUG.FILLER$

    WRITE.DRUG.HOLD = 0
    EXIT FUNCTION

    WRITE.HOLD.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = DRUG.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(DRUG.ITEM.CODE$)

       EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.DRUG.UNLOCK PUBLIC
\**********************************

    INTEGER*2 WRITE.DRUG.UNLOCK

    WRITE.DRUG.UNLOCK = 1

    IF END #DRUG.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C3,C40,I1,I4,C8";                                   \
             #DRUG.SESS.NUM% AUTOUNLOCK;                            \
             DRUG.ITEM.CODE$,                                       \
             DRUG.DESCRIPTION$,                                     \
             DRUG.BIT.FLAGS.1%,                                     \
             DRUG.PACK.SIZE%,                                       \
             DRUG.FILLER$

    WRITE.DRUG.UNLOCK = 0
    EXIT FUNCTION

    WRITE.UNLOCK.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = DRUG.REPORT.NUM%
    CURRENT.CODE$ = UNPACK$(DRUG.ITEM.CODE$)

    EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------

