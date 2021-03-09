\*****************************************************************************
\***                          JOBOK FILE FUNCTIONS
\***    Version A           Steve Windsor               5th Jan 1993
\***
\***    Version B.          Robert Cowey.                07 OCT 1993.
\***    Defined JOBSOK.SET integer for use as a return code.
\***
\***    Version C.          Stuart Highley              5th May 2000
\***    Added flags for Dentistry (ExACT) and Well-being.
\***
\***    Version D.          Harpal Matharu             17th Jun 2010
\***    Added flag for PSD87's run.
\***
\***    Version E.              Nalini Mathusoothanan 22nd Jun 2011
\***    Added new variables for Core 2 Release
\***    JOBSOK.IUF.SOURCE$ 1 byte    - IUF processor [Mainframe or SAP ECC]
\***    JOBSOK.LAST.PROCESSED.BATCH$ - Serial number of last successfully
\***                                   processed batch.
\.............................................................................

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE JOBOKDEC.J86                                               ! BRC

   FUNCTION JOBSOK.SET PUBLIC

    INTEGER*2 JOBSOK.SET                                               ! BRC
    JOBSOK.SET EQ 1                                                    ! BRC

    JOBSOK.REPORT.NUM%  = 14
    JOBSOK.RECL%        = 84
    JOBSOK.FILE.NAME$   = "JOBOK"

    JOBSOK.SET EQ 0                                                    ! BRC

   END FUNCTION

\----------------------------------------------------------------------------

  FUNCTION READ.JOBSOK PUBLIC

   INTEGER*2 READ.JOBSOK

   READ.JOBSOK = 1

   IF END#JOBSOK.SESS.NUM% THEN READ.JOBSOK.ERROR

    READ #JOBSOK.SESS.NUM%,1; JOBSOK.RECORD$
    JOBSOK.PSB21$   = LEFT$(JOBSOK.RECORD$,1)
    JOBSOK.PSB22$   = MID$(JOBSOK.RECORD$,2,1)
    JOBSOK.PSB23$   = MID$(JOBSOK.RECORD$,3,1)
    JOBSOK.PSB24$   = MID$(JOBSOK.RECORD$,4,1)
    JOBSOK.PSB25$   = MID$(JOBSOK.RECORD$,5,1)
    JOBSOK.DATE$    = MID$(JOBSOK.RECORD$,6,4)
    JOBSOK.STATUS$  = MID$(JOBSOK.RECORD$,10,1)
    JOBSOK.PSB27$   = MID$(JOBSOK.RECORD$,11,1)
    JOBSOK.PSB28$   = MID$(JOBSOK.RECORD$,12,1)
    JOBSOK.DENTIST$ = MID$(JOBSOK.RECORD$,13,1)                      ! CSH
    JOBSOK.WBEING$  = MID$(JOBSOK.RECORD$,14,1)                      ! CSH
    JOBSOK.PSD87$   = MID$(JOBSOK.RECORD$,15,1)                      ! DHSM
    JOBSOK.IUF.SOURCE$             = MID$(JOBSOK.RECORD$,16,1)       ! ENM
    JOBSOK.LAST.PROCESSED.BATCH$   = MID$(JOBSOK.RECORD$,17,17)      ! ENM
    JOBSOK.SPACE$   = RIGHT$(JOBSOK.RECORD$,47)                      ! ENM

   READ.JOBSOK = 0
   EXIT FUNCTION

   READ.JOBSOK.ERROR:

   CURRENT.REPORT.NUM% = JOBSOK.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.JOBSOK PUBLIC

   INTEGER*2 WRITE.JOBSOK

   WRITE.JOBSOK = 1

    IF END#JOBSOK.SESS.NUM% THEN WRITE.JOBSOK.ERROR

    JOBSOK.RECORD$ = JOBSOK.PSB21$   +                          \
                     JOBSOK.PSB22$   +                          \
                     JOBSOK.PSB23$   +                          \
                     JOBSOK.PSB24$   +                          \
                     JOBSOK.PSB25$   +                          \
                     JOBSOK.DATE$    +                          \
                     JOBSOK.STATUS$  +                          \
                     JOBSOK.PSB27$   +                          \
                     JOBSOK.PSB28$   +                          \
                     JOBSOK.DENTIST$ +                          \ CSH
                     JOBSOK.WBEING$  +                          \ CSH
                     JOBSOK.PSD87$   +                          \ DHSM
                     JOBSOK.IUF.SOURCE$           +             \ ENM
                     JOBSOK.LAST.PROCESSED.BATCH$ +             \ ENM
                     JOBSOK.SPACE$
    WRITE #JOBSOK.SESS.NUM%,1;JOBSOK.RECORD$

   WRITE.JOBSOK = 0
   EXIT FUNCTION

   WRITE.JOBSOK.ERROR:

   CURRENT.REPORT.NUM% = JOBSOK.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

