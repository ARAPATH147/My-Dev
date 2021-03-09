\*******************************************************************************! LCSk
\*******************************************************************************! LCSk
\***                                                                            ! LCSk
\*** IMPORTANT                                                                  ! LCSk
\*** =========                                                                  ! LCSk
\***    All references to UPDT.IRF.UPDT have been commented out and replaced    ! LCSk
\***    with UPDT.IRF.TIF.UPDT due to the following issue found post MCF:       ! LCSk
\***                                                                            ! LCSk
\***    Node ID check removed as it is preventing the TIF from being updated in ! LCSk
\***    Single MCF controller stores ie. MCF has changed the single DE nodes    ! LCSk
\***    to CE. This change should have been done as part of the MCF project,    ! LCSk
\***    however, the Business were not willing to pay for the relinking and     ! LCSk
\***    testing of some 200+ programs. There is a slight processing overhead    ! LCSk
\***    in dual-MCF stores, since the TIF will now be updated in them as well,  ! LCSk
\***    however the impact is small and TOF does not run in this environment.   ! LCSk
\***                                                                            ! LCSk
\***    If you wish to use the old UPDT.IRF.UPDT function then you will need to ! LCSk
\***    either:                                                                 ! LCSk
\***                                                                            ! LCSk
\***          1. Check out the previous OBJs, PSBF19e.J86 and update the .INP   ! LCSk
\***             and .MAK to include the functions PSBF19, PSBF41 & PSBF42      ! LCSk
\***       or                                                                   ! LCSk
\***          2. Check out the previous PSBF19e.J86 and function library:       ! LCSk
\***             FUNLIB.L86                                                     ! LCSk
\***                                                                            ! LCSk
\*******************************************************************************! LCSk
\*******************************************************************************! LCSk

\*******************************************************************************
\*******************************************************************************
\***
\***
\***        FUNCTION      : UPDT.IRF.TIF.UPDT (was UPDT.IRF.UPDT)     ! LCSk
\***        AUTHOR        : Richard Hopkinson (Pseudocode)
\***                      : Richard Hopkinson (Basic Code)
\***
\***        DATE WRITTEN  : 11th March 1988   (Pseudocode)
\***                      : 11th March 1988   (Basic Code)
\***
\***        REFERENCE     : PSBF19
\***
\***
\***        VERSION B        D.S. O'DARE (Pseudocode)      25th November 1988
\***                         B.C. WILLIS (Basic code)       2nd December 1988
\***        89A VERSION.
\***        Changes to small stores version of program.
\***        Change to incorporate new function SESS.NUM.UTILITY (PSBF20), to
\***        allocate file session numbers dynamically via the global Session
\***        Number Table.  Message 550 replaces message number 551 and message
\***        502 is changed to message number 508.  The decision to write the
\***        IRF unlocked or not, is no longer based on the calling module
\***        number, but rather on an extra parameter (irf.locked.flag) passed
\***        by the calling program.  The program-to-chain-to has been amended
\***        from "01" to "50".
\***
\***        VERSION C        JANET LAWRENCE                10th April 1989
\***        Various small changes: will only read the TIF to see if an item
\***        is already on it if the ACD flag is not "ADD"; sets messages
\***        up correctly; stops adding records to the TIF if it is full;
\***        doesn't increment the TIF record count if only altering a record.
\***
\***        VERSION D        ANDREW WEDGEWORTH              21st July 1992
\***        Redundant function parameters removed.  Standard error detected
\***        function used to log event 101s.
\***
\***        VERSION E        MARK WALKER                     8th June 1993
\***        Debug code removed from the CLOSE.IRF.UPDT function.
\***
\***        VERSION F        STEVE PERKINS             21st September 1993
\***        Deals project: Changed IRF and TIF record layouts, to take
\***        account of new deals fields added.
\***
\***        VERSION G (1.1)    Nik Sen                 31st January 1995
\***        Version letters removed from included code (not commented).
\***
\***        VERSION H         Dave West                 8th September 1998
\***        Changes made for GSA v2, also changes made to TIF processing so
\***        record is written before header record and record number set to
\***        file size instead of being incremented. (TIF changes originally
\***        written by Nik Sen).
\***
\***        VERSION I       Stuart William McConnachie  14th February 2000
\***        Changed IRF.INDICAT2$ to bit flags IRF.INDICAT2%.
\***
\***        VERSION J            Stuart Highley              10th May 2002
\***        Added code to log events when many prices are being updated to
\***        the same price.  This should help identify the pricing bug.
\***
\***        VERSION K       Stuart William McConnachie  7th August 2002
\***        Removed above code.
\***
\***    REVISION 1.7.                                              AUG 2002
\***    Removed debug code. (This comment added for revision 1.9).
\***
\***    REVISION 1.8.                ROBERT COWEY                6 AUG 2002.
\***    Major changes to UPDT.IRF.UPDT function for 2002 Deals Rewrite project.
\***    Removed redundant code processing unused TIF and TMCF file formats.
\***    Used SPLIT.NEW.IRF.DATA$ (new for IRFFUN.BAS revision 1.7) to extract
\***    individual IRF variables from NEW.IRF.DATA$ (IRF record string passed
\***    into function) on entry to function. (There are no internal IRF reads).
\***    Used SPLIT.TIF.IRF.DATA$ (new for TIFFUN.BAS revision 1.5) to extract
\***    individual TIF variables from TIF.IRF.DATA$ (IRF record string copied
\***    from NEW.IRF.DATA$) immediately prior to the WRITE to the TIF.
\***    Use of these SPLIT... functions keeps both IRF and TIF record layouts
\***    invisible (and independant) from the UPDT.IRF.UPDT function.
\***
\***    REVISION 1.9.                ROBERT COWEY.                05 DEC 2003.
\***    Corrected revision number comment for previous change from 1.7 to 1.8.
\***    Changes to functions for Deal Limit removal project.
\***    Moved SPLIT.NEW.IRF.DATA$ to start of UPDT.IRF.UPDT function.
\***    Deleted IRFDEX record when deleting Boots-item-code IRF record
\***    using PSBF19 specific session number F19.IRFDEX.SESS.NUM%.
\***
\***    Version 1.10      Stuart William McConnachie     31st Oct 2006
\***    Chain back to PSB50.286, instead of xxx50.286 derived from
\***    first three letters of MODULE.NUMBER$.  Doesn't work for
\***    PSD and SRP applications.
\***
\***    REVISION 1.11                Neil Bennett.                14 Nov 2006.
\***    Changed to include updates to the Product History File (PHF) for CIP.
\***    Changed to allow the recognition of the calling program (defaults to
\***    'R' - RPD) by checking for a second character to the IRF.LOCKED.FLAG$
\***    in the update routine. This is used for the PHF update. Reads of files
\***    is done by using a 'unique' session number and saving the 'global'
\***    session number if already set up, thus allowing for the files to be
\***    in use (or not) by the calling program.
\***
\***    REVISION 1.11a               Neil Bennett.                28 Feb 2007.
\***    Fix to 1.11 to set the Last Increase date if a price increase is
\***    received and there is no current price history. (assumes that the
\***    Global var IRF.SALEPRIC$ is set to the current price when the function
\***    is called).
\***
\***    REVISION 1.12                Paul Bowers                 3rd March 2007
\***    Fixes to revision 1.11 and 1.11a
\***
\***    REVISION 1.13                Brian Greenfield            19th April 2007
\***    If a Markdown item's label type is already set to type 3, clearance,
\***    force it to stay as clearance no matter what the new label type is.
\***    Also force change type to "C" instead of "R".
\***
\***    REVISION 1.14                Brian Greenfield            17th May 2007
\***    If an item is a WEEE item then force the PHF label type to a
\***    standard label, type 0. this is a CIP phase 2 change request.
\***    Correct the number of records on the PHF create.
\***
\***    REVISION 1.15                Neil Bennett               18th June 2007
\***    Fix to was/was/now label setting. PHF processing code moved to external
\***    function PSBF46 as now required from PSBF10 also.
\***
\***    REVISION 1.16                Neil Bennett                2nd July 2007
\***    Trap for BTCPR00000229 - Stop PSBF46 being called with group code bar
\***    codes as this will force Clearance to be set as markdown flag never set
\***    on group codes.
\***
\***    VERSION L                Charles Skadorwa            25th Sept 2013
\***    F261 Gift Card Mall IIN Range Extension Project - Commented ! LCSk
\***    Node ID check removed as it is preventing the TIF from being updated in
\***    MCF controller stores ie. MCF has changed the single DE nodes to CE.
\***    This change should have been done as part of the MCF project, however,
\***    the Business were not willing to pay for the relinking and testing of
\***    some 200+ programs. There is a slight processing overhead in dual-MCF
\***    stores, since the TIF will now be updated in them as well, however the
\***    impact is small and TOF does not run in this environment.
\***
\*******************************************************************************
\*******************************************************************************

REM peudocode follows...

\*******************************************************************************
\*******************************************************************************
\***
\***
\***                   FUNCTION OVERVIEW
\***                   -----------------
\***
\***     This function handles operations concerned with the new TOF (Terminal
\***     Offline Feature), which enables price look-ups to take place in the
\***     till in the event of a controller failure. Whenever an item is updated
\***     on the IRF, it should be processed in the following manner, in order to
\***     keep the terminal file up to date.
\***     NOTE: All items on the Item Movement File will be included in the TIF
\***     overnight, after PSB30 has finished.
\***
\***     There are three entry points to this function :
\***
\***
\***     1    Called at the start of the program, to open files.
\***
\***
\***     2    Called when an update is to be performed; this can be divided
\***          into four procedures:
\***
\***          i   Write the item record to the IRF, with hold.
\***
\***          ii  Write the Terminal Maintenance Control File header record,
\***              with hold.
\***
\***          iii Write the Terminal Image File record.
\***
\***          iv  Write the Terminal Maintenance Control File data record.
\***
\***
\***     3    Called at the end of the program, to close files.
\***
\***
\***     Due to the limitations in file sizes on the terminal, both the TMCF
\***     and the TIF must be checked to ensure their maximum file sizes are not
\***     exceeded.
\***
\***
\*******************************************************************************
\*******************************************************************************
\***
\***  %INCLUDE of globals for external function CONV.TO.HEX
\***  %INCLUDE of globals for external function CONV.TO.STRING
\***  %INCLUDE of globals for external function UPDATE.IRF
\***  %INCLUDE of globals for screen chaining parameters
\***  %INCLUDE of globals for SESS.NUM.UTILITY function
\***
\***  %INCLUDE of statements for IRF
\***  %INCLUDE of statements for TIF
\***  %INCLUDE of statements for TMCF
\***  %INCLUDE of statements for GAOPT
\***  %INCLUDE of statements for external function APPLICATION.LOG
\***  %INCLUDE of statements for external function ADXERROR
\***  %INCLUDE of statements for external function ADXSERVE
\***  %INCLUDE of statements for external function CONV.TO.HEX
\***  %INCLUDE of statements for external function CONV.TO.STRING
\***  %INCLUDE of statements for external function SESS.NUM.UTILITY
\***  %INCLUDE of statements for external function STANDARD.ERROR.DETECTED
\***
\-------------------------------------------------------------------------------

      ! 1 line deleted from here                                      ! DAW
      %INCLUDE PSBF02G.J86                                            ! 1.11 NWB
      %INCLUDE PSBF16G.J86                                            ! DAW
      %INCLUDE PSBF17G.J86                                            ! DAW
      %INCLUDE PSBF18G.J86                                            ! 1.11 NWB
      %INCLUDE PSBF19G.J86                                            !FSJW
      %INCLUDE PSBF20G.J86                                            ! DAW
      %INCLUDE PSBF46G.J86                                            ! 1.15 NWB
      %INCLUDE PSBUSEG.J86


      STRING GLOBAL                                                   \ DAW
         BATCH.SCREEN.FLAG$,                                          \ DAW
         MODULE.NUMBER$                                               ! DAW

      STRING                                                          \ 1.11NWB
         IDF.OPEN.FLAG$,                                              \ 1.11NWB
         PHF.OPEN.FLAG$,                                              \ 1.11NWB
         SRITL.OPEN.FLAG$                                             ! 1.11NWB

      INTEGER*2                                                       \ 1.11NWB
         F19.IDF.SESS.NUM%,                                           \ 1.11NWB
         F19.SOFTS.SESS.NUM%,                                         \ 1.11NWB
         F19.SRITL.SESS.NUM%,                                         \ 1.11NWB
         SAV.IDF.SESS.NUM%,                                           \ 1.11NWB
         SAV.SOFTS.SESS.NUM%,                                         \ 1.11NWB
         SAV.SRITL.SESS.NUM%                                          ! 1.11NWB

      %INCLUDE IRFDEC.J86                                             ! FSP
      %INCLUDE TIFDEC.J86                                             ! FSP
      %INCLUDE TMCFDEC.J86                                            ! FSP
      %INCLUDE GAOPTDEC.J86                                           ! FSP
      %INCLUDE IDFDEC.J86                                             ! 1.11 NWB
      %INCLUDE PHFDEC.J86                                             ! 1.11 NWB
      %INCLUDE SRITLDEC.J86                                           ! 1.11 NWB
      %INCLUDE SOFTSDEC.J86                                           ! 1.11 NWB

      %INCLUDE ADXERROR.J86
      %INCLUDE ADXSERVE.J86
      %INCLUDE PSBF01E.J86                                            ! DAW
      %INCLUDE PSBF02E.J86                                            ! 1.11 NWB
      %INCLUDE PSBF16E.J86                                            ! DAW
      %INCLUDE PSBF17E.J86                                            ! DAW
      %INCLUDE PSBF18E.J86                                            ! 1.11 NWB
      %INCLUDE PSBF20E.J86                                            ! DAW
      %INCLUDE PSBF24E.J86                                            ! DAW
      %INCLUDE PSBF46E.J86                                            ! 1.15 NWB

      %INCLUDE IRFEXT.J86                                             ! FSP
      %INCLUDE TIFEXT.J86                                             ! FSP
      %INCLUDE TMCFEXT.J86                                            ! DAW
      %INCLUDE GAOPTEXT.J86                                           ! DAW
      %INCLUDE IDFEXT.J86                                             ! 1.11 NWB
      %INCLUDE PHFEXT.J86                                             ! 1.11 NWB
      %INCLUDE SRITLEXT.J86                                           ! 1.11 NWB
      %INCLUDE SOFTSEXT.J86                                           ! 1.11 NWB

REM EJECT
\*******************************************************************************
\*********                 Function OPEN.IRF.UPDT                     **********
\*******************************************************************************
\***
\*** FUNCTION OPEN.IRF.UPDT (new IRF data,
\***                         add/change/delete flag)  PUBLIC
\***
\-------------------------------------------------------------------------------

   FUNCTION OPEN.IRF.UPDT (NEW.IRF.DATA$,                              \
                           ACD.FLAG$)                                  \
   PUBLIC

      STRING    ACD.FLAG$,                                             \
                ADX.PARM.2$,                                           \
\ 1 line deleted from here                                             \ DAW
                ERRFILE$,                                              \
                ERRNUM$,                                               \
                FILE$,                                                 \
                FUNCTION.FLAG$,                                        \ BBCW
                IRF.LOCKED.FLAG$,                                      \ BBCW
\ 1 line deleted from here                                             \ DAW
                NEW.IRF.DATA$,                                         \
\ 1 line deleted from here                                             \ DAW
                PASSED.STRING$,                                        \ BBCW
                STRING.ERRL$,                                          \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$

      INTEGER*1 CIP.ACTIVE%,                                           \ 1.11 NWB
                EVENT.NUM%,                                            \
                FILE.NO%,                                              \ BBCW
                INTEGER1%,                                             \
                MSGGRP%,                                               \
                PASSED.INTEGER%,                                       \ BBCW
                SEVERITY%

      INTEGER   ADX.FUNCTION%,                                         \
                ADX.PARM.1%,                                           \
                EVENT.NO%,                                             \
                MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                OPEN.IRF.UPDT,                                         \ DAW
                RET.CODE%,                                             \
                SESSION.NUMBER%,                                       \
                TERM%

      INTEGER*4 INTEGER4%

!     Lines deleted                                                       ! 1.8 RC
!     Un-necessary duplication of OPEN.IRF.UPDT file SET calls (below)    ! 1.8 RC

\*******************************************************************************
\***
\***   ON ERROR goto UPDT.OPEN.ERROR.DETECTED  (after logging the error, the
\***                                            function is immediately left)
\***
\***
\***   set FILE.NO% to 0
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***
\***   set chaining module to first module in current application
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO UPDT.OPEN.ERROR.DETECTED

      CALL IRF.SET                                                     ! FSP
      CALL TIF.SET                                                     ! FSP
      CALL TMCF.SET                                                    ! FSP
      CALL GAOPT.SET                                                   ! FSP
      CALL IDF.SET                                                    ! 1.11 NWB
      CALL PHF.SET                                                    ! 1.11 NWB
      CALL SRITL.SET                                                  ! 1.11 NWB

      F19.NODE.FILE.NAME$  EQ "ADXLXACN::C:\TEST.LAN"                      ! 1.9 RC
      F19.NODE.REPORT.NUM% EQ  676                                         ! 1.9 RC

      FILE.NO% = 0
      TIF.FULL$ = "N"
      TMCF.FULL$ = "N"
      UNIQUE$ = "          "
      ERRNUM$ = "    "
      ERRFILE$ = " "
      STRING.ERRL$ = "      "
      OPEN.IRF.UPDT = 0                                                ! DAW

      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                !1.10 SWM


\*******************************************************************************
\***
\***   set adx function to 4 (return node ID)
\***   set parameter 1 to terminal number
\***
\***   CALL ADXSERVE (return code,
\***                  adx function,
\***                  parameter 1,
\***                  parameter 2)
\***
\***   IF the node ID <> "CE" THEN    (was DE)                         ! LCSk
\***      set error flag to "NODE"
\***      GOTO FUNCTION.EXIT
\***   endif
\***
\***   set FUNCTION.FLAG$ to "O",
\***       PASSED.INTEGER% to GAOPT.REPORT.NUM% and
\***       PASSED.STRING$ to GAOPT.FILE.NAME$
\***
\***   use SESS.NUM.UTILITY function to allocate session number
\***   IF F20.RETURN.CODE% <> 0 THEN
\***      GOTO PROGRAM EXIT
\***   ENDIF
\***
\***   set gaopt session number to F20.INTEGER.FILE.NO%
\***   set file no to F20.INTEGER.FILE.NO%
\***
\***   IF END #gaopt session number THEN FILE.OPEN.ERROR
\***   OPEN the local version of GAOPT  (random)
\***   IF END #gaopt session number THEN FILE.READ.ERROR
\***   CALL READ.LOCAL.GAOPT
\***   CLOSE the local version of GAOPT
\***
\***   set FUNCTION.FLAG$ to "C",
\***       PASSED.INTEGER% to gaopt session number and
\***       PASSED.STRING$ to ""
\***
\***   use SESS.NUM.UTILITY function to deallocate session number
\***   IF F20.RETURN.CODE% <> 0 THEN
\***      GOTO PROGRAM EXIT
\***   ENDIF
\***
\***   set FUNCTION.FLAG$ to "O",
\***       PASSED.INTEGER% to TMCF.REPORT.NUM% and
\***       PASSED.STRING$ to TMCF.FILE.NAME$
\***
\***   use SESS.NUM.UTILITY function to allocate session number
\***   IF F20.RETURN.CODE% <> 0 THEN
\***      GOTO PROGRAM EXIT
\***   ENDIF
\***
\***   set tmcf session number to F20.INTEGER.FILE.NO%
\***   set file no to F20.INTEGER.FILE.NO%
\***
\***   IF END #tmcf session number THEN FILE.OPEN.ERROR
\***   OPEN the tmcf DIRECT
\***   IF END #tmcf session number THEN FILE.READ.ERROR
\***   CALL READ.TMCF.HEADER
\***   IF terminal look-up not activated THEN
\***      set error flag to "TERMILU 0"
\***      GOTO FUNCTION.EXIT
\***   endif
\***
\***   get tif format
\***
\***  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
\***  !!!!
\***  !!!! Use TIF.FORMAT% to hold the format of the TIF in numerical form, ie:
\***  !!!!
\***  !!!!                  1 = full descriptor and user data
\***  !!!!                  2 = full descriptor, no user data
\***  !!!!                  3 = short descriptor and user data
\***  !!!!                  4 = short descriptor, no user data
\***  !!!!                  5 = no descriptor, just user data
\***  !!!!                  6 = no descriptor or user data
\***  !!!!
\***  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
\***
\***   set FUNCTION.FLAG$ to "O",
\***       PASSED.INTEGER% to TIF.REPORT.NUM% and
\***       PASSED.STRING$ to TIF.FILE.NAME$
\***
\***   use SESS.NUM.UTILITY function to allocate session number
\***   IF F20.RETURN.CODE% <> 0 THEN
\***      GOTO PROGRAM EXIT
\***   ENDIF
\***
\***   set tif session number to F20.INTEGER.FILE.NO%
\***   set file no to F20.INTEGER.FILE.NO%
\***
\***   IF END #tif session number THEN FILE.OPEN.ERROR
\***   OPEN the tif KEYED
\***
\***   set FUNCTION.FLAG$ to "O",                                     ! 1.11 NWB
\***       PASSED.INTEGER% to PHF.REPORT.NUM% and                     ! 1.11 NWB
\***       PASSED.STRING$ to PHF.FILE.NAME$                           ! 1.11 NWB
\***
\***   use SESS.NUM.UTILITY function to allocate session number       ! 1.11 NWB
\***   IF F20.RETURN.CODE% <> 0 THEN                                  ! 1.11 NWB
\***      GOTO PROGRAM EXIT                                           ! 1.11 NWB
\***   ENDIF                                                          ! 1.11 NWB
\***
\***   set phf session number to F20.INTEGER.FILE.NO%                 ! 1.11 NWB
\***   set file no to F20.INTEGER.FILE.NO%                            ! 1.11 NWB
\***
\***   IF END #phf session number THEN SKIP.PHF.FILES                 ! 1.11 NWB
\***   OPEN the phf KEYED                                             ! 1.11 NWB
\***
\***   set FUNCTION.FLAG$ to "O",                                     ! 1.11 NWB
\***       PASSED.INTEGER% to IDF.REPORT.NUM% and                     ! 1.11 NWB
\***       PASSED.STRING$ to IDF.FILE.NAME$                           ! 1.11 NWB
\***
\***   use SESS.NUM.UTILITY function to allocate session number       ! 1.11 NWB
\***   IF F20.RETURN.CODE% <> 0 THEN                                  ! 1.11 NWB
\***      GOTO PROGRAM EXIT                                           ! 1.11 NWB
\***   ENDIF                                                          ! 1.11 NWB
\***
\***   set idf session number to F20.INTEGER.FILE.NO%                 ! 1.11 NWB
\***   set file no to F20.INTEGER.FILE.NO%                            ! 1.11 NWB
\***
\***   IF END #idf session number THEN FILE.OPEN.ERROR                ! 1.11 NWB
\***   OPEN the idf KEYED                                             ! 1.11 NWB
\***
\***   set FUNCTION.FLAG$ to "O",                                     ! 1.11 NWB
\***       PASSED.INTEGER% to SRITL.REPORT.NUM% and                   ! 1.11 NWB
\***       PASSED.STRING$ to SRITL.FILE.NAME$                         ! 1.11 NWB
\***
\***   use SESS.NUM.UTILITY function to allocate session number       ! 1.11 NWB
\***   IF F20.RETURN.CODE% <> 0 THEN                                  ! 1.11 NWB
\***      GOTO PROGRAM EXIT                                           ! 1.11 NWB
\***   ENDIF                                                          ! 1.11 NWB
\***
\***   set sritl session number to F20.INTEGER.FILE.NO%               ! 1.11 NWB
\***   set file no to F20.INTEGER.FILE.NO%                            ! 1.11 NWB
\***
\***   IF END #sritl session number THEN NO.SRITL.FILE                ! 1.11 NWB
\***   OPEN the phf KEYED                                             ! 1.11 NWB
\***
\***   SKIP.PHF.FILES:                                                ! 1.11 NWB
\***
\***   GOTO FUNCTION.EXIT
\***
\***
\-------------------------------------------------------------------------------

      ADX.FUNCTION% = 4
      ADX.PARM.1% = 0

      CALL ADXSERVE (RET.CODE%,                                       \
                     ADX.FUNCTION%,                                   \
                     ADX.PARM.1%,                                     \
                     ADX.PARM.2$)

      IF RET.CODE% <> 0 THEN                                          \
         MSGNUM% = 300                                               :\
         MSGGRP% = ASC("A")                                          :\
         SEVERITY% = 3                                               :\
         TERM% = 0                                                   :\
         EVENT.NUM% = 23                                             :\
         UNIQUE$ = STR$(RET.CODE%) + STR$(ADX.FUNCTION%)             :\
         CALL ADXERROR (TERM%,                                        \
                        MSGGRP%,                                      \
                        MSGNUM%,                                      \
                        SEVERITY%,                                    \
                        EVENT.NUM%,                                   \
                        UNIQUE$)                                     :\
         GOTO FUNCTION.EXIT

!     Allocate a session number to the IRFDEX and open the file.           ! 1.9 RC
!     Do not use IRFDEX.SESS.NUM% as this will change the operation of     ! 1.9 RC
!     IRFFUN functions when called from programs not opening the IRFDEX.   ! 1.9 RC

      FUNCTION.FLAG$  EQ "O"                                               ! 1.9 RC
      PASSED.INTEGER% EQ IRFDEX.REPORT.NUM%                                ! 1.9 RC
      PASSED.STRING$  EQ IRFDEX.FILE.NAME$                                 ! 1.9 RC

      RET.CODE% EQ SESS.NUM.UTILITY (FUNCTION.FLAG$, \                     ! 1.9 RC
                                     PASSED.INTEGER%, \                    ! 1.9 RC
                                     PASSED.STRING$)                       ! 1.9 RC

      IF RET.CODE% NE 0 THEN GOTO PROGRAM.EXIT ! PSBF20 failure            ! 1.9 RC

      F19.IRFDEX.SESS.NUM% EQ F20.INTEGER.FILE.NO%                         ! 1.9 RC

      FILE.NO% EQ F20.INTEGER.FILE.NO%                                     ! 1.9 RC
      IF END # F19.IRFDEX.SESS.NUM% THEN FILE.OPEN.ERROR                   ! 1.9 RC

      OPEN IRFDEX.FILE.NAME$ KEYED RECL IRFDEX.RECL% \                     ! 1.9 RC
         AS F19.IRFDEX.SESS.NUM%                                           ! 1.9 RC

      NODE.ID$ = MID$(ADX.PARM.2$,14,2)

!     Check whether the Node ID is a valid store controller configuration. ! 1.9 RC
!     If not then open the dummy file used on development controllers.     ! 1.9 RC
!     If the dummy file is found set the Node ID to CE to force PSBF19     ! 1.9 RC ! LCSk
!     to process as though for a 1PS store (ie; Update the TIF and TMCFR). ! 1.9 RC

      IF    NODE.ID$ NE "CE" \      ! Node ID does not correspond to any   ! 1.9 RC
        AND NODE.ID$ NE "CF" \      ! valid 1PS or 2PS store configuration ! 1.9 RC
        AND NODE.ID$ NE "DE" THEN \ ! (so may be a development controller) ! 1.9 RC
          BEGIN                                                            ! 1.9 RC

          FUNCTION.FLAG$  EQ "O"                                           ! 1.9 RC
          PASSED.INTEGER% EQ F19.NODE.REPORT.NUM%                          ! 1.9 RC
          PASSED.STRING$  EQ F19.NODE.FILE.NAME$                           ! 1.9 RC

          RET.CODE% EQ SESS.NUM.UTILITY (FUNCTION.FLAG$, \                 ! 1.9 RC
                                         PASSED.INTEGER%, \                ! 1.9 RC
                                         PASSED.STRING$)                   ! 1.9 RC

          IF RET.CODE% NE 0 THEN GOTO PROGRAM.EXIT                         ! 1.9 RC

          F19.NODE.SESS.NUM% EQ F20.INTEGER.FILE.NO%                       ! 1.9 RC

          IF END # F19.NODE.SESS.NUM% THEN F19.NODE.FILE.NOT.PRESENT       ! 1.9 RC

          OPEN F19.NODE.FILE.NAME$ DIRECT RECL 1 \ ! Opens dummy file on   ! 1.9 RC
             AS F19.NODE.SESS.NUM%                 ! dev controller        ! 1.9 RC

         !NODE.ID$ EQ "DE" ! Dummy file present so node ID set to DE       ! 1.9 RC ! LCSk
          NODE.ID$ EQ "CE" ! Dummy file present so node ID set to CE       ! LCSk

          CLOSE F19.NODE.SESS.NUM%                                         ! 1.9 RC

F19.NODE.FILE.NOT.PRESENT: ! File not present on development controller    ! 1.9 RC

          ENDIF                                                            ! 1.9 RC

      FUNCTION.FLAG$  = "O"                                           ! 1.11 NWB
      PASSED.INTEGER% = PHF.REPORT.NUM%                               ! 1.11 NWB
      PASSED.STRING$  = PHF.FILE.NAME$                                ! 1.11 NWB

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      PHF.SESS.NUM% = F20.INTEGER.FILE.NO%                            ! 1.11 NWB
      FILE.NO%      = F20.INTEGER.FILE.NO%                            ! 1.11 NWB
      IF END # PHF.SESS.NUM% THEN CHECK.CIP.STATUS                    ! 1.11 NWB
      OPEN PHF.FILE.NAME$ KEYED RECL PHF.RECL%                        \ 1.11 NWB
           AS PHF.SESS.NUM%                                           ! 1.11 NWB

PHF.CREATE.RESUME:                                                    ! 1.11 NWB
      PHF.OPEN.FLAG$ = "Y"                                            ! 1.11 NWB

      FUNCTION.FLAG$  = "O"                                           ! 1.11 NWB
      PASSED.INTEGER% = IDF.REPORT.NUM%                               ! 1.11 NWB
      PASSED.STRING$  = IDF.FILE.NAME$                                ! 1.11 NWB

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      F19.IDF.SESS.NUM% = F20.INTEGER.FILE.NO%                        ! 1.11 NWB
      FILE.NO% = F20.INTEGER.FILE.NO%                                 ! 1.11 NWB
      IF END # F19.IDF.SESS.NUM% THEN FILE.OPEN.ERROR                 ! 1.11 NWB
      OPEN IDF.FILE.NAME$ KEYED RECL IDF.RECL%                        \ 1.11 NWB
           AS F19.IDF.SESS.NUM%  NOWRITE NODEL                        ! 1.11 NWB

      IDF.OPEN.FLAG$ = "Y"                                            ! 1.11 NWB

      FUNCTION.FLAG$  = "O"                                           ! 1.11 NWB
      PASSED.INTEGER% = SRITL.REPORT.NUM%                             ! 1.11 NWB
      PASSED.STRING$  = SRITL.FILE.NAME$                              ! 1.11 NWB

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      F19.SRITL.SESS.NUM% = F20.INTEGER.FILE.NO%                      ! 1.11 NWB
      FILE.NO% = F20.INTEGER.FILE.NO%                                 ! 1.11 NWB
      IF END # F19.SRITL.SESS.NUM% THEN NO.SRITL.FILE                 ! 1.11 NWB
      OPEN SRITL.FILE.NAME$ KEYED RECL SRITL.RECL%                    \ 1.11 NWB
           AS F19.SRITL.SESS.NUM%  NOWRITE NODEL                      ! 1.11 NWB

      SRITL.OPEN.FLAG$ = "Y"                                          ! 1.11 NWB
SKIP.CIP.FILES:                                                       ! 1.11 NWB
NO.SRITL.FILE:                                                        ! 1.11 NWB

     !IF NODE.ID$ <> "DE" THEN                                        \ ! LCSk
     !   GOTO FUNCTION.EXIT                                             ! LCSk

      FUNCTION.FLAG$  = "O"                                           ! BBCW
      PASSED.INTEGER% = GAOPT.REPORT.NUM%                             ! BBCW
      PASSED.STRING$  = GAOPT.FILE.NAME$                              ! BBCW

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ DAW
                                    PASSED.INTEGER%,                  \ BBCW
                                    PASSED.STRING$)                   ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! DAW

      GAOPT.SESS.NUM% = F20.INTEGER.FILE.NO%                          ! BBCW
      FILE.NO%        = F20.INTEGER.FILE.NO%                          ! BBCW
      IF END # GAOPT.SESS.NUM% THEN FILE.OPEN.ERROR
      OPEN LOCAL.GAOPT.FILE.NAME$ RECL GAOPT.RECL%                    \
           AS GAOPT.SESS.NUM% NOWRITE NODEL

!     GAOPT.REC.NO% = 1
      RET.CODE% = READ.GAOPT                                          ! FSP
      IF RET.CODE% <> 0 THEN BEGIN                                    ! FSP
         GOSUB FILE.READ.ERROR                                        ! FSP
      ENDIF                                                           ! FSP

      CLOSE GAOPT.SESS.NUM%

      FUNCTION.FLAG$  = "C"                                           ! BBCW
      PASSED.INTEGER% = GAOPT.SESS.NUM%                               ! BBCW
      PASSED.STRING$  = ""                                            ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ DAW
                                    PASSED.INTEGER%,                  \ BBCW
                                    PASSED.STRING$)                   ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                       ! DAW

      FUNCTION.FLAG$  = "O"                                           ! BBCW
      PASSED.INTEGER% = TMCF.REPORT.NUM%                              ! BBCW
      PASSED.STRING$  = TMCF.FILE.NAME$                               ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ DAW
                                    PASSED.INTEGER%,                  \ BBCW
                                    PASSED.STRING$)                   ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! DAW

      TMCF.SESS.NUM% = F20.INTEGER.FILE.NO%                           ! BBCW
      FILE.NO%       = F20.INTEGER.FILE.NO%                           ! BBCW
      IF END # TMCF.SESS.NUM% THEN FILE.OPEN.ERROR
      OPEN TMCF.FILE.NAME$ DIRECT RECL TMCF.RECL%                     \
           AS TMCF.SESS.NUM% NODEL


      TMCF.OPEN.FLAG$ = "Y"

      TMCF.REC.NO% = 1
      RET.CODE% = READ.TMCF.HEADER                                    ! FSP
      IF RET.CODE% <> 0 THEN BEGIN                                    ! FSP
         GOSUB FILE.READ.ERROR                                        ! FSP
      ENDIF                                                           ! FSP

      IF TMCF.TERMILU% = 0 THEN                                       \
         GOTO FUNCTION.EXIT

!   Removed redundant code processing unused TIF and TMCF file formats    ! 1.8 RC

      IF TMCF.DESCTYPE% = 4 THEN                                      \ DW
         TIF.RECL% = TIF.RECL7% : TIF.FORMAT% = 7                     ! DW

      FUNCTION.FLAG$  = "O"                                           ! BBCW
      PASSED.INTEGER% = TIF.REPORT.NUM%                               ! BBCW
      PASSED.STRING$  = TIF.FILE.NAME$                                ! BBCW

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ DAW
                                    PASSED.INTEGER%,                  \ BBCW
                                    PASSED.STRING$)                   ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! DAW

      TIF.SESS.NUM% = F20.INTEGER.FILE.NO%                            ! BBCW
      FILE.NO%      = F20.INTEGER.FILE.NO%                            ! BBCW
      IF END # TIF.SESS.NUM% THEN FILE.OPEN.ERROR
      OPEN TIF.FILE.NAME$ KEYED RECL TIF.RECL%                        \
           AS TIF.SESS.NUM%

      TIF.OPEN.FLAG$ = "Y"

      GOTO FUNCTION.EXIT


\*******************************************************************************
\***
\***   CHECK.CIP.STATUS
\***
\*******************************************************************************

CHECK.CIP.STATUS:                                                     ! 1.11 NWB

      RET.CODE% = SOFTS.SET                                           ! 1.11 NWB

\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB
\/* Open SOFTSTAT file
\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB

      RET.CODE% = SESS.NUM.UTILITY ("O",                              \ 1.11 NWB
                                    SOFTS.REPORT.NUM%,                \ 1.11 NWB
                                    SOFTS.FILE.NAME$)                 ! 1.11 NWB
      F19.SOFTS.SESS.NUM% = F20.INTEGER.FILE.NO%                      ! 1.11 NWB

      IF END# F19.SOFTS.SESS.NUM% THEN SKIP.CIP.FILES                 ! 1.11 NWB
      OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%                   \ 1.11 NWB
          AS F19.SOFTS.SESS.NUM% NOWRITE NODEL                        ! 1.11 NWB

\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB
\/* Read required records and set GLOBAL vars accordingly
\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB

      CIP.ACTIVE% = 0                                                 ! 1.11 NWB
      SOFTS.REC.NUM% = 48                                             ! 1.11 NWB
      SAV.SOFTS.SESS.NUM% = SOFTS.SESS.NUM%                           ! 1.11 NWB
      SOFTS.SESS.NUM% = F19.SOFTS.SESS.NUM%                           ! 1.11 NWB
      RET.CODE% = READ.SOFTS                                          ! 1.11 NWB
      SOFTS.SESS.NUM% = SAV.SOFTS.SESS.NUM%                           ! 1.11 NWB
      SAV.SOFTS.SESS.NUM% = 0                                         ! 1.11 NWB
      IF RET.CODE% = 0                                                \ 1.11 NWB
     AND MATCH("INACTIVE", SOFTS.RECORD$, 1) = 0 THEN BEGIN           ! 1.11 NWB
         CIP.ACTIVE% = -1                                             ! 1.11 NWB
      ENDIF                                                           ! 1.11 NWB

\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB
\/* CLOSE Softstat file                                                 1.11 NWB
\/*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */   1.11 NWB

      CLOSE F19.SOFTS.SESS.NUM%                                       ! 1.11 NWB
      RET.CODE% = SESS.NUM.UTILITY ("C", F19.SOFTS.SESS.NUM%, "")     ! 1.11 NWB

      IF CIP.ACTIVE% THEN BEGIN                                       ! 1.11 NWB
         IF END #PHF.SESS.NUM% THEN FILE.OPEN.ERROR                   ! 1.11 NWB
         CREATE POSFILE PHF.FILE.NAME$ KEYED 6,,, 600000              \ 1.11 NWB ! 1.14BMG
               RECL PHF.RECL% AS PHF.SESS.NUM% MIRRORED PERUPDATE     ! 1.11 NWB
         GOTO PHF.CREATE.RESUME                                       ! 1.11 NWB
      ENDIF                                                           ! 1.11 NWB

      GOTO SKIP.CIP.FILES                                             ! 1.11 NWB

\*******************************************************************************
\***
\***   FILE.OPEN.ERROR:
\***
\***         set FUNCTION.FLAG$ to "R"
\***             PASSED.INTEGER% to file no and
\***             PASSED.STRING$ to ""
\***         use SESS.NUM.UTILITY function to obtain reporting number for ERRF%
\***         IF F20.RETURN.CODE% <> 0 THEN
\***            GOTO PROGRAM.EXIT
\***         ENDIF
\***
\***         set ERRFILE$ to CHR$(F20.INTEGER.FILE.NO%)
\***         set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***
\***         CALL APPLICATION.LOG to log error number 501
\***         GOTO FUNCTION.EXIT
\***
\***
\***   FUNCTION.EXIT:
\***
\***      EXIT the function
\***
\***
\***
\-------------------------------------------------------------------------------

   FILE.OPEN.ERROR:

      FUNCTION.FLAG$  = "R"                                            ! BBCW
      PASSED.INTEGER% = FILE.NO%                                       ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! BBCW

      EVENT.NO% = 6
      INTEGER1% = FILE.NO%                                             ! BBCW
      ERRFILE$  = CHR$(F20.INTEGER.FILE.NO%)                           ! BBCW

      MESSAGE.NUMBER% = 501
      VAR.STRING.1$ = "O" + ERRFILE$
      VAR.STRING.2$ = F20.STRING.FILE.NO$                              ! BBCW
      RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                    \ DAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NO%)                         :\

      OPEN.IRF.UPDT = 1                                                ! HAW


   FUNCTION.EXIT:

      EXIT FUNCTION


\*******************************************************************************
\***
\***   FILE.READ.ERROR:
\***
\***         set FUNCTION.FLAG$ to "R"
\***             PASSED.INTEGER% to file no and
\***             PASSED.STRING$ to ""
\***         use SESS.NUM.UTILITY function to obtain reporting number for ERRF%
\***         IF F20.RETURN.CODE% <> 0 THEN
\***            GOTO PROGRAM.EXIT
\***         ENDIF
\***
\***         set ERRFILE$ to CHR$(F20.INTEGER.FILE.NO%)
\***         set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***
\***         CALL APPLICATION.LOG to log error number 503
\***         GOTO FUNCTION.EXIT
\***
\***
\***      EXIT the function
\***
\***
\***
\-------------------------------------------------------------------------------

   FILE.READ.ERROR:

      FUNCTION.FLAG$  = "R"                                            ! BBCW
      PASSED.INTEGER% = FILE.NO%                                       ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

      EVENT.NO% = 6
      INTEGER1% = FILE.NO%                                             ! BBCW
      ERRFILE$ = CHR$(F20.INTEGER.FILE.NO%)                            ! BBCW


      MESSAGE.NUMBER% = 503
      VAR.STRING.1$ = "R" + ERRFILE$
      VAR.STRING.2$ = F20.STRING.FILE.NO$                              ! BBCW
      RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                    \ DAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NO%)                         :\

      OPEN.IRF.UPDT = 1                                                ! DAW
      GOTO FUNCTION.EXIT


\*******************************************************************************
\*********************** subroutine follows ************************************
\*******************************************************************************
\***
\*** UPDT.OPEN.ERROR.DETECTED
\***
\***   Set the function 19 return code to 1.
\***
\***   Log an event 18, message number 553 for a chaining error.  Other errors
\***   are logged as event 101s by calling the standard error detected function.
\***
\***   If the calling program is a batch program then processing stops.  If it
\***   is a screen program then the first program in the current application is
\***   chained back to.
\***
\-------------------------------------------------------------------------------

   UPDT.OPEN.ERROR.DETECTED:

      OPEN.IRF.UPDT = 1                                                ! DAW

      ! lines deleted from here                                        ! DAW


      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF19 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ BBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ BBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ DAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)


      IF ERR <> "CM" AND ERR <> "CT" THEN                              \
         RET.CODE% = STANDARD.ERROR.DETECTED (ERRN,                    \ DAW
                                              ERRF%,                   \ DAW                                       \
                                              ERRL,                    \ DAW
                                              ERR)                     ! DAW
      RESUME FUNCTION.EXIT
   PROGRAM.EXIT:                                                       ! BBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN                                \
         STOP

      %INCLUDE PSBCHNE.J86

   END FUNCTION


\*******************************************************************************
\*********                 Function UPDT.IRF.TIF.UPDT                 ********** ! ! LCSk
\*******************************************************************************
\***
\*** FUNCTION UPDT.IRF.TIF.UPDT (new IRF data,                                   ! ! LCSk
\***                             add/change/delete flag
\***                             irf locked flag) PUBLIC
\***
\***     If irf.locked flag is 2 chars, then 1st is irf.locked.flag and the ! 1.1
\***     second is the calling program type (R/L/E - RPD/Local/Emergency)   ! 1.1
\***     which will default to 'R' if not present.                          ! 1.1
\***
\-------------------------------------------------------------------------------

  !FUNCTION UPDT.IRF.UPDT (NEW.IRF.DATA$,                              ! ! LCSk
   FUNCTION UPDT.IRF.TIF.UPDT (NEW.IRF.DATA$,                          \ ! ! LCSk
                               ACD.FLAG$,                              \ BBCW
                               IRF.LOCKED.FLAG$)                       \ BBCW
   PUBLIC

      STRING    ACD.FLAG$,                                             \
\ 1 line deleted from here                                             \ DAW
                ERRFILE$,                                              \
                ERRNUM$,                                               \
                FILE$,                                                 \
                FUNCTION.FLAG$,                                        \ BBCW
                IRF.LOCKED.FLAG$,                                      \ BBCW
\ 1 line deleted from here                                             \ DAW
                NEW.IRF.DATA$,                                         \
                NEW.LABEL$,                                            \ 1.11 NWB
                NEW.TYPE$,                                             \ 1.11 NWB
                ON.TIF.FLAG$,                                          \
\ 1 line deleted from here                                             \ DAW
                PASSED.STRING$,                                        \ BBCW
                PIPE.NUM$,                                             \ BBCW
                PIPE.OPEN.FLAG$,                                       \ CJAL
                RETURN.CODE$,                                          \
                STRING.ERRL$,                                          \
                TMCF.EOF$,                                             \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$

      INTEGER*1 EALGARLP.REPORT.NUM%,                                  \ BBCW
                EVENT.NUM%,                                            \
                FILE.NO%,                                              \ BBCW
                INTEGER1%,                                             \
                MSGGRP%,                                               \
                ON.LP%,                                                \ 1.11 NWB
                PASSED.INTEGER%,                                       \ BBCW
                pc%,                                                   \ 1.11 NWB
                PHF.DATA%,                                             \ 1.11 NWB
                SEVERITY%,                                             \
                TMCF.SAVED.ACTION                                      !  NFH

      INTEGER   EALGARLP.SESS.NUM%,                                    \ BBCW
                EVENT.NO%,                                             \
                F17.RETURN.CODE%,                                      \ DAW
                MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                RC%,                                                   \ 1.11 NWB
                RET.CODE%,                                             \
                SESSION.NUMBER%,                                       \
                TERM%,                                                 \
               \UPDT.IRF.UPDT,                                         \ DAW ! ! LCSk
                UPDT.IRF.TIF.UPDT,                                     \ ! ! LCSk
                WAIT.PERIOD%                                           ! BBCW

      INTEGER*4 INTEGER4%,                                             \
                NEW.PRICE%,                                            \ 1.11 NWB
                OLD.PRICE%,                                            \ 1.11a NWB
                TMCF.SIZE%

\*******************************************************************************
\***
\***   ON ERROR goto UPDT.UPDT.ERROR.DETECTED  (after logging the error, the
\***                                            function is immediately left)
\***
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***
\***   set chaining module to first module in current application
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO UPDT.UPDT.ERROR.DETECTED

!     Lines deleted                                                       ! 1.8 RC
!     Un-necessary duplication of OPEN.IRF.UPDT file SET calls            ! 1.8 RC

     !UPDT.IRF.UPDT = 0                                                ! DAW ! ! LCSk
      UPDT.IRF.TIF.UPDT = 0                                               ! ! LCSk
      UNIQUE$ = "          "
      ERRNUM$ = "    "
      ERRFILE$ = " "
      STRING.ERRL$ = "      "

      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                !1.10 SWM

!     Debug code removed in revision 1.6                                  ! 1.8 RC

\*******************************************************************************
\***
\***  REMOVED IF the controller id is not "DE" OR                      ! LCSk
\***  REMOVED   the on tif flag is off THEN                            ! LCSk
\***  REMOVED   GOTO ONLY.PROCESS.IRF                                  ! LCSk
\***  REMOVED endif                                                    ! LCSk
\***
\***  IF the ACD flag is not ADD THEN
\***     read the TIF using the IRF bar code as key, to determine the value of
\***     the on tif flag
\***
\***  GOSUB CREATE.PIPE
\***
\***  IF END of the TMCF then FILE.READ.ERROR
\***  CALL READ.TMCF.HEADER
\***
\***  IF acd flag = add THEN
\***     GOSUB ADD.TIF
\***  ELSE
\***     IF acd flag = change THEN
\***        GOSUB CHANGE.TIF
\***     ELSE
\***        GOSUB DELETE.TIF
\***     endif
\***  endif
\***
\***  set FILE.NO% to TMCF session number
\***  WRITE the data record to the end of the TMCF
\***
\***  IF tmcf size > tmcf max size AND
\***     tmcf full flag is "N" THEN
\***     set tmcf full flag to "Y"
\***     CALL ADXERROR logging message number A301,
\***                           severity 3,
\***                           event number 6.
\***  endif
\***
\***  CLOSE the pipe
\***
\***  FUNCTION.EXIT:
\***
\***     EXIT FUNCTION
\***
\***
\*******************************************************************************

    NEW.TYPE$ = "R"                     ! Default to RPD              ! 1.11 NWB
    IF LEN(IRF.LOCKED.FLAG$) = 2 THEN BEGIN                           ! 1.11 NWB
       NEW.TYPE$        = RIGHT$(IRF.LOCKED.FLAG$, 1)                 ! 1.11 NWB
       IRF.LOCKED.FLAG$ = LEFT$ (IRF.LOCKED.FLAG$, 1)                 ! 1.11 NWB
    ENDIF                                                             ! 1.11 NWB

    ! Save passed price for later use in PHF Process                  ! 1.11a NWB
    OLD.PRICE% = 0                                                    ! 1.11a NWB
    IF ACD.FLAG$ = "CHANGE" THEN                                      \ 1.11a NWB
       OLD.PRICE% = VAL(UNPACK$(IRF.SALEPRIC$))                       ! 1.11a NWB

    CALL SPLIT.NEW.IRF.DATA$ ! Sets IRF variables from IRF record string  ! 1.8 RC
                             ! TIF variables only set prior to TIF write  ! 1.8 RC
                             ! Moved to function start for revision 1.9   ! 1.9 RC

   !IF NODE.ID$ <> "DE" THEN                                         \ ! LCSk
   !   GOSUB ONLY.PROCESS.IRF                                       :\ ! LCSk
   !   GOTO FUNCTION.EXIT                                              ! LCSk

!! setting of TIF.FORMAT moved to the OPEN routine where        !FSJW
!! the RECL's are set                           !FSJW

      IF ACD.FLAG$ <> "ADD" THEN \                                     ! CJAL
          BEGIN                                                        ! 1.8 RC
!         Removed redundant code processing                            ! 1.8 RC
!         unused TIF and TMCF file formats                             ! 1.8 RC
          TIF.BAR.CODE$ = IRF.BAR.CODE$                                ! 1.8 RC
          RET.CODE% = READ.TIF.BOOTS.DATA                              ! DW
          GOSUB TEST.TIF.RETURN.CODE                                   ! DW
          ENDIF                                                        ! 1.8 RC

!   Label removed from here                                            ! CJAL

      GOSUB CREATE.PIPE

      FILE.NO%     = TMCF.SESS.NUM%                                    ! BBCW
      TMCF.REC.NO% = 1
      RET.CODE% = READ.TMCF.HEADER.LOCKED                              ! FSP
      IF RET.CODE% <> 0 THEN BEGIN                                     ! FSP
         GOSUB FILE.READ.ERROR                                         ! FSP
      ENDIF                                                            ! FSP

\**********************************************************************!  NFH
\ REM Lines moved from here to further down                            !  NFH

      IF ACD.FLAG$ = "ADD" THEN                                        \
         GOSUB ADD.TIF                                                :\
         GOSUB UPDATE.TIF                                              \
      ELSE                                                             \
         IF ACD.FLAG$ = "CHANGE" THEN                                  \
            GOSUB CHANGE.TIF                                          :\
            GOSUB UPDATE.TIF                                           \
         ELSE                                                          \
            GOSUB DELETE.TIF

      TMCF.SAVED.ACTION = TMCF.ACTION%                                 !  NFH
                                                                       !  NFH
\**********************************************************************!  NFH
\ REM Lines moved to here                                              !  NFH
                                                                       !  NFH

! Lines moved to TMCF.UPDATE
!     TMCF.REC.NO% = 1 + SIZE(TMCF.FILE.NAME$) / 51                    !  NFH

!     TMCF.ACTION% = TMCF.SAVED.ACTION                                 !  NFH

!     TMCF.MNTDATA$ = NEW.IRF.DATA$

!     RET.CODE% =  WRITE.TMCF.DATA                                     ! FSP
!     IF RET.CODE% <>  0 THEN BEGIN                                    ! FSP
!        GOSUB FILE.WRITE.ERROR                                        ! FSP
!     ENDIF                                                            ! FSP
! End of moved lines

      TMCF.SIZE% = SIZE("EALTMCFR")

      IF TMCF.SIZE% > GAOPT.DFLTSIZE% AND                              \
         TMCF.FULL$ = "N" THEN                                         \
         TMCF.FULL$ = "Y"                                             :\
         MSGNUM% = 301                                                :\
         MSGGRP% = ASC("A")                                           :\ CJAL
         SEVERITY% = 3                                                :\
         TERM% = 0                                                    :\
         EVENT.NUM% = 6                                               :\
         UNIQUE$ = "W" + STR$(TMCF.REPORT.NUM%)                       :\ BBCW
         CALL ADXERROR (TERM%,                                         \
                        MSGGRP%,                                       \
                        MSGNUM%,                                       \
                        SEVERITY%,                                     \
                        EVENT.NUM%,                                    \
                        UNIQUE$)

!  Following line moved up from end of this section                    ! CJAL
   FUNCTION.EXIT:                                                      ! CJAL

      IF PIPE.OPEN.FLAG$ = "Y" THEN                                    \ CJAL
         PIPE.OPEN.FLAG$ = "N"                                        :\ CJAL
         CLOSE EALGARLP.SESS.NUM%                                      ! BBCW

      FUNCTION.FLAG$  = "C"                                            ! BBCW
      PASSED.INTEGER% = EALGARLP.SESS.NUM%                             ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

!  Label moved from here                                               ! CJAL

      EXIT FUNCTION


\*******************************************************************************
\*********************** subroutines follow ************************************
\*******************************************************************************
\***
\***  TMCF.UPDATE
\***
\***  Lines moved here from above
\***
\*******************************************************************************

TMCF.UPDATE:

      TMCF.REC.NO% = 1 + SIZE(TMCF.FILE.NAME$) / 51                    !  NFH

!     TMCF.ACTION% = TMCF.SAVED.ACTION                                 !  NFH

      TMCF.MNTDATA$ = NEW.IRF.DATA$ ! IRF record string passed to function

      RET.CODE% =  WRITE.TMCF.DATA                                     ! FSP
      IF RET.CODE% <>  0 THEN BEGIN                                    ! FSP
         GOSUB FILE.WRITE.ERROR                                        ! FSP
      ENDIF

      TMCF.MAINTLVL% = TMCF.REC.NO%

RETURN

\*******************************************************************************
\***
\***  CREATE.PIPE:
\***
\***     ON ERROR GOTO PIPE.ERROR
\***
\***     set pipe open flag to "N"
\***     set return code to null
\***
\***     CREATE the pipe EALGARLP AS 1
\***
\***     set wait period to zero
\***
\***     WHILE return code is not null (i.e. create failed)
\***
\***        IF wait period exceeds half an hour THEN
\***           log event 52 (Time out on pipe)
\***           set return code to 1
\***           exit function
\***
\***        WAIT 1 second
\***        set return code to null
\***        CREATE the pipe EALGARLP AS 1
\***
\***     WEND
\***
\***     ON ERROR GOTO UPDT.UPDT.ERROR.DETECTED
\***
\***     set pipe open flag to "Y"
\***
\***     RETURN
\***
\***  PIPE.ERROR:
\***
\***     set return code to ERR
\***
\***     RESUME
\***
\***
\-------------------------------------------------------------------------------

   CREATE.PIPE:

      PIPE.OPEN.FLAG$ = "N"                                            ! CJAL
      RETURN.CODE$ = ""

      EALGARLP.REPORT.NUM% = 104                                       ! BBCW
      PIPE.NUM$ = "104"                                                ! BBCW
      FUNCTION.FLAG$       = "O"                                       ! BBCW
      PASSED.INTEGER%      = EALGARLP.REPORT.NUM%                      ! BBCW
      PASSED.STRING$       = "PI:EALGARLP"                             ! BBCW

      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

      EALGARLP.SESS.NUM% = F20.INTEGER.FILE.NO%                        ! BBCW
      FILE.NO%           = F20.INTEGER.FILE.NO%                        ! BBCW

      ON ERROR GOTO PIPE.ERROR

      CREATE "PI:EALGARLP" AS EALGARLP.SESS.NUM%                       ! BBCW

      WAIT.PERIOD% = 0                                                 ! BBCW

      WHILE RETURN.CODE$ <> ""
         IF WAIT.PERIOD% > 1800 THEN                                   \ BBCW
            EVENT.NO% = 52                                            :\ BBCW
            MESSAGE.NUMBER% = 0                                       :\ BBCW
            VAR.STRING.1$ = "EALGARLP"                                :\ BBCW
            RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,              \ DAW
                                         VAR.STRING.1$,                \ BBCW
                                         VAR.STRING.2$,                \ BBCW
                                         EVENT.NO%)                   :\ BBCW
           !UPDT.IRF.UPDT = 1                                         :\ DAW ! ! LCSk
            UPDT.IRF.TIF.UPDT = 1                                     :\ ! ! LCSk
            GOTO FUNCTION.EXIT                                         ! BBCW

         WAIT; 1000
         RETURN.CODE$ = ""
         CREATE "PI:EALGARLP" AS EALGARLP.SESS.NUM%                    ! BBCW
         WAIT.PERIOD% = WAIT.PERIOD% + 1                               ! BBCW

      WEND

      ON ERROR GOTO UPDT.UPDT.ERROR.DETECTED
      PIPE.OPEN.FLAG$ = "Y"                                            ! CJAL

      RETURN

   PIPE.ERROR:

      RETURN.CODE$ = ERR

      RESUME


\*******************************************************************************
\***
\***  ADD.TIF:
\***
\***     check size of TIF - if too large, GOSUB CHECK.MESSAGE.DISPLAYED
\***          and exit function
\***
\***     set up individual IRF fields from new irf data
\***     set FILE.NO% to IRF.SESS.NUM%
\***     IF irf locked flag = "Y" THEN
\***        CALL WRITE.IRF.HOLD.UNLOCK
\***     ELSE
\***        CALL WRITE.IRF.HOLD
\***     ENDIF
\***     set tmcf action to 2
\***     set tmcf rcdkey to irf key
\***     add 1 to tmcf numrecs
\***     set FILE.NO% to TMCF.SESS.NUM%
\***     CALL WRITE.TMCF.HEADER.HOLD.UNLOCK
\***
\***     RETURN
\***
\-------------------------------------------------------------------------------

   ADD.TIF:

      IF GAOPT.NUMITEMS% < TMCF.NUMRECS% THEN                          \ CJAL
         GOSUB CHECK.MESSAGE.DISPLAYED                                :\ CJAL
         GOSUB FUNCTION.EXIT                                           ! CJAL

!     Lines deleted - IRF variables now set by SPLIT.NEW.IRF.DATA$        ! 1.8 RC

      FILE.NO% = IRF.SESS.NUM%                                         ! BBCW
      IF IRF.LOCKED.FLAG$ = "Y" THEN BEGIN                             ! BBCW

         RET.CODE% =  WRITE.IRF.HOLD.UNLOCK                            ! BBCW
         IF RET.CODE% <> 0 THEN BEGIN                                  ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
      ENDIF ELSE BEGIN                                                 ! BBCW
                                                                       ! FSP
         RET.CODE% = WRITE.IRF.HOLD                                    ! FSP  ! BBCW
         IF RET.CODE% <> 0 THEN BEGIN                                  ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
     ENDIF                                                             ! FSP

     GOSUB PROCESS.PHF                                                ! 1.11 NWB

      TMCF.ACTION% = 2
! DW  TMCF.MAINTLVL% = TMCF.MAINTLVL% + 1                              !  NFH
      TMCF.RCDKEY$ = MID$(NEW.IRF.DATA$,1,11)
      TMCF.NUMRECS% = TMCF.NUMRECS% + 1
      FILE.NO% = TMCF.SESS.NUM%                                        ! BBCW

      GOSUB TMCF.UPDATE                                                ! DW

      TMCF.REC.NO% = 1                                                 ! DW
      RET.CODE% = WRITE.TMCF.HEADER.HOLD.UNLOCK
      IF RET.CODE% <> 0 THEN BEGIN                                     ! FSP
         GOSUB FILE.WRITE.ERROR                                        ! FSP
      ENDIF                                                            ! FSP


      RETURN


\*******************************************************************************
\***
\***  CHECK.MESSAGE.DISPLAYED:
\***
\***     If the tif full flag is "N" (ie. 1st time this has happened) THEN
\***        set tif full flag to "Y" call ADXERROR to log an event 42
\***
\***     RETURN
\***
\-------------------------------------------------------------------------------

     CHECK.MESSAGE.DISPLAYED:                                          ! CJAL

       IF TIF.FULL$ = "N" THEN                                         \
          TIF.FULL$ = "Y"                                             :\
          MSGNUM% = 300                                               :\
          MSGGRP% = ASC("A")                                          :\
          SEVERITY% = 3                                               :\
          TERM% = 0                                                   :\
          EVENT.NUM% = 46                                             :\
          UNIQUE$ = STR$(TIF.REPORT.NUM%) +                            \ BBCW
                   RIGHT$(UNPACK$(TIF.BAR.CODE$),8)                   :\ BBCW
          CALL ADXERROR (TERM%,                                        \
                         MSGGRP%,                                      \
                         MSGNUM%,                                      \
                         SEVERITY%,                                    \
                         EVENT.NUM%,                                   \
                         UNIQUE$)                                      !

          RETURN                                                       ! CJAL


\*******************************************************************************
\***
\***  CHANGE.TIF:
\***
\***     set up individual IRF fields from new irf data
\***     set FILE.NO% to irf session no
\***     IF irf locked flag = "Y" THEN
\***        CALL WRITE.IRF.HOLD.UNLOCK
\***     ELSE
\***        CALL WRITE.IRF.HOLD
\***     ENDIF
\***
\***     set tmcf action to 2
\***     set tmcf rcdkey to irf key
\***     set FILE.NO% to tmcf session no
\***
\***     RETURN
\***
\-------------------------------------------------------------------------------

   CHANGE.TIF:

!     Lines deleted - IRF variables now set by SPLIT.NEW.IRF.DATA$        ! 1.8 RC

      FILE.NO%           = IRF.SESS.NUM%                               ! BBCW

      IF IRF.LOCKED.FLAG$ = "Y" THEN BEGIN                             ! BBCW

         RET.CODE% =  WRITE.IRF.HOLD.UNLOCK                            ! BBCW
         IF RET.CODE% <> 0 THEN BEGIN                                  ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
      ENDIF ELSE BEGIN                                                 ! BBCW
                                                                       ! FSP
         RET.CODE% = WRITE.IRF.HOLD                                    ! FSP
         IF RET.CODE% <> 0 THEN BEGIN                                  ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
      ENDIF                                                            ! FSP

     GOSUB PROCESS.PHF                                                ! 1.11 NWB

      TMCF.ACTION%   = 2
! DW  TMCF.MAINTLVL% = TMCF.MAINTLVL% + 1                              !  NFH
      TMCF.RCDKEY$   = MID$(NEW.IRF.DATA$,1,11)
      FILE.NO%       = TMCF.SESS.NUM%                                  ! BBCW

      GOSUB TMCF.UPDATE                                                ! DW

      TMCF.REC.NO% = 1                                                 ! DW
      RET.CODE% = WRITE.TMCF.HEADER.HOLD.UNLOCK
      IF RET.CODE% <> 0 THEN BEGIN                                     ! FSP
         GOSUB FILE.WRITE.ERROR                                        ! FSP
      ENDIF                                                            ! FSP


      RETURN


\*******************************************************************************
\***
\***  UPDATE.TIF:
\***
\***     set up front part of TIF record from new IRF data
\***             (all formats use this data)
\***
\***     IF tif format is 1 THEN
\***        set up remainder of TIF record from new IRF data
\***        set FILE.NO% to tif session no
\***        CALL WRITE.TIF.FULL.PLUS.USER.DATA
\***     ELSE
\***        IF tif format is 2 THEN
\***           set up remainder of TIF record from new IRF data
\***           set FILE.NO% to tif session no
\***           CALL WRITE.TIF.FULL.NO.USER.DATA
\***        ELSE
\***           IF tif format is 3 THEN
\***              set up remainder of TIF record from new IRF data
\***              set FILE.NO% to tif session no
\***              CALL WRITE.TIF.SHORT.PLUS.USER.DATA
\***           ELSE
\***              IF tif format is 4 THEN
\***                 set up remainder of TIF record from new IRF data
\***                 set FILE.NO% to tif session no
\***                 CALL WRITE.TIF.SHORT.NO.USER.DATA
\***              ELSE
\***                 IF tif format is 5 THEN
\***                    set up remainder of TIF record from new IRF data
\***                    set FILE.NO% to tif session no
\***                    CALL WRITE.TIF.JUST.USER.DATA
\***                 ELSE
\***                    IF tif format is 6 THEN
\***                       set up remainder of TIF record from new IRF data
\***                       set FILE.NO% to tif session no
\***                       CALL WRITE.TIF.NO.DESC.OR.DATA
\***                    endif
\***                 endif
\***              endif
\***           endif
\***        endif
\***     endif
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

UPDATE.TIF:

!   Lines deleted - TIF variables now set by SPLIT.TIF.IRF.DATA$          ! 1.8 RC

    TIF.IRF.DATA$ EQ NEW.IRF.DATA$ ! IRF record string passed to function ! 1.8 RC

    CALL SPLIT.TIF.IRF.DATA$ ! Sets TIF variables from IRF record string  ! 1.8 RC

!   Removed redundant code processing unused TIF and TMCF file formats    ! 1.8 RC

    FILE.NO% = TIF.SESS.NUM%                                      ! DW

    RET.CODE% = WRITE.TIF.BOOTS.DATA                              ! DW
    IF RET.CODE% <>  0 THEN BEGIN                                 ! DW
        GOSUB FILE.WRITE.ERROR                                     ! DW
    ENDIF                                                         ! DW

RETURN


\*******************************************************************************
\***
\***  DELETE.TIF:
\***
\***     update the TMCF header fields
\***     set FILE.NO% to tmcf session no
\***     CALL WRITE.TMCF.WITH.UNLOCK
\***
\***     delete the IRF record
\***     set FILE.NO% to irf session no
\***
\***     delete the TIF record
\***     set FILE.NO% to tif session no
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

   DELETE.TIF:

      TMCF.ACTION% = 5
! DW  TMCF.MAINTLVL% = TMCF.MAINTLVL% + 1                              !  NFH
      TMCF.RCDKEY$ = MID$(NEW.IRF.DATA$,1,11)
      TMCF.NUMRECS% = TMCF.NUMRECS% - 1
      FILE.NO% = TMCF.SESS.NUM%                                        ! BBCW

      GOSUB TMCF.UPDATE                                                ! DW

      TMCF.REC.NO% = 1                                                 ! DW
      RET.CODE% = WRITE.TMCF.HEADER.UNLOCK                             ! FSP
      IF RET.CODE% <>  0 THEN BEGIN                                    ! FSP
         GOSUB FILE.WRITE.ERROR                                        ! FSP
      ENDIF                                                            ! FSP

      GOSUB DELETE.IRF.AND.IRFDEX.RECS                                     ! 1.9 RC

      GOSUB PROCESS.PHF                                               ! 1.11 NWB

      FILE.NO% = IRF.SESS.NUM%                                         ! BBCW

!     Lines deleted                                                       ! 1.8 RC

      TIF.BAR.CODE$ = RIGHT$(TIF.BAR.CODE$,6)                       ! DW
      DELREC TIF.SESS.NUM%; TIF.BAR.CODE$
      FILE.NO% = TIF.SESS.NUM%                                         ! BBCW

      RETURN


\*******************************************************************************
\***
\***  ONLY.PROCESS.IRF:
\***
\***     IF acd flag is add THEN
\***        GOSUB ONLY.ADD.IRF
\***     ELSE
\***        IF acd flag is change THEN
\***           GOSUB ONLY.CHANGE.IRF
\***        ELSE
\***           GOSUB ONLY.DELETE.IRF
\***        endif
\***     endif
\***
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

   ONLY.PROCESS.IRF:

      IF ACD.FLAG$ = "ADD" THEN                                        \
         GOSUB ONLY.ADD.IRF                                            \
      ELSE                                                             \
         IF ACD.FLAG$ = "CHANGE" THEN                                  \
            GOSUB ONLY.CHANGE.IRF                                      \
         ELSE                                                          \
            GOSUB ONLY.DELETE.IRF

      GOSUB PROCESS.PHF                                               ! 1.11 NWB

      RETURN


\*******************************************************************************
\***
\***  ONLY.ADD.IRF:
\***
\***     format new irf data into individual fields
\***     set FILE.NO% to irf session no
\***
\***     CALL WRITE.IRF
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

   ONLY.ADD.IRF:

!   Lines deleted - IRF variables now set by SPLIT.NEW.IRF.DATA$          ! 1.8 RC

    FILE.NO% = IRF.SESS.NUM%                                         ! BBCW
    RET.CODE% = WRITE.IRF                                            ! FSP
    IF RET.CODE% <>  0 THEN BEGIN                                    ! FSP
       GOSUB FILE.WRITE.ERROR                                        ! FSP
    ENDIF                                                            ! FSP


    RETURN


\*******************************************************************************
\***
\***  ONLY.CHANGE.IRF:
\***
\***     format new irf data into individual fields
\***     set FILE.NO% to irf session no
\***
\***     IF irf locked flag = "Y" THEN
\***        CALL WRITE.IRF.HOLD.UNLOCK
\***     ELSE
\***        CALL WRITE.IRF.HOLD
\***     ENDIF
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

   ONLY.CHANGE.IRF:

!     Lines deleted - IRF variables now set by SPLIT.NEW.IRF.DATA$     ! 1.8 RC

      FILE.NO% = IRF.SESS.NUM%                                         ! BBCW

      IF IRF.LOCKED.FLAG$ = "Y" THEN BEGIN                             ! BBCW

         RET.CODE% = WRITE.IRF.UNLOCK                                  ! BBCW
         IF RET.CODE% <>  0 THEN BEGIN                                 ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
      ENDIF ELSE BEGIN                                                             \ BBCW
                                                                       ! FSP
         RET.CODE% = WRITE.IRF                                         ! BBCW
         IF RET.CODE% <>  0 THEN BEGIN                                 ! FSP
            GOSUB FILE.WRITE.ERROR                                     ! FSP
         ENDIF                                                         ! FSP
                                                                       ! FSP
      ENDIF                                                            ! FSP

      RETURN


\*******************************************************************************
\***
\***  ONLY.DELETE.IRF:
\***
\***
\***     DELREC the IRF record
\***     If the IRF.BARCODE$ is the Boots code IRF.BARCODE$ ...
\***         Delete any corresponding IRFDEX record that may exist.
\***
\***     RETURN
\***
\***
\-------------------------------------------------------------------------------

   ONLY.DELETE.IRF:
   DELETE.IRF.AND.IRFDEX.RECS:                                             ! 1.9 RC

      IRF.BAR.CODE$ = MID$(NEW.IRF.DATA$,1,11)

      DELREC IRF.SESS.NUM%; IRF.BAR.CODE$

      IF IRF.BAR.CODE$ EQ \                                                ! 1.9 RC
           PACK$("0000000000000000") + IRF.BOOTS.CODE$ THEN \              ! 1.9 RC
          BEGIN                                                            ! 1.9 RC
          IF END # F19.IRFDEX.SESS.NUM% THEN IRFDEX.REC.DELETED            ! 1.9 RC
          DELREC F19.IRFDEX.SESS.NUM%; IRF.BOOTS.CODE$                     ! 1.9 RC
IRFDEX.REC.DELETED:                                                        ! 1.9 RC
          ENDIF                                                            ! 1.9 RC

      RETURN


\*******************************************************************************
\***
\*** Process Product History File
\***
\*******************************************************************************

PROCESS.PHF:                                                          ! 1.11 NWB

      IF PHF.OPEN.FLAG$ <> "Y" THEN RETURN                            ! 1.11 NWB

      ! Reject Group Codes                                            ! 1.16 NWB
      IF LEFT$(IRF.BAR.CODE$, 5) <> PACK$("0000000000") THEN BEGIN    ! 1.16 NWB
         RETURN                                                       ! 1.16 NWB
      ENDIF                                                           ! 1.16 NWB

      ! IRF.BAR.CODE$ = 11 bytes UPD to PHF.BAR.CODE$ = 6 bytes UPD   ! 1.11 NWB
      PHF.BAR.CODE$ =                                                 \
            PACK$(RIGHT$("000000000000"+UNPACK$(IRF.BAR.CODE$), 12))  ! 1.12 PAB

      IF VAL(UNPACK$(PHF.BAR.CODE$)) <> 0  AND                        \ 1.12 PAB
         LEN (PHF.BAR.CODE$) = 6 THEN BEGIN                           ! 1.12 PAB

         IF ACD.FLAG$ = "DELETE" THEN BEGIN                           ! 1.11 NWB
            IF END #PHF.SESS.NUM% THEN PHF.REC.DELETED                ! 1.11 NWB
            DELREC PHF.SESS.NUM%; PHF.BAR.CODE$                       ! 1.11 NWB
PHF.REC.DELETED:                                                      ! 1.11 NWB
            RETURN                                                    ! 1.11 NWB
         ENDIF                                                        ! 1.11 NWB

! Following code extracted into new external function PSBF46

         NEW.PRICE% = VAL(UNPACK$(IRF.SALEPRIC$))                     ! 1.15 NWB

         IF GET.LABEL.TYPE(PHF.BAR.CODE$,                             \ 1.15 NWB
                           OLD.PRICE%,                                \ 1.15 NWB
                           NEW.PRICE%,                                \ 1.15 NWB
                           "Y" ) <> 0 THEN RETURN                     ! 1.15 NWB

      ENDIF                                                           ! 1.15 NWB

RETURN                                                                ! 1.15 NWB

\        NEW.LABEL$ = ""                                              ! 1.11 NWB
\        NEW.PRICE% = VAL(UNPACK$(IRF.SALEPRIC$))                     ! 1.11 NWB

\        PHF.DATA% = -1                                               ! 1.11 NWB

\        IF LEN(PHF.BAR.CODE$) = 6 THEN BEGIN                         ! 1.12 PAB
\           RC% = READ.PHF                                            ! 1.11 NW
\        ENDIF ELSE BEGIN
\           RC% = 1                                                   ! 1.12 PAB
\        ENDIF

\        IF RC% <> 0 THEN BEGIN                                       ! 1.11 NWB

\           PHF.DATA% = 0                                             ! 1.11 NWB

\           PHF.CURR.PRICE%      = 0                                  ! 1.11 NWB
\           PHF.PEND.PRICE%      = 0                                  ! 1.11 NWB
\           PHF.HIST1.PRICE%     = 0                                  ! 1.11 NWB
\           PHF.HIST2.PRICE%     = 0                                  ! 1.11 NWB

\           PHF.CURR.TYPE$       = " "                                ! 1.11 NWB
\           PHF.PEND.TYPE$       = " "                                ! 1.11 NWB
\           PHF.HIST1.TYPE$      = " "                                ! 1.11 NWB
\           PHF.HIST2.TYPE$      = " "                                ! 1.11 NWB

\           PHF.CURR.DATE$       = PACK$("000000")                    ! 1.11 NWB
\           PHF.PEND.DATE$       = PHF.CURR.DATE$                     ! 1.11 NWB
\           PHF.HIST1.DATE$      = PHF.CURR.DATE$                     ! 1.11 NWB
\           PHF.HIST2.DATE$      = PHF.CURR.DATE$                     ! 1.11 NWB
\           PHF.LAST.INC.DATE$   = PHF.CURR.DATE$                     ! 1.11 NWB

\           PHF.FILLER$          = "  "                               ! 1.11 NWB

\        ENDIF                                                        ! 1.11 NWB

\        ! Don't process if price hasn't changed                      ! 1.11 NWB
\        IF NEW.PRICE% = PHF.CURR.PRICE% THEN BEGIN                   ! 1.11 NWB
\           RETURN                                                    ! 1.11 NWB
\        ENDIF                                                        ! 1.11 NWB

\        CALL CALC.BOOTS.CODE.CHECK.DIGIT(UNPACK$(IRF.BOOTS.CODE$))   ! 1.11 NWB
\        IDF.BOOTS.CODE$ = PACK$(RIGHT$("00000000"                    \ 1.11 NWB
\                                     + UNPACK$(IRF.BOOTS.CODE$)      \ 1.11 NWB
\                                     + F18.CHECK.DIGIT$, 8))         ! 1.11 NWB

\        SAV.IDF.SESS.NUM% = IDF.SESS.NUM%                            ! 1.11 NWB
\        IDF.SESS.NUM% = F19.IDF.SESS.NUM%                            ! 1.11 NWB
\        RC% = READ.IDF                                               ! 1.11 NWB
\        IDF.SESS.NUM% = SAV.IDF.SESS.NUM%                            ! 1.11 NWB
\        SAV.IDF.SESS.NUM% = 0                                        ! 1.11 NWB

\        ! Check item is on live planner                              ! 1.11 NWB
\        ON.LP% = 0                                                   ! 1.11 NWB
\        IF SRITL.OPEN.FLAG$ = "Y" THEN BEGIN                         ! 1.11 NWB
\           SRITL.ITEM.CODE$ = IRF.BOOTS.CODE$                        ! 1.11 NWB
\           SRITL.RECORD.CHAIN% = 0                                   ! 1.11 NWB
\           SAV.SRITL.SESS.NUM% = SRITL.SESS.NUM%                     ! 1.11 NWB
\           SRITL.SESS.NUM% = F19.SRITL.SESS.NUM%                     ! 1.11 NWB
\           RC% = READ.SRITL                                          ! 1.11 NWB
\           SRITL.SESS.NUM% = SAV.SRITL.SESS.NUM%                     ! 1.11 NWB
\           SAV.SRITL.SESS.NUM% = 0                                   ! 1.11 NWB
\           IF RC% = 0 THEN BEGIN                                     ! 1.11 NWB
\              ON.LP% = -1                                            ! 1.11 NWB
\           ENDIF                                                     ! 1.11 NWB
\        ENDIF                                                        ! 1.11 NWB

\        !  If price increase set the last increase date              ! 1.11a NWB
\        IF (NEW.PRICE%  > OLD.PRICE%                                 \ 1.11a NWB
\            AND OLD.PRICE% > 0      )                                \ 1.11a NWB
\        OR (PHF.DATA% = -1                                           \ 1.11a NWB
\            AND NEW.PRICE%  > PHF.CURR.PRICE%) THEN BEGIN            ! 1.11a NWB
\           PHF.LAST.INC.DATE$ = PACK$(DATE$)                         ! 1.11 NWB
\        ENDIF                                                        ! 1.11 NWB

\        IF (IDF.BIT.FLAGS.1% AND 20H) = 0 THEN BEGIN                 ! 1.11 NWB
\           ! Not markdown item                                       ! 1.11 NWB
\           NEW.LABEL$ = "0"                             ! Standard   ! 1.11 NWB
\        ENDIF ELSE IF NEW.PRICE% <> PHF.CURR.PRICE% THEN BEGIN       ! 1.11 NWB

\           ! Get number of price changes in last 28 days             ! 1.11 NWB
\           !   (incl this one)                                       ! 1.11 NWB
\           F02.DATE$ = DATE$                                         ! 1.11 NWB
\           RC% = UPDATE.DATE(-28)                                    ! 1.11 NWB
\           pc% = 0                                                   ! 1.11 NWB
\           F02.DATE$ = PACK$(F02.DATE$)                              ! 1.11 NWB
\           IF F02.DATE$ < PHF.HIST2.DATE$ THEN BEGIN                 ! 1.11 NWB
\              pc% = 3                                                ! 1.11 NWB
\           ENDIF ELSE IF F02.DATE$ < PHF.HIST1.DATE$ THEN BEGIN      ! 1.11 NWB
\              pc% = 2                                                ! 1.11 NWB
\           ENDIF ELSE IF F02.DATE$ < PHF.CURR.DATE$  THEN BEGIN      ! 1.11 NWB
\              pc% = 1                                                ! 1.11 NWB
\           ENDIF                                                     ! 1.11 NWB

\           IF PHF.DATA% = 0                                          \ 1.11 NWB
\           OR ON.LP% = 0                                             \ 1.11 NWB
\           OR F02.DATE$ < PHF.LAST.INC.DATE$ THEN BEGIN              ! 1.11 NWB
\              ! If no price history available                        ! 1.11 NWB
\              ! OR Not on Live planner                               ! 1.11 NWB
\              ! OR a price increase within last 28 days              ! 1.11 NWB
\              NEW.LABEL$ = "3"                          ! Clearance  ! 1.11 NWB
\           ENDIF ELSE IF pc% = 1 THEN BEGIN                          ! 1.11 NWB
\              ! This is only change in last 28 days                  ! 1.11 NWB
\              NEW.LABEL$ = "1"                          ! Was/Now    ! 1.11 NWB
\           ENDIF ELSE BEGIN                                          ! 1.11 NWB
\              ! More than 1 change in last 28 days                   ! 1.11 NWB
\              NEW.LABEL$ = "2"                          ! Was/Was/Now! 1.11 NWB
\           ENDIF                                                     ! 1.11 NWB

\           !If already set to clearance, force to stay as clearance. ! 1.13 BMG
\           IF PHF.CURR.LABL$ = "3" THEN NEW.LABEL$ = "3"             ! 1.13 BMG
\           !Force markdown items to be flagged as new type "C"       ! 1.13 BMG
\           NEW.TYPE$ = "C"                                           ! 1.13 BMG
\           !If a WEEE item, force label to type 0                    ! 1.14 BMG
\           IF (IRF.INDICAT8% AND 10000000b) THEN NEW.LABEL$ = "0"    ! 1.14 BMG

\        ENDIF                                                        ! 1.11 NWB

\        IF NEW.LABEL$ <> "" THEN BEGIN                               ! 1.11 NWB

\           ! Ripple through the price history                        ! 1.11 NWB
\           PHF.HIST2.PRICE% = PHF.HIST1.PRICE%                       ! 1.11 NWB
\           PHF.HIST1.PRICE% = PHF.CURR.PRICE%                        ! 1.11 NWB
\           PHF.CURR.PRICE%  = NEW.PRICE%                             ! 1.11 NWB

\           PHF.HIST2.DATE$  = PHF.HIST1.DATE$                        ! 1.11 NWB
\           PHF.HIST1.DATE$  = PHF.CURR.DATE$                         ! 1.11 NWB
\           PHF.CURR.DATE$   = PACK$(DATE$)                           ! 1.11 NWB

\           PHF.HIST2.TYPE$  = PHF.HIST1.TYPE$                        ! 1.11 NWB
\           PHF.HIST1.TYPE$  = PHF.CURR.TYPE$                         ! 1.11 NWB
\           PHF.CURR.TYPE$   = NEW.TYPE$                              ! 1.11 NWB

\           PHF.CURR.LABL$   = NEW.LABEL$                             ! 1.11 NWB

\           PHF.FILLER$      = "  "                                   ! 1.11 NWB

\           RC% = WRITE.PHF                                           ! 1.11 NWB

\        ENDIF                                                        ! 1.11 NWB

\     ENDIF                                                           ! 1.11 NWB

\RETURN                                                               ! 1.11 NWB

\*******************************************************************************
\***
\***  TEST.TIF.RETURN.CODE
\***
\***  Tests return code from reading TIF and send control to para
\***  which updates the IRF only, then exits function
\***
\-------------------------------------------------------------------------------

      TEST.TIF.RETURN.CODE:                                              ! FSP
                                                                         ! FSP
         IF RET.CODE% <> 0 THEN BEGIN                                    ! FSP
            GOSUB TIF.READ.ERROR                                         ! FSP
         ENDIF                                                           ! FSP
                                                                         ! FSP
      RETURN                                                             ! FSP


\*******************************************************************************
\***
\***   FILE.READ.ERROR:
\***
\***         set FUNCTION.FLAG$ to "R"
\***             PASSED.INTEGER% to FILE.NO% and
\***             PASSED.STRING$ to ""
\***         use SESS.NUM.UTILITY function to obtain reporting no for FILE.NO%
\***         IF F20.RETURN.CODE% <> 0 THEN
\***            GOTO PROGRAM.EXIT
\***         ENDIF
\***
\***         set ERRFILE$ to CHR$(F20.INTEGER.FILE.NO%)
\***         set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***
\***         CALL APPLICATION.LOG to log error number 503
\***         GOTO FUNCTION.EXIT
\***
\***
\***      EXIT the function
\***
\***
\***
\-------------------------------------------------------------------------------

   FILE.READ.ERROR:

      FUNCTION.FLAG$  = "R"                                            ! BBCW
      PASSED.INTEGER% = FILE.NO%                                       ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

      EVENT.NO% = 6
      INTEGER1% = FILE.NO%                                             ! BBCW
      ERRFILE$  = CHR$(F20.INTEGER.FILE.NO%)                           ! BBCW

      MESSAGE.NUMBER% = 503
      VAR.STRING.1$ = "R" + ERRFILE$
      VAR.STRING.2$ = F20.STRING.FILE.NO$                              ! BBCW
      RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                    \ DAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NO%)                         :\

     !UPDT.IRF.UPDT = 1                                                ! DAW ! ! LCSk
      UPDT.IRF.TIF.UPDT = 1                                            ! ! LCSk
      GOTO FUNCTION.EXIT


\*******************************************************************************
\***
\***   FILE.WRITE.ERROR:
\***
\***         set FUNCTION.FLAG$ to "R"
\***             PASSED.INTEGER% to FILE.NO% and
\***             PASSED.STRING$ to ""
\***         use SESS.NUM.UTILITY function to obtain reporting no for FILE.NO%
\***         IF F20.RETURN.CODE% <> 0 THEN
\***            GOTO PROGRAM.EXIT
\***         ENDIF
\***
\***         set ERRFILE$ to CHR$(F20.INTEGER.FILE.NO%)
\***         set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***
\***         CALL APPLICATION.LOG to log error number 504
\***         GOTO FUNCTION.EXIT
\***
\***
\***      EXIT the function
\***
\***
\***
\-------------------------------------------------------------------------------

   FILE.WRITE.ERROR:

      FUNCTION.FLAG$  = "R"                                            ! BBCW
      PASSED.INTEGER% = FILE.NO%                                       ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

      EVENT.NO% = 6
      INTEGER1% = FILE.NO%                                             ! BBCW
      ERRFILE$ = CHR$(F20.INTEGER.FILE.NO%)                            ! BBCW


      MESSAGE.NUMBER% = 504
      VAR.STRING.1$   = "W" + ERRFILE$
      VAR.STRING.2$   = F20.STRING.FILE.NO$                            ! BBCW
      RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                    \ DAW
                                   VAR.STRING.1$,                      \
                                   VAR.STRING.2$,                      \
                                   EVENT.NO%)                         :\

     !UPDT.IRF.UPDT = 1                                                ! DAW ! ! LCSk
      UPDT.IRF.TIF.UPDT = 1                                            ! ! LCSk
      GOTO FUNCTION.EXIT


\*******************************************************************************
\***
\***  TIF.READ.ERROR:
\***
\***    GOSUB ONLY.PROCESS.IRF
\***
\***    GOTO FUNCTION.EXIT
\***
\***
\-------------------------------------------------------------------------------

   TIF.READ.ERROR:

!  2 lines removed here                                                ! CJAL
      GOSUB ONLY.PROCESS.IRF
      GOTO FUNCTION.EXIT


\*******************************************************************************
\***
\*** UPDT.UPDT.ERROR.DETECTED:
\***
\***   set UPDT.IRF.TIF.UPDT  to 1 (was) UPDT.IRF.UPDT                 ! ! LCSk
\***
\***         set FUNCTION.FLAG$ to "R"
\***             PASSED.INTEGER% to ERRF% and
\***             PASSED.STRING$ to ""
\***         use SESS.NUM.UTILITY function to obtain reporting number for ERRF%
\***         IF F20.RETURN.CODE% <> 0 THEN
\***            GOTO PROGRAM.EXIT
\***         ENDIF
\***
\***   set ERRFILE$ to CHR$(F20.INTEGER.FILE.NO%)
\***
\***   IF the returned error code is OM (out of memory) THEN
\***      CALL ADXERROR to log the error
\***   ELSE
\***      IF the returned error code is EF (delete failure) THEN
\***         set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***         CALL APPLICATION.LOG to log error number 508
\***         RESUME processing to the label FUNCTION.EXIT
\***      ELSE
\***         IF the returned error code is WT (write failure) THEN
\***            set VAR.STRING.2$ to F20.STRING.FILE.NO$
\***            CALL APPLICATION.LOG to log error number 552
\***            RESUME processing to the label FUNCTION.EXIT
\***         ELSE
\***            IF the returned error code is CM, CT (chain failure) THEN
\***               set VAR.STRING.1$ to "BF19 " + (3rd byte of MODULE.NUMBER$) +
\***                                           "50  "
\***               set VAR.STRING.2$ to "PS" + (3rd byte MODULE.NUMBER$) + "50"
\***               CALL APPLICATION.LOG to log message number 553
\***            ELSE
\***               log an event 101, message 550 via the standard error
\***               detected function
\***            endif
\***         endif
\***      endif
\***   endif
\***
\*** PROGRAM.EXIT:
\***
\***   IF program is not screen program THEN
\***      STOP
\***   ENDIF
\***
\***   %INCLUDE PSBCHNE.J86
\***
\*** END FUNCTION
\***
\-------------------------------------------------------------------------------

   UPDT.UPDT.ERROR.DETECTED:

     !UPDT.IRF.UPDT = 1                                                ! DAW ! ! LCSk
      UPDT.IRF.TIF.UPDT = 1                                            ! ! LCSk
      FUNCTION.FLAG$   = "R"                                           ! BBCW
      PASSED.INTEGER%  = ERRF%                                         ! BBCW
      PASSED.STRING$   = ""                                            ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      ERRFILE$ = CHR$(F20.INTEGER.FILE.NO%)                            ! BBCW

\ 6 lines deleted from here                                            \ BBCW

      IF ERR <> "CM" AND ERR <> "CT" THEN                              \
         EVENT.NO% = 1                                                :\
         INTEGER4% = ERRN                                             :\
         F17.RETURN.CODE% = CONV.TO.STRING (EVENT.NO%,                 \ DAW
                                            INTEGER4%)                :\
         IF F17.RETURN.CODE% = 0 THEN                                  \
            ERRNUM$      = F17.RETURNED.STRING$                       :\
            STRING.ERRL$ = STR$(ERRL)                                 :\
            WHILE LEN(STRING.ERRL$) < 6                               :\
               STRING.ERRL$ = "0" + STRING.ERRL$                      :\
            WEND

\ 8 lines deleted from here                                            \ BBCW

      IF ERR = "OM" THEN                                      \REM out of memory
         IF F17.RETURN.CODE% = 0 THEN                                  \
            TERM%         = 0                                         :\
            MSGGRP%       = ASC("J")                                  :\
            MSGNUM%       = 0                                         :\
            SEVERITY%     = 3                                         :\
            EVENT.NUM%    = 1                                         :\
            UNIQUE$      =                                             \
                   ERRNUM$ + ERRFILE$ + ERR + PACK$(STRING.ERRL$)     :\
            RET.CODE%    = ADXERROR (TERM%,                            \
                                     MSGGRP%,                          \
                                     MSGNUM%,                          \
                                     SEVERITY%,                        \
                                     EVENT.NUM%,                       \
                                     UNIQUE$)


      IF ERR = "EF" THEN                                      \REM delete failure
         MESSAGE.NUMBER% = 508                                        :\ BBCW
         VAR.STRING.1$ =                                               \
                ERRNUM$ + ERRFILE$ + ERR + PACK$(STRING.ERRL$)        :\
         VAR.STRING.2$ = F20.STRING.FILE.NO$                          :\ BBCW
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ DAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)                      :\
         RESUME


      IF ERR = "WT" THEN                                      \REM write failure
         MESSAGE.NUMBER% = 552                                        :\
         VAR.STRING.1$ =                                               \
                ERRNUM$ + ERRFILE$ + ERR + PACK$(STRING.ERRL$)        :\
         VAR.STRING.2$ = F20.STRING.FILE.NO$                          :\ BBCW
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ DAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)                      :\
         RESUME FUNCTION.EXIT

      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF19 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ BBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ BBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ DAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)

      IF ERR <> "OM" AND                                               \
         ERR <> "CM" AND ERR <> "CT" THEN                              \
     \ lines deleted from here                                     \ DAW
         RET.CODE% = STANDARD.ERROR.DETECTED (ERRN,                    \ DAW
                                              ERRF%,                   \ DAW                                       \
                                              ERRL,                    \ DAW
                                              ERR)                     ! DAW


   PROGRAM.EXIT:                                                       ! BBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN                                \
         STOP

      %INCLUDE PSBCHNE.J86

   END FUNCTION


\*******************************************************************************
\*********                 Function CLOSE.IRF.UPDT                    **********
\*******************************************************************************
\***
\*** FUNCTION CLOSE.IRF.UPDT (new IRF data,
\***                          add/change/delete flag)  PUBLIC
\***
\-------------------------------------------------------------------------------

   FUNCTION CLOSE.IRF.UPDT (NEW.IRF.DATA$,                             \
                            ACD.FLAG$)                                 \
   PUBLIC

      STRING    ACD.FLAG$,                                             \
\ 1 line deleted from here                                             \ DAW
                ERRFILE$,                                              \
                ERRNUM$,                                               \
                FILE$,                                                 \
                FUNCTION.FLAG$,                                        \ BBCW
                IRF.LOCKED.FLAG$,                                      \ BBCW
\ 1 line deleted from here                                             \ DAW
                NEW.IRF.DATA$,                                         \
\ 1 line deleted from here                                             \ DAW
                PASSED.STRING$,                                        \ BBCW
                STRING.ERRL$,                                          \
                UNIQUE$,                                               \
                VAR.STRING.1$,                                         \
                VAR.STRING.2$

      INTEGER*1 EVENT.NUM%,                                            \
                MSGGRP%,                                               \
                PASSED.INTEGER%,                                       \ BBCW
                SEVERITY%

      INTEGER   CLOSE.IRF.UPDT,                                        \ DAW
                EVENT.NO%,                                             \
                MESSAGE.NUMBER%,                                       \
                MSGNUM%,                                               \
                RET.CODE%,                                             \
                SESSION.NUMBER%,                                       \
                TERM%

      INTEGER*4 INTEGER4%


      CALL TIF.SET                                                     ! FSP
      CALL TMCF.SET                                                    ! FSP
      CALL GAOPT.SET                                                   ! FSP


\*******************************************************************************
\***
\***   ON ERROR goto UPDT.CLOSE.ERROR.DETECTED  (after logging the error, the
\***                                             function is immediately left)
\***
\***
\***   REM set up storage areas for ADXERROR required fields in case of memory
\***   overflow
\***
\***   set chaining module to first module in current application
\***
\-------------------------------------------------------------------------------

      ON ERROR GOTO UPDT.CLOSE.ERROR.DETECTED

      UNIQUE$ = "          "
      ERRNUM$ = "    "
      ERRFILE$ = " "
      STRING.ERRL$ = "      "
      CLOSE.IRF.UPDT = 0                                               ! DAW

      PSBCHN.PRG = "ADX_UPGM:PSB50.286"                                !1.10 SWM


\*******************************************************************************
\***
\***     IF the tif open flag is "Y" THEN
\***        CLOSE the tif session number
\***        set FUNCTION.FLAG$ to "C",
\***            PASSED.INTEGER% to tif session number and
\***            PASSED.STRING$ to ""
\***        use SESS.NUM.UTILITY function to deallocate session number
\***        IF F20.RETURN.CODE% <> 0 THEN
\***           GOTO PROGRAM EXIT
\***        ENDIF
\***     ENDIF
\***
\***     IF the tmcf open flag is "Y" THEN
\***        CLOSE the tmcf session number
\***        set FUNCTION.FLAG$ to "C",
\***            PASSED.INTEGER% to tif session number and
\***            PASSED.STRING$ to ""
\***        use SESS.NUM.UTILITY function to deallocate session number
\***        IF F20.RETURN.CODE% <> 0 THEN
\***           GOTO PROGRAM EXIT
\***        ENDIF
\***     ENDIF
\***
\***     IF the idf open flag is "Y" THEN                             ! 1.11 NWB
\***        CLOSE the idf session number                              ! 1.11 NWB
\***        set FUNCTION.FLAG$ to "C",                                ! 1.11 NWB
\***            PASSED.INTEGER% to idf session number and             ! 1.11 NWB
\***            PASSED.STRING$ to ""                                  ! 1.11 NWB
\***        use SESS.NUM.UTILITY function to deallocate session number! 1.11 NWB
\***        IF F20.RETURN.CODE% <> 0 THEN                             ! 1.11 NWB
\***           GOTO PROGRAM EXIT                                      ! 1.11 NWB
\***        ENDIF                                                     ! 1.11 NWB
\***     ENDIF                                                        ! 1.11 NWB
\***
\***     IF the phf open flag is "Y" THEN                             ! 1.11 NWB
\***        CLOSE the phf session number                              ! 1.11 NWB
\***        set FUNCTION.FLAG$ to "C",                                ! 1.11 NWB
\***            PASSED.INTEGER% to phf session number and             ! 1.11 NWB
\***            PASSED.STRING$ to ""                                  ! 1.11 NWB
\***        use SESS.NUM.UTILITY function to deallocate session number! 1.11 NWB
\***        IF F20.RETURN.CODE% <> 0 THEN                             ! 1.11 NWB
\***           GOTO PROGRAM EXIT                                      ! 1.11 NWB
\***        ENDIF                                                     ! 1.11 NWB
\***     ENDIF                                                        ! 1.11 NWB
\***
\***   FUNCTION.EXIT:
\***
\***     EXIT the function
\***
\***
\-------------------------------------------------------------------------------

\ Code removed from here                                               ! EMW

!     Close the IRFDEX and deallocate the session number                   ! 1.9 RC

      CLOSE F19.IRFDEX.SESS.NUM%                                           ! 1.9 RC

      FUNCTION.FLAG$  EQ "C"                                               ! 1.9 RC
      PASSED.INTEGER% EQ F19.IRFDEX.SESS.NUM%                              ! 1.9 RC
      PASSED.STRING$  EQ ""                                                ! 1.9 RC
      RET.CODE% EQ SESS.NUM.UTILITY (FUNCTION.FLAG$, \                     ! 1.9 RC
                                     PASSED.INTEGER%, \                    ! 1.9 RC
                                     PASSED.STRING$)                       ! 1.9 RC

      IF RET.CODE% NE 0 THEN GOTO PROGRAM.EXIT ! PSBF20 failure            ! 1.9 RC

      IF IDF.OPEN.FLAG$ = "Y" THEN                                    \ 1.11 NWB
         CLOSE F19.IDF.SESS.NUM%                                      ! 1.11 NWB

      FUNCTION.FLAG$  = "C"                                           ! 1.11 NWB
      PASSED.INTEGER% = F19.IDF.SESS.NUM%                             ! 1.11 NWB
      PASSED.STRING$  = ""                                            ! 1.11 NWB
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      IF PHF.OPEN.FLAG$ = "Y" THEN                                    \ 1.11 NWB
         CLOSE PHF.SESS.NUM%                                          ! 1.11 NWB

      FUNCTION.FLAG$  = "C"                                           ! 1.11 NWB
      PASSED.INTEGER% = PHF.SESS.NUM%                                 ! 1.11 NWB
      PASSED.STRING$  = ""                                            ! 1.11 NWB
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      IF SRITL.OPEN.FLAG$ = "Y" THEN                                  \ 1.11 NWB
         CLOSE F19.SRITL.SESS.NUM%                                    ! 1.11 NWB

      FUNCTION.FLAG$  = "C"                                           ! 1.11 NWB
      PASSED.INTEGER% = F19.SRITL.SESS.NUM%                           ! 1.11 NWB
      PASSED.STRING$  = ""                                            ! 1.11 NWB
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                   \ 1.11 NWB
                                    PASSED.INTEGER%,                  \ 1.11 NWB
                                    PASSED.STRING$)                   ! 1.11 NWB

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                        ! 1.11 NWB

      IF TIF.OPEN.FLAG$ = "Y" THEN                                     \
         CLOSE TIF.SESS.NUM%

      FUNCTION.FLAG$  = "C"                                            ! BBCW
      PASSED.INTEGER% = TIF.SESS.NUM%                                  ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

      IF TMCF.OPEN.FLAG$ = "Y" THEN                                    \
         CLOSE TMCF.SESS.NUM%

      FUNCTION.FLAG$  = "C"                                            ! BBCW
      PASSED.INTEGER% = TMCF.SESS.NUM%                                 ! BBCW
      PASSED.STRING$  = ""                                             ! BBCW
      RET.CODE% = SESS.NUM.UTILITY (FUNCTION.FLAG$,                    \ DAW
                                    PASSED.INTEGER%,                   \ BBCW
                                    PASSED.STRING$)                    ! BBCW

      IF RET.CODE% <> 0 THEN GOTO PROGRAM.EXIT                         ! DAW

   FUNCTION.EXIT:

      EXIT FUNCTION


\*******************************************************************************
\***
\*** UPDT.CLOSE.ERROR.DETECTED:
\***
\***   Set the function 19 return code to 1.
\***
\***   Log an event 18, message number 553 for a chaining error.  Other errors
\***   are logged as event 101s by calling the standard error detected function.
\***
\***   If the calling program is a batch program then processing stops.  If it
\***   is a screen program then the first program in the current application is
\***   chained back to.
\***
\-------------------------------------------------------------------------------

   UPDT.CLOSE.ERROR.DETECTED:

      CLOSE.IRF.UPDT = 1                                               ! DAW

      IF ERR = "CM" OR ERR = "CT" THEN                        \REM chain failure
         MESSAGE.NUMBER% = 553                                        :\
         VAR.STRING.1$  = "BF19 " + MID$(MODULE.NUMBER$,3,1) + "50  " :\ BBCW
         VAR.STRING.2$  = "PS" + MID$(MODULE.NUMBER$,3,1) + "50"      :\ BBCW
         EVENT.NO%      = 18                                          :\
         RET.CODE% = APPLICATION.LOG (MESSAGE.NUMBER%,                 \ DAW
                                      VAR.STRING.1$,                   \
                                      VAR.STRING.2$,                   \
                                      EVENT.NO%)


      IF ERR <> "CM" AND ERR <> "CT" THEN                              \
         \ lines deleted from here                                     \ DAW
         RET.CODE% = STANDARD.ERROR.DETECTED (ERRN,                    \ DAW
                                              ERRF%,                   \ DAW                                       \
                                              ERRL,                    \ DAW
                                              ERR)                     ! DAW


   PROGRAM.EXIT:                                                       ! BBCW

      IF BATCH.SCREEN.FLAG$ <> "S" THEN                                \
         STOP

      %INCLUDE PSBCHNE.J86

   END FUNCTION

