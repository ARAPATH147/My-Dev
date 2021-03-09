
\******************************************************************************
\***
\***      PIITM - ITEM LIST FILE - FILE FUNCTIONS
\***
\***           Reference: PIITMFUN.BAS
\***
\***
\******************************************************************************

   INTEGER*2 GLOBAL             \
      CURRENT.REPORT.NUM%
      
   STRING GLOBAL                \
      CURRENT.CODE$,            \
      FILE.OPERATION$
      
   %INCLUDE PIITMDEC.J86
   
   
   FUNCTION PIITM.SET PUBLIC
\****************************

     PIITM.FILE.NAME$  = "PIITM"
     PIITM.REPORT.NUM% = 121
     PIITM.RECL%       = 30   

   END FUNCTION
   
\-----------------------------------------------------------------------------
   
   FUNCTION READ.PIITM PUBLIC
\*****************************

      INTEGER*2 READ.PIITM
      
      READ.PIITM = 1   

      IF END #PIITM.SESS.NUM% THEN READ.ERROR
      READ FORM "T7,C4,2C1,3C2,3C1,2I2,C5";                             \
        # PIITM.SESS.NUM%                                               \
        KEY PIITM.LIST.ITEM.KEY$;                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$
   
        READ.PIITM = 0
        EXIT FUNCTION
        
        READ.ERROR:
        
           CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
           FILE.OPERATION$ = "R"
           CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
           
           EXIT FUNCTION 
   
   END FUNCTION
\-----------------------------------------------------------------------------

  FUNCTION READ.PIITM.LOCK PUBLIC
\*********************************   

      INTEGER*2 READ.PIITM.LOCK
     
      READ.PIITM.LOCK = 1
     
      IF END #PIITM.SESS.NUM% THEN READ.LOCK.ERROR
      READ FORM "T7,C4,2C1,3C2,3C1,2I2,C5";                             \
        # PIITM.SESS.NUM% AUTOLOCK                                              \
        KEY PIITM.LIST.ITEM.KEY$;                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$
       
      READ.PIITM.LOCK = 0 
      EXIT FUNCTION
      
      READ.LOCK.ERROR:
      
         CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
         FILE.OPERATION$ = "R"
         CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
         
         EXIT FUNCTION 
   
   END FUNCTION
\-----------------------------------------------------------------------------


  FUNCTION WRITE.PIITM PUBLIC
\*****************************   

      INTEGER*2 WRITE.PIITM
      
      WRITE.PIITM = 1
      
      IF END #PIITM.SESS.NUM% THEN WRITE.ERROR
      WRITE FORM "C6,C4,2C1,3C2,3C1,2I2,C5";                            \
        # PIITM.SESS.NUM%;                                              \
            PIITM.LIST.ITEM.KEY$,                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$

      WRITE.PIITM = 0
      EXIT FUNCTION
      
      WRITE.ERROR:
      
         CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
         
         EXIT FUNCTION

   END FUNCTION
\-----------------------------------------------------------------------------
   

  FUNCTION WRITE.PIITM.UNLOCK PUBLIC
\************************************   

      INTEGER*2 WRITE.PIITM.UNLOCK
      
      WRITE.PIITM.UNLOCK = 1
      
      IF END #PIITM.SESS.NUM% THEN WRITE.UNLOCK.ERROR
      WRITE FORM "C6,C4,2C1,3C2,3C1,2I2,C5";                            \
            # PIITM.SESS.NUM% AUTOUNLOCK;                               \
            PIITM.LIST.ITEM.KEY$,                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$

      WRITE.PIITM.UNLOCK = 0
      EXIT FUNCTION
      
      WRITE.UNLOCK.ERROR:
      
         CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
         
         EXIT FUNCTION
         
   END FUNCTION
\-----------------------------------------------------------------------------


  FUNCTION WRITE.PIITM.HOLD.UNLOCK PUBLIC
\*****************************************   

      INTEGER*2 WRITE.PIITM.HOLD.UNLOCK
      
      WRITE.PIITM.HOLD.UNLOCK = 1
      
      IF END #PIITM.SESS.NUM% THEN WRITE.HOLD.UNLOCK.ERROR
      WRITE FORM "C6,C4,2C1,3C2,3C1,2I2,C5"; HOLD                       \
            # PIITM.SESS.NUM% AUTOUNLOCK;                               \
            PIITM.LIST.ITEM.KEY$,                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$
      WRITE.PIITM.HOLD.UNLOCK = 0
      EXIT FUNCTION
      
      WRITE.HOLD.UNLOCK.ERROR:
      
         CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
         
         EXIT FUNCTION
         
   END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION WRITE.PIITM.HOLD PUBLIC
\**********************************   

      INTEGER*2 WRITE.PIITM.HOLD
      
      WRITE.PIITM.HOLD = 1
      
      IF END #PIITM.SESS.NUM% THEN WRITE.HOLD.ERROR
      WRITE FORM "C6,C4,2C1,3C2,3C1,2I2,C5";                            \
        HOLD # PIITM.SESS.NUM%;                                         \
            PIITM.LIST.ITEM.KEY$,                                       \
            PIITM.ITEM.CODE$,                                           \
            PIITM.ON.IDF$,                                              \
            PIITM.ACTIVITY.FLAG$,                                       \
            PIITM.CYCLE.LENGTH$,                                        \
            PIITM.FAMILY.MARKER$,                                       \
            PIITM.MEMBERS$,                                             \
            PIITM.ELIGIBILITY.FLAG$,                                    \
            PIITM.LIST.STATUS$,                                         \
            PIITM.POTENTIAL.RECOUNT$,                                   \
            PIITM.DISCREPANCY%,                                         \
            PIITM.DISCREPANCY.AMT%,                                     \
            PIITM.FILLER$
      WRITE.PIITM.HOLD = 0
      EXIT FUNCTION
      
      WRITE.HOLD.ERROR:
      
         CURRENT.CODE$ = PIITM.LIST.ITEM.KEY$
         FILE.OPERATION$ = "O"
         CURRENT.REPORT.NUM% = PIITM.REPORT.NUM%
         
         EXIT FUNCTION
      
   END FUNCTION
