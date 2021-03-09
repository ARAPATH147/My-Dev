#include "transact.h"

#include "idf.h"

IDF_REC idfrec;
FILE_CTRL idf;

void IdfSet(void) {
    idf.sessions   = 0;
    idf.fnum       = -1L;
    idf.pbFileName = "IDF";
    idf.wOpenFlags = 0x2018;
    idf.wReportNum = 6;
    idf.wRecLth    = 60L;
    idf.wKeyLth    = 4;
    idf.pBuffer    = &idfrec;
}

URC IdfOpen(void) {
    return keyed_open(&idf, TRUE);
}

URC IdfClose(WORD type) {
    return close_file(type, &idf);
}

LONG IdfRead(LONG lLineNum) {
    return ReadKeyed(&idf, lLineNum, LOG_CRITICAL);
}


