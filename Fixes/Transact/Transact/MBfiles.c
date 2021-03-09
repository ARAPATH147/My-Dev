// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Microbroker Files Module
//
// Version 1.0               Brian Greenfield            15th November 2010
//     Initial Version
//
//
// Version 1.1               Charles Skadorwa            12th Mar 2012
// Stock File Accuracy  (commented: //CSk 12-03-2012 SFA)
//    DECCONF Reporting No. should be 815
//    RFSCACHE     "     "     "    " 816
//
// Version 1.2               Rejiya Nair                28th June 2013
// Event Log Rationalisation  (commented: // RJN 28-06-2013 ELR)
// Changes to remove the unnecessary application events logged from
// TRANSACT that are not supposed to be actually logged.
// ------------------------------------------------------------------------

#include "transact.h"

#include "osfunc.h"
#include "trxfile.h"
#include "MBfiles.h"

FILE_CTRL rfscache, decconf, ealsopts;

// ------------------------------------------------------------------------
// RFSCACHE File
// ------------------------------------------------------------------------

RFSCACHE_HOME rfscachehomerec;
RFSCACHE_SUB_REC rfscachesubrec;
RFSCACHE_REC rfscacherec;

void RFSCacheSet(void) {
    rfscache.sessions   = 0;
    rfscache.fnum       = -1L;
    rfscache.pbFileName = "RFSCACHE";
    rfscache.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    rfscache.wReportNum = 816;                                                 //CSk 12-03-2012 SFA
    rfscache.pBuffer    = &rfscacherec;
    rfscache.wRecLth    = sizeof(RFSCACHE_REC);
    rfscache.wKeyLth    = 5L ;
}

URC open_rfscache(void) {
    return keyed_open(&rfscache, TRUE);
}
URC close_rfscache(WORD type) {
    return close_file(type, &rfscache);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCacheHome
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCacheHome(LONG lLineNumber) {
    rfscache.pBuffer = &rfscachehomerec;
    return ReadKeyed(&rfscache, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCacheHomeLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCacheHomeLog(LONG lLineNumber, WORD wLogLevel) {
    rfscache.pBuffer = &rfscachehomerec;
    return ReadKeyed(&rfscache, lLineNumber, wLogLevel);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCacheSub
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCacheSub(LONG lLineNumber) {
    rfscache.pBuffer = &rfscachesubrec;
    return ReadKeyed(&rfscache, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCacheSubLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCacheSubLog(LONG lLineNumber, WORD wLogLevel) {
    rfscache.pBuffer = &rfscachesubrec;
    return ReadKeyed(&rfscache, lLineNumber, wLogLevel);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCache
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCache(LONG lLineNumber) {
    rfscache.pBuffer = &rfscacherec;
    return ReadKeyed(&rfscache, lLineNumber, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRFSCacheLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRFSCacheLog(LONG lLineNumber, WORD wLogLevel) {
    rfscache.pBuffer = &rfscacherec;
    return ReadKeyed(&rfscache, lLineNumber, wLogLevel);
}
LONG WriteRFSCacheHome(LONG lLineNumber) {
    rfscache.pBuffer = &rfscachehomerec;
    return WriteKeyed(&rfscache, lLineNumber, LOG_ALL);
}
LONG WriteRFSCache(LONG lLineNumber) {
    rfscache.pBuffer = &rfscacherec;
    return WriteKeyed(&rfscache, lLineNumber, LOG_ALL);
}


// ------------------------------------------------------------------------
// DECCONF File
// ------------------------------------------------------------------------

DECCONF_REC decconfrec;

void DecconfSet(void) {
    decconf.sessions   = 0;
    decconf.fnum       = -1L;
    decconf.pbFileName = "DECCONF";
    decconf.wOpenFlags = A_READ | A_SHARE;
    decconf.wReportNum = 815;                                                  //CSk 12-03-2012 SFA
    decconf.pBuffer    = &decconfrec;
    decconf.wRecLth    = sizeof(DECCONF_REC);
}

URC open_decconf(void) {
    return direct_open(&decconf, TRUE);
}
URC close_decconf(WORD type) {
    return close_file(type, &decconf);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDecconf
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDecconf(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&decconf, lRecNum, lLineNum, LOG_CRITICAL); // RJN 28-06-2013 ELR
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDecconfLog
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDecconfLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel) {
    return ReadDirect(&decconf, lRecNum, lLineNum, wLogLevel);
}


// ------------------------------------------------------------------------
// EALSOPTS File
// ------------------------------------------------------------------------

EALSOPTS_REC ealsoptsrec;

void EalsoptsSet(void) {
    ealsopts.sessions   = 0;
    ealsopts.fnum       = -1L;
    ealsopts.pbFileName = "EALSOPTS";
    ealsopts.wOpenFlags = A_READ | A_SHARE;
    ealsopts.wReportNum = 34;
    ealsopts.pBuffer    = &ealsoptsrec;
    ealsopts.wRecLth    = sizeof(EALSOPTS_REC);
}

URC open_ealsopts(void) {
    return direct_open(&ealsopts, TRUE);
}
URC close_ealsopts(WORD type) {
    return close_file(type, &ealsopts);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadEalsopts
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadEalsopts(LONG lRecNum, LONG lLineNum) {
    return ReadDirect(&ealsopts, lRecNum, lLineNum, LOG_ALL);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadEalsoptsLog
///   Read the direct file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadEalsoptsLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel) {
    return ReadDirect(&ealsopts, lRecNum, lLineNum, wLogLevel);
}

