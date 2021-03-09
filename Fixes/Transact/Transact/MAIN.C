/*************************************************************************
*
*
* File: main.c
*
* Author: Prashant Kotak
*
* Created: 18/08/97
*
* Purpose: Sockets Server for Boots 4690 Application under
*          IBM version of  FlexOs. Start Module.
*
* History:
* Version B:         Paul Bowers                18/09/98
*
* Purpose: To prevent the application from ending if the IP stack
*          is not loaded, rather wait and retry until it is available
*
* Version C:         Brian Greenfield           24-02-2009
* Removed the fact that if RFSCF was phase 5 it didn't lock the
* CCDMY file. this is no longer required.
*
* Version D:         Brian Greenfield           04-03-2009
* Added calls to the two new event handlers.
*
* Version E:         Charles Skadorwa           15-04-2010
* Changes to support POD Logging Enhancements.
*
* Version F:         Brian Greenfield           15-11-2010
* Changes to accomodate the new DEC Microbroker service.
*
* Version G:         Charles Skadorwa           30-11-2010
* Change to ensure that message displayed on foreground/background if user
* attempts to start another version of TRANSACT.
*
* Version H:         Visakha Satya              20-04-2015
* SC079 Dallas Positive Receiving
* - Changes to support Dallas Positive Receiving.
*
*************************************************************************/

#include "transact.h" //SDH 19-May-2006

#include <string.h>
#include "osfunc.h"                     /* needed for disp_msg() */
#include "trxfile.h"                    // Streamline SDH 22-Sep-2008
#include "trxutil.h"                    // Streamline SDH 22-Sep-2008
#include "sockserv.h"
#include "rfglobal.h"                   // v4.0
#include "osfunc.h"
#include "prtctl.h"                     // Streamline SDH 17-Sep-2008
#include "prtlist.h"                    // Streamline SDH 17-Sep-2008
#include "srfiles.h"                    // Streamline SDH 22-Sep-2008
#include "idf.h"                        // Streamline SDH 22-Sep-2008
#include "irf.h"                        // Streamline SDH 22-Sep-2008
#include "isf.h"                        // Streamline SDH 22-Sep-2008
#include "rfscf.h"                      // Streamline SDH 22-Sep-2008
#include "ccfiles.h"                    // Streamline SDH 22-Sep-2008
#include "invok.h"                      // Streamline SDH 22-Sep-2008
#include "ealterms.h"                   // SDH 20-May-2009 Model Day
#include "af.h"                         // Streamline SDH 22-Sep-2008
#include "affcf.h"                      // SDH 20-May-2009 Model Day
#include "bcsmf.h"                      // Streamline SDH 22-Sep-2008
#include "ccfiles.h"                    // Streamline SDH 22-Sep-2008
#include "clfiles.h"                    // Streamline SDH 22-Sep-2008
#include "iudf.h"                       // RJN 28-06-2013 ELR
#include "MBfiles.h"                    // BMG 15-11-2010
#include "nvurl.h"                      // Streamline SDH 22-Sep-2008
#include "pgf.h"                        // Streamline SDH 22-Sep-2008
#include "phf.h"                        // Streamline SDH 22-Sep-2008
#include "rfhist.h"                     // Streamline SDH 22-Sep-2008
#include "seldesc.h"                    // Streamline SDH 22-Sep-2008
#include "GIAfiles.h"                   // BMG ASN/Directs 15-10-2008
#include "podlog.h"                     // CSk 15-04-2010 POD Logging
#include "podok.h"                      // CSk 15-04-2010 POD Logging
#include "DALfiles.h"                   // HVS 20-04-2015 Dallas


//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   startup() - Do once on startup                                       ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

static int startup() {

   WORD i=0, rc=0;
   URC usrrc = RC_OK;

   // Print out a status message
   disp_msg( "RFS - Initialising");
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

   minls.sessions   = 0;
   minls.fnum       = -1L;
   minls.pbFileName = MINLS;                                                // SDH  11-03-2005  EXCESS
   minls.wOpenFlags = MINLS_OFLAGS;                                         // SDH  11-03-2005  EXCESS
   minls.wReportNum = MINLS_REP;                                            // SDH  11-03-2005  EXCESS
   minls.pBuffer    = &minlsrec;                                            // SDH  11-03-2005  EXCESS
   minls.wRecLth    = MINLS_RECL;                                           // SDH  11-03-2005  EXCESS
   minls.wKeyLth    = MINLS_KEYL;                                           // SDH  11-03-2005  EXCESS

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

   rfok.sessions   = 0;                                             // SDH  14-03-05  EXCESS
   rfok.fnum       = -1L;                                           // SDH  14-03-05  EXCESS
   rfok.pbFileName = RFOK;                                          // SDH  14-03-05  EXCESS
   rfok.wOpenFlags = RFOK_OFLAGS;                                   // SDH  14-03-05  EXCESS
   rfok.wReportNum = RFOK_REP;                                      // SDH  14-03-05  EXCESS
   rfok.wRecLth    = RFOK_RECL;                                     // SDH  14-03-05  EXCESS
   rfok.wKeyLth    = 0;                                             // SDH  14-03-05  EXCESS

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
   recall.wKeyLth    = 9;                                             // Mult UOD Rcll 12-08-2011 SDH

   pdtasset.sessions   = 0;                                           // BMG 16-09-2008 MC70
   pdtasset.fnum       = -1L;                                         // BMG 16-09-2008 MC70
   pdtasset.pbFileName = PDTASSET;                                    // BMG 16-09-2008 MC70
   pdtasset.wOpenFlags = PDTASSET_OFLAGS;                             // BMG 16-09-2008 MC70
   pdtasset.wReportNum = PDTASSET_REP;                                // BMG 16-09-2008 MC70
   pdtasset.pBuffer    = &pdtassetrec;                                // BMG 16-09-2008 MC70
   pdtasset.wRecLth    = PDTASSET_RECL;                               // BMG 16-09-2008 MC70
   pdtasset.wKeyLth    = PDTASSET_KEYL;                               // BMG 16-09-2008 MC70

   AfSet();                                                           // Streamline SDH 17-Sep-2008
   AffcfSet();                                                        // SDH 20-May-2009 Model Day
   BcsmfSet();                                                        // Streamline SDH 17-Sep-2008
   CartonSet();                                                       // BMG ASN/Directs 15-10-2008
   CcdirsuSet();                                                      // Streamline SDH 17-Sep-2008
   CcdmySet();                                                        // Streamline SDH 17-Sep-2008
   CchistSet();                                                       // Streamline SDH 17-Sep-2008
   CcilfSet();                                                        // Streamline SDH 17-Sep-2008
   CclolSet();                                                        // Streamline SDH 17-Sep-2008
   ClilfSet();                                                        // Streamline SDH 17-Sep-2008
   ClolfSet();                                                        // Streamline SDH 17-Sep-2008
   DecconfSet();                                                      // BMG 15-11-2010
   DelvindxSet();                                                     // BMG ASN/Directs 15-10-2008
   DelvlistSet();                                                     // BMG ASN/Directs 15-10-2008
   DelvsmrySet();                                                     // BMG ASN/Directs 15-10-2008
   DirorSet();                                                        // BMG ASN/Directs 15-10-2008
   DirsuSet();                                                        // BMG ASN/Directs 15-10-2008
   EalsoptsSet();                                                     // BMG 15-11-2010
   EaltermsSet();                                                     // SDH 20-May-2009 Model Day
   IdfSet();                                                          // Streamline SDH 17-Sep-2008
   InvceSet();                                                        // HVS 20-04-2015 Dallas
   InvokSet();                                                        // Streamline SDH 23-Sep-2008
   IrfSet();                                                          // Streamline SDH 17-Sep-2008
   IrfdexSet();                                                       // Streamline SDH 17-Sep-2008
   IsfSet();                                                          // Streamline SDH 17-Sep-2008
   IudfSet();                                                         // RJN 28-06-2013 ELR
   NvurlSet();                                                        // Streamline SDH 17-Sep-2008
   PgfSet();                                                          // Streamline SDH 17-Sep-2008
   PhfSet();                                                          // Streamline SDH 17-Sep-2008
   PodlogSet();                                                       // CSk 15-04-2010 POD Logging
   PodokSet();                                                        // CSk 15-04-2010 POD Logging
   PogokSet();                                                        // Streamline SDH 17-Sep-2008
   PrtctlSet();                                                       // Streamline SDH 17-Sep-2008
   PrtlistSet();                                                      // Streamline SDH 17-Sep-2008
   RFSCacheSet();                                                     // BMG 15-11-2010
   RfhistSet();                                                       // Streamline SDH 17-Sep-2008
   RfscfSet();                                                        // Streamline SDH 17-Sep-2008
   SeldescSet();                                                      // Streamline SDH 17-Sep-2008
   SrcatSet();                                                        // Streamline SDH 17-Sep-2008
   SritmlSet();                                                       // Streamline SDH 17-Sep-2008
   SritmpSet();                                                       // Streamline SDH 17-Sep-2008
   SrmapSet();                                                        //CSk 12-03-2012 SFA
   SrmodSet();                                                        // Streamline SDH 17-Sep-2008
   SrpogSet();                                                        // Streamline SDH 17-Sep-2008
   SrpogifSet();                                                      // Streamline SDH 17-Sep-2008
   SrpogilSet();                                                      // Streamline SDH 17-Sep-2008
   SrpogipSet();                                                      // Streamline SDH 17-Sep-2008
   SrsxfSet();                                                        // Streamline SDH 17-Sep-2008
   UodinSet();                                                        // BMG ASN/Directs 15-10-2008
   UodotSet();                                                        // BMG ASN/Directs 15-10-2008
   WhindxSet();                                                       // HVS 20-04-2015 Dallas
   WhuodSet();                                                        // HVS 20-04-2015 Dallas

   prepare_logging();

   open_pdtasset();                                                   // BMG 16-09-2008 MC70

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
   usrrc = InvokOpen();                                                 // Streamline SDH 17-Sep-2008
   if ( usrrc<=RC_DATA_ERR ) return 1;
   InvokRead(0L, __LINE__);                                             // Streamline SDH 17-Sep-2008
   InvokClose( CL_SESSION );                                            // Streamline SDH 17-Sep-2008

   /////////////////////////////////////////////////////////////////////////////
   //
   //  Process the RFS Control File (RFSCF)
   //
   /////////////////////////////////////////////////////////////////////////////

   //Open it
   usrrc = RfscfOpen();                                                 // Streamline SDH 17-Sep-2008

   if ( usrrc==RC_OK ) {

      // Read RFSCF records
      RfscfRead(0L, __LINE__);                                          // Streamline SDH 17-Sep-2008
      RfscfRead(1L, __LINE__);                                          // Streamline SDH 17-Sep-2008
      RfscfRead(2L, __LINE__);                                          // Streamline SDH 17-Sep-2008

      //activity = rfscfrec1and2.activity;                                // 16-11-04 SDH

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
      if (rfscfrec3.bCCActive == 'Y') {                           // 16-11-04 SDH // 24-02-2009 BMG
          CcdmyOpenLocked();                                      // 16-11-04 SDH
      } else {                                                    // 16-11-04 SDH
          CcdmyClose();                                           // 16-11-04 SDH
      }                                                           // 16-11-04 SDH

      RfscfClose( CL_SESSION );

   } else {
      //activity = 0;
   }

   // Process any orphan work files that exist
   process_orphans(TRUE);

   //printf( "RFS - Ready\n" );
   sprintf( msg, "RFS - Restarting, Please wait...");
   background_msg(msg);

   return(rc);

}

int shutdown (void);

void main(int argc, char *argv[])
{
   long lrc;

   UNUSED(argc);                                                            // Streamline SDH 16-Sep-2008
   UNUSED(argv);                                                            // Streamline SDH 16-Sep-2008

   // Set debug on by default if on foreground                  // SDH 4-July-2006
   // Determine application start method
   UBYTE bg = establish_start_method();                         // Streamline SDH 23-Sep-2008
   if (!bg) {                                                   // SDH 4-July-2006
       oput = DBG_LOCAL;                                        // SDH 4-July-2006
       debug = TRUE;                                            // SDH 4-July-2006
       disp_msg("Debug ON");                                    // SDH 4-July-2006
   } else {                                                     // SDH 4-July-2006
       debug = FALSE;                                           // SDH 4-July-2006
   }                                                            // SDH 4-July-2006

   //First thing, check whether the comms pipe is already                   //SDH 18-02-2005
   //created -- which means that TRANSACT is already running.               //SDH 18-02-2005
   lrc = s_create(O_FILE, CPIPE_CFLAGS, CPIPE, 1,                           //SDH 18-02-2005
                              0x0FFF, CPIPE_RECL);                          //SDH 18-02-2005
   if (lrc <= 0) {                                                          //SDH 18-02-2005
       sprintf( msg, "Error - Unable to create Comms pipe" );               //SDH 18-02-2005
       disp_msg(msg);                                                       //SDH 18-02-2005
       background_msg(msg);                                                 // CSk 30-11-2010 BAU Improvement
       return;                                                              //SDH 18-02-2005
   }                                                                        //SDH 18-02-2005
   s_close(0, lrc);                                                         //SDH 18-02-2005

   // setup the Control-Break handler
   lrc = TermEvent () ;
   if ( lrc < 0L ) {
      disp_msg ("Ctl-Brk handler failed");                                  // Streamline SDH 16-Sep-2008
   }

   // Set up the Divide By Zero event handler                               // 04-03-2009 BMG
   lrc = DivideExceptEvent () ;                                             // 04-03-2009 BMG
   if ( lrc < 0L ) {                                                        // 04-03-2009 BMG
      disp_msg ("Divide Exception event handler failed");                   // 04-03-2009 BMG
   }                                                                        // 04-03-2009 BMG

   // Set up the Protection Exception event handler                         // 04-03-2009 BMG
   lrc = ProtExceptEvent () ;                                               // 04-03-2009 BMG
   if ( lrc < 0L ) {                                                        // 04-03-2009 BMG
      disp_msg ("Protection Exception event handler failed");               // 04-03-2009 BMG
   }                                                                        // 04-03-2009 BMG

   // check if protocol stack if loaded
   // ---------------------------------------------------
   // Version B         Paul Bowers               18/9/98
   // Wait here until the stack is loaded
   //    do not end the application
   // ---------------------------------------------------

   background_msg("RFS - Waiting for system");
   while ( TcpLoaded() ) {
      disp_msg ("The TCP/IP protocol stack is not loaded");                 //Streamline SDH 16-Sep-2008
      s_timer(0,5000);
   }

   // initialise sockets bits ...
   InitialiseSocketServer ();

   startup ();
   InitialiseDECSocket (0); // BMG 15-11-2010
   SocketServerLoop ();

   // shutdown boots bits should never reach this code as app runs for ever
   shutdown ();
   disp_msg ("Socket Server Stopped");

}

