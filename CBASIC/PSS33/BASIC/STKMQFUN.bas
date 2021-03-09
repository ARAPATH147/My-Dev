\*******************************************************************************
\*******************************************************************************
\***
\***           STOCK MOVEMENT QUEUE FILE FUNCTIONS
\***
\***               REFERENCE    : STKMQFUN.BAS
\***
\***    Version B.              S.P. Kelsey                  7th Oct 1992
\***    Recompiled to pick up version B of the declarations.
\***
\***    Version C.              Les Cook                     5th Jan 1993
\***    To include new WRITE.HOLD function.
\***
\***    Version D.              Steven Goulding              8th Mar 1993
\***    Recompiled to pick up version D of the declarations.
\***
\***    Version E.              Michael J. Kelsall          15th Sep 1993
\***    Recompiled to pick up version E of the declarations.
\***
\***    Version F.              Robert Cowey                21th Oct 1993
\***    Corrected setting of FILE.OPERATION$ within WRITE functions.
\***
\***    Version G.              Mark Walker                 19th Jun 2015
\***    Problem Ticket Reference: PRB0046101
\***    Use WRITE MATRIX rather than a standard WRITE for the STKMQ, as
\***    the operating system does not guarantee the integrity of multiple
\***    applications simultaneously using the WRITE statement to write
\***    to the same sequential file.
\***
\***    NOTE: We cannot do this for the WRITE.HOLD.STKMQ function, as
\***          HOLD functionality is not available for WRITE MATRIX.
\***
\*******************************************************************************
\*******************************************************************************
 
    INTEGER*2 GLOBAL                                                    \
        CURRENT.REPORT.NUM%
         
    STRING GLOBAL                                                       \
        CURRENT.CODE$,                                                  \
        FILE.OPERATION$
         
%INCLUDE STKMQDEC.J86                                                       !FRC
  
\***********************************************************************
\***
\***    STKMQ.SET
\***
\***    Declare STKMQ file constants
\***
\***********************************************************************
FUNCTION STKMQ.SET PUBLIC

    STKMQ.FILE.NAME$  = "STKMQ"
    STKMQ.REPORT.NUM% = 83

END FUNCTION

\***********************************************************************
\***
\***    READ.STKMQ
\***
\***    Read STKMQ file record
\***
\***********************************************************************
FUNCTION READ.STKMQ PUBLIC
  
    INTEGER*2 READ.STKMQ 
    
    READ.STKMQ = 1
    
    IF END #STKMQ.SESS.NUM% THEN READ.ERROR
    READ #STKMQ.SESS.NUM%; LINE STKMQ.RECORD$
    
    STKMQ.TRANS.TYPE$ = UNPACK$(MID$(STKMQ.RECORD$,2,1))
    STKMQ.TIME$       = UNPACK$(MID$(STKMQ.RECORD$,7,3))
    
    READ.STKMQ = 0
    
    EXIT FUNCTION
    
READ.ERROR:
    
    FILE.OPERATION$     = "R"
    CURRENT.CODE$       = UNPACK$(MID$(STKMQ.RECORD$,2,1))
    CURRENT.REPORT.NUM% = STKMQ.REPORT.NUM%
                            
END FUNCTION

\***********************************************************************
\***
\***    WRITE.STKMQ
\***
\***    Write STKMQ file record
\***
\***********************************************************************
FUNCTION WRITE.STKMQ PUBLIC

    INTEGER*2 WRITE.STKMQ
    
!!!!STRING FORMAT$,                                                     \   !GMW
!!!!!!!!!!!STRING.LENGTH$                                                   !GMW
    STRING                                                              \   !GMW
        STKMQ.ARRAY.RECORD$(1)                                              !GMW
            
    WRITE.STKMQ = 1

    ! Initialise single element array for write matrix                      !GMW
    DIM STKMQ.ARRAY.RECORD$(1)                                              !GMW
                                                                            !GMW
!!!!STRING.LENGTH$ = STR$(LEN(STKMQ.RECORD$))                               !GMW
!!!!FORMAT$ = "C" + STRING.LENGTH$                                          !GMW
!!!!IF END #STKMQ.SESS.NUM% THEN WRITE.ERROR                                !GMW
!!!!WRITE FORM FORMAT$; #STKMQ.SESS.NUM%; STKMQ.RECORD$                     !GMW
                                                                            !GMW
    ! IF STKMQ record starts with a double quote AND                        !GMW
    !    ends with a double quote and carriage return/line feed             !GMW
    IF (LEFT$(STKMQ.RECORD$,1) = CHR$(22h)) AND                         \   !GMW
       (RIGHT$(STKMQ.RECORD$,3) =                                       \   !GMW
       CHR$(22h) + CHR$(0Dh) + CHR$(0Ah)) THEN BEGIN                        !GMW
                                                                            !GMW
        ! Remove surrounding double quotes and                              !GMW
        ! carriage return/line feed                                         !GMW
        STKMQ.ARRAY.RECORD$(1) =                                        \   !GMW
            MID$(STKMQ.RECORD$,2,LEN(STKMQ.RECORD$) - 4)                    !GMW
                                                                            !GMW
    ENDIF ELSE BEGIN                                                        !GMW
        STKMQ.ARRAY.RECORD$(1) = STKMQ.RECORD$                              !GMW
    ENDIF                                                                   !GMW
                                                                            !GMW
    ! ------------------------------------------------------                !GMW
    ! Write data component of STKMQ record. The WRITE MATRIX                !GMW
    ! command will automatically surround with double quotes                !GMW
    ! and add a carriage return/line feed.                                  !GMW
    ! ------------------------------------------------------                !GMW
    IF END #STKMQ.SESS.NUM% THEN WRITE.ERROR                                !GMW
    WRITE MATRIX #STKMQ.SESS.NUM%; STKMQ.ARRAY.RECORD$(1), 1                !GMW
       
    WRITE.STKMQ = 0
    
    EXIT FUNCTION
    
WRITE.ERROR:
    
    FILE.OPERATION$     = "W"                                               !FRC
    CURRENT.CODE$       = STKMQ.RECORD$
    CURRENT.REPORT.NUM% = STKMQ.REPORT.NUM%

END FUNCTION

\***********************************************************************
\***
\***    WRITE.HOLD.STKMQ
\***
\***    Write hold STKMQ file record
\***
\***********************************************************************
FUNCTION WRITE.HOLD.STKMQ PUBLIC                                            !CLC

    INTEGER*2 WRITE.HOLD.STKMQ                                              !CLC
    
    STRING FORMAT$,                                                     \   !CLC
           STRING.LENGTH$                                                   !CLC
            
    WRITE.HOLD.STKMQ = 1                                                    !CLC

    STRING.LENGTH$ = STR$(LEN(STKMQ.RECORD$))                               !CLC
    FORMAT$        = "C" + STRING.LENGTH$                                   !CLC
    
    IF END #STKMQ.SESS.NUM% THEN WRITE.HOLD.ERROR                           !CLC
    WRITE FORM FORMAT$; HOLD #STKMQ.SESS.NUM%; STKMQ.RECORD$                !CLC
       
    WRITE.HOLD.STKMQ = 0                                                    !CLC
    
    EXIT FUNCTION                                                           !CLC
    
WRITE.HOLD.ERROR:                                                           !CLC
    
    FILE.OPERATION$     = "W"                                               !FRC
    CURRENT.CODE$       = STKMQ.RECORD$                                     !CLC
    CURRENT.REPORT.NUM% = STKMQ.REPORT.NUM%                                 !CLC

  END FUNCTION                                                              !CLC

