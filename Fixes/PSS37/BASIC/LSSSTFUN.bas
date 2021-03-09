\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE LSS STOCK COUNT FILE
\***
\***                     REFERENCE    : LSSSTFUN.BAS
\***
\***       Version A          Brian Greenfield         16th October 2002
\***
\******************************************************************************
\******************************************************************************

%INCLUDE LSSSTDEC.J86

INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

STRING GLOBAL FILE.OPERATION$

STRING SPACE$          

\------------------------------------------------------------------------------\
   
FUNCTION LSSST.SET PUBLIC   
      
   LSSST.REPORT.NUM% = 657
   LSSST.RECL%       = 27
   LSSST.FILE.NAME$ = "C:\LSSST.BIN"
      
END FUNCTION

\------------------------------------------------------------------------------\

FUNCTION READ.LSSST PUBLIC
      
   INTEGER*2 READ.LSSST      
    
   READ.LSSST = 1

   IF END #LSSST.SESS.NUM% THEN END.OF.LSSST
   READ FORM "T1,C11,C4,C5,C5,C2"; #LSSST.SESS.NUM%; \
             LSSST.IRF.BAR.CODE$,     \
             LSSST.STOCK.BOOTS.CODE$, \
             LSSST.STOCK.COUNT$,      \
             LSSST.TSF$,              \
             SPACE$
    
   READ.LSSST = 0     
   
   EXIT FUNCTION      
      
END.OF.LSSST:
      
   FILE.OPERATION$     = "R"
   CURRENT.REPORT.NUM% = LSSST.REPORT.NUM%          
   
   EXIT FUNCTION                     
          
END FUNCTION                             

\----------------------------------------------------------------------------\

FUNCTION WRITE.LSSST PUBLIC
 
   INTEGER*2 WRITE.LSSST                                                    
    
   WRITE.LSSST = 1      
  
   IF END #LSSST.SESS.NUM% THEN END.OF.WRITE.LSSST
   WRITE FORM "C11,C4,C5,C5,C2"; #LSSST.SESS.NUM%; \
              LSSST.IRF.BAR.CODE$,     \
              LSSST.STOCK.BOOTS.CODE$, \
              LSSST.STOCK.COUNT$,      \
              LSSST.TSF$,              \
              CHR$(13) + CHR$(10)
 
   WRITE.LSSST = 0     
   
   EXIT FUNCTION      
      
END.OF.WRITE.LSSST:
      
   FILE.OPERATION$     = "W"
   CURRENT.REPORT.NUM% = LSSST.REPORT.NUM%          

   EXIT FUNCTION

END FUNCTION
