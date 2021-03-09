\***********************************************************************/
\***********************************************************************/
\***
\***
\***        FUNCTION      : SET.LABEL.TYPE
\***        AUTHOR        : Neil Bennett
\***
\***        DATE WRITTEN  : 18th June 2007
\***
\***        REFERENCE     : PSBF46
\***
\***
\***********************************************************************/
\***********************************************************************/
\***
\***    VERSION A.            NEIL BENNETT.                 18 Jun 2007.
\***    Initial version. - extracted from PSBF19
\***
\***    VERSION B.            NEIL BENNETT.                 03 Jul 2007.
\***    Algorithm for label types modified to CIP2 design from CIP1.
\***    Call to SET.PHF added to initialisation for calls from programs
\***    that have not setup PHF functions/variables.
\***
\***    VERSION C.            NEIL BENNETT.                 09 Jul 2007.
\***    If new price is equal to phf current price then set globals to
\***    existing values in PHF.
\***
\***********************************************************************/
\***********************************************************************/

\***********************************************************************/
\***********************************************************************/
\***
\***
\***                   FUNCTION OVERVIEW
\***                   -----------------
\***
\***     This function handles operations for computing the new label
\***     type and optionally the update of the PHF.
\***
\***     Globals are set up to hold the new label type together with the
\***     was/now and was/was/now prices if applicable.
\***
\***     The code assumes that the IRF and IDF records have been read
\***     before calling the function, and that SRITL file is opened.
\***
\***********************************************************************/
\***********************************************************************/
\***
\-----------------------------------------------------------------------/

      %INCLUDE PSBF02G.J86
      %INCLUDE PSBF18G.J86
      %INCLUDE PSBF20G.J86
      %INCLUDE PSBF46G.J86

      %INCLUDE IRFDEC.J86
      %INCLUDE IDFDEC.J86
      %INCLUDE PHFDEC.J86
      %INCLUDE SRITLDEC.J86

      INTEGER*2 F46.PHF.SESS.NUM%
      INTEGER*2 SAV.SESS.NUM%

      %INCLUDE PSBF02E.J86
      %INCLUDE PSBF18E.J86
      %INCLUDE PSBF20E.J86

      %INCLUDE PHFEXT.J86
      %INCLUDE SRITLEXT.J86

\***********************************************************************/
\***
\***  FUNCTION GET.LABEL.TYPE(BAR.CODE$,        UPD 6 without check dgt
\***                          OLD.PRICE%,       INT 4 current price
\***                          NEW.PRICE%,       INT 4 new price
\***                          UPDATE.PHF$)      ASC 1 Y/N
\***
\***  Function returns 0 - completed successfully
\***                   1 - Invalid barcode
\***                   2 - Price already set in PHF
\***
\***  Sets GLOBALS     F46.LABEL.TYPE$          ASC
\***                   F46.WAS.PRICE%           INT 4
\***                   F46.WAS.WAS.PRICE%       INT 4
\***
\***********************************************************************/


   FUNCTION GET.LABEL.TYPE(BAR.CODE$,                  \ UPD 6
                           OLD.PRICE%,                 \ INT 4
                           NEW.PRICE%,                 \ INT 4
                           UPDATE.PHF$) PUBLIC         ! ASC 1

      INTEGER*1 GET.LABEL.TYPE
      STRING    BAR.CODE$
      INTEGER*4 NEW.PRICE%
      STRING    NEW.TYPE$
      INTEGER*4 OLD.PRICE%
      STRING    UPDATE.PHF$

      INTEGER*2 F46.PHF.SESS.NUM%
      INTEGER*2 F46.SAV.SESS%
      INTEGER*1 ON.LP%
      INTEGER*1 phf.data%
      INTEGER*1 phf.open%
      INTEGER*2 rc%

      GET.LABEL.TYPE = 1

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Set variables
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      F46.LABEL.TYPE$    = "0"
      F46.WAS.PRICE%     = 0
      F46.WAS.WAS.PRICE% = 0

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   If invalid barcode - return
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF VAL(UNPACK$(BAR.CODE$)) = 0                                    \
      OR LEN (BAR.CODE$) <> 6 THEN GOTO EXIT.NOW

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Check/Allocate/Open files
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF NOT phf.open% THEN GOSUB INITIALISE

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Set up file keys - read PHF
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      PHF.BAR.CODE$ = BAR.CODE$

      F46.SAV.SESS% = PHF.SESS.NUM%
      PHF.SESS.NUM% = F46.PHF.SESS.NUM%
      phf.data% = -1
      IF READ.PHF <> 0 THEN BEGIN

         phf.data% = 0

         PHF.CURR.PRICE%      = 0
         PHF.PEND.PRICE%      = 0
         PHF.HIST1.PRICE%     = 0
         PHF.HIST2.PRICE%     = 0

         PHF.CURR.TYPE$       = " "
         PHF.PEND.TYPE$       = " "
         PHF.HIST1.TYPE$      = " "
         PHF.HIST2.TYPE$      = " "

         PHF.CURR.DATE$       = PACK$("000000")
         PHF.PEND.DATE$       = PHF.CURR.DATE$
         PHF.HIST1.DATE$      = PHF.CURR.DATE$
         PHF.HIST2.DATE$      = PHF.CURR.DATE$
         PHF.LAST.INC.DATE$   = PACK$(DATE$)

         PHF.CURR.LABL$       = "0"

         PHF.FILLER$          = "  "

      ENDIF
      PHF.SESS.NUM% = F46.SAV.SESS%
      F46.SAV.SESS% = 0

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Dont proceed if price hasn't changed
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF NEW.PRICE% = PHF.CURR.PRICE% THEN BEGIN
         F46.LABEL.TYPE$ = PHF.CURR.LABL$
         IF            F46.LABEL.TYPE$ = "1" THEN BEGIN
            F46.WAS.PRICE%     = PHF.HIST1.PRICE%
         ENDIF ELSE IF F46.LABEL.TYPE$ = "2" THEN BEGIN
            F46.WAS.PRICE%     = PHF.HIST1.PRICE%
            F46.WAS.WAS.PRICE% = PHF.HIST2.PRICE%
         ENDIF
         GET.LABEL.TYPE = 2
         GOTO EXIT.NOW
      ENDIF

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Check if item is on live planner (not for file update mode)
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF UCASE$(UPDATE.PHF$) <> "Y" THEN BEGIN
         ON.LP% = 0
         IF SRITL.SESS.NUM% <> 0 THEN BEGIN
            SRITL.ITEM.CODE$ = IRF.BOOTS.CODE$
            SRITL.RECORD.CHAIN% = 0
            IF READ.SRITL = 0 THEN ON.LP% = -1
         ENDIF
      ENDIF ELSE BEGIN
         ON.LP% = -1
      ENDIF

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   If price increase set the last increase date
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF (NEW.PRICE%  > OLD.PRICE%                                      \
          AND OLD.PRICE% > 0      )                                     \
      OR (phf.data% = -1                                                \
          AND NEW.PRICE%  > PHF.CURR.PRICE%) THEN BEGIN
         PHF.LAST.INC.DATE$ = PACK$(DATE$)
      ENDIF

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Compute Label Type
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF (IDF.BIT.FLAGS.1% AND 20H) = 0                                 \
      OR (IRF.INDICAT8% AND 10000000B) THEN BEGIN
         ! Not markdown item
         ! or WEEE item
         F46.LABEL.TYPE$ = "0"
      ENDIF ELSE IF NEW.PRICE% <> PHF.CURR.PRICE% THEN BEGIN

         ! Get number of price changes in last 28 days
         F02.DATE$ = DATE$
         rc% = UPDATE.DATE(-28)
         F02.DATE$ = PACK$(F02.DATE$)

         IF phf.data% = 0                                               \
         OR ON.LP% = 0                                                  \
         OR PHF.CURR.LABL$ = "3"                                        \ BNWB
         OR ((PHF.CURR.LABL$ = "0") AND (F02.DATE$ <=PHF.CURR.DATE$))   \
         OR F02.DATE$ <= PHF.LAST.INC.DATE$ THEN BEGIN
            ! IF no price history available
            ! OR NOT on Live planner
            ! OR already set to clearance - stay clearance
            ! OR NOT already markdown
            !    AND price change within last 28 days
            ! OR a price increase within last 28 days
            F46.LABEL.TYPE$    = "3"                ! Clearance
\ - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\        ! Algorithm for label types modified to CIP 2 DD               ! BNWB
\        ENDIF ELSE IF F02.DATE$ <= PHF.HIST1.DATE$ THEN BEGIN
\           ! More than 1 change in last 28 days
\           F46.LABEL.TYPE$    = "2"                ! Was/Was/Now
\           F46.WAS.PRICE%     = PHF.CURR.PRICE%
\           F46.WAS.WAS.PRICE% = PHF.HIST1.PRICE%
\        ENDIF ELSE BEGIN
\           ! Only change in last 28 days
\           F46.LABEL.TYPE$    = "1"                ! Was/Now
\           F46.WAS.PRICE%     = PHF.CURR.PRICE%
\        ENDIF
\ - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
         ENDIF ELSE IF PHF.CURR.LABL$ = "0" THEN BEGIN                  ! BNWB
            ! If NOT already markdown                                   ! BNWB
            F46.LABEL.TYPE$    = "1"                                    ! BNWB
            F46.WAS.PRICE%     = PHF.CURR.PRICE%                        ! BNWB
         ENDIF ELSE BEGIN                                               ! BNWB
            F46.LABEL.TYPE$    = "2"                ! Was/Was/Now       ! BNWB
            F46.WAS.PRICE%     = PHF.CURR.PRICE%                        ! BNWB
            F46.WAS.WAS.PRICE% = PHF.HIST1.PRICE%                       ! BNWB
         ENDIF                                                          ! BNWB

         ! Markdown items to be flagged as new type "C"
         NEW.TYPE$ = "C"

      ENDIF

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Update PHF if required
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      IF UCASE$(UPDATE.PHF$) = "Y" THEN BEGIN

         ! Ripple through the price history
         PHF.HIST2.PRICE% = PHF.HIST1.PRICE%
         PHF.HIST1.PRICE% = PHF.CURR.PRICE%
         PHF.CURR.PRICE%  = NEW.PRICE%

         PHF.HIST2.DATE$  = PHF.HIST1.DATE$
         PHF.HIST1.DATE$  = PHF.CURR.DATE$
         PHF.CURR.DATE$   = PACK$(DATE$)

         PHF.HIST2.TYPE$  = PHF.HIST1.TYPE$
         PHF.HIST1.TYPE$  = PHF.CURR.TYPE$
         PHF.CURR.TYPE$   = NEW.TYPE$

         PHF.CURR.LABL$   = F46.LABEL.TYPE$

         F46.SAV.SESS% = PHF.SESS.NUM%
         PHF.SESS.NUM% = F46.PHF.SESS.NUM%
         rc% = WRITE.PHF
         PHF.SESS.NUM% = F46.SAV.SESS%
         F46.SAV.SESS% = 0

      ENDIF

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*   Set Good return
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

      GET.LABEL.TYPE = 0

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/*      Set Globals
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

EXIT.NOW:
      IF F46.LABEL.TYPE$ = "0" THEN BEGIN
         F46.LABEL.TYPE$ = "N"
      ENDIF ELSE BEGIN
         F46.LABEL.TYPE$ = "N" + F46.LABEL.TYPE$
      ENDIF

EXIT FUNCTION

\***********************************************************************/
\***
\*** ALLOCATE SESSIONS
\***
\***********************************************************************/

INITIALISE:

      CALL PHF.SET                                                      ! BNWB
      rc% = SESS.NUM.UTILITY ("O",                                      \
                               PHF.REPORT.NUM%,                         \
                               PHF.FILE.NAME$  )
      F46.PHF.SESS.NUM% = F20.INTEGER.FILE.NO%

      phf.open% = 0
      IF END# F46.PHF.SESS.NUM% THEN phf.open.err
      OPEN PHF.FILE.NAME$ KEYED RECL PHF.RECL% AS F46.PHF.SESS.NUM%                                      ! 1.11 NWB
      phf.open% = -1
phf.open.err:

RETURN

END FUNCTION

\***********************************************************************/