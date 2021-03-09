\*****************************************************************************
\*****************************************************************************
\***
\***            TERMINAL MAINTENANCE CONTROL FILE FUNCTIONS 
\***
\***      Version A           Steve Windsor              5th Feb 1993
\***
\***    Revision 1.3            ROBERT COWEY                      19 JUN 2002
\***    Defined new function WRITE.TMCF.HEADER.
\***
\*****************************************************************************
\*****************************************************************************

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE TMCFDEC.J86


   FUNCTION TMCF.SET PUBLIC
REM \

    TMCF.REPORT.NUM% = 66
    TMCF.RECL%      = 51
    TMCF.FILE.NAME$ = "EALTMCFR"
END FUNCTION

\----------------------------------------------------------------------------


  FUNCTION READ.TMCF.DATA.RECORD PUBLIC

   INTEGER*2 READ.TMCF.DATA.RECORD

   READ.TMCF.DATA.RECORD = 1

   IF END#TMCF.SESS.NUM% THEN READ.TMCF.DATA.RECORD.ERROR

    READ FORM "I1,C50"; #TMCF.SESS.NUM% AUTOLOCK, TMCF.REC.NO%;   \ 
             TMCF.ACTION%,                                        \
             TMCF.MNTDATA$                                         

   READ.TMCF.DATA.RECORD = 0
   EXIT FUNCTION

   READ.TMCF.DATA.RECORD.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION



  FUNCTION READ.TMCF.HEADER.LOCKED PUBLIC

   INTEGER*2 READ.TMCF.HEADER.LOCKED

   READ.TMCF.HEADER.LOCKED = 1

   IF END#TMCF.SESS.NUM% THEN READ.TMCF.HEADER.LOCKED.ERROR

    READ FORM "I4,I1,C11,I4,C2,I2,I1,I4,I1,I1,C20";               \
              #TMCF.SESS.NUM% AUTOLOCK, TMCF.REC.NO%;             \
             TMCF.MAINTLVL%,                                      \
             TMCF.ACTION%,                                        \
             TMCF.RCDKEY$,                                        \
             TMCF.NUMRECS%,                                       \
             TMCF.TERMLOAD$,                                      \
             TMCF.FILELVL%,                                       \
             TMCF.TERMILU%,                                       \
             TMCF.NUMITEMS%,                                      \
             TMCF.DESCTYPE%,                                      \
             TMCF.USERDATA%,                                      \
             TMCF.SPACE$ 

   READ.TMCF.HEADER.LOCKED = 0
   EXIT FUNCTION

   READ.TMCF.HEADER.LOCKED.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION READ.TMCF.HEADER PUBLIC

   INTEGER*2 READ.TMCF.HEADER

   READ.TMCF.HEADER = 1

   IF END#TMCF.SESS.NUM% THEN READ.TMCF.HEADER.ERROR

    READ FORM "I4,I1,C11,I4,C2,I2,I1,I4,I1,I1,C20";               \
              #TMCF.SESS.NUM%, TMCF.REC.NO%;                      \
             TMCF.MAINTLVL%,                                      \
             TMCF.ACTION%,                                        \
             TMCF.RCDKEY$,                                        \
             TMCF.NUMRECS%,                                       \
             TMCF.TERMLOAD$,                                      \
             TMCF.FILELVL%,                                       \
             TMCF.TERMILU%,                                       \
             TMCF.NUMITEMS%,                                      \
             TMCF.DESCTYPE%,                                      \
             TMCF.USERDATA%,                                      \
             TMCF.SPACE$ 

   READ.TMCF.HEADER = 0
   EXIT FUNCTION

   READ.TMCF.HEADER.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.TMCF.HEADER PUBLIC ! 1.1 RC New function

   INTEGER*2 WRITE.TMCF.HEADER

   WRITE.TMCF.HEADER = 1

   IF END # TMCF.SESS.NUM% THEN WRITE.TMCF.HEADER.ERROR

    WRITE FORM "I4,I1,C11,I4,C2,I2,I1,I4,I1,I1,C20";               \
              # TMCF.SESS.NUM%, TMCF.REC.NO%;                      \
             TMCF.MAINTLVL%,                                      \
             TMCF.ACTION%,                                        \
             TMCF.RCDKEY$,                                        \
             TMCF.NUMRECS%,                                       \
             TMCF.TERMLOAD$,                                      \
             TMCF.FILELVL%,                                       \
             TMCF.TERMILU%,                                       \
             TMCF.NUMITEMS%,                                      \
             TMCF.DESCTYPE%,                                      \
             TMCF.USERDATA%,                                      \
             TMCF.SPACE$ 

   WRITE.TMCF.HEADER = 0
   EXIT FUNCTION

   WRITE.TMCF.HEADER.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.TMCF.HEADER.HOLD.UNLOCK PUBLIC

   INTEGER*2 WRITE.TMCF.HEADER.HOLD.UNLOCK

   WRITE.TMCF.HEADER.HOLD.UNLOCK = 1

   IF END#TMCF.SESS.NUM% THEN WRITE.TMCF.HEADER.HOLD.UNLOCK.ERROR

    WRITE FORM "I4,I1,C11,I4,C2,I2,I1,I4,I1,I1,C20"; HOLD         \
                #TMCF.SESS.NUM% AUTOUNLOCK, TMCF.REC.NO%;         \ 
             TMCF.MAINTLVL%,                                      \
             TMCF.ACTION%,                                        \
             TMCF.RCDKEY$,                                        \
             TMCF.NUMRECS%,                                       \
             TMCF.TERMLOAD$,                                      \
             TMCF.FILELVL%,                                       \
             TMCF.TERMILU%,                                       \
             TMCF.NUMITEMS%,                                      \
             TMCF.DESCTYPE%,                                      \
             TMCF.USERDATA%,                                      \
             TMCF.SPACE$ 

   WRITE.TMCF.HEADER.HOLD.UNLOCK = 0
   EXIT FUNCTION

   WRITE.TMCF.HEADER.HOLD.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.TMCF.HEADER.UNLOCK PUBLIC

   INTEGER*2 WRITE.TMCF.HEADER.UNLOCK

   WRITE.TMCF.HEADER.UNLOCK = 1

   IF END#TMCF.SESS.NUM% THEN WRITE.TMCF.HEADER.UNLOCK.ERROR

    WRITE FORM "I4,I1,C11,I4,C2,I2,I1,I4,I1,I1,C20";              \
                #TMCF.SESS.NUM% AUTOUNLOCK, TMCF.REC.NO%;         \ 
             TMCF.MAINTLVL%,                                      \
             TMCF.ACTION%,                                        \
             TMCF.RCDKEY$,                                        \
             TMCF.NUMRECS%,                                       \
             TMCF.TERMLOAD$,                                      \
             TMCF.FILELVL%,                                       \
             TMCF.TERMILU%,                                       \
             TMCF.NUMITEMS%,                                      \
             TMCF.DESCTYPE%,                                      \
             TMCF.USERDATA%,                                      \
             TMCF.SPACE$ 

   WRITE.TMCF.HEADER.UNLOCK = 0
   EXIT FUNCTION

   WRITE.TMCF.HEADER.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION



  FUNCTION WRITE.TMCF.DATA PUBLIC

   INTEGER*2 WRITE.TMCF.DATA  

   WRITE.TMCF.DATA = 1

   IF END#TMCF.SESS.NUM% THEN WRITE.TMCF.DATA.ERROR

    WRITE FORM "I1,C50"; #TMCF.SESS.NUM%, TMCF.REC.NO%;           \ 
                                          TMCF.ACTION%,           \
                                          TMCF.MNTDATA$

   WRITE.TMCF.DATA   = 0
   EXIT FUNCTION

   WRITE.TMCF.DATA.ERROR:

   CURRENT.REPORT.NUM% = TMCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION



