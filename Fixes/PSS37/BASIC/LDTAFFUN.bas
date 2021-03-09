
\*****************************************************************************
\*****************************************************************************
\***
\***                   FILE HANDLING FUNCTION SOURCE CODE
\***
\***                   FILE TYPE:  DIRECT
\***
\***                   REFERENCE:  LDTAFFUN.J86
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               LDT AUDIT FILE MOVEMENT FILE
\***
\***
\***      VERSION A          Michael J. Kelsall         30th September 1993
\***
\***      VERSION B          Michael J. Kelsall         23rd March 1994
\***      Change to report link duration in seconds as opposed to minutes,
\***      thus requiring the link duration to be stored as a 4 byte integer
\***      
\*****************************************************************************
\*****************************************************************************

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE LDTAFDEC.J86



   FUNCTION LDTAF.SET PUBLIC
 
      INTEGER*2 LDTAF.SET
      LDTAF.SET = 1

         LDTAF.REPORT.NUM%        = 328
         LDTAF.FILE.NAME$         = "LDTAF"
         LDTAF.RECL%              = 18
        
      LDTAF.SET = 0
    
   END FUNCTION



   FUNCTION READ.LDTAF PUBLIC

    INTEGER*2 READ.LDTAF
    STRING    FORMAT$

    READ.LDTAF = 1

     IF END #LDTAF.SESS.NUM% THEN READ.LDTAF.ERROR
     READ FORM "C3,C2,I4,I1,2I4"; # LDTAF.SESS.NUM%;                    \
           LDTAF.START.DATE$,                                           \
           LDTAF.START.TIME$,                                           \
           LDTAF.DURATION%,                                             \
           LDTAF.LINK.TYPE%,                                            \
           LDTAF.DATA.VOLUME.1%,                                        \
           LDTAF.DATA.VOLUME.2%

   READ.LDTAF = 0
   EXIT FUNCTION

   READ.LDTAF.ERROR:

     CURRENT.REPORT.NUM% = LDTAF.REPORT.NUM%
     FILE.OPERATION$ = "R"
     CURRENT.CODE$ = ""
     EXIT FUNCTION

  END FUNCTION



  FUNCTION WRITE.LDTAF PUBLIC

     INTEGER*2 WRITE.LDTAF
     STRING    FORMAT$

     WRITE.LDTAF = 1

        IF END #LDTAF.SESS.NUM% THEN WRITE.LDTAF.ERROR
        WRITE FORM "C3,C2,I4,I1,2I4"; #LDTAF.SESS.NUM%;                 \
           LDTAF.START.DATE$,                                           \
           LDTAF.START.TIME$,                                           \
           LDTAF.DURATION%,                                             \
           LDTAF.LINK.TYPE%,                                            \
           LDTAF.DATA.VOLUME.1%,                                        \
           LDTAF.DATA.VOLUME.2%

     WRITE.LDTAF = 0
     EXIT FUNCTION

     WRITE.LDTAF.ERROR:

        CURRENT.REPORT.NUM% = LDTAF.REPORT.NUM%
        FILE.OPERATION$ = "W"
        CURRENT.CODE$ = ""
        EXIT FUNCTION

  END FUNCTION



  FUNCTION WRITE.HOLD.LDTAF PUBLIC

     INTEGER*2 WRITE.HOLD.LDTAF
     STRING    FORMAT$

     WRITE.HOLD.LDTAF = 1

        IF END #LDTAF.SESS.NUM% THEN WRITE.HOLD.LDTAF.ERROR
        WRITE FORM "C3,C2,I4,I1,2I4"; HOLD #LDTAF.SESS.NUM%;            \
           LDTAF.START.DATE$,                                           \
           LDTAF.START.TIME$,                                           \
           LDTAF.DURATION%,                                             \
           LDTAF.LINK.TYPE%,                                            \
           LDTAF.DATA.VOLUME.1%,                                        \
           LDTAF.DATA.VOLUME.2%

     WRITE.HOLD.LDTAF = 0
     EXIT FUNCTION

     WRITE.HOLD.LDTAF.ERROR:

        CURRENT.REPORT.NUM% = LDTAF.REPORT.NUM%
        FILE.OPERATION$ = "W"
        CURRENT.CODE$ = ""
        EXIT FUNCTION

  END FUNCTION

