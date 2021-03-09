\*******************************************************************************
\***
\***    UOD Outers File Functions
\***
\***    REFERENCE   : SSCCTFUN.BAS
\***
\***    VERSION A.              Stuart Highley                  15th Sep 2008
\***    Initial version.
\***
\***    VERSION B.              Mark Walker                      6th Mar 2015
\***    F391 HUMSS UOD Messaging
\***    Includes the following changes:
\***    - Added HUMSS and NEWIF last processed date fields.
\***    - Various coding standards related changes (uncommented).
\***
\*******************************************************************************

INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

STRING GLOBAL    CURRENT.CODE$,                                         \
                 FILE.OPERATION$,                                       \
                 FORMAT.STRING$                                             !BMW

%INCLUDE SSCCTDEC.J86

!*******************************************************************
!***  PRIVATE FUNCTION TRIM$ (trims trailing spaces)
!*******************************************************************
    FUNCTION TRIM$(TXT$)
        STRING TRIM$, TXT$
        WHILE RIGHT$(TXT$, 1) = " " AND LEN(TXT$) > 0
            TXT$ = LEFT$(TXT$, LEN(TXT$) - 1)
        WEND
        TRIM$ = TXT$
        TXT$ = ""
    END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION SSCCTRL.SET                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION SSCCTRL.SET PUBLIC

    INTEGER*1 SSCCTRL.SET

    SSCCTRL.REPORT.NUM%   = 772
    SSCCTRL.RECL%         = 80
    SSCCTRL.MAX.PREFIXES% = 13
    SSCCTRL.FILE.NAME$  = "SSCCTRL"
    DIM SSCCTRL.EPOS.TYPE$(SSCCTRL.MAX.PREFIXES%)
    DIM SSCCTRL.SAP.TYPE$(SSCCTRL.MAX.PREFIXES%)
    DIM SSCCTRL.PREFIX$(SSCCTRL.MAX.PREFIXES%)

END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.SSCCTRL                                          */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION READ.SSCCTRL PUBLIC

    STRING REC$
    INTEGER*2 I%, READ.SSCCTRL

    READ.SSCCTRL = 1
    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = SSCCTRL.REPORT.NUM%
    CURRENT.CODE$       = STR$(SSCCTRL.REC.NUM%)

    IF END #SSCCTRL.SESS.NUM% THEN READ.ERROR

    IF SSCCTRL.REC.NUM% = 1 THEN BEGIN

        FORMAT.STRING$ = "C3,C3,C3,C3,C3,C3,C8,C4,C1,C8,C4,C2,C2,C2," + \   !BMW
                         "C8,C8,C15"                                        !BMW

        READ FORM FORMAT.STRING$;                                       \   !BMW
            # SSCCTRL.SESS.NUM%, 1;                                     \
            SSCCTRL.UODBNK.ACCEPT.DAYS$,                                \
            SSCCTRL.UODBNK.REJECT.DAYS$,                                \
            SSCCTRL.UODOT.NUM.DAYS$,                                    \
            SSCCTRL.UNBOOKED.NUM.DAYS$,                                 \
            SSCCTRL.UNBOOKED.IST.DAYS$,                                 \
            SSCCTRL.BUFFER.NUM.DAYS$,                                   \
            SSCCTRL.SSC04.LAST.RUN$,                                    \
            SSCCTRL.SSC04.RUN.TIME$,                                    \
            SSCCTRL.SSC04.OK$,                                          \
            SSCCTRL.SSC06.LAST.RUN$,                                    \
            SSCCTRL.SSC06.RUN.TIME$,                                    \
            SSCCTRL.UODBNK.PERC.FULL$,                                  \
            SSCCTRL.UODOT.PERC.FULL$,                                   \
            SSCCTRL.UODIN.PERC.FULL$,                                   \
            SSCCTRL.NEWIF.UOD.DATE$,                                    \   !BMW
            SSCCTRL.HUMSS.UOD.DATE$,                                    \   !BMW
            SSCCTRL.FILLER$

        READ.SSCCTRL = 0

    ENDIF ELSE IF SSCCTRL.REC.NUM% = 2 THEN BEGIN

        READ FORM "C78 C2"; # SSCCTRL.SESS.NUM%, 2; REC$, \
                                                    SSCCTRL.FILLER$
        FOR I% = 1 TO SSCCTRL.MAX.PREFIXES%
            SSCCTRL.EPOS.TYPE$(I%) = MID$(REC$, ((I%-1) * 6) + 1, 1)
            SSCCTRL.SAP.TYPE$(I%)  = MID$(REC$, ((I%-1) * 6) + 2, 1)
            SSCCTRL.PREFIX$(I%)    =                                    \
                TRIM$(MID$(REC$, ((I%-1) * 6) + 3, 4))
        NEXT I%

        READ.SSCCTRL = 0

    ENDIF

    EXIT FUNCTION

READ.ERROR:

END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.SSCCTRL.LOCK                                     */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION READ.SSCCTRL.LOCK PUBLIC

    STRING REC$
    INTEGER*2 I%, READ.SSCCTRL.LOCK

    READ.SSCCTRL.LOCK = 1
    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = SSCCTRL.REPORT.NUM%
    CURRENT.CODE$       = STR$(SSCCTRL.REC.NUM%)

    IF END #SSCCTRL.SESS.NUM% THEN READ.ERROR

    IF SSCCTRL.REC.NUM% = 1 THEN BEGIN

        FORMAT.STRING$ = "C3,C3,C3,C3,C3,C3,C8,C4,C1,C8,C4,C2,C2,C2," + \   !BMW
                         "C8,C8,C15"                                        !BMW
        
        READ FORM FORMAT.STRING$;                                       \
            # SSCCTRL.SESS.NUM% AUTOLOCK, 1;                            \
            SSCCTRL.UODBNK.ACCEPT.DAYS$,                                \
            SSCCTRL.UODBNK.REJECT.DAYS$,                                \
            SSCCTRL.UODOT.NUM.DAYS$,                                    \
            SSCCTRL.UNBOOKED.NUM.DAYS$,                                 \
            SSCCTRL.UNBOOKED.IST.DAYS$,                                 \
            SSCCTRL.BUFFER.NUM.DAYS$,                                   \
            SSCCTRL.SSC04.LAST.RUN$,                                    \
            SSCCTRL.SSC04.RUN.TIME$,                                    \
            SSCCTRL.SSC04.OK$,                                          \
            SSCCTRL.SSC06.LAST.RUN$,                                    \
            SSCCTRL.SSC06.RUN.TIME$,                                    \
            SSCCTRL.UODBNK.PERC.FULL$,                                  \
            SSCCTRL.UODOT.PERC.FULL$,                                   \
            SSCCTRL.UODIN.PERC.FULL$,                                   \
            SSCCTRL.NEWIF.UOD.DATE$,                                    \   !BMW
            SSCCTRL.HUMSS.UOD.DATE$,                                    \   !BMW
            SSCCTRL.FILLER$

        READ.SSCCTRL.LOCK = 0

    ENDIF ELSE IF SSCCTRL.REC.NUM% = 2 THEN BEGIN

        READ FORM "C78 C2"; # SSCCTRL.SESS.NUM%, 2; REC$,               \
                                                    SSCCTRL.FILLER$
        FOR I% = 1 TO SSCCTRL.MAX.PREFIXES%
            SSCCTRL.EPOS.TYPE$(I%) = MID$(REC$, ((I%-1) * 6) + 1, 1)
            SSCCTRL.SAP.TYPE$(I%)  = MID$(REC$, ((I%-1) * 6) + 2, 1)
            SSCCTRL.PREFIX$(I%)    =                                    \
                TRIM$(MID$(REC$, ((I%-1) * 6) + 3, 4))
        NEXT I%

        READ.SSCCTRL.LOCK = 0

    ENDIF

    EXIT FUNCTION

READ.ERROR:

END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.SSCCTRL                                         */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION WRITE.SSCCTRL PUBLIC

    STRING REC$
    INTEGER*2 I%, WRITE.SSCCTRL

    WRITE.SSCCTRL = 1
    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = SSCCTRL.REPORT.NUM%
    CURRENT.CODE$       = STR$(SSCCTRL.REC.NUM%)

    IF END #SSCCTRL.SESS.NUM% THEN WRITE.ERROR

    IF SSCCTRL.REC.NUM% = 1 THEN BEGIN

        FORMAT.STRING$ = "C3,C3,C3,C3,C3,C3,C8,C4,C1,C8,C4,C2,C2,C2," + \   !BMW
                         "C8,C8,C15"                                        !BMW
        
        WRITE FORM FORMAT.STRING$;                                      \   !BMW
            # SSCCTRL.SESS.NUM%, 1;                                     \
            SSCCTRL.UODBNK.ACCEPT.DAYS$,                                \
            SSCCTRL.UODBNK.REJECT.DAYS$,                                \
            SSCCTRL.UODOT.NUM.DAYS$,                                    \
            SSCCTRL.UNBOOKED.NUM.DAYS$,                                 \
            SSCCTRL.UNBOOKED.IST.DAYS$,                                 \
            SSCCTRL.BUFFER.NUM.DAYS$,                                   \
            SSCCTRL.SSC04.LAST.RUN$,                                    \
            SSCCTRL.SSC04.RUN.TIME$,                                    \
            SSCCTRL.SSC04.OK$,                                          \
            SSCCTRL.SSC06.LAST.RUN$,                                    \
            SSCCTRL.SSC06.RUN.TIME$,                                    \
            SSCCTRL.UODBNK.PERC.FULL$,                                  \
            SSCCTRL.UODOT.PERC.FULL$,                                   \
            SSCCTRL.UODIN.PERC.FULL$,                                   \
            SSCCTRL.NEWIF.UOD.DATE$,                                    \   !BMW
            SSCCTRL.HUMSS.UOD.DATE$,                                    \   !BMW
            SSCCTRL.FILLER$

        WRITE.SSCCTRL = 0

    ENDIF ELSE IF SSCCTRL.REC.NUM% = 2 THEN BEGIN

        REC$ = ""
        FOR I% = 1 TO SSCCTRL.MAX.PREFIXES%
            REC$ = REC$ + LEFT$(SSCCTRL.EPOS.TYPE$(I%) + " ", 1) +      \
                          LEFT$(SSCCTRL.SAP.TYPE$(I%)  + " ", 1) +      \
                          LEFT$(SSCCTRL.PREFIX$(I%) + "    ", 4)
        NEXT I%

        WRITE FORM "C78 C2"; # SSCCTRL.SESS.NUM%, 2; REC$, \
                                                     SSCCTRL.FILLER$

        WRITE.SSCCTRL = 0

    ENDIF

    EXIT FUNCTION

WRITE.ERROR:

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.SSCCTRL.UNLOCK                                  */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION WRITE.SSCCTRL.UNLOCK PUBLIC

    STRING REC$
    INTEGER*2 I%, WRITE.SSCCTRL.UNLOCK

    WRITE.SSCCTRL.UNLOCK = 1
    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = SSCCTRL.REPORT.NUM%
    CURRENT.CODE$       = STR$(SSCCTRL.REC.NUM%)

    IF END #SSCCTRL.SESS.NUM% THEN WRITE.ERROR

    IF SSCCTRL.REC.NUM% = 1 THEN BEGIN

        FORMAT.STRING$ = "C3,C3,C3,C3,C3,C3,C8,C4,C1,C8,C4,C2,C2,C2," + \   !BMW
                         "C8,C8,C15"                                        !BMW
        
        WRITE FORM FORMAT.STRING$;                                      \   !BMW
            # SSCCTRL.SESS.NUM% AUTOUNLOCK, 1;                          \
            SSCCTRL.UODBNK.ACCEPT.DAYS$,                                \
            SSCCTRL.UODBNK.REJECT.DAYS$,                                \
            SSCCTRL.UODOT.NUM.DAYS$,                                    \ 
            SSCCTRL.UNBOOKED.NUM.DAYS$,                                 \
            SSCCTRL.UNBOOKED.IST.DAYS$,                                 \
            SSCCTRL.BUFFER.NUM.DAYS$,                                   \
            SSCCTRL.SSC04.LAST.RUN$,                                    \
            SSCCTRL.SSC04.RUN.TIME$,                                    \
            SSCCTRL.SSC04.OK$,                                          \
            SSCCTRL.SSC06.LAST.RUN$,                                    \
            SSCCTRL.SSC06.RUN.TIME$,                                    \
            SSCCTRL.UODBNK.PERC.FULL$,                                  \
            SSCCTRL.UODOT.PERC.FULL$,                                   \
            SSCCTRL.UODIN.PERC.FULL$,                                   \
            SSCCTRL.NEWIF.UOD.DATE$,                                    \   !BMW
            SSCCTRL.HUMSS.UOD.DATE$,                                    \   !BMW
            SSCCTRL.FILLER$

        WRITE.SSCCTRL.UNLOCK = 0

    ENDIF ELSE IF SSCCTRL.REC.NUM% = 2 THEN BEGIN

        REC$ = ""
        FOR I% = 1 TO SSCCTRL.MAX.PREFIXES%
            REC$ = REC$ + LEFT$(SSCCTRL.EPOS.TYPE$(I%) + " ", 1) +      \
                          LEFT$(SSCCTRL.SAP.TYPE$(I%)  + " ", 1) +      \
                          LEFT$(SSCCTRL.PREFIX$(I%) + "    ", 4)
        NEXT I%

        WRITE FORM "C78 C2"; # SSCCTRL.SESS.NUM%, 2; REC$, \
                                                     SSCCTRL.FILLER$

        WRITE.SSCCTRL.UNLOCK = 0

    ENDIF

    EXIT FUNCTION

WRITE.ERROR:

END FUNCTION

!*******************************************************************************
!*** FUNCTION FIND.PREFIX%                                          
!*** Private function to find the matching index in the             
!*** prefix table for a given UOD licence.                          
!*******************************************************************************

FUNCTION FIND.PREFIX%(LICENCE$)

    STRING LICENCE$
    INTEGER*2 I%, FIND.PREFIX%, LEN%

    FIND.PREFIX% = -1

    FOR I% = 1 TO SSCCTRL.MAX.PREFIXES%
        LEN% = LEN(SSCCTRL.PREFIX$(I%))
        IF LEN% > 0 THEN BEGIN
            IF LEFT$(LICENCE$, LEN%) = SSCCTRL.PREFIX$(I%) THEN BEGIN
                FIND.PREFIX% = I%
                I% = SSCCTRL.MAX.PREFIXES%
            ENDIF
        ENDIF
    NEXT I%

    LICENCE$ = ""

END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION SSCCTRL.GET.EPOS.TYPE$                                */
\/* The caller must read record 2 of SSCCTRL before calling this   */
\/* function.                                                      */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION SSCCTRL.GET.EPOS.TYPE$(LICENCE$) PUBLIC

    STRING SSCCTRL.GET.EPOS.TYPE$, LICENCE$
    INTEGER*2 I%

    I% = FIND.PREFIX%(LICENCE$)
    IF I% = -1 THEN BEGIN
        SSCCTRL.GET.EPOS.TYPE$ = "?"
    ENDIF ELSE BEGIN
        SSCCTRL.GET.EPOS.TYPE$ = SSCCTRL.EPOS.TYPE$(I%)
    ENDIF

    LICENCE$ = ""

END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION SSCCTRL.GET.SAP.TYPE$                                 */
\/* The caller must read record 2 of SSCCTRL before calling this   */
\/* function.                                                      */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

FUNCTION SSCCTRL.GET.SAP.TYPE$(LICENCE$) PUBLIC

    STRING SSCCTRL.GET.SAP.TYPE$, LICENCE$
    INTEGER*2 I%

    I% = FIND.PREFIX%(LICENCE$)
    IF I% = -1 THEN BEGIN
        SSCCTRL.GET.SAP.TYPE$ = "?"
    ENDIF ELSE BEGIN
        SSCCTRL.GET.SAP.TYPE$ = SSCCTRL.SAP.TYPE$(I%)
    ENDIF

    LICENCE$ = ""

END FUNCTION

