\********************************************************************
\***      ADCS POG Delta File (POGDE)
\***      Version A           Neil Bennett          6th June 2006
\***
\....................................................................

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$,               \
      POGDE.BLOCK$

  INTEGER*4 GLOBAL                   \
      POGDE.CTR%

  %INCLUDE POGDEDEC.J86

\--------------------------------------------------------------------

  FUNCTION INSTR(patt$,dest$,strt%)

   INTEGER*4 a%, INSTR, strt%
   STRING    patt$, dest$

   IF (strt% = 0) THEN BEGIN
      INSTR = 0
      EXIT FUNCTION
   ENDIF

   IF (LEN(patt$) > LEN(dest$)) THEN BEGIN
      INSTR = 0
      EXIT FUNCTION
   ENDIF

   patt$ = UCASE$(patt$)
   dest$ = UCASE$(dest$)

   FOR a% = strt% TO LEN(dest$) - LEN(patt$)
      IF (MID$(dest$,a%,LEN(patt$)) = patt$) THEN BEGIN
         INSTR = a%
         EXIT FUNCTION
      ENDIF
   NEXT a%

   INSTR = 0
   EXIT FUNCTION

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION POGDE.SET PUBLIC

   POGDE.REPORT.NUM% = 716
   POGDE.FILE.NAME$  = "POGDE"
   POGDE.COPY.NAME$  = "ADXLXACN::D:\ADX_UDT3\POGDELTA.BAK"

  END FUNCTION

\--------------------------------------------------------------------
\- Due to a problem encountered during testing where the READ#LINE
\- command was incorrectly recognising '0A2C' as an end of record,
\- the read was changed to the following where the file is read a
\- 1024 byte block ata a time and then parsed into records.
\-                                                   NWB 29/08/06
\--------------------------------------------------------------------
\- As it is possible to have 0D0Ah in the transmitted data stream
\- where this is not at the end of a record, a further length check
\- by record type has been added to allow this. Any unrecognised
\- record type is ignored.
\-                                                   NWB 17/01/07
\--------------------------------------------------------------------

  FUNCTION READ.POGDE PUBLIC

   INTEGER*2 READ.POGDE
   INTEGER*1 end.loop%
   INTEGER*2 i%
   STRING    a$, crlf$

   INTEGER*2 r.len.00
   INTEGER*2 r.len.01
   INTEGER*2 r.len.02
   INTEGER*2 r.len.03
   INTEGER*2 r.len.10
   INTEGER*2 r.len.21
   INTEGER*2 r.len.99

   READ.POGDE = 1

   POGDE.RCD$ = ""
   a$ = ""
   crlf$ = CHR$(13) + CHR$(10)
   end.loop% = 0

   ! Set up record lengths (incl CR/LF)

   r.len.00 =  14
   r.len.01 = 233
   r.len.02 =  15
   r.len.03 = 233
   r.len.10 =  48
   r.len.21 =  19
   r.len.99 =   7

   WHILE end.loop% = 0
      i% = INSTR(crlf$, POGDE.BLOCK$, 1)
      IF i% = 0 THEN BEGIN
         IF LEN(POGDE.BLOCK$) < (16 * 1024) THEN BEGIN
            GOSUB GET.BLOCK
         ENDIF ELSE BEGIN
            POGDE.BLOCK$ = ""
            GOTO READ.POGDE.ERROR
         ENDIF
      ENDIF ELSE BEGIN
         POGDE.RCD$   = POGDE.RCD$ + LEFT$(POGDE.BLOCK$, i% +1)
         POGDE.BLOCK$ = RIGHT$(POGDE.BLOCK$,                        \
                               LEN(POGDE.BLOCK$) -(i% +1))
      ENDIF
      IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "00" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.00 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.00 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "01" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.01 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.01 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "02" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.02 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.02 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "03" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.03 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.03 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "10" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.10 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.10 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "21" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.21 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.21 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE IF UNPACK$(MID$(POGDE.RCD$, 1, 1)) = "99" THEN BEGIN
         IF LEN(POGDE.RCD$) = r.len.99 THEN BEGIN
            POGDE.RCD$ = LEFT$(POGDE.RCD$, r.len.99 -2)
            end.loop% = -1
         ENDIF
      ENDIF ELSE BEGIN
         POGDE.RCD$ = ""                        ! Ignore record
      ENDIF
   WEND

   READ.POGDE = 0
   EXIT FUNCTION

GET.BLOCK:

   IF END #POGDE.SESS.NUM% THEN READ.POGDE.ERROR
   POGDE.CTR% = POGDE.CTR% +1
   READ FORM "C1024"; #POGDE.SESS.NUM%, POGDE.CTR%; a$
   POGDE.BLOCK$ = POGDE.BLOCK$ + a$
   a$ = ""

   RETURN

READ.POGDE.ERROR:

   CURRENT.REPORT.NUM% = POGDE.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

  END FUNCTION
