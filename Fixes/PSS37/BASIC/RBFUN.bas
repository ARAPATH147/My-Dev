\/******************************************************************/
\/*                                                                */
\/* Recalls Buffer (RB) FILE FUNCTIONS                             */
\/*                                                                */
\/* REFERENCE   : RBFUN.BAS                                        */
\/*                                                                */
\/* VERSION A.          Brian Greenfield        11th may 2007      */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE RBDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION RB.SET                                                */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION RB.SET PUBLIC

      INTEGER*1 RB.SET

      RB.REPORT.NUM% = 744
      RB.FILE.NAME$  = "RB:"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.RB                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.RB PUBLIC

      INTEGER*2 READ.RB

      READ.RB = 1

      IF END #RB.SESS.NUM% THEN READ.ERROR
      READ #RB.SESS.NUM%; LINE RB.RCD$

      RB.REC.TYPE$ = MID$(RB.RCD$,1,1)

      IF RB.REC.TYPE$ = "H" THEN BEGIN
         RB.REFERENCE$    = MID$(RB.RCD$,2,8)
         RB.LABEL$        = MID$(RB.RCD$,10,14)
      ENDIF ELSE IF RB.REC.TYPE$ = "D" THEN BEGIN
         RB.ITEM.CODE$    = MID$(RB.RCD$,2,7)
         RB.STOCK.COUNT$  = MID$(RB.RCD$,9,4)
      ENDIF ELSE IF RB.REC.TYPE$ = "T" THEN BEGIN
         RB.ITEM.COUNT$   = MID$(RB.RCD$,2,5)
         RB.RECORD.COUNT$ = MID$(RB.RCD$,7,4)
      ENDIF

      READ.RB = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = RB.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.RB                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.RB PUBLIC

      INTEGER*2 WRITE.RB
      STRING    CRLF$
      STRING    FORM$

      WRITE.RB = 1

      CRLF$ = CHR$(13) + CHR$(10)

      IF RB.REC.TYPE$ = "H" THEN BEGIN
         RB.RCD$ = RB.REC.TYPE$ \
                 + RIGHT$(STRING$(8,"0") + RB.REFERENCE$,8) \
                 + RIGHT$(STRING$(14,"0") + RB.LABEL$,14) \
                 + CRLF$
      ENDIF ELSE IF RB.REC.TYPE$ = "D" THEN BEGIN
         RB.RCD$ = RB.REC.TYPE$ \
                 + RIGHT$(STRING$(7,"0") + RB.ITEM.CODE$,7) \
                 + RIGHT$(STRING$(4,"0") + RB.STOCK.COUNT$,4) \
                 + CRLF$
      ENDIF ELSE IF RB.REC.TYPE$ = "T" THEN BEGIN
         RB.RCD$ = RB.REC.TYPE$ \
                 + RIGHT$(STRING$(5,"0") + RB.ITEM.COUNT$,5) \
                 + RIGHT$(STRING$(4,"0") + RB.RECORD.COUNT$,4) \
                 + CRLF$
      ENDIF

      FORM$ = "C" + STR$(LEN(RB.RCD$))

      IF END #RB.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM FORM$; #RB.SESS.NUM%; RB.RCD$

      WRITE.RB = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = RB.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/