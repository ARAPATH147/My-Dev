
\******************************************************************************
\***
\***           CHKBF - PRICE CHECK BUFFER FILE FUNCTIONS
\***
\***                    REFERENCE : CHKBFFUN.BAS
\***
\******************************************************************************

  %INCLUDE CHKBFDEC.J86

  INTEGER*2 GLOBAL CURRENT.REPORT.NUM%
  
  STRING GLOBAL FILE.OPERATION$, CURRENT.CODE$
  

  FUNCTION CHKBF.SET PUBLIC
\***************************

     CHKBF.FILE.NAME$  = "CHKBF"
     CHKBF.REPORT.NUM% = 209
     CHKBF.RECL%       = 20
     
  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L       

  FUNCTION READ.CHKBF PUBLIC
\****************************   

     INTEGER*2 READ.CHKBF
     
     READ.CHKBF = 1
     
     
     IF END #CHKBF.SESS.NUM% THEN READ.ERROR     
     READ FORM "C13,C6,C1"; #CHKBF.SESS.NUM% ;				\
	  CHKBF.ITEM.CODE$,						\
  	  CHKBF.PRICE$,							\
	  CHKBF.FILLER$

     READ.CHKBF = 0
     EXIT FUNCTION
     
     
     READ.ERROR:
     
        FILE.OPERATION$     = "R"
        CURRENT.CODE$       = PACK$("0000000000000000")
        CURRENT.REPORT.NUM% = CHKBF.REPORT.NUM% 		
     
        EXIT FUNCTION          
     

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.CHKBF PUBLIC
\*****************************

   INTEGER*2 WRITE.CHKBF
   
   WRITE.CHKBF = 1

   IF END #CHKBF.SESS.NUM% THEN WRITE.ERROR
   WRITE FORM "C13,C6,C1"; #CHKBF.SESS.NUM% , CHKBF.POINTER%;		\ CLC
	CHKBF.ITEM.CODE$,						\
	CHKBF.PRICE$,							\
	CHKBF.FILLER$
   WRITE.CHKBF = 0
   EXIT FUNCTION
   
   WRITE.ERROR:
   
      CURRENT.CODE$ = CHKBF.ITEM.CODE$
      FILE.OPERATION$ = "O"
      CURRENT.REPORT.NUM% = CHKBF.REPORT.NUM%
      
      EXIT FUNCTION

END FUNCTION
