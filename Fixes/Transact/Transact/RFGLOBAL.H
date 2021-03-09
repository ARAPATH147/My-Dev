// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                     Radio Frequency Counts Processor
//
// -----------------------------------------------------------------------
#ifndef RFGLOBAL_H
#define RFGLOBAL_H 1

//#include "transact.h"
#include "trxfile.h"

#pragma Data (Common);

BYTE sbuf[64];                                                              // Streamline SDH 16-Sep-2008
//BYTE  out[2048];    // Master output buffer
BYTE  out[4096];    // Master output buffer                         // SDH
WORD  out_lth;      // Master output buffer length
UWORD hh_unit=0;
BYTE dtls[16];

//FILE_TABLE *ftab[2000];                                                   //SDH 12-04-2005
FILE_TABLE ftab[2000];                                                      //SDH 12-04-2005

ACTIVE_LRT *lrtp[MAX_UNIT];
PROG_PQ  pq[MAX_UNIT];
//BOOLEAN status_wk_block = FALSE;
//BOOLEAN status_wg_block = FALSE;
//WORD  debug = FALSE;
//WORD  oput  = DBG_LOCAL;
FILE_CTRL imstc, stock, cimf, citem, selbf, lrtlg, gapbf,
    stkmq, imfp, pllol, plldb, rfrdesc, tsf, psbt, wrf, cpipe,
    minls, rfsstat, pilst, pst, epsom, pchk, suspt, piitm, rfok,
    deal, tdtff, jobok, rcindx, recok, rcspi, recall,                       // PAB 24-05-07 Recalls
    spipe, pdtasset, dbg;                                                   // BMG 10-09-2008 MC70
BYTE msg[100];
BYTE *pDealRewdMsg = NULL;                                                  // SDH 09-12-04 PROMOTIONS

DEAL_REC dealrec;                                                           // SDH 09-12-04 PROMOTIONS
TDTFF_HEADER TdtffHeader;                                                   // SDH 09-12-04 PROMOTIONS
TDTFF_REC tdtffrec;                                                         // SDH 09-12-04 PROMOTIONS
RFOK_REC rfokrec;                                                           // PAB 24-8-04
JOBOK_REC jobokrec;                                                         // PAB 13-12-04
PLLDB_REC plldbrec;                                                         // SDH 22-02-2005 EXCESS
PLLOL_REC pllolrec;                                                         // SDH 22-02-2005 EXCESS
MINLS_REC minlsrec;                                                         // SDH 22-02-2005 EXCESS
STOCK_REC stockrec;                                                         // SDH 22-02-2005 EXCESS
SELBF_REC selbfrec;                                                         // SDH 23-08-2006 Planners
RCINDX_REC rcindxrec;                                           // PAB 24-5-2007 Recalls
RECOK_REC recokrec;                                             // PAB 24-5-2007 Recalls
RCSPI_REC rcspirec;                                             // PAB 24-5-2007 Recalls
RECALL_REC recallrec;                                           // PAB 24-5-2007 Recalls
IMSTC_REC imstcrec;

WORD sess;
SHARE_STAT_REC *share;                                                      // Shared memory details
//SMTABLE sharetab;

//LONG min_wf_fnum = 0x00007FFF;                                              // PAB 4.03
//LONG max_wf_fnum = 0x00000000;                                              // PAB 4.03
//UWORD write_gap;                                                            // 22-02-04 PAB

int  CurrentHHT;                             // current HHT Unit
char cStoreClosed;

//FILE * DebugFileHandle = NULL ;
//char SockDir[128] ;
//char HhtLiveLog[128] ;
//char HhtFlagFile[128] ;
//int KeepArchives ;
//char HhtLogExt[14] ;

//int Debug = 0 ; // Off by default
WORD debug;
WORD oput;

//char OutputToFile = 'N';
//char PollControlFile = 'N';

//UBYTE bg;                       // background appl flag for use with disp_msg()

//FILE_CTRL dbg;

//int BreakLoop = 0 ;

const BYTE dayname[7][4] = { "Sun","Mon","Tue","Wed","Thu","Fri","Sat"};    // 12-11-04 PAB

#pragma Data ();

#endif

