#include "transact.h"

#include "trxfile.h"
#include "ealterms.h"

EALTERMS_REC ealtermsrec;
FILE_CTRL    ealterms;

void EaltermsSet(void) {
    ealterms.sessions   = 0;
    ealterms.fnum       = -1L;
    ealterms.pbFileName = "EALTERMS";
    ealterms.wOpenFlags = A_READ | A_SHARE;
    ealterms.wReportNum = 29;
    ealterms.wRecLth    = 63L;
    ealterms.wKeyLth    = 2;
    ealterms.pBuffer    = &ealtermsrec;
}

URC EaltermsOpen(void) {
    return keyed_open(&ealterms, TRUE);
}

URC EaltermsClose(WORD type) {
    return close_file(type, &ealterms);
}

LONG EaltermsRead(LONG lLineNum) {
    return ReadKeyed(&ealterms, lLineNum, LOG_CRITICAL);
}


