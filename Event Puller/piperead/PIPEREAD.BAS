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
    PASSED.STRING$  = "test"                              
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    DRWATSON.SESS.NUM% = F20.INTEGER.FILE.NO% 
 
    

     
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

	 
	 
	open "pi:drwatson" as 65
	
	 WHILE 1 = 1 
	
		
	
			     READ #65 ;RECORD$		
		         RECORD1$ = RECORD$
				 			
		         						 
											 
								 
							
								 
							 							    								 		
		   	  
				  
	
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
       

