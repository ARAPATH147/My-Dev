echo off
CLS
ECHO 4690 Application Debugger BASIC compile and link batch file.
REM  4690 Application Debugger BASIC compile and link batch file.
ECHO Copyright (c) 2012 Toshiba Global Commerce Solutions, Inc.
REM  Copyright (c) 2012 Toshiba Global Commerce Solutions, Inc.
IF NOT EXIST %1.BAS GOTO NOTFOUND
ECHO .
ECHO BASIC %1 [DF]
BASIC %1 [DF]
IF ERRORLEVEL 1 GOTO BADCOMP
IF NOT EXIST %1.LST GOTO LINK
ECHO DEBUGFRM %1
DEBUGFRM %1
IF ERRORLEVEL 1 GOTO BADFRM
:LINK
ECHO .
IF NOT EXIST %1.INP GOTO LINKREG
ECHO LINK86 %1[i]
LINK86 %1[i]
IF ERRORLEVEL 1 GOTO BADLINK
GOTO DOCDV
:LINKREG
ECHO LINK86 %1.286 = %1[S,MAP[A],LI,LO,DBI]
LINK86 %1.286 = %1[S,MAP[A],LI,LO,DBI]
IF ERRORLEVEL 1 GOTO BADLINK
:DOCDV
ECHO .
ECHO DEBUGCDV %1
DEBUGCDV %1
IF ERRORLEVEL 1 GOTO BADCDV
ECHO The following files are needed to execute on the 4690:
ECHO %1.LST (all the source files)
ECHO %1.CDV
ECHO %1.286
GOTO END
:NOTFOUND
ECHO ..........................................
ECHO . FILE %1.BAS cannot be found.             .
ECHO ..........................................
GOTO END
:BADFRM
ECHO ..........................................
ECHO . DEBUGFRM cannot format you listing      .
ECHO ..........................................
GOTO END
:BADCOMP
ECHO ................................................
ECHO . Unsuccessful compile. Batch file terminated. .
ECHO ................................................
GOTO END
:BADLINK
ECHO .............................................
ECHO . Unsuccessful link. Batch file terminated. .
ECHO .............................................
GOTO END
:BADCDV
ECHO ...............................................................
ECHO . Unsuccessful Debug file preparation. Batch file terminated. .
ECHO ...............................................................
:END
