\*******************************************************************************
\*******************************************************************************
\***
\***    %ITEM-DEALS FILE FUNCTIONS
\***
\***        REFERENCE   :   ITMDLFUN.BAS
\***        FILE TYPE   :   Keyed
\***        AUTHOR      :   Sandhya Pillai
\***        DATE        :   09/06/2014
\***
\***
\***    Version B               Charles Skadorwa                      09/07/2014
\***    Minor changes following internal code review
\***
\***    Version C               Mark Walker                           11/07/2014
\***    Added functions for adding and removing individual items.
\***    Improved efficiency and consistency in a few areas.
\***
\***    Version D               Mark Walker                           15/07/2014
\***    Added local pricing support.
\***
\***    Version E               Mark Walker                           16/07/2014
\***    Incorporated changes following Applications Management code review:
\***    - Sorted variables into alphabetical order (grouped by type)
\***    - Corrected usage of the ITMDL.DEAL.END% variable
\***    Fixed the WRITE.ITMDL.REC function to handle more than 20 deals.
\***
\***    Version F              Sandhya Pillai                         17/07/2014
\***    Added Maximum deals per Item variable as a global which can be
\***    used by the calling program
\***
\***    Version G               Mark Walker                           24/07/2014
\***    Includes the following enhancements:
\***    - Corrected value of ITMDL.DEALID.START.POS% and modified code to
\***      work with the proper value.
\***    - Improved efficiency of the SPLIT.READ.ITMDL.DIRECT function.
\***    - Changed local variables ITMDL.DEAL.MAX% and ITMDL.DEALID.START.POS%
\***      to global (in ITMDLDEC.J86) to allow them to be referenced by 
\***      calling programs.
\***
\***    Version H               Mark Walker                           24/07/2014
\***    Calculate the value of ITMDL.MAXDEALS.PER.ITEM%.
\***
\***    Version I               Mark Walker                           01/08/2014
\***    Includes the following AM code review comments:
\***    - Sorted further variable definitions and declarations into
\***      alphabetical order (grouped by type).
\***
\***    Version J               Mark Walker                           14/08/2014
\***    Added file function to open the ITMDL file.
\***
\*******************************************************************************
\*******************************************************************************
%INCLUDE ITMDLDEC.J86 ! ITMDL variable declarations

    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$

    INTEGER*1 GLOBAL                                                    \   !CMW
        FALSE,                                                          \   !CMW
        TRUE                                                                !CMW

    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%

    STRING                                                              \
        ITMDL.DEAL.NUM.LID$,                                            \   !CMW
        ITMDL.DEAL.NUM.LID.20$,                                         \
        ITMDL.ITEM.REC.KEY$,                                            \
        ITMDL.RECORD$,                                                  \
        ITMDL.RESERVED$,                                                \
        NULL.DEAL.NUM.LID$                                                  !CMW

    INTEGER*1                                                           \
\       ITMDL.DEALID.START.POS%,                                        \   !GMW
        ITMDL.DEALS.TO.WRITE%,                                          \
        ITMDL.RC%,                                                      \
        ITMDL.REC.MAX%,                                                 \
        ITMDL.RECS.TO.WRITE%

    INTEGER*2                                                          \
        ITMDL.DEAL.END%,                                               \
\       ITMDL.DEAL.MAX%,                                               \    !GMW
        ITMDL.DEAL.START%,                                             \
        ITMDL.REC.NUM%

%INCLUDE BASROUT.J86 ! 'C' utility functions                                !CMW

\***********************************************************************
\*
\* FUNCTION: ITMDL.SET
\* Sets Item-Deals File Variables
\*
\***********************************************************************
FUNCTION ITMDL.SET PUBLIC

    INTEGER*1 ITMDL.SET

    ! Set Boolean flag values (just in case!)                               !CMW
    TRUE = -1                                                               !CMW
    FALSE = 0                                                               !CMW

    ITMDL.SET = 1                     ! Error

    ITMDL.DEAL.COUNT%         = 0       ! Count of Deal Number - List ID
    ITMDL.DEAL.MAX%           = 20      ! Max deals per ITMDL record        !HMW
    ITMDL.DEALID.START.POS%   = 13      ! Deal ID Start Position            !GMW
    ITMDL.DEALNUM.LID.LEN%    = 3       ! Deal Number and List ID Length
    ITMDL.FILE.NAME$          = "ITMDL" ! Item-Deals Logical file name
!   ITMDL.MAXDEALS.PER.ITEM%  = 1980    ! Max number of deals per item      !HMW
    ITMDL.MAX.ITEMS.PER.DEAL% = 40000   ! Max number of items per deal      !GMW
    ITMDL.REC.MAX%            = 99      ! Max ITMDL records per item        !HMW
    ITMDL.RECL%               = 72      ! Item-Deal File Record Length
    ITMDL.REPORT.NUM%         = 235     ! Report Number
    ITMDL.RESERVED$           = STRING$(7,CHR$(0))! Reserved                !GMW
    NULL.DEAL.NUM.LID$        = PACK$("000000") ! Null deal/list ID         !GMW

    ITMDL.DEAL.NUM.LID.20$  = STRING$(ITMDL.DEALNUM.LID.LEN% *          \   !CMW
                              ITMDL.DEAL.MAX%,CHR$(0)) ! Set to Null        !CMW

    ! Calculate the maximum number of deals per item that can possibly      !HMW
    ! occur on the ITMDL file (in reality, this is limited at present       !HMW
    ! to 40 deals per item)                                                 !HMW
    ITMDL.MAXDEALS.PER.ITEM% = ITMDL.DEAL.MAX% * ITMDL.REC.MAX%             !HMW
    
    ITMDL.ALL.DEAL.NUM.LIST.ID$ = ""

    ITMDL.SET = 0                     ! No Error

END FUNCTION

\***********************************************************************
\*
\* FUNCTION: READ.ITMDL.REC
\* Physical Read of Item-Deal File and populate string to holds all of
\* an items deal-numbers-list-ID pairs accumulated from successive reads
\* of all of an items ITMDL records
\*
\***********************************************************************
FUNCTION READ.ITMDL.REC

    INTEGER*1 READ.ITMDL.REC

    ON ERROR GOTO READ.ITMDL.REC.ERROR                                      !CMW

    READ.ITMDL.REC = 1  ! Error

    ITMDL.DEAL.START% = 1

    IF END # ITMDL.SESS.NUM% THEN READ.ITMDL.REC.IF.END
    READ FORM "T5,C1,C7,C60"; #ITMDL.SESS.NUM%                          \
                    KEY ITMDL.ITEM.REC.KEY$;                            \
                        ITMDL.EXCLUSION$,                               \
                        ITMDL.RESERVED$,                                \
                        ITMDL.DEAL.NUM.LID.20$

    ! Checking whether ITMDL.DEAL.NUM.LID.20$ holds Deal Number and
    ! List ID for 20 times per record
    WHILE ITMDL.DEAL.START% <= ITMDL.DEAL.MAX%

        ! Get next deal number/list ID                                      !CMW
        ITMDL.DEAL.NUM.LID$ = MID$(ITMDL.DEAL.NUM.LID.20$,              \   !CMW
            (1 + ((ITMDL.DEAL.START% - 1) * ITMDL.DEALNUM.LID.LEN%)),   \   !CMW
            ITMDL.DEALNUM.LID.LEN%)                                         !CMW

        IF ITMDL.DEAL.NUM.LID$ <> NULL.DEAL.NUM.LID$ THEN BEGIN             !CMW

            ! Store ITMDL.DEAL.NUM.LID.20$ without null values to
            ! ITMDL.ALL.DEAL.NUM.LIST.ID$

            ITMDL.ALL.DEAL.NUM.LIST.ID$ =                               \
                ITMDL.ALL.DEAL.NUM.LIST.ID$ +                           \
                ITMDL.DEAL.NUM.LID$                                         !CMW

            ! Increment counter
            ITMDL.DEAL.START% = ITMDL.DEAL.START% + 1
        ENDIF ELSE BEGIN
            ITMDL.DEAL.START% = ITMDL.DEAL.MAX% + 1
        ENDIF
    WEND

    READ.ITMDL.REC = 0  ! No error

EXIT.FUNCTION:                                                              !CMW

    EXIT FUNCTION

READ.ITMDL.REC.IF.END:

    FILE.OPERATION$     = "R" ! Read
    CURRENT.REPORT.NUM% = ITMDL.REPORT.NUM%
    CURRENT.CODE$       = ITMDL.ITEM.REC.KEY$

READ.ITMDL.REC.ERROR:                                                       !CMW

    RESUME EXIT.FUNCTION                                                    !CMW

END FUNCTION

\***********************************************************************
\*
\* FUNCTION: READ.ITMDL
\* Read Item-Deal File. This function will populate ITMDL.ALL.DEAL.NUM.
\* LIST.ID$ and ITMDL.DEAL.COUNT% variables to be used for the calling
\* program
\*
\***********************************************************************
FUNCTION READ.ITMDL PUBLIC

    INTEGER*1 READ.ITMDL

    ON ERROR GOTO READ.ITMDL.ERROR                                          !CMW

    READ.ITMDL = 1  ! Error

    ITMDL.DEAL.COUNT% = 0
    ITMDL.REC.NUM%    = 1       ! Read first record from ITMDL

    ITMDL.ALL.DEAL.NUM.LIST.ID$ = ""

    ! Set Item Record Number
    ITMDL.REC.NUM$ = PACK$(RIGHT$("00" + STR$(ITMDL.REC.NUM%),2))

    !Set Item Key from Item Code and Record Number
    ITMDL.ITEM.REC.KEY$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$

    ! Read first record
    ITMDL.RC% = READ.ITMDL.REC  ! Populates ITMDL.ALL.DEAL.NUM.LIST.ID$

    ! First record expected but not found
    IF ITMDL.RC% <> 0 THEN BEGIN
        EXIT FUNCTION  ! Item has not records and so no more deals
    ENDIF

    ! If ITMDL final deal entry is not in use, then there are no
    ! more deals for this item so exit function
    IF RIGHT$(ITMDL.DEAL.NUM.LID.20$,                                   \   !CMW
           ITMDL.DEALNUM.LID.LEN%) = NULL.DEAL.NUM.LID$ THEN BEGIN          !CMW
        ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /          \
                            ITMDL.DEALNUM.LID.LEN%
        READ.ITMDL = 0
        EXIT FUNCTION
    ENDIF

    ! Read second record
    ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1

    WHILE ITMDL.REC.NUM% <= ITMDL.REC.MAX%
        ! Set Item Record Number
        ITMDL.REC.NUM$ = PACK$(RIGHT$("00" + STR$(ITMDL.REC.NUM%),2))

        ! Set Item Key from Item Code and Record Number
        ITMDL.ITEM.REC.KEY$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$

        ! Read next record
        ! Populate ITMDL.ALL.DEAL.NUM.LIST.ID$
        ITMDL.RC% = READ.ITMDL.REC

        ! Next record not found.
        IF ITMDL.RC% <> 0 THEN BEGIN
            ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /      \
                                ITMDL.DEALNUM.LID.LEN%
            READ.ITMDL = 0
            EXIT FUNCTION
        ENDIF
        ! If ITMDL final deal entry is not in use, then there are no
        ! more deals for this item so exit function
        IF RIGHT$(ITMDL.DEAL.NUM.LID.20$,                               \   !CMW
               ITMDL.DEALNUM.LID.LEN%) = NULL.DEAL.NUM.LID$ THEN BEGIN      !CMW
            ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /      \
                                ITMDL.DEALNUM.LID.LEN%
            READ.ITMDL = 0
            EXIT FUNCTION
        ENDIF

        ! Increment record count
        ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1

    WEND

    ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /              \
                        ITMDL.DEALNUM.LID.LEN%

    ! Set return code to zero on successful completion
    READ.ITMDL = 0

EXIT.FUNCTION:                                                              !CMW

    EXIT FUNCTION

READ.ITMDL.ERROR:                                                           !CMW

    RESUME EXIT.FUNCTION                                                    !CMW

END FUNCTION

\***********************************************************************
\*
\* FUNCTION: WRITE.ITMDL.REC
\* Physical write to Item-Deal File with string that holds all of
\* an items deal-numbers-list-ID pairs
\*
\***********************************************************************
FUNCTION WRITE.ITMDL.REC

    INTEGER*1 WRITE.ITMDL.REC

    ON ERROR GOTO WRITE.ITMDL.REC.ERROR

    WRITE.ITMDL.REC = 1 ! Error

    ITMDL.DEAL.NUM.LID.20$  = ""

    ! Populate ITMDL.DEAL.NUM.LID.20$ variable
    WHILE ITMDL.DEAL.START% <= ITMDL.DEAL.END% AND                      \   !EMW
          ITMDL.DEAL.START% <= ITMDL.DEALS.TO.WRITE%                        !EMW

        ! Get next deal number/list ID                                      !CMW
        ITMDL.DEAL.NUM.LID$ = MID$(ITMDL.ALL.DEAL.NUM.LIST.ID$,         \   !CMW
            (1 + ((ITMDL.DEAL.START% - 1) * ITMDL.DEALNUM.LID.LEN%)),   \   !CMW
            ITMDL.DEALNUM.LID.LEN%)                                         !CMW

        IF ITMDL.DEAL.NUM.LID$ <> NULL.DEAL.NUM.LID$ THEN BEGIN             !CMW

            ! Store ITMDL.DEAL.NUM.LID.20$ without null values from
            ! ITMDL.ALL.DEAL.NUM.LIST.ID$

            ITMDL.DEAL.NUM.LID.20$ =                                    \
                ITMDL.DEAL.NUM.LID.20$ +                                \
                ITMDL.DEAL.NUM.LID$                                         !CMW

            ! Increment counter
            ITMDL.DEAL.START% = ITMDL.DEAL.START% + 1
        ENDIF
    WEND

    ITMDL.DEAL.END% =  ITMDL.DEAL.START% + ITMDL.DEAL.MAX% - 1

    ! Padding zeros to ITMDL.DEAL.NUM.LID.20% if the string doesn't
    ! have 20 deals in it.
    ITMDL.DEAL.NUM.LID.20$ =                                           \    !CMW
        LEFT$(ITMDL.DEAL.NUM.LID.20$ +                                 \    !CMW
        STRING$(ITMDL.DEAL.MAX%,NULL.DEAL.NUM.LID$),                   \    !CMW
        ITMDL.DEAL.MAX% * ITMDL.DEALNUM.LID.LEN%)                           !CMW

    IF END # ITMDL.SESS.NUM% THEN WRITE.ITMDL.IF.END
    WRITE FORM "C3,C1,C1,C7,C60"; #ITMDL.SESS.NUM%;                    \
                                 ITMDL.ITEM.CODE$,                     \
                                 ITMDL.REC.NUM$,                       \
                                 ITMDL.EXCLUSION$,                     \
                                 ITMDL.RESERVED$,                      \
                                 ITMDL.DEAL.NUM.LID.20$

    WRITE.ITMDL.REC = 0 ! No error

EXIT.FUNCTION:                                                              !CMW

    EXIT FUNCTION

WRITE.ITMDL.IF.END:

    FILE.OPERATION$     = "W" ! Write
    CURRENT.REPORT.NUM% = ITMDL.REPORT.NUM%
    CURRENT.CODE$       = ITMDL.ITEM.REC.KEY$

    EXIT FUNCTION

WRITE.ITMDL.REC.ERROR:                                                      !CMW

    RESUME EXIT.FUNCTION                                                    !CMW

END FUNCTION

\***********************************************************************
\*
\* FUNCTION: WRITE.ITMDL
\* Write Item-Deal File. The calling program will populate
\* ITMDL.ALL.DEAL.NUM.LIST.ID$ variables and all ITMDL file variables.
\* Calculate the records to be written to the file and write to ITMDL
\* file. Delete remaining records from the file
\*
\***********************************************************************
FUNCTION WRITE.ITMDL PUBLIC

    INTEGER*1 WRITE.ITMDL

    ON ERROR GOTO WRITE.ITMDL.ERROR                                         !CMW

    WRITE.ITMDL = 1 ! Error

    ITMDL.DEAL.END%   = ITMDL.DEAL.MAX%
    ITMDL.DEAL.START% = 1
    ITMDL.RESERVED$   = STRING$(7,CHR$(0)) ! Null 7 bytes

    ! Number of deals to write to ITMDL file
    ITMDL.DEALS.TO.WRITE% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /          \
                            ITMDL.DEALNUM.LID.LEN%

    ! Number of records in which the deals will be written
    ITMDL.RECS.TO.WRITE%  = (ITMDL.DEALS.TO.WRITE% +                    \
                             ITMDL.DEAL.MAX% - 1) / ITMDL.DEAL.MAX%

    ITMDL.REC.NUM% = 1

    WHILE ITMDL.REC.NUM% <= ITMDL.RECS.TO.WRITE%
        ! Set Item Record Number
        ITMDL.REC.NUM$ = PACK$(RIGHT$("00" + STR$(ITMDL.REC.NUM%),2))

        !Set Item Key from Item Code and Record Number
        ITMDL.ITEM.REC.KEY$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$

        ! Write ITMDL record
        ! Fields are populated from the calling program
        ! to write to ITMDL file
        ITMDL.RC% = WRITE.ITMDL.REC

        ! Return code not set to zero
        IF ITMDL.RC% <> 0 THEN BEGIN
            EXIT FUNCTION
        ENDIF

        ! Increment record count
        ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1

    WEND

    WHILE ((ITMDL.REC.NUM% = ITMDL.RECS.TO.WRITE%)  OR                  \
          (ITMDL.REC.NUM% <= ITMDL.REC.MAX% ))
        ! Set Item Record Number
        ITMDL.REC.NUM$ = PACK$(RIGHT$("00" + STR$(ITMDL.REC.NUM%),2))

        !Set Item Key from Item Code and Record Number
        ITMDL.ITEM.REC.KEY$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$

        !Delete the remaining records after writing to the file
        IF END #ITMDL.SESS.NUM% THEN DEL.ITMDL.ERROR
        DELREC ITMDL.SESS.NUM%; ITMDL.ITEM.REC.KEY$

        ! Increment record count
        ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1
    WEND
    ! Set return code to zero on successful completion
    WRITE.ITMDL = 0

EXIT.FUNCTION:                                                              !CMW

    EXIT FUNCTION

DEL.ITMDL.ERROR:

    ! Set return code to zero on successful completion
    WRITE.ITMDL = 0

EXIT FUNCTION

WRITE.ITMDL.ERROR:                                                          !CMW

    RESUME EXIT.FUNCTION                                                    !CMW

END FUNCTION

\***********************************************************************
\*
\* FUNCTION: SPLIT.READ.ITMDL.DIRECT
\* Calling Program uses PROCESS.KEYED.FILE to open ITMDL in direct mode
\* and gets full record into ITMDL.RECORD$ and split the records as
\* populated by READ.ITMDL.
\* Programs calling this function will only pass if the item is first(or
\* only record) in ITMDL and reject any others
\* Function will perform further physical reads of ITMDL to obtain all
\* items deal data and populate string ITMDL.ALL.DEAL.NUM.LIST.ID$
\*
\***********************************************************************
FUNCTION SPLIT.READ.ITMDL.DIRECT(ITMDL.RECORD$) PUBLIC
    INTEGER*1 SPLIT.READ.ITMDL.DIRECT
    INTEGER*2 DEAL.POS%                                                     !GMW
    STRING DEAL.NUM.LID$                                                    !GMW
    STRING ITMDL.RECORD$

    SPLIT.READ.ITMDL.DIRECT = 1 ! Error

    ON ERROR GOTO SPLIT.READ.ITMDL.DIRECT.ERROR                             !CMW

    ITMDL.DEAL.START% = 1
    ITMDL.ALL.DEAL.NUM.LIST.ID$ = ""
    ITMDL.DEAL.NUM.LID.20$      = ""

    ITMDL.ITEM.REC.KEY$ = MID$(ITMDL.RECORD$,1,4)
    ITMDL.ITEM.CODE$    = MID$(ITMDL.RECORD$,1,3)
    ITMDL.REC.NUM$      = MID$(ITMDL.RECORD$,4,1)
    ITMDL.REC.NUM%      = VAL(UNPACK$(ITMDL.REC.NUM$))

    ! Reject any ITMDL records that are not the first(or only)
    ! record for the item
    IF ITMDL.REC.NUM% <> 1 THEN BEGIN
        EXIT FUNCTION
    ENDIF

    ! Populate remaining ITMDL variables from ITMDL.RECORD
    ITMDL.EXCLUSION$ = MID$(ITMDL.RECORD$,5,1)
    ITMDL.RESERVED$  = MID$(ITMDL.RECORD$,6,7)

    ! Populate ITMDL.ALL.DEAL.NUM.LIST.ID$ and ITMDL.DEAL.NUM.LID.20$
    ! from ITMDL.RECORD$ bytes 13 to 72
    ! which holds the Deal Number List ID value

    ! Initialise deal number/list ID                                        !GMW
    DEAL.NUM.LID$ = STRING$(ITMDL.DEALNUM.LID.LEN%,CHR$(0))                 !GMW
    WHILE ITMDL.DEAL.START% <= ITMDL.DEAL.MAX%

        ! Get index position of the next deal number/list ID                !GMW
        DEAL.POS% = ((ITMDL.DEAL.START% - 1) *                          \   !GMW
                    ITMDL.DEALNUM.LID.LEN%) +                           \   !GMW
                    ITMDL.DEALID.START.POS%                                 !GMW
                                                                            !GMW
        ! Get the next deal number/list ID                                  !GMW
        CALL EXTRACTS(ITMDL.RECORD$,                                    \   !GMW
                      DEAL.NUM.LID$,                                    \   !GMW
                      DEAL.POS%)                                            !GMW
        
        ! IF deal number/list ID NOT found                                  !GMW
        IF DEAL.NUM.LID$ <> NULL.DEAL.NUM.LID$ THEN BEGIN                   !GMW
                                                                            !GMW
            ! Add deal number/list ID to the current deals list             !GMW
            ITMDL.ALL.DEAL.NUM.LIST.ID$ =                               \   !GMW
                ITMDL.ALL.DEAL.NUM.LIST.ID$ + DEAL.NUM.LID$                 !GMW
                                                                            !GMW
            ! Add deal number/list ID to the deals list                     !GMW
            ! for this record                                               !GMW
            ITMDL.DEAL.NUM.LID.20$ =                                    \   !GMW
                ITMDL.DEAL.NUM.LID.20$ + DEAL.NUM.LID$                      !GMW

            ITMDL.DEAL.START% = ITMDL.DEAL.START% + 1
        ENDIF ELSE BEGIN
            ITMDL.DEAL.START% = ITMDL.DEAL.MAX% + 1
        ENDIF
    WEND

    ! Read second record
    ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1

    WHILE ITMDL.REC.NUM% <= ITMDL.REC.MAX%
        ! Set Item Record Number
        ITMDL.REC.NUM$ = PACK$(RIGHT$("00" + STR$(ITMDL.REC.NUM%),2))

        ! Set Item Key from Item Code and Record Number
        ITMDL.ITEM.REC.KEY$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$

        ! Read next record
        ! Populate ITMDL.ALL.DEAL.NUM.LIST.ID$
        ITMDL.RC% = READ.ITMDL.REC

        ! Next record not found.
        IF ITMDL.RC% <> 0 THEN BEGIN
            ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /      \
                                ITMDL.DEALNUM.LID.LEN%
            SPLIT.READ.ITMDL.DIRECT = 0 ! Item has no further records
            EXIT FUNCTION
        ENDIF

        ! If ITMDL final deal entry is not in use, then there are no
        ! more deals for this item so exit function
        IF RIGHT$(ITMDL.DEAL.NUM.LID.20$,ITMDL.DEALNUM.LID.LEN%) =      \   !CMW
               NULL.DEAL.NUM.LID$ THEN BEGIN                                !CMW
            ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /      \
                                ITMDL.DEALNUM.LID.LEN%
            SPLIT.READ.ITMDL.DIRECT = 0
            EXIT FUNCTION
        ENDIF

        ! Increment record count
        ITMDL.REC.NUM% = ITMDL.REC.NUM% + 1

    WEND
    ITMDL.DEAL.COUNT% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) /              \
                            ITMDL.DEALNUM.LID.LEN%
    SPLIT.READ.ITMDL.DIRECT = 0 ! No Error

EXIT.FUNCTION:                                                              !CMW

    EXIT FUNCTION

SPLIT.READ.ITMDL.DIRECT.ERROR:                                              !CMW

    RESUME EXIT.FUNCTION                                                    !CMW

END FUNCTION

\***********************************************************************
\*
\* SubProgram: ITMDL.SAVE
\* Save ITMDL field variables. This sub-program is in use for PSBF42
\* program which when calls ITMDL.SAVE will save the ITMDL data
\* The parameter SAVE$ holds the value and passes it back to PSBF42
\*
\***********************************************************************
SUB ITMDL.SAVE(SAVE$) PUBLIC
    STRING SAVE$

    SAVE$ = ITMDL.ITEM.CODE$ + ITMDL.REC.NUM$ + ITMDL.EXCLUSION$ +      \
            ITMDL.RESERVED$  + ITMDL.DEAL.NUM.LID.20$

END SUB

\***********************************************************************
\*
\* Sub-Program: ITMDL.RESTORE
\* Restore ITMDL file variables. This function is called after
\* ITMDL.SAVE and prior to function exit. It is used in PSBF42
\*
\***********************************************************************
SUB ITMDL.RESTORE(RESTORE$) PUBLIC
    STRING RESTORE$

    ITMDL.ITEM.REC.KEY$    = MID$(RESTORE$,1,4)
    ITMDL.ITEM.CODE$       = MID$(RESTORE$,1,3)
    ITMDL.REC.NUM$         = MID$(RESTORE$,4,1)
    ITMDL.EXCLUSION$       = MID$(RESTORE$,5,1)
    ITMDL.RESERVED$        = MID$(RESTORE$,6,7)
    ITMDL.DEAL.NUM.LID.20$ = MID$(RESTORE$,13,LEN(RESTORE$))                !CMW

END SUB

\***************************************************************************!CMW
\***                                                                        !CMW
\***   ITMDL.ADD.DEAL.ITEM                                                  !CMW
\***                                                                        !CMW
\***   Add an individual item to the Item Deal file (ITMDL)                 !CMW
\***                                                                        !CMW
\***************************************************************************!CMW
                                                                            !CMW
FUNCTION ITMDL.ADD.DEAL.ITEM(DEAL.NUMBER$,                              \   !DMW
                             LIST.NUMBER%,                              \   !DMW
                             ITEM.CODE$,                                \   !DMW
                             EXCLUSION.FLAG%) PUBLIC                        !DMW
                                                                            !CMW
    INTEGER*1   DEAL.FOUND                                                  !CMW
    INTEGER*1   END.OF.DEAL.LIST                                            !CMW
    INTEGER*1   EXCLUSION.FLAG% ! Local price flag                          !DMW
    INTEGER*1   ITMDL.ADD.DEAL.ITEM                                         !CMW
    INTEGER*1   LIST.NUMBER%                                                !CMW
    INTEGER*1   NEW.DEAL                                                    !CMW
                                                                            !CMW
    INTEGER*2   DEAL.COUNT%                                                 !CMW
    INTEGER*2   DEAL.POS%                                                   !CMW
    INTEGER*2   LIST.POS%                                                   !CMW
    INTEGER*2   LOOP%                                                       !CMW
                                                                            !CMW
    STRING      DEAL.NUMBER$                                                !IMW
    STRING      EXCLUSION$                                                  !DMW
    STRING      EXISTING.DEAL.NUMBER$                                       !CMW
    STRING      EXISTING.LIST.ID$                                           !CMW
    STRING      ITEM.CODE$                                                  !CMW
    STRING      LIST.ID$                                                    !CMW
                                                                            !CMW
    ON ERROR GOTO ITMDL.ADD.DEAL.ITEM.ERROR                                 !CMW
                                                                            !CMW
    ITMDL.ADD.DEAL.ITEM = 0 ! Assume success                                !CMW
                                                                            !CMW
    DEAL.FOUND           = FALSE                                            !CMW
    END.OF.DEAL.LIST     = FALSE                                            !IMW
    NEW.DEAL             = FALSE                                            !IMW
                                                                            !CMW
    ITMDL.ITEM.CODE$ = ITEM.CODE$                                           !CMW
                                                                            !CMW
    ! Convert list number to list ID                                        !CMW
    LIST.ID$ = PACK$(RIGHT$("00" + STR$(LIST.NUMBER%),2))                   !CMW
                                                                            !CMW
    ! IF item is locally priced                                             !DMW
    IF EXCLUSION.FLAG% = 1 THEN BEGIN                                       !DMW
        EXCLUSION$ = "L"                                                    !DMW
    ENDIF ELSE BEGIN                                                        !DMW
        EXCLUSION$ = " "                                                    !DMW
    ENDIF                                                                   !DMW
                                                                            !DMW
    ! Initialise current deals list                                         !CMW
    ITMDL.ALL.DEAL.NUM.LIST.ID$ = ""                                        !CMW
                                                                            !CMW
    ! Read ITMDL record                                                     !CMW
    ITMDL.RC% = READ.ITMDL                                                  !CMW
                                                                            !CMW
    ! IF ITMDL record found                                                 !CMW
    IF ITMDL.RC% <> 0 THEN BEGIN                                            !CMW
        NEW.DEAL = TRUE                                                     !CMW
    ENDIF                                                                   !CMW
                                                                            !CMW
    ! Get count of existing deals                                           !CMW
    DEAL.COUNT% =                                                       \   !CMW
        LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) / ITMDL.DEALNUM.LID.LEN%           !CMW
                                                                            !CMW
    ! Initialise existing deal number, list ID and local price flag         !DMW
    EXISTING.DEAL.NUMBER$ = STRING$(2,CHR$(0))                              !CMW
    EXISTING.LIST.ID$     = CHR$(0)                                         !CMW
                                                                            !CMW
    ! Initialise loop counter                                               !CMW
    LOOP% = 0                                                               !CMW
                                                                            !CMW
    ! WHILE the end of the current deals list has NOT been reached AND      !CMW
    !       the deal has NOT been found in the current deals list AND       !CMW
    !       the current deals list already contained some deals             !CMW
    WHILE (NOT END.OF.DEAL.LIST) AND                                    \   !CMW
          (NOT DEAL.FOUND) AND                                          \   !CMW
          (DEAL.COUNT% > 0)                                                 !CMW
                                                                            !CMW
        DEAL.POS% =                                                     \   !CMW
            (LOOP% * ITMDL.DEALNUM.LID.LEN%) + 1 ! Deal index position      !CMW
                                                                            !CMW
        LIST.POS% = DEAL.POS% + 2 ! List ID index position                  !CMW
                                                                            !CMW
        ! Get the next existing deal number                                 !CMW
        CALL EXTRACTS(ITMDL.ALL.DEAL.NUM.LIST.ID$,                      \   !CMW
                      EXISTING.DEAL.NUMBER$,                            \   !CMW
                      DEAL.POS%)                                            !CMW
                                                                            !CMW
        ! Get the next existing list ID                                     !CMW
        CALL EXTRACTS(ITMDL.ALL.DEAL.NUM.LIST.ID$,                      \   !CMW
                      EXISTING.LIST.ID$,                                \   !CMW
                      LIST.POS%)                                            !CMW
                                                                            !CMW
        ! IF the specified deal number already exists for this item         !CMW
        IF DEAL.NUMBER$ = EXISTING.DEAL.NUMBER$ THEN BEGIN                  !CMW
                                                                            !CMW
            ! IF the list number has changed                                !CMW
            IF LIST.ID$ <> EXISTING.LIST.ID$ THEN BEGIN                     !CMW
                                                                            !CMW
                ! Update the list number only                               !CMW
                CALL INSERTS(ITMDL.ALL.DEAL.NUM.LIST.ID$,               \   !CMW
                             LIST.ID$,                                  \   !CMW
                             LIST.POS%)                                     !CMW
                                                                            !CMW
            ENDIF                                                           !CMW
                                                                            !CMW
            DEAL.FOUND = TRUE                                               !CMW
                                                                            !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
        LOOP% = LOOP% + 1                                                   !CMW
                                                                            !CMW
        ! IF end of the current deals list has been reached                 !CMW
        IF LOOP% = DEAL.COUNT% THEN BEGIN                                   !CMW
            END.OF.DEAL.LIST = TRUE                                         !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
    WEND                                                                    !CMW
                                                                            !CMW
    ! IF the specified deal does NOT exist in the current deals list OR     !CMW
    !    the deal itself currently does NOT exist at all                    !CMW
    IF (NOT DEAL.FOUND) OR                                              \   !CMW
       (NEW.DEAL) THEN BEGIN                                                !CMW
                                                                            !CMW
        ! Set local price flag to its current value                         !DMW
        ITMDL.EXCLUSION$ = EXCLUSION$                                       !DMW
                                                                            !DMW
        ! Add deal number and list ID to the current deals list             !CMW
        ITMDL.ALL.DEAL.NUM.LIST.ID$ =                                   \   !CMW
            ITMDL.ALL.DEAL.NUM.LIST.ID$ +                               \   !CMW
            DEAL.NUMBER$ +                                              \   !CMW
            LIST.ID$                                                        !CMW
                                                                            !CMW
        ! Update ITMDL record                                               !CMW
        ITMDL.RC% = WRITE.ITMDL                                             !CMW
                                                                            !CMW
        ! IF error writing to ITMDL                                         !CMW
        IF ITMDL.RC% <> 0 THEN BEGIN                                        !CMW
            ITMDL.ADD.DEAL.ITEM = 1 ! Failure                               !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
    ENDIF                                                                   !CMW
                                                                            !CMW
EXIT.FUNCTION:                                                              !CMW
                                                                            !CMW
    EXIT FUNCTION                                                           !CMW
                                                                            !CMW
ITMDL.ADD.DEAL.ITEM.ERROR:                                                  !CMW
                                                                            !CMW
    ITMDL.ADD.DEAL.ITEM = 1 ! Failure                                       !CMW
    RESUME EXIT.FUNCTION                                                    !CMW
                                                                            !CMW
END FUNCTION                                                                !CMW
                                                                            !CMW
\***************************************************************************!CMW
\***                                                                        !CMW
\***   ITMDL.REMOVE.DEAL.ITEM                                               !CMW
\***                                                                        !CMW
\***   Remove an individual item from the Item Deal file (ITMDL)            !CMW
\***                                                                        !CMW
\***************************************************************************!CMW
                                                                            !CMW
FUNCTION ITMDL.REMOVE.DEAL.ITEM(DEAL.NUMBER$,ITEM.CODE$) PUBLIC             !CMW
                                                                            !CMW
    INTEGER*1   DEAL.FOUND                                                  !CMW
    INTEGER*1   END.OF.DEAL.LIST                                            !CMW
    INTEGER*1   ITMDL.REMOVE.DEAL.ITEM                                      !CMW
                                                                            !CMW
    INTEGER*2   DEAL.COUNT%                                                 !CMW
    INTEGER*2   DEAL.POS%                                                   !CMW
    INTEGER*2   LEFT.LEN%                                                   !CMW
    INTEGER*2   LOOP%                                                       !CMW
    INTEGER*2   RIGHT.LEN%                                                  !CMW
                                                                            !CMW
    STRING      DEAL.NUMBER$                                                !IMW
    STRING      EXISTING.DEAL.NUMBER$                                       !CMW
    STRING      ITEM.CODE$                                                  !CMW
                                                                            !CMW
    ON ERROR GOTO ITMDL.REMOVE.DEAL.ITEM.ERROR                              !CMW
                                                                            !CMW
    ITMDL.REMOVE.DEAL.ITEM = 0 ! Assume success                             !CMW
                                                                            !CMW
    DEAL.FOUND       = FALSE                                                !CMW
    END.OF.DEAL.LIST = FALSE                                                !CMW
                                                                            !CMW
    ITMDL.ITEM.CODE$ = ITEM.CODE$                                           !CMW
                                                                            !CMW
    ! Initialise current deals list                                         !CMW
    ITMDL.ALL.DEAL.NUM.LIST.ID$ = ""                                        !CMW
                                                                            !CMW
    ! Read ITMDL record                                                     !CMW
    ITMDL.RC% = READ.ITMDL                                                  !CMW
                                                                            !CMW
    ! IF ITMDL record found                                                 !CMW
    IF ITMDL.RC% <> 0 THEN BEGIN                                            !CMW
        EXIT FUNCTION                                                       !CMW
    ENDIF                                                                   !CMW
                                                                            !CMW
    ! Get count of existing deals                                           !CMW
    DEAL.COUNT% =                                                       \   !CMW
        LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) / ITMDL.DEALNUM.LID.LEN%           !CMW
                                                                            !CMW
    ! Initialise existing deal number                                       !CMW
    EXISTING.DEAL.NUMBER$ = STRING$(2,CHR$(0))                              !CMW
                                                                            !CMW
    ! Initialise loop counter                                               !CMW
    LOOP% = 0                                                               !CMW
                                                                            !CMW
    ! WHILE the end of the current deals list has NOT been reached AND      !CMW
    !       the deal has NOT been found in the current deals list AND       !CMW
    !       the current deals list already contained some deals             !CMW
    WHILE (NOT END.OF.DEAL.LIST) AND                                    \   !CMW
          (NOT DEAL.FOUND) AND                                          \   !CMW
          (DEAL.COUNT% > 0)                                                 !CMW
                                                                            !CMW
        DEAL.POS% =                                                     \   !CMW
            (LOOP% * ITMDL.DEALNUM.LID.LEN%) + 1 ! Deal index position      !CMW
                                                                            !CMW
        ! Get the next existing deal number                                 !CMW
        CALL EXTRACTS(ITMDL.ALL.DEAL.NUM.LIST.ID$,                      \   !CMW
                      EXISTING.DEAL.NUMBER$,                            \   !CMW
                      DEAL.POS%)                                            !CMW
                                                                            !CMW
        ! IF match found for the specified deal number                      !CMW
        IF DEAL.NUMBER$ = EXISTING.DEAL.NUMBER$ THEN BEGIN                  !CMW
            DEAL.FOUND = TRUE                                               !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
        LOOP% = LOOP% + 1                                                   !CMW
                                                                            !CMW
        ! IF end of the current deals list has been reached                 !CMW
        IF LOOP% = DEAL.COUNT% THEN BEGIN                                   !CMW
            END.OF.DEAL.LIST = TRUE                                         !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
    WEND                                                                    !CMW
                                                                            !CMW
    ! IF the specified deal exists in the current deals list                !CMW
    IF DEAL.FOUND THEN BEGIN                                                !CMW
                                                                            !CMW
        ! Calculate length of leftmost section of deal list to retain       !CMW
        LEFT.LEN%  = DEAL.POS% - 1                                          !CMW
                                                                            !CMW
        ! Calculate length of rightmost section of deal list to retain      !CMW
        RIGHT.LEN% = LEN(ITMDL.ALL.DEAL.NUM.LIST.ID$) -                 \   !CMW
                     LEFT.LEN% -                                        \   !CMW
                     ITMDL.DEALNUM.LID.LEN%                                 !CMW
                                                                            !CMW
        ! Remove specified deal and list ID from the current deals list     !CMW
        ITMDL.ALL.DEAL.NUM.LIST.ID$ =                                   \   !CMW
            LEFT$(ITMDL.ALL.DEAL.NUM.LIST.ID$,LEFT.LEN%) +              \   !CMW
            RIGHT$(ITMDL.ALL.DEAL.NUM.LIST.ID$,RIGHT.LEN%)                  !CMW
                                                                            !CMW
        ! Update ITMDL record                                               !CMW
        ITMDL.RC% = WRITE.ITMDL                                             !CMW
                                                                            !CMW
        ! IF error writing to ITMDL                                         !CMW
        IF ITMDL.RC% <> 0 THEN BEGIN                                        !CMW
            ITMDL.REMOVE.DEAL.ITEM = 1 ! Failure                            !CMW
        ENDIF                                                               !CMW
                                                                            !CMW
    ENDIF                                                                   !CMW
                                                                            !CMW
EXIT.FUNCTION:                                                              !CMW
                                                                            !CMW
    EXIT FUNCTION                                                           !CMW
                                                                            !CMW
ITMDL.REMOVE.DEAL.ITEM.ERROR:                                               !CMW
                                                                            !CMW
    ITMDL.REMOVE.DEAL.ITEM = 1 ! Failure                                    !CMW
    RESUME EXIT.FUNCTION                                                    !CMW
                                                                            !CMW
END FUNCTION                                                                !CMW

\***************************************************************************!JMW
\***                                                                        !JMW
\***   OPEN.ITMDL                                                           !JMW
\***                                                                        !JMW
\***   Opens the ITMDL file                                                 !JMW
\***                                                                        !JMW
\***************************************************************************!JMW
                                                                            !JMW
FUNCTION OPEN.ITMDL PUBLIC                                                  !JMW
                                                                            !JMW
    INTEGER*1 OPEN.ITMDL                                                    !JMW
                                                                            !JMW
    ON ERROR GOTO OPEN.ITMDL.ERROR                                          !JMW
                                                                            !JMW
    OPEN.ITMDL = 0                                                          !JMW
                                                                            !JMW
    OPEN ITMDL.FILE.NAME$ KEYED RECL ITMDL.RECL%                        \   !JMW
        AS ITMDL.SESS.NUM% NODEL                                            !JMW
                                                                            !JMW
EXIT.FUNCTION:                                                              !JMW
                                                                            !JMW
    EXIT FUNCTION                                                           !JMW
                                                                            !JMW
OPEN.ITMDL.ERROR:                                                           !JMW
                                                                            !JMW
    OPEN.ITMDL = 1                                                          !JMW
                                                                            !JMW
    FILE.OPERATION$ = "O"                                                   !JMW
    CURRENT.REPORT.NUM% = ITMDL.REPORT.NUM%                                 !JMW
                                                                            !JMW
    RESUME EXIT.FUNCTION                                                    !JMW
                                                                            !JMW
END FUNCTION                                                                !JMW


