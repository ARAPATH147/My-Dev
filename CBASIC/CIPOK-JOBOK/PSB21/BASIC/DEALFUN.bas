!****************************************************************************
!
!       MODULE:         DEAL FILE FUNCTIONS
!
!       AUTHOR:         STUART WILLIAM MCCONNACHIE
!
!       DATE:           AUGUST 2002
!
!****************************************************************************
!
!       VERSION A           STUART WILLIAM MCCONNACHIE          AUG 2002
!
!       Functions for adding and removing deals from the new format DEAL
!       file for the flexible deals project.
!       Note the deal file is a double linked list of all deals on the
!       system, so must not be updated directly without using the function.
!
!       VERSION B           STUART WILLIAM MCCONNACHIE          OCT 2003
!
!       Added buffering of DEAL file for reads via READ.DEAL.BUFFER.
!       Speeds up operation of PSBF42 when used there.
!       
!       VERSION C           STUART WILLIAM MCCONNACHIE          AUG 2005
!
!       Addition of DEAL MODEL file.
!       
!****************************************************************************

%INCLUDE DEALDEC.J86


    INTEGER*2           DEAL.RECORD.LTH
    INTEGER*2           DEAL.NUM%                                           !BSWM
    INTEGER*2           I%
    INTEGER*2           DEAL.NO.QLFNS%
    INTEGER*2           DEAL.NO.REWARDS%
    INTEGER*2           DEAL.START.HOUR%
    INTEGER*2           DEAL.START.MIN%
    INTEGER*2           DEAL.END.HOUR%
    INTEGER*2           DEAL.END.MIN%

    STRING              HOME.DEAL.NUM$
    STRING              NULL.DEAL.RECORD$
    STRING              PREV.DEAL.NUM$
    STRING              NEXT.DEAL.NUM$
    STRING              DEAL.START.HOUR$
    STRING              DEAL.START.MIN$
    STRING              DEAL.END.HOUR$
    STRING              DEAL.END.MIN$
    STRING              DEAL.PREV.DEAL.NUM$
    STRING              DEAL.NEXT.DEAL.NUM$
    STRING              NULL.TILL.DATA$
    STRING              NULL.DEAL.DATA$

    INTEGER*1           DEAL.BUFFER                                         !BSWM
    STRING              DEAL.BUFFER$(1)                                     !BSWM
    
    STRING GLOBAL       FILE.OPERATION$
    STRING GLOBAL       CURRENT.CODE$
    INTEGER*2 GLOBAL    CURRENT.REPORT.NUM%                                 !BSWM    
    
    INTEGER*2           SESS.NUM%                                           !CSWM
    INTEGER*2           REPORT.NUM%                                         !CSWM

!****************************************************************************

    FUNCTION SUBSTR(S1$, O1%, S2$, O2%, L2%) EXTERNAL
        STRING    S1$, S2$
        INTEGER*2 O1%, O2%, L2%
    END FUNCTION

    FUNCTION GETN4(S$, O%) EXTERNAL
        INTEGER*4   GETN4
        STRING      S$
        INTEGER*2   O%
    END FUNCTION

    FUNCTION PUTN4(S$, O%, N%) EXTERNAL
        STRING      S$
        INTEGER*2   O%
        INTEGER*4   N%
    END FUNCTION

    FUNCTION GETN2(S$, O%) EXTERNAL
        INTEGER*2   GETN2
        STRING      S$
        INTEGER*2   O%
    END FUNCTION

    FUNCTION PUTN2(S$, O%, N%) EXTERNAL
        STRING      S$
        INTEGER*2   O%
        INTEGER*2   N%
    END FUNCTION

!****************************************************************************

    FUNCTION GETN1 (S$, OFFSET%)

        INTEGER*2 GETN1
        STRING S$
        INTEGER*2 OFFSET%

        GETN1 = ASC(MID$(S$, OFFSET%+1, 1))

    END FUNCTION

!****************************************************************************

    SUB PUTN1 (S$, OFFSET%, VALUE%)

        INTEGER*2 OFFSET%
        INTEGER*2 VALUE%
        STRING S$

        POKE SADD(S$)+OFFSET%+2, VALUE%

    END SUB

!****************************************************************************

    FUNCTION DEAL.SET PUBLIC

        INTEGER*1   DEAL.SET

        DEAL.FILE.NAME$ = "DEAL"
        DEAL.RECL% = 400
        DEAL.REPORT.NUM% = 311

        DEAL.RECORD.LTH = 394
        
        DEAL.MODEL.FILE.NAME$ = "DEALMODL"
        DEAL.MODEL.RECL% = 400
        DEAL.MODEL.REPORT.NUM% = 330

        DEAL.NO.QLFNS% = 4
        DEAL.NO.REWARDS% = 3

        DIM DEAL.QLFY.FLAG%(DEAL.NO.QLFNS%)
        DIM DEAL.QLFY.CODE%(DEAL.NO.QLFNS%)
        DIM DEAL.QLFY.AMNT%(DEAL.NO.QLFNS%)
        DIM DEAL.QLFY.LIST%(DEAL.NO.QLFNS%)

        DIM DEAL.REWARD.CODE%(DEAL.NO.REWARDS%)
        DIM DEAL.REWARD.QTY%(DEAL.NO.REWARDS%)
        DIM DEAL.REWARD.AMNT%(DEAL.NO.REWARDS%)
        DIM DEAL.REWARD.LIST%(DEAL.NO.REWARDS%)
        DIM DEAL.QLFY.MSG%(DEAL.NO.REWARDS%)
        DIM DEAL.REWARD.MSG%(DEAL.NO.REWARDS%)
        DIM DEAL.MAX.QLFY%(DEAL.NO.REWARDS%)
        DIM DEAL.NUM.REWARD.QLFY%(DEAL.NO.REWARDS%)
        DIM DEAL.QLFY.MSG.RCPT$(DEAL.NO.REWARDS%)
        DIM DEAL.QLFY.MSG.DISP$(DEAL.NO.REWARDS%)
        DIM DEAL.REWARD.MSG.RCPT$(DEAL.NO.REWARDS%)

        HOME.DEAL.NUM$    = PACK$("????")
        NULL.DEAL.RECORD$ = STRING$(394, CHR$(0))
        NULL.DEAL.DATA$   = STRING$(180, CHR$(0))

        DEAL.SET = 0

    END FUNCTION

!****************************************************************************

    SUB DEAL.SPLIT.RECORD PUBLIC
    
        IF LEN(DEAL.RECORD$) <> DEAL.RECORD.LTH THEN EXIT SUB               !BSWM

        DEAL.START.DATE$         = MID$(DEAL.RECORD$,   1,  4) !MID$'S START AT 1
        DEAL.START.TIME%         = GETN2(DEAL.RECORD$,  4)     !GET'S START AT 0
        DEAL.END.DATE$           = MID$(DEAL.RECORD$,   7,  4)
        DEAL.END.TIME%           = GETN2(DEAL.RECORD$, 10)
        DEAL.BUSINESS.CENTRE$    = MID$(DEAL.RECORD$,  13,  1)
        DEAL.DEAL.DESC$          = MID$(DEAL.RECORD$,  14, 35)
        DEAL.NUM.LISTS%          = GETN1(DEAL.RECORD$, 48)
        DEAL.RUN.DAY%            = GETN1(DEAL.RECORD$, 49)
        DEAL.PRIORITY%           = GETN1(DEAL.RECORD$, 50)
        DEAL.EXEMPT.TILLS%       = GETN2(DEAL.RECORD$, 51)
        DEAL.FLAGS1%             = GETN1(DEAL.RECORD$, 53)
        DEAL.SALES.PROMPT%       = GETN1(DEAL.RECORD$, 54)
        DEAL.EXCLUSION.MSG%      = GETN1(DEAL.RECORD$, 55)
        DEAL.FLAGS2%             = GETN1(DEAL.RECORD$, 56)
        DEAL.NUM.QLFNS%          = GETN1(DEAL.RECORD$, 57)
        DEAL.NUM.REWARDS%        = GETN1(DEAL.RECORD$, 58)

        FOR I% = 0 TO DEAL.NO.QLFNS% - 1
            DEAL.QLFY.FLAG%(I%)   = GETN1(DEAL.RECORD$, 62+I%*8)
            DEAL.QLFY.CODE%(I%)   = GETN1(DEAL.RECORD$, 63+I%*8)
            DEAL.QLFY.AMNT%(I%)   = GETN4(DEAL.RECORD$, 64+I%*8)
            DEAL.QLFY.LIST%(I%)   = GETN1(DEAL.RECORD$, 68+I%*8)
        NEXT I%

        FOR I% = 0 TO DEAL.NO.REWARDS% - 1
            DEAL.REWARD.CODE%(I%)     = GETN1(DEAL.RECORD$,  94+I%*100)
            DEAL.REWARD.QTY%(I%)      = GETN1(DEAL.RECORD$,  95+I%*100)
            DEAL.REWARD.AMNT%(I%)     = GETN4(DEAL.RECORD$,  96+I%*100)
            DEAL.REWARD.LIST%(I%)     = GETN1(DEAL.RECORD$, 100+I%*100)
            DEAL.QLFY.MSG%(I%)        = GETN1(DEAL.RECORD$, 101+I%*100)
            DEAL.QLFY.MSG.RCPT$(I%)   = MID$(DEAL.RECORD$,  103+I%*100, 38)
            DEAL.QLFY.MSG.DISP$(I%)   = MID$(DEAL.RECORD$,  141+I%*100, 20)
            DEAL.REWARD.MSG%(I%)      = GETN1(DEAL.RECORD$, 160+I%*100)
            DEAL.REWARD.MSG.RCPT$(I%) = MID$(DEAL.RECORD$,  162+I%*100, 27)
            DEAL.MAX.QLFY%(I%)        = GETN2(DEAL.RECORD$, 188+I%*100)
            DEAL.NUM.REWARD.QLFY%(I%) = GETN2(DEAL.RECORD$, 190+I%*100)
        NEXT I%

        DEAL.START.HOUR$ = RIGHT$("00" + STR$(DEAL.START.TIME% / 60),2)
        DEAL.START.MIN$  = RIGHT$("00" + STR$(DEAL.START.TIME% -  (VAL(DEAL.START.HOUR$)* 60)),2)
        DEAL.START.TIME$ = PACK$(DEAL.START.HOUR$) + PACK$(DEAL.START.MIN$)

        DEAL.END.HOUR$   = RIGHT$("00" + STR$(DEAL.END.TIME% / 60),2)
        DEAL.END.MIN$    = RIGHT$("00" + STR$(DEAL.END.TIME% - (VAL(DEAL.END.HOUR$) * 60)),2)
        DEAL.END.TIME$   = PACK$(DEAL.END.HOUR$) + PACK$(DEAL.END.MIN$)

    END SUB

!****************************************************************************

    SUB DEAL.CONCAT.RECORD PUBLIC

        DEAL.RECORD$ = STRING$(DEAL.RECORD.LTH, CHR$(0))

        DEAL.START.HOUR% = VAL(UNPACK$(LEFT$(DEAL.START.TIME$,1))) * 60
        DEAL.START.MIN%  = VAL(UNPACK$(RIGHT$(DEAL.START.TIME$,1)))
        DEAL.START.TIME% = DEAL.START.HOUR% + DEAL.START.MIN%
        DEAL.END.HOUR%   = VAL(UNPACK$(LEFT$(DEAL.END.TIME$,1))) * 60
        DEAL.END.MIN%    = VAL(UNPACK$(RIGHT$(DEAL.END.TIME$,1)))
        DEAL.END.TIME%   = DEAL.END.HOUR% + DEAL.END.MIN%
        DEAL.DATA$       = NULL.DEAL.DATA$

        CALL SUBSTR(DEAL.RECORD$,  0, DEAL.START.DATE$, 0, 4)
        CALL PUTN2(DEAL.RECORD$,   4, DEAL.START.TIME%)
        CALL SUBSTR(DEAL.RECORD$,  6, DEAL.END.DATE$, 0, 4)
        CALL PUTN2(DEAL.RECORD$,  10, DEAL.END.TIME%)
        CALL SUBSTR(DEAL.RECORD$, 12, DEAL.BUSINESS.CENTRE$, 0, 1)
        CALL SUBSTR(DEAL.RECORD$, 13, DEAL.DEAL.DESC$, 0, 35)
        CALL PUTN1(DEAL.RECORD$,  48, DEAL.NUM.LISTS%)
        CALL PUTN1(DEAL.RECORD$,  49, DEAL.RUN.DAY%)
        CALL PUTN1(DEAL.RECORD$,  50, DEAL.PRIORITY%)
        CALL PUTN2(DEAL.RECORD$,  51, DEAL.EXEMPT.TILLS%)
        CALL PUTN1(DEAL.RECORD$,  53, DEAL.FLAGS1%)
        CALL PUTN1(DEAL.RECORD$,  54, DEAL.SALES.PROMPT%)
        CALL PUTN1(DEAL.RECORD$,  55, DEAL.EXCLUSION.MSG%)
        CALL PUTN1(DEAL.RECORD$,  56, DEAL.FLAGS2%)
        CALL PUTN1(DEAL.RECORD$,  57, DEAL.NUM.QLFNS%)
        CALL PUTN1(DEAL.RECORD$,  58, DEAL.NUM.REWARDS%)

        FOR I% = 0 TO DEAL.NO.QLFNS% - 1
           CALL PUTN1(DEAL.RECORD$, 62+I%*8, DEAL.QLFY.FLAG%(I%))
           CALL PUTN1(DEAL.RECORD$, 63+I%*8, DEAL.QLFY.CODE%(I%))
           CALL PUTN4(DEAL.RECORD$, 64+I%*8, DEAL.QLFY.AMNT%(I%))
           CALL PUTN1(DEAL.RECORD$, 68+I%*8, DEAL.QLFY.LIST%(I%))
        NEXT I%

        FOR I% = 0 TO DEAL.NO.REWARDS% - 1
           CALL PUTN1(DEAL.RECORD$,   94+I%*100, DEAL.REWARD.CODE%(I%))
           CALL PUTN1(DEAL.RECORD$,   95+I%*100, DEAL.REWARD.QTY%(I%))
           CALL PUTN4(DEAL.RECORD$,   96+I%*100, DEAL.REWARD.AMNT%(I%))
           CALL PUTN1(DEAL.RECORD$,  100+I%*100, DEAL.REWARD.LIST%(I%))
           CALL PUTN1(DEAL.RECORD$,  101+I%*100, DEAL.QLFY.MSG%(I%))
           CALL SUBSTR(DEAL.RECORD$, 102+I%*100, DEAL.QLFY.MSG.RCPT$(I%), 0, 38)
           CALL SUBSTR(DEAL.RECORD$, 140+I%*100, DEAL.QLFY.MSG.DISP$(I%), 0, 20)
           CALL PUTN1(DEAL.RECORD$,  160+I%*100, DEAL.REWARD.MSG%(I%))
           CALL SUBSTR(DEAL.RECORD$, 161+I%*100, DEAL.REWARD.MSG.RCPT$(I%), 0, 27)
           CALL PUTN2(DEAL.RECORD$,  188+I%*100, DEAL.MAX.QLFY%(I%))
           CALL PUTN2(DEAL.RECORD$,  190+I%*100, DEAL.NUM.REWARD.QLFY%(I%))
        NEXT I%

    END SUB

!****************************************************************************

    SUB DEAL.SAVE (SAVE$) PUBLIC                                            !BSWM
    
        STRING SAVE$                                                        !BSWM
    
        SAVE$ = DEAL.DEAL.NUM$ +                                            \BSWM
                DEAL.PREV.DEAL.NUM$ +                                       \BSWM
                DEAL.NEXT.DEAL.NUM$ +                                       \BSWM
                DEAL.RECORD$                                                !BSWM
                
    END SUB                                                                 !BSWM
    
!****************************************************************************

    SUB DEAL.RESTORE (RESTORE$) PUBLIC                                      !BSWM
    
        STRING RESTORE$                                                     !BSWM
        
        DEAL.DEAL.NUM$ = MID$(RESTORE$, 1, 2)                               !BSWM
        DEAL.PREV.DEAL.NUM$ = MID$(RESTORE$, 3, 2)                          !BSWM
        DEAL.NEXT.DEAL.NUM$ = MID$(RESTORE$, 5, 2)                          !BSWM
        DEAL.RECORD$ = MID$(RESTORE$, 7, DEAL.RECORD.LTH)                   !BSWM
        CALL DEAL.SPLIT.RECORD                                              !BSWM
        
    END SUB                                                                 !BSWM
    
!****************************************************************************

    FUNCTION DELETE.INT     !DO NOT MAKE PUBLIC - MUST USE REMOVE.DEAL      !CSWM

        INTEGER*1   DELETE.INT                                              !CSWM

        DELETE.INT = 1                                                      !CSWM

        IF END # SESS.NUM% THEN EXIT.FUNCTION                               !CSWM
        DELREC SESS.NUM%; DEAL.DEAL.NUM$                                    !CSWM

        DELETE.INT = 0                                                      !CSWM
        
    EXIT FUNCTION                                                           !BSWM
        
    EXIT.FUNCTION:

        CURRENT.REPORT.NUM% = REPORT.NUM%                                   !BSWM
        FILE.OPERATION$ = "D"                                               !BSWM
        CURRENT.CODE$ = DEAL.DEAL.NUM$                                      !BSWM
        
    END FUNCTION

!****************************************************************************

    FUNCTION WRITE.INT      !DO NOT MAKE PUBLIC - MUST USE ADD.OR.UPDATE.DEAL

        INTEGER*1   WRITE.INT                                               !CSWM

        WRITE.INT = 1                                                       !CSWM

        IF END # SESS.NUM% THEN EXIT.FUNCTION                               !CSWM
        WRITE FORM "C2,2C2,C394"; # SESS.NUM%;                              \CSWM
            DEAL.DEAL.NUM$,                                                 \
            DEAL.PREV.DEAL.NUM$,                                            \
            DEAL.NEXT.DEAL.NUM$,                                            \
            DEAL.RECORD$

        WRITE.INT = 0                                                       !CSWM
        
    EXIT FUNCTION                                                           !BSWM

    EXIT.FUNCTION:

        CURRENT.REPORT.NUM% = REPORT.NUM%                                   !BSWM
        FILE.OPERATION$ = "W"                                               !BSWM
        CURRENT.CODE$ = DEAL.DEAL.NUM$                                      !BSWM
        
    END FUNCTION

!****************************************************************************

    FUNCTION READ.INT       !DO NOT MAKE PUBLIC - MUST USE DEAL.READ        !CSWM

        INTEGER*1   READ.INT                                                !CSWM

        READ.INT = 1                                                        !CSWM

        IF END # SESS.NUM% THEN EXIT.FUNCTION                               !CSWM
        READ FORM "T3,2C2,C394"; # SESS.NUM%                                \CSWM
            KEY DEAL.DEAL.NUM$;                                             \
            DEAL.PREV.DEAL.NUM$,                                            \
            DEAL.NEXT.DEAL.NUM$,                                            \
            DEAL.RECORD$

        READ.INT = 0                                                        !CSWM

    EXIT FUNCTION                                                           !BSWM

    EXIT.FUNCTION:

        CURRENT.REPORT.NUM% = REPORT.NUM%                                   !BSWM
        FILE.OPERATION$ = "R"                                               !BSWM
        CURRENT.CODE$ = DEAL.DEAL.NUM$                                      !BSWM
        
    END FUNCTION

!****************************************************************************

    FUNCTION READ.DEAL PUBLIC                                               !CSWM
    
        INTEGER*1   READ.DEAL                                               !CSWM
        
        SESS.NUM% = DEAL.SESS.NUM%                                          !CSWM
        REPORT.NUM% = DEAL.REPORT.NUM%                                      !CSWM
        READ.DEAL = READ.INT                                                !CSWM
        
    END FUNCTION                                                            !CSWM

!****************************************************************************
    
    FUNCTION READ.DEAL.MODEL PUBLIC                                         !CSWM

        INTEGER*1   READ.DEAL.MODEL                                         !CSWM

        SESS.NUM% = DEAL.MODEL.SESS.NUM%                                    !CSWM
        REPORT.NUM% = DEAL.MODEL.REPORT.NUM%                                !CSWM
        READ.DEAL.MODEL = READ.INT                                          !CSWM
        
    END FUNCTION

!****************************************************************************

    FUNCTION READ.DEAL.BUFFER PUBLIC                                        !BSWM
    
        INTEGER*1   READ.DEAL.BUFFER                                        !BSWM
        
        READ.DEAL.BUFFER = 1                                                !BSWM
        
        IF NOT DEAL.BUFFER THEN BEGIN                                       !BSWM
            DIM DEAL.BUFFER$(9999)                                          !BSWM
            DEAL.BUFFER = -1                                                !BSWM
        ENDIF                                                               !BSWM
        
        DEAL.NUM% = VAL(UNPACK$(DEAL.DEAL.NUM$))                            !BSWM
        IF LEN(DEAL.BUFFER$(DEAL.NUM%)) THEN BEGIN                          !BSWM
            DEAL.RECORD$ = DEAL.BUFFER$(DEAL.NUM%)                          !BSWM
            READ.DEAL.BUFFER = 0                                            !BSWM
        ENDIF ELSE BEGIN                                                    !BSWM
            IF READ.DEAL = 0 THEN BEGIN                                     !BSWM
                DEAL.BUFFER$(DEAL.NUM%) = DEAL.RECORD$                      !BSWM
                READ.DEAL.BUFFER = 0                                        !BSWM
            ENDIF                                                           !BSWM
        ENDIF                                                               !BSWM

    END FUNCTION
    
!****************************************************************************

    FUNCTION ADD.OR.UPDATE.INT (DEAL.NUM$, RECORD$) !DO NOT MAKE PUBLIC     !CSWM

        INTEGER*1   ADD.OR.UPDATE.INT                                       !CSWM
        STRING      DEAL.NUM$
        STRING      RECORD$

        ADD.OR.UPDATE.INT = 1                                               !CSWM

        !Check to see if deal already exists
        DEAL.DEAL.NUM$ = DEAL.NUM$

        !If deal already exists
        IF READ.INT = 0 THEN BEGIN                                          !CSWM

            !Save previous and next items
            PREV.DEAL.NUM$ = DEAL.PREV.DEAL.NUM$
            NEXT.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$

        ENDIF ELSE BEGIN

            PREV.DEAL.NUM$ = HOME.DEAL.NUM$
            NEXT.DEAL.NUM$ = HOME.DEAL.NUM$

            !Read home record
            DEAL.DEAL.NUM$ = HOME.DEAL.NUM$
            IF READ.INT = 0 THEN BEGIN                                      !CSWM

                !Save next record pointer
                NEXT.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$

                !Read first record in chain
                DEAL.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$
                IF READ.INT THEN EXIT FUNCTION                              !CSWM

                !If already writen record and home record
                IF NEXT.DEAL.NUM$ = DEAL.NUM$ THEN BEGIN
                    NEXT.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$
                ENDIF ELSE BEGIN
                    DEAL.PREV.DEAL.NUM$ = DEAL.NUM$
                    IF WRITE.INT THEN EXIT FUNCTION                         !CSWM
                ENDIF

            ENDIF

        ENDIF

        !Write the actual record
        DEAL.DEAL.NUM$ = DEAL.NUM$
        DEAL.PREV.DEAL.NUM$ = PREV.DEAL.NUM$
        DEAL.NEXT.DEAL.NUM$ = NEXT.DEAL.NUM$
        DEAL.RECORD$ = RECORD$
        IF WRITE.INT THEN EXIT FUNCTION                                     !CSWM

        !Need to write home record?
        IF PREV.DEAL.NUM$ = HOME.DEAL.NUM$ THEN BEGIN

            DEAL.DEAL.NUM$ = HOME.DEAL.NUM$
            DEAL.PREV.DEAL.NUM$ = HOME.DEAL.NUM$
            DEAL.NEXT.DEAL.NUM$ = DEAL.NUM$
            DEAL.RECORD$ = NULL.DEAL.RECORD$
            IF WRITE.INT THEN EXIT FUNCTION                                 !CSWM

        ENDIF

        IF DEAL.BUFFER AND SESS.NUM% = DEAL.SESS.NUM% THEN BEGIN            !BSWM
            DEAL.NUM% = VAL(UNPACK$(DEAL.NUM$))                             !BSWM
            DEAL.BUFFER$(DEAL.NUM%) = RECORD$                               !BSWM
        ENDIF                                                               !BSWM

        DEAL.DEAL.NUM$ = DEAL.NUM$ ! ENSURE GLOBAL IS UNAFFECTED

        ADD.OR.UPDATE.INT = 0                                               !CSWM

    END FUNCTION

!****************************************************************************

    FUNCTION ADD.OR.UPDATE.DEAL (DEAL.NUM$, RECORD$) PUBLIC                 !CSWM

        INTEGER*1   ADD.OR.UPDATE.DEAL                                      !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        STRING      RECORD$                                                 !CSWM
        
        SESS.NUM% = DEAL.SESS.NUM%                                          !CSWM
        REPORT.NUM% = DEAL.REPORT.NUM%                                      !CSWM
        ADD.OR.UPDATE.DEAL = ADD.OR.UPDATE.INT (DEAL.NUM$, RECORD$)         !CSWM
        
    END FUNCTION                                                            !CSWM
    
!****************************************************************************

    FUNCTION ADD.OR.UPDATE.DEAL.MODEL (DEAL.NUM$, RECORD$) PUBLIC           !CSWM

        INTEGER*1   ADD.OR.UPDATE.DEAL.MODEL                                !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        STRING      RECORD$                                                 !CSWM
        
        SESS.NUM% = DEAL.MODEL.SESS.NUM%                                    !CSWM
        REPORT.NUM% = DEAL.MODEL.REPORT.NUM%                                !CSWM
        ADD.OR.UPDATE.DEAL.MODEL = ADD.OR.UPDATE.INT (DEAL.NUM$, RECORD$)   !CSWM
        
    END FUNCTION                                                            !CSWM
    
!****************************************************************************

    FUNCTION REMOVE.INT (DEAL.NUM$)    !DO NOT MAKE PUBLIC                  !CSWM

        INTEGER*1   REMOVE.INT                                              !CSWM
        STRING      DEAL.NUM$

        REMOVE.INT = 1                                                      !CSWM

        !Read existing deal record
        DEAL.DEAL.NUM$ = DEAL.NUM$
        IF READ.INT THEN GOTO ALREADY.REMOVED                               !CSWM

        !Save previous and next records
        PREV.DEAL.NUM$ = DEAL.PREV.DEAL.NUM$
        NEXT.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$

        !Update previous record
        DEAL.DEAL.NUM$ = PREV.DEAL.NUM$
        IF READ.INT THEN EXIT FUNCTION                                      !CSWM
        DEAL.NEXT.DEAL.NUM$ = NEXT.DEAL.NUM$
        IF WRITE.INT THEN EXIT FUNCTION                                     !CSWM

        !Update next record
        DEAL.DEAL.NUM$ = NEXT.DEAL.NUM$
        IF READ.INT THEN EXIT FUNCTION                                      !CSWM
        DEAL.PREV.DEAL.NUM$ = PREV.DEAL.NUM$
        IF WRITE.INT THEN EXIT FUNCTION                                     !CSWM

        !Delete the actual record
        DEAL.DEAL.NUM$ = DEAL.NUM$
        IF DELETE.INT THEN EXIT FUNCTION                                    !CSWM

        IF DEAL.BUFFER AND SESS.NUM% = DEAL.SESS.NUM% THEN BEGIN            !BSWM
            DEAL.NUM% = VAL(UNPACK$(DEAL.NUM$))                             !BSWM
            DEAL.BUFFER$(DEAL.NUM%) = ""                                    !BSWM
        ENDIF                                                               !BSWM

    ALREADY.REMOVED:

        REMOVE.INT = 0                                                      !CSWM

    END FUNCTION

!****************************************************************************

    FUNCTION REMOVE.DEAL (DEAL.NUM$) PUBLIC                                 !CSWM

        INTEGER*1   REMOVE.DEAL                                             !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        
        SESS.NUM% = DEAL.SESS.NUM%                                          !CSWM
        REPORT.NUM% = DEAL.REPORT.NUM%                                      !CSWM
        REMOVE.DEAL = REMOVE.INT (DEAL.NUM$)                                !CSWM
        
    END FUNCTION                                                            !CSWM
    
!****************************************************************************

    FUNCTION REMOVE.DEAL.MODEL (DEAL.NUM$) PUBLIC                           !CSWM

        INTEGER*1   REMOVE.DEAL.MODEL                                       !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        
        SESS.NUM% = DEAL.MODEL.SESS.NUM%                                    !CSWM
        REPORT.NUM% = DEAL.MODEL.REPORT.NUM%                                !CSWM
        REMOVE.DEAL.MODEL = REMOVE.INT (DEAL.NUM$)                          !CSWM

    END FUNCTION                                                            !CSWM
    
!****************************************************************************

    FUNCTION GET.NEXT.INT (DEAL.NUM$)   !DO NOT MAKE PUBLIC                 !CSWM

        INTEGER*1   GET.NEXT.INT                                            !CSWM
        STRING      DEAL.NUM$

        GET.NEXT.INT = 1                                                    !CSWM

        DEAL.DEAL.NUM$ = DEAL.NUM$
        IF READ.INT THEN EXIT FUNCTION                                      !CSWM

        DEAL.DEAL.NUM$ = DEAL.NEXT.DEAL.NUM$
        IF DEAL.DEAL.NUM$ = HOME.DEAL.NUM$ THEN EXIT FUNCTION
        IF READ.INT THEN EXIT FUNCTION                                      !CSWM

        GET.NEXT.INT = 0

     END FUNCTION

!****************************************************************************

    FUNCTION GET.NEXT.DEAL (DEAL.NUM$) PUBLIC                               !CSWM
    
        INTEGER*1   GET.NEXT.DEAL                                           !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        
        SESS.NUM% = DEAL.SESS.NUM%                                          !CSWM
        REPORT.NUM% = DEAL.REPORT.NUM%                                      !CSWM
        GET.NEXT.DEAL = GET.NEXT.INT (DEAL.NUM$)                            !CSWM
        
     END FUNCTION                                                           !CSWM

!****************************************************************************

    FUNCTION GET.NEXT.DEAL.MODEL (DEAL.NUM$) PUBLIC                         !CSWM
    
        INTEGER*1   GET.NEXT.DEAL.MODEL                                     !CSWM
        STRING      DEAL.NUM$                                               !CSWM
        
        SESS.NUM% = DEAL.MODEL.SESS.NUM%                                    !CSWM
        REPORT.NUM% = DEAL.MODEL.REPORT.NUM%                                !CSWM
        GET.NEXT.DEAL.MODEL = GET.NEXT.INT (DEAL.NUM$)                      !CSWM
        
     END FUNCTION                                                           !CSWM

!****************************************************************************

    FUNCTION GET.FIRST.DEAL PUBLIC                                          !CSWM

        INTEGER*1   GET.FIRST.DEAL                                          !CSWM

        GET.FIRST.DEAL = GET.NEXT.DEAL (HOME.DEAL.NUM$)                     !CSWM

    END FUNCTION                                                            !CSWM

!****************************************************************************

    FUNCTION GET.FIRST.DEAL.MODEL PUBLIC                                    !CSWM

        INTEGER*1   GET.FIRST.DEAL.MODEL                                    !CSWM

        GET.FIRST.DEAL.MODEL = GET.NEXT.DEAL.MODEL (HOME.DEAL.NUM$)         !CSWM

    END FUNCTION                                                            !CSWM

!****************************************************************************

    SUB FLUSH.DEAL.BUFFER PUBLIC

        IF DEAL.BUFFER THEN BEGIN
            DIM DEAL.BUFFER$(0)
            DEAL.BUFFER = 0
        ENDIF
            
    END SUB
    
!****************************************************************************

