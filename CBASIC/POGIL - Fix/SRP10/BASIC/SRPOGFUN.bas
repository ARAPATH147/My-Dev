\********************************************************************
\***      Space and Range Planogram database (SRPOG)
\***      Version A           Neil Bennett          05.06.2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  %INCLUDE SRPOGDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRPOG.KEY$

   STRING     BUILD.SRPOG.KEY$
   STRING     work$

   work$ = STRING$(4,CHR$(0))

   CALL PUTN4(work$, 0, SRPOG.POGDB%)

   BUILD.SRPOG.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION SRPOG.SET PUBLIC

   SRPOG.REPORT.NUM%   = 719
   SRPOG.RECL%         = 101
   SRPOG.FILE.NAME$    = "SRPOG"
   SRPOG.COPY.NAME$    = "ADXLXACN::D:\ADX_UDT3\SRPOG.BAK"

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRPOG PUBLIC

   INTEGER*2 READ.SRPOG
   STRING    key$

   READ.SRPOG = 1

   key$ = BUILD.SRPOG.KEY$

   IF END #SRPOG.SESS.NUM% THEN READ.SRPOG.ERROR

   READ FORM "T5,I4,C4,C4,C30,C30,I1,I4,I1,I1,C4,I1,I4,I4,I4,C1";   \
          #SRPOG.SESS.NUM% KEY key$;                                \
             SRPOG.POGID%,                                          \
             SRPOG.ACT.DATE$,                                       \
             SRPOG.DEACT.DATE$,                                     \
             SRPOG.DESCRIPTION$,                                    \
             SRPOG.PLANNER.FAMILY$,                                 \
             SRPOG.MODULE.COUNT%,                                   \
             SRPOG.CAT.DBKEY%,                                      \
             SRPOG.KEY.LEVEL%,                                      \
             SRPOG.LIVE.RPT.CNT%,                                   \
             SRPOG.DATE.RPT.CNT$,                                   \
             SRPOG.PEND.RPT.CNT%,                                   \
             SRPOG.CAT1.ID%,                                        \
             SRPOG.CAT2.ID%,                                        \
             SRPOG.CAT3.ID%,                                        \
             SRPOG.FILLER$

   READ.SRPOG = 0
   EXIT FUNCTION

READ.SRPOG.ERROR:

   CURRENT.REPORT.NUM% = SRPOG.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRPOG PUBLIC

   INTEGER*2 WRITE.SRPOG
   STRING    key$

   WRITE.SRPOG = 1

   key$ = BUILD.SRPOG.KEY$

   IF END #SRPOG.SESS.NUM% THEN WRITE.SRPOG.ERROR

   WRITE FORM "C4,I4,C4,C4,C30,C30,I1,I4,I1,I1,C4,I1,I4,I4,I4,C1";  \
           #SRPOG.SESS.NUM%;                                        \
             key$,                                                  \
             SRPOG.POGID%,                                          \
             SRPOG.ACT.DATE$,                                       \
             SRPOG.DEACT.DATE$,                                     \
             SRPOG.DESCRIPTION$,                                    \
             SRPOG.PLANNER.FAMILY$,                                 \
             SRPOG.MODULE.COUNT%,                                   \
             SRPOG.CAT.DBKEY%,                                      \
             SRPOG.KEY.LEVEL%,                                      \
             SRPOG.LIVE.RPT.CNT%,                                   \
             SRPOG.DATE.RPT.CNT$,                                   \
             SRPOG.PEND.RPT.CNT%,                                   \
             SRPOG.CAT1.ID%,                                        \
             SRPOG.CAT2.ID%,                                        \
             SRPOG.CAT3.ID%,                                        \
             SRPOG.FILLER$

   WRITE.SRPOG = 0
   EXIT FUNCTION

WRITE.SRPOG.ERROR:

   CURRENT.REPORT.NUM% = SRPOG.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION DELREC.SRPOG PUBLIC

   INTEGER*2 DELREC.SRPOG
   STRING    key$

   DELREC.SRPOG = 1

   IF END #SRPOG.SESS.NUM% THEN DELREC.SRPOG.ERROR

   key$ = BUILD.SRPOG.KEY$

   DELREC SRPOG.SESS.NUM%; key$

   DELREC.SRPOG = 0
   EXIT FUNCTION

DELREC.SRPOG.ERROR:

   CURRENT.REPORT.NUM% = SRPOG.REPORT.NUM%
   FILE.OPERATION$ = "D"
   CURRENT.CODE$ = key$

  END FUNCTION
