
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCUODFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               FILE OF OPEN/CLOSED UODS
\***
\***
\***    VERSION A : Michael J. Kelsall      13th September 1993
\***      
\***    VERSION B : Mark Walker                   18th Jul 2015
\***    F392 Retail Stock 5
\***    Added CCUOD.RETRIEVAL.FLAG$ field to allow record to be
\***    marked as processed.
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCUODDEC.J86



  FUNCTION CCUOD.SET PUBLIC

     INTEGER*2 CCUOD.SET
     CCUOD.SET = 1

       CCUOD.REPORT.NUM% = 314                                   
       CCUOD.RECL%      = 100
       CCUOD.FILE.NAME$ = "CCUOD"
  
     CCUOD.SET = 0

  END FUNCTION



  FUNCTION READ.CCUOD PUBLIC

    INTEGER*2 READ.CCUOD
    
    READ.CCUOD = 1    

    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN    ! header record
       IF END #CCUOD.SESS.NUM% THEN READ.ERROR   
       READ FORM "T8,3C3,C84"; #CCUOD.SESS.NUM% KEY CCUOD.UOD.NUM$;     \
                CCUOD.DATE.FILE.UPDATED$,                               \
                CCUOD.TIME.FILE.UPDATED$,                               \
                CCUOD.LDT.NUM$,                                         \
                CCUOD.HEADER.FILLER$
       READ.CCUOD = 0
       EXIT FUNCTION     
    ENDIF ELSE BEGIN                                    ! detail record
       IF END #CCUOD.SESS.NUM% THEN READ.ERROR
       READ FORM "T8,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18"; \   !BMW
            #CCUOD.SESS.NUM% KEY CCUOD.UOD.NUM$;                        \
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \
                CCUOD.CARRIER$,                                         \
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \
                CCUOD.RECEIVING.STORE$,                                 \
                CCUOD.DESTINATION$,                                     \
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \
                CCUOD.DAMAGE.REASON$,                                   \
                CCUOD.DATE.UOD.OPENED$,                                 \
                CCUOD.DATE.UOD.DESPATCHED$,                             \
                CCUOD.TIME.UOD.DESPATCHED$,                             \
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       READ.CCUOD = 0
       EXIT FUNCTION
    ENDIF
        
    READ.ERROR:

        CURRENT.CODE$ = CCUOD.UOD.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
        EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCUOD.LOCKED PUBLIC

    INTEGER*2 READ.CCUOD.LOCKED
    
    READ.CCUOD.LOCKED = 1    

    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN    ! header record
       IF END #CCUOD.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T8,3C3,C84"; #CCUOD.SESS.NUM% AUTOLOCK                \
            KEY CCUOD.UOD.NUM$;                                         \
                CCUOD.DATE.FILE.UPDATED$,                               \
                CCUOD.TIME.FILE.UPDATED$,                               \
                CCUOD.LDT.NUM$,                                         \
                CCUOD.HEADER.FILLER$
       READ.CCUOD.LOCKED = 0
       EXIT FUNCTION     
    ENDIF ELSE BEGIN                                    ! detail record
       IF END #CCUOD.SESS.NUM% THEN READ.LOCKED.ERROR
       READ FORM "T8,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18"; \   !BMW
            #CCUOD.SESS.NUM% AUTOLOCK                                   \
            KEY CCUOD.UOD.NUM$;                                         \
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \
                CCUOD.CARRIER$,                                         \
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \
                CCUOD.RECEIVING.STORE$,                                 \
                CCUOD.DESTINATION$,                                     \
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \
                CCUOD.DAMAGE.REASON$,                                   \
                CCUOD.DATE.UOD.OPENED$,                                 \
                CCUOD.DATE.UOD.DESPATCHED$,                             \
                CCUOD.TIME.UOD.DESPATCHED$,                             \
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       READ.CCUOD.LOCKED = 0
       EXIT FUNCTION
    ENDIF
        
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCUOD.UOD.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCUOD PUBLIC

    INTEGER*2 WRITE.HOLD.CCUOD
    
    WRITE.HOLD.CCUOD = 1
      
    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN       ! Header Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C7,3C3,C84"; HOLD #CCUOD.SESS.NUM%;                  \        
                CCUOD.UOD.NUM$,                                         \
                CCUOD.DATE.FILE.UPDATED$,                               \
                CCUOD.TIME.FILE.UPDATED$,                               \
                CCUOD.LDT.NUM$,                                         \
                CCUOD.HEADER.FILLER$
       WRITE.HOLD.CCUOD = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C7,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18";\   !BMW
           HOLD #CCUOD.SESS.NUM%;                                       \
                CCUOD.UOD.NUM$,                                         \
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \
                CCUOD.CARRIER$,                                         \
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \
                CCUOD.RECEIVING.STORE$,                                 \
                CCUOD.DESTINATION$,                                     \
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \
                CCUOD.DAMAGE.REASON$,                                   \
                CCUOD.DATE.UOD.OPENED$,                                 \
                CCUOD.DATE.UOD.DESPATCHED$,                             \
                CCUOD.TIME.UOD.DESPATCHED$,                             \
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       WRITE.HOLD.CCUOD = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.HOLD.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
       CURRENT.CODE$ = CCUOD.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCUOD PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCUOD
    
    WRITE.UNLOCK.CCUOD = 1
      
    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN       ! Header Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C7,3C3,C84"; #CCUOD.SESS.NUM% AUTOUNLOCK;                    \        
                CCUOD.UOD.NUM$,                                         \
                CCUOD.DATE.FILE.UPDATED$,                               \
                CCUOD.TIME.FILE.UPDATED$,                               \
                CCUOD.LDT.NUM$,                                         \
                CCUOD.HEADER.FILLER$
       WRITE.UNLOCK.CCUOD = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C7,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18";\   !BMW
           #CCUOD.SESS.NUM% AUTOUNLOCK;                                 \
                CCUOD.UOD.NUM$,                                         \
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \
                CCUOD.CARRIER$,                                         \
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \
                CCUOD.RECEIVING.STORE$,                                 \
                CCUOD.DESTINATION$,                                     \
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \
                CCUOD.DAMAGE.REASON$,                                   \
                CCUOD.DATE.UOD.OPENED$,                                 \
                CCUOD.DATE.UOD.DESPATCHED$,                             \
                CCUOD.TIME.UOD.DESPATCHED$,                             \
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       WRITE.UNLOCK.CCUOD = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
       CURRENT.CODE$ = CCUOD.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCUOD PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCUOD
    
    WRITE.HOLD.UNLOCK.CCUOD = 1
      
    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN       ! Header Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C7,3C3,C84"; HOLD #CCUOD.SESS.NUM% AUTOUNLOCK;       \        
                CCUOD.UOD.NUM$,                                         \
                CCUOD.DATE.FILE.UPDATED$,                               \
                CCUOD.TIME.FILE.UPDATED$,                               \
                CCUOD.LDT.NUM$,                                         \
                CCUOD.HEADER.FILLER$
       WRITE.HOLD.UNLOCK.CCUOD = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C7,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18";\   !BMW
           HOLD #CCUOD.SESS.NUM% AUTOUNLOCK;                            \
                CCUOD.UOD.NUM$,                                         \
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \
                CCUOD.CARRIER$,                                         \
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \
                CCUOD.RECEIVING.STORE$,                                 \
                CCUOD.DESTINATION$,                                     \
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \
                CCUOD.DAMAGE.REASON$,                                   \
                CCUOD.DATE.UOD.OPENED$,                                 \
                CCUOD.DATE.UOD.DESPATCHED$,                             \
                CCUOD.TIME.UOD.DESPATCHED$,                             \
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       WRITE.HOLD.UNLOCK.CCUOD = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.HOLD.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
       CURRENT.CODE$ = CCUOD.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCUOD PUBLIC

    INTEGER*2 WRITE.CCUOD
    
    WRITE.CCUOD = 1
      
    IF CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??")) THEN BEGIN       ! Header Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C7,3C3,C84"; #CCUOD.SESS.NUM%;                       \        
                CCUOD.UOD.NUM$,                                         \ UPD
                CCUOD.DATE.FILE.UPDATED$,                               \ UPD
                CCUOD.TIME.FILE.UPDATED$,                               \ UPD
                CCUOD.LDT.NUM$,                                         \ UPD
                CCUOD.HEADER.FILLER$
       WRITE.CCUOD = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #CCUOD.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C7,C1,C4,I2,3C1,C8,2C15,2C1,C8,C1,C2,4C1,3C3,C1,C18";\   !BMW
           #CCUOD.SESS.NUM%;                                            \
                CCUOD.UOD.NUM$,                                         \ UPD
                CCUOD.STATUS$,                                          \
                CCUOD.CREDIT.CLAIM.NUM$,                                \
                CCUOD.NUM.OF.ITEMS%,                                    \
                CCUOD.SUPPLY.ROUTE$,                                    \
                CCUOD.DISP.LOCATION$,                                   \
                CCUOD.BC.LETTER$,                                       \
                CCUOD.RECALL.NUM$,                                      \
                CCUOD.AUTHORISATION$,                                   \
                CCUOD.SUPPLIER$,                                        \
                CCUOD.METHOD.OF.RETURN$,                                \ UPD
                CCUOD.CARRIER$,                                         \ UPD
                CCUOD.BIRD.NUM$,                                        \
                CCUOD.REASON.NUM$,                                      \ UPD
                CCUOD.RECEIVING.STORE$,                                 \ UPD
                CCUOD.DESTINATION$,                                     \ UPD
                CCUOD.WAREHOUSE.ROUTE$,                                 \
                CCUOD.UOD.TYPE$,                                        \ UPD
                CCUOD.DAMAGE.REASON$,                                   \ UPD
                CCUOD.DATE.UOD.OPENED$,                                 \ UPD
                CCUOD.DATE.UOD.DESPATCHED$,                             \ UPD
                CCUOD.TIME.UOD.DESPATCHED$,                             \ UPD
                CCUOD.RETRIEVAL.FLAG$,                                  \   !BMW
                CCUOD.FILLER$
       WRITE.CCUOD = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUOD.REPORT.NUM%
       CURRENT.CODE$ = CCUOD.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION

