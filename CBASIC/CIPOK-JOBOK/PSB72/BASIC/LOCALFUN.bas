\*****************************************************************************
\***      Local Price Report Data File Functions 
\***      Version A           Paul Flanagan         28.06.93
\***
\***      Version B           Clive Norris          24.11.93
\***      AUTH.NUM$, STOCK.FIG$ and RETRIEVAL.FLAG$ replaced filler as
\***      part of the RETURNS/AUTOMATIC CREDIT CLAIMING package.
\***
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE LOCALDEC.J86

   FUNCTION LOCAL.SET PUBLIC

    LOCAL.REPORT.NUM%   = 306
    LOCAL.RECL%         = 40            
    LOCAL.FILE.NAME$    = "LOCAL"
    
   END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.LOCAL PUBLIC

   INTEGER*2 READ.LOCAL

   READ.LOCAL = 1

   IF END#LOCAL.SESS.NUM% THEN READ.LOCAL.ERROR

    READ FORM "T5,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"; #LOCAL.SESS.NUM% \ 
         KEY LOCAL.ITEM.CODE$;                                  \
             LOCAL.PRICE$,                                      \
             LOCAL.START.DATE$,                                 \
             LOCAL.START.TIME$,                                 \
             LOCAL.END.DATE$,                                   \
             LOCAL.OPERATOR$,                                   \
             LOCAL.REASON$,                                     \
             LOCAL.H.O.PRICE$,                                  \
             LOCAL.HO.CHANGE$,                                  \
             LOCAL.AUTH.NUM$,                                   \
             LOCAL.STOCK.FIG%,                                  \
             LOCAL.RETRIEVAL.FLAG$

   READ.LOCAL = 0
   EXIT FUNCTION

   READ.LOCAL.ERROR:

   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = LOCAL.ITEM.CODE$

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------
  FUNCTION READ.LOCAL.LOCK PUBLIC

   INTEGER*2 READ.LOCAL.LOCK

   READ.LOCAL.LOCK = 1

   IF END#LOCAL.SESS.NUM% THEN READ.LOCAL.LOCK.ERROR

    READ FORM "T5,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"; #LOCAL.SESS.NUM% AUTOLOCK \ 
         KEY LOCAL.ITEM.CODE$;                                  \
             LOCAL.PRICE$,                                      \
             LOCAL.START.DATE$,                                 \
             LOCAL.START.TIME$,                                 \
             LOCAL.END.DATE$,                                   \
             LOCAL.OPERATOR$,                                   \
             LOCAL.REASON$,                                     \
             LOCAL.H.O.PRICE$,                                  \
             LOCAL.HO.CHANGE$,                                  \
             LOCAL.AUTH.NUM$,                                   \
             LOCAL.STOCK.FIG%,                                  \
             LOCAL.RETRIEVAL.FLAG$

   READ.LOCAL.LOCK = 0
   EXIT FUNCTION

   READ.LOCAL.LOCK.ERROR:

   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = LOCAL.ITEM.CODE$

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.LOCAL PUBLIC

   INTEGER*2 WRITE.LOCAL

   WRITE.LOCAL = 1

   IF END#LOCAL.SESS.NUM% THEN WRITE.LOCAL.ERROR

    WRITE FORM "C4,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"; #LOCAL.SESS.NUM%; \
             LOCAL.ITEM.CODE$,                                    \
             LOCAL.PRICE$,                                        \
             LOCAL.START.DATE$,                                   \
             LOCAL.START.TIME$,                                   \
             LOCAL.END.DATE$,                                     \
             LOCAL.OPERATOR$,                                     \
             LOCAL.REASON$,                                       \
             LOCAL.H.O.PRICE$,                                    \
             LOCAL.HO.CHANGE$,                                    \
             LOCAL.AUTH.NUM$,                                     \
             LOCAL.STOCK.FIG%,                                    \
             LOCAL.RETRIEVAL.FLAG$

   WRITE.LOCAL = 0
   EXIT FUNCTION

   WRITE.LOCAL.ERROR:

   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION
\----------------------------------------------------------------------------


  FUNCTION WRITE.LOCAL.UNLOCK PUBLIC

   INTEGER*2 WRITE.LOCAL.UNLOCK

   WRITE.LOCAL.UNLOCK = 1

   IF END#LOCAL.SESS.NUM% THEN WRITE.LOCAL.UNLOCK.ERROR

    WRITE FORM "C4,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"; #LOCAL.SESS.NUM% \
         AUTOUNLOCK; \
             LOCAL.ITEM.CODE$,                                    \
             LOCAL.PRICE$,                                        \
             LOCAL.START.DATE$,                                   \
             LOCAL.START.TIME$,                                   \
             LOCAL.END.DATE$,                                     \
             LOCAL.OPERATOR$,                                     \
             LOCAL.REASON$,                                       \
             LOCAL.H.O.PRICE$,                                    \
             LOCAL.HO.CHANGE$,                                    \
             LOCAL.AUTH.NUM$,                                     \
             LOCAL.STOCK.FIG%,                                    \
             LOCAL.RETRIEVAL.FLAG$

   WRITE.LOCAL.UNLOCK = 0
   EXIT FUNCTION

   WRITE.LOCAL.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = LOCAL.ITEM.CODE$

   EXIT FUNCTION
  END FUNCTION



    
