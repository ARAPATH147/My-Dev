\************************************************************************
\***
\***    DRWATSON          Ranjith Gopalankutty          22/03/2018  
\***                                                                
\***    This program will read OS pipe ADXCSOUP and start dumping   
\***    the data to D:/ADX_UDT1/DRWATSON.BIN                          
\***                                                                
\***    From DRWATSON.BIN , C program named SHERHOLME will start      
\***    putting the data in to the socket                           
\***                                                                
\***                                                                

\***********************************************************************
\***
\***    Function globals
\***
\***********************************************************************
%INCLUDE BEMFDEC.J86  
%INCLUDE BOOTSDEC.J86 
%INCLUDE PSBF01G.J86  
%INCLUDE PSBF20G.J86

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
   EVENT.DATE%,          \
   RC%

\***********************************************************************
\*
\*    Local Variables
\*
\***********************************************************************

STRING                    \
     ADX.COMMAND$,        \
     COMM.TAIL$,          \
     CURRENT.CODE$,       \
     FLAG$,               \
     FUNCTION.FLAG$,      \
     OK.STRING$,          \
     PROG.NAME$,          \
     TEMP.ARR$(1),        \
     TEMP.FORM$,          \
     VAR.STRING.1$,       \
     VAR.STRING.2$
                           
INTEGER*4                 \
     ADX.INTEGER%,        \
     ARR.COUNT%,          \
     I%,                  \
     EOF%,                \
     ERROR.CNT%,          \
     K%,                  \
     FOUND%,              \
     FOUND1%,             \
     J%,                  \
     MESSAGE.NO%,         \
     RPS%,                \
     RC1%,                \
     TEMP.COUNT%  



\***********************************************************************
\*
\*    Included code defining external Boots functions
\*
\***********************************************************************
    
%INCLUDE BEMFEXT.J86
%INCLUDE BASROUT.J86
%INCLUDE BOOTSEXT.J86
%INCLUDE BTCMEM.J86
%INCLUDE PSBF01E.J86 
%INCLUDE PSBF20E.J86 
%INCLUDE PSBF24E.J86

\***********************************************************************
\*
\*    INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************

FUNCTION FLEXOSSVC(COMMAND%, PARAM$) EXTERNAL                           
                                                                        
      STRING PARAM$                                                     
      INTEGER*2 COMMAND%, FLEXOSSVC                                     
                                                                        
END FUNCTION  

FUNCTION HEX.TEXT$(H$) PUBLIC                                                                                                  
                                                                        
    STRING HEX.TEXT$,H$                                                
                                                                       
    HEX.TEXT$ = TRANSLATE$(H$,":;<=>?","ABCDEF")                       
                                                                       
END FUNCTION                                                           
\**********************************************************************

SUB ADXSERVE (RET, FUNC, PARM1, PARM2$) EXTERNAL

    INTEGER*4  RET
    INTEGER*2  FUNC,PARM1
    STRING     PARM2$

END SUB

\**********************************************************************
\***
\***    ADXCOPYF
\***    IBM routine to request Application Services.
\***
\**********************************************************************

SUB ADXCOPYF (RETC,INFILE,OUTFILE,OPT0,OPT1) EXTERNAL

    INTEGER*4 RETC
    STRING    INFILE,OUTFILE
    INTEGER*2 OPT0,OPT1

END SUB 

\***********************************************************************
\*                                                                     *
\*          S T A R T   O F   M A I N L I N E   C O D E                *
\*                                                                     *
\***********************************************************************
\***********************************************************************

ON ERROR GOTO ERROR.DETECTED         

    DRWATSON.FILE.NAME$ = "D:/ADX_UDT1/DRWATSON.DAT"
	DRWATSON.BIN.NAME$ = "D:/ADX_UDT1/DRWATSON.BIN"
	EVLOG.FILE.NAME$ = "D:/ADX_IDT1/EVLOG00"
	GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB MAIN.PROCESSING
	   
STOP.PROGRAM:
    
	STOP

\***********************************************************************
\*
\*    ALLOCATE.SESSION.NUMBERS:   
\*                
\*
\***********************************************************************
ALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ = "O"                                                
    
    PASSED.INTEGER% = 456                              
    PASSED.STRING$  = DRWATSON.FILE.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    DRWATSON.SESS.NUM% = F20.INTEGER.FILE.NO% 
 
    PASSED.INTEGER% = 457                             
    PASSED.STRING$  = DRWATSON.BIN.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    DRWATSON.BIN.NUM% = F20.INTEGER.FILE.NO%  	
	
	PASSED.INTEGER% = 458                             
    PASSED.STRING$  = EVLOG.FILE.NAME$                               
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    EVLOG.SESS.NUM% = F20.INTEGER.FILE.NO% 

     
RETURN

\***********************************************************************
\*
\*    CALL.F20.SESS.NUM.UTILITY:  
\*                
\*
\***********************************************************************
CALL.F20.SESS.NUM.UTILITY: 
        
   FILE.OPERATION$ = FUNCTION.FLAG$
   CURRENT.REPORT.NUM% = PASSED.INTEGER%
   RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
   IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN 
 
\***********************************************************************
\*
\*    MAIN.PROCESSING
\*
\***********************************************************************
MAIN.PROCESSING:   

	 PRINT "Real Time Event Generator is running" 

	 OPEN EVLOG.FILE.NAME$ DIRECT RECL 36 AS EVLOG.SESS.NUM% 	
	 
	 OPEN DRWATSON.FILE.NAME$ AS DRWATSON.SESS.NUM% LOCKED NODEL APPEND	
 
	 OPEN DRWATSON.BIN.NAME$ DIRECT RECL 4 AS DRWATSON.BIN.NUM% NODEL
	 
	 CREATE "pi:drwatson" as 64 BUFFSIZE 57  
     I% = 1
	
	 WHILE 1 = 1 
	
		 RC% = SIZE(EVLOG.FILE.NAME$)
	
		 NUM.RECORDS% = RC% / 36
		 
		 IF RC% > 0  THEN BEGIN	
	
			 WHILE I% <  NUM.RECORDS%	    
        
			     READ FORM "C36" ;#EVLOG.SESS.NUM%, I%;  RECORD$		
		
				 I% = I% + 1      Eve
  
				 IF RIGHT$( RECORD$,28) = STRING$(28,CHR$(0)) THEN BEGIN    
			
			 
				 ENDIF ELSE BEGIN
		
					 EVENT.DATE% =  GETN2(RECORD$,0)
					 EVENT.TIME%  = GETN2(RECORD$,2)					  
					 EVENT.NODE$  = MID$(RECORD$,5,2)	
					 EVENT.NODE$ = RIGHT$(EVENT.NODE$,1) + LEFT$(EVENT.NODE$,1) 		   
					 EVENT.TERMINAL% = GETN2(RECORD$,6)  		   
					 EVENT.SOURCE%  = ASC(MID$(RECORD$,9,1)) 
					 EVENT.GROUP$   = MID$(RECORD$,10,1)   		   
					 EVENT.NUMB% = GETN2(RECORD$,10)		   
					 EVENT.SEV%  = GETN1(RECORD$,12)   
					 LIBRARY$  = MID$(EVENT.UNIQUE.DATA$,13,1)
					 EVENT.UNIQUE$ = MID$(RECORD$,14,18)
					 
					 EVENT.TIME% = EVENT.TIME% AND 0FFE0H  
					 EVENT.DAY%   = EVENT.DATE% AND 0001FH
					 EVENT.MONTH% = SHIFT(EVENT.DATE% AND 001E0H,5)
					 EVENT.YEAR%  = SHIFT(EVENT.DATE% AND 0FE00H,9) + 1980
					 EVENT.HOUR% = SHIFT(EVENT.TIME% AND 0F800H,11)
					 EVENT.MIN%  = SHIFT(EVENT.TIME% AND 007E0H,5)	 
		                                   
				     SOCKET.STRING$ = STR$(EVENT.DAY%) + "/" + STR$(EVENT.MONTH%) + "/" + STR$(EVENT.YEAR%)  + "-" +  STR$(EVENT.HOUR%) + ":" + STR$(EVENT.MIN%) + "-" + EVENT.NODE$ + "-" +  STR$(EVENT.TERMINAL%) + \
								 STR$(EVENT.SOURCE%) + "-" + EVENT.GROUP$  + STR$(EVENT.NUMB%) + "-" + STR$(EVENT.SEV%) + "-" + \
								 UNPACK$(MID$(EVENT.UNIQUE$,1,1)) + UNPACK$(MID$(EVENT.UNIQUE$,2,1)) + UNPACK$(MID$(EVENT.UNIQUE$,3,1)) + \
								 UNPACK$(MID$(EVENT.UNIQUE$,4,1)) + 	UNPACK$(MID$(EVENT.UNIQUE$,5,1)) + 	UNPACK$(MID$(EVENT.UNIQUE$,6,1)) + \
								 UNPACK$(MID$(EVENT.UNIQUE$,7,1))  + UNPACK$(MID$(EVENT.UNIQUE$,8,1)) + MID$(EVENT.UNIQUE$,9,1)	 + \
								 MID$(EVENT.UNIQUE$,10,1) + MID$(EVENT.UNIQUE$,11,1) + MID$(EVENT.UNIQUE$,12,8)								
			
								 PRINT USING "&"; #64 ; SOCKET.STRING$

                                 ! read #64 ; read.string$
                                 ! print read.string$ 											
								 
							 							    								 		
		   	  
				 ENDIF
							
		     WEND

	     ENDIF		
	
	 WEND

	
RETURN





\***********************************************************************
\*
\*    ERROR.LOG  
\*               
\*
\***********************************************************************
ERROR.LOG: 
        
         VAR.STRING.1$ = FILE.OPERATION$                     +          \
                CHR$(SHIFT(CURRENT.REPORT.NUM%, 8) AND 0FFH) +          \
                CHR$(CURRENT.REPORT.NUM% AND 0FFH)           +          \
                CURRENT.CODE$
         VAR.STRING.2$ = "DRWATSON"
         MESSAGE.NO%   = 0
         EVENT.NO%     = 106

         RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,               \
                      VAR.STRING.2$,EVENT.NO%)

   

         GOTO END.PROGRAM

RETURN

\***********************************************************************
\*
\*    CLOSE.FILES:
\*                
\*
\***********************************************************************

CLOSE.FILES:
   
   CLOSE DRWASTON.SESS.NUM%
   
RETURN  
\***********************************************************************
\*
\*    DEALLOCATE.SESS.NUM  
\*                
\*
\***********************************************************************
DEALLOCATE.SESS.NUM: 

    FUNCTION.FLAG$ = "C"   

RETURN

ERROR.DETECTED:

   PRINT ERR
   PRINT ERRF%
   PRINT ERRL


    IF ERR= "OE" AND ERRF% = DRWATSON.SESS.NUM% THEN BEGIN
		CREATE POSFILE DRWATSON.FILE.NAME$ AS DRWATSON.SESS.NUM% 
		RESUME 
    ENDIF 
	
	IF ERR= "OE" AND ERRF% = DRWATSON.BIN.NUM% THEN BEGIN
		CREATE POSFILE DRWATSON.BIN.NAME$ DIRECT 1 RECL 4  AS DRWATSON.BIN.NUM%
		RESUME
	ENDIF 

\***********************************************************************
\*
\*    STOP.PROGRAM 
\*              
\*
\***********************************************************************

   END.PROGRAM:

   END 
       

