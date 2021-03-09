\********************************************************************
\***      Space and Range Planogram database Host File (POGOK)
\***      Version A           Neil Bennett          6th June 2006
\***
\....................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE POGOKDEC.J86

\--------------------------------------------------------------------

   FUNCTION POGOK.SET PUBLIC

    POGOK.REPORT.NUM%   = 718
    POGOK.RECL%         = 80
    POGOK.FILE.NAME$    = "POGOK"

   END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.POGOK PUBLIC

   INTEGER*2 READ.POGOK

   READ.POGOK = 1

   IF END #POGOK.SESS.NUM% THEN READ.POGOK.ERROR

   READ FORM "4C4,6C1,5I1,9C4,I2,C1,I1,C4,2I4,C1";                  \
       #POGOK.SESS.NUM%, 1;                                         \
             POGOK.SRD.SER.NO$,                                     \
             POGOK.SRM.SER.NO$,                                     \
             POGOK.SRD.DATE$,                                       \
             POGOK.SRM.DATE$,                                       \
             POGOK.RELOAD$,                                         \
             POGOK.PE10.RUNFLAG$,                                   \
             POGOK.PE5.RUNFLAG$,                                    \
             POGOK.PE6.RUNFLAG$,                                    \
             POGOK.PE7.RUNFLAG$,                                    \
             POGOK.PE4.RUNFLAG$,                                    \
             POGOK.PE10.RETCODE%,                                   \
             POGOK.PE5.RETCODE%,                                    \
             POGOK.PE6.RETCODE%,                                    \
             POGOK.PE7.RETCODE%,                                    \
             POGOK.PE4.RETCODE%,                                    \
             POGOK.FAILED.SRD.SER.NO$,                              \
             POGOK.FAILED.SRD.DATE$,                                \
             POGOK.FAILED.SRM.SER.NO$,                              \
             POGOK.FAILED.SRM.DATE$,                                \
             POGOK.PE10.RUNDATE$,                                   \
             POGOK.PE5.RUNDATE$,                                    \
             POGOK.PE6.RUNDATE$,                                    \
             POGOK.PE7.RUNDATE$,                                    \
             POGOK.PE4.RUNDATE$,                                    \
             POGOK.DAYS.TO.RETAIN%,                                 \
             POGOK.PE19.RUNFLAG$,                                   \
             POGOK.PE19.RETCODE%,                                   \
             POGOK.PE19.RUNDATE$,                                   \
             POGOK.SRD.REC.COUNT%,                                  \
             POGOK.SRM.REC.COUNT%,                                  \
             POGOK.FILLER$

   READ.POGOK = 0
   EXIT FUNCTION

   READ.POGOK.ERROR:

   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

\--------------------------------------------------------------------
  FUNCTION READ.POGOK.LOCK PUBLIC

   INTEGER*2 READ.POGOK.LOCK

   READ.POGOK.LOCK = 1

   IF END #POGOK.SESS.NUM% THEN READ.POGOK.LOCK.ERROR

   READ FORM "4C4,6C1,5I1,9C4,I2,C1,I1,C4,2I4,C1";                  \
       #POGOK.SESS.NUM% AUTOLOCK, 1;                                \
             POGOK.SRD.SER.NO$,                                     \
             POGOK.SRM.SER.NO$,                                     \
             POGOK.SRD.DATE$,                                       \
             POGOK.SRM.DATE$,                                       \
             POGOK.RELOAD$,                                         \
             POGOK.PE10.RUNFLAG$,                                   \
             POGOK.PE5.RUNFLAG$,                                    \
             POGOK.PE6.RUNFLAG$,                                    \
             POGOK.PE7.RUNFLAG$,                                    \
             POGOK.PE4.RUNFLAG$,                                    \
             POGOK.PE10.RETCODE%,                                   \
             POGOK.PE5.RETCODE%,                                    \
             POGOK.PE6.RETCODE%,                                    \
             POGOK.PE7.RETCODE%,                                    \
             POGOK.PE4.RETCODE%,                                    \
             POGOK.FAILED.SRD.SER.NO$,                              \
             POGOK.FAILED.SRD.DATE$,                                \
             POGOK.FAILED.SRM.SER.NO$,                              \
             POGOK.FAILED.SRM.DATE$,                                \
             POGOK.PE10.RUNDATE$,                                   \
             POGOK.PE5.RUNDATE$,                                    \
             POGOK.PE6.RUNDATE$,                                    \
             POGOK.PE7.RUNDATE$,                                    \
             POGOK.PE4.RUNDATE$,                                    \
             POGOK.DAYS.TO.RETAIN%,                                 \
             POGOK.PE19.RUNFLAG$,                                   \
             POGOK.PE19.RETCODE%,                                   \
             POGOK.PE19.RUNDATE$,                                   \
             POGOK.SRD.REC.COUNT%,                                  \
             POGOK.SRM.REC.COUNT%,                                  \
             POGOK.FILLER$

   READ.POGOK.LOCK = 0
   EXIT FUNCTION

   READ.POGOK.LOCK.ERROR:

   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.POGOK PUBLIC

   INTEGER*2 WRITE.POGOK

   WRITE.POGOK = 1

   IF END #POGOK.SESS.NUM% THEN WRITE.POGOK.ERROR

   WRITE FORM "4C4,6C1,5I1,9C4,I2,C1,I1,C4,2I4,C1";                 \
       #POGOK.SESS.NUM%, 1;                                         \
             POGOK.SRD.SER.NO$,                                     \
             POGOK.SRM.SER.NO$,                                     \
             POGOK.SRD.DATE$,                                       \
             POGOK.SRM.DATE$,                                       \
             POGOK.RELOAD$,                                         \
             POGOK.PE10.RUNFLAG$,                                   \
             POGOK.PE5.RUNFLAG$,                                    \
             POGOK.PE6.RUNFLAG$,                                    \
             POGOK.PE7.RUNFLAG$,                                    \
             POGOK.PE4.RUNFLAG$,                                    \
             POGOK.PE10.RETCODE%,                                   \
             POGOK.PE5.RETCODE%,                                    \
             POGOK.PE6.RETCODE%,                                    \
             POGOK.PE7.RETCODE%,                                    \
             POGOK.PE4.RETCODE%,                                    \
             POGOK.FAILED.SRD.SER.NO$,                              \
             POGOK.FAILED.SRD.DATE$,                                \
             POGOK.FAILED.SRM.SER.NO$,                              \
             POGOK.FAILED.SRM.DATE$,                                \
             POGOK.PE10.RUNDATE$,                                   \
             POGOK.PE5.RUNDATE$,                                    \
             POGOK.PE6.RUNDATE$,                                    \
             POGOK.PE7.RUNDATE$,                                    \
             POGOK.PE4.RUNDATE$,                                    \
             POGOK.DAYS.TO.RETAIN%,                                 \
             POGOK.PE19.RUNFLAG$,                                   \
             POGOK.PE19.RETCODE%,                                   \
             POGOK.PE19.RUNDATE$,                                   \
             POGOK.SRD.REC.COUNT%,                                  \
             POGOK.SRM.REC.COUNT%,                                  \
             POGOK.FILLER$

   WRITE.POGOK = 0

   WRITE.POGOK.ERROR:

   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION

   END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.POGOK.UNLOCK PUBLIC

   INTEGER*2 WRITE.POGOK.UNLOCK

   WRITE.POGOK.UNLOCK = 1

   IF END #POGOK.SESS.NUM% THEN WRITE.POGOK.UNLOCK.ERROR

   WRITE FORM "4C4,6C1,5I1,9C4,I2,C1,I1,C4,2I4,C1";                 \
       #POGOK.SESS.NUM% AUTOUNLOCK, 1;                              \
             POGOK.SRD.SER.NO$,                                     \
             POGOK.SRM.SER.NO$,                                     \
             POGOK.SRD.DATE$,                                       \
             POGOK.SRM.DATE$,                                       \
             POGOK.RELOAD$,                                         \
             POGOK.PE10.RUNFLAG$,                                   \
             POGOK.PE5.RUNFLAG$,                                    \
             POGOK.PE6.RUNFLAG$,                                    \
             POGOK.PE7.RUNFLAG$,                                    \
             POGOK.PE4.RUNFLAG$,                                    \
             POGOK.PE10.RETCODE%,                                   \
             POGOK.PE5.RETCODE%,                                    \
             POGOK.PE6.RETCODE%,                                    \
             POGOK.PE7.RETCODE%,                                    \
             POGOK.PE4.RETCODE%,                                    \
             POGOK.FAILED.SRD.SER.NO$,                              \
             POGOK.FAILED.SRD.DATE$,                                \
             POGOK.FAILED.SRM.SER.NO$,                              \
             POGOK.FAILED.SRM.DATE$,                                \
             POGOK.PE10.RUNDATE$,                                   \
             POGOK.PE5.RUNDATE$,                                    \
             POGOK.PE6.RUNDATE$,                                    \
             POGOK.PE7.RUNDATE$,                                    \
             POGOK.PE4.RUNDATE$,                                    \
             POGOK.DAYS.TO.RETAIN%,                                 \
             POGOK.PE19.RUNFLAG$,                                   \
             POGOK.PE19.RETCODE%,                                   \
             POGOK.PE19.RUNDATE$,                                   \
             POGOK.SRD.REC.COUNT%,                                  \
             POGOK.SRM.REC.COUNT%,                                  \
             POGOK.FILLER$

   WRITE.POGOK.UNLOCK = 0

   WRITE.POGOK.UNLOCK.ERROR:

   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION

   END FUNCTION

\--------------------------------------------------------------------

  FUNCTION CREATE.POGOK PUBLIC

   INTEGER*2 CREATE.POGOK
   INTEGER*2 rc%

   POGOK.SRD.SER.NO$        = "0000"
   POGOK.SRM.SER.NO$        = "0000"
   POGOK.SRD.DATE$          = PACK$("00000000")
   POGOK.SRM.DATE$          = POGOK.SRD.DATE$
   POGOK.RELOAD$            = "N"
   POGOK.PE10.RUNFLAG$      = " "
   POGOK.PE5.RUNFLAG$       = " "
   POGOK.PE6.RUNFLAG$       = " "
   POGOK.PE7.RUNFLAG$       = " "
   POGOK.PE4.RUNFLAG$       = " "
   POGOK.PE10.RETCODE%      = 0
   POGOK.PE5.RETCODE%       = 0
   POGOK.PE6.RETCODE%       = 0
   POGOK.PE7.RETCODE%       = 0
   POGOK.PE4.RETCODE%       = 0
   POGOK.FAILED.SRD.SER.NO$ = "0000"
   POGOK.FAILED.SRD.DATE$   = POGOK.SRD.DATE$
   POGOK.FAILED.SRM.SER.NO$ = "0000"
   POGOK.FAILED.SRM.DATE$   = POGOK.SRD.DATE$
   POGOK.PE10.RUNDATE$      = POGOK.SRD.DATE$
   POGOK.PE5.RUNDATE$       = POGOK.SRD.DATE$
   POGOK.PE6.RUNDATE$       = POGOK.SRD.DATE$
   POGOK.PE7.RUNDATE$       = POGOK.SRD.DATE$
   POGOK.PE4.RUNDATE$       = POGOK.SRD.DATE$
   POGOK.DAYS.TO.RETAIN%    = 0
   POGOK.PE19.RUNFLAG$      = " "
   POGOK.PE19.RETCODE%      = 0
   POGOK.PE19.RUNDATE$      = POGOK.SRD.DATE$
   POGOK.SRD.REC.COUNT%     = 0
   POGOK.SRM.REC.COUNT%     = 0
   POGOK.FILLER$            = " "

   CREATE.POGOK = 1

   IF END #POGOK.SESS.NUM% THEN CREATE.POGOK.ERROR

   CREATE POSFILE POGOK.FILE.NAME$ DIRECT 1 RECL POGOK.RECL%        \
          AS POGOK.SESS.NUM% MIRRORED PERUPDATE                     !

   rc% = WRITE.POGOK
   IF rc% = 0 THEN BEGIN
      CREATE.POGOK = 0
   ENDIF

   EXIT FUNCTION

CREATE.POGOK.ERROR:

   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   FILE.OPERATION$ = "C"
   CURRENT.CODE$ = ""

  END FUNCTION
