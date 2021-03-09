\******************************************************************************
\******************************************************************************
\******************************************************************************
\******************************************************************************
\*******************************************************************************
\*******************************************************************************
\***                                                                         ***
\***                                                                         ***
\***            PROGRAM       : BLOCKITM.286                                 ***
\***                                                                         ***
\***            TITLE         : Block Item From Sale Fiddle Program          ***
\***                                                                         ***
\***            AUTHOR        : Charles Skadorwa                             ***
\***                                                                         ***
\***            DATE WRITTEN  : 13th Jun 2008                                ***
\***                                                                         ***
\***            MODULE        : BLOCKITM.BAS                                 ***
\***                                                                         ***
\***                                                                         ***
\***                                                                         ***
\*******************************************************************************
\***  Description
\***  ===========
\***  The purpose of this program is to read in a file containing a list of
\***  BOOTS item codes and set two BIT flags on the IRF for those items.
\***  
\***     IRF.INDICAT0% Bit 5 - Item not authorised for sale (recall active)
\***     IRF.INDICAT8% Bit 6 - Withdrawn Recall
\***  
\***  This action will prevent the sale of these items at the till   
\***               
\***
\***  Ver A     Charles Skadorwa                                    13 Jun 2008
\***            Initial Version.
\***
\***  Ver B     Charles Skadorwa                                    20 Jun 2008
\***            Defect: Program changed to update all associated barcodes
\***                    by reading the IDF to get no. of barcodes and IEF
\***                    in order to trawl down the chain of barcodes.
\***
\***  Ver C     Charles Skadorwa                                    6  Aug 2008
\***            Defect: Program changed to update first barcode if only 
\***                    one barcode exists eg. Insurance items
\***
\***  Ver D     Charles Skadorwa                                    17 Oct 2008
\***            Enhancement: Ensure that only real program failures will 
\***                         result in a .ERR file ie. we don't want to
\***                         report a failure for items not on file. Also,
\***                         a .WRN file is created which will list any items
\***                         not on file.
\***
\***  Ver E     Charles Skadorwa                                    10 Oct 2009
\***            This program assumed that the IDF, IRF and IDF files are
\***            synchronised with no corruptions. However, in some instances 
\***            an item can exist on the IRF and not the IDF. This change
\***            ensures that if a there is a read failure on the IDF, then the
\***            IRF is scanned for all occurrences of that item and the recall 
\***            bit flags set. This is to resolve the issue whereby Blockitm is 
\***            run but the PPC's still report items being on recall because 
\***            Transact reads the IRF directly and not the IDF.
\***
\*******************************************************************************
\*******************************************************************************
\*******************************************************************************
\***
\***   Define variables
\***
\***---------------------------------------------------------------------------

%INCLUDE IDFDEC.J86     ! IDF Variables                                      !BCS
%INCLUDE IEFDEC.J86     ! IEF Variables                                      !BCS
%INCLUDE IRFDEC.J86     ! IRF Variables
%INCLUDE PSBF01G.J86    ! APPLICATION.LOG
%INCLUDE PSBF11G.J86    ! Gets next barcode in an IEF chain                  !BCS
%INCLUDE PSBF20G.J86    ! GLOBAL VARIABLE DEFINITIONS FOR SESS.NUM.UTILITY FUNCTION

  
STRING GLOBAL                             \
     CURRENT.CODE$,                       \
     FILE.OPERATION$                      !

INTEGER*1 TRUE,                           \
          FALSE,                          \
          EVENT.NUM%                      !
          
INTEGER*2 I%,                             \
          J%,                             \
          CURRENT.REPORT.NUM%,            \
          DATE.FORMAT%,                   \
          ERROR.COUNT%,                   \
          ERR.REPORT.NUM%,                \
          ERR.SESS.NUM%,                  \
          EXPCTD.NO.CODES%,               \                                  !BCS
          INDEX%,                         \                                  !BCS
          MESSAGE.NUMBER%,                \
          BLOCKITM.SESS.NUM%,             \
          INPUT.RECL%,                    \
          INPUT.REPORT.NUM%,              \
          INPUT.SESS.NUM%,                \
          OK.REPORT.NUM%,                 \
          OK.SESS.NUM%,                   \
          LOG.REPORT.NUM%,                \
          LOG.SESS.NUM%,                  \
          WRN.REPORT.NUM%,                \                                  !DCS
          WRN.SESS.NUM%,                  \                                  !DCS
          SB.FILE.REP.NUM%,               \
          SB.FILE.SESS.NUM%,              \
          SB.INTEGER%                     !
                                 
                              
INTEGER*4 ADXSERVE.RET.CODE%,             \
          ADX.RC%,                        \
          INPUT.REC.NO%,                  \
          RC%                             !

STRING    ADXSERVE.DATA$,                 \
          ADX.DATA$,                      \
          BMESG$,                         \
          CMD.LINE$,                      \
          COMM.MODE.FLAG$,                \
          CURRENT.IEF.BAR.CODE$,          \                                  !BCS
          ERROR.OVERRIDE.FLAG$,           \
          GENUINE.ERROR.FLAG$,            \                                  !DCS
          BLOCKITM.COMPLETION.MSG$,       \
          BLOCKITM.ERR.PATH$,             \
          BLOCKITM.INPUT.PATH$,           \
          BLOCKITM.OK.PATH$,              \
          BLOCKITM.LOG.PATH$,             \
          BLOCKITM.WRN.PATH$,             \                                  !DCS
          BLOCKITM.PATH$,                 \
          INPUT.FILLER$,                  \
          INPUT.BOOTS.CODE$,              \
          MSG$,                           \                                  !DCS
          MODULE$,                        \       
          MODULE.NUMBER$,                 \
          NULL.BAR.CODE$,                 \                                  !BCS
          PARAM$,                         \
          PROGRAM$,                       \
          LOG.FILE.OPEN$,                 \      
          RUN.DATE$,                      \
          RUN.TIME$,                      \
          SB.ACTION$,                     \
          SB.STRING$,                     \
          SB.FILE.NAME$,                  \
          SKIP.PAST.ITEM.DUE.TO.ERROR$,   \                                  !BCS
          UPD.BAR.CODE$,                  \                                  !BCS
          VAR.STRING.1$,                  \
          VAR.STRING.2$,                  \
          VERSION$                        !                                  !DCS


%INCLUDE ADXSERVE.J86
%INCLUDE ERRNH.J86      ! Hex Error Number
%INCLUDE IDFEXT.J86     ! IDF FILE FUNCTIONS                                 !BCS
%INCLUDE IEFEXT.J86     ! IEF FILE FUNCTIONS                                 !BCS
%INCLUDE IRFEXT.J86     ! IRF FILE FUNCTIONS
%INCLUDE PSBF01E.J86    ! APPLICATION.LOG
%INCLUDE PSBF11E.J86    ! Gets next barcode in an IEF chain                  !BCS
%INCLUDE PSBF20E.J86    ! GLOBAL VARIABLE DEFINITIONS FOR SESS.NUM.UTILITY FUNCTION
%INCLUDE PSBF24E.J86    ! STANDARD.ERROR.DETECTED
%INCLUDE PSBF30E.J86    ! PROCESS KEYED FILE                                 !ECS



    
\******************************************************************************
\***
\***    DO.MESSAGE 
\***
\******************************************************************************

SUB DO.MESSAGE(MESG$)

    INTEGER*2 STATUS%
    STRING MESG$
    
    IF STATUS% = 0 THEN BEGIN
        CALL ADXSERVE (ADXSERVE.RET.CODE%,26,0,MESG$)
        IF ADXSERVE.RET.CODE% <> 0 THEN BEGIN
            STATUS% = 1 !FOREGROUND
            PRINT MESG$
        ENDIF ELSE BEGIN
            STATUS% = 2 !BACKGROUND
        ENDIF
    ENDIF ELSE IF STATUS% = 1 THEN BEGIN
        PRINT MESG$
    ENDIF ELSE BEGIN
        CALL ADXSERVE (ADXSERVE.RET.CODE%,26,0,MESG$)
    ENDIF
    
    IF LOG.FILE.OPEN$ = "Y" THEN BEGIN
          PRINT # LOG.SESS.NUM%; MESG$
    ENDIF
    
END SUB
        
        
\******************************************************************************
\***
\***    GETN1
\***
\******************************************************************************

FUNCTION GETN1 (S$, OFFSET%)
    INTEGER*2 GETN1
    STRING S$
    INTEGER*2 OFFSET%
    GETN1 = ASC(MID$(S$, OFFSET%+1, 1))
END FUNCTION
    
    
\******************************************************************************
\***
\***    Format date    YYYYMMDD --> DD/MM/YYYY
\***
\******************************************************************************

FUNCTION FORMAT.DATE$ (FIELD$)

    STRING      FIELD$
    STRING      FORMAT.DATE$

    FORMAT.DATE$ = MID$(FIELD$, 5, 2) + "/" +                     \
                   MID$(FIELD$, 3, 2) + "/20" +                   \
                   MID$(FIELD$, 1, 2)

END FUNCTION


\******************************************************************************
\***
\***    Format Time      HHMMSS --> HH:MM                               
\***
\******************************************************************************

FUNCTION FORMAT.TIME$ (FIELD$)

    STRING      FIELD$
    STRING      FORMAT.TIME$


    FORMAT.TIME$ = MID$(FIELD$, 1, 2) +   \
                   ":"                +   \
                   MID$(FIELD$, 3, 2)

END FUNCTION


\******************************************************************************
\***                                                                          *
\***   FUNCTION : READ.INPUT                                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***  Reads the Input file.                                                   *
\***                                                                          *
\******************************************************************************

FUNCTION READ.INPUT PUBLIC

      INTEGER*2 READ.INPUT

      READ.INPUT = 1

      IF END # INPUT.SESS.NUM% THEN READ.ERROR
      READ FORM "C7,C2"; #INPUT.SESS.NUM%, INPUT.REC.NO%;   \
               INPUT.BOOTS.CODE$, \
               INPUT.FILLER$

      READ.INPUT = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = INPUT.REPORT.NUM%
      CURRENT.CODE$       = ""

END FUNCTION


\********************************************************************************
\***                                                                     !ECS   *
\***      READ.AND.UPDATE.IRF                                                   *
\***                                                                            *
\***                                                                            *
\********************************************************************************

SUB READ.AND.UPDATE.IRF

    IRF.BAR.CODE$ = UPD.BAR.CODE$
    
    RC% = READ.IRF.LOCK
    
    IF RC% = 0 THEN BEGIN ! Item exists on IRF file
        IF CMD.LINE$ = "YES" THEN BEGIN
            ! Set Item Not Authorised for Sale Bit flag 5
            IRF.INDICAT0% = IRF.INDICAT0% OR 00010000B  
            ! Set Recall Reason Type Bit flags 6 & 7
            IRF.INDICAT8% = IRF.INDICAT8% OR 01100000B  
        ENDIF ELSE BEGIN ! "NO"
            ! Reset Item Not Authorised for Sale Bit flag 5
            IRF.INDICAT0% = IRF.INDICAT0% AND 11101111B  
            ! Reset Recall Reason Type Bit flags 6 & 7
            IRF.INDICAT8% = IRF.INDICAT8% AND 10011111B  
        ENDIF
        
        RC% = WRITE.IRF.UNLOCK

        IF RC% = 0 THEN BEGIN ! Item updated successfully on IRF file
            CALL DO.MESSAGE(STR$(INPUT.REC.NO%) + ":" + UNPACK$(UPD.BAR.CODE$) + " " + BMESG$)
        ENDIF ELSE BEGIN
            MSG$ = STR$(INPUT.REC.NO%) + ":" + UNPACK$(UPD.BAR.CODE$) + " ERROR - CANNOT UPDATE"  !DCS
            CALL DO.MESSAGE(MSG$)                                                                 !DCS
            PRINT # WRN.SESS.NUM%; MSG$                                                           !DCS
            ! Carry on processing but set error override flag so that error is captured
            ! ie. BLOCKITM.ERR file is produced
            ERROR.OVERRIDE.FLAG$ = "Y"
            SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
            GENUINE.ERROR.FLAG$  = "Y"                                                            !DCS
        ENDIF
        
    ENDIF ELSE BEGIN
        MSG$ = STR$(INPUT.REC.NO%) + ":" + UNPACK$(UPD.BAR.CODE$) + " ** NOT ON FILE **"      !DCS
        CALL DO.MESSAGE(MSG$)                                                                 !DCS
        PRINT # WRN.SESS.NUM%; MSG$                                                           !DCS
    ENDIF

END SUB



\********************************************************************************
\***                                                                     !ECS   *
\***      PROCESS.KEYED.RECORD$                                                 *
\***                                                                            *
\***      'User exit' for PROCESS.KEYED.FILE (PSBF30)                           *
\***                                                                            *
\********************************************************************************

FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC
            
    STRING PROCESS.KEYED.RECORD$,  \
           RECORD$    
    
    IF MID$(RECORD$,43, 3) = IRF.BOOTS.CODE$ THEN BEGIN
    
        CALL DO.MESSAGE("    Updating Barcode: " + UNPACK$(LEFT$(RECORD$,11)) )
        
        IRF.INDICAT0% = ASC(MID$(RECORD$,12, 1))
        IRF.INDICAT8% = ASC(MID$(RECORD$,16, 1)) 
        UPD.BAR.CODE$ = LEFT$(RECORD$,11)
        
        CALL READ.AND.UPDATE.IRF
    ENDIF
    
    PROCESS.KEYED.RECORD$ = RECORD$      

END FUNCTION


    
!****************************************************************************
!****************************************************************************
!****************************************************************************
!****************************************************************************
!****************************************************************************
!****                                                                    ****
!****             S T A R T   O F   M A I N   P R O G R A M              ****
!****                                                                    ****
!****************************************************************************
!****************************************************************************
!****************************************************************************
!****************************************************************************
!****************************************************************************

ON ERROR GOTO ERROR.DETECTED

    GOSUB INITIALISATION
    
    GOSUB MAIN.PROCESSING

    GOSUB TERMINATION
    
ABORT.PROGRAM:

    STOP


!**************************************************************************
!***
!***    INITIALISATION:
!***
!***    Set up global variables.
!***    Display initial message.
!***    Determine if running in background.
!***
!**************************************************************************

INITIALISATION:

      
      PROGRAM$           EQ "BLOCKITM"
      MODULE$            EQ ""
      MODULE.NUMBER$     EQ  PROGRAM$ + MODULE$

      VERSION$ = "5.0  14/10/09"                                             !DCS !ECS
      
      ERROR.OVERRIDE.FLAG$ = "N"
      GENUINE.ERROR.FLAG$  = "N"                                             !DCS
      LOG.FILE.OPEN$ = "N"
      TRUE  = -1
      FALSE = 0
      
      ERROR.COUNT% = 0
      INPUT.RECL%  = 9  ! 7-digit Boots Item code + CRLF
      
      NULL.BAR.CODE$ = PACK$(STRING$(12,"0"))                                !BCS
      
      !------------------------
      ! Check Parameters passed
      !------------------------
      CMD.LINE$ = COMMAND$
      CMD.LINE$ = UCASE$(CMD.LINE$)
      
      CLEARS
      
      IF CMD.LINE$ <> "" THEN BEGIN  ! Only display if parameter passed      !DCS
          CALL DO.MESSAGE("BLOCKITM.286 (ver: " + VERSION$ + ")")            !DCS
          CALL DO.MESSAGE("Initialisation")                                  !DCS
          CALL DO.MESSAGE("CMD.LINE$: " + CMD.LINE$)                         !DCS
      ENDIF                                                                  !DCS
      
      IF CMD.LINE$ <> "YES" AND CMD.LINE$ <> "NO" THEN BEGIN   
          CALL DO.MESSAGE("                              BLOCKITM.286 (ver: " + VERSION$ + ")")
          CALL DO.MESSAGE("                              ============")
          CALL DO.MESSAGE(" This program blocks or unblocks items for sale at the till.")
          CALL DO.MESSAGE("")
          CALL DO.MESSAGE("               USAGE: BLOCKITM YES")
          CALL DO.MESSAGE("                      BLOCKITM NO")
          CALL DO.MESSAGE("")
          CALL DO.MESSAGE(" This program reads an input text file:- ADXLXAAN::C:\BLOCKITM.DAT")
          CALL DO.MESSAGE(" which contains a list of 7-digit Boots item codes.")
          CALL DO.MESSAGE(" The following bit flags are set to 1 (YES) or 0 (NO) on the IRF for ")
          CALL DO.MESSAGE(" each item code within the input file & associated barcodes :")
          CALL DO.MESSAGE("")
          CALL DO.MESSAGE("     Item Not Authorised for Sale (INDICAT0%, Bit flag  5)")
          CALL DO.MESSAGE("     Recall Reason Type           (INDICAT8%, Bit flags 6 & 7)")
          CALL DO.MESSAGE("")
          CALL DO.MESSAGE(" C:\BLOCKITM.OK  is created if successful")
          CALL DO.MESSAGE(" C:\BLOCKITM.ERR is created if failed or could not update an item(s).")
          CALL DO.MESSAGE("                 In this case, run it again or apply update manually.")
          CALL DO.MESSAGE(" C:\BLOCKITM.LOG is created which logs all processing activity.")
          CALL DO.MESSAGE(" C:\BLOCKITM.WRN is created which logs any items not on file.")
          CALL DO.MESSAGE("")
          CALL DO.MESSAGE("     NB: NO files are created if the incorrect parameters are passed.")
          GOTO ABORT.PROGRAM
      ENDIF 
      
      BMESG$ = "Unblocked"
      IF CMD.LINE$ = "YES" THEN BEGIN
          BMESG$ = "Blocked"
      ENDIF
      !--------------------------
      ! Clear up OK and ERR files
      !--------------------------
      CALL IDF.SET                                                           !BCS
      CALL IEF.SET                                                           !BCS
      CALL IRF.SET
      
      GOSUB ALLOCATE.SESSION.NUMBERS
      
      BLOCKITM.OK.PATH$    = "ADXLXAAN::C:\BLOCKITM.OK"
      BLOCKITM.ERR.PATH$   = "ADXLXAAN::C:\BLOCKITM.ERR"
      BLOCKITM.PATH$ = BLOCKITM.ERR.PATH$
      BLOCKITM.SESS.NUM% = ERR.SESS.NUM%
      BLOCKITM.COMPLETION.MSG$ = "BLOCKITM has failed. Check C:\BLOCKITM.LOG and EVENT LOG"
      
      BLOCKITM.INPUT.PATH$ = "ADXLXAAN::C:\BLOCKITM.DAT"
      BLOCKITM.LOG.PATH$   = "ADXLXAAN::C:\BLOCKITM.LOG"
      BLOCKITM.WRN.PATH$   = "ADXLXAAN::C:\BLOCKITM.WRN"                      !DCS
         
      GOSUB DELETE.COMPLETION.FILES
      GOSUB CREATE.COMPLETION.FILE   ! Create .ERR at start
      
      RUN.DATE$   = FORMAT.DATE$(DATE$)                                             
      RUN.TIME$   = FORMAT.TIME$(TIME$)
                                                                             
      IF END # LOG.SESS.NUM% THEN FILE.ERROR
      
        CREATE POSFILE BLOCKITM.LOG.PATH$ AS LOG.SESS.NUM%    \
               BUFFSIZE 32768 LOCKED LOCAL
      
      LOG.FILE.OPEN$ = "Y"
      
      IF END # WRN.SESS.NUM% THEN FILE.ERROR                                  !DCS
                                                                              !DCS
        CREATE POSFILE BLOCKITM.WRN.PATH$ AS WRN.SESS.NUM%    \               !DCS
               BUFFSIZE 32768 LOCKED LOCAL                                    !DCS
               
      CALL DO.MESSAGE("BLOCKITM.286 (ver: " + VERSION$ + ")" + " started: " + RUN.DATE$ + " @ " + RUN.TIME$)
      
      GOSUB OPEN.FILES
      
RETURN
      

!**************************************************************************
!***
!***    MAIN.PROCESSING
!***
!**************************************************************************

MAIN.PROCESSING:

    CALL DO.MESSAGE("MAIN PROCESSING")
                                             
    IF END #INPUT.SESS.NUM% THEN FILE.ERROR  
    
    INPUT.REC.NO% = 1
    
    RC% = READ.INPUT
    
    WHILE RC% <> 1
    
        SKIP.PAST.ITEM.DUE.TO.ERROR$ = "N"  ! Reset flag                     !BCS
        
        GOSUB GET.NO.OF.BARCODES                                             !BCS
        
        IF SKIP.PAST.ITEM.DUE.TO.ERROR$ = "N" THEN BEGIN                     !BCS
            GOSUB TRAWL.THROUGH.ALL.ASSOCIATED.BARCODES.ON.IEF               !BCS
        ENDIF ELSE BEGIN                                                     !BCS!ECS
            CALL DO.MESSAGE("Scanning through IRF")                              !ECS
            IRF.BOOTS.CODE$ = PACK$(MID$(UNPACK$(IDF.BOOTS.CODE$),2, 6))         !ECS
                                                                                 
            RC% = PROCESS.KEYED.FILE(IRF.FILE.NAME$, \                           !ECS 
                                     IRF.REPORT.NUM%,\                           !ECS
                                     "N")  ! READONLY = N                        !ECS
                             
            IF RC% <> 0 THEN BEGIN                                               !ECS
                CALL DO.MESSAGE("PSBF30 ERROR - continuing")                     !ECS
                GOSUB PSBF30.ERROR         ! Log Non-zero return code from ext func   !ECS
            ENDIF                                                                !ECS
        ENDIF                                                                !ECS
         
        INPUT.REC.NO% = INPUT.REC.NO% + 1    ! Next input record
        
        RC% = READ.INPUT
        
    WEND
    
RETURN
    

!**************************************************************************
!***                                                                  !BCS
!***    GET.NO.OF.BARCODES
!***
!**************************************************************************

GET.NO.OF.BARCODES:

    EXPCTD.NO.CODES% = 0
    IDF.BOOTS.CODE$ = PACK$("0" + INPUT.BOOTS.CODE$) 
    RC% = READ.IDF                                                                            
                                                                                              
    IF RC% = 0 THEN BEGIN ! Item read successfully from IDF file                             
        EXPCTD.NO.CODES% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))
        CALL DO.MESSAGE(STR$(INPUT.REC.NO%) + ":" + INPUT.BOOTS.CODE$ + \
                        " " + "Read IDF OK. No. of Barcodes: " + STR$(EXPCTD.NO.CODES%))   
    ENDIF ELSE BEGIN                                                                          
        MSG$ = STR$(INPUT.REC.NO%) + ":" + INPUT.BOOTS.CODE$ + " IDF read FAILED"     !DCS
        CALL DO.MESSAGE(MSG$)                                                         !DCS
        PRINT # WRN.SESS.NUM%; MSG$                                                   !DCS
        ! Carry on processing but set error override flag so that error is captured           
        ! Nb. BLOCKITM.ERR file only produced for catastrophic failures
        ERROR.OVERRIDE.FLAG$ = "Y"
        SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
    ENDIF
    
RETURN


!**************************************************************************
!***                                                                  !BCS
!***    TRAWL.THROUGH.ALL.ASSOCIATED.BARCODES.ON.IEF
!***
!**************************************************************************

TRAWL.THROUGH.ALL.ASSOCIATED.BARCODES.ON.IEF:

   IF IDF.FIRST.BAR.CODE$ = NULL.BAR.CODE$ THEN BEGIN
        MSG$ = STR$(INPUT.REC.NO%) + ":" + INPUT.BOOTS.CODE$ + " ERROR - NULL 1st Barcode on IDF"     !DCS
        CALL DO.MESSAGE(MSG$)                                                                         !DCS
        PRINT # WRN.SESS.NUM%; MSG$                                                                   !DCS
           ! Carry on processing but set error override flag so that error is captured
           ! ie. BLOCKITM.ERR file is produced
           ERROR.OVERRIDE.FLAG$ = "Y"
           SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
           GENUINE.ERROR.FLAG$  = "Y"                                                                 !DCS
       RETURN
   ENDIF
   
   UPD.BAR.CODE$ = PACK$("0000000000") + IDF.FIRST.BAR.CODE$ ! Convert 6-bytes packed to 11-bytes packed
   
                                                                                !CCS
   IF EXPCTD.NO.CODES% = 1 THEN BEGIN                                           !CCS
            !------------------------                                           !CCS
            ! Update 1st Barcode ONLY                                           !CCS
            !-----------------------                                            !CCS
            CALL READ.AND.UPDATE.IRF                                            !CCS !ECS
   ENDIF                                                                        !CCS
   
   
   IF EXPCTD.NO.CODES% > 1 THEN BEGIN
     
       IF IDF.SECOND.BAR.CODE$ = NULL.BAR.CODE$ THEN BEGIN  
           MSG$ = STR$(INPUT.REC.NO%) + ":" + INPUT.BOOTS.CODE$ + " ERROR - NULL 2nd Barcode on IDF"     !DCS
           CALL DO.MESSAGE(MSG$)                                                                         !DCS
           PRINT # WRN.SESS.NUM%; MSG$                                                                   !DCS
           ! Carry on processing but set error override flag so that error is captured
           ! ie. BLOCKITM.ERR file is produced
           ERROR.OVERRIDE.FLAG$ = "Y"
           SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
           GENUINE.ERROR.FLAG$  = "Y"                                                                 !DCS
           RETURN
       ENDIF ELSE BEGIN
            !-------------------
            ! Update 1st Barcode
            !-------------------                                                 
            CALL READ.AND.UPDATE.IRF                                            !ECS
            !-------------------
            ! Update 2nd Barcode
            !-------------------
            UPD.BAR.CODE$ = PACK$("0000000000") + IDF.SECOND.BAR.CODE$ ! Convert 6-bytes packed to 11-bytes packed
            CALL READ.AND.UPDATE.IRF                                            !ECS
       ENDIF
      

       IF EXPCTD.NO.CODES% > 2 THEN BEGIN 
             !-------------------------------------
             ! Update all other associated Barcodes
             !-------------------------------------
             RC% = READ.NEXT.IEF (IDF.SECOND.BAR.CODE$,   \
                                  EXPCTD.NO.CODES%,       \
                                  INPUT.BOOTS.CODE$)      
             IF RC% = 1 THEN BEGIN
                 MSG$ = STR$(INPUT.REC.NO%) + ":" + INPUT.BOOTS.CODE$ + " ERROR - reading 3rd Barcode on IDF"  !DCS
                 CALL DO.MESSAGE(MSG$)                                                                         !DCS
                 PRINT # WRN.SESS.NUM%; MSG$                                                                   !DCS
                 ! Carry on processing but set error override flag so that error is captured
                 ! ie. BLOCKITM.ERR file is produced
                 ERROR.OVERRIDE.FLAG$ = "Y"
                 SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
                 RETURN
             
             ENDIF
       
             F11.CURRENT.COUNT% = 1                                    
             INDEX% = 3
             
             WHILE VAL(UNPACK$(F11.NEXT.BAR.CODE$)) <> 0                       
                 
                 !--------------------------
                 ! Update associated Barcode
                 !--------------------------
                 UPD.BAR.CODE$ = PACK$("0000000000") + F11.NEXT.BAR.CODE$ ! Convert 6-bytes packed to 11-bytes packed
                 CALL READ.AND.UPDATE.IRF                                            !ECS
                 
                 CURRENT.IEF.BAR.CODE$ = F11.NEXT.BAR.CODE$                     
                 
                 RC% = READ.NEXT.IEF (CURRENT.IEF.BAR.CODE$,  \          
                                     EXPCTD.NO.CODES%,    \          
                                     INPUT.BOOTS.CODE$)                           
                 IF RC% = 1 THEN BEGIN           
                     MSG$ = STR$(INPUT.REC.NO%) + ":" + UNPACK$(F11.NEXT.BAR.CODE$) + " ERROR - Associated Barcode " + STR$(INDEX%) + " on IDF"  !DCS
                     CALL DO.MESSAGE(MSG$)                                                                                                       !DCS
                     PRINT # WRN.SESS.NUM%; MSG$                                                                                                 !DCS
                     ! Carry on processing but set error override flag so that error is captured
                     ! ie. BLOCKITM.ERR file is produced
                     ERROR.OVERRIDE.FLAG$ = "Y"
                     SKIP.PAST.ITEM.DUE.TO.ERROR$ = "Y"                                                            
                     GENUINE.ERROR.FLAG$  = "Y"                                                                                                  !DCS
                     RETURN
                 ENDIF
                     
                 INDEX% = INDEX% + 1                                                      
             WEND                                             
       ENDIF
   ENDIF
      
RETURN



!**************************************************************************
!***
!***    TERMINATION
!***
!**************************************************************************

TERMINATION:

    CALL DO.MESSAGE("Termination")

    GOSUB RENAME.COMPLETION.FILE      
    
    RUN.DATE$   = FORMAT.DATE$(DATE$)                                             
    RUN.TIME$   = FORMAT.TIME$(TIME$)
    
    CALL DO.MESSAGE("BLOCKITM ended: " + RUN.DATE$ + " @ " + RUN.TIME$)
    CALL DO.MESSAGE("Completed Successfully")
   
    GOSUB CLOSE.FILES
    
RETURN


!**************************************************************************
!***
!***    ALLOCATE.SESSION.NUMBERS
!***
!**************************************************************************
                                                        
ALLOCATE.SESSION.NUMBERS:                               
                                                        
      CALL DO.MESSAGE("Allocate Session Numbers")       
      SB.ACTION$ = "O"
                                                                              
      SB.INTEGER% = IDF.REPORT.NUM%                                           !BCS
      SB.STRING$ = IDF.FILE.NAME$                                             !BCS
      GOSUB SB.FILE.UTILS                                                     !BCS
      IDF.SESS.NUM% = SB.FILE.SESS.NUM%                                       !BCS
      
      SB.INTEGER% = IEF.REPORT.NUM%                                           !BCS
      SB.STRING$ = IEF.FILE.NAME$                                             !BCS
      GOSUB SB.FILE.UTILS                                                     !BCS
      IEF.SESS.NUM% = SB.FILE.SESS.NUM%                                       !BCS
                                                                              
      SB.INTEGER% = IRF.REPORT.NUM%
      SB.STRING$ = IRF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      IRF.SESS.NUM% = SB.FILE.SESS.NUM%
      

      WRN.REPORT.NUM% = 995                                                   !DCS
      SB.INTEGER% = WRN.REPORT.NUM%                                           !DCS
      SB.STRING$  = BLOCKITM.WRN.PATH$                                        !DCS
      GOSUB SB.FILE.UTILS                                                     !DCS
      WRN.SESS.NUM% = SB.FILE.SESS.NUM%                                       !DCS
      
      INPUT.REPORT.NUM% = 996
      SB.INTEGER% = INPUT.REPORT.NUM%
      SB.STRING$  = BLOCKITM.INPUT.PATH$
      GOSUB SB.FILE.UTILS
      INPUT.SESS.NUM% = SB.FILE.SESS.NUM%
      
      OK.SESS.NUM% = SB.FILE.SESS.NUM%
      OK.REPORT.NUM% = 997
      SB.INTEGER% = OK.REPORT.NUM%
      SB.STRING$  = BLOCKITM.OK.PATH$
      GOSUB SB.FILE.UTILS
      OK.SESS.NUM% = SB.FILE.SESS.NUM%

      ERR.REPORT.NUM% = 998
      SB.INTEGER% = ERR.REPORT.NUM%
      SB.STRING$  = BLOCKITM.ERR.PATH$
      GOSUB SB.FILE.UTILS
      ERR.SESS.NUM% = SB.FILE.SESS.NUM%

      LOG.REPORT.NUM% = 999
      SB.INTEGER% = LOG.REPORT.NUM%
      SB.STRING$  = BLOCKITM.LOG.PATH$
      GOSUB SB.FILE.UTILS
      LOG.SESS.NUM% = SB.FILE.SESS.NUM%
      
RETURN

         
\*******************************************************************************
\***
\***    DELETE.COMPLETION.FILES:
\***
\***    Deletes BLOCKITM.OK  if it exists.
\***    Deletes BLOCKITM.ERR if it exists.
\***    Deletes BLOCKITM.LOG if it exists.
\***
\***............................................................................


DELETE.COMPLETION.FILES:

    IF END # OK.SESS.NUM% THEN BLOCKITM.OK.DELETED
    OPEN BLOCKITM.OK.PATH$ DIRECT RECL 40 AS OK.SESS.NUM%
    DELETE OK.SESS.NUM%

  BLOCKITM.OK.DELETED:

    IF END # ERR.SESS.NUM% THEN BLOCKITM.ERR.DELETED
    OPEN BLOCKITM.ERR.PATH$ DIRECT RECL 40 AS ERR.SESS.NUM%
    DELETE ERR.SESS.NUM%

  BLOCKITM.ERR.DELETED:

    IF END # LOG.SESS.NUM% THEN BLOCKITM.LOG.DELETED
    OPEN BLOCKITM.LOG.PATH$ DIRECT RECL 40 AS LOG.SESS.NUM%
    DELETE LOG.SESS.NUM%

  BLOCKITM.LOG.DELETED:
  
    IF END # WRN.SESS.NUM% THEN BLOCKITM.WRN.DELETED                           !DCS
    OPEN BLOCKITM.WRN.PATH$ DIRECT RECL 40 AS WRN.SESS.NUM%                    !DCS
    DELETE WRN.SESS.NUM%                                                       !DCS
                                                                               !DCS
  BLOCKITM.WRN.DELETED:                                                        !DCS

RETURN


\*****************************************************************************
\*** CREATE.COMPLETION.FILE
\*** Creates BLOCKITM.ERR completion file.
\***............................................................................
    
CREATE.COMPLETION.FILE:

   CURRENT.REPORT.NUM% = 999

   IF END # BLOCKITM.SESS.NUM% THEN FILE.ERROR

   CREATE POSFILE BLOCKITM.PATH$ DIRECT 1 RECL 60 \
          AS BLOCKITM.SESS.NUM% LOCAL
                                                                    
RETURN

            
\*****************************************************************************
\*** RENAME.COMPLETION.FILE
\*** Writes a message to completion file and renames the completion file from  
\*** .ERR to .OK
\***............................................................................
    
RENAME.COMPLETION.FILE:

    ! When set to "Y", the override flag indicates that some of the updates 
    ! to the IRF were NOT successful for some reason or other and needs
    ! investigating further.
    IF ERROR.OVERRIDE.FLAG$ = "N" THEN BEGIN
        BLOCKITM.COMPLETION.MSG$ = "BLOCKITM completed successfully!"                                      
    ENDIF ELSE BEGIN                                                                          !DCS
        BLOCKITM.COMPLETION.MSG$ = "BLOCKITM completed with WARNINGS - check C:\BLOCKITM.WRN" !DCS
    ENDIF                                                                                     !DCS
   
   CURRENT.REPORT.NUM% = 999

   IF END # BLOCKITM.SESS.NUM% THEN FILE.ERROR

   WRITE FORM "C60"; \
         # BLOCKITM.SESS.NUM%, 1;  \
           BLOCKITM.COMPLETION.MSG$ 

    !IF ERROR.OVERRIDE.FLAG$ = "N" THEN BEGIN  ! Only rename to .OK if no errors
    IF GENUINE.ERROR.FLAG$ = "N" THEN BEGIN  ! Only rename to .OK if no genuine errors     !DCS
        RC% = RENAME(BLOCKITM.OK.PATH$, BLOCKITM.PATH$)
        
        IF RC% THEN BEGIN
            CALL DO.MESSAGE("Renaming of BLOCKITM.ERR to BLOCKITM.OK ** SUCCESSFUL **")
        ENDIF ELSE BEGIN
            CALL DO.MESSAGE("Renaming of BLOCKITM.ERR to BLOCKITM.OK ** FAILED **")
        ENDIF
    ENDIF

   CLOSE BLOCKITM.SESS.NUM%
                                                                          
   
RETURN


\******************************************************************************
\***
\***   SB.FILE.UTILS:
\***
\***   Allocate/report/de-allocate a file session number
\***
\******************************************************************************

SB.FILE.UTILS:

       RC% = SESS.NUM.UTILITY(SB.ACTION$,                              \
                              SB.INTEGER%,                             \
                              SB.STRING$)

       IF SB.ACTION$ = "O" THEN BEGIN
          SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
       ENDIF ELSE BEGIN
          IF SB.ACTION$ = "R" THEN BEGIN
             SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
             SB.FILE.NAME$ = F20.FILE.NAME$
          ENDIF
       ENDIF

RETURN

\******************************************************************************
\***
\***   OPEN.FILES:
\***
\******************************************************************************
OPEN.FILES:

    CALL DO.MESSAGE("Open Files")

!RESUME.PROCESSING:

    CURRENT.CODE$ = " "
    FILE.OPERATION$ = "O"
                                              !--------!
    CURRENT.REPORT.NUM% = INPUT.REPORT.NUM%   ! INPUT  !
    IF END #INPUT.SESS.NUM% THEN FILE.ERROR   !--------!
    OPEN BLOCKITM.INPUT.PATH$ DIRECT RECL INPUT.RECL% AS INPUT.SESS.NUM% BUFFSIZE 32767
    
                                              !--------!                     !BCS
    CURRENT.REPORT.NUM% = IDF.REPORT.NUM%     ! IDF    !                     !BCS
    IF END #IDF.SESS.NUM% THEN FILE.ERROR     !--------!                     !BCS
    OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%  NOWRITE NODEL !BCS
    
                                              !--------!                     !BCS
    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%     ! IEF    !                     !BCS
    IF END #IEF.SESS.NUM% THEN FILE.ERROR     !--------!                     !BCS
    OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% AS IEF.SESS.NUM%  NOWRITE NODEL !BCS
        
                                              !--------!
    CURRENT.REPORT.NUM% = IRF.REPORT.NUM%     ! IRF    !
    IF END #IRF.SESS.NUM% THEN FILE.ERROR     !--------!
    OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM% NODEL
        

RETURN

\******************************************************************************
\***
\***   CLOSE.FILES:
\***
\******************************************************************************
CLOSE.FILES:
    
    CALL DO.MESSAGE("Closing Files")
    
    CURRENT.CODE$ = " "
    FILE.OPERATION$ = "C"
    
    CURRENT.REPORT.NUM% = INPUT.REPORT.NUM%
    CLOSE INPUT.SESS.NUM%
    
    CURRENT.REPORT.NUM% = IDF.REPORT.NUM%                                    !BCS
    CLOSE IDF.SESS.NUM%                                                      !BCS
    
    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%                                    !BCS
    CLOSE IEF.SESS.NUM%                                                      !BCS
    
    CURRENT.REPORT.NUM% = IRF.REPORT.NUM%
    CLOSE IRF.SESS.NUM%
    
    CURRENT.REPORT.NUM% = LOG.REPORT.NUM%
    CLOSE LOG.SESS.NUM%
    
    CURRENT.REPORT.NUM% = WRN.REPORT.NUM%                                    !DCS
    CLOSE WRN.SESS.NUM%                                                      !DCS

RETURN

\*******************************************************************************
\***
\***   FILE.ERROR:
\***
\***   Open Read or Write File error
\***
\-------------------------------------------------------------------------------

FILE.ERROR:
        
    CALL DO.MESSAGE("File Error")

    GOSUB LOG.EVENT.106

GOTO PROGRAM.ABEND


!!!*************************************************************************
!!!*************************************************************************
!!!
!!!   PSBF30.ERROR
!!!
!!!   Process keyed file error
!!!
!!!*************************************************************************
!!!*************************************************************************

PSBF30.ERROR: 

    CALL DO.MESSAGE("Processed Keyed File Error")
    EVENT.NUM% = 89
    VAR.STRING.1$ = "PSBF30: " + STR$(RC%)
    VAR.STRING.2$ = " "
    RC% = APPLICATION.LOG (MESSAGE.NUMBER%,  \
                           VAR.STRING.1$,    \
                           VAR.STRING.2$,    \
                           EVENT.NUM%)
    CALL DO.MESSAGE("Update Failed - " + VAR.STRING.1$)

RETURN

                                  
\*******************************************************************************
\***
\***   LOG.EVENT.106 open,read,write error
\***
\-------------------------------------------------------------------------------

LOG.EVENT.106:

    EVENT.NUM% = 106

    VAR.STRING.1$ = FILE.OPERATION$                    +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +  \
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +  \
                    CURRENT.CODE$

    VAR.STRING.2$ = ""
    GOSUB CALL.APPLICATION.LOG

RETURN    



\******************************************************************************
\***
\***   CALL.APPLICATION.LOG:
\***
\***
\------------------------------------------------------------------------------

CALL.APPLICATION.LOG:

          RC% = APPLICATION.LOG (MESSAGE.NUMBER%,  \
                                 VAR.STRING.1$,    \
                                 VAR.STRING.2$,    \
                                 EVENT.NUM%)

RETURN

\*****************************************************************************
\***                                                                         *
\***   ERROR ROUTINE  :  ERROR.DETECTED                                      *
\***                                                                         *
\***                                                                         *
\***   IF another error detected then EXIT PROGRAM                           *
\***                                                                         *
\***   call STANDARD.ERROR.DETECTED                                          *
\***   gosub DISPLAY.ERROR.MESSAGE                                           *
\***                                                                         *
\*****************************************************************************

ERROR.DETECTED:

    ERROR.COUNT% = ERROR.COUNT% + 1

    IF ERROR.COUNT% <= 3 THEN BEGIN
    
        IF (ERRN AND 0000FFFFH) = 0000400CH THEN BEGIN \   ! Trap all other file access conflicts
                
            IF CURRENT.REPORT.NUM% = IDF.REPORT.NUM% OR \                            !BCS
               CURRENT.REPORT.NUM% = IEF.REPORT.NUM% OR \                            !BCS
               CURRENT.REPORT.NUM% = IRF.REPORT.NUM% THEN BEGIN                      !BCS
                 CALL DO.MESSAGE("IDF/IEF/IRF File is locked - Retry " + STR$(ERROR.COUNT%) + " of 3") !BCS
            ENDIF ELSE BEGIN
                 CALL DO.MESSAGE("A File is locked - Retry " + STR$(ERROR.COUNT%) + " of 3")
            ENDIF
            
            WAIT ;20000    ! Wait for 20 seconds and then retry
            
        ENDIF   
        
        RESUME RETRY
        
    ENDIF   
                                                                       
    IF (ERRN AND 0000FFFFH) = 0000400CH THEN BEGIN \   ! Log Event 106 for file access conflicts
        GOSUB LOG.EVENT.106
    ENDIF 
    
    \*****************************************************************************
    \***                                                                         *
    \***   PROGRAM.ABEND                                                         *
    \***                                                                         *
    \*****************************************************************************
    
    PROGRAM.ABEND:

    PRINT "ERR:  "; ERR
    PRINT "ERRN: "; ERRNH
    PRINT "ERRF: "; ERRF%
    PRINT "ERRL: "; ERRL
    
    CALL DO.MESSAGE("Program BLOCKITM has abended. Check App. Log")

    ! Catch any uncaught events
    RC% = STANDARD.ERROR.DETECTED(ERRN,   \
                                  ERRF%,  \
                                  ERRL,   \
                                  ERR)
    
END
