/*************************************************************************
*
* File: IUDF.C
*
* IUDF File Functions
*
* Version 1                Rejiya Nair                 28-06-2013
* Original version.
* Event Log Rationalisation
*
**************************************************************************/

#include "transact.h"

#include "trxfile.h"
#include "iudf.h"

FILE_CTRL iudf;
IUDF_REC iudfrec;

void IudfSet(void) {
    iudf.sessions   = 0;
    iudf.fnum       = -1L;
    iudf.pbFileName = "IUDF";
    iudf.wOpenFlags = A_READ | A_SHARE;
    iudf.wReportNum = 586;
    iudf.wRecLth    = 10L;
    iudf.wKeyLth    = 0;
    iudf.pBuffer    = &iudfrec;
}

URC IudfOpen(void) {
    return direct_open(&iudf, TRUE);
}

URC IudfClose(WORD type) {
    return close_file(type, &iudf);
}

LONG IudfRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&iudf, lRecNum, lLineNum, LOG_ALL);
}

