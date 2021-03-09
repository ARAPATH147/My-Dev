rem\
\*******************************************************************************
\*******************************************************************************
\***
\***
\***         FUNCTION      : CALC.BOOTS.CODE.CHECK.DIGIT
\***         AUTHOR        : Barbara Holbrook (Pseudocode)
\***                       :                  (Basic Code)
\***         DATE WRITTEN  : 29th January 1987  (Pseudocode)
\***                       :                    (Basic Code)
\***
\***         REFERENCE     : PSBF18
\***
\***
\***         VERSION C.    B.A.A.SCRIVER       19th May 1988
\***         STOCK SYSTEM VERSION.
\***         Change to set file session number part of unique data for error
\***         calls to zero, as no files are accessed.
\***         New message number 550 replaces message number 551.
\***
\***         VERSION D.    D.S. O'DARE (Pseudocode)    24th November 1988
\***                       B.C. WILLIS (Basic)          1st December 1988
\***         89A MERGE (ie. small stores changes added to stocks changes).
\***         Replace the CHAIN statement with the new included code 
\***         (PSBCHNE.J86) and CHAIN.FILE.NAME$ with PSBCHN.PRG.  Amend
\***         program-to-chain-to from "01" to "50".
\***
\***         VERSION E.    A. WEDGEWORTH                   7th July 1992
\***         Redundant function parameters removed.
\***
\***         VERSION F.    Jamie Thorpe                    4th August 2000
\***         Amended code so that international check digits can now be
\***         calculated. The program reads in a new value from the SOPTS
\***         file. If this value is set to 0, this implies that the UK
\***         check digit is in operation.
\***         Any non-zero value is the check digit for that country.
\***
\***         VERSION G.  Stuart William McConnachie       18th September 2000
\***         No error checking reading Store Options!
\***         No deallocate of Store Options session number!
\***         Corrected these errors.
\***
\***         Version H.      Stuart William McConnachie     31st Oct 2006
\***         Chain back to PSB50.286, instead of xxx50.286 derived from
\***         first three letters of MODULE.NUMBER$.  Doesn't work for
\***         PSD and SRP applications.
\***
\***         Version I.       Mark Walker                   19th Jul 2014
\***         Removed redundant international check digit processing.
\***
\*******************************************************************************
\*******************************************************************************

REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***                      FUNCTION OVERVIEW
\***                      -----------------
\***
\***         This function receives a 6 digit numeric string, and
\***      calculates the Boots check digit for the string following a
\***      modulus 11 checking routine.
\***      The value of the check digit is then returned to the calling
\***      program.
\***
\***
\*******************************************************************************
\*******************************************************************************

REM PSEUDOCODE for this function follows......

\*******************************************************************************
\*******************************************************************************
\***
\***
\*** %INCLUDE of globals for public function CALC.BOOTS.CODE.CHECK.DIGIT
\*** %INCLUDE of globals for public function CONV.TO.HEX
\*** %INCLUDE of globals for public function CONV.TO.STRING
\*** %INCLUDE of globals for screen chaining parameters (PSBUSEG.J86)
\*** %INCLUDE of statements for external function APPLICATION.LOG
\*** %INCLUDE of statements for external function ADXERROR
\*** %INCLUDE of statements for external function CONV.TO.HEX
\*** %INCLUDE of statements for external function CONV.TO.STRING
\***
\***
\-------------------------------------------------------------------------------

    %INCLUDE PSBF16G.J86
    %INCLUDE PSBF17G.J86
    %INCLUDE PSBF18G.J86
!   %INCLUDE PSBF20G.J86       !SESS.NUM.UTILITY                            !IMW
    %INCLUDE PSBUSEG.J86
!   %INCLUDE SOPTSDEC.J86      !STORE OPTIONS FILE DECLARATIONS             !IMW

    STRING GLOBAL                                                       \   !EAW
        BATCH.SCREEN.FLAG$,                                             \   !EAW
        MODULE.NUMBER$                                                      !EAW
!       F18.SOPTS.READ$                                                     !IMW

    %INCLUDE ADXERROR.J86
    %INCLUDE PSBF01E.J86
    %INCLUDE PSBF16E.J86
    %INCLUDE PSBF17E.J86
!   %INCLUDE PSBF20E.J86       !SESS.NUM.UTILITY                            !IMW
!   %INCLUDE SOPTSEXT.J86      !STORE OPTIONS FILE                          !IMW

\-------------------------------------------------------------------------------
\*******************************************************************************
\*******************************************************************************
\***
\***   Define function parameters and other variables used in the function.
\***
\-------------------------------------------------------------------------------

FUNCTION CALC.BOOTS.CODE.CHECK.DIGIT (BOOTS.CODE.6.DIGIT$) PUBLIC

    STRING                                                              \
        BOOTS.CODE.6.DIGIT$,                                            \
        CHECK.BYTE$,                                                    \
        ERR.IN.CODE$,                                                   \
\       F20.FUNCTION$,                                                  \   !IMW
\       F20.STRING$,                                                    \   !IMW
        MODULE.LETTER$,                                                 \
\       SOPTS.CHECK.DIGIT$,                                             \   !IMW
        STRING.ERRL$,                                                   \
        STRING.ERRN$,                                                   \
        UNIQUE$,                                                        \
        VAR.STRING.1$,                                                  \
        VAR.STRING.2$
                
                
    INTEGER*1                                                           \
        EVENT.NUM%,                                                     \
        MSGGRP%,                                                        \
        SEVERITY%

    INTEGER*2                                                           \
        CALC.BOOTS.CODE.CHECK.DIGIT,                                    \   !EAW
        F17.RETURN.CODE%,                                               \   !EAW
\       F20.INTEGER%,                                                   \   !IMW
        MESSAGE.NO%,                                                    \
        MSGNUM%,                                                        \
\       RC%,                                                            \   !IMW
        RET.CODE%,                                                      \
        TERM%

    INTEGER*4                                                           \
        COUNT%,                                                         \
        CHECK.DIGIT%,                                                   \
        DIGIT.COUNT%,                                                   \
        DIGIT.TOTAL%,                                                   \
        DIGIT.VALUE%,                                                   \
        INTEGER4%

     REAL                                                               \
         BOOTS.CODE.VAL

\-------------------------------------------------------------------------------
\*******************************************************************************
\***
\***   ON ERROR detected GOTO ERROR.DETECTED
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***   set variable string 1 to 10 spaces
\***
\***   set F18.CHECK.DIGIT$ to space
\***   set CALC.BOOTS.CODE.CHECK.DIGIT to 0
\***
\***   IF the length of string <> 6
\***
\***      set message number to 704
\***      set event to 4
\***      GOSUB to the label LOG.INTERNAL.ERROR
\***      EXIT FUNCTION
\***
\***   endif
\-------------------------------------------------------------------------------

    ON ERROR GOTO ERROR.DETECTED

    UNIQUE$ = "          "
    STRING.ERRN$ = "    "
    STRING.ERRL$ = "      "
    MODULE.LETTER$ = MID$(MODULE.NUMBER$,3,1)
   
!   GOSUB PROCESS.SOPTS                                                     !IMW
   
    CALC.BOOTS.CODE.CHECK.DIGIT = 0
    F18.CHECK.DIGIT$ = " "

    IF LEN(BOOTS.CODE.6.DIGIT$) <> 6 THEN BEGIN
        MESSAGE.NO% = 704
        EVENT.NUM% = 4
        GOSUB LOG.INTERNAL.ERROR
        EXIT FUNCTION
    ENDIF

\-------------------------------------------------------------------------------
\***
\***   set count to 0
\***   set error in code flag to "N"
\***
\***   WHILE error in code flag = "N"
\***     AND count < 6
\***
\***      ADD 1 to count
\***      set check field to part of string starting at count for 1
\***      IF check field = "+" or "-" or " " or "."
\***         set event to 4
\***         set message number to 704
\***         GOSUB to the label LOG.INTERNAL.ERROR
\***         set error in code flag to "Y"
\***      endif
\***
\***   WEND
\***
\***   IF error in code flag  = "N" THEN
\***      determine the real value of the 6 digit Boots code
\***         (VAL(6 digit Boots code))
\***   ELSE
\***      EXIT FUNCTION
\***   endif
\***
\-------------------------------------------------------------------------------

    COUNT% = 0
    ERR.IN.CODE$ = "N"

    WHILE ERR.IN.CODE$ = "N" AND COUNT% < 6

        COUNT% = COUNT% + 1
        CHECK.BYTE$ = MID$(BOOTS.CODE.6.DIGIT$,COUNT%,1)
        IF CHECK.BYTE$ = "+" OR CHECK.BYTE$ = "-" OR                     \
            CHECK.BYTE$ = " " OR CHECK.BYTE$ = "." THEN BEGIN
            MESSAGE.NO% = 704
            EVENT.NUM% = 4
            GOSUB LOG.INTERNAL.ERROR
            ERR.IN.CODE$ = "Y"
        ENDIF

    WEND

    IF ERR.IN.CODE$ = "N" THEN BEGIN
        BOOTS.CODE.VAL = VAL(BOOTS.CODE.6.DIGIT$)
    ENDIF ELSE BEGIN
        EXIT FUNCTION
    ENDIF

\-------------------------------------------------------------------------------
\***
\***   set DIGIT.TOTAL% TO 0
\***   FOR digit count 1 to 6
\***
\***       set DIGIT.VALUE% to ((part of Boots code starting at digit
\***                      count for 1) * (8 - digit count))
\***       add DIGIT.VALUE% to DIGIT.TOTAL%
\***
\***   NEXT digit count
\***   calculate CHECK.DIGIT% = 11 - (remainder of DIGIT.TOTAL% / 11)
\***
\***   IF CHECK.DIGIT% = 11
\***      set F18.CHECK.DIGIT$ to "0"
\***   ELSE
\***      IF CHECK.DIGIT% = 10
\***         set F18.CHECK.DIGIT$ to "A"
\***      ELSE
\***         set F18.CHECK.DIGIT$ to CHECK.DIGIT%
\***      endif
\***   endif
\***
\***   EXIT FUNCTION
\***
\***
\-------------------------------------------------------------------------------

    DIGIT.TOTAL% = 0

    FOR DIGIT.COUNT% = 1 TO 6 STEP 1

        DIGIT.VALUE% = VAL(MID$(BOOTS.CODE.6.DIGIT$,DIGIT.COUNT%,1))    \
                     * (8 - DIGIT.COUNT%)
        DIGIT.TOTAL% = DIGIT.TOTAL% + DIGIT.VALUE%

    NEXT DIGIT.COUNT%

    CHECK.DIGIT% = 11 - MOD(DIGIT.TOTAL%,11)

    IF CHECK.DIGIT% = 11 THEN BEGIN                                    
        F18.CHECK.DIGIT$ = "0"                                              !IMW
    ENDIF ELSE BEGIN
        IF CHECK.DIGIT% = 10 THEN BEGIN
!           IF VAL(SOPTS.CHECK.DIGIT$) <> 0 THEN BEGIN                      !IMW
!               F18.CHECK.DIGIT$ = SOPTS.CHECK.DIGIT$                       !IMW
!           ENDIF ELSE BEGIN                                                !IMW
!               F18.CHECK.DIGIT$ = "A"                                      !IMW
!           ENDIF                                                           !IMW
            F18.CHECK.DIGIT$ = "A"                                          !IMW
        ENDIF ELSE BEGIN    
            F18.CHECK.DIGIT$ = STR$(CHECK.DIGIT%)
        ENDIF
    ENDIF
       
    EXIT FUNCTION


\*******************************************************************************
\************************ subroutines follow ***********************************

!\**************************************************************************!IMW
!\***                                                                       !IMW
!\***   SUBROUTINE: PROCESS.SOPTS                                           !IMW
!\***                                                                       !IMW
!\**************************************************************************!IMW
!                                                                           !IMW
!   PROCESS.SOPTS:                                                          !IMW
!                                                                           !IMW
!   IF F18.SOPTS.READ$ <> "Y" THEN BEGIN                                    !IMW
!                                                                           !IMW
!       SOPTS.CHECK.DIGIT$ = "0"                                            !IMW
!                                                                           !IMW
!       CALL SOPTS.SET                                                      !IMW
!                                                                           !IMW
!       F20.FUNCTION$ = "O"                                                 !IMW
!       F20.STRING$   = SOPTS.FILE.NAME$                                    !IMW
!       F20.INTEGER%  = SOPTS.REPORT.NUM%                                   !IMW
!       GOSUB CALL.SESS.NUM.UTILITY                                         !IMW
!       SOPTS.SESS.NUM% = F20.INTEGER.FILE.NO%                              !IMW
!                                                                           !IMW
!       IF END # SOPTS.SESS.NUM% THEN NO.SOPTS.FILE                         !IMW
!       OPEN SOPTS.FILE.NAME$ RECL SOPTS.RECL% AS                           !IMW
!            SOPTS.SESS.NUM% NOWRITE NODEL                                  !IMW
!                                                                           !IMW
!       SOPTS.REC.NUM% = 96                                                 !IMW
!       RC% = READ.SOPTS                                                    !IMW
!                                                                           !IMW
!       IF RC% = 0 THEN BEGIN                                               !IMW
!           SOPTS.CHECK.DIGIT$ = LEFT$(SOPTS.RECORD$,1)                     !IMW
!       ENDIF                                                               !IMW
!                                                                           !IMW
!       CLOSE SOPTS.SESS.NUM%                                               !IMW
!                                                                           !IMW
!   NO.SOPTS.FILE:                                                          !IMW
!       F20.FUNCTION$ = "C"                                                 !IMW
!       F20.STRING$   = ""                                                  !IMW
!       F20.INTEGER%  = SOPTS.SESS.NUM%                                     !IMW
!       GOSUB CALL.SESS.NUM.UTILITY                                         !IMW
!                                                                           !IMW
!       F18.SOPTS.READ$ = "Y"                                               !IMW
!                                                                           !IMW
!   ENDIF                                                                   !IMW
!                                                                           !IMW
!                                                                           !IMW
!   RETURN                                                                  !IMW
!                                                                           !IMW
!\**************************************************************************!IMW
!\***                                                                       !IMW
!\***    SUBROUTINE: CALL.SESS.NUM.UTILITY                                  !IMW
!\***                                                                       !IMW
!\**************************************************************************!IMW
!                                                                           !IMW
!                                                                           !IMW
!   CALL.SESS.NUM.UTILITY:                                                  !IMW
!                                                                           !IMW
!   RC% = SESS.NUM.UTILITY(F20.FUNCTION$,                               \   !IMW
!                          F20.INTEGER%,                                \   !IMW
!                          F20.STRING$ )                                    !IMW
!                                                                           !IMW
!   RETURN                                                                  !IMW

\*******************************************************************************
\***
\*** ERROR.DETECTED
\***
\***   set CALC.BOOTS.CODE.CHECK.DIGIT to 1 to indicate unsuccessful processing
\***
\***   IF ERR is "IH" THEN                         \REM non numeric digit
\***      set event to 4
\***      set message number to 704
\***      GOSUB LOG.INTERNAL.ERROR
\***      EXIT FUNCTION
\***   endif
\***
\***   IF ERR is not "CM" or "CT" THEN
\***      CALL CONV.TO.STRING function to set string errn to ERRN
\***      IF F17.RETURN.CODE% = 0 THEN
\***         set string errn to F17.RETURNED.STRING$
\***         set string errl to STR$ of errl
\***         pad string errl to the left with zeroes until it is 6 bytes long
\***      endif
\***   endif
\***
\***   IF ERR is "OM" THEN                         \REM out of memory
\***      IF F17.RETURN.CODE% = 0 THEN
\***         set up variables for ADXERROR call
\***         set unique data to string errn and CHR$ of 0 and ERR
\***                            and packed string errl
\***         CALL ADXERROR to log the error
\***      endif
\***   endif
\***
\***   IF ERR is "CM" or "CT" THEN                 \REM chain failure
\***      set VAR.STRING.1$ to "BF18 " + MODULE.LETTER$ + "50  "
\***      set VAR.STRING.2$ to "PS" + MODULE.LETTER$ + "50"
\***      CALL APPLICATION.LOG function to log event 18, message number 553
\***   endif
\***
\***   IF ERR is not "OM", "CM", "CT" or "IH" THEN
\***      IF F17.RETURN.CODE% = 0 THEN
\***         set variable string 1 to string errn and CHR$ of 0 and ERR
\***                                packed string errl
\***         CALL CONV.TO.HEX function to obtain hex equivalent of ERRN
\***         IF F16.RETURN.CODE% = 0 THEN
\***            set variable string 2 to ERR and F16.HEX.STRING$
\***            CALL APPLICATION.LOG to log event 1, message 550
\***         endif
\***      endif
\***   endif
\***
\***
\***   IF program is not screen program THEN
\***      STOP
\***   ENDIF
\*** 
\***   set PSBCHN.PRG to "ADX_UPGM:" + (leftmost 3 bytes of MODULE.NUMBER$)
\***                                      + "50.286"
\***
\***   %INCLUDE PSBCHNE.J86
\***
\-------------------------------------------------------------------------------

ERROR.DETECTED:

   CALC.BOOTS.CODE.CHECK.DIGIT = 1                                     ! EAW

    IF ERR = "IH" THEN                              \REM non numeric digit
       MESSAGE.NO% = 704                                               :\
       EVENT.NUM% = 4                                                  :\
       GOSUB LOG.INTERNAL.ERROR                                        :\
       EXIT FUNCTION

   IF ERR <> "CM" AND ERR <> "CT" THEN                                 \
      INTEGER4% = ERRN                                                :\
      F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NUM%,                   \ EAW
                                         INTEGER4%)                   :\
      IF F17.RETURN.CODE% = 0 THEN                                     \
         STRING.ERRN$ = F17.RETURNED.STRING$                          :\
         STRING.ERRL$ = PACK$(RIGHT$(STRING$(6,"0") + STR$(ERRL),6))   ! CBAAS

   IF ERR = "OM" THEN                         \REM out of memory
      IF F17.RETURN.CODE% = 0 THEN                                     \
         TERM%      = 0                                               :\
         MSGGRP%    = ASC("J")                                        :\
         MSGNUM%    = 0                                               :\
         SEVERITY%  = 3                                               :\
         EVENT.NUM% = 1                                               :\
         UNIQUE$    = STRING.ERRN$ + CHR$(0) + ERR + STRING.ERRL$     :\ CBAAS
         RET.CODE%  = ADXERROR (TERM%,                                 \
                                MSGGRP%,                               \
                                MSGNUM%,                               \
                                SEVERITY%,                             \
                                EVENT.NUM%,                            \
                                UNIQUE$)

   IF ERR = "CM" OR ERR = "CT" THEN                \REM chain failure
      MESSAGE.NO% = 553                                               :\
      VAR.STRING.1$ = "BF18 " + MODULE.LETTER$ + "50  "               :\ DBCW
      VAR.STRING.2$ = "PS" + MODULE.LETTER$ + "50"                    :\ DBCW
      EVENT.NUM% = 18                                                 :\
      RET.CODE% = APPLICATION.LOG (MESSAGE.NO%,                        \ EAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NUM%)

   IF ERR <> "OM" AND ERR <> "CM" AND ERR <> "CT" AND ERR <> "IH" THEN \
      IF F17.RETURN.CODE% = 0 THEN                                     \
         MESSAGE.NO% = 550                                            :\ CBAAS
         VAR.STRING.1$ = STRING.ERRN$ + CHR$(0) + ERR + STRING.ERRL$  :\ CBAAS
         INTEGER4% = ERRN                                             :\
         RET.CODE% = CONV.TO.HEX (INTEGER4%)                          :\ EAW
         IF RET.CODE% = 0 THEN                                         \ EAW
            VAR.STRING.2$ = ERR + F16.HEX.STRING$ +                    \
                            "  0" + STR$(ERRL)                        :\ CBAAS
            EVENT.NUM% = 1                                            :\
            RET.CODE% = APPLICATION.LOG (MESSAGE.NO%,                  \ EAW
                                         VAR.STRING.1$,                \
                                         VAR.STRING.2$,                \
                                         EVENT.NUM%)

      IF BATCH.SCREEN.FLAG$ <> "S" THEN STOP                           ! DBCW

      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                               ! HSWM
      %INCLUDE PSBCHNE.J86                                            ! DBCW

\*******************************************************************************
\***
\*** LOG.INTERNAL.ERROR:
\***
\***   CALL APPLICATION.LOG function to log error number 704, event 4
\***
\***   set CALC.BOOTS.CODE.CHECK.DIGIT to 1
\***
\***   RETURN
\***
\-------------------------------------------------------------------------------

LOG.INTERNAL.ERROR:

   INTEGER4% = MESSAGE.NO%                                            :\
   F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NUM%,                      \ EAW
                                      INTEGER4%)
   IF F17.RETURN.CODE% = 0 THEN                                        \
      VAR.STRING.1$ = RIGHT$(F17.RETURNED.STRING$,2) +                 \
                      CHR$(LEN(BOOTS.CODE.6.DIGIT$)) +                 \
                      LEFT$(BOOTS.CODE.6.DIGIT$ +                      \
                            PACK$(STRING$(14," ")),7)                 :\
      VAR.STRING.2$ = "18" +                                           \
                    RIGHT$("00" + STR$(LEN(BOOTS.CODE.6.DIGIT$)),2) +  \
                    BOOTS.CODE.6.DIGIT$                               :\
      RET.CODE% = APPLICATION.LOG (MESSAGE.NO%,                        \ EAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NUM%)

   CALC.BOOTS.CODE.CHECK.DIGIT = 1                                     ! EAW

   RETURN

\-------------------------------------------------------------------------------
\***
\*** END FUNCTION
\***
\-------------------------------------------------------------------------------

END FUNCTION
