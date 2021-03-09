\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   RFSCFFUN.BAS  $
\***
\***   $Revision:   1.7  $
\***
\*****************************************************************************
\*****************************************************************************
\***
\***                      RFSCF  FILE FUNCTIONS
\***
\***                      REFERENCE    : RFSCFFUN
\***
\***
\***           VERSION A       Nik Sen         5th October 1998
\***
\*******************************************************************************
\*******************************************************************************
\***           VERSION B       Mark Goode       23rd August 2004
\***
\***   Updated to reflect current RF version, also includes new fields for OSSR
\*******************************************************************************
\***           VERSION C       Jamie Thorpe     9th December 2004
\***
\***   Added Read & Write to record 3.
\*******************************************************************************
\***           VERSION D       Jamie Thorpe    13th Match 2006
\***
\***   Added RFSCF.RECOUNT.DAYS.RETAIN% for the Removal of RF counts project
\*******************************************************************************
\***           VERSION E       Peter Sserunkuma   9th September 2008
\***
\***   Added RFSCF.PLANNERS.ACTIVE$.  This was currently named
\***   RFSCF.FILLER$ in record 1.
\*******************************************************************************
\***           VERSION F       Peter Sserunkuma   21st January 2009
\***
\***   Six new fields added to record 3 as part of SFSCF2 changes.
\***   RFSCF.DIRECTS.ACTIVE$
\***   RFSCF.ASN.ACTIVE$
\***   RFSCF.POS.UOD.ACTIVE$
\***   RFSCF.ONIGHT.DELIV$
\***   RFSCF.ONIGHT.SCAN$
\***   RFSCF.SCAN.BATCH$
\*******************************************************************************
\***           VERSION G       Tittoo Thomas          24th May 2012
\***
\***   Added RFSCF.PSP.LEAD.TIME$ in record 1, currently named RFSCF.FILLER$.
\***   1 byte Packed and holds the number of days (usually 7 or 21 days). It
\***   is used to indicate if a pending sales plan planner should be counted
\***   if it becomes active in the next N days.
\*******************************************************************************

   INTEGER*2 GLOBAL                  \
      CURRENT.REPORT.NUM%

   STRING GLOBAL                     \
      CURRENT.CODE$,                 \
      FILE.OPERATION$

   %INCLUDE RFSCFDEC.J86


   FUNCTION RFSCF.SET PUBLIC


    RFSCF.REPORT.NUM%  = 517
    RFSCF.RECL%        = 80
    RFSCF.FILE.NAME$   = "RFSCF"
    RFSCF.REC.NUM%     = 1

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION READ.RFSCF1 PUBLIC

      INTEGER*2 READ.RFSCF1

      READ.RFSCF1 = 1

      IF END #RFSCF.SESS.NUM% THEN END.OF.RFSCF
      READ FORM "4I2,4I4,I1,3I4,I2,4I4,I2,3I4,2I1,I4,C1,I2,C1,C1"; \ BMG
         #RFSCF.SESS.NUM%,1;                          \
           RFSCF.PMEDTERM%,                            \
           RFSCF.QBUSTTERM%,                           \
           RFSCF.PMEDNEXTTXN%,                         \
           RFSCF.QBUSTNXTTXN%,                         \
           RFSCF.PMEDTXNCNT%,                          \
           RFSCF.PMEDQTY%,                             \
           RFSCF.QBUSTTXNCNT%,                         \
           RFSCF.QBUSTQTY%,                            \
           RFSCF.ACTIVITY%,                            \
           RFSCF.LDCPARM1%,                            \
           RFSCF.LDCPARM2%,                            \
           RFSCF.LDCPARM3%,                            \
           RFSCF.PCDATES%,                             \
           RFSCF.PCHKTARGET%,                          \
           RFSCF.CNTPCHK%,                             \
           RFSCF.PCHKUPPER%,                           \
           RFSCF.PCHKLOWER%,                           \
           RFSCF.PCHKINC%,                             \
           RFSCF.PCHKDEFAULT%,                         \
           RFSCF.PCHKERRCNT%,                          \
           RFSCF.PCHKERRLST%,                          \
           RFSCF.EMUACTIVE%,                           \
           RFSCF.PRIMCURR%,                            \ BMG
           RFSCF.EMUCNVFACT%,                          \ BMG
           RFSCF.OSSRSTORE$,                           \ BMG
           RFSCF.RECOUNT.DAYS.RETAIN%,                 \ DJT
           RFSCF.PLANNERS.ACTIVE$,                     \ EPS
           RFSCF.PSP.LEAD.TIME$                        ! GTT
!          RFSCF.FILLER$                               ! EPS ! GTT

      READ.RFSCF1 = 0
      EXIT FUNCTION


END.OF.RFSCF:

         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = RFSCF.REPORT.NUM%

         EXIT FUNCTION


END FUNCTION

\----------------------------------------------------------------------------

FUNCTION READ.RFSCF3 PUBLIC

      INTEGER*2 READ.RFSCF3

      READ.RFSCF3 = 1

      IF END #RFSCF.SESS.NUM% THEN END.OF.READ.RFSCF3
      READ FORM "C1,C30,C3,C1,C1,C1,C1,C1,C3,C38";      \    !FPS
         #RFSCF.SESS.NUM%,3;                            \
           RFSCF.ACTIVE$,                               \
           RFSCF.BCENTRES$,                             \
           RFSCF.CCHIST.NUM.DAYS$,                      \
           RFSCF.DIRECTS.ACTIVE$,                       \    !FPS
           RFSCF.ASN.ACTIVE$,                           \    !FPS
           RFSCF.POS.UOD.ACTIVE$,                       \    !FPS
           RFSCF.ONIGHT.DELIV$,                         \    !FPS
           RFSCF.ONIGHT.SCAN$,                          \    !FPS
           RFSCF.SCAN.BATCH.SIZE$,                      \    !FPS
           RFSCF.FILLER3$

      READ.RFSCF3 = 0
      EXIT FUNCTION

END.OF.READ.RFSCF3:

         FILE.OPERATION$     = "R"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = RFSCF.REPORT.NUM%

         EXIT FUNCTION

END FUNCTION

\----------------------------------------------------------------------------

FUNCTION WRITE.RFSCF1 PUBLIC

      INTEGER*2 WRITE.RFSCF1

      WRITE.RFSCF1 = 1

      IF END #RFSCF.SESS.NUM% THEN END.OF.RFSCF
      WRITE FORM "4I2,4I4,I1,3I4,I2,4I4,I2,3I4,2I1,I4,C1,I2,C1,C1"; \
         #RFSCF.SESS.NUM%,1;                          \  BMG
           RFSCF.PMEDTERM%,                            \ BMG
           RFSCF.QBUSTTERM%,                           \ BMG
           RFSCF.PMEDNEXTTXN%,                         \ BMG
           RFSCF.QBUSTNXTTXN%,                         \ BMG
           RFSCF.PMEDTXNCNT%,                          \ BMG
           RFSCF.PMEDQTY%,                             \ BMG
           RFSCF.QBUSTTXNCNT%,                         \ BMG
           RFSCF.QBUSTQTY%,                            \ BMG
           RFSCF.ACTIVITY%,                            \ BMG
           RFSCF.LDCPARM1%,                            \ BMG
           RFSCF.LDCPARM2%,                            \ BMG
           RFSCF.LDCPARM3%,                            \ BMG
           RFSCF.PCDATES%,                             \ BMG
           RFSCF.PCHKTARGET%,                          \ BMG
           RFSCF.CNTPCHK%,                             \ BMG
           RFSCF.PCHKUPPER%,                           \ BMG
           RFSCF.PCHKLOWER%,                           \ BMG
           RFSCF.PCHKINC%,                             \ BMG
           RFSCF.PCHKDEFAULT%,                         \ BMG
           RFSCF.PCHKERRCNT%,                          \ BMG
           RFSCF.PCHKERRLST%,                          \ BMG
           RFSCF.EMUACTIVE%,                           \ BMG
           RFSCF.PRIMCURR%,                            \ BMG
           RFSCF.EMUCNVFACT%,                          \ BMG
           RFSCF.OSSRSTORE$,                           \ BMG
           RFSCF.RECOUNT.DAYS.RETAIN%,                 \ DJT
           RFSCF.PLANNERS.ACTIVE$,                     \ EPS GTT
           RFSCF.PSP.LEAD.TIME$                        ! GTT

      WRITE.RFSCF1 = 0                                 ! BMG
      EXIT FUNCTION                                    ! BMG

END.OF.RFSCF:                                          ! BMG
                                                       ! BMG
         FILE.OPERATION$     = "W"
         CURRENT.CODE$       = PACK$("0000000000000000") ! BMG
         CURRENT.REPORT.NUM% = RFSCF.REPORT.NUM%         ! BMG

         EXIT FUNCTION                                   ! BMG


END FUNCTION                                             ! BMG

\----------------------------------------------------------------------------

FUNCTION WRITE.RFSCF3 PUBLIC

      INTEGER*2 WRITE.RFSCF3

      WRITE.RFSCF3 = 1
      IF END #RFSCF.SESS.NUM% THEN END.OF.WRITE.RFSCF3
      WRITE FORM "C1,C30,C3,C1,C1,C1,C1,C1,C3,C38";     \    !FPS
         #RFSCF.SESS.NUM%,3;                            \
           RFSCF.ACTIVE$,                               \
           RFSCF.BCENTRES$,                             \
           RFSCF.CCHIST.NUM.DAYS$,                      \
           RFSCF.DIRECTS.ACTIVE$,                       \    !FPS
           RFSCF.ASN.ACTIVE$,                           \    !FPS
           RFSCF.POS.UOD.ACTIVE$,                       \    !FPS
           RFSCF.ONIGHT.DELIV$,                         \    !FPS
           RFSCF.ONIGHT.SCAN$,                          \    !FPS
           RFSCF.SCAN.BATCH.SIZE$,                      \    !FPS
           RFSCF.FILLER3$

      WRITE.RFSCF3 = 0
      EXIT FUNCTION

END.OF.WRITE.RFSCF3:

         FILE.OPERATION$     = "W"
         CURRENT.CODE$       = PACK$("0000000000000000")
         CURRENT.REPORT.NUM% = RFSCF.REPORT.NUM%

         EXIT FUNCTION


END FUNCTION

