\***********************************************************************
\***********************************************************************
\***
\***           FILE FUNCTIONS Boots.com Location File (BDCLOCON)
\***
\***           REFERENCE:   BDCLOFUN.BAS
\***
\***           VERSION A         Dave Constable        7th March 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           Initial version for User Story PMLA-17 & PMLA-58 to allow
\***           Location access for Parcels by controller and till for
\***           Boots.com/ie order parcels.
\***
\***           VERSION B         Lino Jacob           11th April 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           - User Story PMLA-16 : To disallow the deactivation of
\***             locations if there are parcels available at that
\***             location - Added a new field to hold count of parcels
\***
\***           VERSION c         Lino Jacob           18th April 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           - Added the node id to the file name to gain access from
\***             alternate controller.
\***
\***           VERSION D         Lino Jacob            8th Aug 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           - PMLA 213 - Changes to ULN of BDCLOCON
\***
\***           VERSION E         Dave Constable       15th Sep 2016
\***           PRJ1361- Order & Collect Parcel Management
\***           Corrected ON ERROR for Read locon
\***
\***********************************************************************
\***********************************************************************

STRING GLOBAL CURRENT.CODE$
STRING GLOBAL FILE.OPERATION$
INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

%INCLUDE BDCLODEC.J86

!***********************************************************************
!***
!***    FUNCTION        BDCLOCON.SET
!***
!***    sets the global variables for the file.
!***
!***********************************************************************
FUNCTION BDCLOCON.SET PUBLIC

    BDCLOCON.FILE.NAME$     = "BDCLOCON"                                !DLJ
!   BDCLOCON.FILE.NAME$     = "ADXLXACN::D:\ADX_UDT1\BDCLOCON.BIN"      !DLJ
                                                                        !DLJ
    BDCLOCON.FORM$          = "C1 C10 C20 I2 C13 C2"                    !BLJ
    BDCLOCON.RECL%          = 48
    BDCLOCON.REPORT.NUM%    = 892

END FUNCTION

!***********************************************************************
!***
!***    FUNCTION        READ.BDCLOCON
!***
!***    Read the Location file into Globals.
!***    Local Error handling to cater for file and LAN errors
!***
!***********************************************************************
FUNCTION READ.BDCLOCON PUBLIC
    INTEGER*2   READ.BDCLOCON
    STRING      F.CRLF$

ON ERROR GOTO READ.BDCLOCON.ERROR                                       !EDC
    READ.BDCLOCON = 1

!    IF END #BDCLOCON.SESS.NUM% THEN READ.BDCLOCON.ERROR                !EDC
    READ FORM BDCLOCON.FORM$;          \
       #BDCLOCON.SESS.NUM%,            \
       BDCLOCON.RECORD.NUM%;           \ record number
       BDCLOCON.STATUS$,               \ active?
       BDCLOCON.SHORT.NAME$,           \ short location name
       BDCLOCON.LONG.NAME$,            \ long location name
       BDCLOCON.PARCEL.COUNT%,         \ count of parcels               !BLJ
       BDCLOCON.FILLER$,               \ filler for future usage
       F.CRLF$                         ! CR & LF

    READ.BDCLOCON = 0

FUNC.EXIT:
    EXIT FUNCTION

READ.BDCLOCON.ERROR:
!***********************************************************************
!* Error Handling
!***********************************************************************

       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%
       ! set defaults on fail
       BDCLOCON.SHORT.NAME$ = \
        "REC #"+RIGHT$(STRING$(5," ")+STR$(BDCLOCON.RECORD.NUM%),5)
       BDCLOCON.LONG.NAME$  = "NO RECORD "+BDCLOCON.SHORT.NAME$

    RESUME FUNC.EXIT

END FUNCTION

!***********************************************************************
!***
!***    FUNCTION        WRITE.BDCLOCON
!***
!***    Write the Location file from Globals.
!***    Local Error handling to cater for file and LAN errors
!***
!***********************************************************************
FUNCTION WRITE.BDCLOCON PUBLIC
    INTEGER*2   WRITE.BDCLOCON
    STRING      F.CRLF$

    ON ERROR GOTO FUNC.ERR

    WRITE.BDCLOCON = 1

    F.CRLF$ = CHR$(0DH)+CHR$(0AH)

    WRITE FORM BDCLOCON.FORM$;          \
       #BDCLOCON.SESS.NUM%,            \
       BDCLOCON.RECORD.NUM%;           \ record number
       BDCLOCON.STATUS$,               \ active?
       BDCLOCON.SHORT.NAME$,           \ short location name
       BDCLOCON.LONG.NAME$,            \ long location name
       BDCLOCON.PARCEL.COUNT%,         \ count of parcels               !BLJ
       BDCLOCON.FILLER$,               \ filler for future usage
       F.CRLF$                         ! CR & LF

       WRITE.BDCLOCON = 0

FUNC.EXIT:
    EXIT FUNCTION

FUNC.ERR:
!***********************************************************************
!* Error Handling
!***********************************************************************

    FILE.OPERATION$ = "W"
    CURRENT.REPORT.NUM% = BDCLOCON.REPORT.NUM%

    RESUME FUNC.EXIT

END FUNCTION



