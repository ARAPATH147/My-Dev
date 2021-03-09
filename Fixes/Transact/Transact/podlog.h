
/*************************************************************************
*
* File: PODLOG.H
* 
* POD Status Summary File Header
*
* Version 1         Charles Skadorwa           15-Apr-2010
* Original version.
* POD Logging Enhancements 2010
*
******************************************************************************/

#ifndef PODLOG_H
#define PODLOG_H

//#include "trxfile.h"

#define PODLOG              "PODLOG"
#define PODLOG_REP          800
#define PODLOG_RECL         101
#define PODLOG_OFLAGS       0x2014
#define PODLOG_CFLAGS       0x2014

#define PODL1               "ADXLXACN::D:\\ADX_UDT1\\PODL1.BIN"
#define PODL2               "ADXLXACN::D:\\ADX_UDT1\\PODL2.BIN"
#define PODL3               "ADXLXACN::D:\\ADX_UDT1\\PODL3.BIN"
#define PODL4               "ADXLXACN::D:\\ADX_UDT1\\PODL4.BIN"
#define PODL5               "ADXLXACN::D:\\ADX_UDT1\\PODL5.BIN"
#define PODL6               "ADXLXACN::D:\\ADX_UDT1\\PODL6.BIN"

typedef struct {
    BYTE abMACaddress[12];    //Device MAC Address
    BYTE bSpace1;
    BYTE abIPaddress[15];     //IP Address ie. xxx.xxx.xxx.xxx
    BYTE bSpace2;
    BYTE abControllerDate[8]; //DD/MM/YY (converted from received YYMMDD)
    BYTE bSpace3;
    BYTE abControllerTime[8]; //HH:MM:SS (converted from received HHMMSS)
    BYTE bSpace4;
    BYTE abAction[9];         //L="TFTP Load"  S="TFTP Send"  F=" FTP Load"  P=" FTP Send"
    BYTE bSpace5;
    BYTE abFilename[12];      //written as aaaaaaaa.bbb - Left justified, space filled
    BYTE bSpace6;
    BYTE abStatus[5];         //S = "Start",    E = "OK   ",    X = "Abend"
    BYTE bSpace7;
    BYTE abReason[2];         //Reason code i.e. 00, 01, 02, etc.
    BYTE bSpace8;
    BYTE abText[20];          //"OK", "Fail - Time Out", etc - Left justified, space filled
    BYTE bCR;                 // 0x0D
    BYTE bLF;                 // 0x0A
} PODLOG_REC;

extern PODLOG_REC podlogrec;
extern FILE_CTRL podlog;

void PodlogSet(void);
URC PodlogOpen(void);
URC PodlogClose(WORD type);
void PodlogInitialiseRec(void);

#endif
