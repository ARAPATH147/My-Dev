\********************************************************************
\***      Space and Range Planogram Live Item database (SRITL)
\***      Version A           Neil Bennett          07.07.2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  STRING                             \
      SRITL.ARRAY$

  %INCLUDE SRITLDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRITL.KEY$

   STRING   BUILD.SRITL.KEY$
   STRING   work$

   work$ = SRITL.ITEM.CODE$ + CHR$(SRITL.RECORD.CHAIN%)

   BUILD.SRITL.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  SUB SRITL.SPLIT.ARRAY

   INTEGER*2 i%

   FOR i% = 0 TO SRITL.MAX.MOD.KEYS% -1

      SRITL.POGDB%(i%)      = GETN4(SRITL.ARRAY$, i%*7 +0)
      SRITL.MODULE.SEQ%(i%) = GETN1(SRITL.ARRAY$, i%*7 +4)
      SRITL.REPEAT.CNT%(i%) = GETN1(SRITL.ARRAY$, i%*7 +5)
      SRITL.CORE.FLAG$(i%)  = MID$ (SRITL.ARRAY$, i%*7 +7, 1)

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  SUB SRITL.CONCAT.ARRAY

   INTEGER*2 i%

   SRITL.ARRAY$ = STRING$(7, CHR$(00))
   SRITL.ARRAY$ = STRING$(SRITL.MAX.MOD.KEYS%, SRITL.ARRAY$)

   FOR i% = 0 TO SRITL.MAX.MOD.KEYS% -1

      CALL PUTN4(SRITL.ARRAY$, i%*7 +0, SRITL.POGDB%(i%))
      CALL PUTN1(SRITL.ARRAY$, i%*7 +4, SRITL.MODULE.SEQ%(i%))
      CALL PUTN1(SRITL.ARRAY$, i%*7 +5, SRITL.REPEAT.CNT%(i%))
      CALL PUTN1(SRITL.ARRAY$, i%*7 +6, ASC(SRITL.CORE.FLAG$(i%)    \
                                                           + "?"))

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  FUNCTION SRITL.SET PUBLIC

   SRITL.REPORT.NUM%   = 721
   SRITL.RECL%         = 127
   SRITL.FILE.NAME$    = "SRITML"
   SRITL.COPY.NAME$    = "ADXLXACN::D:\ADX_UDT3\SRITEML.BAK"
   SRITL.MAX.MOD.KEYS% = 16

   DIM SRITL.POGDB%      (SRITL.MAX.MOD.KEYS%-1)
   DIM SRITL.MODULE.SEQ% (SRITL.MAX.MOD.KEYS%-1)
   DIM SRITL.REPEAT.CNT% (SRITL.MAX.MOD.KEYS%-1)
   DIM SRITL.CORE.FLAG$  (SRITL.MAX.MOD.KEYS%-1)

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRITL PUBLIC

   INTEGER*2 READ.SRITL
   STRING    key$

   READ.SRITL = 1

   IF END #SRITL.SESS.NUM% THEN READ.SRITL.ERROR

   key$ = BUILD.SRITL.KEY$

   READ FORM "T5,I1,I2,I2,C112,C6";                                 \
          #SRITL.SESS.NUM% KEY key$;                                \
             SRITL.MODULE.COUNT%,                                   \
             SRITL.CORE.COUNT%,                                     \
             SRITL.NON.CORE.CNT%,                                   \
             SRITL.ARRAY$,                                          \
             SRITL.FILLER$

   CALL SRITL.SPLIT.ARRAY

   READ.SRITL = 0
   EXIT FUNCTION

READ.SRITL.ERROR:

   CURRENT.REPORT.NUM% = SRITL.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRITL PUBLIC

   INTEGER*2 WRITE.SRITL
   STRING    key$

   WRITE.SRITL = 1

   IF END #SRITL.SESS.NUM% THEN WRITE.SRITL.ERROR

   key$ = BUILD.SRITL.KEY$
   CALL SRITL.CONCAT.ARRAY

   WRITE FORM "C4,I1,I2,I2,C112,C6";                                \
          #SRITL.SESS.NUM%;                                         \
             key$,                                                  \
             SRITL.MODULE.COUNT%,                                   \
             SRITL.CORE.COUNT%,                                     \
             SRITL.NON.CORE.CNT%,                                   \
             SRITL.ARRAY$,                                          \
             SRITL.FILLER$

   WRITE.SRITL = 0
   EXIT FUNCTION

WRITE.SRITL.ERROR:

   CURRENT.REPORT.NUM% = SRITL.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION
