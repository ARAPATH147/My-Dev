REM \
\******************************************************************************
\******************************************************************************
\***
\***         ITEM SHELF EDGE DESCRIPTION FILE FUNCTIONS
\***
\***                 REFERENCE    : ISFFUN.BAS
\***             
\***
\***   VERSION A       Jamie Thorpe            25th June 1997
\***   Removed version letter from ISFDEC function.
\***
\***   VERSION B       Rebecca Dakin           23rd June 1999
\***   Changes have been made to data held on the ISF to include new Unit Pricing 
\***   details. Extra processing has been included to fit the new data into the 
\***   6 bytes of available space previously taken up by filler. This means the 
\***   file size will not need to change, eliminating disk space problems.
\***   Processing of a new file, Item Unit Price Description file (IUDF), has been 
\***   included. This file holds details of the Unit Name (eg. ml).
\***   The new details will be held in the ISF as follows:-
\***   INTEGER.4% (4 bytes) :-  
\***       ISF.UNIT.NAME$ (1st byte) number referencing the Unit Name on the IUDF.
\***       ISF.ITEM.QTY$  (remaining 3 bytes) 
\***   INTEGER.2% (2 bytes) :-
\***       ISF.SEL.PRINTED.FLAG$ (leftmost bit if 2 bytes) indicates ON or OFF
\***       ISF.UNIT.MEASUREMENT$ (value of the rest of ISF.INTEGER.2%)
\***   
\***   VERSION C       David Artiss            18th November 2002
\***   ISF.ITEM.QTY$ read modified to allow the full 24 bits to be used as 
\***   a number, rather than the old method of 23 bits and a single bit for
\***   a sign.
\***
\******************************************************************************
\*******************************************************************************

                          
  %INCLUDE ISFDEC.J86
               
  STRING GLOBAL                                                        \
     FILE.OPERATION$,                                                  \
     CURRENT.CODE$,                                                    \
     F20.FUNCTION$,                                                    \ BRD
     F20.STRING$                                                       ! BRD
     
  INTEGER*2 GLOBAL                                                     \
     CURRENT.REPORT.NUM%,                                              \
     F20.INTEGER%                                                      ! BRD
     
  INTEGER*2                                                            \ BRD
     RC%                                                               ! BRD
     
  %INCLUDE PSBF20G.J86                                                 ! BRD 
  
  %INCLUDE PSBF20E.J86                                                 ! BRD 
  %INCLUDE PSBF24E.J86                                                 ! BRD     
                                                                 
  
  FUNCTION ISF.SET PUBLIC
\*************************

    INTEGER*2 ISF.SET 
    
    ISF.SET = 1                                                           ! BRD
    
    ON ERROR GOTO ERROR.DETECTED                                          ! BRD
    
    ISF.REPORT.NUM% = 9  
    ISF.RECL%       = 55
    ISF.FILE.NAME$  = "ISF"
    
    IUDF.REPORT.NUM%= 586                                                 ! BRD
    IUDF.RECL%      = 10                                                  ! BRD
    IUDF.FILE.NAME$ = "IUDF"                                              ! BRD
    
    ! ALLOCATE SESSION NUMBER for IUDF                                    ! BRD
    F20.FUNCTION$ = "O"                                                   
    F20.INTEGER% = IUDF.REPORT.NUM%                                       ! BRD
    F20.STRING$ = IUDF.FILE.NAME$                                         ! BRD
    RC% = SESS.NUM.UTILITY(F20.FUNCTION$,                                 \ BRD
                           F20.INTEGER%,                                  \ BRD
                           F20.STRING$)                                   ! BRD
    IUDF.SESS.NUM% = F20.INTEGER.FILE.NO%                                 ! BRD
    
    IUDF.FILE.SIZE% = SIZE(IUDF.FILE.NAME$)                               ! BRD
    
    OPEN IUDF.FILE.NAME$ DIRECT RECL IUDF.FILE.SIZE% AS IUDF.SESS.NUM%    \ BRD
         LOCKED                                                           ! BRD
    
    RECORD.FORMAT$ = "C" + STR$(IUDF.FILE.SIZE%)                          ! BRD
    READ FORM RECORD.FORMAT$; # IUDF.SESS.NUM%, 1; IUDF.RECORD$           ! BRD
    
    CLOSE IUDF.SESS.NUM%                                                  ! BRD
    
    ! DE-ALLOCATE SESSION NUMBER                                          ! BRD
    F20.FUNCTION$ = "C"                                                   ! BRD
    F20.INTEGER% = IUDF.SESS.NUM%                                         ! BRD
    RC% = SESS.NUM.UTILITY(F20.FUNCTION$,                                 \ BRD
                              F20.INTEGER%,                               \ BRD
                              F20.STRING$)                                ! BRD
                              
    ISF.SET = 0                                                           ! BRD
    
    END.OF.FUNCTION:                                                      ! BRD
                              
    EXIT FUNCTION                                                         ! BRD
    
\******************************************************************************  
\***   ERROR.DETECTED
\******************************************************************************
 
ERROR.DETECTED:                                                           ! BRD

   CALL STANDARD.ERROR.DETECTED (ERRN,                                    \ BRD
                                 ERRF%,                                   \ BRD
                                 ERRL,                                    \ BRD
                                 ERR)                                     ! BRD

   RESUME END.OF.FUNCTION                                                 ! BRD                       
                                                                           
   EXIT FUNCTION                                                          ! BRD
   
   END FUNCTION                                                           ! BRD
   
\------------------------------------------------------------------------------
REM EJECT^L      


   
  FUNCTION READ.ISF PUBLIC
\**************************

    INTEGER*2 READ.ISF 
    
    READ.ISF = 1    
    
    IF END #ISF.SESS.NUM% THEN READ.ERROR
    READ FORM "T5,C45,I4,I2"; #ISF.SESS.NUM%                              \
         KEY ISF.BOOTS.CODE$;                                             \
             ISF.S.E.DESC$,                                               \
             ISF.INTEGER.4%,                                              \ BRD
             ISF.INTEGER.2%                                               ! BRD
             
    READ.ISF = 0
    
    IF ISF.INTEGER.4% = 20202020H AND ISF.INTEGER.2% = 2020H THEN BEGIN   ! BRD
       ISF.INTEGER.4% = 0                                                 ! BRD
       ISF.INTEGER.2% = 0                                                 ! BRD
    ENDIF                                                                 ! BRD   
       
    ! Extract ISF.UNIT.NAME$                                              ! BRD
    UNIT.NAME.COUNTER% = ISF.INTEGER.4% AND 000000FFH                     ! BRD
    
    IF UNIT.NAME.COUNTER% > 0 THEN BEGIN
       IUDF.RECORD.POSITION% = ((UNIT.NAME.COUNTER% - 1) * IUDF.RECL%) + 1! BRD
       ISF.UNIT.NAME$ = MID$(IUDF.RECORD$,IUDF.RECORD.POSITION%,IUDF.RECL%)! BRD
    ENDIF ELSE BEGIN                                                      ! BRD
       ISF.UNIT.NAME$ = "          "                                      ! BRD
    ENDIF   
    
    ! Extract ISF.ITEM.QTY$                                               ! BRD
    ISF.ITEM.QTY$ = STR$(SHIFT(ISF.INTEGER.4%, 8) AND 0FFFFFFH)           ! CDA
    ISF.ITEM.QTY$ = PACK$(RIGHT$("00000000" + ISF.ITEM.QTY$,8))           ! BRD
    
    ! Extract ISF.SEL.PRINTED.FLAG$                                       ! BRD
    PRINT.FLAG% = ISF.INTEGER.2% AND 8000H                                ! BRD
    IF PRINT.FLAG% = 0 THEN BEGIN                                         ! BRD
       ISF.SEL.PRINTED.FLAG$ = "N"                                        ! BRD
    ENDIF ELSE BEGIN                                                      ! BRD
       ISF.SEL.PRINTED.FLAG$ = "Y"                                        ! BRD
    ENDIF                                                                 ! BRD
   
    ! Extract ISF.UNIT.MEASUREMENT$                                       ! BRD
    ISF.UNIT.MEASUREMENT$ = STR$(ISF.INTEGER.2% AND 7FFFH)                ! BRD
    ISF.UNIT.MEASUREMENT$ = PACK$(RIGHT$("0000" + ISF.UNIT.MEASUREMENT$,4))!BRD
    
    EXIT FUNCTION
    
    
    READ.ERROR:
    
       FILE.OPERATION$     = "R"
       CURRENT.CODE$       = PACK$("00000000") + ISF.BOOTS.CODE$
       CURRENT.REPORT.NUM% = ISF.REPORT.NUM% 
       
       EXIT FUNCTION    
        

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION WRITE.ISF PUBLIC
\***************************  

    INTEGER*2 WRITE.ISF
    
    WRITE.ISF = 1
    
    ! ISF.INTEGER.4% CALCULATIONS:                                        ! BRD
    IUDF.RECORD.POSITION% = MATCH(ISF.UNIT.NAME$,IUDF.RECORD$,1)          ! BRD
    
    IF IUDF.RECORD.POSITION% > 0 THEN BEGIN                               ! BRD
       ISF.INTEGER.4% = ((IUDF.RECORD.POSITION% - 1) / 10) + 1            ! BRD
    ENDIF ELSE BEGIN                                                      ! BRD
       ISF.INTEGER.4% = 0                                                 ! BRD
    ENDIF                                                                 ! BRD
          
    ISF.INTEGER.4% = ISF.INTEGER.4% + (VAL(UNPACK$(ISF.ITEM.QTY$)) * 256) ! BRD    
                                                        
    ! ISF.INTEGER.2% CALCULATIONS:                                        ! BRD
    ISF.INTEGER.2% = VAL(UNPACK$(ISF.UNIT.MEASUREMENT$))                  ! BRD
    IF ISF.SEL.PRINTED.FLAG$ = "Y" THEN BEGIN                             ! BRD
       ISF.INTEGER.2% = 8000H OR ISF.INTEGER.2%                           ! BRD
    ENDIF                                                                 ! BRD
    
    IF END #ISF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C4,C45,I4,I2"; #ISF.SESS.NUM%;                            \
             ISF.BOOTS.CODE$,                                             \
             ISF.S.E.DESC$,                                               \
             ISF.INTEGER.4%,                                              \ BRD
             ISF.INTEGER.2%                                               ! BRD
             
    WRITE.ISF = 0    
    
    EXIT FUNCTION
    
    
    WRITE.ERROR:
    
       FILE.OPERATION$     = "W"
       CURRENT.CODE$       = PACK$("00000000") + ISF.BOOTS.CODE$
       CURRENT.REPORT.NUM% = ISF.REPORT.NUM%                  
    
       EXIT FUNCTION                 

  END FUNCTION
  
  

