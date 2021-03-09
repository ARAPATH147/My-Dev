REM \
\*******************************************************************************
\*******************************************************************************
\***
\***
\***                  FUNCTION      : STANDARD.ERROR.DETECTED
\***
\***                  REFERENCE     : PSBF24
\***
\***      Version A                Janet Smith                13th May 1992
\***
\***      Version B                Andrew Wedgeworth          1st July 1992
\***      Removal of now redundant parameters passed to and from this 
\***      function.  These variables are defined globally instead.       
\***
\***      VERSION C.   STUART WILLIAM MCCONNACHIE         2nd Sept 2005
\***      Removed version numbered included code - About time.
\***      This is so we can compile FUNLIB version without line numbers.
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
\***   This function is called to log an event 1 as a result of a problem
\***   in the calling program.
\***  
\***   The return code will be the result of the function (as opposed to  
\***   being a global variable) and will have the following values:
\***      0 : success
\***      1 : failure
\***
\*******************************************************************************
\******************************************************************************

\/*********************************************************************/
\/*                                                                   */
\/*     Define GLOBALS and EXTERNALS                                  */
\/*                                                                   */
\/*********************************************************************/

        STRING GLOBAL                                                  \ BAW
           BATCH.SCREEN.FLAG$,                                         \ BAW
           OPERATOR.NUMBER$,                                           \ BAW
           MODULE.NUMBER$                         


        %INCLUDE  PSBF16G.J86                                          ! CSWM
        %INCLUDE  PSBF17G.J86                                          ! CSWM
        %INCLUDE  PSBF20G.J86                                          ! CSWM
        %INCLUDE  PSBF01E.J86                                          ! CSWM
        %INCLUDE  PSBF16E.J86                                          ! CSWM
        %INCLUDE  PSBF17E.J86                                          ! CSWM
        %INCLUDE  PSBF20E.J86                                          ! CSWM

\/********************************************************************
\/*                                                                   
\/*     FUNCTION STANDARD.ERROR.DETECTED
\/*     --------------------------------
\/*                                                                   
\/********************************************************************

    FUNCTION STANDARD.ERROR.DETECTED(ERRN%,                \
                     ERRFILE%,             \
                     ERRL%,                \
                     ERR$)       PUBLIC
    ! 4 parameters removed from here                                   ! BAW
      
        STRING      ERR$,                     \
            FUNCTION.FLAG$,           \
        \ 4 lines deleted from here                                    \ BAW    
            PASSED.STRING$,           \
            VAR.STRING.1$,            \
            VAR.STRING.2$
            
            
    INTEGER*2   EVENT.NUMBER%,            \
                    ERRFILE%,                 \
                ERRL%,                    \
            MESSAGE.NUMBER%,          \
            PASSED.INTEGER%,          \
            RC%,                      \                          BAW 
            STANDARD.ERROR.DETECTED
            
        INTEGER*4   ERRN%                       
            
\/********************************************************************
\/*                                                                   
\/*     MAINLINE CODE
\/*                                                                   
\/********************************************************************

    ON ERROR GOTO ERROR.DETECTED

       STANDARD.ERROR.DETECTED = -1
       
       FUNCTION.FLAG$    = "R"
       PASSED.INTEGER%   = ERRFILE%
       GOSUB CALL.SESS.NUM.UTILITY 

       EVENT.NUMBER%    = 101
       MESSAGE.NUMBER%  = 550
       
       GOSUB SET.VAR.STRING.1$
       
       IF BATCH.SCREEN.FLAG$ = "S" THEN                    \
           GOSUB SET.VAR.STRING.2$
       
       RC% = APPLICATION.LOG (MESSAGE.NUMBER%,                         \ BAW
                              VAR.STRING.1$,                           \
                              VAR.STRING.2$,                           \
                              EVENT.NUMBER%)
       ! 3 parameters no longer passed to APPLICATION.LOG              ! BAW                  

       IF RC% = 0 THEN                                                 \ BAW
           STANDARD.ERROR.DETECTED = 0

    EXIT.STD.ERROR.DETECTED:
    
    EXIT FUNCTION
    
\******************************************************************************
\***   SET.VAR.STRING.1$:
\***
\******************************************************************************

    SET.VAR.STRING.1$:
    
      RC% = CONV.TO.STRING (EVENT.NUMBER%,                             \ BAW 
                ERRN%)
      ! 3 parameters removed from here                                 ! BAW                
 
      IF RC% <> 0 THEN                                                 \ BAW
         GOTO EXIT.STD.ERROR.DETECTED

      VAR.STRING.1$ = F17.RETURNED.STRING$                    +    \
                      CHR$(SHIFT(F20.INTEGER.FILE.NO%,8))     +    \
                      CHR$(SHIFT(F20.INTEGER.FILE.NO%,0))     +    \
              PACK$(RIGHT$(MODULE.NUMBER$,2))         +    \
              PACK$(RIGHT$("000000" + STR$(ERRL%),6))

    RETURN

\******************************************************************************
\***   SET.VAR.STRING.2$:
\***
\******************************************************************************

    SET.VAR.STRING.2$:
    
      RC% = CONV.TO.HEX (ERRN%)                                        ! BAW
      ! 3 parameters removed from here                                 ! BAW      
 
      IF RC% <> 0 THEN                                                 \ BAW
         GOTO EXIT.STD.ERROR.DETECTED

      VAR.STRING.2$ = ERR$                                         +    \
                      F16.HEX.STRING$                              +    \
                      RIGHT$("000" + STR$(F20.INTEGER.FILE.NO%),3) +    \
              RIGHT$("000000" + STR$(ERRL%),6)

    RETURN

\/********************************************************************
\/*  CALL.SESS.NUM.UTILITY:
\/*     
\/********************************************************************

   CALL.SESS.NUM.UTILITY:
   
        RC% = SESS.NUM.UTILITY(FUNCTION.FLAG$,                         \ BAW
                               PASSED.INTEGER%,                        \
                   PASSED.STRING$)
        ! 3 parameters removed from here                               ! BAW                   
                   
        IF  RC% = 1 THEN                                               \ BAW
        GOTO EXIT.STD.ERROR.DETECTED

     RETURN

\/********************************************************************
\/*     ERROR.DETECTED:
\/*     
\/********************************************************************

    ERROR.DETECTED:

        RESUME EXIT.STD.ERROR.DETECTED


END FUNCTION
