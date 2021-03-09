\********************************************************************
\***      Space and Range Planogram descriptors (SRPDF)
\***      Version A           Neil Bennett          07.08.2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  %INCLUDE SRPDFDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRPDF.KEY$

   STRING     BUILD.SRPDF.KEY$
   STRING     work$

   work$ = STRING$(4,CHR$(0))

   CALL PUTN4(work$, 0, SRPDF.POGDB%)

   BUILD.SRPDF.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION SRPDF.SET PUBLIC

   SRPDF.REPORT.NUM%   = 733
   SRPDF.RECL%         = 169
   SRPDF.FILE.NAME$    = "SRPDF"

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRPDF PUBLIC

   INTEGER*2 READ.SRPDF
   STRING    key$

   READ.SRPDF = 1

   key$ = BUILD.SRPDF.KEY$

   IF END #SRPDF.SESS.NUM% THEN READ.SRPDF.ERROR

   READ FORM "T5,C50,C100,C15";                                     \
          #SRPDF.SESS.NUM% KEY key$;                                \
             SRPDF.SHRT.DESC$,                                      \
             SRPDF.FULL.DESC$,                                      \
             SRPDF.FILLER$

   READ.SRPDF = 0
   EXIT FUNCTION

READ.SRPDF.ERROR:

   CURRENT.REPORT.NUM% = SRPDF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRPDF PUBLIC

   INTEGER*2 WRITE.SRPDF
   STRING    key$

   WRITE.SRPDF = 1

   key$ = BUILD.SRPDF.KEY$

   IF END #SRPDF.SESS.NUM% THEN WRITE.SRPDF.ERROR

   WRITE FORM "C4,C50,C100,C15";                                    \
           #SRPDF.SESS.NUM%;                                        \
             key$,                                                  \
             SRPDF.SHRT.DESC$,                                      \
             SRPDF.FULL.DESC$,                                      \
             SRPDF.FILLER$

   WRITE.SRPDF = 0
   EXIT FUNCTION

WRITE.SRPDF.ERROR:

   CURRENT.REPORT.NUM% = SRPDF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION DELREC.SRPDF PUBLIC

   INTEGER*2 DELREC.SRPDF
   STRING    key$

   DELREC.SRPDF = 1

   key$ = BUILD.SRPDF.KEY$

   IF END #SRPDF.SESS.NUM% THEN DELREC.SRPDF.ERROR

   DELREC SRPDF.SESS.NUM%; key$

   DELREC.SRPDF = 0
   EXIT FUNCTION

DELREC.SRPDF.ERROR:

   CURRENT.REPORT.NUM% = SRPDF.REPORT.NUM%
   FILE.OPERATION$ = "D"
   CURRENT.CODE$ = key$

  END FUNCTION
