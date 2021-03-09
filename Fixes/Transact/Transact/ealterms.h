#ifndef EALTERMS_H
#define EALTERMS_H

#include "trxfile.h"

// EALTERMS record layout (store record)
typedef struct {
    BYTE abTill[2];     //Packed till number
    BYTE bIndicat0;     //0=Store open, -1=Store close
    BYTE bIndicat1;     //0x0F=Store close requested, 0xFF=Unprocessed close record in TSL, 0x00=Close record processed
    BYTE abTSLName[8];  //Name of most recent TSLOG close
    BYTE abMonitor;     //Number of last monitoring terminal (9999 if controller)
    BYTE bTLOGFlag;     //'0' or ' '=EALLGHC not processed, '1'=EALLGHC processed
    BYTE bRCPStatus;    //'Y'=RCP initiated store close
    BYTE abReserved[47];//Filler
    } EALTERMS_REC;

extern FILE_CTRL ealterms;
extern EALTERMS_REC ealtermsrec;

void EaltermsSet(void);
URC EaltermsOpen(void);
URC EaltermsClose(WORD type);
LONG EaltermsRead(LONG lLineNum);

#endif
