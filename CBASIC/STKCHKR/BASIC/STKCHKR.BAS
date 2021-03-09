 \****************************************************************************
\*  STKCLEAR      RANJITH GOPALANKUTTY               DECEMBER 2016   *
\*                                                                           *
\*  SPECIFY EITHER AN INPUT FILE OR "ALL" AS A PARAMETER AND IT WILL PRINT   *
\*  THE ITEMS STOCK LEVELS. IF "CLR" IS SPECIFIED AS A SECOND PARAMETER IT   *                                                
\*  WILL ALSO CREATE AN STCOR TO ZERO THE ITEMS IN THE STOCK FILE.           *                                                              
\*                                                                           *
\*    24/10/17 - CHANGED TO 'Z' TYPE RECORD INTRODUCED FOR RS6               *
\*****************************************************************************

INTEGER*1 A%,
          B%, 
		  TRUE, 
		  FALSE
INTEGER*2 BLOCK%,
	      I%, 
		  RC%, 
		  INPUT.SESS.NUM%, 
		  KEYSTK.SESS.NUM%, 
		  THE.END%, 
		  DIRSTK.SESS.NUM%, 
		  CLR%, 
		  CURRENT.PARAM%, 
		  COMMAND.LEN%, 
		  DIVIDE.MATCH%, \
		  PREV.DIVIDE.MATCH%, 
		  STRING.SECTION%, 
		  CHK.FILE%, 
		  PARAM1.LEN%, 
		  PARAM2.LEN%, 
		  HDR.WRTN%

INTEGER*4 RECORDS%, RET.CODE%
STRING RECORD$, TEMP$, OUTPUT$, STOCK.IC$, STOCK.SL$, INPUTFILE$, INPCODE$, STORE.NUMBER$, ADXSERVE.DATA$, FILE.NAME$, NORMAL.RUN$, PARAM$(1), \
ALL$

%INCLUDE IMSTCDEC.J86
%INCLUDE STOCKDEC.J86
%INCLUDE STCORDEC.J86
%INCLUDE IMSTCEXT.J86
%INCLUDE STOCKEXT.J86
%INCLUDE STCOREXT.J86
%INCLUDE PSBF30E.J86
%INCLUDE BTCMEM.J86
%INCLUDE ADXSERVE.J86                            
%INCLUDE ADXSTART.J86
%INCLUDE ADXCOPY.J86

FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC
           STRING RECORD$,                    \
           PROCESS.KEYED.RECORD$         
            
    ON ERROR GOTO ERROR.DETECTED
            STOCK.IC$ = UNPACK$(MID$(RECORD$,1,4))
            STOCK.SL$ = STR$(GETN2(RECORD$,4))      
            !IF STOCK.IC$ <> "00000000" THEN BEGIN 
                GOSUB IMSTC.CHECK
                   IF CLR% = 0 THEN PRINT "Item Code: " + STOCK.IC$ + "   Stock Level: " + STOCK.SL$
                   IF CLR% = 1 AND STOCK.SL$ <> "0" THEN GOSUB STCOR.MAIN              
                !        ENDIF
                
                           
END.FUNC:

    PROCESS.KEYED.RECORD$ = RECORD$  
    EXIT FUNCTION
    
IMSTC.CHECK:
IMSTC.BAR.CODE$ = PACK$(RIGHT$("000000000000000000" + LEFT$(STOCK.IC$,(LEN(STOCK.IC$) - 1)),22))                        
RC% = READ.IMSTC
RETURN

STCOR.MAIN:
    STCOR.ITEM.CODE$   = PACK$(STOCK.IC$)
    STCOR.STMVT.DATE$  = PACK$(DATE$)
    STCOR.SALES.QUANT% = 0
    STCOR.STMVT.QUANT% = 0 
    STCOR.STOCK.COUNT% = VAL(STOCK.SL$) * -1
    STCOR.STKCNT.FLAG$ = "Y"

WRITE FORM "C4,C3,3I2,C1,C6"; #STCOR.SESS.NUM%;                     \   
                                   STCOR.ITEM.CODE$,                    \       
                                   STCOR.STMVT.DATE$,                   \
                                   STCOR.SALES.QUANT%,                  \
                                   STCOR.STMVT.QUANT%,                  \
                                   STCOR.STOCK.COUNT%,                  \
                                   STCOR.STKCNT.FLAG$,                  \
                                   STCOR.DET.FILLER$
RECORDS% = RECORDS% + 1                                   
RETURN
    
   
    ERROR.DETECTED: 
    PRINT   
    PRINT "DIAGNOSTIC MESSAGES :"   
    PRINT "    FILE NUMBER IS   :",ERRF%     
    PRINT "    ERROR CODE       :",ERR     
    PRINT "    ERROR NUMBER     :",ERRN      
    PRINT "    CODE LINE        :",ERRL     
    PRINT                             
    PRINT "######## has ended in error."      
    
    RESUME END.FUNC
     
END FUNCTION
             
ON ERROR GOTO ERROR.DETECTED
    GOSUB INITIALISATION
    GOSUB MAIN.PROGRAM
    GOSUB TERMINATION
STOP

INITIALISATION:
CALL IMSTC.SET
CALL STOCK.SET
CALL STCOR.SET
CALL ADXSERVE(RC%,4,0,ADXSERVE.DATA$)
STORE.NUMBER$ = LEFT$(ADXSERVE.DATA$,4) 
STOCK.SESS.NUM% = 1
IMSTC.SESS.NUM% = 2
INPUT.SESS.NUM% = 3
DIRSTK.SESS.NUM% = 4
STCOR.SESS.NUM% = 5
CLR% = 0
TRUE = -1
FALSE = 0
A% = 1
HDR.WRTN% = 0
DIM PARAM$(2)

OPEN STOCK.FILE.NAME$ DIRECT RECL 512 AS DIRSTK.SESS.NUM%
OPEN IMSTC.FILE.NAME$ KEYED RECL IMSTC.RECL% AS IMSTC.SESS.NUM%
 
RETURN

MAIN.PROGRAM:

    COMMAND.LEN% = LEN(COMMAND$)
    IF COMMAND.LEN% > 0 THEN BEGIN
        WHILE NOT B% 
            DIVIDE.MATCH% = MATCH(" ",COMMAND$,A%)
            IF DIVIDE.MATCH% = 0 THEN BEGIN
                PARAM$(CURRENT.PARAM%) = MID$(COMMAND$,PREV.DIVIDE.MATCH% + 1,COMMAND.LEN% - DIVIDE.MATCH%)
                B% = TRUE 
             ENDIF ELSE IF NOT DIVIDE.MATCH% = 0 THEN BEGIN
                IF NOT B% = TRUE THEN BEGIN
                    STRING.SECTION% = DIVIDE.MATCH% - A%
                    PARAM$(CURRENT.PARAM%) = MID$(COMMAND$,A%,STRING.SECTION%)
                ENDIF
            ENDIF
            CURRENT.PARAM% = CURRENT.PARAM% + 1
            PREV.DIVIDE.MATCH% = DIVIDE.MATCH%
            A% = DIVIDE.MATCH% + 1
        WEND
        IF NOT B% = TRUE THEN BEGIN
            PARAM$(CURRENT.PARAM%) = MID$(COMMAND$,PREV.DIVIDE.MATCH% + 1,COMMAND.LEN% - DIVIDE.MATCH%)
        ENDIF
    ENDIF
    
    PARAM1.LEN% = LEN(PARAM$(0))
    
    IF PARAM1.LEN% = 0 THEN BEGIN
        NORMAL.RUN$ = "YES"
    ENDIF ELSE BEGIN
        IF PARAM$(0) = "ALL" THEN BEGIN
            ALL$ = "YES"
        ENDIF ELSE IF PARAM$(0) = "CLR" THEN BEGIN
            CLR% = 1
        ENDIF ELSE IF PARAM1.LEN% > 0 AND PARAM$(0) <> "ALL" AND PARAM$(0) <> "CLR" THEN BEGIN
            FILE.NAME$ = PARAM$(0)
            CHK.FILE% = 1
        ENDIF
        
    ENDIF 
    
    PARAM2.LEN% = LEN(PARAM$(1))
    IF NOT PARAM2.LEN% = 0 THEN BEGIN
        IF PARAM$(1) = "ALL" THEN BEGIN
            ALL$ = "YES"
        ENDIF ELSE IF PARAM$(1) = "CLR" THEN BEGIN
            CLR% = 1
        ENDIF ELSE IF PARAM2.LEN% > 0 AND PARAM$(1) <> "ALL" AND PARAM$(1) <> "CLR" THEN BEGIN
            FILE.NAME$ = PARAM$(1)
            CHK.FILE% = 1
        ENDIF
    ENDIF 
    
IF ALL$ = "YES" THEN BEGIN
    GOSUB CHECK.ALL
ENDIF ELSE IF CHK.FILE% = 1 THEN BEGIN
    GOSUB CHECK.FILE
ENDIF

RETURN

!If ALL is requested then it will provide the stock level of all items in stock.bin prioritising imstc stock levels where possible
CHECK.ALL:
IF CLR% = 1 THEN GOSUB STCOR.HEADER
IF PROCESS.KEYED.FILE(STOCK.FILE.NAME$,108,"N" ) THEN BEGIN
        PRINT "Unsuccesful"
ENDIF
RETURN

!If a specific filename is passed it will read through the provided item codes and provide the stock
CHECK.FILE:
    INPUTFILE$ = FILE.NAME$
    OPEN STOCK.FILE.NAME$ KEYED RECL STOCK.RECL% AS STOCK.SESS.NUM%
    THE.END% = 0 
    OPEN INPUTFILE$ AS INPUT.SESS.NUM%
    IF CLR% = 1 THEN GOSUB STCOR.HEADER 
        WHILE THE.END% = 0
            IF END # INPUT.SESS.NUM% THEN END.OF.FILE
                READ # INPUT.SESS.NUM%;INPCODE$
                IF INPCODE$ <> "" THEN BEGIN
                    INPCODE$ = RIGHT$("00000000" + INPCODE$,8)
                    IF LEN(INPCODE$) = 8 THEN BEGIN
                        STOCK.BOOTS.CODE$ = PACK$(INPCODE$)
                        RC% = READ.STOCK
                        IF NOT RC% THEN BEGIN
                                STOCK.IC$ = UNPACK$(STOCK.BOOTS.CODE$)
                                STOCK.SL$ = STR$(STOCK.STOCK.FIG%)
                               GOSUB IMSTC.CHECK
                               IF CLR% = 0 THEN PRINT "Item Code: " + STOCK.IC$ + "   Stock Level: " + STOCK.SL$
                              IF CLR% = 1 AND STOCK.SL$ <> "0" THEN GOSUB STCOR.MAIN
                         ENDIF ELSE BEGIN
                                PRINT "Item Code: " + STOCK.IC$ + " not found in stock.bin"
                 ENDIF
              ENDIF
        ENDIF        
            LOOP.END:
            WEND
    CLOSE INPUT.SESS.NUM%
    CLOSE STOCK.SESS.NUM%
RETURN

!Checks the stock level of the item in the IMSTC file
IMSTC.CHECK:
IMSTC.BAR.CODE$ = PACK$(RIGHT$("000000000000000000" + LEFT$(STOCK.IC$,(LEN(STOCK.IC$) - 1)),22))                        
RC% = READ.IMSTC

IF NOT RC% THEN BEGIN
    IF STOCK.SL$ <> STR$(IMSTC.STOCK.FIGURE%) THEN BEGIN
        STOCK.SL$ = STR$(IMSTC.STOCK.FIGURE%)
    ENDIF
ENDIF
RETURN 



!Used for looping through the input file
END.OF.FILE:
THE.END% = 1
GOTO LOOP.END:
RETURN

NO.FILE:
PRINT "INPUT FILE NOT FOUND"
STOP


!Creates the header file for STCOR
STCOR.HEADER:
    CREATE STCOR.FILE.NAME$ AS STCOR.SESS.NUM%
    STCOR.ITEM.CODE$ = PACK$("00000000")
    STCOR.SERIAL.NO$ = PACK$("001341")
    STCOR.STORE.NO$  = STORE.NUMBER$
    STCOR.RUN.TYPE$  = "R"
    RECORDS% = 0

    WRITE FORM "C4,C3,C4,C1,C8"; #STCOR.SESS.NUM%;                      \   
                                  STCOR.ITEM.CODE$,                     \       
                                  STCOR.SERIAL.NO$,                     \
                                  STCOR.STORE.NO$,                      \
                                  STCOR.RUN.TYPE$,                      \
                                  STCOR.HDR.FILLER$
    HDR.WRTN% = 1
RETURN

!Creates an item record for STCOR
STCOR.MAIN:
    STCOR.ITEM.CODE$   = PACK$(STOCK.IC$)
    STCOR.STMVT.DATE$  = PACK$(DATE$)
    STCOR.SALES.QUANT% = 0
    STCOR.STMVT.QUANT% = 0 
    STCOR.STOCK.COUNT% = 0
    STCOR.STKCNT.FLAG$ = "Z"

WRITE FORM "C4,C3,3I2,C1,C6"; #STCOR.SESS.NUM%;                     \   
                                   STCOR.ITEM.CODE$,                    \       
                                   STCOR.STMVT.DATE$,                   \
                                   STCOR.SALES.QUANT%,                  \
                                   STCOR.STMVT.QUANT%,                  \
                                   STCOR.STOCK.COUNT%,                  \
                                   STCOR.STKCNT.FLAG$,                  \
                                   STCOR.DET.FILLER$
RECORDS% = RECORDS% + 1                                   
RETURN

!Creates a trailer record for STCOR
STCOR.TRAILER:
    IF HDR.WRTN% = 1 THEN BEGIN 
        STCOR.ITEM.CODE$ = PACK$("09999999")
        STCOR.REC.COUNT% = RECORDS% + 2
                                  
        WRITE FORM "C4,I4,C12"; #STCOR.SESS.NUM%;                           \   
                             STCOR.ITEM.CODE$,                          \       
                             STCOR.REC.COUNT%,                          \
                             STCOR.TRL.FILLER$
        CLOSE STCOR.SESS.NUM%
    ENDIF

RETURN

TERMINATION:
IF CLR% = 1 THEN GOSUB STCOR.TRAILER
CLOSE IMSTC.SESS.NUM%
CLOSE DIRSTK.SESS.NUM%
IF CLR% = 1 THEN PRINT "Program complete. STCOR generated with " + STR$(STCOR.REC.COUNT% - 2) + " item records."
RETURN

           
ERROR.DETECTED:
    PRINT   
    PRINT "DIAGNOSTIC MESSAGES :"   
    PRINT "    FILE NUMBER IS   :",ERRF%     
    PRINT "    ERROR CODE       :",ERR     
    PRINT "    ERROR NUMBER     :",ERRN      
    PRINT "    CODE LINE        :",ERRL     
    PRINT                             
    PRINT "######## has ended in error."      
    END
    