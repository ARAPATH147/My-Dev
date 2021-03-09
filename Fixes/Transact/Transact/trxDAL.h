//*************************************************************************
//
// File    : TRXDAL.H
// Author  : Visakha Satya
// Created : 20th April 2015
//
// Overview: Header file for TRXDAL module.
//
//-------------------------------------------------------------------------
// Version A: Visakha Satya                                 20th Apr 2015
// SC079 Dallas Positive Receiving
//            Initial version
//
// Version B: Charles Skadorwa //BCS                        12th Jun 2015
// SC079 Dallas Positive Receiving
//    The following to be included as part of Defects 1496 & 1497:
//       - Changed AMENDED constant from 'C' to 'A'.
//
//************************************************************************/
#ifndef TRXDAL_H
#define TRXDAL_H

//Status
#define AMENDED      'A'                                                //BCS
#define BANKED       'B'
#define RECEIPTED    'R'
#define UNRECEIPTED  'U'

//Messages
#define DAC_MSG      "DAC"
#define DAD_MSG      "DAD"
#define DAE_MSG      "DAE"
#define DAL_MSG      "DAL"
#define DAR_MSG      "DAR"

typedef struct
{
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE abNextRecordNo[4];   //The last returned record number from DAD.
} LRT_DAL;
#define LRT_DAL_LTH sizeof(LRT_DAL)

typedef struct
{
    BYTE abCmd[3];
    BYTE abNextRecordNo[4];   // Record number obtained from DAL + 1
    BYTE abDallasBarcode[14]; // Dallas UOD Barcode data
    BYTE abExpectedDelDate[6];// Expected delivery date of Dallas UOD - YYMMDD
    BYTE bStatus;             // R - Receipted, B - Banked, U - Un-receipted
} LRT_DAD;
#define LRT_DAD_LTH sizeof(LRT_DAD)

typedef struct
{
    BYTE abCmd[3];
} LRT_DAE;
#define LRT_DAE_LTH sizeof(LRT_DAE)

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE abDallasBarcode[14]; // Dallas UOD Barcode number
    BYTE abUodScanDate[6];    // Dallas UOD scan date
    BYTE bUodScanStatus;      // R - Receipted, B - Banked

} LRT_DAR;
#define LRT_DAR_LTH sizeof(LRT_DAR)

void DAC_Request(void);

void DAL_Request(char *inbound);

void DAR_Request(char *inbound);

#endif
