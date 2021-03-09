\******************************************************************************
\******************************************************************************
\***
\***                     REDEEM ITEMS CHANGE FILE    
\***                                                                
\***                      REFERENCE : RICFFUN 
\***                                                                
\***                      FILE TYPE : Direct                 
\***                                                                
\***                                                                
\***   Version A.              Jamie Thorpe                  1st July 1997
\***   Original version.   
\***
\***   Version B              Rebecca Dakin                   20th July 1999
\***   Fields added for processing of Unit Pricing information.                                    
\***                                                                       
\******************************************************************************
\******************************************************************************

       %INCLUDE RICFDEC.J86 ! RICF variable declarations      

       INTEGER*2 GLOBAL        CURRENT.REPORT.NUM%             
                        
       STRING GLOBAL           FILE.OPERATION$  

\****************************** SET FUNCTION **********************************

       FUNCTION RICF.SET PUBLIC

           INTEGER*2 RICF.SET
    
           RICF.SET = 1

           RICF.FILE.NAME$  = "ADXLXACN::D:\ADX_UDT1\RICF.BIN"
           RICF.REPORT.NUM% =  513
           RICF.RECL% = 80                                 ! BRD

           RICF.SET = 0

       END FUNCTION

\******************************* READ FUNCTION ************************************

       FUNCTION READ.RICF PUBLIC

           INTEGER*2 READ.RICF

           READ.RICF = 1

           IF END # RICF.SESS.NUM% THEN READ.RICF.IF.END
           READ FORM "C4,C1,C1,C45,C4,C3,C4,C2,C10,C6" ; # RICF.SESS.NUM%, RICF.RECORD.NO% ; \
                       RICF.ITEM.CODE$,                 \
                       RICF.DELIVERY.FLAG$,             \
                       RICF.REDEEM.ITEM$,               \
                       RICF.ITEM.DESCRIPTION$,          \
                       RICF.PRICE$,                     \
                       RICF.PRODUCT.GROUP$,             \
                       RICF.ITEM.QTY$,                  \ BRD
                       RICF.UNIT.MEASUREMENT$,          \ BRD
                       RICF.UNIT.NAME$,                 \ BRD
                       RICF.FILLER$                  
       
           READ.RICF = 0

       EXIT FUNCTION

READ.RICF.IF.END:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = RICF.REPORT.NUM%
    
       END FUNCTION                

\****************************** WRITE FUNCTION ***************************************

   FUNCTION WRITE.RICF PUBLIC

       INTEGER*2 WRITE.RICF

       WRITE.RICF = 1

       IF END#RICF.SESS.NUM% THEN WRITE.RICF.ERROR
       WRITE FORM "C4,C1,C1,C45,C4,C3,C4,C2,C10,C6" ; # RICF.SESS.NUM%, RICF.RECORD.NO% ; \
                       RICF.ITEM.CODE$,                 \
                       RICF.DELIVERY.FLAG$,             \
                       RICF.REDEEM.ITEM$,               \
                       RICF.ITEM.DESCRIPTION$,          \
                       RICF.PRICE$,                     \
                       RICF.PRODUCT.GROUP$,             \
                       RICF.ITEM.QTY$,                  \ BRD
                       RICF.UNIT.MEASUREMENT$,          \ BRD
                       RICF.UNIT.NAME$,                 \ BRD
                       RICF.FILLER$                  
       
       WRITE.RICF = 0

   EXIT FUNCTION

   WRITE.RICF.ERROR:

   CURRENT.REPORT.NUM% = RICF.REPORT.NUM%
   FILE.OPERATION$ = "W"

  END FUNCTION                                                                      
