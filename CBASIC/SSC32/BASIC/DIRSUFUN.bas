REM \
\******************************************************************************
\******************************************************************************
\***
\***          STORE DIRECT SUPPLIER FILE FUNCTIONS
\***
\***              REFERENCE    : DIRSUFUN.BAS
\***
\***   Version A           Mark Goode          2nd December 2008
\***   Add new fields for the +ve UOD project
\******************************************************************************
\******************************************************************************

  INTEGER*2 GLOBAL                   \
       CURRENT.REPORT.NUM%
  
  STRING GLOBAL                      \
       CURRENT.CODE$,                \
       FILE.OPERATION$
  
  %INCLUDE DIRSUDEC.J86
  
  FUNCTION DIRSUP.SET PUBLIC
\****************************

    DIRSUP.REPORT.NUM%  = 230
    DIRSUP.RECL%        = 40
    DIRSUP.FILE.NAME$   = "DIRSU"
    DIRSUP.NO.RECS%     = 6000
              
  END FUNCTION

\-----------------------------------------------------------------------------

  FUNCTION READ.DIRSUP.LOCK PUBLIC
\**********************************  

  INTEGER*2 READ.DIRSUP.LOCK

  STRING         FORMAT$

  READ.DIRSUP.LOCK = 1

    FORMAT$ = "T5,C10,6C2,C1,4C2,2C1,C3"                              ! AMG
    IF END #DIRSUP.SESS.NUM% THEN READ.LOCK.ERROR
    READ FORM FORMAT$; #DIRSUP.SESS.NUM% AUTOLOCK KEY DIRSUP.RECKEY$; \
                    DIRSUP.SUPPLIER.NAME$,                            \
                    DIRSUP.LEAD.TIME.MON$,                            \
                    DIRSUP.LEAD.TIME.TUE$,                            \
                    DIRSUP.LEAD.TIME.WED$,                            \
                    DIRSUP.LEAD.TIME.THU$,                            \ 
                    DIRSUP.LEAD.TIME.FRI$,                            \ 
                    DIRSUP.LAPSING.DAYS$,                             \ 
                    DIRSUP.PART.ORDER.RULES$,                         \ 
                    DIRSUP.MAX.CHECK.QTY$,                            \
                    DIRSUP.CHECK.QTY$,                                \
                    DIRSUP.DISCREPANCY.QTY$,                          \
                    DIRSUP.DISCREPANCY.PERC$,                         \
                    DIRSUP.ASN.FLAG$,                                 \ AMG
                    DIRSUP.STATIC.SUPPLIER$,                          \ AMG
                    DIRSUP.FILLER$
     
     READ.DIRSUP.LOCK = 0
     EXIT FUNCTION
     
     READ.LOCK.ERROR:
     
        CURRENT.CODE$ = DIRSUP.RECKEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = DIRSUP.REPORT.NUM%
        
        EXIT FUNCTION               
                    
  END FUNCTION
\-----------------------------------------------------------------------------


  FUNCTION READ.DIRSUP PUBLIC
\*****************************

  INTEGER*2 READ.DIRSUP
  
  STRING         FORMAT$

  READ.DIRSUP = 1

    IF END #DIRSUP.SESS.NUM% THEN READ.ERROR
    FORMAT$ = "T5,C10,6C2,C1,4C2,2C1,C3"                              ! AMG
    READ FORM FORMAT$; #DIRSUP.SESS.NUM% KEY DIRSUP.RECKEY$;          \
                    DIRSUP.SUPPLIER.NAME$,                            \
                    DIRSUP.LEAD.TIME.MON$,                            \
                    DIRSUP.LEAD.TIME.TUE$,                            \
                    DIRSUP.LEAD.TIME.WED$,                            \
                    DIRSUP.LEAD.TIME.THU$,                            \ 
                    DIRSUP.LEAD.TIME.FRI$,                            \ 
                    DIRSUP.LAPSING.DAYS$,                             \ 
                    DIRSUP.PART.ORDER.RULES$,                         \ 
                    DIRSUP.MAX.CHECK.QTY$,                            \
                    DIRSUP.CHECK.QTY$,                                \
                    DIRSUP.DISCREPANCY.QTY$,                          \
                    DIRSUP.DISCREPANCY.PERC$,                         \
                    DIRSUP.ASN.FLAG$,                                 \ AMG
                    DIRSUP.STATIC.SUPPLIER$,                          \ AMG
                    DIRSUP.FILLER$
                    
     READ.DIRSUP = 0
     EXIT FUNCTION
     
     READ.ERROR:
     
        CURRENT.CODE$ = DIRSUP.RECKEY$
        FILE.OPERATION$ = "R"
        CURRENT.REPORT.NUM% = DIRSUP.REPORT.NUM%
        
        EXIT FUNCTION
        
  END FUNCTION
\-----------------------------------------------------------------------------


  FUNCTION WRITE.DIRSUP.UNLOCK PUBLIC
\*************************************

  INTEGER*2 WRITE.DIRSUP.UNLOCK  

  STRING         FORMAT$
  
  WRITE.DIRSUP.UNLOCK = 1

    IF END #DIRSUP.SESS.NUM% THEN WRITE.UNLOCK.ERROR
    FORMAT$ = "C1,C3,C10,6C2,C1,4C2,2C1,C3"                           ! AMG
    WRITE FORM FORMAT$; #DIRSUP.SESS.NUM% AUTOUNLOCK;                 \
                    DIRSUP.BUS.CENTRE$,                               \
                    DIRSUP.SUPPLIER.NO$,                              \
                    DIRSUP.SUPPLIER.NAME$,                            \
                    DIRSUP.LEAD.TIME.MON$,                            \
                    DIRSUP.LEAD.TIME.TUE$,                            \
                    DIRSUP.LEAD.TIME.WED$,                            \
                    DIRSUP.LEAD.TIME.THU$,                            \ 
                    DIRSUP.LEAD.TIME.FRI$,                            \ 
                    DIRSUP.LAPSING.DAYS$,                             \ 
                    DIRSUP.PART.ORDER.RULES$,                         \ 
                    DIRSUP.MAX.CHECK.QTY$,                            \
                    DIRSUP.CHECK.QTY$,                                \
                    DIRSUP.DISCREPANCY.QTY$,                          \
                    DIRSUP.DISCREPANCY.PERC$,                         \
                    DIRSUP.ASN.FLAG$,                                 \ AMG
                    DIRSUP.STATIC.SUPPLIER$,                          \ AMG
                    DIRSUP.FILLER$
    
    WRITE.DIRSUP.UNLOCK = 0
    EXIT FUNCTION
    
    WRITE.UNLOCK.ERROR:
    
       CURRENT.CODE$ = DIRSUP.BUS.CENTRE$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = DIRSUP.REPORT.NUM%
       
       EXIT FUNCTION 

  END FUNCTION
\-----------------------------------------------------------------------------


  FUNCTION WRITE.DIRSUP PUBLIC
\******************************

  INTEGER*2 WRITE.DIRSUP  

  STRING     FORMAT$    
  
  WRITE.DIRSUP = 1    

    IF END #DIRSUP.SESS.NUM% THEN WRITE.ERROR
    FORMAT$ = "C1,C3,C10,6C2,C1,4C2,2C1,C3"                           ! AMG
    WRITE FORM FORMAT$; #DIRSUP.SESS.NUM%;                            \
                    DIRSUP.BUS.CENTRE$,                               \
                    DIRSUP.SUPPLIER.NO$,                              \
                    DIRSUP.SUPPLIER.NAME$,                            \
                    DIRSUP.LEAD.TIME.MON$,                            \
                    DIRSUP.LEAD.TIME.TUE$,                            \
                    DIRSUP.LEAD.TIME.WED$,                            \
                    DIRSUP.LEAD.TIME.THU$,                            \ 
                    DIRSUP.LEAD.TIME.FRI$,                            \ 
                    DIRSUP.LAPSING.DAYS$,                             \ 
                    DIRSUP.PART.ORDER.RULES$,                         \ 
                    DIRSUP.MAX.CHECK.QTY$,                            \
                    DIRSUP.CHECK.QTY$,                                \
                    DIRSUP.DISCREPANCY.QTY$,                          \
                    DIRSUP.DISCREPANCY.PERC$,                         \
                    DIRSUP.ASN.FLAG$,                                 \ AMG
                    DIRSUP.STATIC.SUPPLIER$,                          \ AMG
                    DIRSUP.FILLER$
                    
    WRITE.DIRSUP = 0 
    EXIT FUNCTION
    
    WRITE.ERROR:
    
       CURRENT.CODE$ = DIRSUP.BUS.CENTRE$
       FILE.OPERATION$ = "O"
       CURRENT.REPORT.NUM% = DIRSUP.REPORT.NUM%
       
       EXIT FUNCTION
                    
  END FUNCTION
