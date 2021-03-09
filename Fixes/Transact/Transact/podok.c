/******************************************************************************
*
* File: PODOK.C
* 
* POD Combined Event Log File Functions
*
* Version 1         Charles Skadorwa           15-Apr-2010
* Original version.
* POD Logging Enhancements 2010
*
******************************************************************************/
   
#include "transact.h"
#include <string.h>
#include "trxfile.h"
#include "trxutil.h"

#include "podok.h"
 
PODOK_REC podokrec;
FILE_CTRL podok;
   
void PodokSet(void) {
    podok.sessions   = 0;
    podok.fnum       = -1L;
    podok.pbFileName = PODOK;
    podok.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    podok.wReportNum = PODOK_REP;
    podok.wRecLth    = sizeof(PODOK_REC);
    podok.wKeyLth    = 12;
    podok.pBuffer    = &podokrec;
}


URC PodokOpen(void) {
    return keyed_open(&podok, TRUE);
}

URC PodokClose(WORD type) {
    return close_file(type, &podok);
}

LONG PodokRead(LONG lLineNum) {
    return ReadKeyed(&podok, lLineNum, LOG_CRITICAL);
}

LONG PodokWrite(LONG lLineNum) {                  
    return WriteKeyed(&podok, lLineNum, LOG_ALL); 
}                                                    

// Initialise PODOK record
void PodokInitialiseRec(BOOLEAN DateTimeOnly) {

    WORD i;
    BYTE sbuf[10]; 
    B_TIME nowTime;
    B_DATE nowDate;


    //podok.abMACaddress   //Device MAC Address (Key)
    //podok.abIPaddress    //IP Address ie. xxx.xxx.xxx.xxx
    //Get Controller date & time in string format
    GetSystemDate(&nowTime, &nowDate);

    sprintf(sbuf, "%02d%02d%02d",        //YYMMDD
                  nowDate.wYear % 100,               
                  nowDate.wMonth,                    
                  nowDate.wDay);
    memcpy(&podokrec.abControllerDate, sbuf, 6 );  
    

    sprintf(sbuf, "%02d%02d%02d",        //HHMMSS
                  nowTime.wHour,                     
                  nowTime.wMin,                     
                  nowTime.wSec);
     memcpy(&podokrec.abControllerTime, sbuf, 6 );

    if (!DateTimeOnly) {
        for (i = 0; i < MAX_POD_FILES; i++) {
            memset(podokrec.aSeg[i].abDeviceDate, 0x30, sizeof(podokrec.aSeg[i].abDeviceDate));
            memset(podokrec.aSeg[i].abDeviceTime, 0x30, sizeof(podokrec.aSeg[i].abDeviceTime));
            podokrec.aSeg[i].bFileStatus = 0x20;
        }
        podokrec.abFiller = 0x20;
    }
}

