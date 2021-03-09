\******************************************************************************
\******************************************************************************
\***
\***                    User Descriptors file function
\***                                                                
\***                      REFERENCE : UDESCFUN
\***                                                                
\***                      FILE TYPE : DIRECT    
\***                                                                
\***                                                                
\***   Version A.              Amy Hoggard                   19th May 2000
\***   Original version.   
\***
\***                                                                       
\******************************************************************************
\******************************************************************************

       %INCLUDE UDESCDEC.J86 !         variable declarations      

       INTEGER*2 GLOBAL        CURRENT.REPORT.NUM%             
                        
       STRING GLOBAL           FILE.OPERATION$  
       

\****************************** SET FUNCTION **********************************

       FUNCTION UDESC.SET PUBLIC

           INTEGER*2 UDESC.SET
    
           UDESC.SET = 1

           UDESC.FILE.NAME$  = "USERDESC"
           UDESC.REPORT.NUM% =  603
           UDESC.RECL%       =  49
           
           UDESC.SET = 0

       END FUNCTION

\******************************* READ FUNCTION ************************************

       FUNCTION READ.UDESC PUBLIC

           INTEGER*2 READ.UDESC

           READ.UDESC = 1
           
           IF END # UDESC.SESS.NUM% THEN READ.UDESC.IF.END
       
           READ FORM "C49"; #UDESC.SESS.NUM%, UDESC.RECORD.NUM%; UDESC.RECORD$     
           
           DESC.LEN% = VAL(MID$(UDESC.RECORD$,45,2))
           UDESC.RECORD$ = MID$(UDESC.RECORD$,2,DESC.LEN%)  

           READ.UDESC = 0

       EXIT FUNCTION

READ.UDESC.IF.END:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = UDESC.REPORT.NUM%
    
       END FUNCTION                
