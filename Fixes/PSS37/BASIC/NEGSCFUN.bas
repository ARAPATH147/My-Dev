
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  NEGSCFUN.BAS
\***
\***                 DESCRIPTION:  CURRENT NEGATIVE STOCK COUNT INFORMATION
\***
\***
\***
\***      VERSION 1 : Julia Stones             15th July 1999  
\***      
\***    REVISION 1.2.            ROBERT COWEY.                  09 SEP 2003.
\***    Changes for RF trial.
\***    Recompiled to prevent future automatic recompiles.
\***    No changes to actual code.
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
  
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$         

  %INCLUDE NEGSCDEC.J86



  FUNCTION NEGSC.SET PUBLIC

     INTEGER*2 NEGSC.SET
     NEGSC.SET = 1

       NEGSC.REPORT.NUM% = 582                                   
       NEGSC.RECL%      = 10
       NEGSC.FILE.NAME$ = "NEGSC"
  
     NEGSC.SET = 0

  END FUNCTION



  FUNCTION READ.NEGSC PUBLIC

    INTEGER*2 READ.NEGSC
    
    READ.NEGSC = 1    

    IF END #NEGSC.SESS.NUM% THEN READ.ERROR
    READ FORM "T6,C1,2I2";  \
            #NEGSC.SESS.NUM% KEY NEGSC.KEY$;         \
                             NEGSC.STATUS.1$,        \
                             NEGSC.ITEM.TSF%,        \
                             NEGSC.NUMBER.OF.DAYS%    !                  
       READ.NEGSC = 0
       EXIT FUNCTION
 
    READ.ERROR:

        CURRENT.CODE$ = PACK$(MID$("0" + NEGSC.KEY$,2,4))
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = NEGSC.REPORT.NUM%
 EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.NEGSC.LOCKED PUBLIC

    INTEGER*2 READ.NEGSC.LOCKED
    
    READ.NEGSC.LOCKED = 1    
       IF END #NEGSC.SESS.NUM% THEN READ.LOCKED.ERROR
       READ FORM "T6,C1,2I2";  \
            #NEGSC.SESS.NUM% AUTOLOCK KEY NEGSC.KEY$;            \
            NEGSC.STATUS.1$,                                     \
            NEGSC.ITEM.TSF%,                                     \
            NEGSC.NUMBER.OF.DAYS%      !
       READ.NEGSC.LOCKED = 0
       EXIT FUNCTION
 
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = PACK$(MID$("0" + NEGSC.KEY$,2,4))
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = NEGSC.REPORT.NUM%
    
 EXIT FUNCTION

  END FUNCTION  


  FUNCTION WRITE.UNLOCK.NEGSC PUBLIC

    INTEGER*2 WRITE.UNLOCK.NEGSC
    
    WRITE.UNLOCK.NEGSC = 1

    IF END #NEGSC.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C5,C1,2I2";   \
        #NEGSC.SESS.NUM% AUTOUNLOCK;   \
                 NEGSC.KEY$,           \
                 NEGSC.STATUS.1$,      \
                 NEGSC.ITEM.TSF%,      \
                 NEGSC.NUMBER.OF.DAYS%  !

       WRITE.UNLOCK.NEGSC = 0
       EXIT FUNCTION
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = NEGSC.REPORT.NUM%
       CURRENT.CODE$ = PACK$(MID$("0" + NEGSC.KEY$,2,4))
    
       EXIT FUNCTION    

  END FUNCTION

  FUNCTION WRITE.NEGSC PUBLIC

    INTEGER*2 WRITE.NEGSC
    
    WRITE.NEGSC = 1

    IF END #NEGSC.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C5,C1,2I2";   \
        #NEGSC.SESS.NUM%;            \
                NEGSC.KEY$,            \
                NEGSC.STATUS.1$,       \
                NEGSC.ITEM.TSF%,       \   
                NEGSC.NUMBER.OF.DAYS%  !

       WRITE.NEGSC = 0
       EXIT FUNCTION
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = NEGSC.REPORT.NUM%
       CURRENT.CODE$ = PACK$(MID$("0" + NEGSC.KEY$,2,4))
    
       EXIT FUNCTION    

  END FUNCTION

