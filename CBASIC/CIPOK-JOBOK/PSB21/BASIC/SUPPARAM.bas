\*****************************************************************************
\***            Suppress Labels Parameter file functions
\***            SUPPARAM.BIN
\***      Version A           Jamie Thorpe                         17th Jan 2001
\***
\***      Version B           Brian Greenfield                     5th Nov 2004
\***                Changes to add in a sale date check limit for price 
\***                increases. This means that the existing LAST.SALE.CHECK$
\***                variable is now for decreases only.
\***
\***      SUPPARAM.ACTIVE.FLAG$       (Determines if the suppression of labels is on)
\***
\***      SUPPARAM.LAST.SALE.CHECK$   (The number of months to             )
\***                                  (check against the date of last sale)
\***                                  (Now for decreases only.) !BBG
\***
\***      SUPPARAM.INITIAL.LOAD.CHECK$(The number of months to check        )
\***                                  (against the date of last initial load)
\***
\***      SUPPARAM.INC.DEC.FLAG$      (Whether to suppress just decreases (D) )
\***                                  (or just increases (I) or both (B)      )
\***
\***      SUPPARAM.INITIAL.LOAD.DATE$ (The date that of the last initial load )
\***                                  (yymmdd                                 )
\***
\*******************************************************************************

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE SUPPDEC.J86
   
!*****************************************************************************

   FUNCTION SUPPARAM.SET PUBLIC

    SUPPARAM.REPORT.NUM% = 628
    SUPPARAM.RECL%       = 40
    SUPPARAM.FILE.NAME$  = "ADXLXACN::D:\ADX_UDT1\SUPPARAM.BIN"
    SUPPARAM.REC.NO% = 1

   END FUNCTION
   
!*****************************************************************************   

   FUNCTION READ.SUPPARAM PUBLIC

   INTEGER*2 READ.SUPPARAM
   READ.SUPPARAM = 1

   IF END #SUPPARAM.SESS.NUM% THEN READ.SUPPARAM.ERROR
   READ FORM "C40" ; #SUPPARAM.SESS.NUM%, SUPPARAM.REC.NO%; SUPPARAM.RECORD$
   
   SUPPARAM.ACTIVE.FLAG$         = LEFT$(SUPPARAM.RECORD$,1)
   SUPPARAM.LAST.SALE.CHECK$     = STR$(VAL(MID$(SUPPARAM.RECORD$,2,3)))
   SUPPARAM.INITIAL.LOAD.CHECK$  = STR$(VAL(MID$(SUPPARAM.RECORD$,5,3)))
   SUPPARAM.INC.DEC.FLAG$        = MID$(SUPPARAM.RECORD$,8,1)
   SUPPARAM.INITIAL.LOAD.DATE$   = MID$(SUPPARAM.RECORD$,9,6)
   SUPPARAM.LAST.SALE.INC.CHECK$ = MID$(SUPPARAM.RECORD$,15,3)  ! BBG
   SUPPARAM.FILLER$              = MID$(SUPPARAM.RECORD$,18,23) ! BBG
   
   READ.SUPPARAM = 0
   EXIT FUNCTION

   READ.SUPPARAM.ERROR:

   CURRENT.REPORT.NUM% = SUPPARAM.REPORT.NUM%
   FILE.OPERATION$ = "R"
   CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(SUPPARAM.REC.NO%)),7)

   EXIT FUNCTION
  END FUNCTION

!**********************************************************************************

  FUNCTION WRITE.SUPPARAM PUBLIC

   INTEGER*2 WRITE.SUPPARAM
   
   SUPPARAM.LAST.SALE.CHECK$ = RIGHT$("000" + SUPPARAM.LAST.SALE.CHECK$,3)
   SUPPARAM.INITIAL.LOAD.CHECK$ = RIGHT$("000" + SUPPARAM.INITIAL.LOAD.CHECK$,3)
   
   SUPPARAM.RECORD$ = SUPPARAM.ACTIVE.FLAG$         +     \
                      SUPPARAM.LAST.SALE.CHECK$     +     \
                      SUPPARAM.INITIAL.LOAD.CHECK$  +     \
                      SUPPARAM.INC.DEC.FLAG$        +     \
                      SUPPARAM.INITIAL.LOAD.DATE$   +     \
                      SUPPARAM.LAST.SALE.INC.CHECK$ +     \ ! BBG
                      SUPPARAM.FILLER$
   
   WRITE.SUPPARAM = 1

   IF END #SUPPARAM.SESS.NUM% THEN WRITE.SUPPARAM.ERROR
   WRITE FORM "C40" ; #SUPPARAM.SESS.NUM%, SUPPARAM.REC.NO%; SUPPARAM.RECORD$

   WRITE.SUPPARAM = 0
   
   EXIT FUNCTION
  
  WRITE.SUPPARAM.ERROR:

   CURRENT.REPORT.NUM% = SUPPARAM.REPORT.NUM%
   FILE.OPERATION$ = "W"
   CURRENT.CODE$ = RIGHT$(PACK$("00000000000000"+STR$(SUPPARAM.REC.NO%)),7)

   EXIT FUNCTION
  END FUNCTION
