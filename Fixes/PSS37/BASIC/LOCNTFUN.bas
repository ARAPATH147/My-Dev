\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   LOCNTFUN.BAS  $
\***
\***   $Revision:   1.0  $
\***
\******************************************************************************
\******************************************************************************
\***
\***              STOCKTAKE LOCATION COUNTS FILE FUNCTIONS
\***
\***               REFERENCE    :LOCNTFUN.BAS
\***
\***         VERSION A            Johnnie Chan    6th Jan 1998
\***
\*******************************************************************************
\*******************************************************************************

   INTEGER*2 GLOBAL                 \
             CURRENT.REPORT.NUM%
       
   STRING GLOBAL                    \
          CURRENT.CODE$,            \
          FILE.OPERATION$
    
   %INCLUDE LOCNTDEC.J86                                             

FUNCTION LOCCNT.SET PUBLIC
\***************************

   LOCCNT.REPORT.NUM% = 546
   LOCCNT.RECL%       = 1
   LOCCNT.FILE.NAME$  = "LOCCNT"
    
END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.LOCCNT PUBLIC
\****************************

   INTEGER*2 READ.LOCCNT
    
   READ.LOCCNT = 1
    
   IF END #LOCCNT.SESS.NUM% THEN READ.LOCCNT.ERROR
   READ FORM "I1"; #LOCCNT.SESS.NUM%,LOCCNT.RECORD.NUM%; \
      LOCCNT.LOCATION.COUNT%

   READ.LOCCNT = 0
   EXIT FUNCTION
    
READ.LOCCNT.ERROR:
    
   FILE.OPERATION$ = "R"
   CURRENT.REPORT.NUM% = LOCCNT.REPORT.NUM%
       
   EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  
  
FUNCTION WRITE.LOCCNT PUBLIC
\*****************************

   INTEGER*2 WRITE.LOCCNT
    
   WRITE.LOCCNT = 1
   
   IF END #LOCCNT.SESS.NUM% THEN WRITE.LOCCNT.ERROR
   WRITE FORM "I1"; #LOCCNT.SESS.NUM%,LOCCNT.RECORD.NUM%; \
      LOCCNT.LOCATION.COUNT%

   WRITE.LOCCNT = 0
   EXIT FUNCTION
   
WRITE.LOCCNT.ERROR:
   
   FILE.OPERATION$ = "W"                                            
   CURRENT.REPORT.NUM% = LOCCNT.REPORT.NUM%
      
   EXIT FUNCTION

END FUNCTION


!*************************************************************************
!* This function returns a record number given a location code of the form
!* Sxxxx or Bxxxx. If the location code is not valid, then the function
!* will return a value of 0.
!*************************************************************************
FUNCTION GET.LOCCNT.RECNUM(LOC.CODE$) PUBLIC
\*****************************

   STRING LOC.CODE$
   INTEGER*4 GET.LOCCNT.RECNUM
   INTEGER*4 TEMP.RECNUM%
   INTEGER*2 VALID.FLAG%
   INTEGER*2 A%
   
   VALID.FLAG% = 1

   IF LEFT$(LOC.CODE$, 1) = "S" THEN BEGIN
      !*******************************************************************
      !* Shopfloor locations use records 1 - 10000
      !*******************************************************************
      TEMP.RECNUM% = 1

   ENDIF ELSE IF LEFT$(LOC.CODE$, 1) = "B" THEN BEGIN
      !*******************************************************************
      !* Backroom locations use records 10001-20000
      !*******************************************************************
      TEMP.RECNUM% = 10001

   ENDIF ELSE BEGIN
      !*******************************************************************
      !* Invalid location, since it has to be Sxxxx or Bxxxx
      !*******************************************************************
      VALID.FLAG% = 0

   ENDIF

   IF LEN(LOC.CODE$) <> 5 THEN BEGIN
      !*******************************************************************
      !* Invalid location, since it is not of the required length
      !*******************************************************************
      VALID.FLAG% = 0

   ENDIF ELSE IF VALID.FLAG% = 1 THEN BEGIN
      !*******************************************************************
      !* Check that all the digits are valid.
      !*******************************************************************
      FOR A% = 2 TO 5
         IF MID$(LOC.CODE$,A%,1)<"0" OR MID$(LOC.CODE$,A%,1)>"9" THEN BEGIN
            !*************************************************************
            !* Invalid, since location does not have the last four digits
            !* as numeric characters.
            !*************************************************************
            VALID.FLAG% = 0
         ENDIF
      NEXT A%
   ENDIF

   IF VALID.FLAG% = 1 THEN BEGIN
      !*******************************************************************
      !* All checks have proved the location valid, add on offset and
      !* return this as the result.
      !*******************************************************************
      TEMP.RECNUM% = TEMP.RECNUM% + VAL(MID$(LOC.CODE$,2,4))
     
   ENDIF ELSE BEGIN
      !*******************************************************************
      !* Invalid location code, so prepare to return a zero code.
      !*******************************************************************
      TEMP.RECNUM% = 0

   ENDIF
   
   GET.LOCCNT.RECNUM = TEMP.RECNUM%

END FUNCTION

