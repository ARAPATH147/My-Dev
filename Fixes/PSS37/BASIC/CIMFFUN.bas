REM \
\******************************************************************************
\******************************************************************************
\***
\***         %INCLUDE FOR CSR ITEM MOVEMENT FILE FUNCTIONS
\***
\***               REFERENCE    : CIMFFUN.BAS
\***
\***    VERSION A                                  Les Cook  21/08/92
\***
\***    VERSION B.              ROBERT COWEY.                       21 OCT 1993.
\***    Corrected setting of FILE.OPERATION$ within WRITE functions.
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL                       \
     CURRENT.REPORT.NUM%
  
  STRING GLOBAL                          \
     CURRENT.CODE$,                      \
     FILE.OPERATION$
  
  %INCLUDE CIMFDEC.J86                                                 ! BRC
  

  
  FUNCTION CIMF.SET PUBLIC
\**************************

     CIMF.REPORT.NUM%  = 54
     CIMF.RECL%        = 16
     CIMF.FILE.NAME$   = "CIMFI"  

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION READ.CIMF PUBLIC
\***************************

    INTEGER*1 READ.CIMF
    
    READ.CIMF = 1

    IF END #CIMF.SESS.NUM% THEN READ.CIMF.ERROR
    READ FORM "T5,2I4,C3,C1"; #CIMF.SESS.NUM%                     \
         KEY CIMF.BOOTS.CODE$;                                    \
             CIMF.RESTART%,                                       \
             CIMF.NUMITEM%,                                       \
             CIMF.TRANS.DATE$,                                    \
             CIMF.SPACE$
    READ.CIMF = 0
    EXIT FUNCTION
    
    READ.CIMF.ERROR:
    
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
\-----------------------------------------------------------------------------

  FUNCTION READ.CIMF.LOCK PUBLIC
\********************************

    INTEGER*1 READ.CIMF.LOCK
    
    READ.CIMF.LOCK = 1
    
    IF END #CIMF.SESS.NUM% THEN READ.CIMF.LOCK.ERROR
    READ FORM "T5,2I4,C3,C1"; #CIMF.SESS.NUM% AUTOLOCK            \
         KEY CIMF.BOOTS.CODE$;                                    \
             CIMF.RESTART%,                                       \
             CIMF.NUMITEM%,                                       \
             CIMF.TRANS.DATE$,                                    \
             CIMF.SPACE$
    READ.CIMF.LOCK = 0
    EXIT FUNCTION
    
    READ.CIMF.LOCK.ERROR:
    
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.CIMF PUBLIC
\****************************

   INTEGER*1 WRITE.CIMF
   
   WRITE.CIMF = 1
   
   IF END #CIMF.SESS.NUM% THEN WRITE.CIMF.ERROR
   WRITE FORM "C4,2I4,C3,C1"; #CIMF.SESS.NUM%;                    \
             CIMF.BOOTS.CODE$,                                    \
             CIMF.RESTART%,                                       \
             CIMF.NUMITEM%,                                       \
             CIMF.TRANS.DATE$,                                    \
             CIMF.SPACE$
   WRITE.CIMF = 0
   EXIT FUNCTION
   
   WRITE.CIMF.ERROR:
   
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.CIMF.UNLOCK PUBLIC
\***********************************

   INTEGER*1 WRITE.CIMF.UNLOCK
   
   WRITE.CIMF.UNLOCK = 1
   
   IF END #CIMF.SESS.NUM% THEN WRITE.CIMF.UNLOCK.ERROR
   WRITE FORM "C4,2I4,C3,C1"; #CIMF.SESS.NUM%  AUTOUNLOCK;        \
             CIMF.BOOTS.CODE$,                                    \
             CIMF.RESTART%,                                       \
             CIMF.NUMITEM%,                                       \
             CIMF.TRANS.DATE$,                                    \
             CIMF.SPACE$
   WRITE.CIMF.UNLOCK = 0
   EXIT FUNCTION
   
   WRITE.CIMF.UNLOCK.ERROR:
   
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION


\-----------------------------------------------------------------------------

  FUNCTION WRITE.CIMF.HOLD.UNLOCK PUBLIC
\****************************************

   INTEGER*1 WRITE.CIMF.HOLD.UNLOCK
   
   WRITE.CIMF.HOLD.UNLOCK = 1
   
   IF END #CIMF.SESS.NUM% THEN WRITE.CIMF.HOLD.UNLOCK.ERROR
   WRITE FORM "C4,2I4,C3,C1"; HOLD #CIMF.SESS.NUM%  AUTOUNLOCK;   \
             CIMF.BOOTS.CODE$,                                    \
             CIMF.RESTART%,                                       \
             CIMF.NUMITEM%,                                       \
             CIMF.TRANS.DATE$,                                    \
             CIMF.SPACE$
   WRITE.CIMF.HOLD.UNLOCK = 0
   EXIT FUNCTION
   
   WRITE.CIMF.HOLD.UNLOCK.ERROR:
   
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION
  
  END FUNCTION

\_____________________________________________________________________________

  FUNCTION WRITE.CIMF.HOLD PUBLIC
\*********************************

   INTEGER*1 WRITE.CIMF.HOLD
   
   WRITE.CIMF.HOLD = 1
   
   IF END #CIMF.SESS.NUM% THEN WRITE.CIMF.HOLD.ERROR
   WRITE FORM "C4,2I4,C3,C1"; HOLD #CIMF.SESS.NUM% ;              \ 
             CIMF.BOOTS.CODE$,                                    \ 
             CIMF.RESTART%,                                       \ 
             CIMF.NUMITEM%,                                       \ 
             CIMF.TRANS.DATE$,                                    \ 
             CIMF.SPACE$                                          
   WRITE.CIMF.HOLD = 0
   EXIT FUNCTION
   
   WRITE.CIMF.HOLD.ERROR:
   
       CURRENT.CODE$ = CIMF.BOOTS.CODE$
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.REPORT.NUM% = CIMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION                                                    
