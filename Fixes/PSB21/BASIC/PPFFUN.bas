\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PPFFUN.bas  $
\***
\***   $Revision:   1.2  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PPFFUN.bav  $
\***   
\***      Rev 1.2   08 Jul 2004 13:51:26   dev88ps
\***   Added MARKDOWN flag.
\***   
\***      Rev 1.1   07 Dec 1994 13:07:34   DEVMJPS
\***   Update INCLUDE code for PVCS
\***   
\******************************************************************************
\******************************************************************************
\REM
\*******************************************************************************
\*******************************************************************************
\***
\***    %INCLUDE FOR PENDING PRICES FILES PUBLIC FUNCTIONS
\***
\***        REFERENCE   :   PPFFUN (BAS)
\***
\***        FILE TYPE   :   Sequential
\***
\***    VERSION D.              ROBERT COWEY.                       23 AUG 1993.
\***    Original version created by combining PPFIFNSC, PPFOFNSC, PPFISETC, and 
\***    PPFOSETC.
\***
\***    VERSION E.              ROBERT COWEY and STEVE PERKINS.     22 OCT 1993.
\***    Removed SPLAN.ID$ from RPD and Deal records.
\***    Defined Link-Save Multi-Buy Deal record.
\***    Added two extra functions READ.PPFI.SHORT and PPFI.LOAD.DEAL.FIELDS
\***    to help when processing LODF and PPFI in parallel.
\***
\***    VERSION F               BRIAN GREENFIELD                    24 JUN 2004
\***    Added in PPF.MARKDOWN$ flag into the RPD record for mark-down items.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE PPFDEC.J86      ! PPF variable declarations                   ! ERC

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION PPFI.SET PUBLIC

    INTEGER*2 PPFI.SET
    PPFI.SET EQ 1

    PPFI.FILE.NAME$  EQ "PPFI"
    PPFI.REPORT.NUM% EQ  12

    PPFI.SET EQ 0

END FUNCTION


FUNCTION PPFO.SET PUBLIC

    INTEGER*2 PPFO.SET
    PPFO.SET EQ 1

    PPFO.FILE.NAME$  EQ "PPFO"
    PPFO.REPORT.NUM% EQ  13

    PPFO.SET EQ 0

END FUNCTION


FUNCTION READ.PPFI PUBLIC

    INTEGER*2 READ.PPFI
    READ.PPFI EQ 1

    CURRENT.CODE$ EQ MID$(PPF.RECORD$,1,10)

    IF END # PPFI.SESS.NUM% THEN READ.PPFI.IF.END
    READ # PPFI.SESS.NUM%; PPF.RECORD$

    PPF.BOOTS.CODE$    EQ MID$(PPF.RECORD$,1,7)
    PPF.REC.TYPE.FLAG$ EQ MID$(PPF.RECORD$,8,1)

    IF PPF.REC.TYPE.FLAG$ EQ "R" THEN \ ! RPD record
        BEGIN
        IF LEN(PPF.RECORD$) = 29 THEN PPF.RECORD$ = PPF.RECORD$ + "N"  ! FBG
        PPF.DATE.DUE$     EQ  MID$(PPF.RECORD$,9,6)
        PPF.RPD.NO$       EQ  MID$(PPF.RECORD$,15,5)
        PPF.STATUS.FLAG$  EQ  MID$(PPF.RECORD$,20,1)
        PPF.INC.DEC.FLAG$ EQ  MID$(PPF.RECORD$,21,1)
        PPF.PRICE$        EQ  MID$(PPF.RECORD$,22,8)                   
        PPF.MARKDOWN$     EQ  MID$(PPF.RECORD$,30,1)                   !FBG
!   Line deleted                                                       ! ERC
        GOTO READ.PPFI.OKAY
        ENDIF

    IF PPF.REC.TYPE.FLAG$ EQ "D" THEN \ ! Deal record
        BEGIN
        PPF.DEAL.TYPE$     EQ  MID$(PPF.RECORD$,9,1)
        PPF.DEAL.QUANTITY$ EQ  MID$(PPF.RECORD$,10,2)
        PPF.SPECIAL.PRICE$ EQ  MID$(PPF.RECORD$,12,5)
        PPF.M.P.GROUP$     EQ  MID$(PPF.RECORD$,17,2)
        PPF.FIRST.DATE$    EQ  MID$(PPF.RECORD$,19,6)
        PPF.LAST.DATE$     EQ  MID$(PPF.RECORD$,25,6)
        PPF.EFFECT.FLAG$   EQ  MID$(PPF.RECORD$,31,1)
!   Line deleted                                                       ! ERC
        GOTO READ.PPFI.OKAY
        ENDIF

    IF PPF.REC.TYPE.FLAG$ EQ "B" THEN \ ! Link-Save Multi-Buy          ! ERC
        BEGIN                           ! Deal record                  ! ERC
        PPF.DEAL.ID$          EQ  MID$(PPF.RECORD$,9,1)                ! ERC
        PPF.PAIR.TRIP.FLAG$   EQ  MID$(PPF.RECORD$,10,1)               ! ERC
        PPF.3FOR2.FLAG$       EQ  MID$(PPF.RECORD$,11,1)               ! ERC
        PPF.DEAL.LIMIT$       EQ  MID$(PPF.RECORD$,12,2)               ! ERC
        PPF.DEAL.NUM$         EQ  MID$(PPF.RECORD$,14,4)               ! ERC
        PPF.SAVED.AMOUNT$     EQ  MID$(PPF.RECORD$,18,4)               ! ERC
        PPF.DEAL.DATE.START$  EQ  MID$(PPF.RECORD$,22,6)               ! ERC
        PPF.DEAL.DATE.FINISH$ EQ  MID$(PPF.RECORD$,28,6)               ! ERC
        PPF.ACTIVE.FLAG$      EQ  MID$(PPF.RECORD$,34,1)               ! ERC
        GOTO READ.PPFI.OKAY                                            ! ERC
        ENDIF                                                          ! ERC

    IF PPF.BOOTS.CODE$ EQ "9999999" THEN \ ! Trailer record
        BEGIN
        PPF.REC.COUNT$ EQ MID$(PPF.RECORD$,9,5)
        GOTO READ.PPFI.OKAY
        ENDIF

READ.PPFI.OKAY:

    READ.PPFI EQ 0
    EXIT FUNCTION

READ.PPFI.IF.END:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ  PPFI.REPORT.NUM%
    CURRENT.CODE$       EQ  CURRENT.CODE$ ! Previous successful read (if any)

    EXIT FUNCTION
 
END FUNCTION  


FUNCTION WRITE.PPFO PUBLIC

    INTEGER*2 WRITE.PPFO
    WRITE.PPFO EQ 1

    IF END # PPFO.SESS.NUM% THEN WRITE.PPFO.IF.END

    IF PPF.REC.TYPE.FLAG$ EQ "R" THEN \ ! RPD record
        BEGIN
        PPF.RECORD$ EQ \
      PPF.BOOTS.CODE$ + \
          PPF.REC.TYPE.FLAG$ + \
          PPF.DATE.DUE$ + \
          PPF.RPD.NO$ + \
          PPF.STATUS.FLAG$ + \
          PPF.INC.DEC.FLAG$ + \
          PPF.PRICE$ + \                                        
          PPF.MARKDOWN$                                                ! FBG
!   Line deleted                                                       ! ERC
        WRITE # PPFO.SESS.NUM%; PPF.RECORD$
        GOTO WRITE.PPFO.OKAY
        ENDIF

    IF PPF.REC.TYPE.FLAG$ EQ "D" THEN \ ! Deal price
        BEGIN
        PPF.RECORD$ EQ \
          PPF.BOOTS.CODE$ + \
          PPF.REC.TYPE.FLAG$ + \
          PPF.DEAL.TYPE$ + \
          PPF.DEAL.QUANTITY$ +  \
          PPF.SPECIAL.PRICE$ + \
          PPF.M.P.GROUP$ + \
          PPF.FIRST.DATE$ + \
          PPF.LAST.DATE$ + \
          PPF.EFFECT.FLAG$
!   Line deleted                                                       ! ERC
        WRITE # PPFO.SESS.NUM%; PPF.RECORD$
        GOTO WRITE.PPFO.OKAY
        ENDIF

    IF PPF.REC.TYPE.FLAG$ EQ "B" THEN \ ! Link-Save Multi-Buy          ! ERC
        BEGIN                           ! Deal record                  ! ERC
        PPF.RECORD$ EQ \                                               ! ERC
          PPF.BOOTS.CODE$ + \                                          ! ERC
          PPF.REC.TYPE.FLAG$ + \                                       ! ERC
          PPF.DEAL.ID$ + \                                             ! ERC
          PPF.PAIR.TRIP.FLAG$ + \                                      ! ERC
          PPF.3FOR2.FLAG$ + \                                          ! ERC
          PPF.DEAL.LIMIT$ + \                                          ! ERC
          PPF.DEAL.NUM$ + \                                            ! ERC
          PPF.SAVED.AMOUNT$ + \                                        ! ERC
          PPF.DEAL.DATE.START$ + \                                     ! ERC
          PPF.DEAL.DATE.FINISH$ + \                                    ! ERC
          PPF.ACTIVE.FLAG$                                             ! ERC
        WRITE # PPFO.SESS.NUM%; PPF.RECORD$                            ! ERC
        GOTO WRITE.PPFO.OKAY                                           ! ERC
        ENDIF                                                          ! ERC

    IF PPF.BOOTS.CODE$ EQ "9999999" THEN \ ! Trailer record
        BEGIN
        PPF.RECORD$ EQ \
          PPF.BOOTS.CODE$ + \
          PPF.REC.TYPE.FLAG$ + \
          PPF.REC.COUNT$
        WRITE # PPFO.SESS.NUM%; PPF.RECORD$
        GOTO WRITE.PPFO.OKAY
        ENDIF

WRITE.PPFO.OKAY:

    WRITE.PPFO EQ 0
    EXIT FUNCTION

WRITE.PPFO.IF.END:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ  PPFO.REPORT.NUM%
    CURRENT.CODE$       EQ  RIGHT$(PPF.RECORD$,10) ! Current write

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.PPFI.SHORT PUBLIC                                        ! ESP
                                                                       ! ESP
    INTEGER*2 READ.PPFI.SHORT                                          ! ESP
    READ.PPFI.SHORT EQ 1                                               ! ESP
                                                                       ! ESP
    CURRENT.CODE$ EQ MID$(PPF.RECORD$,1,10)                            ! ESP
                                                                       ! ESP
    IF END # PPFI.SESS.NUM% THEN READ.PPFI.IF.END                      ! ESP
    READ # PPFI.SESS.NUM%; PPF.RECORD$                                 ! ESP
                                                                       ! ESP
    READ.PPFI.SHORT EQ 0                                               ! ESP
    EXIT FUNCTION                                                      ! ESP
                                                                       ! ESP
READ.PPFI.IF.END:                                                      ! ESP
                                                                       ! ESP
    FILE.OPERATION$     EQ "R"                                         ! ESP
    CURRENT.REPORT.NUM% EQ  PPFI.REPORT.NUM%                           ! ESP
    CURRENT.CODE$       EQ  CURRENT.CODE$ ! Previous successful read (if any)
                                                                       ! ESP
    EXIT FUNCTION                                                      ! ESP
                                                                       ! ESP
END FUNCTION                                                           ! ESP


FUNCTION PPFI.LOAD.FIELDS(TEMP.PPFI.RECORD$) PUBLIC                    ! ESP
                                                                       ! ESP
STRING  TEMP.PPFI.RECORD$                                              ! ESP
                                                                       ! ESP
    PPF.BOOTS.CODE$    EQ MID$(TEMP.PPFI.RECORD$,1,7)                  ! ESP
    PPF.REC.TYPE.FLAG$ EQ MID$(TEMP.PPFI.RECORD$,8,1)                  ! ESP
                                                                       ! ESP
    IF PPF.REC.TYPE.FLAG$ EQ "R" THEN \ ! RPD record                   ! ESP
        BEGIN                                                          ! ESP
        PPF.DATE.DUE$     EQ  MID$(TEMP.PPFI.RECORD$,9,6)              ! ESP
        PPF.RPD.NO$       EQ  MID$(TEMP.PPFI.RECORD$,15,5)             ! ESP
        PPF.STATUS.FLAG$  EQ  MID$(TEMP.PPFI.RECORD$,20,1)             ! ESP
        PPF.INC.DEC.FLAG$ EQ  MID$(TEMP.PPFI.RECORD$,21,1)             ! ESP
        PPF.PRICE$        EQ  MID$(TEMP.PPFI.RECORD$,22,8)             ! ESP
        PPF.MARKDOWN$     EQ  MID$(TEMP.PPFI.RECORD$,30,1)             ! FBG
        GOTO EXIT.FUNCTION                                             ! ESP
        ENDIF                                                          ! ESP
                                                                       ! ESP
    IF PPF.REC.TYPE.FLAG$ EQ "D" THEN \ ! Deal record                  ! ESP
        BEGIN                                                          ! ESP
        PPF.DEAL.TYPE$     EQ  MID$(TEMP.PPFI.RECORD$,9,1)             ! ESP
        PPF.DEAL.QUANTITY$ EQ  MID$(TEMP.PPFI.RECORD$,10,2)            ! ESP
        PPF.SPECIAL.PRICE$ EQ  MID$(TEMP.PPFI.RECORD$,12,5)            ! ESP
        PPF.M.P.GROUP$     EQ  MID$(TEMP.PPFI.RECORD$,17,2)            ! ESP
        PPF.FIRST.DATE$    EQ  MID$(TEMP.PPFI.RECORD$,19,6)            ! ESP
        PPF.LAST.DATE$     EQ  MID$(TEMP.PPFI.RECORD$,25,6)            ! ESP
        PPF.EFFECT.FLAG$   EQ  MID$(TEMP.PPFI.RECORD$,31,1)            ! ESP
        GOTO EXIT.FUNCTION                                             ! ESP
        ENDIF                                                          ! ESP
                                                                       ! ESP
    IF PPF.REC.TYPE.FLAG$ EQ "B" THEN \ ! Link-Save Multi-Buy          ! ESP
        BEGIN                           ! Deal record                  ! ESP
        PPF.DEAL.ID$          EQ  MID$(TEMP.PPFI.RECORD$,9,1)          ! ESP
        PPF.PAIR.TRIP.FLAG$   EQ  MID$(TEMP.PPFI.RECORD$,10,1)         ! ESP
        PPF.3FOR2.FLAG$       EQ  MID$(TEMP.PPFI.RECORD$,11,1)         ! ESP
        PPF.DEAL.LIMIT$       EQ  MID$(TEMP.PPFI.RECORD$,12,2)         ! ESP
        PPF.DEAL.NUM$         EQ  MID$(TEMP.PPFI.RECORD$,14,4)         ! ESP
        PPF.SAVED.AMOUNT$     EQ  MID$(TEMP.PPFI.RECORD$,18,4)         ! ESP
        PPF.DEAL.DATE.START$  EQ  MID$(TEMP.PPFI.RECORD$,22,6)         ! ESP
        PPF.DEAL.DATE.FINISH$ EQ  MID$(TEMP.PPFI.RECORD$,28,6)         ! ESP
        PPF.ACTIVE.FLAG$      EQ  MID$(TEMP.PPFI.RECORD$,34,1)         ! ESP
        GOTO EXIT.FUNCTION                                             ! ESP
        ENDIF                                                          ! ESP
                                                                       ! ESP
    IF PPF.BOOTS.CODE$ EQ "9999999" THEN \ ! Trailer record            ! ESP
        BEGIN                                                          ! ESP
        PPF.REC.COUNT$ EQ MID$(TEMP.PPFI.RECORD$,9,5)                  ! ESP
        GOTO EXIT.FUNCTION                                             ! ESP
        ENDIF                                                          ! ESP
                                                                       ! ESP
    EXIT.FUNCTION:                                                     ! ESP
                                                                       ! ESP
END FUNCTION                                                           ! ESP
                                                                       
