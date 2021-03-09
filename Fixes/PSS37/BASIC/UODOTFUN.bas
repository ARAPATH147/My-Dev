\*******************************************************************************
\***
\***    UOD Outers File Functions
\***
\***    REFERENCE   : UODOTFUN.BAS
\***
\***    VERSION A.              Stuart Highley                  11th Jul 2008
\***
\***    VERSION B.              Harpal Matharu                   3rd Nov 2008
\***    Added UODOT key length.
\***
\***    VERSION C.              Mark Goode                      10th Dec 2008
\***    Add summary record read/write functionality.
\***
\***    VERSION D.              Stuart Highley                   5th Feb 2009
\***    Correctly trim the summary record.
\***    Correctly conserve the summary filler on a write.
\***
\***    REVISION 1.8.           Robert Cowey                    14th Jan 2009
\***    Changes for 10A PosUOD fixes creating SSC04.286 Rv 1.3.
\***    Defined new function UODOT.REC$ based on WRITE.UODOT.
\***    Creates UODOT record string UODOT.REC$ from constituent variables.
\***
\***    VERSION E.              Mark Walker                      6th Mar 2015
\***    F391 HUMSS UOD Messaging
\***    Includes the following changes:
\***    - Added UOD number field.
\***    - Modified initialisation of filler field.
\***    - Various coding standards related changes (uncommented).
\***
\*******************************************************************************

    INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

    STRING GLOBAL    CURRENT.CODE$,                                     \
                     FILE.OPERATION$,                                   \
                     FORMAT.STRING$                                         !EMW
                     
    INTEGER*1 LOCK%                                                         !CMG
    
    %INCLUDE UODOTDEC.J86

    %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION UODOT.KEY$                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
                                  
FUNCTION UODOT.KEY$ PUBLIC

    STRING    UODOT.KEY$
    STRING    WORK$

    WORK$ = LEFT$(UODOT.LICENCE$ + "       ", 7)
    CALL PUTN2(WORK$, 5, UODOT.SEQ%)

    UODOT.KEY$ = WORK$
    WORK$ = ""

END FUNCTION
    
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/       !CMG
\/* FUNCTION READ.SUMMARY.STATUS                                   */       !CMG
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/       !CMG
    
FUNCTION READ.SUMMARY.STATUS                                                !CMG
    
    INTEGER*1 READ.SUMMARY.STATUS                                           !CMG
        
    IF END #UODOT.SESS.NUM% THEN READ.ERROR                                 !CMG
    IF LOCK% = 0 THEN BEGIN                                                 !CMG
           
       READ FORM "T8,C160,C2";                                          \   !CMG
           #UODOT.SESS.NUM% KEY UODOT.KEY$;                             \   !CMG
           UODOT.SUMMARY.STATUS$,                                       \   !CMG
           UODOT.FILLER$                                                    !CMG
    ENDIF ELSE BEGIN                                                        !CMG
        READ FORM "T8,C160,C2";                                         \   !CMG
           #UODOT.SESS.NUM% AUTOLOCK KEY UODOT.KEY$;                    \   !CMG
           UODOT.SUMMARY.STATUS$,                                       \   !CMG
           UODOT.FILLER$                                                    !CMG
    ENDIF                                                                   !CMG
        
    WHILE LEN(UODOT.SUMMARY.STATUS$) >= 5 AND                           \   !DSH
          RIGHT$(UODOT.SUMMARY.STATUS$, 5) = "     "                        !DSH 
        UODOT.SUMMARY.STATUS$ = LEFT$(UODOT.SUMMARY.STATUS$,            \   !DSH
                                LEN(UODOT.SUMMARY.STATUS$) - 5)             !DSH
    WEND                                                                    !DSH
                                          
    READ.SUMMARY.STATUS = 0                                                 !CMG
        
READ.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$       = UODOT.KEY$
                               
END FUNCTION                                                                !CMG

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/       !CMG
\/* FUNCTION WRITE.SUMMARY.STATUS                                  */       !CMG
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/       !CMG
    
FUNCTION WRITE.SUMMARY.STATUS                                               !CMG

    INTEGER*1 WRITE.SUMMARY.STATUS                                          !CMG
    
    UODOT.SUMMARY.STATUS$ = LEFT$(UODOT.SUMMARY.STATUS$ +               \   !DSH
                                  STRING$(160, " "), 160)                   !DSH
                                  
    IF END #UODOT.SESS.NUM% THEN WRITE.ERROR                                !CMG
    IF LOCK% = 0 THEN BEGIN                                                 !CMG
        WRITE FORM "C7 C160 C2";                                        \   !DSH
           #UODOT.SESS.NUM%;                                            \   !CMG
           UODOT.KEY$,                                                  \   !CMG
           UODOT.SUMMARY.STATUS$,                                       \   !DSH
           UODOT.FILLER$                                                    !DSH 
    ENDIF ELSE BEGIN                                                        !CMG
        WRITE FORM "C7 C160 C2";                                        \   !DSH
           #UODOT.SESS.NUM% AUTOUNLOCK;                                 \   !CMG
           UODOT.KEY$,                                                  \   !CMG
           UODOT.SUMMARY.STATUS$,                                       \   !DSH
           UODOT.FILLER$                                                    !DSH 
    ENDIF                                                                   !CMG
                                      
    WRITE.SUMMARY.STATUS = 0                                                !CMG

WRITE.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$ = UODOT.KEY$        
        
END FUNCTION                                                                !CMG

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION UODOT.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION UODOT.SET PUBLIC

    INTEGER*1 UODOT.SET

    UODOT.REPORT.NUM% = 766
    UODOT.RECL%       = 169
    UODOT.KEYL%       = 7                                                  !BHSM
    UODOT.FILE.NAME$  = "UODOT"
    UODOT.MAX.CHILDREN% = 15
!   UODOT.FILLER$ = STRING$(21,CHR$(0FFH))                                  !EMW
    UODOT.FILLER$ = STRING$(16,CHR$(0FFH))                                  !EMW
    DIM UODOT.CHILD.LICENCE$(UODOT.MAX.CHILDREN%)
    DIM UODOT.CHILD.TYPE$(UODOT.MAX.CHILDREN%)

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.UODOT                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION READ.UODOT PUBLIC

    STRING    CHILD.LIST$
    INTEGER*1 UODOT.INDICAT1%
    INTEGER*2 I%
    INTEGER*2 READ.UODOT
    INTEGER*2 UODOT.STATUS%

    READ.UODOT = 1

    DIM UODOT.CHILD.LICENCE$(UODOT.MAX.CHILDREN%)
    DIM UODOT.CHILD.TYPE$(UODOT.MAX.CHILDREN%)

    IF UODOT.KEY$ = STRING$(7, CHR$(0FFH)) THEN BEGIN                       !CMG
       LOCK% = 0                                                            !CMG
       READ.UODOT = READ.SUMMARY.STATUS                                     !CMG
       EXIT FUNCTION                                                        !CMG
    ENDIF                                                                   !CMG
    
    FORMAT.STRING$ = "T8,C3,I1,C3,C3,C2,C1,C1,C3,I2,C5,C5,C3,C2,C4," +  \   !EMW
                     "C4,C1,C4,C5,C16,I2,I2,C90"                            !EMW
    
    IF END #UODOT.SESS.NUM% THEN READ.ERROR
    READ FORM FORMAT.STRING$;                                           \   !EMW
        #UODOT.SESS.NUM% KEY UODOT.KEY$;                                \
        UODOT.DESPATCH.DATE$,                                           \
        UODOT.INDICAT1%,                                                \
        UODOT.EST.DEL.DATE$,                                            \
        UODOT.DRVR.DEL.DATE$,                                           \
        UODOT.DRVR.DEL.TIME$,                                           \
        UODOT.TYPE$,                                                    \
        UODOT.REASON$,                                                  \
        UODOT.WAREHOUSE.AREA$,                                          \
        UODOT.STATUS%,                                                  \
        UODOT.ULTIMATE.PARENT$,                                         \
        UODOT.IMMEDIATE.PARENT$,                                        \
        UODOT.BOOKED.DATE$,                                             \
        UODOT.BOOKED.TIME$,                                             \
        UODOT.STORE.OP.ID$,                                             \
        UODOT.DRVR.ID$,                                                 \
        UODOT.LEVEL$,                                                   \
        UODOT.AUDIT.OP.ID$,                                             \
        UODOT.UOD.NUMBER$,                                              \   !EMW
        UODOT.FILLER$,                                                  \
        UODOT.NUM.ITEMS%,                                               \
        UODOT.NUM.CHILDREN%,                                            \
        CHILD.LIST$

    FOR I% = 1 TO UODOT.MAX.CHILDREN%
        UODOT.CHILD.LICENCE$(I%) =                                      \   !EMW
            MID$(CHILD.LIST$,((I% - 1) * 6) + 1, 5)
        UODOT.CHILD.TYPE$(I%) =                                         \   !EMW
            MID$(CHILD.LIST$, ((I% - 1) * 6) + 6, 1)
    NEXT I%

    IF (UODOT.STATUS% AND 0001H) <> 0 THEN BEGIN
        UODOT.BOOKED = -1
    ENDIF ELSE BEGIN
        UODOT.BOOKED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0002H) <> 0 THEN BEGIN
        UODOT.STOCK.UPDATED = -1
    ENDIF ELSE BEGIN
        UODOT.STOCK.UPDATED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0004H) <> 0 THEN BEGIN
        UODOT.AUTO = -1
    ENDIF ELSE BEGIN
        UODOT.AUTO = 0
    ENDIF

    IF (UODOT.STATUS% AND 0008H) <> 0 THEN BEGIN
        UODOT.AUDITED = -1
    ENDIF ELSE BEGIN
        UODOT.AUDITED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0010H) <> 0 THEN BEGIN
        UODOT.PARTIAL = -1
    ENDIF ELSE BEGIN
        UODOT.PARTIAL = 0
    ENDIF

   IF (UODOT.STATUS% AND 0100H) <> 0 THEN BEGIN
        UODOT.GIT.MISMATCH = -1
    ENDIF ELSE BEGIN
        UODOT.GIT.MISMATCH = 0
    ENDIF

    IF (UODOT.STATUS% AND 0200H) <> 0 THEN BEGIN
        UODOT.RF = -1
    ENDIF ELSE BEGIN
        UODOT.RF = 0
    ENDIF

    IF (UODOT.STATUS% AND 0400H) <> 0 THEN BEGIN
        UODOT.PDT = -1
    ENDIF ELSE BEGIN
        UODOT.PDT = 0
    ENDIF

    IF (UODOT.STATUS% AND 0800H) <> 0 THEN BEGIN
        UODOT.MC70 = -1
    ENDIF ELSE BEGIN
        UODOT.MC70 = 0
    ENDIF

    IF (UODOT.STATUS% AND 1000H) <> 0 THEN BEGIN
        UODOT.CONTROLLER = -1
    ENDIF ELSE BEGIN
        UODOT.CONTROLLER = 0
    ENDIF

    IF (UODOT.INDICAT1% AND 01H) <> 0 THEN BEGIN
        UODOT.BOL = -1
    ENDIF ELSE BEGIN
        UODOT.BOL = 0
    ENDIF

    IF (UODOT.INDICAT1% AND 02H) <> 0 THEN BEGIN
        UODOT.SDPD = -1
    ENDIF ELSE BEGIN
        UODOT.SDPD = 0
    ENDIF

    CHILD.LIST$ = ""
    READ.UODOT = 0
    EXIT FUNCTION

READ.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$       = UODOT.KEY$

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.UODOT.ON.DESPATCH                                */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION READ.UODOT.ON.DESPATCH PUBLIC

    STRING TEMP$
    INTEGER*2 RC%
    INTEGER*2 READ.UODOT.ON.DESPATCH

    TEMP$ = UODOT.DESPATCH.DATE$

    UODOT.SEQ% = 0
    RC% = READ.UODOT
    WHILE RC% = 0 AND UODOT.DESPATCH.DATE$ <> TEMP$
        UODOT.SEQ% = UODOT.SEQ% + 1
        RC% = READ.UODOT
    WEND

    TEMP$ = ""
    READ.UODOT.ON.DESPATCH = RC%

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.UODOT.LOCK                                       */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION READ.UODOT.LOCK PUBLIC

    STRING    CHILD.LIST$
    INTEGER*1 UODOT.INDICAT1%
    INTEGER*2 I%
    INTEGER*2 READ.UODOT.LOCK
    INTEGER*2 UODOT.STATUS%

    READ.UODOT.LOCK = 1

    DIM UODOT.CHILD.LICENCE$(UODOT.MAX.CHILDREN%)
    DIM UODOT.CHILD.TYPE$(UODOT.MAX.CHILDREN%)

    IF UODOT.KEY$ = STRING$(7, CHR$(0FFH)) THEN BEGIN                       !CMG
       LOCK% = 1                                                            !CMG
       READ.UODOT.LOCK = READ.SUMMARY.STATUS                                !CMG
       EXIT FUNCTION                                                        !CMG
    ENDIF                                                                   !CMG

    FORMAT.STRING$ = "T8,C3,I1,C3,C3,C2,C1,C1,C3,I2,C5,C5,C3,C2,C4," +  \   !EMW
                     "C4,C1,C4,C5,C16,I2,I2,C90"                            !EMW
    
    IF END #UODOT.SESS.NUM% THEN READ.ERROR
    READ FORM FORMAT.STRING$;                                           \   !EMW
        #UODOT.SESS.NUM% AUTOLOCK KEY UODOT.KEY$;                       \
        UODOT.DESPATCH.DATE$,                                           \
        UODOT.INDICAT1%,                                                \
        UODOT.EST.DEL.DATE$,                                            \
        UODOT.DRVR.DEL.DATE$,                                           \
        UODOT.DRVR.DEL.TIME$,                                           \
        UODOT.TYPE$,                                                    \
        UODOT.REASON$,                                                  \
        UODOT.WAREHOUSE.AREA$,                                          \
        UODOT.STATUS%,                                                  \
        UODOT.ULTIMATE.PARENT$,                                         \
        UODOT.IMMEDIATE.PARENT$,                                        \
        UODOT.BOOKED.DATE$,                                             \
        UODOT.BOOKED.TIME$,                                             \
        UODOT.STORE.OP.ID$,                                             \
        UODOT.DRVR.ID$,                                                 \
        UODOT.LEVEL$,                                                   \
        UODOT.AUDIT.OP.ID$,                                             \
        UODOT.UOD.NUMBER$,                                              \   !EMW
        UODOT.FILLER$,                                                  \
        UODOT.NUM.ITEMS%,                                               \
        UODOT.NUM.CHILDREN%,                                            \
        CHILD.LIST$

    FOR I% = 1 TO UODOT.MAX.CHILDREN%
        UODOT.CHILD.LICENCE$(I%) =                                      \   !EMW
            MID$(CHILD.LIST$, ((I% - 1) * 6) + 1, 5)
        UODOT.CHILD.TYPE$(I%) =                                         \   !EMW
            MID$(CHILD.LIST$, ((I% - 1) * 6) + 6, 1)
    NEXT I%

    IF (UODOT.STATUS% AND 0001H) <> 0 THEN BEGIN
        UODOT.BOOKED = -1
    ENDIF ELSE BEGIN
        UODOT.BOOKED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0002H) <> 0 THEN BEGIN
        UODOT.STOCK.UPDATED = -1
    ENDIF ELSE BEGIN
        UODOT.STOCK.UPDATED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0004H) <> 0 THEN BEGIN
        UODOT.AUTO = -1
    ENDIF ELSE BEGIN
        UODOT.AUTO = 0
    ENDIF

    IF (UODOT.STATUS% AND 0008H) <> 0 THEN BEGIN
        UODOT.AUDITED = -1
    ENDIF ELSE BEGIN
        UODOT.AUDITED = 0
    ENDIF

    IF (UODOT.STATUS% AND 0010H) <> 0 THEN BEGIN
        UODOT.PARTIAL = -1
    ENDIF ELSE BEGIN
        UODOT.PARTIAL = 0
    ENDIF

   IF (UODOT.STATUS% AND 0100H) <> 0 THEN BEGIN
        UODOT.GIT.MISMATCH = -1
    ENDIF ELSE BEGIN
        UODOT.GIT.MISMATCH = 0
    ENDIF

    IF (UODOT.STATUS% AND 0200H) <> 0 THEN BEGIN
        UODOT.RF = -1
    ENDIF ELSE BEGIN
        UODOT.RF = 0
    ENDIF

    IF (UODOT.STATUS% AND 0400H) <> 0 THEN BEGIN
        UODOT.PDT = -1
    ENDIF ELSE BEGIN
        UODOT.PDT = 0
    ENDIF

    IF (UODOT.STATUS% AND 0800H) <> 0 THEN BEGIN
        UODOT.MC70 = -1
    ENDIF ELSE BEGIN
        UODOT.MC70 = 0
    ENDIF

    IF (UODOT.STATUS% AND 1000H) <> 0 THEN BEGIN
        UODOT.CONTROLLER = -1
    ENDIF ELSE BEGIN
        UODOT.CONTROLLER = 0
    ENDIF

    IF (UODOT.INDICAT1% AND 01H) <> 0 THEN BEGIN
        UODOT.BOL = -1
    ENDIF ELSE BEGIN
        UODOT.BOL = 0
    ENDIF

    IF (UODOT.INDICAT1% AND 02H) <> 0 THEN BEGIN
        UODOT.SDPD = -1
    ENDIF ELSE BEGIN
        UODOT.SDPD = 0
    ENDIF

    CHILD.LIST$ = ""
    READ.UODOT.LOCK = 0
    EXIT FUNCTION

READ.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$       = UODOT.KEY$

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.UODOT                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION WRITE.UODOT PUBLIC

    STRING    CHILD.LIST$
    INTEGER*1 UODOT.INDICAT1%
    INTEGER*2 I%
    INTEGER*2 UODOT.STATUS%
    INTEGER*2 WRITE.UODOT

    WRITE.UODOT = 1

    IF UODOT.KEY$ = STRING$(7, CHR$(0FFH)) THEN BEGIN                       !CMG
       LOCK% = 0                                                            !CMG
       WRITE.UODOT = WRITE.SUMMARY.STATUS                                   !CMG
       EXIT FUNCTION                                                        !CMG
    ENDIF                                                                   !CMG

    CHILD.LIST$ = ""
    FOR I% = 1 TO UODOT.MAX.CHILDREN%
        CHILD.LIST$ = CHILD.LIST$ +                                     \
            RIGHT$(STRING$(5,CHR$(0)) + " " +                           \
                   UODOT.CHILD.LICENCE$(I%) +                           \
                   UODOT.CHILD.TYPE$(I%), 6)
    NEXT I%

    UODOT.STATUS% = 0
    UODOT.INDICAT1% = 0

    IF UODOT.BOOKED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0001H)
    ENDIF

    IF UODOT.STOCK.UPDATED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0002H)
    ENDIF

    IF UODOT.AUTO THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0004H)
    ENDIF

    IF UODOT.AUDITED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0008H)
    ENDIF

    IF UODOT.PARTIAL THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0010H)
    ENDIF

    IF UODOT.GIT.MISMATCH THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0100H)
    ENDIF

    IF UODOT.RF THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0200H)
    ENDIF

    IF UODOT.PDT THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0400H)
    ENDIF

    IF UODOT.MC70 THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0800H)
    ENDIF

    IF UODOT.CONTROLLER THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 1000H)
    ENDIF

    IF UODOT.BOL THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 01H)
    ENDIF

    IF UODOT.SDPD THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 02H)
    ENDIF

    FORMAT.STRING$ = "C7,C3,I1,C3,C3,C2,C1,C1,C3,I2,C5,C5,C3,C2,C4," +  \   !EMW
                     "C4,C1,C4,C5,C16,I2,I2,C90"                            !EMW
    
    IF END #UODOT.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM FORMAT.STRING$;                                          \   !EMW
        #UODOT.SESS.NUM%;                                               \
        UODOT.KEY$,                                                     \
        UODOT.DESPATCH.DATE$,                                           \
        UODOT.INDICAT1%,                                                \
        UODOT.EST.DEL.DATE$,                                            \
        UODOT.DRVR.DEL.DATE$,                                           \
        UODOT.DRVR.DEL.TIME$,                                           \
        UODOT.TYPE$,                                                    \
        UODOT.REASON$,                                                  \
        UODOT.WAREHOUSE.AREA$,                                          \
        UODOT.STATUS%,                                                  \
        UODOT.ULTIMATE.PARENT$,                                         \
        UODOT.IMMEDIATE.PARENT$,                                        \
        UODOT.BOOKED.DATE$,                                             \
        UODOT.BOOKED.TIME$,                                             \
        UODOT.STORE.OP.ID$,                                             \
        UODOT.DRVR.ID$,                                                 \
        UODOT.LEVEL$,                                                   \
        UODOT.AUDIT.OP.ID$,                                             \
        UODOT.UOD.NUMBER$,                                              \   !EMW
        UODOT.FILLER$,                                                  \
        UODOT.NUM.ITEMS%,                                               \
        UODOT.NUM.CHILDREN%,                                            \
        CHILD.LIST$

    CHILD.LIST$ = ""
    WRITE.UODOT = 0
    EXIT FUNCTION

WRITE.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$ = UODOT.KEY$

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.UODOT.UNLOCK                                    */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION WRITE.UODOT.UNLOCK PUBLIC

    STRING    CHILD.LIST$
    INTEGER*1 UODOT.INDICAT1%
    INTEGER*2 I%
    INTEGER*2 UODOT.STATUS%
    INTEGER*2 WRITE.UODOT.UNLOCK

    WRITE.UODOT.UNLOCK = 1

    IF UODOT.KEY$ = STRING$(7, CHR$(0FFH)) THEN BEGIN                       !CMG
       LOCK% = 1                                                            !CMG
       WRITE.UODOT.UNLOCK = WRITE.SUMMARY.STATUS                            !CMG
       EXIT FUNCTION                                                        !CMG
    ENDIF                                                                   !CMG

    CHILD.LIST$ = ""
    FOR I% = 1 TO UODOT.MAX.CHILDREN%
        CHILD.LIST$ = CHILD.LIST$ +                                     \
            RIGHT$(STRING$(5,CHR$(0)) + " " +                           \
                   UODOT.CHILD.LICENCE$(I%) +                           \
                   UODOT.CHILD.TYPE$(I%), 6)
    NEXT I%
    UODOT.STATUS% = 0
    UODOT.INDICAT1% = 0

    IF UODOT.BOOKED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0001H)
    ENDIF

    IF UODOT.STOCK.UPDATED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0002H)
    ENDIF

    IF UODOT.AUTO THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0004H)
    ENDIF

    IF UODOT.AUDITED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0008H)
    ENDIF

    IF UODOT.PARTIAL THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0010H)
    ENDIF

    IF UODOT.GIT.MISMATCH THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0100H)
    ENDIF

    IF UODOT.RF THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0200H)
    ENDIF

    IF UODOT.PDT THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0400H)
    ENDIF

    IF UODOT.MC70 THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0800H)
    ENDIF

    IF UODOT.CONTROLLER THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 1000H)
    ENDIF

    IF UODOT.BOL THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 01H)
    ENDIF

    IF UODOT.SDPD THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 02H)
    ENDIF

    FORMAT.STRING$ = "C7,C3,I1,C3,C3,C2,C1,C1,C3,I2,C5,C5,C3,C2,C4," +  \   !EMW
                     "C4,C1,C4,C5,C16,I2,I2,C90"                            !EMW

    IF END #UODOT.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM FORMAT.STRING$;                                          \   !EMW
        #UODOT.SESS.NUM% AUTOUNLOCK;                                    \
        UODOT.KEY$,                                                     \
        UODOT.DESPATCH.DATE$,                                           \
        UODOT.INDICAT1%,                                                \
        UODOT.EST.DEL.DATE$,                                            \
        UODOT.DRVR.DEL.DATE$,                                           \
        UODOT.DRVR.DEL.TIME$,                                           \
        UODOT.TYPE$,                                                    \
        UODOT.REASON$,                                                  \
        UODOT.WAREHOUSE.AREA$,                                          \
        UODOT.STATUS%,                                                  \
        UODOT.ULTIMATE.PARENT$,                                         \
        UODOT.IMMEDIATE.PARENT$,                                        \
        UODOT.BOOKED.DATE$,                                             \
        UODOT.BOOKED.TIME$,                                             \
        UODOT.STORE.OP.ID$,                                             \
        UODOT.DRVR.ID$,                                                 \
        UODOT.LEVEL$,                                                   \
        UODOT.AUDIT.OP.ID$,                                             \
        UODOT.UOD.NUMBER$,                                              \   !EMW
        UODOT.FILLER$,                                                  \
        UODOT.NUM.ITEMS%,                                               \
        UODOT.NUM.CHILDREN%,                                            \
        CHILD.LIST$

    CHILD.LIST$ = ""
    WRITE.UODOT.UNLOCK = 0
    EXIT FUNCTION

WRITE.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$ = UODOT.KEY$

    END FUNCTION

\/******************************************************************/

FUNCTION DELETE.UODOT PUBLIC

    INTEGER*2 DELETE.UODOT

    DELETE.UODOT = 1
    IF END #UODOT.SESS.NUM% THEN DELETE.ERROR
    DELREC UODOT.SESS.NUM%; UODOT.KEY$
    DELETE.UODOT = 0
    EXIT FUNCTION

DELETE.ERROR:

    FILE.OPERATION$     = "D"
    CURRENT.REPORT.NUM% = UODOT.REPORT.NUM%
    CURRENT.CODE$       = UODOT.KEY$

END FUNCTION

\*****************************************************************************
\***
\***    FUNCTION UODOT.REC$
\***    Creates UODOT record string UODOT.REC$ from its constituent variables.
\***    Function is a modification of WRITE.UODOT.
\***
\***..........................................................................

FUNCTION UODOT.REC$ PUBLIC ! Entire function new of Rv 1.8               !1.8 RC

    INTEGER*1 UODOT.INDICAT1%
    
    INTEGER*2 UODOT.STATUS%
    INTEGER*2 I%
    
    STRING    CHILD.LIST$
    STRING    UODOT.REC$
    STRING    WORK$

    
    CHILD.LIST$ = ""
    FOR I% = 1 TO UODOT.MAX.CHILDREN%
        CHILD.LIST$ = CHILD.LIST$ +                                     \
            RIGHT$(STRING$(5,CHR$(0)) + " " +                           \
                   UODOT.CHILD.LICENCE$(I%) +                           \
                   UODOT.CHILD.TYPE$(I%), 6)
    NEXT I%

    UODOT.STATUS% = 0
    UODOT.INDICAT1% = 0

    IF UODOT.BOOKED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0001H)
    ENDIF

    IF UODOT.STOCK.UPDATED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0002H)
    ENDIF

    IF UODOT.AUTO THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0004H)
    ENDIF

    IF UODOT.AUDITED THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0008H)
    ENDIF

    IF UODOT.PARTIAL THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0010H)
    ENDIF

    IF UODOT.GIT.MISMATCH THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0100H)
    ENDIF

    IF UODOT.RF THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0200H)
    ENDIF

    IF UODOT.PDT THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0400H)
    ENDIF

    IF UODOT.MC70 THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 0800H)
    ENDIF

    IF UODOT.CONTROLLER THEN BEGIN
        UODOT.STATUS% = (UODOT.STATUS% OR 1000H)
    ENDIF

    IF UODOT.BOL THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 01H)
    ENDIF

    IF UODOT.SDPD THEN BEGIN
        UODOT.INDICAT1% = (UODOT.INDICAT1% OR 02H)
    ENDIF

!   Create UODOT.REC$ from individual strings (leaving space for integers)
    WORK$ = \                     Start Length
      UODOT.LICENCE$          + \    1   5 
     "  "                     + \    6   2  UODOT.SEQ%
      UODOT.DESPATCH.DATE$    + \    8   3 
 CHR$(UODOT.INDICAT1%)        + \   11   1  UODOT.INDICAT1%
      UODOT.EST.DEL.DATE$     + \   12   3 
      UODOT.DRVR.DEL.DATE$    + \   15   3 
      UODOT.DRVR.DEL.TIME$    + \   18   2 
      UODOT.TYPE$             + \   20   1 
      UODOT.REASON$           + \   21   1 
      UODOT.WAREHOUSE.AREA$   + \   22   3 
     "  "                     + \   25   2  UODOT.STATUS%
      UODOT.ULTIMATE.PARENT$  + \   27   5 
      UODOT.IMMEDIATE.PARENT$ + \   32   5 
      UODOT.BOOKED.DATE$      + \   37   3 
      UODOT.BOOKED.TIME$      + \   40   2 
      UODOT.STORE.OP.ID$      + \   42   4 
      UODOT.DRVR.ID$          + \   46   4 
      UODOT.LEVEL$            + \   50   1 
      UODOT.AUDIT.OP.ID$      + \   51   4 
      UODOT.UOD.NUMBER$       + \   55   5                                  !EMW
      UODOT.FILLER$           + \   60  16                                  !EMW
     "  "                     + \   76   2  UODOT.NUM.ITEMS%
     "  "                     + \   78   2  UODOT.NUM.CHILDREN%
      CHILD.LIST$               !   80  90 

!   Insert integers into WORK$ with PUTN2 (using offsets)
    CALL PUTN2(WORK$,  5, UODOT.SEQ%)
!   CALL PUTN (WORK$, 10, UODOT.INDICAT1%) ! No PUTN for one byte integer
    CALL PUTN2(WORK$, 24, UODOT.STATUS%)
    CALL PUTN2(WORK$, 75, UODOT.NUM.ITEMS%)
    CALL PUTN2(WORK$, 77, UODOT.NUM.CHILDREN%)
    
!   Set UODOT.REC$ from WORK$
    UODOT.REC$ = WORK$

END FUNCTION

