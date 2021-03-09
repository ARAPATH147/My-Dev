
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  DIRECT
\***
\***                   REFERENCE:  CCSTSFUN.BAS
\***
\***                 DESCRIPTION:  CREDIT CLAIMING STOCKTAKING
\***                               SUMMARY FILE
\***
\***
\***      VERSION A : Clive Norris             9th February 1994
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCSTSDEC.J86



  FUNCTION CCSTS.SET PUBLIC

     INTEGER*2 CCSTS.SET
     CCSTS.SET = 1

       CCSTS.REPORT.NUM% = 424
       CCSTS.RECL%       = 32
       CCSTS.FILE.NAME$  = "CCSTS"
  
     CCSTS.SET = 0

  END FUNCTION



  FUNCTION READ.CCSTS PUBLIC

    INTEGER*2 READ.CCSTS
       READ.CCSTS = 1    
         IF END #CCSTS.SESS.NUM% THEN READ.ERROR   
         READ FORM "C32"; #CCSTS.SESS.NUM%, CCSTS.REC.NUM%; CCSTS.RECORD$

           CCSTS.FSI$    = MID$(CCSTS.RECORD$,1,1)
           CCSTS.NAME$   = MID$(CCSTS.RECORD$,2,14)
           CCSTS.CLAIM$  = MID$(CCSTS.RECORD$,16,8)
           CCSTS.FILLER$ = MID$(CCSTS.RECORD$,24,9)
       READ.CCSTS = 0
       EXIT FUNCTION     

    READ.ERROR:
       CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCSTS.LOCKED PUBLIC

    INTEGER*2 READ.CCSTS.LOCKED
    
       READ.CCSTS.LOCKED = 1    
       IF END #CCSTS.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "C32"; #CCSTS.SESS.NUM% AUTOLOCK,                      \
         CCSTS.REC.NUM%; CCSTS.RECORD$

           CCSTS.FSI$    = MID$(CCSTS.RECORD$,1,1)
           CCSTS.NAME$   = MID$(CCSTS.RECORD$,2,14)
           CCSTS.CLAIM$  = MID$(CCSTS.RECORD$,16,8)
           CCSTS.FILLER$ = MID$(CCSTS.RECORD$,24,9)

       READ.CCSTS.LOCKED = 0
       EXIT FUNCTION     
        
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
           
        EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCSTS PUBLIC

    INTEGER*2 WRITE.HOLD.CCSTS
    
       WRITE.HOLD.CCSTS = 1

         IF END #CCSTS.SESS.NUM% THEN WRITE.HOLD.ERROR

         CCSTS.RECORD$ = CCSTS.FSI$ +                                   \
                         CCSTS.NAME$ +                                  \
                         CCSTS.CLAIM$ +                                 \
                         RIGHT$(STRING$(9," ")+CCSTS.FILLER$,9)

         WRITE FORM "C32"; HOLD #CCSTS.SESS.NUM%,                       \
           CCSTS.REC.NUM%; CCSTS.RECORD$
       
       WRITE.HOLD.CCSTS = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCSTS PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCSTS
    
       WRITE.UNLOCK.CCSTS = 1

       IF END #CCSTS.SESS.NUM% THEN WRITE.UNLOCK.ERROR

         CCSTS.RECORD$ = CCSTS.FSI$ +                                   \
                         CCSTS.NAME$ +                                  \
                         CCSTS.CLAIM$ +                                 \
                         RIGHT$(STRING$(9," ")+CCSTS.FILLER$,9)

         WRITE FORM "C32"; #CCSTS.SESS.NUM% AUTOUNLOCK,                 \
            CCSTS.REC.NUM%; CCSTS.RECORD$

       WRITE.UNLOCK.CCSTS = 0
       EXIT FUNCTION         
     
    WRITE.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCSTS PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCSTS
    
       WRITE.HOLD.UNLOCK.CCSTS = 1
       IF END #CCSTS.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR

         CCSTS.RECORD$ = CCSTS.FSI$ +                                   \
                         CCSTS.NAME$ +                                  \
                         CCSTS.CLAIM$ +                                 \
                         RIGHT$(STRING$(9," ")+CCSTS.FILLER$,9)

         WRITE FORM "C32"; HOLD #CCSTS.SESS.NUM% AUTOUNLOCK,            \
           CCSTS.REC.NUM%; CCSTS.RECORD$

       WRITE.HOLD.UNLOCK.CCSTS = 0
       EXIT FUNCTION         
     
    WRITE.HOLD.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCSTS PUBLIC

    INTEGER*2 WRITE.CCSTS
       WRITE.CCSTS = 1

       IF END #CCSTS.SESS.NUM% THEN WRITE.ERROR

         CCSTS.RECORD$ = CCSTS.FSI$ +                                   \
                         CCSTS.NAME$ +                                  \
                         CCSTS.CLAIM$ +                                 \
                         RIGHT$(STRING$(9," ")+CCSTS.FILLER$,9)

         WRITE FORM "C32"; #CCSTS.SESS.NUM%, CCSTS.REC.NUM%; CCSTS.RECORD$

       WRITE.CCSTS = 0
       EXIT FUNCTION         
     
    WRITE.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       CURRENT.CODE$ = CCSTS.FSI$ + CCSTS.NAME$
       EXIT FUNCTION    

  END FUNCTION

