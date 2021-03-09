rem\
\*******************************************************************************
\*******************************************************************************
\***
\***
\***        FUNCTION      : UPDATE.DATE
\***        AUTHOR        : Stephen Kelsey (Pseudocode)
\***                      : Bruce Scriver  (Basic Code)
\***        DATE WRITTEN  : 12th February 1986 (Pseudocode)
\***                      : 4th March 1986     (Basic Code)
\***
\***        REFERENCE     : PSBF02
\***
\***        DATE OF LAST AMENDMENT - 22nd November 1988 (Pseudocode)
\***                               - 29th November 1988 (Basic Code)
\***
\***        DATE OF LAST COMPILATION - 30th November 1988
\***                   on controller - 16
\***
\***
\***        VERSION C.    B.A.A.SCRIVER       18th May 1988
\***        STOCK SYSTEM CHANGES.
\***        Change to set up unique data file session number to 0.
\***        New message number 550 replaces message number 551.
\***
\***        VERSION D.    D.S.O'DARE (Pseudocode)       22nd November, 1988
\***                                 (Basic)
\***        89A MERGE. (ie. small stores changes added to stocks changes).
\***        Replace CHAIN statement with new included code (PSBCHNE.J86).
\***        If length of F02.DATE$ > 6 then set F02.RETURN.CODE% to 1.
\***        Tidy up code in ERROR.DETECTED section.
\***
\***        VERSION E.     A. WEDGEWORTH                      7th July 1992 
\***        Remove redundant parameters passed to and from functions.
\***
\***        VERSION F.     LEE ROCKACH                        12th Sept 1997
\***        Changed the code to make the year 2000 a leap year.
\***
\***        Version G.      Stuart William McConnachie     31st Oct 2006
\***        Chain back to PSB50.286, instead of xxx50.286 derived from
\***        first three letters of MODULE.NUMBER$.  Doesn't work for
\***        PSD and SRP applications.
\***            
\*******************************************************************************
\*******************************************************************************

REM pseudocode follows...

\*******************************************************************************
\*******************************************************************************
\***
\***                       FUNCTION OVERVIEW
\***                       -----------------
\***
\***        This function receives a date, which is checked for validity,
\***     and a number of days that are to be added to the date or subtracted
\***     it. It then processes the date and returns to the calling program.
\***     Global data passed includes the string F02.DATE$, which is in YYMMDD
\***     format. If F02.DATE$ is found to be invalid in any way then the
\***     return code F02.RETURN.CODE% is set to 1.
\***
\*******************************************************************************
\*******************************************************************************
\***
\***  %INCLUDE of globals for public function UPDATE.DATE
\***  %INCLUDE of globals for external function CONV.TO.HEX
\***  %INCLUDE of globals for external function CONV.TO.STRING
\***  %INCLUDE of globals for screen chaining parameters (PSBUSEG.J86)
\***
\***  %INCLUDE of statements for external function ADXERROR
\***  %INCLUDE of statements for external function APPLICATION.LOG
\***  %INCLUDE of statements for external function CONV.TO.HEX
\***  %INCLUDE of statements for external function CONV.TO.STRING
\***
\-------------------------------------------------------------------------------

      ! 1 line deleted from here                                       ! EAW
      %INCLUDE PSBF02G.J86
      %INCLUDE PSBF16G.J86
      %INCLUDE PSBF17G.J86
      %INCLUDE PSBUSEG.J86                                            ! DBCW

      STRING GLOBAL                                                    \ EAW
           BATCH.SCREEN.FLAG$,                                         \ EAW
           MODULE.NUMBER$                                              ! EAW             

      %INCLUDE ADXERROR.J86
      %INCLUDE PSBF01E.J86
      %INCLUDE PSBF16E.J86
      %INCLUDE PSBF17E.J86

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***  Define parameters and other variables used in the function.
\***
\-------------------------------------------------------------------------------

   FUNCTION UPDATE.DATE (INCREMENT%)                                   \ EAW
   PUBLIC
   ! 3 parameters removed from here                                    ! EAW   

      STRING                                                           \
\ 1 line deleted from here                                             \ EAW      
                CHAIN.MODULE$,                                         \
\ 1 line deleted from here                                             \ CBAAS
                ERRNUM$,                                               \
\ 1 line deleted from here                                             \ CBAAS
                MESSAGE$,                                              \
\ 2 lines deleted from here                                            \ EAW
                PARM.LEN$,                                             \
                STRING.ERRL$,                                          \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$

      INTEGER*1 DAY%,                                                  \
                EVENT.NUM%,                                            \
                LEAP.DAY%,                                             \
                MSGGRP%,                                               \
                MONTH%,                                                \
                SEVERITY%,                                             \
                YEAR%

      INTEGER*2 EVENT.NO%,                                             \
                F02.RETURN.CODE%,                                      \ EAW            
                F17.RETURN.CODE%,                                      \ EAW            
            MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                RET.CODE%,                                             \
                TERM%,                                                 \ EAW
                UPDATE.DATE                                            ! EAW

      INTEGER*4 COUNT%,                                                \
                INCREMENT%,                                            \
                INTEGER4%

      REAL      REAL.DATE

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   ON ERROR GOTO ERROR.DETECTED
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***   set variable string 1 to 10 spaces
\***
\***   set F02.RETURN.CODE% to 0
\***   set up PSBCHN.PRG for chaining out in error handling to
\***     "ADX_UPGM:" + leftmost 3 bytes of MODULE.NUMBER$ + "50.286"
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO ERROR.DETECTED

      UNIQUE$ = "          "
      ERRNUM$ = "    "
\ 1 line deleted from here                                             \ CBAAS
      STRING.ERRL$ = "      "

      F02.RETURN.CODE% = 0
      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                ! GSWM

\-------------------------------------------------------------------------------
\***
\***   check that F02.DATE$ contains all numeric digits, obtaining the real
\***   value of it (VAL(F02.DATE$))
\***
\***   IF the length of F02.DATE$ is greater then 6 characters then
\***      set F02.RETURN.CODE% to 1
\***   ENDIF
\***
\***   destring F02.DATE$ into year, month, days
\***   IF month > 12 THEN
\***      set F02.RETURN.CODE% to 1
\***   ELSE
\***      IF days > 31 THEN
\***         set F02.RETURN.CODE% to 1
\***      ELSE
\***         IF days > 30 AND month = 4, 6, 9, 11 THEN
\***            set F02.RETURN.CODE% to 1
\***         ELSE
\***            IF month = 2 THEN
\***               GOSUB DETERMINE.LEAP.YEAR
\***               IF days > 29 THEN
\***                  set F02.RETURN.CODE% to 1
\***               ELSE
\***                  IF leap year day = 0 AND days > 28 THEN
\***                     set F02.RETURN.CODE% to 1
\***                  ENDIF
\***               ENDIF
\***            ENDIF
\***         ENDIF
\***      ENDIF
\***   ENDIF
\***
\***   IF F02.RETURN.CODE% <> 0 THEN
\***      CALL APPLICATION.LOG function to log event 4, message 706
\***   Set PARM.LEN$ to STR$ of LEN(F02.DATE$)
\***   Set VAR.STRING.1$ to (the rightmost two bytes of F17.RETURNED.STRING$)
\***      + PARM.LEN$ + F02.DATE$.
\***
\-------------------------------------------------------------------------------

      REAL.DATE = VAL(F02.DATE$)

      IF LEN(F02.DATE$) > 6 THEN                                      \ DBCW
         F02.RETURN.CODE% = 1                                         ! DBCW

      YEAR%  = VAL(LEFT$(F02.DATE$,2))
      MONTH% = VAL(MID$(F02.DATE$,3,2))
      DAY%   = VAL(RIGHT$(F02.DATE$,2))

      IF MONTH% > 12 THEN                                              \
         F02.RETURN.CODE% = 1                                          \
      ELSE                                                             \
         IF DAY% > 31 THEN                                             \
            F02.RETURN.CODE% = 1                                       \
         ELSE                                                          \
            IF DAY% > 30 AND (MONTH% = 4 OR                            \
                              MONTH% = 6 OR                            \
                              MONTH% = 9 OR                            \
                              MONTH% = 11) THEN                        \
               F02.RETURN.CODE% = 1                                    \
            ELSE                                                       \
               IF MONTH% = 2 THEN                                      \
                  GOSUB DETERMINE.LEAP.YEAR                           :\
                  IF DAY% > 29 THEN                                    \
                     F02.RETURN.CODE% = 1                             :\
                  ELSE                                                 \
                     IF LEAP.DAY% = 0 AND DAY% > 28 THEN               \
                        F02.RETURN.CODE% = 1

      IF F02.RETURN.CODE% <> 0 THEN                                    \
         MESSAGE.NUMBER% = 706                                        :\
         EVENT.NO% = 4                                                :\
         INTEGER4% = 706                                              :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ EAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            PARM.LEN$     = CHR$(LEN(F02.DATE$))                      :\
            VAR.STRING.1$ = RIGHT$(F17.RETURNED.STRING$,2) +           \ DBCW
                            PARM.LEN$ + F02.DATE$                     :\
            VAR.STRING.2$ = "02" + PARM.LEN$ + F02.DATE$              :\
            RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,              \ EAW
                                         VAR.STRING.1$,                \
                                         VAR.STRING.2$,                \
                                         EVENT.NO%)

      PARM.LEN$     = CHR$(LEN(F02.DATE$))                             ! DBCW
      VAR.STRING.1$ = (RIGHT$(F17.RETURNED.STRING$,2) + PARM.LEN$      \ DBCW
                       + F02.DATE$)                                    ! DBCW

\-------------------------------------------------------------------------------
\***
\***   IF F02.RETURN.CODE% = 0 AND increment <> 0 THEN
\***      GOSUB DETERMINE.LEAP.YEAR
\***      IF increment > 0 THEN
\***         FOR count = 1 to increment step 1
\***            add 1 to days
\***            IF days > 31 THEN
\***               IF month = 12 THEN
\***                  GOSUB CHANGE.YEAR.UP
\***               ELSE
\***                  set days to 1
\***                  add 1 to month
\***               ENDIF
\***            ELSE
\***               IF days > 30 THEN
\***                  IF month = 4,6,9,11 THEN
\***                     set days to 1
\***                     add 1 to month
\***                  ELSE
\***                  ENDIF
\***               ELSE
\***                  IF days > 28 AND month = 2 THEN
\***                     IF leap year day = 1 AND days > 29 THEN
\***                        set days to 1
\***                        add 1 to month
\***                     ELSE
\***                        IF leap year day = 0 THEN
\***                           set days to 1
\***                           add 1 to month
\***                        ENDIF
\***                     ENDIF
\***                  ENDIF
\***               ENDIF
\***            ENDIF
\***         NEXT count
\***      ELSE
\***         FOR count = 1 to ABSolute value of increment step 1
\***            subtract 1 from days
\***            IF days = 0 THEN
\***               IF month = 1 THEN
\***                  GOSUB CHANGE.YEAR.DOWN
\***               ELSE
\***                  IF month = 2,4,6,8,9,11 THEN
\***                     set days to 31
\***                     subtract 1 from month
\***                  ELSE
\***                     IF month = 5,7,10,12 THEN
\***                        subtract 1 from month
\***                        set days to 30
\***                     ELSE
\***                        set days to 28 + leap year day
\***                        subtract 1 from month
\***                     ENDIF
\***                  ENDIF
\***               ENDIF
\***            ENDIF
\***         NEXT count
\***      ENDIF
\***   ENDIF
\***
\***   IF F02.RETURN.CODE% <> 0
\***   AND increment <> 0 THEN
\***      GOSUB SET.UP.DATE
\***   ENDIF
\***
\***   FUNCTION.EXIT:
\***
\***      Set UPDATE.DATE to the value of the return code
\***
\***   EXIT FUNCTION
\***
\-------------------------------------------------------------------------------

      IF F02.RETURN.CODE% = 0 AND INCREMENT% <> 0 THEN                 \
         GOSUB DETERMINE.LEAP.YEAR                                    :\
         IF INCREMENT% > 0 THEN                                        \
                                                                       \
            FOR COUNT% = 1 TO INCREMENT% STEP 1                       :\
                                                                       \
               DAY% = DAY% + 1                                        :\
               IF DAY% > 31 THEN                                       \
                  IF MONTH% = 12 THEN                                  \
                     GOSUB CHANGE.YEAR.UP                              \
                  ELSE                                                 \
                     DAY% = 1                                         :\
                     MONTH% = MONTH% + 1                               \
               ELSE                                                    \
                  IF DAY% > 30 THEN                                    \
                     IF MONTH% = 4 OR                                  \
                        MONTH% = 6 OR                                  \
                        MONTH% = 9 OR                                  \
                        MONTH% = 11 THEN                               \
                        DAY% = 1                                      :\
                        MONTH% = MONTH% + 1                            \
                     ELSE                                              \
                  ELSE                                                 \
                     IF DAY% > 28 AND MONTH% = 2 THEN                  \
                        IF LEAP.DAY% = 1 AND DAY% > 29 THEN            \
                           DAY% = 1                                   :\
                           MONTH% = MONTH% + 1                         \
                        ELSE                                           \
                           IF LEAP.DAY% = 0 THEN                       \
                              DAY% = 1                                :\
                              MONTH% = MONTH% + 1                     :\
                                                                       \
            NEXT COUNT%                                               :\
                                                                       \
         ELSE                                                          \
                                                                       \
            FOR COUNT% = 1 TO ABS(INCREMENT%) STEP 1                  :\
                                                                       \
               DAY% = DAY% - 1                                        :\
               IF DAY% = 0 THEN                                        \
                  IF MONTH% = 1 THEN                                   \
                     GOSUB CHANGE.YEAR.DOWN                            \
                  ELSE                                                 \
                     IF MONTH% = 2 OR                                  \
                        MONTH% = 4 OR                                  \
                        MONTH% = 6 OR                                  \
                        MONTH% = 8 OR                                  \
                        MONTH% = 9 OR                                  \
                        MONTH% = 11 THEN                               \
                        DAY% = 31                                     :\
                        MONTH% = MONTH% - 1                            \
                     ELSE                                              \
                        IF MONTH% = 5 OR                               \
                           MONTH% = 7 OR                               \
                           MONTH% = 10 OR                              \
                           MONTH% = 12 THEN                            \
                           DAY% = 30                                  :\
                           MONTH% = MONTH% - 1                         \
                        ELSE                                           \
                           DAY% = 28 + LEAP.DAY%                      :\
                           MONTH% = MONTH% -1                         :\
                                                                       \
            NEXT COUNT%

      IF F02.RETURN.CODE% = 0 AND INCREMENT% <> 0 THEN                 \
         GOSUB SET.UP.DATE

   FUNCTION.EXIT:
   
      UPDATE.DATE = F02.RETURN.CODE%                                   ! EAW   

      EXIT FUNCTION

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\************************ subroutines follow ***********************************
\*******************************************************************************
\***
\*** CHANGE.YEAR.UP:
\***
\***   set days to 1
\***   set month to 1
\***   set the year to the remainder of year + 1 divided by 100
\***   GOSUB DETERMINE.LEAP.YEAR
\***
\***   RETURN
\***
\-------------------------------------------------------------------------------

   CHANGE.YEAR.UP:

      DAY% = 1
      MONTH% = 1
      YEAR% = MOD((YEAR% + 1),100)

      GOSUB DETERMINE.LEAP.YEAR

      RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** CHANGE.YEAR.DOWN:
\***
\***   set days to 31
\***   set month to 12
\***   subtract 1 from year
\***   IF year < 0 THEN
\***      set year to 99
\***   ENDIF
\***   GOSUB DETERMINE.LEAP.YEAR
\***
\***   RETURN
\***
\-------------------------------------------------------------------------------

   CHANGE.YEAR.DOWN:

      DAY% = 31
      MONTH% = 12
      YEAR% = YEAR% - 1

      IF YEAR% < 0 THEN                                                \
         YEAR% = 99

      GOSUB DETERMINE.LEAP.YEAR

      RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** DETERMINE.LEAP.YEAR:
\***
\***   IF the remainder of the division by 4 of the year = 0 THEN
\***       set leap year day to 1
\***   ELSE
\***       set leap year day to 0
\***   ENDIF
\***
\***   RETURN
\***
\-------------------------------------------------------------------------------

   DETERMINE.LEAP.YEAR:

      IF MOD(YEAR%,4) = 0 THEN BEGIN
         LEAP.DAY% = 1              
      ENDIF ELSE BEGIN                          
         LEAP.DAY% = 0              
      ENDIF

      RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** SET.UP.DATE:
\***
\***   IF year has only one digit THEN
\***      set F02.DATE$ to "0" and year
\***   ELSE
\***      set F02.DATE$ to year
\***   ENDIF
\***   IF month has only one digit THEN
\***      add "0" and month to F02.DATE$
\***   ELSE
\***      add month to F02.DATE$
\***   ENDIF
\***   IF day has only one digit THEN
\***      add "0" and day to F02.DATE$
\***   ELSE
\***      add day to F02.DATE$
\***   ENDIF
\***
\***   RETURN
\***
\-------------------------------------------------------------------------------

   SET.UP.DATE:

      IF YEAR% < 10 THEN                                               \
         F02.DATE$ = "0" + STR$(YEAR%)                                 \
      ELSE                                                             \
         F02.DATE$ = STR$(YEAR%)

      IF MONTH% < 10 THEN                                              \
         F02.DATE$ = F02.DATE$ + "0" + STR$(MONTH%)                    \
      ELSE                                                             \
         F02.DATE$ = F02.DATE$ + STR$(MONTH%)

      IF DAY% < 10 THEN                                                \
         F02.DATE$ = F02.DATE$ + "0" + STR$(DAY%)                      \
      ELSE                                                             \
         F02.DATE$ = F02.DATE$ + STR$(DAY%)

      RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** ERROR.DETECTED:
\*** ---------------
\***   set UPDATE.DATE to 1 to indicate unsuccessful processing
\***
\***   IF the value of ERR = "OM" THEN               \REM out of memory
\***      CALL ADXERROR to log the error
\***   ELSE
\***      IF the value of ERR = "IH" THEN            \REM non-numeric data
\***         Set VAR.STRING.1$ to (rightmost two bytes of "00" + MESSAGE$)
\****          + (rightmost byte of F17.RETURNED.STRING$) + F02.DATE$
\***         CALL APPLICATION.LOG to log message number 706
\***         RESUME processing
\***      ELSE
\***         IF the value of ERR = "CM", or "CT" THEN  \REM chain failure
\***            Set VAR.STRING.1$ to "BF02 " + (the 3rd byte of MODULE.NUMBER$)
\***                       + "50  "
\***            Set VAR.STRING.2$ to "PS" + (the 3rd byte of MODULE.NUMBER$) 
\***                       + "50"
\***            CALL APPLICATION.LOG to log message number 553
\***         ELSE
\***            CALL APPLICATION.LOG to log message number 550
\***         ENDIF
\***      ENDIF
\***   ENDIF
\***   
\***   IF program is not a screen program THEN
\***      STOP
\***
\***   %INCLUDE PSBCHNE.J86
\***
\*** END FUNCT]ION
\***
\-------------------------------------------------------------------------------

   ERROR.DETECTED:

      UPDATE.DATE = 1                                                  ! EAW

\ 3 lines deleted from here                                            ! CBAAS

      IF ERR <> "IH" AND ERR <> "CM" AND ERR <> "CT" THEN              \
         EVENT.NO% = 1                                                :\
         INTEGER4% = ERRN                                             :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ EAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            ERRNUM$   = F17.RETURNED.STRING$                          :\
\ 8 lines deleted from here                                            \ CBAAS
            STRING.ERRL$ = PACK$(RIGHT$("000000" + STR$(ERRL),6))      ! CBAAS
\ 4 lines deleted from here                                            ! CBAAS

      IF ERR = "OM" THEN                                      \REM out of memory
         IF F17.RETURN.CODE% = 0 THEN                                  \
            TERM%         = 0                                         :\
            MSGGRP%       = ASC("J")                                  :\
            MSGNUM%       = 0                                         :\
            SEVERITY%     = 3                                         :\
            EVENT.NUM%    = 1                                         :\
            UNIQUE$      = ERRNUM$ + CHR$(0) + ERR + STRING.ERRL$     :\ CBAAS
            RET.CODE%    = ADXERROR (TERM%,                            \
                                     MSGGRP%,                          \
                                     MSGNUM%,                          \
                                     SEVERITY%,                        \
                                     EVENT.NUM%,                       \
                                     UNIQUE$)

      IF ERR = "IH" THEN                          \REM non-numeric **********
         MESSAGE.NUMBER% = 706                                        :\
         EVENT.NO%       = 4                                          :\
         INTEGER4%       = 706                                        :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ EAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            MESSAGE$ = F17.RETURNED.STRING$                           :\
            INTEGER4% = LEN(F02.DATE$)                                :\
            F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,              \ EAW
                                               INTEGER4%)             :\
            IF F17.RETURN.CODE% = 0 THEN                               \
               VAR.STRING.1$ = RIGHT$("00" + MESSAGE$,2)               \ DBCW
                             + RIGHT$(F17.RETURNED.STRING$,1)          \
                             + F02.DATE$                              :\
               VAR.STRING.2$ = "02" + STR$(LEN(F02.DATE$)) +           \
                               F02.DATE$                              :\
               RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,           \ EAW
                                            VAR.STRING.1$,             \
                                            VAR.STRING.2$,             \
                                            EVENT.NO%)                :\
               RESUME FUNCTION.EXIT

      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF02 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ DBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ DBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ EAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)

      IF ERR <> "OM" AND ERR <> "IH" AND                               \
         ERR <> "CM" AND ERR <> "CT" THEN                              \
         IF F17.RETURN.CODE% = 0 THEN                                  \
            MESSAGE.NUMBER% = 550                                     :\ CBAAS
            VAR.STRING.1$ = ERRNUM$ + CHR$(0) + ERR + STRING.ERRL$    :\ CBAAS
            INTEGER4% = ERRN                                          :\
            RET.CODE% = CONV.TO.HEX (INTEGER4%)                       :\ EAW
            IF RET.CODE% = 0 THEN                                      \ EAW
               VAR.STRING.2$ = ERR + F16.HEX.STRING$                   \
                             + "  0" + STR$(ERRL)                     :\ CBAAS
               EVENT.NO%     = 1                                      :\
               RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,           \ EAW
                                            VAR.STRING.1$,             \
                                            VAR.STRING.2$,             \
                                            EVENT.NO%)

\ 5 lines deleted from here                                            ! DBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN STOP                           ! DBCW
      %INCLUDE PSBCHNE.J86                                            ! DBCW

   END FUNCTION

