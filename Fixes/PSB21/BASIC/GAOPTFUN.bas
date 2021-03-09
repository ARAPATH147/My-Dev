\*****************************************************************************
\***      GAOPT file functions 
\***      Reference : GAOPTFU.BAS
\***      Version A           Steve Windsor         10.05.93
\***
\***      Version B           Dom Sweeney           21.09.98
\***      Included READ & WRITE Locked and Unlocked. This can omly be done
\***      if a record number is specified, therefore making the file Direct.
\***      READ = Sequential, READ.LOCKED = Direct.
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE GAOPTDEC.J86

   FUNCTION GAOPT.SET PUBLIC

    GAOPT.REPORT.NUM% = 67    
    GAOPT.FILE.NAME$ = "GAOPT"
    LOCAL.GAOPT.FILE.NAME$ = "$ALGAOPT"
    
   END FUNCTION
\----------------------------------------------------------------------------

  FUNCTION READ.GAOPT PUBLIC

   INTEGER*2 READ.GAOPT

   READ.GAOPT = 1

   IF END#GAOPT.SESS.NUM% THEN READ.GAOPT.ERROR

      READ  #GAOPT.SESS.NUM%;                                       \ 
             GAOPT.LOGMSGS%,                                        \
             GAOPT.TERMILU%,                                        \
             GAOPT.OFFLCHNG%,                                       \
             GAOPT.NUMITEMS%,                                       \
             GAOPT.DESCTYPE%,                                       \
             GAOPT.USERDATA%,                                       \
             GAOPT.PCKGFCTR%,                                       \
             GAOPT.DFLTSIZE%

   READ.GAOPT = 0
   EXIT FUNCTION

   READ.GAOPT.ERROR:

   CURRENT.REPORT.NUM% = GAOPT.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.GAOPT.LOCKED PUBLIC

   INTEGER*2 READ.GAOPT.LOCKED

   READ.GAOPT.LOCKED = 1

   IF END#GAOPT.SESS.NUM% THEN READ.GAOPT.LOCKED.ERROR

      READ  #GAOPT.SESS.NUM% AUTOLOCK,1;                                       \ 
             GAOPT.LOGMSGS%,                                        \
             GAOPT.TERMILU%,                                        \
             GAOPT.OFFLCHNG%,                                       \
             GAOPT.NUMITEMS%,                                       \
             GAOPT.DESCTYPE%,                                       \
             GAOPT.USERDATA%,                                       \
             GAOPT.PCKGFCTR%,                                       \
             GAOPT.DFLTSIZE%

   READ.GAOPT.LOCKED = 0
   EXIT FUNCTION

   READ.GAOPT.LOCKED.ERROR:

   CURRENT.REPORT.NUM% = GAOPT.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION WRITE.GAOPT PUBLIC

   INTEGER*2 WRITE.GAOPT

   WRITE.GAOPT = 1

   IF END# GAOPT.SESS.NUM% THEN WRITE.GAOPT.ERROR

      WRITE  #GAOPT.SESS.NUM%;                                       \ 
             GAOPT.LOGMSGS%,                                        \
             GAOPT.TERMILU%,                                        \
             GAOPT.OFFLCHNG%,                                       \
             GAOPT.NUMITEMS%,                                       \
             GAOPT.DESCTYPE%,                                       \
             GAOPT.USERDATA%,                                       \
             GAOPT.PCKGFCTR%,                                       \
             GAOPT.DFLTSIZE%

   WRITE.GAOPT = 0
   EXIT FUNCTION

   WRITE.GAOPT.ERROR:

   CURRENT.REPORT.NUM% = GAOPT.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION 

\----------------------------------------------------------------------------

  FUNCTION WRITE.GAOPT.UNLOCK PUBLIC

   INTEGER*2 WRITE.GAOPT.UNLOCK

   WRITE.GAOPT.UNLOCK = 1

   IF END# GAOPT.SESS.NUM% THEN WRITE.GAOPT.UNLOCK.ERROR

      WRITE  #GAOPT.SESS.NUM%,1;                                       \ 
             GAOPT.LOGMSGS%,                                        \
             GAOPT.TERMILU%,                                        \
             GAOPT.OFFLCHNG%,                                       \
             GAOPT.NUMITEMS%,                                       \
             GAOPT.DESCTYPE%,                                       \
             GAOPT.USERDATA%,                                       \
             GAOPT.PCKGFCTR%,                                       \
             GAOPT.DFLTSIZE%

   WRITE.GAOPT.UNLOCK = 0
   EXIT FUNCTION

   WRITE.GAOPT.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = GAOPT.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION
