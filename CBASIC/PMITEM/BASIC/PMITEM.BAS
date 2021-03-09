
! PMITEM - QUICK AND DIRTY
! CALLS BOOTS FUNCTIONS BUT DOES NO PRODUCE AN OKAY, ERROR OR REPORT FILE


\*****************************************************************************
\*****************************************************************************
\***
\***    PROGRAM  .....  PMITEM
\***    MODULE  ......  PMITEM (Single module program)
\***
\***    REVISION 1.0.           ROBERT COWEY.                     01 NOV 2011.
\***    Original version based on QAD.BAS.
\***    Initially based on DD Rv 1.4.
\***    Modified INDICAT10% in line with DD Rv 1.5.
\***
\***    REVISION 1.1.           ROBERT COWEY.                     01 DEC 2011.
\***    Incorporated display of ISF (Item Shelf Edge Label file).
\***
\***    REVISION 1.2.           ROBERT COWEY.                     29 JAN 2012.
\***    Accepted item code via command string to output all item info as a
\***    continuous stream of data without need for user interaction.
\***    Completed code to access IRFDEX and display IRF deals 4-12.
\***    Modified input text to indicate full 7-digit item code needed.
\***
\***    REVISION 1.3.           ROBERT COWEY.                     03 MAY 2012.
\***    Displayed IRF records for items having the group code flag set ON.
\***
\***    REVISION 1.4.           ROBERT COWEY.                     05 AUG 2012.
\***    Corrected display of second IRF record for item having group code
\***    flag set ON to use IDF.SECOND.BAR.CODE$ instead of IDF.FIRST.BAR.CODE$.
\***
\***    REVISION 1.5.           ROBERT COWEY.                     13 SEP 2012.
\***    Treats 6-digit key as IDF item code in preparation for treating any
\***    other length as IRF bar code.
\***    Calculates item code check digit automatically.
\***
\***    REVISION 1.5.           ROBERT COWEY.                     13 SEP 2012.
\***    Move all file opens and closes into distinct subroutines.
\***    UNDER DEVELOPMENT
\***
\***    REVISION 1.6.           ATTILA SAGVARI.                   16 JUN 2014.
\***    Updated to print out all the Deal-Number and List-ID pairs on demand,
\***    otherwise First 14 Deal-Number and List-ID pairs will be printed out.
\***
\***    REVISION 1.7.           ATTILA SAGVARI.                   11 JUL 2014.
\***    Updated the code to fix defect 844. Changed the loop so the user will
\***    be offered the option at every IRF record to view all the deals.
\***
\*****************************************************************************
\*****************************************************************************


\*****************************************************************************
\*****************************************************************************
\***
\***    PMITEM
\***    ******
\***
\***    The Post Mortem Item program performs the following tasks ...
\***    Displays IDF data for a given item code
\***    Displays ISF data for the same item code
\***    Displays data for all IRF records on the IEF item-barcode chain
\***
\*****************************************************************************
\*****************************************************************************



\*****************************************************************************
\***
\***    DEC included code defining file related fields
\***
\***..........................................................................


    %INCLUDE   IDFDEC.J86   !   IDF file
    %INCLUDE   IEFDEC.J86   !   IEF file
    %INCLUDE   IRFDEC.J86   !   IRF file
    %INCLUDE   ISFDEC.J86   !   ISF file


\*****************************************************************************
\***
\***    Included code defining function related global variables
\***
\***..........................................................................


    %INCLUDE PSBF01G.J86   !   APPLICATION.LOG
    %INCLUDE PSBF11G.J86   !   READ.NEXT.IEF
    %INCLUDE PSBF18G.J86   !   CALC.BOOTS.CODE.CHECK.DIGIT
    %INCLUDE PSBF20G.J86   !   ALLOCATE.DEALLOCATE.SESS.NUM


\*****************************************************************************
\***
\***    Global varaible definitions
\***
\***..........................................................................


    STRING GLOBAL \
        BATCH.SCREEN.FLAG$, \
        CURRENT.CODE$, \
        FILE.OPERATION$, \
        MODULE.NUMBER$, \
        OPERATOR.NUMBER$


    INTEGER*2 GLOBAL \
        CURRENT.REPORT.NUM%, \
        RC2%


\*****************************************************************************
\***
\***    Varaible definitions
\***
\***..........................................................................


    STRING \
        A$, \
        CLOSE$, \
        COMMAND.STRING$, \
        CRLF$, \
        CURRENT.CODE.LOGGED$, \
        FILE.KEY.TEXT$, \
        IN.FILE.NAME$, \
        IN.REC.KEY$, \
        MODULE$, \
        OPEN$, \
        PASSED.STRING$, \
        PROGRAM$, \
        VAR.STRING.1$, \
        VAR.STRING.2$, \
        RECORD.INFO.TAB$,\                                        !1.6SA
        RECORD.CHECK.STRING$,\                                    !1.6SA
        BOOTS.CODE.CHECK$,\                                       !1.6SA
        IRF.NAME.PRINT$                                           !1.6SA


    INTEGER*1 \
        ERROR.COUNT%, \
        EVENT.NUMBER%, \
        FALSE, \
        SCREEN.MODE, \
        TRUE,\
        BOOTS.CODE.MISMATCH                                       !1.6SA

    INTEGER*2 \
        FAIL%, \
        FILE.RC%, \
        MESSAGE.NUMBER%, \
        OKAY%, \
        PASSED.INTEGER%, \
        SEC.RECL%,\
        DEAL.COUNTER%,\                                           !1.6SA
        RECORD.COUNTER%,\                                         !1.6SA
        INDEX%                                                    !1.6SA


    INTEGER*4 \
        BARCODE.COUNT%, \
        BARCODE.MAX%, \
        FILE.SIZE%, \
        FUNC.RC%,\
        IRF.NAME.LENGTH%,\                                        !1.6SA
        IRF.NAME.LENDIF%                                          !1.6SA



\*****************************************************************************
\***
\***    EXT included code defining file related external functions
\***
\***..........................................................................


    %INCLUDE   IDFEXT.J86   !   IDF file
    %INCLUDE   IEFEXT.J86   !   IDF file
    %INCLUDE   IRFEXT.J86   !   IRF file
    %INCLUDE   ISFEXT.J86   !   ISF file


\*****************************************************************************
\***
\***    Included code defining external Boots functions
\***
\***..........................................................................


    %INCLUDE ERRNH.J86     !   Converts ERRN to an 8 byte ASCII string
    %INCLUDE VIEWS.J86     !   Displays data in hex format
    %INCLUDE PSBF01E.J86   !   APPLICATION.LOG
    %INCLUDE PSBF11E.J86   !   READ.NEXT.IEF
    %INCLUDE PSBF18E.J86   !   CALC.BOOTS.CODE.CHECK.DIGIT
    %INCLUDE PSBF20E.J86   !   ALLOCATE.DEALLOCATE.SESS.NUM
    %INCLUDE PSBF24E.J86   !   STANDARD.ERROR.DETECTED


\*****************************************************************************
\***
\***    Included code defining external IBM functions / subroutines
\***
\***..........................................................................


!   None


\*****************************************************************************
\***
\***    Code defining internal functions / subroutines
\***
\***..........................................................................


\*****************************************************************************
\***
\***    READ.ISF.RAW
\***
\***..........................................................................

FUNCTION READ.ISF.RAW PUBLIC

    INTEGER*2 READ.ISF.RAW

    READ.ISF.RAW = 1

    IF END # ISF.SESS.NUM% THEN READ.ISF.RAW.ERROR

    READ FORM "T5,C45,I4,I2"; #ISF.SESS.NUM% \
         KEY ISF.BOOTS.CODE$;  \
             ISF.S.E.DESC$,   \
             ISF.INTEGER.4%, \
             ISF.INTEGER.2%

    READ.ISF.RAW = 0

READ.ISF.RAW.ERROR:

    EXIT FUNCTION

END FUNCTION


\*****************************************************************************
\***
\***    BIT$
\***    Returns "1" when BIT.VAL% is ON within HEX.VAL%
\***    Otherwise returns "0"
\***
\***..........................................................................

FUNCTION BIT$(HEX.VAL%, BIT.VAL%) PUBLIC

    STRING    BIT$
    INTEGER*4 HEX.VAL%    ! 1.2 RC
    INTEGER*4 BIT.VAL%    ! 1.2 RC

    IF (HEX.VAL% AND BIT.VAL%) = BIT.VAL% THEN BEGIN
        BIT$ = "1"
    ENDIF ELSE BEGIN
       BIT$ = "0"
    ENDIF

END FUNCTION


\*****************************************************************************
\***
\***    DN.LID$(ENTRY%)
\***    Formats deal number and list ID
\***
\***..........................................................................

FUNCTION DN.LID$(ENTRY%)

    STRING    DN.LID$
    STRING    WORK$
    INTEGER*2 ENTRY%

    DN.LID$ = UNPACK$(IRF.DEAL.NUM$(ENTRY%)) + "  " + STR$(IRF.LIST.ID%(ENTRY%))

END FUNCTION


\*****************************************************************************
\***
\***    EXPAND$(INT.IN.STR$)
\***    Extends integer data contained within string
\***
\***..........................................................................

FUNCTION EXPAND$(INT.IN.STR$)

    STRING    EXPAND$
    STRING    INT.IN.STR$
    STRING    WRK$

    WRK$ = UNPACK$("0" + INT.IN.STR$) ! Cater for UNPACK$("FF") giving "F"

    WRK$ = RIGHT$(WRK$, LEN(INT.IN.STR$) *2)

    EXPAND$ = TRANSLATE$(WRK$, ":;<=>?", "ABCDEF")

END FUNCTION


\*****************************************************************************
\***
\***    PAD.RT$
\***    Extends DATA$ parameter by appending spaces to give of bytes required
\***
\***..........................................................................

FUNCTION PAD.RT$(DATA$, BYTES%) PUBLIC

    STRING    PAD.RT$
    STRING    DATA$
    INTEGER*2 BYTES%

    PAD.RT$ = LEFT$(DATA$ + STRING$(BYTES%, " "), BYTES%)

END FUNCTION


\*****************************************************************************
\***
\***    STR.FROM.4.BYTE.INT$(FOUR.BYTE.INT$ function
\***    Converts four byte integer to a string.
\***    Based on PSBF17.
\***
\***..........................................................................

FUNCTION STR.FROM.4.BYTE.INT$(FOUR.BYTE.INT%)

    STRING    STR.FROM.4.BYTE.INT$
    STRING    WRK$

    INTEGER*2 BIT.COUNT%
    INTEGER*2 BYTE%
    INTEGER*4 FOUR.BYTE.INT%


    WRK$ = ""

    FOR BIT.COUNT% = 24 TO 0 STEP -8

        BYTE% = SHIFT(FOUR.BYTE.INT%, BIT.COUNT%)

        WRK$ = WRK$ + CHR$(BYTE%)

    NEXT BIT.COUNT%

    STR.FROM.4.BYTE.INT$ = WRK$

END FUNCTION


\*****************************************************************************
\***
\***    TIME.STAMP$ function
\***    Returns current time in HH:MM:SS format.
\***
\***..........................................................................

FUNCTION TIME.STAMP$ PUBLIC

    STRING TIME.STAMP$

    TIME.STAMP$ = \
      MID$(TIME$,1,2) + ":" + \
      MID$(TIME$,3,2) + ":" + \
      MID$(TIME$,5,2)

END FUNCTION



\*****************************************************************************
\***
\***    PROGRAM.CONTROL
\***
\***..........................................................................

PROGRAM.CONTROL:

    ON ERROR GOTO ERROR.DETECTED

    GOSUB INITIALISATION
    GOSUB MAIN.PROCESS
    GOSUB TERMINATION


STOP.PROGRAM:

    PRINT " "
    STOP



\*****************************************************************************
\***
\***    INITIALISATION
\***
\***..........................................................................

INITIALISATION:

    GOSUB INITIALISE.VARIABLES
    GOSUB INITIALISE.FILE.VARIABLES
    GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB OPEN.ALL.FILES

RETURN


\*****************************************************************************
\***
\***    MAIN.PROCESS
\***
\***..........................................................................

MAIN.PROCESS:

    PRINT " "
    PRINT "PMITEM - Item post mortem utility version 1.7 "
    PRINT " "

    IF COMMAND.STRING$ <> "" THEN BEGIN
        PRINT "Command string: ";COMMAND.STRING$
        IN.REC.KEY$ = RIGHT$(COMMAND.STRING$, 7)
    ENDIF ELSE BEGIN
        SCREEN.MODE = TRUE
        GOSUB OBTAIN.FILE.KEY
    ENDIF

    IF LEN(IN.REC.KEY$) = 6 THEN BEGIN
        CALL CALC.BOOTS.CODE.CHECK.DIGIT(IN.REC.KEY$)
        IDF.BOOTS.CODE$ = PACK$("0" + IN.REC.KEY$ + F18.CHECK.DIGIT$)
    ENDIF ELSE BEGIN
        PRINT "Key length is ";STR$(LEN(IN.REC.KEY$));" digits"
        STOP
!       IRF.BAR.CODE$ = PACK$(RIGHT$(STRING$(22, "0") + IN.REC.KEY$, 22))
!       PRINT "IRF bar code: ";UNPACK$(MID$(IRF.BAR.CODE$, 1, 5));" "; + \
!                              UNPACK$(MID$(IRF.BAR.CODE$, 6, 3));" "; + \
!                              UNPACK$(MID$(IRF.BAR.CODE$, 9, 3))
!       WAIT ; 750 ! Allow barcode to be recognised
    ENDIF

    PRINT " "

    GOSUB DISPLAY.ITEM.DATA

    IF SCREEN.MODE THEN BEGIN
        INPUT "Enter to finish  or (D) to display all "+ \
        "Deal-Number/List-ID Pairs:"; A$
        LOCATE 24,1 ! Row Col
    ENDIF

     IF SCREEN.MODE = FALSE THEN BEGIN
        GOSUB PRINT.ALL.RECORDS
    ENDIF


        IF SCREEN.MODE = TRUE AND A$ = "D" OR A$ = "d" THEN BEGIN
            PRINT.ALL.RECORDS:
            PRINT "IRF and IRFDEX Deal-Number and List-ID pairs"

            !1st line
            PRINT CRLF$ +  DN.LID$(0); "  "; "      "; "  ";     \!1.6SA
            DN.LID$(5);"  "; DN.LID$(10);"  "; DN.LID$(15);"  "; \!1.6SA
            DN.LID$(20);"  "; DN.LID$(25);"  "; DN.LID$(30);"  ";\!1.6SA
            DN.LID$(35);"  ";                                     !1.6SA

            !2nd line
            PRINT CRLF$ + DN.LID$(1); "  "; "      "; "  ";      \!1.6SA
            DN.LID$(6);"  "; DN.LID$(11);"  "; DN.LID$(16);"  "; \!1.6SA
            DN.LID$(21);"  "; DN.LID$(26);"  "; DN.LID$(31);"  ";\!1.6SA
            DN.LID$(36);"  ";                                     !1.6SA

            !3rd line
            PRINT CRLF$ + DN.LID$(2); "  "; "      "; "  ";      \!1.6SA
            DN.LID$(7);"  "; DN.LID$(12);"  "; DN.LID$(17);"  "; \!1.6SA
            DN.LID$(22);"  "; DN.LID$(27);"  "; DN.LID$(32);"  ";\!1.6SA
            DN.LID$(37);"  ";                                     !1.6SA

            !4th line
            PRINT  CRLF$ + "      "; "  "; DN.LID$(3);  "  ";    \!1.6SA
            DN.LID$(8);"  "; DN.LID$(13);"  "; DN.LID$(18);"  "; \!1.6SA
            DN.LID$(23);"  "; DN.LID$(28);"  "; DN.LID$(33);"  ";\!1.6SA
            DN.LID$(38);"  ";                                     !1.6SA


            !5th line
            PRINT CRLF$ + "      "; "  "; DN.LID$(4);  "  ";     \!1.6SA
            DN.LID$(9);"  "; DN.LID$(14);"  "; DN.LID$(19);"  "; \!1.6SA
            DN.LID$(24);"  "; DN.LID$(29);"  "; DN.LID$(34);"  ";\!1.6SA
            DN.LID$(39);"  ";                                     !1.6SA

            PRINT " "

            IF SCREEN.MODE = FALSE THEN BEGIN
                RETURN
            ENDIF

            INPUT "Press enter to finish viewing";A$

        ENDIF


    PRINT "                              " + \  Overwrites "Enter for next rec" text
          "                              "   !  or spaces batch output

    PRINT "Finished"

RETURN


\*****************************************************************************
\***
\***    TERMINATION
\***
\***..........................................................................

TERMINATION:

    GOSUB CLOSE.ALL.FILES
    GOSUB DEALLOCATE.SESSION.NUMBERS

RETURN



\*****************************************************************************
\***
\***    INITIALISATION SPECIFIC ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    INITIALISE.VARIABLES
\***    Sets program variables.
\***
\***..........................................................................

INITIALISE.VARIABLES:

    COMMAND.STRING$ = COMMAND$

!   Program specific variable assignments

    PROGRAM$           = "PMITEM"
    MODULE$            = ""
    MODULE.NUMBER$     =  PROGRAM$ + MODULE$
    BATCH.SCREEN.FLAG$ = "B" ! Batch

    CLOSE$ = "C"
    OPEN$  = "O"
    OKAY% = 0
    FAIL% = 1

    CRLF$     = CHR$(13) + CHR$(10) ! X'0D0A'
    SEC.RECL% = 512

    FALSE = 0
    TRUE = -1
    BOOTS.CODE.CHECK$    = " "                                    !1.6SA
    BOOTS.CODE.MISMATCH  = FALSE                                  !1.6SA
    DEAL.COUNTER% = 0                                             !1.6SA
    RECORD.INFO.TAB$ = ""                                         !1.6SA
    RECORD.COUNTER% = 1                                           !1.6SA
    INDEX% = 1                                                    !1.6SA


RETURN


\*****************************************************************************
\***
\***    INITIALISE.FILE.VARIABLES
\***    Sets file related variables.
\***
\***..........................................................................

INITIALISE.FILE.VARIABLES:

!   Calls file SET functions initialising file related fields

    CALL IDF.SET
    CALL IEF.SET
    CALL IRF.SET
    CALL ISF.SET

RETURN


\*****************************************************************************
\***
\***    ALLOCATE.SESSION.NUMBERS
\***    Calls F20 SESS.NUM.UTILITY to allocate file session numbers
\***    for all files referenced by the program.
\***
\***..........................................................................

ALLOCATE.SESSION.NUMBERS:

    IF SESS.NUM.UTILITY (OPEN$, IDF.REPORT.NUM%, IDF.FILE.NAME$) \
        <> OKAY% THEN GOSUB STOP.PROGRAM
     IDF.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF SESS.NUM.UTILITY (OPEN$, IEF.REPORT.NUM%, IEF.FILE.NAME$) \
        <> OKAY% THEN GOSUB STOP.PROGRAM
     IEF.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF SESS.NUM.UTILITY (OPEN$, IRF.REPORT.NUM%, IRF.FILE.NAME$) \
        <> OKAY% THEN GOSUB STOP.PROGRAM
     IRF.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF SESS.NUM.UTILITY (OPEN$, IRFDEX.REPORT.NUM%, IRFDEX.FILE.NAME$) \
        <> OKAY% THEN GOSUB STOP.PROGRAM
     IRFDEX.SESS.NUM% = F20.INTEGER.FILE.NO%

    IF SESS.NUM.UTILITY (OPEN$, ISF.REPORT.NUM%, ISF.FILE.NAME$) \
        <> OKAY% THEN GOSUB STOP.PROGRAM
     ISF.SESS.NUM% = F20.INTEGER.FILE.NO%

RETURN


\*****************************************************************************
\***
\***    OPEN.ALL.FILES:
\***    Opens all files potentially required by program.
\***
\***..........................................................................

OPEN.ALL.FILES:

    FILE.OPERATION$     = "O"

    CURRENT.REPORT.NUM% =  IDF.REPORT.NUM%

    IF END # IDF.SESS.NUM% THEN FILE.ERROR.EXIT

    OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% \
      AS IDF.SESS.NUM% NODEL

    CURRENT.REPORT.NUM% =  ISF.REPORT.NUM%

    IF END # ISF.SESS.NUM% THEN FILE.ERROR.EXIT

    OPEN ISF.FILE.NAME$ KEYED RECL ISF.RECL% \
      AS ISF.SESS.NUM% NODEL

    CURRENT.REPORT.NUM% = IEF.REPORT.NUM%

    IF END # IEF.SESS.NUM% THEN FILE.ERROR.EXIT

    OPEN IEF.FILE.NAME$ KEYED RECL IEF.RECL% \
      AS IEF.SESS.NUM% NOWRITE NODEL

    CURRENT.REPORT.NUM% =  IRF.REPORT.NUM%

    IF END # IRF.SESS.NUM% THEN FILE.ERROR.EXIT

    OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% \
      AS IRF.SESS.NUM% NODEL

    CURRENT.REPORT.NUM% =  IRFDEX.REPORT.NUM%

    IF END # IRFDEX.SESS.NUM% THEN FILE.ERROR.EXIT

    OPEN IRFDEX.FILE.NAME$ KEYED RECL IRFDEX.RECL% \
      AS IRFDEX.SESS.NUM% NODEL

RETURN


\*****************************************************************************
\***
\***    CLOSE.ALL.FILES:
\***    Closes all files used by program.
\***
\***..........................................................................

CLOSE.ALL.FILES:

    CLOSE IDF.SESS.NUM%
    CLOSE ISF.SESS.NUM%
    CLOSE IEF.SESS.NUM%
    CLOSE IRF.SESS.NUM%
    CLOSE IRFDEX.SESS.NUM%

RETURN



\*****************************************************************************
\***
\***    TERMINATION SPECIFIC ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    DEALLOCATE.SESSION.NUMBERS
\***    Perform CALL.F20.SESS.NUM.UTILITY to de-allocate file session numbers
\***    from all files referenced by the program.
\***
\***..........................................................................

DEALLOCATE.SESSION.NUMBERS:

    CALL SESS.NUM.UTILITY (CLOSE$, IDF.SESS.NUM%, "")
    CALL SESS.NUM.UTILITY (CLOSE$, IEF.SESS.NUM%, "")
    CALL SESS.NUM.UTILITY (CLOSE$, IRF.SESS.NUM%, "")
    CALL SESS.NUM.UTILITY (CLOSE$, IRFDEX.SESS.NUM%, "")
    CALL SESS.NUM.UTILITY (CLOSE$, ISF.SESS.NUM%, "")

RETURN



\*****************************************************************************
\***
\***    MAIN.PROCESS SPECIFIC ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    OBTAIN.FILE.KEY:
\***
\***
\***..........................................................................

OBTAIN.FILE.KEY:

    INPUT "Supply 6-digit IDF key"; IN.REC.KEY$
    RETURN

!   The following code allows successive inputs onto a particular line
!   File IDF   Key 0000000000 000000 000000

!   Note
!   INPUT moves cursor to next line
!     if this is line 26 then screen scrolls
!     if this is line 2 then cursor can be repositioned on line 1 with no scrolling

!   Note PRINT issues a CRLF unless print data followed by a semicolon

    PRINT " " ! No semicolon so CRLF issued and blank line becomes line 24
    LOCATE 24,1
    INPUT "Enter file name ..."; IN.FILE.NAME$

    FILE.KEY.TEXT$ = "File " + IN.FILE.NAME$
    LOCATE 24,1
    PRINT FILE.KEY.TEXT$;

    LOCATE 24,LEN(FILE.KEY.TEXT$) +1
    INPUT "   Enter rec key .."; IN.REC.KEY$

    FILE.KEY.TEXT$ = FILE.KEY.TEXT$ + "   Key " + IN.REC.KEY$ ! => Validate and format
    LOCATE 24,1           ! Column 79 because column 80 causes screen to scroll up
    PRINT FILE.KEY.TEXT$; ! Semicolon prevents PRINT statement sending CRLF$ to screen and scrolling it up

    WAIT ; 1000

RETURN


\*****************************************************************************
\***
\***    DISPLAY.ITEM.DATA:
\***
\***
\***..........................................................................

DISPLAY.ITEM.DATA:

    FILE.RC% = READ.IDF

    IF FILE.RC% <> 0 THEN BEGIN
        PRINT "Item not on IDF"
        RETURN ! Significant problem so prevent further procesing
    ENDIF

    PRINT "IDF ";MID$(UNPACK$(IDF.BOOTS.CODE$), 2, 2);"-"; + \
                 MID$(UNPACK$(IDF.BOOTS.CODE$), 4, 2);"-"; + \
                 MID$(UNPACK$(IDF.BOOTS.CODE$), 6, 3)
    GOSUB DISPLAY.IDF.DATA

    GOSUB OBTAIN.ISF.REC ! Sets FILE.RC%

    IF FILE.RC% <> 0 THEN BEGIN
        PRINT "Item not on ISF"
        PRINT "No Shelf Edge Label data available"
!       RETURN not executed to allow IRF record display
    ENDIF ELSE BEGIN
        PRINT "ISF record ..."
        GOSUB DISPLAY.ISF.DATA
    ENDIF

    BARCODE.MAX% = VAL(UNPACK$(IDF.NO.OF.BAR.CODES$))

!   Process IRF record 1

    BARCODE.COUNT% = BARCODE.COUNT% + 1

    IF SCREEN.MODE THEN BEGIN
        INPUT "Enter for IRF record " + \
                 STR$(BARCODE.COUNT%) + " of " + STR$(BARCODE.MAX%); A$
        LOCATE 24,1 ! Row Col
    ENDIF

    PRINT "                                    " ! Overwrites "Enter for next rec" text
                                                 ! or spaces batch outpit

    IRF.BAR.CODE$ = STRING$(5, PACK$("00")) + IDF.FIRST.BAR.CODE$

    IF READ.IRF <> 0 THEN BEGIN
        PRINT "IDF Barcode "; STR$(BARCODE.COUNT%); " not on IRF (";UNPACK$(IDF.FIRST.BAR.CODE$);")"
    ENDIF ELSE BEGIN
        PRINT "IRF record "; STR$(BARCODE.COUNT%); " ..."
        GOSUB DISPLAY.IRF.DATA
    ENDIF

!   Process IRF record 1 group code (if any)
!   Entire IF test new for Rv 1.3
    IF (IDF.BIT.FLAGS.1% AND 80h) = 80h THEN BEGIN ! Group code ON

        IF SCREEN.MODE THEN BEGIN
            INPUT "Enter for IRF record " + \
                     STR$(BARCODE.COUNT%) + " of " + STR$(BARCODE.MAX%) + " (group code prefix)"; A$
            LOCATE 24,1 ! Row Col
        ENDIF

        PRINT "                              " + \  Overwrites "Enter for next rec" text
              "                              "   !  or spaces batch output

        IRF.BAR.CODE$ = PACK$("2000000000") + IDF.FIRST.BAR.CODE$

        IF READ.IRF <> 0 THEN BEGIN
            PRINT "IDF Barcode "; STR$(BARCODE.COUNT%); " not on IRF (";UNPACK$(IDF.FIRST.BAR.CODE$);")"; " (group code)"
        ENDIF ELSE BEGIN
            PRINT "IRF record "; STR$(BARCODE.COUNT%); " (group code prefix) ..."
            GOSUB DISPLAY.IRF.DATA
        ENDIF

    ENDIF

!   Process IRF record 2

    BARCODE.COUNT% = BARCODE.COUNT% + 1

    IF BARCODE.COUNT% > BARCODE.MAX% THEN BEGIN
        RETURN
    ENDIF

    IF SCREEN.MODE = FALSE THEN BEGIN
        GOSUB PRINT.ALL.RECORDS
    ENDIF

    IF SCREEN.MODE THEN BEGIN
        INPUT "Enter for IRF record "+ STR$(BARCODE.COUNT%) + \
        " of " + STR$(BARCODE.MAX%) + " or (D) to display all " +\
        "Deal-Number/List-ID Pairs:";A$

        IF SCREEN.MODE = TRUE AND A$ = "D" OR A$ = "d" THEN BEGIN !1.6SA
            GOSUB PRINT.ALL.RECORDS                               !1.7SA
            IF SCREEN.MODE = FALSE THEN BEGIN
                RETURN
            ENDIF


        ENDIF



     ENDIF


    PRINT "                              " + \  Overwrites "Enter for next rec" text
          "                              "   !  or spaces batch output

    IRF.BAR.CODE$ = STRING$(5, PACK$("00")) + IDF.SECOND.BAR.CODE$

    IF READ.IRF <> 0  THEN BEGIN
        PRINT "IDF Barcode "; STR$(BARCODE.COUNT%); " not on IRF (";UNPACK$(IDF.SECOND.BAR.CODE$);")"
    ENDIF ELSE BEGIN
        PRINT "IRF record "; STR$(BARCODE.COUNT%); " ..."
        GOSUB DISPLAY.IRF.DATA
    ENDIF

!   Process IRF record 2 group code (if any)
!   Entire IF test new for Rv 1.3
    IF (IDF.BIT.FLAGS.1% AND 80h) = 80h THEN BEGIN ! Group code ON

        IF SCREEN.MODE THEN BEGIN
            INPUT "Enter for IRF record " + \
                     STR$(BARCODE.COUNT%) + " of " + STR$(BARCODE.MAX%) + " (group code prefix)"; A$
            LOCATE 24,1 ! Row Col
        ENDIF

        PRINT "                              " + \  Overwrites "Enter for next rec" text
              "                              "   !  or spaces batch output

        IRF.BAR.CODE$ = PACK$("2000000000") + IDF.SECOND.BAR.CODE$    ! 1.4 RC

        IF READ.IRF <> 0 THEN BEGIN
            PRINT "IDF Barcode "; STR$(BARCODE.COUNT%); " not on IRF (";UNPACK$(IDF.SECOND.BAR.CODE$);")"; " (group code)"
        ENDIF ELSE BEGIN
            PRINT "IRF record "; STR$(BARCODE.COUNT%); " (group code prefix) ..."
            GOSUB DISPLAY.IRF.DATA
        ENDIF



    ENDIF

    BARCODE.COUNT% = BARCODE.COUNT% + 1

    IF BARCODE.COUNT% > BARCODE.MAX% THEN BEGIN
        RETURN ! No further barcodes to process
    ENDIF

    WHILE BARCODE.COUNT% <= BARCODE.MAX%

        IF COMMAND.STRING$ = ""  THEN BEGIN                       !1.7SA
            INPUT "Enter for IRF record "+ STR$(BARCODE.COUNT%) + \1.7SA
            " of " + STR$(BARCODE.MAX%) + " or (D) to display all " +\1.7SA
            "Deal-Number/List-ID Pairs:";A$                       !1.7SA
            IF SCREEN.MODE = TRUE AND A$ = "D" OR A$ = "d" THEN BEGIN!1.6SA
                GOSUB PRINT.ALL.RECORDS                           !1.7SA
            ENDIF ELSE BEGIN                                      !1.7SA
            ENDIF                                                 !1.7SA
        ENDIF                                                     !1.7SA

        IF COMMAND.STRING$ <> ""  THEN BEGIN                      !1.7SA
            GOSUB PRINT.ALL.RECORDS                               !1.7SA
        ENDIF                                                     !1.7SA

        PRINT "                                    " ! Overwrites "Enter for next rec" text
                                                     ! or spaces batch output

        FUNC.RC% = READ.NEXT.IEF (RIGHT$(IRF.BAR.CODE$, 6), \
                                  BARCODE.MAX% - 1, \
                                  RIGHT$(UNPACK$(IDF.BOOTS.CODE$), 7))

        IF FUNC.RC% <> 0 THEN BEGIN
            PRINT "Further barcodes not readable from IEF"
            RETURN
        ENDIF

        IRF.BAR.CODE$ = PACK$("0000000000") + F11.NEXT.BAR.CODE$ ! Non-group code
!                       PACK$("2000000000") ! Defines group codes

        IF READ.IRF <> 0 THEN BEGIN
            PRINT "IDF Barcode "; STR$(BARCODE.COUNT%); " not on IRF (";UNPACK$(IDF.SECOND.BAR.CODE$);")"
        ENDIF ELSE BEGIN
            PRINT "IRF record "; STR$(BARCODE.COUNT%); " ..."
            GOSUB DISPLAY.IRF.DATA
        ENDIF

        BARCODE.COUNT% = BARCODE.COUNT% + 1

    WEND

RETURN


\*****************************************************************************
\***
\***    DISPLAY.IDF.DATA:
\***
\***..........................................................................

! IDF record ...
! BOOTS.CODE$       01234560            BIT.FLAGS.1%  F0h   BIT.FLAGS.2%  0Fh
! FIRST.BAR.CODE$   000000123456         80 Group code 1     80 Own brand  0
! SECOND.BAR.CODE$  501653300420         40 Keylines   1x    40 Exclusive  0x
! NO.OF.BAR.CODES$  0002                 20 Markdown   1     20 Unused     0
!                                        10 Warehouse  1     10 Unused     0
! PRODUCT.GRP$  010190                   08 CSR item   0x    08 Stock item 1
! STNDRD.DESC$  SUP SOLUBL MAXIJUL 200G  04 Directs A  0     04 Pend count 1
! STATUS.1$     B                        02 Directs B  0x    02 Reserved   1
! INTRO.DATE$   930805                   01 Directs C  0x    01 Reserved   1
! BSNS.CNTR$    W
! FILLER$                               PARENT.CODE$        00000000x
!                                       DATE.OF.LAST.SALE$  070718

DISPLAY.IDF.DATA:

    PRINT "BOOTS.CODE$       "; PAD.RT$(UNPACK$(IDF.BOOTS.CODE$), 20); \
           "BIT.FLAGS.1%  "; HEX.VIEW1$(CHR$(IDF.BIT.FLAGS.1%)); "h   "; \
           "BIT.FLAGS.2%  " ; HEX.VIEW1$(CHR$(IDF.BIT.FLAGS.2%)); "h   "

    PRINT "FIRST.BAR.CODE$   "; PAD.RT$(UNPACK$(IDF.FIRST.BAR.CODE$), 20); \
           " 80 Group code "; BIT$(IDF.BIT.FLAGS.1%,80h); "    "; \
           " 80 Own brand   "; BIT$(IDF.BIT.FLAGS.2%,80h)

    PRINT "SECOND.BAR.CODE$  "; PAD.RT$(UNPACK$(IDF.SECOND.BAR.CODE$), 20); \
           " 40 Keylines   "; BIT$(IDF.BIT.FLAGS.1%,40h); "x   "; \
           " 40 Exclsv Line "; BIT$(IDF.BIT.FLAGS.2%,40h); "x"

    PRINT "NO.OF.BAR.CODES$  "; PAD.RT$(UNPACK$(IDF.NO.OF.BAR.CODES$), 20); \
           " 20 Markdown   "; BIT$(IDF.BIT.FLAGS.1%,20h); "    "; \
           " 20 Unused      "; BIT$(IDF.BIT.FLAGS.2%,20h)

    PRINT "                                      "; \
           " 10 Warehouse  "; BIT$(IDF.BIT.FLAGS.1%,10h); "    "; \
           " 10 Unused      "; BIT$(IDF.BIT.FLAGS.2%,10h)

    PRINT "PRODUCT.GRP$  "; PAD.RT$(UNPACK$(IDF.PRODUCT.GRP$), 24); \
           " 08 CSR item   "; BIT$(IDF.BIT.FLAGS.1%,08h); "x   "; \
           " 08 Stock item  "; BIT$(IDF.BIT.FLAGS.2%,08h)

    PRINT "STNDRD.DESC$  "; PAD.RT$(IDF.STNDRD.DESC$, 24); \
           " 04 Directs A  "; BIT$(IDF.BIT.FLAGS.1%,04h); "    "; \
           " 04 Pend count  "; BIT$(IDF.BIT.FLAGS.2%,04h)

    PRINT "STATUS.1$     "; PAD.RT$(IDF.STATUS.1$, 24); \
           " 02 Directs B  "; BIT$(IDF.BIT.FLAGS.1%,02h); "x   "; \
           " 02 Reserved    "; BIT$(IDF.BIT.FLAGS.2%,02h)

    PRINT "INTRO.DATE$   "; PAD.RT$(UNPACK$(IDF.INTRO.DATE$), 24); \
           " 01 Directs C  "; BIT$(IDF.BIT.FLAGS.1%,01h); "x   "; \
           " 01 Reserved    "; BIT$(IDF.BIT.FLAGS.2%,01h)

    PRINT "BSNS.CNTR$    "; PAD.RT$(IDF.BSNS.CNTR$, 24)

    PRINT "FILLER$       "; PAD.RT$(IDF.FILLER$, 24); \
           "PARENT.CODE$        "; UNPACK$(IDF.PARENT.CODE$); "x"

    PRINT "                                      "; \
           "DATE.OF.LAST.SALE$  "; UNPACK$(IDF.DATE.OF.LAST.SALE$)

RETURN


\*****************************************************************************
\***
\***    OBTAIN.ISF.REC:
\***
\***..........................................................................

OBTAIN.ISF.REC:

    ISF.BOOTS.CODE$ = IDF.BOOTS.CODE$

    FILE.RC% = READ.ISF

    IF FILE.RC% = 0 THEN BEGIN
!       Repeat read the file to obtain raw integer data
        FILE.RC% = READ.ISF.RAW
    ENDIF

RETURN


\*****************************************************************************
\***
\***    DISPLAY.ISF.DATA:
\***
\***..........................................................................

! WORK

! SEL-DSEC-1$  ABCDEFGHIJKLMNO    UNIT.NAME$          ABCDEFGHIJ
! SEL-DESC-2$  ABCDEFGHIJKLMNO    ITEM.QTY$           12345678
! SEL-DESC-3$  ABCDEFGHIJKLMNO    SEL.PRINTED.FLAG$   Y
! INTEGER*4      12346578h        UNIT.MEASUREMENT$   1234
! INTEGER*2          1234h

!                                 INTEGER*4  12346578h
!                                  000000FFh  UNIT.NAME.COUNTER%  123d
!                                  UNIT.NAME$  ABCDEFGHIJ from IUDF
!                                  FFFFFF00h  ITEM.QTY$   12345678

!                                              INTEGER*2  1234h
!                                               8000h SEL.PRINTED.FLAG$  Y
!                                               7FFFh UNIT.MEASUREMENT$  1234d

! SEL-DSEC-1$  ABCDEFGHIJKLMNO
! SEL-DESC-2$  ABCDEFGHIJKLMNO
! SEL-DESC-3$  ABCDEFGHIJKLMNO
!
! INTEGER*4  12346578h                           INTEGER*2  1234h
!  000000FFh  UNIT.NAME.COUNTER%  123d            8000h SEL.PRINTED.FLAG$  Y
!             UNIT.NAME$  ABCDEFGHIJ from IUDF    7FFFh UNIT.MEASUREMENT$  1234d
!  FFFFFF00h  ITEM.QTY$   12345678


! ISF record ...
! SEL-DSEC-1$  ABCDEFGHIJKLMNO    INTEGER*4  12346578h
! SEL-DESC-2$  ABCDEFGHIJKLMNO     000000FF UNIT.NAME.COUNTER%  123d
! SEL-DESC-3$  ABCDEFGHIJKLMNO              UNIT.NAME$  ABCDEFGHIJ (from IUDF)
!                                  FFFFFF00 ITEM.QTY$   12345678
!
!                                 INTEGER*2  1234h
!                                      8000 SEL.PRINTED.FLAG$  0 =N
!                                      7FFF UNIT.MEASUREMENT$  1234d

DISPLAY.ISF.DATA:

! PRINT "This is the ISF data display"

    PRINT "SEL-DESC-1$  "; MID$(ISF.S.E.DESC$,  1, 15); "    "; \
          "INTEGER*4  "; EXPAND$(STR.FROM.4.BYTE.INT$(ISF.INTEGER.4%))

    PRINT "SEL-DESC-2$  "; MID$(ISF.S.E.DESC$, 16, 15); "    "; \
          " 000000FF UNIT.NAME.COUNTER% "; RIGHT$("    " + STR$(ISF.INTEGER.4% AND 000000FFh), 4); "d"

    PRINT "SEL-DESC-3$  "; MID$(ISF.S.E.DESC$, 31, 15); "    "; \
          "          UNIT.NAME$  "; ISF.UNIT.NAME$; " (from IUDF)"

    PRINT "                                "; \
          " FFFFFF00 ITEM.QTY$   "; UNPACK$(ISF.ITEM.QTY$); "d"

    PRINT " "

    PRINT "                                "; \
          "INTEGER*2  "; EXPAND$(RIGHT$(STR.FROM.4.BYTE.INT$(ISF.INTEGER.2%),2))

    PRINT "                                "; \
          "     8000 SEL.PRINTED.FLAG$  "; BIT$(ISF.INTEGER.2%,8000h); " ="; ISF.SEL.PRINTED.FLAG$

    PRINT "                                "; \
          "     7FFF UNIT.MEASUREMENT$  "; UNPACK$(ISF.UNIT.MEASUREMENT$); "d  (ISFFUN bug)"

RETURN


\*****************************************************************************
\***
\***    DISPLAY.IRF.DATA:
\***
\***    Changed the layout to fit 14 records
\***..........................................................................

!IRF record ...
!BAR.CODE$   0000000000000000000000   First 14 Deal-Number and List-ID pairs
!SALEPRIC$   0000000000                0000  0  0000  0  0000  0  0000  0  0000  0
!ITEMNAME$   AAAAAAAAAAAAAAAAAA        0000  0  0000  0  0000  0  0000  0  0000  0
!BOOTS.CODE$ 00000000                  0000  0  0000  0  0000  0  0000  0  0000  0
!
! INDICAT0%  OOh             INDICAT1%  00h              INDICAT8%  00h
! 80 Item movement kept  1   80 Cntn Ibuprofen       1   80 WEEE item         1
! 40 Enforce qty entry   1   40 Insurance policy     1   40 Withdrawn Recall  1
! 20 Enforce price entry 1   20 Non-solid painkiller 1*  20 100% Returns      1
! 10 Recall (block sale) 1*  10 Withdrawn sale block 1*  10 Cntn Ephedrine    1
! 08 Cntn alcohol        1   08 Giftcard item        1*  08 Nightingale prmpt 1
! 04 Not returnable      1   04 TPLU inclusion       1   04 Unrestricted grp  1
! 02 Not priced giveaway 1   02 Cntn Paracetamol     1   02 Chlamydia open sl 1
! 01 Cntn Statins        1   01 Cntn Aspirin         1   01 Chlamydia NHS kit 1
!
!INDICAT9%  00h             INDICAT5%  00h              INDICAT3%  00h
! 80 Boots.com ext       1   80 Guarantee 1=Mth 0=Yr 1   80 CSR item          1x
! 40 Resaleable indct    1   40 Cntn Pseudoephedrine 1   40 Stock item        1
! 3F Disposal Spc-Ins   00d  3F Guarantee duration  00d  20 Locally priced    1
!                                                        10 Wellbeing service 1x
!INDICAT10%  00h                                         08 Excl loyalty      1
! C0 Returns route      00h                              04 Redeemable        1
! 38 Ethical class      00h                              02 Boots/brand       1
! 07 Age restriction    00h                              01 Discount exempt   1

DISPLAY.IRF.DATA:

    !To check if there is any data after the 14th record          !1.6SA
    RECORD.CHECK.STRING$ = LEFT$(DN.LID$(14),4) +                \!1.6SA
                           LEFT$(DN.LID$(15),5)                   !1.6SA

    IF RECORD.CHECK.STRING$ = "00000000" THEN BEGIN               !1.6SA

        RECORD.COUNTER% = 0                                       !1.7SA
        FOR INDEX% = 1  TO 13 STEP 1                              !1.7SA
            RECORD.INFO.TAB$ = DN.LID$(RECORD.COUNTER%)           !1.6SA
            IF DN.LID$(RECORD.COUNTER%) <> "0000  0" THEN BEGIN   !1.6SA
                DEAL.COUNTER% = DEAL.COUNTER% + 1                 !1.6SA
            ENDIF                                                 !1.6SA
            RECORD.COUNTER% = RECORD.COUNTER% + 1                 !1.6SA
        NEXT INDEX%                                               !1.6SA

        IF DEAL.COUNTER% <> 0 THEN BEGIN                          !1.6SA
            RECORD.INFO.TAB$ = "Only "+  STR$(DEAL.COUNTER%)      !1.6SA
            !To work out the number of record displayed           !1.6SA
        ENDIF ELSE BEGIN                                          !1.6SA
            RECORD.INFO.TAB$ = "NoDeals"                          !1.6SA
        ENDIF                                                     !1.6SA

        !To reset the variable                                    !1.6SA
        DEAL.COUNTER% = 0                                         !1.7SA

    ENDIF ELSE BEGIN                                              !1.6SA
        RECORD.INFO.TAB$ = "More..."                              !1.6SA
    ENDIF

    PRINT "BAR.CODE$   "; PAD.RT$(UNPACK$(IRF.BAR.CODE$),25); \   !1.6SA
           "First 14 Deal-Number and List-ID pairs"               !1.6SA

    !Changed layout to fit 14 records SA                          !1.6SA
    PRINT "SALEPRIC$   "; PAD.RT$(UNPACK$(IRF.SALEPRIC$),25); \   !1.6SA
           ""; DN.LID$(0); "  "; DN.LID$(3); "  "; DN.LID$(6);\   !1.6SA
            "  "; DN.LID$(9);"  "; DN.LID$(12)                    !1.6SA

    IRF.NAME.PRINT$  = LEFT$(IRF.ITEMNAME$,18)                    !1.6SA
    !To make sure that the itemname is always the same length      1.6SA
    IRF.NAME.PRINT$  = LEFT$(IRF.NAME.PRINT$+ STRING$(25," "),25) !1.6SA

    IF IRF.NAME.LENGTH% >  25 THEN BEGIN                          !1.6SA
        IRF.NAME.PRINT$  = LEFT$(IRF.NAME.PRINT$,27)              !1.6SA                                            !1.6SA
    ENDIF                                                         !1.6SA


    PRINT "ITEMNAME$   "; IRF.NAME.PRINT$;           \            !1.6SA
           ""; DN.LID$(1); "  "; DN.LID$(4); "  "; DN.LID$(7); \  !1.6SA
           "  "; DN.LID$(10);"  "; DN.LID$(13)                    !1.6SA

    !To work out if there is a difference between the two codes    1.6SA
    IF BOOTS.CODE.CHECK$ <> " "  THEN BEGIN                       !1.6SA
        IF BOOTS.CODE.CHECK$ =                             \      !1.6SA
            PAD.RT$(UNPACK$(IRF.BOOTS.CODE$),25)  THEN BEGIN      !1.6SA
            BOOTS.CODE.MISMATCH = FALSE                           !1.6SA
        ENDIF ELSE BEGIN                                          !1.6SA
            BOOTS.CODE.MISMATCH = TRUE                            !1.6SA
        ENDIF                                                     !1.6SA
    ENDIF                                                         !1.6SA

    IF BOOTS.CODE.MISMATCH = FALSE THEN BEGIN                     !1.6SA
        PRINT"BOOTS.CODE$ ";PAD.RT$(UNPACK$(IRF.BOOTS.CODE$),25);\!1.6SA
              ""; DN.LID$(2); "  "; DN.LID$(5); "  "; DN.LID$(8);\!1.6SA
               "  "; DN.LID$(11);"  ";RECORD.INFO.TAB$            !1.6SA
    ENDIF ELSE BEGIN                                              !1.6SA
       PRINT "BOOTS.CODE$ ";LEFT$(UNPACK$(IRF.BOOTS.CODE$),6); \  !1.6SA
           "x Expctd ";LEFT$(BOOTS.CODE.CHECK$,6);"    "; \       !1.6SA
           DN.LID$(2);"  ";DN.LID$(5); "  ";DN.LID$(8);"  ";  \   !1.6SA
           DN.LID$(11);"  ";RECORD.INFO.TAB$                      !1.6SA
    ENDIF                                                         !1.6SA

    !To populate the variable to check if the boots code is the same
    !between the two records SA
    BOOTS.CODE.CHECK$ = PAD.RT$(UNPACK$(IRF.BOOTS.CODE$),25)      !1.6SA



    PRINT " "

    PRINT "INDICAT0%  "; HEX.VIEW1$(CHR$(IRF.INDICAT0%)); "h"; "             "; \
          "INDICAT1%  "; HEX.VIEW1$(CHR$(IRF.INDICAT1%)); "h"; "              "; \
          "INDICAT8%  "; HEX.VIEW1$(CHR$(IRF.INDICAT8%)); "h"

    PRINT " 80 Item movement kept  ";  BIT$(IRF.INDICAT0%,80h); "  "; \
          " 80 Cntn Ibuprofen       "; BIT$(IRF.INDICAT1%,80h); "  "; \
          " 80 WEEE item         ";    BIT$(IRF.INDICAT8%,80h); " "

    PRINT " 40 Enforce qty entry   ";  BIT$(IRF.INDICAT0%,40h); "  "; \
          " 40 Insurance policy     "; BIT$(IRF.INDICAT1%,40h); "  "; \
          " 40 Withdrawn Recall  ";    BIT$(IRF.INDICAT8%,40h); " "

    PRINT " 20 Enforce price entry ";  BIT$(IRF.INDICAT0%,20h); "  "; \
          " 20 Non-solid painkiller "; BIT$(IRF.INDICAT1%,20h); "  "; \
          " 20 100% Returns      ";    BIT$(IRF.INDICAT8%,20h); " "

    PRINT " 10 Recall (block sale) ";  BIT$(IRF.INDICAT0%,10h); "  "; \
          " 10 Withdrawn sale block "; BIT$(IRF.INDICAT1%,10h); "  "; \
          " 10 Cntn Ephedrine    ";    BIT$(IRF.INDICAT8%,10h); " "

    PRINT " 08 Cntn alcohol        ";  BIT$(IRF.INDICAT0%,08h); "  "; \
          " 08 Giftcard item        "; BIT$(IRF.INDICAT1%,08h); "  "; \
          " 08 Nightingale prmpt ";    BIT$(IRF.INDICAT8%,08h); " "

    PRINT " 04 Not returnable      ";  BIT$(IRF.INDICAT0%,04h); "  "; \
          " 04 TPLU inclusion       "; BIT$(IRF.INDICAT1%,04h); "  "; \
          " 04 Unrestricted grp  ";    BIT$(IRF.INDICAT8%,04h); " "

    PRINT " 02 Not priced giveaway ";  BIT$(IRF.INDICAT0%,02h); "  "; \
          " 02 Cntn Paracetamol     "; BIT$(IRF.INDICAT1%,02h); "  "; \
          " 02 Chlamydia open sl ";    BIT$(IRF.INDICAT8%,02h); " "

    PRINT " 01 Cntn Statins        ";  BIT$(IRF.INDICAT0%,01h); "  "; \
          " 01 Cntn Aspirin         "; BIT$(IRF.INDICAT1%,01h); "  "; \
          " 01 Chlamydia NHS kit ";    BIT$(IRF.INDICAT8%,01h); " "

    PRINT " "

    PRINT "INDICAT9%  "; HEX.VIEW1$(CHR$(IRF.INDICAT9%)); "h"; "             "; \
          "INDICAT5%  "; HEX.VIEW1$(CHR$(IRF.INDICAT5%)); "h"; "              "; \
          "INDICAT3%  "; HEX.VIEW1$(CHR$(IRF.INDICAT3%)); "h"

    PRINT " 80 Boots.com ext       ";  BIT$(IRF.INDICAT9%,80h); "  "; \
          " 80 Guarantee 1=Mth 0=Yr "; BIT$(IRF.INDICAT5%,80h); "  "; \
          " 80 CSR item          ";    BIT$(IRF.INDICAT3%,80h); "x"

    PRINT " 40 Resaleable indct    ";  BIT$(IRF.INDICAT9%,40h); "  "; \
          " 40 Cntn Pseudoephedrine "; BIT$(IRF.INDICAT5%,40h); "  "; \
          " 40 Stock item        ";    BIT$(IRF.INDICAT3%,40h)

    PRINT " 3F Disposal Spc-Ins   ";   RIGHT$("00" + STR$(IRF.INDICAT9% AND 3Fh), 2); "d "; \
          " 3F Guarantee duration  ";  RIGHT$("00" + STR$(IRF.INDICAT5% AND 3Fh), 2); "d "; \
          " 20 Locally priced    ";    BIT$(IRF.INDICAT3%,20h)

    PRINT "                           "; \
          "                            "; \
          " 10 Wellbeing service "; BIT$(IRF.INDICAT3%,10h); "x"

    PRINT "INDICAT10%  "; HEX.VIEW1$(CHR$(IRF.INDICAT10%)); "h"; "           "; \
          "                             "; \
          " 08 Excl loyalty      ";  BIT$(IRF.INDICAT3%,08h)

    PRINT " E0 Returns route      "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 0E0h)); "h"; \
          "                             "; \
          " 04 Redeemable        ";  BIT$(IRF.INDICAT3%,04h)

    PRINT " 18 Ethical class      "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 18h)); "h"; \
          "                             "; \
          " 02 Boots/brand       ";  BIT$(IRF.INDICAT3%,02h)

    PRINT " 07 Age restriction    "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 07h)); "h"; \
          "                             "; \
          " 01 Discount exempt   ";  BIT$(IRF.INDICAT3%,01h)

RETURN


DISPLAY.DATA:

    PRINT "INDICAT10%  "; HEX.VIEW1$(CHR$(IRF.INDICAT10%)); "h"
    PRINT " E0 Returns route      "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 0E0h)); "h"
    PRINT " 18 Ethical class      "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 18h)); "h"
    PRINT " 07 Age restriction    "; HEX.VIEW1$(CHR$(IRF.INDICAT10% AND 07h)); "h"

RETURN


\*****************************************************************************
\***
\***    GENERAL ROUTINES:
\***
\***..........................................................................


\*****************************************************************************
\***
\***    CHECK.FILE.RETURN.CODE:
\***    If return code is not zero ...
\***        Diverts program control to FILE.ERROR.EXIT (exiting program).
\***
\***..........................................................................

CHECK.FILE.RETURN.CODE:

    IF RC2% <> OKAY% THEN GOSUB FILE.ERROR.EXIT

RETURN


\*****************************************************************************
\***
\***    CHECK.FUNCTION.RETURN.CODE:
\***    If FUNCTION.RETURN.CODE% is zero ...
\***        By-passes rest of procedure.
\***    Displays error message on background screen.
\***    Stops program.
\***
\***..........................................................................

CHECK.FUNCTION.RETURN.CODE:

    IF RC2% <> OKAY% THEN GOSUB STOP.PROGRAM

RETURN


\*****************************************************************************
\***
\***    FORMAT.CURRENT.CODE:
\***    Sets CURRENT.CODE.LOGGED$ for use with application event log.
\***
\***..........................................................................

FORMAT.CURRENT.CODE:

    IF   FILE.OPERATION$ = "C"      \ ! Create
      OR FILE.OPERATION$ = "O" THEN \ ! Open
        CURRENT.CODE.LOGGED$ = PACK$("00000000000000")

    IF   FILE.OPERATION$ = "R"      \ ! Read
      OR FILE.OPERATION$ = "W" THEN \ ! Write
        CURRENT.CODE.LOGGED$ = RIGHT$(CURRENT.CODE$,7)

RETURN



\*****************************************************************************
\***
\***    PROGRAM INDEPENDANT ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    CALL.F01.APPLICATION.LOG:
\***    References APPLICATION.LOG (F01) to write details of event defined
\***    by EVENT.NUMBER% and VAR.STRING.1$ to Application Event Log, and to
\***    display any message defined by MESSAGE.NUMBER% and VAR.STRING.2$.
\***
\***..........................................................................

CALL.F01.APPLICATION.LOG:

    RC2% = APPLICATION.LOG (MESSAGE.NUMBER%, \
                            VAR.STRING.1$, \
                            VAR.STRING.2$, \
                            EVENT.NUMBER%)

    IF RC2% <> OKAY% THEN GOSUB STOP.PROGRAM


!   IF APPLICATION.LOG (MESSAGE.NUMBER%, \
!                       VAR.STRING.1$, \
!                       VAR.STRING.2$, \
!                       EVENT.NUMBER%) \
!       <> OKAY% THEN GOSUB STOP.PROGRAM

RETURN


\*****************************************************************************
\***
\***    LOG.AN.EVENT.106:
\***    Writes details of Event 106 to application event log and displays
\***    message B501 (for file open errors) or B514 (for other errors).
\***
\***..........................................................................

LOG.AN.EVENT.106:

    EVENT.NUMBER% = 106

    GOSUB FORMAT.CURRENT.CODE

    VAR.STRING.1$ = \ ! Application event log data
      FILE.OPERATION$ + \
        CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) + \ ! Two byte integer byte order
        CHR$(SHIFT(CURRENT.REPORT.NUM%,0)) + \ ! reversed to give hex number
        CURRENT.CODE.LOGGED$

    GOSUB CALL.F01.APPLICATION.LOG

RETURN


\*****************************************************************************
\***
\***    PRINT.AN.EVENT.106:
\***    Prints details of application event 106 to screen
\***
\***..........................................................................

PRINT.AN.EVENT.106:

    PRINT " "
    PRINT "FILE ERROR at " + TIME.STAMP$
    PRINT "Event 106 ... "
    PRINT "File Operation  " + FILE.OPERATION$
    PRINT " Report Number  " + STR$(CURRENT.REPORT.NUM%)
    PRINT "  Current Code  " + UNPACK$(CURRENT.CODE.LOGGED$)

RETURN


\*****************************************************************************
\***
\***    IF END # AND ERROR.DETECTED ROUTINES
\***
\***..........................................................................


\*****************************************************************************
\***
\***    FILE.ERROR.EXIT:
\***    Logs events for specific file errors.
\***    Formats error message and displays on background screen.
\***    Logs an event 106.
\***    Stops program.
\***
\***..........................................................................

FILE.ERROR.EXIT:

    GOSUB FORMAT.CURRENT.CODE
!   GOSUB LOG.AN.EVENT.106
    GOSUB PRINT.AN.EVENT.106
    GOSUB STOP.PROGRAM



\*****************************************************************************
\***
\***    ERROR.DETECTED:
\***    Increments ERROR.COUNT% by one and tests it against values greater
\***    than one before any other commands executed.
\***    Further errors within ERROR.DETECTED causing control to be passed here
\***    again result in this test being failed and the immediate diversion of
\***    program control to STOP.PROGRAM.
\***    If no chaining parameters passed ...
\***        Diverts program control to OBTAIN.CHAIN.PARAMETERS.FROM.COMMAND
\***    References STANDARD.ERROR.DETECTED to log Event 101 and display
\***    message B550.
\***
\***..........................................................................

ERROR.DETECTED:

    ERROR.COUNT% = ERROR.COUNT% + 1

    IF ERROR.COUNT% > 1 THEN RESUME STOP.PROGRAM

    PRINT "ERROR.DETECTED at " + TIME.STAMP$
    PRINT "ERRN .... " + ERRNH ! Function call to translate ERRN
    PRINT "ERRF% ... " + STR$(ERRF%)
    PRINT "ERR ..... " + ERR
    PRINT "ERRL .... " + STR$(ERRL)

    RC2% = STANDARD.ERROR.DETECTED (ERRN, ERRF%, ERRL, ERR)

RESUME STOP.PROGRAM


\*****************************************************************************
\*****************************************************************************
\***
\***    End of program PMITEM
\***
\*****************************************************************************
\*****************************************************************************


