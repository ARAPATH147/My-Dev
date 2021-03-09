\******************************************************************************
\******************************************************************************
\***
\***         INCLUDE FOR RECALLS.BIN FUNCTIONS
\***
\***               REFERENCE    : RECALFUN.BAS
\***
\***    VERSION A               BRIAN GREENFIELD                    14th May 2007
\***    Initial Version
\***
\***    Version B               BRIAN GREENFIELD                    20th June 2007
\***    Altered due to a change in the file layout.
\***
\***    Version C               CHARLES SKADORWA                    26th June 2007
\***    Added DELETE.RECALLS function.
\***
\*******************************************************************************
\*******************************************************************************

  INTEGER*2 GLOBAL                   \
      CURRENT.REPORT.NUM%

  STRING GLOBAL                      \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

  STRING                             \
      RECALLS.ARRAY$

  %INCLUDE RECALDEC.J86

\--------------------------------------------------------------------
                                                                     
  SUB RECALLS.SPLIT.ARRAY

   INTEGER*2 I%

   FOR I% = 0 TO RECALLS.MAX.REC.ITEMS% -1

      RECALLS.ITEM.CODE$(I%)  =    MID$(RECALLS.ARRAY$, I%*8+1,3)
      RECALLS.ITEM.STOCK$(I%) =    MID$(RECALLS.ARRAY$, I%*8+4,2) !BBG
      RECALLS.SESS.STOCK$(I%) =    MID$(RECALLS.ARRAY$, I%*8+6,2) !BBG
      RECALLS.ITEM.UPT.FLAG$(I%) = MID$(RECALLS.ARRAY$, I%*8+8,1)
      
   NEXT I%

  END SUB

\--------------------------------------------------------------------

  SUB RECALLS.CONCAT.ARRAY

   INTEGER*2 I%

   RECALLS.ARRAY$ = ""

   FOR I% = 0 TO RECALLS.MAX.REC.ITEMS% -1

      RECALLS.ARRAY$ = RECALLS.ARRAY$ + \
                       RECALLS.ITEM.CODE$(I%)  + \ CCSk
                       RECALLS.ITEM.STOCK$(I%) + \
                       RECALLS.SESS.STOCK$(I%) + \ BBG
                       RECALLS.ITEM.UPT.FLAG$(I%)
      
   NEXT I%
   
   RECALLS.ARRAY$ = LEFT$(RECALLS.ARRAY$ + STRING$(RECALLS.MAX.REC.ITEMS%*8,CHR$(00)), RECALLS.MAX.REC.ITEMS%*8)

  END SUB

\--------------------------------------------------------------------

  FUNCTION RECALLS.SET PUBLIC
  
   INTEGER*2 RECALLS.SET

   RECALLS.REPORT.NUM%    = 745
   RECALLS.RECL%          = 508
   RECALLS.FILE.NAME$     = "RECALLS"
   RECALLS.MAX.REC.ITEMS% = 50
   RECALLS.KEYL%          = 9

   DIM RECALLS.ITEM.CODE$     (RECALLS.MAX.REC.ITEMS%-1)
   DIM RECALLS.ITEM.STOCK$    (RECALLS.MAX.REC.ITEMS%-1)
   DIM RECALLS.SESS.STOCK$    (RECALLS.MAX.REC.ITEMS%-1) !BBG
   DIM RECALLS.ITEM.UPT.FLAG$ (RECALLS.MAX.REC.ITEMS%-1)
   
   RECALLS.SET = 0
   
  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION READ.RECALLS PUBLIC

   INTEGER*2 READ.RECALLS
   STRING    KEY$

   READ.RECALLS = 1

   IF END #RECALLS.SESS.NUM% THEN READ.RECALLS.ERROR

   KEY$ = RECALLS.REFERENCE$ + CHR$(RECALLS.CHAIN%)

   READ FORM "T10,C1,C20,C8,C1,C1,C1,C8,C8,C8,C1,C30,C4,C400,C8"; \
          #RECALLS.SESS.NUM% KEY KEY$; \
             RECALLS.TYPE$,            \
             RECALLS.DESCRIPTION$,     \
             RECALLS.LABEL.TYPE$,      \
             RECALLS.SUPPLY.ROUTE$,    \
             RECALLS.REASON.CODE$,     \
             RECALLS.BC$,              \
             RECALLS.ACTIVE.DATE$,     \
             RECALLS.DUE.BY.DATE$,     \
             RECALLS.COMPLETION.DATE$, \
             RECALLS.STATUS$,          \
             RECALLS.BATCH.NUM$,       \
             RECALLS.ITEM.COUNT$,      \
             RECALLS.ARRAY$,           \
             RECALLS.FILLER$

   CALL RECALLS.SPLIT.ARRAY

   READ.RECALLS = 0
   EXIT FUNCTION

READ.RECALLS.ERROR:

   CURRENT.REPORT.NUM% = RECALLS.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = KEY$

  END FUNCTION

\--------------------------------------------------------------------

  FUNCTION WRITE.RECALLS PUBLIC

   INTEGER*2 WRITE.RECALLS
   STRING    KEY$

   WRITE.RECALLS = 1

   IF END #RECALLS.SESS.NUM% THEN WRITE.RECALLS.ERROR

   KEY$ = RECALLS.REFERENCE$ + CHR$(RECALLS.CHAIN%)
   CALL RECALLS.CONCAT.ARRAY
   
   RECALLS.FILLER$ = STRING$(19,CHR$(00))

   WRITE FORM "C9,C1,C20,C8,C1,C1,C1,C8,C8,C8,C1,C30,C4,C400,C8"; \
          #RECALLS.SESS.NUM%;          \
             KEY$,                     \
             RECALLS.TYPE$,            \
             RECALLS.DESCRIPTION$,     \
             RECALLS.LABEL.TYPE$,      \
             RECALLS.SUPPLY.ROUTE$,    \
             RECALLS.REASON.CODE$,     \
             RECALLS.BC$,              \
             RECALLS.ACTIVE.DATE$,     \
             RECALLS.DUE.BY.DATE$,     \
             RECALLS.COMPLETION.DATE$, \
             RECALLS.STATUS$,          \
             RECALLS.BATCH.NUM$,       \
             RECALLS.ITEM.COUNT$,      \
             RECALLS.ARRAY$,           \
             RECALLS.FILLER$

   WRITE.RECALLS = 0
   EXIT FUNCTION

WRITE.RECALLS.ERROR:

   CURRENT.REPORT.NUM% = RECALLS.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = KEY$

  END FUNCTION
  
  


\--------------------------------------------------------------------!CCSk
\ DELETE.RECALLS                                                     !CCSk
\                                                                    !CCSk
\    This function returns 4 different return codes                  !CCSk
\                                                                    !CCSk
\            0 - Record successfully deleted                         !CCSk
\            1 - Record Deletion error                               !CCSk
\            2 - Session number invalid ie. 0                        !CCSk
\            3 - Invalid Key Length                                  !CCSk
\                                                                    !CCSk
\--------------------------------------------------------------------!CCSk
  FUNCTION DELETE.RECALLS (KEY$) PUBLIC                              !CCSk
                                                                     !CCSk
   INTEGER*2 DELETE.RECALLS                                          !CCSk
   STRING    KEY$                                                    !CCSk
                                                                     !CCSk
   DELETE.RECALLS = 0                                                !CCSk
                                                                     !CCSk
   IF RECALLS.SESS.NUM% = 0 THEN BEGIN                               !CCSk
       DELETE.RECALLS = 2                                            !CCSk
       GOTO SET.RECALLS.DELETE.ERROR                                 !CCSk
   ENDIF                                                             !CCSk
                                                                     !CCSk
   IF LEN(KEY$) <> RECALLS.KEYL% THEN BEGIN                          !CCSk
       DELETE.RECALLS = 3                                            !CCSk
       GOTO SET.RECALLS.DELETE.ERROR                                 !CCSk
   ENDIF                                                             !CCSk
                                                                     !CCSk
   IF END #RECALLS.SESS.NUM% THEN DELETE.RECALLS.ERROR               !CCSk
                                                                     !CCSk
   DELREC RECALLS.SESS.NUM%; KEY$                                    !CCSk
                                                                     !CCSk
EXIT.FUNCTION:                                                       !CCSk
   EXIT FUNCTION                                                     !CCSk
                                                                     !CCSk
DELETE.RECALLS.ERROR:                                                !CCSk
                                                                     !CCSk
   DELETE.RECALLS = 1                                                !CCSk
                                                                     !CCSk
SET.RECALLS.DELETE.ERROR:                                            !CCSk
      FILE.OPERATION$     = "D"                                      !CCSk
      CURRENT.REPORT.NUM% = RECALLS.REPORT.NUM%                      !CCSk
      CURRENT.CODE$       = ""                                       !CCSk
                                                                     !CCSk
  END FUNCTION                                                       !CCSk
                                                                     
                                                                     
                                                                     
                                                                     
                                                                     
                                                                     
                                                                     
