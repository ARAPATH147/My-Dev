
\******************************************************************************
\***
\***                  LDTBF - LDT BUFFER FILE FUNCTIONS
\***
\***                      REFERENCE : LDTBFFUN.BAS
\*** 
\***            Author :  Mike Kelsall   -   12th October 1992
\***
\******************************************************************************
\***
\***    Version B                Stuart Highley               17-09-97
\***    Added new fields for GRBI changes.
\***
\******************************************************************************

  %INCLUDE LDTBFDEC.J86

  INTEGER*2 GLOBAL CURRENT.REPORT.NUM%
  
  STRING GLOBAL FILE.OPERATION$, CURRENT.CODE$
  

  FUNCTION LDTBF.SET PUBLIC
\***************************

     LDTBF.FILE.NAME$  = "LDTBF"
     LDTBF.REPORT.NUM% = 210
     LDTBF.RECL%       = 20
     
  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT^L       

  FUNCTION READ.LDTBF PUBLIC
\****************************   

     INTEGER*2 READ.LDTBF
     
     READ.LDTBF = 1
     
     IF END #LDTBF.SESS.NUM% THEN READ.ERROR
     READ #LDTBF.SESS.NUM% ;  LDTBF.RECORD.ID$

     IF LDTBF.RECORD.ID$ = "OH" THEN BEGIN
        READ #LDTBF.SESS.NUM% ; LDTBF.SUPPLIER.NUM$,                   \
                                LDTBF.ORDER.NUM$,                      \
                                LDTBF.ORDER.SUFFIX$,                   \
                                LDTBF.DATE$,                           \ BSH
                                LDTBF.START.TIME$,                     \ BSH
                                LDTBF.END.TIME$,                       \ BSH
                                LDTBF.LOGGED.QTY%                      ! BSH
     ENDIF ELSE IF LDTBF.RECORD.ID$ = "OD" THEN BEGIN
        READ #LDTBF.SESS.NUM% ; LDTBF.REF.CODE$,                       \
                                LDTBF.ORDERED.QTY%,                    \ BSH
                                LDTBF.GOOD.QTY%,                       \ BSH
                                LDTBF.DAMAGED.QTY%,                    \
                                LDTBF.STOLEN.QTY%
     ENDIF ELSE IF LDTBF.RECORD.ID$ = "OT" THEN BEGIN
        READ #LDTBF.SESS.NUM% ; LDTBF.NUM.OF.ITEMS%
     ENDIF ELSE BEGIN
        GOTO READ.ERROR
     ENDIF

     READ.LDTBF = 0
     EXIT FUNCTION
     
     
   READ.ERROR:
     
     FILE.OPERATION$     = "R"
     CURRENT.CODE$       = PACK$("0000000000000000")
     CURRENT.REPORT.NUM% = LDTBF.REPORT.NUM%
     
     EXIT FUNCTION          
     

  END FUNCTION
\------------------------------------------------------------------------------

  FUNCTION WRITE.LDTBF PUBLIC
\*****************************

     INTEGER*2 WRITE.LDTBF
     
     WRITE.LDTBF = 1
     
     IF END #LDTBF.SESS.NUM% THEN WRITE.ERROR
     WRITE #LDTBF.SESS.NUM%; LDTBF.RECORD.ID$
     
     IF LDTBF.RECORD.ID$ = "OH" THEN BEGIN
        WRITE # LDTBF.SESS.NUM% ; LDTBF.SUPPLIER.NUM$,                 \
                                  LDTBF.ORDER.NUM$,                    \
                                  LDTBF.ORDER.SUFFIX$,                 \
                                  LDTBF.DATE$,                         \ BSH
                                  LDTBF.START.TIME$,                   \ BSH
                                  LDTBF.END.TIME$,                     \ BSH
                                  LDTBF.LOGGED.QTY%                    ! BSH
        GOTO WRITE.COMPLETE
     ENDIF 

     IF LDTBF.RECORD.ID$ = "OD" THEN BEGIN                        
        WRITE #LDTBF.SESS.NUM% ; LDTBF.REF.CODE$,                      \
                                 LDTBF.ORDERED.QTY%,                   \ BSH
                                 LDTBF.GOOD.QTY%,                      \ BSH
                                 LDTBF.DAMAGED.QTY%,                   \
                                 LDTBF.STOLEN.QTY%
        GOTO WRITE.COMPLETE
     ENDIF

     IF LDTBF.RECORD.ID$ = "OT" THEN BEGIN
        WRITE #LDTBF.SESS.NUM% ; LDTBF.NUM.OF.ITEMS%
        GOTO WRITE.COMPLETE
     ENDIF

     GOTO WRITE.ERROR


   WRITE.COMPLETE:

     WRITE.LDTBF = 0
     EXIT FUNCTION


   WRITE.ERROR:

     FILE.OPERATION$     = "W"
     CURRENT.CODE$       = PACK$("0000000000000000")
     CURRENT.REPORT.NUM% = LDTBF.REPORT.NUM%

     EXIT FUNCTION
     

  END FUNCTION

