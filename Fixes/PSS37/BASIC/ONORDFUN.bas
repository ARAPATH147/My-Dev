\*******************************************************************************
\*******************************************************************************
\***
\***          CSR ON ORDER FILE FUNCTIONS
\***
\***          REFERENCE    : ONORDFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                         
  INTEGER*2 GLOBAL              \
     CURRENT.REPORT.NUM%
     
  STRING GLOBAL                 \
     CURRENT.CODE$,             \
     FILE.OPERATION$
     
  %INCLUDE ONORDDEC.J86
  
  FUNCTION ONORD.SET PUBLIC
\***************************

    ONORD.REPORT.NUM%  = 188      
    ONORD.RECL% = 32
    ONORD.FILE.NAME$  = "ONORD"

  END FUNCTION
\-----------------------------------------------------------------------------
                                                                     
  FUNCTION READ.ONORDER PUBLIC
\******************************
  
    INTEGER*2 READ.ONORDER
    
    READ.ONORDER = 1
    
    ONORD.KEY$ = ONORD.ORDER.DATE$ + ONORD.LIST.FREQ$ +          \
                 ONORD.UNIT.NO$ + ONORD.SEQ.NO$
    IF VAL(ONORD.SEQ.NO$) = 0 THEN BEGIN 
       IF END #ONORD.SESS.NUM% THEN READ.ERROR 
       READ FORM "T13,C3,C17"; #ONORD.SESS.NUM%                  \
         KEY ONORD.KEY$;                                         \
         ONORD.HIGHEST.SEQ.NO$,                                  \
         ONORD.FILLER$                                           \
    ENDIF ELSE BEGIN
       IF END #ONORD.SESS.NUM% THEN READ.ERROR                 
       READ FORM "T13,C7,C3,C3,C1,C1,C1,C4"; #ONORD.SESS.NUM%    \
         KEY ONORD.KEY$;                                         \
         ONORD.ITEM.CODE$,                                       \
         ONORD.QTY.1$,                                           \
         ONORD.QTY.2$,                                           \
         ONORD.CSRITEM.UNDONE.FLAG$,                             \
         ONORD.CSRIMF.UNDONE.FLAG$,                              \
         ONORD.DELETED.FLAG$,                                    \
         ONORD.FILLER$    
    ENDIF        
    READ.ONORDER = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = ONORD.KEY$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = ONORD.REPORT.NUM%
       
       EXIT FUNCTION
                 
   END FUNCTION                         
\-----------------------------------------------------------------------------
    

  FUNCTION WRITE.ONORDER PUBLIC
\*******************************
   
      INTEGER*2 WRITE.ONORDER
      
      WRITE.ONORDER = 1
         
      IF VAL(ONORD.SEQ.NO$) = 0 THEN BEGIN
         ONORD.FILLER$ = STRING$(17," ")        
         IF END #ONORD.SESS.NUM% THEN WRITE.ERROR
         WRITE FORM "C6,C1,C2,C3,C3,C17"; #ONORD.SESS.NUM%;     \
            ONORD.ORDER.DATE$,                                  \
            ONORD.LIST.FREQ$,                                   \
            ONORD.UNIT.NO$,                                     \
            ONORD.SEQ.NO$,                                      \
            ONORD.HIGHEST.SEQ.NO$,                              \
            ONORD.FILLER$                                       \
      ENDIF ELSE BEGIN                                          
         ONORD.FILLER$ = STRING$(4," ")         
         IF END #ONORD.SESS.NUM% THEN WRITE.ERROR  
         WRITE FORM "C6,C1,C2,C3,C7,C3,C3,C1,C1,C1,C4";         \        
            #ONORD.SESS.NUM%;                                   \
            ONORD.ORDER.DATE$,                                  \
            ONORD.LIST.FREQ$,                                   \
            ONORD.UNIT.NO$,                                     \
            ONORD.SEQ.NO$,                                      \
            ONORD.ITEM.CODE$,                                   \
            ONORD.QTY.1$,                                       \
            ONORD.QTY.2$,                                       \
            ONORD.CSRITEM.UNDONE.FLAG$,                         \
            ONORD.CSRIMF.UNDONE.FLAG$,                          \
            ONORD.DELETED.FLAG$,                                \
            ONORD.FILLER$
      ENDIF         
      WRITE.ONORDER = 0
      EXIT FUNCTION      
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = ONORD.ORDER.DATE$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = ONORD.REPORT.NUM%
         
         EXIT FUNCTION

   END FUNCTION
\-----------------------------------------------------------------------------
   

  FUNCTION WRITE.ONORDER.HOLD PUBLIC
\************************************

      INTEGER*2 WRITE.ONORDER.HOLD
      
      WRITE.ONORDER.HOLD = 1
   
      IF VAL(ONORD.SEQ.NO$) = 0 THEN BEGIN
         ONORD.FILLER$ = STRING$(17," ")        
         IF END #ONORD.SESS.NUM% THEN WRITE.HOLD.ERROR
         WRITE FORM "C6,C1,C2,C3,C3,C17"; HOLD #ONORD.SESS.NUM%;\
            ONORD.ORDER.DATE$,                                  \
            ONORD.LIST.FREQ$,                                   \
            ONORD.UNIT.NO$,                                     \
            ONORD.SEQ.NO$,                                      \
            ONORD.HIGHEST.SEQ.NO$,                              \
            ONORD.FILLER$                                       \
      ENDIF ELSE BEGIN
         ONORD.FILLER$ = STRING$(4," ")
         IF END #ONORD.SESS.NUM% THEN WRITE.HOLD.ERROR
         WRITE FORM "C6,C1,C2,C3,C7,C3,C3,C1,C1,C1,C4"; HOLD    \        
            #ONORD.SESS.NUM%;                                   \
            ONORD.ORDER.DATE$,                                  \
            ONORD.LIST.FREQ$,                                   \
            ONORD.UNIT.NO$,                                     \
            ONORD.SEQ.NO$,                                      \
            ONORD.ITEM.CODE$,                                   \
            ONORD.QTY.1$,                                       \
            ONORD.QTY.2$,                                       \
            ONORD.CSRITEM.UNDONE.FLAG$,                         \
            ONORD.CSRIMF.UNDONE.FLAG$,                          \
            ONORD.DELETED.FLAG$,                                \
            ONORD.FILLER$
     ENDIF
     WRITE.ONORDER.HOLD = 0
     EXIT FUNCTION
     
     WRITE.HOLD.ERROR:
     
        CURRENT.CODE$ = ONORD.ORDER.DATE$
        FILE.OPERATION$ = "O"
        CURRENT.REPORT.NUM% = ONORD.REPORT.NUM%
        
        EXIT FUNCTION

   END FUNCTION                                                                                           
                                     
