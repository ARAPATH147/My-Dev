\***********************************************************************
\* 
\* BGCHECK.286               David Artiss                    14/04/2009
\*
\* Designed to be run via NFM, this will check sales and stock support,
\* LINK1 and NFMC and output whether they are working or not.
\*
\* NFM will receive 1 of 4 possible responses...
\* WRONG - This is not being run on the file server
\* GOOD - All tasks are working
\* BAD - One of the tasks is not active
\* ABEND - The program has abended
\*
\* Version B                 David Artiss                    21/07/2010
\* Improved to check for LINK1 and NFMC.
\* 
\* Version C                 Ranjith Gopalankutty            26/01/2017
\* Program was reporting only  sales suport, stock support and NFMC.
\* upgraded the program so that it reports all current applications
\* also removed checking of program LINK1. As it doesn't hold true for
\* all stores
\* 
\***********************************************************************

   STRING GLOBAL    ADX.PARM$, \
                    FAIL$

   INTEGER*2 GLOBAL RC%

   %INCLUDE ADXSERVE.J86
   %INCLUDE BASROUT.J86

    ON ERROR GOTO ERROR.DETECTED

    ! Call ADXSERVE and read record 4

    CALL  ADXSERVE(RC%,4,0,ADX.PARM$)
    IF RC% <> 0 THEN GOTO ERROR.DETECTED

    ! Work out whether on file server or not

    RC% = VAL(MID$(ADX.PARM$,25,2))
    IF (RC% AND 04H) <> 04H AND RC% <> 0 THEN BEGIN
        PRINT "WRONG"
        STOP
    ENDIF


    ! Check Sales Support

    RC% = SRCHPROC("EALCS00L*",STRING$(60,PACK$("00")))
    IF RC% <= 0 THEN FAIL$ = "Yes"

    ! Check Stock Support

    RC% = SRCHPROC("PSS35*",STRING$(60,PACK$("00")))
    IF RC% <= 0 THEN FAIL$ = "Yes"

    ! Check LINK1                                                       !BDA

    ! RC% = SRCHPROC("ADXHSNLL*",STRING$(60,PACK$("00")))               !CRG
    ! IF RC% <= 0 THEN FAIL$ = "Yes"                                    !CRG

    ! Check NFMC                                                        !BDA

    RC% = SRCHPROC("NFMC*",STRING$(60,PACK$("00")))                     !BDA
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !BDA

    ! Check TRANSACT                                                    !CRG

    RC% = SRCHPROC("TRANSACT*",STRING$(60,PACK$("00")))                 !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check BGMON                                                       !CRG

    RC% = SRCHPROC("BGMON*",STRING$(60,PACK$("00")))                    !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check QHANDLER                                                    !CRG

    RC% = SRCHPROC("QHANDLER*",STRING$(60,PACK$("00")))                 !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check QSERVER                                                     !CRG

    RC% = SRCHPROC("QSERVER*",STRING$(60,PACK$("00")))                  !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check QPROCESS                                                    !CRG

    RC% = SRCHPROC("QPROCESS*",STRING$(60,PACK$("00")))                 !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check QMON                                                        !CRG

    RC% = SRCHPROC("QMON*",STRING$(60,PACK$("00")))                     !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG

    ! Check PSB90                                                       !CRG

    RC% = SRCHPROC("PSB90*",STRING$(60,PACK$("00")))                    !CRG
    IF RC% <= 0 THEN FAIL$ = "Yes"                                      !CRG


    ! Output appropriate message

    IF FAIL$ = "Yes" THEN BEGIN
        PRINT "BAD"
    ENDIF ELSE BEGIN
        PRINT "GOOD"
    ENDIF

PROGRAM.EXIT:

    STOP

ERROR.DETECTED:

   PRINT "ABEND"

END
