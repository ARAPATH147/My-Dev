
\***********************************************************************
\***********************************************************************
\***
\***       PARCEL STATUS UPDATE TRANSACTION QUEUE FILE FUNCTIONS
\***
\***               FILE TYPE    : Sequential
\***
\***               REFERENCE    : PSUTQFUN.BAS
\***
\***         VERSION A         Kiran Krishnan        27th July 2017
\***         PRJ2002- Order & Collect Parcel Management - Phase 2
\***         Initial version for User Story PMLA-377 
\***
\***         VERSION B         Kiran Krishnan        14th August 2017
\***         PMLA-397:- Added logical name to the file function
\***
\***         VERSION C         Kiran Krishnan        23rd August 2017
\***         PMLA 419:- Changes to read and write file functions to
\***         use READ MATRIX and WRITE MATRIX.
\***
\***         VERSION D        Kiran Krishnan        07th September 2017
\***         Minor changes to PSUTQ.TRANS.TYPE$ data processing  
\***
\***********************************************************************
\***********************************************************************
 
    INTEGER*2 GLOBAL                                                   \
        CURRENT.REPORT.NUM%
         
    STRING GLOBAL                                                      \
        CURRENT.CODE$,                                                 \
        FILE.OPERATION$
         
%INCLUDE PSUTQDEC.J86                                                   
  
\***********************************************************************
\***
\***    PSUTQ.SET
\***
\***    Declare PSUTQ file constants
\***
\***********************************************************************
FUNCTION PSUTQ.SET PUBLIC

    PSUTQ.FILE.NAME$  = "PSUTQ"                                         ! BKK
    PSUTQ.REPORT.NUM% = 913

END FUNCTION

\***********************************************************************
\***
\***    READ.PSUTQ
\***
\***    Read PSUTQ file record
\***
\***********************************************************************
FUNCTION READ.PSUTQ PUBLIC
  
    INTEGER*2 READ.PSUTQ   
    STRING PSUTQIN.ARRAY.RECORD$(1)                                     ! CKK
    
    READ.PSUTQ = 1
    
    DIM PSUTQIN.ARRAY.RECORD$(1)                                        ! CKK
    
    IF END #PSUTQ.SESS.NUM% THEN READ.ERROR
!   READ #PSUTQ.SESS.NUM%; LINE PSUTQ.RECORD$                           ! CKK 
    READ MATRIX # PSUTQ.SESS.NUM%; PSUTQIN.ARRAY.RECORD$(1),1           ! CKK

    PSUTQ.RECORD$ = PSUTQIN.ARRAY.RECORD$(1)                            ! CKK                                       
!   PSUTQ.TRANS.TYPE$ = UNPACK$(LEFT$(PSUTQ.RECORD$,1))                 ! DKK
    PSUTQ.TRANS.TYPE$ = LEFT$(PSUTQ.RECORD$,2)                          ! DKK
    
    READ.PSUTQ = 0
    
    EXIT FUNCTION
    
READ.ERROR:
    
    FILE.OPERATION$     = "R"
    CURRENT.CODE$       = UNPACK$(MID$(PSUTQ.RECORD$,2,1))
    CURRENT.REPORT.NUM% = PSUTQ.REPORT.NUM%
                            
END FUNCTION

\***********************************************************************
\***
\***    WRITE.PSUTQ
\***
\***    Write PSUTQ file record
\***
\***********************************************************************
FUNCTION WRITE.PSUTQ PUBLIC

    INTEGER*2 WRITE.PSUTQ    
!   STRING FORMAT$,                                                    \! CKK
!          STRING.LENGTH$                                               ! CKK
    STRING PSUTQOUT.ARRAY.RECORD$(1)                                    ! CKK
            
    WRITE.PSUTQ = 1
    
    DIM PSUTQOUT.ARRAY.RECORD$(1)                                       ! CKK
    
    PSUTQOUT.ARRAY.RECORD$(1) = PSUTQ.RECORD$                           ! CKK                                         
!   STRING.LENGTH$ = STR$(LEN(PSUTQ.RECORD$))                           ! CKK 
!   FORMAT$ = "C" + STRING.LENGTH$                                      ! CKK
                                          
    IF END #PSUTQ.SESS.NUM% THEN WRITE.ERROR                            
!   WRITE FORM FORMAT$; #PSUTQ.SESS.NUM%; PSUTQ.RECORD$                 ! CKK
    WRITE MATRIX # PSUTQ.SESS.NUM%; PSUTQOUT.ARRAY.RECORD$(1),1         ! CKK
                                                                               
    WRITE.PSUTQ = 0
    
    EXIT FUNCTION
    
WRITE.ERROR:
    
    FILE.OPERATION$     = "W"                                           
    CURRENT.CODE$       = PSUTQ.RECORD$
    CURRENT.REPORT.NUM% = PSUTQ.REPORT.NUM%

END FUNCTION
