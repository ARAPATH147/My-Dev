\***********************************************************************
\***
\*** FILE FUNCTION DEFINITION Parcel Status Update Control File (PSUCF)  
\***
\***           REFERENCE    : PSUCFFUN.BAS
\***
\***         VERSION A         Kiran Krishnan        28th July 2017
\***         PRJ2002- Order & Collect Parcel Management - Phase 2
\***         Initial version for User Story PMLA-377 
\***
\***         VERSION B         Kiran Krishnan        14th August 2017
\***         PMLA-397:- Added logical name to the file function
\***
\***********************************************************************
\***********************************************************************


    INTEGER*2 GLOBAL                  \
       CURRENT.REPORT.NUM%

    STRING GLOBAL                     \
       CURRENT.CODE$,                 \
       FILE.OPERATION$

    %INCLUDE PSUCFDEC.J86                                       

\***********************************************************************
\***
\***    PSUCF.SET
\***
\***    Set PSUCF file variables
\***
\***********************************************************************

FUNCTION PSUCF.SET PUBLIC

    PSUCF.REPORT.NUM%  = 914
    PSUCF.RECL%        =  25
    PSUCF.FILE.NAME$   = "PSUCF"                                        ! BKK
    PSUCF.REC.NUM%     = 1
    
    PSUCF.FILLER$      = STRING$(20," ")

END FUNCTION
                                                                     
\***********************************************************************
\***
\***    READ.PSUCF
\***
\***    Read PSUCF file record
\***
\***********************************************************************

FUNCTION READ.PSUCF PUBLIC

    INTEGER*1 READ.PSUCF

    READ.PSUCF = 1

    IF END#PSUCF.SESS.NUM% THEN READ.PSUCF.ERROR

    READ FORM "I4,C1,C20"; #PSUCF.SESS.NUM%,                         \                        
                           PSUCF.REC.NUM%;                           \ 
                           PSUCF.PSUTQ.POINTER%,                     \
                           PSD86.PSUTQ.STATUS$,                      \          
                           PSUCF.FILLER$
              
    READ.PSUCF = 0
    EXIT FUNCTION

READ.PSUCF.ERROR:

    CURRENT.REPORT.NUM% = PSUCF.REPORT.NUM%
    FILE.OPERATION$ = "R"
    CURRENT.CODE$ = STR$(PSUCF.REC.NUM%)

    EXIT FUNCTION
    
END FUNCTION

\***********************************************************************
\***
\***    WRITE.PSUCF
\***
\***    Write PSUCF file record
\***
\***********************************************************************


FUNCTION WRITE.PSUCF PUBLIC

    INTEGER*1 WRITE.PSUCF        

    WRITE.PSUCF = 1

    IF END#PSUCF.SESS.NUM% THEN WRITE.PSUCF.ERROR

    WRITE FORM "I4,C1,C20"; #PSUCF.SESS.NUM%,                \
                            PSUCF.REC.NUM%;                  \
                            PSUCF.PSUTQ.POINTER%,            \
                            PSD86.PSUTQ.STATUS$,             \          
                            PSUCF.FILLER$                         

    WRITE.PSUCF = 0
    EXIT FUNCTION

WRITE.PSUCF.ERROR:

    CURRENT.REPORT.NUM% = PSUCF.REPORT.NUM%
    FILE.OPERATION$ = "W"
    CURRENT.CODE$ = STR$(PSUCF.REC.NUM%)

    EXIT FUNCTION
   
END FUNCTION

