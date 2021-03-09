\/******************************************************************/
\/*                                                                */
\/* Advanced Shipping Notification (ASN) FILE FUNCTIONS            */
\/*                                                                */
\/* REFERENCE   : ASNFUN.BAS                                       */
\/*                                                                */
\/* VERSION A.          Neil Bennett.           12 DECEMBER 2006   */
\/*                                                                */
\/* VERSION B           Harpal Matharu          28 November 2007   */
\/*                                                                */
\/* VERSION C           Stuart Highley          11 July 2008       */
\/* Reconverted 1-byte to 2-byte integer.                          */
\/                                                                 */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE ASNDEC.J86

   %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION GET.ASN.KEY                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION GET.ASN.KEY$

      STRING    GET.ASN.KEY$
      STRING    work$

      work$ = ASN.NO$ + STRING$(2, CHR$(0))
      CALL PUTN2(work$, 18, ASN.CHAIN%)

      GET.ASN.KEY$ = work$
      work$ = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION ASN.SET                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION ASN.SET PUBLIC

      INTEGER*1 ASN.SET

      ASN.REPORT.NUM% = 734
      ASN.RECL%       = 127
      ASN.FILE.NAME$  = "ASN"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.ASN                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.ASN PUBLIC

      INTEGER*2 READ.ASN
      INTEGER*2 i%
      STRING    ASN.KEY$
      STRING    work$

      READ.ASN = 1

      ASN.KEY$ = GET.ASN.KEY$

      DIM ASN.CRTN.NO$(20)

      IF END #ASN.SESS.NUM% THEN READ.ERROR
      READ FORM "T21,C3,I1,C80,C23";                                \
           #ASN.SESS.NUM%                                           \
           KEY ASN.KEY$;                                            \
               ASN.SUP.REF$,                                        \
               ASN.TOT.CNT%,                                        \
               work$,                                               \
               ASN.FILLER$

      ! Converts single byte to 2 byte integer                      ! CSH
      IF ASN.TOT.CNT% < 0 THEN ASN.TOT.CNT% = ASN.TOT.CNT% + 256    ! CSH

      FOR i% = 0 TO 19
         ASN.CRTN.NO$(i% +1) = MID$(work$, (i%*4) +1, 4)
      NEXT i%

      READ.ASN = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = ASN.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.ASN                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.ASN PUBLIC

      INTEGER*2 WRITE.ASN
      INTEGER*2 i%
      STRING    ASN.KEY$
      STRING    work$

      WRITE.ASN = 1

      ASN.KEY$ = GET.ASN.KEY$

      work$ = ""
      FOR i% = 1 TO 20
         work$ = work$                                              \
               + RIGHT$(STRING$(4,CHR$(0)) + ASN.CRTN.NO$(i%), 4)
      NEXT i%

      IF END #ASN.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C20,C3,I1,C80,C23";                               \
           #ASN.SESS.NUM%;                                          \
               ASN.KEY$,                                            \
               ASN.SUP.REF$,                                        \
               ASN.TOT.CNT%,                                        \
               work$,                                               \
               ASN.FILLER$

      WRITE.ASN = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = ASN.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/

FUNCTION DELETE.ASN PUBLIC

      INTEGER*2 DELETE.ASN
      INTEGER*2 i%
      STRING    ASN.KEY$
      STRING    work$

      DELETE.ASN = 1

      ASN.KEY$ = GET.ASN.KEY$
      DELREC ASN.SESS.NUM%;ASN.KEY$

      DELETE.ASN = 0
   EXIT FUNCTION


DELETE.ERROR:

      FILE.OPERATION$     = "D"
      CURRENT.REPORT.NUM% = ASN.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION
