/******************************************************************************
*
* File: PODLOG.C
* 
* POD Status Summary File Functions
*
* Version 1         Charles Skadorwa           15-Apr-2010
* Original version.
* POD Logging Enhancements 2010
*
******************************************************************************/

#include "transact.h"
#include "trxutil.h"
#include "podlog.h"
//#include "osfunc.h"
#include <string.h>

PODLOG_REC podlogrec;
FILE_CTRL podlog;

void PodlogSet(void) {
    podlog.sessions   = 0;
    podlog.fnum       = -1L;
    podlog.pbFileName = PODLOG;
    podlog.wOpenFlags = A_READ | A_WRITE | A_SHARE;
    podlog.wReportNum = PODLOG_REP;
    podlog.wRecLth    = sizeof(PODLOG_REC);
    podlog.wKeyLth    = 0;
    podlog.pBuffer    = &podlogrec;
}

URC PodlogOpen(void) {
    return direct_open(&podlog, TRUE);
}

URC PodlogClose(WORD type) {
    return close_file(type, &podlog);
}



// Initialise PODLOG record
void PodlogInitialiseRec(void) {

    BYTE sbuf[10]; 
    B_TIME nowTime;
    B_DATE nowDate;

    podlogrec.bSpace1 = ' ';
    podlogrec.bSpace2 = ' ';
    podlogrec.bSpace3 = ' '; 
    podlogrec.bSpace4 = ' ';
    podlogrec.bSpace5 = ' '; 
    podlogrec.bSpace6 = ' ';
    podlogrec.bSpace7 = ' ';
    podlogrec.bSpace8 = ' ';
    
    //podlogrec.abMACaddress   //Device MAC Address (Key)
    //podlogrec.abIPaddress    //IP Address ie. xxx.xxx.xxx.xxx

    //Get Controller date & time in string format
    GetSystemDate(&nowTime, &nowDate);

    sprintf(sbuf, "%02d/%02d/%02d",  //DD/MM/YY
                  nowDate.wDay,                      
                  nowDate.wMonth,                    
                  nowDate.wYear % 100);               
    memcpy(&podlogrec.abControllerDate, sbuf, 8 );  

    sprintf(sbuf, "%02d:%02d:%02d",  //HH:MM:SS
                  nowTime.wHour,                     
                  nowTime.wMin,                     
                  nowTime.wSec);
    memcpy(&podlogrec.abControllerTime, sbuf, 8 );  

    //memset(podlogrec.abAction,   0x20, sizeof(podlogrec.abAction));
    //memset(podlogrec.abFilename, 0x20, sizeof(podlogrec.abFilename));
    //memset(podlogrec.abStatus,   0x20, sizeof(podlogrec.abStatus));
    //memset(podlogrec.abReason,   0x20, sizeof(podlogrec.abReason));
    //memset(podlogrec.abText,     0x20, sizeof(podlogrec.abText));

    podlogrec.bCR = 0x0D;  //Carriage Return
    podlogrec.bLF = 0x0A;  //Line Feed

}

