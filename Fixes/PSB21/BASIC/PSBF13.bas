rem\
\*******************************************************************************
\*******************************************************************************
\***
\***
\***        FUNCTION      : PSDATE
\***        AUTHOR        : Stephen Kelsey (Pseudocode)
\***                      : Bruce Scriver  (Basic Code)
\***        DATE WRITTEN  : 24th January 1986 (Pseudocode)
\***                      : 6th March 1986    (Basic Code)
\***
\***        REFERENCE     : PSBF13
\***
\***
\***        VERSION C.    B.A.A.SCRIVER       19th May 1988
\***        STOCK SYSTEM CHANGES.
\***        Change to set file session number part of unique data for error
\***        calls to zero, as no files are accessed.
\***        New message number 550 replaces message number 551.
\***
\***        VERSION D.    D.S. O'DARE (Pseudocode)      24th November 1988
\***                      B.C. WILLIS (Basic)            1st December 1988
\***        89A MERGE. (ie. small stores changes added to stocks changes).
\***        Replace the CHAIN statement with the new included code 
\***        (PSBCHNE.J86) and CHAIN.FILE.NAME$ with PSBCHN.PRG.  Amend 
\***        program-to-chain-to from "01" to "50".
\***
\***        VERSION E.    JANET LAWRENCE                16th August 1990  
\***        EPOS-CSR LINK II.
\***        A new global variable has been added to indicate whether or not
\***        this function should display a message and log an event upon
\***        receipt of an invalid date.            
\***        Version letters for all included code have also been added.
\***
\***        VERSION F.    ANDREW WEDGEWORTH             17th July 1992
\***        Redundant function parameters removed (ie. BATCH.SCREEN.FLAG$,
\***        OPERATOR.NUMBER$ and MODULE.NUMBER$).         
\***
\***        VERSION G.    SCOTT BAKER                21st September 1997
\***        Changes necessary for y2k compliance.  The first is to make 2000
\***        a leap year the rest is to make correct errors in Zellars formula.
\***
\***        Version H.      Stuart William McConnachie     31st Oct 2006
\***        Chain back to PSB50.286, instead of xxx50.286 derived from
\***        first three letters of MODULE.NUMBER$.  Doesn't work for
\***        PSD and SRP applications.
\***            
\*******************************************************************************
\*******************************************************************************

REM Pseudocode follows...

\*******************************************************************************
\*******************************************************************************
\***
\***                        FUNCTION OVERVIEW
\***                        -----------------
\***
\***        This function receives a date in YYMMDD format, and calculates
\***     the day of the week upon which the given date fell. (Full checks
\***     are made on the validity of the passed date). This routine uses
\***     Zellars formula -
\***     {[(2.6 * M) - 0.2] + D + Y + [C / 4] + [Y / 4] - 2 * C}
\***     D = day within month, Y = year, C = century (1986 => C = 19)
\***     M = month (march = 1, april = 2, ... february = 12)
\***     { } = take the integer remaining after division by 7
\***     [ ] = take the integer
\***
\*******************************************************************************
\*******************************************************************************
\***
\***  %INCLUDE of globals for external function APPLICATION.LOG
\***  %INCLUDE of globals for public function PSDATE
\***  %INCLUDE of globals for external function CONV.TO.HEX
\***  %INCLUDE of globals for external function CONV.TO.STRING
\***  %INCLUDE of globals for screen chaining parameters (PSBUSEG)
\***
\***  %INCLUDE of statements for external function ADXERROR
\***  %INCLUDE of statements for external function APPLICATION.LOG
\***  %INCLUDE of statements for external function CONV.TO.HEX
\***  %INCLUDE of statements for external function CONV.TO.STRING
\***
\-------------------------------------------------------------------------------

      ! 1 line deleted from here                                        ! FAW
      %INCLUDE PSBF13G.J86                                              ! FAW
      %INCLUDE PSBF16G.J86                                              ! FAW
      %INCLUDE PSBF17G.J86                                              ! FAW           
      %INCLUDE PSBUSEG.J86                                              ! EJAL

      STRING GLOBAL                                                     \ FAW
           BATCH.SCREEN.FLAG$,                                          \ FAW
           MODULE.NUMBER$                 

      %INCLUDE ADXERROR.J86                                             ! EJAL
      %INCLUDE PSBF01E.J86                                              ! FAW
      %INCLUDE PSBF16E.J86                                              ! FAW
      %INCLUDE PSBF17E.J86                                              ! FAW

\-------------------------------------------------------------------------------
\*******************************************************************************
\***
\***  Function and variable definitions.
\***
\-------------------------------------------------------------------------------

   FUNCTION PSDATE (INPUT.DATE$)                                       \
   PUBLIC

      STRING                                                           \
\ 1 line deleted from here                                             \ FAW
                CHAIN.MODULE$,                                         \
                DAY$,                                                  \
\ 1 line deleted from here                                             \ CBAAS
                ERRNUM$,                                               \
\ 1 line deleted from here                                             \ CBAAS
                INPUT.DATE$,                                           \
                MESSAGE$,                                              \
\ 1 line deleted from here                                             \ FAW
                MONTH$,                                                \
\ 1 line deleted from here                                             \ FAW
\ 1 line deleted from here                                             \ CBAAS
                STRING.ERRL$,                                          \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$,                                         \
                YEAR$

      INTEGER*1 DATE.RETURN.CODE%,                                     \
                EVENT.NUM%,                                            \
                MSGGRP%,                                               \
                SEVERITY% 

      INTEGER*2 EVENT.NO%,                                             \
                F13.RETURN.CODE%,                                      \ FAW
                F17.RETURN.CODE%,                                      \ FAW
                MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                PSDATE,                                                \ FAW
                RET.CODE%,                                             \
                TERM%

      INTEGER*4 INTEGER4%,    \   
                NUM.1,        \
                NUM.2,        \
                NUM.3,        \
                NUM.4,        \
                NUM.5,        \        
                MODIFIED.YEAR,         \ 
                CURRENT.DAY,           \
                CURRENT.CENTURY,       \
                TOTAL.SUM  
                

      REAL      REAL.DATE,    \      
                DAY.NUMBER,   \  
                MODIFIED.MONTH

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   ON ERROR goto the label ERROR.DETECTED
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***   set variable string 1 to 10 spaces
\***
\***   set date return code to 0
\***   set F13.RETURN.CODE% to 0
\***   set PSBCHN.PRG to "ADX_UPGM:" + (leftmost 3 bytes of MODULE.NUMBER$)
\***             + "50.286"
\***   set chaining module to first program of calling application
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO ERROR.DETECTED

      UNIQUE$ = "          "
      ERRNUM$ = "    "
\ 1 line deleted from here                                             ! CBAAS
      STRING.ERRL$ = "      "

      DATE.RETURN.CODE% = 0
      F13.RETURN.CODE% = 0                                             
      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                ! HSWM

\-------------------------------------------------------------------------------
\***
\***   check that the date is numeric (use VAL)
\***
\***   REM the date is in YYMMDD format.
\***
\***   IF the length of the date string is less than 6 THEN
\***      set F13.RETURN.CODE% to 1
\***      set message number to 706
\***      GOSUB LOG.INTERNAL.ERROR
\***   ENDIF
\***
\-------------------------------------------------------------------------------

      REAL.DATE = VAL(INPUT.DATE$)

      IF LEN(INPUT.DATE$) <> 6 THEN                                    \
         F13.RETURN.CODE% = 1                                         :\
         MESSAGE.NUMBER% = 706                                        :\
         GOSUB LOG.INTERNAL.ERROR

\-------------------------------------------------------------------------------
\***
\***   IF F13.RETURN.CODE% is set to 0
\***      IF string day is < 01 THEN
\***         set date return code to 1
\***      ELSE
\***         IF string month is < 01 or > 12 THEN
\***            set date return code to 1
\***         ELSE
\***            IF the string month = 01,03,05,07,08,10,12 THEN
\***               IF the string day > 31 THEN
\***                  set date return code to 1
\***               ELSE
\***               ENDIF
\***            ELSE
\***               IF the string month = 04,06,09,11 THEN
\***                  IF the string day > 30 THEN
\***                     set date return code to 1
\***                  ELSE
\***                  ENDIF
\***               ELSE
\***                  IF the string month = 02 THEN
\***                     IF the string year is 00
\***                     OR MOD (VAL(string year), 4) <> 0 THEN
\***                        IF the string day > 28 THEN
\***                           set date return code to 1
\***                        ELSE
\***                        ENDIF
\***                     ELSE
\***                        IF the string day > 29 THEN
\***                           set date return code to 1
\***                        ELSE
\***                        ENDIF
\***                     ENDIF
\***                  ENDIF
\***               ENDIF
\***            ENDIF
\***         ENDIF
\***      ENDIF
\***   ENDIF
\***
\-------------------------------------------------------------------------------

      IF F13.RETURN.CODE% = 0 THEN                                     \
         YEAR$  = LEFT$(INPUT.DATE$,2)                                :\
         MONTH$ = MID$(INPUT.DATE$,3,2)                               :\
         DAY$   = RIGHT$(INPUT.DATE$,2)                               :\
         IF DAY$ < "01" THEN                                           \
            DATE.RETURN.CODE% = 1                                      \
         ELSE                                                          \
            IF MONTH$ < "01" OR MONTH$ > "12" THEN                     \
               DATE.RETURN.CODE% = 1                                   \
            ELSE                                                       \
               IF MONTH$ = "01"                                        \
               OR MONTH$ = "03"                                        \
               OR MONTH$ = "05"                                        \
               OR MONTH$ = "07"                                        \
               OR MONTH$ = "08"                                        \
               OR MONTH$ = "10"                                        \
               OR MONTH$ = "12" THEN                                   \
                  IF DAY$ > "31" THEN                                  \
                     DATE.RETURN.CODE% = 1                             \
                  ELSE                                                 \
               ELSE                                                    \
                  IF MONTH$ = "04"                                     \
                  OR MONTH$ = "06"                                     \
                  OR MONTH$ = "09"                                     \
                  OR MONTH$ = "11" THEN                                \
                     IF DAY$ > "30" THEN                               \
                        DATE.RETURN.CODE% = 1                          \
                     ELSE                                              \
                  ELSE                                                 \
                     IF MOD(VAL(YEAR$),4) <> 0 THEN                    \ GSB
                        IF DAY$ > "28" THEN                            \
                           DATE.RETURN.CODE% = 1                       \
                        ELSE                                           \
                     ELSE                                              \
                        IF DAY$ > "29" THEN                            \
                           DATE.RETURN.CODE% = 1

\-------------------------------------------------------------------------------
\***
\***   IF date return code is <> 0 THEN
\***      set F13.RETURN.CODE% to 1
\***      set message number to 706
\***      GOSUB LOG.INTERNAL.ERROR
\***   ENDIF
\***
\-------------------------------------------------------------------------------

      IF DATE.RETURN.CODE% <> 0 THEN                                   \
         F13.RETURN.CODE% = 1                                         :\
         MESSAGE.NUMBER% = 706                                        :\
         GOSUB LOG.INTERNAL.ERROR

\-------------------------------------------------------------------------------
\***
\***   IF date return code = 0 and F13.RETURN.CODE% = 0 THEN
\***      IF string year < 85 THEN
\***         set integer century to 20
\***      ELSE
\***         set integer century to 19
\***      ENDIF
\***      IF string month = 01,02 THEN
\***         set integer year to string year - 1
\***      ELSE
\***         set integer year to string year
\***      ENDIF
\***      set integer month to MOD((string month + 9),12) + 1
\***      set integer day to string day
\***                                                                                 
\***! The check for the century has been moved to check the modified year
\***! and replace the modified year of -1 to 99 which occured in the first
\***! two months of 2000  GSB     
\-------------------------------------------------------------------------------

      IF DATE.RETURN.CODE% <> 0                                        \
      OR F13.RETURN.CODE%  <> 0 THEN                                   \
         GOTO FUNCTION.EXIT      

      IF MONTH$ = "01"                                                 \
      OR MONTH$ = "02" THEN                                            \
         MODIFIED.YEAR = VAL(YEAR$) - 1                                \
      ELSE                                                             \
         MODIFIED.YEAR = VAL(YEAR$)

      IF MODIFIED.YEAR < 0 THEN BEGIN      ! YEAR 2000                   GSB
         MODIFIED.YEAR = 99
         CURRENT.CENTURY = 19
      ENDIF ELSE BEGIN
     
         IF MODIFIED.YEAR < 85 THEN                                      \
            CURRENT.CENTURY = 20                                         \
         ELSE                                                            \
            CURRENT.CENTURY = 19                                       
      ENDIF

      MODIFIED.MONTH = MOD((VAL(MONTH$) + 9),12) + 1

      CURRENT.DAY = VAL(DAY$)

\-------------------------------------------------------------------------------
\***
\***      calculate the day of week number as =
\***      MOD ((INT ((2.6 * integer month) - 0.2)
\***           + integer day
\***           + integer year
\***           + INT (integer century  / 4)
\***           + INT (integer year / 4)
\***           - (2 * integer century)),7)                         
\***
\***! The final calculation - calculating the MOD(n,7) has 42 (a multiple of
\***! 7) added into the calculation because in y2k n could be a negative number   
\***! (down to -32) and the MOD function returned an invalid value.
\***
\-------------------------------------------------------------------------------

      NUM.1     = (INT((2.6 * MODIFIED.MONTH) - 0.2))
      NUM.2     = (CURRENT.DAY + MODIFIED.YEAR)
      NUM.3     = (INT(CURRENT.CENTURY / 4)) 
      NUM.4     = (INT(MODIFIED.YEAR / 4))
      NUM.5     = ((2 * CURRENT.CENTURY))  
      TOTAL.SUM = NUM.1 + NUM.2 + NUM.3 + NUM.4 - NUM.5

      DAY.NUMBER = MOD((TOTAL.SUM + 42),7)                                ! GSB

\-------------------------------------------------------------------------------
\***
\***      on case of
\***         the day of week number = 0 THEN
\***            F13.DAY$ = 'SUN'
\***
\***         the day of week number = 1 THEN
\***            F13.DAY$ = 'MON'
\***
\***         the day of week number = 2 THEN
\***            F13.DAY$ = 'TUE'
\***
\***         the day of week number = 3 THEN
\***            F13.DAY$ = 'WED'
\***
\***         the day of week number = 4 THEN
\***            F13.DAY$ = 'THU'
\***
\***         the day of week number = 5 THEN
\***            F13.DAY$ = 'FRI'
\***
\***         the day of week number = 6 THEN
\***            F13.DAY$ = 'SAT'
\***      end case
\***
\***   EXIT FUNCTION
\***
\-------------------------------------------------------------------------------

      IF DAY.NUMBER = 0 THEN                                          \
         F13.DAY$ = "SUN"                                              \
      ELSE                                                             \
         IF DAY.NUMBER = 1 THEN                                       \
            F13.DAY$ = "MON"                                           \
         ELSE                                                          \
            IF DAY.NUMBER = 2 THEN                                    \
               F13.DAY$ = "TUE"                                        \
            ELSE                                                       \
               IF DAY.NUMBER = 3 THEN                                 \
                  F13.DAY$ = "WED"                                     \
               ELSE                                                    \
                  IF DAY.NUMBER = 4 THEN                              \
                     F13.DAY$ = "THU"                                  \
                  ELSE                                                 \
                     IF DAY.NUMBER = 5 THEN                           \
                        F13.DAY$ = "FRI"                               \
                     ELSE                                              \
                        F13.DAY$ = "SAT"

   FUNCTION.EXIT:
   
      PSDATE = F13.RETURN.CODE%                                        ! FAW

      EXIT FUNCTION

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\************************* subroutines follow **********************************
\*******************************************************************************
\***
\***  LOG.INTERNAL.ERROR:
\***
\***     Provided that a message is required,
\***     (ie. not a date that has been keyed in at a screen)
\***     CALL APPLICATION.LOG function to log error number 706
\***
\***     RETURN
\***
\-------------------------------------------------------------------------------

   LOG.INTERNAL.ERROR:

      IF F13.DISPLAY.MESSAGE$ = "N" THEN                                \ EJAL
         RETURN                                                         ! EJAL
         
      EVENT.NO%       = 4
      INTEGER4%       = 706
      F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                    \ FAW
                                         INTEGER4%)
      IF F17.RETURN.CODE% = 0 THEN                                     \
         MESSAGE$ = F17.RETURNED.STRING$                              :\
         INTEGER4% = LEN(INPUT.DATE$)                                 :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ FAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            VAR.STRING.1$ = RIGHT$(MESSAGE$,2)                         \
                          + RIGHT$(F17.RETURNED.STRING$,1)             \
                          + INPUT.DATE$                               :\
            VAR.STRING.2$ = "13" + STR$(LEN(INPUT.DATE$))              \
                          + INPUT.DATE$                               :\
            RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,              \ FAW
                                         VAR.STRING.1$,                \
                                         VAR.STRING.2$,                \
                                         EVENT.NO%)

      RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** ERROR.DETECTED:
\***
\***   set F13.RETURN.CODE% to 1
\***
\***   IF the returned error code is "OM" THEN         REM out of memory
\***      CALL ADXERROR to log the error
\***   ELSE
\***      IF the returned error code is "CT", or "CM" THEN   REM chain failure
\***         set VAR.STRING.1$ to "BF13 " + (the 3rd byte of MODULE.NUMBER$)
\***                     + "50  "
\***         set VAR.STRING.2$ to "PS" + (3rd byte of MODULE.NUMBER$) + "50"
\***         CALL APPLICATION.LOG message number 553
\***      ELSE
\***         IF the returned error code is "IH" THEN        REM non-numeric data
\***            CALL APPLICATION.LOG to log message number 706
\***            RESUME to the label FUNCTION.EXIT
\***         ELSE
\***            CALL APPLICATION.LOG to log message number 550
\***         ENDIF
\***      ENDIF
\***   ENDIF
\***
\***   IF program is not a screen program THEN
\***      STOP
\***   ENDIF
\***
\***   %INCLUDE PSBCHNE.J86
\***
\*** END FUNCTION
\***
\-------------------------------------------------------------------------------

   ERROR.DETECTED:

      F13.RETURN.CODE% = 1

\ 3 lines deleted from here                                            ! CBAAS

      IF ERR <> "IH" AND ERR <> "CM" AND ERR <> "CT" THEN              \
         EVENT.NO% = 1                                                :\
         INTEGER4% = ERRN                                             :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ FAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            ERRNUM$   = F17.RETURNED.STRING$                          :\
\ 8 lines deleted from here                                            \ CBAAS
            STRING.ERRL$ = PACK$(RIGHT$("000000" + STR$(ERRL),6))      ! CBAAS
\ 3 lines deleted from here                                            ! CBAAS

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

      IF ERR = "IH" THEN                          \REM non-numeric boots code
         MESSAGE.NUMBER% = 706                                        :\
         EVENT.NO%       = 4                                          :\
         INTEGER4%       = 706                                        :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ FAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            MESSAGE$ = F17.RETURNED.STRING$                           :\
            INTEGER4% = LEN(INPUT.DATE$)                              :\
            F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,              \ FAW
                                               INTEGER4%)             :\
            IF F17.RETURN.CODE% = 0 THEN                               \
               VAR.STRING.1$ = RIGHT$(MESSAGE$,2)                      \
                             + RIGHT$(F17.RETURNED.STRING$,1)          \
                             + INPUT.DATE$                            :\
               VAR.STRING.2$ = "13" + STR$(LEN(INPUT.DATE$))           \
                             + INPUT.DATE$                            :\
               RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,           \ FAW
                                            VAR.STRING.1$,             \
                                            VAR.STRING.2$,             \
                                            EVENT.NO%)                :\
               RESUME FUNCTION.EXIT

      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF13 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ DBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ DBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ FAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)

      IF ERR <> "OM" AND ERR <> "IH" AND                               \
         ERR <> "CM" AND ERR <> "CT" THEN                              \
         IF F17.RETURN.CODE% = 0 THEN                                  \
            MESSAGE.NUMBER% = 550                                     :\
            VAR.STRING.1$ = ERRNUM$ + CHR$(0) + ERR + STRING.ERRL$    :\ CBAAS
            INTEGER4% = ERRN                                          :\
            RET.CODE% = CONV.TO.HEX (INTEGER4%)                       :\ FAW
            IF RET.CODE% = 0 THEN                                      \ FAW
               VAR.STRING.2$ = ERR + F16.HEX.STRING$                   \
                             + "  0" + STR$(ERRL)                     :\ CBAAS
               EVENT.NO%     = 1                                      :\
               RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,           \ FAW
                                            VAR.STRING.1$,             \
                                            VAR.STRING.2$,             \
                                            EVENT.NO%)

\ 5 lines deleted from here                                            ! DBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN STOP                           ! DBCW

      %INCLUDE PSBCHNE.J86                                            ! DBCW

   END FUNCTION

