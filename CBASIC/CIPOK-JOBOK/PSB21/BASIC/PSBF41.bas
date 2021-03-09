\*******************************************************************************! CCSk
\*******************************************************************************! CCSk
\***                                                                            ! CCSk
\*** IMPORTANT                                                                  ! CCSk
\*** =========                                                                  ! CCSk
\***    All references to UPDT.IRF.UPDT have been commented out and replaced    ! CCSk
\***    with UPDT.IRF.TIF.UPDT due to the following issue found post MCF:       ! CCSk
\***                                                                            ! CCSk
\***    Node ID check removed as it is preventing the TIF from being updated in ! CCSk
\***    Single MCF controller stores ie. MCF has changed the single DE nodes    ! CCSk
\***    to CE. This change should have been done as part of the MCF project,    ! CCSk
\***    however, the Business were not willing to pay for the relinking and     ! CCSk
\***    testing of some 200+ programs. There is a slight processing overhead    ! CCSk
\***    in dual-MCF stores, since the TIF will now be updated in them as well,  ! CCSk
\***    however the impact is small and TOF does not run in this environment.   ! CCSk
\***                                                                            ! CCSk
\***    If you wish to use the old UPDT.IRF.UPDT function then you will need to ! CCSk
\***    either:                                                                 ! CCSk
\***                                                                            ! CCSk
\***          1. Check out the previous OBJs, PSBF19e.J86 and update the .INP   ! CCSk
\***             and .MAK to include the functions PSBF19, PSBF41 & PSBF42      ! CCSk
\***       or                                                                   ! CCSk
\***          2. Check out the previous PSBF19e.J86 and function library:       ! CCSk
\***             FUNLIB.L86                                                     ! CCSk
\***                                                                            ! CCSk
\*******************************************************************************! CCSk
\*******************************************************************************! CCSk


!******************************************************************************
!******************************************************************************
!***
!***            PROGRAM         :       PSBF41.BAS
!***
!***            DESCRIPTION     :       Update all IRF barcodes function
!***
!***            AUTHOR          :       Brian Greenfield
!***            DATE WRITTEN    :       July 2002
!***
!***        Pass in the Boots Item Code (7 digits) in the UPDATE.ITEM.CODE$
!***        variable and this function updates every single record linked to
!***        that item code on the IRF.
!***        You MUST have the IDF, & IEF open when calling this function.
!***        You MUST have the IRF open using the OPEN.IRF.UPDT function call.
!***
!***        Revision A    Brian Greenfield     July 2003
!***        Minor change so that if an IEF record is not found, the program
!***        stops correctly. It was simply looping because the test was
!***        WHILE NOT END.OF.IEF.CHAIN AND NOT ERROR.FLAG
!***        instead of the new line
!***        WHILE (NOT END.OF.IEF.CHAIN) AND (ERROR.FLAG <> 0)
!***        The original NOT test assumes TRUE is -1, not 1.
!***
!***        Revision B    Julia Stones    November 2003
!***        Minor change so that if an item has more than 2 bar codes
!***        The IEF records are read and the deal information updated on
!***        the IRF for each barcode record.
!***
\***        Revision C       Charles Skadorwa               25th September 2013
\***        F261 Gift Card Mall IIN Range Extension Project - Commented ! CCSk
\***
!***
!******************************************************************************
!******************************************************************************

%INCLUDE PSBF41G.J86

!******************************************************************************

STRING ACD.FLAG$
STRING GROUP.CODE$
STRING IRF.LOCKED.FLAG$
STRING NON.GROUP.CODE$
STRING NULL.BAR.CODE$

INTEGER*2 NO.OF.BAR.CODES%
INTEGER*2 BAR.CODE.COUNT%

INTEGER*1 ERROR.FLAG
INTEGER*1 END.OF.IEF.CHAIN

!******************************************************************************

%INCLUDE IDFDEC.J86     !IDF FUNCTIONS
%INCLUDE IRFDEC.J86     !IRF FUNCTIONS
%INCLUDE IEFDEC.J86     !IEF FUNCTIONS
%INCLUDE PSBF19G.J86    !UPDATE IRF GLOBALS

%INCLUDE IDFEXT.J86     !IDF EXTERNALS
%INCLUDE IRFEXT.J86     !IRF EXTERNALS
%INCLUDE IEFEXT.J86     !IEF EXTERNALS
%INCLUDE PSBF19E.J86    !UPDATE IRF EXTERNALS

!******************************************************************************

FUNCTION UPDATE.ALL.IRF.BAR.CODES(UPDATE.ITEM.CODE$) PUBLIC

    STRING UPDATE.ITEM.CODE$
    INTEGER*1 UPDATE.ALL.IRF.BAR.CODES

    GROUP.CODE$     = PACK$("2" + STRING$(9,"0"))
    NON.GROUP.CODE$ = PACK$(STRING$(10,"0"))
    NULL.BAR.CODE$  = PACK$(STRING$(12,"0"))

    ACD.FLAG$ = "CHANGE"
    IRF.LOCKED.FLAG$ = "N"

    ERROR.FLAG = 0

    IDF.BOOTS.CODE$ = PACK$("0" + UPDATE.ITEM.CODE$)
    IF READ.IDF = 0 THEN BEGIN

       !READ DATA FROM ITEM CODE WHICH HAS BEEN UPDATED BY CALLING PROGRAM
       IRF.BAR.CODE$ = PACK$(STRING$(16,"0")) + PACK$(LEFT$(UPDATE.ITEM.CODE$,6))

       IF READ.IRF = 0 THEN BEGIN

          NO.OF.BAR.CODES% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))

          IF (IDF.BIT.FLAGS.1% AND 10000000B) <> 0 THEN BEGIN !TEST THE GROUP CODE FLAG

             !GROUP CODES SUCH AS 309 HAVE TWO IRF RECORDS, ONE WITH A 2 AT THE BEGINNING OF THE PACKED IRF KEY
             IRF.BAR.CODE$ = NON.GROUP.CODE$ + IDF.FIRST.BAR.CODE$
             CALL CONCAT.NEW.IRF.DATA$
            !IF UPDT.IRF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1     !CCSk
             IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1 !CCSk

             IRF.BAR.CODE$ = GROUP.CODE$ + IDF.FIRST.BAR.CODE$
             IF NOT ERROR.FLAG THEN BEGIN
                CALL CONCAT.NEW.IRF.DATA$
               !IF UPDT.IRF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1     !CCSk
                IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1 !CCSk
             ENDIF

             IRF.BAR.CODE$ = GROUP.CODE$ + IDF.SECOND.BAR.CODE$
             IF NOT ERROR.FLAG THEN BEGIN
                CALL CONCAT.NEW.IRF.DATA$
               !IF UPDT.IRF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1     !CCSk
                IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1 !CCSk
             ENDIF

          ENDIF ELSE IF IDF.SECOND.BAR.CODE$ <> NULL.BAR.CODE$ THEN BEGIN

             IRF.BAR.CODE$ = NON.GROUP.CODE$ + IDF.SECOND.BAR.CODE$
             CALL CONCAT.NEW.IRF.DATA$
            !IF UPDT.IRF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1     !CCSk
             IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) THEN ERROR.FLAG = 1 !CCSk

             IF NO.OF.BAR.CODES% > 2 AND (ERROR.FLAG = 0) THEN BEGIN      !BJAS

                !GO THROUGH IEF BAR CODES

                IEF.BOOTS.CODE.BAR.CODE$ = IRF.BOOTS.CODE$ + IDF.SECOND.BAR.CODE$

                IF READ.IEF = 0 THEN BEGIN

                   BAR.CODE.COUNT% = 2
                   END.OF.IEF.CHAIN = 0

                   WHILE (NOT END.OF.IEF.CHAIN) AND (ERROR.FLAG = 0) !ABG !BJAS

                      IRF.BAR.CODE$ = NON.GROUP.CODE$ + IEF.NEXT.BAR.CODE$
                      CALL CONCAT.NEW.IRF.DATA$
                     !IF UPDT.IRF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) = 0 THEN BEGIN     !CCSk
                      IF UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$, ACD.FLAG$, IRF.LOCKED.FLAG$) = 0 THEN BEGIN !CCSk

                         IEF.BOOTS.CODE.BAR.CODE$ = IRF.BOOTS.CODE$ + IEF.NEXT.BAR.CODE$

                         IF READ.IEF = 0 THEN BEGIN

                            BAR.CODE.COUNT% = BAR.CODE.COUNT% + 1

                            IF (IEF.NEXT.BAR.CODE$ = NULL.BAR.CODE$) OR \
                               (BAR.CODE.COUNT% = NO.OF.BAR.CODES%) THEN BEGIN
                               END.OF.IEF.CHAIN = -1
                            ENDIF

                         ENDIF ELSE BEGIN
                            ERROR.FLAG = 1
                         ENDIF
                      ENDIF ELSE BEGIN
                         ERROR.FLAG = 1
                      ENDIF

                   WEND

                ENDIF ELSE BEGIN
                   ERROR.FLAG = 1
                ENDIF

             ENDIF

          ENDIF

       ENDIF ELSE BEGIN
          ERROR.FLAG = 1
       ENDIF

    ENDIF ELSE BEGIN
       ERROR.FLAG = 1
    ENDIF

    UPDATE.ALL.IRF.BAR.CODES = ERROR.FLAG

END FUNCTION


