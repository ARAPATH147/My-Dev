\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PRINTFUN.bas  $
\***
\***   $Revision:   1.8  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PRINTFUN.bav  $
\***
\***      Rev 1.8   17 Dec 2008 13:55:54   charles.skadorwa
\***   Modified function to make it compatible
\***   with Alliance Pharmacy stores Brother
\***   Laser printers when condensed printing
\***   selected. In condensed mode, the Lexmark
\***   printer can fit 135 characters per line
\***   whilst the Brother can only fit 120. The
\***   presence of a template file on the
\***   controller determines if it is an AP store.
\***
\***      Rev 1.7   Aug 07 2000 15:11:14   dev11ps
\***   Changed to allow Laserjet printing with
\***   Chinese characters
\***
\***      Rev 1.6   Apr 20 2000 10:59:38   dev11ps
\***   Changed the value of the Recl back to 600
\***   from 700. This was because a problem occurs
\***   when large volumes of labels are required.
\***
\***      Rev 1.5   Nov 02 1999 14:00:00   dev11ps
\***   Change to record length
\***
\***      Rev 1.4   Oct 22 1999 14:46:16   dev11ps
\***
\***
\***      Rev 1.3   Oct 18 1999 11:39:22   dev11ps
\***   Changed for Euro
\***
\***      Rev 1.2   Oct 15 1999 15:43:28   dev11ps
\***   Changed to cater for increase of record length
\***   for Euro
\***
\***      Rev 1.1   11 Jan 1995 15:08:30   NIK
\***
\***
\******************************************************************************
\******************************************************************************
REM \
\******************************************************************************
\******************************************************************************
\***
\***    SOURCE FOR PRINT FILE PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   PRINTFU (BAS)
\***
\***        FILE TYPE   :   Printer / Labeller
\***
\***    VERSION A.              ANDREW WEDGEWORTH.                  09 JUL 1992.
\***    Original version created by merging PRINTFNG and PRINTSEG.
\***
\***    VERSION B.              UNKNOWN.                            ?? ??? 1992.
\***    Changes unknown.
\***
\***    VERSION C.              ROBERT COWEY.                       18 NOV 1992.
\***    Coded IF END # processing.
\***
\***    Version D           Andrew Wedgeworth                     24th May 1993
\***    Function added to produce a condensed report.
\***
\***    Version E           Stuart WIlliam McConnachie           31st July 2000
\***    Added function to write variable length lines.
\***
\***    Version F           Charles Skadorwa                     4th April 2008
\***    Modified function to make it compatible with Alliance Pharmacy
\***    stores Brother Laser printers when condensed printing selected.
\***    In condensed mode, the Lexmark printer can fit 135 characters
\***    per line whilst the Brother can only fit 120. The presence of a
\***    template file on the controller determines if it is an AP store.
\***
\***    Version G           Charles Skadorwa                     4th April 2016
\***    It was found that certain reports would not print when stores
\***    were converted to LAN printing.
\***
\***    Added new function: WRITE.PRINT.PLUS.LF based on WRITE.PRINT.
\***    This adds a Line-Feed characters to every print line.
\***
\*******************************************************************************
\*******************************************************************************

    %INCLUDE PRINTDEC.J86 ! Print file variable declarations

    STRING FORM$                                                       ! ESWM

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$


    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION PRINT.SET PUBLIC

    INTEGER*2 PRINT.SET
    PRINT.SET EQ 1

\    PRINT.CONDENSED.RECL% EQ 137                                       ! DAW
    PRINT.CONDENSED.RECL% EQ 0
    PRINT.REPORT.NUM%     EQ  30
    PRINT.REPORT.RECL%    EQ  0
\    PRINT.REPORT.RECL%    EQ  80
    PRINT.SELF.RECL%      EQ  600
    PRINT.FILE.NAME$      EQ "PRN0:"
    SELF.NOLAN.NAME$      EQ "LABELLER"
    SELF.LAN.NAME$        EQ "PRN0:"

    PRINT.SET EQ 0

END FUNCTION


FUNCTION WRITE.PRINT PUBLIC

    INTEGER*2 WRITE.PRINT
    WRITE.PRINT EQ 1

    IF END # PRINT.SESS.NUM% THEN ERROR.WRITE.PRINT
    WRITE FORM "C80"; # PRINT.SESS.NUM% ; PRINT.LINE$

    WRITE.PRINT EQ 0
    EXIT FUNCTION

ERROR.WRITE.PRINT:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ 30
    CURRENT.CODE$       EQ ""

    EXIT FUNCTION

END FUNCTION


!-----------------------------------------------------------------------!GCS
! WRITE.PRINT.PLUS.LF                                                   !GCS
!                                                                       !GCS
! All report lines passed to this function should be less than 80       !GCS
! characters after which a Line-Feed is added.                          !GCS
!                                                                       !GCS
!-----------------------------------------------------------------------!GCS
                                                                        !GCS
FUNCTION WRITE.PRINT.PLUS.LF PUBLIC                                     !GCS
                                                                        !GCS
    INTEGER*2 WRITE.PRINT.PLUS.LF,                                     \!GCS
              LENGTH%                                                   !GCS
                                                                        !GCS
    WRITE.PRINT.PLUS.LF EQ 1                                            !GCS
                                                                        !GCS
    LENGTH% = LEN(PRINT.LINE$)                                          !GCS
    IF LEN(PRINT.LINE$) < 79 THEN BEGIN                                 !GCS
       PRINT.LINE$ = PRINT.LINE$ + STRING$(79 - LENGTH%, " ")  +       \!GCS
                     CHR$(10)                                           !GCS
    ENDIF ELSE BEGIN                                                    !GCS
       PRINT.LINE$ = LEFT$(PRINT.LINE$, 79)  + CHR$(10)                 !GCS
    ENDIF                                                               !GCS
    LENGTH% = MATCH(CHR$(12), PRINT.LINE$, 1)                           !GCS
    IF LENGTH% > 0 AND LENGTH% < 80 THEN BEGIN                          !GCS
       PRINT.LINE$ = LEFT$(PRINT.LINE$, 79) + CHR$(10)                  !GCS
    ENDIF                                                               !GCS
                                                                        !GCS
    IF END # PRINT.SESS.NUM% THEN ERROR.WRITE.PRINT.PLUS.LF             !GCS
    WRITE  FORM "C80";# PRINT.SESS.NUM% ; PRINT.LINE$                   !GCS
                                                                        !GCS
    WRITE.PRINT.PLUS.LF EQ 0                                            !GCS
    EXIT FUNCTION                                                       !GCS
                                                                        !GCS
ERROR.WRITE.PRINT.PLUS.LF:                                              !GCS
                                                                        !GCS
    FILE.OPERATION$     EQ "W"                                          !GCS
    CURRENT.REPORT.NUM% EQ 30                                           !GCS
    CURRENT.CODE$       EQ ""                                           !GCS
                                                                        !GCS
    EXIT FUNCTION                                                       !GCS
                                                                        !GCS
END FUNCTION                                                            !GCS




!FUNCTION WRITE.CONDENSED.PRINT PUBLIC                                  ! DAW
FUNCTION WRITE.CONDENSED.PRINT (AP.STORE) PUBLIC                       ! FCSk

    INTEGER*2 WRITE.CONDENSED.PRINT                                    ! DAW
    INTEGER*1 AP.STORE                                                 ! FCSk
    STRING MY.FORM$                                                    ! FCSk
    WRITE.CONDENSED.PRINT EQ 1                                         ! DAW

    IF AP.STORE THEN BEGIN                                             ! FCSk
        MY.FORM$ = "C122"                                              ! FCSk
    ENDIF ELSE BEGIN                                                   ! FCSk
        MY.FORM$ = "C137"                                              ! FCSk
    ENDIF                                                              ! FCSk

    IF END # PRINT.SESS.NUM% THEN ERROR.WRITE.CONDENSED.PRINT          ! DAW
    !WRITE FORM "C137"; # PRINT.SESS.NUM% ; PRINT.LINE$                 ! DAW
    WRITE FORM MY.FORM$; # PRINT.SESS.NUM% ; PRINT.LINE$               ! FCSk

    WRITE.CONDENSED.PRINT EQ 0                                         ! DAW
    EXIT FUNCTION                                                      ! DAW

ERROR.WRITE.CONDENSED.PRINT:                                           ! DAW

    FILE.OPERATION$     EQ "W"                                         ! DAW
    CURRENT.REPORT.NUM% EQ 30                                          ! DAW
    CURRENT.CODE$       EQ ""                                          ! DAW

    EXIT FUNCTION                                                      ! DAW

END FUNCTION                                                           ! DAW


FUNCTION WRITE.PCL.PRINT PUBLIC                                        ! ESWM

    INTEGER*2 WRITE.PCL.PRINT                                          ! ESWM
    WRITE.PCL.PRINT EQ 1                                               ! ESWM

    IF END # PRINT.SESS.NUM% THEN ERROR.WRITE.PRINT                    ! ESWM
    FORM$ = "C"+STR$(LEN(PRINT.LINE$))                                 ! ESWM
    WRITE FORM FORM$; # PRINT.SESS.NUM% ; PRINT.LINE$                  ! ESWM

    WRITE.PCL.PRINT EQ 0                                               ! ESWM
    EXIT FUNCTION                                                      ! ESWM

ERROR.WRITE.PRINT:                                                     ! ESWM

    FILE.OPERATION$     EQ "W"                                         ! ESWM
    CURRENT.REPORT.NUM% EQ 30                                          ! ESWM
    CURRENT.CODE$       EQ ""                                          ! ESWM

    EXIT FUNCTION                                                      ! ESWM

END FUNCTION                                                           ! ESWM


FUNCTION WRITE.LABEL PUBLIC

    INTEGER*2 WRITE.LABEL
    WRITE.LABEL EQ 1

    IF END # PRINT.SESS.NUM% THEN ERROR.WRITE.LABEL
    WRITE FORM "C600"; # PRINT.SESS.NUM% ; PRINT.LINE$

    WRITE.LABEL EQ 0
    EXIT FUNCTION

ERROR.WRITE.LABEL:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ 30
    CURRENT.CODE$       EQ ""

    EXIT FUNCTION

END FUNCTION

