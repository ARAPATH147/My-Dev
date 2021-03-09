/******************************************************************************
*
* File: AFFCF.C
* 
* Auto Fast Fill Control File Functions
*
* Version 1         Stuart Highley           20-May-2009
* Original version.
*
******************************************************************************/

#include "transact.h"
#include "trxutil.h"
#include "affcf.h"
#include "ealterms.h"
#include "osfunc.h"
#include <string.h>

AFFCF_REC affcfrec;
FILE_CTRL affcf;
WORD      wNextRunTime; //Minutes past midnight

void AffcfSet(void) {
    affcf.sessions   = 0;
    affcf.fnum       = -1L;
    affcf.pbFileName = "AFFCF";
    affcf.wOpenFlags = A_READ | A_SHARE;
    affcf.wReportNum = 795;
    affcf.wRecLth    = sizeof(AFFCF_REC);
    affcf.wKeyLth    = 0;
    affcf.pBuffer    = &affcfrec;
}

URC AffcfOpen(void) {
    return direct_open(&affcf, TRUE);
}

URC AffcfClose(WORD type) {
    return close_file(type, &affcf);
}

LONG AffcfRead(LONG lLineNum) {
    return ReadDirect(&affcf, 0, lLineNum, LOG_CRITICAL);
}

//Processes the AFFCF file to calculate the next run time. Only calculates run
//times for the current day, and so relies on being called every morning
void AffcfProcess(void) {
    
    LONG lRc;
    B_TIME nowTime;
    B_DATE nowDate;
    BYTE abYYMMDDHHmm[10];
    
    //Initialise next time to run to manual
    wNextRunTime = 9999;
    
    //Read the AFFCF; get out now if error
    AffcfOpen();
    lRc = AffcfRead(__LINE__);
    AffcfClose(CL_ALL);
    if (lRc <= 0) {
        disp_msg("AffcfProcess failed to read AFFCF");
        return;
    }

    //Get date, time, current day of week and date time in string format
    GetSystemDate(&nowTime, &nowDate);
    GetYYMMDDHHmm(abYYMMDDHHmm, &nowTime, &nowDate);
    
    //Adjust from Sunday = 0 to Monday = 0
    nowDate.wDOW = (nowDate.wDOW + 6) % 7;
    
    //Cap end time at 8pm
    if (MEMCMP(affcfrec.aDay[nowDate.wDOW].abEndTime, "2000") > 0) {
        MEMCPY(affcfrec.aDay[nowDate.wDOW].abEndTime, "2000");
    }
    
    //Get run interval as integer, get out now if run interval is zero
    WORD wInterval = satoi(affcfrec.abRunInterval, sizeof(affcfrec.abRunInterval));
    if (wInterval == 0) {
        disp_msg("AffcfProcess detected invalid PSS10 run interval. Will not run.");
        return;
    }

    //If already run today and not XXXX then
    //calculate next run as last run plus interval
    if ((memcmp(affcfrec.abLastRunStart, abYYMMDDHHmm, 6) == 0) &&
        (MEMCMP(affcfrec.aDay[nowDate.wDOW].abStartTime, "XXXX") != 0)) {
        wNextRunTime = satoi(affcfrec.abLastRunStart + 6, 2) * 60 +
                       satoi(affcfrec.abLastRunStart + 8, 2);
        wNextRunTime = wNextRunTime + wInterval;
        //If answer has passed end time then set to 9999 so never runs
        if (wNextRunTime > (satoi(affcfrec.aDay[nowDate.wDOW].abEndTime, 2) * 60 +
                            satoi(affcfrec.aDay[nowDate.wDOW].abEndTime + 2, 2))) {
            wNextRunTime = 9999;
        }
        return;
    }
    
    //If we are here then we have not run today yet
    //If auto start set start time to 6.30am
    //so that we keep trying to start until store opens
    if (MEMCMP(affcfrec.aDay[nowDate.wDOW].abStartTime, "OOOO") == 0) {
        wNextRunTime = (6 * 60) + 30;
    } else if ((MEMCMP(affcfrec.aDay[nowDate.wDOW].abStartTime, "MMMM") != 0) &&
               (MEMCMP(affcfrec.aDay[nowDate.wDOW].abStartTime, "XXXX") != 0)) {
        wNextRunTime = satoi(affcfrec.aDay[nowDate.wDOW].abStartTime, 2) * 60 +
                       satoi(affcfrec.aDay[nowDate.wDOW].abStartTime + 2, 2);
    }
   
}

BOOLEAN AffcfRunRequired(void) {
    
    B_TIME nowTime;
    B_DATE nowDate;
    static WORD wLastDate = 0; 
    
    //Adjust from Sunday = 0 to Monday = 0
    GetSystemDate(&nowTime, &nowDate);
    nowDate.wDOW = (nowDate.wDOW + 6) % 7;

    //Call AffcfProcess if this is the first call of the day
    if (nowDate.wDay != wLastDate) AffcfProcess();
    wLastDate = nowDate.wDay; 
    
    //Determine whether a run is due
    if (nowTime.wHour * 60 + nowTime.wMin >= wNextRunTime) return TRUE;
    return FALSE;
    
}


void AffcfRunNow(void) {
    
    B_TIME nowTime;
    B_DATE nowDate;
    LONG   lRc;
   
    //Get date and time
    //Adjust from Sunday = 0 to Monday = 0
    GetSystemDate(&nowTime, &nowDate);
    nowDate.wDOW = (nowDate.wDOW + 6) % 7;

    //Never run after 8pm
    if (nowTime.wHour >= 20) return;
    
    //Never run before 6.30am
    if (nowTime.wHour < 6) return;
    if (nowTime.wHour == 6 && nowTime.wMin < 30) return;

    //Check whether store open by reading store record on EALTERMS
    EaltermsOpen();
    MEMSET(ealtermsrec.abTill, 0x99); 
    lRc = EaltermsRead(__LINE__);
    EaltermsClose(CL_ALL);
    if (lRc <= 0) {
        disp_msg("AffcfRunNow failed to read EALTERMS. Will not run PSS10.");
        return;
    }
    if (ealtermsrec.bIndicat0 != 0) return;
    
    disp_msg("Starting PSS10");                                  
    start_background_app("ADX_UPGM:PSS10.286", "",
                         "TRANSACT is attempting to start PSS10");

    //Calculate next run time
    if (MEMCMP(affcfrec.aDay[nowDate.wDOW].abStartTime, "XXXX") != 0) {
        WORD wInterval = satoi(affcfrec.abRunInterval, sizeof(affcfrec.abRunInterval));
        if (wInterval == 0) {
            wNextRunTime = 9999;
        } else {
            wNextRunTime = nowTime.wHour * 60 + nowTime.wMin + wInterval;
            //If answer has passed end time then
            //set it to 9999 so we never run again today
            if (wNextRunTime > (satoi(affcfrec.aDay[nowDate.wDOW].abEndTime, 2) * 60 +
                                satoi(affcfrec.aDay[nowDate.wDOW].abEndTime + 2, 2))) {
                wNextRunTime = 9999;
            }
        }
    }
    
}
