\*******************************************************************************! DCSk
\***                                                                            ! DCSk
\*** IMPORTANT                                                                  ! DCSk
\*** =========                                                                  ! DCSk
\***    All references to UPDT.IRF.UPDT have been commented out and replaced    ! DCSk
\***    with UPDT.IRF.TIF.UPDT due to the following issue found post MCF:       ! DCSk
\***                                                                            ! DCSk
\***    Node ID check removed as it is preventing the TIF from being updated in ! DCSk
\***    Single MCF controller stores ie. MCF has changed the single DE nodes    ! DCSk
\***    to CE. This change should have been done as part of the MCF project,    ! DCSk
\***    however, the Business were not willing to pay for the relinking and     ! DCSk
\***    testing of some 200+ programs. There is a slight processing overhead    ! DCSk
\***    in dual-MCF stores, since the TIF will now be updated in them as well,  ! DCSk
\***    however the impact is small and TOF does not run in this environment.   ! DCSk
\***                                                                            ! DCSk
\***    If you wish to use the old UPDT.IRF.UPDT function then you will need to ! DCSk
\***    either:                                                                 ! DCSk
\***                                                                            ! DCSk
\***          1. Check out the previous OBJs, PSBF19e.J86 and update the .INP   ! DCSk
\***             and .MAK to include the functions PSBF19, PSBF41 & PSBF42      ! DCSk
\***       or                                                                   ! DCSk
\***          2. Check out the previous PSBF19e.J86 and function library:       ! DCSk
\***             FUNLIB.L86                                                     ! DCSk
\***                                                                            ! DCSk
\*******************************************************************************! DCSk
\*******************************************************************************! DCSk


!******************************************************************************
!******************************************************************************
!***
!***            PROGRAM         :       PSBF42.BAS
!***
!***            DESCRIPTION     :       Update deal information for an item
!***
!***            AUTHOR          :       Brian Greenfield
!***            DATE WRITTEN    :       July 2002
!***
!***        Pass in the PACKED item code (3 bytes) in the PACKED.CODE$
!***        variable and this function ensures that the deal data for that
!***        item contains only active deals and lists.
!***        Pass in the processing date for the active deal test (in the
!***        DATE$ format.)
!***        The function then calls the UPDATE.IRF function in PSBF41.
!***        You MUST have the DEAL, DINF, IDF, & IEF open when calling this
!***        function.
!***        You MUST have the IRF open using the OPEN.IRF.UPDT function call.
!***
!***        The Item Code should be passed as a packed 3 byte string.
!***
!***        WARNING! This function reloads deal data, save your values!
!***
!******************************************************************************
!***
!***        VERSION B           STUART WILLIAM MCCONNACHIE          OCT 2003
!***
!***        Used buffered read of DEAL file to increase performance.
!***        Added save of DINF values using new DINF functionality.
!***        Added save of DEAL values using new DEAL functionality.
!***        This allows this function to be called from code which is itself
!***        reading items on the DINF chains (to find all items in a deal
!***        for example.
!***
!***        VERSION C           STUART WILLIAM MCCONNACHIE          FEB 2004
!***
!***        The loop reading the DINF would get stuck if it encountered a
!***        deal on the DINF but could not read that deals details from the
!***        DEAL file.  This was due to a faulty NOT operator on a non-
!***        boolean value (If ERROR.FLAG = 01H then NOT ERROR.FLAG = 0FEH).
!***        Fixed this, and just moved to the next deal on the DINF in this
!***        situation.  Not having a deal on the DEAL file is not such a
!***        serious error that we should abort adding all the other deals
!***        for that item!
!***
!***        VERSION D           Charles Skadorwa                    SEP 2013
\***        F261 Gift Card Mall IIN Range Extension Project - Commented ! DCSk
!***
!***        VERSION E             Muthu Mariappan             13/07/2014
!***        FOD353 - Deal Limit Increase
!***        Make the following changes to UPDATE.ITEM.DEAL.INFO
!***        -  Replace DINF file functions by ITMDL file functions
!***        -  Rename any remaining variables prefixed DINF  with ITMDL
!***        -  Replace DINF Open-Close file handling with new code to
!***           handle ITMDL.
!***        -  LOCAL.PRICE% flag becomes redundant and should be removed
!***
!******************************************************************************
!******************************************************************************

%INCLUDE PSBF42G.J86

!******************************************************************************

STRING ACD.FLAG$
STRING IRF.LOCKED.FLAG$

INTEGER*1 ERROR.FLAG
INTEGER*1 DONE.INIT                                                         !BSWM
STRING SORT.ARRAY$(1)                                                       !BSWM

!******************************************************************************

%INCLUDE DEALDEC.J86    !DEAL FUNCTIONS
!%INCLUDE DINFDEC.J86   !DINF FUNCTION                                  !EMM
%INCLUDE ITMDLDEC.J86   !ITMDL FUNCTION                                 !EMM
%INCLUDE PSBF41G.J86    !UPDATE IRF FUNCTION
%INCLUDE IRFDEC.J86     !IRF FUNCTIONS
%INCLUDE PSBF18G.J86    !CALCULATE BOOTS CHECK DIGIT
%INCLUDE PSBF19G.J86    !UPDATE IRF GLOBALS

!******************************************************************************

%INCLUDE DEALEXT.J86    !DEAL EXTERNALS
!%INCLUDE DINFEXT.J86   !DINF EXTERNALS                                 !EMM
%INCLUDE ITMDLEXT.J86   !ITMDL EXTERNALS                                !EMM
%INCLUDE PSBF41E.J86    !UPDATE IRF EXTERNALS
%INCLUDE IRFEXT.J86     !IRF EXTERNALS
%INCLUDE PSBF18E.J86    !CALCULATE BOOTS CHECK DIGIT
%INCLUDE PSBF19E.J86    !UPDATE IRF EXTERNALS
%INCLUDE CSORTDEF.J86   !CSORT ROUTINE                                      !BSWM

!******************************************************************************

FUNCTION UPDATE.ITEM.DEAL.INFO(PACKED.CODE$, PROCESSING.DATE$) PUBLIC

    STRING PACKED.CODE$
    STRING PROCESSING.DATE$
    STRING DEAL.SAVED.STATE$
    STRING ITMDL.SAVED.STATE$                                           !EMM
    STRING DEAL.ITMDL.NUM$                                              !EMM

    INTEGER*2 NUMBER.FOUND                                                  !BSWM
    INTEGER*2 LOOP                                                          !BSWM
    INTEGER*2 DEAL.INDEX%                                               !EMM

    INTEGER*1 DEAL.ITMDL.LIST.ID%                                       !EMM
    INTEGER*1 RC
    INTEGER*1 UPDATE.ITEM.DEAL.INFO

    DEAL.INDEX%  = 1                                                    !EMM
    ERROR.FLAG = 0
    NUMBER.FOUND = 0

    IF NOT DONE.INIT THEN BEGIN                                             !BSWM
        CALL IRF.SET                                                        !BSWM
        DIM SORT.ARRAY$(IRF.MAX.DEALS%+1)                                   !BSWM acrc
        DONE.INIT = -1                                                      !BSWM
    ENDIF                                                                   !BSWM

    CALL ITMDL.SAVE(ITMDL.SAVED.STATE$)                                 !EMM BSWM
    CALL DEAL.SAVE(DEAL.SAVED.STATE$)                                       !BSWM

!    RC = DINF.GET.FIRST.DEAL.ITEM (PACKED.CODE$, 0) !ONLY GET NON-LOCAL PRICE ITEMS !EMM

    ITMDL.ITEM.CODE$ = PACKED.CODE$                                     !EMM

    RC = READ.ITMDL                                                     !EMM

    !FIND AND SORT THE DEALS FOR THIS ITEM
    IF RC = 0 THEN BEGIN                                                !EMM CSWM

        ! It Checks for Non-Local price items                           !EMM
        IF ITMDL.EXCLUSION$ = " " THEN BEGIN                            !EMM
            ! Each Iteration DEAL.ITMDL.NUM$ gets Deal number and
            ! DEAL.ITMDL.LIST.ID% gets List ID for that Item code
            WHILE DEAL.INDEX% <= ITMDL.DEAL.COUNT%                      !EMM

                DEAL.ITMDL.NUM$ = MID$(ITMDL.ALL.DEAL.NUM.LIST.ID$,    \!EMM
                                      (1 + ((DEAL.INDEX% - 1) * 3)),2)  !EMM
                DEAL.ITMDL.LIST.ID% =                                  \!EMM
                        VAL(UNPACK$(MID$(ITMDL.ALL.DEAL.NUM.LIST.ID$,  \!EMM
                        (DEAL.INDEX% * 3),1)))                          !EMM

                DEAL.DEAL.NUM$ = DEAL.ITMDL.NUM$                        !EMM
        IF READ.DEAL.BUFFER = 0 THEN BEGIN

            CALL DEAL.SPLIT.RECORD

            !TEST FOR ACTIVE DEAL
            IF ((DEAL.START.DATE$ <= PACK$("20" + PROCESSING.DATE$)) AND    \BSWM
                (DEAL.END.DATE$ >= PACK$("20" + PROCESSING.DATE$))) THEN BEGIN !BSWM

!                        CALL DINF.SPLIT.RECORD                         !EMM

                SORT.ARRAY$(NUMBER.FOUND +1 ) =  \
                                       DEAL.START.DATE$ +            \  !BSWM acrc
                                       DEAL.ITMDL.NUM$  +            \  !EMM BSWM
                                       CHR$(DEAL.ITMDL.LIST.ID%)        !EMM BSWM

                IF NUMBER.FOUND < IRF.MAX.DEALS% THEN BEGIN                 !BSWM
                    NUMBER.FOUND = NUMBER.FOUND + 1                         !BSWM
                ENDIF                                                       !BSWM
                        ! Replaced by a single sort after the WHILE loop
!                        CALL CSORT (VARPTR(SORT.ARRAY$(0)), NUMBER.FOUND) !EMM BSWM

            ENDIF

        ENDIF                                                               !CSWM
                DEAL.INDEX% = DEAL.INDEX% + 1                           !EMM
            WEND
        ENDIF                                                           !EMM
        !---------------------------------------!
        ! Sorting the array only if the number  !
        ! of values are greater then one        !
        !---------------------------------------!
        IF NUMBER.FOUND > 1 THEN BEGIN                                  !DMM
            CALL CSORT (VARPTR(SORT.ARRAY$(0)), NUMBER.FOUND)           !DMM
        ENDIF                                                           !DMM
!        RC = DINF.GET.NEXT.DEAL                                        !EMM CSWM

    ENDIF                                                               !EMM
!    WEND                                                               !EMM

    !NOW UPDATE ITEM ON IRF REGARDLESS OF ARRAY CONTENT
    IRF.BAR.CODE$ = PACK$(STRING$(16,"0")) + PACKED.CODE$
    IF READ.IRF.LOCK = 0 THEN BEGIN

        FOR LOOP = 0 TO IRF.MAX.DEALS% - 1                                  !BSWM
            IF LOOP < NUMBER.FOUND THEN BEGIN                               !BSWM
                IRF.DEAL.NUM$(LOOP) = MID$(SORT.ARRAY$(LOOP+1),5,2)         !BSWM acrc
                IRF.LIST.ID%(LOOP) = ASC(MID$(SORT.ARRAY$(LOOP+1),7,1))     !BSWM acrc
            ENDIF ELSE BEGIN                                                !BSWM
                IRF.DEAL.NUM$(LOOP) = PACK$("0000")                         !BSWM
                IRF.LIST.ID%(LOOP) = 0                                      !BSWM
            ENDIF                                                           !BSWM
        NEXT LOOP

        CALL CONCAT.NEW.IRF.DATA$
        ACD.FLAG$ = "CHANGE"
        IRF.LOCKED.FLAG$ = "Y"
        ! call changed - ref PSBF19
        IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) = 0 THEN BEGIN
            !POPULATE ALL IRF BARCODES
            CALL CALC.BOOTS.CODE.CHECK.DIGIT (UNPACK$(PACKED.CODE$))
            CALL UPDATE.ALL.IRF.BAR.CODES(UNPACK$(PACKED.CODE$) + F18.CHECK.DIGIT$)
        ENDIF ELSE BEGIN
            ERROR.FLAG = 1
        ENDIF

    ENDIF ELSE BEGIN
        ERROR.FLAG = 1
    ENDIF

    CALL ITMDL.RESTORE(ITMDL.SAVED.STATE$)                              !EMM BSWM
    CALL DEAL.RESTORE(DEAL.SAVED.STATE$)                                    !BSWM

    UPDATE.ITEM.DEAL.INFO = ERROR.FLAG

END FUNCTION


