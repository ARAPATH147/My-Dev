REM \
\******************************************************************************
\******************************************************************************
\***
\***                   SSPSCTRL FILE FUNCTIONS
\***
\***                  REFERENCE  : SSPSCFUN.BAS
\***
\***    Version A         Julia Stones             23rd October 2003
\***
\******************************************************************************
\*******************************************************************************

  STRING GLOBAL                                                        \
     CURRENT.CODE$,                                                    \
     FILE.OPERATION$

  INTEGER*2 GLOBAL                                                     \
     CURRENT.REPORT.NUM%


  %INCLUDE SSPSCDEC.J86


  FUNCTION SSPSCTRL.SET PUBLIC
\***************************

    SSPSCTRL.REPORT.NUM%  = 664
    SSPSCTRL.RECL%        = 80
    SSPSCTRL.FILE.NAME$ = "ADXLXACN::D:\ADX_UDT1\SSPSCTRL.BIN"

  END FUNCTION
\-------------------------------------------------------------------------------
REM EJECT

  FUNCTION READ.SSPSCTRL PUBLIC
\****************************

    STRING FORMAT$
    INTEGER*2 READ.SSPSCTRL

    READ.SSPSCTRL = 1
    FORMAT$ = "C3,C3,C3,C3,C68"

    IF END #SSPSCTRL.SESS.NUM% THEN ERROR.READ.SSPSCTRL
    READ FORM FORMAT$;                                                \
                 #SSPSCTRL.SESS.NUM%                                    \
                 ,1;                                                    \
                 SSPS.BTCSK.NUM$,                                       \
                 SSPS.BTCSF.NUM$,                                       \
                 SSPS.BTCSK.FTP$,                                       \
                 SSPS.BTCSF.FTP$,                                       \
                 SSPS.FILLER$

    READ.SSPSCTRL = 0
    EXIT FUNCTION


    ERROR.READ.SSPSCTRL:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = SSPSCTRL.REPORT.NUM%
       CURRENT.CODE$       = PACK$("0000000000000000")

       EXIT FUNCTION

  END FUNCTION
\----------------------------------------------------------------------
REM EJECT

 FUNCTION READ.SSPSCTRL.LOCKED PUBLIC
\*********************************

    STRING    FORMAT$
    INTEGER*2 READ.SSPSCTRL.LOCKED

    READ.SSPSCTRL.LOCKED = 1
    FORMAT$ = "C3,C3,C3,C3,C68"

    IF END #SSPSCTRL.SESS.NUM% THEN ERROR.READ.SSPSCTRL.LOCKED
    READ FORM FORMAT$;                                                  \
                 #SSPSCTRL.SESS.NUM%                                    \
                 AUTOLOCK,1;                                            \
                 SSPS.BTCSK.NUM$,                                       \
                 SSPS.BTCSF.NUM$,                                       \
                 SSPS.BTCSK.FTP$,                                       \
                 SSPS.BTCSF.FTP$,                                       \
                 SSPS.FILLER$

       READ.SSPSCTRL.LOCKED = 0

       EXIT FUNCTION

ERROR.READ.SSPSCTRL.LOCKED:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = SSPSCTRL.REPORT.NUM%
       CURRENT.CODE$       = PACK$("0000000000000000")

       EXIT FUNCTION

  END FUNCTION
\----------------------------------------------------------------------
REM EJECT

 FUNCTION WRITE.SSPSCTRL PUBLIC
\***************************

    STRING    FORMAT$
    INTEGER*2 WRITE.SSPSCTRL

    WRITE.SSPSCTRL = 1
    FORMAT$ = "C3,C3,C3,C3,C68"

    IF END #SSPSCTRL.SESS.NUM% THEN ERROR.WRITE.SSPSCTRL

    WRITE FORM FORMAT$;                                                 \
                 #SSPSCTRL.SESS.NUM%                                    \
                 ,1;                                                    \
                 SSPS.BTCSK.NUM$,                                       \
                 SSPS.BTCSF.NUM$,                                       \
                 SSPS.BTCSK.FTP$,                                       \
                 SSPS.BTCSF.FTP$,                                       \
                 SSPS.FILLER$

       WRITE.SSPSCTRL = 0

       EXIT FUNCTION

ERROR.WRITE.SSPSCTRL:

       FILE.OPERATION$     = "W"
       CURRENT.REPORT.NUM% = SSPSCTRL.REPORT.NUM%
       CURRENT.CODE$       = PACK$("0000000000000000")

       EXIT FUNCTION

  END FUNCTION
\----------------------------------------------------------------------
REM EJECT

 FUNCTION WRITE.SSPSCTRL.UNLOCK PUBLIC
\*************************************

    STRING    FORMAT$
    INTEGER*2 WRITE.SSPSCTRL.UNLOCK

    WRITE.SSPSCTRL.UNLOCK = 1
    FORMAT$ = "C3,C3,C3,C3,C68"

    IF END #SSPSCTRL.SESS.NUM% THEN ERROR.WRITE.SSPSCTRL.UNLOCK

    WRITE FORM FORMAT$;                                                 \
                 #SSPSCTRL.SESS.NUM%                                    \
                 AUTOUNLOCK,1;                                          \
                 SSPS.BTCSK.NUM$,                                       \
                 SSPS.BTCSF.NUM$,                                       \
                 SSPS.BTCSK.FTP$,                                       \
                 SSPS.BTCSF.FTP$,                                       \
                 SSPS.FILLER$

       WRITE.SSPSCTRL.UNLOCK = 0

       EXIT FUNCTION

ERROR.WRITE.SSPSCTRL.UNLOCK:

       FILE.OPERATION$     = "W"
       CURRENT.REPORT.NUM% = SSPSCTRL.REPORT.NUM%
       CURRENT.CODE$       = PACK$("0000000000000000")

       EXIT FUNCTION

  END FUNCTION
