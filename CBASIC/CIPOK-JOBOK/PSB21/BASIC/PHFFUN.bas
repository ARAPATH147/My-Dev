\/******************************************************************/
\/*                                                                */
\/* PRICE HISTORY FILE FUNCTIONS                                   */
\/*                                                                */
\/* REFERENCE   : PHFFUN.BAS                                       */
\/*                                                                */
\/* VERSION A.          Neil Bennett.           23 OCTOBER 2006    */
\/* VERSION B.          Paul Bowers.             7 March   2007    */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE PHFDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION PHF.SET                                               */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION PHF.SET PUBLIC

      INTEGER*1 PHF.SET

      PHF.REPORT.NUM% = 732                                         !
      PHF.RECL%       = 44
      PHF.FILE.NAME$  = "PHF"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.PHF                                              */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.PHF PUBLIC

      INTEGER*2 READ.PHF

      ON ERROR GOTO PHF.ERROR                                      ! PAB
      
      READ.PHF = 1

      IF END #PHF.SESS.NUM% THEN READ.ERROR
      READ FORM "T7,I4,C3,C1,I4,C3,C1,I4,C3,C1,C1,I4,C3,C1,C3,C2";  \
           #PHF.SESS.NUM%                                           \
           KEY PHF.BAR.CODE$;                                       \
               PHF.HIST2.PRICE%,                                    \
               PHF.HIST2.DATE$,                                     \
               PHF.HIST2.TYPE$,                                     \
               PHF.HIST1.PRICE%,                                    \
               PHF.HIST1.DATE$,                                     \
               PHF.HIST1.TYPE$,                                     \
               PHF.CURR.PRICE%,                                     \
               PHF.CURR.DATE$,                                      \
               PHF.CURR.TYPE$,                                      \
               PHF.CURR.LABL$,                                      \
               PHF.PEND.PRICE%,                                     \
               PHF.PEND.DATE$,                                      \
               PHF.PEND.TYPE$,                                      \
               PHF.LAST.INC.DATE$,                                  \
               PHF.FILLER$

       READ.PHF = 0
    EXIT FUNCTION
    
PHF.ERROR:

   READ.PHF = 1
   RESUME EXITFUN
   
READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = PHF.REPORT.NUM%
      CURRENT.CODE$       = UNPACK$(PHF.BAR.CODE$)
      
EXITFUN:

    EXIT FUNCTION

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.PHF                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.PHF PUBLIC

      INTEGER*2 WRITE.PHF
                                                                    ! PAB
      ON ERROR GOTO PHF.ERROR   
     
      WRITE.PHF = 1

      IF END #PHF.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C6,I4,C3,C1,I4,C3,C1,I4,C3,C1,C1,I4,C3,C1,C3,C2"; \
           #PHF.SESS.NUM%;                                          \
               PHF.BAR.CODE$,                                       \
               PHF.HIST2.PRICE%,                                    \
               PHF.HIST2.DATE$,                                     \
               PHF.HIST2.TYPE$,                                     \
               PHF.HIST1.PRICE%,                                    \
               PHF.HIST1.DATE$,                                     \
               PHF.HIST1.TYPE$,                                     \
               PHF.CURR.PRICE%,                                     \
               PHF.CURR.DATE$,                                      \
               PHF.CURR.TYPE$,                                      \
               PHF.CURR.LABL$,                                      \
               PHF.PEND.PRICE%,                                     \
               PHF.PEND.DATE$,                                      \
               PHF.PEND.TYPE$,                                      \
               PHF.LAST.INC.DATE$,                                  \
               PHF.FILLER$

      WRITE.PHF = 0
      EXIT FUNCTION
      
PHF.ERROR:                                                          ! PAB
                                                                    ! PAB
   WRITE.PHF = 1
   RESUME EXITFUN                                                   ! PAB
                                                                    ! PAB

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = PHF.REPORT.NUM%
      CURRENT.CODE$ = UNPACK$(PHF.BAR.CODE$)
      
EXITFUN:                                                            ! PAB
                                                                    ! PAB
    EXIT FUNCTION                                                   ! PAB
      

   END FUNCTION

\/******************************************************************/