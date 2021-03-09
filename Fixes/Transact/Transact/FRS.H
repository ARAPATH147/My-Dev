// 2.1 - SW - 04/11/1998
//       Phase II enhancements
//       Add extra details into ENQUIRY structure
// 3.0 - SW - 28/08/2003
//       2003 Trial updates
//       Amend General_Enquiry struct to hold an active deal flag instead of
//       containing the deal number. Actual new deals info not required.
//
//4.02 - PAB 18/02/2004
//       2004 RF Trial Phase2 development
//       Changes to SNR and PRT transaction layouts for proximity printing
//
// 5.00 - PAB 5/5/04
//       Changes to include Pre-Scan transactions
//
// 6.00 - PAB 7-7-2004
//       Changes for MyStoreNet to include new transaction types
//
// 7.00 - PAB 22-5-2007
//       Changes to include transaction types for recalls A7C
//
// 8.00 - PAB 7-8-2007
//       new transaction for Mobile Printing
//
// 9.00 - BMG 11-09-2007
//       New printer status PST_STALLED
//       Added last_access_time to PROG_PQ.
//       Moved DealEnquiry to here from transact.c.
//       Moved dump_pq_stack to here from rfs.c
//       Increased size of sales values in LRT_SSR and TILLREC.
//
// 10.00 - BMG 03-01-2008
//       Added WRF.
//
// 11.00 - BMG 06-02-2008
//       Added markdown flag to ENQUIRY and EQR structures.
//
// 12.00 - BMG 12-08-2008
//       Added changes for Multi-Sited Counts including adding new MSA & MSB messages
//       Altered process_gap and trans04_LRT_PRT function calls.
//
// 13.00 - BMG 01-09-2008
//       Added commands for ASN/Directs.
//
// 14.00 - BMG 10-09-2008
//       Added ALR command for MC70 device support and modified SOR
//       request with additiona fields.
//       Extended SOR request.
//       Added prototype for Wriekeyed.
//
// 15.00 - BMG 17-12-2009
//       Changes to support RF Stabilisation - new ANK internal command
//
// 16.00 - CSk 15-04-2010
//       Changes to support POD Logging Enhancements.
//       Constant MAX_POD_FILES.
//       RFS_VER raised to "VERSION : 10A.1".
//
// 17.00 - CSk 11-05-2010
//       Changes to support Auto Fast Fill - adding Product Group
//       to ENQUIRY structure.
//       RFS_VER raised to "VERSION : 10A.2".
//
// 18.00 - CSk 13-05-2010
//       Changes to support 2010 Recalls
//       RFS_VER raised to "VERSION : 11A.0".
//
// 19.00 - CSk 30-11-2010
//       Changes to support 2010 Recall Improvements
//       Defect 4075: change to TRXUOD to set
//       RFS_VER raised to "VERSION : 11A.3".
//
// 20.00 - CSk 21-02-2010
//       Changes to support 2010 Recall Improvements
//       Defect 4982: CR05 - Add Batch Numbers to RCC transaction
//       Defect 4985: Add PPC/POD - Expired Recalls should be not be passed to the devices.
//       RFS_VER raised to "VERSION : 11A.4".
//
// 21.00 - CSk 8-12-2011
//       Stuart's DEC Socket fix
//       RFS_VER raised to "VERSION : 12A.2".
//
// 22.00 - CSk 12-03-2012
//       Stock File Accuracy  (commented: //CSk 12-03-2012 SFA)
//       RFS_VER raised to "VERSION : 12C.0".
//       Defined LRTLG record Type: LOG_CLL        59
//       Revised CLI message structure
//       Added new CLA message structure
//       Added new CLD message structure
//
// 23.00 - CSk 22-06-2012
//       Stock File Accuracy  (commented: //CSk 12-03-2012 SFA - Locked Picking List Fix)
//       Locked Picking List Fix - Picker ID added to PLL message.
//       RFS_VER raised to "VERSION : 12C.1".
//
// 24.00 - Tat 04-07-2012
//       Stock File Accuracy  (commented: //Tat 04-07-2012 SFA)
//       Missing count lists message Fix - Last count date added to CLL message -
//       see clolf_get_next().
//       RFS_VER raised to "VERSION : 12C.2".
//
// 25.00 - CSk 07-08-2012
//       Stock File Accuracy  (commented: //CSk 07-08-2012 SFA)
//       Fix to clilf_get_next() function which was incrementing the Uncounted item
//       count for every item regardless of status which resulted in an incorrect
//       CLI message response.
//       RFS_VER raised to "VERSION : 12C.3".
//
// 26.00 - Tat 11-08-2012
//       Stock File Accuracy  (commented: //Tat 11-08-2012 SFA)
//       Fix to change the message length for CLX to 8.
//       RFS_VER raised to "VERSION : 12C.4".
//
// 27.00 - Tat 14-08-2012
//       Stock File Accuracy  (commented: //TAT 14-08-2012 SFA)
//       Fix to remove unlocking clilf on updating the list once a CLX is received.
//       RFS_VER raised to "VERSION : 12C.5".
//
// 28.00 - Tat 16-08-2012
//       Stock File Accuracy  (commented: //TAT 16-08-2012 SFA)
//       Fixed to update CLOLF record LIST.STATUS to P on a CLX with a commit, if
//       not all counts are counted.
//       RFS_VER raised to "VERSION : 12C.6".
//
// 29.00 - Tat 20-08-2012
//       Stock File Accuracy  (commented: //TAT 20-08-2012 SFA)
//       Fixed to make the count a LONG value instead of WORD to suite the
//       STKMQ type 13 format.(QC Defect : 493)
//       RFS_VER raised to "VERSION : 12C.7".
//
// 30.00 - Tat 23-08-2012
//       Stock File Accuracy  (commented: //TAT 23-08-2012 SFA)
//       Fixed to insert supporting record in MINLS for Stock support to pick the
//       STKMQ entried for count updates.(QC Defect : 493, 550)
//       RFS_VER raised to "VERSION : 12C.8".
//
// 31.00 - Tat 10-09-2012
//       Stock File Accuracy  (commented: //TAT 10-09-2012 SFA)
//       Fixed to update the count start time and also correctly update the
//       count end time.(QC Defect : 653)
//       RFS_VER raised to "VERSION : 12C.9".
//
// 32.00 - CSk 11-09-2012
//       Stock File Accuracy  (commented: //CSk 11-09-2012 SFA)
//       QC Defect: 663 - where a User Generated count list is created from the
//            Back Shop location on a batch-POD device which is then docked
//            and transmitted prior to performing the Sales Floor count.
//            Selection of the list after download results in no planner
//            sites. This fix ensures that the items are written to the CLOLF
//            & CLILF in order that ACTBUILD will add the list to the COUNT.CSV
//            file.
//
//       RFS_VER raised to "VERSION : 12C.10".
//
// 33.00 - TAT 25-09-2012
//       Stock File Accuracy  (commented: //TAT 25-09-2012 SFA)
//       QC Defect: 698 - Fixed to include the PILST id and PIITM number in
//            head office counts while updating the STKMQ.
//       Also fixed the item sequence numbers in the CLI messages
//
// 34.00 - TAT 24-10-2012
//       Stock File Accuracy  (commented: //TAT 24-10-2012 SFA)
//       QC Defect: 770 - Fixed to sent count lists with 'Active' and 'In creation'
//            status as well to the HH in a CLL.
//       QC Defect: 767 & 777 - Fixed to resolve the issue where the the PILST id and
//            PIITM number in head office counts while updating the STKMQ, were over
//            written.
//       QC Defect: 693 & 694 - Sets the picker id and pick start time as well when
//            the first item is added to a user generated count list.
//
//       RFS_VER raised to "VERSION : 13A.2".
//
// 35.00 - TAT 02-11-2012
//       Stock File Accuracy  (commented: //TAT 02-11-2012 SFA)
//       Fixed to support the state of clilf status 'Partially counted'.
//
//       RFS_VER raised to "VERSION : 13A.3".
//
// 36.00 - TAT 12-11-2012
//       Stock File Accuracy  (commented: //TAT 12-11-2012 SFA)
//       Fixed to set the counts always are recounts to ensure that the counts
//       are not discarded by stock support.
//       Also merged in the locked picking list fix from production
//
//       RFS_VER raised to "VERSION : 13A.4".
//
// 37.00 - TAT 16-11-2012
//       Stock File Accuracy  (commented: //TAT 16-11-2012 SFA)
//       Fix for defect in updating stock count. Packed string not always
//       processed as a string. Also includes a temporary fix for defect 799.
//       By deleting the last item in a list for a user generated count list
//       if no counts were receivied.
//
//       RFS_VER raised to "VERSION : 13A.5".
//
// 38.00 - TAT 26-11-2012
//       Stock File Accuracy  (commented: //TAT 26-11-2012 SFA)
//       Fixed to check for active count lists and set them to partially
//       counted only for a proper signon and not during a reconnect.
//       Also fixed transact to set the count as a Recount for all counts except
//       for support office count lists. (SFA defect 813)
//       Also fix for SFA defect 799.
//       Also changed the version to 13A.7 to match the EPoS release version.
//
//       RFS_VER raised to "VERSION : 13A.7".
//
// 39.00 - TAT 27-11-2012
//       Stock File Accuracy  (commented: //TAT 27-11-2012 SFA)
//       An item in an active sales plan planner included in pending list if
//       there is another fragment beyond PSP lead time. (SFA defect 818)
//
// 40.00 - TAT 03-12-2012
//       Stock File Accuracy  (commented: //TAT 03-12-2012 SFA)
//       Replaced the SEL description with item description for items with the
//       SEL description marked as 'X'. (SFA defect 821)
//       Updated the CLL message with the outstanding shop floor, Back shop
//       and OSSR counts instead of total shop floor, back shop or OSSR
//       counts. (SFA defect 823)
//
//       RFS_VER raised to "VERSION : 13A.8".
//
// 40.01 - TAT 06-12-2012
//       Stock File Accuracy  (commented: //TAT 06-12-2012 SFA)
//       Replaced the SEL description with item description for items with the
//       SEL description marked as 'X ' or 'x ' in Item enquiry and picking list
//       (SFA defect 821)
//
//       RFS_VER raised to "VERSION : 13A.9".
//
// 40.02 - TAT 14-12-2012
//       Stock File Accuracy  (commented: //TAT 14-12-2012 SFA)
//       Fixed the CLD message processing to avoid incrementing the total item
//       count if an item is being overwritten.
//       (SFA defect 837)
//
//       RFS_VER raised to "VERSION : 13A.10".
//
// 40.03 - TAT 18-12-2012
//       Stock File Accuracy  (commented: //TAT 18-12-2012 SFA)
//       Fixed the processing of a CLX to decrement the outstanding number of
//       shop floor, back shop and ossr counts as well, while removing an item
//       not counted from CLILF. for user generated count list.
//       (SFA defect 837)
//
//       RFS_VER raised to "VERSION : 13A.11".
// -------------------------------------------------------------------------------

//#include "flexif.h"

//#include "trans2.h"
//#include "dateconv.h"


#if !defined RFS_H
#define RFS_H 1

#include "rfsfile.h"
#include <time.h>                           // 11-09-2007 9 BMG

#define RFS_VER  "VERSION : 13A.11"         // Overall RFS Version Number            //TAT 18-12-2012 SFA
#define RFS_DATE __DATE__                   // Compile date
#define SEM_47_RUNNING  "pi:sellockb"       // PSS47 running semephore
#define SEM_LAB_RUNNING "pi:sellockp"       // PRINTSEL running semephore
#define SEM_CB_RUNNING "pi:cblockdp"        // PSD62 ASN/Directs running semephore   //BMG 01-09-2008 13 ASN/Directs
#define SEM_PUB_RUNNING "pi:ssc01pip"       // SSC01 Positive UOD running semephore  //BMG 01-09-2008 13 ASN/Directs
#define SEM_DIR_RUNNING "pi:psbxxpip"       // PSBXX Directs running semephore       //BMG 01-09-2008 13 ASN/Directs
//#define MAX_UNIT       120                // largest possible unit number
#define MAX_UNIT         255                // largest possible unit number
//#define MAX_CONC_UNITS 40                 // Max. concurrent units
#define MAX_CONC_UNITS   255                // Max. concurrent units
#define ALL_UNITS        -1                 // All units flag for unit dealloc
#define CHECK_G          2200               // number of cycles between checks  was 18 PAB
#define REPORT_DATASIZE  20                 // record size of data blocks
#define REPORT_BUFFER    0x4000L            // report buffer size
#define REPORT_SEQ_SIZE  4                  // Report Command SEQ Size
#define MAX_POD_FILES    52                 // Max. Ref/Active files in SYNCTRL.DAT  //CSk 15-04-2010 POD Logging Enhancements

// Performance tuning options
#define HEADER_MAX       6                  // Max. no. of RLD concatinations
#define DETAIL_MAX       10                 // Max. no. of RUP concatinations

//Debug constants
#define DBG_LOCAL       0
#define DBG_FILE        1

// Report buffer
typedef struct Report_Buffer {
   LONG base;                       // base offset into physical file
   LONG end;                        // end offset of data (rec algnd)
   BYTE *buff;                      // pointer to buffer
} RBUF;

typedef struct FileNum_Table {
   LONG fnumx;
   BYTE ftype[1];
} FILE_TABLE;

typedef struct Active_LRT_Table {
   UBYTE state;
   LONG fnum1;             // work file 1 fnum (CHKWK...)
   LONG fnum2;             // work file 2 fnum (GAPWK...)
   LONG fnum3;             // current report file fnum
   LONG lCBfnum;           // Carton buffer file file number        //BMG 01-09-2008 13 ASN/Directs
   LONG lPUBfnum;          // +ve UOD buufer file file number       //BMG 01-09-2008 13 ASN/Directs
   LONG lDIRfnum;          // Directs buufer file file number       //BMG 01-09-2008 13 ASN/Directs
   WORD pq_sub1;           // work file 1 pq subscript
   WORD pq_sub2;           // work file 2 pq subscript
   WORD pq_CB;             // Carton buffer file pq subscript       //BMG 01-09-2008 13 ASN/Directs
   WORD pq_PUB;            // +ve UOD buffer file pq subscript      //BMG 01-09-2008 13 ASN/Directs
   WORD pq_DIR;            // Directs buffer file pq subscript      //BMG 01-09-2008 13 ASN/Directs
   LONG last_active_time;  // time last message received
   BYTE user[3];           // user id
   BYTE abOpName[15];      // Operator name
   BYTE authority[1];      // user's authority
   WORD count1;            // misc count for this handheld
   WORD count2;            // misc count for this handheld
   RBUF *rbufp;            // pointer to report buffer struct
   BYTE txn[3];            // last transaction received (shared info)
   BYTE unique[5];         // txn specific, unique info (shared info)
   BYTE bLocation;         // [U]nknown, [S]hop, [O]SSR                     //SDH 19-01-2005
   BYTE Type[1];           // Device Type [R] = RF PPC, [M] = MC70          //BMG 10-09-2008 14 MC70
   BYTE MAC[12];           // MAC address of device if supplied             // BMG 17-Dec-2009 RF Stabilisation
   WORD wRecallCclol;  // Current Cclol list for recalls        // Mult UOD Rcll 12-8-2011 SDH
} ACTIVE_LRT;                               // Units currently being used

typedef struct Program_Process_Queue {
   UBYTE state;                     // current processing state
   WORD unit;                       // handheld identifier
   BYTE fname[32];                  // work file name
   BYTE type;                       // work file ind. (SYS_LAB/GAP/CB/PUB) //BMG 01-09-2008 13 ASN/Directs
   WORD submitcnt;                  // 26-1-2007 PAB
   time_t last_access_time;         // Last time of file access 11-09-2007 9 BMG
   BYTE DevType[1];                 // Device Type [R] = RF PPC, [M] = MC70 //BMG 10-09-2008 14 MC70
// BOOLEAN disp;                    // displayed pq entry
// BOOLEAN disp_err;                // displayed pq error
} PROG_PQ;

// v4.01 START
//// 4690 file functions
//typedef struct C4690_File_Control {
//   LONG fnum;
//   UBYTE present;
//   UBYTE sessions;
//} FILE_CTRL;
// v4.01 END

// Shared memory buffer
typedef struct SMem_Status {
   BYTE state[1];          // state
   LONG last_active_time;  // time last txn received
   BYTE user[3];           // user id
   BYTE txn[3];            // last transaction received
   BYTE unique[5];         // variable data, depends on txn
} SHARE_STAT_REC;

// Misc
typedef struct Till_Rec {
   DOUBLE today;            // 11-09-2007 9 BMG
   DOUBLE wtd;              // 11-09-2007 9 BMG
} TILLREC;
typedef struct Till_Takings_Rec {
   TILLREC till[1000];
} TILL_TAKINGS;
//TILL_TAKINGS_REC_LTH = sizeof(TILL_TAKINGS);

// v2.1
typedef struct LRT_PLL_Txn {
   BYTE cmd[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE list_status[1];       // [A]=Supervisor, [U]=normal user
   BYTE stamp[12];            // YYYYMMDDHHMM
   BYTE lines[4];
   BYTE username[15];
   BYTE cLocation;            // [S]helf Monitor, [F]ast Fill, [O]ssr, [E]xcess stock
   BYTE abPickerId[3];        // CSk 12-03-2012 SFA - Locked Picking List Fix
} LRT_PLL;                    // Picking list response
#define LRT_PLL_LTH sizeof(LRT_PLL)

// v2.1

// v2.1
typedef struct LRT_PLI_Txn {
   BYTE cmd[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE boots_code[7];
   BYTE parent_code[7];
   BYTE item_desc[20];
   BYTE fill[4];
   BYTE status[1];
   BYTE gap_flag[1];
   BYTE active_deal_flag[1];                                         // v2.1
   BYTE stock_figure[6];                                             // v2.1
   BYTE sel_desc[45];                                                // v2.1
   BYTE item_code[13];                                               // v2.1
   BYTE pli_qtyshelf[4];                                             // 19-02-04 PAB
   BYTE abBackCount[4];                                              // SDH 17-11-04 OSSR WAN
   BYTE cOssrItem;                                                   // SDH 17-11-04 OSSR WAN
   BYTE MultiSited[1];  // Multi-Site Item "Y" or " "                //BMG 12-Aug-2008 Multi-Sited Counts
} LRT_PLI;                                                           // Picking list item
#define LRT_PLI_LTH sizeof(LRT_PLI)

// v2.1

// v2.1

// v2.1

// v2.1


// Item Information
// v2.1

// v2.1

// v2.1

// v2.1

// v2.1

// v2.1

// v2.1
typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE item_code[13];
} LRT_PLU;                                    // Item enquiry request
#define LRT_PLU_LTH sizeof(LRT_PLU)

// v2.1
typedef struct {
   BYTE cmd[3];
   BYTE boots_code[7];
   BYTE item_desc[20];
   BYTE item_price[6];
   BYTE stock_figure[6];
   BYTE sel_desc[45];
   BYTE supply_method[1];
   BYTE redeemable[1];
} LRT_RSP;                                    // PLU response
#define LRT_RSP_LTH sizeof(LRT_RSP)

// v2.1

//MObile Printing PAB 07-08-07

// Reporting
typedef struct LRT_RPO_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_RPO;                                    // Reports sign on
#define LRT_RPO_LTH sizeof(LRT_RPO)

typedef struct LRT_RPX_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_RPX;                                    // Reports exit
#define LRT_RPX_LTH sizeof(LRT_RPX)

// v2.1

// v2.1
typedef struct LRT_RLR_Txn {
   BYTE cmd[3];
   BYTE seq[4];
   BYTE title[REPORT_DATASIZE];
   BYTE fname[12];
} LRT_RLR;                                    // Reports response
#define LRT_RLR_LTH sizeof(LRT_RLR)

// v2.1

// v2.1
typedef struct {
    BYTE seq[REPORT_SEQ_SIZE];
    BYTE data[REPORT_DATASIZE];
} LRT_RLD_REP;
#define LRT_RLD_REP_LTH sizeof(LRT_RLD_REP)
typedef struct LRT_RLD_Txn {
    BYTE cmd[3];
    BYTE rpt[3];
    LRT_RLD_REP rep[HEADER_MAX];
} LRT_RLD;                                   // Report headers (level 0s)
#define RLD_REP_OFFSET  6                    // Length on non-repeating data
#define LRT_RLD_MAX_LTH sizeof(LRT_RLD)      // N.B. variable length

// v2.1
typedef struct LRT_RPS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE fname[12];
   BYTE seq[REPORT_SEQ_SIZE];
} LRT_RPS;                                    // Report data request
#define LRT_RPS_LTH sizeof(LRT_RPS)

// v2.1
typedef struct LRT_RUP_Rpt {
   BYTE level[1];
   BYTE exp[1];
   BYTE data[REPORT_DATASIZE];
} LRT_RUP_REP;
#define LRT_RUP_REP_LTH sizeof(LRT_RUP_REP)
typedef struct LRT_RUP_Txn {
   BYTE cmd[3];
   BYTE rpt[3];
   LRT_RUP_REP rep[DETAIL_MAX];
} LRT_RUP;                                    // Report detail (level 1+s)
#define RUP_REP_OFFSET  6                // Length on non-repeating data
#define LRT_RUP_MAX_LTH sizeof(LRT_RUP)        // N.B. Variable length

// v2.1


// List-Driven Counting (all new to v2.1)
// v2.1

//---------------------
//       CLL
//---------------------

// Changes req'd : CLL equiv
typedef struct LRT_CLL_Txn {
   BYTE cmd[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE num_items[3];         // Total number of items
   BYTE items_shopfloor[3];   // Items left on shopfloor
   BYTE items_backshop[3];    // Items left on backshop
   BYTE list_type[1];         // [H]ead office / [R]ectification / [N]egative / [U]ser Generated  //CSk 12-03-2012 SFA
   BYTE last_count_date[8];   // Date of last count -Default - blank        //Tat 04-07-2012 SFA
   BYTE bus_unit_name[30];                                                  //CSk 12-03-2012 SFA
   BYTE active[1];
   BYTE abItemsOssr[3];       // SDH 17-01-2005 OSSR WAN
} LRT_CLL;                                    // Counting - List Ack
#define LRT_CLL_LTH sizeof(LRT_CLL)


//---------------------
//       CLI
//---------------------

typedef struct { // CLI Multi-Item Record                                   //CSk 12-03-2012 SFA
    BYTE abItemSeq[3];                                                      //CSk 12-03-2012 SFA
    BYTE abItemcode[7];                                                     //CSk 12-03-2012 SFA
    BYTE abParentCode[7];                                                   //CSk 12-03-2012 SFA
    BYTE abBarcode[13];                                                     //CSk 12-03-2012 SFA
    BYTE abSelDesc[45];                                                     //CSk 12-03-2012 SFA
    BYTE cActiveDealFlag;     // 'Y' or 'N'                                 //CSk 12-03-2012 SFA
    BYTE abProductGroup[6];                                                 //CSk 12-03-2012 SFA
    BYTE abCountBackshop[4];                                                //CSk 12-03-2012 SFA
    BYTE abCountPendingBackshop[4];                                         //CSk 12-03-2012 SFA
    BYTE abCountShopfloor[4];                                               //CSk 12-03-2012 SFA
    BYTE cStatus;             // ' ' active                                 //CSk 12-03-2012 SFA
                              // 'X' discontinued                           //CSk 12-03-2012 SFA
                              // 'Z' discontinued                           //CSk 12-03-2012 SFA
    BYTE abOSSRCount[4];                                                    //CSk 12-03-2012 SFA
    BYTE abPendingOSSRCount[4];                                             //CSk 12-03-2012 SFA
    BYTE cOssrItem;           // 'O' change to OSSR item                    //CSk 12-03-2012 SFA
                              // 'N' change to store item                   //CSk 12-03-2012 SFA
                              // ' ' no change required                     //CSk 12-03-2012 SFA
                                                                            //CSk 12-03-2012 SFA
    BYTE abLastCountDate[8];  // CCYYMMDD - Default = spaces                //CSk 12-03-2012 SFA
    BYTE cPendingSaleFlag;    // 'Y' if on pending sales plan else a space  //CSk 12-03-2012 SFA
    BYTE abStockFigure[6];    // -99999 to 999999                           //CSk 12-03-2012 SFA
} CLI_ITEM_REC;                                                             //CSk 12-03-2012 SFA



typedef struct LRT_CLI_Txn {
   BYTE cmd[3];                      // LRT_CLI_HDRSIZE
   BYTE list_id[3];                  // LRT_CLI_HDRSIZE
   BYTE abNumItemsInList[3];         // LRT_CLI_HDRSIZE                     //CSk 12-03-2012 SFA
   BYTE abNextItemSeq[3];            // LRT_CLI_HDRSIZE                     //CSk 12-03-2012 SFA
   BYTE cMoreToCome;                 // LRT_CLI_HDRSIZE                     //CSk 12-03-2012 SFA

   CLI_ITEM_REC CLI_items[8];                                               //CSk 12-03-2012 SFA

} LRT_CLI;                                                                  // Counting - List Item in List
#define LRT_CLI_LTH sizeof(LRT_CLI)
#define LRT_CLI_HDRSIZE 13                                                  //CSk 12-03-2012 SFA
// When a CLD request is received we only want to return ONE item
// unlike a CLS request which can return info for upto 8 items.
// Redefining the length means less data and greater speed.
#define LRT_CLI_CLD_LTH sizeof(CLI_ITEM_REC)+LRT_CLI_HDRSIZE                //CSk 12-03-2012 SFA


// v2.1
//---------------------
//       CLF
//---------------------

typedef struct LRT_CLF_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_CLF;                                    // Counting - list main exit
#define LRT_CLF_LTH sizeof(LRT_CLF)

// Version 5.0 Pre-Scan                                             5-5-4 PAB
                                                                 // 5-5-2004 PAB
                                                                 // 5-5-2004 PAB

// PAstraMI support - v3.0
/*
typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_PAS;                                    // PAstraMI signon request
#define LRT_PAS_LTH sizeof(LRT_PAS)

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_PAX;                                    // PAstraMI signoff
#define LRT_PAX_LTH sizeof(LRT_PAX)

typedef struct {
    BYTE cmd[3];
    WORD recnum;
    WORD rectot;
    WORD reclth;
    BYTE debug[100];    // Variable length
} LRT_PDT;                                    // PAstraMI debug dump line
#define LRT_PDT_LTH sizeof(LRT_PDT)

typedef struct LRT_PDR_Txn {
   BYTE cmd[3];
} LRT_PDR;                                    // PAstraMI debug dump response
#define LRT_PDR_LTH sizeof(LRT_PDR)

*/

typedef struct LRT_ALR_Txn {                                                            //BMG 10-09-2008 14 MC70
    BYTE cmd[3];                                                                        //BMG 10-09-2008 14 MC70
    BYTE opid[3];                                                                       //BMG 10-09-2008 14 MC70
    BYTE application[8];                                                                //BMG 10-09-2008 14 MC70
} LRT_ALR;                                                                              //BMG 10-09-2008 14 MC70
#define LRT_ALR_LTH sizeof(LRT_ALR)                                                     //BMG 10-09-2008 14 MC70

typedef struct LRT_ANK_Txn {                                                            // BMG 17-Dec-2009 RF Stabilisation
    BYTE abText[40];                                                                    // BMG 17-Dec-2009 RF Stabilisation
} LRT_ANK;                                                                              // BMG 17-Dec-2009 RF Stabilisation
#define LRT_ANK_LTH sizeof(LRT_ANK)                                                     // BMG 17-Dec-2009 RF Stabilisation

// NOTE the Keys below are in INTEL format                                              // SDH 26-11-04 CREDIT CLAIM
// so reverse the key, OR use the macro below                                           // SDH 26-11-04 CREDIT CLAIM
#define CMD(a,b,c) ((LONG)c<<16) + ((LONG)b<<8) + (LONG)a                               // SDH 26-11-04 CREDIT CLAIM

// LRT Command Lookup (inbound commands)
// For inbound messages, add the command message definition                             // BMG 17-Dec-2009 RF Stabilisation
#define CMD_NULL  0x00000000L        // 15-7-2004 to trap null commands PAB
#define CMD_ALR   CMD('A','L','R')                                                      // BMG 10-09-2008 14 MC70
#define CMD_ALR_LTH 14                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_ANK   CMD('A','N','K')                                                      // BMG 17-Dec-2009 RF Stabilisation
#define CMD_ANK_LTH 40                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_CLA   0x00414C43L                                                           //CSk 12-03-2012 SFA
#define CMD_CLA_LTH 10                                                                  //CSk 12-03-2012 SFA
#define CMD_CLC   0x00434C43L
#define CMD_CLC_LTH 34                                                                  // BMG 17-Dec-2009 RF Stabilisation //CSk 12-03-2012 SFA
#define CMD_CLD   0x00444C43L                                                           //CSk 12-03-2012 SFA
#define CMD_CLD_LTH 23                                                                  //CSk 12-03-2012 SFA
#define CMD_CLF   0x00464C43L
#define CMD_CLF_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_CLO   0x004F4C43L
#define CMD_CLO_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_CLR   0x00524C43L
#define CMD_CLR_LTH 9                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_CLS   0x00534C43L
#define CMD_CLS_LTH 12                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_CLX   0x00584C43L
#define CMD_CLX_LTH 8                                                                   // BMG 17-Dec-2009 RF Stabilisation //Tat 11-08-2012 SFA
#define CMD_DNQ   CMD('D','N','Q')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_DNQ_LTH 10                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_DSS   CMD('D','S','S')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_DSS_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_DSG   CMD('D','S','G')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_DSG_LTH 11                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_ENQ   0x00514E45L
#define CMD_ENQ_LTH 24                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GAP   0x00504147L
#define CMD_GAP_LTH 50                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GAS   0x00534147L
#define CMD_GAS_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GAX   0x00584147L
#define CMD_GAX_LTH 21                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GIA   CMD('G','I','A')                                                      //BMG 01-09-2008 13 ASN/Directs
#define CMD_GIA_LTH 16                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GIF   CMD('G','I','F')                                                      //BMG 01-09-2008 13 ASN/Directs
#define CMD_GIF_LTH 86                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GIQ   CMD('G','I','Q')                                                      //BMG 01-09-2008 13 ASN/Directs
#define CMD_GIQ_LTH 42                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_GIX   CMD('G','I','X')                                                      //BMG 01-09-2008 13 ASN/Directs
#define CMD_GIX_LTH 9                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_INS   0x00534E49L
#define CMD_INS_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_INX   0x00584E49L
#define CMD_INX_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_ISE   0x00455349L
#define CMD_ISE_LTH 19                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_LOG   CMD('L','O','G')                                          // CSk 15-04-2010 POD Logging
#define CMD_LOG_LTH 83                                                      // CSk 15-04-2010 POD Logging
#define CMD_MSA   CMD('M','S','A')                                          // BMG 12-Aug-2008 Multi-Sited Counts
#define CMD_MSA_LTH 12                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_MSB   CMD('M','S','B')                                          // BMG 12-Aug-2008 Multi-Sited Counts
#define CMD_OFF   0x0046464FL
#define CMD_OFF_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PAL   0x004C4150L
#define CMD_PAR   0x00524150L
#define CMD_PAS   0x00534150L
#define CMD_PAX   0x00584150L
#define CMD_PCD   0x00444350L
#define CMD_PCM   0x004D4350L
#define CMD_PCM_LTH 20                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PCS   0x00534350L
#define CMD_PCS_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PCX   0x00584350L
#define CMD_PCX_LTH 14                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PDT   0x00544450L
#define CMD_PGF   CMD('P','G','F')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGF_LTH 12                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGG   CMD('P','G','G')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGI   CMD('P','G','I')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGL   CMD('P','G','L')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGL_LTH 19                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGA   CMD('P','G','A')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGA_LTH 22                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGM   CMD('P','G','M')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGM_LTH 21                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGN   CMD('P','G','N')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGQ   CMD('P','G','Q')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGQ_LTH 13                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGR   CMD('P','G','R')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGS   CMD('P','G','S')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGS_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PGX   CMD('P','G','X')                                          //SDH 14-Sep-2006 Planners
#define CMD_PGX_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLC   0x00434C50L
#define CMD_PLC_LTH 33                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLF   0x00464C50L
#define CMD_PLF_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLO   0x004F4C50L
#define CMD_PLO_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLR   0x00524C50L
#define CMD_PLR_LTH 10                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLS   0x00534C50L
#define CMD_PLS_LTH 12                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PLU   0x00554C50L
#define CMD_PLX   0x00584C50L
#define CMD_PLX_LTH 20                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PPL   CMD('P','P','L')                                          //SDH 14-Sep-2006 Planners
#define CMD_PPR   CMD('P','P','R')                                          //SDH 14-Sep-2006 Planners
#define CMD_PRP   CMD('P','R','P')                                          //SDH 14-Sep-2006 Planners
#define CMD_PRP_LTH 16                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PRT   0x00545250L
#define CMD_PRT_LTH 20                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PSL   CMD('P','S','L')                                          //SDH 14-Sep-2006 Planners
#define CMD_PSL_LTH 22                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_PSR   CMD('P','S','R')                                          //SDH 14-Sep-2006 Planners
#define CMD_RCA   CMD('R','C','A')                                         // PAB 22-map-2007 Recalls
#define CMD_RCA_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCB   CMD('R','C','B')                                         // PAB 22-map-2007 Recalls
#define CMD_RCB_LTH 29                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCC   CMD('R','C','C')                                         // PAB 22-map-2007 Recalls
#define CMD_RCD   CMD('R','C','D')                                         // PAB 22-map-2007 Recalls
#define CMD_RCD_LTH 11                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCE   CMD('R','C','E')                                         // PAB 22-map-2007 Recalls
#define CMD_RCF   CMD('R','C','F')                                         // PAB 22-map-2007 Recalls
#define CMD_RCG   CMD('R','C','G')                                         // PAB 22-map-2007 Recalls
#define CMD_RCG_LTH 24                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCH   CMD('R','C','H')                                         // PAB 22-map-2007 Recalls
#define CMD_RCH_LTH 18                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCI   CMD('R','C','I')                                         // PAB 22-map-2007 Recalls
#define CMD_RCI_LTH 14                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCJ   CMD('R','C','J')                                         // PAB 22-map-2007 Recalls
#define CMD_RCN   CMD('R','C','N')                                                      // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RCN_LTH 55                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RLE   0x00454C52L
#define CMD_RLE_LTH 10                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RLS   0x00534C52L
#define CMD_RLS_LTH 22                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RPO   0x004F5052L
#define CMD_RPO_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RPS   0x00535052L
#define CMD_RPS_LTH 22                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RPX   0x00585052L
#define CMD_RPX_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_RSP   0x00505352L
#define CMD_SAK   0x0053414BL       // PAB 5-5-4 Pre-Scan
#define CMD_SIE   0x00454953L
#define CMD_SIE_LTH 3                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_SIR   0x00534953L
#define CMD_SOR   0x00524F53L
#define CMD_SOR_LTH1 9                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_SOR_LTH2 12                                                                 // BMG 17-Dec-2009 RF Stabilisation
#define CMD_SOR_LTH3 52                                                                 // BMG 17-Dec-2009 RF Stabilisation
#define CMD_SUS   0x00535553L       // PAB 5-5-4 Pre-Scan
#define CMD_SSE   0x00455353L
#define CMD_SSE_LTH 6                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_STQ   CMD('S','T','Q')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_STQ_LTH 14                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_UIS   CMD('U','I','S')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UIG   CMD('U','I','G')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOA   CMD('U','O','A')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOA_LTH 131                                                                 // BMG 17-Dec-2009 RF Stabilisation
#define CMD_UOD   CMD('U','O','D')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOQ   CMD('U','O','Q')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOQ_LTH 20                                                                  // BMG 17-Dec-2009 RF Stabilisation
#define CMD_UOS   CMD('U','O','S')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOS_LTH 7                                                                   // BMG 17-Dec-2009 RF Stabilisation
#define CMD_UOX   CMD('U','O','X')                                                      // SDH 26-11-04 CREDIT CLAIM
#define CMD_UOX_LTH 111                                                                 // BMG 17-Dec-2009 RF Stabilisation
#define CMD_UNK   0x00FFFFFFL
#define CMD_WRF   CMD('W','R','F')                                                      // BMG 03-Jan-2008 10
#define CMD_XXX   0x00585858L       // Low-level handheld transactions - keep alive

//DEALSUM - Used by EQR and ENQUIRY
typedef struct {
    BYTE abNum[4];
    BYTE abQualMsgNum[2];
} DEALSUM;

typedef struct {
    BYTE boots_code[7];         // IRF
    BYTE parent_code[7];        // IDF
    BYTE item_desc[24];         // IDF
    BYTE item_price[6];         // IRF
    BYTE sel_desc[45];          // ISF
    BYTE status[1];             // IDF
    BYTE stock_figure[6];       // VARIOUS                             v2.1
    BYTE supply_method[1];      // IDF
    BYTE redeemable[1];         // IRF
    //BYTE deal_num[4];         // IRF                                 v3.0
    BYTE active_deal_flag[1];   // IRF                                 v3.0
    BYTE item_code[13];         // IRF (incl. calculated check digit)  v2.1
    BYTE items_sold_today[6];   // IMSTC (singles, not 1/100ths)       v2.1
    UBYTE idf_bit_flags_2;      // IDF                                 v2.1
    BYTE pcheck_exempt[1];      // PGF YN //PAB 23-10-03
    BYTE date_last_delivery[3]; // STOCK  //PAB 20-10-04
    BYTE cBusCentre;            // IDF    //SDH 17-11-04 OSSR WAN
    BYTE cOssrItem;             // RFHIST/PGF 'Y' or 'N'                    //SDH 17-11-04 OSSR WAN
    BYTE cPgfOssrFlag;          // PGF 'Y' or 'N'                           //SDH 18-03-05 OSSR WAN
    DEALSUM Deal[10];           // Internal deal table                      //SDH 17-11-04 OSSR WAN
    BYTE abCoreCount[3];        // From SRITML                              //SDH 22-Sep-2006 Planners
    BYTE abNonCoreCount[3];     // From SRITML                              //SDH 22-Sep-2006 Planners
    BYTE cRecallFlag[1];        // From IRF                                 //PAB 22-May-2007 Recalls
    BYTE cMarkdown[1];          // From IDF 'Y' or ' '                      //BMG 06-02-2008 1.11
    BYTE cProductGrp[6];        // From IDF 99/9999                         //CSk 11-05-2010 Auto Fast Fill
    BYTE cRecallType;           //                                          //CSk 11-05-2010 Recalls Phase 1
} ENQUIRY;                      // General enquiry block
#define ENQUIRY_LTH sizeof(ENQUIRY)


//LOG                                                                       //CSk 15-04-2010 POD Logging
typedef struct {                                                            //CSk 15-04-2010 POD Logging
    BYTE baCmd[3];                // TID - Txn ID                           //CSk 15-04-2010 POD Logging
    BYTE baOpid[3];               // Operator ID: 000 - 999                 //CSk 15-04-2010 POD Logging
    BYTE baMAC[12];               // MAC address of device                  //CSk 15-04-2010 POD Logging
    BYTE baIP[15];                // IP address of device                   //CSk 15-04-2010 POD Logging
    BYTE baDate[6];               // Device date: YYMMDD                    //CSk 15-04-2010 POD Logging
    BYTE baTime[6];               // Device time: HHMMSS                    //CSk 15-04-2010 POD Logging
    BYTE baSynFileNum[2];         // SYNCTRL File No. 01 - 17               //CSk 15-04-2010 POD Logging
                                  //     01 = BOOTCODE.CSV (Reference)      //CSk 15-04-2010 POD Logging
                                  //     02 = BARCODE.CSV  (Reference)      //CSk 15-04-2010 POD Logging
                                  //     03 = DEAL.CSV     (Reference)      //CSk 15-04-2010 POD Logging
                                  //     04 = PGROUP.CSV   (Reference)      //CSk 15-04-2010 POD Logging
                                  //     05 = SUPPLIER.CSV (Reference)      //CSk 15-04-2010 POD Logging
                                  //     06 = USERS.CSV    (Reference)      //CSk 15-04-2010 POD Logging
                                  //     07 = RECALL.CSV   (Reference)      //CSk 15-04-2010 POD Logging
                                  //     08 = LIVEPOG.CSV  (Reference)      //CSk 15-04-2010 POD Logging
                                  //     09 = CATEGORY.CSV (Reference)      //CSk 15-04-2010 POD Logging
                                  //     10 = MODULE.CSV   (Reference)      //CSk 15-04-2010 POD Logging
                                  //     11 = CONTROL.CSV  (Active)         //CSk 15-04-2010 POD Logging
                                  //     12 = PICKING.CSV  (Active)         //CSk 15-04-2010 POD Logging
                                  //     13 = COUNT.CSV    (Active)         //CSk 15-04-2010 POD Logging
                                  //     14 = CREDIT.CSV   (Active)         //CSk 15-04-2010 POD Logging
                                  //     15 = DIRECTS.CSV  (Active)         //CSk 15-04-2010 POD Logging
                                  //     16 = ASN.CSV      (Active)         //CSk 15-04-2010 POD Logging
                                  //     17 = SSCUODOT.CSV (Active)         //CSk 15-04-2010 POD Logging
    BYTE cAction;                 // "L" = TFTP Load,  "S" = TFTP Send,     //CSk 15-04-2010 POD Logging
                                  // "F" = FTP Load,   "P" = FTP Send       //CSk 15-04-2010 POD Logging
    BYTE cStatus;                 // "S" = Start, "E" = OK, "X" = Abend     //CSk 15-04-2010 POD Logging
    BYTE baFileName[12];          // aaaaaaaa.bbb                           //CSk 15-04-2010 POD Logging
    BYTE baReason[2];             // Reason for logging code: 00 - 99       //CSk 15-04-2010 POD Logging
    BYTE baText[20];              // eg. "OK", "Fail - Time Out", etc.      //CSk 15-04-2010 POD Logging
} LRT_PODLOG;                    // POD LOG message structure               //CSk 15-04-2010 POD Logging
#define PODLOG_LTH sizeof(LRT_PODLOG)                                       //CSk 15-04-2010 POD Logging





// Global Return Code Stuff
typedef WORD URC;
#define RC_FATAL_ERR     (-128)  // Serious error - Application must end
#define RC_SERIOUS_ERR   (-100)  // Serious error - Application can continue
#define RC_FILE_ERR       (-40)  // File(s) missing at point of access
#define RC_DATA_ERR       (-30)  // General file error - see appl event logs
#define RC_IGNORE_ERR      (-1)  // Error can be ignored - see appl event logs
#define RC_OK               (0)  // Success

// LRT Application States
#define ST_FREE         0                       // Must be zero
#define ST_LOGGED_ON    1
#define ST_RECEIVING    2
#define ST_CLOSING      3
#define ST_STACKED      4

// Process queue states
#define PST_FREE        0
#define PST_ALLOC       1
#define PST_READY       2
#define PST_ADOPTED     3   //SDH 22-June-2006
#define PST_RUNNING     4   //PAB 26-January-2007
#define PST_STALLED     5   // 11-09-2007 9 BMG

// File close flags
#define CL_SESSION      0
#define CL_ALL          1

// Access conflict parms
#define AC_RETRY_DELAY  10
#define AC_MAX_RETRY    12

// gap system flag settings
#define SYS_LAB         0
#define SYS_GAP         1
#define SYS_ORPHAN      2
#define SYS_CB          3  // ASN Cartons Buffer  //BMG 01-09-2008 13 ASN/Directs
#define SYS_PUB         4  // Positive UOD Buffer //BMG 01-09-2008 13 ASN/Directs
#define SYS_DIR         5  // Directs Buffer      //BMG 01-09-2008 13 ASN/Directs

// audit file (LRTLG) record types
#define LOG_SOR         0
#define LOG_OFF         1
#define LOG_GAS         2
#define LOG_PCM         3
#define LOG_GAX         4
#define LOG_PLX         5
#define LOG_PCS         6
#define LOG_PCX         7
#define LOG_PRT         8
#define LOG_CSS         9   // <v4.0
#define LOG_CSE        10   // <v4.0
#define LOG_TVR        11   // <v4.0
#define LOG_CRQ        12   // <v4.0
#define LOG_CLS        13   // v4.0
#define LOG_CLC        14   // v4.0
#define LOG_CLX        15   // v4.0
#define LOG_SUS        16   // 5-5-04 PAB
#define LOG_ENQ        17   // 2-7-04 PAB
#define LOG_UOS        18
#define LOG_UOA        19
#define LOG_UOX        20
#define LOG_UOQ        21
#define LOG_DSS        22
#define LOG_DSG        23
#define LOG_STQ        24
#define LOG_DNQ        25
#define LOG_PGS        26
#define LOG_PGX        27
#define LOG_PGF        28
#define LOG_PGQ        29
#define LOG_PGM        30
#define LOG_PPL        31
#define LOG_PSL        32
#define LOG_PGL        33
#define LOG_PRP        34
#define LOG_RCA        35  // 22-5-07 PAB Recalls
#define LOG_RCB        36  // 22-5-07 PAB Recalls
//#define LOG_RCC        37  // 22-5-07 PAB Recalls
#define LOG_RCD        38  // 22-5-07 PAB Recalls
//#define LOG_RCE        39  // 22-5-07 PAB Recalls
//#define LOG_RCF        40  // 22-5-07 PAB Recalls
//#define LOG_RCG        41  // 22-5-07 PAB Recalls
#define LOG_RCH        42  // 22-5-07 PAB Recalls
#define LOG_RCI        43  // 22-5-07 PAB Recalls
#define LOG RCJ        44  // 22-5-07 PAB Recalls
#define LOG_WRF        45  // BMG 03-Jan-2008 10
#define LOG_MSA        46  // BMG 12-Aug-2008 Multi-Sited Counts
#define LOG_MSB        47  // BMG 12-Aug-2008 Multi-Sited Counts
#define LOG_GIA        48  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GIB        49  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GIC        50  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GID        51  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GIE        52  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GIF        53  // BMG 01-09-2008 13 ASN/Directs
#define LOG_GIX        54  // BMG 01-09-2008 13 ASN/Directs
#define LOG_PLC        55  // SDH 20-May-2009 Model Day
#define LOG_PGA        56  // SDH 20-May-2009 Model Day
#define LOG_ANK        57  // BMG 17-Dec-2009 RF Stabilisation
//#define LOG_LOG        58  //CSk 15-04-2010 POD Logging
#define LOG_CLL        59  //CSk 12-03-2012 SFA

// Stock enquiry types
#define SENQ_BOOTS      0
#define SENQ_DESC       1
#define SENQ_SELD       2
#define SENQ_TSF        3

//////////////////////////////////////////////////////////////////////////////
/// Define log levels.
/// LOG_CRITICAL means log all errors except record not found or EOF
//////////////////////////////////////////////////////////////////////////////
#define LOG_NONE     0
#define LOG_CRITICAL 1
#define LOG_ALL      2

#endif /* RFS_H not defined */
// recall file globals


