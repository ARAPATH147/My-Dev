
\*******************************************************************************
\*******************************************************************************
\***
\***        LDTPF - LDT PROGRAM FILE - PUBLIC FUNCTIONS
\***
\***        REFERENCE: LDTPFFUN.BAS
\***
\***        VERSION A       LES COOK                 2ND NOVEMBER 1992
\***
\*******************************************************************************
\*******************************************************************************
                                                                     
   STRING GLOBAL                        \
      CURRENT.CODE$,                    \
      FILE.OPERATION$
      
   INTEGER*2 GLOBAL                     \
      CURRENT.REPORT.NUM%
      
   %INCLUDE LDTPFDEC.J86
   
   FUNCTION LDTPF.SET PUBLIC
\*****************************

   LDTPF.REPORT.NUM% = 243
   LDTPF.FILE.NAME$  = "LDTPF"
   LDTPF.RECL% = 10
   
   END FUNCTION
\-----------------------------------------------------------------------------   
                                                                     
   FUNCTION READ.LDTPF PUBLIC
\******************************   

      INTEGER*2 READ.LDTPF
      
      READ.LDTPF = 1

      IF END #LDTPF.SESS.NUM% THEN READ.ERROR
      READ #LDTPF.SESS.NUM%; LINE LDTPF.RECORD$
      READ.LDTPF = 0
      EXIT FUNCTION
      
      READ.ERROR:
      
      CURRENT.CODE$ = LDTPF.RECORD$
      FILE.OPERATION$ = "R"
      CURRENT.REPORT.NUM% = LDTPF.REPORT.NUM%
      
      EXIT FUNCTION
                            
   END FUNCTION
\-----------------------------------------------------------------------------   

   FUNCTION WRITE.LDTPF PUBLIC
\*******************************

      INTEGER*2 WRITE.LDTPF
      
      WRITE.LDTPF = 1
         
      IF END #LDTPF.SESS.NUM% THEN WRITE.ERROR
      PRINT #LDTPF.SESS.NUM%; LDTPF.RECORD$   
      WRITE.LDTPF = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = LDTPF.RECORD$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = LDTPF.REPORT.NUM%               
         
         EXIT FUNCTION

   END FUNCTION
