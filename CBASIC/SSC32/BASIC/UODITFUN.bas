\*****************************************************************************
\*****************************************************************************
\***
\***           UOD ITEM FILE FUNCTIONS
\***
\***           REFERENCE   :  UODITFUN.BAS
\***
\***           VERSION A   :  Les Cook       23rd December 1992
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE UODITDEC.J86


  FUNCTION UODIT.SET PUBLIC
\***************************

     UODIT.REPORT.NUM% = 262
     UODIT.RECL%      = 27
     UODIT.FILE.NAME$ = "UODIT"

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION READ.UODIT PUBLIC
\****************************  

    INTEGER*2 READ.UODIT
    
    READ.UODIT = 1    

    IF LEFT$(UODIT.KEY$,5) = PACK$(STRING$(5,"??")) THEN BEGIN  ! header record
       IF END #UODIT.SESS.NUM% THEN READ.ERROR   
       READ FORM "T8,C3,C17";                                           \ 
            #UODIT.SESS.NUM%                                            \
            KEY UODIT.KEY$;                                             \
                UODIT.RETRO.DATE$,                                      \
                UODIT.RETRO.FILLER$
         
       READ.UODIT = 0
       EXIT FUNCTION     
    ENDIF ELSE BEGIN                                    ! detail record
       IF END #UODIT.SESS.NUM% THEN READ.ERROR
       READ FORM "T8,C1,C4,I2,4C1,C2,C1,C6"; # UODIT.SESS.NUM%          \
           KEY UODIT.KEY$;                                              \
               UODIT.REC.TYPE$,                                         \
               UODIT.ITEM.CODE$,                                        \
               UODIT.QUANTITY%,                                         \
               UODIT.FSI$,                                              \
               UODIT.FOLIO.YEAR$,                                       \
               UODIT.FOLIO.MONTH$,                                      \
               UODIT.STORE.SUFFIX$,                                     \
               UODIT.FOLIO.NUM$,                                        \
               UODIT.CSR.MARKER$,                                       \
               UODIT.FILLER$
       READ.UODIT = 0
       EXIT FUNCTION
    ENDIF
        
    READ.ERROR:

        CURRENT.CODE$ = UODIT.KEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = UODIT.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.UODIT PUBLIC
\*****************************

    INTEGER*2 WRITE.UODIT
    
    WRITE.UODIT = 1
      
    IF LEFT$(UODIT.KEY$,5) = PACK$(STRING$(5,"??")) THEN BEGIN  ! Retro Record
       IF END #UODIT.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C7,C3,C17"; #UODIT.SESS.NUM%;                        \
             UODIT.KEY$,                                                \
             UODIT.RETRO.DATE$,                                         \
             UODIT.RETRO.FILLER$             

       WRITE.UODIT = 0
       EXIT FUNCTION         
    ENDIF ELSE BEGIN                                    ! Detail Record
       IF END #UODIT.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C7,C1,C4,I2,4C1,C2,C1,C6"; #UODIT.SESS.NUM%;         \
             UODIT.KEY$,                                                \
             UODIT.REC.TYPE$,                                           \
             UODIT.ITEM.CODE$,                                          \
             UODIT.QUANTITY%,                                           \
             UODIT.FSI$,                                                \
             UODIT.FOLIO.YEAR$,                                         \
             UODIT.FOLIO.MONTH$,                                        \
             UODIT.STORE.SUFFIX$,                                       \
             UODIT.FOLIO.NUM$,                                          \
             UODIT.CSR.MARKER$,                                         \
             UODIT.FILLER$
       WRITE.UODIT = 0
       EXIT FUNCTION
    ENDIF
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = UODIT.REPORT.NUM%
       CURRENT.CODE$ = UODIT.KEY$
    
       EXIT FUNCTION    

  END FUNCTION

