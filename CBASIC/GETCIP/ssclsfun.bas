\*****************************************************************************
\*****************************************************************************
\***
\***                 SET STORE CLOSE FILE FUNCTIONS 
\***
\***                      REFERENCE    : SSCLSFUN
\***
\***           VERSION A : STEVEN GOULDING  14.10.92
\***
\***           VERSION B : ANDREW PATON     MAY 2015
\***           FOD-F392(Retail Stock 5)
\***           Addition of new field, SSCLS.STORE.CLOSED$ to both Read
\***            and Write functions
\***           Added SSCLS.REC.NO% to the read function. 
\***           Code lined up according to the coding standards.
\***
\*****************************************************************************
\*****************************************************************************


INTEGER*2 GLOBAL                  \
    CURRENT.REPORT.NUM%

STRING GLOBAL                     \
    CURRENT.CODE$,                \
    FILE.OPERATION$

%INCLUDE SSCLSDEC.J86


FUNCTION SSCLS.SET PUBLIC
REM \

    SSCLS.REPORT.NUM% = 220                                   
    SSCLS.FILE.NAME$ = "SSCLS"
    
END FUNCTION

\----------------------------------------------------------------------------

REM \

FUNCTION WRITE.SSCLS PUBLIC

    INTEGER*1 WRITE.SSCLS

    WRITE.SSCLS = 1

    SSCLS.REC.NO% = 1
    IF END#SSCLS.SESS.NUM% THEN WRITE.SSCLS.ERROR
    WRITE FORM "3C1,C13"; #SSCLS.SESS.NUM%, SSCLS.REC.NO%;             \BAP
        SSCLS.FLAG$,                                                   \
        SSCLS.KEYMODE$,                                                \                     A6C PAB
        SSCLS.STORE.CLOSED$,       \Store Closed indicator             \BAP
        SSCLS.FILLER$
                    
    WRITE.SSCLS = 0
    EXIT FUNCTION

WRITE.SSCLS.ERROR:

    CURRENT.REPORT.NUM% = SSCLS.REPORT.NUM%
    FILE.OPERATION$ = "W"
    CURRENT.CODE$ = ""

    EXIT FUNCTION
    
END FUNCTION

 
FUNCTION READ.SSCLS PUBLIC

    INTEGER*1 READ.SSCLS

    READ.SSCLS = 1

    IF END#SSCLS.SESS.NUM% THEN READ.SSCLS.ERROR

    READ FORM "3C1,C13"; #SSCLS.SESS.NUM%, SSCLS.REC.NO%;              \BAP
        SSCLS.FLAG$,                                                   \
        SSCLS.KEYMODE$,                                                \               A6C PAB
        SSCLS.STORE.CLOSED$,       \Store Closed indicator             \BAP
        SSCLS.FILLER$

    READ.SSCLS = 0
    EXIT FUNCTION

READ.SSCLS.ERROR:

    CURRENT.REPORT.NUM% = SSCLS.REPORT.NUM%
    FILE.OPERATION$ = "R"
    CURRENT.CODE$ = ""   

    EXIT FUNCTION
    
END FUNCTION




