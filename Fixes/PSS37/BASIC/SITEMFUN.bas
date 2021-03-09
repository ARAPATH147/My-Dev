REM \
\******************************************************************************
\******************************************************************************
\***
\***          SHELF EDGE LABELS INDIVIDUAL ITEM FILE FUNCTIONS
\***
\***                   REFERENCE    : SITEMFUN.BAS
\***               
\***   Version A               Jamie Thorpe            25th June 1997
\***   Removed version letter from SITEMDEC.
\***
\******************************************************************************
*******************************************************************************

  STRING GLOBAL              \
         CURRENT.CODE$,      \  
         FILE.OPERATION$,    \	 
         RPRT.LINE$	 

  INTEGER*1 GLOBAL           \
         CURRENT.REPORT.NUM%  
  

  %INCLUDE SITEMDEC.J86                                        ! AJT
  
  
  FUNCTION SITEM.SET PUBLIC
\***************************

     SELFITEM.REPORT.NUM% = 48           
     SELFITEM.FILE.NAME$  = "SITEM"
     
  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT       
    
  FUNCTION READ.SITEM PUBLIC
\****************************

    INTEGER*2 READ.SITEM 
    
    READ.SITEM = 1      

    IF END #SELFITEM.SESS.NUM% THEN READ.ERROR
    READ #SELFITEM.SESS.NUM%; RPRT.LINE$
    
    READ.SITEM = 0
    EXIT FUNCTION
    
    
    READ.ERROR:
    
       FILE.OPERATION$ = "R"
       CURRENT.REPORT.NUM% = SELFITEM.REPORT.NUM%
       CURRENT.CODE$ = PACK$("0000000000000000")
                         
       EXIT FUNCTION            
        

  END FUNCTION
\------------------------------------------------------------------------------
REM EJECT  

  FUNCTION WRITE.SITEM PUBLIC
\*****************************  

    INTEGER*2 WRITE.SITEM
    
    WRITE.SITEM = 1    


    IF END #SELFITEM.SESS.NUM% THEN WRITE.ERROR
    WRITE #SELFITEM.SESS.NUM%; RPRT.LINE$

    WRITE.SITEM = 0
    EXIT FUNCTION
    
    
    WRITE.ERROR:
    
       FILE.OPERATION$ = "W"
       CURRENT.REPORT.NUM% = SELFITEM.REPORT.NUM%
       CURRENT.CODE$ = PACK$("0000000000000000")                  
       EXIT FUNCTION         

  END FUNCTION
