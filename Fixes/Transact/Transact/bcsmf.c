/*****************************************************************************
***
*** BCSMF file functions
***
*****************************************************************************/

#include "transact.h"

#include "trxfile.h"
#include "bcsmf.h"

FILE_CTRL bcsmf;
BCSMF_REC bcsmfrec;

void BcsmfSet(void) {
    bcsmf.sessions   = 0;
    bcsmf.fnum       = -1L;
    bcsmf.pbFileName = "BCSMF";
    bcsmf.wOpenFlags = A_READ | A_SHARE;
    bcsmf.wReportNum = 84;
    bcsmf.wRecLth    = 33L;
    bcsmf.wKeyLth    = 1;
    bcsmf.pBuffer    = &bcsmfrec;
}

URC BcsmfOpen(void) {
    return keyed_open(&bcsmf, TRUE);                                        // SDH 17-03-05 EXCESS
}
URC BcsmfClose(WORD type) {
    return close_file(type, &bcsmf);
}
LONG BcsmfRead(LONG lLineNum) {                                             // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&bcsmf, lLineNum, LOG_ALL);
}                                                                           // SDH 17-11-04 OSSR WAN
