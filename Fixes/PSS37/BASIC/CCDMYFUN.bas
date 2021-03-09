
\*****************************************************************************
\*****************************************************************************
\***
\***                 FILE HANDLING FUNCTIONS SOURCE CODE
\***
\***                   FILE TYPE:  KEYED
\***
\***                   REFERENCE:  CCDMYFUN.BAS
\***
\***                 DESCRIPTION:  RETURNS / AUTOMATIC CREDIT CLAIMING
\***                               DUMMY FILE 
\***
\***      VERSION A         Michael J. Kelsall      30th September 1993
\***      
\*****************************************************************************
\*****************************************************************************

  INTEGER*2 GLOBAL            \
         CURRENT.REPORT.NUM% 
         
  STRING GLOBAL               \
         CURRENT.CODE$,       \
         FILE.OPERATION$           

  %INCLUDE CCDMYDEC.J86



  FUNCTION CCDMY.SET PUBLIC

     INTEGER*2 CCDMY.SET
     CCDMY.SET = 1

       CCDMY.REPORT.NUM% = 324
       CCDMY.FILE.NAME$  = "CCDMY"
  
     CCDMY.SET = 0

  END FUNCTION

