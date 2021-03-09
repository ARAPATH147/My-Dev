
REM \
\******************************************************************************
\******************************************************************************
\***
\***                ITEM DATA FILE FUNCTIONS
\***
\***                REFERENCE   : IDFFUN.BAS
\***
\***    VERSION C.              Robert Cowey.                       25 AUG 1993.
\***    Corrected setting of FILE.OPERATION$, CURRENT.CODE$, and 
\***    CURRENT.REPORT.NUM% when IF END # invoked.
\***    Replaced un-used RANK$ with BSNS.CNTR$ and FILLER$.
\***
\***    VERSION D              Nik Sen                 22nd December 1994
\***    WRITE.IDF.HOLD added
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
	 
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$     	   

  %INCLUDE IDFDEC.J86                                                 ! CRC


  FUNCTION IDF.SET PUBLIC
\*************************

     IDF.REPORT.NUM% = 6                                   
     IDF.RECL%      = 60
     IDF.FILE.NAME$ = "IDF"

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION READ.IDF PUBLIC
\**************************  

    INTEGER*2 READ.IDF
    
    READ.IDF = 1    

    IF END #IDF.SESS.NUM% THEN READ.ERROR   
    READ FORM "T5,2C6,C2,C3,C24,C1,C3,C1,C1,2I1,C4,C3";            \   ! CRC
         #IDF.SESS.NUM%                                            \
         KEY IDF.BOOTS.CODE$;                                      \
             IDF.FIRST.BAR.CODE$,                                  \
             IDF.SECOND.BAR.CODE$,                                 \
             IDF.NO.OF.BAR.CODES$,                                 \
             IDF.PRODUCT.GRP$,                                     \
             IDF.STNDRD.DESC$,                                     \
             IDF.STATUS.1$,                                        \
             IDF.INTRO.DATE$,                                      \
             IDF.BSNS.CNTR$,                                       \   ! CRC
             IDF.FILLER$,                                          \   ! CRC
             IDF.BIT.FLAGS.1%,                                     \
             IDF.BIT.FLAGS.2%,                                     \
             IDF.PARENT.CODE$,                                     \
             IDF.DATE.OF.LAST.SALE$

     READ.IDF = 0
     EXIT FUNCTION     


     READ.ERROR:

        FILE.OPERATION$    EQ "R"                                      ! CRC
        CURRENT.REPORT.NUM% EQ IDF.REPORT.NUM%                         ! CRC
        CURRENT.CODE$      EQ UNPACK$(IDF.BOOTS.CODE$)                 ! CRC

        EXIT FUNCTION     

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  

  FUNCTION READ.IDF.LOCK PUBLIC
\*******************************   

    INTEGER*2 READ.IDF.LOCK
    
    READ.IDF.LOCK = 1    

    IF END #IDF.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T5,2C6,C2,C3,C24,C1,C3,C1,C1,2I1,C4,C3";            \   ! CRC
         #IDF.SESS.NUM% AUTOLOCK                                   \
         KEY IDF.BOOTS.CODE$;                                      \
             IDF.FIRST.BAR.CODE$,                                  \
             IDF.SECOND.BAR.CODE$,                                 \
             IDF.NO.OF.BAR.CODES$,                                 \
             IDF.PRODUCT.GRP$,                                     \
             IDF.STNDRD.DESC$,                                     \
             IDF.STATUS.1$,                                        \
             IDF.INTRO.DATE$,                                      \
             IDF.BSNS.CNTR$,                                       \   ! CRC
             IDF.FILLER$,                                          \   ! CRC
             IDF.BIT.FLAGS.1%,                                     \
             IDF.BIT.FLAGS.2%,                                     \
             IDF.PARENT.CODE$,                                     \
             IDF.DATE.OF.LAST.SALE$
	     
    READ.IDF.LOCK = 0
    EXIT FUNCTION
    
    
    READ.LOCK.ERROR:    	     

       CURRENT.CODE$ = UNPACK$(IDF.BOOTS.CODE$)
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = IDF.REPORT.NUM%              
    
       EXIT FUNCTION    

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L  


  FUNCTION WRITE.IDF PUBLIC
\**************************  

    INTEGER*2 WRITE.IDF
    
    WRITE.IDF = 1

    IF END #IDF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C4,2C6,C2,C3,C24,C1,C3,C1,C1,2I1,C4,C3";           \   ! CRC
             #IDF.SESS.NUM%;                                       \
             IDF.BOOTS.CODE$,                                      \
             IDF.FIRST.BAR.CODE$,                                  \
             IDF.SECOND.BAR.CODE$,                                 \
             IDF.NO.OF.BAR.CODES$,                                 \
             IDF.PRODUCT.GRP$,                                     \
             IDF.STNDRD.DESC$,                                     \
             IDF.STATUS.1$,                                        \
             IDF.INTRO.DATE$,                                      \
             IDF.BSNS.CNTR$,                                       \   ! CRC
             IDF.FILLER$,                                          \   ! CRC
             IDF.BIT.FLAGS.1%,                                     \
             IDF.BIT.FLAGS.2%,                                     \
             IDF.PARENT.CODE$,                                     \
             IDF.DATE.OF.LAST.SALE$

    WRITE.IDF = 0
    EXIT FUNCTION


    WRITE.ERROR:

       FILE.OPERATION$ = "W"                                           ! CRC
       CURRENT.REPORT.NUM% = IDF.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IDF.BOOTS.CODE$)              
    
       EXIT FUNCTION    

  END FUNCTION
\------------------------------------------------------------------------------  
 
REM EJECT^L  


  FUNCTION WRITE.IDF.HOLD PUBLIC
\**************************  

    INTEGER*2 WRITE.IDF.HOLD
    
    WRITE.IDF.HOLD = 1

    IF END #IDF.SESS.NUM% THEN WRITE.HOLD.ERROR
    WRITE FORM "C4,2C6,C2,C3,C24,C1,C3,C1,C1,2I1,C4,C3"; HOLD      \   ! DNS
             #IDF.SESS.NUM%;                                       \
             IDF.BOOTS.CODE$,                                      \
             IDF.FIRST.BAR.CODE$,                                  \
             IDF.SECOND.BAR.CODE$,                                 \
             IDF.NO.OF.BAR.CODES$,                                 \
             IDF.PRODUCT.GRP$,                                     \
             IDF.STNDRD.DESC$,                                     \
             IDF.STATUS.1$,                                        \
             IDF.INTRO.DATE$,                                      \
             IDF.BSNS.CNTR$,                                       \   
             IDF.FILLER$,                                          \   
             IDF.BIT.FLAGS.1%,                                     \
             IDF.BIT.FLAGS.2%,                                     \
             IDF.PARENT.CODE$,                                     \
             IDF.DATE.OF.LAST.SALE$

    WRITE.IDF.HOLD = 0
    EXIT FUNCTION


    WRITE.HOLD.ERROR:

       FILE.OPERATION$ = "W"                                           
       CURRENT.REPORT.NUM% = IDF.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IDF.BOOTS.CODE$)              
    
       EXIT FUNCTION    

  END FUNCTION
\------------------------------------------------------------------------------  
 
  FUNCTION WRITE.IDF.UNLOCK PUBLIC
\**********************************  

    INTEGER*2 WRITE.IDF.UNLOCK
    
    WRITE.IDF.UNLOCK = 1

    IF END #IDF.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C4,2C6,C2,C3,C24,C1,C3,C1,C1,2I1,C4,C3";           \   ! CRC
             #IDF.SESS.NUM% AUTOUNLOCK;                            \
             IDF.BOOTS.CODE$,                                      \
             IDF.FIRST.BAR.CODE$,                                  \
             IDF.SECOND.BAR.CODE$,                                 \
             IDF.NO.OF.BAR.CODES$,                                 \
             IDF.PRODUCT.GRP$,                                     \
             IDF.STNDRD.DESC$,                                     \
             IDF.STATUS.1$,                                        \
             IDF.INTRO.DATE$,                                      \
             IDF.BSNS.CNTR$,                                       \   ! CRC
             IDF.FILLER$,                                          \   ! CRC
             IDF.BIT.FLAGS.1%,                                     \
             IDF.BIT.FLAGS.2%,                                     \
             IDF.PARENT.CODE$,                                     \
             IDF.DATE.OF.LAST.SALE$

    WRITE.IDF.UNLOCK = 0
    EXIT FUNCTION


    WRITE.UNLOCK.ERROR:

       FILE.OPERATION$ = "W"                                           ! CRC
       CURRENT.REPORT.NUM% = IDF.REPORT.NUM%
       CURRENT.CODE$ = UNPACK$(IDF.BOOTS.CODE$)              
    
       EXIT FUNCTION    

  END FUNCTION
\------------------------------------------------------------------------------  
