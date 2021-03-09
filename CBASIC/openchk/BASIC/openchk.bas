\**********************************************************************
\*
\* OPENCHK.286
\*
\* Version 1.0 - 02/02/2009 - David Artiss
\*
\* This program checks whether a store are trading or not by checking
\* the date and time stamp on the TSL.
\* There is only one parameter and this is the hour of today that they
\* must have been trading since to be regarded as open. This must be
\* between 0 and 23.
\* A file named either CLOSED or OPEN will be created on C:/ of the
\* master controller.
\*
\* Version 1.1 - 09/02/2009 - David Artiss
\* 
\* Now that the snow has died down, I'm taking the opportunity to
\* pretty much re-write the code to make it a lot more accurate.
\* Unfortunately, the TSL is updated by store close and simply signing
\* onto a till, so using the timestamp is not 100% accurate.
\* Instead, this new version will read the TSL itself to look for type
\* 0 transactions - and, in particular, the date and timestamp within
\* them.
\* Output messages have been designed for NFM in mind.
\*
\* Version 1.2 - 22/11/2010 - David Artiss
\* Removed output to file - relies on NFM output.
\*
\* Version 1.3 - 01/12/2010 - David Artiss
\* Report whether store have performed a close or not.
\* Removed previously commented out code
\*
\**********************************************************************

%INCLUDE ENDOKDEC.J86                                                   !1.3 DA

INTEGER*2 CLOSE.FLAG%,           \XDA
          RC%,                   \
          TRADING.FLAG%,         \XDA
          TSL.SESS.NUM%          !1.1 DA

INTEGER*4 FIELD%,                \1.1 DA
          LOOP%

STRING    CLOSE.STATUS$,         \1.3 DA
          CURRENT.DATE$,         \
          CURRENT.HOUR$,         \
          TRADING.STATUS$,       \1.3 DA
          TSL.DATE$,             \
          TSL.FILE.NAME$,        \
          TSL.HOUR$,             \
          TSL.REC$,              \
          VALID$ 
          
%INCLUDE ENDOKEXT.J86                                                   !1.3 DA

FUNCTION READ.TSL PUBLIC                                                !1.1 DA

    INTEGER*2 READ.TSL                                                  !1.1 DA

    READ.TSL = 1                                                        !1.1 DA
    IF END # TSL.SESS.NUM% THEN READ.TSL.ERROR                          !1.1 DA
    READ # TSL.SESS.NUM% ; TSL.REC$                                     !1.1 DA
    READ.TSL = 0                                                        !1.1 DA

    EXIT FUNCTION                                                       !1.1 DA

READ.TSL.ERROR:                                                         !1.1 DA

    EXIT FUNCTION                                                       !1.1 DA

END FUNCTION                                                            !1.1 DA

\*
\* Program Control
\*

   ON ERROR GOTO ERROR.DETECTED

   GOSUB INITIALISATION
   
   GOSUB MAIN.PROCESSING
   
   STOP

\*
\* Initialise variables
\*

INITIALISATION:

   CALL ENDOK.SET                                                       !1.3 DA
   
   CLOSE.FLAG% = 0                                                      !XDA
   TRADING.FLAG% = 0                                                    !XDA

   VALID$ = "NO"                                                   
   FOR LOOP% = 0 TO 23
      IF STR$(LOOP%) = COMMAND$ THEN VALID$ = "YES"
   NEXT LOOP%

   IF COMMAND$ = "" OR VALID$ = "NO" THEN BEGIN
      PRINT "OPENCHK.286 - DA - V1.3"                                   !1.3 DA
      PRINT "======================="
      PRINT "OPENCHK.286 checks whether a store are trading or not " +  \1.1 DA
            "by checking for any"                                       !1.1 DA
      PRINT "till transaction within the TSL, and then looking at " +   \1.1 DA
            "their timestamps."                                         !1.1 DA
      PRINT "There is only one parameter and this is the hour of " +    \1.1 DA
            "today that they must have"                                 !1.1 DA
      PRINT "been trading since to be regarded as open. This must " +   \1.1 DA
            "be between 0 and 23."                                      !1.1 DA
      PRINT "A file named either CLOSED or OPEN will be created on " +  \1.1 DA
            "C:/ of the master"                                         !1.1 DA
      PRINT "controller."
      STOP
   ENDIF

   TSL.SESS.NUM%     = 1                                                !1.3 DA
   TSL.FILE.NAME$    = "ADXLXACN::D:\ADX_IDT1\EALTRANS.DAT"             !1.1 DA
   
   ENDOK.SESS.NUM%   = 2                                                !1.3 DA
   ENDOK.REPORT$     = "-"                                              !1.3 DA
   
   GOSUB GET.ENDOK.STATUS                                               !1.3 DA
   
   IF END # TSL.SESS.NUM% THEN TSL.MISSING                              !1.1 DA
   OPEN TSL.FILE.NAME$ AS TSL.SESS.NUM% NOWRITE NODEL                   !1.1 DA
   
RETURN

\*
\* Main Processing 
\*

MAIN.PROCESSING:

   TSL.DATE$ = "------"                                                 !1.1 DA
   TSL.HOUR$ = "--"                                                     !1.1 DA
   
   RC% = READ.TSL                                                       !1.1 DA
   WHILE RC% = 0                                                        !1.1 DA
      IF UNPACK$(LEFT$(TSL.REC$,1)) = "00" THEN BEGIN                   !1.1 DA
         FIELD% = 1                                                     !1.1 DA
         FOR LOOP% = 1 TO LEN(TSL.REC$)                                 !1.1 DA
            IF MID$(TSL.REC$,LOOP%,1) = ":" THEN BEGIN                  !1.1 DA
               FIELD% = FIELD% + 1                                      !1.1 DA
               IF FIELD% = 12 THEN BEGIN                                !1.1 DA
                  TSL.DATE$ = UNPACK$(MID$(TSL.REC$,LOOP%+1,3))         !1.1 DA
               ENDIF                                                    !1.1 DA
               IF FIELD% = 13 THEN BEGIN                                !1.1 DA
                  TSL.HOUR$ = LEFT$(UNPACK$(MID$(TSL.REC$,LOOP%+1,2)),2)!1.1 DA
               ENDIF
            ENDIF                                                       !1.1 DA
         NEXT LOOP%                                                     !1.1 DA
      ENDIF                                                             !1.1 DA
      RC% = READ.TSL                                                    !1.1 DA
   WEND                                                                 !1.1 DA
   
   CURRENT.HOUR$   = LEFT$(TIME$,2)
   CURRENT.DATE$ = LEFT$(DATE$,2) + MID$(DATE$,3,2) + RIGHT$(DATE$,2)
   
   PRINT TSL.DATE$ + "/" + TSL.HOUR$ + "/"; + ENDOK.REPORT$ + "/";      !1.3 DA
   
   IF TSL.DATE$ <> CURRENT.DATE$ THEN BEGIN
      PRINT "CLOSED/DATE"
      PRINT "Store is NOT trading: DIFFERENT DATE"
   ENDIF ELSE BEGIN
      IF VAL(TSL.HOUR$) < VAL(COMMAND$) THEN BEGIN
         PRINT "CLOSED/TIME"
         PRINT "Store is NOT trading: NO ACTIVITY SINCE ";
         IF VAL(COMMAND$) = 0 THEN PRINT "MIDNIGHT"
         IF VAL(COMMAND$) = 12 THEN PRINT "MIDDAY"
         IF VAL(COMMAND$) > 12 THEN PRINT STR$(VAL(COMMAND$)-12) + "PM"
         IF VAL(COMMAND$) < 12 AND VAL(COMMAND$) > 0 THEN               \
                                              PRINT COMMAND$ + "AM"
      ENDIF ELSE BEGIN
         PRINT "OPEN"
         PRINT "Store is open and trading"
      ENDIF
   ENDIF       
   
   IF TSL.DATE$ = "------" THEN BEGIN                                   !1.1 DA
      PRINT "No transaction records were found on the TSL"              !1.1 DA
   ENDIF ELSE BEGIN                                                     !1.1 DA
      PRINT "The most recent transaction date on the TSL was " +        \1.1 DA
            TSL.DATE$                                                   !1.1 DA
      PRINT "The most recent transaction hour on the TSL was " +        \1.1 DA
            TSL.HOUR$                                                   !1.1 DA
   ENDIF
   
   IF ENDOK.REPORT$ = "-" THEN BEGIN                                    !1.3 DA
       PRINT "Could not work out if store have closed today or not"     !1.3 DA
   ENDIF ELSE BEGIN                                                     !1.3 DA
       IF ENDOK.REPORT$ = "Y" THEN BEGIN                                !1.3 DA
           PRINT "The store have closed today"                          !1.3 DA
       ENDIF ELSE BEGIN                                                 !1.3 DA
           PRINT "The store have not closed today"                      !1.3 DA
       ENDIF                                                            !1.3 DA
   ENDIF                                                                !1.3 DA
              
RETURN

GET.ENDOK.STATUS:                                                       !1.3 DA

   IF END # ENDOK.RECL% THEN ENDOK.SKIP                                 !1.3 DA
   OPEN ENDOK.FILE.NAME$ RECL ENDOK.RECL% AS ENDOK.SESS.NUM% NOWRITE    \1.3 DA
                                                             NODEL      !1.3 DA
                                                             
   RC% = READ.ENDOK                                                     !1.3 DA        
   IF RC% = 0 THEN BEGIN                                                !1.3 DA
       IF DATE$ = ENDOK.DATE$ THEN BEGIN                                !1.3 DA
           CLOSE.STATUS$ = "Store closed at " + LEFT$(ENDOK.TIME$,2) +  \1.3 DA
                           ":" + MID$(ENDOK.TIME$,3,2)                  !1.3 DA
           CLOSE.FLAG%   = 1                                            !XDA
       ENDIF ELSE BEGIN                                                 !1.3 DA
           CLOSE.STATUS$ = "Store has not closed today"                 !1.3 DA
           CLOSE.FLAG%   = 2                                            !XDA
       ENDIF                                                            !1.3 DA
   ENDIF                                                                !1.3 DA
   CLOSE ENDOK.SESS.NUM%                                                !1.3 DA
   
   ENDOK.SKIP:                                                          !1.3 DA
      
RETURN                                                                  !1.3 DA

TSL.MISSING:                                                            !1.1 DA

   TRADING.FLAG% = 1                                                    !XDA
   TRADING.STATUS$ = "Store has not traded today"                       !XDA
   GOSUB NFM.OUTPUT                                                     !XDA
   
   STOP                                                                 !1.1 DA
      
RETURN                                                                  !1.1 DA

NFM.OUTPUT:                                                             !XDA

   PRINT "OPENCHK:," + STR$(TRADING.FLAG%) + "," + STR$(CLOSE.FLAG%) +  \XDA
         "," + STORE.NUMBER$ + "," + TRADING.STATUS$ + "," +            \XDA
         CLOSE.STATUS$                                                  \XDA

RETURN                                                                  !XDA

\*
\* Error routine
\*

ERROR.DETECTED:

   TRADING.FLAG% = 0                                                    !XDA
   CLOSE.FLAG% = 0                                                      !XDA
   TRADING.STATUS$ = "The program has ended abnormally"                 !XDA
   CLOSE.STATUS$ = ""                                                   !XDA
   GOSUB NFM.OUTPUT                                                     !XDA

   STOP

END
