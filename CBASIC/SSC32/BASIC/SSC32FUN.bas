\/******************************************************************/
\/*                                                                */
\/* SSC32 Log File Functions                                       */
\/*                                                                */
\/* REFERENCE   : SSC32FUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Stuart Highley          11 July 2008       */
\/                                                                 */
\/******************************************************************/

    INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

    STRING GLOBAL    CURRENT.CODE$,                                 \
                     FILE.OPERATION$

    %INCLUDE SSC32DEC.J86

    %INCLUDE BTCMEM.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION SSC32.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION SSC32.SET PUBLIC

        INTEGER*1 SSC32.SET

        SSC32.REPORT.NUM% = 771
        SSC32.FILE.NAME$  = "SSC32:"

    END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.SSC32                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION READ.SSC32 PUBLIC

        INTEGER*2 READ.SSC32

        READ.SSC32 = 1
        IF END #SSC32.SESS.NUM% THEN READ.ERROR
        READ #SSC32.SESS.NUM%; LINE SSC32.REC$
        READ.SSC32 = 0
        EXIT FUNCTION

READ.ERROR:

        FILE.OPERATION$     = "R"
        CURRENT.REPORT.NUM% = SSC32.REPORT.NUM%
        CURRENT.CODE$       = ""

    END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.SSC32                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

    FUNCTION WRITE.SSC32 PUBLIC

        INTEGER*2 WRITE.SSC32

        WRITE.SSC32 = 1
        IF END #SSC32.SESS.NUM% THEN WRITE.ERROR
        PRINT USING "&"; #SSC32.SESS.NUM%; SSC32.REC$
        WRITE.SSC32 = 0
        EXIT FUNCTION

WRITE.ERROR:

        FILE.OPERATION$ = "W"
        CURRENT.REPORT.NUM% = SSC32.REPORT.NUM%
        CURRENT.CODE$ = ""

    END FUNCTION

\/******************************************************************/

