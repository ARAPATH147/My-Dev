\*****************************************************************************
\*****************************************************************************
\***
\***                     SLEEPER CONTROL FILE FUNCTIONS
\***
\***      Version A           Steve Windsor               5th Jan 1993
\***
\***      Version b           Richard Foster              28th Jun 1993
\***
\***      Version C         Jaya Kumar Inbaraj               28/04/2014
\***      FOD260 - Enhanced Backup and Recovery
\***      Filler variable usage has been updated. Added two variables
\***      related to SLPCF Filler variable. Also added boiler plates
\***      for all the functions
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE SLPCFDEC.J86                                        !BRCF

\***********************************************************************
\*
\*  SLPCF.SET: SLPCF set function
\*
\***********************************************************************

   FUNCTION SLPCF.SET PUBLIC

    SLPCF.REPORT.NUM%  = 60
    SLPCF.FILE.NAME$   = "SLPCF"
    SLPCF.RECL%        = 80                                     !BRCF
   END FUNCTION

\***********************************************************************
\*
\*  READ.SLPCF: SLPCF Read function
\*
\***********************************************************************

  FUNCTION READ.SLPCF PUBLIC

   STRING SLPCF.FILLER.X$                                               !CJK

   INTEGER*2 READ.SLPCF

   READ.SLPCF = 1

   IF END #SLPCF.SESS.NUM% THEN READ.SLPCF.ERROR                        !CJK

    READ FORM "C21,C1,C6,C6,C6,C3,C8,C3,C8,C2,C4,C12";                   \LMG
             #SLPCF.SESS.NUM%,SLPCF.REC.NO%;                             \
             SLPCF.APP.NAME$,                                            \
             SLPCF.RUN.FREQUENCY$,                                       \BRCF
             SLPCF.DAY.NUM$,                                             \BRCF
             SLPCF.RUN.TIME$,                                            \BRCF
             SLPCF.LAST.RUN.DATE$,                                       \BRCF
             SLPCF.FILE.PRESENT$,                                        \
             SLPCF.FILE.PRESENT.NAME$,                                   \
             SLPCF.FILE.ABSENT$,                                         \
             SLPCF.FILE.ABSENT.NAME$,                                    \BRCF
             SLPCF.NODE.ID$,                                             \BRCF
             SLPCF.SOFTS.REC$,                                           \LMG
             SLPCF.FILLER$

    ! Storing the first letter of Filler variable                       !CJK
    SLPCF.FILLER.X$ = LEFT$(SLPCF.FILLER$,1)                            !CJK

    ! Initializing values                                               !CJK
    SLPCF.PARM.LEN% = 0                                                 !CJK
    SLPCF.PARM$     = ""                                                !CJK

    ! Checking whether the 1st letter of the filler variable is numeric !CJK
    IF MATCH("#",SLPCF.FILLER.X$ ,1) <> 0 THEN BEGIN                    !CJK
        ! Converting the 1st letter of Filler to an Integer             !CJK
        SLPCF.PARM.LEN% = VAL(SLPCF.FILLER.X$)                          !CJK
                                                                        !CJK
        ! If SLPCF Filler parameter length is greater than zero         !CJK
        IF SLPCF.PARM.LEN% > 0 THEN BEGIN                               !CJK
            ! Storing the SLPCF Filler parameter                        !CJK
            SLPCF.PARM$ = MID$(SLPCF.FILLER$,2,SLPCF.PARM.LEN%)         !CJK
        ENDIF                                                           !CJK
    ENDIF                                                               !CJK

   READ.SLPCF = 0
   EXIT FUNCTION

   READ.SLPCF.ERROR:

   CURRENT.REPORT.NUM% = SLPCF.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

\***********************************************************************
\*
\*  WRITE.SLPCF: SLPCF Write function
\*
\***********************************************************************

  FUNCTION WRITE.SLPCF PUBLIC

   INTEGER*2 WRITE.SLPCF

   WRITE.SLPCF = 1

   ! SLPCF.FILLER record is considered as 'XYYYYYYYYY' + CRLF

   ! Defining SLPCF Filler variable from Parameter length
   IF SLPCF.PARM.LEN% > 0 THEN BEGIN                                    !CJK
      SLPCF.FILLER$ = STR$(SLPCF.PARM.LEN%)              + \ X record   !CJK
                      LEFT$(SLPCF.PARM$,SLPCF.PARM.LEN%) + \ Y record   !CJK
                      STRING$((9 - SLPCF.PARM.LEN%),"-") + \ Remaining  !CJK
                      CHR$(13) + CHR$(10)                  ! "-" & CRLF !CJK
   ENDIF ELSE BEGIN                                                     !CJK
      ! By Default 10 hyphen and a CRLF                                 !CJK
      SLPCF.FILLER$ = STRING$(10,"-") + CHR$(13) + CHR$(10)             !CJK
   ENDIF                                                                !CJK

   IF END #SLPCF.SESS.NUM% THEN WRITE.SLPCF.ERROR                       !CJK

   WRITE FORM "C21,C1,C6,C6,C6,C3,C8,C3,C8,C2,C4,C12";                 \LMG
             #SLPCF.SESS.NUM%,SLPCF.REC.NO%;                           \
             SLPCF.APP.NAME$,                                          \
             SLPCF.RUN.FREQUENCY$,                                     \BRCF
             SLPCF.DAY.NUM$,                                           \BRCF
             SLPCF.RUN.TIME$,                                          \BRCF
             SLPCF.LAST.RUN.DATE$,                                     \BRCF
             SLPCF.FILE.PRESENT$,                                      \
             SLPCF.FILE.PRESENT.NAME$,                                 \
             SLPCF.FILE.ABSENT$,                                       \
             SLPCF.FILE.ABSENT.NAME$,                                  \BRCF
             SLPCF.NODE.ID$,                                           \BRCF
             SLPCF.SOFTS.REC$,                                         \LMG
             SLPCF.FILLER$

   WRITE.SLPCF = 0
   EXIT FUNCTION

   WRITE.SLPCF.ERROR:

   CURRENT.REPORT.NUM% = SLPCF.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


