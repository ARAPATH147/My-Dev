#include "transact.h"

#include "trxfile.h"
#include "prtlist.h"

FILE_CTRL prtlist;
PRTLIST_REC prtlistrec;

void PrtlistSet(void) {
    prtlist.sessions   = 0;
    prtlist.fnum       = -1L;
    prtlist.pbFileName = "D:/ADX_UDT1/PRTLIST.BIN";
    prtlist.wOpenFlags = 0x2018;
    prtlist.wReportNum = 0;
    prtlist.wRecLth    = 200L;  // Variable Length
    prtlist.wKeyLth    = 0;
    prtlist.pBuffer    = &prtlistrec;
}

URC PrtlistOpen(void) {
    return direct_open(&prtlist, TRUE);
}

URC PrtlistClose(WORD type) {
    return close_file(type, &prtlist);
}

LONG PrtlistRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&prtlist, lRecNum, lLineNum, LOG_ALL);
}

