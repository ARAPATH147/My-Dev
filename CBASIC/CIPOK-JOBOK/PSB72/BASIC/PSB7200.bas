
\*******************************************************************************
\*******************************************************************************
\***
\***
\***        PROGRAM       : PSB72
\***        AUTHOR        : Bruce Scriver (Pseudocode)
\***                      : Stephen Kelsey (Basic Code)
\***        DATE WRITTEN  : 24th April 1986 (Pseudocode)
\***                      : 2nd May 1986    (Basic Code)
\***
\***        MODULE        : PSB7200
\***
\*******************************************************************************
\*******************************************************************************

REM pseudocode follows..

\*******************************************************************************
\*******************************************************************************
\***
\***
\***                         MODULE OVERVIEW
\***                         ---------------
\***
\***   R.J.Hopkinson     28th March 1988
\***
\***   This program was previously module 00 of PSB03 - code was altered in
\***   line with changes to menu structure required by new small store system.
\***   Additional alterations were made to use new method of chaining to other
\***   programs, i.e. via included code PSBCHNE.J86, PSBUSEE.J86 & PSBUSEG.J86.
\***
\*******************************************************************************

\*******************************************************************************
\***
\***    VERSION B.      MARK WALKER                     10th April 1991
\***    1) Redundant code has been removed from subroutines PROCESS.ENTRY
\***    and AUTO.EFFECT.DECREASES.
\***    2) Subroutine IRF.WRITE.ERROR has been removed, as this is also
\***    redundant.
\***    3) A 'Processing - Please Wait ....' message is displayed when
\***    extracting data.
\***    4) If an error routine is called and the program was called by
\***    PSB51, add the message number to the the passed parameter.
\***    5) Move the opening of the PPFI in subroutine PROCESS.SELECTION.
\***    6) Allow the HELP key to be selected from the 'Are you sure? (Y/N)'
\***    prompt.
\***    7) Add an extra parameter to a call to subroutine UPDT.IRF.UPDT.
\***    8) Use PSBF20 to allocate session numbers to files.
\***    9) Update error messages using session numbers to report numbers.
\***    10) Change the format of message 553.
\***    11) Update events to use a report number.
\***    12) Update all included code.
\***    13) Alter input field number 8 to allow function keys and bad keys
\***    to terminate input.
\***    14) Update messages to use a report number.
\***    15) Change GOTO to RESUME within ERROR.DETECTED
\***    16) Message 273 redisplayed when processing continues after an error.
\***
\***    VERSION C.      Stephen Windsor                 3rd  March  1992
\***     Update included code for PPFI/PPFO
\***     Update included code for PSBF20
\***
\***    VERSION D.      Stephen Windsor                 23rd September 1993
\***     New deals system requires changes to the way items are updated on
\***     the IRF.
\***       - Items on deal may now have their prices altered.
\***       - Items with a local price must have their HO price maintained on
\***          the LOCAL file now (not HOLDING PRICE on the IRF).
\***          If the item is not on the LOCAL file, add it.
\***     The processing of the PPFI/O is speeded up by reading and writing
\***     in large blocks.
\***     Fix to Boots problem 1062:-
\***     Price changes to items that cannot be found (on IDF or IRF), are
\***     thrown out and NOT written back to the PPFO. The read error is STILL
\***     logged. (Reason: Stores are not interested in knowing about price
\***                      changes to items NOT on their inventory. Currently
\***                      these items have to be "ESCaped" past or are listed
\***                      at the end of Store Open - causing unnecessary calls
\***                      to the Help Desk.)
\***     The program is also streamlined in this version.
\***
\***    VERSION E.      Stephen Windsor                          4th  March    1994
\***    Swop the positions of delete PPFI and close PPFO.
\***    Reason : When utilizing the large buffsize during I/O functions the
\***    remaining data in the buffer only gets written to the file at the
\***    CLOSE statement. If the controller IPL's between the delete and close
\***    the data within the buffer is lost.
\***
\***
\***    Version F                Andrew Wedgeworth        27th September 1995
\***
\***    Changes for new Customer Card system.  The redemption points value on
\***    the IRF is updated when an item's price changes.
\***
\***    Version G                Scott Baker              13th January 1998
\***
\***    Changes required for y2k compliance. Also removed code pertaining to
\***    'unconverted records' and replaced IRF.MPGROUP with IRF.INDICAT5.
\***
\***    Version H           Stuart William McConnachie         2nd March 2000
\***    Updated IRF fields for meal deal.
\***
\***    Version I           Brian Greenfield                   3rd October 2002
\***    Changes made to accomodate new IRF layout for Deals rewrite.
\***
\***    Version J            Jamie Thorpe                      24th October 2006
\***    Emergency Price Changes:
\***    Added code so that Emergency Price change RPD's (denoted by EPC on the 
\***    LOCAL file) are removed from Local Pricing.
\***
\***    REVISION 1.19.                ROBERT COWEY.                02 APR 2009.
\***    Corrected error handling to display message B513 when PPFI access.
\***    conflict occurs on entry to initial menu screen B7201.
\***
\***    Separated file open and close statements into distinct sub-routines 
\***    and called these only as needed to keep files closed when not in use.
\***    Nullified PPFI.OPEN.FLAG$ as this is now redundant.
\***    Nullified other variables identified by compiler as redundant.
\***
\***    REVISION 1.20.                ROBERT COWEY.                21 JUN 2010.
\***    Fix for OM error 00000058 on 19,870 item PPFI and other changes for 
\***    "VAT Preparation Jan 2011" project creating PSB72.286 Rv 1.20.
\***    Catered for NP error to allow program launch from command line.
\***    Loaded PPFI directly into F14.TABLE$ to avoid appending data into 
\***    FIRST.DATA.STRING$ (which eventually fails when somewhere above 32k).
\***    Also loaded F14.TABLE$ data directly into DISPLAY.TABLE$ to avoid using
\***    SECOND.DATA.STRING$ in a similar way.
\***    Set both table limits to 14,400 and used this for other internal limits.
\***    Ensured BEMF message 265 continues to be displayed on initial entry to
\***    RPD selection screen.
\***
\*******************************************************************************


\*******************************************************************************
\***
\***   %INCLUDE globals for DISPLAY.MANAGER functions
\***   %INCLUDE globals for READ.NEXT.IEF function
\***   %INCLUDE globals for SORT.TABLE function
\***   %INCLUDE globals for CONV.TO.HEX function
\***   %INCLUDE globals for CONV.TO.STRING function
\***   %INCLUDE globals for IRF.UPDATE function
\***   %INCLUDE globals for screen chaining parameters
\***   %INCLUDE field definitions for IDF
\***   %INCLUDE field definitions for IEF
\***   %INCLUDE field definitions for IRF
\***   %INCLUDE field definitions for PPF
\***   %INCLUDE session number definitions for IDF
\***   %INCLUDE session number definitions for IEF
\***   %INCLUDE session number definitions for IRF
\***   %INCLUDE session number definitions for PPFI/O
\***
\...............................................................................

   %INCLUDE PSBF02G.J86                                                ! FAW
   %INCLUDE PSBF03G.J86                                                ! FAW
   %INCLUDE PSBF11G.J86                                                ! FAW
   %INCLUDE PSBF14G.J86                                                ! FAW
   %INCLUDE PSBF16G.J86                                                ! FAW
   %INCLUDE PSBF17G.J86                                                ! FAW
   %INCLUDE PSBF18G.J86                                                ! FAW
   %INCLUDE PSBF19G.J86                                                ! FAW
   %INCLUDE PSBF20G.J86                                                ! FAW
   %INCLUDE PSBUSEG.J86

   %INCLUDE IDFDEC.J86                                                 ! FAW
   %INCLUDE IEFDEC.J86                                                 ! FAW
   %INCLUDE IRFDEC.J86                                                 ! FAW
   %INCLUDE PPFDEC.J86                                                 ! FAW
   %INCLUDE LOCALDEC.J86                                               ! FAW

    STRING                                                             \
          ACD.FLAG$,                                                   \
          B7201.SCREEN.NO$,                                            \
          B7203.SCREEN.NO$,                                            \
          BAR.CODE.FRONT$,                                             \
\         CALLING.PROG.NO$,                                            \ ! 1.19 RC
          CURRENT.BOOTS.CODE$,                                         \
          CURRENT.IEF.BAR.CODE$,                                       \
          CURRENT.KEY$,                                                \
\         CURRENT.SCREEN$,                                             \ ! 1.19 RC
          DEAL.PRICE$,                                                 \
          DECREASES.RUN.ALREADY.FLAG$,                                 \
          DECREASE.PERFORMED.$,                                        \
          DISP.2.TABLE$(1),                                            \
          DISPLAY.TABLE$(1),                                           \
          DISP.TABLE.FLAG$,                                            \
          EFFECT.FLAG$,                                                \
          END.LOOP$,                                                   \
          END.OF.FILE$,                                                \
          ERRL.STRING$,                                                \
          ERRN.STRING$,                                                \
          FAIL.TYPE$,                                                  \
\         FIRST.DATA.STRING$,                                          \ 1.20 RC
          FIRST.EAN$,                                                  \
          IDF.BAR.CODE$,                                               \
          IDF.GROUP.EAN.CODE$,                                         \
          INC.DEC$,                                                    \
\         INDICAT2$,                                                   \ HSWM
          IRF.LOCKED.FLAG$,                                            \ BMW
          KEY$,                                                        \ BMW
          LOCAL.PRICE.FLAG$,                                           \
          MATCH.FOUND$,                                                \
          MOVE.CURSOR$,                                                \
\         PPFI.OPEN.FLAG$,                                             \ BMW ! 1.19 RC
          PPFI.TRAILER.ERROR$,                                         \
          PPFO.THERE.FLAG$,                                            \
          QUIT.FLAG$,                                                  \
          QUIT.PRESSED$,                                               \
          RESF.CALLED$,                                                \
\         SALEQUAN$,                                                   \ HSWM
          SAVED.DISP.FLAG$,                                            \
          SAVED.DISP.MESSAGE$,                                         \
          SAVED.ENTRY$,                                                \
          SAVED.IDF.BOOTS.CODE$,                                       \
\         SAVED.INDICAT2$,                                             \ HSWM
          SAVED.PRICE$,                                                \
\         SAVED.SALEQUAN$,                                             \ IBG SAVED.SALEQUAN$ REMOVED
          SB.ACTION$,                                                  \ BMW
          SB.STRING$,                                                  \ BMW
          SB.FILE.NAME$,                                               \ BMW
\         SECOND.DATA.STRING$,                                         \ 1.20 RC
          SELECTION$,                                                  \
          STRING.DATA$,                                                \
          TABLE.DATE$,                                                 \
          UNIQUE$,                                                     \
          UPDATE.FAILED$,                                              \
          VARIABLE.STRING$,                                            \
          VAR.STRING.1$,                                               \
          VAR.STRING.2$,                                               \
          ZERO.ENTRY.FLAG$

STRING GLOBAL BOOTS.CODE.SIX.DIGIT$, BOOTS.CODE.SEVEN.DIGIT$,          \DSJW
              SAVED.DEAL.NUM$(1),                                      \DSJW IBG SAVED.DEAL.SAVING$ REMOVED
              CURRENT.DATE$, BATCH.SCREEN.FLAG$, OPERATOR.NUMBER$,     \DSJW
              MODULE.NUMBER$, DATE.NEXT.WEEK$                          !DSJW

INTEGER*1 GLOBAL TRUE, FALSE, LOCAL.FILE.OPEN                          !DSJW

    INTEGER*1                                                          \
          BIT.7.MASK%,                                                 \
          EVENT.NO%,                                                   \
          GROUP.CODE.FLAG%,                                            \
          MSGGRP%,                                                     \
          SEVERITY%, SAVED.INDICAT3%, FORTNIGHT%,                      \DSJW  IBG SAVED.INDICAT4% REMOVED
\         SAVED.INDICAT2%,                                             \HSWM IBG SAVED.INDICAT2% REMOVED
          SAVED.INDICAT5%,                                             \GSB
          SAVED.LIST.ID%(1)                                            !IBG 

    INTEGER*2                                                          \
          CURRENT.ENTRY%,                                              \
          CURRENT.FIELD%,                                              \
          CURRENT.PAGE%,                                               \
          CURRENT.SESS.NUM%,                                           \
          DISPLAY.ENTRY%,                                              \
\         DISPLAY.INDEX%,                                              \ ! 1.19 RC
          END.KEY%,                                                    \
          ENTER%,                                                      \
          ESCAPE%,                                                     \
          EXIT.KEY%,                                                   \ BMW
          HELP%,                                                       \
          HOME%,                                                       \
          INDEX%,                                                      \
          INTEGER.DATA%,                                               \
          LINES%,                                                      \
          KEY.PRESSED%,                                                \
          LAST.INPUT.FIELD.NO%,                                        \
          MESSAGE.NO%,                                                 \
          MSGNUM%,                                                     \
          NEXTF.PARM%,                                                 \
          NUM.ON.PAGE%,                                                \
          PAGE.DOWN%,                                                  \
          PAGE.UP%,                                                    \
          QUIT%,                                                       \
          RENAME.RET.CODE%,                                            \
          RETURN.FIELD%,                                               \
          SB.INTEGER%,                                                 \ BMW
          SB.FILE.SESS.NUM%,                                           \ BMW
          SB.FILE.REP.NUM%,                                            \ BMW
          TAB%,                                                        \
          TERM%

INTEGER*2 GLOBAL PPFO.BUFF.SIZE%, RC%, PPFI.BUFF.SIZE%, I%, SUB%        !DSJW

    INTEGER*4                                                          \
          COUNT%,                                                      \
          DISPLAY.COUNT%,                                              \
          ENTRY.COUNT%,                                                \
          INTEGER.4%,                                                  \
          PPFI.RECORD.COUNT%,                                          \
          PPFI.TRAILER.COUNT%,                                         \
          PPFO.RECORD.COUNT%,                                          \
\         REDEEM.POINTS%,                                              \ FAW ! 1.19 RC
          RPD.NUMBER%,                                                 \
          TABLE.LIMIT.14K%,                                            \ 1.20 RC
          TOTAL.BAR.CODE.COUNT%,                                       \
          TOTAL.ENTRIES%,                                              \
          TOTAL.PAGES%

\...............................................................................
\***
\***   %INCLUDE external definition of ADXERROR function
\***   %INCLUDE external definition of APPLICATION.LOG function
\***   %INCLUDE external definition of DISPLAY.MANAGER functions
\***   %INCLUDE external definition of EXTERNAL.MESSAGE function
\***   %INCLUDE external definition of READ.NEXT.IEF function
\***   %INCLUDE external definition of HELP function
\***   %INCLUDE external definition of SORT.TABLE function
\***   %INCLUDE external definition of CONV.TO.HEX function
\***   %INCLUDE external definition of CONV.TO.STRING function
\***   %INCLUDE external definition of IRF.UPDATE function
\***   %INCLUDE i/o function definitions of IDF
\***   %INCLUDE i/o function definitions of IEF
\***   %INCLUDE i/o function definitions of IRF
\***   %INCLUDE i/o function definitions of PPFI
\***   %INCLUDE i/o function definitions of PPFO
\***
\...............................................................................

   %INCLUDE ADXERROR.J86                                               ! FAW
   %INCLUDE PSBF01E.J86                                                ! FAW
   %INCLUDE PSBF02E.J86                                                ! FAW
   %INCLUDE PSBF03E.J86                                                ! FAW
   %INCLUDE PSBF04E.J86                                                ! FAW
   %INCLUDE PSBF11E.J86                                                ! FAW
   %INCLUDE PSBF12E.J86                                                ! FAW
   %INCLUDE PSBF14E.J86                                                ! FAW
   %INCLUDE PSBF16E.J86                                                ! FAW
   %INCLUDE PSBF17E.J86                                                ! FAW
   %INCLUDE PSBF18E.J86                                                ! FAW
   %INCLUDE PSBF19E.J86                                                ! FAW
   %INCLUDE PSBF20E.J86                                                ! FAW
   %INCLUDE PSBF24E.J86                                                ! FAW

   %INCLUDE IDFEXT.J86                                                 ! FAW
   %INCLUDE IEFEXT.J86                                                 ! FAW
   %INCLUDE IRFEXT.J86                                                 ! FAW
   %INCLUDE PPFEXT.J86                                                 ! FAW
   %INCLUDE LOCALEXT.J86                                               ! FAW

\*******************************************************************************
\***
\***   ON ERROR GOTO ERROR.DETECTED
\***
\***   set unique to ten spaces
\***   set PPFO there flag off
\***   set string errl to 6 spaces
\***   set string errn to 8 spaces
\***
\***   %INCLUDE file initialisations for IDF
\***   %INCLUDE file initialisations for IEF
\***   %INCLUDE file initialisations for IRF
\***   %INCLUDE file initialisations for PPFI
\***   %INCLUDE file initialisations for PPFO
\***   %INCLUDE file initialisations for LOCAL
\***
\***   set key values for input key comparisons:
\***   Enter = 0, F1 = -1, F3 = -3, ESC = 27, Tab = 9, Home = 327, End = 335,
\***   PgUp = 329, PgDn = 337
\***
\***   execute USE function to obtain operator number and passed program number
\***
\***   Reset successful completion flag for return to store opening
\***
\***   set operator number to psbchn op
\***
\***   Allocate all file session numbers
\***
\***   IF not called from STORE OPEN then initialise DISPLAY MGR
\***
\***   IF END occurs on open idf THEN OPEN.IDF.ERROR
\***   OPEN idf NOWRITE NODEL
\***
\***   IF END occurs on open ief THEN OPEN.IEF.ERROR
\***   OPEN ief NOWRITE NODEL
\***
\***   IF END occurs on open irf THEN OPEN.IRF.ERROR
\***   OPEN irf NODEL
\***   CALL OPEN.IRF.UPDT
\***
\***   CALL OPEN.PPFI
\***
\***   IF called from Store Opening  THEN
\***      set selection to "2" (Decreases)
\***      GOSUB PROCESS.SELECTION
\***      Set successful completion flag for return
\***      GOTO  END.OF.PROGRAM
\***   endif
\***
\***   Set default option to "1"  (Increases)
\***   GOSUB PUT.SELECTION
\***
\...............................................................................

   ON ERROR GOTO ERROR.DETECTED

   UNIQUE$ = "          "
   ERRL.STRING$ = "      "
   ERRN.STRING$ = "        "
   PPFO.THERE.FLAG$ = "N"

  FALSE EQ 0                                                            !DSJW
  TRUE  EQ -1                                                           !DSJW

  PPFI.BUFF.SIZE%   EQ 32256                                            !DSJW
  PPFO.BUFF.SIZE%   EQ 32256                                            !DSJW
  
  DIM SAVED.DEAL.NUM$(3)                                                !IBG
  DIM SAVED.LIST.ID%(3)                                                 !IBG

  RC% EQ IDF.SET                                                        !DSJW
  RC% EQ IRF.SET                                                        !DSJW
  RC% EQ IEF.SET                                                        !DSJW
  RC% EQ PPFI.SET                                                       !DSJW
  RC% EQ PPFO.SET                                                       !DSJW
  RC% EQ LOCAL.SET                                                      !DSJW
  
  CREATE POSFILE "C:\PSB7200.OUT" AS 992

   BATCH.SCREEN.FLAG$ = "S"
   MODULE.NUMBER$ = "PSB7200"
   PPFI.TRAILER.ERROR$ = "N"
   B7201.SCREEN.NO$ = "B7201"
   B7203.SCREEN.NO$ = "B7203"
   TOTAL.ENTRIES% = 0
   ENTRY.COUNT% = 0
   PPFI.RECORD.COUNT% = 0
   TOTAL.PAGES% = 0
   BIT.7.MASK% = 10000000B
   IRF.LOCKED.FLAG$ = "Y"                                              ! BMW
!  PPFI.OPEN.FLAG$ = "N"                                               ! BMW ! 1.19 RC

   QUIT% = -3
   HELP% = -1
   ENTER% = 0
   TAB% = 9
   ESCAPE% = 27
   HOME% = 327
   PAGE.UP% = 329
   END.KEY% = 335
   PAGE.DOWN% = 337

   TABLE.LIMIT.14K% = 14400 ! Slightly below 4690 maximum of 14464     ! 1.20 RC

   %INCLUDE PSBUSEE.J86                                                ! FAW

RESUME.FROM.NP.ERROR:                                                  ! 1.20 RC

   PSBCHN.U2 = ""

   OPERATOR.NUMBER$ = PSBCHN.OP

   GOSUB ALLOCATE.SESS.NUMS                                            ! BMW

   IF PSBCHN.APP NE "PSB51" THEN BEGIN
      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.INITDM (STRING.DATA$,INTEGER.DATA%)                    !DSJW
      IF RC% NE 0 THEN BEGIN
         GOTO CHAIN.OUT
      ENDIF
   ENDIF

   GOSUB OPEN.ITEM.FILES  ! Files opened on initial entry to menu      ! 1.19 RC
   GOSUB CLOSE.ITEM.FILES ! selection screen to confirm availability   ! 1.19 RC
   CLOSE PPFI.SESS.NUM%   ! and then closed until needed               ! 1.19 RC

   IF PSBCHN.APP = "PSB51" THEN BEGIN
      SELECTION$ = "2"
      GOSUB PROCESS.SELECTION
      GOTO  END.OF.PROGRAM
   ENDIF

   SELECTION$ = "1"
   GOSUB PUT.SELECTION

\...............................................................................
\***
\***   set key pressed to 999
\***   set resf called flag off
\***
\***   WHILE key pressed is not F3 or ESC
\***
\***      IF resf called flag is off THEN
\***         set string data to spaces
\***         set integer data to 5
\***         CALL DM.POSF function to position the cursor in field 5
\***         IF F03.RETURN.CODE% = 0 THEN
\***            set string data to spaces
\***            set integer data to 0
\***            CALL DM.UPDF function to get user input
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            endif
\***         ELSE
\***            GOTO CHAIN.OUT
\***         endif
\***      endif
\***
\***      set key pressed to F03.RETURNED.INTEGER%
\***      set selection to F03.RETURNED.STRING$
\***
\***      set resf called flag off
\***
\***      on case of
\***         key pressed is Enter
\***            GOSUB PROCESS.SELECTION
\***         key pressed is F1
\***            GOSUB HELP.ON.7201
\***         key pressed is not Enter, F1, F3 or ESC
\***            IF key pressed is in the range 32 to 127 ascii THEN
\***               set resf called flag on
\***               set string data to spaces
\***               set integer data to 0
\***               CALL DM.RESF function to indicate invalid entry
\***               IF F03.RETURN.CODE% <> 0 THEN
\***                  GOTO CHAIN.OUT
\***               ELSE
\***               endif
\***            ELSE
\***               CALL EXTERNAL.MESSAGE function to display message 001,
\***                                                 return to field 5
\***               IF F04.RETURN.CODE% <> 0 THEN
\***                  GOTO CHAIN.OUT
\***               endif
\***            endif
\***      endcase
\***
\***   WEND
\***
\*** END.OF.PROGRAM:
\***
\***   CLOSE idf
\***
\***   CLOSE ief
\***
\***   CLOSE irf
\***   CLOSE LOCAL if open
\***   CALL CLOSE.IRF.UPDT
\***
\***   Deallocate all file session numbers
\***
\***   CHAIN.OUT:
\***
\***      IF PPFO.THERE.FLAG$ = Y THEN
\***         set PPFO.THERE.FLAG off
\***         CLOSE PPFO file
\***
\***      CHAIN back to calling program
\***
\***
\...............................................................................

   KEY.PRESSED% = 999
   RESF.CALLED$ = "N"

   WHILE KEY.PRESSED% <> QUIT% AND KEY.PRESSED% <> ESCAPE%

      IF RESF.CALLED$ = "N" THEN BEGIN
         STRING.DATA$ = ""
         INTEGER.DATA% = 5
         RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)
         IF RC% EQ 0 THEN BEGIN
            STRING.DATA$ = ""
            INTEGER.DATA% = 0
            RC% EQ DM.UPDF (STRING.DATA$,INTEGER.DATA%)
            IF RC% NE 0 THEN BEGIN
               GOTO CHAIN.OUT
            ENDIF
         ENDIF ELSE BEGIN
            GOTO CHAIN.OUT
         ENDIF
      ENDIF

      KEY.PRESSED% = F03.RETURNED.INTEGER%
      SELECTION$ = F03.RETURNED.STRING$

      RESF.CALLED$ = "N"

      IF KEY.PRESSED% = ENTER% THEN                                    \
         GOSUB PROCESS.SELECTION                                      :\
      ELSE                                                             \
         IF KEY.PRESSED% = HELP% THEN                                  \
            GOSUB HELP.ON.7201                                        :\
         ELSE                                                          \
            IF KEY.PRESSED% <> QUIT% AND KEY.PRESSED% <> ESCAPE% THEN  \
               IF KEY.PRESSED% > 31 AND KEY.PRESSED% < 128 THEN        \
                  RESF.CALLED$ = "Y"                                  :\
                  STRING.DATA$ = ""                                   :\
                  INTEGER.DATA% = 0                                   :\
                  RC% EQ DM.RESF (STRING.DATA$,INTEGER.DATA%)         :\
                  IF RC% NE 0 THEN                                     \
                     GOTO CHAIN.OUT                                   :\
                  ELSE                                                 \
               ELSE                                                    \
                  VARIABLE.STRING$ = ""                               :\
                  MESSAGE.NO% = 1                                     :\
                  RETURN.FIELD% = 5                                   :\
                  RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                \
                                           VARIABLE.STRING$,           \
                                           RETURN.FIELD%)             :\
                  IF RC% NE 0 THEN                                     \
                     GOTO CHAIN.OUT

   WEND

END.OF.PROGRAM:

!  File CLOSE statements moved to separate subroutine                  ! 1.19 RC
   
   GOSUB DEALLOCATE.SESS.NUMS                                          ! BMW

   CHAIN.OUT:

   IF PPFO.THERE.FLAG$ = "Y" THEN                                      \
      PPFO.THERE.FLAG$ = "N"                                          :\
      CLOSE PPFO.SESS.NUM%

   IF KEY.PRESSED% = ESCAPE% THEN                                      \
      GOTO ESCAPE.TO.MAIN.MENU

   PSBCHN.PRG = "ADX_UPGM:" + PSBCHN.APP + ".286"
   PSBCHN.APP = "PSB72"
   %INCLUDE PSBCHNE.J86                                                ! FAW


\*******************************************************************************
\********************** subroutines follow *************************************
\*******************************************************************************


\*******************************************************************************
\***
\***   OPEN.ITEM.FILES:
\***   Opens item files including IRF update function PSBF19.
\***   Open of PPFI performed within separate sub-routine.
\***
\***............................................................................


OPEN.ITEM.FILES: ! New sub-routine containing existing and unchanged   ! 1.19 RC
                 ! file open commands (except PPFI now opened first)   ! 1.19 RC

   GOSUB OPEN.PPFI.LOCKED                                              ! 1.19 RC
   
   IF END #IDF.SESS.NUM% THEN OPEN.IDF.ERROR
   OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%           \
                       NOWRITE NODEL

   IF END #IEF.SESS.NUM% THEN OPEN.IEF.ERROR
   OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%           \
                       NOWRITE NODEL

   LOCAL.FILE.OPEN EQ FALSE                                             !DSJW
   IF END #LOCAL.SESS.NUM% THEN OPEN.LOCAL.ERROR                        !DSJW
   OPEN LOCAL.FILE.NAME$ KEYED RECL LOCAL.RECL% AS LOCAL.SESS.NUM%      !DSJW
   LOCAL.FILE.OPEN EQ TRUE                                              !DSJW

   END.OPEN.LOCAL.FILE:                                                 !DSJW

   IF END #IRF.SESS.NUM% THEN OPEN.IRF.ERROR
   OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM% NODEL
   
   RC% EQ OPEN.IRF.UPDT (NEW.IRF.DATA$,ACD.FLAG$)
   IF RC% NE 0 THEN BEGIN
      GOTO CHAIN.OUT
   ENDIF

RETURN                                                                 ! 1.19 RC


\*******************************************************************************
\***
\***   OPEN.PPFI.LOCKED:
\***   Opens PPFI locked.
\***
\***............................................................................


OPEN.PPFI.LOCKED: ! New sub-routine containing existing and unchanged  ! 1.19 RC
                  ! PPFI open command                                  ! 1.19 RC

   IF END # PPFI.SESS.NUM% THEN OPEN.PPFI.ERROR                         !DSJW
   OPEN PPFI.FILE.NAME$ AS PPFI.SESS.NUM% BUFFSIZE PPFI.BUFF.SIZE% \    !DSJW
                                          LOCKED NOWRITE                !DSJW
!  PPFI.OPEN.FLAG$ = "Y"                                                !DSJW ! 1.19 RC

RETURN


\*******************************************************************************
\***
\***   CLOSE.ITEM.FILES:
\***   Closes item files including IRF update function PSBF19.
\***   Does not close PPFI as this is done within main code.
\***
\***............................................................................


CLOSE.ITEM.FILES: ! New sub-routine containing existing and unchanged  ! 1.19 RC
                  ! file close commands (excluding PPFI close)         ! 1.19 RC

   IF LOCAL.FILE.OPEN THEN \                                            !DSJW
      CLOSE LOCAL.SESS.NUM%                                             !DSJW

   CLOSE IDF.SESS.NUM%, \                                               !DSJW
         IEF.SESS.NUM%, \                                               !DSJW
         IRF.SESS.NUM%

   RC% EQ CLOSE.IRF.UPDT (NEW.IRF.DATA$,ACD.FLAG$)                      !DSJW
   IF RC% NE 0 THEN \                                                   !DSJW
      GOTO CHAIN.OUT

RETURN                                                                 ! 1.19 RC


\*******************************************************************************
\***
\***   OPEN.IDF.ERROR:
\***
\***   set fail type to "O"
\***   set current session number to idf session number
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   OPEN.IDF.ERROR:

      FAIL.TYPE$ = "O"
      CURRENT.SESS.NUM% = IDF.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*******************************************************************************
\***
\***   OPEN.IEF.ERROR:
\***
\***   set fail type to "O"
\***   set current session number to ief session number
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   OPEN.IEF.ERROR:

      FAIL.TYPE$ = "O"
      CURRENT.SESS.NUM% = IEF.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*******************************************************************************
\***   OPEN.LOCAL.ERROR
\***   set fail type to "O"
\***   set current session number to irf session number
\***   set key to null
\***   GOSUB FILE.ERROR
\***   GOTO END.OPEN.LOCAL.FILE (INITIALISATION)
\...............................................................................

   OPEN.LOCAL.ERROR:

      FAIL.TYPE$ = "O"
      CURRENT.SESS.NUM% = LOCAL.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO END.OPEN.LOCAL.FILE

\*******************************************************************************
\***
\***   OPEN.IRF.ERROR:
\***
\***   set fail type to "O"
\***   set current session number to irf session number
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   OPEN.IRF.ERROR:

      FAIL.TYPE$ = "O"
      CURRENT.SESS.NUM% = IRF.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*******************************************************************************
\***
\***   PROCESS.SELECTION:
\***
\***   IF selection is "2" THEN
\***      set increase/decrease flag to "D"
\***   ELSE
\***      IF selection is "1" THEN
\***         set increase/decrease flag to "I"
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message 003,
\***                                           return to field 5
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            RETURN
\***         endif
\***      endif
\***   endif
\***
\***   CHECK.ANSWER:
\***
\***   IF increase/decrease flag is "I" AND
\***      DECREASES RUN ALREADY FLAG IS "Y" then
\***      OPEN the ppfi LOCKED NOWRITE
\***
\***   IF increase/decrease flag is "D" AND
\***      not called from store opening  THEN
\***      CALL DM.POSF function to position the cursor in field number 7
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         set string data to "Are you sure? (Y/N)"
\***         set integer data to 0
\***         CALL DM.PUTF to put message in field 7
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            CALL DM.POSF function to position the cursor in field 8
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               GOSUB CONFIRM.CHOICE
\***
\***   IF not called from store opening  THEN
\***      IF increase/decrease flag is "D" THEN
\***         set string data to "B7202"
\***         set integer data to 0
\***         CALL DM.DISPD function to display required screen
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***      ELSE
\***         set string data to "B7203"
\***         set integer data to 0
\***         CALL DM.DISPD function to display required screen
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         endif
\***      endif
\***   endif
\***
\***   IF increase/decrease flag = "D" THEN
\***      GOSUB AUTO.EFFECT.DECREASES
\***      IF not called from store opening  THEN
\***         GOTO PROCESS.RETURN
\***      ELSE
\***         GOTO CHAIN.OUT
\***      endif
\***   endif
\***
\***
\***   GOSUB GET.DATA
\***
\...............................................................................

   PROCESS.SELECTION:

      IF SELECTION$ = "2" THEN                                         \
         INC.DEC$ = "D"                                               :\
      ELSE                                                             \
         IF SELECTION$ = "1" THEN                                      \
            INC.DEC$ = "I"                                            :\
         ELSE                                                          \
            VARIABLE.STRING$ = ""                                     :\
            MESSAGE.NO% = 3                                           :\
            RETURN.FIELD% = 5                                         :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \
                                     VARIABLE.STRING$,                 \
                                     RETURN.FIELD%)                   :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               RETURN

      IF INC.DEC$ = "I" AND                                            \
         DECREASES.RUN.ALREADY.FLAG$ = "Y" THEN                        \
         DECREASE.PERFORMED.$ = "N"

      ! 2 lines deleted from here !!!!!                                ! BMW

      IF INC.DEC$ = "D" AND                                            \
         PSBCHN.APP <> "PSB51"  AND                                    \
         DECREASES.RUN.ALREADY.FLAG$ = "Y" THEN                        \
         DECREASE.PERFORMED.$ = "N"                                   :\
         VARIABLE.STRING$ = ""                                        :\
         MESSAGE.NO% = 277                                            :\
         RETURN.FIELD% = 5                                            :\
         RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                         \
                                  VARIABLE.STRING$,                    \
                                  RETURN.FIELD%)                      :\
         IF RC% NE 0 THEN                                              \
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.UPDF (STRING.DATA$,INTEGER.DATA%)               :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               GOTO PROCESS.RETURN

      IF INC.DEC$ = "D" AND                                            \
         PSBCHN.APP <> "PSB51"  THEN                                   \
         STRING.DATA$ = ""                                            :\
         INTEGER.DATA% = 7                                            :\
         RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)                  :\
         IF RC% NE 0 THEN                                              \
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            STRING.DATA$ = "Are you sure? (Y/N)"                      :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.PUTF (STRING.DATA$,INTEGER.DATA%)               :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               STRING.DATA$ = ""                                      :\
               INTEGER.DATA% = 8                                      :\
               RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)            :\
               IF RC% NE 0 THEN                                        \
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
               BEGIN                                                   ! BMW
                  GOSUB CONFIRM.CHOICE                                 ! BMW
                  IF KEY$ = "N" THEN                                   \ BMW
                  BEGIN                                                ! BMW
                     DECREASE.PERFORMED.$ = "N"                        ! BMW
                     GOTO PROCESS.RETURN                               ! BMW
                  ENDIF                                                ! BMW
               ENDIF                                                   ! BMW

      IF PSBCHN.APP <> "PSB51"  THEN                                   \
         IF INC.DEC$ = "D" THEN                                        \
            STRING.DATA$ = "B7202"                                    :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.DISPD (STRING.DATA$,INTEGER.DATA%)              :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
         ELSE                                                          \
            STRING.DATA$ = "B7203"                                    :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.DISPD (STRING.DATA$,INTEGER.DATA%)              :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT

      IF INC.DEC$ = "D" THEN                                           \
      BEGIN                                                            ! BMW
         GOSUB AUTO.EFFECT.DECREASES

!        Lines deleted - PPFI close and reopen                         ! 1.19 RC

         IF PSBCHN.APP <> "PSB51"  THEN                                \
            GOTO PROCESS.RETURN                                        \
         ELSE                                                          \
            GOTO CHAIN.OUT
      ENDIF                                                            ! BMW

      GOSUB GET.DATA

\...............................................................................
\***
\***   set key pressed to 999
\***   set nextf parameter to -20 (first input field)
\***   set move cursor flag on
\***   set quit flag off
\***   set zero entry flag off
\***   set effect flag off
\***
\***   WHILE key pressed is not ESC
\***   AND quit flag is off
\***
\***      IF total entries is 0
\***      AND effect flag is off THEN
\***         GOSUB NO.ENTRIES.MESSAGE
\***         set zero entry flag on
\***         set nextf parameter to 20 (last input field)
\***         set move cursor flag on
\***      endif
\***
\***      IF move cursor flag is on THEN
\***         set string data to spaces
\***         set integer data to nextf parameter
\***         CALL DM.NEXTF function to position the cursor accordingly
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         endif
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 0
\***      CALL DM.UPDF function to get user input
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set key pressed to F03.RETURNED.INTEGER%
\***
\***      on case of
\***         key pressed is F3 and total entries = 0
\***           set quit flag to "Y"
\***         key pressed is F3, Enter, Tab, Home, End, PgUp or PgDn
\***            GOSUB PROCESS.ENTRY
\***         key pressed is F1
\***            GOSUB HELP.ON.7203
\***         key pressed is not Enter,PgUp,PgDn,F1,Tab,Home,End,F3 or ESC and
\***            zero entry flag is off
\***            set move cursor flag off
\***            set string data to spaces
\***            set integer data to 0 (current field)
\***            CALL DM.POSF function to retrieve the current field
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               CALL EXTERNAL.MESSAGE function to display message 001,
\***                                              return to current field
\***               IF F04.RETURN.CODE% <> 0 THEN
\***                  GOTO CHAIN.OUT
\***               endif
\***            endif
\***      endcase
\***
\***   WEND
\***
\...............................................................................

      KEY.PRESSED% = 999
      NEXTF.PARM% = -20
      MOVE.CURSOR$ = "Y"
      QUIT.FLAG$ = "N"
      QUIT.PRESSED$ = "N"
      ZERO.ENTRY.FLAG$ = "N"
      EFFECT.FLAG$ = "N"

      WHILE KEY.PRESSED% <> ESCAPE% AND QUIT.FLAG$ = "N"

         IF TOTAL.ENTRIES% = 0                                         \
         AND EFFECT.FLAG$ = "N" THEN                                   \
            GOSUB NO.ENTRIES.MESSAGE                                  :\
            ZERO.ENTRY.FLAG$ = "Y"                                    :\
            NEXTF.PARM% = 20                                          :\
            MOVE.CURSOR$ = "Y"

         IF MOVE.CURSOR$ = "Y" THEN                                    \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = NEXTF.PARM%                               :\
            RC% EQ DM.NEXTF (STRING.DATA$,INTEGER.DATA%)              :\
            IF RC% NE 0 THEN                                           \
               GOTO CHAIN.OUT

         STRING.DATA$ = ""
         INTEGER.DATA% = 0
         RC% EQ DM.UPDF (STRING.DATA$,INTEGER.DATA%)
         IF RC% NE 0 THEN                                              \
            GOTO CHAIN.OUT

         KEY.PRESSED% = F03.RETURNED.INTEGER%

         IF KEY.PRESSED% = QUIT% AND TOTAL.ENTRIES% = 0 THEN           \
            QUIT.FLAG$ = "Y"                                          :\
         ELSE                                                          \
            IF KEY.PRESSED% = QUIT% OR                                 \
               KEY.PRESSED% = ENTER% OR                                \
               KEY.PRESSED% = TAB% OR                                  \
               KEY.PRESSED% = HOME% OR                                 \
               KEY.PRESSED% = END.KEY% OR                              \
               KEY.PRESSED% = PAGE.UP% OR                              \
               KEY.PRESSED% = PAGE.DOWN% THEN                         :\
               GOSUB PROCESS.ENTRY                                    :\
            ELSE                                                       \
               IF KEY.PRESSED% = HELP% THEN                            \
                  GOSUB HELP.ON.7203                                  :\
               ELSE                                                    \
                  IF KEY.PRESSED% <> ESCAPE% AND                       \
                     ZERO.ENTRY.FLAG$ = "N" THEN                       \
                     MOVE.CURSOR$ = "N"                               :\
                     STRING.DATA$ = ""                                :\
                     INTEGER.DATA% = 0                                :\
                     RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)      :\
                     IF RC% NE 0 THEN                                  \
                        GOTO CHAIN.OUT                                :\
                     ELSE                                              \
                        MESSAGE.NO% = 1                               :\
                        VARIABLE.STRING$ = ""                         :\
                        RETURN.FIELD% = F03.RETURNED.INTEGER%         :\
                        RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,          \
                                                 VARIABLE.STRING$,     \
                                                 RETURN.FIELD%)       :\
                        IF RC% NE 0 THEN                               \
                           GOTO CHAIN.OUT

      WEND

\...............................................................................
\***
\***   IF key pressed is ESC THEN
\***      CHAIN to "ADX_UPGM:PSB50.286"
\***   endif
\***
\***   CLOSE the ppfi
\***   IF END occurs on open ppfi THEN OPEN.PPFI.ERROR
\***   OPEN ppfi LOCKED NOWRITE
\***
\***   PROCESS.RETURN:
\***
\***   set string data to "B7201"
\***   set integer data to 0
\***   CALL DM.DISPD function to display the screen
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   IF increase/decrease flag is "I" THEN
\***      GOSUB PUT.SELECTION
\***   endif
\***
\***   set key pressed to 999
\***
\***   RETURN
\***
\...............................................................................

      IF KEY.PRESSED% = ESCAPE% AND                                    \
         PPFO.THERE.FLAG$ = "Y" THEN                                   \
         PPFO.THERE.FLAG$ = "N"                                       :\
         CLOSE PPFO.SESS.NUM%

      IF KEY.PRESSED% = ESCAPE% THEN                                   \
         GOTO ESCAPE.TO.MAIN.MENU

!     Lines deleted - PPFI close and re-open                           ! 1.19 RC

   PROCESS.RETURN:

      STRING.DATA$ = "B7201"
      INTEGER.DATA% = 0
      RC% EQ DM.DISPD (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      IF INC.DEC$ = "I" THEN                                           \
         GOSUB PUT.SELECTION

      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      IF DECREASE.PERFORMED.$ = "Y" THEN                                 \
        VARIABLE.STRING$ = ""                                           :\
        MESSAGE.NO% = 276                                               :\
        RETURN.FIELD% = 5                                               :\
        RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                            \
                                 VARIABLE.STRING$,                       \
                                 RETURN.FIELD%)                         :\
        IF RC% NE 0 THEN                                                 \
           GOTO CHAIN.OUT

      KEY.PRESSED% = 999

   RETURN

\*******************************************************************************
\***
\***   ESCAPE.TO.MAIN.MENU:
\***
\***   set psbchn mencon to null
\***   set psbchn prg to "ADX_UPGM:PSB50.286"
\***   %INCLUDE PSBCHN.J86
\***
\...............................................................................

      ESCAPE.TO.MAIN.MENU:

         PSBCHN.MENCON = ""
         PSBCHN.PRG = "ADX_UPGM:PSB50.286"
         %INCLUDE PSBCHNE.J86                                          ! FAW

\*******************************************************************************
\***
\***   HELP.ON.7201:
\***
\***   set string data to spaces
\***   set integer data to 1
\***   CALL DM.POSF function to position the cursor in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 0
\***   CALL DM.UPDF function to get message
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   CALL HELP function
\***   IF F12.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 1
\***   CALL DM.POSF function to position cursor in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to message
\***   set integer data to 0
\***   CALL DM.PUTF function to put message in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   GOSUB PUT.SELECTION
\***
\***   RETURN
\***
\...............................................................................

   HELP.ON.7201:

      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.UPDF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT
      SAVED.DISP.MESSAGE$ = F03.RETURNED.STRING$

      RC% EQ HELP (B7201.SCREEN.NO$)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      STRING.DATA$ = SAVED.DISP.MESSAGE$
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      GOSUB PUT.SELECTION

   RETURN

\*******************************************************************************
\***
\***   PUT.SELECTION:
\***
\***   set string data to spaces
\***   set integer data to 5
\***   CALL DM.POSF function to position the cursor in field 5
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to selection
\***   set integer data to 0
\***   CALL DM.PUTF function to put selection in field 5
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   RETURN
\***
\...............................................................................

   PUT.SELECTION:

      STRING.DATA$ = ""
      INTEGER.DATA% = 5
      RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

      STRING.DATA$ = SELECTION$
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF (STRING.DATA$,INTEGER.DATA%)
      IF RC% NE 0 THEN                                                 \
         GOTO CHAIN.OUT

   RETURN

\*******************************************************************************
\***
\***    CONFIRM.CHOICE:
\***
\***    WHILE key selected <> "N" AND key selected <> "Y"
\***            CALL DM.UPDF to get key selected and exit key
\***            IF exit key is the HELP key THEN
\***            BEGIN
\***                    Save the returned string
\***                    Call the required help routine
\***                    Restore the returned string
\***                    Restore the 'Are you sure (Y/N) ?' prompt
\***            ENDIF
\***            ELSE
\***            IF key selected <> "N" AND key selected <> "Y" THEN
\***            BEGIN
\***                    Set message number to 1
\***                    CALL EXTERNAL.MESSAGE to display message B001
\***                    IF the return code <> 0 THEN
\***                            GOTO CHAIN.OUT to exit the program
\***            ENDIF
\***    WEND
\***
\***    IF key selected = "N" THEN
\***    BEGIN
\***            Set decreases already performed flag to "N"
\***    ENDIF
\***
\***    RETURN
\***
\*******************************************************************************

        CONFIRM.CHOICE:

        KEY$ = ""                                                       ! BMW
        EXIT.KEY% = 999                                                 ! BMW

        WHILE (KEY$ <> "N" AND KEY$ <> "Y") OR EXIT.KEY% <> ENTER%      ! BMW

                STRING.DATA$ = ""                                       ! BMW
                INTEGER.DATA% = 0                                       ! BMW

                RC% EQ DM.UPDF(STRING.DATA$,INTEGER.DATA%)              !DSJW
                IF RC% NE 0 THEN                                        \DSJW
                   GOTO CHAIN.OUT                                       !DSJW

                KEY$ = F03.RETURNED.STRING$                             ! BMW
                EXIT.KEY% = F03.RETURNED.INTEGER%                       ! BMW

                IF EXIT.KEY% = HELP% THEN                               \ BMW
                BEGIN                                                   ! BMW
                        GOSUB HELP.ON.7201                              ! BMW

                        STRING.DATA$ = ""                               ! BMW
                        INTEGER.DATA% = 7                               ! BMW

                        RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)      !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW

                        STRING.DATA$ = "Are you sure? (Y/N)"            ! BMW
                        INTEGER.DATA% = 0                               ! BMW

                        RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)      !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW

                        STRING.DATA$ = "0"                              ! BMW
                        INTEGER.DATA% = 0                               ! BMW

                        RC% EQ DM.SETF(STRING.DATA$,INTEGER.DATA%)      !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW

                        STRING.DATA$ = ""                               ! BMW
                        INTEGER.DATA% = 8                               ! BMW

                        RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)      !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW

                        STRING.DATA$ = KEY$                             ! BMW
                        INTEGER.DATA% = 0                               ! BMW

                        RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)      !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW
                ENDIF                                                   \ BMW
                ELSE                                                    \ BMW
                IF KEY$ <> "N" AND KEY$ <> "Y" THEN                     \ BMW
                BEGIN                                                   ! BMW
                        MESSAGE.NO% = 1                                 ! BMW
                        VARIABLE.STRING$ = ""                           ! BMW
                        RETURN.FIELD% = 8                               ! BMW

                        RC% EQ EXTERNAL.MESSAGE(MESSAGE.NO%,            \DSJW
                                                VARIABLE.STRING$,       \DSJW
                                                RETURN.FIELD%)          !DSJW
                        IF RC% NE 0 THEN                                \DSJW
                                GOTO CHAIN.OUT                          !DSJW
                ENDIF                                                   ! BMW
        WEND                                                            ! BMW

        RETURN                                                          ! BMW

\*******************************************************************************
\***
\***   GET.DATA:
\***
\***   set count of entries to 0
\***   set eof flag off
\***   set first string to null
\***   set ppfi record count to 0
\***
\***   IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***   CALL READ.PPFI
\***   increment ppfi record count by 1
\***
\***   WHILE eof flag is off
\***   AND ppfi boots code is not 9999999 (trailer record)
\***   AND count of entries is less than 7250
\***
\***      IF ppf rec type = "R" THEN
\***         IF ppfi increase/decrease flag is same as increase/decrease flag
\***         AND ppfi status flag is not blank THEN
\***            increment count of entries by 1
\***            IF ppfi rpd number is 99999 THEN
\***               set ppfi rpd number to 0
\***               set ppfi date to 0
\***               set first string to first string + ppfi date and rpd number
\***            ELSE
\***               set first string to first string + ppfi date and rpd number
\***            endif
\***         endif
\***      endif
\***
\***      IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***      CALL READ.PPFI
\***      increment ppfi record count by 1
\***
\***   WEND
\***
\***   GOSUB CHECK.PPFI.TRAILER
\***
\...............................................................................

   GET.DATA:

      STRING.DATA$ = ""                                                ! BMW
      INTEGER.DATA% = 107                                              ! BMW

      RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)                      !DSJW

      IF RC% NE 0 THEN                                                 \DSJW
         GOTO CHAIN.OUT                                                \ BMW
      ELSE                                                             \ BMW
         MESSAGE.NO% = 405                                            :\ BMW
         RETURN.FIELD% = F03.RETURNED.INTEGER%                        :\ BMW
         VARIABLE.STRING$ = ""                                        :\ BMW
         RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                         \DSJW
                                  VARIABLE.STRING$,                    \DSJW
                                  RETURN.FIELD%)                      :\DSJW
         IF RC% NE 0 THEN                                              \DSJW
            GOTO CHAIN.OUT                                             !DSJW

      ENTRY.COUNT% = 0
      END.OF.FILE$ = "N"
!     Line deleted                                                     ! 1.20 RC
      PPFI.RECORD.COUNT% = 0

      DIM F14.TABLE$ (TABLE.LIMIT.14K%)                                ! 1.20 RC
      F14.TABLE$(0) = ""                                               ! 1.20 RC

      GOSUB OPEN.PPFI.LOCKED                                           ! 1.19 RC
       
      RC% EQ READ.PPFI                                                  !DSJW
      IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                             !DSJW
      PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      WHILE END.OF.FILE$ = "N"                                         \
      AND PPF.BOOTS.CODE$ <> "9999999"                                 \CSJW
      AND ENTRY.COUNT% < TABLE.LIMIT.14K%                              ! 1.20 RC

         IF PPF.REC.TYPE.FLAG$ = "R" THEN                              \
            IF PPF.INC.DEC.FLAG$ = INC.DEC$                            \
            AND PPF.STATUS.FLAG$ <> " " THEN                           \
               ENTRY.COUNT% = ENTRY.COUNT% + 1                        :\
               IF VAL(PPF.RPD.NO$) = 99999 THEN BEGIN                  !CSJW
                  F14.TABLE$(ENTRY.COUNT%) = \                         ! 1.20 RC
                                       F14.TABLE$(ENTRY.COUNT%) + \    ! 1.20 RC
                                       PACK$("00000000") +             \GSB
                                       PACK$("000000")
               ENDIF ELSE BEGIN
                  IF LEFT$(PPF.DATE.DUE$,2) < "85" THEN BEGIN          !GSB
                     F14.TABLE$(ENTRY.COUNT%) = \                      ! 1.20 RC
                                       F14.TABLE$(ENTRY.COUNT%) + \    ! 1.20 RC
                                       PACK$("20" + PPF.DATE.DUE$) +   \CSJW GSB
                                       PACK$("0" + PPF.RPD.NO$)        !CSJW
                  ENDIF ELSE BEGIN
                     F14.TABLE$(ENTRY.COUNT%) = \                      ! 1.20 RC
                                       F14.TABLE$(ENTRY.COUNT%) + \    ! 1.20 RC
                                       PACK$("19" + PPF.DATE.DUE$) +   \GSB
                                       PACK$("0" + PPF.RPD.NO$)        !GSB
                  ENDIF                                                !GSB
               ENDIF

         RC% EQ READ.PPFI                                               !DSJW
         IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                          !DSJW
         PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      WEND

      GOSUB CHECK.PPFI.TRAILER

      CLOSE PPFI.SESS.NUM%                                             ! 1.19 RC

\...............................................................................
\***
\***   DIMension F14.TABLE$ to have count of entries entries
\***
\***   set F14.TABLE$(0) to null
\***
\***   IF count of entries is 0 THEN
\***      set current page to 1
\***      set total pages to 1
\***      set total entries to 0
\***      GOSUB SHOW.PAGE
\***      RETURN
\***   endif
\***
\***   FOR entry from 1 to count of entries
\***
\***      set F14.TABLE$(entry) to part of first string starting at
\***                                   ((entry - 1) * length of entry) + 1, for
\***                                   length of entry
\***
\***   NEXT entry
\***
\***   IF count of entries is greater than or equal to 7250 THEN
\***      set string data to spaces
\***      set integer data to 0 (current field)
\***      CALL DM.POSF function to retrieve the current field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message number 265,
\***                                           return to current field
\***    "All RPD's may not be displayed - repeat later"
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         endif
\***      endif
\***   endif
\***
\***   CALL SORT.TABLE function to sort F14.TABLE$, using count of entries
\***   IF F14.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\...............................................................................

!     F14.TABLE$ initialisation moved to start of procedure          ! 1.20 RC

      IF ENTRY.COUNT% = 0 THEN                                         \
         CURRENT.PAGE% = 1                                            :\
         TOTAL.PAGES% = 1                                             :\
         TOTAL.ENTRIES% = 0                                           :\
         GOSUB SHOW.PAGE                                              :\
         RETURN

!     Lines deleted                                                  ! 1.20 RC

      IF ENTRY.COUNT% >= TABLE.LIMIT.14K% THEN BEGIN                 ! 1.20 RC
         STRING.DATA$ = ""                                           ! 1.20 RC
         INTEGER.DATA% = 0                                           ! 1.20 RC
         RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)                 ! 1.20 RC
         IF RC% NE 0 THEN BEGIN                                      ! 1.20 RC
            GOTO CHAIN.OUT                                           ! 1.20 RC
         ENDIF ELSE BEGIN                                            ! 1.20 RC
!           WAIT ; 500 ! Make initial of message more visible        ! 1.20 RC
            MESSAGE.NO% = 265
            GOSUB DISPLAY.BEMF.MSG.265                               ! 1.20 RC
!           MESSAGE.NO% = 265                                        ! 1.20 RC
!           RETURN.FIELD% = F03.RETURNED.INTEGER%                    ! 1.20 RC
!           VARIABLE.STRING$ = ""                                    ! 1.20 RC
!           RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \DJSW
!                                    VARIABLE.STRING$,                 \DSJW
!                                    RETURN.FIELD%)                   ! 1.20 RC
!           IF RC% NE 0 THEN                                           \DSJW
!              GOTO CHAIN.OUT
!           WAIT ; 1500 ! Allow message to be read                   ! 1.20 RC
         ENDIF                                                       ! 1.20 RC
      ENDIF                                                          ! 1.20 RC
      
      RC% EQ SORT.TABLE(ENTRY.COUNT%)                                   !DSJW
      IF RC% NE 0 THEN                                                  \DSJW
         GOTO CHAIN.OUT

\..............................................................................
\***
\***   set display count to 0
\***   set saved entry to F14.TABLE$(1)
\***   set lines to 1
\***
\***   FOR entry from 2 to count of entries
\***
\***      IF F14.TABLE$(entry) <> saved entry THEN
\***         increment display count by 1
\***         set second string to second string + saved entry and lines
\***                             and blank space (for increase/decrease flag)
\***         set saved entry to F14.TABLE$(entry)
\***         set lines to 1
\***      ELSE
\***         increment lines by 1
\***      endif
\***
\***   NEXT entry
\***
\***   increment display count by 1
\***   set second string to second string + saved entry and lines
\***                              and blank space (for increase/decrease flag)
\***
\...............................................................................

      DIM DISPLAY.TABLE$ (TABLE.LIMIT.14K%)                          ! 1.20 RC

      DISPLAY.COUNT% = 0
      SAVED.ENTRY$ = F14.TABLE$(1)
      LINES% = 1

      FOR INDEX% = 2 TO ENTRY.COUNT%

         IF F14.TABLE$ (INDEX%) <> SAVED.ENTRY$ THEN BEGIN           ! 1.20 RC
            DISPLAY.COUNT% = DISPLAY.COUNT% + 1
            DISPLAY.TABLE$ (DISPLAY.COUNT%) = \                      ! 1.20 RC
                                  RIGHT$(SAVED.ENTRY$,6) +             \GSB
                                  PACK$(RIGHT$("0000" +                \
                                        STR$(LINES%),4)) +             \
                                  " "                                ! 1.20 RC
            SAVED.ENTRY$ = F14.TABLE$ (INDEX%)                       ! 1.20 RC
            LINES% = 1                                               ! 1.20 RC
         ENDIF ELSE BEGIN                                            ! 1.20 RC
            LINES% = LINES% + 1
         ENDIF                                                       ! 1.20 RC
      
      NEXT INDEX%

      DISPLAY.COUNT% = DISPLAY.COUNT% + 1
      DISPLAY.TABLE$ (DISPLAY.COUNT%) = \                            ! 1.20 RC
                                  RIGHT$(SAVED.ENTRY$,6) +             \GSB
                                  PACK$(RIGHT$("0000" +                \
                                        STR$(LINES%),4)) +             \
                                  " "

\...............................................................................
\***
\***   DIMension display table to have display count entries
\***
\***   FOR entry from 1 to display count
\***
\***      set display table(entry) to part of second string starting at
\***                                  ((entry - 1) * length of entry) + 1, for
\***                                  length of entry
\***
\***   NEXT entry
\***
\***   DIMension F14.TABLE$ to 0
\***   set first string to null
\***   set second string to null
\***
\***   set total entries to display count
\***
\***   IF remainder of (total entries divided by 26) is not 0 THEN
\***      set total pages to (total entries divided by 26) plus 1
\***   ELSE
\***      set total pages to total entries divided by 26
\***   endif
\***
\***   set current page to 1
\***
\***   GOSUB SHOW.PAGE
\***
\***   RETURN
\***
\...............................................................................

!     Lines deleted                                                  ! 1.20 RC
      DIM F14.TABLE$(0)
!     Lines deleted                                                  ! 1.20 RC

      TOTAL.ENTRIES% = DISPLAY.COUNT%

      IF MOD (TOTAL.ENTRIES%,26) <> 0 THEN                             \
         TOTAL.PAGES% = (TOTAL.ENTRIES% / 26) + 1                     :\
      ELSE                                                             \
         TOTAL.PAGES% = (TOTAL.ENTRIES% / 26)

      CURRENT.PAGE% = 1

      GOSUB SHOW.PAGE

   RETURN


\*****************************************************************************
\***   DISPLAY.BEMF.MSG.265
\*****************************************************************************

DISPLAY.BEMF.MSG.265: ! Code extracted into separate procedure       ! 1.20 RC

    MESSAGE.NO% = 265
    RETURN.FIELD% = F03.RETURNED.INTEGER%
    VARIABLE.STRING$ = ""
    
    RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%, \
                             VARIABLE.STRING$, \
                             RETURN.FIELD%)
    
    IF RC% NE 0 THEN GOTO CHAIN.OUT

RETURN


\*****************************************************************************
\***   CREATE.PPFO
\*****************************************************************************

   CREATE.PPFO:                                                         !DSJW

      CURRENT.SESS.NUM% = PPFO.SESS.NUM%                                !DSJW
      IF END # PPFO.SESS.NUM% THEN CREATE.ERROR                         !DSJW
      CREATE POSFILE PPFO.FILE.NAME$ AS PPFO.SESS.NUM% \                !DSJW
             BUFFSIZE PPFO.BUFF.SIZE% LOCKED MIRRORED ATCLOSE           !DSJW
      PPFO.THERE.FLAG$ = "Y"                                            !DSJW

   RETURN                                                               !DSJW

\***************************************************************************
\***
\***   PROCESS.ENTRY:
\***
\***   set effect flag off
\***   IF count of entries is 0 THEN
\***      set move cursor flag off
\***      RETURN
\***   endif
\***
\***   IF F03.RETURNED.STRING$ is not blank or increase/decrease flag THEN
\***      set string data to spaces
\***      set integer data to 0 (current field)
\***      CALL DM.POSF function to retrieve the number of the current field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message number 061,
\***                                           return to current field
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            set move cursor flag off
\***            RETURN
\***         endif
\***      endif
\***   endif
\***
\***   set display table(display entry) to F03.RETURNED.STRING$
\***
\...............................................................................

   PROCESS.ENTRY:

      EFFECT.FLAG$ = "N"

      IF ENTRY.COUNT% = 0 THEN                                         \
         MOVE.CURSOR$ = "N"                                           :\
         RETURN

      IF F03.RETURNED.STRING$ <> " " AND                               \
         F03.RETURNED.STRING$ <> INC.DEC$ THEN                         \
         \ 26 lines deleted from here !!!!!                            ! BMW
         STRING.DATA$ = ""                                            :\
         INTEGER.DATA% = 0                                            :\
         RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)                  :\DSJW
         IF RC% NE 0 THEN                                              \DSJW
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            RETURN.FIELD% = F03.RETURNED.INTEGER%                     :\
            MESSAGE.NO% = 61                                          :\
            VARIABLE.STRING$ = ""                                     :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,VARIABLE.STRING$,     \DSJW
                                     RETURN.FIELD%)                   :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               MOVE.CURSOR$ = "N"                                     :\
               RETURN

      DISPLAY.TABLE$ (DISPLAY.ENTRY%) =                                \
                     LEFT$(DISPLAY.TABLE$ (DISPLAY.ENTRY%),8) +        \
                     F03.RETURNED.STRING$

\...............................................................................
\***
\***   IF key pressed is F3
\***      IF f3 pressed flag is on THEN
\***         set quit flag on
\***         RETURN
\***      ELSE
\***         GOSUB QUIT.KEY.PRESSED
\***         RETURN
\***      endif
\***
\***   set f3 pressed flag off
\***
\***   IF key pressed is Home THEN
\***      set nextf parameter to -20 (first input field)
\***      set current entry to 1
\***      set display entry to ((current page - 1) * 26) + current entry
\***      set move cursor flag on
\***      RETURN
\***   endif
\***
\***   IF key pressed is End THEN
\***      set current entry to number on page
\***      set display entry to ((current page - 1) * 26) + current entry
\***      set string data to spaces
\***      set integer data to last input field number
\***      CALL DM.POSF function to position the cursor accordingly
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         set move cursor flag off
\***         RETURN
\***      endif
\***   endif
\***
\...............................................................................

      IF KEY.PRESSED% = QUIT% THEN                                     \
         IF QUIT.PRESSED$ = "Y" THEN                                   \
            QUIT.FLAG$ = "Y"                                          :\
            RETURN                                                    :\
         ELSE                                                          \
            GOSUB QUIT.KEY.PRESSED                                    :\
            RETURN

      QUIT.PRESSED$ = "N"

      IF KEY.PRESSED% = HOME% THEN                                     \
         NEXTF.PARM% = -20                                            :\
         CURRENT.ENTRY% = 1                                           :\
         DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) + CURRENT.ENTRY% :\
         MOVE.CURSOR$ = "Y"                                           :\
         RETURN

      IF KEY.PRESSED% = END.KEY% THEN                                  \
         CURRENT.ENTRY% = NUM.ON.PAGE%                                :\
         DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) + CURRENT.ENTRY% :\
         STRING.DATA$ = ""                                            :\
         INTEGER.DATA% = LAST.INPUT.FIELD.NO%                         :\
         RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)                  :\DSJW
         IF RC% NE 0 THEN                                              \DSJW
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            MOVE.CURSOR$ = "N"                                        :\
            RETURN

\...............................................................................
\***
\***   IF key pressed is Tab THEN
\***      IF current entry is greater than or equal to number on page THEN
\***         set string data to spaces
\***         set integer data to 0 (current field)
\***         CALL DM.POSF function to retrieve the number of the current field
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            CALL EXTERNAL.MESSAGE function to display message number 001
\***                                               return to current field
\***            IF F04.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               set move cursor flag off
\***               RETURN
\***            endif
\***         endif
\***      ELSE
\***         IF current entry is 13 THEN
\***            set current entry to 14
\***            set display entry to ((current page - 1) * 26) + current entry
\***            set string data to spaces
\***            set integer data to -20 (first input field)
\***            CALL DM.NEXTF function to position the cursor accordingly
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               set nextf parameter to 2(next input field)
\***               set move cursor flag on
\***               RETURN
\***            endif
\***         ELSE
\***            increment current entry by 1
\***            set display entry to ((current page - 1) * 26) + current entry
\***            set string data to spaces
\***            set integer data to 2 (next input field)
\***            CALL DM.NEXTF function to position the cursor accordingly
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               set nextf parameter to 2 (next input field)
\***               set move cursor flag on
\***               RETURN
\***         endif
\***      endif
\***   endif
\***
\...............................................................................

      IF KEY.PRESSED% = TAB% THEN                                      \
         IF CURRENT.ENTRY% >= NUM.ON.PAGE% THEN                        \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.POSF (STRING.DATA$,INTEGER.DATA%)               :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               VARIABLE.STRING$ = ""                                  :\
               MESSAGE.NO% = 1                                        :\
               RETURN.FIELD% = F03.RETURNED.INTEGER%                  :\
               RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                   \DSJW
                                        VARIABLE.STRING$,              \DSJW
                                        RETURN.FIELD%)                :\DSJW
               IF RC% NE 0 THEN                                        \DSJW
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
                  MOVE.CURSOR$ = "N"                                  :\
                  RETURN                                              :\
         ELSE                                                          \
            IF CURRENT.ENTRY% = 13 THEN                                \
               CURRENT.ENTRY% = 14                                    :\
               DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) +           \
                                CURRENT.ENTRY%                        :\
               STRING.DATA$ = ""                                      :\
               INTEGER.DATA% = -20                                    :\
               RC% EQ DM.NEXTF (STRING.DATA$,INTEGER.DATA%)           :\DSJW
               IF RC% NE 0 THEN                                        \DSJW
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
                  NEXTF.PARM% = 2                                     :\
                  MOVE.CURSOR$ = "Y"                                  :\
                  RETURN                                              :\
            ELSE                                                       \
               CURRENT.ENTRY% = CURRENT.ENTRY% + 1                    :\
               DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) +           \
                                CURRENT.ENTRY%                        :\
               STRING.DATA$ = ""                                      :\
               INTEGER.DATA% = 2                                      :\
               RC% EQ DM.NEXTF (STRING.DATA$,INTEGER.DATA%)           :\DSJW
               IF RC% NE 0 THEN                                        \DSJW
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
                  NEXTF.PARM% = 2                                     :\
                  MOVE.CURSOR$ = "Y"                                  :\
                  RETURN

\...............................................................................
\***
\***   IF key pressed is PgUp THEN
\***      IF current page is 1 THEN
\***         set string data to spaces
\***         set integer data to 0 (current field)
\***         CALL DM.POSF function to retrieve the number of the current field
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            CALL EXTERNAL.MESSAGE function to display message number 074,
\***                                              return to current field
\***            IF F04.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               set move cursor flag off
\***               RETURN
\***            endif
\***         endif
\***      ELSE
\***         decrement current page by 1
\***         GOSUB SHOW.PAGE
\***         set nextf parameter to -20 (first input field)
\***         set current entry to 1
\***         set display entry to ((current page - 1) * 26) + current entry
\***         set move cursor flag on
\***         RETURN
\***      endif
\***   endif
\***
\...............................................................................

      IF KEY.PRESSED% = PAGE.UP% THEN                                  \
         IF CURRENT.PAGE% = 1 THEN                                     \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                :\DJSW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               VARIABLE.STRING$ = ""                                  :\
               MESSAGE.NO% = 74                                       :\
               RETURN.FIELD% = F03.RETURNED.INTEGER%                  :\
               RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                   \DSJW
                                        VARIABLE.STRING$,              \DSJW
                                        RETURN.FIELD%)                :\DSJW
               IF RC% NE 0 THEN                                        \DSJW
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
                  MOVE.CURSOR$ = "N"                                  :\
                  RETURN                                              :\
         ELSE                                                          \
            CURRENT.PAGE% = CURRENT.PAGE% - 1                         :\
            GOSUB SHOW.PAGE                                           :\
            NEXTF.PARM% = -20                                         :\
            CURRENT.ENTRY% = 1                                        :\
            DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) +              \
                             CURRENT.ENTRY%                           :\
            MOVE.CURSOR$ = "Y"                                        :\
            RETURN

\...............................................................................
\***
\***   IF key pressed is PgDn THEN
\***      IF current page = total pages THEN
\***         set string data to spaces
\***         set integer data to 0 (current field)
\***         CALL DM.POSF function to retrieve the number of the current field
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            CALL EXTERNAL.MESSAGE function to display message number 075,
\***                                              return to current field
\***            IF F04.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            ELSE
\***               set move cursor flag off
\***               RETURN
\***            endif
\***         endif
\***      ELSE
\***         increment current page by 1
\***         GOSUB SHOW.PAGE
\***         set nextf parameter to -20 (first input field)
\***         set current entry to 1
\***         set display entry to ((current page - 1) * 26) + current entry
\***         set move cursor flag on
\***         RETURN
\***      endif
\***   endif
\***
\...............................................................................

      IF KEY.PRESSED% = PAGE.DOWN% THEN                                \
         IF CURRENT.PAGE% = TOTAL.PAGES% THEN                          \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = 0                                         :\
            RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                :\DJSW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               VARIABLE.STRING$ = ""                                  :\
               MESSAGE.NO% = 75                                       :\
               RETURN.FIELD% = F03.RETURNED.INTEGER%                  :\
               RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                   \DSJW
                                        VARIABLE.STRING$,              \DSJW
                                        RETURN.FIELD%)                :\DSJW
               IF RC% NE 0 THEN                                        \DSJW
                  GOTO CHAIN.OUT                                      :\
               ELSE                                                    \
                  MOVE.CURSOR$ = "N"                                  :\
                  RETURN                                              :\
         ELSE                                                          \
            CURRENT.PAGE% = CURRENT.PAGE% + 1                         :\
            GOSUB SHOW.PAGE                                           :\
            NEXTF.PARM% = -20                                         :\
            CURRENT.ENTRY% = 1                                        :\
            DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) +              \
                             CURRENT.ENTRY%                           :\
            MOVE.CURSOR$ = "Y"                                        :\
            RETURN

\...............................................................................
\***
\***   set count to 1
\***
\***   WHILE count is less than or equal to total entries
\***   AND effect flag is off
\***
\***      IF display table(count) flag is increase/decrease flag THEN
\***         set effect flag on
\***      endif
\***      increment count by 1
\***
\***   WEND
\***
\***   IF effect flag is off THEN
\***      set string data to spaces
\***      set integer data to 0 (current field)
\***      CALL DM.POSF function to retrieve the number of the current field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message number 002
\***                                           return to current field
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            set move cursor flag off
\***            RETURN
\***         endif
\***      endif
\***   endif
\***
\***   CALL DM.POSF function to obtain the current field number
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   CALL EXTERNAL.MESSAGE function to display message number 273,
\***                                       return to current field
\***   IF F04.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\...............................................................................

      INDEX% = 1

      WHILE INDEX% <= TOTAL.ENTRIES%                                   \
      AND EFFECT.FLAG$ = "N"

         IF RIGHT$(DISPLAY.TABLE$ (INDEX%), 1) = INC.DEC$ THEN         \
            EFFECT.FLAG$ = "Y"

         INDEX% = INDEX% + 1

      WEND

      IF EFFECT.FLAG$ = "N" THEN                                       \
         STRING.DATA$ = ""                                            :\
         INTEGER.DATA% = 0                                            :\
         RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                   :\DJSW
         IF RC% NE 0 THEN                                              \DSJW
               GOTO CHAIN.OUT                                         :\
         ELSE                                                          \
            MESSAGE.NO% = 2                                           :\
            RETURN.FIELD% = F03.RETURNED.INTEGER%                     :\
            VARIABLE.STRING$ = ""                                     :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \DSJW
                                     VARIABLE.STRING$,                 \DSJW
                                     RETURN.FIELD%)                   :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               MOVE.CURSOR$ = "N"                                     :\
               RETURN

      STRING.DATA$ = ""
      INTEGER.DATA% = 107                                              ! BMW

      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN                                                 \DSJW
          GOTO CHAIN.OUT

      MESSAGE.NO% = 273
      VARIABLE.STRING$ = ""
      RETURN.FIELD% = F03.RETURNED.INTEGER%
      RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,VARIABLE.STRING$,           \DSJW
                               RETURN.FIELD%)                          !DSJW
      IF RC% NE 0 THEN                                                 \DSJW
         GOTO CHAIN.OUT

\...............................................................................
\***
\***   CREATE the ppfo
\***   CLOSE the ppfi
\***   IF END occurs on opening the ppfi THEN OPEN.PPFI.ERROR
\***   OPEN the ppfi LOCKED NOWRITE
\***   set the ppfi record count to 0
\***   set the ppfo record count to 0
\***
\***   IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***   CALL READ.PPFI
\***   increment ppfi record count by 1
\***
\***   WHILE eof flag is off
\***   AND ppfi boots code is not 9999999 (trailer)
\***
\***      IF ppf rec type flag = "R" AND
\***         ppfi increase/decrease flag is increase/decrease flag AND
\***         ppfi status flag is not blank THEN
\***         GOSUB CHECK.FOR.IRF.UPDATE
\***      ELSE
\***         IF END occurs on writing ppfo THEN FILE.ERROR
\***         CALL WRITE.PPFO to write out ppfi record
\***         increment ppfo record count by 1
\***      endif
\***
\***      IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***      CALL READ.PPFI
\***      increment ppfi record count by 1
\***
\***   WEND
\***
\***   IF ppfi trailer error flag = "N" THEN
\***      GOSUB CHECK.PPFI.TRAILER
\***   endif
\***
\***   WRITE trailer record to ppfo
\***
\***   CLOSE ppfo
\***
\***   DELETE ppfi
\***
\***   RENAME ppfo as ppfi
\***   IF rename return code is not -1 THEN
\***      CALL APPLICATION.LOG function to log event 14
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   IF END occurs on open ppfi THEN OPEN.PPFI.ERROR
\***   OPEN ppfi LOCKED NOWRITE
\***
\...............................................................................

      GOSUB CREATE.PPFO                                                 !DSJW

!     Lines deleted - PPFI close and re-open                           ! 1.19 RC

      GOSUB OPEN.ITEM.FILES                                            ! 1.19 RC

      PPFI.RECORD.COUNT% = 0
      PPFO.RECORD.COUNT% = 0

      RC% EQ READ.PPFI                                                  !DSJW
      IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                             !DSJW
      PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      END.OF.FILE$ = "N"

      WHILE END.OF.FILE$ = "N"                                         \
      AND PPF.BOOTS.CODE$ <> "9999999"                                 !CSJW

         IF   PPF.REC.TYPE.FLAG$ = "R" \                                !DSJW
          AND PPF.INC.DEC.FLAG$ = INC.DEC$ \                            !DSJW
          AND PPF.STATUS.FLAG$ <> " " THEN BEGIN                        !DSJW
            GOSUB CHECK.FOR.IRF.UPDATE                                  !DSJW
         ENDIF ELSE BEGIN                                               !DSJW
            FAIL.TYPE$ = "W"                                            !DSJW
            RC% EQ WRITE.PPFO                                           !DSJW
			PRINT #992 ; "ADDED FROM LINE 2281 " , PPF.RECORD$
            IF RC% NE 0 THEN GOTO FILE.ERROR                            !DSJW
            PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                 !DSJW
         ENDIF                                                          !DSJW

         RC% EQ READ.PPFI                                               !DSJW
         IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                          !DSJW
         PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      WEND

      IF PPFI.TRAILER.ERROR$ = "N" THEN                                \
         GOSUB CHECK.PPFI.TRAILER

      PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1
      PPF.BOOTS.CODE$ = "9999999"                                       !CSJW
      PPF.REC.COUNT$ = RIGHT$("00000" + STR$(PPFO.RECORD.COUNT%),5)
      FAIL.TYPE$ = "W"                                                  !DSJW
      RC% EQ WRITE.PPFO                                                 !DSJW
	  PRINT #992 ; "ADDED FROM LINE 2300 " , PPF.RECORD$
      IF RC% NE 0 THEN GOTO FILE.ERROR                                  !DSJW

      IF PPFO.THERE.FLAG$ = "Y" THEN                                   \
         PPFO.THERE.FLAG$ = "N"                                       :\
         CLOSE PPFO.SESS.NUM%

      DELETE PPFI.SESS.NUM%                                             !ESJW

      RENAME.RET.CODE% = RENAME (PPFI.FILE.NAME$, PPFO.FILE.NAME$)
      IF RENAME.RET.CODE% <> -1 THEN BEGIN
         VAR.STRING.2$ = "PPFO PPFI "
         VAR.STRING.1$ = "PPFO"
         MESSAGE.NO% = 554
         EVENT.NO% = 14
         IF PSBCHN.APP = "PSB51" THEN                                  \
            PSBCHN.U2 = PSBCHN.U2 + "554" + VAR.STRING.2$             :\
            BATCH.SCREEN.FLAG$ = "B"
         RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,            \DSJW
                                 VAR.STRING.2$,EVENT.NO%)              !DSJW
         BATCH.SCREEN.FLAG$ = "S"
         GOTO CHAIN.OUT
         ENDIF

      GOSUB CLOSE.ITEM.FILES                                           ! 1.19 RC

!     Lines deleted - PPFI related                                     ! 1.19 RC


\...............................................................................
\***
\***   DIMension second table to size of display table
\***
\***   set count to 0
\***
\***   FOR entry from 1 to total entries
\***
\***      IF display table(entry) flag is blank THEN
\***         increment count by 1
\***         set second table(count) to display table(entry)
\***      endif
\***
\***   NEXT entry
\***
\***   DIMension display table to count entries
\***
\***   FOR entry from 1 to count
\***
\***      set display table(entry) to second table(entry)
\***
\***   NEXT entry
\***
\***   DIMension second table to 0
\***
\***   set total entries to count
\***
\***   IF remainder of (total entries divided by 26) is not 0 THEN
\***      set total pages to (total entries divided by 26) plus 1
\***   ELSE
\***      set total pages to total entries divided by 26
\***   endif
\***
\***   IF total pages is 0 THEN
\***      set total pages to 1
\***      set count of entries to 0
\***   endif
\***
\***   set current page to 1
\***
\***   GOSUB SHOW.PAGE
\***
\***   CALL DM.POSF function to obtain the current field number
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   CALL EXTERNAL.MESSAGE function to display message number 274,
\***                                       return to current field
\***   IF F04.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set move cursor flag on
\***   IF entry count = 0 THEN
\***      set nextf parm to 20
\***      set zero entry flag to "Y"
\***   endif
\***
\***   RETURN
\***
\...............................................................................

      DIM DISP.2.TABLE$ (DISPLAY.COUNT%)

      COUNT% = 0

      FOR INDEX% = 1 TO TOTAL.ENTRIES%

         IF RIGHT$(DISPLAY.TABLE$ (INDEX%), 1) = " " THEN              \
            COUNT% = COUNT% + 1                                       :\
            DISP.2.TABLE$ (COUNT%) = DISPLAY.TABLE$ (INDEX%)

      NEXT INDEX%

      DIM DISPLAY.TABLE$ (COUNT%)

      FOR INDEX% = 1 TO COUNT%

          DISPLAY.TABLE$ (INDEX%) = DISP.2.TABLE$ (INDEX%)

      NEXT INDEX%

      DIM DISP.2.TABLE$ (0)

      TOTAL.ENTRIES% = COUNT%

      IF MOD (TOTAL.ENTRIES%, 26) <> 0 THEN                            \
         TOTAL.PAGES% = (TOTAL.ENTRIES% / 26) + 1                     :\
      ELSE                                                             \
         TOTAL.PAGES% = TOTAL.ENTRIES% / 26

      IF TOTAL.PAGES% = 0 THEN                                         \
         TOTAL.PAGES% = 1                                             :\
         ENTRY.COUNT% = 0

      CURRENT.PAGE% = 1

      GOSUB SHOW.PAGE

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN                                                  \DSJW
         GOTO CHAIN.OUT

      RETURN.FIELD% = F03.RETURNED.INTEGER%
      MESSAGE.NO% = 274
      VARIABLE.STRING$ = ""
      RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,VARIABLE.STRING$,            \DSJW
                               RETURN.FIELD%)                           !DSJW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      MOVE.CURSOR$ = "Y"
      IF ENTRY.COUNT% = 0 THEN                                         \
         NEXTF.PARM% = 20                                             :\
         ZERO.ENTRY.FLAG$ = "Y"

   DECREASE.PERFORMED.$ = "N"

   RETURN

\*******************************************************************************
\***
\***   AUTO.EFFECT.DECREASES:
\***
\***   IF  not called from store opening  THEN
\***       CALL EXTERNAL.MESSAGE to display message number 275,
\***                             returning to field number 2
\***
\***   CREATE the ppfo
\***   CLOSE the ppfi
\***   IF END occurs on opening the ppfi THEN OPEN.PPFI.ERROR
\***   OPEN the ppfi LOCKED NOWRITE
\***   set the ppfi record count to 0
\***   set the ppfo record count to 0
\***
\***   IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***   CALL READ.PPFI
\***   increment ppfi record count by 1
\***
\***   WHILE eof flag is off
\***   AND ppfi boots code is not 9999999 (trailer)
\***
\***      IF ppf rec type flag = "R" AND
\***         ppfi increase/decrease flag is increase/decrease flag AND
\***         ppfi status flag is not blank THEN
\***         GOSUB UPDATE.IRF
\***      ELSE
\***         IF END occurs on writing ppfo THEN FILE.ERROR
\***         CALL WRITE.PPFO to write out ppfi record
\***         increment ppfo record count by 1
\***      endif
\***
\***      IF END occurs on read ppfi THEN READ.PPFI.ERROR
\***      CALL READ.PPFI
\***      increment ppfi record count by 1
\***
\***   WEND
\***
\***   GOSUB CHECK.PPFI.TRAILER
\***
\***   WRITE trailer record to ppfo
\***
\***   CLOSE ppfo
\***
\***   DELETE ppfi
\***
\***   RENAME ppfo as ppfi
\***   IF rename return code is not -1 THEN
\***      CALL APPLICATION.LOG function to log event 14
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   CALL EXTERNAL.MESSAGE function to display message number 276
\***                         returning to field number 2
\***   IF F04.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set key pressed to 999
\***
\***   RETURN
\***
\***
\*******************************************************************************

   AUTO.EFFECT.DECREASES:

      IF PSBCHN.APP <> "PSB51" THEN BEGIN                               !DSJW
         VARIABLE.STRING$ = ""                                          !DSJW
         MESSAGE.NO% = 275    ! RPD price decreases are being effected  !DSJW
         RETURN.FIELD% = 2                                              !DSJW
         RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,VARIABLE.STRING$,         \DSJW
                                  RETURN.FIELD%)                        !DSJW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW
      ENDIF                                                             !DSJW

      GOSUB CREATE.PPFO                                                 !DSJW

!     Lines deleted - PPFI close and re-open                           ! 1.19 RC

      GOSUB OPEN.ITEM.FILES                                            ! 1.19 RC

      PPFI.RECORD.COUNT% = 0
      PPFO.RECORD.COUNT% = 0

      RC% EQ READ.PPFI                                                  !DSJW
      IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                             !DSJW
      PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      END.OF.FILE$ = "N"

      WHILE END.OF.FILE$ = "N"                                         \
      AND PPF.BOOTS.CODE$ <> "9999999"                                 !CSJW

         IF  PPF.REC.TYPE.FLAG$ = "R" \                                 !DSJW
         AND PPF.INC.DEC.FLAG$ = INC.DEC$ \                             !DSJW
         AND PPF.STATUS.FLAG$ <> " " THEN BEGIN                         !DSJW
             GOSUB UPDATE.IRF                                           !DSJW
         ENDIF ELSE BEGIN                                               !DSJW
             FAIL.TYPE$ = "W"                                           !DSJW
             RC% EQ WRITE.PPFO                                          !DSJW
			 PRINT #992 ; "ADDED FROM LINE 2552 " , PPF.RECORD$
             IF RC% NE 0 THEN GOTO FILE.ERROR                           !DSJW
             PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                !DSJW
         ENDIF                                                          !DSJW

       RC% EQ READ.PPFI                                                 !DSJW
       IF RC% NE 0 THEN GOTO READ.PPFI.ERROR                            !DSJW
       PPFI.RECORD.COUNT% = PPFI.RECORD.COUNT% + 1

      WEND

      GOSUB CHECK.PPFI.TRAILER

      PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1
      PPF.BOOTS.CODE$ = "9999999"                                       !CSJW
      PPF.REC.COUNT$ = RIGHT$("00000" + STR$(PPFO.RECORD.COUNT%),5)
      FAIL.TYPE$ = "W"                                                  !DSJW
      RC% EQ WRITE.PPFO                                                 !DSJW
	  PRINT #992 ; "ADDED FROM LINE 2570 " , PPF.RECORD$
      IF RC% NE 0 THEN GOTO FILE.ERROR                                  !DSJW

      IF PPFO.THERE.FLAG$ = "Y" THEN                                   \
         PPFO.THERE.FLAG$ = "N"                                       :\
         CLOSE PPFO.SESS.NUM%

      DELETE PPFI.SESS.NUM%                                             !ESJW
!     PPFI.OPEN.FLAG$ = "N"                                             !BMW ! 1.19 RC

      RENAME.RET.CODE% = RENAME (PPFI.FILE.NAME$, PPFO.FILE.NAME$)
      IF RENAME.RET.CODE% <> -1 THEN BEGIN
         VAR.STRING.2$ = "PPFO PPFI "
         VAR.STRING.1$ = "PPFO"
         MESSAGE.NO% = 554
         EVENT.NO% = 14
         IF PSBCHN.APP = "PSB51"  THEN                                 \
            PSBCHN.U2 = PSBCHN.U2 + "554" + VAR.STRING.2$             :\
            BATCH.SCREEN.FLAG$ = "B"
         RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,             \DSJW
                                 VAR.STRING.2$,EVENT.NO%)               !DSJW
         BATCH.SCREEN.FLAG$ = "S"
         GOTO CHAIN.OUT
         ENDIF

      ! 52 lines deleted from here!!!!!

      GOSUB CLOSE.ITEM.FILES                                           ! 1.19 RC

      KEY.PRESSED% = 999
      DECREASES.RUN.ALREADY.FLAG$ = "Y"
      DECREASE.PERFORMED.$ = "Y"

      RETURN


\*******************************************************************************
\***
\***   QUIT.KEY.PRESSED:
\***
\***   set effect flag off
\***   WHILE effect flag is off
\***   AND entry is less than or equal to total entries
\***
\***      IF display table(entry)flag is increase/decrease flag THEN
\***         set effect flag on
\***      endif
\***
\***   WEND
\***   IF effect flag is off THEN
\***      set quit flag on
\***   ELSE
\***      set string data to spaces
\***      set integer data to 0
\***      CALL DM.POSF function to obtain current field number
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message 253
\***                                            return to current field
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            set f3 pressed flag on
\***            set move cursor flag off
\***         endif
\***      endif
\***   endif
\***
\...............................................................................

   QUIT.KEY.PRESSED:

      EFFECT.FLAG$ = "N"
      INDEX% = 1

      WHILE EFFECT.FLAG$ = "N" AND TOTAL.ENTRIES% > INDEX%

         IF RIGHT$(DISPLAY.TABLE$ (INDEX%),1) = INC.DEC$ THEN          \
            EFFECT.FLAG$ = "Y"

         INDEX% = INDEX% + 1

      WEND

      IF EFFECT.FLAG$ = "N" THEN                                       \
         QUIT.FLAG$ = "Y"                                             :\
      ELSE                                                             \
         STRING.DATA$ = ""                                            :\
         INTEGER.DATA% = 0                                            :\
         RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                   :\DJSW
         IF RC% NE 0 THEN                                              \DSJW
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            CURRENT.FIELD% = F03.RETURNED.INTEGER%                    :\
            VARIABLE.STRING$ = ""                                     :\
            MESSAGE.NO% = 253                                         :\
            RETURN.FIELD% = CURRENT.FIELD%                            :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \DSJW
                                     VARIABLE.STRING$,                 \DSJW
                                     RETURN.FIELD%)                   :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT                                         :\
            ELSE                                                       \
               MOVE.CURSOR$ = "N"                                     :\
               QUIT.PRESSED$ = "Y"

   RETURN

\*******************************************************************************
\***
\***   HELP.ON.7203:
\***
\***   save current flag to F03.RETURNED.STRING$
\***
\***   set string data to spaces
\***   set integer data to 0
\***   CALL DM.POSF function to return the number of the current field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 1
\***   CALL DM.POSF function to position the cursor in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 0
\***   CALL DM.UPDF function to get message
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   CALL HELP function
\***   IF F12.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   GOSUB SHOW.PAGE
\***
\***   set string data to spaces
\***   set integer data to 1
\***   CALL DM.POSF function to position the cursor in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to message
\***   set integer data to 0
\***   CALL DM.PUTF function to place message in field 01
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to current field
\***   CALL DM.POSF function to position the cursor in the current field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to current flag
\***   set integer data to 0
\***   CALL DM.PUTF function to place current flag in current field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set move cursor flag off
\***
\***   RETURN
\***
\...............................................................................

   HELP.ON.7203:

      SAVED.DISP.FLAG$ = F03.RETURNED.STRING$

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW
      CURRENT.FIELD% = F03.RETURNED.INTEGER%

      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.UPDF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW
      SAVED.DISP.MESSAGE$ = F03.RETURNED.STRING$

      RC% EQ HELP(B7203.SCREEN.NO$)                                     !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      GOSUB SHOW.PAGE

      STRING.DATA$ = ""
      INTEGER.DATA% = 1
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = SAVED.DISP.MESSAGE$
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = CURRENT.FIELD%
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = SAVED.DISP.FLAG$
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      MOVE.CURSOR$ = "N"

   RETURN

\*******************************************************************************
\***
\***   OPEN.PPFI.ERROR:
\***
\***   set fail type to "O"
\***   set current session number to ppfi session number
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   OPEN.PPFI.ERROR:

      FAIL.TYPE$ = "O"
      CURRENT.SESS.NUM% = PPFI.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*******************************************************************************
\***
\***   READ.PPFI.ERROR:
\***
\***   set fail type to "R"
\***   set current session number to ppfi session number
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   READ.PPFI.ERROR:

      FAIL.TYPE$ = "R"
      CURRENT.SESS.NUM% = PPFI.SESS.NUM%
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*******************************************************************************
\***
\***   NO.ENTRIES.MESSAGE:
\***
\***   IF increase/decrease flag is "D" THEN
\***      set string data to spaces
\***      set integer data to 0 (current field)
\***      CALL DM.POSF function to retrieve the current field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message number 272,
\***                                           return to current field
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            RETURN
\***         endif
\***      endif
\***   ELSE
\***      set string data to spaces
\***      set integer data to 0 (current field)
\***      CALL DM.POSF function to retrieve the current field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      ELSE
\***         CALL EXTERNAL.MESSAGE function to display message number 271,
\***                                           return to current field
\***         IF F04.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            RETURN
\***         endif
\***      endif
\***   endif
\***
\***   RETURN
\***
\...............................................................................

   NO.ENTRIES.MESSAGE:

      IF INC.DEC$ = "D" THEN BEGIN                                     !DSJW
         STRING.DATA$ = ""                                             !DSJW
         INTEGER.DATA% = 0                                             !DSJW
         RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                    !DSJW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                              :\DSJW
         ELSE                                                          \
            MESSAGE.NO% = 272                                         :\
            RETURN.FIELD% = F03.RETURNED.INTEGER%                     :\
            VARIABLE.STRING$ = ""                                     :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \DSJW
                                     VARIABLE.STRING$,                 \DSJW
                                     RETURN.FIELD%)                   :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT
      ENDIF ELSE BEGIN                                                 !DSJW
         STRING.DATA$ = ""                                             !DSJW
         INTEGER.DATA% = 0                                             !DSJW
         RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                    !DSJW
         IF RC% NE 0 THEN                                              \DSJW
            GOTO CHAIN.OUT                                            :\
         ELSE                                                          \
            MESSAGE.NO% = 271                                         :\
            RETURN.FIELD% = F03.RETURNED.INTEGER%                     :\
            VARIABLE.STRING$ = ""                                     :\
            RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                      \DSJW
                                     VARIABLE.STRING$,                 \DSJW
                                     RETURN.FIELD%)                   :\DSJW
            IF RC% NE 0 THEN                                           \DSJW
               GOTO CHAIN.OUT
       ENDIF

   RETURN

\*******************************************************************************
\***
\***   SHOW.PAGE:
\***
\***   IF increase/decrease flag is "D" THEN
\***      set string data to "B7202"
\***   ELSE
\***      set string data to "B7203"
\***   endif
\***
\***   set integer data to 0
\***   CALL DM.DISPD function to display the required screen
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to -30 (first output field)
\***   CALL DM.NEXTF function to position the cursor accordingly
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to current page
\***   set integer data to 0
\***   CALL DM.PUTF function to put current page in the field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 3 (next output field)
\***   CALL DM.NEXTF function to position the cursor accordingly
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to total pages
\***   set integer data to 0
\***   CALL DM.PUTF function to put total pages in the field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 3 (next output field)
\***   CALL DM.NEXTF function to position the cursor accordingly
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\...............................................................................

   SHOW.PAGE:

      IF INC.DEC$ = "D" THEN                                           \
         STRING.DATA$ = "B7202"                                       :\
      ELSE                                                             \
         STRING.DATA$ = "B7203"

      INTEGER.DATA% = 0
      RC% EQ DM.DISPD(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = -30
      RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = STR$ (CURRENT.PAGE%)
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = 3
      RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = STR$ (TOTAL.PAGES%)
      INTEGER.DATA% = 0
      RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = 3
      RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

\...............................................................................
\***
\***   set current entry to 1
\***
\***   WHILE current entry is less than 27
\***   AND ((current page - 1) * 26) + current entry is <= total entries
\***
\***      IF current entry is 14 THEN
\***         set string data to spaces
\***         set integer data to -20 (first input field)
\***         CALL DM.NEXTF function to position the cursor accordingly
\***         IF F03.RETURN.CODE% <> 0 THEN
\***            GOTO CHAIN.OUT
\***         ELSE
\***            set string data to spaces
\***            set integer data to 3 (next output field)
\***            CALL DM.NEXTF function to position the cursor accordingly
\***            IF F03.RETURN.CODE% <> 0 THEN
\***               GOTO CHAIN.OUT
\***            endif
\***         endif
\***      ELSE
\***         IF current entry > 1 THEN
\***            GOSUB MOVE.NEXT.LINE
\***         endif
\***      endif
\***
\***      set display entry to ((current page -1) * 26) + current entry
\***
\***      IF display table(display entry) rpd number is 0
\***      AND display table(display entry) rpd date is 000000 THEN
\***         set string data to 99999
\***      ELSE
\***         set string data to display table(display entry) rpd number
\***      endif
\***
\***      set integer data to 0
\***      CALL DM.PUTF function to put the rpd number in the field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 3 (next output field)
\***      CALL DM.NEXTF function to position the cursor accordingly
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to display table(display entry) lines
\***      set integer data to 0
\***      CALL DM.PUTF function to put lines in the field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 3 (next output field)
\***      CALL DM.NEXTF function to position the cursor accordingly
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      IF display table(display entry) date is 0 THEN
\***         set string data to current date (DDMMYY)
\***      ELSE
\***         set string data to display table(display entry) date (DDMMYY)
\***      endif
\***
\***      set integer data to 0
\***      CALL DM.PUTF function to put date in the field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 2 (next input field)
\***      CALL DM.NEXTF function to position the cursor accordingly
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to the increase decrease flag in the table
\***      set integer data to 0
\***      CALL DM.PUTF function to put flag in the field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      increment current entry by 1
\***
\***   WEND
\***
\...............................................................................

      CURRENT.ENTRY% = 1

      WHILE CURRENT.ENTRY% < 27                                        \
      AND ((CURRENT.PAGE% - 1) * 26) + CURRENT.ENTRY% <= TOTAL.ENTRIES%

         IF CURRENT.ENTRY% = 14 THEN                                   \
            STRING.DATA$ = ""                                         :\
            INTEGER.DATA% = -20                                       :\
            RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)               :\DJSW
            IF RC% NE 0 THEN GOTO CHAIN.OUT                           :\DSJW
            ELSE                                                       \
               STRING.DATA$ = ""                                      :\
               INTEGER.DATA% = 3                                      :\
               RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)            :\DJSW
               IF RC% NE 0 THEN GOTO CHAIN.OUT                        :\DSJW
               ELSE                                                    \
         ELSE                                                          \
            IF CURRENT.ENTRY% > 1 THEN                                 \
               GOSUB MOVE.NEXT.LINE

         DISPLAY.ENTRY% = ((CURRENT.PAGE% - 1) * 26) + CURRENT.ENTRY%

         RPD.NUMBER% = VAL(UNPACK$(MID$(DISPLAY.TABLE$                 \
                           (DISPLAY.ENTRY%),4,3)))
         TABLE.DATE$ = UNPACK$(LEFT$ (DISPLAY.TABLE$                   \
                               (DISPLAY.ENTRY%), 3))
         IF RPD.NUMBER% = 0                                            \
         AND TABLE.DATE$ = "000000" THEN                               \
            STRING.DATA$ = "99999"                                    :\
         ELSE                                                          \
            STRING.DATA$ = STR$ (RPD.NUMBER%)

         INTEGER.DATA% = 0
         RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = ""
         INTEGER.DATA% = 3
         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = UNPACK$(MID$ (DISPLAY.TABLE$ (DISPLAY.ENTRY%), 7, 2))
         INTEGER.DATA% = 0
         RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = ""
         INTEGER.DATA% = 3
         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         IF TABLE.DATE$ = "000000" THEN                                \
            STRING.DATA$ = RIGHT$ (DATE$, 2) + "/"                     \
                         + MID$ (DATE$, 3, 2) + "/"                    \
                         + LEFT$ (DATE$, 2)                           :\
         ELSE                                                          \
            STRING.DATA$ = RIGHT$ (TABLE.DATE$, 2) + "/"               \
                         + MID$ (TABLE.DATE$, 3, 2) + "/"              \
                         + LEFT$ (TABLE.DATE$, 2)

         INTEGER.DATA% = 0
         RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = ""
         INTEGER.DATA% = 2
         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = RIGHT$(DISPLAY.TABLE$ (DISPLAY.ENTRY%), 1)
         INTEGER.DATA% = 0
         RC% EQ DM.PUTF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         CURRENT.ENTRY% = CURRENT.ENTRY% + 1

      WEND

\...............................................................................
\***
\***   set string data to spaces
\***   set integer data to 0
\***   CALL DM.POSF function to obtain the value of last input field number
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set number on page to current entry - 1
\***   set nextf parameter to -20 (first input field)
\***   set current entry to 1
\***   set display entry to ((current page - 1) * 26) + current entry
\***
\***   RETURN
\***
\...............................................................................

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                        !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW
      LAST.INPUT.FIELD.NO% = F03.RETURNED.INTEGER%

      NUM.ON.PAGE% = CURRENT.ENTRY% - 1
      NEXTF.PARM% = -20
      CURRENT.ENTRY% = 1
      DISPLAY.ENTRY% = ((CURRENT.ENTRY% - 1) * 26) + CURRENT.ENTRY%

! NEW CODE
      IF CURRENT.PAGE% = 1 \             ! Re-display message 265 on  ! 1.20 RC
        AND MESSAGE.NO% = 265 THEN BEGIN ! entry to selection screee  ! 1.20 RC
          GOSUB DISPLAY.BEMF.MSG.265                                  ! 1.20 RC
          MESSAGE.NO% = 0 ! Prevents redisplay                        ! 1.20 RC
      ENDIF                                                           ! 1.20 RC

   RETURN


\*******************************************************************************
\***
\***   MOVE.NEXT.LINE:
\***
\***   set string data to spaces
\***   set integer data to 3 (next output field)
\***
\***   FOR count from 1 to 4
\***      CALL DM.NEXTF function to position the cursor accordingly
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***   NEXT count
\***
\***   RETURN
\***
\...............................................................................

   MOVE.NEXT.LINE:

      STRING.DATA$ = ""
      INTEGER.DATA% = 3

      FOR COUNT% = 1 TO 4

         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

      NEXT COUNT%

   RETURN

\*******************************************************************************
\***
\***   CHECK.FOR.IRF.UPDATE:
\***
\***   set entry to 1
\***   set end loop flag to "N"
\***
\***   WHILE end loop flag = "N"
\***
\***      IF entry > total entries THEN
\***         set end loop flag to "Y"
\***      ELSE
\***         IF display table(entry) rpd/date = ppfi rpd/date THEN
\***            set end loop flag to "Y"
\***         ELSE
\***            increment entry by 1
\***         endif
\***      endif
\***
\***   WEND
\***
\***   IF entry greater than  total entries THEN
\***      CALL WRITE.PPFO to write out ppfi record
\***      increment ppfo record count by 1
\***   ELSE
\***      IF display table (entry) flag <> increase decrease flag THEN
\***         CALL WRITE.PPFO to write out ppfi record
\***         increment ppfo record count by 1
\***      ELSE
\***         GOSUB UPDATE.IRF
\***      endif
\***   endif
\***
\***   RETURN
\***
\...............................................................................

   CHECK.FOR.IRF.UPDATE:

      INDEX% = 1
      END.LOOP$ = "N"

      WHILE END.LOOP$ = "N"

         IF INDEX% > TOTAL.ENTRIES% THEN BEGIN                          !DSJW
            END.LOOP$ = "Y"                                             !DSJW
         ENDIF ELSE BEGIN                                               !DSJW
            IF LEFT$(DISPLAY.TABLE$(INDEX%),6) = PACK$("000000000000") \!DSJW
            AND PPF.RPD.NO$ = "99999" THEN BEGIN                        !DSJW
               END.LOOP$ = "Y"                                          !DSJW
            ENDIF ELSE BEGIN                                            !DSJW
               IF LEFT$(DISPLAY.TABLE$(INDEX%),6) = \                   !DSJW
                  PACK$(PPF.DATE.DUE$ + "0" + PPF.RPD.NO$) THEN BEGIN   !DSJW
                  END.LOOP$ = "Y"                                       !DSJW
               ENDIF ELSE BEGIN                                         !DSJW
                  INDEX% = INDEX% + 1                                   !DSJW
               ENDIF                                                    !DSJW
            ENDIF                                                       !DSJW
         ENDIF                                                          !DSJW

       WEND

       IF INDEX% > TOTAL.ENTRIES% THEN BEGIN                            !DSJW
          FAIL.TYPE$ = "W"                                              !DSJW
          RC% EQ WRITE.PPFO                                             !DSJW
		  PRINT #992 ; "ADDED FROM LINE 3319" , PPF.RECORD$
          IF RC% NE 0 THEN GOTO FILE.ERROR                              !DSJW
          PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                   !DSJW
       ENDIF ELSE BEGIN                                                 !DSJW
          DISP.TABLE.FLAG$ = RIGHT$(DISPLAY.TABLE$(INDEX%),1)           !DSJW
          IF DISP.TABLE.FLAG$ <> INC.DEC$ THEN BEGIN                    !DSJW
             FAIL.TYPE$ = "W"                                           !DSJW
             RC% EQ WRITE.PPFO                                          !DSJW
			 PRINT #992 ; "ADDED FROM LINE 3327 " , PPF.RECORD$
             IF RC% NE 0 THEN GOTO FILE.ERROR                           !DSJW
             PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                !DSJW
          ENDIF ELSE BEGIN                                              !DSJW
             GOSUB UPDATE.IRF                                           !DSJW
          ENDIF                                                         !DSJW
       ENDIF                                                            !DSJW

   RETURN

\*******************************************************************************
\***
\***   UPDATE.IRF:
\***
\***   IF END occurs on read idf THEN READ.IDF.ERROR
\***   CALL READ.IDF using ppfi boots code as key
\***
\***   set group code flag to idf bit flags 1 and group code mask
\***
\***   set saved idf Boots code to
\***     middle six numbers of IDF.BOOTS.CODE$ packed
\***
\***   IF group code flag is not 0 THEN
\***      set bar code front end to "2000000000" packed
\***   ELSE
\***      set bar code front end to "0000000000" packed
\***   endif
\***
\***   IF idf first ean code = 0 THEN
\***      GOSUB ZERO.IDF.BAR.CODE
\***      GOTO END.UPDATE.IRF
\***   endif
\***
\***   IF END occurs on read irf THEN READ.IRF.ERROR
\***   CALL READ.IRF using bar code front end + idf first ean code as key,
\***                 using autolock
\***
\***   set first ean flag on
\***   set match found flag to "N"
\***   set update failed flag off
\***
\***   IF IRF.BOOTS.CODE$ = saved Boots code THEN
\***      set match found flag to "Y"
\***      GOSUB DO.UPDATE
\***   endif
\***
\***   GOSUB WRITE.IRF.REC
\***
\***   IF update failed flag is on THEN
\***      CALL WRITE.PPFO to write ppfi record
\***      increment ppfo record count by 1
\***      RETURN
\***   endif
\***
\...............................................................................

   UPDATE.IRF:

      IDF.BOOTS.CODE$ = PACK$("0" + PPF.BOOTS.CODE$)
      RC% EQ READ.IDF                                                   !DSJW
      IF RC% NE 0 THEN GOTO READ.IDF.ERROR                              !DSJW

      GROUP.CODE.FLAG% = IDF.BIT.FLAGS.1% AND BIT.7.MASK%

      SAVED.IDF.BOOTS.CODE$ = PACK$(MID$(UNPACK$(IDF.BOOTS.CODE$),2,6))

      IF GROUP.CODE.FLAG% <> 0 THEN                                     \
         BAR.CODE.FRONT$ = PACK$ ("2000000000")                        :\
      ELSE                                                              \
         BAR.CODE.FRONT$ = PACK$ ("0000000000")

      TOTAL.BAR.CODE.COUNT% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))
\ line deleted from here
      IDF.BAR.CODE$ = UNPACK$ (IDF.FIRST.BAR.CODE$)
      IF IDF.BAR.CODE$ = "000000000000" THEN                           \
         GOSUB ZERO.IDF.BAR.CODE                                      :\
         GOTO END.UPDATE.IRF

      IRF.BAR.CODE$ = BAR.CODE.FRONT$ + IDF.FIRST.BAR.CODE$
      RC% EQ READ.IRF.LOCK                                              !DSJW
      IF RC% NE 0 THEN GOTO READ.IRF.ERROR                              !DSJW

      FIRST.EAN$ = "Y"
      MATCH.FOUND$ = "N"
      UPDATE.FAILED$ = "N"

      IF IRF.BOOTS.CODE$ = SAVED.IDF.BOOTS.CODE$ THEN                  \
         MATCH.FOUND$ = "Y"                                           :\
         GOSUB DO.UPDATE

      GOSUB WRITE.IRF.REC

      IF UPDATE.FAILED$ = "Y" THEN BEGIN                                !DSJW
         FAIL.TYPE$ = "W"                                               !DSJW
         RC% EQ WRITE.PPFO                                              !DSJW
		 PRINT #992 ; "ADDED FROM LINE 3422 " , PPF.RECORD$
         IF RC% NE 0 THEN GOTO FILE.ERROR                               !DSJW
         PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                    !DSJW
         RETURN
      ENDIF                                                             !DSJW

\...............................................................................
\***
\***   set idf group ean code to idf first ean code
\***   IF group code flag is not 0 THEN
\***      GOSUB GROUP.CODE.UPDATE
\***   endif
\***
\***   IF idf number of eans is 1 THEN
\***      IF match found flag is "N" THEN
\***         GOSUB UNMATCHED.CODE.ERR
\***         CALL WRITE.PPFO to write ppfi record
\***         increment ppfo record count by 1
\***         RETURN
\***      ELSE
\***         RETURN
\***      endif
\***   endif
\***
\***   IF idf second ean code = 0 THEN
\***      GOSUB ZERO.IDF.BAR.CODE
\***      GOTO END.UPDATE.IRF
\***   endif
\***
\***   IF IRF.BOOTS.CODE$ = saved Boots code THEN
\***      set match found flag to "Y"
\***      GOSUB DO.UPDATE
\***   endif
\***
\***   GOSUB WRITE.IRF.REC
\***
\***   set idf group ean code to idf second ean code
\***   IF group code flag is not 0 THEN
\***      GOSUB GROUP.CODE.UPDATE
\***   endif
\***
\***   IF idf number of eans is 2 THEN
\***      IF match found flag is "N" THEN
\***         GOSUB UNMATCHED.CODE.ERR
\***         CALL WRITE.PPFO to write ppfi record
\***         increment ppfo record count by 1
\***         RETURN
\***      ELSE
\***         RETURN
\***      endif
\***   endif
\***
\***   CALL READ.NEXT.IEF function passing irf bar code
\***   IF F11.RETURN.CODE% <> 0 THEN
\***      CALL WRITE.PPFO to write out the ppfi record
\***      increment ppfo record count by 1
\***      RETURN
\***   endif
\***
\***   WHILE F11.NEXT.BAR.CODE$ is not "0"
\***
\***      IF END occurs on read irf THEN READ.IRF.ERROR
\***      CALL READ.IRF using F11.NEXT.BAR.CODE$ as key using autolock
\***
\***      IF IRF.BOOTS.CODE$ = saved Boots code THEN
\***         set match found flag to "Y"
\***         GOSUB DO.UPDATE
\***      endif
\***
\***      GOSUB WRITE.IRF.REC
\***
\***      set idf group ean code to F11.NEXT.BAR.CODE$
\***      IF group code flag is not 0 THEN
\***         GOSUB GROUP.CODE.UPDATE
\***      endif
\***
\***      CALL READ.NEXT.IEF function
\***      IF F11.RETURN.CODE% <> 0 THEN
\***         CALL WRITE.PPFO to write out the ppfi record
\***         increment ppfo record count by 1
\***         RETURN
\***      endif
\***
\***   WEND
\***
\***   IF match found flag is "N" THEN
\***      GOSUB UNMATCHED.CODE.ERR
\***      CALL WRITE.PPFO to write ppfi record
\***      increment ppfo record count by 1
\***      RETURN
\***   endif
\***
\***   END.UPDATE.IRF:
\***
\***   RETURN
\***
\...............................................................................

      IDF.GROUP.EAN.CODE$ = IDF.FIRST.BAR.CODE$
      IF GROUP.CODE.FLAG% <> 0 THEN                                    \
         GOSUB GROUP.CODE.UPDATE

      IF TOTAL.BAR.CODE.COUNT% = 1 THEN BEGIN                           !DSJW
         IF MATCH.FOUND$ = "N" THEN BEGIN                               !DSJW
            GOSUB UNMATCHED.CODE.ERR                                    !DSJW
            FAIL.TYPE$ = "W"                                            !DSJW
            RC% EQ WRITE.PPFO                                           !DSJW
			PRINT #992 ; "ADDED FROM LINE 3529" , PPF.RECORD$
			
            IF RC% NE 0 THEN GOTO FILE.ERROR                            !DSJW
            PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                 !DSJW
            RETURN                                                      !DSJW
         ENDIF ELSE BEGIN                                               !DSJW
            RETURN                                                      !DSJW
         ENDIF                                                          !DSJW
      ENDIF                                                             !DSJW

      IDF.BAR.CODE$ = UNPACK$ (IDF.SECOND.BAR.CODE$)
      IF IDF.BAR.CODE$ = "000000000000" THEN                           \
         GOSUB ZERO.IDF.BAR.CODE                                      :\
         GOTO END.UPDATE.IRF

      IRF.BAR.CODE$ = BAR.CODE.FRONT$ + IDF.SECOND.BAR.CODE$
      RC% EQ READ.IRF.LOCK                                              !DSJW
      IF RC% NE 0 THEN GOTO READ.IRF.ERROR                              !DSJW

\ line deleted from here

      IF IRF.BOOTS.CODE$ = SAVED.IDF.BOOTS.CODE$ THEN                  \
         MATCH.FOUND$ = "Y"                                           :\
         GOSUB DO.UPDATE

      GOSUB WRITE.IRF.REC

      IDF.GROUP.EAN.CODE$ = IDF.SECOND.BAR.CODE$
      IF GROUP.CODE.FLAG% <> 0 THEN                                    \
         GOSUB GROUP.CODE.UPDATE

      IF TOTAL.BAR.CODE.COUNT% = 2 THEN BEGIN                           !DSJW
         IF MATCH.FOUND$ = "N" THEN BEGIN                               !DSJW
            GOSUB UNMATCHED.CODE.ERR                                    !DSJW
            FAIL.TYPE$ = "W"                                            !DSJW
            RC% EQ WRITE.PPFO                                           !DSJW
			PRINT #992 ; "ADDED FROM LINE 3565" , PPF.RECORD$
            IF RC% NE 0 THEN GOTO FILE.ERROR                            !DSJW
            PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                 !DSJW
            RETURN                                                      !DSJW
         ENDIF ELSE BEGIN                                               !DSJW
            RETURN                                                      !DSJW
         ENDIF                                                          !DSJW
      ENDIF                                                             !DSJW

      F11.CURRENT.COUNT% = 0
      TOTAL.BAR.CODE.COUNT% = TOTAL.BAR.CODE.COUNT% - 1

\ REM the above line is due to PSBF11 performing a check on IEF records
\ REM NOT number of bar codes.

      F11.NEXT.BAR.CODE$ = PACK$("999999999999")
      CURRENT.BOOTS.CODE$ = PPF.BOOTS.CODE$                             !CSJW
      CURRENT.IEF.BAR.CODE$ = RIGHT$(IRF.BAR.CODE$,6)
      RC% EQ READ.NEXT.IEF (CURRENT.IEF.BAR.CODE$,                      \DSJW
                            TOTAL.BAR.CODE.COUNT%,                      \DSJW
                            CURRENT.BOOTS.CODE$)
      IF RC% NE 0 THEN BEGIN                                            !DSJW
         FAIL.TYPE$ = "W"                                               !DSJW
         RC% EQ WRITE.PPFO                                              !DSJW
		 PRINT #992 ; "ADDED FROM LINE 3589" , PPF.RECORD$
         IF RC% NE 0 THEN GOTO FILE.ERROR                               !DSJW
         PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                    !DSJW
         RETURN                                                         !DSJW
      ENDIF                                                             !DSJW

      WHILE VAL(UNPACK$(F11.NEXT.BAR.CODE$)) <> 0

         IRF.BAR.CODE$ = BAR.CODE.FRONT$ + F11.NEXT.BAR.CODE$
         RC% EQ READ.IRF.LOCK                                           !DSJW
         IF RC% NE 0 THEN GOTO READ.IRF.ERROR                           !DSJW

         IF IRF.BOOTS.CODE$ = SAVED.IDF.BOOTS.CODE$ THEN               \
            MATCH.FOUND$ = "Y"                                        :\
            GOSUB DO.UPDATE

         GOSUB WRITE.IRF.REC

         IDF.GROUP.EAN.CODE$ = F11.NEXT.BAR.CODE$
         IF GROUP.CODE.FLAG% <> 0 THEN                                 \
            GOSUB GROUP.CODE.UPDATE


         CURRENT.IEF.BAR.CODE$ = RIGHT$(IRF.BAR.CODE$,6)
         RC% EQ READ.NEXT.IEF (CURRENT.IEF.BAR.CODE$,                   \DSJW
                               TOTAL.BAR.CODE.COUNT%,                   \DSJW
                               CURRENT.BOOTS.CODE$)
         IF RC% NE 0 THEN BEGIN                                         !DSJW
            FAIL.TYPE$ = "W"                                            !DSJW
            RC% EQ WRITE.PPFO                                           !DSJW
			PRINT #992 ; "ADDED FROM LINE 3619" , PPF.RECORD$
            IF RC% NE 0 THEN GOTO FILE.ERROR                            !DSJW
            PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                 !DSJW
            RETURN                                                      !DSJW
         ENDIF                                                          !DSJW

      WEND

      IF MATCH.FOUND$ = "N" THEN BEGIN                                  !DSJW
         GOSUB UNMATCHED.CODE.ERR                                       !DSJW
         FAIL.TYPE$ = "W"                                               !DSJW
         RC% EQ WRITE.PPFO                                              !DSJW
		 PRINT #992 ; "ADDED FROM LINE 3631" , PPF.RECORD$
         IF RC% NE 0 THEN GOTO FILE.ERROR                               !DSJW
         PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                    !DSJW
         RETURN                                                         !DSJW
      ENDIF ELSE BEGIN                                                  !DSJW
         RETURN                                                         !DSJW
      ENDIF                                                             !DSJW

   END.UPDATE.IRF:

   RETURN

\*******************************************************************************
\***
\***   GROUP.CODE.UPDATE:
\***
\***   set bar code grp front end to "0000000000" packed
\***   IF END occurs on read irf THEN READ.IRF.ERROR
\***   CALL READ.IRF using bar code grp front end + idf group ean code as key,
\***                 using autolock
\***
\***   IF IRF.BOOTS.CODE$ = saved Boots code THEN
\***      GOSUB DO.UPDATE
\***   ELSE
\***      CALL APPLICATION.LOG function to log event 33,
\***            message number 557
\***      CALL WRITE.PPFO to write ppfi record
\***      increment ppfo record count by 1
\***      RETURN
\***   endif
\***
\***   GOSUB WRITE.IRF.REC
\***
\***   RETURN
\***
\...............................................................................

   GROUP.CODE.UPDATE:

      BAR.CODE.FRONT$ = PACK$("0000000000")
      IRF.BAR.CODE$ = BAR.CODE.FRONT$ + IDF.GROUP.EAN.CODE$
      RC% EQ READ.IRF.LOCK                                              !DSJW
      IF RC% NE 0 THEN GOTO READ.IRF.ERROR                              !DSJW

      IF IRF.BOOTS.CODE$ = SAVED.IDF.BOOTS.CODE$ THEN                   \
         GOSUB DO.UPDATE                                               :\
      ELSE  BEGIN
         MESSAGE.NO% = 557
         EVENT.NO% = 33
         VAR.STRING.1$ = IDF.GROUP.EAN.CODE$ + PACK$("00000000")
         VAR.STRING.2$ = UNPACK$(IDF.GROUP.EAN.CODE$)
         IF PSBCHN.APP = "PSB51" THEN                                   \
            PSBCHN.U2 = PSBCHN.U2 + "557" + VAR.STRING.1$              :\
            BATCH.SCREEN.FLAG$ = "B"
         RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,             \DSJW
                                 VAR.STRING.2$,EVENT.NO%)               !DSJW
         BATCH.SCREEN.FLAG$ = "S"

         GOSUB RESTORE.MESSAGE                                          ! BMW

         FAIL.TYPE$ = "W"                                               !DSJW
         RC% EQ WRITE.PPFO                                              !DSJW
		 PRINT #992 ; "ADDED FROM LINE 3693" , PPF.RECORD$
         IF RC% NE 0 THEN GOTO FILE.ERROR                               !DSJW
         PPFO.RECORD.COUNT% = PPFO.RECORD.COUNT% + 1                    !DSJW
         RETURN
      ENDIF

      GOSUB WRITE.IRF.REC

   RETURN

\******************************************************************************
\***
\***   UNMATCHED.CODE.ERR:
\***
\***   CALL APPLICATION.LOG function to log event 32,
\***         message number 556
\***
\***   RETURN
\***
\...............................................................................

   UNMATCHED.CODE.ERR:

      MESSAGE.NO% = 556
      EVENT.NO% = 32
      VAR.STRING.1$ = RIGHT$(UNPACK$(IDF.BOOTS.CODE$),7)
      VAR.STRING.2$ = VAR.STRING.1$ + "   "                             ! BMW
      IF PSBCHN.APP = "PSB51" THEN                                      \ BMW
         PSBCHN.U2 = PSBCHN.U2 + "556" + VAR.STRING.2$                 :\ BMW
         BATCH.SCREEN.FLAG$ = "B"                                       ! BMW

      RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,                \DSJW
                              VAR.STRING.2$,EVENT.NO%)                  !DSJW

      BATCH.SCREEN.FLAG$ = "S"                                          ! BMW

      GOSUB RESTORE.MESSAGE                                             ! BMW

      RETURN

\*******************************************************************************
\***
\***   CHECK.PPFI.TRAILER:
\***
\***   set ppfi trailer error flag to "N"
\***   IF ppfi boots code is 9999999 (trailer) THEN
\***      IF ppfi record count is not ppfi trailer record count THEN
\***         CALL APPLICATION.LOG function to log event 5, message 602
\***         GOTO CHAIN.OUT
\***      ELSE
\***         IF END occurs on read ppfi THEN NO.RECORDS.AFTER.TRAILER
\***         CALL READ.PPFI
\***         CALL APPLICATION.LOG function to log event 17
\***         set ppfi trailer error flag to "Y"
\***      endif
\***   endif
\***
\***   NO.RECORDS.AFTER.TRAILER:
\***
\***   RETURN
\***
\...............................................................................

   CHECK.PPFI.TRAILER:

      PPFI.TRAILER.ERROR$ = "N"
      PPFI.BOOTS.CODE$ = PPF.BOOTS.CODE$                               !CSJW
      PPFI.TRAILER.COUNT% = VAL(PPF.REC.COUNT$)

      IF PPFI.BOOTS.CODE$ = "9999999" THEN                             \CSJW
         IF PPFI.RECORD.COUNT% <> PPFI.TRAILER.COUNT% THEN             \
            EVENT.NO% = 5                                             :\
            MESSAGE.NO% = 602                                         :\
            VAR.STRING.1$ = "I" + CHR$(SHIFT(PPFI.REPORT.NUM%,8)) \     !DSJW
                                + CHR$(SHIFT(PPFI.REPORT.NUM%,0))     :\!DSJW
            INTEGER.4% = PPFI.RECORD.COUNT%                           :\
            RC% EQ CONV.TO.STRING (EVENT.NO%,INTEGER.4%)              :\DSJW
            IF RC% = 0 THEN                                            \DSJW
               VAR.STRING.1$ = VAR.STRING.1$ +                         \
                               RIGHT$(F17.RETURNED.STRING$,2)         :\
               INTEGER.4% = PPFI.TRAILER.COUNT%                       :\
               RC% EQ CONV.TO.STRING (EVENT.NO%,INTEGER.4%)           :\DSJW
               IF RC% = 0 THEN BEGIN
                  VAR.STRING.1$ = VAR.STRING.1$ +                      \
                                  RIGHT$ (F17.RETURNED.STRING$,2)      !DSJW
                  VAR.STRING.2$ =                                      \
                      RIGHT$ ("     " + STR$(PPFI.RECORD.COUNT%), 5) + \
                      RIGHT$ ("     " + STR$(PPFI.TRAILER.COUNT%), 5)
                  IF PSBCHN.APP = "PSB51" THEN                         \
                     PSBCHN.U2 = PSBCHN.U2 + "602" + VAR.STRING.2$    :\
                     BATCH.SCREEN.FLAG$ = "B"
                  RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,   \DSJW
                                          VAR.STRING.2$,EVENT.NO%)     !DSJW
                  BATCH.SCREEN.FLAG$ = "S"
                  GOTO CHAIN.OUT
                  ENDIF                                                \
               ELSE                                                    \
                  GOTO CHAIN.OUT                                       \
            ELSE                                                       \
               GOTO CHAIN.OUT                                          \
         ELSE  BEGIN
            RC% EQ READ.PPFI                                            !DSJW
            IF RC% NE 0 THEN GOTO NO.RECORDS.AFTER.TRAILER              !DSJW
            EVENT.NO% = 17
            MESSAGE.NO% = 555
            VAR.STRING.1$ = "I" + CHR$(SHIFT(PPFI.REPORT.NUM%,8)) \     !DSJW
                                + CHR$(SHIFT(PPFI.REPORT.NUM%,0)) \     !DSJW
                                + PACK$("00000000") \                   !DSJW
                                + PACK$("0" + PPF.BOOTS.CODE$)          !DSJW
            VAR.STRING.2$ = LEFT$("0" + PPF.BOOTS.CODE$ \               !CSJW
                                  + "        ",10)                             ! BMW
            IF PSBCHN.APP = "PSB51" THEN                               \
               PSBCHN.U2 = PSBCHN.U2 +                                 \ BMW
                           "555" + VAR.STRING.2$                      :\ BMW
               BATCH.SCREEN.FLAG$ = "B"
            RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,          \DSJW
                                    VAR.STRING.2$,EVENT.NO%)            !DSJW
            PPFI.TRAILER.ERROR$ = "Y"
            BATCH.SCREEN.FLAG$ = "S"
            ENDIF

   NO.RECORDS.AFTER.TRAILER:

   RETURN

\*******************************************************************************
\***
\***   READ.IDF.ERROR:
\***
\***   Log Event 106
\***   NB:
\***    - Do not display message
\***    - Do not report error to PSB51 (Opening Log)
\***
\***   GOTO END.UPDATE.IRF
\***
\...............................................................................

   READ.IDF.ERROR:

      CURRENT.SESS.NUM% = IDF.SESS.NUM%
      CURRENT.KEY$ = PACK$("0" + PPF.BOOTS.CODE$)                       !CSJW
      FAIL.TYPE$ = "R"

      SB.ACTION$ = "R"                                                  !DSJW
      SB.INTEGER% = CURRENT.SESS.NUM%                                   !DSJW
      GOSUB SB.FILE.UTILS                                               !DSJW

      VAR.STRING.2$ = ""                                                !DSJW
      MESSAGE.NO% = 0                                                   !DSJW
      BATCH.SCREEN.FLAG$ = "B"                                          !DSJW
      VAR.STRING.1$ = FAIL.TYPE$ + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) \    !DSJW
                                 + CHR$(SHIFT(SB.FILE.REP.NUM%,0)) \    !DSJW
                                 + RIGHT$(PACK$("00000000000000") \     !DSJW
                                 + CURRENT.KEY$,7)                      !DSJW
      EVENT.NO% = 106                                                   !DSJW
      RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$, \              !DSJW
                              VAR.STRING.2$,EVENT.NO%)                  !DSJW

      BATCH.SCREEN.FLAG$ = "S"                                          !DSJW

      GOTO END.UPDATE.IRF

\*******************************************************************************
\***
\***   READ.IRF.ERROR:
\***
\***   Log Event 106
\***   NB:
\***    - Do not write PPFI rec to PPFO
\***    - Do not display message
\***    - Do not report error to PSB51 (Opening Log)
\***
\***   GOTO END.UPDATE.IRF
\***
\...............................................................................

   READ.IRF.ERROR:

      CURRENT.SESS.NUM% = IRF.SESS.NUM%
      FAIL.TYPE$ = "R"
      CURRENT.KEY$ = IRF.BAR.CODE$

      SB.ACTION$ = "R"                                                  !DSJW
      SB.INTEGER% = CURRENT.SESS.NUM%                                   !DSJW
      GOSUB SB.FILE.UTILS                                               !DSJW

      VAR.STRING.2$ = ""                                                !DSJW
      MESSAGE.NO% = 0                                                   !DSJW
      BATCH.SCREEN.FLAG$ = "B"                                          !DSJW
      VAR.STRING.1$ = FAIL.TYPE$ + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) \    !DSJW
                                 + CHR$(SHIFT(SB.FILE.REP.NUM%,0)) \    !DSJW
                                 + RIGHT$(PACK$("00000000000000") \     !DSJW
                                 + CURRENT.KEY$,7)                      !DSJW
      EVENT.NO% = 106                                                   !DSJW
      RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$, \              !DSJW
                              VAR.STRING.2$,EVENT.NO%)                  !DSJW

      BATCH.SCREEN.FLAG$ = "S"                                          !DSJW

      GOTO END.UPDATE.IRF

\*******************************************************************************
\***
\***   CREATE.ERROR:
\***
\***   set fail type to "C"
\***   set key to null
\***
\***   GOSUB FILE.ERROR
\***
\***   GOTO CHAIN.OUT
\***
\...............................................................................

   CREATE.ERROR:

      FAIL.TYPE$ = "C"
      CURRENT.KEY$ = ""

      GOSUB FILE.ERROR

      GOTO CHAIN.OUT

\*****************************************************************************
\***
\***   WRITE.IRF.REC:
\***
\***   The points value of the item (for Customer Card redemption) is set to
\***   the default value of zero.  If the item is eligible for redemption and
\***   the price value is small enough to fit into a 2 byte integer, the price
\***   is written to the points field.
\***
\***   CALL UPDT.IRF.UPDT to write out the modified irf record using autounlock,
\***                      and update TIF/TMCF
\***
\***   RETURN
\***
\...............................................................................

   WRITE.IRF.REC:

!      IRF.POINTS% = 0                                                  ! FAW   !IBG

!      IF (IRF.INDICAT3% AND 04H) = 04H THEN BEGIN                      ! FAW   !IBG

!         REDEEM.POINTS% = VAL(UNPACK$(IRF.SALEPRIC$))                  ! FAW   !IBG

!         IF REDEEM.POINTS% < 8000H THEN BEGIN                          ! FAW   !IBG

!            IRF.POINTS% = REDEEM.POINTS%                               ! FAW   !IBG

!         ENDIF                                                         ! FAW   !IBG

!      ENDIF                                                            ! FAW   !IBG

!      NEW.IRF.DATA$ = IRF.BAR.CODE$       +                            \       !IBG
!                      CHR$(IRF.INDICAT0%) +                            \       !IBG
!                      CHR$(IRF.INDICAT1%) +                            \       !IBG
!                      CHR$(IRF.INDICAT2%) +                            \ HSWM  !IBG
!                      IRF.DEAL.NUM$       +                            \       !IBG
!                      CHR$(IRF.INDICAT4%) +                            \       !IBG
!                      IRF.SALEQUAN$       +                            \       !IBG
!                      IRF.SALEPRIC$       +                            \       !IBG
!                      CHR$(IRF.INDICAT5%) +                            \ GSB   !IBG
!                      IRF.ITEMNAME$       +                            \       !IBG
!                      IRF.BOOTS.CODE$     +                            \       !IBG
!                      IRF.DEAL.SAVING$    +                            \       !IBG
!                      CHR$(IRF.POINTS%  AND 0FFH)          +           \ FAW   !IBG
!                      CHR$(SHIFT(IRF.POINTS%, 8) AND 0FFH) +           \ FAW   !IBG
!                      CHR$(IRF.INDICAT3%)

      ACD.FLAG$ = "CHANGE"
      
      CALL CONCAT.NEW.IRF.DATA$                                                 !IBG

      RC% EQ UPDT.IRF.UPDT (NEW.IRF.DATA$,ACD.FLAG$,IRF.LOCKED.FLAG$)   !DSJW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

   ! 1 line deleted from here                                           ! BMW

   RETURN

   ! Subroutine deleted from here !!!!!                                 ! BMW

\*******************************************************************************
\***
\***   ZERO.IDF.BAR.CODE:
\***
\***   CALL APPLICATION.LOG to log event number 5, message number 603
\***
\***   RETURN
\***
\...............................................................................

   ZERO.IDF.BAR.CODE:

      VAR.STRING.1$ = "I" + CHR$(IDF.REPORT.NUM%)                       ! BMW
      EVENT.NO% = 5
      INTEGER.4% = F11.CURRENT.COUNT%
      RC% EQ CONV.TO.STRING (EVENT.NO%,INTEGER.4%)                      !DSJW
      IF RC% NE 0 THEN RETURN                                           !DSJW

      VAR.STRING.1$ = VAR.STRING.1$ + RIGHT$(F17.RETURNED.STRING$,2)

      INTEGER.4% = TOTAL.BAR.CODE.COUNT%
      RC% EQ CONV.TO.STRING (EVENT.NO%,INTEGER.4%)                      !DSJW
      IF RC% NE 0 THEN RETURN                                           !DSJW

      VAR.STRING.1$ = VAR.STRING.1$ +                                  \
                      RIGHT$(F17.RETURNED.STRING$,2) +                 \
                      IDF.BOOTS.CODE$

      VAR.STRING.2$ = RIGHT$ (STR$(F11.CURRENT.COUNT%), 1) +           \
                      RIGHT$ ("    " + STR$(TOTAL.BAR.CODE.COUNT%), 4)
      MESSAGE.NO% = 603

      IF PSBCHN.APP = "PSB51" THEN                                     \
         PSBCHN.U2 = PSBCHN.U2 + "603" + LEFT$(                        \
                      UNPACK$(IDF.BOOTS.CODE$) + "          ",10)     :\
         BATCH.SCREEN.FLAG$ = "B"
      RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,                \DSJW
                              VAR.STRING.2$,EVENT.NO%)                  !DSJW
      BATCH.SCREEN.FLAG$ = "S"

      GOSUB RESTORE.MESSAGE                                            ! BMW

      RETURN

\*******************************************************************************
\***
\***   DO.UPDATE:
\***
\***   set deal flag off
\***   set local price flag off
\***
\***   Check whether the IRF is converted or not, depending on the result
\***   set local flag according to the correct field,
\***                     ie: Converted   -> HOLDING PRICE,
\***                         Unconverted -> IRF INDICAT3% (bit-6)
\***
\***
\***   IF (irf indicat2 unpacks to give a non-zero second digit
\***   OR irf salequan unpacked is not "00" or "01")
\***   AND local price flag not set) THEN
\***      set deal flag on
\***      IF first ean flag is on THEN
\***         set external message string data to boots code
\***         set disp. mess. to 261
\***         GOSUB SHOW.MESSAGE
\***         set update failed flag on
\***         RETURN
\***      ELSE
\***         set unique to bar code last 12 digits packed + "D" + ppfi status
\***         CALL ADXERROR function to log event number 25
\***      endif
\***   endif
\***
\...............................................................................

   DO.UPDATE:

      DEAL.PRICE$ = "N"
      LOCAL.PRICE.FLAG$ = "N"                                           !DSJW

! Code removed refering to UNCONVERTED RECORDS                              GSB

      IF (IRF.INDICAT3% AND 00100000B) <> 0 THEN \                      !DSJW
      BEGIN                                                             !DSJW
         LOCAL.PRICE.FLAG$ = "Y"                                        !DSJW
      ENDIF                                                             !DSJW

!! All records are written back to the IRF converted                    !DSJW
      IRF.RECORD$ = "CONVERTED RECORD"                                  !DSJW

!     INDICAT2$ = RIGHT$ (UNPACK$(IRF.INDICAT2$),1)                    ! HSWM
!     SALEQUAN$ = UNPACK$ (IRF.SALEQUAN$)                              ! HSWM
!     IF (INDICAT2$ <> "0"                                             \ HSWM
!     OR (SALEQUAN$ <> "00" AND SALEQUAN$ <> "01"))                    \ HSWM
!     AND LOCAL.PRICE.FLAG$ = "N" THEN                                 \!HSWM
!        DEAL.PRICE$ = "Y"                                            :\ HSWM
!        IF FIRST.EAN$ = "Y" THEN                                      \ HSWM
!           VARIABLE.STRING$  = PPF.BOOTS.CODE$                       :\ HSWM
!           MESSAGE.NO% = 261                                         :\ HSWM
!           GOSUB SHOW.MESSAGE                                        :\ HSWM
!           GOSUB RESTORE.MESSAGE                                     :\ HSWM
!           UPDATE.FAILED$ = "Y"                                      :\ HSWM
!           RETURN                                                    :\ HSWM
!        ELSE                                                          \ HSWM
!           UNIQUE$ = RIGHT$(IRF.BAR.CODE$,6) + "D" + PPF.STATUS.FLAG$ :\HSWM
!           TERM% = 0                                                 :\ HSWM
!           MSGGRP% = ASC("J")                                        :\ HSWM
!           MSGNUM% = 0                                               :\ HSWM
!           SEVERITY% = 3                                             :\ HSWM
!           EVENT.NO% = 25                                            :\ HSWM
!           CALL ADXERROR (TERM%,                                      \ HSWM
!                          MSGGRP%,                                    \ HSWM
!                          MSGNUM%,                                    \ HSWM
!                          SEVERITY%,                                  \ HSWM
!                          EVENT.NO%,                                  \ HSWM
!                          UNIQUE$)                                      HSWM

\...............................................................................
\***
\***   IF local price flag is on THEN
\***      IF ppfi status flag is not "L" THEN
\***         IF first ean flag is on THEN
\***            set external message string data to boots code
\***            set disp. mess. to 262
\***            GOSUB SHOW.MESSAGE
\***            set update failed flag on
\***            RETURN
\***         ELSE
\***            set unique to bar code last 12 digits packed + "L" + ppfi status
\***            CALL ADXERROR function to log event number 25
\***         endif
\***      ELSE
\***      endif
\***   ELSE
\***      IF ppfi status flag is "L" THEN
\***         IF first ean flag is on THEN
\***            set external message string data to boots code
\***            set disp. mess. to 263
\***            GOSUB SHOW.MESSAGE
\***            set update failed flag on
\***            RETURN
\***         ELSE
\***            set unique to bar code last 12 digits packed + " " + ppfi status
\***            CALL ADXERROR function to log event number 25
\***         endif
\***      endif
\***   endif
\***
\...............................................................................

      IF LOCAL.PRICE.FLAG$ = "Y" THEN                                  \!DSJW
         IF PPF.STATUS.FLAG$ <> "L" THEN                               \
            IF FIRST.EAN$ = "Y" THEN                                   \
               VARIABLE.STRING$ = PPF.BOOTS.CODE$                     :\CSJW
               MESSAGE.NO% = 262                                      :\
               GOSUB SHOW.MESSAGE                                     :\
               GOSUB RESTORE.MESSAGE                                  :\ BMW
               UPDATE.FAILED$ = "Y"                                   :\
               RETURN                                                 :\
            ELSE                                                       \
               UNIQUE$ = RIGHT$(IRF.BAR.CODE$,6) + "L" +               \
                                                    PPF.STATUS.FLAG$  :\
               TERM% = 0                                              :\
               MSGGRP% = ASC("J")                                     :\
               MSGNUM% = 0                                            :\
               SEVERITY% = 3                                          :\
               EVENT.NO% = 25                                         :\
               CALL ADXERROR (TERM%,                                   \
                              MSGGRP%,                                 \
                              MSGNUM%,                                 \
                              SEVERITY%,                               \
                              EVENT.NO%,                               \
                              UNIQUE$)                                :\
         ELSE                                                          \
      ELSE                                                             \
         IF PPF.STATUS.FLAG$ = "L" THEN                                \
            IF FIRST.EAN$ = "Y" THEN                                   \
               VARIABLE.STRING$ = PPF.BOOTS.CODE$                     :\CSJW
               MESSAGE.NO% = 263                                      :\
               GOSUB SHOW.MESSAGE                                     :\
               GOSUB RESTORE.MESSAGE                                  :\ BMW
               UPDATE.FAILED$ = "Y"                                   :\
               RETURN                                                 :\
            ELSE                                                       \
               UNIQUE$ = RIGHT$(IRF.BAR.CODE$,6) + " " +               \
                                                   PPF.STATUS.FLAG$   :\
               TERM% = 0                                              :\
               MSGGRP% = ASC("J")                                     :\
               MSGNUM% = 0                                            :\
               SEVERITY% = 3                                          :\
               EVENT.NO% = 25                                         :\
               CALL ADXERROR (TERM%,                                   \
                              MSGGRP%,                                 \
                              MSGNUM%,                                 \
                              SEVERITY%,                               \
                              EVENT.NO%,                               \
                              UNIQUE$)

\...............................................................................
\***
\***   IF first ean flag is on THEN
\***      set first ean flag off
\***      set saved price to irf salepric
\***      set saved mpgroup to irf mpgroup
\***      set saved idicat2 to irf indicat2
\***      set saved.deal.num$ = irf.deal.num$
\***      set saved.indicat4% = irf.indicat4%
\***      set saved.indicat3% = irf.indicat3%
\***      set saved.deal.saving$ = irf.deal.saving$
\***      set saved salequan to irf salequan
\***      IF local price flag is ON
\***      AND local price file is OPEN THEN
\***         update the item on the local file
\***      ELSE
\***         set irf salepric to ppfi new price
\***         set saved price to irf salepric
\***      endif
\***   ELSE
\***      set irf salepric to saved price
\***      set irf indicat2 to saved indicat2
\***      set irf.deal.num$ = saved.deal.num$
\***      set irf.indicat4% = saved.indicat4%
\***      set irf.indicat3% = saved.indicat3%
\***      set irf.deal.saving$ = saved.deal.saving$
\***      set irf salequan to saved salequan
\***      set irf mpgroup to saved mpgroup
\***   endif
\***
\***
\***   RETURN
\***
\............................................................................

      IF FIRST.EAN$ = "Y" THEN BEGIN                                    !DSJW
         FIRST.EAN$ = "N"                                               !DSJW
!        SAVED.DEAL.NUM$ = IRF.DEAL.NUM$                                !DSJW     !IBG
!        SAVED.INDICAT4% = IRF.INDICAT4%                                !DSJW     !IBG
!        SAVED.DEAL.SAVING$ = IRF.DEAL.SAVING$                          !DSJW     !IBG
         SAVED.INDICAT5% = IRF.INDICAT5%                                !DSJW GSB
!        SAVED.INDICAT2% = IRF.INDICAT2%                                !HSWM     !IBG
!        SAVED.SALEQUAN$ = IRF.SALEQUAN$                                !DSJW     !IBG
         SAVED.DEAL.NUM$(0) = IRF.DEAL.NUM$(0)                                    !IBG
         SAVED.DEAL.NUM$(1) = IRF.DEAL.NUM$(1)                                    !IBG
         SAVED.DEAL.NUM$(2) = IRF.DEAL.NUM$(2)                                    !IBG
         SAVED.LIST.ID%(0) = IRF.LIST.ID%(0)                                      !IBG
         SAVED.LIST.ID%(1) = IRF.LIST.ID%(1)                                      !IBG
         SAVED.LIST.ID%(2) = IRF.LIST.ID%(2)                                      !IBG
         IF LOCAL.PRICE.FLAG$ = "Y" THEN \                              !DSJW
         BEGIN                                                          !DSJW
            GOSUB UPDATE.LOCAL.FILE                                     !DSJW
         ENDIF \                                                        !DSJW
         ELSE \                                                         !DSJW
         BEGIN                                                          !DSJW
            IRF.SALEPRIC$ = PACK$("00" + PPF.PRICE$)                    !DSJW
            SAVED.PRICE$ = IRF.SALEPRIC$                                !DSJW
            SAVED.INDICAT3% = IRF.INDICAT3%                             !DSJW
         ENDIF                                                          !DSJW
       ENDIF ELSE BEGIN                                                 !DSJW
         IRF.SALEPRIC$ = SAVED.PRICE$                                   !DSJW
!        IRF.DEAL.NUM$ = SAVED.DEAL.NUM$                                !DSJW     !IBG
!        IRF.INDICAT4% = SAVED.INDICAT4%                                !DSJW     !IBG
         IRF.INDICAT3% = SAVED.INDICAT3%                                !DSJW
!        IRF.DEAL.SAVING$ = SAVED.DEAL.SAVING$                          !DSJW     !IBG
!        IRF.INDICAT2% = SAVED.INDICAT2%                                !HSWM     !IBG
!        IRF.SALEQUAN$ = SAVED.SALEQUAN$                                !DSJW     !IBG
         IRF.DEAL.NUM$(0) = SAVED.DEAL.NUM$(0)                                    !IBG
         IRF.DEAL.NUM$(1) = SAVED.DEAL.NUM$(1)                                    !IBG
         IRF.DEAL.NUM$(2) = SAVED.DEAL.NUM$(2)                                    !IBG
         IRF.LIST.ID%(0) = SAVED.LIST.ID%(0)                                      !IBG
         IRF.LIST.ID%(1) = SAVED.LIST.ID%(1)                                      !IBG
         IRF.LIST.ID%(2) = SAVED.LIST.ID%(2)                                      !IBG
         IRF.INDICAT5% = SAVED.INDICAT5%                                !DSJW GSB
       ENDIF                                                            !DSJW

   RETURN

\*****************************************************************************
\***   UPDATE.LOCAL.FILE
\***   Key for the local file is boots code with check digit packed.
\***   Create key and read the file
\***   If not found, add a record with default settings
\***   Update HO price and HO price date changed fields
\***   Write the rec back to the file
\*****************************************************************************

   UPDATE.LOCAL.FILE:                                                   !DSJW

      IF NOT LOCAL.FILE.OPEN THEN \                                     !DSJW
         GOTO INVALID.BOOTS.CODE                                        !DSJW
                                                           
      BOOTS.CODE.SIX.DIGIT$ EQ UNPACK$(IRF.BOOTS.CODE$)                 !DSJW
      CALL CALC.BOOTS.CODE.CHECK.DIGIT(BOOTS.CODE.SIX.DIGIT$)           !DSJW
      IF F18.CHECK.DIGIT$ EQ "A" THEN BEGIN                             !DSJW
         GOTO INVALID.BOOTS.CODE                                        !DSJW
      ENDIF ELSE BEGIN                                                  !DSJW
         BOOTS.CODE.SEVEN.DIGIT$ EQ BOOTS.CODE.SIX.DIGIT$ \             !DSJW
                                         + F18.CHECK.DIGIT$             !DSJW
      ENDIF                                                             !DSJW

      FAIL.TYPE$ EQ "R"                                                 !DSJW
      CURRENT.SESS.NUM% EQ LOCAL.SESS.NUM%                              !DSJW
      LOCAL.ITEM.CODE$ EQ PACK$("0" + BOOTS.CODE.SEVEN.DIGIT$)          !DSJW
      RC% EQ READ.LOCAL                                                 !DSJW
      IF RC% NE 0 THEN \                                                !DSJW
         GOSUB SET.LOCAL.RECORD.FIELDS                                  !DSJW

     !If the item is an emergency price change, remove the LOCAL price  !JJT
      IF LEFT$(LOCAL.REASON$,3) = "EPC" THEN BEGIN                      !JJT
         DELREC LOCAL.SESS.NUM%; LOCAL.ITEM.CODE$                       !JJT
     !And set the IRF local price flag off                              !JJT
         IRF.INDICAT3% = (IRF.INDICAT3% AND 11011111B)                  !JJT
         IRF.SALEPRIC$ = PACK$("00" + PPF.PRICE$)                       !JJT
      ENDIF ELSE BEGIN                                                  !JJT
         CURRENT.DATE$ EQ DATE$                                         !DSJW
         LOCAL.H.O.PRICE$ EQ PACK$("00" + PPF.PRICE$)                   !DSJW
         LOCAL.HO.CHANGE$ EQ PACK$(CURRENT.DATE$)                       !DSJW

         FAIL.TYPE$ EQ "W"                                              !DSJW
         RC% EQ WRITE.LOCAL                                             !DSJW
         IF RC% NE 0 THEN GOTO FILE.ERROR                               !DSJW
         IRF.INDICAT3% = (IRF.INDICAT3% OR 00100000b)                   !DSJW

      ENDIF                                                             !JJT
      
      SAVED.INDICAT3% = IRF.INDICAT3%                             !DSJW
      SAVED.PRICE$ = IRF.SALEPRIC$                                !DSJW
      
      INVALID.BOOTS.CODE:                                               !DSJW

   RETURN                                                               !DSJW

\*****************************************************************************
\***   SET.LOCAL.RECORD.FIELDS
\***   Set fields to default values
\*****************************************************************************

   SET.LOCAL.RECORD.FIELDS:                                             !DSJW

           FORTNIGHT% EQ 14                                             !DSJW
           F02.DATE$ EQ DATE$                                           !DSJW
           RC% EQ UPDATE.DATE(FORTNIGHT%)                               !DSJW
           IF RC% EQ 0 THEN BEGIN                                       !DSJW
              DATE.NEXT.WEEK$ EQ F02.DATE$                              !DSJW
           ENDIF ELSE BEGIN                                             !DSJW
              DATE.NEXT.WEEK$ EQ "000000"                               !DSJW
           ENDIF                                                        !DSJW

           LOCAL.PRICE$ EQ IRF.SALEPRIC$                                !DSJW
           LOCAL.START.DATE$ EQ PACK$(DATE$)                            !DSJW
           LOCAL.START.TIME$ EQ PACK$(MID$(TIME$,1,4))                  !DSJW
           LOCAL.END.DATE$ EQ PACK$(DATE.NEXT.WEEK$)                    !DSJW
           LOCAL.OPERATOR$ EQ PACK$(RIGHT$("00000000" + \               !DSJW
                                           OPERATOR.NUMBER$,8))         !DSJW
           LOCAL.REASON$ EQ "    "                                      !DSJW
           LOCAL.SPACE$ EQ PACK$("00000000000000")                      !DSJW

        RETURN                                                          !DSJW


\*******************************************************************************
\***
\***   SHOW.MESSAGE:
\***
\***   IF  called from store opening  THEN
\***      Log the message in the user parameter for return
\***      RETURN
\***   ENDIF
\***
\***   IF increase/decrease flag <> "D" THEN
\***      set string data to spaces
\***      set integer data to -3 (previous output field)
\***      CALL DM.NEXTF function to position the cursor in the prev. output field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 0
\***   CALL DM.POSF function to obtain the number of the current field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set string data to spaces
\***   set integer data to 20 (last input field)
\***   CALL DM.NEXTF function to position the cursor in the last input field
\***   IF F03.RETURN.CODE% <> 0 THEN
\***      GOTO CHAIN.OUT
\***   endif
\***
\***   set key pressed to 999
\***
\***   WHILE key pressed is not ESC
\***
\***      CALL EXTERNAL.MESSAGE function to display message number disp.mess.
\***                                         return to current field
\***      IF F04.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 20 (last input field)
\***      CALL DM.NEXTF function to position the cursor in the last input field
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***      set string data to spaces
\***      set integer data to 0
\***      CALL DM.UPDF function to get user input
\***      IF F03.RETURN.CODE% <> 0 THEN
\***         GOTO CHAIN.OUT
\***      endif
\***
\***   WEND
\***
\***   RETURN
\***
\...............................................................................

   SHOW.MESSAGE:

      IF PSBCHN.APP = "PSB51"  THEN BEGIN                               !DSJW
         PSBCHN.U2 = PSBCHN.U2 + RIGHT$(STR$(1000+MESSAGE.NO%),3) + \   !DSJW
                        VARIABLE.STRING$ + "   "                        !DSJW
         KEY.PRESSED% = 999                                             !DSJW
         RETURN
      ENDIF                                                             !DSJW

      IF INC.DEC$ <> "D" THEN BEGIN                                     !DSJW
         STRING.DATA$ = ""                                              !DSJW
         INTEGER.DATA% = -3                                             !DSJW
         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DSJW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW
      ENDIF                                                             !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN BEGIN                                           !DSJW
         GOTO CHAIN.OUT                                                !DSJW
      ENDIF ELSE BEGIN                                                 !DSJW
         CURRENT.FIELD% = F03.RETURNED.INTEGER%
      ENDIF                                                            !DSJW

      STRING.DATA$ = ""
      INTEGER.DATA% = 20
      RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                       !DJSW
      IF RC% NE 0 THEN GOTO CHAIN.OUT                                   !DSJW

      KEY.PRESSED% = 999

      WHILE KEY.PRESSED% <> ESCAPE%

         RETURN.FIELD% = CURRENT.FIELD%
         RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,VARIABLE.STRING$,         \DSJW
                                  RETURN.FIELD%)                        !DSJW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = ""
         INTEGER.DATA% = 20
         RC% EQ DM.NEXTF(STRING.DATA$,INTEGER.DATA%)                    !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW

         STRING.DATA$ = ""
         INTEGER.DATA% = 0
         RC% EQ DM.UPDF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW
         KEY.PRESSED% = F03.RETURNED.INTEGER%

      WEND

      KEY.PRESSED% = 999

   RETURN

\******************************************************************************
\***
\***    RESTORE.MESSAGE:
\***
\***    IF NOT called from store opening THEN
\***    BEGIN
\***            Display message 273 ! RPD price changes are being effected
\***    ENDIF
\***
\***    RETURN
\***
\..............................................................................

      RESTORE.MESSAGE:                                                  ! BMW

      IF PSBCHN.APP <> "PSB51" THEN                                     \ BMW
      BEGIN                                                             ! BMW
         STRING.DATA$ = ""                                              ! BMW
         INTEGER.DATA% = 107                                            ! BMW
         RC% EQ DM.POSF(STRING.DATA$,INTEGER.DATA%)                     !DJSW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW
         MESSAGE.NO% = 273                                              ! BMW
         VARIABLE.STRING$ = ""                                          ! BMW
         RETURN.FIELD% = F03.RETURNED.INTEGER%                          ! BMW
         RC% EQ EXTERNAL.MESSAGE (MESSAGE.NO%,                          \DSJW
                                  VARIABLE.STRING$,                     \DSJW
                                  RETURN.FIELD%)                        !DSJW
         IF RC% NE 0 THEN GOTO CHAIN.OUT                                !DSJW
      ENDIF                                                             ! BMW

      RETURN                                                            ! BMW

\*******************************************************************************
\***
\***   FILE.ERROR:
\***
\***   set string file to STR$ of current session number, length 2 bytes
\***
\***   IF fail type is "O" THEN
\***      set message number to 501
\***   ELSE
\***      IF fail type is "R" THEN
\***         set message number to 503
\***      ELSE
\***         IF fail type is "C" THEN
\***            set message number to 505
\***         ELSE
\***            set message number to 504
\***
\***   set string session number to CHR$ of current session number
\***   set variable string 1 to fail type and string session number and key
\***   set variable string 2 to string file
\***
\***   IF  called from store opening
\***     AND not an IDF or IRF read error THEN
\***      Save error in user parameter 2
\***      Set batch/screen flag to batch and to suppress screen message
\***   endif
\***
\***   CALL APPLICATION.LOG function to log event 106, message number
\***   as indicated
\***
\***   Reset batch/screen flag to screen
\***
\***   RETURN
\***
\...............................................................................

   FILE.ERROR:

      SB.ACTION$ = "R"                                                 ! BMW
      SB.INTEGER% = CURRENT.SESS.NUM%                                  ! BMW
      GOSUB SB.FILE.UTILS                                              ! BMW

      VAR.STRING.2$ = RIGHT$ ("000" + F20.STRING.FILE.NO$,3) +         \
                      UNPACK$(CURRENT.KEY$)

      IF MESSAGE.NO% = 513 THEN VAR.STRING.2$ = "" ! B513 has no data  ! 1.19 RC

      IF FAIL.TYPE$ = "O" THEN \
          BEGIN                                                        ! 1.19 RC
          IF MESSAGE.NO% <> 513 THEN \ ! Access conflict opening PPFI  ! 1.19 RC
              BEGIN                                                    ! 1.19 RC
              MESSAGE.NO% = 501
              ENDIF                                                    ! 1.19 RC
          ENDIF \                                                      ! 1.19 RC
      ELSE \
          IF FAIL.TYPE$ = "R" THEN \
              BEGIN                                                    ! 1.19 RC
              MESSAGE.NO% = 503
              ENDIF \                                                  ! 1.19 RC
          ELSE \
              IF FAIL.TYPE$ = "C" THEN \
                  BEGIN                                                ! 1.19 RC
                  MESSAGE.NO% = 505
                  ENDIF \                                              ! 1.19 RC
              ELSE \
                  BEGIN                                                ! 1.19 RC
                  MESSAGE.NO% = 504
                  ENDIF                                                ! 1.19 RC

      VAR.STRING.1$ = FAIL.TYPE$ + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) \    !DSJW
                                 + CHR$(SHIFT(SB.FILE.REP.NUM%,0)) \    !DSJW
                                 + RIGHT$(PACK$("00000000000000") \     !DSJW
                                 + CURRENT.KEY$,7)                      !DSJW
      EVENT.NO% = 106                                                   !DSJW

      IF PSBCHN.APP = "PSB51" THEN                                      \
         PSBCHN.U2 = PSBCHN.U2 + RIGHT$(STR$(1000+MESSAGE.NO%),3)       \
                               + VAR.STRING.1$                         :\
         BATCH.SCREEN.FLAG$ = "B"

      RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,                \DSJW
                              VAR.STRING.2$,EVENT.NO%)                  !DSJW

      BATCH.SCREEN.FLAG$ = "S"

      IF SELECTION$ = "" THEN \ ! File error occured on B7201 menu     ! 1.19 RC
          BEGIN                 ! screen prior to selecting any option ! 1.19 RC
          RETURN                ! so bypass RESTORE.MESSAGE (used to   ! 1.19 RC
          ENDIF                 ! re-displays B273 for other screens)  ! 1.19 RC
      
      GOSUB RESTORE.MESSAGE                                            ! BMW
   
   RETURN

\******************************************************************************
\***
\***   ALLOCATE.SESS.NUMS:
\***
\***      Allocate all file session numbers
\***
\******************************************************************************

       ALLOCATE.SESS.NUMS:                                             !  BMW

       SB.ACTION$ = "O"                                                !  BMW

       SB.INTEGER% = IDF.REPORT.NUM%                                   !  BMW
       SB.STRING$ = IDF.FILE.NAME$                                     !  BMW
       GOSUB SB.FILE.UTILS                                             !  BMW
       IDF.SESS.NUM% = SB.FILE.SESS.NUM%                               !  BMW

       SB.INTEGER% = IEF.REPORT.NUM%                                   !  BMW
       SB.STRING$ = IEF.FILE.NAME$                                     !  BMW
       GOSUB SB.FILE.UTILS                                             !  BMW
       IEF.SESS.NUM% = SB.FILE.SESS.NUM%                               !  BMW

       SB.INTEGER% = IRF.REPORT.NUM%                                   !  BMW
       SB.STRING$ = IRF.FILE.NAME$                                     !  BMW
       GOSUB SB.FILE.UTILS                                             !  BMW
       IRF.SESS.NUM% = SB.FILE.SESS.NUM%                               !  BMW

       SB.INTEGER% = PPFI.REPORT.NUM%                                  !  BMW
       SB.STRING$ = PPFI.FILE.NAME$                                    !  BMW
       GOSUB SB.FILE.UTILS                                             !  BMW
       PPFI.SESS.NUM% = SB.FILE.SESS.NUM%                              !  BMW

       SB.INTEGER% = PPFO.REPORT.NUM%                                  !  BMW
       SB.STRING$ = PPFO.FILE.NAME$                                    !  BMW
       GOSUB SB.FILE.UTILS                                             !  BMW
       PPFO.SESS.NUM% = SB.FILE.SESS.NUM%                              !  BMW

       SB.INTEGER% = LOCAL.REPORT.NUM%                                  !DSJW
       SB.STRING$ = LOCAL.FILE.NAME$                                    !DSJW
       GOSUB SB.FILE.UTILS                                              !DSJW
       LOCAL.SESS.NUM% = SB.FILE.SESS.NUM%                              !DSJW

       RETURN                                                          !  BMW

\******************************************************************************
\***
\***    DEALLOCATE.SESS.NUMS:
\***
\***    Deallocate all file session numbers
\***
\******************************************************************************

         DEALLOCATE.SESS.NUMS:                                         ! BMW

         SB.ACTION$ = "C"                                              ! BMW
         SB.STRING$ = ""                                               ! BMW

         SB.INTEGER% = PPFI.SESS.NUM%                                  ! BMW
         GOSUB SB.FILE.UTILS                                           ! BMW

         SB.INTEGER% = IRF.SESS.NUM%                                   ! BMW
         GOSUB SB.FILE.UTILS                                           ! BMW

         SB.INTEGER% = IDF.SESS.NUM%                                   ! BMW
         GOSUB SB.FILE.UTILS                                           ! BMW

         SB.INTEGER% = IEF.SESS.NUM%                                   ! BMW
         GOSUB SB.FILE.UTILS                                           ! BMW

         SB.INTEGER% = PPFO.SESS.NUM%                                  ! BMW
         GOSUB SB.FILE.UTILS                                           ! BMW

         SB.INTEGER% = LOCAL.SESS.NUM%                                  !DSJW
         GOSUB SB.FILE.UTILS                                            !DSJW

         RETURN                                                        ! BMW

\******************************************************************************
\***
\***   SB.FILE.UTILS:
\***
\***      Allocate/report/de-allocate a file session number
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
\***         SB.FILE.SESS.NUM% = file session number for action "O" or
\***                             undefined for action "C"
\***         OR
\***         SB.FILE.REP.NUM%  = file reporting number for action "R" or
\***                             undefined for action "C"
\***
\******************************************************************************

       SB.FILE.UTILS:                                                   ! BMW

       RC% EQ SESS.NUM.UTILITY(SB.ACTION$,SB.INTEGER%,SB.STRING$)       !DSJW
       IF RC% NE 0 THEN GOTO CHAIN.OUT                                  !DSJW

       IF SB.ACTION$ = "O" THEN                                         \ BMW
          SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%                      ! BMW

       IF SB.ACTION$ = "R" THEN                                         \ BMW
          SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%                      :\ BMW
          SB.FILE.NAME$ = F20.FILE.NAME$                                ! BMW

       RETURN                                                           ! BMW

\*******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***      IF ERR is "*I" and ERRN is 80F3400C THEN
\***         GOTO CREATE.ERROR
\***      endif
\***
\***      IF ERR is "CM" or "CT"                    \REM chain failure
\***         CALL APPLICATION.LOG function to log event 18, message number 553
\***      endif
\***
\***      Call STANDARD ERROR DETECTED
\***
\***      resume
\***
\***   END
\***
\..............................................................................

   ERROR.DETECTED:

    IF ERR = "NP" THEN BEGIN                                           ! 1.20 RC
        PSBCHN.OP     = "99999999"                                     ! 1.20 RC
        PSBCHN.APP    = "PSB50"                                        ! 1.20 RC
        PSBCHN.MENCON = "421100"                                       ! 1.20 RC
        PSBCHN.U1     = ""                                             ! 1.20 RC
        PSBCHN.U2     = ""                                             ! 1.20 RC
        PSBCHN.U3     = ""                                             ! 1.20 RC
        RESUME RESUME.FROM.NP.ERROR                                    ! 1.20 RC
    ENDIF                                                              ! 1.20 RC
    
      IF    ERR = "*I" \                                               ! 1.19 RC
        AND ERRN = 80F3400CH \                ! Access conflict        ! 1.19 RC
        AND ERRF% = PPFI.SESS.NUM% THEN BEGIN ! opening PPFI           ! 1.19 RC
          MESSAGE.NO% = 513     ! Message number used in FILE.ERROR    ! 1.19 RC
          GOSUB OPEN.PPFI.ERROR ! to indicate PPFI access conflict     ! 1.19 RC
      ENDIF                                                            ! 1.19 RC
      
      IF ERR = "*I" AND ERRN = 80F3400CH THEN BEGIN                     !DSJW
         GOTO CREATE.ERROR
      ENDIF                                                             !DSJW

      IF ERR = "CM" OR ERR = "CT" THEN BEGIN                            !DSJW
         VAR.STRING.1$ = "B72  B50  "                                   !DSJW
         VAR.STRING.2$ = "PSB50" + "     "                              !DSJW
         MESSAGE.NO% = 553                                              !DSJW
         EVENT.NO% = 18                                                 !DSJW
         IF PSBCHN.APP = "PSB51" THEN                                   \DSJW
         BEGIN                                                          !DSJW
            PSBCHN.U2 = PSBCHN.U2 + "553" + VAR.STRING.2$               !DSJW
            BATCH.SCREEN.FLAG$ = "B"                                    !DSJW
         ENDIF                                                          !DSJW
         RC% EQ APPLICATION.LOG (MESSAGE.NO%,VAR.STRING.1$,             \DSJW
                                 VAR.STRING.2$,EVENT.NO%)               !DSJW
         BATCH.SCREEN.FLAG$ = "S"                                       !DSJW
      ENDIF                                                             !DSJW

      RC% EQ STANDARD.ERROR.DETECTED(ERRN,ERRF%,ERRL,ERR)               !DSJW

      RESUME                                                            ! BMW

   END

