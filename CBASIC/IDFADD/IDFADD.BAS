%INCLUDE PSBF01G.J86
%INCLUDE PSBF20G.J86
%INCLUDE IDFDEC.J86  



%INCLUDE PSBF01E.J86                                                   !HSMG
%INCLUDE IDFEXT.J86                                                    !BMJK                                                   
%INCLUDE PSBF20E.J86



ON ERROR GOTO ERROR.DETECTED

	GOSUB INITIALIZATION
	GOSUB MAIN.PROGRAM
	GOSUB TERMINATION

STOP

INITIALIZATION:

	 CALL IDF.SET

	 FUNCTION.FLAG$ EQ "O" 

     PASSED.INTEGER% EQ IDF.REPORT.NUM% 
     PASSED.STRING$ EQ IDF.FILE.NAME$
     GOSUB CALL.F20.SESS.NUM.UTILITY
     IDF.SESS.NUM% EQ F20.INTEGER.FILE.NO% 
	 

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

	CMD$ = COMMAND$
	
	IF CMD$ = "" THEN BEGIN
	
	    PRINT " No parameter passed, please give 7 digit item code"
		
    ENDIF ELSE BEGIN
      
	  IDF.BOOTS.CODE$ = PACK$("0" + CMD$) 
	  
	  RC% = READ.IDF
	  
	  IF RC% = 0 THEN BEGIN
	  
	     PRINT "Item already Exists program ending"
		 
		 RETURN
		 
	  ENDIF ELSE BEGIN

         PRINT "Item does not exists, Adding the Line"

      ENDIF  
          
		 IDF.BIT.FLAGS.2% = IDF.BIT.FLAGS.2%  OR 00001000B 
         RC% = WRITE.IDF

         IF RC% = 0 THEN BEGIN

            PRINT "Item has been added, now restart stock support"
         ENDIF			
      
      ENDIF	  

RETURN




NO.FILE:

	PRINT " NO IDF FILE FOUND"

RETURN


TERMINATION:

    FUNCTION.FLAG$ EQ "C"
	PASSED.INTEGER% EQ IDF.REPORT.NUM% 
    PASSED.STRING$ EQ IDF.FILE.NAME$
    GOSUB CALL.F20.SESS.NUM.UTILITY
    UODOT.SESS.NUM% EQ F20.INTEGER.FILE.NO%

	CLOSE IDF.SESS.NUM%

RETURN



ERROR.DETECTED:

PRINT   "An Error Occurred "   + ERR   + STR$(ERRF%)   + STR$(ERRL)

END



