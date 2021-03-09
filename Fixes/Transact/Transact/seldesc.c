#include "transact.h"

#include "trxfile.h"
#include "seldesc.h"

FILE_CTRL seldesc;
SELDESC_REC seldescrec;

void SeldescSet(void) {
    seldesc.sessions   = 0;
    seldesc.fnum       = -1L;
    seldesc.pbFileName = "ADXLXACN::D:\\ADX_UDT1\\SELDESC.BIN";
    seldesc.wOpenFlags = A_READ | A_WRITE| A_SHARE;
    seldesc.wReportNum = 665;
    seldesc.wRecLth    = 512L;
    seldesc.wKeyLth    = 0;
    seldesc.pBuffer    = &seldescrec;
}

URC SeldescOpen(void) {
    return direct_open(&seldesc, TRUE);
}

URC SeldescClose(WORD type) {
    return close_file(type, &seldesc);
}

LONG SeldescRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&seldesc, lRecNum, lLineNum, LOG_ALL);
}

