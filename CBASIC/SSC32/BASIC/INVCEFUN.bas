REM \
\*****************************************************************************
\*****************************************************************************
\***            STORE INVOCE FILE FUNCTIONS
\***
\***               REFERENCE       : INVCEFUN.BAS
\***
\***   Version A.  Stephen Kelsey (CTG)   6th October 1992
\***
\***   VERSION B               Sumitha Moorthy                14/04/2015
\***   FOD - 431 Dallas Positive Receiving
\***   Introduced a new variable INV.RECEIPT.STATUS$ to INVCE file to
\***   store invoice positive receipt flag.
\***
\*****************************************************************************
\*****************************************************************************

   INTEGER*2 GLOBAL                 \
       CURRENT.REPORT.NUM%

   STRING GLOBAL                    \
       CURRENT.CODE$,               \
       FILE.OPERATION$

   %INCLUDE INVCEDEC.J86


  FUNCTION INVCE.SET PUBLIC
\***************************

    INVCE.REPORT.NUM%  = 82
    INVCE.RECL%      = 508
    INVCE.FILE.NAME$ = "INVCE"

    DIM INV.ITEM.DETAILS$(17,4)
    DIM INV.ITEM.QTYS%(17,2)

  END FUNCTION

\-----------------------------------------------------------------------------
REM EJECT

  FUNCTION READ.INVCE.LOCK PUBLIC
\********************************

    INTEGER*2 READ.INVCE.LOCK

    STRING         FORMAT$

    READ.INVCE.LOCK = 1

    FORMAT$ = "T10,C1,C3,C1,C3,I2,C1,C1,C1,C1,C3,C3,C1,C1,C1" +       \ !BSM
          STRING$(17,",2C4,2I2,C1,C15")
    IF END #INVCE.SESS.NUM% THEN ERROR.READ.INVCE.LOCK
    READ FORM FORMAT$; #INVCE.SESS.NUM% AUTOLOCK KEY INV.RECKEY$;     \
                    INV.FOLIO.YEAR$,                                  \
                    INV.DATE$,                                        \
                    INV.CONFIRM.FLAG$,                                \
                    INV.CONFIRM.DATE$,                                \
                    INV.WHOUSE.AREA%,                                 \
                    INV.INSYST.FLAG$,                                 \
                    INV.COUNT$,                                       \
                    INV.TYPE$,                                        \
                    INV.DALLAS.MKR$,                                  \
                    INV.EXP.DEL.DATE$,                                \
                    INV.SUPPLIER.NO$,                                 \
                    INV.ORDER.SUFFIX$,                                \
                    INV.FILLER$,                                      \
                    INV.RECEIPT.STATUS$,                              \ !BSM
                    INV.ITEM.DETAILS$(1,1),                           \
                    INV.ITEM.DETAILS$(1,2),                           \
                    INV.ITEM.QTYS%(1,1),                              \
                    INV.ITEM.QTYS%(1,2),                              \
                    INV.ITEM.DETAILS$(1,3),                           \
                    INV.ITEM.DETAILS$(1,4),                           \
                    INV.ITEM.DETAILS$(2,1),                           \
                    INV.ITEM.DETAILS$(2,2),                           \
                    INV.ITEM.QTYS%(2,1),                              \
                    INV.ITEM.QTYS%(2,2),                              \
                    INV.ITEM.DETAILS$(2,3),                           \
                    INV.ITEM.DETAILS$(2,4),                           \
                    INV.ITEM.DETAILS$(3,1),                           \
                    INV.ITEM.DETAILS$(3,2),                           \
                    INV.ITEM.QTYS%(3,1),                              \
                    INV.ITEM.QTYS%(3,2),                              \
                    INV.ITEM.DETAILS$(3,3),                           \
                    INV.ITEM.DETAILS$(3,4),                           \
                    INV.ITEM.DETAILS$(4,1),                           \
                    INV.ITEM.DETAILS$(4,2),                           \
                    INV.ITEM.QTYS%(4,1),                              \
                    INV.ITEM.QTYS%(4,2),                              \
                    INV.ITEM.DETAILS$(4,3),                           \
                    INV.ITEM.DETAILS$(4,4),                           \
                    INV.ITEM.DETAILS$(5,1),                           \
                    INV.ITEM.DETAILS$(5,2),                           \
                    INV.ITEM.QTYS%(5,1),                              \
                    INV.ITEM.QTYS%(5,2),                              \
                    INV.ITEM.DETAILS$(5,3),                           \
                    INV.ITEM.DETAILS$(5,4),                           \
                    INV.ITEM.DETAILS$(6,1),                           \
                    INV.ITEM.DETAILS$(6,2),                           \
                    INV.ITEM.QTYS%(6,1),                              \
                    INV.ITEM.QTYS%(6,2),                              \
                    INV.ITEM.DETAILS$(6,3),                           \
                    INV.ITEM.DETAILS$(6,4),                           \
                    INV.ITEM.DETAILS$(7,1),                           \
                    INV.ITEM.DETAILS$(7,2),                           \
                    INV.ITEM.QTYS%(7,1),                              \
                    INV.ITEM.QTYS%(7,2),                              \
                    INV.ITEM.DETAILS$(7,3),                           \
                    INV.ITEM.DETAILS$(7,4),                           \
                    INV.ITEM.DETAILS$(8,1),                           \
                    INV.ITEM.DETAILS$(8,2),                           \
                    INV.ITEM.QTYS%(8,1),                              \
                    INV.ITEM.QTYS%(8,2),                              \
                    INV.ITEM.DETAILS$(8,3),                           \
                    INV.ITEM.DETAILS$(8,4),                           \
                    INV.ITEM.DETAILS$(9,1),                           \
                    INV.ITEM.DETAILS$(9,2),                           \
                    INV.ITEM.QTYS%(9,1),                              \
                    INV.ITEM.QTYS%(9,2),                              \
                    INV.ITEM.DETAILS$(9,3),                           \
                    INV.ITEM.DETAILS$(9,4),                           \
                    INV.ITEM.DETAILS$(10,1),                          \
                    INV.ITEM.DETAILS$(10,2),                          \
                    INV.ITEM.QTYS%(10,1),                             \
                    INV.ITEM.QTYS%(10,2),                             \
                    INV.ITEM.DETAILS$(10,3),                          \
                    INV.ITEM.DETAILS$(10,4),                          \
                    INV.ITEM.DETAILS$(11,1),                          \
                    INV.ITEM.DETAILS$(11,2),                          \
                    INV.ITEM.QTYS%(11,1),                             \
                    INV.ITEM.QTYS%(11,2),                             \
                    INV.ITEM.DETAILS$(11,3),                          \
                    INV.ITEM.DETAILS$(11,4),                          \
                    INV.ITEM.DETAILS$(12,1),                          \
                    INV.ITEM.DETAILS$(12,2),                          \
                    INV.ITEM.QTYS%(12,1),                             \
                    INV.ITEM.QTYS%(12,2),                             \
                    INV.ITEM.DETAILS$(12,3),                          \
                    INV.ITEM.DETAILS$(12,4),                          \
                    INV.ITEM.DETAILS$(13,1),                          \
                    INV.ITEM.DETAILS$(13,2),                          \
                    INV.ITEM.QTYS%(13,1),                             \
                    INV.ITEM.QTYS%(13,2),                             \
                    INV.ITEM.DETAILS$(13,3),                          \
                    INV.ITEM.DETAILS$(13,4),                          \
                    INV.ITEM.DETAILS$(14,1),                          \
                    INV.ITEM.DETAILS$(14,2),                          \
                    INV.ITEM.QTYS%(14,1),                             \
                    INV.ITEM.QTYS%(14,2),                             \
                    INV.ITEM.DETAILS$(14,3),                          \
                    INV.ITEM.DETAILS$(14,4),                          \
                    INV.ITEM.DETAILS$(15,1),                          \
                    INV.ITEM.DETAILS$(15,2),                          \
                    INV.ITEM.QTYS%(15,1),                             \
                    INV.ITEM.QTYS%(15,2),                             \
                    INV.ITEM.DETAILS$(15,3),                          \
                    INV.ITEM.DETAILS$(15,4),                          \
                    INV.ITEM.DETAILS$(16,1),                          \
                    INV.ITEM.DETAILS$(16,2),                          \
                    INV.ITEM.QTYS%(16,1),                             \
                    INV.ITEM.QTYS%(16,2),                             \
                    INV.ITEM.DETAILS$(16,3),                          \
                    INV.ITEM.DETAILS$(16,4),                          \
                    INV.ITEM.DETAILS$(17,1),                          \
                    INV.ITEM.DETAILS$(17,2),                          \
                    INV.ITEM.QTYS%(17,1),                             \
                    INV.ITEM.QTYS%(17,2),                             \
                    INV.ITEM.DETAILS$(17,3),                          \
                    INV.ITEM.DETAILS$(17,4)

    READ.INVCE.LOCK = 0
    EXIT FUNCTION

ERROR.READ.INVCE.LOCK:

    CURRENT.CODE$ = INV.RECKEY$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = INVCE.REPORT.NUM%

    EXIT FUNCTION


END FUNCTION

\-----------------------------------------------------------------------------
REM EJECT


  FUNCTION READ.INVCE PUBLIC
\***************************

    INTEGER*2 READ.INVCE

    STRING    FORMAT$

    READ.INVCE = 1

    FORMAT$ = "T10,C1,C3,C1,C3,I2,C1,C1,C1,C1,C3,C3,C1,C1,C1" +       \ !BSM
          STRING$(17,",2C4,2I2,C1,C15")
    IF END #INVCE.SESS.NUM% THEN ERROR.READ.INVCE
    READ FORM FORMAT$; #INVCE.SESS.NUM% KEY INV.RECKEY$;              \
                    INV.FOLIO.YEAR$,                                  \
                    INV.DATE$,                                        \
                    INV.CONFIRM.FLAG$,                                \
                    INV.CONFIRM.DATE$,                                \
                    INV.WHOUSE.AREA%,                                 \
                    INV.INSYST.FLAG$,                                 \
                    INV.COUNT$,                                       \
                    INV.TYPE$,                                        \
                    INV.DALLAS.MKR$,                                  \
                    INV.EXP.DEL.DATE$,                                \
                    INV.SUPPLIER.NO$,                                 \
                    INV.ORDER.SUFFIX$,                                \
                    INV.FILLER$,                                      \
                    INV.RECEIPT.STATUS$,                              \ !BSM
                    INV.ITEM.DETAILS$(1,1),                           \
                    INV.ITEM.DETAILS$(1,2),                           \
                    INV.ITEM.QTYS%(1,1),                              \
                    INV.ITEM.QTYS%(1,2),                              \
                    INV.ITEM.DETAILS$(1,3),                           \
                    INV.ITEM.DETAILS$(1,4),                           \
                    INV.ITEM.DETAILS$(2,1),                           \
                    INV.ITEM.DETAILS$(2,2),                           \
                    INV.ITEM.QTYS%(2,1),                              \
                    INV.ITEM.QTYS%(2,2),                              \
                    INV.ITEM.DETAILS$(2,3),                           \
                    INV.ITEM.DETAILS$(2,4),                           \
                    INV.ITEM.DETAILS$(3,1),                           \
                    INV.ITEM.DETAILS$(3,2),                           \
                    INV.ITEM.QTYS%(3,1),                              \
                    INV.ITEM.QTYS%(3,2),                              \
                    INV.ITEM.DETAILS$(3,3),                           \
                    INV.ITEM.DETAILS$(3,4),                           \
                    INV.ITEM.DETAILS$(4,1),                           \
                    INV.ITEM.DETAILS$(4,2),                           \
                    INV.ITEM.QTYS%(4,1),                              \
                    INV.ITEM.QTYS%(4,2),                              \
                    INV.ITEM.DETAILS$(4,3),                           \
                    INV.ITEM.DETAILS$(4,4),                           \
                    INV.ITEM.DETAILS$(5,1),                           \
                    INV.ITEM.DETAILS$(5,2),                           \
                    INV.ITEM.QTYS%(5,1),                              \
                    INV.ITEM.QTYS%(5,2),                              \
                    INV.ITEM.DETAILS$(5,3),                           \
                    INV.ITEM.DETAILS$(5,4),                           \
                    INV.ITEM.DETAILS$(6,1),                           \
                    INV.ITEM.DETAILS$(6,2),                           \
                    INV.ITEM.QTYS%(6,1),                              \
                    INV.ITEM.QTYS%(6,2),                              \
                    INV.ITEM.DETAILS$(6,3),                           \
                    INV.ITEM.DETAILS$(6,4),                           \
                    INV.ITEM.DETAILS$(7,1),                           \
                    INV.ITEM.DETAILS$(7,2),                           \
                    INV.ITEM.QTYS%(7,1),                              \
                    INV.ITEM.QTYS%(7,2),                              \
                    INV.ITEM.DETAILS$(7,3),                           \
                    INV.ITEM.DETAILS$(7,4),                           \
                    INV.ITEM.DETAILS$(8,1),                           \
                    INV.ITEM.DETAILS$(8,2),                           \
                    INV.ITEM.QTYS%(8,1),                              \
                    INV.ITEM.QTYS%(8,2),                              \
                    INV.ITEM.DETAILS$(8,3),                           \
                    INV.ITEM.DETAILS$(8,4),                           \
                    INV.ITEM.DETAILS$(9,1),                           \
                    INV.ITEM.DETAILS$(9,2),                           \
                    INV.ITEM.QTYS%(9,1),                              \
                    INV.ITEM.QTYS%(9,2),                              \
                    INV.ITEM.DETAILS$(9,3),                           \
                    INV.ITEM.DETAILS$(9,4),                           \
                    INV.ITEM.DETAILS$(10,1),                          \
                    INV.ITEM.DETAILS$(10,2),                          \
                    INV.ITEM.QTYS%(10,1),                             \
                    INV.ITEM.QTYS%(10,2),                             \
                    INV.ITEM.DETAILS$(10,3),                          \
                    INV.ITEM.DETAILS$(10,4),                          \
                    INV.ITEM.DETAILS$(11,1),                          \
                    INV.ITEM.DETAILS$(11,2),                          \
                    INV.ITEM.QTYS%(11,1),                             \
                    INV.ITEM.QTYS%(11,2),                             \
                    INV.ITEM.DETAILS$(11,3),                          \
                    INV.ITEM.DETAILS$(11,4),                          \
                    INV.ITEM.DETAILS$(12,1),                          \
                    INV.ITEM.DETAILS$(12,2),                          \
                    INV.ITEM.QTYS%(12,1),                             \
                    INV.ITEM.QTYS%(12,2),                             \
                    INV.ITEM.DETAILS$(12,3),                          \
                    INV.ITEM.DETAILS$(12,4),                          \
                    INV.ITEM.DETAILS$(13,1),                          \
                    INV.ITEM.DETAILS$(13,2),                          \
                    INV.ITEM.QTYS%(13,1),                             \
                    INV.ITEM.QTYS%(13,2),                             \
                    INV.ITEM.DETAILS$(13,3),                          \
                    INV.ITEM.DETAILS$(13,4),                          \
                    INV.ITEM.DETAILS$(14,1),                          \
                    INV.ITEM.DETAILS$(14,2),                          \
                    INV.ITEM.QTYS%(14,1),                             \
                    INV.ITEM.QTYS%(14,2),                             \
                    INV.ITEM.DETAILS$(14,3),                          \
                    INV.ITEM.DETAILS$(14,4),                          \
                    INV.ITEM.DETAILS$(15,1),                          \
                    INV.ITEM.DETAILS$(15,2),                          \
                    INV.ITEM.QTYS%(15,1),                             \
                    INV.ITEM.QTYS%(15,2),                             \
                    INV.ITEM.DETAILS$(15,3),                          \
                    INV.ITEM.DETAILS$(15,4),                          \
                    INV.ITEM.DETAILS$(16,1),                          \
                    INV.ITEM.DETAILS$(16,2),                          \
                    INV.ITEM.QTYS%(16,1),                             \
                    INV.ITEM.QTYS%(16,2),                             \
                    INV.ITEM.DETAILS$(16,3),                          \
                    INV.ITEM.DETAILS$(16,4),                          \
                    INV.ITEM.DETAILS$(17,1),                          \
                    INV.ITEM.DETAILS$(17,2),                          \
                    INV.ITEM.QTYS%(17,1),                             \
                    INV.ITEM.QTYS%(17,2),                             \
                    INV.ITEM.DETAILS$(17,3),                          \
                    INV.ITEM.DETAILS$(17,4)

    READ.INVCE = 0

    EXIT FUNCTION

ERROR.READ.INVCE:

    CURRENT.CODE$ = INV.RECKEY$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = INVCE.REPORT.NUM%

    EXIT FUNCTION


END FUNCTION

\-----------------------------------------------------------------------------
REM EJECT

  FUNCTION WRITE.INVCE.UNLOCK PUBLIC
\***********************************


    INTEGER*2 WRITE.INVCE.UNLOCK

    STRING    FORMAT$

    WRITE.INVCE.UNLOCK = 1

    FORMAT$ = "C9,C1,C3,C1,C3,I2,C1,C1,C1,C1,C3,C3,C1,C1,C1" +        \ !BSM
          STRING$(17,",2C4,2I2,C1,C15")
    IF END #INVCE.SESS.NUM% THEN ERROR.WRITE.INVCE.UNLOCK
    WRITE FORM FORMAT$; #INVCE.SESS.NUM% AUTOUNLOCK;                  \
                    INV.RECKEY$,                                      \
                    INV.FOLIO.YEAR$,                                  \
                    INV.DATE$,                                        \
                    INV.CONFIRM.FLAG$,                                \
                    INV.CONFIRM.DATE$,                                \
                    INV.WHOUSE.AREA%,                                 \
                    INV.INSYST.FLAG$,                                 \
                    INV.COUNT$,                                       \
                    INV.TYPE$,                                        \
                    INV.DALLAS.MKR$,                                  \
                    INV.EXP.DEL.DATE$,                                \
                    INV.SUPPLIER.NO$,                                 \
                    INV.ORDER.SUFFIX$,                                \
                    INV.FILLER$,                                      \
                    INV.RECEIPT.STATUS$,                              \ !BSM
                    INV.ITEM.DETAILS$(1,1),                           \
                    INV.ITEM.DETAILS$(1,2),                           \
                    INV.ITEM.QTYS%(1,1),                              \
                    INV.ITEM.QTYS%(1,2),                              \
                    INV.ITEM.DETAILS$(1,3),                           \
                    INV.ITEM.DETAILS$(1,4),                           \
                    INV.ITEM.DETAILS$(2,1),                           \
                    INV.ITEM.DETAILS$(2,2),                           \
                    INV.ITEM.QTYS%(2,1),                              \
                    INV.ITEM.QTYS%(2,2),                              \
                    INV.ITEM.DETAILS$(2,3),                           \
                    INV.ITEM.DETAILS$(2,4),                           \
                    INV.ITEM.DETAILS$(3,1),                           \
                    INV.ITEM.DETAILS$(3,2),                           \
                    INV.ITEM.QTYS%(3,1),                              \
                    INV.ITEM.QTYS%(3,2),                              \
                    INV.ITEM.DETAILS$(3,3),                           \
                    INV.ITEM.DETAILS$(3,4),                           \
                    INV.ITEM.DETAILS$(4,1),                           \
                    INV.ITEM.DETAILS$(4,2),                           \
                    INV.ITEM.QTYS%(4,1),                              \
                    INV.ITEM.QTYS%(4,2),                              \
                    INV.ITEM.DETAILS$(4,3),                           \
                    INV.ITEM.DETAILS$(4,4),                           \
                    INV.ITEM.DETAILS$(5,1),                           \
                    INV.ITEM.DETAILS$(5,2),                           \
                    INV.ITEM.QTYS%(5,1),                              \
                    INV.ITEM.QTYS%(5,2),                              \
                    INV.ITEM.DETAILS$(5,3),                           \
                    INV.ITEM.DETAILS$(5,4),                           \
                    INV.ITEM.DETAILS$(6,1),                           \
                    INV.ITEM.DETAILS$(6,2),                           \
                    INV.ITEM.QTYS%(6,1),                              \
                    INV.ITEM.QTYS%(6,2),                              \
                    INV.ITEM.DETAILS$(6,3),                           \
                    INV.ITEM.DETAILS$(6,4),                           \
                    INV.ITEM.DETAILS$(7,1),                           \
                    INV.ITEM.DETAILS$(7,2),                           \
                    INV.ITEM.QTYS%(7,1),                              \
                    INV.ITEM.QTYS%(7,2),                              \
                    INV.ITEM.DETAILS$(7,3),                           \
                    INV.ITEM.DETAILS$(7,4),                           \
                    INV.ITEM.DETAILS$(8,1),                           \
                    INV.ITEM.DETAILS$(8,2),                           \
                    INV.ITEM.QTYS%(8,1),                              \
                    INV.ITEM.QTYS%(8,2),                              \
                    INV.ITEM.DETAILS$(8,3),                           \
                    INV.ITEM.DETAILS$(8,4),                           \
                    INV.ITEM.DETAILS$(9,1),                           \
                    INV.ITEM.DETAILS$(9,2),                           \
                    INV.ITEM.QTYS%(9,1),                              \
                    INV.ITEM.QTYS%(9,2),                              \
                    INV.ITEM.DETAILS$(9,3),                           \
                    INV.ITEM.DETAILS$(9,4),                           \
                    INV.ITEM.DETAILS$(10,1),                          \
                    INV.ITEM.DETAILS$(10,2),                          \
                    INV.ITEM.QTYS%(10,1),                             \
                    INV.ITEM.QTYS%(10,2),                             \
                    INV.ITEM.DETAILS$(10,3),                          \
                    INV.ITEM.DETAILS$(10,4),                          \
                    INV.ITEM.DETAILS$(11,1),                          \
                    INV.ITEM.DETAILS$(11,2),                          \
                    INV.ITEM.QTYS%(11,1),                             \
                    INV.ITEM.QTYS%(11,2),                             \
                    INV.ITEM.DETAILS$(11,3),                          \
                    INV.ITEM.DETAILS$(11,4),                          \
                    INV.ITEM.DETAILS$(12,1),                          \
                    INV.ITEM.DETAILS$(12,2),                          \
                    INV.ITEM.QTYS%(12,1),                             \
                    INV.ITEM.QTYS%(12,2),                             \
                    INV.ITEM.DETAILS$(12,3),                          \
                    INV.ITEM.DETAILS$(12,4),                          \
                    INV.ITEM.DETAILS$(13,1),                          \
                    INV.ITEM.DETAILS$(13,2),                          \
                    INV.ITEM.QTYS%(13,1),                             \
                    INV.ITEM.QTYS%(13,2),                             \
                    INV.ITEM.DETAILS$(13,3),                          \
                    INV.ITEM.DETAILS$(13,4),                          \
                    INV.ITEM.DETAILS$(14,1),                          \
                    INV.ITEM.DETAILS$(14,2),                          \
                    INV.ITEM.QTYS%(14,1),                             \
                    INV.ITEM.QTYS%(14,2),                             \
                    INV.ITEM.DETAILS$(14,3),                          \
                    INV.ITEM.DETAILS$(14,4),                          \
                    INV.ITEM.DETAILS$(15,1),                          \
                    INV.ITEM.DETAILS$(15,2),                          \
                    INV.ITEM.QTYS%(15,1),                             \
                    INV.ITEM.QTYS%(15,2),                             \
                    INV.ITEM.DETAILS$(15,3),                          \
                    INV.ITEM.DETAILS$(15,4),                          \
                    INV.ITEM.DETAILS$(16,1),                          \
                    INV.ITEM.DETAILS$(16,2),                          \
                    INV.ITEM.QTYS%(16,1),                             \
                    INV.ITEM.QTYS%(16,2),                             \
                    INV.ITEM.DETAILS$(16,3),                          \
                    INV.ITEM.DETAILS$(16,4),                          \
                    INV.ITEM.DETAILS$(17,1),                          \
                    INV.ITEM.DETAILS$(17,2),                          \
                    INV.ITEM.QTYS%(17,1),                             \
                    INV.ITEM.QTYS%(17,2),                             \
                    INV.ITEM.DETAILS$(17,3),                          \
                    INV.ITEM.DETAILS$(17,4)

    WRITE.INVCE.UNLOCK = 0
    EXIT FUNCTION

ERROR.WRITE.INVCE.UNLOCK:

    CURRENT.CODE$ = INV.RECKEY$
    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = INVCE.REPORT.NUM%

    EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
REM EJECT

  FUNCTION WRITE.INVCE PUBLIC
\****************************

    INTEGER*2 WRITE.INVCE

    STRING    FORMAT$

    WRITE.INVCE = 1

    FORMAT$ = "C9,C1,C3,C1,C3,I2,C1,C1,C1,C1,C3,C3,C1,C1,C1" +        \ !BSM
          STRING$(17,",2C4,2I2,C1,C15")
    IF END #INVCE.SESS.NUM% THEN ERROR.WRITE.INVCE
    WRITE FORM FORMAT$; #INVCE.SESS.NUM%;                             \
                    INV.RECKEY$,                                      \
                    INV.FOLIO.YEAR$,                                  \
                    INV.DATE$,                                        \
                    INV.CONFIRM.FLAG$,                                \
                    INV.CONFIRM.DATE$,                                \
                    INV.WHOUSE.AREA%,                                 \
                    INV.INSYST.FLAG$,                                 \
                    INV.COUNT$,                                       \
                    INV.TYPE$,                                        \
                    INV.DALLAS.MKR$,                                  \
                    INV.EXP.DEL.DATE$,                                \
                    INV.SUPPLIER.NO$,                                 \
                    INV.ORDER.SUFFIX$,                                \
                    INV.FILLER$,                                      \
                    INV.RECEIPT.STATUS$,                              \ !BSM
                    INV.ITEM.DETAILS$(1,1),                           \
                    INV.ITEM.DETAILS$(1,2),                           \
                    INV.ITEM.QTYS%(1,1),                              \
                    INV.ITEM.QTYS%(1,2),                              \
                    INV.ITEM.DETAILS$(1,3),                           \
                    INV.ITEM.DETAILS$(1,4),                           \
                    INV.ITEM.DETAILS$(2,1),                           \
                    INV.ITEM.DETAILS$(2,2),                           \
                    INV.ITEM.QTYS%(2,1),                              \
                    INV.ITEM.QTYS%(2,2),                              \
                    INV.ITEM.DETAILS$(2,3),                           \
                    INV.ITEM.DETAILS$(2,4),                           \
                    INV.ITEM.DETAILS$(3,1),                           \
                    INV.ITEM.DETAILS$(3,2),                           \
                    INV.ITEM.QTYS%(3,1),                              \
                    INV.ITEM.QTYS%(3,2),                              \
                    INV.ITEM.DETAILS$(3,3),                           \
                    INV.ITEM.DETAILS$(3,4),                           \
                    INV.ITEM.DETAILS$(4,1),                           \
                    INV.ITEM.DETAILS$(4,2),                           \
                    INV.ITEM.QTYS%(4,1),                              \
                    INV.ITEM.QTYS%(4,2),                              \
                    INV.ITEM.DETAILS$(4,3),                           \
                    INV.ITEM.DETAILS$(4,4),                           \
                    INV.ITEM.DETAILS$(5,1),                           \
                    INV.ITEM.DETAILS$(5,2),                           \
                    INV.ITEM.QTYS%(5,1),                              \
                    INV.ITEM.QTYS%(5,2),                              \
                    INV.ITEM.DETAILS$(5,3),                           \
                    INV.ITEM.DETAILS$(5,4),                           \
                    INV.ITEM.DETAILS$(6,1),                           \
                    INV.ITEM.DETAILS$(6,2),                           \
                    INV.ITEM.QTYS%(6,1),                              \
                    INV.ITEM.QTYS%(6,2),                              \
                    INV.ITEM.DETAILS$(6,3),                           \
                    INV.ITEM.DETAILS$(6,4),                           \
                    INV.ITEM.DETAILS$(7,1),                           \
                    INV.ITEM.DETAILS$(7,2),                           \
                    INV.ITEM.QTYS%(7,1),                              \
                    INV.ITEM.QTYS%(7,2),                              \
                    INV.ITEM.DETAILS$(7,3),                           \
                    INV.ITEM.DETAILS$(7,4),                           \
                    INV.ITEM.DETAILS$(8,1),                           \
                    INV.ITEM.DETAILS$(8,2),                           \
                    INV.ITEM.QTYS%(8,1),                              \
                    INV.ITEM.QTYS%(8,2),                              \
                    INV.ITEM.DETAILS$(8,3),                           \
                    INV.ITEM.DETAILS$(8,4),                           \
                    INV.ITEM.DETAILS$(9,1),                           \
                    INV.ITEM.DETAILS$(9,2),                           \
                    INV.ITEM.QTYS%(9,1),                              \
                    INV.ITEM.QTYS%(9,2),                              \
                    INV.ITEM.DETAILS$(9,3),                           \
                    INV.ITEM.DETAILS$(9,4),                           \
                    INV.ITEM.DETAILS$(10,1),                          \
                    INV.ITEM.DETAILS$(10,2),                          \
                    INV.ITEM.QTYS%(10,1),                             \
                    INV.ITEM.QTYS%(10,2),                             \
                    INV.ITEM.DETAILS$(10,3),                          \
                    INV.ITEM.DETAILS$(10,4),                          \
                    INV.ITEM.DETAILS$(11,1),                          \
                    INV.ITEM.DETAILS$(11,2),                          \
                    INV.ITEM.QTYS%(11,1),                             \
                    INV.ITEM.QTYS%(11,2),                             \
                    INV.ITEM.DETAILS$(11,3),                          \
                    INV.ITEM.DETAILS$(11,4),                          \
                    INV.ITEM.DETAILS$(12,1),                          \
                    INV.ITEM.DETAILS$(12,2),                          \
                    INV.ITEM.QTYS%(12,1),                             \
                    INV.ITEM.QTYS%(12,2),                             \
                    INV.ITEM.DETAILS$(12,3),                          \
                    INV.ITEM.DETAILS$(12,4),                          \
                    INV.ITEM.DETAILS$(13,1),                          \
                    INV.ITEM.DETAILS$(13,2),                          \
                    INV.ITEM.QTYS%(13,1),                             \
                    INV.ITEM.QTYS%(13,2),                             \
                    INV.ITEM.DETAILS$(13,3),                          \
                    INV.ITEM.DETAILS$(13,4),                          \
                    INV.ITEM.DETAILS$(14,1),                          \
                    INV.ITEM.DETAILS$(14,2),                          \
                    INV.ITEM.QTYS%(14,1),                             \
                    INV.ITEM.QTYS%(14,2),                             \
                    INV.ITEM.DETAILS$(14,3),                          \
                    INV.ITEM.DETAILS$(14,4),                          \
                    INV.ITEM.DETAILS$(15,1),                          \
                    INV.ITEM.DETAILS$(15,2),                          \
                    INV.ITEM.QTYS%(15,1),                             \
                    INV.ITEM.QTYS%(15,2),                             \
                    INV.ITEM.DETAILS$(15,3),                          \
                    INV.ITEM.DETAILS$(15,4),                          \
                    INV.ITEM.DETAILS$(16,1),                          \
                    INV.ITEM.DETAILS$(16,2),                          \
                    INV.ITEM.QTYS%(16,1),                             \
                    INV.ITEM.QTYS%(16,2),                             \
                    INV.ITEM.DETAILS$(16,3),                          \
                    INV.ITEM.DETAILS$(16,4),                          \
                    INV.ITEM.DETAILS$(17,1),                          \
                    INV.ITEM.DETAILS$(17,2),                          \
                    INV.ITEM.QTYS%(17,1),                             \
                    INV.ITEM.QTYS%(17,2),                             \
                    INV.ITEM.DETAILS$(17,3),                          \
                    INV.ITEM.DETAILS$(17,4)

    WRITE.INVCE = 0
    EXIT FUNCTION

ERROR.WRITE.INVCE:

    CURRENT.CODE$ = INV.RECKEY$
    CURRENT.REPORT.NUM% = INVCE.REPORT.NUM%
    FILE.OPERATION$ = "W"

    EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------



