
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  MINLSFUN.BAS
\***
\***	             DESCRIPTION:  MINSITS RP - SCREEN COUNT INFORMATION
\***
\***
\***
\***      VERSION 1 : Julia Stones             8th January 1998  
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
  
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$         

  %INCLUDE MINLSDEC.J86



  FUNCTION MINLS.SET PUBLIC

     INTEGER*2 MINLS.SET
     MINLS.SET = 1

       MINLS.REPORT.NUM% = 547                                   
       MINLS.RECL%      = 11 
       MINLS.FILE.NAME$ = "MINLS"
  
     MINLS.SET = 0

  END FUNCTION



  FUNCTION READ.MINLS PUBLIC

    INTEGER*2 READ.MINLS
    
    READ.MINLS = 1    

    IF END #MINLS.SESS.NUM% THEN READ.ERROR
    READ FORM "T5,C3,C3,C1";  \
            #MINLS.SESS.NUM% KEY MINLS.ITEM.CODE$;   \
                             MINLS.RECOUNT.DATE$,    \
                             MINLS.DISCREPANCY$,     \
                             MINLS.COUNT.STATUS$     !
       READ.MINLS = 0
       EXIT FUNCTION
 
    READ.ERROR:

        CURRENT.CODE$ = MINLS.ITEM.CODE$
 FILE.OPERATION$ = "R"
 CURRENT.REPORT.NUM% = MINLS.REPORT.NUM%
 EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.MINLS.LOCKED PUBLIC

    INTEGER*2 READ.MINLS.LOCKED
    
    READ.MINLS.LOCKED = 1    
       IF END #MINLS.SESS.NUM% THEN READ.LOCKED.ERROR
       READ FORM "T5,C3,C3,C1";  \
            #MINLS.SESS.NUM% AUTOLOCK KEY MINLS.ITEM.CODE$;      \
            MINLS.RECOUNT.DATE$,      \
            MINLS.DISCREPANCY$,       \
            MINLS.COUNT.STATUS$      !
       READ.MINLS.LOCKED = 0
       EXIT FUNCTION
 
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = MINLS.ITEM.CODE$
 FILE.OPERATION$ = "R"
 CURRENT.REPORT.NUM% = MINLS.REPORT.NUM%
    
 EXIT FUNCTION

  END FUNCTION  


  FUNCTION WRITE.UNLOCK.MINLS PUBLIC

    INTEGER*2 WRITE.UNLOCK.MINLS
    
    WRITE.UNLOCK.MINLS = 1

    IF END #MINLS.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C4,C3,C3,C1";   \
        #MINLS.SESS.NUM% AUTOUNLOCK;   \
                 MINLS.ITEM.CODE$,     \
                 MINLS.RECOUNT.DATE$,  \
                 MINLS.DISCREPANCY$,   \
                 MINLS.COUNT.STATUS$   !

       WRITE.UNLOCK.MINLS = 0
       EXIT FUNCTION
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = MINLS.REPORT.NUM%
       CURRENT.CODE$ = MINLS.ITEM.CODE$
    
       EXIT FUNCTION    

  END FUNCTION

  FUNCTION WRITE.MINLS PUBLIC

    INTEGER*2 WRITE.MINLS
    
    WRITE.MINLS = 1

    IF END #MINLS.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C4,C3,C3,C1";   \
        #MINLS.SESS.NUM%;            \
                MINLS.ITEM.CODE$,      \   
                MINLS.RECOUNT.DATE$,  \
                MINLS.DISCREPANCY$,   \
                MINLS.COUNT.STATUS$   !

       WRITE.MINLS = 0
       EXIT FUNCTION
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = MINLS.REPORT.NUM%
       CURRENT.CODE$ = MINLS.ITEM.CODE$
    
       EXIT FUNCTION    

  END FUNCTION

