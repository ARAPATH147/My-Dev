
\******************************************************************************
\***
\***           PILST - LIST OF LISTS FILE - FILE FUNCTIONS
\***
\***           REFERENCE: PILSTFUN.BAS
\***
\***           9th October 1992
\***
\******************************************************************************

   INTEGER*2 GLOBAL                     \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                        \
      CURRENT.CODE$,                    \
      FILE.OPERATION$
      
   %INCLUDE PILSTDEC.J86
   
   
  FUNCTION PILST.SET PUBLIC
\***************************

     PILST.FILE.NAME$  = "PILST"
     PILST.REPORT.NUM% = 120
     PILST.RECL%       = 40   
  
  END FUNCTION
  
\-----------------------------------------------------------------------------  

  FUNCTION READ.PILST PUBLIC
\****************************

      INTEGER*2 READ.PILST
      
      READ.PILST = 1   

      IF PILST.LIST.NUMBER$ <> "0000" THEN BEGIN   
         REM ** Detail record **
         IF END #PILST.SESS.NUM% THEN READ.ERROR
         READ FORM "T5,C12,2C1,2C3,2I1,C3,C1,C3,C1,C6";                 \
           # PILST.SESS.NUM%                                            \
           KEY PILST.LIST.NUMBER$;                                      \
               PILST.LIST.NAME$,                                        \
               PILST.BC.LETTER$,                                        \
               PILST.LIST.TYPE$,                                        \
               PILST.COUNT.BY.DATE$,                                    \
               PILST.PRODUCT.GROUP$,                                    \
               PILST.ITEMS.IN.LIST%,                                    \
               PILST.TO.BE.COUNTED%,                                    \
               PILST.COUNT.DATE$,                                       \
               PILST.LIST.STATUS$,                                      \
               PILST.RECOUNT.DATE$,                                     \
               PILST.RECOUNT.ALLOWED$,                                  \
               PILST.DET.FILLER$                                        !
      ENDIF
      
      IF PILST.LIST.NUMBER$ = "0000" THEN BEGIN
         REM ** 'header' record **
         IF END #PILST.SESS.NUM% THEN READ.ERROR
         READ FORM "T5,C4,C3,C1,C4,C24";                                \ 
           # PILST.SESS.NUM%                                            \
           KEY PILST.LIST.NUMBER$;                                      \
               PILST.SPARE.LIST.NUMBER$,                                \
               PILST.CPM.RUN.DATE$,                                     \
               PILST.PIPLN.RUN.OK$,                                     \
               PILST.HIGHEST.LIST.NO$,                                  \ 
               PILST.HDR.FILLER$ 
      ENDIF
      READ.PILST = 0
      EXIT FUNCTION
      
      READ.ERROR:
         
         CURRENT.CODE$ = PILST.LIST.NUMBER$
         FILE.OPERATION$ = "R"
         CURRENT.REPORT.NUM% = PILST.REPORT.NUM%
         
         EXIT FUNCTION
      
   END FUNCTION
\-----------------------------------------------------------------------------
   

  FUNCTION READ.PILST.LOCK PUBLIC
\*********************************

      INTEGER*2 READ.PILST.LOCK
      
      READ.PILST.LOCK = 1
         
      IF PILST.LIST.NUMBER$ <> "0000" THEN BEGIN   
         REM ** Detail record **
         IF END #PILST.SESS.NUM% THEN READ.LOCK.ERROR
         READ FORM "T5,C12,2C1,2C3,2I1,C3,C1,C3,C1,C6";                 \
           # PILST.SESS.NUM% AUTOLOCK                                   \
           KEY PILST.LIST.NUMBER$;                                      \
               PILST.LIST.NAME$,                                        \
               PILST.BC.LETTER$,                                        \
               PILST.LIST.TYPE$,                                        \
               PILST.COUNT.BY.DATE$,                                    \
               PILST.PRODUCT.GROUP$,                                    \
               PILST.ITEMS.IN.LIST%,                                    \
               PILST.TO.BE.COUNTED%,                                    \
               PILST.COUNT.DATE$,                                       \
               PILST.LIST.STATUS$,                                      \
               PILST.RECOUNT.DATE$,                                     \
               PILST.RECOUNT.ALLOWED$,                                  \
               PILST.DET.FILLER$      
      ENDIF
      
      IF PILST.LIST.NUMBER$ = "0000" THEN BEGIN
         REM ** 'header' record **
         IF END #PILST.SESS.NUM% THEN READ.LOCK.ERROR
         READ FORM "T5,C4,C3,C1,C4,C24";                                \ 
           # PILST.SESS.NUM%                                            \
           KEY PILST.LIST.NUMBER$;                                      \
               PILST.SPARE.LIST.NUMBER$,                                \
               PILST.CPM.RUN.DATE$,                                     \
               PILST.PIPLN.RUN.OK$,                                     \
               PILST.HIGHEST.LIST.NO$,                                  \ 
               PILST.HDR.FILLER$                       
      ENDIF
      READ.PILST.LOCK = 0
      EXIT FUNCTION
      
      READ.LOCK.ERROR:
      
         CURRENT.CODE$ = PILST.LIST.NUMBER$
         FILE.OPERATION$ = "R"
         CURRENT.REPORT.NUM% = PILST.REPORT.NUM%
         
         EXIT FUNCTION
   
   END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.PILST PUBLIC
\*****************************

      INTEGER*2 WRITE.PILST
      
      WRITE.PILST = 1  

      IF PILST.LIST.NUMBER$ <> "0000" THEN BEGIN   
         REM ** Detail record **
         IF END #PILST.SESS.NUM% THEN WRITE.ERROR
         WRITE FORM "C4,C12,2C1,2C3,2I1,C3,C1,C3,C1,C6";                \
           # PILST.SESS.NUM%;                                           \
               PILST.LIST.NUMBER$,                                      \
               PILST.LIST.NAME$,                                        \
               PILST.BC.LETTER$,                                        \
               PILST.LIST.TYPE$,                                        \
               PILST.COUNT.BY.DATE$,                                    \
               PILST.PRODUCT.GROUP$,                                    \
               PILST.ITEMS.IN.LIST%,                                    \
               PILST.TO.BE.COUNTED%,                                    \
               PILST.COUNT.DATE$,                                       \
               PILST.LIST.STATUS$,                                      \
               PILST.RECOUNT.DATE$,                                     \
               PILST.RECOUNT.ALLOWED$,                                  \
               PILST.DET.FILLER$
      ENDIF
      
      IF PILST.LIST.NUMBER$ = "0000" THEN BEGIN
         REM ** 'header' record **
         IF END #PILST.SESS.NUM% THEN WRITE.ERROR
         WRITE FORM "C4,C4,C3,C1,C4,C24";                               \ 
           # PILST.SESS.NUM%;                                           \
               PILST.LIST.NUMBER$,                                      \
               PILST.SPARE.LIST.NUMBER$,                                \
               PILST.CPM.RUN.DATE$,                                     \
               PILST.PIPLN.RUN.OK$,                                     \
               PILST.HIGHEST.LIST.NO$,                                  \ 
               PILST.HDR.FILLER$  
      ENDIF
      WRITE.PILST = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = PILST.LIST.NUMBER$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PILST.REPORT.NUM%
         
         EXIT FUNCTION
   
   END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.PILST.HOLD PUBLIC
\**********************************

      INTEGER*2 WRITE.PILST.HOLD
      
      WRITE.PILST.HOLD = 1
      
      IF PILST.LIST.NUMBER$ <> "0000" THEN BEGIN   
         REM ** Detail record **
         IF END #PILST.SESS.NUM% THEN WRITE.HOLD.ERROR
         WRITE FORM "C4,C12,2C1,2C3,2I1,C3,C1,C3,C1,C6";                \
           HOLD # PILST.SESS.NUM%;                                      \
               PILST.LIST.NUMBER$,                                      \
               PILST.LIST.NAME$,                                        \
               PILST.BC.LETTER$,                                        \
               PILST.LIST.TYPE$,                                        \
               PILST.COUNT.BY.DATE$,                                    \
               PILST.PRODUCT.GROUP$,                                    \
               PILST.ITEMS.IN.LIST%,                                    \
               PILST.TO.BE.COUNTED%,                                    \
               PILST.COUNT.DATE$,                                       \
               PILST.LIST.STATUS$,                                      \
               PILST.RECOUNT.DATE$,                                     \
               PILST.RECOUNT.ALLOWED$,                                  \
               PILST.DET.FILLER$
      ENDIF

      
      IF PILST.LIST.NUMBER$ = "0000" THEN BEGIN
         REM ** 'header' record **
         IF END #PILST.SESS.NUM% THEN WRITE.HOLD.ERROR
         WRITE FORM "C4,C4,C3,C1,C4,C24";                               \ 
           # PILST.SESS.NUM%;                                           \
               PILST.LIST.NUMBER$,                                      \
               PILST.SPARE.LIST.NUMBER$,                                \
               PILST.CPM.RUN.DATE$,                                     \
               PILST.PIPLN.RUN.OK$,                                     \
               PILST.HIGHEST.LIST.NO$,                                  \ 
               PILST.HDR.FILLER$
      ENDIF
      WRITE.PILST.HOLD = 0
      EXIT FUNCTION
      
      WRITE.HOLD.ERROR:
      
         CURRENT.CODE$ = PILST.LIST.NUMBER$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PILST.REPORT.NUM%
         
         EXIT FUNCTION      
   
   END FUNCTION
\-----------------------------------------------------------------------------
   

  FUNCTION WRITE.PILST.UNLOCK PUBLIC
\************************************

      INTEGER*2 WRITE.PILST.UNLOCK
      
      WRITE.PILST.UNLOCK = 1
      
      IF PILST.LIST.NUMBER$ <> "0000" THEN BEGIN   
         REM ** Detail record **
         IF END #PILST.SESS.NUM% THEN WRITE.UNLOCK.ERROR
         WRITE FORM "C4,C12,2C1,2C3,2I1,C3,C1,C3,C1,C6";                \
           # PILST.SESS.NUM% AUTOUNLOCK;                                \
               PILST.LIST.NUMBER$,                                      \
               PILST.LIST.NAME$,                                        \
               PILST.BC.LETTER$,                                        \
               PILST.LIST.TYPE$,                                        \
               PILST.COUNT.BY.DATE$,                                    \
               PILST.PRODUCT.GROUP$,                                    \
               PILST.ITEMS.IN.LIST%,                                    \
               PILST.TO.BE.COUNTED%,                                    \
               PILST.COUNT.DATE$,                                       \
               PILST.LIST.STATUS$,                                      \
               PILST.RECOUNT.DATE$,                                     \
               PILST.RECOUNT.ALLOWED$,                                  \
               PILST.DET.FILLER$
      ENDIF
      
      IF PILST.LIST.NUMBER$ = "0000" THEN BEGIN
         REM ** 'header' record **
         IF END #PILST.SESS.NUM% THEN WRITE.UNLOCK.ERROR
         WRITE FORM "C4,C4,C3,C1,C4,C24";                               \ 
           # PILST.SESS.NUM%;                                           \
               PILST.LIST.NUMBER$,                                      \
               PILST.SPARE.LIST.NUMBER$,                                \
               PILST.CPM.RUN.DATE$,                                     \
               PILST.PIPLN.RUN.OK$,                                     \
               PILST.HIGHEST.LIST.NO$,                                  \ 
               PILST.HDR.FILLER$
      ENDIF
      WRITE.PILST.UNLOCK = 0
      EXIT FUNCTION
      
      WRITE.UNLOCK.ERROR:
      
         CURRENT.CODE$ = PILST.LIST.NUMBER$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PILST.REPORT.NUM%
         
         EXIT FUNCTION      
   
   END FUNCTION
