\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSS3702.bas  $
\***
\***   $Revision:   1.8  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PSS3702.bav  $
\***   
\***      Rev 1.8   16 Mar 2007 13:29:30   steve.perkins
\***   Defect 75 - Changes to type 17 STKMQ record
\***
\***      Rev 1.6   01 Mar 2005 11:33:52   dev07ps
\***   Upwards TSF Fix.
\***   Fix to Report & Session numbers.
\***   Relinked with latest PLLOL object.
\***   Removal of LSS specific code.
\***   Fix to Stock Counting - moved to new program PDTSC.286 to solve PDT timeout when > 40 counts.
\***
\***
\***      Rev 1.5   30 Jul 2003 14:28:08   dev88ps
\***   Changes made to accomodate new Stock
\***   Count facility on the PDT. New module
\***   PSS3705 created.
\***
\***      Rev 1.4   17 Jul 2003 10:04:12   dev38ps
\***   Commented out new reference to function 18 as this is already in external includer code
\***
\***      Rev 1.3   29 Oct 2002 09:09:38   dev88ps
\***   Changes to accomodate live stock counting
\***   in the LSS only (store 2999.) Changes are
\***   not visible in any other store.
\***
\***      Rev 1.2   17 Oct 1997 15:11:14   DEV20PS
\***   GRBI changes.
\***
\***      Rev 1.1   07 Oct 1994 15:51:30   DEVSPPS
\***   Fixed PDT Support hang
\***
\******************************************************************************
\******************************************************************************

\REM
\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3702                                          ***
\***                                                                        ***
\***           AUTHOR   :  David Smallwood (Pseudocode / BASIC code)        ***
\***                                                                        ***
\***           DATE     :  16th April 1992                                  ***
\***                                                                        ***
\***                                                                        ***
\***           Current version letter : G                                   ***
\***                                                                        ***
\***           Date of last update:  17th May 1994.  Michael J. Kelsall     ***
\***                                                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   O V E R V I E W                                                      ***
\***                                                                        ***
\***                                                                        ***
\***   PSS37 - P.D.T Support Program.                                       ***
\***                                                                        ***
\***                                                                        ***
\***   PSS37 is designed to run concurrently with PSS38. PSS38 handles      ***
\***   all asyncronous communications with a connected PDT. All data        ***
\***   sent by the PDT is passed to PSS37 via PSS38 by means of a 'pipe'.   ***
\***   PSS37 validates the data sent to ensure the data has been sent in    ***
\***   the correct sequence, has a valid format and is meaningful.          ***
\***   the correct sequence, has a valid format and is meaningful.          ***
\***   There are basically two processes PSS37 performs ;                   ***
\***   i)  takes counts from a PDT and puts them in the stock movement,     ***
\***   ii) creates a file of lists requested by a PDT.                      ***
\***                                                                        ***
\***   This module was created to contain the bulk of the processing for    ***
\***   a new PDT application for Directs Receiving.  This application will  ***
\***   use a new piece of hardware, the Laser Data Terminal (LDT).  When    ***
\***   orders are requested from the LDT PSS37 will process new data files  ***
\***   to format the order information for transmitting back to the LDT.    ***
\***   The new data files are the Direct Supplier File (DIRSUP) which holds ***
\***   information on direct suppliers which is required in the LDT and the ***
\***   Direct Orders File (DIRORD) which holds information on each direct   ***
\***   order on the system.  When receipts are passed back to the controller***
\***   PSS37 will write receipts information to the Stock Movement Queue    ***
\***   for processing by Stock Support.                                     ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   A M E N D M E N T S                                                  ***
\***                                                                        ***
\***   Version B.  Les Cook                               October 1992      ***
\***   Program streamlined according to new Programming Guidelines.         ***
\***                                                                        ***
\***   Version C.  Les Cook                            5th November 1992    ***
\***   Program now creates LDTBF which is used by PSS78 to provide a report ***
\***   showing damaged and stolen goods booked in by the LDT.  PSS37 kicks  ***
\***   off PSS78 (using ADXSTART) on the background screen if there is any  ***
\***   data to process.                                                     ***
\***                                                                        ***
\***   Version D.  Les Cook                            13th January 1993    ***
\***   Interim version to fix problem whereby STKMQ is being corrupted.     ***
\***   STKMQ is now opened READONLY and with BUFFSIZE of 512.               ***
\***                                                                        ***
\***   Version E.  Les Cook                                  February 1993  ***
\***   To handle processing for UOD Receiving                               ***
\***   Note that there are new data states associated with UOD which use    ***
\***   LOWER CASE LETTERS a - f.  !!!!!!!!!!!                               ***
\***                                                                        ***
\***   Version F.  Michael J. Kelsall                     12th October 1993 ***
\***   Fix to clear problem where UOD's with a status of 'I' appear as      ***
\***   the first record has 'I' and any others are 'O'.                     ***
\***   Addition of LDTAF processing in all applications.                    ***
\***                                                                        ***
\***   Version F. (Supplemental)  David Smallwood         7th January 1994  ***
\***   Hardcode LDTPF file name for logging file size to LDTAF.             ***
\***                                                                        ***
\***   Version G. Michael J. Kelsall                       23rd March 1994  ***
\***   Change LDTAF duration time from minutes to seconds.                  ***
\***   Alteration to code to suppress STKMQ (UODTF) type 21 txn production  ***
\***   for null UODs other than Emergency booked in UODs.                   ***
\***   Transfer of LDTAF link type initialisation to clear problem where    ***
\***   LDTAF SIZE error handling does not ignore DRSMQ not being present.   ***
\***                                                                        ***
\***   Version G (Supplemental)  Michael J. Kelsall         20th July 1994  ***
\***   Change to remove holding and subsequent release of LDT during        ***
\***   transfer of data from LDT to STKMQ. Change made in an attempt to     ***
\***   prevent corruption of subsequent transmission if store enter UOD     ***
\***   application before the CR+LR are stopped.                            ***
\***                                                                        ***
\***   Version G (Supplemental)  Michael J. Kelsall         1st August 1994 ***
\***   Change to allow training UOD's to be entered into system and         ***
\***   ignored by PSS37 (not transfered to STKMQ). These will be identified ***
\***   by the dist centre / whse number both being set to all nines.        ***
\***                                                                        ***
\***   REVISION 1.1     ROBERT COWEY / STEVE WRIGHT       6TH OCTOBER 1994  ***
\***   Removed version letters from included code (not commented).          ***
\***                                                                        ***
\***   Version 1.2          Stuart Highley           12th September 1997    ***
\***   Changes for GRBI.  Added new fields to LDTBF and start PSS79         ***
\***   instead of PSS78.                                                    ***
\***                                                                        ***
\***   Version 1.3          Brian Greenfield         17th October 2002      ***
\***   Changes to allow the LSS (store 2999) to perform live stock counts   ***
\***   that change the stock figure to a counted figure.                    ***
\***   LSS stock counts hijack the Directs No Order Number structure.       ***
\***   This code will only ever be executed in store 2999.                  ***
\***                                                                        ***
\***   Version 1.4          Brian Greenfield         18th July 2003         ***
\***                                                                        ***
\***   REVISION 1.5    Charles Skadorwa             4th February 2005       ***
\***   Removed LSS 2999 processing as redundant now.                        ***
\***                                                                        ***
\***   REVISION 1.6     Neil Bennett                 2nd January 2007       ***
\***   Add ASN processing support.                                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   I N C L U D E S   A N D   V A R I A B L E S                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************


!  %INCLUDE PSBF18G.J86                                                 !1.3BG
   %INCLUDE PSS37G.J86                                                  ! GMJK

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL                      ! CLC
      INTEGER*2 ADXSTART                                                ! CLC
      STRING    NAME$, PARM$, MESS$                                     ! CLC
   END FUNCTION                                                         ! CLC

!  %INCLUDE PSBF18E.J86                                                 !1.3BG
   %INCLUDE DIROREXT.J86                                                ! BLC
   %INCLUDE DIRSUEXT.J86                                                ! BLC
   %INCLUDE DIRWFEXT.J86                                                ! BLC
   %INCLUDE LDTCFEXT.J86                                                ! BLC
   %INCLUDE DRSMQEXT.J86                                                ! BLC
!  %INCLUDE STOCKEXT.J86                                                !1.3BG !1.4BG
!  %INCLUDE IMSTCEXT.J86                                                !1.3BG !1.4BG
!   %INCLUDE LSSSTEXT.J86                                                !1.3BG !1.5CS

   SUB PSS3702 PUBLIC

REAL GAP

INTEGER*1                                                               \
        CURR.SESS.NUM%,                                                 \
        END.OF.DRSMQ,                                                   \
        END.OF.UODTF,                                                   \ ELC
        PORT.EVENT%,                                                    \
        SB.EVENT.NO%,                                                   \
        SB.FILE.SESS.NUM%,                                              \
        ORDER.HEADER.WRITTEN,                                           \
        ORDER.FOUND,                                                    \
        SUPERCEDED%,                                                    \ 1.6NWB
        SUPPLIER.FOUND,                                                 \
        MORE.BAR.CODES,                                                 \
        FINISHED,                                                       \
        SWAPPED,                                                        \
        ORDERS.IN.TABLE%

INTEGER*2                                                               \
        MESSAGE.NO%,                                                    \
        NO.SECTORS%,                                                    \
        SECTOR.NUMBER%,                                                 \
        RECORD.NUMBER%,                                                 \
        RECORD.LENGTH%,                                                 \
        COUNT%,                                                         \
        INDEX%,                                                         \
        ITEM.COUNT%,                                                    \
        ITEM.TOTAL%,                                                    \
        ORDER.INDEX%,                                                   \
        BC.INDEX%,                                                      \
        BC.TOTAL%,                                                      \
        HEADER.TOTAL%,                                                  \
        HEADER.INDEX%,                                                  \
        NO.RECORDS%,                                                    \
        ITEMS.IN.ORDER%,                                                \
        SB.FILE.REP.NUM%,                                               \
        SB.INTEGER%,                                                    \
        SINGLES.IN.ORDER%,                                              \
        SINGLES.BOOKED.IN%,                                             \
        START.OF.ORDER%,                                                \
        NO.OF.BAR.CODES%,                                               \
        NO.OF.REQUESTED.ORDERS%,                                        \
        BAR.CODE.TOTAL%,                                                \
        PAGE.NO%,                                                       \
        QUANTITY.GOOD%,                                                 \
        QUANTITY.STOLEN%,                                               \
        QUANTITY.DAMAGED%                                               !


INTEGER*4                                                               \
        ADX.RET.CODE%,                                                  \
        A%,                                                             \
        B%,                                                             \
        MAX%                                                            !

STRING                                                                  \
        SB.ACTION$,                                                     \
        SB.ERRF$,                                                       \
        SB.ERRL$,                                                       \
        SB.ERRS$,                                                       \
        SB.MESSAGE$,                                                    \
        SB.UNIQUE$,                                                     \
        SB.STRING$,                                                     \
        TEMP$,                                                          \
        BAR.ITEM.CODE$,                                                 \ ELC
        BAR.ITEM.CODE.FLAG$,                                            \ ELC
        CURRENT.KEY$,                                                   \
        TEMP.STATE$,                                                    \
        UNIQUE.2$,                                                      \
        SECTOR.FORMAT$,                                                 \
        SECTOR$,                                                        \
        DRSMQ.STRING$,                                                  \
        RECORD$,                                                        \
        NULL.KEY$,                                                      \
        VERSION.DETAILS$,                                               \
        ITEM.CODE$,                                                     \
        ITEM.POS.STRING$,                                               \
        ORDER.DETAILS$,                                                 \
        PAGE.NUMBER$,                                                   \
        SUPPLIER.NO.PACKED$,                                            \
        DIRWF.DETAIL.TABLE$(1),                                         \
        ORDER.TABLE$(1),                                                \
        BAR.CODE.TABLE$(1),                                             \
        ITEM.POSITION.TABLE$(1),                                        \
        HEADER.TABLE$(1),                                               \
        BC.LETTER.TABLE$(1),                                            \
        REJECT.REASON$,                                                 \
        DIRWF.HEADER.RECORD$,                                           \
        DIRWF.NO.OF.ORDERS$,                                            \
        DIRWF.RECORD.TYPE$,                                             \
        DIRWF.SUPPLIER.NUMBER$,                                         \
        DIRWF.ORDER.NUMBER$,                                            \
        DIRWF.ORDER.SUFFIX$,                                            \
        DIRWF.ITEMS.IN.ORDER$,                                          \
        DIRWF.SINGLES.IN.ORDER$,                                        \
        DIRWF.ITEMS.BOOKED.IN$,                                         \
        DIRWF.SINGLES.BOOKED.IN$,                                       \
        DIRWF.SUPPLIER.NAME$,                                           \
        DIRWF.HIGH.QTY.LIMIT$,                                          \
        DIRWF.QTY.LIMIT$,                                               \
        DIRWF.DISCREP.QTY$,                                             \
        DIRWF.DISCREP.PERC$,                                            \
        DIRWF.REJECT.REASON$,                                           \
        DIRWF.SOURCE.FLAG$,                                             \
        DIRWF.APPLICATION.NO$,                                          \
        DIRWF.TERMINAL.NO$,                                             \
        DIRWF.BARCODE$,                                                 \
        DIRWF.NO.OF.BARCODES$,                                          \
        DIRWF.ITEM.POSITION$,                                           \
        DIRWF.ITEM.CODE$,                                               \
        DIRWF.EXPECTED.QTY$,                                            \
        DIRWF.ORDER.FLAG$,                                              \
        DIRWF.CHK.QTY.FOR.NO.ORDER$,                                    \
        DIRWF.PAGE.NO$,                                                 \
        LDTBF.WRITTEN.FLAG$,                                            \ CLC
        OPENING.UODBF$                                                  ! ELC

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   M A I N L I N E   C O D E                                            ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   for any errors, pass control to ERROR.DETECTED
\***
\***   pass control to the appropriate section of code, depending upon
\***   the value of RECEIVE.STATE$
\***   (!!!NOTE!!!  The use of lower case letters is necessary to allow us
\***    to use more than 26 data states.  The characters between Z and a are
\***    the ASCII characters that occur between these two letters)
\***
\***   MODULE.EXIT:
\***     return to calling module
\***
\***   PROGRAM.EXIT:
\***     stop
\***
\******************************************************************************

ON ERROR GOTO ERROR.DETECTED

      IF MATCH(RECEIVE.STATE$,"RSTUVWXYZ[\]^_`abcdef",1) > 0 THEN BEGIN ! ELC
         ON (ASC(RECEIVE.STATE$) - ASC("Q")) GOSUB                      \
               RECEIVED.CHECK.VERSION.REQUEST,                          \
               RECEIVED.PROGRAM.TRAILER,                                \
               RECEIVED.DIRECT.ORDERS.REQUEST,                          \
               RECEIVED.DIRECT.FILE.HEADER,                             \
               RECEIVED.DIRECT.ORDERS.HEADER,                           \
               RECEIVED.DIRECT.ORDERS.DETAIL,                           \
               RECEIVED.DIRECT.ORDERS.TRAILER,                          \
               RECEIVED.DIRECT.FILE.TRAILER,                            \
               RECEIVED.DIRECT.EOT,                                     \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               DUMMY.ROUTINE,                                           \ ELC
               RECEIVED.UOD.FILE.HEADER,                                \ ELC
               RECEIVED.UOD.HEADER,                                     \ ELC
               RECEIVED.UOD.DETAIL,                                     \ ELC
               RECEIVED.UOD.TRAILER,                                    \ ELC
               RECEIVED.UOD.FILE.TRAILER,                               \ ELC
               RECEIVED.UOD.EOT                                         ! ELC
         GOTO MODULE.EXIT
      ENDIF

MODULE.EXIT:

   IF RECEIVE.STATE$ = "*" THEN BEGIN
      GOSUB CREATE.DIRECT.TEMP.STKMQ
   ENDIF

   EXIT SUB

PROGRAM.EXIT:

   STOP


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   S U B R O U T I N E S                                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   RECEIVED.CHECK.VERSION.REQUEST:                          STATE : R
\***
\***      display "PDT support - LDT Check Version Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      Command PSS38 to hold PDT
\***
\***      Open LDT check file (LDTCF)
\***
\***      Read LDTCF
\***
\***      IF the version number from the LDT is either zeroes or is an earlier
\***      version than on the LDTCF then format version details from the LDTCF.
\***      If the LDT version number is greater than or equal to the version
\***      number on the LDTCF, then set version details to zeroes.
\***
\***      Close LDTCF
\***
\***      Command PSS38 to release LDT
\***
\***      Format a program header using version details
\***
\***      Send program header to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.CHECK.VERSION.REQUEST:

      SB.MESSAGE$ = "PDT Support - LDT Check Version Received"          ! ELC
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 19) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      GOSUB HOLD.PDT

      CURR.SESS.NUM% = LDTCF.SESS.NUM%
      CURRENT.KEY$ = ""
      IF END #LDTCF.SESS.NUM% THEN OPEN.ERROR
      OPEN LDTCF.FILE.NAME$ AS LDTCF.SESS.NUM% NOWRITE NODEL
      LDTCF.OPEN.FLAG$ = "Y"

      RC% = READ.LDTCF                                                  ! BLC
      IF RC% = 1 THEN GOTO READ.ERROR                                   ! BLC

      IF MID$(DATA.IN$,3,3) = "000" THEN BEGIN
         CSR.AUDIT.DATA$ = "LDT REQUESTED PROGRAM UPDATE AT " +         \
                           LEFT$(TIME$,2) + ":" +                       \
                           MID$(TIME$,3,2) + ":" +                      \
                           RIGHT$(TIME$,2) +                            \
                           " VERSION LOADING = " +                      \
                           LDTCF.VERSION.NO$ + LDTCF.VERSION.DATE$
         GOSUB LOG.TO.AUDIT.FILE
         VERSION.DETAILS$ = LDTCF.VERSION.NO$ + LDTCF.VERSION.DATE$
      ENDIF ELSE IF MID$(DATA.IN$,3,3) < LDTCF.VERSION.NO$ THEN BEGIN
                    CSR.AUDIT.DATA$ = "AUTOMATIC PROGRAM UPDATE AT " +  \
                                      LEFT$(TIME$,2) + ":" +            \
                                      MID$(TIME$,3,2) + ":" +           \
                                      RIGHT$(TIME$,2) +                 \
                                      " VERSION LOADING = " +           \
                                      LDTCF.VERSION.NO$ + LDTCF.VERSION.DATE$
                    GOSUB LOG.TO.AUDIT.FILE
                    VERSION.DETAILS$ = LDTCF.VERSION.NO$ + LDTCF.VERSION.DATE$
                 ENDIF ELSE BEGIN
                       VERSION.DETAILS$ = "000000000"
                 ENDIF

      CLOSE LDTCF.SESS.NUM%

      GOSUB RELEASE.PDT

      PIPE.OUT$ = "VPH" + VERSION.DETAILS$

      GOSUB SEND.TO.PSS38

   RETURN


\******************************************************************************
\***
\***   RECEIVED.PROGRAM.TRAILER:                                STATE : S
\***
\***      display "PDT support - LDT Program Trailer Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.PROGRAM.TRAILER:

      SB.MESSAGE$ = "PDT Support - LDT Program Trailer Received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 20) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      LDTAF.LINK.TYPE% = 3                                              ! FMJK
      IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                  \ FMJK
        (LDTAF.LINK.TYPE% OR 80H)                                       ! FMJK
      LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                           ! FMJK
      LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                           ! FMJK
      TEMP.TIME$ = TIME$                                                ! GMJK
      LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +             \ GMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +              \ GMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ GMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ GMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+ \ GMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )    ! GMJK
      LDTAF.DATA.VOLUME.1% = 0                                          ! FMJK
      LDTAF.DATA.VOLUME.1% = SIZE ("LDTPF")                             ! FDS
      LDTAF.DATA.VOLUME.2% = 0                                          ! FMJK
      GOSUB LOG.TO.LDTAF.FILE                                           ! FMJK

      CSR.AUDIT.DATA$ = "PROGRAM LOAD FINISHED AT " +                   \
                        LEFT$(TIME$,2) + ":" +                          \
                        MID$(TIME$,3,2) + ":" +                         \
                        RIGHT$(TIME$,2)
      GOSUB LOG.TO.AUDIT.FILE

   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.ORDERS.REQUEST:                          STATE : T
\***
\***      display "PDT support - DIRECT Order Request Received" on background
\***      screen
\***
\***      set up non variable portion of REC.CHECK$(21)
\***
\***      find length of record and build validation string
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      dimension tables
\***
\***      GOSUB HOLD.PDT to command PSS38 to 'hold' the PDT
\***
\***      GOSUB OPEN.FILES.FOR.DIRECT to open all relevant files for Direct
\***
\***      GOSUB BUILD.BUS.CENTRE.LETTER.TABLE to build a business centre table
\***      from the BCSMF
\***
\***      GOSUB BUILD.ORDER.TABLE.FROM.PDT to build all orders (maximum of 10)
\***      requested from the PDT
\***
\***      set ORDER.INDEX% to 1
\***      While the supplier is not found search through DIRSUP file using
\***      supplier number from each order.
\***
\***      If a supplier was found (doesn't matter which), set DIRWF check
\***      quantity for no order to DIRSUP maximum check quantity, otherwise
\***      set to zeroes.
\***
\***      GOSUB WRITE.DIRWF.FILE.HEADER
\***
\***      for each order in table
\***          GOSUB PROCESS.A.DIRECT.ORDER
\***      end for
\***
\***      log DIRECTS information to audit file
\***
\***      GOSUB WRITE.DIRWF.FILE.TRAILER
\***
\***      IF DIRWF file is open then close
\***
\***      GOSUB RELEASE.PDT
\***
\***      GOSUB TRANSMIT.DIRECT.FILE
\***
\***      GOSUB TIDY.UP.FOR.DIRECT
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.ORDERS.REQUEST:

      SB.MESSAGE$ = "PDT Support - DIRECT Order Request Received"
      GOSUB SB.BG.MESSAGE

      REC.CHECK$(21) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(6)

      RECORD.LENGTH% = LEN(DATA.IN$)

      NO.OF.REQUESTED.ORDERS% = ((RECORD.LENGTH% - 10)/11)
      FOR COUNT% = 1 TO NO.OF.REQUESTED.ORDERS%
          REC.CHECK$(21) = REC.CHECK$(21) + "N"+CHR$(6) + "N"+CHR$(4)   \
                           + "A"+CHR$(1)
      NEXT COUNT%

      IF FN.VALIDATE.DATA(DATA.IN$, 21) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      DIM ORDER.TABLE$(10)
      DIM DIRWF.DETAIL.TABLE$(1600)
      DIM BAR.CODE.TABLE$(9000)
      DIM ITEM.POSITION.TABLE$(9000)
      DIM HEADER.TABLE$(10)
      DIM BC.LETTER.TABLE$(16)

      GOSUB HOLD.PDT

      GOSUB OPEN.FILES.FOR.DIRECT

      GOSUB BUILD.BUS.CENTRE.LETTER.TABLE

      GOSUB BUILD.ORDER.TABLE.FROM.PDT

      ORDER.INDEX% = 0
      SUPPLIER.FOUND = FALSE
      WHILE NOT SUPPLIER.FOUND AND                                      \
            ORDER.INDEX% <> ORDERS.IN.TABLE%
            ORDER.INDEX% = ORDER.INDEX% + 1
            GOSUB LOOK.FOR.SUPPLIER
      WEND

      IF SUPPLIER.FOUND THEN BEGIN
         DIRWF.CHK.QTY.FOR.NO.ORDER$ = UNPACK$(DIRSUP.MAX.CHECK.QTY$)
      ENDIF ELSE BEGIN
            DIRWF.CHK.QTY.FOR.NO.ORDER$ = "0000"
      ENDIF

      GOSUB WRITE.DIRWF.FILE.HEADER

      FOR ORDER.INDEX% = 1 TO ORDERS.IN.TABLE%
          GOSUB PROCESS.A.DIRECT.ORDER
      NEXT ORDER.INDEX%

      GOSUB WRITE.DIRWF.FILE.TRAILER

      IF DIRWF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DIRWF.SESS.NUM%
         DIRWF.OPEN.FLAG$ = "N"
      ENDIF

      GOSUB RELEASE.PDT

      GOSUB TRANSMIT.DIRECT.FILE

      GOSUB TIDY.UP.FOR.DIRECT

   RETURN


\******************************************************************************
\***
\***   OPEN.FILES.FOR.DIRECT:
\***
\***      open all files required for DIRECT processing
\***      and create the DIRWF
\***
\***   RETURN
\***
\******************************************************************************

   OPEN.FILES.FOR.DIRECT:

      CURR.SESS.NUM% = IDF.SESS.NUM%
      IF END #IDF.SESS.NUM% THEN OPEN.ERROR
      OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%         \
           NOWRITE NODEL
      IDF.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = IEF.SESS.NUM%
      IF END #IEF.SESS.NUM% THEN OPEN.ERROR
      OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%         \
           NOWRITE NODEL
      IEF.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = STKMQ.SESS.NUM%
      IF END #STKMQ.SESS.NUM% THEN OPEN.ERROR
      OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512 READONLY    \ DLC
           NODEL APPEND
      STKMQ.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = BCSMF.SESS.NUM%
      IF END #BCSMF.SESS.NUM% THEN OPEN.ERROR
      OPEN BCSMF.FILE.NAME$ RECL 512 AS BCSMF.SESS.NUM%                 \
           NOWRITE NODEL
      BCSMF.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = DIRSUP.SESS.NUM%
      IF END #DIRSUP.SESS.NUM% THEN OPEN.ERROR
      OPEN DIRSUP.FILE.NAME$ KEYED RECL DIRSUP.RECL%                    \
           AS DIRSUP.SESS.NUM% NOWRITE NODEL
      DIRSUP.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = DIRORD.SESS.NUM%
      IF END #DIRORD.SESS.NUM% THEN OPEN.ERROR
      OPEN DIRORD.FILE.NAME$ KEYED RECL DIRORD.RECL%                    \
           AS DIRORD.SESS.NUM% NODEL
      DIRORD.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = DIRWF.SESS.NUM%
      IF END #DIRWF.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE DIRWF.FILE.NAME$ AS DIRWF.SESS.NUM%                \
             BUFFSIZE 10240 LOCKED
      DIRWF.OPEN.FLAG$ = "Y"

   RETURN


\******************************************************************************
\***
\***   BUILD.BUS.CENTRE.LETTER.TABLE:
\***
\***      initialize business centre table to blanks
\***
\***      set up variables for reading BCSMF by sector
\***
\***      ignore first sector.
\***      extract each BCSMF record from remaining sectors.
\***      add to business centre table if not a null key and not
\***      a pseudo business centre.
\***
\***      add a business centre letter of blank to table
\***
\***   RETURN
\***
\******************************************************************************

   BUILD.BUS.CENTRE.LETTER.TABLE:

      FOR BC.INDEX% = 1 TO 16
          BC.LETTER.TABLE$(BC.INDEX%) = " "
      NEXT BC.INDEX%

      NO.SECTORS% = SIZE (BCSMF.FILE.NAME$) / 512
      SECTOR.FORMAT$ =  "T5 C508"
      NULL.KEY$ = PACK$("00")
      BC.INDEX% = 0
      NO.RECORDS% = 508 / 33

      FOR SECTOR.NUMBER% = 2 TO NO.SECTORS%

          READ FORM SECTOR.FORMAT$;#BCSMF.SESS.NUM%,                     \
                                   SECTOR.NUMBER%;SECTOR$
          FOR RECORD.NUMBER%= 1 TO NO.RECORDS%
              RECORD$ = LEFT$(SECTOR$,33)
              SECTOR$ = RIGHT$(SECTOR$,LEN(SECTOR$)-33)
              IF LEFT$(RECORD$,1) <> NULL.KEY$                           \
                 AND MID$(RECORD$,29,1) <> "Y" THEN BEGIN
                 BC.INDEX% = BC.INDEX% + 1
                 BC.LETTER.TABLE$(BC.INDEX%) = LEFT$(RECORD$,1)
              ENDIF
          NEXT RECORD.NUMBER%
      NEXT SECTOR.NUMBER%

      BC.INDEX% = BC.INDEX% + 1
      BC.LETTER.TABLE$(BC.INDEX%) = " "
      BC.TOTAL% = BC.INDEX%

   RETURN


\******************************************************************************
\***
\***   BUILD.ORDER.TABLE.FROM.PDT:
\***
\***      set variables to 0
\***
\***      FOR each requested order
\***          set order found flag to "N"
\***          extract order details
\***          IF order is already in order table then
\***             set order found flag to "Y"
\***          ENDIF
\***          IF order found flag is "N" then
\***             add order to table
\***          ENDIF
\***      end for
\***
\***   RETURN
\***
\******************************************************************************

   BUILD.ORDER.TABLE.FROM.PDT:

      ORDERS.IN.TABLE% = 0
      START.OF.ORDER% = 0

      FOR INDEX% = 1 TO NO.OF.REQUESTED.ORDERS%
          ORDER.FOUND = FALSE
          START.OF.ORDER% = START.OF.ORDER% + 11
          ORDER.DETAILS$ = MID$(DATA.IN$,START.OF.ORDER%,11)
          FOR ORDER.INDEX% = 1 TO ORDERS.IN.TABLE%
              IF ORDER.DETAILS$ = ORDER.TABLE$(ORDER.INDEX%) THEN BEGIN
                 ORDER.FOUND = TRUE
              ENDIF
          NEXT ORDER.INDEX%
          IF ORDER.FOUND = FALSE THEN BEGIN
             ORDERS.IN.TABLE% = ORDERS.IN.TABLE% + 1
             ORDER.TABLE$(ORDERS.IN.TABLE%) = ORDER.DETAILS$
          ENDIF
      NEXT INDEX%

   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.FILE.HEADER:
\***
\***      set up variables for DIRWF file header
\***
\***      write DIRWF file header
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.FILE.HEADER:

      DIRWF.RECORD.TYPE$ = "FH"
      DIRWF.APPLICATION.NO$ = APPLICATION.NO$
      DIRWF.TERMINAL.NO$ = CURR.TERMINAL$
      DIRWF.NO.OF.ORDERS$ = RIGHT$("00" + STR$(ORDERS.IN.TABLE%),2)

      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.APPLICATION.NO$ +                           \
                      DIRWF.TERMINAL.NO$ +                              \
                      DIRWF.CHK.QTY.FOR.NO.ORDER$ +                     \
                      DIRWF.NO.OF.ORDERS$

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.FILE.TRAILER:
\***
\***      set up variables for DIRWF file trailer
\***
\***      write DIRWF file trailer
\***
\***      set the respond option to a LDT trailer to false
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.FILE.TRAILER:

      DIRWF.RECORD.TYPE$ = "FT"
      DIRWF.APPLICATION.NO$ = APPLICATION.NO$
      DIRWF.TERMINAL.NO$ = CURR.TERMINAL$
      DIRWF.NO.OF.ORDERS$ = RIGHT$("00" + STR$(ORDERS.IN.TABLE%),2)

      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.APPLICATION.NO$ +                           \
                      DIRWF.TERMINAL.NO$ +                              \
                      DIRWF.NO.OF.ORDERS$

      GOSUB WRITE.DIRWF.RECORD

      LDT.TRANSMIT = FALSE

   RETURN


\******************************************************************************
\***
\***   PROCESS.A.DIRECT.ORDER:
\***
\***      set REJECT.REASON$ to blank
\***      set ITEM.TOTAL% to zero
\***      set BAR.CODE.TOTAL% to zero
\***      set SINGLES.BOOKED.IN% to zero
\***
\***      GOSUB LOG.REQUESTED.ORDER to add order details to audit data string
\***
\***      GOSUB LOOK.FOR.SUPPLIER to find supplier on DIRSUP
\***
\***      IF a supplier was not found then
\***         set reason for rejection to "S"
\***         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER
\***         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER
\***         RETURN
\***      ENDIF
\***
\***      GOSUB BUILD.ORDER.HEADER.TABLE to extract orders from
\***            DIRORD using requested orders
\***
\***      IF there are no entries in header table then
\***         set reason for rejection to "O"
\***         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER
\***         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER
\***         RETURN
\***      ENDIF
\***
\***      GOSUB PROCESS.ORDER.HEADER.TABLE to process orders and
\***            write DIRWF header record
\***
\***      GOSUB PROCESS.ITEMS to extract all items, write DIRWF
\***      detail records and add barcodes to barcode table.
\***
\***      redimension tables ready for next order
\***
\***   RETURN
\***
\******************************************************************************

   PROCESS.A.DIRECT.ORDER:

      REJECT.REASON$ = " "
      ITEM.TOTAL% = 0
      BAR.CODE.TOTAL% = 0
      SINGLES.BOOKED.IN% = 0

      GOSUB LOG.REQUESTED.ORDER

      GOSUB LOOK.FOR.SUPPLIER

      IF NOT SUPPLIER.FOUND THEN BEGIN
         REJECT.REASON$ = "S"
         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER
         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER
         RETURN
      ENDIF

      GOSUB BUILD.ORDER.HEADER.TABLE

      IF ASN.ACTIVE%                                                    \ 1.6NWB
     AND SUPERCEDED% THEN BEGIN                                         ! 1.6NWB
         REJECT.REASON$ = "C"                                           ! 1.6NWB
         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER                            ! 1.6NWB
         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER                            ! 1.6NWB
         RETURN                                                         ! 1.6NWB
      ENDIF                                                             ! 1.6NWB

      IF HEADER.TOTAL% = 0 THEN BEGIN
         REJECT.REASON$ = "O"
         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER
         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER
         RETURN
      ENDIF

      GOSUB PROCESS.ORDER.HEADER.TABLE

      GOSUB PROCESS.ITEMS

      IF REJECT.REASON$ = " " AND                                       \
         NOT ORDER.HEADER.WRITTEN THEN BEGIN
         REJECT.REASON$ = "O"
         GOSUB WRITE.DIRWF.REJECT.ORD.HEADER
         GOSUB WRITE.DIRWF.DUMMY.ORD.TRAILER
      ENDIF

      IF REJECT.REASON$ = " " THEN BEGIN
         GOSUB PROCESS.BAR.CODE.TABLE
         GOSUB WRITE.DIRWF.ORDER.TRAILER
      ENDIF

      DIM BAR.CODE.TABLE$(9000)
      DIM DIRWF.DETAIL.TABLE$(1600)
      DIM ITEM.POSITION.TABLE$(9000)
      DIM HEADER.TABLE$(10)

   RETURN


\******************************************************************************
\***
\***   LOG.REQUESTED.ORDER:
\***
\***      set up CSRAF audit line with direct order information
\***      write to audit file
\***
\***   RETURN
\***
\******************************************************************************

   LOG.REQUESTED.ORDER:

      CSR.AUDIT.DATA$ = "DIRECT ORDER REQUESTED: " +                    \
                        " SUPPLIER NO = " +                             \
                        LEFT$(ORDER.TABLE$(ORDER.INDEX%),6) +           \
                        " ORDER NO = " +                                \
                        MID$(ORDER.TABLE$(ORDER.INDEX%),7,5)

      GOSUB LOG.TO.AUDIT.FILE

   RETURN


\******************************************************************************
\***
\***   LOOK.FOR.SUPPLIER:
\***
\***
\***      pack supplier number from order table and set other variables
\***
\***      While the supplier has not been found and
\***            there are business centres left to check
\***            increment BC.INDEX
\***            format DIRSUP key using business centre letter and
\***                   packed supplier number
\***            key read DIRSUP
\***            IF record not found then
\***               continue with next business centre letter
\***            ELSE
\***               set SUPPLIER.FOUND to true
\***            ENDIF
\***      Wend
\***
\***   RETURN
\***
\******************************************************************************

   LOOK.FOR.SUPPLIER:

      BC.INDEX% = 0
      SUPPLIER.FOUND = FALSE
      SUPPLIER.NO.PACKED$ = PACK$(LEFT$(ORDER.TABLE$(ORDER.INDEX%),6))
      CURR.SESS.NUM% = DIRSUP.SESS.NUM%

      WHILE NOT SUPPLIER.FOUND AND                                    \
            BC.INDEX% <> BC.TOTAL%
            BC.INDEX% = BC.INDEX% + 1
            DIRSUP.RECKEY$ = BC.LETTER.TABLE$(BC.INDEX%) + SUPPLIER.NO.PACKED$
            RC% = READ.DIRSUP                                           ! BLC
            IF RC% = 1 THEN GOTO SUPPLIER.NOT.FOUND                     ! BLC
            SUPPLIER.FOUND = TRUE
   SUPPLIER.NOT.FOUND:
      WEND

      RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.REJECT.ORD.HEADER:
\***
\***      set up variables for DIRWF header
\***
\***      write DIRWF header
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.REJECT.ORD.HEADER:

      DIRWF.RECORD.TYPE$ = "OH"
      DIRWF.SUPPLIER.NUMBER$ = LEFT$(ORDER.TABLE$(ORDER.INDEX%),6)
      DIRWF.ORDER.NUMBER$ = MID$(ORDER.TABLE$(ORDER.INDEX%),7,4)
      DIRWF.ORDER.SUFFIX$ = RIGHT$(ORDER.TABLE$(ORDER.INDEX%),1)
      DIRWF.ITEMS.IN.ORDER$ = "0000"
      DIRWF.SINGLES.IN.ORDER$ = "00000"
      DIRWF.ITEMS.BOOKED.IN$ = "0000"
      DIRWF.SINGLES.BOOKED.IN$ = "00000"
      IF REJECT.REASON$ = "S" THEN BEGIN
         DIRWF.SUPPLIER.NAME$ = "UNKNOWN   "
         DIRWF.HIGH.QTY.LIMIT$ = "000"
         DIRWF.QTY.LIMIT$ = "000"
         DIRWF.DISCREP.QTY$ = "000"
         DIRWF.DISCREP.PERC$ = "000"
      ENDIF ELSE BEGIN
         DIRWF.SUPPLIER.NAME$ = DIRSUP.SUPPLIER.NAME$
         DIRWF.HIGH.QTY.LIMIT$ = RIGHT$(UNPACK$(DIRSUP.MAX.CHECK.QTY$),3)
         DIRWF.QTY.LIMIT$ = RIGHT$(UNPACK$(DIRSUP.CHECK.QTY$),3)
         DIRWF.DISCREP.QTY$ = RIGHT$(UNPACK$(DIRSUP.DISCREPANCY.QTY$),3)
         DIRWF.DISCREP.PERC$ = RIGHT$(UNPACK$(DIRSUP.DISCREPANCY.PERC$),3)
      ENDIF
      DIRWF.REJECT.REASON$ = REJECT.REASON$
      DIRWF.ORDER.FLAG$ = " "

      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.SUPPLIER.NUMBER$ +                          \
                      DIRWF.ORDER.NUMBER$ +                             \
                      DIRWF.ORDER.SUFFIX$ +                             \
                      DIRWF.ITEMS.IN.ORDER$ +                           \
                      DIRWF.SINGLES.IN.ORDER$ +                         \
                      DIRWF.ITEMS.BOOKED.IN$ +                          \
                      DIRWF.SINGLES.BOOKED.IN$ +                        \
                      DIRWF.SUPPLIER.NAME$ +                            \
                      DIRWF.HIGH.QTY.LIMIT$ +                           \
                      DIRWF.QTY.LIMIT$ +                                \
                      DIRWF.DISCREP.QTY$ +                              \
                      DIRWF.DISCREP.PERC$ +                             \
                      DIRWF.REJECT.REASON$ +                            \
                      DIRWF.ORDER.FLAG$

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.DUMMY.ORD.TRAILER:
\***
\***      set up variables for DIRWF trailer
\***
\***      write DIRWF trailer
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.DUMMY.ORD.TRAILER:

      DIRWF.RECORD.TYPE$ = "OT"
      DIRWF.NO.OF.BARCODES$ = "00000"

      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.NO.OF.BARCODES$

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   BUILD.ORDER.HEADER.TABLE:
\***
\***
\***      set header table index to 0
\***
\***      set up first part of DIRORD key from order table
\***
\***      For each business centre letter in table
\***          format a DIRORD record key using key details from
\***                 order table, business letter table and
\***                 source of record set to " "
\***          key read DIRORD
\***          IF record found then
\***             store DIRORD record key in header table
\***          ENDIF
\***      end for
\***
\***      manual direct orders will have a business centre letter set
\***      to blank and a source of record set to "D"
\***
\***      key read DIRORD again for a manual direct order
\***      if DIRORD not found then GOTO MANUAL.ORD.NOT.FOUND
\***
\***      if the header table has only one entry then
\***         set the manual order flag to "M" in header table
\***      else
\***         if the header table has no entries then
\***            add an entry with manual order flag set to " "
\***         endif
\***      endif
\***
\***      (if the header table has more than 1 entry it is not
\***       possible to match the manual order against a particular
\***       entry.  In this case, the manual order is ignored.)
\***
\***   MANUAL.ORD.NOT.FOUND:
\***
\***   RETURN
\***
\******************************************************************************

   BUILD.ORDER.HEADER.TABLE:

      HEADER.TOTAL% = 0
      SUPERCEDED% = 0                                                   ! 1.6NWB
      ORDER.DETAILS$ = PACK$(LEFT$(ORDER.TABLE$(ORDER.INDEX%),10)) +     \
                       RIGHT$(ORDER.TABLE$(ORDER.INDEX%),1)
      CURR.SESS.NUM% = DIRORD.SESS.NUM%

      FOR BC.INDEX% = 1 TO BC.TOTAL%
          DIRORD.RECKEY$ = ORDER.DETAILS$ +                              \
                           BC.LETTER.TABLE$(BC.INDEX%) +                 \
                           " " + PACK$("00")
          RC% = READ.DIRORD                                             ! HLC
          IF RC% = 1 THEN GOTO DIRORD.NOT.FOUND                         ! HLC
          IF DIRORD.SUPERCEDED$ = "Y" THEN SUPERCEDED% = -1             ! 1.6NWB
          HEADER.TOTAL% = HEADER.TOTAL% + 1
          HEADER.TABLE$(HEADER.TOTAL%) = DIRORD.RECKEY$ + " "
   DIRORD.NOT.FOUND:
      NEXT BC.INDEX%

      DIRORD.RECKEY$ = ORDER.DETAILS$ +  " " + "D" + PACK$("00")
      RC% = READ.DIRORD                                                 ! HLC
      IF RC% = 1 THEN GOTO MANUAL.ORD.NOT.FOUND                         ! HLC

      IF HEADER.TOTAL% = 1 THEN BEGIN
         HEADER.TABLE$(1) = LEFT$(HEADER.TABLE$(1),9) + "M"
      ENDIF ELSE BEGIN
         IF HEADER.TOTAL% = 0 THEN BEGIN
            HEADER.TOTAL% = HEADER.TOTAL% + 1
            HEADER.TABLE$(HEADER.TOTAL%) = DIRORD.RECKEY$ + " "
         ENDIF
      ENDIF

   MANUAL.ORD.NOT.FOUND:

      RETURN


\******************************************************************************
\***
\***   PROCESS.ORDER.HEADER.TABLE:
\***
\***      format DIRSUP record key from first entry in header table
\***      set SUPPLIER.FOUND to false
\***      set BC.INDEX% to 0
\***
\***      key read DIRSUP
\***
\***      while DIRSUP record not found then
\***         key read DIRSUP for each business centre in turn
\***      wend
\***
\***      key read DIRORD
\***
\***      format a DIRWF header record
\***
\***      if more than one entry in header table
\***          for each remaining entry in header table
\***              add items in order to total number of items
\***              add singles in order to total number of singles
\***          end for
\***          set lines booked in so far to 0
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   PROCESS.ORDER.HEADER.TABLE:

      DIRSUP.RECKEY$ = MID$(HEADER.TABLE$(1),7,1) +                    \
                       LEFT$(HEADER.TABLE$(1),3)
      SUPPLIER.FOUND = FALSE
      BC.INDEX% = 0

      CURR.SESS.NUM% = DIRSUP.SESS.NUM%
      RC% = READ.DIRSUP                                                 ! BLC
      IF RC% = 1 THEN GOTO TRY.ANOTHER.BC                               ! BLC
      SUPPLIER.FOUND = TRUE

  TRY.ANOTHER.BC:

      WHILE NOT SUPPLIER.FOUND AND                                      \
        BC.INDEX% <> BC.TOTAL%
        BC.INDEX% = BC.INDEX% + 1
        DIRSUP.RECKEY$ = BC.LETTER.TABLE$(BC.INDEX%) + RIGHT$(DIRSUP.RECKEY$,3)
        IF END# DIRSUP.SESS.NUM% THEN SUPPLIER.NOT.FOUND2
        RC% = READ.DIRSUP                                               ! BLC
        IF RC% = 1 THEN GOTO SUPPLIER.NOT.FOUND2                        ! BLC
        SUPPLIER.FOUND = TRUE
  SUPPLIER.NOT.FOUND2:
      WEND

      CURR.SESS.NUM% = DIRORD.SESS.NUM%
      DIRORD.RECKEY$ = LEFT$(HEADER.TABLE$(1),9)
      CURRENT.KEY$ = UNPACK$(LEFT$(DIRORD.RECKEY$,4))
      RC% = READ.DIRORD                                                 ! BLC
      IF RC% = 1 THEN GOTO READ.ERROR                                   ! BLC

      DIRWF.RECORD.TYPE$ = "OH"
      DIRWF.SUPPLIER.NUMBER$ = LEFT$(ORDER.TABLE$(ORDER.INDEX%),6)
      DIRWF.ORDER.NUMBER$ = MID$(ORDER.TABLE$(ORDER.INDEX%),7,4)
      DIRWF.ORDER.SUFFIX$ = RIGHT$(ORDER.TABLE$(ORDER.INDEX%),1)

      ITEMS.IN.ORDER% = VAL(UNPACK$(DIRORD.NO.ORDER.ITEM$))
      SINGLES.IN.ORDER% = VAL(UNPACK$(DIRORD.NO.ORDER.SNGL$))

      DIRWF.ITEMS.BOOKED.IN$ = UNPACK$(DIRORD.NO.ITEMS.BOOKED$)
      DIRWF.SUPPLIER.NAME$ = DIRSUP.SUPPLIER.NAME$
      DIRWF.HIGH.QTY.LIMIT$ = RIGHT$(UNPACK$(DIRSUP.MAX.CHECK.QTY$),3)
      DIRWF.QTY.LIMIT$ = RIGHT$(UNPACK$(DIRSUP.CHECK.QTY$),3)
      DIRWF.DISCREP.QTY$ = RIGHT$(UNPACK$(DIRSUP.DISCREPANCY.QTY$),3)
      DIRWF.DISCREP.PERC$ = RIGHT$(UNPACK$(DIRSUP.DISCREPANCY.PERC$),3)
      DIRWF.REJECT.REASON$ = " "
      DIRWF.ORDER.FLAG$ = RIGHT$(HEADER.TABLE$(1),1)

      IF HEADER.TOTAL% > 1 THEN BEGIN
         FOR INDEX% = 2 TO HEADER.TOTAL%
             DIRORD.RECKEY$ = LEFT$(HEADER.TABLE$(INDEX%),9)
             CURRENT.KEY$ = UNPACK$(LEFT$(DIRORD.RECKEY$,4))
             RC% = READ.DIRORD                                          ! BLC
             IF RC% = 1 THEN GOTO READ.ERROR                            ! BLC
             ITEMS.IN.ORDER% = ITEMS.IN.ORDER% +                        \
                               VAL(UNPACK$(DIRORD.NO.ORDER.ITEM$))
             SINGLES.IN.ORDER% = SINGLES.IN.ORDER% +                    \
                                 VAL(UNPACK$(DIRORD.NO.ORDER.SNGL$))
         NEXT INDEX%
         DIRWF.ITEMS.BOOKED.IN$ = "0000"
         DIRWF.SINGLES.BOOKED.IN$ = "00000"
      ENDIF

      DIRWF.ITEMS.IN.ORDER$ = FN.Z.PACK(STR$(ITEMS.IN.ORDER%),4)
      DIRWF.SINGLES.IN.ORDER$ = FN.Z.PACK(STR$(SINGLES.IN.ORDER%),5)

      RETURN


\******************************************************************************
\***
\***   BUILD.DIRWF.ORDER.HEADER:
\***
\***      build dirwf record
\***
\***   RETURN
\***
\******************************************************************************

   BUILD.DIRWF.ORDER.HEADER:

      DIRWF.HEADER.RECORD$ = DIRWF.RECORD.TYPE$ +                       \
                             DIRWF.SUPPLIER.NUMBER$ +                   \
                             DIRWF.ORDER.NUMBER$ +                      \
                             DIRWF.ORDER.SUFFIX$ +                      \
                             DIRWF.ITEMS.IN.ORDER$ +                    \
                             DIRWF.SINGLES.IN.ORDER$ +                  \
                             DIRWF.ITEMS.BOOKED.IN$ +                   \
                             DIRWF.SINGLES.BOOKED.IN$ +                 \
                             DIRWF.SUPPLIER.NAME$ +                     \
                             DIRWF.HIGH.QTY.LIMIT$ +                    \
                             DIRWF.QTY.LIMIT$ +                         \
                             DIRWF.DISCREP.QTY$ +                       \
                             DIRWF.DISCREP.PERC$ +                      \
                             DIRWF.REJECT.REASON$ +                     \
                             DIRWF.ORDER.FLAG$


   RETURN


\******************************************************************************
\***
\***   PROCESS.ITEMS:
\***
\***      set order header written to false
\***
\***      for each entry in header table
\***          for page number 1 to 99
\***              format a DIRORD record key using header table and page no
\***              if DIRORD not found then goto READ.NEXT.PAGE
\***              for each item in DIRORD record
\***                  GOSUB CHECK.ITEM
\***              next item
\***   READ.NEXT.PAGE:
\***          next page number
\***      next header table entry
\***
\***      if detail records were found then
\***         write DIRWF header record
\***         write all DIRWF detail records
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   PROCESS.ITEMS:

      ORDER.HEADER.WRITTEN = FALSE

      FOR HEADER.INDEX% = 1 TO HEADER.TOTAL%
          ORDER.DETAILS$ = LEFT$(HEADER.TABLE$(HEADER.INDEX%),8)
          FOR PAGE.NO% = 1 TO 99
              DIRORD.RECKEY$ = ORDER.DETAILS$ +                         \
                               PACK$(RIGHT$("00" + STR$(PAGE.NO%),2))
              CURR.SESS.NUM% = DIRORD.SESS.NUM%
              RC% = READ.DIRORD                                         ! BLC
              IF RC% = 1 THEN GOTO READ.NEXT.PAGE                       ! BLC
              FOR ITEM.COUNT% = 1 TO VAL(UNPACK$(DIRORD.ITEM.COUNT$))
                  GOSUB CHECK.ITEM
              NEXT ITEM.COUNT%
   READ.NEXT.PAGE:
          NEXT PAGE.NO%
      NEXT HEADER.INDEX%

      IF ITEM.TOTAL% <> 0 THEN BEGIN
         IF HEADER.TOTAL% = 1 THEN BEGIN
            DIRWF.SINGLES.BOOKED.IN$ = FN.Z.PACK(STR$(SINGLES.BOOKED.IN%),5)
         ENDIF
         GOSUB BUILD.DIRWF.ORDER.HEADER
         DIRWF.RECORD$ = DIRWF.HEADER.RECORD$
         GOSUB WRITE.DIRWF.RECORD
         ORDER.HEADER.WRITTEN = TRUE
         FOR ITEM.COUNT% = 1 TO ITEM.TOTAL%
             GOSUB WRITE.DIRWF.DETAIL.RECORD
         NEXT ITEM.COUNT%
      ENDIF

      RETURN


\******************************************************************************
\***
\***   CHECK.ITEM:
\***
\***      extract item code from DIRORD
\***
\***      if not blank item code then
\***         add details to table
\***         add barcodes to table
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   CHECK.ITEM:

       ITEM.CODE$ = RIGHT$(UNPACK$(DIRORD.ITEM.DETAILS$(ITEM.COUNT%,1)),7)

       IF ITEM.CODE$ = "0000000" THEN BEGIN
          RETURN
       ENDIF ELSE BEGIN
             ITEM.TOTAL% = ITEM.TOTAL% + 1
             GOSUB ADD.DETAIL.RECORD.TO.TABLE
             GOSUB ADD.BARCODES.TO.TABLE
       ENDIF

   RETURN


\******************************************************************************
\***
\***   ADD.DETAIL.RECORD.TO.TABLE:
\***
\***      format DIRWF details and add to table
\***
\***      sum all singles booked in
\***
\***   RETURN
\***
\******************************************************************************

   ADD.DETAIL.RECORD.TO.TABLE:

      DIRWF.ITEM.CODE$ =                                                \
                 RIGHT$(UNPACK$(DIRORD.ITEM.DETAILS$(ITEM.COUNT%,1)),7)
      DIRWF.EXPECTED.QTY$ =                                             \
                 FN.Z.PACK(STR$(DIRORD.ITEM.QTY%(ITEM.COUNT%,1)),4)
      DIRWF.PAGE.NO$ = FN.Z.PACK(STR$(PAGE.NO%),2)

      DIRWF.DETAIL.TABLE$(ITEM.TOTAL%) = DIRWF.ITEM.CODE$ +             \
                                         DIRWF.EXPECTED.QTY$ +          \
                                         DIRWF.PAGE.NO$

      SINGLES.BOOKED.IN% = SINGLES.BOOKED.IN% +                         \
                           DIRORD.ITEM.QTY%(ITEM.COUNT%,2) +            \
                           DIRORD.ITEM.QTY%(ITEM.COUNT%,3) +            \
                           DIRORD.ITEM.QTY%(ITEM.COUNT%,4)


   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.DETAIL.RECORD:
\***
\***      format DIRWF record
\***
\***      write DIRWF detail record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.DETAIL.RECORD:

      DIRWF.RECORD$ = "OD" + DIRWF.DETAIL.TABLE$(ITEM.COUNT%)

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   ADD.BARCODES.TO.TABLE:
\***
\***      read IDF using DIRORD boots code
\***      if IDF record not found then GOTO NOT.ON.IDF
\***
\***      set number of bar codes to IDF number of bar codes
\***      reformat ITEM.TOTAL% to a 3 digit string for adding to table
\***
\***      if number of bar codes < 3 then
\***         add 1 to bar code total
\***         move IDF first bar code into bar code table
\***         move position of this item code into item position table
\***         if number of bar codes = 2 then
\***            add 1 to bar code total
\***            move IDF second bar code into bar code table
\***            move position of this item code into item position table
\***         endif
\***      else
\***         add 1 to bar code total
\***         move IDF first bar code into bar code table
\***         move position of this item code into item position table
\***         add 1 to bar code total
\***         move IDF second bar code into bar code table
\***         move position of this item code into item position table
\***         set function bar code counter to 1
\***         set function key to next bar code to IDF second bar code
\***         set MORE.BAR.CODES to true
\***         set CURR.SESS.NUM% to IEF session number
\***         while there are more bar codes to read
\***               call READ.NEXT.IEF to get next bar code
\***               if bar code zeroes or
\***                  return code not = 0 then
\***                  set MORE.BAR.CODES to false
\***               else
\***                  add 1 to bar code total
\***                  move next IEF bar code into bar code table
\***                  move position of this item code into item position table
\***               endif
\***         wend
\***      endif
\***
\***   NOT.ON.IDF:
\***
\***   RETURN
\***
\******************************************************************************

   ADD.BARCODES.TO.TABLE:
       CURR.SESS.NUM% = IDF.SESS.NUM%
       IDF.BOOTS.CODE$ = DIRORD.ITEM.DETAILS$(ITEM.COUNT%,1)
       IF IDF.BOOTS.CODE$ = PACK$("00000000") THEN BEGIN
          GOTO NOT.ON.IDF
       ENDIF
       CURRENT.KEY$ = UNPACK$(IDF.BOOTS.CODE$)
       RC% = READ.IDF                                           ! BLC
       IF RC% = 1 THEN GOTO NOT.ON.IDF                          ! BLC
       NO.OF.BAR.CODES% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))
       ITEM.POS.STRING$ = FN.Z.PACK(STR$(ITEM.TOTAL%),3)
       IF NO.OF.BAR.CODES% < 3 THEN BEGIN
          BAR.CODE.TOTAL% = BAR.CODE.TOTAL% + 1
          BAR.CODE.TABLE$(BAR.CODE.TOTAL%) = IDF.FIRST.BAR.CODE$
          ITEM.POSITION.TABLE$(BAR.CODE.TOTAL%) = ITEM.POS.STRING$
          IF NO.OF.BAR.CODES% = 2 THEN BEGIN
             BAR.CODE.TOTAL% = BAR.CODE.TOTAL% + 1
             BAR.CODE.TABLE$(BAR.CODE.TOTAL%) = IDF.SECOND.BAR.CODE$
             ITEM.POSITION.TABLE$(BAR.CODE.TOTAL%) = ITEM.POS.STRING$
          ENDIF
       ENDIF ELSE BEGIN
             BAR.CODE.TOTAL% = BAR.CODE.TOTAL% + 1
             BAR.CODE.TABLE$(BAR.CODE.TOTAL%) = IDF.FIRST.BAR.CODE$
             ITEM.POSITION.TABLE$(BAR.CODE.TOTAL%) = ITEM.POS.STRING$
             BAR.CODE.TOTAL% = BAR.CODE.TOTAL% + 1
             BAR.CODE.TABLE$(BAR.CODE.TOTAL%) = IDF.SECOND.BAR.CODE$
             ITEM.POSITION.TABLE$(BAR.CODE.TOTAL%) = ITEM.POS.STRING$
             F11.CURRENT.COUNT% = 1
             F11.NEXT.BAR.CODE$ = IDF.SECOND.BAR.CODE$
             MORE.BAR.CODES = TRUE
             CURR.SESS.NUM% = IEF.SESS.NUM%
             WHILE MORE.BAR.CODES
                   RC% = READ.NEXT.IEF(F11.NEXT.BAR.CODE$,              \ BLC
                                       NO.OF.BAR.CODES%,                \
                                       RIGHT$(UNPACK$(IDF.BOOTS.CODE$),7))
                   IF UNPACK$(F11.NEXT.BAR.CODE$) = STRING$(12,"0")     \
                      OR RC% <> 0 THEN BEGIN
                      MORE.BAR.CODES = FALSE
                   ENDIF ELSE BEGIN
                      BAR.CODE.TOTAL% = BAR.CODE.TOTAL% + 1
                      BAR.CODE.TABLE$(BAR.CODE.TOTAL%) = F11.NEXT.BAR.CODE$
                      ITEM.POSITION.TABLE$(BAR.CODE.TOTAL%) = ITEM.POS.STRING$
                   ENDIF
             WEND
       ENDIF

   NOT.ON.IDF:

       RETURN


\******************************************************************************
\***
\***   PROCESS.BAR.CODE.TABLE:
\***
\***      sort bar code table and corresponding item position table
\***      into ascending order using COMBSORT.
\***
\***      for each entry in barcode table
\***          GOSUB WRITE.DIRWF.BARCODE.RECORD
\***      end for
\***
\***   RETURN
\***
\******************************************************************************

   PROCESS.BAR.CODE.TABLE:

       IF BAR.CODE.TOTAL% = 0 THEN BEGIN
          RETURN
       ENDIF

       MAX% = BAR.CODE.TOTAL%
       FINISHED = FALSE

       GAP = MAX%
       WHILE NOT FINISHED
             GAP = INT(GAP / 1.3)
             IF GAP = 0 THEN GAP = 1
             IF GAP = 9 OR GAP = 10 THEN GAP = 11
             SWAPPED = FALSE
             FOR A%=1 TO (MAX% - GAP)
                 B% = A% + GAP
                 IF BAR.CODE.TABLE$(A%) > BAR.CODE.TABLE$(B%) THEN BEGIN
                    TEMP$ = BAR.CODE.TABLE$(A%)
                    BAR.CODE.TABLE$(A%) = BAR.CODE.TABLE$(B%)
                    BAR.CODE.TABLE$(B%) = TEMP$
                    TEMP$ = ITEM.POSITION.TABLE$(A%)
                    ITEM.POSITION.TABLE$(A%) = ITEM.POSITION.TABLE$(B%)
                    ITEM.POSITION.TABLE$(B%) = TEMP$
                    SWAPPED = TRUE
                 ENDIF
             NEXT A%
             IF (NOT SWAPPED) AND GAP = 1 THEN FINISHED = TRUE
       WEND

       FOR INDEX% = 1 TO BAR.CODE.TOTAL%
           GOSUB WRITE.DIRWF.BARCODE.RECORD
       NEXT INDEX%

       RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.BARCODE.RECORD:
\***
\***      set up variables for DIRWF barcode record
\***
\***      write DIRWF barcode record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.BARCODE.RECORD:

      DIRWF.RECORD.TYPE$ = "BD"
      DIRWF.BARCODE$ = "0" + UNPACK$(BAR.CODE.TABLE$(INDEX%))
      DIRWF.ITEM.POSITION$ = ITEM.POSITION.TABLE$(INDEX%)
      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.BARCODE$ +                                  \
                      DIRWF.ITEM.POSITION$

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.ORDER.TRAILER:
\***
\***      set up variables for DIRWF trailer
\***
\***      write DIRWF trailer record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.ORDER.TRAILER:

      DIRWF.RECORD.TYPE$ = "OT"
      DIRWF.NO.OF.BARCODES$ = FN.Z.PACK(STR$(BAR.CODE.TOTAL%),5)
      DIRWF.RECORD$ = DIRWF.RECORD.TYPE$ +                              \
                      DIRWF.NO.OF.BARCODES$

      GOSUB WRITE.DIRWF.RECORD

   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.FILE.HEADER
\***
\***      display message "PDT support - DIRECT File Header Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*"
\***      and RETURN
\***
\***      create LDTBF file
\***
\***      if received terminal number or application number is invalid then
\***      set RECEIVE.STATE$ to "*" and RETURN
\***
\***      set number of direct headers to 0
\***      store number of orders in file
\***
\***      dimension an order line table for 16 entries
\***
\***      reset fields
\***
\***      GOSUB CREATE.DIRECT.TEMP.STKMQ
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.FILE.HEADER:

      LDTBF.OPEN.FLAG$ = "Y"                                            ! CLC
      LDTBF.NUM.OF.ITEMS% = 0                                           ! CLC

      SB.MESSAGE$ = "PDT Support - DIRECT File Header Received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 22) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      IF MID$(DATA.IN$,3,2) <> APPLICATION.NO$                          \
         OR MID$(DATA.IN$,5,6) <> CURR.TERMINAL$ THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      CURR.SESS.NUM% = LDTBF.SESS.NUM%                                  ! CLC
      IF END #LDTBF.SESS.NUM% THEN CREATE.ERROR                         ! CLC
      CREATE LDTBF.FILE.NAME$                                           \ CLC
         AS LDTBF.SESS.NUM% LOCKED                                      !1.2SH
      LDTBF.WRITTEN.FLAG$ = "N"                                         ! CLC

      ORDERS.FOUND% = 0
      ORDERS.EXPECTED% = VAL(MID$(DATA.IN$,11,2))

      DIM ORDER.LINE.TABLE$(16)

      LINE.TOTAL% = 0
      LINES.BOOKED.IN% = 0
      LINES.ADDED% = 0
      SINGLES.ADDED% = 0

      GOSUB CREATE.DIRECT.TEMP.STKMQ


   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.ORDERS.HEADER
\***
\***      display message "PDT support - DIRECT Order Header Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*"
\***      and RETURN
\***
\***      increment number of direct headers
\***
\***      store supplier number, order number, order suffix,
\***            booking in times and number of items booked in.
\***
\***      set type 17 written flag to false
\***
\***      set first time flag to true
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.ORDERS.HEADER:

      SB.MESSAGE$ = "PDT Support - DIRECT order header received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 23) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      ORDERS.FOUND% = ORDERS.FOUND% + 1

!     IF STORE.NUMBER$ = "2999" THEN BEGIN                                    !1.3BG
!
!         IF LSSST.OPEN.FLAG$ <> "Y" THEN BEGIN                                !1.3BG  !1.5CS
!            IF END #LSSST.SESS.NUM% THEN OPEN.ERROR                           !1.3BG  !1.5CS
!            CURR.SESS.NUM% = LSSST.SESS.NUM%                                  !1.3BG  !1.5CS
!            CREATE LSSST.FILE.NAME$ AS LSSST.SESS.NUM% LOCKED                 !1.3BG  !1.5CS
!            LSSST.OPEN.FLAG$ = "Y"                                            !1.3BG  !1.5CS
!         ENDIF                                                                !1.3BG  !1.5CS
!                                                                                      !1.5CS
!         IF STOCK.OPEN.FLAG$ <> "Y" THEN BEGIN                                !1.3BG  !1.5CS
!            IF END #STOCK.SESS.NUM% THEN OPEN.ERROR                           !1.3BG  !1.5CS
!            CURR.SESS.NUM% = STOCK.SESS.NUM%                                  !1.3BG  !1.5CS
!            OPEN STOCK.FILE.NAME$ KEYED RECL STOCK.RECL%                      \1.3BG  !1.5CS
!                 AS STOCK.SESS.NUM% NODEL                                     !1.3BG  !1.5CS
!            STOCK.OPEN.FLAG$ = "Y"                                            !1.3BG  !1.5CS
!         ENDIF                                                                !1.3BG  !1.5CS
!                                                                                      !1.5CS
!         IF IMSTC.OPEN.FLAG$ <> "Y" THEN BEGIN                                !1.3BG  !1.5CS
!            IF END #IMSTC.SESS.NUM% THEN OPEN.ERROR                           !1.3BG  !1.5CS
!            CURR.SESS.NUM% = IMSTC.SESS.NUM%                                  !1.3BG  !1.5CS
!            OPEN IMSTC.FILE.NAME$ KEYED RECL IMSTC.RECL%                      \1.3BG  !1.5CS
!                 AS IMSTC.SESS.NUM% NODEL                                     !1.3BG  !1.5CS
!            IMSTC.OPEN.FLAG$ = "Y"                                            !1.3BG  !1.5CS
!         ENDIF                                                                !1.3BG  !1.5CS
!                                                                                      !1.5CS
!         IF IRF.OPEN.FLAG$ <> "Y" THEN BEGIN                                  !1.3BG  !1.5CS
!            IF END #IRF.SESS.NUM% THEN OPEN.ERROR                             !1.3BG  !1.5CS
!            CURR.SESS.NUM% = IRF.SESS.NUM%                                    !1.3BG  !1.5CS
!            OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%         \1.3BG  !1.5CS
!                 NOWRITE NODEL                                                !1.3BG  !1.5CS
!            IRF.OPEN.FLAG$ = "Y"                                              !1.3BG  !1.5CS
!         ENDIF                                                                !1.3BG  !1.5CS
!                                                                                      !1.5CS
!         IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                                !1.3BG  !1.5CS
!            IF END #STKMQ.SESS.NUM% THEN OPEN.ERROR                           !1.3BG  !1.5CS
!            CURR.SESS.NUM% = STKMQ.SESS.NUM%                                  !1.3BG  !1.5CS
!            OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% NODEL APPEND             !1.3BG  !1.5CS
!            STKMQ.OPEN.FLAG$ = "Y"                                            !1.3BG  !1.5CS
!         ENDIF                                                                !1.3BG  !1.5CS
!                                                                                      !1.5CS
!      ENDIF ELSE BEGIN                                                        !1.3BG  !1.5CS

         SAVED.SUPPLIER.NO$ = MID$(DATA.IN$,3,6)
         SAVED.ORDER.NO$ = MID$(DATA.IN$,9,4)
         SAVED.ORDER.SUFFIX$ = MID$(DATA.IN$,13,1)
         BOOKING.STARTED$ = MID$(DATA.IN$,14,4)
         BOOKING.ENDED$ = MID$(DATA.IN$,18,4)
         ITEMS.BOOKED.IN% = VAL(MID$(DATA.IN$,22,4))

         TYPE.17.WRITTEN = FALSE

         FIRST.TIME = TRUE

!      ENDIF                                                                   !1.3BG !1.5CS

   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.ORDERS.DETAIL
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*"
\***      and RETURN
\***
\***      extract quantity fields from record
\***
\***      if any of the quantities are non-zero then
\***         add 1 to number of lines booked in
\***      endif
\***
\***      write the information to the LDTBF
\***
\***      extract page number from record
\***
\***      if this is the first direct orders detail record processed then
\***         save page number
\***      endif
\***
\***      if page number is 0 (unexpected line) then
\***         add 1 to lines added
\***         add quantities to singles added
\***      endif
\***
\***      if page number is not = saved page number or
\***         number of entries in order line table = 16 then
\***         GOSUB WRITE.DRSMQ.TYPE.17
\***         clear order line table
\***         set line total to 0
\***         GOSUB ADD.TO.ORDER.LINE.TABLE to add this entry
\***         save page number
\***         if page number is 1 then
\***            GOSUB WRITE.DRSMQ.TYPE.19
\***            set lines booked in, lines added and singles added to 0
\***         endif
\***      else
\***         GOSUB ADD.TO.ORDER.LINE.TABLE to add this entry
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.ORDERS.DETAIL:

      IF FN.VALIDATE.DATA(DATA.IN$, 24) = 0 THEN BEGIN
            RECEIVE.STATE$ = "*"
            RETURN
      ENDIF

!     IF STORE.NUMBER$ = "2999" THEN BEGIN                                    !1.3BG  !1.5CS

!         GOSUB LSS.COUNTS                                                     !1.3BG !1.5CS

!      ENDIF ELSE BEGIN                                                        !1.3BG  !1.5CS

         QUANTITY.GOOD%    = VAL(MID$(DATA.IN$,19,4))
         QUANTITY.DAMAGED% = VAL(MID$(DATA.IN$,23,4))
         QUANTITY.STOLEN%  = VAL(MID$(DATA.IN$,27,4))

         IF DIRORD.OPEN.FLAG$ <> "Y" THEN BEGIN                            !1.2SH
            CURR.SESS.NUM% = DIRORD.SESS.NUM%                              !1.2SH
            IF END #DIRORD.SESS.NUM% THEN OPEN.ERROR                       !1.2SH
            OPEN DIRORD.FILE.NAME$ KEYED RECL DIRORD.RECL%                 \1.2SH
                 AS DIRORD.SESS.NUM% NODEL                                 !1.2SH
            DIRORD.OPEN.FLAG$ = "Y"                                        !1.2SH
         ENDIF                                                             !1.2SH

         IF LDTBF.HEADER.WRITTEN$ = "N" THEN BEGIN                         ! CLC
            LDTBF.RECORD.ID$    = "OH"                                     ! CLC
            LDTBF.SUPPLIER.NUM$ = PACK$(SAVED.SUPPLIER.NO$)                ! CLC
            LDTBF.ORDER.NUM$    = PACK$(SAVED.ORDER.NO$)                   ! CLC
            LDTBF.ORDER.SUFFIX$ = SAVED.ORDER.SUFFIX$                      ! CLC
            LDTBF.START.TIME$   = PACK$(BOOKING.STARTED$)                  !1.2SH
            LDTBF.END.TIME$     = PACK$(BOOKING.ENDED$)                    !1.2SH
            LDTBF.DATE$         = PACK$(DATE$)                             !1.2SH
            GOSUB GET.ALREADY.BOOKED.QTY                                   !1.2SH
            RC% = WRITE.LDTBF                                              ! CLC
            IF RC% = 1 THEN GOTO WRITE.ERROR                               ! CLC
            LDTBF.HEADER.WRITTEN$ = "Y"                                    ! CLC
            LDTBF.WRITTEN.FLAG$ = "Y"                                      ! CLC
         ENDIF                                                             ! CLC
         LDTBF.RECORD.ID$ = "OD"                                           ! CLC
         LDTBF.REF.CODE$  = PACK$(MID$(DATA.IN$,7,12))                     ! CLC
         LDTBF.DAMAGED.QTY% = QUANTITY.DAMAGED%                            ! CLC
         LDTBF.STOLEN.QTY%  = QUANTITY.STOLEN%                             ! CLC
         LDTBF.GOOD.QTY%    = QUANTITY.GOOD%                               !1.2SH
         GOSUB GET.ORDERED.QUANTITY                                        !1.2SH
         RC% = WRITE.LDTBF                                                 ! CLC
         IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! CLC
         LDTBF.NUM.OF.ITEMS% = LDTBF.NUM.OF.ITEMS% +                       \ CLC
                               QUANTITY.DAMAGED%   +                       \ CLC
                               QUANTITY.STOLEN%    +                       \1.2SH
                               QUANTITY.GOOD%                              !1.2SH

         IF QUANTITY.GOOD%    <> 0 OR                                      \
            QUANTITY.DAMAGED% <> 0 OR                                      \
            QUANTITY.STOLEN%  <> 0 THEN BEGIN
             LINES.BOOKED.IN% = LINES.BOOKED.IN% + 1
         ENDIF

         IF FIRST.TIME THEN BEGIN
            SAVED.PAGE.NO$ = PAGE.NUMBER$
            FIRST.TIME = FALSE
         ENDIF

         IF PAGE.NUMBER$ = "00" THEN BEGIN
            LINES.ADDED% = LINES.ADDED% + 1
            SINGLES.ADDED% = SINGLES.ADDED% +                              \
                             QUANTITY.GOOD% +                              \
                             QUANTITY.DAMAGED% +                           \
                             QUANTITY.STOLEN%
         ENDIF

         IF SAVED.PAGE.NO$ <> PAGE.NUMBER$ OR                              \
            LINE.TOTAL% = 16 THEN BEGIN
            GOSUB WRITE.DRSMQ.TYPE.17
            DIM ORDER.LINE.TABLE$(16)
            LINE.TOTAL% = 0
            GOSUB ADD.TO.ORDER.LINE.TABLE
            SAVED.PAGE.NO$ = PAGE.NUMBER$
            IF PAGE.NUMBER$ = "01" THEN BEGIN
               GOSUB WRITE.DRSMQ.TYPE.19
               LINES.BOOKED.IN% = 0
               LINES.ADDED% = 0
               SINGLES.ADDED% = 0
            ENDIF
         ENDIF ELSE BEGIN
               GOSUB ADD.TO.ORDER.LINE.TABLE
         ENDIF

!      ENDIF                                                                   !1.3BG  !1.5CS

   RETURN

\*****************************************************************************
\***                                                                         *
\***   LSS.COUNTS                    COMMENTED OUT 1.5CS                     *
\***                                                                         *
\*****************************************************************************

!LSS.COUNTS:
!
!    STOCK.BOOTS.CODE$ = PACK$("00000000")                                         !1.3BG
!    LSSST.TSF$ = "     "                                                          !1.3BG
!    LSSST.STOCK.COUNT$ = "     "                                                  !1.3BG
!    !GET INPUT CODE FROM DEVICE                                                   !1.3BG
!    IF MID$(DATA.IN$, 5, 1) = "I" THEN BEGIN                                      !1.3BG
!       IRF.BAR.CODE$ = PACK$("0000000000" + MID$(DATA.IN$, 6, 12))                !1.3BG
!    ENDIF ELSE BEGIN                                                              !1.3BG
!       IF MID$(DATA.IN$, 5, 1) = "B" THEN BEGIN                                   !1.3BG
!          IRF.BAR.CODE$ = PACK$("000000000" + MID$(DATA.IN$, 6, 13))              !1.3BG
!       ENDIF                                                                      !1.3BG
!    ENDIF                                                                         !1.3BG
!    RC% = READ.IRF                                                                !1.3BG
!    IF RC% = 0 THEN BEGIN                                                         !1.3BG
!       STOCK.BOOTS.CODE$ = UNPACK$(IRF.BOOTS.CODE$)                               !1.3BG
!       RC% = CALC.BOOTS.CODE.CHECK.DIGIT (STOCK.BOOTS.CODE$)                      !1.3BG
!       IF RC% = 0 AND F18.CHECK.DIGIT$ <> "A" THEN BEGIN                          !1.3BG
!          STOCK.BOOTS.CODE$ = STOCK.BOOTS.CODE$ + F18.CHECK.DIGIT$                !1.3BG
!          STOCK.BOOTS.CODE$ = PACK$(RIGHT$("00000000" + STOCK.BOOTS.CODE$, 8))    !1.3BG
!
!          !CHECK FOR IMSTC RECORD FOR MOST RECENT TSF                             !1.3BG
!          IMSTC.BAR.CODE$ = PACK$(STRING$(16, "0")) + IRF.BOOTS.CODE$             !1.3BG
!          RC% = READ.IMSTC                                                        !1.3BG
!          IF RC% = 0 THEN BEGIN                                                   !1.3BG
!             LSSST.TSF$ = RIGHT$("00000" + STR$(IMSTC.STOCK.FIGURE%), 5)          !1.3BG
!          ENDIF ELSE BEGIN                                                        !1.3BG
!             !NO IMSTC RECORD SO GET OFF STOCK FILE                               !1.3BG
!             RC% = READ.STOCK                                                     !1.3BG
!             IF RC% = 0 THEN BEGIN                                                !1.3BG
!                LSSST.TSF$ = RIGHT$("00000" + STR$(STOCK.STOCK.FIG%), 5)          !1.3BG
!             ENDIF                                                                !1.3BG
!          ENDIF                                                                   !1.3BG
!       ENDIF                                                                      !1.3BG
!
!       !IF WE HAVE A TSF, WRITE THE STOCK COUNT TO THE STOCK FILE                 !1.3BG
!       IF LSSST.TSF$ <> "     " THEN BEGIN                                        !1.3BG
!          LSSST.STOCK.COUNT$ = RIGHT$("00000" + MID$(DATA.IN$, 19, 5), 5)         !1.3BG
!          !WRITE STKMQ TYPE 11 RECORD TO REPLACE TSF                              !1.3BG
!          STKMQ.RECORD$ = CHR$(34) +           \                                  !1.3BG
!                          PACK$("11") +        \                                  !1.3BG
!                          CHR$(59) +           \                                  !1.3BG
!                          PACK$(DATE$) +       \                                  !1.3BG
!                          PACK$(TIME$) +       \                                  !1.3BG
!                          STOCK.BOOTS.CODE$ +  \                                  !1.3BG
!                          CHR$(59) +           \                                  !1.3BG
!                          LSSST.STOCK.COUNT$ + \                                  !1.3BG
!                          CHR$(34) +           \                                  !1.3BG
!                          CHR$(13) + CHR$(10)                                     !1.3BG
!          IF END# STKMQ.SESS.NUM% THEN WRITE.ERROR                                !1.3BG
!          CURR.SESS.NUM% = STKMQ.SESS.NUM%                                        !1.3BG
!          CURRENT.KEY$ = ""                                                       !1.3BG
!          CALL WRITE.STKMQ                                                        !1.3BG
!       ENDIF                                                                      !1.3BG
!    ENDIF                                                                         !1.3BG
!
!    !WRITE REPORT FILE RECORD                                                     !1.3BG
!    IF LSSST.OPEN.FLAG$="Y" THEN BEGIN                                            !1.3BG
!       LSSST.IRF.BAR.CODE$ = IRF.BAR.CODE$                                        !1.3BG
!       LSSST.STOCK.BOOTS.CODE$ = STOCK.BOOTS.CODE$                                !1.3BG
!       IF END #LSSST.SESS.NUM% THEN WRITE.ERROR                                   !1.3BG
!       CURR.SESS.NUM% = LSSST.SESS.NUM%                                           !1.3BG
!       CURRENT.KEY$ = ""                                                          !1.3BG
!       CALL WRITE.LSSST                                                           !1.3BG
!    ENDIF                                                                         !1.3BG
!
!RETURN


\*****************************************************************************
\***                                                                         *
\***   GET.AREADY.BOOKED.QTY                                                 *
\***                                                                         *
\*****************************************************************************

GET.ALREADY.BOOKED.QTY:                                                !1.2SH

   LDTBF.LOGGED.QTY% = 0                                               !1.2SH

   IF BCSMF.OPEN.FLAG$ <> "Y" THEN BEGIN                               !1.2SH
      CURR.SESS.NUM% = BCSMF.SESS.NUM%                                 !1.2SH
      IF END #BCSMF.SESS.NUM% THEN OPEN.ERROR                          !1.2SH
      OPEN BCSMF.FILE.NAME$ RECL 512 AS BCSMF.SESS.NUM%                \1.2SH
           NOWRITE NODEL                                               !1.2SH
      BCSMF.OPEN.FLAG$ = "Y"                                           !1.2SH
   ENDIF                                                               !1.2SH

   DIM BC.LETTER.TABLE$(16)                                            !1.2SH
   GOSUB BUILD.BUS.CENTRE.LETTER.TABLE                                 !1.2SH

   IF BCSMF.OPEN.FLAG$ = "Y" THEN BEGIN                                !1.2SH
      CLOSE BCSMF.SESS.NUM%                                            !1.2SH
      BCSMF.OPEN.FLAG$ = "N"                                           !1.2SH
   ENDIF                                                               !1.2SH

   ORDER.DETAILS$ = PACK$(SAVED.SUPPLIER.NO$) +                        \1.2SH
                    PACK$(SAVED.ORDER.NO$)    +                        \1.2SH
                    SAVED.ORDER.SUFFIX$                                !1.2SH
   CURR.SESS.NUM% = DIRORD.SESS.NUM%                                   !1.2SH

   TEMP$ = "  "                                                        !1.2SH
   FOR BC.INDEX% = 1 TO BC.TOTAL%                                      !1.2SH
      DIRORD.RECKEY$ = LEFT$(ORDER.DETAILS$              +             \1.2SH
                             BC.LETTER.TABLE$(BC.INDEX%) +             \1.2SH
                             " " + CHR$(0)               +             \1.2SH
                             STRING$(9," ")               ,9)          !1.2SH
      RC% = READ.DIRORD                                                !1.2SH
      IF RC% = 1 THEN GOTO DIRORD.REC.NOT.FOUND                        !1.2SH
      TEMP$ = BC.LETTER.TABLE$(BC.INDEX%) + " "                        !1.2SH
      LDTBF.LOGGED.QTY% = VAL(UNPACK$(DIRORD.NO.ITEMS.BOOKED$))        !1.2SH
      BC.INDEX% = BC.TOTAL%                                            !1.2SH
   DIRORD.REC.NOT.FOUND:                                               !1.2SH
   NEXT BC.INDEX%                                                      !1.2SH

   ! Check manual order                                                !1.2SH
   DIRORD.RECKEY$ = ORDER.DETAILS$ +  " " + "D" + PACK$("00")          !1.2SH
   RC% = READ.DIRORD                                                   !1.2SH
   IF RC% = 0 THEN BEGIN                                               !1.2SH
      LDTBF.LOGGED.QTY% = VAL(UNPACK$(DIRORD.NO.ITEMS.BOOKED$))        !1.2SH
      TEMP$ = " D"                                                     !1.2SH
   ENDIF                                                               !1.2SH

RETURN


\*****************************************************************************
\***                                                                         *
\***   GET.ORDERED.QUANTITY                                                  *
\***                                                                         *
\*****************************************************************************

GET.ORDERED.QUANTITY:                                                  !1.2SH

  PAGE.NUMBER$ = MID$(DATA.IN$,3,2)                                    !1.2SH

  LDTBF.ORDERED.QTY% = 0                                               !1.2SH

  ! TEMP$ = Business Centre Letter + Source                            !1.2SH
  DIRORD.RECKEY$ = ORDER.DETAILS$ + TEMP$ + PACK$(PAGE.NUMBER$)        !1.2SH
  RC% = READ.DIRORD                                                    !1.2SH
  IF RC% = 1 THEN GOTO NO.RECORD                                       !1.2SH

  FOR INDEX% = 1 TO 16                                                 !1.2SH
     IF (DIRORD.ITEM.DETAILS$(INDEX%,1) = STRING$(4,CHR$(0)) AND       \1.2SH
         DIRORD.ITEM.DETAILS$(INDEX%,2) = LDTBF.REF.CODE$)             \1.2SH
     OR                                                                \1.2SH
        (DIRORD.ITEM.DETAILS$(INDEX%,1) = RIGHT$(LDTBF.REF.CODE$,4) AND\1.2SH
         DIRORD.ITEM.DETAILS$(INDEX%,2) = STRING$(6,CHR$(0)))          \1.2SH
     THEN BEGIN                                                        !1.2SH
        LDTBF.ORDERED.QTY% = DIRORD.ITEM.QTY%(INDEX%,1)                !1.2SH
        INDEX% = 16                                                    !1.2SH
     ENDIF                                                             !1.2SH
  NEXT INDEX%                                                          !1.2SH

NO.RECORD:                                                             !1.2SH

RETURN                                                                 !1.2SH


\******************************************************************************
\***
\***   RECEIVED.DIRECT.ORDERS.TRAILER
\***
\***      display message "PDT support - DIRECT Order Trailer Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*"
\***      and RETURN
\***
\***      write item details to audit file
\***
\***      if there are any items in the LDTBF then write the order trailer
\***      to the LDTBF
\***
\***      if there any entries in order line table then
\***         GOSUB WRITE.DRSMQ.TYPE.17
\***         clear order line table
\***         set line total to zero
\***      endif
\***
\***      if any DRSMQ type 17 records were written then
\***         GOSUB WRITE.DRSMQ.TYPE.19
\***      endif
\***
\***      set lines booked in, singles added and lines added to 0
\***
\***
\***      set the respond option to a LDT trailer to true
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.ORDERS.TRAILER:

       SB.MESSAGE$ = "PDT Support - DIRECT Order Trailer Received"
       GOSUB SB.BG.MESSAGE

       IF FN.VALIDATE.DATA(DATA.IN$, 25) = 0 THEN BEGIN
          RECEIVE.STATE$ = "*"
          RETURN
       ENDIF

!       IF STORE.NUMBER$ = "2999" THEN BEGIN                                    !1.3BG !1.5CS
!                                                                                      !1.5CS
!          IF STKMQ.OPEN.FLAG$="Y" THEN BEGIN                                   !1.3BG !1.5CS
!             CLOSE STKMQ.SESS.NUM%                                             !1.3BG !1.5CS
!             STKMQ.OPEN.FLAG$="N"                                              !1.3BG !1.5CS
!          ENDIF                                                                !1.3BG !1.5CS
!          IF IRF.OPEN.FLAG$="Y" THEN BEGIN                                     !1.3BG !1.5CS
!             CLOSE IRF.SESS.NUM%                                               !1.3BG !1.5CS
!             IRF.OPEN.FLAG$="N"                                                !1.3BG !1.5CS
!          ENDIF                                                                !1.3BG !1.5CS
!          IF IMSTC.OPEN.FLAG$="Y" THEN BEGIN                                   !1.3BG !1.5CS
!             CLOSE IMSTC.SESS.NUM%                                             !1.3BG !1.5CS
!             IMSTC.OPEN.FLAG$="N"                                              !1.3BG !1.5CS
!          ENDIF                                                                !1.3BG !1.5CS
!          IF STOCK.OPEN.FLAG$="Y" THEN BEGIN                                   !1.3BG !1.5CS
!             CLOSE STOCK.SESS.NUM%                                             !1.3BG !1.5CS
!             STOCK.OPEN.FLAG$="N"                                              !1.3BG !1.5CS
!          ENDIF                                                                !1.3BG !1.5CS
!          IF LSSST.OPEN.FLAG$="Y" THEN BEGIN                                   !1.3BG !1.5CS
!             CLOSE LSSST.SESS.NUM%                                             !1.3BG !1.5CS
!             LSSST.OPEN.FLAG$="N"                                              !1.3BG !1.5CS
!          ENDIF                                                                !1.3BG !1.5CS
!                                                                                      !1.5CS
!          ADX.RET.CODE% = ADXSTART("ADX_UPGM:LSSST.286", \                     !1.3BG !1.5CS
!                          MONITORED.PORT$,             \                       !1.3BG !1.5CS
!                          "LSS Stock count report")                            !1.3BG !1.5CS
!                                                                                      !1.5CS
!       ENDIF ELSE BEGIN                                                        !1.3BG !1.5CS

          CSR.AUDIT.DATA$ = "SUPPLIER " + SAVED.SUPPLIER.NO$ + " ORDER " + \
                           SAVED.ORDER.NO$ + SAVED.ORDER.SUFFIX$ +         \
                           " had " + STR$(LINES.BOOKED.IN%) +              \
                           " items booked in, between times " +            \
                           BOOKING.STARTED$ + " - " + BOOKING.ENDED$


          GOSUB LOG.TO.AUDIT.FILE

          IF LDTBF.HEADER.WRITTEN$ = "Y" THEN BEGIN                        ! CLC
             LDTBF.RECORD.ID$ = "OT"                                       ! CLC
             RC% = WRITE.LDTBF                                             ! CLC
             IF RC% = 1 THEN GOTO WRITE.ERROR                              ! CLC
             LDTBF.HEADER.WRITTEN$ = "N"                                   ! CLC
             LDTBF.NUM.OF.ITEMS% = 0                                       ! CLC
          ENDIF                                                            ! CLC

          IF LINE.TOTAL% <> 0 THEN BEGIN
             GOSUB WRITE.DRSMQ.TYPE.17
             DIM ORDER.LINE.TABLE$(16)
             LINE.TOTAL% = 0
          ENDIF

          IF TYPE.17.WRITTEN THEN BEGIN
             GOSUB WRITE.DRSMQ.TYPE.19
          ENDIF

          LINES.BOOKED.IN% = 0
          LINES.ADDED% = 0
          SINGLES.ADDED% = 0

!       ENDIF                                                                   !1.3BG !1.5CS

       LDT.TRANSMIT = TRUE

   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.FILE.TRAILER
\***
\***      display message "PDT support - DIRECT File Trailer Received" on
\***      background screen
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*"
\***      and RETURN
\***
\***      if received terminal number or application number is invalid then
\***      set RECEIVE.STATE$ to "*" and RETURN
\***
\***      if DRSMQ file open then close
\***
\***      if LDTBF is open, close it and start PSS79 on the background
\***
\***      if no response is expected from the LDT then return
\***
\***      if number of orders expected <> number received then
\***         log an error
\***      endif
\***
\***      GOSUB TRANSMIT.DIRECT.FILE.TRAILER
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.FILE.TRAILER:

       SB.MESSAGE$ = "PDT Support - DIRECT File Trailer Received"
       GOSUB SB.BG.MESSAGE

       IF FN.VALIDATE.DATA(DATA.IN$, 26) = 0 THEN BEGIN
          RECEIVE.STATE$ = "*"
          RETURN
       ENDIF

       IF LDTBF.OPEN.FLAG$ = "Y" THEN BEGIN                             ! CLC
          CLOSE LDTBF.SESS.NUM%                                         ! CLC
          LDTBF.OPEN.FLAG$ = "N"                                        ! CLC
          IF LDTBF.WRITTEN.FLAG$ = "Y" THEN BEGIN                       ! CLC
             ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS79.286",             \1.2SH
                                      MONITORED.PORT$,                  \ CLC
                                      "PSS79 - LDT Reporting program")  ! CLC
             LDTBF.WRITTEN.FLAG$ = "N"                                  ! CLC
          ENDIF                                                         ! CLC
       ENDIF                                                            ! CLC

       IF MID$(DATA.IN$,3,2) <> APPLICATION.NO$                         \
          OR MID$(DATA.IN$,5,6) <> CURR.TERMINAL$ THEN BEGIN
          RECEIVE.STATE$ = "*"
          RETURN
       ENDIF

       IF DRSMQ.OPEN.FLAG$ =  "Y" THEN BEGIN
          CLOSE DRSMQ.SESS.NUM%
          DRSMQ.OPEN.FLAG$ = "N"
       ENDIF

       IF DIRORD.OPEN.FLAG$ = "Y" THEN BEGIN                           !1.2SH
          CLOSE DIRORD.SESS.NUM%                                       !1.2SH
          DIRORD.OPEN.FLAG$ = "N"                                      !1.2SH
       ENDIF                                                           !1.2SH

       IF NOT LDT.TRANSMIT THEN BEGIN
          RETURN
       ENDIF

       IF ORDERS.EXPECTED% <> ORDERS.FOUND% THEN BEGIN
          SB.EVENT.NO% = 91
          SB.UNIQUE$ = PACK$(SAVED.SUPPLIER.NO$) +                          \
                       PACK$(SAVED.ORDER.NO$) +                             \
                       SAVED.ORDER.SUFFIX$ +                                \
                       PACK$(FN.Z.PACK(STR$(ORDERS.EXPECTED%), 4)) +        \
                       PACK$(FN.Z.PACK(STR$(ORDERS.FOUND%), 4))
          GOSUB SB.LOG.AN.EVENT
       ENDIF

       GOSUB TRANSMIT.DIRECT.FILE.TRAILER

   RETURN


\******************************************************************************
\***
\***   RECEIVED.DIRECT.EOT:                                     STATE : Z
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if DRSMQ is open then close
\***
\***      open DRSMQ
\***
\***      if data exists on DRSMQ then
\***         GOSUB WRITE.STKMQ.FROM DRSMQ
\***         GOSUB CREATE.DIRECT.TEMP.DRSMQ to clear down file
\***      endif
\***
\***      close the DIR file
\***
\***      write information to audit file
\***
\***      set RECEIVE.STATE$ to "?" and RE.CHAIN to TRUE
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.DIRECT.EOT:

      SB.MESSAGE$ = "PDT Support - DIRECT E.O.T. received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 27) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      CURR.SESS.NUM% = DRSMQ.SESS.NUM%

      IF DRSMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DRSMQ.SESS.NUM%
         DRSMQ.OPEN.FLAG$ = "N"
      ENDIF

      OPEN DRSMQ.FILE.NAME$ AS DRSMQ.SESS.NUM% NOWRITE NODEL
      DRSMQ.OPEN.FLAG$ = "Y"

      LDTAF.LINK.TYPE% = 4                                              ! FMJK
      LDTAF.DATA.VOLUME.1% = 0                                          ! KMJK
      LDTAF.DATA.VOLUME.2% = 0                                          ! KMJK
      LDTAF.DATA.VOLUME.1% = SIZE (DRSMQ.FILE.NAME$)                    ! KMJK
      IF NOT LDT.TRANSMIT THEN LDTAF.DATA.VOLUME.2% =                   \ GMJK
         SIZE (DIRWF.FILE.NAME$)                                        ! GMJK

      IF SIZE(DRSMQ.FILE.NAME$) <> 0 THEN BEGIN
         GOSUB WRITE.STKMQ.FROM.DRSMQ
         GOSUB CREATE.DIRECT.TEMP.STKMQ
      ENDIF

      IF DIREC.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DIREC.SESS.NUM%
         DIREC.OPEN.FLAG$ = "N"
      ENDIF

      IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                  \ FMJK
        (LDTAF.LINK.TYPE% OR 80H)                                       ! FMJK
      LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                           ! FMJK
      LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                           ! FMJK
      TEMP.TIME$ = TIME$                                                ! GMJK
      LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +             \ GMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +              \ GMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ GMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ GMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+ \ GMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )    ! GMJK
      GOSUB LOG.TO.LDTAF.FILE                                           ! FMJK

      CSR.AUDIT.DATA$ = "Direct session complete at " +                 \
                         LEFT$(TIME$,2) + ":" +                         \
                         MID$(TIME$,3,2) + ":" +                        \
                         RIGHT$(TIME$,2)                                !
      CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +             \
          CSR.AUDIT.DATA$
      GOSUB LOG.TO.AUDIT.FILE

      IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE CSR.AUDIT.SESS.NUM%
         CSR.AUDIT.OPEN.FLAG$ = "N"
      ENDIF

      RE.CHAIN = TRUE                                                   ! ILC
      RECEIVE.STATE$ = "?"                                              ! ILC

   RETURN

\******************************************************************************
\***
\***   WRITE.STKMQ.FROM.DRSMQ
\***
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.STKMQ.FROM.DRSMQ:

      END.OF.DRSMQ = FALSE

      IF STKMQ.OPEN.FLAG$ = "N" THEN BEGIN
         OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512 READONLY \ DLC
              NODEL APPEND
         STKMQ.OPEN.FLAG$ = "Y"
      ENDIF

      WHILE NOT END.OF.DRSMQ

            CURR.SESS.NUM% = DRSMQ.SESS.NUM%
            RC% = READ.DRSMQ                                            ! BLC
            IF RC% = 1 THEN GOTO END.OF.DRSMQ.FILE                      ! BLC

            STKMQ.RECORD$ = DRSMQ.RECORD$ + DRSMQ.ENDREC.MARKER$

            CURR.SESS.NUM% = STKMQ.SESS.NUM%
            RC% = WRITE.STKMQ                                           ! BLC
            IF RC% = 1 THEN GOTO WRITE.ERROR                            ! BLC

      END.OF.DRSMQ.RETURN:

      WEND

      IF STKMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE STKMQ.SESS.NUM%
         STKMQ.OPEN.FLAG$ = "N"
      ENDIF

      IF DRSMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DRSMQ.SESS.NUM%
         DRSMQ.OPEN.FLAG$ = "N"
      ENDIF

   RETURN


\******************************************************************************
\***
\***   END.OF.DRSMQ.FILE:
\***
\***
\***   RETURN
\***
\******************************************************************************

   END.OF.DRSMQ.FILE:                                                   ! HDS

       END.OF.DRSMQ = TRUE                                              ! HDS
       GOTO END.OF.DRSMQ.RETURN                                         ! HDS

   RETURN                                                               ! HDS




\******************************************************************************
\***
\***   CREATE.DIRECT.TEMP.STKMQ:
\***
\***      if a DRSMQ already exists then close it
\***      create a new DRSMQ
\***
\***   RETURN
\***
\******************************************************************************

   CREATE.DIRECT.TEMP.STKMQ:


      CURR.SESS.NUM% = DRSMQ.SESS.NUM%

      IF DRSMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DRSMQ.SESS.NUM%
         DRSMQ.OPEN.FLAG$ = "N"
      ENDIF

      IF END #DRSMQ.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE DRSMQ.FILE.NAME$ AS DRSMQ.SESS.NUM%                 \
             BUFFSIZE 10240 LOCKED
      DRSMQ.OPEN.FLAG$ = "Y"

   RETURN


\******************************************************************************
\***
\***   TRANSMIT.DIRECT.FILE.TRAILER:
\***
\***      send a file trailer to the PSS38
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.DIRECT.FILE.TRAILER:

      PIPE.OUT$ = "L" +                                                 \
                  "FT" +                                                \
                  FN.Z.PACK(APPLICATION.NO$,2) +                        \
                  FN.Z.PACK(CURR.TERMINAL$,6) +                         \
                  FN.Z.PACK(STR$(ORDERS.FOUND%),2)


      GOSUB SEND.TO.PSS38

   RETURN


\******************************************************************************
\***
\***   TRANSMIT.DIRECT.FILE:
\***
\***      set PIPE.OUT$ to "D" to activate DIRWF transmission
\***      GOSUB SEND.TO.PSS38
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.DIRECT.FILE:

      PIPE.OUT$ = "D"
      GOSUB SEND.TO.PSS38

   RETURN:


\******************************************************************************
\***
\***   TIDY.UP.FOR.DIRECT:
\***
\***      close all open files required for DIRECT processing
\***
\***      release table space
\***
\***   RETURN
\***
\******************************************************************************

   TIDY.UP.FOR.DIRECT:

      IF IDF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE IDF.SESS.NUM%
         IDF.OPEN.FLAG$ = "N"
      ENDIF

      IF IEF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE IEF.SESS.NUM%
         IEF.OPEN.FLAG$ = "N"
      ENDIF

      IF STKMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE STKMQ.SESS.NUM%
         STKMQ.OPEN.FLAG$ = "N"
      ENDIF

      IF BCSMF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE BCSMF.SESS.NUM%
         BCSMF.OPEN.FLAG$ = "N"
      ENDIF

      IF DIRSUP.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DIRSUP.SESS.NUM%
         DIRSUP.OPEN.FLAG$ = "N"
      ENDIF

      IF DIRORD.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DIRORD.SESS.NUM%
         DIRORD.OPEN.FLAG$ = "N"
      ENDIF

      IF DIRWF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DIRWF.SESS.NUM%
         DIRWF.OPEN.FLAG$ = "N"
      ENDIF

      IF DRSMQ.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE DRSMQ.SESS.NUM%
         DRSMQ.OPEN.FLAG$ = "N"
      ENDIF

      IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE CSR.AUDIT.SESS.NUM%
         CSR.AUDIT.OPEN.FLAG$ = "N"
      ENDIF

      DIM ORDER.TABLE$(0)
      DIM DIRWF.DETAIL.TABLE$(0)
      DIM BAR.CODE.TABLE$(0)
      DIM ITEM.POSITION.TABLE$(0)
      DIM HEADER.TABLE$(0)
      DIM BC.LETTER.TABLE$(0)


   RETURN

\*****************************************************************************
\*****************************************************************************
\***
\***        U O D   S U B R O U T I N E S   B E L O W
\***
\*****************************************************************************
\*****************************************************************************

\*****************************************************************************
\***
\***   RECEIVED.UOD.FILE.HEADER:                    Data State: a (lower case)
\***
\***   Log the count of the number of UOD's to follow to the CSRAF.
\***   Store this number so that the File Trailer number can be compared.
\***   Create the UODTF
\***
\*****************************************************************************

RECEIVED.UOD.FILE.HEADER:                                               ! ELC

   SB.MESSAGE$ = "PDT Support - UOD File Header Received"               ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,28) = 0 THEN BEGIN                      ! ELC
        RECEIVE.STATE$ = "*"                                            ! ELC
        RETURN                                                          ! ELC
   ENDIF                                                                ! ELC

   DETAIL.RECORDS.RECEIVED$ = "N"                                       ! ELC
   ACTUAL.NUM.OF.UODS% = 0                                              ! ELC
   EXPECTED.NUM.OF.UODS% = VAL(MID$(DATA.IN$,11,4))                     ! ELC
   CSR.AUDIT.DATA$ = "UOD File Header. " +                              \ ELC
                     STR$(EXPECTED.NUM.OF.UODS%) + " UOD's."            ! ELC
   CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +                \ ELC
                     CSR.AUDIT.DATA$                                    ! ELC
   GOSUB LOG.TO.AUDIT.FILE                                              ! ELC

   GOSUB CREATE.UODTF                                                   ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   RECEIVED.UOD.HEADER:                         Data State: b (lower case)
\***
\***   If the UOD is scanned at item level it will have a status of "P", "O"
\***   or "I", with a count of the number of scanned items.
\***
\***   If the UOD is scanned at UOD level the status will be " ".
\***
\***   The UOD number, status flag and number of items (where appl.) are
\***   written to the CSRAF.
\***
\***   If status flag is blank then write a UOD type 21 record to the UODTF.
\***
\***   Otherwise, store the UOD file header details.
\***
\*****************************************************************************

RECEIVED.UOD.HEADER:

   UOD.NUM.ITEMS$ = ""                                                  ! ELC

   SB.MESSAGE$ = "PDT Support - UOD Header Received"                    ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,29) = 0 THEN BEGIN                      ! ELC
      IF MID$(DATA.IN$,3,14) <> STRING$(14,"?") THEN RECEIVE.STATE$ = "*"  !GMJK
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   UOD.STATUS.FLAG$ = MID$(DATA.IN$,17,1)                               ! ELC
   UOD.NUMBER$ = MID$(DATA.IN$,3,14)                                    ! ELC

   IF UOD.STATUS.FLAG$ <> "B" THEN BEGIN                                ! ELC
      IF UOD.STATUS.FLAG$ = "U" THEN UOD.STATUS.FLAG$ = "I"             \ FMJK
        ELSE IF UOD.STATUS.FLAG$ <> "P" THEN UOD.STATUS.FLAG$ = "O"     ! FMJK
      UOD.NUM.ITEMS% = VAL(MID$(DATA.IN$,18,4))                         ! ELC
      UOD.NUM.ITEMS$ = STR$(UOD.NUM.ITEMS%)                             ! ELC
      COUNTED.ITEMS% = 0                                                ! ELC
      IF LEFT$(UOD.NUMBER$,4)="9999" THEN BEGIN                         ! GMJK
        CSR.AUDIT.DATA$ = "UOD Number: " + UOD.NUMBER$ +                \ GMJK
                          " not written to STKMQ"                       ! GMJK
      ENDIF ELSE BEGIN                                                  ! GMJK
        CSR.AUDIT.DATA$ = "UOD Number: " + UOD.NUMBER$ +                \ GMJK
                          " Status: " + UOD.STATUS.FLAG$ +              \ GMJK
                          " Number of items: " + UOD.NUM.ITEMS$         ! GMJK
      ENDIF                                                             ! GMJK
      STORED.UOD.HEADER$ = LEFT$(DATA.IN$,16) + UOD.STATUS.FLAG$ +      \ GMJK
                           RIGHT$(DATA.IN$,LEN(DATA.IN$)-17)            ! GMJK
   ENDIF ELSE                                                           \ GMJK
     IF (NOT (RIGHT$(UOD.NUMBER$,6)="000000" AND                        \ GMJK
              LEFT$(UOD.NUMBER$,4)<>"0000")) AND                        \ GMJK
         LEFT$(UOD.NUMBER$,4)<>"9999" THEN BEGIN                        ! GMJK
       CSR.AUDIT.DATA$ = "UOD Number: " + UOD.NUMBER$                   ! ELC
       UODTF.RECORD$ = CHR$(34) + PACK$("21") + CHR$(59) +              \ ELC
                       PACK$(DATE$) + PACK$(TIME$) +                    \ ELC
                       PACK$(UOD.NUMBER$) +                             \ ELC
                       UOD.STATUS.FLAG$ +                               \ ELC
                       CHR$(34) + CHR$(0Dh) + CHR$(0Ah)                 ! ELC
       RC% = WRITE.UODTF                                                ! ELC
       IF RC% = 1 THEN GOTO WRITE.ERROR                                 ! ELC
     ENDIF ELSE BEGIN                                                   ! GMJK
       CSR.AUDIT.DATA$ = "UOD Number: " + UOD.NUMBER$ +                 \ GMJK
                         " not written to STKMQ"                        ! GMJK
     ENDIF                                                              ! GMJK
   GOSUB LOG.TO.AUDIT.FILE                                              ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   RECEIVED.UOD.DETAIL:                         Data State: c (lower case)
\***
\***   Write UOD type 23 record to UODTF.
\***
\***   Increment the number of detail records.
\***
\*****************************************************************************

RECEIVED.UOD.DETAIL:

   DETAIL.RECORDS.RECEIVED$ = "Y"                                       ! ELC

   SB.MESSAGE$ = "PDT Support - UOD Detail Record Received"             ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,30) = 0 THEN BEGIN                      ! ELC
      RECEIVE.STATE$ = "*"                                              ! ELC
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   IF LEFT$(UOD.NUMBER$,4)="9999" THEN BEGIN                            ! GMJK
     COUNTED.ITEMS% = COUNTED.ITEMS% + 1                                ! GMJK
     RETURN                                                             ! GMJK
   ENDIF                                                                ! GMJK

   IF MID$(DATA.IN$,3,1) = "I" THEN BEGIN                               ! ELC
      BAR.ITEM.CODE$ = MID$(DATA.IN$,5,12)                              ! ELC
      BAR.ITEM.CODE.FLAG$ = "B"                                         ! ELC
   ENDIF ELSE BEGIN                                                     ! ELC
      BAR.ITEM.CODE$ = MID$(DATA.IN$,5,12)                              ! ELC
      BAR.ITEM.CODE.FLAG$ = "E"                                         ! ELC
   ENDIF                                                                ! ELC

! lines deleted here for the alteration of the UOD.STATUS.FLAG$           FMJK

   UODTF.RECORD$ = CHR$(34) + PACK$("23") + CHR$(59) +                  \ ELC
                   PACK$(DATE$) + PACK$(TIME$) +                        \ ELC
                   PACK$(UOD.NUMBER$) +                                 \ ELC
                   UOD.STATUS.FLAG$ +                                   \ ELC
                   PACK$(BAR.ITEM.CODE$) +                              \ ELC
                   BAR.ITEM.CODE.FLAG$ +                                \ ELC
                   STR$(VAL(MID$(DATA.IN$,17,4))) +                     \ ELC
                   ";" + "0" + PACK$("0000") + "0" + PACK$("0000") +    \ ELC
                   " " + CHR$(34) +                                     \ ELC
                   CHR$(0Dh) + CHR$(0Ah)                                ! ELC
   RC% = WRITE.UODTF                                                    ! ELC
   IF RC% = 1 THEN GOTO WRITE.ERROR                                     ! ELC

   COUNTED.ITEMS% = COUNTED.ITEMS% + 1                                  ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   RECEIVED.UOD.TRAILER:                        Data State: d (lower case)
\***
\*****************************************************************************

RECEIVED.UOD.TRAILER:

   SB.MESSAGE$ = "PDT Support - UOD Trailer Received"                   ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,31) = 0 THEN BEGIN                      ! ELC
      RECEIVE.STATE$ = "*"                                              ! ELC
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   IF DETAIL.RECORDS.RECEIVED$ = "Y" THEN BEGIN                         ! ELC
      IF COUNTED.ITEMS% <> UOD.NUM.ITEMS% THEN BEGIN                    ! ELC
         SB.EVENT.NO% = 92                                              ! ELC
         SB.UNIQUE$ = PACK$(STR$(UOD.REPORT.NUM%)) +                    \ ELC
                      PACK$(RIGHT$("00000000"+STR$(UOD.NUM.ITEMS%),8)) +\ ELC
                      PACK$(RIGHT$("00000000"+STR$(COUNTED.ITEMS%),8))  ! ELC
         GOSUB SB.LOG.AN.EVENT                                          ! ELC
         RECEIVE.STATE$ = "*"                                           ! ELC
         RETURN                                                         ! ELC
      ENDIF                                                             ! ELC
      IF LEFT$(UOD.NUMBER$,4)<>"9999" THEN BEGIN                        ! GMJK
        UOD.STATUS.FLAG$ = MID$(STORED.UOD.HEADER$,17,1)                ! ELC
 ! Three lines deleted here, that changes <>P to O                      ! GMJK
        UODTF.RECORD$ = CHR$(34) + PACK$("21") + CHR$(59)       +       \ ELC
                        PACK$(DATE$) + PACK$(TIME$) +                   \ ELC
                        PACK$(MID$(STORED.UOD.HEADER$,3,14)) +          \ ELC
                        UOD.STATUS.FLAG$ +                              \ ELC
                        CHR$(34) + CHR$(0Dh) + CHR$(0Ah)                ! ELC
        RC% = WRITE.UODTF                                               ! ELC
        IF RC% = 1 THEN GOTO WRITE.ERROR                                ! ELC
       ENDIF                                                            ! GMJK
      DETAIL.RECORDS.RECEIVED$ = "N"                                    ! ELC
   ENDIF                                                                ! ELC

   ACTUAL.NUM.OF.UODS% = ACTUAL.NUM.OF.UODS% + 1                        ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   RECEIVED.UOD.FILE.TRAILER:                   Data State: e (lower case)
\***
\***   If actual number of UOD's <> expected number of UOD's
\***   (from File Header), then log an error, set DATA.STATE$ to "*" and
\***   RETURN
\***
\***   Append UODTF records to the STKMQ and UODBF.
\***
\***   Send a File Trailer to the LDT.
\***
\***   Start PSS45 as a background application.
\***
\***
\*****************************************************************************

RECEIVED.UOD.FILE.TRAILER:

   SB.MESSAGE$ = "PDT Support - UOD File Trailer Received"              ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,32) = 0 THEN BEGIN                      ! ELC
      RECEIVE.STATE$ = "*"                                              ! ELC
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   IF ACTUAL.NUM.OF.UODS% <> EXPECTED.NUM.OF.UODS% THEN BEGIN           ! ELC
      SB.EVENT.NO% = 92                                                 ! ELC
      SB.UNIQUE$ = PACK$(STR$(UOD.REPORT.NUM%)) +                       \ ELC
         PACK$(RIGHT$("00000000" + STR$(EXPECTED.NUM.OF.UODS%),8)) +    \ ELC
         PACK$(RIGHT$("00000000" + STR$(ACTUAL.NUM.OF.UODS%),8))        ! ELC
      GOSUB SB.LOG.AN.EVENT                                             ! ELC
      RECEIVE.STATE$ = "*"                                              ! ELC
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   PIPE.OUT$ = "L" +                                                    \ ELC
               "UR" +                                                   \ ELC
               FN.Z.PACK(APPLICATION.NO$,2) +                           \ ELC
               FN.Z.PACK(CURR.TERMINAL$,6) +                            \ ELC
               FN.Z.PACK(RIGHT$("0000" + STR$(ACTUAL.NUM.OF.UODS%),4),4)! ELC
   GOSUB SEND.TO.PSS38                                                  ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   END.OF.UODTF.FILE:
\***
\*****************************************************************************

END.OF.UODTF.FILE:

   END.OF.UODTF = TRUE
   GOTO END.OF.UODTF.RETURN

RETURN

\*****************************************************************************
\***
\***   RECEIVED.UOD.EOT:                            Data State: f (lower case)
\***
\***   Log message to CSRAF
\***
\***   Set RE.CHAIN to true.
\*****************************************************************************

RECEIVED.UOD.EOT:

   SB.MESSAGE$ = "PDT Support - UOD EOT Received"                       ! ELC
   GOSUB SB.BG.MESSAGE                                                  ! ELC

   IF FN.VALIDATE.DATA(DATA.IN$,33) = 0 THEN BEGIN                      ! ELC
      RECEIVE.STATE$ = "*"                                              ! ELC
      RETURN                                                            ! ELC
   ENDIF                                                                ! ELC

   ! Line deleted here that called HOLD.PDT routine                       GMJK

   IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                                ! ELC
      IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                           ! ELC
      CURR.SESS.NUM% = STKMQ.SESS.NUM%                                  ! ELC
      OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512 READONLY    \ ELC
         NODEL APPEND                                                   ! ELC
      STKMQ.OPEN.FLAG$ = "Y"                                            ! ELC
   ENDIF                                                                ! ELC

   IF UODBF.OPEN.FLAG$ <> "Y" THEN BEGIN                                ! ELC
      OPENING.UODBF$ = "Y"                                              ! ELC
      IF END# UODBF.SESS.NUM% THEN CREATE.UODBF                         ! ESMG
      CURR.SESS.NUM% = UODBF.SESS.NUM%                                  ! ELC
      RETURN.FROM.CREATE.UODBF:                                         ! ELC
      OPEN UODBF.FILE.NAME$ AS UODBF.SESS.NUM% BUFFSIZE 512 READONLY    \ ELC
         NODEL APPEND                                                   ! ELC
      UODBF.OPEN.FLAG$ = "Y"                                            ! ELC
   ENDIF                                                                ! ELC

   END.OF.UODTF = FALSE                                                 ! ELC

   WHILE NOT END.OF.UODTF                                               ! ELC

      CURR.SESS.NUM% = UODTF.SESS.NUM%                                  ! ELC
      RC% = READ.UODTF                                                  ! ELC
      IF RC% = 1 THEN GOTO END.OF.UODTF.FILE                            ! ELC

      STKMQ.RECORD$ = UODTF.RECORD$ + CHR$(0Dh) + CHR$(0Ah)             ! ELC

      CURR.SESS.NUM% = STKMQ.SESS.NUM%                                  ! ELC
      RC% = WRITE.STKMQ                                                 ! ELC
      IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! ELC

      UODBF.RECORD$ = STKMQ.RECORD$                                     ! ELC

      CURR.SESS.NUM% = UODBF.SESS.NUM%                                  ! ELC
      RC% = WRITE.UODBF                                                 ! ELC
      IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! ELC

      END.OF.UODTF.RETURN:                                              ! ELC

   WEND                                                                 ! ELC

   CLOSE STKMQ.SESS.NUM%                                                ! ELC
   STKMQ.OPEN.FLAG$ = "N"                                               ! ELC

   CLOSE UODBF.SESS.NUM%                                                ! ELC
   UODBF.OPEN.FLAG$ = "N"                                               ! ELC

   ! Line deleted here that called RELEASE.PDT routine                    GMJK

   ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS45.286",                       \ ELC
                             MONITORED.PORT$,                           \ ELC
                             "PSS45 - Outstanding UOD Report")          ! ELC

   LDTAF.LINK.TYPE% = 5                                                 ! FMJK
   IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                     \ FMJK
     (LDTAF.LINK.TYPE% OR 80H)                                          ! FMJK
   LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                              ! FMJK
   LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                              ! FMJK
   TEMP.TIME$ = TIME$                                                   ! GMJK
   LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +                \ GMJK
                       (VAL(MID$(TEMP.TIME$,3,2))*60) +                 \ GMJK
                        VAL(RIGHT$(TEMP.TIME$,2)) ) -                   \ GMJK
                     ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+ \ GMJK
                       (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+  \ GMJK
                        VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )     ! GMJK
   LDTAF.DATA.VOLUME.2% = 0                                             ! FMJK
   LDTAF.DATA.VOLUME.1% = SIZE (UODTF.FILE.NAME$)                       ! GMJK
   GOSUB LOG.TO.LDTAF.FILE                                              ! FMJK

   CSR.AUDIT.DATA$ = "UOD session complete at " +                       \ FMJK
                         LEFT$(TEMP.TIME$,2) + ":" +                    \ FMJK
                         MID$(TEMP.TIME$,3,2) + ":" +                   \ FMJK
                         RIGHT$(TEMP.TIME$,2)                           ! FMJK
   CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +                \ ELC
       CSR.AUDIT.DATA$                                                  ! ELC
   GOSUB LOG.TO.AUDIT.FILE                                              ! ELC

   IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN                             ! ELC
      CLOSE CSR.AUDIT.SESS.NUM%                                         ! ELC
      CSR.AUDIT.OPEN.FLAG$ = "N"                                        ! ELC
   ENDIF                                                                ! ELC

   RE.CHAIN = TRUE                                                      ! ELC
   RECEIVE.STATE$ = "?"                                                 ! ELC

RETURN                                                                  ! ELC


\*****************************************************************************
\***
\***   DUMMY.ROUTINE:
\***
\***   Will not be called
\***
\*****************************************************************************

DUMMY.ROUTINE:                                                          ! ELC

RETURN                                                                  ! ELC


\******************************************************************************
\***
\***   CREATE.UODTF:
\***
\***      if a UODTF already exists then close it
\***      create a new UODTF
\***
\***   RETURN
\***
\******************************************************************************

   CREATE.UODTF:                                                        ! ELC

      CURR.SESS.NUM% = UODTF.SESS.NUM%                                  ! ELC

      IF UODTF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! ELC
         CLOSE UODTF.SESS.NUM%                                          ! ELC
         UODTF.OPEN.FLAG$ = "N"                                         ! ELC
      ENDIF                                                             ! ELC

      IF END #UODTF.SESS.NUM% THEN CREATE.ERROR                         ! ELC
      CREATE UODTF.FILE.NAME$ AS UODTF.SESS.NUM%                        \ ELC
             BUFFSIZE 10240 LOCKED                                      ! ELC
      UODTF.OPEN.FLAG$ = "Y"                                            ! ELC

   RETURN                                                               ! ELC

\******************************************************************************
\***
\***   CREATE.UODBF:
\***
\******************************************************************************

   CREATE.UODBF:                                                        ! ESMG

         IF END #UODBF.SESS.NUM% THEN CREATE.ERROR                      ! ESMG
         CREATE UODBF.FILE.NAME$ AS UODBF.SESS.NUM% BUFFSIZE 512        \ ESMG
            READONLY                                                    ! ESMG
         CLOSE UODBF.SESS.NUM%                                          ! ESMG
         OPENING.UODBF$ = "N"                                           ! ESMG
         GOTO RETURN.FROM.CREATE.UODBF                                  ! ESMG

   RETURN                                                               ! ESMG

\******************************************************************************
\***
\***   SEND.TO.PSS38:
\***
\***      transmit data to PSS38 (data contained in PIPE.OUT$)
\***
\***   RETURN
\***
\******************************************************************************

   SEND.TO.PSS38:

      IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR
      CURR.SESS.NUM% = PIPEI.SESS.NUM%
      WRITE# PIPEI.SESS.NUM%; PIPE.OUT$

   RETURN


\******************************************************************************
\***
\***   HOLD.PDT:
\***
\***      send a hold PDT command to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   HOLD.PDT:

      PIPE.OUT$ = "HY"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "Y"

   RETURN


\******************************************************************************
\***
\***   RELEASE.PDT:
\***
\***      send a release PDT command to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   RELEASE.PDT:

      PIPE.OUT$ = "HN"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "N"

   RETURN


\******************************************************************************
\***
\***   WRITE.DIRWF.RECORD:
\***
\***     write DIRWF record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DIRWF.RECORD:

      CURR.SESS.NUM% = DIRWF.SESS.NUM%
      RC% = WRITE.DIRWF                                                 ! BLC
      IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! BLC

   RETURN


\******************************************************************************
\***
\***   WRITE.DRSMQ.TYPE.17
\***
\***      if a training order then
\***         return
\***      endif
\***
\***      format DRSMQ type 17 record
\***
\***      write DRSMQ record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DRSMQ.TYPE.17:

       IF VAL(SAVED.SUPPLIER.NO$) > 999000 THEN BEGIN
          RETURN
       ENDIF

       DRSMQ.RECORD.DELIMITER$ = CHR$(34)
       DRSMQ.FIELD.DELIMITER$ = CHR$(59)
       DRSMQ.ENDREC.MARKER$ = CHR$(0Dh) + CHR$(0Ah)

       DRSMQ.STRING$ = ""
       FOR COUNT% = 1 TO LINE.TOTAL%
           IF MID$(ORDER.LINE.TABLE$(COUNT%),3,1) = "I" THEN BEGIN
              DRSMQ.BOOTS.CODE$ = PACK$(MID$(ORDER.LINE.TABLE$(COUNT%),9,8))
              DRSMQ.BAR.CODE$ = PACK$("000000000000")
           ENDIF ELSE BEGIN
              DRSMQ.BOOTS.CODE$ = PACK$("00000000")
              DRSMQ.BAR.CODE$ = PACK$(MID$(ORDER.LINE.TABLE$(COUNT%),5,12))
           ENDIF
           DRSMQ.QTY.GOOD$ = STR$(VAL(MID$(ORDER.LINE.TABLE$(COUNT%),17,4)))
           DRSMQ.QTY.BAD$ = STR$(VAL(MID$(ORDER.LINE.TABLE$(COUNT%),21,4)))
           DRSMQ.QTY.STOLEN$ = STR$(VAL(MID$(ORDER.LINE.TABLE$(COUNT%),25,4)))

           DRSMQ.STRING$ = DRSMQ.STRING$ +                              \
                           DRSMQ.FIELD.DELIMITER$ +                     \
                           DRSMQ.BOOTS.CODE$ +                          \
                           DRSMQ.BAR.CODE$ +                            \
                           DRSMQ.QTY.GOOD$ +                            \
                           DRSMQ.FIELD.DELIMITER$ +                     \
                           DRSMQ.QTY.BAD$ +                             \
                           DRSMQ.FIELD.DELIMITER$ +                     \
                           DRSMQ.QTY.STOLEN$                            !
       NEXT COUNT%

       DRSMQ.TRANS.TYPE$ = PACK$("17")
       DRSMQ.DATE$ = PACK$(DATE$)
       DRSMQ.TIME$ = PACK$(TIME$)
       DRSMQ.SOURCE$ = "P"
       DRSMQ.SUPPLIER.NO$ = PACK$(SAVED.SUPPLIER.NO$)
       DRSMQ.ORDER.NO$ = PACK$(SAVED.ORDER.NO$)
       DRSMQ.ORDER.SFX$ = SAVED.ORDER.SUFFIX$
       DRSMQ.PAGE.NO$ = PACK$(LEFT$(ORDER.LINE.TABLE$(1),2))
\      DRSMQ.ITEM.COUNT$ = PACK$(FN.Z.PACK(STR$(LINE.TOTAL%),2))        ! 1.6NWB
       DRSMQ.ITEM.COUNT$ = PACK$(FN.Z.PACK(STR$(LINE.TOTAL%),8))
       DRSMQ.CARTON.NO$ = PACK$("00000000")                             ! 1.6NWB
       DRSMQ.CARTON.FLAG$ = " "                                         ! 1.6NWB

       DRSMQ.RECORD$ = DRSMQ.RECORD.DELIMITER$ +                        \
                       DRSMQ.TRANS.TYPE$ +                              \
                       DRSMQ.FIELD.DELIMITER$ +                         \
                       DRSMQ.DATE$ +                                    \
                       DRSMQ.TIME$ +                                    \
                       DRSMQ.SOURCE$ +                                  \
                       DRSMQ.SUPPLIER.NO$ +                             \
                       DRSMQ.ORDER.NO$ +                                \
                       DRSMQ.ORDER.SFX$ +                               \
                       DRSMQ.PAGE.NO$ +                                 \
                       DRSMQ.ITEM.COUNT$ +                              \
                       DRSMQ.CARTON.NO$ +                               \ 1.6NWB
                       DRSMQ.CARTON.FLAG$ +                             \ 1.6NWB
                       DRSMQ.STRING$ +                                  \
                       DRSMQ.RECORD.DELIMITER$ +                        \
                       DRSMQ.ENDREC.MARKER$

       CURR.SESS.NUM% = DRSMQ.SESS.NUM%
       CURRENT.KEY$ = ""
       RC% = WRITE.DRSMQ                                                ! BLC
       IF RC% = 1 THEN GOTO WRITE.ERROR                                 ! BLC

       TYPE.17.WRITTEN = TRUE

   RETURN


\******************************************************************************
\***
\***   ADD.TO.ORDER.LINE.TABLE
\***
\***       add 1 to line total
\***       add detail entry to order line table
\***
\***   RETURN
\***
\******************************************************************************

   ADD.TO.ORDER.LINE.TABLE:

       LINE.TOTAL% = LINE.TOTAL% + 1
       ORDER.LINE.TABLE$(LINE.TOTAL%) = MID$(DATA.IN$,3,28)

   RETURN


\******************************************************************************
\***
\***   WRITE.DRSMQ.TYPE.19
\***
\***      if a training order then
\***         return
\***      endif
\***
\***      format DRSMQ type 19 record
\***
\***      write DRSMQ record
\***
\***   RETURN
\***
\******************************************************************************

   WRITE.DRSMQ.TYPE.19:

       IF VAL(SAVED.SUPPLIER.NO$) > 999000 THEN BEGIN
          RETURN
       ENDIF

       DRSMQ.RECORD.DELIMITER$ = CHR$(34)
       DRSMQ.FIELD.DELIMITER$ = CHR$(59)
       DRSMQ.ENDREC.MARKER$ = CHR$(0Dh) + CHR$(0Ah)

       DRSMQ.TRANS.TYPE$ = PACK$("19")
       DRSMQ.DATE$ = PACK$(DATE$)
       DRSMQ.TIME$ = PACK$(TIME$)
       DRSMQ.SUPPLIER.NO$ = PACK$(SAVED.SUPPLIER.NO$)
       DRSMQ.ORDER.NO$ = PACK$(SAVED.ORDER.NO$)
       DRSMQ.ORDER.SFX$ = SAVED.ORDER.SUFFIX$
       DRSMQ.CONF.AMND.FLAG$ = "A"
       DRSMQ.START.TIME$ = PACK$(BOOKING.STARTED$)
       DRSMQ.END.TIME$ = PACK$(BOOKING.ENDED$)
       DRSMQ.SNGLS.ADDED$ = FN.Z.PACK(STR$(SINGLES.ADDED%),5)
       DRSMQ.ITEMS.ADDED$ = FN.Z.PACK(STR$(LINES.ADDED%),5)
       DRSMQ.ITEMS.BOOKED$ = FN.Z.PACK(STR$(LINES.BOOKED.IN%),5)

       DRSMQ.RECORD$ = DRSMQ.RECORD.DELIMITER$ +                        \
                       DRSMQ.TRANS.TYPE$ +                              \
                       DRSMQ.FIELD.DELIMITER$ +                         \
                       DRSMQ.DATE$ +                                    \
                       DRSMQ.TIME$ +                                    \
                       DRSMQ.SUPPLIER.NO$ +                             \
                       DRSMQ.ORDER.NO$ +                                \
                       DRSMQ.ORDER.SFX$ +                               \
                       DRSMQ.CONF.AMND.FLAG$ +                          \
                       DRSMQ.START.TIME$ +                              \
                       DRSMQ.END.TIME$ +                                \
                       DRSMQ.SNGLS.ADDED$ +                             \
                       DRSMQ.ITEMS.ADDED$ +                             \
                       DRSMQ.ITEMS.BOOKED$ +                            \
                       DRSMQ.RECORD.DELIMITER$ +                        \
                       DRSMQ.ENDREC.MARKER$

       CURR.SESS.NUM% = DRSMQ.SESS.NUM%
       CURRENT.KEY$ = ""
       RC% = WRITE.DRSMQ                                                ! BLC
       IF RC% = 1 THEN GOTO WRITE.ERROR                                 ! BLC

   RETURN


\******************************************************************************
\***
\***   LOG.TO.AUDIT.FILE
\***
\***   RETURN
\***
\******************************************************************************

   LOG.TO.AUDIT.FILE:

      IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN
         IF END #CSR.AUDIT.SESS.NUM% THEN AUDIT.FAIL
         OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND
         CSR.AUDIT.OPEN.FLAG$ = "Y"
      ENDIF

      PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$

   AUDIT.FAIL:

   RETURN

\******************************************************************************
\***
\***   LOG.TO.LDTAF.FILE
\***
\******************************************************************************

   LOG.TO.LDTAF.FILE:                                                   ! FMJK
                                                                        ! FMJK
        CURRENT.KEY$ = ""                                               ! FMJK
        CURR.SESS.NUM% = LDTAF.SESS.NUM%                                ! FMJK
        IF LDTAF.OPEN.FLAG$ = "N" THEN BEGIN                            ! FMJK
           IF END #LDTAF.SESS.NUM% THEN CREATE.LDTAF.FILE               ! FMJK
           OPEN LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM% APPEND              ! FMJK
           LDTAF.OPEN.FLAG$ = "Y"                                       ! FMJK
        ENDIF                                                           ! FMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                   \ FMJK
          IF WRITE.LDTAF THEN GOTO WRITE.ERROR                          ! FMJK
        LDTAF.LINK.TYPE% = 0                                            ! FMJK
        LDTAF.OPEN.FLAG$ = "N"                                          ! FMJK
        CLOSE LDTAF.SESS.NUM%                                           ! FMJK
                                                                        ! FMJK
   RETURN                                                               ! FMJK
                                                                        ! FMJK
   CREATE.LDTAF.FILE:                                                   ! FMJK
                                                                        ! FMJK
        IF END #LDTAF.SESS.NUM% THEN CREATE.ERROR                       ! FMJK
        CREATE POSFILE LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM%              \ FMJK
           MIRRORED ATCLOSE                                             ! FMJK
        LDTAF.OPEN.FLAG$ = "Y"                                          ! FMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                   \ FMJK
          IF WRITE.LDTAF THEN GOTO WRITE.ERROR                          ! FMJK
        LDTAF.LINK.TYPE% = 0                                            ! FMJK
        LDTAF.OPEN.FLAG$ = "N"                                          ! FMJK
        CLOSE LDTAF.SESS.NUM%                                           ! FMJK
                                                                        ! FMJK
   RETURN                                                               ! FMJK

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***   L O W   L E V E L   S U B R O U T I N E S                            ***
\***                                                                        ***
\***                                                                        ***
\***   - SB.FILE.UTILS                                                      ***
\***   - SB.BG.MESSAGE                                                      ***
\***   - SB.LOG.AN.EVENT                                                    ***
\***   - SB.FORMAT.ERROR.DATA                                               ***
\***   - SB.FILE.OPEN.ERROR                                                 ***
\***   - SB.FILE.READ.ERROR                                                 ***
\***   - SB.FILE.WRITE.ERROR                                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   Subroutine : SB.FILE.UTILS
\***
\***   Purpose    : Allocate / report / de-allocate a file session number
\***
\***   Parameters : 2 or 3 (depending on action)
\***
\***      SB.ACTION$  = "O" for allocate file session number
\***                    "R" for report file session number
\***                    "C" for de-allocate file session number
\***      SB.INTEGER% = file reporting number for action "O" or
\***                  = file session number for actions "R" or "C"
\***
\***      SB.STRING$  = logical file name for action "O" or
\***                    null ("") for actions "R" and "C"
\***
\***   Output     : 1 or 2 (depending on action)
\***      SB.FILE.SESS.NUM% = file session number for action "O" or
\***                          undefined for action "C"
\***      or
\***      SB.FILE.REP.NUM%  = file reporting number for action "R" or
\***                          undefined for action "C"
\***
\***   Error action : log event 48 and end program
\***
\******************************************************************************

   SB.FILE.UTILS:

      RC% = SESS.NUM.UTILITY(SB.ACTION$,                                \ BLC
                             SB.INTEGER%,                               \
                             SB.STRING$ )

      IF RC% <> 0 THEN BEGIN
         SB.EVENT.NO% = 48
         SB.UNIQUE$ = FN.Z.PACK(STR$(F20.INTEGER.FILE.NO%), 10)
         SB.MESSAGE$ = "SESSION NUMBER ALLOCATION ROUTINE FAILED"
         GOSUB SB.LOG.AN.EVENT
         GOTO PROGRAM.EXIT
      ENDIF

      IF SB.ACTION$ = "O" THEN                                          \
         SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
      IF SB.ACTION$ = "R" THEN                                          \
         SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.BG.MESSAGE
\***
\***   Purpose    : Display a message to the background screen
\***
\***   Parameters : 1
\***
\***      SB.MESSAGE$ = message to be displayed (message will be truncated to
\***                    46 characters if the message is longer than 46 chars)
\***                    Minus the port letter.
\***
\***   Output     : 1
\***      SB.MESSAGE$ = null
\***
\***   Error action : log an event 23 and end program
\***
\******************************************************************************

   SB.BG.MESSAGE:

      IF SB.MESSAGE$ = LAST.MESSAGE$ THEN RETURN
      LAST.MESSAGE$ = SB.MESSAGE$

      SB.MESSAGE$ = MONITORED.PORT$ + ": " + SB.MESSAGE$
      SB.MESSAGE$ = LEFT$(SB.MESSAGE$ + STRING$(46, " "), 46)
      CALL ADXSERVE( ADX.RET.CODE%, 26, 0, SB.MESSAGE$)

      IF ADX.RET.CODE% <> 0 THEN BEGIN
         SB.EVENT.NO% = 23
         SB.UNIQUE$ = FN.Z.PACK(STR$(ADX.RET.CODE%),5) + "04   "
         SB.MESSAGE$ = ""
         GOSUB SB.LOG.AN.EVENT
      ENDIF

      SB.MESSAGE$ = ""

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.LOG.AN.EVENT
\***
\***   Purpose    : General routine to log an event using passed data. If
\***                program has been started manually for a re-run then also
\***                display a message on the background screen.
\***                The event will be preceded by one indicating the port
\***                being monitored by the program in error.
\***
\***   Parameters : 2
\***
\***      SB.EVENT.NO% = number of event to be logged
\***      SB.UNIQUE$   = 10 byte block of data unique to event
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.LOG.AN.EVENT:

      MESSAGE.NO% = 0
      UNIQUE.2$ = ""

      PORT.STRING$ = "PORT : " + MONITORED.PORT$ + "  "
      PORT.EVENT% = 75

      RC% = APPLICATION.LOG(MESSAGE.NO%,                                \
                            PORT.STRING$,                               \
                            UNIQUE.2$,                                  \
                            PORT.EVENT% )

      RC% = APPLICATION.LOG(MESSAGE.NO%,                                \
                            SB.UNIQUE$,                                 \
                            UNIQUE.2$,                                  \
                            SB.EVENT.NO% )

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.FORMAT.ERROR.DATA
\***
\***   Purpose    : General routine to format the common error reporting
\***                data
\***
\***   Parameters : 0
\***
\***   Output     :
\***      SB.ERRS$   = ERRN converted to a 4 byte string
\***      SB.ERRL$   = ERRL zero packed up to 6 bytes
\***      SB.ERRF$   = ERRF converted to a reporting number (1 byte)
\***
\***   Error action : if hex conversion or string conversion fails then the
\***                  program ends
\***
\******************************************************************************

   SB.FORMAT.ERROR.DATA:

      RC% = CONV.TO.HEX( ERRN )                                         ! BLC
      IF RC% <> 0 THEN                                                  \ BLC
         GOTO PROGRAM.EXIT

      RC% = CONV.TO.STRING(0,                                           \ BLC
                           ERRN )
      IF RC% <> 0 THEN                                                  \ BLC
         GOTO PROGRAM.EXIT
      SB.ERRS$ = F17.RETURNED.STRING$

      SB.ERRL$ = FN.Z.PACK(STR$(ERRL), 6)
      SB.ACTION$ = "R" : SB.INTEGER% = ERRF% : SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +                      \ FMJK
                 CHR$(SHIFT(SB.FILE.REP.NUM%,0))                        ! FMJK

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.FILE.OPEN.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to open a file.
\***
\***   Parameters : 1
\***
\***      CURR.SESS.NUM% = file session number of the file that caused the
\***                       error, this is used to look-up the file reporting
\***                       number that is logged in the event's unique data.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.OPEN.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
        SB.EVENT.NO% = 106                                              ! ESMG
        SB.UNIQUE$ = "O" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ ESMG
                     CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                  \ ESMG
                     PACK$(STRING$(16,"0"))                             ! ESMG

      GOSUB SB.LOG.AN.EVENT

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.FILE.READ.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to read a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***      CURRENT.KEY$    = value of key used to try to read from file.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

  SB.FILE.READ.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

        SB.EVENT.NO% = 106                                              ! ESMG
        SB.UNIQUE$ = "R" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ ESMG
                     CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                  \ ESMG
                     FN.Z.PACK(CURRENT.KEY$,8)                          ! ESMG

      GOSUB SB.LOG.AN.EVENT

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.FILE.WRITE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to write a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***    CURRENT.KEY$    = value of key used to try to write to file.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.WRITE.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

        SB.EVENT.NO% = 106                                              ! ESMG
        SB.UNIQUE$ = "W" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ ESMG
                     CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                  \ ESMG
                     FN.Z.PACK(CURRENT.KEY$,8)                          ! ESMG

      GOSUB SB.LOG.AN.EVENT

   RETURN


\******************************************************************************
\***
\***   Subroutine : SB.FILE.CREATE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to create a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.CREATE.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

        SB.EVENT.NO% = 106                                              ! ESMG
        SB.UNIQUE$ = "C" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ ESMG
                     CHR$(SHIFT(SB.FILE.REP.NUM%,4))                    ! ESMG

      GOSUB SB.LOG.AN.EVENT

   RETURN


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***   E R R O R   H A N D L I N G                                          ***
\***                                                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   OPEN.ERROR:
\***
\***      log an event 6 (open error)
\***      set RECEIVE.STATE$ to "*"
\***
\***      if the source of the error is known then return to one of the
\***      specified return points so the program does not end
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   OPEN.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.OPEN.ERROR
      RECEIVE.STATE$ = "*"

      GOTO MODULE.EXIT


\******************************************************************************
\***
\***   READ.ERROR:
\***
\***      log an event 6 (read error)
\***      set RECEIVE.STATE$ to "*"
\***      if the source of the error is known then return to one of the
\***      specified return points so the program does not end
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   READ.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.READ.ERROR
      RECEIVE.STATE$ = "*"

      GOTO MODULE.EXIT


\******************************************************************************
\***
\***   WRITE.ERROR:
\***
\***      log an event 6 (write error)
\***      set RECEIVE.STATE$ to "*"
\***
\***      if the source of the error is known then return to one of the
\***      specified return points so the program does not end
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   WRITE.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.WRITE.ERROR
      RECEIVE.STATE$ = "*"

   GOTO MODULE.EXIT


\******************************************************************************
\***
\***   CREATE.ERROR:
\***
\***      log an event 6 (create error)
\***      set RECEIVE.STATE$ to "*"
\***
\***      if the source of the error is known then return to one of the
\***      specified return points so the program does not end
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   CREATE.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.CREATE.ERROR
      RECEIVE.STATE$ = "*"

   GOTO MODULE.EXIT


\******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***      if an error has occurred after a 'fatal' error then quit program
\***
\***      increment ERROR.COUNT%
\***      NOTE : all returns from error detected should decrement the
\***             variable ERROR.COUNT% and exit using the RESUME command
\***      if ERROR.COUNT% > 1 then end program (error within error detected)
\***
\***      set-up common error reporting information
\***
\***      if an access conflict occurs on a session then retry
\***
\***      if error is out of memory then log an event, using ADXERROR and
\***      quit program
\***
\***      log an event 1
\***
\***   resume MODULE.EXIT
\***
\******************************************************************************

ERROR.DETECTED:

   IF ERR = "CU" OR ERR = "DF" THEN RESUME

   IF (ERRN = 00000052h) AND (LDTAF.LINK.TYPE%<>0) THEN RESUME          ! FMJK

   ERROR.COUNT% = ERROR.COUNT% + 1
   IF ERROR.COUNT% > 1 THEN RESUME PROGRAM.EXIT

   GOSUB SB.FORMAT.ERROR.DATA

   IF (ERRN AND 0000FFFFh) = 400Ch THEN BEGIN
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME RETRY
   ENDIF

   IF ERR = "OM" THEN BEGIN
      SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                                \ FMJK
                   PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))             ! FMJK
      CALL ADXERROR(0, 74, 0, 3, 1, SB.UNIQUE$ )
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME PROGRAM.EXIT
   ENDIF

   SB.EVENT.NO% = 101
   SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                                   \ FMJK
                PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))                ! FMJK
   GOSUB SB.LOG.AN.EVENT

   ERROR.COUNT% = ERROR.COUNT% - 1
   RECEIVE.STATE$ = "*"
   RESUME MODULE.EXIT

END SUB
