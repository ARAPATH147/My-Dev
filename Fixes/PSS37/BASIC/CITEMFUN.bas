REM \
\******************************************************************************
\******************************************************************************
\***
\***                      CSR Item file functions
\***
\***                            CITEMFUN.BAS
\***
\***  VERSION A:   Les Cook 21st August 1992
\***
\***  VERSION B:   Steven Goulding  10/3/93
\***  CITEM.PREVIOUS.THEORETICAL.STOCK% field added
\***
\***    VERSION C.              ROBERT COWEY.                       21 OCT 1993.
\***    Corrected setting of FILE.OPERATION$ within WRITE functions.
\***
\***    VERSION D       STUART WILLIAM MCCONNACHIE     02/04/96
\***    Added fields for last delivery date and quantity.
\***
\*******************************************************************************
\*******************************************************************************

   INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                      \
      CURRENT.CODE$,                  \
      FILE.OPERATION$

   %INCLUDE CITEMDEC.J86                                               ! CRC



  FUNCTION CITEM.SET PUBLIC
\***************************

    CITEM.REPORT.NUM% = 184
    CITEM.RECL%       = 64
    CITEM.FILE.NAME$  = "CITEM"

  END FUNCTION

\******************************************************************************
\***
\***  Function to check the validity of a UPD string
\***  The function return logical true if the string contains a valid
\***  UPD value, and a logical false if it does not.
\***
\******************************************************************************

  FUNCTION VALID.UPD(UPD$) PUBLIC
\*********************************
\


      INTEGER*1 VALID.UPD,             \
                I%,                    \
                VALID,                 \
                VALID.UPD.UPD$         ! VARIABLE NAME CANNOT HAVE () IN IT!!!

      INTEGER*2 BYTE%, HIGH%, LOW%
      STRING UPD$

      VALID.UPD.UPD$ = 1

      IF UPD$ = PACK$(STRING$(LEN(UPD$),"??")) THEN BEGIN
         VALID.UPD = -1
         EXIT FUNCTION
      ENDIF

      I% = 1
      VALID = -1
      WHILE I% <= LEN(UPD$) AND VALID
         BYTE% = ASC(MID$(UPD$, I%, 1))
         HIGH% = (BYTE% AND 11110000b) / 16
         LOW% = BYTE% AND 00001111b
         IF LOW% > 9 OR HIGH% > 9 THEN VALID = 0
         I% = I% + 1
      WEND
      VALID.UPD = VALID

      VALID.UPD.UPD$ = 0

END FUNCTION

\****************************************************************************
\***
\***  Function to verify the contents of the UPD fields to prevent any
\***  packed negative values from beging written to the file.
\***
\***************************************************************************

  FUNCTION CHECK.CITEM.UPD PUBLIC
\*********************************

      INTEGER*1 CHECK.CITEM.UPD

      CHECK.CITEM.UPD = 1


      IF NOT VALID.UPD(CITEM.SHELF.ALLOCATION$) THEN                    \
             CITEM.SHELF.ALLOCATION$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.VULNERABLE.ESA$) THEN                      \
             CITEM.VULNERABLE.ESA$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.SPECIAL.ORDER.ESA$) THEN                   \
             CITEM.SPECIAL.ORDER.ESA$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.TOTAL.ESA$) THEN                           \
             CITEM.TOTAL.ESA$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.INITIAL.DISPLAY.STOCK$) THEN               \
             CITEM.INITIAL.DISPLAY.STOCK$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.ON.ORDER.IN.THIS.PDT$) THEN                \
             CITEM.ON.ORDER.IN.THIS.PDT$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.ON.ORDER.TODAY$) THEN                      \
             CITEM.ON.ORDER.TODAY$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.TOTAL.ON.ORDER$) THEN                      \
             CITEM.TOTAL.ON.ORDER$ = PACK$("0000")

      IF NOT VALID.UPD(CITEM.DATE.OF.LAST.MANUAL.COUNT$) THEN           \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$ = PACK$("000000")

      IF NOT VALID.UPD(CITEM.UNIT$) THEN                                \
             CITEM.UNIT$ = PACK$("00")

      IF NOT VALID.UPD(CITEM.WEEK.4.SALES$) THEN                        \
             CITEM.WEEK.4.SALES$ = PACK$("00000000")

      IF NOT VALID.UPD(CITEM.WEEK.3.SALES$) THEN                        \
             CITEM.WEEK.3.SALES$ = PACK$("00000000")

      IF NOT VALID.UPD(CITEM.WEEK.2.SALES$) THEN                        \
             CITEM.WEEK.2.SALES$ = PACK$("00000000")

      IF NOT VALID.UPD(CITEM.WEEK.1.SALES$) THEN                        \
             CITEM.WEEK.1.SALES$ = PACK$("00000000")

      IF NOT VALID.UPD(CITEM.YESTERDAYS.SALES$) THEN                    \
             CITEM.YESTERDAYS.SALES$ = PACK$("0000")

      CHECK.CITEM.UPD = 0

  END FUNCTION

\-----------------------------------------------------------------------------


  FUNCTION READ.CITEM PUBLIC
\****************************

    INTEGER*1 READ.CITEM

    READ.CITEM = 1

    IF END #CITEM.SESS.NUM% THEN READ.CITEM.ERROR
    READ FORM "T5,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9";              \ DSWM
         #CITEM.SESS.NUM%                                               \
         KEY CITEM.BOOTS.CODE$;                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \ BSMG
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    READ.CITEM = 0
    EXIT FUNCTION

    READ.CITEM.ERROR:

        CURRENT.CODE$       = CITEM.BOOTS.CODE$
        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

        EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION READ.CITEM.LOCK PUBLIC
\*********************************

    INTEGER*1 READ.CITEM.LOCK

    READ.CITEM.LOCK = 1

IF END #CITEM.SESS.NUM% THEN READ.CITEM.LOCK.ERROR
    READ FORM "T5,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9";              \ DSWM
         #CITEM.SESS.NUM% AUTOLOCK                                      \
         KEY CITEM.BOOTS.CODE$;                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \ BSMG
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    READ.CITEM.LOCK = 0
    EXIT FUNCTION

    READ.CITEM.LOCK.ERROR:

       CURRENT.CODE$       = CITEM.BOOTS.CODE$
       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

       EXIT FUNCTION


  END FUNCTION


\-----------------------------------------------------------------------------


  FUNCTION WRITE.CITEM PUBLIC
\*****************************

    INTEGER*1         CHECK.CITEM.RC%, WRITE.CITEM

    WRITE.CITEM = 1

    CHECK.CITEM.RC% = CHECK.CITEM.UPD

    IF CHECK.CITEM.RC% = 1 THEN GOTO WRITE.CITEM.ERROR

    IF END #CITEM.SESS.NUM% THEN WRITE.CITEM.ERROR
    WRITE FORM "C4,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9";             \ DSWM
             #CITEM.SESS.NUM%;                                          \
             CITEM.BOOTS.CODE$,                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \ BSMG
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    WRITE.CITEM = 0
    EXIT FUNCTION

    WRITE.CITEM.ERROR:

       CURRENT.CODE$       = CITEM.BOOTS.CODE$
       FILE.OPERATION$     = "W"                                       ! BRC
       CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.CITEM.HOLD PUBLIC
\**********************************

    INTEGER*1            CHECK.CITEM.RC%, WRITE.CITEM.HOLD

    WRITE.CITEM.HOLD = 1

    CHECK.CITEM.RC% = CHECK.CITEM.UPD

    IF CHECK.CITEM.RC% = 1 THEN GOTO WRITE.CITEM.HOLD.ERROR

    IF END #CITEM.SESS.NUM% THEN WRITE.CITEM.HOLD.ERROR
    WRITE FORM "C4,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9"; HOLD        \ DSWM
             #CITEM.SESS.NUM%;                                          \
             CITEM.BOOTS.CODE$,                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \ BSMG
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    WRITE.CITEM.HOLD = 0
    EXIT FUNCTION

    WRITE.CITEM.HOLD.ERROR:

       CURRENT.CODE$       = CITEM.BOOTS.CODE$
       FILE.OPERATION$     = "W"                                       ! BRC
       CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.CITEM.UNLOCK PUBLIC
\************************************

    INTEGER*1             CHECK.CITEM.RC%, WRITE.CITEM.UNLOCK

    WRITE.CITEM.UNLOCK = 1

    CHECK.CITEM.RC% = CHECK.CITEM.UPD
    IF CHECK.CITEM.RC% = 1 THEN GOTO WRITE.CITEM.UNLOCK.ERROR

    IF END #CITEM.SESS.NUM% THEN WRITE.CITEM.UNLOCK.ERROR
    WRITE FORM "C4,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9";             \ DSWM
             #CITEM.SESS.NUM% AUTOUNLOCK;                               \
             CITEM.BOOTS.CODE$,                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \ BSMG
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    WRITE.CITEM.UNLOCK = 0
    EXIT FUNCTION

    WRITE.CITEM.UNLOCK.ERROR:

       CURRENT.CODE$       = CITEM.BOOTS.CODE$
       FILE.OPERATION$     = "W"                                       ! BRC
       CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.CITEM.HOLD.UNLOCK PUBLIC
\*****************************************

   INTEGER*1             CHECK.CITEM.RC%, WRITE.CITEM.HOLD.UNLOCK

    WRITE.CITEM.HOLD.UNLOCK = 1

    CHECK.CITEM.RC% = CHECK.CITEM.UPD
    IF CHECK.CITEM.RC% = 1 THEN GOTO WRITE.CITEM.HOLD.UNLOCK.ERROR

    IF END #CITEM.SESS.NUM% THEN WRITE.CITEM.HOLD.UNLOCK.ERROR
    WRITE FORM "C4,8C2,C3,3C1,4C4,I2,C1,C2,C1,I2,C3,I2,C9"; HOLD        \ DSWM
             #CITEM.SESS.NUM% AUTOUNLOCK;                               \
             CITEM.BOOTS.CODE$,                                         \
             CITEM.SHELF.ALLOCATION$,                                   \
             CITEM.VULNERABLE.ESA$,                                     \
             CITEM.SPECIAL.ORDER.ESA$,                                  \
             CITEM.TOTAL.ESA$,                                          \
             CITEM.INITIAL.DISPLAY.STOCK$,                              \
             CITEM.ON.ORDER.IN.THIS.PDT$,                               \
             CITEM.ON.ORDER.TODAY$,                                     \
             CITEM.TOTAL.ON.ORDER$,                                     \
             CITEM.DATE.OF.LAST.MANUAL.COUNT$,                          \
             CITEM.UNIT$,                                               \
             CITEM.VULNERABLE.REPORT.FLAG$,                             \
             CITEM.SALES.SIGN.FLAG$,                                    \
             CITEM.WEEK.4.SALES$,                                       \
             CITEM.WEEK.3.SALES$,                                       \
             CITEM.WEEK.2.SALES$,                                       \
             CITEM.WEEK.1.SALES$,                                       \
             CITEM.ALTER.SALES.QUANTITY%,                               \
             CITEM.LIST.FREQUENCY$,                                     \
             CITEM.YESTERDAYS.SALES$,                                   \
             CITEM.COUNT.REQUEST.FLAG$,                                 \
             CITEM.PREVIOUS.THEORETICAL.STOCK%,                         \
             CITEM.LAST.DELIVERY.DATE$,                                 \ DSWM
             CITEM.LAST.DELIVERY.QTY%,                                  \ DSWM
             CITEM.FILLER$

    WRITE.CITEM.HOLD.UNLOCK = 0
    EXIT FUNCTION

    WRITE.CITEM.HOLD.UNLOCK.ERROR:

       CURRENT.CODE$       = CITEM.BOOTS.CODE$
       FILE.OPERATION$     = "W"                                       ! BRC
       CURRENT.REPORT.NUM% = CITEM.REPORT.NUM%

       EXIT FUNCTION

  END FUNCTION
