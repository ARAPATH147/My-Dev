
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCITFFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               FILE OF ITEMS PER CREDIT CLAIM
\***
\***
\***      VERSION A : Michael J. Kelsall      14th September 1993
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCITFDEC.J86



  FUNCTION CCITF.SET PUBLIC

     INTEGER*2 CCITF.SET
     CCITF.SET = 1
       CCITF.REPORT.NUM% = 317
       CCITF.RECL%       = 23
       CCITF.FILE.NAME$  = "CCITF"
     CCITF.SET = 0

  END FUNCTION



  FUNCTION READ.CCITF PUBLIC

    INTEGER*2 READ.CCITF
    READ.CCITF = 1    
       IF END #CCITF.SESS.NUM% THEN READ.ERROR   
       READ FORM "T7,C1,C7,I2,C3,C4"; #CCITF.SESS.NUM%                  \
          KEY CCITF.KEY$;                                               \
              CCITF.ITEM.BAR.CODE.FLAG$,                                \         
              CCITF.BOOTS.BAR.CODE$,                                    \  
              CCITF.QTY%,                                               \
              CCITF.PRICE$,                                             \  
              CCITF.FILLER$
       READ.CCITF = 0
       EXIT FUNCTION     
        
    READ.ERROR:

        CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
        EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCITF.LOCKED PUBLIC

    INTEGER*2 READ.CCITF.LOCKED
    READ.CCITF.LOCKED = 1    
       IF END #CCITF.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T7,C1,C7,I2,C3,C4"; #CCITF.SESS.NUM% AUTOLOCK         \
          KEY  CCITF.KEY$;                                              \
               CCITF.ITEM.BAR.CODE.FLAG$,                               \         
               CCITF.BOOTS.BAR.CODE$,                                   \  
               CCITF.QTY%,                                              \
               CCITF.PRICE$,                                            \  
               CCITF.FILLER$
       READ.CCITF.LOCKED = 0
       EXIT FUNCTION     
        
    READ.LOCKED.ERROR:
        CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
        EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCITF PUBLIC

    INTEGER*2 WRITE.HOLD.CCITF
    WRITE.HOLD.CCITF = 1
       IF END #CCITF.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C6,C1,C7,I2,C3,C4"; HOLD #CCITF.SESS.NUM%;                   \        
              CCITF.KEY$,                                               \
              CCITF.ITEM.BAR.CODE.FLAG$,                                \         
              CCITF.BOOTS.BAR.CODE$,                                    \  
              CCITF.QTY%,                                               \
              CCITF.PRICE$,                                             \  
              CCITF.FILLER$
       WRITE.HOLD.CCITF = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCITF PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCITF
    WRITE.UNLOCK.CCITF = 1
       IF END #CCITF.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C6,I2,C1,C7,I2,C3,C4"; #CCITF.SESS.NUM% AUTOUNLOCK;  \
              CCITF.KEY$,                                               \
              CCITF.ITEM.BAR.CODE.FLAG$,                                \         
              CCITF.BOOTS.BAR.CODE$,                                    \  
              CCITF.QTY%,                                               \
              CCITF.PRICE$,                                             \  
              CCITF.FILLER$
       WRITE.UNLOCK.CCITF = 0
       EXIT FUNCTION         
     
    WRITE.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCITF PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCITF
    WRITE.HOLD.UNLOCK.CCITF = 1
       IF END #CCITF.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C6,C1,C7,I2,C3,C4"; HOLD                             \
             #CCITF.SESS.NUM% AUTOUNLOCK;                               \
              CCITF.KEY$,                                               \
              CCITF.ITEM.BAR.CODE.FLAG$,                                \         
              CCITF.BOOTS.BAR.CODE$,                                    \  
              CCITF.QTY%,                                               \
              CCITF.PRICE$,                                             \  
              CCITF.FILLER$
       WRITE.HOLD.UNLOCK.CCITF = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCITF PUBLIC

    INTEGER*2 WRITE.CCITF
    WRITE.CCITF = 1
       IF END #CCITF.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C6,C1,C7,I2,C3,C4"; #CCITF.SESS.NUM%;                \     
              CCITF.KEY$,                                               \
              CCITF.ITEM.BAR.CODE.FLAG$,                                \         
              CCITF.BOOTS.BAR.CODE$,                                    \  
              CCITF.QTY%,                                               \
              CCITF.PRICE$,                                             \  
              CCITF.FILLER$
       WRITE.CCITF = 0
       EXIT FUNCTION         
     
    WRITE.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       CURRENT.CODE$ = CCITF.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION

