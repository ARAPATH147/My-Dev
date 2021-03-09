#ifndef CCDMY_H
#define CCDMY_H

#include "trxfile.h"

//---- CCLOL ----                                                           // SDH 26-11-04 CREDIT CLAIM
typedef struct CCLOL_Record_Layout {                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cListType; //"G" - Goods out, "C" - credit claim                   // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODNum[14];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cStatus;                                                           // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemCount[4];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODQty[4];                                                       // SDH 26-11-04 CREDIT CLAIM
    BYTE cAdjStockFig;  //"Y" or "N"                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE cSupplyRoute;                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cDispLocation;                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abBusCentreDesc[14];                                               // SDH 26-11-04 CREDIT CLAIM
    BYTE abRecallNum[8];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abAuth[15];                                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplier[15];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abReturnMethod[2];                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE abCarrier[2];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abBirdNum[8];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abReasonNum[2];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abReceivingStoreNum[4];                                            // SDH 26-11-04 CREDIT CLAIM
    BYTE abDestination[2];                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE cWarehouseRoute;                                                   // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODType[2];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abDamageRsn[2];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abDateUODOpened[8];        //YYYYMMDD                              // SDH 26-11-04 CREDIT CLAIM
    BYTE abDateUODDispatched[8];    //YYYYMMDD                              // SDH 26-11-04 CREDIT CLAIM
    BYTE abTimeUODOpened[4];        //HHMM                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpID[3];                                                         // SDH 26-11-04 CREDIT CLAIM
    BYTE abOpName[15];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cRFStatus;                 //'A' active, 'C' complete              // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller[2];                                                       // SDH 26-11-04 CREDIT CLAIM
} CCLOL_REC;                                                                // SDH 26-11-04 CREDIT CLAIM

//---- CCILF ----                                                           // SDH 17-11-04 CREDIT CLAIM
typedef struct CCILF_Record_Layout {                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abRecSeq[4];                                                       // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemCode[7];                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abBarCode[13];                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemDesc[20];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abSelDesc[45];                                                     // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemQty[4];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller1[6];      //Unused                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abItemPrice[6];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abBusCentreDesc[14];                                               // SDH 26-11-04 CREDIT CLAIM
    BYTE abDateAdded[8];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE abTimeAdded[4];                                                    // SDH 26-11-04 CREDIT CLAIM
    BYTE cItemStatus;   //'A' added, 'X' cancelled                          // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller2[3];                                                      // SDH 26-11-04 CREDIT CLAIM
} CCILF_REC;                                                                // SDH 26-11-04 CREDIT CLAIM

//---- CCHIST ----                                                          // SDH 17-11-04 CREDIT CLAIM
typedef struct {                                       // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODNum[14];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abDateUODAdded[6];                                                 // SDH 26-11-04 CREDIT CLAIM
    BYTE abListNum[4];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller[8];                                                       // SDH 26-11-04 CREDIT CLAIM
} CCHIST_REC;                                                               // SDH 26-11-04 CREDIT CLAIM
typedef struct {                                         // SDH 26-11-04 CREDIT CLAIM
    BYTE abUODNum[14];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abLastSeq[6];                                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE abFiller[12];                                                      // SDH 26-11-04 CREDIT CLAIM
} CCHIST_HOME;                                                              // SDH 26-11-04 CREDIT CLAIM

//---- CCDIRSU ----                                                         // SDH 17-11-04 CREDIT CLAIM
typedef struct CCDIRSU_Record_Layout {                                      // SDH 26-11-04 CREDIT CLAIM
    BYTE cBusCentre;                                                        // SDH 26-11-04 CREDIT CLAIM
    BYTE abSeqNum[4];                                                       // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplierNum[6];                                                  // SDH 26-11-04 CREDIT CLAIM
    BYTE abSupplierName[10];                                                // SDH 26-11-04 CREDIT CLAIM
} CCDIRSU_REC;                                                              // SDH 26-11-04 CREDIT CLAIM


extern FILE_CTRL ccdmy;
extern CCLOL_REC cclolrec;
extern CCILF_REC ccilfrec;
extern CCHIST_REC cchistrec;
extern CCDIRSU_REC ccdirsurec;


void CcdmySet(void);
URC CcdmyOpenLocked(void);
URC CcdmyClose(void);
void CclolSet(void);
URC CclolOpen(void);
URC CclolClose(WORD type);
LONG CclolRead(LONG lRecNum, LONG lLineNum);
LONG CclolWrite(LONG lRecNum, LONG lLineNum);
void CcilfSet(void);
URC CcilfOpen(void);
URC CcilfClose(WORD type);
LONG CcilfRead(LONG lLineNumber);
LONG CcilfWrite(LONG lLineNumber);
void CchistSet(void);
URC CchistOpen(void);
URC CchistClose(WORD type);
LONG CchistRead(LONG lLineNumber);
LONG CchistWrite(LONG lLineNumber);
void CcdirsuSet(void);
URC CcdirsuOpen(void);
URC CcdirsuClose(WORD type);
LONG CcdirsuRead(LONG lLineNumber);

#endif

