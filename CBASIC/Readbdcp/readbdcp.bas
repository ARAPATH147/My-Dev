\******************************************************************************
\******************************************************************************
\***
\***     READCRTN 
\***
\***     Original version          17/08/2015            RANJITH GOPALANKUTTY
\***
\***     
\***
\******************************************************************************
\******************************************************************************
\***
\***    READCRTN
\***    ********
\***   
\***    
\***    In the past it came to AppsMgmt attention that PDT stores have issues
\***    while booking in BOOTS.COM orders due to the fact that past orders are 
\***    piled up in the CARTON.BIN file and its not able to take any new 
\***    orders.
\***    
\***    The issue is that even after an order is being collected by customer
\***    it still shows as not booked in CARTON.BIN file as a result house
\***    keeping program does not remove old entries. This utility will address 
\***    the conflicts between BDCP file and CARTON file.
\***
\***    The READCRTN program performs the following tasks ...  
\***
\***    Reads CARTON.BIN file and filter BOOTS.COM orders which are not booked
\***    and compare the status with BDCP.BIN file. if there are mismatches
\***    it changes the status of the order in CARTON.BIN file as 'Booked in
\***    by exception
\***
\******************************************************************************
\******************************************************************************
\***
\***    DEC included code defining file related fields
\***
\******************************************************************************
\******************************************************************************

    %INCLUDE BDCPDEC.J86
    %INCLUDE BOOTSDEC.J86
    %INCLUDE CBDEC.J86
    %INCLUDE CRTNDEC.J86
    %INCLUDE PSBF01G.J86
    %INCLUDE PSBF20G.J86
   
\******************************************************************************
\***
\***    Global Variable definitions
\***
\******************************************************************************


     STRING GLOBAL                                                      \
         BATCH.SCREEN.FLAG$,                                            \
         BDCP.OUTPUT.FILE$,                                             \
         CRTN.OUTPUT.FILE$,                                             \
         CURRENT.CODE$,                                                 \
         CURRENT.CODE.LOGGED$,                                          \
         ERR.FILE.NAME$,                                                \
         FILE.OPERATION$,                                               \
         FUNCTION.FLAG$,                                                \
         MODULE$,                                                       \
         MODULE.NUMBER$,                                                \
         PROGRAM$,                                                      \
         PASSED.STRING$,                                                \
         TEXT.FORMAT$,                                                  \
         VAR.STRING.1$,                                                 \
         VAR.STRING.2$,                                                 \
         CARTON.NO$,                                                    \
         CHAIN$,                                                        \
         STATUS$,                                                       \
         ORDER.NUM$,                                                    \
         ORDER.SUFFIX$,                                                 \
         BUS.CENTRE$,                                                   \
         E.D.D$,                                                        \
         ITEM.COUNT$,                                                   \
         REPEATED$,                                                     \
         FILLER$,          	                                            \
         ASN.CODE$                                                       

     INTEGER*1 GLOBAL                                                   \
         ERROR.COUNT%,                                                  \
         FALSE,                                                         \
         TRUE                                                           !
        
     INTEGER*2 GLOBAL                                                   \
         BDCP.OUTPUT.SESS%,                                             \
         BDCP.OUTPUT.SESS.NUM%,                                         \
         CRTN.OUTPUT.NUM%,                                              \
         CURRENT.REPORT.NUM%,                                           \
         ERR.SESS.NUM%,                                                 \
         EVENT.NO%,                                                     \
         MESSAGE.NO%,                                                   \
         PASSED.INTEGER%                                                !

\******************************************************************************
\***
\***    Variable definitions
\***
\******************************************************************************
        
    STRING                                    \
            LINE.RECORD$,                     \
            READCRTN.FILE.NAME$,              \
            READCRTN.RPT.FILE.NAME$,          \
            SUPPLIER$                         \


    INTEGER*1                                 \
            RC%
    
    INTEGER*2                                 \
            CONFLICT%,                        \
            COUNT%,                           \
            CRTN.OUTPUT.SESS.NUM%,            \
            ERR.CNT%,                         \
            EOF%,                             \
            FUNCTION.RETURN.CODE%,            \
            READCRTN.RPT.SESS.NUM%,           \
            READCRTN.RPT.REPORT.NUM%,         \
            READCRTN.RPT.OPEN%,               \
            CNTR%
 
\******************************************************************************
\***
\***    Included code defining function related global variables
\***
\******************************************************************************
	
	%INCLUDE PSBF01E.J86   ! APPLICATION.LOG
    %INCLUDE PSBF20E.J86   ! ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF30E.J86   ! 
    %INCLUDE PSBF24E.J86   ! STANDARD.ERROR.DETECTED
    %INCLUDE BOOTSEXT.J86  ! GENERIC BOOTS LIB
	%INCLUDE BDCPEXT.J86   ! BDCP
    %INCLUDE BTCMEM.J86
    %INCLUDE ERRNH.J86
    %INCLUDE ADXSERVE.J86  ! Controller Services
	%INCLUDE CBEXT.J86
    %INCLUDE CRTNEXT.J86
	
\******************************************************************************
\***
\***    Sub routine for writing the records
\***
\******************************************************************************	

SUB LOG.MESSAGE(F.MSG$)
STRING F.MSG$
       
  ! IF UNPACK$(SUPPLIER$) = "117838" AND STATUS$ = "U" THEN BEGIN
        PRINT USING TEXT.FORMAT$ ; #BDCP.OUTPUT.SESS% ; F.MSG$

  ! ENDIF


END SUB

\******************************************************************************
\***
\***    START.PROGRAM
\***
\******************************************************************************	
START.PROGRAM:

    ON ERROR GOTO ERROR.DETECTED


    RC% = CRTN.SET  
    RC% = BDCP.SET	

    TEXT.FORMAT$       = "&"    
    CRTN.OUTPUT.FILE$ = "C:\COUNT.OUT"
    CRTN.OUPTUT.SESS% = 150	
    BDCP.OUTPUT.FILE$ = "C:\BDCP.OUT"
    BDCP.OUTPUT.SESS% = 200  
    GOSUB ALLOCATE.SESSION.NUMBERS
    
    ERR.FILE.NAME$ = "C:\READCRTN.ERR"
    ERR.SESS.NUM% = 250

    CREATE CRTN.OUTPUT.FILE$ AS CRTN.OUTPUT.SESS%
    CREATE BDCP.OUTPUT.FILE$ AS BDCP.OUTPUT.SESS%
    CREATE ERR.FILE.NAME$ AS ERR.SESS.NUM%

    GOSUB READ.BDCP.FILE	 
	GOSUB READ.OUT.FILE
    

    PRINT  TIME.STAMP$(2) + "-Found " + STR$(COUNT%) + " BOOTS.COM Orders" 
	
	PRINT #CRTN.OUTPUT.SESS% ; "FOUND" , STR$(COUNT%)
    
    GOSUB TERMINATION

STOP

\******************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS
\***         
\***
\******************************************************************************

ALLOCATE.SESSION.NUMBERS:   

     FUNCTION.FLAG$ EQ "O"    

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%   

     PASSED.INTEGER% EQ BDCP.OUTPUT.SESS%
     PASSED.STRING$ EQ BDCP.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ ERR.SESS.NUM%
     PASSED.STRING$ EQ ERR.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ERR.SESS.NUM% EQ F20.INTEGER.FILE.NO%
	 
	 PASSED.INTEGER% EQ CRTN.OUTPUT.SESS%
     PASSED.STRING$ EQ CRTN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO%


RETURN

\******************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY
\***    Perform CALL.F20.SESS.NUM.UTILITY to allocate file session numbers
\***    for all files referenced by the program.
\***
\******************************************************************************
CALL.F20.SESS.NUM.UTILITY: 
        
         FILE.OPERATION$ = FUNCTION.FLAG$
         CURRENT.REPORT.NUM% = PASSED.INTEGER%
         RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
         IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN

\******************************************************************************
\***
\***    READ.CRTN.FILE:
\***    Reads CARTON.BIN file sequentially and dumps BOOTS.COM order which has
\***    status of un booked in to file C:READCRTN.OUT
\***
\******************************************************************************

READ.BDCP.FILE:  
      
     OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM%       
     CALL PROCESS.KEYED.FILE(BDCP.FILE.NAME$ , \ Sequential process of keyed file
                               BDCP.REPORT.NUM%,\
                                 "N")                           
    
RETURN 

 
\******************************************************************************
\***
\***    READ.OUT.FILE:
\***    Reads CARTON.BIN file sequentially and dumps BOOTS.COM order which has
\***    status of un booked in to file C:READCRTN.OUT
\***
\******************************************************************************

READ.OUT.FILE:  
      
	  
	 
	 WHILE NOT EOF%
	 IF END #BDCP.OUTPUT.SESS% THEN NO.FILE
	 
	 READ #BDCP.OUTPUT.SESS% ; LINE LINE.RECORD$

	 COUNT% = COUNT% + 1

     WEND

NO.FILE:

EOF% = TRUE
	 
                          
    
RETURN  

\******************************************************************************
\***
\***    PROCESS.KEYED.RECORD$
\***    Boots generic function to process a keyed file sequentially
\***     
\******************************************************************************

FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC

   
    STRING RECORD$,                                                \
           PROCESS.KEYED.RECORD$
           
    SUPPLIER$        = MID$(RECORD$,1,3)
    CARTON.NO$       = MID$(RECORD$,4,4)
    PARENT.ORDER$    = MID$(RECORD$,8,5)
    E.D.D$           = MID$(RECORD$,13,3)
	STATUS$          = MID$(RECORD$,16,1)
	DELV.DTIME$      = MID$(RECORD$,17,6)
    DELV.EXPORTED$   = MID$(RECORD$,23,1)
    COLLECTED.TIME$  = MID$(RECORD$,24,6)
    RSN.CODE$        = MID$(RECORD$,30,1)
    COLLECTION.EXP$  = MID$(RECORD$,31,1)
    RETURN.D.D$      = MID$(RECORD$,32,6)
    RETCENT.EXPRTD$  = MID$(RECORD$,38,1)
    LOST.DATE$       = MID$(RECORD$,39,6)
	LOST.EVENT$      = MID$(RECORD$,45,1)
	F.D.T$           = MID$(RECORD$,46,6)
	FOUND.EXPRTD$    = MID$(RECORD$,52,1)
	FILLER$          = MID$(RECORD$,53,49)
    FILLER$          = MID$(RECORD$,470,38)	


       CALL LOG.MESSAGE(UNPACK$(RECORD$))


    PROCESS.KEYED.RECORD$ = RECORD$

END FUNCTION

\******************************************************************************
\***
\***    TERMINATION:
\***    Process dellocation and close of files 
\***     
\***
\******************************************************************************

TERMINATION:

  GOSUB DEALLOCATE.SESSION.NUMBERS
  GOSUB CLOSE.FILES
   
STOP

\******************************************************************************
\***
\***    DEALLOCATE.SESSION.NUMBERS:
\***     
\***     
\***
\******************************************************************************

DEALLOCATE.SESSION.NUMBERS:

     FUNCTION.FLAG$ EQ "C"     

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%     

     PASSED.INTEGER% EQ BDCP.OUTPUT.SESS%
     PASSED.STRING$ EQ BDCP.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO% 

     PASSED.INTEGER% EQ ERR.SESS.NUM%
     PASSED.STRING$ EQ ERR.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ERR.SESS.NUM% EQ F20.INTEGER.FILE.NO% 

RETURN

\******************************************************************************
\***
\***    CLOSE.FILES:
\***    
\***
\******************************************************************************

CLOSE.FILES:

      
     CLOSE BDCP.SESS.NUM%    
     CLOSE BDCP.OUTPUT.SESS% 
     CLOSE ERR.SESS.NUM%

RETURN
\******************************************************************************
\***
\***    FILE.ERROR:
\***     
\***     
\***
\******************************************************************************

FILE.ERROR:


          VAR.STRING.1$ = FILE.OPERATION$                     +          \
                CHR$(SHIFT(CURRENT.REPORT.NUM%, 8) AND 0FFH) +          \
                CHR$(CURRENT.REPORT.NUM% AND 0FFH)           +          \
                CURRENT.CODE$
         VAR.STRING.2$ = "READCRTN"
         MESSAGE.NO%   = 0
         EVENT.NO%     = 106

         RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,               \
                      VAR.STRING.2$,EVENT.NO%)

         GOTO TERMINATION

\*******************************************************************************
\***
\***    ERROR.DETECTED:
\***    
\***     
\***
\******************************************************************************

ERROR.DETECTED:

         ERR.CNT% = ERR.CNT% + 1

         IF ERR = "OE" AND  ERRF% = CRTN.SESS.NUM% THEN BEGIN

             PRINT "CARTON File is missing"

         ENDIF

        IF ERR = "OE" AND  ERRF% = BDCP.SESS.NUM% THEN BEGIN

            PRINT "BDCP File is missing - Program Ending"
             ENDIF

         PRINT #ERR.SESS.NUM%; "An Error Occurred "
         PRINT #ERR.SESS.NUM%; "Fatal Error:" + ERR
         PRINT #ERR.SESS.NUM%; "Session Number: " + STR$(ERRF%)
         PRINT #ERR.SESS.NUM%; "Line Number:" + STR$(ERRL)

GOTO FILE.ERROR


END


 

 