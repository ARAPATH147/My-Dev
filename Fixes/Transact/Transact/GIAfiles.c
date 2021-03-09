// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Goods In Application Files Module
//
// Version 1.0               Brian Greenfield            10th October 2008
//     Initial Version
//
// Version 1.1               Rejiya Nair                 28th June 2013
// Event Log Rationalisation  (commented: //RJN 28-06-2013 ELR)
// Changes to remove the unnecessary application events logged from
// TRANSACT that are not supposed to be actually logged.
// ------------------------------------------------------------------------

#include "transact.h"

#include "osfunc.h"
#include "trxfile.h"
#include "GIAfiles.h"

FILE_CTRL carton, diror, delvindx, delvsmry, delvlist, dirsu, uodot, uodin;


// ------------------------------------------------------------------------
// CARTON File
// ------------------------------------------------------------------------

CARTON_REC cartonrec;

void CartonSet(void) {
    carton.sessions   = 0;
    carton.fnum       = -1L;
    carton.pbFileName = "CARTON";
    carton.wOpenFlags = A_READ | A_SHARE;
    carton.wReportNum = 735;
    carton.pBuffer    = &cartonrec;
    carton.wRecLth    = 508L;
    carton.wKeyLth    = 8L;
}

URC open_carton(void) {
    return keyed_open(&carton, TRUE);
}
URC close_carton(WORD type) {
    return close_file(type, &carton);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadCarton
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadCarton(LONG lLineNumber) {
    return ReadKeyed(&carton, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadCartonLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadCartonLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&carton, lLineNumber, wLogLevel);
}


// ------------------------------------------------------------------------
// DIROR File
// ------------------------------------------------------------------------

DIROR_REC dirorrec;

void DirorSet(void) {
    diror.sessions   = 0;
    diror.fnum       = -1L;
    diror.pbFileName = "DIROR";
    diror.wOpenFlags = A_READ | A_SHARE;
    diror.wReportNum = 229;
    diror.pBuffer    = &dirorrec;
    diror.wRecLth    = 508L;
    diror.wKeyLth    = 9L;
}

URC open_diror(void) {
    return keyed_open(&diror, TRUE);
}
URC close_diror(WORD type) {
    return close_file(type, &diror);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDiror
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDiror(LONG lLineNumber) {
    return ReadKeyed(&diror, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDirorLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirorLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&diror, lLineNumber, wLogLevel);
}


// ------------------------------------------------------------------------
// DELVSMRY File
// ------------------------------------------------------------------------

DELVSMRY_REC delvsmryrec;

void DelvsmrySet(void) {
    delvsmry.sessions   = 0;
    delvsmry.fnum       = -1L;
    delvsmry.pbFileName = "DELVSMRY";
    delvsmry.wOpenFlags = A_READ | A_SHARE;
    delvsmry.wReportNum = 784;
    delvsmry.pBuffer    = &delvsmryrec;
    delvsmry.wRecLth    = 48L;
}

URC open_delvsmry(void) {
    return direct_open(&delvsmry, TRUE);
}
URC close_delvsmry(WORD type) {
    return close_file(type, &delvsmry);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvsmry
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvsmry(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&delvsmry, lRecNum, lLineNum, LOG_CRITICAL); // RJN 28-06-2013 ELR
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvsmryLog
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvsmryLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel) {
    return ReadDirect(&delvsmry, lRecNum, lLineNum, wLogLevel);
}


// ------------------------------------------------------------------------
// DELVINDX File
// ------------------------------------------------------------------------

DELVINDX_REC delvindxrec;

void DelvindxSet(void) {
    delvindx.sessions   = 0;
    delvindx.fnum       = -1L;
    delvindx.pbFileName = "DELVINDX";
    delvindx.wOpenFlags = A_READ | A_SHARE;
    delvindx.wReportNum = 768;
    delvindx.pBuffer    = &delvindxrec;
    delvindx.wRecLth    = 48L;
}

URC open_delvindx(void) {
    return direct_open(&delvindx, TRUE);
}
URC close_delvindx(WORD type) {
    return close_file(type, &delvindx);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvindx
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvindx(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&delvindx, lRecNum, lLineNum, LOG_CRITICAL); // RJN 28-06-2013 ELR
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvindxLog
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvindxLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel) {
    return ReadDirect(&delvindx, lRecNum, lLineNum, wLogLevel);
}


// ------------------------------------------------------------------------
// DELVLIST File
// ------------------------------------------------------------------------

DELVLIST_REC delvlistrec;

void DelvlistSet(void) {
    delvlist.sessions   = 0;
    delvlist.fnum       = -1L;
    delvlist.pbFileName = "DELVLIST";
    delvlist.wOpenFlags = A_READ | A_SHARE;
    delvlist.wReportNum = 785;
    delvlist.pBuffer    = &delvlistrec;
    delvlist.wRecLth    = 18L;
    delvlist.wKeyLth    = 5L;
}

URC open_delvlist(void) {
    return keyed_open(&delvlist, TRUE);
}
URC close_delvlist(WORD type) {
    return close_file(type, &delvlist);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvlist
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvlist(LONG lLineNumber) {
    return ReadKeyed(&delvlist, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDelvlistLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDelvlistLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&delvlist, lLineNumber, wLogLevel);
}


// ------------------------------------------------------------------------
// DIRSU File
// ------------------------------------------------------------------------

DIRSU_REC dirsurec;

void DirsuSet(void) {
    dirsu.sessions   = 0;
    dirsu.fnum       = -1L;
    dirsu.pbFileName = "DIRSU";
    dirsu.wOpenFlags = A_READ | A_SHARE;
    dirsu.wReportNum = 230;
    dirsu.pBuffer    = &dirsurec;
    dirsu.wRecLth    = 40L;
    dirsu.wKeyLth    = 4L;
}

URC open_dirsu(void) {
    return keyed_open(&dirsu, TRUE);
}
URC close_dirsu(WORD type) {
    return close_file(type, &dirsu);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDirsu
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirsu(LONG lLineNumber) {
    return ReadKeyed(&dirsu, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDirsuLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirsuLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&dirsu, lLineNumber, wLogLevel);
}


// ------------------------------------------------------------------------
// UODOT File
// ------------------------------------------------------------------------

UODOT_REC uodotrec;

void UodotSet(void) {
    uodot.sessions   = 0;
    uodot.fnum       = -1L;
    uodot.pbFileName = "UODOT";
    uodot.wOpenFlags = A_READ | A_SHARE;
    uodot.wReportNum = 766;
    uodot.pBuffer    = &uodotrec;
    uodot.wRecLth    = 169L;
    uodot.wKeyLth    = 7L;
}

URC open_uodot(void) {
    return keyed_open(&uodot, TRUE);
}
URC close_uodot(WORD type) {
    return close_file(type, &uodot);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadUodot
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadUodot(LONG lLineNumber) {
    return ReadKeyed(&uodot, lLineNumber, LOG_CRITICAL); // RJN 28-06-2013 ELR
}
//////////////////////////////////////////////////////////////////////////////
///   ReadUodotLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadUodotLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&uodot, lLineNumber, wLogLevel);
}


// ------------------------------------------------------------------------
// UODIN File
// ------------------------------------------------------------------------

UODIN_REC uodinrec;

void UodinSet(void) {
    uodin.sessions   = 0;
    uodin.fnum       = -1L;
    uodin.pbFileName = "UODIN";
    uodin.wOpenFlags = A_READ | A_SHARE;
    uodin.wReportNum = 767;
    uodin.pBuffer    = &uodinrec;
    uodin.wRecLth    = 508L;
    uodin.wKeyLth    = 10L;
}

URC open_uodin(void) {
    return keyed_open(&uodin, TRUE);
}
URC close_uodin(WORD type) {
    return close_file(type, &uodin);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadUodin
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadUodin(LONG lLineNumber) {
    return ReadKeyed(&uodin, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadUodinLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadUodinLog(LONG lLineNumber, WORD wLogLevel) {
    return ReadKeyed(&uodin, lLineNumber, wLogLevel);
}

