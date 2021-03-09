\/******************************************************************/
\/*                                                                */
\/* CIPOK File Functions                                           */
\/*                                                                */
\/* REFERENCE   : CIPOKFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Neil Bennett.            16 April 2007     */
\/*                                                                */
\/* VERSION B.          Charles Skadorwa         31 May 2007       */
\/*                     Added PSD76 fields.                        */
\/*                                                                */
\/* VERSION C.          Steve Perkins            12 September 2007 */
\/*                     Added new report flag.                     */
\/*                     Initially used to control the weekly       */
\/*                     production of a particular section of the  */
\/*                     PSD69 report.                              */    
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE CIPOKDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION CIPOK.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CIPOK.SET PUBLIC

      INTEGER*1 CIPOK.SET

      CIPOK.REPORT.NUM% = 738
      CIPOK.RECL%       =  80
      CIPOK.FILE.NAME$  = "CIPOK"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CIPOK                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CIPOK PUBLIC

      INTEGER*2 READ.CIPOK

      READ.CIPOK = 1

      IF END #CIPOK.SESS.NUM% THEN READ.ERROR
      READ FORM "C4,C4,C8,C1,C8,C6,C1,C3,C1,C1,C8,C6,C1,C8,C6,C1,C1,C12";  \ !CSP
           #CIPOK.SESS.NUM%, 1;                                     \
               CIPOK.STORE$,                                        \
               CIPOK.SER.NO$,                                       \
               CIPOK.DATE$,                                         \
               CIPOK.PSD66.RUNFLAG$,                                \
               CIPOK.PSD67.RUNDATE$,                                \
               CIPOK.PSD67.RUNTIME$,                                \
               CIPOK.PSD67.RUNFLAG$,                                \
               CIPOK.MARKDOWN.DAYS$,                                \
               CIPOK.CIP.STATUS$,                                   \
               CIPOK.THISWEEK.FLAG$,                                \
               CIPOK.PSD69.RUNDATE$,                                \
               CIPOK.PSD69.RUNTIME$,                                \
               CIPOK.PSD69.RUNFLAG$,                                \
               CIPOK.PSD76.RUNDATE$,                                \      !BCSK
               CIPOK.PSD76.RUNTIME$,                                \      !BCSK
               CIPOK.PSD76.RUNFLAG$,                                \      !BCSK
               CIPOK.REPORT.THISWEEK.FLAG$,                         \      !CSP
               CIPOK.FILLER$

      READ.CIPOK = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CIPOK.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CIPOK.LOCK                                       */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CIPOK.LOCK PUBLIC

      INTEGER*2 READ.CIPOK.LOCK

      READ.CIPOK.LOCK = 1

      IF END #CIPOK.SESS.NUM% THEN READ.ERROR
      READ FORM "C4,C4,C8,C1,C8,C6,C1,C3,C1,C1,C8,C6,C1,C8,C6,C1,C1,C12";  \  !CSP
           #CIPOK.SESS.NUM% AUTOLOCK, 1;                            \
               CIPOK.STORE$,                                        \
               CIPOK.SER.NO$,                                       \
               CIPOK.DATE$,                                         \
               CIPOK.PSD66.RUNFLAG$,                                \
               CIPOK.PSD67.RUNDATE$,                                \
               CIPOK.PSD67.RUNTIME$,                                \
               CIPOK.PSD67.RUNFLAG$,                                \
               CIPOK.MARKDOWN.DAYS$,                                \
               CIPOK.CIP.STATUS$,                                   \
               CIPOK.THISWEEK.FLAG$,                                \
               CIPOK.PSD69.RUNDATE$,                                \
               CIPOK.PSD69.RUNTIME$,                                \
               CIPOK.PSD69.RUNFLAG$,                                \
               CIPOK.PSD76.RUNDATE$,                                \      !BCSK
               CIPOK.PSD76.RUNTIME$,                                \      !BCSK
               CIPOK.PSD76.RUNFLAG$,                                \      !BCSK
               CIPOK.REPORT.THISWEEK.FLAG$,                         \      !CSP
               CIPOK.FILLER$

      READ.CIPOK.LOCK = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CIPOK.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CIPOK                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CIPOK PUBLIC

      INTEGER*2 WRITE.CIPOK

      WRITE.CIPOK = 1

      IF END #CIPOK.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C4,C4,C8,C1,C8,C6,C1,C3,C1,C1,C8,C6,C1,C8,C6,C1,C1,C12";  \ !CSP
           #CIPOK.SESS.NUM%, 1;                                     \
               CIPOK.STORE$,                                        \
               CIPOK.SER.NO$,                                       \
               CIPOK.DATE$,                                         \
               CIPOK.PSD66.RUNFLAG$,                                \
               CIPOK.PSD67.RUNDATE$,                                \
               CIPOK.PSD67.RUNTIME$,                                \
               CIPOK.PSD67.RUNFLAG$,                                \
               CIPOK.MARKDOWN.DAYS$,                                \
               CIPOK.CIP.STATUS$,                                   \
               CIPOK.THISWEEK.FLAG$,                                \
               CIPOK.PSD69.RUNDATE$,                                \
               CIPOK.PSD69.RUNTIME$,                                \
               CIPOK.PSD69.RUNFLAG$,                                \
               CIPOK.PSD76.RUNDATE$,                                \      !BCSK
               CIPOK.PSD76.RUNTIME$,                                \      !BCSK
               CIPOK.PSD76.RUNFLAG$,                                \      !BCSK
               CIPOK.REPORT.THISWEEK.FLAG$,                         \      !CSP
               CIPOK.FILLER$

      WRITE.CIPOK = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CIPOK.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CIPOK.UNLOCK                                    */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CIPOK.UNLOCK PUBLIC

      INTEGER*2 WRITE.CIPOK.UNLOCK

      WRITE.CIPOK.UNLOCK = 1

      IF END #CIPOK.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C4,C4,C8,C1,C8,C6,C1,C3,C1,C1,C8,C6,C1,C8,C6,C1,C1,C12";  \ !CSP
           #CIPOK.SESS.NUM% AUTOUNLOCK, 1;                          \
               CIPOK.STORE$,                                        \
               CIPOK.SER.NO$,                                       \
               CIPOK.DATE$,                                         \
               CIPOK.PSD66.RUNFLAG$,                                \
               CIPOK.PSD67.RUNDATE$,                                \
               CIPOK.PSD67.RUNTIME$,                                \
               CIPOK.PSD67.RUNFLAG$,                                \
               CIPOK.MARKDOWN.DAYS$,                                \
               CIPOK.CIP.STATUS$,                                   \
               CIPOK.THISWEEK.FLAG$,                                \
               CIPOK.PSD69.RUNDATE$,                                \
               CIPOK.PSD69.RUNTIME$,                                \
               CIPOK.PSD69.RUNFLAG$,                                \
               CIPOK.PSD76.RUNDATE$,                                \      !BCSK
               CIPOK.PSD76.RUNTIME$,                                \      !BCSK
               CIPOK.PSD76.RUNFLAG$,                                \      !BCSK
               CIPOK.REPORT.THISWEEK.FLAG$,                         \      !CSP
               CIPOK.FILLER$

      WRITE.CIPOK.UNLOCK = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CIPOK.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CIPOK.UNLOCK                                    */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CREATE.CIPOK PUBLIC

      INTEGER*2 CREATE.CIPOK
      INTEGER*2 rc%

      CREATE.CIPOK = 1

      CIPOK.STORE$         = "0000"
      CIPOK.SER.NO$        = "0000"
      CIPOK.DATE$          = "00000000"
      CIPOK.PSD66.RUNFLAG$ = " "
      CIPOK.PSD67.RUNDATE$ = "00000000"
      CIPOK.PSD67.RUNTIME$ = "000000"
      CIPOK.PSD67.RUNFLAG$ = " "
      CIPOK.MARKDOWN.DAYS$ = "000"
      CIPOK.CIP.STATUS$    = " "
      CIPOK.THISWEEK.FLAG$ = "N"
      CIPOK.PSD69.RUNDATE$ = "00000000"
      CIPOK.PSD69.RUNTIME$ = "000000"
      CIPOK.PSD69.RUNFLAG$ = " "
      CIPOK.PSD76.RUNDATE$ = "00000000"                                     !BCSK
      CIPOK.PSD76.RUNTIME$ = "000000"                                       !BCSK
      CIPOK.PSD76.RUNFLAG$ = " "                                            !BCSK
      CIPOK.REPORT.THISWEEK.FLAG$ = "N"                                     !CSP
      CIPOK.FILLER$        = STRING$(13, " ")                               !BCSK

      IF END #CIPOK.SESS.NUM% THEN CREATE.ERROR
      CREATE POSFILE CIPOK.FILE.NAME$ DIRECT 1 RECL CIPOK.RECL%     \
          AS CIPOK.SESS.NUM% MIRRORED PERUPDATE

      rc% = WRITE.CIPOK

      IF rc% = 0 THEN BEGIN
         CREATE.CIPOK = 0
      ENDIF

      EXIT FUNCTION

CREATE.ERROR:

      FILE.OPERATION$ = "C"
      CURRENT.REPORT.NUM% = CIPOK.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/