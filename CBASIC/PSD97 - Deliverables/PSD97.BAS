\******************************************************************************
\******************************************************************************
\***
\***
\***   PSD97                 26/10/2015                 Ranjith Gopalankutty
\***
\***   Original version
\***
\***
\***   Usage
\***   This utility will help to clear the serial number of DVCHR,DIDIR
\***   and DEALDIR bin files in BCF.BIN. Which will be used during deal 
\***   initial load process.
\***   
\***   Screen will have below standard options          
\***       F3  = To return to main menu       
\***       F1  = Help Key 
\***       ESC = To escape out of the screen
\***   
\***   
\******************************************************************************
\******************************************************************************
\***
\***    Included code defining function related Global variables
\***
\******************************************************************************
\******************************************************************************

    %INCLUDE BCFDEC.J86     !   BCF file function
    %INCLUDE BEMFDEC.J86    !   BEMF file function
    %INCLUDE PSBF01G.J86    !   APPLICATION.LOG
    %INCLUDE PSBF20G.J86    !   ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF39G.J86    !   Display Manager
    %INCLUDE PSBUSEG.J86    !   Chain Function

\******************************************************************************
\******************************************************************************
\***
\***    Global Variable definitions
\***
\******************************************************************************
\******************************************************************************

    STRING GLOBAL                                                       \
        BATCH.SCREEN.FLAG$,                                             \ Screen flags
        CHAIN.TO.PROG$,                                                 \ Chain to programs
        CURRENT.CODE$,                                                  \ Current code
        CURRENT.CODE.LOGGED$,                                           \
        FILE.NO$,                                                       \
        FILE.OPERATION$,                                                \ File operations indicator
        FUNCTION.FLAG$,                                                 \
        MODULE$,                                                        \
        MODULE.NUMBER$,                                                 \ Module number
        PASSED.STRING$,                                                 \
        PROGRAM$,                                                       \
        VAR.STRING.1$,                                                  \
        VAR.STRING.2$                                                   !

     INTEGER*1 GLOBAL                                                   \
         COMMAND.MODE,                                                  \
         ERROR.COUNT%,                                                  \
         FALSE,                                                         \
         TRUE                                                           !

     INTEGER*2 GLOBAL                                                   \
         CURRENT.REPORT.NUM%,                                           \
         EVENT.NO%,                                                     \
         MESSAGE.NO%,                                                   \
         PASSED.INTEGER%                                                !

    INTEGER*4 GLOBAL                                                    \
         RET.KEY%                                                       \

\******************************************************************************
\******************************************************************************
\***
\***   VARIABLE DECLARATIONS
\***
\******************************************************************************
\******************************************************************************

    STRING                                                              \
        BCF.OPEN.FLAG$,                                                 \
        OPERATOR.NUMBER$                                                !


    INTEGER*1                                                           \
        EXIT.NOW,                                                       \
        INITIALIZED,                                                    \
        PSD97.FIELD%,                                                   \
        RESET                                                           !

    INTEGER*4                                                           \
        RC%                                                             !

\******************************************************************************
\******************************************************************************
\***
\***    FUNCTION DECLARATIONS
\***
\******************************************************************************
\******************************************************************************

    %INCLUDE BASROUT.J86   ! OSShell function
    %INCLUDE BCFEXT.J86    ! BCF File function

\******************************************************************************
\******************************************************************************
\***
\***    Included code defining external Boots functions
\***
\******************************************************************************
\******************************************************************************

    %INCLUDE PSBF01E.J86   ! APPLICATION.LOG
    %INCLUDE PSBF20E.J86   ! ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF24E.J86   ! Error detected
    %INCLUDE PSBF39E.J86   ! Display Manager

\******************************************************************************
\******************************************************************************
\***
\***    MAIN PROGRAM
\***
\******************************************************************************
\******************************************************************************

    MAIN.PROGRAM:

        ON ERROR GOTO ERROR.DETECTED
        %INCLUDE PSBUSEE.J86            ! Chaining Parameters

    PROGRAM.CONTROL:

        GOSUB INITIALISATION
        GOSUB MAIN.SCREEN
        GOSUB TERMINATION

    STOP

\******************************************************************************
\******************************************************************************
\***
\***    MAIN PROGRAM
\***
\******************************************************************************
\******************************************************************************

    INITIALISATION:


        BCF.OPEN.FLAG$ = "N"

        INITIALIZED = FALSE         
        TRUE = -1
        FALSE = 0  
        BATCH.SCREEN.FLAG$ = "S"        ! Screen
        CHAIN.TO.PROG$     = "PSB50"    ! PSB50 for chain
        MODULE.NUMBER$     = "PSD97"    ! Current Module
        RESET = FALSE
        CALL DM.INIT                    ! Display Manager Initialization
        EXIT.NOW = FALSE
        PSD97.FIELD% = 3                ! Default screen input field

        GOSUB ALLOCATE.SESSION.NUMBERS

        

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS:
\***
\***    Use the session number utility to allocate file numbers for the
\***    files used by the program.
\***
\******************************************************************************
\******************************************************************************

    ALLOCATE.SESSION.NUMBERS:


        CALL BCF.SET 
        FUNCTION.FLAG$ EQ "O"

        PASSED.INTEGER% EQ BCF.REPORT.NUM%
        PASSED.STRING$ EQ BCF.FILE.NAME$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        BCF.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY:
\***
\******************************************************************************
\******************************************************************************

    CALL.F20.SESS.NUM.UTILITY: 
        
        FILE.OPERATION$ = "O"
        CURRENT.REPORT.NUM% = PASSED.INTEGER%
        RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
        IF RC% <> 0 THEN GOTO FILE.ERROR

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    Screen Validation
\***
\******************************************************************************
\******************************************************************************

    MAIN.SCREEN:

        WHILE NOT EXIT.NOW

            CALL DM.SHOW.SCREEN(1, "", 2, 2)


            ! Reads BCF and get the current serial numbers of various deal 
            ! related files and display on the screen. If the current 
            ! values are already zero, user can refrain from running the
            ! program

            GOSUB BCF.OPEN

            GOSUB BCF.READ

            GOSUB BCF.CURRENT.FIELD

            GOSUB CLOSE.PROCESSING

            ! if the values are already zero reset flag will be set. if the user
            ! tries to enter the key again B186 message will be shown on the
            ! screen

            IF RESET = TRUE THEN BEGIN
                  CALL DM.STATUS                                        \
            ("MESSAGE(186,'as BCF serial numbers are already initialized')")

            ENDIF

            CALL DM.CURRENT.FIELD (PSD97.FIELD%) 

            INITIALIZED = FALSE

            WHILE INITIALIZED <> TRUE 

                RET.KEY% = DM.PROCESS.SCREEN(PSD97.FIELD%               \
                                     , PSD97.FIELD%,FALSE)

                IF RET.KEY% = F3.KEY%   THEN BEGIN

                    INITIALIZED = TRUE
                    EXIT.NOW    = TRUE
                    GOSUB CHAIN.TO.CALLER

                ENDIF ELSE IF RET.KEY% = ESC.KEY%    THEN BEGIN
                    GOTO ESCAPE.PRESSED	


                ENDIF ELSE IF RET.KEY% = ENTER.KEY%  THEN BEGIN

                    ! Logic executes only if the 'Y' flag is been typed
                    ! and enter key is pressed, if so it will be display
                    ! processing message.

                    IF UCASE$(FIELD$(3)) = CHR$(89) THEN BEGIN
                        CALL DM.STATUS ("MESSAGE(251,'')")
                        WAIT ;500

                    ! Opens BCF file and reads 14th record, after getting
                    ! the DIDIR,DVCHR AND DEALDIR serial number it sets
                    ! the flag accordingly. If the values are already 
                    ! zero then RESET and INITIALIZED flag will set and
                    ! message B186 will be shown on the screen

                        GOSUB BCF.OPEN

                        GOSUB BCF.READ

                        GOSUB VALIDATE.BCF

                        GOSUB CLOSE.PROCESSING

                        IF RESET = TRUE THEN BEGIN

                            INITIALIZED = TRUE

                        ENDIF ELSE BEGIN

                    ! If serial numbers are not set to zero then main processing
                    ! subroutine will be called and the values will be set to zero

                            GOSUB BCF.OPEN.LOCKED            ! For writing

                            GOSUB MAIN.PROCESSING

                            CALL DM.STATUS ("MESSAGE(299,'')")

                            GOSUB BCF.CURRENT.FIELD

                        ENDIF                 

                    ENDIF ELSE BEGIN

                    ! Displays the message if the input letter is not Y

                        CALL DM.STATUS ("MESSAGE(005,'')")              

                    ENDIF

                ENDIF ELSE BEGIN
                    !Displays if the key pressed is not ESCAPE,F3 or enter 
                    CALL DM.STATUS ("MESSAGE(001,'')")

                ENDIF

            WEND

        WEND 

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    BCF OPEN
\***
\******************************************************************************
\******************************************************************************

    BCF.OPEN:

        FILE.OPERATION$ = "O"
        IF BCF.OPEN.FLAG$ <> "Y" THEN BEGIN

            IF END #BCF.SESS.NUM% THEN FILE.ERROR
            OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM% 
            BCF.OPEN.FLAG$ = "Y"

        ENDIF

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    BCF OPEN
\***
\******************************************************************************
\******************************************************************************

    BCF.OPEN.LOCKED:

        FILE.OPERATION$ = "O"
        IF BCF.OPEN.FLAG$ <> "Y" THEN BEGIN

            IF END #BCF.SESS.NUM% THEN FILE.ERROR
            OPEN BCF.FILE.NAME$ RECL BCF.RECL% AS BCF.SESS.NUM% LOCKED 
            BCF.OPEN.FLAG$ = "Y"

        ENDIF

    RETURN
\******************************************************************************
\******************************************************************************
\***
\***    BCF READ
\***
\******************************************************************************
\******************************************************************************

    BCF.READ:

        ! Record number in BCF , which holds deal related serial numbers
        BCF.REC.NO% = 14                            
        FILE.OPERATION$ = "R"
        RC% = READ.BCF

        IF RC% <> 0 THEN BEGIN
            GOSUB FILE.ERROR
        ENDIF

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    VALIDATE BCF
\***
\******************************************************************************
\******************************************************************************

    VALIDATE.BCF:         

        IF  BCF.DEALDIR.SERIAL.NUM$ = "0000" AND          \          
                          BCF.DIDIR.SERIAL.NUM$   = "0000" AND          \
                          BCF.DVCHR.SERIAL.NUM$   = "0000" THEN BEGIN


            RESET = TRUE
        ENDIF

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    BCF.CURRENT.FIELD
\***
\******************************************************************************
\******************************************************************************

    BCF.CURRENT.FIELD:
        ! fields to display the current values from BCF

        FIELD$(6) = BCF.DEALDIR.SERIAL.NUM$
        FIELD$(7) = BCF.DIDIR.SERIAL.NUM$
        FIELD$(8) = BCF.DVCHR.SERIAL.NUM$

    RETURN


\******************************************************************************
\******************************************************************************
\***
\***    MAIN PROCESSING
\***
\******************************************************************************
\******************************************************************************

    MAIN.PROCESSING:

        BCF.REC.NO% = 14

        FILE.OPERATION$ = "R"

        RC% = READ.BCF

        IF RC% <> 0 THEN BEGIN
            GOTO FILE.ERROR
        ENDIF ELSE BEGIN
            GOSUB SET.SERIAL.ZERO
        ENDIF

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***    SET.SERIAL.ZERO
\***
\******************************************************************************
\******************************************************************************

    SET.SERIAL.ZERO:

      ! Below hard coded variables will reset the serial numbers of DEALDIR
      ! DIDIR and DVCHR. Store can have an initial deal load then

        BCF.DEALDIR.SERIAL.NUM$ = "0000"
        BCF.DIDIR.SERIAL.NUM$   = "0000"
        BCF.DVCHR.SERIAL.NUM$   = "0000"

        FILE.OPERATION$ = "W"

        RC% = WRITE.BCF

        IF RC% <> 0 THEN BEGIN
            GOTO FILE.ERROR
        ENDIF ELSE BEGIN  
            RESET = TRUE
        ENDIF 


    RETURN

\******************************************************************************
\******************************************************************************
\***
\***     CLOSE.PROCESSING
\***
\******************************************************************************
\******************************************************************************

    CLOSE.PROCESSING:

        IF BCF.OPEN.FLAG$ <> "N" THEN BEGIN
            CLOSE BCF.SESS.NUM%   
            BCF.OPEN.FLAG$ = "N"
        ENDIF

    RETURN 

\******************************************************************************
\******************************************************************************
\***
\***     DEALLOCATE.SESSION.NUMBERS
\***
\******************************************************************************
\******************************************************************************

    DEALLOCATE.SESSION.NUMBERS:

        FUNCTION.FLAG$   = "C"          

        PASSED.INTEGER% EQ BCF.REPORT.NUM%
        PASSED.STRING$ EQ BCF.FILE.NAME$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        BCF.SESS.NUM% EQ F20.INTEGER.FILE.NO%

    RETURN

\******************************************************************************
\******************************************************************************
\***
\***   SUBROUTINE :  ESCAPE.PRESSED
\***
\******************************************************************************
\******************************************************************************

    ESCAPE.PRESSED:

        PSBCHN.MENCON = "000000"

    CHAIN.TO.CALLER: 

        IF COMMAND.MODE THEN BEGIN
            CLEARS
            GOSUB STOP.PROGRAM
        ENDIF

        ! Processing - Please wait
        CALL DM.STATUS ("MESSAGE(405,'')")
        CALL DM.QUIT

        ! Current program and chaining program
        PSBCHN.APP = "C:/ADX_UPGM/PSE50.286"
        PSBCHN.PRG = "C:/ADX_UPGM/" + CHAIN.TO.PROG$ + ".286"

        %INCLUDE PSBCHNE.J86          ! Include CHAIN operation

        GOSUB STOP.PROGRAM

    RETURN


\******************************************************************************
\******************************************************************************
\***
\***   SUBROUTINE : TERMINATION
\***
\******************************************************************************
\******************************************************************************

     TERMINATION:

         GOSUB CLOSE.PROCESSING
         GOSUB DEALLOCATE.SESSION.NUMBERS

     RETURN

\******************************************************************************
\******************************************************************************
\***                                                                           
\***   ERROR ROUTINE  :  FILE.ERROR                                            
\***                                                                           
\***   Call APPLICATION.LOG to log an event 106 with                           
\***   flag, current report number and current key                             
\***                                                                           
\***                                                                           
\******************************************************************************
\******************************************************************************

    FILE.ERROR:         

        EVENT.NO%     = 106

        FILE.NO$  = CHR$(SHIFT(BCF.REPORT.NUM%,8))            +         \
                  CHR$(SHIFT(BCF.REPORT.NUM%,0))

        IF FILE.OPERATION$ = "O" THEN BEGIN  ! Open error

            MESSAGE.NO%   = 501    ! Message No. for open error

        ENDIF ELSE IF FILE.OPERATION$ = "R" THEN BEGIN  ! Read error

            MESSAGE.NO%   = 508    ! Message No. for Read error

        ENDIF ELSE IF FILE.OPERATION$ = "W" THEN BEGIN  ! Read error

            MESSAGE.NO%   = 504    ! Message No. for Write error

        ENDIF

        VAR.STRING.1$ = FILE.OPERATION$                       +         \
                 CHR$(SHIFT(BCF.REPORT.NUM%, 8) AND 0FFH)     +         \
                 CHR$(BCF.REPORT.NUM% AND 0FFH)               +         \
                 CURRENT.CODE$
        VAR.STRING.2$ = RIGHT$("000" + STR$(BCF.REPORT.NUM%),3) +       \
                 UNPACK$(CURRENT.CODE$)  

        RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,                \
                      VAR.STRING.2$,EVENT.NO%)


        GOTO ESCAPE.PRESSED

\******************************************************************************
\******************************************************************************
\***
\***   SUBROUTINE : ERROR.DETECTED
\***
\******************************************************************************
\******************************************************************************

    ERROR.DETECTED:

        IF ERR = "NP" THEN BEGIN    ! Command mode run
            COMMAND.MODE = TRUE       ! Command mode is set to TRUE
            RESUME PROGRAM.CONTROL
        ENDIF

      
        GOSUB FILE.ERROR

\******************************************************************************
\******************************************************************************
\***
\***   SUBROUTINE : STOP.PROGRAM
\***
\******************************************************************************
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
 