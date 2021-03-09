\******************************************************************************
\******************************************************************************
\***
\***    CRTNASN 
\***
\***     Original version          23/01/2016            RANJITH GOPALANKUTTY
\***
\***     
\***
\******************************************************************************
\******************************************************************************
\***
\***    CRTNASN
\***    ********
\***   
\***    There has been issues with HHT devices showing incorrect number of 
\***    orders to be booked in when comparing with the controller reports.
\***    When checked in detail, came to the attention that its due to extra
\***    number of entries available in CARTON.BIN file when comparing with	
\***    ASN.BIN causing the issue. 
\***    
\***    In order to fix the issue , a custom made utility will makre sure that
\***    unnecessary entries will be housekept from the CARTON.BIN file 
\***    making it matching with ASN file.
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
    %INCLUDE CRTNDEC.J86
    %INCLUDE PSBF01G.J86
    %INCLUDE PSBF20G.J86
    %INCLUDE ASNDEC.J86
   
\******************************************************************************
\***
\***    Global Variable definitions
\***
\******************************************************************************


     STRING GLOBAL                                                      \
         ASN.OUTPUT.FILE$,                                              \
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
         ASN.OUTPUT.SESS%,                                              \
         BDCP.OUTPUT.SESS%,                                             \
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
    %INCLUDE ASNEXT.J86    ! ASN
    %INCLUDE BDCPEXT.J86   ! BDCP
    %INCLUDE CRTNEXT.J86
    %INCLUDE BTCMEM.J86
    %INCLUDE ERRNH.J86
    %INCLUDE ADXSERVE.J86  ! Controller Services

\******************************************************************************
\***
\***    Sub routine for writing the records
\***
\******************************************************************************	

SUB LOG.MESSAGE(F.MSG$)
STRING F.MSG$
       
  ! IF UNPACK$(SUPPLIER$) = "117838" AND STATUS$ = "U" THEN BEGIN
        PRINT USING TEXT.FORMAT$ ; #CRTN.OUTPUT.SESS.NUM% ; F.MSG$

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
    RC% = ASN.SET	
    RC% = BDCP.SET

    TEXT.FORMAT$      = "&" 
    CRTN.OUTPUT.FILE$ = "C:\CRTN.OUT"
    CRTN.OUTPUT.NUM%  = 150
    ASN.OUTPUT.FILE$  = "C:\ASN.OUT"
    ASN.OUTPUT.SESS%  = 200  
    BDCP.OUTPUT.FILE$ = "C:\BDCP.OUT"
    BDCP.OUTPUT.SESS% = 250 

    GOSUB ALLOCATE.SESSION.NUMBERS
    
    ERR.FILE.NAME$ = "C:\CRTNASN.ERR"
    ERR.SESS.NUM% = 250

    CREATE CRTN.OUTPUT.FILE$ AS CRTN.OUTPUT.SESS.NUM%
    CREATE ASN.OUTPUT.FILE$ AS ASN.OUTPUT.SESS%
    CREATE ERR.FILE.NAME$ AS ERR.SESS.NUM%


    GOSUB READ.CRTN.FILE
    GOSUB READ.ASN.FILE

    PRINT  TIME.STAMP$(2) + "-Found " + STR$(COUNT%) + " Total records in CARTON" 
    PRINT  TIME.STAMP$(2) + "-Found " + STR$(CONFLICT%) + " Missing orders from ASN"

    IF  ERR.CNT% = 0 THEN BEGIN  
        PRINT  TIME.STAMP$(2) + "-Application Completed Successfully "  
    ENDIF ELSE BEGIN
       PRINT #ERR.SESS.NUM% ; TIME.STAMP$(2) + "Program ended with error"  
    ENDIF
    
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

     PASSED.INTEGER% EQ CRTN.REPORT.NUM%
     PASSED.STRING$ EQ CRTN.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ ASN.REPORT.NUM%
     PASSED.STRING$ EQ ASN.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ASN.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ BDCP.OUTPUT.SESS%
     PASSED.STRING$ EQ BDCP.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ CRTN.OUTPUT.NUM%
     PASSED.STRING$ EQ CRTN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.OUTPUT.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ ASN.OUTPUT.SESS%
     PASSED.STRING$ EQ ASN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ASN.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ ERR.SESS.NUM%
     PASSED.STRING$ EQ ERR.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ERR.SESS.NUM% EQ F20.INTEGER.FILE.NO%


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

READ.CRTN.FILE: 
 
     
     OPEN CRTN.FILE.NAME$ KEYED RECL CRTN.RECL% AS CRTN.SESS.NUM%
     CALL LOG.MESSAGE(TIME.STAMP$(2) + "  File opened")
     PRINT TIME.STAMP$(2) + "-Reading Carton file started"  
     CALL PROCESS.KEYED.FILE(CRTN.FILE.NAME$ , \ Sequential process of keyed file
                               CRTN.REPORT.NUM%,\
                                "N")
                           
    
RETURN 

\******************************************************************************
\***
\***    READ.ASN.FILE:
\***    After reading CARTON.BIN file program dumps the records sequentially
\***    in to C:/CRTN.OUT. Program then read the output file and builds the  
\***    key for reading ASN file. If read is successful then no need to delete
\***    the record if not need to delete that particular record from CARTON 
\***    file
\***    
\******************************************************************************

READ.ASN.FILE:

     COUNT%    = 0
     CONFLICT% = 0
     OPEN CRTN.OUTPUT.FILE$ AS CRTN.OUTPUT.NUM% 
     PRINT TIME.STAMP$(2) + "-Checking Carton file for Unbooked Cartons"
     OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM%
     OPEN ASN.FILE.NAME$ KEYED RECL ASN.RECL% AS ASN.SESS.NUM%     

     WHILE EOF% = FALSE
          
         IF END #CRTN.OUTPUT.NUM% THEN CRTN.OUTPUT.ERROR
         READ #CRTN.OUTPUT.NUM% ; LINE LINE.RECORD$	

         BDCP.SUPPLIER$ = PACK$(LEFT$(LINE.RECORD$,6))
         BDCP.CARTON$   = PACK$(MID$(LINE.RECORD$,9,8))
         
         RC% = READ.BDCP
		 
		 IF RC% = 0 THEN BEGIN
		 
		    PRINT " READ IS SUCCESSFUL"
		  
		 ENDIF

         COUNT% = COUNT% + 1
		 
		 GOSUB COMPARE.STATUS

         ASN.CHAIN% = 0                                                     
         ASN.NO$ = MID$(LINE.RECORD$,25,18) 

         IF ASN.NO$ <> " " THEN BEGIN

             RC% = READ.ASN                                                     
         
             IF RC% < > 0 THEN BEGIN

                 CONFLICT% = CONFLICT% + 1
                 

             ENDIF         
        
         ENDIF   

      WEND

RETURN
 
CRTN.OUTPUT.ERROR:


    EOF% = TRUE	

RETURN

\******************************************************************************
\***
\***    COMPARE.STATUS
\***    With the value got from reading CARTON.BIN file program checks the status
\***    between both CARTON.BIN and BDCP.BIN file. If there are mismatches in  
\***    STATUS$. Go to CORRECT.CARTON.STATUS to correct the status
\***    
\***    If any particular carton has unbooked status in CARTON.BIN file and same
\***    carton has any other status other than 'On the way to store' in BDCP
\***    file is not acceptable. Which will be requested for correction
\***
\******************************************************************************
COMPARE.STATUS:

IF MID$(LINE.RECORD$,22,1) = "U" AND MID$(LINE.RECORD$,62,2) = "12" THEN BEGIN  
	GOSUB CORRECT.CARTON.STATUS
 
 ENDIF
    
 

RETURN
\******************************************************************************
\***
\***    CORRECT.CARTON.STATUS 
\***    Mismatched CARTON should be deleted from CARTON file
\***     
\***
\****************************************************************************** 

CORRECT.CARTON.STATUS: 

   CRTN.SUPPLIER$ = PACK$(RIGHT$("000000"   + LEFT$(LINE.RECORD$,6), 6))
   CRTN.NO$       = PACK$(RIGHT$("00000000" + MID$(LINE.RECORD$,9,8), 8))
   CRTN.CHAIN%    = 0       


   RC% = READ.CRTN
   IF RC% = 0 THEN BEGIN

            RC% = DELETE.CRTN
            CNTR% = CNTR% + 1
   ENDIF

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
    CHAIN$           = MID$(RECORD$,8,1)
    STATUS$          = MID$(RECORD$,9,1)
    ASN.CODE$        = MID$(RECORD$,10,18)
    ORDER.NUM$       = MID$(RECORD$,28,5)
    ORDER.SUFFIX$    = MID$(RECORD$,33,1)
    BUS.CENTRE$      = MID$(RECORD$,34,1)
    E.D.D$           = MID$(RECORD$,35,12)
    ITEM.COUNT$      = MID$(RECORD$,47,3)
    REPEATED$        = MID$(RECORD$,50,420)
    FILLER$          = MID$(RECORD$,470,38)	


       CALL LOG.MESSAGE(UNPACK$(SUPPLIER$) + "  " +   \
                        UNPACK$(CARTON.NO$) + "  " + \
                        CHAIN$ + "  " + \
                        STATUS$   + "  " + \
                        ASN.CODE$  + "  " + \
                        ORDER.NUM$  + "  " + \
                        ORDER.SUFFIX$ + "  " + \
                        BUS.CENTRE$ + "  " + \
                        E.D.D$   + "  " + \
                        ITEM.COUNT$ + "  " + \
                        REPEATED$  + "  " + \  
                        FILLER$ )


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

     PASSED.INTEGER% EQ CRTN.REPORT.NUM%
     PASSED.STRING$ EQ CRTN.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ ASN.REPORT.NUM%
     PASSED.STRING$ EQ ASN.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ASN.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ CRTN.OUTPUT.NUM%
     PASSED.STRING$ EQ CRTN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.OUTPUT.SESS.NUM% EQ F20.INTEGER.FILE.NO% 

     PASSED.INTEGER% EQ ASN.OUTPUT.SESS%
     PASSED.STRING$ EQ ASN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     ASN.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO% 

     PASSED.INTEGER% EQ BDCP.OUTPUT.SESS%
     PASSED.STRING$ EQ BDCP.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.OUTPUT.SESS% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%

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

     CLOSE CRTN.SESS.NUM%
     CLOSE ASN.SESS.NUM%
	 CLOSE BDCP.SESS.NUM%
     CLOSE CRTN.OUTPUT.SESS.NUM%
     CLOSE ASN.OUTPUT.SESS% 
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
         VAR.STRING.2$ = "CRTNASN"
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

         IF ERR = "OE" AND  ERRF% = ASN.SESS.NUM% THEN BEGIN

             PRINT "ASN File is missing - Program Ending"
        
         ENDIF

         PRINT #ERR.SESS.NUM%; "An Error Occurred "
         PRINT #ERR.SESS.NUM%; "Fatal Error:" + ERR
         PRINT #ERR.SESS.NUM%; "Session Number: " + STR$(ERRF%)
         PRINT #ERR.SESS.NUM%; "Line Number:" + STR$(ERRL)

GOTO FILE.ERROR


END

 