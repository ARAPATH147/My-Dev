#include "transact.h"

#include "trxfile.h"
#include "prtctl.h"

FILE_CTRL prtctl;
PRTCTL_REC prtctlrec;

void PrtctlSet(void) {
    prtctl.sessions   = 0;
    prtctl.fnum       = -1L;
    prtctl.pbFileName = "D:/ADX_UDT1/PRTCTL.BIN";
    prtctl.wOpenFlags = 0x2018;
    prtctl.wReportNum = 0;
    prtctl.wRecLth    = 12L;
    prtctl.wKeyLth    = 0;
    prtctl.pBuffer    = &prtctlrec;
}

URC PrtctlOpen(void) {
    return direct_open(&prtctl, TRUE);
}

URC PrtctlClose(WORD type) {
    return close_file(type, &prtctl);
}

LONG PrtctlRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&prtctl, lRecNum, lLineNum, LOG_ALL);
}

