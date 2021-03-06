\******************************************************************************
\******************************************************************************
\***
\***   FUNCTION        :       PSBF30
\***   NAME            :       PROCESS.KEYED.FILE
\***   AUTHOR          :       Mark Walker
\***   DATE            :       31st May 1995
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***
\***   Overview
\***   --------
\***
\***   This function processes all the records in a keyed file. It uses the
\***   concept of 'user exits' to allow user-defined processing to occur
\***   on individual records. It is also possible to get the function to
\***   update records as it's going along. Good eh!
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***
\***   Version 96A             Mark Walker                     31st May 1995
\***   Original version.
\***
\***   Version B               Andrew Wedgeworth              17th March 1997
\***   Altered to make opening the file READONLY optional.
\***
\***   Version C               Julia Stones                   12th May 2005
\***   Changed BLOCK.NUM% and FULL.BLOCKS% from integer*2 to integer*4 variables
\***
\******************************************************************************
\******************************************************************************

       STRING GLOBAL           BATCH.SCREEN.FLAG$,                     \
                               MODULE.NUMBER$,                         \
                               OPERATOR.NUMBER$

       %INCLUDE PSBF20G.J86

       %INCLUDE PSBF01E.J86
       %INCLUDE PSBF20E.J86
       %INCLUDE PSBF24E.J86

\******************************************************************************
\***
\***   FUNCTION        :       PROCESS.KEYED.RECORD$
\***
\******************************************************************************
\***
\***   The main body of this 'user exit' function must be written by the
\***   user and included in their application, as a PUBLIC function.
\***
\***   The function must return EITHER the record which was passed into
\***   it OR the modified record. It is important that any amendments to
\***   the record DO NOT cause the size of the record to change, otherwise
\***   the function will end with an error.
\***
\******************************************************************************

       FUNCTION PROCESS.KEYED.RECORD$(RECORD$) EXTERNAL

       STRING                  RECORD$,                                \
                               PROCESS.KEYED.RECORD$

       END FUNCTION

\******************************************************************************
\***
\***   FUNCTION        :       PROCESS.KEYED.FILE
\***
\******************************************************************************
\***
\***   Processes ALL records in a specified keyed file. It calls the function
\***   PROCESS.KEYED.RECORD$, after each record, to allow optional processing
\***   OR updating of individual records to be done.
\***
\***   Possible return codes:
\***
\***   0  = successful
\***   1  = file error
\***   2  = function error
\***   99 = fatal error
\***
\******************************************************************************

       FUNCTION PROCESS.KEYED.FILE(FILE.NAME$,                         \
                                   REPORT.NUM%,                        \
                                   READONLY$ ) PUBLIC                  ! BAW

       STRING                  FILE.NAME$,                             \
                               FILE.OPERATION$,                        \
                               FILE.NO$,                               \
                               VAR.STRING.1$,                          \
                               VAR.STRING.2$,                          \
                               FORMAT$,                                \
                               BLOCK$,                                 \
                               RECORD$,                                \
                               OLD.RECORD$,                            \
                               NEW.RECORD$,                            \
                               SECTOR$,                                \
                               KEY$,                                   \
                               FILE.NAME$,                             \
                               CURRENT.FILE.NAME$,                     \
                               KFCB$,                                  \
                               READONLY$                               ! BAW

       INTEGER*1               EVENT.NO%,                              \
                               TRUE,                                   \
                               FALSE,                                  \
                               EMPTY.RECORD.FOUND,                     \
                               END.OF.FILE,                            \
                               END.OF.BLOCK,                           \
                               BLOCK.UPDATED

       INTEGER*2               SESS.NUM%,                              \
                               REPORT.NUM%,                            \
                               MESSAGE.NO%,                            \
                               BLOCK.SIZE%,                            \
                               SECTOR.SIZE%,                           \
                               REMAINING.BYTES%,                       \
                               RECS.PER.SECTOR%,                       \
                               SECTOR.NUM%,                            \
                               RECORD.COUNT%,                          \
                               BASE%,                                  \
                               RECORD.INDEX%,                          \
                               LEFT.LEN%,                              \
                               RIGHT.LEN%,                             \
                               RECORD.LENGTH%,                         \
                               KEY.LENGTH%,                            \
                               CURRENT.REPORT.NUM%,                    \
                               RETURN.CODE%,                           \
                               PROCESS.KEYED.FILE

       INTEGER*4               FILE.SIZE%,                             \ CJAS
                               FULL.BLOCKS%,                           \ CJAS
                               BLOCK.NUM%                              ! CJAS

\******************************************************************************
\******************************************************************************
\***
\***    S T A R T  O F  M A I N  F U N C T I O N
\***
\******************************************************************************
\******************************************************************************

       ON ERROR GOTO ERROR.DETECTED

       RETURN.CODE% = 0

       IF SESS.NUM.UTILITY("O",REPORT.NUM%,FILE.NAME$) = 0 THEN BEGIN

          SESS.NUM% = F20.INTEGER.FILE.NO%

       ENDIF ELSE BEGIN

          RETURN.CODE% = 2
          GOTO END.PSBF30

       ENDIF

       TRUE = -1
       FALSE = 0

       BLOCK.SIZE%      = 32256
       SECTOR.SIZE%     = 512
       FILE.SIZE%       = SIZE(FILE.NAME$)
       FULL.BLOCKS%     = INT%(FILE.SIZE% / BLOCK.SIZE%)
       REMAINING.BYTES% = MOD(FILE.SIZE%,BLOCK.SIZE%)

       IF FULL.BLOCKS% = 0 AND                                         \
          REMAINING.BYTES% > 0 THEN BEGIN

          BLOCK.SIZE% = REMAINING.BYTES%

       ENDIF

       IF READONLY$ = "N" THEN BEGIN                                   ! BAW

          IF END # SESS.NUM% THEN OPEN.ERROR
          OPEN FILE.NAME$ DIRECT RECL BLOCK.SIZE% AS                   \ BAW
               SESS.NUM% BUFFSIZE BLOCK.SIZE%                          ! BAW

       ENDIF ELSE BEGIN                                                ! BAW

          IF END # SESS.NUM% THEN OPEN.ERROR
          OPEN FILE.NAME$ DIRECT RECL BLOCK.SIZE% AS                   \
               SESS.NUM% BUFFSIZE BLOCK.SIZE% READONLY

       ENDIF                                                           ! BAW

       FORMAT$ = "C" + STR$(BLOCK.SIZE%)

       BLOCK.NUM% = 1

       IF END # SESS.NUM% THEN READ.ERROR
       READ FORM FORMAT$; # SESS.NUM%, BLOCK.NUM%; BLOCK$

       END.OF.FILE = FALSE

       WHILE NOT END.OF.FILE
             GOSUB PROCESS.BLOCK
       WEND

       CLOSE SESS.NUM%

       CALL SESS.NUM.UTILITY("C",SESS.NUM%,"")

END.PSBF30:

       PROCESS.KEYED.FILE = RETURN.CODE%

       EXIT FUNCTION

\******************************************************************************
\******************************************************************************
\***
\***    E N D  O F  M A I N  F U N C T I O N
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***
\***    S T A R T  O F  S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***    SUBROUTINE      :       PROCESS.BLOCK
\***
\******************************************************************************
\***
\***   Processes a block of sectors
\***
\******************************************************************************

PROCESS.BLOCK:

       SECTOR.NUM% = 0

       END.OF.BLOCK  = FALSE
       BLOCK.UPDATED = FALSE

       WHILE NOT END.OF.BLOCK

             GOSUB PROCESS.SECTOR

       WEND

       IF BLOCK.UPDATED THEN BEGIN

          GOSUB UPDATE.BLOCK

       ENDIF

       IF BLOCK.NUM% = FULL.BLOCKS% AND                                \
          REMAINING.BYTES% > 0 THEN BEGIN

          CLOSE SESS.NUM%

          BLOCK.NUM% = (FILE.SIZE% / SECTOR.SIZE%) -                   \
                       (REMAINING.BYTES% / SECTOR.SIZE%)

          BLOCK.SIZE% = SECTOR.SIZE%
          FORMAT$     = "C" + STR$(BLOCK.SIZE%)

          IF READONLY$ = "N" THEN BEGIN                                ! BAW

             IF END # SESS.NUM% THEN OPEN.ERROR
             OPEN FILE.NAME$ DIRECT RECL BLOCK.SIZE% AS                \ BAW
                  SESS.NUM% BUFFSIZE BLOCK.SIZE%                       ! BAW

          ENDIF ELSE BEGIN                                             ! BAW

             IF END # SESS.NUM% THEN OPEN.ERROR
             OPEN FILE.NAME$ DIRECT RECL BLOCK.SIZE% AS                \
                  SESS.NUM% BUFFSIZE BLOCK.SIZE% READONLY

          ENDIF                                                        ! BAW

       ENDIF

       BLOCK.NUM% = BLOCK.NUM% + 1

       IF END # SESS.NUM% THEN END.OF.FOUND
       READ FORM FORMAT$; # SESS.NUM%, BLOCK.NUM%; BLOCK$

       GOTO END.PROCESS.BLOCK

END.OF.FOUND:

       END.OF.FILE = TRUE

END.PROCESS.BLOCK:

       RETURN

\******************************************************************************
\***
\***   SUBROUTINE      :       PROCESS.SECTOR
\***
\******************************************************************************
\***
\***   Processes a block of records
\***
\******************************************************************************

PROCESS.SECTOR:

       IF SECTOR.NUM% = 0 AND                                          \
          BLOCK.NUM% = 1 THEN BEGIN

          KFCB$ = MID$(BLOCK$,1,SECTOR.SIZE%)

          RECORD.LENGTH% = ASC(MID$(KFCB$,47,1)) +                     \
                           ASC(MID$(KFCB$,48,1)) * 256

          KEY.LENGTH%    = ASC(MID$(KFCB$,55,1)) +                     \
                           ASC(MID$(KFCB$,56,1)) * 256

          RECS.PER.SECTOR% = SECTOR.SIZE% / RECORD.LENGTH%

          SECTOR.NUM% = 1

       ENDIF

       SECTOR$ = MID$(BLOCK$,                                          \
                 (SECTOR.NUM% * SECTOR.SIZE%) + 1,                     \
                 SECTOR.SIZE%)

       RECORD.COUNT% = 0

       EMPTY.RECORD.FOUND = FALSE

       WHILE RECORD.COUNT% < RECS.PER.SECTOR% AND                      \
             NOT EMPTY.RECORD.FOUND

             BASE% = (RECORD.COUNT% * RECORD.LENGTH%) + 5

             RECORD$ = MID$(SECTOR$,BASE%,RECORD.LENGTH%)

             KEY$ = MID$(RECORD$,1,KEY.LENGTH%)

             IF KEY$ = STRING$(KEY.LENGTH%,CHR$(0)) THEN BEGIN

                EMPTY.RECORD.FOUND = TRUE
                GOTO NEXT.SECTOR

             ENDIF

             GOSUB PROCESS.RECORD

             RECORD.COUNT% = RECORD.COUNT% + 1

NEXT.SECTOR:

       WEND

       SECTOR.NUM% = SECTOR.NUM% + 1

       IF SECTOR.NUM% = (BLOCK.SIZE% / SECTOR.SIZE%) THEN BEGIN

          END.OF.BLOCK = TRUE

       ENDIF

       RETURN

\******************************************************************************
\***
\***   SUBROUTINE      :       PROCESS.RECORD
\***
\******************************************************************************
\***
\***   Processes an individual record
\***
\******************************************************************************

PROCESS.RECORD:

       OLD.RECORD$ = RECORD$
       NEW.RECORD$ = PROCESS.KEYED.RECORD$(RECORD$)

       RECORD.INDEX% = (SECTOR.NUM% * SECTOR.SIZE%) + BASE%

       IF OLD.RECORD$ <> NEW.RECORD$ THEN BEGIN

          IF LEN(OLD.RECORD$) <> LEN(NEW.RECORD$) THEN BEGIN

             RETURN.CODE% = 2
             GOTO END.PSBF30

          ENDIF

          GOSUB UPDATE.RECORD

       ENDIF

       RETURN

\******************************************************************************
\***
\***   SUBROUTINE      :       UPDATE.RECORD
\***
\******************************************************************************
\***
\***   Inserts an amended record into a block
\***
\******************************************************************************

UPDATE.RECORD:

       LEFT.LEN%  = RECORD.INDEX% - 1
       RIGHT.LEN% = BLOCK.SIZE% - LEFT.LEN% - RECORD.LENGTH%

       BLOCK$ = LEFT$(BLOCK$,LEFT.LEN%) +                              \
                NEW.RECORD$ +                                          \
                RIGHT$(BLOCK$,RIGHT.LEN%)

       BLOCK.UPDATED = TRUE

       RETURN

\******************************************************************************
\***
\***   SUBROUTINE      :       UPDATE.BLOCK
\***
\******************************************************************************
\***
\***
\***
\******************************************************************************

UPDATE.BLOCK:

       IF END # SESS.NUM% THEN WRITE.ERROR
       WRITE FORM FORMAT$; # SESS.NUM%, BLOCK.NUM%; BLOCK$

       RETURN

\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    E N D  O F  S U B R O U T I N E S                                     *
\***                                                                          *
\******************************************************************************
\******************************************************************************


\******************************************************************************
\******************************************************************************
\***                                                                          *
\***    S T A R T  O F  E R R O R  R O U T I N E S                            *
\***                                                                          *
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   ERROR ROUTINE   :       FILE.ERROR
\***
\******************************************************************************

FILE.ERROR:

       EVENT.NO%   = 106
       MESSAGE.NO% = 0

       FILE.NO$ = CHR$(SHIFT(REPORT.NUM%,8)) +                         \
                  CHR$(SHIFT(REPORT.NUM%,0))

       VAR.STRING.1$ = FILE.OPERATION$ +                               \
                       FILE.NO$ +                                      \
                       PACK$(STRING$(14,"0"))

       VAR.STRING.2$ = ""

       CALL APPLICATION.LOG(MESSAGE.NO%,                               \
                            VAR.STRING.1$,                             \
                            VAR.STRING.2$,                             \
                            EVENT.NO%)

       RETURN

\******************************************************************************
\***
\***   ERROR ROUTINE   :       OPEN.ERROR
\***
\******************************************************************************

OPEN.ERROR:

       FILE.OPERATION$ = "O"
       GOSUB FILE.ERROR

       RETURN.CODE% = 1
       GOTO END.PSBF30

\******************************************************************************
\***
\***   ERROR ROUTINE   :       READ.ERROR
\***
\******************************************************************************

READ.ERROR:

       FILE.OPERATION$ = "R"
       GOSUB FILE.ERROR

       RETURN.CODE% = 1
       GOTO END.PSBF30

\******************************************************************************
\***
\***   ERROR ROUTINE   :       WRITE.ERROR
\***
\******************************************************************************

WRITE.ERROR:

       FILE.OPERATION$ = "W"
       GOSUB FILE.ERROR

       RETURN.CODE% = 1
       GOTO END.PSBF30

\******************************************************************************
\***
\***   ERROR ROUTINE   :       ERROR.DETECTED
\***
\******************************************************************************

ERROR.DETECTED:

       CALL STANDARD.ERROR.DETECTED(ERRN,ERRF%,ERRL,ERR)

       RETURN.CODE% = 99
       RESUME END.PSBF30

       END FUNCTION

