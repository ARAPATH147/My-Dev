\/******************************************************************/
\/*                                                                */
\/* CIPPM File Functions                                           */
\/*                                                                */
\/* REFERENCE   : CIPPMFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Neil Bennett.              16 April 2007   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE CIPPMDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION CIPPM.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CIPPM.SET PUBLIC

      INTEGER*1 CIPPM.SET

      CIPPM.REPORT.NUM% = 741
      CIPPM.FILE.NAME$  = "CIPPMR"
      CIPPM.BKUP.NAME$  = "ADXLXACN::D:\ADX_UDT1\CIPPMR.BAK"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CIPPM                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CIPPM PUBLIC

      INTEGER*2 READ.CIPPM

      READ.CIPPM = 1

      IF END #CIPPM.SESS.NUM% THEN READ.ERROR
      READ #CIPPM.SESS.NUM%; LINE CIPPM.RCD$

      READ.CIPPM = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CIPPM.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CIPPM                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CIPPM PUBLIC

      INTEGER*2 WRITE.CIPPM
      STRING    form$

      WRITE.CIPPM = 1

      IF RIGHT$(CIPPM.RCD$, 2) <> CHR$(13) + CHR$(10) THEN BEGIN
         CIPPM.RCD$ = CIPPM.RCD$ + CHR$(13) + CHR$(10)
      ENDIF
      form$ = "C" + STR$(LEN(CIPPM.RCD$))

      IF END #CIPPM.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM form$; #CIPPM.SESS.NUM%; CIPPM.RCD$

      WRITE.CIPPM = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CIPPM.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/