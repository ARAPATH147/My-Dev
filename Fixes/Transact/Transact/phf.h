#ifndef PHF_H
#define PHF_H

#include "trxfile.h"

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

extern PHF_REC phfrec;
extern FILE_CTRL phf;

void PhfSet(void);
URC PhfOpen(void);
URC PhfClose(WORD type);
LONG PhfRead(LONG lLineNum);

#endif

