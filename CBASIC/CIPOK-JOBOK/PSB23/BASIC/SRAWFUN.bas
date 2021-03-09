\********************************************************************
\***      Space and Range Planogram database (SRMOD)
\***      Version A           Neil Bennett          6th June 2006
\***
\....................................................................

INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

STRING GLOBAL CURRENT.CODE$
STRING GLOBAL FILE.OPERATION$


%INCLUDE SRAWDEC.J86

\--------------------------------------------------------------------

SUB SRAW.SPLIT.RECORD PUBLIC

    SRAW.LABEL.TYPE$ = MID$(SRAW.RECORD$, 1, 1)
    SRAW.ITEM.CODE$  = MID$(SRAW.RECORD$, 2, 7)
    SRAW.ITEM.PRICE$ = MID$(SRAW.RECORD$, 9, 8)

END SUB

\--------------------------------------------------------------------

SUB SRAW.CONCAT.RECORD PUBLIC

    SRAW.RECORD$ = LEFT$(SRAW.LABEL.TYPE$+"?",1) + \
                   RIGHT$("0000000"+SRAW.ITEM.CODE$,7) + \
                   RIGHT$("00000000"+SRAW.ITEM.PRICE$,8)

END SUB

\--------------------------------------------------------------------

FUNCTION SRAW.SET PUBLIC

    SRAW.REPORT.NUM%   = 728
    SRAW.FILE.NAME$    = "SRAW:"
    
END FUNCTION

\--------------------------------------------------------------------

FUNCTION READ.SRAW PUBLIC

    INTEGER*2 READ.SRAW

    READ.SRAW = 1

    IF END #SRAW.SESS.NUM% THEN FILE.ERROR

    READ #SRAW.SESS.NUM%; LINE SRAW.RECORD$
    CALL SRAW.SPLIT.RECORD
    
    READ.SRAW = 0

EXIT FUNCTION

FILE.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = SRAW.REPORT.NUM%
    CURRENT.CODE$ = ""
    
END FUNCTION

\--------------------------------------------------------------------

FUNCTION WRITE.SRAW PUBLIC

    INTEGER*2 WRITE.SRAW

    WRITE.SRAW = 1

    IF END #SRAW.SESS.NUM% THEN FILE.ERROR

    CALL SRAW.CONCAT.RECORD
    PRINT USING "&"; #SRAW.SESS.NUM%; SRAW.RECORD$
    
    WRITE.SRAW = 0

EXIT FUNCTION

FILE.ERROR:

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = SRAW.REPORT.NUM%
    CURRENT.CODE$ = SRAW.RECORD$

END FUNCTION

\--------------------------------------------------------------------

