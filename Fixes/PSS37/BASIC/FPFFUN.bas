\******************************************************************************
\******************************************************************************
\***
\***              FLASHPACK / PARENT FILE FUNCTIONS
\***
\***                   REFERENCE    : FPFFUN.BAS
\***
\***         VERSION A : LES COOK       21ST AUGUST 1992
\***
\***    VERSION B.              ROBERT COWEY.                       21 OCT 1993.
\***    Corrected setting of FILE.OPERATION$ within WRITE function.
\***
\***    VERSION C.              CLIVE NORRIS.                       17 JAN 1994.
\***    Altered dimension of FPF.MULT.FACTOR$ to match that of FPF.CODE$.
\***
\*******************************************************************************
\*******************************************************************************


   INTEGER*2 GLOBAL                     \
      CURRENT.REPORT.NUM%  
   
   STRING GLOBAL                       \
      CURRENT.CODE$,                   \
      FILE.OPERATION$
   
   %INCLUDE FPFDEC.J86                                                ! BRC
   
   
  FUNCTION FPF.SET PUBLIC
\*************************    

    FPF.REPORT.NUM% = 53                                    
    FPF.RECL%       = 64                                    
    FPF.FILE.NAME$  = "FPF"
    MAX.FPF.CODES%  = 10				    
    DIM FPF.CODE$(MAX.FPF.CODES%+1)                         
    DIM FPF.MULT.FACTOR$(MAX.FPF.CODES%+1)                             ! CDCN

 END FUNCTION
 
 \----------------------------------------------------------------------------
 

  FUNCTION READ.FPF PUBLIC
\**************************

    INTEGER*1 READ.FPF
    
    READ.FPF = 1
    
    IF END #FPF.SESS.NUM% THEN READ.FPF.ERROR
    READ FORM "T5,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2";\
            #FPF.SESS.NUM%  					  \
         KEY FPF.PARENT.CODE$;                                    \
             FPF.CODE$(1),                                        \
             FPF.MULT.FACTOR$(1),                                 \
             FPF.CODE$(2),                                        \
             FPF.MULT.FACTOR$(2),                                 \
             FPF.CODE$(3),                                        \
             FPF.MULT.FACTOR$(3),                                 \
             FPF.CODE$(4),                                        \
             FPF.MULT.FACTOR$(4),                                 \
             FPF.CODE$(5),                                        \
             FPF.MULT.FACTOR$(5),                                 \
             FPF.CODE$(6),                                        \
             FPF.MULT.FACTOR$(6),                                 \
             FPF.CODE$(7),                                        \
             FPF.MULT.FACTOR$(7),                                 \
             FPF.CODE$(8),                                        \
             FPF.MULT.FACTOR$(8),                                 \
             FPF.CODE$(9),                                        \
             FPF.MULT.FACTOR$(9),                                 \
             FPF.CODE$(10),                                       \
             FPF.MULT.FACTOR$(10)
    READ.FPF = 0
    EXIT FUNCTION
    
    READ.FPF.ERROR:
    
       CURRENT.REPORT.NUM% = FPF.REPORT.NUM%
       FILE.OPERATION$ = "R"
       CURRENT.CODE$ = FPF.PARENT.CODE$
       
       EXIT FUNCTION   
  
  END FUNCTION
 \----------------------------------------------------------------------------
 

  FUNCTION WRITE.FPF PUBLIC
\***************************

    INTEGER*1 WRITE.FPF
    
    WRITE.FPF = 1
    
    IF END #FPF.SESS.NUM% THEN WRITE.FPF.ERROR
    WRITE FORM "2C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2,C4,C2";\
            #FPF.SESS.NUM%; 					  \
             FPF.PARENT.CODE$,                                    \
             FPF.CODE$(1),                                        \
             FPF.MULT.FACTOR$(1),                                 \
             FPF.CODE$(2),                                        \
             FPF.MULT.FACTOR$(2),                                 \
             FPF.CODE$(3),                                        \
             FPF.MULT.FACTOR$(3),                                 \
             FPF.CODE$(4),                                        \
             FPF.MULT.FACTOR$(4),                                 \
             FPF.CODE$(5),                                        \
             FPF.MULT.FACTOR$(5),                                 \
             FPF.CODE$(6),                                        \
             FPF.MULT.FACTOR$(6),                                 \
             FPF.CODE$(7),                                        \
             FPF.MULT.FACTOR$(7),                                 \
             FPF.CODE$(8),                                        \
             FPF.MULT.FACTOR$(8),                                 \
             FPF.CODE$(9),                                        \
             FPF.MULT.FACTOR$(9),                                 \
             FPF.CODE$(10),                                       \
             FPF.MULT.FACTOR$(10)
    WRITE.FPF = 0
    EXIT FUNCTION
    
    WRITE.FPF.ERROR:
    
       CURRENT.REPORT.NUM% = FPF.REPORT.NUM%
       FILE.OPERATION$ = "W"                                           ! BRC
       CURRENT.CODE$ = FPF.PARENT.CODE$
       
       EXIT FUNCTION   
  
  END FUNCTION

