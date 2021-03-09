
\*****************************************************************************
\*****************************************************************************
\***
\***                   FILE HANDLING FUNCTION SOURCE CODE
\***
\***                   FILE TYPE:  DIRECT
\***
\***                   REFERENCE:  CCMVTFUN.J86
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               CREDIT CLAIMS STOCK MOVEMENT FILE
\***
\***
\***    VERSION B : Clive Norris             6th December 1993
\***      
\***    CCMVT.ITEM.INCR.DECR.FLAG$ added to type 4 record      13/1/94
\***      
\***    Version C :             Mark Walker                 21st Jul 2015
\***    F392 Retail Stock 5
\***    Fixed format string in READ.CCMVT for type 1 record.
\***
\*****************************************************************************
\*****************************************************************************

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE CCMVTDEC.J86



   FUNCTION CCMVT.SET PUBLIC
 
      INTEGER*2 CCMVT.SET
      INTEGER*2 LOOP%

      CCMVT.SET = 1

         CCMVT.REPORT.NUM%        = 318
         CCMVT.FILE.NAME$         = "CCMVT"
         CCMVT.RECL%              = 256
         CCMVT.HEADER.FILLER$     = STRING$(239," ")
         CCMVT.FILLER.01$         = STRING$(101," ")
         CCMVT.FILLER.02$         = STRING$(8," ")
         CCMVT.FILLER.03$         = STRING$(12," ")
         CCMVT.FILLER.04$         = STRING$(12," ")
         CCMVT.TRAILER.FILLER$    = STRING$(246," ")

         LOOP% = 1
         WHILE LOOP%<=16
           CCMVT.ITEM.FILLER.02$(LOOP%) = STRING$(2," ")
           LOOP% = LOOP% + 1
         WEND

         LOOP% = 1
         WHILE LOOP%<=16
           CCMVT.ITEM.FILLER.03$(LOOP%) = STRING$(4," ")
           LOOP% = LOOP% + 1
         WEND
         
         LOOP% = 1
         WHILE LOOP%<=8
           CCMVT.ITEM.FILLER.04$(LOOP%) = STRING$(12," ")
           LOOP% = LOOP% + 1
         WEND

      CCMVT.SET = 0
    
   END FUNCTION



   FUNCTION READ.CCMVT PUBLIC

    INTEGER*2 READ.CCMVT
    STRING    FORMAT$

    READ.CCMVT = 1

     IF END #CCMVT.SESS.NUM% THEN READ.CCMVT.ERROR
     READ FORM "C2"; # CCMVT.SESS.NUM%; CCMVT.TRANS.TYPE$

     IF CCMVT.TRANS.TYPE$   = "00" THEN       \Header record
        READ FORM "C5,C6,C2,C239,I2"; # CCMVT.SESS.NUM%;                \
           CCMVT.SERIAL.NUM$,                                           \
           CCMVT.DATE$,                                                 \
           CCMVT.STORE.NUMBER$,                                         \
           CCMVT.HEADER.FILLER$,                                        \
           CCMVT.SEQUENCE%                                              \
     ELSE                                                               \
       IF CCMVT.TRANS.TYPE$   = "01" THEN     \Credit claim hdr record
          FORMAT$ = "C4,C7,C1,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" + \
                    ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,C101,I2"          :\   !CMW
          READ FORM FORMAT$; #CCMVT.SESS.NUM%;                          \   
             CCMVT.CREDIT.CLAIM.NUM$,                                   \   
             CCMVT.UOD.NUM$,                                            \   
             CCMVT.STATUS$,                                             \   
             CCMVT.NUM.OF.ITEMS%,                                       \
             CCMVT.SUPPLY.ROUTE$,                                       \   
             CCMVT.DISP.LOCATION$,                                      \   
             CCMVT.BC.LETTER$,                                          \   
             CCMVT.RECALL.NUM$,                                         \   
             CCMVT.AUTHORISATION$,                                      \  
             CCMVT.SUPPLIER$,                                           \  
             CCMVT.METHOD.OF.RETURN$,                                   \   
             CCMVT.CARRIER$,                                            \   
             CCMVT.BIRD.NUM$,                                           \   
             CCMVT.REASON.NUM$,                                         \   
             CCMVT.RECEIVING.STORE$,                                    \   
             CCMVT.DESTINATION$,                                        \   
             CCMVT.WAREHOUSE.ROUTE$,                                    \  
             CCMVT.UOD.TYPE$,                                           \   
             CCMVT.DAMAGE.REASON$,                                      \   
             CCMVT.INVOICE.NUM$,                                        \   
             CCMVT.FOLIO.NUM$,                                          \   
             CCMVT.BATCH.REF$,                                          \   
             CCMVT.WHOLE.PART.CON$,                                     \
             CCMVT.REPAIR.CATEGORY$,                                    \   
             CCMVT.REPAIR.NUM$,                                         \   
             CCMVT.PLAN4.POLICY.NUM$,                                   \   
             CCMVT.DDDA.DCDR.NUM$,                                      \   
             CCMVT.DELIV.NOTE.NUM$,                                     \   
             CCMVT.DELIV.DATE$,                                         \
             CCMVT.NUM.CARTONS.RECEIV$,                                 \   
             CCMVT.ORDER.NUM$,                                          \   
             CCMVT.COMMENT$,                                            \   
             CCMVT.DATE.OF.CLAIM$,                                      \   
             CCMVT.TIME.OF.CLAIM$,                                      \   
             CCMVT.FILLER.01$,                                          \  
             CCMVT.SEQUENCE%                                            \
       ELSE                                                             \
         IF CCMVT.TRANS.TYPE$ = "02" THEN      \Credit claim item record
            FORMAT$ = "C4," + STRING$(16,"C1,C7,C2,C3,C2,") + "C8,I2"  :\
            READ FORM FORMAT$; #CCMVT.SESS.NUM%;                        \
                CCMVT.CREDIT.CLAIM.NUM$,                                \   
                CCMVT.ITEM.BARCODE.FLAG$(1),                            \   
                CCMVT.ITEM.BARCODE$(1),                                 \   
                CCMVT.ITEM.QTY$(1),                                     \          
                CCMVT.ITEM.PRICE$(1),                                   \          
                CCMVT.ITEM.FILLER.02$(1),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(2),                            \   
                CCMVT.ITEM.BARCODE$(2),                                 \   
                CCMVT.ITEM.QTY$(2),                                     \          
                CCMVT.ITEM.PRICE$(2),                                   \          
                CCMVT.ITEM.FILLER.02$(2),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(3),                            \   
                CCMVT.ITEM.BARCODE$(3),                                 \   
                CCMVT.ITEM.QTY$(3),                                     \          
                CCMVT.ITEM.PRICE$(3),                                   \          
                CCMVT.ITEM.FILLER.02$(3),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(4),                            \   
                CCMVT.ITEM.BARCODE$(4),                                 \   
                CCMVT.ITEM.QTY$(4),                                     \          
                CCMVT.ITEM.PRICE$(4),                                   \          
                CCMVT.ITEM.FILLER.02$(4),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(5),                            \   
                CCMVT.ITEM.BARCODE$(5),                                 \   
                CCMVT.ITEM.QTY$(5),                                     \          
                CCMVT.ITEM.PRICE$(5),                                   \          
                CCMVT.ITEM.FILLER.02$(5),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(6),                            \   
                CCMVT.ITEM.BARCODE$(6),                                 \   
                CCMVT.ITEM.QTY$(6),                                     \          
                CCMVT.ITEM.PRICE$(6),                                   \          
                CCMVT.ITEM.FILLER.02$(6),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(7),                            \   
                CCMVT.ITEM.BARCODE$(7),                                 \   
                CCMVT.ITEM.QTY$(7),                                     \          
                CCMVT.ITEM.PRICE$(7),                                   \          
                CCMVT.ITEM.FILLER.02$(7),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(8),                            \   
                CCMVT.ITEM.BARCODE$(8),                                 \   
                CCMVT.ITEM.QTY$(8),                                     \          
                CCMVT.ITEM.PRICE$(8),                                   \          
                CCMVT.ITEM.FILLER.02$(8),                               \       
                CCMVT.ITEM.BARCODE.FLAG$(9),                            \   
                CCMVT.ITEM.BARCODE$(9),                                 \   
                CCMVT.ITEM.QTY$(9),                                     \          
                CCMVT.ITEM.PRICE$(9),                                   \          
                CCMVT.ITEM.FILLER.02$(9),                               \  
                CCMVT.ITEM.BARCODE.FLAG$(10),                           \   
                CCMVT.ITEM.BARCODE$(10),                                \   
                CCMVT.ITEM.QTY$(10),                                    \          
                CCMVT.ITEM.PRICE$(10),                                  \          
                CCMVT.ITEM.FILLER.02$(10),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(11),                           \   
                CCMVT.ITEM.BARCODE$(11),                                \   
                CCMVT.ITEM.QTY$(11),                                    \          
                CCMVT.ITEM.PRICE$(11),                                  \          
                CCMVT.ITEM.FILLER.02$(11),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(12),                           \   
                CCMVT.ITEM.BARCODE$(12),                                \   
                CCMVT.ITEM.QTY$(12),                                    \          
                CCMVT.ITEM.PRICE$(12),                                  \          
                CCMVT.ITEM.FILLER.02$(12),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(13),                           \   
                CCMVT.ITEM.BARCODE$(13),                                \   
                CCMVT.ITEM.QTY$(13),                                    \          
                CCMVT.ITEM.PRICE$(13),                                  \          
                CCMVT.ITEM.FILLER.02$(13),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(14),                           \   
                CCMVT.ITEM.BARCODE$(14),                                \   
                CCMVT.ITEM.QTY$(14),                                    \          
                CCMVT.ITEM.PRICE$(14),                                  \          
                CCMVT.ITEM.FILLER.02$(14),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(15),                           \   
                CCMVT.ITEM.BARCODE$(15),                                \   
                CCMVT.ITEM.QTY$(15),                                    \          
                CCMVT.ITEM.PRICE$(15),                                  \          
                CCMVT.ITEM.FILLER.02$(15),                              \  
                CCMVT.ITEM.BARCODE.FLAG$(16),                           \   
                CCMVT.ITEM.BARCODE$(16),                                \   
                CCMVT.ITEM.QTY$(16),                                    \          
                CCMVT.ITEM.PRICE$(16),                                  \          
                CCMVT.ITEM.FILLER.02$(16),                              \       
                CCMVT.FILLER.02$,                                       \
                CCMVT.SEQUENCE%                                         \
         ELSE                                                           \
           IF CCMVT.TRANS.TYPE$     = "03" THEN     \Return record
              FORMAT$ = STRING$(16,"C1,C7,C2,C1,C4,") + "C12,I2"       :\
              READ FORM FORMAT$; #CCMVT.SESS.NUM%;                      \
                 CCMVT.ITEM.BARCODE.FLAG$(1),                           \   
                 CCMVT.ITEM.BARCODE$(1),                                \   
                 CCMVT.ITEM.QTY$(1),                                    \          
                 CCMVT.ITEM.REASON$(1),                                 \
                 CCMVT.ITEM.FILLER.03$(1),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(2),                           \   
                 CCMVT.ITEM.BARCODE$(2),                                \   
                 CCMVT.ITEM.QTY$(2),                                    \          
                 CCMVT.ITEM.REASON$(2),                                 \
                 CCMVT.ITEM.FILLER.03$(2),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(3),                           \   
                 CCMVT.ITEM.BARCODE$(3),                                \   
                 CCMVT.ITEM.QTY$(3),                                    \          
                 CCMVT.ITEM.REASON$(3),                                 \
                 CCMVT.ITEM.FILLER.03$(3),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(4),                           \   
                 CCMVT.ITEM.BARCODE$(4),                                \   
                 CCMVT.ITEM.QTY$(4),                                    \          
                 CCMVT.ITEM.REASON$(4),                                 \
                 CCMVT.ITEM.FILLER.03$(4),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(5),                           \   
                 CCMVT.ITEM.BARCODE$(5),                                \   
                 CCMVT.ITEM.QTY$(5),                                    \          
                 CCMVT.ITEM.REASON$(5),                                 \
                 CCMVT.ITEM.FILLER.03$(5),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(6),                           \   
                 CCMVT.ITEM.BARCODE$(6),                                \   
                 CCMVT.ITEM.QTY$(6),                                    \          
                 CCMVT.ITEM.REASON$(6),                                 \
                 CCMVT.ITEM.FILLER.03$(6),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(7),                           \   
                 CCMVT.ITEM.BARCODE$(7),                                \   
                 CCMVT.ITEM.QTY$(7),                                    \          
                 CCMVT.ITEM.REASON$(7),                                 \
                 CCMVT.ITEM.FILLER.03$(7),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(8),                           \   
                 CCMVT.ITEM.BARCODE$(8),                                \   
                 CCMVT.ITEM.QTY$(8),                                    \          
                 CCMVT.ITEM.REASON$(8),                                 \
                 CCMVT.ITEM.FILLER.03$(8),                              \ 
                 CCMVT.ITEM.BARCODE.FLAG$(9),                           \   
                 CCMVT.ITEM.BARCODE$(9),                                \   
                 CCMVT.ITEM.QTY$(9),                                    \          
                 CCMVT.ITEM.REASON$(9),                                 \
                 CCMVT.ITEM.FILLER.03$(9),                              \  
                 CCMVT.ITEM.BARCODE.FLAG$(10),                          \   
                 CCMVT.ITEM.BARCODE$(10),                               \   
                 CCMVT.ITEM.QTY$(10),                                   \          
                 CCMVT.ITEM.REASON$(10),                                \
                 CCMVT.ITEM.FILLER.03$(10),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(11),                          \   
                 CCMVT.ITEM.BARCODE$(11),                               \   
                 CCMVT.ITEM.QTY$(11),                                   \          
                 CCMVT.ITEM.REASON$(11),                                \
                 CCMVT.ITEM.FILLER.03$(11),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(12),                          \   
                 CCMVT.ITEM.BARCODE$(12),                               \   
                 CCMVT.ITEM.QTY$(12),                                   \          
                 CCMVT.ITEM.REASON$(12),                                \
                 CCMVT.ITEM.FILLER.03$(12),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(13),                          \   
                 CCMVT.ITEM.BARCODE$(13),                               \   
                 CCMVT.ITEM.QTY$(13),                                   \          
                 CCMVT.ITEM.REASON$(13),                                \
                 CCMVT.ITEM.FILLER.03$(13),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(14),                          \   
                 CCMVT.ITEM.BARCODE$(14),                               \   
                 CCMVT.ITEM.QTY$(14),                                   \          
                 CCMVT.ITEM.REASON$(14),                                \
                 CCMVT.ITEM.FILLER.03$(14),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(15),                          \   
                 CCMVT.ITEM.BARCODE$(15),                               \   
                 CCMVT.ITEM.QTY$(15),                                   \          
                 CCMVT.ITEM.REASON$(15),                                \
                 CCMVT.ITEM.FILLER.03$(15),                             \  
                 CCMVT.ITEM.BARCODE.FLAG$(16),                          \   
                 CCMVT.ITEM.BARCODE$(16),                               \   
                 CCMVT.ITEM.QTY$(16),                                   \          
                 CCMVT.ITEM.REASON$(16),                                \
                 CCMVT.ITEM.FILLER.03$(16),                             \                
                 CCMVT.FILLER.03$,                                      \
                 CCMVT.SEQUENCE%                                        \                                 
           ELSE                                                         \
             IF CCMVT.TRANS.TYPE$     = "04" THEN     \Local Pricing recd
                FORMAT$ = STRING$(8,"C4,C3,C4,C6,C1,C12,") + "C12,I2"  :\
                READ FORM FORMAT$; #CCMVT.SESS.NUM%;                    \
                 CCMVT.ITEM.ITEM.CODE.04$(1),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(1),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(1),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(1),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(1),                         \
                 CCMVT.ITEM.FILLER.04$(1),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(2),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(2),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(2),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(2),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(2),                         \
                 CCMVT.ITEM.FILLER.04$(2),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(3),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(3),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(3),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(3),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(3),                         \
                 CCMVT.ITEM.FILLER.04$(3),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(4),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(4),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(4),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(4),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(4),                         \
                 CCMVT.ITEM.FILLER.04$(4),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(5),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(5),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(5),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(5),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(5),                         \
                 CCMVT.ITEM.FILLER.04$(5),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(6),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(6),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(6),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(6),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(6),                         \
                 CCMVT.ITEM.FILLER.04$(6),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(7),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(7),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(7),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(7),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(7),                         \
                 CCMVT.ITEM.FILLER.04$(7),                              \  
                 CCMVT.ITEM.ITEM.CODE.04$(8),                           \   
                 CCMVT.ITEM.REASON.CODE.04$(8),                         \          
                 CCMVT.ITEM.AUTH.NUM.04$(8),                            \          
                 CCMVT.ITEM.STOCK.VALUE.04$(8),                         \
                 CCMVT.ITEM.INCR.DECR.FLAG$(8),                         \
                 CCMVT.ITEM.FILLER.04$(8),                              \                
                 CCMVT.FILLER.04$,                                      \
                 CCMVT.SEQUENCE%                                        \                                 
             ELSE                                                       \
               IF CCMVT.TRANS.TYPE$ = "99" THEN       \Trailer record
                  READ FORM "C6,C246,I2"; #CCMVT.SESS.NUM%;             \
                     CCMVT.RECORD.COUNT$,                               \
                     CCMVT.TRAILER.FILLER$,                             \
                     CCMVT.SEQUENCE%
   READ.CCMVT = 0
   EXIT FUNCTION

   READ.CCMVT.ERROR:

     CURRENT.REPORT.NUM% = CCMVT.REPORT.NUM%
     FILE.OPERATION$ = "R"
     CURRENT.CODE$ = ""
     EXIT FUNCTION

  END FUNCTION



  FUNCTION WRITE.CCMVT PUBLIC

     INTEGER*2 WRITE.CCMVT
     STRING    FORMAT$

     WRITE.CCMVT = 1

        IF END #CCMVT.SESS.NUM% THEN WRITE.CCMVT.ERROR

        IF CCMVT.TRANS.TYPE$   = "00" THEN       \Header record
           WRITE FORM "C2,C5,C6,C2,C239,I2"; #CCMVT.SESS.NUM%;          \
              CCMVT.TRANS.TYPE$,                                        \
              CCMVT.SERIAL.NUM$,                                        \
              CCMVT.DATE$,                                              \
              CCMVT.STORE.NUMBER$,                                      \
              CCMVT.HEADER.FILLER$,                                     \
              CCMVT.SEQUENCE%                                           \
        ELSE                                                            \
           IF CCMVT.TRANS.TYPE$   = "01" THEN     \Credit claim hdr record
             FORMAT$ = "C2,C4,C7,C1,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1" +  \
                       ",C9,2C3,2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,C101,I2":\
             WRITE FORM FORMAT$; #CCMVT.SESS.NUM%;                      \
                CCMVT.TRANS.TYPE$,                                      \
                CCMVT.CREDIT.CLAIM.NUM$,                                \   
                CCMVT.UOD.NUM$,                                         \   
                CCMVT.STATUS$,                                          \   
                CCMVT.NUM.OF.ITEMS%,                                    \
                CCMVT.SUPPLY.ROUTE$,                                    \   
                CCMVT.DISP.LOCATION$,                                   \   
                CCMVT.BC.LETTER$,                                       \   
                CCMVT.RECALL.NUM$,                                      \   
                CCMVT.AUTHORISATION$,                                   \  
                CCMVT.SUPPLIER$,                                        \  
                CCMVT.METHOD.OF.RETURN$,                                \   
                CCMVT.CARRIER$,                                         \   
                CCMVT.BIRD.NUM$,                                        \   
                CCMVT.REASON.NUM$,                                      \   
                CCMVT.RECEIVING.STORE$,                                 \   
                CCMVT.DESTINATION$,                                     \   
                CCMVT.WAREHOUSE.ROUTE$,                                 \  
                CCMVT.UOD.TYPE$,                                        \   
                CCMVT.DAMAGE.REASON$,                                   \   
                CCMVT.INVOICE.NUM$,                                     \   
                CCMVT.FOLIO.NUM$,                                       \   
                CCMVT.BATCH.REF$,                                       \   
                CCMVT.WHOLE.PART.CON$,                                  \
                CCMVT.REPAIR.CATEGORY$,                                 \   
                CCMVT.REPAIR.NUM$,                                      \   
                CCMVT.PLAN4.POLICY.NUM$,                                \   
                CCMVT.DDDA.DCDR.NUM$,                                   \   
                CCMVT.DELIV.NOTE.NUM$,                                  \   
                CCMVT.DELIV.DATE$,                                      \
                CCMVT.NUM.CARTONS.RECEIV$,                              \   
                CCMVT.ORDER.NUM$,                                       \   
                CCMVT.COMMENT$,                                         \   
                CCMVT.DATE.OF.CLAIM$,                                   \   
                CCMVT.TIME.OF.CLAIM$,                                   \   
                CCMVT.FILLER.01$,                                       \  
                CCMVT.SEQUENCE%                                         \
           ELSE                                                         \
              IF CCMVT.TRANS.TYPE$ = "02" THEN      \Credit claim item record
                 FORMAT$ = "C2,C4,"+STRING$(16,"C1,C7,C2,C3,C2,")+"C8,I2" :\
                 WRITE FORM FORMAT$; #CCMVT.SESS.NUM%;                  \
                    CCMVT.TRANS.TYPE$,                                  \
                    CCMVT.CREDIT.CLAIM.NUM$,                            \   
                    CCMVT.ITEM.BARCODE.FLAG$(1),                        \   
                    CCMVT.ITEM.BARCODE$(1),                             \   
                    CCMVT.ITEM.QTY$(1),                                 \          
                    CCMVT.ITEM.PRICE$(1),                               \          
                    CCMVT.ITEM.FILLER.02$(1),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(2),                        \   
                    CCMVT.ITEM.BARCODE$(2),                             \   
                    CCMVT.ITEM.QTY$(2),                                 \          
                    CCMVT.ITEM.PRICE$(2),                               \          
                    CCMVT.ITEM.FILLER.02$(2),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(3),                        \   
                    CCMVT.ITEM.BARCODE$(3),                             \   
                    CCMVT.ITEM.QTY$(3),                                 \          
                    CCMVT.ITEM.PRICE$(3),                               \          
                    CCMVT.ITEM.FILLER.02$(3),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(4),                        \   
                    CCMVT.ITEM.BARCODE$(4),                             \   
                    CCMVT.ITEM.QTY$(4),                                 \          
                    CCMVT.ITEM.PRICE$(4),                               \          
                    CCMVT.ITEM.FILLER.02$(4),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(5),                        \   
                    CCMVT.ITEM.BARCODE$(5),                             \   
                    CCMVT.ITEM.QTY$(5),                                 \          
                    CCMVT.ITEM.PRICE$(5),                               \          
                    CCMVT.ITEM.FILLER.02$(5),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(6),                        \   
                    CCMVT.ITEM.BARCODE$(6),                             \   
                    CCMVT.ITEM.QTY$(6),                                 \          
                    CCMVT.ITEM.PRICE$(6),                               \          
                    CCMVT.ITEM.FILLER.02$(6),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(7),                        \   
                    CCMVT.ITEM.BARCODE$(7),                             \   
                    CCMVT.ITEM.QTY$(7),                                 \          
                    CCMVT.ITEM.PRICE$(7),                               \          
                    CCMVT.ITEM.FILLER.02$(7),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(8),                        \   
                    CCMVT.ITEM.BARCODE$(8),                             \   
                    CCMVT.ITEM.QTY$(8),                                 \          
                    CCMVT.ITEM.PRICE$(8),                               \          
                    CCMVT.ITEM.FILLER.02$(8),                           \                   
                    CCMVT.ITEM.BARCODE.FLAG$(9),                        \   
                    CCMVT.ITEM.BARCODE$(9),                             \   
                    CCMVT.ITEM.QTY$(9),                                 \          
                    CCMVT.ITEM.PRICE$(9),                               \          
                    CCMVT.ITEM.FILLER.02$(9),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(10),                       \   
                    CCMVT.ITEM.BARCODE$(10),                            \   
                    CCMVT.ITEM.QTY$(10),                                \          
                    CCMVT.ITEM.PRICE$(10),                              \          
                    CCMVT.ITEM.FILLER.02$(10),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(11),                       \   
                    CCMVT.ITEM.BARCODE$(11),                            \   
                    CCMVT.ITEM.QTY$(11),                                \          
                    CCMVT.ITEM.PRICE$(11),                              \          
                    CCMVT.ITEM.FILLER.02$(11),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(12),                       \   
                    CCMVT.ITEM.BARCODE$(12),                            \   
                    CCMVT.ITEM.QTY$(12),                                \          
                    CCMVT.ITEM.PRICE$(12),                              \          
                    CCMVT.ITEM.FILLER.02$(12),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(13),                       \   
                    CCMVT.ITEM.BARCODE$(13),                            \   
                    CCMVT.ITEM.QTY$(13),                                \          
                    CCMVT.ITEM.PRICE$(13),                              \          
                    CCMVT.ITEM.FILLER.02$(13),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(14),                       \   
                    CCMVT.ITEM.BARCODE$(14),                            \   
                    CCMVT.ITEM.QTY$(14),                                \          
                    CCMVT.ITEM.PRICE$(14),                              \          
                    CCMVT.ITEM.FILLER.02$(14),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(15),                       \   
                    CCMVT.ITEM.BARCODE$(15),                            \   
                    CCMVT.ITEM.QTY$(15),                                \          
                    CCMVT.ITEM.PRICE$(15),                              \          
                    CCMVT.ITEM.FILLER.02$(15),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(16),                       \   
                    CCMVT.ITEM.BARCODE$(16),                            \   
                    CCMVT.ITEM.QTY$(16),                                \          
                    CCMVT.ITEM.PRICE$(16),                              \          
                    CCMVT.ITEM.FILLER.02$(16),                          \       
                    CCMVT.FILLER.02$,                                   \
                    CCMVT.SEQUENCE%                                     \
              ELSE                                                      \
                 IF CCMVT.TRANS.TYPE$     = "03" THEN     \Return record
                   FORMAT$ = "C2,"+STRING$(16,"C1,C7,C2,C1,C4,")+"C12,I2":\
                   WRITE FORM FORMAT$; #CCMVT.SESS.NUM%;                \
                       CCMVT.TRANS.TYPE$,                               \
                       CCMVT.ITEM.BARCODE.FLAG$(1),                     \   
                       CCMVT.ITEM.BARCODE$(1),                          \   
                       CCMVT.ITEM.QTY$(1),                              \          
                       CCMVT.ITEM.REASON$(1),                           \
                       CCMVT.ITEM.FILLER.03$(1),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(2),                     \   
                       CCMVT.ITEM.BARCODE$(2),                          \   
                       CCMVT.ITEM.QTY$(2),                              \          
                       CCMVT.ITEM.REASON$(2),                           \
                       CCMVT.ITEM.FILLER.03$(2),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(3),                     \   
                       CCMVT.ITEM.BARCODE$(3),                          \   
                       CCMVT.ITEM.QTY$(3),                              \          
                       CCMVT.ITEM.REASON$(3),                           \
                       CCMVT.ITEM.FILLER.03$(3),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(4),                     \   
                       CCMVT.ITEM.BARCODE$(4),                          \   
                       CCMVT.ITEM.QTY$(4),                              \          
                       CCMVT.ITEM.REASON$(4),                           \
                       CCMVT.ITEM.FILLER.03$(4),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(5),                     \   
                       CCMVT.ITEM.BARCODE$(5),                          \   
                       CCMVT.ITEM.QTY$(5),                              \          
                       CCMVT.ITEM.REASON$(5),                           \
                       CCMVT.ITEM.FILLER.03$(5),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(6),                     \   
                       CCMVT.ITEM.BARCODE$(6),                          \   
                       CCMVT.ITEM.QTY$(6),                              \          
                       CCMVT.ITEM.REASON$(6),                           \
                       CCMVT.ITEM.FILLER.03$(6),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(7),                     \   
                       CCMVT.ITEM.BARCODE$(7),                          \   
                       CCMVT.ITEM.QTY$(7),                              \          
                       CCMVT.ITEM.REASON$(7),                           \
                       CCMVT.ITEM.FILLER.03$(7),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(8),                     \   
                       CCMVT.ITEM.BARCODE$(8),                          \   
                       CCMVT.ITEM.QTY$(8),                              \          
                       CCMVT.ITEM.REASON$(8),                           \
                       CCMVT.ITEM.FILLER.03$(8),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(9),                     \   
                       CCMVT.ITEM.BARCODE$(9),                          \   
                       CCMVT.ITEM.QTY$(9),                              \          
                       CCMVT.ITEM.REASON$(9),                           \
                       CCMVT.ITEM.FILLER.03$(9),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(10),                    \   
                       CCMVT.ITEM.BARCODE$(10),                         \   
                       CCMVT.ITEM.QTY$(10),                             \          
                       CCMVT.ITEM.REASON$(10),                          \
                       CCMVT.ITEM.FILLER.03$(10),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(11),                    \   
                       CCMVT.ITEM.BARCODE$(11),                         \   
                       CCMVT.ITEM.QTY$(11),                             \          
                       CCMVT.ITEM.REASON$(11),                          \
                       CCMVT.ITEM.FILLER.03$(11),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(12),                    \   
                       CCMVT.ITEM.BARCODE$(12),                         \   
                       CCMVT.ITEM.QTY$(12),                             \          
                       CCMVT.ITEM.REASON$(12),                          \
                       CCMVT.ITEM.FILLER.03$(12),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(13),                    \   
                       CCMVT.ITEM.BARCODE$(13),                         \   
                       CCMVT.ITEM.QTY$(13),                             \          
                       CCMVT.ITEM.REASON$(13),                          \
                       CCMVT.ITEM.FILLER.03$(13),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(14),                    \   
                       CCMVT.ITEM.BARCODE$(14),                         \   
                       CCMVT.ITEM.QTY$(14),                             \          
                       CCMVT.ITEM.REASON$(14),                          \
                       CCMVT.ITEM.FILLER.03$(14),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(15),                    \   
                       CCMVT.ITEM.BARCODE$(15),                         \   
                       CCMVT.ITEM.QTY$(15),                             \          
                       CCMVT.ITEM.REASON$(15),                          \
                       CCMVT.ITEM.FILLER.03$(15),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(16),                    \   
                       CCMVT.ITEM.BARCODE$(16),                         \   
                       CCMVT.ITEM.QTY$(16),                             \          
                       CCMVT.ITEM.REASON$(16),                          \
                       CCMVT.ITEM.FILLER.03$(16),                       \                
                       CCMVT.FILLER.03$,                                \
                       CCMVT.SEQUENCE%                                  \                                 
                 ELSE                                                      \
                   IF CCMVT.TRANS.TYPE$     = "04" THEN     \Local Pricing recd
                    FORMAT$ = "C2,"+STRING$(8,"C4,C3,C4,C6,C1,C12,")+"C12,I2":\
                    WRITE FORM FORMAT$; #CCMVT.SESS.NUM%;               \
                       CCMVT.TRANS.TYPE$,                               \
                       CCMVT.ITEM.ITEM.CODE.04$(1),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(1),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(1),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(1),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(1),                   \
                       CCMVT.ITEM.FILLER.04$(1),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(2),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(2),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(2),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(2),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(2),                   \
                       CCMVT.ITEM.FILLER.04$(2),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(3),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(3),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(3),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(3),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(3),                   \
                       CCMVT.ITEM.FILLER.04$(3),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(4),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(4),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(4),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(4),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(4),                   \
                       CCMVT.ITEM.FILLER.04$(4),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(5),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(5),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(5),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(5),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(5),                   \
                       CCMVT.ITEM.FILLER.04$(5),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(6),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(6),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(6),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(6),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(6),                   \
                       CCMVT.ITEM.FILLER.04$(6),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(7),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(7),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(7),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(7),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(7),                   \
                       CCMVT.ITEM.FILLER.04$(7),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(8),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(8),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(8),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(8),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(8),                   \
                       CCMVT.ITEM.FILLER.04$(8),                        \                
                       CCMVT.FILLER.04$,                                \
                       CCMVT.SEQUENCE%                                  \                                 
                 ELSE                                                   \
                    IF CCMVT.TRANS.TYPE$ = "99" THEN       \Trailer record
                       WRITE FORM "C2,C6,C246,I2"; #CCMVT.SESS.NUM%;    \
                          CCMVT.TRANS.TYPE$,                            \
                          CCMVT.RECORD.COUNT$,                          \
                          CCMVT.TRAILER.FILLER$,                        \
                          CCMVT.SEQUENCE%

     WRITE.CCMVT = 0
     EXIT FUNCTION

     WRITE.CCMVT.ERROR:

        CURRENT.REPORT.NUM% = CCMVT.REPORT.NUM%
        FILE.OPERATION$ = "W"
        CURRENT.CODE$ = ""
        EXIT FUNCTION

  END FUNCTION




  FUNCTION WRITE.HOLD.CCMVT PUBLIC

     INTEGER*2 WRITE.HOLD.CCMVT
     STRING    FORMAT$

     WRITE.HOLD.CCMVT = 1

        IF END #CCMVT.SESS.NUM% THEN WRITE.HOLD.CCMVT.ERROR

        IF CCMVT.TRANS.TYPE$   = "00" THEN       \Header record
           WRITE FORM "C2,C5,C6,C2,C239,I2"; HOLD #CCMVT.SESS.NUM%;     \
              CCMVT.TRANS.TYPE$,                                        \
              CCMVT.SERIAL.NUM$,                                        \
              CCMVT.DATE$,                                              \
              CCMVT.STORE.NUMBER$,                                      \
              CCMVT.HEADER.FILLER$,                                     \
              CCMVT.SEQUENCE%                                           \
        ELSE                                                            \
           IF CCMVT.TRANS.TYPE$   = "01" THEN     \Credit claim hdr record
             FORMAT$ = "C2,C4,C7,C1,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1" +  \
                       ",C9,2C3,2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,C101,I2":\
             WRITE FORM FORMAT$; HOLD #CCMVT.SESS.NUM%;                 \
                CCMVT.TRANS.TYPE$,                                      \
                CCMVT.CREDIT.CLAIM.NUM$,                                \   
                CCMVT.UOD.NUM$,                                         \   
                CCMVT.STATUS$,                                          \   
                CCMVT.NUM.OF.ITEMS%,                                    \
                CCMVT.SUPPLY.ROUTE$,                                    \   
                CCMVT.DISP.LOCATION$,                                   \   
                CCMVT.BC.LETTER$,                                       \   
                CCMVT.RECALL.NUM$,                                      \   
                CCMVT.AUTHORISATION$,                                   \  
                CCMVT.SUPPLIER$,                                        \  
                CCMVT.METHOD.OF.RETURN$,                                \   
                CCMVT.CARRIER$,                                         \   
                CCMVT.BIRD.NUM$,                                        \   
                CCMVT.REASON.NUM$,                                      \   
                CCMVT.RECEIVING.STORE$,                                 \   
                CCMVT.DESTINATION$,                                     \   
                CCMVT.WAREHOUSE.ROUTE$,                                 \  
                CCMVT.UOD.TYPE$,                                        \   
                CCMVT.DAMAGE.REASON$,                                   \   
                CCMVT.INVOICE.NUM$,                                     \   
                CCMVT.FOLIO.NUM$,                                       \   
                CCMVT.BATCH.REF$,                                       \   
                CCMVT.WHOLE.PART.CON$,                                  \
                CCMVT.REPAIR.CATEGORY$,                                 \   
                CCMVT.REPAIR.NUM$,                                      \   
                CCMVT.PLAN4.POLICY.NUM$,                                \   
                CCMVT.DDDA.DCDR.NUM$,                                   \   
                CCMVT.DELIV.NOTE.NUM$,                                  \   
                CCMVT.DELIV.DATE$,                                      \
                CCMVT.NUM.CARTONS.RECEIV$,                              \   
                CCMVT.ORDER.NUM$,                                       \   
                CCMVT.COMMENT$,                                         \   
                CCMVT.DATE.OF.CLAIM$,                                   \   
                CCMVT.TIME.OF.CLAIM$,                                   \   
                CCMVT.FILLER.01$,                                       \  
                CCMVT.SEQUENCE%                                         \
           ELSE                                                         \
              IF CCMVT.TRANS.TYPE$ = "02" THEN      \Credit claim item record
                 FORMAT$ = "C2,C4,"+STRING$(16,"C1,C7,C2,C3,C2,")+"C8,I2"  :\
                 WRITE FORM FORMAT$; HOLD #CCMVT.SESS.NUM%;             \
                    CCMVT.TRANS.TYPE$,                                  \
                    CCMVT.CREDIT.CLAIM.NUM$,                            \   
                    CCMVT.ITEM.BARCODE.FLAG$(1),                        \   
                    CCMVT.ITEM.BARCODE$(1),                             \   
                    CCMVT.ITEM.QTY$(1),                                 \          
                    CCMVT.ITEM.PRICE$(1),                               \          
                    CCMVT.ITEM.FILLER.02$(1),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(2),                        \   
                    CCMVT.ITEM.BARCODE$(2),                             \   
                    CCMVT.ITEM.QTY$(2),                                 \          
                    CCMVT.ITEM.PRICE$(2),                               \          
                    CCMVT.ITEM.FILLER.02$(2),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(3),                        \   
                    CCMVT.ITEM.BARCODE$(3),                             \   
                    CCMVT.ITEM.QTY$(3),                                 \          
                    CCMVT.ITEM.PRICE$(3),                               \          
                    CCMVT.ITEM.FILLER.02$(3),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(4),                        \   
                    CCMVT.ITEM.BARCODE$(4),                             \   
                    CCMVT.ITEM.QTY$(4),                                 \          
                    CCMVT.ITEM.PRICE$(4),                               \          
                    CCMVT.ITEM.FILLER.02$(4),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(5),                        \   
                    CCMVT.ITEM.BARCODE$(5),                             \   
                    CCMVT.ITEM.QTY$(5),                                 \          
                    CCMVT.ITEM.PRICE$(5),                               \          
                    CCMVT.ITEM.FILLER.02$(5),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(6),                        \   
                    CCMVT.ITEM.BARCODE$(6),                             \   
                    CCMVT.ITEM.QTY$(6),                                 \          
                    CCMVT.ITEM.PRICE$(6),                               \          
                    CCMVT.ITEM.FILLER.02$(6),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(7),                        \   
                    CCMVT.ITEM.BARCODE$(7),                             \   
                    CCMVT.ITEM.QTY$(7),                                 \          
                    CCMVT.ITEM.PRICE$(7),                               \          
                    CCMVT.ITEM.FILLER.02$(7),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(8),                        \   
                    CCMVT.ITEM.BARCODE$(8),                             \   
                    CCMVT.ITEM.QTY$(8),                                 \          
                    CCMVT.ITEM.PRICE$(8),                               \          
                    CCMVT.ITEM.FILLER.02$(8),                           \                   
                    CCMVT.ITEM.BARCODE.FLAG$(9),                        \   
                    CCMVT.ITEM.BARCODE$(9),                             \   
                    CCMVT.ITEM.QTY$(9),                                 \          
                    CCMVT.ITEM.PRICE$(9),                               \          
                    CCMVT.ITEM.FILLER.02$(9),                           \  
                    CCMVT.ITEM.BARCODE.FLAG$(10),                       \   
                    CCMVT.ITEM.BARCODE$(10),                            \   
                    CCMVT.ITEM.QTY$(10),                                \          
                    CCMVT.ITEM.PRICE$(10),                              \          
                    CCMVT.ITEM.FILLER.02$(10),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(11),                       \   
                    CCMVT.ITEM.BARCODE$(11),                            \   
                    CCMVT.ITEM.QTY$(11),                                \          
                    CCMVT.ITEM.PRICE$(11),                              \          
                    CCMVT.ITEM.FILLER.02$(11),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(12),                       \   
                    CCMVT.ITEM.BARCODE$(12),                            \   
                    CCMVT.ITEM.QTY$(12),                                \          
                    CCMVT.ITEM.PRICE$(12),                              \          
                    CCMVT.ITEM.FILLER.02$(12),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(13),                       \   
                    CCMVT.ITEM.BARCODE$(13),                            \   
                    CCMVT.ITEM.QTY$(13),                                \          
                    CCMVT.ITEM.PRICE$(13),                              \          
                    CCMVT.ITEM.FILLER.02$(13),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(14),                       \   
                    CCMVT.ITEM.BARCODE$(14),                            \   
                    CCMVT.ITEM.QTY$(14),                                \          
                    CCMVT.ITEM.PRICE$(14),                              \          
                    CCMVT.ITEM.FILLER.02$(14),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(15),                       \   
                    CCMVT.ITEM.BARCODE$(15),                            \   
                    CCMVT.ITEM.QTY$(15),                                \          
                    CCMVT.ITEM.PRICE$(15),                              \          
                    CCMVT.ITEM.FILLER.02$(15),                          \  
                    CCMVT.ITEM.BARCODE.FLAG$(16),                       \   
                    CCMVT.ITEM.BARCODE$(16),                            \   
                    CCMVT.ITEM.QTY$(16),                                \          
                    CCMVT.ITEM.PRICE$(16),                              \          
                    CCMVT.ITEM.FILLER.02$(16),                          \       
                    CCMVT.FILLER.02$,                                   \
                    CCMVT.SEQUENCE%                                     \
              ELSE                                                      \
                 IF CCMVT.TRANS.TYPE$     = "03" THEN     \Return record
                   FORMAT$ = "C2,"+STRING$(16,"C1,C7,C2,C1,C4,")+"C12,I2":\
                   WRITE FORM FORMAT$; HOLD #CCMVT.SESS.NUM%;           \
                       CCMVT.TRANS.TYPE$,                               \
                       CCMVT.ITEM.BARCODE.FLAG$(1),                     \   
                       CCMVT.ITEM.BARCODE$(1),                          \   
                       CCMVT.ITEM.QTY$(1),                              \          
                       CCMVT.ITEM.REASON$(1),                           \
                       CCMVT.ITEM.FILLER.03$(1),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(2),                     \   
                       CCMVT.ITEM.BARCODE$(2),                          \   
                       CCMVT.ITEM.QTY$(2),                              \          
                       CCMVT.ITEM.REASON$(2),                           \
                       CCMVT.ITEM.FILLER.03$(2),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(3),                     \   
                       CCMVT.ITEM.BARCODE$(3),                          \   
                       CCMVT.ITEM.QTY$(3),                              \          
                       CCMVT.ITEM.REASON$(3),                           \
                       CCMVT.ITEM.FILLER.03$(3),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(4),                     \   
                       CCMVT.ITEM.BARCODE$(4),                          \   
                       CCMVT.ITEM.QTY$(4),                              \          
                       CCMVT.ITEM.REASON$(4),                           \
                       CCMVT.ITEM.FILLER.03$(4),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(5),                     \   
                       CCMVT.ITEM.BARCODE$(5),                          \   
                       CCMVT.ITEM.QTY$(5),                              \          
                       CCMVT.ITEM.REASON$(5),                           \
                       CCMVT.ITEM.FILLER.03$(5),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(6),                     \   
                       CCMVT.ITEM.BARCODE$(6),                          \   
                       CCMVT.ITEM.QTY$(6),                              \          
                       CCMVT.ITEM.REASON$(6),                           \
                       CCMVT.ITEM.FILLER.03$(6),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(7),                     \   
                       CCMVT.ITEM.BARCODE$(7),                          \   
                       CCMVT.ITEM.QTY$(7),                              \          
                       CCMVT.ITEM.REASON$(7),                           \
                       CCMVT.ITEM.FILLER.03$(7),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(8),                     \   
                       CCMVT.ITEM.BARCODE$(8),                          \   
                       CCMVT.ITEM.QTY$(8),                              \          
                       CCMVT.ITEM.REASON$(8),                           \
                       CCMVT.ITEM.FILLER.03$(8),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(9),                     \   
                       CCMVT.ITEM.BARCODE$(9),                          \   
                       CCMVT.ITEM.QTY$(9),                              \          
                       CCMVT.ITEM.REASON$(9),                           \
                       CCMVT.ITEM.FILLER.03$(9),                        \  
                       CCMVT.ITEM.BARCODE.FLAG$(10),                    \   
                       CCMVT.ITEM.BARCODE$(10),                         \   
                       CCMVT.ITEM.QTY$(10),                             \          
                       CCMVT.ITEM.REASON$(10),                          \
                       CCMVT.ITEM.FILLER.03$(10),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(11),                    \   
                       CCMVT.ITEM.BARCODE$(11),                         \   
                       CCMVT.ITEM.QTY$(11),                             \          
                       CCMVT.ITEM.REASON$(11),                          \
                       CCMVT.ITEM.FILLER.03$(11),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(12),                    \   
                       CCMVT.ITEM.BARCODE$(12),                         \   
                       CCMVT.ITEM.QTY$(12),                             \          
                       CCMVT.ITEM.REASON$(12),                          \
                       CCMVT.ITEM.FILLER.03$(12),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(13),                    \   
                       CCMVT.ITEM.BARCODE$(13),                         \   
                       CCMVT.ITEM.QTY$(13),                             \          
                       CCMVT.ITEM.REASON$(13),                          \
                       CCMVT.ITEM.FILLER.03$(13),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(14),                    \   
                       CCMVT.ITEM.BARCODE$(14),                         \   
                       CCMVT.ITEM.QTY$(14),                             \          
                       CCMVT.ITEM.REASON$(14),                          \
                       CCMVT.ITEM.FILLER.03$(14),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(15),                    \   
                       CCMVT.ITEM.BARCODE$(15),                         \   
                       CCMVT.ITEM.QTY$(15),                             \          
                       CCMVT.ITEM.REASON$(15),                          \
                       CCMVT.ITEM.FILLER.03$(15),                       \  
                       CCMVT.ITEM.BARCODE.FLAG$(16),                    \   
                       CCMVT.ITEM.BARCODE$(16),                         \   
                       CCMVT.ITEM.QTY$(16),                             \          
                       CCMVT.ITEM.REASON$(16),                          \
                       CCMVT.ITEM.FILLER.03$(16),                       \                
                       CCMVT.FILLER.03$,                                \
                       CCMVT.SEQUENCE%                                  \                                 
                 ELSE                                                   \
                   IF CCMVT.TRANS.TYPE$     = "04" THEN     \Local Pricing recd
                    FORMAT$ = "C2,"+STRING$(8,"C4,C3,C4,C6,C1,C12,")+"C12,I2":\
                    WRITE FORM FORMAT$; HOLD #CCMVT.SESS.NUM%;          \       
                       CCMVT.TRANS.TYPE$,                               \
                       CCMVT.ITEM.ITEM.CODE.04$(1),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(1),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(1),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(1),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(1),                   \
                       CCMVT.ITEM.FILLER.04$(1),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(2),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(2),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(2),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(2),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(2),                   \
                       CCMVT.ITEM.FILLER.04$(2),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(3),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(3),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(3),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(3),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(3),                   \
                       CCMVT.ITEM.FILLER.04$(3),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(4),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(4),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(4),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(4),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(4),                   \
                       CCMVT.ITEM.FILLER.04$(4),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(5),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(5),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(5),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(5),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(5),                   \
                       CCMVT.ITEM.FILLER.04$(5),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(6),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(6),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(6),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(6),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(6),                   \
                       CCMVT.ITEM.FILLER.04$(6),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(7),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(7),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(7),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(7),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(7),                   \
                       CCMVT.ITEM.FILLER.04$(7),                        \  
                       CCMVT.ITEM.ITEM.CODE.04$(8),                     \   
                       CCMVT.ITEM.REASON.CODE.04$(8),                   \          
                       CCMVT.ITEM.AUTH.NUM.04$(8),                      \          
                       CCMVT.ITEM.STOCK.VALUE.04$(8),                   \
                       CCMVT.ITEM.INCR.DECR.FLAG$(8),                   \
                       CCMVT.ITEM.FILLER.04$(8),                        \                
                       CCMVT.FILLER.04$,                                \
                       CCMVT.SEQUENCE%                                  \                                 
                 ELSE                                                   \
                    IF CCMVT.TRANS.TYPE$ = "99" THEN       \Trailer record
                      WRITE FORM "C2,C6,C246,I2"; HOLD #CCMVT.SESS.NUM%;\
                          CCMVT.TRANS.TYPE$,                            \
                          CCMVT.RECORD.COUNT$,                          \
                          CCMVT.TRAILER.FILLER$,                        \
                          CCMVT.SEQUENCE%

     WRITE.HOLD.CCMVT = 0
     EXIT FUNCTION

     WRITE.HOLD.CCMVT.ERROR:

        CURRENT.REPORT.NUM% = CCMVT.REPORT.NUM%
        FILE.OPERATION$ = "W"
        CURRENT.CODE$ = ""
        EXIT FUNCTION

  END FUNCTION

