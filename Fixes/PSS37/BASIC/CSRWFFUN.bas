\*******************************************************************************
\*******************************************************************************
\***
\***                CSR WORKFILE FUNCTIONS
\***
\***                REFERENCE    : CSRWFFUN.BAS
\***
\*******************************************************************************
\*******************************************************************************
                                               
   INTEGER*2 GLOBAL             \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                \
      CURRENT.CODE$,            \
      FILE.OPERATION$
      
   %INCLUDE CSRWFDEC.J86
   
  FUNCTION CSRWF.SET PUBLIC
\***************************

    CSRWF.RECL% = 24
    CSRWF.FILLER$ = "!"
    CSRWF.REPORT.NUM%  = 189      
    CSRWF.FILE.NAME$  = "CSRWF"
    CSRWF.NULL$ = STRING$(24, CHR$(0))

  END FUNCTION
\-----------------------------------------------------------------------------
    
                                                                     
  FUNCTION READ.CSRWF PUBLIC
\****************************

    INTEGER*2 READ.CSRWF
    
    READ.CSRWF = 1
      
    IF END #CSRWF.SESS.NUM% THEN READ.ERROR
    READ FORM "C24"; #CSRWF.SESS.NUM%, CSRWF.RECORD.NO%; CSRWF.RECORD$
    CSRWF.RECORD.TYPE$ = LEFT$(CSRWF.RECORD$,2)
    IF CSRWF.RECORD.NO% = 1 THEN                                 \
       CSRWF.DELETE.FLAG$ = MID$(CSRWF.RECORD$,2,1)             :\
       CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,2)                    \
    ELSE                                                         \
       IF CSRWF.RECORD.TYPE$ = "XH" THEN                         \ Header
          CSRWF.LIST.FREQ$ = MID$(CSRWF.RECORD$,3,1)            :\
          CSRWF.UNIT.NO$ = MID$(CSRWF.RECORD$,4,2)              :\
          CSRWF.ORDER.DATE$ = MID$(CSRWF.RECORD$,6,6)           :\
          CSRWF.ORDER.TIME$ = MID$(CSRWF.RECORD$,12,4)          :\ 
          CSRWF.HD.ONORDER.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,17,1):\
          CSRWF.UNPROCESS.FLAG$ = MID$(CSRWF.RECORD$,18,1)      :\
          CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,19)                \
       ELSE                                                      \
          IF CSRWF.RECORD.TYPE$ = "XC" THEN                      \ Counted
             CSRWF.QTY.1$ = MID$(CSRWF.RECORD$,3,3)             :\
             CSRWF.QTY.2$ = MID$(CSRWF.RECORD$,6,3)             :\
             CSRWF.P.ITEM.CODE$ = MID$(CSRWF.RECORD$,10,4)      :\
             CSRWF.QTY.2.IN.SINGLES$ = MID$(CSRWF.RECORD$,14,3)     :\
             CSRWF.CSRITEM.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,17,1) :\
             CSRWF.CSRIMF.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,18,1)  :\
             CSRWF.ONORDER.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,19,1) :\
             CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,19)             \    
          ELSE                                                   \
             IF CSRWF.RECORD.TYPE$ = "XO" THEN                   \ Override
                CSRWF.ITEM.CODE$ = MID$(CSRWF.RECORD$,3,7)      :\
                CSRWF.QTY.1$ = MID$(CSRWF.RECORD$,10,3)         :\
                CSRWF.CSRITEM.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,14,1) :\
                CSRWF.CSRIMF.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,15,1)  :\
                CSRWF.ONORDER.UPDATED.FLAG$ = MID$(CSRWF.RECORD$,16,1) :\
                CSRWF.ZERO.OVERRIDE.FLAG$ = MID$(CSRWF.RECORD$,17,1) :\
                CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,17)         :\
            ELSE                                                 \
               IF CSRWF.RECORD.TYPE$ = "XT" THEN                :\
                  CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,5)         \
               ELSE                                              \
                  IF CSRWF.RECORD.TYPE$ = "XZ" THEN              \
                     CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$,11)            
    READ.CSRWF = 0
    EXIT FUNCTION
    
    READ.ERROR:
    
       CURRENT.CODE$ = CSRWF.RECORD$
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = CSRWF.REPORT.NUM%
       
       EXIT FUNCTION   

   END FUNCTION
\-----------------------------------------------------------------------------
   
   
   FUNCTION WRITE.CSRWF PUBLIC
\*******************************   
   
      INTEGER*2 WRITE.CSRWF
      
      WRITE.CSRWF = 1
      
      IF CSRWF.RECORD.NO% = 1 THEN                              \
         CSRWF.RECORD$ = CSRWF.FILLER$ +                        \
                         CSRWF.DELETE.FLAG$                     \
      ELSE                                                      \
         IF CSRWF.RECORD.TYPE$ = "XH" THEN                      \               
            CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +                \
                            CSRWF.LIST.FREQ$ +                  \
                            CSRWF.UNIT.NO$ +                    \
                            CSRWF.ORDER.DATE$ +                 \
                            CSRWF.ORDER.TIME$ +                 \               
                            CSRWF.FILLER$ +                     \
                            CSRWF.HD.ONORDER.UPDATED.FLAG$ +    \
                            CSRWF.UNPROCESS.FLAG$               \
         ELSE                                                   \
            IF CSRWF.RECORD.TYPE$ = "XC" THEN                   \               
               CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +             \
                               CSRWF.QTY.1$ +                   \           
                               CSRWF.QTY.2$ +                   \           
                               CSRWF.FILLER$ +                  \
                               CSRWF.P.ITEM.CODE$ +             \
                               CSRWF.QTY.2.IN.SINGLES$ +        \
                               CSRWF.CSRITEM.UPDATED.FLAG$ +    \
                               CSRWF.CSRIMF.UPDATED.FLAG$ +     \
                               CSRWF.ONORDER.UPDATED.FLAG$      \
            ELSE                                                \
               IF CSRWF.RECORD.TYPE$ = "XO" THEN                \               
                  CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +          \
                                  CSRWF.ITEM.CODE$ +            \
                                  CSRWF.QTY.1$ +                \
                                  CSRWF.FILLER$ +               \           
                                  CSRWF.CSRITEM.UPDATED.FLAG$ + \
                                  CSRWF.CSRIMF.UPDATED.FLAG$ +  \
                                  CSRWF.ONORDER.UPDATED.FLAG$ + \
                                  CSRWF.ZERO.OVERRIDE.FLAG$     \
               ELSE                                             \                                   
                  IF CSRWF.RECORD.TYPE$ = "XT" THEN             \               
                     CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +       \
                                     CSRWF.ITEM.COUNT$          \
                  ELSE                                          \
                     IF CSRWF.RECORD.TYPE$ = "XZ" THEN          \
                        CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +    \
                                        CSRWF.TERMINAL.NO$ +    \
                                        CSRWF.LIST.COUNT$                                            

      CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$ + CSRWF.NULL$, 24)
      
      IF END #CSRWF.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C24"; #CSRWF.SESS.NUM%, CSRWF.RECORD.NO%; CSRWF.RECORD$
      WRITE.CSRWF = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = CSRWF.RECORD$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = CSRWF.REPORT.NUM%
         
         EXIT FUNCTION 
                                                                                                  
   END FUNCTION 
\-----------------------------------------------------------------------------
   
                                     
  FUNCTION WRITE.CSRWF.HOLD PUBLIC
\**********************************

      INTEGER*2 WRITE.CSRWF.HOLD
      
      WRITE.CSRWF.HOLD = 1  
   
      IF CSRWF.RECORD.NO% = 1 THEN                              \
         CSRWF.RECORD$ = CSRWF.FILLER$ +                        \
                         CSRWF.DELETE.FLAG$                     \
      ELSE                                                      \
         IF CSRWF.RECORD.TYPE$ = "XH" THEN                      \               
            CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +                \
                            CSRWF.LIST.FREQ$ +                  \
                            CSRWF.UNIT.NO$ +                    \
                            CSRWF.ORDER.DATE$ +                 \
                            CSRWF.ORDER.TIME$ +                 \               
                            CSRWF.FILLER$ +                     \
                            CSRWF.HD.ONORDER.UPDATED.FLAG$ +    \
                            CSRWF.UNPROCESS.FLAG$               \
         ELSE                                                   \
            IF CSRWF.RECORD.TYPE$ = "XC" THEN                   \               
               CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +             \
                               CSRWF.QTY.1$ +                   \           
                               CSRWF.QTY.2$ +                   \           
                               CSRWF.FILLER$ +                  \
                               CSRWF.P.ITEM.CODE$ +             \
                               CSRWF.QTY.2.IN.SINGLES$ +        \
                               CSRWF.CSRITEM.UPDATED.FLAG$ +    \
                               CSRWF.CSRIMF.UPDATED.FLAG$ +     \
                               CSRWF.ONORDER.UPDATED.FLAG$      \
            ELSE                                                \
               IF CSRWF.RECORD.TYPE$ = "XO" THEN                \               
                  CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +          \
                                  CSRWF.ITEM.CODE$ +            \
                                  CSRWF.QTY.1$ +                \
                                  CSRWF.FILLER$ +               \           
                                  CSRWF.CSRITEM.UPDATED.FLAG$ + \
                                  CSRWF.CSRIMF.UPDATED.FLAG$ +  \
                                  CSRWF.ONORDER.UPDATED.FLAG$ + \
                                  CSRWF.ZERO.OVERRIDE.FLAG$     \
               ELSE                                             \                                 
                  IF CSRWF.RECORD.TYPE$ = "XT" THEN             \               
                     CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +       \
                                     CSRWF.ITEM.COUNT$          \
                  ELSE                                          \
                     IF CSRWF.RECORD.TYPE$ = "XZ" THEN          \
                        CSRWF.RECORD$ = CSRWF.RECORD.TYPE$ +    \
                                        CSRWF.TERMINAL.NO$ +    \
                                        CSRWF.LIST.COUNT$                                            

      CSRWF.RECORD$ = LEFT$(CSRWF.RECORD$ + CSRWF.NULL$, 24)
      
      IF END #CSRWF.SESS.NUM% THEN WRITE.HOLD.ERROR
      WRITE FORM "C24"; HOLD #CSRWF.SESS.NUM%, CSRWF.RECORD.NO%; CSRWF.RECORD$
      WRITE.CSRWF.HOLD = 0
      EXIT FUNCTION
      
      WRITE.HOLD.ERROR:
      
         CURRENT.CODE$ = CSRWF.RECORD$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = CSRWF.REPORT.NUM%
         
         EXIT FUNCTION   
                                                                                                  
   END FUNCTION 
                                     
