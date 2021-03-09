\******************************************************************************
\******************************************************************************
\***
\***
\***   FILECHCK utility     01/12/2015                    Ranjith Gopalankutty
\***
\***   Original version
\***
\***
\***    
\***   This utility helps to check if a controller/Store has all the required
\***   files it should be. Program will be running against an input file 
\***   FILECHCK.cfg which will be a snapshot of an ideal file. 
\***
\***   Also using the utility, user can build a new input list of file will
\***   basically dumps all the files in to new input file. This is for enabling
\***   the utility to be updated without the need to manually edit the file 
\***   every time. 
\***
\***   Usage :
\***
\***   FILECHCK <> - Will check the Controller/Store against the provided
\***   input file and reports the output into FILECHCK.ERR
\***
\***   FILECHCK <Dump> - In this mode program will build a new input file
\***   FILECHCK.CFG which can be used for future reference.
\***
\***   FILECHCK <Help> - Displays the Help Screen to guide the user
\***
\******************************************************************************
\******************************************************************************
\******************************************************************************
\***
\***    Included code defining function related Global variables
\***
\******************************************************************************

    %INCLUDE BEMFDEC.J86    !   Message Dec
    %INCLUDE BOOTSDEC.J86   !   Reusable Functions
    %INCLUDE PSBF01G.J86    !   APPLICATION.LOG
    %INCLUDE PSBF20G.J86    !   ALLOCATE.DEALLOCATE.SESS.NUM


\******************************************************************************
\***
\***    Global Variable definitions
\***
\******************************************************************************

     STRING GLOBAL                                                      \
         BATCH.SCREEN.FLAG$,                                            \
         CURRENT.CODE$,                                                 \
         CURRENT.CODE.LOGGED$,                                          \
         FILE.OPERATION$,                                               \
         FUNCTION.FLAG$,                                                \
         MODULE$,                                                       \
         MODULE.NUMBER$,                                                \
         PROGRAM$,                                                      \
         PASSED.STRING$,                                                \
         VAR.STRING.1$,                                                 \
         VAR.STRING.2$                                                  !

     INTEGER*1 GLOBAL                                                   \
         ERROR.COUNT%,                                                  \
         FALSE,                                                         \
         TRUE                                                           !

     INTEGER*2 GLOBAL                                                   \
         CURRENT.REPORT.NUM%,                                           \
         EVENT.NO%,                                                     \
         MESSAGE.NO%,                                                   \
         PASSED.INTEGER%                                                !

\******************************************************************************
\***
\***   VARIABLE DECLARATIONS
\***
\******************************************************************************

    STRING                                                              \
         BUILD.LIST$,                                                   \
         CMD$,                                                          \
         CFG.FILE.NAME$,                                                \
         DAT.LIST$,                                                     \
         DD$,                                                           \
         DIRECTORY.NAME$,                                               \
         DQ2CE.FILE.NAME$,                                              \
         DQ2CE.FILE.PREFIX$,                                            \
         DQ2CE.FILE.SUFFIX$,                                            \
         ERR.FILE.NAME$,                                                \
         ERR.MESSAGE$,                                                  \
         FILECHCK.BULD.CFG$,                                            \
         FILE.PATH.C$,                                                  \
         FILE.PATH.D$,                                                  \
         FORMAT.FILE.FIRST$,                                            \
         FORMAT.FILE.MIDDLE$,                                           \
         FORMAT.FILE.LAST$,                                             \
         FORMAT.FILE.NAME$,                                             \
         FILECHCK.CFG.FILE$,                                            \
         FILECHCK.DAT.FILE$,                                            \
         LINE.CHR$,                                                     \
         SPACE$,                                                        \
         SWITCH$                                                        !

     INTEGER*1                                                          \
         EOB,                                                           \
         EOD,                                                           \
         EOF,                                                           \
         FILECHCK.CFG.CHCK,                                             \
         RC%                                                            !

     INTEGER*2                                                          \
         COUNT%,                                                        \
         ERR.SESS.NUM%,                                                 \
         FOUND%,                                                        \
         I%,                                                            \
         LEN%,                                                          \
         FILECHCK.BULD.SESS%,                                           \
         FILECHCK.CFG.SESS%,                                            \
         FILECHCK.DAT.SESS%                                             !


\******************************************************************************
\***
\***    FUNCTION DECLARATIONS
\***
\******************************************************************************

     %INCLUDE BASROUT.J86   ! OSShell function
     %INCLUDE BOOTSEXT.J86  ! Reusable Functions
     %INCLUDE BTCSTR.J86    ! For Trimming Functions

\******************************************************************************
\***
\***    Included code defining external Boots functions
\***
\******************************************************************************

     %INCLUDE PSBF01E.J86   ! APPLICATION.LOG
     %INCLUDE PSBF20E.J86   ! ALLOCATE.DEALLOCATE.SESS.NUM

\******************************************************************************
\***
\***    MAIN PROGRAM
\***
\******************************************************************************

     MAIN.PROGRAM:

         ON ERROR GOTO ERROR.DETECTED
         GOSUB INITIALISATION
         GOSUB MAIN.PROCESSING
         GOSUB TERMINATION

     STOP

\******************************************************************************
\***
\***   SUBROUTINE : INITIALISATION
\***
\******************************************************************************

     INITIALISATION:       
        
         FILECHCK.CFG.CHCK   = FALSE
         FILE.PATH.C$        = "ADXLXAAN::C:\"    
         ERR.FILE.NAME$      = FILE.PATH.C$ + "FILECHCK.ERR"        
         FILECHCK.CFG.FILE$  = FILE.PATH.C$ + "FILECHCK.CFG"
         FILECHCK.DAT.FILE$  = FILE.PATH.C$ + "FILECHCK.DAT"
         FILECHCK.BULD.CFG$  = FILE.PATH.C$ + "FILEBULD.CFG"


         EOB                 = FALSE 
         EOD                 = FALSE
         FORMAT.FILE.MIDDLE$ = "."
         FOUND%              = 0

         SPACE$              = " "

         GOSUB ALLOCATE.SESSION.NUMBERS

         CALL  OSSHELL("IF EXIST C:\FILECHCK.OK       DEL C:\FILECHCK.OK")
         CALL  OSSHELL("IF EXIST C:\FILECHCK.DAT      DEL C:\FILECHCK.DAT")

         CREATE POSFILE ERR.FILE.NAME$ AS ERR.SESS.NUM%	           

     RETURN

\******************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS:
\***
\***    Use the session number utility to allocate file numbers for the
\***    files used by the program.
\***
\******************************************************************************

     ALLOCATE.SESSION.NUMBERS:

         FUNCTION.FLAG$ = "O"

         PASSED.INTEGER%     = ERR.SESS.NUM%
         PASSED.STRING$      = ERR.FILE.NAME$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         ERR.SESS.NUM%       = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%     = FILECHCK.CFG.SESS%
         PASSED.STRING$      = FILECHCK.CFG.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         FILECHCK.CFG.SESS%  = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%     = FILECHCK.DAT.SESS%
         PASSED.STRING$      = FILECHCK.DAT.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         FILECHCK.DAT.SESS%  = F20.INTEGER.FILE.NO%


         PASSED.INTEGER%     = FILECHCK.BULD.SESS%
         PASSED.STRING$      = FILECHCK.BULD.CFG$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         FILECHCK.BULD.SESS% = F20.INTEGER.FILE.NO%      

     RETURN
 
\******************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY:
\***
\******************************************************************************

     CALL.F20.SESS.NUM.UTILITY: 
        
         FILE.OPERATION$ = FUNCTION.FLAG$
         CURRENT.REPORT.NUM% = PASSED.INTEGER%
         RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
         IF RC% <> 0 THEN GOTO FILE.ERROR

     RETURN

\******************************************************************************
\***
\***   SUBROUTINE : MAIN PROCESSING
\***
\******************************************************************************

    MAIN.PROCESSING:  

        SWITCH$    = UCASE$(COMMAND$)
        
        IF SWITCH$ = " "  THEN BEGIN                                 
                    
            GOSUB FIND.MISSING.PROCESS

            PRINT  COUNT% ; "FILES HAVE BEEN CHECKED"
            PRINT  FOUND% ; "MISSING FILES"

            PRINT #ERR.SESS.NUM%;COUNT%,"TOTAL FILES" 
            PRINT #ERR.SESS.NUM%;FOUND%,"MISSING FILES" 
             

            IF FOUND% = 0 THEN BEGIN
                CALL OSSHELL("REN" + SPACE$ + ERR.FILE.NAME$ + SPACE$ + "C:/FILECHCK.OK")
            ENDIF  


        ENDIF ELSE IF SWITCH$ = "DUMP" THEN BEGIN
           
            GOSUB BUILD.NEW.LIST
            GOSUB FORMAT.DUMP
            GOSUB TIDY.UP
        
        ENDIF ELSE IF SWITCH$ = "HELP" THEN BEGIN
             
            GOSUB FILECHCK.HELP.MENU

        ENDIF ELSE BEGIN
              
            PRINT "INVALID PARAMETERS PASSED, PROGRAM ENDING"

            GOSUB TERMINATION

        ENDIF

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE : FIND.MISSING.PROCESS
\***
\******************************************************************************	 

    FIND.MISSING.PROCESS:   

        PRINT  TIME.STAMP$(2) + "-CONFIGURATION FILE IS BEING OPENED"    


        OPEN FILECHCK.CFG.FILE$ AS FILECHCK.CFG.SESS% NOWRITE NODEL
        IF END #FILECHCK.CFG.SESS% THEN ERROR.DETECTED

        PRINT TIME.STAMP$(2) + "-READING THE CONFIGURATION FILE"
        PRINT TIME.STAMP$(2) + "-CHECKING THE FILES,PLEASE WAIT"

        WHILE EOF = FALSE

            IF END #FILECHCK.CFG.SESS% THEN FILECHCK.CFG.END
            READ #FILECHCK.CFG.SESS%; LINE LINE.CHR$

            LINE.CHR$ = MID$(LINE.CHR$,2,(LEN(LINE.CHR$)-2))


            IF LINE.CHR$ <> " " OR LINE.CHR$ <> "." OR LINE.CHR$ <>     \
                                                 ".." THEN BEGIN
                GOSUB FILE.EXISTENCE.CHECK
            ENDIF ELSE BEGIN

                WRITE #ERR.SESS.NUM% ;                                  \
                "INVALID FILE NAME IN THE INPUT FILE" , LINE.CHR$
            ENDIF

        WEND

    FILECHCK.CFG.END:

        EOF  = TRUE
        PRINT TIME.STAMP$(2) + "-EOF REACHED QUITTING"   

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE : FILE.EXISTENCE.CHECK
\***
\******************************************************************************	 

    FILE.EXISTENCE.CHECK:   
        
        COUNT% = COUNT% + 1

        ! Below command will check the size of the file.if particular file
        ! is not present in the controller,Program will call error routine.
        RC% = SIZE(LINE.CHR$)         

    RETURN
   
\******************************************************************************
\***
\***   SUBROUTINE : FIND.MISSING.PROCESS
\***
\******************************************************************************	 

    BUILD.NEW.LIST:   

        PRINT TIME.STAMP$(2) + "-BUILDING NEW LIST OF FILES"        

        GOSUB BACKUP.EXISTING 

        GOSUB OPEN.BUILD.FILE

        WHILE EOB = FALSE

            IF END #FILECHCK.BULD.SESS% THEN FILE.BUILD.END
            READ #FILECHCK.BULD.SESS%; LINE BUILD.LIST$

            IF BUILD.LIST$ = "OMC" THEN BEGIN
                PRINT TIME.STAMP$(2) + "-CREATING LIST OF FILES FOR C DRIVE"
            ENDIF ELSE IF BUILD.LIST$ = "OMD" THEN BEGIN 
                PRINT TIME.STAMP$(2) + "-CREATING LIST OF FILES FOR D DRIVE"
            ENDIF ELSE BEGIN
                GOSUB DUMP.FILES
            ENDIF

        WEND

    FILE.BUILD.END:

        EOB = TRUE  

    RETURN
\******************************************************************************
\***
\***   SUBROUTINE : BACKUP.EXISTING
\***
\******************************************************************************	

    BACKUP.EXISTING:

        PRINT TIME.STAMP$(2) + "-CHECKING EXISTING FILES "
         

        IF END #FILECHCK.CFG.SESS% THEN CREATE.NEW.FILE
        OPEN FILECHCK.CFG.FILE$ AS FILECHCK.CFG.SESS% NOWRITE NODEL    
        CLOSE FILECHCK.CFG.SESS%

        PRINT TIME.STAMP$(2) + "-BACKING UP EXISTING FILES "

        CALL  OSSHELL("IF EXIST C:\FILECHCK.BAK       DEL C:\FILECHCK.BAK") 
        CALL  OSSHELL("IF EXIST C:\FILECHCK.OK        DEL C:\FILECHCK.OK") 
        CALL  OSSHELL("REN C:\FILECHCK.CFG C:\FILECHCK.BAK" )  

        PRINT TIME.STAMP$(2) + "-CREATING NEW CONFIGURATION FILE"
        CREATE POSFILE FILECHCK.CFG.FILE$ AS FILECHCK.CFG.SESS%

    RETURN

 \******************************************************************************
\***
\***   SUBROUTINE : CREATE.NEW.FILE
\***
\******************************************************************************	

    CREATE.NEW.FILE:

        PRINT TIME.STAMP$(2) + "-CREATING NEW CONFIGURATION FILE"
        CREATE POSFILE FILECHCK.CFG.FILE$ AS FILECHCK.CFG.SESS%

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE : OPEN.BUILD.FILE
\***
\******************************************************************************	

    OPEN.BUILD.FILE:   

        OPEN FILECHCK.BULD.CFG$ AS FILECHCK.BULD.SESS% NOWRITE NODEL

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE :  DUMP.FILES
\***
\******************************************************************************
    
    DUMP.FILES:     

        CALL OSSHELL ( "DIR" + SPACE$ + BUILD.LIST$ + SPACE$ +   ">>"   \
                                  +         SPACE$ +  FILECHCK.DAT.FILE$)
    RETURN 


\******************************************************************************
\***
\***   SUBROUTINE :  FORMAT.DUMP
\***
\******************************************************************************
    
    FORMAT.DUMP:

        PRINT TIME.STAMP$(2) + "-FORMATTING STARTED PLEASE WAIT"
        IF END #FILECHCK.DAT.SESS% THEN ERROR.DETECTED
 
        OPEN FILECHCK.DAT.FILE$ AS FILECHCK.DAT.SESS% NOWRITE NODEL  

        WHILE EOD = FALSE  
   
            IF END #FILECHCK.DAT.SESS% THEN FILE.DATA.END
            READ #FILECHCK.DAT.SESS%; LINE DAT.LIST$

            IF MID$(DAT.LIST$,1,1) <> "." THEN BEGIN

            IF UCASE$(MID$(DAT.LIST$,3,9)) = "DIRECTORY" THEN BEGIN
                DIRECTORY.NAME$ = MID$(DAT.LIST$,16,12)
                READ #FILECHCK.DAT.SESS%; LINE DAT.LIST$
            ENDIF


            IF MID$(DAT.LIST$,1,1) <> SPACE$ AND MID$(DAT.LIST$,1,1) <> "."  \
                                                                THEN BEGIN

                FORMAT.FILE.FIRST$ = MID$(DAT.LIST$,1,8) 

                CALL LTRIM(FORMAT.FILE.FIRST$)
                CALL RTRIM(FORMAT.FILE.FIRST$)

                FORMAT.FILE.LAST$ = MID$(DAT.LIST$,10,3)

                CALL LTRIM(FORMAT.FILE.LAST$)
                CALL RTRIM(FORMAT.FILE.LAST$)

                IF MID$(DAT.LIST$,1,8) <> " " OR MID$(DAT.LIST$,10,3)   \
                                       <> " " OR MID$(DAT.LIST$,1,1)    \
                                       <> "." OR MID$(DAT.LIST$,1,2)    \
                                       <> ".." THEN BEGIN

                    IF MID$(DAT.LIST$,1,8)  = " " AND                   \
                        MID$(DAT.LIST$,10,3) <> " " THEN BEGIN
                        FORMAT.FILE.NAME$ = DIRECTORY.NAME$     +       \
                                            FORMAT.FILE.LAST$
                        GOSUB WRITE.FILECHCK.CFG 
                    ENDIF ELSE IF MID$(DAT.LIST$,1,8) <> " " AND        \
                        MID$(DAT.LIST$,10,3) = " "  THEN BEGIN           

                        FORMAT.FILE.NAME$ = DIRECTORY.NAME$ +           \
                                                FORMAT.FILE.FIRST$
                        GOSUB WRITE.FILECHCK.CFG
                    ENDIF ELSE IF MID$(DAT.LIST$,1,8) <> " " AND        \
                    MID$(DAT.LIST$,10,3) <> " " THEN BEGIN


                        FORMAT.FILE.NAME$ = DIRECTORY.NAME$ +           \
                        FORMAT.FILE.FIRST$ + FORMAT.FILE.MIDDLE$ +      \
                                             FORMAT.FILE.LAST$
                        GOSUB WRITE.FILECHCK.CFG

                    ENDIF

                ENDIF

            ENDIF

            ENDIF

        WEND

    FILE.DATA.END:

        EOD = TRUE

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE :  WRITE.FILE.LIST
\***
\******************************************************************************

    WRITE.FILECHCK.CFG: 

        WRITE #FILECHCK.CFG.SESS% ; FORMAT.FILE.NAME$

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE : CLOSE.PROCESSING
\***
\******************************************************************************

    CLOSE.PROCESSING:

        CLOSE FILECHCK.CFG.SESS%        
        CLOSE FILECHCK.BULD.SESS%

    RETURN
\******************************************************************************
\***
\***   SUBROUTINE : DEALLOCATE.SESSION.NUMBERS
\***
\******************************************************************************

    DEALLOCATE.SESSION.NUMBERS:

        FUNCTION.FLAG$   = "C"

        PASSED.INTEGER%   = ERR.SESS.NUM%
        PASSED.STRING$    = ERR.FILE.NAME$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        ERR.SESS.NUM%     = F20.INTEGER.FILE.NO%

        PASSED.INTEGER%   = FILECHCK.CFG.SESS%
        PASSED.STRING$    = FILECHCK.CFG.FILE$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        FILECHCK.CFG.SESS% = F20.INTEGER.FILE.NO%  

        PASSED.INTEGER%     = FILECHCK.DAT.SESS%
        PASSED.STRING$      = FILECHCK.DAT.FILE$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        FILECHCK.DAT.SESS%  = F20.INTEGER.FILE.NO%

        PASSED.INTEGER%     = FILECHCK.BULD.SESS%
        PASSED.STRING$      = FILECHCK.BULD.CFG$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        FILECHCK.BULD.SESS% = F20.INTEGER.FILE.NO%

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE : TIDY.UP
\***
\******************************************************************************	
    TIDY.UP:

        PRINT TIME.STAMP$(2) + "-PROCESSING COMPLETED SUCCESSFULLY"

        PRINT #ERR.SESS.NUM% ; "NEW LIST OF FILES BEEN CREATED SUCCESSFULLY"
        CLOSE ERR.SESS.NUM%   
        CALL OSSHELL("REN" + SPACE$ + ERR.FILE.NAME$ + SPACE$ + "C:/FILECHCK.OK")
        CLOSE FILECHCK.DAT.SESS%
        CALL OSSHELL("DEL" + SPACE$ + FILECHCK.DAT.FILE$)

    RETURN
\******************************************************************************
\***
\***   SUBROUTINE : FILECHCK.HELP.MENU
\***
\******************************************************************************

    FILECHCK.HELP.MENU:

         PRINT "                                                             "
         PRINT "   **********************************************************"
         PRINT "   *                                                        *"
         PRINT "   *           FILECHCK UTILITY - V1.0                      *"
         PRINT "   *                                                        *"
         PRINT "   *                                                        *"
         PRINT "   *  FILECHCK Utility Help                                 *"
         PRINT "   *                                                        *"
         PRINT "   *  FILECHCK <> - Program runs against FILECHCK.CFG and   *"
         PRINT "   *  checks the existence of the files in the controller.  *"
         PRINT "   *  FILECHCK.CFG should be present in the root drive with *"
         PRINT "   *  valid list of files.                                  *"
         PRINT "   *                                                        *"
         PRINT "   *  FILECHCK <DUMP> - Please run the program in this      *"
         PRINT "   *  mode only if you want to create a new list of files   *"
         PRINT "   *  in FILECHCK.CFG . But please make sure you are        *"
         PRINT "   *  keeping  FILEBULD.CFG with valid list of directory    *"
         PRINT "   *  names in root drive. As new list will be made based   *"
         PRINT "   *  on the directory names supplied                       *"
         PRINT "   *                                                        *"
         PRINT "   **********************************************************"

         GOSUB STOP.PROGRAM

\******************************************************************************
\***                                                                           
\***   ERROR ROUTINE  :  FILE.ERROR                                            
\***                                                                           
\***   Call APPLICATION.LOG to log an event 106 with                           
\***   flag, current report number and current key                             
\***                                                                           
\***                                                                           
\******************************************************************************

     FILE.ERROR:
        
         VAR.STRING.1$ = FILE.OPERATION$                     +          \
                CHR$(SHIFT(CURRENT.REPORT.NUM%, 8) AND 0FFH) +          \
                CHR$(CURRENT.REPORT.NUM% AND 0FFH)           +          \
                CURRENT.CODE$
         VAR.STRING.2$ = "FILECHCK"
         MESSAGE.NO%   = 0
         EVENT.NO%     = 106

         RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,               \
                      VAR.STRING.2$,EVENT.NO%)

         GOTO STOP.PROGRAM

\******************************************************************************
\***
\***   SUBROUTINE : TERMINATION
\***
\******************************************************************************

     TERMINATION:       
              

         GOSUB CLOSE.PROCESSING
         GOSUB DEALLOCATE.SESSION.NUMBERS
         GOSUB STOP.PROGRAM

\******************************************************************************
\***
\***   SUBROUTINE : ERROR.DETECTED
\***
\******************************************************************************

    ERROR.DETECTED:


        IF ERR = "OE" AND ERRF% = FILECHCK.BULD.SESS% THEN BEGIN
            PRINT TIME.STAMP$(2) + "-BUILD CONFIGURATION FILE IS MISSING"
            GOSUB STOP.PROGRAM
        ENDIF


        IF ERR = "OE" AND ERRF% = FILECHCK.CFG.SESS% THEN BEGIN 
            PRINT TIME.STAMP$(2) + "-CONFIGURATION FILE IS MISSING"
            GOSUB STOP.PROGRAM
        ENDIF

        IF ERR = "OE" THEN BEGIN

            PRINT #ERR.SESS.NUM% ; LINE.CHR$ + " IS MISSING IN THE CONTROLLER"
            FOUND% = FOUND% + 1
            RESUME 

        ENDIF


        GOSUB FILE.ERROR

\******************************************************************************
\***
\***   SUBROUTINE : STOP.PROGRAM
\***
\******************************************************************************

     STOP.PROGRAM:

     END

\******************************************************************************
\******************************************************************************
\***
\***    End of program 
\***
\******************************************************************************
\******************************************************************************