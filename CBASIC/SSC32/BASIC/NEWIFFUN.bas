REM \
\*******************************************************************************
\*******************************************************************************
\***
\***          %INCLUDE FOR NEW INVOICES FILE PUBLIC FUNCTIONS
\***
\***                REFERENCE    : NEWIFFUN.J86
\***
\***   VERSION A          MICHAEL KELSALL                  12th October 1992
\***   Derived from NEWIFFNC.J86
\***
\***   VERSION B          Neil Bennett                     12th December 2006
\***   Added support for record type 'C' - Carton records
\***
\***   VERSION C          Neil Bennett                          30th May 2007
\***   Added support for record type 'R' - Add/Update Recall Record
\***                                 'V' - Remove Items from Recall Rcd (or all)
\***                                 'W' - Unblock Recall Items for sale
\***
\***   VERSION D          Charles Skadorwa                      21st Aug 2007
\***   Defect 523: Processing fails if "W" unblocking records exist. Mainframe
\***               spec incorrect as it includes recall no., however, it was
\***               decided to fix the controller end.
\***
\***   VERSION E          Charles Skadorwa                      30th Aug 2007
\***   Defect 523: Processing fails if "W" unblocking records exist.
\***               DIM statement added.
\***
\***   VERSION F          Stuart Highley                        18 July 2008
\***   Added new record types P, E, F, and G, for UOD booking in.
\***
\***   REVISION 1.8.                ROBERT COWEY.                17 DEC 2009.
\***   Incorprated SSC3200.BAS functions associated with truncated NEWIF.
\***
\***   VERSION G               Sumitha Moorthy                14/04/2015
\***   FOD - 431 Dallas Positive Receiving
\***   Changes as part of NEWIF file layout change, a new variable is
\***   added into the NEWIF.ITEM.DETAILS$ array.
\***
\*******************************************************************************
\*******************************************************************************

  %INCLUDE NEWIFDEC.J86

  STRING GLOBAL                        \
       FILE.OPERATION$,                \
       CURRENT.CODE$

  INTEGER*2 GLOBAL                     \
       CURRENT.REPORT.NUM%


  FUNCTION NEWIF.SET PUBLIC

    INTEGER*2       NEWIF.SET
      NEWIF.SET = 1

      NEWIF.REPORT.NUM%  = 88
      NEWIF.FILE.NAME$ = "NEWIF"

      NEWIF.SET = 0

  END FUNCTION



  FUNCTION READ.NEWIF PUBLIC

    INTEGER*2       READ.NEWIF
      READ.NEWIF  =  1

    IF END # NEWIF.SESS.NUM% THEN ERROR.READ.NEWIF
    READ #NEWIF.SESS.NUM%; LINE NEWIF.RECORD$

    NEWIF.REC.TYPE$ = MID$(NEWIF.RECORD$,2,1)

    IF NEWIF.REC.TYPE$ = "H" THEN BEGIN
       ! Hdr rec
       NEWIF.STORE.NO$        = MID$(NEWIF.RECORD$, 3, 4)
       NEWIF.SERIAL.NO$       = MID$(NEWIF.RECORD$, 7, 5)
       NEWIF.DATE$            = MID$(NEWIF.RECORD$,12, 6)
       NEWIF.UOD.BATCH.SIZE$  = MID$(NEWIF.RECORD$,18, 3)           ! FSH
       NEWIF.UOD.NIGHT.DELIV$ = MID$(NEWIF.RECORD$,21, 1)           ! FSH
       NEWIF.UOD.NIGHT.SCAN$  = MID$(NEWIF.RECORD$,22, 1)           ! FSH
       NEWIF.HDR.FILLER$      = MID$(NEWIF.RECORD$,23, 5)           ! FSH
       NEWIF.ENDREC.MARKER$   = RIGHT$(NEWIF.RECORD$, 3)
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "T" THEN BEGIN
       ! Trl rec
       NEWIF.REC.COUNT$     = MID$(NEWIF.RECORD$, 3, 5)
       NEWIF.TRL.FILLER$    = MID$(NEWIF.RECORD$, 8,10)
       NEWIF.ENDREC.MARKER$ = RIGHT$(NEWIF.RECORD$, 3)
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "D" THEN BEGIN
        ! Detail rec
        NEWIF.FSI$             = MID$(NEWIF.RECORD$, 3, 1)
        NEWIF.FOLIO.MONTH$     = MID$(NEWIF.RECORD$, 4, 2)
        NEWIF.FOLIO.NO$        = MID$(NEWIF.RECORD$, 6, 5)
        NEWIF.STORE.SUF$       = MID$(NEWIF.RECORD$,11, 1)
        NEWIF.FOLIO.YEAR$      = MID$(NEWIF.RECORD$,12, 2)
        NEWIF.INVCE.DATE$      = MID$(NEWIF.RECORD$,14, 6)
        NEWIF.WHOUSE.AREA$     = MID$(NEWIF.RECORD$,20, 3)
        NEWIF.INSYST.FLAG$     = MID$(NEWIF.RECORD$,23, 1)
        NEWIF.ITEM.COUNT$      = MID$(NEWIF.RECORD$,24, 2)
        NEWIF.INVCE.TYPE$      = MID$(NEWIF.RECORD$,26, 1)
        NEWIF.DALLAS.MKR$      = MID$(NEWIF.RECORD$,27, 1)
        NEWIF.EXP.DEL.DATE$    = MID$(NEWIF.RECORD$,28, 6)
        NEWIF.SUPPLIER.NO.PKD$ = MID$(NEWIF.RECORD$,34, 3)
        NEWIF.ORDER.SUF$       = MID$(NEWIF.RECORD$,37, 1)
        NEWIF.DET.FILLER.1$    = MID$(NEWIF.RECORD$,38, 3)
        DIM NEWIF.ITEM.DETAILS$(VAL(NEWIF.ITEM.COUNT$), 6)              !GSM
        NEWIF.INDEX% = 41
        FOR NEWIF.CNT% = 1 TO VAL(NEWIF.ITEM.COUNT$)
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 1) =                      \
                          MID$(NEWIF.RECORD$, NEWIF.INDEX%, 7)
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 2) =                      \
                          MID$(NEWIF.RECORD$, NEWIF.INDEX% +  7, 7)
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 3) =                      \
                          MID$(NEWIF.RECORD$, NEWIF.INDEX% + 14, 4)
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 4) =                      \
                          MID$(NEWIF.RECORD$, NEWIF.INDEX% + 18, 1)
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 5) =                      \
                          MID$(NEWIF.RECORD$, NEWIF.INDEX% + 19, 6)     !GSM
            NEWIF.ITEM.DETAILS$(NEWIF.CNT%, 6) =                      \ !GSM
                          MID$(NEWIF.RECORD$, NEWIF.INDEX% + 25, 2)     !GSM
            NEWIF.INDEX% = NEWIF.INDEX% + 27
        NEXT NEWIF.CNT%
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "S" THEN BEGIN
       ! supplier reference record
       NEWIF.BUS.CENTRE$       = MID$(NEWIF.RECORD$, 3, 1)
       NEWIF.SUPPLIER.NO$      = MID$(NEWIF.RECORD$, 4, 7)
       NEWIF.SUPPLIER.NAME$    = MID$(NEWIF.RECORD$,11,10)
       NEWIF.LEAD.TIME.MON$    = MID$(NEWIF.RECORD$,21, 3)
       NEWIF.LEAD.TIME.TUE$    = MID$(NEWIF.RECORD$,24, 3)
       NEWIF.LEAD.TIME.WED$    = MID$(NEWIF.RECORD$,27, 3)
       NEWIF.LEAD.TIME.THU$    = MID$(NEWIF.RECORD$,30, 3)
       NEWIF.LEAD.TIME.FRI$    = MID$(NEWIF.RECORD$,33, 3)
       NEWIF.LAPSING.DAYS$     = MID$(NEWIF.RECORD$,36, 3)
       NEWIF.PART.ORDER.RULES$ = MID$(NEWIF.RECORD$,39, 1)
       NEWIF.MAX.CHECK.QTY$    = MID$(NEWIF.RECORD$,40, 4)
       NEWIF.CHECK.QTY$        = MID$(NEWIF.RECORD$,44, 4)
       NEWIF.DISC.QTY$         = MID$(NEWIF.RECORD$,48, 4)
       NEWIF.DISC.PERC$        = MID$(NEWIF.RECORD$,52, 4)
       NEWIF.SUPREF.FILLER$    = MID$(NEWIF.RECORD$,56, 6)
       NEWIF.ENDREC.MARKER$    = RIGHT$(NEWIF.RECORD$, 3)
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "O" THEN BEGIN
       ! direct order record
       NEWIF.BUS.CENTRE$         = MID$(NEWIF.RECORD$, 3, 1)
       NEWIF.SUPPLIER.NO$        = MID$(NEWIF.RECORD$, 4, 7)
       NEWIF.ORDER.NO$           = MID$(NEWIF.RECORD$,11, 5)
       NEWIF.ORDER.SUFFIX$       = MID$(NEWIF.RECORD$,16, 1)
       NEWIF.PAGE.NO$            = MID$(NEWIF.RECORD$,17, 3)
       NEWIF.ORDER.DATE$         = MID$(NEWIF.RECORD$,20, 6)
       NEWIF.ACTION.MKR$         = MID$(NEWIF.RECORD$,26, 1)
       NEWIF.ALLOC.ON.SALE.DATE$ = MID$(NEWIF.RECORD$,27, 6)
       NEWIF.ORDER.ITEM.CNT$     = MID$(NEWIF.RECORD$,33, 3)
       NEWIF.ORDER.FILLER1$      = MID$(NEWIF.RECORD$,36, 5)
       DIM NEWIF.ORDER.ITEM.DETAILS$(VAL(NEWIF.ORDER.ITEM.CNT$), 5)
       NEWIF.INDEX% = 41
       FOR NEWIF.CNT% = 1 TO VAL(NEWIF.ORDER.ITEM.CNT$)
          NEWIF.ORDER.ITEM.DETAILS$(NEWIF.CNT%, 1) =                \
                 MID$(NEWIF.RECORD$, NEWIF.INDEX%     , 7)
          NEWIF.ORDER.ITEM.DETAILS$(NEWIF.CNT%, 2) =                \
                 MID$(NEWIF.RECORD$, NEWIF.INDEX% +  7, 7)
          NEWIF.ORDER.ITEM.DETAILS$(NEWIF.CNT%, 3) =                \
                 MID$(NEWIF.RECORD$, NEWIF.INDEX% + 14, 4)
          NEWIF.ORDER.ITEM.DETAILS$(NEWIF.CNT%, 4) =                \
                 MID$(NEWIF.RECORD$, NEWIF.INDEX% + 18, 1)
          NEWIF.ORDER.ITEM.DETAILS$(NEWIF.CNT%, 5) =                \
                 MID$(NEWIF.RECORD$, NEWIF.INDEX% + 19, 8)
          NEWIF.INDEX% = NEWIF.INDEX% + 27
       NEXT NEWIF.CNT%
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "P" THEN BEGIN                  ! FSH
        !UOD Parent                                                 ! FSH
        NEWIF.STORE.SFX$      = MID$(NEWIF.RECORD$, 3, 1)           ! FSH
        NEWIF.LICENCE$        = MID$(NEWIF.RECORD$, 4, 10)          ! FSH
        NEWIF.DESPATCH.DATE$  = MID$(NEWIF.RECORD$, 14, 6)          ! FSH
        NEWIF.EXP.DELIV.DATE$ = MID$(NEWIF.RECORD$, 20, 6)          ! FSH
        NEWIF.UOD.TYPE$       = MID$(NEWIF.RECORD$, 26, 1)          ! FSH
        NEWIF.UOD.CATEGORY$   = MID$(NEWIF.RECORD$, 27, 1)          ! FSH
        NEWIF.UOD.REASON$     = MID$(NEWIF.RECORD$, 28, 1)          ! FSH
        NEWIF.UOD.ACTION$     = MID$(NEWIF.RECORD$, 29, 1)          ! FSH
        NEWIF.UOD.WHAREA$     = MID$(NEWIF.RECORD$, 30, 3)          ! FSH
        NEWIF.NUM.CHILDREN$   = MID$(NEWIF.RECORD$, 33, 3)          ! FSH
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "E" THEN BEGIN                  ! FSH
        !UOD Parent/child                                           ! FSH
        NEWIF.HIER.LVL$       = MID$(NEWIF.RECORD$, 3, 1)           ! FSH
        NEWIF.LICENCE$        = MID$(NEWIF.RECORD$, 4, 10)          ! FSH
        NEWIF.PARENT.LICENCE$ = MID$(NEWIF.RECORD$, 14, 10)         ! FSH
        NEWIF.UOD.TYPE$       = MID$(NEWIF.RECORD$, 24, 1)          ! FSH
        NEWIF.NUM.CHILDREN$   = MID$(NEWIF.RECORD$, 25, 3)          ! FSH
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "F" THEN BEGIN                  ! FSH
        !UOD child                                                  ! FSH
        NEWIF.HIER.LVL$       = MID$(NEWIF.RECORD$, 3, 1)           ! FSH
        NEWIF.LICENCE$        = MID$(NEWIF.RECORD$, 4, 10)          ! FSH
        NEWIF.PARENT.LICENCE$ = MID$(NEWIF.RECORD$, 14, 10)         ! FSH
        NEWIF.UOD.TYPE$       = MID$(NEWIF.RECORD$, 24, 1)          ! FSH
        NEWIF.NUM.ITEMS$      = MID$(NEWIF.RECORD$, 25, 3)          ! FSH
        NEWIF.BOL.FLAG$       = MID$(NEWIF.RECORD$, 28, 1)          ! FSH
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "G" THEN BEGIN                  ! FSH
        !UOD child                                                  ! FSH
        NEWIF.ITEM.CODE$ = MID$(NEWIF.RECORD$, 3, 7)                ! FSH
        NEWIF.QTY$       = MID$(NEWIF.RECORD$, 10, 5)               ! FSH
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "U"                             \ BLC
               OR NEWIF.REC.TYPE$ = "Q" THEN BEGIN                  ! BLC
       ! UOD Master                                                 ! BLC
       NEWIF.DIST.CENTRE$    = MID$(NEWIF.RECORD$, 3, 2)            ! BLC
       NEWIF.WAREHOUSE.NUM$  = MID$(NEWIF.RECORD$, 5, 2)            ! BLC
       NEWIF.UOD.NUMBER$     = MID$(NEWIF.RECORD$, 7, 6)            ! BLC
       NEWIF.EXP.DELIV.DATE$ = MID$(NEWIF.RECORD$,13, 6)            ! BLC
       NEWIF.UOD.TYPE$       = MID$(NEWIF.RECORD$,19, 1)            ! BLC
       NEWIF.NUM.ITEM$       = MID$(NEWIF.RECORD$,20, 4)            ! BLC
       NEWIF.UOD.MAS.FILLER$ = MID$(NEWIF.RECORD$,24, 4)            ! BLC
       NEWIF.ENDREC.MARKER$  = RIGHT$(NEWIF.RECORD$, 3)             ! BLC
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "I"                             \ BLC
               OR NEWIF.REC.TYPE$ = "A" THEN BEGIN                  ! BLC
       ! UOD item                                                   ! BLC
       NEWIF.STORE.SUFFIX$    = MID$(NEWIF.RECORD$, 3, 1)           ! BLC
       NEWIF.DIST.CENTRE$     = MID$(NEWIF.RECORD$, 4, 2)           ! BLC
       NEWIF.WAREHOUSE.NUM$   = MID$(NEWIF.RECORD$, 6, 2)           ! BLC
       NEWIF.UOD.NUMBER$      = MID$(NEWIF.RECORD$, 8, 6)           ! BLC
       NEWIF.OCCUR.NUMBER$    = MID$(NEWIF.RECORD$,14, 4)           ! BLC
       NEWIF.FSI$             = MID$(NEWIF.RECORD$,18, 1)           ! BLC
       NEWIF.FOLIO.YEAR$      = MID$(NEWIF.RECORD$,19, 2)           ! BLC
       NEWIF.FOLIO.MONTH$     = MID$(NEWIF.RECORD$,21, 2)           ! BLC
       NEWIF.FOLIO.NUM$       = MID$(NEWIF.RECORD$,23, 5)           ! BLC
       NEWIF.ITEM.CODE$       = MID$(NEWIF.RECORD$,28, 7)           ! BLC
       NEWIF.QUANTITY$        = MID$(NEWIF.RECORD$,35, 4)           ! BLC
       NEWIF.CSR.MKR$         = MID$(NEWIF.RECORD$,39, 1)           ! BLC
       NEWIF.UOD.ITEM.FILLER$ = MID$(NEWIF.RECORD$,40, 3)           ! BLC
       NEWIF.ENDREC.MARKER$   = RIGHT$(NEWIF.RECORD$, 3)            ! BLC
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "C" THEN BEGIN                  !BNWB
       ! Carton Record                                              !BNWB
       NEWIF.CARTON.NO$       = MID$(NEWIF.RECORD$, 3, 8)           !BNWB
       NEWIF.SUPPLIER.NO$     = MID$(NEWIF.RECORD$,11, 7)           !BNWB
       NEWIF.ASN.CODE$        = MID$(NEWIF.RECORD$,18,18)           !BNWB
       NEWIF.ORDER.NO$        = MID$(NEWIF.RECORD$,36, 5)           !BNWB
       NEWIF.ORDER.SUFFIX$    = MID$(NEWIF.RECORD$,41, 1)           !BNWB
       NEWIF.BUS.CENTRE$      = MID$(NEWIF.RECORD$,42, 1)           !BNWB
       NEWIF.DEL.DTTM$        = MID$(NEWIF.RECORD$,43,12)           !BNWB
       NEWIF.CRTN.IN.ASN$     = MID$(NEWIF.RECORD$,55, 3)           !BNWB
       NEWIF.ITMS.IN.CRTN$    = MID$(NEWIF.RECORD$,58, 3)           !BNWB
       DIM NEWIF.ITM.CODE$(VAL(NEWIF.ITMS.IN.CRTN$))                !BNWB
       DIM NEWIF.ITEM.QTY$(VAL(NEWIF.ITMS.IN.CRTN$))                !BNWB
       NEWIF.INDEX% = 61                                            !BNWB
       FOR NEWIF.CNT% = 1 TO VAL(NEWIF.ITMS.IN.CRTN$)               !BNWB
          NEWIF.ITM.CODE$(NEWIF.CNT%)                               \BNWB
                          = MID$(NEWIF.RECORD$, NEWIF.INDEX%   , 7) !BNWB
          NEWIF.ITEM.QTY$(NEWIF.CNT%)                               \BNWB
                          = MID$(NEWIF.RECORD$, NEWIF.INDEX% +7, 4) !BNWB
          NEWIF.INDEX% = NEWIF.INDEX% +11                           !BNWB
       NEXT NEWIF.CNT%                                              !BNWB
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "R" THEN BEGIN                  !CNWB
       ! Add/Update Recall Record                                   !CNWB
       NEWIF.RCL.REF$         = MID$(NEWIF.RECORD$,  3, 8)          !CNWB
       NEWIF.RCL.DESC$        = MID$(NEWIF.RECORD$, 11,20)          !CNWB
       NEWIF.RCL.LABEL$       = MID$(NEWIF.RECORD$, 31, 2)          !CNWB
       NEWIF.RCL.SI.1$        = MID$(NEWIF.RECORD$, 33,77)          !CNWB
       NEWIF.RCL.SI.2$        = MID$(NEWIF.RECORD$,110,77)          !CNWB
       NEWIF.SUPP.ROUTE$      = MID$(NEWIF.RECORD$,187, 1)          !CNWB
       NEWIF.RCL.RC$          = MID$(NEWIF.RECORD$,188, 2)          !CNWB
       NEWIF.BUS.CENTRE$      = MID$(NEWIF.RECORD$,190, 1)          !CNWB
       NEWIF.FLAG.TYPE$       = MID$(NEWIF.RECORD$,191, 1)          !CNWB
       NEWIF.BATCH.NOS$       = MID$(NEWIF.RECORD$,192,30)          !CNWB
       NEWIF.NUM.ITEMS$       = MID$(NEWIF.RECORD$,222, 4)          !CNWB
       NEWIF.ACT.DATE$        = MID$(NEWIF.RECORD$,226, 8)          !CNWB
       NEWIF.EXP.DATE$        = MID$(NEWIF.RECORD$,234, 8)          !CNWB
       DIM NEWIF.ITM.CODE$(VAL(NEWIF.NUM.ITEMS$))                   !CNWB
       NEWIF.INDEX% = 242                                           !CNWB
       FOR NEWIF.CNT% = 1 TO VAL(NEWIF.NUM.ITEMS$)                  !CNWB
          NEWIF.ITM.CODE$(NEWIF.CNT%)                               \CNWB
                          = MID$(NEWIF.RECORD$, NEWIF.INDEX%   , 7) !CNWB
          NEWIF.INDEX% = NEWIF.INDEX% +7                            !CNWB
       NEXT NEWIF.CNT%                                              !CNWB
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "V" THEN BEGIN                  !CNWB
       ! Remove Items from Recall Record                            !CNWB
       NEWIF.RCL.REF$         = MID$(NEWIF.RECORD$,  3, 8)          !CNWB
       NEWIF.NUM.ITEMS$       = MID$(NEWIF.RECORD$, 11, 4)          !CNWB
       DIM NEWIF.ITM.CODE$(VAL(NEWIF.NUM.ITEMS$))                   !CNWB
       NEWIF.INDEX% = 15                                            !CNWB
       FOR NEWIF.CNT% = 1 TO VAL(NEWIF.NUM.ITEMS$)                  !CNWB
          NEWIF.ITM.CODE$(NEWIF.CNT%)                               \CNWB
                          = MID$(NEWIF.RECORD$, NEWIF.INDEX%   , 7) !CNWB
          NEWIF.INDEX% = NEWIF.INDEX% +7                            !CNWB
       NEXT NEWIF.CNT%                                              !CNWB
    ENDIF ELSE IF NEWIF.REC.TYPE$ = "W" THEN BEGIN                  !CNWB
       ! Unblock Recall Items for sale                              !CNWB
       ! Get no. of 7-digit item codes (subtract 11 bytes for       !DCSk
       ! quotes, record type & recall no.)                          !DCSk
       NEWIF.NUM.ITEMS$ = STR$((LEN(NEWIF.RECORD$) - 11) / 7)       !DCSk
       DIM NEWIF.ITM.CODE$(VAL(NEWIF.NUM.ITEMS$))                   !ECSk
       NEWIF.INDEX% = 11                                            !DCSk
       FOR NEWIF.CNT% = 1 TO VAL(NEWIF.NUM.ITEMS$)                  !CNWB
          NEWIF.ITM.CODE$(NEWIF.CNT%)                               \CNWB
                          = MID$(NEWIF.RECORD$, NEWIF.INDEX%   , 7) !CNWB
          NEWIF.INDEX% = NEWIF.INDEX% +7                            !CNWB
       NEXT NEWIF.CNT%                                              !CNWB
    ENDIF                                                           ! BLC

    READ.NEWIF = 0
    EXIT FUNCTION

ERROR.READ.NEWIF:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = NEWIF.REPORT.NUM%
    CURRENT.CODE$ = NEWIF.REC.TYPE$

    EXIT FUNCTION

END FUNCTION


\******************************************************************************
\***
\***    Internal file functions to process truncated NEWIF                                  ! PNWB
\***    READ.NEWIF.TRUNC
\***    WRITE.NEWIF.TRUNC
\***
\******************************************************************************

FUNCTION READ.NEWIF.TRUNC PUBLIC ! Entire function moved from SSC3200  ! 1.8 RC

    INTEGER*2 READ.NEWIF.TRUNC
    READ.NEWIF.TRUNC EQ 1

    IF END # NEWIF.TRUNC.SESS.NUM% THEN READ.NEWIF.TRUNC.IF.END
    READ FORM "C1"; \
      # NEWIF.TRUNC.SESS.NUM%, \
        NEWIF.TRUNC.REC.NUM%; \
          NEWIF.TRUNC.REC.TYPE$

    READ.NEWIF.TRUNC EQ 0
    EXIT FUNCTION

READ.NEWIF.TRUNC.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ  NEWIF.TRUNC.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(NEWIF.TRUNC.REC.NUM%),14))

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.NEWIF.TRUNC PUBLIC ! Entire function moved from SSC3200 ! 1.8 RC

    INTEGER*2 WRITE.NEWIF.TRUNC
    WRITE.NEWIF.TRUNC EQ 1

    IF END # NEWIF.TRUNC.SESS.NUM% THEN WRITE.NEWIF.TRUNC.IF.END
    WRITE FORM "C1"; \
      # NEWIF.TRUNC.SESS.NUM%, \
        NEWIF.TRUNC.REC.NUM%; \
          NEWIF.TRUNC.REC.TYPE$

    WRITE.NEWIF.TRUNC EQ 0
    EXIT FUNCTION

WRITE.NEWIF.TRUNC.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  NEWIF.TRUNC.REPORT.NUM%
    CURRENT.CODE$       EQ \
      PACK$(RIGHT$("00000000000000" + STR$(NEWIF.TRUNC.REC.NUM%),14))

    EXIT FUNCTION

END FUNCTION

