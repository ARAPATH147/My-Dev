  
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCRSNFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               FILE OF RETURN REASON CODES
\***
\***
\***      VERSION A : Michael J. Kelsall      14th September 1993
\***
\***      VERSION B : Andy Cotton             17th November 2003
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCRSNDEC.J86



  FUNCTION CCRSN.SET PUBLIC

     INTEGER*2 CCRSN.SET
     CCRSN.SET = 1

       CCRSN.REPORT.NUM% = 319
       CCRSN.RECL%       = 50
       CCRSN.FILE.NAME$  = "CCRSN"
  
     CCRSN.SET = 0

  END FUNCTION



  FUNCTION READ.CCRSN PUBLIC

    INTEGER*2 READ.CCRSN
       READ.CCRSN = 1    
       IF END #CCRSN.SESS.NUM% THEN READ.ERROR   
       READ FORM "T2,C30,C1,C1,C1,C16"; #CCRSN.SESS.NUM%                         \
         KEY CCRSN.REASON$;                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \
             CCRSN.ALTERNATE.REASON$,                                   \ BAC 
             CCRSN.FILLER$              
       READ.CCRSN = 0
       EXIT FUNCTION     

    READ.ERROR:
       CURRENT.CODE$ = CCRSN.REASON$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
       EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCRSN.LOCKED PUBLIC

    INTEGER*2 READ.CCRSN.LOCKED
    
       READ.CCRSN.LOCKED = 1    
       IF END #CCRSN.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T2,C30,C1,C1,C1,C16"; #CCRSN.SESS.NUM% AUTOLOCK                \
         KEY CCRSN.REASON$;                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \
             CCRSN.ALTERNATE.REASON$,                                   \ BAC 
             CCRSN.FILLER$              
       READ.CCRSN.LOCKED = 0
       EXIT FUNCTION     
        
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCRSN.REASON$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCRSN PUBLIC

    INTEGER*2 WRITE.HOLD.CCRSN
    
       WRITE.HOLD.CCRSN = 1
       IF END #CCRSN.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C1,C30,C1,C1,C1,C16"; HOLD #CCRSN.SESS.NUM%;                  \        
             CCRSN.REASON$,                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \
             CCRSN.ALTERNATE.REASON$,                                   \ BAC 
             CCRSN.FILLER$              
       WRITE.HOLD.CCRSN = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
       CURRENT.CODE$ = CCRSN.REASON$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCRSN PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCRSN
    
       WRITE.UNLOCK.CCRSN = 1
       IF END #CCRSN.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C1,C30,C1,C1,C1,C16"; #CCRSN.SESS.NUM% AUTOUNLOCK;                    \        
             CCRSN.REASON$,                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \
             CCRSN.ALTERNATE.REASON$,                                   \ BAC 
             CCRSN.FILLER$              
       WRITE.UNLOCK.CCRSN = 0
       EXIT FUNCTION         
     
    WRITE.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
       CURRENT.CODE$ = CCRSN.REASON$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCRSN PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCRSN
    
       WRITE.HOLD.UNLOCK.CCRSN = 1
       IF END #CCRSN.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C1,C30,C1,C1,C1,C16"; HOLD #CCRSN.SESS.NUM% AUTOUNLOCK;       \        
             CCRSN.REASON$,                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \
             CCRSN.ALTERNATE.REASON$,                                   \ BAC 
             CCRSN.FILLER$              
       WRITE.HOLD.UNLOCK.CCRSN = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
       CURRENT.CODE$ = CCRSN.REASON$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCRSN PUBLIC

    INTEGER*2 WRITE.CCRSN
       WRITE.CCRSN = 1
       IF END #CCRSN.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C1,C30,C1,C1,C1,C16"; #CCRSN.SESS.NUM%;               \        
             CCRSN.REASON$,                                             \         
             CCRSN.DESC$,                                               \
             CCRSN.PSS30.REQ$,                                          \ 
             CCRSN.PSS93.REQ$,                                          \ 
             CCRSN.ALTERNATE.REASON$,                                   \ BAC
             CCRSN.FILLER$              
       WRITE.CCRSN = 0
       EXIT FUNCTION         
     
    WRITE.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
       CURRENT.CODE$ = CCRSN.REASON$
       EXIT FUNCTION    

  END FUNCTION

