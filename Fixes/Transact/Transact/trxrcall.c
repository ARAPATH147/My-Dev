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
//    Moved suspend_transaction to trans03 due to space.
//
// Version 5.9              Brian Greenfield            1st Sept 2008
//    Added bits for ASN/Directs. (Now not in this module due to Streamlining changes!)
//
// Version 5.10              Brian Greenfield           16th Sept 2008
//    Added PDTASSET file.
// Moved the following to trans04:
//   SignOffNak
//   prep_nak
//   prep_ack
//   prep_pq_full_nak
//   IsStoreClosed
//   IsHandheldUnknown
//   UpdateActiveTime
//   IsReportMntActive
//
//  Version 5.11            Brian Greenfield            20th February 2009
//  Corrected code so that the RCINDX size is no longer hard coded!
//
//  Version 5.12            charles skadorwa            13th May 2010
//  Changes to support 2010 Recalls Phase 1 Project
//
//  Version 5.13            Charles Skadorwa            30th Nov 2010
//  Changes to support 2010 Recalls Improvements Project
//  Defect 4806: Fix to RCF processing where recalled item is not on file.
//  Tailored item rules modified to include Stock last count and last
//  delivery date checks.
//
//  Version 5.14            Charles Skadorwa            21st Feb 2011
//       Defect 4982: CR05 - Add Batch Numbers to RCC transaction             // CSk 21-02-2011 Recalls Phase 1 CR05
//       Defect 4985: Expired Recalls should be not be passed to the devices. // CSk 21-02-2011 Recalls Phase 1 Defect 4985
//
//  Version 5.15            Stuart Highley              12 August 2011
//  Mult UOD Rcll changes.
//  Rather than flagging returned items in the RECALLS file, maintain
//  returns lists on CCLOL and CCILF.
//
// -----------------------------------------------------------------------

/* include files */
#include "transact.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>
#include "trxbase.h"                    // Streamline SDH 22-Sep-2008
#include "trxutil.h"                    // Streamline SDH 22-Sep-2008
#include "osfunc.h"                     /* needed for disp_msg() */
#include "trxfile.h"                    // Streamline SDH 22-Sep-2008
#include "rfsfile.h"
#include "sockserv.h"
#include "rfglobal.h"                   // v4.0
#include "osfunc.h"
#include "prtctl.h"                     // Streamline SDH 17-Sep-2008
#include "srfiles.h"
#include "idf.h"
#include "irf.h"
#include "rfscf.h"
#include "ccfiles.h"
#include "invok.h"

// =============================================================        // Streamline SDH 23-Sep-2008
// Module level static variables                                        // Streamline SDH 23-Sep-2008
// =============================================================        // Streamline SDH 23-Sep-2008
                                                                        
static int gRecallFilesAvailable = 0;                                   // Streamline SDH 23-Sep-2008
                                                                        
// =============================================================        // 24-05-07 PAB Recalls
// Recall Procedures follow          24th May 2007   Paul Bowers        // 24-05-07 PAB Recalls
// =============================================================        // 24-05-07 PAB Recalls

URC process_recok(void) {                                               // 24-05-07 PAB Recalls

    LONG rc;                                                            // 24-05-07 PAB Recalls

    // if the recall OK file is not found.                              // 24-05-07 PAB Recalls
    if (open_recok() != RC_OK) {                                        // 24-05-07 PAB Recalls
        prep_nak("ERRORUnable to open RECOK file. "                     // 24-05-07 PAB Recalls
                 "Check appl event logs" );                             // 24-05-07 PAB Recalls
        return RC_FILE_ERR;                                             // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
                                                                        
    gRecallFilesAvailable = 0;                                          // 24-05-07 PAB Recalls

    // read the current status record & close the file                  // 24-05-07 PAB Recalls
    rc = s_read(A_BOFOFF,recok.fnum,(void *)&recokrec,RECOK_RECL,0L);   // 24-05-07 PAB Recalls
    close_recok(CL_SESSION);                                            // 24-05-07 PAB Recalls

    if (rc <= 0L) {                                                     // 24-05-07 PAB Recalls
        log_event101(rc, RFOK_REP, __LINE__);                           // 24-05-07 PAB Recalls
        sprintf(sbuf, "ERR-R RECOK. RC:%081X", rc);                     // Streamline SDH 24-Sep-2008
        disp_msg(sbuf);                                                 // Streamline SDH 24-Sep-2008
        prep_nak("ERRORRECOK read error. Check appl event logs");       // Streamline SDH 24-Sep-2008
        return RC_DATA_ERR;                                             // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
                                                                        
    return RC_OK;                                                       // 24-05-07 PAB Recalls
}                                                                       // 24-05-07 PAB Recalls

////////////////////////////////////////////////////////////////////////////////
/// CheckRecallAvailable
/// Private function to determine whether recall processing is available
/// at this time.
////////////////////////////////////////////////////////////////////////////////
static LONG CheckRecallAvailable(void ) {
    // check status of recok record                                     // 24-05-07 PAB Recalls
    if (process_recok() != RC_OK) return 0;                             // 24-05-07 PAB Recalls
    // if the recall files are not available.                           // 24-05-07 PAB Recalls
    if (gRecallFilesAvailable == 1) {                                   // 24-05-07 PAB Recalls
        prep_nak("ERRORRecall Files are not available. Please try later.");// 25-05-07 PAB Recalls
        return 1;                                                       // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
    return RC_OK;                                                       // 24-05-07 PAB Recalls
}

////////////////////////////////////////////////////////////////////////// Mult UOD Rcll 12-8-2011 SDH
/// AllocateCclol                                                       // Mult UOD Rcll 12-8-2011 SDH
/// Private function to allocate a new CCLOL list.                      // Mult UOD Rcll 12-8-2011 SDH
/// Returns FALSE if no error, TRUE if error.                           // Mult UOD Rcll 12-8-2011 SDH
////////////////////////////////////////////////////////////////////////// Mult UOD Rcll 12-8-2011 SDH
static BOOLEAN AllocateCclol(void) {                                    // Mult UOD Rcll 12-8-2011 SDH

    //Set up current date and time                                      // Mult UOD Rcll 12-8-2011 SDH
    B_DATE nowD;                                                        // Mult UOD Rcll 12-8-2011 SDH
    B_TIME nowT;                                                        // Mult UOD Rcll 12-8-2011 SDH
    GetSystemDate(&nowT, &nowD);                                        // Mult UOD Rcll 12-8-2011 SDH

    //Read CCLOL header and handle errors                               // Mult UOD Rcll 12-8-2011 SDH
    if (CclolRead(0, __LINE__) <= 0L) {                                 // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        return TRUE;                                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    //Get the list ID and increment for the next available list ID.     // Mult UOD Rcll 12-8-2011 SDH
    //This code assumes that the file does not wrap, and that           // Mult UOD Rcll 12-8-2011 SDH
    //the number of records will not reach 32,767                       // Mult UOD Rcll 12-8-2011 SDH
    WORD wListNum = ARRAY_TO_WORD(cclolrec.abListNum) + 1;              // Mult UOD Rcll 12-8-2011 SDH
                                                                        
    //Build new CCLOL list record                                       // Mult UOD Rcll 12-8-2011 SDH
    memset(&cclolrec, 0x00, sizeof(CCLOL_REC));                         // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(cclolrec.abListNum, wListNum);                        // Mult UOD Rcll 12-8-2011 SDH
    cclolrec.cListType = 'G';                                           // Mult UOD Rcll 12-8-2011 SDH
    sprintf(sbuf, "%04d%02d%02d", nowD.wYear, nowD.wMonth, nowD.wDay);  // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(cclolrec.abDateUODOpened, sbuf);                             // Mult UOD Rcll 12-8-2011 SDH
    sprintf(sbuf, "%02d%02d", nowT.wHour, nowT.wMin);                   // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(cclolrec.abTimeUODOpened, sbuf);                             // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(cclolrec.abOpID, lrtp[hh_unit]->user);                       // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(cclolrec.abOpName, lrtp[hh_unit]->abOpName);                 // Mult UOD Rcll 12-8-2011 SDH

    //Write record and handle errors                                    // Mult UOD Rcll 12-8-2011 SDH
    if (CclolWrite(wListNum, __LINE__) <= 0L) {                         // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        return TRUE;                                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    //Increment the count on the header file and handle errors          // Mult UOD Rcll 12-8-2011 SDH
    memcpy(sbuf, cclolrec.abListNum, sizeof(cclolrec.abListNum));       // Mult UOD Rcll 12-8-2011 SDH
    memset(&cclolrec, 0x00, sizeof(CCLOL_REC));                         // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(cclolrec.abListNum, sbuf);                                   // Mult UOD Rcll 12-8-2011 SDH
    if (CclolWrite(0, __LINE__) <= 0L) {                                // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        return TRUE;                                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    //Store reference to the CCLOL list number                          // Mult UOD Rcll 12-8-2011 SDH
    lrtp[hh_unit]->wRecallCclol = wListNum;                             // Mult UOD Rcll 12-8-2011 SDH

    return FALSE;                                                       // Mult UOD Rcll 12-8-2011 SDH

}                                                                       // Mult UOD Rcll 12-8-2011 SDH


////////////////////////////////////////////////////////////////////////// Mult UOD Rcll 12-8-2011 SDH
/// ProcessCcilf                                                        // Mult UOD Rcll 12-8-2011 SDH
/// Write a new CCILF record                                            // Mult UOD Rcll 12-8-2011 SDH
/// Returns FALSE if no error, TRUE if error.                           // Mult UOD Rcll 12-8-2011 SDH
////////////////////////////////////////////////////////////////////////// Mult UOD Rcll 12-8-2011 SDH

static BOOLEAN ProcessCcilf(WORD wListNum, WORD wRecSeq,                // Mult UOD Rcll 12-8-2011 SDH
                            BYTE *pItemCode6, WORD wQty) {              // Mult UOD Rcll 12-8-2011 SDH

    //Set up current date and time                                      // Mult UOD Rcll 12-8-2011 SDH
    B_DATE nowD;                                                        // Mult UOD Rcll 12-8-2011 SDH
    B_TIME nowT;                                                        // Mult UOD Rcll 12-8-2011 SDH
    GetSystemDate(&nowT, &nowD);                                        // Mult UOD Rcll 12-8-2011 SDH

    WORD_TO_ARRAY(ccilfrec.abListNum, wListNum);                        // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abRecSeq, wRecSeq);                          // Mult UOD Rcll 12-8-2011 SDH 
    AddBootsCheck(ccilfrec.abItemCode, pItemCode6);                     // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abBarCode, '0');                                    // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abItemDesc, ' ');                                   // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abSelDesc, ' ');                                    // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abItemQty, wQty);                            // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abFiller1, ' ');                                    // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abItemPrice, ' ');                                  // Mult UOD Rcll 12-8-2011 SDH
    ccilfrec.cBusCentre = ' ';                                          // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abBusCentreDesc, ' ');                              // Mult UOD Rcll 12-8-2011 SDH
    sprintf(sbuf, "%04d%02d%02d", nowD.wYear,nowD.wMonth, nowD.wDay);   // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(ccilfrec.abDateAdded, sbuf);                                 // Mult UOD Rcll 12-8-2011 SDH
    sprintf(sbuf, "%02d%02d", nowT.wHour, nowT.wMin);                   // Mult UOD Rcll 12-8-2011 SDH
    MEMCPY(ccilfrec.abTimeAdded, sbuf);                                 // Mult UOD Rcll 12-8-2011 SDH
    MEMSET(ccilfrec.abFiller2, ' ');                                    // Mult UOD Rcll 12-8-2011 SDH
    ccilfrec.cItemStatus = 'A';                                         // Mult UOD Rcll 12-8-2011 SDH
    if (CcilfWrite(__LINE__) <= 0L) {                                   // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("ERRORUnable to write to CCILF. Check appl event log");// Mult UOD Rcll 12-8-2011 SDH
        return TRUE;                                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    return FALSE;                                                       // Mult UOD Rcll 12-8-2011 SDH

}                                                                       // Mult UOD Rcll 12-8-2011 SDH


////////////////////////////////////////////////////////////////////////////////
/// RecallStart
////////////////////////////////////////////////////////////////////////////////

void RecallStart(char *inbound) {                                       // 24-05-07 PAB Recalls

    LONG usrrc = RC_OK;
    UNUSED(inbound);                                                    // 24-05-07 PAB Recalls
                                                                        
    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    // Open the recall files                                            // 24-05-07 PAB Recalls
    usrrc = open_recall();                                              // 24-05-07 PAB Recalls
    if (usrrc != RC_OK) {                                               // 24-05-07 PAB Recalls
        disp_msg("RECALL file not found");                              // 24-05-07 PAB Recalls
        prep_nak("There are no Head Office recalls "
                 "to be actioned at this time.");
        return;                                                         // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
    usrrc = open_rcspi();                                               // 24-05-07 PAB Recalls
    if (usrrc != RC_OK) {                                               // 24-05-07 PAB Recalls
        disp_msg("RCSPI file not found");                               // 24-05-07 PAB Recalls
        prep_nak("There are no Head Office recalls "
                 "to be actioned at this time.");
        close_recall(CL_SESSION);
        return;                                                         // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
    usrrc = open_rcindx();                                              // 24-05-07 PAB Recalls
    if (usrrc != RC_OK) {                                               // 24-05-07 PAB Recalls
        disp_msg("RCINDX file not found");                              // 24-05-07 PAB Recalls
        prep_nak("There are no Head Office recalls "
                 "to be actioned at this time.");
        close_recall(CL_SESSION);
        close_rcspi(CL_SESSION);
        return;                                                         // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
    //Open the credit claiming files for this session                   // Mult UOD Rcll 12-8-2011 SDH
    if (CchistOpen() != RC_OK) {                                        // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        close_recall(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        close_rcspi(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        close_rcindx(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        return;                                                         // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH
    if (CclolOpen() != RC_OK) {                                         // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        close_recall(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        close_rcspi(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        close_rcindx(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        CchistClose(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        return;                                                         // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH
    if (CcilfOpen() != RC_OK) {                                         // Mult UOD Rcll 12-8-2011 SDH
        prep_nak("There are no Head Office recalls "                    // Mult UOD Rcll 12-8-2011 SDH
                 "to be actioned at this time.");                       // Mult UOD Rcll 12-8-2011 SDH
        close_recall(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        close_rcspi(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        close_rcindx(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        CchistClose(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        CclolClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        return;                                                         // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    // Read first RCINDX record. Return error if no record found        // 07-09-2007 5.3 BMG
    usrrc = s_read(A_BOFOFF, rcindx.fnum,                               // 07-09-2007 5.3 BMG
                   (void *)&rcindxrec, RCINDX_RECL, 0);                 // 07-09-2007 5.3 BMG
    if (usrrc <= RC_OK) {                                               // 07-09-2007 5.3 BMG
        disp_msg("RCINDX is empty");                                    // 07-09-2007 5.3 BMG
        prep_nak("There are no Head Office recalls "
                 "to be actioned at this time.");                       // 07-09-2007 5.3 BMG
        close_recall(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        close_rcspi(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        close_rcindx(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        CchistClose(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        CclolClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        CcilfClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        return;                                                         // 07-09-2007 5.3 BMG
    }                                                                   // 07-09-2007 5.3 BMG
                                                                        
    //Allocate a new CCLOL list                                         // Mult UOD Rcll 12-8-2011 SDH
    if (AllocateCclol()) {
        close_recall(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        close_rcspi(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        close_rcindx(CL_SESSION);                                       // Mult UOD Rcll 12-8-2011 SDH
        CchistClose(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        CclolClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        CcilfClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        return;                                                         // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    // Recall files available ok send acknowledgement                   // 24-05-07 PAB Recalls
    prep_ack("");                                                       // 24-05-07 PAB Recalls

}                                                                       // 24-05-07 PAB Recalls


////////////////////////////////////////////////////////////////////////////////
/// RecallExit
////////////////////////////////////////////////////////////////////////////////

typedef struct LRT_RCB_Txn {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE acRecallNum[8];                                                // Mult UOD Rcll 12-8-2011 SDH
    BYTE acUODNum[14];                                                  // Mult UOD Rcll 12-8-2011 SDH
    BYTE cListStatus;   //P-Partial A-Actioned X-Save counts            // Mult UOD Rcll 12-8-2011 SDH
} LRT_RCB;                                    
#define LRT_RCB_LTH sizeof(LRT_RCB)

void RecallExit(char *inbound) {                                        // 24-05-07 PAB Recalls
                                                                        // 24-05-07 PAB Recalls
    LRT_RCB* pRCB = (LRT_RCB*)inbound;

    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    //Set up current date and time                                      // Mult UOD Rcll 12-8-2011 SDH
    B_DATE nowD;                                                        // Mult UOD Rcll 12-8-2011 SDH
    B_TIME nowT;                                                        // Mult UOD Rcll 12-8-2011 SDH
    GetSystemDate(&nowT, &nowD);                                        // Mult UOD Rcll 12-8-2011 SDH

    // if status not "X", then "P" or "A", so update and despatch recall// Mult UOD Rcll 12-8-2011 SDH
    if (pRCB->cListStatus != 'X') {                                     // Mult UOD Rcll 12-8-2011 SDH

        //Read and update RECALL record                                 // Mult UOD Rcll 12-8-2011 SDH
        MEMCPY(recallrec.acRecallID, pRCB->acRecallNum);                // Mult UOD Rcll 12-8-2011 SDH
        recallrec.ubChain = 0;                                          // 24-05-07 PAB Recalls
        if (ReadRecall(__LINE__) <= 0L) {                               // Mult UOD Rcll 12-8-2011 SDH
            prep_nak("ERRORThis Recall could not be read from "         // Mult UOD Rcll 12-8-2011 SDH
                     "Recall file. Check appl event logs" );            // Mult UOD Rcll 12-8-2011 SDH
            return;                                                     // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH
        recallrec.cRecallStatus = pRCB->cListStatus;                    // Mult UOD Rcll 12-8-2011 SDH
        if (WriteRecall(__LINE__) <= 0L) {                              // Mult UOD Rcll 12-8-2011 SDH
            prep_nak("ERRORThere was a failure updating the"            // 24-05-07 PAB Recalls
                     "Recall file. Check appl event logs");             // 24-05-07 PAB Recalls
            return;                                                     // 24-05-07 PAB Recalls
        }                                                               // 24-05-07 PAB Recalls

        //Count items in the recall UOD                                 // Mult UOD Rcll 12-8-2011 SDH
        WORD_TO_ARRAY(ccilfrec.abListNum, lrtp[hh_unit]->wRecallCclol); // Mult UOD Rcll 12-8-2011 SDH
        WORD wRecSeq = 1;                                               // Mult UOD Rcll 12-8-2011 SDH
        WORD_TO_ARRAY(ccilfrec.abRecSeq, wRecSeq++);                    // Mult UOD Rcll 12-8-2011 SDH
        while (CcilfRead(__LINE__) > 0L) {                              // Mult UOD Rcll 12-8-2011 SDH
            WORD_TO_ARRAY(ccilfrec.abRecSeq, wRecSeq++);                // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH
        wRecSeq = wRecSeq - 2;                                          // Mult UOD Rcll 12-8-2011 SDH

        //Read CCLOL record                                             // Mult UOD Rcll 12-8-2011 SDH
        if (CclolRead(lrtp[hh_unit]->wRecallCclol, __LINE__) <= 0L) {   // Mult UOD Rcll 12-8-2011 SDH
            prep_nak("ERRORThis Recall could not be read from "         // Mult UOD Rcll 12-8-2011 SDH
                     "CCLOL file. Check appl event logs");              // Mult UOD Rcll 12-8-2011 SDH
            return;                                                     // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH

        //Update CCLOL record                                           // Mult UOD Rcll 12-8-2011 SDH
        MEMCPY(cclolrec.abUODNum, pRCB->acUODNum);                      // Mult UOD Rcll 12-8-2011 SDH
        WORD_TO_ARRAY(cclolrec.abItemCount, wRecSeq);                   // Mult UOD Rcll 12-8-2011 SDH
        MEMCPY(cclolrec.abUODQty, cclolrec.abItemCount);                // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cAdjStockFig = 'Y';                                    // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cSupplyRoute = recallrec.cSupplyRoute;                 // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cDispLocation = ' ';                                   // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cBusCentre = recallrec.cBusinessCentre;                // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abBusCentreDesc, ' ');                          // Mult UOD Rcll 12-8-2011 SDH
        MEMCPY(cclolrec.abRecallNum, pRCB->acRecallNum);                // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abAuth, ' ');                                   // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abSupplier, ' ');                               // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abReturnMethod, '0');                           // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abCarrier, '0');                                // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abBirdNum, ' ');                                // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.abReasonNum[0] = '0';                                  // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.abReasonNum[1] = recallrec.cReasonCode;                // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abReceivingStoreNum, '0');                      // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abDestination, '0');                            // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cWarehouseRoute = ' ';                                 // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abUODType, '0');                                // Mult UOD Rcll 12-8-2011 SDH
        MEMSET(cclolrec.abDamageRsn, '0');                              // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cRFStatus = 'C';                                       // Mult UOD Rcll 12-8-2011 SDH
        sprintf(sbuf, "%04d%02d%02d", nowD.wYear,nowD.wMonth,nowD.wDay);// Mult UOD Rcll 12-8-2011 SDH 
        MEMCPY(cclolrec.abDateUODDispatched, sbuf);                     // Mult UOD Rcll 12-8-2011 SDH
        cclolrec.cStatus = 'D';                                         // Mult UOD Rcll 12-8-2011 SDH
        if (CclolWrite(lrtp[hh_unit]->wRecallCclol, __LINE__) <= 0L) {  // Mult UOD Rcll 12-8-2011 SDH
            prep_nak("ERRORThere was a failure updating the "           // Mult UOD Rcll 12-8-2011 SDH
                     "CCLOL file. Check appl event logs");              // Mult UOD Rcll 12-8-2011 SDH
            return;                                                     // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH

        //Kick off RFSCC, passing the CCLOL list num                    // Mult UOD Rcll 12-8-2011 SDH
        sprintf(sbuf, "%04d -R", lrtp[hh_unit]->wRecallCclol);          // Mult UOD Rcll 12-8-2011 SDH
        if (start_background_app("ADX_UPGM:RFSCC.286", sbuf,            // Mult UOD Rcll 12-8-2011 SDH
                                 "Despatching Recall") < 0) {           // Mult UOD Rcll 12-8-2011 SDH                                     // 24-05-07 PAB Recalls
            prep_nak("ERRORUnable to start RFSCC to despatch "          // Mult UOD Rcll 12-8-2011 SDH
                     "this Recall. Please try again ");                 // Mult UOD Rcll 12-8-2011 SDH
            return;                                                     // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH

    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    //Remove reference to CCLOL list                                    // Mult UOD Rcll 12-8-2011 SDH
    lrtp[hh_unit]->wRecallCclol = 0;                                    // Mult UOD Rcll 12-8-2011 SDH

    // Only close files if exiting Recalls. If Partial then             // Mult UOD Rcll 12-8-2011 SDH
    // Multiple UODs can be requested for the same Recall Ref.          // Mult UOD Rcll 12-8-2011 SDH
    // NOTE: The POD erroneously sends a RecallStart after a RecallExit // Mult UOD Rcll 12-8-2011 SDH
    // (partial), causing multiple logical sessions to be opened.       // Mult UOD Rcll 12-8-2011 SDH
    if (pRCB->cListStatus != 'P') {                                     // Mult UOD Rcll 12-8-2011 SDH
        close_recall(CL_SESSION);                                       // 24-05-07 PAB Recalls
        close_rcindx(CL_ALL);                                           // 07-09-2007 5.4 BMG
        close_rcspi(CL_ALL);                                            // 07-09-2007 5.4 BMG
        CchistClose(CL_SESSION);                                        // Mult UOD Rcll 12-8-2011 SDH
        CclolClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
        CcilfClose(CL_SESSION);                                         // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    prep_ack("");                                                       // 24-05-07 PAB Recalls
    return;                                                             // 24-05-07 PAB Recalls

}                                                                       // 24-05-07 PAB Recalls


////////////////////////////////////////////////////////////////////////////////
/// RecallListRequest
////////////////////////////////////////////////////////////////////////////////

typedef struct LRT_RCC_Txn {
    BYTE cmd[3];
    BYTE opid[3];
    BYTE Index[4];
    BYTE acRecallID[8];                               
    BYTE cRecallType;
    BYTE abRecallDesc[20];
    BYTE anRecallCnt[4];
    BYTE cActiveDate[8]; // YYYYMMDD
    BYTE cMsgAvail;      // "Y" is msg available on Recall Msg File.
    BYTE cLabelType[2];
    BYTE cMRQ[2];        // 2 digit Minimum Return Quantity             // 05-05-2008 1.4 BMG
    BYTE cRecallStatus;  // A-Actioned P-Partially actioned N-Not actioned // CSk 13-05-2010 Recalls Phase 1
    BYTE cTailored;      // Y = Tailored List, N = Not Tailored         // CSk 13-05-2010 Recalls Phase 1
    BYTE abBatchNos[30]; // Batch Nos for Batch recalls (space filled)  // CSk 21-02-2011 Recalls Phase 1 CR05
} LRT_RCC;                                    
#define LRT_RCC_LTH sizeof(LRT_RCC)

typedef struct LRT_RCD_Txn {
    BYTE cmd[3];
    BYTE opid[3];
    BYTE anIndex[4];                             
    BYTE cRecallType;
} LRT_RCD;                                    
#define LRT_RCD_LTH sizeof(LRT_RCD)

typedef struct LRT_RCE_Txn {
    BYTE cmd[3];
    BYTE opid[3];                            
} LRT_RCE;                                    
#define LRT_RCE_LTH sizeof(LRT_RCE)

void RecallListRequest(char *inbound) {                                 // 24-05-07 PAB Recalls

    LRT_RCD* pRCD = (LRT_RCD*)inbound;                                  // 31-05-07 PAB Recalls
    LRT_RCE* pRCE = (LRT_RCE*)out;                                      // 31-05-07 PAB Recalls
    LRT_RCC* pRCC = (LRT_RCC*)out;                                      // 31-05-07 PAB Recalls

    LONG usrrc = RC_OK;                                                 // 24-05-07 PAB Recalls
    LONG lIndexPtr = 0;                                                 // 29-05-07 PAB Recalls
    int iExpired = 0;                                                   // 14-04-2008 5.6 BMG
    int iBypass = 0;                                                    // 30-04-2008 5.7 BMG
    DOUBLE dRecallExpiryDate;                                           // 14-04-2008 5.6 BMG
    DOUBLE dTodayDate;                                                  // 14-04-2008 5.6 BMG
    WORD hour, min;                                                     // 14-04-2008 5.6 BMG
    LONG sec, day, month, year;                                         // 14-04-2008 5.6 BMG

    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    // The HHT will make a number of RCD requests to get the recalls of
    // the requested type. Each RCD contains a record pointer into the
    // RCINDX file which gets incremented each time until EOF or match 
    // found. If a match not found, the RCINDX is scanned until the next
    // match is found or EOF. The RCC contains the pointer to the current
    // position in the RCINDX so the next RCD request can continue 
    // reading. The trace will display ALL the recalls read from the
    // RCINDX file - not just the requested ones! 

    lIndexPtr = satoi(pRCD->anIndex, 4);                                // 29-05-07 PAB Recalls
    lIndexPtr = lIndexPtr * RCINDX_RECL;                                // 14-04-2008 5.6 BMG
                                                                        // 29-05-07 PAB Recalls
    // read the current status record & close the file                  // 24-05-07 PAB Recalls
    usrrc = s_read( A_BOFOFF, rcindx.fnum, (void *)&rcindxrec,          // 29-05-07 PAB Recalls
                    RCINDX_RECL, lIndexPtr);                            // 24-05-07 PAB Recalls

    sprintf(msg, "Read from RCINDX. RC:%08lX recall status %c recall "  // 30-04-2008 5.7 BMG
                "type %c record index %d", usrrc, rcindxrec.cRecallStatus, // 30-04-2008 5.7 BMG
                rcindxrec.cRecallType, lIndexPtr);                      // 30-04-2008 5.7 BMG
    disp_msg(msg);                                                      // 31-05-07 PAB Recalls

    if (usrrc > RC_OK) {                                                               // CSk 13-05-2010 Recalls Phase 1 
        // Checking expiry date for all Recall types                                   // CSk 30-11-2010 Recall Improvements  
        //                          RULES                                              // CSk 30-11-2010 Recall Improvements
        //                          -----                                              // CSk 30-11-2010 Recall Improvements
        // For Customer-Emergency & Withdrawn recalls then expiry date + 28 days       // CSk 30-11-2010 Recall Improvements
        // For Excess Salesplan & Planner Leaver then use actual expiry date as this   // CSk 21-02-2011 Recalls Phase 1 Defect 4985
        //     already has +14 days built in.                                          // CSk 21-02-2011 Recalls Phase 1 Defect 4985
        // For 100% Returns and 100% Batch Returns now operate the same as             // CSk 21-02-2011 Recalls Phase 1 Defect 4985
        //     Ex.Salesplan & Planner Leaver (ie. already has +14 days built in.       // CSk 21-02-2011 Recalls Phase 1 Defect 4985
        sysdate( &day, &month, &year, &hour, &min, &sec );                             // CSk 30-11-2010 Recall Improvements 
        dTodayDate = ConvGJ( day, month, year );                                       // CSk 30-11-2010 Recall Improvements 
        unpack( sbuf, 8, rcindxrec.abExpiryDate, 4, 0 );                               // CSk 30-11-2010 Recall Improvements  
        day   = satol( sbuf+6, 2 );                                                    // CSk 30-11-2010 Recall Improvements  
        month = satol( sbuf+4, 2 );                                                    // CSk 30-11-2010 Recall Improvements  
        year  = satol( sbuf,   4 );                                                    // CSk 30-11-2010 Recall Improvements  
        dRecallExpiryDate = ConvGJ( day, month, year );                                // CSk 30-11-2010 Recall Improvements 
                                                                                       // CSk 30-11-2010 Recall Improvements
        iExpired = 1; // set default to NOT expired                                    // CSk 21-02-2011 Recalls Phase 1 Defect 4985  
        // CSk 30-11-2010 Recall Improvements
        if ((rcindxrec.cRecallType == 'C') || (rcindxrec.cRecallType == 'I') ||        // CSk 21-02-2011 Recalls Phase 1 Defect 4985
            (rcindxrec.cRecallType == 'R') || (rcindxrec.cRecallType == 'S')) {        // CSk 21-02-2011 Recalls Phase 1 Defect 4985
            // Excess Salesplan or Planner Leaver                                      // CSk 30-11-2010 Recall Improvements
            if ((dRecallExpiryDate - dTodayDate) >= 0) iExpired = 0;                   // CSk 30-11-2010 Recall Improvements
        } else { // 28 day rule applies                                                // CSk 21-02-2011 Recalls Phase 1 Defect 4985
            // Customer Emergency or Withdrawn or Batch Customer or Batch Withdrawn    // CSk 30-11-2010 Recall Improvements
            //     E              or     W     or      F         or      X             // CSk 30-11-2010 Recall Improvements
            if ((dRecallExpiryDate+28 - dTodayDate) >= 0) iExpired = 0;                // CSk 30-11-2010 Recall Improvements
        }                                                                              // CSk 30-11-2010 Recall Improvements
        /*If head office recall requested but we have an I or C, bypass this record*/  // 30-04-2008 5.7 BMG
        if (pRCD->cRecallType == '*') {                                                // 30-04-2008 5.7 BMG
            if (rcindxrec.cRecallType == 'C' || rcindxrec.cRecallType == 'I') {        // 30-04-2008 5.7 BMG
                iBypass = 1;                                                           // 30-04-2008 5.7 BMG
            }                                                                          // 30-04-2008 5.7 BMG
        } else { /*An I or C was requested to bypass of we don't have a match */       // 30-04-2008 5.7 BMG
            if (pRCD->cRecallType != rcindxrec.cRecallType) {                          // 30-04-2008 5.7 BMG
                iBypass = 1;                                                           // 30-04-2008 5.7 BMG
            }                                                                          // 30-04-2008 5.7 BMG
        }                                                                              // 30-04-2008 5.7 BMG
    } // end of if (usrrc > RC_OK)                                                      // CSk 13-05-2010 Recalls Phase 1 

    if ((iExpired || iBypass) && (usrrc > RC_OK)) {  // If NOT expired or Bypass....    // CSk 21-02-2011 Recalls Phase 1 Defect 4985
        // Recall just read does not meet criteria so continue scanning through the     // CSk 13-05-2010 Recalls Phase 1
        // RCINDX file until you find the next non-expired/bypassed recall              // CSk 13-05-2010 Recalls Phase 1
        while ((iExpired || iBypass) && (usrrc > RC_OK)) {                              // CSk 13-05-2010 Recalls Phase 1
            lIndexPtr = lIndexPtr + RCINDX_RECL;
            usrrc = s_read(A_BOFOFF, rcindx.fnum, (void *)&rcindxrec,   // 29-05-07 PAB Recalls
                           RCINDX_RECL, lIndexPtr);                     // 24-05-07 PAB Recalls

            if (usrrc > RC_OK) {                                                               // CSk 13-05-2010 Recalls Phase 1
                sysdate( &day, &month, &year, &hour, &min, &sec );                             // CSk 30-11-2010 Recall Improvements 
                dTodayDate = ConvGJ( day, month, year );                                       // CSk 30-11-2010 Recall Improvements 
                unpack( sbuf, 8, rcindxrec.abExpiryDate, 4, 0 );                               // CSk 30-11-2010 Recall Improvements  
                day   = satol( sbuf+6, 2 );                                                    // CSk 30-11-2010 Recall Improvements  
                month = satol( sbuf+4, 2 );                                                    // CSk 30-11-2010 Recall Improvements  
                year  = satol( sbuf,   4 );                                                    // CSk 30-11-2010 Recall Improvements  
                dRecallExpiryDate = ConvGJ( day, month, year );                                // CSk 30-11-2010 Recall Improvements 
                                                                                               // CSk 30-11-2010 Recall Improvements
                iExpired = 1; // set default to expired                                        // CSk 30-11-2010 Recall Improvements  
                // CSk 30-11-2010 Recall Improvements
                if ((rcindxrec.cRecallType == 'C') || (rcindxrec.cRecallType == 'I') ||        // CSk 21-02-2011 Recalls Phase 1 Defect 4985
                    (rcindxrec.cRecallType == 'R') || (rcindxrec.cRecallType == 'S')) {        // CSk 21-02-2011 Recalls Phase 1 Defect 4985
                    // Excess Salesplan or Planner Leaver or 100% Returns or 100% Batch Returns// CSk 30-11-2010 Recall Improvements
                    if ((dRecallExpiryDate - dTodayDate) >= 0) iExpired = 0;                   // CSk 30-11-2010 Recall Improvements
                } else {  // 28 day rule applies                                               // CSk 21-02-2011 Recalls Phase 1 Defect 4985
                    // Customer Emergency or Withdrawn or Batch Customer or Batch Withdrawn    // CSk 30-11-2010 Recall Improvements
                    //     E              or     W     or      F         or      X             // CSk 30-11-2010 Recall Improvements
                    if ((dRecallExpiryDate+28 - dTodayDate) >= 0) iExpired = 0;                // CSk 30-11-2010 Recall Improvements
                }                                                                              // CSk 30-11-2010 Recall Improvements

                /*If head office recall requested but we have an I or C, bypass this record*/// 30-04-2008 5.7 BMG
                iBypass = 0;                                                                 // 30-04-2008 5.7 BMG
                if (pRCD->cRecallType == '*') {                                              // 30-04-2008 5.7 BMG
                    if ((rcindxrec.cRecallType == 'C') ||                                    // 30-04-2008 5.7 BMG
                        (rcindxrec.cRecallType == 'I')) iBypass = 1;                         // 30-04-2008 5.7 BMG
                } else { /*An I or C was requested to bypass of we don't have a match */     // 30-04-2008 5.7 BMG
                    if (pRCD->cRecallType != rcindxrec.cRecallType) iBypass = 1;             // 30-04-2008 5.7 BMG
                }                                                                            // 30-04-2008 5.7 BMG
            } // end of if (usrrc > RC_OK)                                                   // CSk 30-11-2010 Recall Improvements 

            if (usrrc < RC_OK) {
                // end of index file reached
                sprintf(msg, "Search RCINDX. RC:%08lX record index %d EOF RCINDX", usrrc,lIndexPtr); // 30-04-2008 5.7 BMG
                disp_msg(msg);                                                                       // 30-04-2008 5.7 BMG
                //break;
            } else {
                sprintf(msg, "Search RCINDX. RC:%08lX recall "                  // 14-04-2008 5.6 BMG
                        "status %c recall type %c record index %d",             // 14-04-2008 5.6 BMG
                        usrrc, rcindxrec.cRecallStatus,                         // 14-04-2008 5.6 BMG
                        rcindxrec.cRecallType,lIndexPtr);                       // 14-04-2008 5.6 BMG
                disp_msg(msg);                                                  // 31-05-07 PAB Recalls
                if (iExpired) disp_msg("               RECALL IS EXPIRED");     // 14-04-2008 5.6 BMG
                if (iBypass) disp_msg("               RECALL IS WRONG TYPE");   // 30-04-2008 5.7 BMG
            }                                                                   
        } //end of while
    } // end of if iExpired || iBypass

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
    lIndexPtr = (lIndexPtr / RCINDX_RECL);                              // 20-02-2009 5.1 BMG
    LONG_TO_ARRAY(pRCC->Index, lIndexPtr);
    unpack(pRCC->acRecallID, 8, rcindxrec.acRecallID, 4, 0);
    pRCC->cRecallType = rcindxrec.cRecallType;
    memcpy(pRCC->abRecallDesc,rcindxrec.abRecallDesc,sizeof(pRCC->abRecallDesc));
    unpack(pRCC->anRecallCnt,4,rcindxrec.abItemCount,2,0);
    /* If we have a type I or type C, write the due by date into the active */// 30-04-2008 5.7 BMG
    /* date field to make PPC processing easier. */                     // 30-04-2008 5.7 BMG
    // 3 lines commented out                                            // CSk 30-11-2010 Recall Improvements
    //if ( ((memcmp(rcindxrec.cRecallType,"C",1) == 0) || (memcmp(rcindxrec.cRecallType,"I",1) == 0)) ) { // 30-04-2008 5.7 BMG
    //    unpack(pRCC->cActiveDate,8,rcindxrec.abExpiryDate,4,0);       // 30-04-2008 5.7 BMG
    //} else unpack(pRCC->cActiveDate,8,rcindxrec.abActiveDate,4,0);    // 30-04-2008 5.7 BMG
    // Copy Expiry Date to Active Date for all recall types             // CSk 30-11-2010 Recall Improvements
    unpack(pRCC->cActiveDate,8,rcindxrec.abExpiryDate,4,0);             // CSk 30-11-2010 Recall Improvements
    pRCC->cMsgAvail = rcindxrec.cSpecialIns;
    memcpy(pRCC->cLabelType, rcindxrec.abLabelType, 2);
    memcpy(pRCC->cMRQ, rcindxrec.cMRQ, 2);                              // 30-04-2008 5.7 BMG

    pRCC->cRecallStatus = rcindxrec.cRecallStatus;                      // CSk 13-05-2010 Recalls Phase 1
    pRCC->cTailored = 'N';                                              // CSk 13-05-2010 Recalls Phase 1
                                                                        // CSk 13-05-2010 Recalls Phase 1
    if (rcindxrec.cRecallType == 'R' || //100% Returns                  // CSk 13-05-2010 Recalls Phase 1
        rcindxrec.cRecallType == 'S') { //100% Batch Returns            // CSk 13-05-2010 Recalls Phase 1
        // if does NOT start with NT*                                   // CSk 13-05-2010 Recalls Phase 1
        if (strncmp(rcindxrec.abRecallDesc, "NT*", 3)) {                // CSk 13-05-2010 Recalls Phase 1
                pRCC->cTailored = 'Y';                                  // CSk 13-05-2010 Recalls Phase 1
        }
    }                                                                   // CSk 13-05-2010 Recalls Phase 1

    // Add Recall Batch Numbers (Batch Recall Types ONLY)               // CSk 21-02-2011 Recalls Phase 1 CR05
    if (rcindxrec.cRecallType == 'F' ||                                 // CSk 21-02-2011 Recalls Phase 1 CR05
        rcindxrec.cRecallType == 'X' ||                                 // CSk 21-02-2011 Recalls Phase 1 CR05
        rcindxrec.cRecallType == 'S') {                                 // CSk 21-02-2011 Recalls Phase 1 CR05

        // Batch Recall - check status of recall files                  // CSk 21-02-2011 Recalls Phase 1 CR05
        if (CheckRecallAvailable() != 0) return;                        // CSk 21-02-2011 Recalls Phase 1 CR05

        // read the first chain and get batch numbers                   // CSk 21-02-2011 Recalls Phase 1 CR05
        unpack(recallrec.acRecallID, 8, rcindxrec.acRecallID, 4, 0);    // CSk 21-02-2011 Recalls Phase 1 CR05
        recallrec.ubChain = 0;                                          // CSk 21-02-2011 Recalls Phase 1 CR05
        usrrc = ReadRecall(__LINE__);                                   // CSk 21-02-2011 Recalls Phase 1 CR05
        if (usrrc < RC_OK) {                                            // CSk 21-02-2011 Recalls Phase 1 CR05
            prep_nak("ERRORThis Recall could not be found");            // CSk 21-02-2011 Recalls Phase 1 CR05
            return;                                                     // CSk 21-02-2011 Recalls Phase 1 CR05
        }                                                               // CSk 21-02-2011 Recalls Phase 1 CR05

        MEMCPY(pRCC->abBatchNos, recallrec.abBatchNumbers);             // CSk 21-02-2011 Recalls Phase 1 CR05
    } else { // default to spaces                                       // CSk 21-02-2011 Recalls Phase 1 CR05
        MEMSET(pRCC->abBatchNos, ' ');
    }                                                                   // CSk 21-02-2011 Recalls Phase 1 CR05

    out_lth = LRT_RCC_LTH;
    return;

}


////////////////////////////////////////////////////////////////////////////////
/// RecallCount
////////////////////////////////////////////////////////////////////////////////

typedef struct LRT_RCG_Txn {
    BYTE cmd[3];
    BYTE abOpID[3];                                                     // Mult UOD Rcll 12-8-2011 SDH
    BYTE acRecallID[8];
    BYTE anRecallItem[6];  // Boots Code no check digit unpacked
    BYTE anRecallCount[4]; // item count to update recall file
} LRT_RCG;                                    
#define LRT_RCG_LTH sizeof(LRT_RCG)

void RecallCount(char *inbound) {                                       // 24-05-07 PAB Recalls
                                                                        
    LRT_RCG* pRCG = (LRT_RCG*)inbound;                                  // 31-05-07 PAB Recalls
    LONG lItemArrayptr = 0;                                             // 31-05-07 PAB Recalls
                                                                        // 24-05-07 PAB Recalls
    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    //Start reading from first RECALL chain, look for the item to update// 31-05-07 PAB Recalls
    MEMCPY(recallrec.acRecallID, pRCG->acRecallID);                     // 24-05-07 PAB Recalls
    recallrec.ubChain = 0;                                              // 31-05-07 PAB Recalls
    BOOLEAN fFound = FALSE;                                             // Mult UOD Rcll 12-08-2011 SDH
    while (!fFound) {                                                   // Mult UOD Rcll 12-08-2011 SDH
                                                                        
        if (ReadRecall(__LINE__) < RC_OK) {                             // Mult UOD Rcll 12-08-2011 SDH
            prep_nak("ERRORThis item could not be found on this Recall");// 24-05-07 PAB Recalls
            return;                                                     // 24-05-07 PAB Recalls
        }                                                               // Mult UOD Rcll 12-08-2011 SDH
                                                                        
        disp_msg("Update Recall Item");                                 
        pack(sbuf, 3, pRCG->anRecallItem, 6, 0);                        // 31-05-07 PAB Recalls
        dump(sbuf,3);                                                   
                                                                        
        lItemArrayptr = 0;                                              // 31-05-07 PAB Recalls
        while (lItemArrayptr < 50) {                                    // 31-05-07 PAB Recalls
            RecallItem *pItem = &recallrec.aRecallItems[lItemArrayptr]; // Mult UOD Rcll 12-08-2011 SDH
            //If we have a match, update the session count and the      // Mult UOD Rcll 12-08-2011 SDH
            //updated today flag, and write the record back             // Mult UOD Rcll 12-08-2011 SDH
            if (memcmp(sbuf, pItem->abItemCode, 3) == 0) {              // 31-05-07 PAB Recalls
                pack(pItem->anSessionCount,2,pRCG->anRecallCount,4,0);  // 20-6-2007 PAB
                pItem->cCountUpdatedToday = 'Y';                        // Mult UOD Rcll 12-08-2011 SDH
                if (WriteRecall(__LINE__) < RC_OK) {                    // Mult UOD Rcll 12-08-2011 SDH
                    prep_nak("ERRORUnable to update RECALL file. "      // Mult UOD Rcll 12-08-2011 SDH
                                 "Check appl event logs");              // Mult UOD Rcll 12-08-2011 SDH
                    return;                                             // Mult UOD Rcll 12-08-2011 SDH
                }                                                       // Mult UOD Rcll 12-08-2011 SDH
                fFound = TRUE;                                          // Mult UOD Rcll 12-08-2011 SDH
                break;                                                  // Mult UOD Rcll 12-08-2011 SDH
            }                                                           // Mult UOD Rcll 12-08-2011 SDH
            lItemArrayptr++;                                            // 31-05-07 PAB Recalls
        }
        recallrec.ubChain++;

    }
                                                                        
    //If a CCLOL list has not yet been allocated.                       // Mult UOD Rcll 12-8-2011 SDH
    //This is required because PPCs do NOT call RecallStart after       // Mult UOD Rcll 12-8-2011 SDH
    //RecallExit (partial), whereas PODs do.                            // Mult UOD Rcll 12-8-2011 SDH
    if (lrtp[hh_unit]->wRecallCclol == 0) {                             // Mult UOD Rcll 12-8-2011 SDH
        if (AllocateCclol()) return;                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    //Check whether this item is already in the CCILF                   // Mult UOD Rcll 12-8-2011 SDH
    //This block essentially sets up the CCILF key                      // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abListNum, lrtp[hh_unit]->wRecallCclol);     // Mult UOD Rcll 12-8-2011 SDH
    WORD wRecSeq = 1;                                                   // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abRecSeq, wRecSeq);                          // Mult UOD Rcll 12-8-2011 SDH
    while (CcilfRead(__LINE__) > 0L) {                                  // Mult UOD Rcll 12-8-2011 SDH
        if (memcmp(ccilfrec.abItemCode, pRCG->anRecallItem, 6) == 0) {  // Mult UOD Rcll 12-8-2011 SDH
            break;                                                      // Mult UOD Rcll 12-8-2011 SDH
        }                                                               // Mult UOD Rcll 12-8-2011 SDH
        WORD_TO_ARRAY(ccilfrec.abRecSeq, ++wRecSeq);                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    // Update/write CCILF record                                        // Mult UOD Rcll 12-8-2011 SDH
    if (ProcessCcilf(ARRAY_TO_WORD(ccilfrec.abListNum), wRecSeq,        // Mult UOD Rcll 12-8-2011 SDH
                     pRCG->anRecallItem,                                // Mult UOD Rcll 12-8-2011 SDH
                     ARRAY_TO_WORD(pRCG->anRecallCount))) return;       // Mult UOD Rcll 12-8-2011 SDH

    //Prepare ACK                                                     
    prep_ack("");                                                     
    return;                                                           

}                                                                       // 24-05-07 PAB Recalls


////////////////////////////////////////////////////////////////////////////////
/// RecallSelectList
////////////////////////////////////////////////////////////////////////////////

typedef struct {
    BYTE anRecallItem[6];                                                          
    BYTE abItemDesc[20];
    BYTE anItemTSF[4];
    BYTE anRecCnt[4];                                                   // 20-6-2007 PAB
    BYTE cItemFlag;                                                     // Mult UOD Rcll 12-08-2011 SDH
    BYTE cVisible;                                                      // CSk 13-05-2010 Recalls Phase 1
} aRecalls;                                                                         

typedef struct LRT_RCF_Txn {
    BYTE cmd[3];
    BYTE opid[3];
    BYTE acRecallID[8];
    BYTE cMoreItems;          // "Y" if more items to come              // Mult UOD Rcll 12-08-2011 SDH
    BYTE cStatus;             // status of recall from file.            // Mult UOD Rcll 12-08-2011 SDH
    aRecalls abItemArray[10]; // Pre-Pack with "FFFFFF"
} LRT_RCF;                                    
#define LRT_RCF_LTH sizeof(LRT_RCF)

typedef struct LRT_RCH_Txn {
    BYTE cmd[3];
    BYTE opid[3];                  
    BYTE acRecallID[8];
    BYTE anItemPtr[4]; // pointer to item in recall record -> start "0000"
} LRT_RCH;                                    
#define LRT_RCH_LTH sizeof(LRT_RCH)

void RecallSelectList(char *inbound) {                                  // 24-05-07 PAB Recalls
                                                                        // 31-05-07 PAB Recalls
    LRT_RCH* pRCH = (LRT_RCH*)inbound;                                  // 31-05-07 PAB Recalls
    LRT_RCF* pRCF = (LRT_RCF*)out;                                      // 31-05-07 PAB Recalls
    LRT_RCE* pRCE = (LRT_RCE*)out;                                      // 31-05-07 PAB Recalls

    DOUBLE dDateOfStockMovement;                                        // CSk 30-11-2010 Recall Improvements
    DOUBLE dTodayDate;                                                  // CSk 13-05-2010 Recalls Phase 1
    WORD hour, min;                                                     // CSk 13-05-2010 Recalls Phase 1
    LONG sec, day, month, year;                                         // CSk 13-05-2010 Recalls Phase 1
    LONG usrrc = RC_OK;                                                 // 24-05-07 PAB Recalls
    LONG lIndexPtr = 0;                                                 // 29-05-07 PAB Recalls
    LONG lItemCnt = 0;                                                  // 31-05-07 PAB Recalls
    LONG lArrayPtr = 0;                                                 // 31-05-07 PAB Recalls
    LONG lItemsinRecall = 0;                                            // 31-05-07 PAB Recalls
    // BYTE boots_code[4];                                              // 31-05-07 PAB Recalls
    BYTE boots_code_ncd[4];                                             // 31-05-07 PAB Recalls
    memset(boots_code_ncd, 0x00, 4);                                    // 31-05-07 PAB Recalls
    BYTE null_boots_code[3];                                            // 31-05-07 PAB Recalls
    BYTE uncounted[2];                                                  // 20-06-07 PAB Recalls
    memset(null_boots_code, 0x00, 3);                                   // 31-05-07 PAB Recalls
    memset(uncounted, 0xFF, 2);                                         // 27-06-07 PAB recalls

    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    // Read the recalls file for the requested ID                       // 31-05-07 PAB Recalls
    MEMCPY(recallrec.acRecallID, pRCH->acRecallID);                     // Mult UOD Rcll 12-08-2011 SDH
                                                                        
    // compute which recall chain the items required are located        // 31-05-07 PAB Recalls
    lIndexPtr = ARRAY_TO_LONG(pRCH->anItemPtr);                         // Mult UOD Rcll 12-08-2011 SDH
    lItemCnt = lIndexPtr;                                               // 31-05-07 PAB Recalls
    recallrec.ubChain = 0;                                              // 31-05-07 PAB Recalls
    while (lIndexPtr >= 50) {                                           // 31-05-07 PAB Recalls
        recallrec.ubChain++;                                            // 31-05-07 PAB Recalls
        lIndexPtr = lIndexPtr - 50;                                     // 31-05-07 PAB Recalls
    }                                                                   // 31-05-07 PAB Recalls

    if (ReadRecall(__LINE__) <= RC_OK) {                                // 24-05-07 PAB Recalls
        recall.present = FALSE;                                         // 24-05-07 PAB Recalls
    } else {                                                            // 31-05-07 PAB Recalls
        recall.present = TRUE;                                          // 24-05-07 PAB Recalls
    }

    sprintf(msg, "Requesting Recall Items. %08lX ", lItemCnt);
    disp_msg(msg);
    disp_msg("Recall item count is -");
    dump((BYTE *)recallrec.anItemCount,sizeof(recallrec.anItemCount));

    lItemsinRecall = ARRAY_TO_LONG(recallrec.anItemCount);              // Mult UOD Rcll 12-08-2011 SDH

    // if all items send construct RCE
    if (recall.present == FALSE || lItemCnt >= lItemsinRecall) {
        MEMCPY(pRCE->cmd, "RCE");
        MEMCPY(pRCE->opid, pRCH->opid);
        out_lth = LRT_RCE_LTH;
        return;
    }

    // else construct RCF response record to send items
    MEMCPY(pRCF->cmd, "RCF");
    MEMCPY(pRCF->opid, pRCH->opid);
    MEMCPY(pRCF->acRecallID, pRCH->acRecallID);
    if (lItemCnt < lItemsinRecall) pRCF->cMoreItems = 'Y';

    // prepack outbound array with 'FF'
    while (lArrayPtr < 10) {
        MEMSET(pRCF->abItemArray[lArrayPtr].anRecallItem, 'F');
        MEMSET(pRCF->abItemArray[lArrayPtr].abItemDesc, 'F');
        MEMSET(pRCF->abItemArray[lArrayPtr].anItemTSF, 'F');
        MEMSET(pRCF->abItemArray[lArrayPtr].anRecCnt, 'F');
        lArrayPtr++;
    }

    //If a CCLOL list has not yet been allocated.                       // Mult UOD Rcll 12-8-2011 SDH
    //This is just in case we get here without creating a list.         // Mult UOD Rcll 12-8-2011 SDH
    if (lrtp[hh_unit]->wRecallCclol == 0) {                             // Mult UOD Rcll 12-8-2011 SDH
        if (AllocateCclol()) return;                                    // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH
    
    //Find the end of the CCILF chain                                   // Mult UOD Rcll 12-8-2011 SDH
    //We need to rebuild the CCILF list, to ensure no items lost when   // Mult UOD Rcll 12-8-2011 SDH
    //continuing a list from earlier.                                   // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abListNum, lrtp[hh_unit]->wRecallCclol);     // Mult UOD Rcll 12-8-2011 SDH
    WORD wCcilfRecSeq = 1;                                              // Mult UOD Rcll 12-8-2011 SDH
    WORD_TO_ARRAY(ccilfrec.abRecSeq, wCcilfRecSeq);                     // Mult UOD Rcll 12-8-2011 SDH
    while (CcilfRead(__LINE__) > 0L) {                                  // Mult UOD Rcll 12-8-2011 SDH
        WORD_TO_ARRAY(ccilfrec.abRecSeq, ++wCcilfRecSeq);               // Mult UOD Rcll 12-8-2011 SDH
    }                                                                   // Mult UOD Rcll 12-8-2011 SDH

    open_imstc();
    IdfOpen();
    open_stock();

    // populate the item array
    lArrayPtr = 0;
    while ((lArrayPtr < 10) && (lItemCnt <= lItemsinRecall)) {

        // null item code reached in recall record array
        if (memcmp(recallrec.aRecallItems[lIndexPtr].abItemCode,
                   null_boots_code, 3) == 0) {
            pRCF->cMoreItems = 'N';
            break;
        }

        // unpack the boots code from recall rec to output txn.
        unpack(pRCF->abItemArray[lArrayPtr].anRecallItem,6,
               recallrec.aRecallItems[lIndexPtr].abItemCode,3,0);

        //If the item has been counted today then put the Session Count in the response                         // Mult UOD Rcll 12-08-2011 SDH
        if (recallrec.aRecallItems[lIndexPtr].cCountUpdatedToday == 'Y') {                                      // Mult UOD Rcll 12-08-2011 SDH
            
            unpack(pRCF->abItemArray[lArrayPtr].anRecCnt,4,                                                     // 07-09-2007 5.3 BMG
                   recallrec.aRecallItems[lIndexPtr].anSessionCount, 2, 0);                                     // 07-09-2007 5.3 BMG

            //Write a new CCILF record                                          // Mult UOD Rcll 12-8-2011 SDH
            //We need to rebuild the CCILF list, to ensure no items lost when   // Mult UOD Rcll 12-8-2011 SDH
            //continuing a list from earlier.                                   // Mult UOD Rcll 12-8-2011 SDH
            if (ProcessCcilf(lrtp[hh_unit]->wRecallCclol, wCcilfRecSeq++,       // Mult UOD Rcll 12-8-2011 SDH
                             pRCF->abItemArray[lArrayPtr].anRecallItem,         // Mult UOD Rcll 12-8-2011 SDH
                             ARRAY_TO_WORD(pRCF->abItemArray[lArrayPtr].anRecCnt))) {// Mult UOD Rcll 12-8-2011 SDH
                 return;                                                        // Mult UOD Rcll 12-8-2011 SDH
            }                                                                   // Mult UOD Rcll 12-8-2011 SDH

        //Else (NOT counted today)                                                          // Mult UOD Rcll 12-08-2011 SDH
        } else {                                                                            // 07-09-2007 5.3 BMG
                //If the Count TSF is 'FFFF', populate response with spaces                 // Mult UOD Rcll 12-08-2011 SDH
            if (memcmp(recallrec.aRecallItems[lIndexPtr].anCountTSF, uncounted, 2) == 0) {  // 20-6-2007 PAB
                memset(pRCF->abItemArray[lArrayPtr].anRecCnt, ' ', 4);                      // 20-6-2007 PAB
            //Else (valid Count TSF)                                                        // Mult UOD Rcll 12-08-2011 SDH
            } else {                                                                        // 20-6-2007 PAB
                unpack(pRCF->abItemArray[lArrayPtr].anRecCnt,4,                             // 20-6-2007 PAB
                       recallrec.aRecallItems[lIndexPtr].anCountTSF, 2, 0);                 // 20-6-2007 PAB
            }                                                                               // 20-6-2007 PAB
        }                                                                                   // 07-09-2007 5.3 BMG

        pRCF->abItemArray[lArrayPtr].cItemFlag = recallrec.aRecallItems[lIndexPtr].cCountUpdatedToday;          // Mult UOD Rcll 12-08-2011 SDH

        // look up the item description from the IDF
        calc_boots_cd(idfrec.boots_code, recallrec.aRecallItems[lIndexPtr].abItemCode);
        if (IdfRead(__LINE__) <= 0L) {
            // set idf description to not on file
            memcpy(pRCF->abItemArray[lArrayPtr].abItemDesc, "Item not on file    ", 20);     //Defect 4806    // CSk 30-11-2010 Recall Improvements
        } else {
            memcpy(pRCF->abItemArray[lArrayPtr].abItemDesc, idfrec.stndrd_desc, 20);
        }

        // get the current TSF.
        // look up item on IMSTC (to get latest stock figure)

        memcpy(boots_code_ncd+1, recallrec.aRecallItems[lIndexPtr].abItemCode, 3);
        memset(imstcrec.bar_code, 0x00, sizeof(imstcrec.bar_code));
        memcpy(imstcrec.bar_code + 7, boots_code_ncd, 4);

        disp_msg("RD IMSTC : ");
        dump(imstcrec.bar_code, 11);
        
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
            memcpy(stockrec.boots_code, idfrec.boots_code, sizeof(stockrec.boots_code));    // 02-08-07 PAB Recalls
            if (ReadStock(__LINE__) <= 0L) {
                MEMSET(pRCF->abItemArray[lArrayPtr].anItemTSF, '0');
            } else {
                // Use stock figure from STOCK
                WORD_TO_ARRAY(pRCF->abItemArray[lArrayPtr].anItemTSF, stockrec.stock_fig);  // Mult UOD Rcll 12-08-2011 SDH
            }
        }

        pRCF->abItemArray[lArrayPtr].cVisible = 'N';                                              // CSk 13-05-2010 Recalls Phase 1

        //--------------------------------------------------------------------------------        // CSk 13-05-2010 Recalls Phase 1
        //Perform tailoring checks if 100% Returns and description does NOT start with NT*        // CSk 13-05-2010 Recalls Phase 1
        //--------------------------------------------------------------------------------        // CSk 13-05-2010 Recalls Phase 1
        if (( (recallrec.cRecallType == 'R') || (recallrec.cRecallType == 'S')) &&                // CSk 30-11-2010 Recall Improvements
            strncmp(recallrec.abDescription, "NT*", 3)) {                                         // CSk 13-05-2010 Recalls Phase 1
                                                                                                  // CSk 30-11-2010 Recall Improvements
            if (imstc.present) {                                                                  // CSk 30-11-2010 Recall Improvements
                // Item Sold today so no further Tailoring checks needed                          // CSk 30-11-2010 Recall Improvements
                pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                                      // CSk 30-11-2010 Recall Improvements
            } else {                                                                              // CSk 30-11-2010 Recall Improvements
                //-------------------------------------------------------                         // CSk 13-05-2010 Recalls Phase 1
                // Set item to visible if one of the following 3 is true:                         // CSk 13-05-2010 Recalls Phase 1
                //                                                                                // CSk 13-05-2010 Recalls Phase 1
                // 1. Item has had a stock movement in the last 3 months (90 days)                // CSk 13-05-2010 Recalls Phase 1
                // 2. Item was counted in the last 3 months (90 days)                             // CSk 30-11-2010 Recall Improvements
                // 3. Item had a delivery in the last 3 months (90 days)                          // CSk 30-11-2010 Recall Improvements
                //------------------------------------------------------                          // CSk 13-05-2010 Recalls Phase 1
                sysdate( &day, &month, &year, &hour, &min, &sec );                                // CSk 13-05-2010 Recalls Phase 1 
                dTodayDate = ConvGJ( day, month, year );                                          // CSk 13-05-2010 Recalls Phase 1 
                                                                                                  // CSk 13-05-2010 Recalls Phase 1
                // Check dates on STOCK file for date of last movement                            // CSk 13-05-2010 Recalls Phase 1
                memcpy(stockrec.boots_code, idfrec.boots_code, sizeof(stockrec.boots_code) );     // CSk 13-05-2010 Recalls Phase 1
                usrrc = ReadStock(__LINE__);                                                      // CSk 13-05-2010 Recalls Phase 1

                if (usrrc > RC_OK) {  // Use Date of Last Sale from STOCK                         // CSk 30-11-2010 Recall Improvements
                    memcpy(sbuf, "20", 2);                               // Add century           // CSk 13-05-2010 Recalls Phase 1
                    unpack( sbuf+2, 6, stockrec.date_last_move, 3, 0 );                           // CSk 13-05-2010 Recalls Phase 1  
                    day   = satol( sbuf+6, 2 );                                                   // CSk 13-05-2010 Recalls Phase 1  
                    month = satol( sbuf+4, 2 );                                                   // CSk 13-05-2010 Recalls Phase 1  
                    year  = satol( sbuf,   4 );                                                   // CSk 13-05-2010 Recalls Phase 1  
                    dDateOfStockMovement = ConvGJ( day, month, year );                            // CSk 30-11-2010 Recall Improvements 

                    if ((dTodayDate - dDateOfStockMovement) <= 90) {  // approx. 3 months         // CSk 30-11-2010 Recall Improvements 
                        pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                              // CSk 30-11-2010 Recall Improvements
                                                                                                  // CSk 30-11-2010 Recall Improvements
                    } else { // Use Date of Last Count from STOCK                                 // CSk 30-11-2010 Recall Improvements   
                        memcpy(sbuf, "20", 2);                               // Add century       // CSk 30-11-2010 Recall Improvements   
                        unpack( sbuf+2, 6, stockrec.date_last_count, 3, 0 );                      // CSk 30-11-2010 Recall Improvements    
                        day   = satol( sbuf+6, 2 );                                               // CSk 30-11-2010 Recall Improvements    
                        month = satol( sbuf+4, 2 );                                               // CSk 30-11-2010 Recall Improvements    
                        year  = satol( sbuf,   4 );                                               // CSk 30-11-2010 Recall Improvements    
                        dDateOfStockMovement = ConvGJ( day, month, year );                        // CSk 30-11-2010 Recall Improvements 

                        if ((dTodayDate - dDateOfStockMovement) <= 90) {  // approx. 3 months     // CSk 30-11-2010 Recall Improvements 
                            pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                          // CSk 30-11-2010 Recall Improvements
                        } else { // Use Date of Last Delivery from STOCK                          // CSk 30-11-2010 Recall Improvements                                                 
                            memcpy(sbuf, "20", 2);                               // Add century   // CSk 30-11-2010 Recall Improvements   
                            unpack( sbuf+2, 6, stockrec.date_last_rec, 3, 0 );                    // CSk 30-11-2010 Recall Improvements    
                            day   = satol( sbuf+6, 2 );                                           // CSk 30-11-2010 Recall Improvements    
                            month = satol( sbuf+4, 2 );                                           // CSk 30-11-2010 Recall Improvements    
                            year  = satol( sbuf,   4 );                                           // CSk 30-11-2010 Recall Improvements    
                            dDateOfStockMovement = ConvGJ( day, month, year );                    // CSk 30-11-2010 Recall Improvements 

                            if ((dTodayDate - dDateOfStockMovement) <= 90) {  // approx. 3 months // CSk 30-11-2010 Recall Improvements 
                                pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                      // CSk 30-11-2010 Recall Improvements
                            }                                                                     // CSk 30-11-2010 Recall Improvements
                        }                                                                         // CSk 30-11-2010 Recall Improvements
                    }                                                                             // CSk 30-11-2010 Recall Improvements
                } // else drop through to next test                                               // CSk 30-11-2010 Recall Improvements

                // CSk 30-11-2010 Recall Improvements
                if (pRCF->abItemArray[lArrayPtr].cVisible == 'N') {                                    // CSk 13-05-2010 Recalls Phase 1 
                    //-----------------                                                                // CSk 13-05-2010 Recalls Phase 1
                    // 2. Item TSF > 0                                                                 // CSk 13-05-2010 Recalls Phase 1
                    //-----------------                                                                // CSk 13-05-2010 Recalls Phase 1
                    if (strncmp(pRCF->abItemArray[lArrayPtr].anItemTSF, "0000", 4)) {                  // CSk 13-05-2010 Recalls Phase 1                                                                                         
                        pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                                   // CSk 13-05-2010 Recalls Phase 1
                    } else {                                                                           // CSk 13-05-2010 Recalls Phase 1
                        //--------------------------                                                   // CSk 13-05-2010 Recalls Phase 1
                        // 3. Item is an active SKU                                                    // CSk 13-05-2010 Recalls Phase 1
                        //--------------------------                                                   // CSk 13-05-2010 Recalls Phase 1
                        // Check if item is on a LIVE Planner                                          // CSk 13-05-2010 Recalls Phase 1
                        usrrc = SritmlOpen();                                                          // CSk 13-05-2010 Recalls Phase 1
                                                                                                       // CSk 13-05-2010 Recalls Phase 1
                        MEMCPY(sritmlrec.abItemCode, recallrec.aRecallItems[lIndexPtr].abItemCode);    // CSk 13-05-2010 Recalls Phase 1
                        sritmlrec.ubRecChain = 0;                                                      // CSk 13-05-2010 Recalls Phase 1
                                                                                                       // CSk 13-05-2010 Recalls Phase 1
                        usrrc = SritmlRead(__LINE__);                                                  // CSk 13-05-2010 Recalls Phase 1
                        if (usrrc < 0) {                                                               // CSk 13-05-2010 Recalls Phase 1
                            // Item is not on a Live planner so check PENDING Planners                 // CSk 13-05-2010 Recalls Phase 1
                            usrrc = SritmpOpen();                                                      // CSk 13-05-2010 Recalls Phase 1
                                                                                                       // CSk 13-05-2010 Recalls Phase 1
                            MEMCPY(sritmprec.abItemCode, recallrec.aRecallItems[lIndexPtr].abItemCode);// CSk 13-05-2010 Recalls Phase 1
                            sritmprec.ubRecChain = 0;                                                  // CSk 13-05-2010 Recalls Phase 1
                                                                                                       // CSk 13-05-2010 Recalls Phase 1
                            usrrc = SritmpRead(__LINE__);                                              // CSk 13-05-2010 Recalls Phase 1
                            if (usrrc >= 0) {                                                          // CSk 13-05-2010 Recalls Phase 1
                                // Item is ON PENDING Planner                                          // CSk 13-05-2010 Recalls Phase 1
                                pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                           // CSk 13-05-2010 Recalls Phase 1
                            }                                                                          // CSk 13-05-2010 Recalls Phase 1
                            SritmpClose( CL_SESSION );                                                 // CSk 13-05-2010 Recalls Phase 1
                        } else {                                                                       // CSk 13-05-2010 Recalls Phase 1
                            pRCF->abItemArray[lArrayPtr].cVisible = 'Y';                               // CSk 13-05-2010 Recalls Phase 1
                        }                                                                              // CSk 13-05-2010 Recalls Phase 1
                        SritmlClose( CL_SESSION );                                                     // CSk 13-05-2010 Recalls Phase 1
                    }                                                                                  // CSk 13-05-2010 Recalls Phase 1
                }                                                                                      // CSk 13-05-2010 Recalls Phase 1
            }                                                                                          // CSk 13-05-2010 Recalls Phase 1
        }                                                                                              // CSk 13-05-2010 Recalls Phase 1

        lArrayPtr++;                                                    // 31-05-07 PAB Recalls
        lIndexPtr++;                                                    // 31-05-07 PAB Recalls
    }                                                                   // 31-05-07 PAB Recalls
                                                                        
    if (lItemCnt == lItemsinRecall) pRCF->cMoreItems = 'N';             // 31-05-07 PAB Recalls
    pRCF->cStatus = recallrec.cRecallStatus;                            // 09-08-07 PAB Recalls
                                                                        // 31-05-07 PAB Recalls
    close_imstc(CL_SESSION);                                            // 31-05-07 PAB Recalls
    IdfClose(CL_SESSION);                                               // 31-05-07 PAB Recalls
    close_stock( CL_SESSION );                                          // 31-05-07 PAB Recalls
                                                                        // 31-05-07 PAB Recalls
    out_lth = LRT_RCF_LTH;                                              // 31-05-07 PAB Recalls
    return;                                                             // 31-05-07 PAB Recalls
}                                                                       // 24-05-07 PAB Recalls
                                                                        

////////////////////////////////////////////////////////////////////////////////
/// RecallInstructions
////////////////////////////////////////////////////////////////////////////////

typedef struct LRT_RCI_Txn {
    BYTE cmd[3];
    BYTE opid[3];                  
    BYTE acRecallID[8];
} LRT_RCI;                                    
#define LRT_RCI_LTH sizeof(LRT_RCI)

typedef struct LRT_RCJ_Txn {
    BYTE cmd[3];
    BYTE opid[3];                  
    BYTE acRecallID[8];
    BYTE abSpecialIns[160];
} LRT_RCJ;                                    
#define LRT_RCJ_LTH sizeof(LRT_RCJ)

void RecallInstructions(char *inbound) {                                // 24-05-07 PAB Recalls
                                                                        
    LRT_RCI* pRCI = (LRT_RCI*)inbound;                                  // 31-05-07 PAB Recalls
    LRT_RCJ* pRCJ = (LRT_RCJ*)out;                                      // 31-05-07 PAB Recalls
    LONG usrrc = RC_OK;                                                 // 24-05-07 PAB Recalls
                     
    // check status of recall files                                     // 24-05-07 PAB Recalls
    if (CheckRecallAvailable() != 0) return;                            // 24-05-07 PAB Recalls

    MEMCPY(rcspirec.acRecallID, pRCI->acRecallID);                      // Mult UOD Rcll 12-08-2011 SDH
    usrrc = s_read(0,rcspi.fnum,(void *)&rcspirec,RCSPI_RECL,RCSPI_KEYL);// 24-05-07 PAB Recalls
    if (usrrc < RC_OK) {                                                // 24-05-07 PAB Recalls
        disp_msg("RCSPI Record not found");                             // 24-05-07 PAB Recalls
        prep_nak("The special instructions "                            // 20-08-07 PAB Recalls
                 "for this Recall could not be found." );               // 24-05-07 PAB Recalls
        return;                                                         // 24-05-07 PAB Recalls
    }                                                                   // 24-05-07 PAB Recalls
    MEMCPY(pRCJ->cmd, "RCJ");
    MEMCPY(pRCJ->opid, pRCI->opid);
    MEMCPY(pRCJ->acRecallID, pRCI->acRecallID);
    MEMCPY(pRCJ->abSpecialIns, rcspirec.abMessage);

    out_lth = LRT_RCF_LTH;                                              // 31-05-07 PAB Recalls
    return;

}                                                                       // 24-05-07 PAB Recalls

////////////////////////////////////////////////////////////////////////////////
/// StopRecalls
/// Block access to recall files and force close of recall files.   
////////////////////////////////////////////////////////////////////////////////

void StopRecalls(void) {                                                // 24-05-07 PAB Recalls
    gRecallFilesAvailable = 1;                                          // 24-05-07 PAB Recalls
    disp_msg("Got REC* so closing Recalls Files");                      // 11-09-2007 5.5 BMG
    close_recall(CL_ALL);                                               // 31-05-07 PAB Recalls
    close_rcindx(CL_ALL);                                               // 31-05-07 PAB Recalls
    close_rcspi(CL_ALL);                                                // 31-05-07 PAB Recalls
    disp_msg("Closed Recalls Files");                                   // 11-09-2007 5.5 BMG
}                                                                       // 24-05-07 PAB Recalls

