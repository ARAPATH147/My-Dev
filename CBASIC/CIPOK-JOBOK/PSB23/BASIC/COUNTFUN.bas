\******************************************************************************
\******************************************************************************
\***
\***             COUNTRY FILE (USED AS PART OF INTERNATIONAL RETAIL)    
\***                                                                
\***                      REFERENCE : COUNTFUN
\***                                                                
\***                      FILE TYPE : SEQUENTIAL
\***                                                                
\***                                                                
\***   Version A.              Jamie Thorpe                  7th February 2000
\***   Original version.   
\***
\***   Version B               Andy Cotton                   8th May 2000
\***   Added additional fields for sale labels
\***
\***                                                                       
\******************************************************************************
\******************************************************************************

       %INCLUDE COUNTDEC.J86 ! COUNTRY variable declarations      

       INTEGER*2 GLOBAL        CURRENT.REPORT.NUM%             
                        
       STRING GLOBAL           FILE.OPERATION$  

\****************************** SET FUNCTION **********************************

       FUNCTION COUNTRY.SET PUBLIC

           INTEGER*2 COUNTRY.SET
    
           COUNTRY.SET = 1

           COUNTRY.FILE.NAME$  = "ADXLXACN::D:\ADX_UDT1\COUNTRY.BIN"
           COUNTRY.REPORT.NUM% =  595

           COUNTRY.SET = 0

       END FUNCTION

\******************************* READ FUNCTION ************************************

       FUNCTION READ.COUNTRY PUBLIC

           INTEGER*2 READ.COUNTRY

           READ.COUNTRY = 1
           
           IF END # COUNTRY.SESS.NUM% THEN READ.COUNTRY.IF.END
       
           READ # COUNTRY.SESS.NUM%;COUNTRY.POUND.SIGN$,COUNTRY.PENCE.SIGN$,COUNTRY.HELPDESK.NO$,COUNTRY.WAS$, \  BAC
                  COUNTRY.NOW$,COUNTRY.SAVE$,COUNTRY.LOWER.NOW$,COUNTRY.WHILE$ ! BAC

           READ.COUNTRY = 0

       EXIT FUNCTION

READ.COUNTRY.IF.END:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = COUNTRY.REPORT.NUM%
    
       END FUNCTION                
