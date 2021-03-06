%INCLUDE PSBF01G.J86
%INCLUDE PSBF20G.J86
%INCLUDE IDFDEC.J86  
%INCLUDE IRFDEC.J86



%INCLUDE PSBF01E.J86                                                   !HSMG
%INCLUDE IDFEXT.J86                                                    !BMJK                                                   
%INCLUDE PSBF20E.J86
%INCLUDE IRFEXT.J86



ON ERROR GOTO ERROR.DETECTED

	GOSUB INITIALIZATION
	GOSUB MAIN.PROGRAM
	GOSUB TERMINATION

STOP

INITIALIZATION:

	 CALL IDF.SET
	 CALL IRF.SET

	 FUNCTION.FLAG$ EQ "O" 

     PASSED.INTEGER% EQ IDF.REPORT.NUM% 
     PASSED.STRING$ EQ IDF.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     IDF.SESS.NUM% EQ F20.INTEGER.FILE.NO% 
	 
	 PASSED.INTEGER% EQ IRF.REPORT.NUM% 
     PASSED.STRING$ EQ IRF.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     IRF.SESS.NUM% EQ F20.INTEGER.FILE.NO% 
	 

RETURN

CALL.F20.SESS.NUM.UTILITY: 
        
		
         FILE.OPERATION$ = FUNCTION.FLAG$
         CURRENT.REPORT.NUM% = PASSED.INTEGER%
         RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
         IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN



MAIN.PROGRAM:

	IF END #IDF.SESS.NUM% THEN NO.FILE
	OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL% AS IDF.SESS.NUM% NODEL
	
	IF END #IRF.SESS.NUM% THEN NO.FILE
	OPEN IRF.FILE.NAME$ KEYED RECL IRF.RECL% AS IRF.SESS.NUM% NODEL

	CMD$ = COMMAND$
	
	IF CMD$ = "" THEN BEGIN
	
	    PRINT " No parameter passed, please give 7 digit item code"
		
    ENDIF ELSE BEGIN
      
	  IDF.BOOTS.CODE$ = PACK$("0" + CMD$) 
	  
	  RC% = READ.IDF
	  
	  IF RC% = 0 THEN BEGIN
	  BAR.CODE.FRONT$ = PACK$("2000000000")
	  
	  
	  IRF.BAR.CODE$ = BAR.CODE.FRONT$ + IDF.FIRST.BAR.CODE$
	   
	   RC% = READ.IRF

       IF RC% = 0 THEN BEGIN
	   
	      
	   
	       IRF.INDICAT1% = IRF.INDICAT1%  OR 00000010B
		   
		   RC% = WRITE.IRF
		  
		  
	   ENDIF ELSE BEGIN
	   
	     IRF.BAR.CODE$ = BAR.CODE.FRONT$ + IDF.SECOND.BAR.CODE$
	   
	       RC% = READ.IRF

           IF RC% = 0 THEN  BEGIN
	   
	        
		   
		  
		   
		  IRF.INDICAT1% = IRF.INDICAT1%  OR 00000010B
		  
		   RC% = WRITE.IRF
		  
		  
		 
			  
		  ENDIF 
		   
		   
		   
		    IF RC% = 0 THEN BEGIN
			
			   
			   PRINT "WRITE IS SUCCESSFUL"
			   
			    RC% = READ.IRF
		       PRINT IRF.INDICAT1%
			   
			ENDIF
		   
	   ENDIF

	   

      ENDIF  
          
	ENDIF    			
      
      

RETURN




NO.FILE:

	PRINT " NO IDF FILE FOUND"




TERMINATION:

    FUNCTION.FLAG$ EQ "C"
	PASSED.INTEGER% EQ IDF.REPORT.NUM% 
    PASSED.STRING$ EQ IDF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    IDF.SESS.NUM% EQ F20.INTEGER.FILE.NO%
	
	PASSED.INTEGER% EQ IRF.REPORT.NUM% 
    PASSED.STRING$ EQ IRF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    IRF.SESS.NUM% EQ F20.INTEGER.FILE.NO%
 

RETURN



ERROR.DETECTED:

PRINT   "An Error Occurred "   + ERR   + STR$(ERRF%)   + STR$(ERRL)

END



