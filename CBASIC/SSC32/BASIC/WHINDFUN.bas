\***********************************************************************
\***********************************************************************
\***
\***    DESCRIPTION: Dallas UOD Index file (WHINDX)
\***                 Public File Function Definitions
\***
\***    FILE TYPE : Direct
\***
\***********************************************************************
\***
\***    Version A.          Arun Venugopalan              9th April 2015
\***    Initial version.
\***   
\***    Version B           Kiran Krishnan                14th July 2015
\***    Corrected the File operation value in WRITE.WHINDX
\***
\***********************************************************************
\***********************************************************************

    %INCLUDE WHINDDEC.J86             ! WHINDX variable declarations

    INTEGER*2 GLOBAL                  \
        CURRENT.REPORT.NUM%           ! Current report number

    STRING GLOBAL                     \
        CURRENT.CODE$,                \ Error code
        FILE.OPERATION$               ! File operation

\***********************************************************************
\*
\* WHINDX.SET:
\* Sets WHINDX file variables
\*
\***********************************************************************

FUNCTION WHINDX.SET PUBLIC

    WHINDX.FILE.FORM$  = "C14,C6,C1"
    WHINDX.FILE.NAME$  = "WHINDX"
    WHINDX.RECL%       = 21
    WHINDX.REPORT.NUM% = 891

END FUNCTION

\***********************************************************************
\*
\* READ.WHINDX:
\* Reads the record based on the record number and stores field values
\* into the variables.
\*
\***********************************************************************

FUNCTION READ.WHINDX PUBLIC

    INTEGER*1 READ.WHINDX

    READ.WHINDX = 1                   ! Error

    IF END # WHINDX.SESS.NUM% THEN READ.WHINDX.ERROR
    READ FORM WHINDX.FILE.FORM$; # WHINDX.SESS.NUM%, WHINDX.REC.NUM%;  \
              WHINDX.BARCODENO$,      \ Barcode number
              WHINDX.EXPCT.DELDATE$,  \ Expected Date of Delivery
              WHINDX.STATUS$          ! UOD Status

    READ.WHINDX = 0                   ! No error

    EXIT FUNCTION

READ.WHINDX.ERROR:

    FILE.OPERATION$     = "R"         ! To log Read Error
    CURRENT.REPORT.NUM% = WHINDX.REPORT.NUM%
    CURRENT.CODE$       = STR$(WHINDX.REC.NUM%)

END FUNCTION

\***********************************************************************
\*
\* WRITE.WHINDX:
\* Writes record to the file based on the values set to the
\* variables by the calling program.
\*
\***********************************************************************

FUNCTION WRITE.WHINDX PUBLIC

    INTEGER*1 WRITE.WHINDX

    WRITE.WHINDX = 1                    ! Error

    IF END # WHINDX.SESS.NUM% THEN WRITE.WHINDX.ERROR
    WRITE FORM WHINDX.FILE.FORM$; # WHINDX.SESS.NUM%, WHINDX.REC.NUM%; \
               WHINDX.BARCODENO$,       \ Bar code number
               WHINDX.EXPCT.DELDATE$,   \ Expected Date of Delivery
               WHINDX.STATUS$           ! UOD Status

    WRITE.WHINDX = 0                    ! No error

    EXIT FUNCTION

WRITE.WHINDX.ERROR:

    FILE.OPERATION$     = "W"           ! To log Read Error             !BKK
    CURRENT.REPORT.NUM% = WHINDX.REPORT.NUM%
    CURRENT.CODE$       = STR$(WHINDX.REC.NUM%)

END FUNCTION
