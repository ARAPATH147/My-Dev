
\******************************************************************************\******************************************************************************
\******************************************************************************
\***
\***              RF COUNT LISTS FILE FUNCTIONS
\***
\***               REFERENCE    : CLILFFUN.BAS
\***
\***         VERSION A            Nik Sen                  13th October 1998
\***
\***         VERSION B            Charles Skadorwa         22nd January 1999
\***                      Head Office Sequence Number now held (from PIITM
\***                      record) in order that a "complete" Type 13 transaction
\***                      record is written to the STKMQ file. 
\***
\***         VERSION C            Charles Skadorwa         22nd January 1999
\***                      Filler on WRITE not decreased correctly.
\***
\***    REVISION 1.4.            ROBERT COWEY.                  09 SEP 2003.
\***    Changes for RF trial.
\***    Removed redundant PVCS revision control block from top of code.
\***    Recompiled to prevent future automatic recompiles.
\***    No changes to actual code.
\***
\***    REVISION 1.5             ALAN CARR                      12 AUG 2004.
\***    Changes for RF OSSR solution.
\***    Added new fields for Off-Site StockRoom (OSSR) Basic Solution
\***
\***    REVISION 1.6             Mark Goode                     25th January 2005.
\***    Changes due to OSSR WAN
\***
\***    REVISION 1.7             Syam Jayan                     25th January 2012.
\***    The change is to rearrange/remove redundant fields and to add new  
\***    fields in CLILF Format as part of Stock file accuracy project.Few 
\***    variables are commented and few have been converted to array variables.
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$,              \ 1.7 SJ
       CLILF.ARRAY$                  ! 1.7 SJ
    
    %INCLUDE CLILFDEC.J86 
    %INCLUDE BTCMEM.J86               ! 1.7 SJ

  FUNCTION CLILF.SET PUBLIC
\***************************

    CLILF.REPORT.NUM% = 557
    CLILF.RECL%       = 492           ! 1.7 SJ
    CLILF.FILE.NAME$  = "CLILF"
    DIM CLILF.MODULE.ID%(32)          ! 1.7 SJ
    DIM CLILF.MODULE.SEQ%(32)         ! 1.7 SJ
    DIM CLILF.REPEAT.CNT%(32)         ! 1.7 SJ
    DIM CLILF.COUNT%(32)              ! 1.7 SJ
    DIM CLILF.FILL.QUANTITY%(32)      ! 1.7 SJ
    DIM CLILF.FILLER$(32)             ! 1.7 SJ
    
  END FUNCTION
    
\-----------------------------------------------------------------------------   

  SUB CLILF.SPLIT.ARRAY                                               ! 1.7 SJ
\***************************                                          ! 1.7 SJ

! This subprogram will split CLILF.ARRAY into CLILF.MODULE.ID%,       ! 1.7 SJ
! CLILF.MODULE.SEQ%, CLILF.REPEAT.CNT%, CLILF.COUNT%,                 ! 1.7 SJ
! CLILF.FILL.QUANTITY%, CLILF.FILLER$                                 ! 1.7 SJ
    
    INTEGER*2 i%                                                      ! 1.7 SJ

    FOR i% = 0 TO 31                                                  ! 1.7 SJ

        CLILF.MODULE.ID%(i%) = GETN4(CLILF.ARRAY$, i%*14 + 0)         ! 1.7 SJ
        CLILF.MODULE.SEQ%(i%) = GETN1(CLILF.ARRAY$, i%*14 + 4)        ! 1.7 SJ
        CLILF.REPEAT.CNT%(i%) = GETN1(CLILF.ARRAY$, i%*14 + 5)        ! 1.7 SJ
        CLILF.COUNT%(i%) = GETN2(CLILF.ARRAY$, i%*14 + 6)             ! 1.7 SJ
        CLILF.FILL.QUANTITY%(i%) = GETN2(CLILF.ARRAY$, i%*14 + 8)     ! 1.7 SJ
        CLILF.FILLER$(i%) = MID$(CLILF.ARRAY$, i%*14 + 11, 4)         ! 1.7 SJ

    NEXT i%                                                           ! 1.7 SJ
    
  END SUB                                                             ! 1.7 SJ

\-----------------------------------------------------------------------------   
  
  SUB CLILF.CONCAT.ARRAY                                              ! 1.7 SJ
\***************************                                          ! 1.7 SJ

! This subprogram will concatenate CLILF.MODULE.ID%,                  ! 1.7 SJ
! CLILF.MODULE.SEQ%, CLILF.REPEAT.CNT%, CLILF.COUNT%,                 ! 1.7 SJ
! CLILF.FILL.QUANTITY%, CLILF.FILLER$ into CLILF.ARRAY$               ! 1.7 SJ

    INTEGER*2 i%                                                      ! 1.7 SJ
    
    CLILF.ARRAY$ = STRING$(14, CHR$(00))                              ! 1.7 SJ
    CLILF.ARRAY$ = STRING$(32, CLILF.ARRAY$)                          ! 1.7 SJ
    
    FOR i% = 0 TO 31                                                  ! 1.7 SJ

        CALL PUTN4 (CLILF.ARRAY$, i%*14 + 0, CLILF.MODULE.ID%(i%) )   ! 1.7 SJ
        CALL PUTN1 (CLILF.ARRAY$, i%*14 + 4, CLILF.MODULE.SEQ%(i%))   ! 1.7 SJ
        CALL PUTN1 (CLILF.ARRAY$, i%*14 + 5, CLILF.REPEAT.CNT%(i%))   ! 1.7 SJ
        CALL PUTN2 (CLILF.ARRAY$, i%*14 + 6, CLILF.COUNT%(i%))        ! 1.7 SJ
        CALL PUTN2 (CLILF.ARRAY$, i%*14 + 8, CLILF.FILL.QUANTITY%(i%))! 1.7 SJ
        CALL PUTN4 (CLILF.ARRAY$, i%*14 + 10, ASC(CLILF.FILLER$(i%))) ! 1.7 SJ
    
    NEXT i%

  END SUB

\-----------------------------------------------------------------------------   
  
FUNCTION READ.CLILF PUBLIC
\****************************

    INTEGER*2 READ.CLILF
    
    READ.CLILF = 1
                 
    CLILF.KEY$ = CLILF.LISTID$ + CLILF.ITEMSEQ$

    IF END #CLILF.SESS.NUM% THEN READ.CLILF.ERROR
    READ FORM "T7,C4,C2,C1,C3,6I2,C16,C448"; #CLILF.SESS.NUM% \ 1.5 AC \ BCS \ 1.6 MG \ 1.7 SJ
       KEY CLILF.KEY$;                                                 \
       CLILF.BOOTSCODE$,                                               \
       CLILF.HO.SEQNO$,                                                \ BCS    \ 1.7 SJ
       CLILF.COUNTED.STATUS$,                                          \ 1.7 SJ
       CLILF.DATE.LASTCNT$,                                            \ 1.7 SJ
       CLILF.SALESCNT%,                                                \ 1.7 SJ
       CLILF.BSCNT%,                                                   \ 1.7 SJ
       CLILF.OSSR.ITMSTKCNT%,                                          \ 1.7 SJ
       CLILF.BS.PEND.SA.CNT%,                                          \ 1.7 SJ
       CLILF.OSSR.PEND.SA.CNT%,                                        \ 1.7 SJ 
       CLILF.SFCNT%,                                                   \ 1.7 SJ
       CLILF.SPACE$,                                                   \ 1.7 SJ
       CLILF.ARRAY$                                                    ! 1.7 SJ
       !CLILF.BARCODE$,                                                \ 1.7 SJ
       !CLILF.SELDESC$,                                                \ 1.7 SJ
       !CLILF.DEALMKR$,                                                \ 1.7 SJ
       !CLILF.PRODGRP$,                                                \ 1.7 SJ
       !CLILF.PRODGRPDESC$,                                            \ 1.7 SJ
       !CLILF.SALESSFCNT%,                                             \ 1.7 SJ
       !CLILF.BSCNT$,                                                  \ 1.7 SJ
       !CLILF.SFCNT$,                                                  \ 1.7 SJ
       !CLILF.OSSR.ITMSTKCNT$,                                         \ 1.5 AC \ 1.7 SJ
       !CLILF.SALESSFCNT$,                                             \ 1.7 SJ
       !CLILF.SALEBSCNT$,                                              \ 1.5 AC \ 1.7 SJ
       !CLILF.SALEOSSRCNT$,                                            \ 1.5 AC \ 1.7 SJ
      
    CALL CLILF.SPLIT.ARRAY

    READ.CLILF = 0
    EXIT FUNCTION
    
READ.CLILF.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = CLILF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  
FUNCTION WRITE.CLILF PUBLIC
\****************************

    INTEGER*2 WRITE.CLILF
    
    WRITE.CLILF = 1
                 
    CLILF.KEY$ = CLILF.LISTID$ + CLILF.ITEMSEQ$
    CALL CLILF.CONCAT.ARRAY
    
    IF END #CLILF.SESS.NUM% THEN WRITE.CLILF.ERROR
    WRITE FORM "C6,C4,C2,C1,C3,6I2,C16,C448"; #CLILF.SESS.NUM% ;   \ 1.5 AC \ CCS 1.6 MG \ 1.7 SJ 
       CLILF.KEY$,                                                     \
       CLILF.BOOTSCODE$,                                               \
       CLILF.HO.SEQNO$,                                                \ BCS    \ 1.7 SJ
       CLILF.COUNTED.STATUS$,                                          \ 1.7 SJ
       CLILF.DATE.LASTCNT$,                                            \ 1.7 SJ
       CLILF.SALESCNT%,                                                \ 1.7 SJ
       CLILF.BSCNT%,                                                   \ 1.7 SJ
       CLILF.OSSR.ITMSTKCNT%,                                          \ 1.7 SJ
       CLILF.BS.PEND.SA.CNT%,                                          \ 1.7 SJ
       CLILF.OSSR.PEND.SA.CNT%,                                        \ 1.7 SJ 
       CLILF.SFCNT%,                                                   \ 1.7 SJ
       CLILF.SPACE$,                                                   \ 1.7 SJ
       CLILF.ARRAY$                                                    ! 1.7 SJ
       !CLILF.BARCODE$,                                                \ 1.7 SJ
       !CLILF.SELDESC$,                                                \ 1.7 SJ
       !CLILF.DEALMKR$,                                                \ 1.7 SJ
       !CLILF.PRODGRP$,                                                \ 1.7 SJ
       !CLILF.PRODGRPDESC$,                                            \ 1.7 SJ
       !CLILF.SALESSFCNT%,                                             \ 1.7 SJ
       !CLILF.BSCNT$,                                                  \ 1.7 SJ
       !CLILF.SFCNT$,                                                  \ 1.7 SJ
       !CLILF.OSSR.ITMSTKCNT$,                                         \ 1.5 AC \ 1.7 SJ
       !CLILF.SALESSFCNT$,                                             \ 1.7 SJ
       !CLILF.SALEBSCNT$,                                              \ 1.5 AC \ 1.7 SJ
       !CLILF.SALEOSSRCNT$,                                            \ 1.5 AC \ 1.7 SJ
      
    WRITE.CLILF = 0
    EXIT FUNCTION
    
WRITE.CLILF.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = CLILF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION



