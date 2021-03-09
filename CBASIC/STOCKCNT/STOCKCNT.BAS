\*******************************************************************************
\*******************************************************************************
\***
\***    PROGRAM         :       STOCCNT
\***    DESCRIPTION     :       Negative count flag reset
\***    AUTHOR          :       Ranjith Gopalankutty
\***    DATE            :       29-09-2016
 
 

    INTEGER*1       END.OF.FILE
    INTEGER*1       TRUE,FALSE
    INTEGER*1       HELP.SWITCH
    INTEGER*1       FILE.SWITCH
    INTEGER*1       ITEM.SWITCH                                             !BMW
    INTEGER*2       OPTIONS%
    INTEGER*2       MODE%                                                   !DMW
    INTEGER*4       FID%                                                    !BMW
    INTEGER*4       OFFSET%
    INTEGER*4       RECORD.COUNT%
    INTEGER*4       READ.RC%
    INTEGER*4       TERM.RC%
    STRING          FILE.NAME$
    STRING          ITEM.CODE$                                              !BMW
    STRING          COMMTAIL$
    STRING          SAVED.COMMTAIL$
    STRING          HEADER$
    STRING          PATTERN$
    STRING          RECORD$
    STRING          STOCK.RECORD$

%INCLUDE STOCKDEC.J86

%INCLUDE BTCMEM.J86
%INCLUDE KFASMEXT.J86

%INCLUDE STOCKEXT.J86

FUNCTION CHECK.DIGIT$(CODE$)
STRING CHECK.DIGIT$,CODE$
INTEGER*2 CD%,I%,V%,SUM%,CODE.LENGTH%

    CHECK.DIGIT$ = ""
    
    CODE.LENGTH% = LEN(CODE$)

    ! IF code length indicates we have an item code
    IF CODE.LENGTH% <= 6 THEN BEGIN

        SUM% = 0

        ! FOR each item code digit
        FOR I% = 1 TO CODE.LENGTH%

            ! Get digit value from (left to right)
            V% = VAL(MID$(CODE$,I%,1))

            ! Accumulate sum of digits multiplied by
            ! their position in the item code
            SUM% = SUM% + (V% * (CODE.LENGTH% - I% + 2))

        NEXT I%

        ! Calculate modulo 11 check digit
        CD% = 11 - MOD(SUM%,11)
        IF CD% = 11 THEN BEGIN
            CHECK.DIGIT$ = "0"
        ENDIF ELSE IF CD% = 10 THEN BEGIN
            CHECK.DIGIT$ = "0"
        ENDIF ELSE BEGIN
            CHECK.DIGIT$ = STR$(CD%)
        ENDIF

    ENDIF

END FUNCTION

FUNCTION STRIP.ZEROS$(FIELD$)

    STRING STRIP.ZEROS$,FIELD$,CHAR$
    STRING OLD.FIELD$

    OLD.FIELD$ = FIELD$

    ! IF field is too long to use the STR$/VAL method
    IF LEN(FIELD$) > 8 THEN BEGIN

        ! Get 1st digit from string
        CHAR$ = LEFT$(FIELD$,1)

        ! IF we have a leading zero AND
        ! we're not at the end of the string
        WHILE CHAR$ = "0" AND LEN(FIELD$) > 0

            ! IF we have more than one digit left to process
            IF LEN(FIELD$) > 1 THEN BEGIN
                ! Strip leftmost zero digit from string
                FIELD$ = RIGHT$(FIELD$,LEN(FIELD$) - 1)
                CHAR$ = LEFT$(FIELD$,1)
            ENDIF ELSE BEGIN
                ! Whole string must have been zeros!
                FIELD$ = "0"
                CHAR$  = ""
            ENDIF

        WEND

    ENDIF ELSE BEGIN

        ! Convert to integer and back to string again
        FIELD$ = STR$(VAL(FIELD$))

    ENDIF

    STRIP.ZEROS$ = FIELD$

END FUNCTION

FUNCTION STRIP.LEADING$(S$,FIELD$)

    STRING STRIP.LEADING$,S$,FIELD$,CHAR$

    ! Get 1st digit from string
    CHAR$ = LEFT$(FIELD$,1)

    ! IF we have a leading character AND
    !    we're not at the end of the string
    WHILE CHAR$ = S$ AND LEN(FIELD$) > 0

        ! IF we have more than one digit left to process
        IF LEN(FIELD$) > 1 THEN BEGIN
            ! Strip leftmost character from string
            FIELD$ = RIGHT$(FIELD$,LEN(FIELD$) - 1)
            CHAR$ = LEFT$(FIELD$,1)
        ENDIF ELSE BEGIN
            ! Whole string must have been the character!
            FIELD$ = ""
            CHAR$  = ""
        ENDIF

    WEND

    STRIP.LEADING$ = FIELD$

END FUNCTION

SUB CHECK.SWITCH(SWITCH.FOUND,SWITCH$,TAIL$)

    INTEGER*1 SWITCH.FOUND
    INTEGER*2 X%
    STRING SWITCH$,TAIL$

    SWITCH.FOUND = FALSE

    ! IF the command string contains some parameters
    IF TAIL$ <> "" THEN BEGIN

        ! Check whether switch has been specified in command tail
        X% = MATCH(SWITCH$,TAIL$,1)

        ! IF switch has been specified in the command tail
        IF X% > 0 THEN BEGIN
            ! Strip switch from remaining command string
            TAIL$ = LEFT$(TAIL$,X%-1) + \
                    MID$(TAIL$,X% + LEN(SWITCH$),LEN(TAIL$))
            SWITCH.FOUND = TRUE
        ENDIF ELSE BEGIN
            SWITCH.FOUND = FALSE
        ENDIF
    ENDIF

END SUB


FUNCTION HEX4$(H%) PUBLIC
    STRING HEX4$,E$
    INTEGER*2 I%
    INTEGER*4 H%
    E$ = ""
    FOR I% = 28 TO 0 STEP -4
        E$ = E$ + CHR$((SHIFT(H%,I%) AND 000FH) + 48)
    NEXT I%
    HEX4$ = TRANSLATE$(E$,":;<=>?","ABCDEF")
END FUNCTION 

    ON ERROR GOTO ERROR.DETECTED
	
 
    
	
	
    TRUE = -1
    FALSE = 0

    CALL STOCK.SET
    FILE.NAME$ = STOCK.FILE.NAME$
    
    PATTERN$   = ""
    OFFSET%    = 0
    MODE%      = 1                                                          !DMW

    
   

    OPTIONS%   = 201AH
    FID% = INITKF(FILE.NAME$,OPTIONS%)                                      !BMW

    ! Check for errors opening file                                         !BMW
    IF FID% <= 0 THEN BEGIN                                                 !BMW
        PRINT "INITKF ERROR = ";HEX4$(FID%)                                 !BMW
        STOP                                                                !BMW
    ENDIF                                                                   !BMW
    
    RECORD.COUNT% = 0
    
    END.OF.FILE = 0
    
    ! Continue until end of file reached
    WHILE NOT END.OF.FILE
   
 
        STOCK.RECORD$ = STRING$(STOCK.RECL%,CHR$(0))

        READ.RC% = READKF(STOCK.RECORD$,PATTERN$,OFFSET%,MODE%)             !DMW

        IF READ.RC% = 0 THEN BEGIN
		
		     
                
            STOCK.BOOTS.CODE$      = LEFT$(STOCK.RECORD$,4)
            STOCK.STOCK.FIG%       = GETN2(STOCK.RECORD$,4)
            STOCK.LAST.COUNT%      = GETN2(STOCK.RECORD$,6)
            STOCK.DATE.LAST.COUNT$ = UNPACK$(MID$(STOCK.RECORD$,9,3))
            STOCK.DATE.LAST.MOVE$  = UNPACK$(MID$(STOCK.RECORD$,12,3))
            STOCK.LAST.REC%        = GETN2(STOCK.RECORD$,14)
            STOCK.DATE.LAST.REC$   = UNPACK$(MID$(STOCK.RECORD$,17,3))
            STOCK.DATE.LAST.GAP$   = UNPACK$(MID$(STOCK.RECORD$,20,3))
            STOCK.SID%             = GETN4(STOCK.RECORD$,22)
            STOCK.STATUS.1$        = MID$(STOCK.RECORD$,27,1)
            STOCK.FLAGS%           = ASC(MID$(STOCK.RECORD$,28,1))
            STOCK.FILLER$          = UNPACK$(MID$(STOCK.RECORD$,29,2))			!BMW
			
			  
			 

            IF ASC(STOCK.STATUS.1$) = 20H THEN BEGIN
                STOCK.STATUS.1$ = "A"
            ENDIF ELSE IF ASC(STOCK.STATUS.1$) < ASC("A") THEN BEGIN
                STOCK.STATUS.1$ = "?"
            ENDIF
            
            RECORD$ = UNPACK$(STOCK.BOOTS.CODE$) + "," + \
                      STR$(STOCK.STOCK.FIG%)     + "," + \
                      STR$(STOCK.LAST.COUNT%)    + "," + \
                      STOCK.DATE.LAST.COUNT$     + "," + \
                      STOCK.DATE.LAST.MOVE$      + "," + \
                      STR$(STOCK.LAST.REC%)      + "," + \
                      STOCK.DATE.LAST.REC$       + "," + \
                      STOCK.DATE.LAST.GAP$       + "," + \
                      STR$(STOCK.SID%)           + "," + \
                      CHR$(22H)                  +       \
                      STOCK.STATUS.1$            +       \
                      CHR$(22H)                  + "," + \                  !CMW
                      STR$(STOCK.FLAGS%)                                    !CMW
    
            IF RECORD.COUNT% = 0 THEN BEGIN
            
                HEADER$ =  "Item Code"             + "," + \
                           "Stock Qty"             + "," + \
                           "Last Count"            + "," + \
                           "Last Count Date"       + "," + \
                           "Last Movement Date"    + "," + \
                           "Last Received Qty"     + "," + \
                           "Last Received Date"    + "," + \
                           "Last Gap Date"         + "," + \
                           "Sequence ID"           + "," + \
                           "Item Status"           + "," + \                !CMW
                           "Stock flags"                                    !CMW
                           
                PRINT HEADER$
            
            ENDIF
            
              PRINT RECORD$	
			
			 IF STOCK.STOCK.FIG% > 0 AND VAL(STR$(STOCK.FLAGS%)) = 1 THEN BEGIN	 
			 
				 COUNT% = COUNT% + 1	

                    PRINT RECORD$				 
				 
				 
				 STOCK.DATE.LAST.COUNT$ = PACK$(STOCK.DATE.LAST.COUNT$)
				 STOCK.DATE.LAST.MOVE$  = PACK$ (STOCK.DATE.LAST.MOVE$)
				 STOCK.DATE.LAST.REC$   = PACK$(STOCK.DATE.LAST.REC$)
				 STOCK.DATE.LAST.GAP$   = PACK$(STOCK.DATE.LAST.GAP$)
				 STOCK.FLAGS% =  0H 
				 STOCK.STATUS.1$        = MID$(STOCK.RECORD$,27,1) 
				 STOCK.FILLER$ = PACK$(STOCK.FILLER$)
				 STOCK.SESS.NUM% = 64
				 OPEN STOCK.FILE.NAME$ KEYED RECL STOCK.RECL% AS STOCK.SESS.NUM% 			  
				 
				 RC% = WRITE.STOCK
				 
				 
				 
				 IF RC% = 0 THEN BEGIN
				 
				     FIX% = FIX% + 1
					 
					  
					 
					 					 
				 ENDIF
				 
				 CLOSE STOCK.SESS.NUM%
			 ENDIF
			  
			  
			  IF STOCK.STOCK.FIG% < 0 AND VAL(STR$(STOCK.FLAGS%)) = 1 THEN BEGIN 
			     
			    NEG.COUNT% = NEG.COUNT% + 1
			  ENDIF
		    		
            
            RECORD.COUNT% = RECORD.COUNT% + 1

        ENDIF ELSE BEGIN
		
		 
        
            ! IF end of file has been reached
            IF READ.RC% = -1 THEN BEGIN
                END.OF.FILE = TRUE
            ENDIF ELSE BEGIN

            ENDIF 
            
        ENDIF
        
    WEND
    
 
    TERM.RC% = TERMKF(FID%)                                                 !BMW

    IF TERM.RC% < 0 THEN BEGIN                                              !BMW
        PRINT "TERMKF ERROR = ";HEX4$(TERM.RC%)                             !BMW
    ENDIF                                                                   !BMW
    
	
	PRINT "READ" ; RECORD.COUNT%
	PRINT "FOUND" ,   COUNT% , "NEGATIVES"
	PRINT "FIXED" ; FIX%
	PRINT "NEGATIVE" ;NEG.COUNT%
	 
    STOP
        
ERROR.DETECTED:

    PRINT "ERROR ";ERR;HEX4$(ERRN);" AT LINE ";ERRL

    END

