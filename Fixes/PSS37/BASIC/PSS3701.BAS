\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSS3701.BAS  $
\***
\***   $Revision:   1.11  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/pss3701.bav  $
\***
\***      Rev 1.11   09 Dec 2008 19:01:32   peter.sserunkuma
\***   The open PLLDB statement record length
\***   is now 164.  The change was in module
\***   PSS3701.BAS
\***
\***
\***      Rev 1.10   09 Feb 2007 14:43:38   paul.bowers
\***   ASN directs in A7B
\***
\***      Rev 1.9   01 Mar 2005 11:33:50   dev07ps
\***   Upwards TSF Fix.
\***   Fix to Report & Session numbers.
\***   Relinked with latest PLLOL object.
\***   Removal of LSS specific code.
\***   Fix to Stock Counting - moved to new program PDTSC.286 to solve PDT timeout when > 40 counts.
\***
\***
\***      Rev 1.8   12 Jan 2005 12:20:50   dev88ps
\***   Corrected so that old and new price checks
\***   work in this release. Changes could be
\***   removed in the future to get rid of the old
\***   price check code. For A5A
\***
\***      Rev 1.7   07 Jan 2005 16:30:46   dev24ps
\***   Fixed bug - Changed how CURRENT.STOCK figure calculated.
\***
\***      Rev 1.6   24 Dec 2004 13:04:08   dev07ps
\***   A5A Shelf Monitor Project
\***
\***
\***      Rev 1.5   30 Aug 1996 10:56:12   DEV20PS
\***   Changes to 96D pilot problems.
\***
\***      Rev 1.4   21 May 1996 09:58:32   DEV20PS
\***   Gap Monitor Changes
\***
\***      Rev 1.3   28 Jul 1995 13:28:18   DEVDSPS
\***
\***
\***        Rev 1.3   16 Feb 1995 11:43:40   DEVDSPS
\***   Update version number to 20 for CSRAF.
\***
\***      Rev 1.2   14 Feb 1995 11:43:40   NIK
\***   Fixes to Boots Problems 1248, 1245, 1246, 1318, 1105
\***
\***      Rev 1.1   07 Oct 1994 15:51:24   DEVSPPS
\***   Fixed PDT Support hang
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3701                                          ***
\***                                                                        ***
\***           AUTHOR   :  Janet Lawrence (Pseudocode / BASIC code)         ***
\***                                                                        ***
\***           DATE     :  9th August 1990                                  ***
\***                                                                        ***
\***                                                                        ***
\***           Current version letter : M                                   ***
\***                                                                        ***
\***           Date of last amendment : 17th May 1994                       ***
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
\***   There are basically two processes PSS37 performs ;                   ***
\***   i)  takes counts from a PDT and puts them in the stock movement,     ***
\***   ii) creates a file of lists requested by a PDT.                      ***
\***                                                                        ***
\***   This module was created to contain the bulk of the processing for    ***
\***   the two new applications, CSR and PRICE CHECKING, introduced as      ***
\***   part of the new EPOS-CSR link.                                       ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   A M E N D M E N T S                                                  ***
\***                                                                        ***
\***   Version B : Gary Flood (Believe it or not!) 13/06/91                 ***
\***   Details :   If manual count of item varies from original theoretical ***
\***               stock by an amount which is more than 3 and more than    ***
\***               30% of the original TS then set the count request flag   ***
\***               on the CITEM file ON.                                    ***
\***               This is a temporary change (ie. test is hard coded). The ***
\***               test should be changed to be parameter driven from the   ***
\***               BCSMF at the next opportunity.                           ***
\***                                                                        ***
\***   Version B : Paul Bowers    28/6/91   1/7/91                          ***
\***   Details :   To correct the undo processing for override items not on ***
\***               the CITEM file. preventing the program aborting back to  ***
\***               Data state A.                                            ***
\***               To prevent the program tring to DELETE the CSRWF when    ***
\***               it has just already done so. and the file is no longer   ***
\***               open.                                                    ***
\***                                                                        ***
\***   Version B : Paul Bowers   5/7/91                                     ***
\***               To include a check date of last manual count to make sure***
\***               you do not reject a manual count if it the initial count ***
\***               In undo processing close the files if an error occurs    ***
\***               which falls through a logs an event zero.                ***
\***                                                                        ***
\***   Version C : Paul Bowers 16/8/91                                      ***
\***               To fix open problem 464. To prevent first link of an     ***
\***               EPSOM line from ordering a full shelf when it becomes    ***
\***               a CSR line.                                              ***
\***                                                                        ***
\***   Version D : Paul Bowers 1/10/91                                      ***
\***               To include code to increment week4 sales by the value of ***
\***               the order generated instead of PSC12 calculating it from ***
\***               th CSRIMF which could include stock adjustments and make ***
\***               the sales figure go nagative.                            ***
\***               A consequence is that sales now reflect orders.          ***
\***               To include code for a new CSR Audit file, for every PDT  ***
\***               signon, failure, List Header send an entry to the Audit  ***
\***               file.                                                    ***
\***                                                                        ***
\***   Version E : Steve Wright                        30th October 1991    ***
\***               Amend background message routine                         ***
\***                                                                        ***
\***   Version F : Paul Bowers 8/1/92                                       ***
\***               To remove the updating of WEEK 4 SALES to PSC12 where it ***
\***               will now be incremented by the quantity delivered. Thus  ***
\***               preventing item from going vulnerable incorrectly.       ***
\***                                                                        ***
\***   Version G : David Smallwood 24/3/92                                  ***
\***               Update included code for flashpack (FPF).                ***
\***               Invalid data states are now recorded as "*"              ***
\***               Update included code for CSRWF                           ***
\***               Also - change for CSR - restructure subroutine           ***
\***               CHECK.FOR.CHANGE.OF.SALES.  For this change to           ***
\***               be effective changes made in version C will be           ***
\***               commented out.                                           ***
\***                                                                        ***
\***  Version H : Les Cook 30/10/92                                         ***
\***              Included code streamlined.                                ***
\***              Version created to solve memory problems by making        ***
\***              PSS37 chain back to itself after each application.        ***
\***              Also to fix problem B727.                                 ***
\***                                                                        ***
\***  Version I : Les Cook 12/11/92                                         ***
\***              CSR lines linked for the first time which have an alloc.  ***
\***              record on the IDSOF will have CIMF sales figures adjusted.***
\***              IDSOF record then deleted.                                ***
\***                                                                        ***
\***  Version J : Les Cook 25/3/93                                          ***
\***              Updated with UOD changes to included code.                ***
\***              If program logs an ABORT FAIL error, it will also log     ***
\***              instructions to reboot the PDT to the CSRAF.              ***
\***                                                                        ***
\***  Version Z : Paul Bowers  8/7/92; Les Cook 6/10/92                     ***
\***              Emergency fix version for store 1801 Arnold and CD5 OS    ***
\***              L.C.  increase wait time to 20secs (from 10 secs) in      ***
\***              Mainline Code section and RECEIVED.CSR.EOT subroutine.    ***
\***                                                                        ***
\***  Version K : Michael J. Kelsall  11/10/93                              ***
\***                Fix to Boots problem 1060, session numbers for CITEM and***
\***                IDF hardcoded, changed to session number allocated to   ***
\***                each file. (Part of the RETURNS/AUTO CREDIT CLAIMING    ***
\***                SYSTEM).                                                ***
\***              Addition of LDTAF coding for audit function.              ***
\***                                                                        ***
\***  Version L : Michael J. Kelsall  23/03/94                              ***
\***              Change of LDTAF duration field from minutes to seconds.   ***
\***                                                                        ***
\***  Version M : Steven Goulding 2/6/94                                    ***
\***              Recompile for version letter change in audit file         ***
\***                                                                        ***
\***                                                                        ***
\***  REVISION 1.1      ROBERT COWEY / STEVE WRIGHT      6TH OCTOBER 1994   ***
\***  Removed version letters from included code (not commented).           ***
\***                                                                        ***
\***   Version N (1.2)         Nik Sen                 26th January 1995    ***
\***   Amended CSR EOT processing so that Receive.State is set to '?'       ***
\***   which will force a restart when program returns to calling module.   ***
\***   Added error trap for ownership differences on CSRWF delete. Returns  ***
\***   to calling module and data state 'A'.                                ***
\***   Changed code in CHECK.FP.SALES to read CIMF locked to fix Boots      ***
\***   Problem 1105.                                                        ***
\***   Changed SB.FILE.REP.NUM% to a 2 byte integer.                        ***
\***                                                                        ***
\***   REVISION 1.3      Stuart Highley                   10th April 1996   ***
\***   Redirect gapped items (distinguished from price checked items due to ***
\***   the price of the item being entered as 1p at the PDT) to a new file, ***
\***   the Gapped Item Buffer File. Also, start the Gap Report application  ***
\***   (PSS47).                                                             ***
\***                                                                        ***
\***   REVISION 1.4      Stuart Highley               29th September 1996   ***
\***   Add a blank trailer record onto the end of each batch in the Gap     ***
\***   Buffer File. This is so that the start of a new batch is known by    ***
\***                                                                        ***
\***   REVISION 1.5    Charles Skadorwa               25th October 2004     ***
\***   Shelf Monitor Project changes.                                       ***
\***                                                                        ***
\***   REVISION 1.6    Mark Goode                     7th January 2004      ***
\***   Changed how CURRENT.STOCK figure calculated.                         ***
\***                                                                        ***
\***   REVISION 1.7    Brian Greenfield             11th January 2005       ***
\***   Fixes to allow old record type P and new record type P to both work. ***
\***   The old record type P can be removed next time this module is        ***
\***   altered.                                                             ***
\***                                                                        ***
\***   REVISION 1.8    Neil Bennett                 22nd December 2006      ***
\***   Add ASN processing support.                                          ***
\***   Remove CSR processing support.                                       ***
\***                                                                        ***
\***   REVISION 1.9    Peter Sserunkuma             09th December 2007      ***
\***   PLLDB file record length has increased from 54 to 164.               ***
\***                                                                        ***
\***   REVISION 1.10   Tittoo Thomas                  11th August 2012      ***
\***   PLLDB file record length has increased from 164 to 384 as part       ***
\***   of SFA. (Fix for Stock File Accuracy QC defect - 444)                ***
\***                                                                        ***
\***   REVISION 1.11   Tittoo Thomas                  27th December 2012    ***
\***   Included Application management WPI comments                         ***
\***                                                                        ***
\***   REVISION N      David Griffiths                5th July 2016         ***
\***   Amendment made to make sure IMSTC is checked correctly for stock     ***
\***   figure as scanning barcode did not find item.                        ***
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

   %INCLUDE PSS37G.J86                                          ! LMJK

\******************************************************************************
\***                                                                        ***
\***   Function : FN.ROUND.DOWN (DATA%)                                     ***
\***                                                                        ***
\***   Purpose  : To round real numbers down to their nearest integer       ***
\***                                                                        ***
\***   Output   : The new value for the real number                         ***
\***                                                                        ***
\******************************************************************************

\  FUNCTION FN.ROUND.DOWN(DATA%)                                               !1.8NWB

\    INTEGER*2                                                        \        !1.8NWB
\      TEMP.INT%                                                               !1.8NWB
\    REAL                                                             \        !1.8NWB
\      DATA%,                                                         \        !1.8NWB
\      FN.ROUND.DOWN                                                           !1.8NWB

\    TEMP.INT% = DATA%                                                         !1.8NWB
\    IF TEMP.INT% <> DATA% THEN                                       \        !1.8NWB
\       IF TEMP.INT% > DATA% THEN                                     \        !1.8NWB
\        TEMP.INT% = TEMP.INT% - 1                                             !1.8NWB
\    FN.ROUND.DOWN = TEMP.INT%                                                 !1.8NWB

\  END FUNCTION                                                                !1.8NWB

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL
      INTEGER*2 ADXSTART
      STRING    NAME$, PARM$, MESS$
   END FUNCTION

   %INCLUDE CHKBFEXT.J86                                    ! HLC
   %INCLUDE GAPBFEXT.J86                                    ! 1.3 !1.5CS !1.7BG
   %INCLUDE PLLOLEXT.J86                                    !1.5CS
   %INCLUDE PLLDBEXT.J86                                    !1.5CS
\  %INCLUDE CIMFEXT.J86                                     ! HLC              !1.8NWB
\  %INCLUDE CITEMEXT.J86                                    ! HLC              !1.8NWB
\  %INCLUDE CSRBFEXT.J86                                    ! HLC              !1.8NWB
\  %INCLUDE CSRWFEXT.J86                                    ! HLC              !1.8NWB
\  %INCLUDE FPFEXT.J86                                      ! HLC              !1.8NWB
\  %INCLUDE ONORDEXT.J86                                    ! HLC              !1.8NWB
\  %INCLUDE IDSOFEXT.J86                                    ! ILC              !1.8NWB

   SUB PSS3701 PUBLIC

INTEGER*1                                                   \
      CURR.SESS.NUM%,                                       \
\     FLASHPACK%,                                           \                  !1.8NWB
\     IDF.FOUND%,                                           \1.5CS             !1.8NWB
\     IMSTC.FOUND%,                                         \1.5CS             !1.8NWB
\     MASK%,                                                \1.5CS             !1.8NWB
      OLD.PCHECK.RECORD%,                                   \1.7BG
      PORT.EVENT%,                                          \
      SB.EVENT.NO%,                                         \
      SB.FILE.SESS.NUM%,                                    \
      SB.INTEGER%,                                          \
      SMON.NEW.LIST.CREATED,                                \1.5CS
\     STOCK.FOUND%,                                         \1.5CS             !1.8NWB
      GAP.RECORD.EXISTS                                    ! 1.4 !1.7BG

INTEGER*2                                                   \
\        COUNT1%,                                           \                  !1.8NWB
\        COUNT2%,                                           \                  !1.8NWB
\        CURRENT.HEADER.RECORD.NO%,                         \                  !1.8NWB
\        FP.NUMITEM%,                                       \                  !1.8NWB
\        I%,                                                \                  !1.8NWB
         MESSAGE.NO%,                                       \
\        NUMBER.OF.RECORDS%,                                \                  !1.8NWB
\        ONORD.SEQ.NO%,                                     \                  !1.8NWB
\        POSITION%,                                         \                  !1.8NWB
\        PROMOTIONAL.SA%,                                   \                  !1.8NWB
\        SECTOR.COUNT%,                                     \                  !1.8NWB
\        START.TIME%,                                       \ JLC              !1.8NWB
\        TEMP%,                                             \ NNS              !1.8NWB
         WS.NEW.LISTID%,                                    \1.5CS
         WS.SEQ.NO%,                                        \1.5CS
         X%,                                                \1.5CS
         Y%,                                                \1.5CS
         SB.FILE.REP.NUM%                                   ! NNS

INTEGER*4                                                   \
         ADX.RET.CODE%                                      !
\        QTY%                                                                  !1.8NWB

\REAL                                                       \                  !1.8NWB
\        ADJUST.PARENT.SALES%,                              \                  !1.8NWB
\        CSRWF.QTY.1%,                                      \                  !1.8NWB
\        CSRWF.QTY.2%,                                      \                  !1.8NWB
\        CSRWF.QTY.2.IN.SINGLES%,                           \                  !1.8NWB
\        MULTIPLICATION.FACTOR%,                            \                  !1.8NWB
\        PROMOTIONAL.MF%,                                   \                  !1.8NWB
\        THEORETICAL.STOCK%,                                \                  !1.8NWB
\        TOTAL.SA%,                                         \                  !1.8NWB
\        TOTAL.SALES%,                                      \                  !1.8NWB
\        COUNT.DISCREPANCY%                                 ! AGEF             !1.8NWB

STRING                                                      \
\     ALTER.DATE.OF.COUNT$,                                 \                  !1.8NWB
\     ALTER.UNIT$,                                          \                  !1.8NWB
      BOOTS.CODE$,                                          \ 1.5CS
\     CALC.FAMILY.FIGS$,                                    \                  !1.8NWB
\     CITEM.RECORD$,                                        \                  !1.8NWB
\     CITEM.SECTOR.ALTERED$,                                \                  !1.8NWB
      CURRENT.KEY$,                                         \
      CURRENT.STOCK$,                                       \ 1.5CS
      EAN.BARCODE$,                                         \ 1.5CS
\     END.OF.CITEM$,                                        \                  !1.8NWB
\     END.OF.CSRBF$,                                        \                  !1.8NWB
\     END.OF.CSRWF$,                                        \                  !1.8NWB
\     END.OF.ONORD$,                                        \                  !1.8NWB
\     END.OF.UNITS$,                                        \                  !1.8NWB
\     FILLER$,                                              \                  !1.8NWB
\     FIND.ITEM.ON.IDF$,                                    \                  !1.8NWB
\     FIND.ITEM.ON.CIMF$,                                   \                  !1.8NWB
\     FIRST.LINK.FLAG$,                                     \ CPAB             !1.8NWB
\     HOLD.FLAG$,                                           \                  !1.8NWB
      ITEM.FOUND.IN.TABLE$,                                 \ !1.5CS
\     ITEM.FOUND.ON.IDF$,                                   \                  !1.8NWB
\     ITEM.FOUND.ON.CIMF$,                                  \                  !1.8NWB
\     ITEM.FOUND.ON.CITEM$,                                 \                  !1.8NWB
\     ITEM.FOUND.ON.FPF$,                                   \                  !1.8NWB
\     LOCATION$,                                            \                  !1.8NWB
\     MANUAL.COUNT$,                                        \ ILC              !1.8NWB
\     ONORD.MATCH.FOUND$,                                   \                  !1.8NWB
\     ONORD.RECORD$,                                        \                  !1.8NWB
      PACKED.BOOTS.CODE$,                                   \ 1.5CS
\     PARENT.CODE$,                                         \                  !1.8NWB
\     PART.KEY$,                                            \                  !1.8NWB
\     PDT.SHELF.ALLOC$,                                     \                  !1.8NWB
\     PROCESSING.WORKFILE$,                                 \                  !1.8NWB
\     PROMOTION.DONE$,                                      \                  !1.8NWB
\     RECORD.TYPE$,                                         \                  !1.8NWB
      SB.ACTION$,                                           \
      SB.ERRF$,                                             \
      SB.ERRL$,                                             \
      SB.ERRS$,                                             \
      SB.MESSAGE$,                                          \
      SB.UNIQUE$,                                           \
      SB.STRING$,                                           \
      TEMP.STATE$,                                          \
      UNIQUE.2$,                                            \
\     UNITS.RECORD$,                                        \                  !1.8NWB
\     WS.TOTAL.ESA$,                                        \                  !1.8NWB
\     WS.TOTAL.ON.ORDER$,                                   \                  !1.8NWB
\     WS.ON.ORDER.TODAY$,                                   \                  !1.8NWB
\     WS.ON.ORDER.IN.THIS.PDT$,                             \                  !1.8NWB
\     WS.DATE.OF.LAST.MANUAL.COUNT$,                        \                  !1.8NWB
\     WS.SHELF.ALLOCATION$,                                 \                  !1.8NWB
\     WS.UNIT$,                                             \                  !1.8NWB
\     WS.LIST.FREQUENCY$,                                   \                  !1.8NWB
\     WS.SALES.SIGN.FLAG$,                                  \ DPAB             !1.8NWB
\     WS.COUNT.REQUEST.FLAG$,                               \ AGEF             !1.8NWB
\     WS.WEEK4.SALES$,                                      \ DPAB             !1.8NWB
      WS.STRING$                                            ! 1.5CS
\     WRITE.HOLD.CITEM.CIMF$                                                   !1.8NWB

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
\***   if this module has been called in order to process the workfile
\***          then do so
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

\     LOCATION$ = ""                                                           !1.8NWB
\     IF PROCESS.CSR.WORKFILE$ = "Y" THEN BEGIN                                !1.8NWB
\        GOSUB OPEN.FILES.FOR.CSR                                              !1.8NWB
\      CURR.SESS.NUM% = CSRWF.SESS.NUM%                                        !1.8NWB
\      IF END #CSRWF.SESS.NUM% THEN OPEN.ERROR                                 !1.8NWB
\      OPEN CSRWF.FILE.NAME$ DIRECT RECL CSRWF.RECL% AS CSRWF.SESS.NUM%        !1.8NWB
\      CSRWF.OPEN.FLAG$ = "Y"                                                  !1.8NWB
\        GOSUB PROCESS.CSR.WORKFILE                                            !1.8NWB
\      GOSUB RELEASE.PDT                                                       !1.8NWB
\      WAIT ;20000                                                   ! ZLC     !1.8NWB
\        IF CSRWF.OPEN.FLAG$ = "Y" THEN BEGIN                        ! BPAB    !1.8NWB
\         IF END #CSRWF.SESS.NUM% THEN DELETE.ERROR                  ! ASW     !1.8NWB
\           DELETE CSRWF.SESS.NUM%                                   ! ASW     !1.8NWB
\           CSRWF.OPEN.FLAG$ = "N"                                   ! ASW     !1.8NWB
\      ENDIF                                                         ! BPAB    !1.8NWB
\      GOTO MODULE.EXIT                                                        !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF MATCH(RECEIVE.STATE$,"MOPQ",1) > 0 THEN BEGIN                         !1.8NWB
      IF MATCH(RECEIVE.STATE$,"OPQ",1) > 0 THEN BEGIN                          !1.8NWB
         ON (ASC(RECEIVE.STATE$) - ASC("N")) GOSUB                   \         !1.8NWB
\              RECEIVED.CSR.TABLE.REQUEST,                           \         !1.8NWB
\              MODULE.EXIT,                                          \         !1.8NWB
               RECEIVED.PCHECK.HEADER,                               \
               RECEIVED.PCHECK.RECORD,                               \
               RECEIVED.PCHECK.TRAILER
       GOTO MODULE.EXIT
      ENDIF

\    IF FIRST.EOT.FOR.THIS.PDT$ = "N" THEN BEGIN                               !1.8NWB
\       GOSUB RECEIVED.CSR.EOT                                                 !1.8NWB
\       GOTO MODULE.EXIT                                                       !1.8NWB
\    ENDIF                                                                     !1.8NWB

\     CURR.SESS.NUM% = CSRBF.SESS.NUM%                                         !1.8NWB
\     IF END #CSRBF.SESS.NUM% THEN OPEN.ERROR                                  !1.8NWB
\     OPEN CSRBF.FILE.NAME$ AS CSRBF.SESS.NUM% NOWRITE NODEL                   !1.8NWB
\     CSRBF.OPEN.FLAG$ = "Y"                                                   !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     RC% = READ.CSRBF                                               ! HLC     !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                                ! HLC     !1.8NWB
\     END.OF.CSRBF$ = "N"                                                      !1.8NWB
\     WHILE END.OF.CSRBF$ = "N"                                                !1.8NWB
\        GOSUB DETERMINE.DATA.TYPE                                             !1.8NWB
\      IF MATCH(RECEIVE.STATE$,"JKLN",1) > 0 THEN BEGIN                        !1.8NWB
\           ON ((ASC(RECEIVE.STATE$)) - ASC("I")) GOSUB              \         !1.8NWB
\                 RECEIVED.CSR.LIST.HEADER,                          \         !1.8NWB
\                 RECEIVED.CSR.LIST.RECORD,                          \         !1.8NWB
\                 RECEIVED.CSR.LIST.TRAILER,                         \         !1.8NWB
\                 MODULE.EXIT,                                       \         !1.8NWB
\                 RECEIVED.CSR.EOT                                             !1.8NWB
\          RC% = READ.CSRBF                                          ! HLC     !1.8NWB
\         IF RC% = 1 THEN GOTO END.OF.CSRBF                          ! HLC     !1.8NWB
\      END.OF.CSRBF.RETURN:                                                    !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\           END.OF.CSRBF$ = "Y"                                                !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     WEND                                                                     !1.8NWB

!      CSRBF.OPEN.FLAG$ = "N"        ! moved after the delete command  ILC
\     IF END #CSRBF.SESS.NUM% THEN DELETE.ERROR                                !1.8NWB
\     DELETE CSRBF.SESS.NUM%                                                   !1.8NWB
\     CSRBF.OPEN.FLAG$ = "N"                                         ! ILC     !1.8NWB

MODULE.EXIT:

\  PROCESS.CSR.WORKFILE$ = "N"                                                 !1.8NWB
   EXIT SUB

PROGRAM.EXIT:

   STOP

\END.OF.CSRBF:                                                                 !1.8NWB
\  END.OF.CSRBF$ = "Y"                                                         !1.8NWB
\GOTO END.OF.CSRBF.RETURN                                                      !1.8NWB


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   S U B R O U T I N E S                                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   RECEIVED.CSR.LIST.HEADER:                            STATE : J
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if this is the first CSR list since the logon then
\***         gosub OPEN.CSR.FILES
\***
\***      look for a match for this list on the ON ORDER file
\***      ie.  see if this list has already been processed
\***
\***      if a match is found
\***         gosub DUPLICATE.LIST.FOUND
\***
\***      write a header record to CSRWF
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.LIST.HEADER:                                                   !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR Header Unbuffered"                      !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     IF FN.VALIDATE.DATA(CSRBF.DATA$, 9) = 0 THEN BEGIN                       !1.8NWB
\        RECEIVE.STATE$ = "*"                                           ! GDS  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CSR.LIST.ALREADY.PROCESSED.FLAG$ = "N"                                   !1.8NWB

\     IF FIRST.CSR.LIST.FOR.PDT$ = "Y" THEN BEGIN                              !1.8NWB
\        GOSUB OPEN.FILES.FOR.CSR                                              !1.8NWB
\        CURR.SESS.NUM% = CSRWF.SESS.NUM%                                      !1.8NWB
\        IF END #CSRWF.SESS.NUM% THEN CREATE.ERROR                             !1.8NWB
\        CREATE CSRWF.FILE.NAME$ DIRECT RECL CSRWF.RECL% AS CSRWF.SESS.NUM%    !1.8NWB
\        CSRWF.OPEN.FLAG$ = "Y"                                                !1.8NWB
\        PROCESS.CSR.WORKFILE$ = "Y"                                           !1.8NWB
\        CSRWF.RECORD.NO% = 1                                                  !1.8NWB
\        CURRENT.KEY$ = ""                                                     !1.8NWB
\        CSRWF.DELETE.FLAG$ = "Y"                                              !1.8NWB
\        RC% = WRITE.CSRWF                                              ! HLC  !1.8NWB
\        IF RC% = 1 THEN GOTO WRITE.ERROR                               ! HLC  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB LOOK.FOR.MATCH.ON.ONORDER                                          !1.8NWB

\     IF ONORD.MATCH.FOUND$ = "Y" THEN BEGIN                                   !1.8NWB
\        GOSUB DUPLICATE.LIST.FOUND                                            !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB WRITE.WORKFILE.HEADER                                              !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   RECEIVED.CSR.LIST.RECORD:                            STATE : K
\***
\***      if this list has been abandoned THEN
\***         return
\***
\***      increment the number of items read from this list
\***
\***      if this list has already been processed and therefore written
\***             to the ONORDER file
\***         gosub OUTPUT.CSR.RECORD.FROM.ONORDER
\***         return
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      note whether or not this is a zero override
\***
\***      gosub FIND.ITEM.ON.FILES (looks on CITEM, CIMF and IDF)
\***
\***      find the parent for the family (the item passed should have been the
\***      parent, but processing will continue for the family if it wasn't)
\***      provided that this is not a non-zero override
\***
\***      gosub UPDATE.CITEM.AS.REQUIRED
\***
\***      if this is a manual count (ie. on a manually counted list and
\***                this particular item has been counted) then
\***         gosub UPDATE.SALES.USING.MANUAL.COUNT
\***
\***      if item has been found on all files
\***         gosub CALC.ORDER.FOR.VALID.ITEM
\***      else gosub CALC.ORDER.FOR.INVALID.ITEM
\***
\***      gosub WRITE.ORDER
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.LIST.RECORD:                                                   !1.8NWB


\     IF LIST.ABANDONED$ = "Y" THEN RETURN                                     !1.8NWB

\     ALTER.DATE.OF.COUNT$ = "N"                                               !1.8NWB
\     ALTER.UNIT$ = "N"                                                        !1.8NWB
\     CSR.ITEMS.COUNT% = CSR.ITEMS.COUNT% + 1                                  !1.8NWB

\     IF CSR.LIST.ALREADY.PROCESSED.FLAG$ = "Y" THEN BEGIN                     !1.8NWB
\        GOSUB OUTPUT.CSR.RECORD.FROM.ONORDER                                  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XU" THEN BEGIN                                !1.8NWB
\        RECORD.TYPE$ = "UNCOUNTED"                                            !1.8NWB
\        IF FN.VALIDATE.DATA(CSRBF.DATA$, 10) = 0 THEN BEGIN                   !1.8NWB
\           RECEIVE.STATE$ = "*"                                        ! GDS  !1.8NWB
\           RETURN                                                             !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XM" THEN BEGIN                                !1.8NWB
\        RECORD.TYPE$ = "MANUAL"                                               !1.8NWB
\        IF FN.VALIDATE.DATA(CSRBF.DATA$, 11) = 0 THEN BEGIN                   !1.8NWB
\           RECEIVE.STATE$ = "*"                                        ! GDS  !1.8NWB
\           RETURN                                                             !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XO" THEN BEGIN                                !1.8NWB
\        RECORD.TYPE$ = "OVERRIDE"                                             !1.8NWB
\        IF FN.VALIDATE.DATA(CSRBF.DATA$, 12) = 0 THEN BEGIN                   !1.8NWB
\           RECEIVE.STATE$ = "*"                                        ! GDS  !1.8NWB
\           RETURN                                                             !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF RECORD.TYPE$ = "OVERRIDE" THEN BEGIN                                  !1.8NWB
\        IF VAL(MID$(CSRBF.DATA$,10,3)) = 0 THEN BEGIN                         !1.8NWB
\           CSRWF.ZERO.OVERRIDE.FLAG$ = "Y"                                    !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\           CSRWF.ZERO.OVERRIDE.FLAG$ = "N"                                    !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     FIND.ITEM.ON.IDF$ = "Y"                                                  !1.8NWB
\     FIND.ITEM.ON.CIMF$ = "Y"                                                 !1.8NWB
\     CITEM.BOOTS.CODE$ = PACK$("0" + MID$(CSRBF.DATA$,3,7))                   !1.8NWB
\     PARENT.CODE$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     CIMF.BOOTS.CODE$ = PACK$("00" + MID$(CSRBF.DATA$,3,6))                   !1.8NWB
\     IDF.BOOTS.CODE$ = CITEM.BOOTS.CODE$                                      !1.8NWB

\     GOSUB FIND.ITEM.ON.FILES                                                 !1.8NWB

\     IF ITEM.FOUND.ON.IDF$ = "Y" AND ITEM.FOUND.ON.CIMF$ = "Y"         \      !1.8NWB
\     AND ITEM.FOUND.ON.CITEM$ = "Y" THEN BEGIN                                !1.8NWB
\        IF RECORD.TYPE$ <> "OVERRIDE"                                  \      !1.8NWB
\        OR (RECORD.TYPE$ = "OVERRIDE"                                  \      !1.8NWB
\        AND CSRWF.ZERO.OVERRIDE.FLAG$ = "Y") THEN BEGIN                       !1.8NWB
\           GOSUB GET.PARENT.OF.FAMILY                                         !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB MOVE.CITEM.TO.WS                                                   !1.8NWB

\     GOSUB UPDATE.CITEM.AS.REQUIRED                                           !1.8NWB

\     IF ITEM.FOUND.ON.CITEM$ = "Y"                                     \      !1.8NWB
\     AND ITEM.FOUND.ON.CIMF$ = "Y" THEN BEGIN                                 !1.8NWB
\        IF RECORD.TYPE$ = "MANUAL"                                     \      !1.8NWB
\        AND MID$(CSRBF.DATA$,14,6) <> "XXXXXX" THEN BEGIN                     !1.8NWB
\           GOSUB UPDATE.SALES.USING.MANUAL.COUNT                              !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF ITEM.FOUND.ON.IDF$ = "Y"                                       \      !1.8NWB
\     AND ITEM.FOUND.ON.CIMF$ = "Y"                                     \      !1.8NWB
\     AND ITEM.FOUND.ON.CITEM$ = "Y" THEN BEGIN                                !1.8NWB
\        GOSUB CALC.ORDER.FOR.VALID.ITEM                                       !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        GOSUB CALC.ORDER.FOR.INVALID.ITEM                                     !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB WRITE.ORDER                                                        !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   RECEIVED.CSR.LIST.TRAILER:                           STATE : L
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if list has been abandoned then return
\***
\***      check the record count against the program count - if they do
\***      not match then abandon processing of this list
\***
\***      write a trailer to CSRWF
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.LIST.TRAILER:                                                  !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR Trailer Unbuffered"                     !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     IF FN.VALIDATE.DATA(CSRBF.DATA$, 13) = 0 THEN BEGIN                      !1.8NWB
\        RECEIVE.STATE$ = "*"                                           ! GDS  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LIST.ABANDONED$ = "Y" THEN BEGIN                                      !1.8NWB
\        LIST.ABANDONED$ = "N"                                                 !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF CSR.ITEMS.COUNT% <> VAL(MID$(CSRBF.DATA$,3,3)) THEN BEGIN             !1.8NWB
\        GOSUB ABANDON.THIS.LIST                                               !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB WRITE.WORKFILE.TRAILER                                             !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   RECEIVED.CSR.TABLE.REQUEST:                          STATE : M
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      open the UNITS file and write a record (BC letter, unit number)
\***      to a table for each unit read (reading directly)
\***      close UNITS and sort the table
\***      write a record to PSS38 for each record on the table
\***      write a trailer to PSS38 containing a count of records sent
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.TABLE.REQUEST:                                                 !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR Table request received"                 !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     IF FN.VALIDATE.DATA(DATA.IN$, 14) = 0 THEN BEGIN                         !1.8NWB
\        RECEIVE.STATE$ = "*"                                           ! GDS  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     DIM F14.TABLE$(40)                                                       !1.8NWB

\     CURR.SESS.NUM% = UNITS.SESS.NUM%                                         !1.8NWB
\     IF END #UNITS.SESS.NUM% THEN OPEN.ERROR                                  !1.8NWB
\     OPEN UNITS.FILE.NAME$ DIRECT RECL 512 AS UNITS.SESS.NUM% NOWRITE NODEL   !1.8NWB
\     UNITS.OPEN.FLAG$ = "Y"                                                   !1.8NWB

\     END.OF.UNITS$ = "N"                                                      !1.8NWB

\     NUMBER.OF.RECORDS% = 0                                                   !1.8NWB
\     SECTOR.COUNT% = 2                                                        !1.8NWB
\     CURRENT.KEY$ ="SECTOR"                                                   !1.8NWB
\     IF END #UNITS.SESS.NUM% THEN READ.ERROR                                  !1.8NWB
\     READ FORM "C4,C508"; #UNITS.SESS.NUM%, SECTOR.COUNT%;             \      !1.8NWB
\          FILLER$, UNITS.RECORD$                                              !1.8NWB

\     WHILE END.OF.UNITS$ = "N"                                                !1.8NWB
\        POSITION% = 1                                                         !1.8NWB
\        WHILE POSITION% < 480 AND NUMBER.OF.RECORDS% < 40 AND          \      !1.8NWB
\        MID$(UNITS.RECORD$,POSITION%,4) <> PACK$("00000000")                  !1.8NWB
\           NUMBER.OF.RECORDS% = NUMBER.OF.RECORDS% + 1                        !1.8NWB
\           F14.TABLE$(NUMBER.OF.RECORDS%) =                            \      !1.8NWB
\              MID$(UNITS.RECORD$,POSITION% + 31,1) +                   \      !1.8NWB
\              UNPACK$(MID$(UNITS.RECORD$,POSITION%,1))                        !1.8NWB
\           POSITION% = POSITION% + 32                                         !1.8NWB
\        WEND                                                                  !1.8NWB
\        SECTOR.COUNT% = SECTOR.COUNT% + 1                                     !1.8NWB
\        CURR.SESS.NUM% = UNITS.SESS.NUM%                                      !1.8NWB
\        IF END #UNITS.SESS.NUM% THEN END.OF.UNITS                             !1.8NWB
\        READ FORM "C4,C508"; #UNITS.SESS.NUM%, SECTOR.COUNT%;          \      !1.8NWB
\           FILLER$,UNITS.RECORD$                                              !1.8NWB
\        TABLE.REQUEST.CONTINUE:                                               !1.8NWB
\     WEND                                                                     !1.8NWB

\     CLOSE UNITS.SESS.NUM%                                                    !1.8NWB
\     UNITS.OPEN.FLAG$ = "N"                                                   !1.8NWB

\     RC% = SORT.TABLE(NUMBER.OF.RECORDS%)                                     !1.8NWB
\     IF RC% > 0 THEN GOTO PROGRAM.EXIT                                        !1.8NWB

\     PIPE.OUT$ = "L" + "XS"                                                   !1.8NWB
\     FOR I% = 1 TO NUMBER.OF.RECORDS%                                         !1.8NWB
\        PIPE.OUT$ = PIPE.OUT$ + F14.TABLE$(I%)                                !1.8NWB
\     NEXT I%                                                                  !1.8NWB
\     GOSUB SEND.TO.PSS38                                                      !1.8NWB

\     PIPE.OUT$ = "L" + "XZ" + CURR.TERMINAL$ +                         \      !1.8NWB
\                 RIGHT$("000" + STR$(NUMBER.OF.RECORDS%),3)                   !1.8NWB
\     GOSUB SEND.TO.PSS38                                                      !1.8NWB

\  RETURN                                                                      !1.8NWB

\  END.OF.UNITS:                                                               !1.8NWB
\     END.OF.UNITS$ = "Y"                                                      !1.8NWB
\  GOTO TABLE.REQUEST.CONTINUE                                                 !1.8NWB


\******************************************************************************
\***
\***   RECEIVED.CSR.EOT:                                    STATE : N
\***
\***      if this is not the first EOT for this list then goto SECOND.EOT
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if the count or terminal number are incorrect then abandon processing
\***
\***      write an EOT record to CSRWF
\***
\***      close the CSRWF, release the PDT and send a
\***       request to PSS38 to write it out and return
\***
\***   SECOND.EOT:
\***
\***      open the CSRWF and change the delete flag on the first record to "N"
\***
\***      gosub PROCESS.CSR.WORKFILE
\***
\***      release CSR file
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.EOT:                                                           !1.8NWB

\     IF FIRST.EOT.FOR.THIS.PDT$ = "N" THEN GOTO SECOND.EOT                    !1.8NWB

\     FIRST.EOT.FOR.THIS.PDT$ = "N"                                            !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR EOT received"                           !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     IF FN.VALIDATE.DATA(CSRBF.DATA$, 15) = 0 THEN BEGIN                      !1.8NWB
\        RECEIVE.STATE$ = "*"                                           ! GDS  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF CURR.TERMINAL$ <> MID$(CSRBF.DATA$,3,6)                        \      !1.8NWB
\     OR CSRWF.LIST.COUNT$ <> MID$(CSRBF.DATA$,9,3) THEN BEGIN                 !1.8NWB
\        SB.EVENT.NO% = 0                                               ! DPAB !1.8NWB
\        SB.UNIQUE$ = "ABORT FAIL"                                      ! DPAB !1.8NWB
\        GOSUB SB.LOG.AN.EVENT                                          ! DPAB !1.8NWB
\        CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] PDT " +      \ JLC  !1.8NWB
\                    "contains corrupt data and will " +                \ JLC  !1.8NWB
\                    "require a REBOOT before re-linking."              ! JLC  !1.8NWB
\        GOSUB LOG.TO.AUDIT.FILE                                        ! JLC  !1.8NWB
\        CSRWF.RECORD.NO% = 1                                                  !1.8NWB
\        CSRWF.DELETE.FLAG$ = "Y"                                              !1.8NWB
\        CURR.SESS.NUM% = CSRWF.SESS.NUM%                                      !1.8NWB
\        RC% = WRITE.CSRWF                                              ! HLC  !1.8NWB
\        IF RC% = 1 THEN GOTO WRITE.ERROR                               ! HLC  !1.8NWB
\        CSRWF.OPEN.FLAG$ = "N"                                                !1.8NWB
\        IF END #CSRWF.SESS.NUM% THEN DELETE.ERROR                             !1.8NWB
\        DELETE CSRWF.SESS.NUM%                                                !1.8NWB
\        PROCESS.CSR.WORKFILE$ = "N"                                           !1.8NWB
\        GOSUB RELEASE.PDT                                                     !1.8NWB
\        RECEIVE.STATE$ = "*"                                           ! GDS  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     GOSUB WRITE.WORKFILE.EOT                                                 !1.8NWB

\     CSRWF.RECORD.NO% = 1                                                     !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = READ.CSRWF                                                  ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                                   ! HLC  !1.8NWB
\     CSRWF.DELETE.FLAG$ = "N"                                                 !1.8NWB
\     RC% = WRITE.CSRWF                                                 ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! HLC  !1.8NWB
\     CLOSE CSRWF.SESS.NUM%                                                    !1.8NWB
\     CSRWF.OPEN.FLAG$ = "N"                                                   !1.8NWB

! added next line
\     GOSUB PROCESS.CSR.WORKFILE                                        ! ASW  !1.8NWB
\     GOSUB RELEASE.PDT                                                        !1.8NWB
\     GOSUB TRANSMIT.CSR.FILE.TO.PDT                                           !1.8NWB

\     GOTO END.OF.EOT                                                          !1.8NWB

\  SECOND.EOT:                                                                 !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR Data transmitted"                       !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     LDTAF.LINK.TYPE% = 1                                              ! KMJK !1.8NWB
\     IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                  \ KMJK !1.8NWB
\        (LDTAF.LINK.TYPE% OR 80H)                                      ! KMJK !1.8NWB
\     LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                           ! KMJK !1.8NWB
\     LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                           ! KMJK !1.8NWB
\     TEMP.TIME$ = TIME$                                                ! LMJK !1.8NWB
\     LDTAF.DURATION% =                                                 \ LMJK !1.8NWB
\        ( (VAL(LEFT$ (TEMP.TIME$,2))*3600) +                           \ LMJK !1.8NWB
\          (VAL(MID$  (TEMP.TIME$,3,2))*60) +                           \ LMJK !1.8NWB
\          (VAL(RIGHT$(TEMP.TIME$,2))) ) -                              \ LMJK !1.8NWB
\        ( (VAL(UNPACK$(LEFT$ (LDTAF.START.TIME$,1)))*3600)+            \ LMJK !1.8NWB
\          (VAL(UNPACK$(MID$  (LDTAF.START.TIME$,2,1)))*60)+            \ LMJK !1.8NWB
\          (VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1)))) )                ! LMJK !1.8NWB
\     LDTAF.DATA.VOLUME.1% = 0                                          ! KMJK !1.8NWB
\     LDTAF.DATA.VOLUME.1% = SIZE (CSRWF.FILE.NAME$)                    ! KMJK !1.8NWB
\     LDTAF.DATA.VOLUME.2% = NUMBER.OF.RECORDS%                         ! KMJK !1.8NWB
\     GOSUB LOG.TO.LDTAF.FILE                                           ! KMJK !1.8NWB

\     CLOSE CSR.SESS.NUM%                                                      !1.8NWB
\     CSR.OPEN.FLAG$ = "N"                                                     !1.8NWB

\     IF CSRWF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! ILC  !1.8NWB
! WAIT;20000 REMOVED FROM HERE
\        IF END #CSRWF.SESS.NUM% THEN DELETE.ERROR                      ! ILC  !1.8NWB
\        DELETE CSRWF.SESS.NUM%                                         ! ILC  !1.8NWB
\        CSRWF.OPEN.FLAG$ = "N"                                         ! ILC  !1.8NWB
\     ENDIF                                                             ! ILC  !1.8NWB

\     RE.CHAIN = TRUE                                                   ! NNS  !1.8NWB
\     RECEIVE.STATE$ = "?"                                              ! NNS  !1.8NWB

\  END.OF.EOT:                                                                 !1.8NWB
\  RETURN                                                                      !1.8NWB

\******************************************************************************
\******************************************************************************
\***
\***   C S R    S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   OPEN.FILES.FOR.CSR:
\***
\***      open any files required for CSR processing
\***      and create the CSRWF
\***
\***   RETURN
\***
\******************************************************************************

\  OPEN.FILES.FOR.CSR:                                                         !1.8NWB

\     FIRST.CSR.LIST.FOR.PDT$ = "N"                                            !1.8NWB
\     CSRWF.LIST.COUNT$ = "000"                                                !1.8NWB

\     CURR.SESS.NUM% = IDF.SESS.NUM%                                           !1.8NWB
\     IF END #IDF.SESS.NUM% THEN OPEN.ERROR                                    !1.8NWB
\     OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%          \     !1.8NWB
\          NOWRITE NODEL                                                       !1.8NWB
\     IDF.OPEN.FLAG$ = "Y"                                                     !1.8NWB

\     CURR.SESS.NUM% = CIMF.SESS.NUM%                                          !1.8NWB
\     IF END #CIMF.SESS.NUM% THEN OPEN.ERROR                                   !1.8NWB
\     OPEN CIMF.FILE.NAME$ KEYED RECL CIMF.RECL% AS CIMF.SESS.NUM%       \     !1.8NWB
\        NODEL                                                                 !1.8NWB
\     CIMF.OPEN.FLAG$ = "Y"                                                    !1.8NWB

\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     IF END #CITEM.SESS.NUM% THEN OPEN.ERROR                                  !1.8NWB
\     OPEN CITEM.FILE.NAME$ KEYED RECL CITEM.RECL% AS CITEM.SESS.NUM%    \     !1.8NWB
\        NODEL                                                                 !1.8NWB
\     CITEM.OPEN.FLAG$ = "Y"                                                   !1.8NWB

\     CURR.SESS.NUM% = ONORD.SESS.NUM%                                         !1.8NWB
\     IF END #ONORD.SESS.NUM% THEN OPEN.ERROR                                  !1.8NWB
\     OPEN ONORD.FILE.NAME$ KEYED RECL ONORD.RECL% AS ONORD.SESS.NUM%    \     !1.8NWB
\          NODEL                                                               !1.8NWB
\     ONORD.OPEN.FLAG$ = "Y"                                                   !1.8NWB

\     CURR.SESS.NUM% = FPF.SESS.NUM%                                           !1.8NWB
\     IF END #FPF.SESS.NUM% THEN OPEN.ERROR                                    !1.8NWB
\     OPEN FPF.FILE.NAME$ KEYED RECL FPF.RECL% AS FPF.SESS.NUM%          \     !1.8NWB
\          NODEL                                                               !1.8NWB
\     FPF.OPEN.FLAG$ = "Y"                                                     !1.8NWB

\     CURR.SESS.NUM% = IDSOF.SESS.NUM%                                  ! ILC  !1.8NWB
\     IF END #IDSOF.SESS.NUM% THEN OPEN.ERROR                           ! ILC  !1.8NWB
\     OPEN IDSOF.FILE.NAME$ KEYED RECL IDSOF.RECL% AS IDSOF.SESS.NUM%   \ ILC  !1.8NWB
\          NODEL                                                        ! ILC  !1.8NWB
\     IDSOF.OPEN.FLAG$ = "Y"                                            ! ILC  !1.8NWB

\     RETURN.FROM.IDSOF.OPEN.ERROR:                                            !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   LOOK.FOR.MATCH.ON.ONORDER:
\***
\***      look for a match on order date, list frequency and unit number
\***      on ONORD
\***
\***   RETURN
\***
\******************************************************************************

\  LOOK.FOR.MATCH.ON.ONORDER:                                                  !1.8NWB

\     CSRWF.HD.ONORDER.UPDATED.FLAG$ = "N"                                     !1.8NWB
\     CSRWF.UNPROCESS.FLAG$ = "N"                                              !1.8NWB
\     ONORD.ORDER.DATE$ = DATE$                                                !1.8NWB
\     ONORD.LIST.FREQ$ = MID$(CSRBF.DATA$,3,1)                                 !1.8NWB
\     ONORD.UNIT.NO$ = MID$(CSRBF.DATA$,4,2)                                   !1.8NWB
\     ONORD.SEQ.NO$ = "000"                                                    !1.8NWB
\     CURRENT.KEY$ = LEFT$(ONORD.ORDER.DATE$ + ONORD.LIST.FREQ$ +       \      !1.8NWB
\                 ONORD.UNIT.NO$ + ONORD.SEQ.NO$,8)                            !1.8NWB
\     ONORD.MATCH.FOUND$ = "N"                                                 !1.8NWB
\     RC% = READ.ONORDER                                                ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO ONORDER.MATCH.CONTINUE                       ! HLC  !1.8NWB
\     ONORD.MATCH.FOUND$ = "Y"                                                 !1.8NWB

\  ONORDER.MATCH.CONTINUE:                                                     !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   DUPLICATE.LIST.FOUND:
\***
\***      If the list for which a duplicate has been found is a non-override
\***        set flags to indicate that this list is already processed, else
\***        if this is an override list, set a flag to indicate that the
\***        original list of this number needs "unprocessing"
\***
\***   RETURN
\***
\******************************************************************************

\  DUPLICATE.LIST.FOUND:                                                       !1.8NWB

\     IF VAL(ONORD.UNIT.NO$) < 50 THEN BEGIN                                   !1.8NWB
\      CSR.LIST.ALREADY.PROCESSED.FLAG$ = "Y"                                  !1.8NWB
\      CSRWF.HD.ONORDER.UPDATED.FLAG$ = "Y"                                    !1.8NWB
\      ENDIF ELSE BEGIN                                                        !1.8NWB
\      CSRWF.UNPROCESS.FLAG$ = "Y"                                             !1.8NWB
\      ENDIF                                                                   !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   WRITE.WORKFILE.HEADER:
\***
\***   RETURN
\***
\******************************************************************************

\  WRITE.WORKFILE.HEADER:                                                      !1.8NWB

\     CSRWF.RECORD.TYPE$ = "XH"                                                !1.8NWB
\     CSR.ITEMS.COUNT% = 0                                                     !1.8NWB
\     CSRWF.LIST.COUNT$ = RIGHT$("000" + STR$(VAL(CSRWF.LIST.COUNT$) + 1),3)   !1.8NWB
\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     CURRENT.HEADER.RECORD.NO% = CSRWF.RECORD.NO%                             !1.8NWB
\     CSRWF.LIST.FREQ$ = ONORD.LIST.FREQ$                                      !1.8NWB
\     CSRWF.UNIT.NO$ = ONORD.UNIT.NO$                                          !1.8NWB
\     CSRWF.ORDER.DATE$ = DATE$                                                !1.8NWB
\     CSRWF.ORDER.TIME$ = LEFT$(TIME$,4)                                       !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = WRITE.CSRWF                                                 ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! HLC  !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   WRITE.WORKFILE.TRAILER:
\***
\***   RETURN
\***
\******************************************************************************

\  WRITE.WORKFILE.TRAILER:                                                     !1.8NWB

\     CSRWF.RECORD.TYPE$ = "XT"                                                !1.8NWB
\     CSRWF.ITEM.COUNT$ = RIGHT$("000" + STR$(CSR.ITEMS.COUNT%),3)             !1.8NWB
\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = WRITE.CSRWF                                                 ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! HLC  !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   WRITE.WORKFILE.EOT:
\***
\***   RETURN
\***
\******************************************************************************

\  WRITE.WORKFILE.EOT:                                                         !1.8NWB

\     CSRWF.RECORD.TYPE$ = "XZ"                                                !1.8NWB
\     CSRWF.TERMINAL.NO$ = CURR.TERMINAL$                                      !1.8NWB
\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = WRITE.CSRWF                                                 ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! HLC  !1.8NWB
\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   OUTPUT.CSR.RECORD.FROM.ONORDER:
\***
\***      for a list which has already been processed, the data for the
\***      workfile CSRWF will be taken from a record on ONORDER rather
\***      than from the data coming in from the PDT.
\***      If the data from ONORD differs from that from the PDT (ie. not
\***      the same Boots code) or the number of records for this list on
\***      ONORD does not match that from the PDT then the entire list
\***      will be abandoned.
\***
\***   RETURN
\***
\******************************************************************************

\  OUTPUT.CSR.RECORD.FROM.ONORDER:                                             !1.8NWB

\     ONORD.SEQ.NO$ = RIGHT$("000" + STR$(VAL(ONORD.SEQ.NO$) + 1),3)           !1.8NWB
\     IF ONORD.SEQ.NO$ > ONORD.HIGHEST.SEQ.NO$ THEN BEGIN                      !1.8NWB
\        GOSUB ABANDON.THIS.LIST                                               !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CURR.SESS.NUM% = ONORD.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ONORD.ORDER.DATE$ + ONORD.LIST.FREQ$ +             \      !1.8NWB
\                    ONORD.UNIT.NO$ + ONORD.SEQ.NO$                            !1.8NWB
\     RC% = READ.ONORDER                                                ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO NOT.ON.ONORD                                 ! HLC  !1.8NWB
\     CSRWF.RECORD.TYPE$ = "XC"                                                !1.8NWB
\     CSRWF.QTY.1$ = ONORD.QTY.1$                                              !1.8NWB
\     CSRWF.QTY.2$ = ONORD.QTY.2$                                              !1.8NWB
\     CSRWF.P.ITEM.CODE$ = PACK$("0" + ONORD.ITEM.CODE$)                       !1.8NWB
\     CSRWF.QTY.2.IN.SINGLES$ = "000"                                          !1.8NWB
\     CSRWF.CSRITEM.UPDATED.FLAG$ = "Y"                                        !1.8NWB
\     CSRWF.CSRIMF.UPDATED.FLAG$ = "Y"                                         !1.8NWB
\     CSRWF.ONORDER.UPDATED.FLAG$ = "Y"                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     RC% = WRITE.CSRWF                                                 ! HLC  !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! HLC  !1.8NWB

\  NOT.ON.ONORD.RETURN:                                                        !1.8NWB
\  RETURN                                                                      !1.8NWB

\  NOT.ON.ONORD:                                                               !1.8NWB
\     GOSUB ABANDON.THIS.LIST                                                  !1.8NWB
\  GOTO NOT.ON.ONORD.RETURN                                                    !1.8NWB

\******************************************************************************
\***
\***   FIND.ITEM.ON.FILES:
\***
\***      look for the item in question on the CITEM, CIMF and IDF
\***      (if not found on any one of these, a proper order quantity
\***       cannot be calculated : zero will be ordered unless the order
\***       was a non-zero override, in which case the amount requested
\***       will be ordered.)
\***
\***   RETURN
\***
\******************************************************************************

\  FIND.ITEM.ON.FILES:                                                         !1.8NWB

\     ITEM.FOUND.ON.CITEM$ = "N"                                               !1.8NWB
\     ITEM.FOUND.ON.CIMF$ = "N"                                                !1.8NWB
\     ITEM.FOUND.ON.IDF$ = "N"                                                 !1.8NWB
\     LOCATION$ = "FINDITEM"                                                   !1.8NWB

\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     RC% = READ.CITEM                                          ! HLC          !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB
\     ITEM.FOUND.ON.CITEM$ = "Y"                                               !1.8NWB

\     FIRST.LINK.FLAG$ = "N"                                    ! CPAB         !1.8NWB
\     IF CITEM.SHELF.ALLOCATION$ = PACK$("????") THEN BEGIN            ! CPAB  !1.8NWB
\        FIRST.LINK.FLAG$ = "Y"                                    ! CPAB      !1.8NWB
\     ENDIF                                                 ! CPAB             !1.8NWB

\     IF FIND.ITEM.ON.CIMF$ = "Y" THEN BEGIN                                   !1.8NWB
\        CURRENT.KEY$ = CIMF.BOOTS.CODE$                                       !1.8NWB
\        CURR.SESS.NUM% = CIMF.SESS.NUM%                                       !1.8NWB
\        RC% = READ.CIMF                                    ! HLC              !1.8NWB
\        IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC          !1.8NWB
\      ADJUST.PARENT.SALES% = 0                                                !1.8NWB
\        ITEM.FOUND.ON.CIMF$ = "Y"                                             !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF FIND.ITEM.ON.IDF$ = "Y" THEN BEGIN                                    !1.8NWB
\        CURRENT.KEY$ = IDF.BOOTS.CODE$                                        !1.8NWB
\        CURR.SESS.NUM% = IDF.SESS.NUM%                                        !1.8NWB
\        RC% = READ.IDF                                          ! HLC         !1.8NWB
\        IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC          !1.8NWB
\        ITEM.FOUND.ON.IDF$ = "Y"                                              !1.8NWB
\     ENDIF                                                                    !1.8NWB

\  FIND.ITEM.ON.FILES.CONTINUE:                                                !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   CALC.ORDER.FOR.INVALID.ITEM:
\***
\***      if a proper order quantity cannot be calculated
\***      zero will be ordered unless the order
\***      was a non-zero override, in which case the amount requested
\***      will be ordered.
\***
\***   RETURN
\***
\******************************************************************************

\  CALC.ORDER.FOR.INVALID.ITEM:                                                !1.8NWB

\     CSRWF.ONORDER.UPDATED.FLAG$ = "N"                                        !1.8NWB
\     CSRWF.CSRIMF.UPDATED.FLAG$ = "Y"                                         !1.8NWB
\     CSRWF.CSRITEM.UPDATED.FLAG$ = "Y"                                        !1.8NWB

\     IF RECORD.TYPE$ = "UNCOUNTED"                              \             !1.8NWB
\     OR RECORD.TYPE$ = "MANUAL" THEN BEGIN                                    !1.8NWB
\        CSRWF.P.ITEM.CODE$ = PACK$("0" + MID$(CSRBF.DATA$,3,7))               !1.8NWB
\        CSRWF.QTY.1$ = "000"                                                  !1.8NWB
\        CSRWF.QTY.2$ = "000"                                                  !1.8NWB
\        CSRWF.QTY.2.IN.SINGLES$ = "000"                                       !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        CSRWF.ITEM.CODE$ = MID$(CSRBF.DATA$,3,7)                              !1.8NWB
\        CSRWF.QTY.1$ = MID$(CSRBF.DATA$,10,3)                                 !1.8NWB
\        CSRWF.QTY.2$ = "000"                                                  !1.8NWB
\        CSRWF.QTY.2.IN.SINGLES$ = "000"                                       !1.8NWB
\        CSRWF.QTY.2.IN.SINGLES% = 0                                           !1.8NWB
\     ENDIF                                                                    !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   UPDATE.CITEM.AS.REQUIRED:
\***
\***      if this is a parent item with SA <> high-values and SA not
\***         equal to that just received from the PDT then
\***         update the SA and the sales
\***
\***      if the unit number for this item does not equal that of the list
\***        received from the PDT then update the unit number
\***
\***
\***   RETURN
\***
\******************************************************************************

\  UPDATE.CITEM.AS.REQUIRED:                                                   !1.8NWB

\     WRITE.HOLD.CITEM.CIMF$ = "N"                                             !1.8NWB
\     PDT.SHELF.ALLOC$ = PACK$("0" + MID$(CSRBF.DATA$,10,3))                   !1.8NWB
\     IF ITEM.FOUND.ON.CIMF$ = "Y"                              \              !1.8NWB
\     AND ITEM.FOUND.ON.CITEM$ = "Y" THEN BEGIN                                !1.8NWB
\        IF LEFT$(CSRBF.DATA$,2) <> "XO"                        \              !1.8NWB
\        AND WS.SHELF.ALLOCATION$ <> PDT.SHELF.ALLOC$ THEN BEGIN               !1.8NWB
\           GOSUB CHECK.FOR.CHANGE.OF.SALES                                    !1.8NWB
\           GOSUB CHECK.FOR.ALLOCATED.STOCK                     ! ILC          !1.8NWB
\           WS.SHELF.ALLOCATION$ = PDT.SHELF.ALLOC$                            !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF ITEM.FOUND.ON.CITEM$ = "Y"                             \              !1.8NWB
\     AND RECORD.TYPE$ <> "OVERRIDE" THEN BEGIN                                !1.8NWB
\        IF WS.UNIT$ <> PACK$(CSRWF.UNIT.NO$)                   \              !1.8NWB
\        OR WS.LIST.FREQUENCY$ <> PACK$("0" + CSRWF.LIST.FREQ$) THEN BEGIN     !1.8NWB
\           ALTER.UNIT$ = "Y"                                                  !1.8NWB
\           WS.UNIT$ = PACK$(CSRWF.UNIT.NO$)                                   !1.8NWB
\           WS.LIST.FREQUENCY$ = PACK$("0" + CSRWF.LIST.FREQ$)                 !1.8NWB
\        ENDIF                                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB


\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   CHECK.FOR.CHANGE.OF.SALES:
\***
\***   if the shelf allocation has changed value (but not from
\***   high values) then it will be necessary to alter the sales
\***   figure by the same amount
\***
\***   RETURN
\***
\******************************************************************************

\  CHECK.FOR.CHANGE.OF.SALES:                                                  !1.8NWB

\     IF WS.SHELF.ALLOCATION$ = PACK$("????") THEN BEGIN                ! GDS  !1.8NWB
\        WS.SHELF.ALLOCATION$ = PACK$("0000")                           ! GDS  !1.8NWB
\        WS.COUNT.REQUEST.FLAG$ = "Y"                                   ! GDS  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     ADJUST.PARENT.SALES% = VAL(UNPACK$(PDT.SHELF.ALLOC$)) -           \ GDS  !1.8NWB
\                            VAL(UNPACK$(WS.SHELF.ALLOCATION$))         ! GDS  !1.8NWB
\     WRITE.HOLD.CITEM.CIMF$ = "Y"                                      ! GDS  !1.8NWB

\  RETURN                                                                      !1.8NWB

\*****************************************************************************
\***
\***    CHECK.FOR.ALLOCATED.STOCK:
\***
\***          read IDSOF using CITEM boots code as key
\***
\***          If a record was found then subtract IDSOF outstanding quantity
\***          from ADJUST.PARENT.SALES and delete IDSOF record.
\***
\*****************************************************************************

\  CHECK.FOR.ALLOCATED.STOCK:                                           ! ILC  !1.8NWB

\     IF IDSOF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! ILC  !1.8NWB
\        IDSOF.ITEM.CODE$ = CITEM.BOOTS.CODE$                           ! ILC  !1.8NWB
\        RC% = READ.IDSOF                                               ! ILC  !1.8NWB
\        IF RC% = 1 THEN RETURN                                         ! ILC  !1.8NWB

\        ADJUST.PARENT.SALES% = ADJUST.PARENT.SALES% - IDSOF.QUANTITY%  ! ILC  !1.8NWB

\        IF END #IDSOF.SESS.NUM% THEN DELETE.ERROR                      ! ILC  !1.8NWB
\        DELREC IDSOF.SESS.NUM%; IDSOF.ITEM.CODE$                       ! ILC  !1.8NWB

\        RETURN.FROM.IDSOF.DELETE.ERROR:                                ! ILC  !1.8NWB
\     ENDIF                                                             ! ILC  !1.8NWB

\  RETURN                                                               ! ILC  !1.8NWB

\******************************************************************************
\***
\***   UPDATE.SALES.USING.MANUAL.COUNT:
\***
\***       get the theoretical stock for the family
\***
\***       if there is a multipack count (in COUNT2) then convert this to
\***       singles and add it into count 1
\***
\***       alter sales of the parent by the difference between COUNT1 and
\***       the family's theoretical stock
\***
\***       mark this item as having had its theoretical stock updated, and
\***       add it to the table to have its orders for days previous to today
\***       deleted
\***
\***   RETURN
\***
\******************************************************************************

\  UPDATE.SALES.USING.MANUAL.COUNT:                                            !1.8NWB

\     IF IDSOF.OPEN.FLAG$ = "Y" THEN BEGIN                                     !1.8NWB
\        IDSOF.ITEM.CODE$ = CITEM.BOOTS.CODE$                                  !1.8NWB
\        RC% = READ.IDSOF                                                      !1.8NWB
\        IF RC% = 1 THEN GOTO NOT.ON.IDSOF                                     !1.8NWB

\        MANUAL.COUNT$ = "Y"                                                   !1.8NWB
\        IF END # IDSOF.SESS.NUM% THEN DELETE.ERROR                            !1.8NWB
\        DELREC IDSOF.SESS.NUM%; IDSOF.ITEM.CODE$                              !1.8NWB

\        RETURN.FROM.IDSOF.DELETE.ERROR.2:                                     !1.8NWB
\        NOT.ON.IDSOF:                                                         !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     WS.COUNT.REQUEST.FLAG$ = "N"                                      ! BPAB !1.8NWB
\     IF CITEM.DATE.OF.LAST.MANUAL.COUNT$ = PACK$("000000") THEN BEGIN  ! BPAB !1.8NWB
\        WS.COUNT.REQUEST.FLAG$ = "I"                                   ! BPAB !1.8NWB
\     ENDIF                                                             ! BPAB !1.8NWB

\     GOSUB CALC.THEORETICAL.STOCK.FOR.FAMILY                                  !1.8NWB

\     COUNT1% = VAL(MID$(CSRBF.DATA$,14,3))                                    !1.8NWB
\     COUNT2% = VAL(MID$(CSRBF.DATA$,17,3))                                    !1.8NWB

\     IF MID$(CSRBF.DATA$,13,1) = "J" AND COUNT2% > 0 THEN BEGIN               !1.8NWB
\        COUNT1% = COUNT1% + COUNT2% * MULTIPLICATION.FACTOR%                  !1.8NWB
\        COUNT2% = 0                                                           !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF ITEM.FOUND.ON.FPF$ = "Y"                                       \      !1.8NWB
\     AND CIMF.BOOTS.CODE$ <>                                           \      !1.8NWB
\        PACK$("00" + MID$(UNPACK$(FPF.PARENT.CODE$),2,6)) THEN BEGIN          !1.8NWB
\        CIMF.BOOTS.CODE$ =                                             \      !1.8NWB
\            PACK$("00" + MID$(UNPACK$(FPF.PARENT.CODE$),2,6))                 !1.8NWB
\        CURR.SESS.NUM% = CIMF.SESS.NUM%                                       !1.8NWB
\        CURRENT.KEY$ = CIMF.BOOTS.CODE$                                       !1.8NWB
\        RC% = READ.CIMF                                                ! HLC  !1.8NWB
\        IF RC% = 1 THEN GOTO READ.ERROR                                ! HLC  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     ADJUST.PARENT.SALES% = ADJUST.PARENT.SALES% -                     \      !1.8NWB
\                            COUNT1% + THEORETICAL.STOCK% -             \      !1.8NWB
\                      VAL(UNPACK$(WS.TOTAL.ON.ORDER$)) +               \      !1.8NWB
\                      VAL(UNPACK$(WS.ON.ORDER.TODAY$))                        !1.8NWB

\     COUNT.DISCREPANCY% = ABS(THEORETICAL.STOCK% - COUNT1%)            ! AGEF !1.8NWB
\     IF WS.COUNT.REQUEST.FLAG$ <> "I" THEN BEGIN                       ! BPAB !1.8NWB
\       IF COUNT.DISCREPANCY% > 3 THEN BEGIN                            ! AGEF !1.8NWB
\         IF THEORETICAL.STOCK% = 0 THEN BEGIN                          ! AGEF !1.8NWB
\           WS.COUNT.REQUEST.FLAG$ = "Y"                                ! AGEF !1.8NWB
\         ENDIF ELSE BEGIN                                              ! AGEF !1.8NWB
\           IF (COUNT.DISCREPANCY% / THEORETICAL.STOCK%) > 0.3 THEN BEGIN      !1.8NWB
\             WS.COUNT.REQUEST.FLAG$ = "Y"                              ! AGEF !1.8NWB
\           ENDIF                                                       ! AGEF !1.8NWB
\         ENDIF                                                         ! AGEF !1.8NWB
\       ENDIF                                                           ! AGEF !1.8NWB
\     ENDIF                                                             ! BPAB !1.8NWB

\     WS.TOTAL.ON.ORDER$ = WS.ON.ORDER.TODAY$                                  !1.8NWB
\     WS.DATE.OF.LAST.MANUAL.COUNT$ = PACK$(DATE$)                             !1.8NWB
\     ALTER.DATE.OF.COUNT$ = "Y"                                               !1.8NWB
\     WRITE.HOLD.CITEM.CIMF$ = "Y"                                             !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   CALC.THEORETICAL.STOCK.FOR.FAMILY:
\***
\***     TS = SA + total ESA - sales + on order - on order today + init.stock
\***
\***     calculate the theoretical stock for the parent
\***     for each flashpack, add in the theoretical stock for the flashpack
\***   RETURN
\***
\******************************************************************************

\  CALC.THEORETICAL.STOCK.FOR.FAMILY:                                          !1.8NWB

\     LOCATION$ = "CALCTS"                                                     !1.8NWB
\     ITEM.FOUND.ON.FPF$ = "N"                                                 !1.8NWB
\     FLASHPACK% = 0                                                           !1.8NWB
\     IF FPF.PARENT.CODE$ <> PACK$("00000000") THEN BEGIN                      !1.8NWB
\        CURR.SESS.NUM% = FPF.SESS.NUM%                                        !1.8NWB
\        CURRENT.KEY$ = FPF.PARENT.CODE$                                       !1.8NWB
\        RC% = READ.FPF                                                 ! HLC  !1.8NWB
\        IF RC% = 1 THEN GOTO CALC.TS.CONTINUE                          ! HLC  !1.8NWB
\        ITEM.FOUND.ON.FPF$ = "Y"                                              !1.8NWB
\     ENDIF                                                                    !1.8NWB

\  CALC.TS.CONTINUE:                                                           !1.8NWB

\     THEORETICAL.STOCK% = VAL(UNPACK$(WS.SHELF.ALLOCATION$)) +         \      !1.8NWB
\                          VAL(UNPACK$(WS.TOTAL.ESA$)) -                \      !1.8NWB
\                          CIMF.NUMITEM% - ADJUST.PARENT.SALES% +       \      !1.8NWB
\                          VAL(UNPACK$(WS.TOTAL.ON.ORDER$)) -           \      !1.8NWB
\                          VAL(UNPACK$(WS.ON.ORDER.TODAY$)) +           \      !1.8NWB
\                          VAL(UNPACK$(CITEM.INITIAL.DISPLAY.STOCK$))          !1.8NWB

\     IF ITEM.FOUND.ON.FPF$ = "N" THEN RETURN                                  !1.8NWB

\     MULTIPLICATION.FACTOR% = 1                                               !1.8NWB
\     FLASHPACK% = 1                                                           !1.8NWB
\     WHILE FLASHPACK% < 11 AND FPF.CODE$(FLASHPACK%) <> PACK$("99999999")     !1.8NWB
\        CITEM.BOOTS.CODE$ = FPF.CODE$(FLASHPACK%)                             !1.8NWB
\        MULTIPLICATION.FACTOR% =                                       \      !1.8NWB
\            VAL(UNPACK$(FPF.MULT.FACTOR$(FLASHPACK%))) / 10                   !1.8NWB
\        CIMF.BOOTS.CODE$ =                                             \      !1.8NWB
\            PACK$("00" + MID$(UNPACK$(FPF.CODE$(FLASHPACK%)),2,6))            !1.8NWB
\      CURR.SESS.NUM% = CITEM.SESS.NUM%                                        !1.8NWB
\      CURRENT.KEY$ = CITEM.BOOTS.CODE$                                        !1.8NWB
\      RC% = READ.CITEM                                    ! HLC               !1.8NWB
\      IF RC% = 1 THEN GOTO NEXT.FLASHPACK                        ! HLC        !1.8NWB
\      CURR.SESS.NUM% = CIMF.SESS.NUM%                                         !1.8NWB
\      CURRENT.KEY$ = CIMF.BOOTS.CODE$                                         !1.8NWB
\      RC% = READ.CIMF                                    ! HLC                !1.8NWB
\        IF RC% = 1 THEN GOTO NEXT.FLASHPACK                        ! HLC      !1.8NWB
\      THEORETICAL.STOCK% = THEORETICAL.STOCK% +                  \            !1.8NWB
\                             (VAL(UNPACK$(CITEM.TOTAL.ESA$)) -       \        !1.8NWB
\                             CIMF.NUMITEM% +                           \      !1.8NWB
\                       VAL(UNPACK$(CITEM.TOTAL.ON.ORDER$)) -      \           !1.8NWB
\                       VAL(UNPACK$(CITEM.ON.ORDER.TODAY$)) +     \            !1.8NWB
\                   VAL(UNPACK$(CITEM.INITIAL.DISPLAY.STOCK$))) * \            !1.8NWB
\                       MULTIPLICATION.FACTOR%                                 !1.8NWB
\        IF CITEM.ON.ORDER.TODAY$ <> CITEM.TOTAL.ON.ORDER$ THEN BEGIN          !1.8NWB
\         CIMF.NUMITEM% = CIMF.NUMITEM% +                         \            !1.8NWB
\                     VAL(UNPACK$(CITEM.ON.ORDER.TODAY$)) -      \             !1.8NWB
\                     VAL(UNPACK$(CITEM.TOTAL.ON.ORDER$))                      !1.8NWB
\         CURR.SESS.NUM% = CIMF.SESS.NUM%                                      !1.8NWB
\         CURRENT.KEY$ = CIMF.BOOTS.CODE$                                      !1.8NWB
\         RC% = WRITE.CIMF.HOLD                              ! HLC             !1.8NWB
\           IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC      !1.8NWB
\         CITEM.TOTAL.ON.ORDER$ = CITEM.ON.ORDER.TODAY$                        !1.8NWB
\         CURR.SESS.NUM% = CITEM.SESS.NUM%                                     !1.8NWB
\         CURRENT.KEY$ = CITEM.BOOTS.CODE$                                     !1.8NWB
\         RC% = WRITE.CITEM.HOLD                               ! HLC           !1.8NWB
\           IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC      !1.8NWB
\      ENDIF                                                                   !1.8NWB
\     NEXT.FLASHPACK:                                                          !1.8NWB
\        FLASHPACK% = FLASHPACK% + 1                                           !1.8NWB
\     WEND                                                                     !1.8NWB

\     LOCATION$ = ""                                                           !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   CALC.ORDER.FOR.VALID.ITEM:
\***
\***      at this point any manual counts will have been processed, and all
\***      counts can be calculated using the controller figures only.
\***
\***      if this is a non-zero override then order the amount requested
\***
\***      otherwise order = sales - total onorder - onorder in this PDT
\***      or, if a family, gosub CALCULATE.FAMILY.FIGURES
\***
\***   RETURN
\***
\******************************************************************************

\  CALC.ORDER.FOR.VALID.ITEM:                                                  !1.8NWB

\     CSRWF.QTY.1$ = "000"                                                     !1.8NWB
\     CSRWF.QTY.2$ = "000"                                                     !1.8NWB
\     CSRWF.QTY.2.IN.SINGLES$ = "000"                                          !1.8NWB
\     CSRWF.QTY.1% = 0                                                         !1.8NWB
\     CSRWF.QTY.2% = 0                                                         !1.8NWB
\     CSRWF.QTY.2.IN.SINGLES% = 0                                              !1.8NWB

\     CSRWF.ONORDER.UPDATED.FLAG$ = "N"                                        !1.8NWB
\     CSRWF.CSRIMF.UPDATED.FLAG$ = "N"                                         !1.8NWB
\     CSRWF.CSRITEM.UPDATED.FLAG$ = "N"                                        !1.8NWB

\     IF RECORD.TYPE$ = "OVERRIDE" THEN BEGIN                                  !1.8NWB
\        CSRWF.RECORD.TYPE$ = "XO"                                             !1.8NWB
\        CSRWF.ITEM.CODE$ = MID$(CSRBF.DATA$,3,7)                              !1.8NWB
\      IF CSRWF.ZERO.OVERRIDE.FLAG$ = "N" THEN BEGIN                           !1.8NWB
\           CSRWF.QTY.1$ = MID$(CSRBF.DATA$,10,3)                              !1.8NWB
\         RETURN                                                               !1.8NWB
\      ENDIF ELSE BEGIN                                                        !1.8NWB
\         IF WS.SHELF.ALLOCATION$ = PACK$("????") THEN BEGIN                   !1.8NWB
\            CSRWF.QTY.1$ = "000"                                              !1.8NWB
\            RETURN                                                            !1.8NWB
\         ENDIF                                                                !1.8NWB
\      ENDIF                                                                   !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF (RECORD.TYPE$ = "UNCOUNTED" OR RECORD.TYPE$ = "MANUAL")      \        !1.8NWB
\     OR (RECORD.TYPE$ = "OVERRIDE"                              \             !1.8NWB
\     AND CSRWF.ZERO.OVERRIDE.FLAG$ = "Y")  THEN BEGIN                         !1.8NWB
\        IF FPF.PARENT.CODE$ <> PACK$("00000000") THEN BEGIN                   !1.8NWB
\         GOSUB CALCULATE.FAMILY.FIGURES                                       !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\         CSRWF.QTY.1% = CIMF.NUMITEM% + ADJUST.PARENT.SALES% -      \         !1.8NWB
\                        VAL(UNPACK$(WS.TOTAL.ON.ORDER$)) -            \       !1.8NWB
\                      VAL(UNPACK$(WS.ON.ORDER.IN.THIS.PDT$))                  !1.8NWB
\         CSRWF.QTY.1$ = RIGHT$("000" + STR$(CSRWF.QTY.1%),3)                  !1.8NWB
\         TOTAL.SA% = VAL(UNPACK$(WS.SHELF.ALLOCATION$)) +            \        !1.8NWB
\                 VAL(UNPACK$(WS.TOTAL.ESA$))                                  !1.8NWB
\         IF CSRWF.QTY.1% > TOTAL.SA% THEN BEGIN                               !1.8NWB
\            CSRWF.QTY.1% = TOTAL.SA%                                          !1.8NWB
\            CSRWF.QTY.1$ = RIGHT$("000" + STR$(CSRWF.QTY.1%),3)               !1.8NWB
\           ENDIF                                                              !1.8NWB
\        ENDIF                                                                 !1.8NWB
       ! following check commented out to allow a full shelf order for  GDS
         ! items switching from EPSOM to CSR.                             GDS
         !IF RECORD.TYPE$ = "UNCOUNTED"                               \ CPAB
       !   AND FIRST.LINK.FLAG$ = "Y" THEN BEGIN                   ! CPAB
       !   CSRWF.QTY.1$ = "000"                              ! CPAB
       !ENDIF                                                ! CPAB
\     ENDIF                                                                    !1.8NWB

\     CSRWF.P.ITEM.CODE$ = PARENT.CODE$                                        !1.8NWB

\     IF CSRWF.QTY.1% < 0 THEN CSRWF.QTY.1$ = "000"                            !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   GET.PARENT.OF.FAMILY:
\***
\***      use the IDF to determine the parent of any item
\***      if the item was a flashpack, read the CITEM and CIMF for
\***      the parent item
\***
\***   RETURN
\***
\******************************************************************************

\  GET.PARENT.OF.FAMILY:                                                       !1.8NWB

\     IF IDF.PARENT.CODE$ = PACK$("00000000") THEN BEGIN                       !1.8NWB
\        FPF.PARENT.CODE$ = PACK$("00000000")                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF IDF.PARENT.CODE$ = IDF.BOOTS.CODE$ THEN BEGIN                         !1.8NWB
\        FPF.PARENT.CODE$ = IDF.BOOTS.CODE$                                    !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     LOCATION$ = "FPFPROC"                                                    !1.8NWB
\     ITEM.FOUND.ON.CITEM$ = "N"                                               !1.8NWB
\     ITEM.FOUND.ON.CIMF$ = "N"                                                !1.8NWB
\     FPF.PARENT.CODE$ = IDF.PARENT.CODE$                                      !1.8NWB
\     PARENT.CODE$ = FPF.PARENT.CODE$                                          !1.8NWB
\     CITEM.BOOTS.CODE$ = IDF.PARENT.CODE$                                     !1.8NWB
\     CIMF.BOOTS.CODE$ = PACK$("00" + MID$(UNPACK$(IDF.PARENT.CODE$),2,6))     !1.8NWB
\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     RC% = READ.CITEM                                          ! HLC          !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB
\     ITEM.FOUND.ON.CITEM$ = "Y"                                               !1.8NWB
\     CURR.SESS.NUM% = CIMF.SESS.NUM%                                          !1.8NWB
\     CURRENT.KEY$ = CIMF.BOOTS.CODE$                                          !1.8NWB
\     RC% = READ.CIMF                                          ! HLC           !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB
\     ITEM.FOUND.ON.CIMF$ = "Y"                                                !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB
\  FIND.PARENT.RETURN:                                                         !1.8NWB
\  RETURN                                                                      !1.8NWB

\  FIND.PARENT.CONTINUE:                                                       !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB
\  GOTO FIND.PARENT.RETURN                                                     !1.8NWB

\******************************************************************************
\***
\***   CALCULATE.FAMILY.FIGURES:
\***
\***      calculate the order quantity using just the parent
\***
\***      for each flashpack item found on both CIMF and CITEM
\***         increment the order quantity using the figures for the FP
\***
\***         if a qty 2 is expected then use the first item on promotion
\***         to calculate the wants (in "packets", not singles) for that
\***         item
\***
\***         keep a running total of sales and of ESA
\***
\***      ensure that the order quantity does not exceed the total shelf
\***      allocation available to this family
\***
\***      if qty 2 is -ve or greater than the shelf allocation for the
\***         item in question then adjust it and place the difference
\***         (adjusted by MF) in the qty for rest of family (qty 1)
\***
\***      if qty 1 is -ve then, if qty 2 has been used, qty 2 will be reduced
\***         to reflect the overstock of the other members of the family,
\***         and qty 1 will be set to 0.  (If this means that qty 2 has
\***         become negative, which implies that the figures for this family
\***         are incorrect, then it will be set to 0.)
\***         This situation could arise following a transmission failure of
\***         the deliveries file.
\***
\***   RETURN
\***
\******************************************************************************

\  CALCULATE.FAMILY.FIGURES:                                                   !1.8NWB

\     LOCATION$ = "FAMFIGS"                                                    !1.8NWB
\     FLASHPACK% = 0                                                           !1.8NWB
\     ITEM.FOUND.ON.FPF$ = "N"                                                 !1.8NWB
\     CURR.SESS.NUM% = FPF.SESS.NUM%                                           !1.8NWB
\     CURRENT.KEY$ = FPF.PARENT.CODE$                                          !1.8NWB
\     RC% = READ.FPF                                          ! HLC            !1.8NWB
\     IF RC% = 1 THEN GOTO CALC.FAMILY.FIGURES.CONTINUE                  ! HLC !1.8NWB
\     ITEM.FOUND.ON.FPF$ = "Y"                                                 !1.8NWB

\  CALC.FAMILY.FIGURES.CONTINUE:                                               !1.8NWB

\     CSRWF.QTY.1% = CIMF.NUMITEM% + ADJUST.PARENT.SALES% -              \     !1.8NWB
\               VAL(UNPACK$(WS.TOTAL.ON.ORDER$)) -                  \          !1.8NWB
\               VAL(UNPACK$(WS.ON.ORDER.IN.THIS.PDT$))                         !1.8NWB

\     TOTAL.SALES% = CIMF.NUMITEM% + ADJUST.PARENT.SALES%                      !1.8NWB
\     TOTAL.SA% = VAL(UNPACK$(WS.SHELF.ALLOCATION$)) +                  \      !1.8NWB
\                 VAL(UNPACK$(WS.TOTAL.ESA$))                                  !1.8NWB

\     IF ITEM.FOUND.ON.FPF$ = "N" THEN BEGIN                                   !1.8NWB
\        IF CSRWF.QTY.1% > TOTAL.SA% THEN CSRWF.QTY.1% = TOTAL.SA%             !1.8NWB
\        CSRWF.QTY.1$ = RIGHT$("000" + STR$(CSRWF.QTY.1%), 3)                  !1.8NWB
\        CSRWF.QTY.2$ = RIGHT$("000" + STR$(CSRWF.QTY.2%), 3)                  !1.8NWB
\        RETURN                                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     PROMOTION.DONE$ = "N"                                                    !1.8NWB
\     PROMOTIONAL.SA% = 0                                                      !1.8NWB
\     PROMOTIONAL.MF% = 1                                                      !1.8NWB
\     FLASHPACK% = 1                                                           !1.8NWB

\     WHILE FLASHPACK% < 11 AND      FPF.CODE$(FLASHPACK%) <> PACK$("99999999")!1.8NWB
\      CITEM.BOOTS.CODE$ = FPF.CODE$(FLASHPACK%)                               !1.8NWB
\      CIMF.BOOTS.CODE$ =                                     \                !1.8NWB
\            PACK$("00" + MID$(UNPACK$(FPF.CODE$(FLASHPACK%)),2,6))            !1.8NWB
\      CURR.SESS.NUM% = CITEM.SESS.NUM%                                        !1.8NWB
\      CURRENT.KEY$ = CITEM.BOOTS.CODE$                                        !1.8NWB
\      RC% = READ.CITEM                                    ! HLC               !1.8NWB
\        IF RC% = 1 THEN GOTO CALC.FAMILY.FIGURES.CONTINUE2            ! HLC   !1.8NWB
\      IF ALTER.UNIT$ = "Y"                                    \ DPAB          !1.8NWB
\      OR ALTER.DATE.OF.COUNT$ = "Y" THEN BEGIN                                !1.8NWB
\           CITEM.UNIT$ = WS.UNIT$                                             !1.8NWB
\         CITEM.LIST.FREQUENCY$ = WS.LIST.FREQUENCY$                           !1.8NWB
\         CITEM.DATE.OF.LAST.MANUAL.COUNT$ =                         \         !1.8NWB
\                        WS.DATE.OF.LAST.MANUAL.COUNT$                         !1.8NWB
\         RC% = WRITE.CITEM                                    ! HLC           !1.8NWB
\           IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC      !1.8NWB
\      ENDIF                                                                   !1.8NWB
\      CURR.SESS.NUM% = CIMF.SESS.NUM%                                         !1.8NWB
\      CURRENT.KEY$ = CIMF.BOOTS.CODE$                                         !1.8NWB
\      RC% = READ.CIMF                                    ! HLC                !1.8NWB
\        IF RC% = 1 THEN GOTO CALC.FAMILY.FIGURES.CONTINUE2            ! HLC   !1.8NWB
\      FP.NUMITEM% = CIMF.NUMITEM%                                             !1.8NWB
\      MULTIPLICATION.FACTOR% =                              \                 !1.8NWB
\             VAL(UNPACK$(FPF.MULT.FACTOR$(FLASHPACK%))) / 10                  !1.8NWB
\      GOSUB CHECK.FP.SALES                                                    !1.8NWB
\      IF CITEM.TOTAL.ESA$ <> PACK$("0000")                        \           !1.8NWB
\      AND (RECORD.TYPE$ = "UNCOUNTED" OR RECORD.TYPE$ = "MANUAL")      \      !1.8NWB
\      AND (MID$(CSRBF.DATA$,13,1) = "N"                        \              !1.8NWB
\      OR MID$(CSRBF.DATA$,13,1) = "J")                        \               !1.8NWB
\      AND PROMOTION.DONE$ = "N" THEN BEGIN                                    !1.8NWB
\           PROMOTION.DONE$ = "Y"                                              !1.8NWB
\         CSRWF.QTY.2% = FP.NUMITEM% -                         \               !1.8NWB
\                        VAL(UNPACK$(CITEM.TOTAL.ON.ORDER$))                   !1.8NWB
\         PROMOTIONAL.SA% = VAL(UNPACK$(CITEM.TOTAL.ESA$))                     !1.8NWB
\         PROMOTIONAL.MF% = MULTIPLICATION.FACTOR%                             !1.8NWB
\         CSRWF.QTY.2.IN.SINGLES% =                              \             !1.8NWB
\               CSRWF.QTY.2% * MULTIPLICATION.FACTOR%                          !1.8NWB
\           CSRWF.QTY.2.IN.SINGLES% = FN.ROUND.DOWN(CSRWF.QTY.2.IN.SINGLES%)   !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\           CSRWF.QTY.1% = CSRWF.QTY.1% +                         \            !1.8NWB
\                          (FP.NUMITEM% * MULTIPLICATION.FACTOR%) -      \     !1.8NWB
\                        VAL(UNPACK$(CITEM.TOTAL.ON.ORDER$)) -        \        !1.8NWB
\                      VAL(UNPACK$(CITEM.ON.ORDER.IN.THIS.PDT$))               !1.8NWB
\           CSRWF.QTY.1% = FN.ROUND.DOWN(CSRWF.QTY.1%)                         !1.8NWB
\      ENDIF                                                                   !1.8NWB
\        TOTAL.SALES% = TOTAL.SALES% + FP.NUMITEM% * MULTIPLICATION.FACTOR%    !1.8NWB
\      TOTAL.SALES% = FN.ROUND.DOWN(TOTAL.SALES%)                              !1.8NWB
\      TOTAL.SA% = TOTAL.SA% +                              \                  !1.8NWB
\                VAL(UNPACK$(CITEM.TOTAL.ESA$)) *                  \           !1.8NWB
\                MULTIPLICATION.FACTOR%                                        !1.8NWB
\        TOTAL.SA% = FN.ROUND.DOWN(TOTAL.SA%)                                  !1.8NWB
\     CALC.FAMILY.FIGURES.CONTINUE2:                                           !1.8NWB
\      FLASHPACK% = FLASHPACK% + 1                                             !1.8NWB
\     WEND                                                                     !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB

\     IF CSRWF.QTY.1% > TOTAL.SA% THEN BEGIN                                   !1.8NWB
\      CSRWF.QTY.1% = TOTAL.SA%                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF CSRWF.QTY.2% < 0 THEN BEGIN                                           !1.8NWB
\        CSRWF.QTY.1% = CSRWF.QTY.1% + CSRWF.QTY.2.IN.SINGLES%                 !1.8NWB
\      CSRWF.QTY.2% = 0                                                        !1.8NWB
\      CSRWF.QTY.2.IN.SINGLES% = 0                                             !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        IF CSRWF.QTY.2% > PROMOTIONAL.SA% THEN BEGIN                          !1.8NWB
\         CSRWF.QTY.1% = CSRWF.QTY.1% +                        \               !1.8NWB
\                        (CSRWF.QTY.2% - PROMOTIONAL.SA%) *            \       !1.8NWB
\                    PROMOTIONAL.MF%                                           !1.8NWB
\           CSRWF.QTY.2% = PROMOTIONAL.SA%                                     !1.8NWB
\           CSRWF.QTY.2.IN.SINGLES% = PROMOTIONAL.SA% * PROMOTIONAL.MF%        !1.8NWB
\         CSRWF.QTY.2.IN.SINGLES% = FN.ROUND.DOWN(CSRWF.QTY.2.IN.SINGLES%)     !1.8NWB
\      ENDIF                                                                   !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF CSRWF.QTY.1% < 0 AND PROMOTION.DONE$ = "Y" THEN BEGIN                 !1.8NWB
\        CSRWF.QTY.2% = CSRWF.QTY.2% + (CSRWF.QTY.1% / PROMOTIONAL.MF%)        !1.8NWB
\      CSRWF.QTY.2% = FN.ROUND.DOWN(CSRWF.QTY.2%)                              !1.8NWB
\      CSRWF.QTY.1% = 0                                                        !1.8NWB
\      IF CSRWF.QTY.2% < 0 THEN CSRWF.QTY.2% = 0                               !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CSRWF.QTY.1$ = RIGHT$("000" + STR$(CSRWF.QTY.1%),3)                      !1.8NWB
\     CSRWF.QTY.2$ = RIGHT$("000" + STR$(CSRWF.QTY.2%),3)                      !1.8NWB
\     CSRWF.QTY.2.IN.SINGLES$ =                                    \           !1.8NWB
\                        RIGHT$("000" + STR$(CSRWF.QTY.2.IN.SINGLES%),3)       !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   CHECK.FP.SALES:
\***
\***     if the sales for this flashpack exceed the space available to it
\***        (which will be 0 unless the item is on promotion) then the
\***        sales for the flashpack will be set equal to the shelf alloc.
\***        The extra sales will be added to the sales for the parent.
\***
\***   RETURN
\***
\******************************************************************************

\CHECK.FP.SALES:                                                               !1.8NWB

\      IF CIMF.NUMITEM% > VAL(UNPACK$(CITEM.TOTAL.ESA$)) THEN BEGIN            !1.8NWB
\         RC% = READ.CIMF.LOCK                                         ! NNS   !1.8NWB
\         IF RC% = 1 THEN GOTO READ.ERROR                              ! NNS   !1.8NWB
\         ADJUST.PARENT.SALES% = ADJUST.PARENT.SALES% +                \       !1.8NWB
\                                (CIMF.NUMITEM% -                      \       !1.8NWB
\                                VAL(UNPACK$(CITEM.TOTAL.ESA$))) *     \       !1.8NWB
\                                MULTIPLICATION.FACTOR%                        !1.8NWB
\         ADJUST.PARENT.SALES% = FN.ROUND.DOWN(ADJUST.PARENT.SALES%)           !1.8NWB
\         CIMF.NUMITEM% = VAL(UNPACK$(CITEM.TOTAL.ESA$))                       !1.8NWB
\         CURR.SESS.NUM% = CIMF.SESS.NUM%                                      !1.8NWB
\         RC% = WRITE.CIMF.HOLD.UNLOCK                                 ! NNS   !1.8NWBLC
\         IF RC% = 1 THEN GOTO WRITE.ERROR                             ! HLC   !1.8NWB
\         CIMF.BOOTS.CODE$ =                                           \       !1.8NWB
\            PACK$("00" + MID$(UNPACK$(FPF.PARENT.CODE$),2,6))                 !1.8NWB
\         CURRENT.KEY$ = CIMF.BOOTS.CODE$                                      !1.8NWB
\         RC% = READ.CIMF.LOCK                                         ! HLC   !1.8NWB
\         IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC   !1.8NWB
\         CIMF.NUMITEM% = CIMF.NUMITEM% + ADJUST.PARENT.SALES%                 !1.8NWB
\         RC% = WRITE.CIMF.HOLD.UNLOCK                                 ! HLC   !1.8NWB
\         IF RC% = 1 THEN GOTO WRITE.ERROR                             ! HLC   !1.8NWB
\         ADJUST.PARENT.SALES% = 0                                             !1.8NWB
\      ENDIF                                                                   !1.8NWB

\RETURN                                                                        !1.8NWB

\******************************************************************************
\***
\***   WRITE.ORDER:
\***
\***     prepare either an "XO" (override) or "XC" (counted) record for the
\***     workfile
\***
\***     update the "on order in this pdt" figure and write
\***     the new record to the workfile
\***
\***   RETURN
\***
\******************************************************************************

\  WRITE.ORDER:                                                                !1.8NWB

\     IF RECORD.TYPE$ = "OVERRIDE" THEN      BEGIN                             !1.8NWB
\        CSRWF.RECORD.TYPE$ = "XO"                                             !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        CSRWF.RECORD.TYPE$ = "XC"                                             !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     WS.ON.ORDER.IN.THIS.PDT$ = PACK$(RIGHT$("0000" +                  \      !1.8NWB
\                        STR$(VAL(CSRWF.QTY.1$) +            \                 !1.8NWB
\                          VAL(CSRWF.QTY.2.IN.SINGLES$) +       \              !1.8NWB
\                  VAL(UNPACK$(WS.ON.ORDER.IN.THIS.PDT$))),4))                 !1.8NWB

\     IF ITEM.FOUND.ON.CITEM$ = "Y" THEN BEGIN                                 !1.8NWB
\        CURR.SESS.NUM% = CITEM.SESS.NUM%                                      !1.8NWB
\        CITEM.BOOTS.CODE$ = PARENT.CODE$                                      !1.8NWB
\      CIMF.BOOTS.CODE$ = PACK$("0" + LEFT$(UNPACK$(PARENT.CODE$),7))          !1.8NWB
\        CURRENT.KEY$ = PARENT.CODE$                                           !1.8NWB
\      RC% = READ.CITEM.LOCK                                    ! HLC          !1.8NWB
\        IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC          !1.8NWB
\        GOSUB MOVE.WS.TO.CITEM                                                !1.8NWB
\        IF WRITE.HOLD.CITEM.CIMF$ = "N" THEN BEGIN                            !1.8NWB
\           RC% = WRITE.CITEM.UNLOCK                              ! HLC        !1.8NWB
\           IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC      !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\           RC% = WRITE.CITEM.HOLD.UNLOCK                        ! HLC         !1.8NWB
\         IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC        !1.8NWB
\         CURR.SESS.NUM% = CIMF.SESS.NUM%                                      !1.8NWB
\         CURRENT.KEY$ = CIMF.BOOTS.CODE$                                      !1.8NWB
\         RC% = READ.CIMF.LOCK                              ! HLC              !1.8NWB
\         IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC         !1.8NWB
\         CIMF.NUMITEM% = CIMF.NUMITEM% + ADJUST.PARENT.SALES%                 !1.8NWB
\         RC% = WRITE.CIMF.HOLD.UNLOCK                        ! HLC            !1.8NWB
\         IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC        !1.8NWB
\         ADJUST.PARENT.SALES% = 0                                             !1.8NWB
\      ENDIF                                                                   !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     RC% = WRITE.CSRWF                                          ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   MOVE.CITEM.TO.WS:
\***
\***      save any values from CITEM which may alter in WS variables
\***
\***   RETURN
\***
\******************************************************************************

\  MOVE.CITEM.TO.WS:                                                           !1.8NWB

\     WS.TOTAL.ESA$ = CITEM.TOTAL.ESA$                                         !1.8NWB
\     WS.TOTAL.ON.ORDER$ = CITEM.TOTAL.ON.ORDER$                               !1.8NWB
\     WS.DATE.OF.LAST.MANUAL.COUNT$ = CITEM.DATE.OF.LAST.MANUAL.COUNT$         !1.8NWB
\     WS.SHELF.ALLOCATION$ = CITEM.SHELF.ALLOCATION$                           !1.8NWB
\     WS.UNIT$ = CITEM.UNIT$                                                   !1.8NWB
\     WS.LIST.FREQUENCY$ = CITEM.LIST.FREQUENCY$                               !1.8NWB
\     WS.SALES.SIGN.FLAG$ = CITEM.SALES.SIGN.FLAG$                   ! DPAB    !1.8NWB
\     WS.ON.ORDER.TODAY$ = CITEM.ON.ORDER.TODAY$                               !1.8NWB
\     WS.ON.ORDER.IN.THIS.PDT$ = CITEM.ON.ORDER.IN.THIS.PDT$                   !1.8NWB
\     WS.COUNT.REQUEST.FLAG$ = CITEM.COUNT.REQUEST.FLAG$            ! AGEF     !1.8NWB
\     WS.WEEK4.SALES$ = CITEM.WEEK.4.SALES$                        ! DPAB      !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   MOVE.WS.TO.CITEM:
\***
\***      restore all WS values to CITEM
\***
\***   RETURN
\***
\******************************************************************************

\  MOVE.WS.TO.CITEM:                                                           !1.8NWB

\     CITEM.TOTAL.ESA$ = WS.TOTAL.ESA$                                         !1.8NWB
\     CITEM.TOTAL.ON.ORDER$ = WS.TOTAL.ON.ORDER$                               !1.8NWB
\     CITEM.DATE.OF.LAST.MANUAL.COUNT$ = WS.DATE.OF.LAST.MANUAL.COUNT$         !1.8NWB
\     CITEM.SHELF.ALLOCATION$ = WS.SHELF.ALLOCATION$                           !1.8NWB
\     CITEM.UNIT$ = WS.UNIT$                                                   !1.8NWB
\     CITEM.LIST.FREQUENCY$ = WS.LIST.FREQUENCY$                               !1.8NWB
\     CITEM.SALES.SIGN.FLAG$ = WS.SALES.SIGN.FLAG$                   ! DPAB    !1.8NWB
\     CITEM.ON.ORDER.TODAY$ = WS.ON.ORDER.TODAY$                               !1.8NWB
\     CITEM.ON.ORDER.IN.THIS.PDT$ = WS.ON.ORDER.IN.THIS.PDT$                   !1.8NWB
\     CITEM.COUNT.REQUEST.FLAG$ = WS.COUNT.REQUEST.FLAG$            ! AGEF     !1.8NWB
\     CITEM.WEEK.4.SALES$ = WS.WEEK4.SALES$                        ! DPAB      !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   ABANDON.THIS.LIST:
\***
\***      sets all records for this list from the workfile to low values
\***      and resets the record pointer to overwrite them with the next list
\***      (if any - if not, the records of low values will be ignored when the
\***      workfile is processed)
\***      also decrement the "on order for this PDT" figure from CSRITEM
\***
\***   RETURN
\***
\******************************************************************************

\  ABANDON.THIS.LIST:                                                          !1.8NWB


\     LIST.ABANDONED$ = "Y"                                                    !1.8NWB

!      CSRWF.LIST.COUNT$ = RIGHT$("000" + STR$(VAL(CSRWF.LIST.COUNT$) - 1),3)
\     CSRWF.RECORD.NO% = CURRENT.HEADER.RECORD.NO%                             !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     RC% = READ.CSRWF                                          ! HLC          !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB
\     CSRWF.RECORD$ = PACK$(STRING$(48,"0"))                                   !1.8NWB
\     IF END #CSRWF.SESS.NUM% THEN WRITE.ERROR                                 !1.8NWB
\     WRITE FORM "C24"; #CSRWF.SESS.NUM%, CSRWF.RECORD.NO%; CSRWF.RECORD$      !1.8NWB

\     END.OF.CSRWF$ = "N"                                                      !1.8NWB
\     WHILE END.OF.CSRWF$ = "N"                                                !1.8NWB
\        CURR.SESS.NUM% = CSRWF.SESS.NUM%                                      !1.8NWB
\      CURRENT.KEY$ = ""                                                       !1.8NWB
\      RC% = READ.CSRWF                                    ! HLC               !1.8NWB
\      IF RC% = 1 THEN GOTO END.OF.CSRWF1                        ! HLC         !1.8NWB
\      IF CSRWF.RECORD$ = PACK$(STRING$(48," ")) THEN GOTO END.OF.CSRWF1       !1.8NWB
\      IF CSRWF.RECORD.TYPE$ = "XC" THEN BEGIN                                 !1.8NWB
\         CITEM.BOOTS.CODE$ = CSRWF.P.ITEM.CODE$                               !1.8NWB
\         CSRWF.QTY.1% = VAL(CSRWF.QTY.1$) +                         \         !1.8NWB
\                        VAL(CSRWF.QTY.2.IN.SINGLES$)                          !1.8NWB
\      ENDIF ELSE BEGIN                                                        !1.8NWB
\         CITEM.BOOTS.CODE$ = PACK$("0" + CSRWF.ITEM.CODE$)                    !1.8NWB
\         CSRWF.QTY.1% = VAL(CSRWF.QTY.1$)                                     !1.8NWB
\      ENDIF                                                                   !1.8NWB
\      CURR.SESS.NUM% = CITEM.SESS.NUM%                                        !1.8NWB
\      CURRENT.KEY$ = CITEM.BOOTS.CODE$                                        !1.8NWB
\      RC% = READ.CITEM.LOCK                                    ! HLC          !1.8NWB
\      IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC            !1.8NWB
\      IF VAL(UNPACK$(CITEM.ON.ORDER.IN.THIS.PDT$)) >=            \            !1.8NWB
\                                   CSRWF.QTY.1% THEN BEGIN                    !1.8NWB
\           CITEM.ON.ORDER.IN.THIS.PDT$ = PACK$(RIGHT$("0000" +            \   !1.8NWB
\              STR$(VAL(UNPACK$(CITEM.ON.ORDER.IN.THIS.PDT$)) -            \   !1.8NWB
\            CSRWF.QTY.1%),4))                                                 !1.8NWB
\      ENDIF ELSE BEGIN                                                        !1.8NWB
\         CITEM.ON.ORDER.IN.THIS.PDT$ = PACK$("0000")                          !1.8NWB
\      ENDIF                                                                   !1.8NWB
\      RC% = WRITE.CITEM.UNLOCK                               ! HLC            !1.8NWB
\      IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC           !1.8NWB
\      CURR.SESS.NUM% = CSRWF.SESS.NUM%                                        !1.8NWB
\      CURRENT.KEY$ = ""                                                       !1.8NWB
\        CSRWF.RECORD$ = PACK$(STRING$(48,"0"))                                !1.8NWB
\        IF END #CSRWF.SESS.NUM% THEN WRITE.ERROR                              !1.8NWB
\        WRITE FORM "C24"; #CSRWF.SESS.NUM%, CSRWF.RECORD.NO%; CSRWF.RECORD$   !1.8NWB
\     ABANDON.LIST.CONTINUE:                                                   !1.8NWB
\     WEND                                                                     !1.8NWB

\     CSRWF.RECORD.NO% = CURRENT.HEADER.RECORD.NO% - 1                         !1.8NWB

\  RETURN                                                                      !1.8NWB

\  END.OF.CSRWF1:                                                              !1.8NWB
\     END.OF.CSRWF$ = "Y"                                                      !1.8NWB
\  GOTO ABANDON.LIST.CONTINUE                                                  !1.8NWB

\******************************************************************************
\***
\***   PROCESS.CSR.WORKFILE:
\***
\***     at this point the PDT has indicated that it has received all the lists
\***     successfully, and the data in CSRWF can be used to update the
\***     files.
\***     if the DELETE flag is already set off then the file can be deleted.
\***     the DELETE flag on the first record of the file will be set off -
\***     ie. until the CSRWF has been successfully processed this file must
\***     not be deleted
\***     read through the file:
\***        "unprocess" any lists which require it
\***        update the ONORDER file and CSRITEM (on order totals) and
\***        the CSRIMF figure if this is a non-zero override order
\***     Because it is already too late to do anything about the lists, as
\***     they are already in the PDT, this file will not be checked for
\***     completeness; each record will be processed as best it can.
\***
\***   RETURN
\***
\******************************************************************************

\  PROCESS.CSR.WORKFILE:                                                       !1.8NWB

\     SB.MESSAGE$ = "PDT Support - CSR files being updated"                    !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB

\     IF CSRWF.OPEN.FLAG$ <> "Y" THEN BEGIN                                    !1.8NWB
\        CURR.SESS.NUM% = CSRWF.SESS.NUM%                                      !1.8NWB
\        IF END #CSRWF.SESS.NUM% THEN OPEN.ERROR                               !1.8NWB
\        OPEN CSRWF.FILE.NAME$ DIRECT RECL CSRWF.RECL% AS CSRWF.SESS.NUM%      !1.8NWB
\        CSRWF.OPEN.FLAG$ = "Y"                                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CSRWF.RECORD.NO% = 1                                                     !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = READ.CSRWF                                          ! HLC          !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB

\     IF CSRWF.DELETE.FLAG$ = "Y" THEN BEGIN                                   !1.8NWB
\        CSRWF.OPEN.FLAG$ = "N"                                                !1.8NWB
\        IF END #CSRWF.SESS.NUM% THEN DELETE.ERROR                             !1.8NWB
\        DELETE CSRWF.SESS.NUM%                                                !1.8NWB
\      PROCESS.CSR.WORKFILE$ = "N"                                             !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                  !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     RC% = READ.CSRWF                                           ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB

\     PROCESSING.WORKFILE$ = "Y"                                               !1.8NWB
\     END.OF.CSRWF$ = "N"                                                      !1.8NWB
\     WHILE END.OF.CSRWF$ <> "Y"                                               !1.8NWB
\        IF CSRWF.RECORD.TYPE$ = "XH" THEN BEGIN                               !1.8NWB
\           FIRST.ITEM.TO.BE.PROCESSED$ = "Y"                                  !1.8NWB
\         IF CSRWF.UNPROCESS.FLAG$ = "Y" THEN      BEGIN                       !1.8NWB
\            GOSUB UNPROCESS.THIS.LIST                                         !1.8NWB
\         ENDIF                                                                !1.8NWB
\      ENDIF                                                                   !1.8NWB
\      IF CSRWF.RECORD.TYPE$ = "XC"                              \             !1.8NWB
\      OR CSRWF.RECORD.TYPE$ = "XO" THEN BEGIN                                 !1.8NWB
\         GOSUB SET.UP.ITEM.CODE                                               !1.8NWB
\         GOSUB UPDATE.CITEM                                                   !1.8NWB
\         IF CSRWF.CSRITEM.UPDATED.FLAG$ <> "Y" THEN BEGIN                     !1.8NWB
\               CSRWF.CSRITEM.UPDATED.FLAG$ = "Y"                              !1.8NWB
\            GOSUB UPDATE.CSRWF                                                !1.8NWB
\         ENDIF                                                                !1.8NWB
\         GOSUB UPDATE.ONORDER                                                 !1.8NWB
\         IF CSRWF.ONORDER.UPDATED.FLAG$ <> "Y" THEN BEGIN                     !1.8NWB
\            CSRWF.ONORDER.UPDATED.FLAG$ = "Y"                                 !1.8NWB
\            GOSUB UPDATE.CSRWF                                                !1.8NWB
\         ENDIF                                                                !1.8NWB
\      ENDIF                                                                   !1.8NWB
\  NEXT.ITEM:                                                                  !1.8NWB
\      CSRWF.RECORD.NO% = CSRWF.RECORD.NO% + 1                                 !1.8NWB
\      CURR.SESS.NUM% = CSRWF.SESS.NUM%                                        !1.8NWB
\      CURRENT.KEY$ = ""                                                       !1.8NWB
\      RC% = READ.CSRWF                                    ! HLC               !1.8NWB
\      IF RC% = 1 THEN GOTO END.OF.CSRWF                        ! HLC          !1.8NWB
\     PROCESS.CSRWF.CONTINUE:                                                  !1.8NWB
\     WEND                                                                     !1.8NWB
\     PROCESSING.WORKFILE$ = "N"                                               !1.8NWB

\     CSRWF.RECORD.NO% = 1                                                     !1.8NWB
\     CSRWF.DELETE.FLAG$ = "Y"                                                 !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     RC% = WRITE.CSRWF                                          ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

!      IF END #CSRWF.SESS.NUM% THEN DELETE.ERROR                  ! ASW
!      DELETE CSRWF.SESS.NUM%                                    ! ASW
!      CSRWF.OPEN.FLAG$ = "N"                                    ! ASW
\     PROCESS.CSR.WORKFILE$ = "N"                                              !1.8NWB

\     CLOSE IDF.SESS.NUM%                                                      !1.8NWB
\     IDF.OPEN.FLAG$ = "N"                                                     !1.8NWB
\     CLOSE CITEM.SESS.NUM%                                                    !1.8NWB
\     CITEM.OPEN.FLAG$ = "N"                                                   !1.8NWB
\     CLOSE CIMF.SESS.NUM%                                                     !1.8NWB
\     CIMF.OPEN.FLAG$ = "N"                                                    !1.8NWB
\     CLOSE ONORD.SESS.NUM%                                                    !1.8NWB
\     ONORD.OPEN.FLAG$ = "N"                                                   !1.8NWB
\     CLOSE FPF.SESS.NUM%                                                      !1.8NWB
\     FPF.OPEN.FLAG$ = "N"                                                     !1.8NWB
\     CLOSE IDSOF.SESS.NUM%                                    ! ILC           !1.8NWB
\     IDSOF.OPEN.FLAG$ = "N"                                    ! ILC          !1.8NWB

\     PROCESS.CSR.WORKFILE$ = "N"                                              !1.8NWB

\  RETURN                                                                      !1.8NWB

\  END.OF.CSRWF:                                                               !1.8NWB
\     END.OF.CSRWF$ = "Y"                                                      !1.8NWB
\  GOTO PROCESS.CSRWF.CONTINUE                                                 !1.8NWB

\******************************************************************************
\***
\***   SET.UP.ITEM.CODE:
\***
\***      set up the CITEM item code from data on the record from CSRWF,
\***      according to the record type
\***
\***   RETURN
\***
\******************************************************************************

\  SET.UP.ITEM.CODE:                                                           !1.8NWB

\     IF LEFT$(CSRWF.RECORD$,2) = "XO" THEN BEGIN                              !1.8NWB
\        CITEM.BOOTS.CODE$ = PACK$("0" + CSRWF.ITEM.CODE$)                     !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        CITEM.BOOTS.CODE$ = CSRWF.P.ITEM.CODE$                                !1.8NWB
\     ENDIF                                                                    !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   UNPROCESS.THIS.LIST:
\***
\***      from the header record on ONORDER for this list, save the highest
\***      sequence number used.
\***      for each record ONORDER for this list,
\***         decrement the onorder totals (today and total) on CSRITEM
\***         delete the ONORDER record
\***      delete the header record and set the CSRWF unprocess flag off
\***
\***   RETURN
\***
\******************************************************************************

\  UNPROCESS.THIS.LIST:                                                        !1.8NWB

\     LOCATION$ = "UNPROC"                                                     !1.8NWB
\     ONORD.ORDER.DATE$ = CSRWF.ORDER.DATE$                                    !1.8NWB
\     ONORD.LIST.FREQ$ = CSRWF.LIST.FREQ$                                      !1.8NWB
\     ONORD.UNIT.NO$ = CSRWF.UNIT.NO$                                          !1.8NWB
\     ONORD.SEQ.NO$ = "000"                                                    !1.8NWB
\     PART.KEY$ = ONORD.ORDER.DATE$ +                              \           !1.8NWB
\                   ONORD.LIST.FREQ$ +                              \          !1.8NWB
\               ONORD.UNIT.NO$                                                 !1.8NWB
\     CURR.SESS.NUM% = ONORD.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = PART.KEY$ + ONORD.SEQ.NO$                                 !1.8NWB
\     RC% = READ.ONORDER                                    ! HLC              !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB
\                                                                              !1.8NWB
\     FOR I% = 1 TO VAL(ONORD.HIGHEST.SEQ.NO$)                                 !1.8NWB
\        ONORD.SEQ.NO$ = RIGHT$("000" + STR$(I%),3)                            !1.8NWB
\      CURRENT.KEY$ = PART.KEY$ + ONORD.SEQ.NO$                                !1.8NWB
\      RC% = READ.ONORDER                                    ! HLC             !1.8NWB
\      IF RC% = 1 THEN GOTO READ.ERROR                        ! HLC            !1.8NWB
\      IF ONORD.CSRITEM.UNDONE.FLAG$ = "N" THEN BEGIN                          !1.8NWB
\         ONORD.CSRITEM.UNDONE.FLAG$ = "Y"                                     !1.8NWB
\         RC% = WRITE.ONORDER.HOLD                              ! HLC          !1.8NWB
\         IF RC% = 1 THEN GOTO WRITE.ERROR                        ! HLC        !1.8NWB
\         GOSUB UNDO.CSRITEM                                                   !1.8NWB
\      ENDIF                                                                   !1.8NWB
\      CURRENT.KEY$ = PART.KEY$ + ONORD.SEQ.NO$                                !1.8NWB
\      IF END #ONORD.SESS.NUM% THEN DELETE.ERROR                               !1.8NWB
\            DELREC ONORD.SESS.NUM%; PART.KEY$ + ONORD.SEQ.NO$                 !1.8NWB
\  UNPROCESS.LIST.CONTINUE:                                                    !1.8NWB
\     NEXT I%                                                                  !1.8NWB
\     CURRENT.KEY$ = PART.KEY$ + "000"                                         !1.8NWB
\     IF END #ONORD.SESS.NUM% THEN DELETE.ERROR                                !1.8NWB
\     DELREC ONORD.SESS.NUM%; PART.KEY$ + "000"                                !1.8NWB
\     CSRWF.UNPROCESS.FLAG$ = "N"                                              !1.8NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     RC% = WRITE.CSRWF                                          ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB
\     LOCATION$ = ""                                                           !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   UNDO.CSRITEM:
\***
\***      decrement on order total and on order today by the amount
\***      originally ordered
\***
\***   RETURN
\***
\******************************************************************************

\  UNDO.CSRITEM:                                                               !1.8NWB

\     CITEM.BOOTS.CODE$ = PACK$("0" + ONORD.ITEM.CODE$)                        !1.8NWB
\     CURRENT.KEY$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     RC% = READ.CITEM.LOCK                                    ! HLC           !1.8NWB
\     IF RC% = 1 THEN GOTO READ.ERROR                              ! HLC       !1.8NWB

\     TEMP% = VAL(UNPACK$(CITEM.ON.ORDER.TODAY$))                              !1.8NWB
\     TEMP% = TEMP% - VAL(ONORD.QTY.1$)                                        !1.8NWB
\     CITEM.ON.ORDER.TODAY$ = PACK$(RIGHT$("0000" + STR$(TEMP%),4))            !1.8NWB

\     RC% = WRITE.CITEM.HOLD.UNLOCK                              ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\  UNDO.CSRITEM.CONTINUE:                                    ! BPAB            !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   UPDATE.CITEM:
\***
\***     decrement the onorder in this PDT figure (but not below 0)
\***     increment on order today and on order total by the amount ordered
\***
\***   RETURN
\***
\******************************************************************************

\  UPDATE.CITEM:                                                               !1.8NWB

\     IF CSRWF.CSRITEM.UPDATED.FLAG$ = "Y" THEN RETURN                         !1.8NWB

\     CURRENT.KEY$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     RC% = READ.CITEM.LOCK                                    ! HLC           !1.8NWB
\     IF RC% = 1 THEN GOTO NOT.ON.CITEM                              ! HLC     !1.8NWB

\     QTY% = VAL(CSRWF.QTY.1$) + VAL(CSRWF.QTY.2.IN.SINGLES$)                  !1.8NWB
\     CITEM.ON.ORDER.IN.THIS.PDT$ = PACK$("0000")                              !1.8NWB

\     TEMP% = VAL(UNPACK$(CITEM.ON.ORDER.TODAY$)) + QTY%                       !1.8NWB
\     CITEM.ON.ORDER.TODAY$ = PACK$(RIGHT$("0000" + STR$(TEMP%), 4))      ! DSW!1.8NWB

\     TEMP% = VAL(UNPACK$(CITEM.TOTAL.ON.ORDER$)) + QTY%                       !1.8NWB
\     CITEM.TOTAL.ON.ORDER$ = PACK$(RIGHT$("0000" + STR$(TEMP%), 4))      ! DSW!1.8NWB

\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = CITEM.BOOTS.CODE$                                         !1.8NWB
\     RC% = WRITE.CITEM.HOLD.UNLOCK                              ! HLC         !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\  NOT.ON.CITEM.RETURN:                                                        !1.8NWB
\  RETURN                                                                      !1.8NWB

\  NOT.ON.CITEM:                                                               !1.8NWB
\     CSRWF.CSRITEM.UPDATED.FLAG$ = "Y"                                        !1.8NWB
\  GOTO NOT.ON.CITEM.RETURN                                                    !1.8NWB

\******************************************************************************
\***
\***   UPDATE.CSRWF:
\***
\***   RETURN
\***
\******************************************************************************

\  UPDATE.CSRWF:                                                               !1.8NWB

\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = ""                                                        !1.8NWB
\     RC% = WRITE.CSRWF.HOLD                                    ! HLC          !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   UPDATE.ONORDER:
\***
\***   RETURN
\***
\******************************************************************************

\  UPDATE.ONORDER:                                                             !1.8NWB

\     IF FIRST.ITEM.TO.BE.PROCESSED$ = "Y" THEN BEGIN                          !1.8NWB
\        FIRST.ITEM.TO.BE.PROCESSED$ = "N"                                     !1.8NWB
\        ONORD.ORDER.DATE$ = CSRWF.ORDER.DATE$                                 !1.8NWB
\        ONORD.LIST.FREQ$ = CSRWF.LIST.FREQ$                                   !1.8NWB
\        ONORD.UNIT.NO$ = CSRWF.UNIT.NO$                                       !1.8NWB
\        ONORD.SEQ.NO$ = "000"                                                 !1.8NWB
\      PART.KEY$ = DATE$ + ONORD.LIST.FREQ$ +                          \       !1.8NWB
\                           ONORD.UNIT.NO$                                     !1.8NWB
\        ONORD.SEQ.NO% = 0                                                     !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     ONORD.SEQ.NO% = ONORD.SEQ.NO% + 1                                        !1.8NWB
\     ONORD.SEQ.NO$ = RIGHT$("000" + STR$(ONORD.SEQ.NO%),3)                    !1.8NWB

\     IF CSRWF.HD.ONORDER.UPDATED.FLAG$ = "Y"                        \ BGEF    !1.8NWB
\     OR CSRWF.ONORDER.UPDATED.FLAG$ = "Y" THEN RETURN                  ! BGEF !1.8NWB

\     GOSUB SET.UP.ITEM.CODE                                                   !1.8NWB
\     ONORD.ITEM.CODE$ = RIGHT$(UNPACK$(CITEM.BOOTS.CODE$),7)                  !1.8NWB
\     ONORD.QTY.1$ = CSRWF.QTY.1$                                              !1.8NWB
\     ONORD.QTY.2$ = CSRWF.QTY.2$                                              !1.8NWB
\     ONORD.CSRITEM.UNDONE.FLAG$ = "N"                                         !1.8NWB
\     IF RECORD.TYPE$ = "OVERRIDE"                              \              !1.8NWB
\     AND CSRWF.ZERO.OVERRIDE.FLAG$ = "N" THEN BEGIN                           !1.8NWB
\        ONORD.CSRIMF.UNDONE.FLAG$ = "N"                                       !1.8NWB
\     ENDIF ELSE BEGIN                                                         !1.8NWB
\        ONORD.CSRIMF.UNDONE.FLAG$ = "Y"                                       !1.8NWB
\     ENDIF                                                                    !1.8NWB
\     ONORD.DELETED.FLAG$ = "N"                                                !1.8NWB
\     CURR.SESS.NUM% = ONORD.SESS.NUM%                                         !1.8NWB
\     CURRENT.KEY$ = PART.KEY$ + ONORD.SEQ.NO$                                 !1.8NWB
\     RC% = WRITE.ONORDER                                      ! HLC           !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\     ONORD.HIGHEST.SEQ.NO$ = RIGHT$("000" + STR$(ONORD.SEQ.NO%),3)            !1.8NWB
\     ONORD.SEQ.NO$ = "000"                                                    !1.8NWB
\     CURRENT.KEY$ = PART.KEY$ + ONORD.SEQ.NO$                                 !1.8NWB
\     RC% = WRITE.ONORDER.HOLD                                    ! HLC        !1.8NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                              ! HLC      !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\******************************************************************************
\***
\***   P R I C E C H E C K I N G     S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   RECEIVED.PCHECK.HEADER:                            STATE : O
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      open CHKBF file and append data if it exists, otherwise create CHKBF
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.PCHECK.HEADER:

      SB.MESSAGE$ = "PDT Support - PCHECK header received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 16) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"                                           ! GDS
         RETURN
      ENDIF

      IF GAPBF.OPEN.FLAG$ <> "Y" THEN BEGIN                             ! 1.3 !1.7BG
         CURR.SESS.NUM% = GAPBF.SESS.NUM%                               ! 1.3 !1.7BG
         IF END #GAPBF.SESS.NUM% THEN CREATE.GAPBF                      ! 1.3 !1.7BG
         OPEN GAPBF.FILE.NAME$ AS GAPBF.SESS.NUM% APPEND                ! 1.3 !1.7BG
         GOTO GAPBF.OPENED.OK                                           ! 1.3 !1.7BG
         CREATE.GAPBF:                                                  ! 1.3 !1.7BG
         IF END #GAPBF.SESS.NUM% THEN CREATE.ERROR                      ! 1.3 !1.7BG
         CREATE GAPBF.FILE.NAME$ AS GAPBF.SESS.NUM%                     ! 1.3 !1.7BG
         GAPBF.OPENED.OK:                                               ! 1.3 !1.7BG
         GAPBF.OPEN.FLAG$ = "Y"                                         ! 1.3 !1.7BG
         ALL.FILES.CLOSED$ = "N"                                        ! 1.3 !1.7BG
      ENDIF                                                                   !1.7BG

                                                                            !1.5CS
      BTSCNT% = 0                                                           !1.5CS
      EANCNT% = 0                                                           !1.5CS

      IF IRF.OPEN.FLAG$ <> "Y" THEN BEGIN                                   !1.5CS
         CURR.SESS.NUM% = IRF.SESS.NUM%                                     !1.5CS
         IF END #IRF.SESS.NUM% THEN OPEN.ERROR                              !1.5CS
         OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%          \1.5CS
              NOWRITE NODEL                                                 !1.5CS
         IRF.OPEN.FLAG$ = "Y"                                               !1.5CS
      ENDIF                                                                 !1.5CS
                                                                            !1.5CS
      IF PLLOL.OPEN.FLAG$ <> "Y" THEN BEGIN                                 !1.5CS
         CURR.SESS.NUM% = PLLOL.SESS.NUM%                                   !1.5CS
         IF END # PLLOL.SESS.NUM% THEN CREATE.PLLOL                         !1.5CS
         OPEN PLLOL.FILE.NAME$ DIRECT RECL PLLOL.RECL% AS PLLOL.SESS.NUM% NODEL    !1.5CS
         GOTO PLLOL.OPENED.OK                                               !1.5CS
                                                                            !1.5CS
       CREATE.PLLOL:                                                        !1.5CS
       !------------                                                        !1.5CS
         IF END #PLLOL.SESS.NUM% THEN CREATE.ERROR                          !1.5CS
         CREATE POSFILE "PLLOL" DIRECT 0 RECL 34 AS PLLOL.SESS.NUM% MIRRORED PERUPDATE !1.5CS
         CLOSE PLLOL.SESS.NUM%                                              !1.5CS
         OPEN "PLLOL" DIRECT RECL 34 AS PLLOL.SESS.NUM%                     !1.5CS
         PLLOL.RECORD.NUM% = 1                                              !1.5CS
         PLLOL.LISTID$          = "000"                                     !1.5CS
         PLLOL.CREATOR.ID$      = "   "                                     !1.5CS
         PLLOL.PICKER.ID$       = "   "                                     !1.5CS
         PLLOL.ITEM.STATUS$     = " "                                       !1.5CS
         PLLOL.CREATE.DATE$     = DATE$                                     !1.5CS
         PLLOL.CREATE.TIME$     = LEFT$(TIME$,4)                            !1.5CS
         PLLOL.PICK.START.TIME$ = "    "                                    !1.5CS
         PLLOL.PICK.END.TIME$   = "    "                                    !1.5CS
         PLLOL.ITEM.COUNT$      = "    "                                    !1.5CS
         PLLOL.OSSR.PICKING$    = " "                                       !1.5CS
                                                                            !1.5CS
         RC% = WRITE.PLLOL                                                  !1.5CS
         IF RC% = 1 THEN GOTO WRITE.ERROR                                   !1.5CS
                                                                            !1.5CS
       PLLOL.OPENED.OK:                                                     !1.5CS
       !---------------                                                     !1.5CS
         PLLOL.OPEN.FLAG$ = "Y"                                             !1.5CS
         IF PLLDB.OPEN.FLAG$ <> "Y" THEN BEGIN                              !1.5CS
             CURR.SESS.NUM% = PLLDB.SESS.NUM%                               !1.5CS
             IF END # PLLDB.SESS.NUM% THEN CREATE.PLLDB                     !1.5CS
!             OPEN "PLLDB" KEYED RECL 164 AS PLLDB.SESS.NUM% NODEL           !1.5CS 1.8PJS !1.10TT
!             OPEN "PLLDB" KEYED RECL 384 AS PLLDB.SESS.NUM% NODEL           !1.10TT !1.11TT
             OPEN PLLDB.FILE.NAME$ KEYED RECL PLLDB.RECL% AS                \1.11TT
                              PLLDB.SESS.NUM% NODEL                         !1.11TT
             GOTO PLLDB.OPENED.OK                                           !1.5CS
                                                                            !1.5CS
           CREATE.PLLDB:                                                    !1.5CS
           !------------                                                    !1.5CS
             IF END #PLLDB.SESS.NUM% THEN CREATE.ERROR                      !1.5CS
!             CREATE POSFILE "PLLDB" KEYED 6,553,4,10000 RECL 164 AS PLLDB.SESS.NUM% MIRRORED PERUPDATE  !1.5CS 1.8PJS ! 1.11TT
             CREATE POSFILE PLLDB.FILE.NAME$ KEYED 6,,,16000 \               1.11TT
                   RECL PLLDB.RECL% AS PLLDB.SESS.NUM% MIRRORED PERUPDATE   !1.11TT
         ENDIF                                                              !1.5CS
                                                                            !1.5CS
       PLLDB.OPENED.OK:                                                     !1.5CS
       !--------------                                                      !1.5CS
      ENDIF ELSE BEGIN                                                  ! 1.3
         IF PCHK.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.3
            CLOSE PCHK.SESS.NUM%                                        ! 1.3
            PCHK.OPEN.FLAG$ = "N"                                       ! 1.3
         ENDIF                                                          ! 1.3
         RECEIVE.STATE$ = "*"                                           ! 1.3
         RETURN                                                         ! 1.3
      ENDIF                                                             ! 1.3

      SMON.NEW.LIST.CREATED = FALSE                                     !1.5CS

      IF CHKBF.OPEN.FLAG$ <> "Y" THEN BEGIN
         CURR.SESS.NUM% = CHKBF.SESS.NUM%
         IF END #CHKBF.SESS.NUM% THEN CREATE.CHKBF
         OPEN CHKBF.FILE.NAME$ DIRECT RECL CHKBF.RECL%                  \ HLC
            AS CHKBF.SESS.NUM%                                          ! HLC
         CHKBF.POINTER% = (SIZE(CHKBF.FILE.NAME$) / CHKBF.RECL%) + 1    ! HLC
         GOTO CHKBF.OPENED.OK                                           ! HLC
         CREATE.CHKBF:                                                  ! HLC
         IF END #CHKBF.SESS.NUM% THEN CREATE.ERROR
         CREATE CHKBF.FILE.NAME$ DIRECT RECL CHKBF.RECL%                \
            AS CHKBF.SESS.NUM%
         CHKBF.POINTER% = 1                                             ! HLC
         CHKBF.OPENED.OK:                                               ! HLC
         CHKBF.OPEN.FLAG$ = "Y"
         ALL.FILES.CLOSED$ = "N"
      ENDIF ELSE BEGIN
         IF PCHK.OPEN.FLAG$ = "Y" THEN BEGIN
            CLOSE PCHK.SESS.NUM%
            PCHK.OPEN.FLAG$ = "N"
         ENDIF
         RECEIVE.STATE$ = "*"                                           ! GDS
      ENDIF

   RETURN

\******************************************************************************
\***
\***   RECEIVED.PCHECK.RECORD:                              STATE : P
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      check CHKBF file is open, if ok...
\***      ...write a record to the CHKBF file for each record received
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.PCHECK.RECORD:

      SB.MESSAGE$ = "PDT Support - PCHECK record received"
      GOSUB SB.BG.MESSAGE

      IF LEN(DATA.IN$) = 21 THEN BEGIN                                        !1.7BG
         !Check against old style record layout                               !1.7BG
         IF FN.VALIDATE.DATA(DATA.IN$, 17) = 0 THEN BEGIN
               RECEIVE.STATE$ = "*"                                           ! GDS
               RETURN
         ENDIF
         OLD.PCHECK.RECORD% = TRUE                                            !1.7BG
      ENDIF ELSE BEGIN                                                        !1.7BG
         !Check against new style record layout                               !1.7BG
         IF FN.VALIDATE.DATA(DATA.IN$, 54) = 0 THEN BEGIN                     !1.7BG
            RECEIVE.STATE$ = "*"                                              !1.7BG
            RETURN                                                            !1.7BG
         ENDIF                                                                !1.7BG
         OLD.PCHECK.RECORD% = FALSE                                           !1.7BG
      ENDIF                                                                   !1.7BG

      IF MID$(DATA.IN$, 16, 6) = "000001" THEN BEGIN                    ! 1.3

         IF OLD.PCHECK.RECORD% THEN BEGIN                                     !1.7BG

            CURR.SESS.NUM% = GAPBF.SESS.NUM%
            IF GAPBF.OPEN.FLAG$ = "Y" THEN BEGIN                        ! 1.3 !1.7BG
               GAPBF.BOOTS.CODE$ = MID$(DATA.IN$, 3, 13)                ! 1.3 !1.7BG
               RC% = WRITE.GAPBF                                        ! 1.3 !1.7BG
               IF RC% = 1 THEN BEGIN                                    ! 1.3 !1.7BG
                  GOTO WRITE.ERROR                                      ! 1.3 !1.7BG
               ENDIF ELSE BEGIN                                         ! 1.4 !1.7BG
                  GAP.RECORD.EXISTS = TRUE                              ! 1.4 !1.7BG
               ENDIF                                                    ! 1.3 !1.7BG
            ENDIF ELSE BEGIN                                            ! 1.3 !1.7BG
               RECEIVE.STATE$ = "*"                                     ! 1.3 !1.7BG
            ENDIF                                                       ! 1.3 !1.7BG

         ENDIF ELSE BEGIN

            IF SMON.NEW.LIST.CREATED = FALSE THEN BEGIN                           !1.5CS
               PLLDB.OPEN.FLAG$ = "Y"                                             !1.5CS
               ALL.FILES.CLOSED$ = "N"                                            !1.5CS
               WS.SEQ.NO% = 0                                                     !1.5CS
               !-----------------------------------------------------             !1.5CS
               ! Generate New List ID record in PLLOL & update header             !1.5CS
               ! New record offset + 1 as header is record 1.                     !1.5CS
               !-----------------------------------------------------             !1.5CS
               PLLOL.RECORD.NUM% = 1                                              !1.5CS
               RC% = READ.PLLOL                                                   !1.5CS
               IF RC% = 1 THEN GOTO READ.ERROR                                    !1.5CS
               WS.NEW.LISTID% = VAL(PLLOL.LISTID$) + 1                            !1.5CS
               PLLOL.LISTID$ = RIGHT$("000" + STR$(WS.NEW.LISTID%),3)             !1.5CS
               PLLOL.RECORD.NUM% = 1                                              !1.5CS
               RC% = WRITE.PLLOL                                                  !1.5CS
               IF RC% = 1 THEN GOTO WRITE.ERROR                                   !1.5CS
               ! Record 6-digit PDT No. in Creator & Picker variables             !1.5CS
               PLLOL.CREATOR.ID$      = LEFT$(CURR.TERMINAL$, 3)                  !1.5CS
               PLLOL.PICKER.ID$       = RIGHT$(CURR.TERMINAL$, 3)                 !1.5CS
               PLLOL.ITEM.STATUS$     = " "                                       !1.5CS
               PLLOL.CREATE.DATE$     = DATE$                                     !1.5CS
               PLLOL.CREATE.TIME$     = LEFT$(TIME$,4)                            !1.5CS
               PLLOL.PICK.START.TIME$ = "0000"                                    !1.5CS
               PLLOL.PICK.END.TIME$   = "0000"                                    !1.5CS
               PLLOL.ITEM.COUNT$      = "0000"                                    !1.5CS
               PLLOL.OSSR.PICKING$    = "Y"                                       !1.5CS
               PLLOL.RECORD.NUM% = WS.NEW.LISTID% + 1                             !1.5CS
               RC% = WRITE.PLLOL                                                  !1.5CS
               IF RC% = 1 THEN GOTO WRITE.ERROR                                   !1.5CS

               SMON.NEW.LIST.CREATED = TRUE                                       !1.5CS
            ENDIF

            CURR.SESS.NUM% = PLLDB.SESS.NUM%                                                   !1.5CS
            IF PLLDB.OPEN.FLAG$ = "Y" THEN BEGIN                                               !1.5CS
                                                                                               !1.5CS
                !------------------------------                                                !1.5CS
                ! Add Boots Items to BTS table                                                 !1.5CS
                !------------------------------                                                !1.5CS
                WS.SEQ.NO% = WS.SEQ.NO% + 1                                                    !1.5CS
                PLLDB.LISTID$ = PLLOL.LISTID$                                                  !1.5CS
                PLLDB.ITEMSEQ$ = RIGHT$("000" + STR$(WS.SEQ.NO%),3)                            !1.5CS
                PLLDB.QTY.ONSHELF$ = "0" + MID$(DATA.IN$, 22, 3)                               !1.5CS
                PLLDB.FILL.QTY$    = "0" + MID$(DATA.IN$, 25, 3)                               !1.5CS

                ! If non-Boots Code, lookup barcode on IRF to                                  !1.5CS
                ! obtain it and calculate the check digit                                      !1.5CS
                IRF.BAR.CODE$ = PACK$("0000000000" + MID$(DATA.IN$, 3, 12))                    !1.5CS
                CURR.SESS.NUM% = IRF.SESS.NUM%                                                 !1.5CS
                RC% = READ.IRF                                                                 !1.5CS

                IF RC% <> 0 THEN BEGIN                                                         !1.5CS
                    ! Item not setup on system, so set Boots Item Code to all 9's              !1.5CS
                    ! & insert barcode into unused PLLDB vaiables in order that                !1.5CS
                    ! it can be reported as NOT ON FILE by PST47                               !1.5CS
                    BTSCNT% = BTSCNT% + 1                                                      !1.5CS
                    PLLDB.TABLE$(BTSCNT%) = PLLDB.LISTID$ + PLLDB.ITEMSEQ$ + PACK$("99999999") + \ !1.5CS
                              PLLDB.QTY.ONSHELF$  +  PLLDB.FILL.QTY$                       + \ !1.5CS
                              "YU00000000" + MID$(DATA.IN$, 3,  4) + MID$(DATA.IN$, 7,  4) + \ !1.5CS
                              MID$(DATA.IN$, 11, 4) + MID$(DATA.IN$, 15, 1)                + \ !1.5CS
                              "0000000000000"                                                  !1.5CS
                ENDIF ELSE BEGIN                                                               !1.5CS
                    ! Obtain current stock figure for use later - we need to know the amount   !1.5CS
                    ! of stock that has moved between counting & updating later.               !1.5CS
                    GOSUB GET.IMSTC.SALES.FIGURE                                               !1.5CS
                    PLLDB.SHELFMON.SALE.FIG$ = RIGHT$("0000" + CURRENT.STOCK$, 4)              !1.5CS
                    BOOTS.CODE$  = UNPACK$(IRF.BOOTS.CODE$)                                    !1.5CS
                    EAN.BARCODE$ = UNPACK$(IRF.BAR.CODE$)                                      !1.5CS
                    RC% = CALC.BOOTS.CODE.CHECK.DIGIT(BOOTS.CODE$)                             !1.5CS
                    IF RC% <> 0 THEN GOTO PROGRAM.EXIT                                         !1.5CS

                    PACKED.BOOTS.CODE$ = PACK$(RIGHT$("00000000" + BOOTS.CODE$ + F18.CHECK.DIGIT$, 8))!1.5CS
                    WS.STRING$= PLLDB.LISTID$ + PLLDB.ITEMSEQ$ + PACKED.BOOTS.CODE$              +  \ !1.5CS
                              PLLDB.QTY.ONSHELF$  +  PLLDB.FILL.QTY$  +  "YU0000"         +  \ !1.5CS
                              PLLDB.SHELFMON.SALE.FIG$ + "00000000000000000000000000"          !1.5CS
                                                                                            !1.5CS
                    IF EAN.BARCODE$ = ("0000000000000000" + BOOTS.CODE$) THEN BEGIN            !1.5CS
                        ! Boots Code - add stright to table. Can ONLY be one instance since    !1.5CS
                        ! PDT merges same codes together ie. NO lookup required.               !1.5CS
                        BTSCNT% = BTSCNT% + 1                                                  !1.5CS
                        PLLDB.TABLE$(BTSCNT%)= WS.STRING$                                      !1.5CS
                    ENDIF ELSE BEGIN                                                           !1.5CS
                        WS.SEQ.NO% = WS.SEQ.NO% - 1    ! Only increase for PLLDB Entries       !1.5CS
                        EANCNT% = EANCNT% + 1                                                  !1.5CS
                        EAN.TABLE$(EANCNT%) = PLLDB.LISTID$ + "999" + RIGHT$(WS.STRING$, 48)   !1.5CS
                    ENDIF                                                                      !1.5CS
                ENDIF                                                                          !1.5CS
            ENDIF ELSE BEGIN                                               ! 1.3
               RECEIVE.STATE$ = "*"                                        ! 1.3
            ENDIF                                                          ! 1.3

         ENDIF                                                                                 !1.7BG

      ENDIF ELSE BEGIN                                                  ! 1.3
         CURR.SESS.NUM% = CHKBF.SESS.NUM%                               ! 1.3
         IF CHKBF.OPEN.FLAG$ = "Y" THEN BEGIN
            CHKBF.ITEM.CODE$ = MID$(DATA.IN$, 3, 13)
            CHKBF.PRICE$ = MID$(DATA.IN$, 16, 6)
            CHKBF.FILLER$ = " "
            RC% = WRITE.CHKBF                                           ! HLC
            IF RC% = 1 THEN GOTO WRITE.ERROR                            ! HLC
            CHKBF.POINTER% = CHKBF.POINTER% + 1                         ! HLC
         ENDIF ELSE BEGIN
            RECEIVE.STATE$ = "*"                                        ! GDS
         ENDIF
      ENDIF                                                             ! 1.3

   RETURN

\******************************************************************************
\***
\***   RECEIVED.PCHECK.TRAILER:                             STATE : Q
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if already received pcheck header etc. then send trailer back to
\***      PDT, close buffer file and kick-off price check processing
\***
\***   RETURN
\***
\******************************************************************************


   RECEIVED.PCHECK.TRAILER:

      SB.MESSAGE$ = "PDT Support - PCHECK trailer received"
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 18) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"                                           ! GDS
         RETURN
      ENDIF

      IF OLD.PCHECK.RECORD% THEN BEGIN                                             !1.7BG

         IF CHKBF.OPEN.FLAG$ = "Y" AND GAPBF.OPEN.FLAG$ = "Y" THEN BEGIN   ! 1.4   !1.7BG
            PIPE.OUT$ = "L" + DATA.IN$                                             !1.7BG
            GOSUB SEND.TO.PSS38                                                    !1.7BG

            LDTAF.LINK.TYPE% = 2                                            ! KMJK !1.7BG
            IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                \ KMJK !1.7BG
               (LDTAF.LINK.TYPE% OR 80H)                                    ! KMJK !1.7BG
            LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                         ! KMJK !1.7BG
            LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                         ! KMJK !1.7BG
            TEMP.TIME$ = TIME$                                              ! LMJK !1.7BG
            LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +           \ LMJK !1.7BG
                                (VAL(MID$(TEMP.TIME$,3,2))*60)  +           \ LMJK !1.7BG
                                 VAL(RIGHT$(TEMP.TIME$,2)) ) -              \ LMJK !1.7BG
                          ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ LMJK !1.7BG
                            (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+ \ LMJK !1.7BG
                             VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )    ! LMJK !1.7BG
            LDTAF.DATA.VOLUME.1% = 0                                        ! KMJK !1.7BG
            LDTAF.DATA.VOLUME.1% = SIZE (CHKBF.FILE.NAME$)                  ! KMJK !1.7BG
            LDTAF.DATA.VOLUME.2% = 0                                        ! KMJK !1.7BG
            GOSUB LOG.TO.LDTAF.FILE                                         ! KMJK !1.7BG

            CURR.SESS.NUM% = GAPBF.SESS.NUM%                                ! 1.4  !1.7BG
            GAPBF.BOOTS.CODE$ = ""                                          ! 1.4  !1.7BG
            RC% = WRITE.GAPBF                                               ! 1.4  !1.7BG
            IF RC% = 1 THEN GOTO WRITE.ERROR                                ! 1.4  !1.7BG

            CLOSE IRF.SESS.NUM%                                                    !1.7BG
            IRF.OPEN.FLAG$="N"                                                     !1.7BG
            CLOSE PLLOL.SESS.NUM%                                                  !1.7BG
            PLLOL.OPEN.FLAG$ = "N"                                                 !1.7BG
            CLOSE PLLDB.SESS.NUM%                                                  !1.7BG
            PLLDB.OPEN.FLAG$ = "N"                                                 !1.7BG
            CLOSE CHKBF.SESS.NUM%                                                  !1.7BG
            CHKBF.OPEN.FLAG$ = "N"                                                 !1.7BG
            CLOSE GAPBF.SESS.NUM%                                           ! 1.3  !1.7BG
            GAPBF.OPEN.FLAG$ = "N"                                          ! 1.3  !1.7BG
            CLOSE PCHK.SESS.NUM%                                                   !1.7BG
            PCHK.OPEN.FLAG$ = "N"                                                  !1.7BG

            IF SIZE(GAPBF.FILE.NAME$) <> 0 AND                              \ 1.3  !1.7BG
            GAP.RECORD.EXISTS THEN BEGIN                                    ! 1.4  !1.7BG
               ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS47.286",               \ 1.3  !1.7BG
                               MONITORED.PORT$,                             \ 1.3  !1.7BG
                               "Gap Report Processing")                     ! 1.3  !1.7BG
            ENDIF                                                           ! 1.3  !1.7BG
            IF SIZE(CHKBF.FILE.NAME$) <> 0 THEN BEGIN                       ! 1.3  !1.7BG
               ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS43.286",               \      !1.7BG
                               MONITORED.PORT$,                             \      !1.7BG
                               "Price Check Processing")                           !1.7BG
            ENDIF                                                           ! 1.3  !1.7BG

            RE.CHAIN = TRUE                                                 ! HLC  !1.7BG
            RECEIVE.STATE$ = "?"                                            ! HLC  !1.7BG
         ENDIF ELSE BEGIN                                                          !1.7BG
            RECEIVE.STATE$ = "*"                                            ! GDS  !1.7BG
         ENDIF                                                                     !1.7BG

      ENDIF ELSE BEGIN                                                             !1.7BG

         !-----------------------------------------------------             !1.5CS
         ! Update PLLOL New List record with number of items &              !1.5CS
         ! Set list status to active.                                       !1.5CS
         !-----------------------------------------------------             !1.5CS
         PLLOL.RECORD.NUM% = WS.NEW.LISTID% + 1                             !1.5CS

         IF SMON.NEW.LIST.CREATED THEN BEGIN                                !1.5CS

             FOR Y%=1 TO EANCNT%                                                        !1.5CS

                 ITEM.FOUND.IN.TABLE$ = "N"                                                 !1.5CS

                 ! Merge EAN Table with PLLDB Table                                         !1.5CS
                 ! If match then merge counts else add to end of list                       !1.5CS
                 FOR X%=1 TO BTSCNT%                                                        !1.5CS
                     IF MID$(EAN.TABLE$(Y%),7,4) = MID$(PLLDB.TABLE$(X%),7,4) THEN BEGIN    !1.5CS
                         ! Amalgamate Shelf and Fill Quantities into existing record        !1.5CS
                         PLLDB.TABLE$(X%) =  LEFT$(PLLDB.TABLE$(X%),10)                 + \ !1.5CS
                           RIGHT$("0000" + STR$( VAL(MID$(PLLDB.TABLE$(X%), 11, 4))     + \ !1.5CS
                                           VAL(MID$(EAN.TABLE$(Y%),11,4))), 4)          + \ !1.5CS
                           RIGHT$("0000" + STR$( VAL(MID$(PLLDB.TABLE$(X%), 15, 4))     + \ !1.5CS
                                           VAL(MID$(EAN.TABLE$(Y%),15,4))), 4)          + \ !1.5CS
                           RIGHT$(PLLDB.TABLE$(X%),36)                                      !1.5CS

                         X% = BTSCNT% + 1    ! Exit Loop                                    !1.5CS
                         ITEM.FOUND.IN.TABLE$ = "Y"                                         !1.5CS
                     ENDIF                                                                  !1.5CS
                 NEXT X%                                                                    !1.5CS

                 IF ITEM.FOUND.IN.TABLE$ = "N" THEN BEGIN                                   !1.5CS

                     WS.SEQ.NO% = WS.SEQ.NO% + 1                                            !1.5CS
                     PLLDB.ITEMSEQ$ = RIGHT$("000" + STR$(WS.SEQ.NO%),3)                    !1.5CS
                     BTSCNT% = BTSCNT% + 1                                                  !1.5CS
                     PLLDB.TABLE$(BTSCNT%) = PLLDB.LISTID$ + PLLDB.ITEMSEQ$ + RIGHT$(EAN.TABLE$(Y%), 48)   !1.5CS
                 ENDIF                                                                      !1.5CS
             NEXT Y%                                                                        !1.5CS

             FOR X%=1 TO BTSCNT%                                            !1.5CS
                PLLDB.KEY$ = PLLDB.TABLE$(X%)                               !1.5CS
                RC% = WRITE.BRIEF.PLLDB                                     !1.5CS
                IF RC% = 1 THEN GOTO WRITE.ERROR                            !1.5CS
             NEXT X%                                                        !1.5CS

             RC% = READ.PLLOL                                               !1.5CS
             IF RC% = 1 THEN GOTO READ.ERROR                                !1.5CS

             PLLOL.ITEM.COUNT$  = RIGHT$("0000" + STR$(WS.SEQ.NO%),4)        !1.5CS
             PLLOL.ITEM.STATUS$ = "A"                                        !1.5CS

             RC% = WRITE.PLLOL                                               !1.5CS
             IF RC% = 1 THEN GOTO WRITE.ERROR                                !1.5CS
          ENDIF                                                              !1.5CS

          IF CHKBF.OPEN.FLAG$ = "Y" AND PLLOL.OPEN.FLAG$ = "Y" THEN BEGIN   ! 1.4  !1.5CS
            PIPE.OUT$ = "L" + DATA.IN$
            GOSUB SEND.TO.PSS38

            LDTAF.LINK.TYPE% = 2                                            ! KMJK
            IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                \ KMJK
               (LDTAF.LINK.TYPE% OR 80H)                                    ! KMJK
            LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                         ! KMJK
            LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                         ! KMJK
            TEMP.TIME$ = TIME$                                              ! LMJK
            LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +           \ LMJK
                                (VAL(MID$(TEMP.TIME$,3,2))*60)  +           \ LMJK
                                 VAL(RIGHT$(TEMP.TIME$,2)) ) -              \ LMJK
                          ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ LMJK
                            (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+ \ LMJK
                             VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )    ! LMJK
            LDTAF.DATA.VOLUME.1% = 0                                        ! KMJK
            LDTAF.DATA.VOLUME.1% = SIZE (CHKBF.FILE.NAME$)                  ! KMJK
            LDTAF.DATA.VOLUME.2% = 0                                        ! KMJK
            GOSUB LOG.TO.LDTAF.FILE                                         ! KMJK

            CLOSE IRF.SESS.NUM%                                             !1.5CS
            IRF.OPEN.FLAG$="N"                                              !1.5CS
            CLOSE CHKBF.SESS.NUM%
            CHKBF.OPEN.FLAG$ = "N"
            CLOSE PLLOL.SESS.NUM%                                           !1.5CS
            PLLOL.OPEN.FLAG$ = "N"                                          !1.5CS
            CLOSE PLLDB.SESS.NUM%                                           !1.5CS
            PLLDB.OPEN.FLAG$ = "N"                                          !1.5CS
            CLOSE PCHK.SESS.NUM%
            PCHK.OPEN.FLAG$ = "N"
            CLOSE GAPBF.SESS.NUM%                                                  !1.7BG
            GAPBF.OPEN.FLAG$ = "N"                                                 !1.7BG

            IF BTSCNT% > 0 THEN BEGIN                                       ! 1.4  !1.5CS
               ADX.RET.CODE% = ADXSTART("ADX_UPGM:PST47.286",               \ 1.3  !1.5CS
                               PLLOL.LISTID$,                               \ 1.5CS
                               "Gap Report Processing")                     ! 1.3
            ENDIF                                                           ! 1.3
            IF SIZE(CHKBF.FILE.NAME$) <> 0 THEN BEGIN                       ! 1.3
               ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSS43.286",               \
                               MONITORED.PORT$,                             \
                               "Price Check Processing")
            ENDIF                                                           ! 1.3

            RE.CHAIN = TRUE                                                 ! HLC
            RECEIVE.STATE$ = "?"                                            ! HLC
         ENDIF ELSE BEGIN
            RECEIVE.STATE$ = "*"                                            ! GDS
         ENDIF

      ENDIF                                                                        !1.7BG

   RETURN


\******************************************************************************
\***                                                                    1.5CS
\***   GET.IMSTC.SALES.FIGURE:
\***
\******************************************************************************

GET.IMSTC.SALES.FIGURE:

    CURRENT.STOCK$ = ""

    CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%
    IF IMSTC.OPEN.FLAG$ <> "Y" THEN BEGIN
        IF END #IMSTC.SESS.NUM% THEN OPEN.ERROR
        OPEN IMSTC.FILE.NAME$ KEYED RECL IMSTC.RECL% AS IMSTC.SESS.NUM% NOWRITE NODEL
    ENDIF

    !IMSTC.BAR.CODE$ = PACK$(RIGHT$(STRING$(22,"0") + LEFT$(BOOTS.CODE$,6),22))
    !IMSTC.BAR.CODE$ = IRF.BAR.CODE$                                    ! NDG
    IMSTC.BAR.CODE$ = RIGHT$(PACK$(STRING$(22,"0")) + \                 ! NDG
                          IRF.BOOTS.CODE$,11)                           ! NDG
    RC% EQ READ.IMSTC      
    CLOSE IMSTC.SESS.NUM%
    IMSTC.OPEN.FLAG$ = "N"

    IF RC% EQ 0 THEN BEGIN ! FOUND AN IMSTC RECORD
       IF (ASC(IMSTC.STATUS.FLAG$) AND 10000000b) = 0 THEN BEGIN
          ! NOT A STOCKED ITEM
          RETURN
       ENDIF
       ! IMSTC.STOCK.FIGURE% is stock fig - sales
       CURRENT.STOCK$ EQ STR$(IMSTC.NUMITEMS% / 100 )     ! 1.6
    ENDIF

RETURN

\******************************************************************************
\***
\***   LOG.TO.AUDIT.FILE
\***
\******************************************************************************

\  LOG.TO.AUDIT.FILE:                                                   ! JLC  !1.8NWB

\       IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN                        ! JLC  !1.8NWB
\          IF END #CSR.AUDIT.SESS.NUM% THEN CREATE.AUDIT.FILE           ! JLC  !1.8NWB
\          OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND           ! JLC  !1.8NWB
\          CSR.AUDIT.OPEN.FLAG$ = "Y"                                   ! JLC  !1.8NWB
\       ENDIF                                                           ! JLC  !1.8NWB

\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! JLC  !1.8NWB
\       IF LEFT$(APPL$,4) = "LOAD" THEN BEGIN                           ! JLC  !1.8NWB
\          CLOSE CSR.AUDIT.SESS.NUM%                                    ! JLC  !1.8NWB
\          CSR.AUDIT.OPEN.FLAG$ = "N"                                   ! JLC  !1.8NWB
\       ENDIF                                                           ! JLC  !1.8NWB

\  RETURN                                                               ! JLC  !1.8NWB

\  CREATE.AUDIT.FILE:                                                   ! JLC  !1.8NWB

\       CREATE CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM%                   ! JLC  !1.8NWB
\       CSR.AUDIT.OPEN.FLAG$ = "Y"                                      ! JLC  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;                          \ JLC  !1.8NWB
\       "                          LDT/PDT Tracking Log"                ! JLC  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! JLC  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"File Creation Date " +   \ JLC  !1.8NWB
\         LEFT$(DATE$,2) + "/" + MID$(DATE$,3,2) + "/" + RIGHT$(DATE$,2)! JLC  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"Currently executing " +  \ JLC  !1.8NWB
\         "PSS37 version 20, Created on 27th July 1995    "             ! 1.3  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! JLC  !1.8NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! JLC  !1.8NWB

\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! JLC  !1.8NWB

\RETURN                                                                 ! JLC  !1.8NWB

\******************************************************************************
\***
\***   LOG.TO.LDTAF.FILE
\***
\******************************************************************************

   LOG.TO.LDTAF.FILE:                                                  ! KMJK

      CURRENT.KEY$ = ""                                                ! KMJK
      CURR.SESS.NUM% = LDTAF.SESS.NUM%                                 ! KMJK
        IF LDTAF.OPEN.FLAG$ = "N" THEN BEGIN                           ! KMJK
           IF END #LDTAF.SESS.NUM% THEN CREATE.LDTAF.FILE              ! KMJK
           OPEN LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM% APPEND             ! KMJK
           LDTAF.OPEN.FLAG$ = "Y"                                      ! KMJK
        ENDIF                                                          ! KMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                  \ KMJK
        IF WRITE.LDTAF THEN GOTO WRITE.ERROR                           ! KMJK
      LDTAF.LINK.TYPE% = 0                                             ! KMJK
      LDTAF.OPEN.FLAG$ = "N"                                           ! KMJK
      CLOSE LDTAF.SESS.NUM%                                            ! KMJK

   RETURN                                                              ! KMJK

   CREATE.LDTAF.FILE:                                                  ! KMJK

      IF END #LDTAF.SESS.NUM% THEN CREATE.ERROR                        ! KMJK
        CREATE POSFILE LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM%             \ KMJK
        MIRRORED ATCLOSE                                               ! KMJK
        LDTAF.OPEN.FLAG$ = "Y"                                         ! KMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                  \ KMJK
        IF WRITE.LDTAF THEN GOTO WRITE.ERROR                           ! KMJK
      LDTAF.LINK.TYPE% = 0                                             ! KMJK
      LDTAF.OPEN.FLAG$ = "N"                                           ! KMJK
      CLOSE LDTAF.SESS.NUM%                                            ! KMJK

   RETURN                                                              ! KMJK

\******************************************************************************
\******************************************************************************
\***
\***   P D T    S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   DETERMINE.DATA.TYPE:
\***
\***      set RECEIVE.STATE$ depending on record received (DATA.IN$),
\***      decode is follows ;
\***      1st 2 chars = "XH"                              - state "J"
\***      1st 2 chars = "XM"                              - state "K"
\***      1st 2 chars = "XU"                              - state "K"
\***      1st 2 chars = "XO"                              - state "K"
\***      1st 2 chars = "XT"                              - state "L"
\***      1st 2 chars = "XZ"                              - state "N"
\***      any record that does not satisfy any of the above conditions will
\***      be given a state of "*" (invalid record)
\***
\***   RETURN
\***
\******************************************************************************

\  DETERMINE.DATA.TYPE:                                                        !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XH" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "J"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XM" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "K"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XU" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "K"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XO" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "K"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XT" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "L"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LEFT$(CSRBF.DATA$,2) = "XZ" THEN BEGIN                                !1.8NWB
\        RECEIVE.STATE$ = "N"                                                  !1.8NWB
\      RETURN                                                                  !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     RECEIVE.STATE$ = "*"                                              ! GDS  !1.8NWB

\  RETURN                                                                      !1.8NWB

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
\***   RELEASE.PDT:
\***
\***      send a release PDT command to PSS38
\***
\***   RETURN
\***
\******************************************************************************

\  RELEASE.PDT:                                                                !1.8NWB

\     PIPE.OUT$ = "HN"                                                         !1.8NWB
\     GOSUB SEND.TO.PSS38                                                      !1.8NWB
\     HOLD.FLAG$ = "N"                                                         !1.8NWB

\  RETURN                                                                      !1.8NWB

\******************************************************************************
\***
\***   TRANSMIT.CSR.FILE.TO.PDT:
\***
\***     send instruction to PSS38 to request transmission of CSRWF to PDT
\***
\***   RETURN
\***
\******************************************************************************

\  TRANSMIT.CSR.FILE.TO.PDT:                                                   !1.8NWB

\     PIPE.OUT$ = "C"                                                          !1.8NWB
\     GOSUB SEND.TO.PSS38                                                      !1.8NWB

\     SB.MESSAGE$ = "PDT Support - Waiting for CSR E.O.T"                      !1.8NWB
\     GOSUB SB.BG.MESSAGE                                                      !1.8NWB

\  RETURN                                                                      !1.8NWB

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

      RC% = SESS.NUM.UTILITY(SB.ACTION$,                        \
                       SB.INTEGER%,                        \
                       SB.STRING$ )

      IF RC% <> 0 THEN BEGIN
         SB.EVENT.NO% = 48
       SB.UNIQUE$ = FN.Z.PACK(STR$(F20.INTEGER.FILE.NO%), 10)
       SB.MESSAGE$ = "SESSION NUMBER ALLOCATION ROUTINE FAILED"
       GOSUB SB.LOG.AN.EVENT
       GOTO PROGRAM.EXIT
      ENDIF

      IF SB.ACTION$ = "O" THEN                                    \
         SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
      IF SB.ACTION$ = "R" OR SB.ACTION$ = "D" THEN BEGIN               ! NNS
         SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
         SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +                  \ NNS
                    CHR$(SHIFT(SB.FILE.REP.NUM%,0))                    ! NNS
      ENDIF                                                            ! NNS


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
      LAST.MESSAGE$ = SB.MESSAGE$                              ! ESW

      SB.MESSAGE$ = MONITORED.PORT$ + ": " + SB.MESSAGE$
      SB.MESSAGE$ = LEFT$(SB.MESSAGE$ + STRING$(46, " "), 46)            ! ESW
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

      RC% = APPLICATION.LOG(MESSAGE.NO%,                        \
                      PORT.STRING$,                                  \
                      UNIQUE.2$,                              \
                      PORT.EVENT% )

      RC% = APPLICATION.LOG(MESSAGE.NO%,                        \
                      SB.UNIQUE$,                              \
                      UNIQUE.2$,                              \
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
      IF RC% <> 0 THEN                                          \
         GOTO PROGRAM.EXIT

      RC% = CONV.TO.STRING(0,                                    \
                     ERRN )
      IF RC% <> 0 THEN                                          \
         GOTO PROGRAM.EXIT
      SB.ERRS$ = F17.RETURNED.STRING$

      SB.ERRL$ = FN.Z.PACK(STR$(ERRL), 6)
      SB.ACTION$ = "R" : SB.INTEGER% = ERRF% : SB.STRING$ = ""
      GOSUB SB.FILE.UTILS

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
      SB.EVENT.NO% = 6
      SB.UNIQUE$ = "O" + SB.ERRF$ + PACK$(STRING$(15,"0"))             ! NNS
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
      SB.EVENT.NO% = 6
      IF CURRENT.KEY$ <> "SECTOR" THEN                              \
         SB.UNIQUE$ = "R" +                                    \
         SB.ERRF$ +                                                    \ NNS
       FN.Z.PACK(CURRENT.KEY$,8)                              \
      ELSE                                                \
         SB.UNIQUE$ = "R" +                                    \
         SB.ERRF$ +                                                    \ NNS
       CURRENT.KEY$
! do not log an error if the read is of the CITEM or the IDF
\     IF SB.FILE.REP.NUM% = IDF.REPORT.NUM% OR                         \ KMJK  !1.8NWB
\        SB.FILE.REP.NUM% = CITEM.REPORT.NUM% THEN RETURN            ! KMJK    !1.8NWB
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
      SB.EVENT.NO% = 6
      IF CURRENT.KEY$ <> "SECTOR" THEN BEGIN
         SB.UNIQUE$ = "W" + SB.ERRF$ +                                 \ NNS
       FN.Z.PACK(CURRENT.KEY$,8)
      ENDIF ELSE BEGIN
         SB.UNIQUE$ = "W" + SB.ERRF$ + CURRENT.KEY$                    ! NNS
      ENDIF
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.DELETE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to delete a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***      CURRENT.KEY$    = value of key used to try to delete to file.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.DELETE.ERROR:

      SB.ACTION$ = "D"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      SB.EVENT.NO% = 6
      SB.UNIQUE$ = "D" + SB.ERRF$ + CURRENT.KEY$                       ! NNS
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
      SB.EVENT.NO% = 6
      SB.UNIQUE$ = "C" + SB.ERRF$                                      ! NNS
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
\     IF CURR.SESS.NUM% = IDSOF.SESS.NUM% THEN BEGIN                  ! ILC    !1.8NWB
\        GOTO RETURN.FROM.IDSOF.OPEN.ERROR                        ! ILC        !1.8NWB
\     ENDIF                                                ! ILC               !1.8NWB
      RECEIVE.STATE$ = "*"                                              ! GDS

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

\     IF LOCATION$ = "CALCTS"                                    \             !1.8NWB
\     AND CURR.SESS.NUM% = FPF.SESS.NUM% THEN BEGIN                            !1.8NWB
\        GOTO CALC.TS.CONTINUE                                                 !1.8NWB
\     ENDIF                                                                    !1.8NWB


\     IF LOCATION$ = "FINDITEM" THEN BEGIN                        ! BPAB       !1.8NWB
\        GOTO FIND.ITEM.ON.FILES.CONTINUE                                      !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LOCATION$ = "FPFPROC"                                    \            !1.8NWB
\     AND (CURR.SESS.NUM% = CITEM.SESS.NUM%                        \           !1.8NWB
\     OR CURR.SESS.NUM% = CIMF.SESS.NUM%) THEN BEGIN                           !1.8NWB
\        GOTO FIND.PARENT.CONTINUE                                             !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LOCATION$ = "FAMFIGS" THEN BEGIN                                      !1.8NWB
\        IF CURR.SESS.NUM% = FPF.SESS.NUM% THEN BEGIN                          !1.8NWB
\           GOTO CALC.FAMILY.FIGURES.CONTINUE                                  !1.8NWB
\        ENDIF ELSE BEGIN                                                      !1.8NWB
\           IF CURR.SESS.NUM% = CITEM.SESS.NUM%                        \       !1.8NWB
\           OR CURR.SESS.NUM% = CIMF.SESS.NUM% THEN BEGIN                      !1.8NWB
\              GOTO CALC.FAMILY.FIGURES.CONTINUE2                              !1.8NWB
\         ENDIF                                                                !1.8NWB
\      ENDIF                                                                   !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LOCATION$ = "UNPROC" AND CSRWF.UNPROCESS.FLAG$ = "Y"            \     !1.8NWB
\     AND ONORD.SEQ.NO$ <> "000"                              \                !1.8NWB
\     AND CURR.SESS.NUM% = ONORD.SESS.NUM% THEN BEGIN                          !1.8NWB
\        GOTO UNPROCESS.LIST.CONTINUE                                          !1.8NWB
\     ENDIF                                                                    !1.8NWB

\     IF LOCATION$ = "UNPROC" AND CSRWF.UNPROCESS.FLAG$ = "Y"            \ BPAB!1.8NWB
\     AND ONORD.SEQ.NO$ <> "000"                              \ BPAB           !1.8NWB
\     AND CURR.SESS.NUM% = CITEM.SESS.NUM% THEN BEGIN                  ! BPAB  !1.8NWB
\        GOTO UNDO.CSRITEM.CONTINUE                                ! BPAB      !1.8NWB
\     ENDIF                                                ! BPAB              !1.8NWB

      RECEIVE.STATE$ = "*"                                    ! GDS

      SB.EVENT.NO% = 0
\     SB.UNIQUE$ = LEFT$(LOCATION$ + STRING$(10, " "), 10)                     !1.8NWB
      SB.UNIQUE$ = LEFT$(            STRING$(10, " "), 10)                     !1.8NWB
      SB.MESSAGE$ = "MOD 1 READ ERROR COULD NOT BE HANDLED"
      GOSUB SB.LOG.AN.EVENT

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
      RECEIVE.STATE$ = "*"                                              ! GDS

   GOTO MODULE.EXIT

\******************************************************************************
\***
\***   DELETE.ERROR:
\***
\***      log an event 6 (delete error)
\***      set RECEIVE.STATE$ to "*"
\***
\***      if the source of the error is known then return to one of the
\***      specified return points so the program does not end
\***
\***   GOTO MODULE.EXIT (no return is possible)
\***
\******************************************************************************

   DELETE.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.DELETE.ERROR
\     IF CURR.SESS.NUM% = IDSOF.SESS.NUM% THEN BEGIN                  ! ILC    !1.8NWB
\        IF MANUAL.COUNT$ = "Y" THEN BEGIN                        ! ILC        !1.8NWB
\         GOTO RETURN.FROM.IDSOF.DELETE.ERROR.2                  ! ILC         !1.8NWB
\         MANUAL.COUNT$ = "N"                                    ! ILC         !1.8NWB
\      ENDIF ELSE BEGIN                                    ! ILC               !1.8NWB
\         GOTO RETURN.FROM.IDSOF.DELETE.ERROR                        ! ILC     !1.8NWB
\      ENDIF                                                ! ILC              !1.8NWB
\     ENDIF                                                ! ILC               !1.8NWB
      RECEIVE.STATE$ = "*"                                              ! GDS

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
      RECEIVE.STATE$ = "*"                                              ! GDS

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
\***   If the CSR workfile cannot be deleted then resume retry for 5 seconds.
\***
\***      if the CSR workfile does not exist then resume
\***
\***      set-up common error reporting information
\***
\***      if the error is an access conflict on CSR, EPSOM or PCHK files then
\***          resume at FILE.CONFLICT
\***
\***      if unable to open PSS38 communication pipes then
\***         wait 3 seconds then retry
\***      endif
\***
\***      if an access conflict occurs on a session then retry
\***
\***      if error is invalid characters in VAL() then
\***         store RECEIVE.STATE$
\***         set RECEIVE.STATE$ to "*"
\***         use stored receive state variable to decide where to resume
\***      endif
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

   IF (ERRN = 00000052h) AND (LDTAF.LINK.TYPE%<>0) THEN RESUME             ! MMJK

   ERROR.COUNT% = ERROR.COUNT% + 1
   IF ERROR.COUNT% > 1 THEN RESUME PROGRAM.EXIT

   GOSUB SB.FORMAT.ERROR.DATA

\  IF ERRN = 80204001H AND SB.FILE.REP.NUM% = CSRWF.REPORT.NUM% THEN BEGIN ! NN!1.8NWB
\     ERROR.COUNT% = ERROR.COUNT% - 1                                  ! NNS   !1.8NWB
\     RECEIVE.STATE$ = "*"                                             ! NNS   !1.8NWB
\     RESUME MODULE.EXIT                                               ! NNS   !1.8NWB
\  ENDIF                                                               ! NNS   !1.8NWB


   IF (ERRN AND 0000FFFFh) = 400Ch THEN BEGIN
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME RETRY
   ENDIF

   IF ERR = "OM" THEN BEGIN
      CALL ADXERROR(0, 74, 0, 3, 1, SB.UNIQUE$ )
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME PROGRAM.EXIT
   ENDIF

   SB.EVENT.NO% = 101
   SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                               \ KMJK
                PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))            ! KMJK
   GOSUB SB.LOG.AN.EVENT

   ERROR.COUNT% = ERROR.COUNT% - 1
   RECEIVE.STATE$ = "*"                                                 ! GDS
   RESUME MODULE.EXIT

END SUB
