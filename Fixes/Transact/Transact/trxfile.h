// -------------------------------------------------------------------------
//                           Boots The Chemists Ltd
//                      Radio Frequency Server Application
//                                Header
// ------------------------------------------------------------------------
// Version 1.0                Stuart Highley              22-Sep-2008
// Created separate module for common file functions.
//
// Version 1.1               Charles Skadorwa           15th Apr 2010
// Changes to support POD Logging Enhancements.
//
// Version 1.2               Charles Skadorwa           12th Mar 2012
// Stock File Accuracy  (commented: //CSk 12-03-2012 SFA)
// Added function prototype: GetFuturePendingSalesplanFlag().
// ------------------------------------------------------------------------
#ifndef TRXFILE_H
#define TRXFILE_H

// 4690 file functions
typedef struct {
   LONG fnum;
   UBYTE present;
   UBYTE sessions;
   BYTE* pbFileName;                            // SDH 26-11-04 OSSR WAN
   WORD  wOpenFlags;                            // SDH 26-11-04 OSSR WAN
   WORD  wReportNum;                            // SDH 26-11-04 OSSR WAN
   void* pBuffer;                               // SDH 26-11-04 OSSR WAN
   WORD  wRecLth;                               // SDH 26-11-04 OSSR WAN
   WORD  wKeyLth;                               // SDH 26-11-04 OSSR WAN
} FILE_CTRL;


LONG u_read(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                 LONG bufsiz, LONG offset, ULONG ulTimeoutMilli);
LONG u_write(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                  LONG bufsiz, LONG offset);
LONG OpenDirectFile(BYTE *fname, UWORD flags,
                    WORD report, BOOLEAN fLogError);
URC keyed_open(FILE_CTRL* pFile, BOOLEAN fLogError);
URC direct_open(FILE_CTRL* pFile, BOOLEAN fLogError);
URC close_file(WORD type, FILE_CTRL* pFile);
LONG ReadDirect(FILE_CTRL* pFile, LONG lRecNum,
                       LONG lLineNumber, WORD wLogLevel);
LONG WriteDirect(FILE_CTRL* pFile, LONG lRecNum,                            // SDH 29-11-04 CREDIT CLAIM
                        LONG lLineNumber, WORD wLogLevel);
LONG CreateKeyedFile(BYTE *pbFilename, WORD wReportNum, ULONG ulBlockCount, //CSk 15-04-2010 POD Logging
                    ULONG ulDivisor, ULONG ulChainThreshold, UBYTE ubHash,
                    UWORD uwRecordSize, UWORD uwKeyLength,
                    UWORD uwDistType, UBYTE fOverwrite);

LONG ReadKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel);
LONG ReadKeyedLock(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel);
LONG WriteKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel);
LONG WriteKeyedUnlock(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel);
URC lrt_log(BYTE type, WORD unit, BYTE *details);
URC log_file_open(LONG filenum, BYTE ftype);
URC log_file_close(LONG filenum);
URC open_citem(void);
URC close_citem(WORD type);
URC open_imstc(void);
URC close_imstc(WORD type);
URC open_cimf(void);
URC close_cimf(WORD type);
URC open_stkmq(void);
URC close_stkmq(WORD type);
URC open_imfp(void);
URC close_imfp(WORD type);
URC open_pllol(void);
URC close_pllol(WORD type);
LONG ReadPllol(LONG lRecNum, LONG lLineNum);
LONG ReadPllolLock(LONG lRecNum, LONG lLineNum);                        //SDH 20-May-2009 Model Day
LONG ReadPllolLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel);
LONG WritePllol(LONG lRecNum, LONG lLineNum);
LONG WritePllolUnlock(LONG lRecNum, LONG lLineNum);                     //SDH 20-May-2009 Model Day
URC open_plldb(void);
URC close_plldb(WORD type);
LONG ReadPlldb(LONG lLineNumber);
LONG ReadPlldbLog(LONG lLineNumber, WORD wLogLevel);
LONG ReadPlldbLock(LONG lLineNumber);
LONG WritePlldb(LONG lLineNumber);
LONG WritePlldbUnlock(LONG lLineNumber);
URC open_minls(void);
URC close_minls(WORD type);
LONG ReadMinls(LONG lLineNumber);
LONG WriteMinls(LONG lLineNumber);
LONG DeleteMinlsRecord(LONG lLineNumber);
URC open_deal(void);
URC close_deal(void);
LONG ReadDeal(LONG lLineNumber);
LONG ReadDealQuick(void);
URC open_tdtff(void);
URC close_tdtff(void);
LONG ReadTDTFFHeader(LONG lLineNumber);
URC open_jobok(void);                                                   // 13-12-2004 PAB
URC close_jobok(WORD type);
LONG open_recok(void);                                                  // 24-5-2007 PAB
URC close_recok(WORD type);
LONG open_rcindx(void);                                                 // 24-5-2007 PAB
URC close_rcindx(WORD type);
URC open_recall(void);
URC close_recall(WORD type);
LONG ReadRecall(LONG lLineNumber);                                      // Mult UOD Rcll 12-08-2011 SDH
LONG WriteRecall(LONG lLineNumber);                                     // Mult UOD Rcll 12-08-2011 SDH
URC open_rcspi(void);                                                   // 25-5-07 PAB
URC close_rcspi(WORD type);
URC open_rfok(void);                                                    // 24-8-2004 PAB
URC close_rfok(WORD type);
URC open_rfrdesc(void);
URC close_rfrdesc(WORD type);
URC open_stock(void);
URC close_stock(WORD type);
LONG ReadStock(LONG lLineNumber);
URC open_tsfD(void);
URC close_tsf(WORD type);
URC open_psbtD(void);
URC close_psbt(WORD type);
URC open_wrfD(void);
URC close_wrf(WORD type);
URC open_epsom(void);
URC close_epsom(WORD type);
URC open_pchk(void);
URC close_pchk(WORD type);
URC open_pilst(void);
URC close_pilst(WORD type);
URC open_piitm(void);
URC close_piitm(WORD type);
URC open_rfsstat(void);
URC close_rfsstat(WORD type);
URC open_suspt(void);
URC close_suspt(WORD type);
URC process_jobok(void);
URC process_rfok(void);
void prepare_logging();
URC alloc_report_buffer( RBUF **rbufp );
URC dealloc_report_buffer( RBUF *rbufp );
URC create_new_plist( BYTE *ret_list_id, BYTE *user );
URC prepare_workfile(WORD log_unit, BYTE type);
LONG filesize( BYTE *fname );
URC pllol_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_PLL *pllp );
URC plldb_get_next( /*WORD log_unit,*/ BYTE *list_id, BYTE *seq, LRT_PLI *plip );
URC clolf_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_CLL *cllp );
URC clilf_get_next( /*WORD log_unit,*/ BYTE *list_id, BYTE *seq, LRT_CLI *clip );
URC GetFuturePendingSalesplanFlag( /*WORD log_unit,*/ BYTE *bItemCode, BYTE *bPspFlag); //CSk 12-03-2012 SFA
URC rfrdesc_get_next( /*WORD log_unit,*/ BYTE *seq, LRT_RLR *rlrp );
URC rfrep_get_next_lev0( WORD log_unit, BYTE *seq,
                         LRT_RLD *rldp, WORD *rec_cnt );
URC rfrep_get_next( WORD log_unit, BYTE *seq,
                    LRT_RUP *rupp, WORD *rec_cnt );
void process_orphans(BOOLEAN fAction);
void CloseAllFiles ( void );

#pragma Data (Common);
PDTASSET_REC pdtassetrec;
#pragma Data ();

URC open_pdtasset( void );
URC close_pdtasset( WORD type );
LONG WritePDTAsset(LONG lLineNumber);


#endif
