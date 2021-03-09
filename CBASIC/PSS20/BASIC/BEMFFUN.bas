\******************************************************************************
\***
\***            %INCLUDE FOR BOOTS ERROR MESSAGE FILE FUNCTIONS
\***
\***                       REFERENCE : BEMFFNS.J86
\***
\***    Version A                 Mark Walker                 1st November 1993
\***
\***    Version B       Stuart William McConnachie          25th September 2000
\***    READ.BEMF function has got lost!
\***
\******************************************************************************

        %INCLUDE BEMFDEC.J86

        STRING GLOBAL       CURRENT.CODE$
        STRING GLOBAL       FILE.OPERATION$

        INTEGER*2 GLOBAL    CURRENT.REPORT.NUM%
        

        FUNCTION BEMF.SET PUBLIC
        
        BEMF.REPORT.NUM%  = 5
        BEMF.RECL%        = 79
        BEMF.FILE.NAME$   = "BEMF"
        
        END FUNCTION
        
        
        FUNCTION READ.BEMF PUBLIC
        
        INTEGER*1   READ.BEMF
        
        READ.BEMF = 1
        
        IF END # BEMF.SESS.NUM% THEN FILE.ERROR
        READ FORM "C79"; # BEMF.SESS.NUM%, BEMF.REC.NO%; BEMF.MESSAGE$
        
        READ.BEMF = 0
        
        EXIT FUNCTION

        
        FILE.ERROR:
        
        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = BEMF.REPORT.NUM%
        CURRENT.CODE$       = RIGHT$("00000000000000"+STR$(BEMF.REC.NO%), 14)
        CURRENT.CODE$       = PACK$(CURRENT.CODE$)
        
        END FUNCTION
        
