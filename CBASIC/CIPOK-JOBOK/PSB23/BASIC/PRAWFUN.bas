\********************************************************************
\***      Price Change Report Raw data file
\***      Version A           Neil Bennett          6th June 2006
\***
\***      Version B           Jamie Thorpe           23rd October 2006
\***      Added PRAW.SEL.LABEL.TYPE$
\....................................................................

INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

STRING GLOBAL CURRENT.CODE$
STRING GLOBAL FILE.OPERATION$


%INCLUDE PRAWDEC.J86

\--------------------------------------------------------------------

SUB PRAW.SPLIT.RECORD PUBLIC

    PRAW.BOOTS.CODE$   = MID$(PRAW.RECORD$, 1, 7)
    PRAW.ITEM.DESC$    = MID$(PRAW.RECORD$, 8, 24)
    PRAW.NEW.PRICE$    = MID$(PRAW.RECORD$, 32, 8)
    PRAW.OLD.PRICE$    = MID$(PRAW.RECORD$, 40, 8)
    PRAW.RPD.NUM$      = MID$(PRAW.RECORD$, 48, 6)
    PRAW.INC.DEC.FLAG$ = MID$(PRAW.RECORD$, 54, 1)
    PRAW.BUSINESS.CEN$ = MID$(PRAW.RECORD$, 55, 1)
    PRAW.PRODUCT.GRP$  = MID$(PRAW.RECORD$, 56, 5)
    PRAW.SEL.LABEL.TYPE$ = MID$(PRAW.RECORD$, 61, 1)

END SUB

\--------------------------------------------------------------------

SUB PRAW.CONCAT.RECORD PUBLIC

    PRAW.RECORD$ = PRAW.BOOTS.CODE$ + \
                   PRAW.ITEM.DESC$ + \
                   PRAW.NEW.PRICE$ + \
                   PRAW.OLD.PRICE$ + \
                   PRAW.RPD.NUM$ + \
                   PRAW.INC.DEC.FLAG$ + \
                   PRAW.BUSINESS.CEN$ + \
                   PRAW.PRODUCT.GRP$ + \
                   PRAW.SEL.LABEL.TYPE$

END SUB

\--------------------------------------------------------------------

FUNCTION PRAW.SET PUBLIC

    PRAW.REPORT.NUM%   = 728
    PRAW.FILE.NAME$    = "PCTFRAW"
    
END FUNCTION

\--------------------------------------------------------------------

FUNCTION READ.PRAW PUBLIC

    INTEGER*2 READ.PRAW

    READ.PRAW = 1

    IF END #PRAW.SESS.NUM% THEN FILE.ERROR

    READ #PRAW.SESS.NUM%; LINE PRAW.RECORD$
    CALL PRAW.SPLIT.RECORD
    
    READ.PRAW = 0

EXIT FUNCTION

FILE.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = PRAW.REPORT.NUM%
    CURRENT.CODE$ = ""
    
END FUNCTION

\--------------------------------------------------------------------

FUNCTION WRITE.PRAW PUBLIC

    INTEGER*2 WRITE.PRAW

    WRITE.PRAW = 1

    IF END #PRAW.SESS.NUM% THEN FILE.ERROR

    CALL PRAW.CONCAT.RECORD
    PRINT USING "&"; #PRAW.SESS.NUM%; PRAW.RECORD$
    
    WRITE.PRAW = 0

EXIT FUNCTION

FILE.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = PRAW.REPORT.NUM%
    CURRENT.CODE$ = PRAW.RECORD$

END FUNCTION

\--------------------------------------------------------------------

