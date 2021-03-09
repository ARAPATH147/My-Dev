
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  DIRECT
\***
\***                   REFERENCE:  CCTRLFUN.BAS
\***
\***	             DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***				   CREDIT CLAIMS CONTROL FILE
\***
\***
\***      VERSION A : Michael J. Kelsall      14th September 1993
\***
\***      VERSION B : Michael J. Kelsall      03rd March 1994
\***      Problem with stored negative numbers not passing through the
\***      UNPACK$ after being stored using PACK$.
\***
\***      Version C    Nik Sen                 14th February 1995
\***      Removed version letters from included code (not commented).
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
	 
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$     	   

  %INCLUDE CCTRLDEC.J86



  FUNCTION CCTRL.SET PUBLIC

     INTEGER*2 CCTRL.SET
     CCTRL.SET = 1

       CCTRL.REPORT.NUM% = 320
       CCTRL.RECL%       = 80
       CCTRL.FILE.NAME$  = "CCTRL"
  
     CCTRL.SET = 0

  END FUNCTION



  FUNCTION READ.CCTRL PUBLIC

    INTEGER*2 READ.CCTRL
       READ.CCTRL = 1    
         IF END #CCTRL.SESS.NUM% THEN READ.ERROR   
         READ FORM "C4,C3,C3,I4,C66"; #CCTRL.SESS.NUM%, CCTRL.REC.NUM%; \BMJK
            CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
            CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
            CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
            CCTRL.STAFF.SALES%,						\BMJK
            CCTRL.FILLER$ 						!BMJK
       READ.CCTRL = 0
       EXIT FUNCTION     

    READ.ERROR:
       CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       EXIT FUNCTION

  END FUNCTION
  


  FUNCTION READ.CCTRL.LOCKED PUBLIC

    INTEGER*2 READ.CCTRL.LOCKED
    
       READ.CCTRL.LOCKED = 1    
       IF END #CCTRL.SESS.NUM% THEN READ.LOCKED.ERROR   
       READ FORM "C4,C3,C3,I4,C66"; #CCTRL.SESS.NUM% AUTOLOCK, 		\BMJK
         CCTRL.REC.NUM%; 						\BMJK
           CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
           CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
           CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
           CCTRL.STAFF.SALES%,						\BMJK
           CCTRL.FILLER$ 						!BMJK
       READ.CCTRL.LOCKED = 0
       EXIT FUNCTION     
	
    READ.LOCKED.ERROR:

        CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
	FILE.OPERATION$ = "R"
	CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
	   
	EXIT FUNCTION

  END FUNCTION  



  FUNCTION WRITE.HOLD.CCTRL PUBLIC

    INTEGER*2 WRITE.HOLD.CCTRL
    
       WRITE.HOLD.CCTRL = 1

         IF END #CCTRL.SESS.NUM% THEN WRITE.HOLD.ERROR
         WRITE FORM "C4,C3,C3,I4,C66"; HOLD #CCTRL.SESS.NUM%, 		\BMJK
	   CCTRL.REC.NUM%; 						\BMJK
            CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
            CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
            CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
            CCTRL.STAFF.SALES%,						\BMJK
            CCTRL.FILLER$ 						!BMJK
       WRITE.HOLD.CCTRL = 0
       EXIT FUNCTION	     
     
    WRITE.HOLD.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.UNLOCK.CCTRL PUBLIC

    INTEGER*2 WRITE.UNLOCK.CCTRL
    
       WRITE.UNLOCK.CCTRL = 1

       IF END #CCTRL.SESS.NUM% THEN WRITE.UNLOCK.ERROR
       WRITE FORM "C4,C3,C3,I4,C66"; #CCTRL.SESS.NUM% AUTOUNLOCK,	\BMJK
         CCTRL.REC.NUM%; 						\BMJK
            CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
            CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
            CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
            CCTRL.STAFF.SALES%,						\BMJK
            CCTRL.FILLER$ 						!BMJK

       WRITE.UNLOCK.CCTRL = 0
       EXIT FUNCTION	     
     
    WRITE.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.HOLD.UNLOCK.CCTRL PUBLIC

    INTEGER*2 WRITE.HOLD.UNLOCK.CCTRL
    
       WRITE.HOLD.UNLOCK.CCTRL = 1
       IF END #CCTRL.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
       WRITE FORM "C4,C3,C3,I4,C66"; HOLD #CCTRL.SESS.NUM% AUTOUNLOCK,	\BMJK
         CCTRL.REC.NUM%; 						\BMJK
           CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
           CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
           CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
           CCTRL.STAFF.SALES%,						\BMJK
           CCTRL.FILLER$ 						!BMJK

       WRITE.HOLD.UNLOCK.CCTRL = 0
       EXIT FUNCTION	     
     
    WRITE.HOLD.UNLOCK.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION



  FUNCTION WRITE.CCTRL PUBLIC

    INTEGER*2 WRITE.CCTRL
       WRITE.CCTRL = 1

       IF END #CCTRL.SESS.NUM% THEN WRITE.ERROR
       WRITE FORM "C4,C3,C3,I4,C66";#CCTRL.SESS.NUM%,CCTRL.REC.NUM%;    \BMJK
          CCTRL.CREDIT.CLAIM.NUM$,					\BMJK
          CCTRL.CREDIT.RPT.RUN.DATE$,					\BMJK
          CCTRL.UOD.RPT.RUN.DATE$,					\BMJK
          CCTRL.STAFF.SALES%,						\BMJK
          CCTRL.FILLER$ 						!BMJK

       WRITE.CCTRL = 0
       EXIT FUNCTION	     
     
    WRITE.ERROR:
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       CURRENT.CODE$ = CCTRL.CREDIT.CLAIM.NUM$
       EXIT FUNCTION    

  END FUNCTION

