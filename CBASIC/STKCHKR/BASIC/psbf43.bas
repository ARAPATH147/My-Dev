!******************************************************************************
!******************************************************************************
!***
!***            PROGRAM         :       PSBF43.BAS
!***
!***            DESCRIPTION     :       Process Merge Files
!***
!***            AUTHOR          :       Julia Stones
!***            DATE WRITTEN    :       June 2005
!***
!***        This function will merge two files and is used by PSB30 and PSB34.
!***        Pass in the keyed file to be read sequentially by PSBF30
!***        Pass in the keyed file to be key read
!***        Pass in the ADD or SUBTRACT parameter.
!***        The function then calls PROCESS.KEYED.RECORD (PSBF30)
!***        Using PSBF30 this function will process one file sequentially
!***        and then open the other file keyed.
!***        Where the record does not exist on the keyed file then a new
!***        a new record will be added.
!***        Where the record exists on the keyed file then certain fields
!***        will be updated.
!***        The first file passed will be the file to opened for sequential
!***        processing, the second file passed will be opened keyed for update
!***        The third parameter will either be a plus or minus sign (+ or -)
!***
!******************************************************************************
!***
!***    Version B           Mark Walker                 12th Feb 2014
!***    F337 Centralised View of Stock
!***    - Added references to the new sequence ID field.
!***    - Added separate includes for BIMSTC, CIMSTC and BIMSTC files.
!***
!******************************************************************************

%INCLUDE PSBF43G.J86

!******************************************************************************

STRING PROGRAM.NAME$
STRING SIGNED.FLAG$

INTEGER*1 ERROR.FLAG
INTEGER*2 RC%

%INCLUDE IMSTCDEC.J86   ! IMSTC FUNCTIONS                                       !BMW
%INCLUDE CIMSTDEC.J86   ! CIMSTC FUNCTIONS                                      !BMW
%INCLUDE MIMSTDEC.J86   ! MIMSTC FUNCTIONS                                      !BMW

!******************************************************************************

%INCLUDE IMSTCEXT.J86   ! IMSTC EXTERNALS                                       !BMW
%INCLUDE CIMSTEXT.J86   ! CIMSTC EXTERNALS                                      !BMW
%INCLUDE MIMSTEXT.J86   ! MIMSTC EXTERNALS                                      !BMW

!******************************************************************************

%INCLUDE PSBF30E.J86    !PROCESS KEYED FILE EXTERNALS
%INCLUDE PSGASMRT.J86   !GETN2 AND GETN4 EXTERNALS

!******************************************************************************

\******************************************************************************
\***                                                                          *
\***   FUNCTION : PROCESS.KEYED.RECORD$                                       *
\***                                                                          *
\******************************************************************************
\***                                                                          *
\***  Processes each record on the keyed file passed into the PSBF30 seq file *
\***  processing function.  The file being processed will be either the       *
\***  IMSTC, MIMSTC or CIMSTC file                                            *
\***  The record passed into this function will be stored in temporary        *
\***  variables.                                                              *
\***  Each record will then be checked in the MERGE FILE subroutine to see if *
\***  the record needs to be added or merged to the file being processed in   *
\***  keyed file mode.                                                        *
\***                                                                          *
\******************************************************************************

FUNCTION PROCESS.KEYED.RECORD$(RECORD$) PUBLIC

    STRING                                                              \
        RECORD$,                                                        \
        PROCESS.KEYED.RECORD$,                                          \
        SEQ.BAR.CODE$,                                                  \
        SEQ.STATUS.FLAG$,                                               \
        SEQ.REASON.ITEM.REMOVED$,                                       \
        SEQ.FILLER$

    INTEGER*4                                                           \
        SEQ.RESTART%,                                                   \
        SEQ.NUMITEMS%,                                                  \
        SEQ.AMTSALE%,                                                   \
        SEQ.RESERVED%,                                                  \
        SEQ.STKMQ.RESTART%,                                             \
        SEQ.SID%                                                            !BMW

    INTEGER*2                                                           \
        SEQ.STOCK.FIGURE%

    SEQ.BAR.CODE$            = LEFT$(RECORD$,11)
    SEQ.RESTART%             = GETN4(RECORD$,11)
    SEQ.NUMITEMS%            = GETN4(RECORD$,15)
    SEQ.AMTSALE%             = GETN4(RECORD$,19)
    SEQ.RESERVED%            = GETN4(RECORD$,23)
    SEQ.STKMQ.RESTART%       = GETN4(RECORD$,27)
    SEQ.STATUS.FLAG$         = MID$(RECORD$,32,1)
    SEQ.STOCK.FIGURE%        = GETN2(RECORD$,32)
    SEQ.REASON.ITEM.REMOVED$ = MID$(RECORD$,35,1)
    SEQ.SID%                 = GETN4(RECORD$,35)                            !BMW
!!!!SEQ.FILLER$              = RIGHT$(RECORD$,5)                            !BMW
    SEQ.FILLER$              = RIGHT$(RECORD$,1)                            !BMW

    GOSUB MERGE.RECORD

    PROCESS.KEYED.RECORD$ = RECORD$

EXIT FUNCTION

\*********************************************************************
\***
\***   MERGE.RECORD
\***
\***   The program (PSB30 or PSB34) that called this function will have
\***   have opened the files to be processed as keyed before calling this
\***   function.  PSBF30 will have opened the keyed file to be processed
\***   sequentially as part of it's processing.
\***
\***   MERGE.RECORD - Checks if the record being processed exists on the
\***   file being processed as a keyed file.
\***   If the program calling this function is PSB34 then the MIMSTC
\***   will have been opened as keyed by PSB34, PSBF30 will be processing
\***   the keyed file CIMSTC sequentially.
\***   If the CIMSTC record does not exist on the MIMSTC then the record
\***   is added.  If the record was found then the two records are merged
\***   onto the MIMSTC.
\***   If the program calling this function is PSB30 and the + sign is
\***   passed, then the IMSTC will have been opened as keyed by PSB30,
\***   PSBF30 will be processing the keyed file MIMSTC sequentially.
\***   If the MIMSTC record does not exist on the IMSTC then the record
\***   is added.  If the record was found then the two records are merged
\***   onto the IMSTC.
\***   If the program calling this function is PSB30 and the - sign is
\***   passed, then the CIMSTC is opened keyed, the MIMSTC is created
\***   and opened keyed by PSB30, PSBF30 will be processing the keyed
\***   file IMSTC sequentially.
\***   If the IMSTC record does not exist on the CIMSTC then the IMSTC
\***   record is written to the MIMSTC.  If the record was found then
\***   the CIMSTC and IMSTC records are merged and written out to the
\***   MIMSTC.
\***
\***
\*********************************************************************

MERGE.RECORD:

    IF PROGRAM.NAME$ = "PSB34"  THEN BEGIN
       MIMSTC.BAR.CODE$ = SEQ.BAR.CODE$
       RC% = READ.MIMSTC
       IF RC% NE 0 THEN BEGIN
          GOSUB ADD.RECORD.TO.MIMSTC
       ENDIF ELSE BEGIN
          GOSUB MERGE.RECORD.TO.MIMSTC
       ENDIF
    ENDIF ELSE BEGIN
       IF PROGRAM.NAME$ = "PSB30" THEN BEGIN
          IF SIGNED.FLAG$ = "+" THEN BEGIN
             IMSTC.BAR.CODE$ = SEQ.BAR.CODE$
             RC% = READ.IMSTC
             IF RC% NE 0 THEN BEGIN
                GOSUB ADD.RECORD.TO.IMSTC
             ENDIF ELSE BEGIN
                GOSUB MERGE.RECORD.TO.IMSTC
             ENDIF
          ENDIF ELSE BEGIN
             CIMSTC.BAR.CODE$ = SEQ.BAR.CODE$
             MIMSTC.BAR.CODE$ = SEQ.BAR.CODE$
             RC% = READ.CIMSTC
             IF RC% NE 0 THEN BEGIN
                GOSUB ADD.RECORD.TO.MIMSTC
             ENDIF ELSE BEGIN
                GOSUB MERGE.RECORD.TO.MIMSTC
             ENDIF
          ENDIF
       ENDIF
    ENDIF

RETURN

\*******************************************************************
\***
\***  ADD.RECORD.TO MIMSTC:
\***
\***  This processing will write a record to the MIMSTC
\***
\*******************************************************************

ADD.RECORD.TO.MIMSTC:

   MIMSTC.RESTART%             = SEQ.RESTART%
   MIMSTC.NUMITEMS%            = SEQ.NUMITEMS%
   MIMSTC.AMTSALE%             = SEQ.AMTSALE%
   MIMSTC.RESERVED%            = SEQ.RESERVED%
   MIMSTC.STKMQ.RESTART%       = SEQ.STKMQ.RESTART%
   MIMSTC.STATUS.FLAG$         = SEQ.STATUS.FLAG$
   MIMSTC.STOCK.FIGURE%        = SEQ.STOCK.FIGURE%
   MIMSTC.REASON.ITEM.REMOVED$ = SEQ.REASON.ITEM.REMOVED$
   MIMSTC.SID%                 = SEQ.SID%                                   !BMW
   MIMSTC.FILLER$              = SEQ.FILLER$

   RC% = WRITE.MIMSTC

   IF RC% NE 0 THEN BEGIN
      ERROR.FLAG = 2
      GOTO PSBF43.ERROR
   ENDIF

RETURN

\*******************************************************************
\***
\***  ADD.RECORD.TO IMSTC:
\***
\***  This processing will write a record to the IMSTC
\***
\*******************************************************************


ADD.RECORD.TO.IMSTC:

   IMSTC.RESTART%             = SEQ.RESTART%
   IMSTC.NUMITEMS%            = SEQ.NUMITEMS%
   IMSTC.AMTSALE%             = SEQ.AMTSALE%
   IMSTC.RESERVED%            = SEQ.RESERVED%
   IMSTC.STKMQ.RESTART%       = SEQ.STKMQ.RESTART%
   IMSTC.STATUS.FLAG$         = SEQ.STATUS.FLAG$
   IMSTC.STOCK.FIGURE%        = SEQ.STOCK.FIGURE%
   IMSTC.REASON.ITEM.REMOVED$ = SEQ.REASON.ITEM.REMOVED$
   IMSTC.SID%                 = SEQ.SID%                                    !BMW
   IMSTC.FILLER$              = SEQ.FILLER$

   RC% = WRITE.IMSTC

RETURN

\*******************************************************************
\***
\***  MERGE.RECORD.TO MIMSTC:
\***
\***  The MIMSTC record will be updated as follows.
\***
\***  If the program calling this function was PSB34 then the number
\***  of items and amount of sales from the file being processed
\***  sequentially will be added to the values from the file being
\***  processed as keyed.  The stock figure, reason item removed,
\***  and the status flag will be set to the value from the file
\***  being processed sequentially. All the other fields will be
\***  set to the value from the file being processed as keyed.
\***
\***  If the program calling this function is PSB30 then the number
\***  of items and amount of sales from file being processed as
\***  keyed will be taken away from the file being processed
\***  sequentially.  All the other fields will be set to the value
\***  from the file being processed sequentially.
\***
\*******************************************************************


MERGE.RECORD.TO.MIMSTC:

   IF PROGRAM.NAME$ = "PSB34" THEN BEGIN
      MIMSTC.NUMITEMS%     = (SEQ.NUMITEMS% + MIMSTC.NUMITEMS%)
      MIMSTC.AMTSALE%      = (SEQ.AMTSALE% + MIMSTC.AMTSALE%)
      MIMSTC.STOCK.FIGURE% = SEQ.STOCK.FIGURE%
      MIMSTC.SID%          = SEQ.SID%                                       !BMW

      IF MIMSTC.REASON.ITEM.REMOVED$ NE SEQ.REASON.ITEM.REMOVED$ THEN BEGIN
         IF SEQ.REASON.ITEM.REMOVED$ NE " " THEN BEGIN
            MIMSTC.REASON.ITEM.REMOVED$ = SEQ.REASON.ITEM.REMOVED$
         ENDIF
      ENDIF

      IF MIMSTC.STATUS.FLAG$ NE SEQ.STATUS.FLAG$ THEN BEGIN
         IF SEQ.STATUS.FLAG$ = CHR$(192) THEN BEGIN
            MIMSTC.STATUS.FLAG$ = SEQ.STATUS.FLAG$
         ENDIF
      ENDIF

      RC% = WRITE.MIMSTC

   ENDIF ELSE BEGIN
      MIMSTC.BAR.CODE$            = SEQ.BAR.CODE$
      MIMSTC.RESTART%             = SEQ.RESTART%
      MIMSTC.NUMITEMS%            = (SEQ.NUMITEMS% - CIMSTC.NUMITEMS%)
      MIMSTC.AMTSALE%             = (SEQ.AMTSALE% - CIMSTC.AMTSALE%)
      MIMSTC.RESERVED%            = SEQ.RESERVED%
      MIMSTC.STKMQ.RESTART%       = SEQ.STKMQ.RESTART%
      MIMSTC.STATUS.FLAG$         = SEQ.STATUS.FLAG$
      MIMSTC.STOCK.FIGURE%        = SEQ.STOCK.FIGURE%
      MIMSTC.REASON.ITEM.REMOVED$ = SEQ.REASON.ITEM.REMOVED$
      MIMSTC.SID%                 = SEQ.SID%                                !BMW
      MIMSTC.FILLER$              = SEQ.FILLER$

      RC% = WRITE.MIMSTC

   ENDIF

RETURN

\*******************************************************************
\***
\***  MERG.RECORD.TO IMSTC:
\***
\***  The IMSTC record will be updated as follows.
\***
\***  The number of items and amount of sales from the file being
\***  processed sequentially will be added to the values from the
\***  file being processed as keyed.  The reason item removed,
\***  and the status flag will be set to the value from the file
\***  being processed sequentially. All the other fields will be
\***  set to the value from the file being processed as keyed.
\***
\*******************************************************************


MERGE.RECORD.TO.IMSTC:

      IMSTC.NUMITEMS%     = (SEQ.NUMITEMS% + IMSTC.NUMITEMS%)
      IMSTC.AMTSALE%      = (SEQ.AMTSALE% + IMSTC.AMTSALE%)

      IF IMSTC.REASON.ITEM.REMOVED$ NE SEQ.REASON.ITEM.REMOVED$ THEN BEGIN
         IF SEQ.REASON.ITEM.REMOVED$ NE " " THEN BEGIN
            IMSTC.REASON.ITEM.REMOVED$ = SEQ.REASON.ITEM.REMOVED$
         ENDIF
      ENDIF

      IF IMSTC.STATUS.FLAG$ NE SEQ.STATUS.FLAG$ THEN BEGIN
         IF SEQ.STATUS.FLAG$ = CHR$(192) THEN BEGIN
            IMSTC.STATUS.FLAG$ = SEQ.STATUS.FLAG$
         ENDIF
      ENDIF

      RC% = WRITE.IMSTC

RETURN

PSBF43.ERROR:

END FUNCTION

\**********************************************************************
\***
\***  PROCESS.MERGE.FILES
\***
\***  The calling program will pass the program name (PSB30 or PSB34)
\***  and a sign flag (+ or -)
\***  The function will then pass the relevant keyed file to the
\***  PROCESS.KEYED.FILE function (PSBF30)so that it can be processed
\***  sequentially.
\***
\**********************************************************************

FUNCTION PROCESS.MERGE.FILES(PROG.NAME$, SIGN.FLAG$) PUBLIC

    STRING PROG.NAME$
    STRING SIGN.FLAG$

    INTEGER*2 RC%

    INTEGER*1 PROCESS.MERGE.FILES

    ERROR.FLAG = 0

    PROGRAM.NAME$ = PROG.NAME$
    SIGNED.FLAG$ = SIGN.FLAG$

    IF PROG.NAME$ = "PSB34" THEN BEGIN
       RC% = PROCESS.KEYED.FILE(CIMSTC.FILE.NAME$,             \ Sequential process of keyed CIMSTC file
                                CIMSTC.REPORT.NUM%,            \
                                "N")
    ENDIF

    IF PROG.NAME$ = "PSB30" THEN BEGIN
       IF SIGN.FLAG$ = "-" THEN BEGIN
          RC% = PROCESS.KEYED.FILE(IMSTC.FILE.NAME$,           \ Sequential process of keyed IMSTC file
                                   IMSTC.REPORT.NUM%,          \
                                   "N")
       ENDIF ELSE BEGIN
          RC% = PROCESS.KEYED.FILE(MIMSTC.FILE.NAME$,          \ Sequential process of keyed MIMSTC file
                                   MIMSTC.REPORT.NUM%,         \
                                   "N")
       ENDIF
    ENDIF

    IF RC% <> 0 THEN BEGIN                          ! Log Non-zero return code from ext func
       IF RC% <> 2 THEN BEGIN
          ERROR.FLAG = 1
       ENDIF ELSE BEGIN
          ERROR.FLAG = 2
       ENDIF
    ENDIF

   PROCESS.MERGE.FILES = ERROR.FLAG

END FUNCTION


