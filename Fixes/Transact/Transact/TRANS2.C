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
// -----------------------------------------------------------------------

/* include files */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>
#include "trans2.h"     // v4.01

#include "adxsrvfn.h"          /* needed for disp_msg() */
#include "adxsrvst.h"          /* needed for bg */
#include "file.h"   // v4.01
#include "rfs.h"               
#include "rfsfile.h"
#include "dateconv.h"
#include "wrap.h"
#include "sockserv.h"
//#include "transact.h"

// Shared globals (with RFS.C)
#include "globals.h"    // v4.0

extern UBYTE bg;        // background appl flag for use with disp_msg()
UWORD activity;


//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   Static (private) variables                                           ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

static BYTE *look_xxx = { "TRANS2  "};                                      // SDH 17-11-04 OSSR WAN
static BYTE sbuf[64];                                                       // SDH 13-12-04 PROMOTIONS
static LONG lLastTDTFFLevel = -1;                                           // SDH 13-12-04 PROMOTIONS

//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   startup() - Do once on startup                                       ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

int startup()
{
   WORD i=0, rc=0;
   LONG rc2;
   URC usrrc = RC_OK;
   //RFSCF_REC_1AND2 rfscfrec1and2;                            // SDH 17-11-04 OSSR WAN
   //RFSCF_REC_3 rfscfrec3;                                    // SDH 17-11-04 OSSR WAN

   // Determine application start method
   bg = establish_start_method();

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

   oput = DBG_LOCAL;                                      // PAB 15-7-2004

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
   //isf.pBuffer    = &isfrec;                                                // SDH  11-03-2005  EXCESS
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
   epsom.pbFileName = EPSOM;                                                // SDH  11-03-2005  EXCESS
   epsom.wOpenFlags = EPSOM_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   epsom.wReportNum = EPSOM_REP;                                            // SDH  11-03-2005  EXCESS
   //epsom.pBuffer    = &epsomrec;                                            // SDH  11-03-2005  EXCESS
   epsom.wRecLth    = EPSOM_RECL;                                           // SDH  11-03-2005  EXCESS

   pchk.sessions   = 0;                                             // v4.0
   pchk.fnum       = -1L;                                           // v4.0
   pchk.pbFileName = PCHK;                                                  // SDH  11-03-2005  EXCESS
   pchk.wOpenFlags = PCHK_OFLAGS;                                           // SDH  11-03-2005  EXCESS
   pchk.wReportNum = PCHK_REP;                                              // SDH  11-03-2005  EXCESS
   //pchk.pBuffer    = &pchkrec;                                              // SDH  11-03-2005  EXCESS
   pchk.wRecLth    = PCHK_RECL;                                             // SDH  11-03-2005  EXCESS

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

   prepare_logging();

//   cpipe.fnum = s_create( O_FILE, CPIPE_CFLAGS, CPIPE, 1, 0, CPIPE_RECL ); // v4.0
   cpipe.fnum = s_create( O_FILE, CPIPE_CFLAGS, CPIPE, 1, 0x0FFF, CPIPE_RECL );  // v4.0

   if ( cpipe.fnum<=0 ) { 
      log_event101(look_xxx, cpipe.fnum, CPIPE_REP, __LINE__);
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
      log_event101(look_xxx, rc2, INVOK_REP, __LINE__);
      sprintf(msg, "Err-R INVOK. RC:%08lX", rc2);
      disp_msg(msg);
   }
   close_invok( CL_SESSION );
   
   // Process any orphan work files that exist
   process_orphans();

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
         log_event101(look_xxx, rc2, RFSCF_REP, __LINE__);
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


// shutdown - Do once if shutdown required
/*int shutdown( void )
{
   int rc = 0;
   URC usrrc = RC_OK;

   disp_msg( "RFS - Deallocating units" );
   background_msg("RFS - Shutdown in progress");
   usrrc = dealloc_lrt_table( ALL_UNITS );
   if ( usrrc<RC_IGNORE_ERR ) {
      return 1; 
   }

   s_close( 0, cpipe.fnum );
   s_delete( 0, CPIPE );

   disp_msg( "RFS - Closing Log" );
   background_msg("RFS - Closed");

   s_close( 0, lrtlg.fnum );

   close_isf( CL_ALL );
   close_idf( CL_ALL );
   close_irf( CL_ALL );
   close_irfdex( CL_ALL );
   close_imstc( CL_ALL );
   close_stock( CL_ALL );
   close_cimf( CL_ALL );
   close_citem( CL_ALL );
   close_af( CL_ALL );
   close_stkmq( CL_ALL );
   close_imfp( CL_ALL );
   close_pllol( CL_ALL );
   close_plldb( CL_ALL );
   close_rfrdesc( CL_ALL );
   close_tsf( CL_ALL );
   close_psbt( CL_ALL );
   close_minls( CL_ALL );
   close_rfhist( CL_ALL );
   close_invok( CL_ALL );
   close_clolf( CL_ALL );
   close_clilf( CL_ALL );
   close_wrf( CL_ALL );
   close_rfsstat( CL_ALL );
   close_pilst( CL_ALL );
   close_pgf( CL_ALL );             // PAB 23-10-03

   disp_msg( "RFS - Closed" );

   return 0;

}*/

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

void prep_nak( BYTE *outbuff, WORD *outlth, BYTE *msg )
{
   memcpy( ((LRT_NAK *)outbuff)->cmd, "NAK", 3 );
   //memcpy( ((LRT_NAK *)outbuff)->msg, msg, LRT_NAK_LTH );
   strncpy( ((LRT_NAK *)outbuff)->msg, msg, LRT_NAK_LTH );                  //SDH 17-01-2005 Promotions
   *outlth = LRT_NAK_LTH;
}

void prep_ack( BYTE *outbuff, WORD *outlth )
{
   memcpy( ((LRT_ACK *)outbuff)->cmd, "ACK", 3 );
   *outlth = LRT_ACK_LTH;
}

void prep_pq_full_nak(void) {
    prep_nak(out, &out_lth, "ERROR  SELs or gap  "
                            "report stalled.     "
                            "Contact help desk.  " );
}

BOOLEAN IsStoreClosed(void) {                                               // SDH 26-11-04 CREDIT CLAIM
    if (cStoreClosed == 'Y') {                                              // SDH 26-11-04 CREDIT CLAIM
        prep_nak( out, &out_lth, "Store close in "                          // SDH 26-11-04 CREDIT CLAIM
                                 "progress please try "                     // SDH 26-11-04 CREDIT CLAIM
                                 "later                       ");           // SDH 26-11-04 CREDIT CLAIM
        return TRUE;                                                        // SDH 26-11-04 CREDIT CLAIM
    }                                                                       // SDH 26-11-04 CREDIT CLAIM
    return FALSE;                                                           // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

BOOLEAN IsHandheldUnknown(void) {                                           // SDH 26-11-04 CREDIT CLAIM
    if (lrtp[hh_unit] == NULL) {                                            // SDH 26-11-04 CREDIT CLAIM
        // We haven't seen this handheld before                             // SDH 26-11-04 CREDIT CLAIM
        prep_nak( out, &out_lth, "ERRORPlease sign "                        // SDH 26-11-04 CREDIT CLAIM
                                 "off and then sign on "                    // SDH 26-11-04 CREDIT CLAIM
                                 "again                  ");                // SDH 26-11-04 CREDIT CLAIM
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
        prep_nak( out, &out_lth, "Report Maintenance in "                   // 4-11-04 PAB
                                 "progress please try "                     // 4-11-04 PAB  
                                 "again later       " );                    // 4-11-04 PAB
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
    if (lRc < 0) log_event101(look_xxx, lRc, DEAL_REP, __LINE__);           // SDH 10-12-04 PROMOTIONS

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

