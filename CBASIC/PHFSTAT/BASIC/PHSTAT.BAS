
\************************************************************************
\***
\***    PHSTAT            Ranjith Gopalankutty           20/05/2019 
\***                                                                
\***    There was a defect with program SRP10. It supposed to house 
\***    keep the expired planner records from SRPOG,SRMOD,SRSXF     
\***    and SRPDF files. SRPOG is the master planner file SRMOD     
\***    is module,SRSXF is notch data and SRPDF is the planner      
\***    descriptor file.  SRMOD,SRPDF and SRSXF files depends on    
\***    the master planner file SRPOG to build the keys for them.   
\***                                                                
 
\***********************************************************************
\***
\***    Function globals
\***
\***********************************************************************

%INCLUDE BEMFDEC.J86 
%INCLUDE BOOTSDEC.J86
%INCLUDE PSBF01G.J86 
%INCLUDE PSBF20G.J86 
%INCLUDE SOFTSDEC.J86         ! SOFTS File Functions
%INCLUDE RFSCFDEC.J86

\***********************************************************************
\*
\*    Global variable definitions
\*
\***********************************************************************

STRING     GLOBAL        \
     FILE.OPERATION$,    \
     PINFO.TABLE$,       \
     PASSED.STRING$,     \
     STRING.RECORD$,     \
     SVC.PARAM$           

INTEGER*4  GLOBAL        \
   ADX.FUNCTION%,        \
   ADXSERVE.RETURN.CODE%,\
   ADX.PARAM%,           \
   CURRENT.REPORT.NUM%,  \
   EVENT.NO%,            \
   MATCH%,               \
   PASSED.INTEGER%,      \
   RC%
   
\***********************************************************************
\*
\*    Local Variables
\*
\***********************************************************************

STRING                    \
     ADX.COMMAND$,        \
	 ADXSERVE.DATA$,      \
     COMM.TAIL$,          \
     CURRENT.CODE$,       \
     FLAG$,               \
     FUNCTION.FLAG$,      \
     OK.STRING$,          \
     PROG.NAME$,          \
     RCD$,                \
     SEC$,                \
     SPACE$,              \
     STORE.NUMBER$,       \
     VAR.STRING.1$,       \
     VAR.STRING.2$         
                           
INTEGER*4                 \
     ADX.INTEGER%,        \
     ARR.COUNT%,          \
     I%,                  \
     EOF%,                \
     ERROR.CNT%,          \
     K%,                  \
     FOUND%,              \
     FOUND1%,             \
     J%,                  \
     MESSAGE.NO%,         \
     TEMP.ARR%(1),        \
     TEMP.COUNT%           

\***********************************************************************
\*
\*    Included code defining external Boots functions
\*
\***********************************************************************
    
%INCLUDE BEMFEXT.J8 6
%INCLUDE BASROUT.J86 
%INCLUDE BOOTSEXT.J86
%INCLUDE BTCMEM.J86  
%INCLUDE PSBF01E.J86 
%INCLUDE PSBF20E.J86 
%INCLUDE PSBF24E.J86 
%INCLUDE SOFTSEXT.J86         ! SOFTS File Functions
%INCLUDE RFSCFEXT.J86
%INCLUDE ADXSERVE.J86   !ADXSERVE

\***********************************************************************
\*
\*    INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************

FUNCTION FLEXOSSVC(COMMAND%, PARAM$) EXTERNAL                           
                                                                        
      STRING PARAM$                                                     
      INTEGER*2 COMMAND%, FLEXOSSVC                                     
                                                                        
END FUNCTION                                                            

\**********************************************************************
\***
\***    ADXSERVE
\***    IBM routine to request Application Services.
\***
\**********************************************************************

!SUB ADXSERVE (RET, FUNC, PARM1, PARM2$) EXTERNAL

!    INTEGER*4  RET
!    INTEGER*2  FUNC,PARM1
!    STRING     PARM2$

!END SUB

\**********************************************************************
\***
\***    ADXCOPYF
\***    IBM routine to request Application Services.
\***
\**********************************************************************

SUB ADXCOPYF (RETC,INFILE,OUTFILE,OPT0,OPT1) EXTERNAL

    INTEGER*4 RETC
    STRING    INFILE,OUTFILE
    INTEGER*2 OPT0,OPT1

END SUB 

\***********************************************************************
\*                                                                     *
\*          S T A R T   O F   M A I N L I N E   C O D E                *
\*                                                                     *
\***********************************************************************
\***********************************************************************

ON ERROR GOTO ERROR.DETECTED       

    GOSUB INITIALISATION 
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION
 
STOP.PROGRAM:
    STOP 

\***********************************************************************
\*
\*    INITIALISE.VARIABLES :   
\*                
\*
\***********************************************************************
INITIALISATION: 
    
   CALL SOFTS.SET
   CALL RFSCF.SET
  ! CALL SOPTS.SET
  ! CALL RFSCF.SET      
   GOSUB ALLOCATE.SESSION.NUMBERS
   
RETURN 

\***********************************************************************
\*
\*    ALLOCATE.SESSION.NUMBERS:   
\*                
\*
\***********************************************************************
ALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ = "O"  

    PASSED.INTEGER% = SOFTS.REPORT.NUM%                                  
    PASSED.STRING$  = SOFTS.FILE.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                      
    SOFTS.SESS.NUM% = F20.INTEGER.FILE.NO% 

	PASSED.INTEGER% = RFSCF.REPORT.NUM%                                  
    PASSED.STRING$  = RFSCF.FILE.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                      
    RFSCF.SESS.NUM% = F20.INTEGER.FILE.NO% 	                             
    	
                             

RETURN

\***********************************************************************
\*
\*    CALL.F20.SESS.NUM.UTILITY:  
\*                
\*
\***********************************************************************
CALL.F20.SESS.NUM.UTILITY: 
        
   FILE.OPERATION$ = FUNCTION.FLAG$
   CURRENT.REPORT.NUM% = PASSED.INTEGER%
   RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
   IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN 


\***********************************************************************
\*
\*    MAIN.PROCESSING
\*
\***********************************************************************
MAIN.PROCESSING:       

GOSUB SOFTS.READ
!GOSUB RFSCF.READ
!GOSUB SOPTS.READ
!GOSUB ENVOY.READ
!GOSUB ADXSERVE.CHECKS

RETURN

\***********************************************************************
\*
\*    SOFTS.READ
\*
\***********************************************************************
SOFTS.READ: 


     CALL ADXSERVE (ADXSERVE.RETURN.CODE%,4,0,ADXSERVE.DATA$)
     IF ADXSERVE.RETURN.CODE% <> 0 THEN BEGIN    
         PRINT "Unable to get store number"      
         
     ENDIF

     STORE.NUMBER$ = LEFT$(ADXSERVE.DATA$,4)

     PRINT "Store Number is " + STORE.NUMBER$

     IF END #SOFTS.SESS.NUM% THEN FILE.ERROR
     OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%                   \
                             AS SOFTS.SESS.NUM% NODEL

     OPEN RFSCF.FILE.NAME$ DIRECT RECL RFSCF.RECL%  AS RFSCF.SESS.NUM% NODEL

     !Take the value of RFSCF HTT configuration
     RC% = READ.RFSCF1	
      
     SOFTS.REC.NUM% = 1 
     RC% = READ.SOFTS
     IF RC% = 0 THEN BEGIN	 
         PRINT "Current build level is" + " " + MID$(SOFTS.RECORD$,28,3)
     ENDIF 

     SOFTS.REC.NUM% = 19
     RC% = READ.SOFTS
     IF RC% = 0 THEN BEGIN
         PRINT "Configured as " + LEFT$(SOFTS.RECORD$,3) + " " + "Store"
     ENDIF

     SOFTS.REC.NUM% = 20
     RC% = READ.SOFTS
     IF RC% = 0 THEN BEGIN
	 
	    ! Check for specific HHT device type using RFSCF value as well
		
		 MATCH% = MATCH("RF IS ACTIVE",SOFTS.RECORD$,1)
		 
		  
		
		! Now confirm the device type
		 IF MATCH% > 0 THEN BEGIN
		
		      IF RFSCF.ACTIVITY% = 52 THEN BEGIN
			   
			     PRINT "Store has RF/PPC device configuration/MC55A"

              ENDIF ELSE BEGIN

                 PRINT "Store is set as RF in SOFTSTAT record but RFSCF value does not match, please correct the same"

              ENDIF				 
		
	     ENDIF ELSE BEGIN
		
			 MATCH% = MATCH(" RF IS INACTIVE   NETWORK PDT IS ACTIVE (PHASE TWO)",SOFTS.RECORD$,1)
			 
			 
			 
			 IF MATCH% > 0 THEN BEGIN
			 
			     IF RFSCF.ACTIVITY% = 53 THEN BEGIN
				  
				     PRINT "Store has batch/POD configuration - MC70/MC55BH "
			
				 ENDIF ELSE BEGIN	 
			 
			         PRINT "Store is set as batch in SOFTSTAT but RFSCF value does not match, please correct the same"
					  
				 ENDIF
		
			 ENDIF ELSE BEGIN
			 
				 MATCH% = MATCH("RF IS INACTIVE" ,SOFTS.RECORD$,1)			 
				  
			     
				 IF MATCH% > 0 THEN BEGIN
				 
					 IF RFSCF.ACTIVITY% = 48 THEN BEGIN					 
				         PRINT("Store has PDT6100 configuration")
                     ENDIF ELSE BEGIN

                         PRINT ("Store is set as PDT but RFSCF value does not match, please correct the same")
                     ENDIF					  
		  	 
		         ENDIF 
		     ENDIF
		 
	      ENDIF	 
		    
     ENDIF
	       
 

	 SOFTS.REC.NUM% = 55
	 RC% = READ.SOFTS

	 MATCH% = MATCH("SINGLE",SOFTS.RECORD$,1)
	 
	     IF MATCH% > 0 THEN BEGIN 
	         SOFTS.REC.NUM% = 68
	         RC% = READ.SOFTS
			
	         PRINT ("Set as single controller store with MCF PHASE") + MID$(SOFTS.RECORD$,11,1)
		     
		 ENDIF ELSE BEGIN
		     SOFTS.REC.NUM% = 68
	         RC% = READ.SOFTS
			      PRINT ("Set as MCF store with MCF PHASE ") + MID$(SOFTS.RECORD$,11,2)
			  
		 ENDIF

RETURN
  
FILE.ERROR:

PRINT " NO FILE EXISTS"
RETURN  

\***********************************************************************
\*
\*    TERMINATION
\*
\***********************************************************************
 
TERMINATION:    
 
   
   GOSUB DEALLOCATE.SESS.NUM
     

RETURN
 

\***********************************************************************
\*
\*    ERROR.LOG  
\*               
\*
\***********************************************************************
ERROR.LOG: 
        
         VAR.STRING.1$ = FILE.OPERATION$                     +          \
                CHR$(SHIFT(CURRENT.REPORT.NUM%, 8) AND 0FFH) +          \
                CHR$(CURRENT.REPORT.NUM% AND 0FFH)           +          \
                CURRENT.CODE$
         VAR.STRING.2$ = "PHKEEP"
         MESSAGE.NO%   = 0
         EVENT.NO%     = 106

         RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,               \
		 VAR.STRING.2$,EVENT.NO%)

        

         GOTO END.PROGRAM

RETURN

\***********************************************************************
\*
\*    CLOSE.FILES:
\*                
\*
\***********************************************************************

CLOSE.FILES:
   
    CLOSE SOFTS.SESS.NUM%
   
RETURN  
\***********************************************************************
\*
\*    DEALLOCATE.SESS.NUM  
\*                
\*
\***********************************************************************
DEALLOCATE.SESS.NUM: 

    FUNCTION.FLAG$ = "C"   
    
    PASSED.INTEGER% = SOFTS.REPORT.NUM%                                
    PASSED.STRING$  = SOFTS.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SOFTS.SESS.NUM% = F20.INTEGER.FILE.NO%                           


RETURN

\***********************************************************************
\*
\*    ERROR.DETECTED  
\*              
\*
\***********************************************************************
ERROR.DETECTED:     

print "erroring out"

print err
print errl
print errf%
        
     IF ERR = "OE" AND ERRF% =  SOFTS.SESS.NUM% THEN BEGIN          
     ENDIF    
   
     GOSUB ERROR.LOG
   
RETURN  

\***********************************************************************
\*
\*    END.PROGRAM 
\*              
\*
\***********************************************************************

   END.PROGRAM:

   END 
       

