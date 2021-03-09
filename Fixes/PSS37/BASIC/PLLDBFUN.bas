
\******************************************************************************
\******************************************************************************
\***
\***              RF PICKING LIST ITEMS IN LISTS FILE FUNCTIONS
\***
\***               REFERENCE    : PLLDBFUN.BAS
\***
\***         VERSION A            Julia Stones                  11th August 2004
\***
\***         VERSION B            Charles Skadorwa            15th February 2005
\***               Added WRITE.BRIEF.PLLDB for Shelf Monitor Project.
\***
\***         VERSION C             Mark Goode                   28th August 2008
\***              Add new fields for multi-site project
\***
\***         VERSION D             Dave Constable             20th November 2008
\***              Corrected new fields for multi-site project in PDT Replacement
\***
\***         VERSION E             Peter Sserunkuma           09th December 2008
\***              The format string within the WRITE.BRIEF.PLLDB function now
\***              references a record length of 164, it was previously set to 110.
\***
\***         VERSION F             Neil Bennett                    3rd June 2009
\***              Added functions for lock/unlock. Fixed CONCAT.MS.TABLE.DATA
\***
\***         VERSION G             Wasim AbdulKalam           31st  January  2012
\***              Added new fields for backshop and OSSR pending sales plan for
\***              Stock file Accuracy project. Also commented out the old FORM
\***              statements and variables. PLLDB.CNT.PENDSALES.BACKSHOP$,
\***              PLLDB.TIME.PENDSALES.BACKSHOP$,PLLDB.CNT.PENDSALES.OSSR$,
\***              PLLDB.TIME.PENDSALES.OSSR$ are the newly introduced variables.
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                 \
       CURRENT.REPORT.NUM%

    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$

    %INCLUDE PLLDBDEC.J86

\***********************************************************
                                                                                    ! CMG
  SUB SPLIT.MS.TABLE.DATA

   INTEGER*2 LOOP%                                                                  ! CMG

   FOR LOOP% = 0 TO 32                                                              ! CMG ! GWA

      PLLDB.MS.LOCATION.CNT$(LOOP%) = MID$(PLLDB.MS.TABLE$,1 + (LOOP%*10),2)        ! DDC
      PLLDB.MS.SALES.FIG$(LOOP%)    = MID$(PLLDB.MS.TABLE$,3 + (LOOP%*10),2)        ! DDC
      PLLDB.MS.TIME.CNT$(LOOP%)     = MID$(PLLDB.MS.TABLE$,5 + (LOOP%*10),2)        ! DDC
      PLLDB.MS.FILL.QTY$(LOOP%)     = MID$(PLLDB.MS.TABLE$,7 + (LOOP%*10),2)        ! DDC
      PLLDB.FILLER2$(LOOP%)         = MID$(PLLDB.MS.TABLE$,9 + (LOOP%*10),2)        ! DDC

   NEXT LOOP%                                                                       ! CMG ! WWA

  END SUB                                                                           ! CMG

\***************************************************************

  SUB CONCAT.MS.TABLE.DATA                                                          ! CMG

   INTEGER*2 LOOP%                                                                  ! CMG

   PLLDB.MS.TABLE$ = ""                                                             ! CMG

   FOR LOOP% = 0 TO 32                                                              ! CMG ! GWA

       PLLDB.MS.TABLE$ = PLLDB.MS.TABLE$ + PLLDB.MS.LOCATION.CNT$(LOOP%)            \ FNB
                                         + PLLDB.MS.SALES.FIG$(LOOP%)               \ FNB
                                         + PLLDB.MS.TIME.CNT$(LOOP%)                \ FNB
                                         + PLLDB.MS.FILL.QTY$(LOOP%)                \ FNB
                                         + PLLDB.FILLER2$(LOOP%)                    ! FNB

   NEXT LOOP%                                                                       ! CMG ! WWA

  END SUB                                                                           ! CMG

\***************************************************************

  FUNCTION PLLDB.SET PUBLIC
\***************************

    PLLDB.REPORT.NUM% = 511
    PLLDB.RECL%       = 384                                                          ! CMG !GWA
    PLLDB.FILE.NAME$  = "PLLDB"

    DIM PLLDB.MS.LOCATION.CNT$(32)                                                   ! CMG !GWA
    DIM PLLDB.MS.SALES.FIG$(32)                                                      ! CMG !GWA
    DIM PLLDB.MS.TIME.CNT$(32)                                                       ! CMG !GWA
    DIM PLLDB.MS.FILL.QTY$(32)                                                       ! CMG !GWA
    DIM PLLDB.FILLER2$(32)                                                           ! CMG !GWA

  END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.PLLDB PUBLIC
\****************************

    INTEGER*2 READ.PLLDB

    READ.PLLDB = 1

    PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$

    IF END #PLLDB.SESS.NUM% THEN READ.PLLDB.ERROR
   !Instead of filler pending sales plan fields are introduced.          ! GWA
   !So record length changes in the FORM statement.                      ! GWA
    READ FORM "T7,3C4,2C1,2C4,4C2,C4,3C2,2C4,C330"; #PLLDB.SESS.NUM%   \ ! GWA
       KEY PLLDB.KEY$;                                                 \
       PLLDB.BOOTSCODE$,                                               \
       PLLDB.QTY.ONSHELF$,                                             \
       PLLDB.FILL.QTY$,                                                \
       PLLDB.GAPFILL.MRK$,                                             \
       PLLDB.ITEM.STATUS$,                                             \
       PLLDB.STOCKROOM.CNT$,                                           \
       PLLDB.SHELFMON.SALE.FIG$,                                       \
     \!PLLDB.FILLER$,                                                  \ ! BMG !GWA
       PLLDB.CNT.PENDSALES.BACKSHOP$,                                  \ ! GWA
       PLLDB.TIME.PENDSALES.BACKSHOP$,                                 \ ! GWA
       PLLDB.CNT.PENDSALES.OSSR$,                                      \ ! GWA
       PLLDB.TIME.PENDSALES.OSSR$,                                     \ ! GWA
       PLLDB.OSSR.CNT$,                                                \
       PLLDB.TIME.SHELFMON$,                                           \
       PLLDB.TIME.BACKSHOP$,                                           \
       PLLDB.TIME.OSSR$,                                               \
       PLLDB.BACKSHOP.SALE$,                                           \
       PLLDB.OSSR.SALE$,                                               \
       PLLDB.MS.TABLE$                                                   ! CMG

    CALL SPLIT.MS.TABLE.DATA                                             ! CMG

    READ.PLLDB = 0
    EXIT FUNCTION

READ.PLLDB.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.PLLDB.LOCK PUBLIC                                            ! FNB
\******************************                                            ! FNB

    INTEGER*2 READ.PLLDB.LOCK                                              ! FNB

    READ.PLLDB.LOCK = 1                                                    ! FNB

    PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$                            ! FNB

    IF END #PLLDB.SESS.NUM% THEN READ.PLLDB.LOCK.ERROR
   !Instead of filler pending sales plan fields are introduced.            ! GWA
   !So record length changes in the FORM statement.                        ! GWA
    READ FORM "T7,3C4,2C1,2C4,4C2,C4,3C2,2C4,C330";                    \   ! GWA
              #PLLDB.SESS.NUM% AUTOLOCK                                \   ! GWA
       KEY PLLDB.KEY$;                                                 \   ! FNB
       PLLDB.BOOTSCODE$,                                               \   ! FNB
       PLLDB.QTY.ONSHELF$,                                             \   ! FNB
       PLLDB.FILL.QTY$,                                                \   ! FNB
       PLLDB.GAPFILL.MRK$,                                             \   ! FNB
       PLLDB.ITEM.STATUS$,                                             \   ! FNB
       PLLDB.STOCKROOM.CNT$,                                           \   ! FNB
       PLLDB.SHELFMON.SALE.FIG$,                                       \   ! FNB
     \!PLLDB.FILLER$,                                                  \   ! FNB ! GWA
       PLLDB.CNT.PENDSALES.BACKSHOP$,                                  \   ! GWA
       PLLDB.TIME.PENDSALES.BACKSHOP$,                                 \   ! GWA
       PLLDB.CNT.PENDSALES.OSSR$,                                      \   ! GWA
       PLLDB.TIME.PENDSALES.OSSR$,                                     \   ! GWA
       PLLDB.OSSR.CNT$,                                                \   ! FNB
       PLLDB.TIME.SHELFMON$,                                           \   ! FNB
       PLLDB.TIME.BACKSHOP$,                                           \   ! FNB
       PLLDB.TIME.OSSR$,                                               \   ! FNB
       PLLDB.BACKSHOP.SALE$,                                           \   ! FNB
       PLLDB.OSSR.SALE$,                                               \   ! FNB
       PLLDB.MS.TABLE$                                                     ! FNB

       CALL SPLIT.MS.TABLE.DATA                                            ! FNB

    READ.PLLDB.LOCK = 0                                                    ! FNB
    EXIT FUNCTION                                                          ! FNB

READ.PLLDB.LOCK.ERROR:                                                     ! FNB


       FILE.OPERATION$ = "R"                                               ! FNB
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%                             ! FNB

       EXIT FUNCTION                                                       ! FNB

END FUNCTION                                                               ! FNB

\-----------------------------------------------------------------------------

FUNCTION WRITE.PLLDB PUBLIC
\****************************

    INTEGER*2 WRITE.PLLDB

    WRITE.PLLDB = 1

    PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$

    CALL CONCAT.MS.TABLE.DATA

    IF END #PLLDB.SESS.NUM% THEN WRITE.PLLDB.ERROR
   !Instead of filler pending sales plan fields are introduced.          ! GWA
   !So record length changes in the FORM statement.                      ! GWA
    WRITE FORM "C6,3C4,2C1,2C4,4C2,C4,3C2,2C4,C330"; #PLLDB.SESS.NUM%; \ ! GWA
       PLLDB.KEY$,                                                     \
       PLLDB.BOOTSCODE$,                                               \
       PLLDB.QTY.ONSHELF$,                                             \
       PLLDB.FILL.QTY$,                                                \
       PLLDB.GAPFILL.MRK$,                                             \
       PLLDB.ITEM.STATUS$,                                             \
       PLLDB.STOCKROOM.CNT$,                                           \
       PLLDB.SHELFMON.SALE.FIG$,                                       \
     \!PLLDB.FILLER$,                                                  \ BMG ! GWA
       PLLDB.CNT.PENDSALES.BACKSHOP$,                                  \ ! GWA
       PLLDB.TIME.PENDSALES.BACKSHOP$,                                 \ ! GWA
       PLLDB.CNT.PENDSALES.OSSR$,                                      \ ! GWA
       PLLDB.TIME.PENDSALES.OSSR$,                                     \ ! GWA
       PLLDB.OSSR.CNT$,                                                \
       PLLDB.TIME.SHELFMON$,                                           \
       PLLDB.TIME.BACKSHOP$,                                           \
       PLLDB.TIME.OSSR$,                                               \
       PLLDB.BACKSHOP.SALE$,                                           \
       PLLDB.OSSR.SALE$,                                               \ CMG
       PLLDB.MS.TABLE$                                                 ! CMG

    WRITE.PLLDB = 0
    EXIT FUNCTION

WRITE.PLLDB.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION


\-----------------------------------------------------------------------------

FUNCTION WRITE.PLLDB.UNLOCK PUBLIC                                        ! FNB
\*********************************                                        ! FNB

    INTEGER*2 WRITE.PLLDB.UNLOCK                                          ! FNB

    WRITE.PLLDB.UNLOCK = 1                                                ! FNB

    PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$                           ! FNB

    CALL CONCAT.MS.TABLE.DATA                                             ! FNB

    IF END #PLLDB.SESS.NUM% THEN WRITE.PLLDB.UNLOCK.ERROR                 ! FNB
   !Instead of filler pending sales plan fields are introduced.           ! GWA
   !So record length changes in the FORM statement.                       ! GWA
    WRITE FORM "C6,3C4,2C1,2C4,4C2,C4,3C2,2C4,C330";                   \  ! GWA
               #PLLDB.SESS.NUM% AUTOUNLOCK;                            \  ! GWA
       PLLDB.KEY$,                                                     \  ! FNB
       PLLDB.BOOTSCODE$,                                               \  ! FNB
       PLLDB.QTY.ONSHELF$,                                             \  ! FNB
       PLLDB.FILL.QTY$,                                                \  ! FNB
       PLLDB.GAPFILL.MRK$,                                             \  ! FNB
       PLLDB.ITEM.STATUS$,                                             \  ! FNB
       PLLDB.STOCKROOM.CNT$,                                           \  ! FNB
       PLLDB.SHELFMON.SALE.FIG$,                                       \  ! FNB
     \!PLLDB.FILLER$,                                                  \  ! FNB !GWA
       PLLDB.CNT.PENDSALES.BACKSHOP$,                                  \  ! GWA
       PLLDB.TIME.PENDSALES.BACKSHOP$,                                 \  ! GWA
       PLLDB.CNT.PENDSALES.OSSR$,                                      \  ! GWA
       PLLDB.TIME.PENDSALES.OSSR$,                                     \  ! GWA
       PLLDB.OSSR.CNT$,                                                \  ! FNB
       PLLDB.TIME.SHELFMON$,                                           \  ! FNB
       PLLDB.TIME.BACKSHOP$,                                           \  ! FNB
       PLLDB.TIME.OSSR$,                                               \  ! FNB
       PLLDB.BACKSHOP.SALE$,                                           \  ! FNB
       PLLDB.OSSR.SALE$,                                               \  ! FNB
       PLLDB.MS.TABLE$                                                    ! FNB

    WRITE.PLLDB.UNLOCK = 0                                                ! FNB
    EXIT FUNCTION                                                         ! FNB

WRITE.PLLDB.UNLOCK.ERROR:                                                 ! FNB


       FILE.OPERATION$ = "W"                                              ! FNB
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%                            ! FNB

       EXIT FUNCTION                                                      ! FNB

END FUNCTION                                                              ! FNB


\-----------------------------------------------------------------------------

FUNCTION DELREC.PLLDB PUBLIC
\****************************

    INTEGER*2 DELREC.PLLDB

    DELREC.PLLDB = 1

    PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$

    IF END #PLLDB.SESS.NUM% THEN DELREC.PLLDB.ERROR
    DELREC PLLDB.SESS.NUM%; PLLDB.KEY$

    DELREC.PLLDB = 0
    EXIT FUNCTION

DELREC.PLLDB.ERROR:


       FILE.OPERATION$ = "D"
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION


\-----------------------------------------------------------------------------

FUNCTION WRITE.BRIEF.PLLDB PUBLIC                                         !BCS
\****************************

    INTEGER*2 WRITE.BRIEF.PLLDB

    WRITE.BRIEF.PLLDB = 1

    !PLLDB.KEY$ = PLLDB.LISTID$ + PLLDB.ITEMSEQ$ !

    IF END #PLLDB.SESS.NUM% THEN WRITE.PLLDB.ERROR
    !Instead of filler pending sales plan fields are introduced.
    !So record length changes in the FORM statement.                 !GWA
    WRITE FORM "C384"; #PLLDB.SESS.NUM%; PLLDB.KEY$      ! CMG ! EPS !GWA

    WRITE.BRIEF.PLLDB = 0
    EXIT FUNCTION

WRITE.PLLDB.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = PLLDB.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION


