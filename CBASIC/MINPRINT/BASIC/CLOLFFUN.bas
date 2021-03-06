
\******************************************************************************
\******************************************************************************
\***
\***              RF COUNT LIST OF LISTS FILE FUNCTIONS
\***
\***               REFERENCE    : CLOLFFUN.BAS
\***
\***         VERSION A            Nik Sen                  13th October 1998
\***
\***         VERSION B            Charles Skadorwa         22nd January 1999
\***                   TOTAL.ITEMS added to keep track of total items in each list.
\***                 
\***    REVISION 1.4.            ROBERT COWEY.                  09 SEP 2003.
\***    Changes for RF trial.
\***    Removed redundant PVCS revision control block from top of code.
\***    Recompiled to prevent future automatic recompiles.
\***    No changes to actual code.
\***
\***    REVISION 1.5             ALAN CARR                     12 AUG 2004.
\***    Changes for RF OSSR solution.
\***    Added new flag "marked for OSSR count". This can be "Y" or "N"
\***
\***    REVISION 1.6            MARK GOODE                     5th January 2005.
\***    Changes for RF OSSR WAN solution.
\***    New field on the header record for remaining OSSR count figure and user ID.
\***
\***    REVISION 1.7            SYAM JAYAN                     25th January 2012.
\***    The change is to rearrange/remove redundant fields and to add new 
\***    fields in CLOLF Format as part of Stock file accuracy project
\***
\*******************************************************************************
\*******************************************************************************

    INTEGER*2 GLOBAL                \
       CURRENT.REPORT.NUM%
       
    STRING GLOBAL                    \
       CURRENT.CODE$,                \
       FILE.OPERATION$
    
    %INCLUDE CLOLFDEC.J86                                              

  FUNCTION CLOLF.SET PUBLIC
\***************************

    CLOLF.REPORT.NUM% = 556
    CLOLF.RECL%       = 67           ! BCS  ! 1.5 AC ! 1.6 MG ! 1.7 SJ
    CLOLF.FILE.NAME$  = "CLOLF"
    
  END FUNCTION
    
\-----------------------------------------------------------------------------

FUNCTION READ.CLOLF PUBLIC
\****************************

    INTEGER*2 READ.CLOLF
    
    READ.CLOLF = 1
    
    IF END #CLOLF.SESS.NUM% THEN READ.CLOLF.ERROR
    READ FORM "2C3,2C1,C30,C3,C1,C4,C3,C2,C3,2C2,C1,4I2";     \ 1.7 SJ
       #CLOLF.SESS.NUM%, CLOLF.RECORD.NUM%;    \ BCS \ 1.5 AC \ 1.7 SJ
       CLOLF.LISTID$,                          \ 
       CLOLF.USERID$,                          \ 1.6 MG \ 1.7 SJ
       CLOLF.LSTTYP$,                          \ 1.7 SJ
       CLOLF.BULETT$,                          \ 1.7 SJ
       CLOLF.LIST.NAME$,                       \ 1.7 SJ
       CLOLF.PICKER.USER.ID$,                  \ 1.7 SJ
       CLOLF.ACTIVE.STATUS$,                   \ 1.7 SJ
       CLOLF.PILST.ID$,                        \ 1.7 SJ
       CLOLF.CREATION.DATE$,                   \ 1.7 SJ
       CLOLF.CREATION.TIME$,                   \ 1.7 SJ
       CLOLF.EXPIRY.DATE$,                     \ 1.7 SJ
       CLOLF.PICK.START.TIME$,                 \ 1.7 SJ
       CLOLF.PICK.END.TIME$,                   \ 1.7 SJ
       CLOLF.CURRENT.LOCATION$,                \ 1.7 SJ
       CLOLF.TOTAL.ITEMS%,                     \ 1.7 SJ
       CLOLF.SRITEMS%,                         \ 1.7 SJ
       CLOLF.BSITEMS%,                         \ 1.7 SJ
       CLOLF.OSSRITEMS%                        ! 1.7 SJ       
       !CLOLF.TOTAL.ITEMS$,                    \ BCS    \ 1.7 SJ
       !CLOLF.SRITEMS$,                        \ 1.7 SJ
       !CLOLF.BSITEMS$,                        \ 1.7 SJ 
       !CLOLF.BUNAME$,                         \ 1.7 SJ 
       !CLOLF.HOLISTID$,                       \ 1.7 SJ
       !CLOLF.CNTDATE$,                        \ 1.5 AC \ 1.7 SJ
       !CLOLF.OSSR.FLAG$,                      \ 1.5 AC \ 1.7 SJ
       !CLOLF.OSSRITEMS$,                      \ 1.6 MG \ 1.7 SJ

    READ.CLOLF = 0
    EXIT FUNCTION
    
READ.CLOLF.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION

\-----------------------------------------------------------------------------
  

FUNCTION WRITE.CLOLF PUBLIC
\****************************

    INTEGER*2 WRITE.CLOLF
    
    WRITE.CLOLF = 1
    
    IF END #CLOLF.SESS.NUM% THEN WRITE.CLOLF.ERROR
    WRITE FORM "2C3,2C1,C30,C3,C1,C4,C3,C2,C3,2C2,C1,4I2";          \ 1.7 SJ
       #CLOLF.SESS.NUM%, CLOLF.RECORD.NUM%; \ BCS ! 1.5 AC ! 1.6 MG \ 1.7 SJ
       CLOLF.LISTID$,                          \ 
       CLOLF.USERID$,                          \ 1.6 MG \ 1.7 SJ
       CLOLF.LSTTYP$,                          \ 1.7 SJ
       CLOLF.BULETT$,                          \ 1.7 SJ
       CLOLF.LIST.NAME$,                       \ 1.7 SJ
       CLOLF.PICKER.USER.ID$,                  \ 1.7 SJ
       CLOLF.ACTIVE.STATUS$,                   \ 1.7 SJ
       CLOLF.PILST.ID$,                        \ 1.7 SJ
       CLOLF.CREATION.DATE$,                   \ 1.7 SJ
       CLOLF.CREATION.TIME$,                   \ 1.7 SJ
       CLOLF.EXPIRY.DATE$,                     \ 1.7 SJ
       CLOLF.PICK.START.TIME$,                 \ 1.7 SJ
       CLOLF.PICK.END.TIME$,                   \ 1.7 SJ
       CLOLF.CURRENT.LOCATION$,                \ 1.7 SJ
       CLOLF.TOTAL.ITEMS%,                     \ 1.7 SJ
       CLOLF.SRITEMS%,                         \ 1.7 SJ
       CLOLF.BSITEMS%,                         \ 1.7 SJ
       CLOLF.OSSRITEMS%                        ! 1.7 SJ       
       !CLOLF.TOTAL.ITEMS$,                    \ BCS    \ 1.7 SJ
       !CLOLF.SRITEMS$,                        \ 1.7 SJ
       !CLOLF.BSITEMS$,                        \ 1.7 SJ 
       !CLOLF.BUNAME$,                         \ 1.7 SJ 
       !CLOLF.HOLISTID$,                       \ 1.7 SJ
       !CLOLF.CNTDATE$,                        \ 1.5 AC \ 1.7 SJ
       !CLOLF.OSSR.FLAG$,                      \ 1.5 AC \ 1.7 SJ
       !CLOLF.OSSRITEMS$,                      \ 1.6 MG \ 1.7 SJ


    WRITE.CLOLF = 0
    EXIT FUNCTION
    
WRITE.CLOLF.ERROR:

    FILE.OPERATION$ = "R"
    CURRENT.REPORT.NUM% = CLOLF.REPORT.NUM%
       
    EXIT FUNCTION

END FUNCTION

