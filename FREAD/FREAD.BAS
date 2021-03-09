\***********************************************************************
\*
\* Program: FREAD
\*
\* FREAD utility helps the user to read all types of files
\* Sequential, Direct and Keyed Files
\*
\* Version B               Sreemol Mini                      25 MAY 2017
\*   To change FREAD to a screen program
\*
\***********************************************************************

%INCLUDE PSBF20G.J86        ! Session number utility
%INCLUDE PSBF39G.J86        ! Display Manager                           !BSM

STRING GLOBAL               \                                           !BSM
    MODULE.NUMBER$          ! Module number                             !BSM

STRING                      \
    FREAD.RECORD$,          \
    FIELD.LEN$,             \
    RECORD$,                \
    FIELD.TYPE.ARRAY$(1),   \
    FINAL.FIELD.VALUE$,     \
    FIELD.VALUE$,           \
    FIELD.STRING$,          \                                           !BSM
    WRITE.FORM$,            \                                           !BSM
    INPUT.FILE.NAME$,       \
    FREAD.FILE.NAME$,       \
    OUTPUT.FILE.NAME$,      \
    NO.OF.FIELDS$,          \                                           !BSM
    FUNCTION.FLAG$,         \
    FIELD.NAME$,            \
    FIELD.TYPE$,            \
    FINAL.FIELD.NAMES$,     \
    FORM$,                  \
    FILE.OPERATION$,        \
    FILE.TYPE$,             \
    FILES.TYPE$,            \                                           !BSM
    FILE.RECL$,             \
    FIELD.NOS$,             \
    ERROR$,                 \
    FILE.NAME$,             \
    RECORD.LENGTH$,         \                                           !BSM
    FLD$,                   \                                           !BSM
    TEMP.FORM$,             \
    CRLF$,                  \
    INPUT.FILE$,            \                                           !BSM
    DM.INPUT.FILE$,         \                                           !BSM
    DM.FILE.TYPE$,          \                                           !BSM
    DM.RECORD.LENGTH$,      \                                           !BSM
    DM.NO.OF.FIELDS$,       \                                           !BSM
    DM.FIELD.NAME$,         \                                           !BSM
    DM.FIELD.TYPE$                                                      !BSM

INTEGER*1                   \
    EOF,                    \
    FREAD.OPEN.FLAG,        \
    OUTPUT.FILE.OPEN.FLAG,  \
    FREAD.RECORD.PRESENT,   \
    FREAD.SESS.NUM.FLAG,    \
    OUTPUT.SESS.NUM.FLAG,   \
    FILE.OPEN.FLAG,         \
    FILE.EXIST.FLAG,        \                                           !BSM
    FILE.SESS.NUM.FLAG,     \
    FALSE,                  \
    TRUE,                   \
    INVALID.TYPE,           \                                           !BSM
    INVALID,                \                                           !BSM
    READ.FILE.NAME,         \
    READ.FREAD.FILE.NAME

INTEGER*2                   \
    FIELD.OFFSET%,          \
    DM.OFFSET%,             \                                           !BSM
    FILE.LENGTH%,           \                                           !BSM
    FREAD.REPORT.NUM%,      \
    OUTPUT.FILE.REPORT.NUM%,\
    FREAD.SESS.NUM%,        \
    SLASH.POSITION%,        \                                           !BSM
    OUTPUT.FILE.SESS.NUM%,  \
    FILE.REPORT.NUM%,       \
    FILE.SESS.NUM%,         \
    COUNTER%,               \
    VALID%,                 \                                           !BSM
    FIELD.COUNT%,           \                                           !BSM
    CURRENT.REPORT.NUM%, A%,B%,C%,D%,\
    RET.KEY%,               \                                           !BSM
    START.FIELD%                                                        !BSM

INTEGER*4                   \
    LOOP%,                  \
    FIELD.NOS%,             \
    TEMP.OFFSET%,           \
    RECORD.NOS%,            \
    COUNT%

%INCLUDE QXLFUNC2.J86                                                   !BSM

%INCLUDE BTCMEM.J86
%INCLUDE PSBF20E.J86        ! Session number utility
%INCLUDE PSBF30E.J86
%INCLUDE PSBF39E.J86        ! Display Manager                           !BSM


\***********************************************************************
\*
\* FUNC.PARSE.FIELD
\*
\**********************************************************************
FUNCTION FUNC.PARSE.FIELD$

    STRING FUNC.PARSE.FIELD$
    INTEGER*4 OFFSET%

    FUNC.PARSE.FIELD$ = "0"
    OFFSET% = MATCH(";", FREAD.RECORD$, 1)

    IF OFFSET% <> 0 THEN BEGIN
        FUNC.PARSE.FIELD$ = LEFT$(FREAD.RECORD$,OFFSET%-1)
        FREAD.RECORD$ = RIGHT$(FREAD.RECORD$,                          \
                               LEN(FREAD.RECORD$)-OFFSET%)
    ENDIF

END FUNCTION

\***********************************************************************!BSM
\*                                                                      !BSM
\* TRUNCATE.FIELD$                                                      !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
                                                                        !BSM
FUNCTION TRUNCATE.FIELD$(DM.FIELD$)                                     !BSM
    STRING TRUNCATE.FIELD$,DM.FIELD$                                    !BSM
    TRUNCATE.FIELD$ = DM.FIELD$                                         !BSM
    DM.OFFSET% = MATCH(" ",DM.FIELD$,1)                                 !BSM
    IF DM.OFFSET% > 0 THEN BEGIN                                        !BSM
        TRUNCATE.FIELD$ = LEFT$(DM.FIELD$,DM.OFFSET% - 1)               !BSM
    ENDIF                                                               !BSM
END FUNCTION                                                            !BSM
                                                                        !BSM
\***********************************************************************
\*
\* FIELD.SPLIT
\*
\**********************************************************************
FUNCTION FUNC.FIELD.SPLIT

    FIELD.OFFSET% = 1

    FOR LOOP% = 1 TO FIELD.NOS%

        IF (LEFT$(FIELD.TYPE.ARRAY$(LOOP%),1) = "P") OR (LEFT$(FIELD.TYPE.ARRAY$(LOOP%),1) = "C") THEN BEGIN
            FIELD.LEN$ = RIGHT$(FIELD.TYPE.ARRAY$(LOOP%), LEN(FIELD.TYPE.ARRAY$(LOOP%))-1)

            FIELD.VALUE$ = MID$(RECORD$,FIELD.OFFSET%, VAL(FIELD.LEN$))

            IF LEFT$(FIELD.TYPE.ARRAY$(LOOP%),1) = "P" THEN BEGIN
                FIELD.VALUE$ = UNPACK$(FIELD.VALUE$)
            ENDIF

            FIELD.OFFSET% = FIELD.OFFSET% + VAL(FIELD.LEN$)

        ENDIF ELSE IF FIELD.TYPE.ARRAY$(LOOP%) = "I1" THEN BEGIN

            FIELD.OFFSET% = FIELD.OFFSET% -1
            FIELD.VALUE$ = STR$(GETN1(RECORD$, FIELD.OFFSET%))
            FIELD.OFFSET% = FIELD.OFFSET% + 2

        ENDIF ELSE IF FIELD.TYPE.ARRAY$(LOOP%) = "I2" THEN BEGIN

            FIELD.OFFSET% = FIELD.OFFSET% -1
            FIELD.VALUE$ = STR$(GETN2 (RECORD$, FIELD.OFFSET%))
            FIELD.OFFSET% = FIELD.OFFSET% + 3

        ENDIF ELSE IF FIELD.TYPE.ARRAY$(LOOP%) = "I4" THEN BEGIN

            FIELD.OFFSET% = FIELD.OFFSET% -1
            FIELD.VALUE$ = STR$(GETN4 (RECORD$, FIELD.OFFSET%))
            FIELD.OFFSET% = FIELD.OFFSET% + 5
        ENDIF

        IF LOOP% = 1 THEN BEGIN
            FINAL.FIELD.VALUE$ = FIELD.VALUE$
        ENDIF ELSE BEGIN
            FINAL.FIELD.VALUE$ = FINAL.FIELD.VALUE$ + "," + FIELD.VALUE$
        ENDIF

    NEXT LOOP%

    FINAL.FIELD.VALUE$ = FINAL.FIELD.VALUE$ + CRLF$
    TEMP.FORM$ = "C" + STR$(LEN(FINAL.FIELD.VALUE$))
    WRITE FORM TEMP.FORM$ ;# OUTPUT.FILE.SESS.NUM%; FINAL.FIELD.VALUE$

END FUNCTION

\**********************************************************************
\*
\* PROCESS.KEYED.RECORD$
\*
\**********************************************************************

FUNCTION PROCESS.KEYED.RECORD$(RECORD1$) PUBLIC

    STRING PROCESS.KEYED.RECORD$, RECORD1$

    RECORD$ = RECORD1$
    CALL FUNC.FIELD.SPLIT
    PROCESS.KEYED.RECORD$ = RECORD1$

END FUNCTION

\**********************************************************************
\**********************************************************************
\*                                                                    *
\*          S T A R T   O F   M A I N L I N E   C O D E               *
\*                                                                    *
\**********************************************************************
\**********************************************************************

ON ERROR GOTO ERROR.DETECTED

    GOSUB INITIALISATION
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION


! Stopping Program
!------------------
STOP.PROGRAM:
STOP

\**********************************************************************
\*
\* INITIALISATION
\* This sub-program initialises program variables,
\* sets file open flags and allocate session numbers.
\*
\**********************************************************************
INITIALISATION:

    !INPUT.FILE.NAME$ = COMMAND$                                        !BSM
    MODULE.NUMBER$ = "FRE01"                   !Current module          !BSM

   !! Getting the file name from the user                               !BSM
   !!------------------------------------                               !BSM
   ! IF INPUT.FILE.NAME$ = "" THEN BEGIN                                !BSM
   !     GOSUB FREAD.HELP                                               !BSM
   ! ENDIF

    ! Initialising file variables
    !------------------------------------
    FALSE = 0
    TRUE  = -1
    CRLF$ = CHR$(13) + CHR$(10)

    ! Initialising Report number and name for FREAD.TXT and OUTPUT.CSV
    !---------------------------------------------------
    FREAD.FILE.NAME$        = "D:/ADX_UDT1/FREAD.TXT"
    FREAD.REPORT.NUM%       = 450           ! Temp Report Number

    OUTPUT.FILE.NAME$       = "D:/ADX_UDT1/OUTPUT.CSV"
    OUTPUT.FILE.REPORT.NUM% = 451           ! Temp Report Number

    ! Initialising File Open Flag and EOF Flag
    !-----------------------------------------
    EOF                     = FALSE
    FREAD.OPEN.FLAG         = FALSE
    OUTPUT.FILE.OPEN.FLAG   = FALSE

    FIELD.STRING$ = ""                                                  !BSM

    DIM FIELD.TYPE.ARRAY$ (1000)

    ! Initialising Session Number Allocate/De-allocate Flag
    !-------------------------------------------------------
    FREAD.SESS.NUM.FLAG     = FALSE
    OUTPUT.SESS.NUM.FLAG    = FALSE

    ! Allocating session number
    !--------------------------
    FUNCTION.FLAG$ = "O"                    ! Open
    CALL SESS.NUM.UTILITY            \
             (FUNCTION.FLAG$,        \
              FREAD.REPORT.NUM%,     \
              FREAD.FILE.NAME$)
    FREAD.SESS.NUM%         = F20.INTEGER.FILE.NO%
    FREAD.SESS.NUM.FLAG     = TRUE

    FUNCTION.FLAG$ = "O"                    ! Open
    CALL SESS.NUM.UTILITY            \
             (FUNCTION.FLAG$,        \
              OUTPUT.FILE.REPORT.NUM%,    \
              OUTPUT.FILE.NAME$)
    OUTPUT.FILE.SESS.NUM%   = F20.INTEGER.FILE.NO%
    OUTPUT.SESS.NUM.FLAG    = TRUE
    CALL DM.INIT                                                        !BSM

RETURN

\**********************************************************************
\*
\* FREAD.HELP
\* This sub-program displays the help screen for the program
\*
\**********************************************************************
!Commented out the code as it is no longer used                         !BSM
!FREAD.HELP:                                                            !BSM
!                                                                       !BSM
! PRINT                                                                 !BSM
! PRINT "**************************************************************"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "*            FREAD UTILITY v 0.1                             *"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "* This utility will help to read Sequential, Direct and      *"!BSM
! PRINT "* Keyed Files                                                *"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "* For reading a file, add the file and field details to the  *"!BSM
! PRINT "* input file FREAD.TXT in the format                         *"!BSM
! PRINT "* <filename>:<filetype>:<recl>:<no of fields>:               *"!BSM
! PRINT "* <fieldname>,<fieldtype>:........                           *"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "* FREAD will process the files based on the information      *"!BSM
! PRINT "* from FREAD.TXT and writes the records to OUTPUT.CSV file   *"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "* If the file and record details are not correct, errors     *"!BSM
! PRINT "* will be thrown which is not handled fully in this version  *"!BSM
! PRINT "*                                                            *"!BSM
! PRINT "**************************************************************"!BSM
! STOP                                                                  !BSM
!                                                                       !BSM
!RETURN                                                                 !BSM

\**********************************************************************
\*
\* MAIN.PROCESSING
\* This sub-program gets the details of the files from FREAD.TXT
\* and displays it on the screen
\*
\**********************************************************************
MAIN.PROCESSING:

    !! Open FREAD.TXT file                                              !BSM
    !!--------------------                                              !BSM
    !FILE.OPERATION$     = "O" ! Open                                   !BSM
    !CURRENT.REPORT.NUM% = FREAD.REPORT.NUM%                            !BSM
    !                                                                   !BSM
    !PRINT "Opening configuration file - FREAD"                         !BSM
    !IF END # FREAD.SESS.NUM% THEN FILE.ERROR                           !BSM
    !OPEN FREAD.FILE.NAME$ AS FREAD.SESS.NUM% NOWRITE NODEL             !BSM
    !FREAD.OPEN.FLAG = TRUE                                             !BSM
    !                                                                   !BSM
    !! Creating OUTPUT.CSV file                                         !BSM
    !!--------------------                                              !BSM
    !FILE.OPERATION$     = "C" ! Create                                 !BSM
    !CURRENT.REPORT.NUM% = OUTPUT.FILE.REPORT.NUM%                      !BSM
    !                                                                   !BSM
    !PRINT "Creating output file - OUTPUT.CSV"                          !BSM
    !IF END # OUTPUT.FILE.SESS.NUM% THEN FILE.ERROR                     !BSM
    !CREATE OUTPUT.FILE.NAME$ AS OUTPUT.FILE.SESS.NUM%                  !BSM
    !OUTPUT.FILE.OPEN.FLAG     = TRUE                                   !BSM
    !                                                                   !BSM
    !! Reading FREAD.TXT to find the record corresponding to            !BSM
    !! file name                                                        !BSM
    !!-------------------------------------------------------           !BSM
    !PRINT "Reading configuration file - FREAD"                         !BSM
    !                                                                   !BSM
    !WHILE EOF = FALSE                                                  !BSM
    !                                                                   !BSM
    !    FILE.OPERATION$     = "R" ! Open                               !BSM
    !    CURRENT.REPORT.NUM% = FREAD.REPORT.NUM%                        !BSM
    !                                                                   !BSM
    !    READ.FREAD.FILE.NAME      = FALSE                              !BSM
    !    IF END # FREAD.SESS.NUM% THEN READ.FREAD.FILE.NAME.ERR         !BSM
    !    READ #FREAD.SESS.NUM%; LINE FREAD.RECORD$                      !BSM
    !    READ.FREAD.FILE.NAME      = TRUE                               !BSM
    !                                                                   !BSM
    !    FILE.NAME$ = FUNC.PARSE.FIELD$                                 !BSM
    !    IF FILE.NAME$ = "0" THEN BEGIN                                 !BSM
    !        PRINT "Error in reading file name from FREAD.TXT"          !BSM
    !        PRINT "Checking next record from FREAD.TXT"                !BSM
    !    ENDIF                                                          !BSM
    !                                                                   !BSM
    !    IF FILE.NAME$ = INPUT.FILE.NAME$ THEN BEGIN                    !BSM
    !        GOSUB INIT.NEW.FILE                                        !BSM
    !        GOSUB PROCESS.FILE                                         !BSM
    !        FREAD.RECORD.PRESENT = TRUE                                !BSM
    !        EOF = TRUE                                                 !BSM
    !    ENDIF                                                          !BSM
    !                                                                   !BSM
!READ.FREAD.FILE.NAME.ERR:                                              !BSM
    !                                                                   !BSM
    !    IF READ.FREAD.FILE.NAME      = FALSE THEN BEGIN                !BSM
    !        EOF = TRUE                                                 !BSM
    !    ENDIF                                                          !BSM
    !                                                                   !BSM
    !WEND                                                               !BSM
    !                                                                   !BSM
    !IF FREAD.RECORD.PRESENT = FALSE THEN BEGIN                         !BSM
    !    PRINT "File details are not present in FREAD.TXT"              !BSM
    !ENDIF                                                              !BSM
    !PRINT "Opening configuration file - FREAD"                         !BSM

    IF NOT ?EXIST(FREAD.FILE.NAME$) THEN BEGIN                          !BSM
        FILE.OPERATION$     = "C"                 !Create               !BSM
        CURRENT.REPORT.NUM% = FREAD.REPORT.NUM%                         !BSM
        IF END # FREAD.SESS.NUM% THEN FILE.ERROR                        !BSM
        CREATE FREAD.FILE.NAME$ AS FREAD.SESS.NUM%                      !BSM
        CLOSE FREAD.SESS.NUM%                                           !BSM
    ENDIF                                                               !BSM
                                                                        !BSM
    GOSUB DISPLAY.MAIN.SCREEN                                           !BSM
                                                                        !BSM
RETURN

\**********************************************************************
\*
\* INIT.NEW.FILE
\*
\**********************************************************************
INIT.NEW.FILE:

    ! Initialising Report number for file to be read
    !---------------------------------------------------
    FILE.REPORT.NUM%   = 452           ! Temp Report Number

    ! Initialising File Open Flag
    !-----------------------------------------
    FILE.OPEN.FLAG     = FALSE

    ! Initialising Session Number Allocate/De-allocate Flag
    !-------------------------------------------------------
    FILE.SESS.NUM.FLAG = FALSE

    ! Allocating session number
    !--------------------------
    FUNCTION.FLAG$ = "O"                    ! Open
    CALL SESS.NUM.UTILITY        \
         (FUNCTION.FLAG$,        \
          FILE.REPORT.NUM%,      \
          FILE.NAME$)
    FILE.SESS.NUM%      = F20.INTEGER.FILE.NO%
    FILE.SESS.NUM.FLAG  = TRUE

RETURN

\**********************************************************************
\*
\* PROCESS.FILE
\*
\**********************************************************************
PROCESS.FILE:

    FILE.TYPE$ = FUNC.PARSE.FIELD$
    FILE.TYPE$ = UCASE$(FILE.TYPE$)                                     !BSM
    IF (FILE.TYPE$ <> "S" AND FILE.TYPE$ <> "K" AND FILE.TYPE$ <> "D")\
       THEN BEGIN
        !PRINT "Error in reading file type"                             !BSM
        WHILE RET.KEY% <> F3.KEY% AND RET.KEY% <>ESC.KEY%               !BSM
            CALL DM.HIDE.FN.KEY(2)                                      !BSM
            CALL DM.HIDE.FN.KEY(4)                                      !BSM
            CALL DM.HIDE.FN.KEY(5)                                      !BSM
            RET.KEY% = DM.INVISIBLE.INPUT("'"+ERROR$+"'")               !BSM
        WEND                                                            !BSM
        RETURN
    ENDIF

    IF FILE.TYPE$ = "S" THEN BEGIN
        GOSUB PROCESS.SEQUENTIAL.FILE
    ENDIF ELSE IF (FILE.TYPE$ = "K") OR (FILE.TYPE$ = "D") THEN BEGIN

        FILE.RECL$ = FUNC.PARSE.FIELD$
        IF FILE.RECL$ = "0" THEN BEGIN
           !PRINT "Error in reading record length"                      !BSM
            RETURN
        ENDIF

        FIELD.NOS$ = FUNC.PARSE.FIELD$
        IF FIELD.NOS$ = "0" THEN BEGIN
           !PRINT "Error in reading field numbers"                      !BSM
            RETURN
        ENDIF
        FIELD.NOS% = VAL(FIELD.NOS$)

        IF FILE.TYPE$ = "K" THEN BEGIN
            GOSUB FIELD.NAME.SPLIT
            !PRINT "Processing the keyed file " + FILE.NAME$
            CALL PROCESS.KEYED.FILE(FILE.NAME$, FILE.REPORT.NUM%, "N")
            WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% AND   \!BSM
                                             RET.KEY% <> ESC.KEY%       !BSM
                RET.KEY% = DM.INVISIBLE.INPUT("'READING COMPLETED." +  \!BSM
                                           "PRESS ENTER TO CONTINUE..") !BSM
            WEND                                                        !BSM
            CALL DM.STATUS("''")                                        !BSM

        ENDIF ELSE IF FILE.TYPE$ = "D" THEN BEGIN
            GOSUB FIELD.NAME.SPLIT
            GOSUB PROCESS.DIRECT.FILE
        ENDIF

    ENDIF

RETURN

\**********************************************************************
\*
\* PROCESS.SEQUENTIAL.FILE
\*
\**********************************************************************
PROCESS.SEQUENTIAL.FILE:

    !PRINT "Opening the sequential file " + FILE.NAME$                  !BSM

    FILE.OPERATION$         = "O" ! Open
    CURRENT.REPORT.NUM%     = FILE.REPORT.NUM%

    IF END # FILE.SESS.NUM% THEN FILE.ERROR
    OPEN FILE.NAME$ AS FILE.SESS.NUM%
    FILE.OPEN.FLAG          = TRUE

    !PRINT "Reading started.."                                          !BSM

    EOF = FALSE
    WHILE EOF = FALSE
        FILE.OPERATION$     = "R" ! Read
        CURRENT.REPORT.NUM% = FILE.REPORT.NUM%

        READ.FILE.NAME      = FALSE
        IF END # FILE.SESS.NUM% THEN READ.FILE.NAME.ERR
        READ # FILE.SESS.NUM%; RECORD$
        READ.FILE.NAME      = TRUE

READ.FILE.NAME.ERR:

        IF READ.FILE.NAME   = FALSE THEN BEGIN
            EOF = TRUE
           ! PRINT "Reading completed.. "                               !BSM
           WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% \       !BSM
                                        AND RET.KEY% <> ESC.KEY%        !BSM
                RET.KEY% = DM.INVISIBLE.INPUT("'READING COMPLETED." +  \!BSM
                                           "PRESS ENTER TO CONTINUE..") !BSM
           WEND                                                         !BSM
           CALL DM.STATUS("''")                                         !BSM
        ENDIF ELSE BEGIN
            RECORD$ = RECORD$ + CRLF$
            TEMP.FORM$ = "C" + STR$(LEN(RECORD$))
            WRITE FORM TEMP.FORM$ ;# OUTPUT.FILE.SESS.NUM%; RECORD$
        ENDIF
    WEND

RETURN

\**********************************************************************
\*
\* PROCESS.DIRECT.FILE
\*
\**********************************************************************
PROCESS.DIRECT.FILE:

   !PRINT "Opening the Direct file " + FILE.NAME$

    FILE.OPERATION$     = "O" ! Open
    CURRENT.REPORT.NUM% = FILE.REPORT.NUM%

    IF END # FILE.SESS.NUM% THEN FILE.ERROR
    OPEN FILE.NAME$ DIRECT RECL VAL(FILE.RECL$) AS FILE.SESS.NUM%

    RECORD.NOS% = SIZE(FILE.NAME$)/VAL(FILE.RECL$)
    FORM$ = "C" + FILE.RECL$

    !PRINT "Reading started.."

    FOR COUNT% = 1 TO RECORD.NOS%

        FILE.OPERATION$     = "R" ! Read
        CURRENT.REPORT.NUM% = FILE.REPORT.NUM%

        IF END # FILE.SESS.NUM% THEN FILE.ERROR
        READ FORM FORM$; #FILE.SESS.NUM%,COUNT%; RECORD$

        CALL FUNC.FIELD.SPLIT

    NEXT COUNT%

    !PRINT "Reading completed.. "
     WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY%  \            !BSM
                                  AND RET.KEY% <> ESC.KEY%              !BSM
         RET.KEY% = DM.INVISIBLE.INPUT("'READING COMPLETED."       +   \!BSM
                                         "PRESS ENTER TO CONTINUE..")   !BSM
     WEND                                                               !BSM
     CALL DM.STATUS("''")                                               !BSM

RETURN

\**********************************************************************
\*
\* FIELD.NAME.SPLIT
\*
\**********************************************************************
FIELD.NAME.SPLIT:

    FIELD.NAME$ = ""
    FIELD.TYPE$ = ""

    FOR LOOP% = 1 TO FIELD.NOS%
        FLD$ = FUNC.PARSE.FIELD$

        TEMP.OFFSET% = MATCH(",", FLD$, 1)
        FIELD.NAME$  = LEFT$(FLD$, (TEMP.OFFSET%-1))
        FIELD.TYPE$  = RIGHT$(FLD$, LEN(FLD$) -TEMP.OFFSET%)

        IF LOOP% = 1 THEN BEGIN
            FINAL.FIELD.NAMES$ = FIELD.NAME$
        ENDIF ELSE BEGIN
            FINAL.FIELD.NAMES$ = FINAL.FIELD.NAMES$ + "," + FIELD.NAME$
        ENDIF

        FIELD.TYPE.ARRAY$(LOOP%) = FIELD.TYPE$
    NEXT LOOP%

    FINAL.FIELD.NAMES$ = FINAL.FIELD.NAMES$ + CRLF$
    TEMP.FORM$ = "C" + STR$(LEN(FINAL.FIELD.NAMES$))
    WRITE FORM TEMP.FORM$ ;# OUTPUT.FILE.SESS.NUM%; FINAL.FIELD.NAMES$

RETURN

\**********************************************************************
\*
\* TERMINATION
\*
\**********************************************************************

TERMINATION:

   !PRINT "Closing files"                                               !BSM

    IF FILE.OPEN.FLAG  = TRUE THEN BEGIN
        CLOSE FILE.SESS.NUM%
    ENDIF

    IF FREAD.OPEN.FLAG = TRUE THEN BEGIN
        CLOSE FREAD.SESS.NUM%
    ENDIF

    IF OUTPUT.FILE.OPEN.FLAG = TRUE THEN BEGIN
        CLOSE OUTPUT.FILE.SESS.NUM%
    ENDIF

    IF FILE.SESS.NUM.FLAG  = TRUE THEN BEGIN
        FUNCTION.FLAG$ = "C"                        ! Close
        CALL SESS.NUM.UTILITY (FUNCTION.FLAG$,FILE.SESS.NUM%,"")
    ENDIF

    IF FREAD.SESS.NUM.FLAG  = TRUE THEN BEGIN
        FUNCTION.FLAG$ = "C"                        ! Close
        CALL SESS.NUM.UTILITY (FUNCTION.FLAG$,FREAD.SESS.NUM%,"")
    ENDIF

    IF OUTPUT.SESS.NUM.FLAG  = TRUE THEN BEGIN
        FUNCTION.FLAG$ = "C"                        ! Close
        CALL SESS.NUM.UTILITY (FUNCTION.FLAG$,OUTPUT.FILE.SESS.NUM%,"")
    ENDIF

   !PRINT "Program completed successfully"                              !BSM

RETURN

\**********************************************************************
\*
\* Sub-routine : FILE.ERROR:
\*
\**********************************************************************
FILE.ERROR:

    ERROR$ = ""

    IF FILE.OPERATION$ = "O"  THEN BEGIN
        ERROR$ = "Error in opening "
    ENDIF ELSE IF FILE.OPERATION$ = "C"  THEN BEGIN
        ERROR$ = "Error in creating "
    ENDIF ELSE IF FILE.OPERATION$ = "R"  THEN BEGIN
        ERROR$ = "Error in reading "
    ENDIF ELSE IF FILE.OPERATION$ = "W"  THEN BEGIN
        ERROR$ = "Error in writing "
    ENDIF

    IF CURRENT.REPORT.NUM% = FREAD.REPORT.NUM% THEN BEGIN
        ERROR$ = ERROR$ + FREAD.FILE.NAME$
    ENDIF ELSE IF OUTPUT.FILE.REPORT.NUM% THEN BEGIN
        ERROR$ = ERROR$ + OUTPUT.FILE.NAME$
    ENDIF ELSE IF CURRENT.REPORT.NUM% = FILE.REPORT.NUM% THEN BEGIN
        ERROR$ = ERROR$ + FILE.NAME$
    ENDIF

    IF OUTPUT.FILE.OPEN.FLAG THEN BEGIN
        DELETE OUTPUT.FILE.SESS.NUM%
        OUTPUT.FILE.OPEN.FLAG = FALSE
    ENDIF

    WHILE RET.KEY% <> F3.KEY% AND RET.KEY% <>ESC.KEY%
        CALL DM.HIDE.FN.KEY(2)
        CALL DM.HIDE.FN.KEY(4)
        CALL DM.HIDE.FN.KEY(5)
        RET.KEY% = DM.INVISIBLE.INPUT("'"+ERROR$+"'")
    WEND
   !GOSUB STOP.PROGRAM                                                  !BSM

RETURN

\***********************************************************************
\*READ.CONFIGURATION.FILE
\*
\***********************************************************************
READ.CONFIGURATION.FILE:

    ! Open FREAD.TXT file
    !--------------------
    IF FREAD.OPEN.FLAG THEN BEGIN                                       !BSM
        CLOSE FREAD.SESS.NUM%                                           !BSM
        FREAD.OPEN.FLAG = FALSE                                         !BSM
    ENDIF                                                               !BSM

    FILE.OPERATION$     = "O" ! Open
    CURRENT.REPORT.NUM% = FREAD.REPORT.NUM%

    !PRINT "Opening configuration file - FREAD"                         !BSM

    IF END # FREAD.SESS.NUM% THEN FILE.ERROR
    OPEN FREAD.FILE.NAME$ AS FREAD.SESS.NUM%
    FREAD.OPEN.FLAG = TRUE
    FREAD.RECORD.PRESENT = FALSE                                        !BSM
    EOF = FALSE                                                         !BSM

    WHILE EOF = FALSE

        FILE.OPERATION$     = "R" ! Open
        CURRENT.REPORT.NUM% = FREAD.REPORT.NUM%

        READ.FREAD.FILE.NAME      = FALSE
        IF END # FREAD.SESS.NUM% THEN READ.FREAD.FILE.NAME.ERR
        READ #FREAD.SESS.NUM%; LINE FREAD.RECORD$
        READ.FREAD.FILE.NAME      = TRUE

        FILE.NAME$ = FUNC.PARSE.FIELD$
        !IF FILE.NAME$ = "0" THEN BEGIN                                 !BSM
        !    PRINT "Error in reading file name from FREAD.TXT"          !BSM
        !    PRINT "Checking next record from FREAD.TXT"                !BSM
        !ENDIF                                                          !BSM

        IF FILE.NAME$ = UCASE$(DM.INPUT.FILE$) THEN BEGIN               !BSM
            FREAD.RECORD.PRESENT = TRUE
            EOF = TRUE
        ENDIF                                                           !BSM

READ.FREAD.FILE.NAME.ERR:

        IF READ.FREAD.FILE.NAME = FALSE THEN BEGIN
            EOF = TRUE
        ENDIF

    WEND

RETURN

\***********************************************************************!BSM
\* DISPLAY.MAIN.SCREEN:                                                 !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
DISPLAY.MAIN.SCREEN:                                                    !BSM
                                                                        !BSM
    CALL DM.SHOW.SCREEN(1, "", 1, 1)                                    !BSM
                                                                        !BSM
    CALL DM.NAME( 2, "INPUT.FILE$"   , DM.INPUT.FILE$)                  !BSM
    CALL DM.NAME( 4, "FILES.TYPE$"    ,DM.FILE.TYPE$)                   !BSM
    CALL DM.NAME( 7, "RECORD.LENGTH$", DM.RECORD.LENGTH$)               !BSM
    CALL DM.NAME( 9, "NO.OF.FIELDS$" , DM.NO.OF.FIELDS$)                !BSM
    CALL DM.NAME(11, "FIELD.NAME$"   , DM.FIELD.NAME$)                  !BSM
    CALL DM.NAME(13, "FIELD.TYPE$"   , DM.FIELD.TYPE$)                  !BSM
                                                                        !BSM
    WHILE TRUE                                                          !BSM
                                                                        !BSM
        FIELD.STRING$ = ""                                              !BSM
        CALL DM.VISIBLE("2","TRUE")                                     !BSM
        CALL DM.SHOW.FN.KEY(3,"")                                       !BSM
        RET.KEY% = DM.PROCESS.SCREEN(2,14,FALSE)                        !BSM
        IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN         !BSM
            GOSUB ESCAPE.PRESSED                                        !BSM
        ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN                  !BSM
            DM.INPUT.FILE$ = TRUNCATE.FIELD$(DM.INPUT.FILE$)            !BSM
            FILE.LENGTH% = LEN(DM.INPUT.FILE$)                          !BSM
            SLASH.POSITION% = MATCH("\",DM.INPUT.FILE$,1)               !BSM
            IF SLASH.POSITION% = 0 THEN BEGIN                           !BSM
                SLASH.POSITION% = MATCH("/",DM.INPUT.FILE$,1)           !BSM
            ENDIF                                                       !BSM
                                                                        !BSM
            WHILE FILE.LENGTH% = 0 OR SLASH.POSITION% = 0               !BSM
                IF FILE.LENGTH% = 0 THEN BEGIN                          !BSM
                    CALL DM.FOCUS("", "MESSAGE (221, 'ENTER A VALID FILE NAME')")!BSM
                ENDIF ELSE IF SLASH.POSITION% = 0 THEN BEGIN            !BSM
                    CALL DM.FOCUS("", "MESSAGE (221, 'ENTER FULLY QUALIFIED FILE NAME')")!BSM
                    DM.INPUT.FILE$ = STRING$(33," ")                    !BSM
                    INPUT.FILE$ = DM.INPUT.FILE$                        !BSM
                ENDIF                                                   !BSM
                RET.KEY% = DM.PROCESS.SCREEN(2,2,FALSE)                 !BSM
                IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN !BSM
                    GOSUB ESCAPE.PRESSED                                !BSM
                ENDIF                                                   !BSM
                DM.INPUT.FILE$ = TRUNCATE.FIELD$(DM.INPUT.FILE$)        !BSM
                FILE.LENGTH% = LEN(DM.INPUT.FILE$)                      !BSM
                IF DM.INPUT.FILE$ = "" THEN BEGIN                       !BSM
                    FILE.LENGTH% = 0                                    !BSM
                ENDIF                                                   !BSM
                SLASH.POSITION% = MATCH("\",DM.INPUT.FILE$,1)           !BSM
                IF SLASH.POSITION% = 0 THEN BEGIN                       !BSM
                SLASH.POSITION% = MATCH("/",DM.INPUT.FILE$,1)           !BSM
                ENDIF                                                   !BSM
            WEND                                                        !BSM
                                                                        !BSM
            FILE.EXIST.FLAG = ?EXIST(DM.INPUT.FILE$)                    !BSM
                                                                        !BSM
            IF FILE.EXIST.FLAG = FALSE THEN BEGIN                       !BSM
                CALL DM.SHOW.FN.KEY(2,"")                               !BSM
                RET.KEY% = -1                                           !BSM
                WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F2.KEY%   \!BSM
                      AND RET.KEY% <> F3.KEY% AND RET.KEY% <> ESC.KEY%  !BSM
                    RET.KEY% = DM.INVISIBLE.INPUT("'FILE DOES NOT EXIST!'")!BSM
                WEND                                                    !BSM
                CALL DM.STATUS("''")                                    !BSM
            ENDIF ELSE BEGIN                                            !BSM
                GOSUB READ.CONFIGURATION.FILE                           !BSM
                CALL DM.SHOW.FN.KEY(2,"")                               !BSM
                IF FREAD.RECORD.PRESENT THEN BEGIN                      !BSM
                    CALL DM.SHOW.FN.KEY(5,"")                           !BSM
                    RET.KEY% = DM.INVISIBLE.INPUT("'PRESS F5 TO READ FILE'")!BSM
                                                                        !BSM
                    IF RET.KEY% = F5.KEY% THEN BEGIN                    !BSM
                        GOSUB CREATE.OUTPUT.FILE                        !BSM
                        GOSUB INIT.NEW.FILE                             !BSM
                        GOSUB PROCESS.FILE                              !BSM
                   ENDIF ELSE IF RET.KEY% = ESC.KEY% OR                \!BSM
                                          RET.KEY% = F3.KEY%  THEN BEGIN!BSM
                       GOSUB ESCAPE.PRESSED                             !BSM
                   ENDIF ELSE BEGIN                                     !BSM
                       WHILE RET.KEY% <> F2.KEY% AND RET.KEY% = ESC.KEY%\!BSM
                                                  AND RET.KEY% = F3.KEY%!BSM
                           RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")!BSM
                           IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY%\!BSM
                                                              THEN BEGIN!BSM
                               GOSUB ESCAPE.PRESSED                     !BSM
                           ENDIF                                        !BSM
                        WEND                                            !BSM
                        CALL DM.STATUS("''")                            !BSM
                   ENDIF                                                !BSM
               ENDIF ELSE BEGIN                                         !BSM
                   FIELD.STRING$ = DM.INPUT.FILE$ + ";"                 !BSM
                   CALL DM.VISIBLE("3","TRUE")                          !BSM
                   CALL DM.VISIBLE("4","TRUE")                          !BSM
                   CALL DM.VISIBLE("5","TRUE")                          !BSM
                   RET.KEY% = DM.PROCESS.SCREEN (3,5,FALSE)             !BSM
                   IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY%        \!BSM
                                                             THEN BEGIN !BSM
                       GOSUB ESCAPE.PRESSED                             !BSM
                   ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN       !BSM
                       FILE.TYPE$ = DM.FILE.TYPE$                       !BSM
                       INVALID.TYPE = TRUE                              !BSM
                       WHILE INVALID.TYPE                               !BSM
                           IF DM.FILE.TYPE$ <> "D" AND                 \!BSM
                              DM.FILE.TYPE$ <> "K" AND                 \!BSM
                              DM.FILE.TYPE$ <> "S" THEN BEGIN           !BSM
                              CALL DM.FOCUS("", "MESSAGE (221, 'YOU MUST ONLY TYPE S,D OR K')")!BSM
                              DM.FILE.TYPE$ = ""                        !BSM
                              FILES.TYPE$ = DM.FILE.TYPE$               !BSM
                              RET.KEY% = DM.PROCESS.SCREEN(3,5,FALSE)   !BSM
                              IF RET.KEY% = ESC.KEY% OR                \!BSM
                                           RET.KEY% = F3.KEY% THEN BEGIN!BSM
                                  GOSUB ESCAPE.PRESSED                  !BSM
                              ENDIF                                     !BSM
                           ENDIF ELSE BEGIN                             !BSM
                             INVALID.TYPE = FALSE                       !BSM
                           ENDIF                                        !BSM
                       WEND                                             !BSM
                                                                        !BSM
                       IF DM.FILE.TYPE$ = "D" OR DM.FILE.TYPE$ = "K"   \!BSM
                                                              THEN BEGIN!BSM
                           FIELD.STRING$ = FIELD.STRING$ +             \!BSM
                                                    DM.FILE.TYPE$ + ";" !BSM
                           CALL DM.VISIBLE("6","TRUE")                  !BSM
                           CALL DM.VISIBLE("7","TRUE")                  !BSM
                           CALL DM.VISIBLE("8","TRUE")                  !BSM
                           CALL DM.VISIBLE("9","TRUE")                  !BSM
                           CALL DM.VISIBLE("10","TRUE")                 !BSM
                           CALL DM.VISIBLE("11","TRUE")                 !BSM
                           CALL DM.VISIBLE("12","TRUE")                 !BSM
                           CALL DM.VISIBLE("13","TRUE")                 !BSM
                           CALL DM.VISIBLE("14","TRUE")                 !BSM
                           CALL DM.HIDE.FN.KEY(2)                       !BSM
                                                                        !BSM
                           START.FIELD% = 6                             !BSM
                           WHILE START.FIELD% < 14                      !BSM
                                                                        !BSM
                               RET.KEY% = DM.PROCESS.SCREEN(START.FIELD%,14,FALSE)     !BSM
                                                                                       !BSM
                               IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN !BSM
                                   GOSUB ESCAPE.PRESSED                                !BSM
                               ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN          !BSM
                                   IF START.FIELD% = 6 OR START.FIELD% = 8 THEN BEGIN  !BSM
                                       GOSUB VALIDATE.FIELDS                           !BSM
                                   ENDIF                                               !BSM
                                   IF START.FIELD% = 12 THEN BEGIN                     !BSM
                                       FIELD.STRING$ = FIELD.STRING$                \  !BSM
                                           + TRUNCATE.FIELD$(DM.RECORD.LENGTH$)+ ";"\  !BSM
                                           + TRUNCATE.FIELD$(DM.NO.OF.FIELDS$) + ";"   !BSM
                                       FIELD.COUNT% = VAL(DM.NO.OF.FIELDS$)            !BSM
                                       COUNTER% = 1                                    !BSM
                                       WHILE COUNTER% < FIELD.COUNT%                   !BSM
                                           COUNTER% = COUNTER% + 1                     !BSM
                                           FIELD.STRING$ = FIELD.STRING$              \!BSM
                                               + TRUNCATE.FIELD$(DM.FIELD.NAME$) + ","\!BSM
                                               + TRUNCATE.FIELD$(DM.FIELD.TYPE$) + ";" !BSM
                                           CALL DM.FOCUS("", "MESSAGE (221, 'ENTER NEXT FIELD')" )               !BSM
                                           DM.FIELD.NAME$ = STRING$(24," ")                                      !BSM
                                           FIELD.NAME$ = DM.FIELD.NAME$                                          !BSM
                                           DM.FIELD.TYPE$ = STRING$(7," ")                                       !BSM
                                           FIELD.TYPE$ = DM.FIELD.TYPE$                                          !BSM
                                           RET.KEY% = DM.PROCESS.SCREEN(10,11,FALSE)                             !BSM
                                           IF RET.KEY% = ENTER.KEY% THEN BEGIN                                   !BSM
                                                                                                                 !BSM
                                               RET.KEY% = DM.PROCESS.SCREEN(12,14,FALSE)                         !BSM
                                               IF RET.KEY% = ESC.KEY% OR                \                        !BSM
                                                         RET.KEY% = F3.KEY% THEN BEGIN                           !BSM
                                                   GOSUB ESCAPE.PRESSED                                          !BSM
                                               ENDIF ELSE BEGIN                                                  !BSM
                                                   WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% AND  \   !BSM
                                                                                      RET.KEY% <> ESC.KEY%       !BSM
                                                       RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")    !BSM
                                                       IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN   !BSM
                                                           GOSUB ESCAPE.PRESSED                                  !BSM
                                                       ENDIF                                                     !BSM
                                                   WEND                                                          !BSM
                                                   CALL DM.STATUS("''")                                          !BSM
                                               ENDIF                                                             !BSM
                                           ENDIF ELSE IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN    !BSM
                                                GOSUB ESCAPE.PRESSED                                             !BSM
                                           ENDIF ELSE BEGIN                                                      !BSM
                                               WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% AND      \   !BSM
                                                                                          RET.KEY% <> ESC.KEY%   !BSM
                                                  RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")         !BSM
                                                  IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN        !BSM
                                                      GOSUB ESCAPE.PRESSED                                       !BSM
                                                  ENDIF                                                          !BSM
                                               WEND                                                              !BSM
                                               CALL DM.STATUS("''")                                              !BSM
                                          ENDIF                                                                  !BSM
                                                                                                                 !BSM
                                       WEND                                                                      !BSM
                                       FIELD.STRING$ = FIELD.STRING$ + \                                         !BSM
                                                       TRUNCATE.FIELD$(DM.FIELD.NAME$) + "," + \                 !BSM
                                                       TRUNCATE.FIELD$(DM.FIELD.TYPE$) + ";"                     !BSM
                                   ENDIF                                                                         !BSM
                                   START.FIELD% = START.FIELD% + 2                                               !BSM
                                ENDIF ELSE BEGIN                                                                 !BSM
                                     WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% AND             \      !BSM
                                                                                       RET.KEY% <> ESC.KEY%      !BSM
                                        RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")                   !BSM
                                        IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN                  !BSM
                                            GOSUB ESCAPE.PRESSED                                                 !BSM
                                        ENDIF                                                                    !BSM
                                     WEND                                                                        !BSM
                                     CALL DM.STATUS("''")                                                        !BSM
                               ENDIF                                                                             !BSM
                           WEND                                                                                  !BSM
                           CALL DM.SHOW.FN.KEY(2,"")                                                             !BSM
                           CALL DM.SHOW.FN.KEY(3,"")                                                             !BSM
                           CALL DM.SHOW.FN.KEY(4,"")                                                             !BSM
                           CALL DM.SHOW.FN.KEY(5,"")                                                             !BSM
                           RET.KEY% = DM.INVISIBLE.INPUT("'PRESS F4 TO SAVE OR F5 TO READ FILE'")                !BSM
                           IF RET.KEY% = F4.KEY% OR RET.KEY% = F5.KEY% THEN BEGIN                                !BSM
                               FIELD.STRING$ = FIELD.STRING$ + CRLF$                                             !BSM
                               GOSUB SAVE.FILE.DETAILS                                                           !BSM
                               FREAD.RECORD$ = FIELD.STRING$                                                     !BSM
                               IF RET.KEY% = F5.KEY% THEN BEGIN                                                  !BSM
                                   GOSUB CREATE.OUTPUT.FILE                                                      !BSM
                                   FILE.NAME$ = FUNC.PARSE.FIELD$                                                !BSM
                                   GOSUB INIT.NEW.FILE                                                           !BSM
                                   GOSUB PROCESS.FILE                                                            !BSM
                               ENDIF ELSE BEGIN                                                                  !BSM
                                   WHILE RET.KEY% <> ENTER.KEY% AND   \                                          !BSM
                                         RET.KEY% <> F3.KEY%    AND   \                                          !BSM
                                         RET.KEY% <> ESC.KEY%                                                    !BSM
                                       RET.KEY% = DM.INVISIBLE.INPUT("'FILE DETAILS SAVED.PRESS ENTER TO CONTINUE..'")!BSM
                                   WEND                                                                          !BSM
                                   CALL DM.STATUS("''")                                                          !BSM
                               ENDIF                                                                             !BSM
                           ENDIF ELSE IF RET.KEY% = F2.KEY% THEN BEGIN                                           !BSM
                               FIELD.STRING$ = ""                                                                !BSM
                               CALL DM.STATUS("''")                                                              !BSM
                           ENDIF ELSE IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN                    !BSM
                               GOSUB ESCAPE.PRESSED                                                              !BSM
                           ENDIF ELSE BEGIN                                                                      !BSM
                                WHILE RET.KEY% <> F2.KEY% AND RET.KEY% <> F3.KEY% AND RET.KEY% <> ESC.KEY%       !BSM
                                    RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED.'")                      !BSM
                                WEND                                                                             !BSM
                                IF RET.KEY% = F2.KEY% THEN BEGIN                                                 !BSM
                                    FIELD.STRING$ = ""                                                           !BSM
                                ENDIF                                                                            !BSM
                                CALL DM.STATUS("''")                                                             !BSM
                           ENDIF                                                                                 !BSM
                                                                                                                 !BSM
                       ENDIF ELSE IF DM.FILE.TYPE$ = "S" THEN BEGIN                                              !BSM
                           CALL DM.SHOW.FN.KEY(2,"")                                                             !BSM
                           CALL DM.SHOW.FN.KEY(5,"")                                                             !BSM
                           RET.KEY% = DM.INVISIBLE.INPUT("'PRESS F4 TO SAVE OR F5 TO READ FILE'")                !BSM
                           IF RET.KEY% = F4.KEY% OR RET.KEY% = F5.KEY% THEN BEGIN                                !BSM
                               FIELD.STRING$ = FIELD.STRING$ + DM.FILE.TYPE$ + ";"                               !BSM
                               FIELD.STRING$ = FIELD.STRING$ + CRLF$                                             !BSM
                               GOSUB SAVE.FILE.DETAILS                                                           !BSM
                               FREAD.RECORD$ = FIELD.STRING$                                                     !BSM
                               IF RET.KEY% = F5.KEY% THEN BEGIN                                                  !BSM
                                   GOSUB CREATE.OUTPUT.FILE                                                      !BSM
                                   FILE.NAME$ = FUNC.PARSE.FIELD$                                                !BSM
                                   GOSUB INIT.NEW.FILE                                                           !BSM
                                   GOSUB PROCESS.FILE                                                            !BSM
                                ENDIF ELSE BEGIN                                                                 !BSM
                                    WHILE RET.KEY% <> ENTER.KEY% AND RET.KEY% <> F3.KEY% AND \                   !BSM
                                                                                        RET.KEY% <> ESC.KEY%     !BSM
                                        RET.KEY% = DM.INVISIBLE.INPUT("'FILE DETAILS SAVED.PRESS ENTER TO CONTINUE..'")!BSM
                                    WEND                                                                         !BSM
                                    CALL DM.STATUS("''")                                                         !BSM
                               ENDIF                                                                             !BSM
                           ENDIF ELSE IF RET.KEY% = F2.KEY% THEN BEGIN                                           !BSM
                               FIELD.STRING$ = ""                                                                !BSM
                               CALL DM.STATUS("''")                                                              !BSM
                           ENDIF ELSE IF RET.KEY% = ESC.KEY% AND RET.KEY% = F3.KEY% THEN BEGIN                   !BSM
                               GOSUB ESCAPE.PRESSED                                                              !BSM
                           ENDIF ELSE BEGIN                                                                      !BSM
                                WHILE RET.KEY% <> F2.KEY% AND RET.KEY% <> F3.KEY% AND RET.KEY% <> ESC.KEY%       !BSM
                                    RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED.'")                      !BSM
                                WEND                                                                             !BSM
                                IF RET.KEY% = F2.KEY% THEN BEGIN                                                 !BSM
                                   FIELD.STRING$ = ""                                                            !BSM
                                ENDIF                                                                            !BSM
                                CALL DM.STATUS("''")                                                             !BSM
                           ENDIF                                                                                 !BSM
                       ENDIF                                                                                     !BSM
                   ENDIF ELSE BEGIN                                                                              !BSM
                        WHILE RET.KEY% <> ESC.KEY% AND RET.KEY% <> F3.KEY% AND RET.KEY% <> F2.KEY%               !BSM
                            RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")                               !BSM
                        WEND                                                                                     !BSM
                        CALL DM.STATUS("''")                                                                     !BSM
                   ENDIF                                                                                         !BSM
                ENDIF                                                                                            !BSM
            ENDIF                                                                                                !BSM
        ENDIF ELSE BEGIN                                                                                         !BSM
            CALL DM.SHOW.FN.KEY(2,"")                                                                            !BSM
            WHILE RET.KEY% <> F2.KEY% AND RET.KEY% <> F3.KEY% AND RET.KEY% <> F2.KEY%                            !BSM
                RET.KEY% = DM.INVISIBLE.INPUT("'INVALID KEY PRESSED'")                                           !BSM
            WEND                                                                                                 !BSM
            CALL DM.STATUS("''")                                                                                 !BSM
        ENDIF                                                                                                    !BSM
        IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN                                                  !BSM
            GOSUB ESCAPE.PRESSED                                                                                 !BSM
        ENDIF ELSE BEGIN                                                                                         !BSM
            GOSUB RESET.SCREEN                                                                                   !BSM
        ENDIF                                                                                                    !BSM
    WEND                                                                                                         !BSM
    GOSUB ESCAPE.PRESSED                                                                                         !BSM
                                                                                                                 !BSM
RETURN                                                                                                           !BSM

\***********************************************************************!BSM
\*                                                                      !BSM
\* CREATE.OUTPUT.FILE                                                   !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
CREATE.OUTPUT.FILE:                                                     !BSM
     FILE.OPERATION$     = "C" ! Create                                 !BSM
     CURRENT.REPORT.NUM% = OUTPUT.FILE.REPORT.NUM%                      !BSM
     IF OUTPUT.FILE.OPEN.FLAG = TRUE THEN BEGIN                         !BSM
         DELETE OUTPUT.FILE.SESS.NUM%                                   !BSM
         OUTPUT.FILE.OPEN.FLAG = FALSE                                  !BSM
     ENDIF                                                              !BSM
     IF END # OUTPUT.FILE.SESS.NUM% THEN FILE.ERROR                     !BSM
     CREATE OUTPUT.FILE.NAME$ AS OUTPUT.FILE.SESS.NUM%                  !BSM
     OUTPUT.FILE.OPEN.FLAG     = TRUE                                   !BSM
RETURN                                                                  !BSM

\***********************************************************************!BSM
\*                                                                      !BSM
\* SAVE.FILE.DETAILS                                                    !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
SAVE.FILE.DETAILS:                                                      !BSM
    IF FREAD.OPEN.FLAG THEN BEGIN                                       !BSM
        CLOSE FREAD.SESS.NUM%                                           !BSM
        FREAD.OPEN.FLAG = FALSE                                         !BSM
    ENDIF                                                               !BSM
    IF END # FREAD.SESS.NUM% THEN FILE.ERROR                            !BSM
    OPEN FREAD.FILE.NAME$ AS FREAD.SESS.NUM% APPEND                     !BSM
    FREAD.OPEN.FLAG = TRUE                                              !BSM
    WRITE.FORM$ = "C"+STR$(LEN(FIELD.STRING$))                          !BSM
    IF END # FREAD.SESS.NUM% THEN FILE.ERROR                            !BSM
    WRITE FORM WRITE.FORM$ ;#FREAD.SESS.NUM%; FIELD.STRING$             !BSM
                                                                        !BSM
RETURN                                                                  !BSM

\***********************************************************************!BSM
\*                                                                      !BSM
\* VALIDATE.FIELDS                                                      !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
VALIDATE.FIELDS:                                                        !BSM
   INVALID = TRUE                                                       !BSM
    WHILE INVALID                                                       !BSM
        IF START.FIELD% = 6 THEN BEGIN                                  !BSM
           DM.RECORD.LENGTH$ = TRUNCATE.FIELD$(DM.RECORD.LENGTH$)       !BSM
           IF VAL(DM.RECORD.LENGTH$) = 0 THEN BEGIN                     !BSM
               CALL DM.FOCUS("", "MESSAGE (221, 'RECORD LENGTH CAN NOT BE ZERO')")!BSM
               DM.RECORD.LENGTH$ = STRING$(3," ")                       !BSM
               RECORD.LENGTH$ = DM.RECORD.LENGTH$                       !BSM
               RET.KEY% = DM.PROCESS.SCREEN(8,8,FALSE)                  !BSM
               IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN  !BSM
                   GOSUB ESCAPE.PRESSED                                 !BSM
               ENDIF                                                    !BSM
           ENDIF ELSE BEGIN                                             !BSM
              INVALID = FALSE                                           !BSM
           ENDIF                                                        !BSM
        ENDIF ELSE IF START.FIELD% = 8 THEN BEGIN                       !BSM
            DM.NO.OF.FIELDS$ = TRUNCATE.FIELD$(DM.NO.OF.FIELDS$)        !BSM
            IF VAL(DM.NO.OF.FIELDS$) = 0 THEN BEGIN                     !BSM
               CALL DM.FOCUS("", "MESSAGE (221, 'THIS FIELD CAN NOT BE ZERO')")!BSM
               DM.NO.OF.FIELDS$ = STRING$(3," ")                        !BSM
               NO.OF.FIELDS$ = DM.NO.OF.FIELDS$                         !BSM
               RET.KEY% = DM.PROCESS.SCREEN(8,8,FALSE)                  !BSM
               IF RET.KEY% = ESC.KEY% OR RET.KEY% = F3.KEY% THEN BEGIN  !BSM
                   GOSUB ESCAPE.PRESSED                                 !BSM
               ENDIF                                                    !BSM
           ENDIF ELSE BEGIN                                             !BSM
              INVALID = FALSE                                           !BSM
           ENDIF                                                        !BSM
        ENDIF                                                           !BSM
    WEND                                                                !BSM
                                                                        !BSM
RETURN                                                                  !BSM

\***********************************************************************!BSM
\*                                                                      !BSM
\* RESET.SCREEN                                                         !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
RESET.SCREEN:                                                           !BSM
                                                                        !BSM
    IF RET.KEY% = F3.KEY% THEN BEGIN                                    !BSM
        GOSUB ESCAPE.PRESSED                                            !BSM
    ENDIF                                                               !BSM
    CALL DM.HIDE.FN.KEY(2)                                              !BSM
    CALL DM.HIDE.FN.KEY(4)                                              !BSM
    CALL DM.HIDE.FN.KEY(5)                                              !BSM
    DM.INPUT.FILE$ = STRING$(33," ")                                    !BSM
    INPUT.FILE$ = DM.INPUT.FILE$                                        !BSM
    DM.FILE.TYPE$ = " "                                                 !BSM
    FILES.TYPE$ = DM.FILE.TYPE$                                         !BSM
    DM.RECORD.LENGTH$ = STRING$(3," ")                                  !BSM
    RECORD.LENGTH$ = DM.RECORD.LENGTH$                                  !BSM
    DM.NO.OF.FIELDS$ = STRING$(3," ")                                   !BSM
    NO.OF.FIELDS$ = DM.NO.OF.FIELDS$                                    !BSM
    DM.FIELD.NAME$ = STRING$(22," ")                                    !BSM
    FIELD.NAME$ = DM.FIELD.NAME$                                        !BSM
    DM.FIELD.TYPE$ = STRING$(5," ")                                     !BSM
    FIELD.TYPE$ = DM.FIELD.TYPE$                                        !BSM
    CALL DM.VISIBLE("2","FALSE")                                        !BSM
    CALL DM.VISIBLE("3","FALSE")                                        !BSM
    CALL DM.VISIBLE("4","FALSE")                                        !BSM
    CALL DM.VISIBLE("5","FALSE")                                        !BSM
    CALL DM.VISIBLE("6","FALSE")                                        !BSM
    CALL DM.VISIBLE("7","FALSE")                                        !BSM
    CALL DM.VISIBLE("8","FALSE")                                        !BSM
    CALL DM.VISIBLE("9","FALSE")                                        !BSM
    CALL DM.VISIBLE("10","FALSE")                                       !BSM
    CALL DM.VISIBLE("11","FALSE")                                       !BSM
    CALL DM.VISIBLE("12","FALSE")                                       !BSM
    CALL DM.VISIBLE("13","FALSE")                                       !BSM
    CALL DM.VISIBLE("14","FALSE")                                       !BSM
                                                                        !BSM
RETURN                                                                  !BSM

\***********************************************************************!BSM
\*                                                                      !BSM
\* ESCAPE.PRESSED                                                       !BSM
\*                                                                      !BSM
\***********************************************************************!BSM
ESCAPE.PRESSED:                                                         !BSM
        CLEARS                                                          !BSM
        GOSUB TERMINATION                                               !BSM
        GOSUB STOP.PROGRAM                                              !BSM
RETURN                                                                  !BSM

\**********************************************************************
\*
\* Main error handling routine
\*
\**********************************************************************
ERROR.DETECTED:

   !PRINT "ERR      = "; ERR                                            !BSM
   !PRINT "ERRN     = "; ERRN                                           !BSM
   !PRINT "ERRF%    = "; ERRF%                                          !BSM
   !PRINT "ERRL     = "; ERRL                                           !BSM
    CLEARS                                                              !BSM
    GOSUB STOP.PROGRAM                                                  !BSM

END

