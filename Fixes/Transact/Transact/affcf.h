/*************************************************************************
*
* File: AFFCF.H
* 
* Auto Fast Fill Control File Header
*
* Version 1         Stuart Highley           20-May-2009
* Original version.
*
******************************************************************************/

#ifndef AFFCF_H
#define AFFCF_H

#include "trxfile.h"

typedef struct {
    BYTE abStartTime[4];        //HHMM or OOOO=At store Open, MMMM=Manual release, XXXX=Not active today
    BYTE abEndTime[4];          //HHMM or CCCC=At store close, XXXX=Not active today
} AFFCF_DAY;

typedef struct {
    BYTE abMDQOverride[3];      //If 0 then MDQ, otherwise %age of PSC
    BYTE abRunInterval[3];      //Run PSS10 every n minutes
    AFFCF_DAY aDay[7];          //Element 0 = Monday, 6 = Sunday
    BYTE abLastRunStart[10];    //YYMMDDHHmm, updated by PSS10            
    BYTE abLastRunEnd[10];      //YYMMDDHHmm, updated by PSS10
    BYTE bSingleList;           //Y=Single list, N=One list per business centre
    BYTE bLastRunStatus;        //X=Critical error, E=Successful, S=Started
    BYTE abAFFITFSize[3];       //Size in KB to create AFFITF
    BYTE abPickSessions[3];     //Number of pick sessions today
    BYTE abPickCompleted[3];    //Number of picks completed today
    BYTE abFiller[7];
} AFFCF_REC;

extern AFFCF_REC affcfrec;
extern FILE_CTRL affcf;

void AffcfSet(void);
URC AffcfOpen(void);
URC AffcfClose(WORD type);
LONG AffcfRead(LONG lLineNum);
void AffcfProcess(void);
BOOLEAN AffcfRunRequired(void);
void AffcfRunNow(void);

#endif
