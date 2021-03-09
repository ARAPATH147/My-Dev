#ifndef IRF_H
#define IRF_H

#include "trxfile.h"

//Used by IRF and IRFDEX                                                    //SDH 14-01-2005 Promotions
typedef struct DEALDATA {                                                   //SDH 14-01-2005 Promotions
    UWORD uwDealNum:14;                                                     //SDH 14-01-2005 Promotions
    UBYTE ubListID:2;                                                       //SDH 14-01-2005 Promotions
} DEALDATA;                                                                 //SDH 14-01-2005 Promotions

typedef struct IRF_Record_Layout {
    BYTE bar_code[11];
    UBYTE indicat0;
    UBYTE indicat1;
    DEALDATA dealdata0;                                                     // v3.0
    UBYTE indicat8;                                                         // 23-5-07 PAB recalls
    BYTE unused1[2];                                                        // 23-5-07 PAB recalls
    BYTE salepric[5];
    UBYTE indicat5;                                                         // v3.0
    BYTE itemname[18];
    BYTE boots_code[3];
    DEALDATA dealdata1;                                                     // v3.0
    DEALDATA dealdata2;                                                     // v3.0
    UBYTE indicat3;
} IRF_REC;

typedef struct {                                                            //SDH 14-01-2005 Promotions
    BYTE abItemCodePD[3];                                                   //SDH 14-01-2005 Promotions
    DEALDATA aDealData[37];                                                 //SDH 14-01-2005 Promotions // 18-05-2009 BMG
    BYTE abFiller[7];                                                                                   // 18-05-2009 BMG
} IRFDEX_REC;                                                               //SDH 14-01-2005 Promotions

extern FILE_CTRL irf;
extern FILE_CTRL irfdex;
extern IRF_REC irfrec;
extern IRFDEX_REC irfdexrec;

void IrfSet(void);
URC IrfOpen(void);
URC IrfClose(WORD type);
LONG IrfRead(LONG lLineNum);

void IrfdexSet(void);
URC IrfdexOpen(void);
URC IrfdexClose(WORD type);
LONG IrfdexRead(LONG lLineNum);

#endif

