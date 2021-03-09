REM \
\******************************************************************************
\******************************************************************************
\***
\***         BUSINESS CENTRE SUPPLY METHOD FILE FUNCTIONS
\***
\***               REFERENCE    : BCSMFFUN.BAS
\***
\***         Version A              Les Cook                     8/9/92
\*** 
\***         Based on BCSMFFNE.J86
\***
\***
\***         Version C              Les Cook                    11/1/93
\***
\***         To include data missed from bcsmffnf.j86 in error   
\***
\***         Version D              Nik Sen                    22/5/98
\***         Changed incorrect filler length in all functions.
\***
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL                           \
         CURRENT.REPORT.NUM%  
  
  STRING GLOBAL                              \
         CURRENT.CODE$,                      \
	 FILE.OPERATION$
	 
  %INCLUDE BCSMFDEC.J86
  
  FUNCTION BCSMF.SET PUBLIC
\***************************

     BCSMF.REPORT.NUM%  = 84
     BCSMF.RECL%      = 33
     BCSMF.FILE.NAME$ = "BCSMF"
  
  END FUNCTION
\-----------------------------------------------------------------------------

  FUNCTION READ.BCSMF PUBLIC
\****************************  

    INTEGER*2 READ.BCSMF
    
    READ.BCSMF = 1
 
    IF END #BCSMF.SESS.NUM% THEN READ.ERROR   
    READ FORM "T2,C14,3I1,I2,3I1,2I2,I1,C1,I1,C3,"; #BCSMF.SESS.NUM%   \ DNS 
         KEY BCSMF.FSI$;                                               \
             BCSMF.NAME$,                                              \
             BCSMF.RECNT.LIMIT%,                                       \
             BCSMF.MIN.RECNT.LIMIT%,                                   \
             BCSMF.MAX.RECNT.LIMIT%,                                   \
             BCSMF.DISCRPNCY.VAL%,                                     \
             BCSMF.DISCRPNCY.CNT%,                                     \
             BCSMF.DISCRPNCY.PERCNT%,                                  \
             BCSMF.STK.CNT.LIMIT%,                                     \
             BCSMF.MIN.LIST.NO%,                                       \
             BCSMF.MAX.LIST.NO%,                                       \
             BCSMF.SEQUENCE.NO%,                                       \
             BCSMF.PSEUDO.BUSINESS.CENTRE$,                            \
	     BCSMF.NO.REPEAT.TICKETS%,				       \ CLC
             BCSMF.FILLER$                                             !

    READ.BCSMF =0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = BCSMF.FSI$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
       
       EXIT FUNCTION

  END FUNCTION
\-----------------------------------------------------------------------------
  
  FUNCTION READ.BCSMF.LOCK PUBLIC				       !
\*********************************

    INTEGER*2 READ.BCSMF.LOCK
    
    READ.BCSMF.LOCK = 1

    IF END #BCSMF.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM "T2,C14,3I1,I2,3I1,2I2,I1,C1,I1,C3,"; #BCSMF.SESS.NUM%   \ DNS
         AUTOLOCK						       \
         KEY BCSMF.FSI$;                                               \
             BCSMF.NAME$,                                              \ 
             BCSMF.RECNT.LIMIT%,                                       \ 
             BCSMF.MIN.RECNT.LIMIT%,                                   \ 
             BCSMF.MAX.RECNT.LIMIT%,                                   \ 
             BCSMF.DISCRPNCY.VAL%,                                     \ 
             BCSMF.DISCRPNCY.CNT%,                                     \ 
             BCSMF.DISCRPNCY.PERCNT%,                                  \ 
             BCSMF.STK.CNT.LIMIT%,                                     \ 
             BCSMF.MIN.LIST.NO%,                                       \ 
             BCSMF.MAX.LIST.NO%,                                       \ 
             BCSMF.SEQUENCE.NO%,                                       \ 
             BCSMF.PSEUDO.BUSINESS.CENTRE$,                            \ 
	     BCSMF.NO.REPEAT.TICKETS%,				       \ CLC	     
             BCSMF.FILLER$                                             ! 
    READ.BCSMF.LOCK = 0
    EXIT FUNCTION
    
    READ.LOCK.ERROR:
       
       CURRENT.CODE$ = BCSMF.FSI$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
       
       EXIT FUNCTION	     

  END FUNCTION							       ! 
\-----------------------------------------------------------------------------
  
  
  FUNCTION WRITE.BCSMF PUBLIC                                          
\*****************************

    INTEGER*2 WRITE.BCSMF
    
    WRITE.BCSMF = 1
    
    IF END #BCSMF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C1,C14,3I1,I2,3I1,2I2,I1,C1,I1,C3,"; #BCSMF.SESS.NUM%; \ DNS 
             BCSMF.FSI$,                                               \ 
	     BCSMF.NAME$,                                              \ 
	     BCSMF.RECNT.LIMIT%,                                       \ 
	     BCSMF.MIN.RECNT.LIMIT%,                                   \ 
	     BCSMF.MAX.RECNT.LIMIT%,                                   \ 
	     BCSMF.DISCRPNCY.VAL%,                                     \ 
             BCSMF.DISCRPNCY.CNT%,                                     \ 
	     BCSMF.DISCRPNCY.PERCNT%,                                  \ 
	     BCSMF.STK.CNT.LIMIT%,                                     \ 
	     BCSMF.MIN.LIST.NO%,                                       \  
	     BCSMF.MAX.LIST.NO%,                                       \ 
	     BCSMF.SEQUENCE.NO%,                                       \ 
	     BCSMF.PSEUDO.BUSINESS.CENTRE$,                            \ 	
	     BCSMF.NO.REPEAT.TICKETS%,				       \ CLC	     
	     BCSMF.FILLER$                                             ! 
	     
    WRITE.BCSMF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
    CURRENT.CODE$ = BCSMF.FSI$
    FILE.OPERATION$ = "O"
    CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
    
    EXIT FUNCTION
    
  END FUNCTION                                                         
\-----------------------------------------------------------------------------
  
  	        
  FUNCTION WRITE.BCSMF.UNLOCK PUBLIC
\************************************

    INTEGER*2 WRITE.BCSMF.UNLOCK
    
    WRITE.BCSMF.UNLOCK = 1
      
    IF END #BCSMF.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    WRITE FORM "C1,C14,3I1,I2,3I1,2I2,I1,C1,I1,C3,"; #BCSMF.SESS.NUM%  \ DNS 
         AUTOUNLOCK ;						       \ 
             BCSMF.FSI$,                                               \ 
             BCSMF.NAME$,                                              \ 
             BCSMF.RECNT.LIMIT%,                                       \ 
             BCSMF.MIN.RECNT.LIMIT%,                                   \ 
             BCSMF.MAX.RECNT.LIMIT%,                                   \ 
             BCSMF.DISCRPNCY.VAL%,                                     \ 
             BCSMF.DISCRPNCY.CNT%,                                     \ 
             BCSMF.DISCRPNCY.PERCNT%,                                  \ 
             BCSMF.STK.CNT.LIMIT%,                                     \ 
             BCSMF.MIN.LIST.NO%,                                       \ 
             BCSMF.MAX.LIST.NO%,                                       \ 
             BCSMF.SEQUENCE.NO%,                                       \ 
             BCSMF.PSEUDO.BUSINESS.CENTRE$,                            \ 
	     BCSMF.NO.REPEAT.TICKETS%,				       \ CLC	     
             BCSMF.FILLER$                                             
    WRITE.BCSMF.UNLOCK = 0
    EXIT FUNCTION
    
    WRITE.UNLOCK.ERROR:
    
    CURRENT.CODE$ = BCSMF.FSI$
    FILE.OPERATION$ = "O"
    CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
    
    EXIT FUNCTION

  END FUNCTION							       
\-----------------------------------------------------------------------------
  
