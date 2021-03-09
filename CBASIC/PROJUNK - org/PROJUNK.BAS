\******************************************************************************
\******************************************************************************
\***
\***
\***   PROJUNK utility     4/1/2015             Ranjith Gopalankutty
\***
\***   Original version
\***
\***
\***   Usage
\***   This utility will help to clear the junk characters from Second
\***   priority queue. Transactions which are failed at SAP PI side can
\***   be reprocessed using this utility by specifying the date.
\***
\***            PROJUNK 'HELP' 
\*** 
\***   Using the utility with 'HELP' switch will display the help screen.
\***   By specifying the date parameter, PROJUNK will find respective 
\***   DQ2CE or ADQ2CE files. If the user calls the program without 
\***   specifying any parameter also make the program to call the
\***   help screen. 

\******************************************************************************
\******************************************************************************
\******************************************************************************
\***
\***    Included code defining function related Global variables
\***
\******************************************************************************

    %INCLUDE BEMFDEC.J86
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
         CMD$,                                                          \
         CFG.FILE.NAME$,                                                \
         DD$,                                                           \
         DQ2CE.FILE.NAME$,                                              \
         DQ2CE.FILE.PREFIX$,                                            \
         DQ2CE.FILE.SUFFIX$,                                            \
         ERR.FILE.NAME$,                                                \
         ERR.MESSAGE$,                                                  \
         FILE.PATH.C$,                                                  \
         FILE.PATH.D$,                                                  \
         INPUT1$,                                                       \
         INPUT2$,                                                       \
         JUNK.CHAR$,                                                    \
         LINE.STRING$,                                                  \
         MM$,                                                           \
         PROCESSED.STRING$,                                             \
         PROJUNK.CFG.FILE$,                                             \
         PROJUNK.DAT.FILE$,                                             \
         SPACE$,                                                        \
         SWITCH$,                                                       \
         TEMP.STRING$,                                                  \
         VAL1$,                                                         \
         YY$                                                            \

     INTEGER*1                                                          \
         EOF,                                                           \
         RC%                                                            !

     INTEGER*2                                                          \
         COUNT%,                                                        \
         DATE%,                                                         \
         DD%,                                                           \
         DQ2CE.SESS.NUM%,                                               \
         DQ2CE.ARRAY(1),                                                \
         ERR.SESS.NUM%,                                                 \
         FOUND%,                                                        \
         I%,                                                            \
         LEN%,                                                          \
         MONTH.DAYS%,                                                   \
         POS%,                                                          \
         PROJUNK.CFG.SESS%,                                             \
         PROJUNK.DAT.SESS%                                              !

     INTEGER*4                                                          \
         J%,                                                            \
         K%,                                                            \
         LAST.JUNK%,                                                    \
         RECORD%,                                                       \
         TRANSACTION%,                                                  \
         TRANSACTION1%                                                  !


\******************************************************************************
\***
\***    FUNCTION DECLARATIONS
\***
\******************************************************************************

     %INCLUDE BASROUT.J86   ! OSShell function

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

         MONTH.DAYS%   = 12
         DIM DQ2CE.ARRAY(MONTH.DAYS%)
         DQ2CE.ARRAY(1) = 31
         DQ2CE.FILE.NAME$   = "DQ2CE."
         DQ2CE.FILE.PREFIX$ = "A"
         
         
         IF LEFT$(DATE$,2)  = "16" OR LEFT$(DATE$,2)= "20"          \ 
             OR LEFT$(DATE$,2) = "24" OR LEFT$(DATE$,2) = "28"      \
             OR LEFT$(DATE$,2) = "32" THEN BEGIN

             DQ2CE.ARRAY(2)  = 29
         ENDIF  ELSE BEGIN
             DQ2CE.ARRAY(2)  = 28
         ENDIF

         DQ2CE.ARRAY(3)  = 31
         DQ2CE.ARRAY(4)  = 30
         DQ2CE.ARRAY(5)  = 31
         DQ2CE.ARRAY(6)  = 30
         DQ2CE.ARRAY(7)  = 31
         DQ2CE.ARRAY(8)  = 31
         DQ2CE.ARRAY(9)  = 30
         DQ2CE.ARRAY(10) = 31
         DQ2CE.ARRAY(11) = 30
         DQ2CE.ARRAY(12) = 31

         FILE.PATH.C$       = "ADXLXAAN::C:\"
         FILE.PATH.D$       = "ADXLXAAN::D:\ADX_UDT1\"
         ERR.FILE.NAME$     = FILE.PATH.C$ + "PROJUNK.ERR"        
         PROJUNK.CFG.FILE$  = FILE.PATH.C$ + "PROJUNK.CFG"
         PROJUNK.DAT.FILE$  = FILE.PATH.C$ + "PROJUNK.DAT"
         FOUND%             = 0         

         SPACE$             = " "         

         GOSUB ALLOCATE.SESSION.NUMBERS

         CREATE POSFILE ERR.FILE.NAME$     AS ERR.SESS.NUM%        
         CREATE POSFILE PROJUNK.DAT.FILE$  AS PROJUNK.DAT.SESS%
          

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
          
         PASSED.INTEGER%   = ERR.SESS.NUM%
         PASSED.STRING$    = ERR.FILE.NAME$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         ERR.SESS.NUM%     = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%   = PROJUNK.CFG.SESS%
         PASSED.STRING$    = PROJUNK.CFG.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         PROJUNK.CFG.SESS% = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%   = PROJUNK.DAT.SESS%
         PASSED.STRING$    = PROJUNK.DAT.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         PROJUNK.DAT.SESS% = F20.INTEGER.FILE.NO%
      

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
        
        IF SWITCH$ = " "  OR LEN(SWITCH$) <> 6 OR                       \
                   MID$(SWITCH$,3,2) > "12" OR MID$(SWITCH$,3,2) < "01" \
                            OR    LEFT$(SWITCH$,2) < "01" OR            \
                     LEFT$(SWITCH$,2) > "99"                            \
                     OR RIGHT$(SWITCH$,2) > "31" OR RIGHT$(SWITCH$,2)   \
                     < "01"  OR MID$(SWITCH$,3,2) > "12"   OR           \
                                                MID$(SWITCH$,3,2) = " " \
                                                               THEN BEGIN
             GOSUB DISPLAY.HELP
        ENDIF  
        
        IF LEFT$(SWITCH$,2) > LEFT$(DATE$,2) THEN BEGIN
            PRINT "Future Year entered - No File"      
            GOSUB STOP.PROGRAM
        ENDIF  ELSE IF VAL(LEFT$(DATE$,2)) - VAL(LEFT$(SWITCH$,2)) > 1  \
                                                        THEN BEGIN
            PRINT "System Doesn't have the file for that year"
            GOSUB STOP.PROGRAM
        ENDIF


        IF LEFT$(SWITCH$,2) = LEFT$(DATE$,2) THEN BEGIN

            IF  MID$(SWITCH$,3,2) > MID$(DATE$,3,2)   THEN BEGIN
                PRINT " You entered future Month - No File"
                GOSUB STOP.PROGRAM 
            ENDIF

            IF MID$(SWITCH$,3,2) = MID$(DATE$,3,2) AND RIGHT$           \
                                 (SWITCH$,2) > RIGHT$(DATE$,2) THEN BEGIN
                PRINT "You Entered a future Date - No File"
                GOSUB STOP.PROGRAM
            ENDIF


            IF (VAL(MID$(DATE$,3,2)) - VAL(MID$(SWITCH$,3,2))) > 1  \
                THEN BEGIN    
                PRINT " More than one month backward date - No file " 
                GOSUB STOP.PROGRAM                        
            ENDIF 

            IF (VAL(MID$(DATE$,3,2)) - VAL(MID$(SWITCH$,3,2))) = 1 THEN BEGIN

                IF RIGHT$(DATE$,2) > "14" THEN BEGIN
                    PRINT "Given date is beyond 14 days"
                    GOSUB STOP.PROGRAM
                ENDIF
            ENDIF

        ENDIF

          IF  VAL(LEFT$(DATE$,2)) - VAL(LEFT$(SWITCH$,2)) = 1           \
                    AND VAL(MID$(DATE$,3,2)) - VAL(MID$(SWITCH$,3,2))   \
                                                        = -11 THEN BEGIN
            PRINT "Looking for last year files"
        ENDIF ELSE BEGIN
            IF  VAL(LEFT$(DATE$,2)) - VAL(LEFT$(SWITCH$,2)) = 1         \
                    AND VAL(MID$(DATE$,3,2)) - VAL(MID$(SWITCH$,3,2))   \
                                                   <> -11 THEN BEGIN
                 PRINT "File is not Present in the system"
                 GOSUB STOP.PROGRAM
            ENDIF
        ENDIF

        GOSUB FIND.DQ2CE.FILE
        GOSUB OPEN.DQ2CE.FILE
        GOSUB READ.DQ2CE.FILE

        PRINT TRANSACTION% ; "Transaction have been read"

        IF RECORD% = 0 THEN BEGIN
            PRINT "No junk character found"  
        ENDIF ELSE BEGIN
            PRINT RECORD%  ; "Transactions Has Junk Character" 
        ENDIF

        IF RECORD% > 0 THEN BEGIN
           GOSUB PROCESS.QCONSOLE.REPLAY
        ENDIF

    RETURN
\******************************************************************************
\***
\***   SUBROUTINE : FIND.DQ2CE.FILE
\***
\******************************************************************************	 

    FIND.DQ2CE.FILE:      
        
        DD$ = RIGHT$(SWITCH$,2)
        MM$ = MID$(SWITCH$,3,2)
        YY$ = LEFT$(SWITCH$,2)

        I%  = 01 

        IF SWITCH$ = DATE$ THEN BEGIN
            DQ2CE.FILE.PREFIX$ = ""
            DQ2CE.FILE.SUFFIX$ = "BIN"
            RETURN
        ENDIF

        IF MID$(SWITCH$,3,2) =  MID$(DATE$,3,2) THEN BEGIN

         DD% = VAL(RIGHT$(DATE$,2)) - VAL(DD$)
         IF DD% > 14 THEN BEGIN
             PRINT "Given date is beyond 14 days"
             GOSUB STOP.PROGRAM
         ENDIF ELSE BEGIN
             DQ2CE.FILE.SUFFIX$ = STR$(DD%)
         ENDIF

         IF LEN(STR$(DD%)) = 1 THEN BEGIN
             DQ2CE.FILE.SUFFIX$ = "0" + STR$(DD%)
         ENDIF   

        ENDIF ELSE BEGIN
        
        IF I% < = 14 THEN BEGIN
             DATE% = VAL(RIGHT$(DATE$,2))
             FOR I% = 01 TO 14
                 DATE% = DATE% - 1
                 IF DATE%=0 THEN BEGIN

                 IF VAL(MID$(DATE$,3,2)) - VAL(MID$(SWITCH$,3,2))   \
                                                     = -11 THEN BEGIN
                     DATE% = DQ2CE.ARRAY(12)              
                     ENDIF ELSE BEGIN                     
                     DATE% = DQ2CE.ARRAY(VAL(MID$(DATE$,3,2))-1)
                 ENDIF
                 ENDIF

                 IF DATE% = VAL(DD$) THEN BEGIN
                     DQ2CE.FILE.SUFFIX$ = STR$(I%)
                     IF LEN(STR$(I%)) = 1 THEN BEGIN
                         DQ2CE.FILE.SUFFIX$ = "0" + STR$(I%)
                     ENDIF
                 ENDIF   
             NEXT I% 

             IF VAL(DD$) < DATE% THEN BEGIN
                 PRINT "Given Date Is Beyond 14 Days"
                 GOSUB STOP.PROGRAM
             ENDIF

         ENDIF
     ENDIF

  RETURN
  
\******************************************************************************
\***
\***   SUBROUTINE : OPEN.DQ2CE.FILE
\***
\******************************************************************************	 

   OPEN.DQ2CE.FILE:

        DQ2CE.FILE.NAME$ = FILE.PATH.D$ + DQ2CE.FILE.PREFIX$ +          \
                                    DQ2CE.FILE.NAME$ + DQ2CE.FILE.SUFFIX$

        FUNCTION.FLAG$   = "O"          
        PASSED.INTEGER%  = DQ2CE.SESS.NUM%
        PASSED.STRING$   = DQ2CE.FILE.NAME$
        GOSUB CALL.F20.SESS.NUM.UTILITY
        DQ2CE.SESS.NUM%  = F20.INTEGER.FILE.NO%         

        OPEN DQ2CE.FILE.NAME$ AS DQ2CE.SESS.NUM% NOWRITE NODEL
        IF END #DQ2CE.SESS.NUM% THEN ERROR.DETECTED

   RETURN

\******************************************************************************
\***
\***   SUBROUTINE : READ.DQ2CE.FILE
\***
\******************************************************************************	

   READ.DQ2CE.FILE: 

       RECORD%      = 0 
       TRANSACTION% = 0

       ! To read a record from " to " as each transaction will be limited 
       ! between double quotes

       VAL1$ = CHR$(34)


       OPEN PROJUNK.CFG.FILE$ AS PROJUNK.CFG.SESS% NOWRITE NODEL
       IF END #PROJUNK.CFG.SESS% THEN ERROR.DETECTED
       READ #PROJUNK.CFG.SESS%; LINE JUNK.CHAR$

       WHILE EOF = FALSE  

       IF END #DQ2CE.SESS.NUM% THEN DQ2CE.END

                 READ #DQ2CE.SESS.NUM%; LINE  LINE.STRING$
                 IF LEFT$(LINE.STRING$,1) = VAL1$ AND                  \
                 RIGHT$(LINE.STRING$,1) = VAL1$ THEN BEGIN
                 TRANSACTION% = TRANSACTION% + 1

                ENDIF ELSE BEGIN
                     TEMP.STRING$ = LINE.STRING$
                     WHILE  RIGHT$(LINE.STRING$,1) <> VAL1$

                         READ #DQ2CE.SESS.NUM%; LINE  LINE.STRING$
                         TEMP.STRING$ = TEMP.STRING$ + LINE.STRING$

                     WEND

                          TRANSACTION1% = TRANSACTION1% + 1
                          LINE.STRING$  = TEMP.STRING$
                          TEMP.STRING$  = ""

               ENDIF

           GOSUB FIND.DQ2CE.JUNK 
       WEND


   DQ2CE.END:
    
        EOF          = TRUE   
        TRANSACTION% = TRANSACTION1% + TRANSACTION%
    
   RETURN

\******************************************************************************
\***
\***   SUBROUTINE : FIND.DQ2CE.JUNK
\***
\******************************************************************************	

    FIND.DQ2CE.JUNK:

        FOUND%  = MATCH(JUNK.CHAR$,LINE.STRING$,1)

        IF FOUND% > 0 THEN BEGIN         

            RECORD% = RECORD% + 1
            LEN%    = LEN(LINE.STRING$)
            GOSUB PROCESS.DQ2CE.JUNK

        ENDIF

        PROCESSED.STRING$ = "" 

    RETURN

\******************************************************************************
\***
\***   SUBROUTINE :  PROCESS.DQ2CE.JUNK
\***
\******************************************************************************

   PROCESS.DQ2CE.JUNK:         
        
        COUNT%     = 0
        J%         = 0
        K%         = 1
        LAST.JUNK% = 1
        POS%       = 1

        WHILE K% < = LEN% AND LAST.JUNK% = 1

            J% = MATCH(JUNK.CHAR$,LINE.STRING$,POS%)
            IF J% > 0 THEN BEGIN
                PROCESSED.STRING$ = PROCESSED.STRING$ +                 \ 
                            MID$(LINE.STRING$,POS%,J%-POS%)
                COUNT% = COUNT% + 1
            ENDIF ELSE BEGIN
                LAST.JUNK%        = 0
                PROCESSED.STRING$ = PROCESSED.STRING$ + RIGHT$          \
                (LINE.STRING$,(LEN% - LEN(PROCESSED.STRING$))-COUNT%)    
                WRITE #PROJUNK.DAT.SESS% ; MID$(PROCESSED.STRING$,2,    \
                                           (LEN(PROCESSED.STRING$)-2))   
            ENDIF

              POS% = J% + 1
              K%   = K% + 1
              J%   = 0

         WEND
        
       
     RETURN
\******************************************************************************
\***
\***   SUBROUTINE :  PROCESS.QCONSOLE.REPLAY
\***
\******************************************************************************

   PROCESS.QCONSOLE.REPLAY:
     
      CALL OSSHELL("ADX_UPGM/QCONSOLE -REPLAY" + SPACE$ + "C:/PROJUNK.DAT")

       PRINT "                                                            "
       PRINT "    Checking the status of low priority queue               "
       PRINT "                                                            "

      CALL OSSHELL("ADX_UPGM/QCONSOLE -QINFO" + SPACE$ +               \
                                                "D:/ADX_UDT1/DQ3CE.BIN")



   RETURN

\******************************************************************************
\***
\***   SUBROUTINE : CLOSE.PROCESSING
\***
\******************************************************************************

    CLOSE.PROCESSING:

         CLOSE DQ2CE.SESS.NUM%
         CLOSE PROJUNK.CFG.SESS%
         CLOSE PROJUNK.DAT.SESS%


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

         PASSED.INTEGER%   = PROJUNK.CFG.SESS%
         PASSED.STRING$    = PROJUNK.CFG.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         PROJUNK.CFG.SESS% = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%   = PROJUNK.DAT.SESS%
         PASSED.STRING$    = PROJUNK.DAT.FILE$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         PROJUNK.DAT.SESS% = F20.INTEGER.FILE.NO%

         PASSED.INTEGER%   = DQ2CE.SESS.NUM%
         PASSED.STRING$    = DQ2CE.FILE.NAME$
         GOSUB CALL.F20.SESS.NUM.UTILITY
         DQ2CE.SESS.NUM%   = F20.INTEGER.FILE.NO% 
             
     RETURN

\******************************************************************************
\***
\***   SUBROUTINE : DISPLAY.HELP
\***
\******************************************************************************

     DISPLAY.HELP:
         PRINT "                                                             "
         PRINT "   **********************************************************"
         PRINT "   *                                                        *"
         PRINT "   *   SECOND QUEUE JUNK CHARACTER PROCESSOR UTILITY-V1.0   *"
         PRINT "   *                                                        *"
         PRINT "   *                                                        *"
         PRINT "   *  PROJUNK Utility Help                                  *"
         PRINT "   *  Please enter the date in format 'PROJUNK <YYMMDD>'    *"
         PRINT "   *  Eg: PROJUNK 150131 , 15=Year,01=Month,31=Date         *"
         PRINT "   *                                                        *"
         PRINT "   **********************************************************"

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
         VAR.STRING.2$ = "PROJUNK"
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

         PRINT #ERR.SESS.NUM% ; DATE$ + SPACE$ + "REPROCESSING IS SUCCESSFUL"
         CLOSE ERR.SESS.NUM%
         CALL OSSHELL("IF EXIST C:\PROJUNK.OK        DEL C:\PROJUNK.OK")     
         CALL OSSHELL("REN" + SPACE$ + ERR.FILE.NAME$ + SPACE$ +       \
                                                        "C:\PROJUNK.OK")

         GOSUB CLOSE.PROCESSING
         GOSUB DEALLOCATE.SESSION.NUMBERS
         GOSUB STOP.PROGRAM



\******************************************************************************
\***
\***   SUBROUTINE : ERROR.DETECTED
\***
\******************************************************************************

     ERROR.DETECTED:           

         WRITE #ERR.SESS.NUM%; "An Error Occurred "
         WRITE #ERR.SESS.NUM%; "Fatal Error:" + ERR
         WRITE #ERR.SESS.NUM%; "Session Number: " + STR$(ERRF%)
         WRITE #ERR.SESS.NUM%; "Line Number:" + STR$(ERRL)

         IF ERR = "OE" AND ERRF% = DQ2CE.SESS.NUM% THEN BEGIN 
            PRINT  "File is not present for the requested date"
            GOSUB STOP.PROGRAM
         ENDIF

         IF ERR = "OE" AND ERRF% = PROJUNK.CFG.SESS% THEN BEGIN 
            PRINT  "Configuration file is missing "
            GOSUB STOP.PROGRAM
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