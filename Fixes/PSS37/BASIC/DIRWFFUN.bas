\*******************************************************************************
\*******************************************************************************
\***
\***                 FILE FUNCTIONS FOR DIRWF - DIRECT WORK FILE
\***
\***                 REFERENCE: DIRWFFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                            
   INTEGER*2 GLOBAL                     \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                        \
      CURRENT.CODE$,                    \
      FILE.OPERATION$
      
   %INCLUDE DIRWFDEC.J86
   
  FUNCTION DIRWF.SET PUBLIC
\***************************

   DIRWF.REPORT.NUM% = 241
   DIRWF.FILE.NAME$  = "DIRWF"
   DIRWF.RECL%  = 128
   
  END FUNCTION
\-----------------------------------------------------------------------------                                              
                                                                     
  FUNCTION READ.DIRWF PUBLIC
\****************************   
   
      INTEGER*2 READ.DIRWF
      
      READ.DIRWF = 1
      
      IF END #DIRWF.SESS.NUM% THEN READ.ERROR
      READ #DIRWF.SESS.NUM%; LINE DIRWF.RECORD$
      READ.DIRWF = 0
      EXIT FUNCTION
      
      READ.ERROR:
      
         CURRENT.CODE$ = DIRWF.RECORD$
         FILE.OPERATION$ = "R"
         CURRENT.REPORT.NUM% = DIRWF.REPORT.NUM%
         
         EXIT FUNCTION
                            
   END FUNCTION
\-----------------------------------------------------------------------------   


  FUNCTION WRITE.DIRWF PUBLIC
\*****************************

      INTEGER*2 WRITE.DIRWF
      
      WRITE.DIRWF = 1
      
      IF END #DIRWF.SESS.NUM% THEN WRITE.ERROR   
      PRINT #DIRWF.SESS.NUM%; DIRWF.RECORD$   
      WRITE.DIRWF = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = DIRWF.RECORD$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = DIRWF.REPORT.NUM%
         
         EXIT FUNCTION

   END FUNCTION
