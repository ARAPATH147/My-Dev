\/******************************************************************/
\/*                                                                */
\/* CARTON FILE FUNCTIONS                                          */
\/*                                                                */
\/* REFERENCE   : CRTNFUN.BAS                                      */
\/*                                                                */
\/* VERSION A.          Neil Bennett.           12 DECEMBER 2006   */
\/*                                                                */
\/* VERSION B.          Arun Sudhakaran.        10 APRIL 2013      */
\/* Defined new variables for getting CRTN field positions         */
\/* and lengths as part of Automatic Booking In of Chilled Food    */
\/* ASNs project                                                   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE CRTNDEC.J86

   %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION GET.CRTN.KEY                                          */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION GET.CRTN.KEY$

      STRING    GET.CRTN.KEY$
      STRING    work$

      work$ = CRTN.SUPPLIER$ + CRTN.NO$ + CHR$(0)
      CALL PUTN1(work$, 7, CRTN.CHAIN%)

      GET.CRTN.KEY$ = work$
      work$ = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION CRTN.SET                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION CRTN.SET PUBLIC

      INTEGER*1 CRTN.SET

      !Changed the variable definitions in alphabetical order and added new         !BAS
      !variable definitions.                                                        !BAS
      CHAIN.OFFSET%             = 7     !CHAIN% field offset of the CARTON record   !BAS
      CRTN.FILE.NAME$           = "CARTON"                                          !BAS
      CRTN.NO.KEY.LEN%          = 4     !CARTON.NO$ field length of the             !BAS
                                        !CARTON record                              !BAS
      CRTN.NO.KEY.OFFSET%       = 4     !CARTON.NO$ field offset of the             !BAS
                                        !CARTON record                              !BAS
      CRTN.RECL%                = 508                                               !BAS
      CRTN.REPORT.NUM%          = 735                                               !BAS
      DELIVERY.DATE.LEN%        = 6     !Length of delivery date (YYMMDD) in the    !BAS
                                        !field EXPECTED.DELIVERY.DATETIME$          !BAS
                                        !(CCYYMMDDHHmm)                             !BAS
      DELIVERY.DATE.OFFSET%     = 37    !Field offset of delivery date (YYMMDD)     !BAS
                                        !in the field EXPECTED.DELIVERY.DATETIME$   !BAS
                                        !(CCYYMMDDHHmm)                             !BAS
      STATUS.LEN%               = 1     !STATUS$ field length of the CARTON record  !BAS
      STATUS.OFFSET%            = 9     !STATUS$ field offset of the CARTON record  !BAS
      SUPPLIER.NUMBER.LEN%      = 3     !SUPPLIER$ field length of the CARTON record!BAS
      SUPPLIER.NUMBER.OFFSET%   = 1     !SUPPLIER$ field offset of the CARTON record!BAS

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.CRTN                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.CRTN PUBLIC

      INTEGER*2 READ.CRTN
      INTEGER*2 i%
      STRING    CRTN.KEY$
      STRING    work$

      READ.CRTN = 1

      CRTN.KEY$ = GET.CRTN.KEY$

      DIM CRTN.ITEM.CODE$(60)
      DIM CRTN.DESP.QTY%(60)
      DIM CRTN.IN.QTY%(60)

      IF END #CRTN.SESS.NUM% THEN READ.ERROR
      READ FORM "T9,C1,C18,C5,C1,C1,C12,C3,C420,C39";               \
           #CRTN.SESS.NUM%                                          \
           KEY CRTN.KEY$;                                           \
               CRTN.STATUS$,                                        \
               CRTN.ASN.CODE$,                                      \
               CRTN.ORD.NO$,                                        \
               CRTN.ORD.SUFFIX$,                                    \
               CRTN.BUS.CNTR$,                                      \
               CRTN.DEL.DTTM$,                                      \
               CRTN.ITEM.CNT$,                                      \
               work$,                                               \
               CRTN.FILLER$

       FOR i% = 0 TO 59
          CRTN.ITEM.CODE$(i% +1) =  MID$(work$, (i%*7) +1, 3)
          CRTN.DESP.QTY%(i% +1)  = GETN2(work$, (i%*7) +3)
          CRTN.IN.QTY%(i% +1)    = GETN2(work$, (i%*7) +5)
       NEXT i%

       READ.CRTN = 0
    EXIT FUNCTION


READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = CRTN.REPORT.NUM%
      CURRENT.CODE$       = CRTN.KEY$

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.CRTN                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.CRTN PUBLIC

      INTEGER*2 WRITE.CRTN
      INTEGER*2 i%
      STRING    CRTN.KEY$
      STRING    work$

      WRITE.CRTN = 1

      CRTN.KEY$ = GET.CRTN.KEY$

       work$ = ""
       FOR i% = 1 TO 60
          work$ = work$                                             \
                + CRTN.ITEM.CODE$(i%)                               \
                + STRING$(4, CHR$(0))
          CALL PUTN2(work$, ((i% -1)*7) +3, CRTN.DESP.QTY%(i%))
          CALL PUTN2(work$, ((i% -1)*7) +5, CRTN.IN.QTY%(i%))
       NEXT i%

      IF END #CRTN.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C8,C1,C18,C5,C1,C1,C12,C3,C420,C39";              \
           #CRTN.SESS.NUM%;                                         \
               CRTN.KEY$,                                           \
               CRTN.STATUS$,                                        \
               CRTN.ASN.CODE$,                                      \
               CRTN.ORD.NO$,                                        \
               CRTN.ORD.SUFFIX$,                                    \
               CRTN.BUS.CNTR$,                                      \
               CRTN.DEL.DTTM$,                                      \
               CRTN.ITEM.CNT$,                                      \
               work$,                                               \
               CRTN.FILLER$

      WRITE.CRTN = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = CRTN.REPORT.NUM%
      CURRENT.CODE$ = CRTN.KEY$

   END FUNCTION

\/******************************************************************/

FUNCTION DELETE.CRTN PUBLIC

      INTEGER*2 DELETE.CRTN
      INTEGER*2 i%
      STRING    CRTN.KEY$
      STRING    work$

      ON ERROR GOTO RECORD.NOT.FOUND

      DELETE.CRTN = 1

      CRTN.KEY$ = GET.CRTN.KEY$
      DELREC CRTN.SESS.NUM%;CRTN.KEY$

      DELETE.CRTN = 0

EXITFUNC:

    EXIT FUNCTION

RECORD.NOT.FOUND:

    RESUME EXITFUNC

READ.ERROR:

      FILE.OPERATION$     = "D"
      CURRENT.REPORT.NUM% = CRTN.REPORT.NUM%

      CURRENT.CODE$       = CRTN.KEY$

   END FUNCTION

