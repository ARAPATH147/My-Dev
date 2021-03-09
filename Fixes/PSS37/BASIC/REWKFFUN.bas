\/******************************************************************/
\/*                                                                */
\/* Recalls Work (REWKF) FILE FUNCTIONS                            */
\/*                                                                */
\/* REFERENCE   : REWKFFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Brian Greenfield        11th may 2007      */
\/*                                                                */
\/* VERSION B.          Brian Greenfield        15th April 2008    */
\/*                                                                */
\/* VERSION C.          Brian Greenfield        6th May 2008       */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE REWKFDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION REWKF.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION REWKF.SET PUBLIC

      INTEGER*1 REWKF.SET

      REWKF.REPORT.NUM% = 743
      REWKF.FILE.NAME$  = "REWKF"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.REWKF                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.REWKF PUBLIC

      INTEGER*2 READ.REWKF

      READ.REWKF = 1

      IF END #REWKF.SESS.NUM% THEN READ.ERROR
      READ #REWKF.SESS.NUM%; LINE REWKF.RCD$

      REWKF.REC.TYPE$ = MID$(REWKF.RCD$,1,2)

      IF REWKF.REC.TYPE$ = "YH" THEN BEGIN
         REWKF.REFERENCE$   = MID$(REWKF.RCD$,3,8)
         REWKF.LABEL$       = MID$(REWKF.RCD$,11,14)
         REWKF.BATCH.TYPE$  = MID$(REWKF.RCD$,25,1) !BBG
         REWKF.MRQ$         = MID$(REWKF.RCD$,26,2) !BBG !CBG
         REWKF.DUE.BY.DATE$ = MID$(REWKF.RCD$,28,8) !BBG !CBG
      ENDIF ELSE IF REWKF.REC.TYPE$ = "YD" THEN BEGIN
         REWKF.BARCODE$     = MID$(REWKF.RCD$,3,13)
         REWKF.ITEM.CODE$   = MID$(REWKF.RCD$,16,7)
         REWKF.DESCRIPTION$ = MID$(REWKF.RCD$,23,20)
         REWKF.TSF$         = MID$(REWKF.RCD$,43,4)
      ENDIF ELSE IF REWKF.REC.TYPE$ = "YT" THEN BEGIN
         REWKF.ITEM.COUNT$  = MID$(REWKF.RCD$,3,5)
      ENDIF

      READ.REWKF = 0
   EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.REWKF                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.REWKF PUBLIC

      INTEGER*2 WRITE.REWKF
      
      WRITE.REWKF = 1

      IF REWKF.REC.TYPE$ = "YH" THEN BEGIN
         REWKF.RCD$ = REWKF.REC.TYPE$ \
                 + RIGHT$(STRING$(8,"0") + REWKF.REFERENCE$,8) \
                 + RIGHT$(STRING$(14,"0") + REWKF.LABEL$,14) \  !BBG
                 + RIGHT$("0" + REWKF.BATCH.TYPE$,1) \          !BBG
                 + RIGHT$(" " + REWKF.MRQ$,2) \                 !BBG !CBG
                 + RIGHT$(STRING$(8,"0") + REWKF.DUE.BY.DATE$,8)!BBG !CBG
      ENDIF ELSE IF REWKF.REC.TYPE$ = "YD" THEN BEGIN
         REWKF.RCD$ = REWKF.REC.TYPE$ \
                 + RIGHT$(STRING$(13,"0") + REWKF.BARCODE$,13) \
                 + RIGHT$(STRING$(7,"0") + REWKF.ITEM.CODE$,7) \
                 + LEFT$(REWKF.DESCRIPTION$ + STRING$(20," "),20) \
                 + RIGHT$(STRING$(4," ") + REWKF.TSF$,4)
      ENDIF ELSE IF REWKF.REC.TYPE$ = "YT" THEN BEGIN
         REWKF.RCD$ = REWKF.REC.TYPE$ \
                 + RIGHT$(STRING$(5,"0") + REWKF.ITEM.COUNT$,5)
      ENDIF

      IF END #REWKF.SESS.NUM% THEN WRITE.ERROR
      PRINT #REWKF.SESS.NUM%; REWKF.RCD$

      WRITE.REWKF = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = REWKF.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\/******************************************************************/