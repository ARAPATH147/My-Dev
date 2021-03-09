echo off
CLS
ECHO 4680 Application Debugger COBOL compile and link batch file.
REM  4680 Application Debugger COBOL compile and link batch file.
ECHO Copyright IBM Corporation 1991.
REM  Copyright IBM Corporation 1991.
IF NOT EXIST %1.CBL GOTO NOTFOUND
ECHO .
ECHO COBOL %1 EANIM OS(FLEXOS);
COBOL %1 EANIM OS(FLEXOS);
IF ERRORLEVEL = 1 GOTO BADCOMP
ECHO .
IF NOT EXIST %1.INP GOTO LINKREG
ECHO LINK86 %1[i]
LINK86 %1[i]
IF ERRORLEVEL = 1 GOTO BADLINK
GOTO DOCDV
:LINKREG
ECHO LINK86 %1 = filename[S,M,DBI], LCOBOL.L86[S,NOSH], CBLAPI.L86[S,NOSH]
LINK86 %1 = %1[S,M,DBI], LCOBOL.L86[S,NOSH], CBLAPI.L86[S,NOSH]
IF ERRORLEVEL = 1 GOTO BADLINK
:DOCDV
ECHO .
ECHO DEBUGCDV %1
DEBUGCDV %1
REM *********** IF ERRORLEVEL = 1 GOTO BADCDV
ECHO The following files are needed to execute on the 4680:
ECHO %1.CBL (all the source files)
ECHO %1.CDV
ECHO %1.286
GOTO END
:NOTFOUND
ECHO ................................
ECHO . FILE %1.CBL cannot be found. .
ECHO ................................
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
