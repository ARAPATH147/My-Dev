\************************************************************************
\***
\***    SOFON            Ranjith Gopalankutty          08/05/2018        
\***                                                                     
\***    This utility will check if a store is closed by reading SSCLS    
\***    If so will check if any of the tills are signed on. If so report 
\***    it to the log file.   
\***
\***    Version A        Ranjith Gopalankutty         11/07/2018
\***    Addressed the comments on indent and not writing an OK file, now 
\***    the program writes an output called SOFON.OK if the error        
\***    is '0'
\*********************************************************************** 
     
%INCLUDE BEMFDEC.J86
%INCLUDE BOOTSDEC.J86
%INCLUDE PSBF01G.J86
%INCLUDE PSBF20G.J86
%INCLUDE SSCLSDEC.J86
%INCLUDE TSFDEC.J86
 

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
    TERMS.LOG.NAME$,    \
    TERMS.OK.NAME$,     \
    FUNCTION.FLAG$,     \
    FILE.NAME$,         \
    PATTERN$,           \
    FILE.RECORD$
    
       

INTEGER*4               \
    SOFON.SESS.NUM%,    \
    TERMS.OK.SESS%,     \
    TERMS.SESS.NUM%,    \
    FILE.RECL%,         \
    OPTIONS%,           \
    OFFSET%,            \
    MODE%,              \
    FID%,               \
    RECORD.COUNT%,      \
    READKF.RC%,         \
    TERMKF.RC%

 ! REAL END.OF.FILE                                                     ! ARG
 
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
%INCLUDE TSFEXT.J86
%INCLUDE SSCLSEXT.J86
%INCLUDE KFASMEXT.J86
%INCLUDE BTCMEM.J86

ON ERROR GOTO ERROR.DETECTED

    PRINT  TIME.STAMP$(2)  + "-Till sign on check starts"

    GOSUB INITIALISATION     
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION
 
STOP.PROGRAM:

    STOP

INITIALISATION:
    ERROR.COUNT% = 0
    CALL SSCLS.SET
    CALL TSF.SET
    TERMS.LOG.NAME$ = "ADXLXACN::C:/TERMS.LOG"
    TERMS.SESS.NUM% = 456   
    TERMS.OK.NAME$  = "ADXLXACN::C:/TERMS.ERR"
    TERMS.OK.SESS%  = 457
    SOFON.SESS.NUM% = 458
    

    GOSUB ALLOCATE.SESSION.NUMBERS

    IF END #TERMS.SESS.NUM% THEN NO.FILE 
    OPEN TERMS.LOG.NAME$ AS TERMS.SESS.NUM%  APPEND

    CREATE POSFILE TERMS.OK.NAME$ AS TERMS.OK.SESS%

RETURN

NO.FILE:

    CREATE POSFILE TERMS.LOG.NAME$ AS TERMS.SESS.NUM% 

RETURN

ALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ = "O"                           

    PASSED.INTEGER% = SSCLS.REPORT.NUM%                                
    PASSED.STRING$  = SSCLS.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SSCLS.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    !PASSED.INTEGER% = TERMS.REPORT.NUM%                                ! ARG
    !PASSED.STRING$  = TERMS.LOG.NAME$                                  ! ARG
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! ARG
    !TERMS.SESS.NUM% = F20.INTEGER.FILE.NO%                             ! ARG

    !PASSED.INTEGER% = TERMS.OK.SESS%                                   ! ARG
    !PASSED.STRING$  = TERMS.OK.NAME$                                   ! ARG
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! ARG
    !TERMS.OK.SESS% = F20.INTEGER.FILE.NO%                              ! ARG
                              

RETURN	

CALL.F20.SESS.NUM.UTILITY: 
        
    FILE.OPERATION$ = FUNCTION.FLAG$
    CURRENT.REPORT.NUM% = PASSED.INTEGER%
    RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
    IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN

MAIN.PROCESSING:

    OPEN SSCLS.FILE.NAME$ DIRECT RECL 16 AS SSCLS.SESS.NUM% NODEL

    SSCLS.REC.NO% = 1           
    RC% = READ.SSCLS           

    IF RC% = 0 THEN BEGIN
       
        IF SSCLS.STORE.CLOSED$  = "Y" THEN BEGIN
            GOSUB READ.TERMS
    
        ENDIF ELSE BEGIN
    
            PRINT DATE$ + "-" + TIME.STAMP$(2) + "-Store is not closed , not reading TSF"
            PRINT #TERMS.SESS.NUM% ; DATE$ + "-" + TIME.STAMP$(2) + "-Store is not closed , not reading TSF"
        ENDIF
    ENDIF

RETURN

READ.TERMS:

    FILE.NAME$ = "EALTERMS"
    FILE.RECL% = 63
    OPTIONS%   = 201AH
    PATTERN$   = ""
    OFFSET%    = 0
    MODE%      = 0
 
    FID% = INITKF(FILE.NAME$,OPTIONS%)

    ! Check for errors opening file
    IF FID% <= 0 THEN BEGIN
        PRINT #TERMS.OK.SESS% ; "OPENING ERROR"
        STOP
    ENDIF

    RECORD.COUNT% = 0

    END.OF.FILE% = 0

    ! Continue until end of file reached
    WHILE NOT END.OF.FILE%

        FILE.RECORD$ = STRING$(FILE.RECL%,CHR$(0))

        READKF.RC% = READKF(FILE.RECORD$,PATTERN$,OFFSET%,MODE%)

        IF READKF.RC% = 0 THEN BEGIN

            TSF.TERM.STORE$        = LEFT$(FILE.RECORD$,2)

            IF TSF.TERM.STORE$ <> PACK$("9999") THEN BEGIN

                 TSF.INDICAT0%          = ASC(MID$(FILE.RECORD$,3,1))  ! ARG
            !    TSF.INDICAT1%          = ASC(MID$(FILE.RECORD$,4,1))  ! ARG
            !    TSF.GROSSPOS           = GETN4(FILE.RECORD$,4)        ! ARG
            !    TSF.GROSSNEG           = GETN4(FILE.RECORD$,8)        ! ARG
            !    TSF.NETCASH            = GETN4(FILE.RECORD$,12)       ! ARG
            !    TSF.NETNCASH           = GETN4(FILE.RECORD$,16)       ! ARG
            !    TSF.AMTLOAN            = GETN4(FILE.RECORD$,20)       ! ARG
            !    TSF.AMTPICKU           = GETN4(FILE.RECORD$,24)       ! ARG
            !    TSF.AMTCASHC           = GETN4(FILE.RECORD$,28)       ! ARG
            !    TSF.OPERATOR$          = MID$(FILE.RECORD$,33,4)      ! ARG
            !    TSF.TRANSNUM$          = MID$(FILE.RECORD$,37,2)      ! ARG
            !    TSF.SIGN.OFF.DELAY$    = MID$(FILE.RECORD$,39,1)      ! ARG
            !    TSF.INDICAT2%          = GETN2(FILE.RECORD$,39)       ! ARG
            !    TSF.USER$              = MID$(FILE.RECORD$,42,5)      ! ARG
            !    TSF.CONTROLLER$        = MID$(FILE.RECORD$,47,2)      ! ARG
            !    TSF.NETCCURR           = GETN4(FILE.RECORD$,48)       ! ARG
            !    TSF.NETCCCURR          = GETN4(FILE.RECORD$,52)       ! ARG
            !    TSF.SPACE$             = MID$(FILE.RECORD$,57,7)      ! ARG
                 
                 
                IF  TSF.INDICAT0%  = 8 THEN BEGIN

                    PRINT  DATE$ + "-" + TIME.STAMP$(2) + "-" + UNPACK$(TSF.TERM.STORE$) + " IS IN SIGNED ON STATUS"
                    PRINT #TERMS.SESS.NUM% ; DATE$ + "-" + TIME.STAMP$(2) + "-" + UNPACK$(TSF.TERM.STORE$) + " IS IN SIGNED ON STATUS" 

                    RECORD.COUNT% = RECORD.COUNT% + 1
                ENDIF
  
            ENDIF
  
        ENDIF ELSE BEGIN 
  
            IF READKF.RC% = -1 THEN BEGIN
                END.OF.FILE% = -1
            ENDIF ELSE BEGIN
            ! Check whether an error has been returned
                IF READKF.RC% < -1 THEN BEGIN
                    PRINT #TERMS.OK.SESS% ; "READKF ERROR " 
                    STOP
                ENDIF
            ENDIF                                

        ENDIF

    WEND

    TERMKF.RC% = TERMKF(FID%)

    IF TERMKF.RC% < 0 THEN BEGIN
        PRINT #TERMS.OK.SESS% ; "File close failed"
    ENDIF
 
RETURN

DEALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ = "C"                                               

    PASSED.INTEGER% = SSCLS.REPORT.NUM%                                
    PASSED.STRING$  = SSCLS.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SSCLS.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    !PASSED.INTEGER% = TERMS.REPORT.NUM%                                ! ARG
    !PASSED.STRING$  = TERMS.LOG.NAME$                                  ! ARG
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! ARG
    !TERMS.SESS.NUM% = F20.INTEGER.FILE.NO%                             ! ARG

    !PASSED.INTEGER% = TERMS.OK.SESS%                                   ! ARG
    !PASSED.STRING$  = TERMS.OK.NAME$                                   ! ARG
    !GOSUB CALL.F20.SESS.NUM.UTILITY                                    ! ARG
    !TERMS.OK.SESS% = F20.INTEGER.FILE.NO%                              ! ARG
                              

RETURN

TERMINATION:

    IF RECORD.COUNT% = 0 THEN BEGIN
        PRINT #TERMS.SESS.NUM% ; DATE$ + "-" + TIME.STAMP$(2) + "-No tills are in signed on status"
    ENDIF 
    
    IF ERROR.COUNT% = 0 THEN BEGIN
       CREATE POSFILE "C:\SOFON.OK" AS SOFON.SESS.NUM%
    ENDIF
     
    GOSUB DEALLOCATE.SESSION.NUMBERS
    CLOSE TERMS.OK.SESS%
    CLOSE TERMS.SESS.NUM%
    CLOSE SSCLS.SESS.NUM%

RETURN

ERROR.DETECTED:

    ERROR.COUNT% = ERROR.COUNT%  + 1 
    PRINT #TERMS.OK.SESS% ; ERR
    PRINT #TERMS.OK.SESS% ; ERRF%
    PRINT #TERMS.OK.SESS% ; ERRL

! RETURN                                                                !ARG