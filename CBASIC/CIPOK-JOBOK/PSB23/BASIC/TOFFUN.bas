\******************************************************************************
\***
\***        %INCLUDE FOR TERMINAL OPTIONS FILE - FUNCTION DEFINITIONS
\***
\***                         FILE TYPE : Direct
\***
\***                         REFERENCE : TOFFUN
\***
\***    Version A               Mark Walker                  1st November 1993
\***
\***    Version B        Stuart William McConnachie            9th August 1995
\***    Added functions to read and write the terminal options file.
\***
\***
\***    Version C           Lee J Rockach                 1st October 1997
\***
\***    Added functions: READ.TRMOP.SEQ,
\***                     READ.TRMOP.DATA.CRLF,
\***                     READ.TRMOP.DATA.CRLF.LOCK,
\***                     WRITE.TRMOP.DATA.CRLF.UNLOCK.HOLD. 
\***
\***   Version D           Julia Stones                   23rd September 1998
\***
\***   Added function:  WRITE.TRMOP.DATA.CRLF.UNLOCK.
\***
\******************************************************************************

%INCLUDE TOFDEC.J86

    STRING GLOBAL FILE.OPERATION$,                                     \
                  CURRENT.CODE$

    INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

\------------------------------------------------------------------------------

    FUNCTION TOF.SET PUBLIC

    TOF.REPORT.NUM%  = 34
    TOF.RECL%        = 512
    TOF.FILE.NAME$   = "EALTO000"

    TRMOP.REPORT.NUM% = 37
    TRMOP.RECL%       = 102
    TRMOP.FILE.NAME$  = "EALTO:"

    END FUNCTION

\------------------------------------------------------------------------------

    FUNCTION READ.TRMOP PUBLIC

    INTEGER*2 I%, READ.TRMOP

    READ.TRMOP = 1

    IF END #TRMOP.SESS.NUM% THEN END.OF.READ.TRMOP
    READ FORM "T1,C100,C2"; #TRMOP.SESS.NUM%, TRMOP.REC.NUM%;          \
                             TRMOP.REC.DATA$, TRMOP.REC.CRLF$

    I% = 100
    WHILE MID$(TRMOP.REC.DATA$,I%,1) = " "      \
    AND  I% > 1
        I% = I% - 1
    WEND
    TRMOP.REC.DATA$ = LEFT$(TRMOP.REC.DATA$,I%)

    READ.TRMOP = 0
    EXIT FUNCTION


    END.OF.READ.TRMOP:

    FILE.OPERATION$     = "R"
    CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                 STR$(TRMOP.REC.NUM%)), 8)
    CURRENT.REPORT.NUM% = TRMOP.REPORT.NUM%

    EXIT FUNCTION

    END FUNCTION


\------------------------------------------------------------------------------

    FUNCTION READ.TRMOP.SEQ PUBLIC                                 ! CLJR

         INTEGER*2 READ.TRMOP.SEQ                                  ! CLJR

         READ.TRMOP.SEQ = 1                                        ! CLJR

         IF END #TRMOP.SESS.NUM% THEN END.OF.READ.TRMOP.SEQ        ! CLJR
      
         READ # TRMOP.SESS.NUM%;TRMOP.ESE.FLAG%,TRMOP.ASO.FLAG%    ! CLJR
    
         READ.TRMOP.SEQ = 0                                        ! CLJR

    EXIT FUNCTION                                                  ! CLJR
    
    END.OF.READ.TRMOP.SEQ:                                         ! CLJR
    
    END FUNCTION                                                   ! CLJR

\------------------------------------------------------------------------------

    FUNCTION READ.TRMOP.DATA.CRLF PUBLIC                           ! CLJR

            INTEGER*2 READ.TRMOP.DATA.CRLF                         ! CLJR

            READ.TRMOP.DATA.CRLF = 1                               ! CLJR

            IF END #TRMOP.SESS.NUM% THEN END.READ.TRMOP.DATA.CRLF  ! CLJR

            READ FORM "C100,C2";# TRMOP.SESS.NUM%, TRMOP.REC.NUM%; \ CLJR
                                  TRMOP.REC.DATA$, TRMOP.REC.CRLF$ ! CLJR

            READ.TRMOP.DATA.CRLF = 0                               ! CLJR

    EXIT FUNCTION                                                  ! CLJR

    END.READ.TRMOP.DATA.CRLF:                                      ! CLJR

    END FUNCTION                                                   ! CLJR


\-----------------------------------------------------------------------------------

    FUNCTION READ.TRMOP.DATA.CRLF.LOCK PUBLIC                               ! CLJR

            INTEGER*2 READ.TRMOP.DATA.CRLF.LOCK                             ! CLJR

            READ.TRMOP.DATA.CRLF.LOCK = 1                                   ! CLJR

            IF END #TRMOP.SESS.NUM% THEN END.READ.TRMOP.DATA.CRLF.LOCK      ! CLJR
    
            READ FORM "C100,C2";# TRMOP.SESS.NUM% AUTOLOCK, TRMOP.REC.NUM%; \ CLJR
                                  TRMOP.REC.DATA$, TRMOP.REC.CRLF$          ! CLJR
                                                                           
            READ.TRMOP.DATA.CRLF.LOCK = 0                                   ! CLJR
                                                                           
    EXIT FUNCTION                                                           ! CLJR
                                                                            
    END.READ.TRMOP.DATA.CRLF.LOCK:                                          ! CLJR
                                                                           
    END FUNCTION                                                            ! CLJR


\------------------------------------------------------------------------------

    FUNCTION WRITE.TRMOP PUBLIC

    INTEGER*2 WRITE.TRMOP

    WRITE.TRMOP = 1

    IF END #TRMOP.SESS.NUM% THEN END.OF.WRITE.TRMOP
    WRITE FORM "C100,C2"; #TRMOP.SESS.NUM%, TRMOP.REC.NUM%;           \
                            TRMOP.REC.DATA$, TRMOP.REC.CRLF$

    WRITE.TRMOP = 0
    EXIT FUNCTION


    END.OF.WRITE.TRMOP:

    FILE.OPERATION$     = "W"
    CURRENT.CODE$       = RIGHT$(PACK$("0000000000000000" +        \
                                 STR$(TRMOP.REC.NUM%)), 8)
    CURRENT.REPORT.NUM% = TRMOP.REPORT.NUM%

    EXIT FUNCTION


    END FUNCTION

\------------------------------------------------------------------------------

    FUNCTION WRITE.TRMOP.DATA.CRLF.UNLOCK.HOLD PUBLIC                     ! CLJR

    INTEGER*2 WRITE.TRMOP.DATA.CRLF.UNLOCK.HOLD                           ! CLJR
            
            WRITE.TRMOP.DATA.CRLF.UNLOCK.HOLD = 1                         ! CLJR

            IF END #TRMOP.SESS.NUM% THEN END.WRITE.TRMOP.CRLF.UNLOCK.HOLD ! CLJR

            WRITE FORM "C100,C2"; HOLD # TRMOP.SESS.NUM% AUTOUNLOCK,      \ CLJR
                    TRMOP.REC.NUM%;TRMOP.REC.DATA$, TRMOP.REC.CRLF$       ! CLJR

            WRITE.TRMOP.DATA.CRLF.UNLOCK.HOLD = 0                         ! CLJR

    EXIT FUNCTION                                                         ! CLJR

    END.WRITE.TRMOP.CRLF.UNLOCK.HOLD:                                     ! CLJR
    
    END FUNCTION                                                          ! CLJR

\------------------------------------------------------------------------------

    FUNCTION WRITE.TRMOP.DATA.CRLF.UNLOCK PUBLIC                          ! DJAS

    INTEGER*2 WRITE.TRMOP.DATA.CRLF.UNLOCK                                ! DJAS
                                                                        
            WRITE.TRMOP.DATA.CRLF.UNLOCK = 1                              ! DJAS

            IF END #TRMOP.SESS.NUM% THEN END.WRITE.TRMOP.CRLF.UNLOCK      ! DJAS

            WRITE FORM "C100,C2"; # TRMOP.SESS.NUM% AUTOUNLOCK,           \ DJAS
                    TRMOP.REC.NUM%;TRMOP.REC.DATA$, TRMOP.REC.CRLF$       ! DJAS

            WRITE.TRMOP.DATA.CRLF.UNLOCK = 0                              ! DJAS

    EXIT FUNCTION                                                         ! DJAS

    END.WRITE.TRMOP.CRLF.UNLOCK:                                          ! DJAS
    
    END FUNCTION                                                          ! DJAS

\------------------------------------------------------------------------------

