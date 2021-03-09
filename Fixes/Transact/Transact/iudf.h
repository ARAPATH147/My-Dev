/*************************************************************************
*
* File: IUDF.H
*
* IUDF File Header
*
* Version 1                Rejiya Nair                 28-06-2013
* Original version.
* Event Log Rationalisation
*
**************************************************************************/

#ifndef IUDF_H
#define IUDF_H

#include "trxfile.h"

typedef struct {
    BYTE abDescription[10];
} IUDF_REC;

extern FILE_CTRL iudf;
extern IUDF_REC iudfrec;

void IudfSet(void);
URC IudfOpen(void);
URC IudfClose(WORD type);
LONG IudfRead(LONG lRecNum, LONG lLineNum);

#endif

