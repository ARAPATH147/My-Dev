// ****************************************************************************
// v1.0
// v2.0 - 12-03-2012 - Stock File Accuracy - Charles Skadorwa (CSk)
//                     Replaced CLOLF and CLILF layouts with new layouts.
//                     New function declaration: ClilfWrite().
// ****************************************************************************

#ifndef CLFILES_H
#define CLFILES_H

#include "trxfile.h"

//------------------------------------------------                              //CSk 12-03-2012 SFA
//         CLOLF - Count List Of Lists                                          //CSk 12-03-2012 SFA
//------------------------------------------------                              //CSk 12-03-2012 SFA
typedef struct CLOLF_Record_Layout {                                            //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abListId[3];                                                           //CSk 12-03-2012 SFA
    BYTE abCreatorId[3];                                                        //CSk 12-03-2012 SFA
    BYTE bListType;           // N - Negative count list                        //CSk 12-03-2012 SFA
                              // P - PI count list                              //CSk 12-03-2012 SFA
                              // C - User created count list                    //CSk 12-03-2012 SFA
                              // R - Recount list                               //CSk 12-03-2012 SFA
                              // S - Shelf monitor pick/count list (future)     //CSk 12-03-2012 SFA
                              // E - Excess stock pick/count list (future)      //CSk 12-03-2012 SFA
                              // F - Fast fill pick list (future)               //CSk 12-03-2012 SFA
                              // A - Auto fast fill pick list (future)          //CSk 12-03-2012 SFA
    BYTE bBusCentLet;         // Bus Centre Letter (only set for HO else space) //CSk 12-03-2012 SFA
    BYTE abListName[30];      // List name displayed on HHT                     //CSk 12-03-2012 SFA
    BYTE abPickerId[3];       // Last person to count the list                  //CSk 12-03-2012 SFA
    BYTE bListStatus;         // I - Initial                                    //CSk 12-03-2012 SFA
                              // A - Active                                     //CSk 12-03-2012 SFA
                              // C - Complete                                   //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abPILSTID[4];        // Support Office Count List ID eg. 9803          //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abCreateDate[3];     // 3-PD YYMMDD                                    //CSk 12-03-2012 SFA
    BYTE abCreateTime[2];     // 2-PD HHMM                                      //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abExpiryDate[3];     // 3-PD YYMMDD                                    //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abPickStartTime[2];  // 2-PD HHMM                                      //CSk 12-03-2012 SFA
    BYTE abPickEndTime[2];    // 2-PD HHMM                                      //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE bCurrentLocation;    // S - Sales floor                                //CSk 12-03-2012 SFA
                              // B - Back shop                                  //CSk 12-03-2012 SFA
                              // O - OSSR                                       //CSk 12-03-2012 SFA
    UINT uiTotalItems;                                                          //CSk 12-03-2012 SFA
    UINT uiOutSalesFloorCnt;                                                    //CSk 12-03-2012 SFA
    UINT uiOutBackShopCnt;                                                      //CSk 12-03-2012 SFA
    UINT uiOutOSSRCnt;                                                          //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
} CLOLF_REC;                                                                    //CSk 12-03-2012 SFA



//------------------------------------------------                              //CSk 12-03-2012 SFA
//        CLILF - Count List Item File                                          //CSk 12-03-2012 SFA
//------------------------------------------------                              //CSk 12-03-2012 SFA

typedef struct { // CLILF Multi-Site Record                                     //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
//    ULONG ulPlannerId;                     //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
//    UINT  uiModuleId;                      //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
//    UBYTE ubRepeatCnt;                     //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
//    UBYTE ubCount;                         //CSk 11-09-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    ULONG ulModuleId;                        //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
    UBYTE ubModuleSeq;                       //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
    UBYTE ubRepeatCnt;                       //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
    UINT  uiCount;                           //CSk 11-09-2012 SFA               //CSk 12-03-2012 SFA
    UINT  uiFillQty;         // For future use                                  //CSk 12-03-2012 SFA
    BYTE  abFiller[4];       // For future use                                  //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
} CLILF_MS_REC;                                                                 //CSk 12-03-2012 SFA



typedef struct CLILF_Record_Layout {                                            //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abListId[3];         // KEY - ListId from CLOLF                        //CSk 12-03-2012 SFA
    BYTE abSeq[3];            // KEY - List Sequence                            //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abItemCode[4];       // 4-PD - includes check digit                    //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abPIITM[2];          // Support Office Count List Seq. No.             //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE bCountStatus;        // U - Uncounted                                  //CSk 12-03-2012 SFA
                              // C - Counted                                    //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abDateOfLastCnt[3];  // 3-PD YYMMDD. HHT will display date of last     //CSk 12-03-2012 SFA
                              // count if within 2 days (config in HHT)         //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    WORD wLastSalesCount;     // Sales at time of last sales floor count        //CSk 12-03-2012 SFA
    WORD wMainBSCount;        // Initially -1 to indicate not counted           //CSk 12-03-2012 SFA
    WORD wOSSRCount;          // Initially -1 to indicate not counted           //CSk 12-03-2012 SFA
    WORD wPendingBSCount;     // Initially -1 to indicate not counted           //CSk 12-03-2012 SFA
    WORD wPendingOSSRCount;   // Initially -1 to indicate not counted           //CSk 12-03-2012 SFA
    WORD wSalesFloorCount;    // Initially 0 to indicate no planner discreps    //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    BYTE abFiller[16];                                                          //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
    CLILF_MS_REC aMS_Details[32];                                               //CSk 12-03-2012 SFA
                                                                                //CSk 12-03-2012 SFA
} CLILF_REC;                                                                    //CSk 12-03-2012 SFA



//CSk 12-03-2012 SFA //  record layout
//CSk 12-03-2012 SFA typedef struct CLOLF_Record_Layout {
//CSk 12-03-2012 SFA     BYTE list_id[3];                   // ASCII
//CSk 12-03-2012 SFA     BYTE total_items[3];               // ASCII
//CSk 12-03-2012 SFA     BYTE items_shopfloor[3];           // ASCII
//CSk 12-03-2012 SFA     BYTE items_backshop[3];            // ASCII
//CSk 12-03-2012 SFA     BYTE list_type[1];                 // ASCII - [H]ead office, [R]ectification proc, [N] Negative
//CSk 12-03-2012 SFA     BYTE bus_unit[1];                  // ASCII
//CSk 12-03-2012 SFA     BYTE bus_unit_name[15];            // ASCII
//CSk 12-03-2012 SFA     BYTE list_status[1];               // ASCII - [A]ctive, [C]omplete, [ ]Initial
//CSk 12-03-2012 SFA     BYTE head_off_list_id[4];          // ASCII
//CSk 12-03-2012 SFA     BYTE cnt_date[8];                  // ASCII - Format YYYYMMDD
//CSk 12-03-2012 SFA     BYTE ossr_store;                   // marked for OSSR counting Y/N
//CSk 12-03-2012 SFA     BYTE items_ossr[3];                // ASCII
//CSk 12-03-2012 SFA     BYTE abUserID[3];                  // ASCII  SDH 11-01-2005 OSSR WAN
//CSk 12-03-2012 SFA } CLOLF_REC;
//CSk 12-03-2012 SFA
//CSk 12-03-2012 SFA

//CSk 12-03-2012 SFA //  record layout
//CSk 12-03-2012 SFA typedef struct CLILF_Record_Layout {
//CSk 12-03-2012 SFA     BYTE list_id[3];                                                        // ASCII
//CSk 12-03-2012 SFA     BYTE seq[3];                                                            // ASCII
//CSk 12-03-2012 SFA     BYTE boots_code[7];                                                     // ASCII - Format BBBBBBC (C=check digit)
//CSk 12-03-2012 SFA     BYTE item_code[13];                                                     // ASCII
//CSk 12-03-2012 SFA     BYTE sel_desc[45];                                                      // ASCII 3 x 15 charaters
//CSk 12-03-2012 SFA     BYTE active_deal_flag[1];                                               // ASCII [Y]es, [N]o
//CSk 12-03-2012 SFA     BYTE product_group[6];                                                  // ASCII (should be 6)
//CSk 12-03-2012 SFA     BYTE product_group_desc[12];                                            // ASCII
//CSk 12-03-2012 SFA     //The 3 counts                                                          // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE count_backshop[4];                                                 // ASCII
//CSk 12-03-2012 SFA     BYTE count_shopfloor[4];                                                // ASCII
//CSk 12-03-2012 SFA     BYTE abOSSRCount[4];                                                    // SDH 10-01-2005
//CSk 12-03-2012 SFA     //The 3 sales figures at the time of counts                             // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE sales[4];                                                          // ASCII
//CSk 12-03-2012 SFA     BYTE abAtStockSales[4];                                                 // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE abAtOSSRSales[4];                                                  // SDH 10-01-2005
//CSk 12-03-2012 SFA     //Some random field                                                     // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE ho_seq_no[2];                                                      // ASCII
//CSk 12-03-2012 SFA     //The three times of counts                                             // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE abTimeSalesCntPD[2];                                               // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE abTimeStockCntPD[2];                                               // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE abTimeOSSRCntPD[2];                                                // SDH 10-01-2005
//CSk 12-03-2012 SFA     BYTE abFiller[3];                                                       // SDH 10-01-2005
//CSk 12-03-2012 SFA } CLILF_REC;                                                                // SDH 10-01-2005

extern FILE_CTRL clolf;
extern FILE_CTRL clilf;
extern CLOLF_REC clolfrec;
extern CLILF_REC clilfrec;

void ClolfSet(void);
URC ClolfOpen(void);
URC ClolfClose(WORD type);
LONG ClolfRead(LONG lRecNum, LONG lLineNum);
LONG ClolfWrite(LONG lRecNum, LONG lLineNum);

void ClilfSet(void);
URC ClilfOpen(void);
URC ClilfClose(WORD type);
LONG ClilfRead(LONG lLineNum);
LONG ClilfReadLock(LONG lLineNum);
LONG ClilfWriteUnlock(LONG lLineNum);
LONG ClilfWrite(LONG lLineNum);                                                 //CSk 12-03-2012 SFA

#endif

