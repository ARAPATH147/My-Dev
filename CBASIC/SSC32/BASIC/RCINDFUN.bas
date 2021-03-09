
\******************************************************************************
\******************************************************************************
\***                                                                
\***    RCINDX File Functions
\***                                                               
\***    REFERENCE   : RCINDFUN.BAS
\***                                                                
\***    FILE TYPE   : Direct
\***
\******************************************************************************
\******************************************************************************
\***                                                                
\***    VERSION A.          Neil Bennett.                4 June 2007
\***                                                                
\***    REVISION 1.1.                ROBERT COWEY.                23 MAY 2008.
\***    Defined variables EXPIRY.DATE$ and MRQ$ and increased record length 
\***    to 48 bytes.
\***
\******************************************************************************
\******************************************************************************
                                                                       

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE RCINDDEC.J86

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\*** FUNCTION RCINDX.SET                                            */
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION RCINDX.SET PUBLIC

      INTEGER*1 RCINDX.SET

      RCINDX.REPORT.NUM% = 746
      RCINDX.RECL%       = 48                                          ! 1.1 RC
      RCINDX.FILE.NAME$  = "RCINDX"

   END FUNCTION

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\*** FUNCTION READ.RCINDX (Direct Read)                             */
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.RCINDX PUBLIC

      INTEGER*2 READ.RCINDX
      INTEGER*2 indx%

      READ.RCINDX = 1
      indx% = VAL(UNPACK$(RCINDX.RECALL.INDEX.NUM$))

      IF END #RCINDX.SESS.NUM% THEN READ.ERROR
      READ FORM "C2,C4,C20,C4,C1,C2,C1,C1,C2,C4,C2,C5"; \              ! 1.1 RC
           #RCINDX.SESS.NUM%, indx%;                \
               RCINDX.RECALL.INDEX.NUM$,            \
               RCINDX.RECALL.REFERENCE$,            \
               RCINDX.RECALL.DESCRIPTION$,          \
               RCINDX.ACTIVE.DATE$,                 \
               RCINDX.RECALL.TYPE$,                 \
               RCINDX.ITEM.COUNT$,                  \
               RCINDX.RECALL.STATUS$,               \
               RCINDX.RECALL.SPECIAL.INSTRUCTION$,  \
               RCINDX.RECALL.LABEL.TYPE$,           \
               RCINDX.EXPIRY.DATE$,                 \                  ! 1.1 RC
               RCINDX.MRQ$,                         \                  ! 1.1 RC
               RCINDX.FILLER$                     

      READ.RCINDX = 0

   EXIT FUNCTION

READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = RCINDX.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\*** FUNCTION WRITE.RCINDX (Direct Write)                           */
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.RCINDX PUBLIC

      INTEGER*2 WRITE.RCINDX
      INTEGER*2 indx%

      WRITE.RCINDX = 1

      indx% = VAL(UNPACK$(RCINDX.RECALL.INDEX.NUM$))
      RCINDX.FILLER$ = STRING$(5, " ")

      IF END #RCINDX.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C2,C4,C20,C4,C1,C2,C1,C1,C2,C4,C2,C5"; \             ! 1.1 RC
           #RCINDX.SESS.NUM%, indx%;                \
               RCINDX.RECALL.INDEX.NUM$,            \
               RCINDX.RECALL.REFERENCE$,            \
               RCINDX.RECALL.DESCRIPTION$,          \
               RCINDX.ACTIVE.DATE$,                 \
               RCINDX.RECALL.TYPE$,                 \
               RCINDX.ITEM.COUNT$,                  \
               RCINDX.RECALL.STATUS$,               \
               RCINDX.RECALL.SPECIAL.INSTRUCTION$,  \
               RCINDX.RECALL.LABEL.TYPE$,           \
               RCINDX.EXPIRY.DATE$,                 \                  ! 1.1 RC
               RCINDX.MRQ$,                         \                  ! 1.1 RC
               RCINDX.FILLER$                     

      WRITE.RCINDX = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = RCINDX.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\*** FUNCTION READ.RCINDX.RCD (Sequential Read)                     */
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.RCINDX.RCD PUBLIC

      INTEGER*2 READ.RCINDX.RCD

      READ.RCINDX.RCD = 1

      IF END #RCINDX.SESS.NUM% THEN READ.RCD.ERROR
      READ FORM "C2,C46";                                           \  ! 1.1 RC
           #RCINDX.SESS.NUM%;                                       \
               RCINDX.RECALL.INDEX.NUM$,                            \
               RCINDX.RCD$

      READ.RCINDX.RCD = 0
      EXIT FUNCTION

READ.RCD.ERROR:

      FILE.OPERATION$ = "R"
      CURRENT.REPORT.NUM% = RCINDX.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\*** FUNCTION WRITE.RCINDX.RCD (Sequential Write)                   */
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.RCINDX.RCD PUBLIC

      INTEGER*2 WRITE.RCINDX.RCD

      WRITE.RCINDX.RCD = 1

      IF END #RCINDX.SESS.NUM% THEN WRITE.RCD.ERROR
      WRITE FORM "C2,C46";                                          \  ! 1.1 RC
           #RCINDX.SESS.NUM%;                                       \
               RCINDX.RECALL.INDEX.NUM$,                            \
               RCINDX.RCD$

      WRITE.RCINDX.RCD = 0
      EXIT FUNCTION

WRITE.RCD.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = RCINDX.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION

