!****************************************************************************
!
!       MODULE:         DEAL ITEM INFORMATION FILE FUNCTIONS
!
!       AUTHOR:         STUART WILLIAM MCCONNACHIE
!
!       DATE:           AUGUST 2002
!
!****************************************************************************
!
!       VERSION A           STUART WILLIAM MCCONNACHIE          AUG 2002
!
!       Functions for adding and removing items from the new format DINF
!       file for the flexible deals project.
!       Note the DINF file is actually two tables each of which is made
!       up of two double linked list of all deal/item combinations on the
!       system.  For details see the DINF file layout.
!
!       VERSION B           STUART WILLIAM MCCONNACHIE          OCT 2003
!
!       Increase performance by providing a batch add record mode for use
!       during deal initial loads.  During batch adds the previous record
!       backward chain pointers are not updated.  Instead these are
!       corrected once the update is complete.  Hence the number of reads
!       and writes to the DINF is reduced for each item add.
!       
!****************************************************************************

%INCLUDE DINFDEC.J86

    STRING GLOBAL       FILE.OPERATION$
    STRING GLOBAL       CURRENT.CODE$
    INTEGER*2 GLOBAL    CURRENT.REPORT.NUM%


    STRING      PREV.DEAL.NUM$
    STRING      NEXT.DEAL.NUM$
    STRING      PREV.BOOTS.CODE$
    STRING      NEXT.BOOTS.CODE$

    STRING      HOME.DEAL.NUM$
    STRING      HOME.BOOTS.CODE$
    STRING      HOME.PREV.DEAL.NUM$
    STRING      HOME.NEXT.DEAL.NUM$
    STRING      HOME.PREV.BOOTS.CODE$
    STRING      HOME.NEXT.BOOTS.CODE$

    INTEGER*1   DINF.INTEGRITY.ACTION%
    STRING      DINF.INTEGRITY.DEAL.NUM$
    STRING      DINF.INTEGRITY.BOOTS.CODE$
    STRING      DINF.INTEGRITY.RECORD$
    INTEGER*1   DINF.INTEGRITY.BATCH
    STRING      DINF.INTEGRITY.FILLER$
    STRING      DINF.INTEGRITY.EMPTY$

    STRING      DINF.KEY$
    STRING      DINF.INTEGRITY.KEY$
    STRING      NULL.DEAL.NUM$
    STRING      NULL.BOOTS.CODE$
    STRING      HOME.LOP.DEAL.NUM$
    STRING      HOME.HOP.DEAL.NUM$
    STRING      HOME.LOP.BOOTS.CODE$
    STRING      HOME.HOP.BOOTS.CODE$
    STRING      NULL.RECORD$

    INTEGER*1   ACTION.ADD%
    INTEGER*1   ACTION.REMOVE%
    INTEGER*1   BATCH.ADD                                                   !BSWM

    INTEGER*2   I%
    
!******************************************************************************

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

!******************************************************************************

    FUNCTION DINF.SET PUBLIC

        INTEGER*1   DINF.SET

        DINF.SET = 1

        DINF.REPORT.NUM%     = 651
        DINF.RECL%           = 22
        DINF.FILE.NAME$      = "DINF"

        NULL.DEAL.NUM$       = PACK$("0000")
        NULL.BOOTS.CODE$     = PACK$("000000")

        HOME.LOP.DEAL.NUM$   = PACK$("::::")
        HOME.HOP.DEAL.NUM$   = PACK$(";;;;")
        HOME.LOP.BOOTS.CODE$ = PACK$("::::::")
        HOME.HOP.BOOTS.CODE$ = PACK$(";;;;;;")

        NULL.RECORD$         = STRING$(7, CHR$(00))

        DINF.INTEGRITY.KEY$  = PACK$("??????????")
        DINF.INTEGRITY.EMPTY$= STRING$(17, CHR$(00))

        ACTION.ADD%          = ASC("A")
        ACTION.REMOVE%       = ASC("D")

        DINF.SET = 0

    END FUNCTION

!******************************************************************************

    FUNCTION READ.DINF PUBLIC

        INTEGER*1   READ.DINF

        READ.DINF = 1

        DINF.KEY$ = DINF.DEAL.NUM$ + DINF.BOOTS.CODE$
        IF END # DINF.SESS.NUM% THEN FILE.ERROR
        READ FORM "T6,2C2,2C3,C7"; # DINF.SESS.NUM%                             \
            KEY DINF.KEY$;                                                      \
            DINF.PREV.DEAL.NUM$,                                                \
            DINF.NEXT.DEAL.NUM$,                                                \
            DINF.PREV.BOOTS.CODE$,                                              \
            DINF.NEXT.BOOTS.CODE$,                                              \
            DINF.RECORD$

        READ.DINF = 0

    EXIT FUNCTION

    FILE.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = DINF.REPORT.NUM%
        CURRENT.CODE$       = PACK$("0000") + DINF.KEY$

    END FUNCTION

!******************************************************************************

    FUNCTION WRITE.DINF     !DO NOT MAKE PUBLIC - MUST USE DINF.ADD.DEAL.ITEM

        INTEGER*1   WRITE.DINF

        WRITE.DINF = 1

        DINF.KEY$ = DINF.DEAL.NUM$ + DINF.BOOTS.CODE$
        IF END # DINF.SESS.NUM% THEN FILE.ERROR
        WRITE FORM "C5,2C2,2C3,C7"; # DINF.SESS.NUM%;                           \
            DINF.KEY$,                                                          \
            DINF.PREV.DEAL.NUM$,                                                \
            DINF.NEXT.DEAL.NUM$,                                                \
            DINF.PREV.BOOTS.CODE$,                                              \
            DINF.NEXT.BOOTS.CODE$,                                              \
            DINF.RECORD$

        WRITE.DINF = 0

    EXIT FUNCTION

    FILE.ERROR:

        FILE.OPERATION$     = "W"
        CURRENT.REPORT.NUM% = DINF.REPORT.NUM%
        CURRENT.CODE$       = PACK$("0000") + DINF.KEY$

    END FUNCTION

!******************************************************************************

    FUNCTION WRITE.DINF.HOLD   !DO NOT MAKE PUBLIC - MUST USE DINF.ADD.DEAL.ITEM

        INTEGER*1   WRITE.DINF.HOLD

        WRITE.DINF.HOLD = 1

        DINF.KEY$ = DINF.DEAL.NUM$ + DINF.BOOTS.CODE$
        IF END # DINF.SESS.NUM% THEN FILE.ERROR
        WRITE FORM "C5,2C2,2C3,C7"; HOLD # DINF.SESS.NUM%;                      \
            DINF.KEY$,                                                          \
            DINF.PREV.DEAL.NUM$,                                                \
            DINF.NEXT.DEAL.NUM$,                                                \
            DINF.PREV.BOOTS.CODE$,                                              \
            DINF.NEXT.BOOTS.CODE$,                                              \
            DINF.RECORD$

        WRITE.DINF.HOLD = 0

    EXIT FUNCTION

    FILE.ERROR:

        FILE.OPERATION$     = "W"
        CURRENT.REPORT.NUM% = DINF.REPORT.NUM%
        CURRENT.CODE$       = PACK$("0000") + DINF.KEY$

    END FUNCTION

!******************************************************************************

    FUNCTION DELETE.DINF    !DO NOT MAKE PUBLIC - MUST USE DINF.REMOVE.DEAL.ITEM

        INTEGER*1   DELETE.DINF

        DELETE.DINF = 1

        DINF.KEY$ = DINF.DEAL.NUM$ + DINF.BOOTS.CODE$
        IF END # DINF.SESS.NUM% THEN FILE.ERROR
        DELREC DINF.SESS.NUM%; DINF.KEY$

        DELETE.DINF = 0

    EXIT FUNCTION

    FILE.ERROR:

        FILE.OPERATION$     = "D"
        CURRENT.REPORT.NUM% = DINF.REPORT.NUM%
        CURRENT.CODE$       = PACK$("0000") + DINF.KEY$

    END FUNCTION

!******************************************************************************

    SUB DINF.CONCAT.RECORD PUBLIC

        DINF.RECORD$ = CHR$(DINF.LOCAL.PRICE.FLAG%) +                   \
                       CHR$(DINF.DEAL.LIST.ID%)  +                      \
                       DINF.FILLER$

    END SUB

!****************************************************************************

    SUB DINF.SPLIT.RECORD PUBLIC
    
        IF LEN(DINF.RECORD$) <> DINF.RECL% - 15 THEN EXIT SUB

        DINF.LOCAL.PRICE.FLAG% = ASC(MID$(DINF.RECORD$, 1, 1))
        DINF.DEAL.LIST.ID%     = ASC(MID$(DINF.RECORD$, 2, 1))
        DINF.FILLER$           = MID$(DINF.RECORD$, 3, 5)

    END SUB

!****************************************************************************

    SUB DINF.SAVE (SAVE$) PUBLIC                                            !BSWM
    
        STRING      SAVE$                                                   !BSWM
        
        SAVE$ = DINF.DEAL.NUM$ +                                            \BSWM
                DINF.BOOTS.CODE$ +                                          \BSWM
                DINF.PREV.DEAL.NUM$ +                                       \BSWM
                DINF.NEXT.DEAL.NUM$ +                                       \BSWM
                DINF.PREV.BOOTS.CODE$ +                                     \BSWM
                DINF.NEXT.BOOTS.CODE$ +                                     \BSWM
                DINF.RECORD$                                                !BSWM
                
    END SUB                                                                 !BSWM

!****************************************************************************

    SUB DINF.RESTORE (RESTORE$) PUBLIC                                      !BSWM
    
        STRING      RESTORE$                                                !BSWM
        
        DINF.DEAL.NUM$          = MID$(RESTORE$,1,2)                        !BSWM
        DINF.BOOTS.CODE$        = MID$(RESTORE$,3,3)                        !BSWM
        DINF.PREV.DEAL.NUM$     = MID$(RESTORE$,6,2)                        !BSWM
        DINF.NEXT.DEAL.NUM$     = MID$(RESTORE$,8,2)                        !BSWM
        DINF.PREV.BOOTS.CODE$   = MID$(RESTORE$,10,3)                       !BSWM
        DINF.NEXT.BOOTS.CODE$   = MID$(RESTORE$,13,3)                       !BSWM
        DINF.RECORD$            = MID$(RESTORE$,16,7)                       !BSWM
        CALL DINF.SPLIT.RECORD                                              !BSWM
        
    END SUB                                                                 !BSWM
    
!****************************************************************************

    FUNCTION DINF.REMOVE (DEAL.NUM$, BOOTS.CODE$)   !DO NOT MAKE PUBLIC

        INTEGER*1   DINF.REMOVE
        STRING      DEAL.NUM$
        STRING      BOOTS.CODE$

        DINF.REMOVE = 1

        !Read the record for the deal+item to delete
        DINF.DEAL.NUM$ = DEAL.NUM$
        DINF.BOOTS.CODE$ = BOOTS.CODE$
        IF READ.DINF THEN GOTO REMOVED.OKAY

        !Set up home keys
        IF ASC(LEFT$(DINF.RECORD$,1)) THEN BEGIN
           HOME.DEAL.NUM$ = HOME.LOP.DEAL.NUM$
           HOME.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$
        ENDIF ELSE BEGIN
           HOME.DEAL.NUM$ = HOME.HOP.DEAL.NUM$
           HOME.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$
        ENDIF

        !Save pointers
        PREV.DEAL.NUM$ = DINF.PREV.DEAL.NUM$
        NEXT.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
        PREV.BOOTS.CODE$ = DINF.PREV.BOOTS.CODE$
        NEXT.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$

        !Need to correct back chain pointer from next deal
        IF NEXT.DEAL.NUM$ <> NULL.DEAL.NUM$ THEN BEGIN
            !Correct back pointer
            DINF.DEAL.NUM$ = NEXT.DEAL.NUM$
            DINF.BOOTS.CODE$ = BOOTS.CODE$
            IF READ.DINF THEN EXIT FUNCTION
            DINF.PREV.DEAL.NUM$ = PREV.DEAL.NUM$
            IF WRITE.DINF THEN EXIT FUNCTION
        ENDIF

        !Need to correct back chain pointer from next item
        IF NEXT.BOOTS.CODE$ <> NULL.BOOTS.CODE$ THEN BEGIN
            !Correct back pointer
            DINF.DEAL.NUM$ = DEAL.NUM$
            DINF.BOOTS.CODE$ = NEXT.BOOTS.CODE$
            IF READ.DINF THEN EXIT FUNCTION
            DINF.PREV.BOOTS.CODE$ = PREV.BOOTS.CODE$
            IF WRITE.DINF THEN EXIT FUNCTION
        ENDIF

        !Need to remove record from home chain?
        IF PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$ AND                          \
           NEXT.BOOTS.CODE$ = NULL.BOOTS.CODE$ THEN BEGIN
            !Remove home record
            DINF.DEAL.NUM$ = DEAL.NUM$
            DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
            !Home record not already removed
            IF READ.DINF = 0 THEN BEGIN
                HOME.PREV.DEAL.NUM$ = DINF.PREV.DEAL.NUM$
                HOME.NEXT.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
                !Need to correct back pointer
                IF HOME.NEXT.DEAL.NUM$ <> NULL.DEAL.NUM$ THEN BEGIN
                    DINF.DEAL.NUM$ = HOME.NEXT.DEAL.NUM$
                    DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                    IF READ.DINF THEN EXIT FUNCTION
                    DINF.PREV.DEAL.NUM$ = HOME.PREV.DEAL.NUM$
                    IF WRITE.DINF THEN EXIT FUNCTION
                ENDIF
                !Correct forward pointer
                DINF.DEAL.NUM$ = HOME.PREV.DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                IF READ.DINF THEN EXIT FUNCTION
                DINF.NEXT.DEAL.NUM$ = HOME.NEXT.DEAL.NUM$
                IF WRITE.DINF THEN EXIT FUNCTION
                !Actually remove the home record
                DINF.DEAL.NUM$ = DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                IF DELETE.DINF THEN EXIT FUNCTION
            ENDIF
        !Else need to correct forward chain pointer from prev record
        ENDIF ELSE BEGIN
            !Correct forward pointer
            DINF.DEAL.NUM$ = DEAL.NUM$
            DINF.BOOTS.CODE$ = PREV.BOOTS.CODE$
            IF READ.DINF THEN EXIT FUNCTION
            DINF.NEXT.BOOTS.CODE$ = NEXT.BOOTS.CODE$
            IF WRITE.DINF THEN EXIT FUNCTION
        ENDIF

        !Need to remove record from home chain?
        IF PREV.DEAL.NUM$ = HOME.DEAL.NUM$ AND                              \
           NEXT.DEAL.NUM$ = NULL.DEAL.NUM$ THEN BEGIN
            !Remove home record
            DINF.DEAL.NUM$ = HOME.DEAL.NUM$
            DINF.BOOTS.CODE$ = BOOTS.CODE$
            !Home record not already removed
            IF READ.DINF = 0 THEN BEGIN
                HOME.PREV.BOOTS.CODE$ = DINF.PREV.BOOTS.CODE$
                HOME.NEXT.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$
                !Need to correct back pointer
                IF HOME.NEXT.BOOTS.CODE$ <> NULL.BOOTS.CODE$ THEN BEGIN
                    DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                    DINF.BOOTS.CODE$ = HOME.NEXT.BOOTS.CODE$
                    IF READ.DINF THEN EXIT FUNCTION
                    DINF.PREV.BOOTS.CODE$ = HOME.PREV.BOOTS.CODE$
                    IF WRITE.DINF THEN EXIT FUNCTION
                ENDIF
                !Correct forward pointer
                DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.PREV.BOOTS.CODE$
                IF READ.DINF THEN EXIT FUNCTION
                DINF.NEXT.BOOTS.CODE$ = HOME.NEXT.BOOTS.CODE$
                IF WRITE.DINF THEN EXIT FUNCTION
                !Actually remove the home record
                DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.BOOTS.CODE$ = BOOTS.CODE$
                IF DELETE.DINF THEN EXIT FUNCTION
            ENDIF
        !Else need to correct forward chain pointer from prev record
        ENDIF ELSE BEGIN
            !Correct forward pointer
            DINF.DEAL.NUM$ = PREV.DEAL.NUM$
            DINF.BOOTS.CODE$ = BOOTS.CODE$
            IF READ.DINF THEN EXIT FUNCTION
            DINF.NEXT.DEAL.NUM$ = NEXT.DEAL.NUM$
            IF WRITE.DINF THEN EXIT FUNCTION
        ENDIF

        DINF.DEAL.NUM$ = DEAL.NUM$
        DINF.BOOTS.CODE$ = BOOTS.CODE$
        IF DELETE.DINF THEN EXIT FUNCTION

    REMOVED.OKAY:

        DINF.REMOVE = 0

    EXIT FUNCTION

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.ADD (DEAL.NUM$, BOOTS.CODE$, RECORD$) !DO NOT MAKE PUBLIC

        INTEGER*1   DINF.ADD
        STRING      DEAL.NUM$
        STRING      BOOTS.CODE$
        STRING      RECORD$

        DINF.ADD = 1

        !Read the record for this deal+item to see if it exists
        DINF.DEAL.NUM$ = DEAL.NUM$
        DINF.BOOTS.CODE$ = BOOTS.CODE$

        !If record already on file
        IF READ.DINF = 0 THEN BEGIN

            GOSUB GET.HOME.KEYS
            GOSUB SAVE.POINTERS

            !Old and new records not both on local or head office price?
            IF LEFT$(RECORD$,1) <> LEFT$(DINF.RECORD$,1) THEN BEGIN
            
                IF BATCH.ADD THEN EXIT FUNCTION                             !BSWM
            
                !Need to remove old record from file first
                IF DINF.REMOVE (DEAL.NUM$, BOOTS.CODE$) THEN EXIT FUNCTION
                GOSUB INSERT.CHAIN
                
            ENDIF

        !Item not yet on file
        ENDIF ELSE BEGIN

            GOSUB INSERT.CHAIN

        ENDIF

        GOSUB WRITE.RECORD
        GOSUB UPDATE.HOME.POINTERS

        DINF.ADD = 0

    EXIT FUNCTION


    GET.HOME.KEYS:
     IF ASC(LEFT$(DINF.RECORD$,1)) THEN BEGIN
        HOME.DEAL.NUM$ = HOME.LOP.DEAL.NUM$
        HOME.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$
     ENDIF ELSE BEGIN
        HOME.DEAL.NUM$ = HOME.HOP.DEAL.NUM$
        HOME.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$
     ENDIF

    RETURN


    SAVE.POINTERS:

        PREV.DEAL.NUM$ = DINF.PREV.DEAL.NUM$
        NEXT.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
        PREV.BOOTS.CODE$ = DINF.PREV.BOOTS.CODE$
        NEXT.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$

    RETURN


    WRITE.RECORD:

        DINF.DEAL.NUM$ = DEAL.NUM$
        DINF.BOOTS.CODE$ = BOOTS.CODE$
        DINF.PREV.DEAL.NUM$ = PREV.DEAL.NUM$
        DINF.NEXT.DEAL.NUM$ = NEXT.DEAL.NUM$
        DINF.PREV.BOOTS.CODE$ = PREV.BOOTS.CODE$
        DINF.NEXT.BOOTS.CODE$ = NEXT.BOOTS.CODE$
        DINF.RECORD$ = RECORD$
        IF WRITE.DINF THEN EXIT FUNCTION

    RETURN


    INSERT.CHAIN:

        DINF.RECORD$ = RECORD$
        GOSUB GET.HOME.KEYS

        PREV.DEAL.NUM$ = HOME.DEAL.NUM$
        NEXT.DEAL.NUM$ = NULL.DEAL.NUM$
        PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$
        NEXT.BOOTS.CODE$ = NULL.BOOTS.CODE$

        !Read home record for current deal
        DINF.DEAL.NUM$ = DEAL.NUM$
        DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
        !If home record already exists
        IF READ.DINF = 0 THEN BEGIN
            NEXT.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$
        ENDIF

        !Read home record for current item
        DINF.DEAL.NUM$ = HOME.DEAL.NUM$
        DINF.BOOTS.CODE$ = BOOTS.CODE$
        !If home record already exists
        IF READ.DINF = 0 THEN BEGIN
            NEXT.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
        ENDIF

        IF NOT BATCH.ADD THEN BEGIN                                         !BSWM
        
            !Need to adjust back chain pointer
            IF NEXT.DEAL.NUM$ <> NULL.DEAL.NUM$ THEN BEGIN
                DINF.DEAL.NUM$ = NEXT.DEAL.NUM$
                DINF.BOOTS.CODE$ = BOOTS.CODE$
                IF READ.DINF THEN EXIT FUNCTION
                DINF.PREV.DEAL.NUM$ = DEAL.NUM$
                IF WRITE.DINF THEN EXIT FUNCTION
            ENDIF

            !Need to adjust back chain pointer
            IF NEXT.BOOTS.CODE$ <> NULL.BOOTS.CODE$ THEN BEGIN
                DINF.DEAL.NUM$ = DEAL.NUM$
                DINF.BOOTS.CODE$ = NEXT.BOOTS.CODE$
                IF READ.DINF THEN EXIT FUNCTION
                DINF.PREV.BOOTS.CODE$ = BOOTS.CODE$
                IF WRITE.DINF THEN EXIT FUNCTION
            ENDIF

        ENDIF                                                               !BSWM
        
    RETURN


    UPDATE.HOME.POINTERS:

        !Home record needs updating
        IF PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$ THEN BEGIN
            !Read the home record
            DINF.DEAL.NUM$ = DEAL.NUM$
            DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
            !If home record exists
            IF READ.DINF = 0 THEN BEGIN
                !Update home record
                DINF.NEXT.BOOTS.CODE$ = BOOTS.CODE$
                IF WRITE.DINF THEN EXIT FUNCTION
            !Else create home record
            ENDIF ELSE BEGIN
                !Read master record
                DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                !If master record does not exist
                IF READ.DINF <> 0 THEN BEGIN
                    !Create master record
                    DINF.PREV.DEAL.NUM$ = NULL.DEAL.NUM$
                    DINF.NEXT.DEAL.NUM$ = NULL.DEAL.NUM$
                    DINF.PREV.BOOTS.CODE$ = NULL.BOOTS.CODE$
                    DINF.NEXT.BOOTS.CODE$ = NULL.BOOTS.CODE$
                    DINF.RECORD$ = NULL.RECORD$
                ENDIF
                !Update master record
                HOME.NEXT.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
                DINF.NEXT.DEAL.NUM$ = DEAL.NUM$
                IF WRITE.DINF.HOLD THEN EXIT FUNCTION
                !Create home record
                DINF.DEAL.NUM$ = DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                DINF.PREV.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.NEXT.DEAL.NUM$ = HOME.NEXT.DEAL.NUM$
                DINF.PREV.BOOTS.CODE$ = NULL.BOOTS.CODE$
                DINF.NEXT.BOOTS.CODE$ = BOOTS.CODE$
                DINF.RECORD$ = NULL.RECORD$
                IF WRITE.DINF.HOLD THEN EXIT FUNCTION
            ENDIF

            IF NOT BATCH.ADD THEN BEGIN                                     !BSWM
                !If first item in chain
                IF DINF.PREV.DEAL.NUM$ = HOME.DEAL.NUM$ THEN BEGIN
                    !Update backward pointer
                    IF DINF.NEXT.DEAL.NUM$ <> NULL.DEAL.NUM$ THEN BEGIN
                        DINF.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
                        DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                        IF READ.DINF THEN EXIT FUNCTION
                        DINF.PREV.DEAL.NUM$ = DEAL.NUM$
                        IF WRITE.DINF THEN EXIT FUNCTION
                    ENDIF
                ENDIF
            ENDIF                                                           !BSWM

        ENDIF

        !Home record needs updating
        IF PREV.DEAL.NUM$ = HOME.DEAL.NUM$ THEN BEGIN
            !Read the home record
            DINF.DEAL.NUM$ = HOME.DEAL.NUM$
            DINF.BOOTS.CODE$ = BOOTS.CODE$
            !If home record exists
            IF READ.DINF = 0 THEN BEGIN
                !Update home record
                DINF.NEXT.DEAL.NUM$ = DEAL.NUM$
                IF WRITE.DINF THEN EXIT FUNCTION
            !Else create home record
            ENDIF ELSE BEGIN
                !Read master record
                DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.BOOTS.CODE$ = HOME.BOOTS.CODE$
                !If master record does not exist
                IF READ.DINF <> 0 THEN BEGIN
                    !Create master record
                    DINF.PREV.DEAL.NUM$ = NULL.DEAL.NUM$
                    DINF.NEXT.DEAL.NUM$ = NULL.DEAL.NUM$
                    DINF.PREV.BOOTS.CODE$ = NULL.BOOTS.CODE$
                    DINF.NEXT.BOOTS.CODE$ = NULL.BOOTS.CODE$
                    DINF.RECORD$ = NULL.RECORD$
                ENDIF
                !Update master record
                HOME.NEXT.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$
                DINF.NEXT.BOOTS.CODE$ = BOOTS.CODE$
                IF WRITE.DINF.HOLD THEN EXIT FUNCTION
                !Create home record
                DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                DINF.BOOTS.CODE$ = BOOTS.CODE$
                DINF.PREV.DEAL.NUM$ = NULL.DEAL.NUM$
                DINF.NEXT.DEAL.NUM$ = DEAL.NUM$
                DINF.PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$
                DINF.NEXT.BOOTS.CODE$ = HOME.NEXT.BOOTS.CODE$
                DINF.RECORD$ = NULL.RECORD$
                IF WRITE.DINF.HOLD THEN EXIT FUNCTION
            ENDIF

            IF NOT BATCH.ADD THEN BEGIN                                     !BSWM
                !If first item in chain
                IF DINF.PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$ THEN BEGIN
                    !Update backward pointer
                    IF DINF.NEXT.BOOTS.CODE$ <> NULL.BOOTS.CODE$ THEN BEGIN
                        DINF.DEAL.NUM$ = HOME.DEAL.NUM$
                        DINF.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$
                        IF READ.DINF THEN EXIT FUNCTION
                        DINF.PREV.BOOTS.CODE$ = BOOTS.CODE$
                        IF WRITE.DINF THEN EXIT FUNCTION
                    ENDIF
                ENDIF
            ENDIF                                                           !BSWM

        ENDIF

    RETURN


    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.NEXT.ITEM PUBLIC

        INTEGER*1   DINF.GET.NEXT.ITEM

        DINF.GET.NEXT.ITEM = 1

        DINF.BOOTS.CODE$ = DINF.NEXT.BOOTS.CODE$
        IF DINF.BOOTS.CODE$ = NULL.BOOTS.CODE$ THEN EXIT FUNCTION

        DINF.GET.NEXT.ITEM = READ.DINF

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.PREV.ITEM PUBLIC

        INTEGER*1   DINF.GET.PREV.ITEM

        DINF.GET.PREV.ITEM = 1

        DINF.BOOTS.CODE$ = DINF.PREV.BOOTS.CODE$
        IF DINF.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$ OR \
           DINF.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$ THEN EXIT FUNCTION

        DINF.GET.PREV.ITEM = READ.DINF

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.FIRST.ITEM.DEAL (DEAL.NUM$,LOCAL.PRICE%) PUBLIC

        INTEGER*1   DINF.GET.FIRST.ITEM.DEAL
        STRING      DEAL.NUM$
        INTEGER*1   LOCAL.PRICE%

        DINF.GET.FIRST.ITEM.DEAL = 1

        DINF.DEAL.NUM$ = DEAL.NUM$
        IF LOCAL.PRICE% THEN BEGIN
           DINF.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$
        ENDIF ELSE BEGIN
           DINF.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$
        ENDIF

        IF READ.DINF THEN EXIT FUNCTION

        DINF.GET.FIRST.ITEM.DEAL = DINF.GET.NEXT.ITEM

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.FIRST.ITEM(LOCAL.PRICE%) PUBLIC

        INTEGER*1   DINF.GET.FIRST.ITEM
        INTEGER*1   LOCAL.PRICE%

        DINF.GET.FIRST.ITEM = 1

        IF LOCAL.PRICE% THEN BEGIN
           DINF.DEAL.NUM$ = HOME.LOP.DEAL.NUM$
           DINF.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$
        ENDIF ELSE BEGIN
           DINF.DEAL.NUM$ = HOME.HOP.DEAL.NUM$
           DINF.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$
        ENDIF

        IF READ.DINF THEN EXIT FUNCTION

        DINF.GET.FIRST.ITEM = DINF.GET.NEXT.ITEM

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.NEXT.DEAL PUBLIC

        INTEGER*1   DINF.GET.NEXT.DEAL

        DINF.GET.NEXT.DEAL = 1

        DINF.DEAL.NUM$ = DINF.NEXT.DEAL.NUM$
        IF DINF.DEAL.NUM$ = NULL.DEAL.NUM$ THEN EXIT FUNCTION

        DINF.GET.NEXT.DEAL = READ.DINF

    END FUNCTION

!******************************************************************************
    
    FUNCTION DINF.GET.PREV.DEAL PUBLIC

        INTEGER*1   DINF.GET.PREV.DEAL

        DINF.GET.PREV.DEAL = 1

        DINF.DEAL.NUM$ = DINF.PREV.DEAL.NUM$
        IF DINF.DEAL.NUM$ = HOME.HOP.DEAL.NUM$ OR \
           DINF.DEAL.NUM$ = HOME.LOP.DEAL.NUM$ THEN EXIT FUNCTION

        DINF.GET.PREV.DEAL = READ.DINF

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.FIRST.DEAL.ITEM (BOOTS.CODE$,LOCAL.PRICE%) PUBLIC

        INTEGER*1   DINF.GET.FIRST.DEAL.ITEM
        STRING      BOOTS.CODE$
        INTEGER*1   LOCAL.PRICE%

        DINF.GET.FIRST.DEAL.ITEM = 1

        DINF.BOOTS.CODE$ = BOOTS.CODE$
        IF LOCAL.PRICE% THEN BEGIN
           DINF.DEAL.NUM$ = HOME.LOP.DEAL.NUM$
        ENDIF ELSE BEGIN
           DINF.DEAL.NUM$ = HOME.HOP.DEAL.NUM$
        ENDIF

        IF READ.DINF THEN EXIT FUNCTION

        DINF.GET.FIRST.DEAL.ITEM = DINF.GET.NEXT.DEAL

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.GET.FIRST.DEAL (LOCAL.PRICE%) PUBLIC

        INTEGER*1   DINF.GET.FIRST.DEAL
        INTEGER*1   LOCAL.PRICE%

        DINF.GET.FIRST.DEAL = 1

        IF LOCAL.PRICE% THEN BEGIN
           DINF.DEAL.NUM$ = HOME.LOP.DEAL.NUM$
           DINF.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$
        ENDIF ELSE BEGIN
           DINF.DEAL.NUM$ = HOME.HOP.DEAL.NUM$
           DINF.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$
        ENDIF

        IF READ.DINF THEN EXIT FUNCTION

        DINF.GET.FIRST.DEAL = DINF.GET.NEXT.DEAL

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.CORRECT.CHAIN                                             !BSWM
    
        INTEGER*1   DINF.CORRECT.CHAIN                                      !BSWM
        INTEGER*1   PRICE%                                                  !BSWM
        INTEGER*1   RC%                                                     !BSWM
        STRING      STATE$                                                  !BSWM
        STRING      PREV$                                                   !BSWM
    
        DINF.CORRECT.CHAIN = 1                                              !BSWM
    
        FOR PRICE% = 0 TO 1                                                 !BSWM
    
            IF PRICE% THEN BEGIN                                            !BSWM
                HOME.DEAL.NUM$ = HOME.LOP.DEAL.NUM$                         !BSWM
            ENDIF ELSE BEGIN                                                !BSWM
                HOME.DEAL.NUM$ = HOME.HOP.DEAL.NUM$                         !BSWM
            ENDIF                                                           !BSWM
            
            RC% = DINF.GET.FIRST.DEAL (PRICE%)                              !BSWM
            WHILE RC% = 0                                                   !BSWM
                
                IF DINF.PREV.DEAL.NUM$ <> HOME.DEAL.NUM$ THEN BEGIN         !BSWM
                    DINF.PREV.DEAL.NUM$ = HOME.DEAL.NUM$                    !BSWM
                    IF WRITE.DINF THEN EXIT FUNCTION                        !BSWM
                ENDIF                                                       !BSWM
                
                CALL DINF.SAVE (STATE$)                                     !BSWM
                HOME.BOOTS.CODE$ = DINF.BOOTS.CODE$                         !BSWM
                PREV$ = DINF.BOOTS.CODE$                                    !BSWM
                
                RC% = DINF.GET.FIRST.ITEM.DEAL (DINF.DEAL.NUM$, PRICE%)     !BSWM
                WHILE RC% = 0                                               !BSWM
                    IF DINF.PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$ THEN BEGIN  !BSWM
                        DINF.PREV.BOOTS.CODE$ = PREV$                       !BSWM
                        IF WRITE.DINF THEN EXIT FUNCTION                    !BSWM
                    ENDIF                                                   !BSWM
                    PREV$ = DINF.BOOTS.CODE$                                !BSWM
                    RC% = DINF.GET.NEXT.ITEM                                !BSWM
                WEND                                                        !BSWM
                
                CALL DINF.RESTORE (STATE$)                                  !BSWM
                HOME.DEAL.NUM$ = DINF.DEAL.NUM$                             !BSWM
                RC% = DINF.GET.NEXT.DEAL                                    !BSWM
                
            WEND                                                            !BSWM
        
            IF PRICE% THEN BEGIN                                            !BSWM
                HOME.BOOTS.CODE$ = HOME.LOP.BOOTS.CODE$                     !BSWM
            ENDIF ELSE BEGIN                                                !BSWM
                HOME.BOOTS.CODE$ = HOME.HOP.BOOTS.CODE$                     !BSWM
            ENDIF                                                           !BSWM
            
            RC% = DINF.GET.FIRST.ITEM (PRICE%)                              !BSWM
            WHILE RC% = 0                                                   !BSWM
            
                IF DINF.PREV.BOOTS.CODE$ <> HOME.BOOTS.CODE$ THEN BEGIN     !BSWM
                    DINF.PREV.BOOTS.CODE$ = HOME.BOOTS.CODE$                !BSWM
                    IF WRITE.DINF THEN EXIT FUNCTION                        !BSWM
                ENDIF                                                       !BSWM
                
                CALL DINF.SAVE (STATE$)                                     !BSWM
                HOME.DEAL.NUM$ = DINF.DEAL.NUM$                             !BSWM
                PREV$ = DINF.DEAL.NUM$                                      !BSWM
                
                RC% = DINF.GET.FIRST.DEAL.ITEM (DINF.BOOTS.CODE$, PRICE%)   !BSWM
                WHILE RC% = 0                                               !BSWM
                    IF DINF.PREV.DEAL.NUM$ = HOME.DEAL.NUM$ THEN BEGIN      !BSWM
                        DINF.PREV.DEAL.NUM$ = PREV$                         !BSWM
                        IF WRITE.DINF THEN EXIT FUNCTION                    !BSWM
                    ENDIF                                                   !BSWM
                    PREV$ = DINF.DEAL.NUM$                                  !BSWM
                    RC% = DINF.GET.NEXT.DEAL                                !BSWM
                WEND                                                        !BSWM
                
                CALL DINF.RESTORE (STATE$)                                  !BSWM
                HOME.BOOTS.CODE$ = DINF.BOOTS.CODE$                         !BSWM
                RC% = DINF.GET.NEXT.ITEM                                    !BSWM
                
            WEND                                                            !BSWM
        
        NEXT PRICE%                                                         !BSWM
        
        DINF.CORRECT.CHAIN = 0                                              !BSWM
    
    END FUNCTION                                                            !BSWM
    
!******************************************************************************

    FUNCTION SET.INTEGRITY.LOCK (ACTION%, DEAL.NUM$, BOOTS.CODE$, RECORD$, BATCH%)   !BSWM

        INTEGER*1   SET.INTEGRITY.LOCK
        INTEGER*1   INTEGRITY.ERROR
        INTEGER*1   ACTION%
        STRING      DEAL.NUM$
        STRING      BOOTS.CODE$
        STRING      RECORD$
        INTEGER*1   BATCH%

        SET.INTEGRITY.LOCK = 1
        
        IF BATCH.ADD THEN BEGIN
            BATCH% = -1
            GOTO BATCH.UPDATE
        ENDIF

    RETRY.LOCK:

        IF END # DINF.SESS.NUM% THEN MISSING.INTEGRITY.RECORD
        READ FORM "T6,I1,C2,C3,C7,I1,C3"; # DINF.SESS.NUM%                  \BSWM
            AUTOLOCK KEY DINF.INTEGRITY.KEY$;                               \
            DINF.INTEGRITY.ACTION%,                                         \
            DINF.INTEGRITY.DEAL.NUM$,                                       \
            DINF.INTEGRITY.BOOTS.CODE$,                                     \
            DINF.INTEGRITY.RECORD$,                                         \
            DINF.INTEGRITY.BATCH,                                           \BSWM
            DINF.INTEGRITY.FILLER$

        INTEGRITY.ERROR = 0

        IF DINF.INTEGRITY.ACTION% = ACTION.ADD% THEN BEGIN

            INTEGRITY.ERROR = DINF.ADD(DINF.INTEGRITY.DEAL.NUM$,            \
                DINF.INTEGRITY.BOOTS.CODE$,                                 \
                DINF.INTEGRITY.RECORD$)

        ENDIF ELSE IF DINF.INTEGRITY.ACTION% = ACTION.REMOVE% THEN BEGIN

            INTEGRITY.ERROR = DINF.REMOVE(DINF.INTEGRITY.DEAL.NUM$,         \
                DINF.INTEGRITY.BOOTS.CODE$)

        ENDIF

        IF INTEGRITY.ERROR = 0 THEN BEGIN                                   !BSWM
            IF DINF.INTEGRITY.BATCH THEN BEGIN                              !BSWM
                INTEGRITY.ERROR = DINF.CORRECT.CHAIN                        !BSWM
            ENDIF                                                           !BSWM
        ENDIF                                                               !BSWM
            
        IF INTEGRITY.ERROR THEN BEGIN

            IF END # DINF.SESS.NUM% THEN FILE.ERROR
            WRITE FORM "C5,I1,C2,C3,C7,I1,C3"; #DINF.SESS.NUM% AUTOUNLOCK;  \BSWM
                DINF.INTEGRITY.KEY$,                                        \
                DINF.INTEGRITY.ACTION%,                                     \
                DINF.INTEGRITY.DEAL.NUM$,                                   \
                DINF.INTEGRITY.BOOTS.CODE$,                                 \
                DINF.INTEGRITY.RECORD$,                                     \
                DINF.INTEGRITY.BATCH,                                       \BSWM
                DINF.INTEGRITY.FILLER$

            FILE.ERROR:

                FILE.OPERATION$     = "W"
                CURRENT.REPORT.NUM% = DINF.REPORT.NUM%
                CURRENT.CODE$       = PACK$("0000") + DINF.INTEGRITY.KEY$

        ENDIF ELSE BEGIN

            BATCH.UPDATE:
            
            IF END # DINF.SESS.NUM% THEN FILE.ERROR
            WRITE FORM "C5,I1,C2,C3,C7,I1,C3"; #DINF.SESS.NUM%;             \BSWM
                DINF.INTEGRITY.KEY$,                                        \
                ACTION%,                                                    \
                DEAL.NUM$,                                                  \
                BOOTS.CODE$,                                                \
                RECORD$,                                                    \
                BATCH%,                                                     \BSWM
                DINF.INTEGRITY.FILLER$

            SET.INTEGRITY.LOCK = 0

        ENDIF

    EXIT FUNCTION


    MISSING.INTEGRITY.RECORD:

        IF END # DINF.SESS.NUM% THEN FILE.ERROR
        WRITE FORM "C5,C17"; # DINF.SESS.NUM%;                                  \
            DINF.INTEGRITY.KEY$,                                                \
            DINF.INTEGRITY.EMPTY$

        GOTO RETRY.LOCK

    END FUNCTION

!******************************************************************************

    FUNCTION RELEASE.INTEGRITY.LOCK

        INTEGER*1   RELEASE.INTEGRITY.LOCK

        ON ERROR GOTO ERROR.DETECTED

        RELEASE.INTEGRITY.LOCK = 1

    RETRY.UNLOCK:

        IF NOT BATCH.ADD THEN BEGIN                                         !BSWM
            IF END # DINF.SESS.NUM% THEN EXIT.FUNCTION
            WRITE FORM "C5,C17"; # DINF.SESS.NUM% AUTOUNLOCK;               \
                DINF.INTEGRITY.KEY$,                                        \
                DINF.INTEGRITY.EMPTY$
        ENDIF                                                               !BSWM

        RELEASE.INTEGRITY.LOCK = 0

    EXIT.FUNCTION:

    EXIT FUNCTION


    UNLOCK.ERROR:

        IF SET.INTEGRITY.LOCK(0, NULL.DEAL.NUM$, NULL.BOOTS.CODE$,          \BSWM
                              NULL.RECORD$, 0) THEN EXIT FUNCTION           !BSWM
        GOTO RETRY.UNLOCK


    ERROR.DETECTED:

        !OS BUG: Get this error if delete of record occurs in same sector
        !as locked integrity record:
        IF ERRN = 080F306C9H THEN BEGIN
            RESUME UNLOCK.ERROR
        ENDIF

        RESUME EXIT.FUNCTION


    END FUNCTION

!******************************************************************************

    FUNCTION DINF.ADD.DEAL.ITEM (DEAL.NUM$, BOOTS.CODE$, RECORD$) PUBLIC

        INTEGER*1   DINF.ADD.DEAL.ITEM
        INTEGER*1   RETURN.CODE
        STRING      DEAL.NUM$
        STRING      BOOTS.CODE$
        STRING      RECORD$

        DINF.ADD.DEAL.ITEM = 1

        IF SET.INTEGRITY.LOCK (ACTION.ADD%, DEAL.NUM$, BOOTS.CODE$,         \BSWM
                               RECORD$, 0) THEN EXIT FUNCTION               !BSWM
        RETURN.CODE = DINF.ADD (DEAL.NUM$, BOOTS.CODE$, RECORD$)
        IF RELEASE.INTEGRITY.LOCK THEN EXIT FUNCTION

        DINF.ADD.DEAL.ITEM = RETURN.CODE

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.REMOVE.DEAL.ITEM (DEAL.NUM$, BOOTS.CODE$) PUBLIC

        INTEGER*1   DINF.REMOVE.DEAL.ITEM
        INTEGER*1   RETURN.CODE
        STRING      DEAL.NUM$
        STRING      BOOTS.CODE$

        DINF.REMOVE.DEAL.ITEM = 1
        IF BATCH.ADD THEN EXIT FUNCTION                                     !BSWM

        IF SET.INTEGRITY.LOCK (ACTION.REMOVE%, DEAL.NUM$, BOOTS.CODE$,      \BSWM
                               NULL.RECORD$, 0) THEN EXIT FUNCTION          !BSWM
        RETURN.CODE = DINF.REMOVE (DEAL.NUM$, BOOTS.CODE$)
        IF RELEASE.INTEGRITY.LOCK THEN EXIT FUNCTION

        DINF.REMOVE.DEAL.ITEM = RETURN.CODE

    END FUNCTION

!******************************************************************************

    FUNCTION DINF.BATCH.ADD.START PUBLIC                                    !BSWM

        INTEGER*1   DINF.BATCH.ADD.START                                    !BSWM
        
        DINF.BATCH.ADD.START = 1                                            !BSWM
        IF NOT BATCH.ADD THEN BEGIN                                         !BSWM
            IF SET.INTEGRITY.LOCK (0, NULL.DEAL.NUM$, NULL.BOOTS.CODE$,     \BSWM
                                   NULL.RECORD$, -1) THEN EXIT FUNCTION     !BSWM
            BATCH.ADD = -1                                                  !BSWM
        ENDIF                                                               !BSWM
        DINF.BATCH.ADD.START = 0                                            !BSWM
        
    END FUNCTION                                                            !BSWM
    
!******************************************************************************

    FUNCTION DINF.BATCH.ADD.END PUBLIC                                      !BSWM
    
        INTEGER*1   DINF.BATCH.ADD.END                                      !BSWM
        
        DINF.BATCH.ADD.END = 1                                              !BSWM
        IF BATCH.ADD THEN BEGIN                                             !BSWM
            IF DINF.CORRECT.CHAIN THEN EXIT FUNCTION                        !BSWM
            BATCH.ADD = 0                                                   !BSWM
            IF RELEASE.INTEGRITY.LOCK THEN EXIT FUNCTION                    !BSWM
        ENDIF                                                               !BSWM
        DINF.BATCH.ADD.END = 0                                              !BSWM

    END FUNCTION                                                            !BSWM

!******************************************************************************

