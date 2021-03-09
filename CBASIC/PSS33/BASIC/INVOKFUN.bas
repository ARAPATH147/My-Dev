REM \
\******************************************************************************
\******************************************************************************
\***
\***                   INVCE OK FILE FUNCTIONS
\***
\***                  REFERENCE  : INVOKFUN.BAS
\***
\***    Version A         Andrew Wedgeworth             3rd August 1992
\***
\***    Version B         Stephen Kelsey (CTG)          9th October 1992
\***    Add the functions WRITE.INVOK, READ.INVOK.LOCKED and WRITE.INVOK.UNLOCK
\***    Include the new PSC30 fields.
\***
\***    Version C         Les Cook                     15th February 1993
\***    Add new fields to all functions.
\***
\***    Version D         David Smallwood              20th July 1994    
\***    Add CSR PHASE1 transmission flag to all functions.              
\***
\***    Version E         Mick Bayliss                 19th Oct. 1994
\***    Add new CSR Phase 2 PSC14 Flag and Conversion Status Flag to all
\***    functions. Also error in format corrected by adding in missing
\***    fields.
\***
\***    Version F         Andrew Wedgeworth            27th March 1997
\***    Added date of successful processing of Advantage Card Points Events
\***    data sent from the mainframe.  This is updated by PSB59.
\***
\***    Version G         Nik Sen                      10th February 1998
\***    Added parameter for MINSITS RP late delivery days.
\***
\******************************************************************************
\*******************************************************************************

  STRING GLOBAL                                                        \
     CURRENT.CODE$,                                                    \ 
     FILE.OPERATION$

  INTEGER*2 GLOBAL                                                     \
     CURRENT.REPORT.NUM%  


  %INCLUDE INVOKDEC.J86                                                ! DDS
  

  FUNCTION INVOK.SET PUBLIC
\***************************

    INVOK.REPORT.NUM%  = 89
    INVOK.RECL%      = 80
    INVOK.FILE.NAME$ = "INVOK"

  END FUNCTION
\-------------------------------------------------------------------------------  
REM EJECT    

  FUNCTION READ.INVOK PUBLIC
\****************************

    STRING FORMAT$                                                    !BSPK
    INTEGER*2 READ.INVOK
    
    READ.INVOK = 1  
    FORMAT$ = "C5,C3,C1,C4,C5,C1,C5,C1,C5,C1,C1,C5,C6"                !BSPK
    FORMAT$ = FORMAT$ +",C1,C1,C1,C1,C3,C1,C1,C3,C1,C1,C3,"   \       !EMJB
                      +"C5,C1,C1,C1,C1,C1,C3,C1,C6"                   !GNS 

    IF END #INVOK.SESS.NUM% THEN ERROR.READ.INVOK
    READ FORM FORMAT$;                                                \BSPK
                 #INVOK.SESS.NUM%                                       \
                 ,1;                                                    \
                 INVOK.SERIAL.NO$,                                      \ 
                 INVOK.DATE$,                                           \ 
                 INVOK.SUCCESS.FLAG$,                                   \ 
                 INVOK.STORE.NO$,                                       \ 
                 INVOK.INVENTORY.SRLNO$,                                \ 
                 INVOK.INVENTORY.SUCCESS$,                              \ 
                 INVOK.SALES.SRLNO$,                                    \ 
                 INVOK.SALES.SUCCESS$,                                  \ 
                 INVOK.NEW.LIST.SRLNO$,                                 \ 
                 INVOK.NEW.LIST.SUCCESS$,                               \ 
                 INVOK.CSR.IDENT$,                                      \ 
                 INVOK.CSR.DELIVERY.NO$,                                \ 
                 INVOK.CSR.DELIVERY.DATE$,                              \ 
                 INVOK.CSR.PSC11.FLAG$,                                 \ 
                 INVOK.CSR.PSC12.FLAG$,                                 \ 
                 INVOK.CSR.PSC13.FLAG$,                                 \ 
                 INVOK.CSR.PSC12.DAYS$,                                 \ 
                 INVOK.PSS33.RUN.DATE$,                                 \ 
                 INVOK.PSS33.SUCCESS.FLAG$,                             \ 
                 INVOK.DIR.IMPL.FLAG$,                                  \ 
                 INVOK.PSC30.RUN.DATE$,                                 \ BSPK
                 INVOK.PSC30.SUCCESS.FLAG$,                             \ BSPK
                 INVOK.UOD.IMPL.FLAG$,                 \                  CLC
                 INVOK.LAST.UOD.DATE$,                 \                  CLC
                 INVOK.PREV.SERIAL.NO$,                \                  CLC
                 INVOK.PREV.SUCCESS.FLAG$,             \                  CLC
                 INVOK.SUPPRESS.EXCEP.REPORT$,         \                  DDS
                 INVOK.CSR.STARTED.BY.SUP$,            \                  DDS       
                 INVOK.CSR.PSC14.FLAG$,                \              !EMJB
                 INVOK.CSR.CONVERSION.STATUS.FLAG$,    \              !EMJB
                 INVOK.PTS.EVENTS.OK.DATE$,            \               FAW
                 INVOK.RP.DAYS$,                       \               GNS
                 INVOK.FILLER$                                         
    
    READ.INVOK = 0
    EXIT FUNCTION
    
    
    ERROR.READ.INVOK:
    
       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = INVOK.REPORT.NUM%           
       CURRENT.CODE$       = PACK$("0000000000000000")
       
       EXIT FUNCTION             
    
  END FUNCTION
\----------------------------------------------------------------------
REM EJECT    

 FUNCTION READ.INVOK.LOCKED PUBLIC                                     ! BSPK
\*********************************

    STRING    FORMAT$                                                  ! BSPK
    INTEGER*2 READ.INVOK.LOCKED                                        ! BSPK

    READ.INVOK.LOCKED = 1                                               ! BSPK
    FORMAT$ = "C5,C3,C1,C4,C5,C1,C5,C1,C5,C1,C1,C5,C6"                  ! BSPK
    FORMAT$ = FORMAT$ +",C1,C1,C1,C1,C3,C1,C1,C3,C1,C1,C3,"   \       !EMJB
                      +"C5,C1,C1,C1,C1,C1,C3,C1,C6"                     !GNS 

    IF END #INVOK.SESS.NUM% THEN ERROR.READ.INVOK.LOCKED                ! BSPK
    READ FORM FORMAT$;                                                  \ BSPK
                 #INVOK.SESS.NUM%                                       \ BSPK
                 AUTOLOCK,1;                                            \ BSPK
                 INVOK.SERIAL.NO$,                                      \ BSPK
                 INVOK.DATE$,                                           \ BSPK
                 INVOK.SUCCESS.FLAG$,                                   \ BSPK
                 INVOK.STORE.NO$,                                       \ BSPK
                 INVOK.INVENTORY.SRLNO$,                                \ BSPK
                 INVOK.INVENTORY.SUCCESS$,                              \ BSPK
                 INVOK.SALES.SRLNO$,                                    \ BSPK
                 INVOK.SALES.SUCCESS$,                                  \ BSPK
                 INVOK.NEW.LIST.SRLNO$,                                 \ BSPK
                 INVOK.NEW.LIST.SUCCESS$,                               \ BSPK
                 INVOK.CSR.IDENT$,                                      \ BSPK
                 INVOK.CSR.DELIVERY.NO$,                                \ BSPK
                 INVOK.CSR.DELIVERY.DATE$,                              \ BSPK
                 INVOK.CSR.PSC11.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC13.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.DAYS$,                                 \ BSPK
                 INVOK.PSS33.RUN.DATE$,                \              !EMJB
                 INVOK.PSS33.SUCCESS.FLAG$,            \              !EMJB
                 INVOK.DIR.IMPL.FLAG$,                 \              !EMJB    
                 INVOK.PSC30.RUN.DATE$,                                 \ BSPK
                 INVOK.PSC30.SUCCESS.FLAG$,                             \ BSPK
                 INVOK.UOD.IMPL.FLAG$,                 \                  CLC
                 INVOK.LAST.UOD.DATE$,                 \                  CLC
                 INVOK.PREV.SERIAL.NO$,                \                  CLC
                 INVOK.PREV.SUCCESS.FLAG$,             \                  CLC
                 INVOK.SUPPRESS.EXCEP.REPORT$,         \                  DDS
                 INVOK.CSR.STARTED.BY.SUP$,            \                  DDS       
                 INVOK.CSR.PSC14.FLAG$,                \              !EMJB
                 INVOK.CSR.CONVERSION.STATUS.FLAG$,    \              !EMJB
                 INVOK.PTS.EVENTS.OK.DATE$,            \               FAW
                 INVOK.RP.DAYS$,                       \               GNS
                 INVOK.FILLER$                                         
  
       READ.INVOK.LOCKED = 0 

       EXIT FUNCTION                                                    ! BSPK
    
ERROR.READ.INVOK.LOCKED:                                                ! BSPK
    
       FILE.OPERATION$     = "R"                                        ! BSPK
       CURRENT.REPORT.NUM% = INVOK.REPORT.NUM%                          ! BSPK
       CURRENT.CODE$       = PACK$("0000000000000000")                  ! BSPK
       
       EXIT FUNCTION                                                    ! BSPK
    
  END FUNCTION                                                          ! BSPK
\----------------------------------------------------------------------
REM EJECT

 FUNCTION WRITE.INVOK PUBLIC                                           ! BSPK
\***************************

    STRING    FORMAT$                                                  ! BSPK
    INTEGER*2 WRITE.INVOK                                              ! BSPK

    WRITE.INVOK = 1                                                     ! BSPK
    FORMAT$ = "C5,C3,C1,C4,C5,C1,C5,C1,C5,C1,C1,C5,C6"                  ! BSPK
    FORMAT$ = FORMAT$ +",C1,C1,C1,C1,C3,C1,C1,C3,C1,C1,C3,"   \       !EMJB
                      +"C5,C1,C1,C1,C1,C1,C3,C1,C6"                     !GNS

    IF END #INVOK.SESS.NUM% THEN ERROR.WRITE.INVOK                      ! BSPK

    WRITE FORM FORMAT$;                                                 \ BSPK
                 #INVOK.SESS.NUM%                                       \ BSPK
                 ,1;                                                    \ BSPK
                 INVOK.SERIAL.NO$,                                      \ BSPK
                 INVOK.DATE$,                                           \ BSPK
                 INVOK.SUCCESS.FLAG$,                                   \ BSPK
                 INVOK.STORE.NO$,                                       \ BSPK
                 INVOK.INVENTORY.SRLNO$,                                \ BSPK
                 INVOK.INVENTORY.SUCCESS$,                              \ BSPK
                 INVOK.SALES.SRLNO$,                                    \ BSPK
                 INVOK.SALES.SUCCESS$,                                  \ BSPK
                 INVOK.NEW.LIST.SRLNO$,                                 \ BSPK
                 INVOK.NEW.LIST.SUCCESS$,                               \ BSPK
                 INVOK.CSR.IDENT$,                                      \ BSPK
                 INVOK.CSR.DELIVERY.NO$,                                \ BSPK
                 INVOK.CSR.DELIVERY.DATE$,                              \ BSPK
                 INVOK.CSR.PSC11.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC13.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.DAYS$,                                 \ BSPK
                 INVOK.PSS33.RUN.DATE$,                \              !EMJB
                 INVOK.PSS33.SUCCESS.FLAG$,            \              !EMJB
                 INVOK.DIR.IMPL.FLAG$,                 \              !EMJB    
                 INVOK.PSC30.RUN.DATE$,                                 \ BSPK
                 INVOK.PSC30.SUCCESS.FLAG$,                             \ BSPK
                 INVOK.UOD.IMPL.FLAG$,                 \                  CLC
                 INVOK.LAST.UOD.DATE$,                 \                  CLC
                 INVOK.PREV.SERIAL.NO$,                \                  CLC
                 INVOK.PREV.SUCCESS.FLAG$,             \                  CLC
                 INVOK.SUPPRESS.EXCEP.REPORT$,         \                  DDS
                 INVOK.CSR.STARTED.BY.SUP$,            \                  DDS       
                 INVOK.CSR.PSC14.FLAG$,                \              !EMJB
                 INVOK.CSR.CONVERSION.STATUS.FLAG$,    \              !EMJB
                 INVOK.PTS.EVENTS.OK.DATE$,            \               FAW
                 INVOK.RP.DAYS$,                       \               GNS
                 INVOK.FILLER$                                         
  
       WRITE.INVOK = 0

       EXIT FUNCTION
    
ERROR.WRITE.INVOK:                                                      ! BSPK
    
       FILE.OPERATION$     = "W"                                        ! BSPK
       CURRENT.REPORT.NUM% = INVOK.REPORT.NUM%                          ! BSPK
       CURRENT.CODE$       = PACK$("0000000000000000")                  ! BSPK
       
       EXIT FUNCTION                                                    ! BSPK
    
  END FUNCTION                                                          ! BSPK
\----------------------------------------------------------------------
REM EJECT

 FUNCTION WRITE.INVOK.UNLOCK PUBLIC                                    ! BSPK
\**********************************

    STRING    FORMAT$                                                  ! BSPK
    INTEGER*2 WRITE.INVOK.UNLOCK                                       ! BSPK

    WRITE.INVOK.UNLOCK = 1                                              ! BSPK
    FORMAT$ = "C5,C3,C1,C4,C5,C1,C5,C1,C5,C1,C1,C5,C6"                  ! BSPK
    FORMAT$ = FORMAT$ +",C1,C1,C1,C1,C3,C1,C1,C3,C1,C1,C3,"   \       !EMJB
                      +"C5,C1,C1,C1,C1,C1,C3,C1,C6"                     ! GNS
    
    IF END #INVOK.SESS.NUM% THEN ERROR.WRITE.INVOK.UNLOCK               ! BSPK

    WRITE FORM FORMAT$;                                                 \ BSPK
                 #INVOK.SESS.NUM%                                       \ BSPK
                 AUTOUNLOCK,1;                                          \ BSPK 
                 INVOK.SERIAL.NO$,                                      \ BSPK
                 INVOK.DATE$,                                           \ BSPK
                 INVOK.SUCCESS.FLAG$,                                   \ BSPK
                 INVOK.STORE.NO$,                                       \ BSPK
                 INVOK.INVENTORY.SRLNO$,                                \ BSPK
                 INVOK.INVENTORY.SUCCESS$,                              \ BSPK
                 INVOK.SALES.SRLNO$,                                    \ BSPK
                 INVOK.SALES.SUCCESS$,                                  \ BSPK
                 INVOK.NEW.LIST.SRLNO$,                                 \ BSPK
                 INVOK.NEW.LIST.SUCCESS$,                               \ BSPK
                 INVOK.CSR.IDENT$,                                      \ BSPK
                 INVOK.CSR.DELIVERY.NO$,                                \ BSPK
                 INVOK.CSR.DELIVERY.DATE$,                              \ BSPK
                 INVOK.CSR.PSC11.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC13.FLAG$,                                 \ BSPK
                 INVOK.CSR.PSC12.DAYS$,                                 \ BSPK
                 INVOK.PSS33.RUN.DATE$,                \              !EMJB
                 INVOK.PSS33.SUCCESS.FLAG$,            \              !EMJB
                 INVOK.DIR.IMPL.FLAG$,                 \              !EMJB    
                 INVOK.PSC30.RUN.DATE$,                                 \ BSPK
                 INVOK.PSC30.SUCCESS.FLAG$,                             \ BSPK
                 INVOK.UOD.IMPL.FLAG$,                 \                  CLC
                 INVOK.LAST.UOD.DATE$,                 \                  CLC
                 INVOK.PREV.SERIAL.NO$,                \                  CLC
                 INVOK.PREV.SUCCESS.FLAG$,             \                  CLC
                 INVOK.SUPPRESS.EXCEP.REPORT$,         \                  DDS
                 INVOK.CSR.STARTED.BY.SUP$,            \                  DDS       
                 INVOK.CSR.PSC14.FLAG$,                \              !EMJB
                 INVOK.CSR.CONVERSION.STATUS.FLAG$,    \              !EMJB
                 INVOK.PTS.EVENTS.OK.DATE$,            \               FAW
                 INVOK.RP.DAYS$,                       \               GNS
                 INVOK.FILLER$                                         
 
       WRITE.INVOK.UNLOCK = 0
    
       EXIT FUNCTION
    
ERROR.WRITE.INVOK.UNLOCK:                                               ! BSPK
    
       FILE.OPERATION$     = "W"                                        ! BSPK
       CURRENT.REPORT.NUM% = INVOK.REPORT.NUM%                          ! BSPK
       CURRENT.CODE$       = PACK$("0000000000000000")                  ! BSPK
       
       EXIT FUNCTION                                                    ! BSPK
    
  END FUNCTION                                                          ! BSPK
