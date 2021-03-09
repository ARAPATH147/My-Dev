echo off
CLS
ECHO 4690 Application Debugger C language compile and link batch file.
REM  4690 Application Debugger C language compile and link batch file.
ECHO Copyright (c) 2012 Toshiba Global Commerce Solutions, Inc.
REM  Copyright (c) 2012 Toshiba Global Commerce Solutions, Inc.
IF NOT EXIST %1.C GOTO NOTFOUND
ECHO .
ECHO HC %1 -debug -on Codeview -off Optimize_xjmp -off Optimize_fp -off Optimize_xjmp_space -mm big
HC %1 -debug -on Codeview -off Optimize_xjmp -off Optimize_fp -off Optimize_xjmp_space -mm big
IF ERRORLEVEL 1 GOTO BADCOMP
ECHO .
IF NOT EXIST %1.INP GOTO LINKREG
ECHO LINK86 %1[i]
LINK86 %1[i]
IF ERRORLEVEL 1 GOTO BADLINK
GOTO DOCDV
:LINKREG
ECHO LINK86 %1.286 = %1[S,MAP[A],LI,LO,DBI], HCBE.L86[S]
LINK86 %1.286 = %1[S,MAP[A],LI,LO,DBI], HCBE.L86[S]
IF ERRORLEVEL 1 GOTO BADLINK
:DOCDV
ECHO .
ECHO DEBUGCDV %1
DEBUGCDV %1
IF ERRORLEVEL 1 GOTO BADCDV
ECHO Copy the following files for 4690 debug.
ECHO %1.C (All the source files)
ECHO %1.286
ECHO %1.CDV
GOTO END
:NOTFOUND
ECHO ..........................................
ECHO . FILE %1.C cannot be found.             .
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
