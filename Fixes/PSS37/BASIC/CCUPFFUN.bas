
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCUPFFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               UOD PROCESSED FILE 
\***
\***      VERSION A : Michael J. Kelsall      14th March 1994
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCUPFDEC.J86


  FUNCTION CCUPF.SET PUBLIC

     INTEGER*2 CCUPF.SET
     CCUPF.SET = 1

       CCUPF.REPORT.NUM% = 425
       CCUPF.RECL%       = 8
       CCUPF.FILE.NAME$  = "CCUPF"
  
     CCUPF.SET = 0

  END FUNCTION



  FUNCTION READ.CCUPF PUBLIC

    INTEGER*2 READ.CCUPF
    
       READ.CCUPF = 1    
       IF END #CCUPF.SESS.NUM% THEN READ.ERROR   
       READ FORM "T8,C1"; #CCUPF.SESS.NUM%                              \
         KEY CCUPF.UOD.NUM$;                                            \
             CCUPF.CURRENT.STATUS$
       READ.CCUPF = 0
       EXIT FUNCTION     

    READ.ERROR:

       CURRENT.CODE$ = CCUPF.UOD.NUM$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
       EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCUPF.LOCKED PUBLIC

    INTEGER*2 READ.CCUPF.LOCKED
    
       READ.CCUPF.LOCKED = 1    
       IF END #CCUPF.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "T8,C1"; #CCUPF.SESS.NUM% AUTOLOCK                     \
         KEY CCUPF.UOD.NUM$;                                            \
             CCUPF.CURRENT.STATUS$
       READ.CCUPF.LOCKED = 0
       EXIT FUNCTION     
        
    READ.LOCKED.ERROR:

       CURRENT.CODE$ = CCUPF.UOD.NUM$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
           
       EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCUPF PUBLIC

    INTEGER*2 WRITE.HOLD.CCUPF
    
       WRITE.HOLD.CCUPF = 1
       IF END #CCUPF.SESS.NUM% THEN WRITE.HOLD.ERROR
       WRITE FORM "C7,C1"; HOLD #CCUPF.SESS.NUM%;                       \     
             CCUPF.UOD.NUM$,                                            \
             CCUPF.CURRENT.STATUS$
       WRITE.HOLD.CCUPF = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
       CURRENT.CODE$ = CCUPF.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCUPF PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCUPF
    
       WRITE.UNLOCK.CCUPF = 1
       IF END #CCUPF.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C7,C1"; #CCUPF.SESS.NUM% AUTOUNLOCK;                 \        
             CCUPF.UOD.NUM$,                                            \
             CCUPF.CURRENT.STATUS$
       WRITE.UNLOCK.CCUPF = 0
       EXIT FUNCTION         
     
    WRITE.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
       CURRENT.CODE$ = CCUPF.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCUPF PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCUPF
    
       WRITE.HOLD.UNLOCK.CCUPF = 1
       IF END #CCUPF.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C7,C1"; HOLD #CCUPF.SESS.NUM% AUTOUNLOCK;            \        
             CCUPF.UOD.NUM$,                                            \
             CCUPF.CURRENT.STATUS$
       WRITE.HOLD.UNLOCK.CCUPF = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.UNLOCK.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
       CURRENT.CODE$ = CCUPF.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCUPF PUBLIC

    INTEGER*2 WRITE.CCUPF
    
       WRITE.CCUPF = 1
       IF END #CCUPF.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C7,C1"; #CCUPF.SESS.NUM%;                            \     
             CCUPF.UOD.NUM$,                                            \
             CCUPF.CURRENT.STATUS$
       WRITE.CCUPF = 0
       EXIT FUNCTION         
     
    WRITE.ERROR:
     
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCUPF.REPORT.NUM%
       CURRENT.CODE$ = CCUPF.UOD.NUM$
    
       EXIT FUNCTION    

  END FUNCTION

