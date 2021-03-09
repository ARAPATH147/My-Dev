
\******************************************************************************
\******************************************************************************
\***
\***    PROGRAM         :    PSB99
\***    AUTHOR          :    Mark Walker
\***    DATE WRITTEN    :    March 4th 1994
\***
\******************************************************************************
\******************************************************************************


\*****************************************************************************
\*****************************************************************************
\***
\***    MODULE PSB9902
\***    This module handles option 6 "Delete an Operator".
\***
\***    REVISION 1.0.                ROBERT COWEY.                11 JUN 2009.
\***    Original revison created by separation of PROCESS.SCREEN.06 (and its
\***    numerous subroutines) from module PSB9900.
\***    Removed revision initials from transferred code (except for Rv 1.5 RC
\***    retained as 1.0 RC to indicate subroutines converted to subprograms).
\***
\***    REVISION 1.1.                ROBERT COWEY.                15 JUN 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.6.
\***    Defined operator Birth Date.
\***    Add Operator acreen ...
\***      Forced setting of Birth Date.
\***      Continued to allow zeros to be entered for Staff Number.
\***    Change Operator screen ...
\***      Allowed (but not forced) setting of Birth Date.
\***      Prevented access to Birth Date variable if already set.
\***      Allowed (but not forced) setting of Staff Number.
\***      Prevented access to Staff Number variable if already set.
\***
\***    REVISION 1.2.                ROBERT COWEY.                17 JUL 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.7.
\***    Bug fix for defect 3247.
\***    Corrected VALID.YYMMDD function to reject day component less than 1.
\***
\***    REVISION 1.3.                ROBERT COWEY.                22 JUL 2009.
\***    Changes for A9C POS Improvements project creating PSB99.286 Rv 1.8.
\***    Defect 3247 - Redefined AF.BIRTH.DATE$ format as UPD-hex DDMCYY.
\***    Coded functions to handle DDMCYY date formats ...
\***      DDMCYY.HEX.FROM.DDMMCCYY$ (DDMMCCYY$)
\***      DDMMCCYY.FROM.DDMCYY.HEX$ (DDMCYY.HEX$)
\***      YYMMDD.FROM.DDMCYY.HEX$ (DDMCYY.HEX$)
\***      VALID.DDMCYY (DDMCYY.HEX$)
\***
\*****************************************************************************
\*****************************************************************************



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


!       %INCLUDE MODELDEC.J86
        %INCLUDE CSOUFDEC.J86
        %INCLUDE AFDEC.J86
        %INCLUDE OPAUDDEC.J86
!       %INCLUDE PRINTDEC.J86
!       %INCLUDE PPDFDEC.J86
!       %INCLUDE PHRMLDEC.J86

        %INCLUDE PSBF03G.J86   ! Display Manager
        %INCLUDE PSBF20G.J86   ! Session Number Utility

        %INCLUDE PSBUSEG.J86

        STRING GLOBAL           BATCH.SCREEN.FLAG$,                    \
                                CHAIN.TO.PROG$,                        \
                                CURSOR.STATE$,                         \
                                INVISIBLE$,                            \
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
                                CURRENT.CODE$

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
                                INVISIBLE.FIELD%

        STRING GLOBAL           BIRTH.DATE$, \ ! DDMMCCYY              ! 1.1 RC
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
                                GROUP.ID$,                             \
                                OLD.OPERATOR.ID$,                      \
                                OLD.PASSWORD$,                         \
                                MONTH.ARRAY$(1),                       \
                                DISPLAY.DATE$,                         \
                                STAFF.NO$,                             \
                                EMPLOYEE.FLG$,                         \
                                RECEIPT.NAME$,                         \
                                GROUP.CODE$

        INTEGER*1 GLOBAL        VALID.OPTION.FOUND,                    \
                                VALID.OPERATOR.ID.FOUND,               \
                                VALID.NAME.FOUND,                      \
                                VALID.PASSWORD.FOUND,                  \
                                VALID.STAFF.NO.FOUND,                  \
                                VALID.EMPLOYEE.FLG.FOUND,              \
                                VALID.RECEIPT.NAME.FOUND,              \
                                VALID.GROUP.CODE.FOUND,                \
                                VALID.MODEL.FLAG.FOUND,                \
                                VALID.CONFIRM.FOUND,                   \
                                CSOUF.RECORD.FOUND,                    \
                                OPTION.ALLOWED(1)

        INTEGER*2 GLOBAL        S1.OPTION%,                            \
                                S1.DATE%,                              \
                                S2.OPERATOR.ID%,                       \
                                S2.NAME%,                              \
                                S2.PASSWORD%,                          \
                                S2.STAFF.NO%,                          \
                                S2.EMPLOYEE.FLG%,                      \
                                S2.RECEIPT.NAME%,                      \
                                S2.GROUP.CODE%,                        \
                                S2.CONFIRM%,                           \
                                S2.CONFIRM.TEXT%,                      \
\ 1.1 RC                        S3.OPERATOR.ID%,                       \
\ 1.1 RC                        S3.NAME%,                              \
\ 1.1 RC                        S3.STAFF.NO%,                          \
\ 1.1 RC                        S3.EMPLOYEE.FLG%,                      \
\ 1.1 RC                        S3.RECEIPT.NAME%,                      \
\ 1.1 RC                        S3.GROUP.CODE%,                        \
\ 1.1 RC                        S4.OPERATOR.ID%,                       \
\ 1.1 RC                        S4.NAME%,                              \
\ 1.1 RC                        S4.STAFF.NO%,                          \
\ 1.1 RC                        S4.EMPLOYEE.FLG%,                      \
\ 1.1 RC                        S4.RECEIPT.NAME%,                      \
\ 1.1 RC                        S4.GROUP.CODE%,                        \
\ 1.1 RC                        S5.OPERATOR.ID%,                       \
\ 1.1 RC                        S5.NAME%,                              \
\ 1.1 RC                        S5.PASSWORD%,                          \
\ 1.1 RC                        S5.STAFF.NO%,                          \
\ 1.1 RC                        S5.EMPLOYEE.FLG%,                      \
\ 1.1 RC                        S5.RECEIPT.NAME%,                      \
\ 1.1 RC                        S5.GROUP.CODE%,                        \
                                S6.BIRTH.DATE%,                        \ 1.1 RC
                                S6.OPERATOR.ID%,                       \
                                S6.NAME%,                              \
                                S6.STAFF.NO%,                          \
                                S6.EMPLOYEE.FLG%,                      \
                                S6.RECEIPT.NAME%,                      \
                                S6.GROUP.CODE%,                        \
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
                                SAVED.OPAUD.REC.NUM%

        INTEGER*4 GLOBAL        BIT.MASK%,                             \
                                OPM.BIT.MASK%,                         \
                                SDK.BIT.MASK%

        INTEGER*4               ERROR.COUNT%

        %INCLUDE CSOUFEXT.J86
        %INCLUDE AFEXT.J86
        %INCLUDE OPAUDEXT.J86

        %INCLUDE PSBF01E.J86   ! Application Log
        %INCLUDE PSBF03E.J86   ! Display Manager
        %INCLUDE PSBF04E.J86   ! External Message
        %INCLUDE PSBF12E.J86   ! Help
        %INCLUDE PSBF20E.J86   ! Session Number Utility
        %INCLUDE PSBF24E.J86   ! Standard Error Detected

        %INCLUDE DMEXTR.J86


\******************************************************************************
\***
\***    EXTERNAL FUNCTIONS DEFINED WITHIN PSB9900
\***
\***...........................................................................


FUNCTION DISPLAY.SCREEN(SCREEN.NUMBER%) EXTERNAL ! PSB9900
END FUNCTION

FUNCTION VALID.CONFIRM EXTERNAL ! PSB9900
    INTEGER*1 VALID.CONFIRM
END FUNCTION

FUNCTION VALID.OPERATOR.ID EXTERNAL ! PSB9900
    INTEGER*1 VALID.OPERATOR.ID
END FUNCTION

FUNCTION READ.CSOUF.RECORD EXTERNAL ! PSB9900
    INTEGER*1 READ.CSOUF.RECORD
END FUNCTION

FUNCTION DELETE.CSOUF.RECORD EXTERNAL ! PSB9900
    INTEGER*1 DELETE.CSOUF.RECORD
END FUNCTION

FUNCTION SCREEN.HELP(SCREEN.NUMBER%) EXTERNAL ! PSB9900
END FUNCTION



\******************************************************************************
\***
\***    INTERNAL FUNCTIONS
\***
\***...........................................................................



\******************************************************************************
\***
\***    VALID.YYMMDD (YYMMDD$)
\***    Checks if passed date YYMMDD$ is a valid YYMMDD format
\***    Handles both packed and unpacked forms
\***    Based on similar VALID.DATE functions within FIXTBAGK and PSD26
\***
\***...........................................................................

FUNCTION VALID.YYMMDD (YYMMDD$) PUBLIC ! Entire function new for Rv 1.1      RC

    INTEGER*1 VALID.YYMMDD ! Function return code

    STRING    YYMMDD$
    STRING    WORK$

    INTEGER*2 YY%
    INTEGER*2 MM%
    INTEGER*2 DD%
    INTEGER*2 DD.MAX%
    
    
    VALID.YYMMDD = 0 ! FALSE -  Default for EXIT FUNCTION

    IF LEN(YYMMDD$) = 3 THEN YYMMDD$ = UNPACK$(YYMMDD$)
    
    IF LEN(YYMMDD$) <> 6 THEN EXIT FUNCTION ! Not YYMMDD format

    WORK$ = TRANSLATE$(YYMMDD$, "0123456789#", "########## ")

    IF WORK$ <> "######" THEN EXIT FUNCTION ! Contains non-numeric characters
    
    YY% = VAL(MID$(YYMMDD$, 1, 2))
    MM% = VAL(MID$(YYMMDD$, 3, 2))
    DD% = VAL(MID$(YYMMDD$, 5, 2))

!   IF YY% < ? OR YY% > ?  THEN EXIT FUNCTION ! Invalid year not checked for
    
    IF MM% < 1 OR MM% > 12 THEN EXIT FUNCTION ! Month not 1-12

    IF DD% < 1 THEN EXIT FUNCTION ! Less than one day in month         ! 1.2 RC
    
    DD.MAX% = 31 ! Default for Jan Mar May Jul Aug Oct Dec
    
    IF MM% = 4 OR MM% = 6 OR MM% = 9 OR MM% = 11 THEN \
        DD.MAX% = 30 ! Apr Jun Sep Nov
    
    IF MM% = 2 THEN BEGIN ! Feb (may be leap year)
        IF MOD(YY%, 4) = 0 THEN DD.MAX% = 29 ELSE DD.MAX% = 28 ! Feb
    ENDIF

    IF DD% > DD.MAX% THEN EXIT FUNCTION ! More than days in month

    VALID.YYMMDD = -1 ! TRUE
    
END FUNCTION


\******************************************************************************
\***
\***    DDMCYY.HEX.FROM.DDMMCCYY$ (DDMMCCYY$)
\***    Converts 8 byte date DDMMCCYY$ into 3 byte UPD-hex DDMCYY format.
\***
\***...........................................................................

FUNCTION DDMCYY.HEX.FROM.DDMMCCYY$ (DDMMCCYY$) PUBLIC ! Entire function  1.3 RC
                                                      ! new for Rv 1.3   1.3 RC
    
    STRING DDMCYY.HEX.FROM.DDMMCCYY$ ! Function output (3 butes)
    STRING DDMMCCYY$                 ! Function input (8 bytes)

    IF DDMMCCYY$ = "00000000" THEN BEGIN
        DDMCYY.HEX.FROM.DDMMCCYY$ = PACK$("000000")
        EXIT FUNCTION
    ENDIF
    
    DDMCYY.HEX.FROM.DDMMCCYY$ = \
          PACK$(MID$(DDMMCCYY$,1,2))        + \ ! DD
      CHR$( VAL(MID$(DDMMCCYY$,3,2)) *16    + \ ! M from MM
            VAL(MID$(DDMMCCYY$,5,2)) -19  ) + \ ! C from CC
          PACK$(MID$(DDMMCCYY$,7,2))            ! YY

END FUNCTION


\******************************************************************************
\***
\***    DDMMCCYY.FROM.DDMCYY.HEX$ (DDMCYY.HEX$)
\***    Converts 3 byte UPD-hex date DDMCYY.HEX$ into 8 byte DDMMCCYY format.
\***
\***...........................................................................

FUNCTION DDMMCCYY.FROM.DDMCYY.HEX$ (DDMCYY.HEX$) ! Entire function     ! 1.3 RC
                                                 ! new from Rv 1.3     ! 1.3 RC
    
    STRING DDMMCCYY.FROM.DDMCYY.HEX$ ! Function output
    STRING DDMCYY.HEX$               ! Function input
    
    DDMMCCYY.FROM.DDMCYY.HEX$ = \
                    UNPACK$(MID$(DDMCYY.HEX$,1,1))               + \ ! DD
      RIGHT$("0" + STR$(ASC(MID$(DDMCYY.HEX$,2,1) ) / 16),2)     + \ ! MM from M
                  STR$((ASC(MID$(DDMCYY.HEX$,2,1)) AND 0Fh) +19) + \ ! CC from C
                    UNPACK$(MID$(DDMCYY.HEX$,3,1))                   ! YY

END FUNCTION


\******************************************************************************
\***
\***    YYMMDD.FROM.DDMCYY.HEX$ (DDMCYY.HEX$)
\***    Converts 3 byte UPD-hex date DDMCYY.HEX$ into 6 byte YYMMDD format.
\***
\***...........................................................................

FUNCTION YYMMDD.FROM.DDMCYY.HEX$ (DDMCYY.HEX$) PUBLIC ! Entire function  1.3 RC
                                                      ! new for Rv 1.3   1.3 RC
    
    STRING YYMMDD.FROM.DDMCYY.HEX$ ! Function output
    STRING DDMCYY.HEX$             ! Function input
    STRING DDMMCCYY$               ! Work
    
    DDMMCCYY$ = DDMMCCYY.FROM.DDMCYY.HEX$ (DDMCYY.HEX$)
    
    YYMMDD.FROM.DDMCYY.HEX$ = \
      MID$(DDMMCCYY$,7,2) + \ ! YY
      MID$(DDMMCCYY$,3,2) + \ ! MM
      MID$(DDMMCCYY$,1,2)     ! DD

END FUNCTION


\******************************************************************************
\***
\***    VALID.DDMCYY (DDMCYY$)
\***    This function should only be updated in line with PSB9900 function 
\***    VALID.BIRTH.DATE upon which it is based.
\***
\***    Checks whether input date is a valid DDMCYY format.
\***
\***...........................................................................

FUNCTION VALID.DDMCYY (DDMCYY$) PUBLIC ! Entire function new for Rv 1.3 RC

    INTEGER*1 VALID.DDMCYY
    
    STRING    DDMCYY$ ! Function input
    STRING    DDMMCCYY$
    STRING    WORK$
    STRING    YYMMDD$
    INTEGER*4 CCYYMMDD%


    VALID.DDMCYY = 0 ! FALSE

    DDMMCCYY$ = DDMMCCYY.FROM.DDMCYY.HEX$ (DDMCYY$)

!   Confirm converted date is eight bytes numeric data
    
    WORK$ = TRANSLATE$(DDMMCCYY$, "0123456789#", "########## ")
    
    IF WORK$ <> "########" THEN EXIT FUNCTION

!   Confirm DDMMCCYY$ has a sensible CC component

    IF   VAL( MID$(DDMMCCYY$,5,2) ) < 19 \
      OR VAL( MID$(DDMMCCYY$,5,2) ) > 20 THEN EXIT FUNCTION

!   Confirm DDMMCCYY$ has a valid YYMMDD combination
    
    YYMMDD$ = MID$(DDMMCCYY$,7,2) + \
              MID$(DDMMCCYY$,3,2) + \
              MID$(DDMMCCYY$,1,2)

    IF NOT VALID.YYMMDD (YYMMDD$) THEN EXIT FUNCTION

!   Reject birth date when age is before 14th birthday

    CCYYMMDD% = VAL( MID$(DDMMCCYY$,5,2) + YYMMDD$ ) \
                  + 140000 ! Adds 14 years to birth date
    
    IF CCYYMMDD% > VAL("20" + DATE$) THEN EXIT FUNCTION

!   Reject birth date when age is on or after 100th birthday
    
    CCYYMMDD% = VAL( MID$(DDMMCCYY$,5,2) + YYMMDD$ ) \
                  + 1000000 ! Adds 100 years to birth date
    
    IF CCYYMMDD% <= VAL("20" + DATE$) THEN EXIT FUNCTION

    VALID.DDMCYY = -1 ! TRUE

END FUNCTION


\******************************************************************************
\***
\***    SET.STAFF.NO.FROM.AF
\***    Sets STAFF.NO$ from AF.STAFF.NO$
\***
\***...........................................................................

FUNCTION SET.STAFF.NO.FROM.AF ! Entire function new for Rv 1.1       ! 1.1 RC
    
    STRING    WORK$
    
    STAFF.NO$ = "00000000" ! Default value
    
    WORK$ = TRANSLATE$(UNPACK$(AF.STAFF.NUM$), "0123456789#", "########## ")

    IF WORK$ <> "########" THEN EXIT FUNCTION ! Contains non-numeric characters

    STAFF.NO$ = UNPACK$(AF.STAFF.NUM$)

END FUNCTION


\******************************************************************************
\***
\***    SET.BIRTH.DATE.FROM.AF
\***    Sets BIRTH.DATE$ (including century) from AF.BIRTH.DATE$
\***
\***...........................................................................

FUNCTION SET.BIRTH.DATE.FROM.AF ! Entire function new for Rv 1.1       ! 1.1 RC

    BIRTH.DATE$ = "00000000" ! Default CCYYMMDD value

    IF NOT VALID.DDMCYY (AF.BIRTH.DATE$) THEN BEGIN \                  ! 1.3 RC
        AF.BIRTH.DATE$ = PACK$("000000") ! Set invalid AF.BIRTH.DATE$  ! 1.3 RC
        EXIT FUNCTION                    ! to null in case written     ! 1.3 RC
    ENDIF                                ! back to file                ! 1.3 RC
    
    BIRTH.DATE$ = DDMMCCYY.FROM.DDMCYY.HEX$ (AF.BIRTH.DATE$)           ! 1.3 RC
   
END FUNCTION



\******************************************************************************
\***
\***    INTERNAL SUBPROGRAMS - LOW LEVEL
\***    PSB9900 subroutines transferred to PSB9902
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SB.FILE.UTILS                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***      Allocate/report/de-allocate a file session number                   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***      Parameters : 2 or 3 (depending on action)                           *
\***                                                                          *
\***         SB.ACTION$  = "O" for allocate file session number               *
\***                       "R" for report file session number                 *
\***                       "C" for de-allocate file session number            *
\***                                                                          *
\***         SB.INTEGER% = file reporting number for action "O" or            *
\***                       file session number for actions "R" or "C"         *
\***                                                                          *
\***         SB.STRING$  = logical file name for action "O" or                *
\***                       null ("") for action "R" and "C"                   *
\***                                                                          *
\***      Output : 1 or 2 (depending on action)                               *
\***                                                                          *
\***         SB.FILE.NAME$     = logical file name for action "R"             *
\***                                                                          *
\***         SB.FILE.SESS.NUM% = file session number for action "O" or        *
\***                             undefined for action "C"                     *
\***         OR                                                               *
\***         SB.FILE.REP.NUM%  = file reporting number for action "R" or      *
\***                             undefined for action "C"                     *
\***                                                                          *
\******************************************************************************

SUB     SB.FILE.UTILS PUBLIC                                           ! 1.0 RC

        CALL SESS.NUM.UTILITY(SB.ACTION$,                              \
                              SB.INTEGER%,                             \
                              SB.STRING$)

        IF SB.ACTION$ = "O" THEN                                       \
        BEGIN
           SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
        ENDIF                                                          \
        ELSE                                                           \
        IF SB.ACTION$ = "R" THEN                                       \
        BEGIN
           SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
           SB.FILE.NAME$ = F20.FILE.NAME$
        ENDIF

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    ERROR ROUTINE   :       FILE.ERROR                                    *
\***                                                                          *
\******************************************************************************

SUB     FILE.ERROR ! Not PUBLIC as PSB9900 and PSB9902 each contain    ! 1.0 RC
                   ! their own FILE.ERROR routine

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

END SUB                                                                ! 1.0 RC




\******************************************************************************
\***
\***    INTERNAL SUBPROGRAMS - DISPLAY MANAGER ETC
\***    PSB9900 subroutines transferred to PSB9902
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.INPUT                                     *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get input from the keyboard                                           *
\***                                                                          *
\******************************************************************************

SUB     GET.INPUT PUBLIC                                               ! 1.0 RC

        STRING.DATA$ = "3113333333311333"
        CALL SETF(STRING.DATA$)

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.UPDF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$ = "3303333333303333"
        CALL SETF(STRING.DATA$)

        FUNCTION.KEY% = F03.RETURNED.INTEGER%

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PUT.CURSOR.IN.FIELD                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Position cursor in selected input field                               *
\***                                                                          *
\******************************************************************************

SUB     PUT.CURSOR.IN.FIELD PUBLIC                                     ! 1.0 RC

        STRING.DATA$ = ""

        INTEGER.DATA% = CURSOR.POSITION%

        CALL DM.POSF(STRING.DATA$,                                      \
                     INTEGER.DATA%)

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESUME.INPUT                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get input from the keyboard after an interrupt                        *
\***                                                                          *
\******************************************************************************

SUB     RESUME.INPUT PUBLIC                                            ! 1.0 RC

        STRING.DATA$ = "3113333333311333"
        CALL SETF(STRING.DATA$)

        STRING.DATA$ = ""
        INTEGER.DATA% = 0

        CALL DM.RESF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

        STRING.DATA$ = "3303333333303333"
        CALL SETF(STRING.DATA$)

        FUNCTION.KEY% = F03.RETURNED.INTEGER%

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.CURSOR.STATE                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Shows or hides the cursor                                             *
\***                                                                          *
\******************************************************************************

SUB     SET.CURSOR.STATE PUBLIC                                        ! 1.0 RC

        STRING.DATA$  = CURSOR.STATE$
        INTEGER.DATA% = 0

        CALL DM.CURS(STRING.DATA$,                                     \
                     INTEGER.DATA%)

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.FIELD                                     *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the selected field with the data provided                         *
\***                                                                          *
\******************************************************************************

SUB     SET.FIELD PUBLIC                                               ! 1.0 RC

        INTEGER.DATA% = 0

        CALL DM.PUTF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.FIELD.ATTRIBUTES                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redefine the specified field attributes                               *
\***                                                                          *
\******************************************************************************

SUB     SET.FIELD.ATTRIBUTES PUBLIC                                    ! 1.0 RC

        INTEGER.DATA% = 0

        CALL DM.SETF(STRING.DATA$,                                     \
                     INTEGER.DATA%)

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.MESSAGE                                 *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear the current BEMF error message                                  *
\***                                                                          *
\******************************************************************************

SUB     CLEAR.MESSAGE PUBLIC                                           ! 1.0 RC

        OLD.POSITION% = CURSOR.POSITION%

        CURSOR.POSITION% = 1
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = ""
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = OLD.POSITION%

        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DISPLAY.MESSAGE                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display a new BEMF error message                                      *
\***                                                                          *
\******************************************************************************

SUB     DISPLAY.MESSAGE PUBLIC                                         ! 1.0 RC

        CURSOR.STATE$ = INVISIBLE$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

        STRING.DATA$ = ""
        OLD.POSITION% = CURSOR.POSITION%
        CURSOR.POSITION% = 1

        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = "31"
        CALL SET.FIELD.ATTRIBUTES                                      ! 1.0 RC

        MESSAGE.NO% = DISPLAY.MESSAGE.NUMBER%
        STRING.DATA$ = DISPLAY.MESSAGE.TEXT$
        RETURN.FIELD% = 1

        CALL EXTERNAL.MESSAGE(MESSAGE.NO%,                             \
                              STRING.DATA$,                            \
                              RETURN.FIELD%)

        CURSOR.POSITION% = OLD.POSITION%

        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        CURSOR.STATE$ = VISIBLE$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       WAIT.MESSAGE                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display a wait message to the screen                                  *
\***                                                                          *
\******************************************************************************

SUB     WAIT.MESSAGE PUBLIC                                            ! 1.0 RC

        ! B251 Processing - Please Wait .....
        DISPLAY.MESSAGE.NUMBER% = 251
        CALL DISPLAY.MESSAGE                                           ! 1.0 RC

        CURSOR.POSITION% = INVISIBLE.FIELD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***
\***    INTERNAL SUBPROGRAMS - STANDARD ROITINES ALSO CALLED BY PSB9900
\***    PSB9900 subroutines transferred to PSB9902
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CHAIN.TO.CALLER                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    CHAIN back to the required screen program                             *
\***                                                                          *
\******************************************************************************

SUB     CHAIN.TO.CALLER PUBLIC                                         ! 1.0 RC

        CALL WAIT.MESSAGE                                              ! 1.0 RC

        PSBCHN.PRG = "C:\ADX_UPGM\" + CHAIN.TO.PROG$ + ".286"
        PSBCHN.APP = "C:\ADX_UPGM\PSB99.286"

        %INCLUDE PSBCHNE.J86

        STOP

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.MODEL.FLAGS                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear till and controller model fields                                *
\***                                                                          *
\******************************************************************************

SUB     CLEAR.MODEL.FLAGS PUBLIC                                       ! 1.0 RC

        FOR I% = 1 TO MAX.TILL.PTR%

            IF TILL.MODEL.FLAG$(I%) <> MODEL.FLAG$ THEN                \
            BEGIN
               CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(I%)
               CALL PUT.CURSOR.IN.FIELD                                ! 1.0 RC

               STRING.DATA$ = MODEL.FLAG$
               CALL SET.FIELD                                          ! 1.0 RC
            ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.PTR%

            IF CTLR.MODEL.FLAG$(I%) <> MODEL.FLAG$ THEN                \
            BEGIN
               CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(I%)
               CALL PUT.CURSOR.IN.FIELD                                ! 1.0 RC

               STRING.DATA$ = MODEL.FLAG$
               CALL SET.FIELD                                          ! 1.0 RC
            ENDIF

        NEXT I%

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DISPLAY.FORMATTED.DATE                        *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Display the date on the screen                                        *
\***                                                                          *
\******************************************************************************

SUB     DISPLAY.FORMATTED.DATE PUBLIC                                  ! 1.0 RC

        CURSOR.POSITION% = S1.DATE%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = DISPLAY.DATE$
        CALL SET.FIELD                                                 ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.CSOUF.RECORD                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Find the a record on the ADXCSOUF file                                *
\***                                                                          *
\******************************************************************************

SUB     GET.CSOUF.RECORD PUBLIC                                        ! 1.0 RC

        CSOUF.REC.NUM% = 0

        CSOUF.RECORD.FOUND = FALSE

        WHILE NOT CSOUF.RECORD.FOUND

              CSOUF.REC.NUM% = CSOUF.REC.NUM% + 1

              IF READ.CSOUF.RECORD = 0 THEN                            \
              BEGIN
                 IF CSOUF.OP.ID$ = LEFT$(CSOUF.RECORD$,8) OR           \
                    (LEFT$(CSOUF.RECORD$,1) = CHR$(0) AND              \
                    CSOUF.OPERATION$ = "ADD") THEN                     \
                 BEGIN
                    CSOUF.RECORD.FOUND = TRUE
                 ENDIF
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 CSOUF.RECORD.FOUND = TRUE
              ENDIF

        WEND

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.DETAILS                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Get operator details from the EALAUTH file                            *
\***                                                                          *
\******************************************************************************

SUB     GET.OPERATOR.DETAILS PUBLIC                                    ! 1.0 RC

        OPERATOR.NAME$     = UCASE$(AF.OPERATOR.NAME$)

        IF AF.PASSWORD$ = STRING$(4,CHR$(255)) THEN                    \
        BEGIN
           AF.PASSWORD$ = PACK$("00000905")
        ENDIF

        CALL SET.STAFF.NO.FROM.AF  ! Sets STAFF.NO$ from               ! 1.1 RC
                                   ! Authorisation File                ! 1.1 RC
        
        RECEIPT.NAME$ = AF.RECEIPT.NAME$                               !

        IF AF.EMPLOYEE.FLAG$ = PACK$("00") THEN BEGIN
           EMPLOYEE.FLG$ = "Y"
        ENDIF ELSE BEGIN
           EMPLOYEE.FLG$ = "N"
        ENDIF

        CALL SET.BIRTH.DATE.FROM.AF ! Sets BIRTH.DATE$ (CCYYMMDD form) ! 1.1 RC
                                    ! from Authorisation File          ! 1.1 RC
        
        GROUP.CODE$ = UNPACK$(AF.GROUP.CODE$)                          !

        OPERATOR.PASSWORD$ = STR$(VAL(UNPACK$(AF.PASSWORD$)))

        FOR I% = 1 TO MAX.TILL.PTR%

            TILL.INDEX% = VAL(UNPACK$(MID$(                            \
                          TILL.MODEL.RECORD$(I%),2,1)))

            BIT.MASK% = 2 ^ (TILL.INDEX% - 1)

            IF (AF.MODEL.FLAGS.1% AND BIT.MASK%) > 0 THEN              \
            BEGIN
               TILL.MODEL.FLAG$(I%) = "Y"
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               TILL.MODEL.FLAG$(I%) = "N"
            ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.PTR%

            CTLR.INDEX% = VAL(UNPACK$(MID$(                            \
                          CTLR.MODEL.RECORD$(I%),2,1)))

            BIT.MASK% = 2 ^ (CTLR.INDEX% - 1)

            IF (AF.MODEL.FLAGS.2% AND BIT.MASK%) > 0 THEN              \
            BEGIN
               CTLR.MODEL.FLAG$(I%) = "Y"
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               CTLR.MODEL.FLAG$(I%) = "N"
            ENDIF

        NEXT I%

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       HIDE.CONFIRM.MESSAGE                          *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Reveal the confirmational message                                     *
\***                                                                          *
\******************************************************************************

SUB     HIDE.CONFIRM.MESSAGE PUBLIC                                    ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM.TEXT%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = INVISIBLE$
        CALL SET.FIELD.ATTRIBUTES                                      ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = INVISIBLE$
        CALL SET.FIELD.ATTRIBUTES                                      ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.MODEL.FLAGS                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay till and controller model fields                            *
\***                                                                          *
\******************************************************************************

SUB     RESTORE.MODEL.FLAGS PUBLIC                                     ! 1.0 RC

        FOR I% = 1 TO MAX.TILL.PTR%

            CURSOR.POSITION% = S2.TILL.MODEL.TEXT%(I%)
            CALL PUT.CURSOR.IN.FIELD                                   ! 1.0 RC

            STRING.DATA$ = TILL.MODEL.NAME$(I%)
            CALL SET.FIELD                                             ! 1.0 RC

            CURSOR.POSITION% = S2.TILL.MODEL.FLAG%(I%)
            CALL PUT.CURSOR.IN.FIELD                                   ! 1.0 RC

            STRING.DATA$ = TILL.MODEL.FLAG$(I%)
            CALL SET.FIELD                                             ! 1.0 RC

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.PTR%

            CURSOR.POSITION% = S2.CTLR.MODEL.TEXT%(I%)
            CALL PUT.CURSOR.IN.FIELD                                   ! 1.0 RC

            STRING.DATA$ = CTLR.MODEL.NAME$(I%)
            CALL SET.FIELD                                             ! 1.0 RC

            CURSOR.POSITION% = S2.CTLR.MODEL.FLAG%(I%)
            CALL PUT.CURSOR.IN.FIELD                                   ! 1.0 RC

            STRING.DATA$ = CTLR.MODEL.FLAG$(I%)
            CALL SET.FIELD                                             ! 1.0 RC

        NEXT I%

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESET.MODEL.FLAGS                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Initialise the model flags                                            *
\***                                                                          *
\******************************************************************************

SUB     RESET.MODEL.FLAGS PUBLIC                                       ! 1.0 RC

        FOR I% = 1 TO MAX.TILL.MODELS%

             IF I% <= MAX.TILL.PTR% THEN                               \
             BEGIN
                TILL.MODEL.FLAG$(I%) = MODEL.FLAG$
             ENDIF                                                     \
             ELSE                                                      \
             BEGIN
                TILL.MODEL.FLAG$(I%) = " "
             ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.MODELS%

             IF I% <= MAX.CTLR.PTR% THEN                               \
             BEGIN
                CTLR.MODEL.FLAG$(I%) = MODEL.FLAG$
             ENDIF                                                     \
             ELSE                                                      \
             BEGIN
                CTLR.MODEL.FLAG$(I%) = " "
             ENDIF

        NEXT I%

END SUB                                                                ! 1.0 RC

SUB RESTORE.FIELDS.02 EXTERNAL ! PSB9900                               ! 1.0 RC
END SUB                                                                ! 1.0 RC

SUB RESTORE.FIELDS.04 EXTERNAL ! PSB9900                               ! 1.0 RC
END SUB                                                                ! 1.0 RC

SUB RESTORE.FIELDS.05 EXTERNAL ! PSB9900                               ! 1.0 RC
END SUB                                                                ! 1.0 RC

\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       RESTORE.FIELDS.06                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Redisplay all fields for the delete an operator screen                *
\***                                                                          *
\******************************************************************************

SUB     RESTORE.FIELDS.06 PUBLIC                                       ! 1.0 RC

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

        CALL DISPLAY.FORMATTED.DATE                                    ! 1.0 RC

        CURSOR.POSITION% = S6.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        IF VAL(OPERATOR.ID$) > 0 THEN                                  \
        BEGIN
           OPERATOR.ID$ = STR$(VAL(OPERATOR.ID$))
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           OPERATOR.ID$ = ""
        ENDIF

        STRING.DATA$ = OPERATOR.ID$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = S6.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = OPERATOR.NAME$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = S6.STAFF.NO%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = STAFF.NO$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = S6.RECEIPT.NAME%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = RECEIPT.NAME$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = S6.BIRTH.DATE%                              ! 1.1 RC
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.1 RC

        IF BIRTH.DATE$ <> "" THEN BEGIN \                              ! 1.1 RC
            STRING.DATA$ = MID$(BIRTH.DATE$,1,2) + "/" + \             ! 1.1 RC
                           MID$(BIRTH.DATE$,3,2) + "/" + \             ! 1.1 RC
                           MID$(BIRTH.DATE$,5,4)                       ! 1.1 RC
        ENDIF                                                          ! 1.1 RC
        CALL SET.FIELD                                                 ! 1.1 RC

        CURSOR.POSITION% = S6.GROUP.CODE%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = GROUP.CODE$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.POSITION% = S6.EMPLOYEE.FLG%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = EMPLOYEE.FLG$
        CALL SET.FIELD                                                 ! 1.0 RC
        
        CALL RESTORE.MODEL.FLAGS                                       ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = CONFIRM$
        CALL SET.FIELD                                                 ! 1.0 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       REVEAL.CONFIRM.MESSAGE                        *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Reveal the confirmational message                                     *
\***                                                                          *
\******************************************************************************

SUB     REVEAL.CONFIRM.MESSAGE PUBLIC                                  ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM.TEXT%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = VISIBLE$
        CALL SET.FIELD.ATTRIBUTES                                      ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        STRING.DATA$ = VISIBLE$
        CALL SET.FIELD.ATTRIBUTES                                      ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.OLD.OPAUD.DETAILS                         *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the old OPAUD details                                             *
\***                                                                          *
\******************************************************************************

SUB     SET.OLD.OPAUD.DETAILS PUBLIC                                   ! 1.0 RC

        OPAUD.CHANGED.ID$ = RIGHT$("000" + UNPACK$(AF.OPERATOR.NO$),3)

        IF OPTION$ = "1" THEN                                          \
        BEGIN
           OPAUD.DETAILS.1$ = STRING$(27," ")
           GOTO END.SET.OLD.OPAUD.DETAILS
        ENDIF

        OPAUD.DETAILS.1$ = RIGHT$("000" + UNPACK$(AF.PASSWORD$),3)

        FOR I% = 1 TO MAX.TILL.MODELS%

            IF TILL.MODEL.FLAG$(I%) = "Y" OR                           \
               TILL.MODEL.FLAG$(I%) = "N" THEN                         \
            BEGIN
               OPAUD.DETAILS.1$ = OPAUD.DETAILS.1$ +                   \
                                  TILL.MODEL.FLAG$(I%)
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               OPAUD.DETAILS.1$ = OPAUD.DETAILS.1$ + " "
            ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.MODELS%

            IF CTLR.MODEL.FLAG$(I%) = "Y" OR                           \
               CTLR.MODEL.FLAG$(I%) = "N" THEN                         \
            BEGIN
               OPAUD.DETAILS.1$ = OPAUD.DETAILS.1$ +                   \
                                  CTLR.MODEL.FLAG$(I%)
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               OPAUD.DETAILS.1$ = OPAUD.DETAILS.1$ + " "
            ENDIF

        NEXT I%

        END.SET.OLD.OPAUD.DETAILS:

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       SET.NEW.OPAUD.DETAILS                         *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Set the new OPAUD details                                             *
\***                                                                          *
\******************************************************************************

SUB     SET.NEW.OPAUD.DETAILS PUBLIC                                   ! 1.0 RC

        OPAUD.DATE$   = TODAYS.DATE$
        OPAUD.TIME$   = LEFT$(TIME$,4)
        OPAUD.OPTION$ = OPTION$
        OPAUD.FILLER$ = STRING$(7," ")
        OPAUD.CRLF$   = CHR$(0DH) + CHR$(0AH)

        OPAUD.CHANGED.ID$ = RIGHT$("000" + UNPACK$(AF.OPERATOR.NO$),3)

        IF OPTION$ = "5" then                                          \
        BEGIN
           OPAUD.DETAILS.2$ = STRING$(27," ")
           GOTO END.SET.NEW.OPAUD.DETAILS
        ENDIF

        OPAUD.DETAILS.2$  = RIGHT$("000" + UNPACK$(AF.PASSWORD$),3)

        FOR I% = 1 TO MAX.TILL.MODELS%

            IF TILL.MODEL.FLAG$(I%) = "Y" OR                           \
               TILL.MODEL.FLAG$(I%) = "N" THEN                         \
            BEGIN
               OPAUD.DETAILS.2$ = OPAUD.DETAILS.2$ +                   \
                                  TILL.MODEL.FLAG$(I%)
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               OPAUD.DETAILS.2$ = OPAUD.DETAILS.2$ + " "
            ENDIF

        NEXT I%

        FOR I% = 1 TO MAX.CTLR.MODELS%

            IF CTLR.MODEL.FLAG$(I%) = "Y" OR                           \
               CTLR.MODEL.FLAG$(I%) = "N" THEN                         \
            BEGIN
               OPAUD.DETAILS.2$ = OPAUD.DETAILS.2$ +                   \
                                  CTLR.MODEL.FLAG$(I%)
            ENDIF                                                      \
            ELSE                                                       \
            BEGIN
               OPAUD.DETAILS.2$ = OPAUD.DETAILS.2$ + " "
            ENDIF

        NEXT I%

        END.SET.NEW.OPAUD.DETAILS:

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       WRITE.OPAUD.RECORDS                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Log the current operator action to the audit file                     *
\***                                                                          *
\******************************************************************************

SUB     WRITE.OPAUD.RECORDS PUBLIC                                     ! 1.0 RC

        IF WRITE.OPAUD = 0 THEN                                        \
        BEGIN
           SAVED.OPAUD.REC.NUM% = OPAUD.REC.NUM%

           OPAUD.REC.NUM% = 1

           OPAUD.LAST.REC.UPDATED$ = RIGHT$("0000" +                   \
                                     STR$(SAVED.OPAUD.REC.NUM%),4)

           OPAUD.FILLER$           = STRING$(70," ")

           IF WRITE.OPAUD <> 0 THEN                                    \
           BEGIN
              CALL FILE.ERROR                                          ! 1.0 RC
           ENDIF

           OPAUD.REC.NUM% = SAVED.OPAUD.REC.NUM% + 1

           IF OPAUD.REC.NUM% > VAL(OPAUD.FILE.SIZE$) THEN              \
           BEGIN
              OPAUD.REC.NUM% = 2
           ENDIF

        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           CALL FILE.ERROR                                             ! 1.0 RC
        ENDIF

END SUB                                                                ! 1.0 RC



\******************************************************************************
\***
\***    INTERNAL SUBPROGRAMS - DEPENDANT ON OTHERS
\***    PSB9900 subroutines transferred to PSB9902
\***    Dependant on one or more previously defined subprograms
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.QUIT.CONFIRM                              *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for confirmation of the quit key                      *
\***                                                                          *
\******************************************************************************

SUB     GET.QUIT.CONFIRM PUBLIC                                        ! 1.0 RC

        ! B182 Data already keyed will be lost - press F3 to QUIT
        DISPLAY.MESSAGE.NUMBER% = 182
        CALL DISPLAY.MESSAGE                                           ! 1.0 RC

        SAVED.POSITION% = CURSOR.POSITION%

        CURSOR.POSITION% = INVISIBLE.FIELD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        CALL GET.INPUT                                                 ! 1.0 RC

        CURSOR.POSITION% = SAVED.POSITION%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        IF FUNCTION.KEY% = QUIT.KEY% THEN                              \
        BEGIN
           EXIT.KEY.PRESSED(S%) = TRUE
           CONFIRM$ EQ "N"                                             !
        ENDIF                                                          \
        ELSE                                                           \
        IF FUNCTION.KEY% = ESC.KEY% THEN                               \
        BEGIN
           CHAIN.TO.PROG$ = "PSB50"
           PSBCHN.MENCON  = "000000"
           CALL CHAIN.TO.CALLER                                        ! 1.0 RC
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           CALL GET.INPUT                                              ! 1.0 RC
        ENDIF

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.CONFIRM                                   *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for confirming an action                                *
\***                                                                          *
\******************************************************************************

SUB     GET.CONFIRM PUBLIC                                             ! 1.0 RC

        CURSOR.POSITION% = S2.CONFIRM%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        EXIT.KEY.PRESSED(S%) = FALSE
        VALID.CONFIRM.FOUND  = FALSE

        CALL GET.INPUT                                                 ! 1.0 RC

        WHILE NOT (EXIT.KEY.PRESSED(S%) OR                             \
                  VALID.CONFIRM.FOUND)

              CONFIRM$ = UCASE$(F03.RETURNED.STRING$)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.0 RC
                 CALL RESUME.INPUT                                     ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 CALL GET.QUIT.CONFIRM                                 ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(S%)

                 IF S% = 2 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.02                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 4 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.04                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 5 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.05                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 6 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.06                             ! 1.0 RC
                 ENDIF                                                 \

                 CALL REVEAL.CONFIRM.MESSAGE                           ! 1.0 RC

                 CURSOR.POSITION% = S2.CONFIRM%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.0 RC

                 CALL GET.INPUT                                        ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.CONFIRM THEN                                 \
                    VALID.CONFIRM.FOUND = TRUE                         \
                 ELSE                                                  \
                 BEGIN
                    ! B064 You must only type 'N' or 'Y'
                    DISPLAY.MESSAGE.NUMBER% = 64
                    CALL DISPLAY.MESSAGE                               ! 1.0 RC
                    CALL RESUME.INPUT                                  ! 1.0 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.CONFIRM.FOUND AND                                     \
           NOT EXIT.KEY.PRESSED(S%) THEN                               \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.0 RC

           STRING.DATA$ = CONFIRM$
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.QUIT.KEY                                  *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the input of quit key                             *
\***                                                                          *
\******************************************************************************

SUB     GET.QUIT.KEY PUBLIC                                            ! 1.0 RC

        CURSOR.POSITION% = INVISIBLE.FIELD%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        EXIT.KEY.PRESSED(S%) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(S%)

              CALL GET.INPUT                                           ! 1.0 RC

              IF FUNCTION.KEY% = QUIT.KEY% THEN                        \
              BEGIN
                 EXIT.KEY.PRESSED(S%) = TRUE
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(S%)

                 IF S% = 2 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.02                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 4 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.04                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 5 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.05                             ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 IF S% = 6 THEN                                        \
                 BEGIN
                    CALL RESTORE.FIELDS.06                             ! 1.0 RC
                 ENDIF                                                 \

                 CURSOR.POSITION% = INVISIBLE.FIELD%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = ESC.KEY% THEN                         \
              BEGIN
                 CHAIN.TO.PROG$ = "PSB50"
                 PSBCHN.MENCON  = "000000"
                 CALL CHAIN.TO.CALLER                                  ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.0 RC
              ENDIF

        WEND

END SUB                                                                ! 1.0 RC



\******************************************************************************
\***
\***    INTERNAL SUBPROGRAMS - PSB9902 SPECIFIC
\***    PSB9900 subroutines transferred to PSB9902
\***    Specific to the main PROCESS.SCREEN.06 process
\***
\***...........................................................................


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       CLEAR.FIELDS.06                               *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Clear all fields for the delete operator screen                       *
\***                                                                          *
\******************************************************************************

SUB     CLEAR.FIELDS.06                                                ! 1.0 RC

        CURSOR.STATE$ = CURSOR.OFF$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

        IF OPERATOR.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S6.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.0 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF

        IF STAFF.NO$ <> "" THEN BEGIN
           CURSOR.POSITION% = S6.STAFF.NO%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.0 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF

        IF BIRTH.DATE$ <> "" THEN BEGIN                                ! 1.1 RC
           CURSOR.POSITION% = S6.BIRTH.DATE%                           ! 1.1 RC
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.1 RC
           STRING.DATA$ = ""                                           ! 1.1 RC
           CALL SET.FIELD                                              ! 1.1 RC
        ENDIF                                                          ! 1.1 RC

        IF RECEIPT.NAME$ <> "" THEN                                   \
        BEGIN
           CURSOR.POSITION% = S6.RECEIPT.NAME%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.0 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF

        IF GROUP.CODE$ <> "" THEN                                     \
        BEGIN
           CURSOR.POSITION% = S6.GROUP.CODE%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.0 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF

        IF EMPLOYEE.FLG$ <> "" THEN BEGIN
           CURSOR.POSITION% = S6.EMPLOYEE.FLG%
           CALL PUT.CURSOR.IN.FIELD                                    ! 1.0 RC

           STRING.DATA$ = ""
           CALL SET.FIELD                                              ! 1.0 RC
        ENDIF
        
        MODEL.FLAG$ = ""

        CALL CLEAR.MODEL.FLAGS                                         ! 1.0 RC

        CURSOR.STATE$ = CURSOR.ON$
        CALL SET.CURSOR.STATE                                          ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DELETE.AUTH.RECORDS                           *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Delete authorisation records from the EALAUTH and ADXCSOUF files      *
\***                                                                          *
\******************************************************************************

SUB     DELETE.AUTH.RECORDS                                            ! 1.0 RC

        AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +                \
                          OPERATOR.ID$,8))

        CSOUF.OP.ID$ = LEFT$(OPERATOR.ID$ +                            \
                       STRING$(8," "),8)

        CALL GET.CSOUF.RECORD                                          ! 1.0 RC

        CSOUF.RECORD$ = PACK$(STRING$(68,"0"))

        IF DELETE.CSOUF.RECORD = 0 THEN                                \
        BEGIN
           DELREC AF.SESS.NUM%; AF.OPERATOR.NO$

           CALL SET.NEW.OPAUD.DETAILS                                  ! 1.0 RC
           CALL WRITE.OPAUD.RECORDS                                    ! 1.0 RC

           ! B180 Operator successfully deleted
           DISPLAY.MESSAGE.NUMBER% = 180
           CALL DISPLAY.MESSAGE                                        ! 1.0 RC
        ENDIF                                                          \
        ELSE                                                           \
        BEGIN
           CALL FILE.ERROR                                             ! 1.0 RC
        ENDIF

        CALL GET.QUIT.KEY                                              ! 1.0 RC

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       DELETE.AN.OPERATOR                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Delete an operator from the authorisation files                       *
\***                                                                          *
\******************************************************************************

SUB     DELETE.AN.OPERATOR                                             ! 1.0 RC

        CSOUF.OPERATION$ = "DELETE"

        CALL REVEAL.CONFIRM.MESSAGE                                    ! 1.0 RC

        CALL GET.CONFIRM                                               ! 1.0 RC

        CALL HIDE.CONFIRM.MESSAGE                                      ! 1.0 RC

        IF CONFIRM$ = "Y" THEN                                         \
        BEGIN
           CALL WAIT.MESSAGE                                           ! 1.0 RC
           CALL DELETE.AUTH.RECORDS                                    ! 1.0 RC
        ENDIF

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       GET.OPERATOR.ID.06                            *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Input routine for the operator ID on the delete an operator screen    *
\***                                                                          *
\******************************************************************************

SUB     GET.OPERATOR.ID.06                                             ! 1.0 RC

        CURSOR.POSITION% = S6.OPERATOR.ID%
        CALL PUT.CURSOR.IN.FIELD                                       ! 1.0 RC

        EXIT.KEY.PRESSED(6)     = FALSE
        VALID.OPERATOR.ID.FOUND = FALSE

        CALL GET.INPUT                                                 ! 1.0 RC

        WHILE NOT (EXIT.KEY.PRESSED(6) OR                              \
                  VALID.OPERATOR.ID.FOUND)

              OPERATOR.ID$ = RIGHT$(STRING$(3,"0") +                   \
                             STR$(VAL(F03.RETURNED.STRING$)),3)

              IF NOT (FUNCTION.KEY% = ENTER.KEY% OR                    \
                 FUNCTION.KEY% = HELP.KEY% OR                          \
                 FUNCTION.KEY% = QUIT.KEY% OR                          \
                 FUNCTION.KEY% = ESC.KEY%) THEN                        \
              BEGIN
                 DISPLAY.MESSAGE.NUMBER% = 1
                 CALL DISPLAY.MESSAGE                                  ! 1.0 RC
                 CALL RESUME.INPUT                                     ! 1.0 RC
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
                 CALL CHAIN.TO.CALLER                                  ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF FUNCTION.KEY% = HELP.KEY% THEN                        \
              BEGIN
                 CALL SCREEN.HELP(6)
                 CALL RESTORE.FIELDS.06                                ! 1.0 RC

                 CURSOR.POSITION% = S6.OPERATOR.ID%
                 CALL PUT.CURSOR.IN.FIELD                              ! 1.0 RC

                 CALL GET.INPUT                                        ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              BEGIN
                 IF VALID.OPERATOR.ID THEN                             \
                    VALID.OPERATOR.ID.FOUND = TRUE                     \
                 ELSE                                                  \
                 BEGIN
                    ! B058 Invalid operator ID
                    DISPLAY.MESSAGE.NUMBER% = 58
                    CALL DISPLAY.MESSAGE                               ! 1.0 RC
                    CALL RESUME.INPUT                                  ! 1.0 RC
                 ENDIF
              ENDIF
        WEND

        IF VALID.OPERATOR.ID.FOUND AND                                 \
           NOT EXIT.KEY.PRESSED(6) THEN                                \
        BEGIN
           CALL CLEAR.MESSAGE                                          ! 1.0 RC

           STRING.DATA$ = OPERATOR.ID$
           CALL SET.FIELD                                              ! 1.0 RC

           IF FUNCTION.KEY% = ENTER.KEY% THEN                          \
           BEGIN
              IF VAL(OPERATOR.ID$) = 905 THEN                          \
              BEGIN
                 OLD.OPERATOR.ID$ = ""

                 CALL CLEAR.FIELDS.06                                  ! 1.0 RC

                 DISPLAY.MESSAGE.NUMBER% = 362
                 CALL DISPLAY.MESSAGE                                  ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF VAL(OPERATOR.NUMBER$) = VAL(OPERATOR.ID$) THEN        \
              BEGIN
                 OLD.OPERATOR.ID$ = ""

                 CALL CLEAR.FIELDS.06                                  ! 1.0 RC

                 ! B060 An operator cannot delete themselves
                 DISPLAY.MESSAGE.NUMBER% = 60
                 CALL DISPLAY.MESSAGE                                  ! 1.0 RC
              ENDIF                                                    \
              ELSE                                                     \
              IF OLD.OPERATOR.ID$ <> OPERATOR.ID$ THEN                 \
              BEGIN
                 AF.OPERATOR.NO$ = PACK$(RIGHT$(STRING$(8,"0") +       \
                                   OPERATOR.ID$,8))

                 IF READ.AF.ABREV = 0 THEN                             \
                 BEGIN
                    OLD.OPERATOR.ID$ = OPERATOR.ID$

                    CALL GET.OPERATOR.DETAILS                          ! 1.0 RC
                    CALL SET.OLD.OPAUD.DETAILS                         ! 1.0 RC
                    CALL RESTORE.FIELDS.06                             ! 1.0 RC

!                   CALL CHECK.FIELDS.06     ! Will only ever execute  ! 1.0 RC
                    CALL DELETE.AN.OPERATOR  ! DELETE.AN.OPERATOR path ! 1.0 RC
                 ENDIF                                                 \
                 ELSE                                                  \
                 BEGIN
                    OLD.OPERATOR.ID$ = ""

                    CALL CLEAR.FIELDS.06                               ! 1.0 RC

                    ! B171 Operator ID not currently in use
                    DISPLAY.MESSAGE.NUMBER% = 171
                    CALL DISPLAY.MESSAGE                               ! 1.0 RC
                 ENDIF
              ENDIF
           ENDIF
        ENDIF

END SUB                                                                ! 1.0 RC


\******************************************************************************
\***                                                                          *
\***    SUBROUTINE      :       PROCESS.SCREEN.06                             *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***    Control routine for the delete an operator screen                     *
\***                                                                          *
\******************************************************************************

SUB     PROCESS.SCREEN.06 ! Main process separated from module PSB9900 ! 1.0 RC
                          ! Not PUBLIC as name also used in PSB9900    ! 1.0 RC
        S% = 6

        CALL DISPLAY.SCREEN(6)

        OLD.OPERATOR.ID$   = ""
        OPERATOR.ID$       = ""
        OPERATOR.NAME$     = ""
        OPERATOR.PASSWORD$ = ""
        STAFF.NO$          = ""
        RECEIPT.NAME$      = ""
        EMPLOYEE.FLG$      = ""
        BIRTH.DATE$        = ""                                        ! 1.1 RC
        GROUP.CODE$        = ""
        MODEL.FLAG$        = ""

        CALL RESET.MODEL.FLAGS                                         ! 1.0 RC

        CALL RESTORE.FIELDS.06                                         ! 1.0 RC

        EXIT.KEY.PRESSED(6) = FALSE

        WHILE NOT EXIT.KEY.PRESSED(6)
              CALL GET.OPERATOR.ID.06                                  ! 1.0 RC
        WEND

END SUB                                                                ! 1.0 RC



\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  M A I N L I N E  C O D E                              *
\***                                                                          *
\******************************************************************************
\******************************************************************************


SUB PSB9902 PUBLIC

    ON ERROR GOTO ERROR.DETECTED

    CALL PROCESS.SCREEN.06

    EXIT SUB


STOP.PROGRAM:
    
    STOP


\******************************************************************************
\***
\***    ERROR.DETECTED:
\***
\******************************************************************************

\*****************************************************************************
\***
\***    ERROR.DETECTED:
\***    Increments ERROR.COUNT% by one and tests it against values greater 
\***    than one before any other commands executed.
\***    Further errors within ERROR.DETECTED causing control to be passed here
\***    again result in this test being failed and the immediate diversion of
\***    program control to STOP.PROGRAM.
\***    Calls STANDARD.ERROR.DETECTED to log Event 101 and display message B550.
\***    Chains to calling program PSB50
\***
\***..........................................................................


ERROR.DETECTED:

!   PRINT "ERROR DETECTED"
    ERROR.COUNT% = ERROR.COUNT% + 1

    IF ERROR.COUNT% = 1 THEN \
        BEGIN
        RESUME STOP.PROGRAM
        ENDIF

!   PRINT "MAIN: ERROR.DETECTED at " + \
!      MID$(TIME$,1,2) + ":" + MID$(TIME$,3,2) + ":" + MID$(TIME$,5,2)
!   PRINT "ERRN .... " + ERRNH ! Function call to translate ERRN
!   PRINT "ERRF% ... " + STR$(ERRF%)
!   PRINT "ERR ..... " + ERR
!   PRINT "ERRL .... " + STR$(ERRL)

    CALL STANDARD.ERROR.DETECTED (ERRN, ERRF%, ERRL, ERR)
 
    CHAIN.TO.PROG$ = "PSB50"
    CALL CHAIN.TO.CALLER

    RESUME STOP.PROGRAM

END SUB ! Corresponding to SUB PSB9902


\*****************************************************************************
\*****************************************************************************
\***
\***    End of module PSB9902
\***
\*****************************************************************************
\*****************************************************************************


