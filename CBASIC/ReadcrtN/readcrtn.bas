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
    RC% = BDCP.SET	

    TEXT.FORMAT$       = "&" 
    CRTN.OUTPUT.FILE$ = "C:\READCRTN.OUT"
    CRTN.OUTPUT.NUM%  = 150
    BDCP.OUTPUT.FILE$ = "C:\BDCP.OUT"
    BDCP.OUTPUT.SESS% = 200  
    GOSUB ALLOCATE.SESSION.NUMBERS
    
    ERR.FILE.NAME$ = "C:\READCRTN.ERR"
    ERR.SESS.NUM% = 250

    CREATE CRTN.OUTPUT.FILE$ AS CRTN.OUTPUT.SESS.NUM%
    CREATE BDCP.OUTPUT.FILE$ AS BDCP.OUTPUT.SESS%
    CREATE ERR.FILE.NAME$ AS ERR.SESS.NUM%

    GOSUB READ.CRTN.FILE
    GOSUB READ.BDCP.FILE

    PRINT  TIME.STAMP$(2) + "-Found " + STR$(COUNT%) + " Unbooked BOOTS.COM Orders" 
    PRINT  TIME.STAMP$(2) + "-Found " + STR$(CONFLICT%) + " Conflicting BOOTS.COM Orders" 

    IF CONFLICT% > 0 THEN BEGIN
       PRINT  TIME.STAMP$(2) + "-Fixed " + STR$(CONFLICT%) + " Conflicting BOOTS.COM Orders" 
    ENDIF

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

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ CRTN.OUTPUT.NUM%
     PASSED.STRING$ EQ CRTN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.OUTPUT.SESS.NUM% EQ F20.INTEGER.FILE.NO%

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
\***    READ.BDCP.FILE:
\***    After reading CARTON.BIN file program dumps the records sequentially
\***    in to C:/READCRTN.OUT which is BOOTS.COM and has an Un booked status
\***    Now the program reads the file sequentially and starts comparing with 
\***    the value of the same cartons in BDCP.BIN file. 
\***    
\***    
\******************************************************************************

READ.BDCP.FILE:

     COUNT%    = 0	
     CONFLICT% = 0	 
     OPEN CRTN.OUTPUT.FILE$ AS CRTN.OUTPUT.NUM% 
     PRINT TIME.STAMP$(2) + "-Checking Carton file for Unbooked Cartons"
     OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM%

     WHILE EOF% = FALSE

         IF END #CRTN.OUTPUT.NUM% THEN CRTN.OUTPUT.ERROR
         READ #CRTN.OUTPUT.NUM% ; LINE.RECORD$	
         
         BDCP.SUPPLIER$ = PACK$(LEFT$(LINE.RECORD$,6))
         BDCP.CARTON$   = PACK$(MID$(LINE.RECORD$,9,8))
         
         RC% = READ.BDCP


         WRITE #BDCP.OUTPUT.SESS%   ;                     \
                     UNPACK$(SUPPLIER$) +              \  7 bytes UPD Supp + Carton
               " " +  UNPACK$(BDCP.CARTON$) +          \
               " " +  UNPACK$(BDCP.ORDER$) +           \  5 bytes UPD Boots.com order number
               " " +  UNPACK$(BDCP.EXPECT.DATE$) +     \  3 bytes UPD Expected Delivery Date
               " " +  BDCP.STATUS$        +            \  1 bytes ASC Current status
               " " +  BDCP.DEL.DATETIME$ +             \  6 bytes UPD Delivery date/time
               " " +  BDCP.DEL.EXPORTED$ +             \  1 bytes ASC Y/N
               " " +  BDCP.COL.DATETIME$ +             \  6 bytes UPD Collected date/time
               " " +  STR$(BDCP.COL.RC%) +             \  1 byte  INT 0 - Till, 1 - Controller
               " " +  BDCP.COL.EXPORTED$ +             \  1 bytes ASC Y/N
               " " +  BDCP.RET.DATETIME$ +             \  6 bytes UPD Returned date/time
               " " +  BDCP.RET.EXPORTED$ +             \  1 bytes ASC Y/N
               " " +  BDCP.LST.DATETIME$ +             \  6 bytes UPD Lost date/time
               " " +  BDCP.LST.EXPORTED$ +             \  1 bytes ASC Y/N
               " " +  BDCP.FND.DATETIME$ +             \  6 bytes UPD Found date/time
               " " +  BDCP.FND.EXPORTED$               \  1 bytes ASC Y/N
             

           GOSUB COMPARE.STATUS


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

IF MID$(LINE.RECORD$,22,1) = "N"  THEN BEGIN

   
   ! Count to detect the number of unbooked orders in CARTON.BIN
   
   COUNT% = COUNT% + 1
   
   !If the same order is having a different status other than
   ! 'On the way to store' is a conflicting situation which 
   ! will be corrected.
! IF MID$(LINE.RECORD$,62,2) < = "10"  THEN BEGIN  
    
 !  IF BDCP.STATUS$ <> "O" THEN BEGIN
      CONFLICT% = CONFLICT% + 1
      GOSUB CORRECT.CARTON.STATUS

  ! ENDIF
    
ENDIF

RETURN

\******************************************************************************
\***
\***    CORRECT.CARTON.STATUS 
\***    Mismatched CARTON should be booked in via exception. and will marked  
\***    accordingly and calls WRITE.CRTN function for the same 
\***
\****************************************************************************** 

CORRECT.CARTON.STATUS: 
 
   CRTN.SUPPLIER$ = PACK$(RIGHT$("000000"   + LEFT$(LINE.RECORD$,6), 6))
   CRTN.NO$       = PACK$(RIGHT$("00000000" + MID$(LINE.RECORD$,9,8), 8))
   CRTN.CHAIN%    = 0
       
   ! READ is performed again to get the number of items in each carton 
   ! record
   RC% = READ.CRTN
   
   
   ! Conflicting status is replaced with a a status of 'E'
   ! Booked in through Exception 
   
 !  CRTN.STATUS$ = CHR$(69)
   
    RC% = DELETE.CRTN
     
 
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

     PASSED.INTEGER% EQ BDCP.REPORT.NUM%
     PASSED.STRING$ EQ BDCP.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     BDCP.SESS.NUM% EQ F20.INTEGER.FILE.NO%

     PASSED.INTEGER% EQ CRTN.OUTPUT.NUM%
     PASSED.STRING$ EQ CRTN.OUTPUT.FILE$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     CRTN.OUTPUT.SESS.NUM% EQ F20.INTEGER.FILE.NO% 

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

     CLOSE CRTN.SESS.NUM%
     CLOSE BDCP.SESS.NUM%
     CLOSE CRTN.OUTPUT.SESS.NUM%
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


 

 