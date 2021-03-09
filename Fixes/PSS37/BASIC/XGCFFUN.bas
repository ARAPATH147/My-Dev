\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR STOCKTAKE GROUP CODE FILE PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   XGCFFUN.BAS
\***
\***        FILE TYPE   :   Keyed
\***
\***    VERSION A.              STEVE WRIGHT.                       24 AUG 1999.
\***    Design changed, forcing key to change from 3 byte Product Group to
\***    1 byte Business Centre letter. Key length of 3 has been retained to
\***    allow for key to change back to Product Group if BC proves inadequete
\***
\*******************************************************************************
\*******************************************************************************

INTEGER*2 GLOBAL           \
   CURRENT.REPORT.NUM% 
         
STRING GLOBAL              \
   CURRENT.CODE$,          \
   FILE.OPERATION$           

%INCLUDE XGCFDEC.J86


FUNCTION XGCF.SET PUBLIC

   XGCF.REPORT.NUM% = 587
   XGCF.RECL% = 14
   XGCF.FILE.NAME$ = "XGCF"

END FUNCTION


FUNCTION READ.XGCF PUBLIC
   INTEGER*2 READ.XGCF
    
   READ.XGCF = 1
   
   XGCF.PRODUCT.GRP$ = XGCF.BSNS.CNTR$ + CHR$(0) + CHR$(0)    

   IF END #XGCF.SESS.NUM% THEN READ.ERROR   
   READ FORM "T4,C11";                                                  \
             #XGCF.SESS.NUM%                                            \
         KEY XGCF.PRODUCT.GRP$;                                         \
             XGCF.BAR.CODE$

   READ.XGCF = 0
   EXIT FUNCTION     
     
READ.ERROR:
   FILE.OPERATION$ = "R"
   CURRENT.REPORT.NUM% = XGCF.REPORT.NUM%
   CURRENT.CODE$ = UNPACK$(XGCF.PRODUCT.GRP$)
   
END FUNCTION


FUNCTION READ.XGCF.LOCK PUBLIC
   INTEGER*2 READ.XGCF.LOCK
    
   READ.XGCF.LOCK = 1    

   XGCF.PRODUCT.GRP$ = XGCF.BSNS.CNTR$ + CHR$(0) + CHR$(0)    
   
   IF END #XGCF.SESS.NUM% THEN READ.ERROR
   READ FORM "T4,C11";                                                  \
             #XGCF.SESS.NUM% AUTOLOCK                                   \
         KEY XGCF.PRODUCT.GRP$;                                         \
             XGCF.BAR.CODE$
             
   READ.XGCF.LOCK = 0
   EXIT FUNCTION
    
READ.ERROR:                 
   FILE.OPERATION$ = "R"
   CURRENT.REPORT.NUM% = XGCF.REPORT.NUM%
   CURRENT.CODE$ = UNPACK$(XGCF.PRODUCT.GRP$)
   
END FUNCTION


FUNCTION WRITE.XGCF PUBLIC
   INTEGER*2 WRITE.XGCF
    
   WRITE.XGCF = 1

   XGCF.PRODUCT.GRP$ = XGCF.BSNS.CNTR$ + CHR$(0) + CHR$(0)    

   IF END #XGCF.SESS.NUM% THEN WRITE.ERROR
   WRITE FORM "C3,C11";                                                 \
             #XGCF.SESS.NUM%;                                           \
             XGCF.PRODUCT.GRP$,                                         \
             XGCF.BAR.CODE$

   WRITE.XGCF = 0
   EXIT FUNCTION

WRITE.ERROR:
   FILE.OPERATION$ = "W"
   CURRENT.REPORT.NUM% = XGCF.REPORT.NUM%
   CURRENT.CODE$ = UNPACK$(XGCF.PRODUCT.GRP$)
   
END FUNCTION


FUNCTION WRITE.XGCF.HOLD PUBLIC
   INTEGER*2 WRITE.XGCF.HOLD
    
   WRITE.XGCF.HOLD = 1

   XGCF.PRODUCT.GRP$ = XGCF.BSNS.CNTR$ + CHR$(0) + CHR$(0)    

   IF END #XGCF.SESS.NUM% THEN WRITE.ERROR
   WRITE FORM "C3,C11"; HOLD                                            \
             #XGCF.SESS.NUM%;                                           \
             XGCF.PRODUCT.GRP$,                                         \
             XGCF.BAR.CODE$

   WRITE.XGCF.HOLD = 0
   EXIT FUNCTION

WRITE.ERROR:
   FILE.OPERATION$ = "W"
   CURRENT.REPORT.NUM% = XGCF.REPORT.NUM%
   CURRENT.CODE$ = UNPACK$(XGCF.PRODUCT.GRP$)
   
END FUNCTION
 
FUNCTION WRITE.XGCF.UNLOCK PUBLIC
   INTEGER*2 WRITE.XGCF.UNLOCK
    
   WRITE.XGCF.UNLOCK = 1

   XGCF.PRODUCT.GRP$ = XGCF.BSNS.CNTR$ + CHR$(0) + CHR$(0)    

   IF END #XGCF.SESS.NUM% THEN WRITE.ERROR
   WRITE FORM "C3,C11";                                                 \
             #XGCF.SESS.NUM% AUTOUNLOCK;                                \
             XGCF.PRODUCT.GRP$,                                         \
             XGCF.BAR.CODE$

   WRITE.XGCF.UNLOCK = 0
   EXIT FUNCTION

WRITE.ERROR:
   FILE.OPERATION$ = "W"
   CURRENT.REPORT.NUM% = XGCF.REPORT.NUM%
   CURRENT.CODE$ = UNPACK$(XGCF.PRODUCT.GRP$)

END FUNCTION

