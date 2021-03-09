REM \
\*****************************************************************************
\*****************************************************************************
\***            STORE DIRECT ORDERS FILE FUNCTIONS
\***
\***               REFERENCE       : DIRORFUN.BAS
\***
\***  VERSION B                 Neil Bennett                21st December 2006
\***  New variable added to hold superceded flag for ASN Carton Support.
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL                 \
       CURRENT.REPORT.NUM%

  STRING GLOBAL                    \
          CURRENT.CODE$,           \
          FILE.OPERATION$

   %INCLUDE DIRORDEC.J86


  FUNCTION DIRORD.SET PUBLIC
\***************************

    DIRORD.REPORT.NUM%  = 229
    DIRORD.RECL%      = 508
    DIRORD.FILE.NAME$ = "DIROR"
    DIRORD.NO.RECS%   = 2000

    DIM DIRORD.ITEM.DETAILS$(16,4)
    DIM DIRORD.ITEM.QTY%(16,7)

  END FUNCTION

\-----------------------------------------------------------------------------
REM EJECT

  FUNCTION READ.DIRORD.LOCK PUBLIC
\**********************************

     INTEGER*2 READ.DIRORD.LOCK

     STRING         FORMAT$

     READ.DIRORD.LOCK = 1

\***
\*** Read Order header record
\***
     IF MID$(DIRORD.RECKEY$,9,1) = PACK$("00") THEN BEGIN
\       FORMAT$ = "T10,4C2,2C3,C1,C3,2C2,C3,C474"
        FORMAT$ = "T10,4C2,2C3,C1,C3,2C2,C3,C1,C473"                     ! BNWB
        IF END #DIRORD.SESS.NUM% THEN READ.LOCK.ERROR
        READ FORM FORMAT$; #DIRORD.SESS.NUM% AUTOLOCK                    \
            KEY DIRORD.RECKEY$;                                          \
                DIRORD.NO.ORDER.ITEM$,                                   \
                DIRORD.NO.ORDER.SNGL$,                                   \
                DIRORD.NO.ITEMS.BOOKED$,                                 \
                DIRORD.NO.ITEMS.LST.BKD$,                                \
                DIRORD.ORDER.DATE$,                                      \
                DIRORD.EXP.DELV.DATE$,                                   \
                DIRORD.CONFIRM.FLAG$,                                    \
                DIRORD.CONFIRM.DATE$,                                    \
                DIRORD.CONF.STRT.TIME$,                                  \
                DIRORD.CONF.END.TIME$,                                   \
                DIRORD.ON.SALE.DATE$,                                    \
                DIRORD.SUPERCEDED$,                                      \ BNWB
                DIRORD.FILLER2$
     ENDIF                                                               \
     ELSE                                                                \
\***
\*** Read Order Detail record
\***
     IF DIRORD.RECKEY$ <> PACK$(STRING$(9,"99")) THEN BEGIN
        FORMAT$ = "T10,C1,C3,C15" + STRING$(16,",C4,C6,C4,7I2,C2")
        IF END #DIRORD.SESS.NUM% THEN READ.LOCK.ERROR
        READ FORM FORMAT$; #DIRORD.SESS.NUM% AUTOLOCK                    \
                KEY DIRORD.RECKEY$;                                      \
                    DIRORD.ITEM.COUNT$,                                  \
                    DIRORD.CONFIRM.DATE$,                                \
                    DIRORD.FILLER3$,                                     \
                    DIRORD.ITEM.DETAILS$(1,1),                           \
                    DIRORD.ITEM.DETAILS$(1,2),                           \
                    DIRORD.ITEM.DETAILS$(1,3),                           \
                    DIRORD.ITEM.QTY%(1,1),                               \
                    DIRORD.ITEM.QTY%(1,2),                               \
                    DIRORD.ITEM.QTY%(1,3),                               \
                    DIRORD.ITEM.QTY%(1,4),                               \
                    DIRORD.ITEM.QTY%(1,5),                               \
                    DIRORD.ITEM.QTY%(1,6),                               \
                    DIRORD.ITEM.QTY%(1,7),                               \
                    DIRORD.ITEM.DETAILS$(1,4),                           \
                    DIRORD.ITEM.DETAILS$(2,1),                           \
                    DIRORD.ITEM.DETAILS$(2,2),                           \
                    DIRORD.ITEM.DETAILS$(2,3),                           \
                    DIRORD.ITEM.QTY%(2,1),                               \
                    DIRORD.ITEM.QTY%(2,2),                               \
                    DIRORD.ITEM.QTY%(2,3),                               \
                    DIRORD.ITEM.QTY%(2,4),                               \
                    DIRORD.ITEM.QTY%(2,5),                               \
                    DIRORD.ITEM.QTY%(2,6),                               \
                    DIRORD.ITEM.QTY%(2,7),                               \
                    DIRORD.ITEM.DETAILS$(2,4),                           \
                    DIRORD.ITEM.DETAILS$(3,1),                           \
                    DIRORD.ITEM.DETAILS$(3,2),                           \
                    DIRORD.ITEM.DETAILS$(3,3),                           \
                    DIRORD.ITEM.QTY%(3,1),                               \
                    DIRORD.ITEM.QTY%(3,2),                               \
                    DIRORD.ITEM.QTY%(3,3),                               \
                    DIRORD.ITEM.QTY%(3,4),                               \
                    DIRORD.ITEM.QTY%(3,5),                               \
                    DIRORD.ITEM.QTY%(3,6),                               \
                    DIRORD.ITEM.QTY%(3,7),                               \
                    DIRORD.ITEM.DETAILS$(3,4),                           \
                    DIRORD.ITEM.DETAILS$(4,1),                           \
                    DIRORD.ITEM.DETAILS$(4,2),                           \
                    DIRORD.ITEM.DETAILS$(4,3),                           \
                    DIRORD.ITEM.QTY%(4,1),                               \
                    DIRORD.ITEM.QTY%(4,2),                               \
                    DIRORD.ITEM.QTY%(4,3),                               \
                    DIRORD.ITEM.QTY%(4,4),                               \
                    DIRORD.ITEM.QTY%(4,5),                               \
                    DIRORD.ITEM.QTY%(4,6),                               \
                    DIRORD.ITEM.QTY%(4,7),                               \
                    DIRORD.ITEM.DETAILS$(4,4),                           \
                    DIRORD.ITEM.DETAILS$(5,1),                           \
                    DIRORD.ITEM.DETAILS$(5,2),                           \
                    DIRORD.ITEM.DETAILS$(5,3),                           \
                    DIRORD.ITEM.QTY%(5,1),                               \
                    DIRORD.ITEM.QTY%(5,2),                               \
                    DIRORD.ITEM.QTY%(5,3),                               \
                    DIRORD.ITEM.QTY%(5,4),                               \
                    DIRORD.ITEM.QTY%(5,5),                               \
                    DIRORD.ITEM.QTY%(5,6),                               \
                    DIRORD.ITEM.QTY%(5,7),                               \
                    DIRORD.ITEM.DETAILS$(5,4),                           \
                    DIRORD.ITEM.DETAILS$(6,1),                           \
                    DIRORD.ITEM.DETAILS$(6,2),                           \
                    DIRORD.ITEM.DETAILS$(6,3),                           \
                    DIRORD.ITEM.QTY%(6,1),                               \
                    DIRORD.ITEM.QTY%(6,2),                               \
                    DIRORD.ITEM.QTY%(6,3),                               \
                    DIRORD.ITEM.QTY%(6,4),                               \
                    DIRORD.ITEM.QTY%(6,5),                               \
                    DIRORD.ITEM.QTY%(6,6),                               \
                    DIRORD.ITEM.QTY%(6,7),                               \
                    DIRORD.ITEM.DETAILS$(6,4),                           \
                    DIRORD.ITEM.DETAILS$(7,1),                           \
                    DIRORD.ITEM.DETAILS$(7,2),                           \
                    DIRORD.ITEM.DETAILS$(7,3),                           \
                    DIRORD.ITEM.QTY%(7,1),                               \
                    DIRORD.ITEM.QTY%(7,2),                               \
                    DIRORD.ITEM.QTY%(7,3),                               \
                    DIRORD.ITEM.QTY%(7,4),                               \
                    DIRORD.ITEM.QTY%(7,5),                               \
                    DIRORD.ITEM.QTY%(7,6),                               \
                    DIRORD.ITEM.QTY%(7,7),                               \
                    DIRORD.ITEM.DETAILS$(7,4),                           \
                    DIRORD.ITEM.DETAILS$(8,1),                           \
                    DIRORD.ITEM.DETAILS$(8,2),                           \
                    DIRORD.ITEM.DETAILS$(8,3),                           \
                    DIRORD.ITEM.QTY%(8,1),                               \
                    DIRORD.ITEM.QTY%(8,2),                               \
                    DIRORD.ITEM.QTY%(8,3),                               \
                    DIRORD.ITEM.QTY%(8,4),                               \
                    DIRORD.ITEM.QTY%(8,5),                               \
                    DIRORD.ITEM.QTY%(8,6),                               \
                    DIRORD.ITEM.QTY%(8,7),                               \
                    DIRORD.ITEM.DETAILS$(8,4),                           \
                    DIRORD.ITEM.DETAILS$(9,1),                           \
                    DIRORD.ITEM.DETAILS$(9,2),                           \
                    DIRORD.ITEM.DETAILS$(9,3),                           \
                    DIRORD.ITEM.QTY%(9,1),                               \
                    DIRORD.ITEM.QTY%(9,2),                               \
                    DIRORD.ITEM.QTY%(9,3),                               \
                    DIRORD.ITEM.QTY%(9,4),                               \
                    DIRORD.ITEM.QTY%(9,5),                               \
                    DIRORD.ITEM.QTY%(9,6),                               \
                    DIRORD.ITEM.QTY%(9,7),                               \
                    DIRORD.ITEM.DETAILS$(9,4),                           \
                    DIRORD.ITEM.DETAILS$(10,1),                          \
                    DIRORD.ITEM.DETAILS$(10,2),                          \
                    DIRORD.ITEM.DETAILS$(10,3),                          \
                    DIRORD.ITEM.QTY%(10,1),                              \
                    DIRORD.ITEM.QTY%(10,2),                              \
                    DIRORD.ITEM.QTY%(10,3),                              \
                    DIRORD.ITEM.QTY%(10,4),                              \
                    DIRORD.ITEM.QTY%(10,5),                              \
                    DIRORD.ITEM.QTY%(10,6),                              \
                    DIRORD.ITEM.QTY%(10,7),                              \
                    DIRORD.ITEM.DETAILS$(10,4),                          \
                    DIRORD.ITEM.DETAILS$(11,1),                          \
                    DIRORD.ITEM.DETAILS$(11,2),                          \
                    DIRORD.ITEM.DETAILS$(11,3),                          \
                    DIRORD.ITEM.QTY%(11,1),                              \
                    DIRORD.ITEM.QTY%(11,2),                              \
                    DIRORD.ITEM.QTY%(11,3),                              \
                    DIRORD.ITEM.QTY%(11,4),                              \
                    DIRORD.ITEM.QTY%(11,5),                              \
                    DIRORD.ITEM.QTY%(11,6),                              \
                    DIRORD.ITEM.QTY%(11,7),                              \
                    DIRORD.ITEM.DETAILS$(11,4),                          \
                    DIRORD.ITEM.DETAILS$(12,1),                          \
                    DIRORD.ITEM.DETAILS$(12,2),                          \
                    DIRORD.ITEM.DETAILS$(12,3),                          \
                    DIRORD.ITEM.QTY%(12,1),                              \
                    DIRORD.ITEM.QTY%(12,2),                              \
                    DIRORD.ITEM.QTY%(12,3),                              \
                    DIRORD.ITEM.QTY%(12,4),                              \
                    DIRORD.ITEM.QTY%(12,5),                              \
                    DIRORD.ITEM.QTY%(12,6),                              \
                    DIRORD.ITEM.QTY%(12,7),                              \
                    DIRORD.ITEM.DETAILS$(12,4),                          \
                    DIRORD.ITEM.DETAILS$(13,1),                          \
                    DIRORD.ITEM.DETAILS$(13,2),                          \
                    DIRORD.ITEM.DETAILS$(13,3),                          \
                    DIRORD.ITEM.QTY%(13,1),                              \
                    DIRORD.ITEM.QTY%(13,2),                              \
                    DIRORD.ITEM.QTY%(13,3),                              \
                    DIRORD.ITEM.QTY%(13,4),                              \
                    DIRORD.ITEM.QTY%(13,5),                              \
                    DIRORD.ITEM.QTY%(13,6),                              \
                    DIRORD.ITEM.QTY%(13,7),                              \
                    DIRORD.ITEM.DETAILS$(13,4),                          \
                    DIRORD.ITEM.DETAILS$(14,1),                          \
                    DIRORD.ITEM.DETAILS$(14,2),                          \
                    DIRORD.ITEM.DETAILS$(14,3),                          \
                    DIRORD.ITEM.QTY%(14,1),                              \
                    DIRORD.ITEM.QTY%(14,2),                              \
                    DIRORD.ITEM.QTY%(14,3),                              \
                    DIRORD.ITEM.QTY%(14,4),                              \
                    DIRORD.ITEM.QTY%(14,5),                              \
                    DIRORD.ITEM.QTY%(14,6),                              \
                    DIRORD.ITEM.QTY%(14,7),                              \
                    DIRORD.ITEM.DETAILS$(14,4),                          \
                    DIRORD.ITEM.DETAILS$(15,1),                          \
                    DIRORD.ITEM.DETAILS$(15,2),                          \
                    DIRORD.ITEM.DETAILS$(15,3),                          \
                    DIRORD.ITEM.QTY%(15,1),                              \
                    DIRORD.ITEM.QTY%(15,2),                              \
                    DIRORD.ITEM.QTY%(15,3),                              \
                    DIRORD.ITEM.QTY%(15,4),                              \
                    DIRORD.ITEM.QTY%(15,5),                              \
                    DIRORD.ITEM.QTY%(15,6),                              \
                    DIRORD.ITEM.QTY%(15,7),                              \
                    DIRORD.ITEM.DETAILS$(15,4),                          \
                    DIRORD.ITEM.DETAILS$(16,1),                          \
                    DIRORD.ITEM.DETAILS$(16,2),                          \
                    DIRORD.ITEM.DETAILS$(16,3),                          \
                    DIRORD.ITEM.QTY%(16,1),                              \
                    DIRORD.ITEM.QTY%(16,2),                              \
                    DIRORD.ITEM.QTY%(16,3),                              \
                    DIRORD.ITEM.QTY%(16,4),                              \
                    DIRORD.ITEM.QTY%(16,5),                              \
                    DIRORD.ITEM.QTY%(16,6),                              \
                    DIRORD.ITEM.QTY%(16,7),                              \
                    DIRORD.ITEM.DETAILS$(16,4)
      ENDIF                                                              \
      ELSE BEGIN
 \***
 \*** Read DIRORD header record
 \***
              FORMAT$ = "T10,C3,C2,C494"
              IF END #DIRORD.SESS.NUM% THEN READ.LOCK.ERROR
              READ FORM FORMAT$; #DIRORD.SESS.NUM% AUTOLOCK              \
                KEY DIRORD.RECKEY$;                                      \
                    DIRORD.DRRF.LAST.DATE$,                              \
                    DIRORD.DRRF.LAST.TIME$,                              \
                    DIRORD.FILLER1$
              ENDIF

     READ.DIRORD.LOCK = 0
     EXIT FUNCTION

     READ.LOCK.ERROR:

        CURRENT.CODE$ = DIRORD.RECKEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = DIRORD.REPORT.NUM%


        EXIT FUNCTION


  END FUNCTION

\------------------------------------------------------------------------
REM EJECT


  FUNCTION READ.DIRORD PUBLIC
\*****************************

     INTEGER*2 READ.DIRORD

     STRING         FORMAT$

     READ.DIRORD = 1

\***
\*** Read Order header record
\***
     IF MID$(DIRORD.RECKEY$,9,1) = PACK$("00") THEN BEGIN
\       FORMAT$ = "T10,4C2,2C3,C1,C3,2C2,C3,C474"
        FORMAT$ = "T10,4C2,2C3,C1,C3,2C2,C3,C1,C473"                     ! BNWB
        IF END #DIRORD.SESS.NUM% THEN READ.ERROR
        READ FORM FORMAT$; #DIRORD.SESS.NUM%                             \
            KEY DIRORD.RECKEY$;                                          \
                DIRORD.NO.ORDER.ITEM$,                                   \
                DIRORD.NO.ORDER.SNGL$,                                   \
                DIRORD.NO.ITEMS.BOOKED$,                                 \
                DIRORD.NO.ITEMS.LST.BKD$,                                \
                DIRORD.ORDER.DATE$,                                      \
                DIRORD.EXP.DELV.DATE$,                                   \
                DIRORD.CONFIRM.FLAG$,                                    \
                DIRORD.CONFIRM.DATE$,                                    \
                DIRORD.CONF.STRT.TIME$,                                  \
                DIRORD.CONF.END.TIME$,                                   \
                DIRORD.ON.SALE.DATE$,                                    \
                DIRORD.SUPERCEDED$,                                      \ BNWB
                DIRORD.FILLER2$
     ENDIF                                                               \
     ELSE                                                                \
\***
\*** Read Order Detail record
\***
     IF DIRORD.RECKEY$ <> PACK$(STRING$(9,"99")) THEN BEGIN
        FORMAT$ = "T10,C1,C3,C15" + STRING$(16,",C4,C6,C4,7I2,C2")
        IF END #DIRORD.SESS.NUM% THEN READ.ERROR
        READ FORM FORMAT$; #DIRORD.SESS.NUM%                             \
                KEY DIRORD.RECKEY$;                                      \
                    DIRORD.ITEM.COUNT$,                                  \
                    DIRORD.CONFIRM.DATE$,                                \
                    DIRORD.FILLER3$,                                     \
                    DIRORD.ITEM.DETAILS$(1,1),                           \
                    DIRORD.ITEM.DETAILS$(1,2),                           \
                    DIRORD.ITEM.DETAILS$(1,3),                           \
                    DIRORD.ITEM.QTY%(1,1),                               \
                    DIRORD.ITEM.QTY%(1,2),                               \
                    DIRORD.ITEM.QTY%(1,3),                               \
                    DIRORD.ITEM.QTY%(1,4),                               \
                    DIRORD.ITEM.QTY%(1,5),                               \
                    DIRORD.ITEM.QTY%(1,6),                               \
                    DIRORD.ITEM.QTY%(1,7),                               \
                    DIRORD.ITEM.DETAILS$(1,4),                           \
                    DIRORD.ITEM.DETAILS$(2,1),                           \
                    DIRORD.ITEM.DETAILS$(2,2),                           \
                    DIRORD.ITEM.DETAILS$(2,3),                           \
                    DIRORD.ITEM.QTY%(2,1),                               \
                    DIRORD.ITEM.QTY%(2,2),                               \
                    DIRORD.ITEM.QTY%(2,3),                               \
                    DIRORD.ITEM.QTY%(2,4),                               \
                    DIRORD.ITEM.QTY%(2,5),                               \
                    DIRORD.ITEM.QTY%(2,6),                               \
                    DIRORD.ITEM.QTY%(2,7),                               \
                    DIRORD.ITEM.DETAILS$(2,4),                           \
                    DIRORD.ITEM.DETAILS$(3,1),                           \
                    DIRORD.ITEM.DETAILS$(3,2),                           \
                    DIRORD.ITEM.DETAILS$(3,3),                           \
                    DIRORD.ITEM.QTY%(3,1),                               \
                    DIRORD.ITEM.QTY%(3,2),                               \
                    DIRORD.ITEM.QTY%(3,3),                               \
                    DIRORD.ITEM.QTY%(3,4),                               \
                    DIRORD.ITEM.QTY%(3,5),                               \
                    DIRORD.ITEM.QTY%(3,6),                               \
                    DIRORD.ITEM.QTY%(3,7),                               \
                    DIRORD.ITEM.DETAILS$(3,4),                           \
                    DIRORD.ITEM.DETAILS$(4,1),                           \
                    DIRORD.ITEM.DETAILS$(4,2),                           \
                    DIRORD.ITEM.DETAILS$(4,3),                           \
                    DIRORD.ITEM.QTY%(4,1),                               \
                    DIRORD.ITEM.QTY%(4,2),                               \
                    DIRORD.ITEM.QTY%(4,3),                               \
                    DIRORD.ITEM.QTY%(4,4),                               \
                    DIRORD.ITEM.QTY%(4,5),                               \
                    DIRORD.ITEM.QTY%(4,6),                               \
                    DIRORD.ITEM.QTY%(4,7),                               \
                    DIRORD.ITEM.DETAILS$(4,4),                           \
                    DIRORD.ITEM.DETAILS$(5,1),                           \
                    DIRORD.ITEM.DETAILS$(5,2),                           \
                    DIRORD.ITEM.DETAILS$(5,3),                           \
                    DIRORD.ITEM.QTY%(5,1),                               \
                    DIRORD.ITEM.QTY%(5,2),                               \
                    DIRORD.ITEM.QTY%(5,3),                               \
                    DIRORD.ITEM.QTY%(5,4),                               \
                    DIRORD.ITEM.QTY%(5,5),                               \
                    DIRORD.ITEM.QTY%(5,6),                               \
                    DIRORD.ITEM.QTY%(5,7),                               \
                    DIRORD.ITEM.DETAILS$(5,4),                           \
                    DIRORD.ITEM.DETAILS$(6,1),                           \
                    DIRORD.ITEM.DETAILS$(6,2),                           \
                    DIRORD.ITEM.DETAILS$(6,3),                           \
                    DIRORD.ITEM.QTY%(6,1),                               \
                    DIRORD.ITEM.QTY%(6,2),                               \
                    DIRORD.ITEM.QTY%(6,3),                               \
                    DIRORD.ITEM.QTY%(6,4),                               \
                    DIRORD.ITEM.QTY%(6,5),                               \
                    DIRORD.ITEM.QTY%(6,6),                               \
                    DIRORD.ITEM.QTY%(6,7),                               \
                    DIRORD.ITEM.DETAILS$(6,4),                           \
                    DIRORD.ITEM.DETAILS$(7,1),                           \
                    DIRORD.ITEM.DETAILS$(7,2),                           \
                    DIRORD.ITEM.DETAILS$(7,3),                           \
                    DIRORD.ITEM.QTY%(7,1),                               \
                    DIRORD.ITEM.QTY%(7,2),                               \
                    DIRORD.ITEM.QTY%(7,3),                               \
                    DIRORD.ITEM.QTY%(7,4),                               \
                    DIRORD.ITEM.QTY%(7,5),                               \
                    DIRORD.ITEM.QTY%(7,6),                               \
                    DIRORD.ITEM.QTY%(7,7),                               \
                    DIRORD.ITEM.DETAILS$(7,4),                           \
                    DIRORD.ITEM.DETAILS$(8,1),                           \
                    DIRORD.ITEM.DETAILS$(8,2),                           \
                    DIRORD.ITEM.DETAILS$(8,3),                           \
                    DIRORD.ITEM.QTY%(8,1),                               \
                    DIRORD.ITEM.QTY%(8,2),                               \
                    DIRORD.ITEM.QTY%(8,3),                               \
                    DIRORD.ITEM.QTY%(8,4),                               \
                    DIRORD.ITEM.QTY%(8,5),                               \
                    DIRORD.ITEM.QTY%(8,6),                               \
                    DIRORD.ITEM.QTY%(8,7),                               \
                    DIRORD.ITEM.DETAILS$(8,4),                           \
                    DIRORD.ITEM.DETAILS$(9,1),                           \
                    DIRORD.ITEM.DETAILS$(9,2),                           \
                    DIRORD.ITEM.DETAILS$(9,3),                           \
                    DIRORD.ITEM.QTY%(9,1),                               \
                    DIRORD.ITEM.QTY%(9,2),                               \
                    DIRORD.ITEM.QTY%(9,3),                               \
                    DIRORD.ITEM.QTY%(9,4),                               \
                    DIRORD.ITEM.QTY%(9,5),                               \
                    DIRORD.ITEM.QTY%(9,6),                               \
                    DIRORD.ITEM.QTY%(9,7),                               \
                    DIRORD.ITEM.DETAILS$(9,4),                           \
                    DIRORD.ITEM.DETAILS$(10,1),                          \
                    DIRORD.ITEM.DETAILS$(10,2),                          \
                    DIRORD.ITEM.DETAILS$(10,3),                          \
                    DIRORD.ITEM.QTY%(10,1),                              \
                    DIRORD.ITEM.QTY%(10,2),                              \
                    DIRORD.ITEM.QTY%(10,3),                              \
                    DIRORD.ITEM.QTY%(10,4),                              \
                    DIRORD.ITEM.QTY%(10,5),                              \
                    DIRORD.ITEM.QTY%(10,6),                              \
                    DIRORD.ITEM.QTY%(10,7),                              \
                    DIRORD.ITEM.DETAILS$(10,4),                          \
                    DIRORD.ITEM.DETAILS$(11,1),                          \
                    DIRORD.ITEM.DETAILS$(11,2),                          \
                    DIRORD.ITEM.DETAILS$(11,3),                          \
                    DIRORD.ITEM.QTY%(11,1),                              \
                    DIRORD.ITEM.QTY%(11,2),                              \
                    DIRORD.ITEM.QTY%(11,3),                              \
                    DIRORD.ITEM.QTY%(11,4),                              \
                    DIRORD.ITEM.QTY%(11,5),                              \
                    DIRORD.ITEM.QTY%(11,6),                              \
                    DIRORD.ITEM.QTY%(11,7),                              \
                    DIRORD.ITEM.DETAILS$(11,4),                          \
                    DIRORD.ITEM.DETAILS$(12,1),                          \
                    DIRORD.ITEM.DETAILS$(12,2),                          \
                    DIRORD.ITEM.DETAILS$(12,3),                          \
                    DIRORD.ITEM.QTY%(12,1),                              \
                    DIRORD.ITEM.QTY%(12,2),                              \
                    DIRORD.ITEM.QTY%(12,3),                              \
                    DIRORD.ITEM.QTY%(12,4),                              \
                    DIRORD.ITEM.QTY%(12,5),                              \
                    DIRORD.ITEM.QTY%(12,6),                              \
                    DIRORD.ITEM.QTY%(12,7),                              \
                    DIRORD.ITEM.DETAILS$(12,4),                          \
                    DIRORD.ITEM.DETAILS$(13,1),                          \
                    DIRORD.ITEM.DETAILS$(13,2),                          \
                    DIRORD.ITEM.DETAILS$(13,3),                          \
                    DIRORD.ITEM.QTY%(13,1),                              \
                    DIRORD.ITEM.QTY%(13,2),                              \
                    DIRORD.ITEM.QTY%(13,3),                              \
                    DIRORD.ITEM.QTY%(13,4),                              \
                    DIRORD.ITEM.QTY%(13,5),                              \
                    DIRORD.ITEM.QTY%(13,6),                              \
                    DIRORD.ITEM.QTY%(13,7),                              \
                    DIRORD.ITEM.DETAILS$(13,4),                          \
                    DIRORD.ITEM.DETAILS$(14,1),                          \
                    DIRORD.ITEM.DETAILS$(14,2),                          \
                    DIRORD.ITEM.DETAILS$(14,3),                          \
                    DIRORD.ITEM.QTY%(14,1),                              \
                    DIRORD.ITEM.QTY%(14,2),                              \
                    DIRORD.ITEM.QTY%(14,3),                              \
                    DIRORD.ITEM.QTY%(14,4),                              \
                    DIRORD.ITEM.QTY%(14,5),                              \
                    DIRORD.ITEM.QTY%(14,6),                              \
                    DIRORD.ITEM.QTY%(14,7),                              \
                    DIRORD.ITEM.DETAILS$(14,4),                          \
                    DIRORD.ITEM.DETAILS$(15,1),                          \
                    DIRORD.ITEM.DETAILS$(15,2),                          \
                    DIRORD.ITEM.DETAILS$(15,3),                          \
                    DIRORD.ITEM.QTY%(15,1),                              \
                    DIRORD.ITEM.QTY%(15,2),                              \
                    DIRORD.ITEM.QTY%(15,3),                              \
                    DIRORD.ITEM.QTY%(15,4),                              \
                    DIRORD.ITEM.QTY%(15,5),                              \
                    DIRORD.ITEM.QTY%(15,6),                              \
                    DIRORD.ITEM.QTY%(15,7),                              \
                    DIRORD.ITEM.DETAILS$(15,4),                          \
                    DIRORD.ITEM.DETAILS$(16,1),                          \
                    DIRORD.ITEM.DETAILS$(16,2),                          \
                    DIRORD.ITEM.DETAILS$(16,3),                          \
                    DIRORD.ITEM.QTY%(16,1),                              \
                    DIRORD.ITEM.QTY%(16,2),                              \
                    DIRORD.ITEM.QTY%(16,3),                              \
                    DIRORD.ITEM.QTY%(16,4),                              \
                    DIRORD.ITEM.QTY%(16,5),                              \
                    DIRORD.ITEM.QTY%(16,6),                              \
                    DIRORD.ITEM.QTY%(16,7),                              \
                    DIRORD.ITEM.DETAILS$(16,4)
      ENDIF                                                              \
      ELSE BEGIN
 \***
 \*** Read DIRORD header record
 \***
              FORMAT$ = "T10,C3,C2,C494"
              IF END #DIRORD.SESS.NUM% THEN READ.ERROR
              READ FORM FORMAT$; #DIRORD.SESS.NUM%                       \
                KEY DIRORD.RECKEY$;                                      \
                    DIRORD.DRRF.LAST.DATE$,                              \
                    DIRORD.DRRF.LAST.TIME$,                              \
                    DIRORD.FILLER1$
              ENDIF

     READ.DIRORD = 0
     EXIT FUNCTION

     READ.ERROR:

        CURRENT.CODE$ = DIRORD.RECKEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = DIRORD.REPORT.NUM%


        EXIT FUNCTION


  END FUNCTION

\------------------------------------------------------------------------
REM EJECT

  FUNCTION WRITE.DIRORD.UNLOCK PUBLIC
\************************************


  INTEGER*2 WRITE.DIRORD.UNLOCK

  STRING         FORMAT$

  WRITE.DIRORD.UNLOCK = 1

\***
\*** Write Order header record
\***
    IF DIRORD.PAGE.NO$ = PACK$("00") THEN BEGIN
\      FORMAT$ = "C3,C2,4C1,4C2,2C3,C1,C3,2C2,C3,C474"
       FORMAT$ = "C3,C2,4C1,4C2,2C3,C1,C3,2C2,C3,C1,C473"                ! BNWB
       IF END #DIRORD.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM FORMAT$; #DIRORD.SESS.NUM% AUTOUNLOCK;                 \
                DIRORD.SUPPLIER$,                                        \
                DIRORD.ORDER.NO$,                                        \
                DIRORD.ORDER.SUF$,                                       \
                DIRORD.BUS.CENTRE$,                                      \
                DIRORD.SOURCE$,                                          \
                DIRORD.PAGE.NO$,                                         \
                DIRORD.NO.ORDER.ITEM$,                                   \
                DIRORD.NO.ORDER.SNGL$,                                   \
                DIRORD.NO.ITEMS.BOOKED$,                                 \
                DIRORD.NO.ITEMS.LST.BKD$,                                \
                DIRORD.ORDER.DATE$,                                      \
                DIRORD.EXP.DELV.DATE$,                                   \
                DIRORD.CONFIRM.FLAG$,                                    \
                DIRORD.CONFIRM.DATE$,                                    \
                DIRORD.CONF.STRT.TIME$,                                  \
                DIRORD.CONF.END.TIME$,                                   \
                DIRORD.ON.SALE.DATE$,                                    \
                DIRORD.SUPERCEDED$,                                      \ BNWB
                DIRORD.FILLER2$
        ENDIF                                                            \
     ELSE                                                                \
\***
\*** Write Order Detail record
\***
        IF DIRORD.SUPPLIER$ <> PACK$("999999") OR                        \
           DIRORD.ORDER.NO$ <> PACK$("9999") OR                          \
           DIRORD.ORDER.SUF$ <> PACK$("99") OR                           \
           DIRORD.BUS.CENTRE$ <> PACK$("99") OR                          \
           DIRORD.SOURCE$ <> PACK$("99") OR                              \
           DIRORD.PAGE.NO$ <> PACK$("99") THEN BEGIN
           FORMAT$ = "C3,C2,4C1,C1,C3,C15" +                             \
                      STRING$(16,",C4,C6,C4,7I2,C2")
           IF END #DIRORD.SESS.NUM% THEN WRITE.UNLOCK.ERROR
           WRITE FORM FORMAT$; #DIRORD.SESS.NUM% AUTOUNLOCK;             \
                    DIRORD.SUPPLIER$,                                    \
                    DIRORD.ORDER.NO$,                                    \
                    DIRORD.ORDER.SUF$,                                   \
                    DIRORD.BUS.CENTRE$,                                  \
                    DIRORD.SOURCE$,                                      \
                    DIRORD.PAGE.NO$,                                     \
                    DIRORD.ITEM.COUNT$,                                  \
                    DIRORD.CONFIRM.DATE$,                                \
                    DIRORD.FILLER3$,                                     \
                    DIRORD.ITEM.DETAILS$(1,1),                           \
                    DIRORD.ITEM.DETAILS$(1,2),                           \
                    DIRORD.ITEM.DETAILS$(1,3),                           \
                    DIRORD.ITEM.QTY%(1,1),                               \
                    DIRORD.ITEM.QTY%(1,2),                               \
                    DIRORD.ITEM.QTY%(1,3),                               \
                    DIRORD.ITEM.QTY%(1,4),                               \
                    DIRORD.ITEM.QTY%(1,5),                               \
                    DIRORD.ITEM.QTY%(1,6),                               \
                    DIRORD.ITEM.QTY%(1,7),                               \
                    DIRORD.ITEM.DETAILS$(1,4),                           \
                    DIRORD.ITEM.DETAILS$(2,1),                           \
                    DIRORD.ITEM.DETAILS$(2,2),                           \
                    DIRORD.ITEM.DETAILS$(2,3),                           \
                    DIRORD.ITEM.QTY%(2,1),                               \
                    DIRORD.ITEM.QTY%(2,2),                               \
                    DIRORD.ITEM.QTY%(2,3),                               \
                    DIRORD.ITEM.QTY%(2,4),                               \
                    DIRORD.ITEM.QTY%(2,5),                               \
                    DIRORD.ITEM.QTY%(2,6),                               \
                    DIRORD.ITEM.QTY%(2,7),                               \
                    DIRORD.ITEM.DETAILS$(2,4),                           \
                    DIRORD.ITEM.DETAILS$(3,1),                           \
                    DIRORD.ITEM.DETAILS$(3,2),                           \
                    DIRORD.ITEM.DETAILS$(3,3),                           \
                    DIRORD.ITEM.QTY%(3,1),                               \
                    DIRORD.ITEM.QTY%(3,2),                               \
                    DIRORD.ITEM.QTY%(3,3),                               \
                    DIRORD.ITEM.QTY%(3,4),                               \
                    DIRORD.ITEM.QTY%(3,5),                               \
                    DIRORD.ITEM.QTY%(3,6),                               \
                    DIRORD.ITEM.QTY%(3,7),                               \
                    DIRORD.ITEM.DETAILS$(3,4),                           \
                    DIRORD.ITEM.DETAILS$(4,1),                           \
                    DIRORD.ITEM.DETAILS$(4,2),                           \
                    DIRORD.ITEM.DETAILS$(4,3),                           \
                    DIRORD.ITEM.QTY%(4,1),                               \
                    DIRORD.ITEM.QTY%(4,2),                               \
                    DIRORD.ITEM.QTY%(4,3),                               \
                    DIRORD.ITEM.QTY%(4,4),                               \
                    DIRORD.ITEM.QTY%(4,5),                               \
                    DIRORD.ITEM.QTY%(4,6),                               \
                    DIRORD.ITEM.QTY%(4,7),                               \
                    DIRORD.ITEM.DETAILS$(4,4),                           \
                    DIRORD.ITEM.DETAILS$(5,1),                           \
                    DIRORD.ITEM.DETAILS$(5,2),                           \
                    DIRORD.ITEM.DETAILS$(5,3),                           \
                    DIRORD.ITEM.QTY%(5,1),                               \
                    DIRORD.ITEM.QTY%(5,2),                               \
                    DIRORD.ITEM.QTY%(5,3),                               \
                    DIRORD.ITEM.QTY%(5,4),                               \
                    DIRORD.ITEM.QTY%(5,5),                               \
                    DIRORD.ITEM.QTY%(5,6),                               \
                    DIRORD.ITEM.QTY%(5,7),                               \
                    DIRORD.ITEM.DETAILS$(5,4),                           \
                    DIRORD.ITEM.DETAILS$(6,1),                           \
                    DIRORD.ITEM.DETAILS$(6,2),                           \
                    DIRORD.ITEM.DETAILS$(6,3),                           \
                    DIRORD.ITEM.QTY%(6,1),                               \
                    DIRORD.ITEM.QTY%(6,2),                               \
                    DIRORD.ITEM.QTY%(6,3),                               \
                    DIRORD.ITEM.QTY%(6,4),                               \
                    DIRORD.ITEM.QTY%(6,5),                               \
                    DIRORD.ITEM.QTY%(6,6),                               \
                    DIRORD.ITEM.QTY%(6,7),                               \
                    DIRORD.ITEM.DETAILS$(6,4),                           \
                    DIRORD.ITEM.DETAILS$(7,1),                           \
                    DIRORD.ITEM.DETAILS$(7,2),                           \
                    DIRORD.ITEM.DETAILS$(7,3),                           \
                    DIRORD.ITEM.QTY%(7,1),                               \
                    DIRORD.ITEM.QTY%(7,2),                               \
                    DIRORD.ITEM.QTY%(7,3),                               \
                    DIRORD.ITEM.QTY%(7,4),                               \
                    DIRORD.ITEM.QTY%(7,5),                               \
                    DIRORD.ITEM.QTY%(7,6),                               \
                    DIRORD.ITEM.QTY%(7,7),                               \
                    DIRORD.ITEM.DETAILS$(7,4),                           \
                    DIRORD.ITEM.DETAILS$(8,1),                           \
                    DIRORD.ITEM.DETAILS$(8,2),                           \
                    DIRORD.ITEM.DETAILS$(8,3),                           \
                    DIRORD.ITEM.QTY%(8,1),                               \
                    DIRORD.ITEM.QTY%(8,2),                               \
                    DIRORD.ITEM.QTY%(8,3),                               \
                    DIRORD.ITEM.QTY%(8,4),                               \
                    DIRORD.ITEM.QTY%(8,5),                               \
                    DIRORD.ITEM.QTY%(8,6),                               \
                    DIRORD.ITEM.QTY%(8,7),                               \
                    DIRORD.ITEM.DETAILS$(8,4),                           \
                    DIRORD.ITEM.DETAILS$(9,1),                           \
                    DIRORD.ITEM.DETAILS$(9,2),                           \
                    DIRORD.ITEM.DETAILS$(9,3),                           \
                    DIRORD.ITEM.QTY%(9,1),                               \
                    DIRORD.ITEM.QTY%(9,2),                               \
                    DIRORD.ITEM.QTY%(9,3),                               \
                    DIRORD.ITEM.QTY%(9,4),                               \
                    DIRORD.ITEM.QTY%(9,5),                               \
                    DIRORD.ITEM.QTY%(9,6),                               \
                    DIRORD.ITEM.QTY%(9,7),                               \
                    DIRORD.ITEM.DETAILS$(9,4),                           \
                    DIRORD.ITEM.DETAILS$(10,1),                          \
                    DIRORD.ITEM.DETAILS$(10,2),                          \
                    DIRORD.ITEM.DETAILS$(10,3),                          \
                    DIRORD.ITEM.QTY%(10,1),                              \
                    DIRORD.ITEM.QTY%(10,2),                              \
                    DIRORD.ITEM.QTY%(10,3),                              \
                    DIRORD.ITEM.QTY%(10,4),                              \
                    DIRORD.ITEM.QTY%(10,5),                              \
                    DIRORD.ITEM.QTY%(10,6),                              \
                    DIRORD.ITEM.QTY%(10,7),                              \
                    DIRORD.ITEM.DETAILS$(10,4),                          \
                    DIRORD.ITEM.DETAILS$(11,1),                          \
                    DIRORD.ITEM.DETAILS$(11,2),                          \
                    DIRORD.ITEM.DETAILS$(11,3),                          \
                    DIRORD.ITEM.QTY%(11,1),                              \
                    DIRORD.ITEM.QTY%(11,2),                              \
                    DIRORD.ITEM.QTY%(11,3),                              \
                    DIRORD.ITEM.QTY%(11,4),                              \
                    DIRORD.ITEM.QTY%(11,5),                              \
                    DIRORD.ITEM.QTY%(11,6),                              \
                    DIRORD.ITEM.QTY%(11,7),                              \
                    DIRORD.ITEM.DETAILS$(11,4),                          \
                    DIRORD.ITEM.DETAILS$(12,1),                          \
                    DIRORD.ITEM.DETAILS$(12,2),                          \
                    DIRORD.ITEM.DETAILS$(12,3),                          \
                    DIRORD.ITEM.QTY%(12,1),                              \
                    DIRORD.ITEM.QTY%(12,2),                              \
                    DIRORD.ITEM.QTY%(12,3),                              \
                    DIRORD.ITEM.QTY%(12,4),                              \
                    DIRORD.ITEM.QTY%(12,5),                              \
                    DIRORD.ITEM.QTY%(12,6),                              \
                    DIRORD.ITEM.QTY%(12,7),                              \
                    DIRORD.ITEM.DETAILS$(12,4),                          \
                    DIRORD.ITEM.DETAILS$(13,1),                          \
                    DIRORD.ITEM.DETAILS$(13,2),                          \
                    DIRORD.ITEM.DETAILS$(13,3),                          \
                    DIRORD.ITEM.QTY%(13,1),                              \
                    DIRORD.ITEM.QTY%(13,2),                              \
                    DIRORD.ITEM.QTY%(13,3),                              \
                    DIRORD.ITEM.QTY%(13,4),                              \
                    DIRORD.ITEM.QTY%(13,5),                              \
                    DIRORD.ITEM.QTY%(13,6),                              \
                    DIRORD.ITEM.QTY%(13,7),                              \
                    DIRORD.ITEM.DETAILS$(13,4),                          \
                    DIRORD.ITEM.DETAILS$(14,1),                          \
                    DIRORD.ITEM.DETAILS$(14,2),                          \
                    DIRORD.ITEM.DETAILS$(14,3),                          \
                    DIRORD.ITEM.QTY%(14,1),                              \
                    DIRORD.ITEM.QTY%(14,2),                              \
                    DIRORD.ITEM.QTY%(14,3),                              \
                    DIRORD.ITEM.QTY%(14,4),                              \
                    DIRORD.ITEM.QTY%(14,5),                              \
                    DIRORD.ITEM.QTY%(14,6),                              \
                    DIRORD.ITEM.QTY%(14,7),                              \
                    DIRORD.ITEM.DETAILS$(14,4),                          \
                    DIRORD.ITEM.DETAILS$(15,1),                          \
                    DIRORD.ITEM.DETAILS$(15,2),                          \
                    DIRORD.ITEM.DETAILS$(15,3),                          \
                    DIRORD.ITEM.QTY%(15,1),                              \
                    DIRORD.ITEM.QTY%(15,2),                              \
                    DIRORD.ITEM.QTY%(15,3),                              \
                    DIRORD.ITEM.QTY%(15,4),                              \
                    DIRORD.ITEM.QTY%(15,5),                              \
                    DIRORD.ITEM.QTY%(15,6),                              \
                    DIRORD.ITEM.QTY%(15,7),                              \
                    DIRORD.ITEM.DETAILS$(15,4),                          \
                    DIRORD.ITEM.DETAILS$(16,1),                          \
                    DIRORD.ITEM.DETAILS$(16,2),                          \
                    DIRORD.ITEM.DETAILS$(16,3),                          \
                    DIRORD.ITEM.QTY%(16,1),                              \
                    DIRORD.ITEM.QTY%(16,2),                              \
                    DIRORD.ITEM.QTY%(16,3),                              \
                    DIRORD.ITEM.QTY%(16,4),                              \
                    DIRORD.ITEM.QTY%(16,5),                              \
                    DIRORD.ITEM.QTY%(16,6),                              \
                    DIRORD.ITEM.QTY%(16,7),                              \
                    DIRORD.ITEM.DETAILS$(16,4)
            ENDIF                                                        \
        ELSE BEGIN
 \***
 \*** Write DIRORD header record
 \***
          FORMAT$ = "C3,C2,4C1,C3,C2,C494"
          IF END #DIRORD.SESS.NUM% THEN WRITE.UNLOCK.ERROR
          WRITE FORM FORMAT$; #DIRORD.SESS.NUM% AUTOUNLOCK;              \
                    DIRORD.SUPPLIER$,                                    \
                    DIRORD.ORDER.NO$,                                    \
                    DIRORD.ORDER.SUF$,                                   \
                    DIRORD.BUS.CENTRE$,                                  \
                    DIRORD.SOURCE$,                                      \
                    DIRORD.PAGE.NO$,                                     \
                    DIRORD.DRRF.LAST.DATE$,                              \
                    DIRORD.DRRF.LAST.TIME$,                              \
                    DIRORD.FILLER1$
           ENDIF

  WRITE.DIRORD.UNLOCK = 0
  EXIT FUNCTION

  WRITE.UNLOCK.ERROR:

     CURRENT.CODE$ = DIRORD.SUPPLIER$
     FILE.OPERATION$ = "O"
     CURRENT.REPORT.NUM% = DIRORD.REPORT.NUM%

     EXIT FUNCTION

  END FUNCTION

\------------------------------------------------------------------------
REM EJECT

  FUNCTION WRITE.DIRORD PUBLIC
\*****************************

  INTEGER*2 WRITE.DIRORD

  STRING         FORMAT$

  WRITE.DIRORD = 1

\***
\*** Write Order header record
\***
    IF DIRORD.PAGE.NO$ = PACK$("00") THEN BEGIN
\      FORMAT$ = "C3,C2,4C1,4C2,2C3,C1,C3,2C2,C3,C474"
       FORMAT$ = "C3,C2,4C1,4C2,2C3,C1,C3,2C2,C3,C1,C473"                ! BNWB
       IF END #DIRORD.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; #DIRORD.SESS.NUM%;                            \
                DIRORD.SUPPLIER$,                                        \
                DIRORD.ORDER.NO$,                                        \
                DIRORD.ORDER.SUF$,                                       \
                DIRORD.BUS.CENTRE$,                                      \
                DIRORD.SOURCE$,                                          \
                DIRORD.PAGE.NO$,                                         \
                DIRORD.NO.ORDER.ITEM$,                                   \
                DIRORD.NO.ORDER.SNGL$,                                   \
                DIRORD.NO.ITEMS.BOOKED$,                                 \
                DIRORD.NO.ITEMS.LST.BKD$,                                \
                DIRORD.ORDER.DATE$,                                      \
                DIRORD.EXP.DELV.DATE$,                                   \
                DIRORD.CONFIRM.FLAG$,                                    \
                DIRORD.CONFIRM.DATE$,                                    \
                DIRORD.CONF.STRT.TIME$,                                  \
                DIRORD.CONF.END.TIME$,                                   \
                DIRORD.ON.SALE.DATE$,                                    \
                DIRORD.SUPERCEDED$,                                      \ BNWB
                DIRORD.FILLER2$
        ENDIF                                                            \
     ELSE                                                                \
\***
\*** Write Order Detail record
\***
        IF DIRORD.SUPPLIER$ <> PACK$("999999") OR                        \
           DIRORD.ORDER.NO$ <> PACK$("9999") OR                          \
           DIRORD.ORDER.SUF$ <> PACK$("99") OR                           \
           DIRORD.BUS.CENTRE$ <> PACK$("99") OR                          \
           DIRORD.SOURCE$ <> PACK$("99") OR                              \
           DIRORD.PAGE.NO$ <> PACK$("99") THEN BEGIN
           FORMAT$ = "C3,C2,4C1,C1,C3,C15" +                             \
                      STRING$(16,",C4,C6,C4,7I2,C2")
           IF END #DIRORD.SESS.NUM% THEN WRITE.ERROR
           WRITE FORM FORMAT$; #DIRORD.SESS.NUM%;                        \
                    DIRORD.SUPPLIER$,                                    \
                    DIRORD.ORDER.NO$,                                    \
                    DIRORD.ORDER.SUF$,                                   \
                    DIRORD.BUS.CENTRE$,                                  \
                    DIRORD.SOURCE$,                                      \
                    DIRORD.PAGE.NO$,                                     \
                    DIRORD.ITEM.COUNT$,                                  \
                    DIRORD.CONFIRM.DATE$,                                \
                    DIRORD.FILLER3$,                                     \
                    DIRORD.ITEM.DETAILS$(1,1),                           \
                    DIRORD.ITEM.DETAILS$(1,2),                           \
                    DIRORD.ITEM.DETAILS$(1,3),                           \
                    DIRORD.ITEM.QTY%(1,1),                               \
                    DIRORD.ITEM.QTY%(1,2),                               \
                    DIRORD.ITEM.QTY%(1,3),                               \
                    DIRORD.ITEM.QTY%(1,4),                               \
                    DIRORD.ITEM.QTY%(1,5),                               \
                    DIRORD.ITEM.QTY%(1,6),                               \
                    DIRORD.ITEM.QTY%(1,7),                               \
                    DIRORD.ITEM.DETAILS$(1,4),                           \
                    DIRORD.ITEM.DETAILS$(2,1),                           \
                    DIRORD.ITEM.DETAILS$(2,2),                           \
                    DIRORD.ITEM.DETAILS$(2,3),                           \
                    DIRORD.ITEM.QTY%(2,1),                               \
                    DIRORD.ITEM.QTY%(2,2),                               \
                    DIRORD.ITEM.QTY%(2,3),                               \
                    DIRORD.ITEM.QTY%(2,4),                               \
                    DIRORD.ITEM.QTY%(2,5),                               \
                    DIRORD.ITEM.QTY%(2,6),                               \
                    DIRORD.ITEM.QTY%(2,7),                               \
                    DIRORD.ITEM.DETAILS$(2,4),                           \
                    DIRORD.ITEM.DETAILS$(3,1),                           \
                    DIRORD.ITEM.DETAILS$(3,2),                           \
                    DIRORD.ITEM.DETAILS$(3,3),                           \
                    DIRORD.ITEM.QTY%(3,1),                               \
                    DIRORD.ITEM.QTY%(3,2),                               \
                    DIRORD.ITEM.QTY%(3,3),                               \
                    DIRORD.ITEM.QTY%(3,4),                               \
                    DIRORD.ITEM.QTY%(3,5),                               \
                    DIRORD.ITEM.QTY%(3,6),                               \
                    DIRORD.ITEM.QTY%(3,7),                               \
                    DIRORD.ITEM.DETAILS$(3,4),                           \
                    DIRORD.ITEM.DETAILS$(4,1),                           \
                    DIRORD.ITEM.DETAILS$(4,2),                           \
                    DIRORD.ITEM.DETAILS$(4,3),                           \
                    DIRORD.ITEM.QTY%(4,1),                               \
                    DIRORD.ITEM.QTY%(4,2),                               \
                    DIRORD.ITEM.QTY%(4,3),                               \
                    DIRORD.ITEM.QTY%(4,4),                               \
                    DIRORD.ITEM.QTY%(4,5),                               \
                    DIRORD.ITEM.QTY%(4,6),                               \
                    DIRORD.ITEM.QTY%(4,7),                               \
                    DIRORD.ITEM.DETAILS$(4,4),                           \
                    DIRORD.ITEM.DETAILS$(5,1),                           \
                    DIRORD.ITEM.DETAILS$(5,2),                           \
                    DIRORD.ITEM.DETAILS$(5,3),                           \
                    DIRORD.ITEM.QTY%(5,1),                               \
                    DIRORD.ITEM.QTY%(5,2),                               \
                    DIRORD.ITEM.QTY%(5,3),                               \
                    DIRORD.ITEM.QTY%(5,4),                               \
                    DIRORD.ITEM.QTY%(5,5),                               \
                    DIRORD.ITEM.QTY%(5,6),                               \
                    DIRORD.ITEM.QTY%(5,7),                               \
                    DIRORD.ITEM.DETAILS$(5,4),                           \
                    DIRORD.ITEM.DETAILS$(6,1),                           \
                    DIRORD.ITEM.DETAILS$(6,2),                           \
                    DIRORD.ITEM.DETAILS$(6,3),                           \
                    DIRORD.ITEM.QTY%(6,1),                               \
                    DIRORD.ITEM.QTY%(6,2),                               \
                    DIRORD.ITEM.QTY%(6,3),                               \
                    DIRORD.ITEM.QTY%(6,4),                               \
                    DIRORD.ITEM.QTY%(6,5),                               \
                    DIRORD.ITEM.QTY%(6,6),                               \
                    DIRORD.ITEM.QTY%(6,7),                               \
                    DIRORD.ITEM.DETAILS$(6,4),                           \
                    DIRORD.ITEM.DETAILS$(7,1),                           \
                    DIRORD.ITEM.DETAILS$(7,2),                           \
                    DIRORD.ITEM.DETAILS$(7,3),                           \
                    DIRORD.ITEM.QTY%(7,1),                               \
                    DIRORD.ITEM.QTY%(7,2),                               \
                    DIRORD.ITEM.QTY%(7,3),                               \
                    DIRORD.ITEM.QTY%(7,4),                               \
                    DIRORD.ITEM.QTY%(7,5),                               \
                    DIRORD.ITEM.QTY%(7,6),                               \
                    DIRORD.ITEM.QTY%(7,7),                               \
                    DIRORD.ITEM.DETAILS$(7,4),                           \
                    DIRORD.ITEM.DETAILS$(8,1),                           \
                    DIRORD.ITEM.DETAILS$(8,2),                           \
                    DIRORD.ITEM.DETAILS$(8,3),                           \
                    DIRORD.ITEM.QTY%(8,1),                               \
                    DIRORD.ITEM.QTY%(8,2),                               \
                    DIRORD.ITEM.QTY%(8,3),                               \
                    DIRORD.ITEM.QTY%(8,4),                               \
                    DIRORD.ITEM.QTY%(8,5),                               \
                    DIRORD.ITEM.QTY%(8,6),                               \
                    DIRORD.ITEM.QTY%(8,7),                               \
                    DIRORD.ITEM.DETAILS$(8,4),                           \
                    DIRORD.ITEM.DETAILS$(9,1),                           \
                    DIRORD.ITEM.DETAILS$(9,2),                           \
                    DIRORD.ITEM.DETAILS$(9,3),                           \
                    DIRORD.ITEM.QTY%(9,1),                               \
                    DIRORD.ITEM.QTY%(9,2),                               \
                    DIRORD.ITEM.QTY%(9,3),                               \
                    DIRORD.ITEM.QTY%(9,4),                               \
                    DIRORD.ITEM.QTY%(9,5),                               \
                    DIRORD.ITEM.QTY%(9,6),                               \
                    DIRORD.ITEM.QTY%(9,7),                               \
                    DIRORD.ITEM.DETAILS$(9,4),                           \
                    DIRORD.ITEM.DETAILS$(10,1),                          \
                    DIRORD.ITEM.DETAILS$(10,2),                          \
                    DIRORD.ITEM.DETAILS$(10,3),                          \
                    DIRORD.ITEM.QTY%(10,1),                              \
                    DIRORD.ITEM.QTY%(10,2),                              \
                    DIRORD.ITEM.QTY%(10,3),                              \
                    DIRORD.ITEM.QTY%(10,4),                              \
                    DIRORD.ITEM.QTY%(10,5),                              \
                    DIRORD.ITEM.QTY%(10,6),                              \
                    DIRORD.ITEM.QTY%(10,7),                              \
                    DIRORD.ITEM.DETAILS$(10,4),                          \
                    DIRORD.ITEM.DETAILS$(11,1),                          \
                    DIRORD.ITEM.DETAILS$(11,2),                          \
                    DIRORD.ITEM.DETAILS$(11,3),                          \
                    DIRORD.ITEM.QTY%(11,1),                              \
                    DIRORD.ITEM.QTY%(11,2),                              \
                    DIRORD.ITEM.QTY%(11,3),                              \
                    DIRORD.ITEM.QTY%(11,4),                              \
                    DIRORD.ITEM.QTY%(11,5),                              \
                    DIRORD.ITEM.QTY%(11,6),                              \
                    DIRORD.ITEM.QTY%(11,7),                              \
                    DIRORD.ITEM.DETAILS$(11,4),                          \
                    DIRORD.ITEM.DETAILS$(12,1),                          \
                    DIRORD.ITEM.DETAILS$(12,2),                          \
                    DIRORD.ITEM.DETAILS$(12,3),                          \
                    DIRORD.ITEM.QTY%(12,1),                              \
                    DIRORD.ITEM.QTY%(12,2),                              \
                    DIRORD.ITEM.QTY%(12,3),                              \
                    DIRORD.ITEM.QTY%(12,4),                              \
                    DIRORD.ITEM.QTY%(12,5),                              \
                    DIRORD.ITEM.QTY%(12,6),                              \
                    DIRORD.ITEM.QTY%(12,7),                              \
                    DIRORD.ITEM.DETAILS$(12,4),                          \
                    DIRORD.ITEM.DETAILS$(13,1),                          \
                    DIRORD.ITEM.DETAILS$(13,2),                          \
                    DIRORD.ITEM.DETAILS$(13,3),                          \
                    DIRORD.ITEM.QTY%(13,1),                              \
                    DIRORD.ITEM.QTY%(13,2),                              \
                    DIRORD.ITEM.QTY%(13,3),                              \
                    DIRORD.ITEM.QTY%(13,4),                              \
                    DIRORD.ITEM.QTY%(13,5),                              \
                    DIRORD.ITEM.QTY%(13,6),                              \
                    DIRORD.ITEM.QTY%(13,7),                              \
                    DIRORD.ITEM.DETAILS$(13,4),                          \
                    DIRORD.ITEM.DETAILS$(14,1),                          \
                    DIRORD.ITEM.DETAILS$(14,2),                          \
                    DIRORD.ITEM.DETAILS$(14,3),                          \
                    DIRORD.ITEM.QTY%(14,1),                              \
                    DIRORD.ITEM.QTY%(14,2),                              \
                    DIRORD.ITEM.QTY%(14,3),                              \
                    DIRORD.ITEM.QTY%(14,4),                              \
                    DIRORD.ITEM.QTY%(14,5),                              \
                    DIRORD.ITEM.QTY%(14,6),                              \
                    DIRORD.ITEM.QTY%(14,7),                              \
                    DIRORD.ITEM.DETAILS$(14,4),                          \
                    DIRORD.ITEM.DETAILS$(15,1),                          \
                    DIRORD.ITEM.DETAILS$(15,2),                          \
                    DIRORD.ITEM.DETAILS$(15,3),                          \
                    DIRORD.ITEM.QTY%(15,1),                              \
                    DIRORD.ITEM.QTY%(15,2),                              \
                    DIRORD.ITEM.QTY%(15,3),                              \
                    DIRORD.ITEM.QTY%(15,4),                              \
                    DIRORD.ITEM.QTY%(15,5),                              \
                    DIRORD.ITEM.QTY%(15,6),                              \
                    DIRORD.ITEM.QTY%(15,7),                              \
                    DIRORD.ITEM.DETAILS$(15,4),                          \
                    DIRORD.ITEM.DETAILS$(16,1),                          \
                    DIRORD.ITEM.DETAILS$(16,2),                          \
                    DIRORD.ITEM.DETAILS$(16,3),                          \
                    DIRORD.ITEM.QTY%(16,1),                              \
                    DIRORD.ITEM.QTY%(16,2),                              \
                    DIRORD.ITEM.QTY%(16,3),                              \
                    DIRORD.ITEM.QTY%(16,4),                              \
                    DIRORD.ITEM.QTY%(16,5),                              \
                    DIRORD.ITEM.QTY%(16,6),                              \
                    DIRORD.ITEM.QTY%(16,7),                              \
                    DIRORD.ITEM.DETAILS$(16,4)
            ENDIF                                                        \
        ELSE BEGIN
 \***
 \*** Write DIRORD header record
 \***
          FORMAT$ = "C3,C2,4C1,C3,C2,C494"
          IF END #DIRORD.SESS.NUM% THEN WRITE.ERROR
          WRITE FORM FORMAT$; #DIRORD.SESS.NUM%;                         \
                    DIRORD.SUPPLIER$,                                    \
                    DIRORD.ORDER.NO$,                                    \
                    DIRORD.ORDER.SUF$,                                   \
                    DIRORD.BUS.CENTRE$,                                  \
                    DIRORD.SOURCE$,                                      \
                    DIRORD.PAGE.NO$,                                     \
                    DIRORD.DRRF.LAST.DATE$,                              \
                    DIRORD.DRRF.LAST.TIME$,                              \
                    DIRORD.FILLER1$
           ENDIF

      WRITE.DIRORD = 0
      EXIT FUNCTION

      WRITE.ERROR:

         CURRENT.CODE$ = DIRORD.SUPPLIER$
         CURRENT.REPORT.NUM% = DIRORD.REPORT.NUM%
         FILE.OPERATION$ = "O"

         EXIT FUNCTION

  END FUNCTION

\------------------------------------------------------------------------
