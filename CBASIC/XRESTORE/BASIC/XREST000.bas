\***********************************************************************
\*
\*            PROGRAM         :       XRESTORE
\*            MODULE          :       XREST000
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
\* This program displays user friendly screens and allows the user to
\* restore both drives OR single drive OR a single directory OR a
\* single file. This program will only restore on the Acting Master
\* & File Server.
\*
\* For Drive restore, the program checks the existence of BKPFAIL and
\* displays the number of available backup dates. Using available
\* Backup dates, the drive restoration process will be proceeded.
\*
\* For Directory restore, the available directory will be shown from
\* BKPSCRPT file. Then selected directory will be checked in XDISKIMG
\* or XDISKALT (If C:/ADX_UPGM/ is selected, CUPGM.* will be searched)
\* and all the available days will be displayed in the next screen.
\* Using available backup dates, the directory restoration process
\* will be proceeded.
\*
\* For File restore, name of the complete file will be taken as an
\* input. Then the directory of the file will be checked, similar as
\* above directory search. All the available dates will be displayed.
\* ADXUNZIP will be used for File Restore as it has option to extract
\* single file from ZIP archive.
\*
\* Refer BKPSCRPT.TXT for all the available directories eligible
\* under restore.
\*
\* SCRIPT FILE
\* ===========
\* BKPSCRPT file would be in following record format:
\*
\*      COMMAND          REQUIRED VALUES FOLLOWING
\*      -------          -------------------------
\*      TIME RANGE       START TIME     END TIME
\*      DAYS TO KEEP     NO. OF DAYS
\*      BACKUP           DIRECTORY      PRIMARY     SECONDARY
\*                                      ARCHIVE     ARCHIVE
\*                                      DIRECTORY   DIRECTORY
\*      EXCLUDE          FILE NAME
\*
\* PASSED PARAMTERS N/A
\* ================
\*
\* INPUT AND OUTPUT FILES
\* ======================
\* Input files : BKPSCRPT.TXT  (Backup Script File)
\*               BKPLIST.MDD   (Backup List File)
\*               BKPFAILC.MDD  (Backup Fail File for C drive)
\*               BKPFAILD.MDD  (Backup Fail File for D drive)
\*               C:\XDISKIMG\*.* (D directories primary archive)
\*               D:\XDISKIMG\*.* (C directories primary archive)
\*               C:\XDISKALT\*.* (D directories Secondary archive)
\*               D:\XDISKALT\*.* (C directories Secondary archive)
\*
\* Output files: XRESTORE.LOG    (XRESTORE Log File)
\*               EXTRACTED FILES
\*               (e.g. Files will be restored to C:\ADX_SPGM\
\*                from CSPGM.* archive files)
\*
\*======================================================================
\*                   V E R S I O N   C O N T R O L
\* (Update STATUS.TEXT.MSG$(12) for Application Version. Consider
\*  Version H as a Base version 1.0)
\*======================================================================
\*
\* Version B               Jaya kumar Inbaraj                 18/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to the BKPLIST file function changes
\*
\* Version C               Jaya kumar Inbaraj                 29/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to Internal review comments
\*
\* Version D               Jaya kumar Inbaraj                 09/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to Application Management Team
\* review comments
\*
\* Version E               Jaya kumar Inbaraj                 14/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to Application Management Team
\* review comments
\*
\* Version F     Jaya kumar Inbaraj / Dave Constable          21/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Internal and Application Management Team review comments
\* QC671 - adjusted max directory length to 12 (from 20)
\* QC665 - added termination call to sequence to get correct message
\*
\* Version G               Jaya kumar Inbaraj                 04/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC 653, 654, 656, 660, 662, 663, 664, 674, 675, 676
\* Text on the screen / log and the logic has been updated. Also updated
\* the corresponding screen files.
\*
\* Version H               Jaya kumar Inbaraj                 09/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* Added a separate variable to detect the directory creation error in
\* File restoration.
\*
\* Version I                 Dave Constable                   05/06/2014
\* FOD260 - Enhanced Backup and Recovery changes to enable CR for
\* configurable Full backup day and code review changes
\*
\* Version J                 Dave Constable                   25/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* Code review changes; alignment of file open status
\* QC824 - corrected LAN file name for slpcf to local or SUPPS fail
\*
\*       MODULE RENAMED TO XREST000.BAS AS BREACHED 64K LIMIT
\*       ORIGINAL SUB PROGRAM MOVED INTO XREST001.BAS
\*
\* Version K                 Dave Constable                   10/07/2014
\* FOD260 - Enhanced Backup and Recovery
\* CR4 - Help screen changes
\*
\* Version L                 Jaya Kumar Inbaraj               01/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC717 and QC724 - Restoration for files in BKPFAIL has been fixed.
\*
\* Version M                 Jaya Kumar Inbaraj               23/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* CR5 changes to have configuration files in both C and D drives. Also
\* BKPFAIL.MDD file has been replaced with BKPFAILC.MDD and BKPFAILD.MDD
\* to have separate BKPFAIL file for C and D drive.
\* Also worked on Internal and APPS management review comments.
\* Added an Header for easy reference on multi-modular program.
\*
\* Version N                 Jaya Kumar Inbaraj               04/09/2014
\* FOD260 - Enhanced Backup and Recovery
\* Worked on APPS management review comments.
\*
\* Version O                 Jaya Kumar Inbaraj               12/09/2014
\* QC1145 - Using ADXUNZIP for file restore.
\*
\* Version P                 Ranjith Gopalankutty             12/07/2017
\* Enhancement done to XRESTORE, going forward it will not refer the
\* sleepr file for identifying the full backup and incremental back
\* ups. As there are no more incremental backups going forward. 
\* XRESTORE will just do the backup based on the date and file 
\* extension.
\***********************************************************************

\***********************************************************************
\*
\* Included global variables                                            !KDC
\*
\***********************************************************************
    %INCLUDE XRESTORG.J86                                               !MJK

\***********************************************************************
\*
\* Included external functions                                          !KDC
\*
\***********************************************************************
    %INCLUDE XRESTORE.J86                                               !MJK

\***********************************************************************
\*
\* SUB.PROCESS.DAY.DIR.SCREEN: This Sub-Program does all the necessary  !MJK
\*                             actions needed for directory restore     !MJK
\*                             processing including screen navigation.  !MJK
\*
\*                             It receives the directory values as an   !MJK
\*                             input.                                   !MJK
\*
\* As a program modularisation, Directory restore has been made as a
\* Sub-program to perform all the Directory restore functionalities
\* within itself. This enables quick extraction from the main module
\* should breach the 64k limit
\*
\***********************************************************************
SUB SUB.PROCESS.DAY.DIR.SCREEN(IMG.FILE$, ALT.FILE$) EXTERNAL           !KDC
    STRING              \
        ALT.FILE$,      \
        IMG.FILE$
END SUB

\***********************************************************************
\*
\*    FUNC.DIR.NOT.EXISTS: This function checks the existence of given
\*                         directory by using CHDIR command.
\*
\***********************************************************************
FUNCTION FUNC.DIR.NOT.EXISTS(DIRECTORY.NAME$) PUBLIC                    !KDC

    INTEGER*1 FUNC.DIR.NOT.EXISTS
    STRING    DIRECTORY.NAME$

ON ERROR GOTO DIR.NOT.EXISTS.ERR                                        !CJK

    FUNC.DIR.NOT.EXISTS = TRUE

    ! Trim the last slash found to avoid error using CHDIR
    CALL TRIM   (DIRECTORY.NAME$)
    CALL RTRIMC (DIRECTORY.NAME$, ASC("/"))
    CALL RTRIMC (DIRECTORY.NAME$, ASC("\\"))

    CHDIR DIRECTORY.NAME$
    FUNC.DIR.NOT.EXISTS = FALSE

DIR.NOT.EXISTS.ERR:

END FUNCTION

\***********************************************************************
\*
\*    FUNC.FILE.EXISTS: This function checks the existence of
\*                      passed file by using SIZE function.
\*
\***********************************************************************
FUNCTION FUNC.FILE.EXISTS(FILE.NAME$) PUBLIC                            !KDC

    STRING    FILE.NAME$
    INTEGER*1 FUNC.FILE.EXISTS

    ON ERROR GOTO FILE.DOES.NOT.EXIST                                   !CJK

    CALL TRIM(FILE.NAME$)

    FUNC.FILE.EXISTS = TRUE

    FUN.RC2% = SIZE(FILE.NAME$)                                         !MJK

    EXIT FUNCTION

FILE.DOES.NOT.EXIST:
    FUNC.FILE.EXISTS = FALSE

END FUNCTION

\***********************************************************************
\*
\* Generic Comments: For all the screens, FIELD$(1) (status line) value
\*                   is hard-coded instead of using "Message no.". This
\*                   is because "Message No." is not accessible from
\*                   SUPPS mode, as BEMF.BIN file is not accessible.
\*
\***********************************************************************

\***********************************************************************
\***********************************************************************
\***********************************************************************
\*                                                                     *
\*          S T A R T   O F   M A I N L I N E   C O D E                *
\*                                                                     *
\***********************************************************************
\***********************************************************************
\***********************************************************************

ON ERROR GOTO ERROR.DETECTED

    !----------------------------------------------------------!
    ! Executes USE statement. Throws 'NP' error when initiated !
    ! directly. As only two modes are defined, this error is   !
    ! utilized to differentiate the modes.                     !
    !----------------------------------------------------------!
    %INCLUDE PSBUSEE.J86

START.OF.PROGRAM:
    ! XRESTORE started

    GOSUB INITIALISATION
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION

    ! XRESTORE completed

! Called during abnormal run
STOP.PROGRAM:
    STOP

\***********************************************************************
\*
\* INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************
INITIALISATION:

    GOSUB INITIALISE.VARIABLES
    GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB CREATE.DIRECTORIES                                            !MJK
    GOSUB CREATE.XRESTORE.LOG
    GOSUB CHECK.XBACKUP.RUN
    GOSUB CREATE.RUN.PIPE
    GOSUB CONTROLLER.CONFIG.CHECK
    ! Below sub routine is commented as no need to refer to sleeper     !PRG
    ! as it was done for understanding the full backups and calcula-    !PRG
    ! -ting the incrementals.As incrementals are turned off.            !PRG

 !  GOSUB GET.SLEEPER.CONFIGURATION                                     !PRG IDC
    GOSUB DRIVE.FAT32.CHECK
    GOSUB INITIATE.DISP.MNGR

RETURN

\***********************************************************************
\*
\* MAIN.PROCESSING : This Sub-routine does the main processing for
\*                      creating the Backup.
\*
\***********************************************************************
MAIN.PROCESSING:

    GOSUB PROCESS.BKPSCRPT
    GOSUB SCREEN.NAVIGATION

RETURN

\***********************************************************************
\*
\* TERMINATION: Termination Sub-routine will be called before
\*                 program closure.
\*
\***********************************************************************
TERMINATION:

    GOSUB CLOSE.AND.DEALLOC.SESSIONS

RETURN

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                 INITIALISATION SPECIFIC ROUTINES                   *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************
\*
\* INITIALISE.VARIABLES : This Sub-routine Initialize all the
\*                        necessary variables which will be used
\*                        in this program.
\*
\***********************************************************************
INITIALISE.VARIABLES:

    ! set messages first in case of usage
    GOSUB INITALISE.MESSAGES

    ! Program variables
    BATCH.SCREEN.FLAG$ = "S"            ! Screen

    CHAIN.TO.PROG$     = "PSB50"        ! PSB50 for chain
    MODULE.NUMBER$     = "XRESTORE"     ! Current Module

    ! Screen related variables
    DAY.INDEX%   = 77                   ! Screen 2's Day                !MJK
    DD.MM.INDEX% = 3                    !            DD/MM              !MJK
    INPUT.INDEX% = 92                   !            Input              !MJK
    SCREEN%      = 1                    ! variables

    ! Screen number variables
    DIRECTORY.DAY.SELECT.SCR%    = 6    ! Directory day selection       !CJK
    DIRECTORY.PROCESS.SCR%       = 61   ! Directory restore process     !CJK
    DISPLAY.MAIN.SCR%            = 1    ! Restore main screen           !CJK
    DRIVE.DAY.SELECT.SCR%        = 2    ! Drive day selection           !CJK
    DRIVE.DISK.SELECT.SCR%       = 3    ! Drive Disk selection          !CJK
    DRIVE.PROCESS.SCR%           = 4    ! Drive restore process         !CJK
    FILE.CONFIRMATION.SCR%       = 10   ! File restore confirmation     !CJK
    FILE.DAY.SELECT.SCR%         = 8    ! File restore day selection    !CJK
    FILE.DIR.AND.DIST.SCR%       = 9    ! File directory & distribution !CJK
    FILE.PROCESS.SCR%            = 11   ! File restore process          !CJK
    RESTORE.A.DIRECTORY.SCR%     = 5    ! Directory display             !CJK
    RESTORE.A.FILE.SCR%          = 7    ! File name entry screen        !CJK

    ! Constant variables
    ARRAY.LIMIT%     = 200                 ! Setting the Array Index%   !MJK
    COMMA.VALUE$     = ","                 ! Comma value                !MJK
    CONSTANT.COLON$  = ":"                 ! Colon value                !MJK
    CRLF$            = CHR$(13) + CHR$(10) ! Assigning CR/LF            !MJK
    DIR.TO.SHOW%     = 23                  ! No. of directories/screen  !MJK
    FALSE            = 0                   ! False = 0                  !MJK
    TRUE             = -1                  ! True  = -1                 !MJK
    XRE.NULL$        = ""                  ! NULL value                 !MJK
    XRE.SPACE$       = " "                 ! Single space               !MJK
    XRE.ZERO%        = 0                   ! zero use                   !MJK

    ! Array variables
    BKP.INDEX% = XRE.ZERO%

    ! Controller variables
    MASTER.AND.FILE.SERVER% = 21        ! Master/File server value

    BKP.DAYS%   = 14                    ! Number of available backups

    ! File related variables
    C.BKP.ALT$  = "C:\XDISKALT\"        ! Alternate C BACKUP
    C.BKP.IMG$  = "C:\XDISKIMG\"        ! Primary C Backup
    D.BKP.ALT$  = "D:\XDISKALT\"        ! Alternate D BACKUP
    D.BKP.IMG$  = "D:\XDISKIMG\"        ! Primary D BACKUP
    DIR.OUT$    = "C:\DIR.OUT"          ! Directory listing             !MJK
    DIR1.OUT$   = "C:\DIR1.OUT"         ! STDOUT file                   !OJK
    TEMP.DIR$   = "C:\TEMP\"            ! Temp directory

    XBACK.PIPE.NAME$       = "pi:XBACKUP"               ! XBACKUP pipe
    XBACK.PIPE.REPORT.NUM% = 426                        ! Temporary num
    XRE.LOG.FILENAME$      = "D:/ADX_UDT1/XRESTORE.LOG" ! Log file name
    XRE.LOG.REPORT.NUM%    = 428                        ! Temporary num
    XRE.PIPE.NAME$         = "pi:XRESTORE"              ! Run pipe
    XRE.PIPE.REPORT.NUM%   = 427                        ! Temporary num

    ! It is arranged in weekly order                                    !MJK
    ! set day constants to avoid mistakes and repeats                   !IDC
    CONSTANT.SUNDAY.SHORT$    = "SUN"                                   !IDC
    CONSTANT.MONDAY.SHORT$    = "MON"                                   !IDC
    CONSTANT.TUESDAY.SHORT$   = "TUE"                                   !IDC
    CONSTANT.WEDNESDAY.SHORT$ = "WED"                                   !IDC
    CONSTANT.THURSDAY.SHORT$  = "THU"                                   !IDC
    CONSTANT.FRIDAY.SHORT$    = "FRI"                                   !IDC
    CONSTANT.SATURDAY.SHORT$  = "SAT"                                   !IDC
    ! It is arranged in weekly order                                    !MJK
    CONSTANT.SUNDAY.LONG$     = "Sunday"                                !IDC
    CONSTANT.MONDAY.LONG$     = "Monday"                                !IDC
    CONSTANT.TUESDAY.LONG$    = "Tuesday"                               !IDC
    CONSTANT.WEDNESDAY.LONG$  = "Wednesday"                             !IDC
    ! set this to the longest named day if the day identifiers change   !IDC
    ! at all so that later matching and extracting works ok             !IDC
    CONSTANT.LONGEST.DAY%     = LEN(CONSTANT.WEDNESDAY.LONG$)           !IDC
    CONSTANT.THURSDAY.LONG$   = "Thursday"                              !IDC
    CONSTANT.FRIDAY.LONG$     = "Friday"                                !IDC
    CONSTANT.SATURDAY.LONG$   = "Saturday"                              !IDC

    PARM.FULL$          = "F"                                           !IDC

    CALL SLPCF.SET

    ! Initialize variables before actual use
    ERROR.EXIST = FALSE
    SUPPS.ON    = FALSE                                                 !CJK

    ! File not opened yet, hence defaulting to false                    !MJK
    BKPLIST.OPEN = FALSE                                                !MJK
    DIR.OPEN     = FALSE                                                !MJK
    SLPCF.OPEN   = FALSE                                                !MJK
    XBACK.OPEN   = FALSE                                                !MJK
    XRE.LOG.OPEN = FALSE                                                !MJK
    XRE.OPEN     = FALSE                                                !MJK

    ! Incremental file flag for existing and changed                    !IDC
    DIM BKPLI.INCREMENTAL.EXIST$(6)                                     !IDC
    DIM BKPLI.INCREMENTAL.FILE.CHNG$(6)                                 !IDC

    ! Dimensioning array
    DIM BKP.FILE.MDD.ARRAY$(ARRAY.LIMIT%)                               !MJK
    DIM BKP.FILE.MMDD.ARRAY$(ARRAY.LIMIT%)                              !MJK
    DIM BKPSCRPT.DIRECTORIES$(ARRAY.LIMIT%)                             !MJK
    DIM PRIMARY.ARCHIVED.NAMES$(ARRAY.LIMIT%)                           !MJK
    DIM SECONDARY.ARCHVD.NAMES$(ARRAY.LIMIT%)                           !MJK

    GOSUB SET.FILE.NAMES                                                !MJK

RETURN

! IDC START BLOCK
\***********************************************************************
\*
\*    INITALISE.MESSAGES:This Sub-routine Initialize all the messages
\*                       used for status update and errors
\*
\***********************************************************************
INITALISE.MESSAGES:

    VERSION$ = "### XRESTORE.286 - Version M - 23/08/2014 ###"          !MJK

    DIM STATUS.TEXT.MSG$(63)                                            !JDC
    DIM STATUS.TEXT.ERROR$(22)                                          !JDC
    DIM SCREEN.TEXT.MSG$(31)                                            !JDC

    !******************************************************************
    ! Status update messages
    !******************************************************************
    STATUS.TEXT.MSG$(1)  = " directory is created"
    STATUS.TEXT.MSG$(2)  = "Checking "
    STATUS.TEXT.MSG$(3)  = " to check the"
    STATUS.TEXT.MSG$(4)  = "Restore completed successfully"
    STATUS.TEXT.MSG$(5)  = "Restore completed with exception"
    STATUS.TEXT.MSG$(6)  = "Backup file not present"
    STATUS.TEXT.MSG$(7)  = "Deleting file "
    STATUS.TEXT.MSG$(8)  = "Checking BKPFAIL "
    STATUS.TEXT.MSG$(9)  = "File found in BKPFAIL file"
    STATUS.TEXT.MSG$(10) = "Copying and distributing "
    STATUS.TEXT.MSG$(11) = " is extracted to "
    STATUS.TEXT.MSG$(12) = "Restore application started Version ""M"""  !MJK
    STATUS.TEXT.MSG$(13) = "Checking Controller configuration"
    STATUS.TEXT.MSG$(14) = "Controller status not obtainable"
    STATUS.TEXT.MSG$(15) = "This Controller is not configured "
    STATUS.TEXT.MSG$(16) = "SLPCF Full backup day is "
    STATUS.TEXT.MSG$(17) = "SLPCF Full backup configuration not found"
    STATUS.TEXT.MSG$(18) = "Checking the DRIVE format"
    STATUS.TEXT.MSG$(19) = "C drive is not in FAT32 format"
    STATUS.TEXT.MSG$(20) = "D drive is not in FAT32 format"
    STATUS.TEXT.MSG$(21) = "Initializing the display manager"
    STATUS.TEXT.MSG$(22) = "Checking the Backup script file"
    STATUS.TEXT.MSG$(23) = "'BKPSCRPT array index error"
    STATUS.TEXT.MSG$(24) = " day is selected"
    STATUS.TEXT.MSG$(25) = "C Drive restoration process started"
    STATUS.TEXT.MSG$(26) = "D Drive restoration process started"
    STATUS.TEXT.MSG$(27) = ""           ! duplicate                     !JDC
    STATUS.TEXT.MSG$(28) = "Restore completed successfully"
    STATUS.TEXT.MSG$(29) = " restoration "
    STATUS.TEXT.MSG$(30) = " directory is created"
    STATUS.TEXT.MSG$(31) = " dated "
    STATUS.TEXT.MSG$(32) = "Checking BKPFAIL "
    STATUS.TEXT.MSG$(33) = ""           ! duplicate                     !JDC
    STATUS.TEXT.MSG$(34) = ""           ! duplicate                     !JDC
    STATUS.TEXT.MSG$(35) = " is extracted to "
    STATUS.TEXT.MSG$(36) = "Checking "
    STATUS.TEXT.MSG$(37) = " to check the"
    STATUS.TEXT.MSG$(38) = ""           ! duplicate                     !JDC
    STATUS.TEXT.MSG$(39) = " file is entered "
    STATUS.TEXT.MSG$(40) = ""           ! duplicate                     !JDC
    STATUS.TEXT.MSG$(41) = "File doesn't exist in the selected day "
    STATUS.TEXT.MSG$(42) = "Checking the archive file "
    STATUS.TEXT.MSG$(43) = " directory is selected "
    STATUS.TEXT.MSG$(44) = " directory is created"
    STATUS.TEXT.MSG$(45) = " directory is created"
    STATUS.TEXT.MSG$(46) = " is extracted "                             !OJK
    STATUS.TEXT.MSG$(47) = "File is being extracted from BKPFAIL file"
    STATUS.TEXT.MSG$(48) = "File is extracted to "
    STATUS.TEXT.MSG$(49) = "Copying and Restoring the file"
    STATUS.TEXT.MSG$(50) = "File restoration successful"
    STATUS.TEXT.MSG$(51) = "File restored with an exception"
    STATUS.TEXT.MSG$(52) = "Ended: Error while opening "
    STATUS.TEXT.MSG$(53) = "RESTORE not allowed when XBACKUP is active"
    STATUS.TEXT.MSG$(54) = "XRESTORE is already active somewhere"
    STATUS.TEXT.MSG$(55) = "archive file"
    STATUS.TEXT.MSG$(56) = \                                            !JDC
          "'File doesn't exist in the selected archive file."           !JDC
    STATUS.TEXT.MSG$(57) = \                                            !JDC
          "'File doesn't exist in the selected date archive file. "     !JDC
    STATUS.TEXT.MSG$(58) = "Press F3 or ESC"
    STATUS.TEXT.MSG$(59) = \                                            !JDC
          "'Checking the restored file - Please wait..."                !JDC
    STATUS.TEXT.MSG$(60) = "FALSE"
    STATUS.TEXT.MSG$(61) = "TRUE"
    STATUS.TEXT.MSG$(62) = \                                            !JDC
          "All directories and files have been restored to the "        !JDC
    STATUS.TEXT.MSG$(63) = " for restore"                               !JDC

    !******************************************************************
    ! Error messages
    !******************************************************************
    STATUS.TEXT.ERROR$(1)  = "UPDATE date function error in "
    STATUS.TEXT.ERROR$(2)  = "Error in opening DIR output file"
    STATUS.TEXT.ERROR$(3)  = "Error in reading DIR header output file"
    STATUS.TEXT.ERROR$(4)  = "'MDD array index error"
    STATUS.TEXT.ERROR$(5)  = "'BKP array index error"
    STATUS.TEXT.ERROR$(6)  = "PSDATE function error"
    STATUS.TEXT.ERROR$(7)  = "'MDD array index error"
    STATUS.TEXT.ERROR$(8)  = "'MDD array index error"
    STATUS.TEXT.ERROR$(9)  = "Error in opening BKPLIST file"
    STATUS.TEXT.ERROR$(10) = ""                 ! duplication           !JDC
    STATUS.TEXT.ERROR$(11) = ""                 ! duplication           !JDC
    STATUS.TEXT.ERROR$(12) = "Error found in extracting files"
    STATUS.TEXT.ERROR$(13) = "Error in opening BKPSCRPT file"
    STATUS.TEXT.ERROR$(14) = "Error when copying/distributing file"
    STATUS.TEXT.ERROR$(15) = "Error while extracting files"             !OJK
    STATUS.TEXT.ERROR$(16) = "Error while distributing the file"        !OJK
    STATUS.TEXT.ERROR$(17) = "Error in opening BKPFAIL file"
    STATUS.TEXT.ERROR$(18) = "Error in opening BKPLIST file"
    STATUS.TEXT.ERROR$(19) = "Error while creating directory"
    STATUS.TEXT.ERROR$(20) = "'Invalid key pressed"
    STATUS.TEXT.ERROR$(21) = "'No Backups available. "                  !MJK
    STATUS.TEXT.ERROR$(22) = "Error in opening Sleeper file"            !JDC

    !****************************************************************** !JDC
    ! Screen messages                                                   !JDC
    !****************************************************************** !JDC
    SCREEN.TEXT.MSG$(1)  = "RESTORE A DIRECTORY DAY SELECTION"          !JDC
    SCREEN.TEXT.MSG$(2)  = "Place an X in the box next to the day to "  !JDC
    SCREEN.TEXT.MSG$(3)  = "restore, then press ENTER"                  !JDC
    SCREEN.TEXT.MSG$(4)  = "You have chosen to restore the "            !JDC
    SCREEN.TEXT.MSG$(5)  = " directory."                                !JDC
    SCREEN.TEXT.MSG$(6)  = \                                            !JDC
          "Please choose which day you would like to restore:"          !JDC
    SCREEN.TEXT.MSG$(7)  = "'Error in Backup details retrieval."        !JDC
    SCREEN.TEXT.MSG$(8)  = " Refer XRESTORE log"                        !JDC
    SCREEN.TEXT.MSG$(9)  = "RESTORE A DIRECTORY STATUS"                 !JDC
    SCREEN.TEXT.MSG$(10) = " directory restoration in process - "       !JDC
    SCREEN.TEXT.MSG$(11) = " Directory of "                             !JDC
    SCREEN.TEXT.MSG$(12) = " date "                                     !JDC
    SCREEN.TEXT.MSG$(13) = "'Press ESC to go back to "                  !MJK
    SCREEN.TEXT.MSG$(14) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(15) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(16) = " are being extracted"                       !JDC
    SCREEN.TEXT.MSG$(17) = "RESTORE A DRIVE OR DRIVES"                  !JDC
    SCREEN.TEXT.MSG$(18) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(19) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(20) = "Please choose one of the following days:"   !JDC
    SCREEN.TEXT.MSG$(21) = "'No selection has been made. " + \          !JDC
                           "Value entered must be ""X"" OR ""x"" "      !JDC
    SCREEN.TEXT.MSG$(22) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(23) = "D: drive"                                   !JDC
    SCREEN.TEXT.MSG$(24) = "C: drive"                                   !JDC
    SCREEN.TEXT.MSG$(25) = "C: and D: drives"                           !JDC
    SCREEN.TEXT.MSG$(26) = "'Restore completed with exception." + \     !JDC
                           " Check XRESTORE log for more details"       !OJK
    SCREEN.TEXT.MSG$(27) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(28) = ""           ! duplicate                     !JDC
    SCREEN.TEXT.MSG$(29) = "previous screen"                            !JDC
    SCREEN.TEXT.MSG$(30) = "'No directories available to restore." + \  !MJK
                           " Press any key to exit screen"              !MJK
    SCREEN.TEXT.MSG$(31) = "'Data will be lost"                         !JDC

RETURN
! IDC END BLOCK

\***********************************************************************!MJK
\*                                                                      !MJK
\* SET.FILE.NAMES: This Sub-routine initialize the file names based     !MJK
\*                 on its presence                                      !MJK
\*                                                                      !MJK
\***********************************************************************!MJK
SET.FILE.NAMES:                                                         !MJK

    ADXZUDIR.FILE.NAME$ = "C:\ADX_SPGM\ADXZUDIR.386" ! ADXZUDIR         !MJK
    ADXUNZIP.FILE.NAME$ = "C:\ADX_SPGM\ADXUNZIP.386" ! ADXUNZIP         !OJK
    ADXCSU0L.FILE.NAME$ = "C:\ADX_SPGM\ADXCSU0L.286" ! ADXCSU0L         !MJK

    IF NOT FUNC.FILE.EXISTS(ADXZUDIR.FILE.NAME$) THEN BEGIN             !MJK
        ADXZUDIR.FILE.NAME$ = "D:\ADX_SPGM\ADXZUDIR.386" ! ADXZUDIR     !MJK
    ENDIF                                                               !MJK

    IF NOT FUNC.FILE.EXISTS(ADXUNZIP.FILE.NAME$) THEN BEGIN             !OJK
        ADXUNZIP.FILE.NAME$ = "D:\ADX_SPGM\ADXUNZIP.386" ! ADXUNZIP     !OJK
    ENDIF                                                               !OJK

    IF NOT FUNC.FILE.EXISTS(ADXCSU0L.FILE.NAME$) THEN BEGIN             !MJK
        ADXCSU0L.FILE.NAME$ = "D:\ADX_SPGM\ADXCSU0L.286" ! ADXCSU0L     !MJK
    ENDIF                                                               !MJK

    ! Set local drive for sleeper in supps mode                         !MJK
    SUPPS.SLEEPER.FILE.NAME$ = "D:\ADX_UDT1\SLPCF.BIN"                  !MJK

    IF NOT FUNC.FILE.EXISTS(SUPPS.SLEEPER.FILE.NAME$) THEN BEGIN        !MJK
        SUPPS.SLEEPER.FILE.NAME$  = "C:\ADX_UDT1\SLPCF.BIN"             !MJK
    ENDIF                                                               !MJK

    ! Set full path for sleeper in normal runs                          !MJK
    NORMAL.SLEEPER.FILE.NAME$ = "ADXLXACN::" + SUPPS.SLEEPER.FILE.NAME$ !MJK
                                                                        !MJK
    BKPLIST.PREFIX.D.DRIVE$ = "D:\ADX_UDT1\" + "BKPLIST."               !MJK
    BKPLIST.PREFIX.C.DRIVE$ = "C:\ADX_UDT1\" + "BKPLIST."               !MJK

RETURN                                                                  !MJK

\***********************************************************************
\*
\* ALLOCATE.SESSION.NUMBERS: This Sub-routine uses variables of FILE
\*                           functions and calls SESS.NUM.UTILITY to
\*                           allocate session numbers.
\*
\***********************************************************************
ALLOCATE.SESSION.NUMBERS:

    FUNC.FLAG$ = "O"   ! Setting the file operation

    ! XBACKUP pipe
    PASSED.INTEGER% = XBACK.PIPE.REPORT.NUM%
    PASSED.STRING$  = XBACK.PIPE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !IDC
    XBACK.PIPE.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! XRESTORE pipe
    PASSED.INTEGER% = XRE.PIPE.REPORT.NUM%
    PASSED.STRING$  = XRE.PIPE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !IDC
    XRE.PIPE.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! XRESTORE log
    PASSED.INTEGER% = XRE.LOG.REPORT.NUM%
    PASSED.STRING$  = XRE.LOG.FILENAME$
    GOSUB GET.SESSION.NUMBER                                            !IDC
    XRE.LOG.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! Sleeper Control file                                              !IDC
    PASSED.INTEGER% = SLPCF.REPORT.NUM%                                 !IDC
    PASSED.STRING$  = NORMAL.SLEEPER.FILE.NAME$                         !JDC
    GOSUB GET.SESSION.NUMBER                                            !IDC
    SLPCF.SESS.NUM% = F20.INTEGER.FILE.NO%                              !IDC

RETURN

\***********************************************************************!MJK
\*                                                                      !MJK
\* CREATE.DIRECTORIES : This Sub-routine creates the C:/XDISKIMG        !MJK
\*                      and C:/TEMP directory if missing.               !MJK
\*                                                                      !MJK
\***********************************************************************!MJK
CREATE.DIRECTORIES:                                                     !MJK
                                                                        !MJK
    IF FUNC.DIR.NOT.EXISTS(C.BKP.IMG$) THEN BEGIN                       !MJK
        CALL RTRIMC (C.BKP.IMG$, ASC("/"))                              !MJK
        CALL RTRIMC (C.BKP.IMG$, ASC("\\"))                             !MJK
        CALL OSSHELL("MKDIR " + C.BKP.IMG$ + " > " + "XRE.TMP" + \      !MJK
                     " >>* " + "XRE.TMP")                               !MJK
    ENDIF                                                               !MJK
                                                                        !MJK
    IF FUNC.DIR.NOT.EXISTS(TEMP.DIR$) THEN BEGIN                        !MJK
        CALL RTRIMC (TEMP.DIR$, ASC("/"))                               !MJK
        CALL RTRIMC (TEMP.DIR$, ASC("\\"))                              !MJK
        CALL OSSHELL("MKDIR " + TEMP.DIR$ + " > " + "XRE.TMP" + \       !MJK
                     " >>* " + "XRE.TMP")                               !MJK
    ENDIF                                                               !MJK
                                                                        !MJK
RETURN                                                                  !MJK

\***********************************************************************
\*
\* GET.SESSION.NUMBER: This Sub-routine calls SESS.NUM.UTILITY to
\*                     allocate session numbers.
\*
\***********************************************************************
GET.SESSION.NUMBER:

    FUN.RC2% = SESS.NUM.UTILITY(FUNC.FLAG$, PASSED.INTEGER%, \          !IDC
                                PASSED.STRING$)                         !MJK
    GOSUB CHECK.FUN.RC2                                                 !IDC

RETURN

\***********************************************************************
\*
\* CREATE.XRESTORE.LOG : This Sub-routine creates the XRESTORE log.
\*
\***********************************************************************
CREATE.XRESTORE.LOG:

    CREATE XRE.LOG.FILENAME$ AS XRE.LOG.SESS.NUM%
    XRE.LOG.OPEN = TRUE                                                 !MJK

XRE.LOG.CREATE.ERROR:                                                   !MJK

    !---------------------------------------------!
    ! CLOSE session and deallocate the number as  !
    ! file write will be done by C File functions !
    !---------------------------------------------!
    IF XRE.LOG.OPEN THEN BEGIN
        XRE.LOG.OPEN = FALSE                                            !JDC
        CLOSE  XRE.LOG.SESS.NUM%
        CALL SESS.NUM.UTILITY ("C",XRE.LOG.SESS.NUM%,XRE.NULL$)         !MJK
    ENDIF

    XRE.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( XRE.LOG.FILENAME$ )       !NJK
    ! if return is not a failure                                        !JDC
    IF XRE.FILE.RC% > XRE.ZERO% THEN BEGIN
        XRE.LOG.OPEN = TRUE
    ENDIF

    STATUS.MSG$ = STATUS.TEXT.MSG$(12)                                  !JDC
    GOSUB LOG.STATUS.MSG

RETURN

\***********************************************************************
\*
\* CHECK.XBACKUP.RUN : This Sub-routine stops the program if the        !MJK
\*                     XBACKUP program is running
\*
\***********************************************************************
CHECK.XBACKUP.RUN:

    CREATE XBACK.PIPE.NAME$ AS XBACK.PIPE.SESS.NUM% BUFFSIZE XRE.ZERO%
    XBACK.OPEN = TRUE                                                   !MJK

XBACK.PIPE.CREATE.ERROR:                                                !MJK

    ! Closing the session only if the pipe creation is successful
    IF XBACK.OPEN THEN BEGIN                                            !MJK
        CLOSE XBACK.PIPE.SESS.NUM%
        XBACK.OPEN = FALSE                                              !JDC
        CALL SESS.NUM.UTILITY ("C",XBACK.PIPE.SESS.NUM%,XRE.NULL$)      !MJK
    ENDIF

RETURN

\***********************************************************************
\*
\* CREATE.RUN.PIPE: This Sub-routine creates pipe for current module
\*                  to avoid any duplicate run.
\*
\***********************************************************************
CREATE.RUN.PIPE:

    CREATE XRE.PIPE.NAME$ AS XRE.PIPE.SESS.NUM% BUFFSIZE XRE.ZERO%
    XRE.OPEN = TRUE                                                     !MJK

XRE.PIPE.CREATE.ERROR:                                                  !MJK

RETURN

\***********************************************************************
\*
\* CONTROLLER.CONFIG.CHECK: This Sub-routine uses ADXSERVE function
\*                          and check whether the controller is
\*                          Master and File server.If not, program
\*                          should end, logging an appropriate error.
\*
\***********************************************************************
CONTROLLER.CONFIG.CHECK:

    STATUS.MSG$ = STATUS.TEXT.MSG$(13)                                  !JDC
    GOSUB LOG.STATUS.MSG

    ! Using Function 4 to get the Controller details
    ADX.FUNCTION% = 4              ! Adxserve function 4
    ADXSERVE.RC%  = 1              ! Initiate with non zero value

    CALL ADXSERVE (ADXSERVE.RC%,ADX.FUNCTION%,XRE.ZERO%,ADX.PARM.2$)

    IF ADXSERVE.RC% <> XRE.ZERO% THEN BEGIN    ! If return code non zero
        STATUS.MSG$ = STATUS.TEXT.MSG$(13)                              !JDC
        ERROR.MSG$  = STATUS.MSG$
        GOSUB LOG.STATUS.MSG
        ERROR.EXIST = TRUE
    ENDIF

    CNTLR.CONFIG% = VAL(MID$(ADX.PARM.2$, 25, 2)) ! Controller config
    CNTLR.ID$     = MID$(ADX.PARM.2$, 14, 2)      ! Master Controller ID

    !-------------------------------------------!
    ! If controller is not a Master/File server !
    ! OR not in SUPPS mode                      !
    !-------------------------------------------!

    ! Set the flag if controller is in SUPPS mode
    IF CNTLR.ID$ = "CC" THEN BEGIN
        SUPPS.ON = TRUE
    ENDIF

    IF NOT (CNTLR.CONFIG% = MASTER.AND.FILE.SERVER% OR                 \
            CNTLR.ID$     = "CC" ) THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(15) + "to run this program"      !JDC
        ERROR.MSG$  = STATUS.MSG$
        GOSUB LOG.STATUS.MSG
        ERROR.EXIST = TRUE
    ENDIF

RETURN

\***********************************************************************!KDC
\*                                                                      !IDC
\*    GET.SLEEPER.CONFIGURATION:This Sub-routine looks through the      !IDC
\*                              Sleeper control file in order to see    !IDC
\*                              what the current setting for the Full   !IDC
\*                              backup is. It then assumes all are      !IDC
\*                              Incremental based upon this day         !IDC
\*                                                                      !IDC
\***********************************************************************!IDC
GET.SLEEPER.CONFIGURATION:                                              !IDC

    SLEEPER.DAY% = 0                                                    !JDC

    IF SUPPS.ON THEN BEGIN                                              !JDC

        DIR.FILE.RC% = \                                                !NJK
                FUNC.OPEN.SEQUENTIAL.FILE(SUPPS.SLEEPER.FILE.NAME$)     !NJK

        ! If file open unsuccessful                                     !JDC
        IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN                         !JDC
            SLPCF.OPEN  = FALSE                                         !MJK
            STATUS.MSG$ = STATUS.TEXT.MSG$(22)                          !JDC
            GOSUB LOG.STATUS.MSG                                        !JDC
        ENDIF ELSE BEGIN                                                !JDC
            SLPCF.OPEN  = TRUE                                          !MJK
        ENDIF

        SLEEPER.RECORD$ = "X"                                           !JDC
        ! Read the file till EOF/read error                             !JDC
        WHILE LEN(SLEEPER.RECORD$) <> XRE.ZERO%                         !JDC
            SLEEPER.RECORD$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% ) !NJK
            IF LEN(SLEEPER.RECORD$) <> XRE.ZERO% THEN BEGIN             !JDC
                GOSUB GET.SUPPS.SLEEPER.INFO                            !JDC
            ENDIF                                                       !JDC
        WEND                                                            !JDC

        ! Closing File                                                  !JDC
        IF SLPCF.OPEN THEN BEGIN                                        !JDC
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !NJK
            SLPCF.OPEN = FALSE                                          !JDC
        ENDIF                                                           !JDC

    ENDIF ELSE BEGIN                                                    !JDC

        !***************************************************************!JDC
        ! first find the full backup day configured in sleeper          !JDC
        !***************************************************************!JDC
        IF END #SLPCF.SESS.NUM% THEN SLPCF.NOT.FOUND

        OPEN NORMAL.SLEEPER.FILE.NAME$ DIRECT RECL SLPCF.RECL% \        !JDC
             AS SLPCF.SESS.NUM% NOWRITE NODEL

        SLPCF.OPEN             = TRUE
        SLPCF.REC.NO%          = 0
        FULL.BACKUP.NOT.FOUND% = 0

        WHILE FULL.BACKUP.NOT.FOUND% = 0
            SLPCF.REC.NO% = SLPCF.REC.NO% + 1
            IF READ.SLPCF = 0 THEN BEGIN
                IF SLPCF.APP.NAME$ = "ADX_UPGM:XBACKUP.286" THEN BEGIN
                    IF SLPCF.PARM$ = PARM.FULL$ THEN BEGIN
                        FULL.BACKUP.NOT.FOUND% = 1          !found
                        !extract single day for full build
                        SLEEPER.DAY% = VAL(SLPCF.DAY.NUM$)
                        STATUS.MSG$  = STATUS.TEXT.MSG$(16) + \         !MJK
                                       SLPCF.DAY.NUM$                   !MJK
                    ENDIF
                ENDIF
            ENDIF
        WEND

SLPCF.NOT.FOUND:                                                        !JDC
        IF SLPCF.OPEN THEN BEGIN                                        !IDC
            CLOSE SLPCF.SESS.NUM%                                       !IDC
            SLPCF.OPEN = FALSE                                          !JDC
            CALL SESS.NUM.UTILITY ("C",SLPCF.SESS.NUM%,XRE.NULL$)       !MJK
        ENDIF ELSE BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(22)                        !JDC
            GOSUB LOG.STATUS.MSG                                        !JDC
        ENDIF                                                           !IDC

    ENDIF                                                               !JDC

    ! needed so end if not found                                        !JDC
    IF SLEEPER.DAY% = 0 THEN BEGIN                                      !JDC
        STATUS.MSG$ = STATUS.TEXT.MSG$(17)                              !JDC
        GOSUB LOG.STATUS.MSG                                            !JDC
        GOSUB STOP.PROGRAM                                              !JDC
    ENDIF ELSE BEGIN                                                    !JDC
        GOSUB LOG.STATUS.MSG                                            !JDC
    ENDIF                                                               !JDC

    !*******************************************************************!IDC
    !then set the data we will build from - note that the array elements!IDC
    !must match the sleeper settings (ie 1=Sunday) to work correctly    !IDC
    !*******************************************************************!IDC
    DIM CONSTANT.DAY$(7)                                                !IDC
    CONSTANT.DAY$(1) = CONSTANT.SUNDAY.SHORT$                           !IDC
    CONSTANT.DAY$(2) = CONSTANT.MONDAY.SHORT$                           !IDC
    CONSTANT.DAY$(3) = CONSTANT.TUESDAY.SHORT$                          !IDC
    CONSTANT.DAY$(4) = CONSTANT.WEDNESDAY.SHORT$                        !IDC
    CONSTANT.DAY$(5) = CONSTANT.THURSDAY.SHORT$                         !IDC
    CONSTANT.DAY$(6) = CONSTANT.FRIDAY.SHORT$                           !IDC
    CONSTANT.DAY$(7) = CONSTANT.SATURDAY.SHORT$                         !IDC

    !*******************************************************************!IDC
    !and now the same for the longer names we display so all match, so  !IDC
    !we make them all the same length for our matching process later    !IDC
    !*******************************************************************!IDC
    DIM CONSTANT.LONG.DAY$(7)
    CONSTANT.LONG.DAY$(1) = LEFT$(CONSTANT.SUNDAY.LONG$              + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(2) = LEFT$(CONSTANT.MONDAY.LONG$              + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(3) = LEFT$(CONSTANT.TUESDAY.LONG$             + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(4) = LEFT$(CONSTANT.WEDNESDAY.LONG$           + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(5) = LEFT$(CONSTANT.THURSDAY.LONG$            + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(6) = LEFT$(CONSTANT.FRIDAY.LONG$              + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    CONSTANT.LONG.DAY$(7) = LEFT$(CONSTANT.SATURDAY.LONG$            + \!MJK
                            STRING$(CONSTANT.LONGEST.DAY%, XRE.SPACE$) \!MJK
                            , CONSTANT.LONGEST.DAY%)

    !*******************************************************************!IDC
    !Build up the string with all the days, starting with the full      !IDC
    !backup day so it can be matched to any time to find the Full and   !IDC
    !Incremental days adding the : prevents mismatching characters that !IDC
    !might overlap on the short day name. for example if Sunday is the  !IDC
    !full configured day the string would be;                           !IDC
    !  ":SUN:MON:TUE:WED:THU:FRI:SAT"                                   !IDC
    !and the longer names would be;                                     !IDC
    !  "Monday   Tuesday  WednesdayThursday Friday   Saturday Sunday   "!IDC
    !*******************************************************************!IDC
    BACKUP.DAYS$      = CONSTANT.DAY$(SLEEPER.DAY%)                     !IDC
    BACKUP.LONG.DAYS$ = CONSTANT.LONG.DAY$(SLEEPER.DAY%)                !IDC
                                                                        !IDC
    FOR DAY.SINCE.FULL% = 2 TO 7                                        !IDC
                                                                        !IDC
        IF SLEEPER.DAY% = 7 THEN BEGIN              !if Saturday        !IDC
            SLEEPER.DAY% = 1                        !set to Sunday      !IDC
        ENDIF ELSE BEGIN                                                !IDC
            SLEEPER.DAY% = SLEEPER.DAY% + 1         !next day           !IDC
        ENDIF                                                           !IDC
                                                                        !IDC
        BACKUP.DAYS$      = BACKUP.DAYS$ + CONSTANT.COLON$ + \          !IDC
                            CONSTANT.DAY$(SLEEPER.DAY%)                 !IDC
                                                                        !IDC
        BACKUP.LONG.DAYS$ = BACKUP.LONG.DAYS$ + \                       !IDC
                            CONSTANT.LONG.DAY$(SLEEPER.DAY%)            !IDC
    NEXT DAY.SINCE.FULL%                                                !IDC
                                                                        !IDC
    DIM CONSTANT.DAY$(0)                                                !IDC
    DIM CONSTANT.LONG.DAY$(0)                                           !IDC
                                                                        !IDC
RETURN                                                                  !IDC

\***********************************************************************!JDC
\*                                                                      !JDC
\* GET.SUPPS.SLEEPER.INFO : This Sub-routine extracts the record from   !JDC
\*                          the sleeper file in supps mode so must use  !JDC
\*                          the whole record and extract each element   !JDC
\*                          looking for the Full run setting            !JDC
\*                                                                      !JDC
\***********************************************************************!JDC
GET.SUPPS.SLEEPER.INFO:                                                 !JDC

    IF LEFT$(SLEEPER.RECORD$, 20) = "ADX_UPGM:XBACKUP.286" THEN BEGIN   !MJK
        ! Checking whether the 1st letter of the filler                 !JDC
        ! variable is numeric                                           !JDC
        IF MATCH("#",MID$(SLEEPER.RECORD$, 69, 1) ,1) <> 0 THEN BEGIN   !JDC
            SLPCF.PARM.LEN% = VAL(MID$(SLEEPER.RECORD$, 69, 1))         !JDC
            ! If SLPCF Filler parameter length is greater than zero     !JDC
            IF SLPCF.PARM.LEN% > 0 THEN BEGIN                           !JDC
                ! Storing the SLPCF Filler parameter                    !JDC
                SLPCF.PARM$ = MID$(SLEEPER.RECORD$, 70, SLPCF.PARM.LEN%)!MJK
                IF SLPCF.PARM$ = PARM.FULL$ THEN BEGIN                  !JDC
                    !extract single day for full build                  !JDC
                    SLPCF.DAY.NUM$ = MID$(SLEEPER.RECORD$, 23, 6)       !JDC
                    SLEEPER.DAY%   = VAL(SLPCF.DAY.NUM$)                !MJK

                    STATUS.MSG$ = STATUS.TEXT.MSG$(16) + SLPCF.DAY.NUM$ !JDC

                    SLEEPER.RECORD$ = ""          !found                !JDC
                ENDIF                                                   !JDC
            ENDIF                                                       !JDC
        ENDIF                                                           !JDC
    ENDIF                                                               !JDC
                                                                        !JDC
RETURN                                                                  !JDC

\***********************************************************************
\*
\* DRIVE.FAT32.CHECK : This Sub-routine checks the format of C and D
\*                     drive to make sure it is in FAT32 format.
\*
\***********************************************************************
DRIVE.FAT32.CHECK:

    STATUS.MSG$ = STATUS.TEXT.MSG$(18)                                  !JDC
    GOSUB LOG.STATUS.MSG

    ! Storing the random DIR output and check C32 text in 2nd line
    CALL OSSHELL("DIR C:/DRIVE.CHK > " + DIR.OUT$ )                     !MJK

    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )                !NJK

    IF DIR.FILE.RC% > XRE.ZERO% THEN BEGIN
        DIR.OPEN   = TRUE
        !ignore return check for EOF or read error as will error in     !FDC
        !FAT32 check and fail correctly                                 !FDC
        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK
        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK
    ENDIF ELSE BEGIN
        DIR.OPEN    = FALSE                                             !MJK
        STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                             !JDC
        GOSUB LOG.STATUS.MSG                                            !IDC
    ENDIF

    ! C drive FAT32 check
    IF RIGHT$(DIR.VALUE$,3) <> "C32" THEN BEGIN
        ERROR.EXIST = TRUE
        STATUS.MSG$ = STATUS.TEXT.MSG$(19)                              !JDC
        ERROR.MSG$  = STATUS.MSG$
        GOSUB LOG.STATUS.MSG
    ENDIF

    ! Closing File
    IF DIR.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                            !NJK
        DIR.OPEN = FALSE
    ENDIF

    ! Storing the random DIR output and the check D32 text in 2nd line
    CALL OSSHELL("DIR D:/DRIVE.CHK > " + DIR.OUT$ )                     !MJK

    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )                !NJK

    IF DIR.FILE.RC% > XRE.ZERO% THEN BEGIN
        DIR.OPEN   = TRUE
        !ignore return check for EOF or read error as will error in     !FDC
        !FAT32 check and fail correctly                                 !FDC
        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK
        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK
    ENDIF ELSE BEGIN
        DIR.OPEN    = FALSE                                             !MJK
        STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                             !JDC
        GOSUB LOG.STATUS.MSG
    ENDIF

    ! D drive FAT32 check
    IF RIGHT$(DIR.VALUE$,3) <> "D32" THEN BEGIN
        ERROR.EXIST = TRUE
        STATUS.MSG$ = STATUS.TEXT.MSG$(20)                              !MJK
        ERROR.MSG$  = STATUS.MSG$
        GOSUB LOG.STATUS.MSG
    ENDIF

    ! Closing File
    IF DIR.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                            !NJK
        DIR.OPEN = FALSE
    ENDIF

RETURN

\***********************************************************************
\*
\* INITIATE.DISP.MNGR : This Sub-routine initiates the display manager.
\*
\***********************************************************************
INITIATE.DISP.MNGR:

    STATUS.MSG$ = STATUS.TEXT.MSG$(21)                                  !JDC
    GOSUB LOG.STATUS.MSG

    CALL DM.INIT                      ! Display Manager Initialisation

RETURN

                !   INITIALISATION SPECIFIC ROUTINES ENDS   !
                !...........................................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                 MAIN.PROCESSING SPECIFIC ROUTINES                  *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************
\*
\* PROCESS.BKPSCRPT: This Subroutine process the BKPSCRPT file and store
\*                   all the BACKUP and EXCLUDE details in an array.
\*
\***********************************************************************
PROCESS.BKPSCRPT:

    STATUS.MSG$ = STATUS.TEXT.MSG$(22)                                  !JDC
    GOSUB LOG.STATUS.MSG

    BKPSCRPT.FILE.NAME$ = "D:/ADX_UDT1/BKPSCRPT.TXT"    ! Backup script

    IF NOT FUNC.FILE.EXISTS(BKPSCRPT.FILE.NAME$) THEN BEGIN             !MJK
        BKPSCRPT.FILE.NAME$ = "C:/ADX_UDT1/BKPSCRPT.TXT"                !MJK
    ENDIF                                                               !MJK

    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE(BKPSCRPT.FILE.NAME$)       !NJK

    ! If the open successful, set the VALUE.EXISTS equals TRUE
    IF DIR.FILE.RC% > XRE.ZERO% THEN BEGIN
        DIR.OPEN     = TRUE
        VALUE.EXISTS = TRUE
    ENDIF ELSE BEGIN
        VALUE.EXISTS = FALSE
        DIR.OPEN     = FALSE                                            !CJK
        STATUS.MSG$  = "Error in opening BKPSCRPT file"
        GOSUB LOG.STATUS.MSG
    ENDIF

    BKPSCRPT.INDEX% = XRE.ZERO%

    !-----------------------------------------------------------------!
    ! Extracting EXCLUDE and BACKUP and performing respective process !
    !-----------------------------------------------------------------!
    WHILE VALUE.EXISTS

        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK

        COMMA.POSITION% = MATCH(COMMA.VALUE$,DIR.VALUE$,1)

        ! If comma found
        IF COMMA.POSITION% <> XRE.ZERO% THEN BEGIN

            ! Extracting Backup script command
            BKPSCRPT.COMMAND$ = LEFT$(DIR.VALUE$,(COMMA.POSITION% - 1)) !CJK
            CALL TRIM(BKPSCRPT.COMMAND$)

            ! Remaining Backup script value
            DIR.VALUE$ = MID$(DIR.VALUE$,(COMMA.POSITION% + 1),        \!CJK
                              LEN(DIR.VALUE$))
            !-----------------------!
            ! If command is Backup  !
            !-----------------------!
            IF BKPSCRPT.COMMAND$ = "BACKUP" THEN BEGIN

                BKPSCRPT.INDEX% = BKPSCRPT.INDEX% + 1

                IF BKPSCRPT.INDEX% > ARRAY.LIMIT% THEN BEGIN            !MJK
                    STATUS.MSG$ = STATUS.TEXT.MSG$(23)                  !JDC
                    GOSUB PROGRAM.EXIT
                ENDIF

                !------------------------------------------!
                ! Extracting Backup script directory value !
                !------------------------------------------!

                ! Comma position
                COMMA.POSITION%      = MATCH(COMMA.VALUE$,DIR.VALUE$,1) !CJK
                ! Backup directory
                BKPSCRPT.DIRECTORY$  = LEFT$(DIR.VALUE$,               \
                                             (COMMA.POSITION% - 1))

                CALL TRIM(BKPSCRPT.DIRECTORY$)

                BKPSCRPT.DIRECTORIES$(BKPSCRPT.INDEX%) =               \
                                                BKPSCRPT.DIRECTORY$

                ! Remaining Backup script value
                DIR.VALUE$ = MID$(DIR.VALUE$,(COMMA.POSITION% + 1),    \!CJK
                                  LEN(DIR.VALUE$))

                !-------------------------------------------------!
                ! Extracting Backup script output directory value !
                !-------------------------------------------------!

                ! Comma position
                COMMA.POSITION% = MATCH(COMMA.VALUE$,DIR.VALUE$,1)      !CJK

                ! Output directory
                BKPSCRPT.OUT.FILE.NAME$ = LEFT$(DIR.VALUE$,            \!CJK
                                               (COMMA.POSITION% - 1))
                CALL TRIM(BKPSCRPT.OUT.FILE.NAME$)

                PRIMARY.ARCHIVED.NAMES$(BKPSCRPT.INDEX%) =             \
                                            BKPSCRPT.OUT.FILE.NAME$

                ! Remaining Backup script value
                DIR.VALUE$ = MID$(DIR.VALUE$,(COMMA.POSITION% + 1),    \!CJK
                                  LEN(DIR.VALUE$))

                BKPSCRPT.OUT.FILE.NAME$ = DIR.VALUE$
                CALL TRIM(BKPSCRPT.OUT.FILE.NAME$)

                SECONDARY.ARCHVD.NAMES$(BKPSCRPT.INDEX%) =             \
                                            BKPSCRPT.OUT.FILE.NAME$

            ENDIF
        ENDIF

        ! If the read reaches EOF
        IF LEN(DIR.VALUE$) = XRE.ZERO% THEN BEGIN
            VALUE.EXISTS = FALSE
        ENDIF
    WEND

    ! Closing File
    IF DIR.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                            !NJK
        DIR.OPEN = FALSE
    ENDIF

RETURN

\***********************************************************************
\*
\* SCREEN.NAVIGATION: This Subroutine navigates the screen based on
\*                    the user input.
\*
\***********************************************************************
SCREEN.NAVIGATION:

    ! Based on screen selection, display and process the screen
    WHILE SCREEN% <> XRE.ZERO%
        IF SCREEN% = DISPLAY.MAIN.SCR% THEN BEGIN                       !CJK

            ! Display Main Menu
            GOSUB DISPLAY.MAIN.SCREEN

            ! Process Main Menu
            GOSUB PROCESS.MAIN.SCREEN

        ENDIF ELSE BEGIN
            IF SCREEN% = DRIVE.DISK.SELECT.SCR% THEN BEGIN              !MJK

                ! Display and process Drive selection screen
                GOSUB DISPLAY.PROCESS.DRIVES                            !MJK

                ! Display and process Drive restore screen
                GOSUB PROCESS.DRIVE.DAY.SCREEN                          !MJK

            ENDIF ELSE BEGIN
                IF SCREEN% = RESTORE.A.DIRECTORY.SCR% THEN BEGIN        !CJK

                    ! Display directory screen                          !MJK
                    GOSUB DISPLAY.DIRECT.SCREEN

                    ! Process Directory screen
                    GOSUB PROCESS.DIRECT.SCREEN                         !MJK

                ENDIF ELSE BEGIN

                    ! Display file screen
                    CALL DM.SHOW.SCREEN (7, XRE.NULL$, 6, 6)

                    ! Process file screen
                    GOSUB PROCESS.FILE.SCREEN

                ENDIF
            ENDIF
        ENDIF
    WEND

    !-------------------------------------------------!
    ! If screen mode, chain back the program to PSB50 !
    ! else clears the screen and stop the program     !
    !-------------------------------------------------!
    IF SCREEN% = XRE.ZERO% THEN BEGIN                                   !CJK
        GOSUB CHAIN.TO.CALLER
    ENDIF

RETURN

\***********************************************************************
\*
\* DISPLAY.MAIN.SCREEN: This Subroutine displays the Main screen
\*                      for XRESTORE.
\*
\***********************************************************************
DISPLAY.MAIN.SCREEN:

    ! selection Input is stored in FSEL$
    !----------------------------------------
    FSEL$ = ""

    CALL DM.SHOW.SCREEN (1, XRE.NULL$, 1, 1)

    ! Initialising the output fields in the screen
    ! before processing the screen
    !--------------------------------------------
    CALL DM.NAME (2, "FSEL$", FSEL$)

    ! Validation the Menu selection is between 1 and 3
    !-------------------------------------------------
    CALL DM.VALID ("FSEL$", "FSEL$ >= 1 AND FSEL$ <= 3")
    !B003 Invalid selection number
    CALL DM.MESSAGE ("FSEL$", "'Invalid selection number")

RETURN

\***********************************************************************
\*
\* PROCESS.MAIN.SCREEN: This Subroutine processes the Main screen
\*                      for XRESTORE.
\*
\***********************************************************************
PROCESS.MAIN.SCREEN:

    ! Processing Main Menu screen
    !-------------------------------------
    WHILE SCREEN% = DISPLAY.MAIN.SCR%                                   !CJK

        RET.KEY% = DM.PROCESS.SCREEN (2, 2, XRE.ZERO%)

        IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN     !CJK

            ! Program exit
            SCREEN% = XRE.ZERO%

        ENDIF ELSE BEGIN

            ! If Enter key is pressed and no Error exist
            IF RET.KEY% = ENTER.KEY% AND NOT ERROR.EXIST THEN BEGIN

                ! Storing the option in a variable
                OPT.SEL% = VAL(FSEL$)

                IF OPT.SEL% = 1 THEN BEGIN

                    !------------------------------------------!
                    ! Before moving to Drive restore screen    !
                    ! need to make sure that controller is in  !
                    ! SUPPS mode                               !
                    !------------------------------------------!
                    IF NOT SUPPS.ON THEN BEGIN
                        CALL DM.FOCUS("11", "'This functionality "  + \
                                      "is only available under SUPPS")
                    ENDIF ELSE BEGIN
                    ! Drive restore screen
                        SCREEN% = DRIVE.DISK.SELECT.SCR%                !MJK
                    ENDIF

                ENDIF ELSE IF OPT.SEL% = 2 THEN BEGIN
                    ! Directory restore screen
                    SCREEN% = RESTORE.A.DIRECTORY.SCR%                  !CJK
                ENDIF ELSE BEGIN
                    ! File restore screen
                    SCREEN% = RESTORE.A.FILE.SCR%                       !CJK
                ENDIF
            ENDIF ELSE BEGIN                                            !NJK
                IF ERROR.EXIST THEN BEGIN                               !NJK
                    CALL DM.FOCUS("101", "'" + ERROR.MSG$)
                ENDIF ELSE BEGIN
                    ! B001 Invalid key pressed
                    CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))         !IDC
                ENDIF
            ENDIF                                                       !NJK
        ENDIF

    WEND

RETURN

! Sub-routine DISPLAY.PROCESS.DRIVES, CHECK.DRIVE.SELECTION has been    !MJK
! moved up with respect to the changes in the screen execution in CR5.  !MJK
\***********************************************************************
\*
\* DISPLAY.PROCESS.DRIVES: This Subroutine display and processes
\*                         the C and D drives.
\*
\***********************************************************************
DISPLAY.PROCESS.DRIVES:

    CALL DM.SHOW.SCREEN (3, XRE.NULL$, 3, 3)

    ! Processing Drive display screen
    WHILE SCREEN% = DRIVE.DISK.SELECT.SCR%                              !MJK

        RET.KEY% = DM.PROCESS.SCREEN (3, 4, TRUE)

        IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN     !CJK
            ! Return back to the Drive DAY screen
            SCREEN%      = DISPLAY.MAIN.SCR%                            !MJK

        ENDIF ELSE BEGIN
            IF RET.KEY% = ENTER.KEY% THEN BEGIN

                ! Checks the Drive selection input
                GOSUB CHECK.DRIVE.SELECTION

            ENDIF ELSE BEGIN
                ! B001 Invalid key pressed
                CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))             !IDC
            ENDIF
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* CHECK.DRIVE.SELECTION: This Subroutine checks the input of Drive
\*                        Display screen
\*
\***********************************************************************
CHECK.DRIVE.SELECTION:

    ! Storing the C drive and D drive field
    CDRIVE$ = UCASE$(FIELD$(3))
    DDRIVE$ = UCASE$(FIELD$(4))

    CALL TRIM(CDRIVE$)                                                  !MJK
    CALL TRIM(DDRIVE$)                                                  !MJK

    ! If at least one of the value is X
    IF (CDRIVE$ = "X" AND LEN(DDRIVE$) = XRE.ZERO%) OR \                !MJK
       (DDRIVE$ = "X" AND LEN(CDRIVE$) = XRE.ZERO%) THEN BEGIN          !MJK

        SCREEN% = DRIVE.DAY.SELECT.SCR%                                 !MJK

    ENDIF ELSE BEGIN                                                    !NJK
        IF LEN(DDRIVE$) <> XRE.ZERO% OR \                               !NJK
           LEN(CDRIVE$) <> XRE.ZERO% THEN BEGIN                         !MJK
            FIELD$(1) = "'Value entered must be ""X"" OR ""x"". Only" + \MJK
                        " one drive is allowed"                         !MJK
        ENDIF ELSE BEGIN
            FIELD$(1) = SCREEN.TEXT.MSG$(21)                            !JDC
        ENDIF
    ENDIF                                                               !NJK

RETURN

\***********************************************************************!MJK
\*                                                                      !MJK
\* PROCESS.DRIVE.DAY.SCREEN: This Subroutine process the drive restore  !MJK
\*                           screen for XRESTORE.                       !MJK
\*                                                                      !MJK
\***********************************************************************!MJK
PROCESS.DRIVE.DAY.SCREEN:                                               !MJK
                                                                        !MJK
    WHILE SCREEN% = DRIVE.DAY.SELECT.SCR%                               !MJK
                                                                        !MJK
        GOSUB DISPLAY.DRIVE.SCREEN                                      !MJK
        GOSUB PROCESS.DRIVE.SCREEN                                      !MJK
                                                                        !MJK
    WEND                                                                !MJK
                                                                        !MJK
RETURN                                                                  !MJK

\***********************************************************************
\*
\* DISPLAY.DRIVE.SCREEN: This Subroutine displays the drive restore
\*                       screen for XRESTORE.
\*
\***********************************************************************
DISPLAY.DRIVE.SCREEN:

    ! Setting the Header and other variables used for screen display
    SCR.HEADER$   = SCREEN.TEXT.MSG$(17)                                !MJK
    OPT.SELECTED$ = SCREEN.TEXT.MSG$(2) + SCREEN.TEXT.MSG$(3)           !JDC

    OPT.HEADER$   = SCREEN.TEXT.MSG$(20)                                !JDC

    IF CDRIVE$ = "X" THEN BEGIN                                         !MJK
        OPT.HEADER.1$ = "You have chosen to restore the C drive."       !MJK
    ENDIF ELSE BEGIN                                                    !MJK
        OPT.HEADER.1$ = "You have chosen to restore the D drive."       !MJK
    ENDIF                                                               !MJK

    CALL DM.SHOW.SCREEN (2, SCR.HEADER$, 2, 2)

    ! Setting the XRE value which will be displayed in the Left corner
    SCREEN.NUM$ = "03"

    ! Dimensioning array
    DIM DAY.ARRAY$(ARRAY.LIMIT%)                                        !MJK
    DIM BKP.AVAIL.ARRAY$(ARRAY.LIMIT%)                                  !MJK

    ! Setting the screen number
    CALL DM.NAME (2, "SCREEN.NUM$", SCREEN.NUM$)

    ! Calling function to get the array of input values                 !IDC
    FUNCTION.ERROR.NOT.EXIST = 0                                        !IDC
    SECOND.FILE$ = XRE.NULL$                                            !MJK

    IF CDRIVE$ = "X" THEN BEGIN                                         !MJK
        FIRST.FILE$  = D.BKP.IMG$ + "BKPFAILC"                          !MJK
    ENDIF ELSE BEGIN                                                    !MJK
        FIRST.FILE$  = C.BKP.IMG$ + "BKPFAILD"                          !MJK
    ENDIF                                                               !MJK

    GOSUB GET.BKP.DETAILS                                               !IDC

    ! Initialising the output fields in the screen
    ! before processing the screen
    !--------------------------------------------
    CALL DM.NAME (48, "OPT.HEADER.1$", OPT.HEADER.1$)                   !MJK

    ! If backups are available
    IF VALUE.INDEX% <> XRE.ZERO% AND FUNCTION.ERROR.NOT.EXIST THEN BEGIN!EJK

        ! Initialising the output fields in the screen
        ! before processing the screen
        !--------------------------------------------
        CALL DM.NAME (49, "OPT.HEADER$",   OPT.HEADER$)                 !MJK
        CALL DM.NAME (50, "OPT.SELECTED$", OPT.SELECTED$)

        ! Enabling the DAY and DD/MM string
        CALL DM.VISIBLE ("75", STATUS.TEXT.MSG$(61))                    !IDC
        CALL DM.VISIBLE ("76", STATUS.TEXT.MSG$(61))                    !IDC

        ! Setting the first value of the fields before populating it
        DAY.LOOP%   = DAY.INDEX%
        DD.MM.LOOP% = DD.MM.INDEX%
        INPUT.LOOP% = INPUT.INDEX%

        ! Retrieving the values and storing in Field$
        FOR INDEX% = 1 TO VALUE.INDEX%

            ! DAY value and its visibility
            FIELD$(DAY.LOOP%) = DAY.ARRAY$(INDEX%)
            CALL DM.VISIBLE (STR$(DAY.LOOP%), STATUS.TEXT.MSG$(61))     !IDC

            ! DD/MM value
            FIELD$(DD.MM.LOOP%) = RIGHT$(BKP.AVAIL.ARRAY$(INDEX%),2) + \!GJK
                                  "/"                                + \
                                  LEFT$(BKP.AVAIL.ARRAY$(INDEX%),2)     !GJK

            ! Setting a space for input values
            FIELD$(INPUT.LOOP%) = XRE.SPACE$

            ! Setting the visibility for DD/MM and input
            CALL DM.VISIBLE (STR$(DD.MM.LOOP%), STATUS.TEXT.MSG$(61))   !IDC
            CALL DM.VISIBLE (STR$(INPUT.LOOP%), STATUS.TEXT.MSG$(61))   !IDC

            ! Incrementing to move to the next field
            DAY.LOOP%   = DAY.LOOP%   + 1
            DD.MM.LOOP% = DD.MM.LOOP% + 1
            INPUT.LOOP% = INPUT.LOOP% + 1
        NEXT INDEX%

    ENDIF ELSE BEGIN                                                    !NJK
        IF FUNCTION.ERROR.NOT.EXIST THEN BEGIN                          !NJK
            FIELD$(1) = STATUS.TEXT.ERROR$(21)                          !IDC
        ENDIF ELSE BEGIN                                                !EJK
            FIELD$(1) = SCREEN.TEXT.MSG$(7) + SCREEN.TEXT.MSG$(8)       !MJK
        ENDIF
    ENDIF                                                               !NJK

RETURN

\***********************************************************************
\*
\* PROCESS.DRIVE.SCREEN: This Subroutine processes the Drive screen
\*                       for XRESTORE.
\*
\***********************************************************************
PROCESS.DRIVE.SCREEN:

    ! Processing Drive screen
    !-------------------------------------
    WHILE SCREEN% = DRIVE.DAY.SELECT.SCR% OR \                          !MJK
          SCREEN% = DRIVE.PROCESS.SCR%                                  !MJK

        ! Drive restore day screen
        IF SCREEN% = DRIVE.DAY.SELECT.SCR% THEN BEGIN                   !MJK

            ! If the screen is accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                GOSUB DISPLAY.DRIVE.SCREEN
                PREVIOUS.KEY = FALSE
            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (2, 105, TRUE)

            IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN !CJK

                ! Navigate to Drive disk selection screen
                SCREEN% = DRIVE.DISK.SELECT.SCR%                        !MJK

            ENDIF ELSE BEGIN                                            !MJK
                IF VALUE.INDEX% = XRE.ZERO% THEN BEGIN                  !MJK
                    CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(21) + \      !IDC
                                   STATUS.TEXT.MSG$(58))                !IDC
                ENDIF ELSE BEGIN
                    IF RET.KEY% = ENTER.KEY% THEN BEGIN

                        ! Check the entry in day selection screen
                        SCREEN.NUM% = DRIVE.PROCESS.SCR%                !MJK
                        GOSUB CHECK.DAY.SELECTION                       !IDC

                        ! If any error in function, set same screen
                        IF NOT FUNCTION.ERROR.NOT.EXIST THEN BEGIN      !EJK
                            SCREEN% = DRIVE.DAY.SELECT.SCR%             !EJK
                        ENDIF                                           !EJK

                    ENDIF ELSE BEGIN
                        ! B001 Invalid key pressed
                        CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))     !IDC
                    ENDIF
                ENDIF                                                   !MJK
            ENDIF
        ENDIF ELSE IF SCREEN% = DRIVE.PROCESS.SCR% THEN BEGIN           !MJK
            ! Display and Process Drives                                !MJK
            GOSUB PROCESS.DRIVE.RESTORE                                 !MJK
        ENDIF

    WEND

RETURN

\***********************************************************************
\*
\* PROCESS.DRIVE.RESTORE: This Subroutine uses the values entered in
\*                        previous pages and starts the restoration
\*                        process for DRIVE(S).
\*
\***********************************************************************
PROCESS.DRIVE.RESTORE:

    ! Changed the help screen to zero, as it is not needed
    CALL DM.SHOW.SCREEN(4, XRE.NULL$, XRE.ZERO%, XRE.ZERO%)             !GJK

    BKP.INDEX%        = XRE.ZERO%
    RESTORE.ERR.EXIST = FALSE

    STATUS.MSG$ = BKP.DATE.ARRAY$(SEL.INDEX%) + STATUS.TEXT.MSG$(24) + \!JDC
                  STATUS.TEXT.MSG$(63)
    GOSUB LOG.STATUS.MSG

    !----------------------------------------------------------!
    ! Setting the BKP.INDEX% and FULL.TO.MOVING.DATE$ based    !        !IDC
    ! on the selected DAY and depending on the SET values the  !
    ! restoration process will happen                          !
    !----------------------------------------------------------!

    GOSUB SET.BACKUP.INDEX                                              !IDC

    ! match for the day and set according to the Full backup day        !IDC
    BKP.INDEX% = LONG.DAY.INDEX%                                        !IDC
    F02.DATE$  = BKP.DATE.ARRAY$(SEL.INDEX%)                            !IDC
    FUN.RC2%   = XRE.ZERO%                                              !MJK

    IF BKP.INDEX% <> 0 THEN BEGIN                                       !IDC
        FUN.RC2% = UPDATE.DATE( ((BKP.INDEX% - 1)* -1 ) )               !MJK
    ENDIF                                                               !IDC

    ! Checking the Return code
    GOSUB CHECK.UPDATE.DATE.RC                                          !IDC
    FULL.TO.MOVING.DATE$ = F02.DATE$                                    !IDC

    ! Storing the Full date                                             !JDC
    FULL.DATE$ = FULL.TO.MOVING.DATE$                                   !IDC

    ! If C drive is selected
    IF CDRIVE$ = "X" THEN BEGIN

        STATUS.MSG$ = STATUS.TEXT.MSG$(25)                              !JDC
        GOSUB LOG.STATUS.MSG

        ! Setting the drive value as "C"
        DRIVE$ = "C"

        ! Reading the BACKUP directories using index
        FOR INDEX% = 1 TO BKPSCRPT.INDEX%
            ! Checking the C directory alone
            IF UCASE$(LEFT$(BKPSCRPT.DIRECTORIES$(INDEX%),1)) = DRIVE$ \
            THEN BEGIN
                ! Setting the Dynamic directory value                   !GJK
                IF POSF(5) = 5 THEN BEGIN                               !GJK
                    CALL PUTF(BKPSCRPT.DIRECTORIES$(INDEX%))            !GJK
                ENDIF                                                   !GJK
                ! Setting the file name based on selected drive
                BKPFAIL.PREFIX$    = D.BKP.IMG$ + "BKPFAILC."           !MJK
                BKPLIST.CURR.FILE$ = BKPLIST.PREFIX.D.DRIVE$            !MJK
                ! Perform the C drive backup process
                GOSUB PERFORM.BACKUP.DRIVE
            ENDIF
        NEXT INDEX%

        CALL DM.STATUS (STATUS.TEXT.MSG$(59))                           !IDC
        !-------------------------------------!
        ! Check the BKPLIST file to make sure !
        ! relevant files alone present        !
        !-------------------------------------!
        GOSUB PROCESS.BKPLIST.FILE
        ! Backing up configuration files. e.g. BKPLIST.MDD              !MJK
        GOSUB BACKUP.CONFIG.FILES                                       !MJK

    ENDIF

    ! If D drive is selected
    IF DDRIVE$ = "X" THEN BEGIN

        STATUS.MSG$ = STATUS.TEXT.MSG$(26)                              !JDC
        GOSUB LOG.STATUS.MSG

        ! Setting the drive value as "D"
        DRIVE$ = "D"

        ! Reading the BACKUP directories using index
        FOR INDEX% = 1 TO BKPSCRPT.INDEX%
            ! Checking the D directory alone
            IF UCASE$(LEFT$(BKPSCRPT.DIRECTORIES$(INDEX%),1)) = DRIVE$ \
            THEN BEGIN
                ! Setting the Dynamic directory value                   !GJK
                IF POSF(5) = 5 THEN BEGIN                               !GJK
                    CALL PUTF(BKPSCRPT.DIRECTORIES$(INDEX%))            !GJK
                ENDIF                                                   !GJK
                ! Setting the file name based on selected drive
                BKPFAIL.PREFIX$    = C.BKP.IMG$ + "BKPFAILD."           !MJK
                BKPLIST.CURR.FILE$ = BKPLIST.PREFIX.C.DRIVE$            !MJK
                ! Perform the D drive backup process
                GOSUB PERFORM.BACKUP.DRIVE
            ENDIF
        NEXT INDEX%

        CALL DM.STATUS (STATUS.TEXT.MSG$(59))                           !IDC
        !-------------------------------------!
        ! Check the BKPLIST file to make sure !
        ! relevant files alone present        !
        !-------------------------------------!
        GOSUB PROCESS.BKPLIST.FILE
        ! Backing up configuration files. e.g. BKPLIST.MDD              !MJK
        GOSUB BACKUP.CONFIG.FILES                                       !MJK

    ENDIF

    ! Set the visibility OFF for unrelated fields
    CALL DM.VISIBLE ("3", STATUS.TEXT.MSG$(60))                         !IDC
    CALL DM.VISIBLE ("5", STATUS.TEXT.MSG$(60))                         !IDC
    CALL DM.VISIBLE ("6", STATUS.TEXT.MSG$(60))                         !IDC
    CALL DM.VISIBLE ("7", STATUS.TEXT.MSG$(60))                         !IDC

    FIELD$(3) = XRE.SPACE$
    FIELD$(5) = XRE.SPACE$
    FIELD$(6) = XRE.SPACE$
    FIELD$(7) = XRE.SPACE$

    ! Depending on the drive selection, the field value gets displayed
    IF CDRIVE$ = "X" AND DDRIVE$ <> "X" THEN BEGIN
        FIELD$(4) = STATUS.TEXT.MSG$(62) + SCREEN.TEXT.MSG$(24)         !MJK
    ENDIF ELSE BEGIN                                                    !NJK
        IF DDRIVE$ = "X" AND CDRIVE$ <> "X" THEN BEGIN                  !NJK
            FIELD$(4) = STATUS.TEXT.MSG$(62) + SCREEN.TEXT.MSG$(23)     !MJK
        ENDIF ELSE BEGIN
            FIELD$(4) = STATUS.TEXT.MSG$(62) + SCREEN.TEXT.MSG$(25)     !MJK
        ENDIF
    ENDIF                                                               !NJK

    GOSUB COPY.OS.BLANK.FILES                                           !JDC

    ! If any error occurs in extraction process
    IF RESTORE.ERR.EXIST THEN BEGIN

        CALL DM.STATUS (SCREEN.TEXT.MSG$(26))                           !JDC
        STATUS.MSG$ = STATUS.TEXT.MSG$(5)                               !JDC
        GOSUB LOG.STATUS.MSG

    ENDIF ELSE BEGIN
    ! If no errors found

        CALL DM.STATUS ("'Restore completed successfully")
        STATUS.MSG$ = "Restore completed successfully"
        GOSUB LOG.STATUS.MSG

    ENDIF

    WHILE SCREEN% = DRIVE.PROCESS.SCR%                                  !CJK
        RET.KEY% = DM.PROCESS.SCREEN (3, 7, FALSE)

        IF RET.KEY% = ESC.KEY% THEN BEGIN                               !MJK
            ! Set the Drive screen
            SCREEN%      = DRIVE.DAY.SELECT.SCR%                        !MJK
            PREVIOUS.KEY = TRUE

        ENDIF ELSE BEGIN

            CALL DM.FOCUS ("1", SCREEN.TEXT.MSG$(13) + \                !JDC
                                SCREEN.TEXT.MSG$(29))                   !MJK
        ENDIF

    WEND

RETURN

! duplicated subroutine
%INCLUDE XREST00E.J86                                                   !NJK

! Commenting out unused Sub-routine                                     !OJK
\***********************************************************************!OJK
\*                                                                      !OJK
\*    HOW.MANY.DAYS.SINCE.FULL: This Subroutine matches the day set     !OJK
\*                              in F13.DAY and sets the number of days  !OJK
\*                              since the Full backup was taken for     !OJK
\*                              this day                                !OJK
\*                                                                      !OJK
\***********************************************************************!OJK
!HOW.MANY.DAYS.SINCE.FULL:                                              !OJK
                                                                        !OJK
!    ! match for the day and set using the offset as days before        !OJK
!    DAYS.AFTER.FULL.BAKUP% = MATCH(":"+F13.DAY$, BACKUP.DAYS$,1)       !OJK
!    DAYS.AFTER.FULL.BAKUP% = (DAYS.AFTER.FULL.BAKUP%-1)/4              !OJK
                                                                        !OJK
!RETURN                                                                 !OJK

\***********************************************************************
\*
\* PERFORM.BACKUP.DRIVE: This Subroutine performs the Drive backup
\*                       process.
\*
\***********************************************************************
PERFORM.BACKUP.DRIVE:

    CALL DM.STATUS ("'" + BKPSCRPT.DIRECTORIES$(INDEX%)              + \
                    SCREEN.TEXT.MSG$(10) + "Please Wait .....")         !MJK

    STATUS.MSG$ = BKPSCRPT.DIRECTORIES$(INDEX%) + " restoration "    + \
                  "in progress"
    GOSUB LOG.STATUS.MSG

    ! If directory not present create it
    IF FUNC.DIR.NOT.EXISTS(BKPSCRPT.DIRECTORIES$(INDEX%)) THEN BEGIN

        DIRECT.TO.RESTORE$ = BKPSCRPT.DIRECTORIES$(INDEX%)

        ! Trim the last slash found to avoid error using MKDIR
        CALL TRIM   (DIRECT.TO.RESTORE$)
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("/"))
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("\\"))

        CALL OSSHELL("MKDIR " + DIRECT.TO.RESTORE$ + " >> "          + \
                     DIR.OUT$ + " >>* "  + DIR.OUT$)
        STATUS.MSG$ = DIRECT.TO.RESTORE$ + " directory is created"
        GOSUB LOG.STATUS.MSG

    ENDIF

    ! Store Full day moving date before processing each directory       !JDC
    FULL.TO.MOVING.DATE$ = FULL.DATE$                                   !IDC

    ! Initiating the Directory restore
    FOR LOOP% = 1 TO BKP.INDEX%

        RESTORE.STATUS = FALSE

        STATUS.MSG$ = BKPSCRPT.DIRECTORIES$(INDEX%)               + \   !MJK
                      STATUS.TEXT.MSG$(31) + FULL.TO.MOVING.DATE$ + \   !MJK
                      SCREEN.TEXT.MSG$(16)                              !MJK
        GOSUB LOG.STATUS.MSG

        GOSUB GET.FILE.EXTENSION                                        !IDC
        MDD.DATE$ = EXTENSION$                                          !IDC

        ! Storing the Full MDD                                          !JDC
        IF LOOP% = 1 THEN BEGIN
            FULL.MDD.DATE$ = MDD.DATE$                                  !IDC

            ! BKPLIST current file                                      !MJK
            BKPLIST.CURR.FILE$ = BKPLIST.CURR.FILE$ + FULL.MDD.DATE$    !MJK

        ENDIF

        ! Setting the Backup files based on the 1st field which is drive
        BKP.FILENAME.IMG$ = PRIMARY.ARCHIVED.NAMES$(INDEX%)          + \
                            "." + MDD.DATE$
        BKP.FILENAME.ALT$ = SECONDARY.ARCHVD.NAMES$(INDEX%)          + \
                            "." + MDD.DATE$

        ! Current BKPFAIL file
        BKPFAIL.CURR.FILE$ = BKPFAIL.PREFIX$ + MDD.DATE$                !MJK

        STATUS.MSG$ = "Checking BKPFAIL " + BKPFAIL.CURR.FILE$
        GOSUB LOG.STATUS.MSG

        DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( BKPFAIL.CURR.FILE$ )  !NJK

        ! If file open unsuccessful
        IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
            ! Setting NULL to avoid file read
            DIR.VALUE$ = XRE.NULL$
            DIR.OPEN   = FALSE                                          !CJK
        ENDIF ELSE BEGIN
            DIR.OPEN   = TRUE
            DIR.VALUE$ = XRE.SPACE$
        ENDIF
        !-------------------------------------------------------------!
        ! Reading the file till the EOF file reached. C file function !
        ! returns NULL when EOF reached or error                      !FDC
        !-------------------------------------------------------------!
        WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
            DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )      !NJK

            ! Checking the comma position
            COMMA.POSITION% = MATCH(COMMA.VALUE$,DIR.VALUE$,1)

            ! If comma found and drive matches
            IF COMMA.POSITION% <> XRE.ZERO% THEN BEGIN                  !GJK

                ! Storing the failed file                               !MJK
                FAILED.FILE$ = LEFT$(DIR.VALUE$,(COMMA.POSITION% - 1))
                ! Storing the Distribution type                         !MJK
                FAILED.FILE.DIST$ = MID$(DIR.VALUE$,            \       !MJK
                                        (COMMA.POSITION% + 1),1)        !MJK
                ! If directory in BKPFAIL matches with current directory!GJK
                IF (BKPSCRPT.DIRECTORIES$(INDEX%)) =                   \!GJK
                    LEFT$(FAILED.FILE$,                                \!GJK
                    LEN(BKPSCRPT.DIRECTORIES$(INDEX%))) THEN BEGIN      !MJK

                    BEGIN.POSITION% = 4         ! Setting the begin position
                    SLASH.POSITION% = XRE.ZERO%
                    VALUE.EXISTS    = TRUE
                    FILENAME$       = XRE.NULL$

                    WHILE VALUE.EXISTS
                        SLASH.POSITION% = MATCH("\\", FAILED.FILE$, \   !MJK
                                                 BEGIN.POSITION%)

                        IF SLASH.POSITION% > XRE.ZERO% THEN BEGIN
                            ! Move to next position to search next field
                            BEGIN.POSITION% = SLASH.POSITION% + 1
                        ENDIF ELSE BEGIN
                            FILENAME$ = MID$(FAILED.FILE$,          \   !MJK
                                             BEGIN.POSITION%,       \   !MJK
                                             (LEN(FAILED.FILE$) -   \   !MJK
                                              BEGIN.POSITION% + 1))     !MJK
                            ! Storing the file name
                            RESTORE.FILENAME$ = FILENAME$               !LJK
                            ! Checking the dot position                 !MJK
                            MATCH.POS% = MATCH(".", FILENAME$,1)        !LJK
                            IF MATCH.POS% <> XRE.ZERO% THEN BEGIN       !LJK
                                RESTORE.FILENAME$ = LEFT$(FILENAME$, \  !LJK
                                                    (MATCH.POS% - 1))   !LJK
                            ENDIF                                       !LJK
                            VALUE.EXISTS = FALSE
                        ENDIF
                    WEND

                    STATUS.MSG$ = STATUS.TEXT.MSG$(9)                   !JDC
                    GOSUB LOG.STATUS.MSG

                    STATUS.MSG$ = STATUS.TEXT.MSG$(10) + FILENAME$      !MJK
                    GOSUB LOG.STATUS.MSG

                    ! Depending on the drive, XDISKIMG directory will
                    ! be used in copying failed file
                    IF DRIVE$ = "D" THEN BEGIN                          !MJK
                        ! Copying the file to respective directory      !MJK
                        CALL OSSHELL("COPY " + C.BKP.IMG$       + \     !MJK
                                     RESTORE.FILENAME$ + "."    + \     !MJK
                                     MDD.DATE$ + XRE.SPACE$     + \     !MJK
                                     FAILED.FILE$ + " >> "      + \     !MJK
                                     DIR.OUT$ + " >>* " + DIR.OUT$)     !MJK
                    ENDIF ELSE BEGIN                                    !MJK
                        ! Copying the file to respective directory      !MJK
                        CALL OSSHELL("COPY " + D.BKP.IMG$       + \     !MJK
                                     RESTORE.FILENAME$ + "."    + \     !MJK
                                     MDD.DATE$ + XRE.SPACE$     + \     !MJK
                                     FAILED.FILE$ + " >> "      + \     !MJK
                                     DIR.OUT$ + " >>* " + DIR.OUT$)     !MJK
                    ENDIF                                               !MJK

                    !---------------------------------------------!
                    ! Not distributing the file if SUPPS mode, as !
                    ! ADXCSU0L won't work under SUPPS             !
                    !---------------------------------------------!
                    IF NOT SUPPS.ON THEN BEGIN
                        ! Setting the distribution type
                        CALL OSSHELL(ADXCSU0L.FILE.NAME$ + " 3 "    + \ !MJK
                                     FAILED.FILE.DIST$ + XRE.SPACE$ + \ !GJK
                                     FAILED.FILE$ + " >> "          + \ !MJK
                                     DIR.OUT$ + " >>* " + DIR.OUT$ )    !GJK
                    ENDIF

                    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )!NJK

                    ! If file open unsuccessful
                    IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                        DIR.OPEN    = FALSE                             !MJK
                        STATUS.MSG$ = STATUS.TEXT.ERROR$(2)             !JDC
                        GOSUB LOG.STATUS.MSG
                        ! Setting NULL to avoid file read
                        DIR.VALUE$ = XRE.NULL$
                    ENDIF ELSE BEGIN
                        DIR.OPEN   = TRUE
                        DIR.VALUE$ = XRE.SPACE$
                    ENDIF

                    ! Read the file till EOF or read error              !GJK
                    WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                        DIR.VALUE$ = \                                  !NJK
                            FUNC.READ.SEQUENTIAL.FILE(DIR.FILE.RC%)     !NJK

                        !-----------------------------------------------!
                        ! If error string matches, write the error with !
                        ! file name in LOG file                         !
                        !-----------------------------------------------!
                        IF MATCH("ERROR",UCASE$(DIR.VALUE$),1)        \ !OJK
                           <> XRE.ZERO%                            OR \ !OJK
                           MATCH("cannot be found",(DIR.VALUE$),1) <> \ !OJK
                           XRE.ZERO% THEN BEGIN                         !OJK
                            ! Logging the copy / distribution error
                            STATUS.MSG$ = STATUS.TEXT.ERROR$(14)        !JDC
                            GOSUB LOG.STATUS.MSG
                            STATUS.MSG$ = DIR.VALUE$
                            GOSUB LOG.STATUS.MSG

                            RESTORE.ERR.EXIST = TRUE                    !OJK
                            ! To break the WHILE loop                   !OJK
                            DIR.VALUE$ = XRE.NULL$                      !OJK
                        ENDIF
                    WEND

                    ! Closing File
                    IF DIR.OPEN THEN BEGIN
                        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )            !NJK
                        DIR.OPEN = FALSE
                    ENDIF
                ENDIF                                                   !GJK
            ENDIF
        WEND

        ! Closing File
        IF DIR.OPEN THEN BEGIN
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !NJK
            DIR.OPEN = FALSE
        ENDIF

        !--------------------------------------------------------------!
        ! Checking the archive file existence before extraction. First !
        ! the file will be checked in IMG location and it not present, !
        ! then it will be tried in ALT location                        !
        !--------------------------------------------------------------!
        IF FUNC.FILE.EXISTS(BKP.FILENAME.IMG$) THEN BEGIN

            CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -x "                + \
                         BKP.FILENAME.IMG$   + XRE.SPACE$            + \
                         BKPSCRPT.DIRECTORIES$(INDEX%)               + \
                         " > " + DIR.OUT$ + " >>* " + DIR.OUT$)

            STATUS.MSG$ = BKP.FILENAME.IMG$ + STATUS.TEXT.MSG$(11)   + \!JDC
                          BKPSCRPT.DIRECTORIES$(INDEX%)
            GOSUB LOG.STATUS.MSG

            ! Setting the status to check the extraction output file
            RESTORE.STATUS = TRUE

        ENDIF ELSE BEGIN                                                !NJK
            IF FUNC.FILE.EXISTS(BKP.FILENAME.ALT$) THEN BEGIN           !NJK

                CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -x "            + \
                             BKP.FILENAME.ALT$   + XRE.SPACE$        + \
                             BKPSCRPT.DIRECTORIES$(INDEX%)           + \
                             " > " + DIR.OUT$ + " >>* " + DIR.OUT$)

                STATUS.MSG$ = BKP.FILENAME.ALT$ + STATUS.TEXT.MSG$(11)+ \JDC
                              BKPSCRPT.DIRECTORIES$(INDEX%)
                GOSUB LOG.STATUS.MSG

                ! Setting the status to check the extraction output file
                RESTORE.STATUS = TRUE

            ENDIF
        ENDIF                                                           !NJK

        ! Initializing to FALSE
        RESTORE.HAPPENED = FALSE                                        !MJK

        ! If file extraction happened
        IF RESTORE.STATUS THEN BEGIN                                    !MJK

            ! Setting TRUE for successful extraction
            RESTORE.HAPPENED = TRUE                                     !MJK
            ! Open file                                                 !MJK
            DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )        !NJK

            ! If file open unsuccessful
            IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                DIR.OPEN    = FALSE                                     !MJK
                STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                     !JDC
                GOSUB LOG.STATUS.MSG
                ! Setting NULL to avoid file read
                DIR.VALUE$ = XRE.NULL$
            ENDIF ELSE BEGIN
                DIR.OPEN   = TRUE
                DIR.VALUE$ = XRE.SPACE$
            ENDIF

            ! Read the file till EOF or read error                      !FDC
            WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )  !NJK

                !-----------------------------------------------!
                ! If error string matches, write the error with !
                ! file name in LOG file                         !
                !-----------------------------------------------!
                IF MATCH("Error extracting file",DIR.VALUE$,1) <>      \
                   XRE.ZERO% THEN BEGIN

!                    STATUS.MSG$ = STATUS.TEXT.ERROR$(12)               !OJK
!                    GOSUB LOG.STATUS.MSG                               !OJK

                    ! Setting the flag for extraction error
                    RESTORE.ERR.EXIST = TRUE
                    STATUS.MSG$       = DIR.VALUE$
                    GOSUB LOG.STATUS.MSG
                ENDIF
            WEND

            ! Closing File
            IF DIR.OPEN THEN BEGIN
                CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                    !NJK
                DIR.OPEN = FALSE
            ENDIF

        ENDIF
        ! Starting from Full day increment the date till requested date !JDC
        F02.DATE$ = FULL.TO.MOVING.DATE$
        FUN.RC2%  = UPDATE.DATE( 1 )                                    !MJK
        ! Checking the Return code
        GOSUB CHECK.UPDATE.DATE.RC                                      !IDC
        FULL.TO.MOVING.DATE$ = F02.DATE$

    NEXT LOOP%

RETURN

\***********************************************************************
\*
\* PROCESS.BKPLIST.FILE: This Subroutine reads the BKPLIST file and
\*                       removes the file, if any file has NOT EXIST
\*                       option enabled.
\*
\***********************************************************************
PROCESS.BKPLIST.FILE:

    STATUS.MSG$ = "Checking " + BKPLIST.CURR.FILE$ + " to check the" + \
                  " extracted file"
    GOSUB LOG.STATUS.MSG

    !--------------------------------------------------------!
    ! Checking whether BKPLIST file exist and the extraction !
    ! happened and also requested date is NOT Full           !          !JDC
    !--------------------------------------------------------!
    IF RESTORE.HAPPENED AND BKP.INDEX% > 1 THEN BEGIN                   !MJK
        ! only check the file when this far                             !JDC
        IF FUNC.FILE.EXISTS(BKPLIST.CURR.FILE$) THEN BEGIN              !JDC
            ! Opening the BKPLIST file                                  !MJK
            BKPLIST.FILE.RC% = \                                        !NJK
                    FUNC.OPEN.SEQUENTIAL.FILE (BKPLIST.CURR.FILE$)      !NJK
            ! If file open unsuccessful
            IF BKPLIST.FILE.RC% <= XRE.ZERO% THEN BEGIN
                BKPLIST.OPEN = FALSE                                    !MJK
                STATUS.MSG$  = "Error in opening BKPLIST file"          !MJK
                GOSUB LOG.STATUS.MSG
                ! Setting NULL to avoid file read
                BKPLIST.DIR.VALUE$ = XRE.NULL$
            ENDIF ELSE BEGIN
                BKPLIST.OPEN       = TRUE
                BKPLIST.DIR.VALUE$ = XRE.SPACE$
            ENDIF

            ! Read the file till the EOF
            WHILE LEN(BKPLIST.DIR.VALUE$) <> XRE.ZERO%
                BKPLIST.DIR.VALUE$ = \                                  !JDC
                    FUNC.READ.SEQUENTIAL.FILE(BKPLIST.FILE.RC%)         !NJK

                !---------------------------------------------------!
                ! Checking the directory name of BKPLIST files with !
                ! the requested drive                               !
                !---------------------------------------------------!
                IF UCASE$(LEFT$(BKPLIST.DIR.VALUE$,1)) = DRIVE$ \       !JDC
                THEN BEGIN                                              !JDC

                    ! Extract the individual fields using function
                    GOSUB EXTRACT.BKPLIST.FIELDS                        !IDC

                    IF NOT FUNCTION.ERROR.NOT.EXIST THEN BEGIN          !EJK
                        ! Setting the value for exception
                        RESTORE.ERR.EXIST = TRUE                        !EJK
                    ENDIF                                               !EJK

                    IF BKPLI.INCREMENTAL.EXIST$(BKP.INDEX% - 1) <> "Y" \!MJK
                    THEN BEGIN                                          !IDC
                        GOSUB DEL.BKPLIST.FILE                          !IDC
                    ENDIF                                               !IDC
                ENDIF
            WEND

            ! Closing File
            IF BKPLIST.OPEN THEN BEGIN
                CALL FUNC.CLOSE.FILE( BKPLIST.FILE.RC% )                !NJK
                BKPLIST.OPEN = FALSE
            ENDIF
        ENDIF                                                           !JDC
    ENDIF

RETURN

\***********************************************************************!EJK
\*                                                                      !EJK
\*  DEL.BKPLIST.FILE: This Sub-Routine deletes the BKPLIST file         !EJK
\*                    which is set as Non-exist.                        !EJK
\*                                                                      !EJK
\***********************************************************************!EJK
DEL.BKPLIST.FILE:

    CALL OSSHELL("DEL " + BKPLI.FILENAME$ + " >> " + DIR.OUT$ + \       !JDC
                 " >>* " + DIR.OUT$)
    STATUS.MSG$ = STATUS.TEXT.MSG$(7) + BKPLI.FILENAME$                 !JDC
    GOSUB LOG.STATUS.MSG

RETURN

\***********************************************************************!MJK
\*                                                                      !MJK
\* BACKUP.CONFIG.FILES: This Sub-Routine backs up the configuration     !MJK
\*                      files at the end of restore                     !MJK
\*                                                                      !MJK
\***********************************************************************!MJK
BACKUP.CONFIG.FILES:

    IF DRIVE$ = "C" THEN BEGIN
        BKP.DRIVE$ = "D"
    ENDIF ELSE BEGIN
        BKP.DRIVE$ = "C"
    ENDIF

    CALL OSSHELL("COPY " + BKP.DRIVE$ + ":\ADX_UDT1\BKPLIST.* " + \
                 DRIVE$ + ":\ADX_UDT1\" + " > " + DIR.OUT$      + \
                 " >>* " + DIR.OUT$)

    DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )                !NJK

    ! If file open unsuccessful
    IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
        DIR.OPEN    = FALSE
        STATUS.MSG$ = "Error in opening DIR output file"
        GOSUB LOG.STATUS.MSG
        ! Setting NULL to avoid file read
        DIR.VALUE$ = XRE.NULL$
    ENDIF ELSE BEGIN
        DIR.OPEN   = TRUE
        DIR.VALUE$ = XRE.SPACE$
    ENDIF

    ! Read the file till EOF or read error
    WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
        DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )          !NJK

        !-----------------------------------------------!
        ! If error string matches, write the error      !
        !-----------------------------------------------!
        IF MATCH(UCASE$("Error"),UCASE$(DIR.VALUE$),1) <> XRE.ZERO% \
        THEN BEGIN
            ! Logging the error
            STATUS.MSG$ = "Error when backing up BKPLIST files"
            GOSUB LOG.STATUS.MSG
            DIR.VALUE$        = XRE.NULL$
            RESTORE.ERR.EXIST = TRUE                                    !OJK
        ENDIF
    WEND

    ! Closing File
    IF DIR.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                            !NJK
        DIR.OPEN = FALSE
    ENDIF

RETURN

\***********************************************************************
\*
\* DISPLAY.DIRECT.SCREEN: This Subroutine displays the DIRECTORY
\*                        screen for XRESTORE.
\*
\***********************************************************************
DISPLAY.DIRECT.SCREEN:

    ! Screen constant variables
    DIR.INDEX%       = 30               ! Directory name
    DIR.INPUT.INDEX% = 100              ! Input value

    PAGE.NO%         = 1                ! Setting the page number
    MAX.DIRECTORIES% = DIR.TO.SHOW%     ! Setting the number of entries

    ! Dimensioning the array
    DIM SELECTED.DIR.ARRAY$(ARRAY.LIMIT%)                               !MJK

    ! Initiating an array with spaces for input values
    FOR LOOP% = 1 TO BKPSCRPT.INDEX%
        !----------------------------------------------------!
        ! BKPSCRPT.INDEX% array index check has been already !
        ! handled when BKPSCRPT array has been added         !
        !----------------------------------------------------!
        SELECTED.DIR.ARRAY$(LOOP%) = XRE.SPACE$
    NEXT LOOP%

    ! Setting the variables to process
    DIR.LOOP%        = DIR.INDEX%
    DIR.INPUT.LOOP%  = DIR.INPUT.INDEX%

    CALL DM.SHOW.SCREEN (5, XRE.NULL$, 4, 4)

    ! Setting the variable for screen continuation text
    CALL DM.NAME (53, "USER.TEXT$", USER.TEXT$)
    USER.TEXT$ = "Continued...."

    ! Checking the Index for Backups
    IF BKPSCRPT.INDEX% > XRE.ZERO% THEN BEGIN
        ! Storing the page number
        PAGE.DIV% = BKPSCRPT.INDEX%/MAX.DIRECTORIES%

        IF MOD(BKPSCRPT.INDEX%,MAX.DIRECTORIES%) THEN BEGIN
            ! Incrementing by 1 if MOD value is greater than zero
            PAGE.DIV% = PAGE.DIV% + 1
        ENDIF

        ! Setting the total page number
        FIELD$(4) = STR$(PAGE.DIV%)

        GOSUB DISPLAY.DIR.PAGE

    ENDIF ELSE BEGIN
        FIELD$(3) = "1"
        FIELD$(4) = "1"
        FIELD$(1) = SCREEN.TEXT.MSG$(30)                                !JDC
    ENDIF

RETURN

\***********************************************************************
\*
\* PROCESS.DIRECT.SCREEN: This Subroutine displays the DIRECTORY
\*                        screen for XRESTORE.
\*
\***********************************************************************
PROCESS.DIRECT.SCREEN:

    WHILE SCREEN% = RESTORE.A.DIRECTORY.SCR% OR \                       !CJK
          SCREEN% = DIRECTORY.DAY.SELECT.SCR%                           !CJK

        IF SCREEN% = RESTORE.A.DIRECTORY.SCR% THEN BEGIN                !CJK
            ! If the screen is accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                GOSUB DISPLAY.DIRECT.SCREEN
                PREVIOUS.KEY = FALSE
            ENDIF

            ! Process multi page directory screen
            GOSUB PROCESS.MULTI.DIR.SCREEN
        ENDIF ELSE IF SCREEN% = DIRECTORY.DAY.SELECT.SCR% THEN BEGIN    !CJK

            !------------------------------------------------------!
            ! Complete Directory restore process will be carried   !
            ! out by this Sub-program. This has been created as a  !
            ! Sub-program to overcome the foreseen 64K issue       !
            !------------------------------------------------------!
            CALL SUB.PROCESS.DAY.DIR.SCREEN                            \
                    (PRIMARY.ARCHIVED.NAMES$(SELECT.INDEX%),           \
                     SECONDARY.ARCHVD.NAMES$(SELECT.INDEX%))
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* PROCESS.MULTI.DIR.SCREEN: This Subroutine displays the MULTI
\*                           DIRECTORY screen for XRESTORE.
\*
\***********************************************************************
PROCESS.MULTI.DIR.SCREEN:

    RET.KEY% = DM.PROCESS.SCREEN (2, 123, TRUE)

    ! ESC or F3 press or No Backup directories
    IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) OR \
       BKPSCRPT.INDEX% = XRE.ZERO% THEN BEGIN

        CALL DM.FOCUS("10",SCREEN.TEXT.MSG$(31))                        !JDC

        ! Main screen
        SCREEN% = DISPLAY.MAIN.SCR%                                     !CJK

    ENDIF ELSE IF RET.KEY% = F8.KEY% OR RET.KEY% = PGDN.KEY% THEN BEGIN
        ! When F8 or Page down key pressed

        ! If Page no. is lesser than the MAX page
        IF PAGE.NO% < PAGE.DIV% THEN BEGIN
            ! Save the fields first before moving to next page
            GOSUB SAVE.DIR.FIELD

            ! Increment the page
            PAGE.NO% = PAGE.NO% + 1

            ! Display the next page
            GOSUB DISPLAY.DIR.PAGE
        ENDIF ELSE BEGIN
            ! If invalid key press
            FIELD$(1) = "'There are no more pages to display"
        ENDIF
    ENDIF ELSE IF RET.KEY% = F7.KEY% OR RET.KEY% = PGUP.KEY% THEN BEGIN
        ! When F7 or Page up key pressed

        ! If Page no. is greater than 1
        IF PAGE.NO% > 1 THEN BEGIN
            ! Save the fields first before moving to next page
            GOSUB SAVE.DIR.FIELD

            ! Decrement the page
            PAGE.NO% = PAGE.NO% - 1

            ! Display the previous page
            GOSUB DISPLAY.DIR.PAGE
        ENDIF ELSE BEGIN
            ! If invalid key press
            FIELD$(1) = "'There is no previous page to display"
        ENDIF
    ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN
        ! Save the field
        GOSUB SAVE.DIR.FIELD

        ! Process the input values
        GOSUB PROCESS.DIR.INPUT
    ENDIF ELSE BEGIN
        ! B001 Invalid key pressed
        CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))                     !IDC
    ENDIF

RETURN

\***********************************************************************
\*
\* DISPLAY.DIR.PAGE: This Subroutine the display the available
\*                   directories based on the page number value.
\*
\***********************************************************************
DISPLAY.DIR.PAGE:

    !---------------------------------------------------------!
    ! Index values are used as constant variables for screens !
    ! Loop values are used for populating the values in field !
    !---------------------------------------------------------!
    DIR.LOOP%        = DIR.INDEX%
    DIR.INPUT.LOOP%  = DIR.INPUT.INDEX%
    MAX.DIRECTORIES% = DIR.TO.SHOW%

    ! Setting the Page numbers
    FIELD$(3) = STR$(PAGE.NO%)
    FIELD$(4) = STR$(PAGE.DIV%)

    ! Setting the Current field
    CALL DM.CURRENT.FIELD(DIR.INPUT.INDEX%)

    !--------------------------------------------------------------!
    ! Checking whether total available directories is greater than !
    ! current screen values. If not, the condition will go to the  !
    ! Else loop which will show the last page.                     !
    !--------------------------------------------------------------!
    IF BKPSCRPT.INDEX% > (MAX.DIRECTORIES% * PAGE.NO%) THEN BEGIN

        ! If Page num 1, Page up disabled
        IF PAGE.NO% = 1 THEN BEGIN
            CALL DM.HIDE.FN.KEY (7)
            CALL DM.SHOW.FN.KEY (8, "PGDN")

        ENDIF ELSE BEGIN
            CALL DM.SHOW.FN.KEY (7, "PGUP")
            CALL DM.SHOW.FN.KEY (8, "PGDN")
        ENDIF

        ! Setting the visibility of the "Continued" text
        CALL DM.VISIBLE (STR$(53), STATUS.TEXT.MSG$(61))                !IDC

    ENDIF ELSE BEGIN
        ! Setting the max directories based on the available values
        MAX.DIRECTORIES% = BKPSCRPT.INDEX% - (MAX.DIRECTORIES% * \
                                             (PAGE.NO% - 1))
        ! As this is the last page, Hide the Page down key
        CALL DM.HIDE.FN.KEY (8)

        ! If only one page, hide both page up and page down keys
        IF PAGE.NO% = 1 THEN BEGIN
            CALL DM.HIDE.FN.KEY (7)
        ENDIF

        ! Disable the "Continued" text
        CALL DM.VISIBLE (STR$(53), STATUS.TEXT.MSG$(60))                !IDC

    ENDIF

    ! Looping to display the directories in selected page
    FOR LOOP% = 1 TO MAX.DIRECTORIES%

        ! Directory name
        FIELD$(DIR.LOOP%) = BKPSCRPT.DIRECTORIES$(                 \    !MJK
                            LOOP% + ((PAGE.NO% - 1) * DIR.TO.SHOW%))    !MJK
        ! Setting spaces for input value
        FIELD$(DIR.INPUT.LOOP%) = SELECTED.DIR.ARRAY$(LOOP%          + \!MJK
                                       ((PAGE.NO% - 1) * DIR.TO.SHOW%)) !MJK
        ! Setting the visibility
        CALL DM.VISIBLE (STR$(DIR.LOOP%)      , STATUS.TEXT.MSG$(61))   !MJK
        CALL DM.VISIBLE (STR$(DIR.INPUT.LOOP%), STATUS.TEXT.MSG$(61))   !IDC

        ! Incrementing to the next field
        DIR.LOOP%       = DIR.LOOP% + 1
        DIR.INPUT.LOOP% = DIR.INPUT.LOOP% + 1

    NEXT LOOP%

    !---------------------------------------!
    ! If last page, disable the remaining   !
    ! directory field and input field       !
    !---------------------------------------!
    IF MAX.DIRECTORIES% < DIR.TO.SHOW% THEN BEGIN

        ! Looping from above finished value till the MAX value in a page
        FOR LOOP% = MAX.DIRECTORIES% + 1 TO DIR.TO.SHOW%
            CALL DM.VISIBLE (STR$(DIR.LOOP%), STATUS.TEXT.MSG$(60))     !IDC
            CALL DM.VISIBLE (STR$(DIR.INPUT.LOOP%),STATUS.TEXT.MSG$(60))!MJK

            DIR.LOOP%       = DIR.LOOP% + 1
            DIR.INPUT.LOOP% = DIR.INPUT.LOOP% + 1

        NEXT LOOP%
    ENDIF

RETURN

\***********************************************************************
\*
\* SAVE.DIR.FIELD: This Subroutine will be used by Directory page when
\*                 Page up/down, F7/F8 or Enter key has been pressed.
\*                 It will help in saving the value of current page.
\*
\***********************************************************************
SAVE.DIR.FIELD:

    ! First input field
    DIR.INPUT.LOOP%  = DIR.INPUT.INDEX%
    SEL.INPUT.LOOP%  = 1        ! Initiating to 1 before use


    FOR LOOP% = 1 TO MAX.DIRECTORIES%

        ! Set depending on the current page
        SEL.INPUT.LOOP% = ((PAGE.NO% - 1) * DIR.TO.SHOW%) + LOOP%

        !----------------------------------------------------------!
        ! Stored the field value using the index value created by  !
        ! the current page number                                  !
        !----------------------------------------------------------!
        SELECTED.DIR.ARRAY$(SEL.INPUT.LOOP%) = FIELD$(DIR.INPUT.LOOP%)

        ! Incrementing the input field index
        DIR.INPUT.LOOP% = DIR.INPUT.LOOP% + 1

    NEXT LOOP%

RETURN

\***********************************************************************
\*
\* PROCESS.DIR.INPUT: This Subroutine processes the directory input
\*                    values.
\*
\***********************************************************************
PROCESS.DIR.INPUT:

    ! Initiating the variables before use
    SELECT.COUNT% = XRE.ZERO%
    NON.X.VALUES  = FALSE

    ! Checking all the input fields using array index
    FOR LOOP% = 1 TO BKPSCRPT.INDEX%

        ! If input value is not equal to space
        IF SELECTED.DIR.ARRAY$(LOOP%) <> XRE.SPACE$ THEN BEGIN
            ! If non X values are entered
            IF UCASE$(SELECTED.DIR.ARRAY$(LOOP%)) <> "X" THEN BEGIN
                NON.X.VALUES = TRUE
            ENDIF ELSE BEGIN
                ! Storing the X selection
                SELECT.INDEX% = LOOP%
            ENDIF
            ! Incrementing to check the Multiple selection
            SELECT.COUNT% = SELECT.COUNT% + 1
        ENDIF
    NEXT LOOP%

    ! If more than 1 input is selected
    IF SELECT.COUNT% > 1 THEN BEGIN
        FIELD$(1) = "'Multiple selections not allowed"
    ENDIF ELSE BEGIN
        ! If only one value is selected and no Non X values present
        IF SELECT.COUNT% = 1 AND NOT NON.X.VALUES THEN BEGIN            !MJK

            STATUS.MSG$ = BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)       + \
                          " directory is selected for restore"
            GOSUB LOG.STATUS.MSG

            ! Setting screen number to display available days
            SCREEN% = DIRECTORY.DAY.SELECT.SCR%                         !CJK

        ENDIF ELSE BEGIN
            ! If error
            FIELD$(1) = SCREEN.TEXT.MSG$(21)                            !JDC
        ENDIF
    ENDIF

RETURN

\***********************************************************************
\*
\* PROCESS.FILE.SCREEN: This Subroutine displays the FILE screen
\*                      for XRESTORE.
\*
\***********************************************************************
PROCESS.FILE.SCREEN:

    ! If File enter screen or day availability screen
    WHILE SCREEN% = RESTORE.A.FILE.SCR% OR \                            !CJK
          SCREEN% = FILE.DAY.SELECT.SCR%                                !CJK

        IF SCREEN% = RESTORE.A.FILE.SCR% THEN BEGIN                     !CJK
            ! If screen is accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                CALL DM.SHOW.SCREEN (7, XRE.NULL$, 6, 6)
                PREVIOUS.KEY = FALSE
            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (2, 2, TRUE)

            IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN !CJK

                ! Main screen
                SCREEN%      = DISPLAY.MAIN.SCR%                        !CJK
                PREVIOUS.KEY = FALSE

            ENDIF ELSE BEGIN
                IF RET.KEY% = ENTER.KEY% THEN BEGIN
                    ! Checks the entered file
                    GOSUB CHECK.ENTERED.FILE

                ENDIF ELSE BEGIN
                    ! B001 Invalid key pressed
                    CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))         !IDC
                ENDIF
            ENDIF

        ENDIF

        ! Checks the available restore days and display it
        IF SCREEN% = FILE.DAY.SELECT.SCR% THEN BEGIN                    !CJK
            GOSUB CHECK.PROCESSED.FILE
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* CHECK.ENTERED.FILE: This Subroutine checks the FILE input entered
\*                     in Restore File screen
\*
\***********************************************************************
CHECK.ENTERED.FILE:

    ! Storing the File name
    SELECT.FILE.NAME$ = FIELD$(2)

    ! Trim it
    CALL TRIM(SELECT.FILE.NAME$)

    ! Setting the initial values
    SLASH.POSITION% = XRE.ZERO%
    BEGIN.POSITION% = 1
    SELECT.INDEX%   = XRE.ZERO%
    VALUE.EXISTS    = TRUE                                              !MJK

    !-------------------------------------------------------------!     !MJK
    ! BKPSCRPT has directory values with backward slashes. Hence  !     !MJK
    ! converting the forward slashes entered by USER to backward  !     !MJK
    ! slashes for easiness of string comparison                   !     !MJK
    !-------------------------------------------------------------!     !MJK

    ! To convert all forward slashes to backward slashes                !MJK
    WHILE VALUE.EXISTS                                                  !MJK
        ! Checks the forward slash position                             !MJK
        SLASH.POSITION% = MATCH("/",SELECT.FILE.NAME$,BEGIN.POSITION%)  !MJK

        IF SLASH.POSITION% > XRE.ZERO% THEN BEGIN                       !MJK
            SELECT.FILE.NAME$ = LEFT$(SELECT.FILE.NAME$,             \  !MJK
                                      (SLASH.POSITION% - 1)) + "\" + \  !MJK
                                RIGHT$(SELECT.FILE.NAME$,            \  !MJK
                                      (LEN(SELECT.FILE.NAME$) -      \  !MJK
                                       SLASH.POSITION%))                !MJK
            ! Move to next position to search next field                !MJK
            BEGIN.POSITION% = SLASH.POSITION% + 1                       !MJK
        ENDIF ELSE BEGIN                                                !MJK
            VALUE.EXISTS = FALSE                                        !MJK
        ENDIF                                                           !MJK
    WEND                                                                !MJK

    ! Reset the values
    BEGIN.POSITION% = 1
    SLASH.POSITION% = XRE.ZERO%
    VALUE.EXISTS    = TRUE

    ! Loop continues till last backward slash found
    WHILE VALUE.EXISTS
        ! Checks the backward slash position
        SLASH.POSITION% = MATCH("\\",SELECT.FILE.NAME$,BEGIN.POSITION%) !MJK

        IF SLASH.POSITION% > XRE.ZERO% THEN BEGIN
            ! Move to next position to search next field
            BEGIN.POSITION% = SLASH.POSITION% + 1
        ENDIF ELSE BEGIN
            VALUE.EXISTS = FALSE
        ENDIF
    WEND

    !--------------------------------------------------!
    ! Checking the BKPSCRPT directories to see if the  !
    ! entered file matches                             !
    !--------------------------------------------------!
    FOR INDEX% = 1 TO BKPSCRPT.INDEX%
        IF BKPSCRPT.DIRECTORIES$(INDEX%) = UCASE$(MID$(            \
                                           SELECT.FILE.NAME$,1,    \
                                           (BEGIN.POSITION% - 1))) \
        THEN BEGIN
            SELECT.INDEX% = INDEX%
        ENDIF
    NEXT INDEX%

    ! Storing the File name
    SELECTED.FILENAME$ = MID$(SELECT.FILE.NAME$,BEGIN.POSITION%,   \
                             (LEN(SELECT.FILE.NAME$) -             \
                              BEGIN.POSITION% + 1))
    ! Trim spaces
    CALL TRIM(SELECT.FILE.NAME$)

    ! If no matches found
    IF SELECT.INDEX% = XRE.ZERO% THEN BEGIN
        CALL DM.FOCUS ("3", "'Please enter a valid directory name")

    ! If no file name entered after the directory
    ENDIF ELSE BEGIN                                                    !NJK
        IF (BEGIN.POSITION% - 1)   = LEN(SELECT.FILE.NAME$) OR  \       !NJK
           LEN(SELECTED.FILENAME$) > 12 THEN BEGIN                      !NJK
            SELECT.INDEX% = XRE.ZERO%
            CALL DM.FOCUS ("3", "'Please enter a valid file name")
        ENDIF ELSE BEGIN
        ! Valid Directory entered

            STATUS.MSG$ = UCASE$(SELECT.FILE.NAME$)           + \       !OJK
                          " file is entered for restoration"            !OJK
            GOSUB LOG.STATUS.MSG
            ! Setting the screen to move to next screen
            SCREEN% = FILE.DAY.SELECT.SCR%                              !CJK
        ENDIF
    ENDIF                                                               !NJK

RETURN

\***********************************************************************
\*
\* CHECK.PROCESSED.FILE: This Subroutine displays the available days
\*                       for selected file backup. Also processes the
\*                       field for all key strokes.
\*
\***********************************************************************
CHECK.PROCESSED.FILE:

    GOSUB SHOW.FILE.RESTORE.DAYS

    GOSUB PROCESS.FILE.RESTORE

RETURN

\***********************************************************************
\*
\* SHOW.FILE.RESTORE.DAYS: This Subroutine display the available backup
\*                         days for Entered file name
\*
\***********************************************************************
SHOW.FILE.RESTORE.DAYS:

    ! Setting the Header and other variables used for screen display
    SCR.HEADER$   = "RESTORE A FILE PROCESSING"                         !MJK
    OPT.SELECTED$ = SCREEN.TEXT.MSG$(2) + SCREEN.TEXT.MSG$(3)           !JDC
    OPT.HEADER.1$ = "Please choose which day to restore the " + \
                    "file: " + SELECT.FILE.NAME$

    ! Display screen
    CALL DM.SHOW.SCREEN(2, SCR.HEADER$, 7, 7)

    ! Setting the XRE value which will be displayed in the Left corner
    SCREEN.NUM$ = "08"

    ! Dimensioning the array
    DIM DAY.ARRAY$(ARRAY.LIMIT%)                                        !MJK
    DIM BKP.AVAIL.ARRAY$(ARRAY.LIMIT%)                                  !MJK

    ! Setting the default value before calling the Sub-routine          !MJK
    FUNCTION.ERROR.NOT.EXIST = 0                                        !IDC

    FIRST.FILE$  = PRIMARY.ARCHIVED.NAMES$(SELECT.INDEX%)               !MJK
    SECOND.FILE$ = SECONDARY.ARCHVD.NAMES$(SELECT.INDEX%)               !IDC
    GOSUB GET.BKP.DETAILS                                               !IDC

    ! Setting the screen number
    CALL DM.NAME (2, "SCREEN.NUM$", SCREEN.NUM$)

    ! Initialising the output fields in the screen
    ! before processing the screen
    !--------------------------------------------
    CALL DM.NAME (48, "OPT.HEADER.1$", OPT.HEADER.1$)

    ! If backups are available
    IF VALUE.INDEX% <> XRE.ZERO% AND FUNCTION.ERROR.NOT.EXIST THEN BEGIN!EJK

        ! Enabling the DAY and DD/MM string
        CALL DM.VISIBLE ("75", STATUS.TEXT.MSG$(61))                    !IDC
        CALL DM.VISIBLE ("76", STATUS.TEXT.MSG$(61))                    !IDC

        ! Setting other relevant string
        CALL DM.NAME (50, "OPT.SELECTED$", OPT.SELECTED$)

        ! Setting the first value of the fields before populating it
        DAY.LOOP%   = DAY.INDEX%                                        !MJK
        DD.MM.LOOP% = DD.MM.INDEX%                                      !MJK
        INPUT.LOOP% = INPUT.INDEX%                                      !MJK

        ! Retrieving the values and storing in Field$
        FOR INDEX% = 1 TO VALUE.INDEX%

            ! DAY value and its visibility
            FIELD$(DAY.LOOP%) = DAY.ARRAY$(INDEX%)
            CALL DM.VISIBLE (STR$(DAY.LOOP%), STATUS.TEXT.MSG$(61))     !IDC

            ! DD/MM value
            FIELD$(DD.MM.LOOP%) = RIGHT$(BKP.AVAIL.ARRAY$(INDEX%),2) + \!GJK
                                  "/"                                + \
                                  LEFT$(BKP.AVAIL.ARRAY$(INDEX%),2)     !GJK

            ! Setting a space for input values
            FIELD$(INPUT.LOOP%) = XRE.SPACE$

            ! Setting the visibility for DD/MM and input
            CALL DM.VISIBLE (STR$(DD.MM.LOOP%), STATUS.TEXT.MSG$(61))   !IDC
            CALL DM.VISIBLE (STR$(INPUT.LOOP%), STATUS.TEXT.MSG$(61))   !IDC

            ! Incrementing to move to the next field
            DAY.LOOP%   = DAY.LOOP%   + 1
            DD.MM.LOOP% = DD.MM.LOOP% + 1
            INPUT.LOOP% = INPUT.LOOP% + 1

        NEXT INDEX%

    ENDIF ELSE BEGIN                                                    !NJK
        IF FUNCTION.ERROR.NOT.EXIST THEN BEGIN                          !NJK
            FIELD$(1) = STATUS.TEXT.ERROR$(21)                          !IDC
        ENDIF ELSE BEGIN                                                !EJK
            FIELD$(1) = SCREEN.TEXT.MSG$(7) + SCREEN.TEXT.MSG$(8)       !MJK
        ENDIF
    ENDIF                                                               !NJK

RETURN

\***********************************************************************
\*
\* PROCESS.FILE.RESTORE: This Subroutine helps in processing the input
\*                       values entered in File restore day selection
\*                       screen.
\*
\***********************************************************************
PROCESS.FILE.RESTORE:

    ! Handles restore day selection and confirmation screen
    WHILE SCREEN% = FILE.DAY.SELECT.SCR% OR \                           !CJK
          SCREEN% = FILE.DIR.AND.DIST.SCR%                              !CJK

        IF SCREEN% = FILE.DAY.SELECT.SCR% THEN BEGIN                    !CJK
            ! If screen is accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                GOSUB SHOW.FILE.RESTORE.DAYS
                PREVIOUS.KEY = FALSE
            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (2, 105, TRUE)

            IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN !CJK

                ! Setting the screen number and previous key
                PREVIOUS.KEY = TRUE
                SCREEN% = RESTORE.A.FILE.SCR%                           !CJK
            ENDIF ELSE BEGIN                                            !MJK
                IF VALUE.INDEX% = XRE.ZERO% THEN BEGIN                  !MJK
                    CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(21) + \      !IDC
                                   STATUS.TEXT.MSG$(58))                !IDC
                ENDIF ELSE BEGIN
                    IF RET.KEY% = ENTER.KEY% THEN BEGIN

                        ! Check the entry in day selection screen
                        SCREEN.NUM% = FILE.DIR.AND.DIST.SCR%            !IDC
                        GOSUB CHECK.DAY.SELECTION                       !IDC

                        ! If any error in function, set same screen
                        IF NOT FUNCTION.ERROR.NOT.EXIST THEN BEGIN      !EJK
                            SCREEN% = FILE.DAY.SELECT.SCR%              !EJK
                        ENDIF                                           !EJK

                    ENDIF ELSE BEGIN
                        ! B001 Invalid key pressed
                        CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))     !IDC
                    ENDIF
                ENDIF                                                   !MJK
            ENDIF

        ENDIF

        ! Confirmation screen
        IF SCREEN% = FILE.DIR.AND.DIST.SCR% THEN BEGIN                  !CJK

            ! Setting the Distribution type 3 as default
            FSEL$ = "3"
            ! Display the screen and process it
            GOSUB CHECK.AND.DISPLAY.FILE

        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* CHECK.AND.DISPLAY.FILE: This Subroutine checks the archived file for
\*                         for the selected day. If file exist in the
\*                         archive file, the screen will be processed
\*                         for confirmation, else an error message will
\*                         be presented.
\*
\***********************************************************************
CHECK.AND.DISPLAY.FILE:

    STATUS.MSG$ = BKP.DATE.ARRAY$(SEL.INDEX%) + STATUS.TEXT.MSG$(24) + \!JDC
                  " for restoration"
    GOSUB LOG.STATUS.MSG

    ! Display the confirmation screen
    CALL DM.SHOW.SCREEN(8, XRE.NULL$, 9, 9)                             !KDC

    ! Set the field for distribution type
    CALL DM.NAME (4, "FSEL$", FSEL$)

    ! Validation the selection is between 1 and 5
    !-------------------------------------------------
    CALL DM.VALID   ("FSEL$", "FSEL$ >= 1 AND FSEL$ <= 5")              !MJK
    CALL DM.MESSAGE ("FSEL$", "'Invalid Distribution type")

    ! Display the status when the file check is going to happen
    CALL DM.STATUS ("'Processing - Please wait...")

    ! Populating the File name and the directory
    FIELD$(2) = UCASE$(SELECTED.FILENAME$)
    FIELD$(3) = BKPSCRPT.DIRECTORIES$(SELECT.INDEX%)

    RESTORE.STATUS = FALSE

    ! Checking the file presence in the selected archive file
    GOSUB CHECK.THE.FILE.PRESENCE

    ! This field will be notified if file present
    FIELD$(1) = "'Please verify and confirm for restore"

        ! If file not exist
        IF NOT RESTORE.STATUS THEN BEGIN

            ! Making the input filed as Read only
            CALL DM.RO.FIELD(3)
            CALL DM.RO.FIELD(4)

            ! Setting the error to display
            FIELD$(1) = STATUS.TEXT.MSG$(56)                            !IDC

            STATUS.MSG$ = STATUS.TEXT.MSG$(41) + STATUS.TEXT.MSG$(55)   !MJK
            GOSUB LOG.STATUS.MSG

        ENDIF

    ! Handles the confirmation and restoration screen
    WHILE SCREEN% = FILE.DIR.AND.DIST.SCR% OR \                         !CJK
          SCREEN% = FILE.CONFIRMATION.SCR%                              !CJK

        IF SCREEN% = FILE.DIR.AND.DIST.SCR% THEN BEGIN                  !CJK

            ! If screen is accessed by F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN

                ! Display screen
                CALL DM.SHOW.SCREEN(8, XRE.NULL$, 9, 9)                 !KDC

                ! Setting the distribution type 3 as default            !MJK
                FSEL$ = "3"
                CALL DM.NAME (4, "FSEL$", FSEL$)

                ! Validation the selection is between 1 and 5
                !-------------------------------------------------
                CALL DM.VALID ("FSEL$", "FSEL$ >= 1 AND FSEL$ <= 5")
                CALL DM.MESSAGE ("FSEL$", "'Invalid Distribution type")

                ! Setting the File and directory value in field
                FIELD$(2) = UCASE$(SELECTED.FILENAME$)
                FIELD$(3) = DIRECT.TO.RESTORE$

                ! Setting space for status
                FIELD$(1) = XRE.SPACE$

                ! Reset the previous key
                PREVIOUS.KEY = FALSE

            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (3, 4, TRUE)

            IF (RET.KEY% = ESC.KEY%) OR  (RET.KEY% = F3.KEY%) THEN BEGIN

                ! Setting the screen number for F3 or ESC
                SCREEN%      = FILE.DAY.SELECT.SCR%                     !CJK
                PREVIOUS.KEY = TRUE

            ENDIF ELSE BEGIN                                            !MJK
                IF NOT RESTORE.STATUS THEN BEGIN                        !MJK
                    ! If selected file not exist in the archive
                    FIELD$(1) = STATUS.TEXT.MSG$(57)                    !OJK
                ENDIF ELSE BEGIN                                        !MJK
                    IF RET.KEY% = ENTER.KEY% THEN BEGIN                 !MJK
                    !--------------------------------------------------!
                    ! Checks the entered value again before proceeding !
                    ! with the file restore                            !
                    !--------------------------------------------------!
                        GOSUB RECHECK.DIRECTORY
                    ENDIF ELSE BEGIN
                        CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))     !IDC
                    ENDIF                                               !MJK
                ENDIF                                                   !MJK
            ENDIF
        ENDIF

        ! File restoration screen
        IF SCREEN% = FILE.CONFIRMATION.SCR% THEN BEGIN                  !CJK
            GOSUB CONFIRM.AND.RESTORE.FILE
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* CHECK.THE.FILE.PRESENCE: This Subroutine checks the selected file
\*                          presence in the archive file.
\*
\***********************************************************************
CHECK.THE.FILE.PRESENCE:

    ! Date to restore
    FULL.TO.MOVING.DATE$ = BKP.DATE.ARRAY$(SEL.INDEX%)                  !IDC

    GOSUB GET.FILE.EXTENSION                                            !IDC
    MDD.DATE$ = EXTENSION$                                              !IDC

    ! Setting the Backup files based on the 1st field which is drive
    IF LEFT$(FILENAME$,1) = "D" THEN BEGIN
        ! IMG and ALT directory for C drive
        BKP.FILENAME.IMG$ = C.BKP.IMG$ + FILENAME$ + "." + MDD.DATE$    !MJK
        BKP.FILENAME.ALT$ = C.BKP.ALT$ + FILENAME$ + "." + MDD.DATE$    !MJK
        ! BKPFAIL prefix                                                !MJK
        BKPFAIL.PREFIX$   = C.BKP.IMG$ + "BKPFAILD."                    !MJK
    ENDIF ELSE BEGIN
        ! IMG and ALT directory for D drive
        BKP.FILENAME.IMG$ = D.BKP.IMG$ + FILENAME$ + "." + MDD.DATE$    !MJK
        BKP.FILENAME.ALT$ = D.BKP.ALT$ + FILENAME$ + "." + MDD.DATE$    !MJK
        ! BKPFAIL prefix                                                !MJK
        BKPFAIL.PREFIX$   = D.BKP.IMG$ + "BKPFAILC."                    !MJK
    ENDIF

    ! Current BKPFAIL file
    BKPFAIL.CURR.FILE$ = BKPFAIL.PREFIX$ + MDD.DATE$                    !MJK

    VALUE.EXISTS   = FALSE
    RESTORE.STATUS = FALSE

    ! Getting the file details of the Directory archive
    IF FUNC.FILE.EXISTS(BKP.FILENAME.IMG$) THEN BEGIN
        CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -l "                    + \
                     BKP.FILENAME.IMG$ + " > " + DIR.OUT$            + \
                     " >>* " + DIR.OUT$ )
        VALUE.EXISTS = 1
        STATUS.MSG$  = "Checking the archive file " + BKP.FILENAME.IMG$ !MJK
    ENDIF ELSE BEGIN                                                    !NJK
        IF FUNC.FILE.EXISTS(BKP.FILENAME.ALT$) THEN BEGIN               !NJK
            CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -l "                + \
                         BKP.FILENAME.ALT$ + " > " + DIR.OUT$        + \
                         " >>* " + DIR.OUT$)
            VALUE.EXISTS = 2
            STATUS.MSG$  = "Checking the archive file " + \             !NJK
                            BKP.FILENAME.ALT$                           !NJK
        ENDIF
    ENDIF                                                               !NJK

    GOSUB LOG.STATUS.MSG

    !------------------------------------------!
    ! If BKPFAIL file present, check to see if !
    ! that has the selected file record in it  !
    !------------------------------------------!
    IF FUNC.FILE.EXISTS(BKPFAIL.CURR.FILE$) THEN BEGIN

        DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( BKPFAIL.CURR.FILE$ )  !NJK

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
        ! returns NULL when EOF reached or read error                 ! !FDC
        !-------------------------------------------------------------!
        WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
            DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )      !NJK

            ! Checking the comma position
            COMMA.POSITION% = MATCH(COMMA.VALUE$,DIR.VALUE$,1)

            ! If comma found
            IF COMMA.POSITION% <> XRE.ZERO% THEN BEGIN

                ! Storing the failed file
                FAILED.FILE$ = LEFT$(DIR.VALUE$,(COMMA.POSITION% - 1))

                ! If BKPFAIL file matches with current file
                IF UCASE$(FAILED.FILE$) = UCASE$(SELECT.FILE.NAME$) \
                THEN BEGIN
                    VALUE.EXISTS   = 3
                    RESTORE.STATUS = TRUE
                ENDIF
            ENDIF
        WEND

        ! Closing File
        IF DIR.OPEN THEN BEGIN
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !NJK
            DIR.OPEN = FALSE
        ENDIF

    ENDIF

    ! If archive file details are STDOUT to DIR.OUT file
    IF VALUE.EXISTS = 1 OR VALUE.EXISTS = 2 THEN BEGIN

        DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )            !NJK

        ! If file open unsuccessful
        IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
            DIR.OPEN    = FALSE                                         !MJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                         !JDC
            GOSUB LOG.STATUS.MSG
            ! Setting NULL to avoid file read
            DIR.VALUE$ = XRE.NULL$
        ENDIF ELSE BEGIN
            DIR.OPEN   = TRUE
            DIR.VALUE$ = XRE.SPACE$
        ENDIF

        ! Reading the file till it reached the EOF or read error        !FDC
        WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
            DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )      !NJK

            ! Compare the file names from DIR.OUT with Restore file
            IF LEN(DIR.VALUE$) > 29 THEN BEGIN ! If length is > 29
                IF MID$(DIR.VALUE$,30,(LEN(DIR.VALUE$) - 29)) = \
                   UCASE$(SELECTED.FILENAME$) THEN BEGIN
                    ! If file found enable the flag
                    RESTORE.STATUS = TRUE
                    ! To break the WHILE loop                           !OJK
                    DIR.VALUE$     = XRE.NULL$                          !OJK
                ENDIF
            ENDIF
        WEND

        ! Closing File
        IF DIR.OPEN THEN BEGIN
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !NJK
            DIR.OPEN = FALSE
        ENDIF
    ENDIF

RETURN

\***********************************************************************
\*
\* RECHECK.DIRECTORY: This Subroutine checks the value entered in the
\*                    confirmation screen.
\*
\***********************************************************************
RECHECK.DIRECTORY:

    ! Storing the Directory value
    DIRECT.TO.RESTORE$ = FIELD$(3)

    ! Trim it
    CALL TRIM(DIRECT.TO.RESTORE$)

    ! Checking whether the Directory is under C or D drive
    IF UCASE$(LEFT$(DIRECT.TO.RESTORE$,3)) <> "C:\" AND \
       UCASE$(LEFT$(DIRECT.TO.RESTORE$,3)) <> "D:\" THEN BEGIN

        CALL DM.STATUS ("'Incorrect directory value. Please use " + \
                        "C/D drive. Use backward slash (\)")

    ENDIF ELSE BEGIN                                                    !NJK
        ! Maximum allowed length for a directory value                  !OJK
        IF LEN(DIRECT.TO.RESTORE$) > 12 THEN BEGIN                      !OJK
            CALL DM.STATUS ("'Directory length is too high")            !OJK
        ! Checking whether any empty spaces in directory name           !OJK
        ENDIF ELSE IF MATCH(" ",DIRECT.TO.RESTORE$,1)  THEN BEGIN       !OJK
            CALL DM.STATUS ("'Directory name should not contain" + \    !OJK
                            " any spaces in between")                   !OJK
        ! Making sure that invalid characters are not allowed           !OJK
        ENDIF ELSE IF MATCH("""",DIRECT.TO.RESTORE$,1) OR \             !OJK
                      MATCH("\!",DIRECT.TO.RESTORE$,1) OR \             !OJK
                      MATCH("*",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH(".",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH("+",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH(",",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH("<",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH(">",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH("[",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH("]",DIRECT.TO.RESTORE$,1)  OR \             !OJK
                      MATCH("\?",DIRECT.TO.RESTORE$,1) THEN BEGIN       !OJK
            CALL DM.STATUS ("'Special/Operational characters are not" + \OJK
                            " recommended in Directory name")           !OJK
        ENDIF ELSE BEGIN

        ! If all the fields are fine, proceed to the restoration
            SCREEN% = FILE.CONFIRMATION.SCR%                            !CJK
            STATUS.MSG$ = DIRECT.TO.RESTORE$                         + \!NJK
                          " directory is selected " + "for file restore"!NJK
            GOSUB LOG.STATUS.MSG
        ENDIF
    ENDIF                                                               !NJK

RETURN

\***********************************************************************
\*
\* CONFIRM.AND.RESTORE.FILE: This Subroutine display the file restore
\*                           confirmation screen and then process it
\*
\***********************************************************************
CONFIRM.AND.RESTORE.FILE:

    ! Display the screen
    GOSUB DISPLAY.FILE.CONFIRMATION

    ! For Confirmation screen and final restoration screen
    WHILE SCREEN% = FILE.CONFIRMATION.SCR% OR \                         !CJK
          SCREEN% = FILE.PROCESS.SCR%                                   !CJK

        IF SCREEN% = FILE.CONFIRMATION.SCR% THEN BEGIN                  !CJK
            ! If screen accessed using F3 or ESC
            IF PREVIOUS.KEY THEN BEGIN
                GOSUB DISPLAY.FILE.CONFIRMATION
                PREVIOUS.KEY = FALSE
            ENDIF

            RET.KEY% = DM.PROCESS.SCREEN (5, 5, FALSE)

            IF (RET.KEY% = ESC.KEY%) OR (RET.KEY% = F3.KEY%) THEN BEGIN !CJK

                ! Previous screen
                SCREEN%      = FILE.DIR.AND.DIST.SCR%                   !CJK
                PREVIOUS.KEY = TRUE

            ENDIF ELSE IF RET.KEY% = ENTER.KEY% THEN BEGIN
                ! If the input is N, go back to the previous screen
                IF UCASE$(FIELD$(5)) = "N" THEN BEGIN
                    SCREEN%      = FILE.DIR.AND.DIST.SCR%               !CJK
                    PREVIOUS.KEY = TRUE
                ! If Y is entered
                ENDIF ELSE IF UCASE$(FIELD$(5)) = "Y" THEN BEGIN
                    ! Setting the screen number for restoration
                    SCREEN% = FILE.PROCESS.SCR%                         !CJK
                ENDIF ELSE BEGIN
                    ! Invalid entry
                    CALL DM.STATUS ("'Incorrect value entered. " + \
                                   "Enter Y or N ")
                ENDIF

            ENDIF ELSE BEGIN
                CALL DM.FOCUS ("1", STATUS.TEXT.ERROR$(20))             !IDC
            ENDIF
        ENDIF

        ! File restoration screen
        IF SCREEN% = FILE.PROCESS.SCR% THEN BEGIN                       !CJK
            GOSUB FILE.RESTORATION
        ENDIF
    WEND

RETURN

\***********************************************************************
\*
\* DISPLAY.FILE.CONFIRMATION: This Subroutine displays the confirmation
\*                            screen for file restoration.
\*
\***********************************************************************
DISPLAY.FILE.CONFIRMATION:

    CALL DM.SHOW.SCREEN(9, XRE.NULL$, 10, 10)                           !KDC

    ! Setting the required fields to display
    FIELD$(3) = "You are about to restore "                          + \
                UCASE$(SELECTED.FILENAME$) + " to "                  + \
                UCASE$(DIRECT.TO.RESTORE$) + " with the"

    FIELD$(4) = "distribution type of " + FSEL$

RETURN

\***********************************************************************
\*
\* FILE.RESTORATION: This Subroutine does the restoration of the file.
\*                   It uses ADXUNZIP to extract the file. If there is  !OJK
\*                   any extraction error then file will be extracted   !OJK
\*                   to C:/TEMP.                                        !OJK
\*
\***********************************************************************
FILE.RESTORATION:

    CALL DM.SHOW.SCREEN(10, XRE.NULL$, XRE.ZERO%, XRE.ZERO%)            !GJK

    !------------------------------------------!
    ! Display the processing notification, as  !
    ! the restore might take longer duration   !
    !------------------------------------------!
    CALL DM.STATUS ("'Processing - Please wait...")
    DIRECTORY.CREATE = TRUE                                             !HJK

    ! If the directory not exists, create it
    IF FUNC.DIR.NOT.EXISTS(DIRECT.TO.RESTORE$) THEN BEGIN

        ! Trim the last slash found to avoid error using MKDIR
        CALL TRIM   (DIRECT.TO.RESTORE$)
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("/"))
        CALL RTRIMC (DIRECT.TO.RESTORE$, ASC("\\"))

        CALL OSSHELL("MKDIR " + DIRECT.TO.RESTORE$ + " >  " + \         !HJK
                     DIR.OUT$ + " >>* "  + DIR.OUT$ )
        STATUS.MSG$ = DIRECT.TO.RESTORE$ + " directory is created"
! HJK Starting block                                                    !HJK
        DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )            !NJK

        ! If file open unsuccessful
        IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
            DIR.OPEN    = FALSE                                         !MJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                         !JDC
            GOSUB LOG.STATUS.MSG
            ! Setting NULL to avoid file read
            DIR.VALUE$ = XRE.NULL$
        ENDIF ELSE BEGIN
            DIR.OPEN   = TRUE
            DIR.VALUE$ = XRE.SPACE$
        ENDIF

        ! Read the file till EOF or read error                          !FDC
        WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
            DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )      !NJK

            !----------------------------------------!                  !OJK
            ! If error string matches, log the error !                  !OJK
            !----------------------------------------!                  !OJK
            IF MATCH("MKDIR",UCASE$(DIR.VALUE$),1) <> \                 !OJK
               XRE.ZERO% THEN BEGIN
                ! Error in creating directory
                STATUS.MSG$ = DIR.VALUE$                                !OJK
                GOSUB LOG.STATUS.MSG
                DIRECTORY.CREATE = FALSE
            ENDIF
        WEND

        IF DIRECTORY.CREATE THEN BEGIN
            STATUS.MSG$ = DIRECT.TO.RESTORE$ + " directory is created"
            GOSUB LOG.STATUS.MSG
        ENDIF ELSE BEGIN
            STATUS.MSG$ = DIRECT.TO.RESTORE$        + \
                          " directory is not created"
            GOSUB LOG.STATUS.MSG
        ENDIF

        ! Closing File
        IF DIR.OPEN THEN BEGIN
            CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                        !NJK
            DIR.OPEN = FALSE
        ENDIF
! HJK Ending block                                                      !HJK
    ENDIF

    ! Default value                                                     !OJK
    BKP.ZIP.FILE$     = XRE.NULL$                                       !OJK
    RESTORE.ERR.EXIST = FALSE                                           !OJK

    IF DIRECTORY.CREATE THEN BEGIN                                      !HJK

        IF RIGHT$(DIRECT.TO.RESTORE$,1) <> "\" THEN BEGIN
            !--------------------------------------------------!
            ! Adding slash for further processing if directory !
            ! value is not ending with slash                   !
            !--------------------------------------------------!
            DIRECT.TO.RESTORE$ = DIRECT.TO.RESTORE$ + "\"
        ENDIF

        ! If file present in IMG directory
        IF VALUE.EXISTS = 1 THEN BEGIN

! Commenting out the logic, as it is no longer needed                   !OJK
!            ! Extracting the archive file to C:/TEMP                   !OJK
!           CALL OSSHELL(ADXZUDIR.FILE.NAME$ + " -x "                + \!OJK
!                        BKP.FILENAME.IMG$ + XRE.SPACE$ + TEMP.DIR$  + \!OJK
!                        " > " + DIR.OUT$ + " >>* " + DIR.OUT$ )        !OJK

            ! Extracting the file using ADXUNZIP                        !OJK
            CALL OSSHELL(ADXUNZIP.FILE.NAME$ + " -C -o "       + \      !OJK
                         BKP.FILENAME.IMG$ + XRE.SPACE$        + \      !OJK
                         SELECTED.FILENAME$ + " -d "           + \      !OJK
                         DIRECT.TO.RESTORE$ + " > " + DIR.OUT$ + \      !OJK
                         " >>* " + DIR.OUT$ )                           !OJK

            STATUS.MSG$ = BKP.FILENAME.IMG$ + STATUS.TEXT.MSG$(46) + \  !JDC
                          "to get the file"
            GOSUB LOG.STATUS.MSG

            BKP.ZIP.FILE$ = BKP.FILENAME.IMG$                           !OJK

        ! If file present in ALT directory
        ENDIF ELSE IF VALUE.EXISTS = 2 THEN BEGIN

            ! Extracting the file using ADXUNZIP                        !OJK
            CALL OSSHELL(ADXUNZIP.FILE.NAME$ + " -C -o "       + \      !OJK
                         BKP.FILENAME.ALT$ + XRE.SPACE$        + \      !OJK
                         SELECTED.FILENAME$ + " -d "           + \      !OJK
                         DIRECT.TO.RESTORE$ + " > " + DIR.OUT$ + \      !OJK
                         " >>* " + DIR.OUT$ )                           !OJK

            STATUS.MSG$ = BKP.FILENAME.ALT$ + STATUS.TEXT.MSG$(46) + \  !JDC
                          "to get the file"
            GOSUB LOG.STATUS.MSG

            BKP.ZIP.FILE$ = BKP.FILENAME.ALT$                           !OJK

        ! If the file present in BKPFAIL
        ENDIF ELSE IF VALUE.EXISTS = 3 THEN BEGIN
            RESTORE.FILENAME$ = SELECTED.FILENAME$                      !LJK
            MATCH.POS% = MATCH(".", SELECTED.FILENAME$,1)               !LJK
            IF MATCH.POS% <> XRE.ZERO% THEN BEGIN                       !LJK
                RESTORE.FILENAME$ = \                                   !LJK
                            LEFT$(SELECTED.FILENAME$,(MATCH.POS% - 1))  !LJK
            ENDIF                                                       !LJK

            ! Selecting the XDISKIMG directory location based on the    !MJK
            ! given file for restore                                    !MJK
            IF LEFT$(UCASE$(SELECT.FILE.NAME$),1) = "D" THEN BEGIN
                ! Copying the file to respective directory
                CALL OSSHELL("COPY " + C.BKP.IMG$ + RESTORE.FILENAME$ + \LJK
                             "." + MDD.DATE$ + XRE.SPACE$             + \LJK
                             DIRECT.TO.RESTORE$ + SELECTED.FILENAME$  + \LJK
                             " > " + DIR.OUT$ + " >>* " + DIR.OUT$)
            ENDIF ELSE BEGIN                                            !MJK
                ! Copying the file to respective directory
                CALL OSSHELL("COPY " + D.BKP.IMG$ + RESTORE.FILENAME$ + \MJK
                             "." + MDD.DATE$ + XRE.SPACE$             + \MJK
                             DIRECT.TO.RESTORE$ + SELECTED.FILENAME$  + \MJK
                             " > " + DIR.OUT$ + " >>* " + DIR.OUT$)     !MJK
            ENDIF
            !---------------------------------------------!
            ! Not distributing the file if SUPPS mode, as !
            ! ADXCSU0L won't work under SUPPS             !
            !---------------------------------------------!
            IF NOT SUPPS.ON THEN BEGIN
                ! Setting the distribution type
                CALL OSSHELL(ADXCSU0L.FILE.NAME$ + " 3 " + FSEL$     + \!MJK
                             XRE.SPACE$ + DIRECT.TO.RESTORE$         + \
                             SELECTED.FILENAME$ + " >> " + DIR.OUT$  + \
                             " >>* " + DIR.OUT$)
            ENDIF

            STATUS.MSG$ = "File is being extracted from BKPFAIL file"
            GOSUB LOG.STATUS.MSG

        ENDIF

        ! If valid restore
        IF VALUE.EXISTS > XRE.ZERO% THEN BEGIN

            ! Opening the STDOUT file to check for any error
            DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )        !NJK

            ! If file open unsuccessful
            IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                DIR.OPEN    = FALSE                                     !MJK
                STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                     !JDC
                GOSUB LOG.STATUS.MSG
                ! Setting NULL to avoid file read
                DIR.VALUE$ = XRE.NULL$
            ENDIF ELSE BEGIN
                DIR.OPEN   = TRUE
                DIR.VALUE$ = XRE.SPACE$
            ENDIF

            ! Read the file till EOF or read error                      !FDC
            WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE( DIR.FILE.RC% )  !NJK

                !-----------------------------------------------!
                ! If error string matches, write the error with !
                ! file name in LOG file                         !
                !-----------------------------------------------!
                IF MATCH("ERROR",UCASE$(DIR.VALUE$),1) <> XRE.ZERO% OR  \OJK
                   MATCH("cannot be found",(DIR.VALUE$),1)  <>          \OJK
                   XRE.ZERO% THEN BEGIN
                    ! Logging the extraction error OR copy error
                    STATUS.MSG$ = DIR.VALUE$
                    GOSUB LOG.STATUS.MSG
                    ! Setting the status to show the notification
                    RESTORE.ERR.EXIST = TRUE                            !OJK

                    ! If BKPFAIL run
                    IF VALUE.EXISTS = 3 THEN BEGIN
                        STATUS.MSG$ = STATUS.TEXT.ERROR$(14)            !JDC
                        GOSUB LOG.STATUS.MSG
                    ENDIF ELSE BEGIN                                    !OJK
                        STATUS.MSG$ = STATUS.TEXT.ERROR$(15)            !OJK
                        GOSUB LOG.STATUS.MSG                            !OJK
                        ! Extracting the file using ADXUNZIP to C:/TEMP !OJK
                        CALL OSSHELL(ADXUNZIP.FILE.NAME$ + " -C -o "  + \OJK
                                     BKP.ZIP.FILE$ + XRE.SPACE$       + \OJK
                                     SELECTED.FILENAME$ + " -d "      + \OJK
                                     TEMP.DIR$ + " > " + DIR1.OUT$    + \OJK
                                     " >>* " + DIR1.OUT$ )              !OJK
                        STATUS.MSG$ = STATUS.TEXT.MSG$(48) + \          !OJK
                                      TEMP.DIR$ + " directory"          !OJK
                        GOSUB LOG.STATUS.MSG                            !OJK
                    ENDIF
                    ! To break the WHILE loop                           !OJK
                    DIR.VALUE$ = XRE.NULL$                              !OJK
                ENDIF
            WEND

            ! Closing File
            IF DIR.OPEN THEN BEGIN
                CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                    !NJK
                DIR.OPEN = FALSE
            ENDIF

        ENDIF

        ! If IMG or ALT directory run
        IF VALUE.EXISTS = 1 OR VALUE.EXISTS = 2 THEN BEGIN

            !------------------------------------------------!
            ! Not distributing the file if SUPPS mode, as    !
            ! ADXCSU0L won't work under SUPPS. Also make     !          !OJK
            ! sure no error has happened in file extraction  !          !OJK
            !------------------------------------------------!          !OJK
            IF NOT SUPPS.ON AND NOT RESTORE.ERR.EXIST THEN BEGIN        !OJK
                ! Setting the distribution type
                CALL OSSHELL(ADXCSU0L.FILE.NAME$ + " 3 " + FSEL$    + \ !MJK
                             XRE.SPACE$ + DIRECT.TO.RESTORE$        + \
                             SELECTED.FILENAME$ + " >  " + DIR.OUT$ + \ !OJK
                             " >>* " + DIR.OUT$)
!            ENDIF                                                      !OJK

                DIR.FILE.RC% = FUNC.OPEN.SEQUENTIAL.FILE( DIR.OUT$ )    !NJK

                ! If file open unsuccessful
                IF DIR.FILE.RC% <= XRE.ZERO% THEN BEGIN
                    DIR.OPEN    = FALSE                                 !MJK
                    STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                 !JDC
                    GOSUB LOG.STATUS.MSG
                    ! Setting NULL to avoid file read
                    DIR.VALUE$ = XRE.NULL$
                ENDIF ELSE BEGIN
                    DIR.OPEN   = TRUE
                    DIR.VALUE$ = XRE.SPACE$
                ENDIF

                ! Read the file till EOF or read error                  !FDC
                WHILE LEN(DIR.VALUE$) <> XRE.ZERO%
                    DIR.VALUE$ = FUNC.READ.SEQUENTIAL.FILE(DIR.FILE.RC%)!NJK

                    !-----------------------------------------------!
                    ! If error string matches, write the error with !
                    ! file name in LOG file                         !
                    !-----------------------------------------------!
                    IF MATCH("ERROR",UCASE$(DIR.VALUE$),1) <> \         !OJK
                       XRE.ZERO% THEN BEGIN
                        ! Logging the distribution error                !OJK
                        STATUS.MSG$ = STATUS.TEXT.ERROR$(16)            !OJK
                        GOSUB LOG.STATUS.MSG
                        STATUS.MSG$ = DIR.VALUE$
                        GOSUB LOG.STATUS.MSG

                        RESTORE.ERR.EXIST = TRUE                        !OJK
                        ! To break the WHILE loop                       !OJK
                        DIR.VALUE$ = XRE.NULL$                          !OJK
                    ENDIF
                WEND

                ! Closing File
                IF DIR.OPEN THEN BEGIN
                    CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                !NJK
                    DIR.OPEN = FALSE
                ENDIF
            ENDIF                                                       !OJK
        ENDIF
    ENDIF                                                               !HJK

    ! Setting the notification based on the restore status flag
    IF NOT RESTORE.ERR.EXIST AND DIRECTORY.CREATE THEN BEGIN            !OJK
        FIELD$(1) = "'File restoration process successful"

        STATUS.MSG$ = "File restoration successful"

        ! Restore complete message
        FIELD$(3) = "The file " + UCASE$(SELECTED.FILENAME$)         + \
                    " has been restored from "                       + \
                    UCASE$(DAY.ARRAY$(SEL.INDEX%)) + ", "            + \
                    RIGHT$(BKP.DATE.ARRAY$(SEL.INDEX%),2) + "/"      + \
                    MID$(BKP.DATE.ARRAY$(SEL.INDEX%),3,2) + " to "   + \!HJK
                    UCASE$(DIRECT.TO.RESTORE$)                          !HJK
        FIELD$(4) = "This file has been distributed as a type " + FSEL$ !GJK

    ENDIF ELSE BEGIN
        IF DIRECTORY.CREATE THEN BEGIN                                  !HJK
            FIELD$(1) = "'Unable to restore this file because it " + \  !GJK
                        "is currently locked by another program"
            STATUS.MSG$ = "File restored with an exception"             !OJK
            ! Restore complete message
            FIELD$(3) = "The file " + UCASE$(SELECTED.FILENAME$) + \    !OJK
                        " has been restored with an exception from "    !OJK

            FIELD$(4) = UCASE$(DAY.ARRAY$(SEL.INDEX%)) + ", "       + \ !OJK
                        RIGHT$(BKP.DATE.ARRAY$(SEL.INDEX%),2) + "/" + \ !OJK
                        MID$(BKP.DATE.ARRAY$(SEL.INDEX%),3,2)       + \ !OJK
                        " to " + UCASE$(DIRECT.TO.RESTORE$) + "."       !OJK
        ENDIF ELSE BEGIN                                                !HJK
            FIELD$(1) = "'Error in creating directory"                  !HJK

            STATUS.MSG$ = "File restore unsuccessful"                   !OJK

            ! Restore unsuccessful message                              !OJK
            FIELD$(3) = "The file " + UCASE$(SELECTED.FILENAME$) + \    !OJK
                        " is not restored to "                   + \    !OJK
                        UCASE$(DIRECT.TO.RESTORE$)                      !OJK

            FIELD$(4) = "Please re-enter the directory and try again."  !OJK
        ENDIF

    ENDIF

    GOSUB LOG.STATUS.MSG                                                !GJK

    FIELD$(2) = "File restoration complete"                             !MJK

    ! Setting the visibility
    CALL DM.VISIBLE ("5", STATUS.TEXT.MSG$(60))                         !IDC

    ! To Make sure no error occurs when F3 or ESC pressed
    RESTORE.STATUS = TRUE

    ! Handles the final restore screen
    WHILE SCREEN% = FILE.PROCESS.SCR%                                   !CJK
        IF SCREEN% = FILE.PROCESS.SCR% THEN BEGIN                       !CJK
            RET.KEY% = DM.PROCESS.SCREEN (5, 5, FALSE)

            IF RET.KEY% = ESC.KEY% THEN BEGIN                           !MJK

                ! Previous screen
                SCREEN%      = FILE.CONFIRMATION.SCR%                   !CJK
                PREVIOUS.KEY = TRUE

            ENDIF ELSE BEGIN
                CALL DM.FOCUS ("1", SCREEN.TEXT.MSG$(13) + \            !MJK
                                    "previous screen")                  !MJK
            ENDIF
        ENDIF
    WEND

RETURN

                !   MAIN.PROCESSING SPECIFIC ROUTINES ENDS   !
                !............................................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                       GENERIC ROUTINES                             *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************
\*
\* CHECK.FUN.RC2: If FUN.RC2% is zero, By-passes rest of                !MJK
\*                 procedure. Stops program.
\*
\***********************************************************************
CHECK.FUN.RC2:                                                          !IDC

    IF FUN.RC2% = XRE.ZERO% THEN RETURN  ! No error                     !IDC

    STATUS.MSG$  = "Ended: Error while opening " + PASSED.STRING$

    GOSUB PROGRAM.EXIT

RETURN

                        !   GENERIC ROUTINES ENDS   !
                        !...........................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                 TERMINATION SPECIFIC ROUTINES                      *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************
\*
\*    CLOSE.AND.DEALLOC.SESSIONS: This Sub-routine closes the active
\*                                sessions and De-allocates the session
\*                                numbers.
\*
\***********************************************************************
CLOSE.AND.DEALLOC.SESSIONS:

    ! Closing the pipe and deallocating the session if OPEN
    IF XBACK.OPEN THEN BEGIN                                            !MJK
        CLOSE XBACK.PIPE.SESS.NUM%
        XBACK.OPEN = FALSE                                              !JDC
        CALL SESS.NUM.UTILITY("C",XBACK.PIPE.SESS.NUM%,XRE.NULL$)       !MJK
    ENDIF

    ! Closing the Log if OPEN
    IF XRE.LOG.OPEN THEN BEGIN                                          !MJK
        CALL FUNC.CLOSE.FILE(XRE.FILE.RC%)                              !NJK
        XRE.LOG.OPEN = FALSE
    ENDIF

    ! Closing the pipe and deallocating the session if OPEN
    IF XRE.OPEN THEN BEGIN                                              !MJK
        CLOSE XRE.PIPE.SESS.NUM%
        XRE.OPEN = FALSE                                                !JDC
        CALL SESS.NUM.UTILITY("C",XRE.PIPE.SESS.NUM%,XRE.NULL$)         !MJK
    ENDIF

    ! Closing BKPLIST File if OPEN
    IF BKPLIST.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( BKPLIST.FILE.RC% )                        !NJK
        BKPLIST.OPEN = FALSE
    ENDIF

    ! Closing DIR File if OPEN
    IF DIR.OPEN THEN BEGIN
        CALL FUNC.CLOSE.FILE( DIR.FILE.RC% )                            !NJK
        DIR.OPEN = FALSE
    ENDIF

    ! Closing SLPCF File if OPEN                                        !IDC
    IF SLPCF.OPEN THEN BEGIN                                            !IDC
        CALL FUNC.CLOSE.FILE(SLPCF.SESS.NUM% )                          !NJK
        SLPCF.OPEN = FALSE                                              !IDC
    ENDIF                                                               !IDC

RETURN

\***********************************************************************
\*
\* CHAIN.TO.CALLER: Chain to Application or Program
\*
\***********************************************************************
CHAIN.TO.CALLER:

    ! Terminate if any active session present                           !FDC
    GOSUB TERMINATION                                                   !FDC

    ! If command mode, clears and stop the program
    IF COMMAND.MODE THEN BEGIN
        CLEARS
        GOSUB STOP.PROGRAM
    ENDIF

    CALL DM.STATUS ("'Processing - Please Wait .....")
    CALL DM.QUIT

    ! Current program and chaining program
    PSBCHN.APP = "C:/ADX_UPGM/XRESTORE.286"                             !MJK
    PSBCHN.PRG = "C:/ADX_UPGM/" + CHAIN.TO.PROG$ + ".286"               !MJK

    %INCLUDE PSBCHNE.J86          ! Include CHAIN operation

    GOSUB STOP.PROGRAM

RETURN

\***********************************************************************
\*
\* PROGRAM.EXIT: Log the error, Deallocate the session and then chain
\*               back to the caller.
\*
\***********************************************************************
PROGRAM.EXIT:

    GOSUB LOG.STATUS.MSG
    GOSUB CHAIN.TO.CALLER

RETURN
                !   TERMINATION SPECIFIC ROUTINES ENDS   !
                !........................................!

\***********************************************************************
\*
\* ERROR.DETECTED:    Main Error Handling Routine. Starts with the
\*                    resume error conditions following ERROR.COUNT%
\*                    check to avoid error loop. Also References
\*                    STANDARD.ERROR.DETECTED to log Event 101.
\*
\***********************************************************************
ERROR.DETECTED:

    IF ERR = "CU" OR ERR = "DU" THEN RESUME     ! Close and delete
                                                ! session errors
    ! Incrementing the error count
    ERROR.COUNT% = ERROR.COUNT% + 1

    ! Handling infinite loop
    IF ERROR.COUNT% > 1 THEN BEGIN
        RESUME STOP.PROGRAM
    ENDIF

    ! When program runs in command, this error catch will be used
    IF ERR = "NP" THEN BEGIN
        COMMAND.MODE = -1                   ! TRUE
        ERROR.COUNT% = ERROR.COUNT% - 1     ! Non Fatal error
        RESUME START.OF.PROGRAM
    ENDIF

    ! Error creating XBACKUP pipe or XRESTORE pipe
    IF ERR = "ME" THEN BEGIN

        IF ERRF% = XBACK.PIPE.SESS.NUM% THEN BEGIN
            XBACK.OPEN  = FALSE                                         !JDC
            STATUS.MSG$ = "RESTORE not allowed when XBACKUP is active"
            ERROR.MSG$  = STATUS.MSG$
            GOSUB LOG.STATUS.MSG
            ERROR.EXIST  = TRUE                                         !MJK
            ERROR.COUNT% = ERROR.COUNT% - 1  ! Non Fatal error
            RESUME XBACK.PIPE.CREATE.ERROR                              !MJK
        ENDIF ELSE IF ERRF% = XRE.PIPE.SESS.NUM% THEN BEGIN             !MJK
            XRE.OPEN    = FALSE                                         !MJK
            STATUS.MSG$ = "XRESTORE is already active somewhere"
            ERROR.MSG$  = STATUS.MSG$
            GOSUB LOG.STATUS.MSG
            ERROR.EXIST  = TRUE
            ERROR.COUNT% = ERROR.COUNT% - 1  ! Non Fatal error
            RESUME XRE.PIPE.CREATE.ERROR                                !MJK
        ENDIF ELSE IF ERRF% = XRE.LOG.SESS.NUM% THEN BEGIN              !MJK
            XRE.LOG.OPEN = FALSE                                        !MJK
            STATUS.MSG$  = "Unable to create XRESTORE log"              !MJK
            ERROR.MSG$   = STATUS.MSG$                                  !MJK
            GOSUB LOG.STATUS.MSG                                        !MJK
            ERROR.EXIST  = TRUE                                         !MJK
            ERROR.COUNT% = ERROR.COUNT% - 1  ! Non Fatal error          !MJK
            RESUME XRE.LOG.CREATE.ERROR                                 !MJK
        ENDIF                                                           !MJK

    ENDIF

    STATUS.MSG$ = "Ended: " + ERR + " " + ERRNH + " ERRL "  + \
                  STR$(ERRL) + " ERRF% " + STR$(ERRF%)
    IF SUPPS.ON THEN BEGIN                                              !JDC
        CLEARS                                                          !MJK
        PRINT STATUS.MSG$                                               !JDC
    ENDIF ELSE BEGIN                                                    !JDC
        ! Log event 102
        CALL STANDARD.ERROR.DETECTED(ERRN, ERRF%, ERRL, ERR)            !DJK
    ENDIF                                                               !JDC
    ! Error message to log
    GOSUB PROGRAM.EXIT ! updates LOG and Program ends
END

\***********************************************************************
\***********************************************************************
\*
\*    End of program XRESTORE
\*
\***********************************************************************
\***********************************************************************

