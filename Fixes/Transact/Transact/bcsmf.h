#ifndef BCSMF_H
#define BCSMF_H

#include "trxfile.h"

typedef struct {
    BYTE cBusCentre;
    BYTE abDesc[14];
    BYTE bRecntLimit;
    BYTE bMinRecntLimit;
    BYTE bMaxRecntLimit;
    WORD wDiscrepencyValue;
    BYTE bDiscrepencyCount;
    BYTE bDiscrepencyPrcnt;
    BYTE bStockCountLimit;
    WORD wMinListNum;
    WORD wMaxListNum;
    BYTE bSequenceNum;
    BYTE bPseudoBusCentre;
    BYTE bNoRepeatTickets;
    BYTE abFiller[3];
} BCSMF_REC;

extern FILE_CTRL bcsmf;
extern BCSMF_REC bcsmfrec;

void BcsmfSet(void);
URC BcsmfOpen(void);
URC BcsmfClose(WORD type);
LONG BcsmfRead(LONG lLineNum);

#endif

