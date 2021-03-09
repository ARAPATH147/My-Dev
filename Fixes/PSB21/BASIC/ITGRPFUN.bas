\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: IRFITGRP (IRF Attribute Extension File)
\***                 Public File Function Definitions
\***
\***    FILE TYPE : Keyed
\***
\***********************************************************************
\***
\***    Version A.          Rejiya Nair                12th May 2016
\***    PRJ1452 Restricting Item Sales
\***    Initial version.
\***
\***********************************************************************
\***********************************************************************

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

    STRING                                                              \
        FORMAT.STRING$

%INCLUDE ITGRPDEC.J86

\***********************************************************************
\***
\***    IRFITGRP.SET
\***
\***    Declare IRFITGRP file constants
\***
\***********************************************************************
FUNCTION IRFITGRP.SET PUBLIC

    IRFITGRP.FILE.NAME$  = "IRFITGRP" ! File name
    IRFITGRP.KEYL%       = 3          ! Key length
    IRFITGRP.RECL%       = 4          ! Record length
    IRFITGRP.REPORT.NUM% = 878        ! Report number

END FUNCTION

\***********************************************************************
\***
\***    READ.IRFITGRP
\***
\***    Read IRFITGRP file record
\***
\***********************************************************************
FUNCTION READ.IRFITGRP PUBLIC

    INTEGER*1 READ.IRFITGRP

    READ.IRFITGRP = 1

    FORMAT.STRING$ = "T4,I1" 

    IF END #IRFITGRP.SESS.NUM% THEN READ.IRFITGRP.ERROR
	
	 
	 
    READ FORM FORMAT.STRING$; #IRFITGRP.SESS.NUM%                      \
         KEY IRFITGRP.ITEM.CODE$;           \ Item code                \
             IRFITGRP.GROUP.NO%             ! Group number of the item
	 		  
    READ.IRFITGRP = 0
	 

    EXIT FUNCTION

READ.IRFITGRP.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION

\***********************************************************************
\***
\***    READ.IRFITGRP.LOCK
\***
\***    Read IRFITGRP file record with lock
\***
\***********************************************************************
FUNCTION READ.IRFITGRP.LOCK PUBLIC

    INTEGER*1 READ.IRFITGRP.LOCK

    READ.IRFITGRP.LOCK = 1

    FORMAT.STRING$ = "T4,I1"

    IF END #IRFITGRP.SESS.NUM% THEN READ.IRFITGRP.LOCK.ERROR
    READ FORM FORMAT.STRING$; #IRFITGRP.SESS.NUM% AUTOLOCK             \
         KEY IRFITGRP.ITEM.CODE$;           \ Item code                \
             IRFITGRP.GROUP.NO%             ! Group number of the item

    READ.IRFITGRP.LOCK = 0

    EXIT FUNCTION

READ.IRFITGRP.LOCK.ERROR:

    FILE.OPERATION$     = "R"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION

\***********************************************************************
\***
\***    WRITE.IRFITGRP
\***
\***    Write IRFITGRP file record
\***
\***********************************************************************
FUNCTION WRITE.IRFITGRP PUBLIC

    INTEGER*1 WRITE.IRFITGRP

    WRITE.IRFITGRP = 1

    FORMAT.STRING$ = "C3,I1"

    IF END #IRFITGRP.SESS.NUM% THEN WRITE.IRFITGRP.ERROR
    WRITE FORM FORMAT.STRING$; #IRFITGRP.SESS.NUM%;                    \
        IRFITGRP.ITEM.CODE$,               \ Item code                 \
        IRFITGRP.GROUP.NO%                 ! Group number of the item

    WRITE.IRFITGRP = 0

    EXIT FUNCTION

WRITE.IRFITGRP.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION

\***********************************************************************
\***
\***    WRITE.IRFITGRP.HOLD
\***
\***    Write hold IRFITGRP file record
\***
\***********************************************************************
FUNCTION WRITE.IRFITGRP.HOLD PUBLIC

    INTEGER*1 WRITE.IRFITGRP.HOLD

    WRITE.IRFITGRP.HOLD = 1

    FORMAT.STRING$ = "C3,I1"

    IF END #IRFITGRP.SESS.NUM% THEN WRITE.IRFITGRP.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #IRFITGRP.SESS.NUM%;               \
        IRFITGRP.ITEM.CODE$,               \ Item code                 \
        IRFITGRP.GROUP.NO%                 ! Group number of the item

    WRITE.IRFITGRP.HOLD = 0

    EXIT FUNCTION

WRITE.IRFITGRP.HOLD.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION

\***********************************************************************
\***
\***    WRITE.IRFITGRP.UNLOCK
\***
\***    Write IRFITGRP file record and unlock
\***
\***********************************************************************
FUNCTION WRITE.IRFITGRP.UNLOCK PUBLIC

    INTEGER*1 WRITE.IRFITGRP.UNLOCK

    WRITE.IRFITGRP.UNLOCK = 1

    FORMAT.STRING$ = "C3,I1"

    IF END #IRFITGRP.SESS.NUM% THEN WRITE.IRFITGRP.UNLOCK.ERROR
    WRITE FORM FORMAT.STRING$; #IRFITGRP.SESS.NUM% AUTOUNLOCK;         \
        IRFITGRP.ITEM.CODE$,               \ Item code                 \
        IRFITGRP.GROUP.NO%                 ! Group number of the item

    WRITE.IRFITGRP.UNLOCK = 0

    EXIT FUNCTION

WRITE.IRFITGRP.UNLOCK.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION

\***********************************************************************
\***
\***    WRITE.IRFITGRP.UNLOCK.HOLD
\***
\***    Write hold IRFITGRP file record and unlock
\***
\***********************************************************************
FUNCTION WRITE.IRFITGRP.UNLOCK.HOLD PUBLIC

    INTEGER*1 WRITE.IRFITGRP.UNLOCK.HOLD

    WRITE.IRFITGRP.UNLOCK.HOLD = 1

    FORMAT.STRING$ = "C3,I1"

    IF END #IRFITGRP.SESS.NUM% THEN WRITE.IRFITGRP.UNLOCK.HOLD.ERROR
    WRITE FORM FORMAT.STRING$; HOLD #IRFITGRP.SESS.NUM% AUTOUNLOCK;    \
        IRFITGRP.ITEM.CODE$,               \ Item code                 \
        IRFITGRP.GROUP.NO%                 ! Group number of the item

    WRITE.IRFITGRP.UNLOCK.HOLD = 0

    EXIT FUNCTION

WRITE.IRFITGRP.UNLOCK.HOLD.ERROR:

    FILE.OPERATION$     = "W"
    CURRENT.REPORT.NUM% = IRFITGRP.REPORT.NUM%
    CURRENT.CODE$       = IRFITGRP.ITEM.CODE$

END FUNCTION
