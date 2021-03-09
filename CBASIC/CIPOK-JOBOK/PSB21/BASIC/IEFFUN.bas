REM \
\******************************************************************************
\******************************************************************************
\***
\***         ITEM EAN CODES FILE FUNCTIONS
\***
\***         REFERENCE    : IEFFUN.BAS
\***
\***    VERSION B.              ROBERT COWEY.                       21 OCT 1993.
\***    Corrected setting of FILE.OPERATION$ within WRITE function.
\***
\***    VERSION C.              Steve Perkins                       8th Feb 1996
\***    Version letter removed from IEFDEC.J86
\***
\***
\*******************************************************************************
********************************************************************************

  INTEGER*2 GLOBAL 			\
     CURRENT.REPORT.NUM%
     
  STRING GLOBAL				\
     CURRENT.CODE$,			\
     FILE.OPERATION$
     
  %INCLUDE IEFDEC.J86                                                 ! BRC


  FUNCTION IEF.SET PUBLIC
\*************************

     IEF.REPORT.NUM% = 8
     IEF.RECL% = 15
     IEF.FILE.NAME$ = "IEF"
     
  END FUNCTION
  

\-----------------------------------------------------------------------------

  FUNCTION READ.IEF PUBLIC
\**************************

    INTEGER*2 READ.IEF
    
    READ.IEF = 1
      
    IF END #IEF.SESS.NUM% THEN READ.ERROR
    READ FORM "T10,C6"; #IEF.SESS.NUM%				    \   
         KEY IEF.BOOTS.CODE.BAR.CODE$;                              \  	     
             IEF.NEXT.BAR.CODE$

    READ.IEF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
    CURRENT.CODE$ = IEF.BOOTS.CODE.BAR.CODE$
    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%	     
    
    EXIT FUNCTION

  END FUNCTION
\-----------------------------------------------------------------------------

  FUNCTION WRITE.IEF PUBLIC
\***************************

    INTEGER*2 WRITE.IEF
    
    WRITE.IEF = 1
      
    IF END #IEF.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C9,C6"; #IEF.SESS.NUM%;                             \     
             IEF.BOOTS.CODE.BAR.CODE$,                              \     
             IEF.NEXT.BAR.CODE$

    WRITE.IEF = 0
    EXIT FUNCTION
    
    WRITE.ERROR:
    
    CURRENT.CODE$ = IEF.BOOTS.CODE.BAR.CODE$
    FILE.OPERATION$ = "W"                                              ! BRC
    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%
    
    EXIT FUNCTION	     

  END FUNCTION
