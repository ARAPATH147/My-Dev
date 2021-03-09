\********************************************************************
\********************************************************************
\***
\***
\***            PROGRAM         :       PSD85
\***            MODULE          :       PSD8500
\***            AUTHOR          :       Neil Bennett
\***            DATE WRITTEN    :       May 2010
\***
\********************************************************************
\***
\***    VERSION A.            NEIL BENNETT.              17 May 2010.
\***    Initial version.
\***
\***    VERSION B.            Stuart Highley.            17 June 2010.
\***    Do not display Status Updated if F3 pressed to quit update.
\***
\***    VERSION C.            Stuart Highley.            29 June 2010.
\***    CR1: Change padding of parcel number to match till app.
\***
\***    VERSION D.            Denesh Manoharan             05/09/2014
\***    FOD399 - Boots.com Enhancements
\***    The following changes are done as part of this project:
\***      -  Subroutine:PROCESS.SCREEN1 - trap F9.KEY%
\***         and execute PSD96.286 in background with BEMF message call.
\***      -  Subroutine:CHANGE.SCREEN1 - ensure F9PRINT option is always
\***         visible.
\***      -  Subroutine:INITIALISATION - Read days uncollected value
\***         from record 60 of SOFTS file.
\***      -  Function:disp$ - Display 'UNCOLLECTED - RETURN TO W/H?'
\***         for parcels with difference b/w current date and delivery
\***         date greater than days uncollected value.
\***
\***    VERSION E.            Christopher Kitto            16/09/2014
\***    FOD399 - Boots.com Enhancements
\***    Addressing the changes of CR02:
\***      -  Commented out the PSBCHN.* lines in ERR.DETECTED.
\***      -  Avoided the unnecessary opening of the BDCP file. BDCP file
\***         opened only when needed and closed after the need.
\***
\***    VERSION F.             Muthu Mariappan             24/09/2014
\***    FOD399 - Boots.com Enhancements
\***    Fixed Defect 1170 (Qc):
\***      -  The information message 'B388 Report Generation and
\***         printing initiated' is displayed only when there is a
\***         report to be printed
\***    Fixed Defect 1171 (Qc):
\***      -  The status of the parcels displayed in the controller
\***         screen is sorted
\***
\***    VERSION G.             Christopher Kitto           30/09/2014
\***    FOD399 - Boots.com Enhancements
\***    Fixed Defect 1189 (Qc):
\***      -  'In ROI stores, the Parcel management SCREEN TITLE is not
\***         displayed as BOOTS.IE'
\***
\***    VERSION H.             Dave Constable              07/03/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    User Story PMLA-17 & PMLA-58
\***
\***    VERSION I.             Dave Constable              11/03/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Corrected SONAR Critical errors for missing FUNC. and usage
\***    of the Standard Error Detected on 'on error'
\***    Changes for PMLA-20 and PMLA-18.
\***
\***    VERSION J.             Dave Constable              18/03/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Corrected Function buttons at screen load.
\***    Added message function to remove duplication of code.
\***    Changed screen titles to match screens.
\***
\***    VERSION K.             Dave Constable              08/04/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Corrected variable ordering from code review.
\***
\***    VERSION L.             Lino Jacob                  11/04/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User Story PMLA-16 : To disallow the deactivation of locations
\***      if there are parcels available at that location
\***    - Incorporated review comments from sprint 1
\***    - Removed previously commented code
\***
\***    VERSION M.             Lino Jacob                  18/04/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User Stories PMLA-85,15 : Various data validations on screen
\***        - Do not allow user to edit the default location (#1)
\***        - Data validations on Short and Long description fields
\***    - Removed previously commented code
\***
\***    VERSION N.             Lino Jacob                  22/04/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User Stories PMLA-85,15 : Changes to confirmation on F3
\***        - The confirmation prompt on F3 is changed to use display
\***          function DM.PROCESS.SCREEN parameter, this would help in
\***          validating the screen before file update.
\***
\***    VERSION O.             Lino Jacob                  28/04/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User Stories PMLA-15 : Validation for F6 activation           !XLJ
\***        - The description fields need to be validated while changing
\***          the status of a location from inactive to active
\***    - Removed previously commented code
\***
\***    VERSION P.             Lino Jacob                  20/05/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User Story PMLA-98 : Storage location 1
\***        - Location 1 cannot be chosen/edited from the controller
\***
\***    VERSION Q.             Christopher Kitto           27/06/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User story PMLA-132 : Incorporated review comments
\***      Commented out codes in previous version has been removed.
\***
\***    VERSION R.             Lino Jacob                  27/07/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - User story PMLA-213,214 - Location management screen          !XLJ
\***      Changed display to show only long name
\***      Added page up and page down functionality
\***    - User story PMLA-252 - 3 digit location display
\***
\***    VERSION S.             Arun Haridas                27/07/2016   !XLJ
\***    PRJ1361- Order & Collect Parcel Management
\***    - User story PMLA-228 : Changed all Boots ".com" and ".ie"
\***      titles to "Order & Collect".
\***    - User story PMLA-218 : Changes in the location management
\***      screen: The system shows a B463 message when same character
\***      is entered 3 or more times in the location description field.
\***      And, the system shows a B462 message when null or spaces are
\***      entered in the location description field.
\***    - User Story PMLA-197 : Storage location 1
\***      Display an error message while editing default location(1)
\***    - User Story PMLA-212 : After completing the data entry of one
\***      location, when we move to create another location
\***      description, a new B465 message is shown which prompts the
\***      user to activate the location. And if F6 is pressed, the
\***      location is activated and if we are ignoring it, i.e, if it
\***      is not activated, the location description field will be
\***      cleared.
\***
\***    VERSION T.             Arun Haridas                16/08/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2243: In the location management screen, the value
\***      of F6 key is "INACT" for inactive locations and "ACTIV" for
\***      active locations. But, expected value is "ACTIV" for inactive
\***      locations and "INACT" for active locations. Made the
\***      necessary changes.
\***    - Added a "HELP" value to the F1 key, in the location
\***      management screen.
\***
\***    VERSION U.             Christopher Kitto           18/08/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2558: Information was not correctly saved in the
\***      exact inactive location. Fixed the issue.
\***
\***    VERSION V.             Christopher Kitto           26/08/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2571: It should not be possible to make the
\***      location 001 - 'Booked In at Till' Inactive. Fixed the issue.
\***
\***    VERSION W.             Christopher Kitto           01/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2595: It was possible to enter same character
\***      sequentially 3 or more times with the combination of Upper
\***      and Lower cases in location description field, which should
\***      not be allowed. Fixed the issue.
\***
\***    VERSION X.             Lino Jacob                  06/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    User story PMLA-132 - Code review comments
\***    - Corrected version dates and some words in the version history
\***    - Updated the version tag (OLJ) for arranging the variables in
\***      in alphabetical order
\***    - Corrected the indentation of DM.FIELD.CHANGED subprogram
\***    - Removed previously commented code
\***
\***    VERSION Y.             Dave Consstable             13/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Fix for status description spacing and used descriptors
\***
\***    VERSION Z.             Christopher Kitto           16/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2594: When pressed 'Tab' key after modifying long
\***      description of location, the data was getting saved. Fixed
\***      issue. When pressed 'Tab' after modification, now system asks
\***      to save/not.
\***
\***    VERSION AA.            Christopher Kitto           20/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - QC Defect 2638: Inactive locations in the controller not
\***      clearing the description. Fixed the issue. Now the active
\***      locations when made Inactive by pressing F6 will have null
\***      description.
\***
\***    VERSION AB.            Dave Constable              26/09/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - Changed BEMF B459 message to use B466 as conflict with 16B
\***
\***    VERSION AC             Andrew Paron                05/10/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    - changed UPDATE.PARCEL.COUNT subroutine to handle if the
\***    location is blank(2020).
\***
\***    VERSION AD.            Christopher Kitto           07/10/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Fixed defects QC 2669 & 2673
\***    - QC defect 2669: Duplicate descriptions are accepted on a
\***      different location management screen instead of showing error
\***    - QC defect 2673: Inactivating an active location clears
\***      description without asking for confirmation F3 or ENTER.
\***
\***    VERSION AE.            Charles Skadorwa            19/10/2016
\***    PRJ1361- Order & Collect Parcel Management
\***    Fixed defect caused by fixing QC 2669 & 2673. Parcel Mgt screen
\***    displayed Subscript out of bounds error when you pressed any
\***    key.
\***       - modified display manager user exit subroutine
\***         SUB DM.FIELD.CHANGED().
\***       - removed commented out code.
\***
\***    VERSION AF.     Charles Skadorwa/Kiran Krishnan       13/06/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***    PMLA-310: In Location screen, description is blanked out when
\***              you fail to make a location inactive. Also resolved
\***              incorrect toggling of F6INACT button and repeat
\***              request to save change again when tabbing to next
\***              field.
\***
\***    VERSION AG.     Charles Skadorwa                      23/06/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***    PMLA-331: Introduce Book In functionality in O&C Parcel
\***              Management screen.
\***              F4 key will toggle to BOOKIN when a parcel with status
\***              Late Delivery or Expected Delivery is highlighted.
\***              When the parcel is booked in, F4 will change to CLLCT.
\***
\***    VERSION AH.     Charles Skadorwa                      28/06/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***    PMLA-331: Introduce Book In functionality in O&C Parcel
\***              Management screen.
\***              Additional logic to ensure that if the update to the
\***              CARTON file fails, then the BDCP Parcel record update
\***              is rolled back ie. write original record back and
\***              display message informing user to try again.
\***
\***    VERSION AI.     Charles Skadorwa                      17/08/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***    PMLA-407: Introduce near real-time Book In functionality.
\***              Added logic to add a record to the PSUTQ:
\***                 Type 1, Operation "B" - for F4 BOOKIN
\***                 Type 1, Operation "M" - for F10 LOCation
\***                 Type 5 - for F5 RETRN
\***              A file access message is displayed if the file cannot
\***              be updated for any reason (therefore no reason to
\***              read PSUCF file to check PSD86 status).
\***
\***    VERSION AJ.     Charles Skadorwa                      17/08/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***             Fix to set up PSUTQ Type 5 record as Order & Parcel
\***             numbers not set. PSUTQ record format changed.
\***
\***    VERSION AK      Kiran Krishnan                       07/09/2017
\***    PRJ2002- Order & Collect Parcel Management Phase 2
\***            Changes to make the data in PSUTQ ASCII and comma 
\***            seperated
\***
\********************************************************************
\********************************************************************

\********************************************************************
\********************************************************************
\***
\***    Module Overview
\***    ---------------
\***
\***      Boots.com Parcel Management Screen
\***
\***      Screen program to display all parcels in the BDCP file with
\***      their curren status. Program allows modification of parcel
\***      status from InStore to either collected/lost/returned.
\***
\***      A parcel with a 'Lost' status can be modified back to
\***      InStore with a found date/time.
\***
\***      If a parcel status has been modified externally (till?)
\***      then the program will NOT change the record but redisplay
\***      the new status with an information message.
\***
\********************************************************************
\********************************************************************

\********************************************************************
\***
\***    Function globals
\***
\********************************************************************

   %INCLUDE PSBF01G.J86         ! Application Logging
   %INCLUDE PSBF02G.J86         ! Reusable Function (UPDATE.DATE)       !DDM
   %INCLUDE PSBF20G.J86         ! Sess Num Utility
   %INCLUDE PSBF39G.J86         ! New DM Functions
   %INCLUDE PSBUSEG.J86         ! Chain parameters

   %INCLUDE BDCPDEC.J86         ! Boots.com Parcel File
   %INCLUDE CRTNDEC.J86         ! Directs Carton File                   !AGCS
   %INCLUDE PSUTQDEC.J86        ! Parcel Status Update Transaction File !AICS
   %INCLUDE SOFTSDEC.J86        ! Software Status File                  !DDM

\********************************************************************
\***
\***    PSD8500 variables
\***
\********************************************************************

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%
   INTEGER*2 GLOBAL SB.FILE.REP.NUM%
   INTEGER*2 GLOBAL SB.FILE.SESS.NUM%
   INTEGER*2 GLOBAL SB.INTEGER%

   STRING    GLOBAL BATCH.SCREEN.FLAG$
   STRING    GLOBAL CURRENT.CODE$
   STRING    GLOBAL FILE.OPERATION$
   STRING    GLOBAL MODULE.NUMBER$
   STRING    GLOBAL OPERATOR.NUMBER$
   STRING    GLOBAL SB.ACTION$
   STRING    GLOBAL SB.FILE.NAME$
   STRING    GLOBAL SB.STRING$

   INTEGER*1 CARTON.FILE.ERROR    ! TRUE if CARTON file error           !AHCS
   INTEGER*1 FALSE                ! Flag for SOFTS value check          !DDM
   INTEGER*1 LOC.STATUS.CHANGED   ! Boolean                             !LLJ
   INTEGER*1 PRINT.KEY            ! Flag for Print key Check            !FMM
   INTEGER*1 PSUTQ.FILE.ERROR     ! TRUE if PSUTQ file error            !AICS

   INTEGER*1 SCREEN.COMPLETE
   INTEGER*1 SOFTS.LIMIT.VAL.FOUND! Flag variable                       !DDM
   INTEGER*1 SOFTS.OPEN           ! Boolean                             !DDM
   INTEGER*1 TRUE                 ! Flag for SOFTS value check          !DDM

   INTEGER*1 bdcp.open%
   INTEGER*1 CRTN.OPEN                                                  !AGCS
   INTEGER*1 f.key%
   INTEGER*1 file.loaded%
   INTEGER*1 mismatch%

   INTEGER*2 DAYS.UNCOLLECTED%    ! No. of uncollected days             !DDM
   INTEGER*2 EVENT.NUMBER%
   INTEGER*2 LIMIT.LENGTH%        ! Length of the extracted days value  !DDM
   INTEGER*2 MATCH.POS%           ! To find the position                !DDM
   INTEGER*2 MESSAGE.NUMBER%
   INTEGER*2 RET.KEY%
   INTEGER*2 TEMP.REPORT.NUM%     ! Temporary report number for message !AICS

   INTEGER*2 blk.size%
   INTEGER*2 i%
   INTEGER*2 indx%
   INTEGER*2 j%
   INTEGER*2 k%
   INTEGER*2 last.blk%
   INTEGER*2 lpp1%
   INTEGER*2 max.scrn1%
   INTEGER*2 num.blks%
   INTEGER*2 num.recs%
   INTEGER*2 num.sect%
   INTEGER*2 p.cnt%
   INTEGER*2 rc%
   INTEGER*2 scrn1%

   STRING    ADX.MESSAGE$         ! Parameter Message                   !DDM
   STRING    ADX.NAME$            ! Program name                        !DDM
   STRING    ADX.PARM$            ! Parameter value                     !DDM
   STRING    BLANK.MSG$           ! To clear the status message field   !MLJ
   STRING    COMMA$               ! Comma as field delimiter            !AKKK
   STRING    CHAINING.TO.PROG$
   STRING    CURRENT.DATE$        ! Stores current date                 !DDM
   STRING    FILE.NO$
   STRING    LIMIT.STR$           ! To parse days uncollected value     !DDM

   STRING    PSUTQ.CURROP$        ! Holds PSUTQ Current Operation       !AICS
   STRING    PSUTQ.CURR.LOCON$    ! Holds current  parcel location      !AICS
   STRING    PSUTQ.PREV.LOCON$    ! Holds previous parcel location      !AICS
   STRING    RETURN.BY.DATE$      ! Delivery/found date+Uncollected days!DDM
   STRING    SAVED.ARRAY$         ! Saved array element for rollback    !AHCS
   STRING    SAVED.BDCP.STATUS$   ! Saved BDCP fields for rollback      !AHCS
   STRING    SAVED.BDCP.DEL.DATETIME$                                   !AHCS
   STRING    SAVED.BDCP.DEL.EXPORTED$                                   !AHCS

   STRING    SCREEN.TITLE$        ! To store the screen title           !GCK
   STRING    SCREEN2.TITLE$       ! To store the locations screen title !HDC
   STRING    SOFTS.REC.LABEL$     ! To store the ROI store label(EIRE)  !GCK
   STRING    STATUS.SORT.ORDER$   ! To store the sort order for status  !FMM
   STRING    VAR.STRING.1$
   STRING    VAR.STRING.2$
   STRING    WORK.SOFTS.RECORD$   ! Holds extracted string              !DDM

   STRING    blk$
   STRING    d.dt$
   STRING    form$
   STRING    p.arr$(1)
   STRING    p1$
   STRING    p2$
   STRING    p3$
   STRING    p4$
   STRING    p5$
   STRING    p6$
   STRING    page$
   STRING    rcd$
   STRING    sect$
   STRING    wk$
   STRING    x.dt$

\***********************************************************************!HDC
\***                                                                    !HDC
\***    Variables added for location management                         !HDC
\***                                                                    !HDC
\***********************************************************************!HDC
    STRING      F10.MESSAGE$                                            !RLJ
    STRING      LOCATION.ARRAY$(1)                                      !HDC
    STRING      ORDERS.OFFSET$                                          !HDC
    STRING      P7$, P8$                                                !HDC
    STRING      PAGE2$                                                  !HDC
    STRING      PROMPT.MESSAGE$                                         !JDC
    STRING      TITLE.END1$                                             !KDC
    STRING      TITLE.END2$                                             !KDC
    STRING      TITLE.START$                                            !JDC

    ! litterals for function key text                                   !IDC
    STRING      ACTIVATE.FN.TEXT$                                       !IDC
    STRING      DEACTIVATE.FN.TEXT$                                     !IDC
    STRING      LOCATION.FN.TEXT$                                       !IDC
    STRING      LOCATION1.DESC$                                         !RLJ
                                                                        !IDC
    INTEGER*1   ACTIVE.TO.BE.SAVED                                      !OLJ
    INTEGER*1   ANY.CHANGES.DONE                                        !RLJ
    INTEGER*1   BDCO.OPEN%                                              !HDC
    INTEGER*1   CHANGE.ACCEPTED      ! Checks if field change accepted  !ZCK
    INTEGER*1   CONFIRM.PROMPT                                          !NLJ
    INTEGER*1   ERROR.ON.LOCATION    ! Flag to check if there is error  !TAH
    INTEGER*1   F10.NOT.ALLOWED                                         !IDC
    INTEGER*1   INITIAL.FIELD.INDEX% ! Default location field index     !TAH
    INTEGER*1   KEY.ACTION%          ! 1 - F6, 2 - BTab/Up, 3 - Tab/Down!ADCK
    INTEGER*1   MANAGE.LOCATION                                         !HDC
    INTEGER*1   NOT.DEFAULT.LOCATION ! To check if default location     !TAH
    INTEGER*1   RC.INT1%                                                !OLJ
    INTEGER*1   SCREEN.CHANGED       ! Flag to check if screen changed  !TAH
    INTEGER*1   SCREEN.COMPLETE2                                        !HDC
    INTEGER*1   VALID.CHANGE                                            !RLJ

    INTEGER*2   CURRENT.INDEX%       ! Current location index of field  !UCK
    INTEGER*2   CURRENT.LOCATION%    ! To check current location field  !TAH
    INTEGER*2   CURRENT.ORDER.I%                                        !HDC
    INTEGER*2   CURRENT.ORDER.INDX%                                     !HDC
    INTEGER*2   CURRENT.SCREEN%                                         !IDC
    INTEGER*2   INACTIVE.CHECK%                                         !SAH
    INTEGER*2   LPP2%                                                   !HDC
    INTEGER*2   LOC.ARRAY.LIMIT%                                        !HDC
    INTEGER*2   LOC.FIELDS.PER.LINE%                                    !HDC
    INTEGER*2   LOC.FIELDS.START%                                       !HDC
    INTEGER*2   LOC.RECORDS.ON.PAGE%                                    !RLJ
    INTEGER*2   LOCATION.NEW%                                           !HDC
    INTEGER*2   LOCATION.STATUS.NEW%                                    !IDC
    INTEGER*2   MAX.PARCELS%                                            !HDC
    INTEGER*2   MAX.SCRN2%                                              !HDC
    INTEGER*2   ORDERS.OFFSET%                                          !HDC
    INTEGER*2   ORDER.PARCELS%(2)                                       !HDC
    INTEGER*2   P.CNT2%                                                 !HDC
    INTEGER*2   P.FULL%                                                 !HDC
    INTEGER*2   PARCEL.COUNT%                                           !LLJ
    INTEGER*2   SCRN2%                                                  !HDC
    INTEGER*2   TMP.LOCATION%                                           !HDC


\********************************************************************   !HDC
\***                                                                    !HDC
\***    Variables added for loaction file                               !HDC
\***                                                                    !HDC
\********************************************************************   !HDC
%INCLUDE BDCLODEC.J86                                                   !HDC
%INCLUDE BDCODEC.J86                                                    !HDC
\********************************************************************
\***
\***    External functions
\***
\********************************************************************

   %INCLUDE PSBF01E.J86        ! APPLICATION LOG
   %INCLUDE PSBF02E.J86        ! Reusable function (UPDATE.DATE)        !DDM
   %INCLUDE PSBF20E.J86        ! SESSION NUMBER UTILITY
   %INCLUDE PSBF24E.J86        ! STANDARD ERROR DETECTED
   %INCLUDE PSBF39E.J86        ! Display Manager

   %INCLUDE CSORTDEC.J86       ! Assembler csort

   %INCLUDE ADXSTART.J86       ! To start background applications       !DDM
   %INCLUDE BTCSTR.J86         ! String functions                       !MLJ
   %INCLUDE CMPDATE.J86        ! Compare dates                          !DDM

   %INCLUDE BDCPEXT.J86        ! Boots.com Parcel File
   %INCLUDE CRTNEXT.J86        ! Directs Carton File                    !AGCS
   %INCLUDE PSUTQEXT.J86       ! Parcel Status Update Transaction File  !AICS
   %INCLUDE SOFTSEXT.J86       ! Software Status File                   !DDM
\********************************************************************   !HDC
\***                                                                    !HDC
\***    Variables added for loaction file                               !HDC
\***                                                                    !HDC
\********************************************************************   !HDC
%INCLUDE BDCLOEXT.J86                                                   !HDC
%INCLUDE BDCOEXT.J86                                                    !HDC
\********************************************************************
\***
\***    PSD8500 functions
\***
\********************************************************************

\********************************************************************
\***
\***    FUNCTION        ENDF
\***
\***    DM ENDF Function
\***
\********************************************************************

   DEF ENDF EXTERNAL               REM ENDF method of return.
      INTEGER ENDF
   FEND



\********************************************************************   !HDC
\***                                                                    !HDC
\***    Functions added for location management                         !HDC
\***                                                                    !HDC
\********************************************************************   !HDC
FUNCTION GETN2(P1$,P2) EXTERNAL                                         !HDC
 INTEGER*2 GETN2                                                        !HDC
 STRING P1$                                                             !HDC
 INTEGER*2 P2                                                           !HDC
END FUNCTION                                                            !HDC
                                                                        !HDC
!*********************************************************************  !HDC
!                                                                       !HDC
! PUTN2                                                                 !HDC
!                                                                       !HDC
! This routine inserts a two byte integer into a string.                !HDC
! P2 is the offset within the string and P3 is the source integer       !HDC
!                                                                       !HDC
!*********************************************************************  !HDC
 FUNCTION PUTN2(P1$,P2,P3) EXTERNAL                                     !HDC
 INTEGER*1 PUTN2                                                        !IDC
 STRING P1$                                                             !HDC
 INTEGER*2 P2,P3                                                        !HDC
 END FUNCTION                                                           !HDC

!***********************************************************************!IDC
!***                                                                    !IDC
!***    FUNCTION        FUNC.GET.LOCATION$(F.RECORD%,F.NAME%)           !IDC
!***                                                                    !IDC
!***    Returns the location information; status, description (short or !IDC
!***    long) based on F.DETAIL;                                        !IDC
!***    0=Status, 1=Short, 2=Long                                       !IDC
!***    and the passed location number in F.RECORD%.                    !IDC
!***                                                                    !IDC
!***    Hard coded defaults in case a parcel has no value set.          !IDC
!***                                                                    !IDC
!***********************************************************************!IDC
FUNCTION FUNC.GET.LOCATION$(F.RECORD%,F.DETAIL%) PUBLIC                 !IDC
    STRING      FUNC.GET.LOCATION$                                      !IDC
    INTEGER*2   F.DETAIL%                                               !IDC
    INTEGER*2   F.RECORD%                                               !IDC
    STRING      F.LOCATION$                                             !IDC

                                                                        !IDC
    F.LOCATION$ = ""                                                    !IDC
    IF F.RECORD% = 0 THEN BEGIN           ! nothing set yet             !IDC
        ! litterals hard-coded here as fallback if file error           !IDC
        !                       12345678901234567890                    !IDC
        BDCLOCON.SHORT.NAME$ = "NOT SET   "                             !IDC
        BDCLOCON.LONG.NAME$  = "NO LOCATION ON FILE "                   !IDC
        BDCLOCON.STATUS$     = "I"                                      !IDC
    ENDIF ELSE BEGIN                                                    !IDC
        BDCLOCON.RECORD.NUM% = F.RECORD%                                !IDC

        ! Opening BDCLOCON file for reading                             !QCK
        IF NOT BDCLOCON.OPEN THEN BEGIN                                 !QCK
            CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%                  !QCK
            FILE.OPERATION$     = "O"  ! Open                           !QCK
                                                                        !QCK
            ! Open BDCLOCON file                                        !QCK
            OPEN BDCLOCON.FILE.NAME$ DIRECT RECL BDCLOCON.RECL% AS \    !QCK
            BDCLOCON.SESS.NUM%                                          !QCK
                                                                        !QCK
            ! Set the open flag                                         !QCK
            BDCLOCON.OPEN = TRUE                                        !QCK
        ENDIF                                                           !QCK

        CALL READ.BDCLOCON                                              !IDC

        ! Closing BDCLOCON file                                         !QCK
        IF BDCLOCON.OPEN THEN BEGIN                                     !QCK
            CLOSE BDCLOCON.SESS.NUM%                                    !QCK
                                                                        !QCK
            ! Reset the flag                                            !QCK
            BDCLOCON.OPEN = FALSE                                       !QCK
        ENDIF                                                           !QCK

    ENDIF                                                               !IDC
                                                                        !IDC
                                                                        !IDC
    IF F.DETAIL% = 0 THEN BEGIN                                         !IDC
        F.LOCATION$ = BDCLOCON.STATUS$                                  !IDC
    ENDIF ELSE \                                                        !IDC
    IF F.DETAIL% = 1 THEN BEGIN                                         !IDC
        F.LOCATION$ = BDCLOCON.SHORT.NAME$                              !IDC
    ENDIF ELSE \                                                        !IDC
    IF F.DETAIL% = 2 THEN BEGIN                                         !IDC
        F.LOCATION$ = BDCLOCON.LONG.NAME$                               !IDC
    ENDIF                                                               !IDC
                                                                        !IDC
    FUNC.GET.LOCATION$ = F.LOCATION$                                    !IDC
                                                                        !IDC

END FUNCTION                                                            !IDC

!***********************************************************************!JDC
!***                                                                    !JDC
!***    FUNCTION        FUNC.PROMPT.FOR.RESPONSE%(F.MESSAGE$)           !JDC
!***                                                                    !JDC
!***    Displays the passsed message and passes back the response key.  !JDC
!***    F.MESSAGE$ is the prompt to dispay                              !JDC
!***    This function will only accept F3 or ENETER as valid key press. !JDC
!***    Uses ENTER.KEY% and F3.KEY% globals                             !JDC
!***                                                                    !JDC
!***********************************************************************!JDC
FUNCTION FUNC.PROMPT.FOR.RESPONSE%(F.MESSAGE$)                          !JDC
    INTEGER*2   FUNC.PROMPT.FOR.RESPONSE%                               !JDC
    STRING      F.MESSAGE$                                              !JDC
    INTEGER*2   F.KEY.PRESS%                                            !JDC
                                                                        !JDC
    F.KEY.PRESS% = 99               ! set as invalid key                !JDC
    WHILE F.KEY.PRESS% <> ENTER.KEY%                \                   !JDC
    AND F.KEY.PRESS% <> F3.KEY%                                         !JDC
        F.KEY.PRESS% = DM.INVISIBLE.INPUT("221 "    \                   !JDC
                             + "'"+F.MESSAGE$+"' "  \                   !JDC
                             + "MESSAGE")                               !JDC
    WEND                                                                !JDC
    FUNC.PROMPT.FOR.RESPONSE% = F.KEY.PRESS%                            !JDC
                                                                        !JDC
END FUNCTION                                                            !JDC
\***********************************************************************!MLJ
\* FUNC.IS.ALPHANUMERIC:                                                !MLJ
\* This function checks if the data is alphanumeric or not              !MLJ
\*    - returns -1 if alphanumeric, 0 if not alphanumeric               !MLJ
\*                                                                      !MLJ
\***********************************************************************!MLJ
FUNCTION FUNC.IS.ALPHANUMERIC (DATA$)                                   !MLJ
                                                                        !MLJ
    INTEGER*2   FUNC.IS.ALPHANUMERIC                                    !MLJ
    STRING      DATA$                                                   !MLJ
    STRING      TEST$                                                   !MLJ
                                                                        !MLJ
    DATA$ = UCASE$(DATA$)                                               !MLJ
    DATA$ = TRANSLATE$(DATA$, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ #", \MLJ
                              "#####################################_") !MLJ
    TEST$ = STRING$(LEN(DATA$), "#")                                    !MLJ
    FUNC.IS.ALPHANUMERIC = DATA$ = TEST$                                !MLJ
                                                                        !MLJ
END FUNCTION                                                            !MLJ
\***********************************************************************!MLJ
\* FUNC.IS.SAME.SEQ:                                                    !MLJ
\* This function checks if the data has sequentially same characters    !MLJ
\*    - returns 0 if the same character repeats 3 times                 !MLJ
\*                                                                      !MLJ
\***********************************************************************!MLJ
FUNCTION FUNC.IS.SAME.SEQ(DATA$)                                        !MLJ
                                                                        !MLJ
    INTEGER*2   COUNTER%                                                !MLJ
    INTEGER*2   FIND.POS%                                               !MLJ
    INTEGER*2   FUNC.IS.SAME.SEQ                                        !MLJ
    STRING      DATA$                                                   !MLJ
                                                                        !MLJ
    FIND.POS% = 0                !Initialise position                   !MLJ
    COUNTER%  = 1                !Initialise counter                    !MLJ
    WHILE (FIND.POS% = 0) AND (COUNTER% < LEN(DATA$))                   !MLJ
        IF MID$(DATA$,COUNTER%,1) <> " " THEN BEGIN                     !MLJ
            FIND.POS% = MATCH(STRING$(3,UCASE$(MID$(DATA$,COUNTER%,1))),\WCK
                              UCASE$(DATA$),1)                          !WCK
        ENDIF                                                           !MLJ
        COUNTER% = COUNTER% + 1                                         !MLJ
    WEND                                                                !MLJ
                                                                        !MLJ
    FUNC.IS.SAME.SEQ = FIND.POS% = 0                                    !MLJ
                                                                        !MLJ
END FUNCTION                                                            !MLJ
\***********************************************************************!MLJ
\* FUNC.IS.SAME.NAME:                                                   !MLJ
\* This function checks if same data is already there on other fields   !MLJ
\*    - returns 0 if the same data is already there                     !MLJ
\*                                                                      !MLJ
\***********************************************************************!MLJ
FUNCTION FUNC.IS.SAME.NAME(DATA$)                                       !MLJ
                                                                        !MLJ
    INTEGER*1   EXIT.LOOP         !BOOLEAN                              !MLJ
!   INTEGER*1   START.POS.SET     !BOOLEAN                              !ADCK
                                                                        !ADCK
    INTEGER*2   COUNTER%                                                !MLJ
    INTEGER*2   CURR.INDEX%       !Current index on screen              !ADCK
    INTEGER*2   CURR.RECORD%      !Current record for the field         !ADCK
    INTEGER*2   FUNC.IS.SAME.NAME                                       !MLJ
                                                                        !MLJ
    STRING      BLANK.DATA$  !BLANK DATA                                !ADCK
    STRING      COMP.FIELD$                                             !MLJ
    STRING      CURR.FIELD$                                             !MLJ
    STRING      DATA$                                                   !MLJ
                                                                        !MLJ
    ! Initialise the flag                                               !ADCK
    FUNC.IS.SAME.NAME = TRUE                                            !ADCK
    BLANK.DATA$ =  STRING$(LEN(DATA$)," ")                              !ADCK
    ! If the field description value is null then exit function         !ADCK
    IF (DATA$ = "" OR DATA$ = BLANK.DATA$) THEN BEGIN                   !ADCK
        EXIT FUNCTION                                                   !ADCK
    ENDIF                                                               !ADCK
                                                                        !ADCK
    ! Opening BDCLOCON file                                             !ADCK
    IF NOT BDCLOCON.OPEN THEN BEGIN                                     !ADCK
        CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%                      !ADCK
        FILE.OPERATION$     = "O"   ! Open                              !ADCK
                                                                        !ADCK
        ! Open the file BDCLOCON                                        !ADCK
        ! Open error is captured in ON ERROR subroutine ERR.DETECTED    !ADCK
        OPEN BDCLOCON.FILE.NAME$ DIRECT RECL BDCLOCON.RECL% AS \        !ADCK
        BDCLOCON.SESS.NUM%                                              !ADCK
                                                                        !ADCK
        ! Set the open flag                                             !ADCK
        BDCLOCON.OPEN = TRUE                                            !ADCK
    ENDIF                                                               !ADCK
                                                                        !ADCK
    COUNTER%    = 1                         ! Initialise counter        !ADCK
    CURR.FIELD$ = UCASE$(FIELD$(DM.FIELD%)) ! Current field in U.Case   !ADCK
    EXIT.LOOP   = FALSE                     ! Initialising flag         !ADCK
                                                                        !ADCK
    CALL TRIM(CURR.FIELD$)                  !Trim the spaces            !ADCK
                                                                        !ADCK
    ! Finding the current record no.                                    !ADCK
    CURR.INDEX%  = DM.FIELD% + \                                        !ADCK
                   ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))     !ADCK
    CURR.RECORD% = CURR.INDEX%/LOC.FIELDS.PER.LINE%                     !ADCK
                                                                        !ADCK
    ! Compare the current field with location desc. of every record     !ADCK
    WHILE ((COUNTER% <= BDCLOCON.TOTAL.RECORDS%) AND (NOT EXIT.LOOP))   !ADCK
        BDCLOCON.RECORD.NUM% = COUNTER%                                 !ADCK
        IF BDCLOCON.RECORD.NUM% <> CURR.RECORD% THEN BEGIN              !ADCK
                                                                        !ADCK
            ! Read from BDCLOCON and if read was success                !ADCK
            IF NOT READ.BDCLOCON THEN BEGIN                             !ADCK
                COMP.FIELD$ = UCASE$(BDCLOCON.LONG.NAME$)               !ADCK
                CALL TRIM(COMP.FIELD$)     !Trim the spaces             !ADCK
                ! If current field value is equal to that of the record !ADCK
                IF CURR.FIELD$ = COMP.FIELD$                  AND    \  !ADCK
                   BDCLOCON.LONG.NAME$ <> STRING$(LEN(DATA$)," ")    \  !ADCK
                                                              THEN BEGIN!ADCK
                    EXIT.LOOP         = TRUE   ! To exit the while loop !ADCK
                    FUNC.IS.SAME.NAME = FALSE  ! Set the flag           !ADCK
                ENDIF                                                   !ADCK
            ENDIF ELSE BEGIN                                            !ADCK
                ! If reading BDCLOCON was failure                       !ADCK
                EXIT.LOOP = TRUE                                        !ADCK
                ! Prompt screen if error on updating                    !ADCK
                CALL DM.HIDE.FN.KEY(10)                                 !ADCK
                                                                        !ADCK
                EVENT.NUMBER%   = 106                                   !ADCK
                MESSAGE.NUMBER% = 221                                   !ADCK
                VAR.STRING.1$   = STR$(BDCLOCON.REPORT.NUM%) +         \!ADCK
                                  ERR                                   !ADCK
                VAR.STRING.2$   = "Unable to read location file "  +   \!ADCK
                                  "BDCLOCON. Press ESC."                !ADCK
                                                                        !ADCK
                CALL APPLICATION.LOG( MESSAGE.NUMBER%,                 \!ADCK
                                      VAR.STRING.1$, VAR.STRING.2$,    \!ADCK
                                      EVENT.NUMBER% )                   !ADCK
                ! Set the counter to exit the while loop                !ADCK
                COUNTER% = BDCLOCON.TOTAL.RECORDS% + 1                  !ADCK
            ENDIF                                                       !ADCK
                                                                        !ADCK
        ENDIF                                                           !ADCK
        COUNTER% = COUNTER% + 1                                         !ADCK
    WEND                                                                !ADCK
    ! Closing BDCLOCON file                                             !ADCK
    IF BDCLOCON.OPEN THEN BEGIN                                         !ADCK
        CLOSE BDCLOCON.SESS.NUM%                                        !ADCK
                                                                        !ADCK
        ! Reset the flag                                                !ADCK
        BDCLOCON.OPEN = FALSE                                           !ADCK
    ENDIF                                                               !ADCK
                                                                        !ADCK
END FUNCTION                                                            !ADCK
\********************************************************************
\***
\***    FUNCTION        SET.F.KEYS
\***
\***    Set F Key visability according to status of passed index
\***
\********************************************************************

FUNCTION SET.F.KEYS(index%)

    INTEGER*2 index%
    INTEGER*1 f.key%
    INTEGER*2 F.LOCATION%                                               !IDC
    INTEGER*2 KEY%          ! Stores key entered                        !TAH
    STRING    F.STATUS$                                                 !IDC

    KEY% = ENDF             ! Get the key entered                       !TAH
    IF CURRENT.SCREEN% = 1 THEN BEGIN                                   !IDC
        ! The variable f.key% is updated as 2 for the statuses          !IDC
        ! "Uncollected-Return to W/H?", "Instore-Awaiting collection"   !IDC
        ! and "Instore-Found". f.key% is updated as 1 for the status    !IDC
        ! "Lost"                                                        !IDC
        F.STATUS$ = LEFT$(P.ARR$(INDEX%),1)                             !LLJ
        IF F.STATUS$ = "3"  \ Uncollected - return to W/H?              !IDC
        OR F.STATUS$ = "4"  \ Instore-Awaiting collection               !IDC
        OR F.STATUS$ = "5"  \ Instore-Found                             !IDC
        THEN BEGIN                                                      !IDC
            F.KEY% = 2                                                  !LLJ
        ENDIF ELSE \                                                    !IDC
        IF F.STATUS$ = "8" \  Lost                                      !IDC
        THEN BEGIN                                                      !IDC
            f.key% = 1                                                  !FMM
        ENDIF ELSE BEGIN                                                !FMM
            f.key% = 0                                                  !FMM
        ENDIF                                                           !FMM

        IF f.key% > 0 THEN BEGIN
            IF f.key% = 1 THEN BEGIN
                CALL DM.SHOW.FN.KEY( 6, "FOUND")
                CALL DM.HIDE.FN.KEY( 5)
                CALL DM.HIDE.FN.KEY( 4)
            ENDIF ELSE IF f.key% = 2 THEN BEGIN
                CALL DM.SHOW.FN.KEY( 6, "LOST ")
                CALL DM.SHOW.FN.KEY( 5, "")
                CALL DM.SHOW.FN.KEY( 4, "CLLCT ")                       !AGCS
            ENDIF
        ENDIF ELSE BEGIN
            IF F.STATUS$ < "3"  THEN BEGIN ! Expected or Late Delivery  !AGCS
                CALL DM.SHOW.FN.KEY( 4, "BOOKIN")                       !AGCS
            ENDIF ELSE BEGIN                                            !AGCS
                CALL DM.HIDE.FN.KEY( 4)
            ENDIF                                                       !AGCS

            CALL DM.HIDE.FN.KEY( 6)
            CALL DM.HIDE.FN.KEY( 5)
        ENDIF

        ! if Location Status does not allows Location allocation        !IDC
        F10.NOT.ALLOWED = GETN2(RIGHT$(P.ARR$(INDX%),2),0)              !IDC
        IF F10.NOT.ALLOWED THEN BEGIN                                   !IDC
            ! supress the F10 for locations not allowed                 !IDC
            CALL DM.HIDE.FN.KEY(10)                                     !IDC
        ENDIF ELSE BEGIN                                                !IDC
            CALL DM.SHOW.FN.KEY(10, LOCATION.FN.TEXT$)                  !IDC
        ENDIF                                                           !IDC
                                                                        !IDC
    ENDIF ELSE \                                                        !IDC
    IF CURRENT.SCREEN% = 2 \                                            !IDC
    THEN BEGIN                                                          !IDC
        !Displaying the help value in the help button - F1 key.         !TAH
        CALL DM.SHOW.FN.KEY(1,"HELP")                                   !TAH
        F.LOCATION% = (INDEX%)/LOC.FIELDS.PER.LINE%                     !TAH

        !Checking whether we are traversing backward                    !TAH
        !Modified - traversing backward only if field change is accepted!ZCK
        !Since the value of KEY% may get changed adding usage of key    !ADCK
        !action                                                         !ADCK
        IF (KEY% = BTAB.KEY% OR KEY% = UP.KEY% OR KEY.ACTION% = 2) AND \!ADCK
            CHANGE.ACCEPTED THEN BEGIN                                  !ZCK
            F.LOCATION% = CURRENT.LOCATION%                             !TAH
            !If screen has not changed, location field is not 1st and   !TAH
            !there is no error on location, decrement location field    !TAH
            IF SCREEN.CHANGED = FALSE    AND                       \    !TAH
               F.LOCATION% > 1           AND                       \    !TAH
               ERROR.ON.LOCATION = FALSE THEN BEGIN                     !TAH
                !Traversing backward                                    !TAH
                F.LOCATION% = F.LOCATION% - 1                           !TAH
            ENDIF                                                       !TAH
        !Since the value of KEY% may get changed adding usage of key    !ADCK
        !action                                                         !ADCK
        ENDIF ELSE IF (KEY% = TAB.KEY%    OR                       \    !ADCK
                       KEY% = DOWN.KEY%   OR                       \    !ADCK
                       KEY.ACTION% = 3)   AND                      \    !ADCK
                      CHANGE.ACCEPTED THEN BEGIN                        !ZCK
            !Checking whether we are traversing forward if field change !ZCK
            !accepted                                                   !ZCK
            F.LOCATION% = CURRENT.LOCATION%                             !TAH
            !If screen has not changed, location field is not last one  !TAH
            !and there is no error on location, increment location field!TAH
            IF SCREEN.CHANGED = FALSE      AND                     \    !TAH
               F.LOCATION% < 180           AND                     \    !TAH
               ERROR.ON.LOCATION = FALSE   THEN BEGIN                   !TAH
                !Traversing forward                                     !TAH
                F.LOCATION% = F.LOCATION% + 1                           !TAH
            ENDIF                                                       !TAH
        ENDIF ELSE IF KEY% = HOME.KEY% THEN BEGIN                       !TAH
            !If HOME key is pressed, get the location field of first    !TAH
            !record on the existing screen                              !TAH
            CURRENT.INDEX% =  INITIAL.FIELD.INDEX%  +              \    !TAH
                              ((SCRN2% - 1)         *              \    !TAH
                               (LPP2% - LOC.FIELDS.START% + 1))         !TAH
            F.LOCATION%    = (CURRENT.INDEX%)/LOC.FIELDS.PER.LINE%      !TAH
        ENDIF ELSE IF KEY% = END.KEY% THEN BEGIN                        !TAH
            !If END key is pressed, get the location field of last      !TAH
            !record on the existing screen                              !TAH
            CURRENT.INDEX% = LPP2%         +                       \    !TAH
                             ((SCRN2% - 1) *                       \    !TAH
                              (LPP2% - LOC.FIELDS.START% + 1))          !TAH
            F.LOCATION%    = (CURRENT.INDEX%)/LOC.FIELDS.PER.LINE%      !TAH
        ENDIF ELSE BEGIN                                                !TAH
            F.LOCATION% = (INDEX%)/LOC.FIELDS.PER.LINE%                 !TAH
        ENDIF                                                           !TAH

        ! If status of the location is "A" (active)                     !TAH
        IF FUNC.GET.LOCATION$(F.LOCATION%,0) = "A" THEN BEGIN           !IDC
            ! if active location                                        !IDC
            IF MANAGE.LOCATION THEN BEGIN                               !IDC
                IF FUNC.GET.LOCATION$(F.LOCATION%,2) =             \    !TAH
                   "Booked in at Till" THEN BEGIN                       !TAH
                    CALL DM.HIDE.FN.KEY(6)                              !TAH
                    NOT.DEFAULT.LOCATION = FALSE                        !TAH
                ENDIF ELSE BEGIN                                        !TAH
                    ! If the key entered is F6 swap, status change      !AFCS
                    ! is accepted and the parcel count in location      !AFCS
                    ! is <= 0 then change F6 key value to 'ACTIV'       !AFCS
                    ! Since the value of KEY% may get changed adding    !ADCK
                    ! usage of key action                               !ADCK
                    IF (KEY% = F6.KEY% OR KEY.ACTION% = 1)    AND   \   !ADCK
                       (CHANGE.ACCEPTED = TRUE)               AND   \   !AFCS
                       (BDCLOCON.PARCEL.COUNT% <= 0)          THEN BEGIN!AFCS
                        CALL DM.SHOW.FN.KEY(6, ACTIVATE.FN.TEXT$)       !TAH
                    ENDIF ELSE IF KEY% <> F6.KEY% THEN BEGIN            !ADCK
                        ! If key pressed is not F6, show F6 as 'INACT'  !ADCK
                        CALL DM.SHOW.FN.KEY(6, DEACTIVATE.FN.TEXT$)     !TAH
                    ENDIF                                               !TAH
                ENDIF                                                   !TAH
            ENDIF ELSE BEGIN                                            !IDC
                F10.NOT.ALLOWED = 0                                     !IDC
                CALL DM.SHOW.FN.KEY(10, LOCATION.FN.TEXT$)              !IDC
            ENDIF                                                       !IDC
        ENDIF ELSE BEGIN                                                !IDC
            ! if inactive location                                      !IDC
            IF MANAGE.LOCATION THEN BEGIN                               !IDC
                ! If the key entered is F6, and status change is        !ADCK
                ! accepted then change F6 key value to 'INACT'          !ADCK
                ! Since the value of KEY% may get changed adding        !ADCK
                ! usage of key action                                   !ADCK
                IF (KEY% = F6.KEY% OR KEY.ACTION% = 1)        AND   \   !ADCK
                   (CHANGE.ACCEPTED = TRUE)                   THEN BEGIN!ADCK
                    CALL DM.SHOW.FN.KEY(6, DEACTIVATE.FN.TEXT$)         !TAH
                ENDIF ELSE IF KEY% <> F6.KEY% THEN BEGIN                !ADCK
                    ! If key pressed is not F6, show F6 as 'ACTIV'      !ADCK
                    CALL DM.SHOW.FN.KEY(6, ACTIVATE.FN.TEXT$)           !IDC
                ENDIF                                                   !TAH
            ENDIF ELSE BEGIN                                            !IDC
                ! hide selection and don't allow F10                    !IDC
                F10.NOT.ALLOWED = 1                                     !IDC
                CALL DM.HIDE.FN.KEY(10)                                 !IDC
            ENDIF                                                       !IDC
        ENDIF                                                           !IDC

        ! If default location (1st location field), hide F6 button      !TAH
        IF NOT NOT.DEFAULT.LOCATION THEN BEGIN                          !TAH
            !If the flag is false, it means that the current location   !TAH
            !is the default location. So the F6 key is kept hidden.     !TAH
            CALL DM.HIDE.FN.KEY(6)                                      !TAH
        ENDIF                                                           !TAH
        CURRENT.LOCATION% = F.LOCATION%                                 !TAH
        !Reset the location error check flag to false                   !TAH
        ERROR.ON.LOCATION = FALSE                                       !TAH

        IF SCRN2% < MAX.SCRN2% THEN BEGIN                               !LLJ
            CALL DM.SHOW.FN.KEY(8 , "")                                 !JDC
        ENDIF ELSE BEGIN                                                !JDC
            CALL DM.HIDE.FN.KEY( 8)                                     !JDC
        ENDIF                                                           !JDC
        IF SCRN2% > 1 THEN BEGIN                                        !LLJ
            CALL DM.SHOW.FN.KEY( 7, "")                                 !JDC
        ENDIF ELSE BEGIN                                                !JDC
            CALL DM.HIDE.FN.KEY( 7)                                     !JDC
        ENDIF                                                           !JDC

        CALL DM.HIDE.FN.KEY( 9)                                         !JDC
        IF MANAGE.LOCATION THEN BEGIN                                   !JDC
            CALL DM.HIDE.FN.KEY(10)                                     !JDC
        ENDIF ELSE BEGIN                                                !JDC
            CALL DM.SHOW.FN.KEY(10, "")                                 !JDC
        ENDIF                                                           !JDC

        !Reset the screen changed flag                                  !TAH
        IF SCREEN.CHANGED = TRUE THEN BEGIN                             !TAH
            SCREEN.CHANGED = FALSE                                      !TAH
        ENDIF                                                           !TAH
    ENDIF                                                               !IDC

END FUNCTION

!***********************************************************************
!***
!***    FUNCTION        FUNC.UPDATE.ALL.LOCATION.RECORDS%               !IDC
!***
!***    Updates all of the Location records from the display array
!***    LOCATION.ARRAY$ and returns 0 for OK or the number of the failing
!***    record otherwise.
!***
!***    Calculation by field on the logic below;
!***    Position in DMED array  1XX 2XX 3XX 4XX    5XX 6XX 7XX 8XX
!***    Position on DMED screen 4XX 5XX 6XX 7XX    8XX 9XX 10X 11X
!***    Record in file              1                  2
!***    File field position     1   R   2   3      1   R   2   3
!***
!***********************************************************************
    FUNCTION FUNC.UPDATE.ALL.LOCATION.RECORDS%                          !QCK
        INTEGER*2 COUNTER%                                              !SAH
        INTEGER*2 FUNC.UPDATE.ALL.LOCATION.RECORDS%                     !QCK
        INTEGER*2 F.RECORD%                                             !QCK
        INTEGER*2 F.FIELD%                                              !QCK
        INTEGER*2 F.ERR%                                                !QCK

        ! default as OK                                                 !QCK
        F.ERR% = 0                                                      !QCK

        ! Opening BDCLOCON file                                         !QCK
        IF NOT BDCLOCON.OPEN THEN BEGIN                                 !QCK
            CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%                  !QCK
            FILE.OPERATION$     = "O"   ! Open                          !QCK
                                                                        !QCK
            ! Open the file BDCLOCON                                    !QCK
            ! Open error is captured in ON ERROR subroutine ERR.DETECTED!QCK
            OPEN BDCLOCON.FILE.NAME$ DIRECT RECL BDCLOCON.RECL% AS \    !QCK
            BDCLOCON.SESS.NUM%                                          !QCK
                                                                        !QCK
            ! Set the open flag                                         !QCK
            BDCLOCON.OPEN = TRUE                                        !QCK
        ENDIF                                                           !QCK
                                                                        !RLJ
        COUNTER% = LOC.FIELDS.START%                                    !SAH
        ! On all screen fields                                          !RLJ
        WHILE COUNTER% <=  LPP2%                                        !SAH

            INDX% = COUNTER% + \                                        !SAH
                   ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))     !RLJ
            ! For valid indexes                                         !RLJ
            IF INDX% <= P.CNT2% AND COUNTER% <= LPP2% THEN BEGIN        !SAH
                ! For each BDCLOCON record                              !RLJ
                IF MOD(INDX%,LOC.FIELDS.PER.LINE%) = 0 THEN BEGIN       !RLJ
                                                                        !RLJ
                    BDCLOCON.RECORD.NUM% = (INDX%)/LOC.FIELDS.PER.LINE% !RLJ
                    ! If exceeded the file                              !RLJ
                    IF BDCLOCON.RECORD.NUM% > BDCLOCON.TOTAL.RECORDS%  \!RLJ
                                                              THEN BEGIN!RLJ
                        COUNTER% = LPP2% + 1      !Exit loop            !SAH
                                                                        !RLJ
                    ENDIF ELSE BEGIN                                    !RLJ
                        ! Read error on BDCLOCON                        !RLJ
                        IF READ.BDCLOCON THEN BEGIN                     !RLJ
                            COUNTER% = LPP2% + 1   !Exit loop           !SAH
                        ENDIF ELSE BEGIN                                !RLJ
                            BDCLOCON.STATUS$    =                      \!SAH
                            LOCATION.ARRAY$(COUNTER%)                   !SAH
                            BDCLOCON.LONG.NAME$ =                      \!SAH
                            LOCATION.ARRAY$(COUNTER% + 2)               !SAH
                                                                        !RLJ
                            ! If write error                            !RLJ
                            IF WRITE.BDCLOCON THEN BEGIN                !RLJ
                                F.ERR%   = BDCLOCON.RECORD.NUM%         !RLJ
                                COUNTER% = LPP2% + 1   !Exit loop       !SAH
                            ENDIF                                       !RLJ
                                                                        !RLJ
                        ENDIF                                           !RLJ
                    ENDIF                                               !RLJ
                ENDIF                                                   !RLJ
            ENDIF                                                       !RLJ
                                                                        !RLJ
            COUNTER% = COUNTER% + 1        !Increment the sccreen field !SAH
                                                                        !RLJ
       WEND                                                             !RLJ

        ! Closing BDCLOCON file                                         !QCK
        IF BDCLOCON.OPEN THEN BEGIN                                     !QCK
            CLOSE BDCLOCON.SESS.NUM%                                    !QCK
                                                                        !QCK
            ! Reset the flag                                            !QCK
            BDCLOCON.OPEN = FALSE                                       !QCK
        ENDIF                                                           !QCK

        FUNC.UPDATE.ALL.LOCATION.RECORDS% = F.ERR%                      !QCK

    END FUNCTION                                                        !QCK

\***********************************************************************!ADCK
\***                                                                    !ADCK
\***    SUB        :       SUB.UPDATE.LOCATIONS                         !ADCK
\***                                                                    !ADCK
\***    # Prompts to the confirm the change                             !ADCK
\***    # If pressed 'ENTER',                                           !ADCK
\***        - If the key action was F6 (ACTION% = 1), then clear the    !ADCK
\***          field and update in the BDCLOCON file                     !ADCK
\***        - If the key action was ARROW/TAB (ACTION% = 2) or DOWN/BTAB!ADCK
\***          then just update the new field value in BDCLOCON file     !ADCK
\***    # If pressed 'F3'                                               !ADCK
\***        - Display the previous value of the field                   !ADCK
\***                                                                    !ADCK
\***********************************************************************!ADCK
SUB SUB.UPDATE.LOCATIONS(ACTION%, DM.FIELD%, VALUE$, VALID, UPDATE)     !ADCK
                                                                        !ADCK
    INTEGER*1 ACTION%    ! 1 if F6 key press, 2 for ARROW/TAB key press !ADCK
    INTEGER*1 UPDATE     !Return TRUE if updated output fields          !ADCK
    INTEGER*1 VALID      !Return FALSE if field invalid                 !ADCK
                                                                        !ADCK
    INTEGER*2 DM.FIELD%  !Field modified                                !ADCK
                                                                        !ADCK
    STRING    VALUE$     !New value for field (can be modified)         !ADCK
                                                                        !ADCK
    ! Calculate the correct index of the field                          !ADCK
    INDX% = CURRENT.INDEX% +                                           \!ADCK
            ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))            !ADCK
                                                                        !ADCK
    ! Set the message for BEMF 221                                      !ADCK
    PROMPT.MESSAGE$ = "Update all changes on screen? " +               \!ADCK
                      "ENTER to confirm or F3 to cancel."               !ADCK
    RET.KEY% = FUNC.PROMPT.FOR.RESPONSE%(PROMPT.MESSAGE$)               !ADCK
    ! If 'ENTER' pressed                                                !ADCK
    IF RET.KEY% = ENTER.KEY% THEN BEGIN                                 !ADCK
        IF ACTION% = 1 THEN BEGIN                                       !ADCK
            !Commenting below code as part of US310 O&C Phase2 project  !AFKK
            !This was blanking the location name incorrectly            !AFKK
!           LOCATION.ARRAY$(CURRENT.INDEX%) = " "                       !AFKK
            VALID  = TRUE                                               !AACK
            UPDATE = TRUE                                               !AACK
        ENDIF                                                           !ADCK
        IF MANAGE.LOCATION THEN BEGIN                                   !ADCK
            ! Set the flag - Accept the field change                    !ADCK
            CHANGE.ACCEPTED = TRUE                                      !ADCK
            ! Update all records                                        !ADCK
            LOCATION.NEW% = FUNC.UPDATE.ALL.LOCATION.RECORDS%           !ADCK
                                                                        !ADCK
            IF LOCATION.NEW% > 0 THEN BEGIN                             !ADCK
                ! Prompt screen if error on updating                    !ADCK
                CALL DM.HIDE.FN.KEY(10)                                 !ADCK
                EVENT.NUMBER%   = 106                                   !ADCK
                MESSAGE.NUMBER% = 221                                   !ADCK
                VAR.STRING.1$   = STR$(LOCATION.NEW%)           +    \  !ADCK
                                  ERR                                   !ADCK
                VAR.STRING.2$   = "Unable to complete update, " +    \  !ADCK
                                  "fail at location "           +    \  !ADCK
                                  STR$(LOCATION.NEW%)           +    \  !ADCK
                                  ". " + "Press ESC."                   !ADCK
                                                                        !ADCK
                CALL APPLICATION.LOG( MESSAGE.NUMBER%, VAR.STRING.1$,\  !ADCK
                                      VAR.STRING.2$, EVENT.NUMBER% )    !ADCK
                                                                        !ADCK
            ENDIF                                                       !ADCK
        ENDIF                                                           !ADCK
    ENDIF ELSE IF RET.KEY% = F3.KEY% THEN BEGIN                         !ADCK
        ! If 'F3' pressed                                               !ADCK
        ! Set back the previous description to display                  !ADCK
        LOCATION.ARRAY$(CURRENT.INDEX%) =                            \  !ADCK
                        FUNC.GET.LOCATION$(INDX%/LOC.FIELDS.PER.LINE%,2)!ADCK
        ! Setting the flags as input is not valid and                   !ADCK
        ! field change happened                                         !ADCK
        VALID = FALSE                                                   !ADCK
        UPDATE = TRUE                                                   !ADCK
        ! Field change is not accepted                                  !ADCK
        CHANGE.ACCEPTED = FALSE                                         !ADCK
        ! To avoid any error message being displayed                    !ADCK
        CALL DM.MESSAGE(STR$(CURRENT.INDEX%),BLANK.MSG$)                !ADCK
    ENDIF                                                               !ADCK
    ! Clear status message                                              !ADCK
    CALL DM.STATUS(BLANK.MSG$)                                          !ADCK
                                                                        !ADCK
END SUB                                                                 !ADCK

\********************************************************************
\***
\***    SUB        :       DM.FIELD.CHANGED                             !AECS
\***
\***    PERFORMS TASKS WHEN CERTAIN FIELDS HAVE BEEN ALTERED
\***
\********************************************************************

    SUB DM.FIELD.CHANGED (DM.SCREEN%,                                \
                          DM.FIELD%,                                 \
                          VALUE$,                                    \
                          VALID,                                     \
                          UPDATE) PUBLIC

        INTEGER*1 UPDATE     !RETURN TRUE IF UPDATED OUTPUT FIELDS      !OLJ
        INTEGER*1 VALID      !RETURN FALSE IF FIELD INVALID             !OLJ

        INTEGER*2 DM.FIELD%  !FIELD MODIFIED                            !OLJ
        INTEGER*2 DM.SCREEN% !CURRENT SCREEN NUMBER                     !OLJ
        INTEGER*2 KEY%                                                  !OLJ
        ! for Locations we need to move by field for editing but by     !IDC
        ! record for selection so this manages the next position        !IDC
        INTEGER*2 S.FIELD.MOVE%                                         !IDC
        INTEGER*2 S.LOCATION%                                           !IDC

        STRING    BLANK.DATA$!BLANK DATA                                !OLJ
        STRING    VALUE$     !NEW VALUE FOR FIELD (CAN BE MODIFIED)


        KEY% = ENDF                                                     !JDC
        IF DM.SCREEN% = 1 THEN BEGIN
            indx% = ((scrn1% -1) *lpp1%) + (DM.FIELD% -1)
            IF KEY% = TAB.KEY%                                         \
            OR ((KEY% = NEXT.KEY%                                      \
                  OR KEY% = DOWN.KEY% ) AND DM.FIELD% < (lpp1% +1))    \
                                                         THEN BEGIN
                IF indx% < p.cnt% THEN                                 \
                    indx% = indx% +1
            ENDIF ELSE IF KEY% = BTAB.KEY%                             \
                OR ((KEY% = PREV.KEY%                                  \
                    OR KEY% = UP.KEY%) AND DM.FIELD% > 2) THEN BEGIN
                indx% = indx% -1
            ENDIF ELSE IF KEY% = HOME.KEY% THEN BEGIN
                indx% = ((scrn1% -1) *lpp1%) +1
            ENDIF ELSE IF KEY% = END.KEY%  THEN BEGIN
                indx% = ((scrn1% -1) *lpp1%) + lpp1%
                IF indx% > p.cnt% THEN indx% = p.cnt%
            ENDIF

        ENDIF ELSE IF DM.SCREEN% = 2 THEN BEGIN                         !IDC
            !Set the default location check flag to true                !TAH
            NOT.DEFAULT.LOCATION = TRUE                                 !TAH

            !Reset the field change accept flag to true                 !ZCK
            CHANGE.ACCEPTED = TRUE                                      !ZCK

            !Reset the key action                                       !ADCK
            KEY.ACTION% = 0                                             !ADCK

            INDX% = DM.FIELD% + \                                       !RLJ
                  ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))      !RLJ
            S.LOCATION% = INDX%/LOC.FIELDS.PER.LINE%                    !MLJ

            ! Store the current field on screen for future use          !ADCK
            CURRENT.INDEX% = DM.FIELD%                                  !ADCK

            !If location management and default location                !SAH
            IF MANAGE.LOCATION AND S.LOCATION% = 1 AND KEY% <> F3.KEY% \!SAH
              AND DM.CHANGED.FLAG(2) = TRUE           THEN BEGIN        !SAH
                ! Keep the original data                                !SAH
                LOCATION.ARRAY$(DM.FIELD%) = LOCATION1.DESC$            !SAH
                !Reset the changed flag                                 !SAH
                RC.INT1% = DM.CHANGED.FLAG(0)                           !SAH
                FIELD$(1) = "464 MESSAGE"                               !SAH
                VALID  = FALSE                                          !SAH
                UPDATE = TRUE                                           !SAH
                !Set the location error check flag to true              !TAH
                ERROR.ON.LOCATION = TRUE                                !TAH
                                                                        !SAH
            !If location management and not default location            !MLJ
            ENDIF ELSE IF MANAGE.LOCATION AND S.LOCATION% <> 1 AND     \!SAH
              KEY% <> F3.KEY% THEN BEGIN                                !PLJ
                BLANK.DATA$ =  STRING$(LEN(VALUE$)," ")                 !OLJ

                !If not alphanumeric                                    !SAH
                IF NOT FUNC.IS.ALPHANUMERIC(VALUE$) THEN BEGIN          !SAH
                    !Set the field as blanks                            !MLJ
                    LOCATION.ARRAY$(DM.FIELD%) = " "                    !MLJ
                    !Set the error message                              !MLJ
                    FIELD$(1) = "461 MESSAGE"                           !MLJ
                                                                        !MLJ
                    VALID  = FALSE                                      !MLJ
                    UPDATE = TRUE                                       !MLJ
                    !Field change is not accepted                       !AFKK
                    CHANGE.ACCEPTED = FALSE                             !AFKK
                    !Set the location error check flag to true          !TAH
                    ERROR.ON.LOCATION = TRUE                            !TAH

                !If same characters are coming sequentially 3 or        !SAH
                !more times                                             !SAH
                ENDIF ELSE IF NOT FUNC.IS.SAME.SEQ(VALUE$) THEN BEGIN   !SAH
                    !Set the field as balnks                            !SAH
                    LOCATION.ARRAY$(DM.FIELD%) = " "                    !SAH
                    !Setting the new error message                      !SAH
                    FIELD$(1) = "463 MESSAGE"                           !SAH
                    VALID = FALSE                                       !SAH
                    UPDATE = TRUE                                       !SAH
                    !Field change is not accepted                       !AFKK
                    CHANGE.ACCEPTED = FALSE                             !AFKK
                    !Set the location error check flag to true          !TAH
                    ERROR.ON.LOCATION = TRUE                            !TAH
                                                                        !MLJ
                !Description uniqueness check if there is field change  !AACK
                ENDIF ELSE IF (FUNC.GET.LOCATION$(S.LOCATION%,2) <>    \!ADCK
                               VALUE$)                           AND   \!ADCK
                              (NOT FUNC.IS.SAME.NAME(VALUE$)) THEN BEGIN!ADCK
                    !Set the field as blanks                            !MLJ
                    LOCATION.ARRAY$(DM.FIELD%) = " "                    !MLJ
                    !Set the error message                              !MLJ
                    FIELD$(1) = "460 MESSAGE"                           !MLJ
                                                                        !MLJ
                    VALID  = FALSE                                      !MLJ
                    UPDATE = TRUE                                       !MLJ
                    !Field change is not accepted                       !AFKK
                    CHANGE.ACCEPTED = FALSE                             !AFKK
                    !Set the location error check flag to true          !TAH
                    ERROR.ON.LOCATION = TRUE                            !TAH
                !If Active location                                     !AACK
                ENDIF ELSE IF FUNC.GET.LOCATION$(S.LOCATION%,0) = "A"  \!AACK
                                                              THEN BEGIN!AACK
                    ! If the values are null and key pressed is not F6  !AACK
                    IF (VALUE$ = "" OR VALUE$ = BLANK.DATA$) AND       \!AACK
                                             KEY% <> F6.KEY% THEN BEGIN !AACK
                        VALUE$ = STRING$(LEN(VALUE$),"*")               !MLJ
                        !Set the field as blanks                        !MLJ
                        LOCATION.ARRAY$(DM.FIELD%) = " "                !MLJ
                        !Set the error message                          !MLJ
                        FIELD$(1) = "462 MESSAGE"                       !MLJ
                                                                        !MLJ
                        VALID  = FALSE                                  !MLJ
                        UPDATE = TRUE                                   !MLJ
                        !Set the location error check flag to true      !TAH
                        ERROR.ON.LOCATION = TRUE                        !TAH
                    ! If the key pressed is F6                          !ADCK
                    ENDIF ELSE IF KEY% = F6.KEY% THEN BEGIN             !AACK
                        ! Set key action to 1 as key pressed is F6      !ADCK
                        KEY.ACTION% = 1                                 !ADCK
                        CALL SUB.UPDATE.LOCATIONS(KEY.ACTION%,         \!ADCK
                                       DM.FIELD%, VALUE$, VALID, UPDATE)!ADCK
                    ENDIF                                               !AACK

                !If changing the status from I to A                     !OLJ
                ENDIF ELSE IF KEY% = F6.KEY% AND                       \!OLJ
                            FUNC.GET.LOCATION$(S.LOCATION%,0) = "I"    \!OLJ
                            THEN BEGIN                                  !OLJ
!                   !If short and long description are blanks           !RLJ
                    IF LOCATION.ARRAY$(DM.FIELD%) =  BLANK.DATA$       \!RLJ
                                                            THEN BEGIN  !RLJ
                        VALUE$ = STRING$(LEN(VALUE$),"*")               !OLJ
                        !Set the error message                          !OLJ
                        FIELD$(1) = "462 MESSAGE"                       !OLJ
                                                                        !OLJ
                        VALID  = FALSE                                  !OLJ
                        UPDATE = TRUE                                   !OLJ
                        ! Field change is not accepted                  !AFKK
                        CHANGE.ACCEPTED = FALSE                         !AFKK
                        !Set the location error check flag to true      !TAH
                        ERROR.ON.LOCATION = TRUE                        !TAH
                    ENDIF ELSE BEGIN                                    !OLJ
                        ! Save the key action as 1 (since F6 pressed)   !ADCK
                        KEY.ACTION% = 1                                 !ADCK
                        !Set the flag for confirmation                  !OLJ
                        ACTIVE.TO.BE.SAVED = DM.CHANGED.FLAG(2)         !OLJ
                        !Reset the changed flag                         !OLJ
                        RC.INT1% = DM.CHANGED.FLAG(0)                   !OLJ
                        !Reset the inactive checking flag               !SAH
                        INACTIVE.CHECK% = 0                             !SAH
                    ENDIF                                               !OLJ
                !Code for checking whether the location is INACTIVE and !SAH
                !that the location is edited.                           !SAH
                ENDIF ELSE IF FUNC.GET.LOCATION$(S.LOCATION%,0) = "I"  \!SAH
                    AND LOCATION.ARRAY$(DM.FIELD%) <>  BLANK.DATA$ AND \!SAH
                    FUNC.GET.LOCATION$(S.LOCATION%,2) <>               \!SAH
                    LOCATION.ARRAY$(DM.FIELD%) THEN BEGIN               !SAH
                    !Checking whether any other key other than F6 is    !SAH
                    !pressed and whether the location is inactive.      !SAH
                    IF KEY% <> F6.KEY% AND INACTIVE.CHECK% = 1         \!XLJ
                                                             THEN BEGIN !SAH
                        !Clearing the location description field        !SAH
                        LOCATION.ARRAY$(DM.FIELD%) = " "                !SAH
                        VALID  = TRUE                                   !SAH
                        UPDATE = TRUE                                   !SAH
                        !Reset the inactive checking flag               !SAH
                        INACTIVE.CHECK% = 0                             !SAH
                    ENDIF ELSE BEGIN                                    !SAH
                    !Sets the new prompt message                        !SAH
                        FIELD$(1) = "465 MESSAGE"                       !SAH
                        VALID  = FALSE                                  !SAH
                        UPDATE = TRUE                                   !SAH
                        !Set the location error check flag to true      !TAH
                        ERROR.ON.LOCATION = TRUE                        !TAH
                        !Set the inactive checking flag                 !SAH
                        INACTIVE.CHECK% = 1                             !SAH
                    ENDIF                                               !SAH
                ENDIF                                                   !SAH
                VALID.CHANGE = VALID                                    !RLJ
                ! Check if the change is valid                          !RLJ
                IF VALID.CHANGE = TRUE AND DM.CHANGED.FLAG(2) = TRUE   \!RLJ
                                                              THEN BEGIN!RLJ
                    ANY.CHANGES.DONE = TRUE       !Data change happened !RLJ
                ENDIF                                                   !RLJ
            ENDIF                                                       !MLJ

            !Don't change anything if Function or ENTER Key             !JDC
            IF KEY% <> ENTER.KEY% \                                     !JDC
            AND KEY% > 0 \                                              !JDC
            THEN BEGIN                                                  !JDC
                ! Set the key action                                    !ADCK
                ! If the key pressed is F6 then set key action as 1     !ADCK
                IF KEY% = F6.KEY% THEN BEGIN                            !ADCK
                    KEY.ACTION% = 1                                     !ADCK
                ENDIF                                                   !ADCK
                ! If the key pressed is BackTab/Up Key then set key     !ADCK
                ! action as 2                                           !ADCK
                IF (KEY% = BTAB.KEY% OR KEY% = UP.KEY%) THEN BEGIN      !ADCK
                    KEY.ACTION% = 2                                     !ADCK
                ENDIF                                                   !ADCK
                ! If the key pressed is Tab/Down Key then set key       !ADCK
                ! action as 3                                           !ADCK
                IF (KEY% = BTAB.KEY% OR KEY% = UP.KEY%) THEN BEGIN      !ADCK
                    KEY.ACTION% = 3                                     !ADCK
                ENDIF                                                   !ADCK

                IF (KEY% = BTAB.KEY% OR KEY% = UP.KEY%    OR            \ZCK
                    KEY% = TAB.KEY%  OR KEY% = DOWN.KEY%) AND           \ZCK
                   VALID = TRUE                           AND           \ZCK
                   FUNC.GET.LOCATION$(INDX%/LOC.FIELDS.PER.LINE%, 2) <> \ZCK
                                                       VALUE$ THEN BEGIN!ZCK
                                                                        !AACK
                                                                        !ADCK
                    CALL SUB.UPDATE.LOCATIONS(KEY.ACTION%, DM.FIELD%,  \!ADCK
                                                  VALUE$, VALID, UPDATE)!ADCK
                ENDIF                                                   !ZCK
            ENDIF                                                       !JDC

            INDX% = CURRENT.INDEX% + \                                  !AECS
                        ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))!AECS
        ENDIF
        CALL SET.F.KEYS(INDX%)                                          !LLJ
        ! Reset SCREEN.CHANGED flag screen                              !TAH
        IF SCREEN.CHANGED = TRUE THEN BEGIN                             !TAH
            SCREEN.CHANGED = FALSE                                      !TAH
        ENDIF                                                           !TAH
    END SUB

\********************************************************************
\***
\***    FUNCTION        fmt.dt$
\***
\***    Pass Packed Date (and time)
\***    - Return formatted date (and time) dd/mm/yy( HH:MM)
\***
\********************************************************************

   FUNCTION fmt.dt$(dt$)
      STRING fmt.dt$
      STRING dt$
      STRING tmp$

   IF LEN(dt$) < 3 THEN BEGIN
      tmp$ = "Invalid."
   ENDIF ELSE IF LEFT$(dt$,3) = STRING$(3,CHR$(0)) THEN BEGIN
      tmp$ = "        "
   ENDIF ELSE BEGIN
      tmp$ = UNPACK$(MID$(dt$, 3, 1)) + "/"                         \
           + UNPACK$(MID$(dt$, 2, 1)) + "/"                         \
           + UNPACK$(MID$(dt$, 1, 1))
   ENDIF

   IF LEN(dt$) > 3 THEN BEGIN
      IF LEN(dt$) = 5                                               \
      OR LEN(dt$) = 6 THEN BEGIN
         tmp$ = tmp$ + " "                                          \
              + UNPACK$(MID$(dt$, 4, 1)) + ":"                      \
              + UNPACK$(MID$(dt$, 5, 1))
      ENDIF
   ENDIF

   fmt.dt$ = tmp$
   tmp$ = ""

   END FUNCTION

!***********************************************************************!HDC
!* Code change H block marker - START                                   !HDC
!***********************************************************************!HDC
!***********************************************************************
!***
!***    FUNCTION        FUNC.GET.FIELD.DISPLAY2$(F.INDEX%)              !IDC
!***
!***    character diplay field
!***    F.INP% is the current index from the display (starting at 0)
!***
!***********************************************************************
    FUNCTION FUNC.GET.FIELD.DISPLAY2$(F.INDEX%)                         !IDC
        STRING    FUNC.GET.FIELD.DISPLAY2$                              !IDC
        INTEGER*2 F.INDEX%      ! input field number
        INTEGER*2 F.POS%        ! calculated postion of record data

        IF F.INDEX% >= LOC.FIELDS.START% THEN BEGIN
            F.POS% = MOD(F.INDEX%+LOC.FIELDS.PER.LINE%, \               !IDC
                         LOC.FIELDS.PER.LINE%) + 1                      !IDC

            IF F.POS% = 1 THEN BEGIN
                ! Status
                FUNC.GET.FIELD.DISPLAY2$ = BDCLOCON.STATUS$             !IDC
            ENDIF ELSE \
            IF F.POS% = 2 THEN BEGIN
                ! record number
                FUNC.GET.FIELD.DISPLAY2$ = \                            !IDC
                 RIGHT$("000"+STR$(BDCLOCON.RECORD.NUM%),3)             !RLJ
            ENDIF ELSE \
            IF F.POS% = 3 THEN BEGIN                                    !RLJ
                ! long location name
                FUNC.GET.FIELD.DISPLAY2$ = BDCLOCON.LONG.NAME$          !IDC
            ENDIF
        ENDIF

    END FUNCTION

!***********************************************************************!HDC
!* Code change H block marker - END                                     !HDC
!***********************************************************************!HDC


\********************************************************************
\***
\***    FUNCTION        disp$(inp%)
\***
\***    Return 78 character diplay line
\***
\********************************************************************

   FUNCTION disp$(inp%)
      STRING    disp$
      INTEGER*2 inp%

      STRING    tmp$
      STRING    tm1$
      STRING    tm2$                                                    !CSH

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* extract status and status date to work variables
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ! set off until status check allows it                            !IDC
      LOCATION.STATUS.NEW% = 1                                          !IDC
      tm1$  = TRANSLATE$(LEFT$(p.arr$(inp%),1), "12345678", "OORRRCUL") !FMM
      x.dt$ = MID$(p.arr$(inp%), 14, 6)

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* Mark all zero order numbers as UNKNOWN
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      IF MID$(p.arr$(inp%), 2, 5) = STRING$(5, CHR$(0FFh)) THEN     \
         tmp$ = "UNKNOWN   " + " "                                  \
      ELSE                                                          \
         tmp$ = UNPACK$(MID$(p.arr$(inp%), 2, 5)) + " "             !

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* Add the parcel number and the formatted delivery date
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      tm2$ = UNPACK$(MID$(p.arr$(inp%), 7, 4))                          !CSH
      IF LEFT$(tm2$, 1) = "0" THEN BEGIN                                !CSH
          tm2$ = RIGHT$(tm2$, 7) + " "                                  !CSH
      ENDIF                                                             !CSH

      tmp$ = tmp$ + \                                                   !CSH
             tm2$ + " " + \                                             !CSH
             fmt.dt$(MID$(p.arr$(inp%),11, 3)) + " "                    !CSH

    !*******************************************************************!HDC
    !* Get the location in case we need it later and check if nothing   !HDC
    !* or empty (spaces/PD 2020) in the record and force as 0           !HDC
    !*******************************************************************!HDC

      BDCP.LOC.CURRENT% = GETN2(P.ARR$(INP%),22)                        !LLJ
      TMP.LOCATION% = BDCP.LOC.CURRENT%                                 !HDC
      IF TMP.LOCATION% = 0  \       ! either set to nothing             !HDC
      OR TMP.LOCATION% > 999\       ! or out of range                   !RLJ
      THEN BEGIN                                                        !HDC
          TMP.LOCATION% = 0         !                                   !HDC
          BDCP.LOC.CURRENT% = 0     !                                   !HDC
      ENDIF                                                             !HDC

      BDCP.LOC.STATUS% = GETN2(P.ARR$(INP%),24)                         !LLJ
      BDCP.LOC.STATUS% = 0      ! set as allowed as default             !IDC

      ! as long as it has arrived show it's last location recorded      !IDC
      IF TM1$ <> "O" THEN BEGIN                                         !LLJ
          ! Get the shortname for default location                      !RLJ
          IF TMP.LOCATION% = 1 THEN BEGIN                               !RLJ
              TMP$ = TMP$ +                                            \!RLJ
                   LEFT$(FUNC.GET.LOCATION$(TMP.LOCATION%,1),10) + " " \!RLJ
                      + RIGHT$("000"+STR$(BDCP.LOC.CURRENT%),3)+ " "    !RLJ
          ENDIF ELSE BEGIN                                              !RLJ
              TMP$ = TMP$ +                                            \!RLJ
                   LEFT$(FUNC.GET.LOCATION$(TMP.LOCATION%,2),10) + " " \!RLJ
                      + RIGHT$("000"+STR$(BDCP.LOC.CURRENT%),3)+ " "    !RLJ
          ENDIF                                                         !RLJ
      ENDIF ELSE BEGIN                                                  !IDC
          TMP$ = TMP$ + STRING$(15," ")                                 !YDC
      ENDIF                                                             !IDC
    !*******************************************************************!HDC

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* Add the status and the required message/date/time
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      IF tm1$ = "O" THEN BEGIN
         IF DATE$ > UNPACK$(LEFT$(x.dt$,3)) THEN                    \
            tmp$ = tmp$ + "Late "                                       !YDC

            tmp$ = tmp$ + "Delivery Expected on "                   \   !YDC
                 + fmt.dt$(LEFT$(x.dt$,3))                              !YDC
      ENDIF ELSE IF tm1$ = "R" THEN BEGIN
         LOCATION.STATUS.NEW% = 0                                       !IDC
         ! Used for status 'R' Ready in store. If status date [X.DT$]   !DDM
         ! is 0's then update global variable F02.DATE$ with delivery   !DDM
         ! date else update with found date.                            !DDM
         IF X.DT$ = STRING$(6,CHR$(0)) THEN BEGIN                       !DDM
             ! Assigning delivery date to global variable               !DDM
             F02.DATE$ = UNPACK$(MID$(P.ARR$(INP%), 11, 3))             !DDM
         ENDIF ELSE BEGIN                                               !DDM
             ! Assigning found date to global variable                  !DDM
             F02.DATE$ = UNPACK$(MID$(X.DT$,1,3))                       !DDM
         ENDIF                                                          !DDM

         ! For status 'R': Check difference between today's date and    !DDM
         ! delivery date/found date. If difference is greater than      !DDM
         ! Uncollected days value then display the status as            !DDM
         ! 'UNCOLLECTED - RETURN TO W/H?'.

         ! Calculating return by date                                   !DDM
         CALL UPDATE.DATE(DAYS.UNCOLLECTED%)                            !DDM
         RETURN.BY.DATE$ = F02.DATE$                                    !DDM

         ! If Return by date is lesser than today's date                !DDM
         IF DATE.LT(RETURN.BY.DATE$, CURRENT.DATE$) THEN BEGIN          !DDM
            TMP$ = TMP$ + "Uncollected - Return to W/H?"                !DDM
         ENDIF ELSE BEGIN                                               !DDM
            tmp$ = tmp$ + "In Store "
           IF x.dt$ = STRING$(6,CHR$(0)) THEN BEGIN
            ENDIF ELSE BEGIN
                 tmp$ = tmp$ + "Found on " + fmt.dt$(x.dt$)
            ENDIF
         ENDIF                                                          !DDM

      ! text length reduced to allow space for location                 !HDC
      ENDIF ELSE IF tm1$ = "C" THEN BEGIN
            TMP$ = TMP$ + "Collected "                           \      !LLJ
                 + "On " + fmt.dt$(x.dt$)
      ENDIF ELSE IF tm1$ = "L" THEN BEGIN
            TMP$ = TMP$ + "Lost "                                \      !LLJ
                 + "On " + fmt.dt$(x.dt$)
      ENDIF ELSE IF tm1$ = "U" THEN BEGIN
            TMP$ = TMP$ +                                        \      !YDC
                 + "Returned to W/H on " + fmt.dt$(x.dt$)
      ENDIF

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* Set output for return
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      disp$ = LEFT$(tmp$ + STRING$(78," "), 78)

      tm1$ = ""
      tmp$ = ""
      CALL PUTN2(P.ARR$(INP%),24,LOCATION.STATUS.NEW%)                  !LLJ

   END FUNCTION


\********************************************************************
\***
\***    Start of Mainline
\***
\********************************************************************

   ON ERROR GOTO ERR.DETECTED

   CHAINING.TO.PROG$ = "B50  "      ! for B553 msg (default=PSB50)

   %INCLUDE PSBUSEE.J86

START.PROGRAM:

   PSBCHN.APP = "ADX_UPGM:PSD85.286"

   OPERATOR.NUMBER$   = PSBCHN.OP
   MODULE.NUMBER$     = "PSD8500"

   MANAGE.LOCATION = 0          ! default off                           !HDC
   ! Menu sequence 4/6/4/2                                              !IDC
   IF MID$(PSBCHN.MENCON,4,1) = "2" THEN BEGIN                          !IDC
       MANAGE.LOCATION = -1     ! set location management only          !HDC
   ENDIF                                                                !HDC

   GOSUB INITIALISATION
   BATCH.SCREEN.FLAG$ = "S"                                             !ECK
   CALL DM.INIT
   RET.KEY% = ENTER.KEY%

   WHILE RET.KEY% <> F3.KEY%                                        \
     AND RET.KEY% <> ESC.KEY%

      IF MANAGE.LOCATION THEN BEGIN                                     !HDC
          GOSUB DISPLAY.SCREEN2                                         !HDC
          GOSUB PROCESS.SCREEN2                                         !HDC
      ENDIF ELSE BEGIN                                                  !HDC
          GOSUB DISPLAY.SCREEN1                                         !HDC
          GOSUB PROCESS.SCREEN1                                         !HDC
      ENDIF                                                             !HDC

   WEND

END.PROGRAM:

   GOSUB TERMINATE

   CALL DM.QUIT

   CHAINING.TO.PROG$ = "B50  "     ! for B553 msg
   PSBCHN.PRG = "ADX_UPGM:PSB50.286"
   %INCLUDE PSBCHNE.J86

   STOP

\********************************************************************
\***
\***    Subroutines
\***
\********************************************************************
\***
\***    SUBROUTINE      :      DISPLAY.SCREEN1
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

DISPLAY.SCREEN1:

   file.loaded% = 0

REFRESH:

   scrn1%     = 1
   max.scrn1% = INT%((p.cnt% +lpp1% -1) / lpp1%)

   ! Screen title displayed as per the store (UK/ROI)                   !GCK
   CALL DM.SHOW.SCREEN (1, SCREEN.TITLE$, 11, 11)                       !GCK
   CURRENT.SCREEN% = 1                                                  !IDC
   CALL DM.NAME (16, "page$", page$)

   ! If Limit is not found in SOFTS 60th record then hide the function  !DDM
   ! keys F3 and F9 and display BEMF 221 message. Call APPLICATION.LOG  !DDM
   ! to log the event 117 and close the program on pressing 'ESC'       !DDM
   IF NOT SOFTS.LIMIT.VAL.FOUND THEN BEGIN                              !DDM
       CALL DM.HIDE.FN.KEY(3)                                           !DDM
       CALL DM.HIDE.FN.KEY(9)                                           !DDM
       WK$ = "221 'Limit could not be fetched from SOFTS file" + \      !DDM
             " Press F3 to EXIT.' MESSAGE"                              !DDM
       ! Setting the Key value                                          !DDM
       RET.KEY% = ESC.KEY%                                              !DDM

       EVENT.NUMBER%   = 117                                            !DDM
       MESSAGE.NUMBER% = 221                                            !DDM
       VAR.STRING.1$   = STR$(SOFTS.REPORT.NUM%) + "N "                 !DDM
       VAR.STRING.2$   = "Limit could not be fetched from SOFTS file" + \DDM
                         " Press ESC to EXIT."                          !DDM

       CALL APPLICATION.LOG ( MESSAGE.NUMBER%, VAR.STRING.1$,           \DDM
                              VAR.STRING.2$, EVENT.NUMBER% )            !DDM

       GOSUB END.PROGRAM                                                !DDM
   ENDIF                                                                !DDM

    IF NOT file.loaded% THEN BEGIN

        GOSUB LOAD.PARCELS
        GOTO REFRESH
    ENDIF

   IF p.cnt% = 0 THEN BEGIN
      IF NOT bdcp.open% THEN BEGIN
         wk$ = "221 'No Boots.com Parcel file found."            \
             + " Press F3 to EXIT.' MESSAGE"
      ENDIF ELSE BEGIN
         wk$ = "221 'No Parcels found."                          \
             + " Press F3 to EXIT.' MESSAGE"
      ENDIF
      RET.KEY% = ESC.KEY%
      WHILE RET.KEY% <> F3.KEY%
         RET.KEY% = DM.INVISIBLE.INPUT(wk$)
      WEND
      GOTO END.PROGRAM
   ENDIF

   GOSUB CHANGE.SCREEN1

RETURN


\********************************************************************
\***
\***    SUBROUTINE      :      CHANGE.SCREEN1
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

CHANGE.SCREEN1:


   wk$ = "Page " + STR$(scrn1%)                                     \
       + " of "  + STR$(max.scrn1%)

   page$ = RIGHT$(STRING$(15," ") + wk$, 15)

   FOR i% = 2 TO lpp1% +1

      indx% = ((scrn1% -1) *lpp1%) + (i% - 1)

      IF indx% <= p.cnt% THEN BEGIN
         CALL DM.NAME(i%,"f"+STR$(i%)+"$",disp$(indx%))
         CALL DM.VISIBLE (STR$(i%), "TRUE")
         CALL DM.RO.FIELD(i%)
      ENDIF ELSE BEGIN
         CALL DM.VISIBLE (STR$(i%), "FALSE")
      ENDIF

   NEXT i%

   IF scrn1% < max.scrn1% THEN BEGIN
      CALL DM.SHOW.FN.KEY( 8, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 8)
   ENDIF
   IF scrn1% > 1 THEN BEGIN
      CALL DM.SHOW.FN.KEY( 7, "")
   ENDIF ELSE BEGIN
      CALL DM.HIDE.FN.KEY( 7)
   ENDIF
   CALL DM.SHOW.FN.KEY( 3, "")
   CALL DM.SHOW.FN.KEY( 1, "")
   ! blank out location by default
   CALL DM.HIDE.FN.KEY(10)                                              !IDC


   IF PRINT.KEY THEN BEGIN                                              !FMM
       ! Make F9PRINT option visible                                    !FMM
       CALL DM.SHOW.FN.KEY( 9, "")                                      !FMM
   ENDIF ELSE BEGIN                                                     !FMM
       CALL DM.HIDE.FN.KEY( 9)                                          !FMM
   ENDIF                                                                !FMM

   i% = DM.CURRENT.FIELD(0)
   indx% = ((scrn1% -1) * lpp1%) + (i% -1)
   CALL SET.F.KEYS(indx%)

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      PROCESS.SCREEN1
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

PROCESS.SCREEN1:

   SCREEN.COMPLETE = 0

   WHILE NOT SCREEN.COMPLETE

      RET.KEY% = DM.PROCESS.SCREEN (1, lpp1% +1, 0)

      CARTON.FILE.ERROR = FALSE                                         !AHCS
      PSUTQ.FILE.ERROR  = FALSE                                         !AICS

      i% = DM.CURRENT.FIELD(0)
      indx% = ((scrn1% -1) * lpp1%) + (i% -1)

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \* Escape Key pressed
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      IF RET.KEY% = ESC.KEY% THEN BEGIN
         SCREEN.COMPLETE = -1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F3 Key pressed
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF RET.KEY% = F3.KEY% THEN BEGIN
         SCREEN.COMPLETE = -1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F4 Key(Collect) pressed and status is either of the        !FMM
      \*     following:                                                 !FMM
      \*        - Uncollected - Return to W/H?                          !FMM
      \*        - Instore - Awaiting collection                         !FMM
      \*        - Instore - Found                                       !FMM
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF RET.KEY% = F4.KEY% AND            \                 !FMM
                  ((LEFT$(p.arr$(indx%),1) = "3") OR  \    Uncollected  !FMM
                   (LEFT$(p.arr$(indx%),1) = "4") OR  \       Awaiting  !FMM
                   (LEFT$(p.arr$(indx%),1) = "5")) THEN BEGIN   !Found  !FMM

         WHILE RET.KEY% <> ENTER.KEY%                               \
           AND RET.KEY% <> F3.KEY%
            RET.KEY% = DM.INVISIBLE.INPUT("221 "                    \
                             + "'Marking as Collected cannot be "   \     CSH
                             + "undone. "                           \     CSH
                             + "ENTER to confirm or F3 to cancel.' "\
                             + "MESSAGE")
         WEND

         IF RET.KEY% = ENTER.KEY% THEN BEGIN                            ! BSH
            GOSUB COLLECT.PARCEL
            GOSUB CHANGE.SCREEN1
            IF mismatch% THEN BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Change NOT implemented - recheck details.')")
            ENDIF ELSE BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Status has been updated.')")
            ENDIF
         ENDIF ELSE BEGIN                                               ! BSH
            CALL DM.FOCUS("", "MESSAGE(221,"                            \ BSH
                          + "'Change cancelled.')")                     ! BSH
         ENDIF                                                          ! BSH

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    !AGCS
      \*     F4 Key(Book In) pressed and status is either of the        !AGCS
      \*     following:                                                 !AGCS
      \*        - Late Delivery                                         !AGCS
      \*        - Expected Delivery                                     !AGCS
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    !AGCS
      ENDIF ELSE IF RET.KEY% = F4.KEY% AND            \                 !AGCS
                  ((LEFT$(p.arr$(indx%),1) = "1") OR  \       ! Late    !AGCS
                   (LEFT$(p.arr$(indx%),1) = "2")) THEN BEGIN ! Expected!AGCS
                                                                        !AGCS
         WHILE RET.KEY% <> ENTER.KEY%                               \   !AGCS
           AND RET.KEY% <> F3.KEY%                                      !AGCS
            RET.KEY% = DM.INVISIBLE.INPUT("221 "                    \   !AGCS
                             + "'Marking as Booked-in cannot be "   \   !AGCS
                             + "undone. "                           \   !AGCS
                             + "ENTER to confirm or F3 to cancel.' "\   !AGCS
                             + "MESSAGE")                               !AGCS
         WEND                                                           !AGCS
                                                                        !AGCS
         IF RET.KEY% = ENTER.KEY% THEN BEGIN                            !AGCS
            GOSUB BOOKIN.PARCEL                                         !AGCS
            GOSUB CHANGE.SCREEN1                                        !AGCS
            IF mismatch% THEN BEGIN                                     !AGCS
                                                                        !AICS
                IF CARTON.FILE.ERROR OR                                \!AICS
                    PSUTQ.FILE.ERROR THEN BEGIN                         !AICS
                                                                        !AICS
                    IF CARTON.FILE.ERROR THEN BEGIN                     !AICS
                        TEMP.REPORT.NUM% = CRTN.REPORT.NUM%             !AICS
                    ENDIF ELSE BEGIN                                    !AICS
                        TEMP.REPORT.NUM% = PSUTQ.REPORT.NUM%            !AICS
                    ENDIF                                               !AICS
                                                                        !AICS
                    CALL DM.FOCUS("", "MESSAGE(514,'"                + \!AHCS
                       RIGHT$("000" + STR$(TEMP.REPORT.NUM%), 3)     + \!AICS
                              "update - press ESCAPE to continue')")    !AHCS
                ENDIF ELSE BEGIN                                        !AHCS

                    CALL DM.FOCUS("", "MESSAGE(221,"                   \!AGCS
                       + "'Change NOT implemented - recheck details.')")!AGCS
                ENDIF                                                   !AHCS
            ENDIF ELSE BEGIN                                            !AGCS
               CALL DM.FOCUS("", "MESSAGE(221,"                        \!AGCS
                       + "'Status has been updated.')")                 !AGCS
            ENDIF                                                       !AGCS
         ENDIF ELSE BEGIN                                               !AGCS
            CALL DM.FOCUS("", "MESSAGE(221,"                           \!AGCS
                       + "'Change cancelled.')")                        !AGCS
         ENDIF                                                          !AGCS

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F5 Key(Return) pressed and status is either of the         !FMM
      \*     following:                                                 !FMM
      \*        - Uncollected - Return to W/H?                          !FMM
      \*        - Instore - Awaiting collection                         !FMM
      \*        - Instore - Found                                       !FMM
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF RET.KEY% = F5.KEY% AND            \                 !FMM
                  ((LEFT$(p.arr$(indx%),1) = "3") OR  \    Uncollected  !FMM
                   (LEFT$(p.arr$(indx%),1) = "4") OR  \       Awaiting  !FMM
                   (LEFT$(p.arr$(indx%),1) = "5")) THEN BEGIN   !Found  !FMM

         WHILE RET.KEY% <> ENTER.KEY%                               \
           AND RET.KEY% <> F3.KEY%
            RET.KEY% = DM.INVISIBLE.INPUT("221 "                    \
                             + "'Marking as Returned cannot be "    \ CSH
                             + "undone. "                           \ CSH
                             + "ENTER to confirm or F3 to cancel.' "\
                             + "MESSAGE")
         WEND

         IF RET.KEY% = ENTER.KEY% THEN BEGIN                            ! BSH
            GOSUB RETURN.PARCEL
            GOSUB CHANGE.SCREEN1
            IF mismatch% THEN BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Change NOT implemented - recheck details.')")
            ENDIF ELSE BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Status has been updated.')")
            ENDIF
         ENDIF ELSE BEGIN                                               ! BSH
            CALL DM.FOCUS("", "MESSAGE(221,"                            \ BSH
                          + "'Change cancelled.')")                     ! BSH
         ENDIF                                                          ! BSH
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F6 Key(Lost/Found) pressed and status is either of the     !FMM
      \*     following:                                                 !FMM
      \*        - Uncollected - Return to W/H?                          !FMM
      \*        - Instore - Awaiting collection                         !FMM
      \*        - Instore - Found                                       !FMM
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF RET.KEY% = F6.KEY% AND            \                 !FMM
                  ((LEFT$(p.arr$(indx%),1) = "3") OR  \    Uncollected  !FMM
                   (LEFT$(p.arr$(indx%),1) = "4") OR  \       Awaiting  !FMM
                   (LEFT$(p.arr$(indx%),1) = "5")) THEN BEGIN   !Found  !FMM

         WHILE RET.KEY% <> ENTER.KEY%                               \
           AND RET.KEY% <> F3.KEY%
            RET.KEY% = DM.INVISIBLE.INPUT("221 "                    \
                          + "'Do you wish to mark Lost? "           \
                          + "ENTER to confirm or F3 to cancel.' "   \
                          + "MESSAGE")
         WEND

         IF RET.KEY% = ENTER.KEY% THEN BEGIN                            ! BSH
            GOSUB LOST.PARCEL
            GOSUB CHANGE.SCREEN1
            IF mismatch% THEN BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Change NOT implemented - recheck details.')")
            ENDIF ELSE BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                             + "'Status has been updated.')")
            ENDIF
         ENDIF ELSE BEGIN                                               ! BSH
            CALL DM.FOCUS("", "MESSAGE(221,"                            \ BSH
                          + "'Change cancelled.')")                     ! BSH
         ENDIF                                                          ! BSH
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F6 Key(Lost/Found) pressed and status = Lost               !FMM
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF RET.KEY% = F6.KEY% AND \                            !FMM
                    LEFT$(p.arr$(indx%),1) = "8" THEN BEGIN      !Lost  !FMM

         WHILE RET.KEY% <> ENTER.KEY%                               \
           AND RET.KEY% <> F3.KEY%
            RET.KEY% = DM.INVISIBLE.INPUT("221 "                    \
                          + "'Do you wish to mark Found? "          \
                          + "ENTER to confirm or F3 to cancel.' "   \
                          + "MESSAGE")
         WEND

         IF RET.KEY% = ENTER.KEY% THEN BEGIN                            ! BSH
            GOSUB FOUND.PARCEL
            GOSUB CHANGE.SCREEN1
            IF mismatch% THEN BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                              + "'Change NOT implemented - recheck details.')")
            ENDIF ELSE BEGIN
               CALL DM.FOCUS("", "MESSAGE(221,"                         \
                                      + "'Status has been updated.')")
            ENDIF
         ENDIF ELSE BEGIN                                               ! BSH
            CALL DM.FOCUS("", "MESSAGE(221,"                            \ BSH
                          + "'Change cancelled.')")                     ! BSH
         ENDIF                                                          ! BSH

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgUp/F7/Prev Key and NOT on page 1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = F7.KEY%                            \
                   OR RET.KEY% = PGUP.KEY%                          \
                   OR RET.KEY% = PREV.KEY%)                         \
                AND (scrn1% > 1) THEN BEGIN
         scrn1% = scrn1% -1
         IF RET.KEY% = PREV.KEY% THEN BEGIN
            CALL DM.CURRENT.FIELD(lpp1% +1)
         ENDIF ELSE BEGIN
            CALL DM.CURRENT.FIELD(1 +1)
         ENDIF
         GOSUB CHANGE.SCREEN1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgDn/F8/Next Key and NOT on last page
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = F8.KEY%                            \
                   OR RET.KEY% = PGDN.KEY%                          \
                   OR RET.KEY% = NEXT.KEY%)                         \
                AND (scrn1% < max.scrn1%) THEN BEGIN
         scrn1% = scrn1% +1
         CALL DM.CURRENT.FIELD(1 +1)
         GOSUB CHANGE.SCREEN1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgDn and on last page
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = PGDN.KEY%                          \
                AND   scrn1% = max.scrn1% ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(075,'')")
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgUp and on page 1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = PGUP.KEY%                          \
                AND   scrn1% = 1          ) THEN BEGIN
         CALL DM.FOCUS ("", "MESSAGE(074,'')")

      ! If F9 key is pressed                                            !DDM
      ENDIF ELSE IF ( RET.KEY% = F9.KEY%) AND PRINT.KEY = TRUE  \       !FMM
                                                             THEN BEGIN !FMM
         GOSUB PRINT.BDCRPT                                             !DDM

      ENDIF ELSE IF (RET.KEY% = F10.KEY%) THEN BEGIN                    !HDC
      ! If F10 key is pressed                                           !HDC
          IF F10.NOT.ALLOWED THEN BEGIN                                 !IDC
          ENDIF ELSE BEGIN                                              !IDC
              GOSUB DISPLAY.SCREEN2                                     !HDC
              GOSUB PROCESS.SCREEN2                                     !HDC
              SCREEN.COMPLETE = -1                                      !HDC
              RET.KEY% = ENTER.KEY% ! set as special case to reload     !HDC
          ENDIF                                                         !IDC
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     Anything else
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE BEGIN
         ! B001 Invalid key pressed
         CALL DM.FOCUS ("", "MESSAGE(1,'')")
      ENDIF

   WEND

RETURN


!***********************************************************************!HDC
!* Code change H block marker - START                                   !HDC
!***********************************************************************!HDC
!***********************************************************************
\***
\***    SUBROUTINE      :      DISPLAY.SCREEN2
\***
!***********************************************************************

DISPLAY.SCREEN2:

   IF MANAGE.LOCATION THEN FILE.LOADED% = 0                             !LLJ

   SCRN2%     = 1                                                       !LLJ
   MAX.SCRN2% = 1                                                       !LLJ

   ! Get the number of screens required                                 !RLJ
   MAX.SCRN2% = BDCLOCON.TOTAL.RECORDS% / LOC.RECORDS.ON.PAGE%          !RLJ
   ! If the number of records are not multiple of LOC.RECORDS.ON.PAGE%  !RLJ
   IF MOD(BDCLOCON.TOTAL.RECORDS%,LOC.RECORDS.ON.PAGE%) > 0 THEN BEGIN  !RLJ
       MAX.SCRN2% = MAX.SCRN2% + 1                                      !RLJ
   ENDIF                                                                !RLJ
                                                                        !RLJ
   P.CNT2% = BDCLOCON.TOTAL.RECORDS% * LOC.FIELDS.PER.LINE% +          \!RLJ
             ( LOC.FIELDS.START% - 1 )                                  !RLJ

   CURRENT.ORDER.INDX% = DM.CURRENT.FIELD(0)
   CURRENT.ORDER.I%    = ((SCRN1% -1) * LPP1%) + (I% -1)                !LLJ

   ! Screen title displayed as per the store (UK/ROI)
   IF MANAGE.LOCATION THEN BEGIN                                        !IDC
       ! onluy show new help if managing locations                      !IDC
       CALL DM.SHOW.SCREEN (2, SCREEN2.TITLE$, 21, 21)                  !IDC
   ENDIF ELSE BEGIN                                                     !IDC
       CALL DM.SHOW.SCREEN (2, SCREEN2.TITLE$, 11, 11)                  !IDC
   ENDIF                                                                !IDC
   CURRENT.SCREEN% = 2                                                  !IDC
   CALL DM.NAME (160, "page2$", PAGE2$)                                 !LLJ
!  ! Suppress the page number as we have only one page                  !RLJ
                                                                        !RLJ
   CALL DM.NAME (145, "F10.MESSAGE$", F10.MESSAGE$)                     !RLJ
                                                                        !RLJ
   GOSUB CHANGE.SCREEN2

RETURN


!***********************************************************************
\***
\***    SUBROUTINE      :      CHANGE.SCREEN2
\***
!***********************************************************************

CHANGE.SCREEN2:


    WK$ = "Page " + STR$(SCRN2%)                                     \  !LLJ
        + " of "  + STR$(MAX.SCRN2%)                                    !LLJ

    PAGE2$ = RIGHT$(STRING$(15," ") + WK$, 15)                          !LLJ
    ! If location management then do not display F10 message            !RLJ
    IF MANAGE.LOCATION THEN BEGIN                                       !RLJ
        F10.MESSAGE$ = STRING$(74," ")                                  !RLJ
    ENDIF ELSE BEGIN                                                    !RLJ
        F10.MESSAGE$ = "Highlight the location you wish to update " +  \!RLJ
                       "and then press F10 to complete."                !RLJ
    ENDIF                                                               !RLJ

    ! I% is the field number used, with LPP2% being the maximum output  !LLJ
    ! fields per page
    ! Set the end of possible record in this screen variable
    P.FULL% = 0

    FOR I% = LOC.FIELDS.START% TO LPP2% +1                              !LLJ

        INDX% =  I% + ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))  !RLJ
                                                                        !RLJ
        IF INDX% <= P.CNT2% AND I% <= LPP2% THEN BEGIN                  !RLJ

            IF MOD(INDX%,LOC.FIELDS.PER.LINE%) = 0 THEN BEGIN

                BDCLOCON.RECORD.NUM% = (INDX%)/LOC.FIELDS.PER.LINE%     !RLJ
                IF BDCLOCON.RECORD.NUM% > BDCLOCON.TOTAL.RECORDS%      \!RLJ
                                                            THEN BEGIN  !RLJ
                    P.CNT2% = INDX%                                     !LLJ
                    ! set as full now as there are no more records
                    P.FULL% = 1
                ENDIF ELSE \
                BEGIN

                    ! Open BDCLOCON file                                !QCK
                    GOSUB OPEN.BDCLOCON                                 !QCK

                    IF READ.BDCLOCON THEN BEGIN
                        P.CNT2% = INDX%                                 !LLJ
                        ! set as full now as cannot continue read
                        ! this should be unexpected as we know the
                        ! total records (above), so will only be due
                        ! to an error
                        P.FULL% = 1
                    ENDIF ELSE BEGIN                                    !RLJ
                        !Save the description to use during validation  !RLJ
                        IF BDCLOCON.RECORD.NUM% = 1 THEN BEGIN          !RLJ
                            LOCATION1.DESC$ = BDCLOCON.LONG.NAME$       !RLJ
                        ENDIF                                           !RLJ
                    ENDIF                                               !RLJ

                    ! Closing BDCLOCON file                             !QCK
                    IF BDCLOCON.OPEN THEN BEGIN                         !QCK
                        CLOSE BDCLOCON.SESS.NUM%                        !QCK
                                                                        !QCK
                        ! Reset the flag                                !QCK
                        BDCLOCON.OPEN = FALSE                           !QCK
                    ENDIF                                               !QCK

                ENDIF
            ENDIF

        ! only store if we have more to do
            IF P.FULL% <> 1 THEN BEGIN
                LOCATION.ARRAY$(I%) = FUNC.GET.FIELD.DISPLAY2$(INDX%)   !LLJ

                CALL DM.NAME(I%,"f"+STR$(I%)+"$",LOCATION.ARRAY$(I%))   !LLJ
                CALL DM.VISIBLE (STR$(I%), "TRUE")                      !LLJ

                IF MANAGE.LOCATION THEN BEGIN
                    ! allow update except the first record              !MLJ
                ENDIF ELSE BEGIN
                    CALL DM.RO.FIELD(I%)                                !LLJ
                ENDIF
                IF P.CNT2% = INDX% THEN BEGIN                           !LLJ
                    I% = LPP2%+1                                        !LLJ
                ENDIF
            ENDIF
        ENDIF

   NEXT I%                                                              !LLJ

   I%    = DM.CURRENT.FIELD(0)                                          !LLJ
   INDX% =  I% + ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))       !RLJ

   !Checking the index value to update the default location flag.       !TAH
   !The default location is present only in the first screen.           !TAH
   IF INDX% = INITIAL.FIELD.INDEX% THEN BEGIN                           !TAH
       NOT.DEFAULT.LOCATION = FALSE                                     !TAH
   ENDIF ELSE BEGIN                                                     !TAH
       NOT.DEFAULT.LOCATION = TRUE                                      !TAH
   ENDIF                                                                !TAH
   ! Set the screen changed flag                                        !TAH
   SCREEN.CHANGED = TRUE                                                !TAH
   CALL SET.F.KEYS(INDX%)                                               !LLJ

RETURN

!***********************************************************************
\***
\***    SUBROUTINE      :      PROCESS.SCREEN2
\***
!***********************************************************************

PROCESS.SCREEN2:

   !If Location management then use default confirmation prompt         !NLJ
   IF MANAGE.LOCATION THEN BEGIN                                        !NLJ
       CONFIRM.PROMPT = TRUE                                            !NLJ
   ENDIF ELSE BEGIN                                                     !NLJ
       CONFIRM.PROMPT = FALSE                                           !NLJ
   ENDIF                                                                !NLJ
                                                                        !NLJ
   SCREEN.COMPLETE2 = 0

   WHILE NOT SCREEN.COMPLETE2

      RET.KEY% = DM.PROCESS.SCREEN (2, LPP2% +1, CONFIRM.PROMPT)        !NLJ

      I% = DM.CURRENT.FIELD(0)                                          !LLJ
      INDX% = I% + ((SCRN2% - 1) * (LPP2% - LOC.FIELDS.START% + 1))     !RLJ

      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     F3 Key pressed
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      IF RET.KEY% = F3.KEY% THEN BEGIN
         SCREEN.COMPLETE2 = -1
          IF MANAGE.LOCATION THEN BEGIN                                 !JDC
          ENDIF ELSE BEGIN                                              !JDC
              PROMPT.MESSAGE$ = "Exit without saving Location? " +  \   !JDC
                                "F3 to Exit or ENTER Continue."         !JDC
              RET.KEY% = FUNC.PROMPT.FOR.RESPONSE%(PROMPT.MESSAGE$)     !JDC
              IF RET.KEY% = ENTER.KEY% THEN BEGIN                       !JDC
                  CALL DM.FOCUS("", "MESSAGE(221,"  \                   !JDC
                              + "'Change cancelled.')")                 !JDC
                  SCREEN.COMPLETE2 = 0                                  !JDC
              ENDIF                                                     !JDC
           ENDIF
      ENDIF ELSE \
      IF RET.KEY% = F10.KEY% \
      AND NOT MANAGE.LOCATION \
      THEN BEGIN
          IF F10.NOT.ALLOWED THEN BEGIN                                 !IDC
             PROMPT.MESSAGE$ = "Location cannot be used as "          \ !JDC
                             + "currently INACTIVE. F3 to continue."    !JDC
             WHILE RET.KEY% <> F3.KEY%                                  !IDC
                  RET.KEY% = FUNC.PROMPT.FOR.RESPONSE%(PROMPT.MESSAGE$) !JDC
             WEND                                                       !IDC
          ENDIF ELSE IF ((INDX%)/LOC.FIELDS.PER.LINE% = 1) THEN BEGIN   !RLJ
             CALL DM.FOCUS ("", "MESSAGE(336,'')")                      !PLJ
          ENDIF ELSE BEGIN                                              !IDC
             GOSUB UPDATE.LOCATION                                      !IDC
             SCREEN.COMPLETE2 = -1                                      !IDC
          ENDIF
      ENDIF ELSE \
      IF ((RET.KEY% = ENTER.KEY% OR ACTIVE.TO.BE.SAVED) AND            \!OLJ
            MANAGE.LOCATION) THEN BEGIN                                 !OLJ
            GOSUB SAVE.BDCLOCON.DATA                                    !RLJ
            ! Moved the code to SAVE.BDCLOCON.DATA: subroutine          !RLJ
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgUp/F7/Prev Key and NOT on page 1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = F7.KEY%                            \
                   OR RET.KEY% = PGUP.KEY%                          \
                   OR RET.KEY% = PREV.KEY%)                         \
                AND (SCRN2% > 1) THEN BEGIN                             !LLJ
         ! If there are any changes to be saved                         !RLJ
         IF ANY.CHANGES.DONE THEN BEGIN                                 !RLJ
            GOSUB SAVE.BDCLOCON.DATA                                    !RLJ
         ENDIF                                                          !RLJ
         SCRN2% = SCRN2% -1                                             !LLJ
         IF RET.KEY% = PREV.KEY% THEN BEGIN
            CALL DM.CURRENT.FIELD(LPP2% +1)                             !LLJ
         ENDIF ELSE BEGIN
            CALL DM.CURRENT.FIELD(5)                                    !RLJ
         ENDIF

         GOSUB CHANGE.SCREEN2                                           !RLJ
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgDn/F8/Next Key and NOT on last page
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = F8.KEY%                            \
                   OR RET.KEY% = PGDN.KEY%                          \
                   OR RET.KEY% = NEXT.KEY%)                         \
                AND (SCRN2% < MAX.SCRN2%) THEN BEGIN                    !LLJ
         ! If there are any changes to be saved                         !RLJ
         IF ANY.CHANGES.DONE THEN BEGIN                                 !RLJ
            GOSUB SAVE.BDCLOCON.DATA                                    !RLJ
         ENDIF                                                          !RLJ

         SCRN2% = SCRN2% +1                                             !LLJ
         CALL DM.CURRENT.FIELD(5)                                       !RLJ
         GOSUB CHANGE.SCREEN2                                           !RLJ
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgDn and on last page
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = PGDN.KEY%                          \
                AND   SCRN2% = MAX.SCRN2% ) THEN BEGIN                  !LLJ
         CALL DM.FOCUS ("", "MESSAGE(075,'')")
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      \*     PgUp and on page 1
      \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
      ENDIF ELSE IF ( RET.KEY% = PGUP.KEY%                          \
                AND   SCRN2% = 1          ) THEN BEGIN                  !LLJ
         CALL DM.FOCUS ("", "MESSAGE(074,'')")

      ENDIF ELSE \              !                                       !IDC
      IF RET.KEY% = F6.KEY% \   ! if Activating or Deactivating         !IDC
      AND MANAGE.LOCATION \     !                                       !IDC
      THEN BEGIN

          ! If F6 key pressed on any other location other than 001 and
          ! the change is accepted (keyed ENTER on confirmation prompt)
          IF INDX% <> INITIAL.FIELD.INDEX% AND    \                     !ADCK
             CHANGE.ACCEPTED               THEN BEGIN                   !ADCK
              BDCLOCON.RECORD.NUM% = (INDX%)/LOC.FIELDS.PER.LINE%       !RLJ

              ! Open BDCLOCON file                                      !QCK
              GOSUB OPEN.BDCLOCON                                       !QCK

              CALL READ.BDCLOCON
              LOC.STATUS.CHANGED = FALSE                                !LLJ
              IF BDCLOCON.STATUS$ = "A" THEN BEGIN
                  !Deactivate only if there are no parcels at this      !LLJ
                  !location                                             !VCK
                  IF BDCLOCON.PARCEL.COUNT% <= 0 THEN BEGIN             !LLJ
                      BDCLOCON.STATUS$    = "I"    ! Inactive           !LLJ
                      !Deactivating an active location makes the        !AFCS
                      !description null                                 !AFCS
                      LOCATION.ARRAY$(CURRENT.INDEX%) = " "             !AFCS
                      LOC.STATUS.CHANGED = TRUE                         !LLJ
                      !Reset the inactive checking flag                 !SAH
                      INACTIVE.CHECK%    = 0                            !SAH
                  ENDIF ELSE BEGIN                                      !LLJ
                      !Setting below flag to false as no status change  !AFCS
                      LOC.STATUS.CHANGED = FALSE                        !AFCS
                      !Display error message                            !LLJ
                      CALL DM.FOCUS ("", "MESSAGE(466,'')")             !ABDC
                  ENDIF                                                 !LLJ
              ENDIF ELSE BEGIN
                  BDCLOCON.STATUS$   = "A"        ! Active              !LLJ
                  LOC.STATUS.CHANGED = TRUE                             !LLJ
              ENDIF
              !Update BDCLOCON only if the status is changed            !LLJ
              IF LOC.STATUS.CHANGED THEN BEGIN                          !LLJ
                  LOCATION.ARRAY$(I% - 2) = BDCLOCON.STATUS$            !RLJ
                  !Location name is updated as location status changed  !AFCS
                  BDCLOCON.LONG.NAME$ = LOCATION.ARRAY$(CURRENT.INDEX%) !AFCS
                  CALL WRITE.BDCLOCON
              ENDIF                                                     !LLJ

              ! Closing BDCLOCON file                                   !QCK
              IF BDCLOCON.OPEN THEN BEGIN                               !QCK
                  CLOSE BDCLOCON.SESS.NUM%                              !QCK
                                                                        !QCK
                  ! Reset the flag                                      !QCK
                  BDCLOCON.OPEN = FALSE                                 !QCK
              ENDIF                                                     !QCK

          ENDIF ELSE IF INDX% = INITIAL.FIELD.INDEX% THEN BEGIN         !ADCK
              ! If F6 key was pressed on location 001- booked in at till!VCK
              ! BEMF message 464 is displayed                           !VCK
              CALL DM.FOCUS ("", "MESSAGE(464,'')")                     !VCK
          ENDIF                                                         !VCK

          ! Reset the flag                                              !ADCK
          CHANGE.ACCEPTED = TRUE                                        !ADCK

      ENDIF ELSE BEGIN
          ! B001 Invalid key pressed
          CALL DM.FOCUS ("", "MESSAGE(1,'')")
      ENDIF

      ! Reset the variable                                              !ADCK
      KEY.ACTION% = 0                                                   !ADCK

   WEND

RETURN

!***********************************************************************

!***********************************************************************
UPDATE.LOCATION:

    IF MANAGE.LOCATION THEN BEGIN

        ! update all records since we don't know whats changing
        LOCATION.NEW% = FUNC.UPDATE.ALL.LOCATION.RECORDS%               !IDC

        IF LOCATION.NEW% > 0 THEN BEGIN
            ! prompt screen if error on updating
            CALL DM.HIDE.FN.KEY(10)

            EVENT.NUMBER%   = 106
            MESSAGE.NUMBER% = 221
            VAR.STRING.1$   = STR$(LOCATION.NEW%)+ERR
            VAR.STRING.2$   = \
             "Unable to complete update, fail at location " + \
              STR$(LOCATION.NEW%) + ". " + \
              "Press ESC."

           CALL APPLICATION.LOG ( MESSAGE.NUMBER%, VAR.STRING.1$,           \
                                  VAR.STRING.2$, EVENT.NUMBER% )

        ENDIF
    ENDIF ELSE BEGIN
        !IF THIS IS THE CHANGE OPTION FROM SCREEN 1
        SCREEN.COMPLETE2 = -1
        LOCATION.NEW% = (INDX%)/LOC.FIELDS.PER.LINE%                    !RLJ
        GOSUB LOCATION.CHANGE
    ENDIF

RETURN

!***********************************************************************
\***
\***    SUBROUTINE      :      LOCATION.CHANGE
\***
!***********************************************************************

LOCATION.CHANGE:

    ! Close BDCP file if it is opened and reopen for keyed access
    ! (with update)

    IF BDCP.OPEN% THEN BEGIN
        CLOSE BDCP.SESS.NUM%
        BDCP.OPEN% = 0
    ENDIF

    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%

    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL
    BDCP.OPEN% = 1

    INDX% = CURRENT.ORDER.I%     ! not sure why these swap but may need !LLJ
    I%    = CURRENT.ORDER.INDX%  ! to have screen 2 own versions        !LLJ

    GOSUB READ.RECORD.LOCK

    IF NOT MISMATCH% THEN BEGIN                                         !LLJ

        ! Save old and new locations for use later (3-byte ASCII)       !AICS
        PSUTQ.PREV.LOCON$ = RIGHT$("000" + STR$(BDCP.LOC.CURRENT%), 3)  !AICS
        PSUTQ.CURR.LOCON$ = RIGHT$("000" + STR$(LOCATION.NEW%), 3)      !AICS

        !Avoid spaces incase it is a new record having filler spaces    !LLJ
        IF CHR$(BDCP.LOC.CURRENT%) = "  " THEN BEGIN                    !LLJ
            BDCP.LOC.CURRENT% = 0                                       !LLJ
        ENDIF                                                           !LLJ
!AICS        !Decrease the count by one for old locaiton                     !LLJ
!AICS        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
!AICS        PARCEL.COUNT%        = -1        !Decrement by 1                !LLJ
!AICS        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ

        BDCP.LOC.CURRENT% = LOCATION.NEW%

!AICS        !Increase the count by one for new locaiton                     !LLJ
!AICS        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
!AICS        PARCEL.COUNT%        = 1         !Increment by 1                !LLJ
!AICS        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ

    ENDIF
    ! force reset as can be updated outside of this screen
    BDCP.LOC.STATUS% = 0                                                !IDC

    GOSUB WRITE.RECORD.UNLOCK

    ! Closing BDCP file

    IF BDCP.OPEN% THEN BEGIN
        CLOSE BDCP.SESS.NUM%
        BDCP.OPEN% = 0
    ENDIF

    PSUTQ.TRANS.TYPE$ = "1"                                             !AICS
    PSUTQ.CURROP$     = "M"                                             !AICS
    GOSUB ADD.RECORD.TO.END.OF.PSUTQ.FILE                               !AICS

    ! Reset location                                                    !LLJ
    LOCATION.NEW% = 0                                                   !LLJ

RETURN
!***********************************************************************!HDC
!* Code change H block marker - END                                     !HDC
!***********************************************************************!HDC

\***********************************************************************!RLJ
\* SAVE.BDCLOCON.DATA:                                                  !RLJ
\* To update the changes on the screen to BDCLOCON file                 !RLJ
\*                                                                      !RLJ
\***********************************************************************!RLJ
SAVE.BDCLOCON.DATA:                                                     !RLJ

    PROMPT.MESSAGE$ = "Update all changes on screen? "  + \             !RLJ
                      "ENTER to confirm or F3 to cancel."               !RLJ
    RET.KEY% = FUNC.PROMPT.FOR.RESPONSE%(PROMPT.MESSAGE$)               !RLJ
                                                                        !RLJ
    IF RET.KEY% = ENTER.KEY% THEN BEGIN                                 !RLJ
        GOSUB UPDATE.LOCATION                                           !RLJ
        ! Save status also                                              !RLJ
        IF ACTIVE.TO.BE.SAVED THEN BEGIN                                !RLJ
            ! Get the actual index                                      !UCK
            CURRENT.INDEX% =  I%           +              \             !UCK
                             ((SCRN2% - 1) *              \             !UCK
                              (LPP2% - LOC.FIELDS.START% + 1))          !UCK
            ! Modified to get correct record number                     !UCK
            BDCLOCON.RECORD.NUM% = (CURRENT.INDEX%)/LOC.FIELDS.PER.LINE%!UCK
                                                                        !RLJ
            ! Open BDCLOCON file                                        !RLJ
            GOSUB OPEN.BDCLOCON                                         !RLJ
                                                                        !RLJ
            CALL READ.BDCLOCON                                          !RLJ
            BDCLOCON.STATUS$ = "A"                                      !RLJ
            CALL WRITE.BDCLOCON                                         !RLJ
                                                                        !RLJ
            ! Closing BDCLOCON file                                     !RLJ
            IF BDCLOCON.OPEN THEN BEGIN                                 !RLJ
                CLOSE BDCLOCON.SESS.NUM%                                !RLJ
                                                                        !RLJ
                ! Reset the flag                                        !RLJ
                BDCLOCON.OPEN = FALSE                                   !RLJ
            ENDIF                                                       !RLJ
                                                                        !RLJ
            ! Updating the status in array using correct parameter      !UCK
            LOCATION.ARRAY$(I% - 2) = BDCLOCON.STATUS$                  !UCK
        ENDIF                                                           !RLJ
    ! If F3 pressed                                                     !ADCK
    ENDIF ELSE BEGIN                                                    !RLJ
        ! If the main key pressed was F6, show F6 function text as      !ADCK
        ! 'ACTIV' since the status is now 'I' as F3 is pressed.         !ADCK
        IF KEY.ACTION% = 1 THEN BEGIN                                   !ADCK
            CALL DM.SHOW.FN.KEY(6, ACTIVATE.FN.TEXT$)                   !ADCK
        ENDIF                                                           !ADCK
    ENDIF                                                               !ADCK
    ACTIVE.TO.BE.SAVED = FALSE                                          !RLJ
    ANY.CHANGES.DONE   = FALSE                                          !RLJ
    !Reset the Flag changed                                             !RLJ
    RC.INT1% = DM.CHANGED.FLAG(0)                                       !RLJ
    ! Clear status message                                              !RLJ
    CALL DM.STATUS(BLANK.MSG$)                                          !RLJ
RETURN                                                                  !RLJ
                                                                        !RLJ


\***********************************************************************!AHCS
\***                                                                    !AHCS
\***    SUBROUTINE      :      ROLLBACK.PARCEL.RECORD                   !AHCS
\***                                                                    !AHCS
\***********************************************************************!AHCS
                                                                        !AHCS
ROLLBACK.PARCEL.RECORD:                                                 !AHCS
                                                                        !AHCS
    ! Close BDCP file if it is opened and reopen for keyed access       !AHCS
    ! (with update)                                                     !AHCS
                                                                        !AHCS
    IF BDCP.OPEN% THEN BEGIN                                            !AHCS
        CLOSE BDCP.SESS.NUM%                                            !AHCS
        BDCP.OPEN% = FALSE                                              !AHCS
    ENDIF                                                               !AHCS
                                                                        !AHCS
    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !AHCS
                                                                        !AHCS
    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !AHCS
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !AHCS
    BDCP.OPEN% = TRUE                                                   !AHCS
                                                                        !AHCS
    GOSUB READ.RECORD.LOCK                                              !AHCS
                                                                        !AHCS
    ! Reinstate record details as we're rolling back the BDCP change    !AHCS
    p.arr$(indx%)      = SAVED.ARRAY$                                   !AHCS
    BDCP.STATUS$       = SAVED.BDCP.STATUS$                             !AHCS
    BDCP.DEL.DATETIME$ = SAVED.BDCP.DEL.DATETIME$                       !AHCS
    BDCP.DEL.EXPORTED$ = SAVED.BDCP.DEL.EXPORTED$                       !AHCS
                                                                        !AHCS
    GOSUB WRITE.RECORD.UNLOCK                                           !AHCS
                                                                        !AHCS
    ! Close BDCP file                                                   !AHCS
    IF BDCP.OPEN% THEN BEGIN                                            !AHCS
        CLOSE BDCP.SESS.NUM%                                            !AHCS
        BDCP.OPEN% = FALSE                                              !AHCS
    ENDIF                                                               !AHCS
                                                                        !AHCS
    MISMATCH%         = TRUE                                            !AHCS
    CARTON.FILE.ERROR = TRUE                                            !AHCS
                                                                        !AHCS
RETURN                                                                  !AHCS


\***********************************************************************!AGCS
\***                                                                    !AGCS
\***        SUBROUTINE      :      UPDATE.CARTON.FILE                   !AGCS
\***                                                                    !AGCS
\***    Update the carton file (if necessary)                           !AGCS
\***                                                                    !AGCS
\***********************************************************************!AGCS
                                                                        !AGCS
UPDATE.CARTON.FILE:                                                     !AGCS
                                                                        !AHCS
    CARTON.FILE.ERROR = FALSE                                           !AHCS
                                                                        !AGCS
    IF CRTN.OPEN THEN BEGIN                                             !AGCS
        CLOSE CRTN.SESS.NUM%                                            !AGCS
        CRTN.OPEN = FALSE                                               !AGCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
    CURRENT.REPORT.NUM% = CRTN.REPORT.NUM%                              !AGCS
                                                                        !AGCS
    ! Read Carton record                                                !AGCS
    IF END # CRTN.SESS.NUM% THEN DO.ROLLBACK                            !AGCS
    OPEN CRTN.FILE.NAME$ KEYED RECL CRTN.RECL% AS CRTN.SESS.NUM% NODEL  !AGCS
    CRTN.OPEN = TRUE                                                    !AGCS
                                                                        !AGCS
    ! Build Carton key                                                  !AGCS
    CRTN.SUPPLIER$ = BDCP.SUPPLIER$                                     !AGCS
    CRTN.NO$       = BDCP.CARTON$                                       !AGCS
    CRTN.CHAIN%    = 0                                                  !AGCS
                                                                        !AGCS
    RC% = READ.CRTN                                                     !AGCS
                                                                        !AGCS
    ! If Carton record is found in Carton file                          !AGCS
    IF RC% = 0 THEN BEGIN                                               !AGCS
                                                                        !AGCS
        ! Only update if If Carton Unbooked                             !AGCS
        IF CRTN.STATUS$ = "U" THEN BEGIN                                !AGCS
                                                                        !AGCS
            ! Update status to 'Booked Normally'                        !AGCS
            CRTN.STATUS$ = "N"                                          !AGCS
                                                                        !AGCS
            ! Update CARTON file                                        !AGCS
            RC% = WRITE.CRTN                                            !AGCS
                                                                        !AGCS
            ! If update fails                                           !AGCS
            IF RC% <> 0 THEN BEGIN                                      !AGCS
                GOSUB ROLLBACK.PARCEL.RECORD                            !AHCS
            ENDIF                                                       !AGCS
        ENDIF                                                           !AGCS
                                                                        !AGCS
    ENDIF ELSE BEGIN                                                    !AGCS
        GOSUB ROLLBACK.PARCEL.RECORD                                    !AHCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
    IF CRTN.OPEN THEN BEGIN                                             !AGCS
        CLOSE CRTN.SESS.NUM%                                            !AGCS
        CRTN.OPEN = FALSE                                               !AGCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
RETURN                                                                  !AGCS
                                                                        !AGCS
                                                                        !AGCS
DO.ROLLBACK:                                                            !AGCS
!----------                                                             !AGCS
    GOSUB ROLLBACK.PARCEL.RECORD                                        !AGCS
                                                                        !AGCS
RETURN                                                                  !AGCS




\********************************************************************   !AICS
\***                                                                    !AICS
\***    SUBROUTINE      :      ADD.RECORD.TO.END.OF.PSUTQ.FILE          !AICS
\***                                                                    !AICS
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    !AICS
                                                                        !AICS
ADD.RECORD.TO.END.OF.PSUTQ.FILE:                                        !AICS
                                                                        !AICS
    ! Open PSUTQ and write a Type 1 Bookin Record to end of file        !AICS
    IF PSUTQ.OPEN THEN BEGIN                                            !AICS
        CLOSE PSUTQ.SESS.NUM%                                           !AICS
        PSUTQ.OPEN = FALSE                                              !AICS
    ENDIF                                                               !AICS
                                                                        !AICS
    CURRENT.REPORT.NUM% = PSUTQ.REPORT.NUM%                             !AICS
                                                                        !AICS
    IF END # PSUTQ.SESS.NUM% THEN OPEN.ERROR                            !AICS
    OPEN PSUTQ.FILE.NAME$ AS PSUTQ.SESS.NUM% NODEL APPEND               !AICS
    PSUTQ.OPEN = TRUE                                                   !AICS
                                                                        !AKKK
!    IF PSUTQ.TRANS.TYPE$ = "1" THEN BEGIN   ! Book In & Put Away       !AKKK
!        PSUTQ.RECORD$ =           \                                    !AKKK
!              PACK$("01")       + \ ! TRANSACTION TYPE                 !AKKK
!              ";"               + \ ! FIELD DELIMITER                  !AKKK
!              PSUTQ.CURROP$     + \ ! CURRENT OPERATION                !AKKK
!              BDCP.SUPPLIER$    + \ ! SUPPLIER NUMBER (packed)         !AKKK
!              BDCP.ORDER$       + \ ! ORDER NUMBER    (packed)         !AKKK
!              BDCP.CARTON$      + \ ! PARCEL NUMBER   (packed)         !AKKK
!              PSUTQ.CURR.LOCON$ + \ ! CURRENT LOCATION NUMBER          !AKKK
!              PSUTQ.PREV.LOCON$ + \ ! PREVIOUS LOCATION NUMBER         !AKKK
!              PACK$(DATE$)      + \ ! BOOK IN SCAN DATE YYMMDD         !AKKK
!              PACK$(TIME$)          ! BOOK IN SCAN TIME HHMMSS         !AKKK

                                                                        
!    ENDIF ELSE BEGIN  ! PSUTQ.TRANS.TYPE$ = 5  Parcel Returns          !AKKK
!        PSUTQ.RECORD$ =         \                                      !AKKK
!              PACK$("05")    + \ ! TRANSACTION TYPE                    !AKKK
!              ";"            + \ ! FIELD DELIMITER                     !AKKK
!              BDCP.SUPPLIER$ + \ ! SUPPLIER NUMBER (packed)            !AKKK
!              BDCP.ORDER$    + \ ! ORDER NUMBER    (packed)            !AKKK
!              BDCP.CARTON$   + \ ! PARCEL NUMBER   (packed)            !AKKK
!             "000"           + \ ! RETURN LIST ID                      !AKKK
!             "000"           + \ ! RETURN LIST SEQUENCE                !AKKK
!                               \ ! LOCATION NUMBER                     !AKKK
!             RIGHT$("000" + STR$(BDCP.LOC.CURRENT%), 3) + \            !AKKK
!             PACK$(DATE$)    + \ ! BOOK IN SCAN DATE YYMMDD            !AKKK
!             PACK$(TIME$)        ! BOOK IN SCAN TIME HHMMSS            !AKKK

!    ENDIF                                                              !AKKK

    IF PSUTQ.TRANS.TYPE$ = "1" THEN BEGIN   ! Book In & Put Away        !AKKK
        PSUTQ.RECORD$ =                    \                            !AKKK
              "01"                       + \ ! TRANSACTION TYPE         !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              PSUTQ.CURROP$              + \ ! CURRENT OPERATION        !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.SUPPLIER$)    + \ ! SUPPLIER NUMBER          !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.ORDER$)       + \ ! ORDER NUMBER             !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.CARTON$)      + \ ! PARCEL NUMBER            !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              PSUTQ.CURR.LOCON$          + \ ! CURRENT LOCATION NUMBER  !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              PSUTQ.PREV.LOCON$          + \ ! PREVIOUS LOCATION NUMBER !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              DATE$                      + \ ! BOOK IN SCAN DATE YYMMDD !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              TIME$                          ! BOOK IN SCAN TIME HHMMSS !AKKK

                                                                        
    ENDIF ELSE BEGIN  ! PSUTQ.TRANS.TYPE$ = 5  Parcel Returns           !AKKK
        PSUTQ.RECORD$ =         \                                       !AKKK
              "05"                       + \ ! TRANSACTION TYPE         !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.SUPPLIER$)    + \ ! SUPPLIER NUMBER          !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.ORDER$)       + \ ! ORDER NUMBER             !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              UNPACK$(BDCP.CARTON$)      + \ ! PARCEL NUMBER            !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              "000"                      + \ ! RETURN LIST ID           !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK   
              "000"                      + \ ! RETURN LIST SEQUENCE     !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
                                           \ ! LOCATION NUMBER          !AKKK
              RIGHT$("000" + STR$(BDCP.LOC.CURRENT%), 3) + \            !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              DATE$                      + \ ! BOOK IN SCAN DATE YYMMDD !AKKK
              COMMA$                     + \ ! FIELD DELIMITER          !AKKK
              TIME$                          ! BOOK IN SCAN TIME HHMMSS !AKKK

    ENDIF                                                               !AKKK
                                                                        !AICS
    RC% = WRITE.PSUTQ                                                   !AICS
                                                                        !AICS
    IF PSUTQ.OPEN THEN BEGIN                                            !AICS
        CLOSE PSUTQ.SESS.NUM%                                           !AICS
        PSUTQ.OPEN = FALSE                                              !AICS
    ENDIF                                                               !AICS
                                                                        !AICS
    IF RC% <> 0 THEN BEGIN                                              !AICS
        ! Write failed - no need to read PSUCF to see if PSD86 cut-off  !AICS
        ! in progress since we don't need to display specic errors to   !AICS
        ! the user.                                                     !AICS
        PSUTQ.FILE.ERROR = TRUE                                         !AICS
    ENDIF                                                               !AICS
                                                                        !AICS
RETURN                                                                  !AICS




\********************************************************************   !AGCS
\***                                                                    !AGCS
\***    SUBROUTINE      :      BOOKIN.PARCEL                            !AGCS
\***                                                                    !AGCS
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    !AGCS
                                                                        !AGCS
BOOKIN.PARCEL:                                                          !AGCS
                                                                        !AGCS
    ! Close BDCP file if it is opened and reopen for keyed access       !AGCS
    ! (with update)                                                     !AGCS
                                                                        !AGCS
    IF BDCP.OPEN% THEN BEGIN                                            !AGCS
        CLOSE BDCP.SESS.NUM%                                            !AGCS
        BDCP.OPEN% = FALSE                                              !AGCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !AGCS
                                                                        !AGCS
    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !AGCS
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !AGCS
    BDCP.OPEN% = TRUE                                                   !AGCS
                                                                        !AGCS
    GOSUB READ.RECORD.LOCK                                              !AGCS
                                                                        !AGCS
    ! Save record details in case we need to rollback the change        !AHCS
    SAVED.ARRAY$             = p.arr$(indx%)                            !AHCS
    SAVED.BDCP.STATUS$       = BDCP.STATUS$                             !AHCS
    SAVED.BDCP.DEL.DATETIME$ = BDCP.DEL.DATETIME$                       !AHCS
    SAVED.BDCP.DEL.EXPORTED$ = BDCP.DEL.EXPORTED$                       !AHCS
                                                                        !AGCS
    IF NOT mismatch% THEN BEGIN                                         !AGCS
        BDCP.STATUS$       = "R" ! In Store Ready for collection        !AGCS
        BDCP.DEL.DATETIME$ = PACK$(DATE$ + TIME$)                       !AGCS
        BDCP.DEL.EXPORTED$ = "Y"                                        !AJCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
    ! Set date parcel received in store to today. This is achieved by   !AGCS
    ! overwriting the current blank date that is held in bytes 11 - 13  !AGCS
    ! in current array index.                                           !AGCS
    P.ARR$(INDX%) = LEFT$(P.ARR$(INDX%), 10)                         + \!AGCS
                    PACK$(DATE$)                                     + \!AGCS
                    RIGHT$(P.ARR$(INDX%), LEN(P.ARR$(INDX%)) - 13)      !AGCS
                                                                        !AGCS
    GOSUB UPDATE.ARRAY                                                  !AGCS
                                                                        !AGCS
    GOSUB WRITE.RECORD.UNLOCK                                           !AGCS
                                                                        !AGCS
    ! Closing BDCP file                                                 !AGCS
                                                                        !AGCS
    IF BDCP.OPEN% THEN BEGIN                                            !AGCS
        CLOSE BDCP.SESS.NUM%                                            !AGCS
        BDCP.OPEN% = FALSE                                              !AGCS
    ENDIF                                                               !AGCS
                                                                        !AGCS
    GOSUB UPDATE.CARTON.FILE                                            !AGCS

    IF NOT CARTON.FILE.ERROR THEN BEGIN                                 !AICS
        ! Update to BDCP and CARTON file successful ie. No rollback.    !AICS
        ! Add Type 1 record to end of Parcel Status Update Transaction  !AICS
        ! Queue file.                                                   !AICS
        PSUTQ.TRANS.TYPE$ = "1"                                         !AICS
        PSUTQ.CURROP$     = "B"                                         !AICS
        PSUTQ.CURR.LOCON$ = "000"                                       !AICS
        PSUTQ.PREV.LOCON$ = "000"                                       !AICS
        GOSUB ADD.RECORD.TO.END.OF.PSUTQ.FILE                           !AICS
    ENDIF                                                               !AICS
                                                                        !AGCS
RETURN                                                                  !AGCS



\********************************************************************
\***
\***    SUBROUTINE      :      COLLECT.PARCEL
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

COLLECT.PARCEL:

    ! Close BDCP file if it is opened and reopen for keyed access       !ECK
    ! (with update)                                                     !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !ECK

    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !ECK
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !ECK
    BDCP.OPEN% = -1                                                     !ECK

    GOSUB READ.RECORD.LOCK

    IF NOT mismatch% THEN BEGIN
        BDCP.STATUS$       = "C"
        BDCP.COL.DATETIME$ = PACK$(DATE$ + TIME$)
        BDCP.COL.RC%       = 1
        BDCP.COL.EXPORTED$ = "N"

        !Decrease the count by one for old locaiton                     !LLJ
        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
        PARCEL.COUNT%        = -1        !Decrement by 1                !LLJ
        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ

    ENDIF

    GOSUB UPDATE.ARRAY

    GOSUB WRITE.RECORD.UNLOCK

    ! Closing BDCP file                                                 !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      RETURN.PARCEL
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

RETURN.PARCEL:

!AICS    ! Close BDCP file if it is opened and reopen for keyed access       !ECK
!AICS    ! (with update)                                                     !ECK
!AICS
!AICS    IF BDCP.OPEN% THEN BEGIN                                            !ECK
!AICS        CLOSE BDCP.SESS.NUM%                                            !ECK
!AICS        BDCP.OPEN% = 0                                                  !ECK
!AICS    ENDIF                                                               !ECK
!AICS
!AICS    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !ECK
!AICS
!AICS    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !ECK
!AICS    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !ECK
!AICS    BDCP.OPEN% = 1                                                      !ECK
!AICS
!AICS    GOSUB READ.RECORD.LOCK
!AICS
!AICS    IF NOT mismatch% THEN BEGIN
!AICS        BDCP.STATUS$       = "U"
!AICS        BDCP.RET.DATETIME$ = PACK$(DATE$ + TIME$)
!AICS        BDCP.RET.EXPORTED$ = "N"
!AICS
!AICS        !Decrease the count by one for old locaiton                     !LLJ
!AICS        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
!AICS        PARCEL.COUNT%        = -1        !Decrement by 1                !LLJ
!AICS        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ
!AICS
!AICS    ENDIF
!AICS
!AICS    GOSUB UPDATE.ARRAY
!AICS
!AICS    GOSUB WRITE.RECORD.UNLOCK
!AICS
!AICS    ! Closing BDCP file                                                 !ECK
!AICS
!AICS    IF BDCP.OPEN% THEN BEGIN                                            !ECK
!AICS        CLOSE BDCP.SESS.NUM%                                            !ECK
!AICS        BDCP.OPEN% = 0                                                  !ECK
!AICS    ENDIF                                                               !ECK

   !********************************************************************!AJCS
   ! Array layout                                                       !AJCS
   ! 12222233334445555556667777                                         !AJCS
   ! 1 = Status sort Order (ASCII)   1, 1                               !AJCS
   ! 2 = Order Number(PD)            2, 5                               !AJCS
   ! 3 = Parcel number (PD)          7, 4                               !AJCS
   ! 4 = Date received (PD)         11, 3                               !AJCS
   ! 5 = Date for Status Msg (PD)   14, 3                               !AJCS
   !     Time for Status Msg (PD)   17, 3                               !AJCS
   ! 6 = Supplier Number (PD)       20, 3                               !AJCS
   ! 7 = 2x INT2 - Current Location + Location Status                   !AJCS
   !********************************************************************!AJCS
    BDCP.RET.DATETIME$ = PACK$(DATE$ + TIME$)                           !AJCS
    BDCP.SUPPLIER$     = MID$(p.arr$(indx%),20,3)                       !AJCS
    BDCP.ORDER$        = MID$(p.arr$(indx%), 2,5)                       !AJCS
    BDCP.CARTON$       = MID$(p.arr$(indx%), 7,4)                       !AJCS

    PSUTQ.TRANS.TYPE$ = "5"                                             !AICS
    GOSUB ADD.RECORD.TO.END.OF.PSUTQ.FILE                               !AICS

    BDCP.STATUS$ = "U"                                                  !AJCS
    GOSUB UPDATE.ARRAY                                                  !AJCS
RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      LOST.PARCEL
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

LOST.PARCEL:

    ! Close BDCP file if it is opened and reopen for keyed access       !ECK
    ! (with update)                                                     !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !ECK

    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !ECK
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !ECK
    BDCP.OPEN% = -1                                                     !ECK

    GOSUB READ.RECORD.LOCK

    IF NOT mismatch% THEN BEGIN
        BDCP.STATUS$       = "L"
        BDCP.LST.DATETIME$ = PACK$(DATE$ + TIME$)
        BDCP.LST.EXPORTED$ = "N"

        !Decrease the count by one for old locaiton                     !LLJ
        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
        PARCEL.COUNT%        = -1        !Decrement by 1                !LLJ
        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ

    ENDIF

    GOSUB UPDATE.ARRAY

    GOSUB WRITE.RECORD.UNLOCK

    ! Closing BDCP file                                                 !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      FOUND.PARCEL
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

FOUND.PARCEL:

    ! Close BDCP file if it is opened and reopen for keyed access       !ECK
    ! (with update)                                                     !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                              !ECK

    IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                              !ECK
    OPEN BDCP.FILE.NAME$ KEYED RECL BDCP.RECL% AS BDCP.SESS.NUM% NODEL  !ECK
    BDCP.OPEN% = -1                                                     !ECK

    GOSUB READ.RECORD.LOCK

    IF NOT mismatch% THEN BEGIN
        BDCP.STATUS$       = "R"
        BDCP.FND.DATETIME$ = PACK$(DATE$ + TIME$)
        BDCP.FND.EXPORTED$ = "N"

        !Increase the count by one for old locaiton                     !LLJ
        BDCLOCON.RECORD.NUM% = BDCP.LOC.CURRENT%                        !LLJ
        PARCEL.COUNT%        = 1         !Increment by 1                !LLJ
        GOSUB UPDATE.PARCEL.COUNT                                       !LLJ
    ENDIF

    GOSUB UPDATE.ARRAY

    GOSUB WRITE.RECORD.UNLOCK

    ! Closing BDCP file                                                 !ECK

    IF BDCP.OPEN% THEN BEGIN                                            !ECK
        CLOSE BDCP.SESS.NUM%                                            !ECK
        BDCP.OPEN% = 0                                                  !ECK
    ENDIF                                                               !ECK

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      UPDATE.ARRAY
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

UPDATE.ARRAY:

    wk$ = p.arr$(indx%)

    IF      BDCP.STATUS$ = "O" THEN x.dt$ = BDCP.EXPECT.DATE$        \
    ELSE IF BDCP.STATUS$ = "R" THEN x.dt$ = BDCP.FND.DATETIME$       \
    ELSE IF BDCP.STATUS$ = "C" THEN x.dt$ = BDCP.COL.DATETIME$       \
    ELSE IF BDCP.STATUS$ = "L" THEN x.dt$ = BDCP.LST.DATETIME$       \
    ELSE IF BDCP.STATUS$ = "U" THEN x.dt$ = BDCP.RET.DATETIME$       !

    IF LEN(x.dt$) = 3 THEN                                           \
        x.dt$ = x.dt$ + PACK$("000000")                               !

    ! FOR INVESTIGATION                                                 !HDC
    ! why is this here - should it be PD or ASCII surely?               !HDC
    ! check logic before Release 1 Sprint 1 delivery                    !HDC

    ! Set required sort order for status                                !FMM

    IF BDCP.STATUS$ = "O" THEN BEGIN                                    !FMM

        IF DATE$ > UNPACK$(LEFT$(X.DT$,3)) THEN BEGIN                   !FMM
            ! Update STATUS.SORT.ORDER$ as "1" for the parcel status    !FMM
            ! "LATE".                                                   !FMM
            STATUS.SORT.ORDER$ = "1"                                    !FMM
        ENDIF ELSE BEGIN                                                !FMM
            ! Update STATUS.SORT.ORDER$ as "2" for the parcel status    !FMM
            ! "EXPECTED".                                               !FMM
            STATUS.SORT.ORDER$ = "2"                                    !FMM
        ENDIF                                                           !FMM

    ENDIF ELSE IF BDCP.STATUS$ = "R" THEN BEGIN                         !FMM
        ! set parcel as in arrived in store so allow Location           !IDC
        LOCATION.STATUS.NEW% = 0                                        !IDC

        ! If status date [X.DT$] is 0's then update global variable     !FMM
        ! F02.DATE$ with delivery date else update with found date.     !FMM
        IF X.DT$ = STRING$(6,CHR$(0)) THEN BEGIN                        !FMM
            ! Assigning delivery date to global variable                !FMM
            F02.DATE$ = UNPACK$(MID$(WK$,11,3))    !BUG FIX was 10,3    !AGCS
        ENDIF ELSE BEGIN                                                !FMM
            ! Assigning found date to global variable                   !FMM
            F02.DATE$ = UNPACK$(MID$(X.DT$,1,3))                        !FMM
        ENDIF                                                           !FMM

        ! For status 'R': Check difference between today's date and     !FMM
        ! delivery date/found date. For the order status "UNCOLLECTED"  !FMM
        ! the difference is greater than Uncollected days value. For    !FMM
        ! status "IN STORE" the difference is lesser than Uncollected   !FMM
        ! days value.                                                   !FMM

        ! Calculating return by date. DAYS.UNCOLLECTED% is retrieving   !FMM
        ! value from SOFTS file. If days uncollected value is not found !FMM
        ! the program will exit logging the event. Therefore, return    !FMM
        ! code is not checked here for UPDATE.DATE function             !FMM
        CALL UPDATE.DATE(DAYS.UNCOLLECTED%)                             !FMM
        RETURN.BY.DATE$ = F02.DATE$                                     !FMM

        IF DATE.LT(RETURN.BY.DATE$,CURRENT.DATE$) THEN BEGIN            !FMM
            ! Update STATUS.SORT.ORDER$ as "3" for the parcel status    !FMM
            ! "UNCOLLECTED RETURN TO W/H?".                             !FMM
            STATUS.SORT.ORDER$ = "3"                                    !FMM
        ENDIF ELSE BEGIN                                                !FMM
            IF X.DT$ = STRING$(6,CHR$(0)) THEN BEGIN                    !FMM
                ! Update STATUS.SORT.ORDER$ as "4" for the parcel status!FMM
                ! "IN STORE - AWAITING COLLECTION".                     !FMM
                STATUS.SORT.ORDER$ = "4"                                !FMM
            ENDIF ELSE BEGIN                                            !FMM
                ! Update STATUS.SORT.ORDER$ as "5" for the parcel status!FMM
                ! "IN STORE - FOUND".                                   !FMM
                STATUS.SORT.ORDER$ = "5"                                !FMM
            ENDIF                                                       !FMM
        ENDIF                                                           !FMM

    ENDIF ELSE IF BDCP.STATUS$ = "C" THEN BEGIN                         !FMM
        ! Update STATUS.SORT.ORDER$ as "6" for the parcel status        !FMM
        ! "COLLECTED".                                                  !FMM
        STATUS.SORT.ORDER$ = "6"                                        !FMM
    ENDIF ELSE IF BDCP.STATUS$ = "U" THEN BEGIN                         !FMM
        ! Update STATUS.SORT.ORDER$ as "7" for the parcel status        !FMM
        ! "RETURNED".                                                   !FMM
        STATUS.SORT.ORDER$ = "7"                                        !FMM

    ENDIF ELSE IF BDCP.STATUS$ = "L" THEN BEGIN                         !FMM
        ! Update STATUS.SORT.ORDER$ as "8" for the parcel status        !FMM
        ! "RETURNED".                                                   !FMM
        STATUS.SORT.ORDER$ = "8"                                        !FMM
    ENDIF                                                               !FMM

    ! add location information to the end of the main screen array      !HDC
    P.ARR$(INDX%) = STATUS.SORT.ORDER$                               \  !FMM
                  + MID$(wk$, 2, 12)                                 \  !FMM
                  + x.dt$                                            \  !FMM
                  + MID$(WK$, 20, 3)                                 \  !LLJ
                  + MID$(WK$, 23, 4)    ! add location data back        !LLJ

    ! add location and extract any update                               !HDC
    CALL PUTN2(P.ARR$(INDX%),22,LOCATION.NEW%)                          !IDC

    BDCP.LOC.CURRENT% = LOCATION.NEW%                                   !HDC
    ! Reset location                                                    !LLJ
    LOCATION.NEW% = 0                                                   !LLJ
    ! status holds indicator if Parcel allows Location setting          !IDC
    CALL PUTN2(P.ARR$(INDX%),24,LOCATION.STATUS.NEW%)                   !IDC

    wk$   = ""
    x.dt$ = ""
    STATUS.SORT.ORDER$ = ""                                             !FMM

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      READ.RECORD.LOCKED
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

READ.RECORD.LOCK:

   mismatch% = 0

   BDCP.SUPPLIER$ = MID$(p.arr$(indx%),20,3)
   BDCP.CARTON$   = MID$(p.arr$(indx%), 7,4)

   IF READ.BDCP.LOCK <> 0 THEN GOTO READ.ERROR

   ! Adding new sort orders and storing corresponding statuses.         !FMM
   wk$ = TRANSLATE$(LEFT$(p.arr$(indx%),1), "12345678", "OORRRCUL")     !FMM
   IF BDCP.STATUS$ <> wk$ THEN mismatch% = -1

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      WRITE.RECORD.UNLOCKED
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

WRITE.RECORD.UNLOCK:

   IF WRITE.BDCP.UNLOCK <> 0 THEN GOTO WRITE.ERROR

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      INITIALISATION
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

INITIALISATION:

   ! Setting Boolean flag value                                         !DDM
   FALSE = 0                                                            !DDM
   TRUE  = -1                                                           !DDM

   ! Initialising String variables                                      !DDM
   ADX.MESSAGE$       = ""                                              !DDM
   ADX.NAME$          = ""                                              !DDM
   ADX.PARM$          = ""                                              !DDM
   BLANK.MSG$         = "'" + STRING$(78, " ") ! Used to clear screen   !MLJ
                                               ! message                !MLJ
   COMMA$             = ","                                             !AKKK
   CURRENT.DATE$      = DATE$                                           !DDM
   LIMIT.STR$         = "LIMIT="                                        !DDM
   RETURN.BY.DATE$    = ""                                              !DDM
   SCREEN.TITLE$      = ""                                              !GCK
   SCREEN2.TITLE$     = ""                                              !HDC
   SOFTS.REC.LABEL$   = "EIRE"                                          !GCK
   STATUS.SORT.ORDER$ = ""                                              !FMM
   WORK.SOFTS.RECORD$ = ""                                              !DDM

   ! location variables for function key descriptions                   !IDC
    ACTIVATE.FN.TEXT$    = "ACTIV"                                      !IDC
    DEACTIVATE.FN.TEXT$  = "INACT"                                      !IDC
    LOCATION.FN.TEXT$    = "LOCN"                                       !IDC

   ! Initialising Integer variables                                     !DDM
   ACTIVE.TO.BE.SAVED    = FALSE                                        !OLJ
   CHANGE.ACCEPTED       = TRUE         ! Field change accepted flag    !ZCK
   DAYS.UNCOLLECTED%     = 0                                            !DDM
   ERROR.ON.LOCATION     = FALSE                                        !TAH
   INACTIVE.CHECK%       = 0                                            !SAH
   INITIAL.FIELD.INDEX%  = 5            ! Default location field index  !TAH
   LIMIT.LENGTH%         = 2                                            !DDM
   MATCH.POS%            = 0                                            !DDM
   NOT.DEFAULT.LOCATION  = TRUE                                         !TAH
   PRINT.KEY             = FALSE                                        !FMM
   CRTN.OPEN             = FALSE                                        !AGCS
   SOFTS.OPEN            = FALSE                                        !DDM

   bdcp.open% = 0
   lpp1%      = 14

   ! page length by number, of fields                                   !HDC
   ! 15 x 4 per column, 2 columns, equals 120                           !HDC
   ! Lines Per Page set as total fields from start field number         !HDC
   LOC.FIELDS.PER.LINE% = 3             ! Max field per record          !RLJ
   LOC.FIELDS.START%    = 3             ! First user field              !RLJ
   LOC.RECORDS.ON.PAGE% = 45            ! Total records in a page       !RLJ
   LPP2%                = 137           ! Max user fields               !RLJ

   SB.ACTION$ = "O"

   ! Setup file constants                                               !DDM
   CALL BDCP.SET
   CALL CRTN.SET                                                        !AGCS
   CALL PSUTQ.SET                                                       !AICS
   CALL SOFTS.SET                                                       !DDM
   ! Setup for locations and Orders
   CALL BDCLOCON.SET                                                    !HDC
   CALL BDCO.SET                                                        !HDC

   SB.INTEGER%  = BDCP.REPORT.NUM%
   SB.STRING$   = BDCP.FILE.NAME$
   GOSUB SB.FILE.UTILS
   BDCP.SESS.NUM% = SB.FILE.SESS.NUM%

   SB.INTEGER%  = CRTN.REPORT.NUM%                                      !AGCS
   SB.STRING$   = CRTN.FILE.NAME$                                       !AGCS
   GOSUB SB.FILE.UTILS                                                  !AGCS
   CRTN.SESS.NUM% = SB.FILE.SESS.NUM%                                   !AGCS

   SB.INTEGER%  = PSUTQ.REPORT.NUM%                                     !AICS
   SB.STRING$   = PSUTQ.FILE.NAME$                                      !AICS
   GOSUB SB.FILE.UTILS                                                  !AICS
   PSUTQ.SESS.NUM% = SB.FILE.SESS.NUM%                                  !AICS

   ! Allocate session number for file SOFTS                             !DDM
   SB.INTEGER%  = SOFTS.REPORT.NUM%                                     !DDM
   SB.STRING$   = SOFTS.FILE.NAME$                                      !DDM
   GOSUB SB.FILE.UTILS                                                  !DDM
   SOFTS.SESS.NUM% = SB.FILE.SESS.NUM%                                  !DDM

   ! Opening SOFTS file                                                 !DDM
   CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%                              !DDM
   IF END # SOFTS.SESS.NUM% THEN OPEN.ERROR                             !DDM
   OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL% AS SOFTS.SESS.NUM%     !DDM
   SOFTS.OPEN = TRUE                                                    !DDM

   ! Allocate session number for file BDCO                              !HDC
   SB.INTEGER%  = BDCO.REPORT.NUM%                                      !HDC
   SB.STRING$   = BDCO.FILE.NAME$                                       !HDC
   GOSUB SB.FILE.UTILS                                                  !HDC
   BDCO.SESS.NUM% = SB.FILE.SESS.NUM%                                   !HDC

   ! Allocate session number for file BDCLOCON                          !HDC
   SB.INTEGER%  = BDCLOCON.REPORT.NUM%                                  !HDC
   SB.STRING$   = BDCLOCON.FILE.NAME$                                   !HDC
   GOSUB SB.FILE.UTILS                                                  !HDC
   BDCLOCON.SESS.NUM% = SB.FILE.SESS.NUM%                               !HDC

   ! Setting BDCLOCON related variables                                 !QCK
   BDCLOCON.TOTAL.RECORDS% = SIZE(BDCLOCON.FILE.NAME$)                  !HDC
   BDCLOCON.TOTAL.RECORDS% = BDCLOCON.TOTAL.RECORDS% / BDCLOCON.RECL%   !HDC
   ! allow for number of fields per record, plus 1 as field position    !HDC
   ! starts at 2 plus one more for each record to hold the status       !HDC
   LOC.ARRAY.LIMIT% = (BDCLOCON.TOTAL.RECORDS%*LOC.FIELDS.PER.LINE%) + \!HDC
                      1 + BDCLOCON.TOTAL.RECORDS%                       !HDC
   DIM LOCATION.ARRAY$(LOC.ARRAY.LIMIT%)                                !HDC

   ! the parcel order limit is based upon existing file constraints     !HDC
   ! referenced in the existing program                                 !HDC
   DIM ORDER.PARCELS%(3000,1)                                           !HDC

   ! To check whether the store is UK or ROI                            !GCK
   GOSUB CHECK.STORE                                                    !GCK

   ! To parse the uncollected days value from SOFTS 60th record         !DDM
   GOSUB PARSE.UNCOLLECTED.DAYS                                         !DDM

RETURN

\***********************************************************************!GCK
\*                                                                      !GCK
\* CHECK.STORE:                                                         !GCK
\* Reads and checks whether the store is configured as ROI OR UK from   !GCK
\* SOFTS 19th record.                                                   !GCK
\*                                                                      !GCK
\***********************************************************************!GCK

CHECK.STORE:                                                            !GCK

    ! Read SOFTS 19th record                                            !GCK
    SOFTS.REC.NUM% = 19                                                 !GCK
    IF READ.SOFTS THEN GOSUB READ.ERROR                                 !GCK

    ! Update the SCREEN.TITLE$ as "ORDER & COLLECT PARCEL MANAGEMENT    !SAH
    ! SCREEN" or "ORDER & COLLECT LOCATIONS MANAGEMENT SCREEN"          !SAH
    ! irrespective of it's store location (UK or ROI store).            !SAH

    TITLE.START$ = "ORDER & COLLECT "                                   !SAH
    TITLE.END1$ = "PARCEL MANAGEMENT SCREEN"                            !JDC
    SCREEN.TITLE$  = TITLE.START$ + TITLE.END1$                         !JDC

    IF MANAGE.LOCATION THEN BEGIN                                       !JDC
        TITLE.END2$ = "LOCATIONS MANAGEMENT SCREEN"                     !JDC
    ENDIF ELSE BEGIN                                                    !JDC
        TITLE.END2$ = "LOCATION SELECTION SCREEN"                       !JDC
    ENDIF                                                               !JDC
    SCREEN2.TITLE$ = TITLE.START$ + TITLE.END2$                         !JDC

RETURN                                                                  !GCK


\***********************************************************************!DDM
\*                                                                      !DDM
\* PARSE.UNCOLLECTED.DAYS:                                              !DDM
\* Reads and parses the uncollected days value from SOFTS 60th record   !DDM
\*                                                                      !DDM
\***********************************************************************!DDM

PARSE.UNCOLLECTED.DAYS:                                                 !DDM

    ! Read SOFTS 60th record                                            !DDM
    SOFTS.REC.NUM% = 60                                                 !DDM
    IF READ.SOFTS THEN GOSUB READ.ERROR                                 !DDM

    ! Initialising to FALSE                                             !DDM
    SOFTS.LIMIT.VAL.FOUND = FALSE                                       !DDM

    ! Parse uncollected days value. If the match is not found then      !DDM
    ! sets SOFTS.LIMIT.VAL.FOUND to FALSE                               !DDM
    MATCH.POS% = MATCH(LIMIT.STR$, SOFTS.RECORD$, 1)                    !DDM

    IF MATCH.POS% > 0 THEN BEGIN                                        !DDM
        ! Setting to TRUE as match found                                !DDM
        SOFTS.LIMIT.VAL.FOUND = TRUE                                    !DDM
        ! Storing the SOFTS record                                      !DDM
        WORK.SOFTS.RECORD$ = \                                          !DDM
                MID$(SOFTS.RECORD$, MATCH.POS% + 6, LIMIT.LENGTH%)      !DDM
        ! Storing the uncollected days                                  !DDM
        DAYS.UNCOLLECTED% = VAL(WORK.SOFTS.RECORD$)                     !DDM
    ENDIF ELSE BEGIN                                                    !DDM
        SOFTS.LIMIT.VAL.FOUND = FALSE                                   !DDM
    ENDIF                                                               !DDM

    ! De-allocate SOFTS session                                         !DDM
    SB.INTEGER% = SOFTS.SESS.NUM%                                       !DDM
    SB.STRING$  = ""                                                    !DDM
    CLOSE SB.INTEGER%                                                   !DDM
    SOFTS.OPEN  = FALSE                                                 !DDM
    GOSUB SB.FILE.UTILS                                                 !DDM

RETURN                                                                  !DDM

\***********************************************************************!DDM
\*                                                                      !DDM
\* PRINT.BDCRPT:                                                        !DDM
\* Invokes PSD96.286 program at background and Prints BDCRPT            !DDM
\* (report file)                                                        !DDM
\*                                                                      !DDM
\***********************************************************************!DDM

PRINT.BDCRPT:                                                           !DDM

    ! Setting the values to the parameter variables                     !DDM
    ADX.MESSAGE$ = "Started by PSD85 BDC Parcel Mgt Screen."            !DDM
    ADX.NAME$    = "ADX_UPGM:PSD96.286"                                 !DDM
    ADX.PARM$    = "ONDEMAND"                                           !DDM

    ! Initiate PSD96.286 at background                                  !DDM
    RC% = ADXSTART(ADX.NAME$,ADX.PARM$,ADX.MESSAGE$)                    !DDM

    ! If success then display BEMF '388' message else log the event 42. !ECK
    IF RC% = 0 THEN BEGIN                                               !DDM
        CALL DM.FOCUS ("", "MESSAGE(388,'')")                           !ECK
    ENDIF ELSE BEGIN                                                    !DDM
        ! Logging ADXSTART error                                        !DDM
        MESSAGE.NUMBER% = 0                                             !DDM
        EVENT.NUMBER%   = 42                                            !DDM
        VAR.STRING.1$   = ADX.NAME$                                     !DDM
        VAR.STRING.2$   = ADX.PARM$                                     !DDM
        CALL APPLICATION.LOG(MESSAGE.NUMBER%,VAR.STRING.1$, \           !DDM
                             VAR.STRING.2$,EVENT.NUMBER%)               !DDM
        CALL DM.FOCUS ("", "MESSAGE(313,'')")                           !DDM
    ENDIF                                                               !DDM

RETURN                                                                  !DDM

\*********************************************************************
\***
\***    SUBROUTINE      :      LOAD.PARCELS
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

LOAD.PARCELS:

   file.loaded% = -1
   p.cnt%       = 0

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Open Boots.com Parcel file
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   IF bdcp.open% THEN CLOSE BDCP.SESS.NUM%
   bdcp.open% = 0

   ! Modifying the below line. CURRENT.REPORT.NUM% is updated with      !ECK
   ! BDCP.REPORT.NUM%                                                   !ECK
   CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                               !ECK

   ! If error occurs while opening BDCP file go to the subroutine       !ECK
   ! OPEN.ERROR                                                         !ECK

   IF END #BDCP.SESS.NUM% THEN OPEN.ERROR                               !ECK
   OPEN BDCP.FILE.NAME$ AS BDCP.SESS.NUM% BUFFSIZE 32768              \
                                                       NOWRITE NODEL
   bdcp.open% = -1

    IF BDCO.OPEN% = 0 THEN BEGIN                                        !HDC
        IF END #BDCO.SESS.NUM% THEN OPEN.ERROR                          !HDC
        OPEN BDCO.FILE.NAME$ KEYED RECL BDCO.RECL% AS BDCO.SESS.NUM% \  !HDC
         NOWRITE NODEL                                                  !HDC
        BDCO.OPEN% = 1                                                  !HDC
    ENDIF                                                               !HDC

no.bdcp.file:

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Set initial values
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   p1$       = ""
   p2$       = ""
   p3$       = ""
   p4$       = ""
   p5$       = ""
   p6$       = ""
   P7$       = ""                                                       !LLJ
   P8$       = ""                                                       !LLJ

   blk.size% = 32256
   num.blks% = (SIZE(BDCP.FILE.NAME$) / blk.size%) +1
   last.blk% = MOD(SIZE(BDCP.FILE.NAME$),  blk.size%)
   num.sect% = 63
   num.recs% = 508 / BDCP.RECL%

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Extract records from Boots.com parcel file
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   IF END #BDCP.SESS.NUM% THEN READ.ERROR
   FOR i% = 1 TO num.blks%
      CALL DM.STATUS("MESSAGE(221,'Processing .. >"                    \
                     + STRING$(      (i%*50)/num.blks% , CHR$(178))    \
                     + STRING$(50 - ((i%*50)/num.blks%), CHR$(176))    \
                     + "<')")
      IF i% = num.blks% THEN BEGIN
         form$ = "C" + STR$(last.blk%)
         num.sect% = last.blk% / 512
      ENDIF ELSE BEGIN
         form$ = "C" + STR$(blk.size%)
      ENDIF

      ! added missing error settings
      CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%                            !HDC
      FILE.OPERATION$ = "R"                                             !HDC
      READ FORM form$; # BDCP.SESS.NUM%; blk$

      FOR j% = 1 TO num.sect%
         IF i% = 1 AND j% = 1 THEN j% = 2    ! Skip stats
         sect$ = MID$(blk$, ((j% -1) *512) +5, 508)
         IF MID$(sect$,1,7) <> STRING$(7, CHR$(0)) THEN BEGIN
            FOR k% = 1 TO num.recs%
               rcd$ = MID$(sect$, ((k% -1)* BDCP.RECL%) +1, BDCP.RECL%)
               IF MID$(rcd$,1,7) <> STRING$(7, CHR$(0)) THEN BEGIN

                  p.cnt% = p.cnt% +1          ! Increment counter

                  wk$ = MID$(rcd$,16,1)
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  \* Extract required date/time for status message
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  IF      wk$ = "O" THEN x.dt$ = MID$(rcd$,13,3)       \
                  ELSE IF wk$ = "R" THEN x.dt$ = MID$(rcd$,46,6)       \
                  ELSE IF wk$ = "C" THEN x.dt$ = MID$(rcd$,24,6)       \
                  ELSE IF wk$ = "L" THEN x.dt$ = MID$(rcd$,39,6)       \
                  ELSE IF wk$ = "U" THEN x.dt$ = MID$(rcd$,32,6)       !
                  IF LEN(x.dt$) = 3 THEN                               \
                     x.dt$ = x.dt$ + PACK$("000000")

                  ! Checks the parcel status to Enable the 'F9' key     !FMM
                  IF wk$ = "O" OR wk$ = "R" OR wk$ = "L" THEN \         !FMM
                      PRINT.KEY = TRUE                                  !FMM

                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  \* Extract delivery date (not time)
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  d.dt$ = MID$(rcd$,17,3)

                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  \* Set required sort order for status       1 byte
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -

                  IF WK$ = "O" THEN BEGIN                               !FMM

                     IF DATE$ > UNPACK$(LEFT$(X.DT$,3)) THEN BEGIN      !FMM
                        ! Update STATUS.SORT.ORDER$ as "1" for parcel   !FMM
                        ! status "LATE".                                !FMM
                        STATUS.SORT.ORDER$ = "1"                        !FMM
                     ENDIF ELSE BEGIN                                   !FMM
                        ! Update STATUS.SORT.ORDER$ as "2" for parcel   !FMM
                        ! status "EXPECTED".                            !FMM
                        STATUS.SORT.ORDER$ = "2"                        !FMM
                     ENDIF                                              !FMM

                  ENDIF ELSE IF WK$ = "R" THEN BEGIN                    !FMM

                     ! If status date [X.DT$] is 0's then update global !FMM
                     ! variable F02.DATE$ with delivery date else update!FMM
                     ! with found date.                                 !FMM
                     IF X.DT$ = STRING$(6,CHR$(0)) THEN BEGIN           !FMM
                        ! Assigning delivery date to global variable    !FMM
                        F02.DATE$ = UNPACK$(D.DT$)                      !FMM
                     ENDIF ELSE BEGIN                                   !FMM
                        ! Assigning found date to global variable       !FMM
                        F02.DATE$ = UNPACK$(MID$(X.DT$,1,3))            !FMM
                     ENDIF                                              !FMM

                     ! For status 'R': Check difference between today's !FMM
                     ! date and delivery date/found date. For the order !FMM
                     ! status "UNCOLLECTED" the difference is greater   !FMM
                     ! than Uncollected days value. For status          !FMM
                     ! "IN STORE" the difference is lesser than         !FMM
                     ! Uncollected days value.                          !FMM

                     ! Calculating return by date. DAYS.UNCOLLECTED% is !FMM
                     ! retrieving value from SOFTS file. If days        !FMM
                     ! uncollected value is not found the program will  !FMM
                     ! exit logging the event. Therefore, return code is!FMM
                     ! not checked here for UPDATE.DATE function        !FMM
                     CALL UPDATE.DATE(DAYS.UNCOLLECTED%)                !FMM
                     RETURN.BY.DATE$ = F02.DATE$                        !FMM

                     IF DATE.LT(RETURN.BY.DATE$,CURRENT.DATE$)          \FMM
                                                  THEN BEGIN            !FMM
                        ! Update STATUS.SORT.ORDER$ as "3" for parcel   !FMM
                        ! status "UNCOLLECTED RETURN TO W/H?" and update!FMM
                        ! the P1$ string.                               !FMM
                        STATUS.SORT.ORDER$ = "3"                        !FMM
                     ENDIF ELSE BEGIN                                   !FMM
                        IF X.DT$ = STRING$(6,CHR$(0)) THEN BEGIN        !FMM
                           ! Update STATUS.SORT.ORDER$ as "4" for parcel!FMM
                           ! status "IN STORE - AWAITING COLLECTION"    !FMM
                           STATUS.SORT.ORDER$ = "4"                     !FMM
                        ENDIF ELSE BEGIN                                !FMM
                           ! Update STATUS.SORT.ORDER$ as "5" for parcel!FMM
                           ! status "IN STORE - FOUND".                 !FMM
                           STATUS.SORT.ORDER$ = "5"                     !FMM
                        ENDIF                                           !FMM
                     ENDIF                                              !FMM

                  ENDIF ELSE IF WK$ = "C" THEN BEGIN                    !FMM
                     ! Update STATUS.SORT.ORDER$ as "6" for parcel      !FMM
                     ! status "COLLECTED".                              !FMM
                     STATUS.SORT.ORDER$ = "6"                           !FMM
                  ENDIF ELSE IF WK$ = "U" THEN BEGIN                    !FMM
                     ! Update STATUS.SORT.ORDER$ as "7" for parcel      !FMM
                     ! status "RETURNED".                               !FMM
                     STATUS.SORT.ORDER$ = "7"                           !FMM
                  ENDIF ELSE IF WK$ = "L" THEN BEGIN                    !FMM
                     ! Update STATUS.SORT.ORDER$ as "8" for parcel      !FMM
                     ! status "RETURNED".                               !FMM
                     STATUS.SORT.ORDER$ = "8"                           !FMM
                  ENDIF                                                 !FMM

                  ! Append the string P1$ with STATUS.SORT.ORDER$       !FMM
                  p1$ = p1$ + STATUS.SORT.ORDER$                        !FMM

                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  \* Order Number                             5 bytes
                  \* if Order number is zeros then set to F's
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  IF MID$(rcd$,8,5) = STRING$(5,CHR$(0)) THEN          \
                     p2$ = p2$ + STRING$(5,CHR$(0FFh))                 \
                  ELSE                                                 \
                     p2$ = p2$ + MID$(rcd$,8,5)

                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  \* Parcel Number                            4 bytes
                  \* Date Received                            3 bytes
                  \* Date needed for status msg               6 bytes
                  \* Append supplier number for key           3 bytes
                  \*                                         ________
                  \* Total                                   22 bytes
                  \* - - - - - - - - - - - - - - - - - - - - - - - - - -
                  p3$ = p3$ + MID$(rcd$,4,4)
                  p4$ = p4$ + d.dt$
                  p5$ = p5$ + x.dt$
                  p6$ = p6$ + MID$(rcd$,1,3)

                  !*****************************************************!HDC
                  !* Code change H block marker - START                 !HDC
                  !*****************************************************!HDC
                  ! check the number or current Parcels for the order
                  ! and mark the staus into the main array for the order
                  ! so we can use it later
                  BDCO.ORDER$ = UNPACK$(MID$(RCD$,8,5))                 !LLJ
                  ORDERS.OFFSET% = \
                   MATCH(":"+BDCO.ORDER$,ORDERS.OFFSET$,1)
                  IF ORDERS.OFFSET% = 0 THEN BEGIN
                      MAX.PARCELS% = 0
                      IF END #BDCO.SESS.NUM% THEN READ.ERROR

                      BDCO.KEY$ = MID$(RCD$,8,5)                        !LLJ

                      IF BDCO.KEY$ <> PACK$(STRING$(10,"0")) \
                      AND BDCO.KEY$ <> STRING$(5," ") \
                      THEN BEGIN
                          CURRENT.REPORT.NUM% = BDCO.REPORT.NUM%
                          FILE.OPERATION$ = "R"

                          READ FORM "T9,C246" ; #BDCO.SESS.NUM% \
                          KEY MID$(RCD$,1,3) + MID$(RCD$,8,5); P8$      !LLJ
                          ! look to the end of the parcel list, which
                          ! will always have 0s since it can only be
                          ! up to 55 per order (file limit)
                          MAX.PARCELS% = MATCH(STRING$(8,"0"),         \!LLJ
                                         UNPACK$(P8$),1)                !LLJ
                          IF MAX.PARCELS% > 0 THEN BEGIN
                              MAX.PARCELS% = ((MAX.PARCELS%-1)/4)+1
                          ENDIF ELSE BEGIN
                              MAX.PARCELS% = 55
                          ENDIF
                      ENDIF
                      P8$ = RIGHT$("00"+STR$(MAX.PARCELS%),2)

                      ! add the order to the index and put parcel total
                      ! in element 0 for reference
                      ORDERS.OFFSET$ = ORDERS.OFFSET$ + ":" +          \!LLJ
                                        BDCO.ORDER$                     !LLJ
                      ORDERS.OFFSET% = ((LEN(ORDERS.OFFSET$)-1)/11)
                      ORDER.PARCELS%(ORDERS.OFFSET%,0) = MAX.PARCELS%
                  ENDIF ELSE BEGIN
                      ORDERS.OFFSET% = ((ORDERS.OFFSET%-1)/11)+1
                      P8$ = STR$(ORDER.PARCELS%(ORDERS.OFFSET%,0))
                  ENDIF
                  P7$ = P7$ + MID$(RCD$,53,4)                           !LLJ

                  ! update if the parcel is ready or not for the
                  ! overall order staus
                  IF MID$(RCD$,16,1) = "R" THEN BEGIN                   !LLJ
                      ORDER.PARCELS%(ORDERS.OFFSET%,1) = \
                       ORDER.PARCELS%(ORDERS.OFFSET%,1) + 1
                  ENDIF
                  !*****************************************************!HDC
                  !* Code change H block marker - END                   !HDC
                  !*****************************************************!HDC
               ENDIF ELSE BEGIN
                  k% = num.recs%
               ENDIF    ! record populated
            NEXT k%     ! records per sector
         ENDIF          ! sector populated
      NEXT j%           ! sectors per block
   NEXT i%              ! blocks per file

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Populate array
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   DIM p.arr$(p.cnt%)
   !********************************************************************!HDC
   ! Array layout                                                       !HDC
   ! 12222233334445555556667777                                         !HDC
   ! 1 = Status sort Order (ASCII)                                      !HDC
   ! 2 = Order Number(PD)                                               !HDC
   ! 3 = Parcel number (PD)                                             !HDC
   ! 4 = Date received (PD)                                             !HDC
   ! 5 = Date for Status Msg (PD)                                       !HDC
   ! 6 = Supplier Number (PD)                                           !HDC
   ! 7 = 2x INT2 - Current Location + Location Status                   !HDC
   !********************************************************************!HDC
   FOR i% = 1 TO p.cnt%
      p.arr$(i%) = MID$(p1$,((i% -1) * 1) +1, 1)                       \
                 + MID$(p2$,((i% -1) * 5) +1, 5)                       \
                 + MID$(p3$,((i% -1) * 4) +1, 4)                       \
                 + MID$(p4$,((i% -1) * 3) +1, 3)                       \
                 + MID$(p5$,((i% -1) * 6) +1, 6)                       \
                 + MID$(p6$,((i% -1) * 3) +1, 3)                       \
                 + MID$(P7$,((I% -1) * 4) +1, 4)                        !LLJ
   NEXT i%

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Sort the array
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   CALL DM.STATUS("MESSAGE(221,'Sorting List.')")
   IF p.cnt% > 0 THEN BEGIN
      RC% = CSORT(VARPTR(p.arr$(0)), p.cnt%)
   ENDIF
   CALL DM.STATUS("MESSAGE(221,'Sort Complete.')")

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Close file and reopen for keyed access (with update)
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   CLOSE BDCP.SESS.NUM%
   bdcp.open% =  0

   IF BDCO.OPEN% THEN CLOSE BDCO.SESS.NUM%                              !HDC

   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   \* Tidy up
   \* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

   blk$  = ""
   rcd$  = ""
   sect$ = ""
   wk$   = ""
   p1$   = ""
   p2$   = ""
   p3$   = ""
   p4$   = ""
   p5$   = ""
   p6$   = ""
   STATUS.SORT.ORDER$ = ""                                              !FMM

RETURN

\***********************************************************************!QCK
\*                                                                      !QCK
\* OPEN.BDCLOCON:                                                       !QCK
\*                                                                      !QCK
\* Opens the file BDCLOCON                                              !QCK
\*                                                                      !QCK
\***********************************************************************!QCK
OPEN.BDCLOCON:                                                          !QCK
                                                                        !QCK
    ! Opening BDCLOCON file                                             !QCK
    IF NOT BDCLOCON.OPEN THEN BEGIN                                     !QCK
        CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%                      !QCK
        FILE.OPERATION$     = "O"   ! Open                              !QCK
                                                                        !QCK
        ! Open BDCLOCON file                                            !QCK
        ! IF END# statement is not used here so that all open errors for!QCK
        ! BDCLOCON is captured in ERR.DETECETED, including the open     !QCK
        ! error from the functions FUNC.GET.LOCATION$ and               !QCK
        ! FUNC.UPDATE.ALL.LOCATION.RECORDS%                             !QCK
        OPEN BDCLOCON.FILE.NAME$ DIRECT RECL BDCLOCON.RECL% AS \        !QCK
        BDCLOCON.SESS.NUM%                                              !QCK
                                                                        !QCK
        ! Set the open flag                                             !QCK
        BDCLOCON.OPEN = TRUE                                            !QCK
    ENDIF                                                               !QCK
                                                                        !QCK
RETURN                                                                  !QCK
                                                                        !QCK
\********************************************************************
\***
\***    SUBROUTINE      :      FILE.ERROR
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

FILE.ERROR:

    ! Hide the function keys F3 and F9 if BATCH.SCREEN.FLAG$ is "S"     !ECK

    IF BATCH.SCREEN.FLAG$ = "S" THEN BEGIN                              !ECK
        CALL DM.HIDE.FN.KEY(3)                                          !ECK
        CALL DM.HIDE.FN.KEY(9)                                          !ECK
    ENDIF                                                               !ECK

    IF SB.ACTION$ = "C" THEN RETURN             ! Ignore close errs

    EVENT.NUMBER%   = 106

    MESSAGE.NUMBER% = 0

    FILE.NO$ = CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +                  \
               CHR$(SHIFT(CURRENT.REPORT.NUM%,0))

    VAR.STRING.2$ = RIGHT$("000" + STR$(CURRENT.REPORT.NUM%),3)


    ! Updating the variables MESSAGE.NUMBER% and VAR.STRING.2$ as per   !ECK
    ! the file operation, as part of CR02 changes.                      !ECK

    IF FILE.OPERATION$ = "O" THEN BEGIN                                 !ECK
        MESSAGE.NUMBER% = 501                                           !ECK
    ENDIF ELSE BEGIN                                                    !ECK

        IF FILE.OPERATION$ = "R" THEN BEGIN                             !ECK
            MESSAGE.NUMBER% = 508                                       !ECK
            VAR.STRING.2$ = RIGHT$("000" + STR$(CURRENT.REPORT.NUM%),3) \ECK
                            + CURRENT.CODE$  + BDCO.ORDER$                           !ECK
        ENDIF ELSE BEGIN                                                !ECK
            MESSAGE.NUMBER% = 509                                       !ECK

            IF FILE.OPERATION$ = "W" THEN BEGIN                         !ECK
                VAR.STRING.2$ = RIGHT$("000" +                          \ECK
                                       STR$(CURRENT.REPORT.NUM%), 3)    !ECK
            ENDIF                                                       !ECK

        ENDIF                                                           !ECK

    ENDIF                                                               !ECK

    VAR.STRING.1$ = FILE.OPERATION$ +                                \
                    FILE.NO$ +                                       \
                    PACK$(STRING$(12,"0"))

    ! If BATCH.SCREEN.FLAG$ is not "S" update MESSAGE.NUMBER% with 0.   !ECK

    IF BATCH.SCREEN.FLAG$ <> "S" THEN BEGIN                             !ECK
        MESSAGE.NUMBER% = 0                                             !ECK
    ENDIF                                                               !ECK

    CALL APPLICATION.LOG(MESSAGE.NUMBER%,                            \
                         VAR.STRING.1$,                              \
                         VAR.STRING.2$,                              \
                         EVENT.NUMBER%)

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :      TERMINATE
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

TERMINATE:

      SB.ACTION$ = "C"

      IF bdcp.open% THEN BEGIN
         SB.INTEGER% = BDCP.SESS.NUM%
         SB.STRING$ = ""
         CLOSE SB.INTEGER%
         GOSUB SB.FILE.UTILS
         bdcp.open% = 0
      ENDIF

      IF SOFTS.OPEN THEN BEGIN                                          !DDM
         ! De-allocate session number for SOFTS file                    !DDM
         SB.INTEGER% = SOFTS.SESS.NUM%                                  !DDM
         SB.STRING$  = ""                                               !DDM
         CLOSE SB.INTEGER%                                              !DDM
         SOFTS.OPEN  = FALSE                                            !DDM
         GOSUB SB.FILE.UTILS                                            !DDM
      ENDIF                                                             !DDM

      IF BDCLOCON.OPEN THEN BEGIN                                       !HDC
         ! De-allocate session number for LOCATION file                 !HDC
         SB.INTEGER% = BDCLOCON.SESS.NUM%                               !HDC
         SB.STRING$  = ""                                               !HDC
         CLOSE SB.INTEGER%                                              !HDC
         BDCLOCON.OPEN  = FALSE                                         !HDC
         GOSUB SB.FILE.UTILS                                            !HDC
      ENDIF                                                             !HDC
                                                                        !HDC
RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       SB.FILE.UTILS
\***
\********************************************************************
\***
\***      Allocate/report/de-allocate a file session number
\***
\********************************************************************
\***
\***      Parameters : 2 or 3 (depending on action)
\***
\***         SB.ACTION$  = "O" for allocate file session number
\***                       "R" for report file session number
\***                       "C" for de-allocate file session number
\***
\***         SB.INTEGER% = file reporting number for action "O" or
\***                       file session number for actions "R" or "C"
\***
\***         SB.STRING$  = logical file name for action "O" or
\***                       null ("") for action "R" and "C"
\***
\***      Output : 1 or 2 (depending on action)
\***
\***         SB.FILE.NAME$     = logical file name for action "R"
\***
\***         SB.FILE.SESS.NUM% = file session number for action "O"
\***                             or undefined for action "C"
\***         OR
\***         SB.FILE.REP.NUM%  = file reporting number for action "R"
\***                             or undefined for action "C"
\***
\*** - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

SB.FILE.UTILS:

   CALL SESS.NUM.UTILITY(SB.ACTION$, SB.INTEGER%, SB.STRING$)

   IF SB.ACTION$ = "O" THEN BEGIN

       SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%

   ENDIF ELSE IF SB.ACTION$ = "R" THEN BEGIN

       SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
       SB.FILE.NAME$ = F20.FILE.NAME$

   ENDIF

RETURN
\***********************************************************************!LLJ
\* UPDATE.PARCEL.COUNT:                                                 !LLJ
\* This subroutine will update the BDCLOCON.PARCEL.COUNT%               !LLJ
\*                                                                      !LLJ
\***********************************************************************!LLJ
UPDATE.PARCEL.COUNT:                                                    !LLJ
                                                                        !LLJ
    !Update only if the location has value                              !LLJ
    IF BDCLOCON.RECORD.NUM% > 0 AND BDCLOCON.RECORD.NUM% < 999         \!
                                                             THEN BEGIN !LLJ

        ! Open BDCLOCON file                                            !QCK
        GOSUB OPEN.BDCLOCON                                             !QCK

        !If there is read error then error out                          !LLJ
        IF READ.BDCLOCON THEN BEGIN                                     !LLJ
            GOSUB READ.ERROR                                            !LLJ
        ENDIF                                                           !LLJ
                                                                        !LLJ
        !Increment new location parcel count                            !LLJ
        BDCLOCON.PARCEL.COUNT% = BDCLOCON.PARCEL.COUNT% + PARCEL.COUNT% !LLJ
                                                                        !LLJ
        !If there is write error then error out                         !LLJ
        IF WRITE.BDCLOCON THEN BEGIN                                    !LLJ
            GOSUB WRITE.ERROR                                           !LLJ
        ENDIF                                                           !LLJ

        ! Closing BDCLOCON file                                         !QCK
        IF BDCLOCON.OPEN THEN BEGIN                                     !QCK
            CLOSE BDCLOCON.SESS.NUM%                                    !QCK
                                                                        !QCK
            ! Reset the flag                                            !QCK
            BDCLOCON.OPEN = FALSE                                       !QCK
        ENDIF                                                           !QCK

    ENDIF                                                               !LLJ
                                                                        !LLJ
RETURN                                                                  !LLJ
\********************************************************************
\***
\***    Error Routines
\***
\********************************************************************
\***
\***    OPEN Error
\***
\********************************************************************

OPEN.ERROR:

   FILE.OPERATION$ = "O"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    READ Error
\***
\********************************************************************

READ.ERROR:

   FILE.OPERATION$ = "R"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    WRITE Error
\***
\********************************************************************

WRITE.ERROR:

   FILE.OPERATION$ = "W"
   GOSUB FILE.ERROR
   GOTO END.PROGRAM

RETURN

\********************************************************************
\***
\***    General Error
\***
\********************************************************************

ERR.DETECTED:

   ! Ignore Close errors                                                !ECK

   IF ERR = "CU" THEN RESUME                                            !ECK

   IF ERR = "CM"                                                    \
   OR ERR = "CT" THEN BEGIN
      MESSAGE.NUMBER% = 553
      VAR.STRING.1$   = "B5000" + CHAINING.TO.PROG$
      VAR.STRING.2$   = "PS"    + CHAINING.TO.PROG$
      EVENT.NUMBER%   = 18
      rc% = APPLICATION.LOG (MESSAGE.NUMBER%,                       \
                             VAR.STRING.1$,                         \
                             VAR.STRING.2$,                         \
                             EVENT.NUMBER%)
   ENDIF

    ! If BDCLOCON access fails                                          !QCK
    IF ERRF% = BDCLOCON.SESS.NUM% THEN BEGIN                            !QCK
        ! If operation is Open                                          !QCK
        IF FILE.OPERATION$ = "O" THEN BEGIN                             !QCK
            GOSUB OPEN.ERROR                                            !QCK
        ENDIF ELSE IF FILE.OPERATION$ = "R" THEN BEGIN                  !QCK
            ! If operation is Read                                      !QCK
            GOSUB READ.ERROR                                            !QCK
        ENDIF                                                           !QCK
    ENDIF                                                               !QCK

   IF (ERRN AND 0000FFFFH) = 400CH THEN BEGIN   ! File access conflict  !DDM

       IF ERRF% = CRTN.SESS.NUM% THEN BEGIN                             !AHCS
          RESUME DO.ROLLBACK ! Rollback BDCP parcel record update       !AHCS
       ENDIF ELSE BEGIN                                                 !AHCS
          RESUME OPEN.ERROR                                             !DDM
      ENDIF                                                             !AHCS
   ENDIF                                                                !DDM

   !  *** any other system error ***

   CALL STANDARD.ERROR.DETECTED(ERRN,                               \
                                ERRF%,                              \
                                ERRL,                               \
                                ERR)

   RESUME END.PROGRAM


END








