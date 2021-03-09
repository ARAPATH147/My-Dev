\/******************************************************************/
\/*                                                                */
\/* UOD buffer File Functions                                      */
\/*                                                                */
\/* REFERENCE   : UBFUN.BAS                                        */
\/*                                                                */
\/* VERSION A.          Stuart Highley          11 August 2008     */
\/                                                                 */
\/* VERSION B.          Charles Skadorwa        9  October 2008    */
\/*                     Stuart Highley          17 October 2008    */
\/* Changed the length of the user ID from 3 to 8 to cope with     */
\/* all 9s sign on.                                                */
\/*                                                                */
\/* VERSION C.          Stuart Highley          16 December 2008   */
\/* Added rejected batch flag.                                     */
\/*                                                                */
\/* VERSION D           Mark Goode              29th Decemeber 2008*/
\/* Changes to the fields on the audit record                      */
\/*                                                                */
\/* VERSION E           Mark Goode               25th February 2009*/
\/* Add audit item detail record                                   */ 
\/*                                                                */ 
\/* VERSION F           Mark Goode               4th February 2009 */
\/* Add audit trailer record                                       */
\/*                                                                */  
\/******************************************************************/

    INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

    STRING GLOBAL    CURRENT.CODE$,                                 \
                     FILE.OPERATION$

    %INCLUDE UBDEC.J86

    %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION UB.SET                                                */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION UB.SET PUBLIC

        INTEGER*1 UB.SET

        UB.TEMP.NAME$  = "PUT:"
        UB.FILE.NAME$  = "PUB:"
        UB.REPORT.NUM% = 778

    END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.UB                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION READ.UB PUBLIC

        INTEGER*2 READ.UB

        READ.UB = 1

        IF END #UB.SESS.NUM% THEN READ.ERROR
        READ #UB.SESS.NUM% ; LINE UB.REC$

        UB.REC.TYPE$ = LEFT$(UB.REC$, 1)
        IF UB.REC.TYPE$ = "H" THEN BEGIN                     ! HEADER
            UB.OP.ID$                = MID$(UB.REC$,  2, 8)                    !BSH
            UB.METHOD$               = MID$(UB.REC$, 10, 1)                    !BSH
            UB.REPORT.RQD$           = MID$(UB.REC$, 11, 1)                    !BSH
            UB.ONIGHT.DELIVERY.TYPE$ = MID$(UB.REC$, 12, 1)                    !BCSk
            UB.DRIVER.CHECKIN.REQD$  = MID$(UB.REC$, 13, 1)                    !BCSk
        ENDIF ELSE IF UB.REC.TYPE$ = "B" THEN BEGIN          ! BOOKIN
            UB.LICENCE$        = MID$(UB.REC$, 2, 10)
            UB.DESPATCH.DATE$  = MID$(UB.REC$, 12, 6)
            UB.BOOKED.DATE$    = MID$(UB.REC$, 18, 6)
            UB.BOOKED.TIME$    = MID$(UB.REC$, 24, 6)
            UB.BOOK.TYPE$      = MID$(UB.REC$, 30, 1)
        ENDIF ELSE IF UB.REC.TYPE$ = "A" THEN BEGIN          ! AUDIT
            UB.LICENCE$        = MID$(UB.REC$, 2, 10)        ! DMG
            UB.DESPATCH.DATE$  = MID$(UB.REC$, 12, 6)        ! DMG
            UB.BOOKED.DATE$    = MID$(UB.REC$, 18, 6)        ! DMG
            UB.BOOKED.TIME$    = MID$(UB.REC$, 24, 6)        ! DMG
            UB.BOOKED.STATUS$  = MID$(UB.REC$, 30, 1)        ! DMG
        ENDIF ELSE IF UB.REC.TYPE$ = "C" THEN BEGIN          ! CONFIRMATION      !BCSk
            UB.DRVR.ID$           = MID$(UB.REC$,  2, 8)                         !BCSk
            UB.DRVR.CHCK.DATE$    = MID$(UB.REC$, 10, 6)                         !BCSk
            UB.DRVR.CHCK.TIME$    = MID$(UB.REC$, 16, 6)                         !BCSk
            UB.DRVR.GIT.MATCH$    = MID$(UB.REC$, 22, 1)                         !BCSk
            UB.DRVR.CONFIRM.SCAN$ = MID$(UB.REC$, 23, 1)                         !CSH
            UB.DRVR.REJECTED$     = MID$(UB.REC$, 24, 1)                         !CSH
        ENDIF ELSE IF UB.REC.TYPE$ = "D" THEN BEGIN       ! Audit item record    !EMG            
            UB.BAR.CODE$        = MID$(UB.REC$, 2, 13)                           !EMG
            UB.QTY$             = MID$(UB.REC$, 15, 4)                           !EMG
        ENDIF ELSE IF UB.REC.TYPE$ = "E" THEN BEGIN       ! Audit trailer record !FMG            
            UB.REC.COUNT$         = MID$(UB.REC$, 2, 5)                          !FMG
        ENDIF ELSE IF UB.REC.TYPE$ = "T" THEN BEGIN       ! TRAILER
            UB.REC.COUNT$     = MID$(UB.REC$, 2, 5)
        ENDIF

        READ.UB = 0
        EXIT FUNCTION

READ.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = UB.REPORT.NUM%
        CURRENT.CODE$       = ""

    END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.UB                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION WRITE.UB PUBLIC

        INTEGER*2 WRITE.UB

        WRITE.UB = 1

        IF END #UB.SESS.NUM% THEN WRITE.ERROR                     !BSH
        
        IF UB.REC.TYPE$ = "H" THEN BEGIN
            WRITE FORM "C1 C8 C1 C1 C1 C1 C2"; #UB.SESS.NUM% ; \  !BSH
              UB.REC.TYPE$,                              \
              UB.OP.ID$,                                 \
              UB.METHOD$,                                \
              UB.REPORT.RQD$,                            \
              UB.ONIGHT.DELIVERY.TYPE$,                  \        !BCSk
              UB.DRIVER.CHECKIN.REQD$,                   \        !BCSk
              CHR$(0DH) + CHR$(0AH)
        ENDIF ELSE IF UB.REC.TYPE$ = "B" THEN BEGIN
            WRITE FORM "C1 C10 C6 C6 C6 C1 C2"; #UB.SESS.NUM% ; \
              UB.REC.TYPE$,                                     \
              UB.LICENCE$,                                      \
              UB.DESPATCH.DATE$,                                \
              UB.BOOKED.DATE$,                                  \
              UB.BOOKED.TIME$,                                  \
              UB.BOOK.TYPE$,                                    \
              CHR$(0DH) + CHR$(0AH)
        ENDIF ELSE IF UB.REC.TYPE$ = "A" THEN BEGIN
            WRITE FORM "C1 C10 C6 C6 C6 C1"; #UB.SESS.NUM% ; \
            UB.REC.TYPE$,                                       \
            UB.LICENCE$,                                        \ DMG
            UB.DESPATCH.DATE$,                                  \ DMG
            UB.BOOKED.DATE$,                                    \ DMG
            UB.BOOKED.TIME$,                                    \ DMG
            UB.BOOKED.STATUS$,                                  \ DMG
            CHR$(0DH) + CHR$(0AH)
        ENDIF ELSE IF UB.REC.TYPE$ = "C" THEN BEGIN          ! CONFIRMATION    !BCSk
            WRITE FORM "C1 C8 C6 C6 C1 C1 C1 C2"; #UB.SESS.NUM% ; \            !BCSk
              UB.REC.TYPE$,                                       \            !BSH  
              UB.DRVR.ID$,                                        \            !BCSk
              UB.DRVR.CHCK.DATE$,                                 \            !BCSk
              UB.DRVR.CHCK.TIME$,                                 \            !BCSk
              UB.DRVR.GIT.MATCH$,                                 \            !BCSk
              UB.DRVR.CONFIRM.SCAN$,                              \            !BCSk
              UB.DRVR.REJECTED$,                                  \            !CSH
              CHR$(0DH) + CHR$(0AH)                                            !BCSk
        ENDIF ELSE IF UB.REC.TYPE$ = "D" THEN BEGIN
            WRITE FORM "C1 C13 C4 C2"; #UB.SESS.NUM% ;            \            !EMG
            UB.REC.TYPE$,                                         \            !EMG
            UB.BAR.CODE$,                                         \            !EMG
            UB.QTY$,                                              \            !EMG               
            CHR$(0DH) + CHR$(0AH)
        ENDIF ELSE IF UB.REC.TYPE$ = "E" THEN BEGIN                            !FMG
            WRITE FORM "C1 C5 C2"; #UB.SESS.NUM% ; \                           !FMG
              UB.REC.TYPE$,                        \                           !FMG
              UB.REC.COUNT$,                       \                           !FMG
              CHR$(0DH) + CHR$(0AH)                                            !FMG               
        ENDIF ELSE IF UB.REC.TYPE$ = "T" THEN BEGIN
            WRITE FORM "C1 C5 C2"; #UB.SESS.NUM% ; \
              UB.REC.TYPE$,                        \
              UB.REC.COUNT$,                       \
              CHR$(0DH) + CHR$(0AH)
        ENDIF

        WRITE.UB = 0
        EXIT FUNCTION

WRITE.ERROR:

        FILE.OPERATION$ = "W"
        CURRENT.REPORT.NUM% = UB.REPORT.NUM%
        CURRENT.CODE$ = UB.LICENCE$

    END FUNCTION


