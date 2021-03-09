\*****************************************************************************
\***            Branch control file functions
\***      Version A           Steve Windsor               5.2.93
\***
\***      Version B           Michael J. Kelsall         14th Sept 1993
\***      Addition of CCMVT serial and sequence number as part of the
\***      RETURNS/AUTOMATIC CREDIT CLAIMING system.
\***
\***      Version C           Stuart William McConnachie 23rd Jan 1995
\***      Addition of TXR serial number as part of the TRANSACTION RETRIEVAL
\***      project.
\***
\***      Version D           Stuart William McConnachie  7th June 1995
\***      Converted calls to enable more than 1 record on the BCF.
\***      The second record is to be used for the No7 customer card trial.
\***
\***      Version E           Stuart William McConnachie  24th Jan 1997
\***      Added serial number on CUSTD file for advantage card national
\***      rollout.
\***      Added cut-off dates for CUSTD and MTSL.
\***
\***      Version F           Stuart Highley              7th April 1999
\***      Added serial number on ACSAL for new cash accounting.
\***
\***      Version G           Mark Goode                   4th May 2000
\***      New record added for use for Dentistry.
\***
\***      Version H           Mark Goode                   18th July 2000
\***      New record added for use for Well-being.
\***
\***      Version I           Amy Hoggard                  13th Oct 2000
\***      New record added for use for ECO.
\***
\***      Version J           Jamie Thorpe                 11th Jan 2001
\***      Amended BCF.DENTISTRY.PRODUCT.GROUP$ & BCF.WELL.PRODUCT.GROUP$ read
\***      and write lengths from 70 to 71
\***
\***      Version K           Brian Greenfield             1st May 2001
\***      Added BCF.WELL.SERV.PRODUCT.GROUP$ for use in new record 13 to
\***      contain any wellbeing services not covered in records 9 & 10.
\***
\***      Version L           Amy Hoggard                  4th Jan 2002
\***      Added new field in record 13 for ETOPUP product group.
\***
\***      Version M           Julia Stones                22nd July 2002
\***      Added BCF.DEALDIR.SERIAL.NUM$ and BCF.DIDIR.SERIAL.NUM$ for use in
\***      new record 14 to contain last serial numbers processed successfully
\***      on both files.  Added BCF.DEAL.NUM.REC$ and BCF.DINF.NUM.REC$
\***      to new record 14 these fields will contain the value of the number of
\***      records to be used when creating an empty keyed file.
\***      Added BCF.DEAL.KEY.LEN$ and BCF.DINF.KEY.LEN$ to new record 14
\***      these fields will contain the value of the key length of both files
\***      to be used when creating an empty keyed file.
\***
\***    Revision 4.6            ROBERT COWEY.            19 MAY 2003.
\***    Modifications for All Txn Data To CDAS project.
\***    Removed redundant variables ...
\***      BCF.CTSL1.SERIAL.NUM$, BCF.CTSL2,SERIAL.NUM$
\***    Defined new variables ...
\***      BCF.MTSLQ.DAYS$, BCF.FILLER67$
\***
\***    Revision 4.7            Julia Stones              9th July 2003
\***    Modifications for New Lines Report poject.
\***    Defined new variable
\***      BCF.NEWLINES.WEEKS$
\***    Amended existing variable was 73 bytes now 70 bytes
\***      BCF.TBAG.FILLER$
\***
\***    Revision 4.8            Julia Stones              3rd October 2003
\***    Modifications for New Lines Report poject.
\***    Defined new variable
\***      BCF.NEWLINES.LINES$
\***    Amended existing variable was 70 bytes now 65 bytes
\***      BCF.TBAG.FILLER$
\***
\***      Version N           Jamie Thorpe                27th June 2006
\***      Added BCF.DVCHR.SERIAL.NUM$ (Record 14)
\***
\***    Revision 4.10          Charles Skadorwa            4th July 2011
\***    CORE Heritage Stores Release 2 (Outbound) Project.
\***    Functionality added to all read and write functions for the new
\***    format record 14, and the new record 20.
\***
\***    Revision 4.11          Arun Sudhakaran             10th April 2013
\***    Defined new variables for getting Supplier Number lengths
\***    as part of Automatic Booking In of Chilled Food ASNs project
\***
\***    Revision 4.12          Charles Skadorwa            5th Sept 2013
\***    F261 Gift Card Mall IIN Range Extension Project - Commented !4.12 CSk
\***    Set new variables for GCM Product Group Number length and Record 22
\***    lengths.
\***
\***    Version O              Mark Walker                  3rd Feb 2014
\***    F337 Centralised View of Stock
\***    - Added functions for record 23 (stock snapshot parameters).
\***    - Minor formatting changes (uncommented).
\***
\***    Version P              Mark Walker                 12th Mar 2014
\***    F337 Centralised View of Stock
\***    The following changes were made following a coding standards
\***    review by Applications Management:
\***    - Added comment header block for record 23 processing.
\***
\***    Revision Q             Charles Skadorwa            25th June 2014
\***    F353 Deal Limits Increase Project.
\***    Renamed 3 variables in function BCF.RECORD.14.W -
\***        BCF.DINF.NUM.REC$ becomes BCF.ITMDL.NUM.REC$
\***        BCF.DINF.KEY.LEN$ becomes BCF.ITMDL.KEY.LEN$
\***        BCF.ECC.DATETIMESTAMP$ becomes BCF.FILLER.DATETIMESTAMP$
\***
\*******************************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
       CURRENT.CODE$,                                                   \
       FILE.OPERATION$

%INCLUDE BCFDEC.J86

FUNCTION BCF.SET PUBLIC

    !Changed the variable definitions in alphabetical order                 !4.11 AS
    !and added new variable definitions.                                    !4.11 AS
    BCF.FILE.NAME$            = "BRCF"                                      !4.11 AS
    BCF.REC21.REC.LEN%        = 78      !Total Supplier number length       !4.11 AS
    BCF.REC21.SUPPLIER.LEN%   = 6       !Length of the supplier number      !4.11 AS
    BCF.REC22.REC.LEN%        = 78      !Total GCM record length            !4.12 CSk
    BCF.REC22.PROD.GROUP.LEN% = 5       !Length of GCM Product Group No     !4.12 CSk
    BCF.MAX.STATUS.COUNT%     = 4       !Maximum item status records        !OMW
    BCF.RECL%                 = 84                                          !4.11 AS
    BCF.REPORT.NUM%           = 33                                          !4.11 AS

END FUNCTION

FUNCTION BCF.RECORD.1.SPLIT                                                 !SWM

    BCF.IUF.SERIAL.NO$    = MID$(BCF.RECORD$,1,5)
    BCF.CC.SERIAL.NO      = VAL(UNPACK$(MID$(BCF.RECORD$,6,1)))
    BCF.OPEN.DATE$        = MID$(BCF.RECORD$,7,6)
    BCF.FILLER.DATE$      = MID$(BCF.RECORD$,13,3)
    BCF.LABEL.DATE$       = MID$(BCF.RECORD$,16,6)
    BCF.SALES.SERIAL.NO$  = MID$(BCF.RECORD$,22,5)
    BCF.STMVB.SERIAL.NO$  = MID$(BCF.RECORD$,27,5)
    BCF.TOF.DAYS$         = MID$(BCF.RECORD$,32,2)
    BCF.EPS.BATCH$        = MID$(BCF.RECORD$,34,2)
    BCF.NO.CPM.COPIES%    = VAL(MID$(BCF.RECORD$,36,1))
    BCF.NO.EPF.COPIES$    = MID$(BCF.RECORD$,37,2)
    BCF.CCMVT.SERIAL.NUM$ = MID$(BCF.RECORD$,39,5)                          !BMJK
    BCF.TXR.SERIAL.NUM$   = MID$(BCF.RECORD$,44,5)                          !SWM
    BCF.CUSTD.SERIAL.NUM$ = MID$(BCF.RECORD$,49,5)                          !SWM
    BCF.PSB58.DATE$       = MID$(BCF.RECORD$,54,6)                          !SWM
    BCF.CTSL1.SERIAL.NUM$ = MID$(BCF.RECORD$,60,5)                          !SWM
    BCF.MTSLQ.DAYS$       = MID$(BCF.RECORD$,65,2)                          !4.6 RC
    BCF.FILLER67$         = MID$(BCF.RECORD$,67,8)                          !4.6 RC
    BCF.ACSAL.SERIAL.NUM$ = MID$(BCF.RECORD$,75,5)                          !FSH
    BCF.FILLER$           = MID$(BCF.RECORD$,80,1)                          !FSH

    END FUNCTION                                                            !SWM

FUNCTION BCF.RECORD.1.CONCAT                                                !SWM

    BCF.RECORD$ = BCF.IUF.SERIAL.NO$ +                                  \
                  RIGHT$(PACK$("00"+STR$(BCF.CC.SERIAL.NO)),1) +        \
                  BCF.OPEN.DATE$     + BCF.FILLER.DATE$     +           \
                  BCF.LABEL.DATE$    + BCF.SALES.SERIAL.NO$ +           \
                  BCF.STMVB.SERIAL.NO$ + BCF.TOF.DAYS$ +                \
                  BCF.EPS.BATCH$ + STR$(BCF.NO.CPM.COPIES%) +           \
                  BCF.NO.EPF.COPIES$ +                                  \
                  BCF.CCMVT.SERIAL.NUM$ +                               \   !BMJK
                  BCF.TXR.SERIAL.NUM$ +                                 \   !SWM
                  BCF.CUSTD.SERIAL.NUM$ +                               \   !SWM
                  BCF.PSB58.DATE$ +                                     \   !SWM
                  BCF.CTSL1.SERIAL.NUM$ +                               \   !SWM
                  BCF.MTSLQ.DAYS$       +                               \   !4.6 RC
                  BCF.FILLER67$         +                               \   !4.6 RC
                  BCF.ACSAL.SERIAL.NUM$ +                               \   !FSH
                  BCF.FILLER$

END FUNCTION                                                                !SWM

\ DENTISTRY

FUNCTION BCF.RECORD.9.WNT                                                   !GMG

    BCF.RECORD$ = BCF.NTIUF.SERIAL.NO$ +                                \   !GMG
                  BCF.FILLER$          +                                \   !GMG
                  BCF.DENTISTRY.PRODUCT.GROUP$                              !GMG

END FUNCTION                                                                !GMG

FUNCTION BCF.RECORD.9.RNT                                                   !GMG

    BCF.NTIUF.SERIAL.NO$         = MID$(BCF.RECORD$,1,3)                    !GMG
    BCF.FILLER$                  = MID$(BCF.RECORD$,4,6)                    !GMG
    BCF.DENTISTRY.PRODUCT.GROUP$ = MID$(BCF.RECORD$,10,71)                  !JJT

END FUNCTION

\ WELL-BEING

FUNCTION BCF.RECORD.10.WWNT                                                 !HMG

    BCF.RECORD$ = BCF.WELL.SERIAL.NO$ +                                 \   !HMG
                  BCF.FILLER$          +                                \   !HMG
                  BCF.WELL.PRODUCT.GROUP$                                   !HMG

END FUNCTION                                                                !HMG

FUNCTION BCF.RECORD.10.RWNT                                                 !HMG

    BCF.WELL.SERIAL.NO$ = MID$(BCF.RECORD$,1,3)                             !HMG
    BCF.FILLER$            = MID$(BCF.RECORD$,4,6)                          !HMG
    BCF.WELL.PRODUCT.GROUP$     = MID$(BCF.RECORD$,10,71)                   !JJT

END FUNCTION

\ ECO/TBAG

FUNCTION BCF.RECORD.11.R                                                    !IAH

    BCF.TBAG.BATCH.NO$  = MID$(BCF.RECORD$,1,5)                             !IAH
    BCF.TBAG.DAYS.KEPT$ = MID$(BCF.RECORD$,6,2)                             !IAH
    BCF.NEWLINES.WEEKS$ = MID$(BCF.RECORD$,8,3)                             !4.7JAS
    BCF.NEWLINES.LINES$ = MID$(BCF.RECORD$,11,5)                            !4.8JAS
    BCF.TBAG.FILLER$    = MID$(BCF.RECORD$,16,65)                           !4.8JAS

END FUNCTION

FUNCTION BCF.RECORD.11.W                                                    !IAH

    BCF.RECORD$ = BCF.TBAG.BATCH.NO$ +                                  \
                  BCF.TBAG.DAYS.KEPT$ +                                 \   !IAH
                  BCF.NEWLINES.WEEKS$ +                                 \
                  BCF.NEWLINES.LINES$ +                                 \   !4.8JAS
                  BCF.TBAG.FILLER$                                          !4.7JAS

END FUNCTION


\ WELL-BEING SERVICES                                                       !KBG

FUNCTION BCF.RECORD.13.W                                                    !KBG

    BCF.RECORD$ = BCF.FILLER$ +                                         \   !KBG
                  LEFT$(BCF.WELL.SERV.PRODUCT.GROUP$,65) +              \   !KBG
                  BCF.ETOPUP.PROD.GRP$ +                                \   !LAH
                  BCF.END.FILLER$                                           !LAH

END FUNCTION                                                                !KBG

FUNCTION BCF.RECORD.13.R                                                    !KBG

    BCF.FILLER$                  = MID$(BCF.RECORD$,1,9)                    !KBG
    BCF.WELL.SERV.PRODUCT.GROUP$ = MID$(BCF.RECORD$,10,70)                  !KBG
    BCF.ETOPUP.PROD.GRP$         = MID$(BCF.RECORD$,75,5)                   !LAH
    BCF.END.FILLER$              = MID$(BCF.RECORD$,80,1)                   !LAH

END FUNCTION                                                                !KBG

\ DEALDIR & DIDIR serial numbers                                            !MJAS

FUNCTION BCF.RECORD.14.W                                                    !MJAS

    BCF.RECORD$ = BCF.DEALDIR.SERIAL.NUM$ +                             \   !MJAS
                  BCF.DIDIR.SERIAL.NUM$   +                             \   !MJAS
                  BCF.DEAL.NUM.REC$       +                             \   !MJAS
                   BCF.ITMDL.NUM.REC$      +                           \ QCS
                  BCF.DEAL.KEY.LEN$       +                             \   !MJAS
                   BCF.ITMDL.KEY.LEN$      +                           \ QCS
                  BCF.DVCHR.SERIAL.NUM$   +                             \   !NJT
                   BCF.FILLER.DATETIMESTAMP$  +                        \ QCS
                  BCF.FILLER$                                               !MJAS

END FUNCTION                                                                !MJAS

FUNCTION BCF.RECORD.14.R                                                    !MJAS

     BCF.DEALDIR.SERIAL.NUM$      = MID$(BCF.RECORD$,1,4)              ! MJAS
     BCF.DIDIR.SERIAL.NUM$        = MID$(BCF.RECORD$,5,4)              ! MJAS
     BCF.DEAL.NUM.REC$            = MID$(BCF.RECORD$,9,6)              ! MJAS
     BCF.ITMDL.NUM.REC$           = MID$(BCF.RECORD$,15,6)             !QCS
     BCF.DEAL.KEY.LEN$            = MID$(BCF.RECORD$,21,3)             ! MJAS
     BCF.ITMDL.KEY.LEN$           = MID$(BCF.RECORD$,24,3)             !QCS
     BCF.DVCHR.SERIAL.NUM$        = MID$(BCF.RECORD$,27,4)             ! NJT
     BCF.FILLER.DATETIMESTAMP$    = MID$(BCF.RECORD$,31,17)            !QCS
     BCF.FILLER$                  = MID$(BCF.RECORD$,48,33)            ! NJT ! 4.10CSk

END FUNCTION                                                                !MJAS

\ Last successfully processed IUF from SAP.                                 !4.10CSk

FUNCTION BCF.RECORD.20.W                                                    !4.10CSk

    BCF.RECORD$ = BCF.IUF.DATETIMESTAMP$ +                              \   !4.10CSk
                  BCF.FILLER$                                               !4.10CSk

END FUNCTION                                                                !4.10CSk

FUNCTION BCF.RECORD.20.R                                                    !4.10CSk

    BCF.IUF.DATETIMESTAMP$       = MID$(BCF.RECORD$,1,17)                   !4.10CSk
    BCF.FILLER$                  = MID$(BCF.RECORD$,18,63)                  !4.10CSk

END FUNCTION                                                                !4.10CSk

\***********************************************************************    !PMW
\***                                                                        !PMW
\***    BCF.RECORD.23.R                                                     !PMW
\***                                                                        !PMW
\***********************************************************************    !PMW
\***                                                                        !PMW
\***    Split BCF record 23 (stock snapshot parameters) into the            !PMW
\***    required individual fields.                                         !PMW
\***                                                                        !PMW
\***********************************************************************    !PMW
FUNCTION BCF.RECORD.23.R                                                    !OMW
                                                                            !OMW
    INTEGER*1 INDEX%                                                        !OMW
                                                                            !OMW
    DIM BCF.ITEM.STATUS$(BCF.MAX.STATUS.COUNT%)                             !OMW
    DIM BCF.POSITIVE.DAYS%(BCF.MAX.STATUS.COUNT%)                           !OMW
    DIM BCF.POSITIVE.STOCK.FLAG$(BCF.MAX.STATUS.COUNT%)                     !OMW
    DIM BCF.NEGATIVE.DAYS%(BCF.MAX.STATUS.COUNT%)                           !OMW
    DIM BCF.NEGATIVE.STOCK.FLAG$(BCF.MAX.STATUS.COUNT%)                     !OMW
    DIM BCF.ZERO.DAYS%(BCF.MAX.STATUS.COUNT%)                               !OMW
    DIM BCF.ZERO.STOCK.FLAG$(BCF.MAX.STATUS.COUNT%)                         !OMW
                                                                            !OMW
    BCF.MAX.STOCK.INIT.MESSAGES% = VAL(MID$(BCF.RECORD$,1,4))               !OMW
    BCF.MAX.STOCK.INIT.ITEMS%    = VAL(MID$(BCF.RECORD$,5,4))               !OMW
                                                                            !OMW
    FOR INDEX% = 1 TO BCF.MAX.STATUS.COUNT%                                 !OMW
                                                                            !OMW
        BCF.ITEM.STATUS$(INDEX%)         = MID$(BCF.RECORD$,            \   !OMW
                                           ((INDEX% - 1) * 16) + 9,1)       !OMW
        BCF.POSITIVE.STOCK.FLAG$(INDEX%) = MID$(BCF.RECORD$,            \   !OMW
                                           ((INDEX% - 1) * 16) + 10,1)      !OMW
        BCF.POSITIVE.DAYS%(INDEX%)       = VAL(MID$(BCF.RECORD$,        \   !OMW
                                           ((INDEX% - 1) * 16) + 11,4))     !OMW
        BCF.ZERO.STOCK.FLAG$(INDEX%)     = MID$(BCF.RECORD$,            \   !OMW
                                           ((INDEX% - 1) * 16) + 15,1)      !OMW
        BCF.ZERO.DAYS%(INDEX%)           = VAL(MID$(BCF.RECORD$,        \   !OMW
                                           ((INDEX% - 1) * 16) + 16,4))     !OMW
        BCF.NEGATIVE.STOCK.FLAG$(INDEX%) = MID$(BCF.RECORD$,            \   !OMW
                                           ((INDEX% - 1) * 16) + 20,1)      !OMW
        BCF.NEGATIVE.DAYS%(INDEX%)       = VAL(MID$(BCF.RECORD$,        \   !OMW
                                           ((INDEX% - 1) * 16) + 21,4))     !OMW
                                                                            !OMW
        ! IF item status is active                                          !OMW
        IF BCF.ITEM.STATUS$(INDEX%) = "A" THEN BEGIN                        !OMW
            ! Map its value to BLANK for comparison purposes                !OMW
            BCF.ITEM.STATUS$(INDEX%) = " "                                  !OMW
        ENDIF                                                               !OMW
                                                                            !OMW
    NEXT INDEX%                                                             !OMW
                                                                            !OMW
END FUNCTION                                                                !OMW
                                                                            !OMW
\***********************************************************************    !PMW
\***                                                                        !PMW
\***    BCF.RECORD.23.R                                                     !PMW
\***                                                                        !PMW
\***********************************************************************    !PMW
\***                                                                        !PMW
\***    Combine the required individual fields to build                     !PMW
\***    BCF record 23 (stock snapshot parameters)                           !PMW
\***                                                                        !PMW
\***********************************************************************    !PMW
FUNCTION BCF.RECORD.23.W                                                    !OMW
                                                                            !OMW
    INTEGER*1 INDEX%                                                        !OMW
                                                                            !OMW
    BCF.RECORD$ =                                                       \   !OMW
        RIGHT$("0000" + STR$(BCF.MAX.STOCK.INIT.MESSAGES%),4) +         \   !OMW 
        RIGHT$("0000" + STR$(BCF.MAX.STOCK.INIT.ITEMS%),4)                  !OMW
                                                                            !OMW
    FOR INDEX% = 1 TO BCF.MAX.STATUS.COUNT%                                 !OMW
                                                                            !OMW
        ! IF item status is active                                          !OMW
        IF BCF.ITEM.STATUS$(INDEX%) = " " THEN BEGIN                        !OMW
            BCF.ITEM.STATUS$(INDEX%) = "A"                                  !OMW
        ENDIF                                                               !OMW
                                                                            !OMW
        BCF.RECORD$ = BCF.RECORD$ +                                     \   !OMW
            BCF.ITEM.STATUS$(INDEX%) +                                  \   !OMW
            BCF.POSITIVE.STOCK.FLAG$(INDEX%) +                          \   !OMW
            RIGHT$("0000" + STR$(BCF.POSITIVE.DAYS%(INDEX%)),4) +       \   !OMW
            BCF.ZERO.STOCK.FLAG$(INDEX%) +                              \   !OMW
            RIGHT$("0000" + STR$(BCF.ZERO.DAYS%(INDEX%)),4) +           \   !OMW
            BCF.NEGATIVE.STOCK.FLAG$(INDEX%) +                          \   !OMW
            RIGHT$("0000" + STR$(BCF.NEGATIVE.DAYS%(INDEX%)),4)             !OMW
                                                                            !OMW
    NEXT INDEX%                                                             !OMW
                                                                            !OMW
    BCF.RECORD$ = LEFT$(BCF.RECORD$ + STRING$(80," "),80)                   !OMW
                                                                            !OMW
END FUNCTION                                                                !OMW

FUNCTION READ.BCF PUBLIC

    INTEGER*2 READ.BCF
    READ.BCF = 1

    IF END #BCF.SESS.NUM% THEN READ.BCF.ERROR
    READ #BCF.SESS.NUM%, BCF.REC.NO%; BCF.RECORD$

    IF BCF.REC.NO% = 1 THEN BEGIN                                           !SWM
        CALL BCF.RECORD.1.SPLIT                                             !SWM
    ENDIF ELSE IF BCF.REC.NO% = 9 THEN BEGIN                                !GMG
        CALL BCF.RECORD.9.RNT                                               !GMG
    ENDIF ELSE IF BCF.REC.NO% = 10 THEN BEGIN                               !HMG
        CALL BCF.RECORD.10.RWNT                                             !HMG
    ENDIF ELSE IF BCF.REC.NO% = 11 THEN BEGIN                               !IAH
        CALL BCF.RECORD.11.R                                                !IAH
    ENDIF ELSE IF BCF.REC.NO% = 13 THEN BEGIN                               !KBG
        CALL BCF.RECORD.13.R                                                !KBG
    ENDIF ELSE IF BCF.REC.NO% = 14 THEN BEGIN                               !MJAS
        CALL BCF.RECORD.14.R                                                !MJAS
    ENDIF ELSE IF BCF.REC.NO% = 20 THEN BEGIN                               !4.10CSk
        CALL BCF.RECORD.20.R                                                !4.10CSk
    ENDIF ELSE IF BCF.REC.NO% = 23 THEN BEGIN                               !OMW
        CALL BCF.RECORD.23.R                                                !OMW
    ENDIF                                                                   !MJAS

    READ.BCF = 0
    
    EXIT FUNCTION

READ.BCF.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(BCF.REC.NO%)),7)

END FUNCTION

FUNCTION READ.BCF.LOCK PUBLIC

    INTEGER*2 READ.BCF.LOCK

    READ.BCF.LOCK = 1

    IF END #BCF.SESS.NUM% THEN READ.BCF.LOCK.ERROR
    READ #BCF.SESS.NUM% AUTOLOCK, BCF.REC.NO%; BCF.RECORD$

    IF BCF.REC.NO% = 1 THEN BEGIN                                           !SWM
        CALL BCF.RECORD.1.SPLIT                                             !SWM
    ENDIF ELSE IF BCF.REC.NO% = 9 THEN BEGIN                                !GMG
        CALL BCF.RECORD.9.RNT                                               !GMG
    ENDIF ELSE IF BCF.REC.NO% = 10 THEN BEGIN                               !HMG
        CALL BCF.RECORD.10.RWNT                                             !HMG
    ENDIF ELSE IF BCF.REC.NO% = 11 THEN BEGIN                               !IAH
        CALL BCF.RECORD.11.R                                                !IAH
    ENDIF ELSE IF BCF.REC.NO% = 13 THEN BEGIN                               !KBG
        CALL BCF.RECORD.13.R                                                !KBG
    ENDIF ELSE IF BCF.REC.NO% = 14 THEN BEGIN                               !MJAS
        CALL BCF.RECORD.14.R                                                !MJAS
    ENDIF ELSE IF BCF.REC.NO% = 20 THEN BEGIN                               !4.10CSk
        CALL BCF.RECORD.20.R                                                !4.10CSk
    ENDIF ELSE IF BCF.REC.NO% = 23 THEN BEGIN                               !OMW
        CALL BCF.RECORD.23.R                                                !OMW
    ENDIF                                                                   !MJAS

    READ.BCF.LOCK = 0
    
    EXIT FUNCTION

READ.BCF.LOCK.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(BCF.REC.NO%)),7)

  END FUNCTION

FUNCTION WRITE.BCF PUBLIC

    INTEGER*2 WRITE.BCF

    WRITE.BCF = 1

    IF BCF.REC.NO% = 1 THEN BEGIN                                           !SWM
        CALL BCF.RECORD.1.CONCAT                                            !SWM
    ENDIF ELSE IF BCF.REC.NO% = 9 THEN BEGIN                                !GMG
        CALL BCF.RECORD.9.WNT                                               !GMG
    ENDIF ELSE IF BCF.REC.NO% = 10 THEN BEGIN                               !HMG
        CALL BCF.RECORD.10.WWNT                                             !HMG
    ENDIF ELSE IF BCF.REC.NO% = 11 THEN BEGIN                               !IAH
        CALL BCF.RECORD.11.W                                                !IAH
    ENDIF ELSE IF BCF.REC.NO% = 13 THEN BEGIN                               !KBG
        CALL BCF.RECORD.13.W                                                !KBG
    ENDIF ELSE IF BCF.REC.NO% = 14 THEN BEGIN                               !MJAS
        CALL BCF.RECORD.14.W                                                !MJAS
    ENDIF ELSE IF BCF.REC.NO% = 20 THEN BEGIN                               !4.10CSk
        CALL BCF.RECORD.20.W                                                !4.10CSk
    ENDIF ELSE IF BCF.REC.NO% = 23 THEN BEGIN                               !OMW
        CALL BCF.RECORD.23.W                                                !OMW
    ENDIF                                                                   !MJAS

    IF END #BCF.SESS.NUM% THEN WRITE.BCF.ERROR
    WRITE #BCF.SESS.NUM%, BCF.REC.NO%; BCF.RECORD$

    WRITE.BCF = 0

    EXIT FUNCTION

WRITE.BCF.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(BCF.REC.NO%)),7)

END FUNCTION

FUNCTION WRITE.BCF.UNLOCK PUBLIC

    INTEGER*2 WRITE.BCF.UNLOCK

    WRITE.BCF.UNLOCK = 1

    IF BCF.REC.NO% = 1 THEN BEGIN                                           !SWM
        CALL BCF.RECORD.1.CONCAT                                            !SWM
    ENDIF ELSE IF BCF.REC.NO% = 9 THEN BEGIN                                !GMG
        CALL BCF.RECORD.9.WNT                                               !GMG
    ENDIF ELSE IF BCF.REC.NO% = 10 THEN BEGIN                               !HMG
        CALL BCF.RECORD.10.WWNT                                             !HMG
    ENDIF ELSE IF BCF.REC.NO% = 11 THEN BEGIN                               !IAH
        CALL BCF.RECORD.11.W                                                !IAH
    ENDIF ELSE IF BCF.REC.NO% = 13 THEN BEGIN                               !KBG
        CALL BCF.RECORD.13.W                                                !KBG
    ENDIF ELSE IF BCF.REC.NO% = 14 THEN BEGIN                               !MJAS
        CALL BCF.RECORD.14.W                                                !MJAS
    ENDIF ELSE IF BCF.REC.NO% = 20 THEN BEGIN                               !4.10CSk
        CALL BCF.RECORD.20.W                                                !4.10CSk
    ENDIF ELSE IF BCF.REC.NO% = 23 THEN BEGIN                               !OMW
        CALL BCF.RECORD.23.W                                                !OMW
    ENDIF                                                                   !MJAS

    IF END #BCF.SESS.NUM% THEN WRITE.BCF.UNLOCK.ERROR
    WRITE #BCF.SESS.NUM% AUTOUNLOCK, BCF.REC.NO%;BCF.RECORD$

    WRITE.BCF.UNLOCK = 0
    
    EXIT FUNCTION

WRITE.BCF.UNLOCK.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(BCF.REC.NO%)),7)

END FUNCTION

FUNCTION WRITE.HOLD.BCF.UNLOCK PUBLIC

    INTEGER*2 WRITE.HOLD.BCF.UNLOCK

    WRITE.HOLD.BCF.UNLOCK = 1

    IF BCF.REC.NO% = 1 THEN BEGIN                                           !SWM
        CALL BCF.RECORD.1.CONCAT                                            !SWM
    ENDIF ELSE IF BCF.REC.NO% = 9 THEN BEGIN                                !GMG
        CALL BCF.RECORD.9.WNT                                               !GMG
    ENDIF ELSE IF BCF.REC.NO% = 10 THEN BEGIN                               !HMG
        CALL BCF.RECORD.10.WWNT                                             !HMG
    ENDIF ELSE IF BCF.REC.NO% = 11 THEN BEGIN                               !IAH
        CALL BCF.RECORD.11.W                                                !IAH
    ENDIF ELSE IF BCF.REC.NO% = 13 THEN BEGIN                               !KBG
        CALL BCF.RECORD.13.W                                                !KBG
    ENDIF ELSE IF BCF.REC.NO% = 14 THEN BEGIN                               !MJAS
        CALL BCF.RECORD.14.W                                                !MJAS
    ENDIF ELSE IF BCF.REC.NO% = 20 THEN BEGIN                               !4.10CSk
        CALL BCF.RECORD.20.W                                                !4.10CSk
    ENDIF ELSE IF BCF.REC.NO% = 23 THEN BEGIN                               !OMW
        CALL BCF.RECORD.23.W                                                !OMW
    ENDIF                                                                   !MJAS

    IF END #BCF.SESS.NUM% THEN WRITE.HOLD.BCF.UNLOCK.ERROR
    WRITE HOLD #BCF.SESS.NUM% AUTOUNLOCK, BCF.REC.NO%;BCF.RECORD$

    WRITE.HOLD.BCF.UNLOCK = 0
    
    EXIT FUNCTION

WRITE.HOLD.BCF.UNLOCK.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = BCF.REPORT.NUM%
    CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(BCF.REC.NO%)),7)

END FUNCTION

