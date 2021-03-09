REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***            FUNCTION      : APPLICATION.LOG
\***            AUTHOR        : Bruce Scriver  (Pseudocode)
\***                            Stephen Kelsey (Basic code)
\***            DATE WRITTEN  : 21st January 1986  (Pseudocode)
\***                            18th February 1986 (Basic code)
\***
\***            REFERENCE     : PSBF01
\***
\***
\***            VERSION E.    B.A.A.SCRIVER       19th May 1988
\***            STOCK SYSTEM CHANGES.
\***            Change to use session number table to assign file session
\***            numbers as for PSSF20 - SESS.NUM.UTILITY.
\***
\***            VERSION F.    D.S.O'DARE   (Pseudocode)   22nd November, 1988
\***                          B.C.  WILLIS (Basic code)   29th November, 1988  
\***            89A MERGE.(ie. small stores changes added to stocks changes).
\***            CHAIN statement replaced with new included code (PSBCHNE.J86).
\***            Change subroutine CHAIN.MODULE.01 to CHAIN.MODULE.50.
\***            Replace the setting up of CHAIN parameters and the CHAIN 
\***            statement with a GOTO CHAIN.MODULE.50 statement.
\***            
\***            Version G.          Andrew Wedgeworth           1st July 1992             
\***            BATCH.SCREEN.FLAG$ and MODULE.NUMBER$ are defined globally in
\***            the function and any program which calls it, rather than being 
\***            passed as parameters to the function.  Also, the return code
\***            is now the function's name.
\***
\***            Version H.      Stuart William McConnachie      2nd Sep 2005
\***            Removed version numbered included code - About time.
\***            This is so we can compile FUNLIB version without line numbers.
\***            
\***            Version I.      Stuart William McConnachie     31st Oct 2006
\***            Chain back to PSB50.286, instead of xxx50.286 derived from
\***            first three letters of MODULE.NUMBER$.  Doesn't work for
\***            PSD and SRP applications.
\***            
\*******************************************************************************
\*******************************************************************************

REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***            FUNCTION OVERVIEW
\***            -----------------
\***
\***   This function is called to log an event on the system error log, and to
\***   display the associated message if in a screen program.   The error is
\***   first logged, and then if the calling program is a screen program, the
\***   associated message text is retreived from a message file and displayed
\***   in the message window on the screen.  If the display of a message
\***   fails, the display manager file is closed and the message is written to
\***   the screen directly, after which the function chains back to the first
\***   module of the system.
\***
\***
\*******************************************************************************
\*******************************************************************************

REM    PSEUDOCODE for this module follows....\

\*****************************************************************************
\*****************************************************************************
\***
\***
\***   %INCLUDE global definitions for SESS.NUM.UTILITY function
\***   %INCLUDE global definitions for screen chaining parameters (PSBUSEG.J86)
\***   %INCLUDE statements for BEMF
\***   %INCLUDE external definition for ADXERROR function
\***   %INCLUDE external definition of display manager calls (NB NOT Boots fn.)
\***
\------------------------------------------------------------------------------
    
   \ 1 line deleted from here                                          \ GAW

!  %INCLUDE PSBF20G.J86                                                ! HSWM
   
   STRING GLOBAL                                                       \ GAW
          BATCH.SCREEN.FLAG$,                                          \ GAW
          MODULE.NUMBER$                                               ! GAW                

   %INCLUDE PSBUSEG.J86                                                ! HSWM


   %INCLUDE BEMFDEC.J86                                                ! HSWM

   %INCLUDE PSBF20G.J86                                                ! HSWM

!HSWM  %INCLUDE BEMFNUMB.J86


!HSWM  %INCLUDE BEMFFNSB.J86


   %INCLUDE ADXERROR.J86                                               ! HSWM


   %INCLUDE DMEXTR.J86

   %INCLUDE BEMFEXT.J86                                                ! HSWM

   %INCLUDE PSBF20E.J86                                                ! HSWM

\------------------------------------------------------------------------------
\***
\***   FUNCTION APPLICATION.LOG(message number,
\***                            variable string 1, variable string 2,
\***                            event number)
\***
\***   INTEGER message number, event number
\***
\***   STRING  variable string 1, variable string 2
\***
\------------------------------------------------------------------------------

   FUNCTION APPLICATION.LOG (MESSAGE.NO%,                              \
                             VAR.STRING.1$,                            \
                             VAR.STRING.2$,                            \
                             EVENT.NO%)  PUBLIC

   \ 3 parameters no longer reveived from calling program              \ GAW

   STRING BEMF.OPEN.FLAG$,                                             \ EBAAS
          BEMF.REC.NO.STR$,                                            \
          CHAIN.FILE.NAME$,                                            \
          CHAIN.MODULE$,                                               \
\ 2 lines deleted from here                                            \ EBAAS
          CURSOR.SETTING$,                                             \
\ 1 line deleted from here                                             \ EBAAS
          ERROR.STRING$,                                               \
          FIELD.NO.STR$,                                               \
          IN.FIELD.FLAG$,                                              \
          INVISIBLE.CURSOR$,                                           \
\ 1 line deleted from here                                             \ EBAAS
\ 1 line deleted from here                                             \ GAW
          NORMAL.VIDEO$,                                               \
          NULL.FOUND.FLAG$,                                            \ EBAAS
          OPEN.READ$,                                                  \
\ 1 line deleted from here                                             \ GAW
\ 1 line deleted from here                                             \ EBAAS
          RETURNED.STRING$,                                            \
          REVERSE.VIDEO$,                                              \
          SCREEN.MESSAGE$,                                             \
          SETF.RETURN$,                                                \
          STAR.LINE$,                                                  \
          UNIQUE$,                                                     \
\ 1 line deleted from here                                             \ EBAAS
          VAR.STRING.1$,                                               \
          VAR.STRING.2$,                                               \
          VISIBLE.CURSOR$

   INTEGER*1 EVENT.NO%,                                                \
             EVENT.NUM%,                                               \
             MSGGRP%,                                                  \
             SEVERITY%

   INTEGER*2 APPLICATION.LOG,                                          \ GAW
             COUNT%,                                                   \
\ 1 line deleted from here                                             \ EBAAS
             DM.RET.CODE%,                                             \
             ESCAPE.KEY%,                                              \
             FIELD.NO%,                                                \
\H           INDEX%,                                                   \ EBAAS
             KEY.PRESSED%,                                             \
             MESSAGE.NO%,                                              \
             MSGNUM%,                                                  \
             TERM%

   INTEGER*4 ERROR.BYTE%,                                              \
             ERROR.LENGTH%,                                            \
             ERROR.VALUE%

\------------------------------------------------------------------------------
\***
\***   REM start of mainline code
\***
\***   ON ERROR GOTO ERROR.DETECTED
\***
\***   REM set up storage areas for ADXERROR call in event of memory overflow
\***   set variable string 1 to 10 spaces
\***
\***   %INCLUDE bemf set up code
\***   set APPLICATION.LOG to 0
\***
\***   check value of F20.TABLE.DIMENSIONED.FLAG$ - if it is not "Y",
\***   dimension the session number table to have 64 entries, not including
\***   the first entry, subscript zero; and set the flag to "Y"
\***
\***   IF batch/screen flag = "S" THEN
\***      set in field flag off
\***      use display manager POSF to establish initial cursor position
\***      IF value returned from POSF > 0 THEN
\***         set in field flag on
\***      endif
\***   endif
\***
\------------------------------------------------------------------------------

      ON ERROR GOTO ERROR.DETECTED

      UNIQUE$ = "          "

!H    %INCLUDE BEMFSETB.J86
      CALL BEMF.SET                                                    ! HSWM

      APPLICATION.LOG = 0

!H    IF F20.TABLE.DIMENSIONED.FLAG$ <> "Y" THEN                       \ EBAAS
!H       DIM SESS.NUM.TABLE$(64)                                      :\ EBAAS
!H       F20.TABLE.DIMENSIONED.FLAG$ = "Y"                             ! EBAAS

      ESCAPE.KEY% = 27
      VISIBLE.CURSOR$ = "0"
      INVISIBLE.CURSOR$ = "1"

      IF BATCH.SCREEN.FLAG$ = "S" THEN                                 \
         IN.FIELD.FLAG$ = "N"                                         :\
         FIELD.NO% = POSF(0)                                          :\
         IF FIELD.NO% > 0 THEN                                         \
            IN.FIELD.FLAG$ = "Y"

\------------------------------------------------------------------------------
\***
\***   GOSUB LOG.ERROR
\***   IF batch/screen flag is "S" THEN
\***      GOSUB DISPLAY.ROUTINE
\***
\***      IF in field flag is on THEN
\***         use display manager POSF to position cursor in initial position
\***      ELSE
\***         use display manager POSF to position cursor in first input field
\***      endif
\***   endif
\***
\***   IF bemf open flag is on THEN
\***      set bemf open flag off
\***      CLOSE bemf
\***   endif
\***
\***   set session number table entry for bemf to null
\***
\***   EXIT FUNCTION to calling program
\***   N.B. Calling program is now "50" and not "01", so change GOTO labels
\***        accordingly.
\***
\------------------------------------------------------------------------------

      EVENT.NUM% = EVENT.NO%
      UNIQUE$ = VAR.STRING.1$
      GOSUB LOG.ERROR

      IF BATCH.SCREEN.FLAG$ = "S" THEN                                 \
         GOSUB DISPLAY.ROUTINE                                        :\
         IF IN.FIELD.FLAG$ = "Y" THEN                                  \
            DM.RET.CODE% = POSF(FIELD.NO%)                            :\
            FIELD.NO.STR$ = STR$(FIELD.NO%)                           :\
            WHILE LEN(FIELD.NO.STR$) < 3                              :\
               FIELD.NO.STR$ = "0" + FIELD.NO.STR$                    :\
            WEND                                                      :\
            VAR.STRING.2$ = "10" + FIELD.NO.STR$                      :\
            UNIQUE$ = VAR.STRING.2$                                   :\
         ELSE                                                          \
            DM.RET.CODE% = NXTF(-20)                                  :\
            IF DM.RET.CODE% < 1 AND BATCH.SCREEN.FLAG$ = "S" THEN      \
               DM.RET.CODE% = NXTF(-20)                               :\
               VAR.STRING.2$ = "00-20"                                :\
               MESSAGE.NO% = 660                                      :\
               EVENT.NUM% = 20                                        :\
               GOSUB GET.ERROR.MESS                                   :\
               GOSUB LOAD.DISPLAY.MESSAGE                             :\
               GOSUB DISPLAY.FAILED                                   :\
               GOTO CHAIN.MODULE.50                                    ! FBCW

      IF DM.RET.CODE% < 1 AND BATCH.SCREEN.FLAG$ = "S" THEN            \
         MESSAGE.NO% = 653                                            :\
         EVENT.NUM% = 3                                               :\
         GOSUB GET.ERROR.MESS                                         :\
         GOSUB LOAD.DISPLAY.MESSAGE                                   :\
         GOSUB DISPLAY.FAILED                                         :\
         GOTO CHAIN.MODULE.50                                          ! FBCW

      IF BEMF.OPEN.FLAG$ = "Y" THEN                                    \ EBAAS
         BEMF.OPEN.FLAG$ = "N"                                        :\ EBAAS
         CLOSE BEMF.SESS.NUM%                                         :\ EBAAS
         CALL SESS.NUM.UTILITY ("C", BEMF.SESS.NUM%, "")               ! HSWM

!H    SESS.NUM.TABLE$(BEMF.SESS.NUM%) = ""                             ! EBAAS

      EXIT FUNCTION

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\*********************** subroutines follow ************************************
\*******************************************************************************
\***
\***   DISPLAY.ROUTINE:
\***
\***   GOSUB GET.ERROR.MESS
\***
\***   GOSUB LOAD.DISPLAY.MESSAGE
\***   GOSUB SHOW.MESSAGE
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   DISPLAY.ROUTINE:

      GOSUB GET.ERROR.MESS

      GOSUB LOAD.DISPLAY.MESSAGE
      GOSUB SHOW.MESSAGE

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   GET.ERROR.MESS:
\***
\***   search SESS.NUM.TABLE$ sequentially for first null entry
\***   NOTE: ignore entry 0, which is always null
\***
\***   If there is no null entry in the table, GOTO NO.MESSAGE.FOUND
\***
\***   set null entry to one byte character representing the integer value of
\***   the file reporting number (use CHR$ function) plus the passed string
\***   (the file logical name)
\***
\***   set bemf session number to the table entry number for the file
\***   set bemf open flag off
\***
\***   IF open fails on message file THEN NO.MESSAGE.FOUND
\***   OPEN message file NOWRITE NODEL
\***
\***   set bemf open flag on
\***
\***   IF read fails on message file THEN NO.MESSAGE.FOUND
\***   READ message file record with same number as message number
\***
\***   set bemf open flag off
\***   CLOSE message file
\***
\***   set session number table entry for bemf to null
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   GET.ERROR.MESS:

      OPEN.READ$ = "O"

!H    NULL.FOUND.FLAG$ = "N"                                           ! EBAAS
!H    INDEX% = 1                                                       ! EBAAS
!H
!H    WHILE NULL.FOUND.FLAG$ = "N"                                     \ EBAAS
!H      AND INDEX% < 65                                                ! EBAAS
!H
!H       IF SESS.NUM.TABLE$(INDEX%) = "" THEN                          \ EBAAS
!H          NULL.FOUND.FLAG$ = "Y"                                     \ EBAAS
!H       ELSE                                                          \ EBAAS
!H          INDEX% = INDEX% + 1                                        ! EBAAS
!H
!H    WEND                                                             ! EBAAS
!H
!H    IF NULL.FOUND.FLAG$ = "N" THEN                                   \ EBAAS
!H       GOTO NO.MESSAGE.FOUND                                         ! EBAAS
!H
!H    SESS.NUM.TABLE$(INDEX%) = CHR$(BEMF.REPORT.NUM%)                 \ EBAAS
!H                            + BEMF.FILE.NAME$                        ! EBAAS
!H
!H    BEMF.SESS.NUM% = INDEX%                                          ! EBAAS
      
      CALL SESS.NUM.UTILITY ("O", BEMF.REPORT.NUM%, BEMF.FILE.NAME$)   ! HSWM
      BEMF.SESS.NUM% = F20.INTEGER.FILE.NO%                            ! HSWM
      BEMF.OPEN.FLAG$ = "N"                                            ! EBAAS

      IF END #BEMF.SESS.NUM% THEN NO.MESSAGE.FOUND                   
      OPEN BEMF.FILE.NAME$ DIRECT RECL BEMF.RECL% AS BEMF.SESS.NUM% \
                NOWRITE NODEL

      BEMF.OPEN.FLAG$ = "Y"                                            ! EBAAS

      BEMF.REC.NO% = MESSAGE.NO%
!H    OPEN.READ$ = "R"
!H    IF END #BEMF.SESS.NUM% THEN NO.MESSAGE.FOUND
!H    CALL READ.BEMF
      IF READ.BEMF = 1 THEN GOTO NO.MESSAGE.FOUND                      ! HSWM

      BEMF.OPEN.FLAG$ = "N"                                            ! EBAAS

      CLOSE BEMF.SESS.NUM%

!H    SESS.NUM.TABLE$(BEMF.SESS.NUM%) = ""                             ! EBAAS
      CALL SESS.NUM.UTILITY ("C", BEMF.SESS.NUM%, "")                  ! HSWM

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   NO.MESSAGE.FOUND:
\***
\***   set up unique data for event number 6 (includes GOSUB CHAR.STRING.ERROR)
\***   GOSUB LOG.ERROR to log event number 6
\***
\***   set message to "B802 INTERNAL ERROR - can't retreive message @@@
\***                   from file @@"
\***           (replace @@@ with message number, @@ with file)
\***
\***   GOSUB SHOW.MESSAGE
\***
\***   IF bemf open flag is on THEN
\***      set bemf open flag off
\***      CLOSE bemf
\***   endif
\***
\***   set session number table entry for bemf to null
\***
\***   EXIT FUNCTION to calling program
\***
\------------------------------------------------------------------------------

   NO.MESSAGE.FOUND:

      EVENT.NUM% = 6
      ERROR.VALUE% = BEMF.REPORT.NUM%
      ERROR.STRING$ = CHR$(ERROR.VALUE%)
      BEMF.REC.NO.STR$ = STR$(BEMF.REC.NO%)
      WHILE LEN(BEMF.REC.NO.STR$) < 16
         BEMF.REC.NO.STR$ = "0" + BEMF.REC.NO.STR$
      WEND
      UNIQUE$ = OPEN.READ$ + RIGHT$(ERROR.STRING$,1) +                 \
                PACK$(BEMF.REC.NO.STR$)
      GOSUB LOG.ERROR

      SCREEN.MESSAGE$ = "B802 INTERNAL ERROR - can't retrieve message "\
                      + STR$(MESSAGE.NO%)                              \
                      + " from file "                                  \
                      + STR$(BEMF.REPORT.NUM%)

      GOSUB SHOW.MESSAGE

      IF BEMF.OPEN.FLAG$ = "Y" THEN                                    \ EBAAS
         BEMF.OPEN.FLAG$ = "N"                                        :\ EBAAS
         CLOSE BEMF.SESS.NUM%                                         :\ EBAAS
         CALL SESS.NUM.UTILITY ("C", BEMF.SESS.NUM%, "")               ! HSWM

!H    SESS.NUM.TABLE$(BEMF.SESS.NUM%) = ""                             ! EBAAS

      EXIT FUNCTION

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   LOG.ERROR:
\***
\***   CALL ADXERROR to log error using event number and variable string 1
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   LOG.ERROR:


      TERM% = 0
      MSGGRP% = 74
      MSGNUM% = 0
      SEVERITY% = 3
      CALL ADXERROR (TERM%,                                            \
                     MSGGRP%,                                          \
                     MSGNUM%,                                          \
                     SEVERITY%,                                        \
                     EVENT.NUM%,                                       \
                     UNIQUE$)
   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   LOAD.DISPLAY.MESSAGE:
\***
\***   set message to null
\***
\***   WHILE length of remaining message text is greater than 0
\***
\***      IF first character of message text is "@" THEN
\***         IF length of remaining variable string 2 is greater than 0 THEN
\***            set message to (message + first character of variable string 2)
\***            remove first character of variable string 2
\***         ELSE
\***            set message to (message + " ")
\***         endif
\***      ELSE
\***         set message to (message + first character of message text)
\***      endif
\***      remove first character of message text
\***
\***   WEND
\***
\------------------------------------------------------------------------------

   LOAD.DISPLAY.MESSAGE:

      SCREEN.MESSAGE$ = ""

      WHILE LEN(BEMF.MESSAGE$) > 0

         IF LEFT$(BEMF.MESSAGE$,1) = "@" THEN                          \
            IF LEN(VAR.STRING.2$) > 0 THEN                             \
               SCREEN.MESSAGE$ = SCREEN.MESSAGE$                       \
                               + LEFT$(VAR.STRING.2$,1)               :\
               VAR.STRING.2$ = RIGHT$(VAR.STRING.2$,                   \
                                      LEN(VAR.STRING.2$) - 1)         :\
            ELSE                                                       \
               SCREEN.MESSAGE$ = SCREEN.MESSAGE$ + " "                :\
         ELSE                                                          \
            SCREEN.MESSAGE$ = SCREEN.MESSAGE$ + LEFT$(BEMF.MESSAGE$,1)

         BEMF.MESSAGE$ = RIGHT$(BEMF.MESSAGE$,LEN(BEMF.MESSAGE$) - 1)

      WEND

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   SHOW.MESSAGE:
\***
\***   Again change the label CHAIN.MODULE.01 to CHAIN.MODULE.50
\***   as appropriate throughout this section.
\***
\***   use display manager POSF to place cursor in field 01
\***   IF return code < 0 THEN
\***      GOTO DISPLAY.FAILED
\***   ELSE
\***      use display manager SETF to set the field to inverse video
\***      use display manager PUTF to display the message
\***      IF return code <> 0 THEN
\***         GOTO DISPLAY.FAILED
\***      endif
\***   endif
\***
\***   position the cursor at the last input field (NXTF)
\***   IF the return code from NXTF  0 THEN
\***      GOSUB GET.ERROR.MESS to read message number 656
\***      GOSUB LOAD.DISPLAY.MESSAGE to set the message up
\***      GOTO DISPLAY.FAILED
\***   endif
\***
\***   set the cursor to invisible
\***   obtain the user input from the field (GETF)
\***
\***   WHILE the key pressed is not the escape key
\***      obtain the user input from the field (GETF)
\***   WEND
\***
\***   set message to spaces
\***   set cursor to visible (CURS)
\***   use display manager POSF to place cursor in field 01
\***   use display manager SETF to set the field to normal video
\***   use display manager PUTF to display the message
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   SHOW.MESSAGE:

      DM.RET.CODE% = POSF(1)

      IF DM.RET.CODE% < 0 THEN                                         \
         GOSUB DISPLAY.FAILED                                         :\
         UNIQUE$ = "10001"                                            :\
         EVENT.NUM% = 3                                               :\
         GOSUB LOG.ERROR                                              :\
         GOTO CHAIN.MODULE.50                                         :\ FBCW
      ELSE                                                             \
         REVERSE.VIDEO$ = "331"                                       :\
         SETF.RETURN$ = SETF(REVERSE.VIDEO$)                          :\
         DM.RET.CODE% = PUTF(SCREEN.MESSAGE$)                         :\
         IF DM.RET.CODE% <> 0 THEN                                     \
            GOSUB DISPLAY.FAILED                                      :\
            UNIQUE$ = "12001"                                         :\
            EVENT.NUM% = 3                                            :\
            GOSUB LOG.ERROR                                           :\
            GOTO CHAIN.MODULE.50                                       ! FBCW

      GOSUB MOVE.LAST.INPUT.FIELD

      CURSOR.SETTING$ = CURS(INVISIBLE.CURSOR$)

      RETURNED.STRING$ = GETF
      KEY.PRESSED% = ENDF

      WHILE KEY.PRESSED% <> ESCAPE.KEY%

         RETURNED.STRING$ = GETF
         KEY.PRESSED% = ENDF

      WEND

      CURSOR.SETTING$ = CURS(VISIBLE.CURSOR$)
      SCREEN.MESSAGE$ = " "

      DM.RET.CODE% = POSF(1)
      IF DM.RET.CODE% < 0 THEN                                         \
         VAR.STRING.2$ = "10001"                                      :\
         UNIQUE$ = VAR.STRING.2$                                      :\
         MESSAGE.NO% = 653                                            :\
         EVENT.NUM% = 3                                               :\
         GOSUB GET.ERROR.MESS                                         :\
         GOSUB LOAD.DISPLAY.MESSAGE                                   :\
         GOSUB DISPLAY.FAILED                                         :\
         GOTO CHAIN.MODULE.50                                          ! FBCW

      NORMAL.VIDEO$ = "330"
      SETF.RETURN$ = SETF(NORMAL.VIDEO$)

      DM.RET.CODE% = PUTF(SCREEN.MESSAGE$)
      IF DM.RET.CODE% < 0 THEN                                         \
         VAR.STRING.2$ = "12001"                                      :\
         UNIQUE$ = VAR.STRING.2$                                      :\
         MESSAGE.NO% = 653                                            :\
         EVENT.NUM% = 3                                               :\
         GOSUB GET.ERROR.MESS                                         :\
         GOSUB LOAD.DISPLAY.MESSAGE                                   :\
         GOSUB DISPLAY.FAILED                                         :\
         GOTO CHAIN.MODULE.50                                          ! FBCW


   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   MOVE.LAST.INPUT.FIELD:
\***
\***   use display manager NXTF to move the cursor to the last input field
\***   IF the display manager return code < 0 THEN
\***      set the message number to 656
\***      set the variable string 2 to "11020"
\***      GOSUB GET.ERROR.MESSAGE to read message 656
\***      GOSUB LOAD.DISPLAY.MESSAGE to insert variable string 2 into the
\***      message
\***      GOTO DISPLAY.FAILED to display the error on the screen
\***      Change CHAIN.MODULE.01 to CHAIN.MODULE.50
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   MOVE.LAST.INPUT.FIELD:

      DM.RET.CODE% = NXTF(20)
      IF DM.RET.CODE% < 0 THEN                                         \
         MESSAGE.NO% = 660                                            :\
         EVENT.NUM% = 20                                              :\
         GOSUB GET.ERROR.MESS                                         :\
         VAR.STRING.2$ = "00020"                                      :\
         UNIQUE$ = VAR.STRING.2$                                      :\
         GOSUB LOAD.DISPLAY.MESSAGE                                   :\
         GOSUB DISPLAY.FAILED                                         :\
         GOTO CHAIN.MODULE.50                                          ! FBCW

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   DISPLAY.FAILED:
\***
\***   use display manager CLRSCR to clear terminal screen
\***   use display manager CLSDIS to close the display file
\***   PRINT a row of stars on the screen
\***   PRINT a blank line on the screen
\***   PRINT the message on the screen
\***   PRINT a blank line on the screen
\***   PRINT a row of stars on the screen
\***
\***   get input using INKEY
\***   WHILE user input <> ESC key
\***      get input using INKEY
\***   WEND
\***
\***   chain back to Boots system main menu module
\***
\------------------------------------------------------------------------------

   DISPLAY.FAILED:

      DM.RET.CODE% = CLRSCR
      DM.RET.CODE% = CLSDIS

      STAR.LINE$ = ""
      FOR COUNT% = 1 TO 79 STEP 1
          STAR.LINE$ = STAR.LINE$ + "*"
      NEXT COUNT%

      PRINT STAR.LINE$
      PRINT " "
      PRINT SCREEN.MESSAGE$
      PRINT " "
      PRINT STAR.LINE$

      KEY.PRESSED% = INKEY

      WHILE KEY.PRESSED% <> 27
         KEY.PRESSED% = INKEY
      WEND

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   CHAIN.MODULE.50:
\***
\***   set chain module to first 3 bytes of passed module number
\***   set PSBCHN.PRG to "ADX_UPGM:" + chain module + "50.286"
\***
\***   chain back to Boots system using the PSBCHNE.J86 included code.
\***
\------------------------------------------------------------------------------

   CHAIN.MODULE.50:

      CHAIN.MODULE$ = MID$(MODULE.NUMBER$,1,3)
      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                            ! ISWM

      %INCLUDE PSBCHNE.J86                                         ! HSWM
   
\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   SET.UP.UNIQUE:
\***
\***   set error string to null
\***   set error value to ERRN
\***   set error length to 24
\***   GOSUB CHAR.STRING.ERROR
\***   set unique data to error string
\***
\***   set error string to null
\***   set error string to CHR$ of file number part of session number table
\***   entry for ERRF%
\***   set unique data to unique data + error string + ERR + packed ERRL
\***
\------------------------------------------------------------------------------

   SET.UP.UNIQUE:

      ERROR.STRING$ = ""
      ERROR.VALUE% = ERRN
      ERROR.LENGTH% = 24
      GOSUB CHAR.STRING.ERROR
      UNIQUE$ = ERROR.STRING$

      ERROR.STRING$ = ""                                               ! EBAAS
      IF SESS.NUM.TABLE$(ERRF%) = "" THEN                              \ EBAAS
         ERROR.STRING$ = CHR$(0)                                       \ EBAAS
      ELSE                                                             \ EBAAS
         ERROR.STRING$ = LEFT$(SESS.NUM.TABLE$(ERRF%),1)               ! EBAAS

      UNIQUE$ = UNIQUE$ + ERROR.STRING$ + ERR                          \ EBAAS
              + PACK$(RIGHT$("000000" + STR$(ERRL),6))                 ! EBAAS

\ 3 lines deleted from here                                            ! EBAAS

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   CHAR.STRING.ERROR:
\***
\***   FOR count = error length to 0 step -8
\***       error byte = SHIFTed error value by count
\***       error string = error string + the character format of error byte
\***   NEXT count
\***
\***   RETURN
\***
\------------------------------------------------------------------------------

   CHAR.STRING.ERROR:

      FOR COUNT% = ERROR.LENGTH% TO 0 STEP -8

         ERROR.BYTE% = SHIFT(ERROR.VALUE%,COUNT%)
         ERROR.STRING$ = ERROR.STRING$ + CHR$(ERROR.BYTE%)

      NEXT COUNT%

   RETURN

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***   on case of
\***      ERR is CM or CT THEN             \REM chain failure
\***         CALL ADXERROR to log event number 18
\***         GOTO CHAIN.MODULE.50
\***         (setting up CHAIN.MODULE$ and PSBCHN.PRG paramaters as before)
\***
\***      ERR is NL or CU                  \REM close failure
\***         GOSUB SET.UP.UNIQUE
\***         CALL ADXERROR to log event number 1
\***         RESUME
\***
\***      ERR <> CM, CT, NL or CU THEN
\***         GOSUB SET.UP.UNIQUE
\***         CALL ADXERROR to log event number 1
\***         IF batch/screen flag is "S" THEN
\***            find module name from calling program
\***            GOTO CHAIN.MODULE.50
\***             (setting up CHAIN.MODULE$ and PSBCHN.PRG paramaters as before)
\***         ELSE
\***            STOP
\***         endif
\***
\***   endcase
\***
\------------------------------------------------------------------------------

   ERROR.DETECTED:

      TERM% = 0
      MSGGRP% = 74
      MSGNUM% = 0
      SEVERITY% = 3


      IF ERR = "CM" OR ERR = "CT" THEN                                 \
         EVENT.NUM% = 18                                              :\ FBCW
         CHAIN.MODULE$ = MID$(MODULE.NUMBER$,3,1)                     :\
         UNIQUE$ = "BF01 " + CHAIN.MODULE$ + "50  "                   :\ FBCW
         CALL ADXERROR (TERM%,                                         \
                        MSGGRP%,                                       \
                        MSGNUM%,                                       \
                        SEVERITY%,                                     \
                        EVENT.NUM%,                                    \
                        UNIQUE$)                                      :\
         CHAIN.MODULE$ = MID$(MODULE.NUMBER$,1,3)                     :\
         PSBCHN.PRG = "ADX_UPGM:PSB50.286"                            :\ ISWM
         RESUME CHAIN.MODULE.50                                       :\ FBCW
      ELSE                                                             \
         EVENT.NUM% = 1                                               :\
         GOSUB SET.UP.UNIQUE                                          :\
         CALL ADXERROR (TERM%,                                         \
                        MSGGRP%,                                       \
                        MSGNUM%,                                       \
                        SEVERITY%,                                     \
                        EVENT.NUM%,                                    \
                        UNIQUE$)                                      :\
         IF ERR = "NL" OR ERR = "CU" THEN                              \
            RESUME

      IF BATCH.SCREEN.FLAG$ = "S" THEN                                 \
         CHAIN.MODULE$ = MID$(MODULE.NUMBER$,1,3)                     :\
         PSBCHN.PRG = "ADX_UPGM:PSB50.286"                            :\ ISWM
         RESUME CHAIN.MODULE.50                                       :\ FBCW
      ELSE                                                             \
         STOP

\------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   END FUNCTION
\***
\------------------------------------------------------------------------------

   END FUNCTION

END
