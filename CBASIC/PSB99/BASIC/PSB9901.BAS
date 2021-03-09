\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSB9901.BAS  $
\***
\***   $Revision:   1.2  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/psb9901.bav  $
\***
\***      Rev 1.2   22 Jul 2004 15:33:56   devjsps
\***   Archived for MJB
\***   Changes for Beauty Commission project
\***   Operator Staff number and employee flag
\***   must now be entered
\***
\***      Rev 1.1   06 Oct 1994 10:21:48   DEVRCPS
\***   Initial PVCS revision. Corrected F3=ENTER error on leaving screens.
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***                                                                          *
\***            PROGRAM         :       PSB99                                 *
\***            AUTHOR          :       Mark Walker                           *
\***            DATE WRITTEN    :       March 23rd 1994                       *
\***                                                                          *
\***            MODULE          :       PSB9901                               *
\***                                                                          *
\***                                                                          *
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    Module Overview                                                       *
\***    ---------------                                                       *
\***                                                                          *
\***    This module handles the reporting of all operators. The report        *
\***    can be sorted by operator ID, name or model.                          *
\***                                                                          *
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    VERSION D.                 Mark Walker               23rd March 1994  *
\***    This module has been rewritten for the Controller Security package.   *
\***                                                                          *
\***    REVISION 1.1            ROBERT COWEY.                     26 SEP 1994.*
\***    Removed version letters from included code (not commented).           *
\***                                                                          *
\***    Removed OPEN, WRITE, TCLOSE and CLOSE of printer, along with          *
\***    associated checking of printer status.                                *
\***                                                                          *
\***    REVISION 1.2            MIKE BISHOP                       13 JUL 2004.*
\***    Additional fields staff number and employee flag added.               *
\***                                                                          *
\***    REVISION 1.3.           Nalini Mathusoothanan             01 AUG 2011.*
\***    Changes for Core 2 Release project.                                   *
\***       - To enforce the Staff Number to be entered for all new users.     *
\***       - Make the Staff Number field mandatory for Boots Employees only.  *
\***       - Display error message when Staff Number has not been entered.    *
\***    Change to use PRINT.REPORT function, which handles YLBP printers      *
\***                                                                          *
\***    REVISION 1.4.           Tittoo Thomas                     05 JUL 2012.*
\***    Changes for SFA, modified Controller and Till report headers to       *
\***    accomodate for more flags to be added. [SFA Defect 46]                *
\***                                                                          *
\***    REVISION A              David Griffiths                   15 Feb 2017 *
\***    Change to call of print report to not send compresses as this causes  *
\***    print out to stagger diagonally.                                      *
\***                                                                          *
\******************************************************************************
\******************************************************************************

        %INCLUDE AFDEC.J86
        %INCLUDE PRINTDEC.J86
        %INCLUDE PSBF03G.J86   ! Display Manager
        %INCLUDE PSBUSEG.J86

        STRING GLOBAL           BATCH.SCREEN.FLAG$,                    \
                                CHAIN.TO.PROG$,                        \
                                CURSOR.STATE$,                         \
                                INITIAL.DATA$,                         \
                                INVISIBLE$,                            \
                                SAVED.STRING$,                         \
                                LAST.MESSAGE$,                         \
                                MODULE.NUMBER$,                        \
                                OPERATOR.NUMBER$,                      \
                                SB.ACTION$,                            \
                                SB.FILE.NAME$,                         \
                                SB.STRING$,                            \
                                STRING.DATA$,                          \
                                VAR.STRING.1$,                         \
                                VAR.STRING.2$,                         \
                                VISIBLE$,                              \
                                DISPLAY.MESSAGE.TEXT$,                 \
                                CURSOR.ON$,                            \
                                CURSOR.OFF$,                           \
                                CHAR$,                                 \
                                FILE.OPERATION$,                       \
                                FILE.NO$,                              \
                                CURRENT.CODE$,                         \
                                WORKFILE.FILE.NAME$                    ! 1.3 NM

        INTEGER*1 GLOBAL        EVENT.NO%,                             \
                                TRUE,FALSE,                            \
                                EXIT.KEY.PRESSED(1)

        INTEGER*2 GLOBAL        CURRENT.REPORT.NUM%,                   \
                                SB.INTEGER%,                           \
                                SB.FILE.REP.NUM%,                      \
                                SB.FILE.SESS.NUM%,                     \
                                CURSOR.POSITION%,                      \
                                DISPLAY.MESSAGE.NUMBER%,               \
                                INTEGER.DATA%,                         \
                                RETURN.FIELD%,                         \
                                I%,                                    \
                                J%,                                    \
                                S%,                                    \
                                FUNCTION.KEY%,                         \
                                KEY.SELECTED%,                         \
                                MESSAGE.NO%,                           \
                                OLD.POSITION%,                         \
                                SAVED.POSITION%,                       \
                                HELP.KEY%,                             \
                                QUIT.KEY%,                             \
                                END.KEY%,                              \
                                HOME.KEY%,                             \
                                TAB.KEY%,                              \
                                BTAB.KEY%,                             \
                                ESC.KEY%,                              \
                                ENTER.KEY%,                            \
                                PGUP.KEY%,                             \
                                PGDN.KEY%,                             \
                                F7UP.KEY%,                             \
                                F8DN.KEY%,                             \
                                INVISIBLE.FIELD%,                      \
                                WORKFILE.REPORT.NUM%,                  \ 1.3 NM
                                WORKFILE.SESS.NUM%                     ! 1.3 NM

        STRING GLOBAL           AF.TABLE$(1),                          \
                                AF.FORMAT$,                            \
                                AF.BLOCK$,                             \
                                AF.RECORD$,                            \
                                SECTOR.STRING$,                        \
                                REPORT.OPTION$,                        \
                                SORT.OPTION$,                          \
                                TILL.MODEL.RECORD$(1),                 \
                                CTLR.MODEL.RECORD$(1),                 \
                                OPERATOR.ID$,                          \
                                OPERATOR.NAME$,                        \
                                STAFF.NO$,                             \
                                EMPLOYEE.FLG$,                         \
                                TILL.MODEL.STRING$,                    \
                                CTLR.MODEL.STRING$,                    \
                                DISPLAY.TITLE$,                        \
                                DISPLAY.PAGES$,                        \
\                                DISPLAY.HEADINGS$,                    \ 1.4 TT
                                DISPLAY.HEADINGS1$,                    \ 1.4 TT
                                DISPLAY.HEADINGS2$,                    \ 1.4 TT
                                DISPLAY.HEADINGS3$,                    \ 1.4 TT
                                DISPLAY.UNDERLINE$,                    \
                                REPORT.DATE$,                          \
                                REPORT.TIME$,                          \
\                                TILL.HEADINGS$,                       \ 1.4 TT
                                TILL.HEADINGS1$,                       \ 1.4 TT
                                TILL.HEADINGS2$,                       \ 1.4 TT
                                TILL.HEADINGS3$,                       \ 1.4 TT
\                                CTLR.HEADINGS$,                       \ 1.4 TT
                                CTLR.HEADINGS1$,                       \ 1.4 TT
                                CTLR.HEADINGS2$,                       \ 1.4 TT
                                CTLR.HEADINGS3$,                       \ 1.4 TT
                                FORM.FEED$,                            \
                                LINE.FEED$,                            \
                                CARRIAGE.RETURN$,                      \
                                PAD.CHAR$

                                !CONDENSE.ON$,                         \ 1.3 NM
                                !CONDENSE.OFF$,                        \ 1.3 NM

        INTEGER*1 GLOBAL        EMPTY.AF.RECORD.FOUND,                 \
                                END.OF.AF.FILE,                        \
                                END.OF.AF.BLOCK,                       \
                                VALID.REPORT.OPTION.FOUND,             \
                                VALID.SORT.OPTION.FOUND

        INTEGER*2 GLOBAL        AF.BLOCK.SIZE%,                        \
                                FULL.AF.BLOCKS%,                       \
                                MAX.TABLE.SIZE%,                       \
                                AF.PTR%,                               \
                                REMAINING.AF.BYTES%,                   \
                                AF.BLOCK.NUM%,                         \
                                AF.RECS.PER.SECTOR%,                   \
                                SECTOR.NUM%,                           \
                                RECORD.COUNT%,                         \
                                BASE%,                                 \
                                TILL.INDEX%,                           \
                                CTLR.INDEX%,                           \
                                S7.REPORT.OPTION%,                     \
                                S7.SORT.OPTION%,                       \
                                MAX.TILL.PTR%,                         \
                                MAX.CTLR.PTR%,                         \
                                NO.OF.PAGES%,                          \
                                LINES.PER.PAGE%,                       \
                                PAGE.NUMBER%,                          \
                                LINE.NUMBER%,                          \
                                FIRST.PTR%,                            \
                                LAST.PTR%

        INTEGER*4 GLOBAL        DATA.IN.AF%,                           \
                                TILL.MODEL.FLAGS%,                     \
                                CTLR.MODEL.FLAGS%,                     \
                                BIT.MASK%

        %INCLUDE AFEXT.J86
        %INCLUDE PRINTEXT.J86

        %INCLUDE PSBF01E.J86   ! Application Log
        %INCLUDE PSBF03E.J86   ! Display Manager
        %INCLUDE PSBF04E.J86   ! External Message
        %INCLUDE PSBF12E.J86   ! Help
        %INCLUDE PSBF24E.J86   ! Standard Error Detected
        %INCLUDE PSBF08E.J86   ! Print Report Functions                ! 1.3 NM

        %INCLUDE DMEXTR.J86

        %INCLUDE CSORTDEF.J86   ! Assembler Sort

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       DISPLAY.SCREEN                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Displays the required screen                                          *
\***                                                                          *
\******************************************************************************

        FUNCTION DISPLAY.SCREEN(SCREEN.NUMBER%) EXTERNAL

        INTEGER*2                               SCREEN.NUMBER%

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       SCREEN.HELP                                   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Displays the required HELP screen                                     *
\***                                                                          *
\******************************************************************************

        FUNCTION SCREEN.HELP(SCREEN.NUMBER%) EXTERNAL

        STRING                                  SCREEN.NUMBER$

        INTEGER*2                               SCREEN.NUMBER%

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.REPORT.OPTION                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the report option is valid                                *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.REPORT.OPTION PUBLIC

        INTEGER*1                               VALID.REPORT.OPTION

        VALID.REPORT.OPTION = FALSE

        IF REPORT.OPTION$ >= "1" AND                                   \
           REPORT.OPTION$ <= "3" THEN                                  \
           VALID.REPORT.OPTION = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.SORT.OPTION                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the sort option is valid                                  *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.SORT.OPTION PUBLIC

        INTEGER*1                               VALID.SORT.OPTION

        VALID.SORT.OPTION = FALSE

        IF SORT.OPTION$ >= "1" AND                                     \
           SORT.OPTION$ <= "2" THEN                                    \
           VALID.SORT.OPTION = TRUE

        END FUNCTION

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  M A I N L I N E  C O D E                              *
\***                                                                          *
\******************************************************************************
\******************************************************************************

        SUB PSB9901 PUBLIC

        ON ERROR GOTO ERROR.DETECTED

        CARRIAGE.RETURN$ = CHR$(13)
        LINE.FEED$       = CHR$(10)
        FORM.FEED$       = CHR$(12)
        !CONDENSE.ON$     = CHR$(15)                                  ! 1.3 NM
        !CONDENSE.OFF$    = CHR$(18)                                  ! 1.3 NM
        PAD.CHAR$        = CHR$(32)

        MAX.TABLE.SIZE%     = 1000                                      ! DMW
        AF.RECS.PER.SECTOR% = 512 / AF.RECL%
        AF.BLOCK.SIZE%      = 32256
        AF.PTR%             = 0

        S% = 7

        CALL DISPLAY.SCREEN(7)

        SORT.OPTION$   = "1"
        REPORT.OPTION$ = "1"

        GOSUB RESTORE.FIELDS.07

        EXIT.KEY.PRESSED(7) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(7)
              GOSUB GET.SORT.OPTION.07
        WEND

        EXIT SUB

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  S U B R O U T I N E S                                 *
\***                                                                          *
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.SORT.OPTION.07                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the sort option from the report operators menu      *
\***                                                                          *
\******************************************************************************

        GET.SORT.OPTION.07:

        CURSOR.POSITION% = S7.SORT.OPTION%
        GOSUB PUT.CURSOR.IN.FIELD

        EXIT.KEY.PRESSED(7)     = FALSE
        VALID.SORT.OPTION.FOUND = FALSE

        GOSUB GET.INPUT

        WHILE NOT (EXIT.KEY.PRESSED(7) OR                              \
                  VALID.SORT.OPTION.FOUND)

              SORT.OPTION$ = F03.RETURNED.STRING$

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 GOSUB DISPLAY.MESSAGE
                 GOSUB RESUME.INPUT
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
                 EXIT.KEY.PRESSED(7) = TRUE                            \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 GOSUB CHAIN.TO.CALLER
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(7)
                 GOSUB RESTORE.FIELDS.07

                 CURSOR.POSITION% = S7.SORT.OPTION%
                 GOSUB PUT.CURSOR.IN.FIELD

                 GOSUB GET.INPUT
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.SORT.OPTION THEN                             \
                    VALID.SORT.OPTION.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B003 Invalid selection number
                    DISPLAY.MESSAGE.NUMBER% = 3
                    GOSUB DISPLAY.MESSAGE
                    GOSUB RESUME.INPUT
                 ENDIF
              ENDIF
        WEND

        IF VALID.SORT.OPTION.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(7) THEN                                \
        BEGIN
           GOSUB CLEAR.MESSAGE

           STRING.DATA$ = SORT.OPTION$
           GOSUB SET.FIELD

           IF FUNCTION.KEY% = TAB.KEY% OR                              \
              FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              GOSUB GET.REPORT.OPTION.07
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.07
           ENDIF

        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.REPORT.OPTION.07                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the report option from the report operators menu    *
\***                                                                          *
\******************************************************************************

        GET.REPORT.OPTION.07:

        CURSOR.POSITION% = S7.REPORT.OPTION%
        GOSUB PUT.CURSOR.IN.FIELD

        EXIT.KEY.PRESSED(7)       = FALSE
        VALID.REPORT.OPTION.FOUND = FALSE

        GOSUB GET.INPUT

        WHILE NOT (EXIT.KEY.PRESSED(7) OR                              \
                  VALID.REPORT.OPTION.FOUND)

              REPORT.OPTION$ = F03.RETURNED.STRING$

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 GOSUB DISPLAY.MESSAGE
                 GOSUB RESUME.INPUT
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
                 EXIT.KEY.PRESSED(7) = TRUE                            \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 GOSUB CHAIN.TO.CALLER
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(7)
                 GOSUB RESTORE.FIELDS.07

                 CURSOR.POSITION% = S7.REPORT.OPTION%
                 GOSUB PUT.CURSOR.IN.FIELD

                 GOSUB GET.INPUT
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.REPORT.OPTION THEN                           \
                    VALID.REPORT.OPTION.FOUND = TRUE                   \
                 ELSE                                                  \
                 BEGIN
                    ! B003 Invalid selection number
                    DISPLAY.MESSAGE.NUMBER% = 3
                    GOSUB DISPLAY.MESSAGE
                    GOSUB RESUME.INPUT
                 ENDIF
              ENDIF
        WEND

        IF VALID.REPORT.OPTION.FOUND AND                               \
           NOT EXIT.KEY.PRESSED(7) THEN                                \
        BEGIN
           GOSUB CLEAR.MESSAGE

           STRING.DATA$ = REPORT.OPTION$
           GOSUB SET.FIELD

           IF FUNCTION.KEY% = BTAB.KEY% OR                             \
              FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              GOSUB GET.SORT.OPTION.07
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.07
           ENDIF

        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.07                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields on the report operators screen                   *
\***                                                                          *
\******************************************************************************

        RESTORE.FIELDS.07:

        CURSOR.STATE$ = CURSOR.OFF$
        GOSUB SET.CURSOR.STATE

        CURSOR.POSITION% = S7.SORT.OPTION%
        GOSUB PUT.CURSOR.IN.FIELD

        STRING.DATA$ = SORT.OPTION$
        GOSUB SET.FIELD

        CURSOR.POSITION% = S7.REPORT.OPTION%
        GOSUB PUT.CURSOR.IN.FIELD

        STRING.DATA$ = REPORT.OPTION$
        GOSUB SET.FIELD

        CURSOR.STATE$ = CURSOR.ON$
        GOSUB SET.CURSOR.STATE

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHECK.FIELDS.07                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Validate all input fields for the report operators screen             *
\***                                                                          *
\******************************************************************************

        CHECK.FIELDS.07:

        IF NOT VALID.REPORT.OPTION THEN                                \
        BEGIN
           DISPLAY.MESSAGE.NUMBER% = 3
           GOSUB DISPLAY.MESSAGE

           GOSUB GET.REPORT.OPTION.07

           GOTO CHECK.FIELDS.07.FAILED
        ENDIF

        IF NOT VALID.SORT.OPTION THEN                                  \
        BEGIN
           DISPLAY.MESSAGE.NUMBER% = 3
           GOSUB DISPLAY.MESSAGE

           GOSUB GET.SORT.OPTION.07

           GOTO CHECK.FIELDS.07.FAILED
        ENDIF

        GOSUB PRODUCE.OPERATORS.REPORT

        CHECK.FIELDS.07.FAILED:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PRODUCE.OPERATORS.REPORT                      *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Produce a report of all operators                                     *
\***                                                                          *
\******************************************************************************

        PRODUCE.OPERATORS.REPORT:

        DISPLAY.MESSAGE.NUMBER% = 409
        GOSUB DISPLAY.MESSAGE

        AF.PTR% = 0

        GOSUB CREATE.AF.TABLE

        IF SORT.OPTION$ = "1" THEN                                     \
        BEGIN
!           DISPLAY.HEADINGS$  = "OP ID    NAME                 "      ! 1.4 TT

           DISPLAY.HEADINGS1$  = "OPERATOR                      "      ! 1.4 TT

           DISPLAY.HEADINGS2$  = "   ID            NAME         "      ! 1.4 TT

           DISPLAY.HEADINGS3$  = "                              "      ! 1.4 TT

           DISPLAY.UNDERLINE$  = "-------- -------------------- "
        ENDIF                                                          \
        ELSE                                                           \
        IF SORT.OPTION$ = "2" THEN                                     \
        BEGIN
!           DISPLAY.HEADINGS$  = "NAME                 OP ID    "      ! 1.4 TT

!           DISPLAY.UNDERLINE$ = "-------------------- -------- "      ! 1.4 TT
           DISPLAY.HEADINGS1$  = "                     OPERATOR "      ! 1.4 TT

           DISPLAY.HEADINGS2$  = "        NAME            ID    "      ! 1.4 TT

           DISPLAY.HEADINGS3$  = "                              "      ! 1.4 TT

           DISPLAY.UNDERLINE$  = "-------------------- -------- "      ! 1.4 TT
        ENDIF

!       IF REPORT.OPTION$ = "1" OR REPORT.OPTION$ = "2" THEN BEGIN     ! 1.3 NM
!        DISPLAY.HEADINGS$  = DISPLAY.HEADINGS$ +  "STAFF NO "         ! 1.4 TT
!        DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ + "-------- "         ! 1.4 TT
        DISPLAY.HEADINGS1$  = DISPLAY.HEADINGS1$ +  " STAFF   "        ! 1.4 TT
        DISPLAY.HEADINGS2$  = DISPLAY.HEADINGS2$ +  "   NO    "        ! 1.4 TT
        DISPLAY.HEADINGS3$  = DISPLAY.HEADINGS3$ +  "         "        ! 1.4 TT
        DISPLAY.UNDERLINE$  = DISPLAY.UNDERLINE$ +  "-------- "        ! 1.4 TT
!       ENDIF                                                          ! 1.3 NM

!        DISPLAY.HEADINGS$  = DISPLAY.HEADINGS$ +  "BTC "              ! 1.4 TT
        DISPLAY.HEADINGS1$  = DISPLAY.HEADINGS1$ +  " B  "             ! 1.4 TT
        DISPLAY.HEADINGS2$  = DISPLAY.HEADINGS2$ +  " T  "             ! 1.4 TT
        DISPLAY.HEADINGS3$  = DISPLAY.HEADINGS3$ +  " C  "             ! 1.4 TT

!        DISPLAY.UNDERLINE$  = DISPLAY.UNDERLINE$ +  "--- "            ! 1.4 TT

        DISPLAY.UNDERLINE$  = DISPLAY.UNDERLINE$ +  " -  "

        IF REPORT.OPTION$ = "1" THEN                                   \
        BEGIN
           DISPLAY.TITLE$     = "CONTROLLER OPERATORS REPORT"

!           DISPLAY.HEADINGS$  = DISPLAY.HEADINGS$ +                    \ 1.4 TT
!                                CTLR.HEADINGS$                           1.4 TT

!           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \ 1.4 TT
!                                STRING$(MAX.CTLR.PTR%,"--- ")            1.4 TT

           DISPLAY.HEADINGS1$  = DISPLAY.HEADINGS1$ +                  \ 1.4 TT
                                 CTLR.HEADINGS1$                       ! 1.4 TT
           DISPLAY.HEADINGS2$  = DISPLAY.HEADINGS2$ +                  \ 1.4 TT
                                 CTLR.HEADINGS2$
           DISPLAY.HEADINGS3$  = DISPLAY.HEADINGS3$ +                  \ 1.4 TT
                                 CTLR.HEADINGS3$                       ! 1.4 TT

           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \ 1.4 TT
                                STRING$(MAX.CTLR.PTR%,"-  ")           ! 1.4 TT
        ENDIF                                                          \
        ELSE                                                           \
        IF REPORT.OPTION$ = "2" THEN                                   \
        BEGIN
           DISPLAY.TITLE$     = "   TILL OPERATORS REPORT   "

!           DISPLAY.HEADINGS$  = DISPLAY.HEADINGS$ +                    \ 1.4 TT
!                                TILL.HEADINGS$                         ! 1.4 TT

!           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \ 1.4 TT
!                                STRING$(MAX.TILL.PTR%,"--- ")          ! 1.4 TT

           DISPLAY.HEADINGS1$  = DISPLAY.HEADINGS1$ +                  \ 1.4 TT
                                 TILL.HEADINGS1$                       ! 1.4 TT

           DISPLAY.HEADINGS2$  = DISPLAY.HEADINGS2$ +                  \ 1.4 TT
                                 TILL.HEADINGS2$                       ! 1.4 TT

           DISPLAY.HEADINGS3$  = DISPLAY.HEADINGS3$ +                  \ 1.4 TT
                                 TILL.HEADINGS3$                       ! 1.4 TT

           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \ 1.4 TT
                                STRING$(MAX.TILL.PTR%,"-  ")           ! 1.4 TT
        ENDIF                                                          \
        ELSE                                                           \
        IF REPORT.OPTION$ = "3" THEN                                   \
        BEGIN
           REPORT.DATE$       = MID$(DATE$,5,2) + "/" +                \
                                MID$(DATE$,3,2) + "/" +                \
                                MID$(DATE$,1,2)

           REPORT.TIME$       = MID$(TIME$,1,2) + ":" +                \
                                MID$(TIME$,3,2)

           DISPLAY.TITLE$     = "R020" +                               \
                                STRING$(30,PAD.CHAR$) +                \
                                "OPERATORS REPORT" +                   \
                                STRING$(20,PAD.CHAR$) +                \
                                REPORT.DATE$ + "  " +                  \
                                REPORT.TIME$ +                         \
                                STRING$(10,PAD.CHAR$)

!           DISPLAY.HEADINGS$  = DISPLAY.HEADINGS$ +                    \ 1.4 TT
!                                TILL.HEADINGS$ +                       \ 1.4 TT
!                                CTLR.HEADINGS$                         ! 1.4 TT

           DISPLAY.HEADINGS1$  = DISPLAY.HEADINGS1$ +                  \  1.4 TT
                                 TILL.HEADINGS1$ +                     \  1.4 TT
                                 CTLR.HEADINGS1$                       !  1.4 TT

           DISPLAY.HEADINGS2$  = DISPLAY.HEADINGS2$ +                  \  1.4 TT
                                 TILL.HEADINGS2$ +                     \  1.4 TT
                                 CTLR.HEADINGS2$                       !  1.4 TT

           DISPLAY.HEADINGS3$  = DISPLAY.HEADINGS3$ +                  \  1.4 TT
                                 TILL.HEADINGS3$ +                     \  1.4 TT
                                 CTLR.HEADINGS3$                       !  1.4 TT

!           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \ 1.4 TT
!                                STRING$(MAX.TILL.PTR%,"--- ") +        \ 1.4 TT
!                                STRING$(MAX.CTLR.PTR%,"--- ")          ! 1.4 TT

           DISPLAY.UNDERLINE$ = DISPLAY.UNDERLINE$ +                   \  1.4 TT
                                STRING$(MAX.TILL.PTR%,"-  ") +        \   1.4 TT
                                STRING$(MAX.CTLR.PTR%,"-  ")           !  1.4 TT
        ENDIF

        IF REPORT.OPTION$ = "1" OR                                     \
           REPORT.OPTION$ = "2" THEN                                   \
        BEGIN
           GOSUB DISPLAY.REPORT
        ENDIF                                                          \
        ELSE                                                           \
        IF REPORT.OPTION$ = "3" THEN                                   \
        BEGIN
           GOSUB PRINT.OP.REPORT                                       ! 1.3 NM
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CREATE.AF.TABLE                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Produce a table containing all the records from the EALAUTH file      *
\***                                                                          *
\******************************************************************************

        CREATE.AF.TABLE:

        DIM AF.TABLE$(MAX.TABLE.SIZE%)

        CLOSE AF.SESS.NUM%

        CURRENT.REPORT.NUM% = AF.REPORT.NUM%
        IF END # AF.SESS.NUM% THEN OPEN.ERROR
        OPEN AF.FILE.NAME$ AS AF.SESS.NUM%                             \
             BUFFSIZE AF.BLOCK.SIZE%

        AF.FORMAT$ = "C512"

        IF END # AF.SESS.NUM% THEN FILE.ERROR
        READ FORM AF.FORMAT$; # AF.SESS.NUM%; AF.BLOCK$

        DATA.IN.AF%         = SIZE(AF.FILE.NAME$) - 512
        FULL.AF.BLOCKS%     = INT%(DATA.IN.AF% / AF.BLOCK.SIZE%)
        REMAINING.AF.BYTES% = MOD(DATA.IN.AF%,AF.BLOCK.SIZE%)

        IF FULL.AF.BLOCKS% = 0 AND                                     \
           REMAINING.AF.BYTES% > 0 THEN                                \
           AF.BLOCK.SIZE% = REMAINING.AF.BYTES%

        AF.FORMAT$ = "C" + STR$(AF.BLOCK.SIZE%)

        IF END # AF.SESS.NUM% THEN FILE.ERROR
        READ FORM AF.FORMAT$; # AF.SESS.NUM%; AF.BLOCK$

        AF.BLOCK.NUM% = 1

        END.OF.AF.FILE = FALSE

        WHILE NOT END.OF.AF.FILE AND                                   \
              AF.PTR% < MAX.TABLE.SIZE%

              GOSUB PROCESS.AF.BLOCK
        WEND

        CLOSE AF.SESS.NUM%

        IF END # AF.SESS.NUM% THEN OPEN.ERROR
        OPEN AF.FILE.NAME$ KEYED RECL AF.RECL% AS AF.SESS.NUM%

        IF AF.PTR% > 0 THEN                                            \
        BEGIN
           CALL CSORT(VARPTR(AF.TABLE$(0)),AF.PTR%)
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.AF.BLOCK                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Extract all the EALAUTH sectors from a block                          *
\***                                                                          *
\******************************************************************************

        PROCESS.AF.BLOCK:

        SECTOR.NUM% = 0

        END.OF.AF.BLOCK = FALSE

        WHILE NOT END.OF.AF.BLOCK
              GOSUB PROCESS.AF.SECTOR
        WEND

        IF AF.BLOCK.NUM% = FULL.AF.BLOCKS% AND                         \
           REMAINING.AF.BYTES% > 0 THEN                                \
        BEGIN
           AF.BLOCK.SIZE% = REMAINING.AF.BYTES%
           AF.FORMAT$     = "C" + STR$(AF.BLOCK.SIZE%)
        ENDIF

        IF END # AF.SESS.NUM% THEN END.OF.AF.FOUND
        READ FORM AF.FORMAT$; # AF.SESS.NUM%; AF.BLOCK$

        AF.BLOCK.NUM% = AF.BLOCK.NUM% + 1

        GOTO END.PROCESS.AF.BLOCK

        END.OF.AF.FOUND:

        END.OF.AF.FILE = TRUE

        END.PROCESS.AF.BLOCK:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.AF.SECTOR                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Extract all the EALAUTH records from a sector                         *
\***                                                                          *
\******************************************************************************

        PROCESS.AF.SECTOR:

        SECTOR.STRING$ = MID$(AF.BLOCK$,                               \
                         (SECTOR.NUM% * 512) + 1,512)

        RECORD.COUNT% = 0

        EMPTY.AF.RECORD.FOUND = FALSE

        WHILE RECORD.COUNT% < AF.RECS.PER.SECTOR% AND                  \
              NOT EMPTY.AF.RECORD.FOUND AND                            \
              AF.PTR% < MAX.TABLE.SIZE%

              BASE% = (RECORD.COUNT% * AF.RECL%) + 5

              AF.RECORD$ = MID$(SECTOR.STRING$,                        \
                           BASE%,AF.RECL%)

              AF.OPERATOR.NO$ = MID$(AF.RECORD$,1,4)

              IF AF.OPERATOR.NO$ = PACK$("00000000") THEN              \
              BEGIN
                 EMPTY.AF.RECORD.FOUND = TRUE
                 GOTO NEXT.AF.SECTOR
              ENDIF

              GOSUB FORMAT.AF.RECORD

              RECORD.COUNT% = RECORD.COUNT% + 1

        NEXT.AF.SECTOR:

        WEND

        SECTOR.NUM% = SECTOR.NUM% + 1

        IF SECTOR.NUM% = (AF.BLOCK.SIZE% / 512) THEN                   \
           END.OF.AF.BLOCK = TRUE

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       FORMAT.AF.RECORD                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Format the current EALAUTH record for displaying on a report          *
\***                                                                          *
\******************************************************************************

        FORMAT.AF.RECORD:

        OPERATOR.ID$   = LEFT$(STR$(VAL(UNPACK$(                       \
                         MID$(AF.RECORD$,1,4)))) +                     \
                         STRING$(8," "),8)

        OPERATOR.NAME$ = UCASE$(MID$(AF.RECORD$,25,20))

        IF VAL(OPERATOR.ID$) > 999 THEN BEGIN
           STAFF.NO$ = STRING$(8," ")

           EMPLOYEE.FLG$ = " "
        ENDIF ELSE BEGIN
           STAFF.NO$ = UNPACK$(MID$(AF.RECORD$,61,4))

           IF UNPACK$(MID$(AF.RECORD$,65,1)) = "00" THEN BEGIN
              EMPLOYEE.FLG$ = "Y"
           ENDIF ELSE BEGIN
              EMPLOYEE.FLG$ = "N"
           ENDIF
        ENDIF

        TILL.MODEL.STRING$ = ""
!        TILL.HEADINGS$     = ""                                       ! 1.4 TT
        TILL.HEADINGS1$     = ""                                       ! 1.4 TT
        TILL.HEADINGS2$     = ""                                       ! 1.4 TT
        TILL.HEADINGS3$     = ""                                       ! 1.4 TT

        FOR I% = 1 TO MAX.TILL.PTR%

            TILL.INDEX% = VAL(UNPACK$(MID$(                            \
                          TILL.MODEL.RECORD$(I%),2,1)))

!            TILL.HEADINGS$ = TILL.HEADINGS$ +                          \ 1.4 TT
!                             MID$(TILL.MODEL.RECORD$(I%),23,3) + " "   ! 1.4 TT

            TILL.HEADINGS1$ = TILL.HEADINGS1$ +                          \ 1.4 TT
                             MID$(TILL.MODEL.RECORD$(I%),23,1) + "  "    ! 1.4 TT
            TILL.HEADINGS2$ = TILL.HEADINGS2$ +                          \ 1.4 TT
                             MID$(TILL.MODEL.RECORD$(I%),24,1) + "  "    ! 1.4 TT
            TILL.HEADINGS3$ = TILL.HEADINGS3$ +                          \ 1.4 TT
                             MID$(TILL.MODEL.RECORD$(I%),25,1) + "  "    ! 1.4 TT

            BIT.MASK% = 2 ^ (TILL.INDEX% - 1)

            TILL.MODEL.FLAGS% = (ASC(MID$(AF.RECORD$,74,1)) * 256) +   \
                                (ASC(MID$(AF.RECORD$,73,1)))

            IF (TILL.MODEL.FLAGS% AND BIT.MASK%) > 0 THEN              \
            BEGIN
!               TILL.MODEL.STRING$ = TILL.MODEL.STRING$ + "Y   "       ! 1.4 TT
               TILL.MODEL.STRING$ = TILL.MODEL.STRING$ + "Y  "         ! 1.4 TT
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
!               TILL.MODEL.STRING$ = TILL.MODEL.STRING$ + "-   "       ! 1.4 TT
               TILL.MODEL.STRING$ = TILL.MODEL.STRING$ + "-  "         ! 1.4 TT
            ENDIF

        NEXT I%

        CTLR.MODEL.STRING$ = ""
!        CTLR.HEADINGS$     = ""                                       ! 1.4 TT
        CTLR.HEADINGS1$     = ""                                       ! 1.4 TT
        CTLR.HEADINGS2$     = ""                                       ! 1.4 TT
        CTLR.HEADINGS3$     = ""                                       ! 1.4 TT

        FOR I% = 1 TO MAX.CTLR.PTR%

            CTLR.INDEX% = VAL(UNPACK$(MID$(                            \
                          CTLR.MODEL.RECORD$(I%),2,1)))

!            CTLR.HEADINGS$ = CTLR.HEADINGS$ +                          \ 1.4 TT
!                             MID$(CTLR.MODEL.RECORD$(I%),23,3) + " "   ! 1.4 TT

            CTLR.HEADINGS1$ = CTLR.HEADINGS1$ +                          \ 1.4 TT
                             MID$(CTLR.MODEL.RECORD$(I%),23,1) + "  "    ! 1.4 TT
            CTLR.HEADINGS2$ = CTLR.HEADINGS2$ +                          \ 1.4 TT
                             MID$(CTLR.MODEL.RECORD$(I%),24,1) + "  "    ! 1.4 TT
            CTLR.HEADINGS3$ = CTLR.HEADINGS3$ +                          \ 1.4 TT
                             MID$(CTLR.MODEL.RECORD$(I%),25,1) + "  "    ! 1.4 TT

            BIT.MASK% = 2 ^ (CTLR.INDEX% - 1)

            CTLR.MODEL.FLAGS% = (ASC(MID$(AF.RECORD$,76,1)) * 256) +   \
                                (ASC(MID$(AF.RECORD$,75,1)))

            IF (CTLR.MODEL.FLAGS% AND BIT.MASK%) > 0 THEN              \
            BEGIN
!               CTLR.MODEL.STRING$ = CTLR.MODEL.STRING$ + "Y   "       ! 1.4 TT
               CTLR.MODEL.STRING$ = CTLR.MODEL.STRING$ + "Y  "         ! 1.4 TT
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
!               CTLR.MODEL.STRING$ = CTLR.MODEL.STRING$ + "-   "       ! 1.4 TT
               CTLR.MODEL.STRING$ = CTLR.MODEL.STRING$ + "-  "         ! 1.4 TT
            ENDIF

        NEXT I%

        AF.PTR% = AF.PTR% + 1

        IF SORT.OPTION$ = "1" THEN                                     \
        BEGIN
           AF.TABLE$(AF.PTR%) = OPERATOR.ID$ + " " +                   \
                                OPERATOR.NAME$ + " "
        ENDIF                                                          \
        ELSE                                                           \
        IF SORT.OPTION$ = "2" THEN                                     \
        BEGIN
           AF.TABLE$(AF.PTR%) = OPERATOR.NAME$ + " " +                 \
                                OPERATOR.ID$ + " "
        ENDIF

!        IF REPORT.OPTION$ = "1" OR REPORT.OPTION$ = "2" THEN BEGIN    \ 1.3 NM

        IF STAFF.NO$ = STRING$(8,"0") THEN BEGIN
           STAFF.NO$ = STRING$(8," ")
        ENDIF
        AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                      \
                                STAFF.NO$ + "  "

!        ENDIF                                                         ! 1.3 NM

!        AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                      \ 1.4 TT
!                             EMPLOYEE.FLG$ + "   "                     ! 1.4 TT

        AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                      \ 1.4 TT
                             EMPLOYEE.FLG$ + "  "                      ! 1.4 TT

        IF REPORT.OPTION$ = "1" THEN                                   \
        BEGIN
           AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                   \
                                CTLR.MODEL.STRING$
        ENDIF                                                          \
        ELSE                                                           \
        IF REPORT.OPTION$ = "2" THEN                                   \
        BEGIN
           AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                   \
                                TILL.MODEL.STRING$
        ENDIF                                                          \
        ELSE                                                           \
        IF REPORT.OPTION$ = "3" THEN                                   \
        BEGIN
           AF.TABLE$(AF.PTR%) = AF.TABLE$(AF.PTR%) +                   \
                                TILL.MODEL.STRING$ +                   \
                                CTLR.MODEL.STRING$
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DISPLAY.REPORT                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display the operators report on the screen                            *
\***                                                                          *
\******************************************************************************

        DISPLAY.REPORT:

        S% = 8

        CALL DISPLAY.SCREEN(8)

        LINES.PER.PAGE% = 16

        NO.OF.PAGES% = AF.PTR% / LINES.PER.PAGE%

        IF MOD(AF.PTR%,LINES.PER.PAGE%) > 0 THEN                       \
        BEGIN
           NO.OF.PAGES% = NO.OF.PAGES% + 1
        ENDIF

        PAGE.NUMBER% = 1
        GOSUB SET.PAGE.POINTERS

        GOSUB RESTORE.FIELDS.08

        EXIT.KEY.PRESSED(8) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(8)
              GOSUB GET.PAGE.KEY.08
        WEND

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.PAGE.KEY.08                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the paging keys on the report operators screen      *
\***                                                                          *
\******************************************************************************

        GET.PAGE.KEY.08:

        EXIT.KEY.PRESSED(8)  = FALSE

        WHILE NOT EXIT.KEY.PRESSED(8)

              CURSOR.POSITION% = INVISIBLE.FIELD%
              GOSUB PUT.CURSOR.IN.FIELD

              GOSUB GET.INPUT

              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(8) = TRUE

                 CALL DISPLAY.SCREEN(7)
                 GOSUB RESTORE.FIELDS.07
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 GOSUB CHAIN.TO.CALLER
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(8)
                 GOSUB RESTORE.FIELDS.08
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = PGDN.KEY% OR                          \
                 FUNCTION.KEY% = F8DN.KEY% THEN                        \
              BEGIN
                 IF PAGE.NUMBER% = NO.OF.PAGES% THEN                   \
                 BEGIN
                    ! B075 There are no more pages to display
                    DISPLAY.MESSAGE.NUMBER% = 75
                    GOSUB DISPLAY.MESSAGE
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    PAGE.NUMBER% = PAGE.NUMBER% + 1
                    GOSUB SET.PAGE.POINTERS

                    GOSUB DISPLAY.REPORT.LINES
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = PGUP.KEY% OR                          \
                 FUNCTION.KEY% = F7UP.KEY% THEN                        \
              BEGIN
                 IF PAGE.NUMBER% = 1 THEN                              \
                 BEGIN
                    ! B074 There is no previous page to display
                    DISPLAY.MESSAGE.NUMBER% = 74
                    GOSUB DISPLAY.MESSAGE
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    PAGE.NUMBER% = PAGE.NUMBER% - 1
                    GOSUB SET.PAGE.POINTERS

                    GOSUB DISPLAY.REPORT.LINES
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 ! B001 Invalid key pressed
                 DISPLAY.MESSAGE.NUMBER% = 1
                 GOSUB DISPLAY.MESSAGE
              ENDIF
        WEND

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.08                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields on the screen report of all operators            *
\***                                                                          *
\******************************************************************************

        RESTORE.FIELDS.08:

        CURSOR.STATE$ = CURSOR.OFF$
        GOSUB SET.CURSOR.STATE

        LOCATE 1,25
        PRINT DISPLAY.TITLE$

!        LOCATE 4,2                                                    ! 1.4 TT
!        PRINT DISPLAY.HEADINGS$                                       ! 1.4 TT

        LOCATE 3,2                                                     ! 1.4 TT
        PRINT DISPLAY.HEADINGS1$                                       ! 1.4 TT
        LOCATE 4,2                                                     ! 1.4 TT
        PRINT DISPLAY.HEADINGS2$                                       ! 1.4 TT
        LOCATE 5,2                                                     ! 1.4 TT
        PRINT DISPLAY.HEADINGS3$                                       ! 1.4 TT

!        LOCATE 5,2                                                    ! 1.4 TT
        LOCATE 6,2                                                     ! 1.4 TT
        PRINT DISPLAY.UNDERLINE$

        GOSUB DISPLAY.REPORT.LINES

        CURSOR.STATE$ = CURSOR.ON$
        GOSUB SET.CURSOR.STATE

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DISPLAY.REPORT.LINES                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display the report lines for the screen report of all operators       *
\***                                                                          *
\******************************************************************************

        DISPLAY.REPORT.LINES:

        IF NO.OF.PAGES% <> 1 THEN                                      \
        BEGIN
           IF PAGE.NUMBER% = 1 THEN                                    \
           BEGIN
              CURSOR.POSITION% = 4
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = INVISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES

              CURSOR.POSITION% = 6
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = VISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES
           ENDIF                                                       \
           ELSE                                                        \
           IF PAGE.NUMBER% = NO.OF.PAGES% THEN                         \
           BEGIN
              CURSOR.POSITION% = 4
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = VISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES

              CURSOR.POSITION% = 6
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = INVISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES
           ENDIF                                                       \
           ELSE                                                        \
           BEGIN
              CURSOR.POSITION% = 4
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = VISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES

              CURSOR.POSITION% = 6
              GOSUB PUT.CURSOR.IN.FIELD

              STRING.DATA$ = VISIBLE$
              GOSUB SET.FIELD.ATTRIBUTES
           ENDIF
        ENDIF

        LOCATE 1,67
        PRINT DISPLAY.PAGES$

        LINE.NUMBER% = 1

        FOR I% = FIRST.PTR% TO LAST.PTR%

!            LOCATE LINE.NUMBER% + 5,2                                 ! 1.4 TT
            LOCATE LINE.NUMBER% + 6,2                                  ! 1.4 TT
            PRINT AF.TABLE$(I%)

            LINE.NUMBER% = LINE.NUMBER% + 1

        NEXT I%

        IF PAGE.NUMBER% = NO.OF.PAGES% AND                             \
           LINE.NUMBER% <= LINES.PER.PAGE% THEN                        \
        BEGIN
           FOR I% = LINE.NUMBER% TO LINES.PER.PAGE%

!               LOCATE LINE.NUMBER% + 5,2
               LOCATE LINE.NUMBER% + 6,2
               PRINT STRING$(78," ")

               LINE.NUMBER% = LINE.NUMBER% + 1

           NEXT I%
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.PAGE.POINTERS                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Calculate the first and last records to be displayed on a page        *
\***                                                                          *
\******************************************************************************

        SET.PAGE.POINTERS:

        FIRST.PTR% = 1 + (LINES.PER.PAGE% * (PAGE.NUMBER% - 1))

        LAST.PTR% = LINES.PER.PAGE% +                                  \
                    (LINES.PER.PAGE% * (PAGE.NUMBER% - 1))

        IF LAST.PTR% > AF.PTR% THEN                                    \
        BEGIN
           LAST.PTR% = AF.PTR%
        ENDIF

        DISPLAY.PAGES$ = LEFT$("Page " +                               \
                         STR$(PAGE.NUMBER%) +                          \
                         " of " +                                      \
                         STR$(NO.OF.PAGES%) +                          \
                         STRING$(13," "),13)

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PRINT.REPORT                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Produce a printed copy of the operators report                        *
\***                                                                          *
\******************************************************************************

        PRINT.OP.REPORT:                                               ! 1.3 NM

        !Commented as part of Core 2 Release                           ! 1.3 NM
        !CURRENT.REPORT.NUM% = PRINT.REPORT.NUM%                       ! 1.3 NM
        !IF END # PRINT.SESS.NUM% THEN FILE.ERROR                      ! 1.3 NM
        !OPEN PRINT.FILE.NAME$ AS PRINT.SESS.NUM%                      ! 1.3 NM

        CREATE POSFILE WORKFILE.FILE.NAME$ AS WORKFILE.SESS.NUM%       \ 1.3 NM
         LOCKED MIRRORED ATCLOSE                                       ! 1.3 NM

        LINES.PER.PAGE% = 50

        NO.OF.PAGES% = AF.PTR% / LINES.PER.PAGE%

        IF MOD(AF.PTR%,LINES.PER.PAGE%) > 0 THEN                       \
        BEGIN
           NO.OF.PAGES% = NO.OF.PAGES% + 1
        ENDIF

        FOR PAGE.NUMBER% = 1 TO NO.OF.PAGES%

            GOSUB SET.PAGE.POINTERS
            GOSUB PRINT.HEADING
            GOSUB PRINT.REPORT.LINES

            IF PAGE.NUMBER% <> NO.OF.PAGES% THEN                       \
            BEGIN

               !Commented as part of Core 2 Release                    ! 1.3 NM
               !PRINT.LINE$ = STRING$(134,PAD.CHAR$) +                 \ 1.3 NM
               !              FORM.FEED$ +                             \ 1.3 NM
               !              CARRIAGE.RETURN$ +                       \ 1.3 NM
               !              LINE.FEED$                               ! 1.3 NM
               !CALL WRITE.CONDENSED.PRINT                             ! 1.3 NM

               PRINT #WORKFILE.SESS.NUM%; FORM.FEED$                   ! 1.3 NM
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               FOR J% = 1 TO 2

                   !Commented as part of Core 2 Release                ! 1.3 NM
                   !PRINT.LINE$ = STRING$(135,PAD.CHAR$) +             \ 1.3 NM
                   !              CARRIAGE.RETURN$ +                   \ 1.3 NM
                   !              LINE.FEED$                           ! 1.3 NM
                   !CALL WRITE.CONDENSED.PRINT                         ! 1.3 NM

                   PRINT #WORKFILE.SESS.NUM%; ""                       ! 1.3 NM

               NEXT J%

               !Commented as part of Core 2 Release                    ! 1.3 NM
               !PRINT.LINE$ = LEFT$(STRING$(34,PAD.CHAR$) +            \ 1.3 NM
               !              "*** " +                                 \ 1.3 NM
               !              "END OF REPORT " +                       \ 1.3 NM
               !              "***" +                                  \ 1.3 NM
               !              STRING$(135,PAD.CHAR$),135) +            \ 1.3 NM
               !              CARRIAGE.RETURN$ +                       \ 1.3 NM
               !              LINE.FEED$                               ! 1.3 NM
               !CALL WRITE.CONDENSED.PRINT                             ! 1.3 NM

               PRINT #WORKFILE.SESS.NUM%; "*** END OF REPORT ***"      ! 1.3 NM
            ENDIF

        NEXT PAGE.NUMBER%

        !Commented as part of Core 2 Release                           ! 1.3 NM
        !PRINT.LINE$ = STRING$(135,PAD.CHAR$) +                        \ 1.3 NM
        !              CONDENSE.OFF$ +                                 \ 1.3 NM
        !              FORM.FEED$                                      ! 1.3 NM
        !CALL WRITE.CONDENSED.PRINT                                    ! 1.3 NM

        PRINT #WORKFILE.SESS.NUM%; FORM.FEED$                          ! 1.3 NM

        !CLOSE PRINT.SESS.NUM%                                         ! 1.3 NM
        CLOSE WORKFILE.SESS.NUM%                                       ! 1.3 NM

        OPEN PRINT.FILE.NAME$ AS PRINT.SESS.NUM% NOREAD NODEL          ! 1.3 NM
        OPEN WORKFILE.FILE.NAME$ AS WORKFILE.SESS.NUM% LOCKED          ! 1.3 NM
        CALL PRINT.REPORT(WORKFILE.SESS.NUM%,"N")                      ! ADG

        ! CALL PRINT.REPORT(WORKFILE.SESS.NUM%, "Y")                     ! 1.3 NM

        CLOSE WORKFILE.SESS.NUM%                                       ! 1.3 NM
        CLOSE PRINT.SESS.NUM%
        DISPLAY.MESSAGE.NUMBER% = 444
        GOSUB DISPLAY.MESSAGE

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PRINT.HEADING                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display the page heading on the printed report                        *
\***                                                                          *
\******************************************************************************

        PRINT.HEADING:

        !Commented as part of Core 2 Release                           ! 1.3 NM
        !PRINT.LINE$ = LEFT$(CONDENSE.ON$ + DISPLAY.TITLE$ +           \ 1.3 NM
        !              DISPLAY.PAGES$ +                                \ 1.3 NM
        !              STRING$(135,PAD.CHAR$),135) +                   \ 1.3 NM
        !              CARRIAGE.RETURN$ +                              \ 1.3 NM
        !              LINE.FEED$                                      ! 1.3 NM
        !CALL WRITE.CONDENSED.PRINT                                    ! 1.3 NM

        PRINT #WORKFILE.SESS.NUM%; (DISPLAY.TITLE$ + DISPLAY.PAGES$)   \ 1.3 NM

        FOR J% = 1 TO 2

            !Commented as part of Core 2 Release                       ! 1.3 NM
            !PRINT.LINE$ = STRING$(135,PAD.CHAR$) +                    \ 1.3 NM
            !              CARRIAGE.RETURN$ +                          \ 1.3 NM
            !              LINE.FEED$                                  ! 1.3 NM
            !CALL WRITE.CONDENSED.PRINT                                ! 1.3 NM

            PRINT #WORKFILE.SESS.NUM%; ""                              ! 1.3 NM
        NEXT J%

        !Commented as part of Core 2 Release                           ! 1.3 NM
        !PRINT.LINE$ = LEFT$(PAD.CHAR$ + DISPLAY.HEADINGS$ +           \ 1.3 NM
        !              STRING$(135,PAD.CHAR$),135) +                   \ 1.3 NM
        !              CARRIAGE.RETURN$ +                              \ 1.3 NM
        !              LINE.FEED$                                      ! 1.3 NM
        !CALL WRITE.CONDENSED.PRINT                                    ! 1.3 NM

!        PRINT #WORKFILE.SESS.NUM%; DISPLAY.HEADINGS$                   ! 1.3 NM
        PRINT #WORKFILE.SESS.NUM%; DISPLAY.HEADINGS1$                   ! 1.3 NM
        PRINT #WORKFILE.SESS.NUM%; DISPLAY.HEADINGS2$                   ! 1.3 NM
        PRINT #WORKFILE.SESS.NUM%; DISPLAY.HEADINGS3$                   ! 1.3 NM

        !Commented as part of Core 2 Release                           ! 1.3 NM
        !PRINT.LINE$ = LEFT$(PAD.CHAR$ + DISPLAY.UNDERLINE$ +          \ 1.3 NM
        !              STRING$(135,PAD.CHAR$),135) +                   \ 1.3 NM
        !              CARRIAGE.RETURN$ +                              \ 1.3 NM
        !              LINE.FEED$                                      ! 1.3 NM
        !CALL WRITE.CONDENSED.PRINT                                    ! 1.3 NM

        PRINT #WORKFILE.SESS.NUM%; DISPLAY.UNDERLINE$                  ! 1.3 NM

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PRINT.REPORT.LINES                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Print the report lines for the printed report of all operators        *
\***                                                                          *
\******************************************************************************

        PRINT.REPORT.LINES:

        FOR J% = FIRST.PTR% TO LAST.PTR%

            !Commented as part of Core 2 Release                       ! 1.3 NM
            !PRINT.LINE$ = LEFT$(PAD.CHAR$ + AF.TABLE$(J%) +           \ 1.3 NM
            !              STRING$(135,PAD.CHAR$),135) +               \ 1.3 NM
            !              CARRIAGE.RETURN$ +                          \ 1.3 NM
            !              LINE.FEED$                                  ! 1.3 NM
            !CALL WRITE.CONDENSED.PRINT                                ! 1.3 NM

            PRINT #WORKFILE.SESS.NUM%; AF.TABLE$(J%)                   ! 1.3 NM

        NEXT J%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       WAIT.MESSAGE                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display a wait message to the screen                                  *
\***                                                                          *
\******************************************************************************

        WAIT.MESSAGE:

        ! B251 Processing - Please Wait .....
        DISPLAY.MESSAGE.NUMBER% = 251
        GOSUB DISPLAY.MESSAGE

        CURSOR.POSITION% = INVISIBLE.FIELD%
        GOSUB PUT.CURSOR.IN.FIELD

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.INPUT                                     *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get input from the keyboard                                           *
\***                                                                          *
\******************************************************************************

        GET.INPUT:

        STRING.DATA$ = "3113333333311333"
        CALL SETF(STRING.DATA$)

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.UPDF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$ = "3303333333303333"
        CALL SETF(STRING.DATA$)

        FUNCTION.KEY% = F03.RETURNED.INTEGER%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESUME.INPUT                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get input from the keyboard after an interrupt                        *
\***                                                                          *
\******************************************************************************

        RESUME.INPUT:

        STRING.DATA$ = "3113333333311333"
        CALL SETF(STRING.DATA$)

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.RESF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$ = "3303333333303333"
        CALL SETF(STRING.DATA$)

        FUNCTION.KEY% = F03.RETURNED.INTEGER%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.CURSOR.STATE                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Shows or hides the cursor                                             *
\***                                                                          *
\******************************************************************************

        SET.CURSOR.STATE:

        STRING.DATA$  = CURSOR.STATE$
        INTEGER.DATA% = 0

        CALL DM.CURS(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.MESSAGE                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear the current BEMF error message                                  *
\***                                                                          *
\******************************************************************************

        CLEAR.MESSAGE:

        OLD.POSITION% = CURSOR.POSITION%

        CURSOR.POSITION% = 1
        GOSUB PUT.CURSOR.IN.FIELD

        STRING.DATA$ = ""
        GOSUB SET.FIELD

        CURSOR.POSITION% = OLD.POSITION%

        GOSUB PUT.CURSOR.IN.FIELD

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DISPLAY.MESSAGE                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display a new BEMF error message                                      *
\***                                                                          *
\******************************************************************************

        DISPLAY.MESSAGE:

        CURSOR.STATE$ = INVISIBLE$
        GOSUB SET.CURSOR.STATE

        STRING.DATA$ = ""
        OLD.POSITION% = CURSOR.POSITION%
        CURSOR.POSITION% = 1

        GOSUB PUT.CURSOR.IN.FIELD

        STRING.DATA$ = "31"
        GOSUB SET.FIELD.ATTRIBUTES

        MESSAGE.NO% = DISPLAY.MESSAGE.NUMBER%
        STRING.DATA$ = DISPLAY.MESSAGE.TEXT$
        RETURN.FIELD% = 1

        CALL EXTERNAL.MESSAGE(MESSAGE.NO%,                             \
                              STRING.DATA$,                            \
                              RETURN.FIELD%)

        CURSOR.POSITION% = OLD.POSITION%

        GOSUB PUT.CURSOR.IN.FIELD

        CURSOR.STATE$ = VISIBLE$
        GOSUB SET.CURSOR.STATE

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.FIELD.ATTRIBUTES                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redefine the specified field attributes                               *
\***                                                                          *
\******************************************************************************

        SET.FIELD.ATTRIBUTES:

        INTEGER.DATA% = 0

        CALL DM.SETF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.FIELD                                     *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the selected field with the data provided                         *
\***                                                                          *
\******************************************************************************

        SET.FIELD:

        INTEGER.DATA% = 0

        CALL DM.PUTF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PUT.CURSOR.IN.FIELD                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Position cursor in selected input field                               *
\***                                                                          *
\******************************************************************************

        PUT.CURSOR.IN.FIELD:

        STRING.DATA$ = ""

        INTEGER.DATA% = CURSOR.POSITION%

        CALL DM.POSF(STRING.DATA$,                                      \
                     INTEGER.DATA%)

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHAIN.TO.CALLER                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    CHAIN back to the required screen program                             *
\***                                                                          *
\******************************************************************************

        CHAIN.TO.CALLER:

        GOSUB WAIT.MESSAGE

        PSBCHN.PRG = "C:\ADX_UPGM\" + CHAIN.TO.PROG$ + ".286"
        PSBCHN.APP = "C:\ADX_UPGM\PSB99.286"

        %INCLUDE PSBCHNE.J86

        RETURN

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    E N D  O F  S U B R O U T I N E S                                     *
\***                                                                          *
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  E R R O R  R O U T I N E S                            *
\***                                                                          *
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***                                                                          *
\***    ERROR ROUTINE   :       FILE.ERROR                                    *
\***                                                                          *
\******************************************************************************

        FILE.ERROR:

        EVENT.NO% = 106

        FILE.NO$ = CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +                \
                   CHR$(SHIFT(CURRENT.REPORT.NUM%,0))

        IF FILE.OPERATION$ = "O" THEN                                  \
        BEGIN
           MESSAGE.NO%   = 501
           VAR.STRING.2$ = RIGHT$("000" +                              \
                           STR$(CURRENT.REPORT.NUM%),3)
        ENDIF                                                          \
        ELSE                                                           \
        IF FILE.OPERATION$ = "R" THEN                                  \
        BEGIN
           MESSAGE.NO% = 508
           VAR.STRING.2$ = RIGHT$("000" +                              \
                           STR$(CURRENT.REPORT.NUM%),3) +              \
                           UNPACK$(CURRENT.CODE$)
        ENDIF                                                          \
        ELSE                                                           \
        IF FILE.OPERATION$ = "W" THEN                                  \
        BEGIN
           MESSAGE.NO% = 509
           VAR.STRING.2$ = RIGHT$("000" +                              \
                           STR$(CURRENT.REPORT.NUM%),3)
        ENDIF

        VAR.STRING.1$ = FILE.OPERATION$ +                              \
                        FILE.NO$ +                                     \
                        PACK$(STRING$(12,"0"))

        CALL APPLICATION.LOG(MESSAGE.NO%,                              \
                             VAR.STRING.1$,                            \
                             VAR.STRING.2$,                            \
                             EVENT.NO%)

        RETURN

\******************************************************************************
\***                                                                          *
\***    ERROR ROUTINE   :       OPEN.ERROR                                    *
\***                                                                          *
\******************************************************************************

        OPEN.ERROR:

        FILE.OPERATION$ = "O"
        GOSUB FILE.ERROR

        CHAIN.TO.PROG$ = "PSB50"
        PSBCHN.MENCON  = "000000"
        GOTO CHAIN.TO.CALLER

\******************************************************************************
\***                                                                          *
\***    ERROR ROUTINE   :       ERROR.DETECTED                                *
\***                                                                          *
\******************************************************************************

        ERROR.DETECTED:

        CALL STANDARD.ERROR.DETECTED(ERRN,                             \
                                     ERRF%,                            \
                                     ERRL,                             \
                                     ERR)

        CHAIN.TO.PROG$ = "PSB50"
        RESUME CHAIN.TO.CALLER

        END SUB

