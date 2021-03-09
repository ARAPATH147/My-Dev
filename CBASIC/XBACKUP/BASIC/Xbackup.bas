\***********************************************************************
\*
\* Program: XBACKUP          Jaya Kumar Inbaraj              27/01/2014
\*
\* FOD260 - Enhanced Backup and Recovery
\*
\* This program will compress files in directories and the 3 digit
\* extension will be constructed using the hex value of the
\* month as the 1st digit and the date of the month as the last 2 digits
\* of the extension e.g. a file created on the 1st of December would
\* have the extension C01. There will be some changes made to the cross
\* disk backup configuration file BKPSCRPT. A new directory will be
\* created on both controller hard drives named XDISKALT. This directory
\* will be used to backup directories when the secondary controller is
\* configured to run the store.
\*
\* The application is scheduled to start by SLEEPER every day with a
\* passed flag to indicate a Full or Incremental backup. The
\* application reads in the defined backup script, which has a list of
\* files and directories to archive as well as the location of the
\* archive.  Each archive will have the date and month in hex
\* (i.e. 1st December = C01) as an extension. Archives are kept for a
\* configurable amount of time, currently 14 days' worth of backups
\* will be available.
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
\* PASSED PARAMTERS
\* ================
\* Parameters  : SLEEPER OR "" (If SUPPORT run)
\* plus either   F             for Full Backup
\*      or       I             for an Incremental Backup
\*
\* INPUT AND OUTPUT FILES
\* ======================
\* Input files : BKPSCRPT.TXT  (Backup Script File)
\*               BKPEXCL.DAT   (Backup Exclude File)
\*               BKPLIST.MDD   (Backup List File)
\*
\* Output files: XBACKUP.LOG   (XBACKUP Log File)
\*               BKPLIST.MDD   (Backup List File)
\*               BKPFAILC.MDD  (Backup Fail File for C drive)
\*               BKPFAILD.MDD  (Backup Fail File for D drive)
\*               BKPEXCL.DAT   (Backup Exclude File)
\*
\*======================================================================
\*                   V E R S I O N   C O N T R O L
\* PLEASE UPDATE RUN VERSION NUMBER STATUS.TEXT.MSG$(0)
\*======================================================================
\*
\* Version B               Jaya kumar Inbaraj                 04/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to the changes in XBKOK file format.
\* Added ONS Re-run option. Also updated the coding standards with
\* respect to internal review comments.
\*
\* Version C               Jaya kumar Inbaraj                 10/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Worked on Internal review comments and coding standards
\* Commented out the Minor error flag usage in XBKOK.
\*
\* Version D               Jaya kumar Inbaraj                 24/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Updated the code with respect to the CR2 changes.
\*
\* Version E               Jaya kumar Inbaraj                 29/04/2014
\* FOD260 - Enhanced Backup and Recovery
\* Renamed few function names based on Internal review comments
\*
\* Version F               Jaya kumar Inbaraj                 08/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Worked on Application Management Team review comments
\*
\* Version G               Jaya kumar Inbaraj                 14/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Worked on Application Management Team review comments
\*
\* Version H               Dave Constable                     16/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Application Management Team review comments
\*
\* Version I     Jaya kumar Inbaraj / Dave Constable          21/05/2014
\* FOD260 - Enhanced Backup and Recovery
\* Internal and Application Management Team review comments
\* QC659 - corrected for files with no extension
\* QC650 - added create for missing XBKOK file
\*
\* Version J               Jaya kumar Inbaraj                 03/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC677 - Corrected the BKPSCRPT start time validation
\* Corrected the parameter validation in Command mode.
\* Updated the code for older files purging.
\*
\* Version K                  Dave Constable                  03/06/2014
\* FOD260 - Enhanced Backup and Recovery changes to enable CR for
\* configurable Full backup day and code review changes
\* QC 717 - force BKPFAIL on locked files at backup also
\*
\* Version L                  Dave Constable                  25/06/2014
\* FOD260 - Enhanced Backup and Recovery
\* Code review changes; alignment of file open status,
\* QC 719 - opened the BKPLIST file as locked to avoid being unable to
\* write later
\* QC 808 - correctly ignore forcing next day if null set in script
\*
\* Version M                  Dave Constable                  14/07/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC809 - corrected case to match on exclusions
\*
\* Version N               Jaya kumar Inbaraj                 01/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC 717 - Variable issues in storing distribution type is corrected.
\* QC 719 - Locked the BKPLIST file throughout the program.
\* QC 724 - Current BKPLIST file is excluded from BACKUP
\* QC 947 - Adjusted the logic to correct the BACKUP flow.
\*
\* Version O               Jaya kumar Inbaraj                 22/08/2014
\* FOD260 - Enhanced Backup and Recovery
\* CR5 changes to have configuration files in both C and D drives. Also
\* BKPFAIL.MDD file has been replaced with BKPFAILC.MDD and BKPFAILD.MDD
\* to have separate BKPFAIL file for C and D drive.
\* Also incorporated APPS management review comments and Internal
\* review comments.
\*
\* Version P               Jaya kumar Inbaraj                 01/09/2014
\* FOD260 - Enhanced Backup and Recovery
\* QC1124 - BKPSCRPT end time processing issue is fixed.
\*
\* Version Q              Dave Constable                      08/09/2014
\* FOD260 - Enhanced Backup and Recovery
\* Corrected version marker (should have been P, was O, now Q)
\* Array overflow seen in store 499 pilot. Added formatting to LOG to
\* show total in each array count and the total.
\* Reset of ARRAY.INDEX to the limit if second array in use.
\*
\* Version R               Jaya kumar Inbaraj                 09/09/2014
\* FOD260 - Enhanced Backup and Recovery
\* Allowed the background program to pass F/I also as parameter.
\*
\* Version S               Marc Hudson                        30/12/2014
\* To get this in support of the Service Desk i've fixed an issue that
\* would causes backup to error and stop. The error is when it tries to
\* open a file that doesn't exist..
\*
\* Version T              Sreemol Mini                        10/03/2017
\* Its noted that, XBACKUP hangs during lot of situations and doesn't 
\* complete in scheduled time and goes upto 10.00am in some cases.
\* As a result many JOBOK,POGOK failures reported. Issue is due to 
\* millions of open and close happens on BKPEXCL.DAT, the number of
\* open and close will increase based on total files available in 
\* in the directory plus the predefined exclusion list.Updated the   
\* code to use an array instead to avoide open and close also moved   
\* the BKPEXCL.DAT file location to W: drive for faster performance. 
\*
\* Version U               Ranjith Gopalankutty              30/06/2017
\* Further enhancement has been made so that full backup is executed   
\* every day rather than incremental backup, as there are hard coded   
\* variables in the code which decides the execution day. code loop    
\* will go in to incremental backup even if opted for full backup if   
\* the run day is not sunday. Made the changes so that, full backup    
\* is executed on the configured  days in sleeper. Also removed hard   
\* coding so that, we can decide the number of days of execution using 
\* sleeper rather than changing the code.                              
\* Access to BKPLIST files has been taken off, as no more incremental  
\* backups needs to be performed. We don't need to maintain the file   
\* going forward
\*
\***********************************************************************

\***********************************************************************
\*
\*    Included code defining file related global variables
\*
\***********************************************************************
    %INCLUDE BKPLIDEC.J86    ! BKPLIST function declaration
    %INCLUDE HSIUFDEC.J86    ! OS FTP function declaration              !KDC
    %INCLUDE SLPCFDEC.J86    ! Sleeper control                          !KDC
    %INCLUDE XBKOKDEC.J86    ! XBKOK function declaration

\***********************************************************************
\*
\*    Included code defining function related global variables
\*
\***********************************************************************
    %INCLUDE PSBF01G.J86     ! Application log
    %INCLUDE PSBF02G.J86     ! Update Date
    %INCLUDE PSBF13G.J86     ! PSDATE function
    %INCLUDE PSBF20G.J86     ! Session number Utility
\***********************************************************************
\*
\*    Global variable definitions
\*
\***********************************************************************
    STRING GLOBAL                   \
        FILE.OPERATION$

    INTEGER*2 GLOBAL                \
        CURRENT.REPORT.NUM%,        \
        FILE.RC2%,                  \
        RETURN.VALUE.CHECK%         !                                   !KDC

\***********************************************************************
\*
\*    Local Variables
\*
\***********************************************************************
    STRING                          \
        ADX.PARM.2$,                \
        BACKUP.DAYS$,               \                                   !KDC
        BKPEXCL.FILE.NAME$,         \
        BKPEXCL.FORM$,              \
        BKPEXCL.VALUE$,             \
        BKPFAIL.FILE$,              \
        BKPFAILC.FILE.NAME$,        \                                   !OJK
        BKPFAILD.FILE.NAME$,        \                                   !OJK
        BKPFAIL.FORM$,              \
        BKPLIST.COMP.FILENAME$,     \
        BKPLIST.FILE.NAME.C$,       \                                   !OJK
        BKPLIST.FORM$,              \
        BKPLIST.RECD$,              \                                   !KDC
        BKPSCRPT.COMMAND$,          \
        BKPSCRPT.DAYS.TO.KEEP$,     \
        BKPSCRPT.DIRECTORY$,        \
        BKPSCRPT.END.TIME$,         \                                   !PJK
        BKPSCRPT.FILE.EXCLUSION$,   \
        BKPSCRPT.FILE.NAME$,        \
        BKPSCRPT.OUT.FILE.NAME$,    \
        BKPSCRPT.START.TIME$,       \
        BKPSCRPT.VALUE$,            \
        C.DRIVE.ALTERNATE.DIRECTORY$,   \                               !KDC
        C.DRIVE.IMAGE.DIRECTORY$,   \                                   !KDC
        CE.CNTR$,                   \  ! CE value                       !CJK
        CE.C.XDISKALT$,             \  ! Renamed the variable           !FJK
        CE.C.XDISKIMG$,             \  ! Renamed the variable           !FJK
        CE.D.XDISKALT$,             \  ! Renamed the variable           !FJK
        CE.D.XDISKIMG$,             \  ! Renamed the variable           !FJK
        CE.NODE.NAME$,              \                                   !KDC
        CF.CNTR$,                   \  ! CF value                       !CJK
        CF.C.XDISKALT$,             \  ! Renamed the variable           !FJK
        CF.C.XDISKIMG$,             \  ! Renamed the variable           !FJK
        CF.D.XDISKALT$,             \  ! Renamed the variable           !FJK
        CF.D.XDISKIMG$,             \  ! Renamed the variable           !FJK
        CF.NODE.NAME$,              \                                   !KDC
        CNTR.DIR$,                  \                                   !IDK
        CNTLR.ID$,                  \  ! Current Controller node        !CJK
        COMMA.VALUE$,               \
        COMMAND.STRING$,            \
        CONFIGURED.NODES$,          \
        CONSTANT.DAYS$(1),          \                                   !LDC
        CRLF$,                      \
        CURR.DATE$,                 \
        CURR.TIME$,                 \
        D.DRIVE.IMAGE.DIRECTORY$,   \                                   !KDC
        D.DRIVE.ALTERNATE.DIRECTORY$,   \                               !KDC
        DATE.VALUE$,                \  ! Renamed the variable           !FJK
        DIR.FILE.NAME$,             \  ! File name from DIR command     !CJK
        DIR.OUT$,                   \  ! DIR command output file        !CJK
        DIR.SEC.OUT$,               \  ! Renamed the variable           !BJK
        DIREC.TO.SEARCH$,           \  ! Renamed the variable           !FJK
        DIR.VALUE$,                 \  ! DIR command line value         !CJK
        DIR.SEC.VALUE$,             \  ! DIR command line value         !BJK
        DIRECTORY.SEARCH$,          \  ! Directory value                !CJK
        DIRECTORY.SEARCH.VALUE$,    \  ! Search inside Directory        !CJK
        DIST.TYPE$,                 \  ! Distribution type              !CJK
        DRIVE$,                     \
        EXT.MDD$,                   \  ! Extension MDD                  !CJK
        EXTENSION$,                 \                                   !KDC
        FIELDS$(1),                 \                                   !KDC
        FILE.EXT.VALUE$,            \  ! Renamed the variable           !FJK
        FILE.HAS.CHANGED$,          \
        FILE.IS.PRESENT$,           \
        FILE.NAME.VALUE$,           \  ! Renamed the variable           !FJK
        FTP.FILE.NAME$,             \
        FTP.PASSWORD$,              \  ! Extracted password from OS file!KDC
        FTP.USER$,                  \                                   !KDC
        FTPOUT.FILE.NAME$,          \
        FULL.DATE$,                 \                                   !KDC
        FULL.DAY$,                  \                                   !KDC
        FULL.DEL.EXT.MDD$,          \  ! Full Extension to delete       !KDC
        FULL.EXT.MDD$,              \  ! Full Extension MDD             !KDC
        FUNC.FLAG$,                 \
        HHMM.STATUS.MSG$,           \
        IP.ADDRESS$,                \
        MASTER$,                    \  ! Current Master controller      !CJK
        MONTH$,                     \
        PARM.BACKGRND$,             \                                   !RJK
        PARM.BACKGRND.FULL$,        \                                   !RJK
        PARM.BACKGRND.INC$,         \                                   !RJK
        PARM.FULL$,                 \                                   !KDC
        PARM.INCREMENTAL$,          \                                   !KDC
        PARM.ONS$,                  \                                   !KDC
        PARM.RERUN.FULL$,           \                                   !DJK
        PARM.RERUN.INC$,            \                                   !DJK
        PARM.SLEEPER$,              \                                   !KDC
        PARM.SLEEPER.FULL$,         \                                   !DJK
        PARM.SLEEPER.INC$,          \                                   !DJK
        PASSED.STRING$,             \
        PREFIX.HHMMSS.PROG$,        \  ! Included new variable          !FJK
        PROGRAM$,                   \
        PSDATE.DATE$,               \                                   !KDC
        REMAINING.VALUE$,           \
        RUN.ALLOWED$,               \                                   !URG
        RUN.TYPE$,                  \  ! To capture Backup run type     !DJK
        STATUS.END$,                \
        STATUS.MAJOR.ERROR$,        \
        STATUS.MSG$,                \
        STATUS.TEXT.MSG$(1),        \                                   !KDC
        STATUS.TEXT.ERROR$(1),      \                                   !KDC
        STATUS.START$,              \
        TEMP.DIRECTORY.NAME$,       \                                   !KDC
        TEMP.FILE.NAME$,            \
        TODAY.BKPLIST.REC$,         \                                   !KDC
        TODAY.BKPLIST.REC.ERR$,     \                                   !KDC
        TODAY.DATE$,                \
        VAR.STRING.1$,              \
        VAR.STRING.2$,              \
        VERSION$,                   \                                   !OJK
        XBACK.LOG.FILE.NAME$,       \
        XBACK.LOG.FORM$,            \
        XBACK.LOG.LIVE.PATH$,       \
        XBACK.LOG.REC$,             \
        XBACK.NULL$,                \
        XBACK.PIPE.NAME$,           \
        XBACK.YES$,                 \
        XBKLOG.VALUE$,              \
        XBKOK.INTERIM.STATUS$,      \  ! Renamed the variable           !BJK
        XBKPFTP.FORM$,              \  ! FTP form                       !CJK
        XBKPFTP.LINE$,              \  ! FTP line                       !CJK
        XBKTEMP.LOG$                   ! Renamed the variable           !FJK

    ! Grouping Arrays                                                   !CJK
    STRING                          \
        BKPEXCL.ARRAY$(1),          \ Array holding exclude files       !TSM
        BKPEXCL.COMP.ARRAY$(1),     \ Array holding entire exclude files!TSM
        BKPLIST.ARRAY$(1),          \
        BKPLIST.SECOND.ARRAY$(1)       ! Renamed the variable           !BJK

    ! local integer 1 variables
    INTEGER*1                       \
        ADX.FUNCTION%,              \
        ADX.INTEGER%,               \
        BKPSCRPT.HH%,               \                                   !KDC
        BKPSCRPT.MM%,               \                                   !KDC
        BKPSCRPT.END.HH%,           \                                   !PJK
        BKPSCRPT.END.MM%,           \                                   !PJK
        CNTLR.CONFIG%,              \
        COMMA.POSITION%,            \
        CURR.HH%,                   \                                   !KDC
        CURR.MM%,                   \                                   !KDC
        DAYS.AFTER.FULL.BAKUP%,     \                                   !KDC
        ERROR.COUNT%,               \
        FILE.CHECK.1%,              \                                   !KDC
        FILE.CHECK.2%,              \                                   !KDC
        FILE.CHECK.3%,              \                                   !KDC
        FILE.CHECK.4%,              \                                   !KDC
        FULL.BACKUP.NOT.FOUND%,     \                                   !KDC
        MASTER.AND.FILE.SERVER%,    \
        NODE.POSITION%,             \
        NUM.OF.ARRAYS%,             \  ! Included new variable          !FJK
        RUN.ALLOWED%,               \                                   !URG
        SLASH.POSITION%,            \
        SLEEPER.DAY%,               \                                   !KDC
        TODAY.BKPLIST.REC%,         \                                   !LDC
        XBACK.ZERO%
        
    ! Grouping Boolean variables
    INTEGER*1                       \
        ALT.EXISTS,                 \  ! XDISKALT directory exist       !CJK
        ALT.MASTER.ON,              \  ! CF present                     !CJK
        BACKGROUND.RUN,             \
        BACKUP.DIR.EXIST,           \                                   !KDC
        BACKUP.OFF,                 \
        BKPEXCL.OPEN,               \
        BKPEXCL.RUN,                \
        BKPFAILC.OPEN,              \                                   !OJK
        BKPFAILD.OPEN,              \                                   !OJK
        BKPLI.OPEN,                 \  ! Renamed the variable           !FJK
        BKPSCRPT.ERROR,             \
        BKPSCRPT.OPEN,              \
        COMMA.PRESENT,              \                                   !KDC
        DAY.SINCE.FULL%,            \                                   !KDC
        EXCLUDE.PRESENT,            \
        FALSE,                      \
        FILE.MISSING,               \                                   !SMH
        FILE.PRESENT,               \
        FIRST.ARRAY.FOUND,          \
        FSET.ON,                    \  ! FSET command allowed           !CJK
        FTP.SUCCESS,                \                                   !KDC
        IMG.EXISTS,                 \  ! XDISKIMG directory exist       !CJK
        INCLUDE.RUN,                \  ! Backup process                 !CJK
        IS.ARCHIVE.ON,              \                                   !HDC
        OPEN.BKPFAILFILE,           \                                   !SMH
        SECOND.ARRAY.FOUND,         \
        SECOND.ARRAY.ON,            \
        SLPCF.OPEN,                 \                                   !KDC
        TEMP.2.OPEN,                \  ! Temp variable 2                !FJK
        TEMP.OPEN,                  \
        TMP.EXISTS,                 \  ! C:\TEMP directory exist        !CJK
        TRUE,                       \
        VALUE.PRESENT,              \  ! Value present                  !CJK
        VALUE.EXISTS,               \
        VALUE.EXISTS.2,             \                                   !HDC
        XBACK.LOG.OPEN,             \
        XBACK.OPEN,                 \
        XBKOK.OPEN,                 \                                   !KDC
        ZIP.FILE.EXISTS             !                                   !KDC
                        
    ! local integer 2 variables
    INTEGER*2                       \
        ARRAY.INDEX%,               \
        ARRAY.LIMIT%,               \
        ARRAY.SECOND.INDEX%,        \  ! Renamed the variable           !BJK
        BEGIN.POS%,                 \                                   !KDC
        BKPEXCL.ARRAY.INDEX%,       \ Exclude array's index             !TSM
        BKPEXCL.ARRAY.LIMIT%,       \ Exclude array's limit             !TSM
        BKPEXCL.COMP.INDEX%,        \ Complete exclude array's index    !TSM
        BKPEXCL.COMP.LIMIT%,        \ Complete exclude array's limit    !TSM
        BKPEXCL.COMP.POS%,          \ For holding current index number  !TSM
                                    \ of complete exclude array         !TSM
        BKPEXCL.INDEX%,             \ For holding current index number  !TSM
                                    \ of exclude array                  !TSM
        BKPEXCL.REPORT.NUM%,        \
        BKPEXCL.SESS.NUM%,          \
        BKPFAILC.REPORT.NUM%,       \                                   !OJK
        BKPFAILD.REPORT.NUM%,       \                                   !OJK
        BKPFAILC.SESS.NUM%,         \                                   !OJK
        BKPFAILD.SESS.NUM%,         \                                   !OJK
        BKPSCRPT.REPORT.NUM%,       \
        BKPSCRPT.SESS.NUM%,         \
        COMMA.POSITION.2%,          \  ! Renamed the variable           !BJK
        EVENT.NUMBER%,              \
        FILE.POSITION%,             \
        INDEX%,                     \
        MATCH.POS%,                 \                                   !KDC
        MESSAGE.NUMBER%,            \
        PASSED.INTEGER%,            \
        SECOND.INDEX%,              \  ! Renamed the variable           !BJK
        TEMP.REPORT.NUM%,           \
        TEMP.REPORT.NUM.2%,         \  ! Renamed the variable           !BJK
        TEMP.SESS.NUM%,             \
        TEMP.SESS.NUM.2%,           \  ! Renamed the variable           !BJK
        XBACK.LOG.REPORT.NUM%,      \
        XBACK.LOG.SESS.NUM%,        \
        XBACK.PIPE.REPORT.NUM%,     \
        XBACK.PIPE.SESS.NUM%

    ! local integer 4 variables
    INTEGER*4 ADXSERVE.RC%

\***********************************************************************
\*
\*    EXT included code defining file related external functions
\*
\***********************************************************************
    %INCLUDE BKPLIEXT.J86    ! BKPLIST function definition
    %INCLUDE XBKOKEXT.J86    ! XBKOK function definition

\***********************************************************************
\*
\*    Included code defining external Boots functions
\*
\***********************************************************************
    %INCLUDE ADXCOPY.J86     ! To copy files
    %INCLUDE ADXSERVE.J86    ! ADXSERVE function
    %INCLUDE BASROUT.J86     ! OSShell function
    %INCLUDE BTCSTR.J86      ! String functions
    %INCLUDE CMPDATE.J86     ! Compare date utility
    %INCLUDE ERRNH.J86       ! Converts ERRN to 8-byte ASCII string
    %INCLUDE PSBF01E.J86     ! Application log
    %INCLUDE PSBF02E.J86     ! Update Date
    %INCLUDE PSBF13E.J86     ! PSDATE function
    %INCLUDE PSBF20E.J86     ! Session number utility
    %INCLUDE PSBF24E.J86     ! Standard error detected
    %INCLUDE HSIUFEXT.J86    ! OS FTP function declaration              !KDC
    %INCLUDE SLPCFEXT.J86    ! sleeper control                          !KDC

\***********************************************************************
\*
\*    ADXDATE  : This Sub-program is directly referenced from
\*               "Programming Guide" (Version 6 Release 3). It
\*               uses the ADXDATE and returns the System date
\*               in YYYYMMDD format.
\*
\***********************************************************************
SUB ADXDATE(RC, BUFFER) EXTERNAL
    INTEGER*4 RC
    STRING BUFFER
END SUB

\***********************************************************************
\*
\*    FUNC.IS.VALID.TIME(FIELD$):This Function is passed a string and
\*                               returns true (non zero) if it is a
\*                               valid time in the format of HHMM
\*
\***********************************************************************
FUNCTION FUNC.IS.VALID.TIME(FIELD$)                                     !KDC
    INTEGER*2       FUNC.IS.VALID.TIME                                  !KDC
    INTEGER*2       VALUE%                                              !KDC
    STRING          FIELD$                                              !KDC
                                                                        !KDC
ON ERROR GOTO ERROR.TRAP
    ! default as valid
    FUNC.IS.VALID.TIME = -1

    VALUE% = VAL(FIELD$)

    !------------------------------------------------------------!      !OJK
    ! Making sure that it's numeric value between 0 and 2359 and !      !OJK
    ! length of the field is equal to 4 else it's invalid time   !      !OJK
    !------------------------------------------------------------!      !OJK
    IF VALUE% < 0 OR VALUE% > 2359 OR LEN(FIELD$) <> 4 THEN BEGIN       !OJK
        FUNC.IS.VALID.TIME = 0
    ENDIF ELSE BEGIN

        IF VAL(LEFT$(FIELD$,2))  > 23 OR VAL(RIGHT$(FIELD$,2)) > 59 \   !OJK
        THEN BEGIN                                                      !OJK
            FUNC.IS.VALID.TIME = 0
        ENDIF

    ENDIF

!    FIELD$ = ""                                                        !OJK

FUNC.EXIT:

EXIT FUNCTION

ERROR.TRAP:

! Any error in the function is assumed as INVALID time and hence        !OJK
! commenting out the ERR check                                          !OJK
!    IF ERR = "IH" THEN BEGIN                                           !OJK
    FUNC.IS.VALID.TIME = 0
    RESUME FUNC.EXIT
!    ENDIF                                                              !OJK

END FUNCTION                                                            !KDC


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

    GOSUB INITIALISATION
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION

! Called during abnormal run
STOP.PROGRAM:
    STOP

\***********************************************************************
\*
\*    INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************
INITIALISATION:

    GOSUB INITIALISE.VARIABLES
    GOSUB CHECK.PARAM
    GOSUB CONTROLLER.CONFIG.CHECK
    GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB CREATE.RUN.PIPE
    GOSUB CHECK.BKP.DIRECTORIES
    GOSUB CREATE.XBACKUP.LOG
    GOSUB PROG.RUN.MODE
    GOSUB GET.SLEEPER.CONFIGURATION                                     !KDC
    GOSUB CREATE.FILES
    GOSUB OPEN.AND.READ.XBKOK
    GOSUB UPDATE.XBKOK

RETURN

\***********************************************************************
\*
\*    MAIN.PROCESSING : This Sub-routine does the main processing for
\*                      creating the Backup.
\*
\***********************************************************************
MAIN.PROCESSING:

  ! GOSUB BKPLIST.FULL.CHECK                                            !URG KDC
    GOSUB PROCESS.BKPSCRPT
  ! GOSUB CREATE.UPDATED.BKPLIST                                        !URG
    GOSUB BACKUP.COMPLETION
  ! GOSUB BACKUP.CONFIG.FILES                                           !URG OJK
    GOSUB OLD.ARCHIVE.PURGE
    GOSUB FINAL.UPDATE.XBKOK        ! Changed the order of XBKOK update !OJK

RETURN

\***********************************************************************
\*
\*    TERMINATION: Termination Sub-routine will be called before
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
\*    INITIALISE.VARIABLES : This Sub-routine Initialize all the
\*                           necessary variables which will be used
\*                           in this program.
\*
\***********************************************************************
INITIALISE.VARIABLES:

    ! messages for update and errors                                    !KDC
    GOSUB INITALISE.MESSAGES                                            !KDC

    STATUS.MSG$ = STATUS.TEXT.MSG$(2)                                   !KDC
    GOSUB DISPLAY.STATUS.MSG

    ! Program name
    PROGRAM$        = "XBACKUP"

    ! Zero variable
    XBACK.ZERO%     = 0                      ! Setting '0' variable

    ! ADXSERVE function variables
    ADX.FUNCTION%   = 26                     ! Default Function value
    ADX.INTEGER%    = XBACK.ZERO%
    ADXSERVE.RC%    = XBACK.ZERO%

    ! Boolean assignments
    FALSE           = XBACK.ZERO%            !  0 = False
    TRUE            = -1                     ! -1 = True

    ! Command line assignment
    COMMAND.STRING$ = COMMAND$   

    ! Parameter value
    PARM.BACKGRND$      = "BACKGRND"                                    !RJK
    PARM.FULL$          = "F"                                           !KDC
    PARM.INCREMENTAL$   = "I"                                           !KDC
    PARM.ONS$           = "RE-RUN"                                      !KDC
    PARM.SLEEPER$       = "SLEEPER"                                     !KDC
    ! built command string expected at start up                         !KDC
    PARM.BACKGRND.INC$ = PARM.BACKGRND$ + " " + PARM.INCREMENTAL$       !RJK
    PARM.BACKGRND.FULL$= PARM.BACKGRND$ + " " + PARM.FULL$              !RJK
    PARM.RERUN.FULL$   = PARM.ONS$ + " " +  PARM.FULL$                  !KDC
    PARM.SLEEPER.FULL$ = PARM.SLEEPER$ + " " +  PARM.FULL$              !KDC
    PARM.RERUN.INC$    = PARM.ONS$ + " " +  PARM.INCREMENTAL$           !KDC
    PARM.SLEEPER.INC$  = PARM.SLEEPER$ + " " +  PARM.INCREMENTAL$       !KDC

    ! Backup directory names must be hard coded to match the XRESTORE   !KDC
    ! logic which cannot use user logical names because it needs to run !KDC
    ! under supplemental where these are not available.                 !KDC
    ! They are constructed here to make any future change more          !KDC
    ! manageable and shown here in logical order for clarity            !KDC
    CE.NODE.NAME$                   = "ADXLXCEN::"                      !KDC
    CF.NODE.NAME$                   = "ADXLXCFN::"                      !KDC
    C.DRIVE.IMAGE.DIRECTORY$        = "C:\XDISKIMG\"                    !KDC
    C.DRIVE.ALTERNATE.DIRECTORY$    = "C:\XDISKALT\"                    !KDC
    D.DRIVE.IMAGE.DIRECTORY$        = "D:\XDISKIMG\"                    !KDC
    D.DRIVE.ALTERNATE.DIRECTORY$    = "D:\XDISKALT\"                    !KDC
    

    ! To ensure faster processing is achieved using W drive rather than !TSM
    ! C drive to avoide repeated open and close on harddisk             !TSM

    TEMP.DIRECTORY.NAME$           = "W:\"                              !TSM
    ! the full path can then be constructed                             !KDC
    ! for CE                                                            !KDC
    CE.C.XDISKALT$  = CE.NODE.NAME$ + C.DRIVE.ALTERNATE.DIRECTORY$      !KDC
    CE.C.XDISKIMG$  = CE.NODE.NAME$ + C.DRIVE.IMAGE.DIRECTORY$          !KDC
    CE.D.XDISKALT$  = CE.NODE.NAME$ + D.DRIVE.ALTERNATE.DIRECTORY$      !KDC
    CE.D.XDISKIMG$  = CE.NODE.NAME$ + D.DRIVE.IMAGE.DIRECTORY$          !KDC
    ! for CF                                                            !KDC
    CF.C.XDISKALT$  = CF.NODE.NAME$ + C.DRIVE.ALTERNATE.DIRECTORY$      !KDC
    CF.C.XDISKIMG$  = CF.NODE.NAME$ + C.DRIVE.IMAGE.DIRECTORY$          !KDC
    CF.D.XDISKALT$  = CF.NODE.NAME$ + D.DRIVE.ALTERNATE.DIRECTORY$      !KDC
    CF.D.XDISKIMG$  = CF.NODE.NAME$ + D.DRIVE.IMAGE.DIRECTORY$          !KDC
                                                                        !KDC
    ! files located in the temporary directory                          !KDC
                            ! Excluded list                             !KDC
    BKPEXCL.FILE.NAME$      = TEMP.DIRECTORY.NAME$ + "BKPEXCL.DAT"      !KDC
                            ! BKPLIST C drive file                      !OJK

    !Removing the access to BKPLIST file as we dont need to maintain it !URG
    ! BKPLIST.FILE.NAME.C$    = "C:\ADX_UDT1\BKPLIST."                  !URG OJK
                            ! FTP file                                  !KDC
    FTP.FILE.NAME$          = TEMP.DIRECTORY.NAME$ + "XBKPFTP.FTP"      !KDC
                            ! FTP output                                !KDC
    FTPOUT.FILE.NAME$       = TEMP.DIRECTORY.NAME$ + "XBKPFTP.OUT"      !KDC
                            ! Log file                                  !KDC
    XBACK.LOG.LIVE.PATH$    = TEMP.DIRECTORY.NAME$ + "XBACKUP.LOG"      !KDC
                            ! Temporary log                             !KDC
    XBKTEMP.LOG$            = TEMP.DIRECTORY.NAME$ + "XBKTEMP.LOG"      !KDC
    ! Other File names                                                  !KDC
    BKPFAILC.FILE.NAME$     = "D:\XDISKIMG\BKPFAILC."    ! failed C list!OJK
    BKPFAILD.FILE.NAME$     = "C:\XDISKIMG\BKPFAILD."    ! failed D list!OJK
    BKPSCRPT.FILE.NAME$     = "D:\ADX_UDT1\BKPSCRPT.TXT" ! Backup script
    TEMP.FILE.NAME$         = "TEMP"                     ! Temporary
    XBACK.LOG.FILE.NAME$    = "D:\ADX_UDT1\XBACKUP.LOG"  ! Log file
    XBACK.PIPE.NAME$        = "pi:XBACKUP"               ! Pipe name

    ! Updated File and Pipe variables                                   !CJK
    BKPEXCL.REPORT.NUM%     = 205                   !                   !OJK
    BKPFAILC.REPORT.NUM%    = 418                   ! Temporary         !OJK
    BKPFAILD.REPORT.NUM%    = 419                   ! report            !OJK
    BKPSCRPT.REPORT.NUM%    = 420                   ! numbers
    TEMP.REPORT.NUM%        = 426                   !
    TEMP.REPORT.NUM.2%      = 531                   !
    XBACK.LOG.REPORT.NUM%   = 668                   !
    XBACK.PIPE.REPORT.NUM%  = 827                   !

    ! Directory listing output files
    DIR.OUT$                = "C:\DIR.OUT"          ! Directory listing
    DIR.SEC.OUT$            = "C:\DIR.OU1"          ! Directory listing

    ! Controller related variables
    ALT.MASTER.ON           = FALSE                 ! CF presence
    CE.CNTR$                = "CE"                  ! CE Controller     !CJK
    CF.CNTR$                = "CF"                  ! CF Controller     !CJK
    MASTER$                 = "CE"                  ! Default Controller
    ! ADXSERVE file function variable value for MASTER/FILE server      !OJK
    MASTER.AND.FILE.SERVER% = 21                    ! Master/File server

    ! Boolean variables
    ALT.EXISTS              = FALSE                 ! ALT and IMG
    IMG.EXISTS              = FALSE                 ! directory exist
    VALUE.EXISTS            = TRUE                  ! Boolean check

    ! File not opened yet, hence defaulting to FALSE                    !OJK
    BKPEXCL.OPEN   = FALSE                                              !OJK
    BKPFAILC.OPEN  = FALSE                                              !OJK
    BKPFAILD.OPEN  = FALSE                                              !OJK
    BKPLI.OPEN     = FALSE                                              !OJK
    BKPSCRPT.OPEN  = FALSE                                              !OJK
    SLPCF.OPEN     = FALSE                                              !OJK
    TEMP.2.OPEN    = FALSE                                              !OJK
    TEMP.OPEN      = FALSE                                              !OJK
    XBACK.LOG.OPEN = FALSE                                              !OJK
    XBACK.OPEN     = FALSE                                              !OJK
    XBKOK.OPEN     = FALSE                                              !OJK

    ! Repeated variables
    COMMA.VALUE$            = ","                   ! Comma value
    XBACK.NULL$             = ""                    ! Null value
    XBACK.YES$              = "Y"                   ! 'Y' value

    ! Status variables
    RUN.TYPE$               = XBACK.NULL$           ! Null              !DJK
    STATUS.END$             = "E"                   ! Successful End
    STATUS.MAJOR.ERROR$     = "X"                   ! Major error
    STATUS.START$           = "S"                   ! Program started

    ! Array variables
    ARRAY.INDEX%            = XBACK.ZERO%
    ARRAY.LIMIT%            = 10000                 ! Array limit
    ARRAY.SECOND.INDEX%     = XBACK.ZERO%
    
    BKPEXCL.ARRAY.INDEX% = 1               !Exclude array index         !TSM
    BKPEXCL.ARRAY.LIMIT% = 1000            !Exclude array limit         !TSM
    BKPEXCL.COMP.INDEX%  = 1               !Complete exclude array index!TSM
    BKPEXCL.COMP.LIMIT%  = 10000           !Complete exclude array limit!TSM
    
    INDEX%                  = XBACK.ZERO%
    NUM.OF.ARRAYS%          = 2                     ! Number of arrays  !FJK
    SECOND.INDEX%           = XBACK.ZERO%           ! used
    SECOND.ARRAY.ON         = FALSE

    CRLF$                   = CHR$(13) + CHR$(10)   ! Assigning CR/LF   !OJK

    ! It holds BKPLIST line values and 10,000 is the maximum allowed    !OJK
    ! BKPLIST line value in this array                                  !OJK
    ! DIM BKPLIST.ARRAY$(ARRAY.LIMIT%)                                  !URG
    
    !Defines array dimension                                            !TSM
    DIM BKPEXCL.ARRAY$(BKPEXCL.ARRAY.LIMIT%)                            !TSM
    DIM BKPEXCL.COMP.ARRAY$(BKPEXCL.COMP.LIMIT%)                        !TSM

    ! Incremental file flag for existing and changed                    !KDC
    ! Other than one FULL backup day in a week, rest of the 6 days      !OJK
    ! details will be stored and used using in following arrays         !OJK
    DIM BKPLI.INCREMENTAL.EXIST$(6)                                     !KDC
    DIM BKPLI.INCREMENTAL.FILE.CHNG$(6)                                 !KDC

!    CRLF$                   = CHR$(13) + CHR$(10)   ! Assigning CR/LF  !OJK

    ! set the file function variables                                   !KDC
    CALL BKPLI.SET
    CALL XBKOK.SET
    CALL HSIUF.SET                                                      !KDC
    CALL SLPCF.SET                                                      !KDC

    GOSUB SET.PROCESS.DATE

RETURN

! KDC START BLOCK
\***********************************************************************
\*
\*    INITALISE.MESSAGES:This Sub-routine Initialize all the messages
\*                       used for status update and errors
\*
\***********************************************************************
INITALISE.MESSAGES:

    VERSION$ = "### XBACKUP.286 - Version R - 09/09/2014 ###"           !RJK

    DIM STATUS.TEXT.MSG$(60)
    DIM STATUS.TEXT.ERROR$(60)

    !******************************************************************
    ! Status update messages
    !******************************************************************
    STATUS.TEXT.MSG$(0)  = "Version ""R"" "                             !RJK
    STATUS.TEXT.MSG$(1)  = "Ended: ZIP files already present"
    STATUS.TEXT.MSG$(2)  = STATUS.TEXT.MSG$(0) + \
                           " Initialising variables"
    STATUS.TEXT.MSG$(3)  = "Defining Process date"
    STATUS.TEXT.MSG$(4)  = "Processing on "
    STATUS.TEXT.MSG$(5)  = " at "
    STATUS.TEXT.MSG$(6)  = "Checking Controller configuration"
    STATUS.TEXT.MSG$(7)  = "Ending the program"
    STATUS.TEXT.MSG$(8)  = "Controller is not Master/File server"
    STATUS.TEXT.MSG$(9)  = "SLPCF Full backup day is "
    STATUS.TEXT.MSG$(10) = "Allocating Session numbers"
    STATUS.TEXT.MSG$(11) = "Creating run pipe"
    STATUS.TEXT.MSG$(12) = "Checking backup directories"
    STATUS.TEXT.MSG$(13) = " directory created"
    STATUS.TEXT.MSG$(14) = ""
    STATUS.TEXT.MSG$(15) = "Run is not scheduled for today"
    STATUS.TEXT.MSG$(16) = "Backup Configured for all Days"
    STATUS.TEXT.MSG$(17) = ""
    STATUS.TEXT.MSG$(18) = ""
    STATUS.TEXT.MSG$(19) = ""
    STATUS.TEXT.MSG$(20) = ""
    STATUS.TEXT.MSG$(21) = ""
    STATUS.TEXT.MSG$(22) = ""

    STATUS.TEXT.MSG$(23) = "Existing XBACKUP log deleted"
    STATUS.TEXT.MSG$(24) = STATUS.TEXT.MSG$(0) + \
                           " Backup Application started"
    STATUS.TEXT.MSG$(25) = "New XBACKUP log file created"
    STATUS.TEXT.MSG$(26) = "Master Controller is CE"
    STATUS.TEXT.MSG$(27) = "Master Controller is CF"
    STATUS.TEXT.MSG$(28) = "Program running in Sleeper mode"
    STATUS.TEXT.MSG$(29) = "Existing BKPEXCL file deleted"
    STATUS.TEXT.MSG$(30) = "New BKPFAIL files created"                  !OJK
    STATUS.TEXT.MSG$(31) = "Created new XBKOK"
    STATUS.TEXT.MSG$(32) = "XBKOK opened and read"
    STATUS.TEXT.MSG$(33) = ""                   ! removed as duplicated
    STATUS.TEXT.MSG$(34) = "Started: ""S"" XBACKUP started"
    STATUS.TEXT.MSG$(35) = "Ended: ""E"" XBACKUP ended"

    ! As BKPLIST file is not required anymore, no need of below statuses!URG
    ! STATUS.TEXT.MSG$(36) = " files present in BKPLIST"                !URG
    ! STATUS.TEXT.MSG$(37) = "BKPLIST file opened"                      !URG
    ! STATUS.TEXT.MSG$(38) = "New BKPLIST file created"                 !URG

    STATUS.TEXT.MSG$(39) = "FULL backup in progress..."
    STATUS.TEXT.MSG$(40) = "INCREMENTAL backup in progress..."
    ! backup script processing wait message
    STATUS.TEXT.MSG$(41) = "Please note that directory archiving"
    STATUS.TEXT.MSG$(42) = "would take more time depending on the"
    STATUS.TEXT.MSG$(43) = "number of files in a directory"
    STATUS.TEXT.MSG$(44) = "Hence please be patient when"
    STATUS.TEXT.MSG$(45) = "the program is running..."
    STATUS.TEXT.MSG$(46) = "**************************************"

    STATUS.TEXT.MSG$(47) = " directory archiving...  "
    STATUS.TEXT.MSG$(48) = "Success: Directory Backup completed"
    STATUS.TEXT.MSG$(49) = " directory back up starts..."
    ! Removing the status of BKPLIST file                               !URG
    ! STATUS.TEXT.MSG$(50) = "Creating Updated BKPLIST file"            !URG
    ! STATUS.TEXT.MSG$(51) = "Updated BKPLIST file created"             !URG
    ! STATUS.TEXT.MSG$(52) = "Ended: BKPLIST file write error"          !URG
    STATUS.TEXT.MSG$(53) = "Started FTPing the archived files"
    STATUS.TEXT.MSG$(54) = "Archived file FTP transfer completed"
    STATUS.TEXT.MSG$(55) = "Older archive files purged"
    STATUS.TEXT.MSG$(56) = "XBACKUP log file backed up successfully"
    ! STATUS.TEXT.MSG$(57) = "Deleting older BKPLIST file if present"   !URG
    STATUS.TEXT.MSG$(58) = ""
    STATUS.TEXT.MSG$(59) = ""
    STATUS.TEXT.MSG$(60) = ""

    !******************************************************************
    ! Error messages
    !******************************************************************
    STATUS.TEXT.ERROR$(1)  = "Error: Open/read failed in DIR list file"
    STATUS.TEXT.ERROR$(2)  = "Error: Open/Read failed in DIR MDD file"
    STATUS.TEXT.ERROR$(3)  = "ADXDATE sub-program error"
    STATUS.TEXT.ERROR$(4)  = "Parameter is missing or incorrect... "
    STATUS.TEXT.ERROR$(5)  = "Controller status not obtainable"
    STATUS.TEXT.ERROR$(6)  = "Error: Controller Node ID invalid"
    STATUS.TEXT.ERROR$(7)  = "SLPCF Full backup configuration not found"
    STATUS.TEXT.ERROR$(8)  = "Ended: Directory listing error"
    STATUS.TEXT.ERROR$(9)  = "Error: Variable error in BKPSCRPT file"
    STATUS.TEXT.ERROR$(10) = "Ended: Open error in BKPSCRPT file"
    STATUS.TEXT.ERROR$(11) = "Ended: Read error in BKPSCRPT file"
    STATUS.TEXT.ERROR$(12) = "Ended: XBKOK read error"
    STATUS.TEXT.ERROR$(13) = "Ended: XBKOK truncated"
    STATUS.TEXT.ERROR$(14) = "ERROR: XBKOK missing"
    STATUS.TEXT.ERROR$(15) = "Ended: XBKOK write error"
    STATUS.TEXT.ERROR$(16) = "Ended: ""X"" Critical Error, refer log"
    ! STATUS.TEXT.ERROR$(17) = "Full run, New BKPLIST file created"     ! URG
    ! Array limit reached message
    STATUS.TEXT.ERROR$(18) = "More than "
    STATUS.TEXT.ERROR$(19) = " files has been found"
    STATUS.TEXT.ERROR$(20) = "Check for any abnormality in file system"

    ! STATUS.TEXT.ERROR$(21) = "Current week BKPLIST file not present"  ! URG
    STATUS.TEXT.ERROR$(22) = " directory does not exist"
    STATUS.TEXT.ERROR$(23) = "Error: BKPEXCL write failed"
    STATUS.TEXT.ERROR$(24) = "Ended: Open/Read failure in DIR.NOT file"
    STATUS.TEXT.ERROR$(25) = "Error: Open/Read failed in DIR list file"
    STATUS.TEXT.ERROR$(26) = "Error: Read failed in DIR list file"
    STATUS.TEXT.ERROR$(27) = "Ended: Open failed in BKPEXCL file"
    STATUS.TEXT.ERROR$(28) = " backup rename fails"
    STATUS.TEXT.ERROR$(29) = " file COPY error"
    STATUS.TEXT.ERROR$(30) = " directory is not archived"
    STATUS.TEXT.ERROR$(31) = "Because it has no files in it..."
    STATUS.TEXT.ERROR$(32) = "Error: BKPFAIL write failed"
    STATUS.TEXT.ERROR$(33) = "Error: FTP communication issue occurred"
    STATUS.TEXT.ERROR$(34) = "Error in opening FTP output file"
    STATUS.TEXT.ERROR$(35) = "Error: XBKPFTP write failed"
    STATUS.TEXT.ERROR$(36) = "Error: XBACKUP write failed"
    STATUS.TEXT.ERROR$(37) = "UPDATE date function error"
    STATUS.TEXT.ERROR$(38) = "PSDATE function error"
    STATUS.TEXT.ERROR$(39) = "Ended: Error while opening "
    STATUS.TEXT.ERROR$(40) = "Ended: Error in application log"
    ! Invalid parameter message
    STATUS.TEXT.ERROR$(41) = "!**************************************!"
    STATUS.TEXT.ERROR$(42) = "! Please check the parameter..."
    STATUS.TEXT.ERROR$(43) = "! Invalid parameter passed"

    STATUS.TEXT.ERROR$(44) = "Logging 106 event File Op = "
    STATUS.TEXT.ERROR$(45) = "Error while processing "
    ! Program already running message
    STATUS.TEXT.ERROR$(46) = "***! "
    STATUS.TEXT.ERROR$(47) = ".286 is already active         !***"

    STATUS.TEXT.ERROR$(48) = "Ended: Variable error in BKPSCRPT file"
    STATUS.TEXT.ERROR$(49) = "Ended: Unable to access XBACKUP.LOG file"
    STATUS.TEXT.ERROR$(50) = "Ended: Unable to access BKPSCRPT file"
    STATUS.TEXT.ERROR$(51) = "Ended: Unable to access BKPEXCL file"
    STATUS.TEXT.ERROR$(52) = "Ended: Unable to access XBKOK file"
    STATUS.TEXT.ERROR$(53) = "Ended: XBACKUP log create failed"
    STATUS.TEXT.ERROR$(54) = "Ended: BKPEXCL file create failed"
    STATUS.TEXT.ERROR$(55) = "Ended: BKPFAILC file create failed"       !OJK
    ! STATUS.TEXT.ERROR$(56) = "Ended: BKPLIST file create failed"      !URG
    STATUS.TEXT.ERROR$(57) = "Ended: Temporary file create failed"
    STATUS.TEXT.ERROR$(58) = "Ended: DAYS TO KEEP is 0 in BKPSCRPT"     !PJK
    ! STATUS.TEXT.ERROR$(59) = "Ended: Unable to access BKPLIST file"   !URG
    STATUS.TEXT.ERROR$(60) = "Ended: BKPFAILD file create failed"       !OJK
RETURN
! KDC END BLOCK

\***********************************************************************
\*
\*    SET.PROCESS.DATE : This Sub-routine Initialize all the
\*                       date and time variables which will
\*                       be used in this program.
\*
\***********************************************************************
SET.PROCESS.DATE:

    STATUS.MSG$ = STATUS.TEXT.MSG$(3)                                   !KDC
    GOSUB DISPLAY.STATUS.MSG

    !------------------------------------!
    ! The below variable holds the first !
    ! three letters of all the months    !
    !------------------------------------!
    MONTH$ = "   JANFEBMARAPRMAYJUNJULAUGSEPOCTNOVDEC"

    CURR.DATE$ = DATE$
    CURR.TIME$ = TIME$

    !---------------------------------------!
    ! To get the date in YYYYMMDD format    !                           !OJK
    !---------------------------------------!
    CALL ADXDATE (RETURN.VALUE.CHECK%, TODAY.DATE$)                     !OJK

    ! Checking the return value
    IF RETURN.VALUE.CHECK% <> XBACK.ZERO% THEN BEGIN                    !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(3)                             !KDC
        GOSUB DISPLAY.STATUS.MSG                                        !FJK
        GOSUB STOP.PROGRAM                                              !FJK
    ENDIF                                                               !FJK

! Commenting out as it has become redundant                             !OJK
!    TODAY.DATE$ = LEFT$(DATE.YYYY$, 2) + CURR.DATE$ ! Year in YYYYMMDD !OJK

    ! Today's date in DD MMM YYYY
    TODAY.DATE$ = RIGHT$(TODAY.DATE$, 2) + " "                + \ ! DD
                  MID$(MONTH$,VAL(MID$(TODAY.DATE$,5,2)) * 3  + \ ! MMM
                  1 , 3) + " " + LEFT$(TODAY.DATE$, 4)            ! YYYY

    !-------------------------------------------------------!
    ! Defining the Extension name from date and month value !
    !-------------------------------------------------------!
    EXTENSION$ = RIGHT$(CURR.DATE$,4)       ! Extracting Month value    !KDC
    GOSUB GET.FILE.EXTENSION                                            !KDC
    EXT.MDD$ = EXTENSION$                                               !KDC

RETURN

\***********************************************************************
\*
\*    CHECK.PARAM : This Sub-routine uses the ADXSERVE function to
\*                  check whether it's a background run. Also
\*                  it validates the command parameter.
\*
\***********************************************************************
CHECK.PARAM:

    ! Processing message to display
    STATUS.MSG$ = STATUS.TEXT.MSG$(4) + TODAY.DATE$ + \                 !KDC
                  STATUS.TEXT.MSG$(5) + \                               !KDC
                  MID$(TIME$,1,2) + ":" + MID$(TIME$,3,2) + ":" + \     !KDC
                  MID$(TIME$,5,2)                                       !KDC


    CALL ADXSERVE (ADXSERVE.RC%,ADX.FUNCTION%,ADX.INTEGER%,STATUS.MSG$)
    CALL TRIM(COMMAND.STRING$)

    IF ADXSERVE.RC% = -1101 THEN BEGIN  ! Not running as background
        BACKGROUND.RUN = FALSE
        GOSUB DISPLAY.STATUS.MSG

        IF LEN(COMMAND.STRING$) = 1 THEN BEGIN                          !KDC

            IF COMMAND.STRING$ = "F" THEN BEGIN  ! Full backup
                ! Setting the Backup type
                RUN.TYPE$ = "F"
            ENDIF ELSE IF COMMAND.STRING$ = "I" THEN BEGIN ! Incremental
                ! Setting the Backup type
               ! RUN.TYPE$ = "I"                                        !URG
                 RUN.TYPE$ = "F"                                        !URG   
            ENDIF ELSE BEGIN

                IF LEN(COMMAND.STRING$) = XBACK.ZERO% THEN BEGIN
                    !-----------------------------------------------!
                    ! As command parameters not passed, determining !
                    ! the backup type using current day             !
                    !-----------------------------------------------!

                    ! Getting the day using PSDATE function
                    PSDATE.DATE$ = CURR.DATE$                           !KDC
                    GOSUB GET.DAY.AND.CHECK.ERROR                       !KDC

                 !   IF F13.DAY$ = FULL.DAY$ THEN BEGIN       ! If Full !URG KDC
                 !       ! Setting the Backup type                      !URG

                         RUN.TYPE$ = "F"

                 !   ENDIF ELSE BEGIN                                   !URG
                 !      ! Setting the Backup type                       !URG
                 !       RUN.TYPE$ = "I"                                !URG
                 !   ENDIF                                              !URG

                ENDIF ELSE BEGIN
                    GOSUB INVALID.PARAM.EXIT                            !DJK
                ENDIF
            ENDIF

        ENDIF ELSE BEGIN
            GOSUB INVALID.PARAM.EXIT                                    !DJK
        ENDIF

    ENDIF ELSE BEGIN
        GOSUB CHECK.ADXSERVE.RC ! Stops program if non zero
        BACKGROUND.RUN = TRUE

        !-----------------------------------------------------!         !DJK
        ! Implemented changes in Background parameter check   !         !DJK
        ! with respect to the changes in SLEEPER program      !         !DJK
        !-----------------------------------------------------!         !DJK

        ! If parameter is SLEEPER or RERUN Incremental
        IF RIGHT$(COMMAND.STRING$,9) = PARM.SLEEPER.INC$ OR \           !RJK
           RIGHT$(COMMAND.STRING$,8) = PARM.RERUN.INC$   OR \           !RJK
           COMMAND.STRING$ = PARM.BACKGRND.INC$   THEN BEGIN            !RJK
            ! Setting the Backup type
           ! RUN.TYPE$ = "I"                                            !URG
             RUN.TYPE$ = "F"                                            !URG
        ! If parameter is SLEEPER or RERUN Full
        ENDIF ELSE IF RIGHT$(COMMAND.STRING$,9)= PARM.SLEEPER.FULL$ OR \!RJK
                      RIGHT$(COMMAND.STRING$,8) = PARM.RERUN.FULL$  OR \!RJK
                      COMMAND.STRING$ = PARM.BACKGRND.FULL$ THEN BEGIN  !RJK
            ! Setting the Backup type
            RUN.TYPE$ = "F"

        ! Invalid parameter
        ENDIF ELSE BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(4)
            GOSUB DISPLAY.STATUS.MSG
            GOSUB STOP.PROGRAM
        ENDIF

    ENDIF

RETURN

\***********************************************************************
\*
\*    CONTROLLER.CONFIG.CHECK: This Sub-routine uses ADXSERVE function
\*                             and check whether the controller is
\*                             Master and File server.If not, program
\*                             should end, logging an appropriate error.
\*
\***********************************************************************
CONTROLLER.CONFIG.CHECK:

    STATUS.MSG$ = STATUS.TEXT.MSG$(6)                                   !KDC
    GOSUB DISPLAY.STATUS.MSG

    ADX.FUNCTION% = 4      ! Function 4 to get the controller details
    CALL ADXSERVE (ADXSERVE.RC%,ADX.FUNCTION%,ADX.INTEGER%,ADX.PARM.2$)

    ! If return code non zero state error and end                       !KDC
    IF ADXSERVE.RC% <> XBACK.ZERO% THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.ERROR$(5)                             !KDC
        GOSUB DISPLAY.STATUS.MSG
        STATUS.MSG$ = STATUS.TEXT.MSG$(7)                               !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB STOP.PROGRAM
    ENDIF

    ! Controller configuration
    CNTLR.CONFIG% = VAL(MID$(ADX.PARM.2$, 25, 2))
    ! Master Controller ID
    CNTLR.ID$     = MID$(ADX.PARM.2$, 14, 2)

    !-------------------------------------------!
    ! If controller is not a Master/File server !
    !-------------------------------------------!
    IF CNTLR.CONFIG% <> MASTER.AND.FILE.SERVER% THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(8)                               !KDC
        GOSUB DISPLAY.STATUS.MSG
        STATUS.MSG$ = STATUS.TEXT.MSG$(7)                               !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB STOP.PROGRAM
    ENDIF

    ADX.FUNCTION% = 36     ! Function 36 to get the configured nodes
    CALL ADXSERVE (ADXSERVE.RC%,ADX.FUNCTION%,ADX.INTEGER%,ADX.PARM.2$)
    GOSUB CHECK.ADXSERVE.RC ! Stops program if non zero

    ! To identify Controllers in LAN
    NODE.POSITION%    = MATCH("00", ADX.PARM.2$, 1)
    CONFIGURED.NODES$ = LEFT$(ADX.PARM.2$, NODE.POSITION% - 1) ! Node ID

    !---------------------------------------!
    ! Identifying Master controller Node ID !
    !---------------------------------------!
    IF CNTLR.ID$ = CE.CNTR$ THEN BEGIN
        MASTER$ = CE.CNTR$                                              !OJK
        ! If CF controller available
        IF MATCH("CF", CONFIGURED.NODES$, 1) <> XBACK.ZERO% THEN BEGIN
            ALT.MASTER.ON = TRUE
        ENDIF
    ENDIF ELSE BEGIN
        IF CNTLR.ID$ = CF.CNTR$ THEN BEGIN
            MASTER$       = CF.CNTR$
            ALT.MASTER.ON = TRUE
        ENDIF ELSE BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(6)                         !KDC
            GOSUB DISPLAY.STATUS.MSG
            GOSUB STOP.PROGRAM
        ENDIF
    ENDIF

RETURN


\***********************************************************************!KDC
\*                                                                      !KDC
\*    GET.SLEEPER.CONFIGURATION:This Sub-routine looks through the      !KDC
\*                              Sleeper control file in order to see    !KDC
\*                              what the current setting for the Full   !KDC
\*                              backup is. It then assumes all          !KDC
\*                              Incremental based upon this day         !KDC
\*                                                                      !KDC
\***********************************************************************!KDC
GET.SLEEPER.CONFIGURATION:                                              !KDC

    !*******************************************************************!KDC
    ! first find the full backup day configured in sleeper              !KDC
    !*******************************************************************!KDC
    IF END #SLPCF.SESS.NUM% THEN SLPCF.NOT.FOUND

    OPEN SLPCF.FILE.NAME$ DIRECT RECL SLPCF.RECL% \                     !OJK
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
                    !SLEEPER.DAY% = VAL(SLPCF.DAY.NUM$)                 !URG
                    !STATUS.MSG$ = STATUS.TEXT.MSG$(9) + SLPCF.DAY.NUM$ !URG OJK
                    RUN.ALLOWED% = 0                                    !URG
                    RUN.ALLOWED$ = "NO"                                 !URG

               !Below code change is to read sleeper properly rather    !URG
               !than reading it once a week and count the incremental   !URG
               !days. Below approach gives the flexibility to configure !URG
               !XBACKUP days based on need                              !URG

                    IF SLPCF.DAY.NUM$ = " " THEN BEGIN                  !URG
                       STATUS.MSG$ = STATUS.TEXT.MSG$(16)               !URG
                       GOSUB DISPLAY.STATUS.MSG                         !URG
                       GOSUB LOG.STATUS.MSG                             !URG
                       RUN.ALLOWED$ = "YES"                             !URG
                    ENDIF ELSE BEGIN                                    !URG
                       CALL PSDATE(DATE$)                               !URG
                       IF F13.DAY$ = "SUN" THEN BEGIN                   !URG
                          RUN.ALLOWED% = MATCH("1",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "MON" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("2",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "TUE" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("3",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "WED" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("4",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "THU" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("5",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "FRI" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("6",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF ELSE IF F13.DAY$ = "SAT" THEN BEGIN        !URG
                          RUN.ALLOWED% = MATCH("7",SLPCF.DAY.NUM$,1)    !URG
                       ENDIF                                            !URG

                       IF RUN.ALLOWED% > 0 THEN BEGIN                   !URG
                          RUN.ALLOWED$  = "YES"                         !URG
                       ENDIF                                            !URG

                    ENDIF                                               !URG


                ENDIF
            ENDIF
        ENDIF ELSE BEGIN                                !end of file
            STATUS.MSG$ = STATUS.TEXT.ERROR$(7)                         !KDC
            GOSUB DISPLAY.STATUS.MSG
            GOSUB STOP.PROGRAM
        ENDIF
    WEND

    GOSUB DISPLAY.STATUS.MSG

    IF SLPCF.OPEN THEN BEGIN                                            !KDC
        CLOSE SLPCF.SESS.NUM%                                           !KDC
        CALL SESS.NUM.UTILITY ("C",SLPCF.SESS.NUM%,XBACK.NULL$)         !OJK
        SLPCF.OPEN = FALSE                                              !KDC
    ENDIF                                                               !KDC

    IF RUN.ALLOWED$ = "NO" THEN  BEGIN                                  !URG
       STATUS.MSG$ = STATUS.TEXT.MSG$(15)                               !URG
       GOSUB DISPLAY.STATUS.MSG                                         !URG
       GOSUB LOG.STATUS.MSG                                             !URG
       GOSUB STOP.PROGRAM                                               !URG
    ENDIF                                                               !URG


    !*******************************************************************!KDC
    !then set the data we will build from - note that the array elements!KDC
    !must match the sleeper settings (ie 1=Sunday) to work correctly    !KDC
    !*******************************************************************!KDC
 ! No need of below set of codes as we are not doing incrementals        !URG
 ! anymore

 !   DIM CONSTANT.DAYS$(7)                                               !URG LDC
 !    CONSTANT.COLON$ = ":"                                              !URG OJK
 !   CONSTANT.DAYS$(1) = ":SUN"                                          !URG OJK
 !   CONSTANT.DAYS$(2) = ":MON"                                          !URG OJK
 !   CONSTANT.DAYS$(3) = ":TUE"                                          !URG OJK
 !   CONSTANT.DAYS$(4) = ":WED"                                          !URG OJK
 !   CONSTANT.DAYS$(5) = ":THU"                                          !URG OJK
 !   CONSTANT.DAYS$(6) = ":FRI"                                          !URG OJK
 !   CONSTANT.DAYS$(7) = ":SAT"                                          !URG OJK

    !*******************************************************************!KDC
    !Build up the string with all the days, starting with the full      !KDC
    !backup day so it can be matched to any-time to find the Full and   !KDC
    !Incremental days adding the : prevents mismatching characters that !KDC
    !might overlap. for example if Sunday is the full configured day the!KDC
    ! string would be; ":SUN:MON:TUE:WED:THU:FRI:SAT"                   !KDC
    !*******************************************************************!KDC
 !   BACKUP.DAYS$ = CONSTANT.DAYS$(SLEEPER.DAY%)                         !URG LDC

 !   FULL.DAY$    = RIGHT$(CONSTANT.DAYS$(SLEEPER.DAY%),3)               !URG LDC
                                                                         !URG KDC
 !   FOR DAY.SINCE.FULL% = 2 TO 7                                        !URG KDC
                                                                         !URG KDC
 !       IF SLEEPER.DAY% = 7 THEN BEGIN              !if Saturday        !URG KDC
 !           SLEEPER.DAY% = 1                        !set to Sunday      !URG KDC
 !       ENDIF ELSE BEGIN                                                !URG KDC
 !           SLEEPER.DAY% = SLEEPER.DAY% + 1         !next day           !URG KDC
 !       ENDIF                                                           !URG KDC
                                                                         !URG KDC
 !       BACKUP.DAYS$ = BACKUP.DAYS$ + CONSTANT.DAYS$(SLEEPER.DAY%)      !URG LDC
                                                                         !URG KDC
 !   NEXT DAY.SINCE.FULL%                                                !URG KDC
                                                                         !URG KDC
 !   DIM CONSTANT.DAYS$(0)                                               !URG LDC
                                                                         !URG KDC
SLPCF.NOT.FOUND:
    SLPCF.OPEN = FALSE

RETURN                                                                  !KDC

\***********************************************************************
\*
\*    ALLOCATE.SESSION.NUMBERS: This Sub-routine uses variables of FILE
\*                              functions and calls SESS.NUM.UTILITY to
\*                              allocate session numbers.
\*
\***********************************************************************
ALLOCATE.SESSION.NUMBERS:

    STATUS.MSG$ = STATUS.TEXT.MSG$(10)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG

    FUNC.FLAG$ = "O"   ! Setting the file operation to Open

    ! ADXHSIUF FTP file                                                 !KDC
    PASSED.INTEGER% = HSIUF.REPORT.NUM%                                 !KDC
    PASSED.STRING$  = HSIUF.FILE.NAME$                                  !KDC
    GOSUB GET.SESSION.NUMBER                                            !KDC
    HSIUF.SESS.NUM% = F20.INTEGER.FILE.NO%                              !KDC

    ! BKPEXCL
    PASSED.INTEGER% = BKPEXCL.REPORT.NUM%
    PASSED.STRING$  = BKPEXCL.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    BKPEXCL.SESS.NUM%  = F20.INTEGER.FILE.NO%

    ! BKPFAILC                                                          !OJK
    PASSED.INTEGER% = BKPFAILC.REPORT.NUM%                              !OJK
    PASSED.STRING$  = BKPFAILC.FILE.NAME$                               !OJK
    GOSUB GET.SESSION.NUMBER                                            !KDC
    BKPFAILC.SESS.NUM% = F20.INTEGER.FILE.NO%                           !OJK

    ! BKPFAILD                                                          !OJK
    PASSED.INTEGER% = BKPFAILD.REPORT.NUM%                              !OJK
    PASSED.STRING$  = BKPFAILD.FILE.NAME$                               !OJK
    GOSUB GET.SESSION.NUMBER                                            !OJK
    BKPFAILD.SESS.NUM% = F20.INTEGER.FILE.NO%                           !OJK

    ! BKPLIST session allocation
    PASSED.INTEGER% = BKPLI.REPORT.NUM%
    PASSED.STRING$  = BKPLI.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    BKPLI.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! BKPSCRPT session allocation
    PASSED.INTEGER% = BKPSCRPT.REPORT.NUM%
    PASSED.STRING$  = BKPSCRPT.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    BKPSCRPT.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! Sleeper Control file                                              !KDC
    PASSED.INTEGER% = SLPCF.REPORT.NUM%                                 !KDC
    PASSED.STRING$  = SLPCF.FILE.NAME$                                  !OJK
    GOSUB GET.SESSION.NUMBER                                            !KDC
    SLPCF.SESS.NUM% = F20.INTEGER.FILE.NO%                              !KDC

    ! Temporary file
    PASSED.INTEGER% = TEMP.REPORT.NUM%
    PASSED.STRING$  = TEMP.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    TEMP.SESS.NUM%  = F20.INTEGER.FILE.NO%

    ! Temporary file
    PASSED.INTEGER% = TEMP.REPORT.NUM.2%
    PASSED.STRING$  = TEMP.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    TEMP.SESS.NUM.2% = F20.INTEGER.FILE.NO%

    ! XBACKUP log
    PASSED.INTEGER% = XBACK.LOG.REPORT.NUM%
    PASSED.STRING$  = XBACK.LOG.LIVE.PATH$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    XBACK.LOG.SESS.NUM%  = F20.INTEGER.FILE.NO%

    ! XBACKUP pipe
    PASSED.INTEGER% = XBACK.PIPE.REPORT.NUM%
    PASSED.STRING$  = XBACK.PIPE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    XBACK.PIPE.SESS.NUM% = F20.INTEGER.FILE.NO%

    ! XBKOK session allocation
    PASSED.INTEGER% = XBKOK.REPORT.NUM%
    PASSED.STRING$  = XBKOK.FILE.NAME$
    GOSUB GET.SESSION.NUMBER                                            !KDC
    XBKOK.SESS.NUM% = F20.INTEGER.FILE.NO%

RETURN

\***********************************************************************
\*
\*    GET.SESSION.NUMBER: This Sub-routine calls SESS.NUM.UTILITY to
\*                        allocate session numbers.
\*
\***********************************************************************
GET.SESSION.NUMBER:

    RETURN.VALUE.CHECK% = SESS.NUM.UTILITY (FUNC.FLAG$,  \              !KDC
                                        PASSED.INTEGER%, \              !KDC
                                        PASSED.STRING$)                 !KDC
    GOSUB RETURN.VALUE.CHECK                                            !KDC

RETURN

\***********************************************************************
\*
\*      GET.DAY.AND.CHECK.ERROR:This Sub-routine calls PSDATE to get
\*                              a day of the week.
\*
\***********************************************************************
GET.DAY.AND.CHECK.ERROR:

    RETURN.VALUE.CHECK% = PSDATE(PSDATE.DATE$)

    ! Checking the return value
    GOSUB CHECK.PSDATE.RC

RETURN

\***********************************************************************
\*
\*    CREATE.RUN.PIPE: This Sub-routine creates pipe for current module
\*                     to avoid any duplicate run.
\*
\***********************************************************************
CREATE.RUN.PIPE:

    STATUS.MSG$ = STATUS.TEXT.MSG$(11)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG

    CREATE XBACK.PIPE.NAME$ AS XBACK.PIPE.SESS.NUM% BUFFSIZE XBACK.ZERO%
    WAIT ; 1000     ! Allow a second for pipe to be created

    XBACK.OPEN = TRUE

RETURN

\***********************************************************************
\*
\*    CHECK.BKP.DIRECTORIES: This Subroutine verifies the existence of
\*                           backup directories. If any of the
\*                           directory is missing, then respective
\*                           directories will be created.
\*
\***********************************************************************
CHECK.BKP.DIRECTORIES:

    STATUS.MSG$ = STATUS.TEXT.MSG$(12)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG

    !----------------------------!
    ! If Master controller is CE !
    !----------------------------!
    IF MASTER$ = CE.CNTR$ THEN BEGIN

        ! first check drives are available before doing any more

        ! Checking directory presence and verifying listing error
        DRIVE$ = "ADXLXCEN::C:/"
        GOSUB DOES.BACKUP.DIR.EXIST
        IF NOT BACKUP.DIR.EXIST THEN BEGIN
            BACKUP.DIR.EXIST = 1
            STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
            GOSUB PROGRAM.EXIT
        ENDIF

        ! now build each directory as needed
        IF NOT IMG.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCEN::C:\XDISKIMG")                 !KDC
            STATUS.MSG$ = CE.C.XDISKIMG$+ STATUS.TEXT.MSG$(13)          !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT ALT.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCEN::C:\XDISKALT")                 !KDC
            STATUS.MSG$ =  CE.C.XDISKALT$ + STATUS.TEXT.MSG$(13)        !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT TMP.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCEN::C:\TEMP")
            STATUS.MSG$ = CE.NODE.NAME$ + TEMP.DIRECTORY.NAME$ + \      !KDC
                          STATUS.TEXT.MSG$(13)                          !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        ! Checking directory presence and verifying listing error
        DRIVE$ = "ADXLXCEN::D:/"
        GOSUB DOES.BACKUP.DIR.EXIST
        IF NOT BACKUP.DIR.EXIST THEN BEGIN
            BACKUP.DIR.EXIST = 1
            STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
            GOSUB PROGRAM.EXIT
        ENDIF

        IF NOT IMG.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCEN::D:\XDISKIMG")
            STATUS.MSG$ = CE.D.XDISKIMG$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT ALT.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCEN::D:\XDISKALT")
            STATUS.MSG$ = CE.D.XDISKALT$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        !---------------------------------------------!
        ! If Alternate controller is available in LAN !
        !---------------------------------------------!
        IF ALT.MASTER.ON THEN BEGIN

            ! first check drives are available before doing any more

            ! Checking directory presence and verifying listing error
            DRIVE$ = "ADXLXCFN::C:/"
            GOSUB DOES.BACKUP.DIR.EXIST
            IF NOT BACKUP.DIR.EXIST THEN BEGIN
                BACKUP.DIR.EXIST = 1
                STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
                GOSUB PROGRAM.EXIT
            ENDIF

            ! now build each directory as needed

            IF NOT IMG.EXISTS THEN BEGIN                                !CJK
                ! Create directory If not present
                CALL OSSHELL("MKDIR ADXLXCFN::C:\XDISKIMG")
                STATUS.MSG$ = CF.C.XDISKIMG$ + STATUS.TEXT.MSG$(13)     !KDC
                GOSUB DISPLAY.STATUS.MSG
            ENDIF

            IF NOT ALT.EXISTS THEN BEGIN                                !CJK
                ! Create directory If not present
                CALL OSSHELL("MKDIR ADXLXCFN::C:\XDISKALT")
                STATUS.MSG$ = CF.C.XDISKALT$ + STATUS.TEXT.MSG$(13)     !KDC
                GOSUB DISPLAY.STATUS.MSG
            ENDIF

            ! Checking directory presence and verifying listing error
            DRIVE$ = "ADXLXCFN::D:/"
            GOSUB DOES.BACKUP.DIR.EXIST
            IF NOT BACKUP.DIR.EXIST THEN BEGIN
                BACKUP.DIR.EXIST = 1
                STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
                GOSUB PROGRAM.EXIT
            ENDIF

            IF NOT IMG.EXISTS THEN BEGIN                                !CJK
                ! Create directory If not present
                CALL OSSHELL("MKDIR ADXLXCFN::D:\XDISKIMG")
                STATUS.MSG$ = CF.D.XDISKIMG$ + STATUS.TEXT.MSG$(13)     !KDC
                GOSUB DISPLAY.STATUS.MSG
            ENDIF

            IF NOT ALT.EXISTS THEN BEGIN                                !CJK
                ! Create directory If not present
                CALL OSSHELL("MKDIR ADXLXCFN::D:\XDISKALT")
                STATUS.MSG$ = CF.D.XDISKALT$ + STATUS.TEXT.MSG$(13)     !KDC
                GOSUB DISPLAY.STATUS.MSG
            ENDIF

        ENDIF
    ENDIF ELSE BEGIN

    !----------------------------!
    ! If Master controller is CF !
    !----------------------------!
        ! first check drives are available before doing any more

        ! Checking directory presence and verifying listing error
        DRIVE$ = "ADXLXCFN::C:/"
        GOSUB DOES.BACKUP.DIR.EXIST
        IF NOT BACKUP.DIR.EXIST THEN BEGIN
            BACKUP.DIR.EXIST = 1
            STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
            GOSUB PROGRAM.EXIT
        ENDIF

        ! now build each directory as needed

        IF NOT IMG.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCFN::C:\XDISKIMG")
            STATUS.MSG$ = CF.C.XDISKIMG$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT ALT.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCFN::C:\XDISKALT")
            STATUS.MSG$ = CF.C.XDISKALT$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT TMP.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCFN::C:\TEMP")
            STATUS.MSG$ = CF.NODE.NAME$ + TEMP.DIRECTORY.NAME$ + \      !KDC
                          STATUS.TEXT.MSG$(13)                          !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        ! Checking directory presence and verifying listing error
        DRIVE$ = "ADXLXCFN::D:/"
        GOSUB DOES.BACKUP.DIR.EXIST
        IF NOT BACKUP.DIR.EXIST THEN BEGIN
            BACKUP.DIR.EXIST = 1
            STATUS.MSG$ = STATUS.TEXT.ERROR$(8)
            GOSUB PROGRAM.EXIT
        ENDIF

        IF NOT IMG.EXISTS THEN BEGIN                                    !CJK
            ! Create directory If not present
            CALL OSSHELL("MKDIR ADXLXCFN::D:\XDISKIMG")
            STATUS.MSG$ = CF.D.XDISKIMG$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF

        IF NOT ALT.EXISTS THEN BEGIN                                    !CJK
            CALL OSSHELL("MKDIR ADXLXCFN::D:\XDISKALT")
            STATUS.MSG$ = CF.D.XDISKALT$ + STATUS.TEXT.MSG$(13)         !KDC
            GOSUB DISPLAY.STATUS.MSG
        ENDIF
    ENDIF
RETURN

\***********************************************************************
\*
\*    DOES.BACKUP.DIR.EXIST: This function verifies the existence of
\*                           backup directories.
\*
\***********************************************************************
DOES.BACKUP.DIR.EXIST:

    ! flag to capture error against this subroutine only
    BACKUP.DIR.EXIST = 2

    ! Directory list details
    CALL OSSHELL("DIR -T " + DRIVE$ + " > " + DIR.OUT$               + \
                 " >>* " + DIR.OUT$  )

    IF END # TEMP.SESS.NUM% THEN ERROR.LISTING.DIRECTORY
    OPEN DIR.OUT$ AS TEMP.SESS.NUM%

    ! Setting the temporary file open
    TEMP.OPEN = TRUE                                                    !FJK

    ! Ignoring the first 4 lines
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

    ALT.EXISTS   = FALSE
    IMG.EXISTS   = FALSE
    TMP.EXISTS   = FALSE
    VALUE.EXISTS = TRUE

    WHILE VALUE.EXISTS
        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

        ! Match found
        IF MATCH("XDISKIMG     <DIR>", DIR.VALUE$, 1) <> XBACK.ZERO% \
        THEN BEGIN
            IMG.EXISTS = TRUE
        ENDIF ELSE BEGIN
            ! Match found
            IF MATCH("XDISKALT     <DIR>", DIR.VALUE$, 1) \
            <> XBACK.ZERO% THEN BEGIN
                ALT.EXISTS = TRUE
            ENDIF ELSE BEGIN
                ! Match found
                IF MATCH("TEMP         <DIR>", DIR.VALUE$, 1) \
                <> XBACK.ZERO% THEN BEGIN
                    TMP.EXISTS = TRUE
                ENDIF
            ENDIF
        ENDIF

        ! EOF reached
        IF LEN(DIR.VALUE$) = XBACK.ZERO% THEN BEGIN
            VALUE.EXISTS = FALSE
        ENDIF

    WEND

    ! Deleting the file as no longer needed
    DELETE TEMP.SESS.NUM%
    ! Setting the temporary file close
    TEMP.OPEN = FALSE                                                   !FJK

    BACKUP.DIR.EXIST = 1

 BACKUP.DIR.EXIST.EXIT:                                                 !OJK

RETURN

! if end error handling
ERROR.LISTING.DIRECTORY:

    STATUS.MSG$ = STATUS.TEXT.ERROR$(1)                                 !KDC
    BACKUP.DIR.EXIST = FALSE    ! Error occurred

RETURN


\***********************************************************************
\*
\*    CREATE.XBACKUP.LOG: This Subroutine verifies the existence of
\*                        LOG file. If present, delete the log file
\*                        and create a new one.
\*
\***********************************************************************
CREATE.XBACKUP.LOG:

    CURRENT.REPORT.NUM% = XBACK.LOG.REPORT.NUM%

    !---------------------------------------!
    ! Checking the Existence of XBACKUP log !
    !---------------------------------------!

    FILE.OPERATION$ = "O"                                               !CJK

    IF END # XBACK.LOG.SESS.NUM% THEN XBACK.LOG.NOT.PRESENT             !CJK
    OPEN XBACK.LOG.LIVE.PATH$ AS XBACK.LOG.SESS.NUM%
    XBACK.LOG.OPEN = TRUE                                               !HDC

    DELETE XBACK.LOG.SESS.NUM%
    XBACK.LOG.OPEN = FALSE                                              !HDC
    STATUS.MSG$ = STATUS.TEXT.MSG$(23)                                  !HDC
    GOSUB DISPLAY.STATUS.MSG

XBACK.LOG.NOT.PRESENT:                                                  !CJK

    FILE.OPERATION$ = "C"               ! File Create

    ! File error will be captured in ERROR.DETECTED                     !CJK
    CREATE POSFILE XBACK.LOG.LIVE.PATH$ AS XBACK.LOG.SESS.NUM%         \
                                                UNLOCKED LOCAL

    XBACK.LOG.OPEN = TRUE
    STATUS.MSG$ = STATUS.TEXT.MSG$(24)                                  !HDC
    GOSUB LOG.STATUS.MSG

    STATUS.MSG$ = STATUS.TEXT.MSG$(25)                                  !HDC
    GOSUB DISPLAY.STATUS.MSG

RETURN

\***********************************************************************
\*
\*    PROG.RUN.MODE: This Subroutine reads the BKPSCRPT file and stores
\*                   the Start time and Days to keep in a variable. It
\*                   also defines the backup file extension.
\*                   Then it validates the parameter. If it's a
\*                   SLEEPER run, respective Subroutine will be
\*                   processed. Else "Command Mode" Subroutine will
\*                   be processed.
\*
\***********************************************************************
PROG.RUN.MODE:

    IF MASTER$ = CE.CNTR$ THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(26)                              !KDC
    ENDIF ELSE BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(27)                              !KDC
    ENDIF
    GOSUB LOG.STATUS.MSG

    CURRENT.REPORT.NUM% = BKPSCRPT.REPORT.NUM%
    FILE.OPERATION$ = "O"                                               !CJK

    !-------------------------!
    ! Open Backup script file !
    !-------------------------!
    IF END # BKPSCRPT.SESS.NUM% THEN BKPSCRPT.OPEN.ERROR
    OPEN BKPSCRPT.FILE.NAME$ AS BKPSCRPT.SESS.NUM%
    BKPSCRPT.OPEN = TRUE
    VALUE.EXISTS = TRUE

    IF END # BKPSCRPT.SESS.NUM% THEN BKPSCRPT.READ.ERROR
    !----------------------------------------------!
    ! Extracting Time range and Days to Keep value !
    !----------------------------------------------!
    WHILE VALUE.EXISTS
        READ # BKPSCRPT.SESS.NUM%; LINE BKPSCRPT.VALUE$
        ! Finding comma position
        COMMA.POSITION% = MATCH(COMMA.VALUE$, BKPSCRPT.VALUE$, 1)

        !--------------------------------------!
        ! If comma present in the string value !
        !--------------------------------------!
        IF COMMA.POSITION% <> XBACK.ZERO% THEN BEGIN
            ! Storing the comma separated value
            BKPSCRPT.COMMAND$ = LEFT$(BKPSCRPT.VALUE$,                 \
                                     (COMMA.POSITION% - 1) )
            CALL TRIM(BKPSCRPT.COMMAND$)

            ! Remaining comma separated value
            BKPSCRPT.VALUE$ = MID$(BKPSCRPT.VALUE$,                    \
                                   (COMMA.POSITION% + 1),              \
                                   LEN(BKPSCRPT.VALUE$)   )

            !----------------------------!
            ! If command is "Time Range" !
            !----------------------------!
            IF BKPSCRPT.COMMAND$ = "TIME RANGE" THEN BEGIN
                ! Comma Position
                COMMA.POSITION% = MATCH(COMMA.VALUE$,BKPSCRPT.VALUE$,1) !OJK
                ! Extract start time
                BKPSCRPT.START.TIME$ = LEFT$(BKPSCRPT.VALUE$,          \
                                             (COMMA.POSITION% - 1))
                CALL TRIM(BKPSCRPT.START.TIME$)

                ! Remaining Backup script value                         !PJK
                BKPSCRPT.VALUE$ = MID$(BKPSCRPT.VALUE$,                \!PJK
                                       (COMMA.POSITION% + 1),          \!PJK
                                       LEN(BKPSCRPT.VALUE$)   )         !PJK
                !---------------------------!                           !PJK
                ! Extracting end time value !                           !PJK
                !---------------------------!                           !PJK
                                                                        !PJK
                CALL TRIM   (BKPSCRPT.VALUE$)                           !PJK
                CALL RTRIMC (BKPSCRPT.VALUE$, ASC(","))                 !PJK
                CALL TRIM   (BKPSCRPT.VALUE$)                           !PJK
                ! Storing END TIME                                      !PJK
                BKPSCRPT.END.TIME$ = BKPSCRPT.VALUE$                    !PJK

            ENDIF ELSE BEGIN
                !------------------------------!
                ! If command is "Days to Keep" !
                !------------------------------!
                IF BKPSCRPT.COMMAND$ = "DAYS TO KEEP" THEN BEGIN
                    ! Finding comma position
                    COMMA.POSITION%        = MATCH(COMMA.VALUE$,       \
                                                   BKPSCRPT.VALUE$,    \
                                                    1               )
                    ! Extract Days to Keep
                    BKPSCRPT.DAYS.TO.KEEP$ = LEFT$(BKPSCRPT.VALUE$,    \
                                                   (COMMA.POSITION%    \
                                                               - 1) )
                    CALL TRIM(BKPSCRPT.DAYS.TO.KEEP$)
                    VALUE.EXISTS = FALSE
                ENDIF
            ENDIF
        ENDIF
    WEND

    BKPSCRPT.ERROR = TRUE
    ! Checking START time                                               !PJK
    IF LEN(BKPSCRPT.START.TIME$) > 0 THEN BEGIN                         !LDC
        IF NOT FUNC.IS.VALID.TIME(BKPSCRPT.START.TIME$) THEN BEGIN      !LDC
            STATUS.MSG$ = STATUS.TEXT.ERROR$(9)                         !LDC
            GOSUB PROGRAM.EXIT                                          !LDC
        ENDIF                                                           !LDC
    ENDIF                                                               !LDC
    ! Checking END time                                                 !PJK
    IF LEN(BKPSCRPT.END.TIME$) > 0 THEN BEGIN                           !PJK
        IF NOT FUNC.IS.VALID.TIME(BKPSCRPT.END.TIME$) THEN BEGIN        !PJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(9)                         !PJK
            GOSUB PROGRAM.EXIT                                          !PJK
        ENDIF                                                           !PJK
    ENDIF                                                               !PJK
    ! Checking Days to Keep                                             !PJK
    IF VAL(BKPSCRPT.DAYS.TO.KEEP$) < XBACK.ZERO% THEN BEGIN             !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(9)                             !KDC
        GOSUB PROGRAM.EXIT                                              !KDC
    ENDIF ELSE \                                                        !KDC
    IF VAL(BKPSCRPT.DAYS.TO.KEEP$) = XBACK.ZERO% THEN BEGIN             !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(58)                            !KDC
        GOSUB PROGRAM.EXIT                                              !KDC
    ENDIF                                                               !KDC

    BKPSCRPT.ERROR = FALSE

    !-----------------------------------------------------------!
    ! Navigating the program according to the command parameter !
    !-----------------------------------------------------------!
    IF LEN(COMMAND.STRING$) = 1                   OR \ Command length 1 !RJK
       MATCH(PARM.ONS$, COMMAND.STRING$,8)  <> 0  OR \ OR Re-run        !RJK
       MATCH(PARM.BACKGRND$, COMMAND.STRING$,1) <> 0 THEN BEGIN         !RJK
        GOSUB COMMAND.MODE.OR.ONS.RUN                                   !KDC
    ENDIF ELSE BEGIN
        GOSUB PROG.SLEEPER.RUN                                          !KDC
    ENDIF


RETURN

BKPSCRPT.OPEN.ERROR:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(10)                                !KDC
    FILE.OPERATION$ = "O"                               ! File open
    GOSUB FILE.ERROR.EXIT

BKPSCRPT.READ.ERROR:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(11)                                !KDC
    FILE.OPERATION$ = "R"                               ! File read
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*    PROG.SLEEPER.RUN:     This Subroutine checks for any current day
\*                          zip files are present in XDISKIMG or
\*                          XDISKALT. If files are available then
\*                          program should end, logging an appropriate
\*                          error.
\*
\***********************************************************************
PROG.SLEEPER.RUN:

    STATUS.MSG$ = STATUS.TEXT.MSG$(28)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG                                            !KDC
    GOSUB LOG.STATUS.MSG                                                !KDC

    !----------------------------!
    ! If Master controller is CE !
    !----------------------------!
    IF MASTER$ = CE.CNTR$ THEN BEGIN
        !------------------------------------!
        ! Checking for current day ZIP files !
        !------------------------------------!
        CNTR.DIR$ = CE.C.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.1% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CE.D.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.2% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CE.C.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.3% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CE.D.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.4% = ZIP.FILE.EXISTS

        IF FILE.CHECK.1% OR FILE.CHECK.2% \
        OR FILE.CHECK.3% OR FILE.CHECK.4% \
        THEN BEGIN
            GOSUB PROGRAM.EXIT
        ENDIF

        !------------------------------------------!
        ! If Alternate controller available in LAN !
        !------------------------------------------!
        IF ALT.MASTER.ON THEN BEGIN
            !------------------------------------!
            ! Checking for current day ZIP files !
            !------------------------------------!
            CNTR.DIR$ = CF.C.XDISKIMG$
            GOSUB DOES.ZIP.FILE.EXISTS
            FILE.CHECK.1% = ZIP.FILE.EXISTS

            CNTR.DIR$ = CF.D.XDISKIMG$
            GOSUB DOES.ZIP.FILE.EXISTS
            FILE.CHECK.2% = ZIP.FILE.EXISTS

            CNTR.DIR$ = CF.C.XDISKALT$
            GOSUB DOES.ZIP.FILE.EXISTS
            FILE.CHECK.3% = ZIP.FILE.EXISTS

            CNTR.DIR$ = CF.D.XDISKALT$
            GOSUB DOES.ZIP.FILE.EXISTS
            FILE.CHECK.4% = ZIP.FILE.EXISTS

            IF FILE.CHECK.1% OR FILE.CHECK.2% \
            OR FILE.CHECK.3% OR FILE.CHECK.4% \
            THEN BEGIN
                GOSUB PROGRAM.EXIT
            ENDIF
        ENDIF
    ENDIF ELSE BEGIN

    !----------------------------!
    ! If Master controller is CF !
    !----------------------------!
        !------------------------------------!
        ! Checking for current day ZIP files !
        !------------------------------------!
        CNTR.DIR$ = CF.C.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.1% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CF.D.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.2% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CF.C.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.3% = ZIP.FILE.EXISTS

        CNTR.DIR$ = CF.D.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        FILE.CHECK.4% = ZIP.FILE.EXISTS

        IF FILE.CHECK.1% OR FILE.CHECK.2% \
        OR FILE.CHECK.3% OR FILE.CHECK.4% \
        THEN BEGIN
            GOSUB PROGRAM.EXIT
        ENDIF

    ENDIF

RETURN

\***********************************************************************
\*
\*    COMMAND.MODE.OR.ONS.RUN: This Subroutine does the action for
\*                             Command mode run or ONS re-run. It reads
\*                             the START TIME and END TIME from BKPSCRPT
\*                             and also deletes the current day files if
\*                             present.
\*
\***********************************************************************
COMMAND.MODE.OR.ONS.RUN:

    !-------------------------------------------!
    ! Defining the Process date and backup file !
    ! extension, using the Start and End time   !                       !PJK
    !-------------------------------------------!
    IF LEN(BKPSCRPT.START.TIME$) > 0 AND LEN(BKPSCRPT.END.TIME$) > 0 \  !PJK
    THEN BEGIN                                                          !PJK
        !-------------------------------------------------------!
        ! Checking the current time with BKPSCRPT Start time to !
        ! determine the processing day                          !
        !-------------------------------------------------------!
        ! calculate current time once for easy comparison               !KDC
        CURR.HH%     = VAL(LEFT$(CURR.TIME$,2))                         !KDC
        CURR.MM%     = VAL(MID$(CURR.TIME$,3,2))                        !KDC
        ! calculate script time once for easy comparison                !KDC
        BKPSCRPT.HH% = VAL(LEFT$(BKPSCRPT.START.TIME$,2))               !KDC
        BKPSCRPT.MM% = VAL(RIGHT$(BKPSCRPT.START.TIME$,2))              !KDC
        ! calculate script end time                                     !PJK
        BKPSCRPT.END.HH% = VAL(LEFT$(BKPSCRPT.END.TIME$,2))             !PJK
        BKPSCRPT.END.MM% = VAL(RIGHT$(BKPSCRPT.END.TIME$,2))            !PJK
                                                                        !KDC
        !-------------------------------------------------------!       !KDC
        ! if not zero and after script time or a later hour     !       !KDC
        ! and before end time then make the next day            !       !PJK
        !-------------------------------------------------------!       !KDC
        IF ((CURR.HH% = BKPSCRPT.HH% AND CURR.MM% > = BKPSCRPT.MM%) \   !PJK
            OR CURR.HH% > BKPSCRPT.HH%  )                       AND \   !PJK
           ((CURR.HH% = BKPSCRPT.END.HH%   AND                      \   !PJK
            CURR.MM% <= BKPSCRPT.END.MM%)  OR                       \   !PJK
             CURR.HH% < BKPSCRPT.END.HH%)                 THEN BEGIN    !PJK

            F02.DATE$ = CURR.DATE$
            RETURN.VALUE.CHECK% = UPDATE.DATE( 1 )  ! Add one day       !KDC

            GOSUB CHECK.UPDATE.DATE.RC                                  !FJK

            CURR.DATE$ = F02.DATE$
        ENDIF
    ENDIF                                                               !LDC
    !-----------------------------------!
    ! Defining the Extension name from  !
    ! date and month value              !
    !-----------------------------------!
    EXTENSION$ = RIGHT$(CURR.DATE$,4)       ! Extracting Month value    !KDC
    GOSUB GET.FILE.EXTENSION                                            !KDC
    EXT.MDD$ = EXTENSION$                                               !KDC

    !------------------------------------!
    ! Deleting the current day zip files !
    !------------------------------------!
    IF MASTER$ = CE.CNTR$ THEN BEGIN               ! If Master is CE

        CNTR.DIR$ = CE.C.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CE.C.XDISKIMG$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CE.D.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CE.D.XDISKIMG$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CE.C.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CE.C.XDISKALT$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CE.D.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CE.D.XDISKALT$ + "*." + EXT.MDD$)
        ENDIF

    ENDIF

    ! If Master is CF or Alternate controller available                 !DJK
    IF ALT.MASTER.ON OR MASTER$ = CF.CNTR$ THEN BEGIN                   !DJK

        CNTR.DIR$ = CF.C.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CF.C.XDISKIMG$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CF.D.XDISKIMG$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CF.D.XDISKIMG$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CF.C.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CF.C.XDISKALT$ + "*." + EXT.MDD$)
        ENDIF

        CNTR.DIR$ = CF.D.XDISKALT$
        GOSUB DOES.ZIP.FILE.EXISTS
        IF ZIP.FILE.EXISTS THEN BEGIN
            ! Deleting the current day files
            CALL OSSHELL("DEL " + CF.D.XDISKALT$  + "*." + EXT.MDD$)
        ENDIF
    ENDIF

RETURN

\***********************************************************************
\*
\*    DOES.ZIP.FILE.EXISTS: This function checks the existence of
\*                          current day ZIP file.
\*
\***********************************************************************
DOES.ZIP.FILE.EXISTS:

    ! flag to indicate where we are processing
    ZIP.FILE.EXISTS = 2

    ! Extracting today's zip file details
    CALL OSSHELL("DIR " + CNTR.DIR$  + "*." + EXT.MDD$  + " > " + \
                  DIR.OUT$ + " >>* " + DIR.OUT$ )

    IF END # TEMP.SESS.NUM% THEN ZIP.FILE.EXISTS.EXIT
    OPEN DIR.OUT$ AS TEMP.SESS.NUM%

    ! Setting the temporary file open
    TEMP.OPEN = TRUE

    ! Ignoring the first 4 lines
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

    ! Checking the file presence                                        !CJK
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    ! default to false
    ZIP.FILE.EXISTS = 0                                                 !KDC
    IF LEN(DIR.VALUE$) <> XBACK.ZERO% THEN BEGIN
        ZIP.FILE.EXISTS = TRUE
        STATUS.MSG$ = STATUS.TEXT.MSG$(1)                               !KDC
    ENDIF

    ! Deleting the file as no longer needed
    DELETE TEMP.SESS.NUM%

    ! Setting the temporary file close
    TEMP.OPEN = FALSE                                                   !FJK

RETURN

ZIP.FILE.EXISTS.EXIT:

    STATUS.MSG$ = STATUS.TEXT.ERROR$(2)                                 !KDC
!    ZIP.FILE.EXISTS = TRUE                                             !OJK

RETURN


\***********************************************************************
\*
\*    CREATE.FILES: This Subroutine Checks the file existence of
\*                  BKPEXCL.DAT. If file is present already delete it.
\*                  Then create the same files as a new one. Also
\*                  create BKPFAIL files in respective directory.       !OJK
\*
\***********************************************************************
CREATE.FILES:

    CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%
    FILE.OPERATION$ = "O"                                               !CJK

    !-------------------------------------!
    ! If BKPEXCL file exists, deleting it !
    !-------------------------------------!
    IF END # BKPEXCL.SESS.NUM% THEN BKPEXCL.NOT.PRESENT                 !CJK
    OPEN BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM%
    BKPEXCL.OPEN = TRUE                                                 !HDC

    FILE.OPERATION$ = "D"                                               !CJK
    
    DELETE BKPEXCL.SESS.NUM%
    BKPEXCL.OPEN = FALSE                                                !LDC

    STATUS.MSG$ = STATUS.TEXT.MSG$(29)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

BKPEXCL.NOT.PRESENT:                                                    !CJK
!    ! Commented out this flag as it is redundant                       !OJK
!    BKPEXCL.OPEN = FALSE                                               !OJK

    FILE.OPERATION$ = "C"               ! File Create

    ! File error will be captured in ERROR.DETECTED
    CREATE POSFILE BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM% \
                                            UNLOCKED LOCAL

    BKPEXCL.OPEN = TRUE

    ! Setting BKPFAILC values for creating BKPFAILC file                !OJK
    CURRENT.REPORT.NUM% = BKPFAILC.REPORT.NUM%                          !OJK
    BKPFAILC.FILE.NAME$ = BKPFAILC.FILE.NAME$ + EXT.MDD$                !OJK

    ! File error will be captured in ERROR.DETECTED                     !OJK
    CREATE POSFILE BKPFAILC.FILE.NAME$ AS BKPFAILC.SESS.NUM% \          !OJK
                                                UNLOCKED LOCAL          !OJK

    BKPFAILC.OPEN = TRUE                                                !OJK

    ! Setting BKPFAILD values for creating BKPFAILD file                !OJK
    CURRENT.REPORT.NUM% = BKPFAILD.REPORT.NUM%                          !OJK
    BKPFAILD.FILE.NAME$ = BKPFAILD.FILE.NAME$ + EXT.MDD$                !OJK

    ! File error will be captured in ERROR.DETECTED
    CREATE POSFILE BKPFAILD.FILE.NAME$ AS BKPFAILD.SESS.NUM% \          !OJK
                                                UNLOCKED LOCAL          !OJK

    BKPFAILD.OPEN = TRUE                                                !OJK

    STATUS.MSG$ = STATUS.TEXT.MSG$(30)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

RETURN

\***********************************************************************
\*
\*    OPEN.AND.READ.XBKOK: Opens the XBKOK status-and-configuration file.
\*                         The XBKOK should always exist and be readable
\*                         so if missing or cannot be read an event 106
\*                         is logged via the FILE.ERROR.EXIT routine.
\*
\***********************************************************************
OPEN.AND.READ.XBKOK:

    FILE.OPERATION$     = "O"                       ! Open
    CURRENT.REPORT.NUM% = XBKOK.REPORT.NUM%

    !*******************************************************************!KDC
    !* set local error to handle missing OK file only                   !KDC
    !* other errors will handle in the main ON ERROR                    !KDC
    !*******************************************************************!KDC
    IF END # XBKOK.SESS.NUM% THEN XBKOK.FILE.NOT.FOUND                  !KDC
    OPEN XBKOK.FILE.NAME$ DIRECT RECL XBKOK.RECL% AS XBKOK.SESS.NUM%    !KDC
    XBKOK.OPEN = TRUE                                                   !OJK
                                                                        !KDC
 XBKOK.FILE.OPEN.ERROR:                                                 !OJK
    !*******************************************************************!KDC
    !* check if file not open must be a create error                    !KDC
    !*******************************************************************!KDC
    IF NOT XBKOK.OPEN THEN BEGIN                                        !OJK
        !***************************************************************!KDC
        !* set to normal IF END error handling for files in case of     !KDC
        !* other issues then create the OK file and set the write with  !KDC
        !* starting record details                                      !KDC
        !***************************************************************!KDC
        IF END # XBKOK.SESS.NUM% THEN FILE.ERROR.EXIT                   !KDC
        CREATE POSFILE XBKOK.FILE.NAME$ DIRECT 1 RECL XBKOK.RECL% AS \  !KDC
                       XBKOK.SESS.NUM% MIRRORED PERUPDATE               !OJK
        XBKOK.OPEN = TRUE                                               !LDC
        STATUS.MSG$ = STATUS.TEXT.MSG$(31)                              !KDC
        GOSUB DISPLAY.STATUS.MSG                                        !KDC
        ! Assigning defaults                                            !KDC
        XBKOK.START.DATE$ = DATE$                                       !KDC
        XBKOK.START.TIME$ = LEFT$(TIME$,4)                              !KDC
        XBKOK.STATUS$     = STATUS.START$                               !KDC
        XBKOK.TYPE$       = RUN.TYPE$                                   !KDC
    ENDIF ELSE BEGIN                                                    !KDC
        FILE.RC2%   = READ.XBKOK                                        !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(12)                            !KDC
        GOSUB CHECK.FILE.RC2                                            !KDC
                                                                        !KDC
        IF SIZE(XBKOK.FILE.NAME$) <> XBKOK.RECL% THEN BEGIN             !KDC
            FILE.OPERATION$ = "R"                       ! Read          !KDC
            STATUS.MSG$     = STATUS.TEXT.ERROR$(13)                    !KDC
            GOSUB FILE.ERROR.EXIT                                       !KDC
        ENDIF                                                           !KDC
    ENDIF                                                               !KDC

    STATUS.MSG$ = STATUS.TEXT.MSG$(32)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

    XBKOK.INTERIM.STATUS$ = STATUS.START$                               !BJK

    ! Assigning the Start date and time                                 !BJK
    XBKOK.START.DATE$ = DATE$                                           !BJK
    XBKOK.START.TIME$ = LEFT$(TIME$,4)                                  !BJK

RETURN

\***********************************************************************!KDC
\*                                                                      !KDC
\*    XBKOK.FILE.NOT.FOUND: Displays a message and updates LOG and      !KDC
\*                          logs the error in the normal way            !KDC
\*                                                                      !KDC
\***********************************************************************!KDC
XBKOK.FILE.NOT.FOUND:                                                   !KDC
    XBKOK.OPEN = FALSE                                                  !KDC
    STATUS.MSG$ = STATUS.TEXT.ERROR$(14)                                !KDC
    GOSUB DISPLAY.STATUS.MSG                                            !KDC
                                                                        !KDC
    ! Logging event 106 for File error                                  !KDC
    STATUS.MSG$ = STATUS.TEXT.ERROR$(44) + FILE.OPERATION$              !KDC
    GOSUB DISPLAY.STATUS.MSG                                            !KDC
    GOSUB LOG.STATUS.MSG                                                !KDC
                                                                        !KDC
    EVENT.NUMBER% = 106     ! Event 106                                 !KDC
                                                                        !KDC
    ! Application event log data                                        !KDC
    VAR.STRING.1$ = FILE.OPERATION$                                  + \!KDC
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,8))               + \!KDC
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,XBACK.ZERO%))        !KDC
                                                                        !KDC
    GOSUB CALL.F01.APPLICATION.LOG                                      !KDC
    GOTO XBKOK.FILE.OPEN.ERROR                                          !KDC

\***********************************************************************
\*
\*    UPDATE.XBKOK: Calls WRITE.XBKOK to update the XBKOK.
\*                  Displays a message and updates LOG
\*
\***********************************************************************
UPDATE.XBKOK:

    IF NOT XBKOK.OPEN THEN BEGIN    ! If XBKOK not open                 !CJK
        RETURN
    ENDIF

    ! Assigning the Interim status and Backup type before update
    XBKOK.STATUS$ = XBKOK.INTERIM.STATUS$
    XBKOK.TYPE$   = RUN.TYPE$                                           !DJK

    STATUS.MSG$ = STATUS.TEXT.ERROR$(15)                                !KDC
    FILE.RC2%   = WRITE.XBKOK
    GOSUB CHECK.FILE.RC2

    ! Re-aligned the IF statements                                      !BJK
    IF XBKOK.INTERIM.STATUS$ = STATUS.START$ THEN BEGIN                 !KDC
        ! Started status
        STATUS.MSG$ = STATUS.TEXT.MSG$(34)                              !KDC
    ENDIF ELSE IF XBKOK.INTERIM.STATUS$ = STATUS.END$ THEN BEGIN
        ! Success status
        STATUS.MSG$ = STATUS.TEXT.MSG$(35)                              !KDC
    ENDIF ELSE BEGIN
        ! Major error
        STATUS.MSG$ = STATUS.TEXT.ERROR$(16)                            !KDC
    ENDIF

    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

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
\*    BKPLIST.FULL.CHECK: This Subroutine checks the day of the week.   !KDC
\*                        If matching Full day from configuration using !KDC
\*                        (Sleeper), create a new BKPLIST.MDD file. For !KDC
\*                        Non Full (Incremental) run, it also stores the!KDC
\*                        BKPLIST Line details into an array            !KDC
\*
\***********************************************************************
! Below subroutine checks on the execution day based on that it does    !URG
! appropriate actions.We dont need to maintain the BKPLIST file if we   !URG
! are not doing the incremental backup.                                 !URG

! BKPLIST.FULL.CHECK:                                                   !URG KDC

 ! Setting the success status
 !     XBKOK.INTERIM.STATUS$ = STATUS.END$                              !URG

 ! Getting the day using PSDATE function
 !    PSDATE.DATE$ = CURR.DATE$                                         !URG KDC
 !    CALL PSDATE(CURR.DATE$)                                           !URG
 ! Below piece of code is to copy the BKPLIST file on sunday to         !URG
 ! sunday basis, so that xrestore functionality is not affected         !URG

 !   IF F13.DAY$ = "SUN" THEN BEGIN                                     !URG
 !       GOSUB GET.DAY.AND.CHECK.ERROR                                  !KDC
 !       GOSUB DETERMINE.BKPLIST.EXT                                    !OJK
 !       BKPLI.FILE.NAME$ = BKPLI.FILE.NAME$ + EXT.MDD$                 !URG
 !       BKPLI.FILE.NEW$ = LEFT$(BKPLI.FILE.NAME$,20) + ORG.EXT$        !URG
 !       CALL ADXCOPYF(ADXSERVE.RC%,BKPLI.FILE.NEW$,            \       !URG
 !                 BKPLI.FILE.NAME$,0,0,0)                              !URG

 !   ENDIF                                                              !URG
 ! Code follows are commented as no need to read through BKPLIST file   !URG
 ! and sort to array, instead the last run of BKPLIST file will be      !URG
 ! Continued for ever for XRESTORE.                                     !URG

 !   IF F13.DAY$ = FULL.DAY$ THEN BEGIN             ! If Full           !URG KDC

        ! Setting FULL backup as this is the first run of the week      !DJK
 !       RUN.TYPE$ = "F"                                                !URG DJK

 !       FILE.OPERATION$     = "C"              ! File Create           !URG
 !       CURRENT.REPORT.NUM% = BKPLI.REPORT.NUM%                        !URG

        ! Setting the BKPLIST file name
 !       BKPLI.FILE.NAME$ = BKPLI.FILE.NAME$ + EXT.MDD$                 !URG

 !       CREATE POSFILE BKPLI.FILE.NAME$ AS BKPLI.SESS.NUM%             \URG
 !                                       LOCKED \                       !URG LDC
 !                                       MIRRORED PERUPDATE             !URG
 !       BKPLI.OPEN   = TRUE                                            !URG
 !       STATUS.MSG$  = STATUS.TEXT.ERROR$(17)                          !URG KDC
 !       GOSUB DISPLAY.STATUS.MSG                                       !URG
 !       GOSUB LOG.STATUS.MSG                                           !URG

 !   ENDIF ELSE BEGIN                                                   !URG

 !       GOSUB DETERMINE.BKPLIST.EXT                                    !URG OJK

        ! Setting the BKPLIST file name
 !       BKPLI.FILE.NAME$ = BKPLI.FILE.NAME$ + FULL.EXT.MDD$            !URG KDC

 !       FILE.OPERATION$     = "O"                                      !URG CJK
 !       CURRENT.REPORT.NUM% = BKPLI.REPORT.NUM%                        !URG CJK

 !       GOSUB OPEN.BKPLIST.FILE                                        !URG GJK

 !       VALUE.PRESENT = TRUE                                           !URG
 !       ARRAY.INDEX%  = XBACK.ZERO%                                    !URG

 !       FILE.OPERATION$     = "R"                                      !URG CJK
 !       CURRENT.REPORT.NUM% = BKPLI.REPORT.NUM%                        !URG CJK

        !------------------------------------------------!
        ! Storing the BKPLIST file details into an array !
        !------------------------------------------------!
 !       WHILE VALUE.PRESENT                                            !URG 
 !           IF READ.BKPLI THEN BEGIN      ! force exit on fail         !URG KDC
 !               VALUE.PRESENT = FALSE                                  !URG
 !           ENDIF ELSE BEGIN                                           !URG
 !               ARRAY.INDEX% = ARRAY.INDEX% + 1   ! Increment the index!URG

                !-----------------------------------------------------!
                ! If Array limit reached, second array is initialized !
                !-----------------------------------------------------!
 !               IF ARRAY.INDEX% = ARRAY.LIMIT% THEN BEGIN              !URG
 !                   DIM BKPLIST.SECOND.ARRAY$(ARRAY.LIMIT%)            !URG
 !                   BKPLIST.ARRAY$(ARRAY.INDEX%) = BKPLI.VALUE$        !URG KDC
 !               ENDIF ELSE BEGIN                                       !URG

                    !--------------------------------------------------!
                    ! Once Array limit crossed, second array has been  !!KDC
                    ! set ON and program starts using second array     !!KDC
                    !--------------------------------------------------!
 !                   IF ARRAY.INDEX% > ARRAY.LIMIT% THEN BEGIN          !URG
 !                       SECOND.ARRAY.ON     = TRUE                     !URG
 !                       ! Increment the index                       
 !                       ARRAY.SECOND.INDEX% = ARRAY.SECOND.INDEX% + 1  !URG

                        !--------------------------------------!
                        ! Checking the index of second array   !
                        ! before writing in array              !
                        !--------------------------------------!
 !                      IF ARRAY.SECOND.INDEX% > ARRAY.LIMIT% THEN BEGIN!URG
 !                          STATUS.MSG$ = STATUS.TEXT.ERROR$(18)     + \!URG KDC
 !                               STR$(ARRAY.LIMIT% * NUM.OF.ARRAYS%) + \!URG OJK
 !                               STATUS.TEXT.ERROR$(19)                 !URG OJK
 !                          GOSUB DISPLAY.STATUS.MSG                    !URG
 !                          GOSUB LOG.STATUS.MSG                        !URG
 !                          STATUS.MSG$ = STATUS.TEXT.ERROR$(20)        !URG KDC
 !                          XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$ !URG
 !                          GOSUB PROGRAM.EXIT                          !URG
 !                      ENDIF                                           !URG
 !                      BKPLIST.SECOND.ARRAY$(ARRAY.SECOND.INDEX%) = \  !URG
 !                                                          BKPLI.VALUE$!URG KDC
 !                   ENDIF ELSE BEGIN                                   !URG
 !                       BKPLIST.ARRAY$(ARRAY.INDEX%) = BKPLI.VALUE$    !URG KDC
 !                   ENDIF                                              !URG
 !               ENDIF                                                  !URG
 !           ENDIF                                                      !URG KDC
 !       WEND                                                           !URG
 !    BKPLST.COMPL:                                                     !OJK

        !-------------------------------------------!
        ! Not closing the BKPLIST file, as the file !
        ! should not be accessed by any other       !
        ! application throughout the program run    !
        !-------------------------------------------!
        !reset the first array counter to the max if the secondary one  !URG QDC
        ! is in use                                                     !URG QDC
 !       IF ARRAY.INDEX% > ARRAY.LIMIT% THEN ARRAY.INDEX% = ARRAY.LIMIT%!URG QDC

 !       STATUS.MSG$ = STR$(ARRAY.INDEX%)        + "+" + \              !URG RJK
 !                     STR$(ARRAY.SECOND.INDEX%) + " = " + \            !URG RJK
 !                     STR$(ARRAY.INDEX% + ARRAY.SECOND.INDEX%) + \     !URG QDC
 !                     STATUS.TEXT.MSG$(36)                             !URG KDC
 !       GOSUB DISPLAY.STATUS.MSG                                       !URG
 !       GOSUB LOG.STATUS.MSG                                           !URG

 !   ENDIF                                                              !URG

RETURN

\***********************************************************************!GJK
\*                                                                      !GJK
\*    OPEN.BKPLIST.FILE: This Subroutine opens the BKPLIST file if      !GJK
\*                       present. If not present, then creates a new    !GJK
\*                       file for the current week.                     !GJK
\*                                                                      !GJK
\***********************************************************************!GJK
! Commenting out the subroutine as BKPLIST file will not be maintained  !URG
! going forward.
!OPEN.BKPLIST.FILE:                                                     !URG

    ! Open the BKPLIST file if present
!    IF END # BKPLI.SESS.NUM% THEN BKPLI.NOT.PRESENT                    !URG
!    OPEN BKPLI.FILE.NAME$ AS BKPLI.SESS.NUM% LOCKED                    !URG

!    BKPLI.OPEN   = TRUE                                                !URG
!    STATUS.MSG$  = STATUS.TEXT.MSG$(37)                                !URG KDC
!    GOSUB DISPLAY.STATUS.MSG                                           !URG
!    GOSUB LOG.STATUS.MSG                                               !URG

!    RETURN                                                             !URG

!BKPLI.NOT.PRESENT:                                                     !URG 

  !  STATUS.MSG$ = STATUS.TEXT.ERROR$(21)                               !URG KDC
  !  GOSUB DISPLAY.STATUS.MSG                                           !URG
  !  GOSUB LOG.STATUS.MSG                                               !URG

    ! Create the BKPLIST file as it is not present
  !  CREATE POSFILE BKPLI.FILE.NAME$ AS BKPLI.SESS.NUM% LOCKED \        !URG OJK
  !                                         MIRRORED PERUPDATE          !URG OJK
  !  BKPLI.OPEN   = TRUE
  !  STATUS.MSG$  = STATUS.TEXT.MSG$(38)                                !URG KDC
  !  GOSUB DISPLAY.STATUS.MSG
  !  GOSUB LOG.STATUS.MSG

  ! Setting FULL backup as this is the first run of the week            !URG DJK
  ! RUN.TYPE$ = "F"                                                     !URG DJK

RETURN

\***********************************************************************
\*
\*    DETERMINE.BKPLIST.EXT: This Subroutine determines the BKPLIST file
\*                           name's extension and set the file name.
\*
\***********************************************************************
! Below subroutine needs to be commented out as it decides the          !URG
! extension type                                                        !URG

!DETERMINE.BKPLIST.EXT:                                                 !URG

!    F02.DATE$ = CURR.DATE$                                             !URG
!    GOSUB HOW.MANY.DAYS.SINCE.FULL                                     !URG KDC
   
!    RETURN.VALUE.CHECK% = UPDATE.DATE(-DAYS.AFTER.FULL.BAKUP%)         !URG

!    GOSUB CHECK.UPDATE.DATE.RC                                         !URG FJK

    ! Setting the Full date                                             !KDC
!    FULL.DATE$ = F02.DATE$                                             !URG KDC
    
!    RETURN.VALUE.CHECK% = UPDATE.DATE(-7)                              !URG


!    GOSUB CHECK.UPDATE.DATE.RC                                         !URG

!    ORG.DATE$ = F02.DATE$                                              !URG

    !--------------------------------------------------------!
    ! Defining the Extension name from date and month value  !
    !--------------------------------------------------------!
!    EXTENSION$ = RIGHT$(FULL.DATE$,4)      ! Extracting Month value    !URG KDC
!    GOSUB GET.FILE.EXTENSION                                           !URG KDC
!    FULL.EXT.MDD$ = EXTENSION$                                         !URG KDC
!    IF VAL(MID$(CURR.DATE$,3,2)) >  9 THEN BEGIN                       !URG
!        ORG.EXT$ = LEFT$(EXTENSION$,1) + RIGHT$(ORG.DATE$,2)           !URG
!    ENDIF ELSE BEGIN                                                   !URG
!       ORG.EXT$ = RIGHT$(ORG.DATE$,3)                                  !URG
!    ENDIF                                                              !URG

!    STATUS.MSG$ = STATUS.TEXT.MSG$(57)                                 !URG KDC
!    GOSUB LOG.STATUS.MSG                                               !URG JJK
!    GOSUB DISPLAY.STATUS.MSG                                           !URG JJK

    !----------------------------------!                                !JJK
    ! Deleting the older BKPLIST files !
    !----------------------------------!
    ! Determining 3 weeks prior BKPLIST file
!    RETURN.VALUE.CHECK% = UPDATE.DATE(-VAL(BKPSCRPT.DAYS.TO.KEEP$) - 7)!URG KDC
!    GOSUB CHECK.UPDATE.DATE.RC                                         !URG JJK

!    EXTENSION$ = RIGHT$(F02.DATE$,4)   ! Extracting Month value        !URG KDC
!    GOSUB GET.FILE.EXTENSION                                           !URG KDC
!    FULL.DEL.EXT.MDD$ = EXTENSION$                    !KDC

    ! Old BKPLIST file delete in D:/ADX_UDT1/                           !URG OJK
!    CALL OSSHELL("DEL D:\ADX_UDT1\BKPLIST." + FULL.DEL.EXT.MDD$ + \    !URG KDC
!                 " > " + DIR.OUT$ +  " >>* " + DIR.OUT$ )              !URG JJK
    ! Old BKPLIST file delete in C:/ADX_UDT1/                           !URG OJK
!    CALL OSSHELL("DEL C:\ADX_UDT1\BKPLIST." + FULL.DEL.EXT.MDD$ + \    !URG OJK
!                 " > " + DIR.OUT$ +  " >>* " + DIR.OUT$ )              !URG OJK

RETURN

\***********************************************************************
\*
\*    HOW.MANY.DAYS.SINCE.FULL: This Subroutine matches the day set
\*                              in F13.DAY and sets the number of days
\*                              since the Full backup was taken for
\*                              this day
\*
\***********************************************************************
HOW.MANY.DAYS.SINCE.FULL:

    ! match for the day and set using the offset as days before
    DAYS.AFTER.FULL.BAKUP% = MATCH(":"+F13.DAY$, BACKUP.DAYS$,1)        !KDC
    DAYS.AFTER.FULL.BAKUP% = (DAYS.AFTER.FULL.BAKUP%-1)/4               !KDC

RETURN


\***********************************************************************
\*
\*    PROCESS.BKPSCRPT: This Subroutine reads the BKPSCRPT and  stores
\*                      the EXCLUDE items in BKPEXCL.DAT and also
\*                      initiate all the processing required for the
\*                      Backup process.
\*
\***********************************************************************
PROCESS.BKPSCRPT:
    
    !--------------------------------------------------!                !DJK
    ! Logging/Displaying the backup type for reference !                !DJK
    !--------------------------------------------------!                !DJK
    IF RUN.TYPE$ = "F" THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(39)                              !KDC
    ENDIF ELSE BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(40)                              !KDC
    ENDIF

    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

    VALUE.EXISTS = TRUE
    INCLUDE.RUN  = FALSE

    STATUS.MSG$ = STATUS.TEXT.MSG$(46)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(41)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(42)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(43)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(44)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(45)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.MSG$(46)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    WAIT;2000   ! Allow two second for user
                ! to read the instructions

    IF END # BKPSCRPT.SESS.NUM% THEN BKPSCRPT.READ.ERR
    FILE.OPERATION$     = "R"                                           !CJK
    CURRENT.REPORT.NUM% = BKPSCRPT.REPORT.NUM%                          !CJK

    !-----------------------------------------------------------------!
    ! Extracting EXCLUDE and BACKUP and performing respective process !
    !-----------------------------------------------------------------!
    WHILE VALUE.EXISTS
        READ # BKPSCRPT.SESS.NUM%; LINE BKPSCRPT.VALUE$
        ! Comma position
        COMMA.POSITION% = MATCH(COMMA.VALUE$,BKPSCRPT.VALUE$,1)

        IF COMMA.POSITION% <> XBACK.ZERO% THEN BEGIN
            ! Extracting Backup script command
            BKPSCRPT.COMMAND$ = LEFT$(BKPSCRPT.VALUE$,                 \
                                     (COMMA.POSITION% - 1) )
            CALL TRIM(BKPSCRPT.COMMAND$)

            ! Remaining Backup script value
            BKPSCRPT.VALUE$ = MID$(BKPSCRPT.VALUE$,                    \
                                   (COMMA.POSITION% + 1),              \
                                   LEN(BKPSCRPT.VALUE$)   )
            !-----------------------!
            ! If command is Backup  !
            !-----------------------!
            IF BKPSCRPT.COMMAND$ = "BACKUP" THEN BEGIN
                ! To avoid first loop run
                IF INCLUDE.RUN THEN BEGIN

                    ! Check the directory existence
                    GOSUB DIRECTORY.EXISTENCE

                    !-------------------------------------!
                    ! BACKUP.OFF would be set TRUE if the !
                    ! directory is not present            !
                    !-------------------------------------!
                    IF BACKUP.OFF THEN BEGIN
                        ! Directory not exist
                        STATUS.MSG$ = BKPSCRPT.DIRECTORY$            + \
                                      STATUS.TEXT.ERROR$(22)            !KDC
                        GOSUB DISPLAY.STATUS.MSG
                        GOSUB LOG.STATUS.MSG
                    ENDIF ELSE BEGIN
                        ! Directory Archiving
                        STATUS.MSG$ = BKPSCRPT.DIRECTORY$            + \
                                      STATUS.TEXT.MSG$(47)              !KDC
                        GOSUB DISPLAY.STATUS.MSG
                        GOSUB LOG.STATUS.MSG

                        ! Checking the files for Backing up
                        GOSUB BKPLIST.HOUSEKEEPING
                        
                        ! Write to BKPEXCL file and to make sure        !TSM
                        ! Write routine is not called if there are no   !TSM
                        ! Exclusion files present for the subdirecories !TSM
                        
                        IF BKPEXCL.COMP.INDEX% > 1 THEN BEGIN           !TSM
                            GOSUB WRITE.BKPEXCL                         !TSM
                        ENDIF                                           !TSM
                        ! Backup process
                        GOSUB BACKUP.PROCESS

                    ENDIF

                    ! Reset the flag
                    EXCLUDE.PRESENT = FALSE

                ENDIF
                !-----------------------------------!
                ! To start the Backup process after !
                ! skipping the first loop           !
                !-----------------------------------!
                INCLUDE.RUN = TRUE                                      !NJK

                !------------------------------------------!
                ! Extracting Backup script directory value !
                !------------------------------------------!

                ! Comma position
                COMMA.POSITION% = MATCH(COMMA.VALUE$,BKPSCRPT.VALUE$,1) !OJK
                ! Backup directory
                BKPSCRPT.DIRECTORY$  = LEFT$(BKPSCRPT.VALUE$,          \
                                             (COMMA.POSITION% - 1))

                CALL TRIM(BKPSCRPT.DIRECTORY$)

                ! Remaining Backup script value
                BKPSCRPT.VALUE$ = MID$(BKPSCRPT.VALUE$,                \
                                       (COMMA.POSITION% + 1),          \
                                       LEN(BKPSCRPT.VALUE$)   )

                !-------------------------------------------------!
                ! Extracting Backup script output directory value !
                !-------------------------------------------------!

                ! Comma position
                COMMA.POSITION%         = MATCH(COMMA.VALUE$,          \
                                                BKPSCRPT.VALUE$,       \
                                                1)
                ! Output directory
                BKPSCRPT.OUT.FILE.NAME$ = LEFT$(BKPSCRPT.VALUE$,       \
                                               (COMMA.POSITION% - 1))
                CALL TRIM(BKPSCRPT.OUT.FILE.NAME$)

                ! Remaining Backup script value
                BKPSCRPT.VALUE$ = MID$(BKPSCRPT.VALUE$,                \
                                       (COMMA.POSITION% + 1),          \
                                       LEN(BKPSCRPT.VALUE$)   )

                !-----------------------------------!
                ! Backup script output if CF master !
                !-----------------------------------!
                IF MASTER$ <> CE.CNTR$ THEN BEGIN
                    ! Output directory
                    BKPSCRPT.OUT.FILE.NAME$ = BKPSCRPT.VALUE$
                    CALL TRIM(BKPSCRPT.OUT.FILE.NAME$)
                ENDIF

            ENDIF ELSE BEGIN
            !-----------------------!
            ! If command is Exclude !
            !-----------------------!
                IF BKPSCRPT.COMMAND$ = "EXCLUDE" THEN BEGIN
                    ! Comma position
                    COMMA.POSITION%          = MATCH(COMMA.VALUE$,     \
                                                     BKPSCRPT.VALUE$,  \
                                                      1)
                    ! File exclusion
                    BKPSCRPT.FILE.EXCLUSION$ = LEFT$(BKPSCRPT.VALUE$,  \
                                                     (COMMA.POSITION%  \
                                                                 - 1) )
                    CALL TRIM(BKPSCRPT.FILE.EXCLUSION$)
                    !Add the files to be excluded in Exclude array      !TSM
                    BKPEXCL.ARRAY$(BKPEXCL.ARRAY.INDEX%)=              \!TSM
                                               BKPSCRPT.FILE.EXCLUSION$ !TSM
                    ! Add the files to be excluded in Complete exclude  !TSM
                    ! array                                             !TSM
                    BKPEXCL.COMP.ARRAY$(BKPEXCL.COMP.INDEX%) =         \!TSM
                                               BKPSCRPT.FILE.EXCLUSION$ !TSM
                    !Increment the exclude array index                  !TSM
                    BKPEXCL.ARRAY.INDEX% = BKPEXCL.ARRAY.INDEX% + 1     !TSM
                    !Increment the complete array index                 !TSM
                    BKPEXCL.COMP.INDEX%  = BKPEXCL.COMP.INDEX% + 1      !TSM
                    
                    ! Commented out the subroutine call as it is being  !TSM
                    ! done above in the code                            !TSM
                    !GOSUB WRITE.BKPEXCL                                !TSM

                    ! Set the Boolean ON
                    EXCLUDE.PRESENT = TRUE
                ENDIF
            ENDIF
        ENDIF
    WEND

BKPSCRPT.READ.ERR:

    !-------------------------------------!
    ! Last directory backup from BKPSCRPT !
    !-------------------------------------!

    ! Check the directory existence
    GOSUB DIRECTORY.EXISTENCE

    !-------------------------------------!
    ! BACKUP.OFF would be set TRUE if the !
    ! directory is not present            !
    !-------------------------------------!
    IF BACKUP.OFF THEN BEGIN
        ! Directory not exist
        STATUS.MSG$ = BKPSCRPT.DIRECTORY$ + STATUS.TEXT.ERROR$(22)      !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB LOG.STATUS.MSG
    ENDIF ELSE BEGIN
        ! Directory Archiving
        STATUS.MSG$ = BKPSCRPT.DIRECTORY$ + STATUS.TEXT.MSG$(47)        !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB LOG.STATUS.MSG

        GOSUB BKPLIST.HOUSEKEEPING
        IF BKPEXCL.COMP.INDEX% > 1 THEN BEGIN                           !TSM
            GOSUB WRITE.BKPEXCL                                         !TSM
        ENDIF                                                           !TSM
        GOSUB BACKUP.PROCESS

    ENDIF

    !-----------------------------------------------------------------!
    ! Closing the relevant sessions as BKPSCRPT file read is complete !
    !-----------------------------------------------------------------!
    CLOSE BKPEXCL.SESS.NUM%
    BKPEXCL.OPEN = FALSE                                                !LDC
    ! Deallocate session
    CALL SESS.NUM.UTILITY ("C",BKPEXCL.SESS.NUM%,XBACK.NULL$)

    ! Non Re-used sessions, hence closing the file directly             !OJK
    CLOSE BKPFAILC.SESS.NUM%                                            !OJK
    BKPFAILC.OPEN = FALSE                                               !OJK
    ! Deallocate session                                                !OJK
    CALL SESS.NUM.UTILITY ("C",BKPFAILC.SESS.NUM%,XBACK.NULL$)          !OJK

    ! Non Re-used sessions, hence closing the file directly             !OJK
    CLOSE BKPFAILD.SESS.NUM%                                            !OJK
    BKPFAILD.OPEN = FALSE                                               !OJK
    ! Deallocate session                                                !OJK
    CALL SESS.NUM.UTILITY ("C",BKPFAILD.SESS.NUM%,XBACK.NULL$)          !OJK

    ! Non Re-used sessions, hence closing the file directly
    CLOSE BKPSCRPT.SESS.NUM%
    BKPSCRPT.OPEN = FALSE                                               !LDC
    ! Deallocate session
    CALL SESS.NUM.UTILITY ("C",BKPSCRPT.SESS.NUM%,XBACK.NULL$)

! HSIUF session will be handled in File function itself. De-allocation  !OJK
! has been moved to more appropriate place. SLPCF session de-allocation !OJK
! is redundant and hence commented out                                  !OJK
!    ! Deallocate session                                               !OJK
!    CALL SESS.NUM.UTILITY ("C",HSIUF.SESS.NUM%,XBACK.NULL$)            !OJK
!    HSIUF.OPEN = FALSE                                                 !OJK

!    ! Deallocate session                                               !OJK
!    CALL SESS.NUM.UTILITY ("C",SLPCF.SESS.NUM%,XBACK.NULL$)            !OJK
!    SLPCF.OPEN = FALSE                                                 !OJK

    STATUS.MSG$ = STATUS.TEXT.MSG$(48)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG
RETURN

\***********************************************************************
\*
\*   WRITE.BKPEXCL: Writes exclusion details to BKPEXCL.DAT
\*
\***********************************************************************
WRITE.BKPEXCL:    

    CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%
    FILE.OPERATION$     = "W"                   ! File write
    
    !Correct the index value                                            !TSM
    BKPEXCL.COMP.INDEX% = BKPEXCL.COMP.INDEX% - 1                       !TSM
    !Copy the content of complete exclude array to BKPEXCL file         !TSM
    FOR BKPEXCL.COMP.POS% = 1 TO BKPEXCL.COMP.INDEX%                    !TSM

        !BKPSCRPT.FILE.EXCLUSION$ = BKPSCRPT.FILE.EXCLUSION$ + CRLF$    !TSM

        BKPSCRPT.FILE.EXCLUSION$ = BKPEXCL.COMP.ARRAY$                 \!TSM
                                           (BKPEXCL.COMP.POS%) + CRLF$  !TSM
        ! Form String
        BKPEXCL.FORM$ = "C" + STR$(LEN(BKPSCRPT.FILE.EXCLUSION$))

        IF END # BKPEXCL.SESS.NUM% THEN WRITE.BKPEXCL.ERROR
        WRITE FORM BKPEXCL.FORM$; # BKPEXCL.SESS.NUM%;                 \
                             BKPSCRPT.FILE.EXCLUSION$
    NEXT BKPEXCL.COMP.POS%                                              !TSM


RETURN

WRITE.BKPEXCL.ERROR:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(23)                                !KDC
    GOSUB FILE.ERROR.EXIT
RETURN
\***********************************************************************
\*
\*    DIRECTORY.EXISTENCE: This Subroutine checks the existence
\*                         of current directory.
\*
\***********************************************************************
DIRECTORY.EXISTENCE:

    ! Setting the variables
    REMAINING.VALUE$  = BKPSCRPT.DIRECTORY$
    DRIVE$            = LEFT$(REMAINING.VALUE$,2)   ! Current drive
    DIRECTORY.SEARCH$ = XBACK.NULL$
    SLASH.POSITION%   = XBACK.ZERO%
    BACKUP.OFF        = FALSE

    !---------------------------------------------!
    ! To check the directory existence of given   !
    ! sequence of directories under current drive !
    !---------------------------------------------!
    WHILE (LEN(REMAINING.VALUE$) <> SLASH.POSITION%) AND \
    BACKUP.OFF = FALSE
        ! Backward slash position
        SLASH.POSITION%  = MATCH("\\", REMAINING.VALUE$, 1)

        REMAINING.VALUE$ = RIGHT$(REMAINING.VALUE$,                    \
                              (LEN(REMAINING.VALUE$) - SLASH.POSITION%))!OJK
        ! Backward slash position
        SLASH.POSITION%  = MATCH("\\", REMAINING.VALUE$, 1)

        !---------------------!
        ! Directory to search !
        !---------------------!

        ! Extracting the directory value
        DIREC.TO.SEARCH$ = LEFT$(REMAINING.VALUE$,(SLASH.POSITION% - 1))

        ! Appending the slash
        DIRECTORY.SEARCH$ = DIRECTORY.SEARCH$ + "\"

        !--------------------------!
        ! Current Search directory !
        !--------------------------!
        DIRECTORY.SEARCH.VALUE$ = DRIVE$ + DIRECTORY.SEARCH$

        !----------------------------------!
        ! Verifying the directory presence !
        !----------------------------------!
        GOSUB CHECK.DIR.NOT.EXIST

        !----------------------------------!
        ! If last run and directory exists !
        !----------------------------------!
        IF LEN(REMAINING.VALUE$) = SLASH.POSITION% AND \                !OJK
           BACKUP.OFF            = FALSE     THEN BEGIN                 !OJK
            ! Getting file details from current directory
            CALL OSSHELL("DIR " + BKPSCRPT.DIRECTORY$                + \
                         "*.*" + " > " + DIR.SEC.OUT$ )
        ENDIF
        DIRECTORY.SEARCH$ = DIRECTORY.SEARCH$ + DIREC.TO.SEARCH$

    WEND

RETURN
! HDC START BLOCK                                                       ! HDC
\***********************************************************************
\*
\*    CHECK.DIR.NOT.EXIST: This checks the existence of a given
\*                          directory.
\*
\***********************************************************************
CHECK.DIR.NOT.EXIST:

    ! Directory list details
    CALL OSSHELL("DIR -T " + DIRECTORY.SEARCH.VALUE$ + " > " + \
                 DIR.OUT$ + " >>* " + DIR.OUT$)                         !KDC

    IF END # TEMP.SESS.NUM% THEN DIR.NOT.EXISTS.ERR
    OPEN DIR.OUT$ AS TEMP.SESS.NUM%                                     !KDC
    ! Setting the temporary file open
    TEMP.OPEN = TRUE

    ! Ignoring the first 4 lines
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

    BACKUP.OFF     = TRUE
    VALUE.EXISTS.2 = TRUE

    WHILE VALUE.EXISTS.2
        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

        ! Match found
        IF MATCH((LEFT$(DIREC.TO.SEARCH$+STRING$(13," "),13) + "<DIR>")\!KDC
        , DIR.VALUE$, 1) <> XBACK.ZERO% \                               !KDC
        THEN BEGIN
            BACKUP.OFF = FALSE
        ENDIF

        ! IF EOF reached
        IF LEN(DIR.VALUE$) = XBACK.ZERO% THEN BEGIN
            VALUE.EXISTS.2 = FALSE
        ENDIF

    WEND

    ! Deleting the file as no longer needed
    DELETE TEMP.SESS.NUM%
    TEMP.OPEN = FALSE                                                   !LDC
RETURN

DIR.NOT.EXISTS.ERR:

    BACKUP.OFF = TRUE
    STATUS.MSG$ = STATUS.TEXT.ERROR$(24)                                !KDC

RETURN
! HDC END BLOCK                                                         !HDC

\***********************************************************************
\*
\*    BKPLIST.HOUSEKEEPING: This Subroutine stores the file details of
\*                          current directory and navigate the call
\*                          based on the current day.
\*
\***********************************************************************
BKPLIST.HOUSEKEEPING: 
    ! As the status setting in BKPLIST.FULL.CHECK has been  commented   !URG
    ! setting the status here                                           !URG
    XBKOK.INTERIM.STATUS$ = STATUS.END$                                 !URG
    IF END # TEMP.SESS.NUM.2% THEN ERROR.DIR.OPEN

    !--------------------------------------------!
    ! If directory exists, read the file details !
    ! under the directory                        !
    !--------------------------------------------!
    IF NOT BACKUP.OFF THEN BEGIN                                        !CJK

        FILE.OPERATION$     = "O"                                       !CJK
        CURRENT.REPORT.NUM% = TEMP.REPORT.NUM.2%                        !CJK

        OPEN DIR.SEC.OUT$ AS TEMP.SESS.NUM.2%

        ! Setting the temporary file open
        TEMP.2.OPEN = TRUE                                              !FJK

        FILE.OPERATION$     = "R"                                       !CJK

        ! Ignoring the first 4 lines
        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$
        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$
        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$
        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$

        FILE.PRESENT = TRUE

        !-----------------------------------------------------!
        ! Navigating the routine depending on the current day !
        !-----------------------------------------------------!
        ! if the day is the Full backup day                             !URG KDC
       ! IF F13.DAY$ = FULL.DAY$ THEN BEGIN                             !URG KDC
            GOSUB BKPLIST.FULL                                          !URG KDC
       ! ENDIF ELSE BEGIN                                               !URG
       !     GOSUB BKPLIST.NON.FULL                                     !URG KDC
       ! ENDIF                                                          !URG

        ! Deleting the file as no longer needed
        FILE.OPERATION$ = "D"                                           !CJK
        DELETE TEMP.SESS.NUM.2%
        ! Setting the temporary file close
        TEMP.2.OPEN = FALSE                                             !FJK

    ENDIF

    RETURN

ERROR.DIR.OPEN:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(25)
    FILE.OPERATION$ = "O"                       ! File open
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*    BKPLIST.NON.FULL: This Subroutine performs the BACKUP process     !KDC
\*                      for NON SUNDAY run.                             !KDC
\*
\***********************************************************************
! No need of below subroutine as we dont need to do incremental         !URG
! backup                                                                !URG
!BKPLIST.NON.FULL:                                                      !URG KDC

!    FILE.OPERATION$     = "R"                                          !URG CJK
!    CURRENT.REPORT.NUM% = TEMP.REPORT.NUM.2%                           !URG CJK

!    IF END # TEMP.SESS.NUM.2% THEN ERROR.DIR.READ1                     !URG
    !--------------------------------------------------!
    ! Looping until file name present. Storing all the !
    ! file details from the current directory          !
    !--------------------------------------------------!
!    WHILE FILE.PRESENT                                                 !URG
!        BKPEXCL.RUN = TRUE                                             !URG
!        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$                   !URG

        !------------------------------------!
        ! Making sure length value is non    !
        ! zero and it's not a directory      !
        !------------------------------------!
!        IF LEN(DIR.SEC.VALUE$)              <> XBACK.ZERO%         AND \URG
!           MATCH("<DIR>", DIR.SEC.VALUE$, 1) = XBACK.ZERO%             \URG
!           THEN BEGIN                                                  !URG
!           FILE.NAME.VALUE$ = LEFT$(DIR.SEC.VALUE$,8)   ! File name    !URG 
!           FILE.EXT.VALUE$  = MID$(DIR.SEC.VALUE$,10,3) ! and extension!URG
!           CALL TRIM(FILE.NAME.VALUE$)                                 !URG
!            CALL TRIM(FILE.EXT.VALUE$)                                 !URG

            ! Complete file name regardless of extension                !URG
!            IF FILE.EXT.VALUE$ <> STRING$(3," ") THEN BEGIN ! Extension!URG
!                DIR.FILE.NAME$ = FILE.NAME.VALUE$ + "." + \ ! Present  !URG
!                                 FILE.EXT.VALUE$                       !URG
!            ENDIF ELSE BEGIN                                           !URG
!                DIR.FILE.NAME$ = FILE.NAME.VALUE$                      !URG
!            ENDIF                                                      !URG

!            IF EXCLUDE.PRESENT THEN BEGIN                              !URG
            !-----------------------------------------------------!
            ! Closing the BKPEXCL session as the existing session !
            ! might be in APPEND mode                             !
            !-----------------------------------------------------!
!                CLOSE BKPEXCL.SESS.NUM%                                !URG
!                BKPEXCL.OPEN = FALSE                                   !URG

!                FILE.OPERATION$     = "O"                              !URG
!                CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%              !URG

                !--------------------------------------------!
                ! Opening and reading BKPEXCL to avoid       !
                ! exclusion list file to be added in BKPLIST !
                !--------------------------------------------!
!                IF END # BKPEXCL.SESS.NUM% THEN BKPEXCL.OPEN.COMP1     !URG
!                OPEN BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM%           !URG
!                IF END # BKPEXCL.SESS.NUM% THEN BKPEXCL.READ.COMP1     !URG

!                BKPEXCL.OPEN        = TRUE                             !URG FJK
!                FILE.OPERATION$     = "R"                              !URG CJK
!                CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%              !URG CJK

!                WHILE BKPEXCL.RUN                                      !URG  
!                    READ # BKPEXCL.SESS.NUM%; LINE BKPEXCL.VALUE$      !URG 
!                    IF UCASE$(BKPEXCL.VALUE$) = UCASE$(DIR.FILE.NAME$) \URG
!                    THEN BEGIN                                         !URG
!                        ! Setting False if EXCLUDE file matches        !URG
!                        BKPEXCL.RUN = FALSE                            !URG
!                    ENDIF                                              !URG
!                WEND                                                   !URG
!                BKPEXCL.READ.COMP1:                                    !URG

!            ENDIF

            !-----------------------------------!
            ! If file is not present in BKPEXCL !
            !-----------------------------------!
!            IF BKPEXCL.RUN THEN BEGIN                                  !URG

!                FILE.POSITION%     = XBACK.ZERO%                       !URG
!                FILE.IS.PRESENT$   = XBACK.NULL$                       !URG
!                FILE.IS.PRESENT$   = XBACK.NULL$                       !URG
!                COMMA.POSITION.2%  = XBACK.ZERO%                       !URG
!                FIRST.ARRAY.FOUND  = FALSE                             !URG
!                SECOND.ARRAY.FOUND = FALSE                             !URG

!                BKPLIST.COMP.FILENAME$ = BKPSCRPT.DIRECTORY$         + \URG
!                                         DIR.FILE.NAME$

                !-----------------------------------------------!
                ! Checking the file existence in BKPLIST array. !
                ! If present, the array index will be saved for !
                ! updating it respectively                      !
                !-----------------------------------------------!
!                FOR INDEX% = 1 TO ARRAY.INDEX%                         !URG
                                                                        !URG RJK
!                    IF INDEX% > 9999 THEN BEGIN                        !URG
!                    COMMA.POSITION.2% = 0                              !URG
!                    ENDIF                                              !URG RJK
!                    COMMA.POSITION.2% = MATCH(COMMA.VALUE$,            \URG
!                                             BKPLIST.ARRAY$(INDEX%),   \URG
!                                                                 1)    !URG
!                    IF COMMA.POSITION.2% THEN BEGIN                    !URG RJK
!                        IF LEFT$(BKPLIST.ARRAY$(INDEX%),               \URG
!                                 (COMMA.POSITION.2% - 1))          =   \URG
!                           BKPLIST.COMP.FILENAME$                      \URG
!                        THEN BEGIN                                     !URG
!                            FILE.POSITION%    = INDEX%                 !URG
!                            INDEX%            = ARRAY.INDEX%           !URG
!                            FIRST.ARRAY.FOUND = TRUE                   !URG
!                        ENDIF                                          !URG
!                    ENDIF                                              !URG RJK
!                NEXT INDEX%                                            !URG

                !--------------------------------!
                ! Checking the file existence in !
                ! BKPLIST array 2 if present     !
                !--------------------------------!
!                IF SECOND.ARRAY.ON AND FILE.POSITION% = XBACK.ZERO%    \URG
!                THEN BEGIN                                             !URG
!                    FOR SECOND.INDEX% = 1 TO ARRAY.SECOND.INDEX%       !URG
!                        COMMA.POSITION.2% =                            \URG
!                                 MATCH(COMMA.VALUE$,                   \URG
!                                 BKPLIST.SECOND.ARRAY$(SECOND.INDEX%), \URG
!                                 1)                                    !URG
!                        IF LEFT$(BKPLIST.SECOND.ARRAY$(SECOND.INDEX%), \URG
!                                 (COMMA.POSITION.2% - 1))           =  \URG
!                           BKPLIST.COMP.FILENAME$                      \URG
!                        THEN BEGIN                                     !URG
!                            FILE.POSITION%     = SECOND.INDEX%         !URG
!                            SECOND.INDEX%      = ARRAY.SECOND.INDEX%   !URG
!                            SECOND.ARRAY.FOUND = TRUE                  !URG
!                        ENDIF                                          !URG
!                    NEXT SECOND.INDEX%                                 !URG
!                ENDIF                                                  !URG

                !--------------------------------------!
                ! If file is found in BKPLIST array(s) !
                !--------------------------------------!
!                IF FILE.POSITION% <> XBACK.ZERO% THEN BEGIN            !URG
!                    FILE.IS.PRESENT$ = XBACK.YES$                      !URG 

                    !-------------------------------------------------! !DJK
                    ! If backup run is FULL or Archive flag is ON     !
                    ! then FILE.HAS.CHANGED$ will be set to 'Y', else !
                    ! NULL value will be set and the file details     !
                    ! will be added to exclusion list.                !
                    !-------------------------------------------------!
!                    GOSUB IS.ARCHIVE.FLAG.ON                           !URG HDC
!                    IF RUN.TYPE$ = "F" OR IS.ARCHIVE.ON THEN BEGIN     !URG OJK

!                        FILE.HAS.CHANGED$ = XBACK.YES$                 !URG 

                        ! Set OFF Archive attribute
!                        CALL OSSHELL("FSET "                     + \   !URG
!                                      BKPLIST.COMP.FILENAME$     + \   !URG
!                                      " -A=OFF >> " + DIR.OUT$   + \   !URG
!                                      " >>* " + DIR.OUT$  )            !URG
!                    ENDIF ELSE BEGIN                                   !URG
!                        ! Setting Null for File changed flag
!                        FILE.HAS.CHANGED$ = XBACK.NULL$                !URG

!                        EXCLUDE.PRESENT = TRUE                         !URG

!                        FILE.OPERATION$     = "O"                      !URG CJK
!                        CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%      !URG CJK

                        !---------------------------------------!
                        ! Closing the session to open in APPEND !
                        ! mode to update the file               !
                        !---------------------------------------!
!                        CLOSE BKPEXCL.SESS.NUM%                        !URG  
!                        BKPEXCL.OPEN = FALSE                           !URG FJK

!                        IF END # BKPEXCL.SESS.NUM% THEN                \URG
!                                            BKPEXCL.OPEN.COMP1         !URG
!                        OPEN BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM%   \URG
!                                                              APPEND   !URG
!                        BKPEXCL.OPEN = TRUE                            !URG FJK

!                        BKPSCRPT.FILE.EXCLUSION$ = DIR.FILE.NAME$      !URG
!                        GOSUB WRITE.BKPEXCL                            !URG
!                    ENDIF                                              !URG


                    !-----------------------------------!
                    ! To identify in which array the    !
                    ! file has been found and update    !
                    ! accordingly                       !
                    !-----------------------------------!
!                    IF FIRST.ARRAY.FOUND THEN BEGIN                    !URG KDC
!                        TODAY.BKPLIST.REC$ = \                         !URG KDC
!                            BKPLIST.ARRAY$(FILE.POSITION%)             !URG OJK
!                        GOSUB UPD.TODAY.BKPLIST.REC                    !URG KDC
!                        BKPLIST.ARRAY$(FILE.POSITION%) = \             !URG KDC
!                            TODAY.BKPLIST.REC$                         !URG OJK
!                    ENDIF ELSE BEGIN                                   !URG KDC
!                        TODAY.BKPLIST.REC$ = \                         !URG KDC
!                            BKPLIST.SECOND.ARRAY$(FILE.POSITION%)      !URG OJK
!                        GOSUB UPD.TODAY.BKPLIST.REC                    !URG KDC
!                        BKPLIST.SECOND.ARRAY$(FILE.POSITION%) = \      !URG KDC
!                            TODAY.BKPLIST.REC$                         !URG OJK
!                    ENDIF                                              !URG KDC

                    ! Reset the flag
!                    FIRST.ARRAY.FOUND  = FALSE                         !URG BJK
!                    SECOND.ARRAY.FOUND = FALSE                         !URG BJK

                ! If file is not found in BKPLIST
!                ENDIF ELSE BEGIN                                       !URG 

                    !---------------------------------!
                    ! If file is not present in array !
                    ! prepare to add as a new record  !
                    !---------------------------------!
!                    FILE.IS.PRESENT$  = XBACK.YES$                     !URG
!                    FILE.HAS.CHANGED$ = XBACK.YES$                     !URG

                    !--------------------------------!
                    ! Checking which array to select !
                    !--------------------------------!
!                    IF NOT SECOND.ARRAY.ON THEN BEGIN                  !URG

!                        GOSUB CREATE.BKPLIST.RECD                      !URG
!                        BKPLIST.ARRAY$(INDEX%) = BKPLIST.RECD$         !URG KDC

!                        ARRAY.INDEX% = INDEX%                          !URG

!                    ENDIF ELSE BEGIN                                   !URG

                        !----------------------------------!
                        ! If second array limit is reached !
                        !----------------------------------!
!                       IF SECOND.INDEX% > ARRAY.LIMIT% THEN BEGIN      !URG
!                           STATUS.MSG$ = STATUS.TEXT.ERROR$(18)     + \!URG KDC
!                                STR$(ARRAY.LIMIT% * NUM.OF.ARRAYS%) + \!URG OJK
!                                STATUS.TEXT.ERROR$(19)                 !URG OJK
!                           GOSUB DISPLAY.STATUS.MSG                    !URG
!                           GOSUB LOG.STATUS.MSG                        !URG
!                           STATUS.MSG$ = STATUS.TEXT.ERROR$(20)        !URG KDC
!                           XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$ !URG
!                           GOSUB PROGRAM.EXIT                          !URG
!                       ENDIF                                           !URG

!                        GOSUB CREATE.BKPLIST.RECD                      !URG KDC
!                        BKPLIST.SECOND.ARRAY$(SECOND.INDEX%) = \       !URG KDC
!                                                 BKPLIST.RECD$         !URG KDC

!                        ARRAY.SECOND.INDEX% = SECOND.INDEX%            !URG
!                    ENDIF                                              !URG

                    !-------------------------------------------!
                    ! If array limit reached in the first array !
                    ! make the second array ON                  !
                    !-------------------------------------------!
!                    IF INDEX% = ARRAY.LIMIT%                       AND \URG
!                       SECOND.ARRAY.ON = FALSE THEN BEGIN              !URG
!                        DIM BKPLIST.SECOND.ARRAY$(ARRAY.LIMIT%)        !URG  
!                        SECOND.ARRAY.ON = TRUE                         !URG
!                    ENDIF                                              !URG 

                    ! Set OFF Archive attribute
!                    CALL OSSHELL("FSET "                     + \       !URG
!                                  BKPLIST.COMP.FILENAME$     + \       !URG
!                                  " -A=OFF >> " + DIR.OUT$   + \       !URG
!                                  " >>* " + DIR.OUT$  )                !URG
!                ENDIF                                                  !URG
!            ENDIF                                                      !URG
!        ENDIF ELSE BEGIN                                               !URG
!            ! If the file reached End of Line                           
!            IF LEN(DIR.SEC.VALUE$) = XBACK.ZERO% THEN BEGIN            !URG
!                FILE.PRESENT = FALSE                                   !URG
!            ENDIF                                                      !URG
!        ENDIF                                                          !URG
!    WEND                                                               !URG

! RETURN                                                                !URG 

!KDC START CHANGE BLOCK
\***********************************************************************
\*
\*    UPD.TODAY.BKPLIST.REC:    This subroutine uses the line value
\*                              of BKPLIST, process it and return
\*                              with updated value based on current day
\***********************************************************************
! Commenting the subroutine as it is used during incremental backup to  !URG
! to update a record in BKPLIST file based on the change                !URG
!UPD.TODAY.BKPLIST.REC:                                                 !URG
!
!    TODAY.BKPLIST.REC% = 1                                             !URG LDC
!    ! set in case of error failure
!    TODAY.BKPLIST.REC.ERR$ = TODAY.BKPLIST.REC$                        !URG
!    BKPLI.VALUE$           = TODAY.BKPLIST.REC$                        !URG
!
!    BEGIN.POS%    = 1           ! Begin search position                !URG
!    INDEX%        = XBACK.ZERO% ! Index for Field Array                !URG
!    COMMA.PRESENT = TRUE        ! While Boolean                        !URG
!
!    DIM FIELDS$(0)                 ! Clear the array first             !URG
!    DIM FIELDS$(15)                ! To store BKPLIST values           !URG
!
!    !----------------------------------------------------------!
!    ! Extracting all the variables using comma separator value !
!    !----------------------------------------------------------!
!    WHILE COMMA.PRESENT                                                !URG
!        ! Get index of next field delimiter                            !URG 
!        MATCH.POS% = MATCH(COMMA.VALUE$,BKPLI.VALUE$,BEGIN.POS%)       !URG KDC
!
!        INDEX% = INDEX% + 1     ! Incrementing the index               !URG
!
!        IF MATCH.POS% > XBACK.ZERO% THEN BEGIN                         !URG
!            ! If we found a field delimiter                            !URG
!            ! Get contents of field
!            FIELDS$(INDEX%) = MID$(BKPLI.VALUE$, BEGIN.POS%, \         !URG KDC
!                                 (MATCH.POS% - BEGIN.POS%)  )          !URG
!
!            ! Move next start position past field delimiter
!            BEGIN.POS% = MATCH.POS% + 1                                !URG
!        ENDIF ELSE BEGIN                                               !URG
!            ! Else we're at the last field                             !URG
!            COMMA.PRESENT = FALSE                                      !URG
!        ENDIF                                                          !URG
!    WEND                                                               !URG
!
!    ! Storing the values                                               !URG BJK
!    BKPLI.FILENAME$                 = FIELDS$(1)                       !URG KDC
!    BKPLI.FULL.EXIST$               = FIELDS$(2)                       !URG KDC
!    BKPLI.FULL.FILE.CHNG$           = FIELDS$(3)                       !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(1)     = FIELDS$(4)                       !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(1) = FIELDS$(5)                       !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(2)     = FIELDS$(6)                       !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(2) = FIELDS$(7)                       !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(3)     = FIELDS$(8)                       !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(3) = FIELDS$(9)                       !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(4)     = FIELDS$(10)                      !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(4) = FIELDS$(11)                      !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(5)     = FIELDS$(12)                      !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(5) = FIELDS$(13)                      !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(6)     = FIELDS$(14)                      !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(6) = FIELDS$(15)                      !URG KDC
!
!    GOSUB HOW.MANY.DAYS.SINCE.FULL                                     !URG KDC
!
!    BKPLI.INCREMENTAL.EXIST$(DAYS.AFTER.FULL.BAKUP%) = \               !URG  
!        FILE.IS.PRESENT$                                               !URG 
!
!    BKPLI.INCREMENTAL.FILE.CHNG$(DAYS.AFTER.FULL.BAKUP%) = \           !URG  
!        FILE.HAS.CHANGED$                                              !URG
!
!    ! Storing the updated BKPLIST values
!    TODAY.BKPLIST.REC$ =  \                                            !URG KDC
!        BKPLI.FILENAME$                 + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.FULL.EXIST$               + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.FULL.FILE.CHNG$           + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(1)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(1) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(2)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(2) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(3)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(3) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(4)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(4) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(5)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(5) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(6)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(6) + COMMA.VALUE$                 !URG KDC
!
!RETURN                                                                 !URG  
!
!UPD.TODAY.BKPLIST.REC.ERR:                                             !URG 
!    TODAY.BKPLIST.REC% = 0                                             !URG LDC
!    TODAY.BKPLIST.REC$ = TODAY.BKPLIST.REC.ERR$                        !URG 
!    GOSUB LOG.STATUS.MSG                                               !URG  
!    GOSUB DISPLAY.STATUS.MSG                                           !URG  
!RETURN                                                                 !URG

\***********************************************************************
\*
\*    CREATE.BKPLIST.RECD:  This subroutine creates an empty list
\*                          entry for the correct day and builds the
\*                          arrays accordingly
\*
\***********************************************************************
! Commenting the subroutine as no need to create the backup list record !URG
! as it is done to maintain the incremental backup                      !URG
!CREATE.BKPLIST.RECD:                                                   !URG

    ! Setting the BKPLIST file variables
!    BKPLI.FILENAME$       = BKPLIST.COMP.FILENAME$                     !URG OJK
!    BKPLI.FULL.EXIST$     = XBACK.NULL$                                !URG OJK
!    BKPLI.FULL.FILE.CHNG$ = XBACK.NULL$                                !URG OJK
!    DIM BKPLI.INCREMENTAL.EXIST$(0)                                    !URG KDC
!    DIM BKPLI.INCREMENTAL.FILE.CHNG$(0)                                !URG KDC
!    DIM BKPLI.INCREMENTAL.EXIST$(6)                                    !URG KDC
!    DIM BKPLI.INCREMENTAL.FILE.CHNG$(6)                                !URG KDC

    ! Setting the values based on the DAY
!    GOSUB HOW.MANY.DAYS.SINCE.FULL                                     !URG KDC
                                                                        !URG KDC
!    BKPLI.INCREMENTAL.EXIST$(DAYS.AFTER.FULL.BAKUP%) = \               !URG KDC
!        FILE.IS.PRESENT$                                               !URG KDC
                                                                        !URG KDC
!    BKPLI.INCREMENTAL.FILE.CHNG$(DAYS.AFTER.FULL.BAKUP%) = \           !URG KDC
!        FILE.HAS.CHANGED$                                              !URG KDC

    ! Storing the updated BKPLIST values
!    BKPLIST.RECD$ =  \                                                 !URG KDC
!        BKPLI.FILENAME$                 + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.FULL.EXIST$               + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.FULL.FILE.CHNG$           + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(1)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(1) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(2)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(2) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(3)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(3) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(4)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(4) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(5)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(5) + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.EXIST$(6)     + COMMA.VALUE$ + \             !URG KDC
!        BKPLI.INCREMENTAL.FILE.CHNG$(6) + COMMA.VALUE$                 !URG KDC
!
!RETURN                                                                 !URG  
!!KDC END CHANGE BLOCK                                                  !URG
!
!ERROR.DIR.READ1:                                                       !URG 
!    STATUS.MSG$ = STATUS.TEXT.ERROR$(26)                               !URG KDC
!    FILE.OPERATION$ = "R"               ! File read                    !URG  
!    GOSUB FILE.ERROR.EXIT                                              !URG
!
!BKPEXCL.OPEN.COMP1:                                                    !URG
!    STATUS.MSG$ = STATUS.TEXT.ERROR$(27)                               !URG KDC
!    FILE.OPERATION$ = "O"               ! File open                    !URG
!    GOSUB FILE.ERROR.EXIT                                              !URG 
!
!RETURN                                                                 !URG
!
!!HDC START BLOCK                                                       !URG 
\***********************************************************************
\*
\*    IS.ARCHIVE.FLAG.ON: This subroutine checks the ARCHIVE flag of the!OJK
\*                        file BKPLIST.COMP.FILENAME$
\*
\***********************************************************************
!Commenting the subroutine as its used for checking the file change     !URG
!status.                                                                !URG
!IS.ARCHIVE.FLAG.ON:                                                    !URG
!
!    IS.ARCHIVE.ON = FALSE                                              !URG 
!
!    ! Ignore if file contains '-'
!    IF MATCH("-",BKPLIST.COMP.FILENAME$,1) = XBACK.ZERO% THEN BEGIN    !URG
!        ! Storing the Archive values
!        CALL OSSHELL("FSET " + BKPLIST.COMP.FILENAME$ + " > " + \      !URG OJK
!                     DIR.OUT$ + " >>* " + DIR.OUT$)                    !URG OJK
!    ENDIF ELSE BEGIN                                                   !URG
!        RETURN                                                         !URG  
!    ENDIF                                                              !URG
!
!    IF END # TEMP.SESS.NUM% THEN ARCHIVE.ERROR                         !URG
!    OPEN DIR.OUT$ AS TEMP.SESS.NUM%                                    !URG 
!    ! Setting the temporary file open 
!    TEMP.OPEN = TRUE                                                   !URG
!
!    ! Ignoring the first line
!    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$                             !URG
!    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$                             !URG
!
!    ! Match found
!    IF MATCH("A=ON", DIR.VALUE$, 1) <> XBACK.ZERO% THEN BEGIN          !URG 
!        IS.ARCHIVE.ON = TRUE                                           !URG 
!    ENDIF                                                              !URG
!
!    !---------------------------------------!
!    ! Closing the session. No deallocation, !
!    ! as the session will be reused         !
!    !---------------------------------------!
!    CLOSE TEMP.SESS.NUM%                                               !URG
!    ! Setting the temporary file close                           
!    TEMP.OPEN = FALSE                                                  !URG 
!
!RETURN                                                                 !URG
!
!ARCHIVE.ERROR:                                                         !URG
!
!    IS.ARCHIVE.ON = TRUE                                               !URG  
!
!RETURN                                                                 !URG
!! HDC END BLOCK                                                        !URG
\***********************************************************************
\*
\*    BKPLIST.FULL: This Subroutine performs the BACKUP process for the !KDC
\*                  for Full day run.                                   !KDC
\*
\***********************************************************************
BKPLIST.FULL:                                                           !KDC

    IF END # TEMP.SESS.NUM.2% THEN ERROR.DIR.READ

    ! Setting the BKPLIST file variables
!    BKPLI.FULL.EXIST$     = XBACK.YES$                                 !URG OJK
!    BKPLI.FULL.FILE.CHNG$ = XBACK.YES$                                 !URG OJK
!    DIM BKPLI.INCREMENTAL.EXIST$(0)                                    !URG KDC
!    DIM BKPLI.INCREMENTAL.FILE.CHNG$(0)                                !URG KDC
!    DIM BKPLI.INCREMENTAL.EXIST$(6)                                    !URG KDC
!    DIM BKPLI.INCREMENTAL.FILE.CHNG$(6)                                !URG KDC

    FILE.OPERATION$     = "R"                                           !CJK
    CURRENT.REPORT.NUM% = TEMP.REPORT.NUM.2%                            !CJK

    !---------------------------------!
    ! Looping until file name present !
    !---------------------------------!
    WHILE FILE.PRESENT
        BKPEXCL.RUN = TRUE
        READ # TEMP.SESS.NUM.2%; LINE DIR.SEC.VALUE$

        !------------------------------------!
        ! Making sure length value is non    !
        ! zero and it's not a directory      !
        !------------------------------------!
        IF LEN(DIR.SEC.VALUE$)              <> XBACK.ZERO%         AND \
           MATCH("<DIR>", DIR.SEC.VALUE$, 1) = XBACK.ZERO%             \
           THEN BEGIN
            FILE.NAME.VALUE$ = LEFT$(DIR.SEC.VALUE$,8)   ! File name
            FILE.EXT.VALUE$  = MID$(DIR.SEC.VALUE$,10,3) ! and extension
            CALL TRIM(FILE.NAME.VALUE$)
            CALL TRIM(FILE.EXT.VALUE$)

            ! Complete file name regardless of extension                !KDC
            IF FILE.EXT.VALUE$ <> STRING$(3," ") THEN BEGIN ! Extension !KDC
                DIR.FILE.NAME$ = FILE.NAME.VALUE$ + "." + \ ! Present   !KDC
                                 FILE.EXT.VALUE$                        !KDC
            ENDIF ELSE BEGIN                                            !KDC
                DIR.FILE.NAME$ = FILE.NAME.VALUE$                       !KDC
            ENDIF                                                       !KDC

            ! If exclude file present
            IF EXCLUDE.PRESENT THEN BEGIN
               
                !Set the index value to 1                               !TSM
                BKPEXCL.INDEX% = 1                                      !TSM
                
              !Commenting out the code as it is no longer used          !TSM
               !---------------------------------------------!
               ! Closing the existing session as it has been !
               ! opened in APPEND mode                       !
               !---------------------------------------------!
               !CLOSE BKPEXCL.SESS.NUM%                                 !TSM
               !BKPEXCL.OPEN = FALSE                                    !TSM
                
               !FILE.OPERATION$     = "O"                               !TSM
               !CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%               !TSM
               !
               !!--------------------------------------------!
               !! Opening and reading BKPEXCL to avoid       !
               !! exclusion list file to be added in BKPLIST !
               !!--------------------------------------------!
               !IF END # BKPEXCL.SESS.NUM% THEN BKPEXCL.OPEN.COMP       !TSM
               !OPEN BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM%            !TSM
               !IF END # BKPEXCL.SESS.NUM% THEN BKPEXCL.READ.COMP       !TSM
               !
               !BKPEXCL.OPEN        = TRUE                              !TSM
               !FILE.OPERATION$     = "R"                               !TSM
               !CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%               !TSM
               !
               !WHILE BKPEXCL.RUN                                       !TSM
               !    READ # BKPEXCL.SESS.NUM%; LINE BKPEXCL.VALUE$       !TSM
               !    IF UCASE$(BKPEXCL.VALUE$) = UCASE$(DIR.FILE.NAME$) \!TSM
               !    THEN BEGIN                                          !TSM
               !        BKPEXCL.RUN = FALSE                             !TSM
               !    ENDIF                                               !TSM
               !WEND                                                    !TSM
               !BKPEXCL.READ.COMP:                                      !TSM
                
               !FILE.OPERATION$     = "O"                               !TSM
               !CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%               !TSM
                
                !Iterate until a match found or specifided exclude array!TSM
                !index reached                                          !TSM

                WHILE BKPEXCL.INDEX% < BKPEXCL.ARRAY.INDEX%             !TSM
                    BKPEXCL.VALUE$ = BKPEXCL.ARRAY$(BKPEXCL.INDEX%)     !TSM
                    IF UCASE$(BKPEXCL.VALUE$) = UCASE$(DIR.FILE.NAME$) \!TSM
                                                             THEN BEGIN !TSM
                        BKPEXCL.RUN = FALSE                             !TSM
                        BKPEXCL.INDEX% = BKPEXCL.ARRAY.INDEX%           !TSM
                    ENDIF                                               !TSM
                    !Increment array index                              !TSM
                    BKPEXCL.INDEX% = BKPEXCL.INDEX% +1                  !TSM
                WEND                                                    !TSM

             !----------------------------------------------!
             ! If exclude file is not matched  with current !
             ! file name, add the file to BKPLIST           !
             !----------------------------------------------!
             !  No need to add the files in to BKPLIST now as a final   !URG
             !  Processing of array in to the text file will happen     !URG
             ! IF BKPEXCL.RUN THEN BEGIN                                !URG
             !     BKPLI.FILENAME$ = BKPSCRPT.DIRECTORY$            +   !URG
             !                       DIR.FILE.NAME$                     !URG
             !     FILE.OPERATION$ = "W"                                !URG
             !     FILE.RC2%       = WRITE.BKPLI                        !URG
             !      GOSUB CHECK.FILE.RC2                                !URG 

             !     Set OFF Archive attribute
             !      CALL OSSHELL("FSET " + BKPLI.FILENAME$ + \          !URG KDC
             !               " -A=OFF >> " + DIR.OUT$   + \             !URG KDC
             !               " >>* " + DIR.OUT$  )                      !URG KDC

              !  ENDIF                                                  !URG

                !---------------------------------------!
                ! Directly add the files to the BKPLIST !
                ! as no exclude list present            !
                !---------------------------------------!
             ! ENDIF ELSE BEGIN                                         !URG
             !    BKPLI.FILENAME$ = BKPSCRPT.DIRECTORY$ + DIR.FILE.NAME$!URG
             !    FILE.OPERATION$ = "W"                                 !URG
             !    FILE.RC2%       = WRITE.BKPLI                         !URG
             !    GOSUB CHECK.FILE.RC2                                  !URG
             ! Set OFF Archive attribute                                !URG

             ! commenting out as we are not setting incremental any       
             ! more                                                     !URG

             !    CALL OSSHELL("FSET " + BKPLI.FILENAME$ + \            !URG KDC
             !                 " -A=OFF >> " + DIR.OUT$ + \             !URG KDC
             !                 " >>* " + DIR.OUT$  )                    !URG KDC

            ENDIF

        ENDIF ELSE BEGIN
            !----------------------------------!
            ! When end of file listing reached !
            !----------------------------------!
            IF LEN(DIR.SEC.VALUE$) = XBACK.ZERO% THEN BEGIN
                FILE.PRESENT = FALSE
            ENDIF
        ENDIF
    WEND

RETURN

ERROR.DIR.READ:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(26)                                !KDC
    FILE.OPERATION$ = "R"               ! File read
    GOSUB FILE.ERROR.EXIT

BKPEXCL.OPEN.COMP:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(27)                                !KDC
    FILE.OPERATION$ = "O"               ! File open
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*   BACKUP.PROCESS: This Subroutine does the backup archiving process
\*                   using ADXZUDIR utility. If any files are missed to
\*                   archive, then those files will be manually copied
\*                   to backup directory and those details will be
\*                   added to BKPFAIL.
\*
\***********************************************************************
BACKUP.PROCESS:

    STATUS.MSG$ = BKPSCRPT.DIRECTORY$ + STATUS.TEXT.MSG$(49)            !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

    !--------------------------------------------------!
    ! Closing the session. No deallocation, as the     !
    ! session will be reused. De-allocation will       !
    ! happen once the complete backup process finishes !
    !--------------------------------------------------!
    CLOSE BKPEXCL.SESS.NUM%

    !---------------------------------------!
    ! Setting as FALSE to create a new file !
    ! for next directory backup             !
    !---------------------------------------!
    BKPEXCL.OPEN = FALSE

    ! Using BKPEXCL and archiving the files
    CALL OSSHELL("ADXZUDIR -c -XL:" + BKPEXCL.FILE.NAME$ + " "   + \
                 BKPSCRPT.OUT.FILE.NAME$ + ".ZIP" + " "          + \
                 BKPSCRPT.DIRECTORY$ + " > " + XBKTEMP.LOG$ )

    ! Renaming the zip file after archiving
    FILE.RC2% = RENAME ((BKPSCRPT.OUT.FILE.NAME$ + "." + EXT.MDD$),    \
                        (BKPSCRPT.OUT.FILE.NAME$ + ".ZIP"))

    ! If rename fails
    IF NOT FILE.RC2% THEN BEGIN
        STATUS.MSG$ = BKPSCRPT.OUT.FILE.NAME$ + STATUS.TEXT.ERROR$(28)  !OJK
        GOSUB DISPLAY.STATUS.MSG
        GOSUB LOG.STATUS.MSG
    ENDIF

    !---------------------------------------!
    ! Closing the session. No deallocation, !
    ! as the session will be reused         !
    !---------------------------------------!
    CLOSE TEMP.SESS.NUM.2%
    ! Setting the temporary file close
    TEMP.2.OPEN = FALSE                                                 !FJK

    IF END # TEMP.SESS.NUM.2% THEN XBKLOG.READ.COMP
    OPEN XBKTEMP.LOG$ AS TEMP.SESS.NUM.2%

    ! Setting the temporary file open
    TEMP.2.OPEN = TRUE                                                  !FJK

    VALUE.PRESENT = TRUE

    ! Identifying the directory to be copied
    SLASH.POSITION% = MATCH("\\", BKPSCRPT.OUT.FILE.NAME$, 4)

    ! Extracting the directory to be copied
    BKPSCRPT.OUT.FILE.NAME$ = \
            LEFT$(BKPSCRPT.OUT.FILE.NAME$,SLASH.POSITION%)

    FILE.OPERATION$     = "R"                                           !CJK
    CURRENT.REPORT.NUM% = TEMP.REPORT.NUM.2%                            !CJK

    !---------------------------------!
    ! Checking for any archive errors !
    !---------------------------------!
    WHILE VALUE.PRESENT
        FILE.MISSING = FALSE                                               !SMH
        READ # TEMP.SESS.NUM.2%; LINE XBKLOG.VALUE$

        ! Checking for errors
        FILE.POSITION% = MATCH("Error adding file",XBKLOG.VALUE$, 1)

        !----------------------!
        ! If copy errors found !
        !----------------------!
        IF FILE.POSITION% <> XBACK.ZERO% THEN BEGIN

            FILE.NAME.VALUE$ = MID$(XBKLOG.VALUE$,(FILE.POSITION% +    \!FJK
                                    18), (LEN(XBKLOG.VALUE$) -         \
                                    FILE.POSITION% - 17))
            FSET.ON = TRUE

            !-------------------------------------------!
            ! Checking the LOCK mode of the file before !
            ! using FSET command                        !
            !-------------------------------------------!
            BKPFAIL.FILE$ = BKPSCRPT.DIRECTORY$  + FILE.NAME.VALUE$
            
            !----------------------------------------------!                !SMH
            ! Sets variable open.bkpfailfile to true so    !                !SMH
            ! if the open fails its handled by a specific  !                !SMH
            ! error routine, then when it resumes/continue !                !SMH
            ! it sets it to false so that error routine    !                !SMH
            ! isnt used else where.                        !                !SMH
            !----------------------------------------------!                !SMH
            OPEN.BKPFAILFILE = TRUE                                         !SMH
            OPEN BKPFAIL.FILE$ AS TEMP.SESS.NUM% LOCKED                     
            
            OPEN.BKPFAILFILE = FALSE                                        !SMH
            
            !------------------------------------!                          !SMH
            ! File.Missing set to false in error !                          !SMH
            ! routine to catch missing file      !                          !SMH
            !------------------------------------!                          !SMH
            
            IF NOT FILE.MISSING = TRUE THEN BEGIN                           !SMH
            
                TEMP.OPEN = TRUE                                            !HDC
                                                   
                CLOSE TEMP.SESS.NUM%
                TEMP.OPEN = FALSE                                           !HDC
                     
                IF FSET.ON THEN BEGIN
                    ! Making sure that the failed file is not current
                    ! BKPLIST file
                    IF BKPFAIL.FILE$ <> BKPLI.FILE.NAME$ THEN BEGIN         !NJK
                        ! C directory file will be backed up in D:/XDISKIMG/!OJK
                        IF UCASE$(LEFT$(BKPSCRPT.DIRECTORY$,1)) = "C" \     !OJK
                        THEN BEGIN                                          !OJK
                            CALL ADXCOPYF(ADXSERVE.RC%,BKPFAIL.FILE$,     \ !OJK
                                          "D:\XDISKIMG\" +                \ !OJK
                                          FILE.NAME.VALUE$ + "." +        \ !OJK
                                          EXT.MDD$,0,1,0)                   !OJK
                            GOSUB WRITE.BKPFAILC                            !OJK
                        ENDIF ELSE BEGIN                                    !OJK
                        ! D directory file will be backed up in C:/XDISKIMG/!OJK
                            CALL ADXCOPYF(ADXSERVE.RC%,BKPFAIL.FILE$,     \ !OJK
                                          "C:\XDISKIMG\" +                \ !OJK
                                          FILE.NAME.VALUE$ + "." +        \ !OJK
                                          EXT.MDD$,0,1,0)                   !OJK
                            GOSUB WRITE.BKPFAILD                            !OJK
                        ENDIF
                    ENDIF                                                   !NJK
                ENDIF ELSE BEGIN
                    STATUS.MSG$ = BKPFAIL.FILE$ + STATUS.TEXT.ERROR$(29)    !KDC
                    GOSUB DISPLAY.STATUS.MSG
                    GOSUB LOG.STATUS.MSG
                ENDIF
            
                !------------------------------------!
                ! If empty directory archiving found !
                !------------------------------------!
                FILE.POSITION% = MATCH("0x80104010",XBKLOG.VALUE$,1)
        
                ! If no files present
                IF FILE.POSITION% <> XBACK.ZERO%                            OR \
                   MATCH("No matching files found", XBKLOG.VALUE$, 1)       <> \
                   XBACK.ZERO% THEN BEGIN
                    STATUS.MSG$ = BKPSCRPT.DIRECTORY$ + STATUS.TEXT.ERROR$(30)  !OJK
                    GOSUB DISPLAY.STATUS.MSG
                    GOSUB LOG.STATUS.MSG
        
                    STATUS.MSG$ = STATUS.TEXT.ERROR$(31)                        !KDC
                    GOSUB DISPLAY.STATUS.MSG
                    GOSUB LOG.STATUS.MSG
                    VALUE.PRESENT = FALSE
                ENDIF
            ENDIF                                                               !SMH
        ENDIF
    
    WEND

XBKLOG.READ.COMP:

    ! Deleting the file as no longer needed
    FILE.OPERATION$ = "D"                                               !CJK
    
    DELETE TEMP.SESS.NUM.2%
    TEMP.2.OPEN = FALSE                                                 !FJK

    FILE.OPERATION$     = "C"                                           !CJK
    CURRENT.REPORT.NUM% = BKPEXCL.REPORT.NUM%                           !CJK
    !--------------------------------------!                            !BJK
    ! create the BKPEXCL file for next run !                            !BJK
    !--------------------------------------!                            !BJK
    CREATE POSFILE BKPEXCL.FILE.NAME$ AS BKPEXCL.SESS.NUM% LOCAL        !OJK
    BKPEXCL.OPEN = TRUE                                                 !FJK

    !Reset the exclude array index                                      !TSM
    BKPEXCL.ARRAY.INDEX% = 1                                            !TSM
    !Reset the complete exclude array index                             !TSM
    BKPEXCL.COMP.INDEX%  = 1                                            !TSM 

RETURN

\***********************************************************************
\*
\*   WRITE.BKPFAILC: Writes failed file details to BKPFAILC.MDD         !OJK
\*
\***********************************************************************
WRITE.BKPFAILC:                                                         !OJK

    CURRENT.REPORT.NUM% = BKPFAILC.REPORT.NUM%                          !OJK
    GOSUB DETERMINE.DIST.TYPE

    BKPFAIL.FILE$ = BKPFAIL.FILE$ + COMMA.VALUE$ + DIST.TYPE$ + CRLF$
    BKPFAIL.FORM$ = "C" + STR$(LEN(BKPFAIL.FILE$)) ! form string

    IF END # BKPFAILC.SESS.NUM% THEN WRITE.BKPFAILC.ERROR               !OJK
    WRITE FORM BKPFAIL.FORM$; # BKPFAILC.SESS.NUM%; BKPFAIL.FILE$       !OJK

RETURN

WRITE.BKPFAILC.ERROR:                                                   !OJK
    STATUS.MSG$ = STATUS.TEXT.ERROR$(32)
    FILE.OPERATION$ = "W"       ! File write
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************!OJK
\*                                                                      !OJK
\*   WRITE.BKPFAILD: Writes failed file details to BKPFAILD.MDD         !OJK
\*                                                                      !OJK
\***********************************************************************!OJK
WRITE.BKPFAILD:                                                         !OJK
                                                                        !OJK
    CURRENT.REPORT.NUM% = BKPFAILD.REPORT.NUM%                          !OJK
    GOSUB DETERMINE.DIST.TYPE                                           !OJK
                                                                        !OJK
    BKPFAIL.FILE$ = BKPFAIL.FILE$ + COMMA.VALUE$ + DIST.TYPE$ + CRLF$   !OJK
    BKPFAIL.FORM$ = "C" + STR$(LEN(BKPFAIL.FILE$)) ! form string        !OJK
                                                                        !OJK
    IF END # BKPFAILD.SESS.NUM% THEN WRITE.BKPFAILD.ERROR               !OJK
    WRITE FORM BKPFAIL.FORM$; # BKPFAILD.SESS.NUM%; BKPFAIL.FILE$       !OJK
                                                                        !OJK
RETURN                                                                  !OJK
                                                                        !OJK
WRITE.BKPFAILD.ERROR:                                                   !OJK
    STATUS.MSG$ = STATUS.TEXT.ERROR$(32)                                !OJK
    FILE.OPERATION$ = "W"       ! File write                            !OJK
    GOSUB FILE.ERROR.EXIT                                               !OJK
                                                                        !OJK
RETURN                                                                  !OJK

\***********************************************************************
\*
\*   DETERMINE.DIST.TYPE: This Subroutine checks the distribution type
\*                        of the BKPFAIL file and stores it.
\*
\***********************************************************************
DETERMINE.DIST.TYPE:

    CALL OSSHELL("dir -d " + BKPFAIL.FILE$ + " > " + DIR.SEC.OUT$ )     !OJK

    FILE.OPERATION$     = "O"                                           !CJK
    CURRENT.REPORT.NUM% = TEMP.REPORT.NUM%                              !CJK

    IF END # TEMP.SESS.NUM% THEN DIST.ERROR
    OPEN DIR.SEC.OUT$ AS TEMP.SESS.NUM%

    ! Setting the temporary file open
    TEMP.OPEN       = TRUE                                              !FJK
    FILE.OPERATION$ = "R"                                               !CJK

    ! Ignoring the first 4 lines
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

    READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

    ! Initiating to NULL
    DIST.TYPE$ = XBACK.NULL$

    ! Storing the Distribution type
    DIST.TYPE$ = MID$(DIR.VALUE$,26,1)                                  !NJK

    DELETE TEMP.SESS.NUM%                                               !FJK

    ! Setting the temporary file close
    TEMP.OPEN = FALSE                                                   !FJK

RETURN

DIST.ERROR:

    DIST.TYPE$ = "1"        ! Local by default

RETURN

\***********************************************************************
\*
\*   CREATE.UPDATED.BKPLIST: This Subroutine creates the updated
\*                           BKPLIST after deleting the existing
\*                           BKPLIST file.
\*
\***********************************************************************

!Commenting out the subroutine which creates updated backup list file    !URG
!CREATE.UPDATED.BKPLIST:                                                 !URG

!    ! Closing the BKPLIST session                                       !URG
!    CLOSE BKPLI.SESS.NUM%                                               !URG NJK
!    BKPLI.OPEN  = FALSE                                                 !URG NJK

!    ! if the day is the Full backup day                                 !URG KDC
!   ! IF F13.DAY$ <> FULL.DAY$ THEN BEGIN                                !URG KDC

!        STATUS.MSG$ = STATUS.TEXT.MSG$(50)                              !URG KDC
!        GOSUB DISPLAY.STATUS.MSG                                        !URG
!        GOSUB LOG.STATUS.MSG                                            !URG

!        FILE.OPERATION$     = "O"                                       !URG CJK
!        CURRENT.REPORT.NUM% = BKPLI.REPORT.NUM%                         !URG CJK

!        ! Deleting the existing BKPLIST file
!        IF END # BKPLI.SESS.NUM% THEN BKPLIST.NOT.PRESENT               !URG
!        OPEN BKPLI.FILE.NAME$ AS BKPLI.SESS.NUM% LOCKED                 !URG LDC
!        BKPLI.OPEN = TRUE                                               !URG GJK

!        FILE.OPERATION$ = "D"                                           !URG CJK
!        DELETE BKPLI.SESS.NUM%                                          !URG

!BKPLIST.NOT.PRESENT:                                                    !URG
!        ! At this point either the file is deleted or not present
!        BKPLI.OPEN = FALSE                                              !URG GJK

!        FILE.OPERATION$ = "C"                                           !URG CJK

!        ! Creating the new BKPLIST file
!        CREATE POSFILE BKPLI.FILE.NAME$ AS BKPLI.SESS.NUM%              \URG
!                                        LOCKED \                        !URG LDC
!                                        MIRRORED PERUPDATE              !URG
!        BKPLI.OPEN = TRUE                                               !URG FJK

!        IF END # BKPLI.SESS.NUM% THEN WRITE.BKPLIST.ERROR               !URG

!        !----------------------------------------------!
!        ! Extracting the file details from first array !
!        !----------------------------------------------!
!        FOR INDEX% = 1 TO ARRAY.INDEX%                                 !URG

!            BKPLIST.ARRAY$(INDEX%) = BKPLIST.ARRAY$(INDEX%) + CRLF$    !URG

!            ! Form string                                              !URG CJK
!            BKPLIST.FORM$ = "C" + STR$(LEN(BKPLIST.ARRAY$(INDEX%)))    !URG CJK

!            WRITE FORM BKPLIST.FORM$; # BKPLI.SESS.NUM%;               \URG
!                                    BKPLIST.ARRAY$(INDEX%)             !URG

!        NEXT INDEX%                                                    !URG

!        !----------------------------------!
!        ! Extracting the file details from !
!        ! second array if ON               !
!        !----------------------------------!
!        IF SECOND.ARRAY.ON THEN BEGIN                                  !URG
!            FOR SECOND.INDEX% = 1 TO ARRAY.SECOND.INDEX%               !URG
!
!                BKPLIST.SECOND.ARRAY$(SECOND.INDEX%) =                 \URG
!                        BKPLIST.SECOND.ARRAY$(SECOND.INDEX%)  + CRLF$  !URG
!
!                BKPLIST.FORM$ = "C" + \ ! Form                         !URG
!                                STR$(LEN(BKPLIST.SECOND.ARRAY$ \       !URG
!                                (SECOND.INDEX%)))                      !URG

!                WRITE FORM BKPLIST.FORM$; # BKPLI.SESS.NUM%;           \URG
!                                    BKPLIST.SECOND.ARRAY$(SECOND.INDEX%)
!            NEXT SECOND.INDEX%                                         !URG
!        ENDIF                                                          !URG
!        STATUS.MSG$ = STATUS.TEXT.MSG$(51)                             !URG KDC
!        GOSUB DISPLAY.STATUS.MSG                                       !URG
!        GOSUB LOG.STATUS.MSG                                           !URG
!
!   ! ENDIF                                                             !URG

!    !-------------------------------------------------------------!
!    ! Closing the BKPLIST session as processing has been complete !
!    !-------------------------------------------------------------!
!
!    CLOSE BKPLI.SESS.NUM%                                              !URG
!    CALL SESS.NUM.UTILITY ("C",BKPLI.SESS.NUM%,XBACK.NULL$)            !URG
!    BKPLI.OPEN = FALSE                                                 !URG

!RETURN                                                                 !URG

!WRITE.BKPLIST.ERROR:
!    STATUS.MSG$     = STATUS.TEXT.MSG$(52)                             !URG KDC
!    FILE.OPERATION$ = "W"               ! File write                   !URG
!    GOSUB FILE.ERROR.EXIT                                              !URG
!
!RETURN                                                                 !URG

\***********************************************************************
\*
\*   BACKUP.COMPLETION: This Subroutine uses IPCONFIG command to
\*                      determine the CF controller IP. And then FTP
\*                      all the current day archived files to CF
\*                      including BKPFAIL file.
\*
\***********************************************************************
BACKUP.COMPLETION:

    ! If controller is CE and CF is on LAN
    IF MASTER$ = CE.CNTR$ AND ALT.MASTER.ON THEN BEGIN

        STATUS.MSG$ = STATUS.TEXT.MSG$(53)                              !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB LOG.STATUS.MSG

        CALL OSSHELL("IPCONFIG > " + DIR.OUT$)

        FILE.OPERATION$     = "O"                                       !CJK
        CURRENT.REPORT.NUM% = TEMP.REPORT.NUM%                          !CJK

        IF END # TEMP.SESS.NUM% THEN FTPOUT.ERR
        OPEN DIR.OUT$ AS TEMP.SESS.NUM%


        ! Setting the temporary file open
        TEMP.OPEN       = TRUE                                          !FJK
        FILE.OPERATION$ = "R"                                           !CJK

        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

        ! IP configuration details
        READ # TEMP.SESS.NUM%; LINE DIR.VALUE$

        !----------------------------------------------------------!
        ! Deleting the session as temporary file is no more needed !
        !----------------------------------------------------------!
        FILE.OPERATION$ = "D"                                           !CJK
        DELETE TEMP.SESS.NUM%
        ! Setting the temporary file close
        TEMP.OPEN = FALSE                                               !FJK

        ! IP address of CE
        DIR.VALUE$ = MID$(DIR.VALUE$,5,20)
        CALL TRIM(DIR.VALUE$)
        VALUE.PRESENT = TRUE

        INDEX% = XBACK.ZERO%

        !------------------------------------!
        ! Extracting the IP address value    !
        !------------------------------------!
        IP.ADDRESS$ = XBACK.NULL$

        ! Extracting the IP address without last two digit
        WHILE VALUE.PRESENT
            INDEX% = LEN(DIR.VALUE$)
            IF MID$(DIR.VALUE$,INDEX%,1) <> "." THEN BEGIN  ! Identify
                DIR.VALUE$ = LEFT$(DIR.VALUE$,(INDEX% - 1)) ! the DOT
            ENDIF ELSE BEGIN
                VALUE.PRESENT = FALSE
            ENDIF
        WEND

        ! Setting the CF IP address
        IP.ADDRESS$ = DIR.VALUE$ + "28"           ! Secondary controller

        FILE.OPERATION$ = "C"                                           !CJK

        CREATE FTP.FILE.NAME$ AS TEMP.SESS.NUM%
        ! Setting the temporary file open
        TEMP.OPEN   = TRUE                                              !OJK

        FTP.USER$   = "xbackup"                                         !OJK
        FTP.SUCCESS = FALSE                                             !OJK

        !---------------------------------!
        ! Sequence of FTP commands stored !
        ! in FTP file for file transfer   !
        !---------------------------------!
        XBKPFTP.LINE$ = "open " + IP.ADDRESS$       ! Open command
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = XBACK.NULL$
        GOSUB WRITE.XBKPFTP
        FTP.PASSWORD$ = FUNC.GET.FTP.PASSWORD.FOR$("xbackup")           !KDC
        XBKPFTP.LINE$ = "user " + FTP.USER$ + " " + FTP.PASSWORD$       !OJK
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "bin"                       ! Binary command
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "prompt"                    ! Prompt off command
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "lcd C:\XDISKIMG"           ! Local directory
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "cd C:\XDISKIMG"            ! FTP directory
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "mput *." + EXT.MDD$        ! PUT files
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "lcd D:\XDISKIMG"           ! Local directory
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "cd D:\XDISKIMG"            ! FTP directory
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "mput *." + EXT.MDD$        ! PUT files
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "close"                     ! Complete
        GOSUB WRITE.XBKPFTP
        XBKPFTP.LINE$ = "quit"                      ! Quit
        GOSUB WRITE.XBKPFTP

        !-------------------------------------------------------!
        ! Closing the temporary session as processing completed !
        !-------------------------------------------------------!
        CLOSE TEMP.SESS.NUM%
        ! Setting the temporary file close
        TEMP.OPEN = FALSE                                               !FJK

        ! FTP the files
        CALL OSSHELL("TYPE " + FTP.FILE.NAME$ + " | FTP >> "         + \
                     FTPOUT.FILE.NAME$  + " >>* " + FTPOUT.FILE.NAME$)

        FILE.OPERATION$     = "O"                                       !CJK
        CURRENT.REPORT.NUM% = TEMP.REPORT.NUM%                          !CJK

        IF END # TEMP.SESS.NUM% THEN FTPOUT.ERR
        OPEN FTPOUT.FILE.NAME$ AS TEMP.SESS.NUM%

        ! Setting the temporary file open
        TEMP.OPEN       = TRUE                                          !FJK
        FILE.OPERATION$ = "R"                                           !CJK
        VALUE.PRESENT   = TRUE                                          !KDC

        IF END # TEMP.SESS.NUM% THEN FTPOUT.READ.EOF                    !KDC
        WHILE VALUE.PRESENT
            READ # TEMP.SESS.NUM%; LINE DIR.VALUE$
            IF MATCH("226 Transfer complete", \                         !KDC
               DIR.VALUE$,1) <> XBACK.ZERO% THEN BEGIN                  !KDC
                FTP.SUCCESS   = TRUE                                    !KDC
                VALUE.PRESENT = FALSE                                   !KDC
            ENDIF
        WEND
FTPOUT.READ.EOF:                                                        !KDC
        !-------------------------------------------------------!
        ! Closing the temporary session as processing completed !
        !-------------------------------------------------------!
        FILE.OPERATION$ = "D"                                           !CJK
        DELETE TEMP.SESS.NUM%
        ! Setting the temporary file close
        TEMP.OPEN = FALSE                                               !FJK

        IF FTP.SUCCESS THEN BEGIN                                       !KDC
            STATUS.MSG$ = STATUS.TEXT.MSG$(54)                          !KDC
        ENDIF ELSE BEGIN                                                !KDC
            STATUS.MSG$ = STATUS.TEXT.ERROR$(33)                        !KDC
            XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$                 !KDC
        ENDIF                                                           !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB LOG.STATUS.MSG
    ENDIF

    !-------------------------------------------------------------!
    ! Deallocating the Temporary session only at the end of main  !
    ! process as it has been used multiple times by the program   !
    !-------------------------------------------------------------!

    CLOSE TEMP.SESS.NUM%
    ! Setting the temporary file close                                  !HDC
    TEMP.OPEN = FALSE                                                   !HDC
    CALL SESS.NUM.UTILITY ("C",TEMP.SESS.NUM%,XBACK.NULL$)  ! Deallocate


    CLOSE TEMP.SESS.NUM.2%
    TEMP.2.OPEN = FALSE                                                 !LDC
    CALL SESS.NUM.UTILITY ("C",TEMP.SESS.NUM.2%,XBACK.NULL$)! Deallocate

    ! Deallocate session. Open and Close session will be handled by     !OJK
    ! the file function itself.                                         !OJK
    CALL SESS.NUM.UTILITY ("C",HSIUF.SESS.NUM%,XBACK.NULL$)             !OJK

RETURN

FTPOUT.ERR:
    STATUS.MSG$ = STATUS.TEXT.ERROR$(34)                                !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

RETURN


\***********************************************************************
\*
\*   FINAL.UPDATE.XBKOK: This Subroutine does the final update on the
\*                       XBKOK and closes the session
\*
\***********************************************************************
FINAL.UPDATE.XBKOK:

    GOSUB UPDATE.XBKOK

    !--------------------------------------------!
    ! As final XBKOK update is done, closing and !
    ! deallocating the session                   !
    !--------------------------------------------!
    CLOSE XBKOK.SESS.NUM%
    CALL SESS.NUM.UTILITY ("C",XBKOK.SESS.NUM%,XBACK.NULL$) ! Deallocate

    XBKOK.OPEN = FALSE

RETURN

\***********************************************************************
\*
\*   WRITE.XBKPFTP: Writes FTP command details to XBKPFTP
\*
\***********************************************************************
WRITE.XBKPFTP:

    CURRENT.REPORT.NUM% = TEMP.REPORT.NUM%

    XBKPFTP.LINE$ = XBKPFTP.LINE$ + CRLF$
    XBKPFTP.FORM$ = "C" + STR$(LEN(XBKPFTP.LINE$)) ! form string

    IF END # TEMP.SESS.NUM% THEN WRITE.XBKPFTP.ERROR
    WRITE FORM XBKPFTP.FORM$; # TEMP.SESS.NUM%; XBKPFTP.LINE$

RETURN

WRITE.XBKPFTP.ERROR:
    STATUS.MSG$     = STATUS.TEXT.ERROR$(35)                            !KDC
    FILE.OPERATION$ = "W"       ! File write
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*   OLD.ARCHIVE.PURGE: This Subroutine purges the old archive file and
\*                      backup the XBACKUP log
\*
\***********************************************************************
OLD.ARCHIVE.PURGE:

    F02.DATE$           = CURR.DATE$                                    !KDC
    RETURN.VALUE.CHECK% = UPDATE.DATE(VAL("-" + BKPSCRPT.DAYS.TO.KEEP$))!KDC

    ! Checking the return value
    GOSUB CHECK.UPDATE.DATE.RC                                          !FJK

    !-----------------------------------!
    ! Defining the Extension name for   !
    ! older archive files               !
    !-----------------------------------!
    EXTENSION$ = RIGHT$(F02.DATE$,4)       ! Extracting Month value     !KDC
    GOSUB GET.FILE.EXTENSION                                            !KDC
    EXT.MDD$ = EXTENSION$                                               !KDC

    IF MASTER$ = CE.CNTR$ THEN BEGIN
        ! Deleting the old archive files
        CALL OSSHELL("DEL " + CE.C.XDISKIMG$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CE.D.XDISKIMG$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CE.C.XDISKALT$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CE.D.XDISKALT$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

    ENDIF

    IF MASTER$ = CF.CNTR$ OR ALT.MASTER.ON THEN BEGIN
        ! Deleting the old archive files
        CALL OSSHELL("DEL " + CF.C.XDISKIMG$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CF.D.XDISKIMG$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CF.C.XDISKALT$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )

        CALL OSSHELL("DEL " + CF.D.XDISKALT$ + "*." + EXT.MDD$ + \      !JJK
                     " >> " + DIR.OUT$ +  " >>* " + DIR.OUT$ )
    ENDIF

    STATUS.MSG$ = STATUS.TEXT.MSG$(55)                                  !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG

    CALL ADXCOPYF(ADXSERVE.RC%,XBACK.LOG.LIVE.PATH$,                   \
                         XBACK.LOG.FILE.NAME$,0,1,0)

    ! Moving XBACKUP.LOG from TEMP to D:/adx_udt1
    IF ADXSERVE.RC% = XBACK.ZERO% THEN BEGIN
        STATUS.MSG$ = STATUS.TEXT.MSG$(56)                              !KDC
        GOSUB DISPLAY.STATUS.MSG

        !---------------------------!
        ! Deleting the existing LOG !
        ! file in TEMP directory    !
        !---------------------------!
        FILE.OPERATION$     = "D"                                       !CJK
        CURRENT.REPORT.NUM% = XBACK.LOG.REPORT.NUM%                     !CJK
        DELETE XBACK.LOG.SESS.NUM%
        XBACK.LOG.OPEN = FALSE                                          !LDC
        !--------------------------!
        ! Deallocating the session !
        !--------------------------!
        CALL SESS.NUM.UTILITY ("C",XBACK.LOG.SESS.NUM%,XBACK.NULL$)

    ENDIF

RETURN

\***********************************************************************!OJK
\*                                                                      !OJK
\*   BACKUP.CONFIG.FILES: This Subroutine backs up configuration files  !OJK
\*                        in C drive                                    !OJK
\*                                                                      !OJK
\***********************************************************************!OJK
! As there is nothing to backup commenting out the subroutine           !URG
!BACKUP.CONFIG.FILES:                                                   !URG OJK
                                                                        
!    IF F13.DAY$ = "SUN" THEN BEGIN                                     !URG
!        STATUS.MSG$ = "Backing up Configuration files"                 !URG OJK
!        GOSUB LOG.STATUS.MSG                                           !URG OJK
!        GOSUB DISPLAY.STATUS.MSG                                       !URG OJK
                                                                         
    ! Taking a copy of current BKPLIST file in C:/ADX_UDT1/             !URG OJK
!        CALL ADXCOPYF(ADXSERVE.RC%,BKPLI.FILE.NAME$,            \      !URG OJK
!                  BKPLIST.FILE.NAME.C$ + FULL.EXT.MDD$,0,0,0)          !URG OJK
                                                                        
!        IF ADXSERVE.RC% <> XBACK.ZERO% THEN BEGIN                      !URG OJK
!            STATUS.MSG$ = "Error in BKPLIST file backing up"           !URG OJK
!            XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$                !URG OJK
!            GOSUB PROGRAM.EXIT                                         !URG OJK
!        ENDIF                                                          !URG OJK
!    ENDIF                                                              !URG
                                                                       
!RETURN                                                                 !URG OJK

\***********************************************************************!KDC
\*                                                                      !KDC
\*   GET.FILE.EXTENSION: Calculate the extension needed based on month  !KDC
\*                       and day using month A/B/C for 10/11/12         !KDC
\*                                                                      !KDC
\***********************************************************************!KDC
GET.FILE.EXTENSION:                                                     !KDC
                                                                        !KDC
    ! Storing the Month and Date in MDD format in new logic             !KDC
    IF LEFT$(EXTENSION$,2) = "12" THEN BEGIN    ! If December (12)      !KDC
        EXTENSION$ = "C" + RIGHT$(EXTENSION$, 2)                        !KDC
    ENDIF ELSE \                                                        !KDC
    IF LEFT$(EXTENSION$,2) = "11" THEN BEGIN    ! If November (11)      !KDC
        EXTENSION$ = "B" + RIGHT$(EXTENSION$, 2)                        !KDC
    ENDIF ELSE IF \                                                     !KDC
    LEFT$(EXTENSION$,2) = "10" THEN BEGIN       ! If October  (10)      !KDC
        EXTENSION$ = "A" + RIGHT$(EXTENSION$, 2)                        !KDC
    ENDIF ELSE BEGIN                            ! Rest of the Month     !KDC
        EXTENSION$ = RIGHT$(EXTENSION$, 3)                              !KDC
    ENDIF                                                               !KDC
                                                                        !KDC
RETURN                                                                  !KDC
                !   MAIN.PROCESSING SPECIFIC ROUTINES ENDS   !
                !............................................!

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

    ! Changed the variable FILE.OPERATION$ to FUNC.FLAG$                !BJK
    FUNC.FLAG$     = "C"         ! File Close                           !BJK
    PASSED.STRING$ = XBACK.NULL$ ! Setting Null

    IF XBACK.OPEN THEN BEGIN
        CLOSE XBACK.PIPE.SESS.NUM%
        XBACK.OPEN = FALSE                                              !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,XBACK.PIPE.SESS.NUM%,PASSED.STRING$)

    ENDIF

    IF BKPLI.OPEN THEN BEGIN
        CLOSE BKPLI.SESS.NUM%
        BKPLI.OPEN = FALSE                                              !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,BKPLI.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF BKPEXCL.OPEN THEN BEGIN
        CLOSE BKPEXCL.SESS.NUM%
        BKPEXCL.OPEN = FALSE                                            !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,BKPEXCL.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF BKPFAILC.OPEN THEN BEGIN                                         !OJK
        CLOSE BKPFAILC.SESS.NUM%                                        !OJK
        BKPFAILC.OPEN = FALSE                                           !OJK
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,BKPFAILC.SESS.NUM%,PASSED.STRING$)              !OJK
    ENDIF

    IF BKPFAILD.OPEN THEN BEGIN                                         !OJK
        CLOSE BKPFAILD.SESS.NUM%                                        !OJK
        BKPFAILC.OPEN = FALSE                                           !OJK
        CALL SESS.NUM.UTILITY                                          \!OJK
            (FUNC.FLAG$,BKPFAILD.SESS.NUM%,PASSED.STRING$)              !OJK
    ENDIF                                                               !OJK

    IF BKPSCRPT.OPEN THEN BEGIN
        CLOSE BKPSCRPT.SESS.NUM%
        BKPSCRPT.OPEN = FALSE                                           !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,BKPSCRPT.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF TEMP.OPEN THEN BEGIN
        CLOSE TEMP.SESS.NUM%
        TEMP.OPEN = FALSE                                               !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,TEMP.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF TEMP.2.OPEN THEN BEGIN                                           !FJK
        CLOSE TEMP.SESS.NUM.2%                                          !FJK
        TEMP.2.OPEN = FALSE                                             !LDC
        CALL SESS.NUM.UTILITY \                                         !FJK
            (FUNC.FLAG$,TEMP.SESS.NUM.2%,PASSED.STRING$)                !FJK
    ENDIF                                                               !FJK

    IF XBKOK.OPEN THEN BEGIN
        CLOSE XBKOK.SESS.NUM%
        XBKOK.OPEN = FALSE                                              !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,XBKOK.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF XBACK.LOG.OPEN THEN BEGIN
        CLOSE XBACK.LOG.SESS.NUM%
        XBACK.LOG.OPEN = FALSE                                          !LDC
        CALL SESS.NUM.UTILITY                                          \
            (FUNC.FLAG$,XBACK.LOG.SESS.NUM%,PASSED.STRING$)
    ENDIF

    IF HSIUF.OPEN THEN BEGIN                                            !KDC
        CLOSE HSIUF.SESS.NUM%                                           !KDC
        HSIUF.OPEN = FALSE                                              !LDC
        CALL SESS.NUM.UTILITY                                          \!KDC
            (FUNC.FLAG$,HSIUF.SESS.NUM%,PASSED.STRING$)                 !KDC
    ENDIF                                                               !KDC

    IF SLPCF.OPEN THEN BEGIN                                            !KDC
        CLOSE SLPCF.SESS.NUM%                                           !KDC
        SLPCF.OPEN = FALSE                                              !LDC
        CALL SESS.NUM.UTILITY                                          \!KDC
            (FUNC.FLAG$,SLPCF.SESS.NUM%,PASSED.STRING$)                 !KDC
    ENDIF                                                               !KDC

RETURN

                !   TERMINATION SPECIFIC ROUTINES ENDS   !
                !........................................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                       GENERIC ROUTINES                             *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************
\*
\*   LOG.STATUS.MSG: Writes status message to log file
\*
\***********************************************************************
LOG.STATUS.MSG:

    CURRENT.REPORT.NUM% = XBACK.LOG.REPORT.NUM%

    IF NOT XBACK.LOG.OPEN THEN BEGIN ! If not open                      !CJK
        RETURN
    ENDIF

    DATE.VALUE$ = DATE$                                                 !FJK

    PREFIX.HHMMSS.PROG$ = RIGHT$(DATE.VALUE$,2) + "/" + \ DD            !KDC
                          MID$(DATE.VALUE$,3,2) + "/" + \ MM            !KDC
                          LEFT$(DATE.VALUE$,2)  + " " + \ YY            !KDC
                          MID$(TIME$,1,2)       + ":" + \               !KDC
                          MID$(TIME$,3,2)       + ":" + \               !KDC
                          MID$(TIME$,5,2)       + " " + \ Time-stamp    !KDC
                          STATUS.MSG$                   ! Text          !KDC

    XBACK.LOG.REC$  = PREFIX.HHMMSS.PROG$ + CRLF$                       !FJK
    XBACK.LOG.FORM$ = "C" + STR$(LEN(XBACK.LOG.REC$))   ! Form string

    IF END # XBACK.LOG.SESS.NUM% THEN WRITE.XBACK.LOG.ERROR
    WRITE FORM XBACK.LOG.FORM$; # XBACK.LOG.SESS.NUM%; XBACK.LOG.REC$

RETURN

WRITE.XBACK.LOG.ERROR:
    STATUS.MSG$     = STATUS.TEXT.ERROR$(36)                            !KDC
    FILE.OPERATION$ = "W"           ! File write
    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*    DISPLAY.STATUS.MSG: If program running in Background, it display
\*                        it as Background message, else it prints in
\*                        the console.
\*
\***********************************************************************
DISPLAY.STATUS.MSG:

    HHMM.STATUS.MSG$ = MID$(TIME$,1,2) + ":" + MID$(TIME$,3,2) + \      !KDC
                       " " + STATUS.MSG$               ! Text           !FJK

    IF BACKGROUND.RUN THEN BEGIN    ! Running in background

        ADX.INTEGER%  = XBACK.ZERO% ! Setting the parameter
        ADX.FUNCTION% = 26          ! Parameter for Background message

        ! Display STATUS.MSG$ to background screen
        !                Return code,   Function,
        CALL ADXSERVE (ADXSERVE.RC%, ADX.FUNCTION%,                    \
                       ADX.INTEGER%, HHMM.STATUS.MSG$)
        !                   Parm1,       Parm2

        GOSUB CHECK.ADXSERVE.RC ! Stops program if non zero

    ENDIF ELSE BEGIN

        ! Display message to command mode screen status box
        PRINT LEFT$(HHMM.STATUS.MSG$ + STRING$(46, " "), 46) ! message
    ENDIF
RETURN
                        !   GENERIC ROUTINES ENDS   !
                        !...........................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*                   PROGRAM-INDEPENDENT ROUTINES                     *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************!FJK
\*
\*    CHECK.UPDATE.DATE.RC: If RETURN.VALUE.CHECK% is not equal to zero !KDC
\*                          logs the error and end the program.         !KDC
\*
\***********************************************************************
CHECK.UPDATE.DATE.RC:

    ! Checking the return value
    IF RETURN.VALUE.CHECK% <> XBACK.ZERO% THEN BEGIN                    !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(37)                            !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB STOP.PROGRAM
    ENDIF

RETURN

\***********************************************************************!FJK
\*
\*    CHECK.PSDATE.RC: If FUNC.RC2% is not equal to zero ... logs
\*                     the error and end the program.
\*
\***********************************************************************
CHECK.PSDATE.RC:

    ! Checking the return value
    IF RETURN.VALUE.CHECK% <> XBACK.ZERO% THEN BEGIN                    !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(38)                            !KDC
        GOSUB DISPLAY.STATUS.MSG
        GOSUB STOP.PROGRAM
    ENDIF

RETURN

\***********************************************************************
\*
\*    CHECK.FILE.RC2: If FILE.RC2% is zero ... By-passes rest
\*                    of procedure. Diverts program control to
\*                    FILE.ERROR.EXIT (exiting program).
\*
\***********************************************************************
CHECK.FILE.RC2:

    IF FILE.RC2% = 0 THEN RETURN ! No error

    GOSUB FILE.ERROR.EXIT

RETURN

\***********************************************************************
\*
\*    RETURN.VALUE.CHECK: If RETURN.VALUE.CHECK% is zero,               !KDC
\*                        By-passes rest of procedure and Stops         !KDC
\*                        program.                                      !KDC
\*
\***********************************************************************
RETURN.VALUE.CHECK:                                                     !KDC

    IF RETURN.VALUE.CHECK% = XBACK.ZERO% THEN RETURN  ! No error        !KDC

    STATUS.MSG$  = STATUS.TEXT.ERROR$(39) + PASSED.STRING$              !KDC

    GOSUB PROGRAM.EXIT

RETURN

\***********************************************************************
\*
\*    CHECK.ADXSERVE.RC: CALLs PSBF20 SESS.NUM.UTILITY to CLOSE entry
\*                       on session number table and deallocate file
\*                       session number for all files used by program.
\*
\***********************************************************************
CHECK.ADXSERVE.RC:

    IF ADXSERVE.RC% <> XBACK.ZERO% THEN BEGIN  ! Error check
        GOSUB LOG.AN.EVENT.23
        GOSUB PROGRAM.EXIT
    ENDIF

RETURN

\**********************************************************************
\*
\*    LOG.AN.EVENT.23: Writes details of Event 23 to application
\*                     event log.
\*
\**********************************************************************
LOG.AN.EVENT.23:

    EVENT.NUMBER% = 23 ! ADXSERVE error

    ! Formatting the length
    VAR.STRING.1$ = RIGHT$("00000" + STR$(ADXSERVE.RC%),5)           + \
                    PACK$("0000000000")

    GOSUB CALL.F01.APPLICATION.LOG

RETURN

CALL.F01.APPLICATION.LOG:

    MESSAGE.NUMBER% = XBACK.ZERO%                                       !FJK
    VAR.STRING.2$   = XBACK.NULL$                                       !FJK

    RETURN.VALUE.CHECK% = APPLICATION.LOG (MESSAGE.NUMBER%, \           !KDC
                          VAR.STRING.1$, VAR.STRING.2$, EVENT.NUMBER% ) !KDC

    IF RETURN.VALUE.CHECK% <> XBACK.ZERO% THEN BEGIN ! If error         !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(40)                            !KDC
        GOSUB PROGRAM.EXIT
    ENDIF

RETURN
                    !   PROGRAM-INDEPENDENT ROUTINES ENDS   !
                    !.......................................!

\**********************************************************************\
\**********************************************************************\
\*                                                                    *\
\*             IF END # AND ERROR.DETECTED ROUTINES                   *\
\*                                                                    *\
\**********************************************************************\
\**********************************************************************\

\***********************************************************************!DJK
\*
\*    INVALID.PARAM.EXIT: This subroutine will be called if invalid
\*                        parameter is passed in XBACKUP run. It clears
\*                        the screen and ends the program with a
\*                        display message.
\*
\***********************************************************************
INVALID.PARAM.EXIT:

    CLEARS
    STATUS.MSG$ = STATUS.TEXT.ERROR$(41)                                !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.ERROR$(42)                                !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.ERROR$(43)                                !KDC
    GOSUB DISPLAY.STATUS.MSG
    STATUS.MSG$ = STATUS.TEXT.ERROR$(41)                                !KDC
    GOSUB DISPLAY.STATUS.MSG
    GOSUB STOP.PROGRAM

RETURN

\***********************************************************************
\*
\*    FILE.ERROR.EXIT: Logs events for specific file errors.
\*                     Formats error message and displays on background
\*                     screen. Logs an event 106. Stops program.
\*
\***********************************************************************
FILE.ERROR.EXIT:

    GOSUB DISPLAY.STATUS.MSG
    IF CURRENT.REPORT.NUM% <> XBACK.LOG.REPORT.NUM% THEN BEGIN
        GOSUB LOG.STATUS.MSG
    ENDIF

    XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$ ! Critical errors
    GOSUB FINAL.UPDATE.XBKOK                                            !BJK

    ! Logging event 106 for File error                                  !FJK
    STATUS.MSG$ = STATUS.TEXT.ERROR$(44) + FILE.OPERATION$              !KDC
    GOSUB DISPLAY.STATUS.MSG                                            !FJK
    GOSUB LOG.STATUS.MSG                                                !FJK

    EVENT.NUMBER% = 106     ! Event 106                                 !FJK

    ! Application event log data                                        !FJK
    VAR.STRING.1$ = FILE.OPERATION$                                  + \!FJK
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,8))               + \!FJK
                    CHR$(SHIFT(CURRENT.REPORT.NUM%,XBACK.ZERO%))        !FJK

    GOSUB CALL.F01.APPLICATION.LOG                                      !FJK

    GOSUB TERMINATION
    GOSUB STOP.PROGRAM

RETURN

\***********************************************************************
\*
\*    PROGRAM.EXIT: Displays final status message and writes it to
\*                  log file. Updates XBKOK to set XBKOK.STATUS$.
\*
\***********************************************************************
PROGRAM.EXIT:

    GOSUB DISPLAY.STATUS.MSG
    GOSUB LOG.STATUS.MSG
    GOSUB FINAL.UPDATE.XBKOK                                            !BJK
    GOSUB TERMINATION
    GOSUB STOP.PROGRAM

RETURN

\***********************************************************************
\*
\*    ERROR.DETECTED: Main Error Handling Routine. Starts with the
\*                    resume error conditions following ERROR.COUNT%
\*                    check to avoid error loop. Also References
\*                    STANDARD.ERROR.DETECTED to log Event 101.
\*
\***********************************************************************
ERROR.DETECTED:
    
    IF ERR = "OE" AND OPEN.BKPFAILFILE = TRUE THEN BEGIN                !SMH
        FILE.MISSING = TRUE                                             !SMH
        RESUME                                                          !SMH
    ENDIF                                                               !SMH

    IF ERR = "CU" OR ERR = "DU" THEN RESUME     ! Close and delete
                                                ! session errors
    IF ERR = "*I" THEN BEGIN   ! Unexpected error on file
        IF FSET.ON THEN BEGIN
            FSET.ON = FALSE
            RESUME
        ENDIF
    ENDIF

    ! check if this is coming from DOES.BACKUP.DIR.EXIST to handle error!KDC
    IF BACKUP.DIR.EXIST = 2 THEN BEGIN                                  !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(1)                             !KDC
        BACKUP.DIR.EXIST = FALSE    ! Error occurred                    !KDC
        RESUME BACKUP.DIR.EXIST.EXIT                                    !KDC
    ENDIF                                                               !KDC

    ! check if this is coming from DOES.ZIP.FILE.EXISTS to handle error !KDC
    IF ZIP.FILE.EXISTS = 2 THEN BEGIN                                   !KDC
        STATUS.MSG$ = STATUS.TEXT.ERROR$(1)                             !KDC
        ZIP.FILE.EXISTS = FALSE    ! Error occurred                     !KDC
        RESUME ZIP.FILE.EXISTS.EXIT                                     !KDC
    ENDIF                                                               !KDC

    !*******************************************************************!KDC
    ! if an error found processing the list file then report and        !KDC
    ! continue                                                          !KDC
    ! this replicates the original error handling in the                !KDC
    ! FUNC.UPD.TODAY.BKPLIST.REC$ function when it converted to a       !KDC
    ! subroutine                                                        !KDC
    !*******************************************************************!KDC
    IF ERRF% <> 0 THEN BEGIN                                            !KDC
        !IF ERRF% = BKPLI.SESS.NUM% THEN BEGIN                           !KDC

        !    STATUS.MSG$ = STATUS.TEXT.ERROR$(45) + \                   !URG KDC
        !                  BKPLI.FILENAME$ + " file"                    !URG KDC
                                                                        !URG KDC
            ! resume depending on where the error was                   !URG LDC
        !    IF TODAY.BKPLIST.REC% = 1 THEN BEGIN                       !URG LDC
        !        RESUME UPD.TODAY.BKPLIST.REC.ERR                       !URG LDC
        !     ENDIF ELSE BEGIN                                          !URG OJK
                                                                        !URG LDC
        !    ENDIF                                                      !URG LDC
        !ENDIF                                                          !URG KDC
    ENDIF                                                               !KDC

    ERROR.COUNT% = ERROR.COUNT% + 1             ! Updates Error count
    !***************************************************************!   !KDC
    ! error handling below this point will add to the general error !   !KDC
    ! failure count                                                 !   !KDC
    !***************************************************************!   !KDC

    IF ERROR.COUNT% > 1 THEN BEGIN              ! Infinite Error
        GOSUB STOP.PROGRAM                      ! loop check            !BJK
    ENDIF

    !------------------------!
    ! Fatal errors to follow !
    !------------------------!

    ! Error creating run pipe, Program already running
    IF ERR = "ME" AND ERRF% = XBACK.PIPE.SESS.NUM% THEN BEGIN
        CLEARS
        STATUS.MSG$ = STATUS.TEXT.ERROR$(46) + PROGRAM$ + \     !KDC
                      STATUS.TEXT.ERROR$(47)                    !KDC
        GOSUB DISPLAY.STATUS.MSG                               ! display
        GOSUB TERMINATION
        GOSUB STOP.PROGRAM
    ENDIF

    IF ERR = "IH" THEN BEGIN                ! Illegal characters
        IF BKPSCRPT.ERROR THEN BEGIN
            STATUS.MSG$ =  STATUS.TEXT.ERROR$(48)                       !KDC
            GOSUB PROGRAM.EXIT
        ENDIF
    ENDIF

    ! File access error
    IF (ERRN AND 0000FFFFH) = 400CH OR ERR = "ND" THEN BEGIN

        FILE.OPERATION$ = "O"

        IF ERRF% = XBACK.LOG.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(49)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = BKPSCRPT.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(50)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = BKPEXCL.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(51)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = XBKOK.SESS.NUM% THEN BEGIN                           !DJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(52)                        !KDC
            GOSUB FILE.ERROR.EXIT                                       !DJK
        ENDIF                                                           !DJK
        IF ERRF% = BKPLI.SESS.NUM% THEN BEGIN                           !KDC
            STATUS.MSG$ = STATUS.TEXT.ERROR$(59)                        !KDC
            GOSUB FILE.ERROR.EXIT                                       !KDC
        ENDIF                                                           !KDC

    ENDIF

    IF FILE.OPERATION$ = "C" THEN BEGIN              ! File Create
        IF ERRF% = XBACK.LOG.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(53)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = BKPEXCL.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(54)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = BKPFAILC.SESS.NUM% THEN BEGIN                        !OJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(55)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = BKPFAILD.SESS.NUM% THEN BEGIN                        !OJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(60)                        !OJK
            GOSUB FILE.ERROR.EXIT                                       !OJK
        ENDIF                                                           !OJK
        IF ERRF% = BKPLI.SESS.NUM% THEN BEGIN
            STATUS.MSG$ = STATUS.TEXT.ERROR$(56)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
        IF ERRF% = TEMP.SESS.NUM% OR ERRF% = TEMP.SESS.NUM.2% THEN BEGIN!CJK
            STATUS.MSG$ = STATUS.TEXT.ERROR$(57)                        !KDC
            GOSUB FILE.ERROR.EXIT
        ENDIF
    ENDIF

    ! Log event 102
    CALL STANDARD.ERROR.DETECTED(ERRN, ERRF%, ERRL, ERR)                !FJK

    ! Status message to display
    STATUS.MSG$ = "Ended: " + ERR + " " + ERRNH + " ERRL "  + \
                  STR$(ERRL) + " ERRF% " + STR$(ERRF%)
    XBKOK.INTERIM.STATUS$ = STATUS.MAJOR.ERROR$ ! Failed
    GOSUB PROGRAM.EXIT ! Displays status message, updates LOG and XBKOK

END

\***********************************************************************
\***********************************************************************
\*
\*    End of program XBACKUP
\*
\***********************************************************************
\***********************************************************************


