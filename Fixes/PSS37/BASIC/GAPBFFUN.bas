\*****************************************************************************
\***                                                                         *
\***           GAPBF - GAP BUFFER FILE FUNCTIONS                             *
\***                                                                         *
\***                    REFERENCE : GAPBFFUN.BAS                             *
\***                                                                         *
\*****************************************************************************


%INCLUDE GAPBFDEC.J86


INTEGER*2 GLOBAL       CURRENT.REPORT.NUM%
  
STRING    GLOBAL       FILE.OPERATION$,                                \
                       CURRENT.CODE$
  

FUNCTION GAPBF.SET PUBLIC

    GAPBF.FILE.NAME$  = "GAPBF"
    GAPBF.REPORT.NUM% = 473    
     
END FUNCTION

       
FUNCTION READ.GAPBF PUBLIC
  
    INTEGER*2 READ.GAPBF
     
    READ.GAPBF = 1          
    IF END #GAPBF.SESS.NUM% THEN READ.ERROR     
    READ #GAPBF.SESS.NUM% ; GAPBF.BOOTS.CODE$
    READ.GAPBF = 0   

    EXIT FUNCTION
         
    READ.ERROR:
     
        FILE.OPERATION$     = "R"
        CURRENT.CODE$       = PACK$("0" + GAPBF.BOOTS.CODE$)
        CURRENT.REPORT.NUM% = GAPBF.REPORT.NUM%
     
END FUNCTION          
     

FUNCTION WRITE.GAPBF PUBLIC

    INTEGER*2 WRITE.GAPBF
   
    WRITE.GAPBF = 1
    IF END #GAPBF.SESS.NUM% THEN WRITE.ERROR
    WRITE #GAPBF.SESS.NUM% ; GAPBF.BOOTS.CODE$
    WRITE.GAPBF = 0 

    EXIT FUNCTION
   
    WRITE.ERROR:
   
        CURRENT.CODE$ = PACK$("0" + GAPBF.BOOTS.CODE$)
        FILE.OPERATION$ = "O"
        CURRENT.REPORT.NUM% = GAPBF.REPORT.NUM%
      
END FUNCTION

