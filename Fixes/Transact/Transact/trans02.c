// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
// 
// Version 4.0               Steve Wright                28th August 2003
//     2003 Trial
//     Update to cope with new deal system
//
// Version 4.0               Paul Bowers                 26th September 2003
//    Various fixes for 2003 trial
//    Implement split of transact.c into trans2.c
//
// Version 5.0               Paul Bowers                 25 May 2007
//    Implement changes to support the Recall System in A7C
//
// Version 5.1               Brian Greenfield            23rd August 2007
//    Removed check of PSB70 flag in the RECOK file as the PPC should 
//    be allowed to access recalls regardless of this status.
//
// Version 5.2               Brian Greenfield            30th August 2007
//    Removed write of anCountTSF field back to the Recall as this was 
//    forcing the count to 0 and updating it in the recall file for the 
//    item when this should not be happening. This code was left in after 
//    the count calculation had been removed by PAB.
//    Altered the uncounted to be F's not spaces.
//
// Version 5.3               Brian Greenfield            7th September 2007
//    Changed recall file open errors to be more generic and not critial.
//    If Recall rfindx file is opened but is empty, pass non-critical error
//    to PPC. 
//    Also pass item count status flag to PPC in RCF response. now we are 
//    Also if a list request finds items flagged as Y (probably due to a
//    PPC reload) then pass the session count.
//
// Version 5.4               Brian Greenfield            10th September 2007
//    Changed a couple of files to close all sessions.
//
// Version 5.5               Brian Greenfield            11th September 2007
//    Changed a couple of files to close all sessions.
//    Altered process_sel_stack slightly as print is retried every 15 
//    minutes in transact.c.
//    Reduced print retries to 5 because we now try every 15 minutes as well.
//    Added some debug to StopRecalls.
//
// Version 5.6               Brian Greenfield            14th April 2008
//    Reverse Logistics Changes
//    Recalls of type I & C should not be sent if their expiry date is 
//    today or older. Also expiry date is now passed to pocket PC from
//    newly added fiels in RCINDX file
//
// Version 5.7               Brian Greenfield            30th April 2008
//    Reverse Logistics Modifications
//    Recalls type is now passed in RCD message so only send 
//    recalls that match the request.
//    Also moved process_sel_stack to trans03.c.
//    Added passing of MRQ value to the PPC in the RCD message.
//    Expiry date now passed in active date field in RCC. Removed the 
//    expiry date form the RCC record layout.
//
// Version 5.8               Brian Greenfield            20th June 2008
//    Further Reverse Logistics Modifications
//    The Recall expiry date requirement has changed so that the date of 
//    expiry is now valid. tests changed accordingly.
//
// -----------------------------------------------------------------------

/* include files */
#include "transact.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>
#include "trans2.h"                     // v4.01
#include "adxsrvfn.h"                   /* needed for disp_msg() */
#include "adxsrvst.h"                   /* needed for bg */
#include "file.h"                       // v4.01
#include "rfs.h"  
#include "rfs2.h"                       // PAB 24-May-2007 Recalls
#include "rfsfile.h"
#include "rfsfile2.h"                   // PAB 24-May-2007 Recalls
#include "dateconv.h"
#include "wrap.h"
#include "sockserv.h"
#include "rfglobal.h"                   // v4.0
#include "rfglobal2.h"                  // PAB 24-May-2007 Recalls

//extern UBYTE bg;        // background appl flag for use with disp_msg()
UWORD activity;


//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   Static (private) variables                                           ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

//static BYTE *look_xxx = { "TRANS2  "};                                      // SDH 17-11-04 OSSR WAN
static BYTE sbuf[64];                                                       // SDH 13-12-04 PROMOTIONS
static LONG lLastTDTFFLevel = -1;                                           // SDH 13-12-04 PROMOTIONS

//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   startup() - Do once on startup                                       ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

int startup() {

   WORD i=0, rc=0;
   LONG rc2;
   URC usrrc = RC_OK;
   //RFSCF_REC_1AND2 rfscfrec1and2;                            // SDH 17-11-04 OSSR WAN
   //RFSCF_REC_3 rfscfrec3;                                    // SDH 17-11-04 OSSR WAN

   // Determine application start method
   bg = establish_start_method();
   
   // Set debug on by default if on foreground                  // SDH 4-July-2006
   if (!bg) {                                                   // SDH 4-July-2006
       oput = DBG_LOCAL;                                        // SDH 4-July-2006
       debug = TRUE;                                            // SDH 4-July-2006
       disp_msg("Debug ON");                                    // SDH 4-July-2006
   } else {                                                     // SDH 4-July-2006
       debug = FALSE;                                           // SDH 4-July-2006
   }                                                            // SDH 4-July-2006

   // Print out a status message
   printf( "RFS - Initialising\n" );
   background_msg("RFS - Initialising");

   // Initialize handheld table
   for ( i = 0; i < MAX_UNIT; i++) {
      lrtp[i] = NULL;
   }

   // Initialise process queue table
   for ( i = 0; i < MAX_CONC_UNITS; i++) {
      pq[i].state = PST_FREE;
   }

   oput = DBG_LOCAL;                                                        // PAB 15-7-2004

   //Initialise file structures
   irf.sessions    = 0;
   irf.fnum        = -1L;
   irf.pbFileName = IRF;                                                    // SDH  11-03-2005  EXCESS
   irf.wOpenFlags = IRF_OFLAGS;                                             // SDH  11-03-2005  EXCESS
   irf.wReportNum = IRF_REP;                                                // SDH  11-03-2005  EXCESS
   irf.pBuffer    = &irfrec;                                                // SDH  11-03-2005  EXCESS
   irf.wRecLth    = IRF_RECL;                                               // SDH  11-03-2005  EXCESS
   irf.wKeyLth    = IRF_KEYL;                                               // SDH  11-03-2005  EXCESS

   irfdex.sessions   = 0;                                                   // SDH  26-11-04  Promotions
   irfdex.fnum       = -1L;                                                 // SDH  26-11-04  Promotions
   irfdex.pbFileName = IRFDEX;                                              // SDH  26-11-04  Promotions
   irfdex.wOpenFlags = IRFDEX_OFLAGS;                                       // SDH  26-11-04  Promotions
   irfdex.wReportNum = IRFDEX_REP;                                          // SDH  26-11-04  Promotions
   irfdex.pBuffer    = &irfdexrec;                                          // SDH  26-11-04  Promotions
   irfdex.wRecLth    = IRFDEX_RECL;                                         // SDH  26-11-04  Promotions
   irfdex.wKeyLth    = IRFDEX_KEYL;                                         // SDH  26-11-04  Promotions

   idf.sessions   = 0;
   idf.fnum       = -1L;
   idf.pbFileName = IDF;                                                    // SDH  11-03-2005  EXCESS
   idf.wOpenFlags = IDF_OFLAGS;                                             // SDH  11-03-2005  EXCESS
   idf.wReportNum = IDF_REP;                                                // SDH  11-03-2005  EXCESS
   idf.pBuffer    = &idfrec;                                                // SDH  11-03-2005  EXCESS
   idf.wRecLth    = IDF_RECL;                                               // SDH  11-03-2005  EXCESS
   idf.wKeyLth    = IDF_KEYL;                                               // SDH  11-03-2005  EXCESS

   pgf.sessions   = 0;                                                      // PAB 23-10-03
   pgf.fnum       = -1L;                                                    // PAB 23-10-03
   pgf.pbFileName = PGF;                                                    // SDH  26-11-04  Promotions
   pgf.wOpenFlags = PGF_OFLAGS;                                             // SDH  26-11-04  Promotions
   pgf.wReportNum = PGF_REP;                                                // SDH  26-11-04  Promotions
   pgf.pBuffer    = &pgfrec;                                                // SDH  26-11-04  Promotions
   pgf.wRecLth    = PGF_RECL;                                               // SDH  26-11-04  Promotions
   pgf.wKeyLth    = PGF_KEYL;                                               // SDH  26-11-04  Promotions

   stock.sessions   = 0;
   stock.fnum       = -1L;
   stock.pbFileName = STOCK;                                                // SDH  11-03-2005  EXCESS
   stock.wOpenFlags = STOCK_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   stock.wReportNum = STOCK_REP;                                            // SDH  11-03-2005  EXCESS
   stock.pBuffer    = &stockrec;                                            // SDH  11-03-2005  EXCESS
   stock.wRecLth    = STOCK_RECL;                                           // SDH  11-03-2005  EXCESS
   stock.wKeyLth    = STOCK_KEYL;                                           // SDH  11-03-2005  EXCESS

   citem.sessions   = 0;
   citem.fnum       = -1L;
   citem.pbFileName = CITEM;                                                // SDH  11-03-2005  EXCESS
   citem.wOpenFlags = CITEM_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   citem.wReportNum = CITEM_REP;                                            // SDH  11-03-2005  EXCESS
   //citem.pBuffer    = &citemrec;                                            // SDH  11-03-2005  EXCESS
   citem.wRecLth    = CITEM_RECL;                                           // SDH  11-03-2005  EXCESS
   citem.wKeyLth    = CITEM_KEYL;                                           // SDH  11-03-2005  EXCESS

   cimf.sessions   = 0;
   cimf.fnum       = -1L;
   cimf.pbFileName = CIMF;                                                  // SDH  11-03-2005  EXCESS
   cimf.wOpenFlags = CIMF_OFLAGS;                                           // SDH  11-03-2005  EXCESS
   cimf.wReportNum = CIMF_REP;                                              // SDH  11-03-2005  EXCESS
   //cimf.pBuffer    = &cimfrec;                                              // SDH  11-03-2005  EXCESS
   cimf.wRecLth    = CIMF_RECL;                                             // SDH  11-03-2005  EXCESS
   cimf.wKeyLth    = CIMF_KEYL;                                             // SDH  11-03-2005  EXCESS

   af.sessions   = 0;
   af.fnum       = -1L;
   af.pbFileName = EALAUTH;                                                 // SDH  11-03-2005  EXCESS
   af.wOpenFlags = EALAUTH_OFLAGS;                                          // SDH  11-03-2005  EXCESS
   af.wReportNum = EALAUTH_REP;                                             // SDH  11-03-2005  EXCESS
   //af.pBuffer    = &afrec;                                                  // SDH  11-03-2005  EXCESS
   af.wRecLth    = EALAUTH_RECL;                                            // SDH  11-03-2005  EXCESS

   isf.sessions    = 0;
   isf.fnum        = -1L;
   isf.pbFileName = ISF;                                                    // SDH  11-03-2005  EXCESS
   isf.wOpenFlags = ISF_OFLAGS;                                             // SDH  11-03-2005  EXCESS
   isf.wReportNum = ISF_REP;                                                // SDH  11-03-2005  EXCESS
   isf.pBuffer    = &isfrec;                                                // SDH  11-03-2005  EXCESS
   isf.wRecLth    = ISF_RECL;                                               // SDH  11-03-2005  EXCESS
   isf.wKeyLth    = ISF_KEYL;                                               // SDH  11-03-2005  EXCESS

   stkmq.sessions  = 0;
   stkmq.fnum      = -1L;
   stkmq.pbFileName = STKMQ;                                                // SDH  11-03-2005  EXCESS
   stkmq.wOpenFlags = STKMQ_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   stkmq.wReportNum = STKMQ_REP;                                            // SDH  11-03-2005  EXCESS
   //stkmq.pBuffer    = &stkmqrec;                                            // SDH  11-03-2005  EXCESS
   stkmq.wRecLth    = STKMQ_RECL;                                           // SDH  11-03-2005  EXCESS

   imfp.sessions   = 0;
   imfp.fnum       = -1L;
   imfp.pbFileName = IMFP;                                                  // SDH  11-03-2005  EXCESS
   imfp.wOpenFlags = IMFP_OFLAGS;                                           // SDH  11-03-2005  EXCESS
   imfp.wReportNum = IMFP_REP;                                              // SDH  11-03-2005  EXCESS
   //imfp.pBuffer    = &imfprec;                                              // SDH  11-03-2005  EXCESS
   imfp.wRecLth    = IMFP_RECL;                                             // SDH  11-03-2005  EXCESS
   imfp.wKeyLth    = IMFP_KEYL;                                             // SDH  11-03-2005  EXCESS

   pllol.sessions  = 0;
   pllol.fnum      = -1L;
   pllol.pbFileName = PLLOL;                                                // SDH 22-02-2005 EXCESS
   pllol.wOpenFlags = PLLOL_OFLAGS;                                         // SDH 22-02-2005 EXCESS
   pllol.wReportNum = PLLOL_REP;                                            // SDH 22-02-2005 EXCESS
   pllol.pBuffer    = &pllolrec;                                            // SDH 22-02-2005 EXCESS
   pllol.wRecLth    = PLLOL_RECL;                                           // SDH 22-02-2005 EXCESS
   pllol.wKeyLth    = 0;                                                    // SDH 22-02-2005 EXCESS

   plldb.sessions   = 0;
   plldb.fnum       = -1L;
   plldb.pbFileName = PLLDB;                                                // SDH 22-02-2005 EXCESS
   plldb.wOpenFlags = PLLDB_OFLAGS;                                         // SDH 22-02-2005 EXCESS
   plldb.wReportNum = PLLDB_REP;                                            // SDH 22-02-2005 EXCESS
   plldb.pBuffer    = &plldbrec;                                            // SDH 22-02-2005 EXCESS
   plldb.wRecLth    = PLLDB_RECL;                                           // SDH 22-02-2005 EXCESS
   plldb.wKeyLth    = PLLDB_KEYL;                                           // SDH 22-02-2005 EXCESS

   rfrdesc.sessions = 0;
   rfrdesc.fnum     = -1L;
   rfrdesc.pbFileName = RFRDESC;                                            // SDH  11-03-2005  EXCESS
   rfrdesc.wOpenFlags = RFRDESC_OFLAGS;                                     // SDH  11-03-2005  EXCESS
   rfrdesc.wReportNum = RFRDESC_REP;                                        // SDH  11-03-2005  EXCESS
   //rfrdesc.pBuffer    = &rfrdescrec;                                        // SDH  11-03-2005  EXCESS
   rfrdesc.wRecLth    = RFRDESC_RECL;                                       // SDH  11-03-2005  EXCESS

   tsf.sessions    = 0;
   tsf.fnum        = -1L;
   tsf.pbFileName = TSF;                                                    // SDH  11-03-2005  EXCESS
   tsf.wOpenFlags = TSF_OFLAGS;                                             // SDH  11-03-2005  EXCESS
   tsf.wReportNum = TSF_REP;                                                // SDH  11-03-2005  EXCESS
   //tsf.pBuffer    = &tsfrec;                                                // SDH  11-03-2005  EXCESS
   tsf.wRecLth    = TSF_RECL;                                               // SDH  11-03-2005  EXCESS
   tsf.wKeyLth    = TSF_KEYL;                                               // SDH  11-03-2005  EXCESS

   psbt.sessions   = 0;
   psbt.fnum       = -1L;
   psbt.pbFileName = PSBT;                                                  // SDH  11-03-2005  EXCESS
   psbt.wOpenFlags = PSBT_OFLAGS;                                           // SDH  11-03-2005  EXCESS
   psbt.wReportNum = PSBT_REP;                                              // SDH  11-03-2005  EXCESS
   //psbt.pBuffer    = &psbtrec;                                              // SDH  11-03-2005  EXCESS
   psbt.wRecLth    = PSBT_RECL;                                             // SDH  11-03-2005  EXCESS
   psbt.wKeyLth    = PSBT_KEYL;                                             // SDH  11-03-2005  EXCESS

   wrf.sessions   = 0;
   wrf.fnum       = -1L;
   wrf.pbFileName = WRF;                                                    // SDH  11-03-2005  EXCESS
   wrf.wOpenFlags = WRF_OFLAGS;                                             // SDH  11-03-2005  EXCESS
   wrf.wReportNum = WRF_REP;                                                // SDH  11-03-2005  EXCESS
   //wrf.pBuffer    = &wrfrec;                                                // SDH  11-03-2005  EXCESS
   wrf.wRecLth    = WRF_RECL;                                               // SDH  11-03-2005  EXCESS
   wrf.wKeyLth    = WRF_KEYL;                                               // SDH  11-03-2005  EXCESS

   rfscf.sessions   = 0;
   rfscf.fnum       = -1L;
   rfscf.pbFileName = RFSCF;                                                // SDH  11-03-2005  EXCESS
   rfscf.wOpenFlags = RFSCF_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   rfscf.wReportNum = RFSCF_REP;                                            // SDH  11-03-2005  EXCESS
   //rfscf.pBuffer    = &rfscfrec;                                            // SDH  11-03-2005  EXCESS
   rfscf.wRecLth    = RFSCF_RECL;                                           // SDH  11-03-2005  EXCESS

   minls.sessions   = 0;
   minls.fnum       = -1L;
   minls.pbFileName = MINLS;                                                // SDH  11-03-2005  EXCESS
   minls.wOpenFlags = MINLS_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   minls.wReportNum = MINLS_REP;                                            // SDH  11-03-2005  EXCESS
   minls.pBuffer    = &minlsrec;                                            // SDH  11-03-2005  EXCESS
   minls.wRecLth    = MINLS_RECL;                                           // SDH  11-03-2005  EXCESS
   minls.wKeyLth    = MINLS_KEYL;                                           // SDH  11-03-2005  EXCESS
   
   rfhist.sessions = 0;
   rfhist.fnum     = -1L;
   rfhist.pbFileName = RFHIST;                                              // SDH  26-11-04  CREDIT CLAIMING
   rfhist.wOpenFlags = RFHIST_OFLAGS;                                       // SDH  26-11-04  CREDIT CLAIMING
   rfhist.wReportNum = RFHIST_REP;                                          // SDH  26-11-04  CREDIT CLAIMING
   rfhist.pBuffer    = &rfhistrec;                                          // SDH  26-11-04  CREDIT CLAIMING
   rfhist.wRecLth    = RFHIST_RECL;                                         // SDH  26-11-04  CREDIT CLAIMING
   rfhist.wKeyLth    = RFHIST_KEYL;                                         // SDH  26-11-04  CREDIT CLAIMING

   invok.sessions   = 0;
   invok.fnum       = -1L;
   invok.pbFileName = INVOK;                                                // SDH  11-03-2005  EXCESS
   invok.wOpenFlags = INVOK_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   invok.wReportNum = INVOK_REP;                                            // SDH  11-03-2005  EXCESS
   invok.pBuffer    = &invokrec;                                            // SDH  11-03-2005  EXCESS
   invok.wRecLth    = INVOK_RECL;                                           // SDH  11-03-2005  EXCESS

   clolf.sessions = 0;
   clolf.fnum     = -1L;
   clolf.pbFileName = CLOLF;                                                // SDH  11-03-2005  EXCESS
   clolf.wOpenFlags = CLOLF_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   clolf.wReportNum = CLOLF_REP;                                            // SDH  11-03-2005  EXCESS
   //clolf.pBuffer    = &clolfrec;                                            // SDH  11-03-2005  EXCESS
   clolf.wRecLth    = CLOLF_RECL;                                           // SDH  11-03-2005  EXCESS

   clilf.sessions  = 0;
   clilf.fnum      = -1L;
   clilf.pbFileName = CLILF;                                                // SDH  11-03-2005  EXCESS
   clilf.wOpenFlags = CLILF_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   clilf.wReportNum = CLILF_REP;                                            // SDH  11-03-2005  EXCESS
   //clilf.pBuffer    = &clilfrec;                                            // SDH  11-03-2005  EXCESS
   clilf.wRecLth    = CLILF_RECL;                                           // SDH  11-03-2005  EXCESS
   clilf.wKeyLth    = CLILF_KEYL;                                           // SDH  11-03-2005  EXCESS

   cpipe.sessions   = 0;
   cpipe.fnum       = -1L;
   cpipe.pbFileName = CPIPE;                                                // SDH  11-03-2005  EXCESS
   //cpipe.wOpenFlags = CPIPE_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   cpipe.wReportNum = CPIPE_REP;                                            // SDH  11-03-2005  EXCESS
   //cpipe.pBuffer    = &cpiperec;                                            // SDH  11-03-2005  EXCESS
   cpipe.wRecLth    = CPIPE_RECL;                                           // SDH  11-03-2005  EXCESS

   rfsstat.sessions= 0;
   rfsstat.fnum    = -1L;
   rfsstat.pbFileName = SM_BUFFER_NAME;                                     // SDH  11-03-2005  EXCESS
   rfsstat.wOpenFlags = A_READ | A_WRITE | A_SHARE;                         // SDH  11-03-2005  EXCESS
   rfsstat.wReportNum = 0;                                                  // SDH  11-03-2005  EXCESS
   //rfsstat.pBuffer    = &rfsstatrec;                                        // SDH  11-03-2005  EXCESS
   rfsstat.wRecLth    = SM_BUFFER_SIZE;                                     // SDH  11-03-2005  EXCESS

   pilst.sessions  = 0;
   pilst.fnum      = -1L;
   pilst.pbFileName = PILST;                                                // SDH  11-03-2005  EXCESS
   pilst.wOpenFlags = PILST_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   pilst.wReportNum = PILST_REP;                                            // SDH  11-03-2005  EXCESS
   //pilst.pBuffer    = &pilstrec;                                            // SDH  11-03-2005  EXCESS
   pilst.wRecLth    = PILST_RECL;                                           // SDH  11-03-2005  EXCESS
   pilst.wKeyLth    = PILST_KEYL;                                           // SDH  11-03-2005  EXCESS

   nvurl.sessions  = 0;
   nvurl.fnum      = -1L;
   nvurl.pbFileName = NVURL;                                                // SDH  11-03-2005  EXCESS
   nvurl.wOpenFlags = NVURL_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   nvurl.wReportNum = NVURL_REP;                                            // SDH  11-03-2005  EXCESS
   //nvurl.pBuffer    = &nvurlrec;                                            // SDH  11-03-2005  EXCESS
   nvurl.wRecLth    = NVURL_RECL;                                           // SDH  11-03-2005  EXCESS
   nvurl.wKeyLth    = NVURL_KEYL;                                           // SDH  11-03-2005  EXCESS

   epsom.sessions  = 0;                                             // v4.0
   epsom.fnum      = -1L;                                           // v4.0
   epsom.pbFileName = EPSOM;                                        // SDH  11-03-2005  EXCESS
   epsom.wOpenFlags = EPSOM_OFLAGS;                                 // SDH  11-03-2005  EXCESS
   epsom.wReportNum = EPSOM_REP;                                    // SDH  11-03-2005  EXCESS
   //epsom.pBuffer    = &epsomrec;                                  // SDH  11-03-2005  EXCESS
   epsom.wRecLth    = EPSOM_RECL;                                   // SDH  11-03-2005  EXCESS

   pchk.sessions   = 0;                                             // v4.0
   pchk.fnum       = -1L;                                           // v4.0
   pchk.pbFileName = PCHK;                                          // SDH  11-03-2005  EXCESS
   pchk.wOpenFlags = PCHK_OFLAGS;                                   // SDH  11-03-2005  EXCESS
   pchk.wReportNum = PCHK_REP;                                      // SDH  11-03-2005  EXCESS
   //pchk.pBuffer    = &pchkrec;                                    // SDH  11-03-2005  EXCESS
   pchk.wRecLth    = PCHK_RECL;                                     // SDH  11-03-2005  EXCESS

   bcsmf.sessions   = 0;                                            // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.fnum       = -1L;                                          // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.pbFileName = BCSMF;                                        // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.wOpenFlags = BCSMF_OFLAGS;                                 // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.wReportNum = BCSMF_REP;                                    // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.pBuffer    = &bcsmfrec;                                    // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.wRecLth    = BCSMF_RECL;                                   // SDH  26-11-04  CREDIT CLAIMING
   bcsmf.wKeyLth    = BCSMF_KEYL;                                   // SDH  26-11-04  CREDIT CLAIMING

   cclol.sessions   = 0;                                            // SDH  26-11-04  CREDIT CLAIMING
   cclol.fnum       = -1L;                                          // SDH  26-11-04  CREDIT CLAIMING
   cclol.pbFileName = CCLOL;                                        // SDH  26-11-04  CREDIT CLAIMING
   cclol.wOpenFlags = CCLOL_OFLAGS;                                 // SDH  26-11-04  CREDIT CLAIMING
   cclol.wReportNum = CCLOL_REP;                                    // SDH  26-11-04  CREDIT CLAIMING
   cclol.pBuffer    = &cclolrec;                                    // SDH  26-11-04  CREDIT CLAIMING
   cclol.wRecLth    = CCLOL_RECL;                                   // SDH  26-11-04  CREDIT CLAIMING
   cclol.wKeyLth    = 0;  //Direct file                             // SDH  26-11-04  CREDIT CLAIMING

   ccilf.sessions   = 0;                                            // SDH  26-11-04  CREDIT CLAIMING
   ccilf.fnum       = -1L;                                          // SDH  26-11-04  CREDIT CLAIMING
   ccilf.pbFileName = CCILF;                                        // SDH  26-11-04  CREDIT CLAIMING
   ccilf.wOpenFlags = CCILF_OFLAGS;                                 // SDH  26-11-04  CREDIT CLAIMING
   ccilf.wReportNum = CCILF_REP;                                    // SDH  26-11-04  CREDIT CLAIMING
   ccilf.pBuffer    = &ccilfrec;                                    // SDH  26-11-04  CREDIT CLAIMING
   ccilf.wRecLth    = CCILF_RECL;                                   // SDH  26-11-04  CREDIT CLAIMING
   ccilf.wKeyLth    = CCILF_KEYL;                                   // SDH  26-11-04  CREDIT CLAIMING

   cchist.sessions   = 0;                                           // SDH  26-11-04  CREDIT CLAIMING
   cchist.fnum       = -1L;                                         // SDH  26-11-04  CREDIT CLAIMING
   cchist.pbFileName = CCHIST;                                      // SDH  26-11-04  CREDIT CLAIMING
   cchist.wOpenFlags = CCHIST_OFLAGS;                               // SDH  26-11-04  CREDIT CLAIMING
   cchist.wReportNum = CCHIST_REP;                                  // SDH  26-11-04  CREDIT CLAIMING
   cchist.pBuffer    = &cchistrec;                                  // SDH  26-11-04  CREDIT CLAIMING
   cchist.wRecLth    = CCHIST_RECL;                                 // SDH  26-11-04  CREDIT CLAIMING
   cchist.wKeyLth    = CCHIST_KEYL;                                 // SDH  26-11-04  CREDIT CLAIMING

   ccdirsu.sessions   = 0;                                          // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.fnum       = -1L;                                        // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.pbFileName = CCDIRSU;                                    // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.wOpenFlags = CCDIRSU_OFLAGS;                             // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.wReportNum = CCDIRSU_REP;                                // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.pBuffer    = &ccdirsurec;                                // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.wRecLth    = CCDIRSU_RECL;                               // SDH  26-11-04  CREDIT CLAIMING
   ccdirsu.wKeyLth    = CCDIRSU_KEYL;                               // SDH  26-11-04  CREDIT CLAIMING

   deal.sessions   = 0;                                             // SDH  26-11-04  CREDIT CLAIMING
   deal.fnum       = -1L;                                           // SDH  26-11-04  CREDIT CLAIMING
   deal.pbFileName = DEAL;                                          // SDH  26-11-04  CREDIT CLAIMING
   deal.wOpenFlags = DEAL_OFLAGS;                                   // SDH  26-11-04  CREDIT CLAIMING
   deal.wReportNum = DEAL_REP;                                      // SDH  26-11-04  CREDIT CLAIMING
   deal.pBuffer    = &dealrec;                                      // SDH  26-11-04  CREDIT CLAIMING
   deal.wRecLth    = DEAL_RECL;                                     // SDH  26-11-04  CREDIT CLAIMING
   deal.wKeyLth    = DEAL_KEYL;                                     // SDH  26-11-04  CREDIT CLAIMING

   tdtff.sessions   = 0;                                            // SDH  26-11-04  CREDIT CLAIMING
   tdtff.fnum       = -1L;                                          // SDH  26-11-04  CREDIT CLAIMING
   tdtff.pbFileName = TDTFF;                                        // SDH  26-11-04  CREDIT CLAIMING
   tdtff.wOpenFlags = TDTFF_OFLAGS;                                 // SDH  26-11-04  CREDIT CLAIMING
   tdtff.wReportNum = TDTFF_REP;                                    // SDH  26-11-04  CREDIT CLAIMING
   tdtff.pBuffer    = &tdtffrec;                                    // SDH  26-11-04  CREDIT CLAIMING
   tdtff.wRecLth    = TDTFF_RECL;                                   // SDH  26-11-04  CREDIT CLAIMING
   tdtff.wKeyLth    = 0;                                            // SDH  26-11-04  CREDIT CLAIMING
   
   prtlist.sessions   = 0;                                          // SDH  14-03-05  EXCESS
   prtlist.fnum       = -1L;                                        // SDH  14-03-05  EXCESS
   prtlist.pbFileName = PRTLIST;                                    // SDH  14-03-05  EXCESS
   prtlist.wOpenFlags = PRTLIST_OFLAGS;                             // SDH  14-03-05  EXCESS
   prtlist.wReportNum = PRTLIST_REP;                                // SDH  14-03-05  EXCESS
   prtlist.wRecLth    = PRTLIST_RECL;                               // SDH  14-03-05  EXCESS
   prtlist.wKeyLth    = 0;                                          // SDH  14-03-05  EXCESS

   rfok.sessions   = 0;                                             // SDH  14-03-05  EXCESS
   rfok.fnum       = -1L;                                           // SDH  14-03-05  EXCESS
   rfok.pbFileName = RFOK;                                          // SDH  14-03-05  EXCESS
   rfok.wOpenFlags = RFOK_OFLAGS;                                   // SDH  14-03-05  EXCESS
   rfok.wReportNum = RFOK_REP;                                      // SDH  14-03-05  EXCESS
   rfok.wRecLth    = RFOK_RECL;                                     // SDH  14-03-05  EXCESS
   rfok.wKeyLth    = 0;                                             // SDH  14-03-05  EXCESS

   prtctl.sessions   = 0;                                           // SDH  14-03-05  EXCESS
   prtctl.fnum       = -1L;                                         // SDH  14-03-05  EXCESS
   prtctl.pbFileName = PRTCTL;                                      // SDH  14-03-05  EXCESS
   prtctl.wOpenFlags = PRTCTL_OFLAGS;                               // SDH  14-03-05  EXCESS
   prtctl.wReportNum = PRTCTL_REP;                                  // SDH  14-03-05  EXCESS
   prtctl.wRecLth    = PRTCTL_RECL;                                 // SDH  14-03-05  EXCESS
   prtctl.wKeyLth    = 0;                                           // SDH  14-03-05  EXCESS
   
   jobok.sessions   = 0;                                            // SDH  14-03-05  EXCESS
   jobok.fnum       = -1L;                                          // SDH  14-03-05  EXCESS
   jobok.pbFileName = JOBOK;                                        // SDH  14-03-05  EXCESS
   jobok.wOpenFlags = JOBOK_OFLAGS;                                 // SDH  14-03-05  EXCESS
   jobok.wReportNum = JOBOK_REP;                                    // SDH  14-03-05  EXCESS
   jobok.wRecLth    = JOBOK_RECL;                                   // SDH  14-03-05  EXCESS
   jobok.wKeyLth    = 0;                                            // SDH  14-03-05  EXCESS
   
   imstc.sessions   = 0;                                            // SDH  14-03-05  EXCESS
   imstc.fnum       = -1L;                                          // SDH  14-03-05  EXCESS
   imstc.pbFileName = IMSTC;                                        // SDH  14-03-05  EXCESS
   imstc.wOpenFlags = IMSTC_OFLAGS;                                 // SDH  14-03-05  EXCESS
   imstc.wReportNum = IMSTC_REP;                                    // SDH  14-03-05  EXCESS
   imstc.wRecLth    = IMSTC_RECL;                                   // SDH  14-03-05  EXCESS
   imstc.wKeyLth    = IMSTC_KEYL;                                   // SDH  14-03-05  EXCESS
   
   piitm.sessions   = 0;                                            // SDH  14-03-05  EXCESS
   piitm.fnum       = -1L;                                          // SDH  14-03-05  EXCESS
   piitm.pbFileName = PIITM;                                        // SDH  14-03-05  EXCESS
   piitm.wOpenFlags = PIITM_OFLAGS;                                 // SDH  14-03-05  EXCESS
   piitm.wReportNum = PIITM_REP;                                    // SDH  14-03-05  EXCESS
   piitm.wRecLth    = PIITM_RECL;                                   // SDH  14-03-05  EXCESS
   piitm.wKeyLth    = PIITM_KEYL;                                   // SDH  14-03-05  EXCESS

   suspt.sessions   = 0;                                            // SDH  14-03-05  EXCESS
   suspt.fnum       = -1L;                                          // SDH  14-03-05  EXCESS
   suspt.pbFileName = SUSPT;                                        // SDH  14-03-05  EXCESS
   suspt.wOpenFlags = SUSPT_OFLAGS;                                 // SDH  14-03-05  EXCESS
   suspt.wReportNum = SUSPT_REP;                                    // SDH  14-03-05  EXCESS
   suspt.wRecLth    = SUSPT_RECL;                                   // SDH  14-03-05  EXCESS
   suspt.wKeyLth    = SUSPT_KEYL;                                   // SDH  14-03-05  EXCESS

   pogok.sessions   = 0;                                            // SDH  23-Aug-06 Planners
   pogok.fnum       = -1L;                                          // SDH  23-Aug-06 Planners
   pogok.pbFileName = POGOK;                                        // SDH  23-Aug-06 Planners
   pogok.wOpenFlags = POGOK_OFLAGS;                                 // SDH  23-Aug-06 Planners
   pogok.wReportNum = POGOK_REP;                                    // SDH  23-Aug-06 Planners
   pogok.pBuffer    = &pogokrec;                                    // SDH  23-Aug-06 Planners
   pogok.wRecLth    = POGOK_RECL;                                   // SDH  23-Aug-06 Planners
   pogok.wKeyLth    = 0;                                            // SDH  23-Aug-06 Planners

   srpog.sessions   = 0;                                            // SDH  23-Aug-06 Planners
   srpog.fnum       = -1L;                                          // SDH  23-Aug-06 Planners
   srpog.pbFileName = SRPOG;                                        // SDH  23-Aug-06 Planners
   srpog.wOpenFlags = SRPOG_OFLAGS;                                 // SDH  23-Aug-06 Planners
   srpog.wReportNum = SRPOG_REP;                                    // SDH  23-Aug-06 Planners
   srpog.pBuffer    = &srpogrec;                                    // SDH  23-Aug-06 Planners
   srpog.wRecLth    = SRPOG_RECL;                                   // SDH  23-Aug-06 Planners
   srpog.wKeyLth    = SRPOG_KEYL;                                   // SDH  23-Aug-06 Planners

   srmod.sessions   = 0;                                            // SDH  23-Aug-06 Planners
   srmod.fnum       = -1L;                                          // SDH  23-Aug-06 Planners
   srmod.pbFileName = SRMOD;                                        // SDH  23-Aug-06 Planners
   srmod.wOpenFlags = SRMOD_OFLAGS;                                 // SDH  23-Aug-06 Planners
   srmod.wReportNum = SRMOD_REP;                                    // SDH  23-Aug-06 Planners
   srmod.pBuffer    = &srmodrec;                                    // SDH  23-Aug-06 Planners
   srmod.wRecLth    = SRMOD_RECL;                                   // SDH  23-Aug-06 Planners
   srmod.wKeyLth    = SRMOD_KEYL;                                   // SDH  23-Aug-06 Planners

   sritml.sessions   = 0;                                           // SDH  23-Aug-06 Planners
   sritml.fnum       = -1L;                                         // SDH  23-Aug-06 Planners
   sritml.pbFileName = SRITML;                                      // SDH  23-Aug-06 Planners
   sritml.wOpenFlags = SRITML_OFLAGS;                               // SDH  23-Aug-06 Planners
   sritml.wReportNum = SRITML_REP;                                  // SDH  23-Aug-06 Planners
   sritml.pBuffer     = &sritmlrec;                                 // SDH  23-Aug-06 Planners
   sritml.wRecLth    = SRITML_RECL;                                 // SDH  23-Aug-06 Planners
   sritml.wKeyLth    = SRITML_KEYL;                                 // SDH  23-Aug-06 Planners

   sritmp.sessions   = 0;                                           // SDH  23-Aug-06 Planners
   sritmp.fnum       = -1L;                                         // SDH  23-Aug-06 Planners
   sritmp.pbFileName = SRITMP;                                      // SDH  23-Aug-06 Planners
   sritmp.wOpenFlags = SRITMP_OFLAGS;                               // SDH  23-Aug-06 Planners
   sritmp.wReportNum = SRITMP_REP;                                  // SDH  23-Aug-06 Planners
   sritmp.pBuffer    = &sritmprec;                                  // SDH  23-Aug-06 Planners
   sritmp.wRecLth    = SRITMP_RECL;                                 // SDH  23-Aug-06 Planners
   sritmp.wKeyLth    = SRITMP_KEYL;                                 // SDH  23-Aug-06 Planners

   srpogif.sessions   = 0;                                          // SDH  23-Aug-06 Planners
   srpogif.fnum       = -1L;                                        // SDH  23-Aug-06 Planners
   srpogif.pbFileName = SRPOGIF;                                    // SDH  23-Aug-06 Planners
   srpogif.wOpenFlags = SRPOGIF_OFLAGS;                             // SDH  23-Aug-06 Planners
   srpogif.wReportNum = SRPOGIF_REP;                                // SDH  23-Aug-06 Planners
   srpogif.pBuffer    = &srpogifrec;                                // SDH  23-Aug-06 Planners
   srpogif.wRecLth    = SRPOGIF_RECL;                               // SDH  23-Aug-06 Planners
   srpogif.wKeyLth    = 0;                                          // SDH  23-Aug-06 Planners

   srpogil.sessions   = 0;                                          // SDH  23-Aug-06 Planners
   srpogil.fnum       = -1L;                                        // SDH  23-Aug-06 Planners
   srpogil.pbFileName = SRPOGIL;                                    // SDH  23-Aug-06 Planners
   srpogil.wOpenFlags = SRPOGIL_OFLAGS;                             // SDH  23-Aug-06 Planners
   srpogil.wReportNum = SRPOGIL_REP;                                // SDH  23-Aug-06 Planners
   srpogil.pBuffer    = &srpogilrec;                                // SDH  23-Aug-06 Planners
   srpogil.wRecLth    = SRPOGIL_RECL;                               // SDH  23-Aug-06 Planners
   srpogil.wKeyLth    = 0;                                          // SDH  23-Aug-06 Planners

   srpogip.sessions   = 0;                                          // SDH  23-Aug-06 Planners
   srpogip.fnum       = -1L;                                        // SDH  23-Aug-06 Planners
   srpogip.pbFileName = SRPOGIP;                                    // SDH  23-Aug-06 Planners
   srpogip.wOpenFlags = SRPOGIP_OFLAGS;                             // SDH  23-Aug-06 Planners
   srpogip.wReportNum = SRPOGIP_REP;                                // SDH  23-Aug-06 Planners
   srpogip.pBuffer    = &srpogiprec;                                // SDH  23-Aug-06 Planners
   srpogip.wRecLth    = SRPOGIP_RECL;                               // SDH  23-Aug-06 Planners
   srpogip.wKeyLth    = 0;                                          // SDH  23-Aug-06 Planners

   srcat.sessions   = 0;                                            // SDH  23-Aug-06 Planners
   srcat.fnum       = -1L;                                          // SDH  23-Aug-06 Planners
   srcat.pbFileName = SRCAT;                                        // SDH  23-Aug-06 Planners
   srcat.wOpenFlags = SRCAT_OFLAGS;                                 // SDH  23-Aug-06 Planners
   srcat.wReportNum = SRCAT_REP;                                    // SDH  23-Aug-06 Planners
   srcat.pBuffer    = &srcatrec;                                    // SDH  23-Aug-06 Planners
   srcat.wRecLth    = SRCAT_RECL;                                   // SDH  23-Aug-06 Planners
   srcat.wKeyLth    = 0;                                            // SDH  23-Aug-06 Planners

   srsdf.sessions   = 0;                                            // SDH  23-Aug-06 Planners
   srsdf.fnum       = -1L;                                          // SDH  23-Aug-06 Planners
   srsdf.pbFileName = SRSDF;                                        // SDH  23-Aug-06 Planners
   srsdf.wOpenFlags = SRSDF_OFLAGS;                                 // SDH  23-Aug-06 Planners
   srsdf.wReportNum = SRSDF_REP;                                    // SDH  23-Aug-06 Planners
   srsdf.pBuffer    = &srsdfrec;                                    // SDH  23-Aug-06 Planners
   srsdf.wRecLth    = SRSDF_RECL;                                   // SDH  23-Aug-06 Planners
   srsdf.wKeyLth    = SRSDF_KEYL;                                   // SDH  23-Aug-06 Planners

   srsxf.sessions   = 0;                                           // SDH  23-Aug-06 Planners
   srsxf.fnum       = -1L;                                         // SDH  23-Aug-06 Planners
   srsxf.pbFileName = SRSXF;                                       // SDH  23-Aug-06 Planners
   srsxf.wOpenFlags = SRSXF_OFLAGS;                                // SDH  23-Aug-06 Planners
   srsxf.wReportNum = SRSXF_REP;                                   // SDH  23-Aug-06 Planners
   srsxf.pBuffer    = &srsxfrec;                                   // SDH  23-Aug-06 Planners
   srsxf.wRecLth    = SRSXF_RECL;                                  // SDH  23-Aug-06 Planners
   srsxf.wKeyLth    = SRSXF_KEYL;                                  // SDH  23-Aug-06 Planners

     // recall files follow                                           24-5-2007 PAB

   rcindx.sessions   = 0;                                            // PAB 24-May-2007 Recalls
   rcindx.fnum       = -1L;                                          // PAB 24-May-2007 Recalls
   rcindx.pbFileName = RCINDX;                                       // PAB 24-May-2007 Recalls
   rcindx.wOpenFlags = RCINDX_OFLAGS;                                // PAB 24-May-2007 Recalls
   rcindx.wReportNum = RCINDX_REP;                                   // PAB 24-May-2007 Recalls
   rcindx.pBuffer    = &rcindxrec;                                   // PAB 24-May-2007 Recalls
   rcindx.wRecLth    = RCINDX_RECL;                                  // PAB 24-May-2007 Recalls
   rcindx.wKeyLth    = 0;                                            // PAB 24-May-2007 Recalls

   recok.sessions   = 0;                                             // PAB 24-May-2007 Recalls
   recok.fnum       = -1L;                                           // PAB 24-May-2007 Recalls
   recok.pbFileName = RECOK;                                         // PAB 24-May-2007 Recalls
   recok.wOpenFlags = RECOK_OFLAGS;                                  // PAB 24-May-2007 Recalls
   recok.wReportNum = RECOK_REP;                                     // PAB 24-May-2007 Recalls
   recok.pBuffer    = &recokrec;                                     // PAB 24-May-2007 Recalls
   recok.wRecLth    = RECOK_RECL;                                    // PAB 24-May-2007 Recalls
   recok.wKeyLth    = 0;                                             // PAB 24-May-2007 Recalls

   rcspi.sessions   = 0;                                             // PAB 24-May-2007 Recalls
   rcspi.fnum       = -1L;                                           // PAB 24-May-2007 Recalls
   rcspi.pbFileName = RCSPI;                                         // PAB 24-May-2007 Recalls
   rcspi.wOpenFlags = RCSPI_OFLAGS;                                  // PAB 24-May-2007 Recalls
   rcspi.wReportNum = RCSPI_REP;                                     // PAB 24-May-2007 Recalls
   rcspi.pBuffer    = &rcspirec;                                     // PAB 24-May-2007 Recalls
   rcspi.wRecLth    = RCSPI_RECL;                                    // PAB 24-May-2007 Recalls
   rcspi.wKeyLth    = 0;                                             // PAB 24-May-2007 Recalls

   recall.sessions   = 0;                                             // PAB 24-May-2007 Recalls
   recall.fnum       = -1L;                                           // PAB 24-May-2007 Recalls
   recall.pbFileName = RECALL;                                        // PAB 24-May-2007 Recalls
   recall.wOpenFlags = RECALL_OFLAGS;                                 // PAB 24-May-2007 Recalls
   recall.wReportNum = RECALL_REP;                                    // PAB 24-May-2007 Recalls
   recall.pBuffer    = &recallrec;                                    // PAB 24-May-2007 Recalls
   recall.wRecLth    = RECALL_RECL;                                   // PAB 24-May-2007 Recalls
   recall.wKeyLth    = 0;                                             // PAB 24-May-2007 Recalls

   prepare_logging();

//   cpipe.fnum = s_create( O_FILE, CPIPE_CFLAGS, CPIPE, 1, 0, CPIPE_RECL ); // v4.0
   cpipe.fnum = s_create( O_FILE, CPIPE_CFLAGS, CPIPE, 1, 0x0FFF, CPIPE_RECL );  // v4.0

   if ( cpipe.fnum<=0 ) { 
      log_event101(cpipe.fnum, CPIPE_REP, __LINE__);
      sprintf( msg, "Error - Unable to create Comms pipe" );
      disp_msg(msg);
   // return 1; // dont quit continue anyway
   }
   
   // Prepare shared memory
   usrrc = open_rfsstat();
   if ( usrrc == RC_OK ) {
      memset( share, 0x00, (WORD)SM_BUFFER_SIZE );
   }
   
   // Read INVOK record
   usrrc = open_invok();
   if ( usrrc<=RC_DATA_ERR ) {
      printf("ERROR - Unable to open INVOK file. Check appl. event logs\n");
      return 1;
   }
   rc2 = s_read( A_BOFOFF, invok.fnum, (void *)&invokrec, INVOK_RECL, 0L );
   if ( rc2<=0L ) {
      log_event101(rc2, INVOK_REP, __LINE__);
      sprintf(msg, "Err-R INVOK. RC:%08lX", rc2);
      disp_msg(msg);
   }
   close_invok( CL_SESSION );
   
   // Process any orphan work files that exist
   process_orphans(TRUE);

   /////////////////////////////////////////////////////////////////////////////
   //
   //  Process the RFS Control File (RFSCF)
   //
   /////////////////////////////////////////////////////////////////////////////
   
   //Open it
   usrrc = open_rfscf();
   
   if ( usrrc==RC_OK ) {
      
      // Read RFSCF record 1 & 3
      rc2 = s_read(A_BOFOFF, rfscf.fnum, (void *)&rfscfrec1and2, RFSCF_RECL, 0L);           // 16-11-04 SDH
      if (rc2 > 0L) {                                                                       // 16-11-04 SDH
          rc2 = s_read(A_BOFOFF, rfscf.fnum, (void *)&rfscfrec3, RFSCF_RECL, RFSCF_RECL*2); // 16-11-04 SDH
      }                                                                                     // 16-11-04 SDH
      if ( rc2<=0L ) {
         log_event101(rc2, RFSCF_REP, __LINE__);
         printf(msg, "Err-R RFSCF. RC:%08lX", rc2);
      }
      
      activity = rfscfrec1and2.activity;                                // 16-11-04 SDH
      
      //Lock EPSOM and PCHK to prevent EPSOM counts and price checks
      //if the phase is greater than or equal to 1
      if ( satol(rfscfrec1and2.phase,1)>=1 ) {                          // 16-11-04 SDH
          open_epsom();                                          
          open_pchk();
      }

      //If Credit Claiming is active then lock the CCDMY to       // 16-11-04 SDH
      //prevent PDTs from using Credit Claiming                   // 16-11-04 SDH
      //Only call the open routine so that the number of          // 16-11-04 SDH
      //sessions is not continuously incremented                  // 16-11-04 SDH
      if (rfscfrec3.bCCActive == 'Y') {                           // 16-11-04 SDH
          open_ccdmy_locked();                                    // 16-11-04 SDH
      } else {                                                    // 16-11-04 SDH
          close_ccdmy();                                          // 16-11-04 SDH
      }                                                           // 16-11-04 SDH

      close_rfscf( CL_SESSION );

   } else {
      activity = 0;
   }

   //printf( "RFS - Ready\n" );
   sprintf( msg, "RFS - Restarting, Please wait..."); 
   background_msg(msg);

   return(rc);

}

 
void SignOffNak ( char * pNAK, char * pMsg, WORD * iLength )
{
   memcpy( pNAK, "NAK", 3 );
   memcpy( pMsg,
           "Store close has      "
           "started please       "
           "sign off.            "
           "*****************", 80 );
   *iLength = LRT_NAK_LTH;
}

void prep_nak( BYTE *msg ) {                                                //SDH 23-Aug-2006 Planners
   memcpy(((LRT_NAK*)out)->cmd, "NAK", 3);                                  //SDH 23-Aug-2006 Planners
   strncpy(((LRT_NAK*)out)->msg, msg, sizeof(((LRT_NAK*)out)->msg));        //SDH 23-Aug-2006 Planners
   out_lth = LRT_NAK_LTH;                                                   //SDH 23-Aug-2006 Planners
}

void prep_ack( BYTE *msg ) {                                                //SDH 23-Aug-2006 Planners
   memcpy(((LRT_ACK*)out)->cmd, "ACK", 3);                                  //SDH 23-Aug-2006 Planners
   strncpy(((LRT_ACK*)out)->msg, msg, sizeof(((LRT_ACK*)out)->msg));        //SDH 23-Aug-2006 Planners
   out_lth = LRT_ACK_LTH;                                                   //SDH 23-Aug-2006 Planners
}

void prep_pq_full_nak(void) {
    prep_nak("ERROR  SELs or gap  report stalled.     Contact help desk.  ");//SDH 23-Aug-2006 Planners
}

BOOLEAN IsStoreClosed(void) {                                               // SDH 26-11-04 CREDIT CLAIM
    if (cStoreClosed == 'Y') {                                              // SDH 26-11-04 CREDIT CLAIM
        prep_nak("Store close in progress please try later");               //SDH 23-Aug-2006 Planners
        return TRUE;                                                        // SDH 26-11-04 CREDIT CLAIM
    }                                                                       // SDH 26-11-04 CREDIT CLAIM
    return FALSE;                                                           // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

BOOLEAN IsHandheldUnknown(void) {                                           // SDH 26-11-04 CREDIT CLAIM
    if (lrtp[hh_unit] == NULL) {                                            // SDH 26-11-04 CREDIT CLAIM
        // We haven't seen this handheld before                             // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORPlease sign off and then sign on again");            //SDH 23-Aug-2006 Planners
        return TRUE;                                                        // SDH 26-11-04 CREDIT CLAIM
    }                                                                       // SDH 26-11-04 CREDIT CLAIM
    return FALSE;                                                           // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

void UpdateActiveTime (void) {                                              // SDH 26-11-04 CREDIT CLAIM
    TIMEDATE now;                                                           // SDH 26-11-04 CREDIT CLAIM
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                              // SDH 26-11-04 CREDIT CLAIM
    lrtp[hh_unit]->last_active_time = now.td_time;                          // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

BOOLEAN IsReportMntActive(void) {                                           // 4-11-04 PAB
    
    URC urc = process_rfok();                                               // 17-3-2005 SDH
    if ((urc <= RC_DATA_ERR) ||                                             // 17-3-2005 SDH
        (rfokrec.rfmaint == 'S')) {                                         // 4-11-04 PAB
        prep_nak("Report Maintenance in progress please try again later");  //SDH 23-Aug-2006 Planners
        if (lrtp[hh_unit]->fnum3!=0L) {                                     // 11-11-04 PAB
            // if this unit has any report open then close it               // 11-11-04 PAB
            s_close( 0, lrtp[hh_unit]->fnum3 );                             // 11-11-04 PAB
            lrtp[hh_unit]->fnum3 = 0L;                                      // 11-11-04 PAB
        }                                                                   // 11-11-04 PAB
        return TRUE;                                                        // 11-11-04 PAB
    }                                                                       // 11-11-04 PAB
    return FALSE;                                                           // 11-11-04 PAB
}                                                                           // 11-11-04 PAB

//////////////////////////////////////////////////////////////////////////////
//
//  Load the DEAL file and store the first deal reward message for each deal
//  NOTE: Should only be called from DealFileTrickle() to ensure that 
//  lLastTDTFFLevel is set up correctly AND DEAL file is open.
//  
//////////////////////////////////////////////////////////////////////////////
static LONG DealFileReload (void) {                                         // SDH 10-12-04 PROMOTIONS

    //Define variables                                                      // SDH 10-12-04 PROMOTIONS
    LONG lRc;                                                               // SDH 10-12-04 PROMOTIONS
    WORD wDealNum;                                                          // SDH 10-12-04 PROMOTIONS
    WORD wCount = 9999;  //Max recs to read (protects cyclical reference)   // SDH 10-12-04 PROMOTIONS

    if (debug) disp_msg("Loading entire deal file...");                     // SDH 10-12-04 PROMOTIONS

    //Set up memory buffer to store messages                                // SDH 10-12-04 PROMOTIONS
    FreeBuffer(pDealRewdMsg);                                               // SDH 10-12-04 PROMOTIONS
    pDealRewdMsg = (BYTE*)AllocateBuffer(9999);                             // SDH 10-12-04 PROMOTIONS
    memset(pDealRewdMsg, 0x00, 9999);                                       // SDH 10-12-04 PROMOTIONS

    //Start with the home key on the deal file                              // SDH 10-12-04 PROMOTIONS
    *(UWORD*)(dealrec.abDealNumPD) = 0xffff;                                // SDH 10-12-04 PROMOTIONS
    lRc = ReadDealQuick();                                                  // SDH 10-12-04 PROMOTIONS

    //Set up next record to read                                            // SDH 10-12-04 PROMOTIONS
    if (lRc > 0) {                                                          // SDH 10-12-04 PROMOTIONS
        memcpy(dealrec.abDealNumPD, dealrec.abNextDealPD, 2);               // SDH 10-12-04 PROMOTIONS
        unpack(sbuf, 4, dealrec.abDealNumPD, 2, 0);                         // SDH 10-12-04 PROMOTIONS
        wDealNum = satoi(sbuf, 4);                                          // SDH 10-12-04 PROMOTIONS
    }                                                                       // SDH 10-12-04 PROMOTIONS

    while (lRc > 0 && *(UWORD*)(dealrec.abDealNumPD) != 0xffff) {           // SDH 10-12-04 PROMOTIONS

        //Read and handle errors                                            // SDH 10-12-04 PROMOTIONS
        lRc = ReadDealQuick();                                              // SDH 10-12-04 PROMOTIONS
        if (lRc <= 0) break;                                                // SDH 10-12-04 PROMOTIONS

        //Save away the reward message for this deal                        // SDH 10-12-04 PROMOTIONS
        if (wDealNum > 0 && wDealNum < 10000) {                             // SDH 10-12-04 PROMOTIONS
            pDealRewdMsg[wDealNum-1] = dealrec.Rewd[0].bRewdMsg;            // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS
        
        //Get the next deal to read                                         // SDH 10-12-04 PROMOTIONS
        *(UWORD*)(dealrec.abDealNumPD) = *(UWORD*)(dealrec.abNextDealPD);   // SDH 10-12-04 PROMOTIONS
        unpack(sbuf, 4, dealrec.abDealNumPD, 2, 0);                         // SDH 10-12-04 PROMOTIONS
        wDealNum = satoi(sbuf, 4);                                          // SDH 10-12-04 PROMOTIONS

        //Too many records on file, or loop in linked list!                 // SDH 10-12-04 PROMOTIONS
        if (wCount-- == 0) break;                                           // SDH 10-12-04 PROMOTIONS

    }                                                                       // SDH 10-12-04 PROMOTIONS

    //Log any error                                                         // SDH 10-12-04 PROMOTIONS
    if (lRc < 0) log_event101(lRc, DEAL_REP, __LINE__);           // SDH 10-12-04 PROMOTIONS

    if (debug) {                                                            // SDH 10-12-04 PROMOTIONS
        sprintf(sbuf, "DealFileReload end.  RC:%08lX  Num recs:%d",         // SDH 10-12-04 PROMOTIONS
                (lRc<0 ? lRc : 0), (9999 - wCount));                        // SDH 10-12-04 PROMOTIONS
        disp_msg(sbuf);                                                     // SDH 10-12-04 PROMOTIONS
    }                                                                       // SDH 10-12-04 PROMOTIONS

    //Return 0 for positive return codes                                    // SDH 10-12-04 PROMOTIONS
    return (lRc<0 ? lRc : 0);                                               // SDH 10-12-04 PROMOTIONS

}                                                                           // SDH 10-12-04 PROMOTIONS

//////////////////////////////////////////////////////////////////////////////
//
//  DealFileTrickle
//
//  Read the TDTFF file and load any changed deals
//
//////////////////////////////////////////////////////////////////////////////

void DealFileTrickle (void) {                                               // SDH 10-12-04 PROMOTIONS

    LONG lRc;
    LONG lRecords;                                                          // SDH 10-12-04 PROMOTIONS
    LONG lOffset;                                                           // SDH 10-12-04 PROMOTIONS
    UWORD *puwDealNumPD;                                                    // SDH 10-12-04 PROMOTIONS
    WORD wDealNum;                                                          // SDH 10-12-04 PROMOTIONS
    URC usrrc;                                                              // SDH 10-12-04 PROMOTIONS
    BYTE *pBuffer = NULL;                                                   // SDH 10-12-04 PROMOTIONS

    //Create a loop as a tool just so we can break from it                  // SDH 10-12-04 PROMOTIONS
    while (TRUE) {                                                          // SDH 10-12-04 PROMOTIONS

        //Open the deal trickle file                                        // SDH 10-12-04 PROMOTIONS
        usrrc = open_tdtff();                                               // SDH 10-12-04 PROMOTIONS
        if (usrrc <= RC_DATA_ERR) {                                         // SDH 10-12-04 PROMOTIONS
            close_tdtff();                                                  // SDH 10-12-04 PROMOTIONS
            break;                                                          // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS

        //Read the header record                                            // SDH 10-12-04 PROMOTIONS
        lRc = ReadTDTFFHeader(__LINE__);                                    // SDH 10-12-04 PROMOTIONS
        if (lRc != sizeof(TDTFF_HEADER)) {                                  // SDH 10-12-04 PROMOTIONS
            close_tdtff();                                                  // SDH 10-12-04 PROMOTIONS
            break;                                                          // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS

        //Calculate number of records which need to be read in              // SDH 10-12-04 PROMOTIONS
        //Break if nada to do                                               // SDH 10-12-04 PROMOTIONS
        lRecords = TdtffHeader.lPtr - lLastTDTFFLevel;                      // SDH 10-12-04 PROMOTIONS
        if (lRecords == 0) break;                                           // SDH 10-12-04 PROMOTIONS

        //Open the deal file                                                // SDH 10-12-04 PROMOTIONS
        usrrc = open_deal();                                                // SDH 10-12-04 PROMOTIONS
        if (usrrc <= RC_DATA_ERR) break;                                    // SDH 10-12-04 PROMOTIONS

        //Check file has not been reset, or wrap occured                    // SDH 10-12-04 PROMOTIONS
        if(lRecords < 0                    ||                               // SDH 10-12-04 PROMOTIONS
           lRecords >= TdtffHeader.lMaxPtr ||                               // SDH 10-12-04 PROMOTIONS
           lLastTDTFFLevel == -1) {                                         // SDH 10-12-04 PROMOTIONS
            lRc = DealFileReload();                                         // SDH 10-12-04 PROMOTIONS
            if (lRc == 0) lLastTDTFFLevel = TdtffHeader.lPtr;               // SDH 10-12-04 PROMOTIONS
            break;                                                          // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS

        //Calculate offset of next record to read from file                 // SDH 10-12-04 PROMOTIONS
        lOffset = lLastTDTFFLevel % TdtffHeader.lMaxPtr;                    // SDH 10-12-04 PROMOTIONS

        //If block would wrap back round to start of file                   // SDH 10-12-04 PROMOTIONS
        if (lOffset + lRecords > TdtffHeader.lMaxPtr) {                     // SDH 10-12-04 PROMOTIONS
            //Reduce size of block to fit                                   // SDH 10-12-04 PROMOTIONS
            lRecords = TdtffHeader.lMaxPtr - lOffset;                       // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS

        //Convert offset to offset into file                                // SDH 10-12-04 PROMOTIONS
        lOffset = lOffset*sizeof(TDTFF_REC) + sizeof(TDTFF_HEADER);         // SDH 10-12-04 PROMOTIONS
        lRecords *= sizeof(TDTFF_REC);                                      // SDH 10-12-04 PROMOTIONS

        //Allocate buffer space for file records                            // SDH 10-12-04 PROMOTIONS
        pBuffer = AllocateBuffer(lRecords);                                 // SDH 10-12-04 PROMOTIONS

        //Read updated record block from file                               // SDH 10-12-04 PROMOTIONS
        lRc = s_read (A_BOFOFF, tdtff.fnum, pBuffer,                        // SDH 10-12-04 PROMOTIONS
                      lRecords, lOffset);                                   // SDH 10-12-04 PROMOTIONS
        if (lRc != lRecords) {                                              // SDH 10-12-04 PROMOTIONS
            close_tdtff();                                                  // SDH 10-12-04 PROMOTIONS
            break;                                                          // SDH 10-12-04 PROMOTIONS
        }                                                                   // SDH 10-12-04 PROMOTIONS

        //Initialise deal num pointer                                       // SDH 10-12-04 PROMOTIONS
        puwDealNumPD = (UWORD*)pBuffer;                                     // SDH 10-12-04 PROMOTIONS

        //lRecords currently holds the size of the read buffer, so divide   // SDH 10-12-04 PROMOTIONS
        //by the record length to obtain the number of records              // SDH 10-12-04 PROMOTIONS
        lRecords = (ULONG)lRecords / sizeof(TDTFF_REC);                     // SDH 10-12-04 PROMOTIONS

        do {                                                                // SDH 10-12-04 PROMOTIONS

            //Copy packed deal number into DEAL buffer                      // SDH 10-12-04 PROMOTIONS
            //Read the required deal file record                            // SDH 10-12-04 PROMOTIONS
            *(UWORD*)(dealrec.abDealNumPD) = *puwDealNumPD;                 // SDH 10-12-04 PROMOTIONS
            lRc = ReadDealQuick();                                          // SDH 10-12-04 PROMOTIONS
            
            //If good return code then make it zero                         // SDH 10-12-04 PROMOTIONS
            if (lRc > 0) lRc = 0;                                           // SDH 10-12-04 PROMOTIONS

            //Get the deal number as an int                                 // SDH 10-12-04 PROMOTIONS
            unpack(sbuf, 4, dealrec.abDealNumPD, 2, 0);                     // SDH 10-12-04 PROMOTIONS
            wDealNum = satoi(sbuf, 4);                                      // SDH 10-12-04 PROMOTIONS
            
            switch (lRc) {                                                  // SDH 10-12-04 PROMOTIONS

            //Deal record read successfully                                 // SDH 10-12-04 PROMOTIONS
            case 0:                                                         // SDH 10-12-04 PROMOTIONS
                //Save away the reward message for this deal                // SDH 10-12-04 PROMOTIONS
                pDealRewdMsg[wDealNum-1] = dealrec.Rewd[0].bRewdMsg;        // SDH 10-12-04 PROMOTIONS
                lRecords--;                                                 // SDH 10-12-04 PROMOTIONS
                lLastTDTFFLevel++;                                          // SDH 10-12-04 PROMOTIONS
                break;                                                      // SDH 10-12-04 PROMOTIONS

            //Error on read (record deleted)                                // SDH 10-12-04 PROMOTIONS
            case 0x80f306c8:                                                // SDH 10-12-04 PROMOTIONS
                //Set the deal descriptor number to 0                       // SDH 10-12-04 PROMOTIONS
                pDealRewdMsg[wDealNum-1] = dealrec.Rewd[0].bRewdMsg;        // SDH 10-12-04 PROMOTIONS
                lRc = 0;                                                    // SDH 10-12-04 PROMOTIONS
                lRecords--;                                                 // SDH 10-12-04 PROMOTIONS
                lLastTDTFFLevel++;                                          // SDH 10-12-04 PROMOTIONS
                break;                                                      // SDH 10-12-04 PROMOTIONS
            }                                                               // SDH 10-12-04 PROMOTIONS
            
            //Point to the next record in the read buffer                   // SDH 10-12-04 PROMOTIONS
            puwDealNumPD++;                                                 // SDH 10-12-04 PROMOTIONS

        } while (lRecords != 0 && lRc == 0);                                // SDH 10-12-04 PROMOTIONS

        lRc = 0;                                                            // SDH 10-12-04 PROMOTIONS

        //We never actually loop here, its just a tool to break from        // SDH 10-12-04 PROMOTIONS
        break;                                                              // SDH 10-12-04 PROMOTIONS

    }                                                                       // SDH 10-12-04 PROMOTIONS

    //Close deal file                                                       // SDH 10-12-04 PROMOTIONS
    close_deal();                                                           // SDH 10-12-04 PROMOTIONS

    //Release TDTFF read buffer                                             // SDH 10-12-04 PROMOTIONS
    FreeBuffer(pBuffer);                                                    // SDH 10-12-04 PROMOTIONS
    pBuffer = NULL;                                                         // SDH 10-12-04 PROMOTIONS

}                                                                           // SDH 10-12-04 PROMOTIONS

// ==============================================================  // 24-05-07 PAB Recalls
// moved transactions from TRANSACT.C as 64K limit reached         // 24-05-07 PAB Recalls
// =============================================================== // 24-05-07 PAB Recalls

void suspend_transaction(char *inbound) {

       int rc = 0;                                                     // PAB 24-5-2007 
       URC usrrc = RC_OK;                                              // PAB 24-5-2007
       LONG rc2 = 0;                                                   // PAB 24-5-2007
       static SUSPT_REC susptrec;                                      // PAB 24-5-2007
       static LONG day, month, year;                                   // PAB 24-5-2007
       static WORD hour, min;                                          // PAB 24-5-2007
       static LONG sec;                                                // PAB 24-5-2007

       if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
       if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
       UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

       // if there are no barcodes in the transaction then launch      // 5-5-04 PAB
       // the report and ack the HHT                                   // 5-5-04 PAB
       // 5-5-04 PAB
       //if (strlen( ((LRT_SUS *)(inbound))->sus_items )==0) {         // 5-5-04 PAB
       if (((LRT_SUS*)(inbound))->sus_items[0] == 0) {                 // 19-05-05 SDH
           //disp_msg("Start Suspended Transaction Report");           // 5-5-04 PAB
           rc = start_background_app( "ADX_UPGM:RFSUSRPT.286", "",     // 01-07-05 SDH
                                 "On Demand - Suspended Txn Report" ); // 5-5-04 PAB
           if (rc < 0) {
               prep_nak("Unable to start RFSUSRPT. Please try again "  // SDH 27-Sep-2006 Bug fix
                        "shortly.");                                   // SDH 27-Sep-2006 Bug fix
           } else {                                                    // SDH 27-Sep-2006 Bug fix
               prep_ack("");                                           // SDH 23-Aug-2006 Planners
           }                                                           // SDH 27-Sep-2006 Bug fix

           return;                                                     // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       // 5-5-04 PAB
       // Get RF attributes from control file                          // 5-5-04 PAB
       usrrc = open_rfscf();                                           // 5-5-04 PAB
       if (usrrc!=RC_OK) {                                             // 5-5-04 PAB
           prep_nak("ERRORUnable to open RFSCF file. "                 // 5-5-04 PAB
                     "Please phone help desk.");                       // 5-5-04 PAB
           return;                                                     // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       rc2 = s_read( A_BOFOFF, rfscf.fnum,                             // 5-5-04 PAB
                     (void *)&rfscfrec1and2, RFSCF_RECL, 0L );         // 16-11-04 SDH
       if (rc2<=0L) {                                                  // 5-5-04 PAB
           log_event101(rc2, RFSCF_REP, __LINE__);                     // 5-5-04 PAB
           //sprintf(msg, "Err-R RFSCF. RC:%08lX", rc2);               // 5-5-04 PAB
           //disp_msg(msg);                                            // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       rfscfrec1and2.qbust_next_txn++;                                 // 16-11-04 SDH
       rfscfrec1and2.qbust_txn_cnt++;                                  // 16-11-04 SDH

       rc2 = UpdateRfscf(__LINE__);                                    // 03-01-05 SDH
       if (rc2<=0L) {                                                  // 03-01-05 SDH
           prep_nak("ERRORUnable to "                                  // SDH 23-Aug-2006 Planners
                     "write to RFSCF. "                                // 03-01-05 SDH
                     "Check appl event logs" );                        // 03-01-05 SDH
           return;                                                      // 03-01-05 SDH
       }                                                               // 03-01-05 SDH

       // 5-5-04 PAB
       usrrc = close_rfscf( CL_SESSION );                              // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       usrrc = open_suspt();                                           // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       if (usrrc!=RC_OK) {                                             // 5-5-04 PAB
           log_event101(rc2, SUSPT_REP, __LINE__);                     // 5-5-04 PAB
           prep_nak("ERRORUnable to open "                             // SDH 23-Aug-2006 Planners
                     "EALSUSPT file. "                                 // 5-5-04 PAB
                     "Check appl event logs" );                        // 5-5-04 PAB
           return;                                                      // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       if (usrrc == RC_OK) {                                           // 5-5-04 PAB
           sysdate( &day, &month, &year, &hour, &min, &sec );          // 5-5-04 PAB
           year = (year - 2000);                                       // 5-5-04 PAB
           sprintf( sbuf, "%02ld%02ld%02ld%02d%02d%02d",               // 5-5-04 PAB
                    day, month, year, hour, min, sec );                // 5-5-04 PAB
           // pack 0 is even length                                    // 5-5-04 PAB
           pack( susptrec.day_time, 6, sbuf, 12, 0);                   // 5-5-04 PAB
           sprintf( sbuf, "%03d", rfscfrec1and2.qbust_term );          // 16-11-04 SDH
           // pack 1 is odd length and fill with zeros                 // 5-5-04 PAB
           pack(susptrec.terminal, 2, sbuf, 4, 1);                     // 5-5-04 PAB
           sprintf( sbuf, "%04d", rfscfrec1and2.qbust_next_txn );      // 16-11-04 SDH
           pack(susptrec.transnum, 2, sbuf, 4, 0);                     // 5-5-04 PAB
           susptrec.sequence = 0;                                      // 5-5-04 PAB
           memcpy(susptrec.operator_id,                                // 5-5-04 PAB
                  ((LRT_SUS *)(inbound))->opid,3);                     // 5-5-04 PAB
           memcpy(susptrec.reason, "9" ,1);                            // 5-5-04 PAB
           memcpy(susptrec.EAN_data,                                   // 5-5-04 PAB
                  ((LRT_SUS *)(inbound))->sus_items, 481);             // 5-5-04 PAB
                                                                       // 5-5-04 PAB
           rc2 = s_write( 0, suspt.fnum,                               // 5-5-04 PAB
                          (void *)&susptrec, SUSPT_RECL, 0L );         // 5-5-04 PAB
           if (rc2<=0L) {                                              // 5-5-04 PAB
               if (debug) {                                            // 5-5-04 PAB
                   sprintf(msg, "Err-W to SUSPT. RC:%08lX", rc2);      // 5-5-04 PAB
                   disp_msg(msg);                                      // 5-5-04 PAB
               }                                                       // 5-5-04 PAB
               prep_nak("W-Unable to Suspended "                       //SDH 23-Aug-2006 Planners
                         "Transaction. Process "                       // 5-5-04 PAB
                         "Customer manually.   " );                    // 5-5-04 PAB
               memcpy(((LRTLG_SUS *)dtls)->fail, "Y", 1);              // 5-5-04 PAB
               memcpy(((LRTLG_SUS *)dtls)->txnid, sbuf ,4);            // 5-5-04 PAB
               lrt_log(LOG_SUS, hh_unit, dtls );                       // 5-5-04 PAB
               return;                                                  // 5-5-04 PAB
                                                                       // 5-5-04 PAB
           } else {                                                    // 5-5-04 PAB
               if (debug) {                                            // 5-5-04 PAB
                   disp_msg("WRITE EALSUSPT OK");                      // 5-5-04 PAB
               }                                                       // 5-5-04 PAB
           }                                                           // 5-5-04 PAB
           // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       usrrc = close_suspt( CL_SESSION );                              // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       if (usrrc == RC_OK) {                                           // 5-5-04 PAB
           memcpy( lrtp[hh_unit]->user,                                // 5-5-04 PAB
                   ((LRT_SUS *)(inbound))->opid, 3 );                  // 5-5-04 PAB
           // Get time                                                 // 5-5-04 PAB
           sysdate( &day, &month, &year, &hour, &min, &sec );          // 5-5-04 PAB  
           // Prepare SAK                                              // 5-5-04 PAB
           memcpy( ((LRT_SAK *)out)->cmd, "SAK", 3 );                  // 5-5-04 PAB
           sprintf( sbuf, "%03d", rfscfrec1and2.qbust_term );          // 16-11-04 SDH
           memcpy( ((LRT_SAK *)out)->till_id, sbuf , 3 );              // 5-5-04 PAB
           sprintf( sbuf, "%04d", rfscfrec1and2.qbust_next_txn );      // 16-11-04 SDH
           memcpy( ((LRT_SAK *)&out)->txn_num, sbuf, 4 );              // 5-5-04 PAB
           out_lth = LRT_SAK_LTH;                                      // 5-5-04 PAB
           // Audit                                                    // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->fail, "N", 1);                 // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->txnid, sbuf ,4);               // 5-5-04 PAB
           lrt_log( LOG_SUS, hh_unit, dtls );                          // 5-5-04 PAB
       } else {                                                        // 5-5-04 PAB
           // User not authorised                                      // 5-5-04 PAB
           prep_nak("Unable to Suspended "                             //SDH 23-Aug-2006 Planners
                     "Transaction. Process "                           // 5-5-04 PAB
                     "Customer manually. " );                          // 5-5-04 PAB
           // Audit                                                    // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->fail, "Y", 1);                 // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->txnid, sbuf ,4);               // 5-5-04 PAB
           lrt_log( LOG_SUS, hh_unit, dtls );                          // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB

}


// =============================================================// 24-05-07 PAB Recalls
// Recall Procedures follow          24th May 2007   Paul Bowers// 24-05-07 PAB Recalls
// =============================================================// 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
void RecallStart(char *inbound) {                               // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls        
                                                                // 24-05-07 PAB Recalls
    // check status of recall files                             // 24-05-07 PAB Recalls
   usrrc = CheckRecallAvailable();                              // 24-05-07 PAB Recalls
   if (usrrc!=0) {                                              // 24-05-07 PAB Recalls
       return;                                                  // 24-05-07 PAB Recalls
   }                                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls 
   UNUSED(inbound);                                             // 24-05-07 PAB Recalls
   // Open the recall files                                     // 24-05-07 PAB Recalls
   usrrc = open_recall();                                       // 24-05-07 PAB Recalls
   if (usrrc != RC_OK) {                                        // 24-05-07 PAB Recalls
       disp_msg("RECALL file not found");                       // 24-05-07 PAB Recalls
//     prep_nak("ErrorUnable to open RECALL file. "             // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG 
//              "Check appl event logs" );                      // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG
       prep_nak("There are no Head Office recalls "                                     // 07-09-2007 5.3 BMG 
                "to be actioned at this time." );                                       // 07-09-2007 5.3 BMG
       return;                                                  // 24-05-07 PAB Recalls
       }                                                        // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
   usrrc = open_rcspi();                                        // 24-05-07 PAB Recalls
   if (usrrc != RC_OK) {                                        // 24-05-07 PAB Recalls
       disp_msg("RCSPI file not found");                        // 24-05-07 PAB Recalls
//     prep_nak("ErrorUnable to open RCSPI file. "              // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG 
//              "Check appl event logs" );                      // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG
       prep_nak("There are no Head Office recalls "                                     // 07-09-2007 5.3 BMG 
                "to be actioned at this time." );                                       // 07-09-2007 5.3 BMG
       return;                                                  // 24-05-07 PAB Recalls
       }                                                        // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
   usrrc = open_rcindx();                                       // 24-05-07 PAB Recalls
   if (usrrc != RC_OK) {                                        // 24-05-07 PAB Recalls
       disp_msg("RCINDX file not found");                       // 24-05-07 PAB Recalls
//     prep_nak("ErrorUnable to open RCINDX file. "             // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG 
//              "Check appl event logs" );                      // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG
       prep_nak("There are no Head Office recalls "                                     // 07-09-2007 5.3 BMG 
                "to be actioned at this time." );                                       // 07-09-2007 5.3 BMG
       return;                                                  // 24-05-07 PAB Recalls
       }                                                        // 24-05-07 PAB Recalls

   // Read the first record and return an error if no record found                      // 07-09-2007 5.3 BMG
   usrrc = s_read( A_BOFOFF,                                                            // 07-09-2007 5.3 BMG
                rcindx.fnum, (void *)&rcindxrec,                                        // 07-09-2007 5.3 BMG
                RCINDX_RECL, 0);                                                        // 07-09-2007 5.3 BMG
   if (usrrc <= RC_OK) {                                                                // 07-09-2007 5.3 BMG
       disp_msg("RCINDX is empty");                                                     // 07-09-2007 5.3 BMG
       prep_nak("There are no Head Office recalls "                                     // 07-09-2007 5.3 BMG
                "to be actioned at this time." );                                       // 07-09-2007 5.3 BMG
       return;                                                                          // 07-09-2007 5.3 BMG
   }                                                                                    // 07-09-2007 5.3 BMG


   if (open_cchist() != RC_OK) {
//     prep_nak("errorUnable to open "                          // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG 
//              "CCHIST file. "                                 // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG
//              "Check appl event logs" );                      // 24-05-07 PAB Recalls // 07-09-2007 5.3 BMG
       prep_nak("There are no Head Office recalls "                                     // 07-09-2007 5.3 BMG 
                "to be actioned at this time." );                                       // 07-09-2007 5.3 BMG
       return;                                                  // 24-05-07 PAB Recalls
       }                                                                        
   
                                                                // 24-05-07 PAB Recalls
// Recall files available ok send acknowledgement               // 24-05-07 PAB Recalls
   prep_ack("");                                                // 24-05-07 PAB Recalls
}                                                               // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
void RecallExit(char *inbound) {                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    LRT_RCB* pRCB = (LRT_RCB*)inbound; 
    
    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls
    BYTE RecallNumber[23];                                      // 24-05-07 PAB Recalls
    UNUSED(inbound);                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    // check status of recall files                             // 24-05-07 PAB Recalls
   usrrc = CheckRecallAvailable();                              // 24-05-07 PAB Recalls
   if (usrrc!=0) {                                              // 24-05-07 PAB Recalls
       return;                                                  // 24-05-07 PAB Recalls
   }                                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
   if (pRCB->cListStatus[0] != 0x58){                           // 20-06-07 PAB Recalls
      // if not "X" so it must be "P" or "A"
      // then status is "P" or "A" then update and despatch this recall
      // lift the first 8 digits asa this is the recall ref no
      // the next 14 are the UOD number to send to RFSCC
      memcpy( recallrec.abRecallReference,                      // 24-05-07 PAB Recalls
             pRCB->anRecallNumber, 8 );                         // 24-05-07 PAB Recalls
      recallrec.ubChain = 0;                                    // 24-05-07 PAB Recalls
      usrrc = s_read( 0, recall.fnum, (void *)&recallrec,       // 24-05-07 PAB Recalls
                              RECALL_RECL, RECALL_KEYL );       // 24-05-07 PAB Recalls
      //If error reading RECALL                                 // 24-05-07 PAB Recalls
      if (usrrc<=0L) {                                          // 24-05-07 PAB Recalls
          log_event101(usrrc, RECALL_REP, __LINE__);            // 24-05-07 PAB Recalls       
          if (debug) {                                          // 24-05-07 PAB Recalls
              sprintf(msg, "Err-R RECALL. RC:%08lX", usrrc);    // 24-05-07 PAB Recalls
              disp_msg(msg);                                    // 24-05-07 PAB Recalls
              }                                                 // 24-05-07 PAB Recalls
           prep_nak("ERRORThis Recall could not be read from "  // 24-05-07 PAB Recalls
                    "Recall file. Check appl event logs" );     // 24-05-07 PAB Recalls
           return;                                              // 24-05-07 PAB Recalls
          //Else there is a RECALL record                       // 24-05-07 PAB Recalls
      } else {                                                  // 24-05-07 PAB Recalls
          recall.present=TRUE;                                  // 24-05-07 PAB Recalls
          if (debug) {                                          // 24-05-07 PAB Recalls
              sprintf(msg, "OK, REC :");                        // 24-05-07 PAB Recalls
              disp_msg(msg);                                    // 24-05-07 PAB Recalls
              dump( (BYTE *)&recallrec, RECALL_RECL );          // 24-05-07 PAB Recalls
          }                                                     // 24-05-07 PAB Recalls
          // update the recall record                           // 24-05-07 PAB Recalls
          memcpy( recallrec.cRecallStatus,                      // 24-05-07 PAB Recalls
              pRCB->cListStatus, 1 );                           // 24-05-07 PAB Recalls
          usrrc = WriteRecall(__LINE__);                        // 24-05-07 PAB Recalls              
          if (usrrc <= 0L) {                                    // 24-05-07 PAB Recalls
             // there was an error writing back the recall record.
              log_event101(usrrc, RECALL_REP, __LINE__);        // 24-05-07 PAB Recalls
              if (debug) {                                      // 24-05-07 PAB Recalls
                  sprintf(msg, "Err-W RECALL. RC:%08lX", usrrc);// 24-05-07 PAB Recalls
                  disp_msg(msg);                                // 24-05-07 PAB Recalls
              }                                                 // 24-05-07 PAB Recalls
              prep_nak("ERRORThere was a failure updating the"  // 24-05-07 PAB Recalls
                        "Recall file. Check appl event logs" ); // 24-05-07 PAB Recalls
              return;                                           // 24-05-07 PAB Recalls  
              }                                                 // 24-05-07 PAB Recalls
          // The recall record updated ok                       // 24-05-07 PAB Recalls
          memcpy( RecallNumber, pRCB->anRecallNumber, 22);      // 24-05-07 PAB Recalls
          RecallNumber[22] = 0x00;                              // 24-05-07 PAB Recalls
          usrrc = start_background_app( "ADX_UPGM:RFSCC.286",   // 24-05-07 PAB Recalls
                                        RecallNumber,           // 24-05-07 PAB Recalls
                                        "Despatching Recall" ); // 24-05-07 PAB Recalls
          if (usrrc < 0) {                                      // 24-05-07 PAB Recalls
              prep_nak("ERRORUnable to start RFSCC to despatch" // 24-05-07 PAB Recalls
                       " this Recall. Please try again ");      // 24-05-07 PAB Recalls                               
          }                                                     // 24-05-07 PAB Recalls
      }                                                         // 24-05-07 PAB Recalls
   }                                                            // 24-05-07 PAB Recalls
   if (pRCB->cListStatus[0] != 'P'){                            // 21-06-07 PAB recalls
      // Only close the files if finished the UOD, if Partial then
      // Multiple UODs can be requested for the same Recall Ref.
      close_recall( CL_SESSION );                               // 24-05-07 PAB Recalls
      close_rcindx( CL_ALL );                                   // 24-05-07 PAB Recalls  // 07-09-2007 5.4 BMG
      close_rcspi( CL_ALL );                                    // 24-05-07 PAB Recalls  // 07-09-2007 5.4 BMG
      close_cchist ( CL_SESSION );                              // 4-6-07  PAB Recalls
   }                                                            // 21-6-07 PAB Recalls
   prep_ack("");                                                // 24-05-07 PAB Recalls
   return;                                                      // 24-05-07 PAB Recalls
}                                                               // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
void RecallListRequest(char *inbound) {                         // 24-05-07 PAB Recalls
                                                                // 29-05-07 PAB Recalls
    LRT_RCD* pRCD = (LRT_RCD*)inbound;                          // 31-05-07 PAB Recalls
    LRT_RCE* pRCE = (LRT_RCE*)out;                              // 31-05-07 PAB Recalls
    LRT_RCC* pRCC = (LRT_RCC*)out;                              // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls
    LONG lIndexPtr = 0;                                         // 29-05-07 PAB Recalls
    int iExpired = 0;                                                                   // 14-04-2008 5.6 BMG
    int iBypass = 0;                                                                    // 30-04-2008 5.7 BMG
    DOUBLE dRecallExpiryDate;                                                           // 14-04-2008 5.6 BMG
    DOUBLE dTodayDate;                                                                  // 14-04-2008 5.6 BMG
    WORD hour, min;                                                                     // 14-04-2008 5.6 BMG
    LONG sec, day, month, year;                                                         // 14-04-2008 5.6 BMG
    UNUSED(inbound);                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    // check status of recall files                             // 24-05-07 PAB Recalls
    usrrc = CheckRecallAvailable();                             // 24-05-07 PAB Recalls
    if (usrrc!=0) {                                             // 24-05-07 PAB Recalls
        return;                                                 // 24-05-07 PAB Recalls
    }                                                           // 24-05-07 PAB Recalls
                                                                // 29-05-07 PAB Recalls
                                                                // 29-05-07 PAB Recalls
    lIndexPtr = satoi(pRCD->anIndex, 4);                        // 29-05-07 PAB Recalls
    lIndexPtr = lIndexPtr * RCINDX_RECL;                        // 30-05-07 PAB Recalls // 14-04-2008 5.6 BMG
                                                                // 29-05-07 PAB Recalls
    // read the current status record & close the file          // 24-05-07 PAB Recalls      
    usrrc = s_read( A_BOFOFF,                                   // 24-05-07 PAB Recalls     
                 rcindx.fnum, (void *)&rcindxrec,               // 29-05-07 PAB Recalls
                 RCINDX_RECL, lIndexPtr);                       // 24-05-07 PAB Recalls 
                                                                // 29-05-07 PAB Recalls
                                                                // 29-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    if (debug) {                                              
       sprintf(msg, "Read from RCINDX. RC:%08lX recall status %c recall type %c record index %d", usrrc,   // 30-04-2008 5.7 BMG  
                       *rcindxrec.cRecallStatus,*rcindxrec.cRecallType,lIndexPtr); // 31-05-07 PAB Recalls // 30-04-2008 5.7 BMG
       disp_msg(msg);                                           // 31-05-07 PAB Recalls
       }                                                        // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls

    if ( ((memcmp(rcindxrec.cRecallType,"C",1) == 0) ||                                 // 14-04-2008 5.6 BMG
          (memcmp(rcindxrec.cRecallType,"I",1) == 0)) && usrrc > RC_OK) {               // 14-04-2008 5.6 BMG
        // Recall is a type C or I so check expiry date                                 // 14-04-2008 5.6 BMG
        sysdate( &day, &month, &year, &hour, &min, &sec );                              // 14-04-2008 5.6 BMG
        dTodayDate = ConvGJ( day, month, year );                                        // 14-04-2008 5.6 BMG
        unpack( sbuf, 8, rcindxrec.abExpiryDate, 4, 0 );                                // 14-04-2008 5.6 BMG
        day   = satol( sbuf+6, 2 );                                                     // 14-04-2008 5.6 BMG
        month = satol( sbuf+4, 2 );                                                     // 14-04-2008 5.6 BMG
        year  = satol( sbuf,   4 );                                                     // 14-04-2008 5.6 BMG
        dRecallExpiryDate = ConvGJ( day, month, year );                                 // 14-04-2008 5.6 BMG
        if ((dRecallExpiryDate - dTodayDate) >= 0) {                                    // 14-04-2008 5.6 BMG // 20-06-2008 5.8 BMG
            iExpired = 0;                                                               // 14-04-2008 5.6 BMG
        } else iExpired = 1;                                                            // 14-04-2008 5.6 BMG
    }                                                                                   // 14-04-2008 5.6 BMG

    /*If head office recall requested but we have an I or C, bypass this record*/       // 30-04-2008 5.7 BMG
    if ( memcmp(pRCD->cRecallType,"*",1) == 0 ) {                                       // 30-04-2008 5.7 BMG
        if ( (memcmp(rcindxrec.cRecallType,"C",1) == 0) ||                              // 30-04-2008 5.7 BMG
             (memcmp(rcindxrec.cRecallType,"I",1) == 0)) {                              // 30-04-2008 5.7 BMG
            iBypass = 1;                                                                // 30-04-2008 5.7 BMG
        }                                                                               // 30-04-2008 5.7 BMG
    } else { /*An I or C was requested to bypass of we don't have a match */            // 30-04-2008 5.7 BMG
        if ( memcmp(pRCD->cRecallType,rcindxrec.cRecallType,1) ) {                      // 30-04-2008 5.7 BMG
            iBypass = 1;                                                                // 30-04-2008 5.7 BMG
        }                                                                               // 30-04-2008 5.7 BMG
    }                                                                                   // 30-04-2008 5.7 BMG
    
    if (((memcmp(rcindxrec.cRecallStatus,"A",1) == 0) ||        // 03-07-07 PAB Recalls // 14-04-2008 5.6 BMG
        iExpired || iBypass) && (usrrc > RC_OK)) {              // 29-05-07 PAB recalls // 14-04-2008 5.6 BMG // 30-04-2008 5.7 BMG
       // this recall has been processed or is expired (types I & C) so look for the next unprocessed
        while ((memcmp(rcindxrec.cRecallStatus,"A",1 ) == 0 ||  // 03-07-07 PAB Recalls // 14-04-2008 5.6 BMG
               iExpired || iBypass) && (usrrc > RC_OK)) {                               // 14-04-2008 5.6 BMG // 30-04-2008 5.7 BMG
           lIndexPtr = lIndexPtr + RCINDX_RECL; 
           usrrc = s_read( A_BOFOFF,                            // 24-05-07 PAB Recalls     
                rcindx.fnum, (void *)&rcindxrec,                // 29-05-07 PAB Recalls
                RCINDX_RECL, lIndexPtr);                        // 24-05-07 PAB Recalls 
                                                                
            if ( ((memcmp(rcindxrec.cRecallType,"C",1) == 0) ||                          // 14-04-2008 5.6 BMG
                  (memcmp(rcindxrec.cRecallType,"I",1) == 0)) && usrrc > RC_OK) {        // 14-04-2008 5.6 BMG
               // Recall is a type C or I so check expiry date                           // 14-04-2008 5.6 BMG
               sysdate( &day, &month, &year, &hour, &min, &sec );                        // 14-04-2008 5.6 BMG
               dTodayDate = ConvGJ( day, month, year );                                  // 14-04-2008 5.6 BMG
               unpack( sbuf, 8, rcindxrec.abExpiryDate, 4, 0 );                          // 14-04-2008 5.6 BMG
               day   = satol( sbuf+6, 2 );                                               // 14-04-2008 5.6 BMG
               month = satol( sbuf+4, 2 );                                               // 14-04-2008 5.6 BMG
               year  = satol( sbuf,   4 );                                               // 14-04-2008 5.6 BMG
               dRecallExpiryDate = ConvGJ( day, month, year );                           // 14-04-2008 5.6 BMG
               if ((dRecallExpiryDate - dTodayDate) >= 0) {                              // 14-04-2008 5.6 BMG // 20-06-2008 5.8 BMG
                  iExpired = 0;                                                          // 14-04-2008 5.6 BMG
               } else iExpired = 1;                                                      // 14-04-2008 5.6 BMG
            } else iExpired = 0;                                                         // 14-04-2008 5.6 BMG

            /*If head office recall requested but we have an I or C, bypass this record*/// 30-04-2008 5.7 BMG
            iBypass = 0;                                                                 // 30-04-2008 5.7 BMG
            if ( memcmp(pRCD->cRecallType,"*",1) == 0 ) {                                // 30-04-2008 5.7 BMG
                if ( (memcmp(rcindxrec.cRecallType,"C",1) == 0) ||                       // 30-04-2008 5.7 BMG
                     (memcmp(rcindxrec.cRecallType,"I",1) == 0)) {                       // 30-04-2008 5.7 BMG
                    iBypass = 1;                                                         // 30-04-2008 5.7 BMG
                }                                                                        // 30-04-2008 5.7 BMG
            } else { /*An I or C was requested to bypass of we don't have a match */     // 30-04-2008 5.7 BMG
                if ( memcmp(pRCD->cRecallType,rcindxrec.cRecallType,1) ) {               // 30-04-2008 5.7 BMG
                    iBypass = 1;                                                         // 30-04-2008 5.7 BMG
                }                                                                        // 30-04-2008 5.7 BMG
            }                                                                            // 30-04-2008 5.7 BMG

            if (debug) {
                if (usrrc < RC_OK) {
                    // end of index file reached                          
                    sprintf(msg, "Search RCINDX. RC:%08lX record index %d EOF RCINDX", usrrc,lIndexPtr); // 30-04-2008 5.7 BMG
                    disp_msg(msg);                                                                       // 30-04-2008 5.7 BMG
                    break;
                } else {
                    sprintf(msg, "Search RCINDX. RC:%08lX recall status %c recall type %c record index %d", usrrc, // 14-04-2008 5.6 BMG
                           *rcindxrec.cRecallStatus,*rcindxrec.cRecallType,lIndexPtr);                             // 14-04-2008 5.6 BMG
                    disp_msg(msg);                                   // 31-05-07 PAB Recalls  
                    if (iExpired) {                                                          // 14-04-2008 5.6 BMG
                        sprintf(msg, "               RECALL IS EXPIRED");                    // 14-04-2008 5.6 BMG
                        disp_msg(msg);                                                       // 14-04-2008 5.6 BMG
                    }
                    if (iBypass) {                                                           // 30-04-2008 5.7 BMG
                        sprintf(msg, "               RECALL IS WRONG TYPE");                 // 30-04-2008 5.7 BMG
                        disp_msg(msg);                                                       // 30-04-2008 5.7 BMG
                    }
                }
            }                                                    // 31-05-07 PAB Recalls
        }
     }

     if (usrrc < RC_OK) {
        // we reached end of index file without finding an unprocessed record
        memcpy(pRCE->cmd, "RCE", sizeof(pRCE->cmd)); 
        memcpy(pRCE->opid, pRCD->opid, sizeof(pRCE->opid)); 
        out_lth = LRT_RCE_LTH;  
        return;
     }
     
     // prepare RCC response

     memcpy(pRCC->cmd,"RCC", sizeof(pRCC->cmd)); 
     memcpy(pRCC->opid, pRCD->opid, sizeof(pRCC->opid)); 
     lIndexPtr = (lIndexPtr / 40);
     LONG_TO_ARRAY(pRCC->Index, lIndexPtr);
     unpack(pRCC->anRecallRef, 8, rcindxrec.abRecallReference, 4, 0); 
     memcpy(pRCC->cRecallType,rcindxrec.cRecallType,sizeof(pRCC->cRecallType)); 
     memcpy(pRCC->abRecallDesc,rcindxrec.abRecallDesc,sizeof(pRCC->abRecallDesc));       
     unpack(pRCC->anRecallCnt,4,rcindxrec.abItemCount,2,0);  
     /* If we have a type I or type C, write the due by date into the active */                          // 30-04-2008 5.7 BMG
     /* date field to make PPC processing easier. */                                                     // 30-04-2008 5.7 BMG
     if ( ((memcmp(rcindxrec.cRecallType,"C",1) == 0) || (memcmp(rcindxrec.cRecallType,"I",1) == 0)) ) { // 30-04-2008 5.7 BMG
         unpack(pRCC->cActiveDate,8,rcindxrec.abExpiryDate,4,0);                                         // 30-04-2008 5.7 BMG
     } else unpack(pRCC->cActiveDate,8,rcindxrec.abActiveDate,4,0);                                      // 30-04-2008 5.7 BMG
     memcpy(pRCC->cMsgAvail,rcindxrec.cSpeicalIns,sizeof(pRCC->cMsgAvail));   
     memcpy(pRCC->cLabelType, rcindxrec.abLabelType, 2);
     memcpy(pRCC->cMRQ, rcindxrec.cMRQ, 2);                                                              // 30-04-2008 5.7 BMG

     out_lth = LRT_RCC_LTH;
     return;
}


void RecallCount(char *inbound) {                               // 24-05-07 PAB Recalls

    LRT_RCG* pRCG = (LRT_RCG*)inbound;                          // 31-05-07 PAB Recalls

    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls
    LONG lItemArrayptr = 0;                                     // 31-05-07 PAB Recalls
    LONG lRecallCount = 0;                                      // 31-05-07 PAB Recalls
    //LONG lNewCount = 0;                                       // 31-05-07 PAB recalls
    BYTE anRecallCount[4];                                      // 20-06-07 PAB recalls
    BYTE uncounted[2]; 
    memset(uncounted, 0xFF, 2);                                                          // 30-08-2007 5.2 BMG
    UNUSED(inbound);                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    // check status of recall files                             // 24-05-07 PAB Recalls
    usrrc = CheckRecallAvailable();                             // 24-05-07 PAB Recalls
    if (usrrc!=0) {                                             // 24-05-07 PAB Recalls
        return;                                                 // 24-05-07 PAB Recalls
    }                                                           // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    memcpy( recallrec.abRecallReference,                        // 24-05-07 PAB Recalls
            pRCG->anRecallRef, 8 );                             // 24-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    recallrec.ubChain = 0;                                      // 31-05-07 PAB Recalls
    // read the first chain  and look for the item to update    // 31-05-07 PAB Recalls
    while (usrrc >= 0) {
       usrrc = s_read( 0, recall.fnum, (void *)&recallrec,      // 24-05-07 PAB Recalls
                         RECALL_RECL, RECALL_KEYL );            // 24-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
       if (usrrc < RC_OK) {                                     // 24-05-07 PAB Recalls
          disp_msg("RD RECALL failed");                         // 24-05-07 PAB Recalls
          prep_nak("ERRORUThis item could not be found "        // 24-05-07 PAB Recalls
                   "on this Recall");                           // 24-05-07 PAB Recalls
          return;                                               // 24-05-07 PAB Recalls
       } 

       lItemArrayptr = 0;                                       // 31-05-07 PAB Recalls
       // look for the item on this recall record   
       pack(sbuf, 3, pRCG->anRecallItem, 6, 0);                 // 31-05-07 PAB Recalls
       
       if (debug) {                                               
          disp_msg("Update Recall Item");
          dump(sbuf,3);
       } 

       while (lItemArrayptr < 50) {                                                                     // 31-05-07 PAB Recalls
          if (memcmp(sbuf, recallrec.aRecallItems[lItemArrayptr].abItemCode, 3 )==0) {                  // 31-05-07 PAB Recalls
              // the item is found on the current recall record
              // if this item has a previous count then send it to the PPC                              // 20-6-2007 PAB
              if (memcmp(recallrec.aRecallItems[lItemArrayptr].anCountTSF, uncounted, 2) == 0) {        // 20-6-2007 PAB
                  // the count is F's so dont try and unpack it                                         // 20-6-2007 PAB // 30-08-2007 5.2 BMG
                  lRecallCount = 0;
              } else {                                                                                  // 20-6-2007 PAB
                 unpack(anRecallCount,4,recallrec.aRecallItems[lItemArrayptr].anCountTSF, 2, 0);        // 20-6-2007 PAB
                 lRecallCount = satoi(anRecallCount, 4);
              }
              //RFSCC will now update the running total when the recall is committed to the STKMQ       // 9-8-07 PAB    
              //lNewCount = satoi(pRCG->anRecallCount,4);
              //lRecallCount = lRecallCount + lNewCount;                                                // 9-8-07 PAB
              LONG_TO_ARRAY(anRecallCount, lRecallCount);                                               // 20-6-2007 PAB
              //pack(recallrec.aRecallItems[lItemArrayptr].anCountTSF, 4,anRecallCount, 2,0);             // 20-6-2007 PAB // 30-08-2007 5.2 BMG
              pack(recallrec.aRecallItems[lItemArrayptr].anSessionCount, 2, pRCG->anRecallCount ,4 ,0); // 20-6-2007 PAB
              // set counted today flag to "Y"
              memset(recallrec.aRecallItems[lItemArrayptr].cCountUpdatedToday,0x59,1);
              // write back the recall record
              usrrc = s_write( 0, recall.fnum,                               
                             (void *)&recallrec, RECALL_RECL, 0L ); 
              if (usrrc < RC_OK) {                                                                      // 24-05-07 PAB Recalls
                 disp_msg("WR RECALL failed");                                                          // 24-05-07 PAB Recalls
                 prep_nak("ERRORUnable to update RECALL file. "                                         // 24-05-07 PAB Recalls
                          "Check appl event logs" );                                                    // 24-05-07 PAB Recalls
                 return;                                                                                // 24-05-07 PAB Recalls
             } 
             prep_ack("");
             return;
         }
          lItemArrayptr++;                                                                              // 31-05-07 PAB Recalls
       } 
       recallrec.ubChain++;
    }
    // fall through should never get here
    disp_msg("Recall item update item not found");              // 24-05-07 PAB Recalls
    prep_nak("ERRORUThis item could not be found "              // 24-05-07 PAB Recalls
             "on this Recall");                                 // 24-05-07 PAB Recalls
    return;
}                                                               // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
void RecallSelectList(char *inbound) {                          // 24-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    LRT_RCH* pRCH = (LRT_RCH*)inbound;                          // 31-05-07 PAB Recalls
    LRT_RCF* pRCF = (LRT_RCF*)out;                              // 31-05-07 PAB Recalls
    LRT_RCE* pRCE = (LRT_RCE*)out;                              // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls
    LONG lIndexPtr = 0;                                         // 29-05-07 PAB Recalls
    LONG lItemCnt = 0;                                          // 31-05-07 PAB Recalls
    LONG lArrayPtr = 0;                                         // 31-05-07 PAB Recalls
    LONG lItemsinRecall = 0;                                    // 31-05-07 PAB Recalls
    // BYTE boots_code[4];                                      // 31-05-07 PAB Recalls
    BYTE boots_code_ncd[4];                                     // 31-05-07 PAB Recalls
    memset(boots_code_ncd, 0x00, 4);                            // 31-05-07 PAB Recalls
    BYTE null_boots_code[3];                                    // 31-05-07 PAB Recalls
    BYTE uncounted[2];                                          // 20-06-07 PAB Recalls
    memset(null_boots_code, 0x00, 3);                           // 31-05-07 PAB Recalls
    memset(uncounted, 0xFF, 2);                                 // 27-06-07 PAB recalls
                                                                // 31-05-07 PAB Recalls
    UNUSED(inbound);                                            // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    // check status of recall files                             // 24-05-07 PAB Recalls
    usrrc = CheckRecallAvailable();                             // 24-05-07 PAB Recalls
    if (usrrc!=0) {                                             // 24-05-07 PAB Recalls
        return;                                                 // 24-05-07 PAB Recalls
    }                                                           // 24-05-07 PAB Recalls                                               // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    // Read the recalls file for the requested ID               // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    memcpy( recallrec.abRecallReference,                        // 24-05-07 PAB Recalls
             pRCH->anRecallRef, 8 );                            // 24-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
    // compute which recall chain the items required are located// 31-05-07 PAB Recalls
    lIndexPtr = satoi(pRCH->anItemPtr, 4);                      // 31-05-07 PAB Recalls
    lItemCnt = lIndexPtr;                                       // 31-05-07 PAB Recalls
    recallrec.ubChain = 0;                                      // 31-05-07 PAB Recalls

    while (lIndexPtr >= 50) {                                   // 31-05-07 PAB Recalls
        recallrec.ubChain++;                                    // 31-05-07 PAB Recalls
        lIndexPtr = lIndexPtr - 50;                             // 31-05-07 PAB Recalls
    }                                                           // 31-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
        usrrc = s_read( 0, recall.fnum, (void *)&recallrec,     // 24-05-07 PAB Recalls
                              RECALL_RECL, RECALL_KEYL );       // 24-05-07 PAB Recalls
                                                                // 31-05-07 PAB Recalls
        //If error reading RECALL                               // 24-05-07 PAB Recalls
        if (usrrc<=RC_OK) {                                     // 24-05-07 PAB Recalls
           recall.present=FALSE;                                // 24-05-07 PAB Recalls
        } else {  // 24-05-07 PAB Recalls                       // 31-05-07 PAB Recalls
            recall.present=TRUE;                                // 24-05-07 PAB Recalls
            if (debug) {                                        // 24-05-07 PAB Recalls
               sprintf(msg, "OK, REC :");                       // 24-05-07 PAB Recalls
               disp_msg(msg);                                   // 24-05-07 PAB Recalls
               //dump( (BYTE *)&recallrec, RECALL_RECL );       // 24-05-07 PAB Recalls
            }
        }
    
       if (debug) {                                            
          sprintf(msg, "Requesting Recall Items. %08lX ", lItemCnt );    
          disp_msg(msg);
          disp_msg("Recall item count is -");
          dump((BYTE *)recallrec.anItemCount,sizeof(recallrec.anItemCount));
       } 

       lItemsinRecall = satoi(recallrec.anItemCount, sizeof(recallrec.anItemCount));

       // if all items send constuct RCE
       if (recall.present == FALSE || lItemCnt >= lItemsinRecall) {
          // we have sent all the items
          memcpy(pRCE->cmd, "RCE", sizeof(pRCE->cmd)); 
          memcpy(pRCE->opid, pRCH->opid, sizeof(pRCE->opid)); 
          out_lth = LRT_RCE_LTH;  
          return;
       }

       // else construct RCF response record to send items
       memcpy(pRCF->cmd, "RCF", sizeof(pRCF->cmd)); 
       memcpy(pRCF->opid, pRCH->opid, sizeof(pRCF->opid)); 
       memcpy(pRCF->anRecallRef,pRCH->anRecallRef,sizeof(pRCF->anRecallRef));

       if (lItemCnt < lItemsinRecall) {
          memset(pRCF->cMoreItems,'Y',1);       
       }

     while (lArrayPtr < 10) {
        // prepack outbound array with 'FF'
        memset(pRCF->abItemArray[lArrayPtr].anRecallItem,0x46,
               sizeof(pRCF->abItemArray[lArrayPtr].anRecallItem));
        memset(pRCF->abItemArray[lArrayPtr].abItemDesc,0x46,
               sizeof(pRCF->abItemArray[lArrayPtr].abItemDesc));
        memset(pRCF->abItemArray[lArrayPtr].anItemTSF,0x46,
               sizeof(pRCF->abItemArray[lArrayPtr].anItemTSF));
        memset (pRCF->abItemArray[lArrayPtr].anRecCnt,0x46,          // 20-6-2007 PAB
               sizeof(pRCF->abItemArray[lArrayPtr].anRecCnt));       // 20-6-2007 PAB
        lArrayPtr++;
    }
    
    lArrayPtr=0;
    open_imstc();
    open_idf();                                              
    open_stock();

    // populate the item array
    while ((lArrayPtr < 10) &&
           (lItemCnt <= lItemsinRecall)) {

        // unpack the boots code from recall rec to output txn.
        if (memcmp(recallrec.aRecallItems[lIndexPtr].abItemCode,
                   null_boots_code, 3) == 0) {
                   // null item code reached in recall record array
            memset(pRCF->cMoreItems,'N',1);
            break;
        }
        
        unpack(pRCF->abItemArray[lArrayPtr].anRecallItem,6,
               recallrec.aRecallItems[lIndexPtr].abItemCode,3,0);

        if (*recallrec.aRecallItems[lIndexPtr].cCountUpdatedToday == 'Y') {                                      // 07-09-2007 5.3 BMG
            unpack(pRCF->abItemArray[lArrayPtr].anRecCnt,4,                                                      // 07-09-2007 5.3 BMG
                   recallrec.aRecallItems[lIndexPtr].anSessionCount, 2, 0);                                      // 07-09-2007 5.3 BMG
        } else {                                                                                                 // 07-09-2007 5.3 BMG
           // if this item has a previous count then send it to the PPC                    // 20-6-2007 PAB
           if (memcmp(recallrec.aRecallItems[lIndexPtr].anCountTSF, uncounted, 2) == 0) {  // 20-6-2007 PAB
               // the count is spaces so dont try and unpack it                            // 20-6-2007 PAB
               memset(pRCF->abItemArray[lArrayPtr].anRecCnt,0x20,4);                       // 20-6-2007 PAB
           } else {                                                                        // 20-6-2007 PAB
               unpack(pRCF->abItemArray[lArrayPtr].anRecCnt,4,                             // 20-6-2007 PAB
                      recallrec.aRecallItems[lIndexPtr].anCountTSF, 2, 0);                 // 20-6-2007 PAB
           }                                                                               // 20-6-2007 PAB
        }                                                                                                        // 07-09-2007 5.3 BMG

        memcpy(pRCF->abItemArray[lArrayPtr].cItemFlag, recallrec.aRecallItems[lIndexPtr].cCountUpdatedToday, 1); // 07-09-2007 5.3 BMG

        // look up the item description from the IDF      
        calc_boots_cd(idfrec.boots_code, recallrec.aRecallItems[lIndexPtr].abItemCode);                    
        usrrc = ReadIdf(__LINE__);

        if (usrrc<=0L) {
            // set idf descrption to not of file
            memcpy( pRCF->abItemArray[lArrayPtr].abItemDesc, 
                    "Item not on file   ", 20 );
        } else { 
            memcpy( pRCF->abItemArray[lArrayPtr].abItemDesc, idfrec.stndrd_desc, 20 );
        }

        // get the current TSF.
        // look up item on IMSTC (to get latest stock figure)
        
        memcpy(boots_code_ncd+1, recallrec.aRecallItems[lIndexPtr].abItemCode, 3);
        memset ( imstcrec.bar_code, 0x00, sizeof(imstcrec.bar_code) ) ;
        memcpy ( imstcrec.bar_code + 7, boots_code_ncd, 4 ) ;

        if (debug) {
            disp_msg ( "RD IMSTC : " ) ;
            dump ( imstcrec.bar_code, 11 ) ;
        }
        usrrc = s_read(0, imstc.fnum, (void *)&imstcrec, IMSTC_RECL, IMSTC_KEYL);
        
        if (usrrc<=0L) {
            imstc.present=FALSE;
        } else {
            imstc.present=TRUE;
        }

        if (imstc.present) {
           // Use stock figure from IMSTC
           sprintf( sbuf, "%04d", imstcrec.stock_figure );
           memcpy( pRCF->abItemArray[lArrayPtr].anItemTSF, sbuf, 
                   sizeof(pRCF->abItemArray[lArrayPtr].anItemTSF) );
        } else {
           memcpy(stockrec.boots_code, idfrec.boots_code, sizeof(stockrec.boots_code) );    // 02-08-07 PAB Recalls
           usrrc = ReadStock(__LINE__);  
           if (usrrc<=0L) {
               memset( pRCF->abItemArray[lArrayPtr].anItemTSF, 0x30, 
                       sizeof(pRCF->abItemArray[lArrayPtr].anItemTSF) );
           } else {
              // Use stock figure from STOCK
              sprintf( sbuf, "%04d", stockrec.stock_fig );
              memcpy( pRCF->abItemArray[lArrayPtr].anItemTSF, sbuf, 
                      sizeof(pRCF->abItemArray[lArrayPtr].anItemTSF) );
           }
       }
                                                               // 31-05-07 PAB Recalls
       lArrayPtr++;                                            // 31-05-07 PAB Recalls
       lIndexPtr++;                                            // 31-05-07 PAB Recalls
    }                                                          // 31-05-07 PAB Recalls
                                                               // 31-05-07 PAB Recalls
    if (lItemCnt == lItemsinRecall) {                          // 31-05-07 PAB Recalls
       memset(pRCF->cMoreItems,'N',1);                         // 31-05-07 PAB Recalls
    }                                                          // 31-05-07 PAB Recalls
    memcpy(pRCF->cStatus,recallrec.cRecallStatus,1);           // 09-08-07 PAB Recalls
                                                               // 31-05-07 PAB Recalls
    close_imstc(CL_SESSION);                                   // 31-05-07 PAB Recalls
    close_idf(CL_SESSION);                                     // 31-05-07 PAB Recalls
    close_stock( CL_SESSION );                                 // 31-05-07 PAB Recalls
                                                               // 31-05-07 PAB Recalls
    out_lth = LRT_RCF_LTH;                                     // 31-05-07 PAB Recalls
    return;                                                    // 31-05-07 PAB Recalls
}                                                              // 24-05-07 PAB Recalls
                                                               // 24-05-07 PAB Recalls
void RecallInstructions(char *inbound) {                       // 24-05-07 PAB Recalls

   LRT_RCI* pRCI = (LRT_RCI*)inbound;                          // 31-05-07 PAB Recalls
   LRT_RCJ* pRCJ = (LRT_RCJ*)out;                              // 31-05-07 PAB Recalls
                                                               // 31-05-07 PAB Recalls
   LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls
                                                               // 31-05-07 PAB Recalls
   UNUSED(inbound);                                            // 24-05-07 PAB Recalls
                                                               // 24-05-07 PAB Recalls
   // check status of recall files                             // 24-05-07 PAB Recalls
   usrrc = CheckRecallAvailable();                             // 24-05-07 PAB Recalls
   if (usrrc!=0) {                                             // 24-05-07 PAB Recalls
       return;                                                 // 24-05-07 PAB Recalls
   }                                                           // 24-05-07 PAB Recalls
                                                               // 24-05-07 PAB Recalls
   memcpy(rcspirec.abRecallReference,pRCI->anRecallRef,8);     // 24-05-07 PAB Recalls
   usrrc = s_read( 0, rcspi.fnum, (void *)&rcspirec,           // 24-05-07 PAB Recalls
                              RCSPI_RECL, RCSPI_KEYL );        // 24-05-07 PAB Recalls

   if (usrrc < RC_OK) {                                         // 24-05-07 PAB Recalls
       disp_msg("RCSPI Record no found");                       // 24-05-07 PAB Recalls
       prep_nak("The special instructions "                     // 20-08-07 PAB Recalls
                "for this Recall could not be found." );        // 24-05-07 PAB Recalls
       return;                                                  // 24-05-07 PAB Recalls
  }                                                             // 24-05-07 PAB Recalls
   memcpy(pRCJ->cmd, "RCJ", sizeof(pRCJ->cmd)); 
   memcpy(pRCJ->opid,pRCI->opid,sizeof(pRCJ->opid));
   memcpy(pRCJ->anRecallRef, pRCI->anRecallRef, 
          sizeof(pRCJ->anRecallRef)); 
   memcpy(pRCJ->abSpecialIns,rcspirec.abMessage, 
           sizeof(rcspirec.abMessage));

   out_lth = LRT_RCF_LTH;                                       // 31-05-07 PAB Recalls
   return; 

}                                                               // 24-05-07 PAB Recalls
                                                                

LONG CheckRecallAvailable(void ) {

   LONG usrrc = RC_OK;

   // check status of recok record                             // 24-05-07 PAB Recalls
   usrrc = process_recok();                                    // 24-05-07 PAB Recalls
   if (usrrc!=RC_OK) {                                         // 24-05-07 PAB Recalls
       return 0;                                               // 24-05-07 PAB Recalls
       }                                                       // 24-05-07 PAB Recalls
                                                               // 24-05-07 PAB Recalls
                                                               // 24-05-07 PAB Recalls
   // if the recall files are not available.                   // 24-05-07 PAB Recalls
   if (gRecallFilesAvailable == 1) {                           // 24-05-07 PAB Recalls
       prep_nak("ERRORRecall Files are not available."         // 24-05-07 PAB Recalls
                " Please try later.");                         // 25-05-07 PAB Recalls
       return 1;                                               // 24-05-07 PAB Recalls
   }             

}


void StopRecalls(void) {                                        // 24-05-07 PAB Recalls
    // block access to recall files                             // 24-05-07 PAB Recalls
    gRecallFilesAvailable = 1;                                  // 24-05-07 PAB Recalls
    // and force physical close of recall files.                // 31-05-07 PAB Recalls
    if (debug) {                                                // 11-09-2007 5.5 BMG
        disp_msg("Got REC* so closing Recalls Files");          // 11-09-2007 5.5 BMG
    }                                                           // 11-09-2007 5.5 BMG
    close_recall( CL_ALL );                                     // 31-05-07 PAB Recalls
    close_rcindx ( CL_ALL );                                    // 31-05-07 PAB Recalls
    close_rcspi ( CL_ALL) ;                                     // 31-05-07 PAB Recalls
    if (debug) {                                                // 11-09-2007 5.5 BMG
        disp_msg("Closed Recalls Files");                       // 11-09-2007 5.5 BMG
    }                                                           // 11-09-2007 5.5 BMG
                                                                // 31-05-07 PAB Recalls
}                                                               // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
URC process_recok(void)                                         // 24-05-07 PAB Recalls        
{                                                               // 24-05-07 PAB Recalls       
    LONG usrrc = RC_OK;                                         // 24-05-07 PAB Recalls        
    LONG rc;                                                    // 24-05-07 PAB Recalls       
    usrrc = open_recok();                                       // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    // if the recall OK file is not found.                      // 24-05-07 PAB Recalls
    if (usrrc != RC_OK) {                                       // 24-05-07 PAB Recalls
        disp_msg("RECOK not found");                            // 24-05-07 PAB Recalls
        prep_nak("ERRORUnable to open RECOK file. "             // 24-05-07 PAB Recalls
                 "Check appl event logs" );                     // 24-05-07 PAB Recalls
        return RC_FILE_ERR;                                     // 24-05-07 PAB Recalls
        }                                                       // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls   
    // read the current status record & close the file          // 24-05-07 PAB Recalls      
    rc = s_read( A_BOFOFF,                                      // 24-05-07 PAB Recalls     
                 recok.fnum, (void *)&recokrec, RECOK_RECL, 0L);// 24-05-07 PAB Recalls 
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    close_recok ( CL_SESSION );                                 // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    gRecallFilesAvailable = 0;                                  // 24-05-07 PAB Recalls
//    if (recokrec.cPSB70Processing != 'E') {                     // 24-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        disp_msg("Recall Files are not available");             // 24-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        gRecallFilesAvailable = 1;                              // 24-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        // force physical close of recall files                 // 31-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        close_recall( CL_ALL );                                 // 31-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        close_rcindx ( CL_ALL );                                // 31-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        close_rcspi ( CL_ALL) ;                                 // 31-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        return RC_OK;                                           // 24-05-07 PAB Recalls // 23-08-2007 5.1 BMG
//        }                                                       // 24-05-07 PAB Recalls // 23-08-2007 5.1 BMG
                                                                // 24-05-07 PAB Recalls
                                                                // 24-05-07 PAB Recalls
    if (rc <= 0L) {                                             // 24-05-07 PAB Recalls      
        log_event101(rc, RFOK_REP, __LINE__);                   // 24-05-07 PAB Recalls      
        printf("ERR-R RECOK. RC:%081X", rc);                    // 24-05-07 PAB Recalls       
        return RC_DATA_ERR;                                     // 24-05-07 PAB Recalls      
    }                                                           // 24-05-07 PAB Recalls      
                                                                // 24-05-07 PAB Recalls
    return RC_OK;                                               // 24-05-07 PAB Recalls      
}                                                               // 24-05-07 PAB Recalls      
                                                                // 24-05-07 PAB Recalls
