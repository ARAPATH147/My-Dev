\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   CCITMFUN.BAS  $
\***
\***   $Revision:   1.1  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:\archive\bas\ccitmfun.bav  $
\***   
\***      Rev 1.1   28 Jul 1994 12:12:36   DEVSJPS
\***   Recompile without version letter on included code
\***   
\******************************************************************************
\******************************************************************************
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCITMFUN.BAS
\***
\***	             DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***				   FILE OF ITEMS PER OPEN/CLOSED UODS
\***
\***
\***      VERSION A : Michael J. Kelsall      13th September 1993
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
	 
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$     	   

  %INCLUDE CCITMDEC.J86



  FUNCTION CCITM.SET PUBLIC

     INTEGER*2 CCITM.SET
     CCITM.SET = 1

       CCITM.REPORT.NUM% = 315
       CCITM.RECL%      = 23
       CCITM.FILE.NAME$ = "CCITM"
  
     CCITM.SET = 0

  END FUNCTION



  FUNCTION READ.CCITM PUBLIC

    INTEGER*2 READ.CCITM
    
       READ.CCITM = 1    
       IF END #CCITM.SESS.NUM% THEN READ.ERROR   
       READ FORM "T10,C1,C7,I2,C4"; #CCITM.SESS.NUM% 			\
         KEY CCITM.KEY$;						\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       READ.CCITM = 0
       EXIT FUNCTION     

    READ.ERROR:

       CURRENT.CODE$ = CCITM.UOD.NUM$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
       EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCITM.LOCKED PUBLIC

    INTEGER*2 READ.CCITM.LOCKED
    
       READ.CCITM.LOCKED = 1    
       IF END #CCITM.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T10,C1,C7,I2,C4"; #CCITM.SESS.NUM% AUTOLOCK		\
         KEY CCITM.KEY$;     						\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       READ.CCITM.LOCKED = 0
       EXIT FUNCTION     
	
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCITM.UOD.NUM$
	FILE.OPERATION$ = "R"
	CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
	   
	EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCITM PUBLIC

    INTEGER*2 WRITE.HOLD.CCITM
    
       WRITE.HOLD.CCITM = 1
       IF END #CCITM.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C9,C1,C7,I2,C4"; HOLD #CCITM.SESS.NUM%;          	\     
             CCITM.KEY$,     						\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       WRITE.HOLD.CCITM = 0
       EXIT FUNCTION	     
     
    WRITE.HOLD.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
       CURRENT.CODE$ = CCITM.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCITM PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCITM
    
       WRITE.UNLOCK.CCITM = 1
       IF END #CCITM.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C9,C1,C7,I2,C4"; #CCITM.SESS.NUM% AUTOUNLOCK;   	\        
             CCITM.KEY$,	     					\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       WRITE.UNLOCK.CCITM = 0
       EXIT FUNCTION	     
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
       CURRENT.CODE$ = CCITM.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCITM PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCITM
    
       WRITE.HOLD.UNLOCK.CCITM = 1
       IF END #CCITM.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C9,C1,C7,I2,C4"; HOLD 				\
            #CCITM.SESS.NUM% AUTOUNLOCK;     				\        
             CCITM.KEY$,     						\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       WRITE.HOLD.UNLOCK.CCITM = 0
       EXIT FUNCTION	     
     
    WRITE.HOLD.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
       CURRENT.CODE$ = CCITM.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCITM PUBLIC

    INTEGER*2 WRITE.CCITM
    
       WRITE.CCITM = 1
       IF END #CCITM.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C9,C1,C7,I2,C4"; #CCITM.SESS.NUM%;            	\     
             CCITM.KEY$,     						\
	     CCITM.BOOTS.BAR.CODE.FLAG$,				\
	     CCITM.BOOTS.BAR.CODE$,					\
	     CCITM.QTY%,						\
	     CCITM.FILLER$
       WRITE.CCITM = 0
       EXIT FUNCTION	     
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCITM.REPORT.NUM%
       CURRENT.CODE$ = CCITM.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION

