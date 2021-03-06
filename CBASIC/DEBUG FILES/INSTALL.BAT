ECHO OFF
REM
REM This BAT file creates the user logical names and subdirectories
REM needed for The IBM 4690 Application Debugger. It also copies the
REM files into the proper subdirectory
REM
CLS
ECHO 旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �                                                                         �
ECHO �                 IBM 4690 Application Debugger V4.16                     �
ECHO �                                                                         �
ECHO 읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �
PAUSE
CLS
ECHO 旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �                                                                         �
ECHO � Important Note:                                                         �
ECHO �                                                                         �
ECHO �     The documentation files are not placed on the system fixed disk     �
ECHO �     by this installation procedure.  The files may be placed on the     �
ECHO �     fixed disk if desired using the system command COPY (see chapter    �
ECHO �     8 of the 4690 Store System User's Guide).  If the documentation     �
ECHO �     files are to be placed on the fixed disk, they should be put in     �
ECHO �     the C:\DEBUG subdirectory. The name of the The IBM 4690             �
ECHO �     Application Debugger documentation is DEBUGUSE.PDF on diskette #2.  �
ECHO �                                                                         �
ECHO �     To print the documentation, insert diskette #2 into a system        �
ECHO �     that has Microsoft Windows installed, click START, click RUN,       �
ECHO �     type A:DEBUGUSE.PDF, and click OK.  Adobe Acrobat Reader is         �
ECHO �     required.  After it starts, click File - Print.                     �
ECHO �                                                                         �
ECHO �                                                                         �
ECHO �  Installation of the IBM 4690 Application Debugger will now take        �
ECHO �  place.                                                                 �
ECHO �                                                                         �
ECHO 읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �
PAUSE
REM  Set up logical names and subdirectories
ECHO  Setting up logical names and subdirectory
DEFINE DSOURCE=A:\
DEFINE DTARGET=C:\DEBUG\
%DSOURCE%EWTSETUP.286
ECHO .
ECHO RC=806043A1, above is OK if you are re-installing or upgrading the debugger.
REM  Copy files onto the system

REM DEBUG.286
ECHO Installing DEBUG.286
COPY %DSOURCE%DEBUG.286 %DTARGET%DEBUG.286
IF NOT EXIST %DTARGET%DEBUG.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DEBUG.286

  REM DEBUGDMP.286
  REM ECHO Installing DEBUGDMP.286
  REM COPY %DSOURCE%DEBUGDMP.286 %DTARGET%DEBUGDMP.286
  REM IF NOT EXIST %DTARGET%DEBUGDMP.286 GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DEBUGDMP.286

REM DEBUGCDV.286
ECHO Installing DEBUGCDV.286
COPY %DSOURCE%DEBUGCDV.286 %DTARGET%DEBUGCDV.286
IF NOT EXIST %DTARGET%DEBUGCDV.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DEBUGCDV.286

REM DEBUGFRM.286
ECHO Installing DEBUGFRM.286
COPY %DSOURCE%DEBUGFRM.286 %DTARGET%DEBUGFRM.286
IF NOT EXIST %DTARGET%DEBUGFRM.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DEBUGFRM.286

  REM EXT4680.286
  REM ECHO Installing EXT4680.286
  REM COPY %DSOURCE%EXT4680.286 %DTARGET%EXT4680.286
  REM IF NOT EXIST %DTARGET%EXT4680.286 GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EXT4680.286

  REM EXT4690.286
  REM ECHO Installing EXT4690.286
  REM COPY %DSOURCE%EXT4690.286 %DTARGET%EXT4690.286
  REM IF NOT EXIST %DTARGET%EXT4690.286 GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EXT4690.286

  REM EXTRACT.286
  REM ECHO Installing EXTRACT.286
  REM COPY %DSOURCE%EXTRACT.286 %DTARGET%EXTRACT.286
  REM IF NOT EXIST %DTARGET%EXTRACT.286 GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EXTRACT.286

REM EWTMSG.DAT
ECHO Installing EWTMSG.DAT
COPY %DSOURCE%EWTMSG.DAT %DTARGET%EWTMSG.DAT
IF NOT EXIST %DTARGET%EWTMSG.DAT GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EWTMSG.DAT

REM EWTCONT.286
ECHO Installing EWTCONT.286
COPY %DSOURCE%EWTCONT.286 %DTARGET%EWTCONT.286
IF NOT EXIST %DTARGET%EWTCONT.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EWTCONT.286

REM EWTTERM.286
ECHO Installing EWTTERM.286
COPY %DSOURCE%EWTTERM.286 %DTARGET%EWTTERM.286
IF NOT EXIST %DTARGET%EWTTERM.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EWTTERM.286

REM EWTTERM2.286
ECHO Installing EWTTERM2.286
COPY %DSOURCE%EWTTERM2.286 %DTARGET%EWTTERM2.286
IF NOT EXIST %DTARGET%EWTTERM2.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%EWTTERM2.286

REM HELP.286
ECHO Installing HELP.286
COPY %DSOURCE%HELP.286 C:\ADX_SPGM\HELP.286
IF NOT EXIST C:\ADX_SPGM\HELP.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 C:\ADX_SPGM\HELP.286

REM DEBUGGER.HLP
ECHO Installing DEBUGGER.HLP
COPY %DSOURCE%DEBUGGER.HLP %DTARGET%DEBUGGER.HLP
IF NOT EXIST %DTARGET%DEBUGGER.HLP GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DEBUGGER.HLP

  REM APPLDMP.HLP
  REM ECHO Installing APPLDMP.HLP
  REM COPY %DSOURCE%APPLDMP.HLP %DTARGET%APPLDMP.HLP
  REM IF NOT EXIST %DTARGET%APPLDMP.HLP GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%APPLDMP.HLP

REM HELP.XTP
ECHO Removing HELP.XTP
IF EXIST %DTARGET%HELP.XTP ERASE %DTARGET%HELP.XTP
IF EXIST %DTARGET%HELP.XTP GOTO ERROR

REM MW.BAT
ECHO Installing MW.BAT
COPY %DSOURCE%MW.BAT %DTARGET%MW.BAT
IF NOT EXIST %DTARGET%MW.BAT GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%MW.BAT

REM BAS.BAT
ECHO Installing BAS.BAT
COPY %DSOURCE%BAS.BAT %DTARGET%BAS.BAT
IF NOT EXIST %DTARGET%BAS.BAT GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%BAS.BAT

  REM CBL.BAT
  REM ECHO Installing DBINFO.286
  REM COPY %DSOURCE%DBINFO.286 %DTARGET%DBINFO.286
  REM IF NOT EXIST %DTARGET%DBINFO.286 GOTO ERROR
  REM IF LAN SYSTEM DISTRIBUTE FILE
  REM IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DBINFO.286

ECHO Installing DBGINFO.286
COPY %DSOURCE%DBGINFO.286 %DTARGET%DBGINFO.286
IF NOT EXIST %DTARGET%DBGINFO.286 GOTO ERROR
REM IF LAN SYSTEM DISTRIBUTE FILE
IF EXIST ADX_SPGM:ADXLND0L.286  ADXCSU0L 3 5 %DTARGET%DBGINFO.286

CLS
ECHO 旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �                                                                         �
ECHO �  The installation of the IBM 4690 Application Debugger V4.16            �
ECHO �  was successful.                                                        �
ECHO �                                                                         �
ECHO �  To activate all of the changes and make the system capable of          �
ECHO �  running the debugger, please reload all of the store controllers.      �
ECHO �                                                                         �
ECHO 읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �
GOTO EXIT
:ERROR
CLS
ECHO 旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �                         *****  WARNING  *****                           �
ECHO �                                                                         �
ECHO �       Installation of the IBM 4690 Application Debugger has failed.     �
ECHO �                                                                         �
ECHO 읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴�
ECHO �
:EXIT
