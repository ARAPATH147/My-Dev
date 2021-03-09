#include "transact.h"

#include <string.h>
#include "trxutil.h"
#include "trxfile.h"
#include "rfsfile.h"
#include "osfunc.h"
#include <time.h> //BMG 11-09-2007
#include "srfiles.h"
#include "rfscf.h"
#include "idf.h"
#include "trxbase.h"

////////////////////////////////////////////////////////////////////////////////
//
// Private Structures
//
////////////////////////////////////////////////////////////////////////////////

/*
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
} LRT_PGS;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGS_LTH sizeof(LRT_PGS)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
} LRT_PGX;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGX_LTH sizeof(LRT_PGX)                                                     // SDH 25-Aug-2006 Planners
*/

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abFamilyRecNum[4];                                                             // SDH 25-Aug-2006 Planners
    BYTE cCoreFlag;                                                                     // SDH 25-Aug-2006 Planners
    BYTE cLivePend;                                                                     // SDH 25-Aug-2006 Planners
} LRT_PGF;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGF_LTH sizeof(LRT_PGF)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abSeq[4];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abDesc[50];                                                                    // SDH 25-Aug-2006 Planners
    BYTE abStartPOGIRec[6];                                                             // SDH 25-Aug-2006 Planners
    BYTE abFamilyType[2];                                                               // SDH 25-Aug-2006 Planners
    BYTE abHierachy[2];                                                                 // SDH 25-Aug-2006 Planners
} LRT_PGG_FAM;                                                                          // SDH 25-Aug-2006 Planners
#define PGG_NUM_FAMS    4                                                               // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    LRT_PGG_FAM aFam[PGG_NUM_FAMS];                                                     // SDH 25-Aug-2006 Planners
    BYTE cMoreToCome;                   //Y or N                                        // SDH 25-Aug-2006 Planners
} LRT_PGG;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGG_LTH sizeof(LRT_PGG)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abPOGIRec[6];                                                                  // SDH 25-Aug-2006 Planners
    BYTE cLivePend;  //L or P                                                           // SDH 25-Aug-2006 Planners
} LRT_PGQ;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGQ_LTH sizeof(LRT_PGQ)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abPOGKey[6];                                                                   // SDH 25-Aug-2006 Planners
    BYTE abDesc[50];                                                                    // SDH 25-Aug-2006 Planners
    BYTE abActiveDate[8];                                                               // SDH 25-Aug-2006 Planners
    BYTE abDeactiveDate[8];                                                             // SDH 25-Aug-2006 Planners
    BYTE abModuleCount[3];                                                              // SDH 25-Aug-2006 Planners
} LRT_PGR_POG;                                                                          // SDH 25-Aug-2006 Planners
#define PGR_NUM_POGS    4                                                               // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    LRT_PGR_POG aPOG[PGR_NUM_POGS];                                                     // SDH 25-Aug-2006 Planners
    BYTE abNextPOGIRec[6];      //FFFFFF=Last one                                       // SDH 25-Aug-2006 Planners
} LRT_PGR;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGR_LTH sizeof(LRT_PGR)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abPOGKey[6];                                                                   // SDH 25-Aug-2006 Planners
    BYTE abModSeq[3];       //Base 0                                                    // SDH 25-Aug-2006 Planners
    BYTE abBootsCode[6];    //"FFFFFF" no filter                                        // PAB 16-Nov 2006
    //BYTE cChain;            //Base 0                                                    // SDH 25-Aug-2006 Planners
    //BYTE cLivePend;         //L or P                                                    // SDH 25-Aug-2006 Planners
} LRT_PGM;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGM_LTH sizeof(LRT_PGM)                                                     // SDH 25-Aug-2006 Planners

typedef struct {
    BYTE abModSeq[3];       //Base 0                                                    // SDH 25-Aug-2006 Planners
    BYTE abModuleDesc[50];                                                              // SDH 25-Aug-2006 Planners
    BYTE abShelfCount[2];                                                               // SDH 25-Aug-2006 Planners
    BYTE abFilter[1];                                                                   // PAB 16=Nov-2006
} LRT_PGM_MOD;                                                                          // SDH 25-Aug-2006 Planners
#define PGN_NUM_MODS    4                                                               // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    LRT_PGM_MOD aMod[PGN_NUM_MODS];                                                     // SDH 25-Aug-2006 Planners
    BYTE cMoreToCome;                                                                   // SDH 25-Aug-2006 Planners
} LRT_PGN;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGN_LTH sizeof(LRT_PGN)                                                     // SDH 25-Aug-2006 Planners

//typedef struct {                                                                        // SDH 25-Aug-2006 Planners
//    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
//    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
//    BYTE abPOGDataBlast[5][20]; //todo                                                  // SDH 25-Aug-2006 Planners
//} LRT_PPL;                                                                              // SDH 25-Aug-2006 Planners
//#define LRT_PPL_LTH sizeof(LRT_PPL)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abPOGKey[6];                                                                   // SDH 25-Aug-2006 Planners
    BYTE abModSeq[3];       //Base 0                                                    // SDH 25-Aug-2006 Planners
    BYTE abShelfNum[3];                                                                 // SDH 25-Aug-2006 Planners
    BYTE abNextChain[2];    //Base 0                                                    // SDH 25-Aug-2006 Planners
    BYTE abNextItem[2];     //Base 0                                                    // PAB 30 Nov-2006 Planners
} LRT_PSL;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PSL_LTH sizeof(LRT_PSL)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abItemCode[6];                                                                 // SDH 25-Aug-2006 Planners
    BYTE abDesc[24];                                                                    // SDH 25-Aug-2006 Planners
    BYTE abFacings[2];                                                                  // SDH 25-Aug-2006 Planners
} LRT_PSR_IOS;                                                                          // SDH 25-Aug-2006 Planners
#define PSR_NUM_ITEMS   15                                                              // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abNotchNum[3];                                                                 // PAB 14-Nov-2006 Planners
    BYTE abShelfDesc[50];                                                               // SDH 25-Aug-2006 Planners
    LRT_PSR_IOS aShelfItem[PSR_NUM_ITEMS];                                              // SDH 25-Aug-2006 Planners
    BYTE abNextShelf[3];
    BYTE abNextChain[2];                                                                // SDH 25-Aug-2006 Planners
    BYTE abNextItem[2];                                                                 // SDH 25-Aug-2006 Planners
} LRT_PSR;                                                                              // PAB 30-Nov-2006 Planners
#define LRT_PSR_LTH sizeof(LRT_PSR)                                                     // SDH 25-Aug-2006 Planners

/*
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abPOGKey[6];   //TODO                                                          // SDH 25-Aug-2006 Planners
    BYTE abDesc[50];                                                                    // SDH 25-Aug-2006 Planners
} LRT_PPR_POG;                                                                          // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    LRT_PPR_POG aPOG[20];   //TODO                                                      // SDH 25-Aug-2006 Planners
} LRT_PPR;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PPR_LTH sizeof(LRT_PPR)                                                     // SDH 25-Aug-2006 Planners
*/

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abItemCode[6];                                                                 // SDH 25-Aug-2006 Planners
    BYTE abStartChain[3];                                                               // SDH 25-Aug-2006 Planners
    BYTE abStartMod[3];                                                                 // SDH 25-Aug-2006 Planners
    BYTE cLivePend;         //L or P                                                    // SDH 25-Aug-2006 Planners
} LRT_PGL;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGL_LTH sizeof(LRT_PGL)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abOpID[3];                                                                     // SDH 25-Aug-2006 Planners
    BYTE abPOGKey[6];                                                                   // SDH 25-Aug-2006 Planners
    BYTE abMod[3];                                                                      // SDH 25-Aug-2006 Planners
    BYTE cType;                                                                         // SDH 25-Aug-2006 Planners
} LRT_PRP;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PRP_LTH sizeof(LRT_PRP)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abKey[6];                                                                      // SDH 25-Aug-2006 Planners
    BYTE abDesc[50];                                                                    // SDH 25-Aug-2006 Planners
    BYTE abModuleCount[3];
} Pog;                                                                                  // SDH 25-Aug-2006 Planners
#define PGI_NUM_POGS    4                                                               // SDH 25-Aug-2006 Planners
typedef struct {                                                                        // SDH 25-Aug-2006 Planners
    BYTE abCmd[3];                                                                      // SDH 25-Aug-2006 Planners
    Pog aPog[PGI_NUM_POGS];                                                             // SDH 25-Aug-2006 Planners
    BYTE abNextChain[3];  //FFF if no more                                              // SDH 25-Aug-2006 Planners
    BYTE abNextMod[3];    //FFF if no more                                              // SDH 25-Aug-2006 Planners
} LRT_PGI;                                                                              // SDH 25-Aug-2006 Planners
#define LRT_PGI_LTH sizeof(LRT_PGI)                                                     // SDH 25-Aug-2006 Planners

typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abFiller[12];                                                      //SDH 14-Sep-2006 Planners
} LRTLG_PGS;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abFiller[12];                                                      //SDH 14-Sep-2006 Planners
} LRTLG_PGX;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abSeq[4];                                                          //SDH 14-Sep-2006 Planners
    BYTE cCoreFlag;                                                         //SDH 14-Sep-2006 Planners
    BYTE cLivePend;                                                         //SDH 14-Sep-2006 Planners
    BYTE abFiller[6];                                                       //SDH 14-Sep-2006 Planners
} LRTLG_PGF;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abPOGIRec[6];                                                      //SDH 14-Sep-2006 Planners
    BYTE cLivePend;                                                         //SDH 14-Sep-2006 Planners
    BYTE abFiller[5];                                                       //SDH 14-Sep-2006 Planners
} LRTLG_PGQ;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abPOGKey[6];                                                       //SDH 14-Sep-2006 Planners
    BYTE abModSeq[3];                                                       //SDH 14-Sep-2006 Planners
    BYTE abFiller[3];                                                       //SDH 14-Sep-2006 Planners
} LRTLG_PGM;                                                                //SDH 14-Sep-2006 Planners
//typedef struct {                                                            //SDH 14-Sep-2006 Planners
//    BYTE abFiller[12];                                                      //SDH 14-Sep-2006 Planners
//} LRTLG_PPL;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abPOGKey[6];                                                       //SDH 25-Aug-2006 Planners
    BYTE abModSeq[3];       //Base 0                                        //SDH 25-Aug-2006 Planners
    BYTE abShelfNum[2];                                                     //SDH 25-Aug-2006 Planners
    BYTE abNextChain[1];    //Base 0                                        //SDH 25-Aug-2006 Planners
} LRTLG_PSL;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abItemCode[6];                                                     //SDH 14-Sep-2006 Planners
    BYTE abStartChain[3];                                                   //SDH 14-Sep-2006 Planners
    BYTE abStartPlan[3];                                                    //SDH 14-Sep-2006 Planners
} LRTLG_PGL;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 14-Sep-2006 Planners
    BYTE abPOGKey[6];                                                       //SDH 14-Sep-2006 Planners
    BYTE abMod[3];                                                          //SDH 14-Sep-2006 Planners
} LRTLG_PRP;                                                                //SDH 14-Sep-2006 Planners
typedef struct {                                                            //SDH 20-May-2009 Model Day
    BYTE abItemCode[3];                                                     //SDH 20-May-2009 Model Day
    BYTE abStartChain[2];                                                   //SDH 20-May-2009 Model Day
    BYTE abStartPlan[2];                                                    //SDH 20-May-2009 Model Day
    BYTE abStartItem[2];                                                    //SDH 20-May-2009 Model Day
    BYTE bLivePend;                                                         //SDH 20-May-2009 Model Day
    BYTE abFiller[2];                                                       //SDH 20-May-2009 Model Day
} LRTLG_PGA;                                                                //SDH 20-May-2009 Model Day


////////////////////////////////////////////////////////////////////////////////
//
// Module level variables
//
////////////////////////////////////////////////////////////////////////////////

URC usrrc;
LONG rc2;


// ------------------------------------------------------------------------------------
//
// PGS - Planner Session Start
//
//
//
// ------------------------------------------------------------------------------------

void PogSessionStart(char *inbound) {                                       //SDH 23-Aug-2006 Planners

    //Input and output views                                                //SDH 23-Aug-2006 Planners
    //LRT_PGS* pPGS = (LRT_PGS*)inbound;                                    //SDH 23-Aug-2006 Planners
    UNUSED(inbound);                                                        //SDH 23-Aug-2006 Planners
    LRTLG_PGS* pLGPGS = (LRTLG_PGS*)dtls;                                   //SDH 23-Aug-2006 Planners

    //Initial checks                                                        //SDH 23-Aug-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 23-Aug-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 23-Aug-2006 Planners
    UpdateActiveTime();                                                     //SDH 23-Aug-2006 Planners

    //Set up current date and time                                          //SDH 23-Aug-2006 Planners
    B_DATE nowDate;                                                         //SDH 23-Aug-2006 Planners
    B_TIME nowTime;                                                         //SDH 23-Aug-2006 Planners
    GetSystemDate(&nowTime, &nowDate);                                      //SDH 23-Aug-2006 Planners

    //Check store config                                                    //SDH 23-Aug-2006 Planners
    //SDH 20-May-2009 Model Day.  if (rfscfrec1and2.cPlannersActive != 'Y') {                             //SDH 23-Aug-2006 Planners
    //SDH 20-May-2009 Model Day.      prep_nak("ERRORPlanners are not currently active in this store.");  //SDH 23-Aug-2006 Planners
    //SDH 20-May-2009 Model Day.      return;                                                             //SDH 23-Aug-2006 Planners
    //SDH 20-May-2009 Model Day.  }                                                                       //SDH 23-Aug-2006 Planners

    //Open, read, close POGOK                                               //SDH 23-Aug-2006 Planners
    usrrc = PogokOpen();                                                   //SDH 23-Aug-2006 Planners
    if (usrrc < RC_OK) {                                                    //SDH 23-Aug-2006 Planners
        prep_nak("ERRORUnable to open POGOK file. Please phone help desk.");    //SDH 23-Aug-2006 Planners
        return;                                                             //SDH 23-Aug-2006 Planners
    }                                                                       //SDH 23-Aug-2006 Planners
    rc2 = PogokRead(0, __LINE__);                                           //SDH 23-Aug-2006 Planners
    PogokClose(CL_SESSION);                                                 //SDH 23-Aug-2006 Planners

    //BMG 28-Apr-2009
    //Swapped the following two bits around so it reports still running before
    //critical errors due to the way the suites run in store. Apparently 
    //there can be an X written when a program starts.
    
    //If any progams are still active then don't allow                      //SDH 23-Aug-2006 Planners
    //the session to start                                                  //SDH 23-Aug-2006 Planners
    if (pogokrec.cSRP04 == 'S' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP05 == 'S' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP06 == 'S' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP07 == 'S' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP10 == 'S') {                                           //PAB 13-Nov-2006 Planners
        // pogokrec.cSRP19 == 'S') {                                        //PAB 13-Nov-2006 Planners
        prep_nak("ERRORPlanner Update Suite is "                            //SDH 23-Aug-2006 Planners
                 "still active.  Please try later.");                       //SDH 23-Aug-2006 Planners
        return;                                                             //SDH 23-Aug-2006 Planners
    }                                                                       //SDH 23-Aug-2006 Planners

    //Report any critical Planner Suite errors                              //SDH 23-Aug-2006 Planners
    if (pogokrec.cSRP04 == 'X' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP05 == 'X' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP06 == 'X' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP07 == 'X' ||                                           //SDH 23-Aug-2006 Planners
        pogokrec.cSRP10 == 'X') {                                           //PAB 13-Nov-2006 Planners
        // pogokrec.cSRP19 == 'X') {                                        //PAB 13-Nov-2006 Planners
        prep_nak("ERRORPlanner Update Suite "                               //SDH 23-Aug-2006 Planners
                 "has failed.  Please phone help desk.");                   //SDH 23-Aug-2006 Planners
        return;                                                             //SDH 23-Aug-2006 Planners
    }                                                                       //SDH 23-Aug-2006 Planners

    usrrc = prepare_workfile( hh_unit, SYS_LAB );                           //PAB 20-Dec-2006
    if (usrrc<RC_IGNORE_ERR) {                                              //PAB 20-Dec-2006
        if (usrrc == RC_DATA_ERR) {                                         //PAB 20-Dec-2006
            prep_nak("ERRORUnable to create workfile. "                     //PAB 20-Dec-2006
                     "Check appl event logs" );                             //PAB 20-Dec-2006
            return;                                                         //PAB 20-Dec-2006
           }                                                                //PAB 20-Dec-2006
    }                                                                       //PAB 20-Dec-2006


    //If the programs haven't run today then show warning                   //SDH 23-Aug-2006 Planners
    sprintf(msg, "%4.4d%2.2d%2.2d", nowDate.wYear,                          //SDH 23-Aug-2006 Planners
            nowDate.wMonth, nowDate.wDay);                                  //SDH 23-Aug-2006 Planners
    pack(sbuf, 4, msg, 8, 0);                                               //SDH 23-Aug-2006 Planners
    if (memcmp(pogokrec.abSRP04Date, sbuf, 4) != 0 ||                       //SDH 23-Aug-2006 Planners
        memcmp(pogokrec.abSRP05Date, sbuf, 4) != 0 ||                       //SDH 23-Aug-2006 Planners
        memcmp(pogokrec.abSRP06Date, sbuf, 4) != 0 ||                       //SDH 23-Aug-2006 Planners
        memcmp(pogokrec.abSRP07Date, sbuf, 4) != 0 ||                       //SDH 23-Aug-2006 Planners
        memcmp(pogokrec.abSRP10Date, sbuf, 4) != 0) {                       //PAB 14-Nov-2006 Planners
        // memcmp(pogokrec.abSRP19Date, sbuf, 4) != 0) {                    //PAB 14-Nov-2006 Planners
        prep_ack("Planner data may be out of "                              //SDH 23-Aug-2006 Planners
                 "date. Please use with caution.");                         //SDH 23-Aug-2006 Planners
    } else {                                                                //SDH 23-Aug-2006 Planners
        prep_ack("");                                                       //SDH 23-Aug-2006 Planners
    }                                                                       //SDH 23-Aug-2006 Planners

    // Audit                                                                //SDH 23-Aug-2006 Planners
    memset(pLGPGS->abFiller, 0x00, sizeof(pLGPGS->abFiller));               //SDH 23-Aug-2006 Planners
    lrt_log(LOG_PGS, hh_unit, dtls);                                        //SDH 23-Aug-2006 Planners

}                                                                           //SDH 23-Aug-2006 Planners


// ------------------------------------------------------------------------ //SDH 23-Aug-2006 Planners
//                                                                          //SDH 23-Aug-2006 Planners
// PGX - Planner Session Exit                                               //SDH 23-Aug-2006 Planners
//                                                                          //SDH 23-Aug-2006 Planners
//                                                                          //SDH 23-Aug-2006 Planners
//                                                                          //SDH 23-Aug-2006 Planners
// ------------------------------------------------------------------------ //SDH 23-Aug-2006 Planners

void PogSessionExit(char *inbound) {                                        //SDH 23-Aug-2006 Planners

    //Input and output views                                                //SDH 23-Aug-2006 Planners
    UNUSED(inbound);                                                        //SDH 23-Aug-2006 Planners
    LRTLG_PGX* pLGPGX = (LRTLG_PGX*)dtls;                                   //SDH 23-Aug-2006 Planners

    //Initial checks                                                        //SDH 23-Aug-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 23-Aug-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 23-Aug-2006 Planners
    UpdateActiveTime();                                                     //SDH 23-Aug-2006 Planners

    //Process both just in case                                             //SDH 14-Sep-2006 Planners
    process_workfile(hh_unit, SYS_LAB);                                     //SDH 14-Sep-2006 Planners
    process_workfile(hh_unit, SYS_GAP);                                     //SDH 14-Sep-2006 Planners

    //Send ACK                                                              //SDH 14-Sep-2006 Planners
    prep_ack("");                                                           //SDH 14-Sep-2006 Planners

    // Audit                                                                //SDH 23-Aug-2006 Planners
    MEMSET(pLGPGX->abFiller, 0x00);                                         //SDH 23-Aug-2006 Planners
    lrt_log(LOG_PGX, hh_unit, dtls);                                        //SDH 23-Aug-2006 Planners

}                                                                           //SDH 23-Aug-2006 Planners


// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// PGF - Load Planner Families                                              //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners

void PogLoadFamilies(char *inbound) {                                       //SDH 14-Sep-2006 Planners

    //Input and output views                                                //SDH 14-Sep-2006 Planners
    LRT_PGF* pPGF = (LRT_PGF*)inbound;                                      //SDH 14-Sep-2006 Planners
    LRTLG_PGF* pLGPGF = (LRTLG_PGF*)dtls;                                   //SDH 14-Sep-2006 Planners
    LRT_PGG* pPGG = (LRT_PGG*)out;                                          //SDH 14-Sep-2006 Planners

    //Initial checks                                                        //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                     //SDH 14-Sep-2006 Planners

    //Get record number                                                     //SDH 14-Sep-2006 Planners
    WORD wFamilyRec = satoi(pPGF->abFamilyRecNum,                           //SDH 14-Sep-2006 Planners
                            sizeof(pPGF->abFamilyRecNum));                  //SDH 14-Sep-2006 Planners

    //Build initial PGG                                                     //SDH 14-Sep-2006 Planners
    memcpy(pPGG->abCmd, "PGG", sizeof(pPGG->abCmd));                        //SDH 14-Sep-2006 Planners
    pPGG->cMoreToCome = 'Y';                                                //SDH 14-Sep-2006 Planners
    out_lth = LRT_PGG_LTH;                                                  //SDH 14-Sep-2006 Planners

    //For each family to find                                               //SDH 14-Sep-2006 Planners
    //We read one more record than we need just to populate the MoreToCome  //SDH 14-Sep-2006 Planners
    //field                                                                 //SDH 14-Sep-2006 Planners
    memset(pPGG->aFam, 'F', sizeof(pPGG->aFam));                            //SDH 14-Sep-2006 Planners
    for (WORD wFamily = 0; wFamily < PGG_NUM_FAMS + 1; wFamily++) {         //SDH 14-Sep-2006 Planners

        //Direct read the SRPOGIF family file from the rec req              //SDH 14-Sep-2006 Planners
        //until we find a matching record                                   //SDH 14-Sep-2006 Planners
        BOOLEAN fLoop = TRUE;                                               //SDH 14-Sep-2006 Planners
        while (fLoop) {                                                     //SDH 14-Sep-2006 Planners

            //Attempt a read                                                //SDH 14-Sep-2006 Planners
            rc2 = SrpogifRead(wFamilyRec, __LINE__);                        //SDH 14-Sep-2006 Planners
            fLoop = FALSE;                                                  //SDH 14-Sep-2006 Planners
            if (rc2 <= 0) break;                                            //SDH 14-Sep-2006 Planners

            //Work out whether core or not                                  //SDH 14-Sep-2006 Planners
            BYTE cCore = 'N';                                               //SDH 14-Sep-2006 Planners
            if (srpogifrec.ubFamilyType == 1) cCore = 'C';                  //SDH 14-Sep-2006 Planners
            //Ignore key levels 1 and 2                                     //SDH 14-Sep-2006 Planners
            if (srpogifrec.ubKeyHierachy < 3) fLoop = TRUE;                 //SDH 14-Sep-2006 Planners
            //Ignore non-matching core flag                                 //SDH 14-Sep-2006 Planners
            if (cCore != pPGF->cCoreFlag) fLoop = TRUE;                     //SDH 14-Sep-2006 Planners

            if (pPGF->cLivePend == 'L') {                                   //SDH 14-Sep-2006 Planners
                if (srpogifrec.ulPOGLiveIndexPtr == 0xffffffff)             //SDH 14-Sep-2006 Planners
                    fLoop = TRUE;                                           //SDH 14-Sep-2006 Planners
            } else {                                                        //SDH 14-Sep-2006 Planners
                if (srpogifrec.ulPOGPendingIndexPtr == 0xffffffff)          //SDH 14-Sep-2006 Planners
                    fLoop = TRUE;                                           //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners

            //Increment record num                                          //SDH 14-Sep-2006 Planners
            wFamilyRec++;                                                   //SDH 14-Sep-2006 Planners

        }                                                                   //SDH 14-Sep-2006 Planners

        //Handle errors                                                     //SDH 14-Sep-2006 Planners
        if (rc2 <= 0) {                                                     //SDH 14-Sep-2006 Planners
            if ((rc2&0xFFFF) != 0x4003) {                                   //SDH 14-Sep-2006 Planners
                prep_nak("ERRORUnable to read SRPOGIF. "                    //SDH 14-Sep-2006 Planners
                         "Please phone help desk.");                        //SDH 14-Sep-2006 Planners
                return;                                                     //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners
            pPGG->cMoreToCome = 'N';                                        //SDH 14-Sep-2006 Planners
            break;                                                          //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners

        //Add family to PGG                                                 //SDH 14-Sep-2006 Planners
        if (wFamily < PGG_NUM_FAMS) {
            WORD_TO_ARRAY(pPGG->aFam[wFamily].abSeq, wFamilyRec - 1);       //SDH 14-Sep-2006 Planners
            MEMCPY(pPGG->aFam[wFamily].abDesc, srpogifrec.abDesc);          //SDH 14-Sep-2006 Planners
            if (pPGF->cLivePend == 'L') {                                   //SDH 14-Sep-2006 Planners
                sprintf(sbuf, "%06ld",                                      //SDH 14-Sep-2006 Planners
                        srpogifrec.ulPOGLiveIndexPtr);                      //SDH 14-Sep-2006 Planners
            } else {                                                        //SDH 14-Sep-2006 Planners
                sprintf(sbuf, "%06ld",                                      //SDH 14-Sep-2006 Planners
                        srpogifrec.ulPOGPendingIndexPtr);                   //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners
            if (sbuf[0] != '-') {                                           //SDH 14-Sep-2006 Planners
                MEMCPY(pPGG->aFam[wFamily].abStartPOGIRec, sbuf);           //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners
            WORD_TO_ARRAY(pPGG->aFam[wFamily].abFamilyType,                 //SDH 14-Sep-2006 Planners
                          srpogifrec.ubFamilyType);                         //SDH 14-Sep-2006 Planners
            WORD_TO_ARRAY(pPGG->aFam[wFamily].abHierachy,                   //SDH 14-Sep-2006 Planners
                          srpogifrec.ubKeyHierachy);                        //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners

    }                                                                       //SDH 14-Sep-2006 Planners

    // Audit                                                                //SDH 23-Aug-2006 Planners
    memcpy(pLGPGF->abSeq, pPGF->abFamilyRecNum, sizeof(pLGPGF->abSeq));     //SDH 23-Aug-2006 Planners
    pLGPGF->cCoreFlag = pPGF->cCoreFlag;                                    //SDH 23-Aug-2006 Planners
    pLGPGF->cLivePend = pPGF->cLivePend;                                    //SDH 23-Aug-2006 Planners
    memset(pLGPGF->abFiller, 0x00, sizeof(pLGPGF->abFiller));               //SDH 23-Aug-2006 Planners
    lrt_log(LOG_PGF, hh_unit, dtls);                                        //SDH 23-Aug-2006 Planners

}                                                                           //SDH 23-Aug-2006 Planners


// ------------------------------------------------------------------------------------
//
// PGQ - Planner List Query
//
//
//
// ------------------------------------------------------------------------------------

void PogListQuery(char *inbound) {                                          //SDH 14-Sep-2006 Planners

    //Input and output views                                                //SDH 14-Sep-2006 Planners
    LRT_PGQ* pPGQ = (LRT_PGQ*)inbound;                                      //SDH 14-Sep-2006 Planners
    LRTLG_PGQ* pLGPGQ = (LRTLG_PGQ*)dtls;                                   //SDH 14-Sep-2006 Planners
    LRT_PGR* pPGR = (LRT_PGR*)out;                                          //SDH 14-Sep-2006 Planners

    //Initial checks                                                        //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                     //SDH 14-Sep-2006 Planners

    //Initialise array                                                      //SDH 15-Sep-2006 Planners
    MEMSET(pPGR->aPOG, 'F');                                                //SDH 15-Sep-2006 Planners

    //Get record number                                                     //SDH 15-Sep-2006 Planners
    LONG lPOGIRec = satol(pPGQ->abPOGIRec, sizeof(pPGQ->abPOGIRec));        //SDH 15-Sep-2006 Planners

    //Loop to populate multiple POGs                                        //SDH 15-Sep-2006 Planners
    for (WORD wCount = 0; wCount < PGR_NUM_POGS; wCount++) {                //SDH 15-Sep-2006 Planners

        //Attempt read                                                      //SDH 15-Sep-2006 Planners
        if (pPGQ->cLivePend == 'L') {                                       //SDH 15-Sep-2006 Planners
            rc2 = SrpogilRead(lPOGIRec - 1, __LINE__);                      //SDH 15-Sep-2006 Planners
        } else {                                                            //SDH 15-Sep-2006 Planners
            rc2 = SrpogipRead(lPOGIRec - 1, __LINE__);                      //SDH 15-Sep-2006 Planners
            memcpy(&srpogilrec, &srpogiprec, sizeof(srpogilrec));           //SDH 15-Sep-2006 Planners
        }                                                                   //SDH 15-Sep-2006 Planners

        //Handle errors                                                     //SDH 15-Sep-2006 Planners
        if (rc2 <= 0) {                                                     //SDH 15-Sep-2006 Planners
            if ((rc2&0xffff) == 0x4003) {                                   //SDH 15-Sep-2006 Planners
                prep_nak("SRPOGIL/P indexes are corrupt.");                 //SDH 15-Sep-2006 Planners
            } else {                                                        //SDH 15-Sep-2006 Planners
                prep_nak("ERRORUnable to read SRPOGIL/P. "                  //SDH 15-Sep-2006 Planners
                         "Please sign off and sign back in.");              //SDH 15-Sep-2006 Planners
            }                                                               //SDH 15-Sep-2006 Planners
            return;                                                         //SDH 15-Sep-2006 Planners
        }                                                                   //SDH 15-Sep-2006 Planners

        //Populate array                                                    //SDH 15-Sep-2006 Planners
        LONG_TO_ARRAY(pPGR->aPOG[wCount].abPOGKey, srpogilrec.ulPOGKey);    //SDH 15-Sep-2006 Planners
        MEMSET(pPGR->aPOG[wCount].abDesc, ' ');                             //SDH 15-Sep-2006 Planners
        memcpy(pPGR->aPOG[wCount].abDesc, srpogilrec.abDesc,                //SDH 15-Sep-2006 Planners
               _min(sizeof(srpogilrec.abDesc),                              //SDH 15-Sep-2006 Planners
                    sizeof(pPGR->aPOG[wCount].abDesc)));                    //SDH 15-Sep-2006 Planners
        unpack(pPGR->aPOG[wCount].abActiveDate,                             //SDH 15-Sep-2006 Planners
               sizeof(pPGR->aPOG[wCount].abActiveDate),                     //SDH 15-Sep-2006 Planners
               srpogilrec.abActiveDate,                                     //SDH 15-Sep-2006 Planners
               sizeof(srpogilrec.abActiveDate), 0);                         //SDH 15-Sep-2006 Planners
        unpack(pPGR->aPOG[wCount].abDeactiveDate,                           //SDH 15-Sep-2006 Planners
               sizeof(pPGR->aPOG[wCount].abDeactiveDate),                   //SDH 15-Sep-2006 Planners
               srpogilrec.abDeactiveDate,                                   //SDH 15-Sep-2006 Planners
               sizeof(srpogilrec.abDeactiveDate), 0);                       //SDH 15-Sep-2006 Planners
        WORD_TO_ARRAY(pPGR->aPOG[wCount].abModuleCount,                     //SDH 15-Sep-2006 Planners
                      srpogilrec.ubModuleCount);                            //SDH 15-Sep-2006 Planners

        //Set up next rec num                                               //SDH 15-Sep-2006 Planners
        lPOGIRec = srpogilrec.ulNextPOGIRec;                                //SDH 15-Sep-2006 Planners
        if (lPOGIRec < 0) break;                                            //SDH 15-Sep-2006 Planners

    }                                                                       //SDH 15-Sep-2006 Planners

    //Build PGR                                                             //SDH 15-Sep-2006 Planners
    memcpy(pPGR->abCmd, "PGR", sizeof(pPGR->abCmd));                        //SDH 15-Sep-2006 Planners
    out_lth = LRT_PGR_LTH;                                                  //SDH 14-Sep-2006 Planners
    if (lPOGIRec < 0) {                                                     //SDH 15-Sep-2006 Planners
        MEMSET(pPGR->abNextPOGIRec, 'F');                                   //SDH 15-Sep-2006 Planners
    } else {                                                                //SDH 15-Sep-2006 Planners
        LONG_TO_ARRAY(pPGR->abNextPOGIRec, lPOGIRec);                       //SDH 15-Sep-2006 Planners
    }                                                                       //SDH 15-Sep-2006 Planners

    // Audit                                                                //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGQ->abPOGIRec, pPGQ->abPOGIRec);                             //SDH 15-Sep-2006 Planners
    pLGPGQ->cLivePend = pPGQ->cLivePend;                                    //SDH 15-Sep-2006 Planners
    MEMSET(pLGPGQ->abFiller, 0x00);                                         //SDH 15-Sep-2006 Planners
    lrt_log(LOG_PGQ, hh_unit, dtls);                                        //SDH 15-Sep-2006 Planners

}                                                                           //SDH 23-Aug-2006 Planners


// ---------------------------------------------------------------------------
//
// PGM - Planner List Modules
//
//
//
// ---------------------------------------------------------------------------

void PogListModules(char *inbound) {                                        //SDH 23-Aug-2006 Planners

    WORD bFilter = FALSE;                                                   //PAB 16-Nov-2006 Planners
    BYTE wpBootsCode[3];                                                    //PAB 16-Nov-2006

    //Input and output views                                                //SDH 14-Sep-2006 Planners
    LRT_PGM* pPGM = (LRT_PGM*)inbound;                                      //SDH 14-Sep-2006 Planners
    LRTLG_PGM* pLGPGM = (LRTLG_PGM*)dtls;                                   //SDH 14-Sep-2006 Planners
    LRT_PGN* pPGN = (LRT_PGN*)out;                                          //SDH 14-Sep-2006 Planners

    //Initial checks                                                        //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                     //SDH 14-Sep-2006 Planners

    WORD wMod = satoi(pPGM->abModSeq, sizeof(pPGM->abModSeq));              //SDH 14-Sep-2006 Planners
    ULONG ulKey = satol(pPGM->abPOGKey, sizeof(pPGM->abPOGKey));            //SDH 14-Sep-2006 Planners

    memset(sbuf,'F',6);                                                     //PAB 16-Nov 2006
    if (memcmp(pPGM->abBootsCode,sbuf, 6) != 0) {                           //PAB 16=Nov 2006
        bFilter = TRUE;                                                     //PAB 16-Nov 2006
    }                                                                       //PAB 16-Nov 2006

    if (bFilter == TRUE) {                                                  //PAB 16-Nov-2006
        pack(wpBootsCode, 3, pPGM->abBootsCode, 6, 0);                      //PAB 16-Nov-2006
    }                                                                       //PAB 16-Nov-2006

    MEMCPY(pPGN->abCmd, "PGN");                                             //SDH 14-Sep-2006 Planners
    out_lth = LRT_PGN_LTH;                                                  //SDH 14-Sep-2006 Planners
    MEMSET(pPGN->aMod, 'F');                                                //SDH 14-Sep-2006 Planners
    pPGN->cMoreToCome = 'Y';                                                //SDH 14-Sep-2006 Planners

    WORD wModNum = 0;                                                       //SDH 14-Sep-2006 Planners
    WORD wOriginalMod = wMod;                                               //PAB 23-Nov-2006

    while (wModNum <= PGN_NUM_MODS) {                                       //SDH 14-Sep-2006 Planners

        //Read the SRMOD                                                    //SDH 14-Sep-2006 Planners
        srmodrec.ulKey = ulKey;                                             //SDH 14-Sep-2006 Planners
        wOriginalMod = wMod;                                                //PAB 23-Nov-2006
        srmodrec.ubModSeq = wMod++;                                         //SDH 14-Sep-2006 Planners
        srmodrec.ubRecChain = 0;                                            //SDH 14-Sep-2006 Planners
        rc2 = SrmodRead(__LINE__);                                          //SDH 14-Sep-2006 Planners

        //Check for errors                                                  //SDH 14-Sep-2006 Planners
        if (rc2 <= 0) {                                                     //SDH 14-Sep-2006 Planners
            if ((rc2&0xFFFF) != 0x06c8) {                                   //SDH 14-Sep-2006 Planners
                prep_nak("Unable to read SRMOD. Please phone help desk.");  //SDH 14-Sep-2006 Planners
                return;                                                     //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners
            //Module 0 may or may not be there (normally isn't)             //SDH 14-Sep-2006 Planners
            if (srmodrec.ubModSeq == 0) continue;                           //SDH 14-Sep-2006 Planners
            pPGN->cMoreToCome = 'N';                                        //SDH 14-Sep-2006 Planners
            break;                                                          //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners

        //Build the response                                                                  //SDH 14-Sep-2006 Planners
        if (wModNum < PGN_NUM_MODS) {                                                         //SDH 14-Sep-2006 Planners
            MEMSET(pPGN->aMod[wModNum].abFilter, 'F');                                        //PAB 16-Nov-2006
            WORD_TO_ARRAY(pPGN->aMod[wModNum].abModSeq, srmodrec.ubModSeq);                   //SDH 15-Sep-2006 Planners
            MEMSET(pPGN->aMod[wModNum].abModuleDesc, ' ');                                    //SDH 14-Sep-2006 Planners
            memcpy(pPGN->aMod[wModNum].abModuleDesc, srmodrec.abModDesc,30);                  //PAB 16-Nov-2006
            WORD_TO_ARRAY(pPGN->aMod[wModNum].abShelfCount,                                   //PAB 23-Nov-2006 Planners
                          srmodrec.uwShelfCount);                                             //PAB 23-Nov-2006 Planners
            
            // if filtering is required then                                                  //PAB 23-Nov02006
            if (bFilter == TRUE) {

                WORD rc3 = 1;                                                                 //PAB 23-Nov-2006
                srmodrec.ubRecChain = 0;                                                      //PAB 23-Nov-2006
                srmodrec.ubModSeq = wOriginalMod;                                             //PAB 23-Nov-2006
                MEMSET(pPGN->aMod[wModNum].abFilter, 'N');                                    //PAB 23-Nov-2006

                while (srmodrec.ubRecChain < 10 && rc3 > 0) {                                 //PAB 23-Nov-2006

                    // for each module record chain                                           //PAB 23-Nov-2006
                    for (WORD wItem = 0; wItem <= SRMOD_NUM_ITEMS; wItem++) {                 //PAB 16-Nov-2006 Planners

                        memset(sbuf, 0x00, sizeof(srmodrec.aShelfItem[wItem].abItemCode));    //PAB 16-Nov-2006 Planners
                        //Check for zero item code and assume that it's the last item         //PAB 16-Nov-2006 Planners
                        if (memcmp(srmodrec.aShelfItem[wItem].abItemCode,                     //PAB 16-Nov-2006 Planners
                                   sbuf, sizeof(srmodrec.aShelfItem[wItem].                   //PAB 16-Nov-2006 Planners
                                                abItemCode)) == 0) {                          //PAB 16-Nov-2006 Planners
                            wItem = -1;                                                       //PAB 16-Nov-2006 Planners
                            break;
                        }                                                                     //PAB 16-Nov-2006 Planners
                        if (memcmp(srmodrec.aShelfItem[wItem].abItemCode,                     //PAB 16-Nov-2006 Planners
                                   wpBootsCode, sizeof(srmodrec.aShelfItem[wItem].            //PAB 16-Nov-2006 Planners
                                                       abItemCode)) == 0) {                   //PAB 16-Nov-2006 Planners
                            MEMSET(pPGN->aMod[wModNum].abFilter, 'Y');                        //PAB 16-Nov-2006
                            break;                                                            //PAB 16-Nov-2006
                        }                                                                     //PAB 16-Nov-2006

                    }
                    // read next mod rec chain                                                //PAB 23-Nov 2006
                    srmodrec.ubRecChain++;                                                    //PAB 23-Nov-2006 Planners
                    rc3 = SrmodRead(__LINE__);                                                //PAB 23-Nov-2006
                }
            }
        }                                                                                     //SDH 14-Sep-2006 Planners
        wModNum++;                                                                            //SDH 14-Sep-2006 Planners

    }                                                                                         //SDH 14-Sep-2006 Planners

    // Audit                                                                                  //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGM->abPOGKey, pPGM->abPOGKey);                                                 //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGM->abModSeq, pPGM->abModSeq);                                                 //SDH 15-Sep-2006 Planners
    MEMSET(pLGPGM->abFiller, 0x00);                                                           //SDH 15-Sep-2006 Planners
    lrt_log(LOG_PGM, hh_unit, dtls);                                                          //SDH 15-Sep-2006 Planners

}                                                                                             //SDH 23-Aug-2006 Planners


// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// PSL - Planner Shelf Load Request                                         //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// Returns a shelf description and all the items for a specified shelf.     //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners

void PogShelfLoadRequest(char *inbound) {                                               //SDH 23-Aug-2006 Planners

    //Input and output views                                                            //SDH 14-Sep-2006 Planners
    LRT_PSL* pPSL = (LRT_PSL*)inbound;                                                  //SDH 14-Sep-2006 Planners
    LRTLG_PSL* pLGPSL = (LRTLG_PSL*)dtls;                                               //SDH 14-Sep-2006 Planners
    LRT_PSR* pPSR = (LRT_PSR*)out;                                                      //SDH 14-Sep-2006 Planners

    //Initial checks                                                                    //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                                        //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                                    //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                                 //SDH 14-Sep-2006 Planners

    //Set up some useful vars                                                           //SDH 14-Sep-2006 Planners
    WORD wShelfNum = satoi(pPSL->abShelfNum, sizeof(pPSL->abShelfNum));                 //SDH 14-Sep-2006 Planners
    WORD wNextChain = satoi(pPSL->abNextChain, sizeof(pPSL->abNextChain));              //SDH 14-Sep-2006 Planners
    WORD wNextItem = satoi(pPSL->abNextItem, sizeof(pPSL->abNextItem));                 //SDH 14-Sep-2006 Planners
    WORD wNextShelf = 127;                                                              //PAB 22-Nov-2006
    WORD wDone = 0;                                                                     //PAB 22-Nov-2006
    WORD wItem = 0;                                                                     //PAB 23-Nov-2006

    //Read SRSXF to obtains the notch num and the shelf desc                            //SDH 14-Sep-2006 Planners
    MEMSET(srsxfrec.abShelfDesc, ' ');                                                  //SDH 14-Sep-2006 Planners
    memcpy(srsxfrec.abShelfDesc, "Shelf description not on file", 29);                  //SDH 14-Sep-2006 Planners
    srsxfrec.ulPOGDB = satol(pPSL->abPOGKey, sizeof(pPSL->abPOGKey));                   //SDH 14-Sep-2006 Planners
    srsxfrec.ubModSeq = satoi(pPSL->abModSeq, sizeof(pPSL->abModSeq));                  //SDH 14-Sep-2006 Planners
    srsxfrec.ubShelfNum = satoi(pPSL->abShelfNum, sizeof(pPSL->abShelfNum));            //SDH 14-Sep-2006 Planners
    rc2 = SrsxfRead(__LINE__);                                                          //SDH 14-Sep-2006 Planners

    //Build the first part of the response                                              //SDH 14-Sep-2006 Planners
    MEMCPY(pPSR->abCmd, "PSR");                                                         //SDH 14-Sep-2006 Planners
    out_lth = LRT_PSR_LTH;                                                              //SDH 14-Sep-2006 Planners
    WORD_TO_ARRAY(pPSR->abNotchNum, srsxfrec.ubNotchNum);                               //SDH 14-Sep-2006 Planners
    MEMSET(pPSR->abShelfDesc, ' ');                                                     //SDH 14-Sep-2006 Planners
    memcpy(pPSR->abShelfDesc, srsxfrec.abShelfDesc,                                     //SDH 14-Sep-2006 Planners
           _min(sizeof(pPSR->abShelfDesc),                                              //SDH 14-Sep-2006 Planners
                sizeof(srsxfrec.abShelfDesc)));                                         //SDH 14-Sep-2006 Planners
    MEMSET(pPSR->aShelfItem, 'F');                                                      //SDH 14-Sep-2006 Planners
    MEMSET(pPSR->abNextChain, 'F');                                                     //SDH 14-Sep-2006 Planners
    MEMSET(pPSR->abNextItem, 'F');                                                      //SDH 14-Sep-2006 Planners
    MEMSET(pPSR->abNextShelf, 'F');                                                     //PAB 22-Nov-2006 Planners

    //Read each available chain in the SRMOD                                            //SDH 14-Sep-2006 Planners
    //(Start from where we left off last time)                                          //SDH 14-Sep-2006 Planners
    WORD wResponseItem = 0;                                                             //PAB 27-Nov-2006
    for (WORD wChain = wNextChain; wChain < 40; wChain++) {                             //PAB 30-Nov-2006 Planners

        //Read the SRMOD                                                                //SDH 14-Sep-2006 Planners
        srmodrec.ulKey = satol(pPSL->abPOGKey, sizeof(pPSL->abPOGKey));                 //SDH 14-Sep-2006 Planners
        srmodrec.ubModSeq = satoi(pPSL->abModSeq, sizeof(pPSL->abModSeq));              //SDH 14-Sep-2006 Planners
        srmodrec.ubRecChain = wChain;                                                   //SDH 14-Sep-2006 Planners
        rc2 = SrmodRead(__LINE__);                                                      //SDH 14-Sep-2006 Planners
        if (rc2 <= 0) {                                                                 //SDH 14-Sep-2006 Planners
            if ((rc2&0xFFFF != 0x06c8) || (wChain == 0)) {                              //SDH 14-Sep-2006 Planners
                prep_nak("Could not read SRMOD. "                                       //SDH 14-Sep-2006 Planners
                         "Please sign off and sign back in.");                          //SDH 14-Sep-2006 Planners
                return;                                                                 //SDH 14-Sep-2006 Planners
            }                                                                           //SDH 14-Sep-2006 Planners
            break;                                                                      //SDH 14-Sep-2006 Planners
        }                                                                               //SDH 14-Sep-2006 Planners


        //Populate response array                                                       //SDH 14-Sep-2006 Planners
        if (wDone || -1) {                                                              //PAB 22-Nov-2006

            for (wItem = wNextItem; wItem < SRMOD_NUM_ITEMS; wItem++) {                 //SDH 14-Sep-2006 Planners

                if (wResponseItem == PSR_NUM_ITEMS) {                                   //PAB 24-Nov-2006
                     // if the transmit record is full
                     WORD_TO_ARRAY(pPSR->abNextChain, wChain);                          //PAB 24-Nov-2006
                     WORD_TO_ARRAY(pPSR->abNextItem, wItem);                            //PAB 24-Nov-2006
                     wItem = -1;                                                        //PAB 24-NOv-2006
                     wDone = -1;                                                        //PAB 24-NOv-2006
                     break;                                                             //PAB 24-NOv-2006
                }                                                                       //PAB 24-Nov-2006

                //Check for zero item code and assume that it's the last item           //SDH 14-Sep-2006 Planners
                memset(sbuf, 0x00, sizeof(srmodrec.aShelfItem[wItem].abItemCode));      //SDH 14-Sep-2006 Planners
                if (memcmp(srmodrec.aShelfItem[wItem].abItemCode,                       //SDH 14-Sep-2006 Planners
                         sbuf, sizeof(srmodrec.aShelfItem[wItem].abItemCode)) == 0) {   //SDH 14-Sep-2006 Planners
                     wItem = -1;                                                        //SDH 14-Sep-2006 Planners
                     wDone = -1;                                                        //PAB 22-Nov-2006
                     break;                                                             //SDH 14-Sep-2006 Planners
                }


                //Ignore any items on other shelves                                     //SDH 14-Sep-2006 Planners
                if (srmodrec.aShelfItem[wItem].ubShelfNum == wShelfNum) {               //PAB 24-Nov-2006 Planners
                    //Populate response                                                 //SDH 14-Sep-2006 Planners
                    if (wResponseItem < PSR_NUM_ITEMS) {                                //SDH 14-Sep-2006 Planners
                        //Read description                                              //SDH 14-Sep-2006 Planners
                        MEMSET(pPSR->aShelfItem[wResponseItem].abDesc, ' ');            //SDH 14-Sep-2006 Planners
                        calc_boots_cd(idfrec.boots_code,                                //SDH 14-Sep-2006 Planners
                                      srmodrec.aShelfItem[wItem].abItemCode);           //SDH 14-Sep-2006 Planners
                        rc2 = IdfRead(__LINE__);                                        //SDH 14-Sep-2006 Planners
                        if (rc2 > 0) {                                                  //SDH 14-Sep-2006 Planners
                            memcpy(pPSR->aShelfItem[wResponseItem].abDesc,              //SDH 14-Sep-2006 Planners
                                   idfrec.stndrd_desc,                                  //SDH 14-Sep-2006 Planners
                                   _min(sizeof(pPSR->aShelfItem[wResponseItem].         //SDH 14-Sep-2006 Planners
                                               abDesc),sizeof(idfrec.stndrd_desc)));    //SDH 14-Sep-2006 Planners
                        } else {                                                        //SDH 14-Sep-2006 Planners
                            strncpy(pPSR->aShelfItem[wResponseItem].abDesc,             //SDH 14-Sep-2006 Planners
                                    "Item not on file        ",                         //SDH 14-Sep-2006 Planners
                                    sizeof(pPSR->aShelfItem[wResponseItem].abDesc));    //SDH 14-Sep-2006 Planners
                        }                                                               //SDH 14-Sep-2006 Planners
                        unpack(pPSR->aShelfItem[wResponseItem].abItemCode,              //SDH 14-Sep-2006 Planners
                               sizeof(pPSR->aShelfItem[wResponseItem].abItemCode),      //SDH 14-Sep-2006 Planners
                               srmodrec.aShelfItem[wItem].abItemCode,                   //SDH 14-Sep-2006 Planners
                               sizeof(srmodrec.aShelfItem[wItem].abItemCode),0);        //SDH 14-Sep-2006 Planners

                        WORD_TO_ARRAY(pPSR->aShelfItem[wResponseItem].abFacings,        //SDH 14-Sep-2006 Planners
                                      srmodrec.aShelfItem[wItem].ubFacings);            //SDH 14-Sep-2006 Planners
                    } else {                                                            //SDH 14-Sep-2006 Planners
                        WORD_TO_ARRAY(pPSR->abNextChain, wChain);                       //SDH 14-Sep-2006 Planners
                        WORD_TO_ARRAY(pPSR->abNextItem, wItem);                         //SDH 14-Sep-2006 Planners
                        break;                                                          //SDH 14-Sep-2006 Planners
                    }                                                                   //SDH 15-Sep-2006 Planners
                    wResponseItem++;                                                    //SDH 14-Sep-2006 Planners
                }
            }                                                                           //SDH 15-Sep-2006 Planners
        }
                                                                                        //PAB 22-Nov-2006
        //Get out we've returned all items                                              //SDH 14-Sep-2006 Planners
        if (wItem == -1) break;                                                         //pab 22-Nov-2006 Planners

        wNextItem = 0;                                                                  //PAB 24-Nov-2006

    }


    //determine the next shelf number incase they skip - which they will
    //as the data from Intactix is rubbish !!!

    for (wChain = 0; wChain < 40; wChain++) {                                         //PAB 30-Nov-2006
        srmodrec.ulKey = satol(pPSL->abPOGKey, sizeof(pPSL->abPOGKey));               //PAB 22-Nov-2006 Planners
        srmodrec.ubModSeq = satoi(pPSL->abModSeq, sizeof(pPSL->abModSeq));            //PAB 22-Nov-2006 Planners
        srmodrec.ubRecChain = wChain;                                                 //PAB 22-Nov-2006 Planners
        rc2 = SrmodRead(__LINE__);                                                    //PAB 23-Nov-2006
        if (rc2 <= 0) {                                                               //PAB 24-NOv-2006
            break;                                                                    //PAB 24-NOv-2006
        }                                                                             //PAB 24-NOv-2006
        for (WORD wItem2 = 0; wItem2 < SRMOD_NUM_ITEMS; wItem2++) {                   //PAB 22-Nov-2006 Planners
            if (srmodrec.aShelfItem[wItem2].ubShelfNum > wShelfNum) {                 //PAB 22-Nov-2006 Planners
                if (srmodrec.aShelfItem[wItem2].ubShelfNum < wNextShelf) {            //PAB 22-Nov-2006 Planners
                    wNextShelf =  srmodrec.aShelfItem[wItem2].ubShelfNum;             //PAB 22-NOv-2006 Planners
                    WORD_TO_ARRAY(pPSR->abNextShelf,wNextShelf);                      //PAB 22-Nov 2006 Planners
                    break;                                                            //PAB 22-Nov-2006 Planners
                }                                                                     //PAB 22-Nov-2006 Planners
            }
        }
    }

    // Audit                                                                         //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPSL->abPOGKey, pPSL->abPOGKey);                                        //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPSL->abModSeq, pPSL->abModSeq);                                        //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPSL->abShelfNum, pPSL->abShelfNum);                                    //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPSL->abNextChain, pPSL->abNextChain);                                  //SDH 15-Sep-2006 Planners
    lrt_log(LOG_PSL, hh_unit, dtls);                                                 //SDH 15-Sep-2006 Planners

}                                                                                    //SDH 23-Aug-2006 Planners


// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// PGL - Item Planner Enquiry                                               //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// Returns a list of planograms containing the specified item code.         //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners

void PogItemEnq(char *inbound) {                                            //SDH 14-Sep-2006 Planners

    //Input and output views, and local vars                                //SDH 14-Sep-2006 Planners
    LRT_PGL* pPGL = (LRT_PGL*)inbound;                                      //SDH 14-Sep-2006 Planners
    LRTLG_PGL* pLGPGL = (LRTLG_PGL*)dtls;                                   //SDH 14-Sep-2006 Planners
    LRT_PGI* pPGI = (LRT_PGI*)out;                                          //SDH 14-Sep-2006 Planners
    WORD wStartChain = satoi(pPGL->abStartChain, 3);                        //PAB 15-Nov-2006 Planners
    WORD wStartMod = satoi(pPGL->abStartMod, 3);                            //PAB 15-Nov-2006 Planners
    WORD wMod;                                                              //SDH 14-Sep-2006 Planners
    WORD wChn;                                                              //SDH 14-Sep-2006 Planners
    WORD wPGIPog = 0;                                                       //SDH 14-Sep-2006 Planners

    //Initial checks                                                        //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                     //SDH 14-Sep-2006 Planners

    long usrrc = prepare_workfile( hh_unit, SYS_LAB );                      //PAB 9-jan-2007
    if (usrrc<RC_IGNORE_ERR) {                                              //PAB 9-jan-2007
        if (usrrc == RC_DATA_ERR) {                                         //PAB 9-jan-2007
            prep_nak("ERRORUnable to create workfile. "                     //PAB 9-jan-2007
                     "Check appl event logs" );                             //PAB 9-jan-2007
            return;                                                         //PAB 9-jan-2007
        }                                                                   //PAB 9-jan-2007
    }

    //Build first part of resposne                                          //SDH 14-Sep-2006 Planners
    MEMCPY(pPGI->abCmd, "PGI");                                             //SDH 14-Sep-2006 Planners
    out_lth = LRT_PGI_LTH;                                                  //SDH 14-Sep-2006 Planners
    MEMSET(pPGI->abNextChain, 'F');                                         //SDH 14-Sep-2006 Planners
    MEMSET(pPGI->abNextMod, 'F');                                           //SDH 14-Sep-2006 Planners
    MEMSET(pPGI->aPog, 'F');                                                //SDH 14-Sep-2006 Planners

    //Build SRITEM key, read file, handle errors                            //SDH 14-Sep-2006 Planners
    for (wChn = wStartChain; wChn < 255; wChn++) {                          //SDH 14-Sep-2006 Planners
        pack(sritmlrec.abItemCode, sizeof(sritmlrec.abItemCode),            //SDH 14-Sep-2006 Planners
             pPGL->abItemCode, sizeof(pPGL->abItemCode), 0);                //SDH 14-Sep-2006 Planners
        sritmlrec.ubRecChain = wChn;                                        //SDH 14-Sep-2006 Planners
        if (pPGL->cLivePend == 'L') {                                       //SDH 14-Sep-2006 Planners
            rc2 = SritmlRead(__LINE__);                                     //SDH 14-Sep-2006 Planners
        } else {                                                            //SDH 14-Sep-2006 Planners
            memcpy(&sritmprec, &sritmlrec, sizeof(sritmprec));              //SDH 14-Sep-2006 Planners
            rc2 = SritmpRead(__LINE__);                                     //SDH 14-Sep-2006 Planners
            memcpy(&sritmlrec, &sritmprec, sizeof(sritmlrec));              //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners
        if (rc2 <= 0) {                                                     //SDH 14-Sep-2006 Planners
            if (wChn == wStartChain) {                                      //SDH 14-Sep-2006 Planners
                if ((rc2&0xFFFF) != 0x06c8) {                               //SDH 14-Sep-2006 Planners
                    prep_nak("ERRORCould not read from SRITML. "            //SDH 14-Sep-2006 Planners
                             "Please sign off and sign back in.");          //SDH 14-Sep-2006 Planners
                } else if (wChn == 0) {                                     //SDH 14-Sep-2006 Planners
                    prep_nak("This item is not on any planner.");           //SDH 14-Sep-2006 Planners
                } else {                                                    //SDH 14-Sep-2006 Planners
                    prep_nak("Warning: planner database may be corrupt.");  //SDH 14-Sep-2006 Planners
                }                                                           //SDH 14-Sep-2006 Planners
                return;                                                     //SDH 14-Sep-2006 Planners
            }                                                               //SDH 14-Sep-2006 Planners
            break;                                                          //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners

        //Populate response                                                 //SDH 14-Sep-2006 Planners
        for (wMod = wStartMod; wMod <= SRITM_NUM_MODS; wMod++) {            //PAB 14-Nov-2006 Planners

            //Get current POG key and read                                  //SDH 14-Sep-2006 Planners
            BYTE abKey[sizeof(pPGI->aPog->abKey)];                          //SDH 16-Nov-2006 Planners
            if (sritmlrec.aModuleKey[wMod].ulPOGKey == 0) {                 //PAB 16-Nov-2006
                continue;                                                   //PAB 16-Nov-2006
            }                                                               //PAB 16-Nov-2006 Planners
            LONG_TO_ARRAY(abKey, sritmlrec.aModuleKey[wMod].ulPOGKey);      //SDH 14-Sep-2006 Planners
            srpogrec.ulKey = satol(abKey, sizeof(abKey));                   //SDH 14-Sep-2006 Planners
            rc2 = SrpogRead(__LINE__);                                      //SDH 14-Sep-2006 Planners
            if (rc2 < 0) {                                                  //SDH 14-Sep-2006 Planners
               break;                                                       //PAB 15-Nov-2006 PLanners
            }                                                               //SDH 14-Sep-2006 Planners

            MEMCPY(pPGI->aPog[wPGIPog].abKey, abKey);                       //SDH 14-Sep-2006 Planners
            MEMSET(pPGI->aPog[wPGIPog].abDesc, ' ');                        //SDH 14-Sep-2006 Planners
            memcpy(pPGI->aPog[wPGIPog].abDesc, srpogrec.abDesc,             //SDH 14-Sep-2006 Planners
                   _min(sizeof(pPGI->aPog[wPGIPog].abDesc),                 //SDH 14-Sep-2006 Planners
                        sizeof(srpogrec.abDesc)));                          //SDH 14-Sep-2006 Planners
            WORD_TO_ARRAY(pPGI->aPog[wPGIPog].abModuleCount,                //SDH 14-Sep-2006 Planners
                          srpogrec.ubModuleCount);                          //SDH 14-Sep-2006 Planners
            wPGIPog++;                                                      //SDH 14-Sep-2006 Planners
            if (wPGIPog >= PGI_NUM_POGS) break;                             //SDH 14-Sep-2006 Planners

        }                                                                   //SDH 14-Sep-2006 Planners

        //If no more modules to come then quit loop                         //SDH 14-Sep-2006 Planners
        if (wChn * SRITM_NUM_MODS + wMod + 1 >= sritmlrec.ubModuleCount) {  //PAB 15-Nov-2006 Planners
            break;                                                          //SDH 14-Sep-2006 Planners
        }                                                                   //SDH 14-Sep-2006 Planners
        wStartMod = 0;                                                      //SDH 14-Sep-2006 Planners

    }                                                                       //SDH 14-Sep-2006 Planners

    //Populate the response to flag that there are more to come             //SDH 14-Sep-2006 Planners
    if ((wChn -1 )*PGI_NUM_POGS + (wMod) < sritmlrec.ubModuleCount) {       //PAB 15-Nov-2006 Planners
        if (wChn >= 1) {                                                    //PAB 22-Nov-2006
            wChn--;                                                         //PAB 22-Nov-2006
        }
        if (wStartMod < wMod) {                                             //PAB 22-Nov-2006
            WORD_TO_ARRAY(pPGI->abNextChain, wChn);                         //PAB 14-Sep-2006 Planners
            WORD_TO_ARRAY(pPGI->abNextMod, wMod +1);                        //PAB 15-Nov-2006 Planners
        } else {
            MEMSET(pPGI->abNextChain, 'F');                                 //PAB 24-Nov-2006 Planners
            MEMSET(pPGI->abNextMod, 'F');                                   //PAB 24-Nov-2006
        }                                                                   //PAB 24-Nov-2006
    }                                                                       //SDH 15-Nov-2006 Planners

    // Audit                                                                //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGL->abItemCode, pPGL->abItemCode);                           //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGL->abStartChain, pPGL->abStartChain);                       //SDH 15-Sep-2006 Planners
    MEMCPY(pLGPGL->abStartPlan, pPGL->abStartMod);                          //SDH 15-Sep-2006 Planners
    lrt_log(LOG_PGL, hh_unit, dtls);                                        //SDH 15-Sep-2006 Planners

}                                                                           //SDH 15-Sep-2006 Planners


// ------------------------------------------------------------------------ //SDH 20-May-2009 Model Day
//                                                                          //SDH 20-May-2009 Model Day
// PGA - Item Sites Enquiry                                                 //SDH 20-May-2009 Model Day
//                                                                          //SDH 20-May-2009 Model Day
// Returns a detailed list of sites for the specified item code.            //SDH 20-May-2009 Model Day
//                                                                          //SDH 20-May-2009 Model Day
// ------------------------------------------------------------------------ //SDH 20-May-2009 Model Day

typedef struct {                                                            //SDH 20-May-2009 Model Day
    BYTE abCmd[3];                                                          //SDH 20-May-2009 Model Day
    BYTE abOpID[3];                                                         //SDH 20-May-2009 Model Day
    BYTE abItemCode[6];                                                     //SDH 20-May-2009 Model Day
    BYTE abStartChain[3];   //Starts at 0                                   //SDH 20-May-2009 Model Day
    BYTE abStartMod[3];     //Starts at 0                                   //SDH 20-May-2009 Model Day
    BYTE abStartItem[3];    //Starts at 0                                   //SDH 20-May-2009 Model Day
    BYTE cLivePend;         //L or P                                        //SDH 20-May-2009 Model Day
} LRT_PGA;                                                                  //SDH 20-May-2009 Model Day

typedef struct {                                                            //SDH 20-May-2009 Model Day
    BYTE abPOGKey[6];                                                       //SDH 20-May-2009 Model Day
    BYTE abModCount[3];                                                     //SDH 20-May-2009 Model Day
    BYTE abRepeatCount[3];                                                  //SDH 20-May-2009 Model Day
    BYTE abPOGDesc[30];                                                     //SDH 20-May-2009 Model Day
    BYTE abModDesc[30];                                                     //SDH 20-May-2009 Model Day
    BYTE abMDQ[4];                                                          //SDH 20-May-2009 Model Day
    BYTE abPSC[4];                                                          //SDH 20-May-2009 Model Day
} Site;                                                                     //SDH 20-May-2009 Model Day
#define PGB_NUM_SITES    4                                                  //SDH 20-May-2009 Model Day
typedef struct {                                                            //SDH 20-May-2009 Model Day
    BYTE abCmd[3];                                                          //SDH 20-May-2009 Model Day
    Site aSite[PGB_NUM_SITES];                                              //SDH 20-May-2009 Model Day
    BYTE abNextChain[3];  //FFF if no more                                  //SDH 20-May-2009 Model Day
    BYTE abNextMod[3];    //FFF if no more                                  //SDH 20-May-2009 Model Day
    BYTE abNextItem[3];   //FFF if no more                                  //SDH 20-May-2009 Model Day
} LRT_PGB;                                                                  //SDH 20-May-2009 Model Day

typedef struct {                                                            //SDH 20-May-2009 Model Day
    ULONG ulPOGKey;                                                         //SDH 20-May-2009 Model Day
    UBYTE ubModSeq;                                                         //SDH 20-May-2009 Model Day
} Module;                                                                   //SDH 20-May-2009 Model Day

void PogSiteEnq(char *inbound) {                                            //SDH 20-May-2009 Model Day

    //Input and output views, and local vars                                //SDH 20-May-2009 Model Day
    LRT_PGA* pPGA = (LRT_PGA*)inbound;                                      //SDH 20-May-2009 Model Day
    LRTLG_PGA* pLGPGA = (LRTLG_PGA*)dtls;                                   //SDH 20-May-2009 Model Day
    LRT_PGB* pPGB = (LRT_PGB*)out;                                          //SDH 20-May-2009 Model Day
    Module LastModule;                                                      //SDH 20-May-2009 Model Day
    WORD wMod = satoi(pPGA->abStartMod, 3);                                 //SDH 20-May-2009 Model Day
    WORD wStartChain = satoi(pPGA->abStartChain, 3);                        //SDH 20-May-2009 Model Day
    WORD wChn = wStartChain;                                                //SDH 20-May-2009 Model Day
    WORD wItemRepeat = satoi(pPGA->abStartItem, 3);                         //SDH 20-May-2009 Model Day
    WORD wItem;                                                             //SDH 20-May-2009 Model Day
    BOOLEAN fSritmRead = FALSE;                                             //SDH 20-May-2009 Model Day
    BOOLEAN fSrmodRead = FALSE;                                             //SDH 20-May-2009 Model Day
    BOOLEAN fSrpogRead = FALSE;                                             //SDH 20-May-2009 Model Day
    BYTE abItemCodePacked[3];                                               //SDH 20-May-2009 Model Day

    //Initial checks                                                        //SDH 20-May-2009 Model Day
    if (IsStoreClosed()) return;                                            //SDH 20-May-2009 Model Day
    if (IsHandheldUnknown()) return;                                        //SDH 20-May-2009 Model Day
    UpdateActiveTime();                                                     //SDH 20-May-2009 Model Day

    //Initialise packed item code                                           //SDH 20-May-2009 Model Day
    pack(abItemCodePacked, 3, pPGA->abItemCode, 6, 0);                      //SDH 20-May-2009 Model Day 
    
    //Build default response                                                //SDH 20-May-2009 Model Day
    MEMCPY(pPGB->abCmd, "PGB");                                             //SDH 20-May-2009 Model Day
    out_lth = sizeof(LRT_PGB);                                              //SDH 20-May-2009 Model Day
    MEMSET(pPGB->abNextChain, 'F');                                         //SDH 20-May-2009 Model Day
    MEMSET(pPGB->abNextMod, 'F');                                           //SDH 20-May-2009 Model Day
    MEMSET(pPGB->abNextItem, 'F');                                          //SDH 20-May-2009 Model Day
    MEMSET(pPGB->aSite, 'F');                                               //SDH 20-May-2009 Model Day

    //Get the last MOD pointed to, so we can increment the item repeat      //SDH 20-May-2009 Model Day
    //value if required                                                     //SDH 20-May-2009 Model Day
    LastModule.ulPOGKey = 0;                                                //SDH 20-May-2009 Model Day
    LastModule.ubModSeq = 0;                                                //SDH 20-May-2009 Model Day
    if (wChn > 0 || wMod > 0) {                                             //SDH 20-May-2009 Model Day
        
        //Decrement the key values                                          //SDH 20-May-2009 Model Day
        WORD wTmpChn = wChn;                                                //SDH 20-May-2009 Model Day
        WORD wTmpMod = wMod;                                                //SDH 20-May-2009 Model Day
        if (wTmpMod == 0) {                                                 //SDH 20-May-2009 Model Day
            wTmpChn--;                                                      //SDH 20-May-2009 Model Day
            wTmpMod = SRITM_NUM_MODS - 1;                                   //SDH 20-May-2009 Model Day
        } else {                                                            //SDH 20-May-2009 Model Day
            wTmpMod--;                                                      //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
        //Build SRITEM key, read file, remember module key                  //SDH 20-May-2009 Model Day
        MEMCPY(sritmlrec.abItemCode, abItemCodePacked);                     //SDH 20-May-2009 Model Day
        sritmlrec.ubRecChain = wTmpChn;                                     //SDH 20-May-2009 Model Day
        if (pPGA->cLivePend == 'L') {                                       //SDH 20-May-2009 Model Day
            rc2 = SritmlRead(__LINE__);                                     //SDH 20-May-2009 Model Day
        } else {                                                            //SDH 20-May-2009 Model Day
            MEMCPY(&sritmprec, &sritmlrec);                                 //SDH 20-May-2009 Model Day
            rc2 = SritmpRead(__LINE__);                                     //SDH 20-May-2009 Model Day
            MEMCPY(&sritmlrec, &sritmprec);                                 //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        if (rc2 > 0) {                                                      //SDH 20-May-2009 Model Day
            fSritmRead = TRUE;                                              //SDH 20-May-2009 Model Day
            LastModule.ulPOGKey = sritmlrec.aModuleKey[wTmpMod].ulPOGKey;   //SDH 20-May-2009 Model Day
            LastModule.ubModSeq = sritmlrec.aModuleKey[wTmpMod].ubModSeq;   //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
    }                                                                       //SDH 20-May-2009 Model Day

    //Keep looping for a full reponse PLUS ONE                              //SDH 20-May-2009 Model Day
    //The last loop round is to check whether there is more to come         //SDH 20-May-2009 Model Day
    for (WORD wPGBSite = 0; wPGBSite < (PGB_NUM_SITES + 1); wPGBSite++) {   //SDH 20-May-2009 Model Day

        //Build SRITEM key, read file, handle errors                        //SDH 20-May-2009 Model Day
        if ((MEMCMP(sritmlrec.abItemCode, abItemCodePacked) != 0) ||        //SDH 20-May-2009 Model Day
            (sritmlrec.ubRecChain != wChn) || !fSritmRead) {                //SDH 20-May-2009 Model Day                          
            MEMCPY(sritmlrec.abItemCode, abItemCodePacked);                 //SDH 20-May-2009 Model Day
            sritmlrec.ubRecChain = wChn;                                    //SDH 20-May-2009 Model Day
            if (pPGA->cLivePend == 'L') {                                   //SDH 20-May-2009 Model Day
                rc2 = SritmlRead(__LINE__);                                 //SDH 20-May-2009 Model Day
            } else {                                                        //SDH 20-May-2009 Model Day
                MEMCPY(&sritmprec, &sritmlrec);                             //SDH 20-May-2009 Model Day
                rc2 = SritmpRead(__LINE__);                                 //SDH 20-May-2009 Model Day
                MEMCPY(&sritmlrec, &sritmprec);                             //SDH 20-May-2009 Model Day
            }                                                               //SDH 20-May-2009 Model Day
            if (rc2 <= 0) {                                                 //SDH 20-May-2009 Model Day
                if (wPGBSite == 0) {                                        //SDH 20-May-2009 Model Day
                    if ((rc2&0xFFFF) != 0x06c8) {                           //SDH 20-May-2009 Model Day
                        prep_nak("ERRORCould not read from SRITML. "        //SDH 20-May-2009 Model Day
                                 "Please sign off and sign back in.");      //SDH 20-May-2009 Model Day
                    } else if (wStartChain == 0) {                          //SDH 20-May-2009 Model Day
                        prep_nak("This item is not on any planner.");       //SDH 20-May-2009 Model Day
                    } else {                                                //SDH 20-May-2009 Model Day
                        prep_nak("Warning: planner database is inconsistent.");//SDH 20-May-2009 Model Day
                    }                                                       //SDH 20-May-2009 Model Day
                    return;                                                 //SDH 20-May-2009 Model Day
                }                                                           //SDH 20-May-2009 Model Day
                //Break out of the main loop as nothing more to read        //SDH 20-May-2009 Model Day
                break;                                                      //SDH 20-May-2009 Model Day
            }                                                               //SDH 20-May-2009 Model Day
            fSritmRead = TRUE;                                              //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
        //If the key is null then we've processed all the item's sites      //SDH 20-May-2009 Model Day
        if (sritmlrec.aModuleKey[wMod].ulPOGKey == 0) break;                //SDH 20-May-2009 Model Day

        //Get current POG key and read, end on read error                   //SDH 20-May-2009 Model Day
        if (srpogrec.ulKey != sritmlrec.aModuleKey[wMod].ulPOGKey ||        //SDH 20-May-2009 Model Day
            !fSrpogRead) {                                                  //SDH 20-May-2009 Model Day
            srpogrec.ulKey = sritmlrec.aModuleKey[wMod].ulPOGKey;           //SDH 20-May-2009 Model Day
            rc2 = SrpogRead(__LINE__);                                      //SDH 20-May-2009 Model Day
            if (rc2 < 0) break;                                             //SDH 20-May-2009 Model Day
            fSrpogRead = TRUE;                                              //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
        //Increment item repeat if key is the same as last time             //SDH 20-May-2009 Model Day
        if ((sritmlrec.aModuleKey[wMod].ulPOGKey == LastModule.ulPOGKey) && //SDH 20-May-2009 Model Day
            (sritmlrec.aModuleKey[wMod].ubModSeq == LastModule.ubModSeq)) { //SDH 20-May-2009 Model Day
            wItemRepeat++;                                                  //SDH 20-May-2009 Model Day
        } else {                                                            //SDH 20-May-2009 Model Day
            wItemRepeat = 0;                                                //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        LastModule.ulPOGKey = sritmlrec.aModuleKey[wMod].ulPOGKey;          //SDH 20-May-2009 Model Day
        LastModule.ubModSeq = sritmlrec.aModuleKey[wMod].ubModSeq;          //SDH 20-May-2009 Model Day
        
        //Read SRMOD                                                        //SDH 20-May-2009 Model Day
        if (srmodrec.ulKey    != sritmlrec.aModuleKey[wMod].ulPOGKey ||     //SDH 20-May-2009 Model Day
            srmodrec.ubModSeq != sritmlrec.aModuleKey[wMod].ubModSeq ||     //SDH 20-May-2009 Model Day
            srmodrec.ubRecChain != 0) {                                     //SDH 20-May-2009 Model Day
            fSrmodRead = FALSE;                                             //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        srmodrec.ulKey = sritmlrec.aModuleKey[wMod].ulPOGKey;               //SDH 20-May-2009 Model Day
        srmodrec.ubModSeq = sritmlrec.aModuleKey[wMod].ubModSeq;            //SDH 20-May-2009 Model Day
        srmodrec.ubRecChain = 0;                                            //SDH 20-May-2009 Model Day
        WORD wRepeat = wItemRepeat;                                         //SDH 20-May-2009 Model Day
        BOOLEAN fFound = FALSE;                                             //SDH 20-May-2009 Model Day
        if (!fSrmodRead) {                                                  //SDH 20-May-2009 Model Day
            rc2 = SrmodRead(__LINE__);                                      //SDH 20-May-2009 Model Day
            if (rc2 > 0) fSrmodRead = TRUE;                                 //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        while (!fFound && rc2 > 0) {                                        //SDH 20-May-2009 Model Day
            for (wItem = 0; wItem < SRMOD_NUM_ITEMS; wItem++) {             //SDH 20-May-2009 Model Day
                if (MEMCMP(srmodrec.aShelfItem[wItem].abItemCode,           //SDH 20-May-2009 Model Day
                           abItemCodePacked) == 0) {                        //SDH 20-May-2009 Model Day
                    if (wRepeat > 0) {                                      //SDH 20-May-2009 Model Day
                        wRepeat--;                                          //SDH 20-May-2009 Model Day
                    } else {                                                //SDH 20-May-2009 Model Day
                        fFound = TRUE;                                      //SDH 20-May-2009 Model Day
                        break;                                              //SDH 20-May-2009 Model Day
                    }                                                       //SDH 20-May-2009 Model Day
                }                                                           //SDH 20-May-2009 Model Day
            }                                                               //SDH 20-May-2009 Model Day
            if (fFound) break;                                              //SDH 12-Aug-2009 Model Day
            srmodrec.ubRecChain++;                                          //SDH 20-May-2009 Model Day
            rc2 = SrmodRead(__LINE__);                                      //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        if (!fFound) break;                                                 //SDH 20-May-2009 Model Day
        
        //If special look ahead go round loop, populate next fields         //SDH 20-May-2009 Model Day
        if (wPGBSite == PGB_NUM_SITES) {                                    //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->abNextChain, wChn);                         //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->abNextMod, wMod);                           //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->abNextItem, _max(wItemRepeat - 1, 0));      //SDH 20-May-2009 Model Day
       
        //Else normal processing: populate one site in the response         //SDH 20-May-2009 Model Day
        } else {                                                            //SDH 20-May-2009 Model Day
            LONG_TO_ARRAY(pPGB->aSite[wPGBSite].abPOGKey,                   //SDH 20-May-2009 Model Day
                          sritmlrec.aModuleKey[wMod].ulPOGKey);             //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->aSite[wPGBSite].abModCount,                 //SDH 20-May-2009 Model Day
                          srpogrec.ubModuleCount);                          //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->aSite[wPGBSite].abRepeatCount,              //SDH 20-May-2009 Model Day
                          sritmlrec.aModuleKey[wMod].ubRepeatCnt);          //SDH 20-May-2009 Model Day
            MEMCPY(pPGB->aSite[wPGBSite].abPOGDesc, srpogrec.abDesc);       //SDH 20-May-2009 Model Day
            MEMCPY(pPGB->aSite[wPGBSite].abModDesc, srmodrec.abModDesc);    //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->aSite[wPGBSite].abMDQ,                      //SDH 20-May-2009 Model Day
                          srmodrec.aShelfItem[wItem].uwMDQ);                //SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(pPGB->aSite[wPGBSite].abPSC,                      //SDH 20-May-2009 Model Day
                          srmodrec.aShelfItem[wItem].uwPSC);                //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
        //Increment to read next site                                       //SDH 20-May-2009 Model Day
        wMod++;                                                             //SDH 20-May-2009 Model Day
        if (wMod == SRITM_NUM_MODS) {                                       //SDH 20-May-2009 Model Day
            wChn++;                                                         //SDH 20-May-2009 Model Day
            wMod = 0;                                                       //SDH 20-May-2009 Model Day
            if (wChn > 255) break;                                          //SDH 20-May-2009 Model Day
        }                                                                   //SDH 20-May-2009 Model Day
        
    }                                                                       //SDH 20-May-2009 Model Day
    
    // Audit                                                                //SDH 20-May-2009 Model Day
    pack(pLGPGA->abItemCode, sizeof(pLGPGA->abItemCode),                    //SDH 20-May-2009 Model Day
         pPGA->abItemCode, sizeof(pPGA->abItemCode), 0);                    //SDH 20-May-2009 Model Day
    pack(pLGPGA->abStartChain, sizeof(pLGPGA->abStartChain),                //SDH 20-May-2009 Model Day
         pPGA->abStartChain, sizeof(pPGA->abStartChain), 1);                //SDH 20-May-2009 Model Day
    pack(pLGPGA->abStartPlan, sizeof(pLGPGA->abStartPlan),                  //SDH 20-May-2009 Model Day
         pPGA->abStartMod, sizeof(pPGA->abStartMod),1);                     //SDH 20-May-2009 Model Day
    pack(pLGPGA->abStartItem, sizeof(pLGPGA->abStartItem),                  //SDH 20-May-2009 Model Day
         pPGA->abStartItem, sizeof(pPGA->abStartItem),1);                   //SDH 20-May-2009 Model Day
    pLGPGA->bLivePend = pPGA->cLivePend;                                    //SDH 20-May-2009 Model Day
    MEMSET(pLGPGA->abFiller, 0x00);                                         //SDH 20-May-2009 Model Day
    lrt_log(LOG_PGA, hh_unit, dtls);                                        //SDH 20-May-2009 Model Day

}                                                                           //SDH 20-May-2009 Model Day


// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// PRP - Print Planner/Module                                               //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// Sends a planner or a module to the labeller (via PRINTSEL)               //SDH 14-Sep-2006 Planners
//                                                                          //SDH 14-Sep-2006 Planners
// ------------------------------------------------------------------------ //SDH 14-Sep-2006 Planners

void PogPrint(char *inbound) {                                              //SDH 14-Sep-2006 Planners

    LRT_PRP* pPRP = (LRT_PRP*)inbound;                                      //SDH 12-Oct-2006 Planners
    LRTLG_PRP* pLGPRP = (LRTLG_PRP*)dtls;                                   //SDH 12-Oct-2006 Planners
    time_t CurrTime;                                                        //BMG 11-09-2007

    // background_msg("Print Planner Request received");

    //Initial checks                                                        //SDH 14-Sep-2006 Planners
    if (IsStoreClosed()) return;                                            //SDH 14-Sep-2006 Planners
    if (IsHandheldUnknown()) return;                                        //SDH 14-Sep-2006 Planners
    UpdateActiveTime();                                                     //SDH 14-Sep-2006 Planners

    //If module is Fs then the request is to print the whole planner         //SDH 14-Sep-2006 Planners
    memset(selbfrec.item_code, '0', 2);                                      //SDH 14-Sep-2006 Planners
    memcpy(selbfrec.item_code + 2, pPRP->abPOGKey, 6);                       //SDH 14-Sep-2006 Planners
    selbfrec.printerid[0] = pPRP->cType;                                     //SDH 14-Sep-2006 Planners

    //if (pPRP->abMod[0] == 'F') {                                            //SDH 14-Sep-2006 Planners
    //    MEMCPY(selbfrec.info, "PRTPLN");                                    //SDH 14-Sep-2006 Planners
    //    memset(selbfrec.item_code + 8, 'F', 3);                             //SDH 14-Sep-2006 Planners
    //} else {                                                                //SDH 14-Sep-2006 Planners
    //    MEMCPY(selbfrec.info, "PRTMOD");                                    //SDH 14-Sep-2006 Planners
    //    memcpy(selbfrec.item_code + 8, pPRP->abMod, 3);                     //SDH 14-Sep-2006 Planners
    //}                                                                       //SDH 14-Sep-2006 Planners
    //
    //rc2 = s_write(A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,                  //SDH 14-Sep-2006 Planners
    //              (void *)&selbfrec, SELBF_RECL, 0L);                       //SDH 14-Sep-2006 Planners

    //if (rc2 || 0) {                                                         //PAB 22-Nov-2006 Planners
        //prepare_workfile(hh_unit, SYS_LAB);                                 //SDH 14-Sep-2006 Planners
        //Also write a header, Since the user ID and the PRTMOD/PRTPLN can't//SDH 14-Sep-2006 Planners
        //appear on the same SELBF record!                                  //SDH 14-Sep-2006 Planners
        memset(&selbfrec, '0', sizeof(selbfrec));                           //SDH 14-Sep-2006 Planners
        memcpy(selbfrec.info, pPRP->abOpID, sizeof(pPRP->abOpID));          //SDH 14-Sep-2006 Planners
        selbfrec.printerid[0] = pPRP->cType;                                //SDH 14-Sep-2006 Planners
        rc2 = s_write(A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,              //SDH 14-Sep-2006 Planners
                      (void *)&selbfrec, SELBF_RECL, 0L);                   //SDH 14-Sep-2006 Planners
        memset(selbfrec.item_code, '0', 2);                                 //SDH 14-Sep-2006 Planners
        memcpy(selbfrec.item_code + 2, pPRP->abPOGKey, 6);                  //SDH 14-Sep-2006 Planners
        selbfrec.printerid[0] = pPRP->cType;                                //SDH 14-Sep-2006 Planners
        time(&CurrTime);                                                    //BMG 11-09-2007
        pq[lrtp[hh_unit]->pq_sub1].last_access_time = CurrTime;             //BMG 11-09-2007

        if (pPRP->abMod[0] == 'F') {                                        //PAB 23-Nov-2006 Planners
            MEMCPY(selbfrec.info, "PRTPLN");                                //PAB 23-Nov-2006 Planners
            memset(selbfrec.item_code + 8, 'F', 3);                         //PAB 23-Nov-2006 Planners
        } else {                                                            //PAB 23-Nov-2006 Planners
            MEMCPY(selbfrec.info, "PRTMOD");                                //PAB 23-Nov-2006 Planners
            memcpy(selbfrec.item_code + 8, pPRP->abMod, 3);                 //PAB 23-Nov-2006 Planners
        }                                                                   //PAB 23-Nov-2006 Planners

        rc2 = s_write(A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,              //SDH 14-Sep-2006 Planners
                      (void *)&selbfrec, SELBF_RECL, 0L);
        time(&CurrTime);                                                    //BMG 11-09-2007
        pq[lrtp[hh_unit]->pq_sub1].last_access_time = CurrTime;             //BMG 11-09-2007
   // }                                                                       //PAB 22Nov2006 Planners

    if (rc2  < 0) {                                                         //SDH 14-Sep-2006 Planners
        log_event101(rc2, SELBF_REP, __LINE__);                             //PAB 23-Nov-2006 Planners
        prep_nak("Unable to write to temp SELBF. Please phone help desk."); //SDH 14-Sep-2006 Planners
        return;                                                             //SDH 14-Sep-2006 Planners
    }                                                                       //SDH 14-Sep-2006 Planners

    //Ack                                                                   //SDH 14-Sep-2006 Planners
    prep_ack("");                                                           //SDH 14-Sep-2006 Planners

    // Audit
    MEMCPY(pLGPRP->abPOGKey, pPRP->abPOGKey);                               //SDH 14-Sep-2006 Planners
    lrt_log(LOG_PRT, hh_unit, dtls);                                        //SDH 14-Sep-2006 Planners

}                                                                           //SDH 14-Sep-2006 Planners

