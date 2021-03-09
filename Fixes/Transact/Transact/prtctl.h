#ifndef PRTCTL_H
#define PRTCTL_H

#include "trxfile.h"

// record layout
typedef struct {
    BYTE prtnum[10];
} PRTCTL_REC;

extern FILE_CTRL prtctl;
extern PRTCTL_REC prtctlrec;

void PrtctlSet(void);
URC PrtctlOpen(void);
URC PrtctlClose(WORD type);
LONG PrtctlRead(LONG lRecNum, LONG lLineNum);

#endif
