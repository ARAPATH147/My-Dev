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
#include "osfunc.h"                   /* needed for disp_msg() */
//#include "adxsrvst.h"                   /* needed for bg */
#include "trxfile.h"                    // Streamline SDH 22-Sep-2008
//#include "rfs2.h"                       // PAB 24-May-2007 Recalls
#include "rfsfile.h"
//#include "rfsfile2.h"                   // PAB 24-May-2007 Recalls
//#include "dateconv.h"
//#include "wrap.h"
#include "sockserv.h"
#include "rfglobal.h"                   // v4.0
//#include "rfglobal2.h"                  // PAB 24-May-2007 Recalls
#include "osfunc.h"
#include "prtctl.h"                     // Streamline SDH 17-Sep-2008
#include "srfiles.h"
#include "idf.h"
#include "irf.h"
#include "rfscf.h"
#include "ccfiles.h"
#include "invok.h"

//extern UBYTE bg;        // background appl flag for use with disp_msg()
//UWORD activity;


//////////////////////////////////////////////////////////////////////////////
///                                                                        ///
///   Static (private) variables                                           ///
///                                                                        ///
//////////////////////////////////////////////////////////////////////////////

typedef struct LRT_DNQ_Txn {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE abDealNum[4];
} LRT_DNQ;
#define LRT_DNQ_LTH sizeof(LRT_DNQ)                                                     // SDH 25-03-2005 Promotions

typedef struct LRT_DQR_Txn {
    BYTE abCmd[3];
    BYTE abDealNum[4];
    BYTE abStartDate[8];
    BYTE abEndDate[8];
    BYTE abDealDesc[35];
} LRT_DQR;
#define LRT_DQR_LTH sizeof(LRT_DQR)

typedef struct LRTLG_DNQ_Record_layout {
   BYTE abDealNum[4];
   BYTE abFiller[8];
} LRTLG_DNQ;


static LONG lLastTDTFFLevel = -1;                                           // SDH 13-12-04 PROMOTIONS

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

// ------------------------------------------------------------------------------------
//
// DNQ - Deal Enquiry
//
// Always assume that the first qual and first reward are the important ones.
//
// ------------------------------------------------------------------------------------

void DealEnquiry(char *inbound) {                                   // BMG 1.1 11-09-2007 Moved from transact.c.

    int rc = 0;
    LONG rc2 = 0;

    LONG usrrc = RC_OK;
    UNUSED(inbound);

    //Input and output views                                    // SDH 15-12-04 PROMOTIONS
    LRT_DNQ* pDNQ = (LRT_DNQ*)inbound;                          // SDH 15-12-04 PROMOTIONS
    LRTLG_DNQ* pLGDNQ = (LRTLG_DNQ*)dtls;                       // SDH 15-12-04 PROMOTIONS
    LRT_DQR* pDQR = (LRT_DQR*)out;                              // SDH 15-12-04 PROMOTIONS

    //Initial checks                                            // SDH 15-12-04 PROMOTIONS
    if (IsStoreClosed()) return;                                // SDH 15-12-04 PROMOTIONS
    if (IsHandheldUnknown()) return;                            // SDH 15-12-04 PROMOTIONS
    UpdateActiveTime();                                         // SDH 15-12-04 PROMOTIONS

    //Open deal file                                            // SDH 15-12-04 PROMOTIONS
    usrrc = open_deal();                                        // SDH 15-12-04 PROMOTIONS
    if (usrrc < RC_OK) {                                        // SDH 15-12-04 PROMOTIONS
        prep_nak("ERRORUnable to "                              // SDH 23-Aug-2006 Planners
                 "open DEAL file. "                             // SDH 15-12-04 PROMOTIONS
                 "Check appl event logs");                      // SDH 15-12-04 PROMOTIONS
        return;                                                 // SDH 15-12-04 PROMOTIONS
    }                                                           // SDH 15-12-04 PROMOTIONS

    //Build deal file record key, read DEAL, handle errors      // SDH 15-12-04 PROMOTIONS
    pack(dealrec.abDealNumPD, 2, pDNQ->abDealNum, 4, 0);        // SDH 15-12-04 PROMOTIONS
    rc2 = ReadDeal(__LINE__);                                   // SDH 15-12-04 PROMOTIONS
    close_deal();                                               // SDH 15-12-04 PROMOTIONS
    if ((rc & 0xFFFF) == 0x06C8) {                              // SDH 15-12-04 PROMOTIONS
        prep_nak("Deal not on file");                           // SDH 23-Aug-2006 Planners
        return;                                                 // SDH 15-12-04 PROMOTIONS
    } else if (rc2 <= 0) {                                      // SDH 15-12-04 PROMOTIONS
        prep_nak("DEAL Information is "                         // PAB 27-Nov-2006 Planners
                 "out of step. Ring "                           // PAB 27-11-06 PROMOTIONS
                 "the Help Desk");                              // PAB 27-11-06 PROMOTIONS
        return;                                                 // SDH 15-12-04 PROMOTIONS
    }                                                           // SDH 15-12-04 PROMOTIONS

    //Build DQR                                                 // SDH 15-12-04 PROMOTIONS
    memcpy(pDQR->abCmd, "DQR", sizeof(pDQR->abCmd));            // SDH 15-12-04 PROMOTIONS
    memcpy(pDQR->abDealNum, pDNQ->abDealNum,                    // SDH 15-12-04 PROMOTIONS
           sizeof(pDQR->abDealNum));                            // SDH 15-12-04 PROMOTIONS
    unpack(pDQR->abStartDate, 8, dealrec.abStartDatePD, 4, 0);  // SDH 15-12-04 PROMOTIONS
    unpack(pDQR->abEndDate, 8, dealrec.abEndDatePD, 4, 0);      // SDH 15-12-04 PROMOTIONS
    memcpy(pDQR->abDealDesc, dealrec.abDealDesc,                // SDH 15-12-04 PROMOTIONS
           sizeof(pDQR->abDealDesc));                           // SDH 15-12-04 PROMOTIONS
    out_lth = LRT_DQR_LTH;                                      // SDH 15-12-04 PROMOTIONS

    //Build audit                                               // SDH 15-12-04 PROMOTIONS
    memcpy(pLGDNQ->abDealNum, pDNQ->abDealNum,                  // SDH 15-12-04 PROMOTIONS
           sizeof(pLGDNQ->abDealNum));                          // SDH 15-12-04 PROMOTIONS
    lrt_log(LOG_DNQ, hh_unit, dtls);                            // SDH 15-12-04 PROMOTIONS

}

