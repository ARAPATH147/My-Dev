#ifndef SELDESC_H
#define SELDESC_H

#include "trxfile.h"

typedef struct {                                             // 08-11-07 PAB MObile Printing
    BYTE abWarning[512];                                     // 08-11-07 PAB MObile Printing
} SELDESC_REC;                                               // 08-11-07 PAB MObile Printing

extern FILE_CTRL seldesc;
extern SELDESC_REC seldescrec;

void SeldescSet(void);
URC SeldescOpen(void);
URC SeldescClose(WORD type);
LONG SeldescRead(LONG lRecNum, LONG lLineNum);

#endif

