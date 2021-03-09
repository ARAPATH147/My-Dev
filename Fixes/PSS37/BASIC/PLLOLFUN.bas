
\******************************************************************************
\******************************************************************************
\***
\***              RF PICKING LIST OF LISTS FILE FUNCTIONS
\***
\***               REFERENCE    : PLLOLFUN.BAS
\***
\***         VERSION A            Julia Stones                  11th August 2004
\***
\***         VERSION B            Mark Goode                    17th January 2005
\***         Additional field required to state the lists status ('S' - Shelf Monitor
\***         'F' - Fast Fill, 'O' - OSSR or 'E' - excess stock.
\***
\***         VERSION C            Neil Bennett                     3rd June 2009
\***         Added functions for READ.PLLOL.LOCK and WRITE.PLLOL.UNLOCK
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%

    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$

    %INCLUDE PLLOLDEC.J86

  FUNCTION PLLOL.SET PUBLIC
\***************************

    PLLOL.REPORT.NUM% = 510
    PLLOL.RECL%       = 34
    PLLOL.FILE.NAME$  = "PLLOL"

  END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.PLLOL PUBLIC
\****************************

    INTEGER*2 READ.PLLOL

    READ.PLLOL = 1

    IF END #PLLOL.SESS.NUM% THEN READ.PLLOL.ERROR
    READ FORM "3C3,C1,C6,4C4,2C1"; #PLLOL.SESS.NUM%, PLLOL.RECORD.NUM%; \
       PLLOL.LISTID$,                          \
       PLLOL.CREATOR.ID$,                      \
       PLLOL.PICKER.ID$,                       \
       PLLOL.ITEM.STATUS$,                     \
       PLLOL.CREATE.DATE$,                     \
       PLLOL.CREATE.TIME$,                     \
       PLLOL.PICK.START.TIME$,                 \
       PLLOL.PICK.END.TIME$,                   \
       PLLOL.ITEM.COUNT$,                      \
       PLLOL.OSSR.PICKING$,                    \
       PLLOL.OSSR.STATUS$


    READ.PLLOL = 0
    EXIT FUNCTION

READ.PLLOL.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = PLLOL.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.PLLOL.LOCK PUBLIC                                        ! CNB
\******************************                                        ! CNB

    INTEGER*2 READ.PLLOL.LOCK                                          ! CNB

    READ.PLLOL.LOCK = 1                                                ! CNB

    IF END #PLLOL.SESS.NUM% THEN READ.PLLOL.LOCK.ERROR                 ! CNB
    READ FORM "3C3,C1,C6,4C4,2C1"; #PLLOL.SESS.NUM% AUTOLOCK, PLLOL.RECORD.NUM%; \! CNB
       PLLOL.LISTID$,                          \                       ! CNB
       PLLOL.CREATOR.ID$,                      \                       ! CNB
       PLLOL.PICKER.ID$,                       \                       ! CNB
       PLLOL.ITEM.STATUS$,                     \                       ! CNB
       PLLOL.CREATE.DATE$,                     \                       ! CNB
       PLLOL.CREATE.TIME$,                     \                       ! CNB
       PLLOL.PICK.START.TIME$,                 \                       ! CNB
       PLLOL.PICK.END.TIME$,                   \                       ! CNB
       PLLOL.ITEM.COUNT$,                      \                       ! CNB
       PLLOL.OSSR.PICKING$,                    \                       ! CNB
       PLLOL.OSSR.STATUS$                                              ! CNB


    READ.PLLOL.LOCK = 0                                                ! CNB
    EXIT FUNCTION                                                      ! CNB

READ.PLLOL.LOCK.ERROR:                                                 ! CNB


       FILE.OPERATION$ = "R"                                           ! CNB
       CURRENT.REPORT.NUM% = PLLOL.REPORT.NUM%                         ! CNB

       EXIT FUNCTION                                                   ! CNB

END FUNCTION                                                           ! CNB

\-----------------------------------------------------------------------------


FUNCTION WRITE.PLLOL PUBLIC
\****************************

    INTEGER*2 WRITE.PLLOL

    WRITE.PLLOL = 1

    IF END #PLLOL.SESS.NUM% THEN WRITE.PLLOL.ERROR
    WRITE FORM "3C3,C1,C6,4C4,2C1"; #PLLOL.SESS.NUM%, PLLOL.RECORD.NUM%; \
       PLLOL.LISTID$,                          \
       PLLOL.CREATOR.ID$,                      \
       PLLOL.PICKER.ID$,                       \
       PLLOL.ITEM.STATUS$,                     \
       PLLOL.CREATE.DATE$,                     \
       PLLOL.CREATE.TIME$,                     \
       PLLOL.PICK.START.TIME$,                 \
       PLLOL.PICK.END.TIME$,                   \
       PLLOL.ITEM.COUNT$,                      \
       PLLOL.OSSR.PICKING$,                    \
       PLLOL.OSSR.STATUS$


    WRITE.PLLOL = 0
    EXIT FUNCTION

WRITE.PLLOL.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = PLLOL.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------


FUNCTION WRITE.PLLOL.UNLOCK PUBLIC                                     ! CNB
\*********************************                                     ! CNB

    INTEGER*2 WRITE.PLLOL.UNLOCK                                       ! CNB

    WRITE.PLLOL.UNLOCK = 1                                             ! CNB

    IF END #PLLOL.SESS.NUM% THEN WRITE.PLLOL.UNLOCK.ERROR              ! CNB
    WRITE FORM "3C3,C1,C6,4C4,2C1"; #PLLOL.SESS.NUM% AUTOUNLOCK, PLLOL.RECORD.NUM%; \! CNB
       PLLOL.LISTID$,                          \                       ! CNB
       PLLOL.CREATOR.ID$,                      \                       ! CNB
       PLLOL.PICKER.ID$,                       \                       ! CNB
       PLLOL.ITEM.STATUS$,                     \                       ! CNB
       PLLOL.CREATE.DATE$,                     \                       ! CNB
       PLLOL.CREATE.TIME$,                     \                       ! CNB
       PLLOL.PICK.START.TIME$,                 \                       ! CNB
       PLLOL.PICK.END.TIME$,                   \                       ! CNB
       PLLOL.ITEM.COUNT$,                      \                       ! CNB
       PLLOL.OSSR.PICKING$,                    \                       ! CNB
       PLLOL.OSSR.STATUS$                                              ! CNB


    WRITE.PLLOL.UNLOCK = 0                                             ! CNB
    EXIT FUNCTION                                                      ! CNB

WRITE.PLLOL.UNLOCK.ERROR:                                              ! CNB


       FILE.OPERATION$ = "R"                                           ! CNB
       CURRENT.REPORT.NUM% = PLLOL.REPORT.NUM%                         ! CNB

       EXIT FUNCTION                                                   ! CNB

END FUNCTION                                                           ! CNB

