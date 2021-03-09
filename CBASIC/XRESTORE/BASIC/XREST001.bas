\***********************************************************************
\*
\*            PROGRAM         :       XRESTORE
\*            MODULE          :       XREST001
\*            AUTHOR          :       Jaya Kumar Inbaraj
\*            DATE WRITTEN    :       Mar 2014
\*
\***********************************************************************

\***********************************************************************
\*
\* Program: XRESTORE          Jaya Kumar Inbaraj              04/03/2014
\*
\* FOD260 - Enhanced Backup and Recovery
\*
\*       MODULE EXTRACTED FROM ORIGINAL XRESTORE.BAS (RENAMED TO
\*       XREST000.BAS) AS BREACHED 64K LIMIT AND CRETAED AS XREST001.BAS
\*
\*
\*======================================================================
\*                   V E R S I O N   C O N T R O L
\*======================================================================
\*
\*                            Dave Constable                  25/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* Code extracted originally from single module (XRESTORE.BAS) and all
\* code change markers removed.
\* For maintenance and usage all included variables and includes are
\* also replicated here.
\*
\* Version B                 Jaya Kumar Inbaraj               01/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC717 and QC724 - Restoration for files in BKPFAIL has been fixed.
\*
\* Version C                 Jaya Kumar Inbaraj               23/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* CR5 changes to perform the drive restore if a drive is empty.
\* Also commented out few redundant labels and worked on Internal
\* and APPS management review comments.
\* Added an Header for easy reference on multi-modular program.
\*
\* Version D                 Jaya Kumar Inbaraj               03/09/2014
\* FOD260 - Enhanced Backup and Recovery
\* Worked on APPS management review comments.
\*
\* Version E                 Jaya Kumar Inbaraj               12/09/2014
\* QC1145 - Updated to avoid excessive logging.
\*
\***********************************************************************

\***********************************************************************
\*
\* Included global variables
\*
\***********************************************************************
    %INCLUDE XRESTORG.J86                                               !CJK

\***********************************************************************
\*
\* Included external functions
\*
\***********************************************************************
    %INCLUDE XRESTORE.J86                                               !CJK

\***********************************************************************
\*
\*    FUNC.DIR.NOT.EXISTS: This function checks the existence of given
\*                         directory by using CHDIR command.
\*
\***********************************************************************
FUNCTION FUNC.DIR.NOT.EXISTS(DIRECTORY.NAME$) EXTERNAL
    INTEGER*1 FUNC.DIR.NOT.EXISTS
    STRING    DIRECTORY.NAME$
END FUNCTION

\***********************************************************************
\*
\*    FUNC.FILE.EXISTS: This function checks the existence of
\*                      passed file by using SIZE function.
\*
\***********************************************************************
FUNCTION FUNC.FILE.EXISTS(FILE.NAME$) EXTERNAL
    STRING    FILE.NAME$
    INTEGER*1 FUNC.FILE.EXISTS
END FUNCTION

\***********************************************************************
\*
\* PROCESS.DAY.DIR.SCREEN: This Sub-Program does all the necessary
\*                         actions needed for directory restore
\*                         processing including screen navigation.
\*
\*                         It receives the directory values as an input.
\*
\* As a program modularisation, Directory restore has been made as a
\* Sub-program to perform all the Directory restore functionalities
\* within itself. This enables quick extraction from the main module
\* should breach the 64k limit
\*
\***********************************************************************
SUB SUB.PROCESS.DAY.DIR.SCREEN(IMG.FILE$, ALT.FILE$) PUBLIC

    STRING              \
        ALT.FILE$,      \
        IMG.FILE$

    GOSUB SHOW.DIRECTORIES

    ! Directory restore day selection screen and status screen
    WHILE SCREEN% = DIRECTORY.DAY.SELECT.SCR% OR \
          SCREEN% = DIRECTORY.PROCESS.SCR%

        ! If day selection screen
        IF SCREEN% = DIRECTORY.DAY.SELECT.SCR% THEN BEGIN

            ! If the screen is accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                GOSUB SHOW.DIRECTORIES
                PREVIOUS.KEY = FALSE
            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (2, 105, TRUE)

            IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN

                SCREEN%      = RESTORE.A.DIRECTORY.SCR%
                PREVIOUS.KEY = TRUE

            ENDIF ELSE BEGIN
                IF VALUE.INDEX% = XRE.ZERO% THEN BEGIN                  !CJK
                    CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(21) + \      !CJK
                                        STATUS.TEXT.MSG$(58))           !CJK
                ENDIF ELSE BEGIN                                        !CJK
                    IF RET.KEY% = ENTER.KEY% THEN BEGIN

                        ! Check the entry in day selection screen
                        SCREEN.NUM% = DIRECTORY.PROCESS.SCR%
                        GOSUB CHECK.DAY.SELECTION

                        ! If any error in function, set same screen
                        IF NOT FUNCTION.ERROR.NOT.EXIST THEN BEGIN
                            SCREEN% = DIRECTORY.DAY.SELECT.SCR%
                        ENDIF

                    ENDIF ELSE BEGIN
                        ! B001 Invalid key pressed
                        CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))
                    ENDIF
                ENDIF
            ENDIF

        ! Directory restore status screen
        ENDIF ELSE IF SCREEN% = DIRECTORY.PROCESS.SCR% THEN BEGIN
            GOSUB SHOW.PROCESS.DIRECTORY
        ENDIF
    WEND

EXIT SUB

\***********************************************************************
\*
\* SHOW.DIRECTORIES: This Routine displays the directories
\*
\***********************************************************************
SHOW.DIRECTORIES:

    ! Setting the Header and other variables used for screen display
    SCR.HEADER$   = SCREEN.TEXT.MSG$(1)
    OPT.SELECTED$ = SCREEN.TEXT.MSG$(2) + SCREEN.TEXT.MSG$(3)
    OPT.HEADER.1$ = SCREEN.TEXT.MSG$(4)                  + \            !CJK
                    BKPSCRPT.DIRECTORIES$(SELECT.INDEX%) + \            !CJK
                    SCREEN.TEXT.MSG$(5)
    OPT.HEADER$   = SCREEN.TEXT.MSG$(6)

    CALL DM.SHOW.SCREEN(2, SCR.HEADER$, 5, 5)

    ! Setting the XRE value which will be displayed in the Left corner
    SCREEN.NUM$ = "06"

    ! Dimensioning array
    DIM DAY.ARRAY$(ARRAY.LIMIT%)                                        !CJK
    DIM BKP.AVAIL.ARRAY$(ARRAY.LIMIT%)                                  !CJK

    ! Setting the values to get the backup details                      !CJK
    FIRST.FILE$  = IMG.FILE$                                            !CJK
    SECOND.FILE$ = ALT.FILE$                                            !CJK

    ! Defaulting to zero                                                !CJK
    FUNCTION.ERROR.NOT.EXIST = 0                                        !CJK
    ! Calling the routine to get the backup value details               !CJK
    GOSUB GET.BKP.DETAILS

    ! Setting the screen number
    CALL DM.NAME (2, "SCREEN.NUM$", SCREEN.NUM$)

    ! Initialising the output fields in the screen
    ! before processing the screen
    !--------------------------------------------
    CALL DM.NAME (48, "OPT.HEADER.1$", OPT.HEADER.1$)

    ! If backups are available
    IF VALUE.INDEX% <> XRE.ZERO% AND FUNCTION.ERROR.NOT.EXIST THEN BEGIN

        ! Enabling the DAY and DD/MM string
        CALL DM.VISIBLE ("75", STATUS.TEXT.MSG$(61))
        CALL DM.VISIBLE ("76", STATUS.TEXT.MSG$(61))

        ! Setting other relevant string
        CALL DM.NAME (49, "OPT.HEADER$",   OPT.HEADER$  )               !CJK
        CALL DM.NAME (50, "OPT.SELECTED$", OPT.SELECTED$)

        ! Setting the first value of the fields before populating it
        DAY.LOOP%   = DAY.INDEX%                                        !CJK
        DD.MM.LOOP% = DD.MM.INDEX%                                      !CJK
        INPUT.LOOP% = INPUT.INDEX%                                      !CJK

        ! Retrieving the values and storing in Field$
        FOR INDEX% = 1 TO VALUE.INDEX%

            ! DAY value and its visibility
            FIELD$(DAY.LOOP%) = DAY.ARRAY$(INDEX%)
            CALL DM.VISIBLE (STR$(DAY.LOOP%), STATUS.TEXT.MSG$(61))

            ! DD/MM value
            FIELD$(DD.MM.LOOP%) = RIGHT$(BKP.AVAIL.ARRAY$(INDEX%),2) + \
                                  "/"                                + \
                                  LEFT$(BKP.AVAIL.ARRAY$(INDEX%),2)

            ! Setting a space for input values
            FIELD$(INPUT.LOOP%) = XRE.SPACE$

            ! Setting the visibility for DD/MM and input
            CALL DM.VISIBLE (STR$(DD.MM.LOOP%), STATUS.TEXT.MSG$(61))
            CALL DM.VISIBLE (STR$(INPUT.LOOP%), STATUS.TEXT.MSG$(61))

            ! Incrementing to move to the next field
            DAY.LOOP%   = DAY.LOOP%   + 1
            DD.MM.LOOP% = DD.MM.LOOP% + 1
            INPUT.LOOP% = INPUT.LOOP% + 1

        NEXT INDEX%

    ENDIF ELSE BEGIN
        IF FUNCTION.ERROR.NOT.EXIST THEN BEGIN                          !CJK
            FIELD$(1) = STATUS.TEXT.ERROR$(21)                          !CJK
        ENDIF ELSE BEGIN                                                !CJK
            FIELD$(1) = SCREEN.TEXT.MSG$(7) + SCREEN.TEXT.MSG$(8)       !CJK
        ENDIF                                                           !CJK
    ENDIF

RETURN

\***********************************************************************
\*
\* SHOW.PROCESS.DIRECTORY: This Routine displays the directory Restoring
\*                         process.
\*
\***********************************************************************
SHOW.PROCESS.DIRECTORY:

    ! Setting the Header and other variables used for screen display
    SCR.HEADER$ = SCREEN.TEXT.MSG$(9)                                   !CJK

    ! Changed the help screen to zero, as it is not needed
    CALL DM.SHOW.SCREEN(6, SCR.HEADER$, XRE.ZERO%, XRE.ZERO%)

    ! Setting the Dynamic directory value
    IF POSF(7) = 7 THEN BEGIN
        CALL PUTF(STRING$(3,XRE.SPACE$)                              + \
                  BKPSCRPT.DIRECTORIES$(SELECT.INDEX%))
    ENDIF

    ! Displaying the directory name in status bar
    CALL DM.STATUS ("'" + BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)       + \
                    SCREEN.TEXT.MSG$(10) + "Please Wait .....")         !CJK

    BKP.INDEX%        = XRE.ZERO%
    RESTORE.ERR.EXIST = FALSE                                           !EJK
    RESTORE.STATUS    = FALSE

    STATUS.MSG$ = BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)               + \!CJK
                  SCREEN.TEXT.MSG$(11) + BKP.DATE.ARRAY$(SEL.INDEX%) + \!CJK
                  SCREEN.TEXT.MSG$(12) + DAY.ARRAY$(SEL.INDEX%)      + \!CJK
                  " is being extracted"                                 !CJK
    GOSUB LOG.STATUS.MSG

    !----------------------------------------------------------!
    ! Setting the BKP.INDEX% and FULL.TO.MOVING.DATE$ based    !
    ! on the selected DAY and depending on the SET values the  !
    ! restoration process will happen                          !
    !----------------------------------------------------------!
    GOSUB SET.BACKUP.INDEX

    ! Match for the day and set according to the Full backup day
    BKP.INDEX% = LONG.DAY.INDEX%
    F02.DATE$  = BKP.DATE.ARRAY$(SEL.INDEX%)
    FUN.RC2%   = XRE.ZERO%                                              !CJK

    IF BKP.INDEX% <> 0 THEN BEGIN
        FUN.RC2% = UPDATE.DATE( ((BKP.INDEX% - 1)* -1 ) )               !CJK
    ENDIF

    ! Checking the Return code
    GOSUB CHECK.UPDATE.DATE.RC
    ! Setting the respective previous Full day
    FULL.TO.MOVING.DATE$ = F02.DATE$

    ! If directory not exist, create it
    IF FUNC.DIR.NOT.EXISTS(BKPSCRPT.DIRECTORIES$(SELECT.INDEX%))       \
    THEN BEGIN

        DIRECT.TO.RESTORE$ = BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)

        ! Trim the last slash found to avoid error using MKDIR
        CALL TRIM   (DIRECT.TO.RESTORE$)
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("/"))
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("\\"))

        CALL OSSHELL("MKDIR " + DIRECT.TO.RESTORE$ + " >> "          + \
                     DIR.OUT$ + " >>* " + DIR.OUT$)
        STATUS.MSG$ = DIRECT.TO.RESTORE$ + " directory is created"
        GOSUB LOG.STATUS.MSG

    ENDIF

    ! ZIP file extraction process
    GOSUB EXTRACT.DIRECTORIES

    STATUS.MSG$ = "Checking " + BKPLIST.CURR.FILE$ + " to check the" + \
                  " extracted file"
    GOSUB LOG.STATUS.MSG

    !---------------------------------------------------------------!
    ! Checking whether BKPLIST file exist and the requested date is !
    ! NOT Full, If true then the respective BKPLIST file will be    !
    ! checked to delete all the files which are stated as not       !
    ! exist (NULL) in BKPLIST for the selected day                  !
    !---------------------------------------------------------------!
    IF BKP.INDEX% > 1 THEN BEGIN
        ! Checking the file existence before opening it                 !CJK
        IF FUNC.FILE.EXISTS(BKPLIST.CURR.FILE$) THEN BEGIN
            BKPLIST.FILE.RC% = \                                        !DJK
                    FUNC.OPEN.SEQUENTIAL.FILE (BKPLIST.CURR.FILE$)      !DJK
            ! If file open unsuccessful
            IF BKPLIST.FILE.RC% <= XRE.ZERO% THEN BEGIN
                STATUS.MSG$ = "Error in opening BKPLIST file"
                GOSUB LOG.STATUS.MSG
                ! Setting NULL to avoid file read
                BKPLIST.DIR.VALUE$ = XRE.NULL$
            ENDIF ELSE BEGIN
                BKPLIST.OPEN       = TRUE
                BKPLIST.DIR.VALUE$ = XRE.SPACE$
            ENDIF

            ! Read the file till the EOF or read error
            WHILE LEN(BKPLIST.DIR.VALUE$) <> XRE.ZERO%

                BKPLIST.DIR.VALUE$ = \
                    FUNC.READ.SEQUENTIAL.FILE(BKPLIST.FILE.RC%)         !DJK

                !---------------------------------------------------!
                ! Checking the directory name of BKPLIST files with !
                ! the requested directory                           !
                !---------------------------------------------------!
                IF (BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)) =            \
                    LEFT$(BKPLIST.DIR.VALUE$,                          \
                    LEN(BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)))         \
                THEN BEGIN

                    ! Extract the individual fields using function
                    GOSUB EXTRACT.BKPLIST.FIELDS

                    IF NOT FUNCTION.ERROR.NOT.EXIST THEN BEGIN
                        ! Setting the value for exception
                        RESTORE.ERR.EXIST = TRUE                        !EJK
                    ENDIF

                    IF BKPLI.INCREMENTAL.EXIST$(BKP.INDEX%-1) <> "Y" \
                    THEN BEGIN
                        GOSUB DELETE.BKPLIST.FILE
                    ENDIF
                ENDIF
            WEND

            ! Closing File
            IF BKPLIST.OPEN THEN BEGIN
                CALL FUNC.CLOSE.FILE( BKPLIST.FILE.RC% )                !DJK
                BKPLIST.OPEN = FALSE
            ENDIF
        ENDIF
    ENDIF

    ! Setting the Dynamic directory value
    IF POSF(238) = 238 THEN BEGIN
        CALL PUTF(STRING$(10,XRE.SPACE$) + \
                          "RESTORE A DIRECTORY RESTORATION STATUS")
    ENDIF

    !-----------------------------------------!
    ! Set the field to display the Directory  !
    ! extraction process completed message    !
    !-----------------------------------------!
    FIELD$(6) = " The directory "                                    + \
                BKPSCRPT.DIRECTORIES$(SELECT.INDEX%) + " from "      + \
                DAY.ARRAY$(SEL.INDEX%) + ", "                        + \
                RIGHT$(BKP.DATE.ARRAY$(SEL.INDEX%),2) + "/"          + \
                MID$(BKP.DATE.ARRAY$(SEL.INDEX%),3,2)                + \
                " has been restored "
    FIELD$(7) = " to " + BKPSCRPT.DIRECTORIES$(SELECT.INDEX%) +  "."

    ! Disable the visibility of other irrelevant fields
    CALL DM.VISIBLE("5",STATUS.TEXT.MSG$(60))
    CALL DM.VISIBLE("8",STATUS.TEXT.MSG$(60))
    CALL DM.VISIBLE("9",STATUS.TEXT.MSG$(60))

    ! If restore without any error
    IF NOT RESTORE.ERR.EXIST THEN BEGIN                                 !EJK

        CALL DM.STATUS ("'Restore completed successfully")

        STATUS.MSG$ = "Restore completed successfully"

    ! If restore with exceptions                                        !EJK
    ENDIF ELSE BEGIN                                                    !EJK

        CALL DM.STATUS (SCREEN.TEXT.MSG$(26))
        STATUS.MSG$ = STATUS.TEXT.MSG$(5)

! Commenting out, as the logic is redundant and unused                  !EJK
!    ! If backup file not present                                       !EJK
!    ENDIF ELSE BEGIN                                                   !EJK
!        CALL DM.STATUS ("'Restore unsuccessful. Backup file not" + \   !EJK
!                        " present")                                    !EJK
!        STATUS.MSG$ = "Backup file not present"                        !EJK
    ENDIF

    GOSUB LOG.STATUS.MSG

    WHILE SCREEN% = DIRECTORY.PROCESS.SCR%
        RET.KEY% = DM.PROCESS.SCREEN (2, 9, FALSE)

        IF RET.KEY% = ESC.KEY% THEN BEGIN                               !CJK

            SCREEN%      = DIRECTORY.DAY.SELECT.SCR%
            PREVIOUS.KEY = TRUE

        ENDIF ELSE BEGIN

            CALL DM.FOCUS ("1", SCREEN.TEXT.MSG$(13) + \                !CJK
                                SCREEN.TEXT.MSG$(29))                   !CJK
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* DELETE.BKPLIST.FILE: This Routine deletes the BKPLIST file
\*                      which is set as Non-exist.
\*
\***********************************************************************
DELETE.BKPLIST.FILE:

    CALL OSSHELL("DEL " + BKPLI.FILENAME$ + " >> " + DIR.OUT$ + \
                 " >>* " + DIR.OUT$)
    STATUS.MSG$ = STATUS.TEXT.MSG$(7) + BKPLI.FILENAME$
    GOSUB LOG.STATUS.MSG

RETURN

\***********************************************************************
\*
\* EXTRACT.DIRECTORIES: This Routine extracts the selected directories
\*                      archive file.
\*
\***********************************************************************
EXTRACT.DIRECTORIES:

    !------------------------------------------------------------------!
    ! Initiating the Directory restore. Depending on the selected day, !
    ! BKP.INDEX% value would have been set. For example, if WED then   !
    ! BKP.INDEX% value would be 4, so that from Full archive 4 next    !
    ! archive files would be extracted                                 !
    !------------------------------------------------------------------!
    FOR LOOP% = 1 TO BKP.INDEX%

        STATUS.MSG$ = BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)        + \   !CJK
                      STATUS.TEXT.MSG$(31) + FULL.TO.MOVING.DATE$ + \
                      SCREEN.TEXT.MSG$(16)
        GOSUB LOG.STATUS.MSG

        GOSUB GET.FILE.EXTENSION
        MDD.DATE$ = EXTENSION$

        ! Storing the Full MDD
        IF LOOP% = 1 THEN BEGIN
            FULL.MDD.DATE$ = MDD.DATE$

            IF UCASE$(LEFT$(BKPSCRPT.DIRECTORIES$(INDEX%),1)) = "C" \   !CJK
            THEN BEGIN                                                  !CJK
                ! BKPLIST current file                                  !CJK
                BKPLIST.CURR.FILE$ = BKPLIST.PREFIX.D.DRIVE$ + \        !CJK
                                     FULL.MDD.DATE$                     !CJK
            ENDIF ELSE BEGIN                                            !CJK
                ! BKPLIST current file                                  !CJK
                BKPLIST.CURR.FILE$ = BKPLIST.PREFIX.C.DRIVE$ + \        !CJK
                                     FULL.MDD.DATE$                     !CJK
            ENDIF                                                       !CJK
        ENDIF

        ! Setting the Backup files based on the 1st field which is drive
        IF LEFT$(FILENAME$,1) = "D" THEN BEGIN
            ! IMG and ALT directory for C drive
            BKP.FILENAME.IMG$ = C.BKP.IMG$ + FILENAME$ + "." + MDD.DATE$
            BKP.FILENAME.ALT$ = C.BKP.ALT$ + FILENAME$ + "." + MDD.DATE$

            BKPFAIL.PREFIX$ = C.BKP.IMG$ + "BKPFAILD."                  !CJK
        ENDIF ELSE BEGIN
            ! IMG and ALT directory for D drive
            BKP.FILENAME.IMG$ = D.BKP.IMG$ + FILENAME$ + "." + MDD.DATE$
            BKP.FILENAME.ALT$ = D.BKP.ALT$ + FILENAME$ + "." + MDD.DATE$

            BKPFAIL.PREFIX$ = D.BKP.IMG$ + "BKPFAILC."                  !CJK
        ENDIF

        ! Current BKPFAIL file
        BKPFAIL.CURR.FILE$ = BKPFAIL.PREFIX$ + MDD.DATE$                !CJK

        STATUS.MSG$ = "Checking BKPFAIL " + BKPFAIL.CURR.FILE$
        GOSUB LOG.STATUS.MSG

        DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( BKPFAIL.CURR.FILE$ )  !DJK

        ! If file open unsuccessful
        IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
            ! Setting NULL to avoid file read
            DIR.VALUE$ = XRE.NULL$
            DIR.OPEN   = FALSE
        ENDIF ELSE BEGIN
            DIR.OPEN   = TRUE
            DIR.VALUE$ = XRE.SPACE$
        ENDIF

        !-------------------------------------------------------------!
        ! Reading the file till the EOF file reached. C file function !
        ! returns NULL when EOF reached or read error                 !
        !-------------------------------------------------------------!
        WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
            DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )      !DJK

            ! Checking the comma position
            COMMA.POSITION% = MATCH(COMMA.VALUE$,DIR.VALUE$,1)

            ! If comma found
            IF COMMA.POSITION% <> XRE.ZERO% THEN BEGIN

                ! Storing the failed file and Distribution type
                FAILED.FILE$ = LEFT$(DIR.VALUE$,(COMMA.POSITION% - 1))
                FAILED.FILE.DIST$ = MID$(DIR.VALUE$,           \        !CJK
                                        (COMMA.POSITION% + 1), \        !CJK
                                        1)

                ! If directory in BKPFAIL matches with current directory!CJK
                IF (BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)) =    \        !CJK
                    LEFT$(FAILED.FILE$,                        \        !CJK
                    LEN(BKPSCRPT.DIRECTORIES$(SELECT.INDEX%))) \        !CJK
                THEN BEGIN

                    BEGIN.POSITION% = 4         ! Setting the begin
                    SLASH.POSITION% = XRE.ZERO% ! position
                    VALUE.EXISTS    = TRUE
                    FILENAME$       = XRE.NULL$

                    ! Extracting the file name
                    WHILE VALUE.EXISTS
                        SLASH.POSITION% = MATCH("\\", FAILED.FILE$,    \
                                                 BEGIN.POSITION%)

                        IF SLASH.POSITION% > XRE.ZERO% THEN BEGIN
                            ! Move to next position to search next field
                            BEGIN.POSITION% = SLASH.POSITION% + 1
                        ENDIF ELSE BEGIN
                            FILENAME$ = MID$(FAILED.FILE$,        \     !CJK
                                             BEGIN.POSITION%,     \     !CJK
                                             (LEN(FAILED.FILE$) - \     !CJK
                                             BEGIN.POSITION% + 1))      !CJK
                            ! Storing the file name                     !CJK
                            RESTORE.FILENAME$ = FILENAME$               !BJK
                            ! File name without extension               !CJK
                            MATCH.POS% = MATCH(".", FILENAME$,1)        !BJK

                            IF MATCH.POS% <> XRE.ZERO% THEN BEGIN       !BJK
                                RESTORE.FILENAME$ = LEFT$(FILENAME$, \  !BJK
                                                    (MATCH.POS% - 1))   !BJK
                            ENDIF                                       !BJK
                            VALUE.EXISTS = FALSE
                        ENDIF
                    WEND

                    STATUS.MSG$ = STATUS.TEXT.MSG$(9)
                    GOSUB LOG.STATUS.MSG

                    STATUS.MSG$ = STATUS.TEXT.MSG$(10) + FILENAME$      !CJK
                    GOSUB LOG.STATUS.MSG

                    ! Depending on the drive, XDISKIMG directory will   !CJK
                    ! be used in copying failed file                    !CJK
                    IF LEFT$(FILENAME$,1) = "D" THEN BEGIN              !CJK
                        ! Copying the file to respective directory      !CJK
                        CALL OSSHELL("COPY " + C.BKP.IMG$             + \CJK
                                     RESTORE.FILENAME$ + "."          + \CJK
                                     MDD.DATE$ + XRE.SPACE$           + \CJK
                                     FAILED.FILE$ + " > " + DIR.OUT$  + \CJK
                                     " >>* " + DIR.OUT$)                !CJK
                    ENDIF ELSE BEGIN                                    !CJK
                        ! Copying the file to respective directory      !CJK
                        CALL OSSHELL("COPY " + D.BKP.IMG$             + \CJK
                                     RESTORE.FILENAME$ + "."          + \CJK
                                     MDD.DATE$ + XRE.SPACE$           + \CJK
                                     FAILED.FILE$ + " > " + DIR.OUT$  + \CJK
                                     " >>* " + DIR.OUT$)                !CJK
                    ENDIF
                    !---------------------------------------------!
                    ! Not distributing the file if SUPPS mode, as !
                    ! ADXCSU0L won't work under SUPPS             !
                    !---------------------------------------------!
                    IF NOT SUPPS.ON THEN BEGIN
                        ! Setting the distribution type
                        CALL OSSHELL(ADXCSU0L.FILE.NAME$ + " 3 "     + \!CJK
                                     FAILED.FILE.DIST$ + XRE.SPACE$  + \
                                     FAILED.FILE$ + " >> "           + \
                                     DIR.OUT$ + " >>* " + DIR.OUT$)
                    ENDIF

                    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )!DJK

                    ! If file open unsuccessful
                    IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                        STATUS.MSG$ = STATUS.TEXT.ERROR$(2)
                        GOSUB LOG.STATUS.MSG
                        ! Setting NULL to avoid file read
                        DIR.VALUE$ = XRE.NULL$
                        DIR.OPEN   = FALSE
                    ENDIF ELSE BEGIN
                        DIR.OPEN   = TRUE
                        DIR.VALUE$ = XRE.SPACE$
                    ENDIF

                    ! Read the file till EOF or read error
                    WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                        DIR.VALUE$ = \                                  !DJK
                            FUNC.READ.SEQUENTIAL.FILE(DIR.FILE.RC%)     !DJK

                       !-----------------------------------------------!
                       ! If error string matches, write the error with !
                       ! file name in LOG file                         !
                       !-----------------------------------------------!
                        IF MATCH("ERROR",UCASE$(DIR.VALUE$),1)     \    !EJK
                           <> XRE.ZERO% OR                         \    !EJK
                           MATCH("cannot be found",(DIR.VALUE$),1) \    !EJK
                           <> XRE.ZERO% THEN BEGIN                      !EJK
                            ! Copy error and distribution error
                            STATUS.MSG$ = STATUS.TEXT.ERROR$(14)
                            GOSUB LOG.STATUS.MSG
                            STATUS.MSG$ = DIR.VALUE$
                            GOSUB LOG.STATUS.MSG

                            RESTORE.ERR.EXIST = TRUE                    !EJK
                            ! To break the WHILE loop                   !EJK
                            DIR.VALUE$ = XRE.NULL$                      !EJK
                        ENDIF
                    WEND

                    ! Closing File
                    IF DIR.OPEN THEN BEGIN
                        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )            !DJK
                        DIR.OPEN = FALSE
                    ENDIF
                ENDIF
            ENDIF
        WEND

        ! Closing File
        IF DIR.OPEN THEN BEGIN
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !DJK
            DIR.OPEN = FALSE
        ENDIF

        !--------------------------------------------------------------!
        ! Checking the archive file existence before extraction. First !
        ! the file will be checked in IMG location and it not present, !
        ! then it will be tried in ALT location                        !
        !--------------------------------------------------------------!
        IF FUNC.FILE.EXISTS(BKP.FILENAME.IMG$) THEN BEGIN
            CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -x "         + \       !CJK
                         BKP.FILENAME.IMG$   + XRE.SPACE$     + \       !CJK
                         BKPSCRPT.DIRECTORIES$(SELECT.INDEX%) + \       !CJK
                         " > " + DIR.OUT$ + " >>* " + DIR.OUT$)

            STATUS.MSG$ = BKP.FILENAME.IMG$ + STATUS.TEXT.MSG$(11) + \  !CJK
                          BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)
            GOSUB LOG.STATUS.MSG

            ! Setting the status to check the extraction output file
            RESTORE.STATUS = TRUE

        ENDIF ELSE BEGIN                                                !DJK
            IF FUNC.FILE.EXISTS(BKP.FILENAME.ALT$) THEN BEGIN           !DJK
                CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -x "         + \   !CJK
                             BKP.FILENAME.ALT$   + XRE.SPACE$     + \   !CJK
                             BKPSCRPT.DIRECTORIES$(SELECT.INDEX%) + \   !CJK
                             " > " + DIR.OUT$ + " >>* " + DIR.OUT$)

                STATUS.MSG$ = BKP.FILENAME.ALT$ + STATUS.TEXT.MSG$(11)+ \CJK
                              BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)
                GOSUB LOG.STATUS.MSG

                ! Setting the status to check the extraction output file
                RESTORE.STATUS = TRUE
            ENDIF
        ENDIF                                                           !DJK

        ! If file extraction happened
        IF RESTORE.STATUS THEN BEGIN                                    !EJK

            DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )        !DJK

            ! If file open unsuccessful
            IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                DIR.OPEN    = FALSE                                     !CJK
                STATUS.MSG$ = STATUS.TEXT.ERROR$(2)
                GOSUB LOG.STATUS.MSG
                ! Setting NULL to avoid file read
                DIR.VALUE$ = XRE.NULL$
            ENDIF ELSE BEGIN
                DIR.OPEN   = TRUE
                DIR.VALUE$ = XRE.SPACE$
            ENDIF
            ! Read the file till EOF or read error
            WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )  !DJK

                !-----------------------------------------------!
                ! If error string matches, write the error with !
                ! file name in LOG file                         !
                !-----------------------------------------------!
                IF MATCH("Error extracting file",DIR.VALUE$,1) <> \     !CJK
                   XRE.ZERO% THEN BEGIN

!                    STATUS.MSG$ = STATUS.TEXT.ERROR$(12)               !EJK
!                    GOSUB LOG.STATUS.MSG                               !EJK

                    ! Setting the value for extraction error
                    RESTORE.ERR.EXIST = TRUE                            !EJK
                    STATUS.MSG$       = DIR.VALUE$                      !EJK
                    GOSUB LOG.STATUS.MSG
                ENDIF
            WEND

            ! Closing File
            IF DIR.OPEN THEN BEGIN
                CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                    !DJK
                DIR.OPEN = FALSE
            ENDIF

        ENDIF

        ! Starting from Full increment the date till requested date
        F02.DATE$ = FULL.TO.MOVING.DATE$
        FUN.RC2%  = UPDATE.DATE( 1 )                                    !CJK
        ! Checking the Return code
        GOSUB CHECK.UPDATE.DATE.RC
        FULL.TO.MOVING.DATE$ = F02.DATE$

    NEXT LOOP%

RETURN

! duplicated subroutine
%INCLUDE XREST00E.J86                                                   !DJK

END SUB

