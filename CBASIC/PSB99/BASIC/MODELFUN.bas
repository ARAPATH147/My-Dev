\******************************************************************************
\******************************************************************************
\***                                                                          *
\***             %INCLUDE FOR OPERATOR MODEL FILE FUNCTIONS                   *
\***                                                                          *
\***                       REFERENCE : MODELFUN                               *
\***                                                                          *
\***                       FILE TYPE : Keyed                                  *
\***                                                                          *
\***                                                                          *
\***    VERSION A.              Mark Walker                    7th March 1994 *
\***    Original version.                                                     *
\***                                                                          *
\******************************************************************************
\******************************************************************************

        %INCLUDE MODELDEC.J86

        STRING GLOBAL                   CURRENT.CODE$,                  \
                                        FILE.OPERATION$

        INTEGER*2 GLOBAL                CURRENT.REPORT.NUM%

        FUNCTION MODEL.SET PUBLIC

        INTEGER*2 MODEL.SET

        MODEL.SET = 1

        MODEL.FILE.NAME$  = "MODEL"
        MODEL.REPORT.NUM% = 386
        MODEL.RECL%       = 72

        MODEL.SET = 0

        END FUNCTION

        FUNCTION READ.MODEL PUBLIC

        INTEGER*2 READ.MODEL

        READ.MODEL = 1

        IF END # MODEL.SESS.NUM% THEN READ.ERROR
        READ FORM "T3,C70"; # MODEL.SESS.NUM%                           \
             KEY MODEL.KEY$;                                            \
             MODEL.RECORD$

        MODEL.TYPE$              = MID$(MODEL.KEY$,1,1)
        MODEL.IDENTIFIER$        = MID$(MODEL.KEY$,2,1)
        MODEL.DESCRIPTION$       = MID$(MODEL.RECORD$,1,20)
        MODEL.REPORT.DESC$       = MID$(MODEL.RECORD$,21,3)
        MODEL.DISPLAY.FLAG$      = MID$(MODEL.RECORD$,24,1)
        MODEL.EALAUTH.OPTIONSK$  = MID$(MODEL.RECORD$,25,1)
        MODEL.EALAUTH.FLAGS$     = MID$(MODEL.RECORD$,26,19)
        MODEL.ADXCSOUF.USER.ID$  = MID$(MODEL.RECORD$,45,1)
        MODEL.ADXCSOUF.GROUP.ID$ = MID$(MODEL.RECORD$,46,1)
        MODEL.ADXCSOUF.FLAGS$    = MID$(MODEL.RECORD$,47,14)
        MODEL.MODEL.NUM$         = MID$(MODEL.RECORD$,61,3)
        MODEL.SUPERVISOR.FLAG$   = MID$(MODEL.RECORD$,64,1)
        MODEL.FILLER$            = MID$(MODEL.RECORD$,65,6)

        READ.MODEL = 0

        EXIT FUNCTION

        READ.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = MODEL.REPORT.NUM%
        CURRENT.CODE$       = MODEL.KEY$

        END FUNCTION

!*********************************************************************************
 
        FUNCTION WRITE.MODEL PUBLIC

        INTEGER*2 WRITE.MODEL

        WRITE.MODEL = 1        
                                      
        MODEL.RECORD$ = MODEL.DESCRIPTION$              +  \
                        MODEL.REPORT.DESC$              +  \
                        MODEL.DISPLAY.FLAG$             +  \
                        MODEL.EALAUTH.OPTIONSK$         +  \
                        MODEL.EALAUTH.FLAGS$            +  \
                        MODEL.ADXCSOUF.USER.ID$         +  \
                        MODEL.ADXCSOUF.GROUP.ID$        +  \
                        MODEL.ADXCSOUF.FLAGS$           +  \
                        MODEL.MODEL.NUM$                +  \
                        MODEL.SUPERVISOR.FLAG$ 
                                   
        IF END # MODEL.SESS.NUM% THEN WRITE.ERROR                              
        WRITE FORM "C2 C70";                                   \
          #MODEL.SESS.NUM%;                                    \ 
             MODEL.KEY$,                                       \
             MODEL.RECORD$     
        
        WRITE.MODEL = 0  

        WRITE.ERROR:
        
        FILE.OPERATION$     = "W"
        CURRENT.REPORT.NUM% = MODEL.REPORT.NUM%
        CURRENT.CODE$       = MODEL.KEY$

        END FUNCTION
