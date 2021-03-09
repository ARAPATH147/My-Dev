\********************************************************************
\***      Space and Range Planogram Pending Item database (SRITP)
\***      Version A           Neil Bennett          11.07.2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  STRING                             \
      SRITP.ARRAY$

  %INCLUDE SRITPDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRITP.KEY$

   STRING   BUILD.SRITP.KEY$
   STRING   work$

   work$ = SRITP.ITEM.CODE$ + CHR$(SRITP.RECORD.CHAIN%)

   BUILD.SRITP.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  SUB SRITP.SPLIT.ARRAY

   INTEGER*2 i%

   FOR i% = 0 TO SRITP.MAX.MOD.KEYS% -1

      SRITP.POGDB%(i%)      = GETN4(SRITP.ARRAY$, i%*7 +0)
      SRITP.MODULE.SEQ%(i%) = GETN1(SRITP.ARRAY$, i%*7 +4)
      SRITP.REPEAT.CNT%(i%) = GETN1(SRITP.ARRAY$, i%*7 +5)
      SRITP.CORE.FLAG$(i%)  = MID$ (SRITP.ARRAY$, i%*7 +7, 1)

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  SUB SRITP.CONCAT.ARRAY

   INTEGER*2 i%

   SRITP.ARRAY$ = STRING$(7, CHR$(00))
   SRITP.ARRAY$ = STRING$(SRITP.MAX.MOD.KEYS%, SRITP.ARRAY$)

   FOR i% = 0 TO SRITP.MAX.MOD.KEYS% -1

      CALL PUTN4(SRITP.ARRAY$, i%*7 +0, SRITP.POGDB%(i%))
      CALL PUTN1(SRITP.ARRAY$, i%*7 +4, SRITP.MODULE.SEQ%(i%))
      CALL PUTN1(SRITP.ARRAY$, i%*7 +5, SRITP.REPEAT.CNT%(i%))
      CALL PUTN1(SRITP.ARRAY$, i%*7 +6, ASC(SRITP.CORE.FLAG$(i%)    \
                                                           + "?"))

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  FUNCTION SRITP.SET PUBLIC

   SRITP.REPORT.NUM%   = 722
   SRITP.RECL%         = 127
   SRITP.FILE.NAME$    = "SRITMP"
   SRITP.COPY.NAME$    = "ADXLXACN::D:\ADX_UDT3\SRITEMP.BAK"
   SRITP.MAX.MOD.KEYS% = 16

   DIM SRITP.POGDB%      (SRITP.MAX.MOD.KEYS%-1)
   DIM SRITP.MODULE.SEQ% (SRITP.MAX.MOD.KEYS%-1)
   DIM SRITP.REPEAT.CNT% (SRITP.MAX.MOD.KEYS%-1)
   DIM SRITP.CORE.FLAG$  (SRITP.MAX.MOD.KEYS%-1)

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRITP PUBLIC

   INTEGER*2 READ.SRITP
   STRING    key$

   READ.SRITP = 1

   IF END #SRITP.SESS.NUM% THEN READ.SRITP.ERROR

   key$ = BUILD.SRITP.KEY$

   READ FORM "T5,I1,I2,I2,C112,C6";                                 \
          #SRITP.SESS.NUM% KEY key$;                                \
             SRITP.MODULE.COUNT%,                                   \
             SRITP.CORE.COUNT%,                                     \
             SRITP.NON.CORE.CNT%,                                   \
             SRITP.ARRAY$,                                          \
             SRITP.FILLER$

   CALL SRITP.SPLIT.ARRAY

   READ.SRITP = 0
   EXIT FUNCTION

READ.SRITP.ERROR:

   CURRENT.REPORT.NUM% = SRITP.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRITP PUBLIC

   INTEGER*2 WRITE.SRITP
   STRING    key$

   WRITE.SRITP = 1

   IF END #SRITP.SESS.NUM% THEN WRITE.SRITP.ERROR

   key$ = BUILD.SRITP.KEY$
   CALL SRITP.CONCAT.ARRAY

   WRITE FORM "C4,I1,I2,I2,C112,C6";                                \
          #SRITP.SESS.NUM%;                                         \
             key$,                                                  \
             SRITP.MODULE.COUNT%,                                   \
             SRITP.CORE.COUNT%,                                     \
             SRITP.NON.CORE.CNT%,                                   \
             SRITP.ARRAY$,                                          \
             SRITP.FILLER$

   WRITE.SRITP = 0
   EXIT FUNCTION

WRITE.SRITP.ERROR:

   CURRENT.REPORT.NUM% = SRITP.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION
