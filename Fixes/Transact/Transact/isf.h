#ifndef ISF_H
#define ISF_H

#include "trxfile.h"

typedef struct ISF_Record_Layout {
    BYTE boots_code[4];
    BYTE sel_desc[45];
    BYTE cUnitType;                                // PAB 13-8-07
    UWORD wQty1;                                   // PAB 13-8-07 //BMG 6.1 17-10-2007
    BYTE cQty2;                                    // PA B 13-8-07
    WORD integer2:15;                              // PAB 8-8-07
} ISF_REC;

extern FILE_CTRL isf;
extern ISF_REC isfrec;

void IsfSet(void);
URC IsfOpen(void);
URC IsfClose(WORD type);
LONG IsfRead(LONG lLineNum);

#endif

