\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   SOFTSFUN.bas  $
\***
\***   $Revision:   1.4  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/SOFTSFUN.bav  $
\***   
\***      Rev 1.4   26 Oct 2006 14:26:46   paul.bowers
\***    
\***   
\***      Rev 1.3   Oct 22 1999 14:47:44   dev11ps
\***    
\***   
\***      Rev 1.2   22 Mar 1995 13:23:18   DEVDSPS
\***   Fix to WRITE.SOFTS
\***   
\***      Rev 1.1   11 Oct 1994 15:04:22   NIK
\***    
\***   
\***      Rev 1.0   11 Oct 1994 14:50:00   DEVNSPS
\***    
\***   
\******************************************************************************
\******************************************************************************REM \
\******************************************************************************
\******************************************************************************
\***
\***               FUNCTIONS FOR THE SOFTWARE STATUS FILE
\***
\***                     REFERENCE    : SOFTSFUN.BAS
\***
\***	   Version A          Andrew Wedgeworth           24th June 1992
\***
\***   Version B               Nik Sen                 11th October 1994
\***   Added WRITE.SOFTS function. This function had been added previously
\***   but was not on the archive.      
\***
\***   Version C           David Smallwood             22nd March 1995
\***   Correct WRITE.SOFTS: pad with spaces and terminate with 0D0A.
\***
\******************************************************************************
\*******************************************************************************

   %INCLUDE SOFTSDEC.J86

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL                                                       \
      CURRENT.CODE$,                                                   \
      FILE.OPERATION$          
   
   
   FUNCTION SOFTS.SET PUBLIC   
 \***************************
  
      SOFTS.REPORT.NUM% = 247            
      SOFTS.RECL%       = 80 
      SOFTS.FILE.NAME$  = "SOFTS"
  
  
   END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L    
 
 
   FUNCTION READ.SOFTS PUBLIC
 \****************************   

      INTEGER*2 I%, READ.SOFTS      
    
      READ.SOFTS = 1

      IF END #SOFTS.SESS.NUM% THEN END.OF.SOFTS    
      READ FORM "T1,C80"; #SOFTS.SESS.NUM%,SOFTS.REC.NUM%; SOFTS.RECORD$
    
      I% = 78
      WHILE MID$(SOFTS.RECORD$,I%,1) = " "      \
       AND  I% > 1                           
         I% = I% - 1
      WEND
      SOFTS.RECORD$ = LEFT$(SOFTS.RECORD$,I%)  

      READ.SOFTS = 0     
      EXIT FUNCTION      
      
      
      END.OF.SOFTS:
      
         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%	 	  

         EXIT FUNCTION	                   
          

   END FUNCTION                             

\---------------------------------------------------------------------------- 
   FUNCTION WRITE.SOFTS PUBLIC
 \****************************   

      INTEGER*2 I%, WRITE.SOFTS                                                    
    
      WRITE.SOFTS = 1      
  
      SOFTS.SPACE$ = STRING$(78 - LEN(SOFTS.RECORD$), " ")                !1.2
      SOFTS.RECORD$ = SOFTS.RECORD$ + SOFTS.SPACE$ + CHR$(13) + CHR$(10)  !1.2
  
      IF END #SOFTS.SESS.NUM% THEN END.OF.WRITE.SOFTS  
      WRITE FORM "C80"; #SOFTS.SESS.NUM%,SOFTS.REC.NUM%; SOFTS.RECORD$    !1.2          
 
      WRITE.SOFTS = 0     
      EXIT FUNCTION      
      
      
      END.OF.WRITE.SOFTS:
      
         FILE.OPERATION$     = "W"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%	 	  

         EXIT FUNCTION	                   
          

   END FUNCTION                           
\------------------------------------------------------------------------------
REM EJECT^L

\------------------------------------------------------------------------------
