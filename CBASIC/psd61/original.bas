\********************************************************************
\********************************************************************
\***
\***
\***            PROGRAM         :       PSD61
\***            MODULE          :       PSD6100
\***            AUTHOR          :       Neil Bennett
\***            DATE WRITTEN    :       January 2007
\***
\***            MODULE          :       PSD6100.BAS
\***
\********************************************************************
\***
\***    VERSION A.            NEIL BENNETT.              25 Jan 2007.
\***    Initial version.
\***
\***    VERSION B.            PAUL BOWERS                19 March 2007.
\***
\***    Defect Number BTCPR00000096
\***    Status P Cartons shown as unknown when in fact they are
\***    superceded orders booked in by old Direct Order processing.
\***
\***    Defect Number BTCPR00000094
\***    Various formatting fixes on Exception booked in Carton
\***
\***    Defect Number BTCPR00000110
\***    Remove Press ENTER to cont after booking in Carton
\***
\***    VERSION C.            CHARLES SKADORWA  CCSk     11 April 2007.
\***                          NEIL BENNETT      CNWB
\***    Defect Number BTCPR00000134
\***    Fix to ensure that if an Carton has already been marked for
\***    processing it will not generate another entry on the Carton
\***    Buffer file if selected again.
\***
\***    Defect Number BTCPR00000135
\***    Fix to prevent program quitting out when a carton number
\***    is entered that is not on file.
\***
\***    Performance enhancement - using BUFFSIZE of 32768 has
\***
\***    VERSION D.            PAUL BOWERS       DPAB     18 April 2007.
\***    Fix to ASN item count.
\***
\***    VERSION E.            HARPAL MATHARU    EHSM     25 October 2007
\***    Fixed the report to display more than 61 items. Only read total
\***    number of items from first sector of recall
\***
\***    VERSION F.            CHARLES SKADORWA    FCSk     29 June 2009
\***    Fixed bug where if there are >120 items in a ASN, then program
\***    aborts with a SU - subscript out of bounds error when you try
\***    to book it in. the array subscript j% should be reset to 1
\***    every 60 items, before reading the next chain. Not seen in store
\***    until now due to stores booking in ASN's via PPC/PDT/MC70(POD).
\***
\***    REVISION 1.10.                ROBERT COWEY.                07 DEC 2009.
\***    Changes for 10A Waitrose Food Trial creating PSD61.286 Rv 1.10.
\***    Prevent display (and therefore premature book-in) of Waitrose
\***    ASN having future expected delivery date.
\***
\***    VERSION G.            RAMYA RAJENDRAN              13 Oct 2014
\***    FOD399 - Boots.com Enhancements
\***    As part of CR008, changed the text to "BOOTS.IE" instead of
\***    "BOOTS.COM" in Book In By Carton screen for ROI controller.
\***
\********************************************************************
\********************************************************************

\********************************************************************
\********************************************************************
\***
\***    Module Overview
\***    ---------------
\***
\***      VIEW/BOOK IN CARTONS.
\***
\***      This program may be called from the Direct Receiving menu
\***      (pss59).
\***      It comprises 3 screens:
\***
\***         The first screen asks for a supplier reference and
\***         carton number if known.
\***
\***         If the carton number is not known a second screen
\***         displays all cartons for the chosen supplier from which
\***         a particular carton may be selected.
\***
\***         The third screen displays all basic details for the
\***         carton and allows the user to book in the carton
\***         automatically. If this option is selected, a Carton
\***         Buffer file will be created and the 'Book in delivery
\***         by carton' batch program (PSD62) will be started to
\***         process it.
\***
\***         This screen will also allow the user to view the carton
\***         at an item level and can be toggled between booked in
\***         and outstanding orders.
\***
\********************************************************************
\********************************************************************

\********************************************************************
\***
\***    Function globals
\***
\********************************************************************

   %INCLUDE PSBF01G.J86         ! Application Logging
   %INCLUDE PSBF05G.J86         ! Check Boots Code
   %INCLUDE PSBF18G.J86         ! Boots Code Check Digit utility
   %INCLUDE PSBF20G.J86         ! Sess Num Utility
   %INCLUDE PSBF39G.J86         ! New DM Functions
   %INCLUDE PSBUSEG.J86         ! Chain parameters

   %INCLUDE BCSMFDEC.J86
   %INCLUDE CBDEC.J86
   %INCLUDE CRTNDEC.J86
   %INCLUDE DIRSUDEC.J86
   %INCLUDE IDFDEC.J86
   %INCLUDE SOFTSDEC.J86        ! Software Status File                  !GRR

\********************************************************************
\***
\***    PSD6100 variables
\***
\********************************************************************

   STRING    GLOBAL CURRENT.CODE$
   STRING    GLOBAL FILE.OPERATION$
   STRING    GLOBAL MODULE.NUMBER$
   STRING    GLOBAL SB.ACTION$
   STRING    GLOBAL SB.FILE.NAME$
   INTEGER*2 GLOBAL SB.INTEGER%
   INTEGER*2 GLOBAL SB.FILE.REP.NUM%
   INTEGER*2 GLOBAL SB.FILE.SESS.NUM%
   STRING    GLOBAL SB.STRING$

   STRING    asn$
   STRING    bc.arr$(1)
   STRING    BATCH.SCREEN.FLAG$
   INTEGER*1 BCSMF.OPEN%
   INTEGER*4 blk.size%
   STRING    cb.chk$                                                   ! CNWB
   INTEGER*1 CB.OPEN.FLAG%
   STRING    CB.FN$
   STRING    cb.lst$                                                   ! CNWB
   STRING    cb.sts$                                                   ! CNWB
   STRING    CHAINING.TO.PROG$
   STRING    crtb.lst$
   STRING    crtn.arr$(2)
   STRING    crtn.lst$
   INTEGER*1 CRTN.OPEN%
   STRING    crtu.lst$
   INTEGER*2 CURRENT.SESS.NUM%
   INTEGER*1 DIRSUP.OPEN%
   INTEGER*2 EVENT.NUMBER%
   STRING    EXP.DEL$
   INTEGER*1 FALSE                ! Boolean                             !GRR
   STRING    fCAR$
   STRING    file.no$
   STRING    form$
   INTEGER*2 found%
   STRING    fSUP$
   INTEGER*2 i%
   STRING    instruct2$
   STRING    instruct3$
   INTEGER*2 indx%
   INTEGER*2 itm%
   STRING    itm.arr$(1)
   INTEGER*2 j%
   INTEGER*2 k%                                                        ! CNWB
   INTEGER*4 last.blk%
   INTEGER*1 list.ok%
   INTEGER*2 lpp2%
   INTEGER*2 lpp3%
   INTEGER*2 MATCH.POS%           ! Return code for Match function      !GRR
   INTEGER*2 max.scrn2%
   INTEGER*2 max.scrn3%
   INTEGER*2 MESSAGE.NUMBER%
   STRING    NUM.ITEMS$
   INTEGER*4 num.blks%
   INTEGER*2 num.recs%
   STRING    OP.NUM$
   STRING    OPERATOR.NUMBER$
   STRING    page$
   INTEGER*2 rc%
   INTEGER*4 rc4%
   STRING    rcd$
   INTEGER*2 RET.KEY%
   INTEGER*1 SCREEN.COMPLETE
   INTEGER*2 sav.i2%
   INTEGER*2 sav.s2%
   INTEGER*2 scr.i%
   INTEGER*2 scrn2%
   INTEGER*2 scrn3%
   STRING    STATE$
   STRING    sect$
   INTEGER*1 SOFTS.OPEN           ! Boolean                             !GRR
   STRING    SOFTS.REC.LABEL$     ! ROI store label(EIRE)               !GRR
   STRING    SUP.NAME$
   STRING    tmp.arr$(1)
   INTEGER*1 toggle%
   INTEGER*2 tot.asn%
   INTEGER*2 tot.bcs%
   INTEGER*2 tot.crtb%
   INTEGER*2 tot.crtn%
   INTEGER*2 tot.crtu%
   INTEGER*4 TOTAL.NUM.ITEMS%                                   ! EHSM
   INTEGER*1 TRUE                 ! Boolean                             !GRR
   STRING    VAR.STRING.1$
   STRING    VAR.STRING.2$
   INTEGER*1 views%
   STRING    work$

\********************************************************************
\***
\***    External functions
\***
\********************************************************************

   %INCLUDE PSBF01E.J86        ! APPLICATION LOG
   %INCLUDE PSBF05E.J86        ! Check Boots Code
   %INCLUDE PSBF18E.J86        ! Boots Code Check Digit utility
   %INCLUDE PSBF20E.J86        ! SESSION NUMBER UTILITY
   %INCLUDE PSBF24E.J86        ! STANDARD ERROR DETECTED
   %INCLUDE PSBF39E.J86        ! Display Manager

   %INCLUDE ADXSTART.J86
   %INCLUDE BCSMFEXT.J86
   %INCLUDE CBEXT.J86
   %INCLUDE CRTNEXT.J86
   %INCLUDE DIRSUEXT.J86
   %INCLUDE IDFEXT.J86
   %INCLUDE SOFTSEXT.J86       ! Software Status File                   !GRR

\********************************************************************
\***
\***    PSD6100 functions
\***
\********************************************************************

\********************************************************************
\***
\***    FUNCTION        valid.sup%
\***
\***    Validates Entered Supplier Number
\***
\********************************************************************

   FUNCTION valid.sup%
      INTEGER*1 valid.sup%

      valid.sup% = 0
      IF fSUP$ = "" THEN BEGIN
         EXIT FUNCTION
      ENDIF

      RC% = CHECK.BOOTS.CODE (RIGHT$("0000000"+STR$(VAL(fSUP$)),7))
      IF RC% <> 0 THEN BEGIN
         EXIT FUNCTION
      ENDIF

      IF F05.VALID.CODE$ = "Y" THEN BEGIN
         fSUP$ = RIGHT$("000000" + STR$(VAL(fSUP$)), 6)
         valid.sup% = -1
      ENDIF

   END FUNCTION

\********************************************************************
\***
\***    FUNCTION        valid.car%
\***
\***    Validates Entered Supplier Number
\***
\********************************************************************

   FUNCTION valid.car%
      INTEGER*1 valid.car%

      valid.car% = 0

      IF fCAR$ = "" THEN BEGIN
         valid.car% = -1
         EXIT FUNCTION
      ENDIF ELSE BEGIN
         fCAR$ = RIGHT$("00000000" + STR$(VAL(fCAR$)), 8)
         IF fCAR$ > "00000000"                                      \
         OR fCAR$ <= "99999999" THEN BEGIN
            valid.car% = -1
         ENDIF ELSE IF VAL(fCAR$) = 0 THEN BEGIN
            valid.car% = -1
            fCAR$ = ""
         ENDIF
      ENDIF

   END FUNCTION

\********************************************************************
\***
\***    FUNCTION        month$
\***
\***    Return 3 letter month from 2 digit month string
\***
\********************************************************************

   FUNCTION month$(inp$)
      STRING month$
      STRING inp$

      IF LEN(inp$) < 1                                              \
      OR LEN(inp$) > 2 THEN BEGIN
         month$ = "   "                                             ! BPAB
         EXIT FUNCTION
      ENDIF

      inp$ = RIGHT$("00" + STR$(VAL(inp$)), 2)
      IF inp$ = "01" THEN month$ = "JAN"                            \
      ELSE IF inp$ = "02" THEN month$ = "FEB"                       \
      ELSE IF inp$ = "03" THEN month$ = "MAR"                       \
      ELSE IF inp$ = "04" THEN month$ = "APR"                       \
      ELSE IF inp$ = "05" THEN month$ = "MAY"                       \
      ELSE IF inp$ = "06" THEN month$ = "JUN"                       \
      ELSE IF inp$ = "07" THEN month$ = "JUL"                       \
      ELSE IF inp$ = "08" THEN month$ = "AUG"                       \
      ELSE IF inp$ = "09" THEN month$ = "SEP"                       \
      ELSE IF inp$ = "10" THEN month$ = "OCT"                       \
      ELSE IF inp$ = "11" THEN month$ = "NOV"                       \
      ELSE IF inp$ = "12" THEN month$ = "DEC"                       \
      ELSE month$ = "   "                                           ! BPAB

   END FUNCTION

\******************************************************************** ! BPAB
\***                                                                  ! BPAB
\***    FUNCTION        delivery$                                     ! BPAB
\***                                                                  ! BPAB
\***    function to convert 000 delivery values to spaces             ! BPAB
\***                                                                  ! BPAB
\******************************************************************** ! BPAB
                                                                      ! BPAB
   FUNCTION delivery$(inp$)                                           ! BPAB
                                                                      ! BPAB
      STRING delivery$                                                ! BPAB
      STRING inp$                                                     ! BPAB
                                                                      ! BPAB
      IF VAL(inp$) = 0 THEN BEGIN                                     ! BPAB
         delivery$ = "    "                                           ! BPAB
         EXIT FUNCTION                                                ! BPAB
      ENDIF                                                           ! BPAB
                                                                      ! BPAB
      delivery$ = inp$                                                ! BPAB
                                                                      ! BPAB
   END FUNCTION                                                       ! BPAB
                                                                      ! BPAB
\********************************************************************
\***
\***    FUNCTION        status$
\***
\***    Return 19 character status from single status letter
\***
\********************************************************************

   FUNCTION status$(inp$)
      STRING status$
      STRING inp$

      IF LEN(inp$) <> 1 THEN BEGIN
         status$ = "**** Not Known ****"
      ENDIF

      IF inp$ = "U" THEN status$ = "Unbooked           "            \
      ELSE IF inp$ = "N" THEN status$ = "Booked In Normally "       \
      ELSE IF inp$ = "P" THEN status$ = "Booked In via Order"       \ ! BPAB
      ELSE IF inp$ = "A" THEN status$ = "Booked via Audit   "       \
      ELSE IF inp$ = "E" THEN status$ = "Booked by Exception"       \
      ELSE IF inp$ = "*" THEN status$ = "Processing         "       \ ! BPAB
      ELSE status$ = "**** Not Known ****"

   END FUNCTION

\********************************************************************
\***
\***    FUNCTION        get.item.desc$
\***
\***    Get descriptor for boots item code
\***
\********************************************************************

   FUNCTION get.item.desc$(inp$)
      STRING get.item.desc$
      STRING inp$

      IDF.BOOTS.CODE$ = PACK$(RIGHT$("00000000" + inp$,8))

      rc% = -1                                                  ! BPAB
      IF VAL(UNPACK$(IDF.BOOTS.CODE$)) <> 0 THEN BEGIN          ! BPAB
         rc% = READ.IDF
      ENDIF

      IF rc% <> 0 THEN BEGIN
         IDF.STNDRD.DESC$ = ">>> Item not on file <<<"
      ENDIF

      get.item.desc$ = LEFT$(IDF.STNDRD.DESC$ + STRING$(24," "),24)

   END FUNCTION

\********************************************************************
\***
\***    FUNCTION        get.item.code$
\***
\***    Get check digit and format boots item code
\***
\********************************************************************

   FUNCTION get.item.code$(inp$)
      STRING get.item.code$
      STRING inp$

      IF LEN(inp$) <> 3 THEN BEGIN
         get.item.code$ = " "
         EXIT FUNCTION
      ENDIF

      inp$ = UNPACK$(inp$)

      IF (CALC.BOOTS.CODE.CHECK.DIGIT(inp$) <> 0                     \
     AND F18.CHECK.DIGIT$ = "A")                                     \ BPAB
      OR (VAL(inp$) = 0) THEN BEGIN                                  ! BPAB
         get.item.code$ = " "
         EXIT FUNCTION
      ENDIF
      inp$ = inp$ + F18.CHECK.DIGIT$

      get.item.code$ = MID$(inp$, 1, 2) + "-"                       \
                     + MID$(inp$, 3, 2) + "-"                       \
                     + MID$(inp$, 5, 3) + " "                       \
                     + get.item.desc$(inp$)

   END FUNCTION

\********************************************************************
\***
\***    SUB        :       DM.FIELD.CHANGES
\***
\***    PERFORMS TASKS WHEN CERTAIN FIELDS HAVE BEEN ALTERED
\***
\********************************************************************

   SUB DM.FIELD.CHANGED (DM.SCREEN%,                                \
                         DM.FIELD%,                                 \
                         VALUE$,                                    \
                         VALID,                                     \
                         UPDATE) PUBLIC

      INTEGER*2 DM.SCREEN% !CURRENT SCREEN NUMBER
      INTEGER*2 DM.FIELD%  !FIELD MODIFIED
      STRING    VALUE$     !NEW VALUE FOR FIELD (CAN BE MODIFIED)
      INTEGER*1 VALID      !RETURN FALSE IF FIELD INVALID
      INTEGER*1 UPDATE     !RETURN TRUE IF UPDATED OUTPUT FIELDS


      IF DM.SCREEN% = 1 THEN BEGIN

          ! Supplier Reference
          IF DM.FIELD% = 2 THEN BEGIN
             IF valid.sup% THEN BEGIN
                VALUE$ = fSUP$
                UPDATE = -1
             ENDIF ELSE BEGIN
                CALL DM.MESSAGE ("fSUP$", "MESSAGE(221,"            \
                   + "'A Valid Supplier Number must be entered.')")
                VALID = 0
             ENDIF
          ENDIF

          ! Carton Number
          IF DM.FIELD% = 3 THEN BEGIN
             IF valid.car% THEN BEGIN
                VALUE$ = fCAR$
                UPDATE = -1
             ENDIF ELSE BEGIN
                VALID = 0
             ENDIF
          ENDIF

      ENDIF

   END SUB

\********************************************************************
\***
\***    Start of Mainline
\***
\********************************************************************

   ON ERROR GOTO ERR.DETECTED

   CHAINING.TO.PROG$ = "B50  "      ! for B553 msg (default=PSB50)

   %INCLUDE PSBUSEE.J86

START.PROGRAM:

   PSBCHN.APP = "ADX_UPGM:PSD61.286"

   BATCH.SCREEN.FLAG$ = "S"
   OPERATOR.NUMBER$   = PSBCHN.OP
   OP.NUM$            = PSBCHN.OP
   MODULE.NUMBER$     = "PSD6100"

   GOSUB INITIALISATION

   CALL DM.INIT
   RET.KEY% = ENTER.KEY%

   WHILE RET.KEY% <> F3.KEY%                                        \
     AND RET.KEY% <> ESC.KEY%

      GOSUB DISPLAY.SCREEN1
      GOSUB PROCESS.SCREEN1

      IF RET.KEY% <> F3.KEY%                                        \
     AND RET.KEY% <> ESC.KEY% THEN BEGIN

         IF list.ok% THEN BEGIN

            WHILE RET.KEY% <> F3.KEY%                               \
              AND RET.KEY% <> ESC.KEY%

               GOSUB DISPLAY.SCREEN2
               GOSUB PROCESS.SCREEN2

               IF RET.KEY% <> F3.KEY%                               \
              AND RET.KEY% <> ESC.KEY% THEN BEGIN
                  IF itm% = 0 THEN BEGIN
                     ! B055 No details exist ......
                     CALL DM.FOCUS("", "MESSAGE(055,'selection.')")
                  ENDIF ELSE BEGIN

                     GOSUB DISPLAY.SCREEN3
                     GOSUB PROCESS.SCREEN3

                  ENDIF
               ENDIF

               IF list.ok%                                          \
              AND RET.KEY% = F3.KEY% THEN RET.KEY% = ENTER.KEY%

            WEND

            IF RET.KEY% = F3.KEY% THEN RET.KEY% = ENTER.KEY%

         ENDIF ELSE BEGIN

            WHILE RET.KEY% <> F3.KEY%                               \
              AND RET.KEY% <> ESC.KEY%

                  GOSUB DISPLAY.SCREEN3
                  GOSUB PROCESS.SCREEN3

            WEND

            IF RET.KEY% = F3.KEY% THEN RET.KEY% = ENTER.KEY%

         ENDIF

      ENDIF

   WEND

   IF CB.OPEN.FLAG% THEN BEGIN
      CLOSE CB.SESS.NUM%
      cb.lst$ = ""                                                  ! CNWB
      CB.OPEN.FLAG% = 0
      rc4% = ADXSTART("ADX_UPGM:PSD62.286",                         \
                      CB.FN$,                                       \
                      "Carton Book In Batch Program")
      RET.KEY% = ENTER.KEY%
      WHILE RET.KEY% <> F3.KEY%
         RET.KEY% = DM.INVISIBLE.INPUT("221 "                       \
                                   + "'Carton Book in process "     \
                                   + "started Press F3 to Exit.' "  \
                                   + "MESSAGE")
      WEND
   ENDIF

END.PROGRAM:

   GOSUB TERMINATE

   CALL DM.QUIT

   CHAINING.TO.PROG$ = "B50  "     ! for B553 msg
   PSBCHN.PRG = "ADX_UPGM:PSB50.286"
   %INCLUDE PSBCHNE.J86

   STOP

\********************************************************************
\***
\***    Subroutines
\***
\********************************************************************
\***
\***    SUBROUTINE      :      DISPLAY.SCREEN1
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

DISPLAY.SCREEN1:

   fCAR$ = ""
   fSUP$ = ""
   CALL DM.SHOW.SCREEN (1, "", 11, 11)

   CALL DM.NAME (2, "fSUP$", fSUP$)
   CALL DM.NAME (3, "fCAR$", fCAR$)

   CALL DM.VALID ("fSUP$", "fSUP$ >= 1 AND fSUP$ <= 999999")
   CALL DM.VALID ("fCAR$", "fCAR$ >= 0 AND fCAR$ <= 99999999")
   !B003 Invalid selection number
   CALL DM.MESSAGE ("fSUP$", "MESSAGE(221,"                         \
                   + "'A Valid Supplier Number must be entered.')")
   CALL DM.MESSAGE ("fCAR$", "MESSAGE(221,"                         \
                             + "'Invalid Carton Number')")
   CALL DM.SHOW.FN.KEY( 1, "")

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      PROCESS.SCREEN1
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

PROCESS.SCREEN1:

   list.ok% = 0
   scrn2% = 1                                                   ! PAB

   SCREEN.COMPLETE = 0

   WHILE NOT SCREEN.COMPLETE

      RET.KEY% = DM.PROCESS.SCREEN (2, 3, 0)

      IF RET.KEY% = ESC.KEY% THEN BEGIN
         RETURN
      ENDIF ELSE IF RET.KEY% = F3.KEY% THEN BEGIN
         RETURN
      ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN
         GOSUB RETRIEVE.SUPPLIER.NAME
         IF SUP.NAME$ <> "" THEN BEGIN
            IF fCAR$ = "" THEN BEGIN
               ! B405 Processing...
               CALL DM.STATUS ("MESSAGE(405,'')")
               GOSUB BUILD.CARTON.LIST
               IF NOT list.ok% THEN BEGIN
                  CALL DM.FOCUS("", "MESSAGE(221,"                  \
                      + "'No Cartons on file for this supplier.')")
               ENDIF ELSE BEGIN
                  SCREEN.COMPLETE = -1
               ENDIF
            ENDIF ELSE BEGIN
               itm% = 0
               ! B405 Processing...
               CALL DM.STATUS ("MESSAGE(405,'')")
               GOSUB GET.CARTON.DETAIL
               IF itm% = 0 THEN BEGIN
                  ! B055 No details exist ......
                  CALL DM.FOCUS ("", "MESSAGE(055,'selection.')")
               ENDIF ELSE BEGIN
                  SCREEN.COMPLETE = -1
               ENDIF
            ENDIF
         ENDIF
      ENDIF ELSE BEGIN
         ! B001 Invalid key pressed
         CALL DM.FOCUS ("", "MESSAGE(1,'')")
      ENDIF

   WEND

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      DISPLAY.SCREEN2
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

DISPLAY.SCREEN2:

   CALL DM.SHOW.SCREEN (2, "", 21, 21)

   CALL DM.NAME (16, "page$", page$)
   CALL DM.NAME (17, "fSUP$", fSUP$)
   CALL DM.NAME (18, "SUP.NAME$", SUP.NAME$)
   CALL DM.NAME (19, "instruct2$", instruct2$)

   DIM tmp.arr$(lpp2% +1)

   GOSUB CHANGE.SCREEN2

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      CHANGE.SCREEN2
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

CHANGE.SCREEN2:

   i% = VAL(crtn.arr$(toggle%, 0))
   max.scrn2% = INT%((i% +lpp2% -1) / lpp2%)

   page$ = "Page " + RIGHT$("  " + STR$(scrn2%),2)                  \
         + " of "  + RIGHT$("  " + STR$(max.scrn2%),2)

   FOR i% = 2 TO lpp2% +1

      indx% = ((scrn2% -1) *lpp2%) + (i% - 1)

      IF indx% <= VAL(crtn.arr$(toggle%,0)) THEN BEGIN
         tmp.arr$(i%) = crtn.arr$(toggle%,indx%)
         CALL DM.NAME(i%,"f"+STR$(i%)+"$",tmp.arr$(i%))
         CALL DM.VISIBLE (STR$(i%), "TRUE")
         CALL DM.RO.FIELD(i%)
      ENDIF ELSE BEGIN
         tmp.arr$(i%) = ""
         CALL DM.VISIBLE (STR$(i%), "FALSE")
      ENDIF

   NEXT i%

   IF VAL(crtn.arr$(toggle%, 0)) = 0 THEN BEGIN
      ! B055 No details exist ......
      CALL DM.FOCUS ("", "MESSAGE(055,'selection.')")
   ENDIF

   IF scrn2% < max.scrn2% THEN BEGIN
      CALL DM.SHOW.FN.KEY( 8, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 8)
   ENDIF
   IF scrn2% > 1 THEN BEGIN
      CALL DM.SHOW.FN.KEY( 7, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 7)
   ENDIF
   IF views% <> 0 THEN BEGIN
      CALL DM.SHOW.FN.KEY( 5, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 5)
   ENDIF
   CALL DM.SHOW.FN.KEY( 1, "")

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      PROCESS.SCREEN2
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

PROCESS.SCREEN2:

   SCREEN.COMPLETE = 0

   WHILE NOT SCREEN.COMPLETE

      RET.KEY% = DM.PROCESS.SCREEN (2, lpp2% +1, 0)

      IF RET.KEY% = ESC.KEY% THEN BEGIN
         DIM crtn.arr$(0,0)
         list.ok% = 0
         scrn2% = 1                                                 ! PAB
         RETURN
      ENDIF ELSE IF RET.KEY% = F3.KEY% THEN BEGIN
         DIM crtn.arr$(0,0)
         list.ok% = 0
         scrn2% = 1                                                 ! PAB
         RETURN
      ENDIF ELSE IF views% <> 0                                     \
                AND RET.KEY% = F5.KEY% THEN BEGIN
         ! Toggle Display mode - All/Unbooked/booked (1/2/3)
         toggle% = toggle% +1
         IF toggle% > 3 THEN toggle% = 1
         scrn2% = 1
         GOSUB CHANGE.SCREEN2
      ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN
         itm% = 0
         scr.i% = DM.CURRENT.FIELD(0)
         scr.i% = (scr.i% -1) + ((scrn2% -1) *lpp2%)

         IF LEFT$(crtn.arr$(toggle%,scr.i%),4) = "ASN:" THEN BEGIN
            CALL DM.FOCUS ("","MESSAGE(221,"                        \
                             + "'You must select a Carton.')")
         ENDIF ELSE BEGIN
            fCAR$ = MID$(crtn.arr$(toggle%,scr.i%), 3, 8)
            GOSUB GET.CARTON.DETAIL
            IF itm% = 0 THEN BEGIN
               ! B055 No details exist ......
               CALL DM.FOCUS ("", "MESSAGE(055,'selection.')")
            ENDIF ELSE BEGIN
               sav.s2% = scrn2%
               sav.i2% = DM.CURRENT.FIELD(0)
               ! B405 Processing...
               CALL DM.STATUS ("MESSAGE(405,'')")
               SCREEN.COMPLETE = -1
            ENDIF
         ENDIF

      ENDIF ELSE IF ( RET.KEY% = F7.KEY%                            \
                   OR RET.KEY% = PGUP.KEY%                          \
                   OR RET.KEY% = PREV.KEY%)                         \
                AND (scrn2% > 1) THEN BEGIN
         scrn2% = scrn2% -1
         IF RET.KEY% = PREV.KEY% THEN BEGIN
            CALL DM.CURRENT.FIELD(lpp2% +1)
         ENDIF ELSE BEGIN
            CALL DM.CURRENT.FIELD(1 +1)
         ENDIF
         GOSUB CHANGE.SCREEN2
      ENDIF ELSE IF ( RET.KEY% = F8.KEY%                            \
                   OR RET.KEY% = PGDN.KEY%                          \
                   OR RET.KEY% = NEXT.KEY%)                         \
                AND (scrn2% < max.scrn2%) THEN BEGIN
         scrn2% = scrn2% +1
         CALL DM.CURRENT.FIELD(1 +1)
         GOSUB CHANGE.SCREEN2
      ENDIF ELSE IF ( RET.KEY% = PGDN.KEY%                          \
                AND   scrn2% = max.scrn2% ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(075,'')")
      ENDIF ELSE IF ( RET.KEY% = PGUP.KEY%                          \
                AND   scrn2% = 1          ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(074,'')")
      ENDIF ELSE BEGIN
         ! B001 Invalid key pressed
         CALL DM.FOCUS ("", "MESSAGE(1,'')")
      ENDIF

   WEND

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      DISPLAY.SCREEN3
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

DISPLAY.SCREEN3:

   max.scrn3% = INT%((itm% +lpp3% -1) / lpp3%)

   CALL DM.SHOW.SCREEN (3, "", 31, 31)

   CALL DM.TAB.ORDER (1)

   CALL DM.NAME (29, "instruct3$", instruct3$)
   CALL DM.NAME (30, "fSUP$", fSUP$)
   CALL DM.NAME (31, "SUP.NAME$", SUP.NAME$)
   CALL DM.NAME (32, "asn$", asn$)
   CALL DM.NAME (33, "fCAR$", fCAR$)
   CALL DM.NAME (34, "NUM.ITEMS$", NUM.ITEMS$)
   CALL DM.NAME (35, "STATE$", STATE$)
   CALL DM.NAME (36, "EXP.DEL$", EXP.DEL$)
   CALL DM.NAME (37, "page$", page$)

   DIM tmp.arr$(lpp3% +1)

   GOSUB CHANGE.SCREEN3

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      CHANGE.SCREEN3
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

CHANGE.SCREEN3:

   page$ = "Page " + RIGHT$("  " + STR$(scrn3%),2)                  \
         + " of "  + RIGHT$("  " + STR$(max.scrn3%),2)

   FOR i% = 2 TO lpp3% +1

      indx% = ((scrn3% -1) *lpp3%) + (i% - 1)

      IF indx% <= itm% THEN BEGIN
         tmp.arr$(i%) = itm.arr$(indx%)
         CALL DM.NAME(i%,"f"+STR$(i%)+"$",tmp.arr$(i%))
         CALL DM.VISIBLE (STR$(i%), "TRUE")
         CALL DM.RO.FIELD(i%)
      ENDIF ELSE BEGIN
         tmp.arr$(i%) = ""
         CALL DM.VISIBLE (STR$(i%), "FALSE")
      ENDIF

   NEXT i%

   IF itm% = 0 THEN BEGIN
      ! B055 No details exist ......
      CALL DM.FOCUS ("", "MESSAGE(055,'selection.')")
   ENDIF

   IF scrn3% < max.scrn3% THEN BEGIN
      CALL DM.SHOW.FN.KEY( 8, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 8)
   ENDIF
   IF scrn3% > 1 THEN BEGIN
      CALL DM.SHOW.FN.KEY( 7, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 7)
   ENDIF
   IF STATE$ = status$("U") THEN BEGIN
      CALL DM.SHOW.FN.KEY( 6, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 6)
   ENDIF
   CALL DM.SHOW.FN.KEY( 1, "")

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      PROCESS.SCREEN3
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

PROCESS.SCREEN3:

   SCREEN.COMPLETE = 0

   WHILE NOT SCREEN.COMPLETE

      RET.KEY% = DM.PROCESS.SCREEN (2, lpp3% +1, 0)

      IF RET.KEY% = ESC.KEY% THEN BEGIN
         DIM tmp.arr$(0)
         scrn3% = 1                                                 ! PAB
         RETURN
      ENDIF ELSE IF RET.KEY% = F3.KEY% THEN BEGIN
         DIM tmp.arr$(0)
         scrn3% = 1                                                 ! PAB
         RETURN
      ENDIF ELSE IF RET.KEY% = F6.KEY% THEN BEGIN
         ! Confirm
         IF STATE$ <> status$("U") THEN BEGIN
            CALL DM.FOCUS ("","MESSAGE(221,"                        \
                          + "'This carton is already Booked In.')")
         ENDIF ELSE BEGIN
            GOSUB BOOK.IN.CARTON
            DIM tmp.arr$(0)
            ! --------------------------------------------------------
            ! defect BTCPR00000110 remove the Press ENTER to cont
            ! requested by Lee Dorr 22/3/07
            ! --------------------------------------------------------
            ! RET.KEY% = F3.KEY%
            ! WHILE RET.KEY% <> ENTER.KEY%
            !   RET.KEY% = DM.INVISIBLE.INPUT("221 "                 \
            !                       + "'Carton Book in pending - "   \
            !                       + "Press ENTER to continue.' "   \
            !                       + "MESSAGE")
            !WEND
            RET.KEY% = F3.KEY%
            SCREEN.COMPLETE = -1
         ENDIF
      ENDIF ELSE IF ( RET.KEY% = F7.KEY%                            \
                   OR RET.KEY% = PGUP.KEY%                          \
                   OR RET.KEY% = PREV.KEY%)                         \
                AND (scrn3% > 1) THEN BEGIN
         scrn3% = scrn3% -1
         IF RET.KEY% = PREV.KEY% THEN BEGIN
            CALL DM.CURRENT.FIELD(lpp3% +1)
         ENDIF ELSE BEGIN
            CALL DM.CURRENT.FIELD(1 +1)
         ENDIF
         GOSUB CHANGE.SCREEN3
      ENDIF ELSE IF ( RET.KEY% = F8.KEY%                            \
                   OR RET.KEY% = PGDN.KEY%                          \
                   OR RET.KEY% = NEXT.KEY%)                         \
                AND (scrn3% < max.scrn3%) THEN BEGIN
         scrn3% = scrn3% +1
         CALL DM.CURRENT.FIELD(1 +1)
         GOSUB CHANGE.SCREEN3
      ENDIF ELSE IF ( RET.KEY% = PGDN.KEY%                          \
                AND   scrn3% = max.scrn3% ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(075,'')")
      ENDIF ELSE IF ( RET.KEY% = PGUP.KEY%                          \
                AND   scrn3% = 1          ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(074,'')")
      ENDIF ELSE BEGIN
         ! B001 Invalid key pressed
         CALL DM.FOCUS ("", "MESSAGE(1,'')")
      ENDIF

   WEND

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      INITIALISATION
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

INITIALISATION:

   scrn2% = 1
   scrn3% = 1

   lpp2% = 14
   lpp3% = 24

   toggle% = 1
   list.ok% = 0

   cb.chk$ = ""                                                        !CNWB
   cb.lst$ = ""                                                        !CNWB

   SB.ACTION$ = "O"
   SOFTS.REC.LABEL$ = "EIRE"                                            !GRR

   ! Setting Boolean flag value                                         !GRR
   FALSE = 0                                                            !GRR
   TRUE  = -1                                                           !GRR

   CALL BCSMF.SET
   CALL CB.SET
   CALL CRTN.SET
   CALL DIRSUP.SET
   CALL IDF.SET
   CALL SOFTS.SET                                                       !GRR

   SOFTS.OPEN   = FALSE                                                 !GRR

   SB.INTEGER%  = BCSMF.REPORT.NUM%
   SB.STRING$   = BCSMF.FILE.NAME$
   GOSUB SB.FILE.UTILS
   BCSMF.SESS.NUM% = SB.FILE.SESS.NUM%

   CURRENT.SESS.NUM% = BCSMF.SESS.NUM%
   IF END # BCSMF.SESS.NUM% THEN OPEN.ERROR
   OPEN BCSMF.FILE.NAME$ DIRECT RECL 512 AS BCSMF.SESS.NUM%       \
                                            BUFFSIZE 32768        \   !CCSk
                                            NOWRITE NODEL
   BCSMF.OPEN% = -1

   GOSUB BUILD.BC.LETTER.TABLE

   BCSMF.OPEN% = 0
   CLOSE BCSMF.SESS.NUM%

   SB.ACTION$ = "C"
   SB.INTEGER% = BCSMF.SESS.NUM%
   SB.STRING$ = ""
   GOSUB SB.FILE.UTILS

   SB.ACTION$ = "O"

   SB.INTEGER%  = CB.REPORT.NUM%
   SB.STRING$   = CB.FILE.NAME$
   GOSUB SB.FILE.UTILS
   CB.SESS.NUM% = SB.FILE.SESS.NUM%

   SB.INTEGER%  = CRTN.REPORT.NUM%
   SB.STRING$   = CRTN.FILE.NAME$
   GOSUB SB.FILE.UTILS
   CRTN.SESS.NUM% = SB.FILE.SESS.NUM%

   SB.INTEGER%  = DIRSUP.REPORT.NUM%
   SB.STRING$   = DIRSUP.FILE.NAME$
   GOSUB SB.FILE.UTILS
   DIRSUP.SESS.NUM% = SB.FILE.SESS.NUM%

   SB.INTEGER%  = IDF.REPORT.NUM%
   SB.STRING$   = IDF.FILE.NAME$
   GOSUB SB.FILE.UTILS
   IDF.SESS.NUM% = SB.FILE.SESS.NUM%

   ! Allocate session number for file SOFTS                             !GRR
   SB.INTEGER% = SOFTS.REPORT.NUM%                                      !GRR
   SB.STRING$  = SOFTS.FILE.NAME$                                       !GRR
   GOSUB SB.FILE.UTILS                                                  !GRR
   SOFTS.SESS.NUM% = SB.FILE.SESS.NUM%                                  !GRR

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      RETRIEVE.SUPPLIER.NAME
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\***
\***   set dirsup supplier number to supplier reference
\***   set supplier found flag to false
\***   set count to 1
\***   WHILE count < total no of business centres in table
\***     AND supplier not found
\***      set dirsup business centre to next centre in table
\***      format dirsup record key
\***      key read dirsup file
\***      IF record found
\***         set supplier found to true
\***      increment count
\***   WEND
\***   IF the supplier is found in the DIRSUP file
\***      set supplier name to dirsup supplier name
\***   ELSE
\***      set supplier name to blank
\***      display message 241 (This supplier is not on file)
\***   ENDIF
\***   RETURN
\***
\-------------------------------------------------------------------------------

RETRIEVE.SUPPLIER.NAME:

   CURRENT.SESS.NUM% = DIRSUP.SESS.NUM%
   IF END #DIRSUP.SESS.NUM% THEN OPEN.ERROR
   OPEN DIRSUP.FILE.NAME$ KEYED RECL DIRSUP.RECL%                   \
        AS DIRSUP.SESS.NUM%  BUFFSIZE 32768 NOWRITE NODEL           !CCSk


   DIRSUP.OPEN% = -1

   DIRSUP.SUPPLIER.NO$ = PACK$(fSUP$)
   found% = 0
   i%     = 1
   WHILE (i% <= tot.bcs%) AND NOT found%
      DIRSUP.BUS.CENTRE$ = bc.arr$(i%)
      DIRSUP.RECKEY$ = DIRSUP.BUS.CENTRE$ + DIRSUP.SUPPLIER.NO$
      rc% = READ.DIRSUP
      IF rc% = 1 THEN GOTO NO.DIRSUP.RECORD
      found% = -1
NO.DIRSUP.RECORD:
      i% = i% +1
   WEND
   IF found% THEN BEGIN
       SUP.NAME$ = DIRSUP.SUPPLIER.NAME$

       ! To check whether the store is UK or ROI                        !GRR
       GOSUB CHECK.ROI.STORE                                            !GRR

   ENDIF ELSE BEGIN
      CALL DM.FOCUS("","MESSAGE(241,'')")
      SUP.NAME$ = ""
   ENDIF

   IF DIRSUP.OPEN% THEN BEGIN
      CLOSE DIRSUP.SESS.NUM%
   ENDIF

RETURN

\***********************************************************************!GRR
\*                                                                      !GRR
\* CHECK.ROI.STORE:                                                     !GRR
\* Reads and checks whether the store is configured as ROI or UK from   !GRR
\* SOFTS 19th record.                                                   !GRR
\*                                                                      !GRR
\***********************************************************************!GRR

CHECK.ROI.STORE:                                                        !GRR

    ! Opening SOFTS file                                                !GRR
    IF END # SOFTS.SESS.NUM% THEN OPEN.ERROR                            !GRR
    OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL% AS SOFTS.SESS.NUM%    !GRR
    SOFTS.OPEN = TRUE                                                   !GRR

    ! Read SOFTS 60th record                                            !GRR
    SOFTS.REC.NUM% = 60                                                 !GRR
    IF READ.SOFTS THEN GOSUB READ.ERROR                                 !GRR

    ! Check if Supplier ID is Boots.com supplier.                       !GRR
    MATCH.POS% = MATCH(UNPACK$(DIRSUP.SUPPLIER.NO$), SOFTS.RECORD$, 1)  !GRR

    IF MATCH.POS% > 0 THEN BEGIN                                        !GRR

        ! Read SOFTS 19th record                                        !GRR
        SOFTS.REC.NUM% = 19                                             !GRR
        IF READ.SOFTS THEN GOSUB READ.ERROR                             !GRR

        ! Check if the ROI store label is found. If found update        !GRR
        ! SUP.NAME$ as "BOOTS.IE", else update SUP.NAME$ as "BOOTS.COM" !GRR
        MATCH.POS% = MATCH(SOFTS.REC.LABEL$, SOFTS.RECORD$, 1)          !GRR

        IF MATCH.POS% > 0 THEN BEGIN                                    !GRR
            SUP.NAME$ = "BOOTS.IE"          ! ROI store                 !GRR
        ENDIF ELSE BEGIN                                                !GRR
            SUP.NAME$ = "BOOTS.COM"         ! UK store                  !GRR
        ENDIF                                                           !GRR

    ENDIF                                                               !GRR

    ! De-allocate session number for SOFTS file                         !GRR
    SB.INTEGER% = SOFTS.SESS.NUM%                                       !GRR
    SB.STRING$  = ""                                                    !GRR
    CLOSE SB.INTEGER%                                                   !GRR
    SOFTS.OPEN  = FALSE                                                 !GRR
    GOSUB SB.FILE.UTILS                                                 !GRR

RETURN                                                                  !GRR

\********************************************************************
\***
\***    SUBROUTINE      :      BUILD.BC.LETTER.TABLE
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\***
\***   Dimension a business centre table to hold 16 entries.
\***   Load all non-pseudo business centre letters from the BCSMF
\***   into the table keeping a count of entries added.
\***   Add a blank entry to the table to cater for records on the
\***   dirsup file for suppliers with business centre letter in key
\***   set to blank.
\***   RETURN
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

BUILD.BC.LETTER.TABLE:

   DIM bc.arr$(16)
   tot.bcs% = 0

   num.blks% = SIZE(BCSMF.FILE.NAME$)/512
   num.recs% = INT%(508/BCSMF.RECL%)

   FOR i% = 2 TO num.blks%
      IF END # BCSMF.SESS.NUM% THEN READ.ERROR
      READ FORM "C4,C508"; # BCSMF.SESS.NUM%, i%; work$, sect$
      FOR j% = 1 TO num.recs%
         rcd$ = MID$(sect$, ((j% -1)*BCSMF.RECL%)+1, BCSMF.RECL%)
         IF MID$(rcd$,  1, 1) <> PACK$("00")                           \
        AND MID$(rcd$, 29, 1) <> "Y" THEN BEGIN
            tot.bcs% = tot.bcs% +1
            bc.arr$(tot.bcs%) = MID$(rcd$, 1, 1)
         ENDIF
      NEXT j%
   NEXT i%
   tot.bcs% = tot.bcs% +1
   bc.arr$(tot.bcs%) = " "

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      BUILD.CARTON.LIST
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

BUILD.CARTON.LIST:

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Open CARTON file and set initial values
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   list.ok% = 0

   blk.size% = 32256
   num.blks% = (SIZE(CRTN.FILE.NAME$) / blk.size%) +1
   last.blk% = MOD(SIZE(CRTN.FILE.NAME$),  blk.size%)
   num.recs% = 63

   tot.crtn% = 0
   tot.crtu% = 0
   tot.crtb% = 0
   crtn.lst$ = ""
   crtu.lst$ = ""
   crtb.lst$ = ""

   DIM tmp.arr$(0)
   DIM crtn.arr$(0,0)

   CURRENT.SESS.NUM% = CRTN.SESS.NUM%
   IF END # CRTN.SESS.NUM% THEN OPEN.ERROR
   OPEN CRTN.FILE.NAME$ AS CRTN.SESS.NUM%  BUFFSIZE 32768 NOWRITE NODEL    !CCSk
   CRTN.OPEN% = -1

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Extract matching supplier records from CARTON file
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   IF END # CRTN.SESS.NUM% THEN READ.ERROR
   FOR i% = 1 TO num.blks%
      CALL DM.STATUS("MESSAGE(221,'Processing .. >"                 \
                     + STRING$(      (i%*50)/num.blks% , CHR$(178)) \
                     + STRING$(50 - ((i%*50)/num.blks%), CHR$(176)) \
                     + "<')")
      IF i% = num.blks% THEN BEGIN
         form$ = "C" + STR$(last.blk%)
         num.recs% = last.blk% / 512
      ENDIF ELSE BEGIN
         form$ = "C" + STR$(blk.size%)
      ENDIF
      READ FORM form$; # CRTN.SESS.NUM%; sect$
      FOR j% = 1 TO num.recs%
         IF i% = 1 AND j% = 1 THEN j% = 2
         rcd$ = MID$(sect$, ((j% -1) *512) +5, 508)

!        Bypass Waitrose ASN having future expected delivery date      ! 1.10 RC
         IF    MID$(rcd$,34,1) = "H" \        ! B/C is Food (Waitrose) ! 1.10 RC
           AND MID$(rcd$,37,6) > DATE$ THEN \ ! Deliv Date is future   ! 1.10 RC
\            Simulate null CRTN.KEY$ to cause record to be bypassed    ! 1.10 RC
             rcd$ = STRING$(8, CHR$(0)) + MID$(rcd$, 9, len(rcd$) -8)  ! 1.10 RC

         IF MID$(rcd$, 1, 8) <> STRING$(8, CHR$(0)) THEN BEGIN
            ! ---------------------------------------------------------- CNWB
            ! Check for cartons already added to the CB file and flag    CNWB
            ! ---------------------------------------------------------- CNWB
            cb.chk$ = PACK$(RIGHT$("000000"  + fSUP$,6))               \ CNWB
                    + MID$(rcd$, 4, 4)                                 ! CNWB
            IF UNPACK$(MID$(rcd$, 1, 3)) = fSUP$                       \
           AND         MID$(rcd$, 8, 1)  = CHR$(0) THEN BEGIN
               cb.sts$ = MID$(rcd$, 9, 1)                              ! CNWB
               IF cb.lst$ <> "" THEN BEGIN                             ! CNWB
                  FOR k% = 0 TO (LEN(cb.lst$) /7) -1                   ! CNWB
                     IF MID$(cb.lst$,(k% *7)+1,7) = cb.chk$ THEN BEGIN ! CNWB
                        cb.sts$ = "*"                                  ! CNWB
                        k% = (LEN(cb.lst$) /7) -1                      ! CNWB
                     ENDIF                                             ! CNWB
                  NEXT k%                                              ! CNWB
               ENDIF                                                   ! CNWB
               work$ = MID$(rcd$,10,18)       \  1 ASN      (asc)
                     + MID$(rcd$, 4, 4)       \ 19 Carton # (upd)
                     + MID$(rcd$,35,12)       \ 23 ExpDelDt (asc)
\                    + MID$(rcd$, 9, 1)       ! 35 Status   (asc)      ! CNWB
                     + cb.sts$                ! 35 Status   (asc)      ! CNWB
               tot.crtn% = tot.crtn% +1
               crtn.lst$ = crtn.lst$ + work$
               IF MID$(rcd$, 9, 1) = "U" THEN BEGIN
                  tot.crtu% = tot.crtu% +1
                  crtu.lst$ = crtu.lst$ + work$
               ENDIF ELSE BEGIN
                  tot.crtb% = tot.crtb% +1
                  crtb.lst$ = crtb.lst$ + work$
               ENDIF
            ENDIF
         ENDIF
      NEXT j%
   NEXT i%

   CLOSE CRTN.SESS.NUM%
   CRTN.OPEN% = 0

   IF tot.crtn% = 0 THEN BEGIN
      RETURN
   ENDIF

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Populate arrays
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   DIM tmp.arr$(tot.crtn%)

   FOR i% = 1 TO tot.crtn%
      tmp.arr$(i%) = MID$(crtn.lst$, ((i% -1)*35) +1, 35)
   NEXT i%

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Sort All
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   FOR i% = 1 TO tot.crtn% -1
      rcd$ = tmp.arr$(i%)
      FOR j% = i% +1 TO tot.crtn%
         work$ = tmp.arr$(j%)
         IF work$ < rcd$ THEN BEGIN
            tmp.arr$(i%) = work$
            tmp.arr$(j%) = rcd$
            rcd$ = work$
         ENDIF
      NEXT j%
   NEXT i%

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Count number of ASN's
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   asn$ = MID$(tmp.arr$(1), 1, 18)
   tot.asn% = 1
   FOR i% = 2 to tot.crtn%
      IF MID$(tmp.arr$(i%), 1, 18) <> asn$ THEN BEGIN
         tot.asn% = tot.asn% +1
         asn$ = MID$(tmp.arr$(i%), 1, 18)
      ENDIF
   NEXT i%

   DIM crtn.arr$(3,tot.crtn% + tot.asn%)

   crtn.arr$(1,0) = "0"
   crtn.arr$(2,0) = "0"
   crtn.arr$(3,0) = "0"

   FOR i% = 1 TO tot.crtu%
      crtn.arr$(1,i%) = MID$(crtu.lst$, ((i% -1)*35) +1, 35)
   NEXT i%
   FOR i% = 1 TO tot.crtb%
      crtn.arr$(2,i%) = MID$(crtb.lst$, ((i% -1)*35) +1, 35)
   NEXT i%
   crtn.lst$ = ""
   crtu.lst$ = ""
   crtb.lst$ = ""

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Sort Unbooked
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   IF tot.crtu% > 1 THEN BEGIN
      FOR i% = 1 TO tot.crtu% -1
         rcd$ = crtn.arr$(1,i%)
         FOR j% = i% +1 TO tot.crtu%
            work$ = crtn.arr$(1,j%)
            IF work$ < rcd$ THEN BEGIN
               crtn.arr$(1,i%) = work$
               crtn.arr$(1,j%) = rcd$
               rcd$ = work$
            ENDIF
         NEXT j%
      NEXT i%
   ENDIF

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Sort Booked
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   IF tot.crtb% > 1 THEN BEGIN
      FOR i% = 1 TO tot.crtb% -1
         rcd$ = crtn.arr$(2,i%)
         FOR j% = i% +1 TO tot.crtb%
            work$ = crtn.arr$(2,j%)
            IF work$ < rcd$ THEN BEGIN
               crtn.arr$(2,i%) = work$
               crtn.arr$(2,j%) = rcd$
               rcd$ = work$
            ENDIF
         NEXT j%
      NEXT i%
   ENDIF

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Set print lines including ASN headers
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Display Booked
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   asn$ = ""
   j% = 0
   IF tot.crtb% > 0 THEN BEGIN
      FOR i% = 1 TO tot.crtb%
         IF MID$(crtn.arr$(2,i%), 1, 18) <> asn$ THEN BEGIN
            j% = j% +1
            IF MID$(crtn.arr$(2,i%),1,18) =                        \! BPAB
               "000000000000000000" THEN BEGIN                      ! BPAB
               crtn.arr$(3,j%) = "ASN: " + MID$(crtn.arr$(2,i%),1,18)
            ENDIF ELSE BEGIN                                        ! BPAB
               crtn.arr$(3,j%) = "ASN: " + "Unknown Number   "      ! BPAB
            ENDIF                                                   ! BPAB
            asn$ = MID$(crtn.arr$(2,i%), 1, 18)
         ENDIF
         j% = j% +1
         crtn.arr$(3,j%) = "  "                                     \
                         + UNPACK$(MID$(crtn.arr$(2,i%),19, 4))     \
                         + "      "                                 \
                         + MID$(crtn.arr$(2,i%),29, 2)              \
                         + " "                                      \
                         + month$(MID$(crtn.arr$(2,i%),27,2))       \
                         + " "                                      \
                         + delivery$(MID$(crtn.arr$(2,i%),23, 4))   \ BPAB
                         + "     "                                  \
                         + status$(MID$(crtn.arr$(2,i%),35, 1))     !
         crtn.arr$(2,i%) = ""
      NEXT i%
   ENDIF
   crtn.arr$(3,0) = STR$(j%)

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Display Unbooked
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   asn$ = ""
   j% = 0
   IF tot.crtu% > 0 THEN BEGIN
      FOR i% = 1 TO tot.crtu%
         IF MID$(crtn.arr$(1,i%), 1, 18) <> asn$ THEN BEGIN
            j% = j% +1
            IF MID$(crtn.arr$(1,i%),1,18) =                             \! BPAB
               "000000000000000000" THEN BEGIN                           ! BPAB
                crtn.arr$(2,j%) = "ASN: " + "Unknown Number   "          ! BPAB
            ENDIF ELSE BEGIN                                             ! BPAB
                crtn.arr$(2,j%) = "ASN: " + MID$(crtn.arr$(1,i%),1,18)
            ENDIF                                                        ! BPAB
            crtn.arr$(2,j%) = "ASN: " + MID$(crtn.arr$(1,i%),1,18)
            asn$ = MID$(crtn.arr$(1,i%), 1, 18)
         ENDIF
         j% = j% +1
         crtn.arr$(2,j%) = "  "                                     \
                         + UNPACK$(MID$(crtn.arr$(1,i%),19, 4))     \
                         + "      "                                 \
                         + MID$(crtn.arr$(1,i%),29, 2)              \
                         + " "                                      \
                         + month$(MID$(crtn.arr$(1,i%),27,2))       \
                         + " "                                      \
                         + delivery$(MID$(crtn.arr$(1,i%),23, 4))   \!BPAB
                         + "     "                                  \
                         + status$(MID$(crtn.arr$(1,i%),35, 1))     !
         crtn.arr$(1,i%) = ""
      NEXT i%
   ENDIF
   crtn.arr$(2,0) = STR$(j%)

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Display All
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   asn$ = ""
   j% = 0
   FOR i% = 1 TO tot.crtn%
      IF MID$(tmp.arr$(i%), 1, 18) <> asn$ THEN BEGIN
         j% = j% +1
         IF MID$(tmp.arr$(i%),1,18) = "000000000000000000" THEN BEGIN ! BPAB
             crtn.arr$(1,j%) = "ASN: " + "Unknown Number   "          ! BPAB
         ENDIF ELSE BEGIN                                             ! BPAB
             crtn.arr$(1,j%) = "ASN: " + MID$(tmp.arr$(i%),1,18)
         ENDIF                                                        ! BPAB
         asn$ = MID$(tmp.arr$(i%), 1, 18)
      ENDIF
      j% = j% +1
      crtn.arr$(1,j%) = "  "                                        \
                      + UNPACK$(MID$(tmp.arr$(i%),19, 4))           \
                      + "      "                                    \
                      + MID$(tmp.arr$(i%),29, 2)                    \
                      + " "                                         \
                      + month$(MID$(tmp.arr$(i%),27,2))             \
                      + " "                                         \
                      + delivery$(MID$(tmp.arr$(i%),23, 4))         \
                      + "     "                                     \
                      + status$(MID$(tmp.arr$(i%),35, 1))           !
      tmp.arr$(i%) = ""
   NEXT i%
   crtn.arr$(1,0) = STR$(j%)

   list.ok% = -1

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Tidy up
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   IF VAL(crtn.arr$(2,0)) <> 0                                      \
  AND VAL(crtn.arr$(2,0)) <> VAL(crtn.arr$(1,0)) THEN BEGIN
      views% = -1
      instruct2$    = "Press F5 to toggle view through ALL/OUTS"    \
                    + "TANDING/BOOKED IN Cartons.             "
   ENDIF ELSE BEGIN
      views% = 0
      instruct2$    = "                                        "    \
                    + "                                       "
   ENDIF

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Tidy up
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   DIM tmp.arr$(0)
   tot.asn%  = 0
   tot.crtn% = 0
   tot.crtu% = 0
   tot.crtb% = 0
   rcd$  = ""
   work$ = ""

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      GET.CARTON.DETAIL
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

GET.CARTON.DETAIL:

   CURRENT.SESS.NUM% = IDF.SESS.NUM%
   IF END # IDF.SESS.NUM% THEN OPEN.ERROR
   OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%        \
                                                   NOWRITE NODEL

   CURRENT.SESS.NUM% = CRTN.SESS.NUM%
   IF END # CRTN.SESS.NUM% THEN OPEN.ERROR
   OPEN CRTN.FILE.NAME$ KEYED RECL CRTN.RECL% AS CRTN.SESS.NUM%     \
                                                   NOWRITE NODEL
   CRTN.OPEN% = -1

   CRTN.SUPPLIER$ = PACK$(RIGHT$("000000"   + fSUP$, 6))
   CRTN.NO$       = PACK$(RIGHT$("00000000" + fCAR$, 8))
   CRTN.CHAIN%    = 0

   RC% = READ.CRTN
   TOTAL.NUM.ITEMS% =  VAL(CRTN.ITEM.CNT$)                              ! EHSM

   IF RC% = 0 THEN BEGIN                                                ! CCSk

       ! ---------------------------------------------------------------- CNWB
       ! Check for cartons already added to the CB file and flag          CNWB
       ! ---------------------------------------------------------------- CNWB
       cb.sts$       = CRTN.STATUS$                                     ! CNWB
       cb.chk$       = CRTN.SUPPLIER$ + CRTN.NO$                        ! CNWB
       IF cb.lst$ <> "" THEN BEGIN                                      ! CNWB
          FOR k% = 0 TO (LEN(cb.lst$) /7) -1                            ! CNWB
             IF MID$(cb.lst$,(k% *7)+1,7) = cb.chk$ THEN BEGIN          ! CNWB
                cb.sts$ = "*"                                           ! CNWB
                k% = (LEN(cb.lst$) /7) -1                               ! CNWB
             ENDIF                                                      ! CNWB
          NEXT k%                                                       ! CNWB
       ENDIF                                                            ! CNWB

       asn$          = CRTN.ASN.CODE$
\      STATE$        = status$(CRTN.STATUS$)                            ! CNWB
       STATE$        = status$(cb.sts$)                                 ! CNWB
       EXP.DEL$      = MID$(CRTN.DEL.DTTM$, 7, 2) + " "                 \
                     + month$(MID$(CRTN.DEL.DTTM$, 5, 2)) + " "         \
                     + delivery$(MID$(CRTN.DEL.DTTM$, 1, 4))            ! BPAB
       NUM.ITEMS$    = RIGHT$("     " + STR$(TOTAL.NUM.ITEMS%), 5)      ! EHSM

       IF STATE$ = status$("U") THEN BEGIN
          instruct3$    = "Press F6 to CONFIRM booking this Carton."    \
                        + " Press F3 or ESC to cancel.             "
       ENDIF ELSE IF STATE$ = status$("*") THEN BEGIN                   ! CNWB
          instruct3$    = "You are viewing a Carton that is already"    \ CNWB
                        + " listed for book in. Press F3 to exit.  "    ! CNWB
       ENDIF ELSE BEGIN
          instruct3$    = "You are viewing a Carton that is already"    \
                        + " booked in. Press F3 to exit.           "
       ENDIF

       DIM itm.arr$(TOTAL.NUM.ITEMS%)                                      ! EHSM
       itm% = 0

       j% = 0
       FOR i% = 1 TO TOTAL.NUM.ITEMS%                                      ! EHSM
          j% = j% +1

          !IF j% > (60*(CRTN.CHAIN%+1)) THEN BEGIN                          ! BPAB  !FCSk
          IF j% > 60 THEN BEGIN                                                     !FCSk
             CRTN.CHAIN% = CRTN.CHAIN% +1                                  ! BPAB
             j% = 1                                                        ! BPAB
             RC% = READ.CRTN                                               ! BPAB
             IF RC% <> 0 THEN BEGIN
                GOTO READ.ERROR
             ENDIF
          ENDIF
          itm% = itm% +1
          IF STATE$ = status$("U") THEN BEGIN                              ! BPAB
             itm.arr$(itm%) = get.item.code$(CRTN.ITEM.CODE$(j%))          \
                       + RIGHT$("    " + STR$(CRTN.DESP.QTY%(j%)), 4)
          ENDIF ELSE BEGIN                                                ! BPAB
             ! if already booked in show qty what was booked in           ! BPAB
             itm.arr$(itm%) = get.item.code$(CRTN.ITEM.CODE$(j%))         \ BPAB
                       + RIGHT$("    " + STR$(CRTN.IN.QTY%(j%)), 4)       ! BPAB
          ENDIF                                                           ! BPAB

          IF LEFT$(itm.arr$(itm%),1) = " " THEN BEGIN                     ! BPAB
             !NUM.ITEMS$    = RIGHT$("     " + STR$(VAL(NUM.ITEMS$)-1), 5) ! BPAB DPAB
             itm% = itm% - 1                                              ! BPAB
                                                                         ! BPAB
          ENDIF

       NEXT i%
   ENDIF ELSE BEGIN                                                       ! CCSk
       itm% = 0                                                           ! CCSk
   ENDIF                                                                  ! CCSk

   CLOSE IDF.SESS.NUM%
   CLOSE CRTN.SESS.NUM%
   CRTN.OPEN% = 0

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
\*** Sort Items
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   IF itm% > 1 THEN BEGIN
      FOR i% = 1 TO itm% -1
         rcd$ = itm.arr$(i%)
         FOR j% = i% +1 TO itm%
            work$ = itm.arr$(j%)
            IF LEFT$(work$,9) < LEFT$(rcd$,9) THEN BEGIN
               itm.arr$(i%) = work$
               itm.arr$(j%) = rcd$
               rcd$ = work$
            ENDIF
         NEXT j%
      NEXT i%
   ENDIF


RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      BOOK.IN.CARTON
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

BOOK.IN.CARTON:

   IF NOT CB.OPEN.FLAG% THEN BEGIN
      GOSUB OPEN.CB.FILE
   ENDIF

   IF NOT CB.OPEN.FLAG% THEN BEGIN
      GOTO END.PROGRAM
   ENDIF

   CB.REC.TYPE$       = "C"
   CB.CARTON.BARCODE$ = fSUP$ + fCAR$
   CB.REPORT.RQD$     = "Y"                                             ! PAB

   RC% = WRITE.CB
   IF RC% <> 0 THEN BEGIN
      GOTO WRITE.ERROR
   ENDIF

   IF CRTN.OPEN% THEN BEGIN
      CLOSE CRTN.SESS.NUM%
   ENDIF

   CURRENT.SESS.NUM% = CRTN.SESS.NUM%
   IF END # CRTN.SESS.NUM% THEN OPEN.ERROR
   OPEN CRTN.FILE.NAME$ KEYED RECL CRTN.RECL% AS CRTN.SESS.NUM%
   CRTN.OPEN% = -1

   CRTN.SUPPLIER$ = PACK$(RIGHT$("000000"   + fSUP$, 6))
   CRTN.NO$       = PACK$(RIGHT$("00000000" + fCAR$, 8))
   CRTN.CHAIN%    = 0

   ! -------------------------------------------------------------------- CNWB
   ! Save a list of all book in requests                                  CNWB
   ! -------------------------------------------------------------------- CNWB
   cb.lst$ = cb.lst$ + CRTN.SUPPLIER$ + CRTN.NO$                        ! CNWB

   RC% = READ.CRTN
   IF RC% <> 0 THEN BEGIN
      GOTO READ.ERROR
   ENDIF

   CRTN.STATUS$ = "N"

   ! ---------------------------------------------------------------------------- PAB
   ! Dont update the status on the crtn file as PSD62 will reject the book in     PAB
   ! Defect BTCRP00000112                                                         PAB
   ! ---------------------------------------------------------------------------- PAB
   !
   !RC% = WRITE.CRTN
   !IF RC% <> 0 THEN BEGIN
   !   GOTO WRITE.ERROR
   !ENDIF

   CLOSE CRTN.SESS.NUM%
   CRTN.OPEN% = 0

   IF list.ok% THEN BEGIN
      work$ = crtn.arr$(toggle%, scr.i%)
      crtn.arr$(toggle%, scr.i%) = LEFT$(work$, LEN(work$) -19)     \
                                 + status$("*")
      IF toggle% = 1 THEN BEGIN
         FOR i% = 1 TO VAL(crtn.arr$(2,0))
            IF crtn.arr$(2,i%) = work$ THEN BEGIN
               crtn.arr$(2,i%) = LEFT$(work$, LEN(work$) -19)       \
                               + status$("*")
               i% = VAL(crtn.arr$(2,0))
            ENDIF
         NEXT i%
      ENDIF ELSE IF toggle% = 2 THEN BEGIN
         FOR i% = 1 TO VAL(crtn.arr$(1,0))
            IF crtn.arr$(1,i%) = work$ THEN BEGIN
               crtn.arr$(1,i%) = LEFT$(work$, LEN(work$) -19)       \
                               + status$("*")
               i% = VAL(crtn.arr$(1,0))
            ENDIF
         NEXT i%
      ENDIF
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      OPEN.CB.FILE
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

OPEN.CB.FILE:

   CB.FN$ = CB.FILE.NAME$ + TIME$
   CURRENT.SESS.NUM% = CB.SESS.NUM%
   IF END #CB.SESS.NUM% THEN CB.CREATE.ERROR
   CREATE CB.FN$ AS CB.SESS.NUM%
   CB.OPEN.FLAG% = -1

RETURN

CB.CREATE.ERROR:

   CB.FN$ = CB.FILE.NAME$ + TIME$
   IF END #CB.SESS.NUM% THEN CB.CREATE.ERROR2
   CREATE CB.FN$ AS CB.SESS.NUM%
   CB.OPEN.FLAG% = -1

RETURN

CB.CREATE.ERROR2:

   FILE.OPERATION$ = "C"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      FILE.ERROR
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

FILE.ERROR:

   IF SB.ACTION$ = "C" THEN RETURN             ! Ignore close errs

   event.number%   = 106
   message.number% = 0

   file.no$ = CHR$(SHIFT(CURRENT.SESS.NUM%,8)) +                    \
              CHR$(SHIFT(CURRENT.SESS.NUM%,0))

   var.string.2$ = RIGHT$("000" + STR$(CURRENT.SESS.NUM%),3)

   IF FILE.OPERATION$ = "R" THEN BEGIN
          var.string.2$ = var.string.2$                             \
                        + UNPACK$(CURRENT.CODE$)
   ENDIF

   var.string.1$ = FILE.OPERATION$ +                                \
                   file.no$ +                                       \
                   PACK$(STRING$(12,"0"))

   CALL APPLICATION.LOG(message.number%,                            \
                        var.string.1$,                              \
                        var.string.2$,                              \
                        event.number%)

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      TERMINATE
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

TERMINATE:

      SB.ACTION$ = "C"

      SB.INTEGER% = CB.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

      SB.INTEGER% = CRTN.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

      SB.INTEGER% = DIRSUP.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

      SB.INTEGER% = IDF.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       SB.FILE.UTILS
\***
\********************************************************************
\***
\***      Allocate/report/de-allocate a file session number
\***
\********************************************************************
\***
\***      Parameters : 2 or 3 (depending on action)
\***
\***         SB.ACTION$  = "O" for allocate file session number
\***                       "R" for report file session number
\***                       "C" for de-allocate file session number
\***
\***         SB.INTEGER% = file reporting number for action "O" or
\***                       file session number for actions "R" or "C"
\***
\***         SB.STRING$  = logical file name for action "O" or
\***                       null ("") for action "R" and "C"
\***
\***      Output : 1 or 2 (depending on action)
\***
\***         SB.FILE.NAME$     = logical file name for action "R"
\***
\***         SB.FILE.SESS.NUM% = file session number for action "O"
\***                             or undefined for action "C"
\***         OR
\***         SB.FILE.REP.NUM%  = file reporting number for action "R"
\***                             or undefined for action "C"
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

SB.FILE.UTILS:

   CALL SESS.NUM.UTILITY(SB.ACTION$, SB.INTEGER%, SB.STRING$)

   IF SB.ACTION$ = "O" THEN BEGIN

       SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%

   ENDIF ELSE IF SB.ACTION$ = "R" THEN BEGIN

       SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
       SB.FILE.NAME$ = F20.FILE.NAME$

   ENDIF

RETURN

\********************************************************************
\***
\***    Error Routines
\***
\********************************************************************
\***
\***    OPEN Error
\***
\********************************************************************

OPEN.ERROR:

   FILE.OPERATION$ = "O"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    READ Error
\***
\********************************************************************

READ.ERROR:

   FILE.OPERATION$ = "R"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    WRITE Error
\***
\********************************************************************

WRITE.ERROR:

   FILE.OPERATION$ = "W"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    General Error
\***
\********************************************************************

ERR.DETECTED:

   IF ERR = "NP" THEN BEGIN
      PSBCHN.OP     = "99999999"
      PSBCHN.APP    = "ADX_UPGM:PSB50.286"
      PSBCHN.MENCON = ""
      PSBCHN.U1     = ""
      PSBCHN.U2     = ""
      PSBCHN.U3     = ""
      RESUME START.PROGRAM
   ENDIF

   IF ERR = "CM"                                                    \
   OR ERR = "CT" THEN BEGIN
      MESSAGE.NUMBER% = 553
      VAR.STRING.1$   = "B5000" + CHAINING.TO.PROG$
      VAR.STRING.2$   = "PS"    + CHAINING.TO.PROG$
      EVENT.NUMBER%   = 18
      rc% = APPLICATION.LOG (MESSAGE.NUMBER%,                       \
                             VAR.STRING.1$,                         \
                             VAR.STRING.2$,                         \
                             EVENT.NUMBER%)
   ENDIF

   !  *** any other system error ***

   CALL STANDARD.ERROR.DETECTED(ERRN,                               \
                                ERRF%,                              \
                                ERRL,                               \
                                ERR)

   RESUME END.PROGRAM


END

