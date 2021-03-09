\*****************************************************************************
\*****************************************************************************
\***
\***          UOD MASTER FILE FUNCTIONS
\***
\***          REFERENCE   :   UODMSFUN.BAS
\***
\***          VERSION A   :   Les Cook     23rd December 1992
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE UODMSDEC.J86


  FUNCTION UODMS.SET PUBLIC
\***************************

     UODMS.REPORT.NUM% = 261                                   
     UODMS.RECL%      = 21
     UODMS.FILE.NAME$ = "UODMS"

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION READ.UODMS PUBLIC
\****************************  

    INTEGER*2 READ.UODMS
    
    READ.UODMS = 1    

    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! header record
       IF END #UODMS.SESS.NUM% THEN READ.ERROR   
       READ FORM "T6,I2,C3,C11";                                        \ 
            #UODMS.SESS.NUM%                                            \
            KEY UODMS.KEY$;                                             \
                UODMS.NUM.DATED.RETRO.RECS%,                            \
                UODMS.LAST.RUN.DATE$,                                   \
                UODMS.HEADER.FILLER$
       READ.UODMS = 0
       EXIT FUNCTION     
    ENDIF ELSE BEGIN                                    ! detail record
       IF END #UODMS.SESS.NUM% THEN READ.ERROR
       READ FORM "T6,C1,C3,C3,C1,C1,I2,C5"; # UODMS.SESS.NUM%           \
           KEY UODMS.KEY$;                                              \
               UODMS.QA.FLAG$,                                          \
               UODMS.EXPECTED.DELIV.DATE$,                              \
               UODMS.ACTUAL.DELIV.DATE$,                                \
               UODMS.UOD.TYPE$,                                         \
               UODMS.STATUS$,                                           \
               UODMS.NUM.ITEMS%,                                        \
               UODMS.DETAIL.FILLER$
       READ.UODMS = 0
       EXIT FUNCTION
    ENDIF
        
    READ.ERROR:

        CURRENT.CODE$ = UODMS.KEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION
  
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION READ.UODMS.LOCKED PUBLIC
\***********************************  

    INTEGER*2 READ.UODMS.LOCKED
    
    READ.UODMS.LOCKED = 1    

    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! header record
       IF END #UODMS.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T6,I2,C3,C11";                                        \ 
            #UODMS.SESS.NUM% AUTOLOCK                                   \
            KEY UODMS.KEY$;                                             \
                UODMS.NUM.DATED.RETRO.RECS%,                            \
                UODMS.LAST.RUN.DATE$,                                   \
                UODMS.HEADER.FILLER$
       READ.UODMS.LOCKED = 0
       EXIT FUNCTION     
    ENDIF ELSE BEGIN                                    ! detail record
       IF END #UODMS.SESS.NUM% THEN READ.LOCKED.ERROR
       READ FORM "T6,C1,C3,C3,C1,C1,I2,C5"; # UODMS.SESS.NUM% AUTOLOCK  \
           KEY UODMS.KEY$;                                              \
               UODMS.QA.FLAG$,                                          \
               UODMS.EXPECTED.DELIV.DATE$,                              \
               UODMS.ACTUAL.DELIV.DATE$,                                \
               UODMS.UOD.TYPE$,                                         \
               UODMS.STATUS$,                                           \
               UODMS.NUM.ITEMS%,                                        \
               UODMS.DETAIL.FILLER$
       READ.UODMS.LOCKED = 0
       EXIT FUNCTION
    ENDIF
        
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = UODMS.KEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION  
\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.HOLD.UODMS PUBLIC
\**********************************

    INTEGER*2 WRITE.HOLD.UODMS
    
    WRITE.HOLD.UODMS = 1
      
    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! Header Record
       IF END #UODMS.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C5,I2,C3,C11"; HOLD #UODMS.SESS.NUM%;                \
             UODMS.KEY$,                                                \
             UODMS.NUM.DATED.RETRO.RECS%,                               \
             UODMS.LAST.RUN.DATE$,                                      \
             UODMS.HEADER.FILLER$   
       WRITE.HOLD.UODMS = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #UODMS.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C5,C1,2C3,2C1,I2,C5"; HOLD #UODMS.SESS.NUM%;         \
             UODMS.KEY$,                                                \
             UODMS.QA.FLAG$,                                            \
             UODMS.EXPECTED.DELIV.DATE$,                                \
             UODMS.ACTUAL.DELIV.DATE$,                                  \
             UODMS.UOD.TYPE$,                                           \
             UODMS.STATUS$,                                             \
             UODMS.NUM.ITEMS%,                                          \
             UODMS.DETAIL.FILLER$            
       WRITE.HOLD.UODMS = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.HOLD.ERROR:
     
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
       CURRENT.CODE$ = UODMS.KEY$
    
       EXIT FUNCTION    

  END FUNCTION

\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.UNLOCK.UODMS PUBLIC
\************************************

    INTEGER*2 WRITE.UNLOCK.UODMS
    
    WRITE.UNLOCK.UODMS = 1
      
    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! Header Record
       IF END #UODMS.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C5,I2,C3,C11"; #UODMS.SESS.NUM% AUTOUNLOCK;          \
             UODMS.KEY$,                                                \
             UODMS.NUM.DATED.RETRO.RECS%,                               \
             UODMS.LAST.RUN.DATE$,                                      \
             UODMS.HEADER.FILLER$   
       WRITE.UNLOCK.UODMS = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #UODMS.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C5,C1,2C3,2C1,I2,C5"; #UODMS.SESS.NUM% AUTOUNLOCK;   \
             UODMS.KEY$,                                                \
             UODMS.QA.FLAG$,                                            \
             UODMS.EXPECTED.DELIV.DATE$,                                \
             UODMS.ACTUAL.DELIV.DATE$,                                  \
             UODMS.UOD.TYPE$,                                           \
             UODMS.STATUS$,                                             \
             UODMS.NUM.ITEMS%,                                          \
             UODMS.DETAIL.FILLER$            
       WRITE.UNLOCK.UODMS = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
       CURRENT.CODE$ = UODMS.KEY$
    
       EXIT FUNCTION    

  END FUNCTION

\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.HOLD.UNLOCK.UODMS PUBLIC
\*****************************************

    INTEGER*2 WRITE.HOLD.UNLOCK.UODMS
    
    WRITE.HOLD.UNLOCK.UODMS = 1
      
    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! Header Record
       IF END #UODMS.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C5,I2,C3,C11"; HOLD #UODMS.SESS.NUM% AUTOUNLOCK;     \
             UODMS.KEY$,                                                \
             UODMS.NUM.DATED.RETRO.RECS%,                               \
             UODMS.LAST.RUN.DATE$,                                      \
             UODMS.HEADER.FILLER$   
       WRITE.HOLD.UNLOCK.UODMS = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #UODMS.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C5,C1,2C3,2C1,I2,C5"; HOLD #UODMS.SESS.NUM%          \ 
          AUTOUNLOCK;                                                   \
             UODMS.KEY$,                                                \
             UODMS.QA.FLAG$,                                            \
             UODMS.EXPECTED.DELIV.DATE$,                                \
             UODMS.ACTUAL.DELIV.DATE$,                                  \
             UODMS.UOD.TYPE$,                                           \
             UODMS.STATUS$,                                             \
             UODMS.NUM.ITEMS%,                                          \
             UODMS.DETAIL.FILLER$            
       WRITE.HOLD.UNLOCK.UODMS = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.HOLD.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
       CURRENT.CODE$ = UODMS.KEY$
    
       EXIT FUNCTION    

  END FUNCTION


\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.UODMS PUBLIC
\**********************************

    INTEGER*2 WRITE.UODMS
    
    WRITE.UODMS = 1
      
    IF UODMS.KEY$ = PACK$(STRING$(5,"??")) THEN BEGIN   ! Header Record
       IF END #UODMS.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C5,I2,C3,C11"; #UODMS.SESS.NUM%;                \
             UODMS.KEY$,                                                \
             UODMS.NUM.DATED.RETRO.RECS%,                               \
             UODMS.LAST.RUN.DATE$,                                      \
             UODMS.HEADER.FILLER$   
       WRITE.UODMS = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #UODMS.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C5,C1,2C3,2C1,I2,C5"; #UODMS.SESS.NUM%;              \
             UODMS.KEY$,                                                \
             UODMS.QA.FLAG$,                                            \
             UODMS.EXPECTED.DELIV.DATE$,                                \
             UODMS.ACTUAL.DELIV.DATE$,                                  \
             UODMS.UOD.TYPE$,                                           \
             UODMS.STATUS$,                                             \
             UODMS.NUM.ITEMS%,                                          \
             UODMS.DETAIL.FILLER$            
       WRITE.UODMS = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODMS.REPORT.NUM%
       CURRENT.CODE$ = UODMS.KEY$
    
       EXIT FUNCTION    

  END FUNCTION

