rem\
\*******************************************************************************
\*******************************************************************************
\***
\***
\***        FUNCTION      : CONV.TO.HEX
\***        AUTHOR        : Stephen Kelsey (Pseudocode)
\***                      : Bruce Scriver  (Basic Code)
\***        DATE WRITTEN  : 21st February 1986 (Pseudocode)
\***                      : 25th February 1986 (Basic Code)
\***
\***        REFERENCE     : PSBF16
\***
\***
\***         VERSION D.    B.A.A.SCRIVER       19th May 1988
\***         STOCK SYSTEM CHANGES.
\***         Change to set file session number part of unique data for error
\***         calls to zero, as no files are accessed.
\***         New message number 550 replaces message number 551.
\***
\***         VERSION E.    D.S. O'DARE (Pseudocode)     24th November 1988
\***                       B.C. WILLIS (Basic code)      1st December 1988
\***         89A MERGE (ie. small stores changes added to stocks changes).
\***         Replace the CHAIN statement with the new included code 
\***         (PSBCHNE.J86) and CHAIN.FILE.NAME$ with PSBCHN.PRG.  Amend
\***         program-to-chain-to from "01" to "50".
\***
\***         VERSION F.    A. WEDGEWORTH                     2nd July 1992
\***         Redundant function parameters removed and defined as global 
\***         variables.
\***
\***         VERSION G.   STUART WILLIAM MCCONNACHIE         2nd Sept 2005
\***         Removed version numbered included code - About time.
\***         This is so we can compile FUNLIB version without line numbers.
\***
\***         Version H.      Stuart William McConnachie     31st Oct 2006
\***         Chain back to PSB50.286, instead of xxx50.286 derived from
\***         first three letters of MODULE.NUMBER$.  Doesn't work for
\***         PSD and SRP applications.
\***            
\*******************************************************************************
\*******************************************************************************

REM pseudocode follows...

\*******************************************************************************
\*******************************************************************************
\***
\***
\***                   FUNCTION OVERVIEW
\***                   -----------------
\***
\***        This function is passed a four byte integer, which it then
\***     transforms into a string of its hex equivalent in ASCII.
\***     NOTE : this is an extended version of the I.B.M. function ERRFX$.
\***
\***
\*******************************************************************************
\*******************************************************************************
\***
\***  %INCLUDE of globals for external function CONV.TO.STRING
\***  %INCLUDE of globals for public function CONV.TO.HEX
\***  %INCLUDE of globals for screen chaining parameters (PSBUSEG.J86)
\***
\***  %INCLUDE of statements for external function ADXERROR
\***  %INCLUDE of statements for external function APPLICATION.LOG
\***  %INCLUDE of statements for external function CONV.TO.STRING
\***
\-------------------------------------------------------------------------------

      ! 1 line deleted from here                                       ! FAW
      %INCLUDE PSBF16G.J86                                             ! GSWM
      %INCLUDE PSBF17G.J86                                             ! GSWM
      %INCLUDE PSBUSEG.J86                                             ! GSWM

      STRING GLOBAL                                                    \ FAW
             BATCH.SCREEN.FLAG$,                                       \ FAW
             MODULE.NUMBER$,                                           \ FAW
             OPERATOR.NUMBER$                                          ! FAW

      %INCLUDE ADXERROR.J86                                            ! GSWM
      %INCLUDE PSBF01E.J86                                             ! GSWM
      %INCLUDE PSBF17E.J86                                             ! GSWM

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***    Define the function parameters and the variables used by the program.
\***
\-------------------------------------------------------------------------------

   FUNCTION CONV.TO.HEX (INTEGER4%)                                    \
   PUBLIC
! 3 parameters removed from here                                       ! FAW   

      STRING    CHAIN.TO.PROG$,                                        \
\ 1 line deleted from here                                             \ FAW
\ 1 line deleted from here                                             \ DBAAS
                ERRNUM$,                                               \
\ 1 line deleted from here                                             \ DBAAS
\ 2 lines deleted from here                                            \ FAW
                STRING.ERRL$,                                          \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$

      INTEGER*1 EVENT.NUM%,                                            \
                NIBBLE.VALUE%,                                         \
                MSGGRP%,                                               \
                SEVERITY%,                                             \
                SHIFT.VALUE%

      INTEGER   CONV.TO.HEX,                                           \ FAW
                EVENT.NO%,                                             \
                F17.RET.CODE%,                                         \ FAW
                MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                RET.CODE%,                                             \
                TERM%

      INTEGER*4 F.MASK%,                                               \
                INTEGER4%,                                             \
                NIBBLE%

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   ON ERROR goto ERROR.DETECTED
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***   set variable string 1 to 10 spaces
\***   set message group to space
\***
\***   set F16.HEX.STRING$ to null
\***   set CONV.TO.HEX to 0
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO ERROR.DETECTED

      UNIQUE$ = "          "
      ERRNUM$ = "    "
\ 1 line deleted from here                                             ! DBAAS
      STRING.ERRL$ = "      "

      F16.HEX.STRING$  = ""
      CONV.TO.HEX = 0                                                  ! FAW

\-------------------------------------------------------------------------------
\***
\***   FOR value of count = 28 to 0 step -4
\***
\***      right shift the input integer by count to give nibble
\***      mask nibble with hex "000F" to give nibble value
\***      IF nibble value > 9 THEN
\***         add 55 to nibble value   /REM this then sets nibble value to A - F
\***      ELSE
\***         add 48 to nibble value   /REM this then sets nibble value to 0 - 9
\***      endif
\***
\***      place the character form (CHR$) of nibble value in nibble string
\***      string nibble string to F16.HEX.STRING$
\***
\***   NEXT count
\***
\***   EXIT FUNCTION
\***
\-------------------------------------------------------------------------------

      F.MASK% = 000FH

      FOR SHIFT.VALUE% = 28 TO 0 STEP -4

         NIBBLE% = SHIFT(INTEGER4%,SHIFT.VALUE%)
         NIBBLE.VALUE% = NIBBLE% AND F.MASK%

         IF NIBBLE.VALUE% > 9 THEN                                     \
            NIBBLE.VALUE% = NIBBLE.VALUE% + 55                         \
         ELSE                                                          \
            NIBBLE.VALUE% = NIBBLE.VALUE% + 48

         F16.HEX.STRING$ = F16.HEX.STRING$ + CHR$(NIBBLE.VALUE%)

      NEXT SHIFT.VALUE%

   EXIT FUNCTION

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\******************** subroutine follows ***************************************
\*******************************************************************************
\***
\*** ERROR.DETECTED:
\***
\***   IF the returned error code is OM (out of memory) THEN
\***      CALL ADXERROR to log the error
\***   ELSE
\***      IF the returned error code is CM or CT (chain error) THEN
\***         set VAR.STRING.1$ to "BF16 " + (the 3rd byte of MODULE.NUMBER$)
\***                       + "50  "
\***         set VAR.STRING.2$ to "PS" + (the 3rd byte of MODULE.NUMBER$)
\***                                                    + "50"
\***         CALL APPLICATION.LOG to log event number 18, message number 553
\***      ELSE
\***         CALL APPLICATION.LOG to log event number 1, message number 550
\***      ENDIF
\***   ENDIF
\***
\***   IF program is not screen program THEN
\***      STOP
\***   ENDIF
\***
\***   set PSBCHN.PRG to "ADX_UPGM:" + (leftmost 3 bytes of MODULE.NUMBER$)
\***                                                  + "50.286"
\***
\***   %INCLUDE PSBCHNE.J86
\***
\*** END FUNCTION
\***
\-------------------------------------------------------------------------------

   ERROR.DETECTED:

      CONV.TO.HEX = 1                                                  ! FAW

      IF ERR <> "CM" AND ERR <> "CT" THEN                              \
         EVENT.NO% = 1                                                :\
         INTEGER4% = ERRN                                             :\
         F17.RET.CODE% = CONV.TO.STRING (EVENT.NO%,                    \ FAW
                                         INTEGER4%)                   :\
         IF F17.RET.CODE% = 0 THEN                                     \ FAW
            ERRNUM$   = F17.RETURNED.STRING$                          :\
\ 8 lines deleted from here                                            \ DBAAS
            STRING.ERRL$ = PACK$(RIGHT$("000000" + STR$(ERRL),6))      ! DBAAS
\ 3 lines deleted from here                                            ! DBAAS

      IF ERR = "OM" THEN                                      \REM out of memory
         IF F17.RET.CODE% = 0 THEN                                     \
            TERM%         = 0                                         :\
            MSGGRP%       = ASC("J")                                  :\
            MSGNUM%       = 0                                         :\
            SEVERITY%     = 3                                         :\
            EVENT.NUM%    = 1                                         :\
            UNIQUE$       = ERRNUM$ + CHR$(0) + ERR + STRING.ERRL$    :\ DBAAS
            RET.CODE%    = ADXERROR (TERM%,                            \
                                     MSGGRP%,                          \
                                     MSGNUM%,                          \
                                     SEVERITY%,                        \
                                     EVENT.NUM%,                       \
                                     UNIQUE$)

      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF16 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ EBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ EBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ FAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)
      ! 3 parameters removed from here                                 ! FAW                      

      IF ERR <> "OM" AND ERR <> "CM" AND ERR <> "CT" THEN              \
         IF F17.RET.CODE% = 0 THEN                                     \
            MESSAGE.NUMBER% = 550                                     :\ DBAAS
            VAR.STRING.1$ = ERRNUM$ + CHR$(0) + ERR + STRING.ERRL$    :\ DBAAS
            VAR.STRING.2$ = STR$(ERRN)                                :\
            EVENT.NO%     = 1                                         :\
            RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,              \ FAW
                                         VAR.STRING.1$,                \
                                         VAR.STRING.2$,                \
                                         EVENT.NO%)
      ! 3 parameters deleted from here                                 ! FAW                     

\ 5 lines deleted from here                                            ! EBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN STOP                           ! EBCW

      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                ! HSWM
      %INCLUDE PSBCHNE.J86                                             ! GSWM

   END FUNCTION

