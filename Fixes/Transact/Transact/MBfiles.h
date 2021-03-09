// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Microbroker Files Header
//
// Version 1.0               Brian Greenfield            15th November 2010
//     Initial Version
//
// ------------------------------------------------------------------------
#ifndef MBFILES_H
#define MBFILES_H

#include "trxfile.h"

// ------------------------------------------------------------------------
// RFSCACHE File
// ------------------------------------------------------------------------

typedef struct {
    BYTE  abkey[5];         // (UPD) FFFFFFFFFF
    ULONG ulNextMessNum;    // Next message Number
    BYTE  abFiller[499];
} RFSCACHE_HOME;

typedef struct {
    ULONG ulID;              // Unique ID - Part of key - runs from 1 to 3999999999
    BYTE  bSeq;              // Sequence number 0
    BYTE  bStatus;           // Status = S = Message Sent, R = Message Received
    BYTE  abSentDateTime[6]; // (UPD) YYMMDDHHMMSS
    BYTE  abMessage[496];    // Message
} RFSCACHE_REC;                                                  

typedef struct {
    ULONG ulID;              // Unique ID - Part of key - runs from 1 to 3999999999
    BYTE  bSeq;              // Sequence number - from 1 to 255
    BYTE  abMessage[503];    // Message
} RFSCACHE_SUB_REC;                                                  

extern FILE_CTRL rfscache;
extern RFSCACHE_HOME rfscachehomerec;
extern RFSCACHE_SUB_REC rfscachesubrec;
extern RFSCACHE_REC rfscacherec;

void RFSCacheSet(void);
URC open_rfscache( void );
URC close_rfscache( WORD type );
LONG ReadRFSCacheHome(LONG lLineNum);
LONG ReadRFSCacheHomeLog(LONG lLineNumber, WORD wLogLevel);
LONG ReadRFSCacheSub(LONG lLineNum);
LONG ReadRFSCacheSubLog(LONG lLineNumber, WORD wLogLevel);
LONG ReadRFSCache(LONG lLineNum);
LONG ReadRFSCacheLog(LONG lLineNumber, WORD wLogLevel);
LONG WriteRFSCacheHome(LONG lLineNumber);
LONG WriteRFSCache(LONG lLineNumber);


// ------------------------------------------------------------------------
//DECCONF File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abMessageID[20];   // Message ID
    BYTE abMessageName[30]; // Message Name
    BYTE bMessDirection;    // Message direction - O = Outbound, I = Inbound
    BYTE bDeliveryType;     // Deliver Type - Q = Queue, T = Topic
    BYTE bQualOfServ;       // Quality of Service - 0, 1, 2, etc.
    BYTE bWritetype;        // Write Type - Q = Queue, S = RealTime socket
    BYTE abFiller[2];       // Filler - 0D0Ah
} DECCONF_REC;                                                  

extern FILE_CTRL decconf;
extern DECCONF_REC decconfrec;

void DecconfSet(void);
URC open_decconf( void );
URC close_decconf( WORD type );
LONG ReadDecconf(LONG lRecNum, LONG lLineNum);
LONG ReadDecconfLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel);

//DECCONF table structure
typedef struct {
    BYTE abMessageID[20];   // Message ID
    BYTE abMessageName[30]; // message Name
    BYTE bWritetype;        // Write Type - Q = Queue, S = RealTime
} DECCONF;


// ------------------------------------------------------------------------
// EALSOPTS File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abRecord[102];     // EALSOPTS record
} EALSOPTS_REC;

extern FILE_CTRL ealsopts;
extern EALSOPTS_REC ealsoptsrec;

void EalsoptsSet(void);
URC open_ealsopts( void );
URC close_ealsopts( WORD type );
LONG ReadEalsopts(LONG lRecNum, LONG lLineNum);
LONG ReadEalsoptsLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel);

// ------------------------------------------------------------------------
// DQ:
// ------------------------------------------------------------------------

#define DQ_REP 812

#endif

