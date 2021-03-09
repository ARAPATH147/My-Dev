REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***            FUNCTION      : CONV.TO.STRING
\***            AUTHOR        : Bruce Scriver   (Pseudo code)
\***                          : Stephen Kelsey  (Basic code)
\***            DATE WRITTEN  : 19th February 1986  (Pseudo code)
\***                          : 26th February 1986  (Basic code)
\***
\***            REFERENCE     : PSBF17
\***
\***
\***            VERSION B.       ANDREW WEDGEWORTH           2nd July 1992
\***            Redundant function parameters removed and defined as global
\***            variables instead.
\***
\***            VERSION C.   STUART WILLIAM MCCONNACHIE         2nd Sept 2005
\***            Removed version numbered included code - About time.
\***            This is so we can compile FUNLIB version without line numbers.
\***
\*******************************************************************************
\*******************************************************************************

REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***            FUNCTION OVERVIEW
\***            -----------------
\***
\***   This function is called to convert a 4 byte integer into a string of
\***   ASCII characters which is the string representation of the integer.
\***   The function is used primarily to aid the construction of unique data
\***   for use with the APPLICATION.LOG and ADXERROR functions.  If an error
\***   occurs in processing, the error is logged as event 19.
\***
\***
\*******************************************************************************
\*******************************************************************************

REM    PSEUDOCODE for this module follows....\

\*****************************************************************************
\*****************************************************************************
\***
\***
\***   %INCLUDE globals for CONV.TO.STRING function
\***   %INCLUDE external definition of APPLICATION.LOG function
\***

   %INCLUDE PSBF17G.J86                                                ! CSWM

   STRING GLOBAL                                                       \ BAW
          BATCH.SCREEN.FLAG$,                                          \ BAW
          MODULE.NUMBER$,                                              \ BAW
          OPERATOR.NUMBER$                                             ! BAW             

   ! 1 line deleted from here                                          ! BAW

   %INCLUDE PSBF01E.J86                                                ! CSWM

\***...........................................................................
\***
\***  Define function parameters and program variables.
\***
\*******************************************************************************

   FUNCTION CONV.TO.STRING  (EVENT.NO%,                                \
                 INTEGER.4%)  PUBLIC
   ! 3 parameters removed from here                                    ! BAW


   STRING  VAR.STRING.1$,                                              \
           VAR.STRING.2$
   ! 3 variables deleted from here                                     ! BAW
   

   INTEGER*1  EVENT.NO%,                                               \
              EVENT.NUM%

   INTEGER*2  CONV.TO.STRING,                                          \ BAW
              MESSAGE.NO%,                                             \
          RC%                                                      ! BAW

   INTEGER*4  COUNT%,                                                  \
              DIGIT%,                                                  \
              INTEGER.4%


\***...........................................................................
\***
\***   REM start of mainline code
\***
\***   ON ERROR GOTO ERROR.DETECTED
\***
\***   set CONV.TO.STRING to 0
\***
\***   set F17.RETURNED.STRING$ to null
\***
\***   FOR count from 24 TO 0 STEP -8
\***
\***      SHIFT integer by count and place result in digit
\***      set F17.RETURNED.STRING$ to F17.RETURNED.STRING$ + CHR$(digit)
\***
\***   NEXT count
\***
\***   EXIT FUNCTION to calling program
\***
\***...........................................................................

      ON ERROR GOTO ERROR.DETECTED

      CONV.TO.STRING = 0
      F17.RETURNED.STRING$ = ""

      FOR COUNT% = 24 TO 0 STEP -8
          DIGIT% = SHIFT(INTEGER.4%, COUNT%) 
          F17.RETURNED.STRING$ = F17.RETURNED.STRING$ + CHR$(DIGIT%)
      NEXT COUNT%

      EXIT FUNCTION

\*********************** subroutine follows ************************************
\*******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***   set CONV.TO.STRING to 1
\***   CALL APPLICATION.LOG function to log error 707, event number 19
\***
\***   IF batch/screen flag = "S" THEN
\***      EXIT FUNCTION
\***   ELSE
\***      STOP processing
\***   ENDIF
\***
\***...........................................................................

   ERROR.DETECTED:

      CONV.TO.STRING = 1
      VAR.STRING.1$ = CHR$(EVENT.NO%)
      VAR.STRING.2$ = VAR.STRING.2$ + STR$(INTEGER.4%)
      EVENT.NUM% = 19
      MESSAGE.NO% = 707
      RC% = APPLICATION.LOG (MESSAGE.NO%,                              \ BAW
                             VAR.STRING.1$,                            \
                             VAR.STRING.2$,                            \
                             EVENT.NUM%)
      ! 3 parameters removed from here                                 ! BAW                 

      IF BATCH.SCREEN.FLAG$ = "S" THEN                                 \
         EXIT FUNCTION                                                :\
      ELSE                                                             \
         STOP


\*******************************************************************************
\***
\***   END FUNCTION
\***
\*******************************************************************************
\*******************************************************************************
REM end of pseudocode

   END FUNCTION

END
