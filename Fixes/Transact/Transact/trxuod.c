#include "transact.h"

#include <string.h>
//#include "rfs.h"
#include "irf.h"                                                            // CSk 14-12-10 DEFECT 4075
#include "idf.h"                                                            // CSk 14-12-10 DEFECT 4075
#include "rfsfile.h"
#include "osfunc.h"
#include <time.h>
#include <math.h>
#include "rfscf.h"
#include "trxutil.h"
#include "trxfile.h"
#include "ccfiles.h"
#include "trxbase.h"

////////////////////////////////////////////////////////////////////////////////
//
// Private Structures
//
////////////////////////////////////////////////////////////////////////////////

typedef struct {                  // Start of UOD                           // SDH 26-11-04 CREDIT CLAIM
   BYTE abCmd[3];                                                                       // SDH 26-11-04 CREDIT CLAIM
   BYTE abOpID[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
   BYTE cListType;  //"G" - Goods out, "C" - credit claim                               // SDH 26-11-04 CREDIT CLAIM
} LRT_UOS;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_UOS_LTH sizeof(LRT_UOS)                                                     // SDH 26-11-04 CREDIT CLAIM
                                                                                        // SDH 26-11-04 CREDIT CLAIM
typedef struct {                  // UOD reponse                            // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abValidBusCentres[30];                                                         // SDH 26-11-04 CREDIT CLAIM
} LRT_UOR;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_UOR_LTH sizeof(LRT_UOR)                                                     // SDH 26-11-04 CREDIT CLAIM
                                                                                        // SDH 26-11-04 CREDIT CLAIM
typedef struct {                  // Start of UOD                           // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abRecSeq[4];                                                                   // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemCode[7];                                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE abQty[4];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abDesc[20];                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abSelDesc[45];                                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE abPrice[6];                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller[6];                                                                   // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abBusCentreDesc[14];                                                           // SDH 26-11-04 CREDIT CLAIM
    BYTE abBarCode[13];                                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE cItemStatus;                                                                   // SDH 26-11-04 CREDIT CLAIM
} LRT_UOA;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_UOA_LTH sizeof(LRT_UOA)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                  // UOD reponse                            // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE cListType;                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODNum[14];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE cStatus;                                                                       // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemCount[4];                                                                // SDH 26-11-04 CREDIT CLAIM
    BYTE cAdjStockFig;                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE cSupplyRoute;                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE cDispLocation;                                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abBusCentreDesc[14];                                                           // SDH 26-11-04 CREDIT CLAIM
    BYTE abRecallNum[8];                                                                // SDH 26-11-04 CREDIT CLAIM
    BYTE abAuth[15];                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplier[15];                                                                // SDH 26-11-04 CREDIT CLAIM
    BYTE abReturnMethod[2];                                                             // SDH 26-11-04 CREDIT CLAIM
    BYTE abCarrier[2];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abBirdNum[8];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abReasonNum[2];                                                                // SDH 26-11-04 CREDIT CLAIM
    BYTE abReceivingStoreNum[4];                                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abDestination[2];                                                              // SDH 26-11-04 CREDIT CLAIM
    BYTE cWarehouseRoute;                                                               // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODType[2];                                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abDamageRsn[2];                                                                // SDH 26-11-04 CREDIT CLAIM
} LRT_UOX;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_UOX_LTH sizeof(LRT_UOX)                                                     // SDH 26-11-04 CREDIT CLAIM

/*
typedef struct {                  // Get Direct Supplier List Start         // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
} LRT_DSS;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_DSS_LTH sizeof(LRT_DSS)                                                     // SDH 26-11-04 CREDIT CLAIM
*/

typedef struct {                  // Get Direct Supplier List Item          // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abNextSeqNum[4];                                                               // SDH 26-11-04 CREDIT CLAIM
} LRT_DSG;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_DSG_LTH sizeof(LRT_DSG)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                  // Direct Supplier List End               // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
} LRT_DSE;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_DSE_LTH sizeof(LRT_DSE)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                  // Direct Supplier List Response          // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abSeqNum[4];                                                                   // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplierNum[6];                                                              // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplierName[10];                                                            // SDH 26-11-04 CREDIT CLAIM
} LRT_DSR;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_DSR_LTH sizeof(LRT_DSR)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                  // UOD Item Query                         // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODNum[14];                                                                  // SDH 26-11-04 CREDIT CLAIM
} LRT_UOQ;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_UOQ_LTH sizeof(LRT_UOQ)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                // Stock take query                         // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODPrefix[8];                                                                // SDH 26-11-04 CREDIT CLAIM
} LRT_STQ;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_STQ_LTH sizeof(LRT_STQ)                                                     // SDH 26-11-04 CREDIT CLAIM

typedef struct {                                                            // SDH 26-11-04 CREDIT CLAIM
    BYTE abCmd[3];                                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODPrefix[8];                                                                // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODSuffix[6];                                                                // SDH 26-11-04 CREDIT CLAIM
} LRT_STR;                                                                              // SDH 26-11-04 CREDIT CLAIM
#define LRT_STR_LTH sizeof(LRT_STR)                                                     // SDH 26-11-04 CREDIT CLAIM


typedef struct LRTLG_UOS_Record_layout {
    BYTE abListNum[4];
    BYTE abFiller[8];
} LRTLG_UOS;
typedef struct LRTLG_UOA_Record_layout {
    BYTE abListNum[4];
    BYTE abItemCode[7];
    BYTE cItemStatus;
} LRTLG_UOA;
typedef struct LRTLG_UOX_Record_layout {
    BYTE abListNum[4];
    BYTE cStatus;
    BYTE abFiller[7];
} LRTLG_UOX;
typedef struct LRTLG_DSS_Record_layout {
    BYTE cSuccess;  //'Y' or 'N'
    BYTE abFiller[11];
} LRTLG_DSS;
typedef struct LRTLG_DSG_Record_layout {
    BYTE cBusCentre;
    BYTE abSeqNum[4];
    BYTE abSupplierNum[6];
    BYTE cFiller;
} LRTLG_DSG;
typedef struct LRTLG_UOQ_Record_layout {
    BYTE abUODPacked[7];
    BYTE cDuplicate;
    BYTE abFiller[4];
} LRTLG_UOQ;
typedef struct LRTLG_STQ_Record_layout {
    BYTE abUODPrefPkd[4];
    BYTE abUODSuffPkd[3];
    BYTE abFiller[5];
} LRTLG_STQ;


// ------------------------------------------------------------------------------------
//
// UOS - UOD Start
//
// ------------------------------------------------------------------------------------

void UODStart(char* inbound) {

    //Declare working variables                                 // SDH 26-11-04 CREDIT CLAIM
    LONG rc2;                                                   // Streamline SDH 21-Sep-2008
    WORD wListNum;                                              // SDH 26-11-04 CREDIT CLAIM

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_UOS *pUOS = (LRT_UOS*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRT_UOR *pUOR = (LRT_UOR*)out;                              // SDH 26-11-04 CREDIT CLAIM
    LRTLG_UOS *pLGUOS = (LRTLG_UOS*)dtls;                       // SDH 26-11-04 CREDIT CLAIM

    //Set up current date and time                              // SDH 26-11-04 CREDIT CLAIM
    B_DATE nowDate;                                             // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                             // SDH 26-11-04 CREDIT CLAIM
    GetSystemDate(&nowTime, &nowDate);                          // SDH 26-11-04 CREDIT CLAIM

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    //These routines build the NAK if appropriate               // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Open the credit claiming files for this session           // SDH 26-11-04 CREDIT CLAIM
    if (CclolOpen() != RC_OK) {                                // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to open CCLOL file. "             // SDH 26-11-04 CREDIT CLAIM
                  "Check appl event logs" );                    // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM
    if (CcilfOpen() != RC_OK) {                                // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to open CCILF file. "             // SDH 26-11-04 CREDIT CLAIM
                  "Check appl event logs" );                    // SDH 26-11-04 CREDIT CLAIM
        CclolClose(CL_SESSION);                                // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM
    if (CchistOpen() != RC_OK) {
        prep_nak("ERRORUnable to open CCHIST file. "            // SDH 26-11-04 CREDIT CLAIM
                  "Check appl event logs" );                    // SDH 26-11-04 CREDIT CLAIM
        CclolClose(CL_SESSION);
        CcilfClose(CL_SESSION);
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Read CCLOL header and handle errors                       // SDH 26-11-04 CREDIT CLAIM
    rc2 = CclolRead(0, __LINE__);                               // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to read from CCLOL. "             // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event logs" );                     // SDH 26-11-04 CREDIT CLAIM
        CclolClose(CL_SESSION);                                // SDH 26-11-04 CREDIT CLAIM
        CcilfClose(CL_SESSION);                                // SDH 26-11-04 CREDIT CLAIM
        CchistClose(CL_SESSION);                               // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Get the list ID as an int and increment it to get the     // SDH 26-11-04 CREDIT CLAIM
    //next available list ID.                                   // SDH 26-11-04 CREDIT CLAIM
    //This code assumes that the file does not wrap, and that   // SDH 26-11-04 CREDIT CLAIM
    //the number of record will not reach 32,767                // SDH 26-11-04 CREDIT CLAIM
    wListNum = satoi(cclolrec.abListNum,                        // SDH 26-11-04 CREDIT CLAIM
                     sizeof(cclolrec.abListNum)) + 1;           // SDH 26-11-04 CREDIT CLAIM

    //Build new CCLOL list record                               // SDH 26-11-04 CREDIT CLAIM
    memset(&cclolrec, 0x00, sizeof(cclolrec));                  // SDH 26-11-04 CREDIT CLAIM
    sprintf(sbuf, "%04d", wListNum);                            // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abListNum, sbuf, sizeof(cclolrec.abListNum));    // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cListType = pUOS->cListType;                       // SDH 26-11-04 CREDIT CLAIM
    sprintf(sbuf, "%04d%02d%02d",                               // SDH 26-11-04 CREDIT CLAIM
            nowDate.wYear, nowDate.wMonth, nowDate.wDay);       // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abDateUODOpened, sbuf,                      // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abDateUODOpened));                   // SDH 26-11-04 CREDIT CLAIM
    sprintf(sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);     // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abTimeUODOpened, sbuf,                      // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abTimeUODOpened));                   // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abOpID, pUOS->abOpID,                       // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abOpID));                            // SDH 26-11-04 CREDIT CLAIM
    if (strncmp(lrtp[hh_unit]->user, pUOS->abOpID,              // SDH 26-11-04 CREDIT CLAIM
                sizeof(pUOS->abOpID)) == 0) {                   // SDH 26-11-04 CREDIT CLAIM
        memcpy(cclolrec.abOpName, lrtp[hh_unit]->abOpName,      // SDH 26-11-04 CREDIT CLAIM
               sizeof(cclolrec.abOpName));                      // SDH 26-11-04 CREDIT CLAIM
    } else {                                                    // SDH 26-11-04 CREDIT CLAIM
        memcpy(cclolrec.abOpName, "Mismatched oper",            // SDH 26-11-04 CREDIT CLAIM
               sizeof(cclolrec.abOpName));                      // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Write record and handle errors                            // SDH 26-11-04 CREDIT CLAIM
    rc2 = CclolWrite(wListNum, __LINE__);                       // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to write to CCLOL. "              // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event log" );                      // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Increment the count on the header file and handle errors  // SDH 26-11-04 CREDIT CLAIM
    memcpy(sbuf, &cclolrec, sizeof(cclolrec.abListNum));        // SDH 26-11-04 CREDIT CLAIM
    memset(&cclolrec, 0x00, sizeof(cclolrec));                  // SDH 26-11-04 CREDIT CLAIM
    memcpy(&cclolrec, sbuf, sizeof(cclolrec.abListNum));        // SDH 26-11-04 CREDIT CLAIM
    rc2 = CclolWrite(0, __LINE__);                              // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to write to CCLOL. "              // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event log" );                      // SDH 26-11-04 CREDIT CLAIM
        CclolClose(CL_SESSION);                                // SDH 26-11-04 CREDIT CLAIM
        CcilfClose(CL_SESSION);                                // SDH 26-11-04 CREDIT CLAIM
        CchistClose(CL_SESSION);                               // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Build UOR                                                 // SDH 26-11-04 CREDIT CLAIM
    //We don't bother to read the RFSCF since it would have     // SDH 26-11-04 CREDIT CLAIM
    //been read at sign on and the allowed business centres     // SDH 26-11-04 CREDIT CLAIM
    //don't change too often                                    // SDH 26-11-04 CREDIT CLAIM
    memcpy(pUOR->abCmd, "UOR", sizeof(pUOR->abCmd));            // SDH 26-11-04 CREDIT CLAIM
    memcpy(pUOR->abOpID, pUOS->abOpID, sizeof(pUOR->abOpID));   // SDH 26-11-04 CREDIT CLAIM
    memcpy(pUOR->abListNum, cclolrec.abListNum,                 // SDH 26-11-04 CREDIT CLAIM
           sizeof(pUOR->abListNum));                            // SDH 26-11-04 CREDIT CLAIM
    memcpy(pUOR->abValidBusCentres, rfscfrec3.abBusCentres,     // SDH 26-11-04 CREDIT CLAIM
           sizeof(pUOR->abValidBusCentres));                    // SDH 26-11-04 CREDIT CLAIM
    out_lth = LRT_UOR_LTH;                                      // SDH 26-11-04 CREDIT CLAIM

    //Build UOS audit                                           // SDH 26-11-04 CREDIT CLAIM
    memcpy(pLGUOS->abListNum, pUOR->abListNum,                  // SDH 26-11-04 CREDIT CLAIM
           sizeof(pLGUOS->abListNum));                          // SDH 26-11-04 CREDIT CLAIM
    lrt_log(LOG_UOS, hh_unit, (BYTE*)pLGUOS);                   // SDH 26-11-04 CREDIT CLAIM

}                                                               // SDH 26-11-04 CREDIT CLAIM

// ------------------------------------------------------------------------------------
//
// UOA - UOD Add
//
// ------------------------------------------------------------------------------------

void UODAdd(char* inbound) {

    BYTE bar_code[11];                                          // CSk 14-12-10 DEFECT 4075
    LONG rc2;                                                   // Streamline SDH 21-Sep-2008

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_UOA *pUOA = (LRT_UOA*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRTLG_UOA *pLGUOA = (LRTLG_UOA*)dtls;                       // SDH 26-11-04 CREDIT CLAIM

    //Set up current date and time                              // SDH 26-11-04 CREDIT CLAIM
    B_DATE nowDate;                                             // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                             // SDH 26-11-04 CREDIT CLAIM
    GetSystemDate(&nowTime, &nowDate);                          // SDH 26-11-04 CREDIT CLAIM

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Set up the record key                                     // SDH 26-11-04 CREDIT CLAIM
    memcpy(ccilfrec.abListNum, pUOA->abListNum,                 // SDH 26-11-04 CREDIT CLAIM
           sizeof(ccilfrec.abListNum));                         // SDH 26-11-04 CREDIT CLAIM
    memcpy(ccilfrec.abRecSeq, pUOA->abRecSeq,                   // SDH 26-11-04 CREDIT CLAIM
           sizeof(ccilfrec.abRecSeq));                          // SDH 26-11-04 CREDIT CLAIM

    //Are we doing a cancel?                                    // SDH 26-11-04 CREDIT CLAIM
    //If so then read the file rather than building             // SDH 26-11-04 CREDIT CLAIM
    //the CCILF record                                          // SDH 26-11-04 CREDIT CLAIM
    if (pUOA->cItemStatus == 'X') {                             // SDH 26-11-04 CREDIT CLAIM
        rc2 = CcilfRead(__LINE__);                              // SDH 26-11-04 CREDIT CLAIM
        if (rc2 <= 0L) {                                        // SDH 26-11-04 CREDIT CLAIM
            prep_nak("ERRORUnable to read CCILF. "              // SDH 26-11-04 CREDIT CLAIM //CSk 15-04-2010 POD Logging
                     "Check appl event log" );                  // SDH 26-11-04 CREDIT CLAIM
            return;                                              // SDH 26-11-04 CREDIT CLAIM
        }                                                       // SDH 26-11-04 CREDIT CLAIM
        //Otherwise build the entire CCILF record               // SDH 26-11-04 CREDIT CLAIM
    } else {                                                    // SDH 26-11-04 CREDIT CLAIM
        
        //Due to situation on Batch POD devices where the business centre defaults to H
        //for all Unknown Items (this means that the Batch POD does not have it in its
        //internal database). It has been decided to check the business centre for all
        //items. If the Boots Item Code is keyed in then we only need to read the IDF.
        if (strncmp( pUOA->abItemCode, "0000000", 7 )==0) {                    // CSk 14-12-10 DEFECT 4075
            // Pack ASCII item code                                            // CSk 14-12-10 DEFECT 4075
            memset(bar_code, 0x00, 11);                                        // CSk 14-12-10 DEFECT 4075
            pack(bar_code+5, 6, pUOA->abBarCode, 12, 0);                       // CSk 14-12-10 DEFECT 4075
            // Read IRF                                                        // CSk 14-12-10 DEFECT 4075
            memcpy( irfrec.bar_code, bar_code, 11 );                           // CSk 14-12-10 DEFECT 4075
            rc2 = IrfRead(__LINE__);                                           // CSk 14-12-10 DEFECT 4075
            if (rc2>0L) {                                                      // CSk 14-12-10 DEFECT 4075
                //IRF read OK, so read IDF to get Business Centre Letter       // CSk 14-12-10 DEFECT 4075
                                                                               // CSk 14-12-10 DEFECT 4075
                calc_boots_cd(idfrec.boots_code, irfrec.boots_code);           // CSk 14-12-10 DEFECT 4075
                // read the IDF for the current item.                          // CSk 14-12-10 DEFECT 4075
                rc2 = IdfRead(__LINE__);                                       // CSk 14-12-10 DEFECT 4075
                                                                               // CSk 14-12-10 DEFECT 4075
                if (rc2>0L) {                                                  // CSk 14-12-10 DEFECT 4075
                    // Set Boots Item Code and Business Centre Letter          // CSk 14-12-10 DEFECT 4075
                   unpack( pUOA->abItemCode, 7, idfrec.boots_code, 4, 1 );     // CSk 14-12-10 DEFECT 4075
                   pUOA->cBusCentre = idfrec.bsns_cntr[0];                     // CSk 14-12-10 DEFECT 4075
                }                                                              // CSk 14-12-10 DEFECT 4075
            }                                                                  // CSk 14-12-10 DEFECT 4075
                                                                               // CSk 14-12-10 DEFECT 4075
        } else {                                                               // CSk 14-12-10 DEFECT 4075
            //Read IDF and set Business centre letter correctly                // CSk 14-12-10 DEFECT 4075
            //Set up packed boots code                                         // CSk 14-12-10 DEFECT 4075
            memset( idfrec.boots_code, 0x00, 4 );                              // CSk 14-12-10 DEFECT 4075
            pack( idfrec.boots_code, 4, pUOA->abItemCode, 7, 1 );              // CSk 14-12-10 DEFECT 4075
                                                                               // CSk 14-12-10 DEFECT 4075
            rc2 = IdfRead(__LINE__);                                           // CSk 14-12-10 DEFECT 4075
                                                                               // CSk 14-12-10 DEFECT 4075
            if (rc2>0L) {                                                      // CSk 14-12-10 DEFECT 4075
                // Set Business Centre Letter                                  // CSk 14-12-10 DEFECT 4075
               pUOA->cBusCentre = idfrec.bsns_cntr[0];                         // CSk 14-12-10 DEFECT 4075
            }                                                                  // CSk 14-12-10 DEFECT 4075
        }                                                                      // CSk 14-12-10 DEFECT 4075
                                                                               
        memcpy(ccilfrec.abItemCode,  pUOA->abItemCode,          // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abItemCode));                    // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abBarCode,   pUOA->abBarCode,           // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abBarCode));                     // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abItemDesc,  pUOA->abDesc,              // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abItemDesc));                    // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abSelDesc,   pUOA->abSelDesc,           // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abSelDesc));                     // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abItemQty,   pUOA->abQty,               // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abItemQty));                     // SDH 26-11-04 CREDIT CLAIM
        memset(ccilfrec.abFiller1, ' ',                         // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abFiller1));                     // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abItemPrice, pUOA->abPrice,             // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abItemPrice));                   // SDH 26-11-04 CREDIT CLAIM
        ccilfrec.cBusCentre = pUOA->cBusCentre;                 // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abBusCentreDesc, pUOA->abBusCentreDesc, // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abBusCentreDesc));               // SDH 26-11-04 CREDIT CLAIM
        sprintf(sbuf, "%04d%02d%02d",                           // SDH 26-11-04 CREDIT CLAIM
                nowDate.wYear,nowDate.wMonth, nowDate.wDay);    // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abDateAdded, sbuf,                      // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abDateAdded));                   // SDH 26-11-04 CREDIT CLAIM
        sprintf(sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin); // SDH 26-11-04 CREDIT CLAIM
        memcpy(ccilfrec.abTimeAdded, sbuf,                      // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abTimeAdded));                   // SDH 26-11-04 CREDIT CLAIM
        memset(ccilfrec.abFiller2, ' ',                         // SDH 26-11-04 CREDIT CLAIM
               sizeof(ccilfrec.abFiller2));                     // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Update the item status                                    // SDH 26-11-04 CREDIT CLAIM
    ccilfrec.cItemStatus = pUOA->cItemStatus;                   // SDH 26-11-04 CREDIT CLAIM

    //Write the CCILF and handle errors                         // SDH 26-11-04 CREDIT CLAIM
    rc2 = CcilfWrite(__LINE__);                                 // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to write to CCILF. "              // SDH 04-05-06 A6C: Bug fix
                 "Check appl event log");                       // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Success - send ACK                                        // SDH 26-11-04 CREDIT CLAIM
    prep_ack("");                                               //SDH 23-Aug-2006 Planners

    //Build audit log                                           // SDH 26-11-04 CREDIT CLAIM
    memcpy(pLGUOA->abListNum, pUOA->abListNum,                  // SDH 26-11-04 CREDIT CLAIM
           sizeof(pLGUOA->abListNum));                          // SDH 26-11-04 CREDIT CLAIM
    memcpy(pLGUOA->abItemCode, pUOA->abItemCode,                // SDH 26-11-04 CREDIT CLAIM
           sizeof(pLGUOA->abItemCode));                         // SDH 26-11-04 CREDIT CLAIM
    pLGUOA->cItemStatus = pUOA->cItemStatus;                    // SDH 26-11-04 CREDIT CLAIM
    lrt_log(LOG_UOA, hh_unit, dtls);                            // SDH 26-11-04 CREDIT CLAIM
                                                                // SDH 26-11-04 CREDIT CLAIM
}                                                               // SDH 26-11-04 CREDIT CLAIM

// -------------------------------------------------------------------------
//
// UOX - UOD Close/dispatch/save
//
// -------------------------------------------------------------------------

void UODExit(char* inbound) {                                   // Streamline SDH 21-Sep-2008

    //Declare working variables                                 // SDH 26-11-04 CREDIT CLAIM
    LONG rc2;                                                   // Streamline SDH 21-Sep-2008
    WORD wListNum;                                              // SDH 26-11-04 CREDIT CLAIM

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_UOX *pUOX = (LRT_UOX*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRTLG_UOX *pLGUOX = (LRTLG_UOX*)dtls;                       // SDH 26-11-04 CREDIT CLAIM

    //Set up current date and time                              // SDH 26-11-04 CREDIT CLAIM
    B_DATE nowDate;                                             // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                             // SDH 26-11-04 CREDIT CLAIM
    GetSystemDate(&nowTime, &nowDate);                          // SDH 26-11-04 CREDIT CLAIM

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    //Store Closed check not required since this is an exit     // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Read CCLOL and handle errors                              // SDH 26-11-04 CREDIT CLAIM
    //We must re-read the record since the terminal does not    // SDH 26-11-04 CREDIT CLAIM
    //pass the date/time list was created                       // SDH 26-11-04 CREDIT CLAIM
    wListNum = satoi(pUOX->abListNum, sizeof(pUOX->abListNum)); // SDH 26-11-04 CREDIT CLAIM
    rc2 = CclolRead(wListNum, __LINE__);                        // SDH 26-11-04 CREDIT CLAIM

    //Handle errors                                             // SDH 26-11-04 CREDIT CLAIM
    //If we're doing a cancel then assume the HHT will          // SDH 26-11-04 CREDIT CLAIM
    //quit out of Credit Claiming, so end the session           // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to "                              //SDH 23-Aug-2006 Planners
                 "read from CCLOL. "                            // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event logs" );                     // SDH 26-11-04 CREDIT CLAIM
        if (pUOX->cStatus == 'C') {                             // SDH 26-11-04 CREDIT CLAIM
            CclolClose(CL_SESSION);                            // SDH 26-11-04 CREDIT CLAIM
            CcilfClose(CL_SESSION);                            // SDH 26-11-04 CREDIT CLAIM
            CchistClose(CL_SESSION);                           // SDH 26-11-04 CREDIT CLAIM
        }                                                       // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Update CCLOL fields                                       // SDH 26-11-04 CREDIT CLAIM
    //Don't update operator ID though since this is expected    // SDH 26-11-04 CREDIT CLAIM
    //not to change                                             // SDH 26-11-04 CREDIT CLAIM
    //Always set the UOD qty to the same as the Item Count      // SDH 26-11-04 CREDIT CLAIM
    //Set the RF Status flag to 'C' for complete                // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cListType = pUOX->cListType;                       // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abUODNum, pUOX->abUODNum,                   // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abUODNum));                          // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abItemCount, pUOX->abItemCount,             // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abItemCount));                       // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abUODQty, pUOX->abItemCount,                // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abUODQty));                          // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cAdjStockFig = pUOX->cAdjStockFig;                 // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cSupplyRoute = pUOX->cSupplyRoute;                 // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cDispLocation = pUOX->cDispLocation;               // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cBusCentre = pUOX->cBusCentre;                     // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abBusCentreDesc, pUOX->abBusCentreDesc,     // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abBusCentreDesc));                   // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abRecallNum, pUOX->abRecallNum,             // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abRecallNum));                       // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abAuth, pUOX->abAuth,                       // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abAuth));                            // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abSupplier, pUOX->abSupplier,               // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abSupplier));                        // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abReturnMethod, pUOX->abReturnMethod,       // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abReturnMethod));                    // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abCarrier, pUOX->abCarrier,                 // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abCarrier));                         // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abBirdNum, pUOX->abBirdNum,                 // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abBirdNum));                         // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abReasonNum, pUOX->abReasonNum,             // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abReasonNum));                       // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abReceivingStoreNum,                        // SDH 26-11-04 CREDIT CLAIM
           pUOX->abReceivingStoreNum,                           // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abReceivingStoreNum));               // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abDestination, pUOX->abDestination,         // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abDestination));                     // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cWarehouseRoute = pUOX->cWarehouseRoute;           // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abUODType, pUOX->abUODType,                 // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abUODType));                         // SDH 26-11-04 CREDIT CLAIM
    memcpy(cclolrec.abDamageRsn, pUOX->abDamageRsn,             // SDH 26-11-04 CREDIT CLAIM
           sizeof(cclolrec.abDamageRsn));                       // SDH 26-11-04 CREDIT CLAIM
    cclolrec.cRFStatus = 'C';                                   // SDH 26-11-04 CREDIT CLAIM

    //If list was dispatched then set the                       // SDH 26-11-04 CREDIT CLAIM
    //date of dispatch to today                                 // SDH 26-11-04 CREDIT CLAIM
    //Then start RFSCC.286 and pass the list number             // SDH 26-11-04 CREDIT CLAIM
    //Only copy the status across if its NOT dispatched         // SDH 26-11-04 CREDIT CLAIM
    //The dispatched flag is set by RFSCC.286 once              // SDH 26-11-04 CREDIT CLAIM
    //successfully completed.                                   // SDH 26-11-04 CREDIT CLAIM
    if (pUOX->cStatus == 'D') {                                 // SDH 26-11-04 CREDIT CLAIM

        //Validate reason code (1-99).                          // SDH 22-06-2006
        //Invalid reason codes have caused problems in store.   // SDH 22-06-2006
        //Allow a space in the first character                  // SDH 22-06-2006
        WORD wRsn = satoi(pUOX->abReasonNum,                    // SDH 17-Oct-06 Bug fix
                          sizeof(pUOX->abReasonNum));           // SDH 17-Oct-06 Bug fix
        if (wRsn < 1 || wRsn > 99) {                            // SDH 17-Oct-06 Bug fix
            sprintf(sbuf, "The Reason Code is invalid: %.2s",   // SDH 22-06-2006
                    pUOX->abReasonNum);                         // SDH 22-06-2006
            prep_nak(sbuf);                                     // SDH 23-Aug-2006 Planners
            return;                                              // SDH 22-06-2006
        }                                                       // SDH 22-06-2006

        sprintf(sbuf, "%04d%02d%02d",                           // SDH 26-11-04 CREDIT CLAIM
                nowDate.wYear, nowDate.wMonth, nowDate.wDay);   // SDH 26-11-04 CREDIT CLAIM
        memcpy(cclolrec.abDateUODDispatched, sbuf,              // SDH 26-11-04 CREDIT CLAIM
               sizeof(cclolrec.abDateUODDispatched));           // SDH 26-11-04 CREDIT CLAIM

        //Update CCHIST                                         // SDH 26-11-04 CREDIT CLAIM
        if ((strncmp(pUOX->abUODNum, "00000000001000",          // SDH 26-11-04 CREDIT CLAIM
                     sizeof(pUOX->abUODNum)) != 0) &&           // SDH 26-11-04 CREDIT CLAIM
            (pUOX->cListType == 'G')) {                         // SDH 26-11-04 CREDIT CLAIM
            memcpy(cchistrec.abUODNum, pUOX->abUODNum,          // SDH 26-11-04 CREDIT CLAIM
                   sizeof(cchistrec.abUODNum));                 // SDH 26-11-04 CREDIT CLAIM
            sprintf(sbuf, "%02d%02d%02d", nowDate.wYear%100,    // SDH 26-11-04 CREDIT CLAIM
                    nowDate.wMonth, nowDate.wDay);              // SDH 26-11-04 CREDIT CLAIM
            memcpy(cchistrec.abDateUODAdded, sbuf,              // SDH 26-11-04 CREDIT CLAIM
                   sizeof(cchistrec.abDateUODAdded));           // SDH 26-11-04 CREDIT CLAIM
            memcpy(cchistrec.abListNum, pUOX->abListNum,        // SDH 26-11-04 CREDIT CLAIM
                   sizeof(cchistrec.abListNum));                // SDH 26-11-04 CREDIT CLAIM
            memset(cchistrec.abFiller, 0xFF,                    // SDH 26-11-04 CREDIT CLAIM
                   sizeof(cchistrec.abFiller));                 // SDH 26-11-04 CREDIT CLAIM
            rc2 = CchistWrite(__LINE__);                        // SDH 26-11-04 CREDIT CLAIM
            if (rc2 <= 0L) {                                    // SDH 26-11-04 CREDIT CLAIM
                prep_nak("ERRORUnable to write to CCHIST. "     // SDH 26-11-04 CREDIT CLAIM
                         "Check appl event log" );              // SDH 26-11-04 CREDIT CLAIM
                CclolClose(CL_SESSION);                        // SDH 23-06-06
                CcilfClose(CL_SESSION);                        // SDH 23-06-06
                CchistClose(CL_SESSION);                       // SDH 23-06-06
                return;                                          // SDH 26-11-04 CREDIT CLAIM
            }                                                   // SDH 26-11-04 CREDIT CLAIM
        }                                                       // SDH 26-11-04 CREDIT CLAIM

    } else {                                                    // SDH 26-11-04 CREDIT CLAIM
        cclolrec.cStatus = pUOX->cStatus;                       // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Write CCLOL record and handle errors                      // SDH 26-11-04 CREDIT CLAIM
    rc2 = CclolWrite(wListNum, __LINE__);                       // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to write to CCLOL. "              // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event log" );                      // SDH 26-11-04 CREDIT CLAIM
        CclolClose(CL_SESSION);                                // SDH 23-06-06
        CcilfClose(CL_SESSION);                                // SDH 23-06-06
        CchistClose(CL_SESSION);                               // SDH 23-06-06
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //If the list is being dispatched, start RFSCC.286 as a     // SDH 26-11-04 CREDIT CLAIM
    //background task.  First build the list name as a string   // SDH 26-11-04 CREDIT CLAIM
    //to pass to the program                                    // SDH 26-11-04 CREDIT CLAIM
    if (pUOX->cStatus == 'D') {                                 // SDH 26-11-04 CREDIT CLAIM
        memcpy(sbuf, cclolrec.abListNum,                        // SDH 26-11-04 CREDIT CLAIM
               sizeof(cclolrec.abListNum));                     // SDH 26-11-04 CREDIT CLAIM
        sbuf[sizeof(cclolrec.abListNum)] = '\0';                // SDH 26-11-04 CREDIT CLAIM
        rc2 = start_background_app("ADX_UPGM:RFSCC.286", sbuf,  // SDH 27-Sep-06 Bug Fix
                                  "Starting RFSCC");            // SDH 26-11-04 CREDIT CLAIM
        if (rc2 < 0) {                                          // SDH 27-Sep-06 Bug Fix
            prep_nak("Could not start RFSCC. Please try again " // SDH 27-Sep-06 Bug Fix
                     "shortly.");                               // SDH 27-Sep-06 Bug Fix
            return;                                              // SDH 27-Sep-06 Bug Fix
        }                                                       // SDH 27-Sep-06 Bug Fix
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Success - send ACK                                        // SDH 26-11-04 CREDIT CLAIM
    prep_ack("");                                               //SDH 23-Aug-2006 Planners

    //Build UOX audit                                           // SDH 26-11-04 CREDIT CLAIM
    memcpy(pLGUOX->abListNum, pUOX->abListNum,
           sizeof(pLGUOX->abListNum));                          // SDH 26-11-04 CREDIT CLAIM
    pLGUOX->cStatus = pUOX->cStatus;                            // SDH 26-11-04 CREDIT CLAIM

    //Close Credit Claiming Files                               // SDH 26-11-04 CREDIT CLAIM
    CclolClose(CL_SESSION);                                    // SDH 26-11-04 CREDIT CLAIM
    CcilfClose(CL_SESSION);                                    // SDH 26-11-04 CREDIT CLAIM
    CchistClose(CL_SESSION);                                   // SDH 26-11-04 CREDIT CLAIM

}                                                               // SDH 26-11-04 CREDIT CLAIM

// ------------------------------------------------------------------------------------
//
// DSS - Get Direct Supplier List Start
//
// ------------------------------------------------------------------------------------

void UODDirectSupplierListStart(char* inbound) {

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRTLG_DSS *pLGDSS = (LRTLG_DSS*)dtls;                       // SDH 26-11-04 CREDIT CLAIM

    UNUSED(inbound);                                            // Streamline SDH 21-Sep-2008

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Open the CCDIRSU for this list retrieval session          // SDH 26-11-04 CREDIT CLAIM
    //Handle errors                                             // SDH 26-11-04 CREDIT CLAIM
    if (CcdirsuOpen() != RC_OK) {                              // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to open CCDIRSU file. "           // SDH 26-11-04 CREDIT CLAIM
                  "Check appl event logs");                     // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Successful open                                           // SDH 26-11-04 CREDIT CLAIM
    prep_ack("");                                               //SDH 23-Aug-2006 Planners

    //Write audit log                                           // SDH 26-11-04 CREDIT CLAIM
    pLGDSS->cSuccess = 'Y';                                     // SDH 26-11-04 CREDIT CLAIM
    lrt_log(LOG_DSS, hh_unit, dtls);                            // SDH 26-11-04 CREDIT CLAIM

}                                                               // SDH 26-11-04 CREDIT CLAIM

// ------------------------------------------------------------------------------------
//
// DSG - Get Direct Supplier List
//
// ------------------------------------------------------------------------------------

void UODGetDirectSupplierList(char* inbound) {

    LONG rc2;                                                   // Streamline SDH 21-Sep-2008

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_DSG *pDSG = (LRT_DSG*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRTLG_DSG *pLGDSG = (LRTLG_DSG*)dtls;                       // SDH 26-11-04 CREDIT CLAIM
    LRT_DSR *pDSR = (LRT_DSR*)out;                              // SDH 26-11-04 CREDIT CLAIM
    LRT_DSE *pDSE = (LRT_DSE*)out;                              // SDH 26-11-04 CREDIT CLAIM

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Set up initial audit fields                               // SDH 26-11-04 CREDIT CLAIM
    pLGDSG->cBusCentre = pDSG->cBusCentre;                      // SDH 26-11-04 CREDIT CLAIM
    memcpy(pLGDSG->abSeqNum, pDSG->abNextSeqNum,                // SDH 26-11-04 CREDIT CLAIM
           sizeof(pLGDSG->abSeqNum));                           // SDH 26-11-04 CREDIT CLAIM

    //Read the requested record in the CCDIRSU                  // SDH 26-11-04 CREDIT CLAIM
    ccdirsurec.cBusCentre = pDSG->cBusCentre;                   // SDH 26-11-04 CREDIT CLAIM
    memcpy(ccdirsurec.abSeqNum, pDSG->abNextSeqNum,             // SDH 26-11-04 CREDIT CLAIM
           sizeof(ccdirsurec.abSeqNum));                        // SDH 26-11-04 CREDIT CLAIM
    rc2 = CcdirsuRead(__LINE__);                                // SDH 26-11-04 CREDIT CLAIM

    //Handle record not found by sending back an end of list    // SDH 26-11-04 CREDIT CLAIM
    //Any other errors generate a NAK                           // SDH 26-11-04 CREDIT CLAIM
    if (rc2 <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIM

        if (rc2&0xffff != 0x06c8) {                             // SDH 26-11-04 CREDIT CLAIM
            prep_nak("ERRORUnable to read from CCDIRSU. "       // SDH 26-11-04 CREDIT CLAIM
                     "Check appl event logs");                  // SDH 26-11-04 CREDIT CLAIM
            CcdirsuClose(CL_SESSION);                          // SDH 26-11-04 CREDIT CLAIM
            return;                                              // SDH 26-11-04 CREDIT CLAIM
        }                                                       // SDH 26-11-04 CREDIT CLAIM

        //Build the reponse and the audit                       // SDH 26-11-04 CREDIT CLAIM
        memcpy(pDSE->abCmd, "DSE", sizeof(pDSE->abCmd));        // SDH 26-11-04 CREDIT CLAIM
        out_lth = LRT_DSE_LTH;                                  // SDH 26-11-04 CREDIT CLAIM
        memcpy(pLGDSG->abSupplierNum, "EOF   ",                 // SDH 26-11-04 CREDIT CLAIM
               sizeof(pLGDSG->abSupplierNum));                  // SDH 26-11-04 CREDIT CLAIM

        //Close the file now, as its been fully processed       // SDH 26-11-04 CREDIT CLAIM
        CcdirsuClose(CL_SESSION);                              // SDH 26-11-04 CREDIT CLAIM

    //Else success, build DSR and audit                         // SDH 26-11-04 CREDIT CLAIM
    } else {                                                    // SDH 26-11-04 CREDIT CLAIM

        memcpy(pDSR->abCmd, "DSR", sizeof(pDSR->abCmd));        // SDH 26-11-04 CREDIT CLAIM
        pDSR->cBusCentre = ccdirsurec.cBusCentre;               // SDH 26-11-04 CREDIT CLAIM
        memcpy(pDSR->abSeqNum, ccdirsurec.abSeqNum,             // SDH 26-11-04 CREDIT CLAIM
               sizeof(pDSR->abSeqNum));                         // SDH 26-11-04 CREDIT CLAIM
        memcpy(pDSR->abSupplierNum, ccdirsurec.abSupplierNum,   // SDH 26-11-04 CREDIT CLAIM
               sizeof(pDSR->abSupplierNum));                    // SDH 26-11-04 CREDIT CLAIM
        memcpy(pDSR->abSupplierName, ccdirsurec.abSupplierName, // SDH 26-11-04 CREDIT CLAIM
               sizeof(pDSR->abSupplierName));                   // SDH 26-11-04 CREDIT CLAIM
        out_lth = LRT_DSR_LTH;                                  // SDH 26-11-04 CREDIT CLAIM
        memcpy(pLGDSG->abSupplierNum, ccdirsurec.abSupplierName,    // SDH 26-11-04 CREDIT CLAIM
               sizeof(pLGDSG->abSupplierNum));                  // SDH 26-11-04 CREDIT CLAIM

    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Commit audit log                                          // SDH 26-11-04 CREDIT CLAIM
    lrt_log(LOG_DSG, hh_unit, dtls);                            // SDH 26-11-04 CREDIT CLAIM

}                                                               // SDH 26-11-04 CREDIT CLAIM

// ------------------------------------------------------------------------------------
//
// UOQ - UOD Label Enquiry
//
// ------------------------------------------------------------------------------------

void UODLabelEnquiry(char* inbound) {

    LONG rc2;                                                   // Streamline SDH 21-Sep-2008

    //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_UOQ *pUOQ = (LRT_UOQ*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRTLG_UOQ *pLGUOQ = (LRTLG_UOQ*)dtls;                       // SDH 26-11-04 CREDIT CLAIM

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //Check whether the required record is present              // SDH 26-11-04 CREDIT CLAIM
    memcpy(cchistrec.abUODNum, pUOQ->abUODNum,                  // SDH 26-11-04 CREDIT CLAIM
           sizeof(cchistrec.abUODNum));                         // SDH 26-11-04 CREDIT CLAIM
    rc2 = CchistRead(__LINE__);                                 // SDH 26-11-04 CREDIT CLAIM

    //No error - Label already used - Send NAK                  // SDH 26-11-04 CREDIT CLAIM
    //Record not found - label NOT used before - send an ACK    // SDH 26-11-04 CREDIT CLAIM
    //Other errors - send an error NAK                          // SDH 26-11-04 CREDIT CLAIM
    if (rc2 > 0) {                                              // SDH 26-11-04 CREDIT CLAIM
        prep_nak("UOD label has previously been used.");        // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM
    if (rc2&0xffff != 0x06c8) {                                 // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORUnable to read from CCHIST. "            // SDH 26-11-04 CREDIT CLAIM
                 "Check appl event logs");                      // SDH 26-11-04 CREDIT CLAIM
        return;                                                  // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Good - no record found                                    // SDH 26-11-04 CREDIT CLAIM
    prep_ack("");                                               //SDH 23-Aug-2006 Planners
    pack(pLGUOQ->abUODPacked, 7, pUOQ->abUODNum, 14, 0);        // SDH 26-11-04 CREDIT CLAIM
    pLGUOQ->cDuplicate = 'N';                                   // SDH 26-11-04 CREDIT CLAIM
    lrt_log(LOG_UOQ, hh_unit, dtls);                            // SDH 26-11-04 CREDIT CLAIM

}                                                               // SDH 26-11-04 CREDIT CLAIM

// ------------------------------------------------------------------------------------
//
// STQ - Stock Take Query
//
// ------------------------------------------------------------------------------------

void UODStockTakeQuery(char* inbound) {

   //Working vars                                              // SDH 26-11-04 CREDIT CLAIM
   LONG rc2;                                                   // Streamline SDH 21-Sep-2008
   LONG lSeqNum;                                               // SDH 26-11-04 CREDIT CLAIM
   CCHIST_HOME *pHome = (CCHIST_HOME*)&cchistrec;              // SDH 26-11-04 CREDIT CLAIM

   //Input and output views                                    // SDH 26-11-04 CREDIT CLAIM
   LRT_STQ* pSTQ = (LRT_STQ*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
   LRTLG_STQ* pLGSTQ = (LRTLG_STQ*)dtls;                       // SDH 26-11-04 CREDIT CLAIM
   LRT_STR* pSTR = (LRT_STR*)out;                              // SDH 26-11-04 CREDIT CLAIM

   //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
   if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
   if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
   UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

   //Build home record key from prefix                         // SDH 26-11-04 CREDIT CLAIM
   //Prefix + "000000"                                         // SDH 26-11-04 CREDIT CLAIM
   //and Read the home record from the CCHIST                  // SDH 26-11-04 CREDIT CLAIM
   memset(&cchistrec, 0xFF, sizeof(cchistrec));                // SDH 26-11-04 CREDIT CLAIM
   memset(cchistrec.abUODNum, '0', sizeof(cchistrec.abUODNum));// SDH 26-11-04 CREDIT CLAIM
   memcpy(cchistrec.abUODNum, pSTQ->abUODPrefix,               // SDH 26-11-04 CREDIT CLAIM
          sizeof(pSTQ->abUODPrefix));                          // SDH 26-11-04 CREDIT CLAIM
   rc2 = CchistRead(__LINE__);                                 // SDH 26-11-04 CREDIT CLAIM

   //If no error then extract the seq num as a long            // SDH 26-11-04 CREDIT CLAIM
   //Send NAK for errors other than 'record not found'         // SDH 26-11-04 CREDIT CLAIM
   //If record not found error then assume seq num is 0        // SDH 26-11-04 CREDIT CLAIM
   if (rc2 > 0) {                                              // SDH 26-11-04 CREDIT CLAIM
       lSeqNum = satol(pHome->abLastSeq,                       // SDH 26-11-04 CREDIT CLAIM
                       sizeof(pHome->abLastSeq));              // SDH 26-11-04 CREDIT CLAIM
   } else {                                                    // SDH 26-11-04 CREDIT CLAIM
       if (rc2&0xffff != 0x06c8) {                             // SDH 26-11-04 CREDIT CLAIM
           prep_nak("ERRORUnable to read from CCHIST. "        // SDH 26-11-04 CREDIT CLAIM
                    "Check appl event logs");                  // SDH 26-11-04 CREDIT CLAIM
           return;                                              // SDH 26-11-04 CREDIT CLAIM
       }                                                       // SDH 26-11-04 CREDIT CLAIM
       lSeqNum = 0;                                            // SDH 26-11-04 CREDIT CLAIM
   }                                                           // SDH 26-11-04 CREDIT CLAIM

   //Increment seq num (next after 9999 is 1)                  // SDH 26-11-04 CREDIT CLAIM
   lSeqNum = (lSeqNum%9999) + 1;                               // SDH 26-11-04 CREDIT CLAIM
   sprintf(sbuf, "%06ld", lSeqNum);                            // SDH 26-11-04 CREDIT CLAIM
   memcpy(pHome->abLastSeq, sbuf, sizeof(pHome->abLastSeq));   // SDH 26-11-04 CREDIT CLAIM

   //Write the home record to CCHIST                           // SDH 26-11-04 CREDIT CLAIM
   rc2 = CchistWrite(__LINE__);                                // SDH 26-11-04 CREDIT CLAIM
   if (rc2 <= 0) {                                             // SDH 26-11-04 CREDIT CLAIM
       prep_nak("ERRORUnable to write to CCHIST. "             // SDH 26-11-04 CREDIT CLAIM
                "Check appl event logs");                      // SDH 26-11-04 CREDIT CLAIM
       return;                                                  // SDH 26-11-04 CREDIT CLAIM
   }                                                           // SDH 26-11-04 CREDIT CLAIM

   //Build the STR response                                    // SDH 26-11-04 CREDIT CLAIM
   memcpy(pSTR->abCmd, "STR", 3);                              // SDH 26-11-04 CREDIT CLAIM
   memcpy(pSTR->abUODPrefix, pSTQ->abUODPrefix,                // SDH 26-11-04 CREDIT CLAIM
          sizeof(pSTR->abUODPrefix));                          // SDH 26-11-04 CREDIT CLAIM
   sprintf(sbuf, "%06ld", lSeqNum);                            // SDH 26-11-04 CREDIT CLAIM
   memcpy(pSTR->abUODSuffix, sbuf, sizeof(pSTR->abUODSuffix)); // SDH 26-11-04 CREDIT CLAIM
   out_lth = LRT_STR_LTH;                                      // SDH 26-11-04 CREDIT CLAIM

   //Build audit                                               // SDH 26-11-04 CREDIT CLAIM
   pack(pLGSTQ->abUODPrefPkd, 4, pSTR->abUODPrefix, 8, 0);     // SDH 26-11-04 CREDIT CLAIM
   pack(pLGSTQ->abUODSuffPkd, 3, pSTR->abUODSuffix, 6, 0);     // SDH 26-11-04 CREDIT CLAIM
   lrt_log(LOG_STQ, hh_unit, dtls);                            // SDH 26-11-04 CREDIT CLAIM

}                                                              // SDH 26-11-04 CREDIT CLAIM


