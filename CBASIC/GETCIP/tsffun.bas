\*****************************************************************************
\*****************************************************************************
\***
\***                  TERMINAL STATUS FILE FUNCTIONS
\***
\***                      REFERENCE    : TSFFUN
\***
\***           VERSION A : STEVEN GOULDING              14.10.92
\***
\***           VERSION B : STEVE WINDSOR                15.09.93
\***           Updated to keep in step.
\***
\***           VERSION C : STUART WILLIAM MCCONNACHIE   18.03.97
\***           Added extra fields in user data.
\***          
\***           VERSION D : REBECCA DAKIN                19.02.99
\***           Added ACSAL.CUT.OFF, as part of Cash Accounting project.
\***
\***           VERSION E : AMY HOGGARD                  16.10.00
\***           Added TILLBAG.CUT.OFF, as part of ECO project.
\***
\***           VERSION F : STUART WILLIAM MCCONNACHIE   20.05.05
\***           Added "new" fields as no one else can be bothered.
\***           Corrected offset bug of controller ID.
\***
\***           VERSION G : JAMIE THORPE                 05.10.05
\***           Added new INVDUE.DATE record to the store record 
\***           (for Zero TSF proj.)
\***           This is the date that the most recent INVDUE file
\***           was created.
\***
\***    VERSION H               Mark Walker                      3rd Nov 2014
\***    F294 PCI Phase 1
\***    Includes the following changes:
\***    - Removed redundant 'smartcard software level' field.
\***    - Extended TSF.INDICAT2% integer field from 1 to 2 bytes.
\***    - Extended TSF.USER$ string field from 4 to 5 bytes.
\***    - Code formatting changes (uncommented).
\***
\*****************************************************************************
\*****************************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

%INCLUDE TSFDEC.J86                                                        !CSWM

FUNCTION TSF.SET PUBLIC

    TSF.FILE.NAME$  = "EALTERMS"
    TSF.RECL%       = 63
    TSF.REPORT.NUM% = 29

    DIM TSF.REC$(8) ! Dimension of TSF.REC array

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION READ.TSF PUBLIC

    INTEGER*1 READ.TSF

    READ.TSF = 1

    IF END #TSF.SESS.NUM% THEN READ.TSF.ERROR

    IF TSF.TERM.STORE$ = PACK$("9999") THEN BEGIN                          !CSWM

        READ FORM "T3 2I1 C8 C2 2C1 4I1 C3 C40" ;                       \   !GJT 
            #TSF.SESS.NUM% KEY TSF.TERM.STORE$;                         \  !CSWM
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.TSL.NAME$,                                          \
                TSF.MONITOR$,                                           \
                TSF.TLOGFLAG$,                                          \
                TSF.RCPSTATUS$,                                         \  !CSWM
                TSF.MTSL.CUT.OFF%,                                      \  !CSWM
                TSF.CUSTD.CUT.OFF%,                                     \  !CSWM
                TSF.ACSAL.CUT.OFF%,                                     \   !DRD
                TSF.TILLBAG.CUT.OFF%,                                   \   !EAH
                TSF.INVDUE.DATE$,                                       \   !GJT
                TSF.SPACE$

    ENDIF ELSE BEGIN                                                       !CSWM

\       READ FORM "T3 2I1 7I4 C4 C2 C1 I1 I2 C4 C2 2I4 C7" ;            \   !HMW
        READ FORM "T3 2I1 7I4 C4 C2 C1 I2 C5 C2 2I4 C7" ;               \   !HMW
            #TSF.SESS.NUM% KEY TSF.TERM.STORE$;                         \  !CSWM
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.GROSSPOS,                                           \
                TSF.GROSSNEG,                                           \
                TSF.NETCASH,                                            \
                TSF.NETNCASH,                                           \
                TSF.AMTLOAN,                                            \
                TSF.AMTPICKU,                                           \
                TSF.AMTCASHC,                                           \
                TSF.OPERATOR$,                                          \
                TSF.TRANSNUM$,                                          \
                TSF.SIGN.OFF.DELAY$,                                    \  !CSWM
                TSF.INDICAT2%,                                          \  !CSWM
\               TSF.SC.LEVEL%,                                          \   !HMW
                TSF.USER$,                                              \
                TSF.CONTROLLER$,                                        \
                TSF.NETCCURR,                                           \  !FSWM
                TSF.NETCCCURR,                                          \  !FSWM
                TSF.SPACE$

    ENDIF                                                                  !CSWM

    READ.TSF = 0
    EXIT FUNCTION

READ.TSF.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "R"
    CURRENT.CODE$ = TSF.TERM.STORE$

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION READ.TSF.LOCK PUBLIC

    INTEGER*1 READ.TSF.LOCK

    READ.TSF.LOCK = 1

    IF END #TSF.SESS.NUM% THEN READ.TSF.LOCK.ERROR

    IF TSF.TERM.STORE$ = PACK$("9999") THEN BEGIN                          !CSWM

        READ FORM "T3 2I1 C8 C2 2C1 4I1 C3 C40" ;                       \   !GJT 
            #TSF.SESS.NUM% AUTOLOCK KEY TSF.TERM.STORE$;                \  !CSWM
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.TSL.NAME$,                                          \
                TSF.MONITOR$,                                           \
                TSF.TLOGFLAG$,                                          \
                TSF.RCPSTATUS$,                                         \  !CSWM
                TSF.MTSL.CUT.OFF%,                                      \  !CSWM
                TSF.CUSTD.CUT.OFF%,                                     \  !CSWM
                TSF.ACSAL.CUT.OFF%,                                     \   !DRD
                TSF.TILLBAG.CUT.OFF%,                                   \   !EAH
                TSF.INVDUE.DATE$,                                       \   !GJT
                TSF.SPACE$

    ENDIF ELSE BEGIN                                                       !CSWM

\       READ FORM "T3 2I1 7I4 C4 C2 C1 I1 I2 C4 C2 2I4 C7" ;            \   !HMW
        READ FORM "T3 2I1 7I4 C4 C2 C1 I2 C5 C2 2I4 C7" ;               \   !HMW
            #TSF.SESS.NUM% AUTOLOCK KEY TSF.TERM.STORE$;                \  !CSWM
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.GROSSPOS,                                           \
                TSF.GROSSNEG,                                           \
                TSF.NETCASH,                                            \
                TSF.NETNCASH,                                           \
                TSF.AMTLOAN,                                            \
                TSF.AMTPICKU,                                           \
                TSF.AMTCASHC,                                           \
                TSF.OPERATOR$,                                          \
                TSF.TRANSNUM$,                                          \
                TSF.SIGN.OFF.DELAY$,                                    \  !CSWM
                TSF.INDICAT2%,                                          \  !CSWM
\               TSF.SC.LEVEL%,                                          \   !HMW
                TSF.USER$,                                              \
                TSF.CONTROLLER$,                                        \
                TSF.NETCCURR,                                           \  !FSWM
                TSF.NETCCCURR,                                          \  !FSWM
                TSF.SPACE$

    ENDIF                                                                  !CSWM

    READ.TSF.LOCK = 0
    EXIT FUNCTION

READ.TSF.LOCK.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "R"
    CURRENT.CODE$ = TSF.TERM.STORE$

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION WRITE.TSF PUBLIC

    INTEGER*1 WRITE.TSF

    WRITE.TSF = 1

    IF END #TSF.SESS.NUM% THEN WRITE.TSF.ERROR

    IF TSF.TERM.STORE$ = PACK$("9999") THEN BEGIN                          !CSWM

        WRITE FORM "C2 2I1 C8 C2 2C1 4I1 C3 C40" ;                      \   !GJT 
            #TSF.SESS.NUM%;                                             \  !CSWM
                TSF.TERM.STORE$,                                        \
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.TSL.NAME$,                                          \
                TSF.MONITOR$,                                           \
                TSF.TLOGFLAG$,                                          \
                TSF.RCPSTATUS$,                                         \  !CSWM
                TSF.MTSL.CUT.OFF%,                                      \  !CSWM
                TSF.CUSTD.CUT.OFF%,                                     \  !CSWM
                TSF.ACSAL.CUT.OFF%,                                     \   !DRD
                TSF.TILLBAG.CUT.OFF%,                                   \   !EAH
                TSF.INVDUE.DATE$,                                       \   !GJT
                TSF.SPACE$

    ENDIF ELSE BEGIN                                                       !CSWM

\       WRITE FORM "C2 2I1 7I4 C4 C2 C1 I1 I2 C4 C2 2I4 C7" ;           \   !HMW
        WRITE FORM "C2 2I1 7I4 C4 C2 C1 I2 C5 C2 2I4 C7" ;              \   !HMW
            #TSF.SESS.NUM%;                                             \  !CSWM
                TSF.TERM.STORE$,                                        \
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.GROSSPOS,                                           \
                TSF.GROSSNEG,                                           \
                TSF.NETCASH,                                            \
                TSF.NETNCASH,                                           \
                TSF.AMTLOAN,                                            \
                TSF.AMTPICKU,                                           \
                TSF.AMTCASHC,                                           \
                TSF.OPERATOR$,                                          \
                TSF.TRANSNUM$,                                          \
                TSF.SIGN.OFF.DELAY$,                                    \  !CSWM
                TSF.INDICAT2%,                                          \  !CSWM
\               TSF.SC.LEVEL%,                                          \   !HMW
                TSF.USER$,                                              \
                TSF.CONTROLLER$,                                        \
                TSF.NETCCURR,                                           \  !FSWM
                TSF.NETCCCURR,                                          \  !FSWM
                TSF.SPACE$

    ENDIF                                                                  !CSWM

    WRITE.TSF = 0
    EXIT FUNCTION

WRITE.TSF.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "W"
    CURRENT.CODE$ = TSF.TERM.STORE$

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION WRITE.TSF.UNLOCK PUBLIC

    INTEGER*1 WRITE.TSF.UNLOCK

    WRITE.TSF.UNLOCK = 1

    IF END #TSF.SESS.NUM% THEN WRITE.TSF.UNLOCK.ERROR

    IF TSF.TERM.STORE$ = PACK$("9999") THEN BEGIN                          !CSWM

        WRITE FORM "C2 2I1 C8 C2 2C1 4I1 C3 C40" ;                      \   !GJT  
            #TSF.SESS.NUM% AUTOUNLOCK;                                  \  !CSWM
                 TSF.TERM.STORE$,                                       \
                 TSF.INDICAT0%,                                         \
                 TSF.INDICAT1%,                                         \
                 TSF.TSL.NAME$,                                         \
                 TSF.MONITOR$,                                          \
                 TSF.TLOGFLAG$,                                         \
                 TSF.RCPSTATUS$,                                        \  !CSWM
                 TSF.MTSL.CUT.OFF%,                                     \  !CSWM
                 TSF.CUSTD.CUT.OFF%,                                    \  !CSWM
                 TSF.ACSAL.CUT.OFF%,                                    \   !DRD
                 TSF.TILLBAG.CUT.OFF%,                                  \   !EAH
                 TSF.INVDUE.DATE$,                                      \   !GJT
                 TSF.SPACE$

    ENDIF ELSE BEGIN                                                !CSWM

\       WRITE FORM "C2 2I1 7I4 C4 C2 C1 I1 I2 C4 C2 2I4 C7" ;           \   !HMW
        WRITE FORM "C2 2I1 7I4 C4 C2 C1 I2 C5 C2 2I4 C7" ;              \   !HMW
            #TSF.SESS.NUM% AUTOUNLOCK;                                  \  !CSWM
                TSF.TERM.STORE$,                                        \
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.GROSSPOS,                                           \
                TSF.GROSSNEG,                                           \
                TSF.NETCASH,                                            \
                TSF.NETNCASH,                                           \
                TSF.AMTLOAN,                                            \
                TSF.AMTPICKU,                                           \
                TSF.AMTCASHC,                                           \
                TSF.OPERATOR$,                                          \
                TSF.TRANSNUM$,                                          \
                TSF.SIGN.OFF.DELAY$,                                    \  !CSWM
                TSF.INDICAT2%,                                          \  !CSWM
\               TSF.SC.LEVEL%,                                          \   !HMW
                TSF.USER$,                                              \
                TSF.CONTROLLER$,                                        \
                TSF.NETCCURR,                                           \  !FSWM
                TSF.NETCCCURR,                                          \  !FSWM
                TSF.SPACE$

    ENDIF                                                                  !CSWM

    WRITE.TSF.UNLOCK = 0
    EXIT FUNCTION

WRITE.TSF.UNLOCK.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "W"
    CURRENT.CODE$ = TSF.TERM.STORE$
    
END FUNCTION

\----------------------------------------------------------------------------

FUNCTION WRITE.TSF.UNLOCK.HOLD PUBLIC

    INTEGER*1 WRITE.TSF.UNLOCK.HOLD

    WRITE.TSF.UNLOCK.HOLD = 1

    IF END #TSF.SESS.NUM% THEN WRITE.TSF.UNLOCK.HOLD.ERROR

    IF TSF.TERM.STORE$ EQ PACK$("9999") THEN BEGIN                         !CSWM

        WRITE FORM "C2 2I1 C8 C2 2C1 4I1 C3 C40" ; HOLD                 \   !GJT 
            #TSF.SESS.NUM% AUTOUNLOCK;                                  \  !CSWM
                 TSF.TERM.STORE$,                                       \
                 TSF.INDICAT0%,                                         \
                 TSF.INDICAT1%,                                         \
                 TSF.TSL.NAME$,                                         \
                 TSF.MONITOR$,                                          \
                 TSF.TLOGFLAG$,                                         \
                 TSF.RCPSTATUS$,                                        \  !CSWM
                 TSF.MTSL.CUT.OFF%,                                     \  !CSWM
                 TSF.CUSTD.CUT.OFF%,                                    \  !CSWM
                 TSF.ACSAL.CUT.OFF%,                                    \   !DRD
                 TSF.TILLBAG.CUT.OFF%,                                  \   !EAH 
                 TSF.INVDUE.DATE$,                                      \   !GJT
                 TSF.SPACE$

    ENDIF ELSE BEGIN                                                       !CSWM

\       WRITE FORM "C2 2I1 7I4 C4 C2 C1 I1 I2 C4 C2 2I4 C7" ; HOLD      \   !HMW
        WRITE FORM "C2 2I1 7I4 C4 C2 C1 I2 C5 C2 2I4 C7" ;              \   !HMW
            #TSF.SESS.NUM% AUTOUNLOCK;                                  \  !CSWM
                TSF.TERM.STORE$,                                        \
                TSF.INDICAT0%,                                          \
                TSF.INDICAT1%,                                          \
                TSF.GROSSPOS,                                           \
                TSF.GROSSNEG,                                           \
                TSF.NETCASH,                                            \
                TSF.NETNCASH,                                           \
                TSF.AMTLOAN,                                            \
                TSF.AMTPICKU,                                           \
                TSF.AMTCASHC,                                           \
                TSF.OPERATOR$,                                          \
                TSF.TRANSNUM$,                                          \
                TSF.SIGN.OFF.DELAY$,                                    \  !CSWM
                TSF.INDICAT2%,                                          \  !CSWM
\               TSF.SC.LEVEL%,                                          \   !HMW
                TSF.USER$,                                              \
                TSF.CONTROLLER$,                                        \
                TSF.NETCCURR,                                           \  !FSWM
                TSF.NETCCCURR,                                          \  !FSWM
                TSF.SPACE$

    ENDIF                                                                  !CSWM

    WRITE.TSF.UNLOCK.HOLD = 0
    EXIT FUNCTION

WRITE.TSF.UNLOCK.HOLD.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "W"
    CURRENT.CODE$ = TSF.TERM.STORE$

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION READ.TSF.SECTOR PUBLIC

    INTEGER*1 READ.TSF.SECTOR

    READ.TSF.SECTOR = 1

    IF END #TSF.SESS.NUM% THEN READ.TSF.SECTOR.ERROR

    READ FORM "C4,8C63,C4";                                             \
        #TSF.SESS.NUM%,TSF.SECTOR.NUM%;                                 \
            TSF.SECTOR.INFO$,                                           \
            TSF.REC$(1),                                                \
            TSF.REC$(2),                                                \
            TSF.REC$(3),                                                \
            TSF.REC$(4),                                                \
            TSF.REC$(5),                                                \
            TSF.REC$(6),                                                \
            TSF.REC$(7),                                                \
            TSF.REC$(8),                                                \
            TSF.SECTOR.FILLER$

    READ.TSF.SECTOR = 0
    EXIT FUNCTION

READ.TSF.SECTOR.ERROR:

    CURRENT.REPORT.NUM% = TSF.REPORT.NUM%
    FILE.OPERATION$ = "R"
    CURRENT.CODE$ = TSF.SECTOR.INFO$

END FUNCTION

