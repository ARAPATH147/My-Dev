\/******************************************************************/
\/*                                                                */
\/* CIPRM File Functions                                           */
\/*                                                                */
\/* REFERENCE   : CIPRMFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Neil Bennett.              16 April 2007   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE CIPRMDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION CIPRM.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CIPRM.SET PUBLIC

      INTEGER*1 CIPRM.SET

      CIPRM.REPORT.NUM% = 740
      CIPRM.FILE.NAME$  = "CIPRM"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CIPRM                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CIPRM PUBLIC

      INTEGER*2 READ.CIPRM

      READ.CIPRM = 1

      IF END #CIPRM.SESS.NUM% THEN READ.ERROR
      READ #CIPRM.SESS.NUM%; LINE CIPRM.RCD$

      READ.CIPRM = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CIPRM.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CIPRM                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CIPRM PUBLIC

      INTEGER*2 WRITE.CIPRM
      STRING    form$

      WRITE.CIPRM = 1

      IF RIGHT$(CIPRM.RCD$, 2) <> CHR$(13) + CHR$(10) THEN BEGIN
         CIPRM.RCD$ = CIPRM.RCD$ + CHR$(13) + CHR$(10)
      ENDIF
      form$ = "C" + STR$(LEN(CIPRM.RCD$))

      IF END #CIPRM.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM form$; #CIPRM.SESS.NUM%; CIPRM.RCD$

      WRITE.CIPRM = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CIPRM.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/