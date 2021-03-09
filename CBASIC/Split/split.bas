ON ERROR GOTO ERROR.DETECTED

EOF = FALSE
OPEN "D:\ADX_UDT1\DELVINDX.BIN" AS 200
CREATE POSFILE "D:/DELVINDX.TXT" AS 150
!CREATE POSFILE "C:/SUBTRACT.TXT" AS 100
POS%=1
PRINT "PROCESSING STARTED "
WHILE EOF = FALSE
   
   IF END #200 THEN FILE.END
   READ #200; LINE LINE.RECORD$
    
   
  ! FOR I = 1 TO 10
        
   !    POS% = MATCH(",",LINE.RECORD$,POS%)
	   
	!   POS% = POS% + 1
	   
	!   IF I = 10 THEN BEGIN
      
   !       VALUE$ = MID$(LINE.RECORD$,POS%,1)
    IF MID$(LINE.RECORD$,17,2) = "15" AND MID$(LINE.RECORD$,19,2) > =  "10"  THEN BEGIN
	      
	!	  IF VALUE$ = "1" OR VALUE$="4" THEN BEGIN
		      WRITE #150; MID$(LINE.RECORD$,2,    \
                                           (LEN(LINE.RECORD$)-2))
	!	  ENDIF ELSE IF VALUE$ = "2" OR VALUE$ = "3" THEN BEGIN
	!	      WRITE #100; S$(LINE.RECORD$,2,    \
                                           (LEN(LINE.RECORD$)-2))
    !      ENDIF ELSE BEGIN
	!	      PRINT "INVALID VALUE"
	!	  ENDIF
       ENDIF
	    
  ! NEXT I
   
  ! POS% = 1
    
   
WEND

FILE.END:
EOF = TRUE
PRINT "PROCESSING COMPLETED"
STOP
 



ERROR.DETECTED:
PRINT "ERROR HAPPENED"
 PRINT   "Fatal Error:" + ERR
         PRINT   "Session Number: " + STR$(ERRF%)
         PRINT   "Line Number:" + STR$(ERRL)
STOP