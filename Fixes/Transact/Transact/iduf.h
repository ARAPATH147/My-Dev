/*************************************************************************
* This module is redundant
*************************************************************************/

#ifndef IDUF_H
#define IDUF_H

#include "trxfile.h"

typedef struct {
    BYTE abDescription[10];
} IDUF_REC;

extern FILE_CTRL iduf;
extern IDUF_REC idufrec;

void IdufSet(void);
URC IdufOpen(void);
URC IdufClose(WORD type);
LONG IdufRead(LONG lRecNum, LONG lLineNum);

#endif

