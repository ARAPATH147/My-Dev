//--------------------------------------------------------------------------------
// 1.00 - PAB 22-5-2007 
//       Changes to include transaction types for recalls A7C
// 
// As the recall files are only to be available to TRANS02 they are all 
// defined in seperate header file.
// This is because TRANSACT will fail to compile if any more arrays are defined
// as the 64k LIMIT has been reached for that object.
//
// Version 1.1               Brian Greenfield            7th September 2007
// Altered aRecalls to add citemFlag to pass through item session count status.
//
// Version 1.2               Brian Greenfield            3rd January 2008
// Added WriteToFile WRF definitions.
//
// Version 1.3               Brian Greenfield            14th April 2008
// Reverse Logistics Changes
//
// Version 1.4               Brian Greenfield            14th April 2008
// Further Reverse Logistics Changes
// RCC Expiry date (added in 1.3) removed as expiry date now passed in 
// active date field.
// -------------------------------------------------------------------------------


URC open_rcindx(void);                                                     // PAB 22-5-2007 Recalls
LONG ReadRcindx(LONG lLineNumber);                                         // PAB 22-5-2007 Recalls
URC close_rcindx(WORD type);                                               // PAB 22-5-2007 Recalls
URC open_recall(void);                                                     // PAB 22-5-2007 Recalls
LONG ReadRecall(LONG lLineNumber);                                         // PAB 22-5-2007 Recalls
LONG WriteRecall(LONG lLineNumber);                                        // PAB 22-5-2007 Recalls
URC close_recall(WORD type);                                               // PAB 22-5-2007 Recalls
URC open_recok(void);                                                      // PAB 22-5-2007 Recalls
URC close_recok(WORD type);                                                // PAB 22-5-2007 Recalls
LONG Readrecok(LONG lRecNum, LONG lLineNumber);                            // PAB 22-5-2007 Recalls
LONG Writerecok(LONG lRecNum, LONG lLineNumber);                           // PAB 22-5-2007 Recalls
URC open_rcspi(void);                                                      // PAB 22-5-2007 Recalls
LONG ReadRcspi(LONG lLineNumber);                                          // PAB 22-5-2007 Recalls
URC close_rcspi(WORD type);                                                // PAB 22-5-2007 Recalls
LONG CheckRecallAvailable(void );
LONG ProcessPainkiller(void);                                             // PAB 08-1107 Mobile Printing

// Recall Transactions are defined below.

typedef struct LRT_RCA_Txn {
   BYTE cmd[3];
   BYTE opid[3];                             
} LRT_RCA;                                    
#define LRT_RCA_LTH sizeof(LRT_RCA)

typedef struct LRT_RCB_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anRecallNumber[22];                           // recal ref (8) uod (14)
   BYTE cListStatus[1];                               // "P","A"," "
} LRT_RCB;                                    
#define LRT_RCB_LTH sizeof(LRT_RCB)

typedef struct LRT_RCC_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE Index[4];
   BYTE anRecallRef[8];                               
   BYTE cRecallType[1];
   BYTE abRecallDesc[20];
   BYTE anRecallCnt[4];
   BYTE cActiveDate[8];                          // YYYYMMDD
   BYTE cMsgAvail[1];                            // "Y" is msg available on Recall Msg File.
   BYTE cLabelType[2];
   BYTE cMRQ[2];                                 // 2 digit Minimun Return Quantity // 05-05-2008 1.4 BMG
} LRT_RCC;                                    
#define LRT_RCC_LTH sizeof(LRT_RCC)

typedef struct LRT_RCD_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anIndex[4];                             
   BYTE cRecallType[1];
} LRT_RCD;                                    
#define LRT_RCD_LTH sizeof(LRT_RCD)

typedef struct LRT_RCE_Txn {
   BYTE cmd[3];
   BYTE opid[3];                            
} LRT_RCE;                                    
#define LRT_RCE_LTH sizeof(LRT_RCE)

typedef struct {                                                                       
    BYTE anRecallItem[6];                                                          
    BYTE abItemDesc[20];
    BYTE anItemTSF[4];
    BYTE anRecCnt[4];                      // 20-6-2007 PAB
    BYTE cItemFlag[1];                     // 07-09-2007 1.1 BMG
} aRecalls;                                                                         

typedef struct LRT_RCF_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anRecallRef[8];
   BYTE cMoreItems[1];                      // "Y" if more items to come
   BYTE cStatus[1];                        // status of recall from file.
   aRecalls abItemArray[10];               // Pre-Pack with "FFFFFF"
} LRT_RCF;                                    
#define LRT_RCF_LTH sizeof(LRT_RCF)

typedef struct LRT_RCG_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anRecallRef[8];
   BYTE anRecallItem[6];                 // Boots Code no check digit unpacked
   BYTE anRecallCount[4];                // item count to update recall file
} LRT_RCG;                                    
#define LRT_RCG_LTH sizeof(LRT_RCG)

typedef struct LRT_RCH_Txn {
   BYTE cmd[3];
   BYTE opid[3];                  
   BYTE anRecallRef[8];
   BYTE anItemPtr[4];                     // pointer to item in recall record -> start "0000"
} LRT_RCH;                                    
#define LRT_RCH_LTH sizeof(LRT_RCH)

typedef struct LRT_RCI_Txn {
   BYTE cmd[3];
   BYTE opid[3];                  
   BYTE anRecallRef[8];
} LRT_RCI;                                    
#define LRT_RCI_LTH sizeof(LRT_RCI)

typedef struct LRT_RCJ_Txn {
   BYTE cmd[3];
   BYTE opid[3];                  
   BYTE anRecallRef[8];
   BYTE abSpecialIns[160];
} LRT_RCJ;                                    
#define LRT_RCJ_LTH sizeof(LRT_RCJ)

typedef struct {                          // BMG 03-Jan-2008 1.2
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anFilePath[64];
   BYTE cCreateFile[1];
   BYTE anDataLength[4];
   BYTE abData[2000];
} LRT_WRF;                                    
#define LRT_WRF_LTH sizeof(LRT_WRF)

