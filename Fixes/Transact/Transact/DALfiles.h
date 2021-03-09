//*************************************************************************
//
// File    : DALFILES.H
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
//************************************************************************/

#ifndef DALFILES_H
#define DALFILES_H

#include "trxfile.h"


// ------------------------------------------------------------------------
// WHUOD File
// ------------------------------------------------------------------------

typedef struct
{
    BYTE abWhuodKey[4];     // WHUOD file key
    BYTE abPoNos[45];       // PO Numbers
    BYTE abRcrdTotItems[4]; // Total number of items
    BYTE abExpctDelDate[6]; // Expected Date of Delivery
    BYTE abScannedDate[6];  // Scanned UOD date
    BYTE bStatus;           // UOD Status
} WHUOD_REC;

extern FILE_CTRL whuod;
extern WHUOD_REC whuodrec;

void WhuodSet(void);

URC OpenWhuod(void);

URC CloseWhuod(WORD type);

LONG ReadWhuod(LONG lLineNumber);

LONG WriteWhuod(LONG lLineNumber);


// ------------------------------------------------------------------------
// WHINDX File
// ------------------------------------------------------------------------

typedef struct
{
    BYTE abBarcodeNo[14];   // Barcode number
    BYTE abExpctDelDate[6]; // Expected Date of Delivery
    BYTE bStatus;           // UOD Status
} WHINDX_REC;

extern FILE_CTRL whindx;
extern WHINDX_REC whindxrec;

void WhindxSet(void);

URC OpenWhindx(void);

URC CloseWhindx(WORD type);

LONG ReadWhindx(LONG lRecNum, LONG lLineNum);

LONG WriteWhindx(LONG lRecNum, LONG lLineNum);

// ------------------------------------------------------------------------
// INVCE File
// ------------------------------------------------------------------------


typedef struct
{
    BYTE  abBootsCode[4];             // Boots Code
    BYTE  abPriceCode[4];             // Price Code
    UWORD QtyExpected;                // Quantity Expected
    UWORD QtyReceived;                // Quantity Received
    BYTE  abCsrMarker[1];             // CSR Marker
    BYTE  abFiller[15];               // Filler
} DETAIL_ITEM;

typedef struct
{
    BYTE  abRecKey[9];                // INVCE Key
    BYTE  abFolioYear[1];             // Folio Year
    BYTE  abDate[3];                  // Invoice Date
    BYTE  bConfirmFlag;               // Invoice Confirm Flag
    BYTE  abConfirmDate[3];           // Invoice Confirm Date
    UWORD WhseArea;                   // Invoice Warehouse Area
    BYTE  bInsystFlag;                // Stock System Item Flag
    BYTE  abCount[1];                 // Invoice Item Entry Count
    BYTE  bType;                      // Invoice Type
    BYTE  bDallasMkr;                 // Invoice Dallas Marker
    BYTE  abExpDelDate[3];            // Expected Delivery date
    BYTE  abSupplierNum[3];           // Invoice Ssupplier Number
    BYTE  bOrderSuffix;               // Invoice Order Suffix
    BYTE  bFiller;                    // Invoice Filler
    BYTE  bReceiptStatus;             // Invoice Receipt Status
    DETAIL_ITEM abDetailItem[17];     // Item Details
} INVCE_REC;

extern FILE_CTRL invce;
extern INVCE_REC invcerec;

void InvceSet(void);

URC OpenInvce(void);

URC CloseInvce(WORD type);

LONG ReadInvce(LONG lLineNumber);

LONG WriteInvce(LONG lLineNumber);

#endif

