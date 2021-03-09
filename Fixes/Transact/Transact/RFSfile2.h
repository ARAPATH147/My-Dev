// ===================================================================
// File definitions for Recalls Project A7C May 2007
// Built as a seperate Header file as RFSFILE.H was
// at size limit and gave compiler errors.
// This header will only be included in the modules where the
// Recall Files are accessed. (TRANS02.C)
// ===================================================================

// ------------------------------------------------
// Recalls Project Files - May 2007 A7C Paul Bowers
// ------------------------------------------------
//
// 1.00 - BMG 14-05-2008
//        Reverse Logistics Changes

#define RCINDX           "RCINDX"                              // Recalls 22/5/2007 PAB
#define RCINDX_RECL      48L                                   // Recalls 22/5/2007 PAB // 14/04/2008 1.0 BMG 
#define RCINDX_REP       741                                   // Recalls 22/5/2007 PAB
#define RCINDX_OFLAGS    A_READ | A_SHARE                      // Recalls 22/5/2007 PAB
typedef struct RCINDX_Record_Layout {                          // Recalls 22/5/2007 PAB
    BYTE abIndexNum[2];                                        // Recalls 22/5/2007 PAB
    BYTE abRecallReference[4];                                 // Recalls 22/5/2007 PAB
    BYTE abRecallDesc[20];                                     // Recalls 22/5/2007 PAB
    BYTE abActiveDate[4];                                      // Recalls 22/5/2007 PAB
    BYTE cRecallType[1];                                       // Recalls 22/5/2007 PAB
    BYTE abItemCount[2];                                       // Recalls 22/5/2007 PAB
    BYTE cRecallStatus[1];                                     // Recalls 22/5/2007 PAB
    BYTE cSpeicalIns[1];                                       // Recalls 29/5/2007 PAB
    BYTE abLabelType[2];
    BYTE abExpiryDate[4];                                                               // 14/04/2008 1.0 BMG
    BYTE cMRQ[2];                                                                       // 06/05/2008 1.0 BMG
    BYTE abFiller[7];                                          // Recalls 22/5/2007 PAB // 14/04/2008 1.0 BMG
} RCINDX_REC;                                                  // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RECOK           "RECOK"                                // Recalls 22/5/2007 PAB
#define RECOK_RECL      80L                                    // Recalls 22/5/2007 PAB
#define RECOK_REP       742                                    // Recalls 22/5/2007 PAB
#define RECOK_OFLAGS    A_READ | A_SHARE                       // Recalls 22/5/2007 PAB
typedef struct RECOK_Record_Layout {                           // Recalls 22/5/2007 PAB
    BYTE cPSB70Processing;                                     // Recalls 22/5/2007 PAB
    BYTE cIRFRefrsh;                                           // Recalls 22/5/2007 PAB
    BYTE cFlag2;                                               // Recalls 22/5/2007 PAB
    BYTE cFlag3;                                               // Recalls 22/5/2007 PAB
    BYTE cDate[8];                                             // Recalls 22/5/2007 PAB
    BYTE cTime[6];                                             // Recalls 22/5/2007 PAB
    BYTE abFiller[62];                                         // Recalls 22/5/2007 PAB
} RECOK_REC;                                                   // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RCSPI           "RCSPI"                                // Recalls 22/5/2007 PAB
#define RCSPI_RECL      168L                                   // Recalls 22/5/2007 PAB
#define RCSPI_KEYL      8                                      // Recalls 22/5/2007 PAB
#define RCSPI_REP       743                                    // Recalls 22/5/2007 PAB
#define RCSPI_OFLAGS    A_READ | A_SHARE                       // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE abRecallReference[8];                                 // Recalls 22/5/2007 PAB
    BYTE abMessage[160];                                       // Recalls 22/5/2007 PAB
} RCSPI_REC;                                                   // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RECALL           "RECALLS"                             // Recalls 22/5/2007 PAB
#define RECALL_RECL      508L                                  // Recalls 22/5/2007 PAB
#define RECALL_KEYL      9                                     // Recalls 22/5/2007 PAB
#define RECALL_REP       740                                   // Recalls 22/5/2007 PAB
#define RECALL_OFLAGS    A_READ | A_WRITE| A_SHARE             // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE abItemCode[3];                                        // Recalls 22/5/2007 PAB
    BYTE anCountTSF[2];                                        // Recalls 20/6/2007 PAB
    BYTE anSessionCount[2];                                    // Recalls 20/6/2007 PAB
    BYTE cCountUpdatedToday[1];                                // Recalls 22/5/2007 PAB
} RecallItems;                                                 // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE abRecallReference[8];                                 // Recalls 22/5/2007 PAB
    UBYTE ubChain;                                             // Recalls 22/5/2007 PAB
    BYTE cRecallType;                                          // Recalls 22/5/2007 PAB
    BYTE abDescription[20];                                    // Recalls 22/5/2007 PAB
    BYTE anLabelType[8];                                       // Recalls 22/5/2007 PAB
    BYTE cSupplyRoute[1];                                      // Recalls 22/5/2007 PAB
    BYTE cReasonCode[1];
    BYTE cBussinessCenter[1];                                  // Recalls 22/5/2007 PAB
    BYTE abActiveDate[8];                                      // Recalls 22/5/2007 PAB
    BYTE abDueDate[8];                                         // Recalls 22/5/2007 PAB
    BYTE abCompleteDate[8];                                    // Recalls 22/5/2007 PAB
    BYTE cRecallStatus[1];                                     // Recalls 22/5/2007 PAB
    BYTE abBatchNumbers[30];                                   // Recalls 22/5/2007 PAB
    BYTE anItemCount[4];                                       // Recalls 22/5/2007 PAB
    RecallItems aRecallItems[50];                              // Recalls 22/5/2007 PAB
    BYTE abFiller[8];                                          // Recalls 22/5/2007 PAB
} RECALL_REC;                                                  // Recalls 22/5/2007 PAB
                                                               
