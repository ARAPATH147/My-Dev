#include "transact.h"

#include "trxfile.h"
#include "nvurl.h"

FILE_CTRL nvurl;
NVURL_REC nvurlrec;

void NvurlSet(void) {
    nvurl.sessions   = 0;
    nvurl.fnum       = -1L;
    nvurl.pbFileName = "NVURL";
    nvurl.wOpenFlags = 0x2018;
    nvurl.wReportNum = 0;
    nvurl.wRecLth    = 127L;
    nvurl.wKeyLth    = 3;
    nvurl.pBuffer    = &nvurlrec;
}

URC NvurlOpen(void) {
    return keyed_open(&nvurl, TRUE);
}

URC NvurlClose(WORD type) {
    return close_file(type, &nvurl);
}

LONG NvurlRead(LONG lLineNum) {
    return ReadKeyed(&nvurl, lLineNum, LOG_CRITICAL);
}


