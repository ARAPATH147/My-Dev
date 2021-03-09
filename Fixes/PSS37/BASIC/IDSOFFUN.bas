\*******************************************************************************
\*******************************************************************************
\***
\***           OUTSTANDING INITIAL STOCK FILE FILE FUNCTIONS
\***
\***               REFERENCE    : IDSOFFUN.BAS
\***
\***   Version A.       Stephen Kelsey (CTG)            20th October 1992
\***   
\***   Version B.       Steve Perkins                    8th February 1996
\***   Change the IDSOFDEA.J86 to IDSOFDEC.J86 becuase the version letter
\***   has been dropped in PVCS.
\***
\*******************************************************************************
\*******************************************************************************

  INTEGER*2 GLOBAL                                                     \
         CURRENT.REPORT.NUM%
	 
  STRING GLOBAL                                                        \
         CURRENT.CODE$,                                                \ 
	 FILE.OPERATION$
	 
  %INCLUDE IDSOFDEC.J86                                                

\------------------------------------------------------------------------------
  
 FUNCTION IDSOF.SET PUBLIC
\*************************
    
    IDSOF.REPORT.NUM%  = 72  
    IDSOF.RECL%        = 20
    IDSOF.FILE.NAME$  = "IDSOF"
    
END FUNCTION
  
\------------------------------------------------------------------------------
  
 FUNCTION READ.IDSOF PUBLIC
\**************************

    STRING FORMAT$
    INTEGER*2 READ.IDSOF
    
    
    READ.IDSOF = 1 
    FORMAT$ = "T5,C1,I2,C3,C10"  
    IF END #IDSOF.SESS.NUM% THEN READ.IDSOF.ERROR
    
    READ FORM FORMAT$; #IDSOF.SESS.NUM%                                \
         KEY IDSOF.ITEM.CODE$;                                         \
	     IDSOF.BC.LETTER$,                                         \
	     IDSOF.QUANTITY%,                                          \
	     IDSOF.EXP.DELV.DATE$,                                     \
             IDSOF.FILLER$                                            

    READ.IDSOF = 0
    EXIT FUNCTION
       
READ.IDSOF.ERROR:
    
    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION
  
\------------------------------------------------------------------------------
  
 FUNCTION READ.IDSOF.LOCK PUBLIC
\*******************************

    STRING FORMAT$
    INTEGER*2 READ.IDSOF.LOCK
    
    READ.IDSOF.LOCK = 1
    FORMAT$ = "T5,C1,I2,C3,C10"  
    
    IF END #IDSOF.SESS.NUM% THEN READ.IDSOF.LOCK.ERROR

    READ FORM FORMAT$; #IDSOF.SESS.NUM%                                \
         AUTOLOCK                                                      \ 
	 KEY IDSOF.ITEM.CODE$;                                         \
	     IDSOF.BC.LETTER$,                                         \
	     IDSOF.QUANTITY%,                                          \
	     IDSOF.EXP.DELV.DATE$,                                     \
             IDSOF.FILLER$                                            

    READ.IDSOF.LOCK = 0
    EXIT FUNCTION
       
READ.IDSOF.LOCK.ERROR:
    
    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION

\------------------------------------------------------------------------------
  
 FUNCTION WRITE.IDSOF PUBLIC
\***************************

    STRING FORMAT$
    INTEGER*2 WRITE.IDSOF
    
    WRITE.IDSOF = 1
    FORMAT$ = "C4,C1,I2,C3,C10"  
      
    IF END #IDSOF.SESS.NUM% THEN WRITE.IDSOF.ERROR

    WRITE FORM FORMAT$; #IDSOF.SESS.NUM%;                              \
               IDSOF.ITEM.CODE$,                                       \
	       IDSOF.BC.LETTER$,                                       \
	       IDSOF.QUANTITY%,                                        \
               IDSOF.EXP.DELV.DATE$,                                   \
               IDSOF.FILLER$                                            

    WRITE.IDSOF = 0
    EXIT FUNCTION
    
WRITE.IDSOF.ERROR:    

    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION
  
END FUNCTION

\------------------------------------------------------------------------------
  
 FUNCTION WRITE.IDSOF.UNLOCK PUBLIC
\**********************************

    STRING FORMAT$
    INTEGER*2 WRITE.IDSOF.UNLOCK
    
    WRITE.IDSOF.UNLOCK = 1
    FORMAT$ = "C4,C1,I2,C3,C10"  
    
    IF END #IDSOF.SESS.NUM% THEN WRITE.IDSOF.UNLOCK.ERROR
    
    WRITE FORM FORMAT$; #IDSOF.SESS.NUM% AUTOUNLOCK;                   \
               IDSOF.ITEM.CODE$,                                       \
	       IDSOF.BC.LETTER$,                                       \
	       IDSOF.QUANTITY%,                                        \
               IDSOF.EXP.DELV.DATE$,                                   \
               IDSOF.FILLER$                                            

    WRITE.IDSOF.UNLOCK = 0
    EXIT FUNCTION  

WRITE.IDSOF.UNLOCK.ERROR:

    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION

\------------------------------------------------------------------------------
  
 FUNCTION WRITE.IDSOF.HOLD PUBLIC
\********************************

    STRING FORMAT$
    INTEGER*2 WRITE.IDSOF.HOLD
    
    WRITE.IDSOF.HOLD = 1
    FORMAT$ = "C4,C1,I2,C3,C10"  
    
    IF END #IDSOF.SESS.NUM% THEN WRITE.IDSOF.HOLD.ERROR
    
    WRITE FORM FORMAT$; HOLD #IDSOF.SESS.NUM% ;                        \
               IDSOF.ITEM.CODE$,                                       \
	       IDSOF.BC.LETTER$,                                       \
	       IDSOF.QUANTITY%,                                        \
               IDSOF.EXP.DELV.DATE$,                                   \
               IDSOF.FILLER$                                            
	   
    WRITE.IDSOF.HOLD = 0
    EXIT FUNCTION
    
WRITE.IDSOF.HOLD.ERROR:    

    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION
  

END FUNCTION

\------------------------------------------------------------------------------
  
 FUNCTION WRITE.IDSOF.HOLD.UNLOCK PUBLIC
\***************************************

    STRING FORMAT$
    INTEGER*2 WRITE.IDSOF.HOLD.UNLOCK
    
    WRITE.IDSOF.HOLD.UNLOCK = 1
    FORMAT$ = "C4,C1,I2,C3,C10"  

    IF END #IDSOF.SESS.NUM% THEN WRITE.IDSOF.HOLD.UNLOCK.ERROR
    
    WRITE FORM FORMAT$; HOLD #IDSOF.SESS.NUM% AUTOUNLOCK;              \
               IDSOF.ITEM.CODE$,                                       \
	       IDSOF.BC.LETTER$,                                       \
	       IDSOF.QUANTITY%,                                        \
               IDSOF.EXP.DELV.DATE$,                                   \
               IDSOF.FILLER$                                            

    WRITE.IDSOF.HOLD.UNLOCK = 0
    EXIT FUNCTION

WRITE.IDSOF.HOLD.UNLOCK.ERROR:

    CURRENT.CODE$ = IDSOF.ITEM.CODE$
    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = IDSOF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION


