#ifndef RFHIST_H
#define RFHIST_H

#include "trxfile.h"

//  record layout
typedef struct {
    BYTE boots_code[4];                                                     // UPD, format [0BBBBBBC] (C=check digit)
    BYTE date_last_pchk[4];                                                 // UPD, format [YYYYMMDD]
    BYTE price_last_pchk[3];                                                // UPD
    BYTE date_last_gap[4];                                                  // UPD, format [YYYYMMDD]
    UBYTE ubPgfOssrFlag:1;                                                  // SDH 20-04-2005
    UBYTE ubItemOssrFlag:1;                                                 // SDH 20-04-2005
    UBYTE ubGAPMFlag:1;                                                     // SDH 28-04-2005
    BYTE bUnused:5;                                                         // SDH 28-04-2005
    BYTE resrv[2];
} RFHIST_REC;

extern FILE_CTRL rfhist;
extern RFHIST_REC rfhistrec;

void RfhistSet(void);
URC RfhistOpen(void);
URC RfhistClose(WORD type);
LONG RfhistRead(LONG lLineNum);
LONG RfhistWrite(LONG lLineNumber);
URC ProcessRfhist(BYTE* pbBootsCode, BYTE cUpdateOssrItem, LONG lLineNumber); // SDH 17-11-04 OSSR WAN

#endif

