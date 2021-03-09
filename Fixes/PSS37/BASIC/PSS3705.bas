\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3705                                          ***
\***                                                                        ***
\***           AUTHOR   :  Brian Greenfield                                 ***
\***                                                                        ***
\***           DATE     :  17th July 2003                                   ***
\***                                                                        ***
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
\***   the new store stock counts.                                          ***
\***   The functional logic of this code was obtained from the PSS82 screen ***
\***   program with the screen handling and messaging display stripped out. ***
\***                                                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   A M E N D M E N T S                                                  ***
\***                                                                        ***
\***   Version A.   Initial Version                   24th July 2003        ***
\***                                                                        ***
\***   Version 1.1  Julia Stones                      22nd October 2003     ***
\***   This module has been updated to contain the bulk of the processing   ***
\***   to allow SmartScript pharmacy stock to be counted, and those counts  ***
\***   to be available in a file ready for transfer to the new SmartScript  ***
\***   pharmacy system.                                                     ***
\***   Upon successfull completion the program will ADXSTART PSD45 which    ***
\***   transfer the file to the pharmacy system.                            ***
\***   A confirmation report will be sent directly to the printer, showing  ***
\***   Store number, Stock takers number, Date, Time, Count area and number ***
\***   of items counted                                                     ***
\***                                                                        ***
\***   Version 1.2 Julia Stones                      17th December 2003     ***
\***   A small change to the confirmation report - to print what type of    ***
\***   count (Book in or Count)and for what area (MDS or Dispensing)        ***
\***                                                                        ***
\***   Version 1.3 Julia Stones                      22nd January 2004      ***
\***   The output files BTCS need to have a carriage return line feed after ***
\***   each record.  Program changed to add carriage return line feed to    ***
\***   each record on the BTCS files                                        ***
\***                                                                        ***
\***   Version 1.4 Charles Skadorwa                  3rd February 2005      ***
\***   Change to incorporate new Rectification Process logic in             ***
\***   Upwards TSF Adjustments in subroutine:  CHECK.STATUS.COUNT           ***
\***   Fix to Stock Counting PDT Timeout - processing now performed by      ***
\***   another program to prevent PDT timing out due to waiting for         ***
\***   processing to complete before receiving End Of Transmission.         ***
\***                                                                        ***
\***   Version 1.5 Brian Greenfield                  11th may 2007          ***
\***   Added new Recalls functions for A7C.                                 ***
\***                                                                        ***
\***   Version 1.6   Brian Greenfield                15th April 2008        ***
\***   Added Reverse Logistics functionality for A8C (part of Recalls.)     ***
\***                                                                        ***
\***   Version 1.7   Brian Greenfield                6th May 2008           ***
\***   Change request to pass MRQ to the PDT for it to display for type     ***
\***   I & C recalls.                                                       ***
\***                                                                        ***
\***   Version 1.8 Charles Skadorwa                  18th June 2008         ***
\***   Change to expiry date check from >= to > so that recall is still     ***
\***   visible on the day of expiry.                                        ***
\***                                                                        ***
\***   Version 1.9   Stuart Highley                     14 Nov 2008         ***
\***   Positive UOD Receiving.                                              ***
\***                                                                        ***
\***   Version 1.10  Stuart Highley                     11 Mar 2009         ***
\***   CR10: BOL popup on PDT for +UOD.                                     ***
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

    %INCLUDE PSBF06G.J86                                            ! 1.5BG
    %INCLUDE PSS37G.J86
    %INCLUDE PSBF06E.J86                                            ! 1.5BG
    %INCLUDE CMPDATE.J86

! Added function to ADXSTART PSD45 - to transfer Smartscript count file to pharmacy system.  ! 1.1JAS

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL                   ! 1.1JAS
      INTEGER*2 ADXSTART                                             ! 1.1JAS
      STRING    NAME$, PARM$, MESS$                                  ! 1.1JAS
   END FUNCTION                                                      ! 1.1JAS

FUNCTION FN.REMOVE.ZEROS$(INPUT.TEXT$)

    ! STRIP LEADING ZEROS OF TEXT PASSED IN STRING

    STRING INPUT.TEXT$
    STRING FN.REMOVE.ZEROS$
    INTEGER*2 COUNT%

    COUNT% = 1
    WHILE MID$(INPUT.TEXT$, COUNT%, 1) = "0"
        COUNT% = COUNT% + 1
    WEND

    FN.REMOVE.ZEROS$ = MID$(INPUT.TEXT$, COUNT%, 32767)
    INPUT.TEXT$ = ""

END FUNCTION

!Find the value of a string without risking a runtime error             ! 1.9SH
!Returns 0 if an error occurs                                           ! 1.9SH
    FUNCTION VALUE%(TXT$)                                               ! 1.9SH
        STRING TXT$                                                     ! 1.9SH
        INTEGER*4 VALUE%                                                ! 1.9SH
        ON ERROR GOTO ERROR.DETECTED                                    ! 1.9SH
        VALUE% = VAL(TXT$)                                              ! 1.9SH
ERROR.RESUME:                                                           ! 1.9SH
        TXT$ = ""                                                       ! 1.9SH
        EXIT FUNCTION                                                   ! 1.9SH
ERROR.DETECTED:                                                         ! 1.9SH
        VALUE% = 0                                                      ! 1.9SH
        RESUME ERROR.RESUME                                             ! 1.9SH
    END FUNCTION                                                        ! 1.9SH

FUNCTION PADNUM$(NUM%, LEN%)                                            ! 1.9SH
    STRING PADNUM$                                                      ! 1.9SH
    INTEGER*4 NUM%                                                      ! 1.9SH
    INTEGER*2 LEN%                                                      ! 1.9SH
    IF NUM% < 0 THEN BEGIN                                              ! 1.9SH
        PADNUM$ = "-" + RIGHT$(STRING$(LEN%-1, "0") + STR$(ABS(NUM%)), LEN%-1) ! 1.9SH
    ENDIF ELSE BEGIN                                                    ! 1.9SH
        PADNUM$ = RIGHT$(STRING$(LEN%, "0") + STR$(NUM%), LEN%)         ! 1.9SH
    ENDIF                                                               ! 1.9SH
END FUNCTION                                                            ! 1.9SH

FUNCTION PADNUMSTR$(TXT$, LEN%) RECURSIVE                               ! 1.9SH
    STRING PADNUMSTR$, TXT$                                             ! 1.9SH
    INTEGER*2 LEN%                                                      ! 1.9SH
    IF LEFT$(TXT$, 1) = "-" THEN BEGIN                                  ! 1.9SH
        PADNUMSTR$ = "-" + PADNUMSTR$(MID$(TXT$, 2, 32767), LEN% - 1)   ! 1.9SH
    ENDIF ELSE BEGIN                                                    ! 1.9SH
        PADNUMSTR$ = RIGHT$(STRING$(LEN%, "0") + TXT$, LEN%)            ! 1.9SH
    ENDIF                                                               ! 1.9SH
    TXT$ = ""                                                           ! 1.9SH
END FUNCTION                                                            ! 1.9SH

\**********************************************************************
\***  Increment suffix
\**********************************************************************
SUB INCREMENT.SUFFIX(SFX$)                                              ! 1.9SH
    STRING SFX$                                                         ! 1.9SH
    SFX$ = PADNUM$(VALUE%(LEFT$(SFX$,6)) + 1, 6) + MID$(SFX$, 7, 32767) ! 1.9SH
END SUB                                                                 ! 1.9SH


    SUB PSS3705 PUBLIC

STRING BAR.CODE$
STRING BOOTS.CODE$
STRING BUFFER.FILE$                                                  ! 1.4CS
STRING BUFFER.RECORD$                                                ! 1.4CS
STRING COUNT$
STRING COUNT.DAY$
STRING COUNT.MONTH$
STRING COUNT.QTY$
STRING COUNT.YEAR$
STRING CURRENT.STOCK$
STRING PDT.CODE$(1)
STRING SB.ACTION$
STRING SB.ERRF$
STRING SB.ERRL$
STRING SB.ERRS$
STRING SB.MESSAGE$
STRING SB.UNIQUE$
STRING SB.STRING$
STRING STOCK.OPEN.FLAG$
STRING STORED.CODE$(1)
STRING TEMP.CODE$
STRING VAR.STRING.1$
STRING VAR.STRING.2$
STRING SSPSCTRL.OPEN.FLAG$                                           ! 1.1JAS
STRING BTCS.OPEN.FLAG$                                               ! 1.1JAS
STRING PH.STORE.NUM$                                                 ! 1.1JAS
STRING PH.STKTAKE.NUM$                                               ! 1.1JAS
STRING PH.DATE$                                                      ! 1.1JAS
STRING PH.TIME$                                                      ! 1.1JAS
STRING PH.DISP.AREA$                                                 ! 1.1JAS
STRING PH.COUNT.TYPE$                                                ! 1.1JAS
STRING PH.DETAIL.DATA$(1)                                            ! 1.1JAS
STRING PRINTER.STATUS$                                               ! 1.1JAS
STRING BLANK.LINE$                                                   ! 1.1JAS
STRING FOOTER.LINE$                                                  ! 1.1JAS
STRING PAGE.THROW$                                                   ! 1.1JAS
STRING BUFFER.OPEN.FLAG$                                             ! 1.5BG
STRING PACKED.ITEM.CODE$                                             ! 1.5BG
STRING UOD$(1)                                                       ! 1.9SH
STRING BATCH.SIZE$                                                   ! 1.9SH
STRING FILE.SUFFIX$                                                  ! 1.9SH
STRING NEW.SUFFIX$                                                   ! 1.9SH
STRING UB.OPEN.FLAG$                                                 ! 1.9SH
STRING STATUS$                                                       ! 1.9SH
STRING BOL$                                                          ! 1.10SH

INTEGER*4 ADX.RET.CODE%
INTEGER*4 FILE.RETURN.CODE%
INTEGER*4 STORED.COUNT%(1)
INTEGER*4 RECALL.RECS%                                               ! 1.5BG
INTEGER*4 LAST.RECALL.REC%                                           ! 1.5BG

INTEGER*2 BUFFER.REPORT.NUM%                                         ! 1.4CS
INTEGER*2 COUNT.LOOP%
INTEGER*2 COUNT.NUMBER%
INTEGER*2 MESSAGE.NO%
INTEGER*2 PDT.STOCK%(1)
INTEGER*2 RETURN.CODE%
INTEGER*2 REP%
INTEGER*2 SB.FILE.REP.NUM%
INTEGER*2 SB.INTEGER%
INTEGER*2 STOCK.COUNT%
INTEGER*2 TABLE.LOOP%
INTEGER*2 VALID.ITEMS%
INTEGER*2 RECALLS.RECORD.COUNT%                                      ! 1.5BG
INTEGER*2 CURRENT.RECALL.REC%                                        ! 1.5BG
INTEGER*2 REWKF.ITEM.COUNT%                                          ! 1.5BG
INTEGER*2 ITEMS.THIS.RECORD%                                         ! 1.5BG
INTEGER*2 MAX.RECALLS.ITEMS%                                         ! 1.5BG
INTEGER*2 ITEM.IN.PROGRESS%                                          ! 1.5BG
INTEGER*2 NUM.UODS%                                                  ! 1.9SH
INTEGER*2 NUM.RECS%                                                  ! 1.9SH
INTEGER*2 RETRIES%                                                   ! 1.9SH

INTEGER*1 BUFFER.SESS.NUM%                                           ! 1.4CS
INTEGER*1 CURR.SESS.NUM%
INTEGER*1 COUNT.IN.PROGRESS%
INTEGER*1 EVENT.NO%
INTEGER*1 FALSE
INTEGER*1 MASK%
INTEGER*1 SB.EVENT.NO%
INTEGER*1 SB.FILE.SESS.NUM%
INTEGER*1 STOCK.FOUND%
INTEGER*1 TRUE
INTEGER*1 VALID.CODE%
INTEGER*1 VALID.HEADER%
INTEGER*1 VALID.TRAILER%
INTEGER*1 PRINTER.ERROR                                              ! 1.1JAS
INTEGER*1 PRINTER.OPEN                                               ! 1.1JAS
INTEGER*1 PRINTING.COMPLETE%                                         ! 1.1JAS
INTEGER*1 AUTHORISED                                                 ! 1.9SH
INTEGER*1 ONIGHT                                                     ! 1.9SH

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   M A I N L I N E   C O D E                                            ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

ON ERROR GOTO ERROR.DETECTED

    MODULE.NUMBER$ = "PSS3705"                                       ! 1.9SH   

    TRUE = -1                                                        ! 1.9SH
    FALSE = 0                                                        ! 1.9SH

! Check to see if either Stock count or SmartScript pharmacy count   ! 1.1JAS

    IF RECEIVE.STATE$ = "s" OR RECEIVE.STATE$ = "w" THEN BEGIN       ! 1.1JAS
       GOSUB INITIALISE
       COUNT.IN.PROGRESS% = TRUE                                     
       GOSUB OPEN.FILES
    ENDIF
    
    IF RECEIVE.STATE$ = "1" OR \                                       ! 1.5BG
       (RECEIVE.STATE$ = "3" AND MID$(DATA.IN$,25,1) = "Y") THEN BEGIN ! 1.5BG
       GOSUB INITIALISE                                                ! 1.5BG
    ENDIF                                                              ! 1.5BG
    
    !If recalls then go to the relevent subroutines - added extra    ! 1.5BG
    !because these states start at 1.                                ! 1.5BG
    IF MATCH(RECEIVE.STATE$,"123456789:;<=",1) > 0 THEN BEGIN        ! 1.9SH
       ON (ASC(RECEIVE.STATE$) - ASC("1") + 1) GOSUB \               ! 1.5BG
             RECEIVED.RECALLS.REQUEST,        \                      ! 1.5BG
             RECEIVED.RECALLS.RECEIVED.OK,    \                      ! 1.5BG
             RECEIVED.RECALLS.HEADER,         \                      ! 1.5BG
             RECEIVED.RECALLS.DETAIL,         \                      ! 1.5BG
             RECEIVED.RECALLS.TRAILER,        \                      ! 1.5BG
             RECEIVED.RECALLS.EOT,            \                      ! 1.9SH
             RECEIVED.PUOD.SIGN.ON,           \                      ! 1.9SH
             RECEIVED.PUOD.RECEIVED.OK,       \                      ! 1.9SH
             RECEIVED.PUOD.HEADER,            \                      ! 1.9SH
             RECEIVED.PUOD.DETAIL,            \                      ! 1.9SH
             RECEIVED.PUOD.BATCH.TRAILER,     \                      ! 1.9SH
             RECEIVED.PUOD.SESSION.TRAILER,   \                      ! 1.9SH
             RECEIVED.PUOD.EOT                                       ! 1.9SH       
    ENDIF                                                            ! 1.5BG

! Upon Receive State type - process the relevant record type         ! 1.1JAS

    IF COUNT.IN.PROGRESS% THEN BEGIN
       IF MATCH(RECEIVE.STATE$,"stuvwxyz",1) > 0 THEN BEGIN          ! 1.1JAS
          ON (ASC(RECEIVE.STATE$) - ASC("r")) GOSUB \
                RECEIVED.STOCKCOUNT.HEADER,         \
                RECEIVED.STOCKCOUNT.DETAIL,         \
                RECEIVED.STOCKCOUNT.TRAILER,        \
                RECEIVED.STOCKCOUNT.EOT,            \                ! 1.1JAS
                RECEIVED.SMARTSCRIPT.HEADER,        \                ! 1.1JAS
                RECEIVED.SMARTSCRIPT.DETAIL,        \                ! 1.1JAS
                RECEIVED.SMARTSCRIPT.TRAILER,       \                ! 1.1JAS
                RECEIVED.SMARTSCRIPT.EOT                             ! 1.1JAS
       ENDIF
    ENDIF

    GOTO PROGRAM.EXIT

MODULE.EXIT:

    COUNT.IN.PROGRESS% = FALSE
    GOSUB CLOSE.FILES
    RECEIVE.STATE$ = "*"

PROGRAM.EXIT:

! Trailer record for either Stock count or SmartScript count         ! 1.1JAS

    IF RECEIVE.STATE$ = "u" OR RECEIVE.STATE$ = "y" AND     \        ! 1.1JAS
       COUNT.IN.PROGRESS% THEN GOSUB CLOSE.FILES

! EOT record for either Stock count or SmartScript count             ! 1.1JAS

    IF RECEIVE.STATE$ = "v" OR RECEIVE.STATE$ = "z" AND     \        ! 1.1JAS
       COUNT.IN.PROGRESS% THEN BEGIN
       COUNT.IN.PROGRESS% = FALSE
       RE.CHAIN = TRUE
       RECEIVE.STATE$ = "?"
    ENDIF
    
    IF RECEIVE.STATE$ = "1" THEN BEGIN                               ! 1.5BG
      !Release PDT                                                   ! 1.5BG
      PIPE.OUT$ = "HN"                                               ! 1.5BG
      GOSUB SEND.TO.PSS38                                            ! 1.5BG
      HOLD.FLAG$ = "N"                                               ! 1.5BG
   ENDIF                                                             ! 1.5BG

   EXIT SUB

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
\***   INITIALISE
\***
\******************************************************************************

INITIALISE:

    IF RECEIVE.STATE$ = "s" OR RECEIVE.STATE$ = "w" THEN BEGIN          ! 1.5BG
    
       COUNT.QTY$ = ""
       CURRENT.STOCK$ = ""

      !Set up report lines for SmartScript confirmation report          ! 1.1JAS
       BLANK.LINE$ = STRING$(80," ")                                    ! 1.1JAS
       FOOTER.LINE$ = STRING$(15," ") +                               \ ! 1.1JAS
                      "* * *   E N D   O F   R E P O R T   * * *"       ! 1.1JAS
       PAGE.THROW$ = STRING$(79, CHR$(0)) + CHR$(12)                    ! 1.1JAS

       ! THESE DIMS ARE SET TO 501 BECAUSE THE PDT WILL NOT ALLOW MORE THAN 500 COUNTS.
       DIM PDT.CODE$(501)            
       DIM PDT.STOCK%(501)           
       DIM STORED.CODE$(501)         
       DIM STORED.COUNT%(501)        

       DIM PH.DETAIL.DATA$(3000)                                     ! 1.1JAS

       CALL SSPSCTRL.SET                                             ! 1.1JAS
       CALL BTCS.SET                                                 ! 1.1JAS
       CALL PRINT.SET                                                ! 1.1JAS
   
   ENDIF                                                             ! 1.5BG

RETURN


\******************************************************************************
\***
\***   RECEIVED.STOCKCOUNT.HEADER:                              STATE : s
\***
\******************************************************************************

RECEIVED.STOCKCOUNT.HEADER:

    SB.MESSAGE$ = "PDT Support - Stock count header received"
    GOSUB SB.BG.MESSAGE

    IF FN.VALIDATE.DATA(DATA.IN$, 46) = 0 THEN BEGIN
       RECEIVE.STATE$ = "*"
       RETURN
    ENDIF

    PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"START OF STOCK COUNTS"

    VALID.HEADER% = TRUE
    COUNT.NUMBER% = 0
    IF MID$(DATA.IN$, 3, 17) <> "00000000000000000" THEN VALID.HEADER% = FALSE

RETURN

\******************************************************************************
\***
\***   RECEIVED.STOCKCOUNT.DETAIL:                              STATE : t
\***
\******************************************************************************

RECEIVED.STOCKCOUNT.DETAIL:

    SB.MESSAGE$ = "PDT Support - Stock count detail received"
    GOSUB SB.BG.MESSAGE

    IF FN.VALIDATE.DATA(DATA.IN$, 47) = 0 THEN BEGIN
       RECEIVE.STATE$ = "*"
       RETURN
    ENDIF

    IF VALID.HEADER% THEN BEGIN
       COUNT.NUMBER% = COUNT.NUMBER% + 1
       PDT.CODE$(COUNT.NUMBER%) = MID$(DATA.IN$, 3, 13)
       PDT.STOCK%(COUNT.NUMBER%) = VAL(MID$(DATA.IN$, 16, 4))
    ENDIF

RETURN


\******************************************************************************
\***
\***   RECEIVED.STOCKCOUNT.TRAILER:                               STATE : u
\***
\******************************************************************************

RECEIVED.STOCKCOUNT.TRAILER:

    SB.MESSAGE$ = "PDT Support - Stock count trailer received"
    GOSUB SB.BG.MESSAGE

    IF FN.VALIDATE.DATA(DATA.IN$, 48) = 0 THEN BEGIN
       RECEIVE.STATE$ = "*"
       RETURN
    ENDIF

    !RESPOND TO PDT
    PIPE.OUT$ = "L" + DATA.IN$
    IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR
    CURRENT.REPORT.NUM% = PIPEI.REPORT.NUM%                                 !1.9SH
    WRITE# PIPEI.SESS.NUM%; PIPE.OUT$

    IF VALID.HEADER% THEN BEGIN

       VALID.TRAILER% = TRUE
       IF MID$(DATA.IN$, 3, 13) <> "9999999999999" THEN VALID.TRAILER% = FALSE

       !CHECK THAT RECORD COUNT MATCHES
       IF VAL(MID$(DATA.IN$, 16, 4)) <> COUNT.NUMBER% THEN VALID.TRAILER% = FALSE

       IF VALID.TRAILER% THEN BEGIN

          SB.MESSAGE$ = "PDT Support - Stock count trailer received"
          GOSUB SB.BG.MESSAGE
          
          VALID.ITEMS% = 0
          PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"VALIDATING " + STR$(COUNT.NUMBER%) + " STOCK COUNT ITEM(S) "

          ! Output counts to Stock Count Buffer File 
          CURRENT.REPORT.NUM% = BUFFER.REPORT.NUM%                                    !1.9SH
          IF END # BUFFER.SESS.NUM% THEN WRITE.ERROR                                  !1.4CS
                                                                                      !1.4CS
          FOR COUNT.LOOP% = 1 TO COUNT.NUMBER%                                        !1.4CS
                                                                                      
             BUFFER.RECORD$ = PDT.CODE$(COUNT.LOOP%) + \                              !1.4CS
                              RIGHT$("0000" + STR$(PDT.STOCK%(COUNT.LOOP%)), 4)       !1.4CS
                              
             WRITE #BUFFER.SESS.NUM%; BUFFER.RECORD$                                  !1.4CS
                                                                                      !1.4CS
          NEXT                                                                        !1.4CS

!          Redundant as LSS does not use this report now                               !1.4CS
!          ADX.RET.CODE% = ADXSTART("ADX_UPGM:LSSST.286", \                     !1.3BG !1.4CS
!                          MONITORED.PORT$,             \                       !1.3BG !1.4CS
!                          "LSS Stock count report")                            !1.3BG !1.4CS
           ! Call new program to process the counts. This prevents the PDT from !1.4CS
           ! timing out when excessive counts are transmitted and processed     !1.4CS
           ADX.RET.CODE% = ADXSTART("ADX_UPGM:PDTSC.286", \                     !1.4CS
                           MONITORED.PORT$,               \                     !1.4CS
                           "PDT Stock Count Program")                           !1.4CS

          SB.MESSAGE$ = "PDT Support - ADXSTART ADX_UPGM:PDTSC.286"
          GOSUB SB.BG.MESSAGE
          PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"ADXSTART ADX_UPGM:PDTSC.286"
          
       ENDIF

    ENDIF

RETURN


\******************************************************************************
\***
\***   RECEIVED.STOCKCOUNT.EOT:                              STATE : v
\***
\******************************************************************************

RECEIVED.STOCKCOUNT.EOT:

    SB.MESSAGE$ = "PDT Support - Stock count EOT received"
    GOSUB SB.BG.MESSAGE

    IF FN.VALIDATE.DATA(DATA.IN$, 49) = 0 THEN BEGIN
       RECEIVE.STATE$ = "*"
       RETURN
    ENDIF

    PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"STOCK COUNTS COMPLETE"

RETURN

\******************************************************************************
\***
\***   RECEIVED.SMARTSCRIPT.HEADER:                              STATE : w
\***
\******************************************************************************

RECEIVED.SMARTSCRIPT.HEADER:                                         ! 1.1JAS

    SB.MESSAGE$ = "PDT Support - SmartScript header received"        ! 1.1JAS
    GOSUB SB.BG.MESSAGE                                              ! 1.1JAS

    IF FN.VALIDATE.DATA(DATA.IN$, 50) = 0 THEN BEGIN                 ! 1.1JAS
       RECEIVE.STATE$ = "*"                                          ! 1.1JAS
       RETURN                                                        ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"START OF SMARTSCRIPT COUNTS"  ! 1.1JAS

    VALID.HEADER% = TRUE                                             ! 1.1JAS
    COUNT.NUMBER% = 0                                                ! 1.1JAS
    !Validate that header record not set to all zero's               ! 1.1JAS
    IF MID$(DATA.IN$, 3, 22) EQ "0000000000000000000000" THEN VALID.HEADER% = FALSE  ! 1.1JAS

    !Validate header record length                                   ! 1.1JAS
    IF LEN(DATA.IN$) NE 24 THEN VALID.HEADER% = FALSE                ! 1.1JAS

    IF VALID.HEADER% THEN BEGIN                                      ! 1.1JAS
       PH.STORE.NUM$   = MID$(DATA.IN$, 3,  4)                       ! 1.1JAS
       PH.STKTAKE.NUM$ = MID$(DATA.IN$,  7,  4)                      ! 1.1JAS
       PH.DATE$        = MID$(DATA.IN$, 11,  6)                      ! 1.1JAS
       PH.TIME$        = MID$(DATA.IN$, 17,  6)                      ! 1.1JAS
       PH.DISP.AREA$   = MID$(DATA.IN$, 23,  1)                      ! 1.1JAS
       PH.COUNT.TYPE$  = MID$(DATA.IN$, 24,  1)                      ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    !Validate that count type = C for count or B for Book in         ! 1.1JAS
    IF PH.COUNT.TYPE$ <> "C" AND                 \                   ! 1.1JAS
       PH.COUNT.TYPE$ <> "B" THEN VALID.HEADER% = FALSE              ! 1.1JAS

RETURN                                                               ! 1.1JAS

\******************************************************************************
\***
\***   RECEIVED.SMARTSCRIPT.DETAIL:                              STATE : x
\***
\******************************************************************************

RECEIVED.SMARTSCRIPT.DETAIL:                                         ! 1.1JAS

    SB.MESSAGE$ = "PDT Support - SmartScript detail received"        ! 1.1JAS
    GOSUB SB.BG.MESSAGE                                              ! 1.1JAS

    IF FN.VALIDATE.DATA(DATA.IN$, 51) = 0 THEN BEGIN                 ! 1.1JAS
       RECEIVE.STATE$ = "*"                                          ! 1.1JAS
       RETURN                                                        ! 1.1JAS
    ENDIF

    !If header record valid process detail records                   ! 1.1JAS
    IF VALID.HEADER% THEN BEGIN                                      ! 1.1JAS
       COUNT.NUMBER% = COUNT.NUMBER% + 1                             ! 1.1JAS
       PH.DETAIL.DATA$(COUNT.NUMBER%) = MID$(DATA.IN$, 3, 24)        ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

RETURN                                                               ! 1.1JAS

\******************************************************************************
\***
\***   RECEIVED.SMARTSCRIPT.TRAILER:                               STATE : y
\***
\******************************************************************************

RECEIVED.SMARTSCRIPT.TRAILER:                                        ! 1.1JAS

    SB.MESSAGE$ = "PDT Support - SmartScript trailer received"       ! 1.1JAS
    GOSUB SB.BG.MESSAGE                                              ! 1.1JAS

    IF FN.VALIDATE.DATA(DATA.IN$, 52) = 0 THEN BEGIN                 ! 1.1JAS
       RECEIVE.STATE$ = "*"                                          ! 1.1JAS
       RETURN                                                        ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    !RESPOND TO PDT                                                  ! 1.1JAS
!    PIPE.OUT$ = "L" + DATA.IN$                                       ! 1.1JAS
!    IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR                         ! 1.1JAS
!    CURR.SESS.NUM% = PIPEI.SESS.NUM%                                 ! 1.1JAS
!    WRITE# PIPEI.SESS.NUM%; PIPE.OUT$                                ! 1.1JAS

    IF VALID.HEADER% THEN BEGIN                                      ! 1.1JAS
       !Validate that the trailer record count qty is not greater than 3000 ! 1.1JAS
       !and not less than 2                                          ! 1.1JAS
       VALID.TRAILER% = TRUE                                         ! 1.1JAS
       IF VAL(MID$(DATA.IN$, 3, 4)) < 2 OR    \                      ! 1.1JAS
          VAL(MID$(DATA.IN$,  3,  4)) > 3000 THEN VALID.TRAILER% = FALSE ! 1.1JAS

       !Check that the record count processed matches the value held in the trailer count field  ! 1.1JAS
       IF VAL(MID$(DATA.IN$, 3, 4)) <> COUNT.NUMBER% + 2 THEN VALID.TRAILER% = FALSE  ! 1.1JAS

       IF VALID.TRAILER% THEN BEGIN                                  ! 1.1JAS
          !Write count information to file                           ! 1.1JAS
          PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"WRITING " +         \ 1.1JAS
                      STR$(COUNT.NUMBER%) + " SMARTSCRIPT COUNT ITEM(S) "  ! 1.1JAS
          FOR COUNT.LOOP% = 1 TO COUNT.NUMBER%                       ! 1.1JAS

             IF COUNT.LOOP% = 1 THEN BEGIN                           ! 1.1JAS
                GOSUB WRITE.SMARTSCRIPT.HEADER                       ! 1.1JAS
             ENDIF                                                   ! 1.1JAS

             GOSUB WRITE.SMARTSCRIPT.DETAIL                          ! 1.1JAS

          NEXT                                                       ! 1.1JAS

          GOSUB WRITE.SMARTSCRIPT.TRAILER                            ! 1.1JAS

       ENDIF                                                         ! 1.1JAS

    ENDIF                                                            ! 1.1JAS

    !RESPOND TO PDT                                                  ! 1.1JAS
    PIPE.OUT$ = "L" + DATA.IN$                                       ! 1.1JAS
    IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR                         ! 1.1JAS
    CURRENT.REPORT.NUM% = PIPEI.REPORT.NUM%                          ! 1.9SH
    WRITE# PIPEI.SESS.NUM%; PIPE.OUT$                                ! 1.1JAS

RETURN                                                               ! 1.1JAS

\******************************************************************************
\***
\***   RECEIVED.SMARTSCRIPT.EOT:                              STATE : z
\***
\******************************************************************************

RECEIVED.SMARTSCRIPT.EOT:                                            ! 1.1JAS

    SB.MESSAGE$ = "PDT Support - SmartScript EOT received"           ! 1.1JAS
    GOSUB SB.BG.MESSAGE                                              ! 1.1JAS

    IF FN.VALIDATE.DATA(DATA.IN$, 53) = 0 THEN BEGIN                 ! 1.1JAS
       RECEIVE.STATE$ = "*"                                          ! 1.1JAS
       RETURN                                                        ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"SMARTSCRIPT COUNTS COMPLETE" ! 1.1JAS

RETURN                                                               ! 1.1JAS

\*****************************************************************************
\***
\***  WRITE.SMARTSCRIPT.HEADER
\***
\***  Open Pharmacy SmartScrip system control file (SSPSCTRL)
\***  If read is successful check what type of count was passed from the PDT
\***  If type = "C" (count) then find last extension number for BTCSK
\***  If type = "B" (book in) then find last extension number for BTCSF
\***  If last extension number for either file was set to "999" the next number
\***  to use will be "001" else add + 1 to give next extension number.
\***  Add the rest of the file name to BTCS (either BTCSK.???, or BTCSF.???
\***  where ??? = extension number
\***  Create the BTCS?.??? output file.
\***  Write header record to BTCS?.???
\***
\*****************************************************************************

WRITE.SMARTSCRIPT.HEADER:                                            ! 1.1JAS

    !Open control file                                               ! 1.1JAS
    SB.INTEGER% = SSPSCTRL.REPORT.NUM%                               ! 1.1JAS
    IF END# SSPSCTRL.SESS.NUM% THEN FILE.NOT.FOUND                   ! 1.1JAS
    OPEN SSPSCTRL.FILE.NAME$ DIRECT RECL SSPSCTRL.RECL%  \           ! 1.1JAS
         AS SSPSCTRL.SESS.NUM% LOCKED NODEL                          ! 1.1JAS

    SSPSCTRL.OPEN.FLAG$ = "Y"                                        ! 1.1JAS

    !Read control file to get last Extension number                  ! 1.1JAS
    FILE.RETURN.CODE% = READ.SSPSCTRL                                ! 1.1JAS

    IF FILE.RETURN.CODE% NE 0 THEN BEGIN                             ! 1.1JAS
       GOSUB READ.SSPSCTRL.ERROR                                     ! 1.1JAS
       RETURN                                                        ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    !If type = C find last BTCSK ext num, if set to 999 then new ext ! 1.1JAS
    !number = 001 else add + 1 to last number.  File name = BTCSK +  ! 1.1JAS
    ! ext number                                                     ! 1.1JAS
    !If type = B find last BTCSF ext num, if set to 999 then new ext ! 1.1JAS
    !number = 001, else add + 1 to last number.  File name = BTCSF + ! 1.1JAS
    ! ext number                                                     ! 1.1JAS
    IF PH.COUNT.TYPE$ = "C" THEN BEGIN                               ! 1.1JAS
       IF SSPS.BTCSK.NUM$ = "999" THEN BEGIN                         ! 1.1JAS
          SSPS.BTCSK.NUM$ = "001"                                    ! 1.1JAS
       ENDIF ELSE BEGIN                                              ! 1.1JAS
          SSPS.BTCSK.NUM$ = RIGHT$("000" + STR$(VAL(SSPS.BTCSK.NUM$) +1),3)  ! 1.1JAS
       ENDIF
       BTCS.FILE.NAME$ = BTCS.FILE.NAME$ + "K." + SSPS.BTCSK.NUM$    ! 1.1JAS
    ENDIF ELSE BEGIN                                                 ! 1.1JAS
       IF SSPS.BTCSF.NUM$ = "999" THEN BEGIN                         ! 1.1JAS
          SSPS.BTCSF.NUM$ = "001"                                    ! 1.1JAS
       ENDIF ELSE BEGIN                                              ! 1.1JAS
          SSPS.BTCSF.NUM$ = RIGHT$("000" + STR$(VAL(SSPS.BTCSF.NUM$) +1),3)  ! 1.1JAS
       ENDIF
       BTCS.FILE.NAME$ = BTCS.FILE.NAME$ + "F." + SSPS.BTCSF.NUM$    ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    SB.INTEGER% = BTCS.REPORT.NUM%                                   ! 1.1JAS

    !Create BTCS file
    IF END #BTCS.SESS.NUM% THEN CREATE.ERROR                         ! 1.1JAS
    CREATE POSFILE BTCS.FILE.NAME$ DIRECT 0 RECL BTCS.RECL%       \  ! 1.1JAS
           AS BTCS.SESS.NUM% LOCKED MIRRORED ATCLOSE                 !1.1JAS

    BTCS.OPEN.FLAG$ = "Y"                                            ! 1.1JAS

    BTCS.REC.NUM% = BTCS.REC.NUM% + 1                                ! 1.1JAS

    !Write BTCS file header record                                   ! 1.1JAS

    BTCS.STORE.NUMBER$ = PH.STORE.NUM$                               ! 1.1JAS
    BTCS.STKTAKE.NUM$  = PH.STKTAKE.NUM$                             ! 1.1JAS
    BTCS.DATE$         = PH.DATE$                                    ! 1.1JAS
    BTCS.TIME$         = PH.TIME$                                    ! 1.1JAS
    BTCS.DISP.AREA$    = PH.DISP.AREA$                               ! 1.1JAS
    BTCS.FILLER$       = STRING$(14, " ")                            ! 1.1JAS
    BTCS.RECORD.TYPE$  = "H"                                         ! 1.1JAS
    BTCS.NUM.RECORD$   = "0001"                                      ! 1.1JAS
    BTCS.ENDREC$       = CHR$(13) + CHR$(10)                         ! 1.3JAS

    FILE.RETURN.CODE% = WRITE.BTCS                                   ! 1.1JAS

    IF FILE.RETURN.CODE% NE 0 THEN BEGIN                             ! 1.1JAS
       GOSUB WRITE.ERROR                                             ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    BTCS.REC.NUM% = BTCS.REC.NUM% + 1                                ! 1.1JAS

RETURN                                                               ! 1.1JAS

\*****************************************************************************
\***
\*** WRITE.SMARTSCRIPT.DETAIL:
\***
\*****************************************************************************

WRITE.SMARTSCRIPT.DETAIL:                                            ! 1.1JAS

    BTCS.ITEM.CODE$     = LEFT$(PH.DETAIL.DATA$(COUNT.LOOP%), 13)    ! 1.1JAS
    BTCS.CODE.TYPE$     = MID$(PH.DETAIL.DATA$(COUNT.LOOP%), 14,  1) ! 1.1JAS
    BTCS.PACK.QTY$      = MID$(PH.DETAIL.DATA$(COUNT.LOOP%), 15,  6) ! 1.1JAS
    BTCS.DIS.UNIT.QTY$  = MID$(PH.DETAIL.DATA$(COUNT.LOOP%), 21,  4) ! 1.1JAS
    BTCS.FILLER$        = STRING$(11, " ")                           ! 1.1JAS
    BTCS.RECORD.TYPE$   = "D"                                        ! 1.1JAS
    BTCS.NUM.RECORD$    = RIGHT$("0000" + STR$(COUNT.LOOP% +1),4)    ! 1.1JAS
    BTCS.ENDREC$       = CHR$(13) + CHR$(10)                         ! 1.3JAS

    FILE.RETURN.CODE% = WRITE.BTCS                                   ! 1.1JAS

    IF FILE.RETURN.CODE% NE 0 THEN BEGIN                             ! 1.1JAS
       GOSUB WRITE.ERROR                                             ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    BTCS.REC.NUM% = BTCS.REC.NUM% + 1                                ! 1.1JAS

RETURN                                                               ! 1.1JAS

\*****************************************************************************
\***
\*** WRITE.SMARTSCRIPT.TRAILER
\***
\*****************************************************************************

WRITE.SMARTSCRIPT.TRAILER:                                           ! 1.1JAS

    BTCS.RECORD.COUNT$  = RIGHT$("0000" + STR$(COUNT.LOOP% +1),4)    ! 1.1JAS
    BTCS.FILLER$        = STRING$(31, " ")                           ! 1.1JAS
    BTCS.RECORD.TYPE$   = "T"                                        ! 1.1JAS
    BTCS.NUM.RECORD$    = RIGHT$("0000" + STR$(COUNT.LOOP% +1),4)    ! 1.1JAS
    BTCS.ENDREC$       = CHR$(13) + CHR$(10)                         ! 1.3JAS

    FILE.RETURN.CODE% = WRITE.BTCS                                   ! 1.1JAS

    IF FILE.RETURN.CODE% NE 0 THEN BEGIN                             ! 1.1JAS
       GOSUB WRITE.ERROR                                             ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    BTCS.REC.NUM% = BTCS.REC.NUM% + 1                                ! 1.1JAS

    SB.INTEGER% = SSPSCTRL.REPORT.NUM%                               ! 1.1JAS

    FILE.RETURN.CODE% = WRITE.SSPSCTRL                               ! 1.1JAS

    IF FILE.RETURN.CODE% NE 0 THEN BEGIN                             ! 1.1JAS
       GOSUB WRITE.ERROR                                             ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    GOSUB WRITE.SMARTSCRIPT.REPORT                                   ! 1.1JAS

    !ADXSTART PSD45 to transfer file to pharmacy system              ! 1.1JAS

    ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSD45.286",              \    ! 1.1JAS
                    MONITORED.PORT$,                            \    ! 1.1JAS
                  "PSD45 - Smartscript File transfer program")       ! 1.1JAS

RETURN                                                               ! 1.1JAS

\*****************************************************************************
\***
\*** WRITE.SMARTSCRIPT.REPORT
\***
\*****************************************************************************

WRITE.SMARTSCRIPT.REPORT:                                            ! 1.1JAS

  PRINTER.ERROR = FALSE                                              ! 1.1JAS
  PRINTER.OPEN  = FALSE                                              ! 1.1JAS

  SB.INTEGER% = PRINT.REPORT.NUM%                                    ! 1.1JAS
  IF END# PRINT.SESS.NUM% THEN FILE.NOT.FOUND                        ! 1.1JAS
  OPEN PRINT.FILE.NAME$ AS PRINT.SESS.NUM%                           ! 1.1JAS

  PRINTER.OPEN = TRUE                                                ! 1.1JAS

  IF PRINTER.OPEN THEN BEGIN                                         ! 1.1JAS

     PRINT.LINE$ = "S3705" + STRING$(18," ")                  +  \   ! 1.1JAS
                   "SMARTSCRIPT PHARMACY STOCK COUNT"         +  \   ! 1.1JAS
                   STRING$(12," ") + "PAGE 1 OF 1"                   ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = STRING$(5," ") + "STORE NUMBER"            +  \   ! 1.1JAS
                   STRING$(18," ") + PH.STORE.NUM$                   ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = STRING$(5," ") + "STOCK TAKERS NUMBER"     +  \   ! 1.1JAS
                   STRING$(11," ") + PH.STKTAKE.NUM$                 ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = STRING$(5," ") + "DATE"                    +  \   ! 1.1JAS
                   STRING$(26," ") + LEFT$(PH.DATE$,2)        +  \   ! 1.1JAS
                   "/" + MID$(PH.DATE$,3,2) + "/"             +  \   ! 1.1JAS
                   RIGHT$(PH.DATE$,2)                                ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = STRING$(5," ") + "TIME"                    +  \   ! 1.1JAS
                   STRING$(26," ") + LEFT$(PH.TIME$,2)        +  \   ! 1.1JAS
                   ":" + MID$(PH.TIME$,3,2) + ":"             +  \   ! 1.1JAS
                   RIGHT$(PH.TIME$,2)                                ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     IF PH.COUNT.TYPE$ = "C" THEN BEGIN                             ! 1.2JAS
        IF PH.DISP.AREA$ = "M" THEN BEGIN                            ! 1.2JAS
           PRINT.LINE$ = STRING$(5," ") + "COUNT TYPE"        +  \   ! 1.2JAS
                         STRING$(20," ") + "COUNT MDS"               ! 1.2JAS
        ENDIF ELSE BEGIN                                             ! 1.2JAS
           PRINT.LINE$ = STRING$(5," ") + "COUNT TYPE"        +  \   ! 1.2JAS
                         STRING$(20," ") + "COUNT DISPENSING"        ! 1.2JAS
        ENDIF                                                        ! 1.2JAS
     ENDIF ELSE BEGIN                                                ! 1.2JAS
        IF PH.DISP.AREA$ = "M" THEN BEGIN                            ! 1.2JAS
            PRINT.LINE$ = STRING$(5," ") + "COUNT TYPE"        +  \  ! 1.2JAS
                          STRING$(20," ") + "BOOK IN MDS"            ! 1.2JAS
        ENDIF ELSE BEGIN                                             ! 1.2JAS
            PRINT.LINE$ = STRING$(5," ") + "COUNT TYPE"        +  \  ! 1.2JAS
                          STRING$(20," ") + "BOOK IN DISPENSING"     ! 1.2JAS
        ENDIF                                                        ! 1.2JAS
     ENDIF                                                           ! 1.2JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = STRING$(5," ") + "NUMBER OF ITEMS COUNTED" +  \   ! 1.1JAS
                   STRING$(7," ") + RIGHT$("0000" + STR$(COUNT.NUMBER%),4)  ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = BLANK.LINE$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = FOOTER.LINE$                                      ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

     PRINT.LINE$ = PAGE.THROW$                                       ! 1.1JAS

     CALL WRITE.PRINT                                                ! 1.1JAS

  ENDIF                                                              ! 1.1JAS

NO.PRINT.FILE:                                                       ! 1.1JAS

RETURN                                                               ! 1.1JAS


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.REQUEST
\***
\*****************************************************************************

RECEIVED.RECALLS.REQUEST:                                                           ! 1.5BG

   SB.MESSAGE$ = "PDT Support - RECALLS file request received"                      ! 1.5BG
   GOSUB SB.BG.MESSAGE                                                              ! 1.5BG
   IF FN.VALIDATE.DATA(DATA.IN$,55) = 0 THEN BEGIN                                  ! 1.5BG
      RECEIVE.STATE$ = "*"                                                          ! 1.5BG
      RETURN                                                                        ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   RECALLS.REFERENCE$ = MID$(DATA.IN$,3,8)                                          ! 1.5BG
   CSR.AUDIT.DATA$ = "RECALLS Request for Recall " + RECALLS.REFERENCE$             ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG
   
   MAX.RECALLS.ITEMS% = 500                                                         ! 1.5BG
   
   !Hold PDT                                                                        ! 1.5BG
   PIPE.OUT$ = "HY"                                                                 ! 1.5BG
   GOSUB SEND.TO.PSS38                                                              ! 1.5BG
   HOLD.FLAG$ = "Y"                                                                 ! 1.5BG
   
   IF END# IDF.SESS.NUM% THEN OPEN.ERROR                                            ! 1.5BG
   CURRENT.REPORT.NUM% = IDF.REPORT.NUM%                                            ! 1.9SH
   OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM% NOWRITE NODEL          ! 1.5BG
   IDF.OPEN.FLAG$ = "Y"                                                             ! 1.5BG
   
   IF END #IMSTC.SESS.NUM% THEN OPEN.ERROR                                          ! 1.5BG
   CURRENT.REPORT.NUM% = IMSTC.REPORT.NUM%                                          ! 1.9SH
   OPEN IMSTC.FILE.NAME$ KEYED RECL IMSTC.RECL% AS IMSTC.SESS.NUM% NODEL            ! 1.5BG
   IMSTC.OPEN.FLAG$ = "Y"                                                           ! 1.5BG
   
   IF END #STOCK.SESS.NUM% THEN OPEN.ERROR                                          ! 1.5BG
   CURRENT.REPORT.NUM% = STOCK.REPORT.NUM%                                          ! 1.9SH
   OPEN STOCK.FILE.NAME$ KEYED RECL STOCK.RECL% AS STOCK.SESS.NUM% NODEL            ! 1.5BG
   STOCK.OPEN.FLAG$ = "Y"                                                           ! 1.5BG

   !Create REWKF locked                                                             ! 1.5BG
   IF REWKF.OPEN.FLAG$ = "Y" THEN BEGIN                                             ! 1.5BG
      REWKF.OPEN.FLAG$ = "N"                                                        ! 1.5BG
      CLOSE REWKF.SESS.NUM%                                                         ! 1.5BG
      SB.ACTION$ = "C"                                                              ! 1.5BG
      SB.STRING$ = ""                                                               ! 1.5BG
      SB.INTEGER% = REWKF.SESS.NUM%                                                 ! 1.5BG
      GOSUB SB.FILE.UTILS                                                           ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   SB.ACTION$ = "O"                                                                 ! 1.5BG
   SB.INTEGER% = REWKF.REPORT.NUM%                                                  ! 1.5BG
   SB.STRING$ = REWKF.FILE.NAME$                                                    ! 1.5BG
   GOSUB SB.FILE.UTILS                                                              ! 1.5BG
   REWKF.SESS.NUM% = SB.FILE.SESS.NUM%                                              ! 1.5BG
   CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                                          ! 1.9SH
   IF END #REWKF.SESS.NUM% THEN CREATE.ERROR                                        ! 1.5BG
   CREATE POSFILE REWKF.FILE.NAME$ AS REWKF.SESS.NUM%               \               ! 1.5BG
          BUFFSIZE 512 LOCKED MIRRORED ATCLOSE                                      ! 1.5BG
   REWKF.OPEN.FLAG$ = "Y"                                                           ! 1.5BG
   
   !Open Recalls file and then read recall record for passed recall number          ! 1.5BG
   IF END #RECALLS.SESS.NUM% THEN OPEN.ERROR                                        ! 1.5BG
   CURRENT.REPORT.NUM% = RECALLS.REPORT.NUM%                                        ! 1.9SH
   OPEN RECALLS.FILE.NAME$ KEYED RECL RECALLS.RECL% \                               ! 1.5BG
        AS RECALLS.SESS.NUM% NOWRITE NODEL                                          ! 1.5BG
   RECALLS.OPEN.FLAG$ = "Y"                                                         ! 1.5BG
   RECALLS.CHAIN% = 0                                                               ! 1.5BG
   IF END #RECALLS.SESS.NUM% THEN READ.ERROR                                        ! 1.5BG
   FILE.RETURN.CODE% = READ.RECALLS                                                 ! 1.5BG
   IF FILE.RETURN.CODE% NE 0 THEN BEGIN                                             ! 1.5BG
   
      !Recall record not found - write header and trailer                           ! 1.5BG
      CSR.AUDIT.DATA$ = "RECALLS Recall reference " + RECALLS.REFERENCE$ \          ! 1.5BG
                        + " not found"                                              ! 1.5BG
      GOSUB LOG.TO.AUDIT.FILE                                                       ! 1.5BG
      REWKF.REC.TYPE$ = "YH"                                                        ! 1.5BG
      REWKF.REFERENCE$ = "00000000"                                                 ! 1.5BG
      REWKF.LABEL$ = ""                                                             ! 1.5BG
      REWKF.BATCH.TYPE$ = " "                                                       ! 1.6BG
      REWKF.MRQ$ = "  "                                                             ! 1.7BG
      REWKF.DUE.BY.DATE$ = "        "                                               ! 1.6BG
      FILE.RETURN.CODE% = WRITE.REWKF                                               ! 1.5BG
      IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                               ! 1.5BG
      REWKF.REC.TYPE$ = "YT"                                                        ! 1.5BG
      REWKF.ITEM.COUNT$ = ""                                                        ! 1.5BG
      FILE.RETURN.CODE% = WRITE.REWKF                                               ! 1.5BG
      IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                               ! 1.5BG
      
   ENDIF ELSE BEGIN                                                                 ! 1.5BG
   
      IF RECALLS.STATUS$ = "A" THEN BEGIN                                           ! 1.5BG
      
         !Recall is actioned - write header and trailer                             ! 1.5BG
         CSR.AUDIT.DATA$ = "RECALLS Recall reference " + RECALLS.REFERENCE$ \       ! 1.5BG
                           + " actioned - no records to create"                     ! 1.5BG
         GOSUB LOG.TO.AUDIT.FILE                                                    ! 1.5BG
         REWKF.REC.TYPE$ = "YH"                                                     ! 1.5BG
         REWKF.REFERENCE$ = "        "                                              ! 1.5BG
         REWKF.LABEL$ = ""                                                          ! 1.5BG
         REWKF.BATCH.TYPE$ = " "                                                    ! 1.6BG
         REWKF.MRQ$ = "  "                                                          ! 1.7BG
         REWKF.DUE.BY.DATE$ = "        "                                            ! 1.6BG
         FILE.RETURN.CODE% = WRITE.REWKF                                            ! 1.5BG
         IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                            ! 1.5BG
         REWKF.REC.TYPE$ = "YT"                                                     ! 1.5BG
         REWKF.ITEM.COUNT$ = ""                                                     ! 1.5BG
         FILE.RETURN.CODE% = WRITE.REWKF                                            ! 1.5BG
         IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                            ! 1.5BG
         
      ENDIF ELSE BEGIN
      
         IF (RECALLS.TYPE$ = "I" OR RECALLS.TYPE$ = "C") AND \                      ! 1.6BG
            DATE.GT(RIGHT$(DATE$,6), RIGHT$(RECALLS.DUE.BY.DATE$,6)) THEN BEGIN     ! 1.6BG ! 1.8CS
            
            !Recall is expired for recall types I or C                              ! 1.6BG
            CSR.AUDIT.DATA$ = "RECALLS Recall reference " + RECALLS.REFERENCE$ \    ! 1.6BG
                              + " expired - no records to create"                   ! 1.6BG
            GOSUB LOG.TO.AUDIT.FILE                                                 ! 1.6BG
            REWKF.REC.TYPE$ = "YH"                                                  ! 1.6BG
            REWKF.REFERENCE$ = "XXXXXXXX"                                           ! 1.6BG
            REWKF.LABEL$ = ""                                                       ! 1.6BG
            REWKF.BATCH.TYPE$ = " "                                                 ! 1.6BG
            REWKF.MRQ$ = "  "                                                       ! 1.7BG
            REWKF.DUE.BY.DATE$ = "        "                                         ! 1.6BG
            FILE.RETURN.CODE% = WRITE.REWKF                                         ! 1.6BG
            IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                         ! 1.6BG
            REWKF.REC.TYPE$ = "YT"                                                  ! 1.6BG
            REWKF.ITEM.COUNT$ = ""                                                  ! 1.6BG
            FILE.RETURN.CODE% = WRITE.REWKF                                         ! 1.6BG
            IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                         ! 1.6BG
         
         ENDIF ELSE BEGIN                                                           ! 1.5BG
   
            !Calculate the number of chains for this recall.                        ! 1.5BG
            RECALL.RECS% = INT%((VAL(RECALLS.ITEM.COUNT$)-1) / 50) + 1              ! 1.5BG
            !Calculate the number of items in the last record                       ! 1.5BG
            LAST.RECALL.REC% = (VAL(RECALLS.ITEM.COUNT$) - \                        ! 1.5BG
                               ((RECALL.RECS%-1) * 50))                             ! 1.5BG
         
            REWKF.ITEM.COUNT% = 0                                                   ! 1.5BG
            !Have to start from zero to perform WHILE loop at least once            ! 1.5BG
            CURRENT.RECALL.REC% = 0                                                 ! 1.5BG
            WHILE CURRENT.RECALL.REC% <> RECALL.RECS% AND \                         ! 1.5BG
                  REWKF.ITEM.COUNT% < MAX.RECALLS.ITEMS%                            ! 1.5BG
         
               IF (CURRENT.RECALL.REC%) + 1 = RECALL.RECS% THEN BEGIN               ! 1.5BG
                  ITEMS.THIS.RECORD% = LAST.RECALL.REC%                             ! 1.5BG
               ENDIF ELSE BEGIN                                                     ! 1.5BG
                  ITEMS.THIS.RECORD% = 50                                           ! 1.5BG
               ENDIF                                                                ! 1.5BG
               ITEM.IN.PROGRESS% = 0                                                ! 1.5BG
         
               WHILE ITEM.IN.PROGRESS% < ITEMS.THIS.RECORD% AND \                   ! 1.5BG
                     REWKF.ITEM.COUNT% < MAX.RECALLS.ITEMS%                         ! 1.5BG
                  ! The field RECALLS.ITEM.STOCK$ contains packed F's if the item   ! 1.5BG
                  ! has not been counted and this unpackes to ?'s so test for a ?.  ! 1.5BG
                  ! However, if the recall type is an I or C, extract anyway.               ! 1.6BG
                  IF LEFT$(UNPACK$(RECALLS.ITEM.STOCK$(ITEM.IN.PROGRESS%)),1) = "?" \ 1.5BG ! 1.6BG
                     OR (RECALLS.TYPE$ = "I" OR RECALLS.TYPE$ = "C") THEN BEGIN     ! 1.5BG ! 1.6BG
                     !If this is the first valid record, write the header out       ! 1.5BG
                     IF REWKF.ITEM.COUNT% = 0 THEN BEGIN                            ! 1.5BG
                        CSR.AUDIT.DATA$ = "RECALLS Creating records for Recall " + \! 1.5BG
                                          RECALLS.REFERENCE$                        ! 1.5BG
                        GOSUB LOG.TO.AUDIT.FILE                                     ! 1.5BG
                        REWKF.REC.TYPE$ = "YH"                                      ! 1.5BG
                        REWKF.REFERENCE$ = RECALLS.REFERENCE$                       ! 1.5BG
                        REWKF.LABEL$ = RECALLS.LABEL.TYPE$ + "000000"               ! 1.5BG
                        REWKF.BATCH.TYPE$ = RECALLS.TYPE$                           ! 1.6BG
                        IF (RECALLS.TYPE$ = "I" OR RECALLS.TYPE$ = "C") THEN BEGIN  ! 1.7BG
                           !Ensure ASCII digits passed in MRQ field and not zero's  ! 1.7BG
                           !for types I and C recalls                               ! 1.7BG
                           IF LEFT$(RECALLS.BATCH.NUM$,1) < "0" OR \                ! 1.7BG
                              LEFT$(RECALLS.BATCH.NUM$,1) > "9" OR \                ! 1.7BG
                              MID$(RECALLS.BATCH.NUM$,2,1) < "0" OR \               ! 1.7BG
                              MID$(RECALLS.BATCH.NUM$,2,1) > "9" OR \               ! 1.7BG
                              LEFT$(RECALLS.BATCH.NUM$,2) = "00" THEN BEGIN         ! 1.7BG
                              REWKF.MRQ$ = "  "                                     ! 1.7BG
                           ENDIF ELSE REWKF.MRQ$ = LEFT$(RECALLS.BATCH.NUM$,2)      ! 1.7BG
                        ENDIF ELSE REWKF.MRQ$ = "  "                                ! 1.7BG
                        REWKF.DUE.BY.DATE$ = RECALLS.DUE.BY.DATE$                   ! 1.6BG
                        FILE.RETURN.CODE% = WRITE.REWKF                             ! 1.5BG
                        IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR             ! 1.5BG
                     ENDIF                                                          ! 1.5BG
                     REWKF.ITEM.COUNT% = REWKF.ITEM.COUNT% + 1                      ! 1.5BG
                  
                     REWKF.REC.TYPE$ = "YD"                                         ! 1.5BG
                     PACKED.ITEM.CODE$ = RECALLS.ITEM.CODE$(ITEM.IN.PROGRESS%)      ! 1.5BG
                     GOSUB GET.REWKF.ITEM.DETAILS                                   ! 1.5BG
                     FILE.RETURN.CODE% = WRITE.REWKF                                ! 1.5BG
                     IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                ! 1.5BG
                  ENDIF                                                             ! 1.5BG
                  ITEM.IN.PROGRESS% = ITEM.IN.PROGRESS% + 1                         ! 1.5BG
            
               WEND                                                                 ! 1.5BG
            
               CURRENT.RECALL.REC% = CURRENT.RECALL.REC% + 1                        ! 1.5BG
               !If this is not the last chain, read the next RECALL record          ! 1.5BG
               IF CURRENT.RECALL.REC% <> RECALL.RECS% THEN BEGIN                    ! 1.5BG
                  RECALLS.CHAIN% = RECALLS.CHAIN% +  1                              ! 1.5BG
                  FILE.RETURN.CODE% = READ.RECALLS                                  ! 1.5BG
                  IF FILE.RETURN.CODE% NE 0 THEN GOTO READ.ERROR                    ! 1.5BG
               ENDIF                                                                ! 1.5BG
            
            WEND                                                                    ! 1.5BG
         
            IF REWKF.ITEM.COUNT% = 0 THEN BEGIN                                     ! 1.5BG
               !No un-actioned items were found even though the RECALL  is not A    ! 1.5BG
               CSR.AUDIT.DATA$ = "RECALLS Recall reference " + RECALLS.REFERENCE$ \ ! 1.5BG
                              + " - no records to create"                           ! 1.5BG
               GOSUB LOG.TO.AUDIT.FILE                                              ! 1.5BG
               REWKF.REC.TYPE$ = "YH"                                               ! 1.5BG
               REWKF.REFERENCE$ = "        "                                        ! 1.5BG
               REWKF.LABEL$ = ""                                                    ! 1.5BG
               REWKF.BATCH.TYPE$ = " "                                              ! 1.6BG
               REWKF.MRQ$ = "  "                                                    ! 1.7BG
               REWKF.DUE.BY.DATE$ = "        "                                      ! 1.6BG
               FILE.RETURN.CODE% = WRITE.REWKF                                      ! 1.5BG
               IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                      ! 1.5BG
               REWKF.REC.TYPE$ = "YT"                                               ! 1.5BG
               REWKF.ITEM.COUNT$ = ""                                               ! 1.5BG
               FILE.RETURN.CODE% = WRITE.REWKF                                      ! 1.5BG
               IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                      ! 1.5BG
            ENDIF ELSE BEGIN                                                        ! 1.5BG
               !Write valid trailer for items sent                                  ! 1.5BG
               REWKF.REC.TYPE$ = "YT"                                               ! 1.5BG
               REWKF.ITEM.COUNT$ = STR$(REWKF.ITEM.COUNT%)                          ! 1.5BG
               CSR.AUDIT.DATA$ = "RECALLS - " + REWKF.ITEM.COUNT$ + \               ! 1.5BG
                                 " record(s) created for Recall " + RECALLS.REFERENCE$ ! 1.5BG
                        GOSUB LOG.TO.AUDIT.FILE                                     ! 1.5BG
               FILE.RETURN.CODE% = WRITE.REWKF                                      ! 1.5BG
               IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                      ! 1.5BG
            ENDIF                                                                   ! 1.5BG
         ENDIF                                                                      ! 1.6BG
      ENDIF                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                                          ! 1.9SH
   IF REWKF.OPEN.FLAG$ = "Y" THEN BEGIN                                             ! 1.5BG
      CLOSE REWKF.SESS.NUM%                                                         ! 1.5BG
      REWKF.OPEN.FLAG$ = "N"                                                        ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   !De-allocate REWKF session number                                                ! 1.5BG
   SB.ACTION$ = "C"                                                                 ! 1.5BG
   SB.STRING$ = ""                                                                  ! 1.5BG
   SB.INTEGER% = REWKF.SESS.NUM%                                                    ! 1.5BG
   GOSUB SB.FILE.UTILS                                                              ! 1.5BG
      
   IF IDF.OPEN.FLAG$="Y" THEN BEGIN                                                 ! 1.5BG
      CLOSE IDF.SESS.NUM%                                                           ! 1.5BG
      IDF.OPEN.FLAG$="N"                                                            ! 1.5BG
   ENDIF                                                                            ! 1.5BG
      
   IF IMSTC.OPEN.FLAG$="Y" THEN BEGIN                                               ! 1.5BG
      CLOSE IMSTC.SESS.NUM%                                                         ! 1.5BG
      IMSTC.OPEN.FLAG$="N"                                                          ! 1.5BG
   ENDIF                                                                            ! 1.5BG
      
   IF STOCK.OPEN.FLAG$="Y" THEN BEGIN                                               ! 1.5BG
      CLOSE STOCK.SESS.NUM%                                                         ! 1.5BG
      STOCK.OPEN.FLAG$="N"                                                          ! 1.5BG
   ENDIF                                                                            ! 1.5BG
      
   !Release PDT                                                                     ! 1.5BG
   PIPE.OUT$ = "HN"                                                                 ! 1.5BG
   GOSUB SEND.TO.PSS38                                                              ! 1.5BG
   HOLD.FLAG$ = "N"                                                                 ! 1.5BG
   
   !Get PSS38 to send REWKF                                                         ! 1.5BG
   PIPE.OUT$ = "A"                                                                  ! 1.5BG
   GOSUB SEND.TO.PSS38                                                              ! 1.5BG

RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** GET.REWKF.ITEM.DETAILS
\***
\*****************************************************************************

GET.REWKF.ITEM.DETAILS:                                                             ! 1.5BG

   !Calculate check digit to give 7 digit Boots code                                ! 1.5BG
   RC% = CALC.BOOTS.CODE.CHECK.DIGIT(UNPACK$(PACKED.ITEM.CODE$))                    ! 1.5BG
   IF RC% <> 0 THEN F18.CHECK.DIGIT$ = ""                                           ! 1.5BG
   REWKF.ITEM.CODE$ = UNPACK$(PACKED.ITEM.CODE$) + F18.CHECK.DIGIT$                 ! 1.5BG
   
   !Now read the IDF                                                                ! 1.5BG
   IDF.BOOTS.CODE$ = PACK$("0" + REWKF.ITEM.CODE$)                                  ! 1.5BG
   FILE.RETURN.CODE% = READ.IDF                                                     ! 1.5BG
   IF FILE.RETURN.CODE% NE 0 THEN BEGIN                                             ! 1.5BG
      REWKF.BARCODE$ = "0000000000000"                                              ! 1.5BG
      REWKF.DESCRIPTION$ = "* ITEM NOT ON FILE *"                                   ! 1.5BG
      REWKF.TSF$ = "0000"                                                           ! 1.5BG
   ENDIF ELSE BEGIN                                                                 ! 1.5BG
      REWKF.DESCRIPTION$ = LEFT$(IDF.STNDRD.DESC$,20)                               ! 1.5BG
      IF IDF.BIT.FLAGS.2% AND 10000000b THEN BEGIN                                  ! 1.5BG
         !Boots item so pass Boots barcode                                          ! 1.5BG
         REWKF.BARCODE$ = PACK$("000000") + PACKED.ITEM.CODE$                       ! 1.5BG
      ENDIF ELSE BEGIN                                                              ! 1.5BG
         IF IDF.FIRST.BAR.CODE$ <> PACK$("000000") + PACKED.ITEM.CODE$ AND \        ! 1.5BG
            IDF.FIRST.BAR.CODE$ <> PACK$("000000000000") THEN BEGIN                 ! 1.5BG
            REWKF.BARCODE$ = IDF.FIRST.BAR.CODE$                                    ! 1.5BG
         ENDIF ELSE BEGIN                                                           ! 1.5BG
            IF IDF.SECOND.BAR.CODE$ <> PACK$("000000000000") THEN BEGIN             ! 1.5BG
               REWKF.BARCODE$ = IDF.SECOND.BAR.CODE$                                ! 1.5BG
            ENDIF ELSE BEGIN                                                        ! 1.5BG
               REWKF.BARCODE$ = PACK$("000000") + PACKED.ITEM.CODE$                 ! 1.5BG
            ENDIF                                                                   ! 1.5BG
         ENDIF                                                                      ! 1.5BG
      ENDIF                                                                         ! 1.5BG
      !Now add check digit to barcode                                               ! 1.5BG
      REWKF.BARCODE$ = UNPACK$(REWKF.BARCODE$)                                      ! 1.5BG
      RC% = CALC.BAR.CODE.CHECK.DIGIT(REWKF.BARCODE$)                               ! 1.5BG
      IF RC% <> 0 THEN F06.CHECK.DIGIT$ = ""                                        ! 1.5BG
      REWKF.BARCODE$ = REWKF.BARCODE$ + F06.CHECK.DIGIT$                            ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   !Now get TSF only when the Recall type is not F, S, or X                         ! 1.5BG
   IF RECALLS.TYPE$ = "F" OR RECALLS.TYPE$ = "S" OR RECALLS.TYPE$ = "X" THEN BEGIN  ! 1.5BG
      REWKF.TSF$ = "    "                                                           ! 1.5BG
   ENDIF ELSE BEGIN                                                                 ! 1.5BG
      !Check for IMSTC record for most recent TSF                                   ! 1.5BG
      IMSTC.BAR.CODE$ = PACK$(STRING$(16, "0")) + PACKED.ITEM.CODE$                 ! 1.5BG
      FILE.RETURN.CODE% = READ.IMSTC                                                ! 1.5BG
      IF FILE.RETURN.CODE% = 0 THEN BEGIN                                           ! 1.5BG
         !NOTE: REWKF.TSF$ truncated to 4 digits for PDT.                           ! 1.5BG
         !NOTE: Negative TSF's forced to zero - shoudl never happen though!         ! 1.5BG
         IF IMSTC.STOCK.FIGURE% < 0 THEN IMSTC.STOCK.FIGURE% = 0                    ! 1.5BG
         REWKF.TSF$ = RIGHT$("0000" + STR$(IMSTC.STOCK.FIGURE%), 4)                 ! 1.5BG
      ENDIF ELSE BEGIN                                                              ! 1.5BG
         !No IMSTC record so get off STOCK file                                     ! 1.5BG
         STOCK.BOOTS.CODE$ = PACK$("0" + REWKF.ITEM.CODE$)                          ! 1.5BG
         FILE.RETURN.CODE% = READ.STOCK                                             ! 1.5BG
         IF FILE.RETURN.CODE% = 0 THEN BEGIN                                        ! 1.5BG
            IF STOCK.STOCK.FIG% < 0 THEN STOCK.STOCK.FIG% = 0                       ! 1.5BG
            REWKF.TSF$ = RIGHT$("0000" + STR$(STOCK.STOCK.FIG%), 4)                 ! 1.5BG
         ENDIF ELSE BEGIN                                                           ! 1.5BG
            REWKF.TSF$ = "0000"                                                     ! 1.5BG
         ENDIF                                                                      ! 1.5BG
      ENDIF                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG

RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.RECEIVED.OK
\***
\*****************************************************************************

RECEIVED.RECALLS.RECEIVED.OK:                                                       ! 1.5BG

   SB.MESSAGE$ = "PDT Support - RECALLS file received OK"                           ! 1.5BG
   GOSUB SB.BG.MESSAGE                                                              ! 1.5BG
   
   IF FN.VALIDATE.DATA(DATA.IN$,56) = 0 THEN BEGIN                                  ! 1.5BG
     RECEIVE.STATE$ = "*"                                                           ! 1.5BG
     RETURN                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CSR.AUDIT.DATA$ = "RECALLS File Received to PDT OK for Recall " + \              ! 1.5BG
                     RECALLS.REFERENCE$                                             ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG                  
   
   TEMP.TIME$ = TIME$                                                               ! 1.5BG
   CSR.AUDIT.DATA$ = "RECALLS REQUEST session complete at " +        \              ! 1.5BG
                      LEFT$(TEMP.TIME$,2) + ":" +                    \              ! 1.5BG
                      MID$(TEMP.TIME$,3,2) + ":" +                   \              ! 1.5BG
                      RIGHT$(TEMP.TIME$,2)                                          ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG
   
RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.HEADER 
\***
\*****************************************************************************

RECEIVED.RECALLS.HEADER:                                                            ! 1.5BG

   SB.MESSAGE$ = "PDT Support - RECALLS file header received"                       ! 1.5BG
   GOSUB SB.BG.MESSAGE                                                              ! 1.5BG

   IF FN.VALIDATE.DATA(DATA.IN$,57) = 0 THEN BEGIN                                  ! 1.5BG
     RECEIVE.STATE$ = "*"                                                           ! 1.5BG
     RETURN                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CSR.AUDIT.DATA$ = "RECALLS File Header for Recall " + MID$(DATA.IN$,3,8) + \     ! 1.5BG
                     " Label " + MID$(DATA.IN$,11,14)                               ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG                  
   
   IF MID$(DATA.IN$,25,1) = "Y" THEN BEGIN !First Header Record                     ! 1.5BG
      !Create RECALLBF locked                                                       ! 1.5BG
      IF RB.OPEN.FLAG$ = "Y" THEN BEGIN                                             ! 1.5BG
         CLOSE RB.SESS.NUM%                                                         ! 1.5BG
         RB.OPEN.FLAG$ = "N"                                                        ! 1.5BG
      ENDIF                                                                         ! 1.5BG
      !Deallocate session number                                                    ! 1.5BG
      SB.ACTION$ = "C"                                                              ! 1.5BG
      SB.STRING$ = ""                                                               ! 1.5BG
      SB.INTEGER% = RB.SESS.NUM%                                                    ! 1.5BG
      GOSUB SB.FILE.UTILS                                                           ! 1.5BG
      SB.ACTION$ = "O"                                                              ! 1.5BG
      SB.INTEGER% = RB.REPORT.NUM%                                                  ! 1.5BG
      RB.FILE.NAME$ = "RB:" + TIME$                                                 ! 1.5BG
      SB.STRING$ = RB.FILE.NAME$                                                    ! 1.5BG
      GOSUB SB.FILE.UTILS                                                           ! 1.5BG
      RB.SESS.NUM% = SB.FILE.SESS.NUM%                                              ! 1.5BG
      CURRENT.REPORT.NUM% = RB.REPORT.NUM%                                          ! 1.9SH
      IF END #RB.SESS.NUM% THEN CREATE.ERROR                                        ! 1.5BG
      CREATE POSFILE RB.FILE.NAME$ AS RB.SESS.NUM%               \                  ! 1.5BG
             BUFFSIZE 512 LOCKED MIRRORED ATCLOSE                                   ! 1.5BG
      RB.OPEN.FLAG$ = "Y"                                                           ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   RB.REC.TYPE$= "H"                                                                ! 1.5BG
   RB.REFERENCE$ = MID$(DATA.IN$,3,8)                                               ! 1.5BG
   RB.LABEL$ = MID$(DATA.IN$,11,14)                                                 ! 1.5BG
   FILE.RETURN.CODE% = WRITE.RB                                                     ! 1.5BG
   IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                                  ! 1.5BG
   RECALLS.RECORD.COUNT% = 1                                                        ! 1.5BG

RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.DETAIL
\***
\*****************************************************************************

RECEIVED.RECALLS.DETAIL:                                                            ! 1.5BG

   IF FN.VALIDATE.DATA(DATA.IN$,58) = 0 THEN BEGIN                                  ! 1.5BG
     RECEIVE.STATE$ = "*"                                                           ! 1.5BG
     RETURN                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CSR.AUDIT.DATA$ = "RECALLS File Detail Item " + MID$(DATA.IN$,3,7)               ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG
   
   RB.REC.TYPE$= "D"                                                                ! 1.5BG
   RB.ITEM.CODE$ = MID$(DATA.IN$,3,7)                                               ! 1.5BG
   RB.STOCK.COUNT$ = MID$(DATA.IN$,10,4)                                            ! 1.5BG
   FILE.RETURN.CODE% = WRITE.RB                                                     ! 1.5BG
   IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                                  ! 1.5BG
   RECALLS.RECORD.COUNT% = RECALLS.RECORD.COUNT% + 1                                ! 1.5BG
   
RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.TRAILER
\***
\*****************************************************************************

RECEIVED.RECALLS.TRAILER:                                                           ! 1.5BG

   SB.MESSAGE$ = "PDT Support - RECALLS file trailer received"                      ! 1.5BG
   GOSUB SB.BG.MESSAGE                                                              ! 1.5BG

   IF FN.VALIDATE.DATA(DATA.IN$,59) = 0 THEN BEGIN                                  ! 1.5BG
     RECEIVE.STATE$ = "*"                                                           ! 1.5BG
     RETURN                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CSR.AUDIT.DATA$ = "RECALLS File Trailer "                                        ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG
   
   RB.REC.TYPE$= "T"                                                                ! 1.5BG
   RB.ITEM.COUNT$ = MID$(DATA.IN$,3,5)                                              ! 1.5BG
   RB.RECORD.COUNT$ = MID$(DATA.IN$,8,4)                                            ! 1.5BG
   FILE.RETURN.CODE% = WRITE.RB                                                     ! 1.5BG
   IF FILE.RETURN.CODE% NE 0 THEN GOTO WRITE.ERROR                                  ! 1.5BG
   
   RECALLS.RECORD.COUNT% = RECALLS.RECORD.COUNT% + 1                                ! 1.5BG
   
   !Send an acknowledgement to the PDT                                              ! 1.5BG
   !Needs to be in the following format to work with the PDT                        ! 1.5BG
   PIPE.OUT$ = "L" + "YT00000" + CHR$(0Dh) + CHR$(0Ah)                              ! 1.5BG
   IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR                                         ! 1.5BG
   CURRENT.REPORT.NUM% = PIPEI.REPORT.NUM%                                          ! 1.9SH
   WRITE #PIPEI.SESS.NUM%; PIPE.OUT$                                                ! 1.5BG
   
   IF RECALLS.RECORD.COUNT% <> VAL(MID$(DATA.IN$,8,4)) THEN BEGIN                   ! 1.5BG
      CSR.AUDIT.DATA$ = "RECALLS File Trailer RECORD COUNT MISMATCH!"               ! 1.5BG
      GOSUB LOG.TO.AUDIT.FILE                                                       ! 1.5BG
      CSR.AUDIT.DATA$ = "RECALLS File Trailer - Expected " + \                      ! 1.5BG
                        MID$(DATA.IN$,8,4) + " records but got " + \                ! 1.5BG
                        STR$(RECALLS.RECORD.COUNT%) + " records"                    ! 1.5BG
      GOSUB LOG.TO.AUDIT.FILE                                                       ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
RETURN                                                                              ! 1.5BG


\*****************************************************************************
\***
\*** RECEIVED.RECALLS.EOT    
\***
\*****************************************************************************

RECEIVED.RECALLS.EOT:                                                               ! 1.5BG

   SB.MESSAGE$ = "PDT Support - RECALLS EOT received"                               ! 1.5BG
   GOSUB SB.BG.MESSAGE                                                              ! 1.5BG
                                                                          
   IF FN.VALIDATE.DATA(DATA.IN$,60) = 0 THEN BEGIN                                  ! 1.5BG
     RECEIVE.STATE$ = "*"                                                           ! 1.5BG
     RETURN                                                                         ! 1.5BG
   ENDIF                                                                            ! 1.5BG
   
   CSR.AUDIT.DATA$ = "RECALLS File EOT"                                             ! 1.5BG
   GOSUB LOG.TO.AUDIT.FILE                                                          ! 1.5BG
   
   IF MID$(DATA.IN$,3,1) = "T" THEN BEGIN                                           ! 1.5BG
   
      !If the EOT is for data in from the PDT, now close the RB: file               ! 1.5BG
      !and start PSD72.                                                             ! 1.5BG
   
      CURRENT.REPORT.NUM% = RB.REPORT.NUM%                                          ! 1.5BG
      IF RB.OPEN.FLAG$ = "Y" THEN BEGIN                                             ! 1.5BG
         CLOSE RB.SESS.NUM%                                                         ! 1.5BG
         RB.OPEN.FLAG$ = "N"                                                        ! 1.5BG
      ENDIF                                                                         ! 1.5BG
      !De-allocate RB session number                                                ! 1.5BG
      SB.ACTION$ = "C"                                                              ! 1.5BG
      SB.STRING$ = ""                                                               ! 1.5BG
      SB.INTEGER% = RB.SESS.NUM%                                                    ! 1.5BG
      GOSUB SB.FILE.UTILS                                                           ! 1.5BG
      
      ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSD72.286", RB.FILE.NAME$, \               ! 1.5BG
                      "PSD72 - Recall Batch Processing")                            ! 1.5BG
      
      TEMP.TIME$ = TIME$                                                            ! 1.5BG
      CSR.AUDIT.DATA$ = "RECALLS session complete at " +                \           ! 1.5BG
                         LEFT$(TEMP.TIME$,2) + ":" +                    \           ! 1.5BG
                         MID$(TEMP.TIME$,3,2) + ":" +                   \           ! 1.5BG
                         RIGHT$(TEMP.TIME$,2)                                       ! 1.5BG
      GOSUB LOG.TO.AUDIT.FILE                                                       ! 1.5BG
   
   ENDIF                                                                            ! 1.5BG
   
   IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN                                         ! 1.5BG
     CLOSE CSR.AUDIT.SESS.NUM%                                                      ! 1.5BG
     CSR.AUDIT.OPEN.FLAG$ = "N"                                                     ! 1.5BG
   ENDIF                                                                            ! 1.5BG

   RE.CHAIN = TRUE                                                                  ! 1.5BG
   RECEIVE.STATE$ = "?"                                                             ! 1.5BG

RETURN                                                                              ! 1.5BG

\*****************************************************************************
\***
\*** RECEIVED.PUOD.SIGN.ON
\***
\*****************************************************************************

RECEIVED.PUOD.SIGN.ON:                                                           

   SB.MESSAGE$ = "PDT Support - +UOD sign on request received"              !1.9SH                   
   GOSUB SB.BG.MESSAGE                                                      !1.9SH     
   IF FN.VALIDATE.DATA(DATA.IN$,61) = 0 THEN BEGIN                          !1.9SH     
      RECEIVE.STATE$ = "*"                                                  !1.9SH     
      RETURN                                                                !1.9SH     
   ENDIF                                                                    !1.9SH     

   !Hold PDT                                                                !1.9SH     
   PIPE.OUT$ = "HY"                                                         !1.9SH     
   GOSUB SEND.TO.PSS38                                                      !1.9SH     
   HOLD.FLAG$ = "Y"                                                         !1.9SH     

   !Hijack the REWKF file                                                   !1.9SH
   CLOSE REWKF.SESS.NUM%                                                    !1.9SH       
   REWKF.OPEN.FLAG$ = "N"                                                   !1.9SH
   CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                                  !1.9SH              
   IF END #REWKF.SESS.NUM% THEN CREATE.ERROR                                !1.9SH       
   CREATE POSFILE REWKF.FILE.NAME$ AS REWKF.SESS.NUM% \                     !1.9SH
     BUFFSIZE 32767 LOCKED MIRRORED ATCLOSE                                 !1.9SH
   REWKF.OPEN.FLAG$ = "Y"                                                   !1.9SH       
   
   !Default bad response                                                    !1.9SH
   PIPE.OUT$ = "TBNN000"                                                    !1.9SH
   AUTHORISED = FALSE                                                       !1.9SH
   
   !Open authorisation file                                                 !1.9SH
   IF AF.OPEN.FLAG$ = "N" THEN BEGIN                                        !1.9SH
       IF END #AF.SESS.NUM% THEN OPEN.ERROR                                 !1.9SH        
       CURRENT.REPORT.NUM% = AF.REPORT.NUM%                                 !1.9SH
       OPEN AF.FILE.NAME$ KEYED RECL AF.RECL% AS AF.SESS.NUM% NOWRITE NODEL !1.9SH
       AF.OPEN.FLAG$ = "Y"                                                  !1.9SH        
   ENDIF                                                                    !1.9SH

   !Attempt read on authorisation file                                      !1.9SH
   AF.OPERATOR.NO$ = PACK$(RIGHT$("00000000" + MID$(DATA.IN$, 3, 3), 8))    !1.9SH
   IF READ.AF = 0 THEN BEGIN                                                !1.9SH
      IF AF.PASSWORD$ = PACK$(RIGHT$("00000000" + MID$(DATA.IN$, 6, 3), 8)) THEN BEGIN !1.9SH

         !Set authorised flag                                               !1.9SH
         AUTHORISED = TRUE                                                  !1.9SH

         !Get SOFTS options                                                 !1.9SH
         GOSUB PROCESS.SOFTS                                                !1.9SH
         IF ONIGHT THEN BEGIN                                               !1.9SH
            PIPE.OUT$ = "TBYY"                                              !1.9SH
         ENDIF ELSE BEGIN                                                   !1.9SH
            PIPE.OUT$ = "TBYN"                                              !1.9SH
         ENDIF                                                              !1.9SH
         PIPE.OUT$ = PIPE.OUT$ + BATCH.SIZE$                                !1.9SH

      ENDIF                                                                 !1.9SH
   ENDIF                                                                    !1.9SH
   
   !Close auth file                                                         !1.9SH
   CLOSE AF.SESS.NUM%                                                       !1.9SH
   AF.OPEN.FLAG$ = "N"                                                      !1.9SH
                                
   !Send auth response and config to the PDT                                !1.9SH
   IF END# REWKF.SESS.NUM% THEN WRITE.ERROR                                 !1.9SH
   CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                                  !1.9SH
   WRITE #REWKF.SESS.NUM%; PIPE.OUT$                                        !1.9SH

   NUM.RECS% = 1                                                            !1.9SH

   IF AUTHORISED THEN BEGIN                                                 !1.9SH
   
      !Get summary records from the UOD outers                              !1.9SH
      IF UODOT.OPEN.FLAG$ = "N" THEN BEGIN                                  !1.9SH
         OPEN UODOT.FILE.NAME$ KEYED RECL UODOT.RECL% AS UODOT.SESS.NUM% NOWRITE NODEL!1.9SH
         UODOT.OPEN.FLAG$ = "Y"                                             !1.9SH
      ENDIF                                                                 !1.9SH
      UODOT.LICENCE$ = STRING$(5, CHR$(0FFH))                               !1.9SH
      UODOT.SEQ% = 0FFFFh                                                   !1.9SH
      IF READ.UODOT THEN BEGIN                                              !1.9SH
         GOSUB READ.ERROR.SUB                                               !1.9SH
      ENDIF ELSE BEGIN                                                      !1.9SH
         WHILE LEN(UODOT.SUMMARY.STATUS$)                                   !1.9SH
             PIPE.OUT$ = "TC"                              +                \1.9SH
                         LEFT$(UODOT.SUMMARY.STATUS$, 1)   +                \1.9SH
                         MID$(UODOT.SUMMARY.STATUS$, 2, 1) +                \1.9SH
                         RIGHT$(UNPACK$(MID$(UODOT.SUMMARY.STATUS$,3,3)),5) !1.9SH
             CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                        !1.9SH
             WRITE #REWKF.SESS.NUM%; PIPE.OUT$                              !1.9SH
             NUM.RECS% = NUM.RECS% + 1                                      !1.9SH
             UODOT.SUMMARY.STATUS$ = MID$(UODOT.SUMMARY.STATUS$, 6, 32767)  !1.9SH
         WEND                                                               !1.9SH
      ENDIF                                                                 !1.9SH
      
      !Initialise the UOD array                                             !1.9SH
      DIM UOD$(16000)                                                       !1.9SH
      NUM.UODS% = 0                                                         !1.9SH
      
      !Open DELVINDX                                                        !1.9SH
      IF END# DELVINDX.SESS.NUM% THEN OPEN.ERROR                            !1.9SH             
      CURRENT.REPORT.NUM% = DELVINDX.REPORT.NUM%                            !1.9SH                    
      OPEN DELVINDX.FILE.NAME$ AS DELVINDX.SESS.NUM% BUFFSIZE 2048 NOWRITE NODEL!1.9SH
      DELVINDX.OPEN.FLAG$ = "Y"                                             !1.9SH             
      
      !Send DELVINDX recs that have not been booked in to the PDT           !1.9SH
      PIPE.OUT$ = ""                                                        !1.9SH
      WHILE READ.DELVINDX = 0                                               !1.9SH
         UODOT.LICENCE$ = PACK$(DELVINDX.UOD.LICENCE$)                      !1.9SH
         UODOT.SEQ% = VALUE%(DELVINDX.UOD.SEQ$)                             !1.9SH
         IF READ.UODOT THEN BEGIN                                           !1.9SH
            GOSUB READ.ERROR.SUB                                            !1.9SH
         ENDIF ELSE BEGIN                                                   !1.9SH
            IF NOT UODOT.BOOKED THEN BEGIN                                  !1.9SH
               IF UODOT.EST.DEL.DATE$ = PACK$(DATE$) THEN BEGIN             !1.9SH
                  STATUS$ = "E"                                             !1.9SH
               ENDIF ELSE IF UODOT.EST.DEL.DATE$ < PACK$(DATE$) THEN BEGIN  !1.9SH
                  STATUS$ = "O"                                             !1.9SH
               ENDIF ELSE BEGIN                                             !1.9SH
                  STATUS$ = "F"                                             !1.9SH
               ENDIF                                                        !1.9SH
            ENDIF ELSE BEGIN                                                !1.9SH
               STATUS$ = "Y"                                                !1.9SH
            ENDIF                                                           !1.9SH
            
            IF UODOT.BOL THEN BEGIN                                         !1.10SH
                BOL$ = "Y"                                                  !1.10SH
            ENDIF ELSE BEGIN                                                !1.10SH
                BOL$ = "N"                                                  !1.10SH
            ENDIF                                                           !1.10SH
            
            !Don't send UODs marked for deletion                            !1.9SH
            IF UODOT.REASON$ <> "X" THEN BEGIN                              !1.9SH
            
               !Don't send already booked UODs that weren't booked in today,!1.9SH
               !to reduce transmission volumes                              !1.9SH
               IF NOT UODOT.BOOKED OR UODOT.BOOKED.DATE$ = PACK$(DATE$) THEN BEGIN!1.9SH
                  IF PIPE.OUT$ = "" THEN BEGIN                              !1.9SH
                     PIPE.OUT$ = "TD" + DELVINDX.UOD.LICENCE$       + \     !1.9SH
                                        DELVINDX.UOD.DESPATCH.DATE$ + \     !1.9SH
                                        DELVINDX.UOD.PARENT$        + \     !1.9SH
                                        UODOT.TYPE$                 + \     !1.9SH
                                        STATUS$                     + \     !1.10SH
                                        BOL$                                !1.10SH
                  ENDIF ELSE BEGIN                                          !1.9SH
                     PIPE.OUT$ = PIPE.OUT$ + DELVINDX.UOD.LICENCE$       + \!1.9SH
                                             DELVINDX.UOD.DESPATCH.DATE$ + \!1.9SH
                                             DELVINDX.UOD.PARENT$        + \!1.9SH
                                             UODOT.TYPE$                 + \!1.9SH
                                             STATUS$                     + \!1.10SH
                                             BOL$                           !1.10SH
                     CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                !1.9SH
                     WRITE #REWKF.SESS.NUM%; PIPE.OUT$                      !1.9SH
                     NUM.RECS% = NUM.RECS% + 1                              !1.9SH
                     PIPE.OUT$ = ""                                         !1.9SH
                  ENDIF                                                     !1.9SH
               ENDIF                                                        !1.9SH
            
            ENDIF                                                           !1.9SH
            
         ENDIF                                                              !1.9SH
      WEND                                                                  !1.9SH

      !Mop up if there were an odd number of UODs                           !1.9SH
      IF LEN(PIPE.OUT$) THEN BEGIN                                          !1.9SH
         PIPE.OUT$ = PIPE.OUT$ + STRING$(28, "0")                           !1.9SH
         CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                            !1.9SH
         WRITE #REWKF.SESS.NUM%; PIPE.OUT$                                  !1.9SH
         NUM.RECS% = NUM.RECS% + 1                                          !1.9SH
         PIPE.OUT$ = ""                                                     !1.9SH
      ENDIF                                                                 !1.9SH
      
   ENDIF                                                                    !1.9SH
   
   !Send trailer                                                            !1.9SH
   PIPE.OUT$ = "TE" + PADNUM$(NUM.RECS% + 1, 5)                             !1.9SH
   CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%                                  !1.9SH
   WRITE #REWKF.SESS.NUM%; PIPE.OUT$                                        !1.9SH

   !Close REWKF                                                             !1.9SH
   CLOSE REWKF.SESS.NUM%                                                    !1.9SH     
   REWKF.OPEN.FLAG$ = "N"                                                   !1.9SH     
   
   !Release PDT                                                             !1.9SH     
   PIPE.OUT$ = "HN"                                                         !1.9SH     
   GOSUB SEND.TO.PSS38                                                      !1.9SH     
   HOLD.FLAG$ = "N"                                                         !1.9SH     
   
   !Get PSS38 to send REWKF                                                 !1.9SH     
   PIPE.OUT$ = "A"                                                          !1.9SH     
   GOSUB SEND.TO.PSS38                                                      !1.9SH     
   
   !Close files                                                             !1.9SH
   CLOSE DELVINDX.SESS.NUM%                                                 !1.9SH
   DELVINDX.OPEN.FLAG$ = "N"                                                !1.9SH
   CLOSE UODOT.SESS.NUM%                                                    !1.9SH
   UODOT.OPEN.FLAG$ = "N"                                                   !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.RECEIVED.OK
\***
\*****************************************************************************

RECEIVED.PUOD.RECEIVED.OK:                                                  !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD received OK"                           !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
   
   IF FN.VALIDATE.DATA(DATA.IN$,62) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH
   
   TEMP.TIME$ = TIME$                                                       !1.9SH
   CSR.AUDIT.DATA$ = "+UOD send complete at " + \                           !1.9SH
                      LEFT$(TEMP.TIME$,2)  + ":" + \                        !1.9SH
                      MID$(TEMP.TIME$,3,2) + ":" + \                        !1.9SH
                      RIGHT$(TEMP.TIME$,2)                                  !1.9SH
   GOSUB LOG.TO.AUDIT.FILE                                                  !1.9SH

   CLOSE CSR.AUDIT.SESS.NUM%                                                !1.9SH
   CSR.AUDIT.OPEN.FLAG$ = "N"                                               !1.9SH

   RE.CHAIN = TRUE                                                          !1.9SH
   RECEIVE.STATE$ = "?"                                                     !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.HEADER
\***
\*****************************************************************************

RECEIVED.PUOD.HEADER:                                                       !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD header received"                       !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
   
   IF FN.VALIDATE.DATA(DATA.IN$,63) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH

   !Read SOFTS to get overnight config                                      !1.9SH
   GOSUB PROCESS.SOFTS                                                      !1.9SH
   
   FILE.SUFFIX$ = TIME$ + ".PDT"                                            !1.9SH
   RETRIES% = 10                                                            !1.9SH
UB.RETRY:                                                                   !1.9SH
   IF END # UB.SESS.NUM% THEN CREATE.ERROR                                  !1.9SH
   CURRENT.REPORT.NUM% = UB.REPORT.NUM%                                     !1.9SH
   CREATE POSFILE UB.TEMP.NAME$ + FILE.SUFFIX$ AS UB.SESS.NUM% \            !1.9SH
                                          BUFFSIZE 32767 LOCKED MIRRORED ATCLOSE!1.9SH
   UB.OPEN.FLAG$ = "Y"                                                      !1.9SH
   UB.REC.TYPE$   = "H"                                                     !1.9SH
   UB.OP.ID$      = "00000" + MID$(DATA.IN$, 3, 3)                          !1.9SH
   UB.METHOD$     = "P"                                                     !1.9SH
   UB.REPORT.RQD$ = "Y"                                                     !1.9SH
   IF ONIGHT THEN BEGIN                                                     !1.9SH
      UB.ONIGHT.DELIVERY.TYPE$ = "Y"                                        !1.9SH
   ENDIF ELSE BEGIN                                                         !1.9SH
      UB.ONIGHT.DELIVERY.TYPE$ = "N"                                        !1.9SH
   ENDIF                                                                    !1.9SH
   UB.DRIVER.CHECKIN.REQD$ = "N"                                            !1.9SH
   IF WRITE.UB THEN GOTO WRITE.ERROR                                        !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.DETAIL
\***
\*****************************************************************************

RECEIVED.PUOD.DETAIL:                                                       !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD detail received"                       !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
   
   IF FN.VALIDATE.DATA(DATA.IN$,64) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH

   UB.REC.TYPE$      = "B"                                                  !1.9SH
   UB.LICENCE$       = MID$(DATA.IN$, 3, 10)                                !1.9SH
   UB.DESPATCH.DATE$ = MID$(DATA.IN$, 13, 6)                                !1.9SH
   UB.BOOKED.DATE$   = MID$(DATA.IN$, 19, 6)                                !1.9SH
   UB.BOOKED.TIME$   = MID$(DATA.IN$, 25, 6)                                !1.9SH
   UB.BOOK.TYPE$     = MID$(DATA.IN$, 31, 1)                                !1.9SH
   IF WRITE.UB THEN GOTO WRITE.ERROR                                        !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.BATCH.TRAILER
\***
\*****************************************************************************

RECEIVED.PUOD.BATCH.TRAILER:                                                !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD batch trailer received"                !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
   
   IF FN.VALIDATE.DATA(DATA.IN$,65) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH

   UB.REC.TYPE$          = "C"                                              !1.9SH
   UB.DRVR.ID$           = MID$(DATA.IN$, 3, 8)                             !1.9SH
   UB.DRVR.REJECTED$     = MID$(DATA.IN$, 11, 1)                            !1.9SH
   UB.DRVR.CHCK.DATE$    = MID$(DATA.IN$, 12, 6)                            !1.9SH
   UB.DRVR.CHCK.TIME$    = MID$(DATA.IN$, 18, 6)                            !1.9SH
   UB.DRVR.GIT.MATCH$    = " "                                              !1.9SH
   UB.DRVR.CONFIRM.SCAN$ = "C"                                              !1.9SH
   IF WRITE.UB THEN GOTO WRITE.ERROR                                        !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.SESSION.TRAILER
\***
\*****************************************************************************

RECEIVED.PUOD.SESSION.TRAILER:                                              !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD session trailer received"              !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
   
   IF FN.VALIDATE.DATA(DATA.IN$,66) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH

   UB.REC.TYPE$ = "C"                                                       !1.9SH
   UB.DRVR.ID$ = MID$(DATA.IN$, 3, 8)                                       !1.9SH
   UB.DRVR.GIT.MATCH$ = MID$(DATA.IN$, 11, 1)                               !1.9SH
   UB.DRVR.CHCK.DATE$ = MID$(DATA.IN$, 12, 6)                               !1.9SH
   UB.DRVR.CHCK.TIME$ = MID$(DATA.IN$, 18, 6)                               !1.9SH
   UB.DRVR.CONFIRM.SCAN$ = "S"                                              !1.9SH
   UB.DRVR.REJECTED$ = " "                                                  !1.9SH
   IF WRITE.UB THEN GOTO WRITE.ERROR                                        !1.9SH

   UB.REC.TYPE$ = "T"                                                       !1.9SH
   UB.REC.COUNT$ = PADNUM$(VALUE%(MID$(DATA.IN$, 24, 5)) + 1, 5)            !1.9SH
   IF WRITE.UB THEN GOTO WRITE.ERROR                                        !1.9SH

   !Send acknowledgement to the PDT                                         !1.9SH
   PIPE.OUT$ = "L" + "TM"                                                   !1.9SH
   GOSUB SEND.TO.PSS38                                                      !1.9SH
          
RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** RECEIVED.PUOD.EOT
\***
\*****************************************************************************

RECEIVED.PUOD.EOT:                                                          !1.9SH

   SB.MESSAGE$ = "PDT Support - +UOD EOT received"                          !1.9SH
   GOSUB SB.BG.MESSAGE                                                      !1.9SH
                                                                          
   IF FN.VALIDATE.DATA(DATA.IN$,67) = 0 THEN BEGIN                          !1.9SH
     RECEIVE.STATE$ = "*"                                                   !1.9SH
     RETURN                                                                 !1.9SH
   ENDIF                                                                    !1.9SH
   
   IF UB.OPEN.FLAG$ = "Y" THEN BEGIN                                        !1.9SH
     CLOSE UB.SESS.NUM%                                                     !1.9SH
   ENDIF                                                                    !1.9SH

   RETRIES% = 10                                                            !1.9SH
   NEW.SUFFIX$ = FILE.SUFFIX$                                               !1.9SH
   RC% = RENAME(UB.FILE.NAME$ + NEW.SUFFIX$, \                              !1.9SH
                UB.TEMP.NAME$ + FILE.SUFFIX$)                               !1.9SH
   WHILE RC% = 0 AND RETRIES% > 0                                           !1.9SH
     RETRIES% = RETRIES% - 1                                                !1.9SH
     CALL INCREMENT.SUFFIX(NEW.SUFFIX$)                                     !1.9SH
     RC% = RENAME(UB.FILE.NAME$ + NEW.SUFFIX$, \                            !1.9SH
                  UB.TEMP.NAME$ + FILE.SUFFIX$)                             !1.9SH
   WEND                                                                     !1.9SH

   !If rename fails then log error, else start SSC01                        !1.9SH
   IF RC% = 0 THEN BEGIN                                                    !1.9SH
     CALL APPLICATION.LOG(221, UB.FILE.NAME$ + FILE.SUFFIX$, \              !1.9SH
                            "Error booking in, check event log", 14)        !1.9SH
   ENDIF ELSE BEGIN                                                         !1.9SH
     CALL ADXSTART("ADX_UPGM:SSC01.286", UB.FILE.NAME$ + NEW.SUFFIX$, \     !1.9SH
                   "PSS37 is attempting to start SSC01")                    !1.9SH
   ENDIF                                                                    !1.9SH

   CSR.AUDIT.DATA$ = "+UOD EOT"                                             !1.9SH
   GOSUB LOG.TO.AUDIT.FILE                                                  !1.9SH
   IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN                                 !1.9SH
     CLOSE CSR.AUDIT.SESS.NUM%                                              !1.9SH
     CSR.AUDIT.OPEN.FLAG$ = "N"                                             !1.9SH
   ENDIF                                                                    !1.9SH

   RE.CHAIN = TRUE                                                          !1.9SH
   RECEIVE.STATE$ = "?"                                                     !1.9SH

RETURN                                                                      !1.9SH


\*****************************************************************************
\***
\*** PROCESS.SOFTS
\***
\*****************************************************************************

PROCESS.SOFTS:                                                              !1.9SH

   !Open, read, close Software Status File                                  !1.9SH
   IF SOFTS.OPEN.FLAG$ = "N" THEN BEGIN                                     !1.9SH
      IF END #SOFTS.SESS.NUM% THEN OPEN.ERROR                               !1.9SH          
      CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%                               !1.9SH                 
      OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL% AS SOFTS.SESS.NUM% NOWRITE NODEL !1.9SH
      SOFTS.OPEN.FLAG$ = "Y"                                                !1.9SH          
   ENDIF                                                                    !1.9SH
   SOFTS.REC.NUM% = 57                                                      !1.9SH
   IF READ.SOFTS THEN GOTO READ.ERROR                                       !1.9SH
   CLOSE SOFTS.SESS.NUM%                                                    !1.9SH
   SOFTS.OPEN.FLAG$ = "N"                                                   !1.9SH
   
   IF MATCH("ONIGHT STORE=Y", SOFTS.RECORD$, 1) THEN BEGIN                  !1.9SH
      ONIGHT = TRUE                                                         !1.9SH
   ENDIF ELSE BEGIN                                                         !1.9SH
      ONIGHT = FALSE                                                        !1.9SH
   ENDIF                                                                    !1.9SH
   
   FILE.RETURN.CODE% = MATCH("BATCH SIZE=", SOFTS.RECORD$, 1)               !1.9SH
   IF FILE.RETURN.CODE% > 0 THEN BEGIN                                      !1.9SH
      BATCH.SIZE$ = MID$(SOFTS.RECORD$, FILE.RETURN.CODE% + 11, 3)          !1.9SH
   ENDIF ELSE BEGIN                                                         !1.9SH
      BATCH.SIZE$ = "000"                                                   !1.9SH
   ENDIF                                                                    !1.9SH

RETURN                                                                      !1.9SH


\******************************************************************************
\***
\***   SEND.TO.PSS38:
\***
\***      transmit data to PSS38 (data contained in PIPE.OUT$)
\***
\******************************************************************************

SEND.TO.PSS38:                                                              ! 1.5BG

   IF END # PIPEI.SESS.NUM% THEN WRITE.ERROR                                ! 1.5BG
   CURRENT.REPORT.NUM% = PIPEI.REPORT.NUM%                                  ! 1.9SH
   WRITE # PIPEI.SESS.NUM%; PIPE.OUT$                                       ! 1.5BG

RETURN                                                                      ! 1.5BG


\******************************************************************************
\***
\***   LOG.TO.AUDIT.FILE
\***
\***   RETURN
\***
\******************************************************************************

LOG.TO.AUDIT.FILE:                                                                  ! 1.5BG

   IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN                                         ! 1.5BG
      IF END #CSR.AUDIT.SESS.NUM% THEN AUDIT.FAIL                                   ! 1.5BG
      OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND                            ! 1.5BG
      CSR.AUDIT.OPEN.FLAG$ = "Y"                                                    ! 1.5BG
   ENDIF                                                                            ! 1.5BG

   CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " + \                          ! 1.5BG
                        CSR.AUDIT.DATA$                                             ! 1.5BG  
   PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$                            ! 1.5BG

AUDIT.FAIL:                                                                         ! 1.5BG

   RETURN                                                                           ! 1.5BG


\******************************************************************************
\***
\***   OPEN.FILES
\***
\******************************************************************************

OPEN.FILES:

    SB.ACTION$ = "O"                                                        ! 1.4CS
                                                                            ! 1.4CS
    BUFFER.FILE$ = "ADXLXACN::D:\ADX_UDT1\SCBUF" + MONITORED.PORT$ + ".PDT" ! 1.4CS                                      
                                                                            ! 1.4CS
    IF MONITORED.PORT$ = "A" THEN BEGIN                                     ! 1.4CS
        BUFFER.REPORT.NUM% = 210         ! Port A                           ! 1.4CS
    ENDIF ELSE BEGIN                                                        ! 1.4CS
        BUFFER.REPORT.NUM% = 211         ! Port B                           ! 1.4CS          
    ENDIF                                                                   ! 1.4CS

    SB.INTEGER% = BUFFER.REPORT.NUM%                                        ! 1.4CS
    SB.STRING$ = BUFFER.FILE$                                               ! 1.4CS
    GOSUB SB.FILE.UTILS                                                     ! 1.4CS 
    BUFFER.SESS.NUM% = SB.FILE.SESS.NUM%                                    ! 1.4CS
    IF END #BUFFER.SESS.NUM% THEN CREATE.BUFFER.FILE                        ! 1.4CS
    OPEN BUFFER.FILE$ AS BUFFER.SESS.NUM% APPEND                            ! 1.4CS
    GOTO BUFFER.OK                                                          ! 1.4CS
                                                                            ! 1.4CS
  CREATE.BUFFER.FILE:                                                       ! 1.4CS
                                                                            ! 1.4CS
    IF END #BUFFER.SESS.NUM% THEN CREATE.ERROR                              ! 1.4CS
    CREATE BUFFER.FILE$ AS BUFFER.SESS.NUM%                                 ! 1.4CS
                                                                            ! 1.4CS
  BUFFER.OK:                                                                ! 1.4CS
  
    BUFFER.OPEN.FLAG$ = "Y"                                                 ! 1.5BG

RETURN


\******************************************************************************
\***
\***   CLOSE.FILES
\***
\******************************************************************************

CLOSE.FILES:

   CLOSE SOFTS.SESS.NUM%                                            !1.9SH
   SOFTS.OPEN.FLAG$ = ""                                            !1.9SH
   CLOSE DELVINDX.SESS.NUM%                                         !1.9SH
   DELVINDX.OPEN.FLAG$ = ""                                         !1.9SH
   CLOSE UODOT.SESS.NUM%                                            !1.9SH
   UODOT.OPEN.FLAG$ = ""                                            !1.9SH
   CLOSE AF.SESS.NUM%                                               !1.9SH
   AF.OPEN.FLAG$ = ""                                               !1.9SH
   
   IF SSPSCTRL.OPEN.FLAG$ = "Y" THEN BEGIN                           ! 1.1JAS
      SSPSCTRL.OPEN.FLAG$ = "N"                                      ! 1.1JAS
      CLOSE SSPSCTRL.SESS.NUM%                                       ! 1.1JAS
   ENDIF                                                             ! 1.1JAS
   IF BTCS.OPEN.FLAG$ = "Y" THEN BEGIN                               ! 1.1JAS
      BTCS.OPEN.FLAG$ = "N"                                          ! 1.1JAS
      CLOSE BTCS.SESS.NUM%                                           ! 1.1JAS
   ENDIF                                                             ! 1.1JAS
   IF PRINTER.OPEN THEN BEGIN                                        ! 1.1JAS
      PRINTER.OPEN = FALSE                                           ! 1.1JAS
      CLOSE PRINT.SESS.NUM%                                          ! 1.1JAS
   ENDIF                                                             ! 1.1JAS
   
   IF REWKF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.5BG
      REWKF.OPEN.FLAG$ = "N"                                         ! 1.5BG
      CLOSE REWKF.SESS.NUM%                                          ! 1.5BG
   ENDIF                                                             ! 1.5BG
   
   IF IDF.OPEN.FLAG$="Y" THEN BEGIN                                  ! 1.5BG
      CLOSE IDF.SESS.NUM%                                            ! 1.5BG
      IDF.OPEN.FLAG$="N"                                             ! 1.5BG
   ENDIF                                                             ! 1.5BG
      
   IF IMSTC.OPEN.FLAG$="Y" THEN BEGIN                                ! 1.5BG
      CLOSE IMSTC.SESS.NUM%                                          ! 1.5BG
      IMSTC.OPEN.FLAG$="N"                                           ! 1.5BG
   ENDIF                                                             ! 1.5BG
      
   IF STOCK.OPEN.FLAG$="Y" THEN BEGIN                                ! 1.5BG
      CLOSE STOCK.SESS.NUM%                                          ! 1.5BG
      STOCK.OPEN.FLAG$="N"                                           ! 1.5BG
   ENDIF                                                             ! 1.5BG
   
   IF RB.OPEN.FLAG$ = "Y" THEN BEGIN                                 ! 1.5BG
      CLOSE RB.SESS.NUM%                                             ! 1.5BG
      RB.OPEN.FLAG$ = "N"                                            ! 1.5BG
   ENDIF                                                             ! 1.5BG

   SB.ACTION$ = "C"
   SB.STRING$ = ""
   IF MATCH(RECEIVE.STATE$,"wxyz",1) > 0 THEN BEGIN              ! 1.1JAS
      SB.INTEGER% = SSPSCTRL.SESS.NUM%                           ! 1.1JAS
      GOSUB SB.FILE.UTILS                                        ! 1.1JAS
      SB.INTEGER% = BTCS.SESS.NUM%                               ! 1.1JAS
      GOSUB SB.FILE.UTILS                                        ! 1.1JAS
   ENDIF                                                         ! 1.1JAS
   
   IF MATCH(RECEIVE.STATE$,"12",1) > 0 THEN BEGIN                ! 1.5BG
      SB.INTEGER% = REWKF.SESS.NUM%                              ! 1.5BG
      GOSUB SB.FILE.UTILS                                        ! 1.5BG
   ENDIF                                                         ! 1.5BG
   
   IF MATCH(RECEIVE.STATE$,"345",1) > 0 THEN BEGIN               ! 1.5BG
      SB.INTEGER% = RB.SESS.NUM%                                 ! 1.5BG
      GOSUB SB.FILE.UTILS                                        ! 1.5BG
   ENDIF                                                         ! 1.5BG

    IF RECEIVE.STATE$ = "s" OR RECEIVE.STATE$ = "w" THEN BEGIN       ! 1.1JAS
       GOSUB INITIALISE
       COUNT.IN.PROGRESS% = TRUE                                     
       GOSUB OPEN.FILES
    ENDIF

   IF BUFFER.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 1.5BG
      CLOSE BUFFER.SESS.NUM%                                        ! 1.4CS
      SB.INTEGER% = BUFFER.SESS.NUM%                                ! 1.4CS
      GOSUB SB.FILE.UTILS                                           ! 1.4CS
      BUFFER.OPEN.FLAG$ = "N"                                       ! 1.5BG
   ENDIF                                                            ! 1.5BG
      
RETURN


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***   L O W   L E V E L   S U B R O U T I N E S                            ***
\***                                                                        ***
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

    RC% = SESS.NUM.UTILITY(SB.ACTION$,SB.INTEGER%,SB.STRING$ )

    IF RC% <> 0 THEN BEGIN
       SB.EVENT.NO% = 48
       SB.UNIQUE$ = FN.Z.PACK(STR$(SB.FILE.SESS.NUM%), 10)
       SB.MESSAGE$ = "SESSION NUMBER ALLOCATION ROUTINE FAILED"
       GOSUB SB.LOG.AN.EVENT
       GOTO MODULE.EXIT
    ENDIF

    IF SB.ACTION$ = "O" THEN \
       SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
    IF SB.ACTION$ = "R" OR SB.ACTION$ = "C" THEN BEGIN
       SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
       SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) + \
                  CHR$(SHIFT(SB.FILE.REP.NUM%,0))
    ENDIF

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

    PORT.STRING$ = "PORT : " + MONITORED.PORT$ + "  "
    PORT.EVENT% = 75

    RC% = APPLICATION.LOG(MESSAGE.NO%, PORT.STRING$, "", PORT.EVENT%)

    RC% = APPLICATION.LOG(MESSAGE.NO%, SB.UNIQUE$, "", SB.EVENT.NO%)

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
\*** FILE.NOT.FOUND:
\***
\******************************************************************************

FILE.NOT.FOUND:

    IF SB.INTEGER% EQ PRINT.REPORT.NUM% THEN BEGIN                   ! 1.1JAS
       GOTO NO.PRINT.FILE                                            ! 1.1JAS
    ENDIF                                                            ! 1.1JAS

    MESSAGE.NO% = 501
    EVENT.NO% = 106
    VAR.STRING.1$ EQ FILE.OPERATION$ + \
       "O" + CHR$(SHIFT(SB.INTEGER%,8)) + \ ! Two byte integer     ! 1.1JAS
       CHR$(SHIFT(SB.INTEGER%,0)) + \ ! byte order reversed
       CURRENT.CODE$
    VAR.STRING.2$ EQ RIGHT$("000" + STR$(SB.INTEGER%),3)
    CALL APPLICATION.LOG (MESSAGE.NO%, VAR.STRING.1$,VAR.STRING.2$, EVENT.NO%)

    GOTO MODULE.EXIT

RETURN



\******************************************************************************
\***
\***  READ.SSPSCTRL.ERROR:
\***
\******************************************************************************

READ.SSPSCTRL.ERROR:                                                 ! 1.1JAS

    MESSAGE.NO% = 514                                                ! 1.1JAS
    EVENT.NO% = 106                                                  ! 1.1JAS

    VAR.STRING.1$ EQ FILE.OPERATION$ + \                             ! 1.1JAS
       "R" + CHR$(SHIFT(SSPSCTRL.REPORT.NUM%,8)) + \ ! Two byte integer ! 1.1JAS
       CHR$(SHIFT(SSPSCTRL.REPORT.NUM%,0)) + \ ! byte order reversed ! 1.1JAS
       PACK$("000000")                        ! to give hex number   ! 1.1JAS

    VAR.STRING.2$ EQ \                                               ! 1.1JAS
       RIGHT$("000" + STR$(SSPSCTRL.REPORT.NUM%),3) + \              ! 1.1JAS
      " read SSPSCTRL"                                               ! 1.1JAS
    CALL APPLICATION.LOG(MESSAGE.NO%, VAR.STRING.1$, VAR.STRING.2$, EVENT.NO%) ! 1.1JAS

RETURN                                                               ! 1.1JAS

\******************************************************************************
\***
\***   CREATE.ERROR:
\***
\***      log an event 6 (create error)
\***
\******************************************************************************

CREATE.ERROR:

    SB.EVENT.NO% = 106                                                  ! 1.5BG
    SB.UNIQUE$ = "C" + CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \ 1.9SH
                       CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \ 1.9SH
                       STRING$(7,CHR$(0))                               ! 1.9SH
    GOSUB SB.LOG.AN.EVENT                                               ! 1.5BG

    GOTO MODULE.EXIT
    
    
\******************************************************************************
\***
\***   OPEN.ERROR:
\***
\***      Log an event 6 (open error)
\***
\******************************************************************************

OPEN.ERROR:                                                             ! 1.5BG

    SB.EVENT.NO% = 106                                                  ! 1.5BG
    SB.UNIQUE$ = "O" + CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \ 1.9SH
                       CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \ 1.9SH
                       STRING$(7,CHR$(0))                               ! 1.9SH
    GOSUB SB.LOG.AN.EVENT                                               ! 1.5BG

    GOTO MODULE.EXIT                                                    ! 1.5BG


\******************************************************************************
\***
\***   WRITE.ERROR:
\***
\***      log an event 6 (write error)
\***
\******************************************************************************

WRITE.ERROR:

    SB.EVENT.NO% = 106                                                  ! 1.5BG
    SB.UNIQUE$ = "W" + CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \ 1.9SH
                       CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \ 1.9SH
                       RIGHT$(STRING$(7,CHR$(0)) + CURRENT.CODE$,7)     ! 1.9SH
    GOSUB SB.LOG.AN.EVENT                                               ! 1.5BG

    GOTO MODULE.EXIT


\******************************************************************************
\***
\***   READ.ERROR:
\***
\***      Log an event 6 (read error)
\***
\******************************************************************************

READ.ERROR:                                                             ! 1.5BG
    GOSUB READ.ERROR.SUB                                                ! 1.9SH
GOTO MODULE.EXIT                                                        ! 1.5BG
    
READ.ERROR.SUB:                                                         ! 1.9SH

    SB.EVENT.NO% = 106                                                  ! 1.5BG
    SB.UNIQUE$ = "R" + CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \ 1.9SH
                       CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \ 1.9SH
                       RIGHT$(STRING$(7,CHR$(0)) + CURRENT.CODE$,7)     ! 1.9SH
    GOSUB SB.LOG.AN.EVENT                                               ! 1.5BG

RETURN                                                                  ! 1.9SH

                                                                               
\******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***      obtain file report number and file name from session number ERRF%
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

    IF ERR = "CU" THEN RESUME                                           ! 1.9SH
    
    !Simply resume if the error is against these because                ! 1.6BG
    !it is handled in the code                                          ! 1.5BG
    IF ERRF% = IMSTC.SESS.NUM% OR \                                     ! 1.9SH
       ERRF% = STOCK.SESS.NUM% OR \                                     ! 1.9SH
       ERRF% = IDF.SESS.NUM% THEN RESUME                                ! 1.9SH
       
    IF ERRN = 80F3400CH AND ERRF% = UB.SESS.NUM% THEN BEGIN             ! 1.9SH
        IF RETRIES% > 0 THEN BEGIN                                      ! 1.9SH
            RETRIES% = RETRIES% - 1                                     ! 1.9SH
            CALL INCREMENT.SUFFIX(FILE.SUFFIX$)                         ! 1.9SH
            RESUME UB.RETRY                                             ! 1.9SH
        ENDIF                                                           ! 1.9SH
    ENDIF                                                               ! 1.9SH

    !Make pretend that RENAME simply returned a bad rc                  ! 1.9SH
    IF ERR = "FR" THEN BEGIN                                            ! 1.9SH
        RC% = 0                                                         ! 1.9SH
        RESUME                                                          ! 1.9SH
    ENDIF                                                               ! 1.9SH

    RC% = CONV.TO.HEX( ERRN )
    IF RC% <> 0 THEN GOTO MODULE.EXIT
    RC% = CONV.TO.STRING(0, ERRN )
    IF RC% <> 0 THEN GOTO MODULE.EXIT
    SB.ERRS$ = F17.RETURNED.STRING$
    SB.ERRL$ = FN.Z.PACK(STR$(ERRL), 6)
    SB.ACTION$ = "R" : SB.INTEGER% = ERRF% : SB.STRING$ = ""
    GOSUB SB.FILE.UTILS


    IF (ERRN AND 0000FFFFh) = 0000400Ch THEN BEGIN
       REP% = SB.FILE.REP.NUM%
    ENDIF

    IF ERR = "OM" THEN BEGIN
       SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ + PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))
       CALL ADXERROR(0, 74, 0, 3, 101, SB.UNIQUE$ )
       RESUME MODULE.EXIT
    ENDIF

    SB.EVENT.NO% = 101
    SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ + PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))
    GOSUB SB.LOG.AN.EVENT

    RESUME MODULE.EXIT

END SUB
