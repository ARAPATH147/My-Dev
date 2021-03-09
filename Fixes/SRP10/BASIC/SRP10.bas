\********************************************************************
\********************************************************************
\***
\***
\***            PROGRAM         :       SRP10
\***            AUTHOR          :       Neil Bennett
\***            DATE WRITTEN    :       June 2006
\***
\***            MODULE          :       SRP10.BAS
\***
\***
\***    VERSION A.          NEIL BENNETT.           02 JUN 2006.
\***    Initial version.
\***
\***    VERSION B.          Charles Skadorwa.       11 DEC 2008.
\***    No. of records on SRSXF increased from 30,000 to 60,000
\***    due to increase in Intactix data volumes for large stores
\***    which caused keyed file insertion errors.
\***
\***    Version C.          Ranjith Gopalankutty      15 March 2016  
\***    INV10004135 - While housekeeping the planner files, current  
\***    deletion subroutine(POG.DEL) deletes only SRPOG,SRMOD and    
\***    SRPDF and leaving behind SRSXF. File builds up over time     
\***    and fails POGOK suite. POG.DEL will be changed to include    
\***    SRSXF deletion, also existing deletion logic of SRMOD file   
\***    is outdated which will be changed too. Starting 2015,        
\***    refresh of planner has increased, so need to increase the    
\***    file capacity of SRPOG,SRMOD,SRPDF and SRSXF to double of    
\***    its current size                                             
\***
\***    Version D.          Ranjith Gopalankutty      30 June 2016   
\***    Post 16A Desk started receiving failures whenever a POGIL    
\***    is attempted for planner refresh. Its found that its due to  
\***    change done as part of increasing the file size.the earlier  
\***    commented line was ending with '\' and the new line started  
\***    immediately after that. Which compiler taking as commented   
\***    line too and failing while creating new file. Fixed that.    
\********************************************************************
\********************************************************************

\********************************************************************
\********************************************************************
\***
\***    Module Overview
\***    ---------------
\***
\***    This ADCS initiated program takes the Intactix Space and
\***    Range store data and applies it to the in store POG and
\***    module keyed files.
\***
\********************************************************************
\********************************************************************

\********************************************************************
\***
\***    Function globals
\***
\********************************************************************

%INCLUDE PSBF01G.J86    !APPLICATION LOG
%INCLUDE PSBF20G.J86    !SESSION NUMBER UTILITY
%INCLUDE POGDEDEC.J86
%INCLUDE POGOKDEC.J86
%INCLUDE SRPDFDEC.J86
%INCLUDE SRPOGDEC.J86
%INCLUDE SRMODDEC.J86
%INCLUDE SRSXFDEC.J86

\********************************************************************
\***
\***    SRP10 variables
\***
\********************************************************************

   STRING    GLOBAL CURRENT.CODE$
   INTEGER*2 GLOBAL CURRENT.REPORT.NUM%
   STRING    GLOBAL FILE.OPERATION$
   STRING    GLOBAL SB.ACTION$
   STRING    GLOBAL SB.FILE.NAME$
   INTEGER*2 GLOBAL SB.INTEGER%
   INTEGER*2 GLOBAL SB.FILE.REP.NUM%
   INTEGER*2 GLOBAL SB.FILE.SESS.NUM%
   STRING    GLOBAL SB.STRING$

   INTEGER*2 ARRAY.COUNT%                                               ! CRG
   INTEGER*1 bg%
   INTEGER*1 CHAIN.COUNT%                                               ! CRG
   STRING    comm.tail$
   INTEGER*1 eof%
   STRING    err.cd$
   INTEGER*2 event.no%
   STRING    file$
   STRING    file.no$
   INTEGER*1 run.suite%
   INTEGER*1 init.fail%
   INTEGER*1 init.load%
   INTEGER*2 it.ptr%
   INTEGER*2 item.cnt%
   INTEGER*1 last.notch%
   INTEGER*1 last.shelf%
   STRING    mess$
   INTEGER*2 message.no%
   INTEGER*1 mod.cnt%
   INTEGER*1 no.file%
   INTEGER*1 no.inp.file%
   INTEGER*1 no.mod.file%
   INTEGER*1 no.pog.file%
   INTEGER*1 no.read%
   STRING    parm$
   STRING    POGDE.DATE$
   INTEGER*2 POGDE.DTR%
   INTEGER*4 POGDE.SER.NO%
   INTEGER*2 rc%
   INTEGER*4 rc4%
   INTEGER*4 rec.cnt%
   STRING    rectyp$
   STRING    rundate$
   INTEGER*2 shelf.cnt%
   INTEGER*1 srmod.chg%
   INTEGER*2 shelf.item.seq%
   INTEGER*2 sit.cnt%
   INTEGER*1 SRMOD.COUNT.LIMIT%                                         ! CRG
   INTEGER*1 srp10.error%
   INTEGER*1 srp10.event%
   INTEGER*1 tlr.read%
   INTEGER*4 tlr.cnt%
   STRING    text$
   STRING    var.string.1$
   STRING    var.string.2$
   STRING    work$

\********************************************************************
\***
\***    External functions
\***
\********************************************************************

%INCLUDE ADXSERVE.J86   !Controller Services
%INCLUDE PSBF01E.J86    !APPLICATION LOG
%INCLUDE PSBF20E.J86    !SESSION NUMBER UTILITY
%INCLUDE PSBF24E.J86    !STANDARD ERROR DETECTED

%INCLUDE BTCMEM.J86
%INCLUDE SRPEXT.J86
%INCLUDE POGDEEXT.J86
%INCLUDE POGOKEXT.J86
%INCLUDE SRPDFEXT.J86
%INCLUDE SRPOGEXT.J86
%INCLUDE SRMODEXT.J86
%INCLUDE SRSXFEXT.J86

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL
      INTEGER*2 ADXSTART
      STRING    NAME$, PARM$, MESS$
   END FUNCTION

\********************************************************************
\***
\***    SRP10 functions
\***
\********************************************************************

\********************************************************************
\********************************************************************
\***
\***    S T A R T  O F  M A I N L I N E  C O D E
\***
\********************************************************************
\********************************************************************

    ON ERROR GOTO ERROR.DETECTED

START.PROGRAM:

    mess$ = "Program started - Initialising ....."
    GOSUB DISPLAY.MSG
    GOSUB INITIALISATION

    IF NOT init.fail% THEN BEGIN
       mess$ = "Processing S&R Change Delta File ..."
       GOSUB DISPLAY.MSG
       GOSUB MAIN.PROCESS
    ENDIF

TIDY.END.PROG:

    IF run.suite% THEN BEGIN
       mess$ = "Updating run File and Tidy up ......"
       GOSUB DISPLAY.MSG
       GOSUB UPDATE.RUN.FILE
       mess$ = "Program Ended - Flag " + POGOK.PE10.RUNFLAG$        \
             + " Code " + STR$(POGOK.PE10.RETCODE%) + " ......"
    ENDIF ELSE BEGIN
       mess$ = "Suite already processed today - Ending."
    ENDIF

    GOSUB DISPLAY.MSG
    GOSUB TERMINATION

FATAL.END.PROG:

    IF err.cd$ <> "" THEN BEGIN

       mess$ = "Program Abended ERR >" + err.cd$ + "<"
       GOSUB DISPLAY.MSG

    ENDIF

STOP

\********************************************************************
\********************************************************************
\***
\***    S T A R T  O F  S U B R O U T I N E S
\***
\********************************************************************
\********************************************************************

\********************************************************************
\***
\***    SUBROUTINE      :       MAIN.PROCESS
\***
\********************************************************************
\***
\***    READ sequentially through the POG Delta file
\***
\***    Process each record type
\***
\***    RETURN
\***
\********************************************************************

MAIN.PROCESS:

   mod.cnt%  = 0
   sit.cnt%  = 0
   eof%      = 0

   WHILE eof% = 0                                                    \
     AND tlr.read% = 0                              ! Set in Init

      rc% = READ.POGDE
      IF rc% <> 0 THEN BEGIN
         eof% = 1
      ENDIF ELSE BEGIN

         rectyp$ = UNPACK$(MID$(POGDE.RCD$,1,1))

         IF mod.cnt% <> 0 THEN BEGIN
            IF LEFT$(rectyp$,1) = "0"                               \
            OR LEFT$(rectyp$,1) = "9" THEN BEGIN
               IF mod.cnt% <> SRPOG.MODULE.COUNT% THEN BEGIN
                  srp10.event% = 7
                  GOSUB LOG.EVENT
                  GOSUB WRITE.POG.FINAL
               ENDIF
               mod.cnt% = 0
            ENDIF
         ENDIF

         IF srmod.chg% > 0 THEN BEGIN
            IF LEFT$(rectyp$,1) <> "2" THEN BEGIN
               srmod.chg% = 0
               GOSUB WRITE.MODULE.FINAL
            ENDIF ELSE IF item.cnt% <> 0                            \
                      AND MOD(item.cnt%,                            \
                              (SRMOD.MAX.ITEMS%)) = 0 THEN BEGIN
               GOSUB WRITE.MODULE
            ENDIF
         ENDIF

         IF rectyp$ = "01" THEN BEGIN              ! POG Add
            rec.cnt% = rec.cnt% +1
            GOSUB POG.ADD
         ENDIF ELSE IF rectyp$ = "02" THEN BEGIN   ! POG Delete
            rec.cnt% = rec.cnt% +1
            GOSUB POG.DEL
         ENDIF ELSE IF rectyp$ = "03" THEN BEGIN   ! POG Change
            rec.cnt% = rec.cnt% +1
            GOSUB POG.ADD
         ENDIF ELSE IF rectyp$ = "10" THEN BEGIN   ! Module Add
            rec.cnt% = rec.cnt% +1
            mod.cnt% = mod.cnt% +1
            GOSUB MOD.ADD
         ENDIF ELSE IF rectyp$ = "21" THEN BEGIN   ! Shelf/Item Add
            rec.cnt% = rec.cnt% +1
            GOSUB ITEM.ADD
         ENDIF ELSE IF rectyp$ = "99" THEN BEGIN   ! Trailer Record
            rec.cnt% = rec.cnt% +1
            tlr.read% = 1
            tlr.cnt% = GETN4(POGDE.RCD$,1)
         ENDIF

      ENDIF

   WEND

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       POG.ADD
\***
\********************************************************************
\***
\***    Set SRPOG values from POGDE record
\***
\***    Write to SRPOG file
\***
\***    RETURN
\***
\********************************************************************

POG.ADD:

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set POGDB from delta file                                       *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRPOG.POGDB%          = GETN4(POGDE.RCD$,  9    )

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* For POG change, read existing record and keep module count      *\
\*    (this will never be changed - delete POG and add POG used)   *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF rectyp$ = "03" THEN BEGIN
      rc% = READ.SRPOG
      IF rc% <> 0 THEN BEGIN
         SRPOG.MODULE.COUNT%   = GETN1(POGDE.RCD$,118    )
      ENDIF
   ENDIF ELSE BEGIN
      SRPOG.MODULE.COUNT%   = GETN1(POGDE.RCD$,118    )
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set remaining variables supplied in Delta file                  *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRPOG.POGID%          = GETN4(POGDE.RCD$,  1    )
   SRPOG.ACT.DATE$       =  MID$(POGDE.RCD$,  6,  4)
   SRPOG.PLANNER.FAMILY$ =  MID$(POGDE.RCD$, 64, 30)
      ! (Truncated from 50 bytes)
   SRPOG.DEACT.DATE$     =  MID$(POGDE.RCD$,114,  4)
   SRPOG.CAT1.ID%        = GETN4(POGDE.RCD$,119    )
   SRPOG.CAT2.ID%        = GETN4(POGDE.RCD$,123    )
   SRPOG.CAT3.ID%        = GETN4(POGDE.RCD$,127    )

   SRPDF.POGDB%          = SRPOG.POGDB%
   SRPDF.SHRT.DESC$      =  MID$(POGDE.RCD$, 14, 50)
   SRPDF.FULL.DESC$      =  MID$(POGDE.RCD$,132,100)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Extract SRPOG.DESCRIPTION$ (30 bytes) from short decriptor      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRPOG.DESCRIPTION$    = LEFT$(SRPDF.SHRT.DESC$, 30)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Build SRPOG.CAT.DBKEY from category keys in Delta file          *\
\* Set key level                                                   *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF SRPOG.CAT3.ID% <> 0 THEN BEGIN
      SRPOG.KEY.LEVEL% = 3
   ENDIF ELSE IF SRPOG.CAT2.ID% <> 0 THEN BEGIN
      SRPOG.KEY.LEVEL% = 2
   ENDIF ELSE BEGIN
      SRPOG.KEY.LEVEL% = 1
   ENDIF

   SRPOG.CAT.DBKEY% = GET.CATID%(SRPOG.CAT1.ID%,                    \
                                 SRPOG.CAT2.ID%,                    \
                                 SRPOG.CAT3.ID%)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set repeat count values to default unset                        *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRPOG.LIVE.RPT.CNT% = -1
   SRPOG.DATE.RPT.CNT$ = PACK$("00000000")
   SRPOG.PEND.RPT.CNT% = -1

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set filler                                                      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRPOG.FILLER$ = " "
   SRPDF.FILLER$ = STRING$(15," ")

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Write SRPOG record to file                                      *\
\* Write SRPDF record to file                                      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   rc% = WRITE.SRPOG
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF
   rc% = WRITE.SRPDF
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       POG.DEL
\***
\********************************************************************
\***
\***    Build SRPOG key from POGDE record
\***
\***    Read record from SRPOG file
\***
\***    Delete record from SRPOG file (Ignore not found errors)
\***
\***    Delete child records from SRMOD using key + module count
\***
\***    Delete notch details from SRSXF for the modules                 ! CRG
\***
\***    RETURN
\***
\********************************************************************

POG.DEL:

   SRPOG.POGID%        = GETN4(POGDE.RCD$, 1)                           ! CRG
   SRPOG.POGDB%        = GETN4(POGDE.RCD$, 5)                           ! CRG
   SRPOG.ACT.DATE$     = MID$(POGDE.RCD$,10, 4)                         ! CRG
   SRPDF.POGDB%        = SRPOG.POGDB%                                   ! CRG
   SRSXF.POGDB%        = SRPOG.POGDB%                                   ! CRG
   SRMOD.POGDB%        = SRPOG.POGDB%                                   ! CRG
   SRMOD.MODULE.SEQ%   = 0                                              ! CRG
   SRMOD.RECORD.CHAIN% = 0                                              ! CRG
   ARRAY.COUNT%        = 0                                              ! CRG
   CHAIN.COUNT%        = 0                                              ! CRG
   
  ! Module count will be changing from planner to planner but           ! CRG
  ! Iteration has to happen 127 times. In the past, it is noticed that  ! CRG
  ! Module sequence number are not always incremented in order, it      ! CRG
  ! will be random (1,4,7 etc.) So below count will make sure that      ! CRG
  ! deletion covers the entire module range.                            ! CRG
  
   SRMOD.COUNT.LIMIT%  = 127                                            ! CRG
   
   RC% = DELREC.SRPOG                                                   ! CRG
   RC% = DELREC.SRPDF                                                   ! CRG

  ! Delete all the Module records and its chain records for this POG    ! CRG
  ! from SRMOD file and delete the corresponding shelf records from     ! CRG
  ! SRSXF file                                                          ! CRG

   WHILE SRMOD.MODULE.SEQ% >= 0 AND                      \              ! CRG
         SRMOD.MODULE.SEQ% <= SRMOD.COUNT.LIMIT%                        ! CRG

      !Read SRMOD File                                                  ! CRG
      RC% = READ.SRMOD                                                  ! CRG

      IF RC% = 0 THEN BEGIN                                             ! CRG

         !If module record is read successfully then, get its chain     ! CRG
         !records count                                                 ! CRG

         IF SRMOD.ITEM.COUNT% > SRMOD.MAX.ITEMS% THEN BEGIN             ! CRG
            CHAIN.COUNT% = SRMOD.ITEM.COUNT% / SRMOD.MAX.ITEMS%         ! CRG
         ENDIF                                                          ! CRG

         !Assign the current SRMOD module sequence to SRSXF module      ! CRG
         !sequence                                                      ! CRG

         SRSXF.MODULE.SEQ% = SRMOD.MODULE.SEQ%                          ! CRG

         WHILE SRMOD.RECORD.CHAIN% <= CHAIN.COUNT%                      ! CRG

            !Get the shelf numbers from SRSXF file for all module       ! CRG
            !records and its chain records and delete them from         ! CRG
            !SRSXF file                                                 ! CRG

            FOR ARRAY.COUNT% = 0 TO SRMOD.MAX.ITEMS% - 1                ! CRG
               SRSXF.SHELF.NO% = SRMOD.SHELF.NUM%(ARRAY.COUNT%)         ! CRG
               !Ignore delete errors                                    ! CRG
               RC% = DELREC.SRSXF                                       ! CRG
            NEXT ARRAY.COUNT%                                           ! CRG

    ! Delete errors will be ignored as iteration happens for 127 times  ! CRG
    ! even if the module sequence numbers are less. This is done to     ! CRG
    ! avoid dumping of the event log with error message                 ! CRG

            RC% = DELREC.SRMOD                                          ! CRG

            !Increment the chain number to read the next chain record   ! CRG
            !or to exit from the while loop                             ! CRG
            SRMOD.RECORD.CHAIN% = SRMOD.RECORD.CHAIN% + 1               ! CRG

            !Read and delete the chain records if any                   ! CRG
            RC% = 1                                                     ! CRG

            WHILE (RC% = 1) AND (SRMOD.RECORD.CHAIN% <= CHAIN.COUNT%)   ! CRG
               RC% = READ.SRMOD                                         ! CRG
               IF RC% THEN BEGIN                                        ! CRG
                  SRMOD.RECORD.CHAIN% = SRMOD.RECORD.CHAIN% + 1         ! CRG
               ENDIF                                                    ! CRG
            WEND                                                        ! CRG

         WEND                                                           ! CRG

      ENDIF                                                             ! CRG

      !Increment SRMOD module sequence to check for the next available  ! CRG
      !module sequence in the SRMOD file                                ! CRG
      SRMOD.MODULE.SEQ% = SRMOD.MODULE.SEQ% + 1                         ! CRG

      !Reset the chain numbers and chain count for next module record   ! CRG
      SRMOD.RECORD.CHAIN% = 0                                           ! CRG
      CHAIN.COUNT% = 0                                                  ! CRG
   WEND                                                                 ! CRG

   ! Below original subroutine will be commented out as current logic   ! CRG
   ! does not delete records from SRSXF file. Deletion engine is        ! CRG
   ! outdated ,it leaves behind many records untouched, and over time   ! CRG
   ! it piles up in the file and POGOK suite fails                      ! CRG


    

   ! rc% = READ.SRPOG                                                   ! CRG
   ! IF rc% = 0 THEN BEGIN                                              ! CRG
   !   rc% = DELREC.SRPOG                                               ! CRG
   !   SRMOD.POGDB% = SRPOG.POGDB%                                      ! CRG
   !   SRMOD.MODULE.SEQ% = 0                                            ! CRG
   !   SRMOD.RECORD.CHAIN% = 0                                          ! CRG
   !   WHILE SRMOD.MODULE.SEQ% <= SRPOG.MODULE.COUNT%                   ! CRG
   !      rc% = READ.SRMOD                                              ! CRG
   !      rc% = DELREC.SRMOD                                            ! CRG
   !      WHILE SRMOD.ITEM.COUNT% > 50                                  ! CRG
   !         SRMOD.RECORD.CHAIN% = SRMOD.RECORD.CHAIN% +1               ! CRG
   !         SRMOD.ITEM.COUNT% = SRMOD.ITEM.COUNT% -50                  ! CRG
   !         rc% = DELREC.SRMOD                                         ! CRG
   !      WEND                                                          ! CRG
   !      SRMOD.MODULE.SEQ% = SRMOD.MODULE.SEQ% +1                      ! CRG
   !      SRMOD.RECORD.CHAIN% = 0                                       ! CRG
   !   WEND                                                             ! CRG
   ! ENDIF                                                              ! CRG
   ! rc% = READ.SRPDF                                                   ! CRG
   ! IF rc% = 0 THEN BEGIN                                              ! CRG
   !    rc% = DELREC.SRPDF                                              ! CRG
   ! ENDIF                                                              ! CRG

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       MOD.ADD
\***
\********************************************************************
\***
\***    Set SRMOD values from POGDE record
\***
\***    Write to SRMOD file
\***
\***    RETURN
\***
\********************************************************************

MOD.ADD:

   srmod.chg%  =  1
   item.cnt%   =  0
   shelf.cnt%  =  0
   last.shelf% = -1

   SRMOD.POGDB%           = GETN4(POGDE.RCD$, 5)
   SRMOD.MODULE.SEQ%      = GETN1(POGDE.RCD$,13)
   SRMOD.RECORD.CHAIN%    = 0
   SRMOD.DESCRIPTOR$      = MID$ (POGDE.RCD$,15,30)
   FOR it.ptr% = 0 TO (SRMOD.MAX.ITEMS% -1)
      SRMOD.SHELF.NUM%(it.ptr%) = 0
      SRMOD.FACINGS%(it.ptr%)   = 0
      SRMOD.ITEM.CODE$(it.ptr%) = STRING$(3,CHR$(0))
      SRMOD.MDQ%(it.ptr%)       = 0
      SRMOD.PSC%(it.ptr%)       = 0
   NEXT it.ptr%
   it.ptr% = 0
   SRMOD.SHELF.COUNT%     = GETN2(POGDE.RCD$,44)
   SRMOD.ITEM.COUNT%      = 0
   SRMOD.FILLER$          = STRING$(18," ")

   IF SRPOG.POGID% <> GETN4(POGDE.RCD$,1) THEN BEGIN
      srp10.event% = 6
      GOSUB LOG.EVENT
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       ITEM.ADD
\***
\********************************************************************
\***
\***    Set SRMOD values from POGDE record
\***
\***    Write to SRMOD file
\***
\***    RETURN
\***
\********************************************************************

ITEM.ADD:

   IF last.shelf% <> GETN1(POGDE.RCD$,12)                           \
   OR last.notch% <> GETN1(POGDE.RCD$, 1) THEN BEGIN
      shelf.cnt%        = shelf.cnt% +1
      last.shelf%       = GETN1(POGDE.RCD$,12)
      last.notch%       = GETN1(POGDE.RCD$, 1)
      SRSXF.POGDB%      = SRMOD.POGDB%
      SRSXF.MODULE.SEQ% = SRMOD.MODULE.SEQ%
      SRSXF.SHELF.NO%   = GETN1(POGDE.RCD$,12)
      SRSXF.NOTCH.NO%   = GETN1(POGDE.RCD$, 1)
      SRSXF.SHELF.KEY%  = GETN4(POGDE.RCD$,13)
      SRSXF.SHELF.DESC$ = STRING$(50,CHR$(0))
      SRSXF.FILLER$     = "  "
      rc% = WRITE.SRSXF
      IF rc% <> 0 THEN BEGIN
         GOSUB FILE.ERROR
      ENDIF
   ENDIF

   SRMOD.FACINGS%(it.ptr%)   = GETN1(POGDE.RCD$, 2)
   SRMOD.ITEM.CODE$(it.ptr%) = MID$ (POGDE.RCD$, 6, 3)
   SRMOD.MDQ%(it.ptr%)       = GETN2(POGDE.RCD$, 8)
   SRMOD.PSC%(it.ptr%)       = GETN2(POGDE.RCD$,10)
   SRMOD.SHELF.NUM%(it.ptr%) = GETN1(POGDE.RCD$,12)
   it.ptr%  = it.ptr% +1

\  shelf.item.seq%   = GETN2(POGDE.RCD$, 3)      ! not used ?
   item.cnt%         = item.cnt% +1
   SRMOD.ITEM.COUNT% = item.cnt%

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       WRITE.MODULE
\***
\********************************************************************
\***
\***    Write to SRMOD file
\***
\***    Increment CHAIN sequence ready for possible next write
\***
\***    RETURN
\***
\********************************************************************

WRITE.MODULE:

   rc% = WRITE.SRMOD
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF

   FOR it.ptr% = 0 TO (SRMOD.MAX.ITEMS% -1)
      SRMOD.SHELF.NUM%(it.ptr%) = 0
      SRMOD.FACINGS%(it.ptr%)   = 0
      SRMOD.ITEM.CODE$(it.ptr%) = STRING$(3,CHR$(0))
      SRMOD.MDQ%(it.ptr%)       = 0
      SRMOD.PSC%(it.ptr%)       = 0
   NEXT it.ptr%
   it.ptr% = 0

   SRMOD.RECORD.CHAIN%    = SRMOD.RECORD.CHAIN% +1

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       WRITE.MODULE.FINAL
\***
\********************************************************************
\***
\***    Write to SRMOD file
\***
\***    Update previous records in chain with item count
\***
\***    RETURN
\***
\********************************************************************

WRITE.MODULE.FINAL:

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Shelf count not set by host insert computed count               *\
\* This is the count of shelves and/or notch changes within a shelf*\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRMOD.SHELF.COUNT% = shelf.cnt%

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Write any outstanding items to file                             *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF it.ptr% > 0 THEN BEGIN
      GOSUB WRITE.MODULE
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Decrement the chain pointer by 2 to point to last but 1 write   *\
\*    as the last write would have had the correct item count      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   SRMOD.RECORD.CHAIN% = SRMOD.RECORD.CHAIN% -2

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* For each record back to the first, update the item count field  *\
\* and the shelf count field.                                      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   WHILE SRMOD.RECORD.CHAIN% >= 0
      rc% = READ.SRMOD
      IF rc% <> 0 THEN BEGIN
         GOSUB FILE.ERROR
      ENDIF
      SRMOD.ITEM.COUNT% = item.cnt%
      SRMOD.SHELF.COUNT% = shelf.cnt%
      rc% = WRITE.SRMOD
      IF rc% <> 0 THEN BEGIN
         GOSUB FILE.ERROR
      ENDIF
      SRMOD.RECORD.CHAIN% = SRMOD.RECORD.CHAIN% -1
   WEND

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       WRITE.POG.FINAL
\***
\********************************************************************
\***
\***    Write to SRPOG file updating module count
\***
\***    RETURN
\***
\********************************************************************

WRITE.POG.FINAL:

   rc% = READ.SRPOG
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF
   SRPOG.MODULE.COUNT% = mod.cnt%
   rc% = WRITE.SRPOG
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       INITIALISATION
\***
\********************************************************************
\***
\***    Initialise main program variables
\***
\***    Allocate session numbers to files
\***
\***    OPEN required files
\***
\***    SET program started flag
\***
\***    Check run is valid
\***
\***    RETURN
\***
\********************************************************************

INITIALISATION:

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set program variables                                           *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   bg%           = 0
   init.fail%    = 0
   rec.cnt%      = 0
   srmod.chg%    = 0
   srp10.error%  = 0
   srp10.event%  = 0
   tlr.cnt%      = 0
   tlr.read%     = 0
   comm.tail$    = COMMAND$

   IF LEFT$(comm.tail$, 8) = "BACKGRND" THEN BEGIN
      bg% = 1
      comm.tail$ = MID$(comm.tail$,10,LEN(comm.tail$) -9)
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set todays date YYYYMMDD                                        *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   rundate$ = PACK$("20"+DATE$)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Allocate Session Numbers and Open Files                         *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   GOSUB ALLOCATE.SESS.NUMS

   GOSUB OPEN.FILES

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set POGOK run flag                                              *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   no.read% = 0
retry1:
   rc% = READ.POGOK.LOCK
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
      IF no.read% > 0 THEN GOTO retry1
   ENDIF

   run.suite% = 2
   IF LEFT$(comm.tail$, 7) = "SLEEPER" THEN BEGIN
      IF POGOK.PE10.RUNDATE$ = rundate$                             \
     AND POGOK.PE10.RUNFLAG$ <> "X" THEN BEGIN
         run.suite% = run.suite% -1
      ENDIF ELSE BEGIN
         POGOK.PE10.RUNFLAG$ = "S"
      ENDIF
      IF POGOK.PE5.RUNDATE$ = rundate$                              \
     AND POGOK.PE5.RUNFLAG$ <> "X" THEN BEGIN
         run.suite% = run.suite% -1
      ENDIF ELSE BEGIN
         GOSUB START.SRP5
      ENDIF
   ENDIF ELSE BEGIN
      POGOK.PE10.RUNFLAG$ = "S"
      GOSUB START.SRP5
   ENDIF

   rc% = WRITE.POGOK.UNLOCK
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF

   IF NOT run.suite% THEN BEGIN
      init.fail% = -1
      RETURN
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Read input file header record                                   *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF NOT no.inp.file% THEN BEGIN
      rc% = READ.POGDE
      IF rc% <> 0 THEN BEGIN
         GOSUB FILE.ERROR
      ENDIF
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Check for header record                                         *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF (NOT no.inp.file%)                                            \
  AND (UNPACK$(MID$(POGDE.RCD$,1,1)) = "00") THEN BEGIN
      rec.cnt% = rec.cnt% +1
      POGDE.SER.NO% = GETN4(POGDE.RCD$,1)
      POGDE.DATE$   = MID$(POGDE.RCD$,6,4)
      POGDE.DTR%    = GETN2(POGDE.RCD$,9)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* NOT rerun AND serial number and date are equal - Duplicate      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

      IF LEFT$(comm.tail$, 5) <> "RERUN"                            \
     AND LEFT$(comm.tail$, 7) <> "SLEEPER" THEN BEGIN               !
         IF VAL(POGOK.SRD.SER.NO$) = POGDE.SER.NO%                  \
        AND POGOK.SRD.DATE$ = POGDE.DATE$ THEN BEGIN

            ! Log event for duplicate
            srp10.event% = 2
            GOSUB LOG.EVENT

            srp10.error% = srp10.error% OR 08H

            init.fail% = 1

            RETURN

         ENDIF
      ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* serial number or date < reset file - Old data                   *\
\*    (allow for serial number rollover @ 9999)                    *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

      rc% = VAL(POGOK.SRD.SER.NO$)
      IF rc% = 9999 THEN rc% = 0

      IF rc% > GETN4(POGDE.RCD$,1)                                  \
      OR POGOK.SRD.DATE$ > MID$(POGDE.RCD$,6,4) THEN BEGIN

         srp10.event% = 3
         GOSUB LOG.EVENT

         srp10.error% = srp10.error% OR 04H

         init.fail% = 1

         RETURN

      ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Initial load                                                    *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

      IF MID$(POGDE.RCD$,12,1) = "I" THEN BEGIN

         init.load% = 1

         IF NOT no.pog.file% THEN BEGIN
            CLOSE SRPOG.SESS.NUM%
            IF END #SRPOG.SESS.NUM% THEN NO.SRPOG.COPY
            OPEN SRPOG.COPY.NAME$ AS SRPOG.SESS.NUM%
            DELETE SRPOG.SESS.NUM%
NO.SRPOG.COPY:
            rc% = RENAME(SRPOG.COPY.NAME$,SRPOG.FILE.NAME$)
         ENDIF
         IF NOT no.mod.file% THEN BEGIN
            CLOSE SRMOD.SESS.NUM%
            IF END #SRMOD.SESS.NUM% THEN NO.SRMOD.COPY
            OPEN SRMOD.COPY.NAME$ AS SRMOD.SESS.NUM%
            DELETE SRMOD.SESS.NUM%
NO.SRMOD.COPY:
            rc% = RENAME(SRMOD.COPY.NAME$,SRMOD.FILE.NAME$)
         ENDIF

         GOSUB CREATE.SRPOG
         GOSUB CREATE.SRMOD


      ENDIF ELSE IF MID$(POGDE.RCD$,12,1) = "D" THEN BEGIN

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* delta file and no existing                                      *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

         IF no.pog.file% OR no.mod.file% THEN BEGIN

            srp10.event% = 5
            GOSUB LOG.EVENT

            srp10.error% = srp10.error% OR 10H

            IF NOT no.pog.file% THEN BEGIN
               DELETE SRPOG.SESS.NUM%
            ENDIF
            IF NOT no.mod.file% THEN BEGIN
               DELETE SRMOD.SESS.NUM%
            ENDIF

            GOSUB CREATE.SRPOG
            GOSUB CREATE.SRMOD

         ENDIF

      ENDIF

   ENDIF ELSE BEGIN

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* No header record or No input file                               *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

      srp10.event% = 1
      GOSUB LOG.EVENT

      srp10.error% = srp10.error% OR 20H
      init.fail% = 1

      IF no.pog.file% OR no.mod.file% THEN BEGIN

         IF NOT no.pog.file% THEN BEGIN
            DELETE SRPOG.SESS.NUM%
         ENDIF
         IF NOT no.mod.file% THEN BEGIN
            DELETE SRMOD.SESS.NUM%
         ENDIF

         GOSUB CREATE.SRPOG
         GOSUB CREATE.SRMOD

      ENDIF

   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       ALLOCATE.SESS.NUMS
\***
\********************************************************************
\***
\***    Allocate all session numbers
\***
\********************************************************************

ALLOCATE.SESS.NUMS:

    SB.ACTION$ = "O"

    SB.INTEGER% = POGDE.REPORT.NUM%
    SB.STRING$  = POGDE.FILE.NAME$
    GOSUB SB.FILE.UTILS
    POGDE.SESS.NUM% = SB.FILE.SESS.NUM%

    SB.INTEGER% = POGOK.REPORT.NUM%
    SB.STRING$  = POGOK.FILE.NAME$
    GOSUB SB.FILE.UTILS
    POGOK.SESS.NUM% = SB.FILE.SESS.NUM%

    SB.INTEGER% = SRPOG.REPORT.NUM%
    SB.STRING$  = SRPOG.FILE.NAME$
    GOSUB SB.FILE.UTILS
    SRPOG.SESS.NUM% = SB.FILE.SESS.NUM%

    SB.INTEGER% = SRMOD.REPORT.NUM%
    SB.STRING$  = SRMOD.FILE.NAME$
    GOSUB SB.FILE.UTILS
    SRMOD.SESS.NUM% = SB.FILE.SESS.NUM%

    SB.INTEGER% = SRPDF.REPORT.NUM%
    SB.STRING$  = SRPDF.FILE.NAME$
    GOSUB SB.FILE.UTILS
    SRPDF.SESS.NUM% = SB.FILE.SESS.NUM%

    SB.INTEGER% = SRSXF.REPORT.NUM%
    SB.STRING$  = SRSXF.FILE.NAME$
    GOSUB SB.FILE.UTILS
    SRSXF.SESS.NUM% = SB.FILE.SESS.NUM%

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       OPEN.FILES
\***
\********************************************************************
\***
\***    Open required files
\***
\********************************************************************

OPEN.FILES:

   CALL POGDE.SET
   CALL POGOK.SET
   CALL SRPDF.SET
   CALL SRPOG.SET
   CALL SRMOD.SET
   CALL SRSXF.SET

   FILE.OPERATION$ = "O"

   no.inp.file% = 1
   CURRENT.REPORT.NUM% = POGDE.REPORT.NUM%
   IF END # POGDE.SESS.NUM% THEN pogde.open.err
   rc4% = SIZE(POGDE.FILE.NAME$)
   rc%  = MOD(rc4%, 1024)
   IF rc% <> 0 THEN BEGIN
      mess$ = STRING$(1024 -rc%, " ")
      work$ = "C" + STR$(LEN(mess$))
      OPEN POGDE.FILE.NAME$ AS POGDE.SESS.NUM% APPEND
      WRITE FORM work$; #POGDE.SESS.NUM%; mess$
      CLOSE POGDE.SESS.NUM%
   ENDIF

   OPEN POGDE.FILE.NAME$ DIRECT RECL 1024 AS POGDE.SESS.NUM%        \
        NOWRITE NODEL
   no.inp.file% = 0
pogde.open.err:

   no.file% = 1
   CURRENT.REPORT.NUM% = POGOK.REPORT.NUM%
   IF END # POGOK.SESS.NUM% THEN pogok.open.err
   OPEN POGOK.FILE.NAME$ DIRECT RECL POGOK.RECL% AS POGOK.SESS.NUM% \
         NODEL
   no.file% = 0
pogok.open.err:
   IF no.file% = 1 THEN BEGIN
      rc% = CREATE.POGOK
      IF rc% <> 0 THEN BEGIN
         GOSUB CREATE.ERROR
      ENDIF
   ENDIF

   no.pog.file% = 1
   CURRENT.REPORT.NUM% = SRPOG.REPORT.NUM%
   IF END # SRPOG.SESS.NUM% THEN srpog.open.err
   OPEN SRPOG.FILE.NAME$ KEYED RECL SRPOG.RECL% AS SRPOG.SESS.NUM%
   no.pog.file% = 0
srpog.open.err:

   no.mod.file% = 1
   CURRENT.REPORT.NUM% = SRMOD.REPORT.NUM%
   IF END # SRMOD.SESS.NUM% THEN srmod.open.err
   OPEN SRMOD.FILE.NAME$ KEYED RECL SRMOD.RECL% AS SRMOD.SESS.NUM%
   no.mod.file% = 0
srmod.open.err:

   no.file% = 1
   CURRENT.REPORT.NUM% = SRPDF.REPORT.NUM%
   IF END # SRPDF.SESS.NUM% THEN srpdf.open.err
   OPEN SRPDF.FILE.NAME$ KEYED RECL SRPDF.RECL% AS SRPDF.SESS.NUM%
   no.file% = 0
srpdf.open.err:
   IF no.file% = 1 THEN BEGIN
      no.file% = 0
      IF NOT no.inp.file% THEN BEGIN
         FILE.OPERATION$ = "C"
   ! Planner refresh from Inctactix has been increased over time so     ! CRG 
   ! doubling the file capacity to double of its current size           ! CRG
   
   !      CREATE POSFILE SRPDF.FILE.NAME$ KEYED 4,,,3000                ! DRG

          CREATE POSFILE SRPDF.FILE.NAME$ KEYED 4,,,6000                \ CRG
                RECL SRPDF.RECL% AS SRPDF.SESS.NUM% MIRRORED ATCLOSE
      ENDIF
   ENDIF

   no.file% = 1
   CURRENT.REPORT.NUM% = SRSXF.REPORT.NUM%
   IF END # SRSXF.SESS.NUM% THEN srsxf.open.err
   OPEN SRSXF.FILE.NAME$ KEYED RECL SRSXF.RECL% AS SRSXF.SESS.NUM%
   no.file% = 0
srsxf.open.err:
   IF no.file% = 1 THEN BEGIN
      no.file% = 0
      IF NOT no.inp.file% THEN BEGIN
         FILE.OPERATION$ = "C"

   ! Planner refresh from Inctactix has been increased over time so     ! CRG 
   ! increasing the file capacity to 1.5 times of its current size      ! CRG 

   !      CREATE POSFILE SRSXF.FILE.NAME$ KEYED 6,,,60000               ! DRG

          CREATE POSFILE SRSXF.FILE.NAME$ KEYED 6,,,90000               \ CRG
                RECL SRSXF.RECL% AS SRSXF.SESS.NUM% MIRRORED ATCLOSE
      ENDIF
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       UPDATE.RUN.FILE
\***
\********************************************************************
\***
\***    READ POGOK Locked
\***
\***    Set up variables according to run results
\***
\***    WRITE POGOK Unlock
\***
\***    RETURN
\***
\********************************************************************

UPDATE.RUN.FILE:

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set todays date YYYYMMDD (set again incase 24:00 passed)        *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   rundate$ = PACK$("20"+DATE$)

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Set error codes for incomplete Delta file                       *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF (srp10.error% AND 80H) = 80H                                  \
  AND init.fail% = 0 THEN BEGIN
      IF tlr.read% = 0 THEN BEGIN
         srp10.error% = srp10.error% OR 02H
      ENDIF ELSE IF tlr.cnt% <> rec.cnt% THEN BEGIN
         srp10.error% = srp10.error% OR 01H
      ENDIF
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Read POGOK file - Retry if locked                               *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   no.read% = 0
retry2:
   rc% = READ.POGOK.LOCK
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
      IF no.read% > 0 THEN GOTO retry2
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Update POGOK based on run results                               *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF init.load% THEN BEGIN
      POGOK.RELOAD$ = "N"
   ENDIF

   POGOK.PE10.RUNDATE$ = rundate$
   IF (srp10.error% AND 20H) <> 0 THEN BEGIN
      ! Delta File not found or No header record
      POGOK.PE10.RUNFLAG$ = "Y"
      POGOK.PE10.RETCODE% = 6
   ENDIF ELSE IF (srp10.error% AND 83H) <> 0 THEN BEGIN
      ! Abend via Main error routine (80H)
      ! Incorrect record count (01H) or No trailer record (02H)
      srp10.event% = 4
      GOSUB LOG.EVENT
      POGOK.PE10.RUNFLAG$      = "X"
      IF (srp10.error% AND 80H) = 80H THEN BEGIN
         POGOK.PE10.RETCODE% = 8
      ENDIF ELSE BEGIN
         POGOK.PE10.RETCODE% = (srp10.error% AND 03H)
      ENDIF
      POGOK.SRD.REC.COUNT%     = rec.cnt%
      POGOK.FAILED.SRD.SER.NO$ = RIGHT$("0000"                      \
                                      + STR$(POGDE.SER.NO%),4)
      POGOK.FAILED.SRD.DATE$   = POGDE.DATE$
   ENDIF ELSE IF (srp10.error% AND 04H) <> 0 THEN BEGIN
      ! Old data
      POGOK.PE10.RUNFLAG$ = "Y"
      POGOK.PE10.RETCODE% = 3
   ENDIF ELSE IF (srp10.error% AND 08H) <> 0 THEN BEGIN
      ! Already processed (Not 'RERUN')
      POGOK.PE10.RUNFLAG$ = "E"
      POGOK.PE10.RETCODE% = 4
   ENDIF ELSE IF (srp10.error% AND 10H) <> 0 THEN BEGIN
      ! Init Load expected - Delta received
      POGOK.PE10.RUNFLAG$ = "Y"
      POGOK.PE10.RETCODE% = 5
      POGOK.RELOAD$       = "Y"
   ENDIF ELSE BEGIN
      ! No problems
      POGOK.PE10.RUNFLAG$ = "E"
      POGOK.PE10.RETCODE% = 0
   ENDIF

   IF POGOK.PE10.RETCODE% = 0                                       \
   OR POGOK.PE10.RETCODE% = 5 THEN BEGIN
      POGOK.SRD.SER.NO$     = RIGHT$("0000"+STR$(POGDE.SER.NO%),4)
      POGOK.SRD.DATE$       = POGDE.DATE$
      POGOK.SRD.REC.COUNT%  = rec.cnt%
      POGOK.DAYS.TO.RETAIN% = POGDE.DTR%
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Write to POGOK                                                  *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   rc% = WRITE.POGOK.UNLOCK
   IF rc% <> 0 THEN BEGIN
      GOSUB FILE.ERROR
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Delete copies of SRPOG/SRMOD if they exist (Not if abended)     *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF POGOK.PE10.RUNFLAG$ <> "X" THEN BEGIN
      IF SIZE(SRPOG.COPY.NAME$) > 0 THEN BEGIN
         CLOSE SRPOG.SESS.NUM%
         OPEN SRPOG.COPY.NAME$ AS SRPOG.SESS.NUM%
         DELETE SRPOG.SESS.NUM%
      ENDIF
      IF SIZE(SRMOD.COPY.NAME$) > 0 THEN BEGIN
         CLOSE SRMOD.SESS.NUM%
         OPEN SRMOD.COPY.NAME$ AS SRMOD.SESS.NUM%
         DELETE SRMOD.SESS.NUM%
      ENDIF
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Delete old copy of delta file/ Save current (Not if abended)    *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   IF POGOK.PE10.RUNFLAG$ <> "X" THEN BEGIN
      IF SIZE(POGDE.COPY.NAME$) > 0 THEN BEGIN
         CLOSE POGDE.SESS.NUM%
         OPEN POGDE.COPY.NAME$ AS POGDE.SESS.NUM%
         DELETE POGDE.SESS.NUM%
      ENDIF
      rc% = RENAME(POGDE.COPY.NAME$,POGDE.FILE.NAME$)
   ENDIF

\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\
\* Start - House keeping & Index Prime                             *\
\*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*\

   GOSUB START.SRP6

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       TERMINATION
\***
\********************************************************************
\***
\***    Deallocate all session numbers
\***
\***    CLOSE the required files
\***
\********************************************************************

TERMINATION:

    SB.ACTION$ = "C"
    SB.STRING$ = ""

    SB.INTEGER% = POGDE.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

    SB.INTEGER% = POGOK.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

    SB.INTEGER% = SRPDF.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

    SB.INTEGER% = SRPOG.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

    SB.INTEGER% = SRMOD.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

    SB.INTEGER% = SRSXF.SESS.NUM%
    CLOSE SB.INTEGER%
    GOSUB SB.FILE.UTILS

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       START.SRP5
\***
\********************************************************************


START.SRP5:

   file$ = "ADX_UPGM:SRP05.286"
   parm$ = comm.tail$
   text$ = "S&R MAPPING LOAD - Started by SRP10 -"                  \
         + MID$(TIME$,1,2) + ":"                                    \
         + MID$(TIME$,3,2) + ":"                                    \
         + MID$(TIME$,5,2)                                          !
   rc%   = ADXSTART(file$, parm$, text$)
   IF rc% <> 0 THEN BEGIN
      srp10.event% = 8
      GOSUB LOG.EVENT
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       START.SRP6
\***
\********************************************************************


START.SRP6:

   file$ = "ADX_UPGM:SRP06.286"
   parm$ = comm.tail$
   text$ = "S&R Housekeeping + Index from SRP10 -"                  \
         + MID$(TIME$,1,2) + ":"                                    \
         + MID$(TIME$,3,2) + ":"                                    \
         + MID$(TIME$,5,2)                                          !
   rc%   = ADXSTART(file$, parm$, text$)
   IF rc% <> 0 THEN BEGIN
      srp10.event% = 8
      GOSUB LOG.EVENT
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       CREATE.SRPOG
\***
\********************************************************************

CREATE.SRPOG:

   IF END #SRPOG.SESS.NUM% THEN CREATE.ERROR
   CURRENT.REPORT.NUM% = SRPOG.REPORT.NUM%
   
   ! Planner refresh from Inctactix has been increased over time so     ! CRG 
   ! increasing the file capacity to double of its current size         ! CRG 
   
   ! CREATE POSFILE SRPOG.FILE.NAME$ KEYED 4,,,3000                     ! DRG

     CREATE POSFILE SRPOG.FILE.NAME$ KEYED 4,,,6000                   \ ! CRG
          RECL SRPOG.RECL% AS SRPOG.SESS.NUM% MIRRORED ATCLOSE

   no.pog.file% = 0

   RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       CREATE.SRMOD
\***
\********************************************************************


CREATE.SRMOD:

   IF END #SRMOD.SESS.NUM% THEN CREATE.ERROR
   CURRENT.REPORT.NUM% = SRMOD.REPORT.NUM%
   
   ! Planner refresh from inctactix has been increased over time so     ! CRG 
   ! increasing the file capacity to double of its current size         ! CRG 
   ! CREATE POSFILE SRMOD.FILE.NAME$ KEYED 6,,,4000                     ! DRG

     CREATE POSFILE SRMOD.FILE.NAME$ KEYED 6,,,8000                   \ ! CRG
          RECL SRMOD.RECL% AS SRMOD.SESS.NUM% MIRRORED ATCLOSE

   no.mod.file% = 0

   RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       DISPLAY.MSG
\***
\********************************************************************

DISPLAY.MSG:

   mess$ = LEFT$(mess$ + STRING$(36," "), 36) + " "                 \
         + MID$(TIME$,1,2) + ":"                                    \
         + MID$(TIME$,3,2) + ":"                                    \
         + MID$(TIME$,5,2)                                          !

   IF bg% = 1 THEN BEGIN
      CALL ADXSERVE (rc4%, 26, 0, mess$)
   ENDIF ELSE BEGIN
      PRINT mess$
   ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       LOG.EVENT
\***
\***    srp10.event% = 1 - Header not 1st record in Delta file
\***                   2 - Duplicate Delta File
\***                   3 - Old Delta File
\***                   4 - Premature eof on Delta file
\***                   5 - Delta received - no existing POG/MOD file
\***                   6 - ID of module record <> ID of POG Header
\***                   7 - POG module records < POG module count
\***                   8 - ADXSTART error
\***
\********************************************************************

LOG.EVENT:

   IF srp10.event% <  1                                             \
   OR srp10.event% >  8 THEN BEGIN
      GOTO 0
   ENDIF

   message.no%   = 0

   ON srp10.event% GOSUB 1, 2, 3, 4, 5, 6, 7, 8

0:
   srp10.event% = 0

RETURN

1:
   event.no%     = 17
   var.string.1$ = RIGHT$("000" + STR$(POGDE.REPORT.NUM%),3)        \
                 + "00 " + LEFT$(rectyp$ + "   ",3)
   var.string.2$ = ""
   GOTO LOG.IT

2:
   event.no%     = 177
   var.string.1$ = CHR$(SHIFT(POGDE.REPORT.NUM%,8))                 \
                 + CHR$(SHIFT(POGDE.REPORT.NUM%,0))                 \
                 + "D"
   var.string.2$ = ""
   GOTO LOG.IT

3:
   event.no%     = 177
   var.string.1$ = CHR$(SHIFT(POGDE.REPORT.NUM%,8))                 \
                 + CHR$(SHIFT(POGDE.REPORT.NUM%,0))                 \
                 + "O"
   var.string.2$ = ""
   GOTO LOG.IT

4:
   ! (Serial number not logged)
   event.no%     = 92
   var.string.1$ = CHR$(SHIFT(POGDE.REPORT.NUM%,8))                 \
                 + CHR$(SHIFT(POGDE.REPORT.NUM%,0))                 \
                 + PACK$(RIGHT$("00000000" +STR$(tlr.cnt%),8))      \
                 + PACK$(RIGHT$("00000000" +STR$(rec.cnt%),8))
   var.string.2$ = ""
   GOTO LOG.IT

5:
   event.no%     = 178
   IF no.pog.file% THEN BEGIN
      work$ = "SRPOG"
   ENDIF ELSE BEGIN
      work$ = "     "
   ENDIF
   IF no.mod.file% THEN BEGIN
      work$ = work$                                                 \
            + "SRMOD"
   ENDIF ELSE BEGIN
      work$ = work$                                                 \
            + "     "
   ENDIF
   var.string.1$ = work$
   var.string.2$ = ""
   work$         = ""
   GOTO LOG.IT

6:
   event.no%     = 181
   work$         = STRING$(8,CHR$(0))
   CALL PUTN4(work$,0,VAL(UNPACK$(MID$(POGDE.RCD$,2,3))))
   CALL PUTN4(work$,4,SRPOG.POGID%)
   var.string.1$ = CHR$(SHIFT(POGDE.REPORT.NUM%,8))                 \
                 + CHR$(SHIFT(POGDE.REPORT.NUM%,0))                 \
                 + work$
   var.string.2$ = ""
   GOTO LOG.IT

7:
   event.no%     = 182
   work$     = STRING$(2,CHR$(0))
   CALL PUTN1(work$,0,mod.cnt%)
   CALL PUTN1(work$,1,SRPOG.MODULE.COUNT%)
   var.string.1$ = CHR$(SHIFT(POGDE.REPORT.NUM%,8))                 \
                 + CHR$(SHIFT(POGDE.REPORT.NUM%,0))                 \
                 + work$
   var.string.2$ = ""
   GOTO LOG.IT

8:
   event.no%     =  42
   var.string.1$ = STR$(rc%)
   var.string.2$ = ""

LOG.IT:

   CALL APPLICATION.LOG(message.no%,                                \
                        var.string.1$,                              \
                        var.string.2$,                              \
                        event.no%)

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       SB.FILE.UTILS
\***
\********************************************************************
\***
\***      Allocate/report/de-allocate a file session number
\***
\********************************************************************
\***
\***      Parameters : 2 or 3 (depending on action)
\***
\***         SB.ACTION$  = "O" for allocate file session number
\***                       "R" for report file session number
\***                       "C" for de-allocate file session number
\***
\***         SB.INTEGER% = file reporting number for action "O" or
\***                       file session number for actions "R" or "C"
\***
\***         SB.STRING$  = logical file name for action "O" or
\***                       null ("") for action "R" and "C"
\***
\***      Output : 1 or 2 (depending on action)
\***
\***         SB.FILE.NAME$     = logical file name for action "R"
\***
\***         SB.FILE.SESS.NUM% = file session number for action "O"
\***                             or undefined for action "C"
\***         OR
\***         SB.FILE.REP.NUM%  = file reporting number for action "R"
\***                             or undefined for action "C"
\***
\********************************************************************

SB.FILE.UTILS:

    CALL SESS.NUM.UTILITY(SB.ACTION$, SB.INTEGER%, SB.STRING$)

    IF SB.ACTION$ = "O" THEN BEGIN

        SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%

    ENDIF ELSE IF SB.ACTION$ = "R" THEN BEGIN

        SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%
        SB.FILE.NAME$ = F20.FILE.NAME$

    ENDIF

RETURN

\********************************************************************
\***
\***    SUBROUTINE      :       CREATE.ERROR
\***
\********************************************************************

CREATE.ERROR:

   FILE.OPERATION$ = "C"

   GOSUB FILE.ERROR

   GOTO TIDY.END.PROG

RETURN

\********************************************************************
\********************************************************************
\***
\***    E N D  O F  L O W  L E V E L  S U B R O U T I N E S
\***
\********************************************************************
\********************************************************************


\********************************************************************
\********************************************************************
\***
\***    S T A R T  O F  E R R O R  R O U T I N E S
\***
\********************************************************************
\********************************************************************
\********************************************************************
\***
\***    ERROR ROUTINE   :       FILE.ERROR
\***
\********************************************************************

FILE.ERROR:

    IF SB.ACTION$ = "C" THEN RETURN             ! Ignore close errs

    event.no%   = 106
    message.no% = 0

    file.no$ = CHR$(SHIFT(CURRENT.REPORT.NUM%,8)) +                 \
               CHR$(SHIFT(CURRENT.REPORT.NUM%,0))

    var.string.2$ = RIGHT$("000" + STR$(CURRENT.REPORT.NUM%),3)

    IF FILE.OPERATION$ = "R" THEN BEGIN
        IF no.read% < 60                                             \
       AND CURRENT.REPORT.NUM% = POGOK.REPORT.NUM% THEN BEGIN
           WAIT ; 500
           no.read% = no.read% +1
        ENDIF ELSE BEGIN
           var.string.2$ = var.string.2$                            \
                         + UNPACK$(CURRENT.CODE$)
        ENDIF
    ENDIF

    var.string.1$ = FILE.OPERATION$ +                               \
                    file.no$ +                                      \
                    PACK$(STRING$(12,"0"))

    CALL APPLICATION.LOG(message.no%,                               \
                         var.string.1$,                             \
                         var.string.2$,                             \
                         event.no%)

RETURN

\********************************************************************
\***
\***    ERROR ROUTINE   :       ERROR.DETECTED
\***
\********************************************************************

ERROR.DETECTED:

    IF ERR = "OE" AND ERRF% = 0 THEN RESUME     ! Size errors
    IF ERR = "CU" THEN RESUME

    CALL STANDARD.ERROR.DETECTED(ERRN,                              \
                                 ERRF%,                             \
                                 ERRL,                              \
                                 ERR)

    err.cd$ = ERR + " rec " + str$(rec.cnt%)

    IF (srp10.error% AND 80H) = 0 THEN BEGIN
       srp10.error% = srp10.error% OR 80H
       RESUME TIDY.END.PROG
    ENDIF ELSE BEGIN
       RESUME FATAL.END.PROG
    ENDIF

END
