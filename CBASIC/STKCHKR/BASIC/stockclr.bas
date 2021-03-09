\**********************************************************************
\* 
\* STOCKCLR.286               David Artiss                  01/08/2012
\*
\* Zero the stock figure by creating STKMQ records for each STOCK
\* item.
\*
\* Pass a parameter of NFM to output all details to a single line.
\*
\* Files read    - STOCK
\* Files created - STCOR
\*
\**********************************************************************

    %INCLUDE STCORDEC.J86      
   
    STRING    ADXSERVE.DATA$,         \
              FILE.NAME$,             \
              FILLER$,                \
              FULL.RECORD$,           \
              INPUT.FILE$,            \
              ITEM.CODE$,             \
              PARAMETERS$(1),         \
              RECORD$,                \
              STORE.NUMBER$
              
    INTEGER*1 ALL,                    \
              FALSE,                  \
              NFM,                    \
              PROCESS,                \
              TRUE               

    INTEGER*2 MAX.PARAS%,             \
              NEXT.SPACE%,            \
              PARA.NO%,               \
              RECL%,                  \
              SESS.NUM%,              \
              SPACE.POS%
                     
    INTEGER*4 LOOP%,                  \
              NUMBER.OF.RECORDS%,     \
              RC%,                    \
              RECORD.LOOP%,           \
              RECORDS%

    %INCLUDE ADXSERVE.J86                            
    %INCLUDE ADXSTART.J86          
    %INCLUDE STCOREXT.J86
    %INCLUDE PSBF24E.J86    !STANDARD ERROR DETECTED        
    
    ON ERROR GOTO ERROR.DETECTED
    
    GOSUB INITIALISATION
    
    GOSUB MAIN.PROCESSING
    
    GOSUB TERMINATION
             
PROGRAM.EXIT:

    STOP
    
\**********************************************************************
\* 
\* INITIALISATION:
\* Set up and open the files. Define default variable values. 
\* 
\**********************************************************************    
    
INITIALISATION:

    IF END # 1 THEN NO.OK.FILE
    OPEN "ADXLXACN::C:\STOCKCLR.OK" AS 1
    DELETE 1
    
    NO.OK.FILE:
    
    IF END # 1 THEN CREATE.ERROR.FILE
    CREATE "ADXLXACN::C:\STOCKCLR.ERR" AS 1
    
    CLOSE 1
    
    TRUE = -1
    FALSE = 0
    
    ALL = FALSE
    NFM = FALSE
    PROCESS = FALSE
    INPUT.FILE$ = ""

    GOSUB EXTRACT.PARAMETERS
    
    GOSUB CHECK.PARAMETERS    

    CALL STCOR.SET
    
    STCOR.SESS.NUM% = 1
    
    SESS.NUM% = 2
    FILE.NAME$ = "STOCK"
    RECL% = 30
     
    IF NOT ALL THEN BEGIN
        IF END # SESS.NUM% THEN NO.INPUT.FILE
        OPEN INPUT.FILE$ AS SESS.NUM%
    ENDIF

    IF NOT NFM AND ALL THEN PRINT "Creating a Stock Correction " +      \
                                  "(STCOR) file."
    
    RECORDS% = 0
    
    CREATE STCOR.FILE.NAME$ AS STCOR.SESS.NUM%    
    
    CALL ADXSERVE(RC%,4,0,ADXSERVE.DATA$)  
    STORE.NUMBER$ = LEFT$(ADXSERVE.DATA$,4)
    
RETURN

\**********************************************************************
\* 
\* MAIN.PROCESSING:
\* Sequentially read the STOCK file and create a STKMQ record for any
\* found. Plus add up the value of the stock.
\* 
\********************************************************************** 

MAIN.PROCESSING:

    GOSUB ADD.STCOR.HEADER
    
    IF ALL THEN BEGIN
        GOSUB PROCESS.ALL.STOCK
    ENDIF ELSE BEGIN
        GOSUB PROCESS.INPUT.FILE
    ENDIF
    
    GOSUB ADD.STCOR.TRAILER

RETURN

\**********************************************************************
\* 
\* PROCESS.INPUT.FILE:
\* Extract item codes from input file and add them to STCOR.
\* 
\********************************************************************** 

PROCESS.INPUT.FILE:

    WHILE 1=1
    IF END #SESS.NUM% THEN END.OF.INPUT.FILE
        READ #SESS.NUM%; RECORD$
        ITEM.CODE$ = PACK$(LEFT$("00000000",8-LEN(RECORD$)) + RECORD$)
        GOSUB ADD.STCOR.RECORD
        IF NOT NFM THEN PRINT "Item code " + UNPACK$(ITEM.CODE$) +      \
                              " added to STCOR."
    WEND
        
    END.OF.INPUT.FILE:

RETURN

\**********************************************************************
\* 
\* PROCESS.ALL.STOCK:
\* Sequentially read the STOCK file and create a STCOR record for any
\* found.
\* 
\********************************************************************** 

PROCESS.ALL.STOCK:

    IF END #SESS.NUM% THEN ERROR.DETECTED
    OPEN FILE.NAME$ DIRECT RECL 512 AS SESS.NUM% BUFFSIZE 32768         \
                    NOWRITE NODEL
                    
    LOOP% = 2
    NUMBER.OF.RECORDS% = ROUND(508/RECL%,0,-1)
    
    WHILE 1 = 1
       IF END #SESS.NUM% THEN END.OF.KEYED.FILE
       READ FORM "C4,C508"; #SESS.NUM%,LOOP%;FILLER$,FULL.RECORD$
       
       FOR RECORD.LOOP% = 1 TO NUMBER.OF.RECORDS% 
          RECORD$ = MID$(FULL.RECORD$,((RECORD.LOOP%-1)*RECL%)+1,RECL%)
          IF RECORD$ = STRING$(RECL%,CHR$(0)) THEN BEGIN
          ENDIF ELSE BEGIN
              ITEM.CODE$ = LEFT$(RECORD$,4)
              GOSUB ADD.STCOR.RECORD
          ENDIF
       NEXT RECORD.LOOP%
       
       LOOP% = LOOP% + 1
    WEND
    
    END.OF.KEYED.FILE:
    CLOSE SESS.NUM%
    
RETURN

\**********************************************************************
\* 
\* ADD.STCOR.HEADER:
\* Write a header to STCOR
\* 
\********************************************************************** 

ADD.STCOR.HEADER: 

    STCOR.ITEM.CODE$ = PACK$("00000000")
    STCOR.SERIAL.NO$ = PACK$("001341")
    STCOR.STORE.NO$  = STORE.NUMBER$
    STCOR.RUN.TYPE$  = "Z"

    WRITE FORM "C4,C3,C4,C1,C8"; #STCOR.SESS.NUM%;                      \   
                                  STCOR.ITEM.CODE$,                     \       
                                  STCOR.SERIAL.NO$,                     \
                                  STCOR.STORE.NO$,                      \
                                  STCOR.RUN.TYPE$,                      \
                                  STCOR.HDR.FILLER$
                                   
RETURN

\**********************************************************************
\* 
\* ADD.STCOR.RECORD:
\* Write a record to STCOR
\* 
\********************************************************************** 

ADD.STCOR.RECORD: 

    STCOR.ITEM.CODE$   = ITEM.CODE$
    STCOR.STMVT.DATE$  = PACK$(DATE$)
    STCOR.SALES.QUANT% = 0
    STCOR.STMVT.QUANT% = 0
    STCOR.STOCK.COUNT% = 0
    STCOR.STKCNT.FLAG$ = "N"
    
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

\**********************************************************************
\* 
\* ADD.STCOR.TRAILER:
\* Write a trailer to STCOR
\* 
\********************************************************************** 

ADD.STCOR.TRAILER: 

    STCOR.ITEM.CODE$ = PACK$("09999999")
    STCOR.REC.COUNT% = RECORDS% + 2
                                  
    WRITE FORM "C4,I4,C12"; #STCOR.SESS.NUM%;                           \   
                             STCOR.ITEM.CODE$,                          \       
                             STCOR.REC.COUNT%,                          \
                             STCOR.TRL.FILLER$
RETURN


\**********************************************************************
\* 
\* TERMINATION:
\* Close files and output a final message. 
\* 
\********************************************************************** 

TERMINATION:    

    IF NOT ALL THEN CLOSE SESS.NUM%
    
    CLOSE STCOR.SESS.NUM%     
    
    PRINT "STCOR created successfully - " + STR$(RECORDS% + 2) +        \
          " records.";
                                                        
    IF PROCESS THEN BEGIN
        RC% = ADXSTART("ADX_UPGM:PSS42.286", +                          \
                       "", +                                            \
                       "Processing stock corrections") 
        PRINT " PSS42 started in background screen."
    ENDIF
    
    RC% = RENAME("ADXLXACN::C:\STOCKCLR.OK","ADXLXACN::C:\STOCKCLR.ERR")
    
RETURN    

\**********************************************************************
\*
\* EXTRACT.PARAMETERS:
\* Extract parameters from the input string and place in an array.
\* Each parameter must have a space as a seperator for this to work.
\*
\* No need to pass any parameters (uses COMMAND$).
\* Returns an array named PARAMETERS$.
\* PARA.NO% holds the number of parameters extracted into the array.
\*
\**********************************************************************
    
EXTRACT.PARAMETERS:

    MAX.PARAS% = 99

    DIM PARAMETERS$(MAX.PARAS%)
    PARA.NO% = 1
    SPACE.POS% = 0

    WHILE MATCH(" ",COMMAND$,SPACE.POS%+1) > 0
       NEXT.SPACE% = MATCH(" ",COMMAND$,SPACE.POS%+1)
       PARAMETERS$(PARA.NO%) = MID$(COMMAND$,SPACE.POS%+1,              \
                                    NEXT.SPACE%-SPACE.POS%-1)
       IF PARAMETERS$(PARA.NO%) <> " " THEN BEGIN
          PARA.NO% = PARA.NO% + 1
          IF PARA.NO% > MAX.PARAS% THEN BEGIN
             PRINT "Too many parameters - " + STR$(MAX.PARAS%) +        \
                   " is the maximum."
             GOTO PROGRAM.EXIT
          ENDIF
       ENDIF
       SPACE.POS% = NEXT.SPACE%
    WEND

    PARAMETERS$(PARA.NO%) = MID$(COMMAND$,SPACE.POS%+1,                 \
                                 LEN(COMMAND$)-SPACE.POS%)

RETURN

\**********************************************************************
\* 
\* CHECK.PARAMETERS:
\* Read through captured parameters and assign to specific flags 
\* depending on what is found.
\* 
\********************************************************************** 

CHECK.PARAMETERS:

    FOR LOOP% = 1 TO PARA.NO%
    
        IF PARAMETERS$(LOOP%) = "NFM" THEN BEGIN
            NFM = TRUE
        ENDIF ELSE BEGIN
            IF PARAMETERS$(LOOP%) = "ALL" THEN BEGIN
                ALL = TRUE
            ENDIF ELSE BEGIN
                IF PARAMETERS$(LOOP%) = "PROCESS" THEN BEGIN
                    PROCESS = TRUE
                ENDIF ELSE BEGIN
                    INPUT.FILE$ = PARAMETERS$(LOOP%)
                ENDIF
            ENDIF
        ENDIF
    
    NEXT LOOP%
    
    IF NOT ALL AND INPUT.FILE$ = "" THEN BEGIN
        PRINT "ALL or filename parameter not supplied."
        GOTO PROGRAM.EXIT
    ENDIF

RETURN

NO.INPUT.FILE:

    PRINT "Input file of " + INPUT.FILE$ + " does not exist."
    GOTO PROGRAM.EXIT

RETURN

CREATE.ERROR.FILE:

    PRINT "Could not create an error file."
    GOTO PROGRAM.EXIT

RETURN

\**********************************************************************
\* 
\* ERROR.DETECTED:
\* Report an error and write to log.
\* 
\********************************************************************** 

ERROR.DETECTED:
   
   PRINT "Program abended. Check application log for details."
   
   CALL STANDARD.ERROR.DETECTED(ERRN,ERRF%,ERRL,ERR)

END
