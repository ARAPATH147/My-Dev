#include "transact.h"

#include "trxfile.h"
#include "pgf.h"

FILE_CTRL pgf;
PGF_REC pgfrec;

void PgfSet(void) {
    pgf.sessions   = 0;
    pgf.fnum       = -1L;
    pgf.pbFileName = "PGF";
    pgf.wOpenFlags = 0x2018;
    pgf.wReportNum = 10;
    pgf.wRecLth    = 30L;
    pgf.wKeyLth    = 3;
    pgf.pBuffer    = &pgfrec;
}

URC PgfOpen(void) {
    return keyed_open(&pgf, TRUE);
}

URC PgfClose(WORD type) {
    return close_file(type, &pgf);
}

LONG PgfRead(LONG lLineNum) {
    return ReadKeyed(&pgf, lLineNum, LOG_ALL);
}


