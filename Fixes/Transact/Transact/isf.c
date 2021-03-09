#include "transact.h"

#include "trxfile.h"
#include "isf.h"

FILE_CTRL isf;
ISF_REC isfrec;

void IsfSet(void) {
    isf.sessions   = 0;
    isf.fnum       = -1L;
    isf.pbFileName = "ISF";
    isf.wOpenFlags = 0x2018;
    isf.wReportNum = 9;
    isf.wRecLth    = 55L;
    isf.wKeyLth    = 4;
    isf.pBuffer    = &isfrec;
}

URC IsfOpen(void) {
    return keyed_open(&isf, TRUE);
}

URC IsfClose(WORD type) {
    return close_file(type, &isf);
}

LONG IsfRead(LONG lLineNum) {
    return ReadKeyed(&isf, lLineNum, LOG_ALL);
}


