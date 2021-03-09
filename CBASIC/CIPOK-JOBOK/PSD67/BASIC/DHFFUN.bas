\/******************************************************************/
\/*                                                                */
\/* DHF File Functions                                             */
\/*                                                                */
\/* REFERENCE   : DHFFUN.BAS                                       */
\/*                                                                */
\/* VERSION A.          Neil Bennett.              19 April 2007   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE DHFDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION DHF.SET                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION DHF.SET PUBLIC

      INTEGER*1 DHF.SET

      DHF.REPORT.NUM% =    742
      DHF.RECL%       =      9
      DHF.KEY.LEN%    =      3
      DHF.NUM.RECS%   = 300000
      DHF.FILE.NAME$  = "DEHF"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.DHF                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.DHF PUBLIC

      INTEGER*2 READ.DHF

      READ.DHF = 1

      IF END #DHF.SESS.NUM% THEN READ.ERROR
      READ FORM "T4,C2,I1,C3";                                      \
           #DHF.SESS.NUM%                                           \
           KEY DHF.BOOTS.CODE$;                                     \
               DHF.DEAL.NUM$,                                       \
               DHF.DEAL.TYPE%,                                      \
               DHF.END.DATE$

      READ.DHF = 0

   EXIT FUNCTION

READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = DHF.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.DHF                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.DHF PUBLIC

      INTEGER*2 WRITE.DHF

      WRITE.DHF = 1

      IF END #DHF.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C3,C2,I1,C3";                                     \
           #DHF.SESS.NUM%;                                          \
               DHF.BOOTS.CODE$,                                     \
               DHF.DEAL.NUM$,                                       \
               DHF.DEAL.TYPE%,                                      \
               DHF.END.DATE$

      WRITE.DHF = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = DHF.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION


\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION DELETE.DHF                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
    FUNCTION DELETE.DHF   PUBLIC 

        INTEGER*1   DELETE.DHF

        DELETE.DHF = 1

        IF END # DHF.SESS.NUM% THEN FILE.ERROR
        DELREC DHF.SESS.NUM%; DHF.BOOTS.CODE$

        DELETE.DHF = 0

    EXIT FUNCTION

    FILE.ERROR:

        FILE.OPERATION$     = "D"
        CURRENT.REPORT.NUM% = DHF.REPORT.NUM%
        CURRENT.CODE$       = PACK$("0000") + DHF.BOOTS.CODE$

    END FUNCTION

\/******************************************************************/