/*************************************************************************
*
* File: PODOK.H
* 
* POD Combined Event Log File Header
*
* Version 1         Charles Skadorwa           15-Apr-2010
* Original version.
* POD Logging Enhancements 2010
*
******************************************************************************/

#ifndef PODOK_H
#define PODOK_H


#define PODOK               "PODOK"
#define PODOK_REP           799
#define PODOK_RECL          508

#define PODBKP              "ADXLXACN::D:\\ADX_UDT1\\PODBKP.BIN"   //CSk 03-08-2010 Defect 4522


typedef struct {
    BYTE abDeviceDate[4];   //MMDD
    BYTE abDeviceTime[4];   //HHMM
    BYTE bFileStatus;       //S=Start,  E=End/OK,  X=Abend
} PODOK_SEG;


typedef struct {
    BYTE abMACaddress[12];      //Device MAC Address (Key)
    BYTE abIPaddress[15];       //IP Address ie. xxx.xxx.xxx.xxx
    BYTE abControllerDate[6];   //YYMMDD
    BYTE abControllerTime[6];   //HHMMSS
    PODOK_SEG aSeg[MAX_POD_FILES]; //File Load Progress
                                // 00 = SYNCTRL
                                // 01 = BOOTCODE.CSV    (Reference)
                                // 02 = BARCODE.CSV     (Reference)
                                // 03 = DEAL.CSV        (Reference)
                                // 04 = PGROUP.CSV      (Reference)
                                // 05 = SUPPLIER.CSV    (Reference)
                                // 06 = USERS.CSV       (Reference)
                                // 07 = RECALL.CSV      (Reference)
                                // 08 = LIVEPOG.CSV     (Reference)
                                // 09 = CATEGORY.CSV    (Reference)
                                // 10 = MODULE.CSV      (Reference)
                                // 11 = CONTROL.CSV     (Active)
                                // 12 = PICKING.CSV     (Active)
                                // 13 = COUNT.CSV       (Active)
                                // 14 = CREDIT.CSV      (Active)
                                // 15 = DIRECTS.CSV     (Active)
                                // 16 = ASN.CSV         (Active)
                                // 17 = SSCUODOT.CSV    (Active)
                                //  . .    .      .     (Future Use)
                                //  . .    .      .         .
                                //  . .    .      .         .
                                // 51 = POD Log Files 
    BYTE abFiller;
} PODOK_REC;

extern PODOK_REC podokrec;
extern FILE_CTRL podok;

void PodokSet(void);
URC PodokOpen(void);
URC PodokClose(WORD type);
LONG PodokRead(LONG lLineNum);
LONG PodokWrite(LONG lLineNum);
void PodokInitialiseRec(BOOLEAN DateTimeOnly);

#endif


