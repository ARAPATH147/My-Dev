\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSS3704.bas  $
\***
\***   $Revision:   1.18  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PSS3704.bav  $
\***   
\***      Rev 1.18   02 Jul 2007 12:44:48   greenfield.brian
\***   Changes for A7C Recalls.
\***   
\***      Rev 1.17   24 Dec 2004 13:04:44   dev07ps
\***   A5A Shelf Monitor Project
\***   
\***   
\***      Rev 1.16   20 Apr 2004 13:51:52   devjsps
\***   Changes to Goods Out functionality 
\***   as part of A4D package upgrade
\***
\***      Rev 1.15   20 Feb 2004 11:26:32   devjsps
\***   Code change for goods out functionality
\***   If item qty * item price > 999999 then the item
\***   will be split into individual records
\***
\***      Rev 1.14   16 Dec 2003 09:09:42   devjsps
\***   Program updated to add Pharmacy Smartscript
\***   Stock count changes and Credit Claim changes.
\***   David Artiss has also added Stock takings changes
\***   to PSB3704.BAS
\***
\***      Rev 1.13   09 Jul 2003 11:17:08   dev38ps
\***   Ensure LOCCNT is opened whether it's a normal stocktake or BOL one.
\***
\***      Rev 1.11   29 Oct 2002 08:37:26   dev38ps
\***   Improved stocktaking auditing
\***
\***      Rev 1.10   12 Oct 2001 12:03:56   DEV38PS
\***   Added LDT audit file processing
\***
\***      Rev 1.9   08 Apr 1998 11:08:14   DEV45PS
\***   Changes to cope with dates of '000000' and '999999' and to include revised CMPDATE
\***
\***      Rev 1.8   05 Feb 1998 11:58:26   DEV45PS
\***
\***
\***      Rev 1.7   07 Oct 1997 14:01:52   DEV45PS
\***   Changed quantity & price on STKBF to unpacked
\***
\***      Rev 1.6   25 Sep 1997 13:22:30   DEV45PS
\***   Increased processing speed of data from stocktake PDT. Also now sends acknowledgement back to PDT after transmission.
\***
\***      Rev 1.5   10 Sep 1997 11:44:38   DEV45PS
\***   Changes For Stocktake System - Processes transmissions from stocktake PDT's
\***
\***      Rev 1.4   17 Jul 1996 11:03:56   DEVDSPS
\***   ECC enhancements - Download despatched UODs to LDT.
\***
\***      Rev 1.3   15 Feb 1995 12:57:56   NIK
\***   Additional Returns Amendment.
\***
\***
\***      Rev 1.2   14 Feb 1995 11:44:38   NIK
\***   Fixes to Boots Problems 1248, 1245, 1246, 1318, 1105
\***
\***      Rev 1.1   07 Oct 1994 15:51:44   DEVSPPS
\***   Fixed PDT Support hang
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3704                                          ***
\***                                                                        ***
\***           AUTHOR   :  Michael J. Kelsall                               ***
\***                                                                        ***
\***           DATE     :  04th October 1993                                ***
\***                                                                        ***
\***                                                                        ***
\***           Current version letter : B                                   ***
\***                                                                        ***
\***           Date of last update:  17th May 1994  Michael J. Kelsall      ***
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
\***   PSS37 - P.D.T. / L.D.T. Support Program.                             ***
\***                                                                        ***
\***                                                                        ***
\***   PSS37 is designed to run concurrently with PSS38. PSS38 handles      ***
\***   all asyncronous communications with a connected PDT/LDT. All data    ***
\***   sent by the PDT/LDT is passed to PSS37 via PSS38 by means of a pipe. ***
\***   PSS37 validates the data sent to ensure the data has been sent in    ***
\***   the correct sequence, has a valid format and is meaningful.          ***
\***                                                                        ***
\***                                                                        ***
\***   This module was created to handle the RETURNS/AUTOMATIC CREDIT       ***
\***   CLAIMING processing. This application will allow a user to transfer  ***
\***   RETURNS data captured via the LDT. This information will then be     ***
\***   used to update relevent controller files, including the generation   ***
\***   of stock movements that will be processed by Stock Support.            ***
\***   The application will also provide the ability to download current    ***
\***   versions of the RETURNS data files contained within the CCUOD and    ***
\***   CCITM files.                                                         ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***    A M E N D M E N T S                                                 ***
\***                                                                        ***
\***    Version A (Supplemental)  David Smallwood         13 January 1994   ***
\***    Change to processing flow - only write CCTMP to STKMQ after         ***
\***    LDT has completed housekeeping.                                     ***
\***                                                                        ***
\***    Version B  Michael J. Kelsall                     23rd March 1994   ***
\***    Changes required due to modifications to system;                    ***
\***    - Change to throw out UOD records if the current status of the UOD  ***
\***       (as per the CCUPF) has changed since the data was down loaded to ***
\***       the LDT, this can occur if two LDTs log on one immediately after ***
\***       the other and they have different updates made against the same  ***
\***       UOD's.                                                           ***
\***    - Removal of CCTRL locked access to prevent access conflicts.       ***
\***    - Changes to process four digit quantity fields from the LDT as     ***
\***       opposed to the original three digits.                            ***
\***    - Change to remove reseting of item index against items within a    ***
\***       UOD. This value should be sequential throughout all the items    ***
\***       passed to the LDT via the LDTWF.                                 ***
\***    - Change to pass the UOD index before the Item index in the data    ***
\***    - Change to pass only item codes for any closed UOD's, Barcodes     ***
\***       do not need to be sent to the LDT                                ***
\***    - Change to add primer UOD records to the CCUPF to prevent          ***
\***       duplicate processing if transmission fails.                      ***
\***       (Primer record in CCUPF will be based on 00000000hhmmss).        ***
\***    - Change to sort Barcodes sent to LDT using UOD and item pos table  ***
\***       as well as the barcode table.                                    ***
\***    - Change to re-create CCUPF if data file loss detected.             ***
\***                                                                        ***
\***    Version B (Supplemental)  Michael J. Kelsall      03rd June 1994    ***
\***    - Change to prevent update of LDT number on CCUOD if all records    ***
\***       in CCTMP are suppressed and hence type 27 will also be           ***
\***       suppressed.                                                      ***
\***                                                                        ***
\***    Version B (Supplemental)  Michael J. Kelsall      15th June 1994    ***
\***    - Change to correct problem in CCWKF creation routine; Problem      ***
\***       appears during processing of excessive number of items resulting ***
\***       in failure of LEN command (returns 2INT therefore result turns   ***
\***       negative if string longer than 32767 characters!)                ***
\***
\***    REVISION 1.1     ROBERT COWEY / STEVE WRIGHT      6TH OCTOBER 1994
\***    Removed version letters from included code (notcommented).
\***
\***    Version C              Nik Sen                 3rd February 1995
\***    Change to fix Boots Problem 1318 (duplicate returns information
\***    being written to Stock Movement Queue). Transfer of CCTMP to
\***    STKMQ now takes place after End Of Transmission received.
\***    CCUPF no longer updated by primer UOD's.
\***
\***    Version 1.4            David Smallwood        21st May 1996
\***    Change to transmit all despatched UODs to LDT as well as open
\***    and closed UODs.  Despatched UODs taken from CCLAM file.  This
\***    change is part of ECC enhancements and will help prevent duplicate
\***    claims returning to the centre.
\***
\***    Version 1.5            Nik Sen                 30th June 1997
\***    Added data states p,q,r for stocktake processing.
\***
\***    Version 1.6            Nik Sen                 22nd September 1997
\***    Changed copy of stocktake temporary file to buffer file to speed it up.
\***
\***    Version 1.7            Nik Sen                 7th October 1997
\***    Fixed pilot problem - data now written to STKBF unpacked.
\***
\***    Version 1.8            Nik Sen                 19th November 1997
\***    Y2000 changes. Date comparisons in RECEIVED.RETURNS.FILE.ID.
\***
\***    Version 1.9            Nik Sen                 10th December 1997
\***    Changes to accept altered file format from stocktake PDT.
\***
\***    Version 1.10           Nik Sen                 1st April 1998
\***    Changed Y2000 code to cope with dates of '000000'. (Yes really even
\***    though it is April 1st).
\***
\***    Version 1.11           David Artiss            23rd January 2001
\***    Added stocktake audit file STLDT - details of stocktaking transmissions
\***    written to it for confirmation purposes.
\***
\***    Version 1.12           David Artiss            5th September 2002
\***    Moved the writing to the STLDT file so it only updates after a
\***    totally successful transmission.
\***
\***    Version 1.13           David Artiss                7th March 2003
\***    Added new processing to allow stocktake PDTs with other stores
\***    data to be transmitted and sent straight to the appropriate
\***    mainframe file.
\***
\***    Version 1.14           David Artiss                 23rd May 2003
\***    Ensure LOCCNT is opened whether it's a normal stocktake or BOL one.
\***
\***    Version 1.15           David Artiss                25th July 2003
\***    Decimal places are now written to STLDT.
\***
\***    Version 1.16           Julia Stones                14th November 2003
\***    As part of the Simplification of the Credit Claiming project a new
\***    Option has been added to the PDT/LDT STOCKROOM Menu
\***    Option 3 Credit Claiming
\***    The existing Goods Out Data States will be used.  The only change to
\***    these being Data State (g) (Received Returns File ID). This will
\***    contain an extra 1 byte ASCII field, set to either "G" Goods out or
\***    "O" Credit Claiming.
\***    Code has been added to this module to process the new credit Claiming
\***    data and write out a stock movement type 25.
\***    If any items have been rejected then a report will be printed directly
\***    onto the printer
\***
\***    Version 1.17         Julia Stones                 17th February 2004
\***    As part of the stock loss investigation it was found that when an
\***    electronic credit claim was made and the item qty * item price exceeded
\***    £9,999.99 the most significant digits above this value are lost.
\***    The problem is with the CCITF.BIN file, this can only hold a 3 byte UPD
\***    value.  When Stock support updates the CCITF it calculates the total item
\***    value and when this is then written to the CCITF file anything above £9,999.99
\***    is lost (the stock movement record is OK as this holds the individual item price)
\***    The code for Other Credit Claim is being changed to check the value of
\***    the item qty * item price, if this exceeds £9,999.99 then this item will
\***    be split into individual items.  (Goods out is not being changed)
\***
\***    Version 1.18        Julia Stones                  12th March 2004
\***    Added code to write out rejection information to the new CCREJ file
\***    where a Goods out transmission or Other Credit Claim transmission has
\***    been performed and items are removed from the claim as they are either
\***    for an invalid business centre or the item does not exist on both the
\***    IDF and IRF.
\***    Items not on file has been commmented out (may only be a requirement
\***    if all items in all Stores project is initiated).
\***    Goods Out transmission will only reject items where the item business
\***    centre does not match the claim business centre if the rejection flag
\***    on softs is set to all (the right most value on CREDIT.CLAIM.FLAG$) = "Y"
\***    Removed all the code for writing out to the rejection report and the
\***    printing of the rejection report.
\***    Upon the end of a Goods Out or Other Credit claim transmission if there
\***    are any records in the CCREJ file then ADXSTART new program PSS25 to print
\***    the rejection report (the rejection report must print before any advice
\***    of contents reports are produced.
\***
\***    Version 1.19        Charles Skadorwa                  2nd November 2004
\***    Increased CCLAM table size from 400 to 1600 in  
\***    subroutine: DIRECT.EXTRACT.CCLAM.KEYS
\***    Store 7 West Hallam (warehouse) have exceeded the old limit due to an 
\***    increase in claiming (inter-store transfer of stock). This prevented 
\***    the PDT's working when using Goods Out.
\***
\***    Version 1.20        Brian Greenfield                  14th may 2007
\***    Added CCTMP.RECALL.TYPE$ for use with A7C Recalls - only used for
\***    type 26 record when written to the STKMQ.
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   I N C L U D E S   A N D   V A R I A B L E S                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

   INTEGER*1 GLOBAL STOCKTAKING.ALTERNATIVE.STORE                      ! 1.13DA

   %INCLUDE PSS37G.J86                                                 ! BMJK

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL
      INTEGER*2 ADXSTART
      STRING    NAME$, PARM$, MESS$
   END FUNCTION

   FUNCTION FN.INVERT.DATE(PARM$)
      STRING FN.INVERT.DATE,PARM$
      PARM$ = RIGHT$("000000"+PARM$,6)
      FN.INVERT.DATE = RIGHT$(PARM$,2)+MID$(PARM$,3,2)+LEFT$(PARM$,2)
   END FUNCTION

   SUB PSS3704 PUBLIC

REAL                                                               \
       GAP,                                                        \
       CCTMP.RECORD.COMPLETE,                                      \
       DETAIL.RECORDS.RECEIVED,                                    \
       END.OF.CCTMP,                                               \
       END.OF.FILE,                                                \
       TABLE.FULL

INTEGER*1                                                          \
       CURR.SESS.NUM%,                                             \
       MORE.BAR.CODES,                                             \ BMJK
       PORT.EVENT%,                                                \
       PRIMER.PRESENT,                                             \ BMJK
       SB.EVENT.NO%,                                               \
       SB.FILE.SESS.NUM%,                                          \
       FINISHED,                                                   \
       SWAPPED,                                                    \
       HEADER.EXPECTED,                                            \ 1.13DA
       GROUP.CODE,                                                 \ 1.13DA
       CURRENT.CODE,                                               \ 1.13DA
       CODE.LENGTH,                                                \ 1.13DA
       VALID.ENTRY%,                                               \ 1.16JAS
       VALID.BC,                                                   \ 1.16JAS
       VALID.DATA.PRESENT                                          ! BMJK

INTEGER*2                                                          \
       R%,                                                         \ 1.13DA
       IDF.NUM.BARCODES%,                                          \ BMJK
       NUM.OF.UODS.TRANSMITTED%,                                   \ 1.4
       NUM.OF.OPEN.CLOSED.UODS%,                                   \ 1.4
       INDEX%,                                                     \ BMJK
       MESSAGE.NO%,                                                \ BMJK
       NUM.SECTORS%,                                               \ BMJK
       SB.FILE.REP.NUM%,                                           \
       SB.INTEGER%,                                                \
       I%,                                                         \ 1.6
       NO.MAX.RECS%                                                ! 1.6

INTEGER*4                                                        \
          ADX.RET.CODE%,                                                  \
          A%,                                                        \
          B%,                                                        \
          STOCKTAKE.COUNT,                                           \ 1.5
          EXPECTED.RECORDS,                                          \ 1.5
          MAX%,                                                      \
          INPUT.SIZE%,                                               \ 1.6
          MAX.REC.SIZE%,                                             \ 1.6
          CUR.LOC.COUNTER%,                                          \ 1.13DA
          ITEM.COUNT,                                                \ 1.13DA
          BATCH.ITEMS,                                               \ 1.13DA
          QUANTITY,                                                  \ 1.13DA
          SAVED.RECORD.NUM%,                                         \ 1.13DA
          REC.LEN%,                                                  \ 1.16JAS
          COMMA.POS%,                                                \ 1.16JAS
          END.POS%,                                                  \ 1.16JAS
          ITEM.COUNT%,                                               \ 1.16JAS
          VALID.INDEX%,                                              \ 1.16JAS
          ITEM.QTY%,                                                 \ 1.17JAS
          EXTRA.ITEMS%,                                              \ 1.17JAS
          REMAINDER%                                                 ! 1.6

STRING                                                               \
       CCTMP.ITEM.QTY$,                                          \ BMJK
       CCTMP.NUM.ITEM.UPDATED$,                                   \ BMJK
       CCTMP.TOTAL.QTY.IN.UOD$,                                   \ BMJK
       CCUOD.DATE.TIME$,                                          \ BMJK
       CCUOD.KEY$,                                                 \ BMJK
       CCLAM.KEY$,                                                 \ 1.4
       STATUS$,                                                    \ 1.4
       CRLF$,                                                        \
       CURRENT.KEY$,                                                 \ BMJK
       BARCODE.TABLE$(1),                                          \
       BARCODE.POS.TABLE$(1),                                          \
       FILLER$,                                                 \ BMJK
       ITEM.POS.STRING$,                                          \ BMJK
       SB.ACTION$,                                                    \
       SB.ERRF$,                                                      \
       SB.ERRL$,                                                      \
       SB.ERRS$,                                                      \
       SB.MESSAGE$,                                                   \
       SB.UNIQUE$,                                                    \
       SB.STRING$,                                                    \
       SB.BOOTS.ITEM$,                                                \ 1.13DA
       SB.BOOTS.CODE$,                                                \ 1.13DA
       TEMP$,                                                    \
       TEMP.STATE$,                                                 \ BMJK
       TEMP.RECORD$,                                                   \ 1.16JAS
       UNIQUE.2$,                                                 \
       UOD.POS.STRING$,                                                \
       CUR.LOC$,                                                       \ 1.13DA
       NEW.LOC$,                                                       \ 1.13DA
       STOCKTAKER$,                                                    \ 1.13DA
       STORE$,                                                         \ 1.13DA
       CR$,                                                            \ 1.13DA
       ITEM.CODE$,                                                     \ 1.13DA
       RECORD.FORMAT$,                                                 \ 1.6
       DATA.BLOCK$,                                                    \ ! 1.6 1.16JAS
       PREV.BC$,                                                       \ 1.16JAS
       BOOTS.CODE$,                                                    \ 1.16JAS
       COMMA$,                                                         \ 1.16JAS
       LDT.RETURNS.TYPE$,                                              \ 1.16JAS
       OTHER.CLAIM.VALID.BC$,                                          \ 1.16JAS
       BLANK.LINE$,                                                    \ 1.16JAS
       FOOTER.LINE$,                                                   \ 1.16JAS
       PAGE.THROW$,                                                    \ 1.16JAS
       TEMP.TABLE$(1)                                                  ! 1.17JAS

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
\***
\***   MODULE.EXIT:
\***     return to calling module
\***
\***   PROGRAM.EXIT:
\***     stop
\***
\******************************************************************************

ON ERROR GOTO ERROR.DETECTED

      IF MATCH(RECEIVE.STATE$,"ghijklmnopqr",1) > 0 THEN BEGIN         ! 1.5
         ON (ASC(RECEIVE.STATE$) - ASC("f")) GOSUB                       \
              RECEIVED.RETURNS.FILE.ID,                            \
              RECEIVED.RETURNS.FILE.HEADER,                            \
              RECEIVED.RETURNS.UOD.HEADER,                            \
              RECEIVED.RETURNS.ITEM.DETAIL,                            \
              RECEIVED.RETURNS.UOD.TRAILER,                            \
              RECEIVED.RETURNS.FILE.TRAILER,                            \
              RECEIVED.RETURNS.FILE.REQUEST,                            \
              RECEIVED.RETURNS.FILE.RECEIVED.OK,                     \
              RECEIVED.RETURNS.EOT,                                    \ 1.5
              RECEIVED.STOCKTAKE.HEADER,                               \ 1.5
              RECEIVED.STOCKTAKE.DETAIL,                               \ 1.5
              RECEIVED.STOCKTAKE.TRAILER                               ! 1.5
        GOTO MODULE.EXIT
      ENDIF


MODULE.EXIT:

   EXIT SUB

PROGRAM.EXIT:

   STOP


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***          M A I N     R E T U R N S     S U B R O U T I N E S           ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.FILE.ID:                                            ***
\***                                                                        ***
\***     validate file id record, return if invalid                         ***
\***                                                                        ***
\***     log details to audit file                                          ***
\***     extract records from data record                                   ***
\***                                                                        ***
\***     As part of the new credit claiming system a new flag will be       ***
\***     passed on the data record.  If the flag is set to "G" then the     ***
\***     transmission is Goods Out, if the flag is set to "O" then the      ***
\***     transmission is Credit Claiming                                    ***
\***                                                                        ***
\***     open CCTRL and read current claim number                           ***
\***     if claim number on LDT is greater than controller;                 ***
\***       update CCTRL with value from LDT                                 ***
\***       close CCTRL                                                      ***
\***                                                                        ***
\***     compare the date/time of the controller CCUOD with the date/time   ***
\***     on the LDT then;                                                   ***
\***       if the controller values = 999999999999;                         ***
\***         format a file ID reply of 999999999999 for the date/time       ***
\***       otherwise if the LDT values > controller values;                 ***
\***         format a file ID reply of 000000000000 for the date/time       ***
\***       otherwise if the controller values >= LDT values;                ***
\***         format a file ID reply of the controller values for date/time  ***
\***                                                                        ***
\***     transmission of the file ID reply to the LDT                       ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

    RECEIVED.RETURNS.FILE.ID:

      SB.MESSAGE$ = "PDT Support - RETURNS file ID received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 34) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         GOTO MODULE.EXIT
      ENDIF

      LDT.LAST.CLAIM.NUM$ = MID$(DATA.IN$,3,8)
      LDT.CCUOD.UPDATE.DATE$ = MID$(DATA.IN$,11,6)
      LDT.CCUOD.UPDATE.TIME$ = MID$(DATA.IN$,17,6)
      LDT.RETURNS.TYPE$ = MID$(DATA.IN$,23,1)                          ! 1.16JAS  "G" = Goods Out
                                                                       ! 1.16JAS  "O" = Credit Claiming

      CURR.SESS.NUM% = CCTRL.SESS.NUM%
      IF END #CCTRL.SESS.NUM% THEN OPEN.ERROR
      OPEN CCTRL.FILE.NAME$ DIRECT RECL CCTRL.RECL% AS CCTRL.SESS.NUM%        ! BMJK
      CCTRL.OPEN.FLAG$ = "Y"

      CCTRL.REC.NUM% = 1
      CURRENT.KEY$ = "1"
      IF READ.CCTRL.LOCKED THEN GOTO READ.ERROR
      IF LDT.LAST.CLAIM.NUM$ > UNPACK$(CCTRL.CREDIT.CLAIM.NUM$) THEN BEGIN
        CCTRL.CREDIT.CLAIM.NUM$ = PACK$(LDT.LAST.CLAIM.NUM$)
       CCTRL.REC.NUM% = 1
       IF WRITE.UNLOCK.CCTRL THEN GOTO WRITE.ERROR
      ENDIF

      CCTRL.OPEN.FLAG$ = "N"
      CLOSE CCTRL.SESS.NUM%

      LDT.DATE.TIME$ = FN.INVERT.DATE(LDT.CCUOD.UPDATE.DATE$) +              \
                     LDT.CCUOD.UPDATE.TIME$

      IF LEFT$(LDT.DATE.TIME$,6) = "000000" THEN BEGIN                 ! 2.0
         LDT.DATE.TIME$ = "850101" + LDT.CCUOD.UPDATE.TIME$            ! 2.0
      ENDIF                                                            ! 2.0

      IF LEFT$(LDT.DATE.TIME$,6) = "999999" THEN BEGIN                 ! 2.0
         LDT.DATE.TIME$ = "841231" + LDT.CCUOD.UPDATE.TIME$            ! 2.0
      ENDIF                                                            ! 2.0

      IF LEFT$(LDT.DATE.TIME$,2) < "85" THEN BEGIN                     ! 1.8
         LDT.DATE.TIME$ = "20" + LDT.DATE.TIME$                        ! 1.8
      ENDIF ELSE BEGIN                                                 ! 1.8
         LDT.DATE.TIME$ = "19" + LDT.DATE.TIME$                        ! 1.8
      ENDIF                                                            ! 1.8

      CCUOD.DATE.TIME$ = UNPACK$(CCUOD.DATE.FILE.UPDATED$) +              \
                         UNPACK$(CCUOD.TIME.FILE.UPDATED$)

      IF LEFT$(CCUOD.DATE.TIME$,6) = "000000" THEN BEGIN               ! 2.0
         CCUOD.DATE.TIME$ = "850101" + UNPACK$(CCUOD.TIME.FILE.UPDATED$) ! 2.0
      ENDIF                                                            ! 2.0

      IF CCUOD.DATE.TIME$ <> STRING$(12,"9") THEN BEGIN                ! 1.8
         IF LEFT$(CCUOD.DATE.TIME$,6) = "999999" THEN BEGIN            ! 2.0
            CCUOD.DATE.TIME$ = "841231" + UNPACK$(CCUOD.TIME.FILE.UPDATED$) ! 2.0
         ENDIF                                                         ! 2.0
         IF LEFT$(CCUOD.DATE.TIME$,2) < "85" THEN BEGIN                ! 1.8
            CCUOD.DATE.TIME$ = "20" + CCUOD.DATE.TIME$                 ! 1.8
         ENDIF ELSE BEGIN                                              ! 1.8
            CCUOD.DATE.TIME$ = "19" + CCUOD.DATE.TIME$                 ! 1.8
         ENDIF                                                         ! 1.8
      ENDIF                                                            ! 1.8

      IF CCUOD.DATE.TIME$ = STRING$(12,"9") THEN BEGIN
        LDT.CCUOD.UPDATE.DATE$ = "999999"
        LDT.CCUOD.UPDATE.TIME$ = "999999"
         GOSUB CREATE.EMPTY.CCUPF                                   ! BMJK
      ENDIF ELSE IF LDT.DATE.TIME$ > CCUOD.DATE.TIME$ THEN BEGIN
        LDT.CCUOD.UPDATE.DATE$ = "000000"
        LDT.CCUOD.UPDATE.TIME$ = "000000"
      ENDIF ELSE IF LDT.DATE.TIME$ <= CCUOD.DATE.TIME$ THEN BEGIN
        LDT.CCUOD.UPDATE.DATE$ =                                    \
            FN.INVERT.DATE(UNPACK$(CCUOD.DATE.FILE.UPDATED$))
        LDT.CCUOD.UPDATE.TIME$ = UNPACK$(CCUOD.TIME.FILE.UPDATED$)
      ENDIF

      PIPE.OUT$ = "L" +                                                 \
                  "RI" +                                                \
                  FN.Z.PACK(UNPACK$(CCTRL.CREDIT.CLAIM.NUM$),8) +       \
                  FN.Z.PACK(LDT.CCUOD.UPDATE.DATE$,6) +                 \
                  FN.Z.PACK(LDT.CCUOD.UPDATE.TIME$,6)

      GOSUB SEND.TO.PSS38

    RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.FILE.HEADER:                                        ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     create CCBUF                                                       ***
\***     create CCREJ                                                       ***
\***                                                                        ***
\***     If expected number of uods > 0                                     ***
\***        If LDT.RETURNS.TYPE is "O" Credit Claiming                      ***
\***           Open the Store options file                                  ***
\***           Read record 105                                              ***
\***           Put valid Claim Business Centres into a variable             ***
\***           Close Store options file                                     ***
\***        endif                                                           ***
\***     endif else                                                         ***
\***        if LDT.RETURNS.TYPE is "G" Goods out                            ***
\***           write a type 27 stock movement record                        ***
\***     endif                                                              ***
\***                                                                        ***
\***     record information on the LDT audit file                           ***
\***                                                                        ***
\***     create new CCTMP for storage                                       ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

    RECEIVED.RETURNS.FILE.HEADER:

      SB.MESSAGE$ = "PDT Support - RETURNS file hdr received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 35) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         GOTO MODULE.EXIT
      ENDIF

      GOSUB CREATE.CCBUF                                               !ADS
      GOSUB CREATE.CCREJ                                               ! 1.18 JAS
      CCBUF.RECS.PRESENT = FALSE                                       !ADS
      CCREJ.RECS.PRESENT = FALSE                                       ! 1.18JAS
      DETAIL.RECORDS.RECEIVED = FALSE
      ACTUAL.NUM.OF.UODS% = 0
      ITEM.POS% = 0                                                 !BMJK
      EXPECTED.NUM.OF.UODS% = VAL(MID$(DATA.IN$,3,4))

      CSR.AUDIT.DATA$ = "RETURNS File Header. " +                      \
                        STR$(EXPECTED.NUM.OF.UODS%) + " UOD(s) to follow."
      GOSUB LOG.TO.AUDIT.FILE

      IF EXPECTED.NUM.OF.UODS% > 0 THEN BEGIN                            !BMJK
        CSR.AUDIT.DATA$ = "    UOD Number      Status   " +               \BMJK
                          "Total items  Updated items "               !BMJK
        GOSUB LOG.TO.AUDIT.FILE                                          !BMJK
        ! Read store options record 105 to get valid claim Business centres
        IF SOPTS.OPEN.FLAG$ <> "Y" THEN BEGIN                       ! 1.16JAS
           CURR.SESS.NUM% = SOPTS.SESS.NUM%                         ! 1.16JAS
           IF END# SOPTS.SESS.NUM% THEN OPEN.ERROR                  ! 1.16JAS
           OPEN SOPTS.FILE.NAME$ DIRECT RECL 102      \             ! 1.16JAS
              AS SOPTS.SESS.NUM% NOWRITE NODEL                      ! 1.16JAS

        ENDIF                                                       ! 1.16JAS

        SOPTS.REC.NUM% = 105                                        ! 1.16JAS

        RC% = READ.SOPTS                                            ! 1.16JAS

        IF RC% <> 0 THEN GOTO READ.ERROR                            ! 1.16JAS

        REC.LEN% = LEN(SOPTS.RECORD$)                               ! 1.16JAS
        COMMA$   = ","                                              ! 1.16JAS
        COMMA.POS% = MATCH(COMMA$, SOPTS.RECORD$, 1) +2             ! 1.16JAS
        END.POS%   = MATCH(COMMA$, SOPTS.RECORD$, COMMA.POS%) -1    ! 1.16JAS
        END.POS%   = END.POS% - COMMA.POS%                          ! 1.16JAS
        OTHER.CLAIM.VALID.BC$ = MID$(SOPTS.RECORD$,COMMA.POS%,END.POS%) ! 1.16JAS

        SOPTS.OPEN.FLAG$ = "N"                                      ! 1.16JAS
        CLOSE SOPTS.SESS.NUM%                                       ! 1.16JAS

      ENDIF ELSE BEGIN                                                 !BMJK
        ! If Goods out and not uods to process write type 27 stock movement record
        IF LDT.RETURNS.TYPE$ = "G" THEN BEGIN                          ! 1.16JAS
           IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                            !BMJK
             IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                     !BMJK
             CURR.SESS.NUM% = STKMQ.SESS.NUM%                            !BMJK
             OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512              \BMJK
               READONLY APPEND                                                 !BMJK
             STKMQ.OPEN.FLAG$ = "Y"                                    !BMJK
           ENDIF                                                        !BMJK
           FINISH.DATE$ = DATE$                                          !BMJK
           FINISH.TIME$ = TIME$                                          !BMJK
           STKMQ.RECORD$ = CHR$(34) + PACK$("27") + CHR$(59) +               \BMJK
                           PACK$(FINISH.DATE$) +                             \BMJK
                           PACK$(FINISH.TIME$) +                             \BMJK
                           CHR$(34) + CHR$(0Dh) + CHR$(0Ah)              !BMJK
           CURRENT.KEY$ = ""                                          !BMJK
           CURR.SESS.NUM% = STKMQ.SESS.NUM%                            !BMJK
           IF WRITE.STKMQ THEN GOTO WRITE.ERROR                            !BMJK
           CLOSE STKMQ.SESS.NUM%                                          !BMJK
           STKMQ.OPEN.FLAG$ = "N"                                          !BMJK
        ENDIF                                                         ! 1.16JAS
      ENDIF                                                           !BMJK

      GOSUB CREATE.CCTMP

    RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.UOD.HEADER:                                         ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     record information on the CSR audit file                           ***
\***                                                                        ***
\***     If the LDT.RETURNS.TYPE$ = "G" Goods out then                      ***
\***      if the number of updated lines contained in the UOD is zero or the***
\***       UOD status is despatched and the previous status is closed then  ***
\***       a UOD status change is present and a type 28 CCTMP record will be***
\***       formatted otherwise format a type 26 CCTMP record                ***
\***     else                                                               ***
\***       for credit claim a type 25 CCTMP record is written and this is   ***
\***       is also written to a temp record as well                         ***
\***                                                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

    RECEIVED.RETURNS.UOD.HEADER:

      IF FN.VALIDATE.DATA(DATA.IN$,36) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        RETURN
      ENDIF

      CCTMP.CREDIT.CLAIM.NUM$ = PACK$(MID$(DATA.IN$,3,8))
      CCTMP.UOD.NUM$ = PACK$(MID$(DATA.IN$,11,14))
      CCTMP.UOD.STATUS$ = MID$(DATA.IN$,25,1)
      CCTMP.NUM.ITEMS.UPDATED% = VAL(MID$(DATA.IN$,27,4))
      CCTMP.NUM.OF.ITEMS% = VAL(MID$(DATA.IN$,31,4))
      CCTMP.PREVIOUS.STATUS$ = MID$(DATA.IN$,26,1)
      DIM F14.TABLE$(CCTMP.NUM.ITEMS.UPDATED%)                            ! 1.16JAS
      VALID.INDEX% = 0                                                    ! 1.16JAS

      ACTUAL.UOD.ITEMS% = 0

      CSR.AUDIT.DATA$ = "  " + UNPACK$(CCTMP.UOD.NUM$) +              \
                        "      " + CCTMP.UOD.STATUS$ +                     \
                     "         " +                                    \
                     RIGHT$("    "+STR$(CCTMP.NUM.OF.ITEMS%),4) +        \
                     "           " +                             \
                     RIGHT$("    "+STR$(CCTMP.NUM.ITEMS.UPDATED%),4)
      GOSUB LOG.TO.AUDIT.FILE

      !  Goods out transmission
      IF LDT.RETURNS.TYPE$ = "G" THEN BEGIN                               ! 1.16JAS
         IF (CCTMP.UOD.STATUS$ = "D" AND CCTMP.PREVIOUS.STATUS$ = "C")       \
           OR CCTMP.UOD.STATUS$ = "X" THEN BEGIN
           CCTMP.RECORD$ = CCTMP.RECORD.DELIMITER$ +                      \
                         PACK$("28") +                                   \
                        CCTMP.FIELD.DELIMITER$ +                     \
                        PACK$(DATE$) +                                   \
                        PACK$(TIME$) +                                    \
                        CCTMP.CREDIT.CLAIM.NUM$ +                     \
                           CCTMP.UOD.NUM$ +                            \
                        CCTMP.UOD.STATUS$ +                            \
                        CCTMP.RECORD.DELIMITER$ +                      \
                        CCTMP.ENDREC.MARKER$
           CCTMP.RECORD.COMPLETE = TRUE
           CURR.SESS.NUM% = CCTMP.SESS.NUM%                            ! 1.18JAS
           CURRENT.KEY$ = ""                                           ! 1.18JAS
           IF WRITE.CCTMP THEN GOTO WRITE.ERROR                        ! 1.18JAS

         ENDIF ELSE BEGIN
           CCTMP.NUM.ITEM.UPDATED$ = MID$(DATA.IN$,27,4)
           CCTMP.TOTAL.QTY.IN.UOD$ = MID$(DATA.IN$,31,4)
           CCTMP.ADJ.STOCK.FIGURE$ = MID$(DATA.IN$,35,1)
           CCTMP.DATE.UOD.OPENED$ = PACK$(FN.INVERT.DATE(MID$(DATA.IN$,36,6)))
           CCTMP.DATE.DESPATCHED$ = PACK$(FN.INVERT.DATE(MID$(DATA.IN$,42,6)))
           CCTMP.TIME.DESPATCHED$ = PACK$(MID$(DATA.IN$,48,6))
           CCTMP.SUPPLY.ROUTE$ = MID$(DATA.IN$,54,1)
           CCTMP.DISP.LOCATION$ = MID$(DATA.IN$,55,1)
           CCTMP.BC.LETTER$ = MID$(DATA.IN$,56,1)
           CCTMP.RECALL.NUM$ = MID$(DATA.IN$,57,8)
           CCTMP.AUTHORISATION$ = MID$(DATA.IN$,65,15)
           CCTMP.SUPPLIER$ = MID$(DATA.IN$,80,15)
           CCTMP.RETURN.METHOD$ = PACK$("0"+MID$(DATA.IN$,95,1))
           CCTMP.CARRIER$ = PACK$("0"+MID$(DATA.IN$,96,1))
           CCTMP.BIRD.NUM$ = MID$(DATA.IN$,97,8)
           CCTMP.RETURN.REASON$ = PACK$(MID$(DATA.IN$,105,2))
           CCTMP.RECEIVING.STORE$ = PACK$(MID$(DATA.IN$,107,4))
           CCTMP.DESTINATION$ = PACK$("0"+MID$(DATA.IN$,111,1))
           CCTMP.WAREHOUSE.ROUTE$ = MID$(DATA.IN$,112,1)
           CCTMP.UOD.TYPE$ = PACK$("0"+MID$(DATA.IN$,113,1))
           CCTMP.DAMAGE.REASON$ = PACK$("0"+MID$(DATA.IN$,114,1))
           CCTMP.RECALL.TYPE$ = " "                                 ! 1.20BG

           CCTMP.RECORD$ = CCTMP.RECORD.DELIMITER$ +                     \
                          PACK$("26") +                                   \
                        CCTMP.FIELD.DELIMITER$ +                      \
                        PACK$(DATE$) +                                    \
                        PACK$(TIME$) +                                    \
                        CCTMP.UOD.NUM$ +                             \
                        CCTMP.UOD.STATUS$ +                             \
                        CCTMP.CREDIT.CLAIM.NUM$ +                     \
                        CCTMP.NUM.ITEM.UPDATED$ +                     \
                        CCTMP.TOTAL.QTY.IN.UOD$ +                     \
                        CCTMP.ADJ.STOCK.FIGURE$ +                     \
                        CCTMP.SUPPLY.ROUTE$ +                            \
                        CCTMP.DISP.LOCATION$ +                            \
                        CCTMP.BC.LETTER$ +                            \
                        CCTMP.RECALL.NUM$ +                            \
                        CCTMP.AUTHORISATION$ +                             \
                        CCTMP.SUPPLIER$ +                            \
                        CCTMP.RETURN.METHOD$ +                            \
                        CCTMP.CARRIER$ +                            \
                        CCTMP.BIRD.NUM$ +                            \
                        CCTMP.RETURN.REASON$ +                            \
                        CCTMP.WAREHOUSE.ROUTE$ +                     \
                        CCTMP.UOD.TYPE$ +                            \
                        CCTMP.DAMAGE.REASON$ +                            \
                        CCTMP.RECEIVING.STORE$ +                      \
                        CCTMP.DESTINATION$ +                            \
                        CCTMP.DATE.UOD.OPENED$ +                      \
                        CCTMP.DATE.DESPATCHED$ +                     \
                        CCTMP.TIME.DESPATCHED$ +                     \
                        CCTMP.RECALL.TYPE$                          ! 1.20BG
           CCTMP.RECORD.COMPLETE = FALSE
         ENDIF
      ENDIF ELSE BEGIN
           ! Credit Claim transmission
           CCTMP.TOTAL.QTY.IN.UOD$ = MID$(DATA.IN$,31,4)             ! 1.16JAS
           CCTMP.ADJ.STOCK.FIGURE$ = MID$(DATA.IN$,35,1)             ! 1.16JAS
           CCTMP.DATE.DESPATCHED$ = PACK$(FN.INVERT.DATE(MID$(DATA.IN$,42,6))) ! 1.16JAS
           CCTMP.TIME.DESPATCHED$ = PACK$(MID$(DATA.IN$,48,6))       ! 1.16JAS
           CCTMP.RETURN.REASON$ = PACK$(MID$(DATA.IN$,105,2))        ! 1.16JAS

           EXTRA.ITEMS% = 0                                          ! 1.17JAS

           TEMP.RECORD$  = CCTMP.RECORD.DELIMITER$ +                     \ ! 1.16JAS
                           PACK$("25")             +                     \ ! 1.16JAS
                           CCTMP.FIELD.DELIMITER$  +                     \ ! 1.16JAS
                           CCTMP.DATE.DESPATCHED$  +                     \ ! 1.16JAS
                           CCTMP.TIME.DESPATCHED$  +                     \ ! 1.16JAS
                           CCTMP.CREDIT.CLAIM.NUM$ +                     \ ! 1.16JAS
                           CCTMP.TOTAL.QTY.IN.UOD$ +                     \ ! 1.16JAS
                           STRING$(15, " ")        +                     \ ! 1.16JAS
                           CCTMP.ADJ.STOCK.FIGURE$ +                     \ ! 1.16JAS
                           CCTMP.RETURN.REASON$                            ! 1.16JAS
           CCTMP.RECORD.COMPLETE = FALSE                                   ! 1.16JAS
      ENDIF                                                                ! 1.16JAS

    RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.ITEM.DETAIL:                                        ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     transfer record information to the CCTMP record in memory          ***
\***     increment number of UOD item received                              ***
\***     If LDT.RETURNS.TYPE$ = "O" Credit Claim                            ***
\***        Open the IRF and IDF                                            ***
\***        Depending on Item/Barcode flag either validate Item or Barcode  ***
\***        If valid item add item to valid table                           ***
\***        else add item to reject table                                   ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.RETURNS.ITEM.DETAIL:

       IF FN.VALIDATE.DATA(DATA.IN$,37) = 0 THEN BEGIN
          RECEIVE.STATE$ = "*"
          GOTO MODULE.EXIT
       ENDIF

       IF (NOT CCTMP.RECORD.COMPLETE) THEN BEGIN
          CCTMP.ITEM.UPDATED$ = MID$(DATA.IN$,3,1)
          CCTMP.ITEM.BARCODE.FLAG$ = MID$(DATA.IN$,4,1)
          CCTMP.ITEM.BARCODE$ = PACK$(MID$(DATA.IN$,6,12))
          CCTMP.ITEM.QTY$ = MID$(DATA.IN$,18,4)

          CURR.SESS.NUM% = IDF.SESS.NUM%                              ! 1.16JAS
          IF IDF.OPEN.FLAG$ = "Y" THEN BEGIN                          ! 1.16JAS
           IDF.OPEN.FLAG$ = "N"                                       ! 1.16JAS
            CLOSE IDF.SESS.NUM%                                       ! 1.16JAS
          ENDIF                                                       ! 1.16JAS
          IF END #IDF.SESS.NUM% THEN OPEN.ERROR                       ! 1.16JAS
          OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%   ! 1.16JAS                                     ! MMJK
          IDF.OPEN.FLAG$ = "Y"                                        ! 1.16JAS

          IF END# IRF.SESS.NUM% THEN OPEN.ERROR                       ! 1.16JAS
          CURR.SESS.NUM% = IRF.SESS.NUM%                              ! 1.16JAS
          OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%   \ 1.16JAS
                  NOWRITE NODEL                                       ! 1.16JAS

         IF CCTMP.ITEM.BARCODE.FLAG$ = "I" THEN BEGIN                 ! 1.16JAS
            GOSUB CHECK.VALID.ITEM                                    ! 1.16JAS
            IF VALID.ENTRY% THEN BEGIN                                ! 1.16JAS
               GOSUB ADD.ITEM.TO.VALID.TABLE                          ! 1.16JAS
            ENDIF ELSE BEGIN                                          ! 1.16JAS
               GOSUB ADD.ITEM.TO.REJECT.TABLE                         ! 1.16JAS
            ENDIF                                                     ! 1.16JAS
         ENDIF ELSE BEGIN                                             ! 1.16JAS
            GOSUB CHECK.VALID.BARCODE                                 ! 1.16JAS
            IF VALID.ENTRY% THEN BEGIN                                ! 1.16JAS
               CCTMP.ITEM.BARCODE$ = PACK$(RIGHT$("0000"   +          \ 1.16JAS
                                     BOOTS.CODE$,12))                 ! 1.16JAS
               GOSUB ADD.ITEM.TO.VALID.TABLE                          ! 1.16JAS
            ENDIF ELSE BEGIN                                          ! 1.16JAS
               GOSUB ADD.ITEM.TO.REJECT.TABLE                         ! 1.16JAS
            ENDIF                                                     ! 1.16JAS
         ENDIF                                                        ! 1.16JAS
         ACTUAL.UOD.ITEMS% = ACTUAL.UOD.ITEMS% + 1                    ! 1.16JAS
       ENDIF

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.UOD.TRAILER:                                        ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***        If no valid items to process                                    ***
\***           Set CCTMP.RECORD.COMPLETE to TRUE                            ***
\***        ELSE                                                            ***
\***          unload active table                                           ***
\***        endif                                                           ***
\***        If rejected items to process                                    ***
\***           print rejection report                                       ***
\***        endif                                                           ***
\***                                                                        ***
\***     compare the number of items expected on the trailer to actual      ***
\***     number received and if a discrepancy has occurred;                 ***
\***       log an event 92 detailing discrepancy                            ***
\***       abort transmission                                               ***
\***                                                                        ***
\***     write record to CCTMP and increment total UODs counter             ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.RETURNS.UOD.TRAILER:

       IF FN.VALIDATE.DATA(DATA.IN$,38) = 0 THEN BEGIN
          RECEIVE.STATE$ = "*"
          GOTO MODULE.EXIT
       ENDIF

       ! If no items to process set record complete to true
       ! else unload active table
       ! If reject items to process - print rejection report
       IF VALID.INDEX% = 0 THEN BEGIN                               ! 1.16JAS
          CCTMP.RECORD.COMPLETE = TRUE                              ! 1.16JAS
       ENDIF ELSE BEGIN                                             ! 1.16JAS
          GOSUB UNLOAD.ACTIVE.TABLE                                 ! 1.16JAS
       ENDIF                                                        ! 1.16JAS

       IF (NOT CCTMP.RECORD.COMPLETE) THEN BEGIN                       ! BMJK
          CCTMP.RECORD$ = CCTMP.RECORD$ +                              \ BMJK
                          CCTMP.RECORD.DELIMITER$ +                    \ BMJK
                          CCTMP.ENDREC.MARKER$                         ! BMJK
          IF ACTUAL.UOD.ITEMS% <> CCTMP.NUM.OF.ITEMS% THEN BEGIN       ! BMJK
             CURR.SESS.NUM% = CCTMP.SESS.NUM%                          ! BMJK
             CURRENT.KEY$ = ""                                         ! BMJK
             IF WRITE.CCTMP THEN GOTO WRITE.ERROR                      ! BMJK
             FILE.NUM% = 244                                           ! BMJK
             EXPECTED.NUM% = CCTMP.NUM.OF.ITEMS%                       ! BMJK
             ACTUAL.NUM% = ACTUAL.UOD.ITEMS%                           ! BMJK
             GOSUB LOG.AN.EVENT.92                                     ! BMJK
             RECEIVE.STATE$ = "*"                                      ! BMJK
             GOTO MODULE.EXIT                                          ! BMJK
          ENDIF                                                        ! BMJK
       ENDIF                                                           ! BMJK

\*** Code for assigning claim number and writing to CCBUF here now     ! BMJK
\*** redundant due to changes to include CCUPF interrogation in the    ! BMJK
\*** transfer of the CCTMP to the STKMQ                                ! BMJK

       IF  VALID.INDEX% > 0 THEN BEGIN                                 ! 1.18JAS
             CURR.SESS.NUM% = CCTMP.SESS.NUM%
             CURRENT.KEY$ = ""
             IF WRITE.CCTMP THEN GOTO WRITE.ERROR
       ENDIF                                                           ! 1.16JAS

       ACTUAL.NUM.OF.UODS% = ACTUAL.NUM.OF.UODS% + 1

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.FILE.TRAILER:                                       ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     compare the number of UODs received with the expected number       ***
\***     received and if the figures differ;                                ***
\***       log an event 92 and abort transmission                           ***
\***                                                                        ***
\***     Check for existence of CCUPF and STKMQ by opening them             ***
\***                                                                        ***
\***         extract last claim number from CCTRL                           ***
\***                                                                        ***
\***         create and send a File received OK record to LDT that          ***
\***           contains the last issued credit claim number                 ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.RETURNS.FILE.TRAILER:

       SB.MESSAGE$ = "PDT Support - RETURNS file trailer received"
       GOSUB SB.BG.MESSAGE

       IF FN.VALIDATE.DATA(DATA.IN$,39) = 0 THEN BEGIN
          RECEIVE.STATE$ = "*"
          GOTO MODULE.EXIT
       ENDIF

       IF ACTUAL.NUM.OF.UODS% <> EXPECTED.NUM.OF.UODS% THEN BEGIN
          FILE.NUM% = 244
          EXPECTED.NUM% = EXPECTED.NUM.OF.UODS%
          ACTUAL.NUM% = ACTUAL.NUM.OF.UODS%
          GOSUB LOG.AN.EVENT.92
          RECEIVE.STATE$ = "*"
          GOTO MODULE.EXIT
       ENDIF


       CURR.SESS.NUM% = CCUPF.SESS.NUM%                                ! CNS
       IF END #CCUPF.SESS.NUM% THEN OPEN.ERROR                         ! CNS
       OPEN CCUPF.FILE.NAME$ KEYED RECL CCUPF.RECL% AS CCUPF.SESS.NUM% ! CNS
       CCUPF.OPEN.FLAG$ = "Y"                                          ! CNS

       IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                           ! CNS
          IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                      ! CNS
          CURR.SESS.NUM% = STKMQ.SESS.NUM%                             ! CNS
          OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512        \ CNS
             READONLY APPEND                                           ! CNS
          STKMQ.OPEN.FLAG$ = "Y"                                       ! CNS
       ENDIF                                                           ! CNS

       CLOSE CCUPF.SESS.NUM%                                           ! CNS
       CCUPF.OPEN.FLAG$ = "N"                                          ! CNS

       CLOSE STKMQ.SESS.NUM%                                           ! CNS
       STKMQ.OPEN.FLAG$ = "N"                                          ! CNS


       CURR.SESS.NUM% = CCTRL.SESS.NUM%
       IF END #CCTRL.SESS.NUM% THEN OPEN.ERROR
       OPEN CCTRL.FILE.NAME$ DIRECT RECL CCTRL.RECL% AS CCTRL.SESS.NUM%        ! BMJK
       CCTRL.OPEN.FLAG$ = "Y"
       CCTRL.REC.NUM% = 1
       CURRENT.KEY$ = "1"
       IF READ.CCTRL THEN GOTO READ.ERROR
       CCTRL.OPEN.FLAG$ = "N"
       CLOSE CCTRL.SESS.NUM%

       PIPE.OUT$ = "L" +                                               \
                   "RO" +                                              \
                   FN.Z.PACK(UNPACK$(CCTRL.CREDIT.CLAIM.NUM$),8)
       GOSUB SEND.TO.PSS38

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.FILE.REQUEST:                                       ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     issue hold message to LDT                                          ***
\***                                                                        ***
\***     check file request flag on LDT record                              ***
\***     record information on the audit file                               ***
\***                                                                        ***
\***     if file request is yes; then call routine to extract CCUOD keys    ***
\***        AND CCLAM keys.                                                 ***
\***                                                                        ***
\***     create empty CCWKF and write header with number of UODs to CCWKF   ***
\***                                                                        ***
\***     if file request is yes and CCUOD contains UODs to transmit         ***
\***        OR CCLAM contains despatched UODs to transmit:                  ***
\***       call routine to extract all CCUOD records and write to CCWKF     ***
\***                                                                        ***
\***     call routine to extract all business centres and write to CCWKF    ***
\***                                                                        ***
\***     write file trailer record to CCWKF and close file                  ***
\***                                                                        ***
\***     release LDT                                                        ***
\***     store transmission details on LDTAF                                ***
\***     instruct PSS38 to send CCWKF to LDT                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

    RECEIVED.RETURNS.FILE.REQUEST:

      SB.MESSAGE$ = "PDT Support - RETURNS file request received"
      GOSUB SB.BG.MESSAGE
      IF FN.VALIDATE.DATA(DATA.IN$,40) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         GOTO MODULE.EXIT
      ENDIF

      GOSUB HOLD.LDT

      GOSUB CREATE.CCTMP                                          !ADS

      DATA.FILES.REQ$ = MID$(DATA.IN$,3,1)
      CSR.AUDIT.DATA$ = "RETURNS file request; "
      IF DATA.FILES.REQ$ = "Y" THEN CSR.AUDIT.DATA$ =                     \
        CSR.AUDIT.DATA$ + "Data file update required."                     \
      ELSE CSR.AUDIT.DATA$ =                                           \
        CSR.AUDIT.DATA$ + "Business Centre update only."
      GOSUB LOG.TO.AUDIT.FILE

      DIM BARCODE.TABLE$(5000),BARCODE.POS.TABLE$(5000)

      NUM.STATUS.O.C.X.UODS% = 0                                         ! 1.4
      NUM.OF.CANCELLED.UODS% = 0                                         ! 1.4
      NUM.OF.DESPATCHED.UODS% = 0                                        ! 1.4
      IF DATA.FILES.REQ$ = "Y" THEN BEGIN                                ! 1.4
         GOSUB DIRECT.EXTRACT.CCUOD.KEYS
         GOSUB DIRECT.EXTRACT.CCLAM.KEYS                                 ! 1.4
      ENDIF                                                              ! 1.4

      GOSUB CREATE.CCWKF.AND.WRITE.HEADER

      IF (NUM.STATUS.O.C.X.UODS%>0 OR NUM.OF.DESPATCHED.UODS%>0) AND     \ 1.4
         (DATA.FILES.REQ$="Y") THEN BEGIN                                ! 1.4
         GOSUB PROCESS.UOD.RECORDS                                       ! 1.4
      ENDIF

      GOSUB EXTRACT.ALL.BUSINESS.CENTRES
      GOSUB WRITE.CCWKF.TRAILER.AND.CLOSE.FILE

      LDTAF.LINK.TYPE% = 9
      IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                      \
         (LDTAF.LINK.TYPE% OR 80H)
      LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)
      LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)
      TEMP.TIME$ = TIME$                                                ! BMJK
      LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +              \ BMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +               \ BMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ BMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ BMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+       \ BMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )       ! BMJK
      LDTAF.DATA.VOLUME.1% = 0
      LDTAF.DATA.VOLUME.2% = 0
      LDTAF.DATA.VOLUME.1% = SIZE (CCWKF.FILE.NAME$)
      GOSUB LOG.TO.LDTAF.FILE

      GOSUB RELEASE.LDT                                                 ! BMJK

      GOSUB TRANSMIT.CCWKF.VIA.PSS38

    RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.FILE.RECEIVED.OK:                                   ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

    RECEIVED.RETURNS.FILE.RECEIVED.OK:

      SB.MESSAGE$ = "PDT Support - RETURNS file received OK"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$,41) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        GOTO MODULE.EXIT
      ENDIF

    RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.RETURNS.EOT:                                                ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     If records on CCREJ adxstart PSS25                                 ***
\***                                                                        ***
\***     If records on CCBUF and transmission was from Goods out adxstart   ***
\***     PSS22                                                              ***
\***     record end of session on audit file                                ***
\***     set flags to allow PSS37 to rechain                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.RETURNS.EOT:

      SB.MESSAGE$ = "PDT Support - RETURNS EOT received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$,42) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        GOTO MODULE.EXIT
      ENDIF

      CURR.SESS.NUM% = CCREJ.SESS.NUM%                                 ! 1.18JAS
      IF CCREJ.OPEN.FLAG$ = "Y" THEN BEGIN                             ! 1.18JAS
         CCREJ.OPEN.FLAG$ = "N"                                        ! 1.18JAS
         CLOSE CCREJ.SESS.NUM%                                         ! 1.18JAS
      ENDIF                                                            ! 1.18JAS

      IF CCREJ.RECS.PRESENT AND SIZE(CCREJ.FILE.NAME$) > 0 THEN BEGIN  ! 1.18JAS
         ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS25.286",       \        ! 1.18JAS
                         MONITORED.PORT$,                     \        ! 1.18JAS
                       "PSS25 - Credit Claiming Rejection reporting")  ! 1.18JAS
      ENDIF                                                            ! 1.18JAS

! The following lines moved from Received Returns File Trailer routine - CNS
      CURR.SESS.NUM% = CCTMP.SESS.NUM%                                 ! CNS
      IF CCTMP.OPEN.FLAG$ = "Y" THEN BEGIN                             ! CNS
         CCTMP.OPEN.FLAG$ = "N"                                        ! CNS
         CLOSE CCTMP.SESS.NUM%                                         ! CNS
      ENDIF                                                            ! CNS
      IF END #CCTMP.SESS.NUM% THEN OPEN.ERROR                          ! CNS
      OPEN CCTMP.FILE.NAME$ AS CCTMP.SESS.NUM% LOCKED                  ! CNS
      CCTMP.OPEN.FLAG$ = "Y"                                           ! CNS

      LDTAF.DATA.VOLUME.1% = 0                                         ! CNS
      LDTAF.DATA.VOLUME.1% = SIZE (CCTMP.FILE.NAME$)                   ! CNS
      IF LDTAF.DATA.VOLUME.1% <> 0 THEN BEGIN                          ! CNS
         GOSUB WRITE.STKMQ.FROM.CCTMP                                  ! CNS
         GOSUB CREATE.CCTMP                                            ! CNS
      ENDIF                                                            ! CNS


      LDTAF.DATA.VOLUME.2% = 0
      LDTAF.DATA.VOLUME.2% = SIZE (CCBUF.FILE.NAME$)
      LDTAF.LINK.TYPE% = 8
      IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                      \
         (LDTAF.LINK.TYPE% OR 80H)
      LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)
      LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)
      TEMP.TIME$ = TIME$                                               ! BMJK
      LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +              \ BMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +               \ BMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ BMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ BMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+       \ BMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )       ! BMJK
      GOSUB LOG.TO.LDTAF.FILE

      !If records on CCBUF file and transmission was for Goods out Start PSS22 to print Advice of UOD contents report
      IF CCBUF.RECS.PRESENT AND                                             \ ! 1.16JAS
         LDT.RETURNS.TYPE$ = "G" THEN BEGIN                                   ! 1.16JAS
         ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS22.286",                     \
                         MONITORED.PORT$,                            \
                       "PSS22 - Advice of UOD contents reporting")
      ENDIF

      TEMP.TIME$ = TIME$
      CSR.AUDIT.DATA$ = "RETURNS session complete at " +                \
                         LEFT$(TEMP.TIME$,2) + ":" +                    \ BMJK
                      MID$(TEMP.TIME$,3,2) + ":" +                   \ BMJK
                      RIGHT$(TEMP.TIME$,2)                           ! BMJK
      GOSUB LOG.TO.AUDIT.FILE

      IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN
        CLOSE CSR.AUDIT.SESS.NUM%
        CSR.AUDIT.OPEN.FLAG$ = "N"
      ENDIF

      RE.CHAIN = TRUE
      RECEIVE.STATE$ = "?"

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.STOCKTAKE.HEADER - Added for version 1.5                    ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     Create temprorary buffer file                                      ***
\***     Write header to file                                               ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.STOCKTAKE.HEADER:

      SB.MESSAGE$ = "PDT Support - Stocktake hdr received"
      GOSUB SB.BG.MESSAGE

      LAST.STOCKTAKE.DATE$ = DATE$
      LAST.STOCKTAKE% = FN.SECONDS(TIME$)

      IF FN.VALIDATE.DATA(DATA.IN$,43) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        GOTO MODULE.EXIT
      ENDIF

      TEMP.TIME$ = TIME$
      CSR.AUDIT.DATA$ = "Stocktake session started at " +            \
                         LEFT$(TEMP.TIME$,2) + ":" +                 \
                      MID$(TEMP.TIME$,3,2) + ":" +                   \
                      RIGHT$(TEMP.TIME$,2)
      GOSUB LOG.TO.AUDIT.FILE

      IF SXTMP.OPEN.FLAG$ = "Y" THEN BEGIN                             ! 1.6
         CLOSE SXTMP.SESS.NUM%                                         ! 1.6
         SXTMP.OPEN.FLAG$ = "N"                                        ! 1.6
      ENDIF

      IF STLDT.OPEN.FLAG$ = "Y" THEN BEGIN                             ! 1.11
         CLOSE STLDT.SESS.NUM%                                         ! 1.11
         STLDT.OPEN.FLAG$ = "N"                                        ! 1.11
      ENDIF                                                            ! 1.11

      CURR.SESS.NUM% = SXTMP.SESS.NUM%
      IF END# SXTMP.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE SXTMP.FILE.NAME$                                  \ 1.6
            AS SXTMP.SESS.NUM% LOCKED MIRRORED PERUPDATE
      SXTMP.OPEN.FLAG$ = "Y"

      CURR.SESS.NUM% = STLDT.SESS.NUM%                                 ! 1.11
      IF END# STLDT.SESS.NUM% THEN CREATE.STLDT                        ! 1.13DA
      OPEN STLDT.FILE.NAME$ DIRECT RECL STLDT.RECL% AS STLDT.SESS.NUM% ! 1.11
      GOTO SKIP.CREATE.STLDT                                           ! 1.13DA
   CREATE.STLDT:                                                       ! 1.13DA
      CREATE STLDT.FILE.NAME$ DIRECT RECL STLDT.RECL% AS               \ 1.13DA
             STLDT.SESS.NUM%                                           ! 1.13DA
   SKIP.CREATE.STLDT:                                                  ! 1.13DA
      STLDT.OPEN.FLAG$ = "Y"                                           ! 1.11
      IF SIZE(STLDT.FILE.NAME$) = 0 THEN BEGIN                         ! 1.11
         STLDT.RECORD.NUM% = 1                                         ! 1.11
      ENDIF ELSE BEGIN                                                 ! 1.11
         STLDT.RECORD.NUM% = (SIZE(STLDT.FILE.NAME$) / STLDT.RECL%) + 1! 1.11
      ENDIF                                                            ! 1.11

      STOCKTAKE.COUNT = 1
      EXPECTED.RECORDS = VAL(MID$(DATA.IN$,9,6))

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.STOCKTAKE.DETAIL - Added for version 1.5                    ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     Write record to buffer file                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.STOCKTAKE.DETAIL:


      IF FN.VALIDATE.DATA(DATA.IN$,44) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        GOTO MODULE.EXIT
      ENDIF

      LAST.STOCKTAKE.DATE$ = DATE$
      LAST.STOCKTAKE% = FN.SECONDS(TIME$)

      IF STOCKTAKE.COUNT = 1 THEN BEGIN
        SXTMP.DATA$ = MID$(DATA.IN$,3,14) + "                " + CHR$(13) ! 1.9
        STLDT.STORE$ = LEFT$(SXTMP.DATA$,4)                               ! 1.11
        STLDT.STOCKTAKER$ = MID$(SXTMP.DATA$,5,4)                         ! 1.11
        STLDT.DATE$ = MID$(SXTMP.DATA$,9,6)                               ! 1.11
        STLDT.TIME$ = LEFT$(TIME$,4)                                      ! 1.11
      ENDIF ELSE BEGIN
        IF STOCKTAKE.COUNT = EXPECTED.RECORDS THEN BEGIN
          SXTMP.DATA$ = MID$(DATA.IN$,3,6) + "                        " + \ 1.9
                        CHR$(13)
          STLDT.LDT.SERIAL$ = CURR.TERMINAL$                              ! 1.12
          RC% = WRITE.STLDT                                               ! 1.12
          IF RC% <> 0 THEN GOTO WRITE.ERROR                               ! 1.12
          STLDT.RECORD.NUM% = STLDT.RECORD.NUM% + 1                       ! 1.12

        ENDIF ELSE BEGIN
          SXTMP.DATA$ = MID$(DATA.IN$,3,5) + MID$(DATA.IN$,8,13) +       \ 1.9
            MID$(DATA.IN$,21,6) + MID$(DATA.IN$,27,6) +                  \ 1.9
            CHR$(13)
          STLDT.COUNTS% = STLDT.COUNTS% + VAL(MID$(DATA.IN$,21,6))       ! 1.15DA
          STLDT.ITEMS% = STLDT.ITEMS% + 1                                ! 1.11
        ENDIF
      ENDIF

      RC% = WRITE.SXTMP
      IF RC% <> 0 THEN GOTO WRITE.ERROR
      STOCKTAKE.COUNT = STOCKTAKE.COUNT + 1

RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   RECEIVED.STOCKTAKE.TRAILER - Added for version 1.5                   ***
\***                                                                        ***
\***     log message to background screen to indicate record received       ***
\***     validate record layout; return if invalid                          ***
\***                                                                        ***
\***     Write record to buffer file                                        ***
\***     Close buffer file                                                  ***
\***                                                                        ***
\***     Send acknowledgement to PDT                                        ***
\***     Copy temporary buffer file to stocktake buffer file                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

RECEIVED.STOCKTAKE.TRAILER:

      SB.MESSAGE$ = "PDT Support - Stocktake trlr received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$,45) = 0 THEN BEGIN
        RECEIVE.STATE$ = "*"
        GOTO MODULE.EXIT
      ENDIF

      LAST.STOCKTAKE.DATE$ = DATE$
      LAST.STOCKTAKE% = FN.SECONDS(TIME$)

      IF STOCKTAKE.COUNT <> EXPECTED.RECORDS + 1 THEN BEGIN
          FILE.NUM% = 244
          EXPECTED.NUM% = EXPECTED.RECORDS
          ACTUAL.NUM% = STOCKTAKE.COUNT + 1
          GOSUB LOG.AN.EVENT.92
          RECEIVE.STATE$ = "*"
          GOTO MODULE.EXIT
      ENDIF

      T.DATE$ = DATE$                                                  ! 1.6
      T.TIME$ = TIME$                                                  ! 1.6

      PIPE.OUT$ = "L" +                                                \ 1.6
                  SOH$ +                                               \ 1.6
                  FN.Z.PACK(STORE.NUMBER$, 4) +                        \ 1.6
                  FN.Z.PACK(CURR.TERMINAL$, 6) +                       \ 1.6
                  APPLICATION.NO$ +                                    \ 1.6
                  RIGHT$(T.DATE$,2)+MID$(T.DATE$,3,2)+LEFT$(T.DATE$,2) + \ 1.6
                  LEFT$(T.TIME$,4) +                                   \ 1.6
                  ACK$                                                 ! 1.6

      IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR                         ! 1.6
      CURR.SESS.NUM% = PIPEI.SESS.NUM%                                 ! 1.6
      WRITE# PIPEI.SESS.NUM%; PIPE.OUT$                                ! 1.6


       IF STKBF.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.6
          CLOSE STKBF.SESS.NUM%                                        ! 1.6
          STKBF.OPEN.FLAG$ = "N"                                       ! 1.6
       ENDIF                                                           ! 1.6

       IF STKMF.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.13DA
          CLOSE STKMF.SESS.NUM%                                        ! 1.13DA
          STKMF.OPEN.FLAG$ = "N"                                       ! 1.13DA
       ENDIF                                                           ! 1.13DA

       IF END# STKBF.SESS.NUM% THEN OPEN.ERROR
       CURR.SESS.NUM% = STKBF.SESS.NUM%
       OPEN STKBF.FILE.NAME$ AS STKBF.SESS.NUM% BUFFSIZE MAX.REC.SIZE% \ 1.6
          READONLY APPEND
       STKBF.OPEN.FLAG$ = "Y"

       IF END# LOCCNT.SESS.NUM% THEN OPEN.ERROR                         ! 1.14DA
       CURR.SESS.NUM% = LOCCNT.SESS.NUM%                                ! 1.14DA
       OPEN LOCCNT.FILE.NAME$ DIRECT RECL LOCCNT.RECL% AS               \ 1.14DA
            LOCCNT.SESS.NUM% NODEL                                      ! 1.14DA
       LOCCNT.OPEN.FLAG$ = "Y"                                          ! 1.14DA
       LOCCNT.RECORD.NUM% = (SIZE(LOCCNT.FILE.NAME$) / LOCCNT.RECL%)    ! 1.14DA

       IF STOCKTAKING.ALTERNATIVE.STORE = TRUE THEN BEGIN               ! 1.13DA

       IF END# STKMF.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKMF.SESS.NUM%                                 ! 1.13DA
       OPEN STKMF.FILE.NAME$ DIRECT RECL STKMF.RECL% AS                 \ 1.13DA
            STKMF.SESS.NUM% NODEL                                       ! 1.13DA
       STKMF.OPEN.FLAG$ = "Y"                                           ! 1.13DA
       IF SIZE(STKMF.FILE.NAME$) = STKMF.RECL% THEN BEGIN               ! 1.13DA
          STKMF.RECORD.NUM% = 1                                         ! 1.13DA
       ENDIF ELSE BEGIN                                                 ! 1.13DA
          STKMF.RECORD.NUM% = (SIZE(STKMF.FILE.NAME$) / STKMF.RECL%) + 1! 1.13DA
       ENDIF                                                            ! 1.13DA

!      IF END# STKCF.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
!      CURR.SESS.NUM% = STKCF.SESS.NUM%                                 ! 1.13DA
!      OPEN STKCF.FILE.NAME$ DIRECT RECL STKCF.RECL% AS                 \ 1.13DA
!           STKCF.SESS.NUM% NODEL                                       ! 1.13DA
!      STKCF.OPEN.FLAG$ = "Y"                                           ! 1.13DA

       IF END# STKRC.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKRC.SESS.NUM%                                 ! 1.13DA
       OPEN STKRC.FILE.NAME$ DIRECT RECL STKRC.RECL% AS STKRC.SESS.NUM% \ 1.13DA
               NODEL                                                    ! 1.13DA
       STKRC.OPEN.FLAG$ = "Y"                                           ! 1.13DA
       STKRC.RECORD.NUM% = (SIZE(STKRC.FILE.NAME$) / STKRC.RECL%)       ! 1.13DA

       IF END# XGCF.SESS.NUM% THEN OPEN.ERROR                           ! 1.13DA
       CURR.SESS.NUM% = XGCF.SESS.NUM%                                  ! 1.13DA
       OPEN XGCF.FILE.NAME$ KEYED RECL XGCF.RECL% AS XGCF.SESS.NUM%     \ 1.13DA
       UNLOCKED NOWRITE NODEL                                           ! 1.13DA
       XGCF.OPEN.FLAG$ = "Y"                                            ! 1.13DA

       IF END# STKEX.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKEX.SESS.NUM%                                 ! 1.13DA
       OPEN STKEX.FILE.NAME$ DIRECT RECL STKEX.RECL% AS STKEX.SESS.NUM% \ 1.13DA
               NODEL                                                    ! 1.13DA
       STKEX.OPEN.FLAG$ = "Y"                                           ! 1.13DA
       STKEX.RECORD.NUM% = (SIZE(STKEX.FILE.NAME$) / STKEX.RECL%)       ! 1.13DA

       IF END# STKDC.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKDC.SESS.NUM%                                 ! 1.13DA
       OPEN STKDC.FILE.NAME$ DIRECT RECL STKDC.RECL% AS STKDC.SESS.NUM% \ 1.13DA
               NODEL                                                    ! 1.13DA
       STKDC.OPEN.FLAG$ = "Y"                                           ! 1.13DA
       STKDC.RECORD.NUM% = (SIZE(STKDC.FILE.NAME$) / STKDC.RECL%)       ! 1.13DA

       IF END# STKIF.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKIF.SESS.NUM%                                 ! 1.13DA
       OPEN STKIF.FILE.NAME$ KEYED RECL STKIF.RECL% AS STKIF.SESS.NUM%  ! 1.13DA
       STKIF.OPEN.FLAG$ = "Y"                                           ! 1.13DA

       IF END# STKTF.SESS.NUM% THEN OPEN.ERROR                          ! 1.13DA
       CURR.SESS.NUM% = STKTF.SESS.NUM%                                 ! 1.13DA
       OPEN STKTF.FILE.NAME$ KEYED RECL STKTF.RECL% AS STKTF.SESS.NUM%  ! 1.13DA
       STKTF.OPEN.FLAG$ = "Y"                                           ! 1.13DA

       IF END# IDF.SESS.NUM% THEN OPEN.ERROR                            ! 1.13DA
       CURR.SESS.NUM% = IDF.SESS.NUM%                                   ! 1.13DA
       OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%        \ 1.13DA
               NOWRITE NODEL                                            ! 1.13DA

       ENDIF

       IF SXTMP.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.6
          CLOSE SXTMP.SESS.NUM%                                        ! 1.6
          SXTMP.OPEN.FLAG$ = "N"                                       ! 1.6
       ENDIF                                                           ! 1.6

       IF STLDT.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.11
          CLOSE STLDT.SESS.NUM%                                        ! 1.11
          STLDT.OPEN.FLAG$ = "N"                                       ! 1.11
       ENDIF                                                           ! 1.11

       IF END# SXTMP.SESS.NUM% THEN OPEN.ERROR                         ! 1.6
       CURR.SESS.NUM% = SXTMP.SESS.NUM%                                ! 1.6
       OPEN SXTMP.FILE.NAME$ AS SXTMP.SESS.NUM% BUFFSIZE MAX.REC.SIZE% \ 1.6
          NOWRITE NODEL                                                ! 1.6
       SXTMP.OPEN.FLAG$ = "Y"                                          ! 1.6

       MAX.REC.SIZE% = 32767                                           ! 1.6
       INPUT.SIZE% = SIZE(SXTMP.FILE.NAME$)                            ! 1.6

       IF INPUT.SIZE% < MAX.REC.SIZE% THEN BEGIN                       ! 1.6
          RECORD.FORMAT$ = "C" + STR$(INPUT.SIZE%)                     ! 1.6
       ENDIF ELSE BEGIN                                                ! 1.6
          RECORD.FORMAT$ = "C" + STR$(MAX.REC.SIZE%)                   ! 1.6
       ENDIF

       NO.MAX.RECS% = 1                                                ! 1.6
       IF INPUT.SIZE% > MAX.REC.SIZE% THEN BEGIN                       ! 1.6
          REMAINDER% = MOD(INPUT.SIZE%, MAX.REC.SIZE%)                 ! 1.6
          NO.MAX.RECS% = INPUT.SIZE% / MAX.REC.SIZE%                   ! 1.6
       ENDIF

       GOSUB PREPARE.PROCESS                                           ! 1.13DA

       IF END #STKBF.SESS.NUM% THEN WRITE.ERROR                        ! 1.6
       IF END #SXTMP.SESS.NUM% THEN READ.ERROR                         ! 1.6
       FOR I% = 1 TO NO.MAX.RECS%                                      ! 1.6
          READ FORM RECORD.FORMAT$; #SXTMP.SESS.NUM%; DATA.BLOCK$      ! 1.6
          WRITE FORM RECORD.FORMAT$; #STKBF.SESS.NUM%; DATA.BLOCK$     ! 1.6
       NEXT I%

       IF INPUT.SIZE% > MAX.REC.SIZE% THEN BEGIN                       ! 1.6
          REMAINDER% = MOD(INPUT.SIZE%, MAX.REC.SIZE%)                 ! 1.6
          IF REMAINDER% > 0 THEN BEGIN                                 ! 1.6
             RECORD.FORMAT$ = "C" + STR$(REMAINDER%)                   ! 1.6
             READ FORM RECORD.FORMAT$; #SXTMP.SESS.NUM%; DATA.BLOCK$   ! 1.6
             WRITE FORM RECORD.FORMAT$; #STKBF.SESS.NUM%; DATA.BLOCK$  ! 1.6
          ENDIF                                                        ! 1.6
       ENDIF                                                           ! 1.6

       CLOSE STKBF.SESS.NUM%
       STKBF.OPEN.FLAG$ = "N"

       IF STOCKTAKING.ALTERNATIVE.STORE = TRUE THEN BEGIN              ! 1.13DA
          GOSUB READ.THRU.STKBF                                        ! 1.13DA
       ENDIF                                                           ! 1.13DA

       CLOSE STKMF.SESS.NUM%                                           ! 1.13DA
       STKMF.OPEN.FLAG$ = "N"                                          ! 1.13DA
       CLOSE SXTMP.SESS.NUM%
       SXTMP.OPEN.FLAG$ = "N"
       CLOSE STLDT.SESS.NUM%                                           ! 1.11
       STLDT.OPEN.FLAG$ = "N"                                          ! 1.11
       CLOSE SXTCF.SESS.NUM%                                           ! 1.13DA

       TEMP.TIME$ = TIME$
       CSR.AUDIT.DATA$ = "Stocktake session completed at " +          \
                         LEFT$(TEMP.TIME$,2) + ":" +                 \
                      MID$(TEMP.TIME$,3,2) + ":" +                   \
                      RIGHT$(TEMP.TIME$,2)
       GOSUB LOG.TO.AUDIT.FILE

       RE.CHAIN = TRUE
       RECEIVE.STATE$ = "?"

RETURN

READ.THRU.STKBF:

       IF END# STKBF.SESS.NUM% THEN OPEN.ERROR
       CURR.SESS.NUM% = STKBF.SESS.NUM%
       OPEN STKBF.FILE.NAME$ AS STKBF.SESS.NUM% BUFFSIZE MAX.REC.SIZE% \ 1.6
          READONLY APPEND
       STKBF.OPEN.FLAG$ = "Y"

     READ.STKBF.LOOP:
       RC% = READ.STKBF
       IF RC% = 0 THEN BEGIN
          GOSUB PROCESS.RECORD
          GOTO READ.STKBF.LOOP
       ENDIF

       CLOSE STKBF.SESS.NUM%
       STKBF.OPEN.FLAG$ = "N"

       CURR.SESS.NUM% = STKBF.SESS.NUM%
       IF END# STKBF.SESS.NUM% THEN CREATE.ERROR
       CREATE POSFILE STKBF.FILE.NAME$ AS STKBF.SESS.NUM% MIRRORED PERUPDATE

       CLOSE STKBF.SESS.NUM%
       STKBF.OPEN.FLAG$ = "N"

RETURN

\******************************************************************************
\***
\***   PREPARE.PROCESS:
\***
\***   Check that pointer to STKBF on SXTCF is valid.
\***   Point to current record
\***   Repeat
\***           Lock control file
\***           Read buffer file
\***           Process record
\***
\******************************************************************************

PREPARE.PROCESS:

       GOSUB GET.MATRIX                                                  ! 1.13DA

       IF END# SXTCF.SESS.NUM% THEN OPEN.ERROR                           ! 1.13DA
       CURR.SESS.NUM% = SXTCF.SESS.NUM%                                  ! 1.13DA
       OPEN SXTCF.FILE.NAME$ DIRECT RECL SXTCF.RECL% AS SXTCF.SESS.NUM%  \ 1.13DA
               NODEL                                                     ! 1.13DA

       IF END# IRF.SESS.NUM% THEN OPEN.ERROR                             ! 1.13DA
       CURR.SESS.NUM% = IRF.SESS.NUM%                                    ! 1.13DA
       OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%         \ 1.13DA
               NOWRITE NODEL                                             ! 1.13DA

       CURR.SESS.NUM% = SXTCF.SESS.NUM%
       RC% = READ.SXTCF
       IF RC% <> 0 THEN GOTO READ.ERROR

       !******************************************************************!1.3
       !* Initialise variable to hold currently processed location code   !1.3
       !******************************************************************!1.3
       CUR.LOC$ = SXTCF.CUR.LOCATION$                                     !1.3
       CUR.LOC.COUNTER% = 0                                               !1.3
       LOCCNT.RECORD.NUM% = GET.LOCCNT.RECNUM(CUR.LOC$)                   !1.3
       IF LOCCNT.RECORD.NUM% > 0 THEN BEGIN                               !1.3
          RC% = READ.LOCCNT                                               !1.3
          IF RC% = 0 THEN BEGIN                                           !1.3
             CUR.LOC.COUNTER% = LOCCNT.LOCATION.COUNT%                    !1.3
          ENDIF                                                           !1.3
       ENDIF ELSE BEGIN                                                   !1.3
          CUR.LOC$ = "     "                                              !1.3
       ENDIF                                                              !1.3

       IF SXTCF.STKBF.POINTER% > SIZE(STKBF.FILE.NAME$) THEN BEGIN
            SB.UNIQUE$ = "0000000000000000" + STR$(SXTCF.STKBF.POINTER%)
            SB.EVENT.NO% = 58
            GOSUB SB.LOG.AN.EVENT

            SXTCF.STKBF.POINTER% = 0
       ENDIF

       RC% = WRITE.SXTCF
       IF RC% <> 0 THEN GOTO WRITE.ERROR

       POINT STKBF.SESS.NUM%; SXTCF.STKBF.POINTER%

       HEADER.EXPECTED = SXTCF.HEADER.EXPECTED%
       IF SXTCF.STKBF.POINTER% = 0 THEN HEADER.EXPECTED = 1
       ITEM.COUNT = SXTCF.CURRENT.COUNT%
       STOCKTAKER$ = SXTCF.STOCKTAKER$
       STORE$ = "0000"

       CR$ = CHR$(13)

RETURN

\******************************************************************************
\***
\***   PROCESS.RECORD:
\***
\***   Check record type
\***
\******************************************************************************

PROCESS.RECORD:

     RC% = READ.SXTCF.LOCK
     IF RC% <> 0 THEN BEGIN
        CURR.SESS.NUM% = SXTCF.SESS.NUM%
        GOTO READ.ERROR
     ENDIF

     IF LEFT$(STKBF.DATA$,1) = "S" OR LEFT$(STKBF.DATA$,1) = "B" THEN BEGIN
       GOSUB PROCESS.ITEM
     ENDIF ELSE BEGIN
       IF LEFT$(STKBF.DATA$,4) = STORE.NUMBER$ THEN BEGIN
          GOSUB PROCESS.HEADER
       ENDIF ELSE BEGIN
          GOSUB PROCESS.TRAILER
       ENDIF
     ENDIF

RETURN

\******************************************************************************
\***
\***   PROCESS.HEADER:
\***
\***   If header expected Then
\***           Write record to Mainframe Transmission File
\***   Else
\***           Write record to Recount Audit Trail
\***
\***   RETURN
\***
\******************************************************************************
PROCESS.HEADER:

       IF HEADER.EXPECTED = 1 THEN BEGIN
            STKMF.DATA$ = LEFT$(STKBF.DATA$,8) + "                  " + CR$
            RC% = WRITE.STKMF
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKMF.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKMF.RECORD.NUM% = STKMF.RECORD.NUM% + 1
            STKMF.DATA$ = MID$(STKBF.DATA$,9,6) + "                    " + CR$
            RC% = WRITE.STKMF
            IF RC% <> 0 THEN GOTO WRITE.ERROR
            STKMF.RECORD.NUM% = STKMF.RECORD.NUM% + 1
            STORE$ = LEFT$(STKBF.DATA$,4)
            STOCKTAKER$ = MID$(STKBF.DATA$,5,4)
            HEADER.EXPECTED = 0
            ITEM.COUNT = 0
            BATCH.ITEMS = 0
       ENDIF ELSE BEGIN
            STKRC.ITEM.CODE$ = PACK$("00000000000000")
            STKRC.ITEM.DESCRIPTION$ = "Unexpected Header Record"
            STKRC.LOCATION$ = "X0000"                                  ! 1.3
            STKRC.INITIAL.QUANTITY$ = PACK$("000000")
            STKRC.INITIAL.STOCKTAKER$ = PACK$(MID$(STKBF.DATA$,5,4))
            STKRC.FINAL.QUANTITY$ = PACK$("000000")
            STKRC.FINAL.STOCKTAKER$ = PACK$("0000")
            RC% = WRITE.STKRC
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKRC.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKRC.RECORD.NUM% = STKRC.RECORD.NUM% + 1
       ENDIF




RETURN

\******************************************************************************
\***
\***   PROCESS.ITEM:
\***
\***   If header expected Then
\***           Write record to Recount Audit Trail
\***   Else
\***
\***           Check for Group Code
\***           If Group Code Then
\***                   Write record to Dump Code Audit File
\***                   Write record to Mainframe Transmission File
\***           Else
\***                   If Barcode Then
\***                      Read IRF to get Boots Code
\***                      If not on IRF Then
\***                           Write record to Exceptions File
\***                           Read STKIF using Barcode and Location
\***                           If found on STKIF Then
\***                              Write record to Recount Audit Trail
\***                              Write record to STKIF
\***                              Update Mainframe Transmission File
\***                           Else
\***                              Write record to Mainframe Transmission File
\***                              Create new STKIF record
\***                   If Boots Code Then
\***                      Format Boots Code
\***                      Read STKIF using Boots Code and Location
\***                      If found on STKIF Then
\***                           Write record to Recount Audit Trail
\***                           Write record to STKIF
\***                           Update Mainframe Transmission File
\***                           Read Stocktake Totals File
\***                           Add count difference to total on file
\***                           Write record to Stocktake Totals File
\***                      Else
\***                           Read IDF using Boots Code
\***                           If item on IDF
\***                              Check for within stocktake scope
\***                              If Not in scope Then
\***                                 Write record to Exceptions File
\***                              Else
\***                                 Write record to Mainframe Transmission File
\***                                 Write record to STKIF
\***                                 Check if item is on stock system
\***                                 If item on stock Then
\***                                    Read Stocktake Totals File
\***                                    If item found Then
\***                                       Add count to total on file
\***                                       Write record to Stocktake Totals File
\***                                    Else
\***                                       Create new record on Stocktake Totals File
\***                                 Else
\***                                    Lookup item on XGCF (Group Codes File)         !1.4
\***                                    If item on XGCF                                !1.4
\***                                       (same processing as for group code)         !1.4
\***                                       Write record to Dump Code Audit File        !1.4
\***                                       Write record to Mainframe Transmission File !1.4
\***                                    Else                                           !1.4
\***                                       Write record to Exceptions File
\***                           Else
\***                              Write record to Exceptions File
\***                              Write record to Mainframe Transmission File
\***                              Write record to STKIF
\***
\***
\***   RETURN
\***
\******************************************************************************
PROCESS.ITEM:

       IF HEADER.EXPECTED = 1 THEN BEGIN
            STKRC.ITEM.CODE$ = PACK$("0" + MID$(STKBF.DATA$,6,13))  ! 1.1 ! 1.3
            STKRC.ITEM.DESCRIPTION$ = "Header Record Missing   "
            STKRC.LOCATION$ = LEFT$(STKBF.DATA$,5)                  ! 1.3
            STKRC.INITIAL.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
            STKRC.INITIAL.STOCKTAKER$ = PACK$("0000")
            STKRC.FINAL.QUANTITY$ = PACK$("000000")
            STKRC.FINAL.STOCKTAKER$ = PACK$("0000")
            RC% = WRITE.STKRC
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKRC.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKRC.RECORD.NUM% = STKRC.RECORD.NUM% + 1
       ENDIF ELSE BEGIN
            !*************************************************************!1.3
            !* Process a location change if it is detected.               !1.3
            !*************************************************************!1.3
            IF CUR.LOC$ <> LEFT$(STKBF.DATA$,5) THEN BEGIN                !1.3
               NEW.LOC$ = LEFT$(STKBF.DATA$,5)                            !1.3
               GOSUB PROCESS.LOCATION.CHANGE                              !1.3
            ENDIF                                                         !1.3

            ITEM.COUNT = ITEM.COUNT + 1
            SXTCF.ITEM.COUNT% = SXTCF.ITEM.COUNT% + 1

            GOSUB CHECK.GROUP.CODE

            IF GROUP.CODE = 1 THEN BEGIN

               ! Group code
               GOSUB PROCESS.GROUP.CODE

            ENDIF ELSE BEGIN

               ! Not a group code (Barcode / Boots code)
               IF CURRENT.CODE = 0 THEN BEGIN
                  ! Bar code
                  IRF.BAR.CODE$ = MID$(STKBF.DATA$,6,12)               ! 1.3
                  IRF.BAR.CODE$ = PACK$(FN.ZERO.PL(IRF.BAR.CODE$,22))
                  RC% = READ.IRF
                  IF RC% <> 0 THEN BEGIN
                     STKEX.ITEM.CODE$ = PACK$("0" + MID$(STKBF.DATA$,6,13)) ! 1.3
                     STKEX.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6))  ! 1.1 ! 1.3
                     STKEX.LOCATION$ = LEFT$(STKBF.DATA$,5)            ! 1.3
                     STKEX.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                     STKEX.REASON.CODE$ = "I"
                     RC% = WRITE.STKEX
                     IF RC% <> 0 THEN BEGIN
                        CURR.SESS.NUM% = STKEX.SESS.NUM%
                        GOTO WRITE.ERROR
                     ENDIF
                     STKEX.RECORD.NUM% = STKEX.RECORD.NUM% + 1

                     STKIF.ITEM.CODE$ = PACK$("0" + MID$(STKBF.DATA$,6,13)) ! 1.3
                     STKIF.LOCATION$ = LEFT$(STKBF.DATA$,5)            ! 1.3
                     RC% = READ.STKIF
                     IF RC% = 0 THEN BEGIN
                        GOSUB ADD.TO.RECOUNT
                     ENDIF ELSE BEGIN
                        GOSUB ADD.TO.STKMF
                        BATCH.ITEMS = BATCH.ITEMS + 1
                        STKIF.BUSINESS.CENTRE$ = "X"
                        STKIF.GROUP.SEQUENCE$ = PACK$("000000")
                        STKIF.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                        STKIF.DESCRIPTION$ = "UNKNOWN ITEM            "
                        STKIF.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                        STKIF.STKMF.RECORD.NUM% = STKMF.RECORD.NUM% - 1
                        RC% = WRITE.STKIF
                        IF RC% <> 0 THEN BEGIN
                            CURR.SESS.NUM% = STKIF.SESS.NUM%
                            GOTO WRITE.ERROR
                        ENDIF
                     ENDIF

                  ENDIF ELSE BEGIN
                     ITEM.CODE$ = UNPACK$(IRF.BOOTS.CODE$)
                     CURRENT.CODE = 1
                  ENDIF
               ENDIF
               ! Could have a Boots code by here
               IF CURRENT.CODE = 1 THEN BEGIN
                  IF LEN(ITEM.CODE$) = 6 THEN BEGIN
                     SB.BOOTS.ITEM$ = ITEM.CODE$
                     GOSUB SB.CALC.BOOTS.CHECK.DIGIT
                     ITEM.CODE$ = SB.BOOTS.CODE$
                  ENDIF
                  IF LEN(ITEM.CODE$) = 7 THEN ITEM.CODE$ = "0" + ITEM.CODE$
                  STKIF.ITEM.CODE$ = PACK$("000000" + ITEM.CODE$)
                  STKIF.LOCATION$ = LEFT$(STKBF.DATA$,5)                ! 1.3

                  RC% = READ.STKIF
                  IF RC% = 0 THEN BEGIN
                     GOSUB ADD.TO.RECOUNT

                     STKTF.BOOTS.CODE$ = PACK$(ITEM.CODE$)
                     RC% = READ.STKTF
                     IF RC% = 0 THEN BEGIN
                         QUANTITY = VAL(UNPACK$(STKTF.QUANTITY$)) +     \
                            VAL(UNPACK$(STKRC.FINAL.QUANTITY$)) -       \
                            VAL(UNPACK$(STKRC.INITIAL.QUANTITY$))
                         STKTF.QUANTITY$ = PACK$(FN.ZERO.PL(STR$(QUANTITY),6))
                         RC% = WRITE.STKTF
                         IF RC% <> 0 THEN BEGIN
                            CURR.SESS.NUM% = STKTF.SESS.NUM%
                            GOTO WRITE.ERROR
                         ENDIF
                     ENDIF

                  ENDIF ELSE BEGIN
                     IDF.BOOTS.CODE$ = PACK$(ITEM.CODE$)
                     RC% = READ.IDF

                     IF RC% <> 0 THEN BEGIN
                         STKEX.ITEM.CODE$ = STKIF.ITEM.CODE$
                         STKEX.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                         STKEX.LOCATION$ = LEFT$(STKBF.DATA$,5)          ! 1.3
                         STKEX.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                         STKEX.REASON.CODE$ = "I"
                         RC% = WRITE.STKEX
                         IF RC% <> 0 THEN BEGIN
                            CURR.SESS.NUM% = STKEX.SESS.NUM%
                            GOTO WRITE.ERROR
                         ENDIF
                         STKEX.RECORD.NUM% = STKEX.RECORD.NUM% + 1

                         GOSUB ADD.TO.STKMF
                         BATCH.ITEMS = BATCH.ITEMS + 1
                         STKIF.BUSINESS.CENTRE$ = "X"
                         STKIF.GROUP.SEQUENCE$ = PACK$("000000")
                         STKIF.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                         STKIF.DESCRIPTION$ = "UNKNOWN ITEM            "
                         STKIF.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                         STKIF.STKMF.RECORD.NUM% = STKMF.RECORD.NUM% - 1
                         RC% = WRITE.STKIF
                         IF RC% <> 0 THEN BEGIN
                            CURR.SESS.NUM% = STKIF.SESS.NUM%
                            GOTO WRITE.ERROR
                         ENDIF

                     ENDIF ELSE BEGIN

                        IF ALL.PRODUCTS = 1 THEN GOTO ITEM.IN.SCOPE

                        FOR R% = 1 TO BC%
                          IF IDF.BSNS.CNTR$ = BUSINESS.CENTRES$(R%) THEN BEGIN
                            GOTO ITEM.IN.SCOPE
                          ENDIF
                        NEXT R%

                        FOR R% = 1 TO CG%
                          IF LEFT$(UNPACK$(IDF.PRODUCT.GRP$),2) = CONCEPT.GROUPS$(R%) \
                             THEN BEGIN
                               GOTO ITEM.IN.SCOPE
                          ENDIF
                        NEXT R%

                        FOR R% = 1 TO PG%
                          IF UNPACK$(IDF.PRODUCT.GRP$) = PRODUCT.GROUPS$(R%) THEN BEGIN
                             GOTO ITEM.IN.SCOPE
                          ENDIF
                        NEXT R%

                        ! Lookup item on XGCF                              !1.4
                        XGCF.BSNS.CNTR$ = IDF.BSNS.CNTR$                   !1.4
                        RC% = READ.XGCF                                    !1.4
                        IF RC% <> 0 THEN BEGIN                             !1.4
                           ! Error occurred                                !1.4
                           CURR.SESS.NUM% = XGCF.SESS.NUM%                 !1.4
                           GOSUB SB.FILE.READ.ERROR                        !1.4
                        ENDIF ELSE BEGIN                                   !1.4
                           ! Record on XGCF, so treat as group code        !1.4
                           ITEM.CODE$ = RIGHT$(UNPACK$(XGCF.BAR.CODE$), 14)!1.4
                           ! Poke group code back into STKBF record to     !1.4
                           ! force it to be written to the STKMF           !1.4
                           STKBF.DATA$ = LEFT$(STKBF.DATA$, 5) +           \1.4
                                         RIGHT$(ITEM.CODE$, 13) +          \1.4
                                         MID$(STKBF.DATA$, 19, 6) +        \1.4
                                         RIGHT$(UNPACK$(IRF.SALEPRIC$), 6) + \1.4
                                         RIGHT$(STKBF.DATA$,               \1.4
                                                LEN(STKBF.DATA$)-30)       !1.4
                           GOSUB PROCESS.GROUP.CODE                        !1.4
                           GOTO ITEM.PROCESSED                             !1.4
                        ENDIF                                              !1.4

                        ! Not in scope, write to exception log
                        STKEX.ITEM.CODE$ = STKIF.ITEM.CODE$
                        STKEX.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                        STKEX.LOCATION$ = LEFT$(STKBF.DATA$,5)         ! 1.3
                        STKEX.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                        STKEX.REASON.CODE$ = "S"
                        RC% = WRITE.STKEX
                        IF RC% <> 0 THEN BEGIN
                            CURR.SESS.NUM% = STKEX.SESS.NUM%
                            GOTO WRITE.ERROR
                        ENDIF
                        STKEX.RECORD.NUM% = STKEX.RECORD.NUM% + 1
                        GOTO ITEM.PROCESSED

ITEM.IN.SCOPE:

                        GOSUB ADD.TO.STKMF
                        BATCH.ITEMS = BATCH.ITEMS + 1
                        STKIF.BUSINESS.CENTRE$ = IDF.BSNS.CNTR$
                        STKIF.GROUP.SEQUENCE$ = IDF.PRODUCT.GRP$
                        STKIF.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                        STKIF.DESCRIPTION$ = IDF.STNDRD.DESC$
                        STKIF.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                        STKIF.STKMF.RECORD.NUM% = STKMF.RECORD.NUM% - 1
                        RC% = WRITE.STKIF
                            IF RC% <> 0 THEN BEGIN
                               CURR.SESS.NUM% = STKIF.SESS.NUM%
                               GOTO WRITE.ERROR
                            ENDIF
                        IF (IDF.BIT.FLAGS.2% AND 00001000b) <> 0 THEN BEGIN

                            STKTF.BOOTS.CODE$ = IDF.BOOTS.CODE$
                            RC% = READ.STKTF
                            IF RC% = 0 THEN BEGIN
                               QUANTITY = VAL(UNPACK$(STKTF.QUANTITY$)) +  \
                                  VAL(UNPACK$(STKIF.QUANTITY$))
                               STKTF.QUANTITY$ = PACK$(FN.ZERO.PL(STR$(QUANTITY),6))
                               RC% = WRITE.STKTF
                               IF RC% <> 0 THEN BEGIN
                                  CURR.SESS.NUM% = STKTF.SESS.NUM%
                                  GOTO WRITE.ERROR
                               ENDIF
                            ENDIF ELSE BEGIN
                               STKTF.QUANTITY$ = STKIF.QUANTITY$
                               STKTF.BUSINESS.CENTRE$ = STKIF.BUSINESS.CENTRE$
                               STKTF.GROUP.SEQUENCE$ = STKIF.GROUP.SEQUENCE$
                               RC% = WRITE.STKTF
                               IF RC% <> 0 THEN BEGIN
                                  CURR.SESS.NUM% = STKTF.SESS.NUM%
                                  GOTO WRITE.ERROR
                               ENDIF
                            ENDIF
                        ENDIF ELSE BEGIN
                            STKEX.ITEM.CODE$ = PACK$("000000") + IDF.BOOTS.CODE$
                            STKEX.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6)) ! 1.1 ! 1.3
                            STKEX.LOCATION$ = LEFT$(STKBF.DATA$,5)      ! 1.3
                            STKEX.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
                            STKEX.REASON.CODE$ = "N"
                            RC% = WRITE.STKEX
                            IF RC% <> 0 THEN BEGIN
                               CURR.SESS.NUM% = STKEX.SESS.NUM%
                               GOTO WRITE.ERROR
                            ENDIF
                            STKEX.RECORD.NUM% = STKEX.RECORD.NUM% + 1
                        ENDIF
                     ENDIF
                  ENDIF
               ENDIF
            ENDIF

       ENDIF

ITEM.PROCESSED:

RETURN


PROCESS.GROUP.CODE:                                                        ! CSW

   ! Group code
   STKDC.DUMP.CODE$ = PACK$(RIGHT$("00000000000000"+ITEM.CODE$,14))
   STKDC.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6))    ! 1.1 ! 1.3
   STKDC.PRICE$ = PACK$(MID$(STKBF.DATA$,25,6))       ! 1.1 ! 1.3
   STKDC.LOCATION$ = LEFT$(STKBF.DATA$,5)             ! 1.3
   STKDC.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
   RC% = WRITE.STKDC
   IF RC% <> 0 THEN BEGIN
      CURR.SESS.NUM% = STKDC.SESS.NUM%
      GOTO WRITE.ERROR
   ENDIF
   STKDC.RECORD.NUM% = STKDC.RECORD.NUM% + 1

   GOSUB ADD.TO.STKMF
   BATCH.ITEMS = BATCH.ITEMS + 1

RETURN                                                                     ! CSW


\******************************************************************************
\***   ADD TO RECOUNT
\***
\***   Format data
\***   Write to STKRC
\***   Update record number
\***   Update STKIF
\***   Update STKMF
\***
\******************************************************************************
ADD.TO.RECOUNT:

       !******************************************************************!1.3
       !* For first time we count a location, we add recounts to current  !1.3
       !* totals.                                                         !1.3
       !******************************************************************!1.3
       IF CUR.LOC.COUNTER% = 0 THEN BEGIN
          !***************************************************************!1.3
          !* STKBF.QUANTITY = STKBF.QUANTITY + STKIF.QUANTITY.            !1.3
          !* where STKBF.QUANTITY = MID$(STKBF.DATA$,19,6)                !1.3
          !*   and STKIF.QUANTITY is a packed decimal string.             !1.3
          !***************************************************************!1.3
          STKBF.DATA$=LEFT$(STKBF.DATA$,18)   \                           !1.3
                      +                       \                           !1.3
                      RIGHT$("000000"+STR$(VAL(MID$(STKBF.DATA$,19,6)) + \!1.3
                                           VAL(UNPACK$(STKIF.QUANTITY$)) \!1.3
                                          ), 6)                          \!1.3
                      +                       \                           !1.3
                      MID$(STKBF.DATA$,25,LEN(STKBF.DATA$)-25)            !1.3
       ENDIF                                                              !1.3

       STKRC.LOC.CNT% = CUR.LOC.COUNTER% + 1                              !1.3
       STKRC.ITEM.CODE$ = STKIF.ITEM.CODE$
       STKRC.ITEM.DESCRIPTION$ = STKIF.DESCRIPTION$
       STKRC.LOCATION$ = LEFT$(STKBF.DATA$,5)                             !1.3
       STKRC.INITIAL.QUANTITY$ = STKIF.QUANTITY$
       STKRC.INITIAL.STOCKTAKER$ = STKIF.STOCKTAKER.NUM$
       STKRC.FINAL.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6))   ! 1.1 ! 1.3
       STKRC.FINAL.STOCKTAKER$ = PACK$(STOCKTAKER$)
       RC% = WRITE.STKRC
       IF RC% <> 0 THEN BEGIN
           CURR.SESS.NUM% = STKRC.SESS.NUM%
           GOTO WRITE.ERROR
       ENDIF
       STKRC.RECORD.NUM% = STKRC.RECORD.NUM% + 1

       STKIF.QUANTITY$ = PACK$(MID$(STKBF.DATA$,19,6))         ! 1.1 ! 1.3
       STKIF.STOCKTAKER.NUM$ = PACK$(STOCKTAKER$)
       RC% = WRITE.STKIF
       IF RC% <> 0 THEN BEGIN
           CURR.SESS.NUM% = STKIF.SESS.NUM%
           GOTO WRITE.ERROR
       ENDIF

       SAVED.RECORD.NUM% = STKMF.RECORD.NUM%
       STKMF.RECORD.NUM% = STKIF.STKMF.RECORD.NUM%
       RC% = READ.STKMF
       IF RC% <> 0 THEN BEGIN
           CURR.SESS.NUM% = STKMF.SESS.NUM%
           GOTO READ.ERROR
       ENDIF

       GOSUB ADD.TO.STKMF
       STKMF.RECORD.NUM% = SAVED.RECORD.NUM%

RETURN

\******************************************************************************
\***   ADD TO STKMF
\***
\***   Format data
\***   Write to STKMF
\***   Update record number
\***
\******************************************************************************
ADD.TO.STKMF:

            STKMF.DATA$ = LEFT$(STKBF.DATA$,1) +                  \
               MID$(STKBF.DATA$,6,13) +                           \ 1.3
               MID$(STKBF.DATA$,19,12) +                          \ 1.1 1.3
               CR$
            RC% = WRITE.STKMF
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKMF.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKMF.RECORD.NUM% = STKMF.RECORD.NUM% + 1

RETURN

\******************************************************************************
\***   CHECK.GROUP.CODE:
\***
\***   Set code type = barcode
\***   Set Item Code
\***   If Boots Code Then
\***      If 1st digit = 0 Then Group Code
\***   Else
\***      Remove leading zero's
\***      If length = 13 Then
\***         If 1st digit = 2 Then Group Code
\***      If length = 12 Then
\***         If 1st digit = 2 Then Group Code
\***      If length = 8 Then
\***         If 1st digit = 2 Then Group Code
\***      If length = 6 Then
\***         Set Group Code
\***
\******************************************************************************
CHECK.GROUP.CODE:

       ITEM.CODE$ = MID$(STKBF.DATA$,6,13)                             ! 1.3
       CURRENT.CODE = 0
       GROUP.CODE = 0
       IF LEFT$(ITEM.CODE$,6) = "      " THEN BEGIN
          ITEM.CODE$ = RIGHT$(ITEM.CODE$,7)
          CURRENT.CODE = 1
          IF LEFT$(ITEM.CODE$,1) = "0" THEN GROUP.CODE = 1
       ENDIF ELSE BEGIN
          ITEM.CODE$ = STR$(VAL(ITEM.CODE$))
          CODE.LENGTH = LEN(ITEM.CODE$)
          IF CODE.LENGTH = 13 OR CODE.LENGTH = 12 OR CODE.LENGTH = 8 THEN BEGIN
             IF LEFT$(ITEM.CODE$,1)="2" THEN GROUP.CODE = 1
          ENDIF ELSE BEGIN
             IF CODE.LENGTH = 6 THEN GROUP.CODE = 1
          ENDIF
       ENDIF


RETURN


\******************************************************************************
\***
\***   PROCESS.TRAILER:
\***
\***   If header expected Then
\***           Write record to Recount Audit Trail
\***   Else
\***           Write record to Mainframe Transmission File
\***           Check trailer item count
\***           If mismatch Then
\***                   Write record to Recount Audit Trail
\***
\***   RETURN
\***
\******************************************************************************
PROCESS.TRAILER:

       IF HEADER.EXPECTED = 1 THEN BEGIN
            STKRC.ITEM.CODE$ = PACK$("00000000000000")
            STKRC.ITEM.DESCRIPTION$ = "Unexpected Trailer      "
            STKRC.LOCATION$ = "X0000"                                  ! 1.3
            STKRC.INITIAL.QUANTITY$ = PACK$("000000")
            STKRC.INITIAL.STOCKTAKER$ = PACK$("0000")
            STKRC.FINAL.QUANTITY$ = PACK$("000000")
            STKRC.FINAL.STOCKTAKER$ = PACK$("0000")
            RC% = WRITE.STKRC
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKRC.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKRC.RECORD.NUM% = STKRC.RECORD.NUM% + 1
       ENDIF ELSE BEGIN
            !*************************************************************!1.3
            !* Trailer reached, so signfy location has changed.           !1.3
            !*************************************************************!1.3
            NEW.LOC$ = "     "                                            !1.3
            GOSUB PROCESS.LOCATION.CHANGE                                 !1.3

            IF ITEM.COUNT <> VAL(LEFT$(STKBF.DATA$,6)) THEN BEGIN
               STKRC.ITEM.CODE$ = PACK$("00000000000000")
               STKRC.ITEM.DESCRIPTION$ = "Record Count Mismatch   "
               STKRC.LOCATION$ = "X0000"                               ! 1.3
               STKRC.INITIAL.QUANTITY$ = PACK$(FN.ZERO.PL(STR$(ITEM.COUNT),6))
               STKRC.INITIAL.STOCKTAKER$ = PACK$(STOCKTAKER$)
               STKRC.FINAL.QUANTITY$ = PACK$(LEFT$(STKBF.DATA$,6))
               STKRC.FINAL.STOCKTAKER$ = PACK$("0000")
               RC% = WRITE.STKRC
               IF RC% <> 0 THEN BEGIN
                  CURR.SESS.NUM% = STKRC.SESS.NUM%
                  GOTO WRITE.ERROR
               ENDIF
               STKRC.RECORD.NUM% = STKRC.RECORD.NUM% + 1
            ENDIF
            STKMF.DATA$ = RIGHT$("000000" + STR$(BATCH.ITEMS),6) +    \
               "                    " + CR$
            RC% = WRITE.STKMF
            IF RC% <> 0 THEN BEGIN
               CURR.SESS.NUM% = STKMF.SESS.NUM%
               GOTO WRITE.ERROR
            ENDIF
            STKMF.RECORD.NUM% = STKMF.RECORD.NUM% + 1
            HEADER.EXPECTED = 1
       ENDIF

RETURN

\*************************************************************************!1.3
\***                                                                      !1.3
\*** PROCESS.LOCATION.CHANGE:                                             !1.3
\***                                                                      !1.3
\***    increment number of times location has been counted in LOCCNT     !1.3
\***    read in the value for the next location from LOCCNT               !1.3
\***    set next location as current location.                            !1.3
\***                                                                      !1.3
\*** RETURN                                                               !1.3
\***                                                                      !1.3
\*************************************************************************!1.3
                                                                          !1.3
PROCESS.LOCATION.CHANGE:                                                  !1.3
                                                                          !1.3
   IF CUR.LOC$ <> "     " THEN BEGIN                                      !1.3
      LOCCNT.RECORD.NUM% = GET.LOCCNT.RECNUM(CUR.LOC$)                    !1.3
      RC% = READ.LOCCNT                                                   !1.3
      IF RC% = 0 THEN BEGIN                                               !1.3
         CUR.LOC.COUNTER% = LOCCNT.LOCATION.COUNT% + 1                    !1.3
         LOCCNT.LOCATION.COUNT% = CUR.LOC.COUNTER%                        !1.3
         RC% = WRITE.LOCCNT                                               !1.3
      ENDIF                                                               !1.3
   ENDIF                                                                  !1.3
   CUR.LOC$ = NEW.LOC$                                                    !1.3
   SXTCF.CUR.LOCATION$ = CUR.LOC$                                         !1.3
   LOCCNT.RECORD.NUM% = GET.LOCCNT.RECNUM(CUR.LOC$)                       !1.3
   IF LOCCNT.RECORD.NUM% > 0 THEN BEGIN                                   !1.3
      RC% = READ.LOCCNT                                                   !1.3
      IF RC% = 0 THEN BEGIN                                               !1.3
         CUR.LOC.COUNTER% = LOCCNT.LOCATION.COUNT%                        !1.3
      ENDIF                                                               !1.3
   ENDIF                                                                  !1.3
                                                                          !1.3
RETURN                                                                    !1.3


\******************************************************************************
\***
\***   DETERMINE.START.TYPE:
\***
\***      if the command tail is null then quit program and print a message
\***           saying "Program should not be started from command mode"
\***
\***   RETURN
\***
\******************************************************************************

DETERMINE.START.TYPE:

       IF COMMAND$ = "" THEN BEGIN
               PRINT "PSX01 should not be started from command mode."
               GOTO PROGRAM.EXIT
       ENDIF

RETURN



\******************************************************************************
\******************************************************************************
\***
\***           G E N E R A L    R E T U R N S    R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***    CHECK.VALID.ITEM
\***
\***    This routine will check that the item code passed is valid
\***    Check that the item code is found on the IDF
\***    Checks that the item code is for a valid credit claim Business Centre
\***    Check that the item code is found on the IRF
\***    If the local price flag is set on the IRF then provided the read
\***    of the local price file is successfull the item price will be the local
\***    price else it will take the IRF.SALEPRICE
\***    If the item is not found on the IDF, IRF or has an invalid credit claim BC then
\***    the valid entry variable will be set to false
\***
\*****************************************************************************

   CHECK.VALID.ITEM:                                                     ! 1.16JAS

   VALID.ENTRY% = TRUE                                                  ! 1.16JAS
   VALID.BC = FALSE                                                     ! 1.16JAS

   !Check item is a valid code
   BOOTS.CODE$ = MID$(DATA.IN$, 11,7)                                   ! 1.16JAS
   RC% = CHECK.BOOTS.CODE(BOOTS.CODE$)                                  ! 1.16JAS
   IF F05.VALID.CODE$ = "Y" THEN BEGIN                                  ! 1.16JAS
      IDF.BOOTS.CODE$ = PACK$(RIGHT$("00000000" + BOOTS.CODE$,8))      ! 1.16JAS
      CURR.SESS.NUM% = IDF.SESS.NUM%                                    ! 1.16JAS
      RC% = READ.IDF                                                    ! 1.16JAS
      IF RC% <> 0 THEN BEGIN                                            ! 1.16JAS
        IDF.BOOTS.CODE$ = PACK$(RIGHT$("00000000" + BOOTS.CODE$,8))    ! 1.16JAS
        CCTMP.ITEM.PRICE$ = PACK$("000000")                             ! 1.16JAS
        IDF.STNDRD.DESC$ = "+++ ITEM NOT ON FILE +++"                   ! 1.16JAS
        IDF.BSNS.CNTR$ = " "                                            ! 1.16JAS
!JAS        VALID.ENTRY% = FALSE                                            ! 1.16JAS
!JAS        CCREJ.REJECT.REASON$ = "3"                                      ! 1.18JAS
      ENDIF ELSE BEGIN                                                  ! 1.16JAS
        GOSUB CHECK.BUSINESS.CENTRE                                     ! 1.16JAS
        IF VALID.BC THEN BEGIN                                          ! 1.16JAS
            IRF.BAR.CODE$ = PACK$("0000000000") + IDF.FIRST.BAR.CODE$   ! 1.16JAS
            CURR.SESS.NUM% = IRF.SESS.NUM%                              ! 1.16JAS
            RC% = READ.IRF                                              ! 1.16JAS
            IF RC% <> 0 THEN BEGIN                                      ! 1.16JAS
               CCTMP.ITEM.PRICE$ = PACK$("000000")                      ! 1.16JAS
               IDF.STNDRD.DESC$ = "+++ ITEM NOT ON FILE +++"            ! 1.16JAS
               IDF.BSNS.CNTR$ = " "                                     ! 1.16JAS
!JAS               VALID.ENTRY% = FALSE                                     ! 1.16JAS
!JAS               CCREJ.REJECT.REASON$ = "3"                               ! 1.18JAS
            ENDIF ELSE BEGIN                                            ! 1.16JAS
               ! If local price item then try reading local file - if read not successfull
               ! then set item price to IRF.SALEPRICE
               IF (IRF.INDICAT3% AND 00100000B) = 00100000B THEN BEGIN  ! 1.16JAS
                  GOSUB OPEN.LOCAL                                      ! 1.16JAS
                  LOCAL.ITEM.CODE$ = IDF.BOOTS.CODE$                    ! 1.16JAS
                  CURR.SESS.NUM% = LOCAL.SESS.NUM%                      ! 1.16JAS
                  RC% = READ.LOCAL                                      ! 1.16JAS
                  GOSUB CLOSE.LOCAL                                     ! 1.16JAS
                  IF RC% = 0 THEN BEGIN                                 ! 1.16JAS
                     CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + LOCAL.H.O.PRICE$,3) ! 1.16JAS
                  ENDIF ELSE BEGIN                                      ! 1.16JAS
                     CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + IRF.SALEPRIC$,3)    ! 1.16JAS
                  ENDIF                                                 ! 1.16JAS
               ENDIF ELSE BEGIN                                         ! 1.16JAS
                  CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + IRF.SALEPRIC$,3)! 1.16JAS
               ENDIF                                                    ! 1.16JAS
            ENDIF                                                       ! 1.16JAS
        ENDIF ELSE BEGIN                                                ! 1.16JAS
            VALID.ENTRY% = FALSE                                        ! 1.16JAS
        ENDIF                                                           ! 1.16JAS
      ENDIF                                                             ! 1.16JAS
   ENDIF ELSE BEGIN                                                     ! 1.16JAS
      IDF.STNDRD.DESC$ = "+++ ITEM NOT ON FILE +++"                     ! 1.16JAS
      IDF.BSNS.CNTR$ = " "                                              ! 1.16JAS
!JAS      VALID.ENTRY% = FALSE                                              ! 1.16JAS
!JAS      CCREJ.REJECT.REASON$ = "3"                                        ! 1.18JAS
   ENDIF                                                                ! 1.16JAS

   RETURN                                                               ! 1.16JAS

\******************************************************************************
\***
\***    CHECK.VALID.BARCODE
\***
\***    This routine will check that the bar code passed is valid
\***    Check that the item code is found on the IRF
\***    Using the IRF.BOOTS.CODE calculate the check digit
\***    Using the IRF.BOOTS.CODE + check digit, check that the item code is found on the IDF
\***    Checks that the item code is for a valid credit claim Business Centre
\***    If the local price flag is set on the IRF then provided the read
\***    of the local price file is successfull the item price will be the local
\***    price else it will take the IRF.SALEPRICE
\***    If the item is not found on the IDF, IRF or has an invalid credit claim BC then
\***    the valid entry variable will be set to false
\***
\*****************************************************************************

   CHECK.VALID.BARCODE:                                                 ! 1.16JAS

   VALID.ENTRY% = TRUE                                                  ! 1.16JAS
   VALID.BC     = FALSE                                                 ! 1.16JAS

   BOOTS.CODE$  = MID$(DATA.IN$, 6, 12)                                 ! 1.16JAS

   IRF.BAR.CODE$ = PACK$(RIGHT$(STRING$(22,"0") +         \             ! 1.16JAS
                                  BOOTS.CODE$, 22))                     ! 1.16JAS

   CURR.SESS.NUM% = IRF.SESS.NUM%                                       ! 1.16JAS
   RC% = READ.IRF                                                       ! 1.16JAS
   IF RC% <> 0 THEN BEGIN                                               ! 1.16JAS
       CCTMP.ITEM.PRICE$ = PACK$("000000")                              ! 1.16JAS
       IDF.STNDRD.DESC$ = "+++ ITEM NOT ON FILE +++"                    ! 1.16JAS
       IDF.BSNS.CNTR$ = " "                                             ! 1.16JAS
       BOOTS.CODE$ = RIGHT$("00" + BOOTS.CODE$,8)                       ! 1.16JAS
!JAS       VALID.ENTRY% = FALSE                                             ! 1.16JAS
!JAS       CCREJ.REJECT.REASON$ = "3"                                       ! 1.18JAS

   ENDIF ELSE BEGIN                                                     ! 1.16JAS
      BOOTS.CODE$ = UNPACK$(IRF.BOOTS.CODE$)                            ! 1.16JAS
      RC% = CALC.BOOTS.CODE.CHECK.DIGIT(BOOTS.CODE$)                    ! 1.16JAS
      IF RC% <> 0 THEN GOTO PROGRAM.EXIT                                ! 1.16JAS
      IDF.BOOTS.CODE$ = PACK$(RIGHT$("00000000" +             \         ! 1.16JAS
                                       BOOTS.CODE$ +          \         ! 1.16JAS
                                       F18.CHECK.DIGIT$, 8))            ! 1.16JAS
      BOOTS.CODE$ = UNPACK$(IDF.BOOTS.CODE$)                            ! 1.16JAS
      CCTMP.ITEM.BARCODE.FLAG$ = "I"                                    ! 1.16JAS
      CURR.SESS.NUM% = IDF.SESS.NUM%                                    ! 1.16JAS
      RC% = READ.IDF                                                    ! 1.16JAS
      IF RC% <> 0 THEN BEGIN                                            ! 1.16JAS
         IDF.STNDRD.DESC$ = IRF.ITEMNAME$                               ! 1.16JAS
         IDF.BSNS.CNTR$ = " "                                           ! 1.16JAS
         CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + IRF.SALEPRIC$,3)  ! 1.16JAS
!         VALID.ENTRY% = FALSE                                           ! 1.16JAS
!         CCREJ.REJECT.REASON$ = "3"                                     ! 1.18JAS
      ENDIF ELSE BEGIN                                                  ! 1.16JAS
        GOSUB CHECK.BUSINESS.CENTRE                                     ! 1.16JAS
        IF VALID.BC THEN BEGIN                                          ! 1.16JAS
           ! If local price item then try reading local file - if read not successfull
           ! then set item price to IRF.SALEPRICE
           IF (IRF.INDICAT3% AND 00100000B) = 00100000B THEN BEGIN      ! 1.16JAS
              GOSUB OPEN.LOCAL                                          ! 1.16JAS
              LOCAL.ITEM.CODE$ = IDF.BOOTS.CODE$                        ! 1.16JAS
              CURR.SESS.NUM% = LOCAL.SESS.NUM%                          ! 1.16JAS
              RC% = READ.LOCAL                                          ! 1.16JAS
              GOSUB CLOSE.LOCAL                                         ! 1.16JAS
              IF RC% = 0 THEN BEGIN                                     ! 1.16JAS
                 CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + LOCAL.H.O.PRICE$,3) ! 1.16JAS
              ENDIF ELSE BEGIN                                          ! 1.16JAS
                 CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + IRF.SALEPRIC$,3) ! 1.16JAS
              ENDIF                                                     ! 1.16JAS
           ENDIF ELSE BEGIN                                             ! 1.16JAS
              CCTMP.ITEM.PRICE$ = RIGHT$(PACK$("000000") + IRF.SALEPRIC$,3) ! 1.16JAS
           ENDIF                                                        ! 1.16JAS
        ENDIF ELSE BEGIN                                                ! 1.16JAS
           VALID.ENTRY% = FALSE                                         ! 1.16JAS
        ENDIF                                                           ! 1.16JAS
      ENDIF                                                             ! 1.16JAS
   ENDIF                                                                ! 1.16JAS

   RETURN                                                               ! 1.16JAS

\******************************************************************************
\***
\***    ADD.ITEM.TO.REJECT.TABLE
\***
\***    Item has been rejected - either the business centre the item is in is not
\***    allowed in credit claiming or the item code does not exist on both the
\***    IRF and IDF (Items not on file has been commented out as these items
\***    currently need to be passed through to credit claiming)
\***    The following item information is added to the CCREJ file to be reported on
\***    later in the program
\***    The Item Barcode flag (either I = item code, B = Barcode)
\***    The Item or Barcode
\***    The Standard description (will be set to Item not on file - if not on IDF/IRF)
\***    The Business Centre (will be set to blank if not on IDF/IRF)
\***    The item qty claimed
\***    Add record to rejection file for item being rejected from claim.
\***
\*****************************************************************************

   ADD.ITEM.TO.REJECT.TABLE:                                            ! 1.16JAS

  ! Add record to Credit Claim Rejection file

  CCREJ.RECORD$ = ""                                                    ! 1.18JAS

   IF LEFT$(DATE$,2) < "85" THEN BEGIN                                  ! 1.18JAS
      CCREJ.RECORD$ = "20" + DATE$                                      ! 1.18JAS
   ENDIF ELSE BEGIN                                                     ! 1.18JAS
      CCREJ.RECORD$ = "19" + DATE$                                      ! 1.18JAS
   ENDIF                                                                ! 1.18JAS

  ! If Other Credit Claiming transmission set claim business centre to item business centre

   IF LDT.RETURNS.TYPE$ NE "G" THEN BEGIN                               ! 1.18JAS
      CCTMP.BC.LETTER$ = IDF.BSNS.CNTR$                                 ! 1.18JAS
   ENDIF                                                                ! 1.18JAS

   CCREJ.RECORD$ = CCREJ.RECORD$                 +         \            ! 1.18JAS
                   UNPACK$(CCTMP.RETURN.REASON$) +         \            ! 1.18JAS
                   CCTMP.BC.LETTER$              +         \            ! 1.18JAS
                   UNPACK$(CCTMP.UOD.NUM$)       +         \            ! 1.18JAS
                   CCTMP.ITEM.BARCODE.FLAG$      +         \            ! 1.18JAS
                   UNPACK$(CCTMP.ITEM.BARCODE$)  +         \            ! 1.18JAS
                   IDF.STNDRD.DESC$              +         \            ! 1.18JAS
                   IDF.BSNS.CNTR$                +         \            ! 1.18JAS
                   CCTMP.ITEM.QTY$               +         \            ! 1.18JAS
                   CCREJ.REJECT.REASON$                                 ! 1.18JAS

   CCREJ.RECORD$ = CCREJ.RECORD$ + CCREJ.ENDREC.MARKER$                 ! 1.18JAS

   IF CCREJ.OPEN.FLAG$ <> "Y" THEN BEGIN                                ! 1.18JAS
     IF END# CCREJ.SESS.NUM% THEN OPEN.ERROR                            ! 1.18JAS
     CURR.SESS.NUM% = CCREJ.SESS.NUM%                                   ! 1.18JAS
     OPEN CCREJ.FILE.NAME$ AS CCREJ.SESS.NUM% BUFFSIZE 512              \ 1.18JAS
       READONLY APPEND                                                  ! 1.18JAS
     CCREJ.OPEN.FLAG$ = "Y"                                             ! 1.18JAS
   ENDIF                                                                ! 1.18JAS

   CURR.SESS.NUM% = CCREJ.SESS.NUM%                                     ! 1.18JAS
   IF WRITE.CCREJ THEN GOTO WRITE.ERROR                                 ! 1.18JAS

   CCREJ.RECS.PRESENT = TRUE                                            ! 1.18JAS

   RETURN                                                               ! 1.16JAS

\******************************************************************************
\***
\***   ADD.ITEM.TO.VALID.TABLE
\***
\***   Item qty is set to 1
\***   The value of item qty * item price is checked
\***   Set the item qty to equal the CCTMP.ITEM.QTY value
\***   If the value is > £9,999.99 then if the item is the first item in the claim
\***   the F14 table needs to be resized (number of items in claim + EXTRA.ITEMS)
\***   If there were already entries in the F14.TABLE then size the temp table to
\***   the number of records already held in the f14 table.  Load the F14 table into
\***   the temp table.  Resize F14.TABLE to the number of items in the claim + EXTRA.ITEMS
\***   Load the resized F14 table with data from the temp table.
\***   For the item qty value add the entries to the F14.TABLE
\***   Valid items are added to this table to be added to the STKMQ file later in the processing
\***   If other credit claim transaction then a type 25 STKMQ record will be written later in
\***   the processing, if Goods out transaction then a type 26 STKMQ record will be written
\***   later in the processing
\***   The following data is added to the table for each item from Other Credit Claim transmission
\***   The items business centre
\***   The item/barcode flag (I = item code, B = barcode)
\***   The item/barcode
\***   The item qty claimed
\***   The item price
\***   The following data is added to the table for each item from Goods Out transmission
\***   The items business centre
\***   The item/barcode flag (I = item code, B = barcode)
\***   The item/barcode
\***   The item qty claimed
\***   The item updated
\***
\******************************************************************************

   ADD.ITEM.TO.VALID.TABLE:                                             ! 1.16JAS

   ITEM.QTY% = 1                                                        ! 1.17JAS

   IF CCTMP.ITEM.PRICE$ <> PACK$("000000") THEN BEGIN                        ! 1.18JAS
      IF (VAL(CCTMP.ITEM.QTY$) * VAL(UNPACK$(CCTMP.ITEM.PRICE$))) > 999999 THEN BEGIN   ! 1.17JAS
         ITEM.QTY% = VAL(CCTMP.ITEM.QTY$)                                      ! 1.17JAS
         EXTRA.ITEMS% = EXTRA.ITEMS% + (ITEM.QTY% - 1)                         ! 1.17JAS
         CCTMP.ITEM.QTY$ = "0001"                                              ! 1.17JAS
         IF VALID.INDEX% > 0 THEN BEGIN                                        ! 1.17JAS
            DIM TEMP.TABLE$(VALID.INDEX%)                                      ! 1.17JAS
            FOR I% = 1 TO VALID.INDEX%                                         ! 1.17JAS
              TEMP.TABLE$(I%) = F14.TABLE$(I%)                                ! 1.17JAS
            NEXT I%                                                            ! 1.17JAS
            DIM F14.TABLE$(CCTMP.NUM.ITEMS.UPDATED% + (EXTRA.ITEMS%))          ! 1.17JAS
            FOR I% = 1 TO VALID.INDEX%                                         ! 1.17JAS
               F14.TABLE$(I%) = TEMP.TABLE$(I%)                                ! 1.17JAS
            NEXT I%                                                            ! 1.17JAS
         ENDIF ELSE BEGIN                                                      ! 1.17JAS
            DIM F14.TABLE$(CCTMP.NUM.ITEMS.UPDATED% + (EXTRA.ITEMS%))          ! 1.17JAS
         ENDIF                                                                 ! 1.17JAS
      ENDIF
   ENDIF                                                                   ! 1.18JAS

   IF LDT.RETURNS.TYPE$ NE "G" THEN BEGIN                                  ! 1.18JAS
      FOR I% = 1 TO ITEM.QTY%                                              ! 1.17JAS

         VALID.INDEX% = VALID.INDEX% + 1                                   ! 1.16JAS
         F14.TABLE$(VALID.INDEX%) = IDF.BSNS.CNTR$           +             \ 1.16JAS
                                    CCTMP.ITEM.BARCODE.FLAG$ +             \ 1.16JAS
                                    CCTMP.ITEM.BARCODE$      +             \ 1.16JAS
                                    CCTMP.ITEM.QTY$          +             \ 1.16JAS
                                    CCTMP.ITEM.PRICE$                      ! 1.16JAS

      NEXT I%                                                              ! 1.17JAS
   ENDIF ELSE BEGIN                                                        ! 1.18JAS
      FOR I% = 1 TO ITEM.QTY%                                              ! 1.18JAS

         VALID.INDEX% = VALID.INDEX% + 1                                   ! 1.18JAS
         F14.TABLE$(VALID.INDEX%) = IDF.BSNS.CNTR$           +             \ 1.18JAS
                                    CCTMP.ITEM.BARCODE.FLAG$ +             \ 1.18JAS
                                    CCTMP.ITEM.BARCODE$      +             \ 1.18JAS
                                    CCTMP.ITEM.QTY$          +             \ 1.18JAS
                                    CCTMP.ITEM.UPDATED$                    ! 1.18JAS

      NEXT I%                                                              ! 1.18JAS

   ENDIF                                                                   ! 1.18JAS

   RETURN                                                                  ! 1.16JAS

\******************************************************************************
\***
\***    CHECK.BUSINESS.CENTRE
\***
\***    A list of valid Business Centres allowed for Credit claiming is held
\***    on the EALSOPTS file (record 105)
\***    The file was read earlier in the program and the business centres loaded
\***    into the variable OTHER.CLAIM.VALID.BC
\***    The items business centre is matched against the business centres contained
\***    in the OTHER.CLAIM.VALID.BC variable
\***    If a match is found that the VALID.BC is set to TRUE
\***    If a match is not found then the reject reason is set to 2 (invalid
\***    business centre)
\***
\*****************************************************************************

   CHECK.BUSINESS.CENTRE:                                               ! 1.16JAS

   IF MATCH(IDF.BSNS.CNTR$,OTHER.CLAIM.VALID.BC$,1) > 0 THEN BEGIN      ! 1.16JAS
      IF LDT.RETURNS.TYPE$ = "G" AND CREDIT.CLAIM.FLAG$ = "YY" THEN BEGIN  ! 1.18JAS
         IF IDF.BSNS.CNTR$ = CCTMP.BC.LETTER$ THEN BEGIN                ! 1.18JAS
            VALID.BC = TRUE                                             ! 1.18JAS
         ENDIF ELSE BEGIN                                               ! 1.18JAS
            CCREJ.REJECT.REASON$ = "2"                                  ! 1.18JAS
         ENDIF                                                          ! 1.18JAS
      ENDIF ELSE BEGIN                                                  ! 1.18JAS
         VALID.BC = TRUE                                                ! 1.16JAS
      ENDIF                                                             ! 1.18JAS
   ENDIF ELSE BEGIN                                                     ! 1.16JAS 1.18JAS
      CCREJ.REJECT.REASON$ = "1"                                        ! 1.18JAS
   ENDIF                                                                ! 1.18JAS



   RETURN                                                               ! 1.16JAS

\******************************************************************************
\***
\***    UNLOAD.ACTIVE.TABLE
\***
\***    When writing out to the CCTMP file
\***    If the transmission was from Other Credit Claiming need to split the claims
\***    for the same reason but different business centres into separate claims.
\***    This part of the code will create a type 25 transaction for each
\***    different business centre.
\***
\***    The table contain valid items is sorted into business centre order.
\***    Where the current business centre does not match the previous business
\***    centre and the previous business centre was not set to ? (this would
\***    indicate the first time in this processing) then using the data held
\***    in the TEMP.RECORD (which contains the claim global data) write this out
\***    as the start of the next type 25 record)
\***    If the previous Business centre was not set to space then write out the
\***    end of record data to the CCTMP.RECORD before adding the next TEMP data
\***    to complete a type 25 transaction
\***
\***    Where the transmission was from Goods out then the data from the table
\***    will be unloaded and a type 26 transaction created.
\***    The number of items will be rewritten to the num.item.updated field and
\***    total.qty.in.uod (this could have changed from the value passed by the
\***    pdt due to the removal of rejected items or where the value of the item
\***    * item prices exceeded £9,999.99 and the item had to be split into individual
\***    items.
\***
\*****************************************************************************

   UNLOAD.ACTIVE.TABLE:                                                 ! 1.16JAS

   IF LDT.RETURNS.TYPE$ NE "G" THEN BEGIN                               ! 1.18JAS

      RC% EQ SORT.TABLE(VALID.INDEX%)                                   ! 1.16JAS

      PREV.BC$ = "?"                                                    ! 1.16JAS 1.18JAS
      ITEM.COUNT% = 0                                                   ! 1.16JAS

      FOR I% = 1 TO VALID.INDEX%                                        ! 1.16JAS
         IF PREV.BC$ NE LEFT$(F14.TABLE$(I%),1) THEN BEGIN              ! 1.16JAS
            IF PREV.BC$ NE "?" THEN BEGIN                               ! 1.16JAS 1.18JAS
               CCTMP.RECORD$ = LEFT$(CCTMP.RECORD$, 13)    +            \ 1.16JAS
                               RIGHT$("0000" + STR$(ITEM.COUNT%),4) +      \ 1.16JAS
                               RIGHT$(CCTMP.RECORD$,(LEN(CCTMP.RECORD$)-17)) ! 1.16JAS
               ITEM.COUNT% = 0                                          ! 1.16JAS
               CCTMP.RECORD$ = CCTMP.RECORD$ +                          \ 1.16JAS
                               CCTMP.RECORD.DELIMITER$ +                \ 1.16JAS
                               CCTMP.ENDREC.MARKER$                     ! 1.16JAS
            CURR.SESS.NUM% = CCTMP.SESS.NUM%                            ! 1.16JAS
            CURRENT.KEY$ = ""                                           ! 1.16JAS
            IF WRITE.CCTMP THEN GOTO WRITE.ERROR                        ! 1.16JAS
            CCTMP.RECORD$ = ""                                          ! 1.16JAS
            ENDIF                                                       ! 1.16JAS
            CCTMP.RECORD$ = LEFT$(TEMP.RECORD$,13)   +                  \ 1.16JAS
                            RIGHT$("0000" + STR$(ITEM.COUNT%),4) +      \ 1.16JAS
                            RIGHT$(TEMP.RECORD$,(LEN(TEMP.RECORD$)-17)) ! 1.16JAS
         ENDIF                                                          ! 1.16JAS
         ITEM.COUNT% = ITEM.COUNT% + 1                                  ! 1.16JAS
         CCTMP.RECORD$ = CCTMP.RECORD$ +                                \ 1.16JAS
                         RIGHT$(F14.TABLE$(I%),14)                      ! 1.16JAS
         PREV.BC$ = LEFT$(F14.TABLE$(I%),1)                             ! 1.16JAS
      NEXT I%                                                           ! 1.16JAS

      CCTMP.RECORD$ = LEFT$(CCTMP.RECORD$, 13)    +                     \ 1.16JAS
                      RIGHT$("0000" + STR$(ITEM.COUNT%),4) +            \ 1.16JAS
                      RIGHT$(CCTMP.RECORD$,(LEN(CCTMP.RECORD$)-17))     ! 1.16JAS

   ENDIF ELSE BEGIN                                                     ! 1.18JAS

      ITEM.COUNT% = 0                                                   ! 1.18JAS

      FOR I% = 1 TO VALID.INDEX%                                        ! 1.18JAS
         ITEM.COUNT% = ITEM.COUNT% + 1                                  ! 1.18JAS
         CCTMP.RECORD$ = CCTMP.RECORD$ +                                \ 1.18JAS
                         RIGHT$(F14.TABLE$(I%),12)                      ! 1.18JAS
      NEXT I%                                                           ! 1.18JAS

      CCTMP.RECORD$ = LEFT$(CCTMP.RECORD$, 21)    +                     \ 1.18JAS
                      RIGHT$("0000" + STR$(ITEM.COUNT%),4) +            \ 1.18JAS
                      RIGHT$("0000" + STR$(ITEM.COUNT%),4) +            \ 1.18JAS
                      RIGHT$(CCTMP.RECORD$,(LEN(CCTMP.RECORD$)-29))     ! 1.18JAS

   ENDIF                                                                ! 1.18JAS

   RETURN                                                               ! 1.16JAS

\/*****************************************************************************
\/****
\/****              OPEN.LOCAL
\/****
\/*****************************************************************************

OPEN.LOCAL:                                                          !1.16JAS

     CURR.SESS.NUM% = LOCAL.SESS.NUM%                                !1.16JAS
     IF END #LOCAL.SESS.NUM% THEN OPEN.ERROR                         !1.16JAS
     OPEN LOCAL.FILE.NAME$ KEYED RECL LOCAL.RECL% AS LOCAL.SESS.NUM% \1.16JAS
          NOWRITE  NODEL                                             !1.16JAS
     LOCAL.OPEN.FLAG$ = "Y"                                          !1.16JAS

RETURN                                                               !1.16JAS

\/*****************************************************************************
\/****
\/****              CLOSE.LOCAL
\/****
\/*****************************************************************************

CLOSE.LOCAL:                                                         !1.16JAS

     IF LOCAL.OPEN.FLAG$ = "Y" THEN BEGIN                            !1.16JAS

       CLOSE LOCAL.SESS.NUM%                                         !1.16JAS
       LOCAL.OPEN.FLAG$ = "N"                                        !1.16JAS
     ENDIF                                                           !1.16JAS

RETURN                                                               !1.16JAS

\******************************************************************************
\***
\***    CREATE.EMPTY.CCUPF:
\***
\***      close CCUPF if already open
\***      create CCUPF as KEYED file
\***
\******************************************************************************

   CREATE.EMPTY.CCUPF:                                                 ! BMJK
                                                               ! BMJK
      CURR.SESS.NUM% = CCUPF.SESS.NUM%                                      ! BMJK
      IF CCUPF.OPEN.FLAG$ = "Y" THEN BEGIN                                  ! BMJK
         CCUPF.OPEN.FLAG$ = "N"                                             ! BMJK
         CLOSE CCUPF.SESS.NUM%                                           ! BMJK
      ENDIF                                                             ! BMJK
                                                               ! BMJK
      IF END #CCUPF.SESS.NUM% THEN CREATE.ERROR                            ! BMJK
      CREATE POSFILE CCUPF.FILE.NAME$ KEYED 7,,,500 RECL CCUPF.RECL%       \ BMJK
        AS CCUPF.SESS.NUM% MIRRORED ATCLOSE                            ! BMJK
      CLOSE CCUPF.SESS.NUM%                                          ! BMJK
                                                               ! BMJK
   RETURN                                                        ! BMJK

\******************************************************************************
\***
\***    CREATE.CCBUF:
\***
\***       close CCBUF if already open
\***       create CCBUF as sequential readonly
\***
\******************************************************************************

   CREATE.CCBUF:

      CURR.SESS.NUM% = CCBUF.SESS.NUM%

      IF CCBUF.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE CCBUF.SESS.NUM%
         CCBUF.OPEN.FLAG$ = "N"
      ENDIF

      IF END #CCBUF.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE CCBUF.FILE.NAME$ AS CCBUF.SESS.NUM%               \
             BUFFSIZE 512 READONLY MIRRORED ATCLOSE
      CCBUF.OPEN.FLAG$ = "Y"

   RETURN

\******************************************************************************
\***
\***    CREATE.CCREJ:
\***
\***       close CCREJ if already open
\***       create CCREJ as sequential readonly
\***
\******************************************************************************

   CREATE.CCREJ:                                                        ! 1.18JAS

      CURR.SESS.NUM% = CCREJ.SESS.NUM%                                  ! 1.18JAS

      IF CCREJ.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.18JAS
         CLOSE CCREJ.SESS.NUM%                                          ! 1.18JAS
         CCREJ.OPEN.FLAG$ = "N"                                         ! 1.18JAS
      ENDIF                                                             ! 1.18JAS

      IF END #CCREJ.SESS.NUM% THEN CREATE.ERROR                         ! 1.18JAS
      CREATE POSFILE CCREJ.FILE.NAME$ AS CCREJ.SESS.NUM%               \! 1.18JAS
             BUFFSIZE 512 READONLY MIRRORED ATCLOSE                     ! 1.18JAS
      CCREJ.OPEN.FLAG$ = "Y"                                            ! 1.18JAS

   RETURN                                                               ! 1.18JAS

\******************************************************************************
\***
\***    CREATE.CCTMP:
\***
\***       close CCTMP if already open
\***       create CCTMP as sequential readonly
\***
\******************************************************************************

   CREATE.CCTMP:

      CURR.SESS.NUM% = CCTMP.SESS.NUM%

      IF CCTMP.OPEN.FLAG$ = "Y" THEN BEGIN
         CLOSE CCTMP.SESS.NUM%
         CCTMP.OPEN.FLAG$ = "N"
      ENDIF

      IF END #CCTMP.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE CCTMP.FILE.NAME$ AS CCTMP.SESS.NUM%               \
             BUFFSIZE 512 READONLY MIRRORED ATCLOSE
      CCTMP.OPEN.FLAG$ = "Y"

   RETURN

! ASSIGN.NEXT.CREDIT.CLAIM.NUMBER moved to after CCTMP to STKMQ transfer !BMJK
! CHECK.FOR.WRITE.TO.CCBUF moved to after CCTMP to STKMQ transfer        !BMJK

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   WRITE.STKMQ.FROM.CCTMP:                                              ***
\***                                                                        ***
\***      Transfer all records from CCTMP to STKMQ, after first checking    ***
\***        the UOD on the CCUPF for any updates since the data was         ***
\***        downloaded into the LDT; (And it goes something like this;)     ***
\***      open the STKMQ and CCUPF files for access                         ***
\***      set flag to initially suppress type 27 txn production             ***
\***      perform loop to sequentially read all records in the CCTMP        ***
\***        attempt to read CCTMP, if read failed then set flag otherwise;  ***
\***        prepare STKMQ record by adding CR+LF to CCTMP record            ***
\***        extract UOD num and status depending if type 25, 26 or 28 record***
\***        if primer is present then change UOD number to zeros plus time  ***
\***          of DESPATCH to prevent re-processing - CNS                    ***
\***        attempt to read UOD from CCUPF file;                            ***
\***          if read failed then;                                          ***
\***            if status <> O then;                                        ***
\***              if transaction type is 25 or 26 assign next claim num     ***
\***              perform writes to STKMQ + CCUPF                           ***
\***            otherwise; write to STKMQ without hold or unlock and        ***
\***              if UOD was TA primer then write to CCUPF                  ***
\***          otherwise if read was successful;                             ***
\***            check for valid new status for STKMQ record (ie can only    ***
\***              be C->D ) and if valid and type 26 record; convert to     ***
\***              type 28 to despatch the UOD but lose the item level data  ***
\***            if valid status change then; write STKMQ and write          ***
\***              unlock CCUPF to update status change                      ***
\***            otherwise if invalid status change then write CCUPF unlock  ***
\***              to release record previously locked  - Phew!              ***
\***         if during any of the stages above a record is actually written ***
\***           to the STKMQ and it was a goods out transmission , then the  ***
\***           type 27 requirement flag should be changed                   ***
\***                                                                        ***
\***      If required then write an end of session record to the STKMQ      ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

WRITE.STKMQ.FROM.CCTMP:

       CURR.SESS.NUM% = CCUPF.SESS.NUM%                                ! BMJK
       IF END #CCUPF.SESS.NUM% THEN OPEN.ERROR                         ! BMJK
       OPEN CCUPF.FILE.NAME$ KEYED RECL CCUPF.RECL% AS CCUPF.SESS.NUM% ! BMJK
       CCUPF.OPEN.FLAG$ = "Y"                                          ! BMJK

       IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                           ! BMJK
          IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                      ! BMJK
          CURR.SESS.NUM% = STKMQ.SESS.NUM%                             ! BMJK
          OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512        \ BMJK
             READONLY APPEND                                           ! BMJK
          STKMQ.OPEN.FLAG$ = "Y"                                       ! BMJK
       ENDIF                                                           ! BMJK

       END.OF.CCTMP = FALSE                                            ! BMJK
       VALID.DATA.PRESENT = FALSE                                      ! BMJK

       WHILE (NOT END.OF.CCTMP)                                        ! BMJK

          CURR.SESS.NUM% = CCTMP.SESS.NUM%                             ! BMJK
          IF READ.CCTMP THEN END.OF.CCTMP = TRUE ELSE BEGIN            ! BMJK

             STKMQ.RECORD$ = CCTMP.RECORD$ + CCTMP.ENDREC.MARKER$      ! BMJK

             IF CCTMP.TRANS.TYPE$ = "26" THEN BEGIN                    ! BMJK
                CCUPF.UOD.NUM$ = MID$(CCTMP.RECORD$,10,7)              ! BMJK
                CCTMP.UOD.STATUS$ = MID$(CCTMP.RECORD$,17,1)           ! BMJK
             ENDIF ELSE BEGIN                                          ! BMJK
                IF CCTMP.TRANS.TYPE$ = "28" THEN BEGIN                 ! 1.16JAS
                   CCUPF.UOD.NUM$ = MID$(CCTMP.RECORD$,14,7)           ! BMJK
                   CCTMP.UOD.STATUS$ = MID$(CCTMP.RECORD$,21,1)        ! BMJK
                ENDIF ELSE BEGIN                                       ! BMJK ! 1.16JAS
                   CCUPF.UOD.NUM$ = PACK$("00000000000000")            ! 1.16JAS
                   CCTMP.UOD.STATUS$ = "D"                             ! 1.16JAS
                ENDIF                                                  ! 1.16JAS
             ENDIF                                                     ! 1.16JAS

             PRIMER.PRESENT = FALSE                                    ! BMJK
             IF MID$(CCUPF.UOD.NUM$,3,3) = PACK$("000000") THEN BEGIN  ! BMJK
                PRIMER.PRESENT = TRUE                                  ! BMJK
                IF LDT.RETURNS.TYPE$ = "G" THEN BEGIN                  ! 1.16JAS
                   TEMP$ = MID$(CCTMP.RECORD$,95,3)                       ! CNS
                   IF TEMP$ = PACK$("000000") THEN TEMP$ = PACK$("999999") ! CNS
                   CCUPF.UOD.NUM$ = PACK$(STRING$(8,"0")) + TEMP$         ! CNS
                ENDIF ELSE BEGIN                                       ! 1.16JAS
                   CCUPF.UOD.NUM$ = PACK$(STRING$(8,"0")) + PACK$("999999") ! 1.16JAS
                ENDIF                                                       ! 1.16JAS
             ENDIF                                                     ! BMJK

             CURR.SESS.NUM% = CCUPF.SESS.NUM%                          ! BMJK
             IF READ.CCUPF.LOCKED THEN BEGIN                           ! BMJK
                IF CCTMP.UOD.STATUS$ <> "O" THEN BEGIN                 ! BMJK
                   IF CCTMP.TRANS.TYPE$ = "26" OR                      \ 1.16JAS
                      CCTMP.TRANS.TYPE$ = "25" THEN                    \ BMJK ! 1.16JAS
                      GOSUB ASSIGN.NEXT.CREDIT.CLAIM.NUMBER            ! BMJK
                   CCUPF.CURRENT.STATUS$ = CCTMP.UOD.STATUS$           ! BMJK
                   IF PRIMER.PRESENT = FALSE THEN BEGIN                ! CNS
                      CURR.SESS.NUM% = CCUPF.SESS.NUM%                 ! BMJK
                      IF WRITE.CCUPF THEN GOTO WRITE.ERROR             ! BMJK
                   ENDIF                                               ! CNS
                   CURR.SESS.NUM% = STKMQ.SESS.NUM%                    ! BMJK
                   IF WRITE.STKMQ THEN GOTO WRITE.ERROR                ! BMJK
                   VALID.DATA.PRESENT = TRUE                           ! BMJK
                   GOSUB CHECK.FOR.WRITE.TO.CCBUF                      ! BMJK
                ENDIF ELSE BEGIN                                       ! BMJK
! 4 lines removed from here - CNS
                   CURR.SESS.NUM% = STKMQ.SESS.NUM%                    ! BMJK
                   IF WRITE.STKMQ THEN GOTO WRITE.ERROR                ! BMJK
                   VALID.DATA.PRESENT = TRUE                           ! BMJK
                ENDIF                                                  ! BMJK
             ENDIF ELSE BEGIN                                          ! BMJK
                IF ( CCUPF.CURRENT.STATUS$="C" AND                     \ BMJK
                   CCTMP.UOD.STATUS$="D" ) THEN BEGIN                  ! BMJK
                   IF CCTMP.TRANS.TYPE$ = "26" THEN                    \ BMJK
                      STKMQ.RECORD$ = CCTMP.RECORD.DELIMITER$ +        \ BMJK
                                      PACK$("28") +                    \ BMJK
                                      MID$(CCTMP.RECORD$,3,7) +        \ BMJK
                                      MID$(CCTMP.RECORD$,18,4) +       \ BMJK
                                      MID$(CCTMP.RECORD$,10,8) +       \ BMJK
                                      CCTMP.RECORD.DELIMITER$ +        \ BMJK
                                      CCTMP.ENDREC.MARKER$             ! BMJK
                   CURR.SESS.NUM% = STKMQ.SESS.NUM%                    ! BMJK
                   IF WRITE.STKMQ THEN GOTO WRITE.ERROR                ! BMJK
                   CURR.SESS.NUM% = CCUPF.SESS.NUM%                    ! BMJK
                   CCUPF.CURRENT.STATUS$ = CCTMP.UOD.STATUS$           ! BMJK
                   IF WRITE.UNLOCK.CCUPF THEN GOTO WRITE.ERROR         ! BMJK
                   VALID.DATA.PRESENT = TRUE                           ! BMJK
                ENDIF ELSE BEGIN                                       ! BMJK
                   IF WRITE.UNLOCK.CCUPF THEN GOTO WRITE.ERROR         ! BMJK
                ENDIF                                                  ! BMJK
             ENDIF                                                     ! BMJK
          ENDIF                                                        ! BMJK

       WEND

      ! If VALID data and Goods out transmission

       IF VALID.DATA.PRESENT AND                                       \ ! 1.16JAS
          LDT.RETURNS.TYPE$ = "G" THEN BEGIN                           ! BMJK 1.16JAS
          GOSUB UPDATE.CCUOD.WITH.LDT.NUMBER                           ! BMJK
             FINISH.DATE$ = DATE$
             FINISH.TIME$ = TIME$
             STKMQ.RECORD$ = CHR$(34) + PACK$("27") + CHR$(59) +          \
                             PACK$(FINISH.DATE$) +                        \
                             PACK$(FINISH.TIME$) +                        \
                             CHR$(34) + CHR$(0Dh) + CHR$(0Ah)
             CURRENT.KEY$ = ""
             CURR.SESS.NUM% = STKMQ.SESS.NUM%
             IF WRITE.STKMQ THEN GOTO WRITE.ERROR
       ENDIF                                                           ! BMJK

       CLOSE STKMQ.SESS.NUM%
       STKMQ.OPEN.FLAG$ = "N"

       CLOSE CCUPF.SESS.NUM%                                           ! BMJK
       CCUPF.OPEN.FLAG$ = "N"                                          ! BMJK

  RETURN

\******************************************************************************
\***
\***   ASSIGN.NEXT.CREDIT.CLAIM.NUMBER:
\***
\***     open CCTRL as DIRECT
\***     read record from file
\***     increment record and update record in file
\***     check for claim number exceeding 9999999; if so set to 00000001
\***     close CCTRL
\***
\***     If Goods out transmission insert new claim number into existing
\***        CCTMP type 26 record
\***     If credit claim transmission insert new claim number into existing
\***        CCTMP type 25 record
\***
\******************************************************************************

ASSIGN.NEXT.CREDIT.CLAIM.NUMBER:

       CURR.SESS.NUM% = CCTRL.SESS.NUM%
       IF END #CCTRL.SESS.NUM% THEN OPEN.ERROR
       OPEN CCTRL.FILE.NAME$ DIRECT RECL CCTRL.RECL% AS CCTRL.SESS.NUM%
       CCTRL.OPEN.FLAG$ = "Y"
       CCTRL.REC.NUM% = 1
       CURRENT.KEY$ = "1"
       IF READ.CCTRL.LOCKED THEN GOTO READ.ERROR
       CCTRL.CREDIT.CLAIM.NUM$ = PACK$(RIGHT$(STRING$(8,"0") +         \
                                 STR$(VAL(UNPACK$(                     \
                                 CCTRL.CREDIT.CLAIM.NUM$))+1),8))
       IF (VAL(UNPACK$(CCTRL.CREDIT.CLAIM.NUM$))>9999999) OR           \
          (VAL(UNPACK$(CCTRL.CREDIT.CLAIM.NUM$))=0) THEN               \
             CCTRL.CREDIT.CLAIM.NUM$ = PACK$("00000001")

       CCTRL.REC.NUM% = 1
       IF WRITE.UNLOCK.CCTRL THEN GOTO WRITE.ERROR
       CCTRL.OPEN.FLAG$ = "N"
       CLOSE CCTRL.SESS.NUM%

       ! Goods out transmission
       IF LDT.RETURNS.TYPE$ = "G" THEN BEGIN                              ! 1.16JAS
          STKMQ.RECORD$ = LEFT$(STKMQ.RECORD$,17) +                       \ BMJK
                         CCTRL.CREDIT.CLAIM.NUM$ +                        \ BMJK
                         RIGHT$(STKMQ.RECORD$,(LEN(STKMQ.RECORD$)-21))    ! BMJK
       ENDIF ELSE BEGIN                                                   ! 1.16JAS
          ! Credit Claim transmission
          STKMQ.RECORD$ = LEFT$(STKMQ.RECORD$, 9) +                       \ 1.16JAS
                         CCTRL.CREDIT.CLAIM.NUM$ +                        \ 1.16JAS
                         RIGHT$(STKMQ.RECORD$,(LEN(STKMQ.RECORD$)-13))    ! 1.16JAS
       ENDIF                                                              ! 1.16JAS

RETURN

\******************************************************************************
\***
\***   CHECK.FOR.WRITE.TO.CCBUF:
\***
\***      If Goods out transmission
\***      check for requirement using UOD number structure, if required then
\***        append record to the CCBUF
\***
\******************************************************************************

CHECK.FOR.WRITE.TO.CCBUF:

    ! Goods out transmission
    IF LDT.RETURNS.TYPE$ = "G" THEN BEGIN                              ! 1.16JAS
       IF MID$(CCUPF.UOD.NUM$,3,2) <> PACK$("0006") AND                \ BMJK
          MID$(CCUPF.UOD.NUM$,3,2) <> PACK$("0083") AND                \ BMJK
          (NOT PRIMER.PRESENT) THEN BEGIN                              ! BMJK

          CCBUF.RECS.PRESENT = TRUE                                    ! BMJK
          CCBUF.RECORD$ = STKMQ.RECORD$                                ! BMJK
          CURRENT.KEY$ = ""                                            ! BMJK
          CURR.SESS.NUM% = CCBUF.SESS.NUM%                             ! BMJK
          IF WRITE.CCBUF THEN GOTO WRITE.ERROR                         ! BMJK

       ENDIF                                                           ! BMJK
    ENDIF                                                              ! 1.16JAS

RETURN

\******************************************************************************
\***
\***   UPDATE.CCUOD.WITH.LDT.NUMBER:
\***
\***     close CCUOD if currently open
\***     open CCUOD as keyed
\***     read header record from file as LOCKED
\***     store current LDT number in file
\***     update record in file and release
\***     close CCUOD
\***
\******************************************************************************

UPDATE.CCUOD.WITH.LDT.NUMBER:

       IF CCUOD.OPEN.FLAG$="Y" THEN BEGIN
          CCUOD.OPEN.FLAG$ = "N"
          CLOSE CCUOD.SESS.NUM%
       ENDIF
       CURR.SESS.NUM% = CCUOD.SESS.NUM%
       IF END #CCUOD.SESS.NUM% THEN OPEN.ERROR
       OPEN CCUOD.FILE.NAME$ KEYED RECL CCUOD.RECL% AS CCUOD.SESS.NUM%                                        ! MMJK
       CCUOD.OPEN.FLAG$ = "Y"
       CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??"))
       CURRENT.KEY$ = UNPACK$(CCUOD.UOD.NUM$)
       IF READ.CCUOD.LOCKED THEN GOTO READ.ERROR
       CCUOD.LDT.NUM$ = PACK$(CURR.TERMINAL$)
       IF WRITE.UNLOCK.CCUOD THEN GOTO WRITE.ERROR
       CCUOD.OPEN.FLAG$ = "N"
       CLOSE CCUOD.SESS.NUM%

RETURN

\******************************************************************************
\***
\***   DIRECT.EXTRACT.CCUOD.KEYS:
\***
\***     close CCUOD if already open
\***     open CCUOD for direct access
\***     process each sector extracting any open,closed or cancelled UODs
\***       store all extracts in a table
\***       KEEP A SEPARATE COUNT OF CANCELLED UODS       1.4
\***     complete loop
\***     close file
\***
\******************************************************************************

    DIRECT.EXTRACT.CCUOD.KEYS:

      MAX.TABLE.SIZE% = 200
      DIM CCUOD.TABLE$(MAX.TABLE.SIZE%)
      RECS.PER.SECTOR% = (508/CCUOD.RECL%)
      SECTOR% = 2
      NUM.STATUS.O.C.X.UODS% = 0                                          !1.4
      NUM.OF.CANCELLED.UODS% = 0                                          !1.4
      TABLE.FULL = FALSE
      END.OF.FILE = FALSE

      CURR.SESS.NUM% = CCUOD.SESS.NUM%
      IF CCUOD.OPEN.FLAG$ = "Y" THEN BEGIN
        CCUOD.OPEN.FLAG$ = "N"
       CLOSE CCUOD.SESS.NUM%
      ENDIF
      IF END #CCUOD.SESS.NUM% THEN OPEN.ERROR
      OPEN CCUOD.FILE.NAME$ DIRECT RECL 512 AS CCUOD.SESS.NUM%
      CCUOD.OPEN.FLAG$ = "Y"

      WHILE (NOT END.OF.FILE) AND (NOT TABLE.FULL)
        IF END #CCUOD.SESS.NUM% THEN END.OF.CCUOD.DIRECT
        READ FORM "C4,C508";#CCUOD.SESS.NUM%,SECTOR%;FILLER$,SECTOR$
       IF LEFT$(SECTOR$,7)<>PACK$(STRING$(14,"0")) THEN BEGIN
         INDEX% = 1
         WHILE (INDEX%<(RECS.PER.SECTOR%*CCUOD.RECL%)) AND (NOT TABLE.FULL)
           CCUOD.KEY$ = MID$(SECTOR$,INDEX%,7)
           CCUOD.STATUS$ = MID$(SECTOR$,INDEX%+7,1)
           IF CCUOD.KEY$ = PACK$(STRING$(14,"0")) THEN INDEX% = 508       \
            ELSE IF (CCUOD.STATUS$ = "O" OR CCUOD.STATUS$ = "C" OR        \
                     CCUOD.STATUS$ = "X")                             \
                AND CCUOD.KEY$ <> PACK$(STRING$(14,"?")) THEN BEGIN
                  IF CCUOD.STATUS$ = "X" THEN BEGIN                       ! 1.4
                     NUM.OF.CANCELLED.UODS% = NUM.OF.CANCELLED.UODS% + 1 ! 1.4
                  ENDIF                                                   ! 1.4
                  NUM.STATUS.O.C.X.UODS% = NUM.STATUS.O.C.X.UODS% + 1     ! 1.4
                  IF NUM.STATUS.O.C.X.UODS%>MAX.TABLE.SIZE% THEN TABLE.FULL=TRUE \1.4
                 ELSE CCUOD.TABLE$(NUM.STATUS.O.C.X.UODS%) = CCUOD.KEY$          !1.4
                ENDIF
           INDEX% = INDEX% + CCUOD.RECL%
         WEND
       ENDIF
       SECTOR% = SECTOR% + 1
        END.OF.CCUOD.DIRECT.RETURN:
      WEND

      CCUOD.OPEN.FLAG$ = "N"
      CLOSE CCUOD.SESS.NUM%

      IF TABLE.FULL THEN BEGIN
       SB.UNIQUE$ = "CCUOD KEYS"
        GOSUB LOG.TABLE.FULL.ERROR
      ENDIF

    RETURN


    END.OF.CCUOD.DIRECT:


       END.OF.FILE = TRUE

       GOTO END.OF.CCUOD.DIRECT.RETURN


\******************************************************************************
\***
\***   DIRECT.EXTRACT.CCLAM.KEYS:     !1.4
\***
\***     close CCLAM if already open
\***     open CCLAM for direct access
\***     process each sector extracting any despatched UODs (not including
\***       primer UODs) and store extracts in a table.
\***     complete loop
\***     close file
\***
\******************************************************************************

    DIRECT.EXTRACT.CCLAM.KEYS:

      MAX.TABLE.SIZE% = 1600                   ! Was 400                          1.19CS
                                               ! CCLAM is keyed & holds 2000
                                               ! records. Max table size needs
                                               ! to be 20% less ie. 1600 to 
                                               ! prevent performance errors.
      DIM CCLAM.TABLE$(MAX.TABLE.SIZE%)
      RECS.PER.SECTOR% = (508/CCLAM.RECL%)
      SECTOR% = 2
      NUM.OF.DESPATCHED.UODS% = 0
      TABLE.FULL = FALSE
      END.OF.FILE = FALSE

      CURR.SESS.NUM% = CCLAM.SESS.NUM%
      IF CCLAM.OPEN.FLAG$ = "Y" THEN BEGIN
        CCLAM.OPEN.FLAG$ = "N"
       CLOSE CCLAM.SESS.NUM%
      ENDIF
      IF END #CCLAM.SESS.NUM% THEN OPEN.ERROR
      OPEN CCLAM.FILE.NAME$ DIRECT RECL 512 AS CCLAM.SESS.NUM%
      CCLAM.OPEN.FLAG$ = "Y"

      WHILE (NOT END.OF.FILE) AND (NOT TABLE.FULL)
        IF END #CCLAM.SESS.NUM% THEN END.OF.CCLAM.DIRECT
        READ FORM "C4,C508";#CCLAM.SESS.NUM%,SECTOR%;FILLER$,SECTOR$
       IF LEFT$(SECTOR$,4)<>PACK$(STRING$(8,"0")) THEN BEGIN
         INDEX% = 1
         WHILE (INDEX%<(RECS.PER.SECTOR%*CCLAM.RECL%)) AND (NOT TABLE.FULL)
           CCLAM.KEY$ = MID$(SECTOR$,INDEX%,4)
           IF CCLAM.KEY$ = PACK$(STRING$(8,"0")) THEN BEGIN
              INDEX% = 508
           ENDIF ELSE BEGIN
              IF (CCLAM.KEY$ <> PACK$(STRING$(8,"?"))) THEN BEGIN
                 ! Do not include primer UODs (starting with 10 zeroes)
                 IF MID$(SECTOR$,INDEX%+4,5) <> PACK$("0000000000") THEN BEGIN
                    NUM.OF.DESPATCHED.UODS% = NUM.OF.DESPATCHED.UODS% + 1
                    IF NUM.OF.DESPATCHED.UODS% > MAX.TABLE.SIZE% THEN BEGIN
                       TABLE.FULL=TRUE
                    ENDIF ELSE BEGIN
                          CCLAM.TABLE$(NUM.OF.DESPATCHED.UODS%) = CCLAM.KEY$
                    ENDIF
                 ENDIF
              ENDIF
           ENDIF
           INDEX% = INDEX% + CCLAM.RECL%
         WEND
       ENDIF
       SECTOR% = SECTOR% + 1
        END.OF.CCLAM.DIRECT.RETURN:
      WEND

      CCLAM.OPEN.FLAG$ = "N"
      CLOSE CCLAM.SESS.NUM%

      IF TABLE.FULL THEN BEGIN
       SB.UNIQUE$ = "CCLAM KEYS"
        GOSUB LOG.TABLE.FULL.ERROR
      ENDIF

    RETURN


    END.OF.CCLAM.DIRECT:

       END.OF.FILE = TRUE

       GOTO END.OF.CCLAM.DIRECT.RETURN

\******************************************************************************
\***
\***    CREATE.CCWKF.AND.WRITE.HEADER
\***
\***       close CCWKF if already open
\***       create CCWKF as sequential readonly
\***
\***       clear item storage variable for extracted items
\***
\***       format CCWKF header record and write to CCWKF
\***
\***       Only send down the number of open/closed UODs not the number  1.4
\***       of cancelled/despatched UODs.                                 1.4
\***
\***       initialise item and barcode storage tables
\***
\******************************************************************************

   CREATE.CCWKF.AND.WRITE.HEADER:

      CURR.SESS.NUM% = CCWKF.SESS.NUM%
      IF CCWKF.OPEN.FLAG$ = "Y" THEN BEGIN
        CCWKF.OPEN.FLAG$ = "N"
         CLOSE CCWKF.SESS.NUM%
      ENDIF

      IF END #CCWKF.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE CCWKF.FILE.NAME$ AS CCWKF.SESS.NUM%               \
             BUFFSIZE 512 READONLY MIRRORED ATCLOSE
      CCWKF.OPEN.FLAG$ = "Y"

      CRLF$ = CHR$(0DH) + CHR$(0AH)
      ITEM.STORAGE$ = ""
      ITEM.DESC.STORAGE$ = ""
      BARCODE.STORAGE$ = ""
      BARCODE.INDEX.STORAGE$ = ""
      NUM.ITEMS% = 0
      NUM.BARCODES% = 0

      NUM.OF.OPEN.CLOSED.UODS% = NUM.STATUS.O.C.X.UODS% -             \ 1.4
                                 NUM.OF.CANCELLED.UODS%               ! 1.4

      CCWKF.RECORD$ = "RE" + RIGHT$("0000"+STR$(NUM.OF.OPEN.CLOSED.UODS%),4) !1.4

      CURRENT.KEY$ = ""
      IF WRITE.CCWKF THEN GOTO WRITE.ERROR

   RETURN

\******************************************************************************
\***
\***    PROCESS.UOD.RECORDS:  !1.4 title changed
\***
\***      open keyed files for use in routine
\***
\***      initialise table pointer
\***      perform loop to process contents of CCUOD table
\***        read record from CCUOD
\***        call routine to write UOD header record to CCWKF
\***        perform loop to extract all relevent items from CCITM
\***          create key and attempt to read item from CCITM
\***          if item code is present then call routine to process item code
\***           and store in string to be added to CCWKF later
\***          increment item counter
\***        complete loop to process all items
\***      complete loop to process CCUOD table
\***
\***
\***      close CCUOD and CCITM                                         1.4
\***
\***      loop through CCLAM table and extract claims for writing to    1.4
\***      CCWKF.                                                        1.4
\***
\***
\***      close CCLAM                                                   1.4
\***
\***      call routine to write items to CCWKF
\***      call routine to sort and write any barcodes to CCWKF
\***
\***      call routine to format and write UOD trailer to CCWKF
\***
\******************************************************************************

     PROCESS.UOD.RECORDS:                                               !1.4

       CURR.SESS.NUM% = CCUOD.SESS.NUM%
       IF CCUOD.OPEN.FLAG$ = "Y" THEN BEGIN
        CCUOD.OPEN.FLAG$ = "N"
         CLOSE CCUOD.SESS.NUM%
       ENDIF
       IF END #CCUOD.SESS.NUM% THEN OPEN.ERROR
       OPEN CCUOD.FILE.NAME$ KEYED RECL CCUOD.RECL% AS CCUOD.SESS.NUM%                                        ! MMJK
       CCUOD.OPEN.FLAG$ = "Y"

       CURR.SESS.NUM% = CCITM.SESS.NUM%
       IF CCITM.OPEN.FLAG$ = "Y" THEN BEGIN
        CCITM.OPEN.FLAG$ = "N"
         CLOSE CCITM.SESS.NUM%
       ENDIF
       IF END #CCITM.SESS.NUM% THEN OPEN.ERROR
       OPEN CCITM.FILE.NAME$ KEYED RECL CCITM.RECL% AS CCITM.SESS.NUM%                                        ! MMJK
       CCITM.OPEN.FLAG$ = "Y"

       CURR.SESS.NUM% = IDF.SESS.NUM%
       IF IDF.OPEN.FLAG$ = "Y" THEN BEGIN
        IDF.OPEN.FLAG$ = "N"
         CLOSE IDF.SESS.NUM%
       ENDIF
       IF END #IDF.SESS.NUM% THEN OPEN.ERROR
       OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%                                        ! MMJK
       IDF.OPEN.FLAG$ = "Y"

       CURR.SESS.NUM% = IEF.SESS.NUM%
       IF IEF.OPEN.FLAG$ = "Y" THEN BEGIN
        IEF.OPEN.FLAG$ = "N"
         CLOSE IEF.SESS.NUM%
       ENDIF
       IF END #IEF.SESS.NUM% THEN OPEN.ERROR
       OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%                                        ! MMJK
       IEF.OPEN.FLAG$ = "Y"
       CURR.SUBSCRIPT% = 1
       NUM.BARCODES% = 0
       WHILE CURR.SUBSCRIPT% <= NUM.STATUS.O.C.X.UODS%              ! 1.4
         CCUOD.UOD.NUM$ = CCUOD.TABLE$(CURR.SUBSCRIPT%)
        CURR.SESS.NUM% = CCUOD.SESS.NUM%
        CURRENT.KEY$ = UNPACK$(CCUOD.UOD.NUM$)
         IF READ.CCUOD THEN BEGIN
          RE.CHAIN = TRUE
          RECEIVE.STATE$ = "?"
          GOTO READ.ERROR
        ENDIF
         GOSUB WRITE.CCWKF.OPEN.CLOSED.HEADER                         ! 1.4
         IF CCUOD.NUM.OF.ITEMS%>0 THEN BEGIN
           CURR.ITEM% = 1
          WHILE CURR.ITEM% <= CCUOD.NUM.OF.ITEMS%
            CCITM.KEY$ = CCUOD.UOD.NUM$ +                             \
                         PACK$(FN.Z.PACK(STR$(CURR.ITEM%),4))
            CURR.SESS.NUM% = CCITM.SESS.NUM%
            CURRENT.KEY$ = UNPACK$(CCUOD.UOD.NUM$)
             IF READ.CCITM <> 1 THEN GOSUB PROCESS.CCITM.RECORD        \
              ELSE BEGIN
                RE.CHAIN = TRUE
               RECEIVE.STATE$ = "?"
               GOTO READ.ERROR
              ENDIF
             CURR.ITEM% = CURR.ITEM% + 1
          WEND
        ENDIF
         CURR.SUBSCRIPT% = CURR.SUBSCRIPT% + 1
       WEND

      IF CCUOD.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.4
         CCUOD.OPEN.FLAG$ = "N"                                       ! 1.4
         CLOSE CCUOD.SESS.NUM%                                        ! 1.4
      ENDIF                                                           ! 1.4

      IF CCITM.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.4
         CCITM.OPEN.FLAG$ = "N"                                       ! 1.4
         CLOSE CCITM.SESS.NUM%                                        ! 1.4
      ENDIF                                                           ! 1.4

      CURR.SUBSCRIPT% = 1                                             ! 1.4
      CURR.SESS.NUM% = CCLAM.SESS.NUM%                                ! 1.4
      IF CCLAM.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.4
         CCLAM.OPEN.FLAG$ = "N"                                       ! 1.4
         CLOSE CCLAM.SESS.NUM%                                        ! 1.4
      ENDIF                                                           ! 1.4
      IF END #CCLAM.SESS.NUM% THEN OPEN.ERROR                         ! 1.4
      OPEN CCLAM.FILE.NAME$ KEYED RECL CCLAM.RECL% AS CCLAM.SESS.NUM% ! 1.4                                       ! MMJK
      CCLAM.OPEN.FLAG$ = "Y"                                          ! 1.4
      WHILE CURR.SUBSCRIPT% <= NUM.OF.DESPATCHED.UODS%                ! 1.4
            CCLAM.CREDIT.CLAIM.NUM$ = CCLAM.TABLE$(CURR.SUBSCRIPT%)   ! 1.4
            CURRENT.KEY$ = UNPACK$(CCLAM.CREDIT.CLAIM.NUM$)           ! 1.4
            IF READ.CCLAM THEN BEGIN                                  ! 1.4
               RE.CHAIN = TRUE                                        ! 1.4
               RECEIVE.STATE$ = "?"                                   ! 1.4
               GOTO READ.ERROR                                        ! 1.4
            ENDIF                                                     ! 1.4
            GOSUB WRITE.CCWKF.DESPATCHED.HEADER                       ! 1.4
            CURR.SUBSCRIPT% = CURR.SUBSCRIPT% + 1                     ! 1.4
      WEND                                                            ! 1.4

      IF CCLAM.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.4
         CCLAM.OPEN.FLAG$ = "N"                                       ! 1.4
         CLOSE CCLAM.SESS.NUM%                                        ! 1.4
      ENDIF                                                           ! 1.4


      GOSUB PROCESS.ITEM.STORAGE
      GOSUB PROCESS.BARCODE.TABLE

      IDF.OPEN.FLAG$ = "N"
      CLOSE IDF.SESS.NUM%
      IEF.OPEN.FLAG$ = "N"
      CLOSE IEF.SESS.NUM%

    RETURN

\******************************************************************************
\***
\***   WRITE.CCWKF.OPEN.CLOSED.HEADER:  !1.4 title changed
\***
\***      prepare the UOD header to be written to the CCWKF
\***      write record and trap errors
\***
\******************************************************************************

    WRITE.CCWKF.OPEN.CLOSED.HEADER:                                     !1.4

      CCWKF.RECORD$ = "RH" +                                            \
                     UNPACK$(CCUOD.CREDIT.CLAIM.NUM$) +               \
                     UNPACK$(CCUOD.UOD.NUM$) +                      \
                    CCUOD.STATUS$ +                                   \
                      FN.Z.PACK(STR$(CCUOD.NUM.OF.ITEMS%),4) +               \
                    FN.INVERT.DATE(UNPACK$(CCUOD.DATE.UOD.OPENED$)) + \
                    CCUOD.SUPPLY.ROUTE$ +                            \
                    CCUOD.DISP.LOCATION$ +                             \
                    CCUOD.BC.LETTER$ +                            \
                    CCUOD.RECALL.NUM$ +                             \
                    CCUOD.AUTHORISATION$ +                            \
                    CCUOD.SUPPLIER$ +                             \
                    RIGHT$(UNPACK$(CCUOD.METHOD.OF.RETURN$),1) +       \
                    RIGHT$(UNPACK$(CCUOD.CARRIER$),1) +               \
                    CCUOD.BIRD.NUM$ +                             \
                    UNPACK$(CCUOD.REASON.NUM$) +                      \
                    UNPACK$(CCUOD.RECEIVING.STORE$) +               \
                    RIGHT$(UNPACK$(CCUOD.DESTINATION$),1) +               \
                    CCUOD.WAREHOUSE.ROUTE$ +                             \
                    RIGHT$(UNPACK$(CCUOD.UOD.TYPE$),1) +               \
                    RIGHT$(UNPACK$(CCUOD.DAMAGE.REASON$),1)

      CURR.SESS.NUM% = CCWKF.SESS.NUM%
      CURRENT.KEY$ = ""
      IF WRITE.CCWKF THEN GOTO WRITE.ERROR

    RETURN


\******************************************************************************
\***
\***   WRITE.CCWKF.DESPATCHED.HEADER:  !1.4 NEW SUBROUTINE
\***
\***      prepare the UOD header to be written to the CCWKF
\***      write record and trap errors
\***
\******************************************************************************

    WRITE.CCWKF.DESPATCHED.HEADER:

      IF CCLAM.CANC.MARKER$ = "Y" THEN BEGIN
         STATUS$ = "X"
      ENDIF ELSE BEGIN
            STATUS$ = "D"
      ENDIF

      CCWKF.RECORD$ = "RH" +                                            \
                    UNPACK$(CCLAM.CREDIT.CLAIM.NUM$) +               \
                    UNPACK$(CCLAM.UOD.NUM$) +                      \
                    STATUS$ +                                   \
                    FN.Z.PACK(STR$(CCLAM.NUM.OF.ITEMS%),4) +               \
                    FN.INVERT.DATE(UNPACK$(CCLAM.DATE.OF.CLAIM$)) + \
                    CCLAM.SUPPLY.ROUTE$ +                            \
                    CCLAM.DISP.LOCATION$ +                             \
                    CCLAM.BC.LETTER$ +                            \
                    CCLAM.RECALL.NUM$ +                             \
                    CCLAM.AUTHORISATION$ +                            \
                    CCLAM.SUPPLIER$ +                             \
                    RIGHT$(UNPACK$(CCLAM.METHOD.OF.RETURN$),1) +       \
                    RIGHT$(UNPACK$(CCLAM.CARRIER$),1) +               \
                    CCLAM.BIRD.NUM$ +                             \
                    UNPACK$(CCLAM.REASON.NUM$) +                      \
                    UNPACK$(CCLAM.RECEIVING.STORE$) +               \
                    RIGHT$(UNPACK$(CCLAM.DESTINATION$),1) +               \
                    CCLAM.WAREHOUSE.ROUTE$ +                             \
                    RIGHT$(UNPACK$(CCLAM.UOD.TYPE$),1) +               \
                    RIGHT$(UNPACK$(CCLAM.DAMAGE.REASON$),1)

      CURR.SESS.NUM% = CCWKF.SESS.NUM%
      CURRENT.KEY$ = ""
      IF WRITE.CCWKF THEN GOTO WRITE.ERROR

    RETURN

\******************************************************************************
\***
\***   PROCESS.CCITM.RECORD:
\***
\***     check item record for item code present as opposed to barcode;
\***       set up key and attempt to read record from IDF for description
\***       if read was successful;
\***         store description for later use
\***         if item code is not parent of family then attempt to read IDF
\***           using parent code
\***         if first code was parent or new parent record was retrieved then;
\***           store IDF barcodes in table with reference to items position
\***           if codes are present in IEF (>3 codes) then;
\***             call READ.NEXT.IEF to read next barcode
\***             if read was successful; store barcodes in table
\***           complete loop
\***
\***      create item detail record and move to item storage area
\***
\******************************************************************************

    PROCESS.CCITM.RECORD:

       ITEM.POS% = ITEM.POS% + 1
       UOD.POS.STRING$ = FN.Z.PACK(STR$(CURR.SUBSCRIPT%),3)
       ITEM.POS.STRING$ = FN.Z.PACK(STR$(ITEM.POS%),4)
       IDF.STNDRD.DESC$ = "* ITEM NOT ON FILE *    "

       IF CCITM.BOOTS.BAR.CODE.FLAG$ = "I" THEN BEGIN                  ! BMJK
         IDF.NO.OF.BAR.CODES$ = PACK$("00")
         IDF.BOOTS.CODE$ = RIGHT$(CCITM.BOOTS.BAR.CODE$,4)
         CURR.SESS.NUM% = IDF.SESS.NUM%
         CURRENT.KEY$ = IDF.BOOTS.CODE$
         IF (NOT READ.IDF) THEN BEGIN                                   ! BMJK
          IF CCUOD.STATUS$ = "O" THEN BEGIN                            ! BMJK
             IDF.NUM.BARCODES% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))
            BARCODE.DATA$ = "0" + UNPACK$(IDF.FIRST.BAR.CODE$)
            BARCODE.INDEX$ = UOD.POS.STRING$ + ITEM.POS.STRING$
              GOSUB ADD.TO.BARCODE.STORAGE
            IF IDF.NUM.BARCODES% > 1 THEN BEGIN
              BARCODE.DATA$ = "0" + UNPACK$(IDF.SECOND.BAR.CODE$)
              BARCODE.INDEX$ = UOD.POS.STRING$ + ITEM.POS.STRING$
              GOSUB ADD.TO.BARCODE.STORAGE
            ENDIF
             IF IDF.NUM.BARCODES% > 2 THEN BEGIN
              F11.CURRENT.COUNT% = 1
              F11.NEXT.BAR.CODE$ = IDF.SECOND.BAR.CODE$
              MORE.BAR.CODES = TRUE
              CURR.SESS.NUM% = IEF.SESS.NUM%
              WHILE MORE.BAR.CODES
                RC% = READ.NEXT.IEF(F11.NEXT.BAR.CODE$,                     \
                                         NUM.BARCODES%,                       \
                                  RIGHT$(UNPACK$(IDF.BOOTS.CODE$),7))
                IF UNPACK$(F11.NEXT.BAR.CODE$) = STRING$(12,"0")            \
                  OR RC% <> 0 THEN MORE.BAR.CODES = FALSE              \
                ELSE BEGIN
                  BARCODE.DATA$ = "0" + UNPACK$(F11.NEXT.BAR.CODE$)
                   BARCODE.INDEX$ = UOD.POS.STRING$ + ITEM.POS.STRING$
                  GOSUB ADD.TO.BARCODE.STORAGE
                ENDIF
              WEND
             ENDIF
           ENDIF
         ENDIF ELSE IDF.STNDRD.DESC$ = "* ITEM NOT ON FILE *    "             ! BMJK
       ENDIF

       CCITM.BOOTS.BAR.CODE$ = RIGHT$(UNPACK$(CCITM.BOOTS.BAR.CODE$),13)
       ITEM.DATA$ = CCITM.BOOTS.BAR.CODE.FLAG$ +                      \
                  CCITM.BOOTS.BAR.CODE$ +                            \
                  FN.Z.PACK(STR$(CCITM.QTY%),4)                       ! BMJK
       ITEM.DATA.DESC$ = IDF.STNDRD.DESC$ +                            \
                           UOD.POS.STRING$
       GOSUB ADD.TO.ITEM.STORAGE

    RETURN

\******************************************************************************
\***
\***   ADD.TO.BARCODE.STORAGE:
\***
\***     increment barcode counter
\***     add data onto end of current storage string
\***     check for overflow of barcode storage, abort program if found
\***
\******************************************************************************

    ADD.TO.BARCODE.STORAGE:

       NUM.BARCODES% = NUM.BARCODES% + 1
       IF NUM.BARCODES%>5000 THEN BEGIN
        SB.UNIQUE$ = "BC STORAGE"
        GOSUB LOG.TABLE.FULL.ERROR
       ENDIF
       BARCODE.TABLE$(NUM.BARCODES%) = BARCODE.DATA$
       BARCODE.POS.TABLE$(NUM.BARCODES%) = BARCODE.INDEX$

    RETURN

\******************************************************************************
\***
\***   ADD.TO.ITEM.STORAGE:
\***
\***     increment item counter
\***     add data onto end of current storage string
\***     check for overflow of current storage string, increment storage if req
\***     check for overflow of all storage areas, abort program if found
\***
\******************************************************************************

    ADD.TO.ITEM.STORAGE:

       NUM.ITEMS% = NUM.ITEMS% + 1
       ITEM.STORAGE$ = ITEM.STORAGE$ + ITEM.DATA$
       ITEM.DESC.STORAGE$ = ITEM.DESC.STORAGE$ + ITEM.DATA.DESC$
       IF NUM.ITEMS% > 2400 THEN BEGIN
        SB.UNIQUE$ = "IT STORAGE"
        GOSUB LOG.TABLE.FULL.ERROR
       ENDIF

    RETURN

\******************************************************************************
\***
\***   PROCESS.ITEM.STORAGE:
\***
\***     process storage area sequentially, writing each record to the CCWKF
\***
\******************************************************************************

    PROCESS.ITEM.STORAGE:

      IF NUM.ITEMS%>0 THEN BEGIN
        ITEM.POS% = 1
        WHILE ITEM.POS%<=NUM.ITEMS%
          CCWKF.RECORD$ = "RD" +                                    \BMJK
                         MID$(ITEM.STORAGE$,1+((ITEM.POS%-1)*18),18) +       \BMJK
                          MID$(ITEM.DESC.STORAGE$,1+((ITEM.POS%-1)*27),27)!BMJK
          CURRENT.KEY$ = ""
         CURR.SESS.NUM% = CCWKF.SESS.NUM%
          IF WRITE.CCWKF THEN GOTO WRITE.ERROR
         ITEM.POS% = ITEM.POS% + 1
       WEND
      ENDIF

    RETURN

\******************************************************************************
\***
\***   PROCESS.BARCODE.TABLE:
\***
\***     if >1 entry sort contents into numerical order
\***     process table sequentially, writing each record to the CCWKF
\***
\******************************************************************************

    PROCESS.BARCODE.TABLE:

       IF NUM.BARCODES%>0 THEN BEGIN
         IF NUM.BARCODES%>1 THEN GOSUB SORT.BARCODE.TABLES
        GOSUB WRITE.BARCODES.TO.CCWKF
       ENDIF

    RETURN

\******************************************************************************
\***
\***   SORT.BARCODE.TABLES:
\***
\***      sort bar code table and corresponding item position table
\***      into ascending order using COMBSORT.
\***
\******************************************************************************

    SORT.BARCODE.TABLES:

       MAX% = NUM.BARCODES%
       FINISHED = FALSE

       GAP = MAX%
       WHILE NOT FINISHED
         GAP = INT(GAP / 1.3)
         IF GAP = 0 THEN GAP = 1
         IF GAP = 9 OR GAP = 10 THEN GAP = 11
         SWAPPED = FALSE
         FOR A%=1 TO (MAX% - GAP)
           B% = A% + GAP
           IF (BARCODE.TABLE$(A%)+BARCODE.POS.TABLE$(A%)) >               \ BMJK
             (BARCODE.TABLE$(B%)+BARCODE.POS.TABLE$(B%)) THEN BEGIN    ! BMJK
             TEMP$ = BARCODE.TABLE$(A%)
            BARCODE.TABLE$(A%) = BARCODE.TABLE$(B%)
             BARCODE.TABLE$(B%) = TEMP$
             TEMP$ = BARCODE.POS.TABLE$(A%)
            BARCODE.POS.TABLE$(A%) = BARCODE.POS.TABLE$(B%)
             BARCODE.POS.TABLE$(B%) = TEMP$
             SWAPPED = TRUE
          ENDIF
         NEXT A%
         IF (NOT SWAPPED) AND GAP = 1 THEN FINISHED = TRUE
       WEND

    RETURN

\******************************************************************************
\***
\***   WRITE.BARCODES.TO.CCWKF:
\***
\***     perform loop to process contents of barcode holding table
\***       format CCWKF record and write to file
\***     complete loop
\***
\******************************************************************************

    WRITE.BARCODES.TO.CCWKF:

      CURR.REC% = 1
      WHILE CURR.REC% <= NUM.BARCODES%
        CCWKF.RECORD$ = "RB" +                                          \
                          BARCODE.TABLE$(CURR.REC%) +                      \
                     BARCODE.POS.TABLE$(CURR.REC%)
        CURRENT.KEY$ = ""
        CURR.SESS.NUM% = CCWKF.SESS.NUM%
        IF WRITE.CCWKF THEN GOTO WRITE.ERROR
        CURR.REC% = CURR.REC% + 1
      WEND

    RETURN

\******************************************************************************
\***
\***   EXTRACT.ALL.BUSINESS.CENTRES:
\***
\***     open BCSMF as direct
\***     perform loop to read records out of file sequentially
\***       if record is present that is not null or spaces for BC then;
\***         format and write record to CCWKF
\***     complete loop
\***     close file
\***
\******************************************************************************

    EXTRACT.ALL.BUSINESS.CENTRES:

      IF BCSMF.OPEN.FLAG$ = "Y"THEN BEGIN
        BCSMF.OPEN.FLAG$ = "N"
       CLOSE BCSMF.SESS.NUM%
      ENDIF

      CURRENT.KEY$ = ""
      CURR.SESS.NUM% = BCSMF.SESS.NUM%
      IF END #BCSMF.SESS.NUM% THEN OPEN.ERROR
      OPEN BCSMF.FILE.NAME$ RECL 512 AS BCSMF.SESS.NUM%       NOWRITE NODEL
      BCSMF.OPEN.FLAG$ = "Y"

      NUM.SECTORS% = SIZE(BCSMF.FILE.NAME$)/512
      NUM.RECORDS% = 508 / 33
      NUM.BC.EXTRACTED% = 0

      SECTOR% = 2
      WHILE SECTOR%<=NUM.SECTORS%
        READ FORM "T5,C508";#BCSMF.SESS.NUM%,SECTOR%;SECTOR$
        REC.NUM% = 1
       WHILE (REC.NUM%<=NUM.RECORDS%)
          BCSMF.RECORD$ = LEFT$(SECTOR$,33)
          SECTOR$ = RIGHT$(SECTOR$,LEN(SECTOR$)-33)
          IF LEFT$(BCSMF.RECORD$,1)=PACK$("00") THEN                      \
           REC.NUM%=NUM.RECORDS%                                   \
         ELSE IF (LEFT$(BCSMF.RECORD$,1)<>" ") AND                      \
                 (MID$(BCSMF.RECORD$,29,1)<>"Y") THEN                      \
           GOSUB WRITE.BC.RECORD.TO.CCWKF
         REC.NUM% = REC.NUM% + 1
        WEND
      SECTOR% = SECTOR% + 1
      WEND

      BCSMF.OPEN.FLAG$ = "N"
      CLOSE BCSMF.SESS.NUM%

    RETURN

\******************************************************************************
\***
\***   WRITE.BC.RECORD.TO.CCWKF:
\***
\***     format record for writing to CCWKF
\***     attempt to write record to file
\***
\******************************************************************************

     WRITE.BC.RECORD.TO.CCWKF:

       CCWKF.RECORD$ = "BC" +                                           \
                     LEFT$(BCSMF.RECORD$,15)
       CURRENT.KEY$ = ""
       CURR.SESS.NUM% = CCWKF.SESS.NUM%
       IF WRITE.CCWKF THEN GOTO WRITE.ERROR
       NUM.BC.EXTRACTED% = NUM.BC.EXTRACTED% + 1

     RETURN

\******************************************************************************
\***
\***   WRITE.CCWKF.TRAILER.AND.CLOSE.FILE:
\***
\***     close CCTRL file if already open
\***     open CCTRL and extract current credit claiming number
\***     close CCTRL after use
\***
\***     format CCWKF record and write to file
\***
\***     issue close to CCWKF
\***
\******************************************************************************

    WRITE.CCWKF.TRAILER.AND.CLOSE.FILE:

      IF CCTRL.OPEN.FLAG$ = "Y" THEN BEGIN
        CCTRL.OPEN.FLAG$ = "N"
       CLOSE CCTRL.SESS.NUM%
      ENDIF

      CURRENT.KEY$ = ""
      CURR.SESS.NUM% = CCTRL.SESS.NUM%
      IF END #CCTRL.SESS.NUM% THEN OPEN.ERROR
      OPEN CCTRL.FILE.NAME$ DIRECT RECL CCTRL.RECL% AS CCTRL.SESS.NUM% !BMJK
      CCTRL.OPEN.FLAG$ = "Y"
      CCTRL.REC.NUM% = 1
      CURRENT.KEY$ = "1"
      IF READ.CCTRL THEN GOTO READ.ERROR

      CCTRL.OPEN.FLAG$ = "N"
      CLOSE CCTRL.SESS.NUM%

      NUM.OF.UODS.TRANSMITTED% = NUM.STATUS.O.C.X.UODS% +               \1.4
                                 NUM.OF.DESPATCHED.UODS%                !1.4
      CCWKF.RECORD$ = "RR" +                                            \
                    FN.Z.PACK(STR$(NUM.OF.UODS.TRANSMITTED%),3) +       \1.4
                    FN.Z.PACK(STR$(NUM.ITEMS%),4) +                     \
                    FN.Z.PACK(STR$(NUM.BARCODES%),4) +                  \
                       FN.Z.PACK(STR$(NUM.BC.EXTRACTED%),2) +                      \
                    UNPACK$(CCTRL.CREDIT.CLAIM.NUM$)
      CURRENT.KEY$ = ""
      CURR.SESS.NUM% = CCWKF.SESS.NUM%
      IF WRITE.CCWKF THEN GOTO WRITE.ERROR

      CCWKF.OPEN.FLAG$ = "N"
      CLOSE CCWKF.SESS.NUM%

    RETURN

\******************************************************************************
\***
\***   TRANSMIT.CCWKF.VIA.PSS38:
\***
\***     set PIPE.OUT$ to "R" to instruct PSS38 to send the CCWKF to the LDT
\***     call routine to transmit the pipe
\***
\******************************************************************************

    TRANSMIT.CCWKF.VIA.PSS38:

      PIPE.OUT$ = "R"
      GOSUB SEND.TO.PSS38

    RETURN

\******************************************************************************
\***
\***   LOG.AN.EVENT.92:
\***
\***     prepare unique data with file number and exp/act records
\***
\******************************************************************************

    LOG.AN.EVENT.92:

        SB.EVENT.NO% = 92
       SB.UNIQUE$ = CHR$(SHIFT(FILE.NUM%,8)) +                          \
                    CHR$(SHIFT(FILE.NUM%,0)) +                         \
                   PACK$(RIGHT$(STRING$(8,"0")+STR$(EXPECTED.NUM%),8))+\
                   PACK$(RIGHT$(STRING$(8,"0")+STR$(ACTUAL.NUM%),8))
       SB.MESSAGE$ = ""
       GOSUB SB.LOG.AN.EVENT

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

      CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +              \BMJK
                        CSR.AUDIT.DATA$                                   !BMJK
      PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$

   AUDIT.FAIL:

   RETURN

\******************************************************************************
\***
\***   LOG.TO.LDTAF.FILE
\***
\******************************************************************************

   LOG.TO.LDTAF.FILE:

        CURRENT.KEY$ = ""
       CURR.SESS.NUM% = LDTAF.SESS.NUM%
        IF LDTAF.OPEN.FLAG$ <> "Y" THEN BEGIN
           IF END #LDTAF.SESS.NUM% THEN CREATE.LDTAF.FILE
           OPEN LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM% APPEND
           LDTAF.OPEN.FLAG$ = "Y"
        ENDIF
        IF LDTAF.LINK.TYPE% <> 0 THEN                                    \
         IF WRITE.LDTAF THEN GOTO WRITE.ERROR
       LDTAF.OPEN.FLAG$ = "N"
       CLOSE LDTAF.SESS.NUM%

   RETURN

   CREATE.LDTAF.FILE:

       IF END #LDTAF.SESS.NUM% THEN CREATE.ERROR
        CREATE POSFILE LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM%               \
          MIRRORED ATCLOSE
        LDTAF.OPEN.FLAG$ = "Y"
        IF LDTAF.LINK.TYPE% <> 0 THEN                                    \
         IF WRITE.LDTAF THEN GOTO WRITE.ERROR
        LDTAF.OPEN.FLAG$ = "N"
       CLOSE LDTAF.SESS.NUM%

   RETURN

\******************************************************************************
\***
\***   LOG.TABLE.FULL.ERROR:
\***
\***     prepare unique data with failing table description
\***
\******************************************************************************

    LOG.TABLE.FULL.ERROR:

        SB.EVENT.NO% = 126
       SB.MESSAGE$ = ""
       GOSUB SB.LOG.AN.EVENT
        RECEIVE.STATE$ = "*"
       GOTO MODULE.EXIT

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***              L D T    H A N D L I N G    R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   SEND.TO.PSS38:
\***
\***      transmit data to PSS38 (data contained in PIPE.OUT$)
\***
\******************************************************************************

   SEND.TO.PSS38:

      IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR
      CURR.SESS.NUM% = PIPEI.SESS.NUM%
      WRITE# PIPEI.SESS.NUM%; PIPE.OUT$

   RETURN

\******************************************************************************
\***
\***   HOLD.LDT:
\***
\***      send a hold LDT command to PSS38
\***
\******************************************************************************

   HOLD.LDT:

      PIPE.OUT$ = "HY"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "Y"

   RETURN

\******************************************************************************
\***
\***   RELEASE.LDT:
\***
\***      send a release LDT command to PSS38
\***
\******************************************************************************

   RELEASE.LDT:

      PIPE.OUT$ = "HN"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "N"

   RETURN

\****************************************************************************
\***
\***   GET MATRIX
\***
\***   Open STKIG file
\***   While Not EOF
\***      Read STKIG record
\***      If 'ALL   ' Then
\***         Set All Products flag
\***      Else
\***         If 'X     ' Then
\***            Store record in Business Centre array
\***         Else
\***            If 'XX    ' Then
\***               Store record in Concept Group array
\***            Else
\***               If 'XXXXXX' Then
\***                  Store record in Product Group array
\***
\***   Close STKIG file
\***
\****************************************************************************

GET.MATRIX:

       IF END# STKIG.SESS.NUM% THEN OPEN.ERROR
       CURR.SESS.NUM% = STKIG.SESS.NUM%
       OPEN STKIG.FILE.NAME$ DIRECT RECL STKIG.RECL% AS STKIG.SESS.NUM% \
               NODEL

       STKIG.RECORD.NUM% = 1
       BC% = 1
       CG% = 1
       PG% = 1
       ALL.PRODUCTS = 0
       STKIG.DATA$ = ""

       WHILE 1 = 1

           RC% = READ.STKIG
           IF RC% <> 0 THEN GOTO END.MATRIX

           IF STKIG.DATA$ = "ALL   " THEN BEGIN
              ALL.PRODUCTS = 1
              GOTO END.MATRIX
           ENDIF ELSE BEGIN
              IF RIGHT$(STKIG.DATA$,5) = "     " THEN BEGIN
                 BUSINESS.CENTRES$(BC%) = LEFT$(STKIG.DATA$,1)
                 BC% = BC% + 1
              ENDIF ELSE BEGIN
                 IF RIGHT$(STKIG.DATA$,4) = "    " THEN BEGIN
                    CONCEPT.GROUPS$(CG%) = LEFT$(STKIG.DATA$,2)
                    CG% = CG% + 1
                 ENDIF ELSE BEGIN
                    PRODUCT.GROUPS$(PG%) = STKIG.DATA$
                    PG% = PG% + 1
                 ENDIF
              ENDIF
           ENDIF


           STKIG.RECORD.NUM% = STKIG.RECORD.NUM% + 1

       WEND

END.MATRIX:

       CLOSE STKIG.SESS.NUM%

RETURN

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

      RC% = SESS.NUM.UTILITY(SB.ACTION$,                            \
                          SB.INTEGER%,                            \
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

      RC% = APPLICATION.LOG(MESSAGE.NO%,                            \
                         PORT.STRING$,                                   \
                         UNIQUE.2$,                                   \
                         PORT.EVENT% )

      RC% = APPLICATION.LOG(MESSAGE.NO%,                            \
                         SB.UNIQUE$,                                   \
                         UNIQUE.2$,                                   \
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

      RC% = CONV.TO.HEX( ERRN )
      IF RC% <> 0 THEN                                                 \
         GOTO PROGRAM.EXIT

      RC% = CONV.TO.STRING(0,                                          \
                        ERRN )
      IF RC% <> 0 THEN                                                 \
         GOTO PROGRAM.EXIT
      SB.ERRS$ = F17.RETURNED.STRING$

      SB.ERRL$ = FN.Z.PACK(STR$(ERRL), 6)
      SB.ACTION$ = "R" : SB.INTEGER% = ERRF% : SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +                      \
                 CHR$(SHIFT(SB.FILE.REP.NUM%,0))

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
      SB.EVENT.NO% = 106
      SB.UNIQUE$ = "O" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \
                        CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +            \
                             PACK$(STRING$(14,"0"))
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
      SB.EVENT.NO% = 106
      SB.UNIQUE$ = "R" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \
                        CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +            \
                        PACK$(FN.Z.PACK(CURRENT.KEY$,14))
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
      SB.EVENT.NO% = 106
      SB.UNIQUE$ = "W" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \
                        CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +            \
                        PACK$(FN.Z.PACK(CURRENT.KEY$,14))
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
      SB.EVENT.NO% = 106
      SB.UNIQUE$ = "C" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \
                         CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +             \
                      PACK$(STRING$(14,"0"))
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.CALC.BOOTS.CHECK.DIGIT
\***
\***   Purpose    : calculate boots check digit from a six byte string
\***                containing a boots code
\***
\***   Parameters : 1
\***      SB.BOOTS.ITEM$ = 6 character boots code (cccccc)
\***
\***   Output     : 1
\***      SB.BOOTS.CODE$      = 7 character boots code (ccccccc)
\***
\***   Error action : end program if called function fails
\***
\******************************************************************************

SB.CALC.BOOTS.CHECK.DIGIT:

      RC% = CALC.BOOTS.CODE.CHECK.DIGIT(SB.BOOTS.ITEM$)
      IF RC% <> 0 THEN GOTO PROGRAM.EXIT

      SB.BOOTS.CODE$ = SB.BOOTS.ITEM$ + F18.CHECK.DIGIT$

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
      IF RECEIVE.STATE$ <> "?" THEN RECEIVE.STATE$ = "*"

      GOTO MODULE.EXIT


\******************************************************************************
\***
\***   READ.ERROR:
\***
\***      log an event 6 (read error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   READ.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.READ.ERROR
      IF RECEIVE.STATE$ <> "?" THEN RECEIVE.STATE$ = "*"

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
      IF RECEIVE.STATE$ <> "?" THEN RECEIVE.STATE$ = "*"

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
      IF RECEIVE.STATE$ <> "?" THEN RECEIVE.STATE$ = "*"

   GOTO MODULE.EXIT


\******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***      if an error has occurred after a 'fatal' error then quit program
\***      if SIZE error has occurred (00000052h) and is not against the BCSF
\***       then resume with next command
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
   IF (ERRN = 00000052h) AND (ERRF%<>BCSMF.SESS.NUM%) THEN RESUME

   ERROR.COUNT% = ERROR.COUNT% + 1
   IF ERROR.COUNT% > 1 THEN RESUME PROGRAM.EXIT

   GOSUB SB.FORMAT.ERROR.DATA

   IF (ERRN AND 0000FFFFh) = 400Ch THEN BEGIN
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME RETRY
   ENDIF

   IF ERR = "OM" THEN BEGIN
      SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                             \
                   PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))
      CALL ADXERROR(0, 74, 0, 3, 101, SB.UNIQUE$ )
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME PROGRAM.EXIT
   ENDIF

   SB.EVENT.NO% = 101
   SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                                    \
                PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))
   GOSUB SB.LOG.AN.EVENT

   ERROR.COUNT% = ERROR.COUNT% - 1
   RECEIVE.STATE$ = "*"
   RESUME MODULE.EXIT

END SUB
