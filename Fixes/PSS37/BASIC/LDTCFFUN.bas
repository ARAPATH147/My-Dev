\*******************************************************************************
\*******************************************************************************
\***
\***         LDTCF - LDT CHECK FILE - FILE FUNCTIONS
\***
\***         REFERENCE: LDTCFFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                    
   INTEGER*2 GLOBAL             \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                \
      CURRENT.CODE$,            \
      FILE.OPERATION$
      
   %INCLUDE LDTCFDEC.J86
   
  FUNCTION LDTCF.SET PUBLIC
\***************************

   LDTCF.REPORT.NUM% = 242
   LDTCF.FILE.NAME$  = "LDTCF"
   
  END FUNCTION
\-----------------------------------------------------------------------------
                                    
                                                                     
  FUNCTION READ.LDTCF PUBLIC
\****************************

      INTEGER*2 READ.LDTCF
      
      READ.LDTCF = 1
      
      IF END #LDTCF.SESS.NUM% THEN READ.ERROR
      READ FORM "C3,C6"; #LDTCF.SESS.NUM%;                              \
           LDTCF.VERSION.NO$,                                           \
           LDTCF.VERSION.DATE$
      READ.LDTCF = 0
      EXIT FUNCTION
      
      READ.ERROR:
      
         CURRENT.CODE$ = LDTCF.VERSION.NO$
         FILE.OPERATION$ = "R"
         CURRENT.REPORT.NUM% = LDTCF.REPORT.NUM%
         
         EXIT FUNCTION
                            
   END FUNCTION
\-----------------------------------------------------------------------------   


  FUNCTION WRITE.LDTCF PUBLIC
\*****************************

      INTEGER*2 WRITE.LDTCF
      
      WRITE.LDTCF = 1
      
      IF END #LDTCF.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C3,C6"; #LDTCF.SESS.NUM%;                             \
            LDTCF.VERSION.NO$,                                          \
            LDTCF.VERSION.DATE$
      WRITE.LDTCF = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = LDTCF.VERSION.NO$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = LDTCF.REPORT.NUM%
         
         EXIT FUNCTION

   END FUNCTION
