\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   SXTCFFUN.BAS  $
\***
\***   $Revision:   1.2  $
\***
\******************************************************************************
\******************************************************************************\*****************************************************************************
\*****************************************************************************
\***
\***                      SXTCF  FILE FUNCTIONS 
\***
\***                      REFERENCE    : SXTCFFUN
\***
\***
\***           VERSION A       Nik Sen         3rd July 1997
\***
\***           VERSION B       Johnnie Chan    6th Jan 1998
\***                           Added current location code.
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE SXTCFDEC.J86                                       


   FUNCTION SXTCF.SET PUBLIC


    SXTCF.REPORT.NUM%  = 533
    SXTCF.RECL%        = 80
    SXTCF.FILE.NAME$   = "SXTCF"
    SXTCF.REC.NUM%     = 1

END FUNCTION

\----------------------------------------------------------------------------
                                                                     


  FUNCTION READ.SXTCF.LOCK PUBLIC

   INTEGER*1 READ.SXTCF.LOCK

   READ.SXTCF.LOCK = 1

   IF END#SXTCF.SESS.NUM% THEN READ.SXTCF.LOCK.ERROR

    READ FORM "I4,I4,C3,C2,C3,C2,C1,I1,I4,C4,C5,C47";                 \ Ver B
        #SXTCF.SESS.NUM%  AUTOLOCK,             \
                                    SXTCF.REC.NUM%;                         \
                                    SXTCF.STKBF.POINTER%,                   \
                                    SXTCF.ITEM.COUNT%,                      \
                                    SXTCF.START.DATE$,                      \
                                    SXTCF.START.TIME$,                      \
                                    SXTCF.END.DATE$,                        \
                                    SXTCF.END.TIME$,                        \
                                    SXTCF.STOCKTAKE.IN.PROGRESS$,           \
                                    SXTCF.HEADER.EXPECTED%,                 \
                                    SXTCF.CURRENT.COUNT%,                   \
                                    SXTCF.STOCKTAKER$,                      \
                                    SXTCF.CUR.LOCATION$,              \ Ver B
                                    SXTCF.FILLER$
                            
   READ.SXTCF.LOCK = 0
   EXIT FUNCTION

   READ.SXTCF.LOCK.ERROR:

   CURRENT.REPORT.NUM% = SXTCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = STR$(SXTCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.SXTCF.UNLOCK PUBLIC

   INTEGER*1 WRITE.SXTCF.UNLOCK

   WRITE.SXTCF.UNLOCK = 1

   IF END#SXTCF.SESS.NUM% THEN WRITE.SXTCF.UNLOCK.ERROR

    WRITE FORM "I4,I4,C3,C2,C3,C2,C1,I1,I4,C4,C5,C47";                \ Ver B
        #SXTCF.SESS.NUM%  AUTOUNLOCK,             \
                                    SXTCF.REC.NUM%;                         \
                                    SXTCF.STKBF.POINTER%,                   \
                                    SXTCF.ITEM.COUNT%,                      \
                                    SXTCF.START.DATE$,                      \
                                    SXTCF.START.TIME$,                      \
                                    SXTCF.END.DATE$,                        \
                                    SXTCF.END.TIME$,                        \
                                    SXTCF.STOCKTAKE.IN.PROGRESS$,           \
                                    SXTCF.HEADER.EXPECTED%,                 \
                                    SXTCF.CURRENT.COUNT%,                   \
                                    SXTCF.STOCKTAKER$,                      \
                                    SXTCF.CUR.LOCATION$,              \ Ver B
                                    SXTCF.FILLER$


   WRITE.SXTCF.UNLOCK = 0
   EXIT FUNCTION

   WRITE.SXTCF.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = SXTCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(SXTCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.SXTCF.UNLOCK.HOLD PUBLIC

   INTEGER*1 WRITE.SXTCF.UNLOCK.HOLD

   WRITE.SXTCF.UNLOCK.HOLD = 1

   IF END#SXTCF.SESS.NUM% THEN WRITE.SXTCF.UNLOCK.HOLD.ERROR

    WRITE FORM "I4,I4,C3,C2,C3,C2,C1,I1,I4,C4,C5,C47";                \ Ver B
       HOLD  #SXTCF.SESS.NUM%  AUTOUNLOCK,             \
                                    SXTCF.REC.NUM%;                         \
                                    SXTCF.STKBF.POINTER%,                   \
                                    SXTCF.ITEM.COUNT%,                      \
                                    SXTCF.START.DATE$,                      \
                                    SXTCF.START.TIME$,                      \
                                    SXTCF.END.DATE$,                        \
                                    SXTCF.END.TIME$,                        \
                                    SXTCF.STOCKTAKE.IN.PROGRESS$,           \
                                    SXTCF.HEADER.EXPECTED%,                 \
                                    SXTCF.CURRENT.COUNT%,                   \
                                    SXTCF.STOCKTAKER$,                      \
                                    SXTCF.CUR.LOCATION$,              \ Ver B
                                    SXTCF.FILLER$



   WRITE.SXTCF.UNLOCK.HOLD = 0
   EXIT FUNCTION

   WRITE.SXTCF.UNLOCK.HOLD.ERROR:

   CURRENT.REPORT.NUM% = SXTCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(SXTCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION READ.SXTCF PUBLIC

   INTEGER*1 READ.SXTCF

   READ.SXTCF = 1

   IF END#SXTCF.SESS.NUM% THEN READ.SXTCF.ERROR

    READ FORM "I4,I4,C3,C2,C3,C2,C1,I1,I4,C4,C5,C47";                 \ Ver B
        #SXTCF.SESS.NUM%,             \
                                    SXTCF.REC.NUM%;                         \
                                    SXTCF.STKBF.POINTER%,                   \
                                    SXTCF.ITEM.COUNT%,                      \
                                    SXTCF.START.DATE$,                      \
                                    SXTCF.START.TIME$,                      \
                                    SXTCF.END.DATE$,                        \
                                    SXTCF.END.TIME$,                        \
                                    SXTCF.STOCKTAKE.IN.PROGRESS$,           \
                                    SXTCF.HEADER.EXPECTED%,                 \
                                    SXTCF.CURRENT.COUNT%,                   \
                                    SXTCF.STOCKTAKER$,                      \
                                    SXTCF.CUR.LOCATION$,              \ Ver B
                                    SXTCF.FILLER$

   READ.SXTCF = 0
   EXIT FUNCTION

   READ.SXTCF.ERROR:

   CURRENT.REPORT.NUM% = SXTCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = STR$(SXTCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.SXTCF PUBLIC

   INTEGER*1 WRITE.SXTCF        

   WRITE.SXTCF = 1

   IF END#SXTCF.SESS.NUM% THEN WRITE.SXTCF.ERROR

    WRITE FORM "I4,I4,C3,C2,C3,C2,C1,I1,I4,C4,C5,C47";                \ Ver B
        #SXTCF.SESS.NUM%,             \
                                    SXTCF.REC.NUM%;                         \
                                    SXTCF.STKBF.POINTER%,                   \
                                    SXTCF.ITEM.COUNT%,                      \
                                    SXTCF.START.DATE$,                      \
                                    SXTCF.START.TIME$,                      \
                                    SXTCF.END.DATE$,                        \
                                    SXTCF.END.TIME$,                        \
                                    SXTCF.STOCKTAKE.IN.PROGRESS$,           \
                                    SXTCF.HEADER.EXPECTED%,                 \
                                    SXTCF.CURRENT.COUNT%,                   \
                                    SXTCF.STOCKTAKER$,                      \
                                    SXTCF.CUR.LOCATION$,              \ Ver B
                                    SXTCF.FILLER$

   WRITE.SXTCF = 0
   EXIT FUNCTION

   WRITE.SXTCF.ERROR:

   CURRENT.REPORT.NUM% = SXTCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = STR$(SXTCF.REC.NUM%)

   EXIT FUNCTION
  END FUNCTION


