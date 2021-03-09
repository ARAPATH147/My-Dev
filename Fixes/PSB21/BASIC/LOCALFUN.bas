\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Local Price File
\***                 Public File Function Definitions
\***
\***    FILE TYPE : Keyed
\***
\***********************************************************************
\***
\***    Version A.          Paul Flanagan               28th Jun 1993
\***    Initial version.
\***
\***    Version B.          Clive Norris                24th Nov 1993
\***    AUTH.NUM$, STOCK.FIG$ and RETRIEVAL.FLAG$ replaced filler as
\***    part of the RETURNS/AUTOMATIC CREDIT CLAIMING package.
\***
\***    Version C.          Mark Walker                 26th Jun 2015
\***    F392 Retail Stock 5
\***    Includes the following changes:
\***    - Added function WRITE.LOCAL.UNLOCK.HOLD.
\***    - Defined key length constant in function LOCAL.SET.
\***
\***********************************************************************
\***********************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

    STRING                                                              \
        FORMAT.STRING$

%INCLUDE LOCALDEC.J86

\***********************************************************************
\***
\***    LOCAL.SET
\***
\***    Declare LOCAL file constants
\***
\***********************************************************************
FUNCTION LOCAL.SET PUBLIC

    LOCAL.FILE.NAME$  = "LOCAL"
    LOCAL.KEYL%       = 4                                                   !CMW
    LOCAL.RECL%       = 40
    LOCAL.REPORT.NUM% = 306
    
END FUNCTION

\***********************************************************************
\***
\***    READ.LOCAL
\***
\***    Read LOCAL file record
\***
\***********************************************************************
FUNCTION READ.LOCAL PUBLIC

    INTEGER*2 READ.LOCAL

    READ.LOCAL = 1

    FORMAT.STRING$ = "T5,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"                  !CMW
    
    IF END #LOCAL.SESS.NUM% THEN READ.LOCAL.ERROR
    READ FORM FORMAT.STRING$; #LOCAL.SESS.NUM%                          \   !CMW
         KEY LOCAL.ITEM.CODE$;                                          \
             LOCAL.PRICE$,                                              \
             LOCAL.START.DATE$,                                         \
             LOCAL.START.TIME$,                                         \
             LOCAL.END.DATE$,                                           \
             LOCAL.OPERATOR$,                                           \
             LOCAL.REASON$,                                             \
             LOCAL.H.O.PRICE$,                                          \
             LOCAL.HO.CHANGE$,                                          \
             LOCAL.AUTH.NUM$,                                           \
             LOCAL.STOCK.FIG%,                                          \
             LOCAL.RETRIEVAL.FLAG$

    READ.LOCAL = 0
   
    EXIT FUNCTION

READ.LOCAL.ERROR:

   FILE.OPERATION$     = "R"
   CURRENT.CODE$       = LOCAL.ITEM.CODE$
   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%

END FUNCTION

\***********************************************************************
\***
\***    READ.LOCAL.LOCK
\***
\***    Read LOCAL file record with lock
\***
\***********************************************************************
FUNCTION READ.LOCAL.LOCK PUBLIC

    INTEGER*2 READ.LOCAL.LOCK

    READ.LOCAL.LOCK = 1
    
    FORMAT.STRING$ = "T5,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"                  !CMW

    IF END #LOCAL.SESS.NUM% THEN READ.LOCAL.LOCK.ERROR
    READ FORM FORMAT.STRING$; #LOCAL.SESS.NUM% AUTOLOCK                 \   !CMW
         KEY LOCAL.ITEM.CODE$;                                          \
             LOCAL.PRICE$,                                              \
             LOCAL.START.DATE$,                                         \
             LOCAL.START.TIME$,                                         \
             LOCAL.END.DATE$,                                           \
             LOCAL.OPERATOR$,                                           \
             LOCAL.REASON$,                                             \
             LOCAL.H.O.PRICE$,                                          \
             LOCAL.HO.CHANGE$,                                          \
             LOCAL.AUTH.NUM$,                                           \
             LOCAL.STOCK.FIG%,                                          \
             LOCAL.RETRIEVAL.FLAG$

    READ.LOCAL.LOCK = 0
   
    EXIT FUNCTION

READ.LOCAL.LOCK.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.CODE$       = LOCAL.ITEM.CODE$
    CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%

END FUNCTION

\***********************************************************************
\***
\***    WRITE.LOCAL
\***
\***    Write LOCAL file record
\***
\***********************************************************************
FUNCTION WRITE.LOCAL PUBLIC

    INTEGER*2 WRITE.LOCAL

    WRITE.LOCAL = 1

    FORMAT.STRING$ = "C4,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"                  !CMW

    IF END #LOCAL.SESS.NUM% THEN WRITE.LOCAL.ERROR
    WRITE FORM FORMAT.STRING$; #LOCAL.SESS.NUM%;                        \   !CMW
        LOCAL.ITEM.CODE$,                                               \
        LOCAL.PRICE$,                                                   \
        LOCAL.START.DATE$,                                              \
        LOCAL.START.TIME$,                                              \
        LOCAL.END.DATE$,                                                \
        LOCAL.OPERATOR$,                                                \
        LOCAL.REASON$,                                                  \
        LOCAL.H.O.PRICE$,                                               \
        LOCAL.HO.CHANGE$,                                               \
        LOCAL.AUTH.NUM$,                                                \
        LOCAL.STOCK.FIG%,                                               \
        LOCAL.RETRIEVAL.FLAG$

    WRITE.LOCAL = 0

    EXIT FUNCTION

WRITE.LOCAL.ERROR:

   FILE.OPERATION$     = "W"
   CURRENT.CODE$       = LOCAL.ITEM.CODE$                                   !CMW
   CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%

END FUNCTION

\***********************************************************************
\***
\***    WRITE.LOCAL.UNLOCK
\***
\***    Write LOCAL file record and unlock
\***
\***********************************************************************
FUNCTION WRITE.LOCAL.UNLOCK PUBLIC

    INTEGER*2 WRITE.LOCAL.UNLOCK

    WRITE.LOCAL.UNLOCK = 1

    FORMAT.STRING$ = "C4,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"                  !CMW

    IF END #LOCAL.SESS.NUM% THEN WRITE.LOCAL.UNLOCK.ERROR
    WRITE FORM FORMAT.STRING$; #LOCAL.SESS.NUM% AUTOUNLOCK;             \   !CMW
        LOCAL.ITEM.CODE$,                                               \
        LOCAL.PRICE$,                                                   \
        LOCAL.START.DATE$,                                              \
        LOCAL.START.TIME$,                                              \
        LOCAL.END.DATE$,                                                \
        LOCAL.OPERATOR$,                                                \
        LOCAL.REASON$,                                                  \
        LOCAL.H.O.PRICE$,                                               \
        LOCAL.HO.CHANGE$,                                               \
        LOCAL.AUTH.NUM$,                                                \
        LOCAL.STOCK.FIG%,                                               \
        LOCAL.RETRIEVAL.FLAG$

    WRITE.LOCAL.UNLOCK = 0

    EXIT FUNCTION

WRITE.LOCAL.UNLOCK.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.CODE$       = LOCAL.ITEM.CODE$
    CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%
    
END FUNCTION

\***********************************************************************    !CMW
\***                                                                        !CMW
\***    WRITE.LOCAL.UNLOCK.HOLD                                             !CMW
\***                                                                        !CMW
\***    Write hold LOCAL file record and unlock                             !CMW
\***                                                                        !CMW
\***********************************************************************    !CMW
FUNCTION WRITE.LOCAL.UNLOCK.HOLD PUBLIC                                     !CMW
                                                                            !CMW
    INTEGER*2 WRITE.LOCAL.UNLOCK.HOLD                                       !CMW
                                                                            !CMW
    WRITE.LOCAL.UNLOCK.HOLD = 1                                             !CMW
                                                                            !CMW
    FORMAT.STRING$ = "C4,C5,C3,C2,C3,C4,C4,C5,C3,C4,I2,C1"                  !CMW
                                                                            !CMW
    IF END #LOCAL.SESS.NUM% THEN WRITE.LOCAL.UNLOCK.HOLD.ERROR              !CMW
    WRITE FORM FORMAT.STRING$; HOLD #LOCAL.SESS.NUM% AUTOUNLOCK;        \   !CMW
        LOCAL.ITEM.CODE$,                                               \   !CMW
        LOCAL.PRICE$,                                                   \   !CMW
        LOCAL.START.DATE$,                                              \   !CMW
        LOCAL.START.TIME$,                                              \   !CMW
        LOCAL.END.DATE$,                                                \   !CMW
        LOCAL.OPERATOR$,                                                \   !CMW
        LOCAL.REASON$,                                                  \   !CMW
        LOCAL.H.O.PRICE$,                                               \   !CMW
        LOCAL.HO.CHANGE$,                                               \   !CMW
        LOCAL.AUTH.NUM$,                                                \   !CMW
        LOCAL.STOCK.FIG%,                                               \   !CMW
        LOCAL.RETRIEVAL.FLAG$                                               !CMW
                                                                            !CMW
    WRITE.LOCAL.UNLOCK.HOLD = 0                                             !CMW
                                                                            !CMW
    EXIT FUNCTION                                                           !CMW
                                                                            !CMW
WRITE.LOCAL.UNLOCK.HOLD.ERROR:                                              !CMW
                                                                            !CMW
    FILE.OPERATION$     = "W"                                               !CMW
    CURRENT.CODE$       = LOCAL.ITEM.CODE$                                  !CMW
    CURRENT.REPORT.NUM% = LOCAL.REPORT.NUM%                                 !CMW
                                                                            !CMW
END FUNCTION                                                                !CMW

