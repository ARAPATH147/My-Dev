\*******************************************************************************
\*******************************************************************************
\***                                                                           *
\***    SOURCE FOR GSA AUTHORISATION FILE PUBLIC FUNCTIONS                     *
\***                                                                           *
\***        REFERENCE   :  AFFUN (BAS)                                         *
\***                                                                           *
\***        FILE TYPE   :  Keyed                                               *
\***                                                                           *
\***    The GSA Authorisation File (EALAUTH.DAT) should not be confused with   *
\***    the Operating System Authorisation File (ADXCSOUF.DAT).                *
\***                                                                           *
\***    VERSION A.              ROBERT COWEY.                     18 AUG 1992. *
\***    Original version created by merging AFFNSD and AFSETD.                 *
\***                                                                           *
\***    VERSION B.              Steve Windsor                        Nov 1992. *
\***    Additions required for security changes - supervisor flag & operator   *
\***    model.                                                                 *
\***                                                                           *
\***    VERSION C.              ROBERT COWEY.                     03 MAR 1994. *
\***    Defined fields DATE.PSWD.CHANGE$, and MODEL.FLAGS.1% and ...2%.        *
\***                                                                           *
\***    VERSION D.             Mike Bishop                          30 JUN 2004
\***    Defined field AF.STAFF.NUM$ AF.EMPLOYEE.FLAG$
\***
\***    VERSION E.              Alan Carr                            4 Oct 2004
\***    Defined field AF.RECEIPT.NAME$
\***
\***    VERSION F.              Alan Carr                            4 Oct 2004
\***    Defined field AF.GROUP.CODE$
\***
\***    REVISION 1.6.                ROBERT COWEY.                15 JUN 2009.
\***    Changes for A9C POS improvements project.
\***    Used up last available three bytes of EALAUTH user data by redefining 
\***    remaining AF.USER$ variable as AF.BIRTH.DATE$.
\***
\***    REVISION 1.7.                ROBERT COWEY.                22 JUN 2009.
\***    Changes for A9C POS improvements project creating PSB99.286 Rv 1.8.
\***    Defect 3247 - Redefined AF.BIRTH.DATE$ format within AFDEC.J86.
\***    Description text change only - No code changes to this file.
\***
\*******************************************************************************
\*******************************************************************************


    %INCLUDE AFDEC.J86 ! AF variable declarations                     ! CRC

    STRING GLOBAL \
        CURRENT.CODE$, \
        FILE.OPERATION$

    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%


FUNCTION AF.SET PUBLIC

    INTEGER*2 AF.SET
    AF.SET EQ 1

    AF.FILE.NAME$  EQ "AF" ! Defines local copy of EALAUTH.DAT
    AF.REPORT.NUM% EQ  2
    AF.RECL%       EQ  80

    AF.SET EQ 0

END FUNCTION


FUNCTION READ.AF PUBLIC

    INTEGER*2 READ.AF
    READ.AF EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.READ.AF
    READ FORM "T5,C4,C1,3I2,9I1,C20,4I1,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \  ! CRC
      # AF.SESS.NUM% \
        KEY AF.OPERATOR.NO$; \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.INDICAT1%, \
            AF.INDICAT2%, \
            AF.INDICAT3%, \
            AF.INDICAT4%, \
            AF.INDICAT5%, \
            AF.INDICAT6%, \
            AF.INDICAT7%, \
            AF.INDICAT8%, \
            AF.INDICAT9%, \
            AF.INDICAT10%, \
            AF.INDICAT11%, \
            AF.INDICAT12%, \
            AF.OPERATOR.NAME$, \
            AF.INDICAT13%, \
            AF.INDICAT14%, \
            AF.INDICAT15%, \
            AF.INDICAT16%, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    READ.AF EQ 0
    EXIT FUNCTION

ERROR.READ.AF:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.AF.LOCKED PUBLIC

    INTEGER*2 READ.AF.LOCKED
    READ.AF.LOCKED EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.READ.AF.LOCKED
    READ FORM "T5,C4,C1,3I2,9I1,C20,4I1,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \  ! CRC
      # AF.SESS.NUM% AUTOLOCK \
        KEY AF.OPERATOR.NO$; \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.INDICAT1%, \
            AF.INDICAT2%, \
            AF.INDICAT3%, \
            AF.INDICAT4%, \
            AF.INDICAT5%, \
            AF.INDICAT6%, \
            AF.INDICAT7%, \
            AF.INDICAT8%, \
            AF.INDICAT9%, \
            AF.INDICAT10%, \
            AF.INDICAT11%, \
            AF.INDICAT12%, \
            AF.OPERATOR.NAME$, \
            AF.INDICAT13%, \
            AF.INDICAT14%, \
            AF.INDICAT15%, \
            AF.INDICAT16%, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    READ.AF.LOCKED EQ 0
    EXIT FUNCTION

ERROR.READ.AF.LOCKED:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.AF.ABREV PUBLIC

    INTEGER*2 READ.AF.ABREV
    READ.AF.ABREV EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.READ.AF.ABREV
    READ FORM "T5,C4,C1,C15,C20,C4,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \       ! CRC
      # AF.SESS.NUM% \
        KEY AF.OPERATOR.NO$; \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.FLAGS.01.12$, \
            AF.OPERATOR.NAME$, \
            AF.FLAGS.13.16$, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    READ.AF.ABREV EQ 0
    EXIT FUNCTION

ERROR.READ.AF.ABREV:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION READ.AF.ABREV.LOCKED PUBLIC

    INTEGER*2 READ.AF.ABREV.LOCKED
    READ.AF.ABREV.LOCKED EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.READ.AF.ABREV.LOCKED
    READ FORM "T5,C4,C1,C15,C20,C4,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \      ! CRC
      # AF.SESS.NUM% AUTOLOCK \
        KEY AF.OPERATOR.NO$; \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.FLAGS.01.12$, \
            AF.OPERATOR.NAME$, \
            AF.FLAGS.13.16$, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    READ.AF.ABREV.LOCKED EQ 0
    EXIT FUNCTION

ERROR.READ.AF.ABREV.LOCKED:

    FILE.OPERATION$     EQ "R"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.AF.UNLOCK PUBLIC

    INTEGER*2 WRITE.AF.UNLOCK
    WRITE.AF.UNLOCK EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.WRITE.AF.UNLOCK
    WRITE FORM "C4,C4,C1,3I2,9I1,C20,4I1,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \ ! CRC
      # AF.SESS.NUM% AUTOUNLOCK; \
            AF.OPERATOR.NO$, \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.INDICAT1%, \
            AF.INDICAT2%, \
            AF.INDICAT3%, \
            AF.INDICAT4%, \
            AF.INDICAT5%, \
            AF.INDICAT6%, \
            AF.INDICAT7%, \
            AF.INDICAT8%, \
            AF.INDICAT9%, \
            AF.INDICAT10%, \
            AF.INDICAT11%, \
            AF.INDICAT12%, \
            AF.OPERATOR.NAME$, \
            AF.INDICAT13%, \
            AF.INDICAT14%, \
            AF.INDICAT15%, \
            AF.INDICAT16%, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    WRITE.AF.UNLOCK EQ 0
    EXIT FUNCTION

ERROR.WRITE.AF.UNLOCK:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.AF.ABREV PUBLIC

    INTEGER*2 WRITE.AF.ABREV
    WRITE.AF.ABREV EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.WRITE.AF.ABREV
    WRITE FORM "C4,C4,C1,C15,C20,C4,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \      ! CRC
      # AF.SESS.NUM%; \
            AF.OPERATOR.NO$, \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.FLAGS.01.12$, \
            AF.OPERATOR.NAME$, \
            AF.FLAGS.13.16$, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    WRITE.AF.ABREV EQ 0
    EXIT FUNCTION

ERROR.WRITE.AF.ABREV:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION


FUNCTION WRITE.AF.ABREV.UNLOCK PUBLIC

    INTEGER*2 WRITE.AF.ABREV.UNLOCK
    WRITE.AF.ABREV.UNLOCK EQ 1

    IF END # AF.SESS.NUM% THEN ERROR.WRITE.AF.ABREV.UNLOCK
    WRITE FORM "C4,C4,C1,C15,C20,C4,C12,C4,C1,C1,C3,C3,I2,I2,C1,C3"; \      ! CRC
      # AF.SESS.NUM% AUTOUNLOCK; \
            AF.OPERATOR.NO$, \
            AF.PASSWORD$, \
            AF.OPTIONS.KEY$, \
            AF.FLAGS.01.12$, \
            AF.OPERATOR.NAME$, \
            AF.FLAGS.13.16$, \
            AF.RECEIPT.NAME$, \                                        ! AJC
            AF.STAFF.NUM$, \                                            DMB
            AF.EMPLOYEE.FLAG$, \                                        DMB
            AF.GROUP.CODE$, \                                           AJC
            AF.BIRTH.DATE$, \                                          ! 1.6 RC
            AF.DATE.PSWD.CHANGE$, \                                    ! CRC
            AF.MODEL.FLAGS.1%, \                                       ! CRC
            AF.MODEL.FLAGS.2%, \                                       ! CRC
            AF.SUP.FLAG$, \                                             BSJW
            AF.OP.MODEL$                                               !BSJW

    WRITE.AF.ABREV.UNLOCK EQ 0
    EXIT FUNCTION

ERROR.WRITE.AF.ABREV.UNLOCK:

    FILE.OPERATION$     EQ "W"
    CURRENT.REPORT.NUM% EQ AF.REPORT.NUM%
    CURRENT.CODE$       EQ AF.OPERATOR.NO$

    EXIT FUNCTION

END FUNCTION

