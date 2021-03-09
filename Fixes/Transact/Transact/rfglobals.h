// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                     Radio Frequency Counts Processor
// 
// -----------------------------------------------------------------------
#ifndef GLOBALS_H
#define GLOBALS_H 1

#include "transact.h"

#pragma Data (Common);
      
//BYTE  out[2048];    // Master output buffer
BYTE  out[4096];    // Master output buffer                         // SDH
WORD  out_lth;      // Master output buffer length
UWORD hh_unit=0;

//FILE_TABLE *ftab[2000];                                                   //SDH 12-04-2005
FILE_TABLE ftab[2000];                                                      //SDH 12-04-2005

ACTIVE_LRT *lrtp[MAX_UNIT];
PROG_PQ  pq[MAX_UNIT];
BOOLEAN status_wk_block = FALSE;
BOOLEAN status_wg_block = FALSE;
//WORD  debug = FALSE;
//WORD  oput  = DBG_LOCAL;
FILE_CTRL irf, idf, imstc, stock, cimf, citem, selbf, lrtlg, gapbf, af,
isf, stkmq, imfp, pllol, plldb, rfrdesc, tsf, psbt, wrf, cpipe,
rfscf, minls, rfhist, invok, clolf, clilf, rfsstat, pilst,
nvurl, pst, epsom, pchk, pgf, suspt, piitm, prtctl, prtlist, rfok,
ccdmy, bcsmf, cclol, ccilf, cchist, ccdirsu, deal, tdtff, jobok, irfdex;    // SDH 17-11-04 OSSR WAN
BYTE msg[100];
BYTE *pDealRewdMsg = NULL;                                                  // SDH 09-12-04 PROMOTIONS

INVOK_REC invokrec;     
RFHIST_REC rfhistrec;                                                       // SDH 17-11-04 OSSR WAN
BCSMF_REC bcsmfrec;                                                         // SDH 17-11-04 OSSR WAN
CCLOL_REC cclolrec;                                                         // SDH 29-11-04 CREDIT CLAIM
CCILF_REC ccilfrec;                                                         // SDH 29-11-04 CREDIT CLAIM
CCHIST_REC cchistrec;                                                       // SDH 29-11-04 CREDIT CLAIM
CCDIRSU_REC ccdirsurec;                                                     // SDH 29-11-04 CREDIT CLAIM
DEAL_REC dealrec;                                                           // SDH 09-12-04 PROMOTIONS
TDTFF_HEADER TdtffHeader;                                                   // SDH 09-12-04 PROMOTIONS
TDTFF_REC tdtffrec;                                                         // SDH 09-12-04 PROMOTIONS
IRFDEX_REC irfdexrec;                                                       // SDH 09-12-04 PROMOTIONS
RFSCF_REC_1AND2 rfscfrec1and2;                                              // 16-11-04 SDH
RFSCF_REC_3 rfscfrec3;                                                      // 16-11-04 SDH
RFOK_REC rfokrec;                                                           // PAB 24-8-04
JOBOK_REC jobokrec;                                                         // PAB 13-12-04
PGF_REC pgfrec;                                                             // SDH 26-01-2005 OSSR WAN
PLLDB_REC plldbrec;                                                         // SDH 22-02-2005 EXCESS
PLLOL_REC pllolrec;                                                         // SDH 22-02-2005 EXCESS
MINLS_REC minlsrec;                                                         // SDH 22-02-2005 EXCESS
IRF_REC irfrec;                                                             // SDH 22-02-2005 EXCESS
IDF_REC idfrec;                                                             // SDH 22-02-2005 EXCESS
STOCK_REC stockrec;                                                         // SDH 22-02-2005 EXCESS

WORD sess;
SHARE_STAT_REC *share;                                                      // Shared memory details
SMTABLE sharetab;

LONG min_wf_fnum = 0x00007FFF;                                              // PAB 4.03
LONG max_wf_fnum = 0x00000000;                                              // PAB 4.03
//LONG wk_fnum; 
//LONG wg_fnum;  

UWORD write_gap;                                                            // 22-02-04 PAB

int   CurrentHHT;                             // current HHT Unit 
char cStoreClosed;

FILE * DebugFileHandle = NULL ;
char SockDir[128] ;
char HhtLiveLog[128] ;
char HhtFlagFile[128] ;
int KeepArchives ;
char HhtLogExt[14] ;

int Debug = 0 ; // Off by default
WORD debug;
WORD oput;

char OutputToFile = 'N'; 
char PollControlFile = 'N';

UBYTE bg;                       // background appl flag for use with disp_msg()

FILE_CTRL dbg;

int BreakLoop = 0 ;

#pragma Data ();

#endif

