
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  NLINEFUN.BAS
\***
\***               DESCRIPTION:  NEWLINES - NEWLINES REPORT INFORMATION
\***
\***
\***
\***      VERSION 1 : Julia Stones             7th July 2003
\***
\***      Version 1.1  Julia Stones            6th October 2003
\***      Added NEWLINES.COUNT% to hold number of new existing lines found
\***
\***    REVISION 1.2.                ROBERT COWEY.               05 DEC 2003.
\***    Changed NLINE reporting number from 660 to 670.
\***
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM%

  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$

  %INCLUDE NLINEDEC.J86



  FUNCTION NEWLINES.SET PUBLIC

     INTEGER*2 NEWLINES.SET
     NEWLINES.SET = 1

       NEWLINES.REPORT.NUM% = 670
       NEWLINES.RECL%      = 16
       NEWLINES.FILE.NAME$ = "NEWLINES"

     NEWLINES.SET = 0

  END FUNCTION



  FUNCTION READ.NEWLINES PUBLIC

    INTEGER*2 READ.NEWLINES

    READ.NEWLINES = 1

    IF END #NEWLINES.SESS.NUM% THEN READ.ERROR
    READ FORM "T5,C4,I2,C6";  \                                      ! 1.1JAS
            #NEWLINES.SESS.NUM% KEY NEWLINES.BOOTS.CODE$;   \
                             NEWLINES.DATE.ADDED$,    \
                             NEWLINES.COUNT%,         \              ! 1.1JAS
                             NEWLINES.FILLER$         !
       READ.NEWLINES = 0
       EXIT FUNCTION

    READ.ERROR:

       CURRENT.CODE$ = NEWLINES.BOOTS.CODE$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = NEWLINES.REPORT.NUM%
 EXIT FUNCTION

  END FUNCTION


  FUNCTION WRITE.NEWLINES PUBLIC

    INTEGER*2 WRITE.NEWLINES

    WRITE.NEWLINES = 1

    IF END #NEWLINES.SESS.NUM% THEN WRITE.ERROR
    WRITE FORM "C4,C4,I2,C6";   \                               ! 1.1JAS
        #NEWLINES.SESS.NUM%;            \
                NEWLINES.BOOTS.CODE$,      \
                NEWLINES.DATE.ADDED$,      \
                NEWLINES.COUNT%,           \                    !1.1JAS
                NEWLINES.FILLER$           !

       WRITE.NEWLINES = 0
       EXIT FUNCTION

    WRITE.ERROR:

       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = NEWLINES.REPORT.NUM%
       CURRENT.CODE$ = NEWLINES.BOOTS.CODE$

       EXIT FUNCTION

  END FUNCTION

