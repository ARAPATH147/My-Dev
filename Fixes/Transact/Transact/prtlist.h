#ifndef PRTLIST_H
#define PRTLIST_H

#include "trxfile.h"

// record layout
typedef struct {
    BYTE prtdesc[200];
} PRTLIST_REC;

extern FILE_CTRL prtlist;
extern PRTLIST_REC prtlistrec;

void PrtlistSet(void);
URC PrtlistOpen(void);
URC PrtlistClose(WORD type);
LONG PrtlistRead(LONG lRecNum, LONG lLineNum);

#endif

