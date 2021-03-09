\******************************************************************************
\******************************************************************************
\***
\***           FILE FUNCTIONS Boots.com Parcel File (BDCP)
\***
\***           REFERENCE:   BDCPFUN.BAS
\***
\***           VERSION A         Neil Bennett          19th May 2010
\***
\***
\*******************************************************************************
\*******************************************************************************

INTEGER*2 GLOBAL                       \
          CURRENT.REPORT.NUM%

STRING    GLOBAL                       \
          CURRENT.CODE$,               \
          FILE.OPERATION$

STRING    BDCP.KEY$

%INCLUDE BDCPDEC.J86


FUNCTION BDCP.SET PUBLIC
\****************************

    BDCP.REPORT.NUM% = 803
    BDCP.KEYL%       =   7
    BDCP.RECL%       = 101
    BDCP.FILE.NAME$  = "BDCP"

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.BDCP PUBLIC
\****************************

    INTEGER*2 READ.BDCP

    READ.BDCP = 1

    BDCP.KEY$ = BDCP.SUPPLIER$ + BDCP.CARTON$

    IF END #BDCP.SESS.NUM% THEN READ.BDCP.ERROR
    READ FORM "T8,C5,C3,C1,C6,C1,C6,I1,C1,C6,C1,C6,C1,C6,C1,C49";       \
       #BDCP.SESS.NUM%                 \
       KEY BDCP.KEY$;                  \  7 bytes UPD Supp + Carton
       BDCP.ORDER$,                    \  5 bytes UPD Boots.com order number
       BDCP.EXPECT.DATE$,              \  3 bytes UPD Expected Delivery Date
       BDCP.STATUS$,                   \  1 bytes ASC Current status
       BDCP.DEL.DATETIME$,             \  6 bytes UPD Delivery date/time
       BDCP.DEL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.COL.DATETIME$,             \  6 bytes UPD Collected date/time
       BDCP.COL.RC%,                   \  1 byte  INT 0 - Till, 1 - Controller
       BDCP.COL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.RET.DATETIME$,             \  6 bytes UPD Returned date/time
       BDCP.RET.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.LST.DATETIME$,             \  6 bytes UPD Lost date/time
       BDCP.LST.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FND.DATETIME$,             \  6 bytes UPD Found date/time
       BDCP.FND.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FILLER$                    ! 49 bytes filler


    READ.BDCP = 0
    EXIT FUNCTION

READ.BDCP.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION WRITE.BDCP PUBLIC
\****************************

    INTEGER*2 WRITE.BDCP

    WRITE.BDCP = 1

    BDCP.KEY$ = BDCP.SUPPLIER$ + BDCP.CARTON$

    IF END #BDCP.SESS.NUM% THEN WRITE.BDCP.ERROR
    WRITE FORM "C7,C5,C3,C1,C6,C1,C6,I1,C1,C6,C1,C6,C1,C6,C1,C49";    \
       #BDCP.SESS.NUM%;                \
       BDCP.KEY$,                      \  7 bytes UPD Supp + Carton
       BDCP.ORDER$,                    \  5 bytes UPD Boots.com order number
       BDCP.EXPECT.DATE$,              \  3 bytes UPD Expected Delivery Date
       BDCP.STATUS$,                   \  1 bytes ASC Current status
       BDCP.DEL.DATETIME$,             \  6 bytes UPD Delivery date/time
       BDCP.DEL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.COL.DATETIME$,             \  6 bytes UPD Collected date/time
       BDCP.COL.RC%,                   \  1 byte  INT 0 - Till, 1 - Controller
       BDCP.COL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.RET.DATETIME$,             \  6 bytes UPD Returned date/time
       BDCP.RET.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.LST.DATETIME$,             \  6 bytes UPD Lost date/time
       BDCP.LST.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FND.DATETIME$,             \  6 bytes UPD Found date/time
       BDCP.FND.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FILLER$                    ! 49 bytes filler


    WRITE.BDCP = 0
    EXIT FUNCTION

WRITE.BDCP.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION READ.BDCP.LOCK PUBLIC
\*****************************

    INTEGER*2 READ.BDCP.LOCK

    READ.BDCP.LOCK = 1

    BDCP.KEY$ = BDCP.SUPPLIER$ + BDCP.CARTON$

    IF END #BDCP.SESS.NUM% THEN READ.BDCP.LOCK.ERROR
    READ FORM "T8,C5,C3,C1,C6,C1,C6,I1,C1,C6,C1,C6,C1,C6,C1,C49";       \
       #BDCP.SESS.NUM% AUTOLOCK        \
       KEY BDCP.KEY$;                  \  7 bytes UPD Supp + Carton
       BDCP.ORDER$,                    \  5 bytes UPD Boots.com order number
       BDCP.EXPECT.DATE$,              \  3 bytes UPD Expected Delivery Date
       BDCP.STATUS$,                   \  1 bytes ASC Current status
       BDCP.DEL.DATETIME$,             \  6 bytes UPD Delivery date/time
       BDCP.DEL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.COL.DATETIME$,             \  6 bytes UPD Collected date/time
       BDCP.COL.RC%,                   \  1 byte  INT 0 - Till, 1 - Controller
       BDCP.COL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.RET.DATETIME$,             \  6 bytes UPD Returned date/time
       BDCP.RET.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.LST.DATETIME$,             \  6 bytes UPD Lost date/time
       BDCP.LST.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FND.DATETIME$,             \  6 bytes UPD Found date/time
       BDCP.FND.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FILLER$                    ! 49 bytes filler


    READ.BDCP.LOCK = 0
    EXIT FUNCTION

READ.BDCP.LOCK.ERROR:


       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION WRITE.BDCP.UNLOCK PUBLIC
\********************************

    INTEGER*2 WRITE.BDCP.UNLOCK

    WRITE.BDCP.UNLOCK = 1

    BDCP.KEY$ = BDCP.SUPPLIER$ + BDCP.CARTON$

    IF END #BDCP.SESS.NUM% THEN WRITE.BDCP.UNLOCK.ERROR
    WRITE FORM "C7,C5,C3,C1,C6,C1,C6,I1,C1,C6,C1,C6,C1,C6,C1,C49";    \
       #BDCP.SESS.NUM% AUTOUNLOCK;     \
       BDCP.KEY$,                      \  7 bytes UPD Supp + Carton
       BDCP.ORDER$,                    \  5 bytes UPD Boots.com order number
       BDCP.EXPECT.DATE$,              \  3 bytes UPD Expected Delivery Date
       BDCP.STATUS$,                   \  1 bytes ASC Current status
       BDCP.DEL.DATETIME$,             \  6 bytes UPD Delivery date/time
       BDCP.DEL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.COL.DATETIME$,             \  6 bytes UPD Collected date/time
       BDCP.COL.RC%,                   \  1 byte  INT 0 - Till, 1 - Controller
       BDCP.COL.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.RET.DATETIME$,             \  6 bytes UPD Returned date/time
       BDCP.RET.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.LST.DATETIME$,             \  6 bytes UPD Lost date/time
       BDCP.LST.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FND.DATETIME$,             \  6 bytes UPD Found date/time
       BDCP.FND.EXPORTED$,             \  1 bytes ASC Y/N
       BDCP.FILLER$                    ! 49 bytes filler


    WRITE.BDCP.UNLOCK = 0
    EXIT FUNCTION

WRITE.BDCP.UNLOCK.ERROR:


       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%

       EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------

FUNCTION DELETE.BDCP PUBLIC

    INTEGER*2 DELETE.BDCP

    DELETE.BDCP = 1
    BDCP.KEY$ = BDCP.SUPPLIER$ + BDCP.CARTON$
    IF END # BDCP.SESS.NUM% THEN DELETE.ERROR
    DELREC BDCP.SESS.NUM%; BDCP.KEY$
    DELETE.BDCP = 0
    EXIT FUNCTION

DELETE.ERROR:

    FILE.OPERATION$     = "D"
    CURRENT.REPORT.NUM% = BDCP.REPORT.NUM%
    CURRENT.CODE$       = BDCP.KEY$

END FUNCTION

