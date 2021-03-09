REM\
\*******************************************************************************
\*******************************************************************************
\***
\***
\***        FUNCTION      : SESS.NUM.UTILITY
\***        AUTHOR        : Bruce Scriver (Pseudocode)
\***                      : Bruce Scriver (Basic Code)
\***        DATE WRITTEN  : 4th May 1988 (Pseudocode)
\***                      : 6th May 1988 (Basic Code)
\***
\***        REFERENCE     : PSBF20
\***
\***
\***        VERSION B.      D.S. O'DARE (Pseudocode)        24th November 1988
\***                        B.C. WILLIS (Basic code)         1st December 1988
\***        89A VERSION (ie. small stores changes added to stocks version).
\***        Replace CHAIN statement with new included code (PSBCHNE.J86) and
\***        CHAIN.FILE.NAME$ with PSBCHN.PRG.  Amend program-to-chain-to from
\***        "01" to "50".
\***
\***        VERSION C.      Robert Cowey                          7th May 1991
\***        Upgraded PSBF20 included code from version B to C.
\***        Upgraded other included code to un-lettered to version A.
\***
\***        VERSION D.      Janet Smith                          13th May 1991
\***        Changed table to accommodate file reporting numbers greater
\***        than 128.   Change error processing to log an event 101, using
\***        2 byte file number.
\***
\***    DO NOT ADD STANDARD ERROR DETECTED TO THIS FUNCTION
\***    AS IT CALLS PSBF20 AND COULD END UP IN A LOOP
\***
\***    Note: Errors are reported including the SESSION number rather than
\***          the report number, in case an error occurs reading the
\***          report number table.
\***
\***        VERSION E.      Andrew Wedgeworth                    1st July 1992
\***        Redundant function parameters removed and defined as global
\***        variables. 
\***
\***        VERSION F.   STUART WILLIAM MCCONNACHIE         2nd Sept 2005
\***        Removed version numbered included code - About time.
\***        This is so we can compile FUNLIB version without line numbers.
\***
\***        VERSION G.   STUART WILLIAM MCCONNACHIE         10th Jan 2006
\***        Allow use from GSA programs by allocating session numbers in range
\***        65-100 instead of 1-64.  Note for GSA apps we will still allocate
\***        the lower table entries, just never allocate them.  This ensures
\***        other programs that look at the table will work without change.
\***        To use from GSA apps MODULE.NAME$ must start EAL something.
\***        Also general tidy up, use BEGIN ENDIF etc. (uncommented)
\***        Really this function could/should be rewritten completely....
\***
\***        Version H.      Stuart William McConnachie     31st Oct 2006
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
\***
\***                   FUNCTION OVERVIEW
\***                   -----------------
\***
\***        This function controls the allocation and deallocation of session
\***     numbers.  On opening a file, the function is called to allocate a
\***     session number, which it returns, updating the session number table
\***     held in global storage.  If a session number is passed to the function,
\***     it will return the file reporting number and name. When closing or
\***     deleting a file, the function is called to remove the table entry
\***     corresponding to the file.
\***
\***     The session number table is of the following format:
\***
\***     Number of entries: 64
\***     Each entry:        File reporting number (1 character, string
\***                                               representation of integer)
\***                        File name             (8 characters, logical name)
\***     e.g. - For file reporting number 54, name "CIMFI", the table entry
\***            would be 003643494D4649202020H, or " 6CIMFI   "
\***
\***     NOTE *********   CHANGE DUE TO VERSION D           ! DJAS
\***     The 2 byte integer containing the file reporting number is
\***     stored in readable format on the table, NOT as it would be on a file
\***
\***     Note: the first table entry, entry 0, should not be used.
\***
\***     The table must be defined as global at the start of each program -
\***     this will be done by means of included code.
\***
\***     Function parameters are as follows:
\***
\***     batch/screen flag, operator number, module number - as usual.
\***
\***     function flag - 1 character, values: "O" - create file table entry
\***                                          "R" - access file name/number
\***                                          "C" - remove file table entry
\***
\***     passed integer - 2 byte integer, for "O" - file reporting number
\***                                      for "R" and "C" - file session number
\***
\***     passed string - null / 5 characters, for "O" - file logical name
\***                                          for "R" and "C" - null
\***
\***     The function has five global fields:
\***
\***     SESS.NUM.UTILITY - function return code - 0 - successful processing
\***                                             - 1 - unsuccessful processing
\***
\***     F20.INTEGER.FILE.NO% - for "O" - allocated file session number
\***                          - for "R" - file reporting number
\***                          - for "C" - zero
\***
\***     F20.FILE.NAME$ - for "O" and "C" - null
\***                    - for "R" - file logical name
\***
\***     F20.STRING.FILE.NO$ - for "R" - 3 character string equivalent of
\***                                     F20.INTEGER.FILE.NO% - leading zeroes
\***                         - for "O" and "C" - null
\***
\***     F20.TABLE.DIMENSIONED.FLAG$ - value "Y" if the session number table
\***                                   has been dimensioned.
\***
\*******************************************************************************
\*******************************************************************************
\***
\***  %INCLUDE of globals and external definitions
\***
\-------------------------------------------------------------------------------

! 1 line deleted from here                                              ! EAW
%INCLUDE PSBF16G.J86                                                    ! FSWM
%INCLUDE PSBF17G.J86                                                    ! FSWM
%INCLUDE PSBF20G.J86                                                    ! FSWM
%INCLUDE PSBUSEG.J86                                                    ! FSWM

STRING GLOBAL BATCH.SCREEN.FLAG$                                        ! EAW
STRING GLOBAL MODULE.NUMBER$                                            ! EAW
STRING GLOBAL OPERATOR.NUMBER$                                          ! EAW

%INCLUDE ADXERROR.J86                                                   ! CRC
%INCLUDE PSBF01E.J86                                                    ! FSWM
%INCLUDE PSBF16E.J86                                                    ! FSWM
%INCLUDE PSBF17E.J86                                                    ! FSWM
%INCLUDE PSBF24E.J86                                                    ! FSWM

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** define public function SESS.NUM.UTILITY 
\***
\*** define variables used by the function
\***
\-------------------------------------------------------------------------------

FUNCTION SESS.NUM.UTILITY (FUNCTION.FLAG$,                              \
                           PASSED.INTEGER%,                             \
                           PASSED.STRING$)                              \
            PUBLIC
! 3 parameters deleted from here                                        ! EAW
  
    STRING ERRNUM$
    STRING FUNCTION.FLAG$
    STRING MODULE.LETTER$
    STRING NULL.FOUND.FLAG$
    STRING PASSED.STRING$
    STRING STRING.ERRL$
    STRING UNIQUE$
    STRING VAR.STRING.1$
    STRING VAR.STRING.2$
! 3 variables deleted from here                                         ! EAW

    INTEGER*1 EVENT.NUM%
    INTEGER*1 INDEX%
    INTEGER*1 MSGGRP%
    INTEGER*1 SEVERITY%

    INTEGER*2 EVENT.NO%
    INTEGER*2 FILE.NO%
    INTEGER*2 F17.RET.CODE%                                             ! EAW            
    INTEGER*2 MESSAGE.NUMBER%
    INTEGER*2 MSGNUM%
    INTEGER*2 PASSED.INTEGER%                                           ! DJAS
    INTEGER*2 RET.CODE%
    INTEGER*2 SESS.NUM.UTILITY                                          ! EAW
    INTEGER*2 TERM%
    INTEGER*2 F20.TABLE.SIZE%                                           ! GSWM
    INTEGER*2 F20.TABLE.BASE%                                           ! GSWM

    INTEGER*4 INTEGER4%

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\***   ON ERROR GOTO label ERROR.DETECTED
\***
\***   set SESS.NUM.UTILITY to zero
\***   initialise storage areas for ADXERROR call in case of memory overflow
\***
\***   initialise other variables as required
\***
\***   check value of F20.TABLE.DIMENSIONED.FLAG$ - if it is not "Y",
\***   dimension the session number table to have 64 entries, not including
\***   the first entry, subscript zero; and set the flag to "Y"
\***
\***   perform the appropriate subroutine as specified by the value of
\***   the function flag parameter - "O" = SET.TABLE.ENTRY
\***                               - "R" = READ.TABLE.ENTRY
\***                               - "C" = DELETE.TABLE.ENTRY
\***                               - other = log event 4, message 711 - invalid
\***                                         option parameter, set return code
\***                                         to 1 and blank out global fields
\***                                         except F20.TABLE.DIMENSIONED.FLAG$
\***
\***   EXIT FUNCTION
\***
\-------------------------------------------------------------------------------

    ON ERROR GOTO ERROR.DETECTED

    SESS.NUM.UTILITY = 0
    UNIQUE$          = "          "
    ERRNUM$          = "    "
    STRING.ERRL$     = "      "
    MODULE.LETTER$   = MID$(MODULE.NUMBER$+"???",3,1)

    IF F20.TABLE.DIMENSIONED.FLAG$ <> "Y" THEN BEGIN                    !GSWM
        IF LEFT$(MODULE.NUMBER$,3) = "EAL" THEN BEGIN                   !GSWM
            F20.TABLE.SIZE% = 35                                        !GSWM
            F20.TABLE.BASE% = 64                                        !GSWM
        ENDIF ELSE BEGIN                                                !GSWM
            F20.TABLE.SIZE% = 64                                        !GSWM
            F20.TABLE.BASE% = 0                                         !GSWM
        ENDIF                                                           !GSWM
        DIM SESS.NUM.TABLE$(F20.TABLE.BASE%+F20.TABLE.SIZE%)            !GSWM
        F20.TABLE.DIMENSIONED.FLAG$ = "Y"                               !GSWM
    ENDIF                                                               !GSWM

    IF FUNCTION.FLAG$ = "O" THEN BEGIN
        GOSUB SET.TABLE.ENTRY
    ENDIF ELSE IF FUNCTION.FLAG$ = "R" THEN BEGIN
        GOSUB READ.TABLE.ENTRY
    ENDIF ELSE IF FUNCTION.FLAG$ = "C" THEN BEGIN
        GOSUB DELETE.TABLE.ENTRY
    ENDIF ELSE BEGIN
        F20.FILE.NAME$ = ""
        F20.STRING.FILE.NO$ = ""
        F20.INTEGER.FILE.NO% = 0
        SESS.NUM.UTILITY = 1
        MESSAGE.NUMBER% = 711
        EVENT.NO% = 4
        INTEGER4% = 711
        RET.CODE% = CONV.TO.STRING (EVENT.NO%, INTEGER4%)               ! EAW
        IF RET.CODE% = 0 THEN BEGIN                                     ! EAW
            VAR.STRING.1$ = F17.RETURNED.STRING$
            INTEGER4% = LEN(FUNCTION.FLAG$)
            RET.CODE% = CONV.TO.STRING (EVENT.NO%, INTEGER4%)           ! EAW
            IF RET.CODE% = 0 THEN BEGIN                                 ! EAW
                VAR.STRING.1$ = VAR.STRING.1$                           \
                                + F17.RETURNED.STRING$                  \
                                + FUNCTION.FLAG$
                VAR.STRING.2$ = "20"                                    \
                                + RIGHT$("00"                           \
                                + STR$(LEN(FUNCTION.FLAG$)),2)          \
                                + FUNCTION.FLAG$
                CALL APPLICATION.LOG (MESSAGE.NUMBER%,                  \ EAW
                                      VAR.STRING.1$,                    \
                                      VAR.STRING.2$,                    \
                                      EVENT.NO%)
      ! 3 parameters no longer passed to APPLICATION.LOG                ! EAW
            ENDIF
        ENDIF
    ENDIF

EXIT FUNCTION

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\************************ subroutines follow ***********************************
\*******************************************************************************
\***
\*** SET.TABLE.ENTRY:
\***
\***   search SESS.NUM.TABLE$ sequentially for first null entry
\***   NOTE: ignore entry 0, which is always null
\***
\***   If there is no null entry in the table, log event 46, message number 558
\***   - session number table full - set return code to 1, blank out global 
\***   fields except F20.TABLE.DIMENSIONED.FLAG$ and return to main line
\***
\***   set null entry to one byte character representing the integer value of
\***   the file reporting number (use CHR$ function) plus the passed string
\***   (the file logical name) padded to the right with spaces to 8 characters
\***
\***   set F20.FILE.NAME$ to null
\***   set F20.INTEGER.FILE.NO% to the table entry number for the file
\***   set F20.STRING.FILE.NO$ to null
\***
\***   RETURN to main line
\***
\-------------------------------------------------------------------------------

SET.TABLE.ENTRY:

    NULL.FOUND.FLAG$ = "N"
    INDEX% = F20.TABLE.BASE% + 1

    WHILE NULL.FOUND.FLAG$ = "N" AND                                    \
          INDEX% <= F20.TABLE.BASE% + F20.TABLE.SIZE%                   !GSWM
        
        IF SESS.NUM.TABLE$(INDEX%) = "" THEN BEGIN
            NULL.FOUND.FLAG$ = "Y"
        ENDIF ELSE BEGIN
            INDEX% = INDEX% + 1
        ENDIF

    WEND

    IF NULL.FOUND.FLAG$ = "N" THEN BEGIN
    
        F20.FILE.NAME$ = ""
        F20.STRING.FILE.NO$ = ""
        F20.INTEGER.FILE.NO% = 0
        SESS.NUM.UTILITY = 1
        MESSAGE.NUMBER% = 558
        VAR.STRING.1$ = ""
        VAR.STRING.2$ = ""
        EVENT.NO% = 46
        RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                   \ EAW
                                      VAR.STRING.1$,                    \
                                      VAR.STRING.2$,                    \
                                      EVENT.NO%)
        RETURN
        
    ENDIF

    SESS.NUM.TABLE$(INDEX%) = CHR$(SHIFT(PASSED.INTEGER%,8)) +          \ DJAS
                              CHR$(SHIFT(PASSED.INTEGER%,0)) +          \ DJAS
                              LEFT$(PASSED.STRING$ + "        ",8)

    F20.FILE.NAME$ = ""
    F20.INTEGER.FILE.NO% = INDEX%
    F20.STRING.FILE.NO$ = ""

RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** READ.TABLE.ENTRY:
\***
\***   read SESS.NUM.TABLE$ entry as indicated by the passed integer parameter
\***
\***   set F20.FILE.NAME$ to the file name part of the table entry
\***   set F20.INTEGER.FILE.NO% to the integer equivalent of the reporting
\***   number part of the table entry - use the ASC function.
\***   set F20.STRING.FILE.NO$ to the string equivalent of the reporting number
\***   part of the table entry - convert negative values to positive values by
\***   treating as 2-byte integers, adding 256 if negative.
\***
\***   NOTE: if the table entry is null or the passed integer is null or zero,
\***         set F20.INTEGER.FILE.NO% to zero, set F20.STRING.FILE.NO$ to null
\***         and set F20.FILE.NAME$ to null
\***
\***   RETURN to main line
\***
\-------------------------------------------------------------------------------

READ.TABLE.ENTRY:

    IF SESS.NUM.TABLE$(PASSED.INTEGER%) = "" THEN BEGIN
        F20.FILE.NAME$ = ""
        F20.STRING.FILE.NO$ = ""
        F20.INTEGER.FILE.NO% = 0
    ENDIF ELSE BEGIN
        F20.FILE.NAME$ = RIGHT$(SESS.NUM.TABLE$(PASSED.INTEGER%),8)
        F20.INTEGER.FILE.NO% =                                          \ DJAS
               (256 * ASC(LEFT$(SESS.NUM.TABLE$(PASSED.INTEGER%),1))) + \ DJAS
               ASC(MID$(SESS.NUM.TABLE$(PASSED.INTEGER%),2,1))          ! DJAS
        IF F20.INTEGER.FILE.NO% < 0 THEN BEGIN
            FILE.NO% = F20.INTEGER.FILE.NO% + 256
            F20.STRING.FILE.NO$ = RIGHT$("000" + STR$(FILE.NO%),3)
        ENDIF ELSE BEGIN
            F20.STRING.FILE.NO$ = RIGHT$("000"                         \
                                        + STR$(F20.INTEGER.FILE.NO%),3)
        ENDIF
    ENDIF

RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** DELETE.TABLE.ENTRY:
\***
\***   read SESS.NUM.TABLE$ entry as indicated by the passed integer parameter
\***
\***   set the table entry to null
\***
\***   set F20.FILE.NAME$ to null
\***   set F20.INTEGER.FILE.NO% to zero
\***   set F20.STRING.FILE.NO$ to null
\***
\***   RETURN to main line
\***
\-------------------------------------------------------------------------------

DELETE.TABLE.ENTRY:

    SESS.NUM.TABLE$(PASSED.INTEGER%) = ""

    F20.FILE.NAME$ = ""
    F20.INTEGER.FILE.NO% = 0
    F20.STRING.FILE.NO$ = ""

RETURN

\-------------------------------------------------------------------------------
\***
\*******************************************************************************
\***
\*** ERROR.DETECTED
\***
\***   set F20.RETURN.CODE% to 1 to indicate unsuccessful processing
\***
\***   take appropriate action depending on the value of the error code ERR:
\***
\***   - out of memory (ERR = OM): CALL ADXERROR
\***   - chain failure (ERR = CM or CT): CALL APPLICATION.LOG, error number 553,
\***                                                           event number 18
\***     (having set VAR.STRING.1$ to "BF20 " + MODULE.LETTER$ + "50  "
\***             and VAR.STRING.2$ to "PS" + MODULE.LETTER$ + "50")
\***   - other errors: CALL APPLICATION.LOG, error number 551,
\***                                         event number 1
\***
\***   use CONV.TO.STRING function to give the string equivalent of ERRN and
\***   CONV.TO.HEX function to give the Hex equivalent of ERRN for logging
\***
\***
\***   IF program is not screen program THEN
\***      STOP
\***   ENDIF
\***
\***   set PSBCHN.PRG to "ADX_UPGM:" + (leftmost 3 bytes of MODULE.NUMBER$)
\***                                          + "50.286"
\***   %INCLUDE PSBCHNE.J86
\***
\*** END FUNCTION
\***
\-------------------------------------------------------------------------------

ERROR.DETECTED:

    SESS.NUM.UTILITY = 1

    IF ERR <> "CM" AND ERR <> "CT" THEN BEGIN
        EVENT.NO% = 1
        INTEGER4% = ERRN
        F17.RET.CODE% = CONV.TO.STRING (EVENT.NO%, INTEGER4%)           ! EAW
        IF RET.CODE% = 0 THEN BEGIN                                     ! EAW
            ERRNUM$   = F17.RETURNED.STRING$
            STRING.ERRL$ = PACK$(RIGHT$("000000" + STR$(ERRL),6))
        ENDIF
    ENDIF

    IF ERR = "OM" THEN BEGIN !out of memory
        IF F17.RET.CODE% = 0 THEN BEGIN
            TERM%         = 0
            MSGGRP%       = ASC("J")
            MSGNUM%       = 0
            SEVERITY%     = 3
            EVENT.NUM%    = 1
            UNIQUE$      = ERRNUM$ + CHR$(ERRF%) + ERR + STRING.ERRL$
            RET.CODE%    = ADXERROR (TERM%,                             \
                                     MSGGRP%,                           \
                                     MSGNUM%,                           \
                                     SEVERITY%,                         \
                                     EVENT.NUM%,                        \
                                     UNIQUE$)
        ENDIF
    ENDIF

    IF ERR = "CM" OR ERR = "CT" THEN BEGIN !chain failure
        MESSAGE.NUMBER% = 553
        VAR.STRING.1$   = "BF20 " + MODULE.LETTER$ + "50  "             ! BBCW
        VAR.STRING.2$   = "PS" + MODULE.LETTER$ + "50"                  ! BBCW
        EVENT.NO%       = 18
        RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                   \ EAW
                                      VAR.STRING.1$,                    \
                                      VAR.STRING.2$,                    \
                                      EVENT.NO%)
      ! 3 parameters removed from here                                  ! EAW
    ENDIF

    IF ERR <> "OM" AND ERR <> "CM" AND ERR <> "CT" THEN BEGIN
        IF F17.RET.CODE% = 0 THEN BEGIN
            MESSAGE.NUMBER% = 550
            VAR.STRING.1$ = ERRNUM$ + CHR$(ERRF%) + ERR + STRING.ERRL$
            INTEGER4% = ERRN
            RET.CODE% = CONV.TO.HEX (INTEGER4%)                         ! EAW
            IF RET.CODE% = 0 THEN BEGIN                                 ! EAW
                VAR.STRING.2$ = ERR + F16.HEX.STRING$                   \
                              + RIGHT$("000" + STR$(ERRF%),3)           \
                              + STR$(ERRL)
                EVENT.NO%     = 1
                RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,           \ EAW
                                            VAR.STRING.1$,              \
                                            VAR.STRING.2$,              \
                                            EVENT.NO%)
!5 lines deleted from here                                             ! BBCW
            ENDIF
        ENDIF
    ENDIF

    IF BATCH.SCREEN.FLAG$ <> "S" THEN STOP                              ! BBCW

    PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                   ! HSWM
    %INCLUDE PSBCHNE.J86                                                ! FSWM

END FUNCTION


