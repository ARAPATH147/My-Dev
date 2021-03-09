
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCLAMFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               FILE OF CURRENT CREDIT CLAIMS
\***
\***
\***      VERSION A : Michael J. Kelsall      16th December 1993
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCLAMDEC.J86



  FUNCTION CCLAM.SET PUBLIC

     INTEGER*2 CCLAM.SET
     CCLAM.SET = 1
       CCLAM.REPORT.NUM% = 316
       CCLAM.RECL%      = 160
       CCLAM.FILE.NAME$ = "CCLAM"
     CCLAM.SET = 0

  END FUNCTION



  FUNCTION READ.CCLAM PUBLIC

    INTEGER*2 READ.CCLAM
    STRING FORMAT$
    READ.CCLAM = 1    
       FORMAT$ = "T5,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +               \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN READ.ERROR   
       READ FORM FORMAT$; #CCLAM.SESS.NUM% KEY CCLAM.CREDIT.CLAIM.NUM$; \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$             
       READ.CCLAM = 0
       EXIT FUNCTION     
        
    READ.ERROR:

        CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
        EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCLAM.LOCKED PUBLIC

    INTEGER*2 READ.CCLAM.LOCKED
    STRING FORMAT$
    READ.CCLAM.LOCKED = 1    
       FORMAT$ = "T5,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +               \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM FORMAT$; #CCLAM.SESS.NUM% AUTOLOCK                     \
          KEY CCLAM.CREDIT.CLAIM.NUM$;                                  \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$             
       READ.CCLAM.LOCKED = 0
       EXIT FUNCTION     
        
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCLAM PUBLIC

    INTEGER*2 WRITE.HOLD.CCLAM
    STRING FORMAT$
    WRITE.HOLD.CCLAM = 1
       FORMAT$ = "C4,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +               \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM FORMAT$; HOLD #CCLAM.SESS.NUM%;                       \        
              CCLAM.CREDIT.CLAIM.NUM$,                                  \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$             
       WRITE.HOLD.CCLAM = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCLAM PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCLAM
    STRING FORMAT$
    WRITE.UNLOCK.CCLAM = 1
       FORMAT$ = "C4,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +               \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM FORMAT$; #CCLAM.SESS.NUM% AUTOUNLOCK;                 \
              CCLAM.CREDIT.CLAIM.NUM$,                                  \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$                     
       WRITE.UNLOCK.CCLAM = 0
       EXIT FUNCTION         
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCLAM PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCLAM
    STRING FORMAT$
    WRITE.HOLD.UNLOCK.CCLAM = 1
       FORMAT$ = "C4,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +               \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM FORMAT$; HOLD #CCLAM.SESS.NUM% AUTOUNLOCK;            \
              CCLAM.CREDIT.CLAIM.NUM$,                                  \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$                     
       WRITE.HOLD.UNLOCK.CCLAM = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCLAM PUBLIC

    INTEGER*2 WRITE.CCLAM
    STRING FORMAT$
    WRITE.CCLAM = 1
       FORMAT$ = "C4,C7,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,C9,2C3" +       \
                 ",2C1,2C6,C4,C9,C3,C1,C7,C20,2C3,3C1,C7"
       IF END #CCLAM.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; #CCLAM.SESS.NUM%;                            \     
              CCLAM.CREDIT.CLAIM.NUM$,                                  \
              CCLAM.UOD.NUM$,                                           \  
              CCLAM.NUM.OF.ITEMS%,                                      \
              CCLAM.SUPPLY.ROUTE$,                                      \        
              CCLAM.DISP.LOCATION$,                                     \  
              CCLAM.BC.LETTER$,                                         \  
              CCLAM.RECALL.NUM$,                                        \  
              CCLAM.AUTHORISATION$,                                     \ 
              CCLAM.SUPPLIER$,                                          \ 
              CCLAM.METHOD.OF.RETURN$,                                  \  
              CCLAM.CARRIER$,                                           \  
              CCLAM.BIRD.NUM$,                                          \  
              CCLAM.REASON.NUM$,                                        \  
              CCLAM.RECEIVING.STORE$,                                   \  
              CCLAM.DESTINATION$,                                       \  
              CCLAM.WAREHOUSE.ROUTE$,                                   \  
              CCLAM.UOD.TYPE$,                                          \  
              CCLAM.DAMAGE.REASON$,                                     \  
              CCLAM.INVOICE.NUM$,                                       \  
              CCLAM.FOLIO.NUM$,                                         \  
              CCLAM.BATCH.REF$,                                         \  
              CCLAM.WHOLE.PART.CON$,                                    \
              CCLAM.REPAIR.CATEGORY$,                                   \  
              CCLAM.REPAIR.NUM$,                                        \  
              CCLAM.PLAN4.POLICY.NUM$,                                  \  
              CCLAM.DDDA.DCDR.NUM$,                                     \  
              CCLAM.DELIV.NOTE.NUM$,                                    \  
              CCLAM.DELIV.DATE$,                                        \
              CCLAM.NUM.CARTONS.RECEIV$,                                \  
              CCLAM.ORDER.NUM$,                                         \  
              CCLAM.COMMENT$,                                           \  
              CCLAM.DATE.OF.CLAIM$,                                     \  
              CCLAM.TIME.OF.CLAIM$,                                     \  
              CCLAM.RETRIEVAL.FLAG$,                                    \  
              CCLAM.CF.RPT.MARKER$,                                     \
              CCLAM.CANC.MARKER$,                                       \  
              CCLAM.FILLER$                   
       WRITE.CCLAM = 0
       EXIT FUNCTION         
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       CURRENT.CODE$ = CCLAM.CREDIT.CLAIM.NUM$
    
       EXIT FUNCTION    

  END FUNCTION

