\********************************************************************
\***      Space and Range Planogram database (SRMOD)
\***      Version A           Neil Bennett          6th June 2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  %INCLUDE SRMODDEC.J86

\--------------------------------------------------------------------

  %INCLUDE BTCMEM.J86
  %INCLUDE BTCSTR.J86

\--------------------------------------------------------------------

  FUNCTION BUILD.SRMOD.KEY$

   STRING   BUILD.SRMOD.KEY$
   STRING   work$

   work$ = STRING$(6,CHR$(0))

   CALL PUTN4(work$, 0, SRMOD.POGDB%)
   CALL PUTN1(work$, 4, SRMOD.MODULE.SEQ%)
   CALL PUTN1(work$, 5, SRMOD.RECORD.CHAIN%)

   BUILD.SRMOD.KEY$ = work$
   work$ = ""

  END FUNCTION

\--------------------------------------------------------------------

  SUB SRMOD.SPLIT.SHELF.ITEM PUBLIC

   INTEGER*2 i%

   FOR i% = 0 TO SRMOD.MAX.ITEMS% - 1

      SRMOD.SHELF.NUM%(i%) = GETN1(SRMOD.ARRAY$, i%*9 +0)
      SRMOD.FACINGS%(i%)   = GETN1(SRMOD.ARRAY$, i%*9 +1)
      SRMOD.ITEM.CODE$(i%) = MID$ (SRMOD.ARRAY$, i%*9 +3,3)
      SRMOD.MDQ%(i%)       = GETN2(SRMOD.ARRAY$, i%*9 +5)
      SRMOD.PSC%(i%)       = GETN2(SRMOD.ARRAY$, i%*9 +7)

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  SUB SRMOD.CONCAT.SHELF.ITEM

   INTEGER*2 i%

   SRMOD.ARRAY$ = STRING$(9, CHR$(00))
   SRMOD.ARRAY$ = STRING$(SRMOD.MAX.ITEMS%, SRMOD.ARRAY$)

   FOR i% = 0 TO SRMOD.MAX.ITEMS% - 1

      CALL PUTN1 (SRMOD.ARRAY$, i%*9 +0, SRMOD.SHELF.NUM%(i%))
      CALL PUTN1 (SRMOD.ARRAY$, i%*9 +1, SRMOD.FACINGS%(i%))
      CALL SUBSTR(SRMOD.ARRAY$, i%*9 +2, SRMOD.ITEM.CODE$(i%),0,3)
      CALL PUTN2 (SRMOD.ARRAY$, i%*9 +5, SRMOD.MDQ%(i%))
      CALL PUTN2 (SRMOD.ARRAY$, i%*9 +7, SRMOD.PSC%(i%))

   NEXT i%

  END SUB

\--------------------------------------------------------------------

  FUNCTION SRMOD.SET PUBLIC

   SRMOD.REPORT.NUM%   = 727
   SRMOD.RECL%         = 508
   SRMOD.FILE.NAME$    = "SRMOD"
   SRMOD.COPY.NAME$    = "ADXLXACN::D:\ADX_UDT3\SRMOD.BAK"
   SRMOD.MAX.ITEMS%    = 50

   DIM SRMOD.SHELF.NUM% (SRMOD.MAX.ITEMS%-1)
   DIM SRMOD.FACINGS%   (SRMOD.MAX.ITEMS%-1)
   DIM SRMOD.ITEM.CODE$ (SRMOD.MAX.ITEMS%-1)
   DIM SRMOD.MDQ%       (SRMOD.MAX.ITEMS%-1)
   DIM SRMOD.PSC%       (SRMOD.MAX.ITEMS%-1)

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.SRMOD PUBLIC

   INTEGER*2 READ.SRMOD
   STRING    key$

   READ.SRMOD = 1

   IF END #SRMOD.SESS.NUM% THEN READ.SRMOD.ERROR

   key$ = BUILD.SRMOD.KEY$

   READ FORM "T7,C30,C450,I2,I2,C18"; #SRMOD.SESS.NUM%              \
         KEY key$;                                                  \
             SRMOD.DESCRIPTOR$,                                     \
             SRMOD.ARRAY$,                                          \
             SRMOD.SHELF.COUNT%,                                    \
             SRMOD.ITEM.COUNT%,                                     \
             SRMOD.FILLER$

   CALL SRMOD.SPLIT.SHELF.ITEM

   READ.SRMOD = 0

   EXIT FUNCTION

READ.SRMOD.ERROR:

   CURRENT.REPORT.NUM% = SRMOD.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.SRMOD PUBLIC

   INTEGER*2 WRITE.SRMOD
   STRING    key$

   WRITE.SRMOD = 1

   IF END #SRMOD.SESS.NUM% THEN WRITE.SRMOD.ERROR

   key$ = BUILD.SRMOD.KEY$
   CALL SRMOD.CONCAT.SHELF.ITEM

   WRITE FORM "C6,C30,C450,I2,I2,C18"; #SRMOD.SESS.NUM%;            \
             key$,                                                  \
             SRMOD.DESCRIPTOR$,                                     \
             SRMOD.ARRAY$,                                          \
             SRMOD.SHELF.COUNT%,                                    \
             SRMOD.ITEM.COUNT%,                                     \
             SRMOD.FILLER$

   WRITE.SRMOD = 0

   EXIT FUNCTION

WRITE.SRMOD.ERROR:

   CURRENT.REPORT.NUM% = SRMOD.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = key$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION DELREC.SRMOD PUBLIC

   INTEGER*2 DELREC.SRMOD
   STRING    key$

   DELREC.SRMOD = 1

   IF END #SRMOD.SESS.NUM% THEN DELREC.SRMOD.ERROR

   key$ = BUILD.SRMOD.KEY$

   DELREC SRMOD.SESS.NUM%; key$

   DELREC.SRMOD = 0
   EXIT FUNCTION

DELREC.SRMOD.ERROR:

   CURRENT.REPORT.NUM% = SRMOD.REPORT.NUM%
   FILE.OPERATION$ = "D"
   CURRENT.CODE$ = key$

  END FUNCTION
