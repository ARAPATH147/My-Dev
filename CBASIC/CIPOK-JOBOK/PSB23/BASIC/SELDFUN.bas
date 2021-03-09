\******************************************************************************
\******************************************************************************
\***
\***             SEL DESCRIPTION FILE USED FOR PAINKILLER WARNINGS    
\***                                                                
\***                      REFERENCE : SELDFUN
\***                                                                
\***                      FILE TYPE : SEQUENTIAL
\***                                                                
\***
\***   Version A               Andy Cotton                   8th April 2003
\***   
\***                                                                       
\******************************************************************************
\******************************************************************************

       %INCLUDE SELDDEC.J86 ! COUNTRY variable declarations      

       INTEGER*2 GLOBAL        CURRENT.REPORT.NUM%             
                        
       STRING GLOBAL           FILE.OPERATION$  

\****************************** SET FUNCTION **********************************

       FUNCTION SELDESC.SET PUBLIC

           INTEGER*2 SELDESC.SET
    
           SELDESC.SET = 1

           SELDESC.FILE.NAME$  = "ADXLXACN::D:\ADX_UDT1\SELDESC.BIN"
           SELDESC.REPORT.NUM% =  665
           
           DIM SELDESC$(20)

           SELDESC.SET = 0

       END FUNCTION

\******************************* READ FUNCTION ************************************

       FUNCTION READ.SELDESC PUBLIC

           INTEGER*2 READ.SELDESC,  \
                     INDEX%,        \
                     OFFSET%
                     
           STRING SELDESC.RECORD$

           ON ERROR GOTO READ.SELDESC.ERROR
           
           IF END # SELDESC.SESS.NUM% THEN END.OF.FILE

           INDEX% = 1
       
           WHILE -1

             READ # SELDESC.SESS.NUM%; LINE SELDESC.RECORD$
           
             !Remove any comments from the end of the descriptions
              
              OFFSET% = MATCH("\#", SELDESC.RECORD$, 1)
              IF OFFSET% > 0 THEN BEGIN
                 SELDESC.RECORD$ = LEFT$(SELDESC.RECORD$, OFFSET% -1)
              ENDIF
              
              SELDESC$(INDEX%) = SELDESC.RECORD$
              INDEX% = INDEX% + 1
           WEND
           
END.OF.FILE:
        
        READ.SELDESC = 0
        
EXIT.FUNCTION:              

       EXIT FUNCTION

READ.SELDESC.ERROR:

       FILE.OPERATION$     = "R"
       CURRENT.REPORT.NUM% = SELDESC.REPORT.NUM%
       READ.SELDESC = 1
    
       RESUME EXIT.FUNCTION
       
END FUNCTION                
