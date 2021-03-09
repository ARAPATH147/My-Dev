#include "transact.h"

#include "phf.h"

PHF_REC phfrec;
FILE_CTRL phf;

void PhfSet(void) {
    phf.sessions   = 0;
    phf.fnum       = -1L;
    phf.pbFileName = "PHF";
    phf.wOpenFlags = A_READ | A_WRITE| A_SHARE;
    phf.wReportNum = 732;
    phf.wRecLth    = 44L;
    phf.wKeyLth    = 6;
    phf.pBuffer    = &phfrec;
}

URC PhfOpen(void) {
    return keyed_open(&phf, TRUE);
}

URC PhfClose(WORD type) {
    return close_file(type, &phf);
}

LONG PhfRead(LONG lLineNum) {
    return ReadKeyed(&phf, lLineNum, LOG_CRITICAL);
}

