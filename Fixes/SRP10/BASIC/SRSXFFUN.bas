\********************************************************************
\***      Space and Range Planogram database (SRSXF)
\***      Version A           Neil Bennett          05.06.2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  %INCLUDE SRSXFDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRSXF.KEY$

   STRING   BUILD.SRSXF.KEY$
   STRING   work$

   work$ = STRING$(6,CHR$(0))

   CALL PUTN4(work$, 0, SRSXF.POGDB%)
   CALL PUTN1(work$, 4, SRSXF.MODULE.SEQ%)
   CALL PUTN1(work$, 5, SRSXF.SHELF.NO%)

   BUILD.SRSXF.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION SRSXF.SET PUBLIC

   SRSXF.REPORT.NUM%   = 729
   SRSXF.RECL%         = 63
   SRSXF.FILE.NAME$    = "SRSXF"

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRSXF PUBLIC

   INTEGER*2 READ.SRSXF
   STRING    key$

   READ.SRSXF = 1

   IF END #SRSXF.SESS.NUM% THEN READ.SRSXF.ERROR

   key$ = BUILD.SRSXF.KEY$

   READ FORM "T7,I1,I4,C50,C2";                                     \
          #SRSXF.SESS.NUM% KEY key$;                                \
             SRSXF.NOTCH.NO%,                                       \
             SRSXF.SHELF.KEY%,                                      \
             SRSXF.SHELF.DESC$,                                     \
             SRSXF.FILLER$

   READ.SRSXF = 0
   EXIT FUNCTION

READ.SRSXF.ERROR:

   CURRENT.REPORT.NUM% = SRSXF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRSXF PUBLIC

   INTEGER*2 WRITE.SRSXF
   STRING    key$

   WRITE.SRSXF = 1

   IF END #SRSXF.SESS.NUM% THEN WRITE.SRSXF.ERROR

   key$ = BUILD.SRSXF.KEY$

   WRITE FORM "C6,I1,I4,C50,C2";                                    \
           #SRSXF.SESS.NUM%;                                        \
             key$,                                                  \
             SRSXF.NOTCH.NO%,                                       \
             SRSXF.SHELF.KEY%,                                      \
             SRSXF.SHELF.DESC$,                                     \
             SRSXF.FILLER$

   WRITE.SRSXF = 0
   EXIT FUNCTION

WRITE.SRSXF.ERROR:

   CURRENT.REPORT.NUM% = SRSXF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION DELREC.SRSXF PUBLIC

   INTEGER*2 DELREC.SRSXF
   STRING    key$

   DELREC.SRSXF = 1

   IF END #SRSXF.SESS.NUM% THEN DELREC.SRSXF.ERROR

   key$ = BUILD.SRSXF.KEY$

   DELREC SRSXF.SESS.NUM%; key$

   DELREC.SRSXF = 0
   EXIT FUNCTION

DELREC.SRSXF.ERROR:

   CURRENT.REPORT.NUM% = SRSXF.REPORT.NUM%
   FILE.OPERATION$ = "D"
   CURRENT.CODE$ = key$

  END FUNCTION
