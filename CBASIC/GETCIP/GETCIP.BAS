\************************************************************************
\***
\***    GETCIP            Ranjith Gopalankutty          05/02/2019        
\***                                                                     
\***    This utility is written to read IDF completely and write all  
\***    the item codes which has mark down set on, tried to use 
\***    existing programs, there isn't any and IDFPM is extremely   
\***    time consuming, hence new program.
    

\*********************************************************************** 
     
%INCLUDE BEMFDEC.J86
%INCLUDE BOOTSDEC.J86
%INCLUDE PSBF01G.J86
%INCLUDE PSBF20G.J86
%INCLUDE IDFDEC.J86
 
\***********************************************************************
\*
\*    Global variable definitions
\*
\***********************************************************************

STRING     GLOBAL        \
     FILE.OPERATION$,    \
     PINFO.TABLE$,       \
     PASSED.STRING$,     \
     SVC.PARAM$          

INTEGER*4  GLOBAL        \
   ADX.FUNCTION%,        \
   ADX.PARAM%,           \
   CURRENT.REPORT.NUM%,  \
   EVENT.NO%,            \
   PASSED.INTEGER%,      \
   RC%
 
STRING                  \
    GETCIP.LOG.NAME$,   \
    GETCIP.OK.NAME$,    \
    FUNCTION.FLAG$,     \
    FILE.NAME$,         \
    PATTERN$,           \
    FILE.RECORD$

INTEGER*4               \
    CIPVALUE%,         \ 
    GETCIP.OK.SESS%,    \
    GETCIP.SESS.NUM%,   \
    FILE.RECL%,         \
    OPTIONS%,           \
    OFFSET%,            \
    MODE%,              \
    FID%,               \
    RECORD.COUNT%,      \
    READKF.RC%,         \
    TERMKF.RC%
 
INTEGER*1              \
   END.OF.FILE%,       \
   ERROR.COUNT%

\***********************************************************************
\*
\*    Included code defining external Boots functions
\*
\***********************************************************************

%INCLUDE PSBF01E.J86 
%INCLUDE PSBF20E.J86 
%INCLUDE PSBF24E.J86
%INCLUDE BEMFEXT.J86 
%INCLUDE BOOTSEXT.J86
%INCLUDE KFASMEXT.J86
%INCLUDE BTCMEM.J86
%INCLUDE IDFEXT.J86

ON ERROR GOTO ERROR.DETECTED

    PRINT  TIME.STAMP$(2)  + "-CIP check starts, hold your breath"	
    GOSUB INITIALISATION 
    PRINT #GETCIP.OK.SESS% ; TIME.STAMP$(2) + "-GETCIP check starts, hold your breath"    
    GOSUB IDFREADER
    GOSUB TERMINATION
 
STOP.PROGRAM:
    STOP
	
INITIALISATION:

    ERROR.COUNT% = 0
    CALL IDF.SET
    GETCIP.LOG.NAME$ 	= "ADXLXACN::D:ADX_UDT1/GETCIP.LOG"     
    GETCIP.OK.NAME$ 	= "ADXLXACN::C:/GETCIP.ERR"
    GETCIP.OK.SESS% = 101
	GETCIP.SESS.NUM% = 100
	
	 GOSUB ALLOCATE.SESSION.NUMBERS:
	
    
	 
	CREATE POSFILE GETCIP.LOG.NAME$ AS GETCIP.SESS.NUM%
 
	CREATE POSFILE GETCIP.OK.NAME$ AS GETCIP.OK.SESS%
    
 
RETURN

ALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ EQ "O"

    PASSED.INTEGER% EQ 100
    PASSED.STRING$ EQ GETCIP.LOG.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    GETCIP.SESS.NUM% EQ F20.INTEGER.FILE.NO%
	
 
    
    PASSED.INTEGER% EQ 101
    PASSED.STRING$ EQ GETCIP.OK.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    GETCIP.OK.SESS% EQ F20.INTEGER.FILE.NO%
	
 

RETURN

CALL.F20.SESS.NUM.UTILITY: 
        
    FILE.OPERATION$ = FUNCTION.FLAG$
    CURRENT.REPORT.NUM% = PASSED.INTEGER%
    RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
    IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN

IDFREADER:

    FILE.NAME$ = "IDF"
    FILE.RECL% = 60
    OPTIONS%   = 201AH
    PATTERN$   = ""
    OFFSET%    = 0
    MODE%      = 0
 
    FID% = INITKF(FILE.NAME$,OPTIONS%)

    ! Check for errors opening file
    IF FID% <= 0 THEN BEGIN
        PRINT #GETCIP.OK.SESS% ; "OPENING ERROR"
        STOP
    ENDIF

    RECORD.COUNT% = 0

    END.OF.FILE% = 0

    ! Continue until end of file reached
    WHILE NOT END.OF.FILE%

        FILE.RECORD$ = STRING$(FILE.RECL%,CHR$(0))

        READKF.RC% = READKF(FILE.RECORD$,PATTERN$,OFFSET%,MODE%)

        IF READKF.RC% = 0 THEN BEGIN

            IDF.BOOTS.CODE$        = LEFT$(FILE.RECORD$,4)		 
			IF UNPACK$(IDF.BOOTS.CODE$) <> "00000000" THEN BEGIN
			    CIPVALUE% = GETN1(FILE.RECORD$,51)				 
				IF CIPVALUE%  = 48 THEN BEGIN
				    PRINT #GETCIP.SESS.NUM%; UNPACK$(IDF.BOOTS.CODE$)
				ENDIF  
			ENDIF 
        ENDIF ELSE BEGIN 
  
            IF READKF.RC% = -1 THEN BEGIN
                END.OF.FILE% = -1
            ENDIF ELSE BEGIN
            ! Check whether an error has been returned
                IF READKF.RC% < -1 THEN BEGIN
                    PRINT #GETCIP.OK.SESS% ; "IDF READ ERROR " 
                    STOP
                ENDIF
            ENDIF                                

        ENDIF

    WEND

    TERMKF.RC% = TERMKF(FID%)

    IF TERMKF.RC% < 0 THEN BEGIN
        PRINT #GETCIP.OK.SESS% ; "File close failed"
    ENDIF
 
RETURN

TERMINATION:

    IF RECORD.COUNT% = 0 THEN BEGIN
        PRINT #GETCIP.OK.SESS% ; TIME.STAMP$(2) + "-GETCIP completed successfully"
    ENDIF      
     
 
RETURN

ERROR.DETECTED:

    ERROR.COUNT% = ERROR.COUNT%  + 1 
    PRINT #GETCIP.OK.SESS% ; ERR
    PRINT #GETCIP.OK.SESS% ; ERRF%
    PRINT #GETCIP.OK.SESS% ; ERRL
