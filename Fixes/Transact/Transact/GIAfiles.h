// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Goods In Application Files Header
//
// Version 1.0               Brian Greenfield            10th October 2008
//     Initial Version
//
// Version A                 Visakha Satya                 13th May 2015
// SC079 Dallas Positive Receiving
// - Incorporating HUMSS UOD Messaging Project changes as it was missed
///  out during HUMSS project.
// - Added UODOT number field for UODOT file layout changes.
// ------------------------------------------------------------------------
#ifndef GIAFILES_H
#define GIAFILES_H

#include "trxfile.h"

// ------------------------------------------------------------------------
// CARTON File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abItemCode[3];     // (UPD) Item code without check digit
    UWORD uwDesptchQty;     // Despatched quantity
    UWORD uwBookedInQty;    // Booked in quantity
} CARTON_ITEMS;

typedef struct {
    BYTE abSupplier[3];     // (UPD) Supplier Name - Part of key
    BYTE abCarton[4];       // (UPD) Carton Number - Part of key
    BYTE bChain;            // (INT) Chain - starting at 0 - Part of key
    BYTE bStatus;           // Status - U = Unbooked, N = normally booked, A = Booked via audit, E = Booked via exception, P = Previously booked in
    BYTE abASNCode[18];     // ASN code
    BYTE abOrderNum[5];     // Order number
    BYTE bOrderSuffix;      // Order suffix
    BYTE bBusCentre;        // Business centre letter
    BYTE abExpDeliv[12];    // Expected delivery date and time - CCYYMMDDHHMM
    BYTE abItemCount[3];    // Item count
    CARTON_ITEMS CartonItems[60];
    BYTE filler[40];
} CARTON_REC;


extern FILE_CTRL carton;
extern CARTON_REC cartonrec;

void CartonSet(void);
URC open_carton( void );
URC close_carton( WORD type );
LONG ReadCarton(LONG lLineNum);
LONG ReadCartonLog(LONG lLineNumber, WORD wLogLevel);


// ------------------------------------------------------------------------
// DIROR File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abNoOrderItems[2];     // (UPD) Total items in order
    BYTE abNoOrderSing[2];      // (UPD) Total single quantity in order
    BYTE abNoItemsBooked[2];    // (UPD) Total number of items booked in
    BYTE abNoItemsLastBooked[2];// (UPD) Number of items booked in during last date of update
    BYTE abOrderDate[3];        // (UPD) Date of order
    BYTE abExpDelivDate[3];     // (UPD) Expected date of delivery
    BYTE bConfirmFlag;          // C = Complete, A = Amended, " " = Not confirmed
    BYTE abConfirmDate[3];      // (UPD) Confirm date
    BYTE abConfirmStartTime[2]; // (UPD) Start time of last update
    BYTE abConfirmEndTime[2];   // (UPD) End time of last update
    BYTE abOnSaleDate[3];       // (UPD) Date stock should be on sale
    BYTE bSuperceded;           // Superceded flag for ASN carton support
    BYTE abFiller[473];
} DIROR_HEADER_REC;

typedef struct {
    BYTE abBootsCode[4];            // (UPD) Boots code
    BYTE abBarcode[6];              // (UPD) Barcode
    BYTE abPrice[4];                // (UPD) Price
    WORD wQtyExpected;              // Item Quantity
    WORD wQtyBookedGoodCond;        // Quantity booked in in good condition
    WORD wQtyBookedDamaged;         // Quantity booked in in damaged condition
    WORD wQtyBookedStolen;          // Quantity booked in stolen
    WORD wQtyLastBookedGoodCond;    // Quantity last booked in in good condition
    WORD wQtyLastBookedDamaged;     // Quantity last booked in in damaged condition
    WORD wQtyLastBookedStolen;      // Quantity last booked in stolen
    BYTE abFiller[2];
} DIROR_ITEM;

typedef struct {
    BYTE abItemCount[1];        // (UPD) Item count
    BYTE abConfirmDate[3];      // (UPD) Confirm date
    BYTE abFiller[15];
    DIROR_ITEM DirorItem[16];
} DIROR_DETAIL_REC;

typedef union {
    DIROR_HEADER_REC DirorHeader;
    DIROR_DETAIL_REC DirorDetailRec;
} DIROR_REST;

typedef struct {
    BYTE abSupplier[3];// (UPD) Supplier number - Part of key
    BYTE abOrder[2];   // (UPD) Order number - Part of key
    BYTE bOrderSuf;    // Order suffix - Part of key
    BYTE bBC;          // Business Centre - Part of key
    BYTE bSource;      // Source - Part of key - "D" = PSS37, " " = Head Office
    BYTE abPageNo[1];  // Page number - Part of key - Packed "00" for header
    DIROR_REST DirorRest;
} DIROR_REC;

extern FILE_CTRL diror;
extern DIROR_REC dirorrec;

void DirorSet(void);
URC open_diror( void );
URC close_diror( WORD type );
LONG ReadDiror(LONG lLineNum);
LONG ReadDirorLog(LONG lLineNumber, WORD wLogLevel);


// ------------------------------------------------------------------------
// DELVSMRY File
// ------------------------------------------------------------------------

typedef struct {
    BYTE bFiller;           // The " at the start of the record
    BYTE abSupplier[6];     // Supplier number
    BYTE abSuppName[20];    // Supplier name
    BYTE abTotOrders[5];    // Total number of orders
    BYTE abUnbOrders[5];    // Number of expected unbooked orders
    BYTE bASNSupp;          // Supplier ASN flag - 'A' = ASN, space = legacy directs
    BYTE bStaticSupp;       // 'S' = static supplier, space = normal
    BYTE abFiller[9];       // Including the end " and CR/LF
} DELVSMRY_REC;

extern FILE_CTRL delvsmry;
extern DELVSMRY_REC delvsmryrec;

void DelvsmrySet(void);
URC open_delvsmry( void );
URC close_delvsmry( WORD type );
LONG ReadDelvsmry(LONG lRecNum, LONG lLineNum);
LONG ReadDelvsmryLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel);


// ------------------------------------------------------------------------
// DELVINDX File
// ------------------------------------------------------------------------

typedef struct {
    BYTE bFiller;           // The " at the start of the record
    BYTE abLicence[10];     // Licence plate
    BYTE abSeq[5];          // UODOT start of chain sequence number
    BYTE abDespDate[6];     // Despatch date YYMMDD
    BYTE abImmParent[10];   // immediate parent licence plate
    BYTE bUODType;          // UOD type
    BYTE abExpDelivDate[6]; // Expedted delivery date YYMMDD
    BYTE bBOLFlag;          // 'B' = BOL, ' ' = no BOL
    BYTE abFiller[8];       // Including the end " and CR/LF
} DELVINDX_REC;

extern FILE_CTRL delvindx;
extern DELVINDX_REC delvindxrec;

void DelvindxSet(void);
URC open_delvindx( void );
URC close_delvindx( WORD type );
LONG ReadDelvindx(LONG lRecNum, LONG lLineNum);
LONG ReadDelvindxLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel);


// ------------------------------------------------------------------------
// DELVLIST File
// ------------------------------------------------------------------------

// NOTE: when referencing these fields the caller needs to know if it is reading ASn or directs data.
// Only the one carton field will be used as the  key to the CARTON file itself.
// For directs, the 4 directs fileds form part of the key for the DIROR file (along with a page number byte starting at 0.)
typedef struct {
    BYTE abSupplier[3];     // (UPD) Supplirt number - part of key
    WORD wSeq;              // Sequence number starting at 0 - part of key
    BYTE abCarton[4];       // (UPD) Carton number - used for ASn cartons - NOT used for directs
    BYTE abDirOrdNum[2];    // (UPD) Directs order number - used for directs - NOT used for ASN's
    BYTE bDirOrdSuf;        // Directs order suffix - used for directs - NOT used for ASN's
    BYTE bDirBC;            // Directs Business Centre letter - used for directs - NOT used for ASN's
    BYTE bDirSource;        // Directs source 'D' form PSS57, ' ' from head office - used for directs - NOT used for ASN's
    BYTE abFiller[4];
} DELVLIST_REC;

extern FILE_CTRL delvlist;
extern DELVLIST_REC delvlistrec;

void DelvlistSet(void);
URC open_delvlist( void );
URC close_delvlist( WORD type );
LONG ReadDelvlist(LONG lLineNum);
LONG ReadDelvListLog(LONG lLineNumber, WORD wLogLevel);


// ------------------------------------------------------------------------
// DIRSU File
// ------------------------------------------------------------------------

typedef struct {
    BYTE bBC;                 // BC Letter - part of key
    BYTE abSupplier[3];       // (UPD) Supplier number - part of key
    BYTE abSuppName[10];      // Supplier name
    BYTE abLeadTimeMon[2];
    BYTE abLeadTimeTue[2];
    BYTE abLeadTimeTed[2];
    BYTE abLeadTimeThu[2];
    BYTE abLeadTimeFri[2];
    BYTE abLapsingDays[2];
    BYTE bPartOrderRules;
    BYTE abMaxChkQty[2];
    BYTE abCheckQty[2];
    BYTE abDiscrepQty[2];
    BYTE abDiscrepPerc[2];
    BYTE bASNFlag;            // 'A' = ASN, space = legacy directs
    BYTE bStaticSupp;         // 'S' = static supplier, space = normal
    BYTE abFiller[3];
} DIRSU_REC;

extern FILE_CTRL dirsu;
extern DIRSU_REC dirsurec;

void DirsuSet(void);
URC open_dirsu( void );
URC close_dirsu( WORD type );
LONG ReadDirsu(LONG lLineNumber);
LONG ReadDirsuLog(LONG lLineNumber, WORD wLogLevel);


// ------------------------------------------------------------------------
// UODOT File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abLicence[5];          // (UPD) Child Licence Plate
    BYTE bType;                 // Child type
} DETAIL_CHILD;

typedef struct {
    BYTE ubDateofDespatch[3];   // (UPD) Date Of Despatch YYMMDD

    // UOD INDICAT1 (1byte) :
    UBYTE ubBOLFlag:1;          // BOL bit flag
    UBYTE ubONightFlag:1;       // Overnight bit flag
    UBYTE ubUnused:6;           // unused bit flags

    BYTE abEstDateDeliv[3];     // (UPD) estimated date of delivery YYMMDD
    BYTE abDateDriverCheckIn[3];// (UPD) Delivery driver booked in date YYMMDD
    BYTE abTimeDriverCheckIn[2];// (UPD) Delivery driver time booked in HHMM
    BYTE bOuterType;            // UOD type
    BYTE bReason;               // UOD reason
    BYTE abWHouseArea[3];       // Warehouse Area

    // UOD Status Flags (2 Bytes) :
    UWORD uwBookedFlag:1;
    UWORD uwUpdatedFlag:1;
    UWORD uwAutoFlag:1;
    UWORD uwAuditedFlag:1;
    UWORD uwPartialFlag:1;
    UWORD uwUnused6:1;
    UWORD uwUnused7:1;
    UWORD uwUnused8:1;
    UWORD uwGITMismatchFlag:1;
    UWORD uwBookRFFlag:1;
    UWORD uwBookPDTFlag:1;
    UWORD uwBookMC70Flag:1;
    UWORD uwBOOKContFlag:1;
    UWORD uwUnused14:1;
    UWORD uwUnused15:1;
    UWORD uwUnused16:1;

    BYTE abUltParentUOD[5];     // (UPD) Highest level parent UOD
    BYTE abImmParentUOD[5];     // (UPD)Immediate parent UOD
    BYTE abDateBookedIn[3];     // (UPD) Date booked in YYMMDD
    BYTE abTimeBookedIn[2];     // (UPD) Time booked in HHMM
    BYTE abStoreOpID[4];        // (UPD) Store operator ID
    BYTE abDriverID[4];         // (UPD) Driver ID
    BYTE abBookInLevel[1];      // (UPD) Type
    BYTE abAuditOpId[4];        // (UPD) Audit operator ID
    BYTE abUodNumber[5];        // (UPD) UOD Number                     //AVS 20-04-2015 Dallas
    BYTE abFiller[16];          // Filler                               //AVS 20-04-2015 Dallas
    WORD wNumItems;             // Number of UOD items - zero for a UOD not containing items
    WORD wNumChildren;          // Number of children -  zero for a UOD containing items
    DETAIL_CHILD child[15];
} DETAIL_REC;

typedef struct {
    BYTE bSmryPeriod;           // E = Expected, or O = Outstanding
    BYTE bSmryCont;             // D = Dolly, C = Crate, R = (RoCo) Roll Cage, O = Outer, P = pallet, I = Inter Store transfer
    BYTE abSmryQty[3];          // (UPD) Quantity expected
} SUMMARY_RECS;

typedef union {
    DETAIL_REC Detail;
    SUMMARY_RECS Summary[32];
} UODOT_DET;

typedef struct {
    BYTE abLicence[5];          // (UPD) licence Plate - part of key
    WORD wSeqNo;                // Sequence Number starting 0 - part of key
    UODOT_DET Record;
} UODOT_REC;

extern FILE_CTRL uodot;
extern UODOT_REC uodotrec;

void UodotSet(void);
URC open_uodot( void );
URC close_uodot( WORD type );
LONG ReadUodot(LONG lLineNum);
LONG ReadUodotLog(LONG lLineNumber, WORD wLogLevel);


// ------------------------------------------------------------------------
// UODIN File
// ------------------------------------------------------------------------

typedef struct {
    BYTE abItemCode[3];         // (UPD) Item code with no check digit
    WORD wDespQty;              // Despatch quantity
    WORD wAuditQty;             // Audit quantity
} UODIN_ITEM;

typedef struct {
    BYTE abLicence[5];          // (UPD) licence Plate - part of key
    BYTE abDespDate[3];         // (UPD) Date of despatch YYMMDD
    WORD wSeqNo;                // Sequence Number starting 0 - part of key
    BYTE filler[8];
    UODIN_ITEM Item[70];
} UODIN_REC;

extern FILE_CTRL uodin;
extern UODIN_REC uodinrec;

void UodinSet(void);
URC open_uodin( void );
URC close_uodin( WORD type );
LONG ReadUodin(LONG lLineNum);
LONG ReadUodinLog(LONG lLineNumber, WORD wLogLevel);

#endif

