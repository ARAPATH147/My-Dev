
\*******************************************************************************
\*******************************************************************************
\***
\***        PDTWF - PDT WORK FILE - FILE FUNCTIONS
\***
\***        REFERENCE: PDTWFFUN.BAS
\***
\***        8th October 1992
\***
\*******************************************************************************
\*******************************************************************************
                                                                     
   INTEGER*2 GLOBAL             \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                \
      CURRENT.CODE$,            \
      FILE.OPERATION$
      
   %INCLUDE PDTWFDEC.J86
   
  FUNCTION PDTWF.SET PUBLIC
\****************************   

   PDTWF.REPORT.NUM% = 136
   PDTWF.FILE.NAME$  = "PDTWF"
 
 END FUNCTION
\-----------------------------------------------------------------------------
   
  FUNCTION READ.PDTWF PUBLIC
\****************************

     INTEGER*2 READ.PDTWF
     
     READ.PDTWF = 1
     
     IF END #PDTWF.SESS.NUM% THEN READ.ERROR 
     READ #PDTWF.SESS.NUM%; LINE PDTWF.RECORD$

     READ.PDTWF = 0
     EXIT FUNCTION
     
     READ.ERROR:
     
     CURRENT.CODE$ = PDTWF.RECORD$
     FILE.OPERATION$ = "R"
     CURRENT.REPORT.NUM% = PDTWF.REPORT.NUM%
     
     EXIT FUNCTION
                            
   END FUNCTION
\-----------------------------------------------------------------------------
   

  FUNCTION WRITE.PDTWF PUBLIC
\*****************************

     INTEGER*2 WRITE.PDTWF
     
     WRITE.PDTWF = 1
     
     IF END #PDTWF.SESS.NUM% THEN WRITE.ERROR
     PRINT #PDTWF.SESS.NUM%; PDTWF.RECORD$                  
     
     WRITE.PDTWF = 0
     EXIT FUNCTION
     
     WRITE.ERROR:
     
     CURRENT.CODE$ = PDTWF.RECORD$
     FILE.OPERATION$ = "O"
     CURRENT.REPORT.NUM% = PDTWF.REPORT.NUM%
     
     EXIT FUNCTION

   END FUNCTION
