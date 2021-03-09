// ===================================================================
// File definitions for Mobile Printing Project A7D August 2007
// Built as a seperate Header file as RFSFILE.H was
// at size limit and gave compiler errors.
// This header will only be included in the modules where the
// Recall Files are accessed. (TRANS03.C)
// ===================================================================

// --------------------------------------------------------
// Recalls Project Files - August 2007 A7C Paul Bowers
// --------------------------------------------------------


#define PHF           "PHF"                                 // Mobile Printing 08/8/2007 PAB
#define PHF_RECL      44L                                   // Mobile Printing 08/8/2007 PAB
#define PHF_KEYL      6                                     // Mobile Printing 08/8/2007 PAB
#define PHF_REP       732                                   // Mobile Printing 08/8/2007 PAB
#define PHF_OFLAGS    A_READ | A_WRITE| A_SHARE             // Mobile Printing 08/8/2007 PAB
typedef struct {                                               // Mobile Printing 08/8/2007 PAB
    BYTE ubPHFBarCode[6];                                      // 
    LONG lHist2Price;                                          // 
    BYTE aHist2Date[3];
    BYTE aHist2Type[1];
    LONG lHist1Price;                                          // 
    BYTE aHist1Date[3];
    BYTE aHist1Type[1];
    LONG lCurrentPrice;                                        // 
    BYTE aCurrentDate[3];
    BYTE aCurrentType[1];
    BYTE aCurrentLabel[1];
    LONG lPendPrice;                                           // 
    BYTE aPendDate[3];
    BYTE aPendType[1];
    BYTE aDateLastInc[3];
    BYTE aFiller[2];
} PHF_REC;               

#define IDUF           "IUDF"                              
#define IDUF_RECL      10L                                  
#define IDUF_REP       741                                  
#define IDUF_OFLAGS    A_READ | A_SHARE                     
typedef struct IDUF_Record_Layout {                         
    BYTE abDescription[10];                                       
} IDUF_REC;        

#define SELDESC        "ADXLXACN::D:\\ADX_UDT1\\SELDESC.BIN" // 08-11-07 PAB MObile Printing
#define SELDESC_RECL   512L // variable length               // 08-11-07 PAB MObile Printing
#define SELDESC_REP    665                                   // 08-11-07 PAB MObile Printing
#define SELDESC_OFLAGS    A_READ | A_WRITE| A_SHARE          // 08-11-07 PAB MObile Printing
typedef struct SELDESC_Record_Layout {                       // 08-11-07 PAB MObile Printing
    BYTE abWarning[512];                                     // 08-11-07 PAB MObile Printing
} SELDESC_REC;                                               // 08-11-07 PAB MObile Printing


