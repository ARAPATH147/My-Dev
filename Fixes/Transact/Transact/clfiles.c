// ****************************************************************************
// v1.0
// v2.0 - 12-03-2012 - Stock File Accuracy - Charles Skadorwa (CSk)
//                     Replaced CLOLF and CLILF layouts with new layouts.
// ****************************************************************************
#include "transact.h"

#include "trxfile.h"
#include "clfiles.h"

FILE_CTRL clolf;
FILE_CTRL clilf;
CLOLF_REC clolfrec;
CLILF_REC clilfrec;

void ClolfSet(void) {
    clolf.sessions   = 0;
    clolf.fnum       = -1L;
    clolf.pbFileName = "CLOLF";
    clolf.wOpenFlags = 0x201C;
    clolf.wReportNum = 556;
    clolf.wRecLth    = 67L;                                      //CSk 12-03-2012 SFA
    clolf.wKeyLth    = 0;
    clolf.pBuffer    = &clolfrec;
}

URC ClolfOpen(void) {
    return direct_open(&clolf, TRUE);
}

URC ClolfClose(WORD type) {
    return close_file(type, &clolf);
}

LONG ClolfRead(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&clolf, lRecNum, lLineNum, LOG_CRITICAL);
}

LONG ClolfWrite(LONG lRecNum, LONG lLineNum) {
    return WriteDirect(&clolf, lRecNum, lLineNum, LOG_ALL);
}

void ClilfSet(void) {
    clilf.sessions   = 0;
    clilf.fnum       = -1L;
    clilf.pbFileName = "CLILF";
    clilf.wOpenFlags = 0x201C;
    clilf.wReportNum = 557;
    clilf.wRecLth    = 492L;                                     //CSk 12-03-2012 SFA
    clilf.wKeyLth    = 6;
    clilf.pBuffer    = &clilfrec;
}

URC ClilfOpen(void) {
    return keyed_open(&clilf, TRUE);
}

URC ClilfClose(WORD type) {
    return close_file(type, &clilf);
}

LONG ClilfRead(LONG lLineNum) {
    return ReadKeyed(&clilf, lLineNum, LOG_CRITICAL);
}

LONG ClilfReadLock(LONG lLineNum) {
    return ReadKeyedLock(&clilf, lLineNum, LOG_CRITICAL);
}

LONG ClilfWriteUnlock(LONG lLineNum) {
    return WriteKeyedUnlock(&clilf, lLineNum, LOG_ALL);
}

LONG ClilfWrite(LONG lLineNum) {                                    //CSk 12-03-2012 SFA
    return WriteKeyed(&clilf, lLineNum, LOG_ALL);                   //CSk 12-03-2012 SFA
}                                                                   //CSk 12-03-2012 SFA
