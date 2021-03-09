!******************************************************************************
!******************************************************************************
!***
!***            PROGRAM         :       PSBF39.BAS
!***
!***            DESCRIPTION     :       DISPLAY MANAGER INTERFACE
!***
!***            AUTHOR          :       Stuart William McConnachie
!***            DATE WRITTEN    :       September 2000
!***
!***        Provides a much needed and easier to use interface to
!***        IBM Display Manager.
!***
!***        Saves the user having to set up, highlight, tab between and
!***        validate fields.  Also handles the help screens and conversion
!***        of date fields to external/internal format.
!***
!***    VERSION A           STUART WILLIAM MCCONNACHIE          21 JUN 2002
!***    Added support for dynamic visibility of function key text.
!***
!***    VERSION B           STUART WILLIAM MCCONNACHIE          24 JUN 2002
!***    Change to invisible fields to make them more consistent:
!***    Fields which are set invisible on the DM screen designer now
!***    remain invisible if no VISIBLE$ expression is set from the code.
!***    NB: This change MAY have a knock on effect on programs coded before
!***        this date if they do not explicitly set the VISIBLE$ expression
!***        for a fields that is invisible on the display manager screen.
!***        These fields now no longer automatically become visible when
!***        DM.PROCESS.SCREEN is called.
!***    Also visibilty of fields now set AFTER new input field has been
!***    moved to.  This ensures visibility of fields can be set by an
!***    expression to depend on the current input field.
!***    Added support for definition of aditional keys which validate all
!***    fields on display before returning (in addition to ENTER key).
!***
!***    VERSION C           STUART WILLIAM MCCONNACHIE          27 JUN 2002
!***    Added support for time field formatting similar to existing date
!***    fields.
!***    Added UP and DOWN arrow keys to work like BTAB and TAB.
!***
!***    VERSION C           STUART WILLIAM MCCONNACHIE          27 JUN 2002
!***    Added INSERT key.
!***
!***    VERSION D           STUART WILLIAM MCCONNACHIE          24 FEB 2006
!***    Added support for naming of field variables.
!***    Added support for linear or number ordered tabbing of fields.
!***
!***    VERSION E           STUART WILLIAM MCCONNACHIE           1 SEP 2006
!***    Added support for read only input fields.  This allows a way to TAB
!***    to a field and select an item from a list (instead of the more
!***    traditional "Place an X next ot the item" method).
!***
!***    VERSION F           Neil Bennett                        6 June 2007
!***    Added 'coarse' support for changing field attributes by supplying
!***    the standard 'SETF' parameter. Also added the ability to set a flag
!***    that makes DM treat an Input field as an output field when positioning
!***    cursor/active field.
!***
!***    VERSION G           Charles Skadorwa                    12 SEP 2008
!***    Help Screen processing enhanced to to save the current screen function
!***    key attributes before displaying the Help Screen and re-instating
!***    them correctly after exiting the Help Screen.
!***
!******************************************************************************
!******************************************************************************

!******************************************************************************
!***
!***    LOCAL VARIABLES
!***

STRING              DISPLAY.FILE$
STRING              HELP.FILE$
STRING              VALUE$
STRING              PREV.VALUE$
STRING              TEMP$
STRING              STATE$
STRING              ATTRIB$
STRING              CURRENT.TITLE$
!E  STRING              DATE.FIELDS$
!E  STRING              TIME.FIELDS$                                        !CSWM
STRING              CHAR.DATE$                                              !CSWM
STRING              CHAR.TIME$                                              !CSWM
STRING              INVISIBLE.FIELD$
STRING              FN.VISIBLE$(1)                                          !ASWM
STRING              VALIDATE.KEYS$                                          !BSWM

INTEGER*1           TRUE
INTEGER*1           FALSE
INTEGER*1           VISIBLE
INTEGER*1           CURRENT.STATE
INTEGER*1           EXIT.FLAG
INTEGER*1           VALID
INTEGER*1           UPDATE
INTEGER*1           RESUME.INPUT
INTEGER*1           FIELDS.CHANGED
INTEGER*1           FIELD.TAB.ORDER                                         !DSWM
INTEGER*1           DISPLAY.ORDER                                           !DSWM
INTEGER*1           NUMBER.ORDER                                            !DSWM

INTEGER*2           CURRENT.SCREEN%
!DSWMINTEGER*2      CURRENT.FIELD%
INTEGER*2           FIRST.HELP.SCREEN%
INTEGER*2           LAST.HELP.SCREEN%
INTEGER*2           PAGE%
INTEGER*2           PAGE.COUNT%

INTEGER*2           RC%
INTEGER*2           FIELD.COUNT%
INTEGER*2           KEY%
INTEGER*2           CONFIRM.KEY%
INTEGER*2           LEN%

INTEGER*2           DATE.FORMAT%
INTEGER*2           TIME.FORMAT%                                            !CSWM

INTEGER*2           FIELD.FLAGS%(1)                                         !ESWM
INTEGER*2           DATE.FLAG%                                              !ESWM
INTEGER*2           TIME.FLAG%                                              !ESWM
INTEGER*2           RO.FLAG%                                                !ESWM
INTEGER*2           OUT.FLAG%                                               !FNWB

INTEGER*2           MESSAGE.FIELD%
INTEGER*2           TITLE.FIELD%
INTEGER*2           DATE.FIELD%
INTEGER*2           INVISIBLE.FIELD%
INTEGER*2           F.FIELD%
INTEGER*2           F1.FIELD%
INTEGER*2           F2.FIELD%
INTEGER*2           F3.FIELD%
INTEGER*2           F4.FIELD%
INTEGER*2           F5.FIELD%
INTEGER*2           F6.FIELD%
INTEGER*2           F7.FIELD%
INTEGER*2           F8.FIELD%
INTEGER*2           F9.FIELD%
INTEGER*2           F10.FIELD%

INTEGER*4           ADX.RC%

STRING              NAME$(1)                                                !DSWM
STRING              SAVED.FUNC.KEY$(1)                                      !GCSK

INTEGER*2           F%                                                      !DSWM
INTEGER*2           LAST.INPUT%                                             !DSWM
INTEGER*2           NEXT.INPUT%(1)                                          !DSWM
INTEGER*2           PREV.INPUT%(1)                                          !DSWM


!******************************************************************************
!***
!***    GLOBAL VARIBALES
!***

!These two globals are shared with PSBF38, but "hidden" (i.e. not included in
!PSBF39G so as not to be visible to other routines.  The reason for doing this
!is so that PSBF38 can be linked to modules without the need to link in F39
!as well (which would be the case if a function call to F39 were used).
!Note we use PSBF38 from PSBF44 (SEL printing) which will be linked with
!batch programs, so we do not want the overhead of all the DM stuff as well.
INTEGER*2 GLOBAL    ?F39.CF%                                                !DSWM
STRING GLOBAL       ?F39.CF$                                                !DSWM

STRING GLOBAL       MODULE.NUMBER$

%INCLUDE PSBF38G.J86
%INCLUDE PSBF39G.J86


!******************************************************************************
!***
!***    EXTERNAL FUNCTIONS
!***

%INCLUDE PSBF38E.J86
%INCLUDE DMEXTR.J86       ! I.B.M. DISPLAY MANAGER ext functions
%INCLUDE ADXEXT.J86


!******************************************************************************
!***
!***    DM.FIELD.CHANGED
!***    A user exit which is called each time a field on the screen is updated
!***

SUB DM.FIELD.CHANGED (SCREEN%, FIELD%, VALUE$, VALID, UPDATE) EXTERNAL

    INTEGER*2   SCREEN%     !Current screen number
    INTEGER*2   FIELD%      !Field modifed
    STRING      VALUE$      !New value for field (can be modified)
    INTEGER*1   VALID       !Return FALSE if field is invalid
    INTEGER*1   UPDATE      !Return TRUE if fields changed

END SUB


!******************************************************************************
!***
!***    DM.CHECK.ERROR
!***    Checks for an error after a call to the runtime expression evaluator
!***

SUB DM.CHECK.ERROR                                                          !CSWM

    IF LEN(F38.EVAL.ERROR$) THEN BEGIN                                      !CSWM

        CLEARS                                                              !CSWM
        PRINT "PSBF39 ENHANCED DISPLAY MANAGER API"                         !CSWM
        PRINT                                                               !CSWM
        PRINT "Detected error from runtime expression evaluator:"           !CSWM
        PRINT F38.EVAL.ERROR$                                               !CSWM
        PRINT                                                               !CSWM
        PRINT "Last runtime expression:"                                    !CSWM
        PRINT DETOKENISE$(F38.LAST.EXPR$)                                   !CSWM
        PRINT                                                               !CSWM
        PRINT "Press Ctrl+C to exit"                                        !CSWM

        WHILE 1                                                             !CSWM
            WAIT ;1000                                                      !CSWM
        WEND                                                                !CSWM

    ENDIF                                                                   !CSWM

END SUB                                                                     !CSWM


!******************************************************************************
!***
!***    SET.CF
!***    Updates the current field, adjusting ?39.CF$ at the same time
!***

FUNCTION SET.CF (FIELD%)                                                    !DSWM

    INTEGER*2 FIELD%                                                        !DSWM
    INTEGER*2 SET.CF                                                        !DSWM

    ?F39.CF% = FIELD%                                                       !DSWM
    IF FIELD% >= 2 AND FIELD% <= FIELD.COUNT% THEN BEGIN                    !DSWM
        ?F39.CF$ = NAME$(FIELD%)                                            !DSWM
    ENDIF ELSE BEGIN                                                        !DSWM
        ?F39.CF$ = ""                                                       !DSWM
    ENDIF                                                                   !DSWM
    SET.CF = ?F39.CF%                                                       !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    IS.VISIBLE.FIELD
!***    Returns true if the currently selected DM field is visible
!***

FUNCTION IS.VISIBLE.FIELD

    INTEGER*1   IS.VISIBLE.FIELD

    STATE$ = SETF("")
    IS.VISIBLE.FIELD = LEFT$(STATE$, 1) = "0"

END FUNCTION


!******************************************************************************
!***
!***    IS.INFUT.FIELD
!***    Returns true if the currently selected DM field is an input field
!***

FUNCTION IS.INPUT.FIELD

    INTEGER*1   IS.INPUT.FIELD

    STATE$ = RETF
    IS.INPUT.FIELD = MID$(STATE$, 8, 1) = "I"

END FUNCTION


!******************************************************************************
!***
!***    IS.OUTPUT.FIELD
!***    Returns true if the currently selected DM field is an output field
!***

FUNCTION IS.OUTPUT.FIELD

    INTEGER*1   IS.OUTPUT.FIELD

    STATE$ = RETF
    IS.OUTPUT.FIELD = MID$(STATE$, 8, 1) = "O"

END FUNCTION


!******************************************************************************
!***
!***    IS.DATE.FIELD
!***    Returns true if the field given is defined as a date field
!***

FUNCTION IS.DATE.FIELD (FIELD%)

    INTEGER*2   FIELD%
    INTEGER*1   IS.DATE.FIELD

\E  IS.DATE.FIELD = MATCH("\"+CHR$(FIELD%), DATE.FIELDS$, 1) <> 0
    IS.DATE.FIELD = (FIELD.FLAGS%(FIELD%) AND DATE.FLAG%) <> 0              !ESWM

END FUNCTION


!******************************************************************************
!***
!***    IS.TIME.FIELD
!***    Returns true if the field given is defined as a time field
!***

FUNCTION IS.TIME.FIELD (FIELD%)

    INTEGER*2   FIELD%
    INTEGER*1   IS.TIME.FIELD

\E  IS.TIME.FIELD = MATCH("\"+CHR$(FIELD%), TIME.FIELDS$, 1) <> 0
\   IS.DATE.FIELD = (FIELD.FLAGS%(FIELD%) AND TIME.FLAG%) <> 0              !ESWM
    IS.TIME.FIELD = (FIELD.FLAGS%(FIELD%) AND TIME.FLAG%) <> 0              !FNWB ESWM

END FUNCTION


!******************************************************************************
!***
!***    IS.RO.FIELD
!***    Returns true if the field given is defined as a read only field
!***

FUNCTION IS.RO.FIELD (FIELD%)                                               !ESWM

    INTEGER*2   FIELD%                                                      !ESWM
    INTEGER*1   IS.RO.FIELD                                                 !ESWM

    IS.RO.FIELD = (FIELD.FLAGS%(FIELD%) AND RO.FLAG%) <> 0                  !ESWM

END FUNCTION                                                                !ESWM


!******************************************************************************
!***
!***    IS.OO.FIELD
!***    Returns true if the field given is defined as a read only field
!***

FUNCTION IS.OO.FIELD (FIELD%)                                               !FNWB

    INTEGER*2   FIELD%                                                      !FNWB
    INTEGER*1   IS.OO.FIELD                                                 !FNWB

    IS.OO.FIELD = (FIELD.FLAGS%(FIELD%) AND OUT.FLAG%) <> 0                 !FNWB

END FUNCTION                                                                !FNWB


!******************************************************************************
!***
!***    GET.FIELD.ROW
!***    Returns the row of the current DM field
!***

FUNCTION GET.FIELD.ROW

    INTEGER*2   GET.FIELD.ROW

    STATE$ = RETF
    GET.FIELD.ROW = ASC(MID$(STATE$, 1, 1))

END FUNCTION


!******************************************************************************
!***
!***    GET.FIELD.COL
!***    Returns the column of the current DM field
!***

FUNCTION GET.FIELD.COL

    INTEGER*2   GET.FIELD.COL

    STATE$ = RETF
    GET.FIELD.COL = ASC(MID$(STATE$, 3, 1))

END FUNCTION


!******************************************************************************
!***
!***    GET.FIELD.LEN
!***    Returns the length of the current DM field
!***

FUNCTION GET.FIELD.LEN

    INTEGER*2   GET.FIELD.LEN

    STATE$ = RETF
    GET.FIELD.LEN = ASC(MID$(STATE$, 5, 1))

END FUNCTION


!******************************************************************************
!***
!***    EXT.DATE$
!***    Converts a date from internal (YYMMDD) format to external format
!***    according to the system date format (i.e. DDMMYY or MMDDYY).
!***

FUNCTION EXT.DATE$ (IN.DATE$)

    STRING      IN.DATE$
    STRING      EXT.DATE$

    IF IN.DATE$ = "" THEN BEGIN
        EXT.DATE$ = "      "
        EXIT FUNCTION
    ENDIF

    !Internal date is YYMMDD format
    IN.DATE$ = RIGHT$("000000" + IN.DATE$, 6)

    IF DATE.FORMAT% = 2 OR DATE.FORMAT% = 4 THEN BEGIN
        !External date is DDMMYY format
        EXT.DATE$ = RIGHT$(IN.DATE$, 2) + MID$(IN.DATE$, 3, 2) +            \
                    LEFT$(IN.DATE$, 2)
    ENDIF ELSE BEGIN
        !External date is MMDDYY format
        EXT.DATE$ = MID$(IN.DATE$, 3, 2) + RIGHT$(IN.DATE$, 2) +            \
                    LEFT$(IN.DATE$, 2)
    ENDIF

END FUNCTION


!******************************************************************************
!***
!***    INT.DATE$
!***    Converts a date from external (i.e. DDMMYY or MMDDYY) format to
!***    internal (YYMMDD) format according to the system date format.
!***

FUNCTION INT.DATE$ (EX.DATE$)

    STRING      EX.DATE$
    STRING      INT.DATE$

    IF EX.DATE$ = "" THEN BEGIN
        INT.DATE$ = "      "
        EXIT FUNCTION
    ENDIF

    !Pad external date
    EX.DATE$ = LEFT$(EX.DATE$ + "000000", 6)

    IF DATE.FORMAT% = 2 OR DATE.FORMAT% = 4 THEN BEGIN
        !External date is DDMMYY format
        INT.DATE$ = RIGHT$(EX.DATE$, 2) + MID$(EX.DATE$, 3, 2) +            \
                    LEFT$(EX.DATE$, 2)
    ENDIF ELSE BEGIN
        !External date is MMDDYY format
        INT.DATE$ = RIGHT$(EX.DATE$, 2) + LEFT$(EX.DATE$, 2) +              \
                    MID$(EX.DATE$, 3, 2)
    ENDIF

END FUNCTION


!******************************************************************************
!***
!***    SEP.DATE$
!***    Adds date seperators to the supplied date.
!***

FUNCTION SEP.DATE$ (IN.DATE$)                                               !CSWM

    STRING SEP.DATE$                                                        !CSWM
    STRING IN.DATE$                                                         !CSWM

    SEP.DATE$ = MID$(IN.DATE$,1,2) + CHAR.DATE$ +                           \CSWM
                MID$(IN.DATE$,3,2) + CHAR.DATE$ +                           \CSWM
                MID$(IN.DATE$,5,2)                                          !CSWM

END FUNCTION                                                                !CSWM


!******************************************************************************
!***
!***    EXT.TIME$
!***    Converts a time to internal format (either HHMM or HHMMSS) to external
!***    HHMMSS format.
!***

FUNCTION EXT.TIME$ (IN.TIME$)                                               !CSWM

    STRING      IN.TIME$                                                    !CSWM
    STRING      EXT.TIME$                                                   !CSWM

    IF GET.FIELD.LEN <= 5 THEN BEGIN                                        !CSWM
        IF IN.TIME$ = "" THEN BEGIN                                         !CSWM
            EXT.TIME$ = "    "                                              !CSWM
        ENDIF ELSE BEGIN                                                    !CSWM
            !Internal time is HHMM format                                   !CSWM
            EXT.TIME$ = RIGHT$("0000" + IN.TIME$, 4)                        !CSWM
        ENDIF                                                               !CSWM
    ENDIF ELSE BEGIN                                                        !CSWM
        IF IN.TIME$ = "" THEN BEGIN                                         !CSWM
            EXT.TIME$ = "      "                                            !CSWM
        ENDIF ELSE BEGIN                                                    !CSWM
            !Internal time is HHMMSS format                                 !CSWM
            EXT.TIME$ = RIGHT$("000000" + IN.TIME$, 6)                      !CSWM
        ENDIF                                                               !CSWM
    ENDIF                                                                   !CSWM

END FUNCTION                                                                !CSWM


!******************************************************************************
!***
!***    INT.TIME$
!***    Converts a time to external format (HHMMSS) from internal displayed
!***    format, which could be either HHMM or HHMMSS.
!***

FUNCTION INT.TIME$ (EX.TIME$)                                               !CSWM

    STRING      EX.TIME$                                                    !CSWM
    STRING      INT.TIME$                                                   !CSWM

    IF GET.FIELD.LEN <= 5 THEN BEGIN                                        !CSWM
        IF EX.TIME$ = "" THEN BEGIN                                         !CSWM
            INT.TIME$ = "    "                                              !CSWM
        ENDIF ELSE BEGIN                                                    !CSWM
            INT.TIME$ = LEFT$(EX.TIME$ + "0000", 4)                         !CSWM
        ENDIF                                                               !CSWM
    ENDIF ELSE BEGIN                                                        !CSWM
        IF EX.TIME$ = "" THEN BEGIN                                         !CSWM
            INT.TIME$ = "      "                                            !CSWM
        ENDIF ELSE BEGIN                                                    !CSWM
            INT.TIME$ = LEFT$(EX.TIME$ + "000000", 6)                       !CSWM
        ENDIF                                                               !CSWM
    ENDIF                                                                   !CSWM

END FUNCTION                                                                !CSWM


!******************************************************************************
!***
!***    SEP.TIME$
!***    Adds time separators to the supplied time.
!***

FUNCTION SEP.TIME$ (IN.TIME$)                                               !CSWM

    STRING SEP.TIME$                                                        !CSWM
    STRING IN.TIME$                                                         !CSWM

    IF LEN(IN.TIME$) <= 4 THEN BEGIN                                        !CSWM
        SEP.TIME$ = MID$(IN.TIME$,1,2) + CHAR.TIME$ +                       \CSWM
                    MID$(IN.TIME$,3,2)                                      !CSWM
    ENDIF ELSE BEGIN                                                        !CSWM
        SEP.TIME$ = MID$(IN.TIME$,1,2) + CHAR.TIME$ +                       \CSWM
                    MID$(IN.TIME$,3,2) + CHAR.TIME$ +                       \CSWM
                    MID$(IN.TIME$,5,2)                                      !CSWM
    ENDIF                                                                   !CSWM

END FUNCTION                                                                !CSWM


!******************************************************************************
!***
!***    FORMAT.DATE$
!***    Formats a supplied date to DD MMM YYYY format.
!***

FUNCTION FORMAT.DATE$ (IN.DATE$)

    STRING      IN.DATE$
    STRING      FORMAT.DATE$
    STRING      MONTH$
    INTEGER*2   YEAR%
    INTEGER*2   MONTH%
    INTEGER*2   DAY%

    YEAR% = VAL(LEFT$(IN.DATE$, 2))
    MONTH% = VAL(MID$(IN.DATE$, 3, 2))
    DAY% = VAL(RIGHT$(IN.DATE$, 2))

    IF YEAR% >= 90 THEN BEGIN
        YEAR% = YEAR% + 1900
    ENDIF ELSE BEGIN
        YEAR% = YEAR% + 2000
    ENDIF

    MONTH$ = "   JANFEBMARAPRMAYJUNJULAUGSEPOCTNOVDEC"
    FORMAT.DATE$ = STR$(DAY%) + " " + MID$(MONTH$, MONTH%*3+1, 3) +     \
                   " " + STR$(YEAR%)

END FUNCTION


!******************************************************************************
!***
!***    DISPLAY.TITLE.AND.DATE
!***    Displays the screen title and date
!***

SUB DISPLAY.TITLE.AND.DATE

    IF CURRENT.TITLE$ <> "" THEN BEGIN

        DM.FIELD% = POSF(TITLE.FIELD%)
        LEN% = GET.FIELD.LEN

        IF LEN% <= LEN(CURRENT.TITLE$) THEN BEGIN
            TEMP$ = LEFT$(CURRENT.TITLE$, LEN%)
        ENDIF ELSE BEGIN
            LEN% = LEN% - LEN(CURRENT.TITLE$)
            TEMP$ = STRING$(LEN% / 2, " ") + CURRENT.TITLE$
        ENDIF

        RC% = PUTF(TEMP$)

    ENDIF

    DM.FIELD% = POSF(DATE.FIELD%)
    TEMP$ = FORMAT.DATE$ (DATE$)
    RC% = PUTF(TEMP$)

END SUB


!******************************************************************************
!***
!***    DM.INIT
!***    Initialises PSBF39 display manager functions for use
!***

FUNCTION DM.INIT PUBLIC

    INTEGER*1   DM.INIT

    DM.INIT = 1

    TRUE = -1
    FALSE = 0

    DISPLAY.ORDER = 0                                                       !DSWM
    NUMBER.ORDER = 1                                                        !DSWM
    FIELD.TAB.ORDER = DISPLAY.ORDER                                         !DSWM

    HOME.KEY% = 327
    END.KEY%  = 335
    PGUP.KEY% = 329
    PGDN.KEY% = 337

    TAB.KEY%  = 9
    BTAB.KEY% = 8217
    ESC.KEY%  = 27
    ENTER.KEY%= 0

    UP.KEY%   = 16                                                          !CSWM
    DOWN.KEY% = 17                                                          !CSWM
    INS.KEY%  = 8201                                                        !DSWM
    NEXT.KEY% = 07FFEH                                                      !DSWM
    PREV.KEY% = 07FFFH                                                      !DSWM

    F1.KEY%   = -1
    F2.KEY%   = -2
    F3.KEY%   = -3
    F4.KEY%   = -4
    F5.KEY%   = -5
    F6.KEY%   = -6
    F7.KEY%   = -7
    F8.KEY%   = -8
    F9.KEY%   = -9
    F10.KEY%  = -10

    MESSAGE.FIELD% = 1
    TITLE.FIELD% = 238
    DATE.FIELD% = 239
    INVISIBLE.FIELD% = 240
    F.FIELD% = 240
    F1.FIELD% = 241
    F2.FIELD% = 242
    F3.FIELD% = 243
    F4.FIELD% = 244
    F5.FIELD% = 245
    F6.FIELD% = 246
    F7.FIELD% = 247
    F8.FIELD% = 248
    F9.FIELD% = 249
    F10.FIELD%= 250

    DIM FN.VISIBLE$(10)                                                     !ASWM
    DIM SAVED.FUNC.KEY$(10)                                                 !GCSK

    DATE.FLAG% = 1                                                          !ESWM
    TIME.FLAG% = 2                                                          !ESWM
    RO.FLAG% = 4                                                            !ESWM
    OUT.FLAG% = 8                                                           !FNWB

    CALL ADXSERVE(ADX.RC%, 4, 0, TEMP$)
    IF ADX.RC% = 0 THEN BEGIN
        DATE.FORMAT% = VAL(MID$(TEMP$, 5, 1))
        TIME.FORMAT% = VAL(MID$(TEMP$, 6, 1))                               !CSWM
    ENDIF ELSE BEGIN
        DATE.FORMAT% = 2
        TIME.FORMAT% = 1                                                    !CSWM
    ENDIF

    IF DATE.FORMAT% = 1 OR DATE.FORMAT% = 2 THEN BEGIN                      !CSWM
        CHAR.DATE$ = "/"                                                    !CSWM
    ENDIF ELSE BEGIN                                                        !CSWM
        CHAR.DATE$ = "."                                                    !CSWM
    ENDIF                                                                   !CSWM

    IF TIME.FORMAT% = 1 THEN BEGIN                                          !CSWM
        CHAR.TIME$ = ":"                                                    !CSWM
    ENDIF ELSE BEGIN                                                        !CSWM
        CHAR.TIME$ = "."                                                    !CSWM
    ENDIF                                                                   !CSWM

    RC% = INITDM ("£AAAA")

    DISPLAY.FILE$ = "ADX_UPGM:" + LEFT$(MODULE.NUMBER$, 3) + "D" +          \
        MID$(MODULE.NUMBER$, 4, 2) + ".DAT"
    HELP.FILE$ = "ADX_UPGM:" + LEFT$(MODULE.NUMBER$, 3) + "H" +             \
        MID$(MODULE.NUMBER$, 4, 2) + ".DAT"
    RC% = OPNDIS (DISPLAY.FILE$)

    DM.INIT = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.QUIT
!***    Frees resources allocated by DM.INIT
!***

FUNCTION DM.QUIT PUBLIC

    INTEGER*1   DM.QUIT

    DM.QUIT = 1

    RC% = CLSDIS

    CURRENT.SCREEN% = 0
    ?F39.CF% = 0                                                            !DSWM
    ?F39.CF$ = ""                                                           !DSWM
    FIELD.COUNT% = 0
    FIRST.HELP.SCREEN% = 0
    LAST.HELP.SCREEN% = 0

    DIM MESSAGE$(FIELD.COUNT%)
    DIM VALID$(FIELD.COUNT%)
    DIM VISIBLE$(FIELD.COUNT%)
    DIM FIELD$(FIELD.COUNT%)
    DIM NAME$(FIELD.COUNT%)                                                 !DSWM
    DIM NEXT.INPUT%(FIELD.COUNT%)                                           !DSWM
    DIM PREV.INPUT%(FIELD.COUNT%)                                           !DSWM

    DIM FN.VISIBLE$(0)                                                      !ESWM
    DIM FIELD.FLAGS%(0)                                                     !ESWM
    DIM SAVED.FUNC.KEY$(0)                                                  !GCSK

    DISPLAY.FILE$ = ""
    HELP.FILE$ = ""

    DM.QUIT = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.SHOW.FN.KEY
!***    Makes a function key name visible with optional message
!***

FUNCTION DM.SHOW.FN.KEY (KEY.NUM%, MESSAGE$) PUBLIC

    INTEGER*1   DM.SHOW.FN.KEY
    INTEGER*2   KEY.NUM%
    STRING      MESSAGE$

    DM.SHOW.FN.KEY = 1

    !Move cursor to field
    RC% = POSF(F.FIELD% + KEY.NUM%)

    !Populate with optional message (otherwise use default)
    IF MESSAGE$ <> "" THEN BEGIN
        RC% = PUTF(MESSAGE$)
    ENDIF

    !Make field visible
    FN.VISIBLE$(KEY.NUM%) = TOKENISE$("TRUE")                               !ASWM
    CALL DM.CHECK.ERROR                                                     !DSWM
    TEMP$ = SETF("0")

    DM.SHOW.FN.KEY = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.HIDE.FN.KEY
!***    Makes a function key invisible
!***

FUNCTION DM.HIDE.FN.KEY (KEY.NUM%) PUBLIC

    INTEGER*1   DM.HIDE.FN.KEY
    INTEGER*2   KEY.NUM%

    DM.HIDE.FN.KEY = 1

    !Move cursor to field
    RC% = POSF(F.FIELD% + KEY.NUM%)

    !Make field invisible
    FN.VISIBLE$(KEY.NUM%) = TOKENISE$("FALSE")                              !ASWM
    CALL DM.CHECK.ERROR                                                     !DSWM
    TEMP$ = SETF("1")

    DM.HIDE.FN.KEY = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.FN.KEY.VISIBLE
!***    Sets the visibility expression for a function key
!***    This is the legacy function which takes a reverse polish expression
!***

FUNCTION DM.FN.KEY.VISIBLE (KEY.NUM%, EXPR$) PUBLIC                         !ASWM

    INTEGER*1   DM.FN.KEY.VISIBILITY                                        !ASWM
    INTEGER*2   KEY.NUM%                                                    !ASWM
    STRING      EXPR$                                                       !ASWM

    DM.FN.KEY.VISIBILITY = 1                                                !ASWM

    FN.VISIBLE$(KEY.NUM%) = TOKENISE$(EXPR$)                                !ASWM
    CALL DM.CHECK.ERROR                                                     !DSWM

    DM.FN.KEY.VISIBILITY = 0                                                !ASWM

END FUNCTION                                                                !ASWM


!******************************************************************************
!***
!***    DM.FN.VISIBLE
!***    Sets the visibility expression for a function key
!***    This is the new version which takes a infix notation expression
!***

FUNCTION DM.FN.VISIBLE (KEY.NUM%, EXPR$) PUBLIC                             !DSWM

    INTEGER*1   DM.FN.VISIBILITY                                            !DSWM
    INTEGER*2   KEY.NUM%                                                    !DSWM
    STRING      EXPR$                                                       !DSWM

    DM.FN.VISIBILITY = 1                                                    !DSWM
    EXPR$ = POLISH$(EXPR$)                                                  !DSWM
    CALL DM.CHECK.ERROR                                                     !DSWM
    DM.FN.VISIBILITY = DM.FN.KEY.VISIBLE (KEY.NUM%, EXPR$)                  !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.SHOW.SCREEN
!***    Displays a screen prior to accepting input
!***

FUNCTION DM.SHOW.SCREEN (SCREEN%, TITLE$, FIRST.HELP%, LAST.HELP%) PUBLIC

    INTEGER*1   DM.SHOW.SCREEN
    INTEGER*2   SCREEN%
    STRING      TITLE$
    INTEGER*2   FIRST.HELP%
    INTEGER*2   LAST.HELP%

    DM.SHOW.SCREEN = 1

    !Save current screen number, title and help screen range
    CURRENT.SCREEN% = SCREEN%
    CURRENT.TITLE$ = TITLE$
    FIRST.HELP.SCREEN% = FIRST.HELP%
    LAST.HELP.SCREEN% = LAST.HELP%
!E  DATE.FIELDS$ = ""
!E  TIME.FIELDS$ = ""                                                       !CSWM

    FIELDS.CHANGED = FALSE

    RC% = DISPD(CURRENT.SCREEN%)

    !Cursor off
    TEMP$ = CURS("1")

    CALL DISPLAY.TITLE.AND.DATE

    FIELD.COUNT% = 0
    RC% = NXTF(-10)
    WHILE RC% > 0
        IF RC% > FIELD.COUNT% AND RC% < 230 THEN BEGIN
            FIELD.COUNT% = RC%
        ENDIF
        RC% = NXTF(1)
    WEND

    CALL EVAL.CLEAR.ALL.VARIABLES                                           !DSWM
    DIM MESSAGE$(FIELD.COUNT%)
    DIM VALID$(FIELD.COUNT%)
    DIM VISIBLE$(FIELD.COUNT%)
    DIM FIELD$(FIELD.COUNT%)
    DIM NAME$(FIELD.COUNT%)                                                 !DSWM
    DIM NEXT.INPUT%(FIELD.COUNT%)                                           !DSWM
    DIM PREV.INPUT%(FIELD.COUNT%)                                           !DSWM

    DIM FN.VISIBLE$(10)                                                     !ASWM
    DIM FIELD.FLAGS%(255)                                                   !ESWM

    VALIDATE.KEYS$ = ":" + STR$(ENTER.KEY%) + ":"                           !BSWM

    IF FIELD.TAB.ORDER = DISPLAY.ORDER THEN BEGIN                           !DSWM
        CALL SET.CF(NXTF(-20))                                              !DSWM
    ENDIF ELSE BEGIN                                                        !DSWM
        CALL SET.CF(2)                                                      !DSWM
    ENDIF                                                                   !DSWM

    DM.SHOW.SCREEN = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.DATE.FIELD
!***    Defines a field as containing a date
!***

FUNCTION DM.DATE.FIELD (FIELD%) PUBLIC

    INTEGER*1   DM.DATE.FIELD
    INTEGER*2   FIELD%

    DM.DATE.FIELD = 1

!E  DATE.FIELDS$ = DATE.FIELDS$ + CHR$(FIELD%)
    FIELD.FLAGS%(FIELD%) = FIELD.FLAGS%(FIELD%) OR DATE.FLAG%               !ESWM

    DM.DATE.FIELD = 0

END FUNCTION


!******************************************************************************
!***
!***    DM.TIME.FIELD
!***    Defines a field as containing a time
!***

FUNCTION DM.TIME.FIELD (FIELD%) PUBLIC                                      !CSWM

    INTEGER*1   DM.TIME.FIELD                                               !CSWM
    INTEGER*2   FIELD%                                                      !CSWM

    DM.TIME.FIELD = 1                                                       !CSWM

!E  TIME.FIELDS$ = TIME.FIELDS$ + CHR$(FIELD%)                              !CSWM
    FIELD.FLAGS%(FIELD%) = FIELD.FLAGS%(FIELD%) OR TIME.FLAG%               !ESWM

    DM.TIME.FIELD = 0                                                       !CSWM

END FUNCTION                                                                !CSWM


!******************************************************************************
!***
!***    DM.RO.FIELD
!***    Defines an input field as read only
!***

FUNCTION DM.RO.FIELD (FIELD%) PUBLIC                                        !ESWM

    INTEGER*1   DM.RO.FIELD                                                 !ESWM
    INTEGER*2   FIELD%                                                      !ESWM

    DM.RO.FIELD = 1                                                         !ESWM

    FIELD.FLAGS%(FIELD%) = FIELD.FLAGS%(FIELD%) OR RO.FLAG%                 !ESWM

    DM.RO.FIELD = 0                                                         !ESWM

END FUNCTION                                                                !ESWM


!******************************************************************************
!***
!***    DM.RW.FIELD
!***    Defines an input field as read write
!***

FUNCTION DM.RW.FIELD (FIELD%) PUBLIC                                        !ESWM

    INTEGER*1   DM.RW.FIELD                                                 !ESWM
    INTEGER*2   FIELD%                                                      !ESWM

    DM.RW.FIELD = 1                                                         !ESWM

    FIELD.FLAGS%(FIELD%) = FIELD.FLAGS%(FIELD%) AND NOT RO.FLAG%            !ESWM

    DM.RW.FIELD = 0                                                         !ESWM

END FUNCTION                                                                !ESWM


!******************************************************************************
!***
!***    DM.DISPLAY.MESSAGE
!***    Displays a message imediately on the status line (field 1)
!***    This is the legacy function which takes a reverse polish expression
!***

FUNCTION DM.DISPLAY.MESSAGE (MESSAGE$) PUBLIC

    INTEGER*1   DM.DISPLAY.MESSAGE
    STRING      MESSAGE$

    DM.FIELD% = MESSAGE.FIELD%
    RC% = POSF(DM.FIELD%)
    TEMP$ = EVAL$(MESSAGE$)
    CALL DM.CHECK.ERROR                                                     !DSWM
    RC% = PUTF(TEMP$)

END FUNCTION


!******************************************************************************
!***
!***    DM.STATUS
!***    Displays a message imediately on the status line (field 1)
!***    This is the new function which takes a infix expression
!***

FUNCTION DM.STATUS (MESSAGE$) PUBLIC                                        !DSWM

    INTEGER*1   DM.STATUS                                                   !DSWM
    STRING      MESSAGE$                                                    !DSWM

    MESSAGE$ = POLISH$(MESSAGE$)                                            !DSWM
    CALL DM.CHECK.ERROR                                                     !DSWM
    DM.STATUS = DM.DISPLAY.MESSAGE(MESSAGE$)                                !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.VALIDATE.KEY
!***    Defines a key as being a validation key
!***

FUNCTION DM.VALIDATE.KEY (KEY%) PUBLIC                                      !BSWM

    INTEGER*1   DM.VALIDATE.KEY                                             !BSWM
    INTEGER*2   KEY%                                                        !BSWM

    DM.VALIDATE.KEY = 1                                                     !BSWM
    VALIDATE.KEYS$ = VALIDATE.KEYS$ + STR$(KEY%) + ":"                      !BSWM
    DM.VALIDATE.KEY = 0                                                     !BSWM

END FUNCTION                                                                !BSWM


!******************************************************************************
!***
!***    DM.INVISBLE.INPUT
!***    Gets an input key from the invisible input field
!***

FUNCTION DM.INVISIBLE.INPUT (MESSAGE$) PUBLIC

    STRING      MESSAGE$
    INTEGER*2   DM.INVISIBLE.INPUT

    !Turn cursor off
    TEMP$ = CURS("1")

    !Show required message
    CALL DM.DISPLAY.MESSAGE (MESSAGE$)

    !Position cursor and wait for input
    DM.FIELD% = INVISIBLE.FIELD%
    RC% = POSF(DM.FIELD%)
    RC% = PUTF("")
    INVISIBLE.FIELD$ = UPDF

    DM.INVISIBLE.INPUT = ENDF

END FUNCTION


!******************************************************************************
!***
!***    DM.INVISBLE.FIELD
!***    Gets the last contents of the invisible input field
!***

FUNCTION DM.INVISIBLE.FIELD PUBLIC

    STRING DM.INVISIBLE.FIELD

    DM.INVISIBLE.FIELD = INVISIBLE.FIELD$

END FUNCTION


!******************************************************************************
!***
!***    DM.INVISBLE.FIELD
!***    Gets or sets the current input field on the display
!***

FUNCTION DM.CURRENT.FIELD (NEW.FIELD%) PUBLIC

    INTEGER*2   DM.CURRENT.FIELD
    INTEGER*2   NEW.FIELD%

    IF NEW.FIELD% > 0 THEN BEGIN
        CALL SET.CF(NEW.FIELD%)                                             !DSWM
    ENDIF

    DM.CURRENT.FIELD = ?F39.CF%                                             !DSWM

END FUNCTION


!******************************************************************************
!***
!***    DM.INVISBLE.FIELD
!***    Gets or sets the current flag indicating if the screen contents
!***    have been modified.
!***

FUNCTION DM.CHANGED.FLAG (FLAG%) PUBLIC                                     !BSWM

    INTEGER*1   FLAG%                                                       !BSWM
    INTEGER*1   DM.CHANGED.FLAG                                             !BSWM

    IF FLAG% = TRUE OR FLAG% = FALSE THEN BEGIN                             !BSWM
        FIELDS.CHANGED = FLAG%                                              !BSWM
    ENDIF                                                                   !BSWM

    DM.CHANGED.FLAG = FIELDS.CHANGED                                        !BSWM

END FUNCTION                                                                !BSWM


!******************************************************************************
!***
!***    DM.TAB.ORDER
!***    Gets or sets the active tab ordering for fields
!***

FUNCTION DM.TAB.ORDER (ORDER%) PUBLIC                                       !DSWM

    INTEGER*1   ORDER%                                                      !DSWM
    INTEGER*1   DM.TAB.ORDER                                                !DSWM

    IF ORDER% = DISPLAY.ORDER OR ORDER% = NUMBER.ORDER THEN BEGIN           !DSWM
         FIELD.TAB.ORDER = ORDER%                                           !DSWM
    ENDIF                                                                   !DSWM

    DM.TAB.ORDER = FIELD.TAB.ORDER                                          !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.PROCESS.SCREEN
!***    Performs the bulk of the processing on the screen
!***

FUNCTION DM.PROCESS.SCREEN (FIRST.FIELD%, LAST.FIELD%, CONFIRM) PUBLIC

    INTEGER*2   DM.PROCESS.SCREEN
    INTEGER*2   FIRST.FIELD%
    INTEGER*2   LAST.FIELD%
    INTEGER*2   PREV.FIELD%
    INTEGER*1   CONFIRM
    INTEGER*1   READ.ONLY                                                   !ESWM

    !Turn cursor off
    TEMP$ = CURS("1")

    !Validate passed field indecies are within range
    !This allows defaults to be used (0,0) for the whole screen
    IF FIRST.FIELD% < 2 OR FIRST.FIELD% > FIELD.COUNT% THEN BEGIN           !DSWM
        FIRST.FIELD% = 2                                                    !DSWM
    ENDIF                                                                   !DSWM
    IF LAST.FIELD% < 2 OR LAST.FIELD% > FIELD.COUNT% THEN BEGIN             !DSWM
        LAST.FIELD% = FIELD.COUNT%                                          !DSWM
    ENDIF                                                                   !DSWM

    !Set visibility of fields on display
    GOSUB SET.VISIBILITY.AND.FIELDS

    !Move to first visible input field
    DM.FIELD% = ?F39.CF%                                                    !DSWM
    GOSUB MOVE.FORWARD.VISIBLE
    !But if we are now past the last displayed field, move back
    GOSUB MOVE.BACKWARD.VISIBLE                                             !DSWM

    KEY% = 0
    RESUME.INPUT = FALSE
    EXIT.FLAG = FALSE

    WHILE NOT EXIT.FLAG

        IF FIELD$(1) <> "" THEN BEGIN
            CALL DM.DISPLAY.MESSAGE(FIELD$(MESSAGE.FIELD%))
        ENDIF

        READ.ONLY = IS.RO.FIELD(?F39.CF%)                                   !ESWM

        !Put cursor in current field
        RC% = POSF(?F39.CF%)                                                !DSWM

        IF ?F39.CF% <> INVISIBLE.FIELD% THEN BEGIN                          !DSWM

            !Cursor on for input
            IF NOT READ.ONLY THEN TEMP$ = CURS("0")                         !ESWM

            !Set attributes for input field
            ATTRIB$ = SETF("")
            TEMP$ = SETF("3113333333311333")
            PREV.VALUE$ = FIELD$(?F39.CF%)                                  !DSWM

        ENDIF

        !For read-only fields, we must take input from the invisible field
        IF READ.ONLY THEN BEGIN                                             !ESWM
            RC% = POSF(INVISIBLE.FIELD%)                                    !ESWM
        ENDIF                                                               !ESWM

        IF RESUME.INPUT THEN BEGIN
            VALUE$ = RESF(0)
            RESUME.INPUT = FALSE
        ENDIF ELSE BEGIN
            VALUE$ = UPDF
        ENDIF

        !For read-only fields, we must restore current field from invisible
        IF READ.ONLY THEN BEGIN                                             !ESWM
            RC% = POSF(?F39.CF%)                                            !ESWM
            VALUE$ = PREV.VALUE$                                            !ESWM
        ENDIF                                                               !ESWM

        IF ?F39.CF% <> INVISIBLE.FIELD% THEN BEGIN                          !DSWM

            !Restore field atributes
            TEMP$ = SETF(ATTRIB$)

            !Cursor off after input
            TEMP$ = CURS("1")

        ENDIF

        !Input date/time field needs conversion?
        IF IS.DATE.FIELD(?F39.CF%) THEN BEGIN                               !DSWM
            VALUE$ = INT.DATE$ (VALUE$)
        ENDIF ELSE IF IS.TIME.FIELD(?F39.CF%) THEN BEGIN                    !DSWM
            VALUE$ = INT.TIME$ (VALUE$)                                     !CSWM
        ENDIF                                                               !CSWM

        GOSUB UPDATE.FIELD.VALUE                                            !DSWM

        !Get key which terminated entry
        CONFIRM.KEY% = KEY%
        KEY% = ENDF

        !Confirmation is invalid if changed field
        IF VALUE$ <> PREV.VALUE$ AND ?F39.CF% <> INVISIBLE.FIELD% THEN BEGIN!DSWM
            FIELDS.CHANGED = TRUE
            CONFIRM.KEY% = 0
        ENDIF

        !Remove any previously visible error
        IF FIELD$(1) <> "" THEN BEGIN
            RC% = POSF(MESSAGE.FIELD%)
            RC% = PUTF("")
            FIELD$(1) = ""
        ENDIF

        !Validate input
        VALID = TRUE

        IF ?F39.CF% <> INVISIBLE.FIELD% THEN BEGIN                          !DSWM

            IF LEN(VALID$(?F39.CF%)) THEN BEGIN                             !DSWM
                DM.FIELD% = ?F39.CF%                                        !DSWM
                VALID = EVAL%(VALID$(DM.FIELD%))
                CALL DM.CHECK.ERROR
            ENDIF

            !Call user exit
            DM.FIELD% = ?F39.CF%                                            !DSWM
            DM.SCREEN% = CURRENT.SCREEN%
            UPDATE = FALSE
            CALL DM.FIELD.CHANGED (DM.SCREEN%, DM.FIELD%, VALUE$, VALID, UPDATE)


            !If fields updated by user exit
            IF UPDATE THEN BEGIN
                GOSUB REFRESH.FIELDS
            ENDIF

        ENDIF

        IF KEY% = ESC.KEY% THEN BEGIN

            !If confirmation required
            IF CONFIRM AND KEY% <> CONFIRM.KEY% AND FIELDS.CHANGED THEN BEGIN

                !B182 Data already keyed will be lost -
                !Press ESC to QUIT or ENTER to continue
                FIELD$(1) = "183 MESSAGE"
                RESUME.INPUT = TRUE

            ENDIF ELSE BEGIN

                GOSUB UPDATE.FIELD.VALUE                                    !DSWM
                EXIT.FLAG = TRUE

            ENDIF

        ENDIF ELSE IF KEY% = F3.KEY% THEN BEGIN

            !If confirmation required
            IF CONFIRM AND KEY% <> CONFIRM.KEY% AND FIELDS.CHANGED THEN BEGIN

                !B183 Data already keyed will be lost -
                !Press F3 to QUIT or ENTER to continue
                FIELD$(1) = "182 MESSAGE"
                RESUME.INPUT = TRUE

            ENDIF ELSE BEGIN

                GOSUB UPDATE.FIELD.VALUE                                    !DSWM
                EXIT.FLAG = TRUE

            ENDIF

        ENDIF ELSE IF KEY% = F1.KEY% THEN BEGIN

            !Display help screen
            GOSUB DISPLAY.HELP

        ENDIF ELSE BEGIN !Any other key

            !Field is valid, or blank
            IF VALID OR VALUE$ = "" THEN BEGIN

!BSWM               !Adjust visibiliity of all fields on display
!BSWM               GOSUB REFRESH.VISIBILITY
REPARSE.KEY:                                                                !DSWM
                IF KEY% = HOME.KEY% THEN BEGIN

                    !Move forward to first visible input field
                    DM.FIELD% = FIRST.FIELD%
                    IF FIELD.TAB.ORDER = DISPLAY.ORDER THEN BEGIN           !DSWM
                        DM.FIELD% = NXTF(-20)   !First input field          !DSWM
                    ENDIF                                                   !DSWM
                    GOSUB MOVE.FORWARD.VISIBLE

                ENDIF ELSE IF KEY% = END.KEY% THEN BEGIN

                    !Move backward to last visible input field
                    DM.FIELD% = LAST.FIELD%
                    IF FIELD.TAB.ORDER = DISPLAY.ORDER THEN BEGIN           !DSWM
                        DM.FIELD% = NXTF(20)    !Last input field           !DSWM
                    ENDIF                                                   !DSWM
                    GOSUB MOVE.BACKWARD.VISIBLE

                ENDIF ELSE IF KEY% = BTAB.KEY% OR KEY% = UP.KEY% THEN BEGIN !CSWM

                    !Move backward to prev visible input field
                    DM.FIELD% = ?F39.CF%                                    !DSWM
                    PREV.FIELD% = ?F39.CF%                                  !DSWM
                    GOSUB MOVE.BACKWARD.FIELD
                    IF ?F39.CF% = PREV.FIELD% THEN BEGIN                    !DSWM
                        KEY% = PREV.KEY%                                    !DSWM
                        GOTO REPARSE.KEY                                    !DSWM
                    ENDIF                                                   !DSWM

                ENDIF ELSE IF KEY% = TAB.KEY% OR KEY% = DOWN.KEY% THEN BEGIN!CSWM

                    !Move forward to next visible input field
                    DM.FIELD% = ?F39.CF%                                    !DSWM
                    PREV.FIELD% = ?F39.CF%                                  !DSWM
                    GOSUB MOVE.FORWARD.FIELD
                    IF ?F39.CF% = PREV.FIELD% THEN BEGIN                    !DSWM
                        KEY% = NEXT.KEY%                                    !DSWM
                        GOTO REPARSE.KEY                                    !DSWM
                    ENDIF                                                   !DSWM

                ENDIF ELSE IF MATCH(":"+STR$(KEY%)+":", VALIDATE.KEYS$, 1) THEN BEGIN

                    GOSUB VALIDATE.INPUT.FIELDS

                    IF VALID THEN BEGIN
                        EXIT.FLAG = TRUE
                    ENDIF ELSE BEGIN
                        CALL SET.CF(DM.FIELD%)                              !DSWM
                        IF MESSAGE$(?F39.CF%) = "" THEN BEGIN               !DSWM
                            !B166 Some fields have invalid data
                            FIELD$(1) = "166 MESSAGE"
                        ENDIF ELSE BEGIN
                            FIELD$(1) = MESSAGE$(?F39.CF%)                  !DSWM
                        ENDIF
                    ENDIF


                ENDIF ELSE BEGIN

                    EXIT.FLAG = TRUE

                ENDIF

                !Adjust visibiliity of all fields on display                !BSWM
                GOSUB REFRESH.VISIBILITY                                    !BSWM
                GOSUB REFRESH.FN.KEY.VISIBILITY                             !ASWM

            !Else invalid field, so if no message set up in user exit
            ENDIF ELSE IF FIELD$(1) = "" THEN BEGIN

                IF MESSAGE$(?F39.CF%) = "" THEN BEGIN                       !DSWM
                    !B166 Some fields have invalid data
                    FIELD$(1) = "166 MESSAGE"
                ENDIF ELSE BEGIN
                    FIELD$(1) = MESSAGE$(?F39.CF%)                          !DSWM
                ENDIF

            ENDIF

        ENDIF

    WEND

    DM.PROCESS.SCREEN = KEY%

EXIT FUNCTION


MOVE.FORWARD.VISIBLE:

    DM.FIELD% = POSF(DM.FIELD%)
    IF IS.INPUT.FIELD THEN GOTO FORWARD.FIELD

MOVE.FORWARD.FIELD:

    DM.FIELD% = POSF(DM.FIELD%)
    IF FIELD.TAB.ORDER = DISPLAY.ORDER THEN BEGIN                           !DSWM
        DM.FIELD% = NXTF(2)                                                 !DSWM
    ENDIF ELSE BEGIN                                                        !DSWM
        DM.FIELD% = NEXT.INPUT%(DM.FIELD%)                                  !DSWM
    ENDIF                                                                   !DSWM

FORWARD.FIELD:
    IF DM.FIELD% <= 0 THEN RETURN
    IF DM.FIELD% > LAST.FIELD% THEN RETURN
    IF DM.FIELD% < FIRST.FIELD% THEN GOTO MOVE.FORWARD.FIELD

    IF IS.OO.FIELD(DM.FIELD%) THEN GOTO MOVE.FORWARD.FIELD                  !FNWB

    IF LEN(VISIBLE$(DM.FIELD%)) THEN BEGIN                                  !BSWM
        RC% = EVAL%(VISIBLE$(DM.FIELD%))                                    !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
        IF NOT RC% THEN GOTO MOVE.FORWARD.FIELD                             !DSWM
    ENDIF ELSE BEGIN                                                        !BSWM
        IF NOT IS.VISIBLE.FIELD THEN GOTO MOVE.FORWARD.FIELD
    ENDIF                                                                   !BSWM

    CALL SET.CF(DM.FIELD%)                                                  !DSWM

RETURN


MOVE.BACKWARD.VISIBLE:

    DM.FIELD% = POSF(DM.FIELD%)
    IF IS.INPUT.FIELD THEN GOTO BACKWARD.FIELD

MOVE.BACKWARD.FIELD:

    DM.FIELD% = POSF(DM.FIELD%)
    IF FIELD.TAB.ORDER = DISPLAY.ORDER THEN BEGIN                           !DSWM
        DM.FIELD% = NXTF(-2)                                                !DSWM
    ENDIF ELSE BEGIN                                                        !DSWM
        DM.FIELD% = PREV.INPUT%(DM.FIELD%)                                  !DSWM
    ENDIF                                                                   !DSWM

BACKWARD.FIELD:
    IF DM.FIELD% <= 0 THEN RETURN
    IF DM.FIELD% < FIRST.FIELD% THEN RETURN
    IF DM.FIELD% > LAST.FIELD% THEN GOTO MOVE.BACKWARD.FIELD

    IF IS.OO.FIELD(DM.FIELD%) THEN GOTO MOVE.BACKWARD.FIELD                 !FNWB

    IF LEN(VISIBLE$(DM.FIELD%)) THEN BEGIN                                  !BSWM
        RC% = EVAL%(VISIBLE$(DM.FIELD%))                                    !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
        IF NOT RC% THEN GOTO MOVE.BACKWARD.FIELD                            !DSWM
    ENDIF ELSE BEGIN                                                        !BSWM
        IF NOT IS.VISIBLE.FIELD THEN GOTO MOVE.BACKWARD.FIELD
    ENDIF

    CALL SET.CF(DM.FIELD%)                                                  !DSWM

RETURN


SET.VISIBILITY.AND.FIELDS:

    !We have not found an input field yet
    LAST.INPUT% = -1                                                        !DSWM

    FOR DM.FIELD% = 2 TO FIELD.COUNT%

        IF LEN(NAME$(DM.FIELD%)) THEN BEGIN                                 !DSWM
            FIELD$(DM.FIELD%) = EVAL.GET.VARIABLE$(NAME$(DM.FIELD%))        !DSWM
            CALL DM.CHECK.ERROR                                             !DSWM
        ENDIF                                                               !DSWM

        RC% = POSF(DM.FIELD%)
        !Set the previous input field pointer
        PREV.INPUT%(DM.FIELD%) = LAST.INPUT%                                !DSWM
        !If this is an input field
\       IF IS.INPUT.FIELD THEN BEGIN                                        !DSWM
        IF IS.INPUT.FIELD                                                   \FNWB
       AND NOT IS.OO.FIELD(DM.FIELD%) THEN BEGIN                            !FNWB
            !Set the forward pointer for all fields from the last input
            IF LAST.INPUT% = -1 THEN LAST.INPUT% = 2                        !DSWM
            FOR F% = LAST.INPUT% TO DM.FIELD% - 1                           !DSWM
                NEXT.INPUT%(F%) = DM.FIELD%                                 !DSWM
            NEXT F%                                                         !DSWM
            !The current field is the last input field found
            LAST.INPUT% = DM.FIELD%                                         !DSWM
        ENDIF                                                               !DSWM

!BSWM       VISIBLE = TRUE
        IF LEN(VISIBLE$(DM.FIELD%)) THEN BEGIN

            VISIBLE$(DM.FIELD%) = TOKENISE$(VISIBLE$(DM.FIELD%))
            VISIBLE = EVAL%(VISIBLE$(DM.FIELD%))
            CALL DM.CHECK.ERROR                                             !DSWM
            CURRENT.STATE = IS.VISIBLE.FIELD
            IF VISIBLE <> CURRENT.STATE THEN BEGIN
                IF DM.FIELD% >= FIRST.FIELD% AND DM.FIELD% <=LAST.FIELD% THEN BEGIN
                    TEMP$ = SETF("2")
                ENDIF
            ENDIF

        ENDIF                                                               !BSWM

        IF LEN(VALID$(DM.FIELD%)) THEN BEGIN
            VALID$(DM.FIELD%) = TOKENISE$(VALID$(DM.FIELD%))
        ENDIF

        TEMP$ = FIELD$(DM.FIELD%)
        IF LEN(TEMP$) THEN BEGIN
            IF IS.DATE.FIELD(DM.FIELD%) THEN BEGIN
                TEMP$ = EXT.DATE$ (TEMP$)
                IF IS.OUTPUT.FIELD THEN BEGIN
                    TEMP$ = SEP.DATE$ (TEMP$)                               !CSWM
                ENDIF
            ENDIF ELSE IF IS.TIME.FIELD(DM.FIELD%) THEN BEGIN               !CSWM
                TEMP$ = EXT.TIME$ (TEMP$)                                   !CSWM
                IF IS.OUTPUT.FIELD THEN BEGIN                               !CSWM
                    TEMP$ = SEP.TIME$ (TEMP$)                               !CSWM
                ENDIF                                                       !CSWM
            ENDIF
            RC% = PUTF(TEMP$)
        ENDIF ELSE IF IS.OUTPUT.FIELD THEN BEGIN
            TEMP$ = GETF
            IF IS.DATE.FIELD(DM.FIELD%) THEN BEGIN
                TEMP$ = MID$(TEMP$, 1, 2) +                                 \
                        MID$(TEMP$, 4, 2) +                                 \
                        MID$(TEMP$, 7, 2)
                TEMP$ = INT.DATE$ (TEMP$)
            ENDIF ELSE IF IS.TIME.FIELD(DM.FIELD%) THEN BEGIN               !CSWM
                TEMP$ = MID$(TEMP$, 1, 2) +                                 \CSWM
                        MID$(TEMP$, 4, 2) +                                 \CSWM
                        MID$(TEMP$, 7, 2)                                   !CSWM
                TEMP$ = INT.TIME$ (TEMP$)                                   !CSWM
            ENDIF
            FIELD$(DM.FIELD%) = TEMP$
            IF LEN(NAME$(DM.FIELD%)) THEN BEGIN                             !DSWM
                CALL EVAL.SET.VARIABLE (NAME$(DM.FIELD%), TEMP$)            !DSWM
                CALL DM.CHECK.ERROR                                         !DSWM
            ENDIF                                                           !DSWM

        ENDIF

    NEXT DM.FIELD%

    !Finally set the next input field pointer for all remaining fields
    IF LAST.INPUT% = -1 THEN LAST.INPUT% = 2                                !DSWM
    FOR F% = LAST.INPUT% TO FIELD.COUNT%                                    !DSWM
        NEXT.INPUT%(F%) = -1                                                !DSWM
    NEXT F%                                                                 !DSWM

    GOSUB REFRESH.FN.KEY.VISIBILITY                                         !ASWM

RETURN


REFRESH.FIELDS:

    FOR DM.FIELD% = FIRST.FIELD% TO LAST.FIELD%

        IF LEN(NAME$(DM.FIELD%)) THEN BEGIN                                 !DSWM
            FIELD$(DM.FIELD%) = EVAL.GET.VARIABLE$(NAME$(DM.FIELD%))        !DSWM
            CALL DM.CHECK.ERROR                                             !DSWM
        ENDIF                                                               !DSWM

        RC% = POSF(DM.FIELD%)

        TEMP$ = FIELD$(DM.FIELD%)
        IF LEN(TEMP$) THEN BEGIN
            IF IS.DATE.FIELD(DM.FIELD%) THEN BEGIN
                TEMP$ = EXT.DATE$(TEMP$)
                IF IS.OUTPUT.FIELD THEN BEGIN
                    TEMP$ = SEP.DATE$(TEMP$)                                !CSWM
                ENDIF
            ENDIF ELSE IF IS.TIME.FIELD(DM.FIELD%) THEN BEGIN               !CSWM
                TEMP$ = INT.TIME$(TEMP$)                                    !CSWM
                IF IS.OUTPUT.FIELD THEN BEGIN                               !CSWM
                    TEMP$ = SEP.TIME$(TEMP$)                                !CSWM
                ENDIF                                                       !CSWM
            ENDIF
            RC% = PUTF(TEMP$)
        ENDIF

    NEXT DM.FIELD%

RETURN


REFRESH.VISIBILITY:

    FOR DM.FIELD% = FIRST.FIELD% TO LAST.FIELD%

        RC% = POSF(DM.FIELD%)

!BSWM       VISIBLE = TRUE
        IF LEN(VISIBLE$(DM.FIELD%)) THEN BEGIN

            VISIBLE = EVAL%(VISIBLE$(DM.FIELD%))
            CALL DM.CHECK.ERROR
            CURRENT.STATE = IS.VISIBLE.FIELD
            IF VISIBLE <> CURRENT.STATE THEN BEGIN
                TEMP$ =  SETF("2")
            ENDIF

        ENDIF                                                               !BSWM

    NEXT DM.FIELD%

RETURN


REFRESH.FN.KEY.VISIBILITY:                                                  !ASWM

    FOR DM.FIELD% = 1 TO 10                                                 !ASWM

        RC% = POSF(F.FIELD% + DM.FIELD%)                                    !ASWM

        IF RC% = F.FIELD% + DM.FIELD% THEN BEGIN                            !ASWM
            IF LEN(FN.VISIBLE$(DM.FIELD%)) THEN BEGIN                       !ASWM
                VISIBLE = EVAL%(FN.VISIBLE$(DM.FIELD%))                     !ASWM
                CALL DM.CHECK.ERROR                                         !DSWM
                CURRENT.STATE = IS.VISIBLE.FIELD                            !ASWM
                IF VISIBLE <> CURRENT.STATE THEN BEGIN                      !ASWM
                    TEMP$ = SETF("2")                                       !ASWM
                ENDIF                                                       !ASWM
            ENDIF                                                           !ASWM
        ENDIF                                                               !ASWM

    NEXT DM.FIELD%                                                          !ASWM

RETURN                                                                      !ASWM


VALIDATE.INPUT.FIELDS:

    VALID = TRUE
    DM.FIELD% = 1

    WHILE DM.FIELD% < FIELD.COUNT% AND VALID

        DM.FIELD% = DM.FIELD% + 1

        RC% = POSF(DM.FIELD%)

        IF LEN(VISIBLE$(DM.FIELD%)) THEN BEGIN                              !BSWM
            VISIBLE = EVAL%(VISIBLE$(DM.FIELD%))                            !BSWM
            CALL DM.CHECK.ERROR                                             !DSWM
        ENDIF ELSE BEGIN                                                    !BSWM
            VISIBLE = IS.VISIBLE.FIELD                                      !BSWM
        ENDIF                                                               !BSWM

        IF VISIBLE THEN BEGIN                                               !BSWM
            IF IS.INPUT.FIELD THEN BEGIN
                IF LEN(VALID$(DM.FIELD%)) THEN BEGIN
                    VALID = EVAL%(VALID$(DM.FIELD%))
                    CALL DM.CHECK.ERROR                                     !DSWM
                ENDIF
            ENDIF
        ENDIF ELSE BEGIN
            IF IS.INPUT.FIELD THEN BEGIN
                IF LEN(VALID$(DM.FIELD%)) THEN BEGIN
                    RC% = EVAL%(VALID$(DM.FIELD%))                          !DSWM
                    CALL DM.CHECK.ERROR                                     !DSWM
                    IF NOT RC% THEN BEGIN                                   !DSWM
                        GOSUB UPDATE.BLANK.FIELD                            !DSWM
                        RC% = PUTF("")
                    ENDIF
                ENDIF
            ENDIF
        ENDIF

    WEND

RETURN


DISPLAY.HELP:

    ! Save Function Key field attributes so that                            !GCSk
    ! they can be re-instated correctly later                               !GCSk
    FOR KEY% = 0 TO 9                                                       !GCSk
         RC% = POSF(F1.FIELD% + KEY%)                                       !GCSk
         SAVED.FUNC.KEY$(KEY%) = GETF                                       !GCSk
    NEXT KEY%                                                               !GCSk

    RC% = CLSDIS
    RC% = OPNDIS(HELP.FILE$)

    !Turn cursor off
    TEMP$ = CURS("1")

    DM.SCREEN% = FIRST.HELP.SCREEN% - 1
    PAGE% = 1
    PAGE.COUNT% = LAST.HELP.SCREEN% - DM.SCREEN%

    KEY% = 0
    WHILE KEY% <> F1.KEY%

        RC% = DISPD(DM.SCREEN% + PAGE%)
        CALL DISPLAY.TITLE.AND.DATE

        IF PAGE.COUNT% <> 1 THEN BEGIN

            !Page count replaces date on multi-page help screen
            RC% = POSF(DATE.FIELD%)
            TEMP$ = "Page "+STR$(PAGE%)+" of "+STR$(PAGE.COUNT%)
            RC% = PUTF(TEMP$)

            IF PAGE% > 1 THEN BEGIN
                CALL DM.SHOW.FN.KEY (7, "")
            ENDIF

            IF PAGE% < PAGE.COUNT% THEN BEGIN
                CALL DM.SHOW.FN.KEY (8, "")
            ENDIF

        ENDIF

        UPDATE = FALSE
        WHILE NOT UPDATE

            RC% = POSF(INVISIBLE.FIELD%)
            TEMP$ = UPDF
            KEY% = ENDF

            IF KEY% = PGUP.KEY% OR KEY% = F7.KEY% THEN BEGIN

                IF PAGE% > 1 THEN BEGIN
                    PAGE% = PAGE% - 1
                    UPDATE = TRUE
                ENDIF

            ENDIF ELSE IF KEY% = PGDN.KEY% OR KEY% = F8.KEY% THEN BEGIN

                IF PAGE% < PAGE.COUNT% THEN BEGIN
                    PAGE% = PAGE% + 1
                    UPDATE = TRUE
                ENDIF

            ENDIF ELSE IF KEY% = F1.KEY% THEN BEGIN

                UPDATE = TRUE

            ENDIF

        WEND

    WEND

    RC% = CLSDIS
    RC% = OPNDIS(DISPLAY.FILE$)

    !Display current screen
    RC% = DISPD(CURRENT.SCREEN%)

    !Cursor off
    TEMP$ = CURS("1")

    ! Re-instate Function Key field attributes                              !GCSk
    ! that were saved earlier                                               !GCSk
    FOR KEY% = 0 TO 9                                                       !GCSk
         RC% = POSF(F1.FIELD% + KEY%)                                       !GCSk
         RC% = PUTF(SAVED.FUNC.KEY$(KEY%))                                  !GCSk
    NEXT KEY%                                                               !GCSk

    CALL DISPLAY.TITLE.AND.DATE

    GOSUB SET.VISIBILITY.AND.FIELDS

RETURN


UPDATE.FIELD.VALUE:                                                         !DSWM

    IF ?F39.CF% <> INVISIBLE.FIELD% THEN BEGIN                              !DSWM

        !Save updated field
        FIELD$(?F39.CF%) = VALUE$                                           !DSWM
        IF LEN(NAME$(?F39.CF%)) THEN BEGIN                                  !DSWM
            CALL EVAL.SET.VARIABLE (NAME$(?F39.CF%), VALUE$)                !DSWM
            CALL DM.CHECK.ERROR                                             !DSWM
        ENDIF                                                               !DSWM

    ENDIF                                                                   !DSWM

RETURN                                                                      !DSWM


UPDATE.BLANK.FIELD:                                                         !DSWM

    FIELD$(DM.FIELD%) = ""                                                  !DSWM
    CALL EVAL.SET.VARIABLE(NAME$(DM.FIELD%), "")                            !DSWM
    CALL DM.CHECK.ERROR                                                     !DSWM

RETURN                                                                      !DSWM

END FUNCTION


!******************************************************************************
!***
!***    DM.NAME
!***    Supplies a name for a field and associates with a variable
!***

SUB DM.NAME (FIELD%, VAR$, VALUE$) PUBLIC                                   !DSWM

    INTEGER*2 FIELD%                                                        !DSWM
    STRING VAR$                                                             !DSWM
    STRING VALUE$                                                           !DSWM

    VAR$ = UCASE$(VAR$)                                                     !DSWM
    NAME$(FIELD%) = VAR$                                                    !DSWM
    CALL EVAL.SET.UPDATABLE (VAR$, VALUE$)                                  !DSWM
    CALL SET.CF(?F39.CF%)                                                   !DSWM
    CALL DM.CHECK.ERROR                                                     !DSWM

END SUB                                                                     !DSWM


!******************************************************************************
!***
!***    DM.INDEX
!***    Returns the index number of a field, given its name
!***

FUNCTION DM.INDEX (FIELD$) PUBLIC                                           !DSWM

    STRING FIELD$                                                           !DSWM
    INTEGER*2 DM.INDEX                                                      !DSWM

    DM.INDEX = 0                                                            !DSWM
    FIELD$ = UCASE$(FIELD$)                                                 !DSWM
    !Field name is a numeric offset?
    IF LEFT$(FIELD$,1) >= "0" AND LEFT$(FIELD$,1) <= "9" THEN BEGIN         !DSWM
        F% = VAL(FIELD$)                                                    !DSWM
        IF F% < 2 OR F% > FIELD.COUNT% THEN F% = 0                          !DSWM
        DM.INDEX = F%                                                       !DSWM
    !Otherwise search for name of field
    ENDIF ELSE BEGIN                                                        !DSWM
        FOR F% = 2 TO FIELD.COUNT%                                          !DSWM
            IF NAME$(F%) = FIELD$ THEN BEGIN                                !DSWM
                DM.INDEX = F%                                               !DSWM
                EXIT FUNCTION                                               !DSWM
            ENDIF                                                           !DSWM
        NEXT F%                                                             !DSWM
    ENDIF                                                                   !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.VALID
!***    Sets the validation expression for a field in infix notation
!***

FUNCTION DM.VALID (FIELD$, VALUE$) PUBLIC                                   !DSWM

    STRING FIELD$                                                           !DSWM
    STRING VALUE$                                                           !DSWM
    INTEGER*2 DM.VALID                                                      !DSWM

    DM.VALID = 0                                                            !DSWM
    F% = DM.INDEX (FIELD$)                                                  !DSWM
    IF F% THEN BEGIN                                                        !DSWM
        VALID$(F%) = POLISH$(VALUE$)                                        !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
        DM.VALID = F%                                                       !DSWM
    ENDIF                                                                   !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.MESSAGE
!***    Sets the message expression for a field in infix notation
!***

FUNCTION DM.MESSAGE (FIELD$, VALUE$) PUBLIC                                 !DSWM

    STRING FIELD$                                                           !DSWM
    STRING VALUE$                                                           !DSWM
    INTEGER*2 DM.MESSAGE                                                    !DSWM

    DM.MESSAGE = 0                                                          !DSWM
    F% = DM.INDEX (FIELD$)                                                  !DSWM
    IF F% THEN BEGIN                                                        !DSWM
        MESSAGE$(F%) = POLISH$(VALUE$)                                      !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
        DM.MESSAGE = F%                                                     !DSWM
    ENDIF                                                                   !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.FLD.ATT
!***    Sets field attributes
!***

SUB DM.FLD.ATT (FIELD$, VALUE$) PUBLIC                                      !FNWB

    STRING      FIELD$                                                      !FNWB
    STRING      VALUE$                                                      !FNWB

    F% = DM.INDEX (FIELD$)                                                  !FNWB
    IF F% THEN BEGIN                                                        !FNWB
        RC% = POSF(F%)                                                      !FNWB
        TEMP$ = SETF(VALUE$)                                                !FNWB
        VALUE$ = TEMP$                                                      !FNWB
    ENDIF                                                                   !FNWB

END SUB                                                                     !FNWB


!******************************************************************************
!***
!***    DM.OO.FIELD
!***    Defines a field to be treated as an output only field
!***

FUNCTION DM.OO.FIELD (FIELD$) PUBLIC                                        !FNWB

    INTEGER*1   DM.OO.FIELD                                                 !FNWB
    STRING      FIELD$                                                      !FNWB

    DM.OO.FIELD = 1                                                         !FNWB

    F% = DM.INDEX (FIELD$)                                                  !FNWB
    IF F% THEN BEGIN                                                        !FNWB
        FIELD.FLAGS%(F%) = FIELD.FLAGS%(F%) OR OUT.FLAG%                    !FNWB
    ENDIF                                                                   !FNWB

END FUNCTION                                                                !FNWB


!******************************************************************************
!***
!***    DM.IO.FIELD
!***    Turns off the DM.OO.FIELD setting
!***

FUNCTION DM.IO.FIELD (FIELD$) PUBLIC                                        !FNWB

    INTEGER*1   DM.IO.FIELD                                                 !FNWB
    STRING      FIELD$                                                      !FNWB

    DM.IO.FIELD = 1                                                         !FNWB

    F% = DM.INDEX (FIELD$)                                                  !FNWB
    IF F% THEN BEGIN                                                        !FNWB
        FIELD.FLAGS%(F%) = FIELD.FLAGS%(F%) AND NOT OUT.FLAG%               !FNWB
    ENDIF                                                                   !FNWB

END FUNCTION                                                                !FNWB


!******************************************************************************
!***
!***    DM.VISIBLE
!***    Sets the visibility expression for a field in infix notation
!***

FUNCTION DM.VISIBLE (FIELD$, VALUE$) PUBLIC                                 !DSWM

    STRING FIELD$                                                           !DSWM
    STRING VALUE$                                                           !DSWM
    INTEGER*2 DM.VISIBLE                                                    !DSWM

    DM.VISIBLE = 0                                                          !DSWM
    F% = DM.INDEX (FIELD$)                                                  !DSWM
    IF F% THEN BEGIN                                                        !DSWM
        VISIBLE$(F%) = POLISH$(VALUE$)                                      !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
        DM.VISIBLE = F%                                                     !DSWM
    ENDIF                                                                   !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************
!***
!***    DM.VISIBLE
!***    Changes the input focus to the specified field, optionally sets
!***    a status message for display in field 1.
!***

FUNCTION DM.FOCUS (FLD$, VALUE$) PUBLIC                                     !DSWM

    STRING FLD$                                                             !DSWM
    STRING VALUE$                                                           !DSWM
    INTEGER*2 DM.FOCUS                                                      !DSWM

    DM.FOCUS = 0                                                            !DSWM
    IF LEN(FLD$) THEN BEGIN                                                 !DSWM
        F% = DM.INDEX (FLD$)                                                !DSWM
        IF F% THEN CALL DM.CURRENT.FIELD(F%)                                !DSWM
    ENDIF ELSE BEGIN                                                        !DSWM
        F% = ?F39.CF%                                                       !DSWM
    ENDIF                                                                   !DSWM

    IF LEN(VALUE$) THEN BEGIN                                               !DSWM
        FIELD$(1) = POLISH$(VALUE$)                                         !DSWM
        CALL DM.CHECK.ERROR                                                 !DSWM
    ENDIF                                                                   !DSWM

END FUNCTION                                                                !DSWM


!******************************************************************************

