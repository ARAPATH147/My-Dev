\******************************************************************************
\******************************************************************************
\***                                                                          *
\***         %INCLUDE FOR OPERATOR AUTHORISATION AUDIT FILE FUNCTIONS         *
\***                                                                          *
\***                       REFERENCE : OPAUDFUN                               *
\***                                                                          *
\***                       FILE TYPE : Direct                                 *
\***                                                                          *
\***                                                                          *
\***    VERSION A.              Mark Walker                   22nd March 1994 *
\***    Original version.                                                     *
\***                                                                          *
\******************************************************************************
\******************************************************************************

        %INCLUDE OPAUDDEC.J86       \ BAC Changed version letter

        STRING GLOBAL                   CURRENT.CODE$,                 \
                                        FILE.OPERATION$

        INTEGER*2 GLOBAL                CURRENT.REPORT.NUM%

        FUNCTION OPAUD.SET PUBLIC

        INTEGER*2 OPAUD.SET

        OPAUD.SET = 1

        OPAUD.FILE.NAME$  = "OPAUD"
        OPAUD.REPORT.NUM% = 387
        OPAUD.RECL%       = 80

        OPAUD.SET = 0

        END FUNCTION

        FUNCTION READ.OPAUD PUBLIC

        INTEGER*2 READ.OPAUD

        READ.OPAUD = 1

        IF END # OPAUD.SESS.NUM% THEN READ.ERROR
        READ FORM "C80"; # OPAUD.SESS.NUM%,                            \
             OPAUD.REC.NUM%;                                           \
             OPAUD.RECORD$

        IF OPAUD.REC.NUM% = 1 THEN                                     \
        BEGIN
           OPAUD.LAST.REC.UPDATED$ = MID$(OPAUD.RECORD$,1,4)
           OPAUD.FILE.SIZE$        = MID$(OPAUD.RECORD$,5,4)
           OPAUD.FILLER$           = MID$(OPAUD.RECORD$,9,70)
           OPAUD.CRLF$             = MID$(OPAUD.RECORD$,79,2)
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPAUD.CURRENT.ID$ = MID$(OPAUD.RECORD$,1,3)
           OPAUD.OPTION$     = MID$(OPAUD.RECORD$,4,1)
           OPAUD.DATE$       = MID$(OPAUD.RECORD$,5,6)
           OPAUD.TIME$       = MID$(OPAUD.RECORD$,11,4)
           OPAUD.CHANGED.ID$ = MID$(OPAUD.RECORD$,15,3)
           OPAUD.DETAILS.1$  = MID$(OPAUD.RECORD$,18,27)
           OPAUD.DETAILS.2$  = MID$(OPAUD.RECORD$,45,27)
           OPAUD.FILLER$     = MID$(OPAUD.RECORD$,72,7)
           OPAUD.CRLF$       = MID$(OPAUD.RECORD$,79,2)
        ENDIF   
        
        READ.OPAUD = 0

        EXIT FUNCTION

        READ.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = OPAUD.REPORT.NUM%
        CURRENT.CODE$       = STR$(OPAUD.REC.NUM%)

        END FUNCTION 

        FUNCTION WRITE.OPAUD PUBLIC

        INTEGER*2 WRITE.OPAUD

        WRITE.OPAUD = 1

        IF OPAUD.REC.NUM% = 1 THEN                                     \
        BEGIN
           OPAUD.RECORD$ = OPAUD.LAST.REC.UPDATED$ +                   \
                           OPAUD.FILE.SIZE$ +                          \
                           OPAUD.FILLER$ +                             \
                           OPAUD.CRLF$
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN   
           OPAUD.RECORD$ = OPAUD.CURRENT.ID$ +                         \
                           OPAUD.OPTION$ +                             \
                           OPAUD.DATE$ +                               \
                           OPAUD.TIME$ +                               \
                           OPAUD.CHANGED.ID$ +                         \
                           OPAUD.DETAILS.1$ +                          \
                           OPAUD.DETAILS.2$ +                          \
                           OPAUD.FILLER$ +                             \
                           OPAUD.CRLF$
        ENDIF

        IF END # OPAUD.SESS.NUM% THEN WRITE.ERROR
        WRITE FORM "C80"; # OPAUD.SESS.NUM%,                           \
             OPAUD.REC.NUM%;                                           \
             OPAUD.RECORD$

        WRITE.OPAUD = 0

        EXIT FUNCTION

        WRITE.ERROR:

        FILE.OPERATION$     = "W"
        CURRENT.REPORT.NUM% = OPAUD.REPORT.NUM%
        CURRENT.CODE$       = STR$(OPAUD.REC.NUM%)

        END FUNCTION 
