\/******************************************************************/
\/*                                                                */
\/* RCSPI File Functions                                           */
\/*                                                                */
\/* REFERENCE   : RCSPIFUN.BAS                                     */
\/*                                                                */
\/* VERSION A.          Neil Bennett.                29 May 2007   */
\/*                                                                */
\/******************************************************************/

   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%

   STRING GLOBAL    CURRENT.CODE$,                                  \
                    FILE.OPERATION$

   %INCLUDE RCSPIDEC.J86

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION RCSPI.SET                                             */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION RCSPI.SET PUBLIC

      INTEGER*1 RCSPI.SET

      RCSPI.REPORT.NUM% = 748
      RCSPI.KEYL%       =   8
      RCSPI.RECL%       = 168
      RCSPI.FILE.NAME$  = "RCSPI"

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION READ.RCSPI                                            */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION READ.RCSPI PUBLIC

      INTEGER*2 READ.RCSPI

      READ.RCSPI = 1

      IF END #RCSPI.SESS.NUM% THEN READ.ERROR
      READ FORM "T9,C160";                                          \
           #RCSPI.SESS.NUM%                                         \
           KEY RCSPI.REFERENCE$;                                    \
               RCSPI.RECALL.SPECIAL.INSTRUCTION$

      READ.RCSPI = 0

   EXIT FUNCTION

READ.ERROR:

      FILE.OPERATION$     = "R"
      CURRENT.REPORT.NUM% = RCSPI.REPORT.NUM%
      CURRENT.CODE$       = ""

   END FUNCTION

\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION WRITE.RCSPI                                           */
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

   FUNCTION WRITE.RCSPI PUBLIC

      INTEGER*2 WRITE.RCSPI

      WRITE.RCSPI = 1

      IF END #RCSPI.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C8,C160";                                         \
           #RCSPI.SESS.NUM%;                                        \
               RCSPI.REFERENCE$,                                    \
               RCSPI.RECALL.SPECIAL.INSTRUCTION$

      WRITE.RCSPI = 0
      EXIT FUNCTION

WRITE.ERROR:

      FILE.OPERATION$ = "W"
      CURRENT.REPORT.NUM% = RCSPI.REPORT.NUM%
      CURRENT.CODE$ = ""

   END FUNCTION





\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
\/* FUNCTION DELETE.RCSPI                                          */
\/*                                                                */
\/*    This function returns 4 different return codes              */    
\/*                                                                */    
\/*            0 - Record successfully deleted                     */    
\/*            1 - Record Deletion error                           */    
\/*            2 - Session number invalid ie. 0                    */    
\/*            3 - Invalid Key Length                              */    
\/*                                                                */    
\/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/

  FUNCTION DELETE.RCSPI (KEY$) PUBLIC                              
                                                                     
   INTEGER*2 DELETE.RCSPI                                          
   STRING    KEY$                                                    
                                                                     
   DELETE.RCSPI = 0                                                
                                                                     
   IF RCSPI.SESS.NUM% = 0 THEN BEGIN                               
       DELETE.RCSPI = 2                                            
       GOTO SET.RCSPI.DELETE.ERROR                                                 
   ENDIF                                                             
                                                                     
   IF LEN(KEY$) <> RCSPI.KEYL% THEN BEGIN                          
       DELETE.RCSPI = 3                                            
       GOTO SET.RCSPI.DELETE.ERROR                                                 
   ENDIF                                                             
                                                                     
   IF END #RCSPI.SESS.NUM% THEN DELETE.RCSPI.ERROR                     
                                                                     
   DELREC RCSPI.SESS.NUM%; KEY$                                    

EXIT.FUNCTION:
   EXIT FUNCTION                                                     
                                                                     
DELETE.RCSPI.ERROR:                                                     
                                                                     
   DELETE.RCSPI = 1                                                
                                                                     
SET.RCSPI.DELETE.ERROR:                                           
      FILE.OPERATION$     = "D"                                   
      CURRENT.REPORT.NUM% = RCSPI.REPORT.NUM%                   
      CURRENT.CODE$       = ""                                    
                                                                  
  END FUNCTION  
                                                       
\/******************************************************************/
