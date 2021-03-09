\*****************************************************************************
\*****************************************************************************
\***
\***            ADXCSOUF.DAT       FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor                     Nov 92    
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE CSOUFDEC.J86


   FUNCTION CSOUF.SET PUBLIC

REM \


    CSOUF.FILE.NAME$  EQ "ADXCSOUF" ! I.B.M. logical file name
    CSOUF.REPORT.NUM% EQ  218
    CSOUF.RECL%       EQ  34

END FUNCTION

\----------------------------------------------------------------------------


REM \


FUNCTION READ.CSOUF.ABREV PUBLIC

   INTEGER*2 READ.CSOUF.ABREV

   READ.CSOUF.ABREV = 1

   IF END#CSOUF.SESS.NUM% THEN READ.CSOUF.ABREV.ERROR

    READ FORM "C8,C1,C8,C1,C1,C1,C14";              \
        #CSOUF.SESS.NUM%,                           \
        CSOUF.REC.NUM%;                             \
            CSOUF.OP.ID$,                           \
            CSOUF.FILLER.01$,                       \
            CSOUF.PSWD$,                            \
            CSOUF.FILLER.02$,                       \
            CSOUF.USER.ID$,                         \
            CSOUF.GROUP.ID$,                        \
            CSOUF.FLAGS$

   READ.CSOUF.ABREV = 0
   EXIT FUNCTION

   READ.CSOUF.ABREV.ERROR:

   CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = " "

   EXIT FUNCTION
  END FUNCTION



FUNCTION READ.CSOUF.ABREV.LOCKED PUBLIC

   INTEGER*2 READ.CSOUF.ABREV.LOCKED

   READ.CSOUF.ABREV.LOCKED = 1

   IF END#CSOUF.SESS.NUM% THEN READ.CSOUF.ABREV.LOCKED.ERROR

    READ FORM "C8,C1,C8,C1,C1,C1,C14";              \
        #CSOUF.SESS.NUM% AUTOLOCK,                  \
        CSOUF.REC.NUM%;                             \
            CSOUF.OP.ID$,                           \
            CSOUF.FILLER.01$,                       \
            CSOUF.PSWD$,                            \
            CSOUF.FILLER.02$,                       \
            CSOUF.USER.ID$,                         \
            CSOUF.GROUP.ID$,                        \
            CSOUF.FLAGS$

   READ.CSOUF.ABREV.LOCKED = 0
   EXIT FUNCTION

   READ.CSOUF.ABREV.LOCKED.ERROR:

   CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = " "

   EXIT FUNCTION
  END FUNCTION



FUNCTION WRITE.CSOUF.ABREV.UNLOCK PUBLIC

   INTEGER*2 WRITE.CSOUF.ABREV.UNLOCK

   WRITE.CSOUF.ABREV.UNLOCK = 1

   IF END#CSOUF.SESS.NUM% THEN WRITE.CSOUF.ABREV.UNLOCK.ERROR

    WRITE FORM "C8,C1,C8,C1,C1,C1,C14";             \
        #CSOUF.SESS.NUM% AUTOUNLOCK,                \
        CSOUF.REC.NUM%;                             \
            CSOUF.OP.ID$,                           \
            CSOUF.FILLER.01$,                       \
            CSOUF.PSWD$,                            \
            CSOUF.FILLER.02$,                       \
            CSOUF.USER.ID$,                         \
            CSOUF.GROUP.ID$,                        \
            CSOUF.FLAGS$

   WRITE.CSOUF.ABREV.UNLOCK = 0
   EXIT FUNCTION

   WRITE.CSOUF.ABREV.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = " "

   EXIT FUNCTION
  END FUNCTION



FUNCTION WRITE.CSOUF.ABREV PUBLIC

   INTEGER*2 WRITE.CSOUF.ABREV

   WRITE.CSOUF.ABREV = 1

   IF END#CSOUF.SESS.NUM% THEN WRITE.CSOUF.ABREV.ERROR

    WRITE FORM "C8,C1,C8,C1,C1,C1,C14";             \
        # CSOUF.SESS.NUM%,                          \
        CSOUF.REC.NUM%;                             \
            CSOUF.OP.ID$,                           \
            CSOUF.FILLER.01$,                       \
            CSOUF.PSWD$,                            \
            CSOUF.FILLER.02$,                       \
            CSOUF.USER.ID$,                         \
            CSOUF.GROUP.ID$,                        \
            CSOUF.FLAGS$

   WRITE.CSOUF.ABREV = 0
   EXIT FUNCTION

   WRITE.CSOUF.ABREV.ERROR:

   CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = " "

   EXIT FUNCTION
  END FUNCTION


