\******************************************************************************
\******************************************************************************
\***
\***           FILE FUNCTIONS Boots.com Orders File (BDCO)
\***
\***           REFERENCE:   BDCOFUN.BAS
\***
\***
\***           VERSION B         Dave Constable        7th March 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           Initial version for User Story PMLA-17 & PMLA-58 to allow 
\***           Location access for Parcels by controller and till for 
\***           Boots.com/ie order parcels.
\***
\***
\*******************************************************************************
\*******************************************************************************

INTEGER*2 GLOBAL                       \
          CURRENT.REPORT.NUM%

STRING    GLOBAL                       \
          CURRENT.CODE$,               \
          FILE.OPERATION$

INTEGER*2 cnt%

!STRING    BDCO.KEY$,                                                   !BDC
STRING    BDCO.CARTONS$                                                 !BDC


%INCLUDE BDCODEC.J86


\-----------------------------------------------------------------------------

FUNCTION SPLIT.CARTONS
\*********************

   BDCO.NUM.CARTONS% = 0
   FOR cnt% = 1 TO 55
      BDCO.CARTON$(cnt%) = MID$(BDCO.CARTONS$, ((cnt%-1)*4) +1, 4)
      IF BDCO.CARTON$(cnt%) <> PACK$("00000000") THEN                  \
         BDCO.NUM.CARTONS% = BDCO.NUM.CARTONS% +1
   NEXT cnt%
   BDCO.CARTONS$ = ""

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION JOIN.CARTONS
\********************

   BDCO.CARTONS$ = ""
   FOR cnt% = 1 TO 55
      IF BDCO.CARTON$(cnt%) = "" THEN                                  \
         BDCO.CARTON$(cnt%) = PACK$("00000000")
      BDCO.CARTONS$ = BDCO.CARTONS$ + BDCO.CARTON$(cnt%)
   NEXT cnt%

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION BDCO.SET PUBLIC
\****************************

    BDCO.REPORT.NUM% = 804
    BDCO.KEYL%       =   8
    BDCO.RECL%       = 254
    BDCO.FILE.NAME$  = "BDCO"

    DIM BDCO.CARTON$(55)

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.BDCO PUBLIC
\****************************

    INTEGER*2 READ.BDCO

    READ.BDCO = 1

    BDCO.KEY$ = BDCO.SUPPLIER$ + BDCO.ORDER$

    IF END #BDCO.SESS.NUM% THEN READ.BDCO.ERROR
    READ FORM "T9,C220,C26";                                               \
       #BDCO.SESS.NUM%                 \
       KEY BDCO.KEY$;                  \  8 bytes UPD Supp + Order
       BDCO.CARTONS$,                  \220 bytes UPD Boots.com Consignment#s
       BDCO.FILLER$                    ! 26 bytes filler

    CALL SPLIT.CARTONS

    READ.BDCO = 0
    EXIT FUNCTION

READ.BDCO.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%

       ! Ensure no previous values set after error
       FOR cnt% = 1 TO 55
          BDCO.CARTON$(cnt%) = PACK$("00000000")
       NEXT cnt%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION WRITE.BDCO PUBLIC
\****************************

    INTEGER*2 WRITE.BDCO

    WRITE.BDCO = 1

    BDCO.KEY$ = BDCO.SUPPLIER$ + BDCO.ORDER$

    CALL JOIN.CARTONS

    IF END #BDCO.SESS.NUM% THEN WRITE.BDCO.ERROR
    WRITE FORM "C8,C220,C26";                                              \
       #BDCO.SESS.NUM%;                \
       BDCO.KEY$,                      \  8 bytes UPD Supp + Order
       BDCO.CARTONS$,                  \220 bytes UPD Boots.com consignment#s
       BDCO.FILLER$                    ! 26 bytes filler

    WRITE.BDCO = 0
    EXIT FUNCTION

WRITE.BDCO.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.BDCO.LOCK PUBLIC
\*****************************

    INTEGER*2 READ.BDCO.LOCK

    READ.BDCO.LOCK = 1

    BDCO.KEY$ = BDCO.SUPPLIER$ + BDCO.ORDER$

    IF END #BDCO.SESS.NUM% THEN READ.BDCO.LOCK.ERROR
    READ FORM "T9,C220,C26";                                               \
       #BDCO.SESS.NUM% AUTOLOCK        \
       KEY BDCO.KEY$;                  \  8 bytes UPD Supp + Order
       BDCO.CARTONS$,                  \220 bytes UPD Boots.com Consignment#s
       BDCO.FILLER$                    ! 26 bytes filler

    CALL SPLIT.CARTONS

    READ.BDCO.LOCK = 0
    EXIT FUNCTION

READ.BDCO.LOCK.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%

       ! Ensure no previous values set after error
       FOR cnt% = 1 TO 55
          BDCO.CARTON$(cnt%) = PACK$("00000000")
       NEXT cnt%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION WRITE.BDCO.UNLOCK PUBLIC
\********************************

    INTEGER*2 WRITE.BDCO.UNLOCK

    WRITE.BDCO.UNLOCK = 1

    BDCO.KEY$ = BDCO.SUPPLIER$ + BDCO.ORDER$

    CALL JOIN.CARTONS

    IF END #BDCO.SESS.NUM% THEN WRITE.BDCO.UNLOCK.ERROR
    WRITE FORM "C8,C220,C26";                                              \
       #BDCO.SESS.NUM% AUTOUNLOCK;     \
       BDCO.KEY$,                      \  8 bytes UPD Supp + Order
       BDCO.CARTONS$,                  \220 bytes UPD Boots.com consignment#s
       BDCO.FILLER$                    ! 26 bytes filler


    WRITE.BDCO.UNLOCK = 0
    EXIT FUNCTION

WRITE.BDCO.UNLOCK.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION DELETE.BDCO PUBLIC
\**************************

    INTEGER*2 DELETE.BDCO

    DELETE.BDCO = 1
    BDCO.KEY$ = BDCO.SUPPLIER$ + BDCO.ORDER$
    IF END # BDCO.SESS.NUM% THEN DELETE.ERROR
    DELREC BDCO.SESS.NUM%; BDCO.KEY$
    DELETE.BDCO = 0
    EXIT FUNCTION

DELETE.ERROR:

    FILE.OPERATION$     = "D"
    CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%
    CURRENT.CODE$       = BDCO.KEY$

END FUNCTION

