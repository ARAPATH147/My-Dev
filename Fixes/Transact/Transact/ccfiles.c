#include "transact.h"

#include "osfunc.h"
#include "trxfile.h"
#include "ccfiles.h"

FILE_CTRL ccdmy, cclol, ccilf, cchist, ccdirsu;
CCLOL_REC cclolrec;
CCILF_REC ccilfrec;
CCHIST_REC cchistrec;
CCDIRSU_REC ccdirsurec;

void CcdmySet(void) {
    ccdmy.sessions   = 0;
    ccdmy.fnum       = -1L;
    ccdmy.pbFileName = "CCDMY";
    ccdmy.wOpenFlags = A_FORCE;
    ccdmy.wReportNum = 324;
    ccdmy.wRecLth    = 0;
    ccdmy.wKeyLth    = 0;
    ccdmy.pBuffer    = NULL;
}
//Routines to open and close the CCDMY.  This file is opened locked
//to prevent PDTs from accessing credit claiming functionality
//if RF credit claiming is enabled.  As such, there is no concept
//of sessions with HHTs and so the session count is onmly incremented
//when the file is physically opened
URC CcdmyOpenLocked(void) {
    if (ccdmy.sessions==0) {
        ccdmy.fnum = s_open(ccdmy.wOpenFlags, ccdmy.pbFileName);
        if (ccdmy.fnum <= 0L) {
            sprintf(msg, "Err-O CCDMY. RC:%08lX", ccdmy.fnum);
            disp_msg(msg);
            return RC_DATA_ERR;
        }
        ccdmy.sessions = 1;
    }
    return RC_OK;
}
URC CcdmyClose(void) {
    if (ccdmy.sessions != 0) {
        s_close(0, ccdmy.fnum);
        ccdmy.sessions = 0;
    }
    return RC_OK;
}


void CclolSet(void) {
    cclol.sessions   = 0;
    cclol.fnum       = -1L;
    cclol.pbFileName = "ADXLXACN::D:/ADX_UDT1/CCLOL.BIN";
    cclol.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    cclol.wReportNum = 691;
    cclol.wRecLth    = 150L;
    cclol.wKeyLth    = 0;
    cclol.pBuffer    = &cclolrec;
}
URC CclolOpen(void) {
    return direct_open(&cclol, TRUE);                                       // SDH 17-03-05 EXCESS
}
URC CclolClose(WORD type) {
    return close_file(type, &cclol);
}
LONG CclolRead(LONG lRecNum, LONG lLineNum) {                               // SDH 29-11-04 CREDIT CLAIM
    return ReadDirect(&cclol, lRecNum, lLineNum, LOG_ALL);                  // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM
LONG CclolWrite(LONG lRecNum, LONG lLineNum) {                              // SDH 29-11-04 CREDIT CLAIM
    return WriteDirect(&cclol, lRecNum, lLineNum, LOG_ALL);                 // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

void CcilfSet(void) {
    ccilf.sessions   = 0;
    ccilf.fnum       = -1L;
    ccilf.pbFileName = "ADXLXACN::D:/ADX_UDT1/CCILF.BIN";
    ccilf.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    ccilf.wReportNum = 692;
    ccilf.wRecLth    = sizeof(CCILF_REC);
    ccilf.wKeyLth    = 8;
    ccilf.pBuffer    = &ccilfrec;
}
URC CcilfOpen(void) {
    return keyed_open(&ccilf, TRUE);                                        // SDH 17-03-05 EXCESS
}
URC CcilfClose(WORD type) {
    return close_file(type, &ccilf);
}
LONG CcilfRead(LONG lLineNumber) {                                          // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&ccilf, lLineNumber, LOG_CRITICAL);                    // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
LONG CcilfWrite(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&ccilf, lLineNumber, LOG_ALL);                        // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

void CchistSet(void) {
    cchist.sessions   = 0;
    cchist.fnum       = -1L;
    cchist.pbFileName = "ADXLXACN::D:/ADX_UDT1/CCHIST.BIN";
    cchist.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    cchist.wReportNum = 693;
    cchist.wRecLth    = sizeof(CCHIST_REC);
    cchist.wKeyLth    = 14;
    cchist.pBuffer    = &cchistrec;
}
URC CchistOpen(void) {
    return keyed_open(&cchist, TRUE);                                       // SDH 17-03-05 EXCESS
}
URC CchistClose(WORD type) {
    return close_file(type, &cchist);
}
LONG CchistRead(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&cchist, lLineNumber, LOG_CRITICAL);                   // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
LONG CchistWrite(LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&cchist, lLineNumber, LOG_ALL);                       // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

void CcdirsuSet(void) {
    ccdirsu.sessions   = 0;
    ccdirsu.fnum       = -1L;
    ccdirsu.pbFileName = "ADXLXACN::D:/ADX_UDT1/CCDIRSU.BIN";
    ccdirsu.wOpenFlags = A_READ | A_SHARE;
    ccdirsu.wReportNum = 694;
    ccdirsu.wRecLth    = sizeof(CCDIRSU_REC);
    ccdirsu.wKeyLth    = 5;
    ccdirsu.pBuffer    = &ccdirsurec;
}
URC CcdirsuOpen(void) {
    return keyed_open(&ccdirsu, TRUE);                                      // SDH 17-03-05 EXCESS
}
URC CcdirsuClose(WORD type) {
    return close_file(type, &ccdirsu);
}
LONG CcdirsuRead(LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&ccdirsu, lLineNumber, LOG_CRITICAL);                  // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

