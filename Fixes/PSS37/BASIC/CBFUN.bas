\/******************************************************************/
\/*                                                                */
\/* Carton Buffer (CB) FILE FUNCTIONS                              */
\/*                                                                */
\/* REFERENCE   : CBFUN.BAS                                        */
\/*                                                                */
\/* VERSION A.          Neil Bennett.           27 DECEMBER 2006   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE CBDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION CB.SET                                                */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CB.SET PUBLIC

      INTEGER*1 CB.SET

      CB.REPORT.NUM% = 736
      CB.FILE.NAME$  = "CB:"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CB                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CB PUBLIC

      INTEGER*2 READ.CB

      READ.CB = 1

      IF END #CB.SESS.NUM% THEN READ.ERROR
      READ #CB.SESS.NUM%; LINE CB.RCD$

      CB.REC.TYPE$ = MID$(CB.RCD$,  1,  1)

      IF CB.REC.TYPE$ = "C" THEN BEGIN
         CB.CARTON.BARCODE$ = MID$(CB.RCD$,  2, 14)
         CB.REPORT.RQD$     = MID$(CB.RCD$, 16,  1)
      ENDIF ELSE IF CB.REC.TYPE$ = "H" THEN BEGIN
         CB.CARTON.BARCODE$ = MID$(CB.RCD$,  2, 14)
      ENDIF ELSE IF CB.REC.TYPE$ = "D" THEN BEGIN
         CB.ITEM.BARCODE$   = MID$(CB.RCD$,  2, 13)
         CB.ITEM.QUANTITY$  = MID$(CB.RCD$, 15,  4)
      ENDIF ELSE IF CB.REC.TYPE$ = "T" THEN BEGIN
         CB.ITEM.COUNT$     = MID$(CB.RCD$,  2,  5)
      ENDIF

      READ.CB = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CB.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CB                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CB PUBLIC

      INTEGER*2 WRITE.CB
      STRING    crlf$
      STRING    form$

      WRITE.CB = 1

      crlf$ = CHR$(13) + CHR$(10)

      IF CB.REC.TYPE$ = "C" THEN BEGIN
         CB.RCD$ = CB.REC.TYPE$                                     \
                 + RIGHT$(STRING$(14,"0") + CB.CARTON.BARCODE$, 14) \
                 + RIGHT$(           " "  + CB.REPORT.RQD$    ,  1) \
                 + crlf$
      ENDIF ELSE IF CB.REC.TYPE$ = "H" THEN BEGIN
         CB.RCD$ = CB.REC.TYPE$                                     \
                 + RIGHT$(STRING$(14,"0") + CB.CARTON.BARCODE$, 14) \
                 + crlf$
      ENDIF ELSE IF CB.REC.TYPE$ = "D" THEN BEGIN
         CB.RCD$ = CB.REC.TYPE$                                     \
                 + RIGHT$(STRING$(13,"0") + CB.ITEM.BARCODE$  , 13) \
                 + RIGHT$(STRING$( 4,"0") + CB.ITEM.QUANTITY$ ,  4) \
                 + crlf$
      ENDIF ELSE IF CB.REC.TYPE$ = "T" THEN BEGIN
         CB.RCD$ = CB.REC.TYPE$                                     \
                 + RIGHT$(STRING$( 5,"0") + CB.ITEM.COUNT$    ,  5) \
                 + crlf$
      ENDIF

      form$ = "C" + STR$(LEN(CB.RCD$))

      IF END #CB.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM form$; #CB.SESS.NUM%; CB.RCD$

      WRITE.CB = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CB.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/