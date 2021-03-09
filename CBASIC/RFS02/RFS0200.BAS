\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   rfs0200.bas  $
\***
\***   $Revision:   1.8  $
\***
\******************************************************************************

\*******************************************************************************
\*******************************************************************************
\***
\***    PROGRAM:  RFS02 -  RF Report Browser Program
\***
\***    VERSION A.       NIK SEN              15 October 1998
\***
\***    VERSION B.       Charles Skadorwa     15 December 1998
\***                     Prevent program abort if ENTER pressed & no reports.
\***
\***    VERSION C.       Charles Skadorwa     12 Jnauary 1999
\***                     Fix to stop report screen (1st screen) redrawing twice   
\***                     when returning from viewing a reports versions.
\***
\***    VERSION D.       Charles Skadorwa     13 Jnauary 1999
\***                     Fix to correctly interpret short file names listed in the
\***                     rfrdesc.dat file. Fix to enable correct processing of
\***                     non-existent filenames in the rfrdesc.dat file.
\***
\***    VERSION E.       Chris Combes         21st May 2004
\***                     Check added to ensure that report cannot be more than 
\***                     ARRAY.SIZE% lines long (and thus cause an error when 
\***                     assigning to REPORT$ Array). If report is too long it is
\***                     trucated to ARRAY.SIZE%-1 and a line is added to the end
\***                     of the report stating it has been trucated.  ARRAY.SIZE%
\***                     increased from 2000 to 3000
\***
\***    VERSION F.       RANJITH GOPALANKUTTY   24th October  2014
\***                     GAP Archival report has issue of duplicity, display
\***                     engine combines more than one report and displays.Also
\***                     when  the user taps in to different versions of  report
\***                     ,it just shows first report and repeating it to total
\***                     number of times the user performed GAP in a business
\***                     day, added logic to fix those.
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        
\***  O V E R V I E W                                                      
\***                                                                        
\***  This program allows RF HHT Reports to be viewed or printed from the 
\***  controller.      
\***
\****************************************************************************
       
       %INCLUDE RFRDEDEC.J86      ! RF REPORT DESCRIPTORS FILE
       %INCLUDE PRINTDEC.J86      ! PRINTER

       %INCLUDE PSBUSEG.J86       ! CHAINING VARIABLES
       %INCLUDE PSBF01G.J86       ! APPLICATION.LOG
       %INCLUDE PSBF03G.J86       ! DISPLAY MANAGER
       %INCLUDE PSBF16G.J86       ! CONV TO HEX   
       %INCLUDE PSBF20G.J86       ! ALLOCATE.DEALLOCATE.SESS.NUM   



\****************************************************************************
\    Declare working variables
\****************************************************************************

   INTEGER*1 EOF
   INTEGER*1 EVENT.NUMBER%,    \
             ERROR.COUNT%,     \
             IP.ERROR.COUNT%
  
   INTEGER*1 GLOBAL FALSE
   INTEGER*1 GLOBAL TRUE
  
   INTEGER*2 MESSAGE.NO%
   INTEGER*2 DISPLAY.ITEM%       ! The line in the table,i.e., min 1, max 12
   INTEGER*2 DISPLAY.COUNT%

   INTEGER*2 RC%
   INTEGER*2 INPUT.EXIT%
   INTEGER*2 SCREEN.EXIT%
   INTEGER*2 NEW.HHT.EXIT%
   INTEGER*2 BACKUP.EXISTS%
   INTEGER*2 INSERT.LINE%
   INTEGER*2 PAGE.MOVE%
   INTEGER*2 TWIN.HOST%             
   INTEGER*2 SAVED.CHANGES%
   INTEGER*2 FILENAME.END%       ! End of filename position (for extract)  !DCS
   INTEGER*2 ESC.KEY                                                   
   INTEGER*2 F1.KEY
   INTEGER*2 F3.KEY
   INTEGER*2 F4.KEY
   INTEGER*2 F5.KEY
   INTEGER*2 F6.KEY
   INTEGER*2 F7.KEY
   INTEGER*2 F8.KEY
   INTEGER*2 F10.KEY
   INTEGER*2 ENTER.KEY
   INTEGER*2 TAB.KEY
   INTEGER*2 SHIFT.TAB      
   INTEGER*2 UP.KEY
   INTEGER*2 DOWN.KEY,         \
             PGUP.KEY,         \
             PGDN.KEY,         \
             NO.OF.LINES%

   INTEGER*2 WORKFILE.SESS.NUM%,       \
             WKFILE2.SESS.NUM%

   INTEGER*2 BACKUP.SESS.NUM%,         \
             RETURN.FIELD%,            \
             SCR.EXITKEY,              \
             SB.EVENT.NO%,             \
             CURR.SESS.NUM%,           \
             INDEX%,                   \
             PAGE%,                    \
             PAGES%
             
   INTEGER*2 ARRAY.SIZE%               !ECC - Stores the size of REPORT$ Array   

   INTEGER*2 GLOBAL INTEGER.DATA%, PASSED.INTEGER%,    \
                    CURRENT.REPORT.NUM%, ADX.FUNCTION%

   INTEGER*4 X%
   INTEGER*4 Y%,                          \
             RFRDESC.REC.NO%,             \
             NO.OF.HEADERS%,              \
             ADX.RETURN.CODE%,            \
             REPORT.START,                \
             REPORT.END
   INTEGER*4 NO.REPORTS%
   INTEGER*4 TERM%                        ! Number of HHT found in file
   INTEGER*4 STARTING.POINT%              ! For displaying the table
   INTEGER*4 END.POINT%                   ! For displaying the table
   INTEGER*4 LINE.COUNT%
   INTEGER*4 LIST.ITEM%                   ! Current position in total list

   INTEGER*4 L% (1)                       ! Length
   INTEGER*4 INDEX2%                                                    ! FRG
   INTEGER*4 RECORD%                                                    ! FRG
   INTEGER*4 RECORD2%                                                   ! FRG
   INTEGER*4 S% (1)          ! Start


   INTEGER*4 GLOBAL F24.RETURN.CODE%
      
   STRING MESSAGE$
   STRING FILE.NAME$
   STRING GAP.FILE.NAME$                                                ! FRG
   STRING VAR.STRING.1$                   ! For use with logging event 106
   STRING VAR.STRING.2$                  
   STRING LINE.X$
   STRING CHAIN.FLAG$
   STRING MODULE$,                     \
          PASSED.STRING$,              \
          SB.UNIQUE$,                  \
          UNIQUE.2$,                   \
          SB.MESSAGE$,                 \
          FILE.OPERATION$,             \
          ADXSERVE.DATA$,              \
          TITLE$,                      \
          TODAYS.DATE$,                \
          REPORT.DATE$,                \
          REPORT.TIME$,                \
          NEW.LINE$,                   \
          WORKFILE.FILE.NAME$,         \
          REPORT.LINE$,                \
          STORE.NUMBER$,               \
          NAME$,                       \
          REPORT.FILE.NAME$





   STRING GLOBAL STRING.DATA$
   STRING GLOBAL PROGRAM$
   STRING GLOBAL MODULE.NUMBER$           ! For use with display mgr routines
   STRING GLOBAL BATCH.SCREEN.FLAG$
   STRING GLOBAL OPERATOR.NUMBER$
   STRING GLOBAL SCREEN.NUMBER$,       \
                 FUNCTION.FLAG$,       \
                 CURRENT.CODE.LOGGED$


   
\****************************************************************************
\    Variables to be used as field names when read from the file
\****************************************************************************


   STRING FILLER$
   STRING LN$
   STRING FILENAME.STR$       ! End of filename position (for extract)  !DCS
     

   STRING LINE$(1)                                                      ! FRG
   STRING LINE2$(1)                                                     ! FRG
   STRING LINE3$(1)                                                     ! FRG
   STRING REPORT$(1) 


\****************************************************************************
\   Included code defining external Boots functions
\****************************************************************************

       %INCLUDE RFRDEEXT.J86               
       %INCLUDE PRINTEXT.J86


       %INCLUDE PSBF01E.J86       ! APPLICATION.LOG
       %INCLUDE PSBF03E.J86       ! DISPLAY MANAGER
       %INCLUDE PSBF04E.J86       ! EXTERNAL MESSAGE
       %INCLUDE PSBF08E.J86       ! Print Report Function
       %INCLUDE PSBF12E.J86       ! HELP
       %INCLUDE PSBF16E.J86       ! CONV TO HEX
       %INCLUDE PSBF20E.J86       ! ALLOCATE.DEALLOCATE.SESS.NUM
       %INCLUDE PSBF24E.J86       ! STANDARD.ERROR.DETECTED
       %INCLUDE PSBF26E.J86       ! CONV.TO.INTEGER


\****************************************************************************
\   EXT included code defining file related external functions
\****************************************************************************

   %INCLUDE ADXSERVE.J86
   
\****************************************************************************
\    MAIN PROGRAM
\****************************************************************************

   ON ERROR GOTO ERROR.DETECTED 

   %INCLUDE PSBUSEE.J86          ! External functions for chaining operations

   SCR.EXITKEY = 0

   GOSUB INITIALISATION
   GOSUB DISPLAY.SCREEN1
   GOSUB INITIALISE.TABLE
   GOSUB DISPLAY.TABLE
   GOSUB RECEIVE.INPUT

              
PROGRAM.EXIT:               
                                          

       STRING.DATA$ = ""  :  INTEGER.DATA% = 1
       GOSUB  X.DM.POSF
       STRING.DATA$ = "30"  :  INTEGER.DATA% = 0
       GOSUB  X.DM.SETF
       MESSAGE.NO% = 405  :  STRING.DATA$ = ""  :  RETURN.FIELD% = 1
       GOSUB X.EXTERNAL.MESSAGE
       STRING.DATA$ = ""  :  INTEGER.DATA% = 20
       GOSUB  X.DM.NEXTF
                                 

      
\*********************************************************************
\*                                                                   *
\*     CHAIN.TO.CALLER: Return to calling program                    *
\*     ------------------------------------------                    *
\*                                                                   *
\*********************************************************************

CHAIN.TO.CALLER:

      PSBCHN.PRG = "ADX_UPGM:" + PSBCHN.APP + ".286"
      PSBCHN.APP = PROGRAM$
      STRING.DATA$ = ""
      INTEGER.DATA% = 0
      RC% = DM.CLSDIS(STRING.DATA$, INTEGER.DATA%)   

      CHAIN.FLAG$ = "P"
      %INCLUDE PSBCHNE.J86

       STOP

\****************************************************************************
\    Initialisation
\****************************************************************************

INITIALISATION:


      PROGRAM$           = "RFS02"
      MODULE$            = "00"
      MODULE.NUMBER$     =  PROGRAM$ + MODULE$
      BATCH.SCREEN.FLAG$ = "S" ! Screen
                                                   
      STRING.DATA$ = ""
      INTEGER.DATA% = -1
      RC% = DM.INITDM(STRING.DATA$, INTEGER.DATA%)
      IF RC% <> 0 THEN GOTO PROGRAM.EXIT

      STRING.DATA$ = "D"
      INTEGER.DATA% = 0
      RC% = DM.OPNDIS(STRING.DATA$, INTEGER.DATA%)
      IF RC% <> 0 THEN GOTO PROGRAM.EXIT           

      CALL RFRDE.SET             
      CALL PRINT.SET

      FUNCTION.FLAG$ = "O"

      PASSED.INTEGER% = RFRDE.REPORT.NUM%
      PASSED.STRING$ = RFRDE.FILE.NAME$
      GOSUB CALL.F20.SESS.NUM.UTILITY
      RFRDE.SESS.NUM% = F20.INTEGER.FILE.NO% 

      PASSED.INTEGER% = PRINT.REPORT.NUM%
      PASSED.STRING$ = PRINT.FILE.NAME$
      GOSUB CALL.F20.SESS.NUM.UTILITY
      PRINT.SESS.NUM% = F20.INTEGER.FILE.NO% 
      
      ESC.KEY = 27
      F1.KEY = -1
      F3.KEY = -3
      F4.KEY = -4
      F5.KEY = -5 
      F6.KEY = -6 
      F7.KEY = -7
      F8.KEY = -8
      F10.KEY = -10 
      ENTER.KEY = 0
      UP.KEY = 16 
      DOWN.KEY = 17
      TAB.KEY = 9
      SHIFT.TAB = 8217
      PGUP.KEY = 329
      PGDN.KEY = 337
      
      FALSE = 0
      TRUE = 1
      
      GAP.FILE.NAME$ = "D:\ADX_UDT1\RFRONWAY.WEK"                       ! FRG
      EOF = FALSE
      SAVED.CHANGES%= TRUE
      INPUT.EXIT% = FALSE
      SCREEN.EXIT% = FALSE
      
      STARTING.POINT% = 1
      DISPLAY.ITEM% = 1
      LIST.ITEM% = 1
      NO.REPORTS% = 0


      WORKFILE.SESS.NUM% = 53
      WKFILE2.SESS.NUM% = 54

      ARRAY.SIZE%=3000           !ECC
      DIM REPORT$(ARRAY.SIZE%)   !ECC      


RETURN


!****************************************************************************
!   Read the RFRDESC file and assign the values to array variables 
!****************************************************************************

INITIALISE.TABLE:
   

   EOF = FALSE
                     
   CURR.SESS.NUM% = RFRDE.SESS.NUM%
   RFRDESC.REC.NO% = 1
   NO.OF.HEADERS% = 1
   IF END # RFRDE.SESS.NUM% THEN END.OF.RFRDESC.FILE

   CURR.SESS.NUM% = RFRDE.SESS.NUM%
       
   OPEN RFRDE.FILE.NAME$ DIRECT RECL RFRDE.RECL% AS RFRDE.SESS.NUM%    \
        NOWRITE NODEL
     

   WHILE EOF = FALSE

         READ FORM "C51"; #RFRDE.SESS.NUM%, RFRDESC.REC.NO% ; LN$
                       
         IF MID$(LN$,49,1) <> "H" THEN BEGIN
            REPORT$(NO.OF.HEADERS%) = LEFT$(LN$,32)   
            NO.OF.HEADERS% = NO.OF.HEADERS% + 1
         ENDIF
         RFRDESC.REC.NO% = RFRDESC.REC.NO% + 1

END.INITIALISE.TABLE:
   WEND    

   NO.REPORTS% = NO.OF.HEADERS% - 2

   CLOSE RFRDE.SESS.NUM%
   TITLE$ = "Report Name                     "

RETURN                           


END.OF.RFRDESC.FILE:
   
      EOF = TRUE
      
      GOTO END.INITIALISE.TABLE
                   



!****************************************************************************
!   Display table on screen
!****************************************************************************
   
DISPLAY.TABLE:
   
   !***************************************************************************
   !   First ensure that the table is cleared.
   !   For each field in the table, erase any contents.
   !***************************************************************************
                        

      FOR X% = 1 TO 12
         STRING.DATA$=""                        
         INTEGER.DATA%=9+(X%*3)          
         GOSUB X.DM.POSF                       
         STRING.DATA$=STRING$(3,"   ")   
         INTEGER.DATA%=0                      
         GOSUB X.DM.PUTF                      

         STRING.DATA$=""                      
         INTEGER.DATA%=10+(X%*3)       
         GOSUB X.DM.POSF                        
         STRING.DATA$=STRING$(12," ")
         INTEGER.DATA%=0                        
         GOSUB X.DM.PUTF                       
         STRING.DATA$ = "1"
         GOSUB X.DM.SETF
   
      NEXT X%
      
    
      !****************************************************************************
      !   Fist work out if showing last page and less than a full page
      !****************************************************************************
    
      IF LIST.ITEM% > (NO.REPORTS%-MOD(NO.REPORTS%,12)) \
         THEN END.POINT%=MOD(NO.REPORTS%,12) ELSE END.POINT%=12
      
      IF LIST.ITEM% > 12 THEN BEGIN
         IF MOD(LIST.ITEM%,12) = 0 THEN X% = LIST.ITEM%-11 ELSE \
            X% = LIST.ITEM% - (MOD(LIST.ITEM%,12))+1 
      ENDIF ELSE X% = 1

      FOR LINE.COUNT% = 1 TO END.POINT%
         
         STRING.DATA$=""                        !   Position the ID number
         INTEGER.DATA%=9+(LINE.COUNT%*3)        !
         GOSUB X.DM.POSF                        !
         STRING.DATA$=STR$(X%)                  !   Place the ID number 
         INTEGER.DATA%=0                        !
         GOSUB X.DM.PUTF                        !
 
         STRING.DATA$=""                        !   Position the report title
         INTEGER.DATA%=10+(LINE.COUNT%*3)       !  
         GOSUB X.DM.POSF                        !
         STRING.DATA$ = LEFT$(REPORT$(X%),20)   !   Place the report title
         INTEGER.DATA%=0                        !
         GOSUB X.DM.PUTF                        !
         STRING.DATA$ = "0"                     !   Make the value visible
         GOSUB X.DM.SETF                        !
 
                       
         X% = X% + 1
 
      NEXT LINE.COUNT%

      IF NO.REPORTS% > 0 THEN BEGIN
         
         STRING.DATA$ = ""
         INTEGER.DATA% = 2
         GOSUB X.DM.POSF
         STRING.DATA$ = "Use Arrow Keys to select required report  " 
         INTEGER.DATA% = 0
         GOSUB X.DM.PUTF
 
      ENDIF ELSE BEGIN

         STRING.DATA$ = ""
         INTEGER.DATA% = 2
         GOSUB X.DM.POSF
         STRING.DATA$ = "No Reports Available.  "
         INTEGER.DATA% = 0
         GOSUB X.DM.PUTF

      ENDIF    

      STRING.DATA$ = ""
      INTEGER.DATA% = 6
      GOSUB X.DM.POSF
      STRING.DATA$ = TITLE$
      INTEGER.DATA% = 0
      GOSUB X.DM.PUTF
      
RETURN

!****************************************************************************
!    Procedure to receive and respond to main screen input
!****************************************************************************

   !****************************************************************************
   !   Receive input loop
   !****************************************************************************
    
RECEIVE.INPUT:
       
      WHILE SCREEN.EXIT% <> TRUE 
         WHILE INPUT.EXIT% <> TRUE
       
!****************************************************************************
!   Position and highlight the input field
!****************************************************************************
 
            STRING.DATA$ = ""
            INTEGER.DATA% = 10+(3*DISPLAY.ITEM%)
            GOSUB X.DM.POSF

            STRING.DATA$ = "3313333333113001"
            INTEGER.DATA% = 0
            GOSUB X.DM.SETF

            STRING.DATA$=""
            INTEGER.DATA%=0
            GOSUB X.DM.UPDF
            SCR.EXITKEY = F03.RETURNED.INTEGER%
        
!****************************************************************************
!   Check if the input is valid, i.e., needs acting upon
!****************************************************************************

            IF (SCR.EXITKEY = F1.KEY) OR      \
               (SCR.EXITKEY = F3.KEY) OR      \
               (SCR.EXITKEY = F5.KEY) OR      \
               (SCR.EXITKEY = F7.KEY) OR      \
               (SCR.EXITKEY = F8.KEY) OR      \
               (SCR.EXITKEY = ESC.KEY) OR     \
               (SCR.EXITKEY = UP.KEY) OR      \
               (SCR.EXITKEY = DOWN.KEY) OR    \
               (SCR.EXITKEY = PGUP.KEY) OR    \
               (SCR.EXITKEY = PGDN.KEY) OR    \
               (SCR.EXITKEY = TAB.KEY) OR     \
               (SCR.EXITKEY = SHIFT.TAB) OR   \
               (SCR.EXITKEY = ENTER.KEY) THEN \
               INPUT.EXIT% = TRUE
         WEND 

      !****************************************************************************
      !   Respond to input
      !****************************************************************************
      
         INPUT.EXIT% = FALSE
  
      !****************************************************************************
      !   Respond to help key
      !****************************************************************************
      
         IF SCR.EXITKEY = F1.KEY THEN BEGIN
            GOSUB SCREEN.HELP
            STRING.DATA$ = SCREEN.NUMBER$                             
            INTEGER.DATA% = 0
            RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
            GOSUB DISPLAY.TABLE
         ENDIF
      
      !****************************************************************************
      !   Respond to cursor movements
      !****************************************************************************

         IF (SCR.EXITKEY = DOWN.KEY) OR  \
            (SCR.EXITKEY = UP.KEY) OR    \
            (SCR.EXITKEY = TAB.KEY) OR   \
            (SCR.EXITKEY = SHIFT.TAB) THEN GOSUB CHOOSE.ITEM

      !****************************************************************************
      !   Respond to page up and page down keys
      !****************************************************************************
      
         IF (SCR.EXITKEY = F7.KEY) OR (SCR.EXITKEY = PGUP.KEY) OR \
            (SCR.EXITKEY = F8.KEY) OR (SCR.EXITKEY = PGDN.KEY)    \
            THEN GOSUB MOVE.PAGE
 
      !****************************************************************************
      !   Respond to escape key or quit key
      !****************************************************************************
      
         IF (SCR.EXITKEY = ESC.KEY) OR \
            (SCR.EXITKEY = F3.KEY) THEN GOSUB EXIT.RFS0200
 
                       
         IF (SCR.EXITKEY = ENTER.KEY) AND (NO.REPORTS% > 0) THEN BEGIN ! BCS
            GOSUB DISPLAY.REPORTS
            GOSUB INITIALISE.TABLE
            GOSUB DISPLAY.TABLE
         ENDIF

      WEND 
RETURN


\****************************************************************************
\***
\*** DISPLAY REPORTS
\***
\****************************************************************************
DISPLAY.REPORTS:

   EOF = FALSE
                      
   STRING.DATA$ = "3303333333003111"
   INTEGER.DATA% = 0
   GOSUB X.DM.SETF
       
   STRING.DATA$ = LEFT$(REPORT$(LIST.ITEM%),20)
   INTEGER.DATA% = 0
   GOSUB X.DM.PUTF
       
   TITLE$ = "Versions of " + LEFT$(REPORT$(LIST.ITEM%),20)
   
   ! Extract filename from selected entry (remove spaces at end)        !DCS
   FILENAME.STR$ = MID$(REPORT$(LIST.ITEM%),21,12)                      !DCS
   FILENAME.END% = MATCH(" ", FILENAME.STR$,1)                          !DCS
   
   IF FILENAME.END% > 1 THEN BEGIN                                      !DCS
      FILENAME.STR$ = LEFT$(FILENAME.STR$,(FILENAME.END% - 1))          !DCS
   ENDIF                                                                !DCS
   
   REPORT.FILE.NAME$ = "D:\ADX_UDT1\" + FILENAME.STR$                   !DCS


   GOSUB GET.VERSIONS


                      
   INPUT.EXIT% = FALSE
   SCREEN.EXIT% = FALSE
      
   STARTING.POINT% = 1
   DISPLAY.ITEM% = 1
   LIST.ITEM% = 1

   GOSUB DISPLAY.TABLE
            
   GOSUB GET.INPUT

   GOSUB INITIALISE.TABLE
                            
   INPUT.EXIT% = FALSE
   SCREEN.EXIT% = FALSE
      
   STARTING.POINT% = 1
   DISPLAY.ITEM% = 1
   LIST.ITEM% = 1

RETURN
                

\****************************************************************************
\***
\*** GET VERSIONS
\***
\****************************************************************************
GET.VERSIONS:


   DIM LINE$(ARRAY.SIZE%)                                               ! FRG
   CURR.SESS.NUM% = WORKFILE.SESS.NUM%
   INDEX% = 1
   RECORD% = 1                                                          ! FRG
   LN$    = " "                                                         ! FRG
   IF END # WORKFILE.SESS.NUM% THEN NO.DISPLAY.REPORTS                    !DCS
   OPEN REPORT.FILE.NAME$ AS WORKFILE.SESS.NUM% NOWRITE NODEL
   EOF = FALSE

   ! If Report selected is GAP Archival then read the second line       ! FRG
   !    starting with 1M, which has the time of GAP and add it with     ! FRG
   !    0M line to make the version. As all other reports occurs only   ! FRG
   !    once in a day, GAP archival may have many reports against       ! FRG
   !    each user for a particular business date. So 1M line will help  ! FRG
   !    to separate those.                                              ! FRG
   ! Else                                                               ! FRG
   !    Just read all the 0M lines and make a version by increasing     ! FRG
   !    the Version counter by one.                                     ! FRG
   
   
   WHILE EOF = FALSE

   IF REPORT.FILE.NAME$ = GAP.FILE.NAME$  THEN BEGIN                    ! FRG
       IF END # WORKFILE.SESS.NUM% THEN END.OF.WORKFILE                 ! FRG
       READ #WORKFILE.SESS.NUM% ; LN$                                   ! FRG
       IF LEFT$(LN$,2) = "0M" THEN   BEGIN                              ! FRG
           REPORT$(INDEX%) = RIGHT$(LN$,20)                             ! FRG
       ENDIF ELSE IF LEFT$(LN$,2) = "1M" THEN BEGIN                     ! FRG
           LINE$(RECORD%)= RIGHT$(LN$,20)                               ! FRG
           REPORT$(INDEX%)=REPORT$(INDEX%)  + RIGHT$(LN$,20)            ! FRG
           INDEX%  = INDEX%  + 1                                        ! FRG
           RECORD% = RECORD% + 1                                        ! FRG
       ENDIF                                                            ! FRG
   ENDIF ELSE BEGIN                                                     ! FRG
       IF END # WORKFILE.SESS.NUM% THEN END.OF.WORKFILE                 
       READ #WORKFILE.SESS.NUM% ; LN$                                   
       IF LEFT$(LN$,1) = "0" THEN BEGIN                                 
           REPORT$(INDEX%) = RIGHT$(LN$,20)                             
           INDEX% = INDEX% + 1                                          
       ENDIF                                                            
   ENDIF                                                                

END.DISPLAY.REPORTS:
   WEND

   CLOSE WORKFILE.SESS.NUM%

NO.DISPLAY.REPORTS:

   
   NO.REPORTS% = INDEX% - 1

RETURN


END.OF.WORKFILE: 
       EOF = TRUE
       GOTO END.DISPLAY.REPORTS

!****************************************************************************
!    Procedure to receive and respond to report list screen input
!****************************************************************************
    
GET.INPUT:
       
      WHILE SCREEN.EXIT% <> TRUE 
         WHILE INPUT.EXIT% <> TRUE
       
 
            STRING.DATA$ = ""
            INTEGER.DATA% = 10+(3*DISPLAY.ITEM%)
            GOSUB X.DM.POSF

            STRING.DATA$ = "3313333333113001"
            INTEGER.DATA% = 0
            GOSUB X.DM.SETF

            STRING.DATA$=""
            INTEGER.DATA%=0
            GOSUB X.DM.UPDF
            SCR.EXITKEY = F03.RETURNED.INTEGER%
        

            IF (SCR.EXITKEY = F1.KEY) OR      \
               (SCR.EXITKEY = F3.KEY) OR      \
               (SCR.EXITKEY = F5.KEY) OR      \
               (SCR.EXITKEY = F7.KEY) OR      \
               (SCR.EXITKEY = F8.KEY) OR      \
               (SCR.EXITKEY = ESC.KEY) OR     \
               (SCR.EXITKEY = UP.KEY) OR      \
               (SCR.EXITKEY = DOWN.KEY) OR    \
               (SCR.EXITKEY = PGUP.KEY) OR    \
               (SCR.EXITKEY = PGDN.KEY) OR    \
               (SCR.EXITKEY = TAB.KEY) OR     \
               (SCR.EXITKEY = SHIFT.TAB) OR   \
               (SCR.EXITKEY = ENTER.KEY) THEN \
               INPUT.EXIT% = TRUE
         WEND 

      
         INPUT.EXIT% = FALSE
  
      
         IF SCR.EXITKEY = F1.KEY THEN BEGIN
            GOSUB SCREEN.HELP
            STRING.DATA$ = SCREEN.NUMBER$                             
            INTEGER.DATA% = 0
            RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
            GOSUB DISPLAY.TABLE
         ENDIF
      

         IF (SCR.EXITKEY = DOWN.KEY) OR  \
            (SCR.EXITKEY = UP.KEY) OR    \
            (SCR.EXITKEY = TAB.KEY) OR   \
            (SCR.EXITKEY = SHIFT.TAB) THEN GOSUB CHOOSE.ITEM

      
         IF (SCR.EXITKEY = F7.KEY) OR (SCR.EXITKEY = PGUP.KEY) OR \
            (SCR.EXITKEY = F8.KEY) OR (SCR.EXITKEY = PGDN.KEY)    \
            THEN GOSUB MOVE.PAGE
 
      
         IF (SCR.EXITKEY = ESC.KEY) THEN GOTO EXIT.RFS0200

         IF (SCR.EXITKEY = F3.KEY) THEN BEGIN
            STRING.DATA$ = "3303333333003111"
            INTEGER.DATA% = 0
            GOSUB X.DM.SETF
            SCREEN.EXIT% = TRUE
         ENDIF
      

         IF SCR.EXITKEY = F5.KEY AND NO.REPORTS% > 0 THEN BEGIN
            GOSUB REPORT.PRINT   
            GOSUB GET.VERSIONS
            GOSUB DISPLAY.TABLE
         ENDIF

         IF SCR.EXITKEY = ENTER.KEY AND NO.REPORTS% > 0 THEN BEGIN
            NAME$ = REPORT$(LIST.ITEM%)
            GOSUB DISPLAY.SCREEN2
            GOSUB REPORT.DISPLAY
            GOSUB REPORT.INPUT
            SCREEN.EXIT% = FALSE
            GOSUB DISPLAY.SCREEN1
            GOSUB GET.VERSIONS
            GOSUB DISPLAY.TABLE
         ENDIF

      WEND 
RETURN

\****************************************************************************
\***
\***   REPORT DISPLAY
\***
\***   Extract report from file
\***   Print report
\***
\****************************************************************************
REPORT.DISPLAY:
                                  
 
     PAGE% = 1
                                                                                                                                         
     CURR.SESS.NUM% = WKFILE2.SESS.NUM%
     !Processing will happen in PROCESS.REPORT WITH END.OF.WKFILE2      ! FRG
     !IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE3                     
     IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2                      ! FRG
     OPEN REPORT.FILE.NAME$ AS WKFILE2.SESS.NUM% NOWRITE NODEL
     NO.OF.LINES% = 1 
     ! DIM REPORT$(ARRAY.SIZE%)                                         ! FRG
     GOSUB PROCESS.REPORT                                               ! FRG

     ! Below Lines code have been moved to PROCESS.REPORT Sub routine & ! FRG
     ! logic has been added to fix duplicate report issue while seeing  ! FRG
     ! GAP Archival reports.                                            ! FRG

     ! EOF = FALSE                                                      ! FRG
     ! WHILE (EOF = FALSE) AND (NO.OF.LINES%<=ARRAY.SIZE%)              ! FRG ECC
           

     !   READ #WKFILE2.SESS.NUM% ; LN$                                  ! FRG
     !    IF LN$ = "0M" + NAME$ THEN BEGIN                              ! FRG
     !       REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)                     ! FRG
     !       NO.OF.LINES% = NO.OF.LINES% + 1                            ! FRG
     !       LN$ = "  "                                                 ! FRG
     !       WHILE (LEFT$(LN$,2) <> "0M") AND \                         ! FRG
     !             (NO.OF.LINES%<=ARRAY.SIZE%)                          ! FRG ECC    
     !          READ #WKFILE2.SESS.NUM%;LN$                             ! FRG
     !          IF LEFT$(LN$,2) <> "0M" THEN BEGIN                      ! FRG
     !             REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)               ! FRG
     !             NO.OF.LINES% = NO.OF.LINES% + 1                      ! FRG
     !          ENDIF                                                   ! FRG
     !       WEND                                                       ! FRG
     !    ENDIF                                                         ! FRG

!END.GET.REPORT:                                                        ! FRG  

!     WEND                                                              ! FRG  

    
   
     IF NO.OF.LINES%>ARRAY.SIZE% THEN BEGIN           !ECC
        NO.OF.LINES%=ARRAY.SIZE%                      !ECC
    REPORT$(ARRAY.SIZE%)="  REPORT TRUNCATED  "   !ECC 
     ENDIF
      
     CLOSE WKFILE2.SESS.NUM%                                           

     REPORT.START = 1
     GOSUB OUTPUT.REPORT
                 
                                      
RETURN

!END.OF.WKFILE3:                                                        ! FRG
!       EOF = TRUE                                                      ! FRG
!       GOTO END.GET.REPORT                                             ! FRG


\****************************************************************************
\***
\***   PROCESS REPORT
\***
\***   Reads Each file and reads the content in to the array
\***  
\***
\****************************************************************************

PROCESS.REPORT:                                                         ! FRG

     INDEX2% =1                                                         ! FRG
     RECORD2%=1                                                         ! FRG
     DIM LINE2$(ARRAY.SIZE%)                                            ! FRG
     DIM LINE3$(ARRAY.SIZE%)                                            ! FRG
     DIM REPORT$(ARRAY.SIZE%)                                           ! FRG
     EOF = FALSE

     ! IF Report selected is GAP Archival then it reads both OM         ! FRG
     !    and  1M ,As 1M line has unique time factor, which will be     ! FRG
     !    validated  against the Version.If it matches then report      ! FRG
     !    will be displayed Else read will continue until it finds the  ! FRG
     !    right report.                                                 ! FRG
     ! ELSE                                                             ! FRG
     !    Read will be performed against only 0M lines as it has        ! FRG
     !    unique Date and Time combination of all other reports and     ! FRG
     !    displays on the screen                                        ! FRG

     WHILE (EOF = FALSE) AND (NO.OF.LINES% <= ARRAY.SIZE%)              ! ECC

     IF REPORT.FILE.NAME$ = GAP.FILE.NAME$ THEN BEGIN                   ! FRG
         IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2                  ! FRG
         READ #WKFILE2.SESS.NUM% ; LN$                                  ! FRG
         IF LEFT$(LN$,2) = "0M" THEN BEGIN                              ! FRG
             LINE2$(INDEX2%) = RIGHT$(LN$,20)                           ! FRG
         ENDIF ELSE IF LEFT$(LN$,2) = "1M" THEN BEGIN                   ! FRG
             LINE3$(RECORD2%)= RIGHT$(LN$,20)                           ! FRG
             IF LINE3$(RECORD2%) = RIGHT$(NAME$,20) THEN BEGIN          ! FRG
                  REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)                ! FRG
                  NO.OF.LINES% = NO.OF.LINES% + 1                       ! FRG
                  LN$ = ""                                              ! FRG

             WHILE ((LEFT$(LN$,2) <> "0M") AND                          \ FRG
                  (NO.OF.LINES% <= ARRAY.SIZE%))                        ! FRG
                 IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2          ! FRG
                 READ #WKFILE2.SESS.NUM%;LN$                            ! FRG
                 IF LEFT$(LN$,2) <> "0M" THEN BEGIN                     ! FRG
                     REPORT$(1) = LINE2$(INDEX2%)                       ! FRG
                     REPORT$(2) = LINE3$(RECORD2%)                      ! FRG
                     REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)             ! FRG
                     NO.OF.LINES% = NO.OF.LINES% + 1                    ! FRG
                 ENDIF ELSE IF LEFT$(LN$,2) = "0M" THEN BEGIN           ! FRG
                     EOF = TRUE                                         ! FRG
                 ENDIF                                                  ! FRG
             WEND                                                       ! FRG
             ENDIF                                                      ! FRG
                 INDEX2%  = INDEX2%  + 1                                ! FRG
                 RECORD2% = RECORD2% + 1                                ! FRG
         ENDIF                                                          ! FRG
     ENDIF  ELSE BEGIN                                                  ! FRG
         IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2                  ! FRG
         READ #WKFILE2.SESS.NUM% ; LN$
         IF LN$ = "0M" + NAME$ THEN BEGIN                                
             REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)                      
             NO.OF.LINES% = NO.OF.LINES% + 1                             
             LN$ = ""                                                    
             WHILE ((LEFT$(LN$,2) <> "0M") AND                          \ FRG
                  (NO.OF.LINES% <= ARRAY.SIZE%))                         
                 IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2          ! FRG
                 READ #WKFILE2.SESS.NUM%;LN$
                 IF LEFT$(LN$,2) <> "0M" THEN BEGIN
                     REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)
                     NO.OF.LINES% = NO.OF.LINES% + 1  

             ! Below lines of code will Restrict the extra Read being   ! FRG
             ! performed after 'END OF REPORT' and screen will not      ! FRG
             ! display any extra lines after it.                        ! FRG
            
                 ENDIF ELSE IF LEFT$(LN$,2) = "0M" THEN BEGIN           ! FRG
                     EOF = TRUE                                         ! FRG
                ENDIF                                                   ! FRG
             WEND                                                       ! FRG
         ENDIF                                                          ! FRG
     ENDIF                                                              ! FRG
   
END.GET.REPORT: 
     WEND
RETURN

END.OF.WKFILE2:
     EOF = TRUE
     GOTO END.GET.REPORT
\****************************************************************************
\***
\***   OUTPUT REPORT
\***
\***   Clear table
\***   Work out start and end point
\***
\****************************************************************************
   
OUTPUT.REPORT:
                         
      FOR X% = 12 TO 65
         STRING.DATA$ = ""                        
         INTEGER.DATA% = X%       
         GOSUB X.DM.POSF                       
         STRING.DATA$ = STRING$(20," ")   
         INTEGER.DATA% = 0                      
         GOSUB X.DM.PUTF                      
   
      NEXT X%
              
      REPORT.END = REPORT.START + 53
      IF REPORT.END > NO.OF.LINES% THEN REPORT.END = NO.OF.LINES%

      X% = 12
      FOR Y% = REPORT.START TO REPORT.END
         
         STRING.DATA$ = ""                      
         INTEGER.DATA% = X%                     
         GOSUB X.DM.POSF                         
         STRING.DATA$ = REPORT$(Y%)             
         INTEGER.DATA% = 0                      
         GOSUB X.DM.PUTF                        
 
         X% = X% + 1
 
      NEXT Y%

      STRING.DATA$ = ""
      INTEGER.DATA% = 6
      GOSUB X.DM.POSF

             
    ! All other report uses NAME$ to display the date as the NAME$      ! FRG
    ! string get changed during GAP Archival processing,it will be      ! FRG
    ! replaced with REPORT$(1)                                          ! FRG
     
      IF REPORT.FILE.NAME$  = "D:\ADX_UDT1\RFRONWAY.WEK" THEN BEGIN     ! FRG
          STRING.DATA$ = RIGHT$(TITLE$,20) + " - " + REPORT$(1)         ! FRG
      ENDIF ELSE BEGIN                                                  ! FRG
          STRING.DATA$ = RIGHT$(TITLE$,20) + " - " + NAME$
      ENDIF 
      INTEGER.DATA% = 0
      GOSUB X.DM.PUTF
      
RETURN


\****************************************************************************
\***
\***   REPORT INPUT
\***
\****************************************************************************
    
REPORT.INPUT:
       
      WHILE SCREEN.EXIT% <> TRUE 
         WHILE INPUT.EXIT% <> TRUE
       
 
            STRING.DATA$ = ""
            INTEGER.DATA% = 11
            GOSUB X.DM.POSF


            STRING.DATA$=""
            INTEGER.DATA%=0
            GOSUB X.DM.UPDF
            SCR.EXITKEY = F03.RETURNED.INTEGER%
        
            IF (SCR.EXITKEY = F1.KEY) OR      \
               (SCR.EXITKEY = F3.KEY) OR      \
               (SCR.EXITKEY = F7.KEY) OR      \
               (SCR.EXITKEY = F8.KEY) OR      \
               (SCR.EXITKEY = ESC.KEY) OR     \
               (SCR.EXITKEY = PGUP.KEY) OR    \
               (SCR.EXITKEY = PGDN.KEY) THEN  \
               INPUT.EXIT% = TRUE
         WEND 

      
         INPUT.EXIT% = FALSE
  
      
         IF SCR.EXITKEY = F1.KEY THEN BEGIN
            GOSUB SCREEN.HELP
            STRING.DATA$ = SCREEN.NUMBER$                             
            INTEGER.DATA% = 0
            RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
            GOSUB REPORT.DISPLAY
         ENDIF      

      
         IF (SCR.EXITKEY = F7.KEY) OR (SCR.EXITKEY = PGUP.KEY) OR \
            (SCR.EXITKEY = F8.KEY) OR (SCR.EXITKEY = PGDN.KEY)    \
            THEN GOSUB REPORT.PAGE
 
      
         IF (SCR.EXITKEY = ESC.KEY) THEN GOTO EXIT.RFS0200

         IF (SCR.EXITKEY = F3.KEY) THEN BEGIN
            SCREEN.EXIT% = TRUE
         ENDIF


      WEND 

RETURN

\****************************************************************************
\***
\***   REPORT PAGE
\***
\***   Display page of report
\***
\****************************************************************************
REPORT.PAGE:
      
       IF (SCR.EXITKEY = F8.KEY OR SCR.EXITKEY = PGDN.KEY) THEN BEGIN
          REPORT.START = REPORT.START + 54
          IF REPORT.START > NO.OF.LINES% THEN BEGIN
             REPORT.START = REPORT.START - 54 
          ENDIF
       ENDIF   

       IF (SCR.EXITKEY = F7.KEY OR SCR.EXITKEY = PGUP.KEY) THEN BEGIN
          REPORT.START = REPORT.START - 54
          IF REPORT.START < 1 THEN BEGIN
             REPORT.START = REPORT.START + 54
          ENDIF
       ENDIF

       GOSUB OUTPUT.REPORT

RETURN
              
\****************************************************************************
\***
\***   REPORT PRINT
\***
\***   Format date and time
\***   Get store number
\***   Extract report from file
\***   Print report
\***
\****************************************************************************
REPORT.PRINT:
                                  
     NAME$ = REPORT$(LIST.ITEM%)
     LN$   = " "                                                        ! FRG
     TODAYS.DATE$ = DATE$                                                
     REPORT.DATE$ = RIGHT$(TODAYS.DATE$,2) + "/" + MID$(TODAYS.DATE$,3,2) + \
        "/" + LEFT$(TODAYS.DATE$,2)                                      
     REPORT.TIME$ = LEFT$(TIME$,2) + ":" + MID$(TIME$,3,2)  

     NEW.LINE$ = """" + CHR$(13) + CHR$(10) + """"  
     PAGE% = 1

     ADX.FUNCTION% = 4                                                 
     CALL ADXSERVE (ADX.RETURN.CODE%,ADX.FUNCTION%,0,ADXSERVE.DATA$)   
     IF ADX.RETURN.CODE% <> 0 THEN GOSUB ADXSERVE.FAILURE              
                                                                                                                                    
     STORE.NUMBER$ = LEFT$(ADXSERVE.DATA$,4)    
      
     WORKFILE.FILE.NAME$ = "D:\ADX_UDT1\RFREPORT.BIN" 

     CURR.SESS.NUM% = WORKFILE.SESS.NUM%                         
     IF END #WORKFILE.SESS.NUM% THEN CREATE.ERROR                       
     CREATE POSFILE WORKFILE.FILE.NAME$ AS WORKFILE.SESS.NUM% LOCAL    


     CURR.SESS.NUM% = WKFILE2.SESS.NUM%
     IF END #WKFILE2.SESS.NUM% THEN END.OF.WKFILE2
     OPEN REPORT.FILE.NAME$ AS WKFILE2.SESS.NUM% NOWRITE NODEL
                    
     NO.OF.LINES% = 1                           

     GOSUB PROCESS.REPORT                                               ! FRG

     ! Below logic will be taken care in PROCESS.REPORT Sub routine     ! FRG
     ! also added code to fix Duplicity issue while printing GAP        ! FRG
     ! Archival reports                                                 ! FRG
     ! DIM REPORT$(ARRAY.SIZE%)                                         ! FRG

     !EOF = FALSE                                                       ! FRG
     !WHILE EOF = FALSE                                                 ! FRG

     !    READ #WKFILE2.SESS.NUM% ; LN$                                 ! FRG
     !    IF LN$ = "0M" + NAME$ THEN BEGIN                              ! FRG
     !       REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)                     ! FRG
     !       NO.OF.LINES% = NO.OF.LINES% + 1                            ! FRG
     !       LN$ = "  "                                                 ! FRG
     !       WHILE LEFT$(LN$,2) <> "0M"                                 ! FRG
     !          READ #WKFILE2.SESS.NUM%;LN$                             ! FRG
     !          IF LEFT$(LN$,2) <> "0M" THEN BEGIN                      ! FRG
     !             REPORT$(NO.OF.LINES%) = RIGHT$(LN$,20)               ! FRG
     !             NO.OF.LINES% = NO.OF.LINES% + 1                      ! FRG
     !          ENDIF                                                   ! FRG
     !       WEND                                                       ! FRG
     !    ENDIF                                                         ! FRG

! END.READ.REPORT:                                                      ! FRG

!     WEND                                                              ! FRG
   
    
     PAGES% = (NO.OF.LINES% / 150) + 1

     FOR X% = 1 TO NO.OF.LINES% STEP 150
        GOSUB OUTPUT.HEADER
        FOR Y% = 0 TO 49
           REPORT.LINE$ = REPORT$(X% + Y%) + STRING$(30," ") +  \
           REPORT$(X% + Y% + 50) + STRING$(30," ") + REPORT$(X% + Y% + 100)
           PRINT #WORKFILE.SESS.NUM%; REPORT.LINE$
        NEXT Y%
        PAGE% = PAGE% + 1
        PRINT #WORKFILE.SESS.NUM%;NEW.LINE$
        
        IF PAGE% > PAGES% THEN BEGIN
           REPORT.LINE$ = STRING$(50," ") + "*** END OF REPORT ***"                                       
           PRINT #WORKFILE.SESS.NUM%;REPORT.LINE$
        ENDIF
        PRINT #WORKFILE.SESS.NUM%;CHR$(12)
     NEXT X%

     CURRENT.REPORT.NUM% = PRINT.REPORT.NUM%
     CURR.SESS.NUM% = PRINT.SESS.NUM%                                
     IF END #PRINT.SESS.NUM% THEN OPEN.ERROR
     OPEN PRINT.FILE.NAME$ AS PRINT.SESS.NUM% 

     RC% = PRINT.REPORT(WORKFILE.SESS.NUM%, "Y")
                       
     CLOSE PRINT.SESS.NUM%
     CLOSE WORKFILE.SESS.NUM%
     CLOSE WKFILE2.SESS.NUM%

     DIM REPORT$(ARRAY.SIZE%)
                                      
RETURN

!END.OF.WKFILE2:                                                        ! FRG
!       EOF = TRUE                                                      ! FRG 
!       GOTO END.READ.REPORT                                            ! FRG   
 

\****************************************************************************
\***
\***   OUTPUT HEADER
\***
\****************************************************************************

OUTPUT.HEADER:
                        
      REPORT.LINE$ = "STORE: " + STORE.NUMBER$ + STRING$(29, " ") +    \
                     + "旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴커" \ 
                     + STRING$(38," ") + "Page " +                     \ 
                     STR$(PAGE%) + " of " + STR$(PAGES%)            

       PRINT #WORKFILE.SESS.NUM%;REPORT.LINE$                             

       IF REPORT.FILE.NAME$ = GAP.FILE.NAME$ THEN BEGIN                 ! FRG
           NAME$= REPORT$(1)                                            ! FRG
       ENDIF                                                            ! FRG

       REPORT.LINE$ =   STRING$(40, " ") +                              \ 
         "� " + RIGHT$(TITLE$,20) + " - " + NAME$ +                     \ 
         + "�" + STRING$(41," ") + REPORT.DATE$ + "  " +                \ 
         REPORT.TIME$                                                   
     
       PRINT #WORKFILE.SESS.NUM%;REPORT.LINE$                          
      
       REPORT.LINE$ = STRING$(40," ") +                                 \ 
          "읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴켸"  
       PRINT #WORKFILE.SESS.NUM%;REPORT.LINE$                            

       PRINT #WORKFILE.SESS.NUM%;NEW.LINE$    
                    
RETURN

!****************************************************************************
!   Move up and down individual items in the list
!****************************************************************************
   
CHOOSE.ITEM:
    
       STRING.DATA$ = "3303333333003111"
       INTEGER.DATA% = 0
       GOSUB X.DM.SETF
       
       STRING.DATA$ = LEFT$(REPORT$(LIST.ITEM%),20)
       INTEGER.DATA% = 0
       GOSUB X.DM.PUTF
          
       IF (SCR.EXITKEY = UP.KEY) OR      \
          (SCR.EXITKEY = SHIFT.TAB) THEN BEGIN
             DISPLAY.ITEM% = DISPLAY.ITEM% - 1
             LIST.ITEM% = LIST.ITEM% - 1
          IF DISPLAY.ITEM% = 0 THEN BEGIN
             IF LIST.ITEM% > 0 THEN BEGIN
                DISPLAY.ITEM% = 12
                GOSUB DISPLAY.TABLE
             ENDIF ELSE BEGIN
                DISPLAY.ITEM% = 1
                LIST.ITEM% = 1
             ENDIF
          ENDIF
       ENDIF
                               

       IF ((SCR.EXITKEY = DOWN.KEY) OR  \
          (SCR.EXITKEY = TAB.KEY)) AND (LIST.ITEM% < NO.REPORTS%) THEN BEGIN
             DISPLAY.ITEM% = DISPLAY.ITEM% + 1
             LIST.ITEM% = LIST.ITEM% + 1
          IF DISPLAY.ITEM% > 12 THEN BEGIN
                DISPLAY.ITEM% = 1
                GOSUB DISPLAY.TABLE
          ENDIF
       ENDIF               

       STRING.DATA$ = ""
       INTEGER.DATA% = 10+(3*DISPLAY.ITEM%)
       GOSUB X.DM.POSF       


RETURN  


\* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *
\*                                                                   *
\*     DISPLAY SCREEN 1                                              *
\*                                                                   *
\*     IF  last exit key <> F1 (help)  THEN                          *
\*         Output screen 1                                           *
\*     ENDIF                                                         *
\*                                                                   *
\*                                                                   *
\*     RETURN                                                        *
\*                                                                   *
\* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *

DISPLAY.SCREEN1:

        SCREEN.NUMBER$ = MID$(MODULE.NUMBER$,3,3) + "01"
        IF  SCR.EXITKEY <> F1.KEY  THEN BEGIN                               
            STRING.DATA$ = SCREEN.NUMBER$                             
            INTEGER.DATA% = 0
            RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
        ENDIF
                                                   
                                                   
RETURN

\* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *
\*                                                                   *
\*     DISPLAY SCREEN 2                                              *
\*                                                                   *
\* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - *

DISPLAY.SCREEN2:

        SCREEN.NUMBER$ = MID$(MODULE.NUMBER$,3,3) + "02"

        STRING.DATA$ = SCREEN.NUMBER$                             
        INTEGER.DATA% = 0
        RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
                                                                                                     
RETURN

!****************************************************************************
!   F3 - Exit RFS0200
!   -----------------
!****************************************************************************

EXIT.RFS0200:
      
      SCREEN.EXIT% = TRUE
     
RETURN




!****************************************************************************
!   F7 & F8 - Move up or down a page
!   --------------------------------
!   If F7 is pressed then move up the list by 12 items.
!   If F8 is pressed then move down the list by 12 items.
!****************************************************************************

MOVE.PAGE:
 
       IF SCR.EXITKEY = F7.KEY OR SCR.EXITKEY = PGUP.KEY THEN BEGIN
          SCR.EXITKEY = UP.KEY
       ENDIF ELSE BEGIN
          SCR.EXITKEY = DOWN.KEY
       ENDIF

\*************************************************************************
\*** By using X% for the following loop - when a new page is displayed 
\*** by the Choose Item routine this loop will exit leaving the cursor
\*** at the top (for PGDN) or bottom (for PGUP) of the screen - which is  
\*** where we want it to be!!
\*************************************************************************



       FOR X% = 1 TO 12
           GOSUB CHOOSE.ITEM
                  
       NEXT X%


RETURN






\*****************************************************************************
\***
\***  SCREEN HELP
\***
\***  Call Display Manager help function
\***
\*****************************************************************************


SCREEN.HELP:

        GOSUB X.HELP
         

RETURN

\*********************************************************************
\*                                                                   *
\*     X.xxxxxxxxx:    Calls to PSB functions with return code check *
\*     ------------------------------------------------------------- *
\*                                                                   *
\*     If an error is found then the application will return to the  *
\*     calling program.                                              *
\*                                                                   *
\*     X.DM.OPNDIS     X.DM.INITDM     X.DM.CURS       X.DM.POSF     *
\*     X.DM.NEXTF      X.DM.UPDF       X.DM.DISPD      X.DM.SETF     *
\*     X.DM.RETF       X.DM.PUTF       X.DM.CLSDIS                   *
\*     X.EXTERNAL.MESSAGE      X.HELP                                *
\*                                                                   *
\*********************************************************************

X.DM.OPNDIS:

      RC% = DM.OPNDIS(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT      
      ENDIF
RETURN

X.DM.INITDM:
      RC% = DM.INITDM(STRING.DATA$, INTEGER.DATA%)               
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT      
      ENDIF
RETURN

X.DM.CURS:
      RC% = DM.CURS(STRING.DATA$, INTEGER.DATA%)                  
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT      
      ENDIF
RETURN

X.DM.POSF:
      RC% = DM.POSF(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT     
      ENDIF
RETURN
   
X.DM.NEXTF:
      RC% = DM.NEXTF(STRING.DATA$, INTEGER.DATA%)               
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT     
      ENDIF
RETURN

X.DM.UPDF:
      RC% = DM.UPDF(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT     
      ENDIF
RETURN

X.DM.DISPD:
      RC% = DM.DISPD(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT    
      ENDIF
RETURN

X.DM.SETF:
      RC% = DM.SETF(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT   
      ENDIF
RETURN

X.DM.PUTF:
      RC% = DM.PUTF(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT    
      ENDIF
RETURN

X.DM.CLSDIS:
      RC% = DM.CLSDIS(STRING.DATA$, INTEGER.DATA%)
      IF RC% = 1  THEN BEGIN
         GOTO PROGRAM.EXIT   
      ENDIF
RETURN

X.DM.RETF:
      RC% = DM.RETF(STRING.DATA$, INTEGER.DATA%)
        IF RC% = 1  THEN BEGIN
           GOTO PROGRAM.EXIT  
      ENDIF
RETURN
       
X.EXTERNAL.MESSAGE:
        RC% = EXTERNAL.MESSAGE(MESSAGE.NO%, STRING.DATA$, RETURN.FIELD%)
RETURN

X.HELP:
      RC% = HELP(SCREEN.NUMBER$)

RETURN


\*******************************************************************************
\***
\***    CALL.F20.SESS.NUM.UTILITY:
\***    References SESS.NUM.UTILITY (F20) to create, read, or delete entry on
\***    Session Number Table as determined by FUNCTION.FLAG$ ("O" "R" "C").
\***
\***............................................................................


CALL.F20.SESS.NUM.UTILITY:

       RC% = SESS.NUM.UTILITY(FUNCTION.FLAG$,PASSED.INTEGER%,PASSED.STRING$)

       IF RC% <> 0 THEN BEGIN
               SB.EVENT.NO% = 48
               SB.UNIQUE$ = RIGHT$(STRING$(10,"0") + STR$(F20.INTEGER.FILE.NO%),10)
               SB.MESSAGE$ = "SESSION NUMBER ALLOCATION ROUTINE FAILED"
               GOSUB SB.LOG.AN.EVENT
               GOTO PROGRAM.EXIT
       ENDIF

RETURN
                         
\****************************************************************************
\***
\***   DISPLAY.MESSAGE
\***
\***   If background task then show message on background else print message
\***
\****************************************************************************

DISPLAY.MESSAGE:

                                     
         PRINT ADXSERVE.DATA$                               
                 

RETURN
                                     

\*******************************************************************************
\***
\***    SB.LOG.AN.EVENT:
\***    Writes details of Event to application event log.
\***
\***............................................................................

SB.LOG.AN.EVENT:
   
       RC% = APPLICATION.LOG(MESSAGE.NO%,SB.UNIQUE$,UNIQUE.2$,SB.EVENT.NO%)
   
RETURN

\*******************************************************************************
\***
\***    LOG.AN.EVENT.106:
\***    Writes details of Event 106 to application event log.
\***
\***............................................................................


LOG.AN.EVENT.106:

    EVENT.NUMBER% = 106


    FUNCTION.FLAG$ = "R"
    PASSED.INTEGER% = CURR.SESS.NUM%
    PASSED.STRING$ = ""
    GOSUB CALL.F20.SESS.NUM.UTILITY

    CURRENT.REPORT.NUM% = F20.INTEGER.FILE.NO%


    VAR.STRING.1$ = \ ! Application event log data
      FILE.OPERATION$ + \
        CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) + \ ! Two byte integer byte order
        CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) + \ ! reversed to give hex number
        CURRENT.CODE.LOGGED$

    GOSUB CALL.F01.APPLICATION.LOG

RETURN

\*******************************************************************************
\***
\***    CALL.F01.APPLICATION.LOG:
\***    References APPLICATION.LOG (F01) to write details of event defined
\***    by EVENT.NUMBER% and VAR.STRING.1$ to Application Event Log.
\***
\***............................................................................


CALL.F01.APPLICATION.LOG:
                                                   
      RC% = APPLICATION.LOG(MESSAGE.NO%,                          \
                        VAR.STRING.1$,                            \
                        VAR.STRING.2$,                            \
                        EVENT.NUMBER%)


RETURN

\******************************************************************************
\***
\***   ADXSERVE FAILURE
\***
\***   LOG an event 106
\***   GOTO PROGRAM.EXIT
\***
\******************************************************************************                                                                       ! MNS
ADXSERVE.FAILURE:                                                      
                                                                       
    VAR.STRING.1$ = STR$(ADX.RETURN.CODE%) +                           \ 
                    PACK$(STR$(ADX.FUNCTION%))                         
    VAR.STRING.2$ = ""                                                 
    EVENT.NUMBER% = 23                                                 
    GOSUB SB.LOG.AN.EVENT                                              
    GOTO PROGRAM.EXIT                                                  

RETURN                                                                 
        

\******************************************************************************
\***
\***   CREATE ERROR:
\***
\***   LOG an event 106
\***   GOTO PROGRAM.EXIT
\***
\******************************************************************************

CREATE.ERROR:
   
       FILE.OPERATION$ = "C"
  
       GOSUB LOG.AN.EVENT.106 
   
       GOTO PROGRAM.EXIT

\******************************************************************************
\***
\***   READ.ERROR:
\***
\***   LOG an event 106
\***   GOTO PROGRAM.EXIT
\***
\******************************************************************************

READ.ERROR:
   
       FILE.OPERATION$ = "R"
  
       GOSUB LOG.AN.EVENT.106 
   
       GOTO PROGRAM.EXIT
  
\******************************************************************************
\***
\***   OPEN.ERROR:
\***
\***   LOG an event 106 
\***
\***   GOTO PROGRAM.EXIT
\***
\******************************************************************************

OPEN.ERROR:
      
      FILE.OPERATION$ = "O"

      GOSUB LOG.AN.EVENT.106

      GOTO PROGRAM.EXIT

!****************************************************************************
!   Display details of errors
!****************************************************************************
                                           
RENAME.ERROR:

       SB.EVENT.NO% = 26
       MESSAGE.NO% = 221
       SB.UNIQUE$ = "RFS02"
       UNIQUE.2$ = MESSAGE$ + SB.UNIQUE$
       GOSUB SB.LOG.AN.EVENT

GOTO PROGRAM.EXIT     
            


            

RFRDESC.ERROR:

       SB.EVENT.NO% = 26
       MESSAGE.NO% = 221
       SB.UNIQUE$ = "RFS02"
       UNIQUE.2$ = \
          "INTERNAL ERROR - Problem with RFRDESC file " + SB.UNIQUE$
       GOSUB SB.LOG.AN.EVENT

       GOTO PROGRAM.EXIT


\*******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***   References STANDARD.ERROR.DETECTED to log Event 101.
\***   Stops program.
\***
\***............................................................................


ERROR.DETECTED:
                                
       ERROR.COUNT% = ERROR.COUNT% + 1
    
       IF ERROR.COUNT% > 1 THEN BEGIN
          RESUME PROGRAM.EXIT
       ENDIF

       IF ERR = "OE" THEN BEGIN 
          IF CURR.SESS.NUM% = WORKFILE.SESS.NUM% THEN BEGIN
             ERROR.COUNT% = 0
             RESUME NO.DISPLAY.REPORTS
          ENDIF
       ENDIF                       


       RC% =                                         \
       STANDARD.ERROR.DETECTED                                         \
          (ERRN,                                                       \
           ERRF%,                                                      \
           ERRL,                                                       \
           ERR)
                       
       RC% = CONV.TO.HEX(ERRN)
       ADXSERVE.DATA$ = "ERROR: " + ERR + " " + F16.HEX.STRING$ 
       GOSUB DISPLAY.MESSAGE
 


       RESUME PROGRAM.EXIT

      STOP
               
      
   END
