\*****************************************************************************
\* Program:       PSS20             CREDIT CLAIMS REPORTING                  *
\* Created:       04 November 1993                                           *
\* Author:        Mick Bayliss                                               *
\*                                                                           *
\* Current Version        : D                                                *
\*                                                                           *
\* Date of last Amendment : 19th August 1994                                 *
\* Last Amendment by      : Michael J. Kelsall                               *
\*                                                                           *
\*****************************************************************************
\*****************************************************************************
\* PROGRAM OVERVIEW                                                          *
\*                                                                           *
\*  This Program is part of the Returns/Automatic Credit Claiming System     *
\*  and will be initiated daily via sleeper mode, or ad-hoc in command mode. *
\*  This program will run if the Chilled Foods Software is active or the     *
\*  Returns Software is active. The purpose of this program is to produce    *
\*  reports on Credit Claiming activity and to perform weekly housekeeping   *
\*  on Credit Claims files.                                                  *
\*                                                                           *
\*  If CHILLED FOODS ACTIVE then the Chilled Foods Wastage Report (CCCFW)    *
\*  will be produced daily from CCLAM and CCITF. This report shows totals    *
\*  of credit claimed for chilled foods wastage and the value of stock sold  *
\*  to staff from the Credit Claiming Control file (CCTRL), the difference   *
\*  between the above is calculated and displayed. Housekeeping of the       *
\*  Claims files is performed weekly, on a Sunday or on a subsequent day if  * !4.1BMG
\*  Sunday's housekeeping fails. The CCTRL file is updated to show the last  * !4.1BMG
\*  successful housekeeping date (weekly) and to reset the Staff Sales       *
\*  figure to zero (daily).                                                  *
\*                                                                           *
\*  If RETURNS IS ACTIVE then both reporting and housekeeping are performed  *
\*  weekly on a Sunday or subsequent day due to failure. The Claiming        * !4.1BMG
\*  activity shown in CCLAM and CCITF is reported in two reports, one a      *
\*  Summary report (CCSMY) and one a detailed report (CCDET). Each report    *
\*  shows Credit Claims data ordered by claim number within Business Centre. *
\*  In addition the detailed report shows Item level data for credit claimed.*
\*  Housekeeping is as for chilled foods, as is the CCTRL update.            *
\*                                                                           *
\*  If neither Chilled Foods nor Returns is active the no reporting or       *
\*  housekeeping takes place. The CCTRL Staff Sales value will still be      *
\*  reset to zero (daily).                                                   *
 \*                                                                           *
\*****************************************************************************
\*                                                                           *
\*   Version B (part 1)   Clive Norris              23rd February 1994       *
\*                                                                           *
\*     Changed to include maintenance of the new Credit Claiming             *
\*     Stocktaking file (CCSTS) prior to Housekeeping when Returns is active *
\*     The CCSTS file will be updated with the latest credit claim numbers   *
\*     for each business centre from the CCLAM file.                         *
\*                                                                           *
\*   Version B (part 2)         Mick Bayliss              14th March 1994    *
\*                                                                           *
\*     If reason for claim is Repair/Estimate (23) or Plan 4 (26) then do    *
\*     not show item level detail on Detail Report (CCDET) (Amendment to     *
\*     RET.RPT.END.CLAIM:)                                                   *
\*                                                                           *
\*   Version C            Michael J. Kelsall        27th April 1994          *
\*                                                                           *
\*     Changes to fix problems detected during Returns system testing;       *
\*     - Prevention of program loop if zero report date in CCTRL.            *
\*     - Change of report description from credit note to credit claim       *
\*     - Adjustment of qty field on CCDET from two to four digits.           *
\*     - Change to remove item level details from cancelled claims on both   *
\*         CCSMY and CCDET, also addition of line to indicate cancellation.  *
\*     - Change to categorise all non-alphabetic Business centres encountered*
\*         to a single BC table record.                                      *
\*     - Change to combine EPSOM DIRECTS and OTHER DIRECTS supply routes to  *
\*        a single supply route of 'O' - 'DIRECTS'                           *
\*     - Removal of error handling of failed record deletion from CCLAM/CCITF*
\*                                                                           *
\*   Version D            David Smallwood           18th May 1994            *
\*                                                                           *
\*     - Do not display item header information if no items to display.      *
\*     - Do not display item code "9999991"                                  *
\*     - Calculate item price from CCITF.PRICE% (actually item value)        *
\*                                                                           *
\*   Version D (II)       Michael J. Kelsall        18th May 1994            *
\*                                                                           *
\*     - Slight mod to this mod to handle pharmacy qtys (claim/(qty/pack))   *
\*     - Change to handle bar code formats correctly on CCDET                *
\*     - Change to include pack size on CCDET for any pharmacy lines         *
\*     - Change to add 'quick' fix to page throw on CCDET item level output  *
\*     - Change to add item claim value to CCDET report                      *
\*                                                                           *
\*   Version 1.3       Julia Stones              4th July 1996               *
\*                                                                           *
\*     PSS20 currently only runs on a Sunday or on a subsequent day if       * !4.1BMG
\*     Sunday's run failed for any reason.                                   * !4.1BMG
\*     Change to PSS20 to housekeep claims daily i.e. claims over 6 days     *
\*     old that were despatched before the last run of PSS20 and have been   *
\*     retrieved to the centre will be deleted from CCLAM and CCITF.         *
\*     Reporting will remain weekly.                                         *
\*                                                                           *
\*   Version 1.4       Rebecca Dakin              3rd February 1998          *
\*                                                                           *
\*     Changes have been made to make PSS20 Y2K compliant. DATE.LT, DATE.GE  *
\*     and DATE.GT have been used to handle the date comparisons from a      *
\*     function called CMPDATE.                                              *
\*     All code for Chilled Foods active has been removed from the program   *
\*     as it is now redundant.                                               *
\*                                                                           *
\*   Version 1.5       Amy Hoggard                 21st April 2000           *
\*    Changes for Internationalisation to call ADXSERVE to determine how     *
\*    many decimal places are in use.                                        *
\*                                                                           *
\*                                                                           *
\*   Version 1.6       Amy Hoggard                 19th May                  *
\*    Changes for Internationalisation to process new Business Centre        *
\*    Numbers as well as current letters.                                    *
\*    Also changed to call report descriptors from USERDESC in stead of      *
\*    hard coding them so USERDESC file needed for UK and Taiwan             *
\*                                                                           *
\*   Version 1.7       Julia Stones                30th June                 *
\*   Commented out the following change   - Change to categorise all         *
\*   non-alphabetic Business centres encountered to a single BC table record.*
\*   As non-alphabetic Business Centres now used for International processing*
\*                                                                           *
\*   Version 1.8       Brian Greenfield         20th December 2000           *
\*   Re-jigged the logic to determine if the BC letter is valid or not.      *
\*   If the BC letter is not between A & Z or between 1 & 9 it sets it to    *
\*   ASCII value 91 (Don't ask me why, this is how it was before!)           *
\*   This is to stop the program falling over when it comes across a BC      *
\*   letter of SPACE in a CCLAM record, which is itself generated by a       *
\*   manual claim of an item not found on file.                              *
\*                                                                           *
\*   Version 1.9       Jamie Thorpe             24th November 2003           *
\*   Changed CCSMY reporting as part of the Credit Claiming Simplification   *
\*   project. The new style summary report will now be split in to 2 parts.  *
\*   The 1st section will detail any exception claims i.e. Any claims over of*
\*   a high value (as defined on the Software status file). The Claim number *
\*   reason code and value are detailed on the report.                       *
\*   The 2nd section will list each reason code for which a claim has been   *
\*   raised, along with the claim value.                                     *
\*   The new style report is switchable, this is determined by reading the   *
\*   CREDIT CLAIM record on softs Rec.43. If this is set to INACTIVE, then   *
\*   the report will still be created in the original format, otherwise it   *
\*   will be formatted in the new style.                                     *
\*                                                                           *
\*   Version 2.0       Jamie Thorpe             17th August 2004             *
\*   Small change to the report so that amounts BELOW £400 are reconciled    *
\*   rather than amounts ABOVE                                               *
\*                                                                           *
\*   Version 3.0      Julia Stones              24th November 2004           *
\*                                                                           *
\*   Changed all Integer 2 variables to Integer 4 (Store 7 have more than    *
\*   33,000 items to be reported on the detail report                        *
\*                                                                           *
\*   Version 4.1      Brian Greenfield          17th October 2007            *
\*   Changed to run reports on Sunday rather than Monday.                    *
\*                                                                           *
\*   Version 4.2      Harpal Matharu            27th January 2008            *
\*   A9B RETURNS @ POS                                                       *
\*   Changes made to the reports CCDET and CCSMY the store number is now     *
\*   will be added on the header of each page. If the claim is created by    *
\*   TILL the transaction number, till number and operator number will be    *
\*   added.                                                                  *
\*                                                                           *
\*****************************************************************************
\*  %INCLUDE global definitions for;                                         *
\*                                                                           *
\*  utility functions :            APPLICATION.LOG  (01)                     *
\*                                 UPDATE.DATE (02)                          *
\*                                 CALC.BAR.CODE.DISPLAY (07)                *
\*                                 PSDATE (13)                               *
\*                                 SESS.NUM.UTILITY (20)                     *
\*                                 CONV.TO.INTEGER  (26)                     *
\*                                                                           *
\*  file functions    :            IDF, SOFTS, BCSMF                         *
\*                                 CCLAM, CCITF, CCTRL, CCRSN,               *
\*                                 CCSMY, CCDET                              *
\*                                                                           *
\*****************************************************************************

%INCLUDE PSBF01G.J86
%INCLUDE PSBF02G.J86
%INCLUDE PSBF07G.J86
%INCLUDE PSBF13G.J86
%INCLUDE PSBF14G.J86                                                    !1.9JAT
%INCLUDE PSBF20G.J86
%INCLUDE PSBF26G.J86

%INCLUDE IDFDEC.J86                                                     ! BDCN
%INCLUDE SOFTSDEC.J86
%INCLUDE BCSMFDEC.J86
%INCLUDE CCLAMDEC.J86
%INCLUDE CCITFDEC.J86
%INCLUDE CCTRLDEC.J86
%INCLUDE CCRSNDEC.J86
%INCLUDE CCSMYDEC.J86
%INCLUDE CCSTSDEC.J86                                                   ! BDCN
%INCLUDE CCDETDEC.J86
%INCLUDE UDESCDEC.J86   !1.6AH

\*****************************************************************************
\*  Declare GLOBAL program variables                                         *
\*                                                                           *
\*****************************************************************************

STRING GLOBAL                            \
       ADXSERVE.DATA$,                   \ ! 1.5AH
       CURRENT.CODE$,                    \
       DIVIDER$,                         \ ! 1.5AH
       FILE.OPERATION$,                  \
       F20.FUNCTION$,                    \
       F20.STRING$,                      \
       STORE.NUMBER$                       ! 4.2HSM

INTEGER*2 GLOBAL                         \
       CURRENCY.FORMAT%,                 \ ! 1.5AH
       CURRENT.REPORT.NUM%,              \
       DECIMAL.PLACES%,                  \ ! 1.5AH
       F20.INTEGER%


\*****************************************************************************
\*  Declare LOCAL  program variables                                         *
\*                                                                           *
\*  nb. integer*1 program flags do not end in %                              *
\*****************************************************************************

INTEGER*1 GLOBAL                        \program flags
       TRUE,                            \
       FALSE,                           \
       CCDET.OPEN,                      \
       CCLAM.OPEN,                      \
       CCRSN.OPEN,                      \
       CCSMY.OPEN,                      \
       CCSTS.OPEN,                      \                                 BDCN
       CHAR.FOUND,                      \
       CURRENT.REASON.CODE%,            \                              !1.9JAT
       DETAIL.FIRST.PART.FULL,          \
       CREDIT.CLAIM.ACTIVE,             \                              !1.9JAT
       DISPLAY.OUTPUT.ERROR,            \
       EXCEPTION.HEADER.WRITTEN,        \                              !1.9JAT
       EXCEPTION.ON.REPORT,             \                              !1.9JAT
       END.OF.BC,                       \
       END.OF.REPORT,                   \
       ERROR.PROCESSING.ACTIVE,         \
       EXIT.FLAG,                       \
       REPORTING.REQUIRED,              \                              !1.3JAS
       HEADER.PRINTED,                  \                                  DDS
       MATCH.DELIMITER1,                \                              !1.9JAT
       MATCH.DELIMITER2,                \                              !1.9JAT
       PROGRAM.FAIL,                    \
       REASON.CODE.ON.REPORT,           \                              !1.9JAT
       RETURNS.ACTIVE,                  \
       UPDATE.CCTRL.LAST.DATE,          \
       SWAPPED,                         \
       SORT.COMPLETE

INTEGER*4 GLOBAL                        \                     ! 3.0 JAS
       A%,                              \
       ADX.PARM.1%,                     \
       B%,                              \
       INCREMENT%,                      \
       ITEM.NO%,                        \
       L%,                              \
       CLAIMS.COUNTER%,                 \                               1.9JAT
       CCDET.LINE.NO%,                  \
       CCDET.PAGE.NO%,                  \
       CCSMY.LINE.NO%,                  \
       CCSMY.PAGE.NO%,                  \
       CCRSN.ALTERNATE.REASON%,         \                               1.9JAT
       CCRSN.REASON.NUM%,               \                               1.9JAT
       CHAR.POS%,                       \
       HIGHEST.REASON.CODE%,            \                               1.9JAT
       LINES.PER.PAGE%,                 \
       LOG.EVENT.NO%,                   \
       LOG.MESSAGE.NO%,                 \
       MAX.CCLAM.SUBSCRIPT%,            \                               1.2MJK
       PACK.SIZE%,                      \                                 DMJK
       RC%,                             \
       RECORD.NO%,                      \
       RECORDS.PER.SECTOR%,             \
       REPORT.REC.SIZE%,                \
       SECTOR.RECORD.PART%,             \
       SECTOR.SIZE%

INTEGER*4 GLOBAL                        \
       ADX.FUNCTION%,                   \ ! 1.5AH
       ADX.INTEGER%,                    \ ! 1.5AH
       ADX.RETURN.CODE%,                \ ! 1.5AH
       BC.TAB.IDX%,                     \
       CLAIM.EXCEPTION.VALUE%,          \                               1.9JAT
       CLAIM.TOTAL.VALUE%,              \
       CREDIT.TOTAL.VALUE%,             \                               1.9JAT
       CURRENT.CLAIM.VALUE%,            \
       DEL.TAB.IDX%,                    \
       DEL.TAB.NO.ENTRIES%,             \
       ITEM.CLAIM.VALUE%,               \
       ITEM.PRICE%,                     \
       MAX.BC.TABLE.SIZE%,              \
       MAX.REPORT.TABLE.SIZE%,          \
       MAX.DELETE.TABLE.SIZE%,          \
       NEW.CLAIM.VALUE%,                \                               1.9JAT
       NUM.OF.SECTORS%,                 \
       RPT.TAB.IDX%,                    \
       RPT.TAB.NO.ENTRIES%,             \
       SECTOR.NO%,                      \
       TOTAL.CLAIM.VALUE%                                              !1.9JAT

STRING       GLOBAL                     \
       ADJ$,                            \  !1.6AH
       ADX.PARM.2$,                     \
       BCSMF.RECORD$,                   \
       BCSMF.SECTOR$,                   \
       BC.TABLE$(1),                    \
       BLANK.LINE$,                     \
       CANCELLED.LINE$,                 \                                 CMJK
       CCLAM.RECORD$,                   \
       CCLAM.SECTOR$,                   \
       CCDET.PAGE.NO$,                  \
       CCSMY.PAGE.NO$,                  \
       CLAIM.DETAIL$,                   \
       COMPLETED.MESSAGE$,              \
       CURRENCY.SYMBOL$,                \                              !1.9JAT
       DAT$,                            \  !1.6AH
       DELETE.TABLE$(1),                \
       DETAIL.FIRST.PART$,              \
       DETAIL.SECOND.PART$,             \
       DISPLAY.BC$,                     \                                 CMJK
       EOF.CCSTS.FLAG$,                 \                                 BDCN
       FAILED.MESSAGE$,                 \
       FIELD$,                          \
       INVALID.BC$,                     \
       ITEM.BAR.CODE$,                  \
       ITEM.BOOTS.CODE$,                \
       ITEM.CLAIM.VALUE$,               \                                 DMJK
       ITEM.CODE$,                      \
       ITEM.CODE.FLAG$,                 \
       ITEM.PRICE$,                     \
       ITEM.QTY$,                       \
       ITEM.TABLE$(1),                  \
       LAST.CREDIT.RUN.DATE$,           \
       LOCATION$,                       \                              !1.9JAT
       LOG.FAILED.MESSAGE$,             \
       LOG.STRING.1.UNIQUE$,            \
       LOG.STRING.2$,                   \
       NUM.OF.PAGES$,                   \
       NUMB$,                           \  !1.6AH
       OUTPUT.MESSAGE$,                 \
       PACK.SIZE$,                      \                                 DMJK
       PAGE.NUM.MARKER$,                \
       PAGE.THROW$,                     \
       PRINT.REPORT.NO$,                \
       PRINT.TIME$,                     \
       PRINT.TODAYS.DATE$,              \
       PRINT.WC.DATE$,                  \
       REPORT.TABLE$(1),                \
       ROI.CLAIM.EXCEPTION.VALUE$,      \                              !1.9JAT
       SECTOR.FILLER$,                  \
       NEW.CLAIM.RECORDS$(1),           \                               1.9JAT
       NEW.REASON.CODE.RECORDS$(1),     \                               1.9JAT
       SIX.DAYS.DATE$,                  \                               1.3JAS
       START$,                          \  !1.6AH
       STARTUP.MESSAGE$,                \
       STORE.BC.LETTER$,                \
       SUPPLY.ROUTE.DESC$,              \
       TEMP$,                           \
       TITLE$,                          \  !1.6AH
       TODAYS.DATE$,                    \
       TRAINING.BC$,                    \
       UK.CLAIM.EXCEPTION.VALUE$,       \                               1.9JAT
       UNIT$,                           \  !1.6AH
       VALUE$,                          \
       VALUE.NO.SPACES$,                \
       WC.DATE$,                        \
       DUMMY$,                          \
       CURRENT.TILL.CLAIM$                                              !4.2HSM
     

REAL                                    \
       GAP,                             \
       REAL.ITEM.CLAIM.VALUE,           \                                 DMJK
       REAL.CCITF.QTY,                  \                                 DMJK
       REAL.PACK.SIZE                   !                                 DMJK

\*****************************************************************************
\* %INCLUDE external function definitions for;                               *
\*                                                                           *
\* utility functions :                    ADXSERVE                           *
\*                                        CONV.TO.INTEGER                    *
\*                                        APPLICATION.LOG (01)               *
\*                                        UPDATE.DATE (02)                   *
\*                                        CALC.BAR.CODE.DISPLAY (07)         *
\*                                        PSDATE (13)                        *
\*                                        SESS.NUM.UTILITY (20)              *
\*                                        STANDARD.ERROR.DETECTED(24)        *
\*                                        CONV.TO.INTEGER(26)                *
\*                                                                           *
\* file functions    :                    IDF, SOFTS, BCSMF                  *
\*                                        CCLAM, CCITF, CCTRL, CCRSN,        *
\*                                        CCSMY, CCDET, CCSTS, CMPDATE       *
\*                                                                           *
\*****************************************************************************

%INCLUDE ADXSERVE.J86
%INCLUDE PSBF01E.J86
%INCLUDE PSBF02E.J86
%INCLUDE PSBF07E.J86
%INCLUDE PSBF13E.J86
%INCLUDE PSBF14E.J86                                                    !1.9JAT
%INCLUDE PSBF20E.J86
%INCLUDE PSBF24E.J86
%INCLUDE PSBF26E.J86

%INCLUDE IDFEXT.J86                                                     ! BDCN
%INCLUDE SOFTSEXT.J86
%INCLUDE BCSMFEXT.J86
%INCLUDE CCLAMEXT.J86
%INCLUDE CCITFEXT.J86
%INCLUDE CCTRLEXT.J86
%INCLUDE CCRSNEXT.J86
%INCLUDE CCSMYEXT.J86
%INCLUDE CCDETEXT.J86
%INCLUDE CCSTSEXT.J86                                                   ! BDCN
%INCLUDE CMPDATE.J86                                                    !1.4RD
%INCLUDE UDESCEXT.J86  !1.6AH

\*****************************************************************************
\*****************************************************************************
\* PROGRAM.START:                                                            *
\*                                                                           *
\*****************************************************************************

PROGRAM.START:

       ON ERROR GOTO ERROR.DETECTED
 
       GOSUB PROGRAM.INITIALISE
	 
       GOSUB MAIN.PROCESS
	  
       GOSUB PROGRAM.FINALISE
	  
       GOTO PROGRAM.EXIT

\*****************************************************************************
\*****************************************************************************
\* PROGRAM.INITIALISE:                                                       *
\*       Assign initial variable values;                                     *
\*              initialise flags                                             *
\*              assign messages/strings                                      *
\*                                                                           *
\*       display startup message                                             *
\*       call SET file functions : IDF, SOFTS, BCSMF                         *
\*                                 CCLAM, CCITF, CCTRL, CCRSN,               *
\*                                 CCSMY, CCDET, CCSTS                       *
\*       gosub ALLOCATE.SESSION.NUMBERS                                      *
\*       gosub OPEN.FILES                                                    *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
PROGRAM.INITIALISE:

       FALSE = 0
       TRUE = 1
       EXIT.FLAG = FALSE
        EOF.CCSTS.FLAG$ = "N"                                           ! BDCN

       CALL UDESC.SET                                            !1.6AH
       GOSUB PROCESS.UDESC                                       !1.6AH

       CALL CCLAM.SET                                                  !1.2MJK
       MAX.BC.TABLE.SIZE% = 43                                  !1.6AH !1.2MJK
       SECTOR.SIZE% = 512                                              !1.2MJK
       SECTOR.RECORD.PART% = 508                                       !1.2MJK
                                                                       !1.2MJK
       NUM.OF.SECTORS%      = (SIZE(CCLAM.FILE.NAME$)/SECTOR.SIZE%)-1  !1.2MJK
       RECORDS.PER.SECTOR%  = (SECTOR.RECORD.PART%/CCLAM.RECL%)        !1.2MJK
       MAX.CCLAM.SUBSCRIPT% = NUM.OF.SECTORS%*RECORDS.PER.SECTOR%      !1.2MJK
                                                                       !1.2MJK
       MAX.REPORT.TABLE.SIZE% = MAX.CCLAM.SUBSCRIPT%                   !1.2MJK
       MAX.DELETE.TABLE.SIZE% = MAX.CCLAM.SUBSCRIPT%                   !1.2MJK

       REPORT.REC.SIZE% = 84

       CLAIMS.COUNTER% = 1                                             !1.9JAT
       EXCEPTION.HEADER.WRITTEN = FALSE                                !1.9JAT
       CCRSN.OPEN = FALSE                                              !1.9JAT
       EXCEPTION.ON.REPORT = FALSE                                     !1.9JAT
       REASON.CODE.ON.REPORT = FALSE                                   !1.9JAT
       HIGHEST.REASON.CODE% = 0                                        !1.9JAT

       BLANK.LINE$         = STRING$(75," ")
       PAGE.THROW$         = STRING$(79," ") + CHR$(12)
       STARTUP.MESSAGE$    = START$                                     !1.6AH
       COMPLETED.MESSAGE$  = "PSS20 completed successfully."
       FAILED.MESSAGE$     = "PSS20 has failed. Check Application event log."
       LOG.FAILED.MESSAGE$ = "Application Event Log has failed."
       PRINT.REPORT.NO$    = " BR0020    "
       INVALID.BC$         = "UNKNOWN BC    "                           ! CMJK
       TRAINING.BC$        = "TRAINING BC   "
       PAGE.NUM.MARKER$    = ">><<"
       CANCELLED.LINE$     = STRING$(4," ") +                           \ CMJK
                             "+  +  +  +    C A N C E L L E D    " +    \ CMJK
                             ADJ$ +   "  +  +  +  +" +                  \ CMJK    !1.6AH
                           STRING$(29," ")                              ! CMJK

       OUTPUT.MESSAGE$ = STARTUP.MESSAGE$
       GOSUB DISPLAY.OUTPUT.MESSAGE

       CALL IDF.SET
       CALL SOFTS.SET
       CALL BCSMF.SET
       CALL CCITF.SET
       CALL CCTRL.SET
       CALL CCRSN.SET
       CALL CCSMY.SET
       CALL CCDET.SET
       CALL CCSTS.SET                                                   ! BDCN

       ADX.FUNCTION% = 4                                       ! 1.5AH
       ADX.INTEGER% = 0                                        ! 1.5AH

       CALL ADXSERVE(ADX.RETURN.CODE%,    \                    ! 1.5AH
                     ADX.FUNCTION%,       \                    ! 1.5AH
                     ADX.INTEGER%,        \                    ! 1.5AH
                     ADXSERVE.DATA$)                           ! 1.5AH

       CURRENCY.FORMAT% = VAL(MID$(ADXSERVE.DATA$,7,1))        ! 1.5AH
       DECIMAL.PLACES%  = VAL(MID$(ADXSERVE.DATA$,23,1))       ! 1.5AH
       STORE.NUMBER$    = " STORE - " +    \                   ! 4.2HSM
                          MID$(ADXSERVE.DATA$,1,4)             ! 4.2HSM
       
       IF CURRENCY.FORMAT% = 1 THEN BEGIN                      ! 1.5AH
          DIVIDER$ = "."                                       ! 1.5AH
       ENDIF ELSE BEGIN                                        ! 1.5AH
          DIVIDER$ = ","                                       ! 1.5AH
       ENDIF                                                   ! 1.5AH

       GOSUB ALLOCATE.SESSION.NUMBERS
       GOSUB OPEN.FILES
RETURN

\*****************************************************************************
\*****************************************************************************
\* MAIN.PROCESS:                                                             *
\*       gosub EVALUATE.SOFTWARE.STATUS                                      *
\*       If Returns is active                                                *
\*               gosub EVALUATE.REPORTING.REQUIRED                     !1.3JAS
\*                             gosub INIT.DESPATCH.6.DAY.DATE          !1.3JAS
\*                             gosub SETUP.BC.TABLE   (BC = Business Centre) *
\*                             gosub EXTRACT.CCSTS.CLAIMS                    *
\*                             gosub SETUP.CLAIMS.TABLES                     *
\*                             gosub UPDATE.CCSTS.FILE                       *
\*                             gosub CLAIMS.HOUSEKEEPING               !1.3JAS
\*                             if Reporting required                   !1.3JAS
\*                                gosub RET.CLAIMS.REPORTING           !1.3JAS
\*                             endif                                   !1.3JAS
\*        endif                                                              *
\*        gosub UPDATE.CCTRL                                                 *
\*       set Exit Flag                                                       *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
MAIN.PROCESS:
    
    WHILE EXIT.FLAG = FALSE
       GOSUB EVALUATE.SOFTWARE.STATUS
       IF RETURNS.ACTIVE THEN BEGIN
	          
              GOSUB EVALUATE.REPORTING.REQUIRED                        !1.3JAS
			  
                GOSUB INIT.DESPATCH.6.DAY.DATE                         !1.3JAS
				 
                  GOSUB SETUP.BC.TABLE                                 !1.3JAS
				 
                    GOSUB EXTRACT.CCSTS.CLAIMS                          ! BDCN
					 
                    GOSUB SETUP.CLAIMS.TABLES
					 
                    GOSUB UPDATE.CCSTS.FILE                             ! BDCN
					 
                    GOSUB CLAIMS.HOUSEKEEPING                          !1.3JAS
					 
                    IF REPORTING.REQUIRED THEN BEGIN                   !1.3JAS
					    
                       GOSUB RET.CLAIMS.REPORTING                      !1.3JAS
					    
                    ENDIF                                              !1.3JAS
       ENDIF
	   
       GOSUB UPDATE.CCTRL
	  
       EXIT.FLAG = TRUE
    WEND
RETURN

\*****************************************************************************
\*****************************************************************************
\* PROGRAM.FINALISE:                                                         *
\*       gosub CLOSE.FILES                                                   *
\*       gosub DEALLOCATE.SESSION.NUMBERS                                    *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
PROGRAM.FINALISE:
       GOSUB CLOSE.FILES
       GOSUB DEALLOCATE.SESSION.NUMBERS
RETURN

\*****************************************************************************
\*****************************************************************************
\* PROGRAM.EXIT:                                                             *
\*       If not display output error condition                               *
\*              display appropriate success or fail message                  *
\*        endif                                                              *
\*       Terminate program execution                                         *
\*                                                                           *
\*****************************************************************************
PROGRAM.EXIT:
       IF NOT DISPLAY.OUTPUT.ERROR THEN BEGIN
              IF PROGRAM.FAIL THEN BEGIN
                         OUTPUT.MESSAGE$ = FAILED.MESSAGE$
              ENDIF ELSE BEGIN
                     OUTPUT.MESSAGE$ = COMPLETED.MESSAGE$
              ENDIF
       ENDIF
       GOSUB DISPLAY.OUTPUT.MESSAGE

       STOP

\*****************************************************************************
\*****************************************************************************
\* EVALUATE.SOFTWARE.STATUS:                                                 *
\*        Read SOFTS record 5                                                *
\*        set Returns Active flag as appropriate                             *
\*        Read SOFTS record 19                                               *
\*        This is used to deterine which value to use for exceptional        *
\*        reporting.                                                         *
\*        Read SOFTS record 43, Set CREDIT CLAIM ACTIVE flag as appropriate  *
\*        Set Exception values as appropriate.                               *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
EVALUATE.SOFTWARE.STATUS:
       SOFTS.REC.NUM% = 5
       RC% = READ.SOFTS
       IF RC% <> 0 THEN BEGIN
          GOSUB READ.FILE.ERROR
       ENDIF
       IF SOFTS.RECORD$ = "RETURNS IS ACTIVE" THEN BEGIN
          RETURNS.ACTIVE = TRUE
       ENDIF

       SOFTS.REC.NUM% = 19                                                          !1.9JAT
       RC% = READ.SOFTS                                                             !1.9JAT
       IF RC% <> 0 THEN BEGIN                                                       !1.9JAT
          GOSUB READ.FILE.ERROR                                                     !1.9JAT
       ENDIF                                                                        !1.9JAT
       IF LEFT$(SOFTS.RECORD$,20) = "EIRE STORE IS ACTIVE" THEN BEGIN               !1.9JAT
          LOCATION$ = "EIRE"                                                        !1.9JAT
       ENDIF ELSE BEGIN                                                             !1.9JAT
          LOCATION$ = "UK"                                                          !1.9JAT
       ENDIF                                                                        !1.9JAT

       CREDIT.CLAIM.ACTIVE = FALSE                                                  !1.9JAT
       SOFTS.REC.NUM% = 43                                                          !1.9JAT
       RC% = READ.SOFTS                                                             !1.9JAT
       IF RC% <> 0 THEN BEGIN                                                       !1.9JAT
          GOSUB READ.FILE.ERROR                                                     !1.9JAT
       ENDIF                                                                        !1.9JAT
       IF LEFT$(SOFTS.RECORD$,22) = "CREDIT CLAIM IS ACTIVE" THEN BEGIN             !1.9JAT
          CREDIT.CLAIM.ACTIVE = TRUE                                                !1.9JAT

           MATCH.DELIMITER1 = MATCH(",",SOFTS.RECORD$,26)                           !1.9JAT
           MATCH.DELIMITER2 = MATCH(",",SOFTS.RECORD$,(MATCH.DELIMITER1 + 1))       !1.9JAT
           UK.CLAIM.EXCEPTION.VALUE$= MID$(SOFTS.RECORD$,26,(MATCH.DELIMITER1 - 26))!1.9JAT
           ROI.CLAIM.EXCEPTION.VALUE$ = MID$(SOFTS.RECORD$,(MATCH.DELIMITER1 + 1), \!1.9JAT
                                         (MATCH.DELIMITER2 - (MATCH.DELIMITER1 +1)))!1.9JAT

           IF LOCATION$ = "EIRE" THEN BEGIN                                         !1.9JAT
              CLAIM.EXCEPTION.VALUE% = VAL(ROI.CLAIM.EXCEPTION.VALUE$)              !1.9JAT
              CURRENCY.SYMBOL$ = "EUR "                                             !1.9JAT
           ENDIF ELSE BEGIN                                                         !1.9JAT
              CLAIM.EXCEPTION.VALUE% = VAL(UK.CLAIM.EXCEPTION.VALUE$)               !1.9JAT
              CURRENCY.SYMBOL$ = "GBP "                                             !1.9JAT
           ENDIF                                                                    !1.9JAT
       ENDIF
RETURN



\*****************************************************************************
\*****************************************************************************
\* EVALUATE.REPORTING.REQUIRED:                                        !1.3JAS
\*        Read CCTRL file for Last Credit Run Date                           *
\*        If today is a Sunday                                               *      !4.1BMG
\*           set Reporting Required to true                            !1.3JAS
\*        else                                                               *
\*           If last Credit Run Date < last Sunday's date               !1.4RD      !4.1BMG
\*              set Reporting Required to true                         !1.3JAS
\*           endif                                                           *
\*        endif                                                              *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
EVALUATE.REPORTING.REQUIRED:                                           !1.3JAS
       CCTRL.REC.NUM% = 1
       RC% = READ.CCTRL
       IF RC% <> 0 THEN BEGIN
              GOSUB READ.FILE.ERROR
       ENDIF
       LAST.CREDIT.RUN.DATE$ = UNPACK$(CCTRL.CREDIT.RPT.RUN.DATE$)

       TODAYS.DATE$ = DATE$
       RC% = PSDATE(TODAYS.DATE$)
       IF RC% <> 0 THEN BEGIN
              OUTPUT.MESSAGE$ = "System Date is invalid."
              GOSUB DISPLAY.OUTPUT.MESSAGE
              PROGRAM.FAIL = TRUE
              GOTO PROGRAM.EXIT
       ENDIF
       IF F13.DAY$ = "SUN" THEN BEGIN                                  !4.1BMG
              REPORTING.REQUIRED = TRUE                                !1.3JAS
       ENDIF ELSE BEGIN

              F02.DATE$ = TODAYS.DATE$
              INCREMENT% = -1
              WHILE F13.DAY$ <> "SUN"                                  !4.1BMG
                     RC% = UPDATE.DATE(INCREMENT%)
                     IF RC% <> 0 THEN BEGIN
                            PROGRAM.FAIL = TRUE
                            GOTO PROGRAM.EXIT
                     ENDIF
                     RC% = PSDATE(F02.DATE$)
                     IF RC% <> 0 THEN BEGIN
                            PROGRAM.FAIL = TRUE
                            GOTO PROGRAM.EXIT
                     ENDIF
              WEND
              IF DATE.LT(LAST.CREDIT.RUN.DATE$,F02.DATE$) THEN BEGIN    !1.4RD
                     REPORTING.REQUIRED = TRUE                         !1.3JAS
              ENDIF
       ENDIF

RETURN

\*****************************************************************************
\*****************************************************************************
\***                                                                         *
\***    PROCESS.UDESC:                                                       *
\***                                                                         *
\***    Allocate session number                                              *
\***    Read in report descriptors                                           *
\***                                                                         *
\***                                                                         *
\*****************************************************************************

PROCESS.UDESC:                                                     !1.6AH

      F20.FUNCTION$ = "O"                                          !1.6AH
      F20.INTEGER%  = UDESC.REPORT.NUM%                            !1.6AH
      F20.STRING$   = UDESC.FILE.NAME$                             !1.6AH
      GOSUB CALL.SESS.NUM.UTILITY                                  !1.6AH
      UDESC.SESS.NUM% = F20.INTEGER.FILE.NO%                       !1.6AH

      OPEN "USERDESC" RECL 49 AS UDESC.SESS.NUM% NOWRITE NODEL     !1.6AH

      UDESC.RECORD.NUM% = 1                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      START$ = UDESC.RECORD$                                       !1.6AH

      UDESC.RECORD.NUM% = 2                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      ADJ$ = UDESC.RECORD$                                         !1.6AH

      UDESC.RECORD.NUM% = 3                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      NUMB$ = UDESC.RECORD$                                        !1.6AH

      UDESC.RECORD.NUM% = 4                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      TITLE$ = UDESC.RECORD$                                       !1.6AH

      UDESC.RECORD.NUM% = 5                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      UNIT$ = UDESC.RECORD$                                        !1.6AH

      UDESC.RECORD.NUM% = 6                                        !1.6AH
      CALL READ.UDESC                                              !1.6AH
      DAT$ = UDESC.RECORD$                                         !1.6AH


RETURN                                                             !1.6AH

\*****************************************************************************
\*****************************************************************************
\* SETUP.BC.TABLE:                                                           *
\*         The Business Centre (BC) Table holds the Business Centre letter   *
\*         Code, Name, BCSMF sequence number and credit claim number.        *
\*         The table has 26 entries and records are placed in the table      *
\*         position corresponding to the ASCII number of the letter code     *
\*         (A to Z being 1 to 26, ASC(A)-64 = 65-64 = 1 etc)                 *
\*         Each entry is initialised with name "UNKNOWN BC" and inc sequence *
\*         no."30" along with letter codes A to Z, then entries corresponding*
\*         to the BCSMF are updated with valid data, the last but one entry) *
\*         (Z) is set to "TRAINING BC " and sequence no. "99";               *
\*                                                                           *
\*         dimension BC Table                                                *
\*         initialise each entry in BC Table (seq.no = 0, 'INVALID BC')      *
\*         initialise 2nd to last (Z) entry in BC Table (seq.no=0,'TRAINING  *
\*         BC') calculate number of sectors and no. of records per sector    *
\*         for BCSMF read direct BCSMF file sector by sector, record by      *
\*         record, until all sectors have been read                          *
\*         for each Business Centre Record;                                  *
\*         update the BC Table entry corresponding to the BC letter code     *
\*         with the valid name and sequence number                           *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
SETUP.BC.TABLE:
        DIM BC.TABLE$ (MAX.BC.TABLE.SIZE%)
       FOR BC.TAB.IDX% = 1 TO (MAX.BC.TABLE.SIZE%)
          IF BC.TAB.IDX% > 9 AND BC.TAB.IDX% < 17 THEN BEGIN            !1.6AH
             BC.TABLE$(BC.TAB.IDX%) = STRING$(25," ")                   !1.6AH! CMJK
          ENDIF ELSE BEGIN                                              !1.6AH
            BC.TABLE$(BC.TAB.IDX%) = CHR$(48 + BC.TAB.IDX%) +        \  !1.6AH
                                     INVALID.BC$ +                   \
                                 RIGHT$("00"+STR$(29+BC.TAB.IDX%),2) \   CMJK
                                 + "00000000"                           ! CMJK
          ENDIF
       NEXT BC.TAB.IDX%

       BC.TAB.IDX% = MAX.BC.TABLE.SIZE%-1                               ! CMJK
        BC.TABLE$(BC.TAB.IDX%) = CHR$(48 + BC.TAB.IDX%) +                    \
                               TRAINING.BC$ +                                \
                             "99" + "00000000"                          ! BDCN

       NUM.OF.SECTORS% = SIZE(BCSMF.FILE.NAME$) / SECTOR.SIZE%
       RECORDS.PER.SECTOR% = SECTOR.RECORD.PART% / BCSMF.RECL%

       FOR SECTOR.NO% = 2 TO NUM.OF.SECTORS%
           GOSUB READ.BCSMF.SECTOR
           FOR RECORD.NO% = 1 TO RECORDS.PER.SECTOR%
               BCSMF.RECORD$ = MID$(BCSMF.SECTOR$,                           \
                                ((RECORD.NO% - 1)*BCSMF.RECL%) + 1,          \
                                 BCSMF.RECL%)
              IF LEFT$(BCSMF.RECORD$,4) = PACK$(STRING$(4,"00")) THEN        \
                  BEGIN
                  GOTO NEXT.SECT
              ENDIF ELSE BEGIN
                  GOSUB PROCESS.BCSMF.RECORD
              ENDIF
           NEXT RECORD.NO%
       NEXT.SECT:
       NEXT SECTOR.NO%
RETURN

\*****************************************************************************
\*****************************************************************************
\* PROCESS.BCSMF.RECORD:                                                     *
\*     If BC is not Training BC ("Z")                                        *
\*     and not Pseudo Business Center (flag not = "Y")                       *
\*         save BC letter Code,Name and Sequence No. to corresponding        *
\*         BC Table entry and initialise claim number                        *
\*     endif                                                                 *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
PROCESS.BCSMF.RECORD:
    IF  (LEFT$(BCSMF.RECORD$,1) <> "Z")                                      \
    AND (MID$(BCSMF.RECORD$,29,1) <> "Y") THEN BEGIN                    ! BDCN
        BC.TAB.IDX% = ASC(LEFT$(BCSMF.RECORD$,1)) - 48
        BC.TABLE$(BC.TAB.IDX%) =                                             \
                 LEFT$(BCSMF.RECORD$,1) +                                    \
                   MID$(BCSMF.RECORD$,2,14) +                                \
                RIGHT$("00" + STR$(ASC(MID$(BCSMF.RECORD$,28,1))),2) +       \
                 "00000000"                                             ! BDCN
    ENDIF
RETURN




\*****************************************************************************
\***                                                                         *
\***    EXTRACT.CCSTS.CLAIMS:                                                *
\***                                                                         *
\***     Update BC table with current claim numbers from CCSTS records       *
\***                                                                         *
\*****************************************************************************

    EXTRACT.CCSTS.CLAIMS:

    CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%                             ! BDCN
    IF END #CCSTS.SESS.NUM% THEN OPEN.FILE.ERROR                        ! BDCN
    OPEN CCSTS.FILE.NAME$ DIRECT RECL CCSTS.RECL% AS CCSTS.SESS.NUM%    ! BDCN

    CCSTS.REC.NUM% = 1

    WHILE EOF.CCSTS.FLAG$ = "N"
       CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
       RC% = READ.CCSTS
       IF RC% <> 0 THEN GOTO CCSTS.READ.FAIL
                                                                               
       BC.TAB.IDX% = ASC(CCSTS.FSI$) - 48                              !1.6AH
       IF BC.TAB.IDX% > 0 AND                                          \ BDCN
          BC.TAB.IDX% <= MAX.BC.TABLE.SIZE% THEN BEGIN                 ! BDCN

              BC.TABLE$(BC.TAB.IDX%)  =                                \ BDCN
                   LEFT$(BC.TABLE$(BC.TAB.IDX%),17) +                  \ BDCN
                   RIGHT$( STRING$(8,"0") + CCSTS.CLAIM$, 8)           ! BDCN

       ENDIF                                                           ! BDCN
       CCSTS.REC.NUM% = CCSTS.REC.NUM% + 1                             ! BDCN

       RETURN.CCSTS.READ.FAIL:

    WEND

    RETURN


\*****************************************************************************
\*****************************************************************************
\* SETUP.CLAIMS.TABLES:                                                      *
\*     calculate number of sectors and no. of records per sector for CCLAM   *
\*     dimension tables and initialise table indices                         *
\*     open CCLAM in Direct mode                                             *
\*     read CCLAM file sector by sector, record by record until all sectors  *
\*     have been read                                                        *
\*     for each record gosub PROCESS.CCLAM.RECORD                            *
\*     close CCLAM file                                                      *
\*     store table number of entries ( = table indices -1 )                  *
\*     if Returns is Active                                                  *
\*         gosub SORT.REPORT.TABLE (sorts by BC sequence no., claim no.)     *
\*     endif                                                                 *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
SETUP.CLAIMS.TABLES:
       NUM.OF.SECTORS% = SIZE(CCLAM.FILE.NAME$) / SECTOR.SIZE%
       RECORDS.PER.SECTOR% = SECTOR.RECORD.PART% / CCLAM.RECL%

       DIM REPORT.TABLE$ (MAX.REPORT.TABLE.SIZE%)
       DIM DELETE.TABLE$ (MAX.DELETE.TABLE.SIZE%)
       DIM NEW.CLAIM.RECORDS$(MAX.REPORT.TABLE.SIZE%)             !1.9JAT
       DIM NEW.REASON.CODE.RECORDS$(100)                          !1.9JAT

       RPT.TAB.IDX% = 1
       DEL.TAB.IDX% = 1

       GOSUB OPEN.CCLAM.DIRECT

       FOR SECTOR.NO% = 2 TO NUM.OF.SECTORS%
           GOSUB READ.CCLAM.SECTOR
           FOR RECORD.NO% = 1 TO RECORDS.PER.SECTOR%
                     CCLAM.RECORD$ = MID$ (CCLAM.SECTOR$,                    \
                                   ((RECORD.NO% - 1)*CCLAM.RECL%)+ 1,        \
                                   CCLAM.RECL%)
              IF LEFT$(CCLAM.RECORD$,4) = PACK$(STRING$(4,"00")) THEN        \
                  BEGIN
                  GOTO NEXT.SECTOR
                  ENDIF ELSE BEGIN
                  GOSUB PROCESS.CCLAM.RECORD
              ENDIF
           NEXT RECORD.NO%
       NEXT.SECTOR:
       NEXT SECTOR.NO%

       CLOSE CCLAM.SESS.NUM%
       CCLAM.OPEN = FALSE

       RPT.TAB.NO.ENTRIES% = RPT.TAB.IDX% - 1
       DEL.TAB.NO.ENTRIES% = DEL.TAB.IDX% - 1

       IF RETURNS.ACTIVE THEN BEGIN
              GOSUB SORT.REPORT.TABLE
       ENDIF
RETURN

\*****************************************************************************  !1.3JAS
\*****************************************************************************  !1.3JAS
\* INIT.DESPATCH.6.DAY.DATE:                                           !1.3JAS
\*     To check if the date of despatch was > than 6 days ago          !1.3JAS
\*                                                                     !1.3JAS
\* RETURN                                                              !1.3JAS
\*****************************************************************************

INIT.DESPATCH.6.DAY.DATE:                                              !1.3JAS
     F02.DATE$ = TODAYS.DATE$                                          !1.3JAS
     INCREMENT% = -6                                                   !1.3JAS
     RC% = UPDATE.DATE(INCREMENT%)                                     !1.3JAS
     IF RC% <> 0 THEN BEGIN                                            !1.3JAS
        PROGRAM.FAIL = TRUE                                            !1.3JAS
        GOTO PROGRAM.EXIT                                              !1.3JAS
     ENDIF                                                             !1.3JAS
     SIX.DAYS.DATE$ = F02.DATE$                                        !1.3JAS
RETURN                                                                 !1.3JAS

\*****************************************************************************
\*****************************************************************************
\* PROCESS.CCLAM.RECORD:                                                     *
\*      move CCLAM record data into CCLAM fields                             *
\*      if returns is active                                                 *
\*          Calculate BC.Table index from CCLAM BC letter                    *
\*          If CCLAM credit claim number > BC table entry claim number       *
\*              save CCLAM credit claim no. in BC table                      *
\*          endif                                                            *
\*          if Reporting required and not a cancelled UOD              !1.3JAS
\*              gosub RET.CLAIM.TO.RPT.TABLE                                 *
\*          endif                                                            *
\*       endif                                                               *
\*       if older than six days AND CCLAM Retrieval Flag = "N"          !1.4RD
\*       and it has previously been reported                                 *
\*            gosub SAVE.CLAIM.TO.DELETE.TABLE                               *
\*       endif                                                               *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
PROCESS.CCLAM.RECORD:
       CCLAM.CREDIT.CLAIM.NUM$ = MID$(CCLAM.RECORD$,1,4)
       CCLAM.NUM.OF.ITEMS%     = CONV.TO.INTEGER(MID$(CCLAM.RECORD$,12,2)) !BDCN
       CCLAM.BC.LETTER$        = MID$(CCLAM.RECORD$,16,1)
       CCLAM.AUTHORISATION$    = MID$(CCLAM.RECORD$,25,15)
       CCLAM.FILLER$           = MID$(CCLAM.RECORD$,154,7)                            !4.2HSM

!       IF ASC(CCLAM.BC.LETTER$)<65 OR ASC(CCLAM.BC.LETTER$)>90 THEN     \CMJK  1.7JAS
!          CCLAM.BC.LETTER$ = CHR$(91)                                    !CMJK  1.7JAS

       IF ASC(CCLAM.BC.LETTER$) > 90 THEN BEGIN                                       !1.8BMG
          CCLAM.BC.LETTER$ = CHR$(91)                                                 !1.8BMG
       ENDIF ELSE BEGIN                                                               !1.8BMG
          IF ASC(CCLAM.BC.LETTER$) < 65 THEN BEGIN                                    !1.8BMG
             IF ASC(CCLAM.BC.LETTER$) < 49 OR ASC(CCLAM.BC.LETTER$) > 57 THEN BEGIN   !1.8BMG
                CCLAM.BC.LETTER$ = CHR$(91)                                           !1.8BMG
             ENDIF                                                                    !1.8BMG
          ENDIF                                                                       !1.8BMG
       ENDIF                                                                          !1.8BMG


       CCLAM.REASON.NUM$        = MID$(CCLAM.RECORD$,65,1)
       CCLAM.DATE.OF.CLAIM$     = MID$(CCLAM.RECORD$,145,3)
       CCLAM.RETRIEVAL.FLAG$    = MID$(CCLAM.RECORD$,151,1)
       CCLAM.CF.RPT.MARKER$     = MID$(CCLAM.RECORD$,152,1)
       CCLAM.CANC.MARKER$       = MID$(CCLAM.RECORD$,153,1)              !CMJK


       IF RETURNS.ACTIVE THEN BEGIN
            BC.TAB.IDX%  = ASC(CCLAM.BC.LETTER$) - 48                   ! BDCN
            IF BC.TAB.IDX% > 0 AND                                      \ BDCN
               BC.TAB.IDX% <= MAX.BC.TABLE.SIZE% THEN BEGIN             ! BDCN
               IF VAL(UNPACK$(CCLAM.CREDIT.CLAIM.NUM$)) >               \ BDCN
                  VAL(MID$(BC.TABLE$(BC.TAB.IDX%),18,8)) THEN BEGIN     ! BDCN

                  BC.TABLE$(BC.TAB.IDX%)  =                             \ BDCN
                   LEFT$(BC.TABLE$(BC.TAB.IDX%),17) +                   \ BDCN
                   RIGHT$( STRING$(8,"0") +                             \
                   UNPACK$(CCLAM.CREDIT.CLAIM.NUM$),8 )                 ! BDCN

               ENDIF                                                    ! BDCN
            ENDIF                                                       ! BDCN
                                                                        ! BDCN
            IF REPORTING.REQUIRED AND                                  \1.3JAS
             DATE.GE (UNPACK$(CCLAM.DATE.OF.CLAIM$),                    \1.4RD
                      LAST.CREDIT.RUN.DATE$) AND                        \1.4RD
             CCLAM.CANC.MARKER$ <> "Y" AND                              \1.3JAS
             CCLAM.RETRIEVAL.FLAG$ <> "Z" THEN BEGIN                    !1.1MJK
             GOSUB RET.CLAIM.TO.REPORT.TABLE
             IF CREDIT.CLAIM.ACTIVE = TRUE THEN BEGIN                   !1.9JAT
                GOSUB UPDATE.CLAIMS.TABLE                               !1.9JAT
             ENDIF                                                      !1.9JAT
            ENDIF                                                       !1.9JAT

       ENDIF
          IF CCLAM.RETRIEVAL.FLAG$ = "N" AND                           \1.3JAS
           DATE.LT (UNPACK$(CCLAM.DATE.OF.CLAIM$),SIX.DAYS.DATE$) AND   \1.4RD
             DATE.LT (UNPACK$(CCLAM.DATE.OF.CLAIM$),                    \1.4RD
                      LAST.CREDIT.RUN.DATE$) THEN BEGIN                 !1.4RD
              GOSUB SAVE.CLAIM.TO.DELETE.TABLE
       ENDIF
RETURN

\*****************************************************************************
\***                                                                         *
\***    UPDATE.CCSTS.FILE:                                                   *
\***                                                                         *
\***     For each BC table entry                                             *
\***        If a valid BC entry and not the Training BC                      *
\***           and BC table entry claim no. is not zero                      *
\***           Read CCSTS locked                                             *
\***           Format output record from table entry                         *
\***           Write CCSTS unlock                                            *
\***        endif                                                            *
\***     next entry                                                          *
\***                                                                         *
\***     close CCSTS file                                                    *
\***                                                                         *
\*****************************************************************************

    UPDATE.CCSTS.FILE:

      FOR BC.TAB.IDX% = 1 TO MAX.BC.TABLE.SIZE%                         ! BDCN

        IF MID$(BC.TABLE$(BC.TAB.IDX%),2,14) <> INVALID.BC$ AND              \
           MID$(BC.TABLE$(BC.TAB.IDX%),2,14) <> TRAINING.BC$ AND             \
           MID$(BC.TABLE$(BC.TAB.IDX%),18,8) <> "00000000" THEN BEGIN

           CCSTS.REC.NUM% = BC.TAB.IDX%                                 ! BDCN
           CURRENT.REPORT.NUM% = CCSTS.REPORT.NUM%
           RC% = READ.CCSTS.LOCKED                                      ! BDCN
           IF RC% <> 0 THEN GOTO READ.FILE.ERROR                        ! BDCN

           CCSTS.FSI$     = LEFT$(BC.TABLE$(BC.TAB.IDX%),1)             ! BDCN
           CCSTS.NAME$    = MID$(BC.TABLE$(BC.TAB.IDX%),2,14)           ! BDCN
           CCSTS.CLAIM$   = MID$(BC.TABLE$(BC.TAB.IDX%),18,8)           ! BDCN
           CCSTS.FILLER$  = " "                                         ! BDCN

           RC% = WRITE.UNLOCK.CCSTS                                     ! BDCN
           IF RC% <> 0 THEN GOTO WRITE.FILE.ERROR                       ! BDCN
        ENDIF                                                           ! BDCN

      NEXT BC.TAB.IDX%

      CLOSE CCSTS.SESS.NUM%

    RETURN


\*****************************************************************************
\*****************************************************************************
\* RET.CLAIM.TO.REPORT.TABLE:                                                *
\*        using BC Letter Code from CCLAM record, look up the BC sequence    *
\*        no. from the BC Table                                              *
\*        save sequence no. and claim no. to report table                    *
\*      add 1 to report table index                                          *
\*      if report table index > max report table size                        *
\*          log event  126                                                   *
\*          exit program                                                     *
\*      endif                                                                *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************

RET.CLAIM.TO.REPORT.TABLE:
       BC.TAB.IDX% = ASC(CCLAM.BC.LETTER$) - 48
       REPORT.TABLE$(RPT.TAB.IDX%) = MID$(BC.TABLE$(BC.TAB.IDX%),16,2)  \!BMJB
                                  + CCLAM.CREDIT.CLAIM.NUM$
       RPT.TAB.IDX% = RPT.TAB.IDX% + 1
       IF RPT.TAB.IDX% > MAX.REPORT.TABLE.SIZE% THEN BEGIN
           LOG.EVENT.NO% = 126
           LOG.STRING.1.UNIQUE$ = "REPORT TAB"
           GOSUB LOG.AN.EVENT
           PROGRAM.FAIL = TRUE
           GOTO PROGRAM.EXIT
       ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* SORT.REPORT.TABLE:                              (Returns only)            *
\*        if more than 1 entry in report table                               *
\*              sort report table on BC sequence no.,claim no. ascending     *
\*       endif                                                               *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
SORT.REPORT.TABLE:
    IF RPT.TAB.NO.ENTRIES% > 1 THEN BEGIN
       GAP = RPT.TAB.NO.ENTRIES%
       WHILE NOT SORT.COMPLETE
           GAP = INT(GAP / 1.3)
           IF GAP = 0 THEN GAP = 1
           IF GAP = 9 OR GAP = 10 THEN GAP = 11
           SWAPPED = FALSE
           FOR A% = 1 TO (RPT.TAB.NO.ENTRIES% - GAP)
               B% = A% + GAP
              IF REPORT.TABLE$(A%) > REPORT.TABLE$(B%) THEN BEGIN
                  TEMP$ = REPORT.TABLE$(A%)
                  REPORT.TABLE$(A%) = REPORT.TABLE$(B%)
                  REPORT.TABLE$(B%) = TEMP$
                  SWAPPED = TRUE
               ENDIF
           NEXT A%
           IF (NOT SWAPPED) AND GAP = 1 THEN SORT.COMPLETE = TRUE
       WEND
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* SAVE.CLAIM.TO.DELETE.TABLE:                                               *
\*        save claim No.(4 bytes) and No. of Items (4 bytes) to delete table *
\*      add 1 to delete table index                                          *
\*      if delete table index > max delete table size                        *
\*          log event  126                                                   *
\*          exit program                                                     *
\*      endif                                                                *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
SAVE.CLAIM.TO.DELETE.TABLE:
       DELETE.TABLE$(DEL.TAB.IDX%) = CCLAM.CREDIT.CLAIM.NUM$ +               \
                                       RIGHT$("0000" +                       \
                                        STR$(CCLAM.NUM.OF.ITEMS%),4)
       DEL.TAB.IDX% = DEL.TAB.IDX% + 1
       IF DEL.TAB.IDX% > MAX.DELETE.TABLE.SIZE% THEN BEGIN
           LOG.EVENT.NO% = 126
           LOG.STRING.1.UNIQUE$ = "DELETE TAB"
           GOSUB LOG.AN.EVENT
           PROGRAM.FAIL = TRUE
           GOTO PROGRAM.EXIT
       ENDIF
RETURN


\*****************************************************************************
\*****************************************************************************
\* RET.CLAIMS.REPORTING:                                                     *
\*       this is the controlling routine for producing the Returns Credit    *
\*       Claiming Summary and Detail Reports (CCSMY and CCDET)               *
\*                                                                           *
\*       gosub RET.RPT.START                                                 *
\*       repeat until End of Report                                          *
\*           gosub RET.RPT.PROCESS.BC                                        *
\*       end repeat                                                          *
\*       gosub RET.RPT.END                                                   *
\*       gosub RET.RPT.NUM.OF.PAGES                                          *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.CLAIMS.REPORTING:
    
    GOSUB RET.RPT.START
	 
    WHILE NOT END.OF.REPORT
	    
       GOSUB RET.RPT.PROCESS.BC
	    
    WEND
    
    GOSUB RET.RPT.END
	 
    IF CREDIT.CLAIM.ACTIVE = TRUE THEN BEGIN                                 !1.9JAT
       GOSUB CREATE.NEW.CCSMY                                                !1.9JAT
    ENDIF                                                                    !1.9JAT
    GOSUB RET.RPT.NUM.OF.PAGES
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.START:                                                            *
\*       gosub FORMAT.WC.DATE                                                *
\*       gosub OPEN.CCRSN                                                    *
\*       gosub OPEN.CCLAM.KEYED                                              *
\*       gosub RET.CREATE.REPORT.FILES                                       *
\*       gosub GET.LINES.PER.PAGE                                            *
\*       initialise Page Nos. = 0 and report table index = 1                 *
\*       format report date                                                  *
\*       format report time                                                  *
\*       if no. of claims in report table > 0                                *
\*           key read CCLAM (priming read)                                   *
\*       else                                                                *
\*           gosub CCSMY.START.NEW.PAGE                                      *
\*           gosub CCDET.START.NEW.PAGE                                      *
\*           set End of Report to True                                       *
\*       endif                                                               *
\* RETURN                                                                    *
\*****************************************************************************

RET.RPT.START:
    GOSUB FORMAT.WC.DATE
    GOSUB OPEN.CCRSN
    GOSUB OPEN.CCLAM.KEYED
    GOSUB RET.CREATE.REPORT.FILES
    GOSUB GET.LINES.PER.PAGE
    CCSMY.PAGE.NO% = 0
    CCDET.PAGE.NO% = 0
    RPT.TAB.IDX% = 1

    PRINT.TODAYS.DATE$ = MID$(TODAYS.DATE$,5,2) + "/" +                      \
                           MID$(TODAYS.DATE$,3,2) + "/" +                      \
                      MID$(TODAYS.DATE$,1,2)
    PRINT.TIME$ = LEFT$(TIME$,2) + ":" + MID$(TIME$,3,2)

    IF RPT.TAB.NO.ENTRIES% > 0 THEN BEGIN
        CCLAM.CREDIT.CLAIM.NUM$ = RIGHT$(REPORT.TABLE$(RPT.TAB.IDX%),4)
       RC% = READ.CCLAM
       IF RC% <> 0 THEN BEGIN
              GOSUB READ.FILE.ERROR
       ENDIF
       END.OF.REPORT = FALSE
    ENDIF ELSE BEGIN
        GOSUB CCSMY.START.NEW.PAGE
        GOSUB CCDET.START.NEW.PAGE
       END.OF.REPORT = TRUE
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.PROCESS.BC:                                                       *
\*        this is the controlling routine for reporting each Business Centre *
\*                                                                           *
\*       gosub RET.RPT.START.BC                                              *
\*       repeat       until End of Report or End of BC                       *
\*              gosub RET.RPT.PROCESS.CLAIM                                  *
\*       end repeat                                                          *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.PROCESS.BC:

     
    GOSUB RET.RPT.START.BC
    WHILE  NOT END.OF.BC
	          
             GOSUB RET.RPT.PROCESS.CLAIM
    WEND
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.START.BC:                                                         *
\*       store current BC letter                                             *
\*       gosub CCSMY.START.NEW.PAGE                                          *
\*       gosub CCDET.START.NEW.PAGE                                          *
\*       format BC Headers and write to CCSMY and CCDET Reports              *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.START.BC:
     
    STORE.BC.LETTER$ = CCLAM.BC.LETTER$
    END.OF.BC = FALSE

    GOSUB CCSMY.START.NEW.PAGE
    GOSUB CCDET.START.NEW.PAGE

    BC.TAB.IDX% = ASC(CCLAM.BC.LETTER$) - 48

    DISPLAY.BC$ = CCLAM.BC.LETTER$                                       !CMJK

    IF DISPLAY.BC$ = CHR$(91) THEN DISPLAY.BC$ = "-"                     !CMJK
       IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                         !1.9JAT

          CCSMY.REPORT.LINE$ = " BC  " +                                 \
                                DISPLAY.BC$ +                            \CMJK
                            ", " +                                       \
                            MID$(BC.TABLE$(BC.TAB.IDX%),2,14)
          GOSUB WRITE.CCSMY.REPORT
          CCSMY.REPORT.LINE$ = " ---------------------"
          GOSUB WRITE.CCSMY.REPORT
          CCSMY.REPORT.LINE$ = BLANK.LINE$
          GOSUB WRITE.CCSMY.REPORT
       ENDIF                                                             !1.9JAT
    CCDET.REPORT.LINE$ = " BC  " +                                       \
                          DISPLAY.BC$ +                                  \CMJK
                      ", " +                                             \
                      MID$(BC.TABLE$(BC.TAB.IDX%),2,14)
    GOSUB WRITE.CCDET.REPORT
    CCDET.REPORT.LINE$ = " ---------------------"
    GOSUB WRITE.CCDET.REPORT
    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT

RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.PROCESS.CLAIM:                                                    *
\*       this is the controlling routine for reporting each claim within a   *
\*       business centre                                                     *
\*                                                                           *
\*       gosub RET.RPT.START.CLAIM                                           *
\*       repeat until Item No. > CCLAM.NUM.OF>ITEMS                          *
\*              gosub RET.RPT.PROCESS.ITEM                                   *
\*       end repeat                                                          *
\*       gosub RET.RPT.END.CLAIM                                             *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************

RET.RPT.PROCESS.CLAIM:
     
    IF CCLAM.CANC.MARKER$="Y" THEN BEGIN                                 !CMJK
      CLAIM.TOTAL.VALUE% = 0                                             !CMJK
    ENDIF ELSE BEGIN                                                     !CMJK
	  
      GOSUB RET.RPT.START.CLAIM
      WHILE (NOT ITEM.NO% > CCLAM.NUM.OF.ITEMS%)
	       
          GOSUB RET.RPT.PROCESS.ITEM
      WEND
    ENDIF                                                                !CMJK
    GOSUB RET.RPT.END.CLAIM
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.START.CLAIM:                                                      *
\*    initialise Item No. = 1, Claim Total Value = 0                         *
\*    dimension Item Table = CCLAM.NUM.OF.ITEMS                              *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.START.CLAIM:
     
    ITEM.NO% = 1
    CLAIM.TOTAL.VALUE% = 0
    DIM ITEM.TABLE$(CCLAM.NUM.OF.ITEMS%)
    CCITF.CREDIT.CLAIM.NUM$ = CCLAM.CREDIT.CLAIM.NUM$
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.PROCESS.ITEM:                                                     *
\*       read CCITF by key (if error gosub READ.FILE.ERROR)                  *
\*       Item Claim Value = Item Price * Item Quantity                       *
\*       Add Item Claim Value to Claim Total Value                           *
\*       Save Item details (Item/Bar code flag, Boots/Bar code, quantity     *
\*       and price to Item Table                                             *
\*       Add 1 to Item No.                                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.PROCESS.ITEM:
     
    CCITF.ITEM.NUM$ = PACK$(RIGHT$("0000" + STR$(ITEM.NO%),4))	
    CCITF.KEY$ = CCITF.CREDIT.CLAIM.NUM$ + CCITF.ITEM.NUM$
	 
	PRINT UNPACK$(CCITF.KEY$)
	 
    RC% = READ.CCITF
    IF RC% <> 0 THEN BEGIN
	    
        GOSUB READ.FILE.ERROR
 
    ENDIF
     
    ITEM.CLAIM.VALUE% = VAL(UNPACK$(CCITF.PRICE$))                       ! DDS
	PRINT "PASSED"
    PACK.SIZE% = VAL(CCITF.FILLER$)                                     ! DMJK	
    IF PACK.SIZE%>0 THEN BEGIN                                          ! DMJK
      PACK.SIZE$ = "/"+RIGHT$("    "+STR$(PACK.SIZE%),4)                ! DMJK
    ENDIF ELSE BEGIN                                                    ! DMJK
      PACK.SIZE$ = STRING$(5," ")                                       ! DMJK
    ENDIF                                                               ! DMJK

    IF PACK.SIZE%<1 THEN PACK.SIZE% = 1                                 ! DMJK
    IF CCITF.QTY% <> 0 THEN BEGIN                                        ! DDS
       IF PACK.SIZE% = 1 THEN BEGIN                                     ! DMJK
         ITEM.PRICE% = ITEM.CLAIM.VALUE% / CCITF.QTY%                    ! DDS
       ENDIF ELSE BEGIN                                                 ! DMJK
         REAL.ITEM.CLAIM.VALUE = ITEM.CLAIM.VALUE%                      ! DMJK
        REAL.CCITF.QTY = CCITF.QTY%                                     ! DMJK
        REAL.PACK.SIZE = PACK.SIZE%                                     ! DMJK
         ITEM.PRICE% = REAL.ITEM.CLAIM.VALUE /                          \ DMJK
                      (REAL.CCITF.QTY/REAL.PACK.SIZE)                   ! DMJK
       ENDIF                                                            ! DMJK
    ENDIF ELSE BEGIN                                                     ! DDS
       ITEM.PRICE% = 0                                                   ! DDS
    ENDIF
    CLAIM.TOTAL.VALUE% = CLAIM.TOTAL.VALUE% + ITEM.CLAIM.VALUE%

    VALUE$ = STR$(ITEM.CLAIM.VALUE%)                                    ! DMJK
    GOSUB FORMAT.VALUE                                                  ! DMJK
    ITEM.CLAIM.VALUE$ = VALUE$                                          ! DMJK

    VALUE$ = STR$(ITEM.PRICE%)
    GOSUB FORMAT.VALUE


    ITEM.TABLE$(ITEM.NO%) = CCITF.ITEM.BAR.CODE.FLAG$ +                      \
                             CCITF.BOOTS.BAR.CODE$ +                         \
                         RIGHT$(STRING$(4," ")+STR$(CCITF.QTY%),4) +    \ CMJK
                         VALUE$ + PACK.SIZE$       + ITEM.CLAIM.VALUE$  ! DMJK

    ITEM.NO% = ITEM.NO% + 1

    !Locate the index number where the current claim is stored on the claims tables 1.9JAT
    !UPDATE THE CLAIM VALUE FOR THIS ITEM TO THE TOTAL STORED ON THE CLAIMS TABLE   1.9JAT
    FOR A% = 1 TO CLAIMS.COUNTER%
        IF CCITF.CREDIT.CLAIM.NUM$ = LEFT$(NEW.CLAIM.RECORDS$(A%),4) THEN BEGIN     !1.9JAT
           CURRENT.CLAIM.VALUE% = VAL(MID$(NEW.CLAIM.RECORDS$(A%),37,8))             !1.9JAT
           NEW.CLAIM.VALUE% = CURRENT.CLAIM.VALUE% + ITEM.CLAIM.VALUE%              !1.9JAT                      
           NEW.CLAIM.RECORDS$(A%) = (LEFT$(NEW.CLAIM.RECORDS$(A%),36) +            \ 1.9JAT
                                    RIGHT$("00000000" + STR$(NEW.CLAIM.VALUE%),8)+ \ 1.9JAT
                                    MID$(NEW.CLAIM.RECORDS$(A%),45,6))              !4.2HSM
           A% = CLAIMS.COUNTER%                                                     !1.9JAT
        ENDIF                                                                       !1.9JAT
    NEXT A%                                                                         !1.9JAT
   
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.END.CLAIM:                                                        *
\*     gosub RET.RPT.WRITE.CLAIM.HEADER                                      *
\*     gosub RET.RPT.WRITE.CLAIM.DETAILS                                     *
\*     if reason code is not '23' or '26'                                    *
\*         initialise Item No. = 1                                           *
\*         repeat until Item No. > CCLAM.NUM.OF.ITEMS                        *
\*             gosub RET.RPT.WRITE.ITEM.DETAILS                              *
\*         end repeat                                                        *
\*     endif                                                                 *
\*     Add 1 to Report table Index                                           *
\*     If Report Table Index > No. of Report Table Entries                   *
\*          set End of Report to True                                        *
\*     else                                                                  *
\*               read next CCLAM record by key                               *
\*          if new BC Letter Code                                            *
\*              set End of BC to True                                        *
\*          endif                                                            *
\*    endif                                                                  *
\*    if required, start new page, else write blank lines to reports         *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************

RET.RPT.END.CLAIM:

    GOSUB RET.RPT.WRITE.CLAIM.HEADER

    IF CCLAM.CANC.MARKER$ = "Y" THEN BEGIN                               !CMJK
      IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                          !1.9JAT
         CCSMY.REPORT.LINE$ = CANCELLED.LINE$                            !CMJK
         GOSUB WRITE.CCSMY.REPORT                                        !CMJK
      ENDIF                                                              !1.9JAT
      CCDET.REPORT.LINE$ = CANCELLED.LINE$                               !CMJK
      GOSUB WRITE.CCDET.REPORT                                           !CMJK
    ENDIF ELSE BEGIN                                                     !CMJK

      GOSUB RET.RPT.WRITE.CLAIM.DETAILS
      IF (UNPACK$(CCLAM.REASON.NUM$) <> "23")                            \BMJB
      AND (UNPACK$(CCLAM.REASON.NUM$) <> "26") THEN BEGIN                !BMJB
            ITEM.NO% = 1
       HEADER.PRINTED = 0                                                ! DDS
           WHILE (NOT ITEM.NO% > CCLAM.NUM.OF.ITEMS%)
            GOSUB RET.RPT.WRITE.ITEM.DETAILS
           WEND
      ENDIF                                                              !BMJB

    ENDIF                                                                !CMJK

    RPT.TAB.IDX% = RPT.TAB.IDX% + 1
    IF RPT.TAB.IDX% > RPT.TAB.NO.ENTRIES% THEN BEGIN
        END.OF.BC = TRUE
           END.OF.REPORT = TRUE
    ENDIF ELSE BEGIN
        CCLAM.CREDIT.CLAIM.NUM$ = RIGHT$(REPORT.TABLE$(RPT.TAB.IDX%),4)
        RC% = READ.CCLAM
        IF RC% <> 0 THEN BEGIN
           GOSUB READ.FILE.ERROR
        ENDIF
!        IF ASC(CCLAM.BC.LETTER$)<65 OR ASC(CCLAM.BC.LETTER$)>90 THEN     \CMJK  1.7JAS
!         CCLAM.BC.LETTER$ = CHR$(91)                                     !CMJK  1.7JAS

        IF ASC(CCLAM.BC.LETTER$) > 90 THEN BEGIN                                       !1.8BMG
           CCLAM.BC.LETTER$ = CHR$(91)                                                 !1.8BMG
        ENDIF ELSE BEGIN                                                               !1.8BMG
           IF ASC(CCLAM.BC.LETTER$) < 65 THEN BEGIN                                    !1.8BMG
              IF ASC(CCLAM.BC.LETTER$) < 49 OR ASC(CCLAM.BC.LETTER$) > 57 THEN BEGIN   !1.8BMG
                 CCLAM.BC.LETTER$ = CHR$(91)                                           !1.8BMG
              ENDIF                                                                    !1.8BMG
           ENDIF                                                                       !1.8BMG
        ENDIF                                                                          !1.8BMG

       IF CCLAM.BC.LETTER$ <> STORE.BC.LETTER$ THEN BEGIN
           END.OF.BC = TRUE
       ENDIF
    ENDIF

    IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 2 THEN BEGIN
        IF NOT END.OF.BC THEN BEGIN
               GOSUB CCSMY.START.NEW.PAGE
       ENDIF
    ENDIF ELSE BEGIN
        IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                             !1.9JAT
           CCSMY.REPORT.LINE$ = BLANK.LINE$
           GOSUB WRITE.CCSMY.REPORT
           GOSUB WRITE.CCSMY.REPORT
        ENDIF                                                                 !1.9JAT
    ENDIF

    IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 2 THEN BEGIN
        IF NOT END.OF.BC THEN BEGIN
               GOSUB CCDET.START.NEW.PAGE
       ENDIF
    ENDIF ELSE BEGIN
        CCDET.REPORT.LINE$ = BLANK.LINE$
        GOSUB WRITE.CCDET.REPORT
        GOSUB WRITE.CCDET.REPORT
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.WRITE.CLAIM.HEADER:                                               *
\*     if less than 3 lines left on CCSMY page                               *
\*         gosub CCSMY.START.NEW.PAGE                                        *
\*     endif                                                                 *
\*     if less than 3 lines left on CCDET page                               *
\*         gosub CCDET.START.NEW.PAGE                                        *
\*     endif                                                                 *
\*     gosub FORMAT.VALUE to format Claim Total Value for printing           *
\*     format and write Credit Claim header to CCSMY                         *
\*     format and write Credit Claim header to CCDET                         *
\*     write blank line to CCSMY/CCDET                                       *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.WRITE.CLAIM.HEADER:

   IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 3 THEN BEGIN
        GOSUB CCDET.START.NEW.PAGE
    ENDIF

    VALUE$ = STR$(CLAIM.TOTAL.VALUE%)
    GOSUB FORMAT.VALUE

IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                                    !1.9JAT

    IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 3 THEN BEGIN
        GOSUB CCSMY.START.NEW.PAGE
    ENDIF

    CCSMY.REPORT.LINE$ = NUMB$ + " : " + RIGHT$("00000000"   +               \1.6AH
                         UNPACK$(CCLAM.CREDIT.CLAIM.NUM$),8) +               \
                         STRING$(13," ")                     +               \
                         "Total value   "                    +               \
                         VALUE$
    GOSUB WRITE.CCSMY.REPORT
    CCSMY.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCSMY.REPORT

ENDIF                                                                        !1.9JAT

    CCDET.REPORT.LINE$ = NUMB$ + " : " + RIGHT$("00000000"   +               \1.6AH
                         UNPACK$(CCLAM.CREDIT.CLAIM.NUM$),8) +               \
                         STRING$(12," ")                     +               \
                         "Total value   "                    +               \
                         VALUE$
    GOSUB WRITE.CCDET.REPORT
    
    GOSUB PROCESS.CURRENT.TILL.CLAIM                                         !4.2HSM                                 
    CCDET.REPORT.LINE$ = CURRENT.TILL.CLAIM$                                 !4.2HSM
    GOSUB WRITE.CCDET.REPORT                                                 !4.2HSM
                                                                             
    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT

RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.WRITE.CLAIM.DETAILS:                                              *
\*     if Reason Code present on claim record                                *
\*         read CCRSN using reason code key                                  *
\*         format reason description line and write to CCSMY/CCDET           *
\*     endif                                                                 *
\*     if Supply Route present on claim record                               *
\*         format Supply Route detail                                        *
\*         gosub FORMAT.CLAIM.DETAIL (formats and prints claim details)      *
\*     endif                                                                 *
\*     if Recall Number present on claim record                              *
\*         format Supply Route detail                                        *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Authorisation present on claim record                              *
\*         format Authorisation detail                                       *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Supplier present on claim record                                   *
\*         format Supplier detail                                            *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Method of Return is present on claim record                        *
\*         format Method of Return detail                                    *
\*         if Carrier format Carrier detail                                  *
\*         if BIRD format BIRD Number detail                                 *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Receiving Store is present on claim record                         *
\*         format Receiving Store detail                                     *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Destination is present on claim record                             *
\*         format Destination detail                                         *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if UOD Type is present on claim record                                *
\*         format UOD Type detail                                            *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Invoice Number is present on claim record                          *
\*         format Invoice Number detail                                      *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Folio Number is present on claim record                            *
\*         format Folio Number detail                                        *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Batch Ref. Number is present on claim record                       *
\*         format Batch Ref Number detail                                    *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Consignment Type is present on claim record                        *
\*         format Consignment Type detail                                    *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Repair Category is present on claim record                         *
\*         format Repair Category detail                                     *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Repair Number is present on claim record                           *
\*         format Repair Number detail                                       *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Plan4 Policy Number is present on claim record                     *
\*         format Plan4 Policy Number detail                                 *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if DDDA/DCDR Number is present on claim record                        *
\*         format DDDA/DDCR Number detail                                    *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if Delivery Note Number is present on claim record                    *
\*         format Delivery Note Number Detail                                *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*     endif                                                                 *
\*     if first part of claim detail line is populated (not yet printed)     *
\*         gosub FORMAT.CLAIM.DETAIL                                         *
\*         (this ensures that all claim details present get printed)         *
\*     endif                                                                 *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.WRITE.CLAIM.DETAILS:

\*Reason
    IF CCLAM.REASON.NUM$ <> PACK$("00") THEN BEGIN
        CCRSN.REASON$ = CCLAM.REASON.NUM$
        RC% = READ.CCRSN
        IF RC% <> 0 THEN BEGIN
               CCRSN.DESC$ = "NOT ON FILE                   "
        ENDIF

        IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                    !1.9JAT
           CCSMY.REPORT.LINE$ = " Reason for return :  " +           \
                                   CCRSN.DESC$
           GOSUB WRITE.CCSMY.REPORT
        ENDIF                                                        !1.9JAT

        CCDET.REPORT.LINE$ = " Reason for return :  " +              \
                                CCRSN.DESC$
        GOSUB WRITE.CCDET.REPORT
    ENDIF

\*Supply Route
    IF CCLAM.SUPPLY.ROUTE$ <> " " THEN BEGIN
        IF CCLAM.SUPPLY.ROUTE$ = "W" THEN BEGIN
            SUPPLY.ROUTE.DESC$ = "Warehouse       "
        ENDIF ELSE BEGIN
          IF CCLAM.SUPPLY.ROUTE$ = "O" THEN BEGIN
              SUPPLY.ROUTE.DESC$ = "Directs         "                    !CMJK
          ENDIF ELSE BEGIN
            IF CCLAM.SUPPLY.ROUTE$ = "D" THEN BEGIN
              SUPPLY.ROUTE.DESC$ = "Dispensary      "
              ENDIF ELSE BEGIN
               SUPPLY.ROUTE.DESC$ = "Unknown Route   "
            ENDIF
          ENDIF
        ENDIF
        CLAIM.DETAIL$ = " Supply Route :       " + SUPPLY.ROUTE.DESC$
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Recall Number
    IF CCLAM.RECALL.NUM$ <> STRING$(8," ") THEN BEGIN
        FIELD$ = CCLAM.RECALL.NUM$
       GOSUB LEFT.JUSTIFY.FIELD
       CCLAM.RECALL.NUM$ = FIELD$
        CLAIM.DETAIL$ = " Recall Number :      " +                           \
                     CCLAM.RECALL.NUM$
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Authorisation
    IF CCLAM.AUTHORISATION$ <> STRING$(15," ") THEN BEGIN  
        IF CCLAM.FILLER$ = STRING$(7," ") THEN BEGIN                              !4.2HSM this will used for till claims 
            FIELD$ = CCLAM.AUTHORISATION$
            GOSUB LEFT.JUSTIFY.FIELD
            CCLAM.AUTHORISATION$ = FIELD$
            CLAIM.DETAIL$ = " Authorisation :      " +                           \
                             CCLAM.AUTHORISATION$
            GOSUB FORMAT.CLAIM.DETAIL                                          
        ENDIF                                                                     !4.2HSM
    ENDIF

\*Supplier
    IF CCLAM.SUPPLIER$ <> STRING$(15," ") THEN BEGIN
        FIELD$ = CCLAM.SUPPLIER$
       GOSUB LEFT.JUSTIFY.FIELD
       CCLAM.SUPPLIER$ = FIELD$
        CLAIM.DETAIL$ = " Supplier :           " +                           \
                     CCLAM.SUPPLIER$
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Method of Return
    IF CCLAM.METHOD.OF.RETURN$ <> PACK$("00") THEN BEGIN
        IF UNPACK$(CCLAM.METHOD.OF.RETURN$) = "01" THEN BEGIN         !Carrier

           IF UNPACK$(CCLAM.CARRIER$) = "01" THEN BEGIN                   !GPO
               CLAIM.DETAIL$ = " Method of Return :   " +                    \
                            "GPO             "
           ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.CARRIER$) = "02" THEN BEGIN           !Parcelforce
               CLAIM.DETAIL$ = " Method of Return :   " +                    \
                            "Parcelforce     "
           ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.CARRIER$) = "03" THEN BEGIN             !Securicor
               CLAIM.DETAIL$ = " Method of Return :   " +                    \
                            "Securicor       "
           ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.CARRIER$) = "04" THEN BEGIN                 !Other
               CLAIM.DETAIL$ = " Method of Return :   " +                    \
                            "Other Carrier   "
           ENDIF ELSE BEGIN                                           !Invalid

               CLAIM.DETAIL$ = " Method of Return :   " +                    \
                            "Unknown Carrier "
           ENDIF
           ENDIF
           ENDIF
           ENDIF
       ENDIF ELSE BEGIN

       IF UNPACK$(CCLAM.METHOD.OF.RETURN$) = "02" THEN BEGIN             !BIRD
            FIELD$ = CCLAM.BIRD.NUM$
           GOSUB LEFT.JUSTIFY.FIELD
            CCLAM.BIRD.NUM$ = FIELD$
           CLAIM.DETAIL$ = " BIRD Number :        " +                        \
                         CCLAM.BIRD.NUM$
       ENDIF ELSE BEGIN

       IF UNPACK$(CCLAM.METHOD.OF.RETURN$) = "03" THEN BEGIN           !Via D6
           CLAIM.DETAIL$ = " Method of Return :   " +                        \
                             "Via D6          "
       ENDIF ELSE BEGIN

        IF UNPACK$(CCLAM.METHOD.OF.RETURN$) = "04" THEN BEGIN             !Rep
           CLAIM.DETAIL$ = " Method of Return :   " +                        \
                             "Representative  "
       ENDIF ELSE BEGIN                                               !Invalid
           CLAIM.DETAIL$ = " Method of Return :   " +                        \
                             "Unknown Method  "

        ENDIF
        ENDIF
       ENDIF
       ENDIF
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Receiving Store
    IF CCLAM.RECEIVING.STORE$ <> PACK$("0000") THEN BEGIN
        CLAIM.DETAIL$ = " Receiving Store :    " +                           \
                     UNPACK$(CCLAM.RECEIVING.STORE$)
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Destination
    IF CCLAM.DESTINATION$ <> PACK$("00") THEN BEGIN
        IF UNPACK$(CCLAM.DESTINATION$) = "01" THEN BEGIN                   !BC
               CLAIM.DETAIL$ = " Destination :        " +                    \
                             "Business Centre "
       ENDIF ELSE BEGIN

        IF UNPACK$(CCLAM.DESTINATION$) = "02" THEN BEGIN                  !MTS
               CLAIM.DETAIL$ = " Destination :        " +                    \
                             "MTS             "
       ENDIF ELSE BEGIN

        IF UNPACK$(CCLAM.DESTINATION$) = "03" THEN BEGIN             !Pharmacy
               CLAIM.DETAIL$ = " Destination :        " +                    \
                             "Pharmacy        "
       ENDIF ELSE BEGIN

        IF UNPACK$(CCLAM.DESTINATION$) = "04" THEN BEGIN                !Other
               CLAIM.DETAIL$ = " Destination :        " +                    \
                             "Other Destn.    "
       ENDIF ELSE BEGIN                                               !Invalid
               CLAIM.DETAIL$ = " Destination :        " +                    \
                             "Unknown Destn.  "
           ENDIF
           ENDIF
           ENDIF
           ENDIF
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*UOD Type
    IF CCLAM.UOD.TYPE$ <> PACK$("00") THEN BEGIN
           IF UNPACK$(CCLAM.UOD.TYPE$) = "01" THEN BEGIN         !Travel Outer
               CLAIM.DETAIL$ = " UOD Type :           " +                    \
                             "Travel Outer    "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.UOD.TYPE$) = "02" THEN BEGIN            !Town Tray
               CLAIM.DETAIL$ = " UOD Type :           " +                    \
                             "Town Tray       "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.UOD.TYPE$) = "03" THEN BEGIN            !Roll Cage
               CLAIM.DETAIL$ = " UOD Type :           " +                    \
                             "Roll Cage       "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.UOD.TYPE$) = "04" THEN BEGIN                !Other
               CLAIM.DETAIL$ = " UOD Type :           " +                    \
                             "Other UOD       "
       ENDIF ELSE BEGIN                                               !Invalid
               CLAIM.DETAIL$ = " UOD Type :           " +                    \
                             "Unknown UOD     "
       ENDIF
       ENDIF
       ENDIF
       ENDIF
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Invoice Number
    IF CCLAM.INVOICE.NUM$ <> STRING$(9," ") THEN BEGIN
        FIELD$ = CCLAM.INVOICE.NUM$
       GOSUB LEFT.JUSTIFY.FIELD
       CCLAM.INVOICE.NUM$ = FIELD$
        CLAIM.DETAIL$ = " Invoice Number :     " +                           \
                     CCLAM.INVOICE.NUM$
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Folio Number
    IF CCLAM.FOLIO.NUM$ <> PACK$(STRING$(6,"0")) THEN BEGIN
        CLAIM.DETAIL$ = " Folio Number :       " +                           \
                     UNPACK$(CCLAM.FOLIO.NUM$)
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Batch Reference
    IF CCLAM.BATCH.REF$ <> PACK$(STRING$(6,"0")) THEN BEGIN
        CLAIM.DETAIL$ = " Batch Ref. Number :  " +                           \
                     UNPACK$(CCLAM.BATCH.REF$)
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Consignment Type
    IF CCLAM.WHOLE.PART.CON$ <> " " THEN BEGIN
        IF CCLAM.WHOLE.PART.CON$ = "W" THEN BEGIN                       !Whole
           CLAIM.DETAIL$ = " Consignment Type :   " +                        \
                             "Whole           "
       ENDIF ELSE BEGIN

        IF CCLAM.WHOLE.PART.CON$ = "P" THEN BEGIN                        !Part
           CLAIM.DETAIL$ = " Consignment Type :   " +                        \
                             "Part            "
       ENDIF ELSE BEGIN                                               !Invalid
           CLAIM.DETAIL$ = " Consignment Type :   " +                        \
                             "Unknown Type    "
           ENDIF
           ENDIF
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Repair Category
    IF CCLAM.REPAIR.CATEGORY$ <> PACK$("00") THEN BEGIN
           IF UNPACK$(CCLAM.REPAIR.CATEGORY$) = "01" THEN BEGIN         !Plan4
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Plan4           "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.REPAIR.CATEGORY$) = "02" THEN BEGIN      !Estimate
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Estimate        "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.REPAIR.CATEGORY$) = "03" THEN BEGIN   !Boots Guar.
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Boots Guarantee "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.REPAIR.CATEGORY$) = "04" THEN BEGIN   !Supp. Guar.
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Supplier Guar.  "
       ENDIF ELSE BEGIN

           IF UNPACK$(CCLAM.REPAIR.CATEGORY$) = "05" THEN BEGIN   !Other Guar.
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Other Guarantee "
       ENDIF ELSE BEGIN                                               !Invalid
               CLAIM.DETAIL$ = " Repair Category :    " +                    \
                             "Unknown Category"
       ENDIF
       ENDIF
       ENDIF
       ENDIF
       ENDIF
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Repair Number
    IF CCLAM.REPAIR.NUM$ <> PACK$(STRING$(12,"0")) THEN BEGIN
        CLAIM.DETAIL$ = " Repair Number :      " +                           \
                     UNPACK$(CCLAM.REPAIR.NUM$)
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Plan4 Policy Number
    IF CCLAM.PLAN4.POLICY.NUM$ <> PACK$(STRING$(12,"0")) THEN BEGIN
        CLAIM.DETAIL$ = " Plan4 Policy Number: " +                           \
                     UNPACK$(CCLAM.PLAN4.POLICY.NUM$)
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*DDDA/DCDR Number
    IF CCLAM.DDDA.DCDR.NUM$ <> PACK$(STRING$(8,"0")) THEN BEGIN
        CLAIM.DETAIL$ = " DDDA/DCDR Number :   " +                           \
                     UNPACK$(CCLAM.DDDA.DCDR.NUM$)
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF

\*Delivery Note Number
    IF CCLAM.DELIV.NOTE.NUM$ <> STRING$(9," ") THEN BEGIN
        FIELD$ = CCLAM.DELIV.NOTE.NUM$
       GOSUB LEFT.JUSTIFY.FIELD
       CCLAM.DELIV.NOTE.NUM$ = FIELD$
        CLAIM.DETAIL$ = " Delivery Note No. :  " +                           \
                     CCLAM.DELIV.NOTE.NUM$
        GOSUB FORMAT.CLAIM.DETAIL
    ENDIF
  
\*If first part of claim detail line populated then print to reports
    IF DETAIL.FIRST.PART.FULL THEN BEGIN
        CLAIM.DETAIL$ = STRING$(38," ")
       GOSUB FORMAT.CLAIM.DETAIL
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* FORMAT.CLAIM.DETAIL:                                                      *
\*     formats claim details into pairs on one line and prints to reports    *
\*                                                                           *
\*     if detail first part not full                                         *
\*         move claim detail to first part                                   *
\*         set Detail First Part Full to true                                *
\*     else                                                                  *
\*         else move claim detail to second part                             *
\*         if less than 1 line left on CCSMY page                            *
\*            gosub CCSMY.START.NEW.PAGE                                     *
\*         endif                                                             *
\*         if less than 1 line left on CCDET page                            *
\*            gosub CCDET.START.NEW.PAGE                                     *
\*         endif                                                             *
\*         format CCSMY/CCDET report line = first part + second part         *
\*         write detail lines to CCSMY/CCDET                                 *
\*         initialise detail first part/second part to spaces                *
\*         set Detail First Part Full to false                               *
\*     endif                                                                 *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
FORMAT.CLAIM.DETAIL:
    IF NOT DETAIL.FIRST.PART.FULL THEN BEGIN
        DETAIL.FIRST.PART$ = LEFT$(CLAIM.DETAIL$ + STRING$(38," "),38)
       DETAIL.FIRST.PART.FULL = TRUE
    ENDIF ELSE BEGIN
        DETAIL.SECOND.PART$ = LEFT$(CLAIM.DETAIL$ + STRING$(38," "),38)

        IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 1 THEN BEGIN
            GOSUB CCSMY.START.NEW.PAGE
        ENDIF
        IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 1 THEN BEGIN
            GOSUB CCDET.START.NEW.PAGE
        ENDIF

       IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                             !1.9JAT
          CCSMY.REPORT.LINE$ = DETAIL.FIRST.PART$ +                          \
                             STRING$(3," ") +                                \
                             DETAIL.SECOND.PART$
           GOSUB WRITE.CCSMY.REPORT                                          !1.9JAT
       ENDIF
       CCDET.REPORT.LINE$ = DETAIL.FIRST.PART$ +                             \
                          STRING$(3," ") +                                   \
                          DETAIL.SECOND.PART$
        GOSUB WRITE.CCDET.REPORT

       DETAIL.FIRST.PART$ = STRING$(38," ")
       DETAIL.SECOND.PART$ = STRING$(38," ")
        DETAIL.FIRST.PART.FULL = FALSE
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.WRITE.ITEM.HEADER:                                                *
\*     format Item Details header line and write to CCDET report             *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.WRITE.ITEM.HEADER:

    IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 4 THEN BEGIN
        GOSUB CCDET.START.NEW.PAGE
    ENDIF
    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT
    CCDET.REPORT.LINE$ = "   Item Code             Description      " + \ DMJK
                         "        Price    Qty/Pack       Value "       ! DMJK

    GOSUB WRITE.CCDET.REPORT
    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.WRITE.ITEM.DETAILS:                                               *
\*     read Item Table (item No.)                                            *
\*     if Item Code Flag is "I" (Boots Item Code)                            *
\*         read IDF by key (Boots Code)       and obtain item description    *
\*         format Boots Item Code for printing                               *
\*     else                                                                  *
\*         item description = 'not on file'                                  *
\*         format Item Bar Code for printing using Bar Code Display function *
\*     endif                                                                 *
\*     format Item Detail line and write to CCDET report                     *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.WRITE.ITEM.DETAILS:

    ITEM.CODE.FLAG$ = LEFT$(ITEM.TABLE$(ITEM.NO%),1)
    ITEM.CODE$ = MID$(ITEM.TABLE$(ITEM.NO%),2,7)                         !DMJK
    ITEM.QTY$ = MID$(ITEM.TABLE$(ITEM.NO%),9,4)
    ITEM.PRICE$ = MID$(ITEM.TABLE$(ITEM.NO%),13,9)                       !DMJK
    PACK.SIZE$ = MID$(ITEM.TABLE$(ITEM.NO%),22,5)                        !DMJK
    ITEM.CLAIM.VALUE$ = RIGHT$(ITEM.TABLE$(ITEM.NO%),9)                  !DMJK

    IF ITEM.CODE$ = "9999991" OR                                         \ DDS
       ITEM.QTY$ = "   0" THEN BEGIN                                     ! DDS
       GOTO SKIP.PRINT                                                   ! DDS
    ENDIF

    IF ITEM.CODE.FLAG$ = "I" THEN BEGIN                            !Boots Code
        IDF.BOOTS.CODE$ = RIGHT$(ITEM.CODE$,4)
       RC% = READ.IDF
       IF RC% <> 0 THEN BEGIN
           IDF.STNDRD.DESC$ = "+ + Item not on file + +"
       ENDIF
       ITEM.BOOTS.CODE$ = RIGHT$(UNPACK$(RIGHT$(ITEM.CODE$,4)),7)
       ITEM.CODE$ = STRING$(2," ") +                                         \
                   LEFT$(ITEM.BOOTS.CODE$,2) + "-" +                         \
                   MID$(ITEM.BOOTS.CODE$,3,2) + "-" +                        \
                   RIGHT$(ITEM.BOOTS.CODE$,3) +                              \
                   STRING$(6," ")
    ENDIF ELSE BEGIN                                                 !Bar Code
        IDF.STNDRD.DESC$ = "+ + Item not on file + +"
        ITEM.BAR.CODE$ = PACK$(MID$(UNPACK$(ITEM.CODE$),2,12))          ! DMJK
       RC% = CALC.BAR.CODE.DISPLAY(ITEM.BAR.CODE$)
       IF RC% <> 0 THEN BEGIN
              PROGRAM.FAIL = TRUE
           GOTO PROGRAM.EXIT
       ENDIF
       ITEM.CODE$ = F07.BAR.CODE.FORMAT$
    ENDIF

    IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 3 THEN BEGIN            ! DMJK
        GOSUB CCDET.START.NEW.PAGE                                      ! DMJK
    ENDIF                                                               ! DMJK

    IF NOT HEADER.PRINTED THEN BEGIN                                     ! DDS
       GOSUB RET.RPT.WRITE.ITEM.HEADER                                   ! DDS
       HEADER.PRINTED = 1                                                ! DDS
    ENDIF                                                                ! DDS

    CCDET.REPORT.LINE$ = " " + ITEM.CODE$ + "  " +                           \
                          IDF.STNDRD.DESC$ + "  " +                     \ DMJK
                      ITEM.PRICE$ +       "   " +                       \ DMJK
                      ITEM.QTY$ +                                       \ DMJK
                      PACK.SIZE$ + "   " +                              \ DMJK
                      ITEM.CLAIM.VALUE$                                 ! DMJK
    GOSUB WRITE.CCDET.REPORT

SKIP.PRINT:

    ITEM.NO% = ITEM.NO% + 1
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.END:                                                              *
\*    if new page required gosub CCSMY/CCDET.START.NEW.PAGE                  *
\*    format End of Report line and write to CCSMY and CCDET reports         *
\*    write page throw to end of each report                                 *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.END:

    IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                                !1.9JAT

       IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 1 THEN BEGIN
          GOSUB CCSMY.START.NEW.PAGE
       ENDIF
       CCSMY.REPORT.LINE$ = STRING$(13," ") +                                   \
                             "*  *  *   E N D   O F   R E P O R T    *  *  *"
       GOSUB WRITE.CCSMY.REPORT
    ENDIF                                                                    !1.9JAT

    IF LINES.PER.PAGE% - (CCDET.LINE.NO% - 1) < 1 THEN BEGIN
           GOSUB CCDET.START.NEW.PAGE
    ENDIF
    CCDET.REPORT.LINE$ = STRING$(13," ") +                                   \
                          "*  *  *   E N D   O F   R E P O R T    *  *  *"
    GOSUB WRITE.CCDET.REPORT

    IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                                !1.9JAT
       CCSMY.REPORT.LINE$ = PAGE.THROW$
       GOSUB WRITE.CCSMY.REPORT
    ENDIF                                                                    !1.9JAT

    CCDET.REPORT.LINE$ = PAGE.THROW$
    GOSUB WRITE.CCDET.REPORT
RETURN

\*****************************************************************************
\*****************************************************************************
\* CCSMY.START.NEW.PAGE:                                                     *
\*       write new page                                                      *
\*       add 1 to page number                                                *
\*       format page headers and write to CCSMY                              *
\*       first page only - write header text to report                       *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
CCSMY.START.NEW.PAGE:

IF CREDIT.CLAIM.ACTIVE = FALSE THEN BEGIN                                    !1.9JAT
    CCSMY.LINE.NO% = 0
    CCSMY.REPORT.LINE$ = PAGE.THROW$
    GOSUB WRITE.CCSMY.REPORT
    CCSMY.PAGE.NO% = CCSMY.PAGE.NO% + 1
    CCSMY.PAGE.NO$ = RIGHT$("  " + STR$(CCSMY.PAGE.NO%),2)

    CCSMY.REPORT.LINE$ = PRINT.REPORT.NO$ +                                  \
                          STRING$(50," ") +                                  \
                      PRINT.TODAYS.DATE$ + " " +                             \
                      PRINT.TIME$
    GOSUB WRITE.CCSMY.REPORT

    CCSMY.REPORT.LINE$ = STRING$(11," ") + TITLE$ + " " + PRINT.WC.DATE$ + \ !1.6AH 4.2HSM
                         STORE.NUMBER$                                       !4.2HSM
    GOSUB WRITE.CCSMY.REPORT                                                 !4.2HSM
    CCSMY.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCSMY.REPORT

    CCSMY.REPORT.LINE$ = STRING$(26," ") +                                   \
                          "S U M M A R Y" +                                  \
                      STRING$(23," ") +                                      \
                      "Page " +                                              \
                      CCSMY.PAGE.NO$ +                                       \
                      " of "       +                                         \
                      PAGE.NUM.MARKER$
    GOSUB WRITE.CCSMY.REPORT


    IF CCSMY.PAGE.NO% = 1 THEN BEGIN
        CCSMY.REPORT.LINE$ = BLANK.LINE$
        GOSUB WRITE.CCSMY.REPORT
       CCSMY.REPORT.LINE$ = " The following " + UNIT$ + " activities"+     \         !1.6AH
                          " took place last week. If more "
        GOSUB WRITE.CCSMY.REPORT
       CCSMY.REPORT.LINE$ = " information is required request line" +        \
                          " level report from EPOS controller. "
        GOSUB WRITE.CCSMY.REPORT
    ENDIF

    CCSMY.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCSMY.REPORT
    GOSUB WRITE.CCSMY.REPORT

ENDIF                                                                        !1.9JAT
RETURN

\*****************************************************************************
\*****************************************************************************
\* CCDET.START.NEW.PAGE:                                                     *
\*       write new page                                                      *
\*       add 1 to page number                                                *
\*       format page headers and write to CCSMY                              *
\*       first page only - write header text to report                       *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
CCDET.START.NEW.PAGE:
    CCDET.LINE.NO% = 0
    CCDET.REPORT.LINE$ = PAGE.THROW$
    GOSUB WRITE.CCDET.REPORT
    CCDET.PAGE.NO% = CCDET.PAGE.NO% + 1
    CCDET.PAGE.NO$ = RIGHT$("  " + STR$(CCDET.PAGE.NO%),2)

    CCDET.REPORT.LINE$ = PRINT.REPORT.NO$ +                                  \
                          STRING$(50," ") +                                  \
                      PRINT.TODAYS.DATE$ + " " +                             \
                      PRINT.TIME$
    GOSUB WRITE.CCDET.REPORT

    CCDET.REPORT.LINE$ = STRING$(11," ") + TITLE$ + " " + PRINT.WC.DATE$ + \ !1.6AH  4.2HSM
                         STORE.NUMBER$                                       !4.2HSM

    GOSUB WRITE.CCDET.REPORT

    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT

    CCDET.REPORT.LINE$ = STRING$(28," ") +                                   \
                          "DETAILED REPORT" +                                \
                      STRING$(19," ") +                                      \
                      "Page " +                                              \
                      CCDET.PAGE.NO$ +                                       \
                      " of "       +                                         \
                      PAGE.NUM.MARKER$
    GOSUB WRITE.CCDET.REPORT

    IF CCDET.PAGE.NO% = 1 THEN BEGIN
        CCDET.REPORT.LINE$ = BLANK.LINE$
        GOSUB WRITE.CCDET.REPORT
       CCDET.REPORT.LINE$ = " The following " + DAT$ + " last week."            !1.6AH
        GOSUB WRITE.CCDET.REPORT
    ENDIF

    CCDET.REPORT.LINE$ = BLANK.LINE$
    GOSUB WRITE.CCDET.REPORT
    GOSUB WRITE.CCDET.REPORT
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.RPT.NUM.OF.PAGES:                                                     *
\*    This routine replaces the 'page number marker' in the page headers     *
\*    for CCSMY and CCDET with the number of pages for that report.          *
\*                                                                           *
\*    close CCSMY file                                                       *
\*    open CCSMY Direct, record length 84                                    *
\*    repeat until End of CCSMY Report                                       *
\*        read CCSMY next report line                                        *
\*             check for End of CCSMY Report                                 *
\*             if page number marker is present on record line               *
\*                 replace page number marker with Number of Pages for CCSMY *
\*                 write CCSMY record                                        *
\*             endif                                                         *
\*    end repeat                                                             *
\*                                                                           *
\*    repeat the above process for CCDET report                              *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.RPT.NUM.OF.PAGES:
    IF CCSMY.OPEN THEN BEGIN
        CLOSE CCSMY.SESS.NUM%
       CCSMY.OPEN = FALSE
    ENDIF
    GOSUB OPEN.CCSMY.DIRECT
    END.OF.REPORT = FALSE
    CCSMY.LINE.NO% = 1
    NUM.OF.PAGES$ = RIGHT$("  " + STR$(CCSMY.PAGE.NO%),2)
    WHILE NOT END.OF.REPORT
        IF END #CCSMY.SESS.NUM% THEN END.OF.CCSMY.REPORT
       READ FORM "C84"; #CCSMY.SESS.NUM%, CCSMY.LINE.NO%;                    \
               CCSMY.REPORT.LINE$
       IF MID$(CCSMY.REPORT.LINE$,75,4) = PAGE.NUM.MARKER$ THEN BEGIN
           TEMP$ = LEFT$(CCSMY.REPORT.LINE$,74)
           CCSMY.REPORT.LINE$ = TEMP$ +                                      \
                                 NUM.OF.PAGES$ +                             \
                             STRING$(5," ") +                                \
                             CHR$(34) + CHR$(13) + CHR$(10)
           GOSUB REWRITE.CCSMY.REPORT.LINE
       ENDIF
       CCSMY.LINE.NO% = CCSMY.LINE.NO% + 1
       END.CCSMY.RECORD:
    WEND

    IF CCDET.OPEN THEN BEGIN
        CLOSE CCDET.SESS.NUM%
       CCDET.OPEN = FALSE
    ENDIF
    GOSUB OPEN.CCDET.DIRECT
    END.OF.REPORT = FALSE
    CCDET.LINE.NO% = 1
    NUM.OF.PAGES$ = RIGHT$("  " + STR$(CCDET.PAGE.NO%),2)
    WHILE NOT END.OF.REPORT
        IF END #CCDET.SESS.NUM% THEN END.OF.CCDET.REPORT
       READ FORM "C84"; #CCDET.SESS.NUM%, CCDET.LINE.NO%;                    \
               CCDET.REPORT.LINE$
       IF MID$(CCDET.REPORT.LINE$,75,4) = PAGE.NUM.MARKER$ THEN BEGIN
           TEMP$ = LEFT$(CCDET.REPORT.LINE$,74)
           CCDET.REPORT.LINE$ = TEMP$ +                                      \
                                 NUM.OF.PAGES$ +                             \
                             STRING$(5," ") +                                \
                             CHR$(34) + CHR$(13) + CHR$(10)
           GOSUB REWRITE.CCDET.REPORT.LINE
       ENDIF
       CCDET.LINE.NO% = CCDET.LINE.NO% + 1
       END.CCDET.RECORD:
    WEND

RETURN

END.OF.CCSMY.REPORT:
    END.OF.REPORT = TRUE
    GOTO END.CCSMY.RECORD

END.OF.CCDET.REPORT:
    END.OF.REPORT = TRUE
    GOTO END.CCDET.RECORD

\*****************************************************************************
\*****************************************************************************
\* FORMAT.VALUE:                                                             *
\*       formats an integer monetary value into a character string for       *
\*       printing on reports.                                                *
\*       formated string includes decimal point for œ display and leading    *
\*       spaces.                                                             *
\*       total string length is 9 chars.                                     *
\*       input: value$ = str$(value.integer%)                                *
\*       output: value$ = formatted string                                   *
\*                                                                           *
\*       examples:                                                           *
\*                                                                           *
\*       input; value$ = str$(0)           output; value$ = "     0.00"      *
\*                                                                           *
\*       input; value$ = str$(2599)  output; value$ = "    25.99"            *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
FORMAT.VALUE:
       IF DECIMAL.PLACES% = 2 THEN BEGIN                         ! 1.5AH
          IF VALUE$ = "0" THEN BEGIN
                 TEMP$ = "0" + DIVIDER$ + "00"                   ! 1.5AH
                 GOTO FORMAT.VALUE.COMPLETE
          ENDIF                                                  ! 1.5AH
       ENDIF ELSE BEGIN                                          ! 1.5AH
          IF VALUE$ = "0" THEN BEGIN                             ! 1.5AH
                 TEMP$ = RIGHT$("00" + VALUE$,3)                 ! 1.5AH
                 GOTO FORMAT.VALUE.COMPLETE                      ! 1.5AH
          ENDIF                                                  ! 1.5AH
       ENDIF


       L% = LEN(VALUE$)
       IF DECIMAL.PLACES% = 2 THEN BEGIN                         ! 1.5AH
          IF LEN(VALUE$) = 1 THEN BEGIN
             TEMP$ = "0" + DIVIDER$ + "0" + VALUE$               ! 1.5AH
          ENDIF ELSE BEGIN
             IF LEN(VALUE$) = 2 THEN BEGIN
                TEMP$ = "0" + DIVIDER$ + VALUE$                  ! 1.5AH
             ENDIF ELSE BEGIN
                TEMP$ = LEFT$(VALUE$,L%-2) + DIVIDER$ + RIGHT$(VALUE$,2)   ! 1.5AH
             ENDIF
          ENDIF
       ENDIF ELSE BEGIN                                          ! 1.5AH
          TEMP$ = VALUE$                                         ! 1.5AH
       ENDIF                                                     ! 1.5AH

       FORMAT.VALUE.COMPLETE:
       VALUE$ = RIGHT$( STRING$(6," ") + TEMP$, 9)
       VALUE.NO.SPACES$ = TEMP$
RETURN

\*****************************************************************************
\*****************************************************************************
\* FORMAT.WC.DATE:                                                           *
\*       find date of Sunday last week                                       *      !4.1BMG
\*       if last Sunday date > date of last report run find date for Sunday  *      !4.1BMG
\*       prior to that, and so on until Sunday found is not > last report    *      !4.1BMG
\*       run date - this is the week/commencing date for the current reports *
\*       format WC date for printing on reports                              *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
FORMAT.WC.DATE:
       F13.DAY$ = ""
       F02.DATE$ = TODAYS.DATE$
       INCREMENT% = -7
       WHILE F13.DAY$ <> "SUN"                                          !4.1BMG
              RC% = UPDATE.DATE(INCREMENT%)
              IF RC% <> 0 THEN BEGIN
                     PROGRAM.FAIL = TRUE
                     GOTO PROGRAM.EXIT
              ENDIF
              RC% = PSDATE(F02.DATE$)
              IF RC% <> 0 THEN BEGIN
                     PROGRAM.FAIL = TRUE
                     GOTO PROGRAM.EXIT
              ENDIF
              INCREMENT% = -1
       WEND
       INCREMENT% = -7
        IF LAST.CREDIT.RUN.DATE$ <> "000000" THEN BEGIN                 ! CMJK
         WHILE DATE.GT (F02.DATE$,LAST.CREDIT.RUN.DATE$)                !1.4RD
              RC% = UPDATE.DATE(INCREMENT%)
              IF RC% <> 0 THEN BEGIN
                     PROGRAM.FAIL = TRUE
                     GOTO PROGRAM.EXIT
              ENDIF
         WEND
       ENDIF ELSE BEGIN                                                 ! CMJK
           RC% = UPDATE.DATE(INCREMENT%)                                ! CMJK
         IF RC% <> 0 THEN BEGIN                                         ! CMJK
              PROGRAM.FAIL = TRUE                                       ! CMJK
              GOTO PROGRAM.EXIT                                         ! CMJK
         ENDIF                                                          ! CMJK
       ENDIF                                                            ! CMJK
       WC.DATE$ = F02.DATE$
       PRINT.WC.DATE$ = MID$(WC.DATE$,5,2) + "/" +                           \
                       MID$(WC.DATE$,3,2) + "/" +                            \
                       MID$(WC.DATE$,1,2)
RETURN

\*****************************************************************************
\*****************************************************************************
\* GET.LINES.PER.PAGE:                                                       *
\*       call ADXSERVE to determine the number of lines per page for reports *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
GET.LINES.PER.PAGE:
    CALL ADXSERVE(RC%,4,0,ADX.PARM.2$)
    IF RC% <> 0 THEN BEGIN
        LOG.EVENT.NO% = 24
       LOG.STRING.1.UNIQUE$ = STR$(RC%)
       GOSUB LOG.AN.EVENT
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT
    ENDIF
    LINES.PER.PAGE% = VAL(MID$(ADX.PARM.2$,20,3))
RETURN

\*****************************************************************************
\*****************************************************************************
\* LEFT.JUSTIFY.FIELD:                                                       *
\*           This routine strips away leading spaces from a character string,*
\*       thereby making it left justified.                                   *
\*                                                                           *
\*       input : field$ = char.string$                                       *
\*       output: field$                                                      *
\*                                                                           *
\*       eg. input : field$ = "   Hello"                                     *
\*           output: field$ = "Hello"                                        *
\*                                                                           *
\*       If field$ input is null or spaces then no action is taken on field$ *
\*       ie. returned value will be null or spaces                           *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
LEFT.JUSTIFY.FIELD:
    IF LEN(FIELD$) > 0 THEN BEGIN
        CHAR.FOUND = FALSE
        CHAR.POS% = 1
       WHILE NOT CHAR.FOUND = TRUE                                           \
         AND NOT (CHAR.POS% > LEN(FIELD$))
           IF MID$(FIELD$,CHAR.POS%,1) <> " " THEN BEGIN
               CHAR.FOUND = TRUE
           ENDIF ELSE BEGIN
               CHAR.POS% = CHAR.POS% +1
           ENDIF
       WEND
       IF CHAR.FOUND THEN BEGIN
             TEMP$ = RIGHT$(FIELD$,( LEN(FIELD$) - (CHAR.POS% - 1) ) )
           FIELD$ = TEMP$
        ENDIF
    ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* CLAIMS.HOUSEKEEPING:                                                      *
\*   GOSUB OPEN.CCLAM.KEYED                                                  *
\*    for each claim held in the delete file;                                *
\*        for each item associated with the claim;                           *
\*              delete CCITF Item record by key                              *
\*         delete CCLAM Claim record by key                                  *
\*         CLOSE CCLAM.SESS.NUM%                                             *
\*         CCLAM.OPEN = FALSE                                                *
\*    Only set UPDATE.CCTRL.LAST.DATE to TRUE if Reporting Required    !1.3JAS
\*    (CCTRL record re-written at finalise program)                    !1.3JAS
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
CLAIMS.HOUSEKEEPING:
  GOSUB OPEN.CCLAM.KEYED                                               !1.3JAS
    FOR DEL.TAB.IDX% = 1 TO DEL.TAB.NO.ENTRIES% STEP 1
       CCLAM.CREDIT.CLAIM.NUM$ = LEFT$(DELETE.TABLE$(DEL.TAB.IDX%),4)
       CCITF.CREDIT.CLAIM.NUM$ = LEFT$(DELETE.TABLE$(DEL.TAB.IDX%),4)
       CCLAM.NUM.OF.ITEMS% = VAL(RIGHT$(DELETE.TABLE$(DEL.TAB.IDX%),4))

       FOR ITEM.NO% = 1 TO CCLAM.NUM.OF.ITEMS% STEP 1
           CCITF.ITEM.NUM$ = PACK$(RIGHT$("0000" + STR$(ITEM.NO%),4))
           CCITF.KEY$ = CCITF.CREDIT.CLAIM.NUM$ + CCITF.ITEM.NUM$
           GOSUB DELREC.CCITF
       NEXT ITEM.NO%

       GOSUB DELREC.CCLAM

    NEXT DEL.TAB.IDX%

    CLOSE CCLAM.SESS.NUM%                                              !1.3JAS
    CCLAM.OPEN = FALSE                                                 !1.3JAS

    IF REPORTING.REQUIRED THEN BEGIN                                   !1.3JAS
       UPDATE.CCTRL.LAST.DATE = TRUE
    ENDIF ELSE BEGIN                                                   !1.3JAS
          UPDATE.CCTRL.LAST.DATE = FALSE                               !1.3JAS
    ENDIF                                                              !1.3JAS
RETURN

\*****************************************************************************
\*****************************************************************************
\* UPDATE.CCTRL:                                                             *
\*     read CCTRL record (locked)                                            *
\*     set Staff Sales to 0                                                  *
\*     if Update CCTRL Last Date = true                                      *
\*         set CCTRL Last Date = Today's Date                                *
\*     endif                                                                 *
\*     write CCTRL (unlock)                                                  *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
UPDATE.CCTRL:
       CCTRL.REC.NUM% = 1
       RC% = READ.CCTRL.LOCKED
       IF RC% <> 0 THEN BEGIN
              GOSUB READ.FILE.ERROR
       ENDIF
       CCTRL.STAFF.SALES% = 0
       IF UPDATE.CCTRL.LAST.DATE THEN BEGIN
              CCTRL.CREDIT.RPT.RUN.DATE$ = PACK$(TODAYS.DATE$)
       ENDIF
       RC% = WRITE.UNLOCK.CCTRL
       IF RC% <> 0 THEN BEGIN
              GOSUB WRITE.FILE.ERROR
       ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* OPEN.FILES:                                                               *
\*                                                                           *
\*      open IDF        KEYED            NOWRITE  NODEL                      *
\*      open SOFTS       DIRECT           NOWRITE  NODEL                     *
\*      open BCSMF       DIRECT           NOWRITE  NODEL                     *
\*      open CCITF       KEYED                                               *
\*      open CCTRL       DIRECT                                              *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.FILES:

       CURRENT.REPORT.NUM% = IDF.REPORT.NUM%
       IF END #IDF.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM%             \
       NOWRITE NODEL


       CURRENT.REPORT.NUM% = SOFTS.REPORT.NUM%
       IF END #SOFTS.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL% AS SOFTS.SESS.NUM%      \
       NOWRITE NODEL

       CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
       IF END #BCSMF.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN BCSMF.FILE.NAME$ DIRECT RECL SECTOR.SIZE% AS BCSMF.SESS.NUM%     \
       NOWRITE NODEL

       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       IF END #CCITF.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCITF.FILE.NAME$ KEYED  RECL CCITF.RECL% AS CCITF.SESS.NUM%

       CURRENT.REPORT.NUM% = CCTRL.REPORT.NUM%
       IF END #CCTRL.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCTRL.FILE.NAME$ DIRECT RECL CCTRL.RECL% AS CCTRL.SESS.NUM%
RETURN

\*****************************************************************************
\*****************************************************************************
\* OPEN.CCRSN:                                                               *
\*                                                                           *
\*      open CCRSN       KEYED            NOWRITE NODEL                      *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.CCRSN:

       IF CCRSN.OPEN = FALSE THEN BEGIN                                     !1.9JAT

          CURRENT.REPORT.NUM% = CCRSN.REPORT.NUM%
          IF END #CCRSN.SESS.NUM% THEN OPEN.FILE.ERROR
          OPEN CCRSN.FILE.NAME$ KEYED  RECL CCRSN.RECL% AS CCRSN.SESS.NUM%      \
          NOWRITE NODEL
          CCRSN.OPEN = TRUE

       ENDIF                                                                !1.9JAT
RETURN

\*****************************************************************************
\*****************************************************************************
\* READ.BCSMF.SECTOR:                                                        *
\*      read one sector of data from BCSMF                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
READ.BCSMF.SECTOR:
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BCSMF.REPORT.NUM%
       IF END #BCSMF.SESS.NUM% THEN READ.FILE.ERROR
       READ FORM "C4,C508"; #BCSMF.SESS.NUM%, SECTOR.NO%;                    \
                          SECTOR.FILLER$, BCSMF.SECTOR$
RETURN

\*****************************************************************************
\*****************************************************************************
\* OPEN.CCLAM.DIRECT:                                                        *
\*        open CCLAM      DIRECT     (RECL = Sector Size)                    *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.CCLAM.DIRECT:
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       IF END #CCLAM.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCLAM.FILE.NAME$ DIRECT RECL SECTOR.SIZE% AS CCLAM.SESS.NUM%
       CCLAM.OPEN = TRUE
RETURN

\*****************************************************************************
\*****************************************************************************
\* OPEN.CCLAM.KEYED:                                                         *
\*        open CCLAM      KEYED                                              *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.CCLAM.KEYED:
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       IF END #CCLAM.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCLAM.FILE.NAME$ KEYED RECL CCLAM.RECL% AS CCLAM.SESS.NUM%
       CCLAM.OPEN = TRUE
RETURN

\*****************************************************************************
\*****************************************************************************
\* READ.CCLAM.SECTOR:                                                        *
\*      read one sector of data from CCLAM                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
READ.CCLAM.SECTOR:
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       IF END #CCLAM.SESS.NUM% THEN READ.FILE.ERROR
       READ FORM "C4,C508"; #CCLAM.SESS.NUM%, SECTOR.NO%;                    \
                          SECTOR.FILLER$, CCLAM.SECTOR$
RETURN

\*****************************************************************************
\*****************************************************************************
\* RET.CREATE.REPORT.FILES:                                                  *
\*       Create new CCSMY Report File                                        *
\*       Create new CCDET Report File                                        *
\*       If error then gosub CREATE.FILE.ERROR                               *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
RET.CREATE.REPORT.FILES:
       CURRENT.REPORT.NUM% = CCSMY.REPORT.NUM%
       IF END# CCSMY.SESS.NUM% THEN CREATE.FILE.ERROR
       CREATE POSFILE CCSMY.FILE.NAME$ AS CCSMY.SESS.NUM% MIRRORED ATCLOSE
       CCSMY.OPEN = TRUE

       CURRENT.REPORT.NUM% = CCDET.REPORT.NUM%
       IF END# CCDET.SESS.NUM% THEN CREATE.FILE.ERROR
       CREATE POSFILE CCDET.FILE.NAME$ AS CCDET.SESS.NUM% MIRRORED ATCLOSE
       CCDET.OPEN = TRUE
RETURN


\*****************************************************************************
\*****************************************************************************
\* OPEN.CCSMY.DIRECT:                                                        *
\*        open CCSMY      DIRECT     (RECL = Report Record size = 84)        *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.CCSMY.DIRECT:
       CURRENT.REPORT.NUM% = CCSMY.REPORT.NUM%
       IF END #CCSMY.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCSMY.FILE.NAME$ DIRECT RECL REPORT.REC.SIZE% AS CCSMY.SESS.NUM%
       CCSMY.OPEN = TRUE
RETURN

\*****************************************************************************
\*****************************************************************************
\* OPEN.CCDET.DIRECT:                                                        *
\*        open CCDET      DIRECT     (RECL = Report Record size = 84)        *
\*      if open error then OPEN.FILE.ERROR                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.CCDET.DIRECT:
       CURRENT.REPORT.NUM% = CCDET.REPORT.NUM%
       IF END #CCDET.SESS.NUM% THEN OPEN.FILE.ERROR
       OPEN CCDET.FILE.NAME$ DIRECT RECL REPORT.REC.SIZE% AS CCDET.SESS.NUM%
       CCDET.OPEN = TRUE
RETURN


\*****************************************************************************
\*****************************************************************************
\* WRITE.CCSMY.REPORT:                                                       *
\*       edit CCSMY report line to 80 characters                             *
\*       write CCSMY report line                                             *
\*       if error gosub WRITE.FILE.ERROR                                     *
\*       add 1 to CCSMY Line No.                                             *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
WRITE.CCSMY.REPORT:
        CCSMY.REPORT.LINE$ = LEFT$(CCSMY.REPORT.LINE$ + STRING$(80," "),80)
       RC% = WRITE.CCSMY
       IF RC% <> 0 THEN BEGIN
              GOSUB WRITE.FILE.ERROR
       ENDIF
       CCSMY.LINE.NO% = CCSMY.LINE.NO% + 1
RETURN

\*****************************************************************************
\*****************************************************************************
\* REWRITE.CCSMY.REPORT.LINE:                                                *
\*       write CCSMY report line                                             *
\*       if error gosub WRITE.FILE.ERROR                                     *
\* RETURN                                                                    *
\*****************************************************************************
REWRITE.CCSMY.REPORT.LINE:
    CURRENT.REPORT.NUM%= CCSMY.REPORT.NUM%
    FILE.OPERATION$ = "W"
    IF END #CCSMY.SESS.NUM% THEN WRITE.FILE.ERROR
    WRITE FORM "C84"; #CCSMY.SESS.NUM%, CCSMY.LINE.NO%;                      \
          CCSMY.REPORT.LINE$
RETURN

\*****************************************************************************
\*****************************************************************************
\* WRITE.CCDET.REPORT:                                                       *
\*       edit CCDET report line to 80 characters                             *
\*       write CCDET report line                                             *
\*       if error gosub WRITE.FILE.ERROR                                     *
\*       add 1 to CCDET Line No.                                             *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
WRITE.CCDET.REPORT:
        CCDET.REPORT.LINE$ = LEFT$(CCDET.REPORT.LINE$ + STRING$(80," "),80)
       RC% = WRITE.CCDET
       IF RC% <> 0 THEN BEGIN
              GOSUB WRITE.FILE.ERROR
       ENDIF
       CCDET.LINE.NO% = CCDET.LINE.NO% + 1
RETURN

\*****************************************************************************
\*****************************************************************************
\* REWRITE.CCDET.REPORT.LINE:                                                *
\*       write CCDET report line                                             *
\*       if error gosub WRITE.FILE.ERROR                                     *
\* RETURN                                                                    *
\*****************************************************************************
REWRITE.CCDET.REPORT.LINE:
    CURRENT.REPORT.NUM%= CCDET.REPORT.NUM%
    FILE.OPERATION$ = "W"
    IF END #CCDET.SESS.NUM% THEN WRITE.FILE.ERROR
    WRITE FORM "C84"; #CCDET.SESS.NUM%, CCDET.LINE.NO%;                      \
          CCDET.REPORT.LINE$
RETURN

\*****************************************************************************
\*****************************************************************************
\* DELREC.CCITF:                                                             *
\*       delete record on CCITF File by key                                  *
\*       If error then branch to subroutine return                           *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
DELREC.CCITF:
       CURRENT.REPORT.NUM% = CCITF.REPORT.NUM%
       IF END# CCITF.SESS.NUM% THEN CCITF.REC.ABSENT                     !CMJK
       DELREC CCITF.SESS.NUM% ; CCITF.KEY$
       CCITF.REC.ABSENT:                                                 !CMJK
RETURN

\*****************************************************************************
\*****************************************************************************
\* DELREC.CCLAM:                                                             *
\*       delete record on CCLAM File by key                                  *
\*       If error then branch to subroutine return                           *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
DELREC.CCLAM:
       CURRENT.REPORT.NUM% = CCLAM.REPORT.NUM%
       IF END# CCLAM.SESS.NUM% THEN CCLAM.REC.ABSENT                     !CMJK
       DELREC CCLAM.SESS.NUM% ; CCLAM.CREDIT.CLAIM.NUM$
       CCLAM.REC.ABSENT:                                                 !CMJK
RETURN


\*****************************************************************************
\***                                                                         *
\*** UPDATE.CLAIMS.TABLE 1.9JAT                                              *
\***                                                                         *
\*****************************************************************************

UPDATE.CLAIMS.TABLE:                                                          !1.9JAT

      GOSUB OPEN.CCRSN

      IF CCLAM.REASON.NUM$ <> PACK$("00") THEN BEGIN                          !1.9JAT
         CCRSN.REASON$ = CCLAM.REASON.NUM$                                    !1.9JAT
         RC% = READ.CCRSN                                                     !1.9JAT
         IF RC% <> 0 THEN BEGIN                                               !1.9JAT
            CCRSN.DESC$ = "NOT ON FILE                   "                    !1.9JAT
         ENDIF                                                                !1.9JAT
      ENDIF

      IF VAL(UNPACK$(CCRSN.ALTERNATE.REASON$))>HIGHEST.REASON.CODE% THEN BEGIN!1.9JAT
         HIGHEST.REASON.CODE% = VAL(UNPACK$(CCRSN.ALTERNATE.REASON$))         !1.9JAT
      ENDIF

      IF LEFT$(CCLAM.FILLER$,7)  = STRING$(7," ") THEN BEGIN                  !4.2HSM
          NEW.CLAIM.RECORDS$(CLAIMS.COUNTER%) = CCLAM.CREDIT.CLAIM.NUM$ +     \1.9JAT
                                                CCLAM.REASON.NUM$       +     \1.9JAT
                                                CCRSN.DESC$             +     \1.9JAT
                                                CCRSN.ALTERNATE.REASON$ +     \1.9JAT
                                                "00000000"                    !1.9JAT
      ENDIF ELSE BEGIN
          NEW.CLAIM.RECORDS$(CLAIMS.COUNTER%) = CCLAM.CREDIT.CLAIM.NUM$ +     \4.2HSM
                                                CCLAM.REASON.NUM$       +     \4.2HSM
                                                CCRSN.DESC$             +     \4.2HSM
                                                CCRSN.ALTERNATE.REASON$ +     \4.2HSM
                                                "00000000"              +     \4.2HSM
                                                MID$(CCLAM.FILLER$,1,6)       !4.2HSM
      ENDIF                                                                            
      CLAIMS.COUNTER% = CLAIMS.COUNTER% + 1                                   !1.9JAT

RETURN                                                                        !1.9JAT

\*****************************************************************************
\***                                                                         *
\*** CREATE.NEW.CCSMY  1.9JAT                                                *
\***                                                                         *
\*** Use the data stored in NEW.CLAIM.RECORDS$ to create the new style       *
\*** CCSMY report.                                                           *
\***                                                                         *
\*****************************************************************************

CREATE.NEW.CCSMY:                                                             !1.9JAT

    !Sort the claim table by claim number                                     !1.9JAT
    DIM F14.TABLE$(CLAIMS.COUNTER%)                                           !1.9JAT

    FOR A% = 1 TO CLAIMS.COUNTER%                                             !1.9JAT
        F14.TABLE$ (A%) = NEW.CLAIM.RECORDS$(A%)                              !1.9JAT
    NEXT A%                                                                   !1.9JAT

    RC% = SORT.TABLE(CLAIMS.COUNTER%)                                         !1.9JAT
    FOR A% = 1 TO CLAIMS.COUNTER%                                             !1.9JAT
        NEW.CLAIM.RECORDS$(A%) = F14.TABLE$(A%)                               !1.9JAT
    NEXT A%                                                                   !1.9JAT

    DIM F14.TABLE$(0)                                                         !1.9JAT

    !Write the title to the CCSMY                                             !1.9JAT

    GOSUB WRITE.EXCEPTION.HEADER                                              !1.9JAT

    !Write exceptional claims to CCSMY                                        !1.9JAT
    FOR A% = 1 TO (CLAIMS.COUNTER% - 1)                                       !1.9JAT
       ! CURRENT.CLAIM.VALUE% = VAL(RIGHT$(NEW.CLAIM.RECORDS$(A%),8))          !1.9JAT
        CURRENT.CLAIM.VALUE%    = VAL(MID$(NEW.CLAIM.RECORDS$(A%),37,8))      !4.2HSM
        IF CURRENT.CLAIM.VALUE% >= CLAIM.EXCEPTION.VALUE% THEN BEGIN          !1.9JAT
           EXCEPTION.ON.REPORT = TRUE                                         !1.9JAT
           GOSUB WRITE.EXCEPTION.SEGMENT                                      !1.9JAT
        ENDIF                                                                 !1.9JAT
        CCRSN.ALTERNATE.REASON% = VAL(UNPACK$(MID$(NEW.CLAIM.RECORDS$(A%),36,1)))!1.9JAT
        CCRSN.REASON.NUM% = VAL(UNPACK$(MID$(NEW.CLAIM.RECORDS$(A%),5,1)))    !1.9JAT
        !Also build a table of reason codes for reporting later.              !1.9JAT
        IF LEFT$(NEW.REASON.CODE.RECORDS$(CCRSN.REASON.NUM%),1)               \1.9JAT
                                                               = "" THEN BEGIN!1.9JAT
           NEW.REASON.CODE.RECORDS$(CCRSN.REASON.NUM%) =                      \1.9JAT
                      RIGHT$("00" + STR$(CCRSN.ALTERNATE.REASON%),2)  +       \1.9JAT
                      RIGHT$("00" + STR$(CCRSN.REASON.NUM%),2)        +       \1.9JAT
                      MID$(NEW.CLAIM.RECORDS$(A%),6,30) + \ REASON DESC        1.9JAT                     
                      RIGHT$("00000000" + STR$(CURRENT.CLAIM.VALUE%),8)       !1.9JAT

        ENDIF ELSE BEGIN                                                      !1.9JAT
           TOTAL.CLAIM.VALUE% = VAL(RIGHT$(NEW.REASON.CODE.RECORDS$           \1.9JAT
                                          (CCRSN.REASON.NUM%),8))             !1.9JAT
           TOTAL.CLAIM.VALUE% = TOTAL.CLAIM.VALUE% + CURRENT.CLAIM.VALUE%     !1.9JAT

           NEW.REASON.CODE.RECORDS$(CCRSN.REASON.NUM%) = RIGHT$("00"  +       \1.9JAT
                      STR$(CCRSN.ALTERNATE.REASON%),2) +                      \1.9JAT
                      RIGHT$("00" + STR$(CCRSN.REASON.NUM%),2)        +       \1.9JAT
                      MID$(NEW.CLAIM.RECORDS$(A%),6,30) + \ REASON DESC        1.9JAT
                      RIGHT$("00000000" + STR$(TOTAL.CLAIM.VALUE%),8)         !1.9JAT

        ENDIF                                                                 !1.9JAT
    NEXT A%                                                                   !1.9JAT

    VALUE$ = STR$(CLAIM.EXCEPTION.VALUE%)                                     !1.9JAT
    GOSUB FORMAT.VALUE                                                        !1.9JAT

    IF EXCEPTION.HEADER.WRITTEN = FALSE THEN BEGIN                            !1.9JAT
         GOSUB WRITE.EXCEPTION.HEADER                                         !1.9JAT
    ENDIF                                                                     !1.9JAT

    !Write a trailer for the exceptional lines segment                        !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    IF EXCEPTION.ON.REPORT = TRUE THEN BEGIN                                  !1.9JAT
       CCSMY.REPORT.LINE$ = "All credit other than known theft will appear on the CN19 report" !1.9JAT
       GOSUB WRITE.CCSMY.REPORT                                               !1.9JAT
    ENDIF ELSE BEGIN                                                          !1.9JAT
       CCSMY.REPORT.LINE$ = "There are no credit claims above " +             \1.9JAT
                             CURRENCY.SYMBOL$                   +             \1.9JAT
                             VALUE.NO.SPACES$                                 !1.9JAT
       GOSUB WRITE.CCSMY.REPORT                                               !1.9JAT
    ENDIF


    !Write reason code summary to CCSMY
    GOSUB WRITE.REASON.CODE.SEGMENT                                           !1.9JAT
    !Write End of Report trailer to CCSMY
    CCSMY.REPORT.LINE$ = STRING$(13," ") +                                    \1.9JAT
                             "*  *  *   E N D   O F   R E P O R T    *  *  *" !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = PAGE.THROW$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    GOSUB RET.RPT.NUM.OF.PAGES                                                !1.9JAT

RETURN                                                                        !1.9JAT

\*****************************************************************************
\***                                                                         *
\*** WRITE.EXCEPTION.SEGMENT 1.9JAT                                          *
\***                                                                         *
\*** Any claims in excess of the exception value taken from SOFTS will be    *
\*** written to the CCSMY report (At time of writing, these values are       *
\*** GBP400 or Eur400 in ROI                                                 *
\***                                                                         *
\*****************************************************************************

WRITE.EXCEPTION.SEGMENT:                                                                !1.9JAT
                                                                                   
      IF EXCEPTION.HEADER.WRITTEN = FALSE THEN BEGIN                                    !1.9JAT
         GOSUB WRITE.EXCEPTION.HEADER                                                   !1.9JAT
      ENDIF                                                                             !1.9JAT
                                                                                   
      CCLAM.CREDIT.CLAIM.NUM$ = UNPACK$(LEFT$(NEW.CLAIM.RECORDS$(A%),4))                !1.9JAT
      CCRSN.DESC$             = MID$(NEW.CLAIM.RECORDS$(A%),6,30)                       !1.9JAT
      CCRSN.ALTERNATE.REASON$ = UNPACK$(MID$(NEW.CLAIM.RECORDS$(A%),36,1))              !1.9JAT
      !CURRENT.CLAIM.VALUE% = VAL(RIGHT$(NEW.CLAIM.RECORDS$(A%),8))                      !1.9JAT
      CURRENT.CLAIM.VALUE%    = VAL(MID$(NEW.CLAIM.RECORDS$(A%),37,8))                  !4.2HSM
      CURRENT.TILL.CLAIM$     = UNPACK$(MID$(NEW.CLAIM.RECORDS$(A%),45,6))              !4.2HSM

      VALUE$ = STR$(CURRENT.CLAIM.VALUE%)                                               !1.9JAT
      GOSUB FORMAT.VALUE                                                                !1.9JAT
                                                                                     
      CCSMY.REPORT.LINE$ = "Credit claim number : " + CCLAM.CREDIT.CLAIM.NUM$           !1.9JAT
      GOSUB WRITE.CCSMY.REPORT                                                          !1.9JAT                                    
      
      CCSMY.REPORT.LINE$ = "Transaction Number : " + MID$(CURRENT.TILL.CLAIM$,1,4) + \  !4.2HSM                        
                           " Till Number : " + MID$(CURRENT.TILL.CLAIM$,6,3)       + \  !4.2HSM
                           " Operator : " + MID$(CURRENT.TILL.CLAIM$,10,3)              !4.2HSM
      GOSUB WRITE.CCSMY.REPORT                                                          !4.2HSM 
      
      CCSMY.REPORT.LINE$ = "Reason Code : " + CCRSN.ALTERNATE.REASON$ + " " +       \   !1.9JAT
                                              CCRSN.DESC$                               !1.9JAT
      GOSUB WRITE.CCSMY.REPORT                                                          !1.9JAT
      CCSMY.REPORT.LINE$ = "Value : " + VALUE.NO.SPACES$                                !1.9JAT
      GOSUB WRITE.CCSMY.REPORT                                                          !1.9JAT
                                                                                    
      CCSMY.REPORT.LINE$ = BLANK.LINE$                                                  !1.9JAT
      GOSUB WRITE.CCSMY.REPORT                                                          !1.9JAT
                                                                                    
      IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 3 THEN BEGIN                          !1.9JAT
         GOSUB WRITE.EXCEPTION.HEADER                                                   !1.9JAT
      ENDIF                                                                             !1.9JAT
                                                                                     
RETURN


\*****************************************************************************
\***                                                                         *
\*** WRITE.REASON.CODE.SEGMENT 1.9JAT                                        *
\***                                                                         *
\*** Write a list of used Reason Codes to the CCSMY, along with the amount   *
\*** claimed against each code.                                              *
\*** Data is taken from the NEW.REASON.CODE.RECORDS$ table, loaded in sub-   *
\*** CREATE.NEW.CCSMY                                                        *
\*****************************************************************************

WRITE.REASON.CODE.SEGMENT:                                                    !1.9JAT


    !Sort the reason code table by Alternate Reason Code num                   !1.9JAT
    DIM F14.TABLE$(HIGHEST.REASON.CODE%)                                       !1.9JAT

    FOR A% = 1 TO HIGHEST.REASON.CODE%                                        !1.9JAT
        F14.TABLE$ (A%) = NEW.REASON.CODE.RECORDS$(A%)                        !1.9JAT
    NEXT A%                                                                   !1.9JAT

    RC% = SORT.TABLE(HIGHEST.REASON.CODE%)                                    !1.9JAT
    FOR A% = 1 TO HIGHEST.REASON.CODE%                                        !1.9JAT
        NEW.REASON.CODE.RECORDS$(A%) = F14.TABLE$(A%)                         !1.9JAT
    NEXT A%                                                                   !1.9JAT

    DIM F14.TABLE$(0)                                                         !1.9JAT

    GOSUB WRITE.REASON.CODE.HEADER                                            !1.9JAT

    FOR A% = 1 TO HIGHEST.REASON.CODE%                                        !1.9JAT
        IF LEFT$(NEW.REASON.CODE.RECORDS$(A%),1) <> "" THEN BEGIN             !1.9JAT
           REASON.CODE.ON.REPORT = TRUE                                       !1.9JAT
           CCRSN.ALTERNATE.REASON$ = LEFT$(NEW.REASON.CODE.RECORDS$(A%),2)    !1.9JAT
           CCRSN.DESC$ = MID$(NEW.REASON.CODE.RECORDS$(A%),5,30)              !1.9JAT
           TOTAL.CLAIM.VALUE% = VAL(RIGHT$(NEW.REASON.CODE.RECORDS$(A%),8))   !1.9JAT
           VALUE$ = STR$(TOTAL.CLAIM.VALUE%)                                  !1.9JAT
           GOSUB FORMAT.VALUE                                                 !1.9JAT

           CCSMY.REPORT.LINE$ = "Reason code " + CCRSN.ALTERNATE.REASON$ + " "\1.9JAT
                                + CCRSN.DESC$ + ": " + VALUE.NO.SPACES$       !1.9JAT
           GOSUB WRITE.CCSMY.REPORT                                           !1.9JAT
           CCSMY.REPORT.LINE$ = BLANK.LINE$                                   !1.9JAT
           GOSUB WRITE.CCSMY.REPORT                                           !1.9JAT
           IF LINES.PER.PAGE% - (CCSMY.LINE.NO% - 1) < 2 THEN BEGIN           !1.9JAT
              GOSUB WRITE.REASON.CODE.HEADER                                  !1.9JAT
           ENDIF                                                              !1.9JAT
        ENDIF                                                                 !1.9JAT
    NEXT A%                                                                   !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    IF REASON.CODE.ON.REPORT = FALSE THEN BEGIN                               !1.9JAT
       CCSMY.REPORT.LINE$ = "There are no credit claims to report"            !1.9JAT
       GOSUB WRITE.CCSMY.REPORT                                               !1.9JAT
    ENDIF                                                                     !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

RETURN

\*****************************************************************************
\***                                                                         *
\*** WRITE.EXCEPTION.HEADER  1.9JAT                                          *
\***                                                                         *
\*** Write the header details to the exception section of the CCSMY          *
\***                                                                         *
\*****************************************************************************

   WRITE.EXCEPTION.HEADER:

    CCSMY.LINE.NO% = 0                                                        !1.9JAT
    CCSMY.REPORT.LINE$ = PAGE.THROW$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.PAGE.NO% = CCSMY.PAGE.NO% + 1                                       !1.9JAT
    CCSMY.PAGE.NO$ = RIGHT$("  " + STR$(CCSMY.PAGE.NO%),2)                    !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    CCSMY.REPORT.LINE$ = PRINT.REPORT.NO$ +                                   \1.9JAT
                         STRING$(50," ") +                                    \1.9JAT
                         PRINT.TODAYS.DATE$ + " " +                           \1.9JAT
                         PRINT.TIME$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = STRING$(11," ") + TITLE$ + " " + PRINT.WC.DATE$ + \  !1.9JAT
                         STORE.NUMBER$                                        !4.2HSM       
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = STRING$(62," ") +                                    \1.9JAT
                         "Page " +                                            \1.9JAT
                          CCSMY.PAGE.NO$ +                                    \1.9JAT
                          " of "       +                                      \1.9JAT
                          PAGE.NUM.MARKER$                                    !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

   VALUE$ = STR$(CLAIM.EXCEPTION.VALUE%)                                      !1.9JAT
   GOSUB FORMAT.VALUE                                                         !1.9JAT
   CCSMY.REPORT.LINE$ = "CREDIT CLAIMING VALUES BELOW " +                     \1.0JAT
                         CURRENCY.SYMBOL$               +                     \1.9JAT
                         VALUE.NO.SPACES$               +                     \1.9JAT
                         " - DO NOT RECONCILE"                                !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    EXCEPTION.HEADER.WRITTEN = TRUE                                           !1.9JAT

    RETURN

\*****************************************************************************
\***                                                                         *
\*** WRITE.REASON.CODE.HEADER  1.9JAT                                        *
\***                                                                         *
\*** Write the header details to the reason code section                     *
\***                                                                         *
\*****************************************************************************

   WRITE.REASON.CODE.HEADER:

    CCSMY.LINE.NO% = 0                                                        !1.9JAT
    CCSMY.REPORT.LINE$ = PAGE.THROW$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.PAGE.NO% = CCSMY.PAGE.NO% + 1                                       !1.9JAT
    CCSMY.PAGE.NO$ = RIGHT$("  " + STR$(CCSMY.PAGE.NO%),2)                    !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    CCSMY.REPORT.LINE$ = PRINT.REPORT.NO$ +                                   \1.9JAT
                         STRING$(50," ") +                                    \1.9JAT
                         PRINT.TODAYS.DATE$ + " " +                           \1.9JAT
                         PRINT.TIME$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = STRING$(11," ") + TITLE$ + " " + PRINT.WC.DATE$ + \  !1.9JAT    
                         STORE.NUMBER$                                        !4.2HSM    
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = STRING$(62," ") +                                    \1.9JAT
                         "Page " +                                            \1.9JAT
                          CCSMY.PAGE.NO$ +                                    \1.9JAT
                          " of "       +                                      \1.9JAT
                          PAGE.NUM.MARKER$                                    !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    CCSMY.REPORT.LINE$ = "TOTAL CLAIMED AGAINST EACH REASON CODE - DO NOT RECONCILE"!1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT
    CCSMY.REPORT.LINE$ = BLANK.LINE$                                          !1.9JAT
    GOSUB WRITE.CCSMY.REPORT                                                  !1.9JAT

    RETURN


\*****************************************************************************
\***                                                                         *
\*** PROCESS.CURRENT.TILL.CLAIM  4.2 HSM                                     *
\***                                                                         *
\*** Keyreads the CCITF file to for Till Claims and writes them to the CCDET *
\*** report.                                                                 *
\***                                                                         *
\*****************************************************************************

PROCESS.CURRENT.TILL.CLAIM:                                                   
                                                                              
    CCITF.ITEM.NUM$ = PACK$(RIGHT$("0000" + STR$(ITEM.NO%),4))                !4.2HSM
    CCITF.KEY$ = CCITF.CREDIT.CLAIM.NUM$ + CCITF.ITEM.NUM$                    !4.2HSM
    RC% = READ.CCITF                                                          !4.2HSM
    CURRENT.TILL.CLAIM$ = UNPACK$(MID$(CCLAM.FILLER$,1,6))                    !4.2HSM
    IF LEFT$(CCLAM.FILLER$,7) <> STRING$(7," ") THEN BEGIN                    !4.2HSM           
        CURRENT.TILL.CLAIM$ = "Transaction Number : " + \                     !4.2HSM
                             MID$(CURRENT.TILL.CLAIM$,1,4) + \                !4.2HSM 
                             " Till Number : " + \                            !4.2HSM
                             MID$(CURRENT.TILL.CLAIM$,6,3) + \                !4.2HSM 
                             " Operator : " + \                               !4.2HSM
                             MID$(CURRENT.TILL.CLAIM$,10,3)                   !4.2HSM                                          
    ENDIF                                                                     !4.2HSM
      
RETURN

\*****************************************************************************
\*****************************************************************************
\* CLOSE.FILES:                                                              *
\*       close files : IDF, SOFTS, BCSMF                                     *
\*                    CCLAM, CCITF, CCTRL, CCRSN,                            *
\*                    CCSMY, CCDET, CCSTS                                    *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
CLOSE.FILES:
       CLOSE IDF.SESS.NUM%
       CLOSE SOFTS.SESS.NUM%
       CLOSE BCSMF.SESS.NUM%
       CLOSE CCITF.SESS.NUM%
       CLOSE CCTRL.SESS.NUM%
       CLOSE UDESC.SESS.NUM%    !1.6AH
       IF CCRSN.OPEN THEN BEGIN
              CLOSE CCRSN.SESS.NUM%
              CCRSN.OPEN = FALSE
       ENDIF
       IF CCLAM.OPEN THEN BEGIN
              CLOSE CCLAM.SESS.NUM%
              CCLAM.OPEN = FALSE
       ENDIF
       IF CCSMY.OPEN THEN BEGIN
              CLOSE CCSMY.SESS.NUM%
              CCSMY.OPEN = FALSE
       ENDIF
       IF CCDET.OPEN THEN BEGIN
              CLOSE CCDET.SESS.NUM%
              CCDET.OPEN = FALSE
       ENDIF
       IF CCSTS.OPEN THEN BEGIN                                         ! BDCN
              CLOSE CCSTS.SESS.NUM%                                     ! BDCN
              CCSTS.OPEN = FALSE                                        ! BDCN
       ENDIF                                                            ! BDCN
RETURN


\*****************************************************************************
\*****************************************************************************
\* CCSTS.READ.FAIL:                                                          *
\*      EOF reached - set EOF flag                                           *
\*                                                                           *
\*****************************************************************************
   CCSTS.READ.FAIL:

        EOF.CCSTS.FLAG$ = "Y"
        GOTO RETURN.CCSTS.READ.FAIL



\*****************************************************************************
\*****************************************************************************
\* OPEN.FILE.ERROR:                                                          *
\*      log event no 106 (IF END error)                                      *
\*      unique event data includes O + file no                               *
\*      gosub LOG.AN.EVENT                                                   *
\*      goto PROGRAM.EXIT                                                    *
\* RETURN                                                                    *
\*****************************************************************************
OPEN.FILE.ERROR:
       LOG.EVENT.NO% = 106
       LOG.STRING.1.UNIQUE$ = "O" +                                          \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \
                            PACK$(STRING$(14,"0"))
       GOSUB LOG.AN.EVENT
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT
RETURN

\*****************************************************************************
\*****************************************************************************
\* CREATE.FILE.ERROR:                                                        *
\*      log event no 106 (IF END error)                                      *
\*      unique event data includes C + file no                               *
\*      gosub LOG.AN.EVENT                                                   *
\*      goto PROGRAM.EXIT                                                    *
\* RETURN                                                                    *
\*****************************************************************************
CREATE.FILE.ERROR:
       LOG.EVENT.NO% = 106
       LOG.STRING.1.UNIQUE$ = "C" +                                          \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \
                            PACK$(STRING$(14,"0"))
       GOSUB LOG.AN.EVENT
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT
RETURN

\*****************************************************************************
\*****************************************************************************
\* READ.FILE.ERROR:                                                          *
\*      log event no 106 (IF END error)                                      *
\*      unique event data includes R + file no                               *
\*      gosub LOG.AN.EVENT                                                   *
\*      goto PROGRAM.EXIT                                                    *
\* RETURN                                                                    *
\*****************************************************************************
READ.FILE.ERROR:
       LOG.EVENT.NO% = 106
       LOG.STRING.1.UNIQUE$ =  FILE.OPERATION$ +                             \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \
                            PACK$(STRING$(14,"0"))
       GOSUB LOG.AN.EVENT
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT
RETURN

\*****************************************************************************
\*****************************************************************************
\* WRITE.FILE.ERROR:                                                         *
\*      log event no 106 (IF END error)                                      *
\*      unique event data includes W + file no                               *
\*      gosub LOG.AN.EVENT                                                   *
\*      goto PROGRAM.EXIT                                                    *
\* RETURN                                                                    *
\*****************************************************************************
WRITE.FILE.ERROR:
       LOG.EVENT.NO% = 106
       LOG.STRING.1.UNIQUE$ =  FILE.OPERATION$ +                                   \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +             \
                            CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) +             \
                            PACK$(STRING$(14,"0"))
       GOSUB LOG.AN.EVENT
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT
RETURN

\*****************************************************************************
\*****************************************************************************
\* ALLOCATE.SESSION.NUMBERS:                                                 *
\*       gosub CALL.SESS.NUM.UTILITY with function flag = "O" for each       *
\*      file used : IDF, SOFTS, BCSMF                                        *
\*                      CCLAM, CCITF, CCTRL, CCRSN,                          *
\*                      CCSMY, CCDET, CCSTS                                  *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
ALLOCATE.SESSION.NUMBERS:

            F20.FUNCTION$ = "O"

        F20.INTEGER% = IDF.REPORT.NUM%
        F20.STRING$  = IDF.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        IDF.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = SOFTS.REPORT.NUM%
       F20.STRING$  = SOFTS.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        SOFTS.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = BCSMF.REPORT.NUM%
       F20.STRING$  = BCSMF.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        BCSMF.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCLAM.REPORT.NUM%
       F20.STRING$  = CCLAM.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCLAM.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCITF.REPORT.NUM%
       F20.STRING$  = CCITF.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCITF.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCSTS.REPORT.NUM%                                 ! BDCN
       F20.STRING$  = CCSTS.FILE.NAME$                                  ! BDCN
        GOSUB CALL.SESS.NUM.UTILITY                                     ! BDCN
        CCSTS.SESS.NUM% = F20.INTEGER.FILE.NO%                          ! BDCN

       F20.INTEGER% = CCTRL.REPORT.NUM%
       F20.STRING$  = CCTRL.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCTRL.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCRSN.REPORT.NUM%
       F20.STRING$  = CCRSN.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCRSN.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCSMY.REPORT.NUM%
       F20.STRING$  = CCSMY.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCSMY.SESS.NUM% = F20.INTEGER.FILE.NO%

       F20.INTEGER% = CCDET.REPORT.NUM%
       F20.STRING$  = CCDET.FILE.NAME$
        GOSUB CALL.SESS.NUM.UTILITY
        CCDET.SESS.NUM% = F20.INTEGER.FILE.NO%

RETURN

\*****************************************************************************
\*****************************************************************************
\* DEALLOCATE.SESSION.NUMBERS                                                *
\*       gosub CALL.SESS.NUM.UTILITY with function flag = "C" for each       *
\*      file used : IDF, SPFTS, BCSMF                                        *
\*                  CCLAM, CCITF, CCTRL, CCRSN,                              *
\*                  CCSMY, CCDET, CCSTS                                      *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
DEALLOCATE.SESSION.NUMBERS:

            F20.FUNCTION$ = "C"

        F20.INTEGER% = IDF.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = SOFTS.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = BCSMF.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCLAM.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCITF.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCTRL.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCRSN.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCSMY.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCDET.SESS.NUM%
        GOSUB CALL.SESS.NUM.UTILITY

        F20.INTEGER% = CCSTS.SESS.NUM%                                  ! BDCN
        GOSUB CALL.SESS.NUM.UTILITY                                     ! BDCN
RETURN

\*****************************************************************************
\*****************************************************************************
\* CALL.SESS.NUM.UTILITY:                                                    *
\*       call SESS.NUM.UTILITY                                               *
\*      If F20.RETURN.CODE <> 0 THEN                                         *
\*                goto PROGRAM.EXIT                                          *
\*       endif                                                               *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
CALL.SESS.NUM.UTILITY:
       RC% = SESS.NUM.UTILITY(F20.FUNCTION$,                                 \
                               F20.INTEGER%,                                 \
                               F20.STRING$)
        IF RC% <> 0 THEN BEGIN
           LOG.EVENT.NO% = 48
           LOG.STRING.1.UNIQUE$ = F20.FUNCTION$ +                            \
                                   RIGHT$("000000000" + STR$(RC%), 9)
           GOSUB LOG.AN.EVENT
           PROGRAM.FAIL = TRUE
            GOTO PROGRAM.EXIT
       ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* DISPLAY.OUTPUT.MESSAGE:                                                   *
\*   if sleeper/background mode display message on background task           *
\*      call adxserve (return code,func=26,unused,message string)            *
\*      if error, log event and exit program                                 *
\*   else                                                                    *
\*      display message on screen (command mode)                             *
\*   endif                                                                   *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
DISPLAY.OUTPUT.MESSAGE:
         IF ( (LEFT$(COMMAND$,7) = "SLEEPER")                                \
         OR   (LEFT$(COMMAND$,8) = "BACKGRND") ) THEN BEGIN
                CALL ADXSERVE (RC%, 26, 0, OUTPUT.MESSAGE$)
                IF RC% <> 0 THEN BEGIN
                     DISPLAY.OUTPUT.ERROR = TRUE
                    LOG.EVENT.NO% = 23
                    LOG.STRING.1.UNIQUE$ = STR$(RC%)
                    GOSUB LOG.AN.EVENT
                  PROGRAM.FAIL = TRUE
                    GOTO PROGRAM.EXIT
                ENDIF
         ENDIF ELSE BEGIN
              PRINT OUTPUT.MESSAGE$
         ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* LOG.AN.EVENT:                                                             *
\*       Pass details of an event to application log                         *
\*       using 2 parameters - event no. and unique 10 byte data string       *
\*                                                                           *
\* RETURN                                                                    *
\*****************************************************************************
LOG.AN.EVENT:
       LOG.MESSAGE.NO% = 0                       ! set to 0 for batch programs
       LOG.STRING.2$ = ""                     ! set to null for batch programs

       RC% = APPLICATION.LOG (       LOG.MESSAGE.NO%,                        \
                            LOG.STRING.1.UNIQUE$,                            \
                            LOG.STRING.2$,                                   \
                            LOG.EVENT.NO% )
       IF RC% <> 0 THEN BEGIN
              OUTPUT.MESSAGE$ = LOG.FAILED.MESSAGE$
              GOSUB DISPLAY.OUTPUT.MESSAGE
              PROGRAM.FAIL = TRUE
              GOTO PROGRAM.EXIT
       ENDIF
RETURN

\*****************************************************************************
\*****************************************************************************
\* ERROR.DETECTED:                                                           *
\*       if already processing an error (ie.error has occured within error   *
\*       detected) then goto program.exit                                    *
\*                                                                           *
\*       call STANDARD.ERROR.DETECTED(ERRN, ERRF%, ERRL, ERR)                *
\*       goto program.exit                                                   *
\*****************************************************************************
ERROR.DETECTED:
       IF ERROR.PROCESSING.ACTIVE THEN GOTO PROGRAM.EXIT

       RC% = STANDARD.ERROR.DETECTED (ERRN,ERRF%,ERRL,ERR)
       ERROR.PROCESSING.ACTIVE = TRUE
       PROGRAM.FAIL = TRUE
       GOTO PROGRAM.EXIT

\*****************************************************************************
\*****************************************************************************
\ End of program                                                             *
\*****************************************************************************
END
