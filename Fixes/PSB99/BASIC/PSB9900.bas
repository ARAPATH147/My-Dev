
\******************************************************************************
\******************************************************************************
\***                                                                          *
\***            PROGRAM         :       PSB99                                 *
\***            AUTHOR          :       Mark Walker                           *
\***            DATE WRITTEN    :       March 4th 1994                        *
\***                                                                          *
\***            MODULE          :       PSB9900                               *
\***                                                                          *
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    Module Overview                                                       *
\***    ---------------                                                       *
\***                                                                          *
\***    This program allows the user to add, change, delete or report on      *
\***    operators. The following options are available :                      *
\***                                                                          *
\***            - Add a new operator                                          *
\***            - Display operator details                                    *
\***            - Change operator details                                     *
\***            - Set operator password                                       *
\***            - Delete an operator                                          *
\***            - Report operators                                            *
\***                                                                          *
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    VERSION H.                 Mark Walker                4th March 1994  *
\***    This module has been rewritten for the Controller Security package.   *
\***                                                                          *
\***    The re-written version excludes the pre-4690 OPEN, WRITE, TCLOSE and
\***    CLOSE of printer, along with associated checking of printer status.
\***
\***    REVISION 1.1            ROBERT COWEY.                     26 SEP 1994.
\***    Removed version letters from included code (not commented).
\***    Prevented update of authorisation files following use of F3 QUIT key.
\***    -Initial PVCS revision.
\***    -Corrected F3=ENTER error on leaving screens.
\***
\***    REVISION 1.2            MIKE BISHOP                       13 JUL 2004.
\***    Additional fields staff number and employee flag added.
\***    -Relinked to new Printfun due to a change
\***    -for printing reports for international
\***
\***    REVISION 1.3            Alan Carr  (AJC)                   4 Oct 2004.
\***    Additional field Name on Receipt added.
\***    -Changes for Beauty Commission project
\***    -Operator Staff number and employee flag
\***    -must now be entered
\***
\***    REVISION 1.4            Alan Carr  (AJC)                  31 Jan 2004.
\***    Add GROUP CODE for Beauty Commisson.
\***    -Added Group Code for MTSL processing
\***
\***    REVISION 1.5.                ROBERT COWEY.                09 JUN 2009.
\***    Removed discontinued PVCS variables $Log $Workfile and $Revision.
\***    Moved comments captured by these to this comment box (prefixed "-").
\***    Replaced call to CHECK.FIELDS.06 with call to DELETE.AN.OPERATOR.
\***    Removed variables not used by either PSB9900 or PSB9901.
\***
\***    Redefined PROCESS.SCREEN.06 and numerous subroutines as subprograms.
\***    Separated these into new module PSB9902 to reduce code size below 64k.
\***    New code segment sizes are approx 56K for PSB9900 and 15K for PSB9902.
\***    If the PSB9900 code size needs to be reduced further it should be
\***    possible to transfer many of the PUBLIC functions to PSB9902.
\***
\***    REVISION 1.6.                ROBERT COWEY.                15 JUN 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.6.
\***    Displayed operator Birth Date on all screens except reports.
\***    Add Operator acreen ...
\***      Forced setting of Birth Date.
\***      Continued to allow zeros to be entered for Staff Number.
\***    Change Operator screen ...
\***      Allowed (but not forced) setting of Birth Date.
\***      Prevented access to Birth Date variable if already set.
\***      Allowed (but not forced) setting of Staff Number.
\***      Prevented access to Staff Number variable if already set.
\***
\***    REVISION 1.7.                ROBERT COWEY.                20 JUL 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.7.
\***    Fix for minor bug found whilst correcting defect 3247.
\***    Corrected VALID.BIRTH.DATE function message (to allow 14th birthday).
\***
\***    REVISION 1.8.                ROBERT COWEY.                22 JUL 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.8.
\***    Defect 3247 - Redefined AF.BIRTH.DATE$ format as UPD-hex DDMCYY.
\***    Modified code to use PSB9902 date format conversion functions when
\***    updating or validating AF.BIRTH.DATE$.
\***    Modified VALID.BIRTH.DATE function to accept ages 14 to 99.
\***
\***    REVISION 1.9.                Nalini Mathusoothanan        01 AUG 2011.
\***    Changes for Core 2 Release project.
\***        - To enforce the Staff Number to be entered for all new users.
\***        - Make the Staff Number field mandatory for Boots Employees only.
\***        - Display error message when Staff Number has not been entered.
\***    Change to use PRINT.REPORT function, which handles YLBP printers.
\***
\******************************************************************************

        %INCLUDE MODELDEC.J86
        %INCLUDE CSOUFDEC.J86
        %INCLUDE AFDEC.J86
        %INCLUDE OPAUDDEC.J86
        %INCLUDE PRINTDEC.J86
        %INCLUDE PPDFDEC.J86
        %INCLUDE PHRMLDEC.J86

        %INCLUDE PSBF03G.J86   ! Display Manager
        %INCLUDE PSBF20G.J86   ! Session Number Utility

        %INCLUDE PSBUSEG.J86

        STRING GLOBAL           BATCH.SCREEN.FLAG$,                    \
                                CHAIN.TO.PROG$,                        \
                                CURSOR.STATE$,                         \
\ 1.5 RC                        INITIAL.DATA$,                         \
                                INVISIBLE$,                            \
\ 1.5 RC                        SAVED.STRING$,                         \
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
                                WORKFILE.FILE.NAME$                    ! 1.9 NM

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
\ 1.5 RC                        P%,                                    \
                                I%,                                    \
                                J%,                                    \
\ 1.5 RC                        C%,                                    \
                                S%,                                    \
                                FUNCTION.KEY%,                         \
\ 1.5 RC                        KEY.SELECTED%,                         \
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
                                WORKFILE.REPORT.NUM%,                  \ 1.9 NM
                                WORKFILE.SESS.NUM%                     ! 1.9 NM

        STRING GLOBAL           BIRTH.DATE$, \ ! DDMMCCYY              ! 1.6 RC
                                OPTION$,                               \
                                OPERATOR.ID$,                          \
                                OPERATOR.NAME$,                        \
                                OPERATOR.PASSWORD$,                    \
                                MODEL.FLAG$,                           \
                                CONFIRM$,                              \
                                TILL.MODEL.FLAG$(1),                   \
                                CTLR.MODEL.FLAG$(1),                   \
                                TILL.MODEL.NAME$(1),                   \
                                CTLR.MODEL.NAME$(1),                   \
                                TILL.MODEL.RECORD$(1),                 \
                                CTLR.MODEL.RECORD$(1),                 \
                                TODAYS.DATE$,                          \
                                CSOUF.RECORD$,                         \
                                CSOUF.OPERATION$,                      \
                                MODEL.NUMBER$,                         \
                                SUPERVISOR.FLAG$,                      \
                                OPTIONS.KEY$,                          \
\ 1.5 RC                        USER.ID$,                              \
                                GROUP.ID$,                             \
                                OLD.OPERATOR.ID$,                      \
                                OLD.PASSWORD$,                         \
                                MONTH.ARRAY$(1),                       \
                                DISPLAY.DATE$,                         \
                                STAFF.NO$,                             \
                                EMPLOYEE.FLG$,                         \ AJC
                                RECEIPT.NAME$,                         \ AJC
                                GROUP.CODE$                            !\ AJC

        INTEGER*1 GLOBAL        VALID.BIRTH.DATE.FOUND,                \ 1.6 RC
                                VALID.OPTION.FOUND,                    \
                                VALID.OPERATOR.ID.FOUND,               \
                                VALID.NAME.FOUND,                      \
                                VALID.PASSWORD.FOUND,                  \
                                VALID.STAFF.NO.FOUND,                  \
                                VALID.EMPLOYEE.FLG.FOUND,              \
                                VALID.RECEIPT.NAME.FOUND,              \ AJC
                                VALID.GROUP.CODE.FOUND,                \ AJC
                                VALID.MODEL.FLAG.FOUND,                \
                                VALID.CONFIRM.FOUND,                   \
                                CSOUF.RECORD.FOUND,                    \
                                OPTION.ALLOWED(1)

        INTEGER*2 GLOBAL        S1.OPTION%,                            \
                                S1.DATE%,                              \
                                S2.BIRTH.DATE%,                        \ 1.6 RC
                                S2.OPERATOR.ID%,                       \
                                S2.NAME%,                              \
                                S2.PASSWORD%,                          \
                                S2.STAFF.NO%,                          \
                                S2.EMPLOYEE.FLG%,                      \
                                S2.RECEIPT.NAME%,                      \  AJC
                                S2.GROUP.CODE%,                        \  AJC 1.4
                                S2.CONFIRM%,                           \
                                S2.CONFIRM.TEXT%,                      \
                                S3.BIRTH.DATE%,                        \ 1.6 RC
                                S3.OPERATOR.ID%,                       \
                                S3.NAME%,                              \
                                S3.STAFF.NO%,                          \
                                S3.EMPLOYEE.FLG%,                      \
                                S3.RECEIPT.NAME%,                      \  AJC
                                S3.GROUP.CODE%,                        \  AJC 1.4
                                S4.BIRTH.DATE%,                        \ 1.6 RC
                                S4.OPERATOR.ID%,                       \
                                S4.NAME%,                              \
                                S4.STAFF.NO%,                          \
                                S4.EMPLOYEE.FLG%,                      \
                                S4.RECEIPT.NAME%,                      \  AJC
                                S4.GROUP.CODE%,                        \  AJC 1.4
                                S5.BIRTH.DATE%,                        \ 1.6 RC
                                S5.OPERATOR.ID%,                       \
                                S5.NAME%,                              \
                                S5.PASSWORD%,                          \
                                S5.STAFF.NO%,                          \
                                S5.EMPLOYEE.FLG%,                      \
                                S5.RECEIPT.NAME%,                      \  AJC
                                S5.GROUP.CODE%,                        \  AJC 1.4
                                S6.BIRTH.DATE%,                        \ 1.6 RC
                                S6.OPERATOR.ID%,                       \
                                S6.NAME%,                              \
                                S6.STAFF.NO%,                          \
                                S6.EMPLOYEE.FLG%,                      \
                                S6.RECEIPT.NAME%,                      \  AJC
                                S6.GROUP.CODE%,                        \  AJC 1.4
                                S7.REPORT.OPTION%,                     \
                                S7.SORT.OPTION%,                       \
                                S2.TILL.MODEL.TEXT%(1),                \
                                S2.TILL.MODEL.FLAG%(1),                \
                                S2.CTLR.MODEL.TEXT%(1),                \
                                S2.CTLR.MODEL.FLAG%(1),                \
                                TILL.PTR%,                             \
                                MAX.TILL.PTR%,                         \
                                CTLR.PTR%,                             \
                                MAX.CTLR.PTR%,                         \
                                MAX.TILL.MODELS%,                      \
                                MAX.CTLR.MODELS%,                      \
                                INDICATOR%,                            \
                                AUTH.FLAGS%,                           \
                                TILL.INDEX%,                           \
                                CTLR.INDEX%,                           \
                                SAVED.OPAUD.REC.NUM%                   !\

         INTEGER*4 GLOBAL        BIT.MASK%,                            \
                                OPM.BIT.MASK%,                         \
                                SDK.BIT.MASK%

        %INCLUDE MODELEXT.J86
        %INCLUDE CSOUFEXT.J86
        %INCLUDE AFEXT.J86
        %INCLUDE OPAUDEXT.J86
        %INCLUDE PRINTEXT.J86
        %INCLUDE PPDFEXT.J86
        %INCLUDE PHRMLEXT.J86

        %INCLUDE PSBF01E.J86   ! Application Log
        %INCLUDE PSBF03E.J86   ! Display Manager
        %INCLUDE PSBF04E.J86   ! External Message
        %INCLUDE PSBF12E.J86   ! Help
        %INCLUDE PSBF20E.J86   ! Session Number Utility
        %INCLUDE PSBF24E.J86   ! Standard Error Detected

        %INCLUDE ADXAUTH.J86

        %INCLUDE DMEXTR.J86

        SUB PSB9901 EXTERNAL
        END SUB

        SUB PSB9902 EXTERNAL ! Separation of PROCESS.SCREEN.06         ! 1.5 RC
        END SUB                                                        ! 1.5 RC



\******************************************************************************
\***
\***    EXTERNAL FUNCTIONS DEFINED WITHIN PSB9902
\***
\***...........................................................................

FUNCTION VALID.YYMMDD (YYMMDD$) EXTERNAL ! PSB9902                     ! 1.6 RC
    INTEGER*1 VALID.YYMMDD                                             ! 1.6 RC
END FUNCTION                                                           ! 1.6 RC

FUNCTION DDMCYY.HEX.FROM.DDMMCCYY$ (DDMMCCYY$) EXTERNAL ! PSB9902      ! 1.8 RC
    STRING DDMCYY.HEX.FROM.DDMMCCYY$                                   ! 1.8 RC
END FUNCTION                                                           ! 1.8 RC

FUNCTION YYMMDD.FROM.DDMCYY.HEX$ (DDMCYY.HEX$) EXTERNAL ! PSB9902      ! 1.8 RC
    STRING YYMMDD.FROM.DDMCYY.HEX$                                     ! 1.8 RC
END FUNCTION                                                           ! 1.8 RC

FUNCTION VALID.DDMCYY (DDMCYY.HEX$) EXTERNAL ! PSB9902                 ! 1.8 RC
    INTEGER*1 VALID.DDMCYY                                             ! 1.8 RC
END FUNCTION                                                           ! 1.8 RC


\******************************************************************************
\***
\***    INTERNAL FUNCTIONS
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       DISPLAY.SCREEN                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Displays the required screen                                          *
\***                                                                          *
\******************************************************************************

        FUNCTION DISPLAY.SCREEN(SCREEN.NUMBER%) PUBLIC

        INTEGER*2                               SCREEN.NUMBER%

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.CLRSCR(STRING.DATA$,                                   \
                       INTEGER.DATA%)

        STRING.DATA$ = "B99" + RIGHT$("00" + STR$(SCREEN.NUMBER%),2)
        INTEGER.DATA% = 0

        CALL DM.DISPD(STRING.DATA$,                                    \
                      INTEGER.DATA%)

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

        FUNCTION SCREEN.HELP(SCREEN.NUMBER%) PUBLIC

        STRING                                  SCREEN.NUMBER$

        INTEGER*2                               SCREEN.NUMBER%

        STRING.DATA$ = ""
        INTEGER.DATA% = 1

        CALL DM.POSF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.UPDF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        LAST.MESSAGE$ = F03.RETURNED.STRING$

        SCREEN.NUMBER$ = "B99" + RIGHT$("00" +                         \
                         STR$(SCREEN.NUMBER%),2)

        CALL HELP(SCREEN.NUMBER$)

        STRING.DATA$  = ""
        INTEGER.DATA% = 1

        CALL DM.POSF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$  = "31"
        INTEGER.DATA% = 0

        CALL DM.SETF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$  = LAST.MESSAGE$
        INTEGER.DATA% = 0

        CALL DM.PUTF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.OPTION                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the option is valid                                       *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.OPTION PUBLIC

        INTEGER*1                               VALID.OPTION

        VALID.OPTION = FALSE

        IF OPTION$ >= "1" AND                                          \
           OPTION$ <= "6" THEN                                         \
           VALID.OPTION = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.OPERATOR.ID                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the operator ID is valid                                  *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.OPERATOR.ID PUBLIC

        INTEGER*1                               VALID.OPERATOR.ID

        VALID.OPERATOR.ID = FALSE

        IF OPERATOR.ID$ >= "100" AND                                   \
           OPERATOR.ID$ <= "999" THEN                                  \
           VALID.OPERATOR.ID = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.NAME                                    *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the operator name is valid                                *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.NAME PUBLIC

        INTEGER*1                               VALID.NAME

        VALID.NAME = FALSE

        IF OPERATOR.NAME$ <> "" THEN                                   \
           VALID.NAME = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.PASSWORD                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the operator password is valid                            *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.PASSWORD PUBLIC

        INTEGER*1                               VALID.PASSWORD

        VALID.PASSWORD = FALSE

        IF OPERATOR.PASSWORD$ >= "100" AND                             \
           OPERATOR.PASSWORD$ <= "999" THEN                            \
           VALID.PASSWORD = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.STAFF.NO                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the operator staff no is valid                            *
\***    If staff no. is zero for boots employee then display error message    *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.STAFF.NO(CHECK.NOT.NULL) PUBLIC                 ! 1.9 NM

            INTEGER*1                           VALID.STAFF.NO,   \    ! 1.9 NM
                                                CHECK.NOT.NULL         ! 1.9 NM
            VALID.STAFF.NO = FALSE
            DISPLAY.MESSAGE.TEXT$ = "Staff Number must be " +     \    ! 1.9 NM
                                    "zero or greater"                  ! 1.9 NM

            IF STAFF.NO$ >= "00000000"                            \
            AND STAFF.NO$ <= "99999999"    THEN BEGIN                  ! 1.9 NM

                VALID.STAFF.NO = TRUE                                  ! 1.9 NM
                DISPLAY.MESSAGE.TEXT$ = ""                             ! 1.9 NM

                IF CHECK.NOT.NULL                                 \    ! 1.9 NM
                AND EMPLOYEE.FLG$ = "Y"                           \    ! 1.9 NM
                AND STAFF.NO$ = "00000000" THEN BEGIN                  ! 1.9 NM
                    VALID.STAFF.NO = FALSE                             ! 1.9 NM
                    DISPLAY.MESSAGE.TEXT$ = "Staff Number  " +    \    ! 1.9 NM
                                        "must be entered "   +    \    ! 1.9 NM
                                        "for Boots Employees"          ! 1.9 NM
                ENDIF                                                  ! 1.9 NM

            ENDIF                                                      ! 1.9 NM

        END FUNCTION


\******************************************************************************
\***
\***    VALID.AF.STAFF.NUM
\***    Checks whether staff number read from authorisation file is set
\***    Assumes a non-zero numeric value is a valid staff number
\***    Checks value on file rather than STAFF.NO$ entered from screen
\***
\***...........................................................................

FUNCTION VALID.AF.STAFF.NUM ! Entire function new for Rv 1.6          ! 1.6 RC

    STRING    WORK$
    INTEGER*1 VALID.AF.STAFF.NUM

    VALID.AF.STAFF.NUM = 0 ! FALSE

    WORK$ = TRANSLATE$(UNPACK$(AF.STAFF.NUM$), "0123456789#", "########## ")

    IF WORK$ <> "########" THEN EXIT FUNCTION ! Contains non-numeric nibbles

    IF AF.STAFF.NUM$ = PACK$("00000000") THEN EXIT FUNCTION ! No value set

    VALID.AF.STAFF.NUM = -1 ! TRUE

END FUNCTION


\******************************************************************************
\***
\***    VALID.BIRTH.DATE
\***    Changes to this function should also be effected within PSB9902
\***    function VALID.DDMCYY.
\***
\***    Confirms BIRTH.DATE$ is a valid date with a realistic century.
\***    Confirms BIRTH.DATE$ is date of 14th birthday or later.
\***    Confirms BIRTH.DATE$ is before 100th birthday.
\***    Birth date is rejected if operator age is 13yrs X months but is
\***    accepted from 14th birthday onwards.
\***    Birth date is accepted if operator age is 99yrs X months but is
\***    rejected from 100th birthday onwards
\***
\***...........................................................................

FUNCTION VALID.BIRTH.DATE ! Entire function new for Rv 1.6             ! 1.6 RC

    STRING    YYMMDD$
    STRING    WORK$
    INTEGER*1 VALID.BIRTH.DATE
    INTEGER*4 CCYYMMDD%

    VALID.BIRTH.DATE = 0 ! FALSE

!   Confirm BIRTH.DATE$ is eight bytes numeric data

    WORK$ = TRANSLATE$(BIRTH.DATE$, "0123456789#", "########## ")

    IF WORK$ <> "########" THEN BEGIN
       DISPLAY.MESSAGE.NUMBER% = 123 ! B123 INVALID DATE SPECIFIED
       EXIT FUNCTION ! Not eight bytes numeric
     ENDIF

!   Confirm BIRTH.DATE$ has a sensible CC component                    ! 1.8 RC

    IF   VAL( MID$(BIRTH.DATE$,5,2) ) < 19 \                           ! 1.8 RC
      OR VAL( MID$(BIRTH.DATE$,5,2) ) > 20 THEN BEGIN \                ! 1.8 RC
        DISPLAY.MESSAGE.NUMBER% = 123 ! B123 INVALID DATE SPECIFIED    ! 1.8 RC
        EXIT FUNCTION ! Unrealistic century                            ! 1.8 RC
    ENDIF                                                              ! 1.8 RC

!   Confirm BIRTH.DATE$ has a valid YYMMDD combination

    YYMMDD$ = MID$(BIRTH.DATE$,7,2) + \   ! Valid BIRTH.DATE$ has
              MID$(BIRTH.DATE$,3,2) + \   ! unpacked DDMMCCYY format
              MID$(BIRTH.DATE$,1,2)

    IF NOT VALID.YYMMDD (YYMMDD$) THEN BEGIN ! Invalid date
       DISPLAY.MESSAGE.NUMBER% = 123 ! B123 INVALID DATE SPECIFIED
       EXIT FUNCTION
    ENDIF

!   Reject birth date when age is before 14th birthday                 ! 1.8 RC
!     "B196 Value entered must be dd/mm/ccyy or earlier                ! 1.8 RC
!             (minimum age allowed is 14)"                             ! 1.8 RC

    CCYYMMDD% = VAL( MID$(BIRTH.DATE$,5,2) + YYMMDD$ ) \               ! 1.8 RC
                  + 140000 ! Adds 14 years to birth date               ! 1.8 RC

    IF CCYYMMDD% > VAL("20" + DATE$) THEN BEGIN
        DISPLAY.MESSAGE.NUMBER% = 196 ! B196 Value entered must be ...
        DISPLAY.MESSAGE.TEXT$ = \                                      ! 1.7 RC
          MID$(DATE$,5,2) + "/" + \                ! DD
          MID$(DATE$,3,2) + "/" + \                ! MM
          STR$(VAL("20" + MID$(DATE$,1,2))-14) + \ ! CCYY -14 years
                             " or earlier (minimum age allowed is 14)" ! 1.7 RC
        EXIT FUNCTION
    ENDIF

!   Reject birth date when age is on or after 100th birthday           ! 1.8 RC
!     "B196 Value entered must be later than dd/mm/ccyy                ! 1.8 RC
!             (maximum age allowed is 99)"                             ! 1.8 RC

    CCYYMMDD% = VAL( MID$(BIRTH.DATE$,5,2) + YYMMDD$ ) \               ! 1.8 RC
                  + 1000000 ! Adds 100 years to birth date             ! 1.8 RC

    IF CCYYMMDD% <= VAL("20" + DATE$) THEN BEGIN                       ! 1.8 RC
        DISPLAY.MESSAGE.NUMBER% = 196 ! B196 Value entered must be ... ! 1.8 RC
        DISPLAY.MESSAGE.TEXT$ = "later than " + \                      ! 1.8 RC
          MID$(DATE$,5,2) + "/" + \                 ! DD               ! 1.8 RC
          MID$(DATE$,3,2) + "/" + \                 ! MM               ! 1.8 RC
          STR$(VAL("20" + MID$(DATE$,1,2))-100) + \ ! CCYY -100 years  ! 1.8 RC
                             " (maximum age allowed is 99)"            ! 1.8 RC
        EXIT FUNCTION                                                  ! 1.8 RC
    ENDIF                                                              ! 1.8 RC

    VALID.BIRTH.DATE = -1 ! TRUE

END FUNCTION


\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.EMPLOYEE.FLG                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the employee flag is valid                                *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.EMPLOYEE.FLG PUBLIC

        INTEGER*1                               VALID.EMPLOYEE.FLG

        VALID.EMPLOYEE.FLG = FALSE

        IF EMPLOYEE.FLG$ = "Y" OR                                        \
           EMPLOYEE.FLG$ = "N" THEN                                      \
           VALID.EMPLOYEE.FLG = TRUE

        END FUNCTION


\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.GROUP.CODE                  ! AJC  1.4  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that GROUP CODE for Beauty Commission is valid                 *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.GROUP.CODE PUBLIC                                ! AJC 1.4

        INTEGER*1                               VALID.GROUP.CODE        ! AJC 1.4

        VALID.GROUP.CODE = FALSE                                        ! AJC 1.4

        IF GROUP.CODE$  >= "00" AND                                     \ AJC 1.4
           GROUP.CODE$  <= "99" THEN                                    \ AJC 1.4
           VALID.GROUP.CODE = TRUE                                      ! AJC 1.4

        END FUNCTION                                                    ! AJC 1.4

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.MODEL.FLAG                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the model flag is valid                                   *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.MODEL.FLAG PUBLIC

        INTEGER*1                               VALID.MODEL.FLAG

        VALID.MODEL.FLAG = FALSE

        IF MODEL.FLAG$ = "Y" OR                                        \
           MODEL.FLAG$ = "N" THEN                                      \
           VALID.MODEL.FLAG = TRUE

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.CONFIRM                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the confirmation entry is valid                           *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.CONFIRM PUBLIC

        INTEGER*1                               VALID.CONFIRM

        VALID.CONFIRM = FALSE

        IF CONFIRM$ = "Y" OR                                           \
           CONFIRM$ = "N" THEN                                         \
           VALID.CONFIRM = TRUE

        END FUNCTION


\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       VALID.RECEIPT.NAME                    ! AJC   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Checks that the name to be printed on the receipt is valid    ! AJC   *
\***                                                                          *
\******************************************************************************

        FUNCTION VALID.RECEIPT.NAME PUBLIC                            ! AJC

        INTEGER*1                               VALID.RECEIPT.NAME    ! AJC

        VALID.RECEIPT.NAME = FALSE                                    ! AJC

        FOR I% = 1 TO 12                                     !

            IF I% = 1 THEN BEGIN
            IF LEFT$(RECEIPT.NAME$, I% + 1) = PACK$("0000")  \!OR
                   THEN BEGIN                                !
              RECEIPT.NAME$ = (" ")                         \
                            + RIGHT$(RECEIPT.NAME$, I% + 10)
            ENDIF
            ENDIF

            IF I% = 12 THEN BEGIN
            IF RIGHT$(RECEIPT.NAME$, 1) = PACK$("00")   \
                      THEN BEGIN                                !
            RECEIPT.NAME$ = LEFT$(RECEIPT.NAME$, I% - 1 ) + " " !
            ENDIF
            ENDIF

            IF I% > 1 AND I% < 12 THEN BEGIN
            IF MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("0000000000000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("00000000000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("000000000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("0000000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("00000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("000000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("0000") OR  \
               MID$(RECEIPT.NAME$, I%, I% + 1) = PACK$("00")   \
                   THEN BEGIN                                !
              RECEIPT.NAME$ = LEFT$(RECEIPT.NAME$, I% - 1) + " " \
                              + RIGHT$(RECEIPT.NAME$, 12 - I%)
            ENDIF                                             !
            ENDIF                                             !

        NEXT I%                                              !

        IF RECEIPT.NAME$ <> "" THEN                                  \! AJC
           VALID.RECEIPT.NAME = TRUE                                  ! AJC

        END FUNCTION                                                  ! AJC

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       READ.CSOUF.RECORD                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Read a record from the ADXCSOUF file                                  *
\***                                                                          *
\******************************************************************************

        FUNCTION READ.CSOUF.RECORD PUBLIC

        INTEGER*1                               READ.CSOUF.RECORD

        READ.CSOUF.RECORD = 1

        IF END # CSOUF.SESS.NUM% THEN CSOUF.READ.ERROR
        READ FORM "C34"; # CSOUF.SESS.NUM%, CSOUF.REC.NUM%; CSOUF.RECORD$

        READ.CSOUF.RECORD = 0

        EXIT FUNCTION

        CSOUF.READ.ERROR:

        CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
        FILE.OPERATION$ = "R"
        CURRENT.CODE$ = ""

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       DELETE.CSOUF.RECORD                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Delete a record from the ADXCSOUF file                                *
\***                                                                          *
\******************************************************************************

        FUNCTION DELETE.CSOUF.RECORD PUBLIC

        INTEGER*1                               DELETE.CSOUF.RECORD

        DELETE.CSOUF.RECORD = 1

        IF END # CSOUF.SESS.NUM% THEN CSOUF.WRITE.ERROR
        WRITE FORM "C34"; # CSOUF.SESS.NUM%, CSOUF.REC.NUM%; CSOUF.RECORD$

        DELETE.CSOUF.RECORD = 0

        EXIT FUNCTION

        CSOUF.WRITE.ERROR:

        CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
        FILE.OPERATION$ = "W"
        CURRENT.CODE$ = ""

        END FUNCTION

\******************************************************************************
\***                                                                          *
\***    FUNCTION        :       FORMAT.DATE$                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Format the date for displaying on the screen                          *
\***                                                                          *
\******************************************************************************

        FUNCTION FORMAT.DATE$(D$) PUBLIC

        STRING                                  D$,                    \
                                                DAY$,                  \
                                                MONTH$,                \
                                                YEAR$,                 \
                                                CENTURY$,              \
                                                FORMAT.DATE$

        DAY$     = RIGHT$("  " + STR$(VAL(MID$(D$,5,2))),2)
        MONTH$   = MONTH.ARRAY$(VAL(MID$(D$,3,2)))
        YEAR$    = MID$(D$,1,2)
        CENTURY$ = "19"

        IF VAL(YEAR$) < 80 THEN                                        \
        BEGIN
           CENTURY$ = "20"
        ENDIF

        FORMAT.DATE$ = DAY$ + " " + MONTH$ + " " + CENTURY$ + YEAR$

        END FUNCTION


\******************************************************************************
\***
\***    DEFINITIONS OF EXTERNAL SUBPROGRAMS SEPARATED INTO PSB9902     ! 1.5 RC
\***
\***...........................................................................


SUB SB.FILE.UTILS EXTERNAL ! PSB9902                                   ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB GET.INPUT EXTERNAL ! PSB9902                                       ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB PUT.CURSOR.IN.FIELD EXTERNAL ! PSB9902                             ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB RESUME.INPUT EXTERNAL ! PSB9902                                    ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB SET.CURSOR.STATE EXTERNAL ! PSB9902                                ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB SET.FIELD EXTERNAL ! PSB9902                                       ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB SET.FIELD.ATTRIBUTES EXTERNAL ! PSB9902                            ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB CLEAR.MESSAGE EXTERNAL ! PSB9902                                   ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB DISPLAY.MESSAGE EXTERNAL ! PSB9902                                 ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB WAIT.MESSAGE EXTERNAL ! PSB9902                                    ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB CHAIN.TO.CALLER EXTERNAL ! PSB9902                                 ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB CLEAR.MODEL.FLAGS EXTERNAL ! PSB9902                               ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB DISPLAY.FORMATTED.DATE EXTERNAL ! PSB9902                          ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB GET.CSOUF.RECORD EXTERNAL ! PSB9902                                ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB GET.OPERATOR.DETAILS EXTERNAL ! PSB9902                            ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB HIDE.CONFIRM.MESSAGE EXTERNAL ! PSB9902                            ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB RESET.MODEL.FLAGS EXTERNAL ! PSB9902                               ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB RESTORE.MODEL.FLAGS EXTERNAL ! PSB9902                             ! 1.5 RC
END SUB                                                                ! 1.5 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.02                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields for the add an operator screen                   *
\***                                                                          *
\******************************************************************************

SUB     RESTORE.FIELDS.02 PUBLIC                                       ! 1.5 RC

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.5 RC

        CURSOR.POSITION% = S2.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.ID$) > 0 THEN                                  \
        BEGIN
           OPERATOR.ID$ = STR$(VAL(OPERATOR.ID$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.ID$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.ID$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = OPERATOR.NAME$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.PASSWORD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.PASSWORD$) > 0 THEN                            \
        BEGIN
           OPERATOR.PASSWORD$ = STR$(VAL(OPERATOR.PASSWORD$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.PASSWORD$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.PASSWORD$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = STAFF.NO$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = RECEIPT.NAME$                                  ! AJC
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.BIRTH.DATE%                              ! 1.6 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.6 RC

        IF BIRTH.DATE$ <> "" THEN BEGIN \                              ! 1.6 RC
            STRING.DATA$ = MID$(BIRTH.DATE$,1,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,3,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,5,4)                       ! 1.6 RC
        ENDIF                                                          ! 1.6 RC
        CALL SET.FIELD                                                 ! 1.6 RC

        CURSOR.POSITION% = S2.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = GROUP.CODE$                                    ! AJC 1.4
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = EMPLOYEE.FLG$
        CALL SET.FIELD                                                 ! 1.5 RC

        CALL RESTORE.MODEL.FLAGS                                       ! 1.5 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = CONFIRM$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

END SUB                                                                ! 1.5 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.04                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields for the change operator screen                   *
\***                                                                          *
\******************************************************************************

SUB     RESTORE.FIELDS.04 PUBLIC                                       ! 1.5 RC

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.5 RC

        CURSOR.POSITION% = S4.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.ID$) > 0 THEN                                  \
        BEGIN
           OPERATOR.ID$ = STR$(VAL(OPERATOR.ID$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.ID$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.ID$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S4.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = OPERATOR.NAME$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S4.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = STAFF.NO$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S4.BIRTH.DATE%                              ! 1.6 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.6 RC

        IF BIRTH.DATE$ <> "" THEN BEGIN \                              ! 1.6 RC
            STRING.DATA$ = MID$(BIRTH.DATE$,1,2) + \                   ! 1.6 RC
                           MID$(BIRTH.DATE$,3,2) + \                   ! 1.6 RC
                           MID$(BIRTH.DATE$,5,4)                       ! 1.6 RC
        ENDIF                                                          ! 1.6 RC
        CALL SET.FIELD                                                 ! 1.6 RC

        CURSOR.POSITION% = S4.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = RECEIPT.NAME$                                  ! AJC
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S4.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = GROUP.CODE$                                    ! AJC 1.4
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S4.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = EMPLOYEE.FLG$
        CALL SET.FIELD                                                 ! 1.5 RC

        CALL RESTORE.MODEL.FLAGS                                       ! 1.5 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = CONFIRM$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

END SUB                                                                ! 1.5 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.05                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields for the set operator password screen             *
\***                                                                          *
\******************************************************************************

SUB     RESTORE.FIELDS.05 PUBLIC                                       ! 1.5 RC

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.5 RC

        CURSOR.POSITION% = S5.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.ID$) > 0 THEN                                  \
        BEGIN
           OPERATOR.ID$ = STR$(VAL(OPERATOR.ID$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.ID$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.ID$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = OPERATOR.NAME$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.PASSWORD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.PASSWORD$) > 0 THEN                            \
        BEGIN
           OPERATOR.PASSWORD$ = STR$(VAL(OPERATOR.PASSWORD$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.PASSWORD$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.PASSWORD$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = STAFF.NO$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = RECEIPT.NAME$                                  ! AJC
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.BIRTH.DATE%                              ! 1.6 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.6 RC

        IF BIRTH.DATE$ <> "" THEN BEGIN \                              ! 1.6 RC
            STRING.DATA$ = MID$(BIRTH.DATE$,1,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,3,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,5,4)                       ! 1.6 RC
        ENDIF                                                          ! 1.6 RC
        CALL SET.FIELD                                                 ! 1.6 RC

        CURSOR.POSITION% = S5.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = GROUP.CODE$                                    ! AJC 1.4
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S5.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = EMPLOYEE.FLG$
        CALL SET.FIELD                                                 ! 1.5 RC

        CALL RESTORE.MODEL.FLAGS                                       ! 1.5 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = CONFIRM$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

END SUB                                                                ! 1.5 RC

! Included as part of Core 2 Release                                   ! 1.9 NM
\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       WRK.SET                                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Initialise variables for work file                                    *
\***                                                                          *
\******************************************************************************

SUB     WRK.SET PUBLIC                                                 ! 1.9 NM

        WORKFILE.REPORT.NUM% = 426 ! Temporary work file               ! 1.9 NM
        WORKFILE.FILE.NAME$ = "ADXLXACN::D:\ADX_UDT1\PSB99RPT.BIN"     ! 1.9 NM

END SUB                                                                ! 1.9 NM

SUB RESTORE.FIELDS.06 EXTERNAL ! PSB9902                               ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB REVEAL.CONFIRM.MESSAGE EXTERNAL ! PSB9902                          ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB SET.OLD.OPAUD.DETAILS EXTERNAL ! PSB9902                           ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB SET.NEW.OPAUD.DETAILS EXTERNAL ! PSB9902                           ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB WRITE.OPAUD.RECORDS EXTERNAL ! PSB9902                             ! 1.5 RC
END SUB                                                                ! 1.5 RC


\******************************************************************************
\***
\***    HIGH LEVEL SUBPROGRAMS
\***    Dependant on one or more previously defined subprograms
\***
\******************************************************************************

SUB GET.QUIT.CONFIRM EXTERNAL ! PSB9902                                ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB GET.CONFIRM EXTERNAL ! PSB9902                                     ! 1.5 RC
END SUB                                                                ! 1.5 RC

SUB GET.QUIT.KEY EXTERNAL ! PSB9902                                    ! 1.5 RC
END SUB                                                                ! 1.5 RC

! SUB CLEAR.FIELDS.06 EXTERNAL ! PSB9902                               ! 1.5 RC
! END SUB                                                              ! 1.5 RC

! SUB DELETE.AUTH.RECORDS EXTERNAL ! PSB9902                           ! 1.5 RC
! END SUB                                                              ! 1.5 RC

! SUB DELETE.AN.OPERATOR EXTERNAL ! PSB9902                            ! 1.5 RC
! END SUB                                                              ! 1.5 RC

! SUB GET.OPERATOR.ID.06 EXTERNAL ! PSB9902                            ! 1.5 RC
! END SUB                                                              ! 1.5 RC


\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  M A I N L I N E  C O D E                              *
\***                                                                          *
\******************************************************************************
\******************************************************************************

        ON ERROR GOTO ERROR.DETECTED

        %INCLUDE PSBUSEE.J86

        RESUME.FROM.NP.ERROR: ! Resume point from NP error             ! 1.5 RC

        GOSUB INITIALISATION

        GOSUB PROCESS.SCREEN.01

        GOSUB TERMINATION

        STOP

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  S U B R O U T I N E S                                 *
\***                                                                          *
\******************************************************************************
\******************************************************************************


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       INITIALISATION                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Initialise main program variables                                     *
\***                                                                          *
\***    Allocate session numbers to files                                     *
\***                                                                          *
\***    OPEN required files                                                   *
\***                                                                          *
\***    Initialise Display Manager                                            *
\***                                                                          *
\***    RETURN                                                                *
\***                                                                          *
\******************************************************************************

        INITIALISATION:

        CALL MODEL.SET
        CALL AF.SET
        CALL CSOUF.SET
        CALL OPAUD.SET
        CALL PRINT.SET
        CALL PPDF.SET
        CALL PHRML.SET
        CALL WRK.SET                                                   ! 1.9 NM

        AF.FILE.NAME$ = "EALAUTH"                                      ! HMW

        DIM EXIT.KEY.PRESSED(8),                                       \
            TILL.MODEL.FLAG$(8),                                       \
            CTLR.MODEL.FLAG$(16),                                      \
            TILL.MODEL.NAME$(8),                                       \
            CTLR.MODEL.NAME$(16),                                      \
            TILL.MODEL.RECORD$(8),                                     \
            CTLR.MODEL.RECORD$(16),                                    \
            S2.TILL.MODEL.FLAG%(8),                                    \
            S2.TILL.MODEL.TEXT%(8),                                    \
            S2.CTLR.MODEL.FLAG%(16),                                   \
            S2.CTLR.MODEL.TEXT%(16),                                   \
            OPTION.ALLOWED(6),                                         \
            MONTH.ARRAY$(12)

        BATCH.SCREEN.FLAG$ = "S"
        MODULE.NUMBER$     = "PSB9900"
        OPERATOR.NUMBER$   = PSBCHN.OP
        CHAIN.TO.PROG$     = "PSB50"
        VAR.STRING.2$      = ""

        DISPLAY.MESSAGE.TEXT$ = ""

        TODAYS.DATE$ = DATE$

        TRUE  = -1
        FALSE = 0

        VISIBLE$    = "0"
        INVISIBLE$  = "1"
        CURSOR.ON$  = "0"
        CURSOR.OFF$ = "1"

        MONTH.ARRAY$(1)  = "JAN"
        MONTH.ARRAY$(2)  = "FEB"
        MONTH.ARRAY$(3)  = "MAR"
        MONTH.ARRAY$(4)  = "APR"
        MONTH.ARRAY$(5)  = "MAY"
        MONTH.ARRAY$(6)  = "JUN"
        MONTH.ARRAY$(7)  = "JUL"
        MONTH.ARRAY$(8)  = "AUG"
        MONTH.ARRAY$(9)  = "SEP"
        MONTH.ARRAY$(10) = "OCT"
        MONTH.ARRAY$(11) = "NOV"
        MONTH.ARRAY$(12) = "DEC"

        DISPLAY.DATE$ = FORMAT.DATE$(DATE$)

        HELP.KEY%  = -1
        QUIT.KEY%  = -3
        ESC.KEY%   = 27
        ENTER.KEY% = 0
        BTAB.KEY%  = 8217
        END.KEY%   = 335
        HOME.KEY%  = 327
        TAB.KEY%   = 9
        PGUP.KEY%  = 329
        PGDN.KEY%  = 337
        F7UP.KEY%  = -7
        F8DN.KEY%  = -8

        OPM.BIT.MASK% = 0080H
        SDK.BIT.MASK% = 8000H

        INVISIBLE.FIELD% = 5
        S1.OPTION%       = 6
        S1.DATE%         = 60

        S2.OPERATOR.ID%  = 6
        S2.NAME%         = 7
        S2.GROUP.CODE%   = 62                                         ! AJC 1.4
        S2.PASSWORD%     = 8
        S2.STAFF.NO%     = 58
        S2.EMPLOYEE.FLG% = 59
        S2.RECEIPT.NAME% = 61                                         ! AJC
        S2.BIRTH.DATE%   = 63                                         ! 1.6 RC
        S2.CONFIRM%      = 4
        S2.CONFIRM.TEXT% = 57

        S3.OPERATOR.ID%  = 6
        S3.NAME%         = 7
        S3.STAFF.NO%     = 58
        S3.EMPLOYEE.FLG% = 59
        S3.RECEIPT.NAME% = 61                                         ! AJC
        S3.GROUP.CODE%   = 62                                         ! AJC 1.4
        S3.BIRTH.DATE%   = 63                                         ! 1.6 RC

        S4.OPERATOR.ID%  = 6
        S4.NAME%         = 7
        S4.GROUP.CODE%   = 62                                         ! AJC 1.4
        S4.STAFF.NO%     = 58
        S4.EMPLOYEE.FLG% = 59
        S4.RECEIPT.NAME% = 61                                         ! AJC
        S4.BIRTH.DATE%   = 63                                         ! 1.6 RC

        S5.OPERATOR.ID%  = 6
        S5.NAME%         = 7
        S5.PASSWORD%     = 8
        S5.STAFF.NO%     = 58
        S5.EMPLOYEE.FLG% = 59
        S5.RECEIPT.NAME% = 61                                         ! AJC
        S5.GROUP.CODE%   = 62                                         ! AJC 1.4
        S5.BIRTH.DATE%   = 63                                         ! 1.6 RC

        S6.OPERATOR.ID%  = 6
        S6.NAME%         = 7
        S6.STAFF.NO%     = 58
        S6.EMPLOYEE.FLG% = 59
        S6.RECEIPT.NAME% = 61                                         ! AJC
        S6.GROUP.CODE%   = 62                                         ! AJC 1.4
        S6.BIRTH.DATE%   = 63                                         ! 1.6 RC

        S7.SORT.OPTION%   = 6
        S7.REPORT.OPTION% = 7

        FOR I% = 1 TO 6
            OPTION.ALLOWED(I%) = TRUE
        NEXT I%

        FOR I% = 1 TO 8
            S2.TILL.MODEL.TEXT%(I%)     = I% + 8
            S2.TILL.MODEL.FLAG%(I%)     = I% + 32
            S2.CTLR.MODEL.TEXT%(I%)     = I% + 16
            S2.CTLR.MODEL.FLAG%(I%)     = I% + 40
            S2.CTLR.MODEL.TEXT%(I% + 8) = I% + 24
            S2.CTLR.MODEL.FLAG%(I% + 8) = I% + 48
        NEXT I%

        MAX.TILL.MODELS% = 8
        MAX.CTLR.MODELS% = 16

        GOSUB ALLOCATE.SESS.NUMS

        CURRENT.REPORT.NUM% = AF.REPORT.NUM%
        IF END # AF.SESS.NUM% THEN OPEN.ERROR
        OPEN AF.FILE.NAME$ KEYED RECL AF.RECL% AS AF.SESS.NUM%

        CURRENT.REPORT.NUM% = CSOUF.REPORT.NUM%
        IF END # CSOUF.SESS.NUM% THEN OPEN.ERROR
        OPEN CSOUF.FILE.NAME$ DIRECT RECL CSOUF.RECL% AS CSOUF.SESS.NUM%

        CURRENT.REPORT.NUM% = OPAUD.REPORT.NUM%
        IF END # OPAUD.SESS.NUM% THEN CREATE.OPAUD.FILE
        OPEN OPAUD.FILE.NAME$ DIRECT RECL OPAUD.RECL% AS OPAUD.SESS.NUM%

        GOTO OPAUD.FILE.FOUND

        CREATE.OPAUD.FILE:

        CREATE POSFILE OPAUD.FILE.NAME$                                \
               DIRECT 0                                                \
               RECL OPAUD.RECL%                                        \
               AS OPAUD.SESS.NUM%                                      \
               LOCAL

        OPAUD.REC.NUM%          = 1
        OPAUD.LAST.REC.UPDATED$ = "0100"
        OPAUD.FILE.SIZE$        = "0100"
        OPAUD.FILLER$           = STRING$(70," ")
        OPAUD.CRLF$             = CHR$(0DH) + CHR$(0AH)

        IF WRITE.OPAUD <> 0 THEN                                       \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        OPAUD.FILE.FOUND:

        GOSUB READ.OPAUD.HEADER.RECORD

        STRING.DATA$ = "B9901"
        INTEGER.DATA% = -1

        CALL DM.INITDM(STRING.DATA$,                                   \
                       INTEGER.DATA%)

        GOSUB GET.OPERATOR.AUTH

        GOSUB BUILD.MODEL.TABLES

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       READ.OPAUD.HEADER.RECORD                      *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Read the header record from the OPAUD file                            *
\***                                                                          *
\******************************************************************************

        READ.OPAUD.HEADER.RECORD:

        OPAUD.REC.NUM% = 1

        IF READ.OPAUD = 0 THEN                                         \
        BEGIN
           OPAUD.REC.NUM% = VAL(OPAUD.LAST.REC.UPDATED$) + 1

           IF OPAUD.REC.NUM% > VAL(OPAUD.FILE.SIZE$) THEN              \
           BEGIN
              OPAUD.REC.NUM% = 2
           ENDIF
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.AUTH                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get the authorisation of options for the current operator             *
\***                                                                          *
\******************************************************************************

        GET.OPERATOR.AUTH:

        AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +                \
                          OPERATOR.NUMBER$,8))

        IF LEN(OPERATOR.NUMBER$) = 8 THEN                              \
        BEGIN
           OPAUD.CURRENT.ID$ = "*" +                                   \
                               LEFT$(OPERATOR.NUMBER$,1) +             \
                               "*"
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPAUD.CURRENT.ID$ = RIGHT$("000" + OPERATOR.NUMBER$,3)
        ENDIF

        IF READ.AF.ABREV = 0 THEN                                      \
        BEGIN
           IF NOT (((AF.MODEL.FLAGS.2% AND OPM.BIT.MASK%) > 0) OR      \
                  ((AF.MODEL.FLAGS.2% AND SDK.BIT.MASK%) > 0)) THEN    \
           BEGIN
              OPTION.ALLOWED(1) = FALSE
              OPTION.ALLOWED(3) = FALSE
              OPTION.ALLOWED(4) = FALSE
              OPTION.ALLOWED(5) = FALSE
           ENDIF
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       BUILD.MODEL.TABLES                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Build a table of the till and controller models from the MODEL file   *
\***                                                                          *
\******************************************************************************

        BUILD.MODEL.TABLES:

        CURRENT.REPORT.NUM% = MODEL.REPORT.NUM%
        IF END # MODEL.SESS.NUM% THEN OPEN.ERROR
        OPEN MODEL.FILE.NAME$ KEYED RECL MODEL.RECL% AS MODEL.SESS.NUM%

        TILL.PTR% = 0

        FOR I% = 1 TO MAX.TILL.MODELS%

            MODEL.KEY$ = "T" + PACK$(RIGHT$("00" + STR$(I%),2))

            IF READ.MODEL = 0 THEN                                     \
            BEGIN
               IF MODEL.DISPLAY.FLAG$ = "Y" THEN                       \
               BEGIN
                  TILL.PTR%                     = TILL.PTR% + 1
                  TILL.MODEL.RECORD$(TILL.PTR%) = MODEL.KEY$ + MODEL.RECORD$
                  TILL.MODEL.NAME$(TILL.PTR%)   = MID$(MODEL.RECORD$,1,20)
               ENDIF
            ENDIF

        NEXT I%

        MAX.TILL.PTR% = TILL.PTR%

        CTLR.PTR% = 0

        FOR I% = 1 TO MAX.CTLR.MODELS%

            MODEL.KEY$ = "C" + PACK$(RIGHT$("00" + STR$(I%),2))

            IF READ.MODEL = 0 THEN                                     \
            BEGIN
               IF MODEL.DISPLAY.FLAG$ = "Y" THEN                       \
               BEGIN
                  CTLR.PTR%                     = CTLR.PTR% + 1
                  CTLR.MODEL.RECORD$(CTLR.PTR%) = MODEL.KEY$ + MODEL.RECORD$
                  CTLR.MODEL.NAME$(CTLR.PTR%)   = MID$(MODEL.RECORD$,1,20)
               ENDIF
            ENDIF

        NEXT I%
        MAX.CTLR.PTR% = CTLR.PTR%

        CLOSE MODEL.SESS.NUM%

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.01                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the initial screen                                *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.01:

        S% = 1

        CALL DISPLAY.SCREEN(1)

        OPTION$ = ""

        GOSUB RESTORE.FIELDS.01

        EXIT.KEY.PRESSED(1) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(1)
              GOSUB GET.OPTION.01
        WEND

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.01                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Displays the fields for the initial screen                            *
\***                                                                          *
\******************************************************************************

        RESTORE.FIELDS.01:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.5 RC

        FOR I% = 1 TO 6

            IF OPTION.ALLOWED(I%) THEN                                 \
            BEGIN
               CURSOR.POSITION% = 10 + I%
               CALL PUT.CURSOR.IN.FIELD                                ! 1.5 RC

               STRING.DATA$ = "31"
               CALL SET.FIELD.ATTRIBUTES                               ! 1.5 RC
            ENDIF

        NEXT I%

        CURSOR.POSITION% = S1.OPTION%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = OPTION$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPTION.01                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for selecting an option from the main menu              *
\***                                                                          *
\******************************************************************************

        GET.OPTION.01:

        CURSOR.POSITION% = S1.OPTION%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(1) = FALSE
        VALID.OPTION.FOUND  = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(1) OR                              \
                  VALID.OPTION.FOUND)

              OPTION$ = F03.RETURNED.STRING$

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
                 EXIT.KEY.PRESSED(1) = TRUE                            \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(1)
                 GOSUB RESTORE.FIELDS.01

                 CURSOR.POSITION% = S1.OPTION%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPTION THEN                                  \
                    VALID.OPTION.FOUND = TRUE                          \
                 ELSE                                                  \
                 BEGIN
                    ! B003 Invalid selection number
                    DISPLAY.MESSAGE.NUMBER% = 3
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPTION.FOUND AND                                      \
           NOT EXIT.KEY.PRESSED(1) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPTION$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
              GOSUB REQUIRED.OPTION
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.02                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the add an operator screen                        *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.02:

        S% = 2

        CALL DISPLAY.SCREEN(2)

        OLD.OPERATOR.ID$    = ""
        OPERATOR.ID$        = ""
        OPERATOR.NAME$      = ""
        OPERATOR.PASSWORD$  = ""
        STAFF.NO$           = ""
        EMPLOYEE.FLG$       = ""
        RECEIPT.NAME$       = ""                                       ! AJC
        BIRTH.DATE$         = ""                                       ! 1.6 RC
        GROUP.CODE$           = ""                                       ! AJC 1.4
        MODEL.FLAG$         = "N"
        CONFIRM$            = "N"

        CALL RESET.MODEL.FLAGS                                         ! 1.5 RC

        CALL SET.OLD.OPAUD.DETAILS                                     ! 1.5 RC

        CALL RESTORE.FIELDS.02                                         ! 1.5 RC

        EXIT.KEY.PRESSED(2) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(2)
              GOSUB GET.OPERATOR.ID.02
        WEND

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.FIELDS.02                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear all fields for the add operator screen                          *
\***                                                                          *
\******************************************************************************

        CLEAR.FIELDS.02:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        IF OPERATOR.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S2.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF OPERATOR.PASSWORD$ <> "" THEN                               \
        BEGIN
           CURSOR.POSITION% = S2.PASSWORD%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        CURSOR.POSITION% = S2.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = ""
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = ""                                             ! AJC
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.BIRTH.DATE%                              ! 1.6 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.6 RC

        STRING.DATA$ = ""                                              ! 1.6 RC
        CALL SET.FIELD                                                 ! 1.6 RC

        CURSOR.POSITION% = S2.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = ""                                             ! AJC 1.4
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S2.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = ""
        CALL SET.FIELD                                                 ! 1.5 RC

        CALL CLEAR.MODEL.FLAGS                                         ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.ID.02                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator ID on the add an operator screen       *
\***                                                                          *
\******************************************************************************

        GET.OPERATOR.ID.02:

        CURSOR.POSITION% = S2.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)     = FALSE
        VALID.OPERATOR.ID.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.OPERATOR.ID.FOUND)

              OPERATOR.ID$ = RIGHT$(STRING$(3,"0") +                   \
                             STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(S%) = TRUE
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.OPERATOR.ID%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPERATOR.ID THEN                             \
                    VALID.OPERATOR.ID.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B058 Invalid operator ID
                    DISPLAY.MESSAGE.NUMBER% = 58
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPERATOR.ID.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.ID$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% OR                              \
              FUNCTION.KEY% = END.KEY% OR                              \
              FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              IF VAL(OPERATOR.ID$) >= 901 AND                          \
                 VAL(OPERATOR.ID$) <= 909 AND                          \
                 VAL(OPERATOR.ID$) <> 905 THEN                         \
              BEGIN
                 OLD.OPERATOR.ID$ = ""

                 GOSUB CLEAR.FIELDS.02

                 DISPLAY.MESSAGE.NUMBER% = 173
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ <> OPERATOR.ID$ THEN                 \
              BEGIN
                 AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +       \
                                   OPERATOR.ID$,8))

                 IF READ.AF.ABREV <> 0 THEN                            \
                 BEGIN
                    OLD.OPERATOR.ID$ = OPERATOR.ID$

                    GOSUB GET.NAME.02
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    OLD.OPERATOR.ID$ = ""

                    GOSUB CLEAR.FIELDS.02

                    ! B172 Operator ID already in use
                    DISPLAY.MESSAGE.NUMBER% = 172
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ = OPERATOR.ID$ THEN                  \
              BEGIN
                 IF FUNCTION.KEY% = ENTER.KEY% THEN                    \
                 BEGIN
                    GOSUB CHECK.FIELDS.02
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF FUNCTION.KEY% = TAB.KEY% THEN                      \
                 BEGIN
                    GOSUB GET.NAME.02
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF FUNCTION.KEY% = END.KEY% THEN                      \
                 BEGIN
                    GOSUB GET.EMPLOYEE.FLG.02
                 ENDIF
              ENDIF
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.NAME.02                                   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator name on the add an operator screen     *
\***                                                                          *
\******************************************************************************

        GET.NAME.02:

        CURSOR.POSITION% = S2.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2) = FALSE
        VALID.NAME.FOUND    = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.NAME.FOUND)

              OPERATOR.NAME$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.NAME%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.NAME THEN                                    \
                    VALID.NAME.FOUND = TRUE                            \
                 ELSE                                                  \
                 BEGIN
                    ! B358 Invalid name
                    DISPLAY.MESSAGE.NUMBER% = 358
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.NAME.FOUND AND                                        \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           OPERATOR.NAME$ = LEFT$(OPERATOR.NAME$ +                     \
                            STRING$(20," "),20)

           STRING.DATA$ = OPERATOR.NAME$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              GOSUB GET.PASSWORD.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% OR                             \
              FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              GOSUB GET.OPERATOR.ID.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              GOSUB GET.EMPLOYEE.FLG.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.PASSWORD.02                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the password on the add an operator screen          *
\***                                                                          *
\******************************************************************************

        GET.PASSWORD.02:

        CURSOR.POSITION% = S2.PASSWORD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)  = FALSE
        VALID.PASSWORD.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.PASSWORD.FOUND)

              OPERATOR.PASSWORD$ = RIGHT$(STRING$(3,"0") +             \
                                   STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.PASSWORD%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.PASSWORD THEN                                \
                    VALID.PASSWORD.FOUND = TRUE                        \
                 ELSE                                                  \
                 BEGIN
                    ! B332 Invalid password
                    DISPLAY.MESSAGE.NUMBER% = 332
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.PASSWORD.FOUND AND                                    \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.PASSWORD$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              GOSUB GET.STAFF.NO.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              GOSUB GET.OPERATOR.ID.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              GOSUB GET.NAME.02
           ENDIF                                                       \
           ELSE
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              GOSUB GET.EMPLOYEE.FLG.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.STAFF.NO.02                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the staff no on the add an operator screen          *
\***                                                                          *
\******************************************************************************

        GET.STAFF.NO.02:

        CURSOR.POSITION% = S2.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)  = FALSE
        VALID.STAFF.NO.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.STAFF.NO.FOUND)

              STAFF.NO$ = RIGHT$(STRING$(8,"0") +             \
                                   STR$(VAL(F03.RETURNED.STRING$)),8)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.STAFF.NO%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.STAFF.NO(FALSE)                             \! 1.9 NM
                 THEN BEGIN                                            ! 1.9 NM
                    VALID.STAFF.NO.FOUND = TRUE
                 ENDIF ELSE BEGIN                                      ! 1.9 NM

                    ! B221 Free format
                    DISPLAY.MESSAGE.NUMBER% = 221

                    !Commented as part of Core 2 Release               ! 1.9 NM
                    !DISPLAY.MESSAGE.TEXT$ =                          \! 1.9 NM
                    !"STAFF NUMBER MUST BE 0 OR GREATER"               ! 1.9 NM

                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.STAFF.NO.FOUND AND                                    \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = STAFF.NO$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \ !.6 RC
           BEGIN
              GOSUB GET.BIRTH.DATE.02                                  ! 1.6 RC
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              GOSUB GET.OPERATOR.ID.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              GOSUB GET.PASSWORD.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \ !.6 RC
           BEGIN                                                       ! 1.6 RC
              GOSUB GET.EMPLOYEE.FLG.02                                ! 1.6 RC
           ENDIF                                                       \ 1.6 RC
           ELSE                                                        \ 1.6 RC
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN


\******************************************************************************
\***
\***    SUBROUTINE      :       GET.BIRTH.DATE.02
\***
\******************************************************************************
\***
\***    Input routine for the birth date on the add an operator screen
\***
\******************************************************************************

        GET.BIRTH.DATE.02: ! Entire procedure new for Rv 1.6              ! 1.6 RC
                           ! Modified from copy of GET.PASSWORD.02        ! 1.6 RC

        CURSOR.POSITION% = S2.BIRTH.DATE%
        CALL PUT.CURSOR.IN.FIELD

        EXIT.KEY.PRESSED(2)  = FALSE
        VALID.BIRTH.DATE.FOUND = FALSE

        CALL GET.INPUT

        WHILE NOT (EXIT.KEY.PRESSED(2) OR  \
                  VALID.BIRTH.DATE.FOUND)

              BIRTH.DATE$ = F03.RETURNED.STRING$

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR \
                 FUNCTION.KEY% = END.KEY% OR \
                 FUNCTION.KEY% = TAB.KEY% OR \
                 FUNCTION.KEY% = BTAB.KEY% OR \
                 FUNCTION.KEY% = HOME.KEY% OR \
                 FUNCTION.KEY% = HELP.KEY% OR \
                 FUNCTION.KEY% = QUIT.KEY% OR \
                 FUNCTION.KEY% = ESC.KEY%) THEN \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE
                 CALL RESUME.INPUT
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = QUIT.KEY% THEN \
              BEGIN
                 CALL GET.QUIT.CONFIRM
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = ESC.KEY% THEN \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = HELP.KEY% THEN \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02
                 CURSOR.POSITION% = S2.BIRTH.DATE%
                 CALL PUT.CURSOR.IN.FIELD
                 CALL GET.INPUT
              ENDIF \
              ELSE \
              BEGIN
                 IF VALID.BIRTH.DATE THEN \ ! Validates BIRTH.DATE$
                    VALID.BIRTH.DATE.FOUND = TRUE \
                 ELSE \
                 BEGIN
!                   DISPLAY.MESSAGE.NUMBER% set within VALID.BIRTH.DATE
                    CALL DISPLAY.MESSAGE
                    CALL RESUME.INPUT
                 ENDIF
              ENDIF
        WEND

        IF VALID.BIRTH.DATE.FOUND AND \
           NOT EXIT.KEY.PRESSED(2) THEN \
        BEGIN
           CALL CLEAR.MESSAGE

           STRING.DATA$ = BIRTH.DATE$
           CALL SET.FIELD

           IF FUNCTION.KEY% = TAB.KEY% THEN \
           BEGIN
              GOSUB GET.RECEIPT.NAME.02
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = HOME.KEY% THEN \
           BEGIN
              GOSUB GET.OPERATOR.ID.02
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = BTAB.KEY% THEN \
           BEGIN
              GOSUB GET.STAFF.NO.02
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = END.KEY% THEN \
           BEGIN
              GOSUB GET.EMPLOYEE.FLG.02
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = ENTER.KEY% THEN \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.RECEIPT.NAME.02                     ! AJC *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the receipt name on the add an operator screen! AJC *
\***                                                                          *
\******************************************************************************

        GET.RECEIPT.NAME.02:                                          ! AJC

        CURSOR.POSITION% = S2.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)  = FALSE                                  ! AJC
        VALID.RECEIPT.NAME.FOUND = FALSE                              ! AJC

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                            \! AJC
                  VALID.RECEIPT.NAME.FOUND)                           ! AJC

              RECEIPT.NAME$ = F03.RETURNED.STRING$                    ! AJC

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                  \! AJC
                 FUNCTION.KEY% = END.KEY% OR                         \! AJC
                 FUNCTION.KEY% = TAB.KEY% OR                         \! AJC
                 FUNCTION.KEY% = BTAB.KEY% OR                        \! AJC
                 FUNCTION.KEY% = HOME.KEY% OR                        \! AJC
                 FUNCTION.KEY% = HELP.KEY% OR                        \! AJC
                 FUNCTION.KEY% = QUIT.KEY% OR                        \! AJC
                 FUNCTION.KEY% = ESC.KEY%) THEN                      \! AJC
              BEGIN                                                   ! AJC
                 DISPLAY.MESSAGE.NUMBER% = 1                          ! AJC
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = QUIT.KEY% THEN                      \! AJC
              BEGIN                                                   ! AJC
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = ESC.KEY% THEN                       \! AJC
              BEGIN                                                   ! AJC
                 CHAIN.TO.PROG$ = "PSB50"                             ! AJC
                 PSBCHN.MENCON  = "000000"                            ! AJC
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = HELP.KEY% THEN                      \! AJC
              BEGIN                                                   ! AJC
                 CALL SCREEN.HELP(2)                                  ! AJC
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.RECEIPT.NAME%                  ! AJC
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              BEGIN                                                   ! AJC

                 IF VALID.RECEIPT.NAME THEN                          \! AJC
                    VALID.RECEIPT.NAME.FOUND = TRUE                  \! AJC
                 ELSE                                                \! AJC
                 BEGIN                                                ! AJC
                    ! B221 Free format                                ! AJC
                    DISPLAY.MESSAGE.NUMBER% = 221                     ! AJC
                    DISPLAY.MESSAGE.TEXT$ =                          \! AJC
                    "NAME ON RECEIPT MUST BE ENTERED  "               ! AJC
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF                                                ! AJC
              ENDIF                                                   ! AJC
        WEND                                                          ! AJC

        IF VALID.RECEIPT.NAME.FOUND AND                              \! AJC
           NOT EXIT.KEY.PRESSED(2) THEN                              \! AJC
        BEGIN                                                         ! AJC
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = RECEIPT.NAME$                               ! AJC
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                          \!  1.6 RC
           BEGIN                                                      ! AJC
!AJC 1.4      GOSUB GET.EMPLOYEE.FLG.02                               ! AJC
              GOSUB GET.GROUP.CODE.02                                 ! AJC 1.4
           ENDIF                                                     \! AJC
           ELSE                                                      \! AJC
           IF FUNCTION.KEY% = HOME.KEY% THEN                         \! AJC
           BEGIN                                                      ! AJC
              GOSUB GET.OPERATOR.ID.02                                ! AJC
           ENDIF                                                     \! AJC
           ELSE                                                      \! AJC
           IF FUNCTION.KEY% = BTAB.KEY% THEN                         \! AJC
           BEGIN                                                      ! AJC
              GOSUB GET.BIRTH.DATE.02                                  ! !.1 RC
           ENDIF                                                     \! AJC
           ELSE                                                      \! AJC
           IF FUNCTION.KEY% = END.KEY% THEN \                          ! 1.6 RC
           BEGIN                                                       ! 1.6 RC
              GOSUB GET.EMPLOYEE.FLG.02                                ! 1.6 RC
           ENDIF \                                                     ! 1.6 RC
           ELSE \                                                      ! 1.6 RC
           IF FUNCTION.KEY% = ENTER.KEY% THEN                        \! AJC
           BEGIN                                                      ! AJC
              GOSUB CHECK.FIELDS.02                                   ! AJC
           ENDIF                                                      ! AJC
        ENDIF                                                         ! AJC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.EMPLOYEE.FLG.02                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the employee flag on the add an operator screen     *
\***                                                                          *
\******************************************************************************

        GET.EMPLOYEE.FLG.02:

        CURSOR.POSITION% = S2.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)  = FALSE
        VALID.EMPLOYEE.FLG.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.EMPLOYEE.FLG.FOUND)

              EMPLOYEE.FLG$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.EMPLOYEE.FLG%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.EMPLOYEE.FLG THEN                            \
                    VALID.EMPLOYEE.FLG.FOUND = TRUE                    \
                 ELSE                                                  \
                 BEGIN
                    ! B064 YOU MUST ONLY TYPE "N" OR "Y"
                    DISPLAY.MESSAGE.NUMBER% = 64
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.EMPLOYEE.FLG.FOUND AND                                \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = EMPLOYEE.FLG$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              TILL.PTR% = 1
              GOSUB GET.TILL.MODEL.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              GOSUB GET.OPERATOR.ID.02
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
!             GOSUB GET.STAFF.NO.02                                    ! AJC
! AJC 1.4     GOSUB GET.RECEIPT.NAME.02                                ! AJC
              GOSUB GET.GROUP.CODE.02                                  ! AJC 1.4
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \ AJC 1.4
           BEGIN                                                       ! AJC 1.4
             TILL.PTR% = MAX.TILL.PTR%                                 ! AJC 1.4
             GOSUB GET.TILL.MODEL.02                                   ! AJC 1.4
           ENDIF                                                       \ AJC 1.4
           ELSE                                                        \ AJC 1.4
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.GROUP.CODE.02                   ! AJC 1.4 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the Behaviour Group on Add an operator screen       *
\***                                                                          *
\******************************************************************************

        GET.GROUP.CODE.02:                                            ! AJC 1.4

        CURSOR.POSITION% = S2.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)  = FALSE                                  ! AJC 1.4
        VALID.GROUP.CODE.FOUND = FALSE                                ! AJC 1.4

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                            \! AJC 1.4
                  VALID.GROUP.CODE.FOUND)                             ! AJC 1.4

              GROUP.CODE$ = RIGHT$(STRING$(2,"0") +                  \  AJC 1.4
                                   STR$(VAL(F03.RETURNED.STRING$)),2) ! AJC 1.4

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                  \! AJC 1.4
                 FUNCTION.KEY% = END.KEY% OR                         \! AJC 1.4
                 FUNCTION.KEY% = TAB.KEY% OR                         \! AJC 1.4
                 FUNCTION.KEY% = BTAB.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = HOME.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = HELP.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = QUIT.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = ESC.KEY%) THEN                      \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 DISPLAY.MESSAGE.NUMBER% = 1                          ! AJC 1.4
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = QUIT.KEY% THEN                      \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = ESC.KEY% THEN                       \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 CHAIN.TO.PROG$ = "PSB50"                             ! AJC 1.4
                 PSBCHN.MENCON  = "000000"                            ! AJC 1.4
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = HELP.KEY% THEN                      \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 CALL SCREEN.HELP(2)                                  ! AJC 1.4
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.GROUP.CODE%                    ! AJC 1.4
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              BEGIN                                                   ! AJC 1.4

                 IF VALID.GROUP.CODE THEN                            \! AJC 1.4
                    VALID.GROUP.CODE.FOUND = TRUE                    \! AJC 1.4
                 ELSE                                                \! AJC 1.4
                 BEGIN                                                ! AJC 1.4
                    ! B221 Free format                                ! AJC 1.4
                    DISPLAY.MESSAGE.NUMBER% = 221                     ! AJC 1.4
                    DISPLAY.MESSAGE.TEXT$ =                          \! AJC 1.4
                    "GROUP CODE MUST BE ENTERED  "                    ! AJC 1.4
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF                                                ! AJC 1.4
              ENDIF                                                   ! AJC 1.4
        WEND                                                          ! AJC 1.4

        IF VALID.GROUP.CODE.FOUND AND                                \! AJC 1.4
           NOT EXIT.KEY.PRESSED(2) THEN                              \! AJC 1.4
        BEGIN                                                         ! AJC 1.4
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = GROUP.CODE$                                 ! AJC 1.4
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% OR                             \ AJC 1.4
              FUNCTION.KEY% = END.KEY% THEN                           \ AJC 1.4
           BEGIN                                                      ! AJC 1.4
             GOSUB GET.EMPLOYEE.FLG.02                                ! AJC 1.4
           ENDIF                                                     \! AJC 1.4
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = HOME.KEY% THEN                         \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB GET.OPERATOR.ID.02                                ! AJC 1.4
           ENDIF                                                     \! AJC 1.4
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = BTAB.KEY% THEN                         \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB GET.RECEIPT.NAME.02                               ! AJC 1.4
           ENDIF                                                     \! AJC 1.4
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = ENTER.KEY% THEN                        \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB CHECK.FIELDS.02                                   ! AJC 1.4
           ENDIF                                                      ! AJC 1.4
        ENDIF                                                         ! AJC 1.4

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.TILL.MODEL.02                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the till model flags on the add an operator screen  *
\***                                                                          *
\******************************************************************************

        GET.TILL.MODEL.02:

        CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(TILL.PTR%)
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)    = FALSE
        VALID.MODEL.FLAG.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.MODEL.FLAG.FOUND)

              MODEL.FLAG$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(TILL.PTR%)
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 STRING.DATA$ = MODEL.FLAG$
                 CALL SET.FIELD                                        ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.MODEL.FLAG THEN                              \
                    VALID.MODEL.FLAG.FOUND = TRUE                      \
                 ELSE                                                  \
                 BEGIN
                    ! B359 Invalid model flag
                    DISPLAY.MESSAGE.NUMBER% = 359
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.MODEL.FLAG.FOUND AND                                  \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           TILL.MODEL.FLAG$(TILL.PTR%) = MODEL.FLAG$

           STRING.DATA$ = MODEL.FLAG$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              IF TILL.PTR% = MAX.TILL.PTR% THEN                        \
              BEGIN
                 CTLR.PTR% = 1
                 GOSUB GET.CTLR.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = TILL.PTR% + 1
                 GOTO GET.TILL.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              IF TILL.PTR% = 1 THEN                                    \
              BEGIN
                 GOSUB GET.OPERATOR.ID.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = 1
                 GOTO GET.TILL.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              IF TILL.PTR% = 1 THEN                                    \
              BEGIN
                 GOSUB GET.EMPLOYEE.FLG.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = TILL.PTR% - 1
                 GOTO GET.TILL.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              IF TILL.PTR% = MAX.TILL.PTR% THEN                        \
              BEGIN
                 CTLR.PTR% = MAX.CTLR.PTR%
                 GOSUB GET.CTLR.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = MAX.TILL.PTR%
                 GOTO GET.TILL.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.CTLR.MODEL.02                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the ctlr model flags on the add an operator screen  *
\***                                                                          *
\******************************************************************************

        GET.CTLR.MODEL.02:

        CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(CTLR.PTR%)
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(2)    = FALSE
        VALID.MODEL.FLAG.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(2) OR                              \
                  VALID.MODEL.FLAG.FOUND)

              MODEL.FLAG$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(2)
                 CALL RESTORE.FIELDS.02                                ! 1.5 RC

                 CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(CTLR.PTR%)
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 STRING.DATA$ = MODEL.FLAG$
                 CALL SET.FIELD                                        ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.MODEL.FLAG THEN                              \
                    VALID.MODEL.FLAG.FOUND = TRUE                      \
                 ELSE                                                  \
                 BEGIN
                    ! B359 Invalid model flag
                    DISPLAY.MESSAGE.NUMBER% = 359
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.MODEL.FLAG.FOUND AND                                  \
           NOT EXIT.KEY.PRESSED(2) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           CTLR.MODEL.FLAG$(CTLR.PTR%) = MODEL.FLAG$

           STRING.DATA$ = MODEL.FLAG$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              IF CTLR.PTR% = MAX.CTLR.PTR% THEN                        \
              BEGIN
                 ! B001 Invalid key pressed
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 GOTO GET.CTLR.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = CTLR.PTR% + 1
                 GOTO GET.CTLR.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              IF CTLR.PTR% = 1 THEN                                    \
              BEGIN
                 TILL.PTR% = 1
                 GOSUB GET.TILL.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = 1
                 GOTO GET.CTLR.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              IF CTLR.PTR% = 1 THEN                                    \
              BEGIN
                 TILL.PTR% = MAX.TILL.PTR%
                 GOSUB GET.TILL.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = CTLR.PTR% - 1
                 GOTO GET.CTLR.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              IF CTLR.PTR% = MAX.CTLR.PTR% THEN                        \
              BEGIN
                 ! B001 Invalid key pressed
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 GOTO GET.CTLR.MODEL.02
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = MAX.CTLR.PTR%
                 GOTO GET.CTLR.MODEL.02
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.02
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHECK.FIELDS.02                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Validate all input fields for the add operator screen                 *
\***                                                                          *
\******************************************************************************

        CHECK.FIELDS.02:

        IF NOT VALID.OPERATOR.ID THEN                                  \
        BEGIN
           ! B058 Invalid operator ID
           DISPLAY.MESSAGE.NUMBER% = 58
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.OPERATOR.ID.02

           GOTO CHECK.FIELDS.02.FAILED
        ENDIF

        IF NOT VALID.NAME THEN                                         \
        BEGIN
           ! B358 Invalid name
           DISPLAY.MESSAGE.NUMBER% = 358
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.NAME.02

           GOTO CHECK.FIELDS.02.FAILED
        ENDIF

        IF NOT VALID.PASSWORD THEN                                     \
        BEGIN
           ! B332 Invalid password
           DISPLAY.MESSAGE.NUMBER% = 332
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.PASSWORD.02

           GOTO CHECK.FIELDS.02.FAILED
        ENDIF

        IF NOT VALID.STAFF.NO(TRUE) THEN                               \ 1.9 NM
        BEGIN
           ! B332 Free format
           DISPLAY.MESSAGE.NUMBER% = 221

           !Commented as part of Core 2 Release                        ! 1.9 NM
           !DISPLAY.MESSAGE.TEXT$ =                                    \ 1.9 NM
           !  "STAFF NUMBER MUST BE 0 OR GREATER"                      ! 1.9 NM

           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.STAFF.NO.02

           GOTO CHECK.FIELDS.02.FAILED
        ENDIF

        IF NOT VALID.BIRTH.DATE THEN \                                 ! 1.6 RC
        BEGIN                                                          ! 1.6 RC
!          DISPLAY.MESSAGE.NUMBER% set within VALID.BIRTH.DATE         ! 1.6 RC
           CALL DISPLAY.MESSAGE                                        ! 1.6 RC
           GOSUB GET.BIRTH.DATE.02                                     ! 1.6 RC
           GOTO CHECK.FIELDS.02.FAILED                                 ! 1.6 RC
        ENDIF                                                          ! 1.6 RC

        IF NOT VALID.RECEIPT.NAME THEN                                 \ ! AJC
        BEGIN                                                            ! AJC
           ! B332 Free format                                            ! AJC
           DISPLAY.MESSAGE.NUMBER% = 221                                 ! AJC
           DISPLAY.MESSAGE.TEXT$ =                                     \ ! AJC
             "NAME ON RECEIPT MUST BE ENTERED  "                         ! AJC
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC
                                                                         ! AJC
           GOSUB GET.RECEIPT.NAME.02                                     ! AJC
                                                                         ! AJC
           GOTO CHECK.FIELDS.02.FAILED                                   ! AJC
        ENDIF                                                            ! AJC

        IF NOT VALID.GROUP.CODE THEN                                   \ ! AJC 1.4
        BEGIN                                                            ! AJC 1.4
           ! B332 Free format                                            ! AJC 1.4
           DISPLAY.MESSAGE.NUMBER% = 221                                 ! AJC 1.4
           DISPLAY.MESSAGE.TEXT$ =                                     \ ! AJC 1.4
             "GROUP CODE MUST BE ENTERED  "                              ! AJC 1.4
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC
                                                                         ! AJC 1.4
           GOSUB GET.GROUP.CODE.02                                       ! AJC 1.4
                                                                         ! AJC 1.4
           GOTO CHECK.FIELDS.02.FAILED                                   ! AJC 1.4
        ENDIF                                                            ! AJC 1.4

        IF NOT VALID.EMPLOYEE.FLG THEN                                  \
        BEGIN
           ! B064 YOU MUST ONLY TYPE "N" OR "Y"
           DISPLAY.MESSAGE.NUMBER% = 64
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.EMPLOYEE.FLG.02

           GOTO CHECK.FIELDS.02.FAILED
        ENDIF

        I% = 1

        WHILE I% <= MAX.TILL.PTR%

              MODEL.FLAG$ = TILL.MODEL.FLAG$(I%)

              IF NOT VALID.MODEL.FLAG THEN                             \
              BEGIN
                 ! B359 Invalid model flag
                 DISPLAY.MESSAGE.NUMBER% = 359
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC

                 TILL.PTR% = I%
                 GOSUB GET.TILL.MODEL.02

                 GOTO CHECK.FIELDS.02.FAILED
              ENDIF

              I% = I% + 1

        WEND

        I% = 1

        WHILE I% <= MAX.CTLR.PTR%

              MODEL.FLAG$ = CTLR.MODEL.FLAG$(I%)

              IF NOT VALID.MODEL.FLAG THEN                             \
              BEGIN
                 ! B359 Invalid model flag
                 DISPLAY.MESSAGE.NUMBER% = 359
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC

                 CTLR.PTR% = I%
                 GOSUB GET.CTLR.MODEL.02

                 GOTO CHECK.FIELDS.02.FAILED
              ENDIF

              I% = I% + 1

        WEND

        GOSUB ADD.AN.OPERATOR

        CHECK.FIELDS.02.FAILED:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       ADD.AN.OPERATOR                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Add an operator to the authorisation files                            *
\***                                                                          *
\******************************************************************************

        ADD.AN.OPERATOR:

        CSOUF.OPERATION$ = "ADD"

        CALL REVEAL.CONFIRM.MESSAGE                                    ! 1.5 RC

        CALL GET.CONFIRM                                               ! 1.5 RC

        CALL HIDE.CONFIRM.MESSAGE                                      ! 1.5 RC

        IF CONFIRM$ = "Y" THEN                                         \
        BEGIN
           CALL WAIT.MESSAGE                                           ! 1.5 RC
           GOSUB UPDATE.AUTH.RECORDS
        ENDIF

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       UPDATE.AUTH.RECORDS                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Write authorisation records to the EALAUTH and ADXCSOUF files         *
\***                                                                          *
\******************************************************************************

        UPDATE.AUTH.RECORDS:

        AF.OPERATOR.NO$      = PACK$(RIGHT$(STRING$(8,"0") +           \
                               OPERATOR.ID$,8))

        AF.PASSWORD$         = PACK$(RIGHT$(STRING$(8,"0") +           \
                               OPERATOR.PASSWORD$,8))

        AF.OPERATOR.NAME$    = OPERATOR.NAME$

        AF.RECEIPT.NAME$     = RECEIPT.NAME$                           ! AJC

!       AF.RESERVED$         = PACK$(STRING$(24,"0"))                  ! AJC

        IF CSOUF.OPERATION$ = "ADD" THEN                               \
        BEGIN
           AF.DATE.PSWD.CHANGE$ = PACK$(TODAYS.DATE$)
        ENDIF

        AF.STAFF.NUM$        = PACK$(RIGHT$(STRING$(8,"0") +           \
                               STAFF.NO$,8))

        AF.BIRTH.DATE$ = DDMCYY.HEX.FROM.DDMMCCYY$ (BIRTH.DATE$)       ! 1.8 RC

        IF EMPLOYEE.FLG$ = "Y" THEN BEGIN
           AF.EMPLOYEE.FLAG$ = PACK$("00")
        ENDIF ELSE BEGIN
           AF.EMPLOYEE.FLAG$ = PACK$("01")
        ENDIF

        IF GROUP.CODE$         = " " THEN BEGIN                        ! AJC 1.4
           AF.GROUP.CODE$      = ""                                    ! AJC 1.4
        ENDIF ELSE BEGIN                                               ! AJC 1.4
           AF.GROUP.CODE$      = PACK$(GROUP.CODE$)                    ! AJC 1.4
        ENDIF                                                          ! AJC 1.4

        CSOUF.OP.ID$         = LEFT$(OPERATOR.ID$ +                    \
                               STRING$(8," "),8)

        CSOUF.FILLER.01$     = " "
        CSOUF.PSWD$          = "********"
        CSOUF.FILLER.02$     = " "

        GOSUB SET.CSOUF.IDS
        GOSUB SET.AF.OPTIONS.KEY
        GOSUB SET.AF.FLAGS
        GOSUB SET.CSOUF.FLAGS
        GOSUB SET.AF.TILL.MODEL.FLAGS
        GOSUB SET.AF.CTLR.MODEL.FLAGS
        GOSUB SET.OLD.MODEL.INFO

        CALL GET.CSOUF.RECORD                                          ! 1.5 RC

        IF WRITE.AF.ABREV = 0 THEN                                     \
        BEGIN
           IF WRITE.CSOUF.ABREV = 0 THEN                               \
           BEGIN
              GOSUB SET.CSOUF.PASSWORD
              CALL SET.NEW.OPAUD.DETAILS                               ! 1.5 RC
              CALL WRITE.OPAUD.RECORDS                                 ! 1.5 RC
           ENDIF                                                       \
           ELSE                                                        \
           BEGIN
              GOSUB FILE.ERROR
           ENDIF
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        CALL GET.QUIT.KEY                                              ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.CSOUF.PASSWORD                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Write the encrypted password to the ADXCSOUF file                     *
\***                                                                          *
\******************************************************************************

        SET.CSOUF.PASSWORD:

        IF ADXAUTH(8,CSOUF.OP.ID$,                                     \
                   OPERATOR.PASSWORD$,"") = 0 THEN                     \
        BEGIN
           IF CSOUF.OPERATION$ = "ADD" THEN                            \
           BEGIN
              ! B178 Operator details successfully added
              DISPLAY.MESSAGE.NUMBER% = 178
              CALL DISPLAY.MESSAGE                                     ! 1.5 RC
           ENDIF                                                       \
           ELSE                                                        \
           IF CSOUF.OPERATION$ = "CHANGE" THEN                         \
           BEGIN
              IF VAL(OPERATOR.ID$) = 905 THEN                          \
              BEGIN
                 GOSUB UPDATE.PPDF.RECORD
              ENDIF

              ! B179 Operator details successfully changed
              DISPLAY.MESSAGE.NUMBER% = 179
              CALL DISPLAY.MESSAGE                                     ! 1.5 RC
           ENDIF
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.CSOUF.IDS:                                *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Combine the user and group IDs from the selected models               *
\***                                                                          *
\******************************************************************************

        SET.CSOUF.IDS:

        CSOUF.USER.ID$  = PACK$("01")
        CSOUF.GROUP.ID$ = PACK$("03")

        FOR I% = 1 TO MAX.CTLR.PTR%

            IF CTLR.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               GROUP.ID$ = MID$(CTLR.MODEL.RECORD$(I%),48,1)

               IF VAL(UNPACK$(GROUP.ID$)) <                            \
                  VAL(UNPACK$(CSOUF.GROUP.ID$)) THEN                   \
               BEGIN
                  CSOUF.GROUP.ID$ = GROUP.ID$
               ENDIF

            ENDIF

        NEXT I%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.AF.OPTIONS.KEY                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Combine the options authorisation key from the selected models        *
\***                                                                          *
\******************************************************************************

        SET.AF.OPTIONS.KEY:

        AF.OPTIONS.KEY$ = PACK$("01")

        FOR I% = 1 TO MAX.CTLR.PTR%

            IF CTLR.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               OPTIONS.KEY$ = MID$(CTLR.MODEL.RECORD$(I%),27,1)

               IF VAL(UNPACK$(OPTIONS.KEY$)) >                         \
                  VAL(UNPACK$(AF.OPTIONS.KEY$)) THEN                   \
               BEGIN
                  AF.OPTIONS.KEY$ = OPTIONS.KEY$
               ENDIF
            ENDIF

        NEXT I%

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.AF.FLAGS                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Combine the EALAUTH flags from the selected models                    *
\***                                                                          *
\******************************************************************************

        SET.AF.FLAGS:

        AF.FLAGS.01.12$ = ""
        AF.FLAGS.13.16$ = ""

        FOR I% = 1 TO 19

            INDICATOR% = 0

            FOR J% = 1 TO MAX.TILL.PTR%

                IF TILL.MODEL.FLAG$(J%) = "Y" THEN                     \
                BEGIN
                   AUTH.FLAGS% = ASC(MID$(TILL.MODEL.RECORD$(J%),      \
                                 27 + I%,1))

                   INDICATOR% = INDICATOR% OR AUTH.FLAGS%
                ENDIF

            NEXT J%

            FOR J% = 1 TO MAX.CTLR.PTR%

                IF CTLR.MODEL.FLAG$(J%) = "Y" THEN                     \
                BEGIN
                   AUTH.FLAGS% = ASC(MID$(CTLR.MODEL.RECORD$(J%),      \
                                 27 + I%,1))

                   INDICATOR% = INDICATOR% OR AUTH.FLAGS%
                ENDIF

            NEXT J%

            IF I% >= 1 AND                                             \
               I% <= 15 THEN                                           \
            BEGIN
               AF.FLAGS.01.12$ = AF.FLAGS.01.12$ + CHR$(INDICATOR%)
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               AF.FLAGS.13.16$ = AF.FLAGS.13.16$ + CHR$(INDICATOR%)
            ENDIF

        NEXT I%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.CSOUF.FLAGS                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Combine the ADXCSOUF flags from the selected models                   *
\***                                                                          *
\******************************************************************************

        SET.CSOUF.FLAGS:

        CSOUF.FLAGS$ = ""

        FOR I% = 1 TO 14


            INDICATOR% = 0

            FOR J% = 1 TO MAX.TILL.PTR%

                IF TILL.MODEL.FLAG$(J%) = "Y" THEN                     \
                BEGIN
                   AUTH.FLAGS% = ASC(MID$(TILL.MODEL.RECORD$(J%),      \
                                 48 + I%,1))

                   INDICATOR% = INDICATOR% OR AUTH.FLAGS%
                ENDIF

            NEXT J%

            FOR J% = 1 TO MAX.CTLR.PTR%

                IF CTLR.MODEL.FLAG$(J%) = "Y" THEN                     \
                BEGIN
                   AUTH.FLAGS% = ASC(MID$(CTLR.MODEL.RECORD$(J%),      \
                                 48 + I%,1))

                   INDICATOR% = INDICATOR% OR AUTH.FLAGS%
                ENDIF

            NEXT J%

            CSOUF.FLAGS$ = CSOUF.FLAGS$ + CHR$(INDICATOR%)

        NEXT I%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.OLD.MODEL.INFO                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Calculate the best match for the model number and supervisor flag     *
\***                                                                          *
\******************************************************************************

        SET.OLD.MODEL.INFO:

        AF.SUP.FLAG$ = "N"
        AF.OP.MODEL$ = "905"

        FOR I% = 1 TO MAX.TILL.PTR%

            IF TILL.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               MODEL.NUMBER$    = MID$(TILL.MODEL.RECORD$(I%),63,3)
               SUPERVISOR.FLAG$ = MID$(TILL.MODEL.RECORD$(I%),66,1)

               IF SUPERVISOR.FLAG$ = "Y" AND                           \
                  AF.SUP.FLAG$ = "N" THEN                              \
               BEGIN
                  AF.SUP.FLAG$ = "Y"
               ENDIF

               IF MODEL.NUMBER$ = "901" AND                            \
                  AF.OP.MODEL$ <> "901" THEN                           \
               BEGIN
                  AF.OP.MODEL$ = "901"
               ENDIF                                                   \
               ELSE                                                    \
               IF MODEL.NUMBER$ = "909" AND                            \
                  (AF.OP.MODEL$ <> "901" OR                            \
                  AF.OP.MODEL$ <> "909") THEN                          \
               BEGIN
                  AF.OP.MODEL$ = "909"
               ENDIF
            ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.PTR%

            IF CTLR.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               MODEL.NUMBER$    = MID$(CTLR.MODEL.RECORD$(I%),63,3)

               IF MODEL.NUMBER$ = "908" AND                            \
                  AF.OP.MODEL$ <> "908" THEN                           \
               BEGIN
                  AF.OP.MODEL$ = "908"
               ENDIF                                                   \
               ELSE                                                    \
               IF MODEL.NUMBER$ = "904" AND                            \
                  (AF.OP.MODEL$ <> "908" OR                            \
                  AF.OP.MODEL$ <> "904") THEN                          \
               BEGIN
                  AF.OP.MODEL$ = "904"
               ENDIF

            ENDIF

        NEXT I%

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.AF.TILL.MODEL.FLAGS                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the till authorisation flags on EALAUTH                           *
\***                                                                          *
\******************************************************************************

        SET.AF.TILL.MODEL.FLAGS:

        AF.MODEL.FLAGS.1% = 0

        FOR I% = 1 TO MAX.TILL.PTR%

            IF TILL.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               BIT.MASK% = VAL(UNPACK$(MID$(                           \
                           TILL.MODEL.RECORD$(I%),2,1)))

               BIT.MASK% = 2 ^ (BIT.MASK% - 1)

               AF.MODEL.FLAGS.1% = AF.MODEL.FLAGS.1% OR BIT.MASK%
            ENDIF

        NEXT I%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.AF.CTLR.MODEL.FLAGS                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the controller authorisation flags on EALAUTH                     *
\***                                                                          *
\******************************************************************************

        SET.AF.CTLR.MODEL.FLAGS:

        AF.MODEL.FLAGS.2% = 0

        FOR I% = 1 TO MAX.CTLR.PTR%

            IF CTLR.MODEL.FLAG$(I%) = "Y" THEN                         \
            BEGIN
               BIT.MASK% = VAL(UNPACK$(MID$(                           \
                           CTLR.MODEL.RECORD$(I%),2,1)))

               BIT.MASK% = 2 ^ (BIT.MASK% - 1)

               AF.MODEL.FLAGS.2% = AF.MODEL.FLAGS.2% OR BIT.MASK%
            ENDIF

        NEXT I%

        RETURN


\******************************************************************************
\***
\***    SUBROUTINE      :       REQUIRED.OPTION
\***
\******************************************************************************
\***
\***    Select the required option
\***
\******************************************************************************

        REQUIRED.OPTION:

        IF OPTION.ALLOWED(VAL(OPTION$)) THEN                           \
        BEGIN
           IF OPTION$ = "1" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.02
           ENDIF                                                       \
           ELSE                                                        \
           IF OPTION$ = "2" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.03
           ENDIF                                                       \
           ELSE                                                        \
           IF OPTION$ = "3" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.04
           ENDIF                                                       \
           ELSE                                                        \
           IF OPTION$ = "4" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.05
           ENDIF                                                       \
           ELSE                                                        \
           IF OPTION$ = "5" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.06
           ENDIF                                                       \
           ELSE                                                        \
           IF OPTION$ = "6" THEN                                       \
           BEGIN
              GOSUB PROCESS.SCREEN.07
           ENDIF
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           ! B004 No authorisation for this selection
           DISPLAY.MESSAGE.NUMBER% = 4
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOTO END.REQUIRED.OPTION
        ENDIF

        CALL DISPLAY.SCREEN(1)

        GOSUB RESTORE.FIELDS.01

        EXIT.KEY.PRESSED(1) = FALSE

        END.REQUIRED.OPTION:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.03                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the display operator screen                       *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.03:

        S% = 3

        CALL DISPLAY.SCREEN(3)

        OLD.OPERATOR.ID$   = ""
        OPERATOR.ID$       = ""
        OPERATOR.NAME$     = ""
        OPERATOR.PASSWORD$ = ""
        STAFF.NO$          = ""
        EMPLOYEE.FLG$      = ""
        BIRTH.DATE$        = ""                                        ! 1.6 RC
        RECEIPT.NAME$      = ""                                       ! AJC
        GROUP.CODE$        = ""                                       ! AJC 1.4
        MODEL.FLAG$        = ""

        CALL RESET.MODEL.FLAGS                                         ! 1.5 RC

        GOSUB RESTORE.FIELDS.03

        EXIT.KEY.PRESSED(3) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(3)
              GOSUB GET.OPERATOR.ID.03
        WEND

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.03                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields for the display operator screen                  *
\***                                                                          *
\******************************************************************************

        RESTORE.FIELDS.03:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.5 RC

        CURSOR.POSITION% = S3.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        IF VAL(OPERATOR.ID$) > 0 THEN                                  \
        BEGIN
           OPERATOR.ID$ = STR$(VAL(OPERATOR.ID$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.ID$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.ID$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S3.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = OPERATOR.NAME$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S3.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = STAFF.NO$
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S3.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = RECEIPT.NAME$                                  ! AJC
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S3.BIRTH.DATE%                              ! 1.6 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.6 RC

        IF BIRTH.DATE$ <> "" THEN BEGIN \                              ! 1.6 RC
            STRING.DATA$ = MID$(BIRTH.DATE$,1,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,3,2) + "/" + \             ! 1.6 RC
                           MID$(BIRTH.DATE$,5,4)                       ! 1.6 RC
        ENDIF                                                          ! 1.6 RC
        CALL SET.FIELD                                                 ! 1.6 RC

        CURSOR.POSITION% = S3.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = GROUP.CODE$                                    ! AJC 1.4
        CALL SET.FIELD                                                 ! 1.5 RC

        CURSOR.POSITION% = S3.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        STRING.DATA$ = EMPLOYEE.FLG$
        CALL SET.FIELD                                                 ! 1.5 RC

        CALL RESTORE.MODEL.FLAGS                                       ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.FIELDS.03                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear all fields for the display operator screen                      *
\***                                                                          *
\******************************************************************************

        CLEAR.FIELDS.03:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        IF OPERATOR.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S3.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF STAFF.NO$ <> "" THEN BEGIN
           CURSOR.POSITION% = S3.STAFF.NO%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF BIRTH.DATE$ <> "" THEN BEGIN                                ! 1.6 RC
           CURSOR.POSITION% = S3.BIRTH.DATE%                           ! 1.6 RC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.6 RC
           STRING.DATA$ = ""                                           ! 1.6 RC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                          ! 1.6 RC

        IF RECEIPT.NAME$ <> "" THEN                                   \ AJC
        BEGIN                                                         ! AJC
           CURSOR.POSITION% = S3.RECEIPT.NAME%                        ! AJC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC

        IF GROUP.CODE$ <> "" THEN                                     \ AJC 1.4
        BEGIN                                                         ! AJC 1.4
           CURSOR.POSITION% = S3.GROUP.CODE%                          ! AJC 1.4
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC 1.4
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC 1.4

        IF EMPLOYEE.FLG$ <> "" THEN BEGIN
           CURSOR.POSITION% = S3.EMPLOYEE.FLG%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        MODEL.FLAG$ = ""

        CALL CLEAR.MODEL.FLAGS                                         ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.ID.03                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator ID on the display operator screen      *
\***                                                                          *
\******************************************************************************

        GET.OPERATOR.ID.03:

        CURSOR.POSITION% = S3.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(3)     = FALSE
        VALID.OPERATOR.ID.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(3) OR                              \
                  VALID.OPERATOR.ID.FOUND)

              OPERATOR.ID$ = RIGHT$(STRING$(3,"0") +                   \
                             STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(S%) = TRUE
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(3)
                 GOSUB RESTORE.FIELDS.03

                 CURSOR.POSITION% = S3.OPERATOR.ID%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPERATOR.ID THEN                             \
                    VALID.OPERATOR.ID.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B058 Invalid operator ID
                    DISPLAY.MESSAGE.NUMBER% = 58
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPERATOR.ID.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(3) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.ID$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              IF OLD.OPERATOR.ID$ <> OPERATOR.ID$ THEN                 \
              BEGIN
                 AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +       \
                                   OPERATOR.ID$,8))

                 IF READ.AF.ABREV = 0 THEN                             \
                 BEGIN
                    OLD.OPERATOR.ID$ = OPERATOR.ID$

                    CALL GET.OPERATOR.DETAILS                          ! 1.5 RC
                    GOSUB RESTORE.FIELDS.03
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    OLD.OPERATOR.ID$ = ""

                    GOSUB CLEAR.FIELDS.03

                    ! B171 Operator ID not currently in use
                    DISPLAY.MESSAGE.NUMBER% = 171
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                 ENDIF
              ENDIF
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.04                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the change operator screen                        *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.04:

        S% = 4

        CALL DISPLAY.SCREEN(4)

        OLD.OPERATOR.ID$   = ""
        OPERATOR.ID$       = ""
        OPERATOR.NAME$     = ""
        OPERATOR.PASSWORD$ = ""
        STAFF.NO$          = ""
        BIRTH.DATE$        = ""                                       ! 1.6 RC
        EMPLOYEE.FLG$      = ""
        RECEIPT.NAME$      = ""                                       ! AJC
        GROUP.CODE$        = ""                                       ! AJC 1.4
        MODEL.FLAG$        = ""
        CONFIRM$           = "N"

        CALL RESET.MODEL.FLAGS                                         ! 1.5 RC

        CALL RESTORE.FIELDS.04                                         ! 1.5 RC

        EXIT.KEY.PRESSED(4) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(4)
              GOSUB GET.OPERATOR.ID.04
        WEND

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.FIELDS.04                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear all fields for the change operator screen                       *
\***                                                                          *
\******************************************************************************

        CLEAR.FIELDS.04:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        IF OPERATOR.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S4.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF STAFF.NO$ <> "" THEN BEGIN
           CURSOR.POSITION% = S4.STAFF.NO%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF BIRTH.DATE$ <> "" THEN BEGIN                                ! 1.6 RC
           CURSOR.POSITION% = S4.BIRTH.DATE%                           ! 1.6 RC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.6 RC
           STRING.DATA$ = ""                                           ! 1.6 RC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                          ! 1.6 RC

        IF RECEIPT.NAME$ <> "" THEN                                   \ AJC
        BEGIN                                                         ! AJC
           CURSOR.POSITION% = S4.RECEIPT.NAME%                        ! AJC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC

        IF GROUP.CODE$ <> "" THEN                                     \ AJC 1.4
        BEGIN                                                         ! AJC 1.4
           CURSOR.POSITION% = S4.GROUP.CODE%                          ! AJC 1.4
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC 1.4
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC 1.4

        IF EMPLOYEE.FLG$ <> "" THEN BEGIN
           CURSOR.POSITION% = S4.EMPLOYEE.FLG%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        MODEL.FLAG$ = ""

        CALL CLEAR.MODEL.FLAGS                                         ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.ID.04                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator ID on the change operator screen       *
\***                                                                          *
\******************************************************************************

        GET.OPERATOR.ID.04:

        CURSOR.POSITION% = S4.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(4)     = FALSE
        VALID.OPERATOR.ID.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(4) OR                              \
                  VALID.OPERATOR.ID.FOUND)

              OPERATOR.ID$ = RIGHT$(STRING$(3,"0") +                   \
                             STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(S%) = TRUE
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(4)
                 CALL RESTORE.FIELDS.04                                ! 1.5 RC

                 CURSOR.POSITION% = S4.OPERATOR.ID%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPERATOR.ID THEN                             \
                    VALID.OPERATOR.ID.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B058 Invalid operator ID
                    DISPLAY.MESSAGE.NUMBER% = 58
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPERATOR.ID.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(4) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.ID$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% OR                              \
              FUNCTION.KEY% = END.KEY% OR                              \
              FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              IF VAL(OPERATOR.ID$) = 905 THEN                          \
              BEGIN
                 OLD.OPERATOR.ID$ = ""

                 GOSUB CLEAR.FIELDS.04

                 DISPLAY.MESSAGE.NUMBER% = 361
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF VAL(OPERATOR.NUMBER$) = VAL(OPERATOR.ID$) THEN        \
              BEGIN
                 OLD.OPERATOR.ID$ = ""

                 GOSUB CLEAR.FIELDS.04

                 ! B360 An operator cannot change their own details
                 DISPLAY.MESSAGE.NUMBER% = 360
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ <> OPERATOR.ID$ THEN                 \
              BEGIN
                 AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +       \
                                   OPERATOR.ID$,8))

                 IF READ.AF.ABREV = 0 THEN                             \
                 BEGIN
                    OLD.OPERATOR.ID$ = OPERATOR.ID$

                    CALL GET.OPERATOR.DETAILS                          ! 1.5 RC
                    CALL SET.OLD.OPAUD.DETAILS                         ! 1.5 RC
                    CALL RESTORE.FIELDS.04                             ! 1.5 RC

                    IF NOT VALID.AF.STAFF.NUM THEN \                   ! 1.6 RC
\                       Staff number not set so allow user to change   ! 1.6 RC
                        GOSUB GET.STAFF.NO.04 \                        ! 1.6 RC
                    ELSE \                                             ! 1.6 RC
                        IF NOT VALID.DDMCYY (AF.BIRTH.DATE$) THEN \    ! 1.8 RC
\                           Birth Date not set so allow user to change ! 1.6 RC
                            GOSUB GET.BIRTH.DATE.04 \                  ! 1.6 RC
                        ELSE \                                         ! 1.6 RC
                            GOSUB GET.RECEIPT.NAME.04                  ! 1.6 RC

!ajc                TILL.PTR% = 1
!ajc                GOSUB GET.TILL.MODEL.04
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    OLD.OPERATOR.ID$ = ""

                    GOSUB CLEAR.FIELDS.04

                    ! B171 Operator ID not currently in use
                    DISPLAY.MESSAGE.NUMBER% = 171
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ = OPERATOR.ID$ THEN                  \
              BEGIN
                 IF FUNCTION.KEY% = ENTER.KEY% THEN                    \
                 BEGIN
                    GOSUB CHECK.FIELDS.04
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF FUNCTION.KEY% = TAB.KEY% THEN                      \
                 BEGIN
!ajc                TILL.PTR% = 1
!ajc                GOSUB GET.TILL.MODEL.04
                    IF NOT VALID.AF.STAFF.NUM THEN \                   ! 1.6 RC
\                       Staff number not set so allow user to change   ! 1.6 RC
                        GOSUB GET.STAFF.NO.04 \                        ! 1.6 RC
                    ELSE \                                             ! 1.6 RC
                        IF NOT VALID.DDMCYY (AF.BIRTH.DATE$) THEN \    ! 1.8 RC
\                           Birth Date not set so allow user to change ! 1.6 RC
                            GOSUB GET.BIRTH.DATE.04 \                  ! 1.6 RC
                        ELSE \                                         ! 1.6 RC
                            GOSUB GET.RECEIPT.NAME.04                  ! 1.6 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF FUNCTION.KEY% = END.KEY% THEN                      \
                 BEGIN
                    TILL.PTR% = MAX.TILL.PTR%
                    GOSUB GET.TILL.MODEL.04
                 ENDIF
              ENDIF
           ENDIF                                                       \

        ENDIF

        RETURN


\******************************************************************************
\***
\***    SUBROUTINE      :       GET.STAFF.NO.04
\***
\******************************************************************************
\***
\***    Input routine for the staff no on the add an operator screen
\***
\******************************************************************************

        GET.STAFF.NO.04: ! Entire procedure new for Rv 1.6             ! 1.6 RC
                         ! Modified from copy of GET.STAFF.NO.02       ! 1.6 RC

        CURSOR.POSITION% = S4.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD

        EXIT.KEY.PRESSED(4)  = FALSE
        VALID.STAFF.NO.FOUND = FALSE

        CALL GET.INPUT

        WHILE NOT (EXIT.KEY.PRESSED(4) OR \
                  VALID.STAFF.NO.FOUND)

              STAFF.NO$ = RIGHT$(STRING$(8,"0") +             \
                                   STR$(VAL(F03.RETURNED.STRING$)),8)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR \
                 FUNCTION.KEY% = END.KEY% OR \
                 FUNCTION.KEY% = TAB.KEY% OR \
                 FUNCTION.KEY% = BTAB.KEY% OR \
                 FUNCTION.KEY% = HOME.KEY% OR \
                 FUNCTION.KEY% = HELP.KEY% OR \
                 FUNCTION.KEY% = QUIT.KEY% OR \
                 FUNCTION.KEY% = ESC.KEY%) THEN \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE
                 CALL RESUME.INPUT
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = QUIT.KEY% THEN \
              BEGIN
                 CALL GET.QUIT.CONFIRM
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = ESC.KEY% THEN \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = HELP.KEY% THEN \
              BEGIN
                 CALL SCREEN.HELP(4)
                 CALL RESTORE.FIELDS.04

                 CURSOR.POSITION% = S4.STAFF.NO%
                 CALL PUT.CURSOR.IN.FIELD

                 CALL GET.INPUT
              ENDIF \
              ELSE \
              BEGIN
                 IF VALID.STAFF.NO(FALSE)                              \ 1.9 NM
                 THEN BEGIN                                            ! 1.9 NM
                    VALID.STAFF.NO.FOUND = TRUE \
                 ENDIF ELSE BEGIN                                      ! 1.9 NM
                    ! B221 Free format
                    DISPLAY.MESSAGE.NUMBER% = 221

                    !Commented as part of Core 2 Release               ! 1.9 NM
                    !DISPLAY.MESSAGE.TEXT$ =                           \ 1.9 NM
                    !"STAFF NUMBER MUST BE 0 OR GREATER"               ! 1.9 NM

                    CALL DISPLAY.MESSAGE
                    CALL RESUME.INPUT
                 ENDIF
              ENDIF
        WEND

        IF VALID.STAFF.NO.FOUND AND \
           NOT EXIT.KEY.PRESSED(4) THEN \
        BEGIN
           CALL CLEAR.MESSAGE

           STRING.DATA$ = STAFF.NO$
           CALL SET.FIELD

           IF FUNCTION.KEY% = TAB.KEY% THEN \
           BEGIN
              IF NOT VALID.DDMCYY (AF.BIRTH.DATE$) THEN \              ! 1.8 RC
\                 Birth Date not set so allow user to change           ! 1.6 RC
                  GOSUB GET.BIRTH.DATE.04 \                            ! 1.6 RC
              ELSE \                                                   ! 1.6 RC
                  GOSUB GET.RECEIPT.NAME.04                            ! 1.6 RC
           ENDIF \
           ELSE \
           IF (   FUNCTION.KEY% = HOME.KEY% \
               OR FUNCTION.KEY% = BTAB.KEY%) THEN \
           BEGIN
              GOSUB GET.OPERATOR.ID.04
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = END.KEY% THEN \
           BEGIN
              TILL.PTR% = MAX.TILL.PTR%
              GOSUB GET.TILL.MODEL.04
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = ENTER.KEY% THEN \
           BEGIN
              GOSUB CHECK.FIELDS.04
           ENDIF
        ENDIF

        RETURN


\******************************************************************************
\***
\***    SUBROUTINE      :       GET.BIRTH.DATE.04
\***
\******************************************************************************
\***
\***    Input routine for the birth date on the add an operator screen
\***
\******************************************************************************

        GET.BIRTH.DATE.04: ! Entire procedure new for Rv 1.6           ! 1.6 RC
                           ! Modified from copy of GET.PASSWORD.02     ! 1.6 RC

        CURSOR.POSITION% = S4.BIRTH.DATE%
        CALL PUT.CURSOR.IN.FIELD

        EXIT.KEY.PRESSED(4)  = FALSE
        VALID.BIRTH.DATE.FOUND = FALSE

        CALL GET.INPUT

        WHILE NOT (EXIT.KEY.PRESSED(4) OR  \
                  VALID.BIRTH.DATE.FOUND)

              BIRTH.DATE$ = F03.RETURNED.STRING$

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR \
                 FUNCTION.KEY% = END.KEY% OR \
                 FUNCTION.KEY% = TAB.KEY% OR \
                 FUNCTION.KEY% = BTAB.KEY% OR \
                 FUNCTION.KEY% = HOME.KEY% OR \
                 FUNCTION.KEY% = HELP.KEY% OR \
                 FUNCTION.KEY% = QUIT.KEY% OR \
                 FUNCTION.KEY% = ESC.KEY%) THEN \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE
                 CALL RESUME.INPUT
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = QUIT.KEY% THEN \
              BEGIN
                 CALL GET.QUIT.CONFIRM
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = ESC.KEY% THEN \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER
              ENDIF \
              ELSE \
              IF FUNCTION.KEY% = HELP.KEY% THEN \
              BEGIN
                 CALL SCREEN.HELP(4)
                 CALL RESTORE.FIELDS.04
                 CURSOR.POSITION% = S4.BIRTH.DATE%
                 CALL PUT.CURSOR.IN.FIELD
                 CALL GET.INPUT
              ENDIF \
              ELSE \
              BEGIN
                  IF   BIRTH.DATE$ = "00000000" \                      ! 1.6 RC
                    OR VALID.BIRTH.DATE THEN \ ! Validates BIRTH.DATE$   1.6 RC
\                    Change Operator screen allows change of Birth Date  1.6 RC
\                    but does not force it (so BIRTH.DATE$ zero allowed) 1.6 RC
                     VALID.BIRTH.DATE.FOUND = TRUE \
                  ELSE \
                  BEGIN
!                    DISPLAY.MESSAGE.NUMBER% set within VALID.BIRTH.DATE 1.6 RC
                     CALL DISPLAY.MESSAGE
                     CALL RESUME.INPUT
                  ENDIF
              ENDIF
        WEND

        IF VALID.BIRTH.DATE.FOUND AND \
           NOT EXIT.KEY.PRESSED(4) THEN \
        BEGIN
           CALL CLEAR.MESSAGE

           STRING.DATA$ = BIRTH.DATE$
           CALL SET.FIELD

           IF FUNCTION.KEY% = TAB.KEY% THEN \
           BEGIN
              GOSUB GET.RECEIPT.NAME.04
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = HOME.KEY% THEN \
           BEGIN
              GOSUB GET.OPERATOR.ID.04
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = BTAB.KEY% THEN \
           BEGIN
              IF NOT VALID.AF.STAFF.NUM THEN \                         ! 1.6 RC
\                 Staff Number not set so allow user to change         ! 1.6 RC
                  GOSUB GET.STAFF.NO.04 \                              ! 1.6 RC
              ELSE \                                                   ! 1.6 RC
                  GOSUB GET.OPERATOR.ID.04 \                           ! 1.6 RC
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = END.KEY% THEN \
           BEGIN
              TILL.PTR% = MAX.TILL.PTR%
              GOSUB GET.TILL.MODEL.04
           ENDIF \
           ELSE \
           IF FUNCTION.KEY% = ENTER.KEY% THEN \
           BEGIN
              GOSUB CHECK.FIELDS.04
           ENDIF
        ENDIF

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.RECEIPT.NAME.04                     ! AJC *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the receipt name on the change operator screen! AJC *
\***                                                                          *
\******************************************************************************

        GET.RECEIPT.NAME.04:                                          ! AJC

        CURSOR.POSITION% = S4.RECEIPT.NAME%                           ! AJC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(4)  = FALSE                                  ! AJC
        VALID.RECEIPT.NAME.FOUND = FALSE                              ! AJC

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(4) OR                            \! AJC
                  VALID.RECEIPT.NAME.FOUND)                           ! AJC

              RECEIPT.NAME$ = F03.RETURNED.STRING$                    ! AJC

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                  \! AJC
                 FUNCTION.KEY% = END.KEY% OR                         \! AJC
                 FUNCTION.KEY% = TAB.KEY% OR                         \! AJC
                 FUNCTION.KEY% = BTAB.KEY% OR                        \! AJC
                 FUNCTION.KEY% = HOME.KEY% OR                        \! AJC
                 FUNCTION.KEY% = HELP.KEY% OR                        \! AJC
                 FUNCTION.KEY% = QUIT.KEY% OR                        \! AJC
                 FUNCTION.KEY% = ESC.KEY%) THEN                      \! AJC
              BEGIN                                                   ! AJC
                 DISPLAY.MESSAGE.NUMBER% = 1                          ! AJC
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = QUIT.KEY% THEN                      \!
              BEGIN                                                   !
                 EXIT.KEY.PRESSED(S%) = TRUE                          !
              ENDIF                                                  \!
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = ESC.KEY% THEN                       \! AJC
              BEGIN                                                   ! AJC
                 CHAIN.TO.PROG$ = "PSB50"                             ! AJC
                 PSBCHN.MENCON  = "000000"                            ! AJC
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              IF FUNCTION.KEY% = HELP.KEY% THEN                      \! AJC
              BEGIN                                                   ! AJC
                 CALL SCREEN.HELP(4)                                  ! AJC
                 CALL RESTORE.FIELDS.04                                ! 1.5 RC

                 CURSOR.POSITION% = S4.RECEIPT.NAME%                  ! AJC
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                  \! AJC
              ELSE                                                   \! AJC
              BEGIN                                                   ! AJC
                 IF VALID.RECEIPT.NAME THEN                          \! AJC
                    VALID.RECEIPT.NAME.FOUND = TRUE                  \! AJC
                 ELSE                                                \! AJC
                 BEGIN                                                ! AJC
                    ! B221 Free format                                ! AJC
                    DISPLAY.MESSAGE.NUMBER% = 221                     ! AJC
                    DISPLAY.MESSAGE.TEXT$ =                          \! AJC
                    "NAME ON RECEIPT MUST BE ENTERED  "               ! AJC
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF                                                ! AJC
              ENDIF                                                   ! AJC
        WEND                                                          ! AJC

        IF VALID.RECEIPT.NAME.FOUND AND                              \! AJC
           NOT EXIT.KEY.PRESSED(4) THEN                              \! AJC
        BEGIN                                                         ! AJC
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = RECEIPT.NAME$                               ! AJC
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                      \
           BEGIN
!              TILL.PTR% = 1                                          ! AJC
!              GOSUB GET.TILL.MODEL.04                                ! AJC
               GOSUB GET.GROUP.CODE.04                                ! AJC 1.4
           ENDIF                                                 \
           ELSE                                                  \
           IF FUNCTION.KEY% = END.KEY% THEN                      \
           BEGIN
              TILL.PTR% = MAX.TILL.PTR%
              GOSUB GET.TILL.MODEL.04
           ENDIF                                                 \
           ELSE                                                      \! AJC
           IF FUNCTION.KEY% = HOME.KEY% THEN \                         ! 1.6 RC
           BEGIN                                                      ! AJC
              GOSUB GET.OPERATOR.ID.04                                ! AJC
           ENDIF                                                     \! AJC
           ELSE                                                      \! AJC
           IF FUNCTION.KEY% = BTAB.KEY% THEN \                         ! 1.6 RC
           BEGIN                                                       ! 1.6 RC
               IF NOT VALID.DDMCYY (AF.BIRTH.DATE$) THEN \             ! 1.8 RC
\                  Birth Date not set so allow user to change          ! 1.6 RC
                   GOSUB GET.BIRTH.DATE.04 \                           ! 1.6 RC
               ELSE \                                                  ! 1.6 RC
                   IF NOT VALID.AF.STAFF.NUM THEN \                    ! 1.6 RC
\                      Staff number not set so allow user to change    ! 1.6 RC
                       GOSUB GET.STAFF.NO.04 \                         ! 1.6 RC
                   ELSE \                                              ! 1.6 RC
                       GOSUB GET.OPERATOR.ID.04                        ! 1.6 RC
           ENDIF \                                                     ! 1.6 RC
           ELSE \                                                      ! 1.6 RC
           IF FUNCTION.KEY% = ENTER.KEY% THEN                        \! AJC
           BEGIN                                                      ! AJC
              GOSUB CHECK.FIELDS.04                                   ! AJC
           ENDIF                                                      ! AJC
        ENDIF                                                         ! AJC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.GROUP.CODE.04                   ! AJC 1.4 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the dept no. on the change operator screen          *
\***                                                                          *
\******************************************************************************

        GET.GROUP.CODE.04:                                            ! AJC 1.4

        CURSOR.POSITION% = S4.GROUP.CODE%                             ! AJC 1.4
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(4)  = FALSE                                  ! AJC 1.4
        VALID.GROUP.CODE.FOUND = FALSE                                ! AJC 1.4

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(4) OR                            \! AJC 1.4
                  VALID.GROUP.CODE.FOUND)                             ! AJC 1.4

              GROUP.CODE$ = RIGHT$(STRING$(2,"0") +                  \  AJC 1.4
                                   STR$(VAL(F03.RETURNED.STRING$)),2) ! AJC 1.4

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                  \! AJC 1.4
                 FUNCTION.KEY% = END.KEY% OR                         \! AJC 1.4
                 FUNCTION.KEY% = TAB.KEY% OR                         \! AJC 1.4
                 FUNCTION.KEY% = BTAB.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = HOME.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = HELP.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = QUIT.KEY% OR                        \! AJC 1.4
                 FUNCTION.KEY% = ESC.KEY%) THEN                      \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 DISPLAY.MESSAGE.NUMBER% = 1                          ! AJC 1.4
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = QUIT.KEY% THEN                      \!
              BEGIN                                                   !
                 EXIT.KEY.PRESSED(S%) = TRUE                          !
              ENDIF                                                  \!
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = ESC.KEY% THEN                       \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 CHAIN.TO.PROG$ = "PSB50"                             ! AJC 1.4
                 PSBCHN.MENCON  = "000000"                            ! AJC 1.4
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              IF FUNCTION.KEY% = HELP.KEY% THEN                      \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 CALL SCREEN.HELP(4)                                  ! AJC 1.4
                 CALL RESTORE.FIELDS.04                                ! 1.5 RC

                 CURSOR.POSITION% = S4.GROUP.CODE%                    ! AJC 1.4
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                  \! AJC 1.4
              ELSE                                                   \! AJC 1.4
              BEGIN                                                   ! AJC 1.4
                 IF VALID.GROUP.CODE     THEN                        \! AJC 1.4
                    VALID.GROUP.CODE.FOUND = TRUE                    \! AJC 1.4
                 ELSE                                                \! AJC 1.4
                 BEGIN                                                ! AJC 1.4
                    ! B221 Free format                                ! AJC 1.4
                    DISPLAY.MESSAGE.NUMBER% = 221                     ! AJC 1.4
                    DISPLAY.MESSAGE.TEXT$ =                          \! AJC 1.4
                    "GROUP CODE MUST BE ENTERED  "                    ! AJC 1.4
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF                                                ! AJC 1.4
              ENDIF                                                   ! AJC 1.4
        WEND                                                          ! AJC 1.4

        IF VALID.GROUP.CODE.FOUND AND                                \! AJC 1.4
           NOT EXIT.KEY.PRESSED(4) THEN                              \! AJC 1.4
        BEGIN                                                         ! AJC 1.4
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = GROUP.CODE$                                 ! AJC 1.4
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                      \
           BEGIN
              TILL.PTR% = 1
              GOSUB GET.TILL.MODEL.04
           ENDIF                                                 \
           ELSE                                                  \
           IF FUNCTION.KEY% = END.KEY% THEN                      \
           BEGIN
              TILL.PTR% = MAX.TILL.PTR%
              GOSUB GET.TILL.MODEL.04
           ENDIF                                                     \
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = HOME.KEY% THEN                         \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB GET.OPERATOR.ID.04                                ! AJC 1.4
           ENDIF                                                     \! AJC 1.4
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = BTAB.KEY% THEN                         \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB GET.RECEIPT.NAME.04                               ! AJC 1.4
           ENDIF                                                     \! AJC 1.4
           ELSE                                                      \! AJC 1.4
           IF FUNCTION.KEY% = ENTER.KEY% THEN                        \! AJC 1.4
           BEGIN                                                      ! AJC 1.4
              GOSUB CHECK.FIELDS.04                                   ! AJC 1.4
           ENDIF                                                      ! AJC 1.4
        ENDIF                                                         ! AJC 1.4

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.TILL.MODEL.04                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the till model flags on the change operator screen  *
\***                                                                          *
\******************************************************************************

        GET.TILL.MODEL.04:

        CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(TILL.PTR%)
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(4)    = FALSE
        VALID.MODEL.FLAG.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(4) OR                              \
                  VALID.MODEL.FLAG.FOUND)

              MODEL.FLAG$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(4)
                 CALL RESTORE.FIELDS.04                                ! 1.5 RC

                 CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(TILL.PTR%)
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 STRING.DATA$ = MODEL.FLAG$
                 CALL SET.FIELD                                        ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.MODEL.FLAG THEN                              \
                    VALID.MODEL.FLAG.FOUND = TRUE                      \
                 ELSE                                                  \
                 BEGIN
                    ! B359 Invalid model flag
                    DISPLAY.MESSAGE.NUMBER% = 359
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.MODEL.FLAG.FOUND AND                                  \
           NOT EXIT.KEY.PRESSED(4) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           TILL.MODEL.FLAG$(TILL.PTR%) = MODEL.FLAG$

           STRING.DATA$ = MODEL.FLAG$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              IF TILL.PTR% = MAX.TILL.PTR% THEN                        \
              BEGIN
                 CTLR.PTR% = 1
                 GOSUB GET.CTLR.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = TILL.PTR% + 1
                 GOTO GET.TILL.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              IF TILL.PTR% = 1 THEN                                    \
              BEGIN
                 GOSUB GET.OPERATOR.ID.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = 1
                 GOTO GET.TILL.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              IF TILL.PTR% = 1 THEN                                    \
              BEGIN
!                GOSUB GET.OPERATOR.ID.04
!AJC 1.4         GOSUB GET.RECEIPT.NAME.04
                 GOSUB GET.GROUP.CODE.04                               ! AJC 1.4
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = TILL.PTR% - 1
                 GOTO GET.TILL.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              IF TILL.PTR% = MAX.TILL.PTR% THEN                        \
              BEGIN
                 CTLR.PTR% = MAX.CTLR.PTR%
                 GOSUB GET.CTLR.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 TILL.PTR% = MAX.TILL.PTR%
                 GOTO GET.TILL.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.04
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.CTLR.MODEL.04                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the ctlr model flags on the change operator screen  *
\***                                                                          *
\******************************************************************************

        GET.CTLR.MODEL.04:

        CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(CTLR.PTR%)
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(4)    = FALSE
        VALID.MODEL.FLAG.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(4) OR                              \
                  VALID.MODEL.FLAG.FOUND)

              MODEL.FLAG$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(4)
                 CALL RESTORE.FIELDS.04                                ! 1.5 RC

                 CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(CTLR.PTR%)
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 STRING.DATA$ = MODEL.FLAG$
                 CALL SET.FIELD                                        ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.MODEL.FLAG THEN                              \
                    VALID.MODEL.FLAG.FOUND = TRUE                      \
                 ELSE                                                  \
                 BEGIN
                    ! B359 Invalid model flag
                    DISPLAY.MESSAGE.NUMBER% = 359
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.MODEL.FLAG.FOUND AND                                  \
           NOT EXIT.KEY.PRESSED(4) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           CTLR.MODEL.FLAG$(CTLR.PTR%) = MODEL.FLAG$

           STRING.DATA$ = MODEL.FLAG$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% THEN                            \
           BEGIN
              IF CTLR.PTR% = MAX.CTLR.PTR% THEN                        \
              BEGIN
                 ! B001 Invalid key pressed
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 GOTO GET.CTLR.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = CTLR.PTR% + 1
                 GOTO GET.CTLR.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% THEN                           \
           BEGIN
              IF CTLR.PTR% = 1 THEN                                    \
              BEGIN
                 TILL.PTR% = 1
                 GOSUB GET.TILL.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = 1
                 GOTO GET.CTLR.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              IF CTLR.PTR% = 1 THEN                                    \
              BEGIN
                 TILL.PTR% = MAX.TILL.PTR%
                 GOSUB GET.TILL.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = CTLR.PTR% - 1
                 GOTO GET.CTLR.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = END.KEY% THEN                            \
           BEGIN
              IF CTLR.PTR% = MAX.CTLR.PTR% THEN                        \
              BEGIN
                 ! B001 Invalid key pressed
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 GOTO GET.CTLR.MODEL.04
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CTLR.PTR% = MAX.CTLR.PTR%
                 GOTO GET.CTLR.MODEL.04
              ENDIF
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.04
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHECK.FIELDS.04                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Validate all input fields for the change operator screen              *
\***                                                                          *
\******************************************************************************

        CHECK.FIELDS.04:

        IF NOT VALID.OPERATOR.ID THEN                                  \
        BEGIN
           ! B058 Invalid operator ID
           DISPLAY.MESSAGE.NUMBER% = 58
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.OPERATOR.ID.04

           GOTO CHECK.FIELDS.04.FAILED
        ENDIF

        IF NOT VALID.STAFF.NO(FALSE) THEN \                            ! 1.9 NM
        BEGIN                                                          ! 1.6 RC
           ! B332 Free format                                          ! 1.6 RC
           DISPLAY.MESSAGE.NUMBER% = 221                               ! 1.6 RC

           !Commented as part of Core 2 Release                        ! 1.9 NM
           !DISPLAY.MESSAGE.TEXT$ = \                                  ! 1.9 NM
           !  "STAFF NUMBER MUST BE 0 OR GREATER"                      ! 1.9 NM

           CALL DISPLAY.MESSAGE                                        ! 1.6 RC

           GOSUB GET.STAFF.NO.04                                       ! 1.6 RC

           GOTO CHECK.FIELDS.04.FAILED                                 ! !.6 RC
        ENDIF                                                          ! !.6 RC

        IF    (NOT VALID.BIRTH.DATE) \           ! Brackets not needed ! 1.6 RC
          AND (BIRTH.DATE$ <> "00000000") THEN \ ! added for clarity   ! 1.6 RC
\         Change Operator screen allows change of Birth Date           ! 1.6 RC
\         but does not force it (so BIRTH.DATE$ zero allowed)          ! 1.6 RC
        BEGIN                                                          ! 1.6 RC
!          DISPLAY.MESSAGE.NUMBER% set within VALID.BIRTH.DATE         ! 1.6 RC
           CALL DISPLAY.MESSAGE                                        ! 1.6 RC
           GOSUB GET.BIRTH.DATE.04                                     ! 1.6 RC
           GOTO CHECK.FIELDS.04.FAILED                                 ! 1.6 RC
        ENDIF                                                          ! 1.6 RC

        I% = 1

        WHILE I% <= MAX.TILL.PTR%

              MODEL.FLAG$ = TILL.MODEL.FLAG$(I%)

              IF NOT VALID.MODEL.FLAG THEN                             \
              BEGIN
                 ! B359 Invalid model flag
                 DISPLAY.MESSAGE.NUMBER% = 359
                 DISPLAY.MESSAGE.TEXT$   = "Invalid model flag"
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC

                 TILL.PTR% = I%
                 GOSUB GET.TILL.MODEL.04

                 GOTO CHECK.FIELDS.04.FAILED
              ENDIF

              I% = I% + 1

        WEND

        I% = 1

        WHILE I% <= MAX.CTLR.PTR%

              MODEL.FLAG$ = CTLR.MODEL.FLAG$(I%)

              IF NOT VALID.MODEL.FLAG THEN                             \
              BEGIN
                 ! B359 Invalid model flag
                 DISPLAY.MESSAGE.NUMBER% = 359
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC

                 CTLR.PTR% = I%
                 GOSUB GET.CTLR.MODEL.04

                 GOTO CHECK.FIELDS.04.FAILED
              ENDIF

              I% = I% + 1

        WEND

        GOSUB CHANGE.OPERATOR.DETAILS

        CHECK.FIELDS.04.FAILED:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHANGE.OPERATOR.DETAILS                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Change an operator on the authorisation files                         *
\***                                                                          *
\******************************************************************************

        CHANGE.OPERATOR.DETAILS:

        CSOUF.OPERATION$ = "CHANGE"

        CALL REVEAL.CONFIRM.MESSAGE                                    ! 1.5 RC

        CALL GET.CONFIRM                                               ! 1.5 RC

        CALL HIDE.CONFIRM.MESSAGE                                      ! 1.5 RC

        IF CONFIRM$ = "Y" THEN                                         \
        BEGIN
           CALL WAIT.MESSAGE                                           ! 1.5 RC
           GOSUB UPDATE.AUTH.RECORDS
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.05                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the set operator password screen                  *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.05:

        S% = 5

        CALL DISPLAY.SCREEN(5)

        OLD.OPERATOR.ID$   = ""
        OPERATOR.ID$       = ""
        OPERATOR.NAME$     = ""
        OPERATOR.PASSWORD$ = ""
        STAFF.NO$          = ""
        RECEIPT.NAME$      = ""                                       ! AJC
        BIRTH.DATE$        = ""                                        ! 1.6 RC
        GROUP.CODE$        = ""                                       ! AJC 1.4
        EMPLOYEE.FLG$      = ""
        MODEL.FLAG$        = ""
        CONFIRM$           = "N"

        CALL RESET.MODEL.FLAGS                                         ! 1.5 RC

        CALL RESTORE.FIELDS.05                                         ! 1.5 RC

        EXIT.KEY.PRESSED(5) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(5)
              GOSUB GET.OPERATOR.ID.05
        WEND

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.FIELDS.05                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear all fields for the set operator password screen                 *
\***                                                                          *
\******************************************************************************

        CLEAR.FIELDS.05:

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        IF OPERATOR.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S5.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF OPERATOR.PASSWORD$ <> "" THEN \                             ! 1.6 RC
        BEGIN                                                          ! 1.6 RC
           CURSOR.POSITION% = S5.PASSWORD%                             ! 1.6 RC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.6 RC
           STRING.DATA$ = ""                                           ! 1.6 RC
           CALL SET.FIELD                                              ! 1.6 RC
        ENDIF                                                          ! 1.6 RC

        IF STAFF.NO$ <> "" THEN BEGIN
           CURSOR.POSITION% = S5.STAFF.NO%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        IF BIRTH.DATE$ <> "" THEN BEGIN                                ! 1.6 RC
           CURSOR.POSITION% = S5.BIRTH.DATE%                           ! 1.6 RC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.6 RC
           STRING.DATA$ = ""                                           ! 1.6 RC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                          ! 1.6 RC

        IF RECEIPT.NAME$ <> "" THEN                                   \ AJC
        BEGIN                                                         ! AJC
           CURSOR.POSITION% = S5.RECEIPT.NAME%                        ! AJC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC

        IF GROUP.CODE$ <> "" THEN                                     \ AJC 1.4
        BEGIN                                                         ! AJC 1.4
           CURSOR.POSITION% = S5.GROUP.CODE%                          ! AJC 1.4
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""                                          ! AJC 1.4
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF                                                         ! AJC 1.4

        IF EMPLOYEE.FLG$ <> "" THEN BEGIN
           CURSOR.POSITION% = S5.EMPLOYEE.FLG%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.5 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.5 RC
        ENDIF

        MODEL.FLAG$ = ""

        CALL CLEAR.MODEL.FLAGS                                         ! 1.5 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.ID.05                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator ID on the set password screen          *
\***                                                                          *
\******************************************************************************

        GET.OPERATOR.ID.05:

        CURSOR.POSITION% = S5.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(5)     = FALSE
        VALID.OPERATOR.ID.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(5) OR                              \
                  VALID.OPERATOR.ID.FOUND)

              OPERATOR.ID$ = RIGHT$(STRING$(3,"0") +                   \
                             STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = TAB.KEY% OR                           \
                 FUNCTION.KEY% = END.KEY% OR                           \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(S%) = TRUE
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(5)
                 CALL RESTORE.FIELDS.05                                ! 1.5 RC

                 CURSOR.POSITION% = S5.OPERATOR.ID%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPERATOR.ID THEN                             \
                    VALID.OPERATOR.ID.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B058 Invalid operator ID
                    DISPLAY.MESSAGE.NUMBER% = 58
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPERATOR.ID.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(5) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.ID$
           CALL SET.FIELD                                              ! 1.5 RC

           IF FUNCTION.KEY% = TAB.KEY% OR                              \
              FUNCTION.KEY% = END.KEY% OR                              \
              FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN

              IF OLD.OPERATOR.ID$ <> OPERATOR.ID$ THEN                 \
              BEGIN
                 AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +       \
                                   OPERATOR.ID$,8))

                 IF READ.AF.ABREV = 0 THEN                             \
                 BEGIN
                    OLD.OPERATOR.ID$ = OPERATOR.ID$
                    OLD.PASSWORD$    = RIGHT$(STRING$(3,"0") +         \
                                       UNPACK$(AF.PASSWORD$),3)

                    CALL GET.OPERATOR.DETAILS                          ! 1.5 RC
                    CALL SET.OLD.OPAUD.DETAILS                         ! 1.5 RC

                    OPERATOR.PASSWORD$ = ""

                    CALL RESTORE.FIELDS.05                             ! 1.5 RC

                    GOSUB GET.PASSWORD.05
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    OLD.OPERATOR.ID$ = ""

                    GOSUB CLEAR.FIELDS.05

                    ! B171 Operator ID not currently in use
                    DISPLAY.MESSAGE.NUMBER% = 171
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ = OPERATOR.ID$ THEN                  \
              BEGIN
                 IF FUNCTION.KEY% = ENTER.KEY% THEN                    \
                 BEGIN
                    GOSUB CHECK.FIELDS.05
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF FUNCTION.KEY% = TAB.KEY% OR                        \
                    FUNCTION.KEY% = END.KEY% THEN                      \
                 BEGIN
                    GOSUB GET.PASSWORD.05
                 ENDIF
              ENDIF
           ENDIF                                                       \
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.PASSWORD.05                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the password on the set password screen             *
\***                                                                          *
\******************************************************************************

        GET.PASSWORD.05:

        CURSOR.POSITION% = S5.PASSWORD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.5 RC

        EXIT.KEY.PRESSED(5)  = FALSE
        VALID.PASSWORD.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.5 RC

        WHILE NOT (EXIT.KEY.PRESSED(5) OR                              \
                  VALID.PASSWORD.FOUND)

              OPERATOR.PASSWORD$ = RIGHT$(STRING$(3,"0") +             \
                                   STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = BTAB.KEY% OR                          \
                 FUNCTION.KEY% = HOME.KEY% OR                          \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.5 RC
                 CALL RESUME.INPUT                                     ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(5)
                 CALL RESTORE.FIELDS.05                                ! 1.5 RC

                 CURSOR.POSITION% = S5.PASSWORD%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.5 RC

                 CALL GET.INPUT                                        ! 1.5 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.PASSWORD THEN                                \
                    VALID.PASSWORD.FOUND = TRUE                        \
                 ELSE                                                  \
                 BEGIN
                    ! B332 Invalid password
                    DISPLAY.MESSAGE.NUMBER% = 332
                    CALL DISPLAY.MESSAGE                               ! 1.5 RC
                    CALL RESUME.INPUT                                  ! 1.5 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.PASSWORD.FOUND AND                                    \
           NOT EXIT.KEY.PRESSED(5) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.5 RC

           STRING.DATA$ = OPERATOR.PASSWORD$
           CALL SET.FIELD                                              ! 1.5 RC

           IF OLD.PASSWORD$ = OPERATOR.PASSWORD$ THEN                  \
           BEGIN
              DISPLAY.MESSAGE.NUMBER% = 363
              CALL DISPLAY.MESSAGE                                     ! 1.5 RC

              GOTO GET.PASSWORD.05
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = HOME.KEY% OR                             \
              FUNCTION.KEY% = BTAB.KEY% THEN                           \
           BEGIN
              GOSUB GET.OPERATOR.ID.05
           ENDIF                                                       \
           ELSE                                                        \
           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              GOSUB CHECK.FIELDS.05
           ENDIF
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHECK.FIELDS.05                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Validate all input fields for the set password screen                 *
\***                                                                          *
\******************************************************************************

        CHECK.FIELDS.05:

        IF NOT VALID.OPERATOR.ID THEN                                  \
        BEGIN
           ! B058 Invalid operator ID
           DISPLAY.MESSAGE.NUMBER% = 58
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.OPERATOR.ID.05

           GOTO CHECK.FIELDS.05.FAILED
        ENDIF

        IF NOT VALID.PASSWORD THEN                                     \
        BEGIN
           ! B332 Invalid password
           DISPLAY.MESSAGE.NUMBER% = 332
           CALL DISPLAY.MESSAGE                                        ! 1.5 RC

           GOSUB GET.PASSWORD.05

           GOTO CHECK.FIELDS.05.FAILED
        ENDIF

        GOSUB SET.OPERATOR.PASSWORD

        CHECK.FIELDS.05.FAILED:

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.OPERATOR.PASSWORD                         *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the password on the authorisation files                           *
\***                                                                          *
\******************************************************************************

        SET.OPERATOR.PASSWORD:

        CSOUF.OPERATION$ = "CHANGE"

        CALL REVEAL.CONFIRM.MESSAGE                                    ! 1.5 RC

        CALL GET.CONFIRM                                               ! 1.5 RC

        CALL HIDE.CONFIRM.MESSAGE                                      ! 1.5 RC

        IF CONFIRM$ = "Y" THEN                                         \
        BEGIN
           CALL WAIT.MESSAGE                                           ! 1.5 RC
           GOSUB CHANGE.PASSWORD.DETAILS
        ENDIF

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHANGE.PASSWORD.DETAILS                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Write the password to the EALAUTH and ADXCSOUF files                  *
\***                                                                          *
\******************************************************************************

        CHANGE.PASSWORD.DETAILS:

        AF.OPERATOR.NO$      = PACK$(RIGHT$(STRING$(8,"0") +           \
                               OPERATOR.ID$,8))

        AF.PASSWORD$         = PACK$(RIGHT$(STRING$(8,"0") +           \
                               OPERATOR.PASSWORD$,8))

        AF.DATE.PSWD.CHANGE$ = PACK$(TODAYS.DATE$)

        CSOUF.OP.ID$         = LEFT$(OPERATOR.ID$ +                    \
                               STRING$(8," "),8)

        CSOUF.PSWD$          = "********"

        IF WRITE.AF.ABREV = 0 THEN                                     \
        BEGIN
           GOSUB SET.CSOUF.PASSWORD
           CALL SET.NEW.OPAUD.DETAILS                                  ! 1.5 RC
           CALL WRITE.OPAUD.RECORDS                                    ! 1.5 RC
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        CALL GET.QUIT.KEY                                              ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       UPDATE.PPDF.RECORD                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Update the pharmacy password details file                             *
\***                                                                          *
\******************************************************************************

        UPDATE.PPDF.RECORD:

        CURRENT.REPORT.NUM% = PPDF.REPORT.NUM%
        IF END # PPDF.SESS.NUM% THEN OPEN.ERROR
        OPEN PPDF.FILE.NAME$ DIRECT RECL PPDF.RECL% AS PPDF.SESS.NUM%

        PPDF.REC.NUM% = 1

        IF READ.PPDF = 0 THEN                                          \
        BEGIN
           IF PPDF.INTERMEDIATE.FLAG$ = "Y" THEN                       \
           BEGIN
              GOSUB SET.PHRML.PASSWORD
           ENDIF

           GOSUB SET.PPDF.DATE                                         ! HMW
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        CLOSE PPDF.SESS.NUM%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.PHRML.PASSWORD                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Update the password on the PHRML file                                 *
\***                                                                          *
\******************************************************************************

        SET.PHRML.PASSWORD:

        CURRENT.REPORT.NUM% = PHRML.REPORT.NUM%
        IF END # PHRML.SESS.NUM% THEN OPEN.ERROR
        OPEN PHRML.FILE.NAME$ DIRECT RECL PHRML.RECL% AS               \
             PHRML.SESS.NUM%

        PHRML.PASSWORD$ = LEFT$(OPERATOR.PASSWORD$ +                   \
                          PACK$(STRING$(18,"0")),9)

        IF WRITE.PHRML.PASSWORD <> 0 THEN                              \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        CLOSE PHRML.SESS.NUM%

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.PPDF.DATE                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Update the date of last password change on the PPDF file              *
\***                                                                          *
\******************************************************************************

        SET.PPDF.DATE:

        PPDF.DATE.LAST.PSWD$ = PACK$(TODAYS.DATE$)

        IF WRITE.PPDF <> 0 THEN                                        \
        BEGIN
           GOSUB FILE.ERROR
        ENDIF

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.06                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the delete an operator screen                     *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.06:

        CALL PSB9902 ! Processing separated into module PSB9902        ! 1.5 RC

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHECK.FIELDS.06                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Validate all input fields for the delete an operator screen           *
\***                                                                          *
\******************************************************************************

!   Entire procedure neutralised for Rv 1.5                            ! 1.5 RC
!   VALID.OPERATOR.ID function is TRUE when OPERATOR.ID$ is from 100 to 999
!   CHECK.FIELDS.06 only called from end of GET.OPERATOR.ID.06 routine
!     when VALID.OPERATOR.ID.FOUND is TRUE
!   VALID.OPERATOR.ID.FOUND is set FALSE at start of GET.OPERATOR.ID.06
!     and set TRUE witin it only when VALID.OPERATOR.ID is TRUE
!   Similar code exists within GET.OPERATOR.ID... 02 03 04 05 routines
!   Therefore the IF NOT VALID.OPERATOR.ID path below will never execute
!   This means a CALL to this subroutine can be replaced by
!     a CALL to DELETE.AN.OPERATOR


!       CHECK.FIELDS.06: ! Entire procedure neutralised for Rv 1.5     ! 1.5 RC

!       IF NOT VALID.OPERATOR.ID THEN                                  \
!       BEGIN
!          ! B058 Invalid operator ID
!          DISPLAY.MESSAGE.NUMBER% = 58
!          GOSUB DISPLAY.MESSAGE

!          GOSUB GET.OPERATOR.ID.06

!          GOTO CHECK.FIELDS.06.FAILED
!       ENDIF

!       GOSUB DELETE.AN.OPERATOR

!       CHECK.FIELDS.06.FAILED:

!       RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.07                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the report operators screen                       *
\***                                                                          *
\******************************************************************************

        PROCESS.SCREEN.07:

        CALL PSB9901

        RETURN


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       TERMINATION                                   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Deallocate all session numbers                                        *
\***                                                                          *
\***    CLOSE the required files                                              *
\***                                                                          *
\***    CHAIN back to a previous screen program                               *
\***                                                                          *
\******************************************************************************

        TERMINATION:

        CALL CHAIN.TO.CALLER                                           ! 1.5 RC

        RETURN

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       ALLOCATE.SESS.NUMS                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Allocate all session numbers                                          *
\***                                                                          *
\******************************************************************************

        ALLOCATE.SESS.NUMS:

        SB.ACTION$ = "O"

        SB.INTEGER% = MODEL.REPORT.NUM%
        SB.STRING$ = MODEL.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        MODEL.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = CSOUF.REPORT.NUM%
        SB.STRING$ = CSOUF.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        CSOUF.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = AF.REPORT.NUM%
        SB.STRING$ = AF.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        AF.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = OPAUD.REPORT.NUM%
        SB.STRING$ = OPAUD.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        OPAUD.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = PRINT.REPORT.NUM%
        SB.STRING$ = PRINT.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        PRINT.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = PPDF.REPORT.NUM%
        SB.STRING$ = PPDF.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        PPDF.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = PHRML.REPORT.NUM%
        SB.STRING$ = PHRML.FILE.NAME$
        CALL SB.FILE.UTILS                                             ! 1.5 RC
        PHRML.SESS.NUM% = SB.FILE.SESS.NUM%

        SB.INTEGER% = WORKFILE.REPORT.NUM%                             ! 1.9 NM
        SB.STRING$ = WORKFILE.FILE.NAME$                               ! 1.9 NM
        CALL SB.FILE.UTILS                                             ! 1.9 NM
        WORKFILE.SESS.NUM% = SB.FILE.SESS.NUM%                         ! 1.9 NM

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
                           CURRENT.CODE$
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
        CALL CHAIN.TO.CALLER                                           ! 1.5 RC

\******************************************************************************
\***                                                                          *
\***    ERROR ROUTINE   :       ERROR.DETECTED                                *
\***                                                                          *
\******************************************************************************

        ERROR.DETECTED:

!       Resolve "NP" error to facilitate program running from          ! 1.5 RC
!       debugger or command line                                       ! 1.5 RC

        IF ERR = "NP" THEN \ ! No Parameters passed on CHAIN           ! 1.5 RC
            BEGIN            ! when USE statement executed             ! 1.5 RC
            PSBCHN.PRG    = ""                                         ! 1.5 RC
            PSBCHN.OP     = "99999999" ! Parameters in use for A9C     ! 1.5 RC
            PSBCHN.APP    = "PSB50"    ! (though only PSBCHN.APP       ! 1.5 RC
            PSBCHN.MENCON = "413000"   !  used within program)         ! 1.5 RC
            PSBCHN.U1     = ""                                         ! 1.5 RC
            PSBCHN.U2     = ""                                         ! 1.5 RC
            PSBCHN.U3     = ""                                         ! 1.5 RC
            RESUME RESUME.FROM.NP.ERROR                                ! 1.5 RC
            ENDIF                                                      ! 1.5 RC

!   PRINT "MAIN: ERROR.DETECTED at " + \
!      MID$(TIME$,1,2) + ":" + MID$(TIME$,3,2) + ":" + MID$(TIME$,5,2)
!   PRINT "ERRN .... " + ERRNH ! Function call to translate ERRN
!   PRINT "ERRF% ... " + STR$(ERRF%)
!   PRINT "ERR ..... " + ERR
!   PRINT "ERRL .... " + STR$(ERRL)

        CALL STANDARD.ERROR.DETECTED(ERRN,                             \
                                     ERRF%,                            \
                                     ERRL,                             \
                                     ERR)

        CHAIN.TO.PROG$ = "PSB50"
        CALL CHAIN.TO.CALLER                                           ! 1.5 RC

        END

