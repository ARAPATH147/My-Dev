#include "transact.h"

#include "trxfile.h"
#include "af.h"

FILE_CTRL af;
EALAUTH_REC afrec;

void AfSet(void) {
    af.sessions   = 0;
    af.fnum       = -1L;
    af.pbFileName = "EALAUTH";
    af.wOpenFlags = 0x2018;
    af.wReportNum = 2;
    af.wRecLth    = 80L;
    af.wKeyLth    = 4;
    af.pBuffer    = &afrec;
}

URC AfOpen(void) {
    return keyed_open(&af, TRUE);
}

URC AfClose(WORD type) {
    return close_file(type, &af);
}

LONG AfRead(LONG lLineNum) {
    return ReadKeyed(&af, lLineNum, LOG_CRITICAL);
}


