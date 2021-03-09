/*************************************************************************
*
* File: trxbase.c
*
* Revision 1         Brian Greenfield           24-02-2009
* Removed the fact that if RFSCF was phase 5 it didn't lock the
* CCDMY file. this is no longer required.
*
* Revision 2         Brian Greenfield           04-03-2009
* Always set planners to be Y in SNR. This is a change to stop
* issues with this flag being reset in the RFSCF but planners are
* live in all stores now and this will stop some issues.
* Also in PLC message if the workfile is not created, create it.
* This can happen because MC70's don't send PLS messages.
*
* Revision 3        Brian Greenfield            09-03-2009
* Within PListUpdateItem if the device is an MC70 it now just
* outputs the list ID to the file rather than the item number.
* PST47(which processes the file)  processes by list.
*
* Revision 4        Brian Greenfield            25-03-2009
* Removed the updating of the RFSCF within GapSignOff and
* PriceCheckSignOff as no longer required. MC70's now send ENQ's
* which do the updates.
* Also changed process_gap so we write the output file twice for
* MC70 so the launcher doesn;t delete the file if only one record!
*
* Revision 5        Brian Greenfield            7th April 2009
* Removed PLC code whereby weekends are taken into account for
* delivery date checking on counts. This differed from the controller
* and was specifically asked to be removed as part of the SFSC2
* project. When +ve UPD is activated, the INVOK report days will
* be dropped to 0 so effectively no delivery date checking will be done.
*
* Revision 6        Brian Greenfield            16th June 2009
* Minor change to not check Planner Suite status for a Goods In
* application SOR (Goods In ID is 5.)
*
* Revision 7        Stuart Highley             20-May-2009
* Model Day Improvements changes.  Auto Fast Fill, Auto Fill Qty,
* Stuff Your Shelves. Commented Model Day.
*
* Revision 8        Brian Greenfield           17-Dec 2009
* Changes for RF Stabilisation. New ANK internal command.
*
* Revision 9        Charles Skadorwa           15-Apr-2010
* Changes to support POD Logging Enhancements.
*
* Revision 10       Charles Skadorwa           11-May-2010
* Changes to support Auto Fast Fill - adding Product Group
* and Recall Type to the EQR transaction.
*
* Revision 11       Brian Greenfield           25th June 2010
* Changes to RCN to also test Operator ID when validating PLLOL list status.
*
* Revision 12       Brian Greenfield           30th June 2010
* Further tweaks to RCN to also block unwanted list status connections.
*
* Revision 13       Charles Skadorwa           12-Mar-2012
* Stock File Accuracy - added stock adjustment flag to SNR message.
* Updated SignOn() to include stock adjustment flag.
* Increased sites from 10 to 32.
* Updated CLC messsage
* Updated EQR messsage - added cPendSaleFlag
* Updated MSB messsage
*
* Revision 14       Tittoo Thomas              14th Aug 2012
* Stock File Accuracy  (commented: //TAT 14-08-2012 SFA)
* Fixed to update CLILF record without locking.
*
* Revision 15       Tittoo Thomas              16th Aug 2012
* Stock File Accuracy  (commented: //TAT 16-08-2012 SFA)
* Fixed to update CLOLF record LIST.STATUS to P on a CLX with a commit,
* if not all counts are counted.
*
* Revision 16       Tittoo Thomas              20th Aug 2012
* Stock File Accuracy  (commented: //TAT 20-08-2012 SFA)
* Fixed to make the count a LONG value instead of WORD to suite the
* STKMQ type 13 format.(QC Defect : 493)
*
* Revision 16       Tittoo Thomas              23rd Aug 2012
* Stock File Accuracy  (commented: //TAT 23-08-2012 SFA)
* Fixed to insert supporting record in MINLS for Stock support to pick the
* STKMQ entried for count updates.(QC Defect : 493, 550)
*
* Revision 17       Tittoo Thomas              11th Sep 2012
* Stock File Accuracy  (commented: //TAT 11-09-2012 SFA)
* Fixed to update the count start time and also correctly update the
* count end time.(QC Defect : 653)
* Also modified the NAKERROR message in case of an unknown item added
* to a User generated list. (QC Defect : 643, 645)
*
* Revision 18       Tittoo Thomas              20th Sep 2012
* Stock File Accuracy  (commented: //TAT 20-09-2012 SFA)
* Fixed to include the PILST id and PIITM number in head office counts
* while updating the STKMQ.(QC Defect : 698)
*
* Revision 19       Tittoo Thomas              02nd Nov 2012
* Stock File Accuracy  (commented: //TAT 02-11-2012 SFA)
* Fixed to clear out the PILST id and PIITM number fields for non
* head office counts while updating the STKMQ.
* Also fixed to ensure only fully counted lists are set as counted.
*
* Revision 20       Tittoo Thomas              12th Nov 2012
* Stock File Accuracy  (commented: //TAT 12-11-2012 SFA)
* Fixed to always set a count received through picking as a recount, else
* the counts will be rejected by the stock support if a recount is due.
* Also merged in from production (commented: KK 16-Oct-2012):
* Fix for picking list locked issue.In sign on function the list
*  status is made from "A" to "U" if the following conditions
* are satisfied :
* -The picker id is 000
* -Creator id is same as current sign in id
* -List status is "A"
* In reconnect function the list status is made from "U" to "A" if
* the following conditions are satisfied
* -The picker id is 000
* -Creator id is same as current sign in id
* -List status is "U"
*
* Revision 21       Tittoo Thomas              16th Nov 2012
* Stock File Accuracy  (commented: //TAT 16-11-2012 SFA)
* Fixed issue with STKMQ update Defect.
* Temporary fix for defect 799.
*
* Revision 22       Tittoo Thomas              26th Nov 2012
* Stock File Accuracy  (commented: //TAT 23-11-2012 SFA)
* Fixed to check for active count lists and set them to partially
* counted only for a proper signon and not during a reconnect.
* Also fixed transact to set the count as a Recount for all counts except
* for support office count lists. (SFA defect 813)
* Also fix for SFA defect 799.
*
* Revision 22       Tittoo Thomas              03rd Dec 2012
* Stock File Accuracy  (commented: //TAT 03-12-2012 SFA)
* Updated the CLL message with the outstanding shop floor, Back shop
* and OSSR counts instead of total shop floor, back shop or OSSR
* counts. (SFA defect 823)
*
* Revision 23       Tittoo Thomas              06th Dec 2012
* Stock File Accuracy  (commented: //TAT 06-12-2012 SFA)
* Fixed the CLI message sent for a CLD to use the item description
* for an item with SEL description marked with a "X " or "x "
* (SFA defect 821)
*
* Revision 24       Tittoo Thomas              13th Dec 2012
* Stock File Accuracy  (commented: //TAT 13-12-2012 SFA)
* Fixed the processing of a CLD to not increment the total item count
* in case a CLILF item is overwritten.
* (SFA defect 837)
*
* Revision 25       Tittoo Thomas              18th Dec 2012
* Stock File Accuracy  (commented: //TAT 18-12-2012 SFA)
* Fixed the processing of a CLX to decrement the outstanding number of
* shop floor, back shop and ossr counts as well, while removing an item
* not counted from CLILF.
* (SFA defect 837)
*
* Revision 26       Tittoo Thomas              24th Apr 2013
* Stock File Accuracy  (commented: //TAT 24-04-2013 SFA)
* Changes to fix UAT defect 25. Sets a list status to active only if it
* is not already active and also not already completed.
*
* Revision 27       Rejiya Nair                28th June 2013
* Event Log Rationalisation  (commented: // RJN 28-06-2013 ELR)
* Changes to remove the unnecessary application events logged from
* TRANSACT that are not supposed to be actually logged.
*
** Revision A        Charles Skadorwa           25th June 2015
* BAU Defect: PRB0044559 - RF Reporting for POD stores showing Unknown
*                          User.
* On POD batch devices the Picker ID is being incorrectly set to 000.
* The PListUpdateItem() function has been modified to assign the current
* operator ID to the Picker ID in the PLLOL record.
*
* Version A        Arun Venugopalan           21st October 2015
* HHT Date and Time sync issue: PRB0045752
*
* Changed to perform the automation of HHT date and time synchronisation
* from the controller as soon as the HHT device is rebooted. When
* Transact receives a SOR message with application id 6(Date and Time)
* a SNR message with current controller date and time will be sent to
* HHT device.
*
* Version B       Ranjith Gopalankutty        07th July  2016 
* Reverted the previous version changes done to get the Automatic date 
* time. New function has been created to handle the dummy signon request
* so it doesn't need to handled here. New module trxbase2.c has been 
* created for the same.
*
*************************************************************************/

#include "transact.h"
#include <string.h>
#include "osfunc.h"
#include "prtctl.h"
#include "prtlist.h"
#include "srfiles.h"
#include "idf.h"
#include "irf.h"
#include "isf.h"
#include "rfhist.h"
#include "bcsmf.h"
#include "pgf.h"
#include "invok.h"
#include "rfscf.h"
#include "af.h"
#include "ccfiles.h"
#include <math.h>
#include "clfiles.h"
#include "trxrcall.h"
#include "seldesc.h"
#include "phf.h"
#include "iudf.h"                         // RN 28-06-2013 EVR
#include "trxfile.h"
#include "trxutil.h"
#include "podlog.h"                       //CSk 15-04-2010 POD Logging
#include "podok.h"                        //CSk 15-04-2010 POD Logging

typedef struct {                          // BMG 03-Jan-2008 1.2
   BYTE cmd[3];
   BYTE opid[3];
   BYTE anFilePath[64];
   BYTE cCreateFile[1];
   BYTE anDataLength[4];
   BYTE abData[2000];
} LRT_WRF;
#define LRT_WRF_LTH sizeof(LRT_WRF)


//////////////////////////////////////////////////////////////////////////////////
///
///   Private structures
///
//////////////////////////////////////////////////////////////////////////////////

typedef struct {
   BYTE cmd[3];
   BYTE msg[150];                             // 25-5-07 PAB Recalls
} LRT_NAK;                                    // -ve acknowledge
#define LRT_NAK_LTH sizeof(LRT_NAK)

// LRTLG details field overlays - dependant on type
typedef struct {
    BYTE authority[1];                                                      // X = Unauthorised
    BYTE resv[11];
} LRTLG_SOR;

typedef struct  {
    BYTE boots_code[4];
    BYTE check_type[1];                                                     // 26-7-2004 PAB
    BYTE resv[7];
} LRTLG_PCM;
typedef struct  {
    LONG gaps_identified;
    BYTE resv[8];
} LRTLG_GAX;
typedef struct  {
    LONG items_checked;
    LONG sels_printed;
    BYTE resv[4];
} LRTLG_PCX;
typedef struct  {
    BYTE resv[12];
} LRTLG_PRT;
typedef struct  {
    BYTE enqpc[1];
    BYTE resv[11];
} LRTLG_ENQ;
typedef struct  {
    BYTE abListID[3];
    BYTE bComplete;
    BYTE bListType;
    BYTE abFiller[7];
} LRTLG_PLX;
// v4.0 START
typedef struct  {
    BYTE list_id[4];
    BYTE resv[8];
} LRTLG_CLS;
typedef struct  {
    BYTE list_id[4];
    BYTE boots_code[7];
    BYTE resv[1];
} LRTLG_CLC;
//zzzz typedef struct  {                             //CSk 12-03-2012 SFA
//zzzz     BYTE abListID[3];                         //CSk 12-03-2012 SFA
//zzzz     BYTE abTotalItems[3];                     //CSk 12-03-2012 SFA
//zzzz     BYTE bListType;                           //CSk 12-03-2012 SFA
//zzzz } LRTLG_CLL;                                  //CSk 12-03-2012 SFA
typedef struct  {
    BYTE list_id[4];
    BYTE commit_flag[1];
    BYTE resv[7];
} LRTLG_CLX;
typedef struct  {
    BYTE txnid[4];
    BYTE fail[1];
} LRTLG_SUS;
// v4.0 END


//////////////////////////////////////////////////////////////////////////////////
///
///   process_gap
///
///   Called from CMD_GAP to perform the majority of the gap processing
///
//////////////////////////////////////////////////////////////////////////////////

static URC process_gap( BYTE *pass_list_id, BYTE *seq,
                 BYTE *item_code_unp, BYTE *boots_code_unp,
                 BYTE *current, BYTE *fill_qty, BYTE gap_flag,
                 BYTE cUpdateOssritm, BYTE *MSLocation, UWORD log_unit ) {  // 17-11-04 SDH OSSR WAN // 13-08-2008 16 BMG

    LONG ws_item_count, rc, rec;
    BYTE boots_code[4];                                                     // BCD incl cd  (0c cc cc cd)
    //RFSCF_REC_1AND2 rfscfrec1and2;                                        // 16-11-04 SDH
    URC urc;
    //BYTE gapbfrec[32];
    //WORD dest_lth = 1;
    ENQUIRY enqbuf;
    //TIMEDATE now;                                                         // 31-8-04 PAB OSSR
    BYTE sbuf[64];                                                          // 31-8-04 PAB OSSR
    //WORD hour, min;                                                       // 31-8-04 PAB OSSR
    B_TIME nowTime;                                                         // 16-11-04 SDH
    B_DATE nowDate;                                                         // 16-11-04 SDH
    BYTE abDatePD[4];                                                       // 16-11-04 SDH
    BYTE abTimePD[2];                                                       // 16-11-04 SDH
    BOOLEAN fUpdateRfhist = FALSE;                                          // 16-11-04 SDH
    BYTE bLocation;                                                         // 22-02-05 SDH EXCESS
    BOOLEAN fNewRecord = TRUE;                                              // 13-09-05 SDH RECOUNT
    BYTE loop;                                                              // 13-08-2008 16 BMG
    WORD wMSLoc;                                                            // 13-08-2008 16 BMG

    //Avoid compiler warning                                                // 16-11-04 SDH
    UNUSED(log_unit);                                                       // 16-11-04 SDH

    //Get date/time                                                         // 16-11-04 SDH
    GetSystemDate(&nowTime, &nowDate);                                      // 16-11-04 SDH
    sprintf(sbuf, "%04d%02d%02d", nowDate.wYear,                            // 16-11-04 SDH
            nowDate.wMonth, nowDate.wDay);                                  // 16-11-04 SDH
    pack(abDatePD, 4, sbuf, 8, 0);                                          // 16-11-04 SDH
    sprintf(sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);                 // 16-11-04 SDH
    pack(abTimePD, 2, sbuf, 4, 0);                                          // 16-11-04 SDH

    //Find out if this will be a new PLLDB record                           // 13-09-05 SDH RECOUNT
    // You need to set up the key before you attempt a read or it will      // PAB 20-7-07
    // always return false.                                                 // PAB 20-7-07
    memcpy(plldbrec.list_id, pass_list_id, 3);                              // PAB 20-7-07
    memcpy(plldbrec.seq, seq, 3);                                           // PAB 20-7-07
    rc = ReadPlldbLog(__LINE__, LOG_CRITICAL);                              // 13-09-05 SDH RECOUNT
    if (rc > 0) fNewRecord = FALSE;                                         // 13-09-05 SDH RECOUNT

    //Set up packed boots code
    memset( boots_code, 0x00, 4 );
    pack( boots_code, 4, boots_code_unp, 7, 1 );

    //Get item details
    urc = stock_enquiry( SENQ_TSF, (BYTE *)item_code_unp, &enqbuf );
    if (urc!=RC_OK) return urc;

    // Open the Control File to get RF phase                                // 16-11-04 SDH
    //Read RFSCF record 1                                                   // 16-11-04 SDH
    urc = RfscfOpen();                                                      // 07-04-04 PAB
    if (urc != RC_OK) return urc;                                           // 07-04-04 PAB
    rc = RfscfRead(0L, __LINE__);                                           // Streamline SDH 17-Sep-2008
    rc = RfscfRead(1L, __LINE__);                                           // Streamline SDH 17-Sep-2008
    if (rc <= 0L) return RC_DATA_ERR;                                       // 16-11-04 SDH
    urc = RfscfClose(CL_SESSION);                                           // 07-04-04 PAB
    //write_gap = satol(rfscfrec1and2.phase,1);                             // Streamline SDH 23-Sep-2008

    //Read RFHIST                                                           // SDH 17-11-04 OSSR WAN
    //Missing RFHIST Records will not be tolerated - since the ENQ          // SDH 17-11-04 OSSR WAN
    //should have added one                                                 // SDH 17-11-04 OSSR WAN
    memcpy(rfhistrec.boots_code, boots_code, sizeof(rfhistrec.boots_code)); // SDH 17-11-04 OSSR WAN
    rc = RfhistRead(__LINE__);                                              // SDH 17-11-04 OSSR WAN
    if (rc <= 0) return RC_DATA_ERR;                                        // SDH 17-11-04 OSSR WAN

    //Ensure that the RFHIST OSSR item flag is up to date                   // SDH 17-11-04 OSSR WAN
    if ((rfscfrec1and2.ossr_store == 'W') &&                                // SDH 17-11-04 OSSR WAN
        (cUpdateOssritm != ' ')) {                                          // SDH 17-11-04 OSSR WAN
        rfhistrec.ubItemOssrFlag = (cUpdateOssritm == 'O' ? TRUE:FALSE);    // SDH 19-03-05 OSSR WAN
        fUpdateRfhist = TRUE;                                               // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    //Determine the handheld's current location                             // SDH 22-02-05 EXCESS
    if (gap_flag == 'B') {                                                  // SDH 22-02-05 EXCESS
        bLocation  = 'B';                                                   // SDH 22-02-05 EXCESS
        gap_flag = 'Y';                                                     // SDH 22-02-05 EXCESS
    } else if (gap_flag == 'O') {                                           // SDH 22-02-05 EXCESS
        bLocation = 'O';                                                    // SDH 22-02-05 EXCESS
        gap_flag = 'Y';                                                     // SDH 22-02-05 EXCESS
    } else {                                                                // SDH 22-02-05 EXCESS
        bLocation = 'S';                                                    // SDH 22-02-05 EXCESS
    }                                                                       // SDH 22-02-05 EXCESS
    lrtp[log_unit]->bLocation = (bLocation == 'O' ? 'O':'S');               // SDH 22-02-05 EXCESS

    //Update RFHIST if required                                             // SDH 17-11-04 OSSR WAN
    if (fUpdateRfhist) {                                                    // SDH 17-11-04 OSSR WAN
        rc = RfhistWrite(__LINE__);                                         // SDH 17-11-04 OSSR WAN
        if (rc <= 0) return RC_DATA_ERR;                                    // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    // Build the PLLDB record                                               // SDH 22-02-05 EXCESS
    // Only reset the values if the record does not yet exist               // 13-08-2008 16 BMG
    if (fNewRecord) {                                                       // 13-08-2008 16 BMG
       memset(&plldbrec, '0', sizeof(plldbrec));                            // SDH 22-02-05 EXCESS
       memset(plldbrec.abPendingBSTime,   0xFF, sizeof(plldbrec.abPendingBSTime));   //CSk 12-03-2012 SFA
       memset(plldbrec.abPendingOSSRTime, 0xFF, sizeof(plldbrec.abPendingOSSRTime)); //CSk 12-03-2012 SFA
       memset(plldbrec.time_sf_count, 0xFF, sizeof(plldbrec.time_sf_count));// SDH 22-02-05 EXCESS
       memset(plldbrec.time_bs_count, 0xFF, sizeof(plldbrec.time_bs_count));// SDH 22-02-05 EXCESS
       memset(plldbrec.time_ossr_count, 0xFF,                               // SDH 22-02-05 EXCESS
              sizeof(plldbrec.time_ossr_count));                            // SDH 22-02-05 EXCESS
       for (loop=0;loop<33;loop++) {                                        // 13-08-2008 16 BMG //CSk 12-03-2012 SFA
           memset(plldbrec.aMS_details[loop].ms_loc_count, 0x00,            // 13-08-2008 16 BMG
                  sizeof(plldbrec.aMS_details->ms_loc_count));              // 13-08-2008 16 BMG
           memset(plldbrec.aMS_details[loop].ms_sales_fig, 0x00,            // 13-08-2008 16 BMG
                  sizeof(plldbrec.aMS_details->ms_sales_fig));              // 13-08-2008 16 BMG
           memset(plldbrec.aMS_details[loop].ms_time_of_count, 0xFF,        // 13-08-2008 16 BMG
                  sizeof(plldbrec.aMS_details->ms_time_of_count));          // 13-08-2008 16 BMG
           memset(plldbrec.aMS_details[loop].ms_fill_qty, 0x00,             // 13-08-2008 16 BMG
                  sizeof(plldbrec.aMS_details->ms_fill_qty));               // 13-08-2008 16 BMG
           memset(plldbrec.aMS_details[loop].ms_filler, 'X',                // 13-08-2008 16 BMG
                  sizeof(plldbrec.aMS_details->ms_filler));                 // 13-08-2008 16 BMG
       }                                                                    // 13-08-2008 16 BMG
//XXX       memset(plldbrec.filler, 'X', sizeof(plldbrec.filler));               // SDH 22-02-05 EXCESS
       memcpy(plldbrec.list_id, pass_list_id, 3);                           // SDH 22-02-05 EXCESS
       memcpy(plldbrec.seq, seq, 3);                                        // SDH 22-02-05 EXCESS
       memcpy(plldbrec.boots_code, boots_code, 4);                          // SDH 22-02-05 EXCESS
       plldbrec.gap_flag[0] = gap_flag;                                     // SDH 22-02-05 EXCESS
    }                                                                       // 13-08-2008 16 BMG
    memcpy(plldbrec.sales_figure, (enqbuf.items_sold_today)+2, 4 );         // SDH 22-02-05 EXCESS

    if (memcmp(MSLocation, "  ", 2) == 0) {                                     // 13-08-2008 16 BMG
        //Not a multi-site count so update base fields                          // 13-08-2008 16 BMG
        if (bLocation == 'S') {                                                 // SDH 22-02-05 EXCESS
            memcpy(plldbrec.qty_on_shelf, current,                              // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.qty_on_shelf));                              // SDH 22-02-05 EXCESS
            memcpy(plldbrec.fill_qty, fill_qty, sizeof(plldbrec.fill_qty));     // SDH 22-02-05 EXCESS
            memcpy(plldbrec.time_sf_count, abTimePD,                            // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.time_sf_count));                             // SDH 22-02-05 EXCESS
            memcpy(plldbrec.sales_figure, (enqbuf.items_sold_today)+2,          // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.sales_figure));                              // SDH 22-02-05 EXCESS
        } else if (bLocation == 'B') {                                          // SDH 22-02-05 EXCESS
            memcpy(plldbrec.stock_room_count, current,                          // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.qty_on_shelf));                              // SDH 22-02-05 EXCESS
            memcpy(plldbrec.time_bs_count, abTimePD,                            // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.time_bs_count));                             // SDH 22-02-05 EXCESS
            memcpy(plldbrec.sales_bs_count, (enqbuf.items_sold_today)+2,        // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.sales_bs_count));                            // SDH 22-02-05 EXCESS
        } else if (bLocation == 'O') {                                          // SDH 22-02-05 EXCESS
            memcpy(plldbrec.ossr_count, current, sizeof(plldbrec.ossr_count));  // SDH 22-02-05 EXCESS
            memcpy(plldbrec.time_ossr_count, abTimePD,                          // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.time_ossr_count));                           // SDH 22-02-05 EXCESS
            memcpy(plldbrec.sales_ossr_count, (enqbuf.items_sold_today)+2,      // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.sales_ossr_count));                          // SDH 22-02-05 EXCESS
        }                                                                       // SDH 22-02-05 EXCESS
    } else {                                                                    // 13-08-2008 16 BMG
        // Multi-site location so update location values                        // 13-08-2008 16 BMG
        wMSLoc = satoi(MSLocation,2); //00 is first location                    // 13-08-2008 16 BMG
        //Force to 'other' location if > 33rd lcoation                          // 13-08-2008 16 BMG //CSk 12-03-2012 SFA
        if (wMSLoc > 32) wMSLoc = 32;                                           // 13-08-2008 16 BMG //CSk 12-03-2012 SFA
        pack(plldbrec.aMS_details[wMSLoc].ms_loc_count, 2, current, 4, 0);      // 13-08-2008 16 BMG
        pack(plldbrec.aMS_details[wMSLoc].ms_fill_qty, 2, fill_qty, 4, 0);      // 13-08-2008 16 BMG
        memcpy(plldbrec.aMS_details[wMSLoc].ms_time_of_count, abTimePD,         // 13-08-2008 16 BMG
               sizeof(plldbrec.aMS_details->ms_time_of_count));                 // 13-08-2008 16 BMG
        pack(plldbrec.aMS_details[wMSLoc].ms_sales_fig, 2,                      // 13-08-2008 16 BMG
               (enqbuf.items_sold_today)+2, 4, 0);                              // 13-08-2008 16 BMG
        //Also update the details on the standard PLLDB fields                  // 13-08-2008 16 BMG
        memcpy(plldbrec.time_sf_count, abTimePD,                                // 13-08-2008 16 BMG
               sizeof(plldbrec.time_sf_count));                                 // 13-08-2008 16 BMG
        memcpy(plldbrec.sales_figure, (enqbuf.items_sold_today)+2,              // 13-08-2008 16 BMG
                   sizeof(plldbrec.sales_figure));                              // 13-08-2008 16 BMG
        //Add counts to main PLLDB fields for running totals                    // 13-08-2008 16 BMG
        WordToArray(plldbrec.qty_on_shelf, 4,                                   // 13-08-2008 16 BMG
                    (satoi(plldbrec.qty_on_shelf, 4) + satoi(current, 4)));     // 13-08-2008 16 BMG
        WordToArray(plldbrec.fill_qty, 4,                                       // 13-08-2008 16 BMG
                    (satoi(plldbrec.fill_qty, 4) + satoi(fill_qty, 4)));        // 13-08-2008 16 BMG
    }                                                                           // 13-08-2008 16 BMG

    //A change has been made here to always force the status to unpicked.
    //The full logic has been left here since it will probably change back
    //in future.
    if ((satol(enqbuf.stock_figure , 6) +
         satol(plldbrec.qty_on_shelf, 4) ) != 0L) {                         //V4.0 PAB
        plldbrec.item_status[0] = 'U';                                      // Status : Unpicked
    } else {
        plldbrec.item_status[0] = 'U';                                      // Status : Ignore
        // overide status X
        // A record is required to maintain the correct sequence numbering
    }

    //Write PLLDB
    //Attempt to write to the PLLDB.  The code suggests that this function
    //will return an error if the record already exists on the keyed file.
    //HOWEVER, current thinking suggests that the return code 06CC is never
    //returned, and the record is simply overwritten successfully
    rc = WritePlldb(__LINE__);                                              // SDH 22-02-05 EXCESS
    if (rc <= 0L) return RC_DATA_ERR;

        //If the record already exists then read lock the record, update the details and
        //then write unlock
        //SDH: I really don't think this code ever gets executed!  It is commented out,
        //and NO LONGER works due to the changes made for Excess Stock
/*
        if ((rc & 0xFFFF)==0x06CC) {

            //Attempt to read the PLLDB record with LOCK
            //Wait for 250ms for the lock to become free
            rc = u_read( 1, 0, plldb.fnum, (void *)&plldbrec,
                         PLLDB_RECL, PLLDB_KEYL, 250 );

            //Handle read errors
            if (rc<=0L) {
                log_event101(rc, PLLDB_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R (lock) PLLDB. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }

            // Update fields that can change
            memcpy( plldbrec.qty_on_shelf, current, 4);
            memcpy( plldbrec.fill_qty, fill_qty, 4);
            plldbrec.gap_flag[0] = gap_flag;                                // SDH 17-11-04 OSSR WAN
            memset( plldbrec.stock_room_count, '0', 4);                     // SDH 17-11-04 OSSR WAN
            memcpy( plldbrec.sales_figure, enqbuf.items_sold_today+2, 4 );

            // Set default values for new fields in the OSSR extension to this record
            memset(plldbrec.filler, 'X', sizeof(plldbrec.filler));          // SDH 17-11-04 OSSR WAN
            memset(plldbrec.ossr_count, '0', 4);                            // SDH 17-11-04 OSSR WAN
            memset(plldbrec.time_bs_count, 0xFF, 2);                        // SDH 17-11-04 OSSR WAN
            memset(plldbrec.time_ossr_count, 0xFF, 2);                      // SDH 17-11-04 OSSR WAN
            memset(plldbrec.sales_bs_count, '0', 4);                        // SDH 17-11-04 OSSR WAN
            memset(plldbrec.sales_ossr_count, '0', 4);                      // SDH 17-11-04 OSSR WAN
            memcpy(plldbrec.time_sf_count, abTimePD,                        // SDH 17-11-04 OSSR WAN
                   sizeof(plldbrec.time_sf_count));                         // SDH 17-11-04 OSSR WAN

            //Write unlock the PLLDB
            //Handle write errors
            rc = u_write(1, 0, plldb.fnum, (void *)&plldbrec, PLLDB_RECL, 0L);
            if (rc<=0L) {
                if (debug) {
                    sprintf(msg, "Err-W (unlk) PLLDB. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }

            //Else must be a write error other than 'record already exists'
        } else {
            return RC_DATA_ERR;
        }
    }
*/

    // Read PLLOL record                                                    // 19-05-04 PAB
    rec = satol( pass_list_id, 3 );                                         // 19-05-04 PAB // 11-09-2007 13 BMG
    rc = ReadPllol(rec, __LINE__);                                          // SDH 26-11-04 CREDIT CLAIM
    if (rc <= 0L) return RC_DATA_ERR;                                       // 19-05-04 PAB

    //Get the item count; Increment it; Put it back                         // 19-05-04 PAB
    if (fNewRecord) {                                                       // 13-09-05 SDH RECOUNT
        ws_item_count = satol(pllolrec.item_count,4);                       // 19-05-04 PAB
        ws_item_count++;                                                    // 19-05-04 PAB
        sprintf(sbuf, "%04ld", ws_item_count);                              // 19-05-04 PAB
        memcpy(pllolrec.item_count, sbuf, sizeof(pllolrec.item_count));     // 19-05-04 PAB
        if (debug) {                                                        // 20-07-07 PAB
            sprintf(msg, "New PLLDB GAP");                                  // 20-07-07 PAB
            disp_msg(msg);                                                  // 20-07-07 PAB
        }                                                                   // 20-07-07 pAB
    }                                                                       // 13-09-05 SDH RECOUNT

    //Update the list type if this is the first item in it                  // SDH 22-02-05 EXCESS
    //'S' if Shelf Monitor, 'F' if Fast Fill,                               // SDH 22-02-05 EXCESS
    //'E' - Excess stock originating in backshop (and SF next)              // SDH 22-02-05 EXCESS
    //'D' - Excess stock originating in OSSR (and SF next)                  // SDH 22-02-05 EXCESS
    if (satoi(seq, 3) == 1) {                                               // SDH 22-02-05 EXCESS
        if (bLocation == 'S') {                                             // SDH 22-02-05 EXCESS
            if (gap_flag == 'Y') {                                          // SDH 22-02-05 EXCESS
                pllolrec.cLocation = 'S';                                   // SDH 22-02-05 EXCESS
            } else {                                                        // SDH 22-02-05 EXCESS
                pllolrec.cLocation = 'F';                                   // SDH 22-02-05 EXCESS
            }                                                               // SDH 22-02-05 EXCESS
        } else if (bLocation == 'B') {                                      // SDH 22-02-05 EXCESS
            pllolrec.cLocation = 'E';                                       // SDH 22-02-05 EXCESS
        } else {                                                            // SDH 22-02-05 EXCESS
            pllolrec.cLocation = 'D';                                       // SDH 22-02-05 EXCESS
        }                                                                   // SDH 22-02-05 EXCESS
    }                                                                       // SDH 22-02-05 EXCESS

    //Write the record back                                                 // 19-05-04 PAB
    rc = WritePllol(rec, __LINE__);                                         // SDH 22-02-05 EXCESS
    if (rc <= 0L) return RC_DATA_ERR;                                       // 19-05-04 PAB

    // 19-05-04 PAB
    // if on palm the write the gap now

/* Redundant palm pilot code - SDH - 21-12-2004
    if (write_gap<=3) {                                                     // if old PALM mode
        // if TSF and non on shelf is zero
        if (satol(enqbuf.stock_figure, 6)+satol(plldbrec.qty_on_shelf,4)==0L) {    //V4.0 PAB
            // Stock figure - send to GAP report

            // Write record to GAP workfile if this is a gap monitor
            if (gap_flag == 'Y') {                                          // SDH 17-11-04 OSSR WAN

                *(gapbfrec) = 0x22;
                memcpy( (BYTE *)&gapbfrec+1, "000000", 6 );
                memcpy( (BYTE *)&gapbfrec+7, boots_code_unp, 6 );           // SW2.3
                *(gapbfrec+13) = 0x22;
                *(gapbfrec+14) = 0x0D;
                *(gapbfrec+15) = 0x0A;
                if (debug) {
                    disp_msg( "WR GAPBF :" );
                    dump( (BYTE *)&gapbfrec, 16L );
                }
                rc = s_write( A_FLUSH | A_FPOFF, lrtp[log_unit]->fnum2,
                              (void *)&gapbfrec, 16L, 0L );

                if (rc<0L) {
                    if (debug) {
                        sprintf( msg, "Err-W to %s. RC:%08lX",
                                 pq[lrtp[log_unit]->pq_sub2].fname, rc );
                        disp_msg(msg);
                    }
                    return RC_DATA_ERR;
                }

            }
        }
    }                                                                       // 20-02-04 PAB
*/
    return RC_OK;
}


typedef struct {
   BYTE cmd[3];
   BYTE item_desc[20];
   BYTE sold_q_t[4];
   BYTE sold_v_t[8];
   BYTE sold_q_w[4];
   BYTE sold_v_w[8];
} LRT_ISR;                                    // Item sales response
#define LRT_ISR_LTH sizeof(LRT_ISR)

static URC sales_enquiry(BYTE *item_code_unp, LONG item_price, LRT_ISR *isrp)
{
    LONG rc;
    BYTE bar_code[11];                                                      // BCD
    URC usrrc;
    IMSTC_REC imstcrec = {0};
    IMF_REC imfprec = {0};
    BYTE sbuf[64];

    UNUSED(item_price);                                                     //Prevent compiler warning - variable not used

    if (debug) {
        disp_msg("Sales Enquiry - RD IMSTC :");
        dump(item_code_unp, 6);
    }

    // Pack ASCII item code
    memset(bar_code, 0x00, 11);
    pack(bar_code+8, 3, item_code_unp, 6, 0);

    // Open IMSTC
    usrrc = open_imstc();
    if (usrrc<RC_IGNORE_ERR) {
        return usrrc;
    }
    // look up item on IMSTC (to get latest stock figure)
    memcpy(imstcrec.bar_code, bar_code, 11);
    if (debug) {
        disp_msg("RD IMSTC :");
        dump( imstcrec.bar_code, (WORD)IMSTC_KEYL );
    }
    rc = s_read(0, imstc.fnum, (void *)&imstcrec, IMSTC_RECL, IMSTC_KEYL);
    close_imstc( CL_SESSION );
    if (rc<=0L) {
        imstc.present=FALSE;
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            imstc.present=FALSE;
            imstcrec.numitems = 0L;
            if (debug) {
                disp_msg("NOF");
            }
        } else {
            log_event101(rc, IMSTC_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-R IMSTC. RC:%08lX", rc);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
    } else {
        imstc.present=TRUE;
        if (debug) {
            disp_msg("OK");
        }
    }

    // Open EALIMOVP
    usrrc = open_imfp();
    if (usrrc<RC_IGNORE_ERR) {
        return usrrc;
    }
    // look up item on EALIMOVP
    if (debug) {
        disp_msg("RD EALIMOVP :");
        dump( imstcrec.bar_code, (WORD)IMFP_KEYL );
    }
    memcpy(imfprec.bar_code, bar_code, 11);
    rc = s_read(0, imfp.fnum, (void *)&imfprec, IMFP_RECL, IMFP_KEYL);
    close_imfp( CL_SESSION );
    if (rc<=0L) {
        imfp.present=FALSE;
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            imfp.present=FALSE;
            imfprec.numitems = 0L;
            if (debug) {
                disp_msg("NOF");
            }
        } else {
            log_event101(rc, IMFP_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-R IMFP. RC:%08lX", rc);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
    } else {
        imfp.present=TRUE;
        if (debug) {
            disp_msg("OK");
        }
    }

    if (!imstc.present) {                                                   // v4.0
        imstcrec.numitems = 0;                                              // v4.0
        imstcrec.amtsale = 0;                                               // v4.0
    }                                                                       // v4.0

    if (!imfp.present) {                                                    // v4.0
        imfprec.numitems = 0;                                               // v4.0
        imfprec.amtsale = 0;                                                // v4.0
    }                                                                       // v4.0

    sprintf( sbuf, "%04ld", imstcrec.numitems/100 );
    memcpy( ((LRT_ISR *)isrp)->sold_q_t, sbuf, 4 );
//   sprintf( sbuf, "%08ld", ((imstcrec.numitems/100) * item_price) );  // v4.0
    sprintf( sbuf, "%08ld", imstcrec.amtsale );
    memcpy( ((LRT_ISR *)isrp)->sold_v_t, sbuf, 8 );
    sprintf( sbuf, "%04ld", (imfprec.numitems/100) + (imstcrec.numitems/100) );
    memcpy( ((LRT_ISR *)isrp)->sold_q_w, sbuf, 4 );
//   sprintf( sbuf, "%08ld", ((imfprec.numitems/100) * item_price) +
//            ((imstcrec.numitems/100) * item_price) );
    sprintf( sbuf, "%08ld", imfprec.amtsale + imstcrec.amtsale );
    memcpy( ((LRT_ISR *)isrp)->sold_v_w, sbuf, 8 );

    return RC_OK;
}


typedef struct {
   BYTE cmd[3];
   BYTE sales_t[10];           // Pence       // 11-09-2007 9 BMG
   BYTE sales_w[10];           // Pence       // 11-09-2007 9 BMG
} LRT_SSR;                                    // Store sales info response
#define LRT_SSR_LTH sizeof(LRT_SSR)

static URC store_sales_enquiry(LRT_SSR *ssrp)
{
    BYTE finished;
    WORD tno                                                                /*, i*/;
    LONG rc, tsf_lth, psbt_lth, wrf_lth;                                // 11-09-2007 13 BMG
    LONG tsf_rps, psbt_rps, wrf_rps, tsf_os, psbt_os                        /*, wrf_os*/, r;
    MPB mem;
    BYTE *tsfp, *psbtp, *wrfp;
    BYTE sbuf[64];
    BYTE z[128];
    TILL_TAKINGS *ttak;
    DOUBLE today, wtd;                                                  // 11-09-2007 13 BMG

    //Initialise totals                                                 //SDH 26-Jul-2005
    today = 0;                                                         //SDH 26-Jul-2005 // 11-09-2007 13 BMG
    wtd = 0;                                                           //SDH 26-Jul-2005 // 11-09-2007 13 BMG

    // Set up null record comparison
    memset( z, 0x00, 128 );

    // Allocate memory
    mem.mp_start = 0L;
    mem.mp_min = TSF_BUFFER;
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    } else {
        tsfp = (BYTE *)mem.mp_start;
    }
    mem.mp_start = 0L;
    mem.mp_min = TSF_BUFFER;
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    } else {
        psbtp = (BYTE *)mem.mp_start;
    }
    mem.mp_start = 0L;
    mem.mp_min = WRF_BUFFER;
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    } else {
        wrfp = (BYTE *)mem.mp_start;
    }
    mem.mp_start = 0L;
    mem.mp_min = (LONG)sizeof(TILL_TAKINGS)                                 /*TILL_TAKINGS_REC_LTH*/;
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    } else {
        ttak = (TILL_TAKINGS *)mem.mp_start;
        memset( ttak, 0x00, sizeof(TILL_TAKINGS) );
    }

    // Read TSF, PSBT and WRF files into memory (don't read KF control sector)
    rc = s_read( A_BOFOFF, tsf.fnum, (void *)tsfp, TSF_BUFFER, 512L );
    if (rc==TSF_BUFFER) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Temp TSF buffer too small. Size:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    if (rc<0L) {
        log_event101(rc, TSF_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-R TSF. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_DATA_ERR;
    }
    tsf_lth = rc;
    rc = s_read( A_BOFOFF, psbt.fnum, (void *)psbtp, TSF_BUFFER, 512L );
    if (rc==TSF_BUFFER) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Temp PSBT buffer too small. Size:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    if (rc<0L) {
        log_event101(rc, PSBT_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-R PSBT. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_DATA_ERR;
    }
    psbt_lth = rc;
    rc = s_read( A_BOFOFF, wrf.fnum, (void *)wrfp, WRF_BUFFER, 512L );
    if (rc==WRF_BUFFER) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Temp WRF buffer too small. Size:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    if (rc<0L) {
        log_event101(rc, WRF_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-R WRF. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_DATA_ERR;
    }
    wrf_lth = rc;
    UNUSED(wrf_lth);                                                        //SDH - Prevent compiler warning - var not used

    // Accumulate totals
    tsf_rps = 508L / TSF_RECL;                                              // Records per sector
    psbt_rps = 508L / PSBT_RECL;                                            // Records per sector
    wrf_rps = 508L / WRF_RECL;                                              // Records per sector
    UNUSED(wrf_rps);                                                        //SDH - Prevent compiler warning - var not used

    // TSF (Figures from last end of day store close to now)
    r = 0L;
    finished = FALSE;
    do {
        tsf_os = ((r/tsf_rps)*512L) + 4L + ((r%tsf_rps)*TSF_RECL);
        if (tsf_os <= tsf_lth) {
            if (memcmp((BYTE *)(tsfp+tsf_os), z, TSF_RECL) != 0) {
                // non-blank record
                tno = unpack_to_word(((TSF_TILL_REC *)(tsfp+tsf_os))->till, 2);
                if (tno>=0 && tno<=999) {
                    ttak->till[tno].today =
                    ((TSF_TILL_REC *)(tsfp+tsf_os))->netcash +
                    ((TSF_TILL_REC *)(tsfp+tsf_os))->netncash;
                }
            }
            r++;
        } else {
            finished = TRUE;
        }
    } while (!finished);

    // PSBT (Figures from last end of day store close to now)
    r = 0L;
    finished = FALSE;
    do {
        psbt_os = ((r/psbt_rps)*512L) + 4L + ((r%psbt_rps)*PSBT_RECL);
        if (psbt_os <= psbt_lth) {
            if (memcmp((BYTE *)(psbtp+psbt_os), z, PSBT_RECL) != 0) {
                // non-blank record
                tno = unpack_to_word(((TSF_TILL_REC *)(psbtp+psbt_os))->till, 2);
                if (tno>=0 && tno<=999) {
                    ttak->till[tno].wtd =
                    ((TSF_TILL_REC *)(psbtp+psbt_os))->netcash +
                    ((TSF_TILL_REC *)(psbtp+psbt_os))->netncash;
                }
            }
            r++;
        } else {
            finished = TRUE;
        }
    } while (!finished);

//   // WRF (Determine which tills are in workgroups, and ignore rest)
//   r = 0L;
//   finished = FALSE;
//   do {
//      wrf_os = ((r/wrf_rps)*512L) + 4L + ((r%wrf_rps)*WRF_RECL);
//      if ( wrf_os <= wrf_lth ) {
//         if ( memcmp((BYTE *)(wrfp+wrf_os), z, WRF_RECL) != 0 ) {
//            // non-blank record
//            for (i=0; i<20; i++) {
//               tno=unpack_to_word(((WRF_REC *)(wrfp+wrf_os))->till[i], 2);
//               if (tno>=0 && tno<=999) {
//                  today += ttak->till[tno].today;
//                  wtd += ttak->till[tno].wtd;
//               }
//            }
//         }
//         r++;
//      } else {
//         finished = TRUE;
//      }
//   } while (!finished);

    // WRF (Determine which tills are in workgroups, and ignore rest)
    for (tno=0; tno<1000; tno++) {
        today += ttak->till[tno].today;
        wtd += ttak->till[tno].wtd;
    }

    // Free memory
    rc = s_mfree( (void *) ttak );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    rc = s_mfree( (void *)wrfp );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    rc = s_mfree( (void *)psbtp );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    rc = s_mfree( (void *) tsfp );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }

    sprintf( sbuf, "%010.0f", today - wtd );                          // 11-09-2007 13 BMG
    memcpy( ssrp->sales_t, sbuf, 10);                                 // 11-09-2007 13 BMG
    sprintf( sbuf, "%010.0f", today );                                // 11-09-2007 13 BMG
    memcpy( ssrp->sales_w, sbuf, 10);                                 // 11-09-2007 13 BMG

    return RC_OK;
}


//NOTE: This routine assumes that the caller has already read the PLLOL,    // 23-05-2005 SDH Excess
// and will write the PLLOL on return                                       // 23-05-2005 SDH Excess
static URC SetListUnpicked(WORD wListId) {                                  // 23-05-2005 SDH Excess

    LONG lRc = 0;                                                           // 23-05-2005 SDH Excess
    WORD wSeqNum = 1;                                                       // 23-05-2005 SDH Excess
    WORD wHoles = 0;                                                        // 23-05-2005 SDH Excess
    WORD wItemCount = 0;                                                    // 23-05-2005 SDH Excess
    BYTE abListId[4];                                                       // 23-05-2005 SDH Excess
    BYTE abTemp[5];                                                         // 23-05-2005 SDH Excess

    disp_msg("Entered SetListUnpicked");                                    // 23-05-2005 SDH Excess
    sprintf(abListId, "%03d", wListId);                                     // 23-05-2005 SDH Excess

    while (wSeqNum <= 999) {                                                // 23-05-2005 SDH Excess

        //Read the next record                                              // 23-05-2005 SDH Excess
        sprintf(abTemp, "%03d", wSeqNum);                                   // 23-05-2005 SDH Excess
        memcpy(plldbrec.seq, abTemp, sizeof(plldbrec.seq));                 // 23-05-2005 SDH Excess
        memcpy(plldbrec.list_id, abListId, sizeof(plldbrec.list_id));       // 23-05-2005 SDH Excess
        lRc = ReadPlldbLog(__LINE__, LOG_CRITICAL);                         // 23-05-2005 SDH Excess

        //Handle errors and holes                                           // 23-05-2005 SDH Excess
        if (lRc <= 0L) {                                                    // 23-05-2005 SDH Excess
            if ((lRc&0xFFFF)==0x06C8 || (lRc&0xFFFF)==0x06CD) {             // 23-05-2005 SDH Excess
                if (++wHoles > 5) break;                                    // 23-05-2005 SDH Excess
                wSeqNum++;                                                  // 23-05-2005 SDH Excess
                continue;                                                   // 23-05-2005 SDH Excess
            }                                                               // 23-05-2005 SDH Excess
            return RC_DATA_ERR;                                             // 23-05-2005 SDH Excess
        }                                                                   // 23-05-2005 SDH Excess

        //Reset holes, set item to Unpicked and write                       // 23-05-2005 SDH Excess
        wHoles = 0;                                                         // 23-05-2005 SDH Excess
        plldbrec.item_status[0] = 'U';                                      // 23-05-2005 SDH Excess
        lRc = WritePlldb(__LINE__);                                         // 23-05-2005 SDH Excess
        if (lRc <= 0L) return RC_DATA_ERR;                                  // 23-05-2005 SDH Excess
        wSeqNum++;                                                          // 23-05-2005 SDH Excess
        wItemCount++;                                                       // 23-05-2005 SDH Excess

    }                                                                       // 23-05-2005 SDH Excess

    sprintf(abTemp, "%04d", wItemCount);                                    // 23-05-2005 SDH Excess
    memcpy(pllolrec.item_count, abTemp, sizeof(pllolrec.item_count));       // 23-05-2005 SDH Excess

    return RC_OK;                                                           // 23-05-2005 SDH Excess

}                                                                           // 23-05-2005 SDH Excess




//////////////////////////////////////////////////////////////////////////
///                                                                     //
///   buildSNR                                                          //
///                                                                     //
///   Build the SNR response from the component fields.                 //
///                                                                     //
//////////////////////////////////////////////////////////////////////////

// SNR - existing record appears to be unused - format changed
typedef struct LRT_SNR_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE auth;                 // ' ' = user, 'S' = Supervisor   //17-11-2004 SDH
   BYTE name[15];
   BYTE stamp[12];            // Date/time stamp - format YYYYMMDDHHMM
   BYTE prtnum[10];           // phase 2 printer config status flags  4.02PAB
   BYTE bOssrWanActive;       // 'Y' if OSSR WAN is active      //17-11-2004  SDH
   BYTE cStockAccess;         // 'Y' if user allowed to adjust TSF          //CSk 12-03-2012 SFA
   BYTE snrprtdesc[200];      // Printer location descriptor
} LRT_SNR;                                    // Price check response
#define LRT_SNR_LTH sizeof(LRT_SNR)

static void buildSNR(LRT_SNR* pSNR, BYTE* pbPrtNum, BYTE* pbOpID, BYTE bAuth,// 16-11-2004 SDH
              BYTE* pbUserName, BYTE cOssrStore, BYTE cStockAccess) {  // 25-Sep-2006 SDH Planners //CSk 12-03-2012 SFA

    LONG sec, day, month, year;                                             // 16-11-2004 SDH
    WORD hour, min;                                                         // 16-11-2004 SDH

    memcpy(pSNR->cmd, "SNR", 3);                                            // 16-11-2004 SDH
    memcpy(pSNR->prtnum, pbPrtNum, 10);                                     // 16-11-2004 SDH
    memcpy(pSNR->opid, pbOpID, 3);                                          // 16-11-2004 SDH
    pSNR->auth = bAuth;                                                     // 16-11-2004 SDH
    memcpy(pSNR->name, pbUserName, 15);                                     // 16-11-2004 SDH
    sysdate( &day, &month, &year, &hour, &min, &sec );                      // 16-11-2004 SDH
    sprintf(msg, "%04ld%02ld%02ld%02d%02d", year, month, day, hour, min );  // 16-11-2004 SDH
    memcpy(pSNR->stamp, msg, sizeof(pSNR->stamp));                          // 16-11-2004 SDH
    pSNR->bOssrWanActive = (cOssrStore == 'W') ? 'Y':'N';                   // 16-11-2004 SDH
    pSNR->cStockAccess = cStockAccess;                                      //CSk 12-03-2012 SFA
}                                                                           // 16-11-2004 SDH


typedef struct {
    BYTE cmd[3];                              // "LPR"
    BYTE was_price1[6];                       // 2dp assumed
    BYTE was_price2[6];                       // 2dp assumed
    BYTE PHF_label_type[1];                   // 0,1,2,3
    BYTE unit_price_flag[1];                  // Y/N if Y then WEEE must be N
    BYTE unit_measurement[6];                 // 0-9999
    BYTE unit_item_quantity[8];               // 0-9999
    BYTE unit_type[10];                       // ltr,Kg,g,ml etc
    BYTE WEEE_item_flag[1];                   // Y/N
    BYTE WEEE_prf_price[6];                   // 2dp assumed
    BYTE MS_marker[1];                        // Y/N
    BYTE PainKillerMessage[40];               // 08-11-07 PAB PainKiller SEL Messsage
} LRT_LPR;
#define LRT_LPR_LTH sizeof(LRT_LPR)

static LONG ProcessPainkiller(void) {                                        // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    LRT_LPR* pLPR = (LRT_LPR*)out;                                           // 08-11-07 PAB Mobile Printing
    //LONG fnum;                                                               // 08-11-07 PAB Mobile Printing
    //BYTE seldescrec[512];                                                    // 08-11-07 PAB Mobile Printing
    BYTE messagerec[80];                                                     // 08-11-07 PAB Mobile Printing
    LONG rc=1;                                                               // 08-11-07 PAB Mobile Printing
    WORD wPainKillerNo=0;                                                    // 08-11-07 PAB Mobile Printing
    WORD wRecordNumber=0;                                                    // 08-11-07 PAB Mobile Printing
    WORD wIdx=0;                                                             // 08-11-07 PAB Mobile Printing
    WORD wPtr=0;                                                             // 08-11-07 PAB Mobile Printing
    WORD wPtr2=0;                                                            // 08-11-07 PAB Mobile Printing
    BYTE indicat=0;                                                          // 08-11-07 PAB Mobile Printing
    BYTE bEOL=0x0D;                                                          // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    indicat=irfrec.indicat1;                                                 // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    //IF IRF.INDICAT1% AND 01H THEN PAINKILLER.NO% = PAINKILLER.NO% OR 01H   // 08-11-07 PAB Mobile Printing
    if ((indicat&0x01)!=0) {                                                 // 08-11-07 PAB Mobile Printing
        wPainKillerNo = 1;                                                   // 08-11-07 PAB Mobile Printing
    }                                                                        // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    //IF IRF.INDICAT1% AND 02H THEN PAINKILLER.NO% = PAINKILLER.NO% OR 02H   // 08-11-07 PAB Mobile Printing
    if ((indicat & 0x02)!=0) {                                               // 08-11-07 PAB Mobile Printing
         wPainKillerNo = wPainKillerNo + 2;                                  // 08-11-07 PAB Mobile Printing
    }                                                                        // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    //IF IRF.INDICAT1% AND 80H THEN PAINKILLER.NO% = PAINKILLER.NO% OR 04H   // 08-11-07 PAB Mobile Printing
    if ((indicat & 0x80)!=0) {                                               // 08-11-07 PAB Mobile Printing
         wPainKillerNo = wPainKillerNo + 4;                                  // 08-11-07 PAB Mobile Printing
         // wPainKillerNo = (wPainKillerNo || 0x04);                         // 08-11-07 PAB Mobile Printing
    }                                                                        // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
    //IF IRF.INDICAT1% AND 20H THEN PAINKILLER.NO% = PAINKILLER.NO% OR 08H  // 08-11-07 PAB Mobile Printing
    if ((indicat & 0x20)!=0) {                                              // 08-11-07 PAB Mobile Printing
       //wPainKillerNo = (wPainKillerNo || 0x08);                           // 08-11-07 PAB Mobile Printing
       wPainKillerNo = wPainKillerNo + 8;                                   // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
    if (debug) {                                                            // 08-11-07 PAB Mobile Printing
       sprintf(msg, "Determine Painkiller message number %d ",              // 08-11-07 PAB Mobile Printing
               wPainKillerNo);                                              // 08-11-07 PAB Mobile Printing
       disp_msg(msg);                                                       // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
    switch (wPainKillerNo) {                                                // 08-11-07 PAB Mobile Printing
    case 0: {       // blank message                                        // 08-11-07 PAB Mobile Printing
        wRecordNumber = 0;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 1: {      // asprin                                                // 08-11-07 PAB Mobile Printing
        wRecordNumber = 2;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 2: {      // paracetamol                                           // 08-11-07 PAB Mobile Printing
        wRecordNumber = 1;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 3: {      // par and aspr                                          // 08-11-07 PAB Mobile Printing
        wRecordNumber = 4;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 4: {      //ibprophen                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 3;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 5: {                                                               // 08-11-07 PAB Mobile Printing
        wRecordNumber = 6;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 6: {                                                               // 08-11-07 PAB Mobile Printing
        wRecordNumber = 5;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 7: {                                                               // 08-11-07 PAB Mobile Printing
        wRecordNumber = 7;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 8: {                                                               // 08-11-07 PAB Mobile Printing
        wRecordNumber = 0;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 9: {                                                               // 08-11-07 PAB Mobile Printing
        wRecordNumber = 9;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 10: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 8;                                                  // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 11: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 11;                                                 // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 12: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 10;                                                 // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 13: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 13;                                                 // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 14: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 12;                                                 // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    case 15: {                                                              // 08-11-07 PAB Mobile Printing
        wRecordNumber = 14;                                                 // 08-11-07 PAB Mobile Printing
        break;                                                              // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
    if (debug) {                                                            // 08-11-07 PAB Mobile Printing
       sprintf(msg, "Returning Painkiller Record number %d ",               // 08-11-07 PAB Mobile Printing
               wRecordNumber);                                              // 08-11-07 PAB Mobile Printing
       disp_msg(msg);                                                       // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
                                                                            // 08-11-07 PAB Mobile Printing
    if ((wRecordNumber == 0L) || (wRecordNumber > 15)) {                    // 08-11-07 PAB Mobile Printing
        // not a painkiller item or message no invalild                     // 08-11-07 PAB Mobile Printing
        memset(pLPR->PainKillerMessage,0x20,                                // 08-11-07 PAB Mobile Printing
               sizeof(pLPR->PainKillerMessage));                            // 08-11-07 PAB Mobile Printing
        return 0;                                                           // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing

    URC usrrc = SeldescOpen();                                              // Streamline SDH 21-Sep-2008
    if (usrrc <= RC_DATA_ERR) {                                             // Streamline SDH 21-Sep-2008
        // file open error return spaces as the painkiller message          // 08-11-07 PAB Mobile Printing
        memset(pLPR->PainKillerMessage,0x20,                                // 08-11-07 PAB Mobile Printing
               sizeof(pLPR->PainKillerMessage));                            // 08-11-07 PAB Mobile Printing
        return -1;                                                          // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    memset( (BYTE *)&seldescrec, 0x00, sizeof(SELDESC_REC) );               // 08-11-07 PAB Mobile Printing
    // read the entire file in one go                                       // 08-11-07 PAB Mobile Printing
    rc = SeldescRead(0L, __LINE__);                                         // Streamline SDH 17-Sep-2008
    wIdx=1;                                                                 // 08-11-07 PAB Mobile Printing
    wPtr=0;
    wPtr2=0;                                                                // 08-11-07 PAB Mobile Printing
    while (rc >0L) {                                                        // 08-11-07 PAB Mobile Printing
       if (memcmp(&seldescrec.abWarning[wPtr],&bEOL,1)==0) {                // 08-11-07 PAB Mobile Printing
         wIdx++;  //ODOA found, increment record count                      // 08-11-07 PAB Mobile Printing
         wPtr++;  //skip over OA to point to start of next record           // 08-11-07 PAB Mobile Printing
         wPtr++;  //point to start of next record
       }
       if (wIdx == wRecordNumber) {  // if record number matches required   // 08-11-07 PAB Mobile Printing
         memset(messagerec,0x20,sizeof(messagerec));                        // 08-11-07 PAB Mobile Printing
         // extract out the message record we want                          // 08-11-07 PAB Mobile Printing
         while(memcmp(&seldescrec.abWarning[wPtr],&bEOL,1)!=0){             // 08-11-07 PAB Mobile Printing
            memcpy(&messagerec[wPtr2],&seldescrec.abWarning[wPtr],1);       // 08-11-07 PAB Mobile Printing
            wPtr2++;                                                        // 08-11-07 PAB Mobile Printing
            wPtr++;                                                         // 08-11-07 PAB Mobile Printing
         }                                                                // 14-11-07 PAB Mobile Printing
  // 08-11-07 PAB Mobile Printing
         // return the extracted record in the out structure                // 08-11-07 PAB Mobile Printing
         memcpy(pLPR->PainKillerMessage,                                    // 08-11-07 PAB Mobile Printing
               (BYTE*)&messagerec, sizeof(pLPR->PainKillerMessage));        // 08-11-07 PAB Mobile Printing
         rc=-1; // force outer while auto break                             // 08-11-07 PAB Mobile Printing
         break;                                                             // 08-11-07 PAB Mobile Printing
       }                                                                    // 08-11-07 PAB Mobile Printing
       wPtr++;    // increment pointer to check at next byte for ODOA       // 08-11-07 PAB Mobile Printing
       if (wPtr >=512) {                                                    // 08-11-07 PAB Mobile Printing
           // end of SelDesc File buffer reached without ODOA               // 08-11-07 PAB Mobile Printing
           // and no match on message number was found                      // 08-11-07 PAB Mobile Printing
           rc=-1;                                                           // 08-11-07 PAB Mobile Printing
           break;                                                           // 08-11-07 PAB Mobile Printing
       }                                                                    // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    SeldescClose(CL_SESSION);                                               // Streamline SDH 21-Sep-2008
    return 0;                                                               // 08-11-07 PAB Mobile Printing
}                                                                           // 08-11-07 PAB Mobile Printing


typedef struct LRT_PRT_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE item_code[13];
   BYTE type[1];                              // [B]atch / [I]mmediate print
} LRT_PRT;                                    // SEL print request
#define LRT_PRT_LTH sizeof(LRT_PRT)

static URC BuildLPR(char *inbound) {                                               // 07-08-07 PAB Mobile Printing

   LRT_PRT* pPRT = (LRT_PRT*)inbound;                                       // 07-08-07 PAB Mobile Printing
   LRT_LPR* pLPR = (LRT_LPR*)out;                                           // 07-08-07 PAB Mobile Printing
                                                                            // 07-08-07 PAB Mobile Printing
   LONG usrrc = RC_OK;                                                      // 07-08-07 PAB Mobile Printing
   UWORD lUnitNameCounter;

   //LONG lIudfRecNum;                                                      // RJN 28-06-2013 EVR
   LONG lUnitQty;
   LONG lQty1;
   LONG lQty2;
                                                                            // 07-08-07 PAB Mobile Printing
   UNUSED(inbound);
   BYTE bar_code[11];                                                      // BCD
   BYTE boots_code[4];                                                     // BCD incl cd  (0c cc cc cd)
   BYTE boots_code_ncd[4];                                                 // BCD excl cd  (00 cc cc cc)
   BYTE packed_zeros[8];


    // read the IRF for the item code on the PRT transaction

    // return error if required files are not open
    if (irf.sessions==0 || irfdex.sessions==0 ||
        idf.sessions==0 || isf.sessions==0) {
        return 1;
    }

    //memset( (BYTE *)&irfrec, 0x00, sizeof(IRF_REC) );
    memset(packed_zeros, 0x00, 8);
    memset(boots_code, 0x00, 4);
    memset(boots_code_ncd, 0x00, 4);

    // Pack ASCII item code
    memset(bar_code, 0x00, 11);
    pack(bar_code+5, 6, pPRT->item_code, 12, 0);
    // Read IRF
    memcpy( irfrec.bar_code, bar_code, 11 );
    usrrc = IrfRead(__LINE__);
    if (usrrc<=0L) {
        return 1;
    }

    //IRF read OK
    irf.present=TRUE;

    usrrc = ProcessPainkiller();

    calc_boots_cd(idfrec.boots_code, irfrec.boots_code);
    // read the IDF for the current item.
    usrrc = IdfRead(__LINE__);

    if (usrrc<=0L) {
       return 2;
    }

    idf.present=TRUE;

    // if the item is WEEE then set the PHF fields to zero and the label type to zero
    // IRF Indicate8 bit 8
    memset(pLPR->WEEE_item_flag,'N',1);

    if (irf.present==TRUE) {
        pLPR->WEEE_item_flag[0] = ((irfrec.indicat8 & 0x80) != 0 ? 'Y' : 'N');
    }

    // otherwise read the PHF using IRF barcode
    if (pLPR->WEEE_item_flag[0] == 'N') {
        usrrc = PhfOpen();
        pack(phfrec.ubPHFBarCode, 6, pPRT->item_code, 12, 0);
        phf.present=TRUE;
        usrrc = PhfRead(__LINE__);

        if (usrrc <= 0L) {
           phf.present=FALSE;
        }
        usrrc = PhfClose( CL_SESSION );
    }

    // read the isf to get the weee prf price (item qty field)
    isf.present=TRUE;
    lUnitQty = 0;

    MEMCPY(isfrec.boots_code, idfrec.boots_code);
    usrrc = IsfRead(__LINE__);

    if (usrrc<=0L) {
        // if not on isf set to defaults
        isf.present=FALSE;
        memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));
        memset(pLPR->unit_item_quantity, 0x30, sizeof(pLPR->unit_item_quantity));
        memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));
        sprintf( sbuf, "%06ld", lUnitQty );
        memcpy( pLPR->WEEE_prf_price, sbuf, sizeof(pLPR->WEEE_prf_price) );
    } else {
        lQty1 = isfrec.wQty1;
        lQty2 = isfrec.cQty2;
        lUnitQty = (lQty2 * (LONG)65535);
        lUnitQty = lUnitQty + lQty1;
        lUnitQty = lUnitQty + lQty2;
        //sprintf( sbuf, "%06ld", lUnitQty + isfrec.cUnitType ); //BMG 1.2 17-10-2007
        sprintf( sbuf, "%06ld", lUnitQty); //BMG 1.2 17-10-2007
        if (pLPR->WEEE_item_flag[0] == 'Y') {
            // if on iSF and item is weee item then cant be unit price, the prf is the integer4 value
            sprintf( sbuf, "%06ld", lUnitQty );
            memcpy( pLPR->WEEE_prf_price, sbuf, sizeof(pLPR->WEEE_prf_price) );
            memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));
            memset(pLPR->unit_item_quantity, 0x30, sizeof(pLPR->unit_item_quantity));
            memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));
        } else {
            if ((isfrec.integer2 == 0x2020))  {
               // if not weee and if the values on the ISF are not initialised then set them to zero
               isfrec.wQty1 = 0;
               isfrec.cQty2 = 0;
               isfrec.integer2 = 0;
               lUnitQty = 0;
            }
            // if not WEEE set to defaults
            memset( pLPR->WEEE_prf_price, 0x30, sizeof(pLPR->WEEE_prf_price) );
            // Extract unit type part of the integer
            lUnitNameCounter = isfrec.cUnitType;
            sprintf( sbuf, "%06d", lUnitNameCounter );
            if (debug) {                                                            // 14-11-07 PAB Mobile Printing
               sprintf(msg, "Read IUDF for message %d",lUnitNameCounter);           //RJN 28-06-2013 EVR
               disp_msg(msg);                                                       // 14-11-07 PAB Mobile Printing
            }
            // if not weee then determine the unit price attributes
            if (lUnitNameCounter > 0) {
                // read the IUDF
                usrrc = IudfOpen();                                                // RJN 28-06-2013 ELR
                if (usrrc == RC_OK) {                                              // RJN 28-06-2013 ELR
                    // Streamline SDH 21-Sep-2008
                    // compute the IUDF record number for the unit type            // RJN 28-06-2013 ELR
                    // remember in C we compute the byte location of the start of each record.
                    //lIudfRecNum = ((lUnitNameCounter - 1) * IUDF_RECL) ;         // RJN 28-06-2013 ELR
                    usrrc = IudfRead(lUnitNameCounter - 1, __LINE__);              // RJN 28-06-2013 ELR
                    // read as a direct file remove function to improve performance.
                    //usrrc = s_read( A_BOFOFF,iudf.fnum,                                // RJN 28-06-2013 ELR
                    //    (void *)&iudfrec, IUDF_RECL, lIudfRecNum);                     // RJN 28-06-2013 ELR
                    if (usrrc <= 0L) {
                        // the unit type was not found set to spaces
                        memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));
                        memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));
                    } else {
                        memcpy(pLPR->unit_type,iudfrec.abDescription,sizeof(pLPR->unit_type)); // RJN 28-06-2013 ELR
                        memset(pLPR->unit_price_flag, 'Y' ,sizeof(pLPR->unit_price_flag));
                    }
                    usrrc = IudfClose ( CL_SESSION );                                   // RJN 28-06-2013 ELR
                } else {                                                                // RJN 28-06-2013 ELR
                    memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));               // RJN 28-06-2013 ELR
                    memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));  // RJN 28-06-2013 ELR
                }                                                                       // RJN 28-06-2013 ELR
            } else {
                // unit type not set return spaces
                memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));
                memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));
            }

            lQty1 = (LONG)isfrec.wQty1;
            lQty2 = (LONG)isfrec.cQty2;
            lUnitQty = (lQty2 * (LONG)65535);
            lUnitQty = lUnitQty + lQty1;
            lUnitQty = lUnitQty + lQty2;

            sprintf( sbuf, "%08ld", lUnitQty );
            memcpy ( pLPR->unit_item_quantity, sbuf ,sizeof(pLPR->unit_item_quantity));
            sprintf( sbuf, "%06d", isfrec.integer2 );
            memcpy ( pLPR->unit_measurement, sbuf ,sizeof(pLPR->unit_measurement));
        }
    }

    // populate the PHF fields on the LPR outbound record (set to defaults if no PHF)
    if (phf.present==TRUE) {
        sprintf( sbuf, "%06d",phfrec.lHist2Price );
        memcpy(pLPR->was_price1, sbuf, sizeof(pLPR->was_price1));
        sprintf( sbuf, "%06d",phfrec.lHist1Price );
        memcpy(pLPR->was_price2, sbuf, sizeof(pLPR->was_price2));
        if ((idfrec.bit_flags_1 & 0x20) == 0) {
           // CIP is off for this item on the IDF - force standard label.
           memset(pLPR->PHF_label_type, 0x30, sizeof(pLPR->PHF_label_type));
        } else {
           memcpy(pLPR->PHF_label_type, phfrec.aCurrentLabel, sizeof(pLPR->PHF_label_type));
        }
    } else {
        // there is no price history for this item - set defaults.
        memset(pLPR->was_price1, 0x30, sizeof(pLPR->was_price1));
        memset(pLPR->was_price2, 0x30, sizeof(pLPR->was_price2));
        memset(pLPR->PHF_label_type, 0x30, sizeof(pLPR->PHF_label_type));
    }

    // read the SRITML for this item to obtain the MS marker. if record not found set flag to "X"
    usrrc = SritmlOpen();

    MEMCPY(sritmlrec.abItemCode, irfrec.boots_code);
    sritmlrec.ubRecChain = 0;
    usrrc = SritmlRead(__LINE__);
    if (usrrc < 0) {
        // item is not on an active planner
        memset(pLPR->MS_marker,'X',sizeof(pLPR->MS_marker));
    } else {
        if (sritmlrec.uwCoreItemCount > 1) {
           // item is multi-sited
           memset(pLPR->MS_marker,'Y',sizeof(pLPR->MS_marker));
        } else {
           // item is not multi-sited
           memset(pLPR->MS_marker,'N',sizeof(pLPR->MS_marker));
        }
    }
    usrrc = SritmlClose( CL_SESSION );

    // return LPR transaction
    memcpy(pLPR->cmd, "LPR", sizeof(pLPR->cmd));
    out_lth = LRT_LPR_LTH;

    return RC_OK;
}

typedef struct LRT_ACK_Txn {
   BYTE cmd[3];
   BYTE msg[63];
} LRT_ACK;                                    // +ve acknowledge
#define LRT_ACK_LTH sizeof(LRT_ACK)

void prep_ack( BYTE *msg ) {                                                //SDH 23-Aug-2006 Planners // BMG 1.7 10-09-2008
   memcpy(((LRT_ACK*)out)->cmd, "ACK", 3);                                  //SDH 23-Aug-2006 Planners
   strncpy(((LRT_ACK*)out)->msg, msg, sizeof(((LRT_ACK*)out)->msg));        //SDH 23-Aug-2006 Planners
   out_lth = LRT_ACK_LTH;                                                   //SDH 23-Aug-2006 Planners
}

void prep_nak( BYTE *msg ) {                                                //SDH 23-Aug-2006 Planners // BMG 1.7 10-09-2008
   memcpy(((LRT_NAK*)out)->cmd, "NAK", 3);                                  //SDH 23-Aug-2006 Planners
   strncpy(((LRT_NAK*)out)->msg, msg, sizeof(((LRT_NAK*)out)->msg));        //SDH 23-Aug-2006 Planners
   out_lth = LRT_NAK_LTH;                                                   //SDH 23-Aug-2006 Planners
}

void SignOffNak ( char * pNAK, char * pMsg, WORD * iLength )          // BMG 1.7 10-09-2008
{
   memcpy( pNAK, "NAK", 3 );
   memcpy( pMsg,
           "Store close has      "
           "started please       "
           "sign off.            "
           "*****************", 80 );
   *iLength = LRT_NAK_LTH;
}

void prep_pq_full_nak(void) {                                                                          // BMG 1.7 10-09-2008
    prep_nak("ERROR  SELs or gap  report stalled.     Contact help desk.  ");//SDH 23-Aug-2006 Planners
}

BOOLEAN IsStoreClosed(void) {                                               // SDH 26-11-04 CREDIT CLAIM // BMG 1.7 10-09-2008
    if (cStoreClosed == 'Y') {                                              // SDH 26-11-04 CREDIT CLAIM
        prep_nak("Store close in progress please try later");               //SDH 23-Aug-2006 Planners
        return TRUE;                                                        // SDH 26-11-04 CREDIT CLAIM
    }                                                                       // SDH 26-11-04 CREDIT CLAIM
    return FALSE;                                                           // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

BOOLEAN IsHandheldUnknown(void) {                                           // SDH 26-11-04 CREDIT CLAIM // BMG 1.7 10-09-2008
    if (lrtp[hh_unit] == NULL) {                                            // SDH 26-11-04 CREDIT CLAIM
        // We haven't seen this handheld before                             // SDH 26-11-04 CREDIT CLAIM
        prep_nak("ERRORPlease sign off and then sign on again");            //SDH 23-Aug-2006 Planners
        return TRUE;                                                        // SDH 26-11-04 CREDIT CLAIM
    }                                                                       // SDH 26-11-04 CREDIT CLAIM
    return FALSE;                                                           // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

void UpdateActiveTime (void) {                                              // SDH 26-11-04 CREDIT CLAIM // BMG 1.7 10-09-2008
    TIMEDATE now;                                                           // SDH 26-11-04 CREDIT CLAIM
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                              // SDH 26-11-04 CREDIT CLAIM
    lrtp[hh_unit]->last_active_time = now.td_time;                          // SDH 26-11-04 CREDIT CLAIM
}                                                                           // SDH 26-11-04 CREDIT CLAIM

BOOLEAN IsReportMntActive(void) {                                           // 4-11-04 PAB               // BMG 1.7 10-09-2008

    URC urc = process_rfok();                                               // 17-3-2005 SDH
    if ((urc <= RC_DATA_ERR) ||                                             // 17-3-2005 SDH
        (rfokrec.rfmaint == 'S')) {                                         // 4-11-04 PAB
        prep_nak("Report Maintenance in progress please try again later");  //SDH 23-Aug-2006 Planners
        if (lrtp[hh_unit]->fnum3!=0L) {                                     // 11-11-04 PAB
            // if this unit has any report open then close it               // 11-11-04 PAB
            s_close( 0, lrtp[hh_unit]->fnum3 );                             // 11-11-04 PAB
            lrtp[hh_unit]->fnum3 = 0L;                                      // 11-11-04 PAB
        }                                                                   // 11-11-04 PAB
        return TRUE;                                                        // 11-11-04 PAB
    }                                                                       // 11-11-04 PAB
    return FALSE;                                                           // 11-11-04 PAB
}                                                                           // 11-11-04 PAB



typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE pass[3];
   BYTE abAppID[3];                           // SDH 07-12-2004
   BYTE AppVer[4];                            // BMG 10-09-2008 14 MC70
   BYTE MAC[12];                              // BMG 10-09-2008 14 MC70
   BYTE DevType[1];                           // BMG 10-09-2008 14 MC70
   BYTE IPADDR[15];                           // BMG 10-09-2008 14 MC70
   BYTE FreeMem[8];                           // BMG 10-09-2008 14 MC70
} LRT_SOR;                                    // Master signon request
#define LRT_SOR_LTH sizeof(LRT_SOR)

void Log_Asset(char *inbound) {  //Log asset data passed in the SOR message // BMG 1.7 10-09-2008 MC70

    B_DATE nowDate;
    B_TIME nowTime;
    WORD wAppID;

    LRT_SOR* pSOR = (LRT_SOR*)inbound;

    //Convert the application ID to an int
    wAppID = satoi(pSOR->abAppID, sizeof(pSOR->abAppID));

    if (pdtasset.fnum >0) {
        memcpy( pdtassetrec.MAC, pSOR->MAC, 12);
        memcpy( pdtassetrec.Dev_Type, pSOR->DevType, 1);
        memcpy( pdtassetrec.IP_Addr, pSOR->IPADDR,15);
        if (wAppID >= 0 && wAppID < 10) {
            memcpy(pdtassetrec.AppVer[wAppID].AppVer, pSOR->AppVer, sizeof(pSOR->AppVer));
        }
        GetSystemDate(&nowTime, &nowDate);
        sprintf( sbuf, "%04d%02d%02d", nowDate.wYear, nowDate.wMonth, nowDate.wDay);
        memcpy( pdtassetrec.Date_Last_Dock, sbuf, 8);
        sprintf( sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);
        memcpy( pdtassetrec.Time_Last_Dock, sbuf, 4);
        memcpy( pdtassetrec.User_Last_Dock, pSOR->opid, 3);
        memcpy( pdtassetrec.Mem_Free, pSOR->FreeMem, 8);
        WritePDTAsset(__LINE__);
    }

}



// ------------------------------------------------------------------------------------
//
// SOR - Sign On Request
//
//
//
// ------------------------------------------------------------------------------------

void SignOn(char* inbound, WORD wReqLen) {                      // Streamline SDH 17-Sep-2008

    //Working variables                                         // 07-12-04 SDH
    TIMEDATE now;                                               // Streamline SDH 17-Sep-2008
    WORD wAppID;                                                // 07-12-04 SDH
    URC usrrc;                                                  // Streamline SDH 17-Sep-2008
    LONG rc2;                                                   // Streamline SDH 17-Sep-2008
    LONG rc;                                                    //Picking list locked KK 16-Oct-2012
    LONG rec;                                                   //Picking list locked KK 16-Oct-2012
    WORD wLastPicker;                                           //Picking list locked KK 16-Oct-2012
    WORD wCreator;                                              //Picking list locked KK 16-Oct-2012
    WORD wNewSignin;                                            //Picking list locked KK 16-Oct-2012
    BYTE authority[1], username[32];                            // Streamline SDH 17-Sep-2008
    BYTE cStockAccess;                                          //CSk 12-03-2012 SFA
    BOOLEAN repeat;                                             // TAT 24-10-2012

    //Setup views                                               // 07-12-04 SDH
    LRT_SOR* pSOR = (LRT_SOR*)inbound;                          // 07-12-04 SDH

    if (IsStoreClosed()) return;                                // 07-12-04 SDH

    // always ensure the PGF is closed this file is opened on demand in the
    // ENQ function, and always closed, but only if the item update suite is not
    // active. As PSB20 suite does not issue a liCLS to TRANSACT then if a user
    // does attempt to sign on during the night then we should always do our
    // best to make sure that the PSB20/ or PSB26 run does not fail cos of us !
    PgfClose( CL_ALL );                                        // PAB 13-12-04

    // verify that batch suite is not running
    // functionalised the RFOK file open / read / close         // 04-11-04 PAB
    usrrc = process_rfok();                                     // 04-11-04 PAB
    if (usrrc<=RC_DATA_ERR) {                                   // 24-08-04 PAB
        prep_nak("ERRORUnable to open RFOK file. "              // SDH 23-Aug-2006 Planners
                 "Check appl event logs" );                     // 24-08-04 PAB
        return;                                                 // 24-08-04 PAB
    }

    // verify that recallsbatch suite is not running
    // functionalised the RECOK file open / read / close        // 24-05-07 PAB Recalls
    usrrc = process_recok();                                    // 24-05-07 PAB Recalls
    if (usrrc != RC_OK) return;                                 // 24-05-07 PAB Recalls

    //Convert the application ID to an int                      // 07-12-04 SDH
    wAppID = satoi(pSOR->abAppID, sizeof(pSOR->abAppID));       // 07-12-04 SDH

    //Shelf Management                                          // 07-12-04 SDH
    // RFMAINT will be blocked at report browser level to allow // 04-11-04 PAB
    // the rest of the application to be used                   // 04-11-04 PAB
    if ((wAppID == 0) || (wAppID == 1)) {                       // 24-08-04 PAB
        if ((rfokrec.rfaudit == 'S') ||                         // 24-08-04 PAB
            (rfokrec.rfpikmnt == 'S')) {                        // 24-08-04 PAB
            prep_nak("File Maintenance in progress please try " //SDH 23-Aug-2006 Planners
                     "again in 10 minutes" );                   // 24-08-04 PAB
            return;                                             // 24-08-04 PAB
        }                                                       // 07-12-04 SDH

    //Credit Claim / UOD                                        // 07-12-04 SDH
    } else if (wAppID == 2) {                                   // 07-12-04 SDH
        if (rfokrec.rfccmnt == 'S') {                           // 07-12-04 SDH
            prep_nak("File Maintenance in progress please try " //SDH 23-Aug-2006 Planners
                      "again in 10 minutes" );                  // 24-08-04 PAB
            return;                                             // 24-08-04 PAB
        }                                                       // 07-12-04 SDH
    }                                                           // 07-12-04 SDH


    // Allocate handheld a table entry
    disp_msg("Allocate lrt table...");
    usrrc = alloc_lrt_table( hh_unit );
    if (usrrc<RC_IGNORE_ERR) {
        prep_nak("ERROR unable to allocate storage. "           //SDH 23-Aug-2006 Planners
                  "Check appl event logs " );
        return;
    }

    // Log current time
    disp_msg("Log current time...");
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );
    lrtp[hh_unit]->last_active_time = now.td_time;
    memcpy( (BYTE *)&(lrtp[hh_unit]->txn), (BYTE *)inbound, 3); // Streamline SDH 17-Sep-2008
    memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );

    // Set state
    lrtp[hh_unit]->state = ST_LOGGED_ON;

    // Reset misc counts (Unused at present)
    lrtp[hh_unit]->count1 = 0;
    lrtp[hh_unit]->count2 = 0;

    //Set the device type in the table
    memset( (BYTE *)&(lrtp[hh_unit]->Type),'R', 1);
    //Call the asset logging function.                          // BMG 10-09-2008 18 MC70
    if (wReqLen > 12) {                                         // BMG 10-09-2008 18 MC70
        memcpy(lrtp[hh_unit]->Type, pSOR->DevType, 1);          // BMG 10-09-2008 18 MC70
        memcpy(lrtp[hh_unit]->MAC, pSOR->MAC, 12);              // BMG 8 17-12-2009 RF Stabilisation
        Log_Asset(inbound);                                     // BMG 10-09-2008 18 MC70
    }                                                           // BMG 10-09-2008 18 MC70

    ///////////////////////////////////////////////////////////////////////
    // Get RF attributes from control file                               //
    ///////////////////////////////////////////////////////////////////////

    disp_msg("Read RFSCF...");

    //Open RFSCF
    usrrc = RfscfOpen();
    if (usrrc!=RC_OK) {
        prep_nak("ERROR unable to open RFSCF file. "            //SDH 23-Aug-2006 Planners
                  "Please phone help desk." );
        return;
    }

    disp_msg("SOR read rfscf");

    //Read RFSCF record 1, 2, 3
    rc2 = RfscfRead(0L, __LINE__);
    rc2 = RfscfRead(1L, __LINE__);
    rc2 = RfscfRead(2L, __LINE__);

    // Check POG status here after reading is POGs are active from the RFSCF.

    // Check POG status here after reading is POGs are active from the RFSCF.
    //SDH 20-May-2009 Model Day.  if (rfscfrec1and2.cPlannersActive == 'Y') { //PAB 27-Nov-2006
    // Don't do this check for Goods In because Goods In can be     //BMG 16-06-2009 Goods In
    // Don't do this check for Automated date & time sync           //AAV 21-10-2015 Automated Date and Time
    //if ((wAppID != 5) || (wAppID != 6)){                          //BMG 16-06-2009 Goods In //BRG 07-07-2016 reversal of the fix
	  if (wAppID  != 5)    {                                        //BRG 07-07-2016 Reversal of the enhancement done for 16A                          
        //Open, read, close POGOK                                   //SDH 23-Aug-2006 Planners
        usrrc = PogokOpen();                                        //SDH 23-Aug-2006 Planners
        if (usrrc < RC_OK) {                                        //SDH 23-Aug-2006 Planners
            prep_nak("ERRORUnable to open POGOK file. "             //SDH 23-Aug-2006 Planners
                     "Please phone help desk.");                    //SDH 23-Aug-2006 Planners
            return;                                                 //SDH 23-Aug-2006 Planners
        }                                                           //SDH 23-Aug-2006 Planners
        rc2 = PogokRead(0, __LINE__);                               //SDH 23-Aug-2006 Planners
        PogokClose(CL_SESSION);                                     //SDH 23-Aug-2006 Planners
        if (rc2 > 0) {                                              //SDH 23-Aug-2006 Planners
            if (pogokrec.cSRP04 == 'S' ||                           //SDH 23-Aug-2006 Planners
                pogokrec.cSRP05 == 'S' ||                           //SDH 23-Aug-2006 Planners
                pogokrec.cSRP06 == 'S' ||                           //SDH 23-Aug-2006 Planners
                pogokrec.cSRP07 == 'S' ||                           //SDH 23-Aug-2006 Planners
                pogokrec.cSRP10 == 'S' ) {                          //PAB 21-Sept-2006 Planners
                //pogokrec.cSRP19 == 'S') {                           //SDH 23-Aug-2006 Planners
                prep_nak("Planner maintenance in progress. "        //SDH 23-Aug-2006 Planners
                         "Please try again in 10 minutes" );        //SDH 23-Aug-2006 Planners
                return;                                             //SDH 23-Aug-2006 Planners
            }                                                       //SDH 23-Aug-2006 Planners
        }                                                           //SDH
    }                                                               //PSB 27-Nov-2006

    //If Credit Claiming is active then lock the CCDMY to       // 16-11-04 SDH
    //prevent PDTs from using Credit Claiming                   // 16-11-04 SDH
    //Only call the open routine so that the number of          // 16-11-04 SDH
    //sessions is not continuously incremented                  // 16-11-04 SDH
    if (rfscfrec3.bCCActive == 'Y') {                           // 16-11-04 SDH // BMG 24-02-2009
        CcdmyOpenLocked();                                      // 16-11-04 SDH
    } else {                                                    // 16-11-04 SDH
        CcdmyClose();                                          // 16-11-04 SDH
    }                                                           // 16-11-04 SDH

    // Get SEL printer status                                   // 18-02-04 PAB
    disp_msg("SOR get printer status");                         // 17-05-04 PAB
    //write_gap = satol(rfscfrec1and2.phase,1);                 // 16-11-04 SDH
    //lne = 0;                                                  // 18-05-04 PAB

    //This code was previously only executed if the gap         // SDH 04-05-06  A6C: Bug fix
    //config was 4 or more.  No longer needed.                  // SDH 04-05-06  A6C: Bug fix
    //One IF statement removed.                                 // SDH 04-05-06  A6C: Bug fix
    usrrc = PrtctlOpen();                                       // 18-02-04 PAB
    if (usrrc <= RC_DATA_ERR) {                                 // 18-02-04 PAB
        prep_nak("ERRORUnable to open PRTCTL file. "            //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );                    // 18-02-04 PAB
        return;                                                 // 18-02-04 PAB
    }

    // read the current status record & close the file          // 18-02-04 PAB
    rc2 = PrtctlRead(0L, __LINE__);                             // Streamline SDH 17-Sep-2008
    PrtctlClose(CL_SESSION);                                    // 18-02-04 PAB

    usrrc = PrtlistOpen();                                      // Streamline SDH 17-Sep-2008
    if (usrrc <= RC_DATA_ERR) {                                 // 17-05-04 PAB
        prep_nak("ERRORUnable to open PRTLIST file. "           //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );                    // 17-05-04 PAB
        return;                                                 // 17-05-04 PAB
    }                                                           // 17-05-04 PAB

    memset( (BYTE *)&prtlistrec, 0x00, sizeof(PRTLIST_REC) );   // 17-05-04 PAB
    rc2 = PrtlistRead(0L, __LINE__);                            // Streamline SDH 17-Sep-2008

    memcpy(((LRT_SNR*)&out)->snrprtdesc,                        //SDH 10-May-2006
           (BYTE*)&prtlistrec, 200);                            //SDH 10-May-2006
    PrtlistClose(CL_SESSION);                                   // Streamline SDH 17-Sep-2008

    // Authorise user
    usrrc = AfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open EALAUTH file. "           //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }
    usrrc = authorise_user( ((LRT_SOR *)(inbound))->opid,
                            ((LRT_SOR *)(inbound))->pass,
                            (BYTE *)&authority,
                            (BYTE *)&username,
                            (BYTE *)&cStockAccess);             //CSk 12-03-2012 SFA

    AfClose( CL_SESSION );
    if (usrrc<RC_IGNORE_ERR) {
        prep_nak("ERRORUser authorisation failed. "             //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    } 
    //if ((usrrc == RC_OK) || (wAppID == 6)) {                  //BRG 07-07-2016 Reversal of the fix done as part of 16A
    if (usrrc == RC_OK)  {                                      //BRG 07-07-2016 Reversal of the fix done as part of 16A

        // User authorised
        // Save user ID & name
        memcpy(lrtp[hh_unit]->user, ((LRT_SOR *)(inbound))->opid, 3 );
        memcpy(lrtp[hh_unit]->abOpName, username,               // 16-11-2004 SDH
               sizeof(lrtp[hh_unit]->abOpName));                // 16-11-2004 SDH

        // Prepare SNR                                          // 16-11-2004 SDH
        buildSNR((LRT_SNR *)&out, prtctlrec.prtnum,             // 16-11-2004 SDH
                 ((LRT_SOR *)(inbound))->opid, authority[0],    // 16-11-2004 SDH
                 username, rfscfrec1and2.ossr_store,            // SDH 20-May-2009 Model Day
                 cStockAccess);                                 //CSk 12-03-2012 SFA
        //SDH 20-May-2009 Model Day.  rfscfrec1and2.cPlannersActive);                // 25-Sep-2006 SDH Planners

        out_lth = LRT_SNR_LTH;                                  // 17-10-2006 SDH Bug fix
        //authorised = TRUE;                                    // 16-11-2004 SDH

        // Audit                                                // 16-11-2004 SDH
        ((LRTLG_SOR *)dtls)->authority[0] = 'Y';                // 16-11-2004 SDH
        memset( ((LRTLG_SOR *)dtls)->resv, 0x00, 11 );          // 16-11-2004 SDH
        lrt_log( LOG_SOR, hh_unit, dtls );                      // 16-11-2004 SDH

    } else {

        //authorised = FALSE;

        // Audit
        memcpy( ((LRTLG_SOR *)dtls)->authority, "N", 1);
        memset( ((LRTLG_SOR *)dtls)->resv, 0x00, 11 );
        lrt_log( LOG_SOR, hh_unit, dtls );

        // Deallocate handheld's table entry
        usrrc = dealloc_lrt_table( hh_unit );

        // User not authorised
        prep_nak("User ID unknown or incorrect password");
        return;
    }

    usrrc = IrfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open EALITEMR file. "          //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }

    usrrc = IrfdexOpen();                                      // SDH 14-01-2005 Promotions
    if (usrrc<=RC_DATA_ERR) {                                   // SDH 14-01-2005 Promotions
        prep_nak("ERRORUnable to open IRFDEX file. "            // SDH 23-Aug-2006 Planners
                  "Check appl event logs" );                    // SDH 14-01-2005 Promotions
        IrfClose( CL_SESSION );                                // SDH 14-01-2005 Promotions
        return;                                                 // SDH 14-01-2005 Promotions
    }                                                           // SDH 14-01-2005 Promotions

    usrrc = IdfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open IDF file. "               //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        IrfClose( CL_SESSION );
        IrfdexClose( CL_SESSION );                             // SDH 14-01-2005 Promotions
        return;
    }
    usrrc = IsfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open ISF file. "               //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        IdfClose( CL_SESSION );
        IrfClose( CL_SESSION );
        IrfdexClose( CL_SESSION );                             // SDH 14-01-2005 Promotions
        return;
    }

    usrrc = RfhistOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open RFHIST file. "            //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        IdfClose( CL_SESSION );
        IrfClose( CL_SESSION );
        IrfdexClose( CL_SESSION );                             // SDH 14-01-2005 Promotions
        IsfClose( CL_SESSION );
        PgfClose( CL_SESSION );                                     // PAB 23-10-03
        return;
    }

    //Start of fix for Picking list locked issue KK 16-Oct-2012 Rev13
    rec = 1  ;                     //Picking list locked KK 16-Oct-2012
    rc = 1 ;                      //Picking list locked KK 16-Oct-2012
    usrrc = open_pllol();        //Picking list locked KK 16-Oct-2012
    if (usrrc < RC_IGNORE_ERR)   //Picking list locked KK 16-Oct-2012
    {
        prep_nak("ERRORUnable to open PLLOL file. Check appl event logs"); //Picking list locked KK 16-Oct-2012
        return;                 //Picking list locked KK 16-Oct-2012
    }
    while (rc > 0L)              //Picking list locked KK 16-Oct-2012
    {
        rc = ReadPllol(rec, __LINE__);                                 //Picking list locked KK 16-Oct-2012
        wLastPicker = satoi(pllolrec.picker, 3);                      //Picking list locked KK 16-Oct-2012
        wCreator = satoi(pllolrec.creator, sizeof(pllolrec.creator)); //Picking list locked KK 16-Oct-2012
        wNewSignin = satoi(((LRT_SOR *)(inbound))->opid, 3);         //Picking list locked KK 16-Oct-2012

        //Condition to verify if Last picker id is 0,the creator id is same as the sign in id and the list status is "A"
        if  ((wLastPicker == 0) && (wCreator == wNewSignin) && (pllolrec.list_status[0] == 'A'))  //Picking list locked KK 16-Oct-2012
        {
            pllolrec.list_status[0] = 'U' ;              //Picking list locked KK 16-Oct-2012
            rc2 = WritePllol(rec, __LINE__);            //Picking list locked KK 16-Oct-2012
            if (rc2<=0L)                                //Picking list locked KK 16-Oct-2012
            {
                prep_nak("ERRORUnable to write to PLLOL. "  //Picking list locked KK 16-Oct-2012
                         "Check appl event logs" );         //Picking list locked KK 16-Oct-2012
                close_pllol( CL_SESSION );                  //Picking list locked KK 16-Oct-2012
                return;                                     //Picking list locked KK 16-Oct-2012
            }                                               //Picking list locked KK 16-Oct-2012
        }
        rec++;                                             //Picking list locked KK 16-Oct-2012
    }
    close_pllol( CL_SESSION );                             //Picking list locked KK 16-Oct-2012
    //End of fix for Picking list locked issue KK 16-Oct-2012

    //Open planner data (ignore errors)                         // SDH 12-Oct-2006 Planners
    SrpogOpen();                                               // SDH 12-Oct-2006 Planners
    SrmapOpen();                                               //CSk 12-03-2012 SFA
    SrmodOpen();                                               // SDH 12-Oct-2006 Planners
    SritmlOpen();                                              // SDH 12-Oct-2006 Planners
    SritmpOpen();                                              // SDH 12-Oct-2006 Planners
    SrpogifOpen();                                             // SDH 12-Oct-2006 Planners
    SrpogilOpen();                                             // SDH 12-Oct-2006 Planners
    SrpogipOpen();                                             // SDH 12-Oct-2006 Planners
    SrcatOpen();                                               // SDH 12-Oct-2006 Planners
    SrsxfOpen();                                               // SDH 12-Oct-2006 Planners

    //Increment number of sessions
    sess++;

    //Fix for locked Count lists - TAT 24-10-2012
    // Open CLOLF and look for any count list being picked by the user
    // If found set the status to 'Partially' counted, such that the list
    // is no longer locked.
    if (memcmp(((LRT_SOR *)(inbound))->cmd,                               // TAT 23-11-2012 SFA
                           "SOR", 3) == 0) {                              // TAT 23-11-2012 SFA
        // A proper sign on and not a reconnect
        rec = 0;                                                          // TAT 24-10-2012 SFA
        usrrc = ClolfOpen();                                              // TAT 24-10-2012 SFA
        if (usrrc == RC_OK) {                                             // TAT 24-10-2012 SFA
            do {                                                          // TAT 24-10-2012 SFA
                repeat = TRUE;                                            // TAT 24-10-2012 SFA
                //os = rec * CLOLF_RECL;                                  // TAT 24-10-2012 SFA
                rc = ClolfRead(rec, __LINE__);                            // TAT 24-10-2012 SFA
                if (rc <= 0L) {                                           // TAT 24-10-2012 SFA
                    repeat = FALSE;                                       // TAT 24-10-2012 SFA
                } else {                                                  // TAT 24-10-2012 SFA
                    // Record found                                       // TAT 24-10-2012 SFA
                    if (memcmp(((LRT_SOR *)(inbound))->opid,              // TAT 24-10-2012 SFA
                               clolfrec.abPickerId, 3) == 0) {            // TAT 24-10-2012 SFA
                        if (clolfrec.bListStatus == 'A') {                // TAT 24-10-2012 SFA
                            clolfrec.bListStatus = 'P';                   // TAT 24-10-2012 SFA
                            if (clolfrec.uiTotalItems ==                  // TAT 06-12-2012 SFA
                                    clolfrec.uiOutSalesFloorCnt) {        // TAT 06-12-2012 SFA
                                clolfrec.bListStatus = 'I';               // TAT 06-12-2012 SFA
                            }                                             // TAT 06-12-2012 SFA
                            rc2 = ClolfWrite(rec, __LINE__);              // TAT 24-10-2012 SFA
                            if (rc2 <= 0) {                               // TAT 24-10-2012 SFA
                                repeat = FALSE;                           // TAT 24-10-2012 SFA
                            }                                             // TAT 24-10-2012 SFA
                        }                                                 // TAT 24-10-2012 SFA
                    }                                                     // TAT 24-10-2012 SFA
                }                                                         // TAT 24-10-2012 SFA
                rec++;                                                    // TAT 24-10-2012 SFA
            } while (repeat);                                             // TAT 24-10-2012 SFA
            ClolfClose(CL_SESSION);                                       // TAT 24-10-2012 SFA
        }                                                                 // TAT 24-10-2012 SFA
    }                                                                     // TAT 23-11-2012 SFA


}


// ------------------------------------------------------------------------------------
//
// OFF - Sign Off
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_OFF_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_OFF;                                    // Master signoff
#define LRT_OFF_LTH sizeof(LRT_OFF)


void SignOff(char* inbound) {

    if (lrtp[hh_unit] == NULL) {
        // We haven't seen this handheld before
        prep_nak("ERRORForced Sign off in progress. Thank you.");   //SDH 23-Aug-2006 Planners
        return;
    }

    if (strlen( ((LRT_OFF *)(inbound))->opid )==0) {                // 24-8-04 PAB
        // this is a rogue OFF command from a Pocket PC which has   // 24-8-04 PAB
        // been IPLed and is flushing its IP buffer. no user ID.    // 24-8-04 PAB
        // but the unit number may be invalid. IGNORE or you may be // 24-8-04 PAB
        // signing off another unit, which may then appear to lock  // 24-8-04 PAB
        // or hang.                                                 // 24-8-04 PAB
        disp_msg("Rogue OFF Flushing");                             // 24-8-04 PAB
        sess--;                                                     // 24-8-04 PAB
        if (sess < 0) sess = 0;                                     // 24-8-04 PAB
        return;                                                     // 24-8-04 PAB
    }                                                               // 24-8-04 PAB

    //Process both just in case                                     // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_CB );                            // BMG 01-09-2008 17 ASN/Directs
    process_workfile( hh_unit, SYS_PUB );                           // BMG 01-09-2008 17 ASN/Directs
    process_workfile( hh_unit, SYS_DIR );                           // BMG 01-09-2008 17 ASN/Directs

    RfhistClose( CL_SESSION );
    IsfClose( CL_SESSION );
    IdfClose( CL_SESSION );
    IrfClose( CL_SESSION );
    IrfdexClose( CL_SESSION );                                     // SDH 14-01-2005 Promotions
    RfscfClose( CL_SESSION );
    PgfClose( CL_SESSION );                                        // PAB 23-10-03

    // Close planner files                                          // SDH 12-Oct-2006 Planners
    SrpogClose(CL_SESSION);                                        // SDH 12-Oct-2006 Planners
    SrmodClose(CL_SESSION);                                        // SDH 12-Oct-2006 Planners
    SritmlClose(CL_SESSION);                                       // SDH 12-Oct-2006 Planners
    SritmpClose(CL_SESSION);                                       // SDH 12-Oct-2006 Planners
    SrpogifClose(CL_SESSION);                                      // SDH 12-Oct-2006 Planners
    SrpogilClose(CL_SESSION);                                      // SDH 12-Oct-2006 Planners
    SrpogipClose(CL_SESSION);                                      // SDH 12-Oct-2006 Planners
    SrcatClose(CL_SESSION);                                        // SDH 12-Oct-2006 Planners
    SrsxfClose(CL_SESSION);                                        // SDH 12-Oct-2006 Planners

    prep_ack( "" );                                                 //SDH 23-Aug-2006 Planners

    // Deallocate handheld's table entry
    dealloc_lrt_table( hh_unit );

    sess--;
    if (sess < 0) sess = 0;                                         // 1-9-2004 PAB

}

// ------------------------------------------------------------------------------------
//
// SIE - GetStoreNumber
//
//
//
// ------------------------------------------------------------------------------------

/*
// version 6.0 MyStoreNet changes                                // 7-7-2004 PAB
typedef struct  {                                     // 7-7-2004 PAB
  BYTE cmd[3];                                                   // 7-7-2004 PAB
  BYTE opid[3];                                                  // 7-7-2004 PAB
} LRT_SIE;                                                       // 7-7-2004 PAB
#define LRT_SIE_LTH sizeof(LRT_SIE)                              // 7-7-2004 PAB
*/
                                                                 // 7-7-2004 PAB
typedef struct  {                                     // 7-7-2004 PAB
  BYTE cmd[3];                                                   // 7-7-2004 PAB
  BYTE store_number[4];                                          // 7-7-2004 PAB
} LRT_SIR;                                                       // 7-7-2004 PAB
#define LRT_SIR_LTH sizeof (LRT_SIR)                             // 7-7-2004 PAB

void GetStoreNumber(char* inbound) {

    LRT_SIR* pSIR = (LRT_SIR*)out;                              // Streamline SDH 17-Sep-2008
    LONG rc2;                                                   // Streamline SDH 17-Sep-2008

    UNUSED(inbound);                                            // Streamline SDH 17-Sep-2008
    hh_unit = 255;

    disp_msg("Process CMD_SIE");

    InvokOpen();                                                // 7-7-04 PAB
    rc2 = InvokRead(0L, __LINE__);
    InvokClose(CL_SESSION);                                     // 7-7-04 PAB

    if (rc2 <= 0L) {                                            // 7-7-04 PAB
        prep_nak("ERRORUnable to READ the INVOK. "              //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );                    // 7-7-04 PAB
        return;                                                  // 7-7-04 PAB
    }                                                           // 7-7-04 PAB

    MEMCPY(pSIR, "SIR");
    MEMCPY(pSIR->store_number, invokrec.store_no);
    out_lth = LRT_SIR_LTH;                                      // 7-7-04 PAB

}


// ------------------------------------------------------------------------------------
//
// ENQ - Item Enquiry
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_ENQ_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE enq_type[1];          // [I]tem / [P]arent
   BYTE function[1];          // [P]Chk / [ ]Other
   BYTE item_code[13];
   BYTE stock_req_flag[1];    // Stock figure required on EQR [Y]es / [N]o
   BYTE cUpdateOssrItem;      // ' '-No change, 'N'-non-OSSR, 'O'-change to OSSR    // SDH 17-11-04 OSSR WAN
} LRT_ENQ;                                      // Stock enquiry
#define LRT_ENQ_LTH sizeof(LRT_ENQ)

// v2.1
typedef struct LRT_EQR_Txn {
   BYTE cmd[3];
   BYTE boots_code[7];
   BYTE parent_code[7];
   BYTE item_desc[20];
   BYTE item_price[6];
   BYTE sel_desc[45];
   BYTE status[1];
   BYTE supply_method[1];
   BYTE redeemable[1];
   BYTE stock_figure[6];      // Items
   BYTE pchk_target[4];
   BYTE pchk_done[4];
   BYTE price_emu[6];         // Cents
   BYTE prim_curr[1];         // [S]terling / [E]cu
   BYTE item_code[13];
   BYTE active_deal_flag[1];  // [Y]es / [N]o
   BYTE check_accepted[1];    // [Y]es / [N]o / [ ]n/a
   BYTE reject_msg[14];       // ASCII "DOW DD/MM/YYYY", set if check rejected
   BYTE cBusCentre;           // SDH 17-11-04 OSSR WAN
   BYTE abBusCentreDesc[14];  // SDH 17-11-04 OSSR WAN
   BYTE cOssrItem;            // SDH 17-11-04 OSSR WAN
   DEALSUM Deal[10];          // SDH 09-12-04 PROMOTIONS
   BYTE abCoreCount[3];       // SDH 12-Oct-06 Planners
   BYTE abNonCoreCount[3];    // SDH 12-Oct-06 Planners
   BYTE cRecallItem[1];       // PAB 22-5-07 Recalls "Y" if item is on recall. else "N"
   BYTE cMarkdown[1];         // BMG 06-02-2008 1.11 'Y' if a Markdown item else ' '
   BYTE cProductGrp[6];       // CSk 11-05-2010 Auto Fast Fill
   BYTE cRecallType;          // CSk 11-05-2010 Recalls Phase 1
   BYTE cPendSaleFlag;        //CSk 12-03-2012 SFA
} LRT_EQR;                                    // stock enquiry response
#define LRT_EQR_LTH sizeof(LRT_EQR)

void ItemEnquiry(char* inbound) {

    LRT_ENQ* pENQ = (LRT_ENQ*)inbound;                          // 17-11-04 SDH OSSR WAN
    LRT_EQR* pEQR = (LRT_EQR*)out;                              // 17-11-04 SDH OSSR WAN
    BYTE txtbuf[256];                                           // Streamline SDH 17-Sep-2008
    TIMEDATE now;                                               // Streamline SDH 17-Sep-2008
    ENQUIRY enqbuf;                                             // Streamline SDH 17-Sep-2008
    DOUBLE  dLastPriceCheckDate;                                // 17-11-04 SDH OSSR WAN
    DOUBLE  dTodayDate;                                         // 17-11-04 SDH OSSR WAN
    LONG rc2;                                                   // Streamline SDH 17-Sep-2008
    LONG day, month, year, sec;                                 // Streamline SDH 17-Sep-2008
    WORD hour, min;                                             // Streamline SDH 17-Sep-2008
    URC usrrc;                                                  // Streamline SDH 17-Sep-2008
    BOOLEAN check_accepted = FALSE;                             // Streamline SDH 17-Sep-2008
    BOOLEAN fUpdateRfhist = FALSE;                              // 17-11-04 SDH OSSR WAN
    BOOLEAN fRfhistRecFound = FALSE;                            // Streamline SDH 17-Sep-2008

    //Set up current date and time
    B_DATE nowDate;
    B_TIME nowTime;
    GetSystemDate(&nowTime, &nowDate);

    // if this is not a My StoreNet blind Sign On and
    // We haven't seen this handheld before
    if (satoi(pENQ->opid, sizeof(pENQ->opid)) != 0) {           // SDH 26-11-04 CREDIT CLAIM
        if (IsHandheldUnknown()) return;                         // SDH 26-11-04 CREDIT CLAIM
        // if this request has come from StoreNet USerID 000 then simulate
        // operator sign on processing - allocate hht table entry// 5-7-2004 PAB
    } else {
        hh_unit = 255;                                          // 20-7-2004 PAB
        usrrc = alloc_lrt_table( hh_unit );                     // 5-7-2004 PAB
        if (usrrc<RC_IGNORE_ERR) {                              // 5-7-2004 PAB
            prep_nak("ERRORUnable to allocate storage. "        //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB
        // Log current time                                     // 5-7-2004 PAB
        s_get( T_TD, 0L, (void *)&now, TIMESIZE );              // 5-7-2004 PAB
        lrtp[hh_unit]->last_active_time = now.td_time;          // 5-7-2004 PAB
        memcpy( (BYTE *)&(lrtp[hh_unit]->txn), (BYTE *)inbound, 3);    // 5-7-2004 PAB
        memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );    // 5-7-2004 PAB
                                                                // 5-7-2004 PAB
        // Set state                                            // 5-7-2004 PAB
        lrtp[hh_unit]->state = ST_LOGGED_ON;                    // 5-7-2004 PAB
        // Reset misc counts (Unused at present)                // 5-7-2004 PAB
        lrtp[hh_unit]->count1 = 0;                              // 5-7-2004 PAB
        lrtp[hh_unit]->count2 = 0;                              // 5-7-2004 PAB
        if (usrrc!=RC_OK) {                                     // 5-7-2004 PAB
            prep_nak("ERRORUnable to open RFSCF file. "         //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB

        usrrc = IrfOpen();                                      // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // 5-7-2004 PAB
            prep_nak("ERRORUnable to open EALITEMR file. "      //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            //close_rfscf( CL_SESSION );                        // 5-7-2004 pab
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB

        usrrc = IrfdexOpen();                                   // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // SDH 14-01-2005 Promotions
            prep_nak("ERRORUnable to open IRFDEX file. "        //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // SDH 14-01-2005 Promotions
            IrfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            return;                                              // SDH 14-01-2005 Promotions
        }                                                       // SDH 14-01-2005 Promotions

        usrrc = IdfOpen();                                      // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // 5-7-2004 PAB
            prep_nak("ERRORUnable to open IDF file. "           //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            IrfClose(CL_SESSION);                               // Streamline SDH 17-Sep-2008
            IrfdexClose( CL_SESSION );                          // Streamline SDH 17-Sep-2008
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB
        usrrc = IsfOpen();                                      // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // 5-7-2004 PAB
            prep_nak("ERRORUnable to open ISF file. "           //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            IdfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            IrfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            IrfdexClose( CL_SESSION );                          // Streamline SDH 17-Sep-2008
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB
        usrrc = RfhistOpen();                                   // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // 5-7-2004 PAB
            prep_nak("ERRORUnable to open RFHIST file. "        //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 5-7-2004 PAB
            IdfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            IrfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            IrfdexClose( CL_SESSION );                          // Streamline SDH 17-Sep-2008
            IsfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            return;                                              // 5-7-2004 PAB
        }                                                       // 5-7-2004 PAB

        //Open planner data (ignore errors)                     // SDH 12-Oct-2006 Planners
        SrpogOpen();                                            // Streamline SDH 17-Sep-2008
        SrmapOpen();                                            //CSk 12-03-2012 SFA
        SrmodOpen();                                            // Streamline SDH 17-Sep-2008
        SritmlOpen();                                           // Streamline SDH 17-Sep-2008
        SritmpOpen();                                           // Streamline SDH 17-Sep-2008
        SrpogifOpen();                                          // Streamline SDH 17-Sep-2008
        SrpogilOpen();                                          // Streamline SDH 17-Sep-2008
        SrpogipOpen();                                          // Streamline SDH 17-Sep-2008
        SrcatOpen();                                            // Streamline SDH 17-Sep-2008
        SrsxfOpen();                                            // Streamline SDH 17-Sep-2008

    }                                                           // 5-7-2004 PAB

    if (IsStoreClosed()) return;                                 // SDH 07-12-2004

    // verify that item update batch suite is not running
    // functionalised the JOBOK file open / read / close        // 13-12-04 PAB
    usrrc = process_jobok();                                    // 13-12-04 PAB
    // if the jobok not there then ok to continue               // 13-12-04 PAB
    // process nullifies the record space                       // 13-12-04 PAB
    // 13-12-04 PAB
    if (debug) {                                                // 13-12-04 PAB
        sprintf(msg, "RD JOBOK :");                             // 13-12-04 PAB
        disp_msg(msg);                                          // 13-12-04 PAB
        dump( jobokrec.psb21, 5 );                              // 13-12-04 PAB
    }                                                           // 13-12-04 PAB
    // 13-12-04 PAB
    if (jobokrec.psb21[0] == 'S' ||                             // 13-12-04 PAB
        jobokrec.psb22[0] == 'S' ||                             // 13-12-04 PAB
        jobokrec.psb23[0] == 'S' ||                             // 13-12-04 PAB
        jobokrec.psb24[0] == 'S' ||                             // 13-12-04 PAB
        jobokrec.psb25[0] == 'S') {                             // 13-12-04 PAB
        if (debug) {                                            // 13-12-04 PAB
            sprintf(msg, "Item Update Suite is Active");        // 13-12-04 PAB
            disp_msg(msg);                                      // 13-12-04 PAB
        }                                                       // 13-12-04 PAB
    } else {                                                    // 13-12-04 PAB
        if (debug) {                                            // 13-12-04 PAB
            sprintf(msg, "Item Update Suite is not Active");    // 13-12-04 PAB
            disp_msg(msg);                                      // 13-12-04 PAB
        }                                                       // 13-12-04 PAB
        usrrc = PgfOpen();                                      // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // PAB 23-10-03
            prep_nak("ERRORUnable to open PGF file. "           //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // PAB 23-10-03
            PgfClose( CL_SESSION );                             // Streamline SDH 17-Sep-2008
            //close_idf( CL_SESSION );                          // PAB 23-10-03
            //close_irf( CL_SESSION );                          // PAB 23-10-03
            return;                                              // PAB 23-10-03
        }
    }                                                           // 13-12-04 PAB

    usrrc = RfscfOpen();                                        // Streamline SDH 17-Sep-2008
    if (usrrc != RC_OK) {                                       // 11-09-2007 12 BMG
        prep_nak("ERRORUnable to open RFSCF file. "             // 11-09-2007 12 BMG
                  "Check appl event logs" );                    // 11-09-2007 12 BMG
        return;                                                  // 11-09-2007 12 BMG
    }                                                           // 11-09-2007 12 BMG
    rc2 = RfscfRead(0L, __LINE__);
    rc2 = RfscfRead(0L, __LINE__);
    if (rc2 <= 0L) {                                            // 11-09-2007 12 BMG
        prep_nak("ERRORUnable to read from RFSCF. "             // 11-09-2007 12 BMG
                  "Check appl event logs" );                    // 11-09-2007 12 BMG
        return;                                                  // 11-09-2007 12 BMG
    }                                                           // 11-09-2007 12 BMG
    RfscfClose(CL_SESSION);                                     // Streamline SDH 23-Sep-2008

    // Get stock details (only get stock figure if absolutely necessary)
    usrrc = stock_enquiry( (pENQ->stock_req_flag[0] == 'Y') ?   // 17-11-04 SDH OSSR WAN
                           SENQ_TSF:SENQ_SELD,                  // 17-11-04 SDH OSSR WAN
                           pENQ->item_code, &enqbuf );          // PAB 25-10-04

    PgfClose ( CL_SESSION );                                    // Streamline SDH 17-Sep-2008
    if (usrrc < RC_IGNORE_ERR) {                                // PAB 23-10-03
        if (usrrc==RC_FILE_ERR) {                               // PAB 23-10-03
            prep_nak("ERROROne or more   files are not open   " //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // PAB 23-10-03
        } else if (usrrc==RC_DATA_ERR) {                        // PAB 23-10-03
            prep_nak("ERRORUnable to access data. "             //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // PAB 23-10-03
        } else {                                                // PAB 23-10-03
            prep_nak("ERRORUndefined 4                        " //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // PAB 23-10-03
        }                                                       // PAB 23-10-03
        return;                                                  // PAB 23-10-03
    }                                                           // PAB 23-10-03
    if (usrrc!=RC_OK) {                                         // PAB 23-10-03
        // Prepare NAK                                          // PAB 23-10-03
        prep_nak("  Item not on file" );                        //SDH 23-Aug-2006 Planners
        return;                                                  // PAB 23-10-03
    }                                                           // PAB 23-10-03

    // PAB 23-10-03
    // Read RFHIST
    pack(rfhistrec.boots_code, 4, enqbuf.boots_code, 7, 1 );
    rc2 = RfhistRead(__LINE__);

    //If error reading RFHIST
    if (rc2 <= 0L) {
        if ((rc2&0xFFFF)==0x06C8 || (rc2&0xFFFF)==0x06CD) {
            fRfhistRecFound = FALSE;
        } else {
            prep_nak("ERRORUnable to read from RFHIST. "        //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            return;
        }

        //Else there is a RFHIST record
    } else {

        fRfhistRecFound = TRUE;

        // return the date even if this is not a price check 6-7-2004 PAB
        sprintf( msg, "%s %02d/%02d/%04d",
                 dayname[nowDate.wDOW], nowDate.wDay, nowDate.wMonth, nowDate.wYear );
        memcpy( pEQR->reject_msg, msg, 14 );

    }

    // Prepare EQR
    memcpy( pEQR->cmd, "EQR", 3 );
    memcpy( pEQR->boots_code, enqbuf.boots_code, 7 );
    memcpy( pEQR->parent_code, enqbuf.parent_code, 7 );
    memcpy( pEQR->item_desc, enqbuf.item_desc, 20 );
    memcpy( pEQR->item_price, enqbuf.item_price, 6 );
    memcpy( pEQR->price_emu, enqbuf.item_price,6 );             // 13-08-07 PAB mobile printing
    memcpy( pEQR->cRecallItem, enqbuf.cRecallFlag,1);           // 23-5-07 PAB Recalls
    pEQR->prim_curr[0] = rfscfrec1and2.prim_curr[0];            // 13-08-07 PAB mobile Printing

    memcpy( pEQR->cMarkdown, enqbuf.cMarkdown,1);               // 06-02-08 BMG 15
    memcpy( pEQR->cProductGrp, enqbuf.cProductGrp,6);           // CSk 11-05-2010 Auto Fast Fill
    pEQR->cRecallType = enqbuf.cRecallType;                     // CSk 11-05-2010 Recalls Phase 1
    pEQR->cPendSaleFlag = ' ';                                  //CSk 12-03-2012 SFA
    //xxx INCLUDE CODE TO READ PENDING PLANNER                  //CSk 12-03-2012 SFA
    // Check if item on a Pending Salesplanner that is active in the next N days
    GetFuturePendingSalesplanFlag( enqbuf.boots_code,           //CSk 12-03-2012 SFA
                                   &pEQR->cPendSaleFlag);       //CSk 12-03-2012 SFA

    pEQR->cBusCentre = enqbuf.cBusCentre;                       // 23-11-04 SDH
    memcpy(pEQR->Deal, enqbuf.Deal, sizeof(pEQR->Deal));        // 23-11-04 SDH

    // Obtain bus centre description                            // 17-11-04 SDH OSSR WAN
    usrrc = BcsmfOpen();                                        // Streamline SDH 17-Sep-2008
    if (usrrc<=RC_DATA_ERR) {                                   // 17-11-04 SDH OSSR WAN
        prep_nak("ERRORUnable to open BCSMF file. "             // SDH 23-Aug-2006 Planners
                  "Check appl event logs" );                    // 17-11-04 SDH OSSR WAN
        BcsmfClose( CL_SESSION );                               // Streamline SDH 17-Sep-2008
        return;                                                 // 17-11-04 SDH OSSR WAN
    }                                                           // 17-11-04 SDH OSSR WAN
    //Set up key to read                                        // 17-11-04 SDH OSSR WAN
    bcsmfrec.cBusCentre = enqbuf.cBusCentre;                    // 17-11-04 SDH OSSR WAN
    //If the read is successful then use the desc on file       // 17-11-04 SDH OSSR WAN
    //otherwise use UNKNOWN                                     // 17-11-04 SDH OSSR WAN
    if (BcsmfRead(__LINE__) < 0L) {                             // 17-11-04 SDH OSSR WAN
        memcpy(pEQR->abBusCentreDesc, "UNKNOWN       ",         // 17-11-04 SDH OSSR WAN
               sizeof(pEQR->abBusCentreDesc));                  // 17-11-04 SDH OSSR WAN
    } else {                                                    // 17-11-04 SDH OSSR WAN
        memcpy(pEQR->abBusCentreDesc, bcsmfrec.abDesc,          // 17-11-04 SDH OSSR WAN
               sizeof(pEQR->abBusCentreDesc));                  // 17-11-04 SDH OSSR WAN
    }                                                           // 17-11-04 SDH OSSR WAN
    BcsmfClose(CL_SESSION);                                     // 14-03-05 SDH OSSR WAN

    //Copy relevant SEL description into place
    //If the first two chars of the SEL desc are "X " then
    //replace it with the IDF description
    if (strncmp( enqbuf.sel_desc, "X ", 2 )!=0 &&
        strncmp( enqbuf.sel_desc, "x ", 2 )!=0) {               //TAT 06-12-2012 SFA
        memcpy( pEQR->sel_desc, enqbuf.sel_desc, 45 );
    } else {
        memset( txtbuf, 0x20, 45 );
        format_text( enqbuf.item_desc, 24, txtbuf, 45, 15 );
        memcpy( pEQR->sel_desc, txtbuf, 45 );
    }

    //If the status is 'B' or 'C' then force it to ' '.
    if (enqbuf.status[0] == 'B' || enqbuf.status[0] == 'C') {   // 16-7-2004 PAB
        enqbuf.status[0] = 0x20;                                // 16-7-2004 PAB
    }                                                           // 16-7-2004 PAB

    //Build various fields into the EQR response
    pEQR->status[0] = enqbuf.status[0];
    pEQR->supply_method[0] = enqbuf.supply_method[0];
    pEQR->redeemable[0] = enqbuf.redeemable[0];

    // Return stock figures if required
    if (pENQ->stock_req_flag[0] == 'Y') {
        memcpy(pEQR->stock_figure, enqbuf.stock_figure, 6 );
    } else {
        memset(pEQR->stock_figure, 0x20, 6 );
    }

    //If the RFHIST record was not present then initialise one  // SDH 12-05-2005 EXCESS
    if (!fRfhistRecFound) {                                     // SDH 12-05-2005 EXCESS

        fUpdateRfhist = TRUE;                                   // SDH 12-05-2005 EXCESS

        //Initialise certain fields.                            // SDH 07-01-2005
        //We now ALWAYS add a RFHIST record if it was absent    // SDH 07-01-2005
        memset(rfhistrec.date_last_gap, 0x00,                   // SDH 07-01-2005
               sizeof(rfhistrec.date_last_gap));                // SDH 07-01-2005
        memset(rfhistrec.date_last_pchk, 0x00,                  // SDH 07-01-2005
               sizeof(rfhistrec.date_last_pchk));               // SDH 07-01-2005
        memset(rfhistrec.price_last_pchk, 0x00,                 // SDH 07-01-2005
               sizeof(rfhistrec.price_last_pchk));              // SDH 07-01-2005
        rfhistrec.bUnused = 0;                                  // SDH 28-04-2005
        memset(rfhistrec.resrv, 0xFF, sizeof(rfhistrec.resrv)); // SDH 07-01-2005

        //Set the OSSR item flags as appropriate                // SDH 22-12-2004
        if (rfscfrec1and2.ossr_store != 'W') {                  // SDH 22-12-2004
            pEQR->cOssrItem = 'N';                              // SDH 22-12-2004
            rfhistrec.ubPgfOssrFlag = FALSE;                    // SDH 18-03-2005
            rfhistrec.ubItemOssrFlag = FALSE;                   // SDH 18-03-2005
        } else if (pENQ->cUpdateOssrItem != ' ') {              // SDH 22-12-2004
            pEQR->cOssrItem =                                   // SDH 22-12-2004
                (pENQ->cUpdateOssrItem == 'O' ? 'O':'N');       // SDH 22-12-2004
            rfhistrec.ubPgfOssrFlag =                           // SDH 18-03-2005
                (enqbuf.cPgfOssrFlag == 'Y');                   // SDH 18-03-2005
            rfhistrec.ubItemOssrFlag =                          // SDH 18-03-2005
                (pENQ->cUpdateOssrItem == 'O');                 // SDH 22-12-2004
        } else {                                                // SDH 22-12-2004
            pEQR->cOssrItem =                                   // SDH 22-12-2004
                (enqbuf.cOssrItem == 'Y' ? 'O':'N');            // SDH 22-12-2004
            rfhistrec.ubPgfOssrFlag =                           // SDH 18-03-2005
                (enqbuf.cPgfOssrFlag == 'Y');                   // SDH 18-03-2005
            rfhistrec.ubItemOssrFlag = (enqbuf.cOssrItem == 'Y');// SDH 18-03-2005
        }                                                       // SDH 22-12-2004

        // Item is on RFHIST file (item seen before)                // SDH 12-05-2005 EXCESS
    } else {                                                    // SDH 12-05-2005 EXCESS

        //Set the OSSR item flags as appropriate                // SDH 22-12-2004
        if (rfscfrec1and2.ossr_store != 'W') {                  // SDH 22-12-2004
            pEQR->cOssrItem = 'N';                              // SDH 22-12-2004
        } else if (pENQ->cUpdateOssrItem != ' ') {              // SDH 22-12-2004
            pEQR->cOssrItem =                                   // SDH 22-12-2004
                (pENQ->cUpdateOssrItem == 'O' ? 'O':'N');       // SDH 22-12-2004
            rfhistrec.ubItemOssrFlag =                          // SDH 22-12-2004
                (pENQ->cUpdateOssrItem == 'O');                 // SDH 22-12-2004
            fUpdateRfhist = TRUE;                               // SDH 22-12-2004
        } else {                                                // SDH 22-12-2004
            pEQR->cOssrItem =                                   // SDH 22-12-2004
                (rfhistrec.ubItemOssrFlag ? 'O':'N');           // SDH 22-12-2004
        }                                                       // SDH 22-12-2004

    }

    // Determine whether item has recently been counted for
    // '[P]rice Check' or '[C]ount and Assemble/Routine Inspection'
    if (pENQ->function[0] == 'P' ||
        pENQ->function[0] == 'C') {

        // Compare current price with last pchk price and determine
        // whether item has been recently counted
        unpack( sbuf, 6, rfhistrec.price_last_pchk, 3, 0 );
        if (satol(sbuf, 6) != satol(enqbuf.item_price, 10)) {
            check_accepted = TRUE;
            rfscfrec1and2.pchk_done++;                      // 12-1-07 PAB
            // dont count it if price ok. count is <28days
            // PCM will count it if error.
        } else {
            sysdate( &day, &month, &year, &hour, &min, &sec );
            dTodayDate = ConvGJ( day, month, year );
            unpack( sbuf, 8, rfhistrec.date_last_pchk, 4, 0 );
            day   = satol( sbuf+6, 2 );
            month = satol( sbuf+4, 2 );
            year  = satol( sbuf,   4 );
            dLastPriceCheckDate = ConvGJ( day, month, year );
            memset(sbuf,0x00,sizeof(rfhistrec.date_last_pchk));                                   // 02-11-07 13 BMG
            if (((LONG)(dTodayDate - dLastPriceCheckDate) >     // 16-11-04 SDH                   // 02-11-07 13 BMG
                (LONG)rfscfrec1and2.recheck_days) ||            // 16-11-04 SDH                   // 02-11-07 13 BMG
                memcmp(rfhistrec.date_last_pchk, sbuf, sizeof(rfhistrec.date_last_pchk)) == 0 ) { // 02-11-07 13 BMG
                check_accepted = TRUE;
                // only count if > 28day check and price is correct
                // PPC will send PCM is price incorrect and count it
                rfscfrec1and2.pchk_done++;                      // 07-04-07 SDH
            }
        }

        //If price check is allowed...
        if (check_accepted) {
            // Increment and update price checks done figure on RFSCF
            // update the RFSCF
            // rfscfrec1and2.pchk_done++;                          // 16-11-04 SDH
            rc2 = RfscfUpdate(__LINE__);                           // 03-01-05 SDH
            if (rc2<=0L) {
                prep_nak("ERRORUnable to write to RFSCF. "      //SDH 23-Aug-2006 Planners
                          "Check appl event logs" );
                return;
            }

            // Update RFHIST record
            sysdate( &day, &month, &year, &hour, &min, &sec );
            sprintf( sbuf, "%04ld%02ld%02ld", year, month, day );
            pack( rfhistrec.date_last_pchk, 4, sbuf, 8, 0 );
            pack( rfhistrec.price_last_pchk, 3, enqbuf.item_price, 6, 0 );
            fUpdateRfhist = TRUE;

            //This should NOT be reset
            //memset( rfhistrec.date_last_gap, 0x00, 4 );

            pEQR->check_accepted[0] = (check_accepted?'Y':'N');
            memset(pEQR->reject_msg, 0x20, 14);

            // Recount not allowed
        } else {

            pEQR->check_accepted[0] = (check_accepted?'Y':'N');
            sprintf( msg, "%s %02ld/%02ld/%04ld",
                     dayname[ConvDOW(dLastPriceCheckDate)], day, month, year );
            memcpy( ((LRT_EQR *)out)->reject_msg, msg, 14 );
        }
    }

    //Update the RFHIST file if a price check was accepted OR
    //an update of the OSSR item status was requested
    if (fUpdateRfhist) {
        rc2 = RfhistWrite(__LINE__);                            // SDH 18-03-2005
        if (rc2<=0L) {
            prep_nak("ERRORUnable to write to RFHIST. "         //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            return;
        }
    }

    if (enqbuf.pcheck_exempt[0] == 'N') {

        disp_msg("Price Check is blocked");

        // if we decide to send a message to say exempt from
        // price check then modify the code below.
        //
        // sprintf( msg, "%s %02ld/%02ld/%04ld",
        // dayname[ConvDOW(dLastPriceCheckDate)], day, month, year );
        // memcpy( ((LRT_EQR *)out)->reject_msg, msg, 14 );

        check_accepted = FALSE;
        pEQR->check_accepted[0] = (check_accepted?'Y':'N');     // 17-11-04 SDH OSSR WAN
        memset(pEQR->reject_msg, 0x20, sizeof(pEQR->reject_msg));    // 17-11-04 SDH OSSR WAN
    }

    //If price check or fast fill
    if ((pENQ->function[0] == 'P') || (pENQ->function[0] == 'C')) {
        sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_target );    // 17-11-04 SDH OSSR WAN
        memcpy( pEQR->pchk_target, sbuf, 4 );                   // 17-11-04 SDH OSSR WAN
        sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_done );      // 17-11-04 SDH OSSR WAN
        memcpy( pEQR->pchk_done, sbuf, 4 );                     // 17-11-04 SDH OSSR WAN

        //Else
    } else {
        memset( pEQR->pchk_target, 0x20, sizeof(pEQR->pchk_target));
        memset( pEQR->pchk_done, 0x20, sizeof(pEQR->pchk_done));
        pEQR->check_accepted[0] = ' ';
        memset( pEQR->reject_msg, 0x20, sizeof(pEQR->reject_msg));
    }

    //Add currency info to response
    sprintf(sbuf, "%06.f", emu_round((DOUBLE)                   // 16-11-04 SDH
      (rfscfrec1and2.emu_conv_fact * satol(enqbuf.item_price, 6)) / 1000000.0));    // 16-11-04 SDH
    memcpy(pEQR->price_emu, sbuf, sizeof(pEQR->price_emu));     // 16-11-04 SDH
    pEQR->prim_curr[0] = rfscfrec1and2.prim_curr[0];
    // 16-11-04 SDH
    //      memcpy( ((LRT_EQR *)out)->item_code,                        // v4.0
    //              ((LRT_ENQ *)(inbound))->item_code, 13 );            // v4.0
    memcpy( pEQR->item_code, enqbuf.item_code, 13 );            // v4.0

    //*(((LRT_EQR *)out)->active_deal_flag) =                       // v4.0
    //   ((satoi( enqbuf.deal_num, 4 )!=0) ? 'Y' : 'N');            // v4.0
    pEQR->active_deal_flag[0] = enqbuf.active_deal_flag[0];     // v4.0

    // Populate planner fields
    MEMCPY(pEQR->abCoreCount, enqbuf.abCoreCount);              // SDH 12-Oct-2006 Planners
    MEMCPY(pEQR->abNonCoreCount, enqbuf.abNonCoreCount);        // SDH 12-Oct-2006 Planners

    //Format log file details
    //Write to log file
    ((LRTLG_ENQ*)dtls)->enqpc[0] = 0x00;                        // 2-7-04 PAB
    if (pEQR->check_accepted[0] == 'Y') {
        ((LRTLG_ENQ*)dtls)->enqpc[0] = 0xFF;                    // 2-7-04 PAB
    }
    memset( ((LRTLG_ENQ *)dtls)->resv, 0x00, 11 );              // 2-7-04 PAB
    lrt_log( LOG_ENQ, hh_unit, dtls );                          // SDH 26-01-2005

    // IF BLIND ACCESS FROM STORENET THEN SIMULATE A CMD_OFF    // 5-7-2004 pab
    if (strncmp(((LRT_ENQ *)(inbound))->opid, "000", 3)==0) {   // 5-7-2004 PAB

        RfhistClose( CL_SESSION );                             // 5-7-2004 PAB
        IsfClose( CL_SESSION );                                // 5-7-2004 PAB
        IdfClose( CL_SESSION );                                // 5-7-2004 PAB
        IrfClose( CL_SESSION );                                // 5-7-2004 PAB
        IrfdexClose( CL_SESSION );                             // SDH 14-01-2005 Promtions
        SrpogClose(CL_SESSION);                                // SDH 12-Oct-2006 Planners
        SrmapClose(CL_SESSION);                                //CSk 12-03-2012 SFA
        SrmodClose(CL_SESSION);                                // SDH 12-Oct-2006 Planners
        SritmlClose(CL_SESSION);                               // SDH 12-Oct-2006 Planners
        SritmpClose(CL_SESSION);                               // SDH 12-Oct-2006 Planners
        SrpogifClose(CL_SESSION);                              // SDH 12-Oct-2006 Planners
        SrpogilClose(CL_SESSION);                              // SDH 12-Oct-2006 Planners
        SrpogipClose(CL_SESSION);                              // SDH 12-Oct-2006 Planners
        SrcatClose(CL_SESSION);                                // SDH 12-Oct-2006 Planners
        SrsxfClose(CL_SESSION);                                // SDH 12-Oct-2006 Planners

        // Deallocate handheld's table entry                        // 5-7-2004 PAB
        usrrc = dealloc_lrt_table( hh_unit );                   // 5-7-2004 PAB
    }                                                           // 5-7-2004 PAB

    out_lth = LRT_EQR_LTH;

}

// ------------------------------------------------------------------------------------
//
// GAS - Gap Monitor Sign On
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_GAS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_GAS;                                    // Gap Monitor Start
#define LRT_GAS_LTH sizeof(LRT_GAS)

typedef struct LRT_GAR_Txn {
   BYTE cmd[3];
   BYTE list_id[3];
   BYTE pchk_target[4];
   BYTE pchk_done[4];
} LRT_GAR;                                    // Gap signon response
#define LRT_GAR_LTH sizeof(LRT_GAR)

void GapSignOn(char* inbound) {

    URC usrrc;

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = open_pllol();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open PLLOL file. Check appl event logs");
        return;
    }
    usrrc = open_plldb();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open PLLDB file. Check appl event logs");
        return;
    }

    // v4.0 START
    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to create workfile. "             //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        return;
    }
    // v4.0 END

    // Get RF attributes from control file
    usrrc = RfscfOpen();                                            // 12-7-4 PAB
    if (usrrc != RC_OK) {                                             // 12-7-4 PAB
        prep_nak("ERRORUnable to open RFSCF file. Check appl event logs");// 12-7-4 PAB
        return;                                                      // 12-7-4 PAB
    }                                                               // 12-7-4 PAB

    RfscfRead(0L, __LINE__);
    RfscfRead(1L, __LINE__);
    RfscfClose (CL_SESSION );                                       // 12-7-4 PAB

    // Prepare GAR
    memcpy( ((LRT_GAR *)out)->cmd, "GAR", 3 );
    sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_target );            // 16-11-04 SDH
    memcpy( ((LRT_GAR *)out)->pchk_target, sbuf, 4 );               // 12-7-2004 PAB
    sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_done );              // 16-11-04 SDH
    memcpy( ((LRT_GAR *)out)->pchk_done, sbuf, 4 );                 // 12-7-2004 PAB

    // Create a new list (list_id is returned)
    usrrc = create_new_plist( (BYTE *)&(((LRT_GAR *)out)->list_id),
                              (BYTE *)&(((LRT_GAS *)(inbound))->opid) );
    if (usrrc < RC_IGNORE_ERR) {
        prep_nak("ERRORUnable to create new PLIST. Check appl event logs" );
        return;
    }
    out_lth = LRT_GAR_LTH;

}

// ------------------------------------------------------------------------------------
//
// PCM - Price Mismatch
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE boots_code[7];
   BYTE variance[6];
   BYTE type[1];                              // 26-7-2004 PAB
} LRT_PCM;                                    // Price mismatch
#define LRT_PCM_LTH sizeof(LRT_PCM)

void PriceMismatch(char* inbound) {

    time_t CurrTime;                                                // 11-09-2007 12 BMG
    LONG rc2;

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    // Read rfscf records                                           // 11-09-2007 12 BMG
    rc2 = RfscfRead(0L, __LINE__);                                  // Streamline SDH 17-Sep-2008
    rc2 = RfscfRead(1L, __LINE__);                                  // Streamline SDH 17-Sep-2008
    if (rc2 <= 0L) {                                                // 11-09-2007 12 BMG
        prep_nak("ERRORUnable to read from RFSCF. Check appl event logs");// 11-09-2007 12 BMG
        return;                                                      // 11-09-2007 12 BMG
    }                                                               // 11-09-2007 12 BMG

    // Increment and update price checks done figure on rfscf
    rfscfrec1and2.pchk_errors_c++;                                  // 16-11-04 SDH
    rfscfrec1and2.pchk_done++;                                      // 16-11-04 SDH

    rc2 = RfscfUpdate(__LINE__);                                    // 03-01-05 SDH
    if (rc2<=0L) {                                                  // 27-10-04 PAB
        prep_nak("ERRORUnable to write to RFSCF. Check appl event logs");
        return;
    }

    if (((LRT_PCM *)(inbound))->type[0] != 'L') {                       // 10.08.07 PAB Mobile Printing
        // if not printing on a local mobile printer then               // 10.08.07 PAB Mobile Printing
        // Write entry to LAB workfile to trigger price error message   // 28.06.04 PAB
        // 28.06.04 PAB
        memcpy( selbfrec.item_code, "X000000000000", 13 );              // 28.06.04 PAB
        memcpy( selbfrec.info, lrtp[hh_unit]->user, 3 );                // 28.06.04 PAB
        memcpy( selbfrec.info+3, "000", 3 );                            // 28.06.04 PAB
        memcpy( selbfrec.printerid, "0", 1 );                           // 28.06.04 PAB
        rc2 = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,         // 28.06.04 PAB
                   (void *)&selbfrec, SELBF_RECL, 0L );                 // 28.06.04 PAB

        time(&CurrTime);                                                //11-09-2007 12 BMG
        pq[lrtp[hh_unit]->pq_sub1].last_access_time = CurrTime;         //11-09-2007 12 BMG

        if (rc2<=0L) {
           sprintf(msg, "Err-W SELWF. RC:%08lX", rc2);
           disp_msg(msg);
           prep_nak("ERRORUnable to write to SELWF for PCM Record");
           return;
        }                                                              // 10.08.07 PAB Mobile Printing
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

    // Audit
    pack( ((LRTLG_PCM *)dtls)->boots_code, 4,
          ((LRT_PCM *)(inbound))->boots_code, 7, 1 );
    memset( ((LRTLG_PCM *)dtls)->resv, 0x00, 7 );                   // 26-7-2004 PAB
    memcpy( ((LRTLG_PCM *)dtls)->check_type,
            ((LRT_PCM *)(inbound))->type, 1);                       // 26-7-2004 PAB
    lrt_log( LOG_PCM, hh_unit, dtls );                              // SDH 26-01-2005 SDH

}

// ------------------------------------------------------------------------------------
//
// GAP - Gap Notification
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE item_code[13];
   BYTE boots_code[7];
   BYTE current[4];
   BYTE fill_qty[4];
   BYTE gap_flag[1];          // [Y]=Stock monitor, [N]=C&A/RI
   BYTE stock_figure[6];      // Items
   BYTE cUpdateOssrItem;      // ' '-No change, 'N'-non-OSSR, 'O'-change to OSSR    // SDH 17-11-04 OSSR WAN
   BYTE LocCounted[2];        // Multi-Site Location Counted //BMG 12-Aug-2008 Multi-Sited Counts
} LRT_GAP;                                    // Gap Monitor - PL Add/Replace
#define LRT_GAP_LTH sizeof(LRT_GAP)

void GapNotification(char* inbound) {

    LRT_GAP* pGAP = (LRT_GAP*)inbound;                          // 17-11-04 SDH OSSR WAN
    URC usrrc;

    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    // write to GAPBF workfile trigger flag
    //write_gap = satol(rfscfrec1and2.phase,1);                 // Streamline SDH 23-Sep-2008

    //NAK if creating an OSSR list in a non-OSSR store          // SDH 22-2-2005 EXCESS
    if ((pGAP->gap_flag[0] == 'O') &&                           // SDH 22-2-2005 EXCESS
        (rfscfrec1and2.ossr_store != 'W')) {                    // SDH 22-2-2005 EXCESS
        prep_nak("ERRORCannot create "                          //SDH 23-Aug-2006 Planners
                 "an OSSR list in a non-OSSR WAN store.");      // SDH 22-2-2005 EXCESS
        return;                                                  // SDH 22-2-2005 EXCESS
    }                                                           // SDH 22-2-2005 EXCESS

    // Perform the bulk of the processing                       //SDH 19-01-2005 OSSR WAN
    usrrc = process_gap( pGAP->list_id,                         // 17-11-04 SDH OSSR WAN
                         pGAP->seq,                             // 17-11-04 SDH OSSR WAN
                         pGAP->item_code,                       // 17-11-04 SDH OSSR WAN
                         pGAP->boots_code,                      // 17-11-04 SDH OSSR WAN
                         pGAP->current,                         // 17-11-04 SDH OSSR WAN
                         pGAP->fill_qty,                        // 17-11-04 SDH OSSR WAN
                         pGAP->gap_flag[0],                     // 17-11-04 SDH OSSR WAN
                         pGAP->cUpdateOssrItem,                 // 17-11-04 SDH OSSR WAN
                         pGAP->LocCounted,                      //BMG 13-Aug-2008 16
                         hh_unit );

    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORRecord not on file. Check appl event logs");
        } else {
             prep_nak("ERRORUndefined 1. Check appl event logs");//SDH 23-Aug-2006 Planners
        }
        return;
    }

    prep_ack("");                                               //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// GAX - Gap Monitor Sign Off
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_GAX_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE picks[4];
   BYTE sels[4];
   BYTE pchks[4];
   //BYTE gaps[4];
} LRT_GAX;                                    // Gap Monitor Exit
#define LRT_GAX_LTH sizeof(LRT_GAX)

void GapSignOff(char* inbound) {

    LRT_GAX* pGAX = (LRT_GAX*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LONG rc2;
//    URC usrrc;                                                  // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
    WORD wListId = satoi(pGAX->list_id, 3);                     // SDH 26-11-04 CREDIT CLAIM
    WORD wPicks = satoi(pGAX->picks, 4);                        // SDH 26-11-04 CREDIT CLAIM

    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

//    if (memcmp(lrtp[hh_unit]->Type, "M", 1)==0) {               // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        //If the device is an MC70 we need up update the        // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        //price check total on the RFSCF from the GAX PCHKS     // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        usrrc = RfscfOpen();                                    // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        if (usrrc != RC_OK) {                                   // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            prep_nak("ERRORUnable to open RFSCF file. "         // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "Please phone help desk.");               // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            return;                                             // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        }                                                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rc2 = RfscfRead(0L, __LINE__);                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        rc2 = RfscfRead(1L, __LINE__);                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rfscfrec1and2.pchk_done+=satoi(pGAX->pchks, 4);         // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rc2 = RfscfUpdate(__LINE__);                            // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        if (rc2<=0L) {                                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            prep_nak("ERRORUnable to "                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "write to RFSCF. "                        // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "Check appl event logs" );                // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            return;                                             // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        }                                                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        usrrc = RfscfClose( CL_SESSION );                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//    }                                                           // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

    close_plldb( CL_SESSION );

    // Read pllol record                                        // SDH 22-02-2005 EXCESS
    rc2 = ReadPllol(wListId, __LINE__);                         // SDH 22-02-2005 EXCESS
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read from PLLOL. "             //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }

    // Update pllol record
    if (wPicks > 0) {                                           // SDH 22-02-2005 EXCESS
        *(pllolrec.list_status) = 'U';                          // Status : Uncounted
    } else {
        *(pllolrec.list_status) = 'X';                          // Status : Cancelled
    }
    memcpy(pllolrec.item_count, pGAX->picks,                    // SDH 22-02-2005 EXCESS
           sizeof(pllolrec.item_count));                        // SDH 22-02-2005 EXCESS

    //Write the PLLOL record back                               // SDH 22-02-2005 EXCESS
    rc2 = WritePllol(wListId, __LINE__);                        // SDH 22-02-2005 EXCESS
    if (rc2<=0L) {
        prep_nak("ERRORUnable to write to PLLOL. "              //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        close_pllol( CL_SESSION );
        return;
    }

    //Close PLLOL
    close_pllol( CL_SESSION );

    //Process both just in case                                     // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

    //Update location: assume handheld is in shop as that's where   //SDH 19-01-2005 OSSR WAN
    //Shelf Monitor and fast fill is done                           //SDH 19-01-2005 OSSR WAN
    lrtp[hh_unit]->bLocation = 'S';                                 //SDH 19-01-2005 OSSR WAN

    // Audit
    ((LRTLG_GAX *)dtls)->gaps_identified = wPicks;              //SDH 26-11-04 CREDIT CLAIM
    memset( ((LRTLG_GAX *)dtls)->resv, 0x00, 8 );
    lrt_log( LOG_GAX, hh_unit, dtls );                          //SDH 19-01-2005 OSSR WAN

}

// ------------------------------------------------------------------------------------
//
// PLO - Picking List Sign On
//
//
//
// ------------------------------------------------------------------------------------

/*
typedef struct LRT_PLO_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_PLO;                                                          // Picking list signon
#define LRT_PLO_LTH sizeof(LRT_PLO)
*/

void PListSignOn(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = open_pllol();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open PLLOL file. "                 //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }
    usrrc = open_plldb();
    if (usrrc<=RC_DATA_ERR) {
        close_pllol(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        prep_nak("ERRORUnable to open PLLDB file. "                 //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }

    usrrc = open_minls();
    if (usrrc<=RC_DATA_ERR) {
        close_pllol(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        close_plldb(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        prep_nak("ERRORUnable to open MINLS file. "                 //SDH 23-Aug-2006 Planners
                  "Check appl event logs" );
        return;
    }

    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to create workfile. "             //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        close_pllol(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        close_plldb(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        close_minls(CL_SESSION);                                    // SDH 14-03-2005 EXCESS
        return;
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// PRT - Print Shelf Edge Label
//
//
//
// ------------------------------------------------------------------------------------

void PrintSEL(char *inbound) {                                  // BMG 1.6 13-08-2008

    //Static (permanent) data
    static BYTE abLastItemCode[13] = "";

    //Transient data
    ENQUIRY enqbuf;                                             // Streamline SDH 17-Sep-2008
    LRT_PRT* pPRT = (LRT_PRT*)inbound;                          // SDH 12-Oct-2006
    time_t CurrTime;                                            //11-09-2007 12 BMG
    LONG sec, day, month, year;                                 // BMG 1.6 13-08-2008
    WORD hour, min;                                             // BMG 1.6 13-08-2008
    BYTE authority[1], username[32];                            // BMG 1.6 13-08-2008
    BYTE cStockAccess;                                          //CSk 12-03-2012 SFA
    URC usrrc = RC_OK;                                          // BMG 1.6 13-08-2008
    WORD rc = 0;                                                // BMG 1.6 13-08-2008

    memset( (BYTE *)&prtctlrec, 0x00, sizeof(PRTCTL_REC) );     // BMG 1.6 13-08-2008

    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM // BMG 1.6 13-08-2008
    // If local type print the the PPC needs extra info.        // 07-08-07 PAB Mobile Printing
    if (strncmp(pPRT->type,"L",1)== 0) {                        // 07-08-07 PAB Mobile Printing
        usrrc = BuildLPR(inbound);                              // 07-08-07 PAB Mobile Printing
        if (usrrc != RC_OK) {                                   // Streamline SDH 24-Sep-2008
            prep_nak("ERRORItem not on file. "                  // Streamline SDH 24-Sep-2008
                     "Check appl event logs" );                 // 08-08-07 PAB
            return;                                             // 08-08-07 PAB // BMG 1.6 13-08-2008
        }
        return;                                                 // 07-08-07 PAB Mobile Printing // BMG 1.6 13-08-2008
    }

    // if barcode is zeros then get update on printer status    // 19-02-04 PAB
    // new functionality Phase 2 Proximity printing             // 19-02-04 PAB
    if (strncmp(pPRT->item_code, "0000000000000", 13 ) == 0) {  // 19-02-04 PAB
        // Get SEL printer status                               // 19-02-04 PAB
        usrrc = PrtctlOpen();                                   // Streamline SDH 17-Sep-2008
        if (usrrc<=RC_DATA_ERR) {                               // 19-02-04 PAB
            prep_nak("ERRORUnable to open PRTCTL file. "        // SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 19-02-04 PAB
            return;                                             // 19-02-04 PAB // BMG 1.6 13-08-2008
        }                                                       // 19-02-04 PAB
        // read the current status record & close the file      // 19-02-04 PAB
        rc = PrtctlRead(0L, __LINE__);                          // Streamline SDH 17-Sep-2008
        PrtctlClose(CL_SESSION);                                // Streamline SDH 17-Sep-2008
        // Prep SNR tx and Break;                               // 19-02-04 PAB
        // Authorise user                                       // 19-02-04 PAB
        // 19-02-04 PAB
        AfOpen();                                               // 19-02-04 PAB
        if (usrrc<=RC_DATA_ERR) {                               // 19-02-04 PAB
            prep_nak("ERRORUnable to open EALAUTH file. "       //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 19-02-04 PAB
            return;                                             // 19-02-04 PAB // BMG 1.6 13-08-2008
        }                                                       // 19-02-04 PAB
        usrrc = authorise_user( ((LRT_SOR *)(inbound))->opid,   // 19-02-04 PAB
                                ((LRT_SOR *)(inbound))->pass,   // 19-02-04 PAB
                                (BYTE *)&authority,             // 19-02-04 PAB
                                (BYTE *)&username,              // 19-02-04 PAB
                                (BYTE *)&cStockAccess);         //CSk 12-03-2012 SFA
        AfClose( CL_SESSION );                                  // 19-02-04 PAB
        if (usrrc<RC_IGNORE_ERR) {                              // 19-02-04 PAB
            prep_nak("ERRORauthorise_user failed"               //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 19-02-04 PAB
            return;                                             // 19-02-04 PAB // BMG 1.6 13-08-2008
        }                                                       // 19-02-04 PAB
        // 19-02-04 PAB
        // Save user ID                                         // 19-02-04 PAB
        MEMCPY(lrtp[hh_unit]->user, ((LRT_SOR *)(inbound))->opid);// 19-02-04 PAB
        // Get time                                             // 19-02-04 PAB
        sysdate( &day, &month, &year, &hour, &min, &sec );      // 19-02-04 PAB

        // Prepare SNR                                          // 19-02-04 PAB
        buildSNR((LRT_SNR *)&out, prtctlrec.prtnum,             // 16-11-2004 SDH
                 ((LRT_SOR *)(inbound))->opid, authority[0],    // 16-11-2004 SDH
                 username, rfscfrec1and2.ossr_store,            // SDH 20-May-2009 Model Day
                 cStockAccess);                                 //CSk 12-03-2012 SFA

        out_lth = LRT_SNR_LTH;                                  // 19-02-04 PAB
        //authorised = TRUE;                                    // 19-02-04 PAB
        ((LRTLG_SOR *)dtls)->authority[0] = 'Y';                // 17-11-04 SDH
        memset( ((LRTLG_SOR *)dtls)->resv, 0x00, 11 );          // 19-02-04 PAB
                                                                // 19-02-04 PAB
        return;                                                 // 19-02-04 PAB // BMG 1.6 13-08-2008
    }                                                           // 19-02-04 PAB

    if (IsStoreClosed()) return;                                // SDH 26-11-04 CREDIT CLAIM // BMG 1.6 13-08-2008

    //Check that item is on file, if it wasn't the last item    // SDH 26-11-04 CREDIT CLAIM
    //checked                                                   // SDH 26-11-04 CREDIT CLAIM
    if (strncmp(pPRT->item_code,                                // SDH 26-11-04 CREDIT CLAIM
                abLastItemCode, sizeof(abLastItemCode)) != 0) { // SDH 26-11-04 CREDIT CLAIM
        usrrc = stock_enquiry(SENQ_BOOTS, pPRT->item_code,      // SDH 26-11-04 CREDIT CLAIM
                              &enqbuf);                         // SDH 26-11-04 CREDIT CLAIM
        if (usrrc != RC_OK) {                                   // SDH 26-11-04 CREDIT CLAIM
            prep_nak("Item is not on file.");                   // SDH 23-Aug-2006 Planners
            return;                                             // SDH 26-11-04 CREDIT CLAIM // BMG 1.6 13-08-2008
        }                                                       // SDH 26-11-04 CREDIT CLAIM
    }                                                           // SDH 26-11-04 CREDIT CLAIM

    //Save away last item                                       // SDH 26-11-04 CREDIT CLAIM
    MEMCPY(abLastItemCode, pPRT->item_code);                    // SDH 26-11-04 CREDIT CLAIM

    // Write entry to LAB workfile (#1)
    MEMCPY(selbfrec.item_code, pPRT->item_code);
    MEMCPY(selbfrec.info, lrtp[hh_unit]->user);
    MEMCPY(selbfrec.info + 3, "000");
    selbfrec.printerid[0] = pPRT->type[0];                        // 18-02-04 PAB
    LONG rc2 = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,
                   (void *)&selbfrec, SELBF_RECL, 0L );

    //sprintf(msg, "RC= :, %d",rc2);
    //disp_msg(msg);

    time(&CurrTime);                                            //11-09-2007 12 BMG
    pq[lrtp[hh_unit]->pq_sub1].last_access_time = CurrTime;     //11-09-2007 12 BMG

    if (rc2 >= 0) {                                             //PAB 28-NOv-2006
        prep_ack("");                                           //SDH 23-Aug-2006 Planners
    } else {
        rc2 = prepare_workfile( hh_unit, SYS_LAB );
        // Write entry to LAB workfile (#1)
        MEMCPY(selbfrec.item_code, pPRT->item_code);
        MEMCPY(selbfrec.info, lrtp[hh_unit]->user);
        MEMCPY(selbfrec.info + 3, "000");
        selbfrec.printerid[0] = pPRT->type[0];                  // 18-02-04 PAB
        rc2 = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum1,
                       (void *)&selbfrec, SELBF_RECL, 0L );
        pPRT->type[0] = '0';                                    // 24-02-04 PAB
        if (rc2>=0) {                                           //PAB 28-Nov-2006
            prep_ack("");                                       //SDH 23-Aug-2006 Planners
        } else {
            log_event101(rc, SELBF_REP, __LINE__);
            prep_nak("ERRORUnable to write to temp SELBF. "     //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            return;                                             // BMG 1.6 13-08-2008
        }
    }

    // Audit
    memcpy(((LRTLG_PRT *)dtls)->resv, ((LRT_PRT *)(inbound))->item_code, 12 );    // 25-8-2004 PAB
    lrt_log( LOG_PRT, hh_unit, dtls );

}

// ------------------------------------------------------------------------------------
//
// PLR - Picking Lists - Get List of Lists
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE auth[1];
} LRT_PLR;                                                           // Picking list request
#define LRT_PLR_LTH sizeof(LRT_PLR)

typedef struct LRT_PLE_Txn {
   BYTE cmd[3];
   BYTE list_id[3];
} LRT_PLE;                                    // Picking list end
#define LRT_PLE_LTH sizeof(LRT_PLE)

void PListGetListOfLists(char* inbound) {

    URC usrrc;

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    // Get next record from PLLOL
    usrrc = pllol_get_next( /*hh_unit,*/
                            (BYTE *)((LRT_PLR *)(inbound))->list_id,
                            (LRT_PLL *)out );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORI/O error. Check appl event logs" );     //SDH 23-Aug-2006 Planners
        } else {
            prep_nak("ERRORUndefined 6. Check appl event logs" );   //SDH 23-Aug-2006 Planners
        }
        return;
    }
    if (usrrc==1) {
        // Record available - Prepare PLL
        memcpy( ((LRT_PLL *)out)->cmd, "PLL", 3 );
        out_lth = LRT_PLL_LTH;
    } else {
        // Record not available - Prepare PLE
        memcpy( ((LRT_PLE *)out)->cmd, "PLE", 3 );
        memcpy( ((LRT_PLE *)out)->list_id,
                ((LRT_PLR *)(inbound))->list_id, 3);                // v2.3
        out_lth = LRT_PLE_LTH;
    }

}

// ------------------------------------------------------------------------------------
//
// PLS - Picking Lists - Get List
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_PLS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE seq[3];
} LRT_PLS;                                                           // Picking list select
#define LRT_PLS_LTH sizeof(LRT_PLS)

void PListGetList(char* inbound) {

    //Setup specific view of input                              // SDH 26-11-04 CREDIT CLAIM
    LRT_PLS* pPLS = (LRT_PLS*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LONG list_id = satol(pPLS->list_id, 3);                     // SDH 26-11-04 CREDIT CLAIM
    LONG day, month, year, sec;                                 // Streamline SDH 17-Sep-2008
    WORD hour, min;                                             // Streamline SDH 17-Sep-2008
    URC usrrc;
    LONG rc2;
    BOOLEAN fNakPrepared = FALSE;                               // SDH 20-May-2009 Model Day

    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                // SDH 26-11-04 CREDIT CLAIM

    pack((BYTE *)&(lrtp[hh_unit]->unique), 2, pPLS->seq, 3, 1); // SDH 22-02-2005 EXCESS

    // Update PLLOL status and picker details for 1st item in list
    if (strncmp(pPLS->seq, "001", 3) == 0) {                    // SDH 26-11-04 CREDIT CLAIM

        // Read pllol record LOCKED                             // SDH 20-May-2009 Model Day
        rc2 = ReadPllolLock(list_id, __LINE__);                 // SDH 20-May-2009 Model Day
        if (rc2 <= 0L) {
            prep_nak("ERRORUnable to read from PLLOL. "         //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            return;
        }

        //PLLOL Record is now locked, so don't quit out without // SDH 20-May-2009 Model Day
        //unlocking it first.                                   // SDH 20-May-2009 Model Day

        if (pllolrec.list_status[0] == 'P') {                   // SDH 10-1-2005 OSSR WAN
            prep_nak("This list has already been picked and is "// SDH 23-Aug-2006 Planners
                      "no longer available." );                 // 27-10-04 PAB
            fNakPrepared = TRUE;                                // SDH 20-May-2009 Model Day

        //Prevent anyone except the person who was last in the  // SDH 10-1-2005 OSSR WAN
        //list from accessing it.                               // SDH 10-1-2005 OSSR WAN
        } else if (pllolrec.list_status[0] == 'A') {            // SDH 10-1-2005 OSSR WAN
            WORD wLastPicker = satoi(pllolrec.picker, 3);       // SDH 10-1-2005 OSSR WAN
            WORD wNewPicker = satoi(pPLS->opid, 3);             // SDH 10-1-2005 OSSR WAN
            if ((wLastPicker != wNewPicker) &&                  // SDH 10-1-2005 OSSR WAN
                (wLastPicker != 0)) {                           // SDH 10-1-2005 OSSR WAN
                sprintf(sbuf, "List already active.\nOnly user ID "    // SDH 10-1-2005 OSSR WAN
                        "%.3d can open this list.",             // SDH 10-1-2005 OSSR WAN
                        wLastPicker);                           // SDH 10-1-2005 OSSR WAN
                prep_nak(sbuf);                                 // SDH 23-Aug-2006 Planners
                fNakPrepared = TRUE;                            // SDH 20-May-2009 Model Day
            }                                                   // SDH 10-1-2005 OSSR WAN
        }                                                       // SDH 10-1-2005 OSSR WAN

        if (!fNakPrepared) {                                    // SDH 20-May-2009 Model Day

            //Update handheld's location                        // SDH 10-1-2005 OSSR WAN
            if ((pllolrec.cLocation == 'O') ||                  // SDH 22-02-2005 EXCESS
                (pllolrec.cLocation == 'C')) {                  // SDH 22-02-2005 EXCESS
                lrtp[hh_unit]->bLocation = 'O';                 // SDH 22-02-2005 EXCESS
            } else {                                            // SDH 22-02-2005 EXCESS
                lrtp[hh_unit]->bLocation = 'S';                 // SDH 22-02-2005 EXCESS
            }                                                   // SDH 22-02-2005 EXCESS

            // Update pllol record
            pllolrec.list_status[0] = 'A';                      // Status : Active
            memcpy(pllolrec.picker, (((LRT_PLS *)(inbound))->opid), 3);
            sysdate(&day, &month, &year, &hour, &min, &sec);
            sprintf( sbuf, "%02d%02d", hour, min);
            memcpy( pllolrec.pick_start_time, sbuf, 4);

        }                                                       // SDH 20-May-2009 Model Day

        if (WritePllolUnlock(list_id, __LINE__) <= 0L) {        // SDH 20-May-2009 Model Day
            prep_nak("ERRORUnable to write to PLLOL. "          // SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            fNakPrepared = TRUE;                                // SDH 20-May-2009 Model Day
        }

        if (fNakPrepared) return;                               // SDH 20-May-2009 Model Day

    }

    //Create SEL workfile                                   // SDH 08-05-2006 Bug fix
    prepare_workfile(hh_unit, SYS_LAB);                     // SDH 08-05-2006 Bug fix
    prepare_workfile(hh_unit, SYS_GAP);                     // SDH 30-07-2006 Bug fix

    // Get next record from PLLDB
    usrrc = plldb_get_next(pPLS->list_id, pPLS->seq, (LRT_PLI*)out); //SDH 20-May-2009 Model Day

    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to read from PLLDB. "         //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
        } else {
            prep_nak("ERRORUndefined 7. "                       //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
        }
        return;
    }


    if (usrrc == 1) {
        // Record available - Prepare PLI
        memcpy(((LRT_PLI*)out)->cmd, "PLI", 3);
        out_lth = LRT_PLI_LTH;
    } else {
        if (strncmp(pPLS->seq, "001", 3) == 0) {
            prep_nak("This list has already been picked and is "//SDH 23-Aug-2006 Planners
                      "no longer available." );
        } else {
            // Record not available - Prepare PLE
            memcpy(((LRT_PLE*)out)->cmd, "PLE", 3);
            out_lth = LRT_PLE_LTH;
        }
    }

}

// ------------------------------------------------------------------------------------
//
// PLC - Picking Lists - Update Item
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_PLC_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE boots_code[7];
   BYTE count[4];
   BYTE gap_flag[1];
   BYTE cPickLocation;                                               // SDH 11-1-04 OSSR WAN
   BYTE abOssrCount[4];                                              // SDH 11-1-04 OSSR WAN
   BYTE cUpdateOssrItem;                                             // SDH 11-1-04 OSSR WAN
   BYTE abLocCounted[2];   // Multi-Site Location Counted                       //BMG 12-Aug-2008 Multi-Sited Counts
   BYTE abMSPicked[1];     // Y = All Multi-site locations picked, ' ' = N/A    //BMG 12-Aug-2008 Multi-Sited Counts
} LRT_PLC;                                    // Picking list item update
#define LRT_PLC_LTH sizeof(LRT_PLC)

typedef struct {                                                    // SDH 20-May-2009 Model Day
    BYTE abListIDPkd[2];                                            // SDH 20-May-2009 Model Day
    BYTE abListSeqPkd[2];                                           // SDH 20-May-2009 Model Day
    BYTE abItemCodePkd[3];                                          // SDH 20-May-2009 Model Day
    BYTE abCountPkd[2];                                             // SDH 20-May-2009 Model Day
    BYTE bListType;                                                 // SDH 20-May-2009 Model Day
    BYTE abFiller[2];                                               // SDH 20-May-2009 Model Day
} LRTLG_PLC;                                                        // SDH 20-May-2009 Model Day

void PListUpdateItem(char* inbound) {

    BYTE gapbfrec[32];
    BYTE stkmqrec[128];

    ENQUIRY enqbuf;                                             // Streamline SDH 17-Sep-2008
    B_DATE nowDate;                                             // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                             // SDH 26-11-04 CREDIT CLAIM
    DOUBLE dTodayJul;                                           // SDH 26-11-04 CREDIT CLAIM
    DOUBLE dLastDeliveryJul;                                    // SDH 26-11-04 CREDIT CLAIM
    DOUBLE dMinlsRecountJul;                                    // SDH 26-11-04 CREDIT CLAIM
    LRT_PLC* pPLC = (LRT_PLC*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LRTLG_PLC* pLGPLC = (LRTLG_PLC*)dtls;                       // SDH 20-May-2009 Model Day
    BOOLEAN fDecrementItemCount = TRUE;                         // SDH 26-11-04 CREDIT CLAIM
    WORD wListId = satoi(pPLC->list_id, sizeof(pPLC->list_id)); // SDH 26-11-04 CREDIT CLAIM
    WORD wDeliveryAdj = 0;                                      // SDH 26-11-04 CREDIT CLAIM
    WORD wOssrCount;                                            // SDH 22-02-05 EXCESS
    WORD wShelfCount;                                           // SDH 22-02-05 EXCESS
    WORD wBackCount;                                            // SDH 22-02-05 EXCESS
    LONG lStockFigure;                                          // SDH 22-02-05 EXCESS
    BOOLEAN fExcessList;                                        // SDH 07-07-05 EXCESS
    WORD wMSLoc = 0;                                            // BMG 13-Aug-2008 16
    LONG day, month, year, sec;                                 // Streamline SDH 17-Sep-2008
    LONG rc2;
    LONG mismatch_qty, mismatch_val, variance;
    LONG items_sold_today, sales_figure;
    WORD hour, min;                                             // Streamline SDH 17-Sep-2008
    LONG new_count;
    URC usrrc;
    BOOLEAN count_pending;
    BOOLEAN count_accepted = FALSE;                             // 15-9-04 PAB
    BYTE item_code[13];

    //Get today's date as a Julian                              // SDH 22-02-05 EXCESS
    GetSystemDate(&nowTime, &nowDate);                          // SDH 22-02-05 EXCESS
    dTodayJul = ConvGJ(nowDate.wDay, nowDate.wMonth,            // SDH 22-02-05 EXCESS
                       nowDate.wYear);                          // SDH 22-02-05 EXCESS

    //Initial checks                                            // SDH 26-11-04 CREDIT CLAIM
    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //NAK a location of OSSR for non-OSSR stores                // SDH 22-02-05 EXCESS
    if ((pPLC->cPickLocation == 'O') &&                         // SDH 22-02-05 EXCESS
        (rfscfrec1and2.ossr_store != 'W')) {                    // SDH 22-02-05 EXCESS
        prep_nak("ERRORCannot count an OSSR "                   //SDH 23-Aug-2006 Planners
                  "location in a non-OSSR WAN store. ");        // SDH 22-02-05 EXCESS
        return;                                                 // SDH 22-02-05 EXCESS
    }                                                           // SDH 22-02-05 EXCESS

    //Set up PLLDB key                                          // SDH 26-11-04 CREDIT CLAIM
    memcpy( plldbrec.list_id, pPLC->list_id, 3 );               // SDH 26-11-04 CREDIT CLAIM
    memcpy( plldbrec.seq, pPLC->seq, 3 );                       // SDH 26-11-04 CREDIT CLAIM

    // move the stock enquiry up in the transaction so          // 2-9-2004 PAB OSSR
    // enable the sales figure at time of backshop count        // 2-9-2004 PAB OSSR
    // to be stored on the plldb record. - can then be used     // 2-9-2004 PAB OSSR
    // for future reporting and full OSSR solution.             // 2-9-2004 PAB OSSR
    // Do stock enquiry for counted item.                       // 2-9-2004 PAB OSSR
    // Made read independent of whether gap flag set to ensure  // 2-9-2004 PAB OSSR
    // we have the item details regardless                      // 2-9-2004 PAB OSSR
    memset( item_code, '0', 13 );                               // 2-9-2004 PAB OSSR
    memcpy( item_code+6, pPLC->boots_code, 7 );                 // 2-9-2004 PAB OSSR
    usrrc = stock_enquiry( SENQ_TSF, item_code, &enqbuf );      // 2-9-2004 PAB OSSR
    if (usrrc != RC_OK) {                                       // 2-9-2004 PAB OSSR
        log_event101(usrrc, 0, __LINE__);                       // 2-9-2004 PAB OSSR
        if (debug) {                                            // 2-9-2004 PAB OSSR
            sprintf(msg, "Error - stock_enquiry(). RC:%d",      // 2-9-2004 PAB OSSR
                    usrrc);                                     // 2-9-2004 PAB OSSR
            disp_msg(msg);                                      // 2-9-2004 PAB OSSR
        }                                                       // 2-9-2004 PAB OSSR
        prep_nak("ERRORStock enquiry failed. "                  // SDH 23-Aug-2006 Planners
                 "Check appl event logs");                      // 2-9-2004 PAB OSSR
        return;                                                 // 2-9-2004 PAB OSSR
    }                                                           // 2-9-2004 PAB OSSR

    //Reformat certain fields from the stock enquiry            // SDH 22-02-05 EXCESS
    lStockFigure = satol(enqbuf.stock_figure, 6);               // SDH 22-02-05 EXCESS

    // if list id is zero then this is a Theo Stock Figure change// SDH 26-11-04 UPWARDS TSF
    if (wListId == 0) {                                         // SDH 26-11-04 UPWARDS TSF

        // compute date of last delivery as Julian date         // 21-10-04 PAB
        unpack( sbuf, 6, enqbuf.date_last_delivery, 3, 0 );     // 21-10-04 PAB
        day   = satol( sbuf+4, 2 );                             // 21-10-04 PAB
        month = satol( sbuf+2, 2 );                             // 21-10-04 PAB
        year  = satol( sbuf,   2 )+2000L;                       // 21-10-04 PAB
        dLastDeliveryJul = ConvGJ(day, month, year);            // 21-10-04 PAB
        if (debug) {                                            // 21-10-04 PAB
            sprintf(msg, "Item had a delivery on %s "           // 21-10-04 PAB
                    "%02ld/%02ld/%04ld %16ld",                  // 21-10-04 PAB
                    dayname[ConvDOW(dLastDeliveryJul)], day, month,    // 21-10-04 PAB
                    year, dLastDeliveryJul);                    // 21-10-04 PAB
            disp_msg(msg);                                      // 21-10-04 PAB
        }                                                       // 21-10-04 PAB

        // if date of last delivery is less/equal INVOK.RP.days then// 21-10-04 PAB
        //    nak the update with a message                         // 21-10-04 PAB
        //    and break                                             // 21-10-04 PAB
        // if today is Thur,Fri,Sat,Sun at 2 to rpdays to allow for // 21-10-04 PAB
        //    for 5 day delivery week                               // 21-10-04 PAB
//        if (nowDate.wDOW >= 1 && nowDate.wDOW <= 3) {           //Mon to Wed  // SDH 22-02-05 EXCESS  // 07-04-2009 BMG
//            wDeliveryAdj = 2;                                   // 21-10-04 PAB                       // 07-04-2009 BMG
//            if (debug) {                                        // 21-10-04 PAB                       // 07-04-2009 BMG
//                sprintf(msg, "Adjust Delivery days for 5 day cycle");    //21-10-04 PAB               // 07-04-2009 BMG
//                disp_msg(msg);                                  // 21-10-04 PAB                       // 07-04-2009 BMG
//            }                                                   // 21-10-04 PAB                       // 07-04-2009 BMG
//        }                                                       // 21-10-04 PAB                       // 07-04-2009 BMG

        //Open, read, close INVOK                               // 21-10-04 PAB
        InvokOpen();                                            // 21-10-04 PAB
        rc2 = InvokRead(0L, __LINE__);
        InvokClose(CL_SESSION);                                 // 10-03-05 SDH EXCESS
        if (rc2 <= 0L) {                                        // 21-10-04 PAB
            prep_nak("ERRORUnable to READ the INVOK. "          // SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // 21-10-04 PAB
            return;                                             // 21-10-04 PAB
        }                                                       // 21-10-04 PAB

        //Extract the current stock figure and the new stock figure
        wBackCount = satoi(pPLC->count, 4);                     // SDH 22-02-05 EXCESS

        //Alway allow TSF adjustments for +ve stock adj         // SDH 03-02-2005 UPWARDS TSF
        if (wBackCount < lStockFigure) {                        // SDH 12-09-2005 UPWARDS TSF
            //Reject deliv dt if stk might not have arrived yet // SDH 03-02-2005 UPWARDS TSF
            LONG daysSinceDelivery = dTodayJul - dLastDeliveryJul;    // SDH 03-02-2005 UPWARDS TSF
            LONG deliveryLeadDays = satol(invokrec.rp_days,1) + // SDH 03-02-2005 UPWARDS TSF
                                    wDeliveryAdj;               // SDH 03-02-2005 UPWARDS TSF
            if (daysSinceDelivery < deliveryLeadDays) {         // SDH 03-02-2005 UPWARDS TSF
                count_accepted = FALSE;                         // 21-10-04 PAB
                if (debug) {                                    // 21-10-04 PAB
                    sprintf(msg, "TSF Rejected delivery date "  // 21-10-04 PAB
                            "%08ld/%08ld %8ld",                 // 21-10-04 PAB
                            dTodayJul, dLastDeliveryJul,        // 21-10-04 PAB
                            invokrec.rp_days );                 // 21-10-04 PAB
                    disp_msg(msg);                              // 21-10-04 PAB
                }                                               // 21-10-04 PAB
                sprintf( msg,                                   // 21-10-04 PAB
                         "Count Rejected. Item had a delivery " // 21-10-04 PAB
                         "on %s %02ld/%02ld/%04ld",             // 21-10-04 PAB
                         dayname[ConvDOW(dLastDeliveryJul)],    // 10-01-05 SDH OSSR WAN
                         day, month, year );                    // 10-01-05 SDH OSSR WAN
                prep_nak(msg);                                  // SDH 23-Aug-2006 Planners
                return;                                         // 21-10-04 PAB
            }                                                   // 21-10-04 PAB
        }                                                       // SDH 03-02-2005 UPWARDS TSF

    }                                                           // 21-10-04 PAB

    //If NOT a Theo Stock Figure update                         // SDH 03-02-2005 UPWARDS TSF
    if (wListId != 0) {                                         // SDH 03-02-2005 UPWARDS TSF

        //Auto-set the OSSR item flag if stock has been counted // SDH 09-03-05 OSSR WAN
        //in the OSSR                                           // SDH 09-03-05 OSSR WAN
        if (pPLC->cPickLocation == 'O') {                       // SDH 09-03-05 OSSR WAN
            WORD wOSCnt = satoi(pPLC->abOssrCount,              // SDH 09-03-05 OSSR WAN
                                sizeof(pPLC->abOssrCount));     // SDH 09-03-05 OSSR WAN
            if (wOSCnt > 0) pPLC->cUpdateOssrItem = 'O';        // SDH 09-03-05 OSSR WAN
        }                                                       // SDH 09-03-05 OSSR WAN

        //Attempt to read the PLLOL                             // SDH 22-02-05 EXCESS
        //We need to know what the current list location flag   // SDH 22-02-05 EXCESS
        //is in order to determine whether to flag the item     // SDH 22-02-05 EXCESS
        //as picked.                                            // SDH 22-02-05 EXCESS
        rc2 = ReadPllolLock(wListId, __LINE__);                 // SDH 20-May-2009 Model Day
        if (rc2 <= 0L) {                                        // SDH 22-02-05 EXCESS
            prep_nak("ERRORUnable to read from PLLOL. "         // SDH 23-Aug-2006 Planners
                      "Check appl event logs");                 // SDH 22-02-05 EXCESS
            return;                                             // SDH 22-02-05 EXCESS
        }                                                       // SDH 22-02-05 EXCESS

        // Update the picker ID for POD stores only                     //ACS PRB0044559
        if (memcmp(lrtp[hh_unit]->Type, "M",1) == 0) {                  //ACS PRB0044559
            memcpy(pllolrec.picker, (((LRT_PLC *)(inbound))->opid), 3); //ACS PRB0044559
        }                                                               //ACS PRB0044559

        //Also NAK cancelled lists.  MC70s may have these loaded// SDH 20-May-2009 Model Day
        //if housekeeping has just run.                         // SDH 20-May-2009 Model Day
        if (pllolrec.list_status[0] == 'X') {                   // SDH 20-May-2009 Model Day
            prep_nak("List has been cancelled.");               // SDH 20-May-2009 Model Day
            WritePllolUnlock(wListId, __LINE__);                // SDH 20-May-2009 Model Day
            return;                                             // SDH 20-May-2009 Model Day
        }                                                       // SDH 20-May-2009 Model Day

        //Set flag if Excess Stock list location                // SDH 07-07-05 EXCESS
        if (pllolrec.cLocation == 'E' ||                        // SDH 07-07-05 EXCESS
            pllolrec.cLocation == 'C' ||                        // SDH 07-07-05 EXCESS
            pllolrec.cLocation == 'D' ||                        // SDH 07-07-05 EXCESS
            pllolrec.cLocation == 'B') {                        // SDH 07-07-05 EXCESS
            fExcessList = TRUE;                                 // SDH 07-07-05 EXCESS
        } else {                                                // SDH 07-07-05 EXCESS
            fExcessList = FALSE;                                // SDH 07-07-05 EXCESS
        }                                                       // SDH 07-07-05 EXCESS

        //Picking lists counts are never NAKed                  // 15-9-04 PAB
        count_accepted = TRUE;                                  // 15-9-04 PAB

        //Attempt to read the PLLDB locked                      // SDH 22-02-05 EXCESS
        rc2 = ReadPlldbLock(__LINE__);                          // SDH 22-02-05 EXCESS
        if (rc2 <= 0L) {                                        // SDH 22-02-05 EXCESS
            prep_nak("ERRORUnable to read (lock) PLLDB. "       // SDH 23-Aug-2006 Planners
                     "Check appl event logs" );                 // SDH 22-02-05 EXCESS
            WritePllolUnlock(wListId, __LINE__);                // SDH 20-May-2009 Model Day
            return;                                             // SDH 22-02-05 EXCESS
        }                                                       // SDH 22-02-05 EXCESS

        if (debug) {                                            // PAB 31-10-03
            disp_msg( "PLLDB item status" );                    // PAB 31-10-03
            dump ( plldbrec.item_status, 1);                    // PAB 31-10-03
        }                                                       // PAB 31-10-03
        if (plldbrec.item_status[0] == 'P') {                   // PAB 31-10-03
            fDecrementItemCount = FALSE;                        // PAB 31-10-03
            disp_msg("Already picked");                         // PAB 31-10-03
        }                                                       // PAB 31-10-03

        //Read, update or create RFHIST record                  // SDH 17-11-04 OSSR WAN
        //if the OSSR item status needs updating                // SDH 17-11-04 OSSR WAN
        //We only need the RFHIST record if OSSR WAN is active  // SDH 17-11-04 OSSR WAN
        //to compare the picking location with the 'home'       // SDH 17-11-04 OSSR WAN
        //location of the item                                  // SDH 17-11-04 OSSR WAN
        if ((rfscfrec1and2.ossr_store == 'W') &&                // SDH 17-11-04 OSSR WAN
            (pPLC->cUpdateOssrItem != ' ')) {                   // SDH 17-11-04 OSSR WAN
            usrrc = ProcessRfhist(plldbrec.boots_code,          // SDH 17-11-04 OSSR WAN
                                  pPLC->cUpdateOssrItem,        // SDH 17-11-04 OSSR WAN
                                  __LINE__);                    // SDH 17-11-04 OSSR WAN
            if (usrrc > RC_OK) {                                // SDH 17-11-04 OSSR WAN
                prep_nak("ERRORUnable to process RFHIST. "      // SDH 23-Aug-2006 Planners
                          "Check appl event logs" );            // SDH 17-11-04 OSSR WAN
            }                                                   // SDH 17-11-04 OSSR WAN
        }                                                       // SDH 17-11-04 OSSR WAN

        //Get the current date/time for writing to PLLDB        // SDH 22-02-05 EXCESS
        sprintf(sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin); // SDH 22-02-05 EXCESS

        //Update appropriate location fields                    // SDH 22-02-05 EXCESS
        if (pPLC->cPickLocation == 'S') {                       // SDH 22-02-05 EXCESS
            //If not Multi-Sited update base Shelf Count values // BMG 29-Oct-2008 17
            if (memcmp(pPLC->abLocCounted, "  ", 2) == 0) {     // BMG 29-Oct-2008 17
                lrtp[hh_unit]->bLocation = 'S';                 // SDH 22-02-05 EXCESS
                memcpy(plldbrec.qty_on_shelf, pPLC->count,      // SDH 22-02-05 EXCESS
                       sizeof(plldbrec.qty_on_shelf));          // SDH 22-02-05 EXCESS
                pack(plldbrec.time_sf_count, 2, sbuf, 4, 0);    // SDH 22-02-05 EXCESS
                memcpy(plldbrec.sales_figure,                   // SDH 22-02-05 EXCESS
                       enqbuf.items_sold_today+2,               // SDH 22-02-05 EXCESS
                       sizeof(plldbrec.sales_figure));          // SDH 22-02-05 EXCESS
            // Multi-site location so update location values                                 // BMG 29-Oct-2008 17
            } else {                                                                         // BMG 29-Oct-2008 17
                wMSLoc = satoi(pPLC->abLocCounted,2); //00 is first location//               // BMG 29-Oct-2008 17
                if (wMSLoc > 32) wMSLoc = 32; //Force to 'other' location if > 33rd location // BMG 29-Oct-2008 17 //CSk 12-03-2012 SFA
                pack(plldbrec.aMS_details[wMSLoc].ms_loc_count, 2, pPLC->count, 4, 0);       // BMG 29-Oct-2008 17
                pack(plldbrec.aMS_details[wMSLoc].ms_time_of_count, 2, sbuf, 4, 0);          // BMG 29-Oct-2008 17
                pack(plldbrec.aMS_details[wMSLoc].ms_sales_fig, 2,                           // BMG 29-Oct-2008 17
                       enqbuf.items_sold_today+2, 4, 0);                                     // BMG 29-Oct-2008 17
                //Also update sales figure and time on the standard shopfloor fields         // BMG 29-Oct-2008 17
                pack(plldbrec.time_sf_count, 2, sbuf, 4, 0);                                 // BMG 29-Oct-2008 17
                memcpy(plldbrec.sales_figure, enqbuf.items_sold_today+2,                     // BMG 29-Oct-2008 17
                       sizeof(plldbrec.sales_figure));                                       // BMG 29-Oct-2008 17
                //Add location count to main plldb shelf count value for running total       // BMG 29-Oct-2008 17
                WordToArray(plldbrec.qty_on_shelf, 4,                                        // BMG 29-Oct-2008 17
                            (satoi(plldbrec.qty_on_shelf, 4) + satoi(pPLC->count, 4)));      // BMG 29-Oct-2008 17
            }                                                                                // BMG 29-Oct-2008 17
        } else if (pPLC->cPickLocation == 'B') {            // SDH 22-02-05 EXCESS
            lrtp[hh_unit]->bLocation = 'S';                 // SDH 22-02-05 EXCESS
            memcpy(plldbrec.stock_room_count, pPLC->count,  // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.stock_room_count));      // SDH 22-02-05 EXCESS
            pack( plldbrec.time_bs_count, 2, sbuf, 4, 0);   // SDH 22-02-05 EXCESS
            memcpy(plldbrec.sales_bs_count,                 // SDH 22-02-05 EXCESS
                   enqbuf.items_sold_today+2,               // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.sales_figure));          // SDH 22-02-05 EXCESS
        } else if (pPLC->cPickLocation == 'O') {            // SDH 22-02-05 EXCESS
            lrtp[hh_unit]->bLocation = 'O';                 // SDH 22-02-05 EXCESS
            memcpy(plldbrec.ossr_count, pPLC->abOssrCount,  // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.ossr_count));            // SDH 22-02-05 EXCESS
            pack( plldbrec.time_ossr_count, 2, sbuf, 4, 0); // SDH 22-02-05 EXCESS
            memcpy(plldbrec.sales_ossr_count,               // SDH 22-02-05 EXCESS
                   enqbuf.items_sold_today+2,               // SDH 22-02-05 EXCESS
                   sizeof(plldbrec.sales_figure));          // SDH 22-02-05 EXCESS
        }                                                   // SDH 22-02-05 EXCESS

        //Flag as picked if counted on shop floor and home      // SDH 22-02-05 EXCESS
        //location. If it's determined not to have              // SDH 22-02-05 EXCESS
        //been picked yet then don't decrement the PLLOL count  // SDH 22-02-05 EXCESS

        if ((rfscfrec1and2.ossr_store == 'W') &&                // SDH 22-02-05 EXCESS
            (enqbuf.cOssrItem == 'Y')) {                        // SDH 22-02-05 EXCESS
            if ((plldbrec.time_sf_count[0] != 0xff) &&          // SDH 22-02-05 EXCESS
                (plldbrec.time_ossr_count[0] != 0xff)) {        // SDH 22-02-05 EXCESS
                plldbrec.item_status[0] = 'P';                  // SDH 22-02-05 EXCESS
            }                                                   // SDH 22-02-05 EXCESS
        } else if ((plldbrec.time_sf_count[0] != 0xff) &&       // SDH 22-02-05 EXCESS
                (memcmp(pPLC->abLocCounted, "  ", 2) == 0) &&   // BMG 29-Oct-2008 17
                (plldbrec.time_bs_count[0] != 0xff)) {          // SDH 22-02-05 EXCESS
            // This is not a multi-site item so set to picked   // BMG 29-Oct-2008 17
            // if we have salesfloor and backshop counts. Multi-// BMG 29-Oct-2008 17
            // site lists get set to P below from the PPC.      // BMG 29-Oct-2008 17
            plldbrec.item_status[0] = 'P';                      // SDH 22-02-05 EXCESS
            //Also set the item as picked if we're in the final     // SDH 22-02-05 EXCESS
            //location as we'll not get another chance to flag it   // SDH 22-02-05 EXCESS
            //pllolrec.cLocation field shown in brackets            // SDH 22-02-05 EXCESS
            //       GAP         PLC         PLC                    // SDH 22-02-05 EXCESS
            //       ---         ---         ---                    // SDH 22-02-05 EXCESS
            // SM/FF SF(S/F)->  BS(S/F) -->   OS (O)                // SDH 07-07-05 EXCESS
            // EXC    BS(E)-->    SF(E) -->   OS (C)                // SDH 07-07-05 EXCESS
            // EXC    OS(D)-->    SF(D) -->   BS (B)                // SDH 07-07-05 EXCESS
        } else if ((pllolrec.cLocation == 'B') ||               // SDH 22-02-05 EXCESS
                   (pllolrec.cLocation == 'C') ||               // SDH 22-02-05 EXCESS
                   (pllolrec.cLocation == 'O')) {               // SDH 22-02-05 EXCESS
            plldbrec.item_status[0] = 'P';                      // SDH 22-02-05 EXCESS
        }                                                       // SDH 22-02-05 EXCESS

        if (memcmp(pPLC->abLocCounted, "  ", 2) &&              // BMG 29-Oct-2008 17
           (memcmp(pPLC->abMSPicked, "Y", 1) ==0)) {            // BMG 29-Oct-2008 17
            // Multisite count item marked as picked from PPC.  // BMG 29-Oct-2008 17
            plldbrec.item_status[0] = 'P';                      // BMG 29-Oct-2008 17
        }

        //Special processing for Auto Fast Fill lists           //SDH 20-May-2009 Model Day
        if (pllolrec.cLocation == 'A') {                        //SDH 20-May-2009 Model Day
            WORD wPlldbFillQty = satoi(plldbrec.fill_qty,       //SDH 20-May-2009 Model Day
                                       sizeof(plldbrec.fill_qty));//SDH 20-May-2009 Model Day
            WORD wUnitFillQty = satoi(pPLC->count,              //SDH 20-May-2009 Model Day
                                      sizeof(pPLC->count));     //SDH 20-May-2009 Model Day
            wPlldbFillQty = _max(wPlldbFillQty - wUnitFillQty, 0);//SDH 20-May-2009 Model Day
            WORD_TO_ARRAY(plldbrec.fill_qty, wPlldbFillQty);    //SDH 20-May-2009 Model Day
            if (wPlldbFillQty == 0) {                           //SDH 20-May-2009 Model Day
                plldbrec.item_status[0] = 'P';                  //SDH 20-May-2009 Model Day
            } else {                                            //SDH 20-May-2009 Model Day
                plldbrec.item_status[0] = 'U';                  //SDH 20-May-2009 Model Day
            }                                                   //SDH 20-May-2009 Model Day
        }                                                       //SDH 20-May-2009 Model Day

        if (plldbrec.item_status[0] != 'P') {                   // SDH 22-02-05 EXCESS
            fDecrementItemCount = FALSE;                        // SDH 22-02-05 EXCESS
        }                                                       // SDH 22-02-05 EXCESS

        //Write PLLDB unlock                                    // SDH 22-02-05 EXCESS
        rc2 = WritePlldbUnlock(__LINE__);                       // SDH 22-02-05 EXCESS
        if (rc2 <= 0L) {                                        // SDH 22-02-05 EXCESS
            prep_nak("ERRORUnable to write (unlock) PLLDB. "    //SDH 23-Aug-2006 Planners
                      "Check appl event logs" );                // SDH 22-02-05 EXCESS
            WritePllolUnlock(wListId, __LINE__);                // SDH 20-May-2009 Model Day
            return;                                             // SDH 22-02-05 EXCESS
        }                                                       // SDH 22-02-05 EXCESS

        //Save these for later                                  // SDH 22-02-05 EXCESS
        wOssrCount = satoi(plldbrec.ossr_count,                 // SDH 22-02-05 EXCESS
                           sizeof(plldbrec.ossr_count));        // SDH 22-02-05 EXCESS
        wShelfCount = satoi(plldbrec.qty_on_shelf,              // SDH 22-02-05 EXCESS
                            sizeof(plldbrec.qty_on_shelf));     // SDH 22-02-05 EXCESS
        wBackCount = satoi(plldbrec.stock_room_count,           // SDH 22-02-05 EXCESS
                           sizeof(plldbrec.stock_room_count));  // SDH 22-02-05 EXCESS

        // Update PLLOL record if the item has just been picked // SDH 11-01-2005 OSSR WAN
        // for the first time, by decrementing the item count   // SDH 11-01-2005 OSSR WAN
        if (fDecrementItemCount) {                              // SDH 11-01-2005 OSSR WAN
            sprintf(sbuf, "%04d",                               // SDH 20-May-2009 Model Day
                    _max(satoi(pllolrec.item_count, 4) - 1, 0));// SDH 20-May-2009 Model Day
            memcpy(pllolrec.item_count, sbuf, 4);               // SDH 11-01-2005 OSSR WAN
        }                                                       // SDH 20-May-2009 Model Day

        //Write PLLOL back with UNLOCK                          // SDH 20-May-2009 Model Day
        rc2 = WritePllolUnlock(wListId, __LINE__);              // SDH 20-May-2009 Model Day
        if (rc2 <= 0L) {
            prep_nak("ERRORUnable to write to PLLOL. "          // SDH 23-Aug-2006 Planners
                      "Check appl event logs" );
            return;
        }

    }                                                           // 19-02-04 PAB

    //Start of STKMQ update processing                          // SDH 22-02-2005 EXCESS
    //If Shelf Monitor or Excess Stock then                     // SDH 07-07-2005 EXCESS
    //  If Item Picked or Fast Count then                       // SDH 07-07-2005 EXCESS
    if ((pPLC->gap_flag[0] == 'Y') || fExcessList) {            // SDH 07-07-2005 EXCESS
        if ((plldbrec.item_status[0] == 'P') || (wListId == 0)) {// SDH 07-07-2005 EXCESS

            //Only use the saved sales figures if the shop
            //floor has been counted
            if ((wListId != 0) &&                               // SDH 22-02-2005 EXCESS
                (plldbrec.time_sf_count[0] != 0xFF)) {          // SDH 22-02-2005 EXCESS
                sales_figure = satol(plldbrec.sales_figure, 4);
                items_sold_today = satol(enqbuf.items_sold_today, 6);
                // if this is a fast count there is no sales    // 19-02-04 PAB
                // history nullify the variable for the variance// 19-02-04 PAB
                //calc is correct                               // 19-02-04 PAB
            } else {                                            // 19-02-04 PAB
                sales_figure = 0;                               // 19-02-04 PAB
                items_sold_today = 0;                           // 19-02-04 PAB
                wShelfCount = 0;                                // 19-02-04 PAB
                wBackCount = satoi(pPLC->count,                 // SDH 22-02-2005 EXCESS
                                   sizeof(pPLC->count));        // SDH 22-02-2005 EXCESS
                wOssrCount = 0;                                 // SDH 22-02-2005 EXCESS
            }                                                   // 19-02-04 PAB

            if (debug) {
                sprintf(msg, "1 - Current Stock Figure : %ld", lStockFigure);
                disp_msg(msg);
                sprintf(msg, "2 - Shop Floor Count     : %d", wShelfCount);
                disp_msg(msg);
                sprintf(msg, "3 - Back Shop Count      : %d", wBackCount);
                disp_msg(msg);
                sprintf(msg, "4 - OSSR Count           : %d", wOssrCount);
                disp_msg(msg);
                sprintf(msg, "5 - Sales at Shop Count  : %ld", sales_figure);
                disp_msg(msg);
                sprintf(msg, "6 - items_sold_today     : %ld", items_sold_today);
                disp_msg(msg);
            }

            variance = items_sold_today - sales_figure;
            new_count = wShelfCount + wBackCount + wOssrCount - variance;
            mismatch_qty = abs(lStockFigure - new_count);
            mismatch_val = abs(mismatch_qty * satol( enqbuf.item_price, 6 ));

            if (debug) {
                sprintf( msg, "6 - variance     : %ld", variance);
                disp_msg(msg);
                sprintf( msg, "7 - new count    : %ld", new_count);
                disp_msg(msg);
                sprintf( msg, "8 - mismatch_qty : %ld", mismatch_qty);
                disp_msg(msg);
                sprintf( msg, "9 - mismatch_val : %ld", mismatch_val);
                disp_msg(msg);
            }

            if (wListId == 0) {                                 // SDH 03-02-2005 UPWARDS TSF
                usrrc = open_minls();                           // 24-02-04 PAB
                if (usrrc<=RC_DATA_ERR) {                       // 24-02-04 PAB
                    prep_nak("ERRORUnable to open MINLS file. " //SDH 23-Aug-2006 Planners
                              "Check appl event logs" );        // 24-02-04 PAB
                    return;                                      // 24-02-04 PAB
                }
            }

            //Read the MINLS and handle any errors
            pack( minlsrec.boots_code, 4, pPLC->boots_code, 7, 1 );
            rc2 = ReadMinls(__LINE__);                          // SDH 14-03-2005 EXCESS
            if (rc2<=0L) {
                if ((rc2&0xFFFF)==0x06C8 || (rc2&0xFFFF)==0x06CD) {
                    minls.present=FALSE;
                } else {
                    prep_nak("ERRORUnable to read from MINLS. " //SDH 23-Aug-2006 Planners
                              "Check appl event logs" );
                    //Close MINLS if a fast count
                    if (wListId == 0) close_minls(CL_SESSION);  // 24-02-04 PAB
                    return;
                }
            } else {
                minls.present=TRUE;
            }

            //Is there a count pending -- as shown on the IDF?
            count_pending = ((enqbuf.idf_bit_flags_2 & 0x04)!=0);
            if (debug) {
                sprintf(msg, "count pending : %d", count_pending);
                disp_msg(msg);
            }

            //Ignore the count pending flag for picks           // 09-03-05 SDH OSSR WAN
            if (wListId != 0) count_pending = 0;                // 09-03-05 SDH OSSR WAN

            //Get the recount date as a Julian
            if (minls.present) {
                unpack( sbuf, 6, minlsrec.recount_date, 3, 0 );
                day   = satol( sbuf+4, 2 );
                month = satol( sbuf+2, 2 );
                year  = satol( sbuf,   2 );
                year += ((year>84L)?1900L:2000L);
                dMinlsRecountJul = ConvGJ( day, month, year );  // 15-9-04 PAB
            } else {
                dMinlsRecountJul = dTodayJul;
            }

            if (debug) {
                ConvJG( dMinlsRecountJul, &day, &month, &year );
                sprintf( msg, "Recount for item expected on %02ld/%02ld/%04ld (jd:%9.2f)",
                         day, month, year, dMinlsRecountJul );
                disp_msg(msg);
            }

            if ((!minls.present && !count_pending) ||
                (minls.present                 &&
                 minlsrec.count_status[0]=='2' &&
                 dMinlsRecountJul <= dTodayJul   ) ||
                (lStockFigure <= -1 )              ||           // 10-3-2005 SDH EXCESS
                (new_count == 0)) {                             //  5-7-2004 PAB new count is zero

                // Write record to MINLS
                if (!minls.present) {
                    memset( minlsrec.boots_code, 0x00, 4 );
                    pack( minlsrec.boots_code, 4,
                          ((LRT_PLC *)(inbound))->boots_code, 7, 1 );
                    memset( minlsrec.recount_date, 0x00, 3);    // 15-9-04 PAB
                    sprintf( sbuf, "%06ld", mismatch_qty );
                    pack( minlsrec.discrepancy, 3, sbuf, 6, 0 );
                    //Always set the MINLS rec                  //SDH 09-02-2005 UPWARDS TSF
                    //type to 3, unless its an item with no     //SDH 09-02-2005 UPWARDS TSF
                    //count pending, a +ve TSF, a -ve adj.      //SDH 09-02-2005 UPWARDS TSF
                    //(We also ASSUME that the item is NOT      //SDH 09-02-2005 UPWARDS TSF
                    // on the NEGSC)                            //SDH 09-02-2005 UPWARDS TSF
                    if ((count_pending)      ||                 //SDH 09-02-2005 UPWARDS TSF
                        (lStockFigure <= -1) ||                 //SDH 09-02-2005 UPWARDS TSF
                        (new_count >= lStockFigure)) {          //SDH 09-02-2005 UPWARDS TSF
                        minlsrec.count_status[0] = '3';         //SDH 09-02-2005 UPWARDS TSF
                    } else {                                    //SDH 09-02-2005 UPWARDS TSF
                        minlsrec.count_status[0] = '1';         //SDH 09-02-2005 UPWARDS TSF
                    }                                           //SDH 09-02-2005 UPWARDS TSF
                }                                               //SDH 09-02-2005 UPWARDS TSF

                // minls status 1 = initial count
                // minls status 2 = pending recount
                // minls status 3 = recount
                // minls status 4 = count complete

                //*minlsrec.count_status =
                //    ( (!minls.present && !count_pending) ?'1':'3');


                //Force the record type to '3' for a fast count // SDH 03-02-2005 UPWARDS TSF
                if (*minlsrec.count_status == '2') {            // SDH 03-02-2005 UPWARDS TSF
                    *minlsrec.count_status = '3';               // 16-9-04 PAB
                }

                if ((*minlsrec.count_status == '3') &&
                    (minlsrec.recount_date[1] == 0x00 )) {      // 16-9-04 PAB
                    //we have a recount with no date. Set       // 16-9-04 PAB
                    //recount date to today                     // 16-9-04 PAB
                    ConvJG(dTodayJul, &day, &month, &year);     // 16-9-04 PAB
                    sprintf(sbuf, "%02d", (UBYTE)(year%100L));  // 16-9-04 PAB
                    pack(minlsrec.recount_date, 1, sbuf, 2, 0); // 16-9-04 PAB
                    sprintf(sbuf, "%02d", (UBYTE)(month));      // 16-9-04 PAB
                    pack(minlsrec.recount_date+1, 1, sbuf, 2, 0);//16-9-04 PAB
                    sprintf(sbuf, "%02d", (UBYTE)(day));        // 16-9-04 PAB
                    pack(minlsrec.recount_date+2, 1, sbuf, 2, 0);//16-9-04 PAB
                }

                //Should we check the result?                   // SDH 14-03-2005 EXCESS
                WriteMinls(__LINE__);                           // SDH 14-03-2005 EXCESS

                usrrc = open_stkmq();
                if (usrrc<=RC_DATA_ERR) {
                    prep_nak("ERRORUnable to open STKMQ file. " //SDH 23-Aug-2006 Planners
                              "Check appl event logs" );
                    //Close MINLS if a fast count               // SDH 03-02-2005 UPWARDS TSF
                    if (wListId == 0) {                         // SDH 03-02-2005 UPWARDS TSF
                        usrrc = close_minls( CL_SESSION);       // SDH 03-02-2005 UPWARDS TSF
                    }                                           // SDH 03-02-2005 UPWARDS TSF
                    return;
                }

                BYTE cStkmqType;                                // SDH 06-04-2005 UPWARDS TSF
                if (minlsrec.count_status[0] == '1') {          // SDH 03-02-2005 UPWARDS TSF
                    cStkmqType = 'C';                           // SDH 03-02-2005 UPWARDS TSF
                } else {                                        // SDH 03-02-2005 UPWARDS TSF
                    cStkmqType = 'R';                           // SDH 03-02-2005 UPWARDS TSF
                }                                               // SDH 03-02-2005 UPWARDS TSF

                // Write type 13 record to STKMQ
                count_accepted = TRUE;                          // 15-9-04 PAB
                stkmqrec[0] = 0x22;                             // SDH 03-02-2005 UPWARDS TSF
                stkmqrec[1] = 0x13;                             // SDH 03-02-2005 UPWARDS TSF
                stkmqrec[2] = 0x3B;                             // SDH 03-02-2005 UPWARDS TSF
                sysdate( &day, &month, &year, &hour, &min, &sec );
                sprintf( sbuf, "%02ld%02ld%02ld", year%100, month, day );
                pack( stkmqrec+3, 3, sbuf, 6, 0 );

                sprintf( sbuf, "%02d%02d%02d", hour, min, (WORD)sec );
                pack( stkmqrec+6, 3, sbuf, 6, 0 );
                pack( stkmqrec+23, 2, sbuf, 4, 0 );             // SDH 03-02-2005 UPWARDS TSF

                memcpy( stkmqrec+9, "000000", 6 );
                stkmqrec[15] = cStkmqType;                      // SDH 03-02-2005 UPWARDS TSF
                pack( stkmqrec+16, 4,
                      ((LRT_PLC *)(inbound))->boots_code, 7, 1 );
                sprintf( sbuf, "%02ld%02ld%02ld", day, month, year%100 );
                pack( stkmqrec+20, 3, sbuf, 6, 0 );

                memset( stkmqrec+25, 0x00, 5);
                pack( stkmqrec+25+2, 3, enqbuf.item_price, 6, 0 );
                memset( stkmqrec+31-1, 0x3B, 1 );
                sprintf( sbuf, "%04ld%cXXXX%c%c%c",
                         new_count, 0x3B, 0x22, 0x0D, 0x0A );
                memcpy( stkmqrec+31, sbuf, 16 );

                if (debug) {
                    sprintf(msg, "WR STKMQ :");
                    disp_msg(msg);
                    dump( (BYTE *)&stkmqrec, STKMQ_T13_LTH );
                }
                rc2 = s_write( A_EOFOFF, stkmq.fnum, (void *)&stkmqrec,
                               STKMQ_T13_LTH, 0L );
                if (rc2<=0) {
                    log_event101(rc2, STKMQ_REP, __LINE__);
                    if (debug) {
                        sprintf(msg, "Err-W to STKMQ. RC:%08lX", rc2);
                        disp_msg(msg);
                    }
                    prep_nak("ERRORUnable to write to STKMQ. Check appl event logs");
                    return;
                }
                disp_msg("Write STKMQ OK");
                close_stkmq( CL_SESSION );

            }                                                   // endif - write STKMQ & MINLS record
        }                                                       //SDH 07-07-05 EXCESS STOCK
    }                                                           //SDH 07-07-05 EXCESS STOCK

    //Get counts as integers                                    //SDH 25-01-2005 OSSR WAN
    //Belt & braces: force the item to non-OSSR                 //SDH 25-01-2005 OSSR WAN
    //if not an OSSR store                                      //SDH 25-01-2005 OSSR WAN
    wOssrCount = satoi(plldbrec.ossr_count,                     //SDH 25-01-2005 OSSR WAN
                       sizeof(plldbrec.ossr_count));            //SDH 25-01-2005 OSSR WAN
    wShelfCount = satoi(plldbrec.qty_on_shelf,                  //SDH 25-01-2005 OSSR WAN
                        sizeof(plldbrec.qty_on_shelf));         //SDH 25-01-2005 OSSR WAN
    wBackCount = satoi(plldbrec.stock_room_count,               //SDH 25-01-2005 OSSR WAN
                       sizeof(plldbrec.stock_room_count));      //SDH 25-01-2005 OSSR WAN
    BOOLEAN fTrueGap = FALSE;                                   //SDH 25-01-2005 OSSR WAN
    if (rfscfrec1and2.ossr_store != 'W') enqbuf.cOssrItem = 'N';    //SDH 25-01-2005 OSSR WAN

    //Determine whether the gap is a true gap.                  //SDH 25-01-2005 OSSR WAN
    //Don't gap excess stock lists                              //SDH 07-07-2005 EXCESS
    if ((plldbrec.gap_flag[0] == 'Y') &&                        //SDH 25-01-2005 OSSR WAN
        (!fExcessList)                &&                        //SDH 07-07-2005 EXCESS
        (wShelfCount == 0)            &&                        //SDH 25-01-2005 OSSR WAN
        (wBackCount  == 0)            &&                        //SDH 25-01-2005 OSSR WAN
        (wOssrCount  == 0)) {                                   //SDH 25-01-2005 OSSR WAN
        //Its gotta be a gap everywhere if its not an OSSR item,//SDH 25-01-2005 OSSR WAN
        //or if its been counted in the OSSR                    //SDH 25-01-2005 OSSR WAN
        if ((enqbuf.cOssrItem != 'Y') ||                        //SDH 25-01-2005 OSSR WAN
            (plldbrec.time_ossr_count[0] != 0xff)) {            //SDH 25-01-2005 OSSR WAN
            fTrueGap = TRUE;                                    //SDH 25-01-2005 OSSR WAN
        }                                                       //SDH 25-01-2005 OSSR WAN
    }                                                           //SDH 25-01-2005 OSSR WAN


    if (lrtp[hh_unit]->fnum2 <= 0) {                                // 04-03-2009 BMG
        //Create workfile because it is not yet there.              // 04-03-2009 BMG
        //Can happen with MC70's as they don't send PLS's.          // 04-03-2009 BMG
        usrrc = prepare_workfile( hh_unit, SYS_GAP );               // 04-03-2009 BMG
        if (usrrc<RC_IGNORE_ERR) {                                  // 04-03-2009 BMG
            if (usrrc == RC_DATA_ERR) {                             // 04-03-2009 BMG
                prep_nak("ERRORUnable to create GAPBF workfile. "   // 04-03-2009 BMG
                          "Check appl event logs" );                // 04-03-2009 BMG
            } else {                                                // 04-03-2009 BMG
                prep_pq_full_nak();                                 // 04-03-2009 BMG
            }                                                       // 04-03-2009 BMG
            if (wListId == 0) close_minls(CL_SESSION);              // 04-03-2009 BMG
            return;                                                 // 04-03-2009 BMG
        }                                                           // 04-03-2009 BMG
    }                                                               // 04-03-2009 BMG

    //Build GAPBF record if it's a true gap                         //SDH 19-08-2009 Model Day
    if (fTrueGap) {                                                 //SDH 19-08-2009 Model Day
        //If device is MC70                                         //SDH 19-08-2009 Model Day
        if (lrtp[hh_unit]->Type[0] == 'M') {                        //SDH 19-08-2009 Model Day
            //Write out list number instead of item code. The header// 09-03-2009 BMG
            //record of the file was not written out when the file  // 09-03-2009 BMG
            //was created, to accomodate this change.               // 09-03-2009 BMG
            memcpy(gapbfrec, "\"000         \"\r\n", 16);           // 09-03-2009 BMG
            memcpy(gapbfrec + 1, pPLC->list_id, 3);                 // 09-03-2009 BMG
            // Write an extra time so even if there is only one list// 25-03-2009 BMG
            // the file is > 16 bytes and the launcher won't delete // 25-03-2009 BMG
            // the file! Bit of a bodge!                            // 25-03-2009 BMG
            s_write(A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum2,        // 09-03-2009 BMG
                    (void*)&gapbfrec, 16L, 0L);                     // 09-03-2009 BMG
        } else {                                                    // 09-03-2009 BMG
            memcpy(gapbfrec, "\"000000000000\"\r\n", 16);           //SDH 25-01-2005 OSSR WAN
            memcpy(gapbfrec + 7, pPLC->boots_code, 6);              //SDH 25-01-2005 OSSR WAN
            if (debug) {                                            //SDH 25-01-2005 OSSR WAN
                disp_msg( "WR GAPBF IN PLC:" );                     //SDH 25-01-2005 OSSR WAN
                dump(gapbfrec, 16);                                 //SDH 25-01-2005 OSSR WAN
            }                                                       //SDH 25-01-2005 OSSR WAN
        }                                                           //SDH 25-01-2005 OSSR WAN
        rc2 = s_write( A_FLUSH | A_FPOFF, lrtp[hh_unit]->fnum2,     //SDH 25-01-2005 OSSR WAN
               (void *)&gapbfrec, 16L, 0L );                        //SDH 25-01-2005 OSSR WAN
        if (rc2<0L) {                                               //SDH 25-01-2005 OSSR WAN
            if (debug) {                                            //SDH 25-01-2005 OSSR WAN
                sprintf(msg, "Err-W to %s. RC:%08lX",               //SDH 25-01-2005 OSSR WAN
                    pq[lrtp[hh_unit]->pq_sub2].fname, rc2);         //SDH 25-01-2005 OSSR WAN
                disp_msg(msg);                                      //SDH 25-01-2005 OSSR WAN
            }                                                       //SDH 25-01-2005 OSSR WAN
            prep_nak("ERRORUnable to write to GAPBF. "              //SDH 25-01-2005 OSSR WAN
                     "Check appl event logs" );                     //SDH 25-01-2005 OSSR WAN
            //Close MINLS if a fast count                           //SDH 25-01-2005 OSSR WAN
            if (wListId == 0) close_minls(CL_SESSION);              //SDH 25-01-2005 OSSR WAN
            return;                                                 //SDH 25-01-2005 OSSR WAN
        }                                                           //SDH 25-01-2005 OSSR WAN
    }                                                               // 09-03-2009 BMG

    // 15-9-04 PAB                                              // 15-9-04 PAB
    if (count_accepted == TRUE) {                               // 15-9-04 PAB
        prep_ack("");                                           // 15-9-04 PAB
    } else {                                                    // 15-9-04 PAB

        //Count not accepted.  If a count was pending then      // SDH 16-02-2005 UPWARDS TSF
        //delete any MINLS record                               // SDH 16-02-2005 UPWARDS TSF
        if (count_pending) {                                    // SDH 16-02-2005 UPWARDS TSF
            pack(minlsrec.boots_code, 4,                        // SDH 16-02-2005 UPWARDS TSF
                 pPLC->boots_code, 7, 1);                       // SDH 16-02-2005 UPWARDS TSF
            rc2 = s_special(0x74, 0, minls.fnum,                // SDH 16-02-2005 UPWARDS TSF
                            minlsrec.boots_code, MINLS_KEYL,    // SDH 16-02-2005 UPWARDS TSF
                            0L, 0L);                            // SDH 16-02-2005 UPWARDS TSF
            if (rc2 < 0L) {                                     // SDH 16-02-2005 UPWARDS TSF
                sprintf(msg, "MINLS record delete error.  "     // SDH 16-02-2005 UPWARDS TSF
                        "RC:%08lX", rc2);                       // SDH 16-02-2005 UPWARDS TSF
            } else {                                            // SDH 16-02-2005 UPWARDS TSF
                strcpy(msg, "MINLS record deleted.");           // SDH 16-02-2005 UPWARDS TSF
            }                                                   // SDH 16-02-2005 UPWARDS TSF
            disp_msg(msg);                                      // SDH 16-02-2005 UPWARDS TSF
        }                                                       // SDH 16-02-2005 UPWARDS TSF

        if ((dMinlsRecountJul > dTodayJul) &&                   // 15-9-04 PAB
            (new_count != lStockFigure)) {                      // 15-9-04 PAB
            //Don't assume that day/month/year still contain    // SDH 16-02-2005 UPWARDS TSF
            //the expected vars                                 // SDH 16-02-2005 UPWARDS TSF
            ConvJG(dMinlsRecountJul, &day, &month, &year);      // SDH 16-02-2005 UPWARDS TSF
            sprintf(msg, "Count rejected. Recount for item "    // 15-9-04 PAB
                    "not expected until %02ld/%02ld/%04ld",     // 15-9-04 PAB
                     day, month, year );                        // SDH 16-02-2005 UPWARDS TSF
        } else if ((minls.present) &&                           // 15-9-04 PAB
                   (*minlsrec.count_status == '1')) {           // 15-9-04 PAB
            sprintf (msg, "A previous count is still "          // 15-9-04 PAB
                     "being processed for this item");          // 15-9-04 PAB
        } else if (count_pending) {                             // SDH 09-03-2005 OSSR WAN
            sprintf (msg, "Count rejected: head office "        // SDH 09-03-2005 OSSR WAN
                     "count pending.\nCheck HO count list.");   // SDH 09-03-2005 OSSR WAN
        } else {                                                // 15-9-04 PAB
            sprintf (msg, "Count rejected. Could not be "       // 15-9-04 PAB
                     "processed at this time.");                // 15-9-04 PAB
        }                                                       // 15-9-04 PAB
        prep_nak(msg);                                          // SDH 25-Aug-2006 Planners
    }                                                           // 15-9-04 PAB

    //Close MINLS if a fast count                               // SDH 26-11-04 CREDIT CLAIM
    if (wListId == 0) close_minls( CL_SESSION);                 // SDH 26-11-04 CREDIT CLAIM

    // Audit                                                    // SDH 20-May-2009 Model Day
    MEMSET(pLGPLC->abListIDPkd, 0x00);                          // SDH 19-Aug-2009 Model Day
    pack(pLGPLC->abListIDPkd, 2, pPLC->list_id, 3, 1);          // SDH 20-May-2009 Model Day
    MEMSET(pLGPLC->abListSeqPkd, 0x00);                         // SDH 19-Aug-2009 Model Day
    pack(pLGPLC->abListSeqPkd, 2, pPLC->seq, 3, 1);             // SDH 20-May-2009 Model Day
    pack(pLGPLC->abItemCodePkd, 3, pPLC->boots_code, 6, 0);     // SDH 20-May-2009 Model Day
    pack(pLGPLC->abCountPkd, 2, pPLC->count, 4, 0);             // SDH 20-May-2009 Model Day
    if (wListId == 0) {                                         // SDH 19-Aug-2009 Model Day
        pLGPLC->bListType = ' ';                                // SDH 19-Aug-2009 Model Day
    } else {                                                    // SDH 19-Aug-2009 Model Day
        pLGPLC->bListType = pllolrec.cLocation;                 // SDH 19-Aug-2009 Model Day
    }                                                           // SDH 19-Aug-2009 Model Day
    MEMSET(pLGPLC->abFiller, 0x00);                             // SDH 20-May-2009 Model Day
    lrt_log(LOG_PLC, hh_unit, dtls);                            // SDH 20-May-2009 Model Day

}

// ------------------------------------------------------------------------------------
//
// PLX - Picking Lists - Sign Off List
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_PLX_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE lines[4];
   BYTE items[6];
   BYTE complete[1];
} LRT_PLX;                                    // Picking list exit
#define LRT_PLX_LTH sizeof(LRT_PLX)

void PListSignOffList(char* inbound) {

    LRT_PLX* pPLX = (LRT_PLX*)inbound;                          // SDH 22-02-2005 EXCESS
    LRTLG_PLX* pLGPLX = (LRTLG_PLX*)dtls;                       // SDH 20-MAy-2009 Model Day
    LONG list_id = satol(pPLX->list_id, 3);                     // SDH 22-02-2005 EXCESS
    LONG day, month, year, sec;                                 // Streamline SDH 17-Sep-2008
    WORD hour, min;                                             // Streamline SDH 17-Sep-2008
    LONG rc2;

    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    // Read pllol record
    rc2 = ReadPllolLock(list_id, __LINE__);                     // SDH 20-May-2009 Model Day
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read from PLLOL. Check appl event logs");
        return;
    }

    //For auto fast fill lists, ignore PPC confirm flag and     // SDH 20-May-2009 Model Day
    //set internally if all items are picked                    // SDH 20-May-2009 Model Day
    if (pllolrec.cLocation == 'A') {                            // SDH 20-May-2009 Model Day
        if (satoi(pllolrec.item_count, 4) == 0) {               // SDH 20-May-2009 Model Day
            pPLX->complete[0] = 'Y';                            // SDH 20-May-2009 Model Day
        } else {                                                // SDH 20-May-2009 Model Day
            pPLX->complete[0] = 'N';                            // SDH 20-May-2009 Model Day
        }                                                       // SDH 20-May-2009 Model Day
    }                                                           // SDH 20-May-2009 Model Day

    //Update current location for logging                       // SDH 12-01-2005 OSSR WAN
    if ((pllolrec.cLocation == 'O') ||                          // SDH 22-02-2005 EXCESS
        (pllolrec.cLocation == 'C')) {                          // SDH 22-02-2005 EXCESS
        lrtp[hh_unit]->bLocation = 'O';                         // SDH 22-02-2005 EXCESS
    } else {                                                    // SDH 22-02-2005 EXCESS
        lrtp[hh_unit]->bLocation = 'S';                         // SDH 22-02-2005 EXCESS
    }                                                           // SDH 22-02-2005 EXCESS

    // Update pllol record
    if (pPLX->complete[0] == 'Y') {
        pllolrec.list_status[0] = 'P';                          // Status : Picked
        sysdate( &day, &month, &year, &hour, &min, &sec );
        sprintf( sbuf, "%02d%02d", hour, min );
        memcpy( pllolrec.pick_end_time, sbuf, 4 );
        //Assigned to OSSR, set back to unpicked                // SDH 12-01-2005 OSSR WAN
    } else if (pPLX->complete[0] == 'O') {                      // SDH 12-01-2005 OSSR WAN
        pllolrec.cLocation = 'O';                               // SDH 12-01-2005 OSSR WAN
        pllolrec.list_status[0] = 'U';                          // SDH 12-01-2005 OSSR WAN
    } else if (pPLX->complete[0] == 'B') {                      // SDH 23-02-2005 EXCESS
        if (SetListUnpicked(list_id) != RC_OK) {                // SDH 20-May-2009 Model Day
            WritePllolUnlock(list_id, __LINE__);                // SDH 20-May-2009 Model Day
            prep_nak("ERRORCannot access PLLDB. "               // SDH 25-Aug-2006 Planners
                     "Check appl event logs" );                 // SDH 23-05-2005 EXCESS
            return;                                             // SDH 23-05-2005 EXCESS
        }                                                       // SDH 23-05-2005 EXCESS
        pllolrec.cLocation = 'B';                               // SDH 23-02-2005 EXCESS
        pllolrec.list_status[0] = 'U';                          // SDH 23-02-2005 EXCESS
    } else if (pPLX->complete[0] == 'C') {                      // SDH 23-02-2005 EXCESS
        if (SetListUnpicked(list_id) != RC_OK) {                // SDH 20-May-2009 Model Day
            WritePllolUnlock(list_id, __LINE__);                // SDH 20-May-2009 Model Day
            prep_nak("ERRORCannot access PLLDB. "               // SDH 25-Aug-2006 Planners
                     "Check appl event logs" );                 // SDH 23-05-2005 EXCESS
            return;                                             // SDH 23-05-2005 EXCESS
        }                                                       // SDH 23-05-2005 EXCESS
        pllolrec.cLocation = 'C';                               // SDH 23-02-2005 EXCESS
        pllolrec.list_status[0] = 'U';                          // SDH 23-02-2005 EXCESS
    } else {                                                    // New
        pllolrec.list_status[0] = ' ';                          // Status : Inactive
    }                                                           // New

    // Following section made unconditional
    if (WritePllolUnlock(list_id, __LINE__) <= 0L) {            // SDH 20-May-2009 Model Day
        prep_nak("ERRORUnable to write to PLLOL. "              // SDH 20-May-2009 Model Day
                 "Check appl event logs");
        return;
    }

    //Do both just in case                                      // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                       // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                       // SDH 08-05-2006 Bug fix

    prep_ack("");                                               //SDH 23-Aug-2006 Planners

    // Audit
    MEMCPY(pLGPLX->abListID, pPLX->list_id);                    //SDH 20-May-2009 Model Day
    pLGPLX->bComplete = pPLX->complete[0];                      //SDH 20-May-2009 Model Day
    pLGPLX->bListType = pllolrec.cLocation;                     //SDH 20-May-2009 Model Day
    MEMSET(pLGPLX->abFiller, 0x00);                             //SDH 07-Jul-2009 Model Day
    lrt_log(LOG_PLX, hh_unit, dtls);                            // SDH 19-01-2005 OSSR WAN
}

// ------------------------------------------------------------------------------------
//
// PLF - Picking Lists - Sign Off Session
//
//
//
// ------------------------------------------------------------------------------------

/*
typedef struct LRT_PLF_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_PLF;                                    // Picking list main exit
#define LRT_PLF_LTH sizeof(LRT_PLF)
*/

void PListSignOffSession(char* inbound) {

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    close_minls( CL_SESSION );
    close_plldb( CL_SESSION );
    close_pllol( CL_SESSION );

    //Process both just in case                                     // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// PCS - Price Check Sign On
//
//
//
// ------------------------------------------------------------------------------------

/*
typedef struct LRT_PCS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_PCS;                                    // Price check signon
#define LRT_PCS_LTH sizeof(LRT_PCS)
*/

typedef struct {
    BYTE cmd[3];
    BYTE pchk_target[4];
    BYTE pchk_done[4];
} LRT_PCR;                                    // Price check start response
#define LRT_PCR_LTH sizeof(LRT_PCR)

void PriceCheckSignOn(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to "
                      "create workfile. "
                      "Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        return;
    }

    // Prepare PCR
    memcpy( ((LRT_PCR *)out)->cmd, "PCR", 3 );
    sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_target );            // 16-11-04 SDH
    memcpy( ((LRT_PCR *)out)->pchk_target, sbuf, 4 );
    sprintf( sbuf, "%04ld", rfscfrec1and2.pchk_done );              // 16-11-04 SDH
    memcpy( ((LRT_PCR *)out)->pchk_done, sbuf, 4 );
    out_lth = LRT_PCR_LTH;

}

// ------------------------------------------------------------------------------------
//
// PCX - Price Check Sign off
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE items[4];
   BYTE sels[4];
} LRT_PCX;                                    // Price check signoff
#define LRT_PCX_LTH sizeof(LRT_PCX)

void PriceCheckSignOff(char* inbound) {

//    LRT_PCX* pPCX = (LRT_PCX*)inbound;                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//    LONG rc2;                                                   // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//    URC usrrc;                                                  // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

//    if (memcmp(lrtp[hh_unit]->Type, "M", 1)==0) {               // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        //If the device is an MC70 we need up update the        // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        //price check total on the RFSCF from PCX items         // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        usrrc = RfscfOpen();                                    // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        if (usrrc != RC_OK) {                                   // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            prep_nak("ERRORUnable to open RFSCF file. "         // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "Please phone help desk.");               // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            return;                                             // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        }                                                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rc2 = RfscfRead(0L, __LINE__);                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        rc2 = RfscfRead(1L, __LINE__);                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rfscfrec1and2.pchk_done+=satoi(pPCX->items, 4);         // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        rc2 = RfscfUpdate(__LINE__);                            // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        if (rc2<=0L) {                                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            prep_nak("ERRORUnable to "                          // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "write to RFSCF. "                        // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//                      "Check appl event logs" );                // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//            return;                                             // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG
//        }                                                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//        usrrc = RfscfClose( CL_SESSION );                       // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

//    }                                                           // BMG 10-09-2008 18 MC70 // 25-03-2009 BMG

    //Do both just in case                                          // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

    // Audit
    ((LRTLG_PCX *)dtls)->items_checked =
    ((LONG)satol( (((LRT_PCX *)(inbound))->items), 4 ));
    ((LRTLG_PCX *)dtls)->sels_printed =
    ((LONG)satol( (((LRT_PCX *)(inbound))->sels), 4 ));
    memset( ((LRTLG_PCX *)dtls)->resv, 0x00, 4 );
    lrt_log( LOG_PCX, hh_unit, dtls );                              // SDH 26-01-2005

}

// ------------------------------------------------------------------------------------
//
// INS - Information sign on
//
//
//
// ------------------------------------------------------------------------------------

/*
typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_INS;                                    // Information sign on
#define LRT_INS_LTH sizeof(LRT_INS)
*/

void InfoSignOn(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to create workfile. Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        close_wrf( CL_SESSION );
        close_psbt( CL_SESSION );
        return;
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// INX - Information sign off
//
//
//
// ------------------------------------------------------------------------------------

/*
typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_INX;                                    // Information exit
#define LRT_INX_LTH sizeof(LRT_INX)
*/

void InfoSignOff(char* inbound) {

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    //Do both just in case                                          // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// SSE - Information Store Sales request
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_SSE_Txn {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_SSE;                                    // Store sales enquiry
#define LRT_SSE_LTH sizeof(LRT_SSE)

void InfoStoreSales(char* inbound) {

    TIMEDATE now;                                               // Streamline SDH 17-Sep-2008
    URC usrrc;                                                  // Streamline SDH 17-Sep-2008

    if (strncmp(((LRT_SSE *)(inbound))->opid, "000", 3)==0) {       // 5-7-2004 PAB
        hh_unit = 255;                                              // 20-7-2004 PAB
        // if this request has come from StoreNet USerID 000 then simulate
        // operator sign on processing - allocate hht table entry // 5-7-2004 PAB
        usrrc = alloc_lrt_table( hh_unit );                         // 5-7-2004 PAB
        if (usrrc<RC_IGNORE_ERR) {                                  // 5-7-2004 PAB
            prep_nak("ERRORUnable to allocate storage. Check appl event logs");// 5-7-2004 PAB
            return;                                                  // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB
        // Log current time                                         // 5-7-2004 PAB
        s_get( T_TD, 0L, (void *)&now, TIMESIZE );                  // 5-7-2004 PAB
        lrtp[hh_unit]->last_active_time = now.td_time;              // 5-7-2004 PAB
        memcpy( (BYTE *)&(lrtp[hh_unit]->txn), (BYTE *)inbound, 3); // 5-7-2004 PAB
        memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );        // 5-7-2004 PAB
                                                                    // 5-7-2004 PAB
        // Set state                                                // 5-7-2004 PAB
        lrtp[hh_unit]->state = ST_LOGGED_ON;                        // 5-7-2004 PAB
        // Reset misc counts (Unused at present)                    // 5-7-2004 PAB
        lrtp[hh_unit]->count1 = 0;                                  // 5-7-2004 PAB
        lrtp[hh_unit]->count2 = 0;
    }

    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM

    if (strncmp(((LRT_SSE *)(inbound))->opid, "000", 3)!=0) {       // 5-7-2004 PAB
        if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
        UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM
    }

    // 15-7-2004 PAB
    usrrc = open_tsfD();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open EALTERMS file. Check appl event logs");
        return;
    }

    usrrc = open_psbtD();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open PSBTERMS file. Check appl event logs");
        return;
    }

    usrrc = open_wrfD();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open EALWORKG file. Check appl event logs");
        close_psbt( CL_SESSION );
        return;
    }

    usrrc = store_sales_enquiry( (LRT_SSR *)&out );
    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc==RC_FILE_ERR) {
            prep_nak("ERROROne or more files are not open. Check appl event logs");
        } else if (usrrc==RC_DATA_ERR) {
            prep_nak("ERRORUnable to access data. Check appl event logs");
        } else {
            prep_nak("ERRORUndefined 16. Check appl event logs");
        }
        return;
    }

    // Prepare SSR
    memcpy( ((LRT_SSR *)out)->cmd, "SSR", 3 );
    out_lth = LRT_SSR_LTH;

    close_wrf( CL_SESSION );
    close_psbt( CL_SESSION );
    close_tsf( CL_SESSION );

    if (strncmp(((LRT_SSE *)(inbound))->opid, "000", 3)==0) {         // 5-7-2004 PAB
        // IF BLIND ACCESS FROM STORENET THEN SIMULATE A off COMMAND  // 5-7-4 pab
        // Deallocate handheld's table entry                          // 5-7-2004 PAB
        usrrc = dealloc_lrt_table( hh_unit );                         // 5-7-2004 PAB
    }

}

// ------------------------------------------------------------------------------------
//
// ISE - Information - Item Sales request
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_ISE_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE item_code[13];
} LRT_ISE;                                    // Item sales request
#define LRT_ISE_LTH sizeof(LRT_ISE)

void InfoItemSales(char* inbound) {


    ENQUIRY enqbuf;                                             // Streamline SDH 17-Sep-2008
    TIMEDATE now;                                               // Streamline SDH 17-Sep-2008
    URC usrrc;

    if (strncmp(((LRT_ISE *)(inbound))->opid, "000", 3) != 0) {      // 5-7-2004 PAB
        if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    }

    if (strncmp(((LRT_ISE *)(inbound))->opid, "000", 3)==0) {       // 5-7-2004 PAB
        hh_unit = 255;                                              // 20-7-2004 PAB
        // if this request has come from StoreNet USerID 000 then simulate
        // operator sign on processing - allocate hht table entry // 5-7-2004 PAB
        usrrc = alloc_lrt_table( hh_unit );                         // 5-7-2004 PAB
        if (usrrc<RC_IGNORE_ERR) {                                  // 5-7-2004 PAB
            prep_nak("ERRORUnable to allocate storage. Check appl event logs");// 5-7-2004 PAB
            return;                                                  // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB
        // Log current time                                         // 5-7-2004 PAB
        s_get( T_TD, 0L, (void *)&now, TIMESIZE );                  // 5-7-2004 PAB
        lrtp[hh_unit]->last_active_time = now.td_time;              // 5-7-2004 PAB
        memcpy( (BYTE *)&(lrtp[hh_unit]->txn), (BYTE *)inbound, 3); // 5-7-2004 PAB
        memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );        // 5-7-2004 PAB
                                                                    // 5-7-2004 PAB
        // Set state                                                // 5-7-2004 PAB
        lrtp[hh_unit]->state = ST_LOGGED_ON;                        // 5-7-2004 PAB
        // Reset misc counts (Unused at present)                    // 5-7-2004 PAB
        lrtp[hh_unit]->count1 = 0;                                  // 5-7-2004 PAB
        lrtp[hh_unit]->count2 = 0;                                  // 5-7-2004 PAB

        //Open IRF
        usrrc = IrfOpen();                                          // 5-7-2004 PAB
        if (usrrc<=RC_DATA_ERR) {                                   // 5-7-2004 PAB
            prep_nak("ERRORUnable to open EALITEMR file. Check appl event logs");// 5-7-2004 PAB
            return;                                                  // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB

        //Open IRFDEX
        usrrc = IrfdexOpen();                                       //SDH 15-01-2005 Promotions
        if (usrrc<=RC_DATA_ERR) {                                   //SDH 15-01-2005 Promotions
            prep_nak("ERRORUnable to open IRFDEX file. Check appl event logs");//SDH 15-01-2005 Promotions
            IrfClose( CL_SESSION );                                 //SDH 15-01-2005 Promotions
            return;                                                 //SDH 15-01-2005 Promotions
        }                                                           //SDH 15-01-2005 Promotions

        //Open IDF
        usrrc = IdfOpen();                                          // 5-7-2004 PAB
        if (usrrc<=RC_DATA_ERR) {                                   // 5-7-2004 PAB
            prep_nak("ERRORUnable to open IDF file. Check appl event logs");// 5-7-2004 PAB
            IrfClose( CL_SESSION );                                 // 5-7-2004 PAB
            IrfdexClose( CL_SESSION );                              // SDH 14-01-2005 Promtions
            return;                                                 // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB

        //Open ISF
        usrrc = IsfOpen();                                          // 5-7-2004 PAB
        if (usrrc<=RC_DATA_ERR) {                                   // 5-7-2004 PAB
            prep_nak("ERRORUnable to open ISF file. Check appl event logs");// 5-7-2004 PAB
            IdfClose( CL_SESSION );                                 // 5-7-2004 PAB
            IrfClose( CL_SESSION );                                 // 5-7-2004 PAB
            IrfdexClose( CL_SESSION );                              // SDH 14-01-2005 Promtions
            return;                                                 // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB

        //Open RFHIST
        usrrc = RfhistOpen();                                       // 5-7-2004 PAB
        if (usrrc<=RC_DATA_ERR) {                                   // 5-7-2004 PAB
            prep_nak("ERROR unable to open RFHIST file. Check appl event logs");// 5-7-2004 PAB
            IdfClose( CL_SESSION );                                 // 5-7-2004 PAB
            IrfClose( CL_SESSION );                                 // 5-7-2004 PAB
            IrfdexClose( CL_SESSION );                              // SDH 14-01-2005 Promtions
            IsfClose( CL_SESSION );                                 // 5-7-2004 PAB
            return;                                                 // 5-7-2004 PAB
        }                                                           // 5-7-2004 PAB

        //Open planner data (ignore errors)                         // SDH 12-Oct-2006 Planners
        SrpogOpen();                                                // SDH 12-Oct-2006 Planners
        SrmodOpen();                                                // SDH 12-Oct-2006 Planners
        SritmlOpen();                                               // SDH 12-Oct-2006 Planners
        SritmpOpen();                                               // SDH 12-Oct-2006 Planners
        SrpogifOpen();                                              // SDH 12-Oct-2006 Planners
        SrpogilOpen();                                              // SDH 12-Oct-2006 Planners
        SrpogipOpen();                                              // SDH 12-Oct-2006 Planners
        SrcatOpen();                                                // SDH 12-Oct-2006 Planners
        SrsxfOpen();                                                // SDH 12-Oct-2006 Planners

    }

    if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = stock_enquiry( SENQ_DESC,
                           ((LRT_ISE *)(inbound))->item_code,
                           &enqbuf );
    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc==RC_FILE_ERR) {
            prep_nak("ERROROne or more files are not open. Check appl event logs");
        } else if (usrrc==RC_DATA_ERR) {
            prep_nak("ERRORUnable to access data. Check appl event logs");
        } else {
            prep_nak("ERRORUndefined 11. Check appl event logs");
        }
        return;
    }

    if (usrrc==1) {
        // Prepare NAK
        prep_nak("  Item not on file");
        return;
    }

    memcpy( ((LRT_ISR *)out)->item_desc, enqbuf.item_desc , 20 );

    usrrc = sales_enquiry( enqbuf.boots_code,
                           (LONG)satol( enqbuf.item_price, 6 ),
                           (LRT_ISR *)&out );


    if (usrrc<RC_IGNORE_ERR) {
        prep_nak("ERRORUnable to action sales enquiry. Check appl event logs");
        return;
    }

    // IF BLIND ACCESS FROM STORENET THEN SIMULATE A off COMMAND// 5-7-4 pab
    if (strncmp(((LRT_ISE *)(inbound))->opid, "000", 3)==0) {       // 5-7-2004 PAB

        RfhistClose( CL_SESSION );                                 // 5-7-2004 PAB
        IsfClose( CL_SESSION );                                    // 5-7-2004 PAB
        IdfClose( CL_SESSION );                                    // 5-7-2004 PAB
        IrfClose( CL_SESSION );                                    // 5-7-2004 PAB
        IrfdexClose( CL_SESSION );                                 // SDH 14-01-2005 Promtions
        SrpogClose(CL_SESSION);                                    // SDH 12-Oct-2006 Planners
        SrmodClose(CL_SESSION);                                    // SDH 12-Oct-2006 Planners
        SritmlClose(CL_SESSION);                                   // SDH 12-Oct-2006 Planners
        SritmpClose(CL_SESSION);                                   // SDH 12-Oct-2006 Planners
        SrpogifClose(CL_SESSION);                                  // SDH 12-Oct-2006 Planners
        SrpogilClose(CL_SESSION);                                  // SDH 12-Oct-2006 Planners
        SrpogipClose(CL_SESSION);                                  // SDH 12-Oct-2006 Planners
        SrcatClose(CL_SESSION);                                    // SDH 12-Oct-2006 Planners
        SrsxfClose(CL_SESSION);                                    // SDH 12-Oct-2006 Planners

        // Deallocate handheld's table entry                        // 5-7-2004 PAB
        usrrc = dealloc_lrt_table( hh_unit );                       // 5-7-2004 PAB
    }

    // Prepare ISR
    memcpy( ((LRT_ISR *)out)->cmd, "ISR", 3 );
    out_lth = LRT_ISR_LTH;

}

// ------------------------------------------------------------------------------------
//
// RPO - Report Sign On
//
//
//
// ------------------------------------------------------------------------------------

void ReportSignOn(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM

    // verify that batch suite is not running
    if (IsReportMntActive()) return;                                 // 4-11-04 PAB

    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = open_rfrdesc();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open "
                  "RFRDESC file. "
                  "Check appl event logs" );
        return;
    }

    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to "
                      "create workfile. "
                      "Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        return;
    }

    // Prepare for reporting
    lrtp[hh_unit]->fnum3 = 0L;                                      // current report file fnum
    if (debug) {
        sprintf(msg, "Allocating report buffer");
        disp_msg(msg);
    }
    usrrc = alloc_report_buffer( (RBUF **)&lrtp[hh_unit]->rbufp );
    if (usrrc<RC_IGNORE_ERR) {
        prep_nak("ERRORUnable to "
                  "allocate storage. "
                  "Check appl event logs" );
        lrtp[hh_unit]->rbufp = NULL;
        return;
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// RLE - Report - Get Report
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE seq[REPORT_SEQ_SIZE];
} LRT_RLE;                                    // Reports request
#define LRT_RLE_LTH sizeof(LRT_RLE)

typedef struct {
   BYTE cmd[3];
} LRT_RLF;                                    // Report - no more data
#define LRT_RLF_LTH sizeof(LRT_RLF)

void ReportGetReport(char* inbound) {

    URC usrrc;

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM

    // verify that batch suite is not running
    if (IsReportMntActive()) return;                                 // 4-11-04 PAB

    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    if (lrtp[hh_unit]->rbufp == NULL) {
        prep_nak("ERRORNo report buffer allocated. Check appl event logs");
        return;
    }

    // Get next record from RFRDESC
    usrrc = rfrdesc_get_next( /*hh_unit,*/
                              (BYTE *)((LRT_RLE *)(inbound))->seq,
                              (LRT_RLR *)out );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to read from RFRDESC. "
                      "Check appl event logs" );
        } else {
            prep_nak("ERRORUndefined 12. Check appl event logs");
        }
        return;
    }

    if (usrrc==1) {
        // Record available - Prepare RLR
        memcpy( ((LRT_RLR *)out)->cmd, "RLR", 3 );
        out_lth = LRT_RLR_LTH;
    } else {
        // Record not available - Prepare RLF
        memcpy( ((LRT_RLF *)out)->cmd, "RLF", 3 );
        out_lth = LRT_RLF_LTH;
    }

}

// ------------------------------------------------------------------------------------
//
// RLS - Report - Get Top Level of report
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_RLS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE fname[12];
   BYTE seq[REPORT_SEQ_SIZE];
} LRT_RLS;                                    // Report header request
#define LRT_RLS_LTH sizeof(LRT_RLS)

void ReportGetTopLevel(char* inbound) {

    BYTE tbuf[32];
    LONG rc2;
    WORD rec_cnt;
    URC usrrc;

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM

    // verify that batch suite is not running
    if (IsReportMntActive()) return;                                 // 4-11-04 PAB

    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    if (lrtp[hh_unit]->rbufp == NULL) {
        prep_nak("ERRORNo report buffer allocated. "
                  "Check appl event logs" );
        return;
    }

    if (satoi(((LRT_RLS *)(inbound))->seq, REPORT_SEQ_SIZE)==0) {
        memcpy( sbuf, ((LRT_RLS *)(inbound))->fname, 12 );
        *(sbuf+12) = 0x00;
        sprintf( tbuf, "RFREP:%12s", sbuf );
        for (WORD i=17;i>0;i--) {
            if (*(tbuf+i)==0x20) {
                *(tbuf+i)=0x00;
            } else {
                break;
            }
        }

        // Open file
        if (lrtp[hh_unit]->fnum3!=0L) {
            s_close( 0, lrtp[hh_unit]->fnum3 );
            log_file_close ( lrtp[hh_unit]->fnum3 );                // 11-11-04 PAB
            lrtp[hh_unit]->fnum3 = 0L;
        }
        rc2 = s_open( RFREP_OFLAGS, tbuf );
        if (rc2<=0) {
            log_event101(rc2, RFREP_REP, __LINE__);
            memcpy( ((LRT_NAK *)out)->cmd, "NAK", 3 );
            sprintf( ((LRT_NAK *)out)->msg,
                     "No report "                                   // PAB 29-Nov-2006
                     "available. "                                  // PAB 29-Nov-2006
                     "", tbuf );                                    // PAB 29-Nov-2006
            out_lth = LRT_NAK_LTH;
            return;
        }
        // save fnum
        lrtp[hh_unit]->fnum3 = rc2;                                 // 11-11-04 PAB
        log_file_open ( rc2,'R' );
    }

    // if seq is zero then this is a new report select. clear down the old buffer and
    // reallocate a new empty one so when the selected report is empty, and this is
    // not the first report we have browsed the cache is certain to be empty.
    // and we do not send wrong report data.

    if (strncmp(((LRT_RLS *)(inbound))->seq, "0000",4) == 0) {      // 09-11-04 PAB
        // Deallocate report buffer                                 // 09-11-04 PAB
        usrrc = dealloc_report_buffer( lrtp[hh_unit]->rbufp );      // 09-11-04 PAB
        lrtp[hh_unit]->rbufp = (void *)NULL;                        // 09-11-04 PAB
        if (usrrc<RC_IGNORE_ERR) {                                  // 09-11-04 PAB
            prep_nak("ERRORUnable to deallocate rpt buffer. "       // 09-11-04 PAB
                      "Check appl event logs" );                    // 09-11-04 PAB
            return;                                                  // 09-11-04 PAB
        }                                                           // 09-11-04 PAB
        usrrc = alloc_report_buffer( (RBUF **)&lrtp[hh_unit]->rbufp );    // 09-11-04 PAB
        if (usrrc<RC_IGNORE_ERR) {                                  // 09-11-04 PAB
            prep_nak("ERRORUnable to allocate storage. "            // 09-11-04 PAB
                      "Check appl event logs" );                    // 09-11-04 PAB
            lrtp[hh_unit]->rbufp = NULL;                            // 09-11-04 PAB
            return;                                                  // 09-11-04 PAB
        }                                                           // 09-11-04 PAB
    }                                                               // 09-11-04 PAB

    usrrc = rfrep_get_next_lev0( hh_unit,
                                 ((LRT_RLS *)(inbound))->seq,
                                 (LRT_RLD *)out,
                                 (WORD *)&rec_cnt );
    if (usrrc<RC_IGNORE_ERR) {
        memcpy( tbuf, ((LRT_RLS *)(inbound))->fname, 12 );
        *(tbuf+12) = 0x00;
        if (usrrc == RC_DATA_ERR) {
            memcpy( ((LRT_NAK *)out)->cmd, "NAK", 3 );
            sprintf( ((LRT_NAK *)out)->msg,
                     "ERRORUnable to "
                     "read %-12s   0"
                     "Check appl event logs"
                     "*****************", tbuf );
            out_lth = LRT_NAK_LTH;
        } else {
            prep_nak("ERRORUndefined 13. "
                      "Check appl event logs" );
        }
        return;
    }

    if (usrrc==1) {
        // Prepare RLD
        memcpy( ((LRT_RLD *)out)->cmd, "RLD", 3 );
        sprintf( sbuf , "%03d", rec_cnt );
        memcpy( ((LRT_RLD *)out)->rpt , sbuf, 3 );
        out_lth = RLD_REP_OFFSET + (rec_cnt * LRT_RLD_REP_LTH);
    } else {
        // Prepare RLF
        memcpy( ((LRT_RLF *)out)->cmd, "RLF", 3 );
        out_lth = LRT_RLF_LTH;
    }

}

// ------------------------------------------------------------------------------------
//
// RPS - Report - Get Specific Level of report
//
//
//
// ------------------------------------------------------------------------------------

void ReportGetSpecificLevel(char* inbound) {

    BYTE tbuf[32];
    URC usrrc;
    WORD rec_cnt;

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM

    // verify that RFMAINT is not running
    if (IsReportMntActive()) return;                                 // 4-11-04 PAB

    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = rfrep_get_next( hh_unit,
                            ((LRT_RLS *)(inbound))->seq,
                            (LRT_RUP *)out,
                            (WORD *)&rec_cnt );
    if (usrrc<RC_IGNORE_ERR) {
        memcpy( tbuf, ((LRT_RLS *)(inbound))->fname, 12 );
        *(tbuf+12) = 0x00;
        if (usrrc == RC_DATA_ERR) {
            memcpy( ((LRT_NAK *)out)->cmd, "NAK", 3 );
            sprintf( ((LRT_NAK *)out)->msg,
                     "ERRORUnable to read %-12s   +"
                     "Check appl event logs"
                     "*****************", tbuf );
            out_lth = LRT_NAK_LTH;
        } else {
            prep_nak("ERRORUndefined 14. Check appl event logs" );
        }
        return;
    }

    if (usrrc==1) {
        // Prepare RUP
        memcpy( ((LRT_RUP *)out)->cmd, "RUP", 3 );
        sprintf( sbuf , "%03d", rec_cnt );
        memcpy( ((LRT_RUP *)out)->rpt , sbuf, 3 );
        // RBS RFHHT.EXE expects the whole structure to be sent
        // regardless of if it is only partially populated TH  28/10/97
        // out_lth = RUP_REP_OFFSET + (rec_cnt * LRT_RUP_REP_LTH);
        out_lth = sizeof ( LRT_RUP ) ;
    } else {
        // Prepare RLF
        memcpy( ((LRT_RLF *)out)->cmd, "RLF", 3 );
        out_lth = LRT_RLF_LTH;
    }

}

// ------------------------------------------------------------------------------------
//
// RPX - Report - Sign Off
//
//
//
// ------------------------------------------------------------------------------------

void ReportSignOff(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    //Do both just in case                                          // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    // Close report file if it's open
    if (lrtp[hh_unit]->fnum3!=0) {
        s_close( 0, lrtp[hh_unit]->fnum3);
        log_file_close ( lrtp[hh_unit]->fnum3 );                    // 11-11-04 PAB
    }
    lrtp[hh_unit]->fnum3 = 0L;


    close_rfrdesc( CL_SESSION );

    // Deallocate report buffer
    usrrc = dealloc_report_buffer( lrtp[hh_unit]->rbufp );
    lrtp[hh_unit]->rbufp = (void *)NULL;
    if (usrrc<RC_IGNORE_ERR) {
        prep_nak("ERRORUnable to deallocate rpt buffer. "
                  "Check appl event logs" );
        return;
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}

// ------------------------------------------------------------------------------------
//
// CLO - Count Lists - Sign On
//
//
//
// ------------------------------------------------------------------------------------

void CountsSignOn(char* inbound) {

    URC usrrc;

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                     // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    usrrc = ClolfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open CLOLF file. "
                  "Check appl event logs" );
        return;
    }
    usrrc = ClilfOpen();
    if (usrrc<=RC_DATA_ERR) {
        prep_nak("ERRORUnable to open CLILF file. "
                  "Check appl event logs" );
        return;
    }

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

    // v4.0 START
    usrrc = prepare_workfile( hh_unit, SYS_LAB );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to "
                      "create workfile. "
                      "Check appl event logs" );
        } else {
            prep_pq_full_nak();                                     // 22-12-04 SDH
        }
        return;
    }
    // v4.0 END

}

// ------------------------------------------------------------------------------------
//
// CLR - Count Lists - Get List of Lists
//
//       Response: CLL
//
// ------------------------------------------------------------------------------------

/*
typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
} LRT_CLO;                                    // Counting - List Signon
#define LRT_CLO_LTH sizeof(LRT_CLO)
*/

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
} LRT_CLR;                                    // Counting - List Request
#define LRT_CLR_LTH sizeof(LRT_CLR)

typedef struct {
   BYTE cmd[3];
   BYTE list_id[3];
} LRT_CLE;                                    // Counting - List End
#define LRT_CLE_LTH sizeof(LRT_CLE)

void CountsGetListOfLists(char* inbound) {

    URC usrrc;

    if (IsHandheldUnknown()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    // Get next record from CLOLF
    usrrc = clolf_get_next( /*hh_unit,*/
                            ((LRT_CLR *)(inbound))->list_id,
                            (LRT_CLL *)out );
    if (usrrc<RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORI/O error. "
                      "Check appl event logs" );
        } else {
            prep_nak("ERRORUndefined 6. "
                      "Check appl event logs" );
        }
        return;
    }
    if (usrrc==1) {
        // Record available - Prepare CLL
        memcpy( ((LRT_CLL *)out)->cmd, "CLL", 3 );
        out_lth = LRT_CLL_LTH;

        // Audit                                                                 //CSk 12-03-2012 SFA
//xxx        memcpy( ((LRTLG_CLC *)dtls)->list_id, (LRT_CLL *)out->abListID      ,3); //CSk 12-03-2012 SFA
//xxx        memcpy( ((LRTLG_CLC *)dtls)->num_items, (LRT_CLL *)out->abTotalItems,3); //CSk 12-03-2012 SFA
//xxx        ((LRTLG_CLC *)dtls)->list_type = (LRT_CLL *)out->bListType;              //CSk 12-03-2012 SFA
        lrt_log( LOG_CLL, hh_unit, dtls);                                        //CSk 12-03-2012 SFA
        // END                                                                   //CSk 12-03-2012 SFA

    } else {
        // Record not available - Prepare CLE
        memcpy( ((LRT_CLE *)out)->cmd, "CLE", 3 );
        memcpy( ((LRT_CLE *)out)->list_id,
                ((LRT_CLR *)(inbound))->list_id, 3);                // v2.3
        out_lth = LRT_CLE_LTH;
    }

}

// ------------------------------------------------------------------------------------
//
// CLS - Count Lists - Get List
//
//
//
// ------------------------------------------------------------------------------------

typedef struct LRT_CLS_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE seq[3];   // xxxx Addition to spec - need to check out !!!!!!!!!!!!!!!!
} LRT_CLS;                                    // Counting - List Select
#define LRT_CLS_LTH sizeof(LRT_CLS)

void CountsGetList(char* inbound) {

    //Set up specific view of input
    LRT_CLS* pCLS = (LRT_CLS*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LONG list_id = satol(pCLS->list_id, 3);
    LONG rc2;
    LONG sec, day, month, year;                                  //TAT 11-09-2012 SFA
    WORD hour, min;                                              //TAT 11-09-2012 SFA
    URC usrrc;

    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM

    pack((BYTE *)&(lrtp[hh_unit]->unique), 2, pCLS->seq, 3, 1); // SDH 26-11-04 CREDIT CLAIM

    // Read clolf record
    rc2 = ClolfRead((list_id - 1L), __LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read from CLOLF. "
                  "Check appl event logs" );
        return;
    }

//xxx    // if the list has been submitted to RFSBG or completed by RFSBG// 20-5-4 PAB
//xxx    // then block updates                                           // 20-5-4 PAB
//xxx    // V4.01 PAB                                                    // 20-5-4 PAB
//xxx    if ((clolfrec.bListStatus == 'C') ||                        // 10-3-2005 SDH OSSR WAN //CSk 12-03-2012 SFA
//xxx        (clolfrec.bListStatus == 'P')) {                        // 20-5-4 PAB             //CSk 12-03-2012 SFA
//xxx        prep_nak("This count List has already been closed for today");// 20-5-4 PAB
//xxx        return;                                                  // 20-5-4 PAB
//xxx    }                                                           // 20-5-4 PAB
//xxx    // end block update to closed count lists    PAB                // 20-5-4 PAB

    //Prevent anyone except the person who was last in the list     // SDH 10-1-2005 OSSR WAN
    //from accessing it, if it is active.                           // SDH 10-1-2005 OSSR WAN
    if (clolfrec.bListStatus == 'A') {                          // SDH 10-1-2005 OSSR WAN //CSk 12-03-2012 SFA
        WORD wLastUser = satoi(clolfrec.abPickerId, 3);         // SDH 10-1-2005 OSSR WAN //CSk 12-03-2012 SFA
        WORD wThisUser = satoi(pCLS->opid, 3);                  // SDH 10-1-2005 OSSR WAN
        if ((wLastUser != wThisUser) && (wLastUser != 0)) {     // SDH 10-1-2005 OSSR WAN
            sprintf(sbuf, "List already active.\nOnly user ID " // SDH 10-1-2005 OSSR WAN
                    "%.3d can open this list.", wLastUser);     // SDH 10-1-2005 OSSR WAN
            prep_nak(sbuf);                                     //SDH 23-Aug-2006 Planners
            return;                                              // SDH 10-1-2005 OSSR WAN
        }                                                       // SDH 10-1-2005 OSSR WAN
    }                                                           // SDH 10-1-2005 OSSR WAN

    // Update CLOLF status for 1st count sent                       // SDH 10-1-2005 OSSR WAN
    //Also update the user ID                                       // SDH 10-1-2005 OSSR WAN
//    if (clolfrec.bListStatus != 'A') {                       // 20-5-4 PAB //CSk 12-03-2012 SFA
    if (clolfrec.bListStatus != 'A' &&                       //TAT 24-04-2013 SFA
        clolfrec.bListStatus != 'C') {                       //TAT 24-04-2013 SFA
        clolfrec.bListStatus = 'A';                          // Status : Active // 20-5-4 PAB //CSk 12-03-2012 SFA
        memcpy(clolfrec.abPickerId, pCLS->opid,              // SDH 10-1-2005 OSSR WAN     //CSk 12-03-2012 SFA
               sizeof(clolfrec.abPickerId));                 // SDH 10-1-2005 OSSR WAN     //CSk 12-03-2012 SFA
        sysdate( &day, &month, &year, &hour, &min, &sec );            //TAT 11-09-2012 SFA
        sprintf( sbuf, "%02d%02d", hour, min );                       //TAT 11-09-2012 SFA
        pack( clolfrec.abPickStartTime, 2, sbuf, 4, 0 ); // 2-PD HHMM //TAT 11-09-2012 SFA
        rc2 = ClolfWrite(list_id - 1L, __LINE__);
        if (rc2 <= 0) {
            prep_nak("ERRORUnable to write to CLOLF. Check appl event logs");
            return;
        }
    }

    // Get next record from CLILF
    usrrc = clilf_get_next( /*hh_unit,*/
                            pCLS->list_id,
                            pCLS->seq,
                            (LRT_CLI *)out );
    sprintf(msg, "clilf_get_next() rc=%d", usrrc);
    disp_msg(msg);
    if (usrrc < RC_IGNORE_ERR) {
        if (usrrc == RC_DATA_ERR) {
            prep_nak("ERRORUnable to read from CLILF. "
                      "Check appl event logs" );
        } else {
            prep_nak("ERRORUndefined 7. Check appl event logs" );
        }
        return;
    }

    if (usrrc==1) {
        // v4.0 START
        // Audit
        sprintf( msg, "%04ld", list_id );
        memcpy( ((LRTLG_CLS *)dtls)->list_id, msg , 4 );
        memset( ((LRTLG_CLS *)dtls)->resv, 0x00, 8 );
        lrt_log( LOG_CLS, hh_unit, dtls );
        // v4.0 END
        // Record available - Prepare CLI
        memcpy( ((LRT_CLI *)out)->cmd, "CLI", 3 );
        // Only return records found & not all 8!                                 //CSk 12-03-2012 SFA
        //out_lth = LRT_CLI_LTH;                                                  //CSk 12-03-2012 SFA
        out_lth = LRT_CLI_HDRSIZE +                                               //CSk 12-03-2012 SFA
                  ( sizeof(CLI_ITEM_REC) *                                        //CSk 12-03-2012 SFA
                    satoi(((LRT_CLI *)out)->abNumItemsInList, 3)                  //CSk 12-03-2012 SFA
                  );                                                              //CSk 12-03-2012 SFA

    } else {
        // Record not available - Prepare CLE
        memcpy( ((LRT_CLE *)out)->cmd, "CLE", 3 );
        out_lth = LRT_CLE_LTH;
    }

}



// ------------------------------------------------------------------------------------
//
// CLA - Create User Generated Count List Request
//
// CLB - Create User Generated Count List Acknowledgement
//
//
// ------------------------------------------------------------------------------------
typedef struct {                                                                 //CSk 12-03-2012 SFA
   BYTE abCmd[3];                                                                //CSk 12-03-2012 SFA
   BYTE abOpid[3];                                                               //CSk 12-03-2012 SFA
   BYTE abListId[3];     // ignored if bListStatus is 'U'                        //CSk 12-03-2012 SFA
   BYTE bListStatus;     // Status of User generated list:                       //CSk 12-03-2012 SFA
                         // 'S' start user gen cnt list process                  //CSk 12-03-2012 SFA
                         // 'X' end user gen cnt list process for abListId       //CSk 12-03-2012 SFA
} LRT_CLA;   // Stock File Accuracy - Count List Create                          //CSk 12-03-2012 SFA
#define LRT_CLA_LTH sizeof(LRT_CLA)                                              //CSk 12-03-2012 SFA

typedef struct {                                                                 //CSk 12-03-2012 SFA
   BYTE abCmd[3];                                                                //CSk 12-03-2012 SFA
   BYTE abListId[3];                                                             //CSk 12-03-2012 SFA
} LRT_CLB;   // Stock File Accuracy - Count List Acknowledgement                 //CSk 12-03-2012 SFA
#define LRT_CLB_LTH sizeof(LRT_CLB)                                              //CSk 12-03-2012 SFA
                                                                                 //CSk 12-03-2012 SFA

void CreateCountList(char* inbound) {                                            //CSk 12-03-2012 SFA
                                                                                 //CSk 12-03-2012 SFA
    B_DATE nowDate;
    B_TIME nowTime;
    LONG rc2;
    LONG list_id;
    LONG lFileSize;
    LONG lNextListId;
    BYTE abFileName[32];

    //Input and output views
    LRT_CLA* pCLA = (LRT_CLA*)inbound;                                           //CSk 12-03-2012 SFA
    LRT_CLB* pCLB = (LRT_CLB*)out;

    //Initial checks
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();

    if (pCLA->bListStatus == 'S') {  // Create new list
        // Work out next List No. to use by getting file length
        // & divide by record size. Return List Id in CLB txn
        strncpy(abFileName, clolf.pbFileName, sizeof(abFileName));
        abFileName[sizeof(abFileName) - 1] = 0x00;
        lFileSize = filesize (abFileName);
        lNextListId = (lFileSize / clolf.wRecLth) + 1;

        // Create new CLOLF record - initialise WHOLE record first
        // (more efficient than initialising individual fields).
        memset(&clolfrec, 0x00, sizeof(clolfrec));

        sprintf(clolfrec.abListId, "%03ld", lNextListId);
        memcpy(clolfrec.abCreatorId, pCLA->abOpid, 3);
        clolfrec.bListType = 'U';   // U - User generated count list
        clolfrec.bBusCentLet = ' '; // Only set for HO Lists
        memset(clolfrec.abListName, 0x20, sizeof(clolfrec.abListName));
        memcpy(clolfrec.abListName, "User Id", 7);
        memcpy(clolfrec.abListName+8, pCLA->abOpid, 3);
        memcpy(clolfrec.abPickerId, "000", 3);
        clolfrec.bListStatus = ' '; // space - list creation in progress
        memcpy(clolfrec.abPILSTID, "0000", 4);

        //Get today's date as a Julian
        GetSystemDate(&nowTime, &nowDate);

        sprintf( sbuf, "%02d%02d%02d", nowDate.wYear%100, nowDate.wMonth, nowDate.wDay);
        pack(clolfrec.abCreateDate, 3, sbuf, 6, 0);  // 3-PD YYMMDD

        sprintf(sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);
        pack(clolfrec.abCreateTime, 2, sbuf, 4, 0);  // 2-PD HHMM

        //clolfrec.abExpiryDate,   3);  // 3-PD YYMMDD   // Include for clarity
        //clolfrec.abPickStartTime,2);  // 2-PD HHMM     // as all set to 0x00
        //clolfrec.abPickEndTime,  2);  // 2-PD HHMM     // by memset earlier.

        clolfrec.bCurrentLocation = ' ';    // Location assigned at time of count

        //clolfrec.uiTotalItems       = 0;  // Include for clarity
        //clolfrec.uiOutSalesFloorCnt = 0;  // as all set to 0x00
        //clolfrec.uiOutBackShopCnt   = 0;  // by memset earlier.
        //clolfrec.uiOutOSSRCnt       = 0;  //

        rc2 = ClolfWrite(lNextListId-1, __LINE__);
        if (rc2 <= 0L) {
            prep_nak("ERRORUnable to write to CLOLF. "
                      "Check appl event logs" );
            return;
        }
        //----------------------
        //Build the CLB response
        //----------------------
        memcpy(pCLB->abCmd, "CLB", 3);
        sprintf(pCLB->abListId, "%03ld", lNextListId);
        out_lth = LRT_CLB_LTH;
        return;

    } else if (pCLA->bListStatus == 'X') { //End create list process
        list_id = satol(pCLA->abListId, 3);
        // Read clolf record
        rc2 = ClolfRead((list_id - 1L), __LINE__);
        if (rc2 <= 0L) {
            prep_nak("ERRORUnable to read from CLOLF. "
                      "Check appl event logs" );
            return;
        }
        clolfrec.bListStatus = 'I'; // I - Initial List ready to count

        rc2 = ClolfWrite((list_id - 1L), __LINE__);
        if (rc2 <= 0L) {
            prep_nak("ERRORUnable to write to CLOLF. "
                      "Check appl event logs" );
            return;
        }

    } else {
        prep_nak("ERRORInvalid List Status passed. "
                  "Check appl event logs" );
        return;
    }
    prep_ack("");
}



// ------------------------------------------------------------------------------------
//
// CLC - Count Lists - Update Item
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE list_id[3];
   BYTE seq[3];
   BYTE boots_code[7];
   BYTE bLocation;       // 'O' Off-Site Stock Room                            //CSk 12-03-2012 SFA
                         // 'S' Sales Floor                                    //CSk 12-03-2012 SFA
                         // 'B' Back Shop/Auto Fast Fill List                  //CSk 12-03-2012 SFA
                         // 'P' Pending sales plan back shop                   //CSk 12-03-2012 SFA
                         // 'Q' Pending sales plan OSSR                        //CSk 12-03-2012 SFA
   BYTE abCount[4];                                                            //CSk 12-03-2012 SFA
   BYTE cUpdateOssrItem; // ' ' no change
                         // 'O' OSSR item
                         // 'N' Non-OSSR item
   BYTE abSalesFig[9];   // Current Sales figure POD: 000000000 - 999999999    //CSk 12-03-2012 SFA
                         //                       RF: XXXXXXXXX                //CSk 12-03-2012 SFA
} LRT_CLC;               // Counting - List Item Confirm
#define LRT_CLC_LTH sizeof(LRT_CLC)


void CountsUpdateItem(char* inbound) {

    IMSTC_REC imstcrec = {0};

    TIMEDATE now;                                               // Streamline SDH 17-Sep-2008
    LRT_CLC* pCLC = (LRT_CLC*)inbound;                          // SDH 26-11-04 CREDIT CLAIM
    LONG list_id = satol(pCLC->list_id, 3);
    LONG rc2;
    WORD hour, min;                                             // Streamline SDH 17-Sep-2008
    URC usrrc;

    if (IsHandheldUnknown()) return;                             // SDH 26-11-04 CREDIT CLAIM
    if (IsStoreClosed()) return;                                 // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    //If we're in an OSSR WAN store then update the OSSR flag   // SDH 17-11-04 OSSR WAN
    //if required                                               // SDH 17-11-04 OSSR WAN
    if ((rfscfrec1and2.ossr_store == 'W') &&                    // SDH 17-11-04 OSSR WAN
        (pCLC->cUpdateOssrItem != ' ')) {                       // SDH 17-11-04 OSSR WAN
        usrrc = ProcessRfhist(pCLC->boots_code,                 // SDH 17-11-04 OSSR WAN
                              pCLC->cUpdateOssrItem, __LINE__); // SDH 17-11-04 OSSR WAN
        prep_nak("ERRORUnable to update OSSR flag. "            // SDH 17-11-04 OSSR WAN
                  "Check appl event logs" );                    // SDH 17-11-04 OSSR WAN
        return;                                                 // SDH 17-11-04 OSSR WAN
    }                                                           // SDH 17-11-04 OSSR WAN

    // Read CLOLF for list header details
    rc2 = ClolfRead(list_id - 1L, __LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read from CLOLF. Check appl event logs");
        return;
    }

    // Open and read current IMSTC sales figure
    usrrc = open_imstc();
    if (usrrc!=RC_OK) {
        prep_nak("ERRORUnable to open IMSTC file. "
                  "Check appl event logs" );
        return;
    }
    memset(imstcrec.bar_code, 0x00, 11);                        // SDH 26-11-04 CREDIT CLAIM
    pack(imstcrec.bar_code+8, 3, pCLC->boots_code, 6, 0);       // SDH 26-11-04 CREDIT CLAIM
    disp_msg ( "RD IMSTC : " );
    dump ( imstcrec.bar_code, (WORD)IMSTC_KEYL );
    rc2 = s_read( 0, imstc.fnum, (void *)&imstcrec,
                  IMSTC_RECL, IMSTC_KEYL );
    if (rc2<=0L) {
        if ((rc2&0xFFFF)==0x06C8 || (rc2&0xFFFF)==0x06CD) {
            imstc.present=FALSE;
            disp_msg("NOF");
        } else {
            log_event101(rc2, IMSTC_REP, __LINE__);
            sprintf(msg, "Err-R IMSTC. RC:%08lX", rc2);
            disp_msg(msg);
        }
    } else {
        imstc.present=TRUE;
        disp_msg("OK");
    }
    close_imstc( CL_SESSION );

    // Read CLILF item record WITH A LOCK
    // Function waits for 250ms for record to be freed
    memcpy(clilfrec.abListId, pCLC->list_id, 3);                // SDH 26-11-04 CREDIT CLAIM //CSk 12-03-2012 SFA
    memcpy(clilfrec.abSeq, pCLC->seq, 3);                       // SDH 26-11-04 CREDIT CLAIM //CSk 12-03-2012 SFA
    rc2 = ClilfReadLock(__LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read (lk) from CLILF. "
                  "Check appl event logs" );
        return;
    }

    // Check we are dealing with the same boots code
    unpack( sbuf, 7, clilfrec.abItemCode, 4, 1 );               //CSk 12-03-2012 SFA
    if (memcmp(pCLC->boots_code, sbuf, 7)==0) {                 // SDH 26-11-04 CREDIT CLAIM //CSk 12-03-2012 SFA

        //Format current time                                   // 01-9-04 PAB OSSR
        BYTE abTimePD[2];                                       // 10-1-2005 SDH OSSR WAN
        s_get( T_TD, 0L, (void *)&now, TIMESIZE);               // 31-8-04 PAB OSSR
        min = now.td_time / 60000;                              // 31-8-04 PAB OSSR
        hour = min / 60;                                        // 31-8-04 PAB OSSR
        min %= 60;                                              // 31-8-04 PAB OSSR
        sprintf(sbuf, "%02d%02d", hour, min);                   // 10-1-2005 SDH OSSR WAN
        pack(abTimePD, 2, sbuf, 4,0 );                          // 10-1-2005 SDH OSSR WAN

        if(clilfrec.wSalesFloorCount < 0 &&                                      // TAT 31-10-2012 SFA
           clilfrec.wMainBSCount < 0 &&                                          // TAT 31-10-2012 SFA
           clilfrec.wOSSRCount < 0 &&                                            // TAT 31-10-2012 SFA
           clilfrec.wPendingBSCount < 0 &&                                       // TAT 31-10-2012 SFA
           clilfrec.wPendingOSSRCount < 0) {                                     // TAT 31-10-2012 SFA
            //This is the first count for the item                               // TAT 31-10-2012 SFA
            //Format current sales (use 0 if no IMSTC)                           // TAT 31-10-2012 SFA
            clilfrec.wLastSalesCount = (imstc.present ? imstcrec.numitems:0)     // TAT 31-10-2012 SFA
                                        / 100L;                                  // TAT 31-10-2012 SFA
            clilfrec.bCountStatus = 'P';                                         // TAT 31-10-2012 SFA
        }                                                                        // TAT 31-10-2012 SFA

        switch (pCLC->bLocation) {
            case 'S':  // Sales Floor
                clilfrec.wSalesFloorCount = satoi(pCLC->abCount, 4);                //CSk 12-03-2012 SFA  TAT 31-10-2012 SFA
                break;
            case 'B':  // Back Shop/Auto Fast Fill List
                if (clilfrec.wMainBSCount < 0) {                                    //TAT 03-12-2012 SFA
                    clolfrec.uiOutBackShopCnt--;                                    //TAT 03-12-2012 SFA
                    clolfrec.uiOutOSSRCnt--;                                        //TAT 03-12-2012 SFA
                }                                                                   //TAT 03-12-2012 SFA
                clilfrec.wMainBSCount = satoi(pCLC->abCount, 4);                    //CSk 12-03-2012 SFA  TAT 31-10-2012 SFA
                break;
            case 'O':  // Off-Site Stock Room
                if (clilfrec.wOSSRCount < 0) {                                      //TAT 03-12-2012 SFA
                    clolfrec.uiOutBackShopCnt--;                                    //TAT 03-12-2012 SFA
                    clolfrec.uiOutOSSRCnt--;                                        //TAT 03-12-2012 SFA
                }                                                                   //TAT 03-12-2012 SFA
                clilfrec.wOSSRCount = satoi(pCLC->abCount, 4);                      //CSk 12-03-2012 SFA  TAT 31-10-2012 SFA
                break;
            case 'P':  // Pending sales plan back shop
                clilfrec.wPendingBSCount = satoi(pCLC->abCount, 4);                 //CSk 12-03-2012 SFA  TAT 31-10-2012 SFA
                break;
            case 'Q':  // Pending sales plan OSSR
                clilfrec.wPendingOSSRCount = satoi(pCLC->abCount, 4);               //CSk 12-03-2012 SFA  TAT 31-10-2012 SFA
                break;
            default:  // Invalid location
                break;
        }

        if (debug) {

            sprintf(msg, "1 - Shop Floor Count      : %d", clilfrec.wSalesFloorCount);
            disp_msg(msg);
            sprintf(msg, "2 - Main Back Shop Count  : %d", clilfrec.wMainBSCount);
            disp_msg(msg);
            sprintf(msg, "3 - OSSR Count            : %d", clilfrec.wOSSRCount);
            disp_msg(msg);
            sprintf(msg, "4 - Pending Back Shop Cnt : %d", clilfrec.wPendingBSCount);
            disp_msg(msg);
            sprintf(msg, "5 - Pending sales OSSR Cnt: %d", clilfrec.wPendingOSSRCount);
            disp_msg(msg);
            sprintf(msg, "6 - Sales at Shop Count   : %d", clilfrec.wLastSalesCount);
            disp_msg(msg);
        }
    }  else {
        prep_nak("ERRORCLILF boots code has changed. Contact service desk" );
        return;
    }

    // Update CLILF item record
    rc2 = ClilfWriteUnlock(__LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to update CLILF. Check appl event logs" );
        return;
    }

    // Update CLOLF list header
    rc2 = ClolfWrite(list_id - 1L, __LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to write to CLOLF. Check appl event logs");
        return;
    }

    // v4.0 START
    // Audit
    sprintf( msg, "%04ld", list_id );
    memcpy( ((LRTLG_CLC *)dtls)->list_id, msg , 4 );
    memcpy( ((LRTLG_CLC *)dtls)->boots_code,
            ((LRT_CLC *)(inbound))->boots_code, 7 );
    memset( ((LRTLG_CLC *)dtls)->resv, 0x00, 1 );
    lrt_log( LOG_CLC, hh_unit, dtls);                           // SDH 10-01-05 OSSR WAN
    // v4.0 END

    prep_ack("");

}

//xxx        // variance = items_sold_today - sales_figure
//xxx        variance = satoi(abSales, 4) - clilfrec.uiSalesFloorCount;
//xxx        new_count = wShelfCount + wBackCount + wOssrCount - variance;
//xxx        mismatch_qty = abs(lStockFigure - new_count);
//xxx        mismatch_val = abs(mismatch_qty * satol( enqbuf.item_price, 6 ));
//xxx
//xxx        if (debug) {
//xxx            sprintf( msg, "6 - variance     : %ld", variance);
//xxx            disp_msg(msg);
//xxx            sprintf( msg, "7 - new count    : %ld", new_count);
//xxx            disp_msg(msg);
//xxx            sprintf( msg, "8 - mismatch_qty : %ld", mismatch_qty);
//xxx            disp_msg(msg);
//xxx            sprintf( msg, "9 - mismatch_val : %ld", mismatch_val);
//xxx            disp_msg(msg);
//xxx        }







        //Extract various count fields                          // 10-1-2005 SDH OSSR WAN
//xxx        WORD wCurSFCnt = clilfrec.uiSalesFloorCount;            //CSk 12-03-2012 SFA
//xxx        WORD wCurBSCnt = clilfrec.uiMainBSCount;                //CSk 12-03-2012 SFA
//xxx        WORD wCurOSCnt = clilfrec.uiOSSRCount;                  //CSk 12-03-2012 SFA
//xxx        WORD wNewSFCnt = pCLC->uiSalesFloorCount;               //CSk 12-03-2012 SFA
//xxx        WORD wNewBSCnt = pCLC->uiMainBSCount;                   //CSk 12-03-2012 SFA
//xxx        WORD wNewOSCnt = pCLC->uiOSSRCount;                     //CSk 12-03-2012 SFA

        //If Shop floor count has changed...                    // 10-1-2005 SDH OSSR WAN
//xxx        if ((wNewSFCnt != wCurSFCnt)) {                         // 10-1-2005 SDH OSSR WAN
//xxx
//xxx            //Set the location to Shop                          // 10-1-2005 SDH OSSR WAN
//xxx            lrtp[hh_unit]->bLocation = 'S';                     // 10-1-2005 SDH OSSR WAN
//xxx
//xxx            //If it was previously not counted then             // 10-1-2005 SDH OSSR WAN
//xxx            //decrement items remaining                         // 10-1-2005 SDH OSSR WAN
//xxx            if (wCurSFCnt == -1) {                              // 10-1-2005 SDH OSSR WAN
//xxx                WORD wTemp = clolfrec.uiOutSalesFloorCnt;       //CSk 12-03-2012 SFA
//xxx                wTemp = _max(wTemp - 1, 0);                     // 10-1-2005 SDH OSSR WAN
//xxx                clolfrec.uiOutSalesFloorCnt = wTemp;            //CSk 12-03-2012 SFA
//xxx            }                                                   // 10-1-2005 SDH OSSR WAN

            //Update the sales and the time                     // 10-1-2005 SDH OSSR WAN
//xxx no time in clilf            memcpy(clilfrec.abTimeSalesCntPD, abTimePD, 2);     // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.sales, abSales,                     // 10-1-2005 SDH OSSR WAN
//xxx                   sizeof(clilfrec.sales));                     // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.count_shopfloor,                    // 10-1-2005 SDH OSSR WAN
//xxx                   pCLC->count_shopfloor, 4);                   // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.count_shopfloor,                    //CSk 12-03-2012 SFA
//xxx                   pCLC->abCount_shopfloor, 4);                 //CSk 12-03-2012 SFA

//xxx        }                                                       // 10-1-2005 SDH OSSR WAN

        //If Back Shop count has changed...                     // 10-1-2005 SDH OSSR WAN
//xxx        if ((wNewBSCnt != wCurBSCnt)) {                         // 10-1-2005 SDH OSSR WAN
//xxx
//xxx            //Set the location to Shop                          // 10-1-2005 SDH OSSR WAN
//xxx            lrtp[hh_unit]->bLocation = 'S';                     // 10-1-2005 SDH OSSR WAN
//xxx
//xxx            //If it was previously not counted then             // 10-1-2005 SDH OSSR WAN
//xxx            //decrement items remaining                         // 10-1-2005 SDH OSSR WAN
//xxx            if (wCurBSCnt == -1) {                              // 10-1-2005 SDH OSSR WAN
//xxx                WORD wTemp = satoi(clolfrec.items_backshop, 3); // 10-1-2005 SDH OSSR WAN
//xxx                wTemp = _max(wTemp - 1, 0);                     // 10-1-2005 SDH OSSR WAN
//xxx                sprintf(sbuf, "%03d", wTemp);                   // 10-1-2005 SDH OSSR WAN
//xxx                memcpy(clolfrec.items_backshop, sbuf, 3);       // 10-1-2005 SDH OSSR WAN
//xxx            }                                                   // 10-1-2005 SDH OSSR WAN

            //Update the sales and the time                     // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abTimeStockCntPD, abTimePD, 2);     // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abAtStockSales, abSales,            // 10-1-2005 SDH OSSR WAN
//xxx                   sizeof(clilfrec.abAtStockSales));            // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.count_backshop,                     // 10-1-2005 SDH OSSR WAN
//xxx                   pCLC->count_backshop, 4);                    // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.count_backshop,                     //CSk 12-03-2012 SFA
//xxx                   pCLC->abCount, 4);                           //CSk 12-03-2012 SFA
//xxx        }                                                       // 10-1-2005 SDH OSSR WAN

        //If OSSR count has changed                             // 10-1-2005 SDH OSSR WAN
//xxx        if (wNewOSCnt != wCurOSCnt) {                           // 10-1-2005 SDH OSSR WAN

//xxx            //Set location to OSSR                              // 10-1-2005 SDH OSSR WAN
//xxx            lrtp[hh_unit]->bLocation = 'O';                     // 10-1-2005 SDH OSSR WAN

            //If its the first time then decrement remaining    // 10-1-2005 SDH OSSR WAN
            //count ensure it doesn't go negative for some      // 10-1-2005 SDH OSSR WAN
            //strange reason                                    // 10-1-2005 SDH OSSR WAN
//xxx           if (wCurOSCnt == -1) {                              // 10-1-2005 SDH OSSR WAN
//xxx                WORD wTemp = satoi(clolfrec.items_ossr,         // 10-1-2005 SDH OSSR WAN
//xxx                                   sizeof(clolfrec.items_ossr));    // 10-1-2005 SDH OSSR WAN
//xxx                wTemp = _max(wTemp - 1, 0);                     // 10-1-2005 SDH OSSR WAN
//xxx                sprintf(sbuf, "%03d", wTemp);                   // 10-1-2005 SDH OSSR WAN
//xxx                memcpy(clolfrec.items_ossr, sbuf,               // 10-1-2005 SDH OSSR WAN
//xxx                       sizeof(clolfrec.items_ossr));            // 10-1-2005 SDH OSSR WAN
//xxx            }                                                   // 10-1-2005 SDH OSSR WAN

            //In any case, update the time, sales, and count    // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abTimeOSSRCntPD, abTimePD,          // 10-1-2005 SDH OSSR WAN
//xxx                   sizeof(clilfrec.abTimeOSSRCntPD));           // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abAtOSSRSales, abSales,             // 10-1-2005 SDH OSSR WAN
//xxx                   sizeof(clilfrec.abAtOSSRSales));             // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abOSSRCount, pCLC->abOssrCount,     // 10-1-2005 SDH OSSR WAN
//xxx                   sizeof(clilfrec.abOSSRCount));               // 10-1-2005 SDH OSSR WAN
//xxx            memcpy(clilfrec.abOSSRCount, pCLC->abCount,         //CSk 12-03-2012 SFA
//xxx                   sizeof(clilfrec.abOSSRCount));               //CSk 12-03-2012 SFA
//xxx        }                                                       // 10-1-2005 SDH OSSR WAN




// ------------------------------------------------------------------------------------
//                                                                               //CSk 12-03-2012 SFA
// CLD - Count Lists - Create List Item Request
//
//       Response: CLI
// ------------------------------------------------------------------------------------
typedef struct {
   BYTE abCmd[3];
   BYTE abListId[3];     // Ignored if bListStatus is 'U'
   BYTE abSeq[3];        // Sequence No.
   BYTE bLocation;       // Count location:
                         // 'S' salesfloor
                         // 'B' Backshop
                         // 'O' OSSR
   BYTE abBarcode[13];   // 13-digit Barcode with check digit
} LRT_CLD;   // Stock File Accuracy - Count List Item Create
#define LRT_CLD_LTH sizeof(LRT_CLD)

typedef struct {                                                            //CSk 11-09-2012 SFA
    ULONG ulPOGKey;                                                         //CSk 11-09-2012 SFA
    UBYTE ubModSeq;                                                         //CSk 11-09-2012 SFA
} Module;                                                                   //CSk 11-09-2012 SFA


void CreateListItem(char* inbound) {

    ENQUIRY enqbuf;
    URC usrrc;
    LONG rc2;
    LONG list_id;
    WORD hour, min;                                                              //TAT 24-10-2012 SFA
    LONG sec, day, month, year;                                                  //TAT 24-10-2012 SFA

    //Input and output views
    LRT_CLD* pCLD = (LRT_CLD*)inbound;
    LRT_CLI* pCLI = (LRT_CLI*)out;

    //Initial checks
    if (IsHandheldUnknown()) return;
    UpdateActiveTime();

    // Need to do this because data needed by CLI removed from clilfrec
    usrrc = stock_enquiry( SENQ_TSF, pCLD->abBarcode, &enqbuf );
    if (debug) {
        sprintf(msg, "stock_enquiry() rc:%d", usrrc);
        disp_msg(msg);
    }

    if (usrrc != RC_OK) {
        log_event101(usrrc, 0, __LINE__);
//        if (debug) {                                                           //TAT 11-09-2012 SFA
//            sprintf(msg, "Error - stock_enquiry(). RC:%d",                     //TAT 11-09-2012 SFA
//                    usrrc);                                                    //TAT 11-09-2012 SFA
//            disp_msg(msg);                                                     //TAT 11-09-2012 SFA
//        }                                                                      //TAT 11-09-2012 SFA
//        prep_nak("ERRORStock enquiry failed. "                                 //TAT 11-09-2012 SFA
//                 "Check appl event logs");                                     //TAT 11-09-2012 SFA
        prep_nak("ERROR Item not on file" );                                     //TAT 11-09-2012 SFA
        return;
    }

    // Create new CLILF record - initialise WHOLE record first
    memset(&clilfrec, 0x00, sizeof(clilfrec));

    memcpy(clilfrec.abListId, pCLD->abListId, sizeof(clilfrec.abListId));
    memcpy(clilfrec.abSeq,    pCLD->abSeq,    sizeof(clilfrec.abSeq));
    pack(clilfrec.abItemCode, 4, enqbuf.boots_code, 7, 1 );
    memset(&clilfrec.abPIITM, 0x30, sizeof(clilfrec.abPIITM));
    clilfrec.bCountStatus = 'U';

    memset(&clilfrec.abDateOfLastCnt, 0x00, sizeof(clilfrec.abDateOfLastCnt));
    if (stock.present && (clilfrec.abDateOfLastCnt[0] != 0x00)) {
        memcpy(clilfrec.abDateOfLastCnt, stockrec.date_last_count, 3);
    }

    clilfrec.wLastSalesCount   = 0;
    clilfrec.wMainBSCount      = -1; // -1 means location NOT counted
    clilfrec.wOSSRCount        = -1;
    clilfrec.wPendingBSCount   = -1;
    clilfrec.wPendingOSSRCount = -1;
    clilfrec.wSalesFloorCount  = -1;

    // ONLY set up site info in CLILF if a batch-POD device is detected.           //CSk 11-09-2012 SFA
    // This follows similar logic to PogSiteEnq() function.                        //CSk 11-09-2012 SFA
    if (!memcmp(lrtp[hh_unit]->Type, "M", 1)) {                                    //CSk 11-09-2012 SFA
        // Check if item is multi-sited                                            //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
        //Input and output views, and local vars                                   //CSk 11-09-2012 SFA
        Module LastModule;                                                         //CSk 11-09-2012 SFA
        WORD wMod = 0;                                                             //CSk 11-09-2012 SFA
        WORD wStartChain = 0;                                                      //CSk 11-09-2012 SFA
        WORD wChn = wStartChain;                                                   //CSk 11-09-2012 SFA
        WORD wItemRepeat = 0;                                                      //CSk 11-09-2012 SFA
        WORD wItem;                                                                //CSk 11-09-2012 SFA
        BOOLEAN fSritmRead = FALSE;                                                //CSk 11-09-2012 SFA
        BOOLEAN fSrmodRead = FALSE;                                                //CSk 11-09-2012 SFA
        BOOLEAN fSrpogRead = FALSE;                                                //CSk 11-09-2012 SFA
        BYTE abItemCodePacked[3];                                                  //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
        //Initialise packed item code - ignore check digit                         //CSk 11-09-2012 SFA
        pack(abItemCodePacked, 3, enqbuf.boots_code, 6, 0);                        //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
        LastModule.ulPOGKey = 0;                                                   //CSk 11-09-2012 SFA
        LastModule.ubModSeq = 0;                                                   //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
        // Loop round 32 possible sites                                            //CSk 11-09-2012 SFA
        for (WORD wSite = 0; wSite <= 32; wSite++) {                               //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //Build SRITEM key, read file, handle errors                           //CSk 11-09-2012 SFA
            if ((MEMCMP(sritmlrec.abItemCode, abItemCodePacked) != 0) ||           //CSk 11-09-2012 SFA
                (sritmlrec.ubRecChain != wChn) || !fSritmRead) {                   //CSk 11-09-2012 SFA
                MEMCPY(sritmlrec.abItemCode, abItemCodePacked);                    //CSk 11-09-2012 SFA
                sritmlrec.ubRecChain = wChn;                                       //CSk 11-09-2012 SFA
                rc2 = SritmlRead(__LINE__);                                        //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
                if (rc2 <= 0) {                                                    //CSk 11-09-2012 SFA
                    //Break out of the main loop as nothing more to read           //CSk 11-09-2012 SFA
                    break;                                                         //CSk 11-09-2012 SFA
                }                                                                  //CSk 11-09-2012 SFA
                fSritmRead = TRUE;                                                 //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //If the key is null then we've processed all the item's sites         //CSk 11-09-2012 SFA
            if (sritmlrec.aModuleKey[wMod].ulPOGKey == 0) break;                   //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //Get current POG key and read, end on read error                      //CSk 11-09-2012 SFA
            if (srpogrec.ulKey != sritmlrec.aModuleKey[wMod].ulPOGKey ||           //CSk 11-09-2012 SFA
                !fSrpogRead) {                                                     //CSk 11-09-2012 SFA
                srpogrec.ulKey = sritmlrec.aModuleKey[wMod].ulPOGKey;              //CSk 11-09-2012 SFA
                rc2 = SrpogRead(__LINE__);                                         //CSk 11-09-2012 SFA
                if (rc2 < 0) break;                                                //CSk 11-09-2012 SFA
                fSrpogRead = TRUE;                                                 //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //Increment item repeat if key is the same as last time                //CSk 11-09-2012 SFA
            if ((sritmlrec.aModuleKey[wMod].ulPOGKey == LastModule.ulPOGKey) &&    //CSk 11-09-2012 SFA
                (sritmlrec.aModuleKey[wMod].ubModSeq == LastModule.ubModSeq)) {    //CSk 11-09-2012 SFA
                wItemRepeat++;                                                     //CSk 11-09-2012 SFA
            } else {                                                               //CSk 11-09-2012 SFA
                wItemRepeat = 0;                                                   //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
            LastModule.ulPOGKey = sritmlrec.aModuleKey[wMod].ulPOGKey;             //CSk 11-09-2012 SFA
            LastModule.ubModSeq = sritmlrec.aModuleKey[wMod].ubModSeq;             //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //Read SRMOD                                                           //CSk 11-09-2012 SFA
            if (srmodrec.ulKey    != sritmlrec.aModuleKey[wMod].ulPOGKey ||        //CSk 11-09-2012 SFA
                srmodrec.ubModSeq != sritmlrec.aModuleKey[wMod].ubModSeq ||        //CSk 11-09-2012 SFA
                srmodrec.ubRecChain != 0) {                                        //CSk 11-09-2012 SFA
                fSrmodRead = FALSE;                                                //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
            srmodrec.ulKey = sritmlrec.aModuleKey[wMod].ulPOGKey;                  //CSk 11-09-2012 SFA
            srmodrec.ubModSeq = sritmlrec.aModuleKey[wMod].ubModSeq;               //CSk 11-09-2012 SFA
            srmodrec.ubRecChain = 0;                                               //CSk 11-09-2012 SFA
            WORD wRepeat = wItemRepeat;                                            //CSk 11-09-2012 SFA
            BOOLEAN fFound = FALSE;                                                //CSk 11-09-2012 SFA
            if (!fSrmodRead) {                                                     //CSk 11-09-2012 SFA
                rc2 = SrmodRead(__LINE__);                                         //CSk 11-09-2012 SFA
                if (rc2 > 0) fSrmodRead = TRUE;                                    //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
            while (!fFound && rc2 > 0) {                                           //CSk 11-09-2012 SFA
                for (wItem = 0; wItem < SRMOD_NUM_ITEMS; wItem++) {                //CSk 11-09-2012 SFA
                    if (MEMCMP(srmodrec.aShelfItem[wItem].abItemCode,              //CSk 11-09-2012 SFA
                               abItemCodePacked) == 0) {                           //CSk 11-09-2012 SFA
                        if (wRepeat > 0) {                                         //CSk 11-09-2012 SFA
                            wRepeat--;                                             //CSk 11-09-2012 SFA
                        } else {                                                   //CSk 11-09-2012 SFA
                            fFound = TRUE;                                         //CSk 11-09-2012 SFA
                            break;                                                 //CSk 11-09-2012 SFA
                        }                                                          //CSk 11-09-2012 SFA
                    }                                                              //CSk 11-09-2012 SFA
                }                                                                  //CSk 11-09-2012 SFA
                if (fFound) break;                                                 //CSk 11-09-2012 SFA
                srmodrec.ubRecChain++;                                             //CSk 11-09-2012 SFA
                rc2 = SrmodRead(__LINE__);                                         //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
            if (!fFound) break;                                                    //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            // Populate site info                                                  //CSk 11-09-2012 SFA
            clilfrec.aMS_Details[wSite].ulModuleId  = sritmlrec.aModuleKey[wMod].ulPOGKey;    //CSk 11-09-2012 SFA
            clilfrec.aMS_Details[wSite].ubModuleSeq = sritmlrec.aModuleKey[wMod].ubModSeq;    //CSk 11-09-2012 SFA
            clilfrec.aMS_Details[wSite].ubRepeatCnt = sritmlrec.aModuleKey[wMod].ubRepeatCnt; //CSk 11-09-2012 SFA
            clilfrec.aMS_Details[wSite].uiCount     = -1;                          //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
            //Increment to read next site                                          //CSk 11-09-2012 SFA
            wMod++;                                                                //CSk 11-09-2012 SFA
            if (wMod == SRITM_NUM_MODS) {                                          //CSk 11-09-2012 SFA
                wChn++;                                                            //CSk 11-09-2012 SFA
                wMod = 0;                                                          //CSk 11-09-2012 SFA
                if (wChn > 255) break;                                             //CSk 11-09-2012 SFA
            }                                                                      //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
        }                                                                          //CSk 11-09-2012 SFA
                                                                                   //CSk 11-09-2012 SFA
    } // if (!memcmp(lrtp[hh_unit]->Type, "M", 1))                                 //CSk 11-09-2012 SFA

    // Update CLILF item record
    rc2 = ClilfWrite(__LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to update CLILF. Check appl event logs" );
        return;
    }

    list_id = satol(pCLD->abListId, 3);
    // Read clolf record
    rc2 = ClolfRead((list_id - 1L), __LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to read from CLOLF. "
                  "Check appl event logs" );
        return;
    }


    if(clolfrec.uiTotalItems != satol(clilfrec.abSeq,3)) {                //TAT 13-12-2012 SFA
        clolfrec.uiTotalItems++;
        clolfrec.uiOutSalesFloorCnt++;                                    //TAT 18-12-2012 SFA
        clolfrec.uiOutBackShopCnt++;                                      //TAT 18-12-2012 SFA
        clolfrec.uiOutOSSRCnt++;                                          //TAT 18-12-2012 SFA
    } else {                                                              //TAT 13-12-2012 SFA
        // We are overwriting an exisiting record                         //TAT 13-12-2012 SFA
    }                                                                     //TAT 13-12-2012 SFA

    if (clolfrec.uiTotalItems == 1) {                                     //TAT 23-08-2012 SFA
         memcpy(clolfrec.abPickerId, clolfrec.abCreatorId,                //TAT 24-10-2012 SFA
                sizeof(clolfrec.abPickerId));                             //TAT 24-10-2012 SFA
         sysdate( &day, &month, &year, &hour, &min, &sec );               //TAT 24-10-2012 SFA
         sprintf( sbuf, "%02d%02d", hour, min );                          //TAT 24-10-2012 SFA
         pack( clolfrec.abPickStartTime, 2, sbuf, 4, 0 ); // 2-PD HHMM    //TAT 24-10-2012 SFA
         clolfrec.bListStatus = 'A';                                      //TAT 23-08-2012 SFA
    }                                                                     //TAT 23-08-2012 SFA

    // Update CLOLF list header
    rc2 = ClolfWrite(list_id - 1L, __LINE__);
    if (rc2 <= 0L) {
        prep_nak("ERRORUnable to write to CLOLF. Check appl event logs");
        return;
    }
    //------------------
    // Setup CLI Message
    //------------------
    memcpy( pCLI->cmd, "CLI", 3 );
    memcpy( pCLI->list_id, clilfrec.abListId, 3 );
    sprintf(sbuf, "%03d", clolfrec.uiTotalItems);
    memcpy( pCLI->abNumItemsInList, sbuf, 3);
    memcpy( pCLI->abNextItemSeq, "000", 3 );
    pCLI->cMoreToCome = 'N';

    // Sequence not applicable when sending details for ONE item
    memcpy( pCLI->CLI_items[0].abItemSeq,    "000",              3 );
    memcpy( pCLI->CLI_items[0].abItemcode,   enqbuf.boots_code,  7 );
    memcpy( pCLI->CLI_items[0].abParentCode, enqbuf.parent_code, 7 );
    memcpy( pCLI->CLI_items[0].abBarcode,    enqbuf.item_code,   13);
    memcpy( pCLI->CLI_items[0].abSelDesc,    enqbuf.sel_desc,    45);
    //If the first two chars of the SEL desc are "X " then
    //replace it with the IDF description
    if (strncmp( enqbuf.sel_desc, "X ", 2 )==0 ||                         //TAT 06-12-2012 SFA
        strncmp( enqbuf.sel_desc, "x ", 2 )==0) {                         //TAT 06-12-2012 SFA
        memset( sbuf, 0x20, 45 );                                         //TAT 06-12-2012 SFA
        format_text( enqbuf.item_desc, 24, sbuf, 45, 15 );                //TAT 06-12-2012 SFA
        memcpy( pCLI->CLI_items[0].abSelDesc, sbuf, 45);                  //TAT 06-12-2012 SFA
    }                                                                     //TAT 06-12-2012 SFA

    pCLI->CLI_items[0].cActiveDealFlag = enqbuf.active_deal_flag[0];

    memcpy(pCLI->CLI_items[0].abProductGroup, enqbuf.cProductGrp,  6 );

    sprintf(pCLI->CLI_items[0].abCountBackshop,    "%04d", clilfrec.wMainBSCount);
    sprintf(pCLI->CLI_items[0].abCountPendingBackshop, "%04d", clilfrec.wPendingBSCount);
    sprintf(pCLI->CLI_items[0].abCountShopfloor,   "%04d", clilfrec.wSalesFloorCount);
    sprintf(pCLI->CLI_items[0].abOSSRCount,        "%04d", clilfrec.wOSSRCount);
    sprintf(pCLI->CLI_items[0].abPendingOSSRCount, "%04d", clilfrec.wPendingOSSRCount);

    pCLI->CLI_items[0].cStatus   = enqbuf.status[0];
    pCLI->CLI_items[0].cOssrItem = (enqbuf.cOssrItem == 'Y' ? 'O' : 'N');    //TAT 11-09-2012 SFA

    memcpy(sbuf, "00000000", 8); // Initialise CCYYMMDD
    if (stock.present) {
        unpack( sbuf+2, 6, stockrec.date_last_count, 3, 0 );
        if (*sbuf+2 != '0') {
            memcpy(sbuf, "20", 2);  // Add century
        }
    }
    memcpy( pCLI->CLI_items[0].abLastCountDate, sbuf,  8 );

    // Check if item on a Pending Salesplanner that is active in the next N days
    GetFuturePendingSalesplanFlag( enqbuf.boots_code,
                                   &pCLI->CLI_items[0].cPendingSaleFlag);

    memcpy(pCLI->CLI_items[0].abStockFigure,  enqbuf.stock_figure, 6 );

    out_lth = LRT_CLI_CLD_LTH;  // Only return info for ONE item

}


// ------------------------------------------------------------------------------------
//
// CLX - Count Lists - Sign Off List
//
//
//
// ------------------------------------------------------------------------------------

typedef struct {
   BYTE cmd[3];
   BYTE list_id[3];
   BYTE commit_flag[1];
   BYTE bCountListType;  // 'H' Head office                     //CSk 12-03-2012 SFA
                         // 'R' Recount                         //CSk 12-03-2012 SFA
                         // 'N' Negative                        //CSk 12-03-2012 SFA
                         // 'U' User Generated Count list       //CSk 12-03-2012 SFA
                         // 'L' Local Lists (for future use)    //CSk 12-03-2012 SFA
} LRT_CLX;               // Counting - List Exit
#define LRT_CLX_LTH sizeof(LRT_CLX)

void CountsSignOffList(char* inbound) {

    ENQUIRY enqbuf;                                             //CSk 12-03-2012 SFA
    BYTE stkmqrec[128];                                         //CSk 12-03-2012 SFA
    BYTE abItemCode[13];                                        //CSk 12-03-2012 SFA
    LRT_CLX* pCLX = (LRT_CLX*)inbound;                          // SDH 10-01-05 OSSR WAN
    LONG rc2;
    LONG list_id = satol(pCLX->list_id, 3);
    LONG lItemsSoldToday;                                       //CSk 12-03-2012 SFA
    LONG sec, day, month, year;                                 //CSk 12-03-2012 SFA
    UINT uiCompletedCnt = 0;                                    //CSk 12-03-2012 SFA
    UINT uiSeqCnt = 1;  //Sequence Counter                      //CSk 12-03-2012 SFA
    LONG wAllCntFig;                                            //TAT 20-08-2012 SFA
    WORD hour, min;                                             //CSk 12-03-2012 SFA
    URC usrrc;                                                  //CSk 12-03-2012 SFA
    B_DATE nowDate;                                             //TAT 23-08-2012 SFA
    B_TIME nowTime;                                             //TAT 23-08-2012 SFA
    DOUBLE dTodayJul;                                           //TAT 23-08-2012 SFA

    //Get today's date as a Julian
    GetSystemDate(&nowTime, &nowDate);                          //TAT 23-08-2012 SFA
    dTodayJul = ConvGJ(nowDate.wDay, nowDate.wMonth,            //TAT 23-08-2012 SFA
                       nowDate.wYear);                          //TAT 23-08-2012 SFA


    if (IsHandheldUnknown()) return;                            // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                         // SDH 26-11-04 CREDIT CLAIM

    // Read CLOLF list header                                                        //CSk 12-03-2012 SFA
    rc2 = ClolfRead(list_id - 1L, __LINE__); // list_id set above                    //CSk 12-03-2012 SFA
    if (rc2 <= 0L) {                                                                 //CSk 12-03-2012 SFA
        prep_nak("ERRORUnable to read from CLOLF. "                                  //CSk 12-03-2012 SFA
                  "Check appl event logs" );                                         //CSk 12-03-2012 SFA
        return;                                                                      //CSk 12-03-2012 SFA
    }                                                                                //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
    if (clolfrec.bListType == 'U') {                                                 //TAT 16-11-2012 SFA
        memcpy( clilfrec.abListId, pCLX->list_id, 3 );                               //TAT 16-11-2012 SFA
        sprintf( sbuf, "%03d", clolfrec.uiTotalItems); // runs from 001 to 999       //TAT 16-11-2012 SFA
        memcpy( clilfrec.abSeq, sbuf, 3 );                                           //TAT 16-11-2012 SFA
        rc2 = ClilfRead(__LINE__);                                                   //TAT 16-11-2012 SFA
                                                                                     //TAT 16-11-2012 SFA
        if (rc2 > 0L){                                                               //TAT 16-11-2012 SFA
            // This is the last item in a user generated list
            if (clilfrec.wSalesFloorCount < 0 &&                                     //TAT 16-11-2012 SFA
                clilfrec.wMainBSCount < 0 &&                                         //TAT 16-11-2012 SFA
                clilfrec.wOSSRCount < 0 &&                                           //TAT 16-11-2012 SFA
                clilfrec.wPendingBSCount < 0 &&                                      //TAT 16-11-2012 SFA
                clilfrec.wPendingOSSRCount < 0) {                                    //TAT 16-11-2012 SFA
                // No counts added for the item so this item was incorrectly added
                memcpy( sbuf, clilfrec.abListId, 3);                                 //TAT 16-11-2012 SFA
                memcpy( sbuf + 3, clilfrec.abSeq, 3);                                //TAT 16-11-2012 SFA
                // Delete the item from CLILF
                rc2 = s_special(0x74, 0, clilf.fnum, sbuf, 6, 0L, 0L);               //TAT 16-11-2012 SFA
                if (rc2 < 0L) {                                                      //TAT 16-11-2012 SFA
                    sprintf(msg, "CLILF record delete error. RC:%08lX  KEY:%6s",     //TAT 16-11-2012 SFA
                            rc2, sbuf);                                              //TAT 16-11-2012 SFA
                } else {                                                             //TAT 16-11-2012 SFA
                    sprintf(msg, "CLILF record deleted. KEY:%6s", sbuf);             //TAT 16-11-2012 SFA
                    clolfrec.uiTotalItems--;                                         //TAT 16-11-2012 SFA
                    clolfrec.uiOutSalesFloorCnt--;                                   //TAT 18-12-2012 SFA
                    clolfrec.uiOutBackShopCnt--;                                     //TAT 18-12-2012 SFA
                    clolfrec.uiOutOSSRCnt--;                                         //TAT 18-12-2012 SFA
                }                                                                    //TAT 16-11-2012 SFA
                disp_msg(msg);                                                       //TAT 16-11-2012 SFA
            }                                                                        //TAT 16-11-2012 SFA
        }                                                                            //TAT 16-11-2012 SFA
    }                                                                                //TAT 16-11-2012 SFA
    // Scan through the List and write all completed item counts                     //CSk 12-03-2012 SFA
    // directly to the STKMQ and mark them as completed. If all                      //CSk 12-03-2012 SFA
    // the items and sites have been counted in a list then mark                     //CSk 12-03-2012 SFA
    // the list as complete.                                                         //CSk 12-03-2012 SFA
    if (pCLX->commit_flag[0] == 'Y') {                                               //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
        memcpy(clilfrec.abListId, pCLX->list_id, 3 );                                //CSk 12-03-2012 SFA
        sprintf( sbuf, "%03d", uiSeqCnt); // runs from 001 to 999                    //CSk 12-03-2012 SFA
        memcpy(clilfrec.abSeq, sbuf, 3 );                                            //CSk 12-03-2012 SFA
        rc2 = ClilfRead(__LINE__);                                                   //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
        while (rc2 > 0L){                                                            //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
            if (clilfrec.bCountStatus != 'C') {                                      //CSk 12-03-2012 SFA //TAT 02-11-2012 SFA
                // Only process uncounted items. Only mark as                        //CSk 12-03-2012 SFA
                // Counted if ALL locations have been counted                        //CSk 12-03-2012 SFA
                // A store can only have a Backshop or OSSR and not both.            //CSk 12-03-2012 SFA
                // Write count to STKMQ.                                             //CSk 12-03-2012 SFA
                if (clilfrec.wSalesFloorCount != -1 &&                               //TAT 20-08-2012 SFA
                       ( !(clilfrec.wMainBSCount == -1 && clilfrec.wOSSRCount == -1) )) { //TAT 20-08-2012 SFA
                    // Add 1 to offset one of the -1 (not counted) counts            //CSk 12-03-2012 SFA
                    wAllCntFig = clilfrec.wSalesFloorCount + clilfrec.wMainBSCount + clilfrec.wOSSRCount + 1; //CSk 12-03-2012 SFA
                    if (clilfrec.wPendingBSCount > 0) {                              //CSk 12-03-2012 SFA
                        wAllCntFig += clilfrec.wPendingBSCount;                      //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                    if (clilfrec.wPendingOSSRCount > 0) {                            //CSk 12-03-2012 SFA
                        wAllCntFig += clilfrec.wPendingOSSRCount;                    //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    // Perform stock enquiry in order to obtain item data            //CSk 12-03-2012 SFA
                    memset( abItemCode, '0', 13 );                                   //CSk 12-03-2012 SFA
                    unpack( sbuf, 8, clilfrec.abItemCode, 4, 0 );                    //CSk 12-03-2012 SFA
                    memcpy( abItemCode+6, sbuf+1, 7 );                               //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    usrrc = stock_enquiry( SENQ_TSF, abItemCode, &enqbuf );          //CSk 12-03-2012 SFA
                    if (usrrc != RC_OK) {                                            //CSk 12-03-2012 SFA
                        log_event101(usrrc, 0, __LINE__);                            //CSk 12-03-2012 SFA
                        if (debug) {                                                 //CSk 12-03-2012 SFA
                            sprintf(msg, "Error - stock_enquiry(). RC:%d",           //CSk 12-03-2012 SFA
                                    usrrc);                                          //CSk 12-03-2012 SFA
                            disp_msg(msg);                                           //CSk 12-03-2012 SFA
                        }                                                            //CSk 12-03-2012 SFA
                        prep_nak("ERRORStock enquiry failed. "                       //CSk 12-03-2012 SFA
                                 "Check appl event logs");                           //CSk 12-03-2012 SFA
                        return;                                                      //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    lItemsSoldToday = satol(enqbuf.items_sold_today,6);              //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA

                    // Subtract variance ie. sales now minus sales at start of count //CSk 12-03-2012 SFA
                    wAllCntFig -= (lItemsSoldToday - clilfrec.wLastSalesCount);      //CSk 12-03-2012 SFA

                    usrrc = open_minls();                                            //TAT 23-08-2012 SFA
                    if (usrrc<=RC_DATA_ERR) {                                        //TAT 23-08-2012 SFA
                        prep_nak("ERRORUnable to open MINLS file. "                  //TAT 23-08-2012 SFA
                                  "Check appl event logs" );                         //TAT 23-08-2012 SFA
                        return;                                                      //TAT 23-08-2012 SFA
                    }                                                                //TAT 23-08-2012 SFA

                    //Read the MINLS and handle any errors                           //TAT 23-08-2012 SFA
                    memcpy(minlsrec.boots_code, clilfrec.abItemCode, 4);             //TAT 16-11-2012 SFA
                    dump ( minlsrec.boots_code, 4 ) ;                                //TAT 16-11-2012 SFA
                    rc2 = ReadMinls(__LINE__);                                       //TAT 23-08-2012 SFA
                    if (rc2<=0L) {                                                   //TAT 23-08-2012 SFA
                        // Write record to MINLS                                     //TAT 23-08-2012 SFA
                        memset( minlsrec.recount_date, 0x00, 3);                     //TAT 23-08-2012 SFA
                        sprintf( sbuf, "%06ld", wAllCntFig);                         //TAT 23-08-2012 SFA
                        pack( minlsrec.discrepancy, 3, sbuf, 6, 0 );                 //TAT 23-08-2012 SFA
                        // minls status 1 = initial count                            //TAT 23-08-2012 SFA
                        // minls status 2 = pending recount                          //TAT 23-08-2012 SFA
                        // minls status 3 = recount                                  //TAT 23-08-2012 SFA
                        // minls status 4 = count complete                           //TAT 23-08-2012 SFA
                        //Force the record type to '3' for a fast count              //TAT 23-08-2012 SFA
                        *minlsrec.count_status = '3';                                //TAT 23-08-2012 SFA

                        //we have a recount with no date. Set                        //TAT 23-08-2012 SFA
                        //recount date to today                                      //TAT 23-08-2012 SFA
                        ConvJG(dTodayJul, &day, &month, &year);                      //TAT 23-08-2012 SFA
                        sprintf(sbuf, "%02d", (UBYTE)(year%100L));                   //TAT 23-08-2012 SFA
                        pack(minlsrec.recount_date, 1, sbuf, 2, 0);                  //TAT 23-08-2012 SFA
                        sprintf(sbuf, "%02d", (UBYTE)(month));                       //TAT 23-08-2012 SFA
                        pack(minlsrec.recount_date+1, 1, sbuf, 2, 0);                //TAT 23-08-2012 SFA
                        sprintf(sbuf, "%02d", (UBYTE)(day));                         //TAT 23-08-2012 SFA
                        pack(minlsrec.recount_date+2, 1, sbuf, 2, 0);                //TAT 23-08-2012 SFA

                        WriteMinls(__LINE__);                                        //TAT 23-08-2012 SFA
                    }                                                                //TAT 23-08-2012 SFA

                    close_minls(CL_SESSION);                                         //TAT 23-08-2012 SFA

                    // Write type 13 record to STKMQ                                 //CSk 12-03-2012 SFA
                    usrrc = open_stkmq();                                            //CSk 12-03-2012 SFA
                    if (usrrc<=RC_DATA_ERR) {                                        //CSk 12-03-2012 SFA
                        prep_nak("ERRORUnable to open STKMQ file. "                  //CSk 12-03-2012 SFA
                                  "Check appl event logs" );                         //CSk 12-03-2012 SFA
                        return;                                                      //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                    stkmqrec[0] = 0x22;                                              //CSk 12-03-2012 SFA
                    stkmqrec[1] = 0x13;                                              //CSk 12-03-2012 SFA
                    stkmqrec[2] = 0x3B;                                              //CSk 12-03-2012 SFA
                    sysdate( &day, &month, &year, &hour, &min, &sec );               //CSk 12-03-2012 SFA
                    sprintf( sbuf, "%02ld%02ld%02ld", year%100, month, day );        //CSk 12-03-2012 SFA
                    pack( stkmqrec+3, 3, sbuf, 6, 0 );                               //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    sprintf( sbuf, "%02d%02d%02d", hour, min, (WORD)sec );           //CSk 12-03-2012 SFA
                    pack( stkmqrec+6, 3, sbuf, 6, 0 );                               //CSk 12-03-2012 SFA
                    if (clolfrec.bListType == 'H') {                                 //TAT 20-09-2012 SFA
                        memcpy( stkmqrec+9, clolfrec.abPILSTID, 4 );                 //TAT 20-09-2012 SFA
                        memcpy( stkmqrec+9+4, clilfrec.abPIITM, 2 );                 //TAT 20-09-2012 SFA
                        stkmqrec[15] = 'C';                                          //TAT 23-11-2012 SFA
                    } else {                                                         //TAT 02-11-2012 SFA
                        memcpy( stkmqrec+9, "000000", 6 );                           //TAT 02-11-2012 SFA
                        stkmqrec[15] = 'R';                                          //TAT 23-11-2012 SFA
                    }                                                                //TAT 20-09-2012 SFA
                    pack( stkmqrec+23, 2, sbuf, 4, 0 );                              //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
//                    stkmqrec[15] = 'C'; // Stock Movement Type - Count               //CSk 12-03-2012 SFA
                    memcpy( stkmqrec+16, clilfrec.abItemCode, 4 );                   //CSk 12-03-2012 SFA
                    sprintf( sbuf, "%02ld%02ld%02ld", day, month, year%100 );        //CSk 12-03-2012 SFA
                    pack( stkmqrec+20, 3, sbuf, 6, 0 );                              //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    memset( stkmqrec+25, 0x00, 5);                                   //CSk 12-03-2012 SFA
                    pack( stkmqrec+25+2, 3, enqbuf.item_price, 6, 0 );               //CSk 12-03-2012 SFA
                    memset( stkmqrec+31-1, 0x3B, 1 );                                //CSk 12-03-2012 SFA
                    sprintf( sbuf, "%04ld%cXXXX%c%c%c",                              //CSk 12-03-2012 SFA
                            wAllCntFig, 0x3B, 0x22, 0x0D, 0x0A );                    //CSk 12-03-2012 SFA
                    memcpy( stkmqrec+31, sbuf, 16 );                                 //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    if (debug) {                                                     //CSk 12-03-2012 SFA
                        sprintf(msg, "WR STKMQ :");                                  //CSk 12-03-2012 SFA
                        disp_msg(msg);                                               //CSk 12-03-2012 SFA
                        dump( (BYTE *)&stkmqrec, STKMQ_T13_LTH );                    //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                    rc2 = s_write( A_EOFOFF, stkmq.fnum, (void *)&stkmqrec,          //CSk 12-03-2012 SFA
                                   STKMQ_T13_LTH, 0L );                              //CSk 12-03-2012 SFA
                    if (rc2<=0) {                                                    //CSk 12-03-2012 SFA
                        log_event101(rc2, STKMQ_REP, __LINE__);                      //CSk 12-03-2012 SFA
                        if (debug) {                                                 //CSk 12-03-2012 SFA
                            sprintf(msg, "Err-W to STKMQ. RC:%08lX", rc2);           //CSk 12-03-2012 SFA
                            disp_msg(msg);                                           //CSk 12-03-2012 SFA
                        }                                                            //CSk 12-03-2012 SFA
                        prep_nak("ERRORUnable to write to STKMQ. Check appl event logs");
                        return;                                                      //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                    disp_msg("Write STKMQ OK");                                      //CSk 12-03-2012 SFA
                    close_stkmq( CL_SESSION );                                       //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
                    // Update CLILF item record                                      //CSk 12-03-2012 SFA
                    clilfrec.bCountStatus = 'C';                                     //CSk 12-03-2012 SFA
                    rc2 = ClilfWrite(__LINE__);                                      //TAT 14-08-2012 SFA
                    if (rc2 <= 0L) {                                                 //CSk 12-03-2012 SFA
                        prep_nak("ERRORUnable to update CLILF. Check appl event logs" );
                        return;                                                      //CSk 12-03-2012 SFA
                    }                                                                //CSk 12-03-2012 SFA
                    clolfrec.uiOutSalesFloorCnt--;                                   //TAT 03-12-2012 SFA
                    uiCompletedCnt++;                                                //CSk 12-03-2012 SFA
                } //if (wSalesFloorCount != -1 &&                                    //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
            } else {                                                                 //CSk 12-03-2012 SFA
                uiCompletedCnt++;  // must already be counted                        //CSk 12-03-2012 SFA
            } //if (clilfrec.bCountStatus != 'C')                                    //CSk 12-03-2012 SFA //TAT 02-11-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
            uiSeqCnt++;                                                              //CSk 12-03-2012 SFA
            sprintf( sbuf, "%03d", uiSeqCnt);                                        //CSk 12-03-2012 SFA
            memcpy(clilfrec.abSeq, sbuf, 3 );                                        //CSk 12-03-2012 SFA
            rc2 = ClilfRead(__LINE__);                                               //CSk 12-03-2012 SFA
        } // end while (rc2 > 0L)                                                    //CSk 12-03-2012 SFA
                                                                                     //CSk 12-03-2012 SFA
        if (clolfrec.uiTotalItems == uiCompletedCnt) {                               //CSk 12-03-2012 SFA
            // Update CLOLF list header                                              //CSk 12-03-2012 SFA
            clolfrec.bListStatus = 'C';                                              //CSk 12-03-2012 SFA
        } else {                                                                     //TAT 16-08-2012 SFA
            clolfrec.bListStatus = 'P';                                              //TAT 16-08-2012 SFA
            if (uiCompletedCnt == 0) {                                               //TAT 24-09-2012 SFA
                clolfrec.bListStatus = 'I';                                          //TAT 24-09-2012 SFA
            }                                                                        //TAT 24-09-2012 SFA
        }                                                                            //CSK 12-03-2012 SFA
        sysdate( &day, &month, &year, &hour, &min, &sec );                           //CSk 12-03-2012 SFA
        sprintf( sbuf, "%02d%02d", hour, min );                                      //TAT 11-09-2012 SFA
        pack( clolfrec.abPickEndTime, 2, sbuf, 4, 0 ); // 2-PD HHMM                  //CSk 12-03-2012 SFA
        rc2 = ClolfWrite(list_id - 1L, __LINE__);                                    //CSk 12-03-2012 SFA
        if (rc2 <= 0L) {                                                             //CSk 12-03-2012 SFA
            prep_nak("ERRORUnable to write to CLOLF. Check appl event logs");        //CSk 12-03-2012 SFA
            return;                                                                  //CSk 12-03-2012 SFA
        }                                                                            //CSk 12-03-2012 SFA
    } // if (pCLX->commit_flag == 'Y')                                               //CSk 12-03-2012 SFA


    // v4.0 START
    // Audit
    sprintf( msg, "%04ld", list_id );
    memcpy( ((LRTLG_CLX *)dtls)->list_id, msg , 4 );
    memcpy( ((LRTLG_CLX *)dtls)->commit_flag,
            (((LRT_CLX *)(inbound))->commit_flag), 1 );
    memset( ((LRTLG_CLX *)dtls)->resv, 0x00, 7 );
    lrt_log( LOG_CLX, hh_unit, dtls );
    // v4.0 END

    prep_ack("");                                               //SDH 23-Aug-2006 Planners

}

//xxx    //If the list is being commited then ensure that            // SDH 10-01-05 OSSR WAN
//xxx    //it is complete enough to commit                           // SDH 10-01-05 OSSR WAN
//xxx    if (pCLX->commit_flag[0] == 'Y') {                          // SDH 10-01-05 OSSR WAN
//xxx
//xxx        //If we're not in an OSSR store...                      // SDH 10-01-05 OSSR WAN
//xxx        if ((rfscfrec1and2.ossr_store == 'N') ||                // SDH 04-04-05 OSSR WAN
//xxx            (rfscfrec1and2.ossr_store == ' ')) {                // SDH 04-04-05 OSSR WAN
//xxx
//xxx            //If nothing has been counted then don't submit it  // SDH 10-01-05 OSSR WAN
//xxx            if ((strncmp(clolfrec.uiTotalItems, clolfrec.items_shopfloor, 3)==0) &&    // SDH 10-01-05 OSSR WAN //CSk 12-03-2012 SFA
//xxx                (strncmp(clolfrec.uiTotalItems, clolfrec.items_backshop,  3)==0)) {    // SDH 10-01-05 OSSR WAN //CSk 12-03-2012 SFA
//xxx                pbDenyMsg = "No items counted in"               // SDH 10-01-05 OSSR WAN
//xxx                            " this list, List is"               // SDH 10-01-05 OSSR WAN
//xxx                            " not submitted.";                  // SDH 10-01-05 OSSR WAN
//xxx                //Else if one of the locations has not been counted // SDH 10-01-05 OSSR WAN
//xxx            } else if ((strncmp(clolfrec.uiTotalItems, clolfrec.items_shopfloor, 3) == 0) ||    // SDH 10-01-05 OSSR WAN //CSk 12-03-2012 SFA
//xxx                       (strncmp(clolfrec.uiTotalItems, clolfrec.items_backshop,  3) == 0)) {    // SDH 10-01-05 OSSR WAN //CSk 12-03-2012 SFA
//xxx                pbDenyMsg = "You must count both"               // SDH 10-01-05 OSSR WAN
//xxx                            " backshop and shop"                // SDH 10-01-05 OSSR WAN
//xxx                            " floor locations.";                // SDH 10-01-05 OSSR WAN
//xxx            }                                                   // SDH 10-01-05 OSSR WAN
//xxx
//xxx            //Else if an offline OSSR store                         // SDH 10-01-05 OSSR WAN
//xxx        } else if (rfscfrec1and2.ossr_store == 'Y') {           // SDH 10-01-05 OSSR WAN
//xxx            //If no shop floor items counted then deny the commit//SDH 10-01-05 OSSR WAN
//xxx            if (strncmp(clolfrec.uiTotalItems, clolfrec.items_shopfloor, 3)==0) {    // SDH 10-01-05 OSSR WAN //CSk 12-03-2012 SFA
//xxx                pbDenyMsg = "OSSR Store. You must"              // SDH 10-01-05 OSSR WAN
//xxx                            " count the shopfloor."             // SDH 10-01-05 OSSR WAN
//xxx                            " LIST NOT SUBMITTED.";             // SDH 10-01-05 OSSR WAN
//xxx            }                                                   // SDH 10-01-05 OSSR WAN
//xxx
//xxx            //Else if an OSSR WAN store                             // SDH 10-01-05 OSSR WAN
//xxx        } else if (rfscfrec1and2.ossr_store == 'W') {           // SDH 10-01-05 OSSR WAN
//xxx            //If no shop floor items OR no back shop locations counted then deny the commit  // SDH 10-01-05 OSSR WAN
//xxx            if ((strncmp(clolfrec.uiTotalItems, clolfrec.items_shopfloor, 3)==0)    ||    // SDH 10-01-05 OSSR WAN    //CSk 12-03-2012 SFA
//xxx                ((strncmp(clolfrec.uiTotalItems, clolfrec.items_backshop, 3)==0) &&    // SDH 08-04-05 OSSR WAN       //CSk 12-03-2012 SFA
//xxx                 (strncmp(clolfrec.uiTotalItems, clolfrec.items_ossr, 3)==0)       )) {    // SDH 08-04-05 OSSR WAN   //CSk 12-03-2012 SFA
//xxx                pbDenyMsg = "OSSR Store. You must"              // SDH 10-01-05 OSSR WAN
//xxx                            " count the shopfloor"              // SDH 10-01-05 OSSR WAN
//xxx                            " & a stock location";              // SDH 10-01-05 OSSR WAN
//xxx            }                                                   // SDH 10-01-05 OSSR WAN
//xxx        }                                                       // SDH 10-01-05 OSSR WAN
//xxx    }                                                           // SDH 10-01-05 OSSR WAN
//xxx
//xxx    //Update list status as appropriate                         // SDH 04-04-05 OSSR WAN
//xxx    if ((pCLX->commit_flag[0] == 'Y') &&                        // SDH 04-04-05 OSSR WAN
//xxx        (pbDenyMsg == NULL)) {                                  // SDH 04-04-05 OSSR WAN
//xxx        clolfrec.bListStatus = 'P';                             //CSk 12-03-2012 SFA
//xxx    } else {                                                    // (commit)
//xxx        clolfrec.bListStatus = 'N';                             //CSk 12-03-2012 SFA
//xxx    }                                                           // endif (commit)
//xxx
//xxx    // Update CLOLF list header
//xxx    rc2 = ClolfWrite(list_id - 1L, __LINE__);
//xxx    if (rc2 <= 0L) {
//xxx        prep_nak("ERRORUnable to write to CLOLF. Check appl event logs");
//xxx        return;
//xxx    }

//xxx    //If we're denying the commit then NAK                      // SDH 10-01-05 OSSR WAN
//xxx    if (pbDenyMsg != NULL) {                                    // SDH 10-01-05 OSSR WAN
//xxx        prep_nak(pbDenyMsg);                                    //SDH 23-Aug-2006 Planners
//xxx        return;                                                  // SDH 10-01-05 OSSR WAN
//xxx    }                                                           // SDH 10-01-05 OSSR WAN

//xxx    // All counts to be written directly to the STKMQ                       //CSk 12-03-2012 SFA
//xxx    if (pCLX->commit_flag[0] == 'Y') {                          // SDH 04-04-05 OSSR WAN
//xxx        //sprintf( sbuf, "%03ld", list_id );                                //CSk 12-03-2012 SFA
//xxx        //LONG rc = start_background_app("ADX_UPGM:RFSBG.286",              //CSk 12-03-2012 SFA
//xxx        //                          sbuf, "Processing Counts - Starting");  //CSk 12-03-2012 SFA
//xxx        //if (rc < 0) {                                                     //CSk 12-03-2012 SFA
//xxx        //    prep_nak("Unable to start RFSBG. Please try again shortly."); //CSk 12-03-2012 SFA
//xxx        //    return;                                                       //CSk 12-03-2012 SFA
//xxx        //}                                                                 //CSk 12-03-2012 SFA
//xxx    }
//xxx
//xxx    // v4.0 START
//xxx    // Audit
//xxx    sprintf( msg, "%04ld", list_id );
//xxx    memcpy( ((LRTLG_CLX *)dtls)->list_id, msg , 4 );
//xxx    memcpy( ((LRTLG_CLX *)dtls)->commit_flag,
//xxx            (((LRT_CLX *)(inbound))->commit_flag), 1 );
//xxx    memset( ((LRTLG_CLX *)dtls)->resv, 0x00, 7 );
//xxx    lrt_log( LOG_CLX, hh_unit, dtls );
//xxx    // v4.0 END
//xxx
//xxx    prep_ack("");                                               //SDH 23-Aug-2006 Planners
//xxx
//xxx}


// ------------------------------------------------------------------------------------
//
// CLF - Count Lists - Sign Off Session
//
//
//
// ------------------------------------------------------------------------------------

void CountsSignOffSession(char* inbound) {

    UNUSED(inbound);

    if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
    UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

    ClilfClose( CL_SESSION );
    ClolfClose( CL_SESSION );

    //Do both just in case                                          // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_LAB );                           // SDH 08-05-2006 Bug fix
    process_workfile( hh_unit, SYS_GAP );                           // SDH 08-05-2006 Bug fix

    prep_ack("");                                                   //SDH 23-Aug-2006 Planners

}


void WriteToFile(char *inbound) {                              // BMG 1.4 03-0-2008

    int iLoop;
    LONG WRFfnum;
    LONG lDataLength;

    LRT_WRF* pWRF = (LRT_WRF*)inbound;

    /*Convert spaces to nulls in the path */
    for (iLoop=0;iLoop<64;iLoop++) {
        if (pWRF->anFilePath[iLoop] == 32) {pWRF->anFilePath[iLoop] = 0;}
    }

    if (memcmp(pWRF->cCreateFile, "Y", 1)==0) {
       WRFfnum = s_delete( 0, pWRF->anFilePath );
       if ( (WRFfnum != 0) && ((WRFfnum&0xFFFF) != 0x4010)) {
           /* Error deleting file and NOT file not found error*/
           prep_nak("Unable to delete file");
           return;
       }
       WRFfnum=s_create( O_FILE, 0x2014, pWRF->anFilePath, 1, 0x0FFF, 0);
       if (WRFfnum < 0) {
           prep_nak("Unable to create file");
           return;
       }

    } else {
        WRFfnum=s_open(0x2014, pWRF->anFilePath);
        if ( (WRFfnum&0xFFFF) == 0x4010) {
            /*File not found so create it */
            WRFfnum=s_create( O_FILE, 0x2014, pWRF->anFilePath, 1, 0x0FFF, 0);
            if (WRFfnum < 0) {
                prep_nak("Unable to create file");
                return;
            }
        } else {
            if (WRFfnum < 0) {
                prep_nak("Unable to open file");
                return;
            }
        }
    }

    lDataLength = satol( pWRF->anDataLength, 4 );
    s_write( A_EOFOFF, WRFfnum, pWRF->abData, lDataLength, 0L );
    s_close(0, WRFfnum);
    prep_ack("");
}


typedef struct {                                     // 5-5-2004 PAB
   BYTE cmd[3];                                                  // 5-5-2004 PAB
   BYTE opid[3];                                                 // 5-5-2004 pab
   BYTE sus_items[481]; // 37 * EAN13                            // 5-5-2004 pab
}  LRT_SUS;                                                      // 5-5-2004 PAB
#define LRT_SUS_LTH sizeof(LRT_SUS)                              // 5-5-2004 PAB

typedef struct LRT_SAK_Txn {                                     // 5-5-2004 PAB
  BYTE cmd[3];                                                   // 5-5-2004 PAB
  BYTE till_id[3];                                               // 5-5-2004 PAB
  BYTE txn_num[4];                                               // 5-5-2004 PAB
} LRT_SAK;                                                       // 5-5-2004 PAB
#define LRT_SAK_LTH sizeof(LRT_SAK)                              // 5-5-2004 PAB

void suspend_transaction(char *inbound) {                              // BMG 1.6 13-08-2008 Moved here from trans02.
                                                                       // BMG 1.7 10-09-2008 Moved again from trans03 to trans04.
       int rc = 0;                                                     // PAB 24-5-2007
       URC usrrc = RC_OK;                                              // PAB 24-5-2007
       LONG rc2 = 0;                                                   // PAB 24-5-2007
       static SUSPT_REC susptrec;                                      // PAB 24-5-2007
       static LONG day, month, year;                                   // PAB 24-5-2007
       static WORD hour, min;                                          // PAB 24-5-2007
       static LONG sec;                                                // PAB 24-5-2007

       if (IsStoreClosed()) return;                                    // SDH 26-11-04 CREDIT CLAIM
       if (IsHandheldUnknown()) return;                                // SDH 26-11-04 CREDIT CLAIM
       UpdateActiveTime();                                             // SDH 26-11-04 CREDIT CLAIM

       // if there are no barcodes in the transaction then launch      // 5-5-04 PAB
       // the report and ack the HHT                                   // 5-5-04 PAB
       // 5-5-04 PAB
       //if (strlen( ((LRT_SUS *)(inbound))->sus_items )==0) {         // 5-5-04 PAB
       if (((LRT_SUS*)(inbound))->sus_items[0] == 0) {                 // 19-05-05 SDH
           //disp_msg("Start Suspended Transaction Report");           // 5-5-04 PAB
           rc = start_background_app( "ADX_UPGM:RFSUSRPT.286", "",     // 01-07-05 SDH
                                 "On Demand - Suspended Txn Report" ); // 5-5-04 PAB
           if (rc < 0) {
               prep_nak("Unable to start RFSUSRPT. Please try again "  // SDH 27-Sep-2006 Bug fix
                        "shortly.");                                   // SDH 27-Sep-2006 Bug fix
           } else {                                                    // SDH 27-Sep-2006 Bug fix
               prep_ack("");                                           // SDH 23-Aug-2006 Planners
           }                                                           // SDH 27-Sep-2006 Bug fix

           return;                                                     // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       // 5-5-04 PAB
       // Get RF attributes from control file                          // 5-5-04 PAB
       usrrc = RfscfOpen();                                           // 5-5-04 PAB
       if (usrrc != RC_OK) {                                             // 5-5-04 PAB
           prep_nak("ERRORUnable to open RFSCF file. "                 // 5-5-04 PAB
                     "Please phone help desk.");                       // 5-5-04 PAB
           return;                                                     // 5-5-04 PAB
       }                                                               // 5-5-04 PAB

       rc2 = RfscfRead(0L, __LINE__);                                   // Streamline SDH 17-Sep-2008
       rc2 = RfscfRead(1L, __LINE__);                                   // Streamline SDH 17-Sep-2008

       rfscfrec1and2.qbust_next_txn++;                                 // 16-11-04 SDH
       rfscfrec1and2.qbust_txn_cnt++;                                  // 16-11-04 SDH

       rc2 = RfscfUpdate(__LINE__);                                    // 03-01-05 SDH
       if (rc2<=0L) {                                                  // 03-01-05 SDH
           prep_nak("ERRORUnable to "                                  // SDH 23-Aug-2006 Planners
                     "write to RFSCF. "                                // 03-01-05 SDH
                     "Check appl event logs" );                        // 03-01-05 SDH
           return;                                                      // 03-01-05 SDH
       }                                                               // 03-01-05 SDH

       // 5-5-04 PAB
       usrrc = RfscfClose( CL_SESSION );                               // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       usrrc = open_suspt();                                           // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       if (usrrc!=RC_OK) {                                             // 5-5-04 PAB
           log_event101(rc2, SUSPT_REP, __LINE__);                     // 5-5-04 PAB
           prep_nak("ERRORUnable to open "                             // SDH 23-Aug-2006 Planners
                     "EALSUSPT file. "                                 // 5-5-04 PAB
                     "Check appl event logs" );                        // 5-5-04 PAB
           return;                                                      // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       if (usrrc == RC_OK) {                                           // 5-5-04 PAB
           sysdate( &day, &month, &year, &hour, &min, &sec );          // 5-5-04 PAB
           year = (year - 2000);                                       // 5-5-04 PAB
           sprintf( sbuf, "%02ld%02ld%02ld%02d%02d%02d",               // 5-5-04 PAB
                    day, month, year, hour, min, sec );                // 5-5-04 PAB
           // pack 0 is even length                                    // 5-5-04 PAB
           pack( susptrec.day_time, 6, sbuf, 12, 0);                   // 5-5-04 PAB
           sprintf( sbuf, "%03d", rfscfrec1and2.qbust_term );          // 16-11-04 SDH
           // pack 1 is odd length and fill with zeros                 // 5-5-04 PAB
           pack(susptrec.terminal, 2, sbuf, 4, 1);                     // 5-5-04 PAB
           sprintf( sbuf, "%04d", rfscfrec1and2.qbust_next_txn );      // 16-11-04 SDH
           pack(susptrec.transnum, 2, sbuf, 4, 0);                     // 5-5-04 PAB
           susptrec.sequence = 0;                                      // 5-5-04 PAB
           memcpy(susptrec.operator_id,                                // 5-5-04 PAB
                  ((LRT_SUS *)(inbound))->opid,3);                     // 5-5-04 PAB
           memcpy(susptrec.reason, "9" ,1);                            // 5-5-04 PAB
           memcpy(susptrec.EAN_data,                                   // 5-5-04 PAB
                  ((LRT_SUS *)(inbound))->sus_items, 481);             // 5-5-04 PAB
                                                                       // 5-5-04 PAB
           rc2 = s_write( 0, suspt.fnum,                               // 5-5-04 PAB
                          (void *)&susptrec, SUSPT_RECL, 0L );         // 5-5-04 PAB
           if (rc2<=0L) {                                              // 5-5-04 PAB
               if (debug) {                                            // 5-5-04 PAB
                   sprintf(msg, "Err-W to SUSPT. RC:%08lX", rc2);      // 5-5-04 PAB
                   disp_msg(msg);                                      // 5-5-04 PAB
               }                                                       // 5-5-04 PAB
               prep_nak("W-Unable to Suspended "                       //SDH 23-Aug-2006 Planners
                         "Transaction. Process "                       // 5-5-04 PAB
                         "Customer manually.   " );                    // 5-5-04 PAB
               memcpy(((LRTLG_SUS *)dtls)->fail, "Y", 1);              // 5-5-04 PAB
               memcpy(((LRTLG_SUS *)dtls)->txnid, sbuf ,4);            // 5-5-04 PAB
               lrt_log(LOG_SUS, hh_unit, dtls );                       // 5-5-04 PAB
               return;                                                  // 5-5-04 PAB
                                                                       // 5-5-04 PAB
           } else {                                                    // 5-5-04 PAB
               if (debug) {                                            // 5-5-04 PAB
                   disp_msg("WRITE EALSUSPT OK");                      // 5-5-04 PAB
               }                                                       // 5-5-04 PAB
           }                                                           // 5-5-04 PAB
           // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB
       usrrc = close_suspt( CL_SESSION );                              // 5-5-04 PAB
                                                                       // 5-5-04 PAB
       if (usrrc == RC_OK) {                                           // 5-5-04 PAB
           memcpy( lrtp[hh_unit]->user,                                // 5-5-04 PAB
                   ((LRT_SUS *)(inbound))->opid, 3 );                  // 5-5-04 PAB
           // Get time                                                 // 5-5-04 PAB
           sysdate( &day, &month, &year, &hour, &min, &sec );          // 5-5-04 PAB
           // Prepare SAK                                              // 5-5-04 PAB
           memcpy( ((LRT_SAK *)out)->cmd, "SAK", 3 );                  // 5-5-04 PAB
           sprintf( sbuf, "%03d", rfscfrec1and2.qbust_term );          // 16-11-04 SDH
           memcpy( ((LRT_SAK *)out)->till_id, sbuf , 3 );              // 5-5-04 PAB
           sprintf( sbuf, "%04d", rfscfrec1and2.qbust_next_txn );      // 16-11-04 SDH
           memcpy( ((LRT_SAK *)&out)->txn_num, sbuf, 4 );              // 5-5-04 PAB
           out_lth = LRT_SAK_LTH;                                      // 5-5-04 PAB
           // Audit                                                    // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->fail, "N", 1);                 // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->txnid, sbuf ,4);               // 5-5-04 PAB
           lrt_log( LOG_SUS, hh_unit, dtls );                          // 5-5-04 PAB
       } else {                                                        // 5-5-04 PAB
           // User not authorised                                      // 5-5-04 PAB
           prep_nak("Unable to Suspended "                             //SDH 23-Aug-2006 Planners
                     "Transaction. Process "                           // 5-5-04 PAB
                     "Customer manually. " );                          // 5-5-04 PAB
           // Audit                                                    // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->fail, "Y", 1);                 // 5-5-04 PAB
           memcpy( ((LRTLG_SUS *)dtls)->txnid, sbuf ,4);               // 5-5-04 PAB
           lrt_log( LOG_SUS, hh_unit, dtls );                          // 5-5-04 PAB
       }                                                               // 5-5-04 PAB
       // 5-5-04 PAB

}

typedef struct {                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE cmd[3];                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE opid[3];                                                                       // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE list_id[3];                                                                    // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE seq[3];                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
} LRT_MSA;                                                                              // BMG 12-8-2008 Multi-Sited Counts 1.5
#define LRT_MSA_LTH sizeof(LRT_MSA)                                                     // BMG 12-8-2008 Multi-Sited Counts 1.5

typedef struct {                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE cmd[3];                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE opid[3];                                                                       // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE list_id[3];                                                                    // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE seq[3];                                                                        // BMG 12-8-2008 Multi-Sited Counts 1.5
    BYTE abShelfCount[132]; //Location count quantities X 32 + other                    // BMG 12-8-2008 Multi-Sited Counts 1.5 //CSk 12-03-2012 SFA
    BYTE abFillQty[132];    //location fill quantities  X 32 + other                    // BMG 12-8-2008 Multi-Sited Counts 1.5 //CSk 12-03-2012 SFA
} LRT_MSB;                                                                              // BMG 12-8-2008 Multi-Sited Counts 1.5
#define LRT_MSB_LTH sizeof(LRT_MSB)                                                     // BMG 12-8-2008 Multi-Sited Counts 1.5

void PlistGetItemMultiSite(char *inbound) {                                             // SDH 20-May-2009 Model Day

   LRT_MSA* pMSA = (LRT_MSA*)inbound;
   LRT_MSB* pMSB = (LRT_MSB*)out;
   LONG rc;
   int i;

   //Build the key and attempt the read
   memcpy( plldbrec.list_id, pMSA->list_id, 3 );
   memcpy(plldbrec.seq, pMSA->seq, 3);

   //Attempt read of PLLDB
   //If record not found return error - we should only be sent known list numbers
   rc = ReadPlldbLog(__LINE__, LOG_CRITICAL);
   if (rc<=0L) {
       if (rc == RC_DATA_ERR) {
           prep_nak("ERRORUnable to read from PLLDB. Check appl event logs" );
       } else {
           prep_nak("ERRORUndefined 7. Check appl event logs" );
       }
       return;
   }

   // return MSB transaction
   memcpy(pMSB->cmd, "MSB", sizeof(pMSB->cmd));
   memcpy(pMSB->opid, pMSA->opid, sizeof(pMSA->opid));
   memcpy(pMSB->list_id, pMSA->list_id, sizeof(pMSA->list_id));
   memcpy(pMSB->seq, pMSA->seq, sizeof(pMSA->seq));

   for (i=0;i<33;i++)                                                                   //CSk 12-03-2012 SFA
   {
       unpack(pMSB->abShelfCount+(i*4), 4, plldbrec.aMS_details[i].ms_loc_count, 2, 0 );
       unpack(pMSB->abFillQty+(i*4), 4, plldbrec.aMS_details[i].ms_fill_qty, 2, 0 );
   }

   out_lth = LRT_MSB_LTH;

}

void ALR_Request(char *inbound) {  //Launch ACTBUILD on receipt of ALR // BMG 1.7 10-09-2008 MC70

    LRT_ALR* pALR = (LRT_ALR*)inbound;
    LONG rc;

    if (memcmp(pALR->application, "ACTBUILD", sizeof(pALR->application)) == 0) {

        //Firstly see if ACTBUILD is already running by trying to create it's semaphore pipe
        spipe.fnum = s_create(O_FILE, SPIPE_CFLAGS, SPIPE, 1, 0x0FFF, 1);
        if (spipe.fnum <= 0) {
            //Failed to create pipe so program is already running
            prep_ack( " ACTBUILD is already running" );
        } else {
            //ACTBUILD is not running so start it up
            s_close(0,spipe.fnum);
            rc = start_background_app("ADX_UPGM:ACTBUILD.286", "", "Starting ACTBUILD");
            if (rc != 0) {
                sprintf( sbuf, " Failed to launch ACTBUILD");
                prep_nak(sbuf);
            } else {
                prep_ack( " ACTBUILD has been started" );
            }
        }
    } else {
        sprintf( sbuf, "No application identifier of %8s found", pALR->application);
        prep_nak(sbuf);
    }

}

void ANK_Request(char *inbound) {  //Convert internal ANK to NAK and send. // BMG 8 17-12-2009 RF Stabilisation

    LRT_ANK* pANK = (LRT_ANK*)inbound;
    int i;

    memset(sbuf, 00, sizeof(sbuf));
    memcpy(sbuf, pANK->abText+3, LRT_ANK_LTH);
    prep_nak(sbuf);
    i=LRT_ANK_LTH;
    i=sizeof(sbuf);
    sprintf(sbuf, "%d", i);
}


//------------------------------------------------------------
void PODLOG_Request(char *inbound) {  //Populate PODLOG and PODOK records           //CSk 15-04-2010 POD Logging

    int i;
    LONG lRc;
    LRT_PODLOG* pPODLOG = (LRT_PODLOG*)inbound;

    //---------------------------
    //          PODLOG
    //---------------------------
    // Setup and write record to end of sequential PODLOG file
    PodlogInitialiseRec();

    memcpy(podlogrec.abMACaddress, pPODLOG->baMAC, sizeof(podlogrec.abMACaddress));
    memcpy(podlogrec.abIPaddress,  pPODLOG->baIP,  sizeof(podlogrec.abIPaddress ));

    switch (pPODLOG->cAction) {
        case 'L':
            memcpy(podlogrec.abAction, "TFTP Load", sizeof(podlogrec.abAction));
            break;
        case 'S':
            memcpy(podlogrec.abAction, "TFTP Send", sizeof(podlogrec.abAction));
            break;
        case 'F':
            memcpy(podlogrec.abAction, " FTP Load", sizeof(podlogrec.abAction));
            break;
        case 'P':       // blank message
            memcpy(podlogrec.abAction, " FTP Send", sizeof(podlogrec.abAction));
            break;
        default:
            memcpy(podlogrec.abAction, "*UNKNOWN*", sizeof(podlogrec.abAction));
            break;
    }

    memcpy(podlogrec.abFilename, pPODLOG->baFileName, sizeof(podlogrec.abFilename));

    switch (pPODLOG->cStatus) {
        case 'S':                                                            //CSk 03-08-2010 Defect 4478
            memcpy(podlogrec.abStatus, "Start", sizeof(podlogrec.abStatus));
            break;
        case 'E':                                                            //CSk 03-08-2010 Defect 4478
            memcpy(podlogrec.abStatus, "End  ", sizeof(podlogrec.abStatus)); //CSk 03-08-2010 Defect 4478
            break;
        case 'X':                                                            //CSk 03-08-2010 Defect 4478
            memcpy(podlogrec.abStatus, "FAIL ", sizeof(podlogrec.abStatus)); //CSk 03-08-2010 Defect 4478
            break;
        default:
            memcpy(podlogrec.abStatus, "?????", sizeof(podlogrec.abStatus));
            break;
    }

    memcpy(podlogrec.abReason, pPODLOG->baReason, sizeof(podlogrec.abReason));
    memcpy(podlogrec.abText,   pPODLOG->baText,   sizeof(podlogrec.abText  ));

    if (debug) {
        sprintf(msg, "WR PODLOG :");
        disp_msg(msg);
        dump( (BYTE *)&podlogrec, sizeof(PODLOG_REC) );
    }
    lRc = s_write( A_EOFOFF, podlog.fnum, (void *)&podlogrec,
                   sizeof(PODLOG_REC), 0L );
    if (lRc<=0) {
        log_event101(lRc, PODLOG_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-W to PODLOG. RC:%08lX", lRc);
            disp_msg(msg);
        }
        prep_nak("ERRORUnable to write to PODLOG. Check appl event logs");
        return;
    }

    //---------------------------
    //           PODOK
    //---------------------------
    memcpy(podokrec.abMACaddress, pPODLOG->baMAC, sizeof(podokrec.abMACaddress));

    lRc = PodokRead(__LINE__);

    if (lRc <= 0L) {
        if ((lRc&0xFFFF) == 0x06C8) {   // Record not found
            //Create PODOK record for new device
            PodokInitialiseRec(FALSE);  //Initialise fields
        } else {
            prep_nak("ERROR Unable to read PODOK - check event logs");
            return;
        }
    } else {
        PodokInitialiseRec(TRUE);   // Update Controller Date & Time only
    }

    memcpy(podokrec.abIPaddress,  pPODLOG->baIP,  sizeof(podokrec.abIPaddress ));

    sscanf( pPODLOG->baSynFileNum, "%2d", &i ); //Get SYNCTRL file no.

    if (i < 0  ||  i >= MAX_POD_FILES) {
        if (debug) {
            disp_msg("Err-W to PODOK. Invalid CSV file specified");
        }
        prep_nak("ERROR Unable to write PODOK. Invalid CSV file specified");
        return;
    }

    memcpy(podokrec.aSeg[i].abDeviceDate, pPODLOG->baDate+2,  4); //MMDD from YYMMDD
    memcpy(podokrec.aSeg[i].abDeviceTime, pPODLOG->baTime,    4); //HHMM from HHMMSS
    podokrec.aSeg[i].bFileStatus = pPODLOG->cStatus;

    //write keyed record etc
    lRc = PodokWrite(__LINE__);

    if (lRc <= 0L) {
        log_event101(lRc, PODOK_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-W to PODOK. RC:%08lX", lRc);
            disp_msg(msg);
        }
        prep_nak("ERROR Unable to write PODOK. Check appl event log");
        return;
		
    }
    prep_ack( "" );
}


typedef struct {                                                           // BMG 8 17-12-2009 RF Stabilisation
   BYTE cmd[3];
   BYTE opid[3];
   BYTE pass[3];
   BYTE abAppID[3];
   BYTE AppVer[4];
   BYTE MAC[12];
   BYTE DevType[1];
   BYTE IPADDR[15];
   BYTE FreeMem[8];
   BYTE abListId[3];
} LRT_RCN;
#define LRT_RCN_LTH sizeof(LRT_RCN)

void Reconnect(char* inbound) {                                            // BMG 8 17-12-2009 RF Stabilisation

    LRT_RCN* pRCN = (LRT_RCN*)inbound;
    int i;
    int found = 0;
    URC usrrc;
    long rc;
    long rc2;    //Picking list locked KK 16-Oct-2012
    WORD wAppID;
    WORD wListId;

    wAppID = satoi(pRCN->abAppID, sizeof(pRCN->abAppID));

    //Try and match MAC to an existing session that is still signed in
    for (i=0;i<MAX_UNIT;i++) {
        if (lrtp[i] != NULL) {
            if ( (memcmp(lrtp[i]->MAC, pRCN->MAC, sizeof(pRCN->MAC)) == 0) && (lrtp[i]->state == ST_LOGGED_ON) ) {
                //If a different session, move all data across
                if (hh_unit != i) {
                    //Allocate a new entry for this unit
                    usrrc = alloc_lrt_table( hh_unit );
                    if (usrrc<RC_IGNORE_ERR) {
                        prep_nak("Unable to reconnect - please try again later" );
                        return;
                    }
                    //Copy all the data from the found entry i to the current entry hh_unit then destroy i
                    memcpy(lrtp[hh_unit], lrtp[i], sizeof(ACTIVE_LRT));
                    dealloc_lrt_table(i);
                }
                i = MAX_UNIT;
                found = 1;
            }
        }
    }

    if (found == 1) {
        disp_msg ("Reconnect - Found a matching MAC entry in LRT_TABLE - doing checks");
        UpdateActiveTime();

        if (IsStoreClosed()){
             disp_msg ("STORE IS CLOSED - Unable to reconnect");
             dealloc_lrt_table(hh_unit);
             return;
        }

        PgfClose( CL_ALL );

        //Verify that batch suite is not running
        usrrc = process_rfok();
        if (usrrc<=RC_DATA_ERR) {
            disp_msg ("Error processing RFOK File - Unable to reconnect");
            prep_nak("Unable to reconnect - please try again later" );
            dealloc_lrt_table(hh_unit);
            return;
        }

        //Verify that recalls batch suite is not running
        usrrc = process_recok();
        if (usrrc != RC_OK) {
            disp_msg ("Error processing RECOK File - Unable to reconnect");
            prep_nak("Unable to reconnect - please try again later" );
            dealloc_lrt_table(hh_unit);
            return;
        }

        //Shelf Management
        //RFMAINT will be blocked at report browser level to allow
        //the rest of the application to be used
        if ((wAppID == 0) || (wAppID == 1)) {
            if ((rfokrec.rfaudit == 'S') || (rfokrec.rfpikmnt == 'S')) {
                disp_msg ("File Maintenance in progress - Unable to reconnect");
                prep_nak("Unable to reconnect - please try again later" );
                dealloc_lrt_table(hh_unit);
                return;
            }
        //Credit Claim / UOD
        } else if (wAppID == 2) {
            if (rfokrec.rfccmnt == 'S') {
                disp_msg ("File Maintenance in progress - Unable to reconnect");
                prep_nak("Unable to reconnect - please try again later" );
                dealloc_lrt_table(hh_unit);
                return;
            }
        }

        //Check POG status here after reading is POGs are active from the RFSCF.
        if (wAppID != 5) {
            usrrc = PogokOpen();
            if (usrrc < RC_OK) {
                disp_msg ("Error opening POGOK - Unable to reconnect");
                prep_nak("Unable to reconnect - please try again later" );
                dealloc_lrt_table(hh_unit);
                return;
            }
            rc = PogokRead(0, __LINE__);
            PogokClose(CL_SESSION);
            if (rc > 0) {
                if (pogokrec.cSRP04 == 'S' ||
                    pogokrec.cSRP05 == 'S' ||
                    pogokrec.cSRP06 == 'S' ||
                    pogokrec.cSRP07 == 'S' ||
                    pogokrec.cSRP10 == 'S' ) {
                    disp_msg ("Planner maintenance in progress - Unable to reconnect");
                    prep_nak("Unable to reconnect - please try again later" );
                    dealloc_lrt_table(hh_unit);
                    return;
                }
            }
        }
    } else {
        disp_msg ("Reconnect - No matching MAC entry in LRT_TABLE - Signing on again");

        //Send to SOR
        SignOn(inbound, CMD_SOR_LTH3);
        if ( memcmp( (BYTE *)out, "SNR", 3) ) {
            disp_msg ("Reconnect - Failed to Sign On Again");
            prep_nak("Unable to reconnect - please try again later");
            dealloc_lrt_table(hh_unit);
            return;
        }
    }

    if ( (memcmp(pRCN->abListId, "000", 3)==0) || (wAppID !=0 && wAppID !=1) ) {
        //We have a zero list ID or a non-Shelf Management Application ID
        prep_ack( "You have been successfully reconnected to your previous session - please continue" );
    } else {
        usrrc = open_pllol();
        if (usrrc<=RC_DATA_ERR) {
            disp_msg ("Reconnect - Error opening PLLOL - Unable to continue previous session");
            prep_nak("It is not possible to continue the previous session. Please continue");
            dealloc_lrt_table(hh_unit);
            return;
        } else {
            wListId = satoi(pRCN->abListId, 3);
            rc = ReadPllol(wListId, __LINE__);
            if (rc <= 0L) {
                disp_msg ("Reconnect - Error reading PLLOL - Unable to continue previous session");
                prep_nak("It is not possible to continue the previous session. Please continue" );
                close_pllol( CL_SESSION );
                dealloc_lrt_table(hh_unit);
                return;
            } else {
                WORD wLastPicker = satoi(pllolrec.picker, 3); //Picking list locked KK 16-Oct-2012
                if (wAppID == 0) {
                 //Condition to verify if Last picker id is 0,the creator id is same as the sign in id and the list status is "U"

                 if ((wLastPicker == 0 ) && ( memcmp(pllolrec.creator, pRCN->opid, sizeof(pRCN->opid))== 0) && (pllolrec.list_status[0] == 'U' )) //Picking list locked KK 16-Oct-2012
                 {    pllolrec.list_status[0] = 'A' ;                 //Picking list locked KK 16-Oct-2012
                      rc2 = WritePllol(wListId, __LINE__);           //Picking list locked KK 16-Oct-2012
                      if (rc2<=0L)                                   //Picking list locked KK 16-Oct-2012
                      {
                          prep_nak("ERRORUnable to write to PLLOL. "
                                   "Check appl event logs" );         //Picking list locked KK 16-Oct-2012
                      }
                      else
                      {
                          prep_ack( "List still available - please continue" ); //Picking list locked KK 16-Oct-2012
                      }
                      close_pllol( CL_SESSION );                //Picking list locked KK 16-Oct-2012
                 } else if ( memcmp(pllolrec.creator, pRCN->opid, sizeof(pRCN->opid)) || pllolrec.list_status[0] == 'P'    // BMG 11 25-06-2010 // BMG 12 30-06-2010
                         || pllolrec.list_status[0] == 'U' || pllolrec.list_status[0] == 'X') {                        // BMG 11 25-06-2010 // BMG 12 30-06-2010
                        //List is not in the right state for Shelf Monitor/Fast Fill
                        //Requesting operator doesn't match the list creator, of the list is marked P.                 // BMG 11 25-06-2010
                        //if (pllolrec.list_status[0] == 'A') {                                                                             // BMG 12 30-06-2010
                        //    prep_nak("List cannot be resumed as it has been marked as available for picking. Please create a new list");  // BMG 12 30-06-2010
                        //} else {                                                                                                          // BMG 12 30-06-2010
                        //    prep_nak("List cannot be resumed as it is being picked by another user");                                     // BMG 12 30-06-2010
                        //}                                                                                                                 // BMG 12 30-06-2010
                        prep_nak("It is not possible to continue the previous session. Please continue" );                                  // BMG 12 30-06-2010
                        disp_msg ("Reconnect - Bad list status for Shelf Monitor / Fast Fill - Unable to continue previous session");
                        dealloc_lrt_table(hh_unit);
                        return;
                    } else {
                        //List status OK - force to L though
                        //rc = ReadPllolLock(wListId, __LINE__);                                                                            // BMG 12 30-06-2010
                        //pllolrec.list_status[0] = 'L';                                                                                    // BMG 12 30-06-2010
                        //WritePllolUnlock(wListId, __LINE__);                                                                              // BMG 12 30-06-2010
                        prep_ack( "List still available - please continue" );
                        close_pllol( CL_SESSION );
                    }
                } else { //Application ID must be 1
                    if ( pllolrec.list_status[0] != 'A' || memcmp(pllolrec.picker , pRCN->opid, sizeof(pRCN->opid)) ) { // BMG 11 25-06-2010 // BMG 12 30-06-2010
                        //List is not in the right state for Picking Lists
                        //Picker doesn't match operator or list is locked                                               // BMG 11 25-06-2010
                        if (pllolrec.list_status[0] == 'P') {
                            prep_nak("List cannot be resumed as it is being picked by another user");
                        } else {
                            prep_nak("It is not possible to continue the previous session. Please continue");
                        }
                        disp_msg ("Reconnect - Bad list status for Picking Lists - Unable to continue previous session");
                        dealloc_lrt_table(hh_unit);
                        return;
                    } else {
                        //List status OK - force to A though
                        //rc = ReadPllolLock(wListId, __LINE__);                                                        // BMG 11 25-06-2010
                        //pllolrec.list_status[0] = 'A';                                                                // BMG 11 25-06-2010
                        //WritePllolUnlock(wListId, __LINE__);                                                          // BMG 11 25-06-2010
                        prep_ack( "List still available - please continue" );
                        close_pllol( CL_SESSION );
                    }
                }
            }
        }
    }
}
