\/******************************************************************/
\/*                                                                */
\/* DELVINDX Log File Functions                                    */
\/*                                                                */
\/* REFERENCE   : DELVIFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Stuart Highley          11 July 2008       */
\/*                                                                */
\/* VERSION B.          Stuart Highley           18 Dec 2008       */
\/* Make file fixed rec length                                     */
\/******************************************************************/

    STRING GLOBAL CURRENT.CODE$
    STRING GLOBAL FILE.OPERATION$
    INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

    STRING CRLF$, QUOTE$

    %INCLUDE DELVIDEC.J86

    %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION DELVINDX.SET                                          */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION DELVINDX.SET PUBLIC

        INTEGER*1 DELVINDX.SET

        DELVINDX.REPORT.NUM% = 768
        DELVINDX.FILE.NAME$  = "DELVINDX"
        CRLF$ = CHR$(0DH) + CHR$(0AH)
        QUOTE$ = """"

    END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.DELVINDX                                         */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION READ.DELVINDX PUBLIC

        STRING DELVINDX.REC$
        INTEGER*2 READ.DELVINDX

        READ.DELVINDX = 1

        IF END #DELVINDX.SESS.NUM% THEN READ.ERROR
        READ #DELVINDX.SESS.NUM%; DELVINDX.REC$                             !BSH

        DELVINDX.UOD.LICENCE$       = MID$(DELVINDX.REC$, 1, 10)            !BSH
        DELVINDX.UOD.SEQ$           = MID$(DELVINDX.REC$, 11, 5)            !BSH
        DELVINDX.UOD.DESPATCH.DATE$ = MID$(DELVINDX.REC$, 16, 6)            !BSH
        DELVINDX.UOD.PARENT$        = MID$(DELVINDX.REC$, 22, 10)           !BSH
        DELVINDX.UOD.TYPE$          = MID$(DELVINDX.REC$, 32, 1)            !BSH
        DELVINDX.UOD.EXP.DEL.DATE$  = MID$(DELVINDX.REC$, 33, 6)            !BSH
        DELVINDX.UOD.BOL.FLAG$      = MID$(DELVINDX.REC$, 39, 1)            !BSH
        DELVINDX.FILLER$            = MID$(DELVINDX.REC$, 40, 32767)        !BSH

        READ.DELVINDX = 0
        EXIT FUNCTION

READ.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = DELVINDX.REPORT.NUM%
        CURRENT.CODE$       = ""

    END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.DELVINDX                                        */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION WRITE.DELVINDX PUBLIC

        INTEGER*2 WRITE.DELVINDX

        WRITE.DELVINDX = 1

        IF END #DELVINDX.SESS.NUM% THEN WRITE.ERROR
        WRITE FORM "C1 C10 C5 C6 C10 C1 C6 C1 C5 C1 C2";        \
            #DELVINDX.SESS.NUM%;                                \
            QUOTE$,                                             \
            DELVINDX.UOD.LICENCE$,                              \
            DELVINDX.UOD.SEQ$,                                  \
            DELVINDX.UOD.DESPATCH.DATE$,                        \
            DELVINDX.UOD.PARENT$,                               \
            DELVINDX.UOD.TYPE$,                                 \
            DELVINDX.UOD.EXP.DEL.DATE$,                         \
            DELVINDX.UOD.BOL.FLAG$,                             \
            DELVINDX.FILLER$,                                   \
            QUOTE$,                                             \
            CRLF$
            
        WRITE.DELVINDX = 0
        EXIT FUNCTION

WRITE.ERROR:

        FILE.OPERATION$ = "W"
        CURRENT.REPORT.NUM% = DELVINDX.REPORT.NUM%
        CURRENT.CODE$ = ""

    END FUNCTION

\/******************************************************************/

