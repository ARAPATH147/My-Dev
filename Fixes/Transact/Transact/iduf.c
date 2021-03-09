/*************************************************************************
* This module is redundant
*************************************************************************/

#include "transact.h"

#include "trxfile.h"
#include "iduf.h"

FILE_CTRL iduf;
IDUF_REC idufrec;

void IdufSet(void) {
    iduf.sessions   = 0;
    iduf.fnum       = -1L;
    iduf.pbFileName = "IDUF";
    iduf.wOpenFlags = A_READ | A_SHARE;
    iduf.wReportNum = 741;
    iduf.wRecLth    = 10L;
    iduf.wKeyLth    = 0;
    iduf.pBuffer    = &idufrec;
}

URC IdufOpen(void) {
    return direct_open(&iduf, TRUE);
}

URC IdufClose(WORD type) {
    return close_file(type, &iduf);
}

LONG IdufRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&iduf, lRecNum, lLineNum, LOG_ALL);
}

