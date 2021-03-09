#include "transact.h"

#include "trxfile.h"
#include "invok.h"

FILE_CTRL invok;
INVOK_REC invokrec;

void InvokSet(void) {
    invok.sessions   = 0;
    invok.fnum       = -1L;
    invok.pbFileName = "INVOK";
    invok.wOpenFlags = 0x2018;
    invok.wReportNum = 89;
    invok.wRecLth    = 80L;
    invok.wKeyLth    = 0;
    invok.pBuffer    = &invokrec;
}

URC InvokOpen(void) {
    return direct_open(&invok, TRUE);
}

URC InvokClose(WORD type) {
    return close_file(type, &invok);
}

LONG InvokRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&invok, lRecNum, lLineNum, LOG_ALL);
}
