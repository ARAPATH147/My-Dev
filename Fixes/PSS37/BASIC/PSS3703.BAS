\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSS3703.bas  $
\***
\***   $Revision:   1.6  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PSS3703.bav  $
\***   
\***      Rev 1.6   17 Jul 2003 10:09:04   dev38ps
\***   Commented out IRF which is now in the global definitions file
\***   
\***      Rev 1.5   Apr 14 2000 17:29:12   dev26ps
\***   Removed references to INDICAT2 pricing
\***   method and relinked with new PSBF19.
\***   Stuart William McConnachie.
\***   
\***      Rev 1.4   08 Apr 1998 11:08:04   DEV45PS
\***   Changes to cope with dates of '000000' and '999999' and to include revised CMPDATE
\***   
\***      Rev 1.3   05 Feb 1998 11:58:14   DEV45PS
\***    
\***   
\***      Rev 1.2   14 Feb 1995 11:44:20   NIK
\***   Fixes to Boots Problems 1248, 1245, 1246, 1318, 1105
\***   
\***      Rev 1.1   07 Oct 1994 15:51:38   DEVSPPS
\***   Fixed PDT Support hang
\***   
\******************************************************************************
\******************************************************************************

REM
\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3703                                          ***
\***                                                                        ***
\***           AUTHOR   :  Les Cook                                         ***
\***                                                                        ***
\***           DATE     :  16th March 1993                                  ***
\***                                                                        ***
\***                                                                        ***
\***           Current version letter : D                                   ***
\***                                                                        ***
\***    Date of last change;  2nd February 1994            Nik Sen          ***
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
\***                                                                   ***       
\***   This module was created to contain the bulk of the processing for    ***       
\***   the EPSOM PDT application. This has been moved from module 0 to      ***
\***   allow for future applications.                                        ***
\***                                                                        ***
\***   Version B.       Michael J. Kelsall           12th October 1993      ***
\***   Changes to include processing of LDTAF records.                      ***
\***                                                                        ***
\***   Version C.       Michael J. Kelsall           23rd March 1994        ***
\***   Change to LDTAF duration to record in seconds as opposed to minutes  ***
\***                                                                        ***
\***   REVISION 1.1     ROBERT COWEY / STEVE WRIGHT      6TH OCTOBER 1994 
\***   Removed version letters from included code (not commented).
\***
\***   Version D (1.2)         Nik Sen                 26th January 1995
\***   Amendments to Error.Detected to handle STKMQ access conflict errors.
\***   Also now logs event 101 instead of event 1.                      
\***   Error trap introduced to detect 'File cannot be accessed due to current
\***   usage' error against PDTWF when a create is attempted. The create fails
\***   as PSS38 is holding the file open. Program now returns to data state 'A'. 
\***   Amendment to RECEIVED.EPSOM.LIST.COUNT to trap invalid time from PDT.
\***   Changed SB.FILE.REP.NUM% to 2 byte integer.
\***
\***   Version E (1.3)         Nik Sen                 19th November 1997
\***   Y2000 changes. Comparisons in PROCESS.AN.EPSOM.LIST. Sort in 
\***   SORT.EPSOM.TRANSMISSION.LIST.
\***
\***   Version F (1.4)         Nik Sen                 1st April 1998
\***   Changed so Y2000 code copes with dates of '000000' or '999999'. 
\***   (No joke - despite todays date). 
\***
\***   Version G        Stuart William McConnachie      3rd March 2000
\***   Updated IRF fields for meal deal.
\***
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   I N C L U D E S   A N D   V A R I A B L E S                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

   %INCLUDE PSBF06G.J86
   %INCLUDE ASYNCNUB.J86
   %INCLUDE EPSOMDEC.J86
   %INCLUDE PDTWFDEC.J86
   %INCLUDE PIPEONUB.J86

   %INCLUDE PSS37G.J86                                                 \ CMJK

   %INCLUDE PSBF06E.J86
   %INCLUDE PDTWFEXT.J86

   %INCLUDE CMPDATE.J86                                                ! ENS


   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL                     
      INTEGER*2 ADXSTART                                          
      STRING    NAME$, PARM$, MESS$                                   
   END FUNCTION                                                        
   
\******************************************************************************
\***
\***   Function : FN.TRANSLATE.TEXT( TEXT$ )
\***
\***   Purpose  : Translate passed text from 4680 character set to PDT
\***              character set
\***
\***   Output   : = string
\***
\******************************************************************************

   FUNCTION FN.TRANSLATE.TEXT( TEXT$ )                                  

      STRING                                                            \
        FN.TRANSLATE.TEXT,                                              \
        TEXT$                                                           

      FN.TRANSLATE.TEXT = TRANSLATE$(TEXT$, TRANS.FROM$, TRANS.TO$)     

   END FUNCTION                                                         
       

   SUB PSS3703 PUBLIC
          
REAL GAP

INTEGER*1                                                        \
          BAR.CODE.PTR%,                                                 \ 
       CURR.SESS.NUM%,                                                 \
       EVENT.NUMBER%,                                                 \
       RES.POS%,                                                 \
       SB.EVENT.NO%,                                                 \
       SB.FILE.SESS.NUM%,                                          \
       SB.INTEGER%,                                                    \
       FINISHED,                                                       \
       SWAPPED                                                         !
       
INTEGER*2                                                        \
       ADX.FUNCTION%,                                                 \ 
       ADX.PARM.1%,                                                 \ 
        BAR.CODES%,                                                     \
        BC%,                                                            \
        CD%,                                                            \ 
        DATA.LENGTH%,                                                   \
        FILE.HEADER.LISTS%,                                             \
        FILE.TRAILER.LISTS%,                                            \
        INACTIVITY.SHUTDOWN%,                                           \ 
        INDX%,                                                          \ 
        ITEM%,                                                          \
        ITEM.COUNT%,                                                    \
        ITEM.TRAILER.COUNT%,                                            \
        LAST.REC%,                                                      \
        LIST%,                                                          \
        LIST.COUNT%,                                                    \
        LIST.EXTRACTION%,                                               \
        LIST.TRAILER.ITEMS%,                                            \
        LIST.TRANSMIT.PTR%,                                             \
        LOG.ON.DISABLE%,                                          \ 
       MESSAGE.NO%,                                                 \
        MESSAGE.NUMBER%,                                          \ 
       NUMBER.OF.ITEMS%,                                               \
        NUMBER.OF.LISTS%,                                               \
        POSITION%,                                                      \
        REP%,                                                           \ 
       RETURN.CODE%,                                                 \ 
        REQ.LIST.PTR%,                                                  \
       SB.FILE.REP.NUM%,                                               \ DNS
        SECTOR.COUNT%,                                                  \
        SORT.PTR%,                                                      \
        START.TIME%,                                                    \ 
        TEMP.EVENT%,                                                    \
       TOTAL.WAIT.TIME%                                                ! ASMG
       
       
       
INTEGER*4                                                        \
          ADX.RET.CODE%,                                                  \
          A%,                                                        \
          B%,                                                        \
        CSRWF.EXISTS%,                                                  \
        I%,                                                             \
        LISTS.CANCELLED%,                                               \
          MAX%,                                                         \
        NOW%,                                                           \
        RECORDS.WRITTEN%,                                               \
        TIMEOUT.VALUE%       
       
REAL                                                               \
        TIME.DIFFERENCE       

STRING                                                               \
        ADX.PARM.2$,                                                 \
        APPL$,                                                          \
        PDT.ACTION$,                                                    \
        BC$,                                                            \
        CITEM.RECORD$,                                                  \
        CITEM.SECTOR.ALTERED$,                                          \
        CONTROL$,                                                       \
       COUNT.DATE$,                                                    \
        CURR.BKRM.COUNT$,                                               \
        CURR.BOOTS.CODE$,                                               \
        CURR.COUNT.DATE$,                                               \
        CURR.COUNT.TIME$,                                               \
        CURR.DETAIL.USER$,                                              \
        CURR.HEADER.USER$,                                              \
        CURR.LIST$,                                                     \
        CURR.PRICE$,                                                    \
        CURR.SHOP.COUNT$,                                               \
        CURR.TIME$,                                                     \
        DATA$,                                                          \
       END.OF.CITEM$,                                                  \
        ENQ$,                                                           \
        FILLER$,                                                        \
\       IRF.PRICE.TYPE$,                                                \ GSWM
        ITEM.ON.IDF.FLAG$,                                              \
        KEY.VALUE$,                                                     \
        LAST.TIME$,                                                     \
        LIST.BC$,                                                       \
        LIST.NAME$,                                                     \
        LIST.NUMBER$,                                                   \
        LIST.STATUS$,                                                   \
        LIST.TYPE$,                                                     \
        MORE.FLAG$,                                                     \
        NAK.LINE.1$,                                                    \
        NAK.LINE.2$,                                                    \
        NAK.LINE.3$,                                                    \
        PDTWF.HEADER.RECORD$,                                           \
        PILST.EOF.FLAG$,                                                \
        PILST.RECORD$,                                                  \
        PRICE$,                                                         \
        RECOUNT.DATE$,                                                  \
        REQ.BC$,                                                        \
        REQ.FLAG$,                                                      \
        REQ.LIST$,                                                      \
        SAVED.PIPE.OUT$,                                                \
       SB.ACTION$,                                                    \ 
       SB.ERRF$,                                                      \ 
       SB.ERRL$,                                                      \ 
       SB.ERRS$,                                                      \ 
       SB.MESSAGE$,                                                   \ 
       SB.UNIQUE$,                                                    \ 
       SB.STRING$,                                                    \
        SECTOR$,                                                        \
        SELECT.ALL.FLAG$,                                               \
        SORT.DATE$,                                                     \
        SORT.ORDER$,                                                    \
       TEMP$,                                                    \ 
       CURRENT.KEY$,                                             \ 
        TEMP.LIST$,                                                     \
        TEMP.PDTWF.RECORD$,                                             \ 
        TEMP.STATE$,                                                    \
        TEST.LIST$,                                                     \
        UNIQUE.2$,                                                      \
        USER.DATA$,                                                 \
       VAR.STRING.1$,                                                 \ 
       VAR.STRING.2$                                                 ! 
   
   
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

      IF MATCH(RECEIVE.STATE$,"CDEFGHI",1) > 0 THEN BEGIN ! ELC
         ON (ASC(RECEIVE.STATE$) - ASC("B")) GOSUB                       \
               RECEIVED.EPSOM.FILE.HEADER,                            \
               RECEIVED.EPSOM.LIST.HEADER,                            \
               RECEIVED.EPSOM.LIST.COUNT,                            \
               RECEIVED.EPSOM.LIST.TRAILER,                            \
              RECEIVED.EPSOM.FILE.TRAILER,                            \
               RECEIVED.EPSOM.LIST.REQUEST,                            \
               RECEIVED.EPSOM.EOT
        GOTO MODULE.EXIT       
      ENDIF
              
MODULE.EXIT:

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
\***   RECEIVED.EPSOM.FILE.HEADER:                              STATE : C
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if received terminal number or application number is invalid then
\***      set RECEIVE.STATE$ to "*" and RETURN
\***
\***      set PDT action to "COUNTS"
\***      set FILE.HEADER.LISTS% to number of lists in file
\***      reset LIST.COUNT% counter to 0
\***
\***      open STKMQ file if not already open
\***
\***   EPSOM.FILE.HEADER.CONTINUE:
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.FILE.HEADER:                                          

      SB.MESSAGE$ = "PDT Support - EPSOM File header received"          
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 2) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF
      
      CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +              \
                        "EPSOM list count received at " +              \
                     LEFT$(TIME$,2) + ":" +                            \
                     MID$(TIME$,3,2) + ":" +                            \
                     RIGHT$(TIME$,2)
      GOSUB LOG.TO.AUDIT.FILE

      IF MID$(DATA.IN$, 7, 6) <> CURR.TERMINAL$                         \
      OR MID$(DATA.IN$, 3, 2) <> APPLICATION.NO$ THEN BEGIN             
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      PDT.ACTION$ = "COUNTS"
      FILE.HEADER.LISTS% = VAL(MID$(DATA.IN$, 5, 2))
      LIST.COUNT% = 0

! 7 LINES Deleted from here                                            ! ASMG

   EPSOM.FILE.HEADER.CONTINUE:                                         

   RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.LIST.HEADER:                              STATE : D
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      set CURR.LIST$ to received list number
\***      set HEADER.USER$ to received 'user data'
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.LIST.HEADER:                       
   
      SB.MESSAGE$ = "PDT Support - EPSOM List header received"         
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 3) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      CURR.LIST$ = MID$(DATA.IN$, 3, 4)
      CURR.HEADER.USER$ = MID$(DATA.IN$, 7, 5)
      LIST.BC$ = MID$(CURR.HEADER.USER$, 3, 1)
      ITEM.COUNT% = 0

      IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                         ! ASMG
         IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                    ! ASMG
         CURR.SESS.NUM% = STKMQ.SESS.NUM%                           ! ASMG
         OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512 READONLY\ ASMG
         NODEL APPEND                                               ! ASMG
         STKMQ.OPEN.FLAG$ = "Y"                                     ! ASMG
        TOTAL.WAIT.TIME% = 0                                       ! ASMG
      ENDIF                                                         ! ASMG

   RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.LIST.COUNT:                               STATE : E
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      set CURR.BOOTS.CODE$ to received boots code
\***      set CURR.SHOP.COUNT$ to received shop count
\***      set CURR.BKRM.COUNT$ to received back-room count
\***      set CURR.COUNT.DATE$ to received date of count
\***      set CURR.COUNT.TIME$ to received time of count
\***      set CURR.PRICE$ to received price
\***      set DETAIL.USER$ to received 'user data' (unused at present)
\***
\***      if any of the above values are invalid then set RECEIVE.STATE$ to
\***      "*" and return
\***
\***      write a transaction type 13 to the STKMQ file if current list is
\***      not a training list (i.e type "Z")
\***
\***      increment ITEM.COUNT% by 1
\***
\***   EPSOM.LIST.ITEM.CONTINUE:
\***   RETURN
\***
\******************************************************************************

RECEIVED.EPSOM.LIST.COUNT:                          
   
      IF FN.VALIDATE.DATA(DATA.IN$, 4) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      CURR.BOOTS.CODE$ = FN.Z.PACK(STR$(VAL(MID$(DATA.IN$, 3, 7))), 8)
      CURR.SHOP.COUNT$ = MID$(DATA.IN$, 10, 4)
      CURR.BKRM.COUNT$ = MID$(DATA.IN$, 14, 4)
      CURR.COUNT.DATE$ = MID$(DATA.IN$, 18, 6)
      CURR.COUNT.TIME$ = MID$(DATA.IN$, 24, 4)
      CURR.PRICE$ = MID$(DATA.IN$, 28, 6)
      CURR.DETAIL.USER$ = MID$(DATA.IN$, 34, 9)
      IF CURR.COUNT.DATE$ = "202020" THEN CURR.COUNT.DATE$ = "000000"
      IF CURR.COUNT.TIME$ = "2020" THEN CURR.COUNT.TIME$ = "0000"

      IF VAL(CURR.COUNT.DATE$) < 0 OR VAL(CURR.COUNT.TIME$) < 0        \ DNS
      OR VAL(CURR.PRICE$) < 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      IF LIST.BC$ <> "Z" THEN BEGIN                                    
         STKMQ.TRANS.TYPE$ = PACK$("13")
         STKMQ.DATE$ = PACK$(DATE$)
         STKMQ.TIME$ = PACK$(TIME$)
         STKMQ.LIST.NUMBER$ = FN.Z.PACK(CURR.LIST$, 4)
         STKMQ.ITEM.NUMBER$ = FN.Z.PACK(MID$(CURR.DETAIL.USER$, 1, 2), 2)
         STKMQ.ITEM.STATUS$ = FN.Z.PACK(MID$(CURR.DETAIL.USER$, 3, 1), 1)
         STKMQ.BOOTS.CODE$ = PACK$(CURR.BOOTS.CODE$)
         STKMQ.COUNT.DATE$ = PACK$(FN.Z.PACK(CURR.COUNT.DATE$,6))
         STKMQ.COUNT.TIME$ = PACK$(FN.Z.PACK(CURR.COUNT.TIME$,4))
         STKMQ.PRICE$ = PACK$(FN.Z.PACK(CURR.PRICE$,10))
         STKMQ.SHOP.COUNT$ = CURR.SHOP.COUNT$
         STKMQ.BKRM.COUNT$ = CURR.BKRM.COUNT$
         STKMQ.RECORD$ = STKMQ.RECORD.DELIMITER$ +                      \
                         STKMQ.TRANS.TYPE$ +                            \
                         STKMQ.FIELD.DELIMITER$ +                       \
                         STKMQ.DATE$ +                                  \
                         STKMQ.TIME$ +                                  \
                         STKMQ.LIST.NUMBER$ +                           \
                         STKMQ.ITEM.NUMBER$ +                           \
                         STKMQ.ITEM.STATUS$ +                           \
                         STKMQ.BOOTS.CODE$ +                            \
                         STKMQ.COUNT.DATE$ +                            \
                         STKMQ.COUNT.TIME$ +                            \
                         STKMQ.PRICE$ +                                 \
                         STKMQ.FIELD.DELIMITER$ +                       \
                         STKMQ.SHOP.COUNT$ +                            \
                         STKMQ.FIELD.DELIMITER$ +                       \
                         STKMQ.BKRM.COUNT$ +                            \
                         STKMQ.RECORD.DELIMITER$ +                      \
                         STKMQ.ENDREC.MARKER$
         CURR.SESS.NUM% = STKMQ.SESS.NUM%
         CURRENT.KEY$ = ""
         RC% = WRITE.STKMQ                                          
        IF RC% = 1 THEN  GOTO WRITE.ERROR                            
      ENDIF                                                             

      ITEM.COUNT% = ITEM.COUNT% + 1

EPSOM.LIST.ITEM.CONTINUE:                                           

RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.LIST.TRAILER:                             STATE : F
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      set TRAILER.LIST$ to list number on list trailer record
\***      set LIST.TRAILER.ITEMS% to number of lists value on trailer record
\***
\***      if TRAILER.LIST$ does not equal CURR.LIST$ then set RECEIEVE.STATE$
\***      to "*" and return
\***
\***      if LIST.TRAILER.ITEMS% does not equal ITEM.COUNT% then
\***         log an event 5 (count discrepancy)
\***      endif
\***
\***      write a transaction type 14 to the STKMQ file if current list is
\***      a training list (i.e type "Z")
\***
\***      increment LIST.COUNT% by 1
\***
\***   EPSOM.LIST.TRAILER.CONTINUE:
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.LIST.TRAILER:                         
   
      SB.MESSAGE$ = "PDT Support - EPSOM List trailer received"        
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 5) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      TRAILER.LIST$ = MID$(DATA.IN$, 3, 4)
      LIST.TRAILER.ITEMS% = VAL(MID$(DATA.IN$, 7, 3))

      IF TRAILER.LIST$ <> CURR.LIST$ THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF
      IF LIST.TRAILER.ITEMS% <> ITEM.COUNT% THEN BEGIN
         SB.EVENT.NO% = 5
         SB.UNIQUE$ = "I" + CHR$(ASYNC.REPORT.NUM%) +                   \
                      PACK$(FN.Z.PACK(STR$(ITEM.COUNT%), 4)) +          \
                      PACK$(FN.Z.PACK(STR$(LIST.TRAILER.ITEMS%), 4)) +  \
                      PK4$
         GOSUB SB.LOG.AN.EVENT
      ENDIF

      IF LIST.BC$ <> "Z" THEN BEGIN                                    
         STKMQ.TRANS.TYPE$ = PACK$("14")
         STKMQ.DATE$ = PACK$(DATE$)
         STKMQ.TIME$ = PACK$(TIME$)
         STKMQ.LIST.NUMBER$ = CURR.LIST$
         STKMQ.LIST.STATUS$ = MID$(CURR.HEADER.USER$, 1, 1)
         STKMQ.RECORD$ = STKMQ.RECORD.DELIMITER$ +                      \
                         STKMQ.TRANS.TYPE$ +                            \
                         STKMQ.FIELD.DELIMITER$ +                       \
                         STKMQ.DATE$ +                                  \
                         STKMQ.TIME$ +                                  \
                         STKMQ.LIST.NUMBER$ +                           \
                         STKMQ.LIST.STATUS$ +                           \
                         STKMQ.RECORD.DELIMITER$ +                      \
                         STKMQ.ENDREC.MARKER$
         CURR.SESS.NUM% = STKMQ.SESS.NUM%
         CURRENT.KEY$ = ""
         RC% = WRITE.STKMQ                                          
        IF RC% = 1 THEN GOTO WRITE.ERROR                            
      ENDIF                                                             

      LIST.COUNT% = LIST.COUNT% + 1

   EPSOM.LIST.TRAILER.CONTINUE:                                        

      IF STKMQ.OPEN.FLAG$ = "Y" THEN BEGIN                           ! KSMG
        CLOSE STKMQ.SESS.NUM%                                        ! KSMG
        STKMQ.OPEN.FLAG$ = "N"                                       ! KSMG
      ENDIF                                                          ! KSMG

   RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.FILE.TRAILER:                             STATE : G
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      if received PDT number or application number are invalid then set
\***      RECEIVE.STATE$ to "*" and return
\***
\***      close STKMQ file if open
\***
\***      set FILE.TRAILER.LISTS% to numbers of lists on trailer record
\***
\***      if the received file trailer is for COUNTS processing then
\***         if received list count does not equal actual list count then
\***            log an event 5 (count discrepancy)
\***         endif
\***         transmit a file header back to the PDT
\***      endif
\***
\***      if the received file trailer is for LISTS (i.e successful xmission)
\***         open and delete PDTWF
\***      endif
\***
\***   EPSOM.FILE.TRAILER.CONTINUE:
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.FILE.TRAILER:            
   
      SB.MESSAGE$ = "PDT Support - EPSOM File trailer received"        
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 6) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      IF MID$(DATA.IN$, 3, 2) <> APPLICATION.NO$                        \
      OR MID$(DATA.IN$, 5, 6) <> CURR.TERMINAL$ THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

! 4 lines deleted from here                                             ! ASMG    

      FILE.TRAILER.LISTS% = VAL(MID$(DATA.IN$, 11, 2))

      IF PDT.ACTION$ = "COUNTS" THEN BEGIN
         IF FILE.TRAILER.LISTS% <> LIST.COUNT% THEN BEGIN
            SB.EVENT.NO% = 5
            SB.UNIQUE$ = "I" + CHR$(ASYNC.REPORT.NUM%) +                \
                         PACK$(FN.Z.PACK(STR$(LIST.COUNT%), 4)) +       \
                         PACK$(FN.Z.PACK(STR$(FILE.TRAILER.LISTS%),4)) +\
                         PK4$
            GOSUB SB.LOG.AN.EVENT
         ENDIF
         NUMBER.OF.LISTS% = FILE.TRAILER.LISTS%
         GOSUB TRANSMIT.EPSOM.FILE.TRAILER                            
      ENDIF

      LDTAF.LINK.TYPE% = 7                                          ! BMJK
      IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =                      \ BMJK
        (LDTAF.LINK.TYPE% OR 80H)                                   ! BMJK
      LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                            ! BMJK
      LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                            ! BMJK
      TEMP.TIME$ = TIME$                                                ! CMJK
      LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +              \ CMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +               \ CMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ CMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ CMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+       \ CMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )       ! CMJK
      LDTAF.DATA.VOLUME.1% = 0                                          ! BMJK
      LDTAF.DATA.VOLUME.1% = NUMBER.OF.LISTS%                            ! BMJK
      LDTAF.DATA.VOLUME.2% = 0                                          ! BMJK
      GOSUB LOG.TO.LDTAF.FILE                                          ! BMJK

   EPSOM.FILE.TRAILER.CONTINUE:                                       

   RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.LIST.REQUEST:                             STATE : H
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      set PDT action to "LISTS"
\***
\***      command PSS38 to 'hold' the PDT
\***
\***      open all files required
\***      create a new PDTWF
\***      GOSUB CREATE.EPSOM.TRANSMISSION.LIST
\***      GOSUB CREATE.EPSOM.TRANSMISSION.FILE
\***      close PDTWF
\***
\***      command PSS38 to 'release' the PDT
\***      command PSS38 to transmit PDTWF if receive state is not "*"
\***
\***      close all used files
\***
\***   EPSOM.LIST.REQUEST.CONTINUE:
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.LIST.REQUEST:                        
   
      SB.MESSAGE$ = "PDT Support - EPSOM List request received"       
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 7) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      PDT.ACTION$ = "LISTS"
      GOSUB HOLD.PDT

      CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +              \
                        "EPSOM list request at " +                     \
                     LEFT$(TIME$,2) + ":" +                            \
                     MID$(TIME$,3,2) + ":" +                            \
                     RIGHT$(TIME$,2)
      GOSUB LOG.TO.AUDIT.FILE

      IF END# BCSMF.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = BCSMF.SESS.NUM%
      OPEN BCSMF.FILE.NAME$ KEYED RECL BCSMF.RECL% AS BCSMF.SESS.NUM%   \
           NOWRITE NODEL
      BCSMF.OPEN.FLAG$ = "Y"

      IF END# IDF.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = IDF.SESS.NUM%
      OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%         \
           NOWRITE NODEL
      IDF.OPEN.FLAG$ = "Y"

      IF END# IEF.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = IEF.SESS.NUM%
      OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%         \
           NOWRITE NODEL
      IEF.OPEN.FLAG$ = "Y"

      IF END# IRF.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = IRF.SESS.NUM%
      OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM%         \
           NOWRITE NODEL
      IRF.OPEN.FLAG$ = "Y"

      IF END# PDTWF.SESS.NUM% THEN CREATE.ERROR
      CURR.SESS.NUM% = PDTWF.SESS.NUM%
      CREATE POSFILE PDTWF.FILE.NAME$ AS PDTWF.SESS.NUM%                \
             BUFFSIZE 10240 LOCKED
      PDTWF.OPEN.FLAG$ = "Y"

      IF END# PIITM.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = PIITM.SESS.NUM%
      OPEN PIITM.FILE.NAME$ KEYED RECL PIITM.RECL% AS PIITM.SESS.NUM%   \
           NODEL
      PIITM.OPEN.FLAG$ = "Y"

      IF END# PILST.SESS.NUM% THEN OPEN.ERROR
      CURR.SESS.NUM% = PILST.SESS.NUM%
      OPEN PILST.FILE.NAME$ DIRECT RECL 512 AS PILST.SESS.NUM%          \
           NOWRITE NODEL
      PILST.OPEN.FLAG$ = "Y"

! 5 Lines deleted from here                                            ! ASMG

      GOSUB CREATE.EPSOM.TRANSMISSION.LIST                      
      IF PILST.OPEN.FLAG$ = "Y" THEN BEGIN                              
         CLOSE PILST.SESS.NUM%                                          
         PILST.OPEN.FLAG$ = "N"                                         
      ENDIF                                                             
      IF RECEIVE.STATE$ = "*" THEN GOTO SKIP.CREATE.EPSOM.FILE          
      IF END# PILST.SESS.NUM% THEN OPEN.ERROR                           
      CURR.SESS.NUM% = PILST.SESS.NUM%                                  
      OPEN PILST.FILE.NAME$ KEYED RECL PILST.RECL% AS PILST.SESS.NUM%   \
           NODEL                                                        
      PILST.OPEN.FLAG$ = "Y"                                            
      GOSUB CREATE.EPSOM.TRANSMISSION.FILE                              

   SKIP.CREATE.EPSOM.FILE:                                              
      IF PDTWF.OPEN.FLAG$ = "Y" THEN BEGIN                              
         CLOSE PDTWF.SESS.NUM%                                          
         PDTWF.OPEN.FLAG$ = "N"                                         
      ENDIF                                                             

      IF RECEIVE.STATE$ <> "*" THEN BEGIN         
        LDTAF.LINK.TYPE% = 6                                          ! BMJK
        IF MONITORED.PORT$ = "B" THEN LDTAF.LINK.TYPE% =               \ BMJK
          (LDTAF.LINK.TYPE% OR 80H)                                   ! BMJK
        LDTAF.START.DATE$ = PACK$(LOG.ON.DATE$)                            ! BMJK
        LDTAF.START.TIME$ = PACK$(LOG.ON.TIME$)                            ! BMJK
        TEMP.TIME$ = TIME$                                              ! CMJK
        LDTAF.DURATION% = ( (VAL(LEFT$(TEMP.TIME$,2))*3600) +              \ CMJK
                          (VAL(MID$(TEMP.TIME$,3,2))*60) +               \ CMJK
                           VAL(RIGHT$(TEMP.TIME$,2)) ) -                \ CMJK
                      ( (VAL(UNPACK$(LEFT$(LDTAF.START.TIME$,1)))*3600)+\ CMJK
                        (VAL(UNPACK$(MID$(LDTAF.START.TIME$,2,1)))*60)+       \ CMJK
                         VAL(UNPACK$(RIGHT$(LDTAF.START.TIME$,1))) )       ! CMJK
        LDTAF.DATA.VOLUME.1% = 0                                   ! BMJK
        LDTAF.DATA.VOLUME.1% = SIZE (PDTWF.FILE.NAME$)                     ! BMJK
        LDTAF.DATA.VOLUME.2% = 0                                   ! BMJK
        GOSUB LOG.TO.LDTAF.FILE                                          ! BMJK
        GOSUB RELEASE.PDT
        GOSUB TRANSMIT.EPSOM.FILE                                      
      ENDIF                                                             

      CLOSE BCSMF.SESS.NUM%
      BCSMF.OPEN.FLAG$ = "N"

      CLOSE IDF.SESS.NUM%
      IDF.OPEN.FLAG$ = "N"

      CLOSE IEF.SESS.NUM%
      IEF.OPEN.FLAG$ = "N"

      CLOSE IRF.SESS.NUM%
      IRF.OPEN.FLAG$ = "N"

      CLOSE PIITM.SESS.NUM%
      PIITM.OPEN.FLAG$ = "N"

      IF PILST.OPEN.FLAG$ = "Y" THEN BEGIN                              
         CLOSE PILST.SESS.NUM%
         PILST.OPEN.FLAG$ = "N"
      ENDIF                                                             

! 2 Lines deleted from here                                            ! ASMG

   EPSOM.LIST.REQUEST.CONTINUE:                                        

   RETURN

\******************************************************************************
\***
\***   RECEIVED.EPSOM.EOT:                                      STATE : I
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      close the EPSOM file ( PDT has finished )
\***
\***      gosub warm.start to resolve memory problems.
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.EPSOM.EOT:                                                 

      SB.MESSAGE$ = "PDT Support - EPSOM E.O.T. received"              
      GOSUB SB.BG.MESSAGE                                              

      IF FN.VALIDATE.DATA(DATA.IN$, 8) = 0 THEN BEGIN                  
         RECEIVE.STATE$ = "*"                                          
         RETURN                                                        
      ENDIF                                                            

      IF EPSOM.OPEN.FLAG$ = "Y" THEN BEGIN                             
         CLOSE EPSOM.SESS.NUM%                                         
         EPSOM.OPEN.FLAG$ = "N"                                        
      ENDIF                                                            

      CSR.AUDIT.DATA$ = "EPSOM session complete at " +                     \
                        LEFT$(TIME$,2) + ":" +                            \
                     MID$(TIME$,3,2) + ":" +                            \
                     RIGHT$(TIME$,2)
      CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +              \
                        CSR.AUDIT.DATA$
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
\***
\***   E P S O M   S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   TRANSMIT.EPSOM.FILE.TRAILER:
\***
\***      send a file trailer to the PSS38
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.EPSOM.FILE.TRAILER:                 

      PIPE.OUT$ = "L" +                                                 \
                  STX$ +                                                \
                  "CT" +                                                \
                  FN.Z.PACK(APPLICATION.NO$,2) +                        \
                  FN.Z.PACK(CURR.TERMINAL$,6) +                         \
                  FN.Z.PACK(STR$(NUMBER.OF.LISTS%),2)

      GOSUB SEND.TO.PSS38

   RETURN

\******************************************************************************
\***
\***   CREATE.EPSOM.TRANSMISSION.LIST:
\***
\***      from the requested lists data sent from the PDT create the
\***      array REQ.LIST.STORE$() with REQ.LIST.DATA$() containing the
\***      letter "C" to indicate a count. Also create the string REQ.BC$
\***      containing a list of requested business centre letters.
\***
\***      for each list contained on the PILST (list of lists) file GOSUB ...
\***      ... PROCESS.AN.EPSOM.LIST
\***
\***      GOSUB ADD.EXCEPTIONS.TO.EPSOM.LIST
\***
\***   RETURN
\***
\******************************************************************************

   CREATE.EPSOM.TRANSMISSION.LIST:                  
   
      REQ.LIST.PTR% = 0
      LIST.EXTRACTION% = 0
      REQ.BC$ = ""
      SELECT.ALL.FLAG$ = "N"
      WHILE ((LIST.EXTRACTION% * 4) + 10) < LEN(DATA.IN$)
         REQ.LIST$ = MID$(DATA.IN$, (LIST.EXTRACTION% * 4) + 10, 4)
         BC$ = RIGHT$(REQ.LIST$, 1)
         IF BC$ >= "A" AND BC$ <= "Z" THEN BEGIN
            REQ.BC$ = REQ.BC$ + BC$
            IF BC$ = "X" THEN SELECT.ALL.FLAG$ = "Y"
         ENDIF ELSE BEGIN
            REQ.LIST.PTR% = REQ.LIST.PTR% + 1
            REQ.LIST.STORE$(REQ.LIST.PTR%) = REQ.LIST$
            REQ.LIST.DATA$(REQ.LIST.PTR%)  = "C"
         ENDIF
         LIST.EXTRACTION% = LIST.EXTRACTION% + 1
      WEND


      LIST.TRANSMIT.PTR% = 0

      SECTOR.COUNT% = 2
      LAST.REC% = ((INT(508 / PILST.RECL%) - 1) * PILST.RECL%) + 1
      PILST.EOF.FLAG$ = "N"
      CURR.SESS.NUM% = PILST.SESS.NUM%
      IF END# PILST.SESS.NUM% THEN PILST.READ.FAIL
      WHILE PILST.EOF.FLAG$ <> "Y" AND RECEIVE.STATE$ <> "*"            

         CURR.TIME$ = TIME$                                             
         IF MOD(VAL(CURR.TIME$), INT.DELAY%) = 0                        \
         AND LAST.TIME$ <> CURR.TIME$ THEN BEGIN                        
            GOSUB INTERROGATE.PSS38                                     
            LAST.TIME$ = CURR.TIME$                                     
            IF LEFT$(PSS38.STATUS$,2) = "I" + "N" THEN BEGIN            
               RECEIVE.STATE$ = "*"                                     
               GOTO RETURN.PILST.EOF                                    
            ENDIF                                                       
         ENDIF                                                          

         READ FORM "C4,C508";# PILST.SESS.NUM%, SECTOR.COUNT%;          \
              CONTROL$, SECTOR$
         IF LEFT$(SECTOR$, 4) <> PK4$ THEN BEGIN                        
            FOR POSITION% = 1 TO LAST.REC% STEP PILST.RECL%
               PILST.RECORD$ = MID$(SECTOR$, POSITION%, PILST.RECL%)
               KEY.VALUE$ = LEFT$(PILST.RECORD$, 4)
               IF VAL(UNPACK$(KEY.VALUE$)) <> 0                         \
               AND KEY.VALUE$ <> "0000" THEN BEGIN
                  GOSUB PROCESS.AN.EPSOM.LIST                           
               ENDIF
            NEXT POSITION%
         ENDIF                                                          
         SECTOR.COUNT% = SECTOR.COUNT% + 1

      RETURN.PILST.EOF:
      WEND

      IF RECEIVE.STATE$ <> "*" THEN BEGIN                               
         GOSUB ADD.EXCEPTIONS.TO.EPSOM.LIST                             
      ENDIF                                                             

   RETURN

\******************************************************************************
\***
\***   PROCESS.AN.EPSOM.LIST:
\***
\***      GOSUB WAS.EPSOM.LIST.REQUESTED to find out whether current list is a
\***      requested list
\***
\***      if the list status is "X" (cancelled) or "F" (counted) then
\***         if the list is a requested list then
\***            set the list's REQ.LIST.DATA$() to the list status
\***         endif
\***         return
\***      endif
\***
\***      if the lists status is "C" (count) and its count date is valid
\***      and greater than today's date then return
\***
\***      if the lists status is "R" (recount) and its recount date is valid
\***      and greater than today's date then return
\***
\***      if the list number is a requested list then
\***         GOSUB STORE.EPSOM.LIST.INFO
\***         set the list's REQ.LIST.DATA$() to "D" (delete request)
\***      endif
\***
\***      if all business centres were requested or the current business
\***      centre is a requested business centre letter then
\***         GOSUB STORE.EPSOM.LIST.INFO
\***         set the "X" entry within BC.CHECK%() to 1
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   PROCESS.AN.EPSOM.LIST:                              
   
      PILST.LIST.NUMBER$ = KEY.VALUE$
      PILST.LIST.NAME$ = MID$(PILST.RECORD$, 5, 12)
      PILST.BC.LETTER$ = MID$(PILST.RECORD$, 17, 1)
      PILST.LIST.TYPE$ = MID$(PILST.RECORD$, 18, 1)
      PILST.COUNT.BY.DATE$ = MID$(PILST.RECORD$, 19, 3)
      PILST.PRODUCT.GROUP$ = MID$(PILST.RECORD$, 22, 3)
      PILST.ITEMS.IN.LIST% = FN.CONV.TO.INTEGER(MID$(PILST.RECORD$, 25, 1))
      PILST.TO.BE.COUNTED% = FN.CONV.TO.INTEGER(MID$(PILST.RECORD$, 26, 1))
      PILST.COUNT.DATE$ = MID$(PILST.RECORD$, 27, 3)
      PILST.LIST.STATUS$ = MID$(PILST.RECORD$, 30, 1)
      PILST.RECOUNT.DATE$ = MID$(PILST.RECORD$, 31, 3)
      PILST.RECOUNT.ALLOWED$ = MID$(PILST.RECORD$, 34, 1)
      SORT.ORDER$ = "B"                                                 
      IF PILST.LIST.TYPE$ = "E" THEN SORT.ORDER$ = "A"                  
      SORT.DATE$ = UNPACK$(PILST.COUNT.BY.DATE$)
      LIST.BC$ = PILST.BC.LETTER$

      COUNT.DATE$ = UNPACK$(PILST.COUNT.DATE$)
      RECOUNT.DATE$ = UNPACK$(PILST.RECOUNT.DATE$)
      TEST.LIST$ = PILST.LIST.NUMBER$
      GOSUB WAS.EPSOM.LIST.REQUESTED                                    

      IF PILST.LIST.STATUS$ = "X" OR PILST.LIST.STATUS$ = "F" THEN BEGIN
         IF REQ.FLAG$ = "Y" THEN REQ.LIST.DATA$(LIST%) = PILST.LIST.STATUS$
         RETURN
      ENDIF

      IF PILST.LIST.STATUS$ = "C" THEN BEGIN
         IF COUNT.DATE$ = "000000"                                      \
         OR DATE.GT(COUNT.DATE$,DATE.TODAY$)  THEN BEGIN               ! ENS        
            IF REQ.FLAG$ = "Y" THEN BEGIN
               SORT.ORDER$ = "C"                                        
               SORT.DATE$ = "850101"                                   ! FNS
               PILST.LIST.NAME$ = "Future List"                         
               PILST.ITEMS.IN.LIST% = 0
               PILST.LIST.STATUS$ = "?"
               PILST.LIST.TYPE$ = "?"
               GOSUB STORE.EPSOM.LIST.INFO                              
               REQ.LIST.DATA$(LIST%) = "D"
            ENDIF
            RETURN
         ENDIF
      ENDIF

      IF PILST.LIST.STATUS$ = "R" THEN BEGIN
         IF RECOUNT.DATE$ = "000000"                                    \
         OR DATE.GT(RECOUNT.DATE$,DATE.TODAY$) THEN BEGIN              ! ENS        
            IF REQ.FLAG$ = "Y" THEN BEGIN
               SORT.ORDER$ = "C"                                        
               SORT.DATE$ = "850101"                                   ! FNS
               PILST.LIST.NAME$ = "Future List"                         
               PILST.ITEMS.IN.LIST% = 0
               PILST.LIST.STATUS$ = "?"
               PILST.LIST.TYPE$ = "?"
               GOSUB STORE.EPSOM.LIST.INFO                              
               REQ.LIST.DATA$(LIST%) = "D"
            ENDIF
            RETURN
         ENDIF
      ENDIF

      IF REQ.FLAG$ = "Y" THEN BEGIN
         GOSUB STORE.EPSOM.LIST.INFO                                    
         REQ.LIST.DATA$(LIST%) = "D"
         RETURN
      ENDIF

      IF (SELECT.ALL.FLAG$ = "Y" AND PILST.BC.LETTER$ <> "Z")                                           \
      OR MATCH(PILST.BC.LETTER$, REQ.BC$, 1) > 0 THEN BEGIN
         GOSUB STORE.EPSOM.LIST.INFO                                    
         BC.CHECK%(ASC("X") - ASC("A") + 1) = 1
         RETURN
      ENDIF

   RETURN


\******************************************************************************
\***
\***   WAS.EPSOM.LIST.REQUESTED:
\***
\***      Determine whether TEST.LIST$ was a list requested by the PDT
\***      (REQ.FLAG$ = "Y" for a requested list,
\***                   "N" for a non-requested list.)
\***
\***   RETURN
\***
\******************************************************************************

   WAS.EPSOM.LIST.REQUESTED:                                            

      REQ.FLAG$ = "N"

      IF REQ.LIST.PTR% < 1 THEN RETURN

      LIST% = 0
      WHILE LIST% < REQ.LIST.PTR% AND REQ.FLAG$ = "N"
         LIST% = LIST% + 1
         IF REQ.LIST.STORE$(LIST%) = TEST.LIST$ THEN REQ.FLAG$ = "Y"
      WEND

   RETURN

\******************************************************************************
\***
\***   STORE.EPSOM.LIST.INFO:
\***
\***      Add a list number to the list transmission list
\***      (this list contains the list numbers of all lists that will
\***      eventually be transmitted to the PDT)
\***
\***   RETURN
\***
\******************************************************************************

   STORE.EPSOM.LIST.INFO:                                               

      PILST.LIST.NAME$ = LEFT$(PILST.LIST.NAME$ + STRING$(16, " "), 16)
      LIST.TRANSMIT.PTR% = LIST.TRANSMIT.PTR% + 1
      LIST.TRANSMIT$(LIST.TRANSMIT.PTR%) =                              \
                SORT.ORDER$ +                                           \
                SORT.DATE$ +                                            \
                PILST.LIST.NUMBER$ +                                    \
                FN.TRANSLATE.TEXT(PILST.LIST.NAME$) +                   \
                FN.Z.PACK(STR$(PILST.ITEMS.IN.LIST%), 3) +              \
                PILST.LIST.STATUS$ +                                    \
                PILST.LIST.TYPE$ +                                      \
                LIST.BC$

      IF PILST.LIST.STATUS$ = "C"                                       \
      OR PILST.LIST.STATUS$ = "R" THEN BEGIN
         BC.CHECK%(ASC(PILST.BC.LETTER$) - ASC("A") + 1) = 1
      ENDIF

   RETURN

\******************************************************************************
\***
\***   ADD.EXCEPTIONS.TO.EPSOM.LIST:
\***
\***      add to the transmission list a list of all invalid / unavailable
\***      lists and business centre codes
\***
\***   RETURN
\***
\******************************************************************************

   ADD.EXCEPTIONS.TO.EPSOM.LIST:                                        
      SORT.PTR% = 1
      GOSUB SORT.EPSOM.TRANSMISSION.LIST                                

      FOR LIST% = 1 TO REQ.LIST.PTR%
         PILST.LIST.NAME$ = "List Error"
         DATA$ = REQ.LIST.DATA$(LIST%)
         IF DATA$ <> "D" THEN BEGIN
            IF DATA$ = "C" THEN PILST.LIST.NAME$ = "List Not Found"
            IF DATA$ = "F" THEN PILST.LIST.NAME$ = "List Completed"
            IF DATA$ = "X" THEN PILST.LIST.NAME$ = "List Cancelled"
            SORT.ORDER$ = "C"                                           
            SORT.DATE$ = "850101"                                      ! FNS
            PILST.LIST.NUMBER$ = REQ.LIST.STORE$(LIST%)
            PILST.ITEMS.IN.LIST% = 0
            PILST.LIST.STATUS$ = "?"
            PILST.LIST.TYPE$ = "?"
            GOSUB STORE.EPSOM.LIST.INFO                                 
         ENDIF
      NEXT LIST%

      GOSUB SORT.EPSOM.TRANSMISSION.LIST                                

      IF LEN(REQ.BC$) > 0 THEN BEGIN
         FOR BC% = 1 TO LEN(REQ.BC$)
            BCSMF.FSI$ = MID$(REQ.BC$, BC%, 1)
            IF (BC.CHECK%(ASC(BCSMF.FSI$) - ASC("A") + 1) = 0)          \
            AND BCSMF.FSI$ <> "X" THEN BEGIN
               CURRENT.KEY$ = BCSMF.FSI$
               RC% = READ.BCSMF                                          
              IF RC% = 1 THEN GOTO BC.NOT.FOUND                     
               SORT.ORDER$ = "C"                                        
               SORT.DATE$ = "850101"                                   ! FNS
               PILST.LIST.NUMBER$ = BCSMF.FSI$ + "   "
               PILST.LIST.NAME$ = "Lists Not Found"
               PILST.ITEMS.IN.LIST% = 0                                 
               PILST.LIST.STATUS$ = "?"
               PILST.LIST.TYPE$ = "?"
               GOSUB STORE.EPSOM.LIST.INFO                              
            ENDIF
         RETURN.TO.BC.CHECK:
         NEXT BC%
      ENDIF

      GOSUB SORT.EPSOM.TRANSMISSION.LIST                                

   RETURN

\******************************************************************************
\***
\***   SORT.EPSOM.TRANSMISSION.LIST:
\***
\***      sort the transmission list into ascending order of list number
\***      (the subroutine actually sorts a group of list numbers within
\***      the main transmission list so the valid lists can be separated
\***      from the invalid / unavailable lists and business centres)
\***
\***   RETURN
\***
\******************************************************************************

   SORT.EPSOM.TRANSMISSION.LIST:                     

      IF SORT.PTR% >= LIST.TRANSMIT.PTR% THEN RETURN
       
      FOR A% = SORT.PTR% TO LIST.TRANSMIT.PTR%                         ! ENS
         TEMP.LIST$ = "19"                                             ! ENS
         IF MID$(LIST.TRANSMIT$(A%),2,2) < "85" THEN TEMP.LIST$ = "20" ! ENS
         LIST.TRANSMIT$(A%) = LEFT$(LIST.TRANSMIT$(A%),1) +            \ ENS
            TEMP.LIST$ + RIGHT$(LIST.TRANSMIT$(A%),                     \ ENS
               LEN(LIST.TRANSMIT$(A%)) - 1)                             ! ENS
      NEXT A%                                                          ! ENS


      FOR A% = SORT.PTR% TO (LIST.TRANSMIT.PTR% - 1)
         FOR B% = (A% + 1) TO LIST.TRANSMIT.PTR%
            IF LIST.TRANSMIT$(A%) > LIST.TRANSMIT$(B%) THEN BEGIN
               TEMP.LIST$ = LIST.TRANSMIT$(A%)
               LIST.TRANSMIT$(A%) = LIST.TRANSMIT$(B%)
               LIST.TRANSMIT$(B%) = TEMP.LIST$
            ENDIF
         NEXT B%
      NEXT A%

      FOR A% = SORT.PTR% TO LIST.TRANSMIT.PTR%                         ! ENS
         LIST.TRANSMIT$(A%) = LEFT$(LIST.TRANSMIT$(A%),1) +            \ ENS
             RIGHT$(LIST.TRANSMIT$(A%), LEN(LIST.TRANSMIT$(A%)) - 3)   ! ENS
      NEXT A%                                                          ! ENS



      SORT.PTR% = LIST.TRANSMIT.PTR% + 1

   RETURN

\******************************************************************************
\***
\***   CREATE.EPSOM.TRANSMISSION.FILE:
\***
\***      create a PDTWF file header and write it to the PDTWF
\***      work through the list created by CREATE.TRANSMISSION.LIST
\***      and for each list entry do the following
\***         create and write a list header to the PDTWF
\***         for each item within the list GOSUB GET.EPSOM.ITEM.INFO,
\***         which sets-up
\***         PDTWF.RECORD$
\***         if PDTWF.RECORD$ is not null then write it to the PDTWF
\***         create and write a list trailer to the PDTWF
\***
\***      create a PDTWF file header and write it to the PDTWF
\***
\***   RETURN
\***
\******************************************************************************

   CREATE.EPSOM.TRANSMISSION.FILE:                                      

      IF STKMQ.OPEN.FLAG$ <> "Y" THEN BEGIN                             ! ASMG
       IF END# STKMQ.SESS.NUM% THEN OPEN.ERROR                          ! ASMG
       CURR.SESS.NUM% = STKMQ.SESS.NUM%                                 ! ASMG
       OPEN STKMQ.FILE.NAME$ AS STKMQ.SESS.NUM% BUFFSIZE 512 READONLY  \ ASMG
       NODEL APPEND                                                 ! ASMG
       STKMQ.OPEN.FLAG$ = "Y"                                           ! ASMG
       TOTAL.WAIT.TIME% = 0                                             ! ASMG
      ENDIF                                                             ! ASMG

      NESTING.LEVEL% = 0
      PDTWF.RECORD$ = STX$ + "CH" + APPLICATION.NO$ +                   \
                      CURR.TERMINAL$ +                                  \
                      FN.Z.PACK(STR$(LIST.TRANSMIT.PTR%) , 2)
      CURR.SESS.NUM% = PDTWF.SESS.NUM%
      CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)
      RC% = WRITE.PDTWF                                                 
      IF RC% = 1 THEN GOTO WRITE.ERROR                                   
      NESTING.LEVEL% = 1
      FOR LIST% = 1 TO LIST.TRANSMIT.PTR%

         IF RECEIVE.STATE$ = "*" THEN GOTO BACK.OUT.EPSOM.LIST.LOOP     
         LIST.NUMBER$ = MID$(LIST.TRANSMIT$(LIST%), 8, 4)
         LIST.NAME$ = MID$(LIST.TRANSMIT$(LIST%), 12, 16)
         NUMBER.OF.ITEMS% = VAL(MID$(LIST.TRANSMIT$(LIST%), 28, 3))
         LIST.STATUS$ = MID$(LIST.TRANSMIT$(LIST%), 31, 1)
         LIST.TYPE$ = MID$(LIST.TRANSMIT$(LIST%), 32, 1)
         LIST.BC$ = MID$(LIST.TRANSMIT$(LIST%), 33, 1)
         USER.DATA$ = LEFT$(LIST.STATUS$ +                              \
                            LIST.TYPE$ +                                \
                            LIST.BC$ +                                  \
                            "-----", 5)
         ITEM.TRAILER.COUNT% = 0
         LISTS.CANCELLED% = 0                                           
         RECORDS.WRITTEN% = 0                                           
         PDTWF.HEADER.RECORD$ = "LH" + LIST.NUMBER$ + LIST.NAME$ +      \ 
                                USER.DATA$                              
         IF RECEIVE.STATE$ = "*" THEN GOTO BACK.OUT.EPSOM.LIST.LOOP     
         IF LIST.STATUS$ = "C" OR LIST.STATUS$ = "R" THEN BEGIN
            NESTING.LEVEL% = 2
            FOR ITEM% = 1 TO NUMBER.OF.ITEMS%

               CURR.TIME$ = TIME$                                       
               IF MOD(VAL(CURR.TIME$), INT.DELAY%) = 0                  \
               AND LAST.TIME$ <> CURR.TIME$ THEN BEGIN                  
                  GOSUB INTERROGATE.PSS38                               
                  LAST.TIME$ = CURR.TIME$                               
                  IF LEFT$(PSS38.STATUS$,2) = "I" + "N" THEN BEGIN      
                     RECEIVE.STATE$ = "*"                               
                  ENDIF                                                 
               ENDIF                                                    

               IF RECEIVE.STATE$ = "*" THEN GOTO BACK.OUT.EPSOM.ITEM.LOOP
               GOSUB GET.EPSOM.ITEM.INFO                                
               IF PDTWF.RECORD$ <> "" THEN BEGIN
                  IF RECORDS.WRITTEN% = 0 THEN BEGIN                   
                     TEMP.PDTWF.RECORD$ = PDTWF.RECORD$                
                     PDTWF.RECORD$ = PDTWF.HEADER.RECORD$              
                     CURR.SESS.NUM% = PDTWF.SESS.NUM%                  
                     CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)             
                     RC% = WRITE.PDTWF                                 
                   IF RC% = 1 THEN GOTO WRITE.ERROR                     
                     PDTWF.RECORD$ = TEMP.PDTWF.RECORD$                 
                  ENDIF                                                 
                  CURR.SESS.NUM% = PDTWF.SESS.NUM%
                  CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)
                  RC% = WRITE.PDTWF                                   
                IF RC% = 1 THEN GOTO WRITE.ERROR                     
                  RECORDS.WRITTEN% = RECORDS.WRITTEN% + 1               
               ENDIF
            BACK.OUT.EPSOM.ITEM.LOOP:                                   
            NEXT ITEM%
            NESTING.LEVEL% = 1
            IF RECEIVE.STATE$ = "*" THEN GOTO BACK.OUT.EPSOM.LIST.LOOP  
         ENDIF                                                          
         IF RECORDS.WRITTEN% = 0                                        \
         AND NUMBER.OF.ITEMS% > 0 THEN BEGIN                            
            PDTWF.RECORD$ = LEFT$(PDTWF.HEADER.RECORD$, 6) +            \
                            "Items Not Found " +                        \
                            MID$(PDTWF.HEADER.RECORD$, 23, 5)           
            CURR.SESS.NUM% = PDTWF.SESS.NUM%                            
            CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)                       
            RC% = WRITE.PDTWF                                           
           IF RC% = 1 THEN GOTO WRITE.ERROR                            
            RECORDS.WRITTEN% = RECORDS.WRITTEN% + 1                     
         ENDIF                                                          
         IF NUMBER.OF.ITEMS% = LISTS.CANCELLED%                         \
         AND NUMBER.OF.ITEMS% > 0 THEN BEGIN                            
            PILST.LIST.NUMBER$ = LIST.NUMBER$                           
            CURR.SESS.NUM% = PILST.SESS.NUM%                            
            CURRENT.KEY$ = LIST.NUMBER$                                 
            RC% = READ.PILST                                            
           IF RC% = 1 THEN GOTO READ.ERROR                            
            PILST.LIST.STATUS$ = "X"                                    
            CURR.SESS.NUM% = PILST.SESS.NUM%                            
            CURRENT.KEY$ = LIST.NUMBER$                                 
            RC% = WRITE.PILST                                           
           IF RC% = 1 THEN GOTO WRITE.ERROR                            
         ENDIF                                                          
         IF LIST.STATUS$ = "?" THEN BEGIN                               
            NUMBER.OF.ITEMS% = 0                                        
            IF RECORDS.WRITTEN% = 0 THEN BEGIN                          
               PDTWF.RECORD$ = PDTWF.HEADER.RECORD$                     
               CURR.SESS.NUM% = PDTWF.SESS.NUM%                         
               CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)                    
               RC% = WRITE.PDTWF                                        
              IF RC% = 1 THEN GOTO WRITE.ERROR                            
            ENDIF                                                       
         ENDIF                                                          
         PDTWF.RECORD$ = "LT" + LIST.NUMBER$ +                          \
                         RIGHT$("000"+STR$(ITEM.TRAILER.COUNT%), 3)
         CURR.SESS.NUM% = PDTWF.SESS.NUM%
         CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)
         RC% = WRITE.PDTWF                                          
        IF RC% = 1 THEN GOTO WRITE.ERROR                            
      BACK.OUT.EPSOM.LIST.LOOP:                                         
      NEXT LIST%
      NESTING.LEVEL% = 0
      IF RECEIVE.STATE$ = "*" THEN RETURN
      PDTWF.RECORD$ = "CT" +                                            \
                      APPLICATION.NO$ +                                 \
                      CURR.TERMINAL$ +                                  \
                      FN.Z.PACK(STR$(LIST.TRANSMIT.PTR%), 2)
      CURR.SESS.NUM% = PDTWF.SESS.NUM%
      CURRENT.KEY$ = LEFT$(PDTWF.RECORD$,8)
      RC% = WRITE.PDTWF                                                 
      IF RC% = 1 THEN GOTO WRITE.ERROR                                   

   CREATE.EPSOM.XM.FILE.CONTINUE:                                       

       IF STKMQ.OPEN.FLAG$ = "Y" THEN BEGIN                             ! ASMG 
         CLOSE STKMQ.SESS.NUM%                                          ! ASMG
         STKMQ.OPEN.FLAG$ = "N"                                         ! ASMG
       ENDIF                                                            ! ASMG

   RETURN


\******************************************************************************
\***
\***   GET.EPSOM.ITEM.INFO:
\***
\***      for each item number passed do the following ;
\***      read the IDF ( to get the item description )
\***      if the item is on the IDF then
\***         get 2 bar codes ( from the IEF if available, if not use the IDF
\***                          bar code(s) )
\***         read the IRF ( to get the price )
\***         if the IRF was not readable or the item code was not the same
\***         as the one on the IDF then set the price to zero
\***         format a PDTWF record
\***         write a type 12 transaction to the STKMQ
\***      else
\***         update the item's status, on the item list file, to "X"
\***      endif
\***
\***   RETURN
\***
\******************************************************************************

   GET.EPSOM.ITEM.INFO:                                                 

      PDTWF.RECORD$ = ""

      PIITM.LIST.ITEM.KEY$ = LIST.NUMBER$ + RIGHT$("00"+STR$(ITEM%), 2)
      CURR.SESS.NUM% = PIITM.SESS.NUM%
      CURRENT.KEY$ = PIITM.LIST.ITEM.KEY$
      RC% = READ.PIITM                                                 
      IF RC% = 1 THEN GOTO READ.ERROR                                   
      IF RECEIVE.STATE$ = "*" THEN RETURN
      IF PIITM.LIST.STATUS$="C" OR PIITM.LIST.STATUS$="R" THEN BEGIN
         IDF.BOOTS.CODE$ = PIITM.ITEM.CODE$
         CURR.SESS.NUM% = IDF.SESS.NUM%
         CURRENT.KEY$ = UNPACK$(PIITM.ITEM.CODE$)
         RC% = READ.IDF                                                 
        IF RC% = 1 THEN GOTO NOT.ON.IDF                            
         ITEM.ON.IDF.FLAG$ = "Y"
      NO.IDF.REC.RETURN1:
         IF ITEM.ON.IDF.FLAG$ = "Y" THEN BEGIN
            BAR.CODES% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))

            FOR BC%=1 TO 2
               BAR.CODES$(BC%) = STRING$(12,"0")
            NEXT BC%
            BAR.CODE.PTR% = 0
            IF BAR.CODES% < 3 THEN BEGIN

               BAR.CODE.PTR% = BAR.CODE.PTR% + 1
               BAR.CODES$(BAR.CODE.PTR%) = UNPACK$(IDF.FIRST.BAR.CODE$)
               IF BAR.CODES% = 2 THEN BEGIN
                  BAR.CODE.PTR% = BAR.CODE.PTR% + 1
                  BAR.CODES$(BAR.CODE.PTR%) = UNPACK$(IDF.SECOND.BAR.CODE$)
               ENDIF

            ENDIF ELSE BEGIN

               BAR.CODE.PTR% = BAR.CODE.PTR% + 1
               BAR.CODES$(BAR.CODE.PTR%) = UNPACK$(IDF.SECOND.BAR.CODE$)
               F11.CURRENT.COUNT% = 1
               F11.NEXT.BAR.CODE$ = IDF.SECOND.BAR.CODE$
               MORE.FLAG$ = "Y"
               WHILE BAR.CODE.PTR% < 2 AND MORE.FLAG$ = "Y"
                  CURR.SESS.NUM% = IEF.SESS.NUM%
                  RC% = READ.NEXT.IEF( F11.NEXT.BAR.CODE$,              \
                                      BAR.CODES%,                       \
                                      RIGHT$(UNPACK$(PIITM.ITEM.CODE$),7) )
                  IF UNPACK$(F11.NEXT.BAR.CODE$) = STRING$(12, "0")     \
                  OR RC% <> 0 THEN BEGIN                            
                     MORE.FLAG$ = "N"
                  ENDIF ELSE BEGIN
                     BAR.CODE.PTR% = BAR.CODE.PTR% + 1
                     BAR.CODES$(BAR.CODE.PTR%) = UNPACK$(F11.NEXT.BAR.CODE$)
                  ENDIF
               WEND

            ENDIF

            IF BAR.CODE.PTR% < 2 AND BAR.CODES% > 1 THEN BEGIN
               BAR.CODES$(2) = BAR.CODES$(1)
               BAR.CODES$(1) = UNPACK$(IDF.FIRST.BAR.CODE$)
               BAR.CODE.PTR% = 2
            ENDIF

            IRF.BAR.CODE$ = FN.Z.PACK(UNPACK$(IDF.FIRST.BAR.CODE$), 22)
            IRF.BAR.CODE$ = PACK$(IRF.BAR.CODE$)
            CURR.SESS.NUM% = IRF.SESS.NUM%
            RC% = READ.IRF                                          
           IF RC% = 1 THEN GOTO IRF.ERROR                            
            PRICE$ = "000000"
!           IRF.PRICE.TYPE$ = MID$(UNPACK$(IRF.INDICAT2$), 2, 1)        !GSWM
!           IF IRF.PRICE.TYPE$ = "0" OR IRF.PRICE.TYPE$ = "1" THEN      !GSWM
               PRICE$ = MID$(UNPACK$(IRF.SALEPRIC$), 4, 7)              \
!           ELSE                                                        !GSWM
!              IF IRF.PRICE.TYPE$ = "2" THEN                            !GSWM
!                 PRICE$ = MID$(UNPACK$(IRF.SALEPRIC$), 6, 5)           !GSWM
!              ELSE                                                     !GSWM
!                 IF IRF.PRICE.TYPE$ = "3" THEN                         !GSWM
!                    PRICE$ = MID$(UNPACK$(IRF.SALEPRIC$), 1, 5)        !GSWM
            IF UNPACK$(IRF.BOOTS.CODE$)                                 \
            <> MID$(UNPACK$(IDF.BOOTS.CODE$), 2, 6) THEN                \
               GOTO IRF.ERROR
         IRF.ERROR.RETURN:

            GOSUB FORMAT.BAR.CODES

            USER.DATA$ = FN.Z.PACK(STR$(ITEM%), 2) +                    \
                         LEFT$(PIITM.LIST.STATUS$+" ", 1) +             \
                         "------"
            IDF.STNDRD.DESC$ = LEFT$(IDF.STNDRD.DESC$+STRING$(24," "),24)
            PDTWF.RECORD$ = "LI" +                                      \
                            RIGHT$(UNPACK$(PIITM.ITEM.CODE$),7) +       \
                            FN.TRANSLATE.TEXT(IDF.STNDRD.DESC$) +       \
                            FN.Z.PACK(PRICE$, 6) +                      \
                            FN.Z.PACK(PIITM.FAMILY.MARKER$, 1) +        \
                            FN.Z.PACK(PIITM.MEMBERS$, 1) +              \
                            BAR.CODES$(1) +                             \
                            BAR.CODES$(2) +                             \
                            USER.DATA$
            ITEM.TRAILER.COUNT% = ITEM.TRAILER.COUNT% + 1

            IF LIST.BC$ <> "Z" THEN BEGIN
               STKMQ.TRANS.TYPE$ = PACK$("12")
               STKMQ.DATE$ = PACK$(DATE$)
               STKMQ.TIME$ = PACK$(TIME$)
               STKMQ.BOOTS.CODE$ = IDF.BOOTS.CODE$
               STKMQ.RECORD$ = STKMQ.RECORD.DELIMITER$ +                \
                               STKMQ.TRANS.TYPE$ +                      \
                               STKMQ.FIELD.DELIMITER$ +                 \
                               STKMQ.DATE$ +                            \
                               STKMQ.TIME$ +                            \
                               STKMQ.BOOTS.CODE$ +                      \
                               STKMQ.RECORD.DELIMITER$ +                \
                               STKMQ.ENDREC.MARKER$
               CURR.SESS.NUM% = STKMQ.SESS.NUM%
               CURRENT.KEY$ = ""
               RC% = WRITE.STKMQ                                   
              IF RC% = 1 THEN GOTO WRITE.ERROR                            
            ENDIF
            IF RECEIVE.STATE$ = "*" THEN RETURN
         ENDIF ELSE BEGIN
            PDTWF.RECORD$ = ""
            PIITM.LIST.STATUS$ = "X"
            CURR.SESS.NUM% = PIITM.SESS.NUM%
            CURRENT.KEY$ = PIITM.LIST.ITEM.KEY$
            RC% = WRITE.PIITM                                          
           IF RC% = 1 THEN GOTO WRITE.ERROR                            
            LISTS.CANCELLED% = LISTS.CANCELLED% + 1                     
            IF RECEIVE.STATE$ = "*" THEN RETURN
         ENDIF
      ENDIF ELSE BEGIN                                                  
         IF PIITM.LIST.STATUS$ = "X" THEN BEGIN                        
            LISTS.CANCELLED% = LISTS.CANCELLED% + 1                    
         ENDIF                                                         
      ENDIF                                                            

   GET.EPSOM.ITEM.INFO.CONTINUE:                                       
   RETURN

\******************************************************************************
\***
\***   FORMAT.BAR.CODES:
\***
\***      add a check digit onto bar codes to be passed to PDT
\***      note : instore bar codes are treated differently to non-instore
\***             bar codes
\***
\***   RETURN
\***
\******************************************************************************

   FORMAT.BAR.CODES:

      FOR BC% = 1 TO 2
         BAR.CODES$(BC%) = FN.Z.PACK(BAR.CODES$(BC%),12)
         BC$ = BAR.CODES$(BC%)
         IF BC$ <> STRING$(12, "0") THEN BEGIN
            RC% = CALC.BAR.CODE.CHECK.DIGIT( BC$ )
            IF RC% <> 0 THEN F06.CHECK.DIGIT$ = ""
            BC$ = BC$ + F06.CHECK.DIGIT$
         ENDIF
         BAR.CODES$(BC%) = FN.Z.PACK(BC$, 13)
      NEXT BC%

   RETURN

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

   LOG.TO.LDTAF.FILE:                                                 ! BMJK
                                                               ! BMJK
       CURRENT.KEY$ = ""                                          ! BMJK
       CURR.SESS.NUM% = LDTAF.SESS.NUM%                            ! BMJK
        IF LDTAF.OPEN.FLAG$ = "N" THEN BEGIN                               ! BMJK 
           IF END #LDTAF.SESS.NUM% THEN CREATE.LDTAF.FILE                  ! BMJK 
           OPEN LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM% APPEND                     ! BMJK
           LDTAF.OPEN.FLAG$ = "Y"                                          ! BMJK
        ENDIF                                                           ! BMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                    \ BMJK
         IF WRITE.LDTAF THEN GOTO WRITE.ERROR                            ! BMJK
       LDTAF.OPEN.FLAG$ = "N"                                          ! BMJK
       CLOSE LDTAF.SESS.NUM%                                          ! BMJK
                                                               ! BMJK
   RETURN                                                        ! BMJK
                                                               ! BMJK
   CREATE.LDTAF.FILE:                                                       ! BMJK
                                                               ! BMJK
       IF END #LDTAF.SESS.NUM% THEN CREATE.ERROR                     ! BMJK
       CREATE POSFILE LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM%              \ BMJK 
          MIRRORED ATCLOSE                                          ! BMJK
        LDTAF.OPEN.FLAG$ = "Y"                                             ! BMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                    \ BMJK
         IF WRITE.LDTAF THEN GOTO WRITE.ERROR                            ! BMJK
       LDTAF.OPEN.FLAG$ = "N"                                          ! BMJK
       CLOSE LDTAF.SESS.NUM%                                          ! BMJK
                                                               ! BMJK
   RETURN                                                        ! BMJK

\******************************************************************************
\***
\***   TRANSMIT.EPSOM.FILE:
\***
\***      send file transmission request to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.EPSOM.FILE:                     

      PIPE.OUT$ = "F"
      GOSUB SEND.TO.PSS38

      SB.MESSAGE$ = "PDT Support - Waiting for EPSOM E.O.T"             
      GOSUB SB.BG.MESSAGE 
         
   RETURN

\******************************************************************************
\***
\***   INTERROGATE.PSS38:
\***
\***      determine current state of PSS38
\***
\***   RETURN
\***
\******************************************************************************

   INTERROGATE.PSS38:
   
      SAVED.PIPE.OUT$ = PIPE.OUT$
      PIPE.OUT$ = "I"
      GOSUB SEND.TO.PSS38
      IF END# PIPEO.SESS.NUM% THEN READ.ERROR
      CURR.SESS.NUM% = PIPEO.SESS.NUM%
      READ# PIPEO.SESS.NUM%; PSS38.STATUS$
      PIPE.OUT$ = SAVED.PIPE.OUT$

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
      IF SB.ACTION$ = "R" OR SB.ACTION$ = "D" THEN BEGIN               ! DNS
         SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
         SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +                  \ DNS
                    CHR$(SHIFT(SB.FILE.REP.NUM%,0))                    ! DNS
      ENDIF                                                            ! DNS
                    


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
      SB.EVENT.NO% = 106                                          ! BMJK
      SB.UNIQUE$ = "O" + SB.ERRF$ + PACK$(STRING$(16,"0"))             ! DNS
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
      
      SB.UNIQUE$ = "R" +                                          \
                   SB.ERRF$ +                                          \ DNS
                   FN.Z.PACK(CURRENT.KEY$,7)                           ! DNS        
      
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
      SB.EVENT.NO% = 106                                          ! BMJK
      SB.UNIQUE$ = "W" +                                                \
                   SB.ERRF$ +                                          \ DNS
                  FN.Z.PACK(CURRENT.KEY$,8)
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
      SB.EVENT.NO% = 106                                          ! BMJK
      SB.UNIQUE$ = "C" + SB.ERRF$                                      ! DNS
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
      
      IF TEMP.STATE$ = "H" THEN GOTO EPSOM.LIST.REQUEST.CONTINUE       
      IF TEMP.STATE$ = "C" THEN GOTO EPSOM.FILE.HEADER.CONTINUE        
      
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

      IF TEMP.STATE$ = "H" THEN BEGIN
         IF CURR.SESS.NUM% = PIITM.SESS.NUM% THEN BEGIN
            GOTO GET.EPSOM.ITEM.INFO.CONTINUE                          
         ENDIF
      ENDIF

      IF CURR.SESS.NUM% = STKMQ.SESS.NUM% THEN BEGIN
         IF TEMP.STATE$ = "E" THEN GOTO EPSOM.LIST.ITEM.CONTINUE       
      ENDIF


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
   
         IF TEMP.STATE$ = "H" THEN BEGIN
         IF CURR.SESS.NUM% = PDTWF.SESS.NUM% THEN BEGIN
            IF NESTING.LEVEL% = 0 THEN GOTO CREATE.EPSOM.XM.FILE.CONTINUE 
            IF NESTING.LEVEL% = 1 THEN GOTO BACK.OUT.EPSOM.LIST.LOOP   
            IF NESTING.LEVEL% = 2 THEN GOTO BACK.OUT.EPSOM.ITEM.LOOP   
         ENDIF
         IF CURR.SESS.NUM% = PIITM.SESS.NUM%                            \
         OR CURR.SESS.NUM% = STKMQ.SESS.NUM% THEN BEGIN
            GOTO GET.EPSOM.ITEM.INFO.CONTINUE                          
         ENDIF
      ENDIF

      IF CURR.SESS.NUM% = STKMQ.SESS.NUM% THEN BEGIN
         IF TEMP.STATE$ = "E" THEN GOTO EPSOM.LIST.ITEM.CONTINUE       
         IF TEMP.STATE$ = "F" THEN GOTO EPSOM.LIST.TRAILER.CONTINUE    
      ENDIF

   
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
   
         IF CURR.SESS.NUM% = PDTWF.SESS.NUM% THEN BEGIN
         IF TEMP.STATE$ = "H" THEN GOTO EPSOM.LIST.REQUEST.CONTINUE     
      ENDIF
   
   GOTO MODULE.EXIT
   

\******************************************************************************
\***
\***   NOT.ON.IDF:
\***
\***      the item was not found on the IDF so log a read error
\***
\***   GOTO NO.IDF.REC.RETURN1:
\***
\******************************************************************************

   NOT.ON.IDF:

      ITEM.ON.IDF.FLAG$ = "N"
      GOSUB SB.FILE.READ.ERROR

   GOTO NO.IDF.REC.RETURN1

\******************************************************************************
\***
\***   IRF.ERROR:
\***
\***      the item was either not found on the IRF or the item code on the
\***      IDF did not match the one or the IRF so and log an error
\***
\***   GOTO IRF.ERROR.RETURN
\***
\******************************************************************************

   IRF.ERROR:

      SB.EVENT.NO% = 8
      SB.UNIQUE$ = IDF.BOOTS.CODE$                                      
      GOSUB SB.LOG.AN.EVENT

   GOTO IRF.ERROR.RETURN

\******************************************************************************
\***
\***   PILST.READ.FAIL:
\***
\***      end of PILST file encountered so set the E.O.F flag
\***
\***   GOTO RETURN.PILST.EOF
\***
\******************************************************************************

   PILST.READ.FAIL:                                             !!

      PILST.EOF.FLAG$ = "Y"                                     !!

   GOTO RETURN.PILST.EOF                                        !!

\******************************************************************************
\***
\***   BC.NOT.FOUND:
\***
\***      An invalid business centre code was encountered so set up the
\***      return data to indicate this
\***
\***   GOTO RETURN.TO.BC.CHECK
\***
\******************************************************************************

   BC.NOT.FOUND:

      SORT.ORDER$ = "C"                                                 
      SORT.DATE$ = "850101"                                            ! FNS
      PILST.LIST.NUMBER$ = BCSMF.FSI$ + "   "
      PILST.LIST.NAME$ = "Invalid Letter"
      PILST.ITEMS.IN.LIST% = 0                                          
      PILST.LIST.STATUS$ = "?"
      PILST.LIST.TYPE$ = "?"
      GOSUB STORE.EPSOM.LIST.INFO                                       

   GOTO RETURN.TO.BC.CHECK


\*****************************************************************************
\***   REDIRECT:
\***
\*****************************************************************************

   REDIRECT:

      RECEIVE.STATE$ = "*"
      ON RES.POS% GOTO EPSOM.FILE.HEADER.CONTINUE,                      \ 
                       EPSOM.LIST.ITEM.CONTINUE,                        \ 
                       EPSOM.LIST.TRAILER.CONTINUE,                     \ 
                       EPSOM.FILE.TRAILER.CONTINUE                      

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
 

   
   ERROR.COUNT% = ERROR.COUNT% + 1
   IF ERROR.COUNT% > 1 THEN RESUME PROGRAM.EXIT

   GOSUB SB.FORMAT.ERROR.DATA

   IF (ERRN AND 0000FFFFh) = 0000400Ch THEN BEGIN
      ERROR.COUNT% = ERROR.COUNT% - 1
        REP% = SB.FILE.REP.NUM%                                        ! DNS
       IF REP% = STKMQ.REPORT.NUM% THEN BEGIN                          ! ASMG
         IF TOTAL.WAIT.TIME% < 600 THEN BEGIN                          ! ASMG
            WAIT;2000                                                  ! ASMG
            TOTAL.WAIT.TIME% = TOTAL.WAIT.TIME% + 2                    ! ASMG
            RESUME RETRY                                               ! ASMG
         ENDIF ELSE BEGIN                                              ! ASMG
            SB.EVENT.NO% = 49                                          ! ASMG
            GOSUB SB.LOG.AN.EVENT                                      ! ASMG
            RESUME PROGRAM.EXIT                                        ! ASMG
         ENDIF                                                         ! ASMG
       ENDIF                                                           ! ASMG
   ENDIF

   IF ERRN = 80F3400CH AND SB.FILE.REP.NUM% = PDTWF.REPORT.NUM% THEN BEGIN ! DNS
      ERROR.COUNT% = ERROR.COUNT% - 1                                  ! DNS
      RECEIVE.STATE$ = "*"                                             ! DNS
      RESUME MODULE.EXIT                                               ! DNS
   ENDIF                                                               ! DNS

    IF ERR = "CU" OR ERR = "OE" OR ERR = "DF" THEN BEGIN               ! DNS
       ERROR.COUNT% = ERROR.COUNT% - 1                                 ! DNS
       RESUME                                                          ! DNS
    ENDIF                                                              ! DNS

   
   IF ERR = "OM" THEN BEGIN
      SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                             \ BMJK
                       PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))              ! BMJK
      CALL ADXERROR(0, 74, 0, 3, 101, SB.UNIQUE$ )
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME PROGRAM.EXIT
   ENDIF

   IF ERR = "IH" AND ERRN = 00000094h THEN BEGIN
      TEMP.STATE$ = RECEIVE.STATE$
      RES.POS% = MATCH(TEMP.STATE$, "CEFG", 1)
      IF RES.POS% = 0 THEN BEGIN
         RESUME PROGRAM.EXIT
      ENDIF ELSE BEGIN
         RESUME REDIRECT
      ENDIF
   ENDIF

   SB.EVENT.NO% = 101                                                  ! DNS
   SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                                  \ DNS
                 PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))              ! BMJK
   GOSUB SB.LOG.AN.EVENT       
   
   ERROR.COUNT% = ERROR.COUNT% - 1
   RECEIVE.STATE$ = "*"
   RESUME MODULE.EXIT

END SUB
