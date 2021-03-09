\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   STKCFFUN.bas  $
\***
\***   $Revision:   1.3  $
\***
\******************************************************************************
\******************************************************************************\*****************************************************************************
\*****************************************************************************
\***
\***                      STKCF  FILE FUNCTIONS 
\***
\***                      REFERENCE    : STKCFFUA
\***
\***           VERSION A : STEVEN GOULDING  11.01.93
\***
\***           VERSION B       Nik Sen         5th June 1997
\***           Added Stocktake.In.Progress flag 
\***
\***           VERSION C       Nik Sen         3rd July 1997
\***           Removed Stocktake In Progress flag. Now in Stocktake Control
\***           file.
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE STKCFDEC.J86                                       ! BNS


   FUNCTION STKCF.SET PUBLIC
REM \

    STKCF.REPORT.NUM%  = 109
    STKCF.RECL%        = 80
    STKCF.FILE.NAME$   = "STKCF"
    STKCF.REC.NUM%     = 1

END FUNCTION

\----------------------------------------------------------------------------

REM \
                                                                     


  FUNCTION READ.STKCF.LOCK PUBLIC

   INTEGER*1 READ.STKCF.LOCK

   READ.STKCF.LOCK = 1

   IF END#STKCF.SESS.NUM% THEN READ.STKCF.LOCK.ERROR

    READ FORM "I4,I4,I2,C1,C1,I1,C3,C2,C1,C1,C1,C1,C58";          \CNS
    				    #STKCF.SESS.NUM%  AUTOLOCK,             \
                                    STKCF.REC.NUM%;                         \
                                    STKCF.STKMQ.POINTER%,                   \
                                    STKCF.ITEM.COUNT%,                      \
                                    STKCF.STMVT.REC.CNT%,                   \
                                    STKCF.STOCK.SUPPORT.STATUS$,            \
                                    STKCF.STK.FILE.AVAIL$,                  \
                                    STKCF.PREV.PITRL.UPDATE%,               \
			  	    STKCF.LAST.DIRORD.SUPPLIER$,	    \
				    STKCF.LAST.DIRORD.ORDER.NO$,	    \
			    	    STKCF.LAST.DIRORD.ORDER.SUFFIX$,	    \
				    STKCF.LAST.DIRORD.BC$,	 	    \
			  	    STKCF.LAST.DIRORD.REC.SOURCE$, 	    \
				    STKCF.STOCK.TAKE.PROCESSING.REQUIRED$,  \
				    STKCF.FILLER$
                            
   READ.STKCF.LOCK = 0
   EXIT FUNCTION

   READ.STKCF.LOCK.ERROR:

   CURRENT.REPORT.NUM% = STKCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = STR$(STKCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.STKCF.UNLOCK PUBLIC

   INTEGER*1 WRITE.STKCF.UNLOCK

   WRITE.STKCF.UNLOCK = 1

   IF END#STKCF.SESS.NUM% THEN WRITE.STKCF.UNLOCK.ERROR

    WRITE FORM "I4,I4,I2,C1,C1,I1,C3,C2,C1,C1,C1,C1,C58";           \ CNS
    				    #STKCF.SESS.NUM%  AUTOUNLOCK,         \
                                    STKCF.REC.NUM%;                       \
                                    STKCF.STKMQ.POINTER%,                 \
                                    STKCF.ITEM.COUNT%,                    \
                                    STKCF.STMVT.REC.CNT%,                 \
                                    STKCF.STOCK.SUPPORT.STATUS$,          \
                                    STKCF.STK.FILE.AVAIL$,                \
                                    STKCF.PREV.PITRL.UPDATE%,             \
			  	    STKCF.LAST.DIRORD.SUPPLIER$,	    \
				    STKCF.LAST.DIRORD.ORDER.NO$,	    \
				    STKCF.LAST.DIRORD.ORDER.SUFFIX$,	    \
				    STKCF.LAST.DIRORD.BC$,	 	    \
				    STKCF.LAST.DIRORD.REC.SOURCE$,	    \ 
				    STKCF.STOCK.TAKE.PROCESSING.REQUIRED$,  \         
				    STKCF.FILLER$

   WRITE.STKCF.UNLOCK = 0
   EXIT FUNCTION

   WRITE.STKCF.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = STKCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(STKCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.STKCF.UNLOCK.HOLD PUBLIC

   INTEGER*1 WRITE.STKCF.UNLOCK.HOLD

   WRITE.STKCF.UNLOCK.HOLD = 1

   IF END#STKCF.SESS.NUM% THEN WRITE.STKCF.UNLOCK.HOLD.ERROR

    WRITE FORM "I4,I4,I2,C1,C1,I1,C3,C2,C1,C1,C1,C1,C58";              \ CNS
				    HOLD #STKCF.SESS.NUM%  AUTOUNLOCK,    \
                                    STKCF.REC.NUM%;                       \
                                    STKCF.STKMQ.POINTER%,                 \
                                    STKCF.ITEM.COUNT%,                    \
                                    STKCF.STMVT.REC.CNT%,                 \
                                    STKCF.STOCK.SUPPORT.STATUS$,          \
                                    STKCF.STK.FILE.AVAIL$,                \
                                    STKCF.PREV.PITRL.UPDATE%,             \
			  	    STKCF.LAST.DIRORD.SUPPLIER$,	    \
				    STKCF.LAST.DIRORD.ORDER.NO$,	    \
				    STKCF.LAST.DIRORD.ORDER.SUFFIX$,	    \
				    STKCF.LAST.DIRORD.BC$,	 	    \
				    STKCF.LAST.DIRORD.REC.SOURCE$,	    \
	                            STKCF.STOCK.TAKE.PROCESSING.REQUIRED$,  \         
    	                            STKCF.FILLER$

   WRITE.STKCF.UNLOCK.HOLD = 0
   EXIT FUNCTION

   WRITE.STKCF.UNLOCK.HOLD.ERROR:

   CURRENT.REPORT.NUM% = STKCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(STKCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION READ.STKCF PUBLIC

   INTEGER*1 READ.STKCF

   READ.STKCF = 1

   IF END#STKCF.SESS.NUM% THEN READ.STKCF.ERROR

    READ FORM "I4,I4,I2,C1,C1,I1,C3,C2,C1,C1,C1,C1,C58";             \ CNS
    				    #STKCF.SESS.NUM%,                \
                                    STKCF.REC.NUM%;                  \ 
                                    STKCF.STKMQ.POINTER%,            \
                                    STKCF.ITEM.COUNT%,               \
                                    STKCF.STMVT.REC.CNT%,            \
                                    STKCF.STOCK.SUPPORT.STATUS$,     \
                                    STKCF.STK.FILE.AVAIL$,           \
                                    STKCF.PREV.PITRL.UPDATE%,        \
			  	    STKCF.LAST.DIRORD.SUPPLIER$,	    \
				    STKCF.LAST.DIRORD.ORDER.NO$,	    \
				    STKCF.LAST.DIRORD.ORDER.SUFFIX$,	    \
				    STKCF.LAST.DIRORD.BC$,	 	    \
				    STKCF.LAST.DIRORD.REC.SOURCE$,	    \
				    STKCF.STOCK.TAKE.PROCESSING.REQUIRED$,  \                            
    	                            STKCF.FILLER$
              


   READ.STKCF = 0
   EXIT FUNCTION

   READ.STKCF.ERROR:

   CURRENT.REPORT.NUM% = STKCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = STR$(STKCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.STKCF PUBLIC

   INTEGER*1 WRITE.STKCF        

   WRITE.STKCF = 1

   IF END#STKCF.SESS.NUM% THEN WRITE.STKCF.ERROR

    WRITE FORM "I4,I4,I2,C1,C1,I1,C3,C2,C1,C1,C1,C1,C58";         \ CNS
    				    #STKCF.SESS.NUM%,                \
                                    STKCF.REC.NUM%;                  \
                                    STKCF.STKMQ.POINTER%,            \
                                    STKCF.ITEM.COUNT%,               \
                                    STKCF.STMVT.REC.CNT%,            \
                                    STKCF.STOCK.SUPPORT.STATUS$,     \
                                    STKCF.STK.FILE.AVAIL$,           \
                                    STKCF.PREV.PITRL.UPDATE%,        \
			  	    STKCF.LAST.DIRORD.SUPPLIER$,	    \
				    STKCF.LAST.DIRORD.ORDER.NO$,	    \
				    STKCF.LAST.DIRORD.ORDER.SUFFIX$,	    \
				    STKCF.LAST.DIRORD.BC$,	 	    \
				    STKCF.LAST.DIRORD.REC.SOURCE$,	    \
				    STKCF.STOCK.TAKE.PROCESSING.REQUIRED$,  \                             
	                            STKCF.FILLER$
                            

   WRITE.STKCF = 0
   EXIT FUNCTION

   WRITE.STKCF.ERROR:

   CURRENT.REPORT.NUM% = STKCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(STKCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


