\*****************************************************************************
\*****************************************************************************
\***
\***                   ENDOK FILE FUNCTIONS 
\***
\***                      REFERENCE    : ENDOKFUN
\***
\***           VERSION A : STEVEN GOULDING  13.10.92
\***
\***           Version B : Stephen Windsor  21.01.93
\***           Updated to include new security flag (CASTLE)
\***                                                                           
\***           Version C : Chris Osborne    11-06-96
\***           Updated to include gap summary files (GAP MONITOR)
\***
\***           Version D: Mark Goode        11-01-99
\***           Updated to include extra fields for cash accounting flags
\***
\***           Version E: Brian Greenfield  24/12/99
\***           Added PSB90 run date to the end of the file.
\***           Changed FREESPACEto 30 bytes for future flags.
\***           Included ENDOK.FILLER$ for CR/LF at end of file.
\***  
\***           Version F:  Amy Hoggard  12/12/2000
\***           ENDOK.RESET.TBAGK$ added for ECO
\***           FREESPACE$ reduced to 29 bytes.
\***
\***           Version G:  David Artiss 29/01/2002
\***           ENDOK.RESET.ETULC$ and ENDOK.RESET.ETULP$ added for eTop-Up
\***           FREESPACE$ reduced to 27 bytes.
\***
\***           Version H:  David Artiss 19/07/2002
\***           Add new fields ENDOK.PSB90.TIME$, ENDOK.PSB04.DATE$ and
\***           ENDOK.PSB04.TIME$.
\***
\*****************************************************************************
\*****************************************************************************


   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE ENDOKDEC.J86             !BSJW


   FUNCTION ENDOK.SET PUBLIC
REM \

    ENDOK.REPORT.NUM%  = 36
    ENDOK.RECL%        = 80
    ENDOK.FILE.NAME$   = "ENDOK"
END FUNCTION
                           
\----------------------------------------------------------------------------

REM \

  FUNCTION READ.ENDOK PUBLIC

   INTEGER*1 READ.ENDOK

   READ.ENDOK = 1

   IF END#ENDOK.SESS.NUM% THEN READ.ENDOK.ERROR

    READ FORM "2C6,33C1,C9,4C6,C2"; #ENDOK.SESS.NUM%,1;          \ HDA
              ENDOK.DATE$,                                       \
              ENDOK.TIME$,                                       \
              ENDOK.PROCEDURE$,                                  \
              ENDOK.RECOVERY$,                                   \
              ENDOK.RERUN$,                                      \
              ENDOK.DISABLE$,                                    \
              ENDOK.MESS.DELETE$,                                \
              ENDOK.COPY.TERMS$,                                 \
              ENDOK.STC.UPDATE$,                                 \
              ENDOK.TILL.TAKINGS$,                               \
              ENDOK.M.D.TRANS$,                                  \ BJAL
              ENDOK.RESET.STP$,                                  \ 
              ENDOK.RESET.ACCTP$,                                \
              ENDOK.RESET.PERFP$,                                \
              ENDOK.RESET.TPROP$,                                \
              ENDOK.RESET.PSBTERMS$,                             \
              ENDOK.RESET.TSF$,                                  \
              ENDOK.RESET.STC$,                                  \
              ENDOK.RESET.ACCTC$,                                \
              ENDOK.RESET.PERFC$,                                \
              ENDOK.RESET.TPROD$,                                \
              ENDOK.RESET.SECURITY$,                             \ BSJW
              ENDOK.RESET.GAPSUMC$,                              \ CCJO
              ENDOK.RESET.GAPSUMP$,                              \ CCJO
              ENDOK.RESET.SAFEP$,                                \ WMG
              ENDOK.RESET.TNDDC$,                                \ WMG
              ENDOK.RESET.TNDDP$,                                \ WMG
              ENDOK.RESET.COTTP$,                                \ WMG
              ENDOK.RESET.CADF$,                                 \ WMG
              ENDOK.RESET.BANKP$,                                \ WMG
              ENDOK.RESET.CCSC$,                                 \ WMG
              ENDOK.RESET.CCSP$,                                 \ WMG
              ENDOK.RESET.TBAGK$,                                \ FAH
              ENDOK.RESET.ETULC$,                                \ GDA
              ENDOK.RESET.ETULP$,                                \ GDA
              ENDOK.FREESPACE$,                                  \ WMG
              ENDOK.PSB90.DATE$,                                 \ EBG
              ENDOK.PSB90.TIME$,                                 \ HDA
              ENDOK.PSB04.DATE$,                                 \ HDA
              ENDOK.PSB04.TIME$,                                 \ HDA
              ENDOK.FILLER$                                      ! EBG
             
   READ.ENDOK = 0
   EXIT FUNCTION

   READ.ENDOK.ERROR:

   CURRENT.REPORT.NUM% = ENDOK.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION


  FUNCTION WRITE.ENDOK PUBLIC

   INTEGER*1 WRITE.ENDOK

   WRITE.ENDOK = 1

   IF END#ENDOK.SESS.NUM% THEN WRITE.ENDOK.ERROR

    WRITE FORM "2C6,33C1,C9,4C6,C2"; #ENDOK.SESS.NUM%,1;         \ HDA
              ENDOK.DATE$,                                       \
              ENDOK.TIME$,                                       \
              ENDOK.PROCEDURE$,                                  \
              ENDOK.RECOVERY$,                                   \
              ENDOK.RERUN$,                                      \
              ENDOK.DISABLE$,                                    \
              ENDOK.MESS.DELETE$,                                \
              ENDOK.COPY.TERMS$,                                 \
              ENDOK.STC.UPDATE$,                                 \
              ENDOK.TILL.TAKINGS$,                               \
              ENDOK.M.D.TRANS$,                                  \ BJAL
              ENDOK.RESET.STP$,                                  \
              ENDOK.RESET.ACCTP$,                                \
              ENDOK.RESET.PERFP$,                                \
              ENDOK.RESET.TPROP$,                                \
              ENDOK.RESET.PSBTERMS$,                             \
              ENDOK.RESET.TSF$,                                  \
              ENDOK.RESET.STC$,                                  \
              ENDOK.RESET.ACCTC$,                                \
              ENDOK.RESET.PERFC$,                                \
              ENDOK.RESET.TPROD$,                                \    
              ENDOK.RESET.SECURITY$,                             \ BSJW
              ENDOK.RESET.GAPSUMC$,                              \ CCJO
              ENDOK.RESET.GAPSUMP$,                              \ CCJO
              ENDOK.RESET.SAFEP$,                                \ WMG
              ENDOK.RESET.TNDDC$,                                \ WMG
              ENDOK.RESET.TNDDP$,                                \ WMG
              ENDOK.RESET.COTTP$,                                \ WMG
              ENDOK.RESET.CADF$,                                 \ WMG
              ENDOK.RESET.BANKP$,                                \ WMG
              ENDOK.RESET.CCSC$,                                 \ WMG
              ENDOK.RESET.CCSP$,                                 \ WMG
              ENDOK.RESET.TBAGK$,                                \ FAH
              ENDOK.RESET.ETULC$,                                \ GDA
              ENDOK.RESET.ETULP$,                                \ GDA
              ENDOK.FREESPACE$,                                  \
              ENDOK.PSB90.DATE$,                                 \ EBG
              ENDOK.PSB90.TIME$,                                 \ HDA
              ENDOK.PSB04.DATE$,                                 \ HDA
              ENDOK.PSB04.TIME$,                                 \ HDA
              ENDOK.FILLER$                                      ! EBG

   WRITE.ENDOK = 0
   EXIT FUNCTION

   WRITE.ENDOK.ERROR:

   CURRENT.REPORT.NUM% = ENDOK.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = ""

   EXIT FUNCTION
  END FUNCTION

