// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Goods In Application Functions Module
//
// Version 1.0               Brian Greenfield            10th October 2008
//     Initial Version
//
// Version 1.1               Brian Greenfield            20th February 2009
//     Minor corrections due to changes in the DD.
//     We only extract UOD's from DELVINDX if they have no parent.
//     Corrections to the UOD GIF Scancode extract.
//     Change so that a GIQ B request for +ve UOD works for all functions.
//
// Version 1.2               Brian Greenfield            24th February 2009
//     Minor change to GIR for GIQ 3 B for ASN's.
//     Now return the expected delivery date in the despatch date but with
//     century skipped.
//
// Version 1.3               Brian Greenfield            4th March 2009
//     Changed UOD GIF trailer to write out as a record type E rather than
//     a T due to a DD issue.
//     Also corrected the ASN scancode to read the first 14 of the 18 digit
//     number rather than the last 14.
//
// Version 1.4               Brian Greenfield            9th March 2009
//    Corrected GIR response for items in +ve UOD so it correctly 
//    returns the despatch quantity.
//
// Version 1.5               Brian Greenfield            12th March 2009
//    Change request (SFSC2 CR15) to allow devices to request a full GIR 
//    even in booking in mode.
//
// Version 1.6               Brian Greenfield            26th March 2009
//    Change request (SFSC2 CR16) to set the partial flags for
//    UOD/s but only on GIR response.
//    Also corrected a few container type settings to remove unneccesary 
//    hard coding of types C or I.
//
// Version 1.7               Brian Greenfield            7th April 2009
//    Change request (SFSC2 CR21) to allow item level booking records
//    for ASN GIF requests.
//
// Version 1.8               Brian Greenfield            20th April 2009
//    Change request (SFSC2 CR24) to supply despatch quantities for ASN 
//    items rather than booked in quantities (which was incorrectly 
//    specified in the DD.)
//
// Version 1.9               Brian Greenfield            5th May 2009
//    Change request (SFSC2 CR29) to pass a status of A for ASN
//    cartons if audited, else pass U or B.
//    Change also asked for the booked in quantity to be passed for
//    ASN items if the item is booked in.
//
// Version 1.10              Brian Greenfield            19th may 2009
//    Change Request (SFSC2 CR32) to now also pass outstanding unbooked 
//    UOD's when a GIA T is requested. This was missed off in the DD.
//
// Version 1.11              Brian Greenfield            1st Dec 2009
//    Change to ASn GIF write so that it uses the new IRFRead() function
//    to determine if the passed barcode is price embedded. If it is price
//    embedded then the IRFRead() function effectively passes pack the
//    barcode without the price embedded in it any more so use that one.
// ------------------------------------------------------------------------

/* include files */
#include "transact.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>
#include "trxbase.h"
#include "trxutil.h"
#include "osfunc.h"
#include "trxfile.h"
#include "rfsfile.h"
#include "sockserv.h"
#include "rfglobal.h"
#include "osfunc.h"
#include "irf.h"
#include "idf.h"
#include "rfscf.h"
#include "trxGIA.h"
#include "GIAfiles.h"


// ------------------------------------------------------------------------
// GIA / GIB
// ------------------------------------------------------------------------

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE bType;         // 'X' = Logon status, '1' = SSC, '2' = Directs
    BYTE bFunc;         // 'X' = Logon status, '1' = Booking In, '2' = Audit, '3' = View
    BYTE bRequest;      // 'C' = Config, 'S' = Container Summary, 'L' = Directs List
    BYTE bPeriod;       // (Only for SSC) 'T' = Today, 'F' = Future, 'X' = unused
    BYTE abPointer[6];  // 0 = first message, X = not used
} LRT_GIA;
#define LRT_GIA_LTH sizeof(LRT_GIA)

typedef struct {
    BYTE abIdent[10];   // Directs Supplier Number (not used for SSC)
    BYTE abSeq[5];      // SSC only - UODOT sequence number
    BYTE abName[20];    // For SSC = Dolly, Create, etc, for directs = supplier name
    BYTE bSuppType;     // Supplier type for Directs, 'A' = ASN supplier, 'D' = Directs supplier, 'X' = unused
    BYTE bContType;     // Content type - 'C' = container with nested containers, 'I' = Container with items, 'X' = unused
    BYTE abExpDate[8];  // YYYYMMDD Expected delivery date or X's for past delivery
    BYTE bBooked;       // 'B' = booked in, 'U' = Unbooked, 'P' = Partial, 'A' = Already Audited, 'X' = unused
    BYTE abQty[6];      // Number expected (by container for SSC, by supplier for directs)
} GIB_LIST;

typedef struct {
    BYTE abCount[2];            // Count of items sent in this request
    BYTE abPointer[6];          // Pointer to next record to read or -1 if no more
    GIB_LIST ListData[20];      // List data records X 20 (up to)
} GIB_SUMMARY;

typedef struct {
    BYTE bDirectsActive;        // Y or N
    BYTE bPOSUODActive;         // Y or N
    BYTE bASNActive;            // Y or N
    BYTE bOvernightDeliv;       // Y or N
    BYTE bOvernightScan;        // Y or N
    BYTE abBatchSize[3];        // 000 to nnn
} GIB_CONFIG;

typedef union {
    GIB_CONFIG Config;
    GIB_SUMMARY Summary;
} GIB_DETAILS;

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE bRespType;            // 'C' = Config Response, 'S' = Summary response
    GIB_DETAILS Details;
} LRT_GIB;
#define LRT_GIB_CONFIG_LTH 15      // Config response length
#define LRT_GIB_SUM_LTH_NO_LIST 15 // Summary length with no list entries - add lists as below
#define LRT_GIB_SUM_LTH_LIST 52    // Times this by the number of list items sent back

static void Directs_Summary(char *inbound) {

    LRT_GIA* pGIA = (LRT_GIA*)inbound;
    LRT_GIB* pGIB = (LRT_GIB*)out;
    URC urc;
    LONG lrc, rec, found, count;
    
    if (pGIA->bRequest == 'S') { // Should only see Summary requests for directs
    
        switch (pGIA->bFunc) {
    
        case '1': // Booking In
        case '3': // View

            urc = open_delvsmry();
            if (urc) {
                prep_nak("ERROR Unable to open DELVSMRY - check event logs");
                break;
            }

            count = 0;
            found = 0;
            rec = satol(pGIA->abPointer, sizeof(pGIA->abPointer));
            disp_msg("RD DELVSMRY");
            lrc = ReadDelvsmry(rec, __LINE__);
            if (lrc <= 0L) {
                if ((lrc&0xFFFF)==0x4003) { // End Of File
                    found = 0;
                } else {
                    prep_nak("ERROR Unable to read DELVSMY - check event logs");
                    break;
                }
            } else found = lrc;

            if (found == 0) {
                prep_nak("No records");
                break;
            }

            while (found && count < 20) {

                // If booking in function (1) then only select summary records if they 
                // have a +ve unbooked value or the supplier is static
                // If audit function (3) then only select summary records if they 
                // have a +ve total value and the supplier is ASN, or the supplier is static
                if ( (delvsmryrec.bStaticSupp == 'S') || 
                     ( (pGIA->bFunc == '1') && (satol(delvsmryrec.abUnbOrders,sizeof(delvsmryrec.abUnbOrders)) > 0) ) ||
                     ( (pGIA->bFunc == '3') && 
                       (satol(delvsmryrec.abTotOrders,sizeof(delvsmryrec.abTotOrders)) > 0) &&
                       delvsmryrec.bASNSupp == 'A') ) {
                
                    // Add details to GIB
                    sprintf(sbuf, "0000000000", 10);
                    memcpy(sbuf+4, delvsmryrec.abSupplier, 6);
                    memcpy(pGIB->Details.Summary.ListData[count].abIdent, sbuf, 10);
                    memcpy(pGIB->Details.Summary.ListData[count].abSeq, "XXXXX", 5);
                    memcpy(pGIB->Details.Summary.ListData[count].abName , delvsmryrec.abSuppName, 20);
                    pGIB->Details.Summary.ListData[count].bSuppType = (delvsmryrec.bASNSupp == 'A' ? 'A':'D');
                    pGIB->Details.Summary.ListData[count].bContType = 'X';
                    memcpy(pGIB->Details.Summary.ListData[count].abExpDate, "XXXXXXXX", 8);
                    pGIB->Details.Summary.ListData[count].bBooked = 'X';
                    if (pGIA->bFunc == '1') { // Output relevant order value
                        sprintf(sbuf, "%06ld", satol(delvsmryrec.abUnbOrders,sizeof(delvsmryrec.abUnbOrders))); 
                    } else { // Function must be 3
                        sprintf(sbuf, "%06ld", satol(delvsmryrec.abTotOrders,sizeof(delvsmryrec.abTotOrders))); 
                    }
                    memcpy(pGIB->Details.Summary.ListData[count].abQty, sbuf, 6);
                    sprintf(sbuf, "%06ld", rec);
                    memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                    count++;
                }

                // Check if there is another record
                found = 0;
                rec+=1;
                disp_msg("RD NEXT DELVSMRY");
                lrc = ReadDelvsmry(rec, __LINE__);
                if (lrc <= 0L) {
                    if ((lrc&0xFFFF)==0x4003) { // End Of File
                        found = 0;
                    } else {
                        prep_nak("ERROR Unable to read DELVSMRY - check event logs");
                        break;
                    }
                } else found = lrc;
                if (found) {
                    sprintf(sbuf, "%06ld", rec);
                    memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                } else memcpy(pGIB->Details.Summary.abPointer, "-1    ", 6);
            }
            
            // Write count to GIB
            if (count) {
                pGIB->bRespType = 'S';
                memcpy(pGIB->abCmd, "GIB", 3);
                memcpy(pGIB->abOpID, pGIA->abOpID, 3);
                sprintf(sbuf, "%02ld", count);
                memcpy(pGIB->Details.Summary.abCount, sbuf, 2);
                out_lth = LRT_GIB_SUM_LTH_NO_LIST + (LRT_GIB_SUM_LTH_LIST * count);
            } else prep_nak("No records");
            break;

        case '2': // Audit

            // Simply send an ACK as no summary details required for this request
            prep_ack( "" );
            break;

        default:
            prep_nak("ERROR Malformed Transaction - GIA Type");
            break;
        }

        close_delvsmry(CL_ALL);

    } else prep_nak("ERROR Malformed Transaction - GIA Type");
}

static void UOD_Summary(char *inbound) {

    LRT_GIA* pGIA = (LRT_GIA*)inbound;
    LRT_GIB* pGIB = (LRT_GIB*)out;
    URC urc;
    LONG lrc, count, found, rec, sec, day, month, year;
    WORD hour, min;
    DOUBLE dTodayDate, dUODDate;
    BYTE abCentury[2];

    switch (pGIA->bFunc) {
    
    case '1': //Booking In
        
        if (pGIA->bRequest == 'S') { // Should only see summary requests for UOD booking in

            urc = open_uodot();
            if (urc) {
                prep_nak("ERROR Unable to open UODOT - check event logs");
                break;
            }
            
            memset(uodotrec.abLicence, 0xFF, sizeof(uodotrec.abLicence));
            uodotrec.wSeqNo =  0xFFFF;
            disp_msg("RD UODOT");
            lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
            if (lrc <= 0L) {
                prep_nak("ERROR Unable to read UODOT - check event logs");
                break;
            } else {

                count = 0;
                rec = satol(pGIA->abPointer, sizeof(pGIA->abPointer));

                while (count < 20 && rec < 32 ) {
                       
                    if (uodotrec.Record.Summary[rec].bSmryPeriod == 'E' || 
                        uodotrec.Record.Summary[rec].bSmryPeriod == 'O') {
                        sprintf(sbuf, "%c          ", uodotrec.Record.Summary[rec].bSmryPeriod);
                        memcpy(pGIB->Details.Summary.ListData[count].abIdent, sbuf, 10);
                        memcpy(pGIB->Details.Summary.ListData[count].abSeq, "XXXXX", 5);
                        switch(uodotrec.Record.Summary[rec].bSmryCont) {
                        case 'D': sprintf(sbuf, "Dolly               "); break;
                        case 'C': sprintf(sbuf, "Crate               "); break;
                        case 'R': sprintf(sbuf, "RoCo                "); break;
                        case 'O': sprintf(sbuf, "Outer               "); break;
                        case 'P': sprintf(sbuf, "Pallet              "); break;
                        case 'I': sprintf(sbuf, "Inter Store Transfer"); break;
                        default : sprintf(sbuf, "Unknown             "); break;
                        }
                        memcpy(pGIB->Details.Summary.ListData[count].abName , sbuf, 20);
                        pGIB->Details.Summary.ListData[count].bSuppType = 'X';
                        pGIB->Details.Summary.ListData[count].bContType = 'X';
                        memcpy(pGIB->Details.Summary.ListData[count].abExpDate, "XXXXXXXX", 8);
                        pGIB->Details.Summary.ListData[count].bBooked = 'X';
                        unpack(pGIB->Details.Summary.ListData[count].abQty, 6, uodotrec.Record.Summary[rec].abSmryQty, 3, 0 );
                        sprintf(sbuf, "%06ld", rec);
                        memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                        count++;
                    }
                    rec++;
                }

                // If we've got 20 output records we need to work out if there are any more
                // otherwise set the returned pointer to -1
                if (count == 20 &&
                    (uodotrec.Record.Summary[rec].bSmryPeriod == 'E' ||
                    uodotrec.Record.Summary[rec].bSmryPeriod == 'O')) {
                    // We have more records so set pointer to the next record
                    sprintf(sbuf, "%06ld", rec);
                    memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                } else memcpy(pGIB->Details.Summary.abPointer, "-1    ", 6);
                
                sprintf(sbuf, "%02ld", count);
                memcpy(pGIB->Details.Summary.abCount, sbuf, 2);
                pGIB->bRespType = 'S';
                memcpy(pGIB->abCmd, "GIB", 3);
                memcpy(pGIB->abOpID, pGIA->abOpID, 3);
                out_lth = LRT_GIB_SUM_LTH_NO_LIST + (LRT_GIB_SUM_LTH_LIST * count);
            }
            
        } else prep_nak("ERROR Malformed Transaction - GIA Type");
        break;

    case '2': // Audit

        // Simply respond with an ACK. Do data to pass to PPC and only needed to get the files opened
        prep_ack( "" );
        break;
    
    case '3': // View

        if (pGIA->bRequest == 'L' && (pGIA->bPeriod == 'T' || pGIA->bPeriod == 'F')) { // Should only see List requests for today or future for a UOD view
            
            urc = open_uodot();
            if (urc) {
                prep_nak("ERROR Unable to open UODOT - check event logs");
                break;
            }
            
            urc = open_delvindx();
            if (urc) {
                prep_nak("ERROR Unable to open DELVINDX - check event logs");
                break;
            }
            
            // Get todays date as a number
            sysdate( &day, &month, &year, &hour, &min, &sec );
            dTodayDate = ConvGJ( day, month, year );
            sprintf(sbuf, "%ld", year);
            memcpy(abCentury, sbuf, 2);
            
            count = 0;
            found = 0;
            rec = satol(pGIA->abPointer, sizeof(pGIA->abPointer));
            disp_msg("RD DELVINDX");
            lrc = ReadDelvindx(rec, __LINE__);
            if (lrc <= 0L) {
                if ((lrc&0xFFFF)==0x4003) { // End Of File
                    found = 0;
                } else {
                    prep_nak("ERROR Unable to read DELINDX - check event logs");
                    break;
                }
            } else found = lrc;

            if (found == 0) {
                prep_nak("No records");
                break;
            }

            while (found && count < 20) {

                // Only extract if the parent code is 0's                       /*1.1 BMG 20-02-2009*/
                memset(sbuf, '0', 10);                                          /*1.1 BMG 20-02-2009*/
                if ( memcmp(delvindxrec.abImmParent, sbuf, 10) == 0 ) {         /*1.1 BMG 20-02-2009*/
                
                    // Extract the record if the date matches: T = todays date, F = future date
                    // Note that the year is only 2 digits so have to set the first to digits from the system century for compare
                    memcpy(sbuf, abCentury, 2);
                    memcpy(sbuf+2, delvindxrec.abExpDelivDate, 6);  
                    day   = satol( sbuf+6, 2 );                       
                    month = satol( sbuf+4, 2 );                       
                    year  = satol( sbuf, 4 );                       
                    dUODDate = ConvGJ( day, month, year );   

                    if ( ((pGIA->bPeriod == 'T') && (dUODDate <= dTodayDate)) || /*1.10 BMG 19-05-2009*/
                         ((pGIA->bPeriod == 'F') && (dUODDate > dTodayDate))) {
                
                        // Read UODOT file 
                        pack(uodotrec.abLicence, 5, delvindxrec.abLicence, 10, 0);
                        uodotrec.wSeqNo = satol(delvindxrec.abSeq, 5);
                        disp_msg("RD UODOT");
                        lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
                        if (lrc > 0L) {
                            // Add details to GIB if found UODOT record

                            //Have to re-test so we can extract only required records.                                               /*1.10 BMG 19-05-2009*/
                            //Required because we only need past unbooked UOD's if "T" was requested                                 /*1.10 BMG 19-05-2009*/
                            //and this is only known once we've read the UODOT file.                                                 /*1.10 BMG 19-05-2009*/
                            //This is quicker than reading the UODOT for every record in the DELVINDX.                               /*1.10 BMG 19-05-2009*/
                            if (((pGIA->bPeriod == 'T') && (dUODDate == dTodayDate)) ||                                              /*1.10 BMG 19-05-2009*/
                                ((pGIA->bPeriod == 'T') && (dUODDate < dTodayDate) && (uodotrec.Record.Detail.uwBookedFlag == 0)) || /*1.10 BMG 19-05-2009*/
                                ((pGIA->bPeriod == 'F') && (dUODDate > dTodayDate))) {                                               /*1.10 BMG 19-05-2009*/
                                
                                memcpy(pGIB->Details.Summary.ListData[count].abIdent, delvindxrec.abLicence, 10);
                                memcpy(pGIB->Details.Summary.ListData[count].abSeq, delvindxrec.abSeq, 5);
                                sprintf(sbuf, "                    ");
                                memset(sbuf, delvindxrec.bUODType, 1);
                                memcpy(pGIB->Details.Summary.ListData[count].abName , sbuf, 20);
                                pGIB->Details.Summary.ListData[count].bSuppType = 'X';
                                pGIB->Details.Summary.ListData[count].bContType = (delvindxrec.bUODType == 'D' ? 'C':'I');
                                sprintf(sbuf, "20      ");
                                memcpy(sbuf+2, delvindxrec.abExpDelivDate, 6);
                                memcpy(pGIB->Details.Summary.ListData[count].abExpDate, sbuf, 8);
                                // If audit then A = booked, U = unbooked
                                // If not audit then B = booked, U = unbooked
                                if (uodotrec.Record.Detail.uwAuditedFlag) {
                                   pGIB->Details.Summary.ListData[count].bBooked = (uodotrec.Record.Detail.uwBookedFlag ?'A':'U');
                                } else {
                                    pGIB->Details.Summary.ListData[count].bBooked = (uodotrec.Record.Detail.uwBookedFlag ?'B':'U');
                                }
                                sprintf(sbuf, "%06d", uodotrec.Record.Detail.wNumItems);
                                memcpy(pGIB->Details.Summary.ListData[count].abQty, sbuf, 6);
         
                                sprintf(sbuf, "%06ld", rec);
                                memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                                count++;
                            }                                                                                                        /*1.10 BMG 19-05-2009*/
                        }
                    }
                }                                                               /*1.1 BMG 20-02-2009*/

                // Check if there is another record
                found = 0;
                rec+=1;
                disp_msg("RD NEXT DELVINDX");
                lrc = ReadDelvindx(rec, __LINE__);
                if (lrc <= 0L) {
                    if ((lrc&0xFFFF)==0x4003) { // End Of File
                        found = 0;
                    } else {
                        prep_nak("ERROR Unable to read DELVINDX - check event logs");
                        break;
                    }
                } else found = lrc;
                if (found) {
                    sprintf(sbuf, "%06ld", rec);
                    memcpy(pGIB->Details.Summary.abPointer, sbuf, 6);
                } else memcpy(pGIB->Details.Summary.abPointer, "-1    ", 6);
            }
            
            // Write count to GIB
            if (count) {
                pGIB->bRespType = 'S';
                memcpy(pGIB->abCmd, "GIB", 3);
                memcpy(pGIB->abOpID, pGIA->abOpID, 3);
                sprintf(sbuf, "%02ld", count);
                memcpy(pGIB->Details.Summary.abCount, sbuf, 2);
                out_lth = LRT_GIB_SUM_LTH_NO_LIST + (LRT_GIB_SUM_LTH_LIST * count);
            } else prep_nak("No records");
            break;

        } else prep_nak("ERROR Malformed Transaction - GIA Type");
        break;

    default:
        prep_nak("ERROR Malformed Transaction - GIA Type");
        break;
    }

    close_delvindx(CL_ALL);
    close_uodot(CL_ALL);
}

void GIA_Start(char *inbound) {

    LRT_GIA* pGIA = (LRT_GIA*)inbound;
    LRT_GIB* pGIB = (LRT_GIB*)out;

    //Initial checks                
    if (IsStoreClosed()) return;    
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();

    switch (pGIA->bType) {
    
    case 'X': // Logon type GIA request so format GIB response accordingly

        memcpy(pGIB->abCmd, "GIB", 3);
        memcpy(pGIB->abOpID, pGIA->abOpID, 3);
        pGIB->bRespType = 'S';
        pGIB->Details.Config.bDirectsActive = rfscfrec3.bDirectsActive;
        pGIB->Details.Config.bPOSUODActive= rfscfrec3.bPOSUODActive;
        pGIB->Details.Config.bASNActive = rfscfrec3.bASNActive;
        pGIB->Details.Config.bOvernightDeliv = rfscfrec3.bOvernightDeliv;
        pGIB->Details.Config.bOvernightScan = rfscfrec3.bOvernightScan;
        memcpy(pGIB->Details.Config.abBatchSize, rfscfrec3.abScanBatchSize, 3);
        out_lth = LRT_GIB_CONFIG_LTH;
        break;

    case '1': // SSC Check

        if (rfscfrec3.bPOSUODActive == 'Y') {
            UOD_Summary(inbound);
        } else {
            prep_nak("Goods In Application is not available in your store");
        }
        break;

    case '2': // Directs Check

        if (rfscfrec3.bDirectsActive == 'Y') {
            Directs_Summary(inbound);
        } else {
            prep_nak("Goods In Application is not available in your store");
        }
        break;

    default:
        prep_nak("ERROR Malformed Transaction - GIA Type");
        break;
    }
}


// ------------------------------------------------------------------------
// GIF
// ------------------------------------------------------------------------

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE bType;         // 1 = SSC, 2 = Directs, 3 = ASN's
    BYTE bFunc;         // 1 = Booking in, 2 = Audit, 3 = View
    BYTE abScanCode[20];// Scan Code
    BYTE abDespDate[6]; // Despatch date YYMMDD
    BYTE bScanType;     // Scan Type B = Staff booking, A = Audit, N = Misdirect(SSC only), L = UOD (SSC only), C = Batch Confirmation, S = End of Session
    BYTE bLevel;        // Scan Level, D = Delivery unit level, I = Item level, X = unused
    BYTE abScanDate[8];     // YYYYMMDD Scan Date
    BYTE abScanTime[6];     // HHMMSS Scan Time
    BYTE abBadge[8];        // Drivers badge Number
    BYTE bGITNote;          // Y = matched, N = discrepancy (only for scantype S)
    BYTE bRescan;           // Inidcates if batch is rejected and will be rescanned
    BYTE abBarcode[13];     // Audit Barcode
    BYTE abQty[6];          // Audit Quantity
    BYTE abStatus[5];       // S = start of item batch, X = item detail record, nnnnn = item count at end of batch
    BYTE abSeq[2];          // Directs sequence or XX for unused
} LRT_GIF;
#define LRT_GIF_SCAN_LTH 60
#define LRT_GIF_AUDIT_LTH 63

void GIF_Booking(char *inbound) {

    URC usrrc;
    LRT_GIF* pGIF = (LRT_GIF*)inbound;
    BYTE cbbufrec[32];
    BYTE pbbufrec[40];
    BYTE dbbufrec[54];
    BYTE bar_code[7]; /*1.11 BMG 01-12-2009*/
    LONG rc;

    //Initial checks                
    if (IsStoreClosed()) return;    
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();             

    
    switch (pGIF->bType) {
    
    case '1': // SSC

        // Create PUB buffer file if needed (created as temp file WU....)
        if ( lrtp[hh_unit]->lPUBfnum <= 0 ) {
            usrrc = prepare_workfile( hh_unit, SYS_PUB );
            if (usrrc<RC_IGNORE_ERR) {
                prep_nak("ERRORUnable to create workfile. "
                          "Check appl event logs" );
                return;
            } else {
                // Create and write header record
                memcpy(pbbufrec, "H00000    Y  \r\n", 15);
                memcpy(pbbufrec+6, pGIF->abOpID, 3);
                memcpy(pbbufrec+9, lrtp[hh_unit]->Type, 1);
                pbbufrec[11] = rfscfrec3.bOvernightDeliv;
                pbbufrec[12] = rfscfrec3.bOvernightScan;
                if (debug) {                                 
                    disp_msg( "WR WU IN GIF:" );          
                    dump(pbbufrec, 15);
                }                                            
                rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 15L, 0L );           
                if (rc<0L) {                                          
                    if (debug) {                                       
                        sprintf(msg, "Err-W to %s. RC:%08lX",          
                                pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                        disp_msg(msg);                                 
                    }                                                  
                    prep_nak("ERRORUnable to "                         
                              "write to WU*.*. "                       
                              "Check appl event logs" );               
                    return;                                            
                }
            }
        }

        if (pGIF->bFunc == '1') {
            // Booking in
            if (pGIF->bScanType == 'C' || pGIF->bScanType == 'S') {
                // Batch/Session scan message 
                memcpy(pbbufrec, "C                       \r\n", 26);
                memcpy(pbbufrec+1, pGIF->abBadge, 8);
                memcpy(pbbufrec+9, pGIF->abScanDate+2, 6);
                memcpy(pbbufrec+15, pGIF->abScanTime, 6);
                pbbufrec[21] = pGIF->bGITNote;
                pbbufrec[22] = pGIF->bScanType;
                pbbufrec[23] = pGIF->bRescan;
                if (debug) {                                 
                    disp_msg( "WR WU IN GIF:" );          
                    dump(pbbufrec, 26);
                }                                            
                rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 26L, 0L );
                if (rc<0L) {                                          
                    if (debug) {                                       
                        sprintf(msg, "Err-W to %s. RC:%08lX",          
                                pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                        disp_msg(msg);                                 
                    }                                                  
                    prep_nak("ERRORUnable to "                         
                              "write to WU*.*. "                       
                              "Check appl event logs" );               
                    return;                                            
                } else {
                    prep_ack( "" );
                }
            } else {
                // UOD booking record
                memcpy(pbbufrec, "B                             \r\n", 32);
                memcpy(pbbufrec+1, pGIF->abScanCode+10, 10);                    /*1.1 BMG 20-02-2009*/
                memcpy(pbbufrec+11, pGIF->abDespDate, 6);
                memcpy(pbbufrec+17, pGIF->abScanDate+2, 6);
                memcpy(pbbufrec+23, pGIF->abScanTime, 6);
                pbbufrec[29] = pGIF->bScanType;
                
                if (debug) {                                 
                    disp_msg( "WR WU IN GIF:" );          
                    dump(pbbufrec, 32);
                }                                            
                rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 32L, 0L );
                if (rc<0L) {                                          
                    if (debug) {                                       
                        sprintf(msg, "Err-W to %s. RC:%08lX",          
                                pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                        disp_msg(msg);                                 
                    }                                                  
                    prep_nak("ERRORUnable to "                         
                              "write to WU*.*. "                       
                              "Check appl event logs" );               
                    return;                                            
                } else {
                    prep_ack( "" );
                }
            }
        } else {
            if (pGIF->bFunc == '2') {
                // Audit
                if (pGIF->abStatus[0] == 'S') {
                    // Header for audit
                    memcpy(pbbufrec, "A                             \r\n", 32);
                    memcpy(pbbufrec+1, pGIF->abScanCode+10, 10);                /*1.1 BMG 20-02-2009*/
                    memcpy(pbbufrec+11, pGIF->abDespDate, 6);
                    memcpy(pbbufrec+17, pGIF->abScanDate+2, 6);
                    memcpy(pbbufrec+23, pGIF->abScanTime, 6);
                    pbbufrec[29] = pGIF->bScanType;
                    if (debug) {                                 
                        disp_msg( "WR WU IN GIF:" );          
                        dump(pbbufrec, 32);                      
                    }                                            
                    rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 32L, 0L );           
                    if (rc<0L) {                                          
                        if (debug) {                                       
                            sprintf(msg, "Err-W to %s. RC%08lX",          
                                    pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                            disp_msg(msg);                                 
                        }                                                  
                        prep_nak("ERRORUnable to "                         
                                  "write to WU*.*. "                       
                                  "Check appl event logs" );               
                        return;                                            
                    } else {
                        // Now write item detail record out from the same request
                        memcpy(pbbufrec, "D                 \r\n", 20);
                        memcpy(pbbufrec+1, pGIF->abBarcode, 13);
                        memcpy(pbbufrec+14, pGIF->abQty+2, 4);
                        if (debug) {                                 
                            disp_msg( "WR WU IN GIF:" );          
                            dump(pbbufrec, 20);                      
                        }                                            
                        rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 20L, 0L );           
                        if (rc<0L) {                                          
                            if (debug) {                                       
                                sprintf(msg, "Err-W to %s. RC%08lX",          
                                        pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                                disp_msg(msg);                                 
                            }                                                  
                            prep_nak("ERRORUnable to "                         
                                      "write to WU*.*. "                       
                                      "Check appl event logs" );               
                            return;
                        } else {
                            prep_ack( "" );
                        }
                    }
                } else {
                    if (pGIF->abStatus[0] == 'X') {
                        // Item for audit
                        memcpy(pbbufrec, "D                 \r\n", 20);
                        memcpy(pbbufrec+1, pGIF->abBarcode, 13);
                        memcpy(pbbufrec+14, pGIF->abQty+2, 4);
                        if (debug) {                                 
                            disp_msg( "WR WU IN GIF:" );          
                            dump(pbbufrec, 20);                      
                        }                                            

                        rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 20L, 0L );           
                        if (rc<0L) {                                          
                            if (debug) {                                       
                                sprintf(msg, "Err-W to %s. RC%08lX",          
                                        pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                                disp_msg(msg);                                 
                            }                                                  
                            prep_nak("ERRORUnable to "                         
                                      "write to WU*.*. "                       
                                      "Check appl event logs" );               
                            return;                                            
                        } else {
                            prep_ack( "" );
                        }
                    } else {
                        if (pGIF->abStatus[0] != 'X' && pGIF->abStatus[0] != 'S') {
                            // Trailer for audit
                            memcpy(pbbufrec, "E     \r\n", 8);                      /*1.3 BMG 04-03-2009*/
                            memcpy(pbbufrec+1, pGIF->abStatus, 5);
                            if (debug) {                                 
                                disp_msg( "WR WU IN GIF:" );          
                                dump(pbbufrec, 8);
                            }                                            

                            rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lPUBfnum, (void *)&pbbufrec, 8L, 0L );           
                            if (rc<0L) {                                          
                                if (debug) {                                       
                                    sprintf(msg, "Err-W to %s. RC%08lX",          
                                            pq[lrtp[hh_unit]->pq_PUB].fname, rc);
                                    disp_msg(msg);                                 
                                }                                                  
                                prep_nak("ERRORUnable to "                         
                                          "write to WU*.*. "                       
                                          "Check appl event logs" );               
                                return;                                            
                            } else {
                                prep_ack( "" );
                            }
                        } else {
                            prep_nak("ERROR Malformed Transaction - GIF Function");
                        }
                    }
                }
            } else {
               prep_nak("ERROR Malformed Transaction - GIF Function");
            }
        }
        break;

    case '2': // Directs

        // Create DIR buffer file if needed  (created as temp file WD....)
        if ( lrtp[hh_unit]->lDIRfnum <= 0 ) {
            usrrc = prepare_workfile( hh_unit, SYS_DIR );
            if (usrrc<RC_IGNORE_ERR) {
                prep_nak("ERRORUnable to create workfile. "
                          "Check appl event logs" );
                return;
            }
        }
        memcpy(dbbufrec, "O                                               \r\n", 50);
        memcpy(dbbufrec+1, pGIF->abScanCode+5, 13);
        memcpy(dbbufrec+14, "000", 3);
        memcpy(dbbufrec+17, pGIF->abScanCode+18, 2);
        memcpy(dbbufrec+19, pGIF->abBarcode, 13);
        memcpy(dbbufrec+32, pGIF->abQty+2, 4);
        memcpy(dbbufrec+36, pGIF->abScanDate+2, 6);
        memcpy(dbbufrec+42, pGIF->abScanTime, 6);
        if (debug) {                                 
            disp_msg( "WR WD IN GIF:" );          
            dump(dbbufrec, 50);                      
        }                                            

        rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lDIRfnum, (void *)&dbbufrec, 50L, 0L );           
        if (rc<0L) {                                          
            if (debug) {                                       
                sprintf(msg, "Err-W to %s. RC%08lX",          
                        pq[lrtp[hh_unit]->pq_DIR].fname, rc);
                disp_msg(msg);                                 
            }                                                  
            prep_nak("ERRORUnable to "                         
                      "write to WD*.*. "                       
                      "Check appl event logs" );               
            return;                                            
        } else {
            prep_ack( "" );
        }
        break;

    case '3': //ASN's

        // Create CB buffer file if needed  (created as temp file WC....)
        if ( lrtp[hh_unit]->lCBfnum <= 0 ) {
            usrrc = prepare_workfile( hh_unit, SYS_CB );
            if (usrrc<RC_IGNORE_ERR) {
                prep_nak("ERRORUnable to create workfile. "
                          "Check appl event logs" );
                return;
            }
        }

        // Write the record to the carton buffer file
        if (pGIF->bFunc == '1' && pGIF->bLevel != 'I') {                /*1.7 BMG 07-04-2009*/
            // Carton Record Booking In
            memcpy(cbbufrec, "C              Y\r\n", 18);
            memcpy(cbbufrec+1, pGIF->abScanCode+2 , 14);                /*1.3 BMG 04-03-2009*/
            if (debug) {                                 
                disp_msg( "WR WC IN GIF:" );          
                dump(cbbufrec, 18);                      
            }                                            

            rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lCBfnum, (void *)&cbbufrec, 18L, 0L );           
            if (rc<0L) {                                          
                if (debug) {                                       
                    sprintf(msg, "Err-W to %s. RC%08lX",          
                            pq[lrtp[hh_unit]->pq_CB].fname, rc);
                    disp_msg(msg);                                 
                }                                                  
                prep_nak("ERRORUnable to "                         
                          "write to WC*.*. "                       
                          "Check appl event logs" );               
                return;                                            
            } else {
                prep_ack( "" );
            }

        } else {
            if ( (pGIF->bFunc == '1' && pGIF->bLevel == 'I') || pGIF->bFunc == '2') {   /*1.7 BMG 07-04-2009*/
                // Audit level - also now covers item level booking in                  /*1.7 BMG 07-04-2009*/
                if (pGIF->abStatus[0] == 'S') {
                    // Header for audit
                    memcpy(cbbufrec, "H              \r\n", 17);
                    memcpy(cbbufrec+1, pGIF->abScanCode+2 , 14);        /*1.3 BMG 04-03-2009*/
                    if (debug) {                                 
                        disp_msg( "WR WC IN GIF:" );          
                        dump(cbbufrec, 17);                      
                    }                                            
                    rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lCBfnum, (void *)&cbbufrec, 17L, 0L );           
                    if (rc<0L) {                                          
                        if (debug) {                                       
                            sprintf(msg, "Err-W to %s. RC%08lX",          
                                    pq[lrtp[hh_unit]->pq_CB].fname, rc);
                            disp_msg(msg);                                 
                        }                                                  
                        prep_nak("ERRORUnable to "                         
                                  "write to WC*.*. "                       
                                  "Check appl event logs" );               
                        return;                                            
                    } else {
                        // Now write item detail record out from the same request
                        memcpy(cbbufrec, "D                 \r\n", 20);
                        //memcpy(cbbufrec+1, pGIF->abBarcode, 13);                                 /*1.11 BMG 01-12-2009*/
                        //Read the IRF in case this is a price-embedded barcode                    /*1.11 BMG 01-12-2009*/
                        memset(irfrec.bar_code, 0x00, 11);                                         /*1.11 BMG 01-12-2009*/
                        pack(irfrec.bar_code+5, 6, pGIF->abBarcode, 12, 0);                        /*1.11 BMG 01-12-2009*/
                        // Read IRF                                                                /*1.11 BMG 01-12-2009*/
                        rc = IrfRead(__LINE__);                                                    /*1.11 BMG 01-12-2009*/
                        if (rc<=0L) {                                                              /*1.11 BMG 01-12-2009*/
                            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {                      /*1.11 BMG 01-12-2009*/
                                //Read failed so write out what was passed                         /*1.11 BMG 01-12-2009*/
                                memcpy(cbbufrec+1, pGIF->abBarcode, 13);                           /*1.11 BMG 01-12-2009*/
                            }                                                                      /*1.11 BMG 01-12-2009*/
                        } else {                                                                   /*1.11 BMG 01-12-2009*/
                            //Read worked so use IRF barcode just in case it has changed           /*1.11 BMG 01-12-2009*/
                            //because it was an embedded price barcode                             /*1.11 BMG 01-12-2009*/
                            calc_ean13_cd(bar_code, irfrec.bar_code+5);                            /*1.11 BMG 01-12-2009*/
                            unpack(cbbufrec+1, 13, bar_code, 7, 1);                                /*1.11 BMG 01-12-2009*/
                        }                                                                          /*1.11 BMG 01-12-2009*/
                        memcpy(cbbufrec+14, pGIF->abQty+2, 4);
                        if (debug) {                                 
                            disp_msg( "WR WC IN GIF:" );          
                            dump(cbbufrec, 20);                      
                        }                                            
                        rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lCBfnum, (void *)&cbbufrec, 20L, 0L );           
                        if (rc<0L) {                                          
                            if (debug) {                                       
                                sprintf(msg, "Err-W to %s. RC%08lX",          
                                        pq[lrtp[hh_unit]->pq_CB].fname, rc);
                                disp_msg(msg);                                 
                            }                                                  
                            prep_nak("ERRORUnable to "                         
                                      "write to WC*.*. "                       
                                      "Check appl event logs" );               
                            return;
                        } else {
                            prep_ack( "" );
                        }
                    }
                } else {
                    if (pGIF->abStatus[0] == 'X') {
                        // Item for audit
                        memcpy(cbbufrec, "D                 \r\n", 20);
                        memcpy(cbbufrec+1, pGIF->abBarcode, 13);
                        memcpy(cbbufrec+14, pGIF->abQty+2, 4);
                        if (debug) {                                 
                            disp_msg( "WR WC IN GIF:" );          
                            dump(cbbufrec, 20);                      
                        }                                            

                        rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lCBfnum, (void *)&cbbufrec, 20L, 0L );           
                        if (rc<0L) {                                          
                            if (debug) {                                       
                                sprintf(msg, "Err-W to %s. RC%08lX",          
                                        pq[lrtp[hh_unit]->pq_CB].fname, rc);
                                disp_msg(msg);                                 
                            }                                                  
                            prep_nak("ERRORUnable to "                         
                                      "write to WC*.*. "                       
                                      "Check appl event logs" );               
                            return;                                            
                        } else {
                            prep_ack( "" );
                        }
                    } else {
                        if (pGIF->abStatus[0] != 'X' && pGIF->abStatus[0] != 'S') {
                            // Trailer for audit
                            memcpy(cbbufrec, "T     \r\n", 8);
                            memcpy(cbbufrec+1, pGIF->abStatus, 5);
                            if (debug) {                                 
                                disp_msg( "WR WC IN GIF:" );          
                                dump(cbbufrec, 8);
                            }                                            

                            rc = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->lCBfnum, (void *)&cbbufrec, 8L, 0L );           
                            if (rc<0L) {                                          
                                if (debug) {                                       
                                    sprintf(msg, "Err-W to %s. RC%08lX",          
                                            pq[lrtp[hh_unit]->pq_CB].fname, rc);
                                    disp_msg(msg);                                 
                                }                                                  
                                prep_nak("ERRORUnable to "                         
                                          "write to WC*.*. "                       
                                          "Check appl event logs" );               
                                return;                                            
                            } else {
                                prep_ack( "" );
                            }
                        } else {
                            prep_nak("ERROR Malformed Transaction - GIF Function");
                        }
                    }
                }
            } else {
                prep_nak("ERROR Malformed Transaction - GIF Function");
            }
        }
        break;

    default:
        prep_nak("ERROR Malformed Transaction - GIF Type");
        break;
    }
}


// ------------------------------------------------------------------------
// GIQ / GIR
// ------------------------------------------------------------------------

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE bType;         // 1 = SSC, 2 = Directs, 3 = ASN
    BYTE bFunc;         // 1 = Booking in, 2 = Audit, 3 = View
    BYTE abCode[20];    // Booking in code or supplier number for search
    BYTE abSeq[5];      // SSC or Directs sequence number, or X's if not used
    BYTE bReqType;      // Request Type, B = Booking enquiry (short) , F = Full Summary, S = Supplier query
    BYTE bContType;     // Content type - 'C' = container with nested containers, 'I' = Container with items, 'X' = unused
    BYTE bSuppType;     // Code type. A = ASN, D = Legacy directs, X = unused
    BYTE abPointer[6];  // Pointer, 0 = first record
} LRT_GIQ;
#define LRT_GIQ_LTH sizeof(LRT_GIQ)

typedef struct {
    BYTE abDespDate[6]; // Despatch date YYMMDD
    BYTE bOuter;        // D= Dolly, C = Crate, R = RoCo (Roll Cage), O = Outer, P = Pallet
    BYTE bContType;     // Content type - 'C' = container with nested containers, 'I' = Container with items, 'X' = unused
    BYTE bReason;       // M = Misdirected, L = Late, X = purge, ' ' = standard
    BYTE bStatus;       // B = booked in, U = Unbooked, P = Partial, A = Already Audited
    BYTE bBOLUOD;       // Y = contains BOL child UOD's or is chulkd BOL UOD, N = contains only BTC containers
    BYTE abOrderNum[5]; // Order Number
    BYTE bOrderSuf;     // Order Suffix
    BYTE bBC;           // BC Letter
} GIR_BOOKING;

typedef struct {
    BYTE abIdent[10];   // Identifier
    BYTE abName[13];    // Name
    BYTE bBooked;       // 'B' = booked in, 'U' = Unbooked, 'P' = Partial, 'A' = Already Audited, 'X' = unused
    BYTE bContType;     // Content type - 'C' = container with nested containers, 'I' = Container with items, 'X' = unused
    BYTE abDesc[20];    // Description
    BYTE abQuantity[6]; // Quantity
    BYTE abSeq[2];      // Directs sequence number or XX for unused.
} GIR_ITEMS;

typedef struct {
    BYTE abDespDate[6];     // Despatch date YYMMDD
    BYTE bOuter;            // D= Dolly, C = Crate, R = (RoCo) Roll Cage, O = Outer, P = Pallet
    BYTE bContType;         // Content type - 'C' = container with nested containers, 'I' = Container with items, 'X' = unused
    BYTE bReason;           // M = Misdirected, L = Late, X = purge, ' ' = standard
    BYTE bStatus;           // B = booked in, U = Unbooked, P = Partial, A = Already Audited
    BYTE bBOLUOD;           // Y = contains BOL child UOD's or is chulkd BOL UOD, N = contains only BTC containers
    BYTE abOrderNum[5];     // Order Number
    BYTE bOrderSuf;         // Order Suffix
    BYTE bBC;               // BC Letter
    BYTE abEstDelivDate[6]; // YYMMDD Estimated delivery date
    BYTE abDriver[8];       // Driver badge
    BYTE abDrivChkDate[6];  // YYMMDD Driver Check In Date
    BYTE abDrivChkTime[4];  // HHMM Driver Check In Time
    BYTE abStoreOpId[8];    // Store operator ID
    BYTE abBookInDate[6];   // YYMMDD Book In Date
    BYTE abBookInTime[4];   // HHMM Book In Time
    BYTE abCount[2];        // Count of entries sent in this response
    BYTE abPointer[6];      // Pointer of next record to read or -1 for the end
    GIR_ITEMS Items[16];    // Item records X 16 (up to)
} GIR_FULL;

typedef struct {
    BYTE abSuppNum[6];      // Supplier number
    BYTE abSuppName[10];    // Supplier name
    BYTE bSuppType;         // A (ASN) , D (Directs), X unused
} GIR_SUPPLIER;

typedef union {
    GIR_BOOKING BookingRec;
    GIR_SUPPLIER SupplierRec;
    GIR_FULL FullRec;
} GIR_DETAILS;

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE abCode[20];    // Code from search
    BYTE bRespType;     // Response Type, B = Booking enquiry (short), S = Supplier query, F = Full Summary
    GIR_DETAILS details;
} LRT_GIR;
#define LRT_GIR_BOOKING_LTH 45
#define LRT_GIR_SUPPLIER_LTH 44
#define LRT_GIR_FULL_LTH_NO_ITEMS 95
#define LRT_GIR_FULL_LTH_ITEM 53

static void UOD_Query(char *inbound) {

    LRT_GIQ* pGIQ = (LRT_GIQ*)inbound;
    LRT_GIR* pGIR = (LRT_GIR*)out;
    LONG lrc, count, item, recs, sofar, loop, use_first;
    URC urc;
    BYTE ubDateofDespatch[3];
    BYTE boots_code[4];
    BYTE bar_code[7];
    
    switch (pGIQ->bContType) {
    
    case 'C': // Container details

        urc = open_uodot();
        if (urc) {
            prep_nak("ERROR Unable to open UODOT - check event logs");
            break;
        }
        
        // Build UODOT key from ASCII key passed in GIQ
        pack(uodotrec.abLicence, 5, pGIQ->abCode+10, 10, 0);
        uodotrec.wSeqNo = satol(pGIQ->abSeq, 5);
        
        disp_msg("RD UODOT");
        lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
        if (lrc <= 0L) {
            prep_nak("ERROR Unable to read UODOT - check event logs");
            break;
        } else {

            // Build main part of GIR response
            memcpy(pGIR->abCmd, "GIR", 3);
            memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
            pGIR->bRespType = 'F';
            memcpy(pGIR->abCode, pGIQ->abCode+10, 10); // Requested code
            unpack(pGIR->abCode+10, 10, uodotrec.Record.Detail.abImmParentUOD, 5, 0 ); // Parent code

            unpack(pGIR->details.FullRec.abDespDate, 6, uodotrec.Record.Detail.ubDateofDespatch, 3, 0);
            pGIR->details.FullRec.bOuter = uodotrec.Record.Detail.bOuterType;
            pGIR->details.FullRec.bContType = (uodotrec.Record.Detail.bOuterType == 'D' ? 'C':'I'); /*1.6 BMG 25-03-2009*/
            pGIR->details.FullRec.bReason = uodotrec.Record.Detail.bReason;
            // If Partial then P = Partial otherwise,              /*1.6 BMG 25-03-2009*/
            // If audit then A = booked, U = unbooked
            // If not audit then B = booked, U = unbooked
            if (uodotrec.Record.Detail.uwPartialFlag) {            /*1.6 BMG 25-03-2009*/
                pGIR->details.FullRec.bStatus = 'P';               /*1.6 BMG 25-03-2009*/
            } else {                                               /*1.6 BMG 25-03-2009*/
                if (uodotrec.Record.Detail.uwAuditedFlag) {
                    pGIR->details.FullRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'A':'U');
                } else {
                    pGIR->details.FullRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'B':'U');
                }
            }                                                      /*1.6 BMG 25-03-2009*/
            pGIR->details.FullRec.bBOLUOD = (uodotrec.Record.Detail.ubBOLFlag ? 'Y':'N');
            memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
            pGIR->details.FullRec.bOrderSuf = 'X';
            pGIR->details.FullRec.bBC = 'X';
            unpack(pGIR->details.FullRec.abEstDelivDate, 6, uodotrec.Record.Detail.abEstDateDeliv, 3, 0);
            unpack(pGIR->details.FullRec.abDriver, 8, uodotrec.Record.Detail.abDriverID, 4,0);
            unpack(pGIR->details.FullRec.abDrivChkDate, 6, uodotrec.Record.Detail.abDateDriverCheckIn, 3, 0);
            unpack(pGIR->details.FullRec.abDrivChkTime, 4, uodotrec.Record.Detail.abTimeDriverCheckIn, 2, 0);
            unpack(pGIR->details.FullRec.abStoreOpId, 8, uodotrec.Record.Detail.abStoreOpID, 4, 0);
            unpack(pGIR->details.FullRec.abBookInDate, 6, uodotrec.Record.Detail.abDateBookedIn, 3, 0);
            unpack(pGIR->details.FullRec.abBookInTime, 4, uodotrec.Record.Detail.abTimeBookedIn, 2, 0);

            memcpy(ubDateofDespatch, uodotrec.Record.Detail.ubDateofDespatch, 3);

            recs = uodotrec.Record.Detail.wNumChildren;

            // Work out if we need to read a different UODOT chain to read the items from based on 15 items per chain
            // If the pointer is zero, no need to work it out
            sofar = satol(pGIQ->abPointer,sizeof(pGIQ->abPointer));
            if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) > 0) {
                // Work out which chain offset then read that UODOT record
                uodotrec.wSeqNo += (sofar / 15);
                disp_msg("RD UODOT");
                lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
                if (lrc <= 0L) {
                    prep_nak("ERROR Unable to read UODOT - check event logs");
                    break;
                }
            }

            count = 0;
            item = 0;

            while (count < 15 && sofar < recs) {

                // Buld item part of GIR message
                unpack(pGIR->details.FullRec.Items[count].abIdent, 10, uodotrec.Record.Detail.child[item].abLicence, 5, 0);
                memcpy(pGIR->details.FullRec.Items[count].abName, "             ", 13);
                pGIR->details.FullRec.Items[count].bBooked = ' ';
                pGIR->details.FullRec.Items[count].bContType = ' '; /*1.6 BMG 25-03-2009*/
                memcpy(pGIR->details.FullRec.Items[count].abDesc, "XXXXXXXXXXXXXXXXXXXX", 20);
                memcpy(pGIR->details.FullRec.Items[count].abQuantity,  "      ", 6);
                memcpy(pGIR->details.FullRec.Items[count].abSeq, "  ", 2);
                count++;
                sofar++;
                item++;
            }

            // Now loop through all output child records and obtain their details
            // based on license and despatch date (from parent UODOT above)

            for (loop=0;loop<count;loop++) {

                // Build UODOT key from Licence, starting at sequence 0
                pack(uodotrec.abLicence, 5, pGIR->details.FullRec.Items[loop].abIdent, 10, 0);
                uodotrec.wSeqNo = 0;

                lrc = 1;

                while (lrc > 0) {
                
                    disp_msg("RD UODOT");
                    lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
                    if (lrc > 0L) {
                        
                        if ( memcmp(uodotrec.Record.Detail.ubDateofDespatch, ubDateofDespatch, 3) ==0 ) {
                        
                            // We've got a match on licence and date of despatch so extract the details
                            memcpy(pGIR->details.FullRec.Items[loop].abName, "             ", 13);
                            pGIR->details.FullRec.Items[loop].abName[0] = uodotrec.Record.Detail.bOuterType;
                            pGIR->details.FullRec.Items[loop].bContType =(uodotrec.Record.Detail.bOuterType == 'D' ? 'C':'I'); /*1.6 BMG 25-03-2009*/
                            // If Partial then P = Partial otherwise,              /*1.6 BMG 25-03-2009*/
                            // If audit then A = booked, U = unbooked
                            // If not audit then B = booked, U = unbooked
                            if (uodotrec.Record.Detail.uwPartialFlag) {            /*1.6 BMG 25-03-2009*/
                                pGIR->details.FullRec.Items[loop].bBooked = 'P';   /*1.6 BMG 25-03-2009*/
                            } else {                                               /*1.6 BMG 25-03-2009*/
                                if (uodotrec.Record.Detail.uwAuditedFlag) {
                                    pGIR->details.FullRec.Items[loop].bBooked = (uodotrec.Record.Detail.uwBookedFlag ? 'A':'U');
                                } else {
                                    pGIR->details.FullRec.Items[loop].bBooked = (uodotrec.Record.Detail.uwBookedFlag ? 'B':'U');
                                }
                            }                                                      /*1.6 BMG 25-03-2009*/
                            sprintf(sbuf, "%06d", uodotrec.Record.Detail.wNumItems);
                            memcpy(pGIR->details.FullRec.Items[loop].abQuantity,  sbuf, 6);
                            sprintf(sbuf, "%02d", uodotrec.wSeqNo);
                            memcpy(pGIR->details.FullRec.Items[loop].abSeq, sbuf, 2);
                            break;
                        }
                    }
                    uodotrec.wSeqNo++;
                }
            }

            if (sofar == recs) {
                // No more items left so set the pointer to -1
                memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
            } else {
                // Set pointer to number of items so far (first item to read next time round)
                sprintf(sbuf, "%06ld", sofar);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            }
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        }
        break;

    case 'I': // Item Details

        urc = open_uodot();
        if (urc) {
            prep_nak("ERROR Unable to open UODOT - check event logs");
            break;
        }

        urc = open_uodin();
        if (urc) {
            prep_nak("ERROR Unable to open UODIN - check event logs");
            break;
        }
        
        // Build UODOT key from ASCII key passed in GIQ
        pack(uodotrec.abLicence, 5, pGIQ->abCode+10, 10, 0);
        uodotrec.wSeqNo = satol(pGIQ->abSeq, 5);

        disp_msg("RD UODOT");
        lrc = ReadUodotLog(__LINE__, LOG_CRITICAL);
        if (lrc <= 0L) {
            prep_nak("ERROR Unable to read UODOT - check event logs");
            break;
        } else {

            // Build main part of GIR response
            memcpy(pGIR->abCmd, "GIR", 3);
            memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
            pGIR->bRespType = 'F';
            memcpy(pGIR->abCode, pGIQ->abCode+10, 10); // Requested code
            unpack(pGIR->abCode+10, 10, uodotrec.Record.Detail.abImmParentUOD, 5, 0 ); // Parent code

            unpack(pGIR->details.FullRec.abDespDate, 6, uodotrec.Record.Detail.ubDateofDespatch, 3, 0);
            pGIR->details.FullRec.bOuter = uodotrec.Record.Detail.bOuterType;
            pGIR->details.FullRec.bContType =(uodotrec.Record.Detail.bOuterType == 'D' ? 'C':'I'); /*1.6 BMG 25-03-2009*/
            pGIR->details.FullRec.bReason = uodotrec.Record.Detail.bReason;
            // If Partial then P = Partial otherwise,              /*1.6 BMG 25-03-2009*/
            // If audit then A = booked, U = unbooked
            // If not audit then B = booked, U = unbooked
            if (uodotrec.Record.Detail.uwPartialFlag) {            /*1.6 BMG 25-03-2009*/
                pGIR->details.FullRec.bStatus = 'P';               /*1.6 BMG 25-03-2009*/
            } else {                                               /*1.6 BMG 25-03-2009*/
                if (uodotrec.Record.Detail.uwAuditedFlag) {
                    pGIR->details.FullRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'A':'U');
                } else {
                    pGIR->details.FullRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'B':'U');
                }
            }                                                      /*1.6 BMG 25-03-2009*/
            pGIR->details.FullRec.bBOLUOD = (uodotrec.Record.Detail.ubBOLFlag ? 'Y':'N');
            memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
            pGIR->details.FullRec.bOrderSuf = 'X';
            pGIR->details.FullRec.bBC = 'X';
            unpack(pGIR->details.FullRec.abEstDelivDate, 6, uodotrec.Record.Detail.abEstDateDeliv, 3, 0);
            unpack(pGIR->details.FullRec.abDriver, 8, uodotrec.Record.Detail.abDriverID, 4,0);
            unpack(pGIR->details.FullRec.abDrivChkDate, 6, uodotrec.Record.Detail.abDateDriverCheckIn, 3, 0);
            unpack(pGIR->details.FullRec.abDrivChkTime, 4, uodotrec.Record.Detail.abTimeDriverCheckIn, 2, 0);
            unpack(pGIR->details.FullRec.abStoreOpId, 8, uodotrec.Record.Detail.abStoreOpID, 4, 0);
            unpack(pGIR->details.FullRec.abBookInDate, 6, uodotrec.Record.Detail.abDateBookedIn, 3, 0);
            unpack(pGIR->details.FullRec.abBookInTime, 4, uodotrec.Record.Detail.abTimeBookedIn, 2, 0);


            recs = uodotrec.Record.Detail.wNumItems;

            // Build UODIN key
            memcpy(uodinrec.abLicence, uodotrec.abLicence, sizeof(uodotrec.abLicence));
            memcpy(uodinrec.abDespDate, uodotrec.Record.Detail.ubDateofDespatch, sizeof(uodotrec.Record.Detail.ubDateofDespatch));

            // Work out UODIN sequence to read based on 70 items per chain
            sofar = satol(pGIQ->abPointer,sizeof(pGIQ->abPointer));
            if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) == 0) {
                uodinrec.wSeqNo = 0;
            } else {
                if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) < 70) {
                    uodinrec.wSeqNo = 0;
                } else {
                    uodinrec.wSeqNo = (sofar / 70);
                }
            }

            disp_msg("RD UODIN");
            lrc = ReadUodinLog(__LINE__, LOG_CRITICAL);
            if (lrc <= 0L) {
                prep_nak("ERROR Unable to read UODIN - check event logs");
                break;
            } else {

                // Work out from which item in the record to start reading from
                if (sofar < 70) {
                   item = sofar;
                } else item = sofar%70;
            
                count = 0;
            
                while (count < 16 && sofar < recs) {

                    // Buld item part of GIR message
                    memcpy(pGIR->details.FullRec.Items[count].abIdent, "0000000000", 10);
                    calc_boots_cd(boots_code, uodinrec.Item[item].abItemCode);
                    unpack(pGIR->details.FullRec.Items[count].abIdent+2, 8, boots_code, 4, 0);

                    //*** IDF barcode and description
                    use_first = 1;
                    memcpy(idfrec.boots_code, boots_code, 4);
                    lrc = IdfRead(__LINE__);
                    if (lrc > 0L) {
                        memcpy(pGIR->details.FullRec.Items[count].abDesc, idfrec.stndrd_desc, 20);

                        memset(sbuf, 0x00, 6);
                        memcpy(sbuf+3, uodinrec.Item[item].abItemCode, 3);
                        if ( memcmp(idfrec.first_bar_code, sbuf, 6) == 0) {
                            // Barcode matches Boots code so use the 2nd barcode if not null
                            memset(sbuf, 0x00, 6);
                            if (memcmp(idfrec.second_bar_code, sbuf, 6) != 0) use_first = 0;
                        }
                        if (use_first) {
                            calc_ean13_cd(bar_code, idfrec.first_bar_code);
                        } else {
                            calc_ean13_cd(bar_code, idfrec.second_bar_code);
                        }
                        unpack(sbuf, 13, bar_code, 7, 1);
                        memcpy(pGIR->details.FullRec.Items[count].abName, sbuf , 13);
                    } else {
                        memcpy(pGIR->details.FullRec.Items[count].abDesc, "<No description>    ", 20);
                        memcpy(pGIR->details.FullRec.Items[count].abName, "0000000000000", 13);
                    }

                    pGIR->details.FullRec.Items[count].bBooked = pGIR->details.FullRec.bStatus;
                    pGIR->details.FullRec.Items[count].bContType = pGIR->details.FullRec.bContType; /*1.6 BMG 25-03-2009*/
                    sprintf(sbuf, "%06d", uodinrec.Item[item].wDespQty);                /*1.4 BMG 09-03-2009*/
                    memcpy(pGIR->details.FullRec.Items[count].abQuantity, sbuf , 6);
                    sprintf(sbuf, "%02d", uodotrec.wSeqNo);
                    memcpy(pGIR->details.FullRec.Items[count].abSeq, sbuf, 2);
                    count++;
                    item++;
                    sofar++;

                    if (item == 70) {
                        // We've hit the end of the items in this sequence so we need to move the sequence on
                        item = 0;
                        uodinrec.wSeqNo++;

                        disp_msg("RD UODIN");
                        lrc = ReadUodinLog(__LINE__, LOG_CRITICAL);
                        if (lrc <= 0L) {
                            prep_nak("ERROR Unable to read UODIN - check event logs");
                            break;
                        }
                    }
                }
            }
            
            if (sofar == recs) {
                // No more items left so set the pointer to -1
                memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
            } else {
                // Set pointer to number of items so far (first item to read next time round)
                sprintf(sbuf, "%06ld", sofar);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            }
            
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        }
        break;

    default:
        prep_nak("ERROR Malformed Transaction - GIQ Function");
        break;
    }

    close_uodot(CL_ALL);
    close_uodin(CL_ALL);

}

static void Directs_Query(char *inbound) {

    LRT_GIQ* pGIQ = (LRT_GIQ*)inbound;
    LRT_GIR* pGIR = (LRT_GIR*)out;
    LONG lrc, found, count, recs, loop, page, use_first;
    URC urc;
    WORD seq;
    BYTE bar_code[7];
    BYTE tbuf[8];
    
    switch (pGIQ->bContType) {
    
    case 'C': // Supplier Order Query

        urc = open_delvlist();
        if (urc) {
            prep_nak("ERROR Unable to open DELVLIST - check event logs");
            break;
        }

        urc = open_diror();
        if (urc) {
            prep_nak("ERROR Unable to open DIROR - check event logs");
            break;
        }

        count = 0;
        found = 0;
        pack(delvlistrec.abSupplier, 3, pGIQ->abCode, 6, 0);
        seq = satol(pGIQ->abPointer, sizeof(pGIQ->abPointer));
        delvlistrec.wSeq = seq;
        disp_msg("RD DELVLIST");
        lrc = ReadDelvlist(__LINE__);
        if (lrc <= 0L) {
            if ((lrc&0xFFFF)==0x06C8) { // Record not found
                found = 0;
            } else {
                prep_nak("ERROR Unable to read DELLIST - check event logs");
                break;
            }
        } else found = lrc;

        if (found == 0) {
            prep_nak("No records");
            break;
        }

        // Build main part of GIR response
        memcpy(pGIR->abCmd, "GIR", 3);
        memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
        pGIR->bRespType = 'F';
        memcpy(pGIR->abCode, pGIQ->abCode, 20);
        memcpy(pGIR->details.FullRec.abDespDate, "XXXXXX", 6);
        pGIR->details.FullRec.bOuter = 'X';
        pGIR->details.FullRec.bContType = 'X';
        pGIR->details.FullRec.bReason = 'X';
        pGIR->details.FullRec.bStatus = 'X';
        pGIR->details.FullRec.bBOLUOD = 'X';
        memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
        pGIR->details.FullRec.bOrderSuf = 'X';
        pGIR->details.FullRec.bBC = 'X';
        memcpy(pGIR->details.FullRec.abEstDelivDate, "XXXXXX",6);
        memcpy(pGIR->details.FullRec.abDriver, "XXXXXXXX", 8);
        memcpy(pGIR->details.FullRec.abDrivChkDate, "XXXXXX", 6);
        memcpy(pGIR->details.FullRec.abDrivChkTime, "XXXX", 4);
        memcpy(pGIR->details.FullRec.abStoreOpId, "XXXXXXXX", 8);
        memcpy(pGIR->details.FullRec.abBookInDate, "XXXXXX", 6);
        memcpy(pGIR->details.FullRec.abBookInTime, "XXXX", 4);
        
        while (found && count < 16) {

            // Read DIROR file from record found in DELVLIST and build detail part of GIR response

            // Prepare DIROR key
            memcpy(dirorrec.abSupplier, delvlistrec.abSupplier, 3);
            memcpy(dirorrec.abOrder, delvlistrec.abDirOrdNum, 2);
            dirorrec.bOrderSuf = delvlistrec.bDirOrdSuf;
            dirorrec.bBC = delvlistrec.bDirBC;
            dirorrec.bSource = delvlistrec.bDirSource;
            pack(dirorrec.abPageNo, 1, "00", 2, 0);

            // Don't do anything if no DIROR found, move to the next record
            lrc = ReadDiror(__LINE__);
            if (lrc > 0L) {
                // Build detail record part of GIR response
                memcpy(pGIR->details.FullRec.Items[count].abIdent, "          ", 10);
                memcpy(pGIR->details.FullRec.Items[count].abName, "             ", 13);
                unpack(pGIR->details.FullRec.Items[count].abName, 6,dirorrec.DirorRest.DirorHeader.abExpDelivDate, 3, 0);
                pGIR->details.FullRec.Items[count].bBooked = (dirorrec.DirorRest.DirorHeader.bConfirmFlag == ' ' ? 'U':'B');
                pGIR->details.FullRec.Items[count].bContType = 'X';

                // Build the DIROR key as an ASCII string in the description field
                unpack(pGIR->details.FullRec.Items[count].abDesc, 6, dirorrec.abSupplier, 3, 0);
                unpack(pGIR->details.FullRec.Items[count].abDesc+6, 4, dirorrec.abOrder, 2, 0);
                pGIR->details.FullRec.Items[count].abDesc[10] = dirorrec.bOrderSuf;
                pGIR->details.FullRec.Items[count].abDesc[11] = dirorrec.bBC;
                pGIR->details.FullRec.Items[count].abDesc[12] = dirorrec.bSource;
                memcpy(pGIR->details.FullRec.Items[count].abDesc+13, "00     ", 7);
                
                memcpy(pGIR->details.FullRec.Items[count].abQuantity, "XXXXXX", 6);
                memcpy(pGIR->details.FullRec.Items[count].abSeq, "XX", 2);
                count++;
            }
            
            // Check if there is another record
            found = 0;
            seq+=1;
            delvlistrec.wSeq = seq;
            disp_msg("RD NEXT DELVLIST");
            lrc = ReadDelvlist( __LINE__);
            if (lrc <= 0L) {
                if ((lrc&0xFFFF)==0x06C8) { // Record not found
                    found = 0;
                } else {
                    prep_nak("ERROR Unable to read DELVLIST - check event logs");
                    break;
                }
            } else found = lrc;
            if (found) {
                sprintf(sbuf, "%06d", seq);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            } else memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
        }

        // Write count to GIR
        if (count) {
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        } else prep_nak("No records");
        break;
    
    case 'I':

        urc = open_diror();
        if (urc) {
            prep_nak("ERROR Unable to open DIROR - check event logs");
            break;
        }

        // Build diror key from ASCII key passed in GIQ
        pack(dirorrec.abSupplier, 3, pGIQ->abCode, 6, 0);
        pack(dirorrec.abOrder, 2, pGIQ->abCode+6, 4, 0);
        dirorrec.bOrderSuf = pGIQ->abCode[10];
        dirorrec.bBC = pGIQ->abCode[11];
        dirorrec.bSource = pGIQ->abCode[12];
        
        // Work out DIROR page number to read
        if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) == 0) {
            // If the pointer is zero we actually want to read from page 01
            pack(dirorrec.abPageNo, 1, "01", 2, 0);
        } else {
            pack(dirorrec.abPageNo, 1, pGIQ->abPointer+4, 2, 0);
        }
        
        disp_msg("RD DIROR");
        lrc = ReadDirorLog(__LINE__, LOG_CRITICAL);
        if (lrc <= 0L) {
            prep_nak("ERROR Unable to read DIROR - check event logs");
            break;
        } else {

            // Build main part of GIR response
            memcpy(pGIR->abCmd, "GIR", 3);
            memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
            pGIR->bRespType = 'F';
            memcpy(pGIR->abCode, pGIQ->abCode, 20);
            memcpy(pGIR->details.FullRec.abDespDate, "XXXXXX", 6);
            pGIR->details.FullRec.bOuter = 'X';
            pGIR->details.FullRec.bContType = 'X';
            pGIR->details.FullRec.bReason = 'X';
            pGIR->details.FullRec.bStatus = 'X';
            pGIR->details.FullRec.bBOLUOD = 'X';
            memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
            pGIR->details.FullRec.bOrderSuf = 'X';
            pGIR->details.FullRec.bBC = 'X';
            memcpy(pGIR->details.FullRec.abEstDelivDate, "XXXXXX",6);
            memcpy(pGIR->details.FullRec.abDriver, "XXXXXXXX", 8);
            memcpy(pGIR->details.FullRec.abDrivChkDate, "XXXXXX", 6);
            memcpy(pGIR->details.FullRec.abDrivChkTime, "XXXX", 4);
            memcpy(pGIR->details.FullRec.abStoreOpId, "XXXXXXXX", 8);
            memcpy(pGIR->details.FullRec.abBookInDate, "XXXXXX", 6);
            memcpy(pGIR->details.FullRec.abBookInTime, "XXXX", 4);

            count = 0;
            unpack(sbuf, 2, dirorrec.DirorRest.DirorDetailRec.abItemCount, 1, 0);
            recs = satol(sbuf, 2);
            
            for (loop=0;loop<recs;loop++) {

                // Buld item part of GIR message
                memcpy(pGIR->details.FullRec.Items[count].abIdent, "0000000000", 10);
                unpack(pGIR->details.FullRec.Items[count].abIdent+2, 8, dirorrec.DirorRest.DirorDetailRec.DirorItem[loop].abBootsCode, 4, 0);

                //*** IDF barcode and description
                use_first = 1;
                memcpy(idfrec.boots_code, dirorrec.DirorRest.DirorDetailRec.DirorItem[loop].abBootsCode, 4);
                lrc = IdfRead(__LINE__);
                if (lrc > 0L) {
                    memcpy(pGIR->details.FullRec.Items[count].abDesc, idfrec.stndrd_desc, 20);

                    unpack(tbuf, 8, dirorrec.DirorRest.DirorDetailRec.DirorItem[loop].abBootsCode, 4, 0);
                    memset(sbuf, 0x00, 6);
                    pack(sbuf+3, 3, tbuf+1, 6, 0);
                    if ( memcmp(idfrec.first_bar_code, sbuf, 6) == 0) {
                        // Barcode matches Boots code so use the 2nd barcode if not null
                        memset(sbuf, 0x00, 6);
                        if (memcmp(idfrec.second_bar_code, sbuf, 6) != 0) use_first = 0;
                    }
                    if (use_first) {
                        calc_ean13_cd(bar_code, idfrec.first_bar_code);
                    } else {
                        calc_ean13_cd(bar_code, idfrec.second_bar_code);
                    }
                    unpack(sbuf, 13, bar_code, 7, 1);
                    memcpy(pGIR->details.FullRec.Items[count].abName, sbuf , 13);
                } else {
                    memcpy(pGIR->details.FullRec.Items[count].abDesc, "<No description>    ", 20);
                    memcpy(pGIR->details.FullRec.Items[count].abName, "0000000000000", 13);
                }

                pGIR->details.FullRec.Items[count].bBooked = 'X';
                pGIR->details.FullRec.Items[count].bContType = 'I';
                sprintf(sbuf, "%06d", dirorrec.DirorRest.DirorDetailRec.DirorItem[loop].wQtyExpected);
                memcpy(pGIR->details.FullRec.Items[count].abQuantity, sbuf , 6);
                unpack(sbuf, 2, dirorrec.abPageNo, 1, 0);
                memcpy(pGIR->details.FullRec.Items[count].abSeq, sbuf, 2);
                count++;
            }

            // Increment the page number and try and read the next in sequence in the DIROR
            unpack(sbuf, 2, dirorrec.abPageNo, 1, 0);
            page = satol(sbuf, 2);
            page++;
            sprintf(sbuf, "%02d", page);
            pack(dirorrec.abPageNo, 1, sbuf, 2, 0);

            disp_msg("RD DIROR");
            lrc = ReadDirorLog(__LINE__, LOG_CRITICAL);
            if (lrc <= 0L) {
                // No more pages so set the pointer to -1
                memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
            } else {
                // Set pointer to page number
                sprintf(sbuf, "%06ld", page);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            }
            
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        }
        break;
    
    default:
        prep_nak("ERROR Malformed Transaction - GIQ Function");
        break;
    }

    close_delvlist(CL_ALL);
    close_diror(CL_ALL);
}

static void ASN_Query(char *inbound) {

    LRT_GIQ* pGIQ = (LRT_GIQ*)inbound;
    LRT_GIR* pGIR = (LRT_GIR*)out;
    LONG lrc, found, count, recs, item, sofar, use_first;
    URC urc;
    WORD seq;
    BYTE boots_code[4];
    BYTE bar_code[7];
    
    switch (pGIQ->bContType) {
    
    case 'C': // Supplier Order Details

        urc = open_delvlist();
        if (urc) {
            prep_nak("ERROR Unable to open DELVLIST - check event logs");
            break;
        }

        urc = open_carton();
        if (urc) {
            prep_nak("ERROR Unable to open CARTON - check event logs");
            break;
        }

        count = 0;
        found = 0;
        pack(delvlistrec.abSupplier, 3, pGIQ->abCode+14, 6, 0);
        seq = satol(pGIQ->abPointer, sizeof(pGIQ->abPointer));
        delvlistrec.wSeq = seq;
        disp_msg("RD DELVLIST");
        lrc = ReadDelvlist(__LINE__);
        if (lrc <= 0L) {
            if ((lrc&0xFFFF)==0x06C8) { // Record not found
                found = 0;
            } else {
                prep_nak("ERROR Unable to read DELVLIST - check event logs");
                break;
            }
        } else found = lrc;

        if (found == 0) {
            prep_nak("No records");
            break;
        }

        // Build main part of GIR response
        memcpy(pGIR->abCmd, "GIR", 3);
        memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
        pGIR->bRespType = 'F';
        memcpy(pGIR->abCode, pGIQ->abCode, 20);
        memcpy(pGIR->details.FullRec.abDespDate, "XXXXXX", 6);
        pGIR->details.FullRec.bOuter = 'X';
        pGIR->details.FullRec.bContType = 'X';
        pGIR->details.FullRec.bReason = 'X';
        pGIR->details.FullRec.bStatus = 'X';
        pGIR->details.FullRec.bBOLUOD = 'X';
        memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
        pGIR->details.FullRec.bOrderSuf = 'X';
        pGIR->details.FullRec.bBC = 'X';
        memcpy(pGIR->details.FullRec.abEstDelivDate, "XXXXXX",6);
        memcpy(pGIR->details.FullRec.abDriver, "XXXXXXXX", 8);
        memcpy(pGIR->details.FullRec.abDrivChkDate, "XXXXXX", 6);
        memcpy(pGIR->details.FullRec.abDrivChkTime, "XXXX", 4);
        memcpy(pGIR->details.FullRec.abStoreOpId, "XXXXXXXX", 8);
        memcpy(pGIR->details.FullRec.abBookInDate, "XXXXXX", 6);
        memcpy(pGIR->details.FullRec.abBookInTime, "XXXX", 4);
        
        while (found && count < 16) {

            // Read CARTON file from record found in DELVLIST and build detail part of GIR response

            // Prepare CARTON key
            memcpy(cartonrec.abSupplier, delvlistrec.abSupplier, 3);
            memcpy(cartonrec.abCarton, delvlistrec.abCarton, 4);
            cartonrec.bChain = 0;
            
            // Don't do anything if no CARTON record found, move to the next record
            lrc = ReadCarton(__LINE__);
            if (lrc > 0L) {
                // Build detail record part of GIR response
                memcpy(pGIR->details.FullRec.Items[count].abIdent, "0000000000", 10);
                unpack(pGIR->details.FullRec.Items[count].abIdent+2, 8, cartonrec.abCarton, 4, 0);
                memcpy(pGIR->details.FullRec.Items[count].abName, cartonrec.abExpDeliv, 12);
                memcpy(pGIR->details.FullRec.Items[count].abName+12, " ", 1);
                if (cartonrec.bStatus == 'A') {                                         /*1.9 BMG 05-05-2009*/
                    pGIR->details.FullRec.Items[count].bBooked = cartonrec.bStatus;     /*1.9 BMG 05-05-2009*/
                } else {                                                                /*1.9 BMG 05-05-2009*/
                    pGIR->details.FullRec.Items[count].bBooked = (cartonrec.bStatus == 'U' ? 'U':'B');
                }                                                                       /*1.9 BMG 05-05-2009*/

                pGIR->details.FullRec.Items[count].bContType = 'I';
                memcpy(pGIR->details.FullRec.Items[count].abDesc, "XXXXXXXXXXXXXXXXXXXX", 20);
                sprintf(sbuf, "%06d", satol(cartonrec.abItemCount, 3));
                memcpy(pGIR->details.FullRec.Items[count].abQuantity, sbuf, 6);
                memcpy(pGIR->details.FullRec.Items[count].abSeq, "XX", 2);
                count++;
            }
            
            // Check if there is another record
            found = 0;
            seq+=1;
            delvlistrec.wSeq = seq;
            disp_msg("RD NEXT DELVLIST");
            lrc = ReadDelvlist( __LINE__);
            if (lrc <= 0L) {
                if ((lrc&0xFFFF)==0x06C8) { // Record not found
                    found = 0;
                } else {
                    prep_nak("ERROR Unable to read DELVLIST - check event logs");
                    break;
                }
            } else found = lrc;
            if (found) {
                sprintf(sbuf, "%06d", seq);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            } else memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
        }

        // Write count to GIR
        if (count) {
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        } else prep_nak("No records");
        break;
    
    case 'I': // Carton item details

        urc = open_carton();
        if (urc) {
            prep_nak("ERROR Unable to open CARTON - check event logs");
            break;
        }

        // Build CARTON key from ASCII key passed in GIQ
        pack(cartonrec.abSupplier, 3, pGIQ->abCode+2, 6, 0);
        pack(cartonrec.abCarton, 4, pGIQ->abCode+8, 8, 0);
        
        // Work out CARTON chain to read based on 60 items per chain
        // We will only pass up to 15 back to make calculations easier
        sofar = satol(pGIQ->abPointer,sizeof(pGIQ->abPointer));
        if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) == 0) {
            cartonrec.bChain = 0;
        } else {
            if (satol(pGIQ->abPointer, sizeof(pGIQ->abPointer)) < 60) {
                cartonrec.bChain = 0;
            } else {
                cartonrec.bChain = (sofar / 60);
            }
        }
        
        disp_msg("RD CARTON");
        lrc = ReadCartonLog(__LINE__, LOG_CRITICAL);
        if (lrc <= 0L) {
            prep_nak("ERROR Unable to read CARTON - check event logs");
            break;
        } else {

            // Build main part of GIR response
            memcpy(pGIR->abCmd, "GIR", 3);
            memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
            pGIR->bRespType = 'F';
            memcpy(pGIR->abCode, pGIQ->abCode, 20);
            memcpy(pGIR->details.FullRec.abDespDate, "XXXXXX", 6);
            pGIR->details.FullRec.bOuter = 'X';
            pGIR->details.FullRec.bContType = 'X';
            pGIR->details.FullRec.bReason = 'X';
            pGIR->details.FullRec.bStatus = 'X';
            pGIR->details.FullRec.bBOLUOD = 'X';
            memcpy(pGIR->details.FullRec.abOrderNum, "XXXXX", 5);
            pGIR->details.FullRec.bOrderSuf = 'X';
            pGIR->details.FullRec.bBC = 'X';
            memcpy(pGIR->details.FullRec.abEstDelivDate, "XXXXXX",6);
            memcpy(pGIR->details.FullRec.abDriver, "XXXXXXXX", 8);
            memcpy(pGIR->details.FullRec.abDrivChkDate, "XXXXXX", 6);
            memcpy(pGIR->details.FullRec.abDrivChkTime, "XXXX", 4);
            memcpy(pGIR->details.FullRec.abStoreOpId, "XXXXXXXX", 8);
            memcpy(pGIR->details.FullRec.abBookInDate, "XXXXXX", 6);
            memcpy(pGIR->details.FullRec.abBookInTime, "XXXX", 4);

            recs = satol(cartonrec.abItemCount,sizeof(cartonrec.abItemCount));

            // Work out from which item in the record to start reading from
            if (sofar < 60) {
               item = sofar;
            } else item = sofar%60;
            
            count = 0;
            
            while (count < 15 && sofar < recs) {

                // Buld item part of GIR message
                memcpy(pGIR->details.FullRec.Items[count].abIdent, "0000000000", 10);
                calc_boots_cd(boots_code, cartonrec.CartonItems[item].abItemCode);
                unpack(pGIR->details.FullRec.Items[count].abIdent+2, 8, boots_code, 4, 0);

                //*** IDF barcode and description
                use_first = 1;
                memcpy(idfrec.boots_code, boots_code, 4);
                lrc = IdfRead(__LINE__);
                if (lrc > 0L) {
                    memcpy(pGIR->details.FullRec.Items[count].abDesc, idfrec.stndrd_desc, 20);

                    memset(sbuf, 0x00, 6);
                    memcpy(sbuf+3, cartonrec.CartonItems[item].abItemCode, 3);
                    if ( memcmp(idfrec.first_bar_code, sbuf, 6) == 0) {
                        // Barcode matches Boots code so use the 2nd barcode if not null
                        memset(sbuf, 0x00, 6);
                        if (memcmp(idfrec.second_bar_code, sbuf, 6) != 0) use_first = 0;
                    }
                    if (use_first) {
                        calc_ean13_cd(bar_code, idfrec.first_bar_code);
                    } else {
                        calc_ean13_cd(bar_code, idfrec.second_bar_code);
                    }
                    unpack(sbuf, 13, bar_code, 7, 1);
                    memcpy(pGIR->details.FullRec.Items[count].abName, sbuf , 13);
                } else {
                    memcpy(pGIR->details.FullRec.Items[count].abDesc, "<No description>    ", 20);
                    memcpy(pGIR->details.FullRec.Items[count].abName, "0000000000000", 13);
                }
                if (cartonrec.bStatus == 'A') {                                         /*1.9 BMG 05-05-2009*/
                    pGIR->details.FullRec.Items[count].bBooked = cartonrec.bStatus;     /*1.9 BMG 05-05-2009*/
                } else {                                                                /*1.9 BMG 05-05-2009*/
                    pGIR->details.FullRec.Items[count].bBooked = (cartonrec.bStatus == 'U' ? 'U':'B');
                }                                                                       /*1.9 BMG 05-05-2009*/
                pGIR->details.FullRec.Items[count].bContType = 'I';
                if (cartonrec.bStatus == 'U') {                                         /*1.9 BMG 05-05-2009*/
                    sprintf(sbuf, "%06d", cartonrec.CartonItems[item].uwDesptchQty );   /*1.8 BMG 20-04-2009*/
                } else {                                                                /*1.9 BMG 05-05-2009*/
                    sprintf(sbuf, "%06d", cartonrec.CartonItems[item].uwBookedInQty );  /*1.9 BMG 05-05-2009*/
                }                                                                       /*1.9 BMG 05-05-2009*/
                memcpy(pGIR->details.FullRec.Items[count].abQuantity, sbuf , 6);
                memcpy(pGIR->details.FullRec.Items[count].abSeq, "XX", 2);
                count++;
                item++;
                sofar++;
            }
            
            if (sofar == recs) {
                // No more items left so set the pointer to -1
                memcpy(pGIR->details.FullRec.abPointer, "-1    ", 6);
            } else {
                // Set pointer to number of items so far (first item to read next time round)
                sprintf(sbuf, "%06ld", sofar);
                memcpy(pGIR->details.FullRec.abPointer, sbuf, 6);
            }
            
            sprintf(sbuf, "%02ld", count);
            memcpy(pGIR->details.FullRec.abCount, sbuf, 2);
            out_lth = LRT_GIR_FULL_LTH_NO_ITEMS + (LRT_GIR_FULL_LTH_ITEM * count);
        }
        break;
    
    default:
        prep_nak("ERROR Malformed Transaction - GIQ Function");
        break;
    }

    close_delvlist(CL_ALL);
    close_carton(CL_ALL);
}

void GIQ_Enquiry(char *inbound) {

    LRT_GIQ* pGIQ = (LRT_GIQ*)inbound;
    LRT_GIR* pGIR = (LRT_GIR*)out;
    LONG lrc, day, month, year, sec, found;
    URC urc;
    DOUBLE dDespDate, dHighDate, dEstDate, dTodayDate;
    WORD wSavedSeq, hour, min;
    
    //Initial checks                
    if (IsStoreClosed()) return;    
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();             

    switch (pGIQ->bType) {
    
    case '1': // SSC UOD Query

        if (pGIQ->bReqType == 'B') {                                            /*1.1 BMG 20-02-2009*/

            urc = open_uodot();
            if (urc) {
                prep_nak("ERROR Unable to open UODOT - check event logs");
                break;
            }
            
            // Find the newest UODOT entry matching the passed licence plate.
            // The result will always point to the correct record if two have the same date.
            // We use the delivery date rather than the despatch date because this is how
            // Expected and Outstanding orders are categorised in the store.
            pack(uodotrec.abLicence, 5, pGIQ->abCode+10, 10, 0);
            uodotrec.wSeqNo = 0;
            dHighDate = 0;
            lrc = 1;
            found = 0;

            while (lrc > 0) {
                disp_msg("RD UODOT");
                lrc = ReadUodot( __LINE__);
                if (lrc <= 0L) {
                    // Only report an error on sequence 0 as there should be at least one entry
                    if (uodotrec.wSeqNo == 0) {
                        if ((lrc&0xFFFF)==0x06C8) { // Record not found
                            prep_nak("ERROR UODOT Not On File");
                            break;
                        } else {
                            prep_nak("ERROR Unable to read UODOT - check event logs");
                            break;
                        }
                    }
                } else {
                    found = 1;
                    unpack( sbuf, 6, uodotrec.Record.Detail.abEstDateDeliv, 3, 0 );
                    day = satol(sbuf+4, 2);
                    month = satol(sbuf+2, 2);
                    year = satol(sbuf, 2);
                    dDespDate = ConvGJ( day, month, year );
                    if (dDespDate > dHighDate ) {
                        dHighDate = dDespDate;
                        wSavedSeq = uodotrec.wSeqNo;
                    }
                }
                uodotrec.wSeqNo++;
            }
            
            if (found) {
                uodotrec.wSeqNo = wSavedSeq;
                disp_msg("RD UODOT");
                lrc = ReadUodot( __LINE__);
                if (lrc <= 0L) {
                    if ((lrc&0xFFFF)==0x06C8) { // Record not found
                        prep_nak("ERROR UODOT Not On File");
                    } else {
                        prep_nak("ERROR Unable to read UODOT - check event logs");
                    }
                } else {
                    // Build main part of GIR response
                    memcpy(pGIR->abCmd, "GIR", 3);
                    memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
                    pGIR->bRespType = 'B';
                    memcpy(pGIR->abCode, pGIQ->abCode+10, 10); // Requested code
                    unpack(pGIR->abCode+10, 10, uodotrec.Record.Detail.abImmParentUOD, 5, 0 ); // Parent code

                    unpack(pGIR->details.BookingRec.abDespDate, 6, uodotrec.Record.Detail.ubDateofDespatch, 3, 0);
                    pGIR->details.BookingRec.bOuter = uodotrec.Record.Detail.bOuterType;
                    pGIR->details.BookingRec.bContType = (uodotrec.Record.Detail.bOuterType == 'D' ? 'C':'I');
                    pGIR->details.BookingRec.bReason = uodotrec.Record.Detail.bReason;
                    // If Partial then P = Partial otherwise,              /*1.6 BMG 25-03-2009*/
                    // If audit then A = booked, U = unbooked
                    // If not audit then B = booked, U = unbooked
                    if (uodotrec.Record.Detail.uwPartialFlag) {            /*1.6 BMG 25-03-2009*/
                        pGIR->details.BookingRec.bStatus = 'P';            /*1.6 BMG 25-03-2009*/
                    } else {                                               /*1.6 BMG 25-03-2009*/
                        if (uodotrec.Record.Detail.uwAuditedFlag) {
                            pGIR->details.BookingRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'A':'U');
                        } else {
                            pGIR->details.BookingRec.bStatus = (uodotrec.Record.Detail.uwBookedFlag ? 'B':'U');
                        }
                    }                                                      /*1.6 BMG 25-03-2009*/
                    pGIR->details.BookingRec.bBOLUOD = (uodotrec.Record.Detail.ubBOLFlag ? 'Y':'N');
                    sprintf(sbuf, "%05d", uodotrec.wSeqNo);
                    memcpy(pGIR->details.BookingRec.abOrderNum, sbuf, 5);

                    // Work out if this order is outstanding or expected
                    sysdate( &day, &month, &year, &hour, &min, &sec );
                    dTodayDate = ConvGJ( day, month, year );
                    sprintf(sbuf, "%ld", year);
                    // Note that the year is only 2 digits so have to set the first to digits from the system century for compare
                    unpack(sbuf+2, 6, uodotrec.Record.Detail.abEstDateDeliv, 3, 0 );
                    day   = satol( sbuf+6, 2 );                       
                    month = satol( sbuf+4, 2 );                       
                    year  = satol( sbuf, 4 );

                    dEstDate = ConvGJ( day, month, year );
                    if (dEstDate < dTodayDate) {                        //SDH 10A 8-Oct-2009
                        pGIR->details.BookingRec.bOrderSuf = 'O';       //SDH 10A 8-Oct-2009
                    } else if (dEstDate == dTodayDate) {                //SDH 10A 8-Oct-2009
                        pGIR->details.BookingRec.bOrderSuf = 'E';       //SDH 10A 8-Oct-2009
                    } else {                                            //SDH 10A 8-Oct-2009
                        pGIR->details.BookingRec.bOrderSuf = 'F';       //SDH 10A 8-Oct-2009
                    }                                                   //SDH 10A 8-Oct-2009
                    
                    pGIR->details.BookingRec.bBC = 'X';
                    out_lth = LRT_GIR_BOOKING_LTH;
                }
            }

        } else {
            if ( pGIQ->bReqType == 'F' && pGIQ->bSuppType == 'X') {         /*1.5 BMG 129-03-2009*/
                UOD_Query(inbound);
            } else {
                prep_nak("ERROR Malformed Transaction - GIQ Type");
            }
        }
        break;

    case '2': // Directs Query

        if (pGIQ->bFunc == '1' && pGIQ->bReqType == 'F' && pGIQ->bSuppType == 'D') {
            Directs_Query(inbound);
        } else {
            if (pGIQ->bFunc == '1' && pGIQ->bReqType == 'S') {
                
                urc = open_dirsu();
                if (urc) {
                    prep_nak("ERROR Unable to open DIRSU - check event logs");
                    break;
                }
                
                dirsurec.bBC = ' ';
                pack(dirsurec.abSupplier, 3, pGIQ->abCode+14, 6, 0);

                disp_msg("RD DIRSU");
                lrc = ReadDirsu( __LINE__);
                if (lrc <= 0L) {
                    if ((lrc&0xFFFF)==0x06C8) { // Record not found
                        prep_nak("ERROR Supplier Not On File");
                    } else {
                        prep_nak("ERROR Unable to read DIROR - check event logs");
                    }
                } else {
                    // Build main part of GIR response
                    memcpy(pGIR->abCmd, "GIR", 3);
                    memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
                    pGIR->bRespType = 'S';
                    memcpy(pGIR->abCode, pGIQ->abCode, 20);
                    memcpy(pGIR->details.SupplierRec.abSuppNum, pGIQ->abCode+14, 6);
                    memcpy(pGIR->details.SupplierRec.abSuppName, dirsurec.abSuppName, 10);
                    pGIR->details.SupplierRec.bSuppType = (dirsurec.bASNFlag == 'A' ? 'A':'D');
                    out_lth = LRT_GIR_SUPPLIER_LTH;
                }

            } else {
                prep_nak("ERROR Malformed Transaction - GIQ Type");
            }
        }
        break;

    case '3': // ASN Query

        if (pGIQ->bReqType == 'B') {

            urc = open_carton();
            if (urc) {
                prep_nak("ERROR Unable to open CARTON - check event logs");
                break;
            }

            pack(cartonrec.abSupplier, 3, pGIQ->abCode+2, 6, 0);
            pack(cartonrec.abCarton, 4, pGIQ->abCode+8, 8, 0);
            cartonrec.bChain = 0;

            disp_msg("RD CARTON");
            lrc = ReadCarton( __LINE__);
            if (lrc <= 0L) {
                if ((lrc&0xFFFF)==0x06C8) { // Record not found
                    prep_nak("ERROR Carton Not On File");
                } else {
                    prep_nak("ERROR Unable to read CARTON - check event logs");
                }
            } else {
                // Build main part of GIR response
                memcpy(pGIR->abCmd, "GIR", 3);
                memcpy(pGIR->abOpID, pGIQ->abOpID, 3);
                pGIR->bRespType = 'B';
                memcpy(pGIR->abCode, pGIQ->abCode, 20);
                //memcpy(pGIR->details.BookingRec.abDespDate, "XXXXXX", 6);             /*1.2 BMG 24-02-2009*/ 
                memcpy(pGIR->details.BookingRec.abDespDate, cartonrec.abExpDeliv+2, 6); /*1.2 BMG 24-02-2009*/ 
                pGIR->details.BookingRec.bOuter = 'X';
                pGIR->details.BookingRec.bContType = 'X';
                pGIR->details.BookingRec.bReason = 'X';
                if (cartonrec.bStatus == 'A') {                                         /*1.9 BMG 05-05-2009*/
                    pGIR->details.BookingRec.bStatus = cartonrec.bStatus;               /*1.9 BMG 05-05-2009*/
                } else {                                                                /*1.9 BMG 05-05-2009*/
                    pGIR->details.BookingRec.bStatus = (cartonrec.bStatus == 'U' ? 'U':'B');
                }                                                                       /*1.9 BMG 05-05-2009*/
                pGIR->details.BookingRec.bBOLUOD = 'X';
                memcpy(pGIR->details.BookingRec.abOrderNum, "XXXXX", 5);
                pGIR->details.BookingRec.bOrderSuf = 'X';
                pGIR->details.BookingRec.bBC = cartonrec.bBusCentre;
                out_lth = LRT_GIR_BOOKING_LTH;
            }

        } else {
            if ( (pGIQ->bFunc == '2' || pGIQ->bFunc == '3') && pGIQ->bReqType == 'F' && pGIQ->bSuppType == 'A') {
                ASN_Query(inbound);
            } else {
                prep_nak("ERROR Malformed Transaction - GIQ Type");
            }
        }
        break;

    default:
        prep_nak("ERROR Malformed Transaction - GIQ Type");
        break;
    }

    close_uodot(CL_ALL);
    close_dirsu(CL_ALL);
    close_carton(CL_ALL);

}


// ------------------------------------------------------------------------
// GIX
// ------------------------------------------------------------------------

typedef struct {
    BYTE abCmd[3];
    BYTE abOpID[3];
    BYTE bType;         // X = logon pad, 1 = SSC, 2 = Directs, 3 = ASN
    BYTE bFunc;         // 1 = Booking in, 2 = Audit, 3 = View
    BYTE bAbort;        // Y = Abort, N = Complete
} LRT_GIX;
#define LRT_GIX_LTH sizeof(LRT_GIX)

void GIX_End(char *inbound) {

    LRT_GIX* pGIX = (LRT_GIX*)inbound;
    LONG    *plFileNum = NULL;
    WORD    *pwPQRec = NULL;
    PROG_PQ *pPQ = NULL;

    //Initial checks                
    if (IsStoreClosed()) return;    
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();             

    switch (pGIX->bType) {
    
    case '1': //SSC

        // Close files just in case
        close_delvindx(CL_ALL);
        close_uodot(CL_ALL);
        close_uodin(CL_ALL);

        if (pGIX->bAbort == 'Y') {
            // Abort so remove the buffer file for this session
            pwPQRec = &lrtp[hh_unit]->pq_PUB;
            pPQ = &pq[*pwPQRec];
            plFileNum = &lrtp[hh_unit]->lPUBfnum;
            s_close (0, *plFileNum);    
            log_file_close (*plFileNum);
            s_delete(0, pPQ->fname);
            lrtp[hh_unit]->pq_PUB = 0;  
            lrtp[hh_unit]->lPUBfnum = 0;
            prep_ack( "" );
        } else {
            process_workfile(hh_unit, SYS_PUB);
            prep_ack( "" );
        }
        break;

    case '2': // Directs

        // Close files just in case
        close_carton(CL_ALL);
        close_diror(CL_ALL);
        close_delvsmry(CL_ALL);
        close_delvlist(CL_ALL);
        close_dirsu(CL_ALL);

        if (pGIX->bAbort == 'Y') {
            // Abort so remove the buffer file for this session
            pwPQRec = &lrtp[hh_unit]->pq_DIR;
            pPQ = &pq[*pwPQRec];
            plFileNum = &lrtp[hh_unit]->lDIRfnum;
            s_close (0, *plFileNum);    
            log_file_close (*plFileNum);
            s_delete(0, pPQ->fname);
            lrtp[hh_unit]->pq_DIR = 0;  
            lrtp[hh_unit]->lDIRfnum = 0;
            prep_ack( "" );
        } else {
            process_workfile(hh_unit, SYS_DIR);
            prep_ack( "" );
        }
        break;

    case '3': // ASNs
        
        // Close files just in case
        close_carton(CL_ALL);
        close_diror(CL_ALL);
        close_delvsmry(CL_ALL);
        close_delvlist(CL_ALL);
        close_dirsu(CL_ALL);

        if (pGIX->bAbort == 'Y') {
            // Abort so remove the buffer file for this session
            pwPQRec = &lrtp[hh_unit]->pq_CB;
            pPQ = &pq[*pwPQRec];
            plFileNum = &lrtp[hh_unit]->lCBfnum;
            s_close (0, *plFileNum);    
            log_file_close (*plFileNum);
            s_delete(0, pPQ->fname);
            lrtp[hh_unit]->pq_CB = 0;  
            lrtp[hh_unit]->lCBfnum = 0;
            prep_ack( "" );
        } else {
            process_workfile(hh_unit, SYS_CB);
            prep_ack( "" );
        }
        break;
    
    default:
        prep_nak("ERROR Malformed Transaction - GIX Type");
        break;
    }
}
