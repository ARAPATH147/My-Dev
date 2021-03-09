\******************************************************************************
\******************************************************************************
\***
\***         UNITS FILE FUNCTIONS
\***
\***         REFERENCE    : UNITSFUN.BAS
\***
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL                      \
     CURRENT.REPORT.NUM%
     
  STRING GLOBAL                         \
     CURRENT.CODE$,                     \
     FILE.OPERATION$
     
  %INCLUDE UNITSDEC.J86


  FUNCTION UNITS.SET PUBLIC
\***************************

    UNITS.REPORT.NUM% = 185                         
    UNITS.RECL%      = 32
    UNITS.FILE.NAME$ = "UNITS"
    
  END FUNCTION
\-----------------------------------------------------------------------------    

  FUNCTION READ.UNITS PUBLIC
\****************************  

    INTEGER*2 READ.UNITS
    
    READ.UNITS = 1

    IF END #UNITS.SESS.NUM% THEN READ.ERROR   
    READ FORM "T2,C30,C1";                                         \ 
         #UNITS.SESS.NUM%                                          \
         KEY UNITS.UNIT$;                                          \
             UNITS.UNIT.NAME$,                                     \
             UNITS.BC.LETTER$
    READ.UNITS = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = UNITS.UNIT$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = UNITS.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
\-----------------------------------------------------------------------------

  FUNCTION READ.UNITS.LOCK PUBLIC
\*********************************  

    INTEGER*2 READ.UNITS.LOCK
    
    READ.UNITS.LOCK = 1
    
    IF END #UNITS.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T2,C30,C1";                                         \ 
         #UNITS.SESS.NUM% AUTOLOCK                                 \
         KEY UNITS.UNIT$;                                          \
             UNITS.UNIT.NAME$,                                     \
             UNITS.BC.LETTER$
    READ.UNITS.LOCK = 0
    EXIT FUNCTION
    
    READ.LOCK.ERROR:
    
       CURRENT.CODE$ = UNITS.UNIT$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = UNITS.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
\-----------------------------------------------------------------------------
  

  FUNCTION WRITE.UNITS PUBLIC
\*****************************

    INTEGER*2 WRITE.UNITS
    
    WRITE.UNITS = 1
        
    IF END #UNITS.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C1,C30,C1";                                        \ 
         #UNITS.SESS.NUM%;                                         \
             UNITS.UNIT$,                                          \
             UNITS.UNIT.NAME$,                                     \
             UNITS.BC.LETTER$
    WRITE.UNITS = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
       CURRENT.CODE$ = UNITS.UNIT$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UNITS.REPORT.NUM%
       
       EXIT FUNCTION     

  END FUNCTION
\-----------------------------------------------------------------------------
  

  FUNCTION WRITE.UNITS.UNLOCK PUBLIC
\************************************

    INTEGER*2 WRITE.UNITS.UNLOCK
    
    WRITE.UNITS.UNLOCK = 1
    
    IF END #UNITS.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C1,C30,C1";                                        \ 
         #UNITS.SESS.NUM% AUTOUNLOCK;                              \
             UNITS.UNIT$,                                          \
             UNITS.UNIT.NAME$,                                     \
             UNITS.BC.LETTER$
    WRITE.UNITS.UNLOCK = 0
    EXIT FUNCTION
    
    WRITE.UNLOCK.ERROR:
    
       CURRENT.CODE$ = UNITS.UNIT$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UNITS.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION


